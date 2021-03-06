﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using System.Threading.Tasks;

using LayoutManager.Model;

#pragma warning disable IDE0051, IDE0060
namespace LayoutManager {

    /// <summary>
    /// Actions that the user can perform in the launch dialog
    /// </summary>
    public enum LaunchAction {
        NewLayout, OpenLayout, Exit
    }

    public enum FrameWindowCommand {
        CloseLayout, CloseWindow, NewWindow,
    }

    public class FrameWindowAction {
        public LayoutFrameWindow FrameWindow { get; }
        public FrameWindowCommand Command { get; }

        public FrameWindowAction(LayoutFrameWindow frameWindow, FrameWindowCommand command) {
            this.FrameWindow = frameWindow;
            this.Command = command;
        }
    }

    public class LayoutControllerImplementation : ApplicationContext, ILayoutController, ILayoutSelectionManager {
        string layoutFilename;
        readonly LayoutCommandManager commandManager;
        readonly List<LayoutSelection> displayedSelections = new List<LayoutSelection>();
        readonly LayoutSelectionWithUndo userSelection;
        readonly PreviewRouteManager previewRouteManager = new PreviewRouteManager();
        readonly LayoutModuleManager moduleManager;
        int trainsAnalysisPhaseCount = 0;       // How many command stations perform layout analysis phase
        int layoutDesignTimeActivationNesting = 0;
        bool showConnectTrackPowerWarning = true;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            try {
                Application.EnableVisualStyles();
                Application.Run(new LayoutControllerImplementation());
            }
            catch (TargetInvocationException ex) {
                Trace.WriteLine("Unhandled exception: " + ex.InnerException.Message);
                Trace.Write("At: " + ex.InnerException.StackTrace);
            }
        }

        public LayoutControllerImplementation() {
            LayoutController.Instance = this;

            moduleManager = new LayoutModuleManager();
            EventManager.Instance = new LayoutEventManager(moduleManager);
            commandManager = new LayoutCommandManager();

            userSelection = new LayoutSelectionWithUndo();
            userSelection.Display(new LayoutSelectionLook(System.Drawing.Color.Blue));

            #region Modules Initialization

            //
            // Initialize the module manager
            //
            moduleManager.DocumentFilename = CommonDataFolderName + Path.DirectorySeparatorChar + "ModuleManager.xml";

            //
            // dummy references to assemblies that contains only layout modules. This is needed, otherwise
            // those assemblies are not automatically referenced and the user will have to use the file/modules
            // menu to add them.
            //
            String s;

            s = LayoutManager.Logic.LayoutTopologyServices.TopologyServicesVersion;
            s = LayoutManager.Tools.ComponentEditingTools.ComponentEditingToolsVersion;
            s = LayoutManager.ControlComponents.ControlComponents.ControlComponentsVersion;

            // Initialize built in modules

            AssemblyName[] referenced = this.GetType().Assembly.GetReferencedAssemblies();

            // Load all the assemblies which are referenced by the application
            foreach (AssemblyName assemblyName in referenced) {
                Assembly refAssembly = Assembly.Load(assemblyName);

                moduleManager.LayoutAssemblies.Add(new LayoutAssembly(refAssembly));
            }

            moduleManager.LayoutAssemblies.Add(new LayoutAssembly(this.GetType().Assembly));

            // Add assemblies in the Modules directory
            string modulesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules");

            foreach (string moduleFilename in Directory.GetFiles(modulesDirectory, "*.dll")) {
                if (!moduleManager.LayoutAssemblies.IsDefined(moduleFilename))
                    moduleManager.LayoutAssemblies.Add(new LayoutAssembly(moduleFilename, LayoutAssembly.AssemblyOrigin.InModulesDirectory));
            }

            // Load assemblies that were defined by the user
            moduleManager.LoadState();

            EventManager.Instance.AddObjectSubscriptions(this);
            EventManager.Event(new LayoutEvent("modules-initialized", this));

            #endregion

            if (!LayoutModel.Instance.ReadModelXmlInfo(CommonDataFolderName)) {
                MessageBox.Show("The system wide definition files could not be found. Default definitions for " +
                    "fonts etc. will be used", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Wire areas collection events
            LayoutModel.Areas.AreaAdded += new EventHandler(Area_Added);
            LayoutModel.Areas.AreaRemoved += new EventHandler(Area_Removed);
            LayoutModel.Areas.AreaRenamed += new EventHandler(Area_Renamed);

            EventManager.Event(new LayoutEvent("initialize-event-interthread-relay", this));

            LauchLayoutManager();
        }

        #region Layout Manager flow

        private async void LauchLayoutManager() {
            var applicationState = new ApplicationStateInfo(CommonDataFolderName + Path.DirectorySeparatorChar + "ApplicationState.xml");
            var launchDialog = new Dialogs.LaunchDialog(applicationState.LayoutFilename);
            bool restoreDisplayState = true;
            bool forceDesignMode = true;

            launchDialog.Show();

            var action = await launchDialog.Task;

            switch (action) {

                case LaunchAction.Exit:
                    EventManager.Event(new LayoutEvent("terminate-event-interthread-relay", this));
                    ExitThreadCore();
                    return;

                case LaunchAction.NewLayout:
                    CreateNewModel(launchDialog.LayoutFilename);
                    restoreDisplayState = false;
                    break;

                case LaunchAction.OpenLayout:
                    LoadModel(launchDialog.LayoutFilename);

                    if (launchDialog.ResetToDefaultDisplayLayout)
                        restoreDisplayState = false;
                    if (launchDialog.UseLastOpenLayout)
                        forceDesignMode = false;
                    break;
            };

            LayoutDisplayState displayState;

            if (restoreDisplayState && File.Exists(LayoutDisplayStateFilename))
                displayState = new LayoutDisplayState(LayoutDisplayStateFilename);
            else
                displayState = new LayoutDisplayState();

            FrameWindows = displayState.FrameWindows;

            foreach (var frameWindow in FrameWindows)
                frameWindow.Show();

            if (displayState.ActiveWindowIndex >= 0)
                FrameWindows[displayState.ActiveWindowIndex].Activate();

            if (!forceDesignMode && displayState.OperationModeSettings != null) {
                await Task.Delay(500);
                EnterOperationModeRequest(displayState.OperationModeSettings);
            }
            else
                await EnterDesignModeRequest();

            await UserInteraction();

            // Save layout model if needed
            if (CommandManager.ChangeLevel != 0)
                SaveModel(LayoutFilename);

            applicationState.LayoutFilename = LayoutController.LayoutFilename;
            applicationState.Save();

            // Go back into design mode
            Trace.WriteLine("Before EnterDesignRequest");
            await EnterDesignModeRequest();
            Trace.WriteLine("After EnterDesignRequest");

            EventManager.Event(new LayoutEvent("terminate-event-interthread-relay", this));

            ExitThread();
        }

        void SaveDisplayState(IEnumerable<LayoutFrameWindow> frameWindows) {
            var displayState = new LayoutDisplayState(frameWindows);

            displayState.Save(LayoutDisplayStateFilename);
        }

        private async Task UserInteraction() {
            bool exit = false;

            Debug.Assert(FrameWindows.Count > 0);

            while (!exit) {
                var actionTask = await Task.WhenAny(from frameWindow in FrameWindows select frameWindow.Task);

                switch (actionTask.Result.Command) {
                    case FrameWindowCommand.CloseLayout:
                        actionTask.Result.FrameWindow.InitializeTask();

                        SaveDisplayState(FrameWindows);
                        FrameWindows.Remove(actionTask.Result.FrameWindow);

                        EventManager.Event(new LayoutEvent("close-all-frame-windows"));

                        // Wait until all frame windows are closed
                        await Task.WhenAll(from frameWindow in FrameWindows select frameWindow.Task);
                        exit = true;
                        break;

                    case FrameWindowCommand.CloseWindow:
                        if (FrameWindows.Count == 1) {      // Last window is about to be closed, save its state
                            SaveDisplayState(FrameWindows);
                            exit = true;
                        }
                        else
                            FrameWindows.Remove(actionTask.Result.FrameWindow);

                        break;

                    case FrameWindowCommand.NewWindow: {
                            actionTask.Result.FrameWindow.InitializeTask();

                            var newWindow = new LayoutFrameWindow();

                            newWindow.Show();
                            newWindow.Activate();
                            FrameWindows.Add(newWindow);
                        }
                        break;
                }
            }
        }

        #endregion

        #region Mode transition (design -> operational/simulation and back)

        #region Operation Mode

        public bool EnterOperationModeRequest(OperationModeParameters settings) {
            bool switchMode = true;

            Debug.Assert(settings.Simulation || settings.Phases == LayoutPhase.Operational);

            if (!settings.Simulation)
                settings.Phases = LayoutPhase.Operational;

            if (commandManager.ChangeLevel != 0)
                SaveModel(LayoutFilename);

            EventManager.Event(new LayoutEvent("clear-messages", this));

            if (!(bool)EventManager.Event(new LayoutEvent("check-layout", LayoutModel.Instance, true).SetPhases(settings.Phases))) {
                MessageBox.Show(ActiveFrameWindow, "Problems were detected in the layout. Please fix those problems before entering operation mode", "Layout Check Results",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                switchMode = false;
            }

            if (switchMode) {
                try {
                    // Switch to operation mode.
                    if (!IsOperationMode) {
                        if (layoutDesignTimeActivationNesting > 0) {
                            EndDesignTimeActivation();
                            layoutDesignTimeActivationNesting = 0;
                        }

                        EventManager.Event(new LayoutEvent("exit-design-mode", this));
                    }

                    commandManager.Clear();         // Clear undo stack

                    List<IModelComponentIsCommandStation> performTrainsAnalysis = new List<IModelComponentIsCommandStation>();

                    EventManager.Event(new LayoutEvent("query-perform-trains-analysis", this, performTrainsAnalysis));
                    trainsAnalysisPhaseCount = performTrainsAnalysis.Count;

                    if (trainsAnalysisPhaseCount > 0)
                        EventManager.Event(new LayoutEvent("begin-trains-analysis-phase", this));

                    OperationModeSettings = settings;
                    LayoutModel.ActivePhases = settings.Phases;

                    EventManager.Event(new LayoutEvent<OperationModeParameters>("prepare-enter-operation-mode", settings));

                    if (!(bool)EventManager.Event(new LayoutEvent("rebuild-layout-state", LayoutModel.Instance).SetPhases(settings.Phases))) {
                        if (MessageBox.Show(ActiveFrameWindow, "The layout design or locomotive collection were modified. The previous state could not be fully restored.\n\n" +
                            "Would you like to continue with the partially restored state?\n\nSelecting \"No\" will clear the state. In this case, you will " +
                            "have to indicate the locomotive positions again", "Locomotive state cannot be restored",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                            EventManager.Event(new LayoutEvent("clear-layout-state", LayoutModel.Instance));
                    }

                    EventManager.Event(new LayoutEvent<OperationModeParameters>("enter-operation-mode", settings));
                    LayoutModel.Instance.Redraw();

                }
                catch (Exception ex) {
                    EventManager.Event(new LayoutEvent(null, "add-error", null, "Could not enter operational mode - " + ex.Message));

                    ExitOperationModeRequest().Wait();
                    EnterDesignModeRequest().Wait();
                    switchMode = false;
                }
            }

            return switchMode;
        }

        public async Task ExitOperationModeRequest() {
            bool simulation = LayoutController.OperationModeSettings.Simulation;

            EventManager.Event(new LayoutEvent("exit-operation-mode", this));
            Trace.WriteLine("Before invoking exit-operation-mode-async");
            await Task.WhenAll(EventManager.AsyncEventBroadcast(new LayoutEvent("exit-operation-mode-async", this)));
            Trace.WriteLine("After invoking exit-operation-mode-async");

            if (simulation)
                await Task.Delay(200);      // Allow the emulator to process last command
        }

        [LayoutEvent("command-station-trains-analysis-phase-done")]
        private void trainsAnalysisPhaseDone(LayoutEvent e) {
            if (--trainsAnalysisPhaseCount == 0) {
                EventManager.Event(new LayoutEvent("end-trains-analysis-phase", this));
                EventManager.Event(new LayoutEvent("perform-trains-analysis", LayoutModel.Instance));
            }

            Debug.Assert(trainsAnalysisPhaseCount >= 0);
        }

        #endregion

        #region Design Mode 

        public async Task EnterDesignModeRequest() {
            if (IsOperationMode) {
                Trace.WriteLine("Before ExitOperationModeRequest");
                await ExitOperationModeRequest();
                Trace.WriteLine("After ExitOperationModeRequest");
            }

            layoutDesignTimeActivationNesting = 0;
            OperationModeSettings = null;
            LayoutModel.ActivePhases = LayoutPhase.All;

            EventManager.Event(new LayoutEvent<OperationModeParameters>("enter-design-mode", null));
            LayoutModel.Instance.Redraw();
        }

        #endregion

        #region Design time activation

        /// <summary>
        /// Begin design time activation of command stations. Use this for enabling limited operation such as testing elements etc.
        /// </summary>
        /// <returns>True - design time operation activated, false - not activated</returns>
        public bool BeginDesignTimeActivation() {
            bool ok = false;

            if (layoutDesignTimeActivationNesting == 0) {
                var commandStations = LayoutModel.Components<IModelComponentIsBusProvider>(LayoutPhase.All);

                if (!commandStations.Any())
                    throw new LayoutException("The layout does not contain any command station component. Please add a command station component");

                bool doit = true;

                if (showConnectTrackPowerWarning) {
                    if (MessageBox.Show(ActiveFrameWindow, "This operation will power your layout.\n" +
                        "Make sure that your layout is in a state that it can be safely powered!\n\n" +
                        "Do you want to continue? (this warning will not be shown again)", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) {
                        doit = false;
                    }
                    else
                        showConnectTrackPowerWarning = false;
                }

                if (doit) {
                    object result = EventManager.Event(new LayoutEvent("begin-design-time-layout-activation", this));

                    if (result == null || !(result is bool))
                        ok = false;
                    else
                        ok = (bool)result;

                    if (ok)
                        layoutDesignTimeActivationNesting++;
                }
            }
            else {
                layoutDesignTimeActivationNesting++;        // nested request
                ok = true;
            }

            return ok;
        }

        /// <summary>
        /// Terminate design time activation
        /// </summary>
        public void EndDesignTimeActivation() {
            if (layoutDesignTimeActivationNesting <= 0)
                Trace.WriteLine("unbalanced enter/exit design time layout activation");

            if (--layoutDesignTimeActivationNesting == 0)
                EventManager.Event(new LayoutEvent("end-design-time-layout-activation", this));
        }

        /// <summary>
        /// True if design time activation is in effect.
        /// </summary>
        public bool IsDesignTimeActivation => layoutDesignTimeActivationNesting > 0;

        #endregion

        #endregion

        #region Controller Properties

        #region Layout files

        public string LayoutDisplayStateFilename {
            get;
            private set;
        }

        /// <summary>
        /// The file name in which layout runtime state is saved
        /// </summary>
        public string LayoutRuntimeStateFilename {
            get;
            private set;
        }

        public string LayoutRoutingFilename {
            get;
            private set;
        }

        public string LayoutFilename {
            get {
                return layoutFilename;
            }

            private set {
                layoutFilename = value;
                LayoutDisplayStateFilename = Path.ChangeExtension(value, ".DisplayState");
                LayoutRuntimeStateFilename = Path.ChangeExtension(value, ".State");
                LayoutRoutingFilename = Path.ChangeExtension(value, ".Routing");
            }
        }

        #endregion

        /// <summary>
        /// The currently active frame window
        /// </summary>
        public ILayoutFrameWindow ActiveFrameWindow {
            get;
            set;
        }

        /// <summary>
        /// The command manager (Do/Undo support)
        /// </summary>
        public ILayoutCommandManager CommandManager => commandManager;

        /// <summary>
        /// Operation mode settings, Null if in design mode
        /// </summary>
        public OperationModeParameters OperationModeSettings {
            get;
            private set;
        }

        /// <summary>
        /// Currently open frame windows
        /// </summary>
        public List<LayoutFrameWindow> FrameWindows {
            get;
            private set;
        }

        /// <summary>
        /// Where common data (settings that are relevant to all layouts) are saved
        /// </summary>
        public string CommonDataFolderName => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + Application.ProductName;

        public LayoutSelection UserSelection => userSelection;

        #endregion

        #region Model Initialization/Loading/saving

        /// <summary>
        /// Create a new layout, that is a new model, and one default view called overview
        /// </summary>
        void CreateNewModel(string layoutFilename) {
            LayoutModel.Instance.Clear();
            LayoutModelArea area = new LayoutModelArea {

                // Create the default area
                Name = "Layout area"
            };
            LayoutModel.Areas.Add(area);

            LayoutFilename = layoutFilename;
            commandManager.ChangeLevel = 0;
            EventManager.Event(new LayoutEvent(null, "new-layout-document", null, layoutFilename));
        }

        /// <summary>
        /// Load existing layout
        /// </summary>
        void LoadModel(string layoutFilename) {
            LayoutModel.Instance.Clear();
            ReadModelXmlDocument(layoutFilename);

            LayoutFilename = layoutFilename;

            LayoutModel.StateManager.Load();
            EventManager.Event(new LayoutEvent(null, "new-layout-document", null, layoutFilename));
        }

        void ReadModelXmlDocument(string filename) {
            XmlTextReader r = new LayoutXmlTextReader(filename, LayoutReadXmlContext.LoadingModel) {
                WhitespaceHandling = WhitespaceHandling.None
            };

            // Skip all kind of declaration stuff
            while (r.Read() && r.NodeType == XmlNodeType.XmlDeclaration)
                ;

            r.Read();       // <LayoutManager>

            while (r.NodeType == XmlNodeType.Element) {
                if (r.Name == "LayoutModel")
                    LayoutModel.Instance.ReadXml(r);
                else
                    r.Skip();
            }

            r.Close();
        }

        /// <summary>
        /// Save the model
        /// </summary>
        private void SaveModel(string layoutFilename) {
            WriteModelXmlDocument(layoutFilename);

            commandManager.ChangeLevel = 0;             // Layout saved
        }

        void WriteModelXmlDocument(string filename) {
            XmlTextWriter w = new XmlTextWriter(filename, new System.Text.UTF8Encoding());

            w.WriteStartDocument();
            w.WriteStartElement("LayoutManager");
            LayoutModel.Instance.WriteXml(w);
            w.WriteEndElement();
            w.WriteEndDocument();

            w.Close();
        }

        #endregion

        #region Handle model area /Add/Removed/Rename events

        /// <summary>
        /// Add a new area to the model
        /// </summary>
        /// <returns>The newly created area</returns>
        public LayoutModelArea AddArea(String areaName) {
            LayoutModelArea area = new LayoutModelArea {
                Name = areaName
            };
            LayoutModel.Areas.Add(area);

            return area;
        }

        public void Area_Added(object sender, EventArgs e) {
            LayoutModelArea area = (LayoutModelArea)sender;

            EventManager.Event(new LayoutEvent<LayoutModelArea>("area-added", area));
            LayoutModified();
        }

        public void Area_Removed(object sender, EventArgs e) {
            LayoutModelArea area = (LayoutModelArea)sender;

            EventManager.Event(new LayoutEvent<LayoutModelArea>("area-removed", area));
            LayoutModified();
        }

        public void Area_Renamed(object sender, EventArgs e) {
            LayoutModelArea area = (LayoutModelArea)sender;

            EventManager.Event(new LayoutEvent<LayoutModelArea>("area-renamed", area));
            LayoutModified();
        }

        #endregion

        #region ILayoutController Members

        public void Do(ILayoutCommand command) {
            commandManager.Do(command);
        }

        public LayoutModuleManager ModuleManager => moduleManager;

        [LayoutEvent("get-module-manager")]
        private void getModuleManager(LayoutEvent e) {
            e.Info = ModuleManager;
        }

        public bool IsOperationMode => OperationModeSettings != null;

        public bool TrainsAnalysisPhase => trainsAnalysisPhaseCount > 0;

        public PreviewRouteManager PreviewRouteManager => previewRouteManager;

        /// <summary>
        /// Called to mark the layout as modified
        /// </summary>
        public void LayoutModified() {
            commandManager.ChangeLevel++;
        }

        #endregion

        #region ILayoutSelectionManager Members

        public void DisplaySelection(LayoutSelection selection, int zOrder) {
            if (zOrder == LayoutSelection.ZOrderBottom)
                displayedSelections.Add(selection);
            else
                displayedSelections.Insert(0, selection);
        }

        public void HideSelection(LayoutSelection selection) {
            displayedSelections.Remove(selection);
        }

        public IList<LayoutSelection> DisplayedSelections => displayedSelections.AsReadOnly();

        #endregion

        #region Event Handlers

        [LayoutEvent("save-layout")]
        private void saveLayout(LayoutEvent e) {
            SaveModel(LayoutFilename);
            SaveDisplayState(FrameWindows);

            if (IsOperationMode)
                LayoutModel.StateManager.Save();
        }

        #endregion
    }

    public class LayoutDisplayState : LayoutXmlWrapper {
        public LayoutDisplayState(IEnumerable<LayoutFrameWindow> frameWindows) : base("DisplayState") {
            OperationModeSettings = LayoutController.OperationModeSettings;

            XmlElement windowStatesElement = CreateChildElement("WindowStates");

            int windowIndex = 0;

            foreach (var frameWindow in frameWindows) {
                var frameWindowState = new FrameWindowState(windowStatesElement, frameWindow);

                frameWindow.SaveFrameWindowState(frameWindowState);

                if (LayoutController.ActiveFrameWindow == frameWindow)
                    this.ActiveWindowIndex = windowIndex;
                windowIndex++;
            }
        }

        public LayoutDisplayState(string displayStateFilename) {
            Load(displayStateFilename);
        }

        public LayoutDisplayState() : base("DisplayState") {
            OperationModeSettings = null;       // default is design
        }

        public void Save(string displayStateFilename) {
            Element.OwnerDocument.Save(displayStateFilename);
        }

        public OperationModeParameters OperationModeSettings {
            get {
                string mode = GetAttribute("Mode");

                if (mode == "Design")
                    return null;
                else {
                    var phases = (LayoutPhase)Enum.Parse(typeof(LayoutPhase), GetOptionalAttribute("Phases") ??  "Operational");

                    return new OperationModeParameters() { Simulation = (mode == "Simulation"), Phases = phases };
                }
            }

            set {
                string mode = "Design";

                if (value != null) {
                    if (value.Simulation)
                        mode = "Simulation";
                    else
                        mode = "Operate";

                    SetAttribute("Phases", value.Phases.ToString());
                }

                SetAttribute("Mode", mode);
            }
        }

        public int ActiveWindowIndex {
            get { return XmlConvert.ToInt32(GetOptionalAttribute("ActiveWindowIndex") ??  "-1"); }
            set { SetAttribute("ActiveWindowIndex", XmlConvert.ToString(value)); }
        }

        public IEnumerable<FrameWindowState> FrameWindowStates {
            get {
                XmlElement windowStatesElement = Element["WindowStates"];

                if (windowStatesElement != null)
                    foreach (XmlElement windowStateElement in windowStatesElement)
                        yield return new FrameWindowState(windowStateElement);
            }
        }
        /// <summary>
        /// Return a list of frame windows. If display state is empty, the list consists of one default frame window
        /// </summary>
        public List<LayoutFrameWindow> FrameWindows {
            get {
                var frameWindows = new List<LayoutFrameWindow>();

                if (FrameWindowStates.Any()) {
                    foreach (var frameWindowState in FrameWindowStates)
                        frameWindowState.Restore(frameWindows);
                }
                else
                    frameWindows.Add(new LayoutFrameWindow());

                return frameWindows;
            }
        }
    }
}
