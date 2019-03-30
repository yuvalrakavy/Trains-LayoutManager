using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using System.IO;

using LayoutManager.Model;
using LayoutManager.View;
using LayoutManager.CommonUI;
using LayoutManager.Components;

namespace LayoutManager {
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    #pragma warning disable IDE0051, IDE0060
    public partial class LayoutFrameWindow : Form, ILayoutFrameWindow {
        TaskCompletionSource<FrameWindowAction> tcs;

        LayoutTool designTool = null;
        LayoutTool operationTool = null;
        LayoutTool activeTool = null;
        ZoomEntry[] zoomEntries;

        bool layoutControlVisible = false;
        bool tripsMonitorVisible = false;
        bool messageViewVisible = false;
        bool locomotiveViewVisible = false;

        int tripsMonitorHeightInOperationMode = 0;
        int layoutControlWidth = 0;

        PrintState printState = null;

        DetailsPopupWindow detailsPopupWindow;
        LayoutView viewWithMouse;

        Point viewMouseLocation = new Point(0, 0);
        Point detailsPopupLocationInModelPoints = new Point(0, 0);

        OperationModeParameters uiMode = null;

        #region UI setting classes

        enum UIitemSetting {
            Hidden,
            Disabled
        };

        abstract class UIsettingEntry {
            protected UIitemSetting setting;

            public UIitemSetting Setting => setting;
        }

        class DoSettingEntry : UIsettingEntry {
            public UIsettingEntry[] DoThis { get; }

            public DoSettingEntry(UIsettingEntry[] doThis) {
                this.DoThis = doThis;
            }
        }

        class MenuSettingEntry : UIsettingEntry {
            readonly MenuItem menuItem;

            public MenuSettingEntry(MenuItem menuItem, UIitemSetting setting) {
                this.menuItem = menuItem;
                this.setting = setting;
            }

            public MenuItem MenuItem => menuItem;
        }

        class ToolbarSettingEntry : UIsettingEntry {
            readonly ToolBarButton button;

            public ToolbarSettingEntry(ToolBarButton button, UIitemSetting setting) {
                this.button = button;
                this.setting = setting;
            }

            public ToolBarButton Button => button;
        }

        #endregion

        UIsettingEntry[] operationBaseUIsetting;
        UIsettingEntry[] operationModeUIsetting;
        UIsettingEntry[] simulationModeUIsetting;

        UIsettingEntry[] designModeUIsetting;
        UIsettingEntry[] activeUIsetting;

        /// <summary>
        /// Initialize frame window with all areas in the model, and with one default view page in each area
        /// </summary>
        public LayoutFrameWindow() {
            CommonInitialization();

            // Create default frame window settings (one view for each area)
            foreach (var area in LayoutModel.Areas) {
                var areaPage = new LayoutFrameWindowAreaTabPage(area);

                tabAreas.TabPages.Add(areaPage);
                AddViewToArea(areaPage, "Overview");
            }
        }

        /// <summary>
        /// Initialize frame window and restore its state
        /// </summary>
        /// <param name="windowStateElement">Xml element describing the frame window state</param>
        public LayoutFrameWindow(FrameWindowState frameWindowState) {
            CommonInitialization();

            StartPosition = FormStartPosition.Manual;

            Rectangle b = frameWindowState.Bounds;
            SetDesktopBounds(b.Left, b.Top, b.Width, b.Height);

            if (frameWindowState.WindowState == FormWindowState.Maximized)
                WindowState = FormWindowState.Maximized;

            frameWindowState.Restore(tabAreas);

            locomotivesViewer.Width = frameWindowState.LocomotiveViewerWidth;
            UpdateLocomotivesVisible();

            layoutControlViewer.Width = frameWindowState.ControlViewerWidth;
            UpdateLayoutControlVisible();

            messageViewer.Height = frameWindowState.MessagesViewerHeight;
            UpdateMessageVisible();

            tripsMonitorHeightInOperationMode = frameWindowState.TripsViewerHeight;
        }

        #region Initialization

        private void CommonInitialization() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            EventManager.AddObjectSubscriptions(this);

            Id = Guid.NewGuid();

            layoutControlViewer.Width = 0;
            tripsMonitor.Height = 0;
            messageViewer.Height = 0;
            locomotivesViewer.Width = 0;

            // Initialize the built-in zoom preset table
            zoomEntries = new ZoomEntry[] {
                                               new ZoomEntry(200, menuItemZoom200),
                                               new ZoomEntry(150, menuItemZoom150),
                                               new ZoomEntry(100, menuItemZoom100),
                                               new ZoomEntry(70, menuItemZoom75),
                                               new ZoomEntry(50, menuItemZoom50),
                                               new ZoomEntry(20, menuItemZoom25),
            };

            operationBaseUIsetting = new UIsettingEntry[] {
                new MenuSettingEntry(menuItemArea, UIitemSetting.Hidden),
                new MenuSettingEntry(menuItemOperational, UIitemSetting.Disabled),
                new MenuSettingEntry(menuItemSimulation, UIitemSetting.Disabled),
                new MenuSettingEntry(menuItemCompile, UIitemSetting.Hidden),
                new MenuSettingEntry(menuItemDesignTimeOnlySeperator, UIitemSetting.Hidden),
                new MenuSettingEntry(menuItemLearnLayout, UIitemSetting.Hidden),
                new MenuSettingEntry(menuItemConnectLayout, UIitemSetting.Hidden),
                new MenuSettingEntry(menuItemDefaultPhase, UIitemSetting.Hidden),
                new MenuSettingEntry(menuItemSetComponentPhase, UIitemSetting.Hidden),
                new MenuSettingEntry(menuItemNewArea, UIitemSetting.Disabled),
                new MenuSettingEntry(menuItemDeleteArea, UIitemSetting.Disabled),
                new ToolbarSettingEntry(toolBarButtonOpenLayiout, UIitemSetting.Hidden),
            };

            // Initialize the menu setting tables
            operationModeUIsetting = new UIsettingEntry[] {
                new DoSettingEntry(operationBaseUIsetting),
                new ToolbarSettingEntry(toolBarButtonSimulation, UIitemSetting.Hidden),
            };

            simulationModeUIsetting = new UIsettingEntry[] {
                new DoSettingEntry(operationBaseUIsetting),
                new ToolbarSettingEntry(toolBarButtonOperationMode, UIitemSetting.Hidden),
            };

            designModeUIsetting = new UIsettingEntry[] {
                new MenuSettingEntry(menuItemDesign, UIitemSetting.Disabled),
                new MenuSettingEntry(menuItemCommonDestinations, UIitemSetting.Hidden),
                new MenuSettingEntry(menuItemManualDispatchRegions, UIitemSetting.Hidden),
                new MenuSettingEntry(menuItemShowTripsMonitor, UIitemSetting.Hidden),
                new MenuSettingEntry(menuItemEmergencyStop, UIitemSetting.Hidden),
                new MenuSettingEntry(menuItemSuspendLocomotives, UIitemSetting.Hidden),
                new MenuSettingEntry(menuItemEmergencyStop, UIitemSetting.Hidden),
                new MenuSettingEntry(menuItemViewActivePhases, UIitemSetting.Hidden),
                new ToolbarSettingEntry(toolBarButtonShowTripsMonitor, UIitemSetting.Hidden),
                new ToolbarSettingEntry(toolBarButtonToggleAllLayoutManualDispatch, UIitemSetting.Hidden),
                new ToolbarSettingEntry(toolBarButtonStopAllLocomotives, UIitemSetting.Hidden),
                new ToolbarSettingEntry(toolBarButtonEmergencyStop, UIitemSetting.Hidden),
            };

            messageViewer.Initialize();
            locomotivesViewer.Initialize();
            tripsMonitor.Initialize();
            layoutControlViewer.Initialize();

            doUIsetting(designModeUIsetting);
            uiMode = null;

            designTool = new LayoutEditingTool();
            operationTool = new LayoutOperationTool();

            locomotivesViewer.LocomotiveCollection = LayoutModel.LocomotiveCollection;

            InitializeTask();
            ForceSetUserInterfaceMode(LayoutController.OperationModeSettings);
        }

        #endregion

        /// <summary>
        /// Called when this frame window is activated. Set the controller's ActiveFrameWindow to point to this window
        /// </summary>
        /// <param name="e"></param>
        protected override void OnActivated(EventArgs e) {
            base.OnActivated(e);

            LayoutController.ActiveFrameWindow = this;
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);

            ControllerCommand = FrameWindowCommand.CloseWindow;
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        /// <summary>
        /// Save the current window state (areas/view etc.)
        /// </summary>
        /// <param name="windowStateElement">The Xml element in which the state is saved</param>
        public void SaveFrameWindowState(FrameWindowState frameWindowState) {
            frameWindowState.LocomotiveViewerWidth = locomotivesViewer.Width;
            frameWindowState.ControlViewerWidth = layoutControlViewer.Width;
            frameWindowState.MessagesViewerHeight = messageViewer.Height;

            if (LayoutController.IsOperationMode)
                frameWindowState.TripsViewerHeight = tripsMonitor.Height;
            else
                frameWindowState.TripsViewerHeight = tripsMonitorHeightInOperationMode;
        }

        /// <summary>
        /// Initialize the Task object associated with this window, the task will be marked as completed whenever
        /// the controller needs to do something relating to this frame window
        /// </summary>
        public void InitializeTask() {
            Debug.Assert(tcs == null || tcs.Task.Status == TaskStatus.RanToCompletion);
            tcs = new TaskCompletionSource<FrameWindowAction>();
        }

        /// <summary>
        /// The frame window ID. This can be used to figure out if event is intended to this frame window or not.
        /// </summary>
        public Guid Id {
            get;
            private set;
        }

        /// <summary>
        /// Task associated with this frame window. The controller await on this task, and will do the action
        /// specified by the returned value
        /// </summary>
        public Task<FrameWindowAction> Task => tcs.Task;

        /// <summary>
        /// Return the area tab pages in the window
        /// </summary>
        internal IEnumerable<LayoutFrameWindowAreaTabPage> AreaTabs {
            get {
                foreach (LayoutFrameWindowAreaTabPage areaTab in tabAreas.TabPages)
                    yield return areaTab;
            }
        }

        internal int ActiveAreaIndex => tabAreas.SelectedIndex;

        /// <summary>
        /// Tell the controller what to do with frame window. Setting this property will set the task state associated with
        /// this frame window to completed
        /// </summary>
        private FrameWindowCommand ControllerCommand {
            set {
                Debug.Assert(tcs != null);
                var myTcs = tcs;

                if (tcs.Task.Status == TaskStatus.RanToCompletion)
                    tcs = null;

                myTcs.SetResult(new FrameWindowAction(this, value));
            }
        }

        #region active Area/View properties

        LayoutFrameWindowAreaTabPage ActiveAreaPage => (LayoutFrameWindowAreaTabPage)tabAreas.SelectedTab;

        LayoutFrameWindowAreaViewTabPage ActiveViewPage {
            get {
                if (ActiveAreaPage == null)
                    return null;
                return (LayoutFrameWindowAreaViewTabPage)ActiveAreaPage.TabViews.SelectedTab;
            }
        }

        LayoutView ActiveView {
            get {
                if (ActiveViewPage == null)
                    return null;
                return ActiveViewPage.View;
            }
        }

        #endregion

        #region Setting UI for various mode (design/operational/simulation)

        void ForceSetUserInterfaceMode(OperationModeParameters settings) {
            undoUIsetting();

            if (settings == null) {
                doUIsetting(designModeUIsetting);
                activeTool = designTool;

                toolBarButtonDesignMode.Pushed = true;
                toolBarButtonOperationMode.Pushed = false;
                toolBarButtonSimulation.Pushed = false;
                statusBarPanelMode.Text = "Design";

                tripsMonitorHeightInOperationMode = tripsMonitor.Height;
                EventManager.Event(new LayoutEvent("hide-trips-monitor", this).SetFrameWindow(this));
                splitterTripsMonitor.Visible = false;

                ChangeAllViewsPhases(LayoutPhase.All);      // In editing show all phases
            }
            else {
                toolBarButtonDesignMode.Pushed = false;

                if (settings.Simulation) {
                    doUIsetting(simulationModeUIsetting);
                    toolBarButtonOperationMode.Pushed = false;
                    toolBarButtonSimulation.Pushed = true;

                    string s = "Simulation";

                    switch (settings.Phases) {
                        case LayoutPhase.Operational: s += " on operational region"; break;
                        case LayoutPhase.NotPlanned: s += " on operational+in construnction regions"; break;
                    }

                    statusBarPanelMode.Text = s;
                }
                else {
                    doUIsetting(operationModeUIsetting);
                    toolBarButtonOperationMode.Pushed = true;
                    toolBarButtonSimulation.Pushed = false;
                    statusBarPanelMode.Text = "Operation";
                }

                if (tripsMonitorHeightInOperationMode > 0) {
                    tripsMonitor.Height = tripsMonitorHeightInOperationMode;
                    UpdateTripsMonitorVisible();
                }

                splitterTripsMonitor.Visible = true;

                AdjustAllViewsPhases();
                activeTool = operationTool;
            }

            foreach (LayoutFrameWindowAreaTabPage atp in tabAreas.TabPages) {
                foreach (LayoutFrameWindowAreaViewTabPage vtp in atp.TabViews.TabPages) {
                    LayoutPhase phases = LayoutPhase.All;

                    if (settings != null && vtp.ShowActivePhases)
                        phases = LayoutModel.ActivePhases;

                    if (vtp.View.Phases != phases) {
                        vtp.View.Phases = phases;
                        vtp.View.Invalidate();
                    }
                }
            }

            uiMode = settings;
        }

        void SetUserInterfaceMode(OperationModeParameters settings) {
            if (settings != uiMode)
                ForceSetUserInterfaceMode(settings);
        }

        #endregion

        /// <summary>
        /// Add a view to an area. A new tab page is created to show the view
        /// </summary>
        /// <param name="area">The area to which to assign the view</param>
        /// <param name="name">The view's name</param>
        /// <param name="view">The view object</param>
        public LayoutFrameWindowAreaViewTabPage AddViewToArea(LayoutFrameWindowAreaTabPage areaTabPage, String name, LayoutView view) {
            view.Area = areaTabPage.Area;

            // Wire events in order to reflect them to the current tool
            view.MouseDown += new MouseEventHandler(this.LayoutView_MouseDown);
            view.MouseMove += new MouseEventHandler(this.LayoutView_MouseMove);
            view.ModelComponentClick += new EventHandler<LayoutViewEventArgs>(this.LayoutView_ModelComponentClick);
            view.ModelComponentDragged += new EventHandler<LayoutViewEventArgs>(this.LayoutView_ModelComponentDragged);
            view.ModelComponentQueryDrop += new EventHandler<LayoutViewEventArgs>(LayoutView_ModelComponentQueryDrop);
            view.ModelComponentDrop += new EventHandler<LayoutViewEventArgs>(LayoutView_ModelComponentDrop);

            LayoutFrameWindowAreaViewTabPage viewTabPage = new LayoutFrameWindowAreaViewTabPage(view) {
                Text = name
            };
            areaTabPage.TabViews.TabPages.Add(viewTabPage);

            return viewTabPage;
        }

        public LayoutFrameWindowAreaViewTabPage AddViewToArea(LayoutFrameWindowAreaTabPage areaPage, String name) => AddViewToArea(areaPage, name, new LayoutView());

        void ChangeAllViewsPhases(LayoutPhase phases) {
            foreach (LayoutFrameWindowAreaTabPage atp in tabAreas.TabPages) {
                foreach (LayoutFrameWindowAreaViewTabPage vtp in atp.TabViews.TabPages) {
                    if (vtp.View.Phases != phases) {
                        vtp.View.Phases = phases;
                        vtp.View.Invalidate();
                    }
                }
            }
        }

        void AdjustAllViewsPhases() {
            foreach (LayoutFrameWindowAreaTabPage atp in tabAreas.TabPages) {
                foreach (LayoutFrameWindowAreaViewTabPage vtp in atp.TabViews.TabPages) {
                    LayoutPhase phases = vtp.ShowActivePhases ? LayoutModel.ActivePhases : LayoutPhase.All;

                    if (vtp.View.Phases != phases) {
                        vtp.View.Phases = phases;
                        vtp.View.Invalidate();
                    }
                }
            }

        }

        /// <summary>
        /// Update the form title based on the layout file used
        /// </summary>
        private void updateFormTitle() {
            string layoutName;
            string modified = "";

            layoutName = LayoutController.LayoutFilename;

            if (LayoutController.CommandManager.ChangeLevel != 0)
                modified = "*";

            this.Text = Path.GetFileName(layoutName) + modified + " - VillaRakavy Layout Manager";
        }

        [LayoutEvent("close-all-frame-windows")]
        private void closeAllFrameWindows(LayoutEvent e) {
            Close();
        }

        [LayoutEvent("model-modified")]
        [LayoutEvent("model-not-modified")]
        private void modelModificationStateChanged(LayoutEvent e) {
            updateFormTitle();
        }

        internal void OnViewChanged() {
            if (ActiveView != null)
                toolBarButtonToggleGrid.Pushed = ActiveView.ShowGrid != ShowGridLinesOption.Hide;
        }

        [LayoutEvent("test")]
        private void test(LayoutEvent e) {
            Trace.WriteLine("Got event test");
            Trace.WriteLine("Sender is " + (e.Sender == null ? "Null" : e.Sender.ToString()));
            Trace.WriteLine("Info is " + (e.Info == null ? "Null" : e.Info.ToString()));

            XmlElement optionsElement = e.Element["Options"];

            if (optionsElement != null) {
                Trace.WriteLine("Options:");

                foreach (XmlAttribute a in optionsElement.Attributes)
                    Trace.WriteLine("  Option " + a.Name + " is " + a.Value);
            }
        }

        [LayoutEvent("tools-menu-open-request")]
        private void OnToolsMenuOpenRequest(LayoutEvent e) {
            ILayoutEmulatorServices layoutEmulationServices = (ILayoutEmulatorServices)EventManager.Event(new LayoutEvent("get-layout-emulation-services", this));

            if (layoutEmulationServices != null) {
                Menu toolsMenu = (Menu)e.Info;

                toolsMenu.MenuItems.Add(new MenuItem("Reset layout emulation", new EventHandler(onResetLayoutEmulation)));
            }
        }

        [LayoutEvent("show-routing-table-generation-dialog")]
        private void showRoutingTableGenerationDialog(LayoutEvent e) {
            int nTurnouts = (int)e.Info;
            Dialogs.RoutingTableCalcProgress d = new Dialogs.RoutingTableCalcProgress(nTurnouts);

            d.ShowDialog(this);
        }

        private void onResetLayoutEmulation(object sender, EventArgs e) {
            EventManager.Event(new LayoutEvent("reset-layout-emulation", this));
        }

        [LayoutEvent("enter-design-mode")]
        private void enterDesignMode(LayoutEvent e) {
            SetUserInterfaceMode(null);
        }

        [LayoutEvent("prepare-enter-operation-mode")]
        private void enterOperationMode(LayoutEvent e0) {
            var e = (LayoutEvent<OperationModeParameters>)e0;

            SetUserInterfaceMode(e.Sender);
        }

        [LayoutEvent("begin-trains-analysis-phase")]
        private void beginTrainsAnalysisPhase(LayoutEvent e) {
            statusBarPanelOperation.Text = "Analyzing trains status...";
        }

        [LayoutEvent("end-trains-analysis-phase")]
        private void endTrainsAnalysisPhase(LayoutEvent e) {
            statusBarPanelOperation.Text = "Ready";
        }

        #region Menu Editing functions

        /// <summary>
        /// Apply the setting (do) to menu items based on instructions in a provided table
        /// </summary>
        /// <param name="menuSetting">Instructions for the menu item setting</param>
        private void doUIsetting(UIsettingEntry[] uiSetting) {
            foreach (UIsettingEntry aEntry in uiSetting) {
                if (aEntry is DoSettingEntry)
                    doUIsetting(((DoSettingEntry)aEntry).DoThis);
                else if (aEntry is MenuSettingEntry menuEntry) {
                    if (menuEntry.Setting == UIitemSetting.Hidden)
                        menuEntry.MenuItem.Visible = false;
                    else if (menuEntry.Setting == UIitemSetting.Disabled)
                        menuEntry.MenuItem.Enabled = false;
                }
                else if (aEntry is ToolbarSettingEntry toolbarEntry) {
                    if (toolbarEntry.Setting == UIitemSetting.Hidden)
                        toolbarEntry.Button.Visible = false;
                    else if (toolbarEntry.Setting == UIitemSetting.Disabled)
                        toolbarEntry.Button.Enabled = false;
                }
            }

            activeUIsetting = uiSetting;
        }

        /// <summary>
        /// Undo the setting to menu items. The setting are described in the passed table
        /// </summary>
        /// <param name="menuSetting"0>Instructions for the menu item setting</param>
        private void undoUIsetting(UIsettingEntry[] uiSetting) {
            foreach (UIsettingEntry aEntry in uiSetting) {
                if (aEntry is DoSettingEntry)
                    undoUIsetting(((DoSettingEntry)aEntry).DoThis);
                else if (aEntry is MenuSettingEntry menuEntry) {
                    if (menuEntry.Setting == UIitemSetting.Hidden)
                        menuEntry.MenuItem.Visible = true;
                    else if (menuEntry.Setting == UIitemSetting.Disabled)
                        menuEntry.MenuItem.Enabled = true;
                }
                else if (aEntry is ToolbarSettingEntry toolbarEntry) {
                    if (toolbarEntry.Setting == UIitemSetting.Hidden)
                        toolbarEntry.Button.Visible = true;
                    else if (toolbarEntry.Setting == UIitemSetting.Disabled)
                        toolbarEntry.Button.Enabled = true;
                }
            }
        }

        private void undoUIsetting() {
            if (activeUIsetting != null) {
                undoUIsetting(activeUIsetting);
                activeUIsetting = null;
            }
        }

        #endregion


        #region Layout Event handlers

        [LayoutEvent("show-control-connection-point")]
        private void showControlConnectionPoint(LayoutEvent e) {
            if (e.IsThisFrameWindow(this)) {
                ControlConnectionPointReference connectionPointRef = (ControlConnectionPointReference)e.Sender;

                EventManager.Event(new LayoutEvent("show-layout-control", this).SetFrameWindow(e));
                layoutControlViewer.EnsureVisible(connectionPointRef, true);
            }
        }

        [LayoutEvent("show-control-module")]
        private void showControlModule(LayoutEvent e) {
            if (e.IsThisFrameWindow(this)) {
                ControlModuleReference moduleRef = (ControlModuleReference)e.Sender;

                EventManager.Event(new LayoutEvent("show-layout-control", this));
                layoutControlViewer.EnsureVisible(moduleRef, true);
            }
        }

        [LayoutEvent("deselect-control-objects")]
        private void deselectControlObjects(LayoutEvent e) {
            if (e.IsThisFrameWindow(this))
                layoutControlViewer.DeselectAll();
        }

        [LayoutEvent("ensure-component-visible")]
        private void ensureComponentVisible(LayoutEvent e) {
            if (e.IsThisFrameWindow(this)) {
                ModelComponent component = (ModelComponent)e.Sender;
                LayoutModelArea area = component.Spot.Area;
                LayoutFrameWindowAreaTabPage areaPage = null;
                bool markComponent = (bool)e.Info;

                foreach (LayoutFrameWindowAreaTabPage ap in tabAreas.TabPages)
                    if (ap.Area == area) {
                        areaPage = ap;
                        break;
                    }

                if (areaPage != null) {
                    LayoutFrameWindowAreaViewTabPage bestViewPage = null;
                    LayoutView.VisibleResult bestVisibleResult = LayoutView.VisibleResult.No;

                    // Figure out what view in this area shows the component best

                    foreach (LayoutFrameWindowAreaViewTabPage vp in areaPage.TabViews.TabPages) {
                        LayoutView.VisibleResult visibleResult = vp.View.IsComponentVisible(component);

                        if ((int)visibleResult > (int)bestVisibleResult) {
                            bestVisibleResult = visibleResult;
                            bestViewPage = vp;
                        }
                        else if (visibleResult != LayoutView.VisibleResult.No &&
                            visibleResult == bestVisibleResult && vp.View.Zoom > bestViewPage.View.Zoom) {
                            bestVisibleResult = visibleResult;
                            bestViewPage = vp;
                        }
                    }

                    if (bestViewPage == null)
                        bestViewPage = (LayoutFrameWindowAreaViewTabPage)areaPage.TabViews.TabPages[0];

                    tabAreas.SelectedTab = areaPage;
                    areaPage.TabViews.SelectedTab = bestViewPage;

                    bestViewPage.View.EnsureVisible(component.Location);

                    if (markComponent)
                        EventManager.Event(new LayoutEvent("show-marker", component).SetFrameWindow(e));
                }
            }
        }

        [LayoutEvent("all-layout-manual-dispatch-mode-status-changed")]
        private void AllLayoutManualDispatchModeStatusChanged(LayoutEvent e) {
            bool active = (bool)e.Info;

            toolBarButtonToggleAllLayoutManualDispatch.Pushed = active;
        }

        [LayoutEvent("manual-dispatch-region-status-changed")]
        private void manualDispatchRegionStatusChanged(LayoutEvent e) {
            bool enableAllLayoutManualDispatch = true;

            foreach (ManualDispatchRegionInfo manualDispatchRegion in LayoutModel.StateManager.ManualDispatchRegions)
                if (manualDispatchRegion.Active)
                    enableAllLayoutManualDispatch = false;

            toolBarButtonToggleAllLayoutManualDispatch.Enabled = enableAllLayoutManualDispatch;
        }

        [LayoutEvent("all-locomotives-suspended-status-changed")]
        private void allLocomotivesSuspendedStatusChanged(LayoutEvent e) {
            bool allSuspended = (bool)e.Info;

            toolBarButtonStopAllLocomotives.Pushed = allSuspended;
        }

        [LayoutEvent("show-marker")]
        private void showMarker(LayoutEvent e) {
            if (e.IsThisFrameWindow(this)) {
                LayoutSelection markerSelection;

                if (e.Sender is ModelComponent) {
                    markerSelection = new LayoutSelection {
                        (ModelComponent)e.Sender
                    };
                }
                else if (e.Sender is LayoutSelection)
                    markerSelection = (LayoutSelection)e.Sender;
                else
                    throw new ArgumentException("Invalid marker (can be either component or selection");

                new Marker(markerSelection);
            }
        }

        #region Marker class

        class MarkerLook : ILayoutSelectionLook {
            public Color Color => Color.IndianRed;
        }

        class Marker {
            Timer hideMarkerTimer;
            readonly LayoutSelection markerSelection;

            public Marker(LayoutSelection markerSelection) {
                this.markerSelection = markerSelection;

                markerSelection.Display(new MarkerLook());
                hideMarkerTimer = new Timer {
                    Interval = 500
                };
                hideMarkerTimer.Tick += new EventHandler(this.onHideMarker);
                hideMarkerTimer.Start();
            }

            private void onHideMarker(object sender, EventArgs e) {
                hideMarkerTimer.Dispose();
                hideMarkerTimer = null;
                markerSelection.Hide();
            }
        }

        #endregion

        void UpdateLayoutControlVisible() {
            bool previousState = layoutControlVisible;

            layoutControlVisible = layoutControlViewer.Width > 10;
            if (layoutControlVisible != previousState)
                EventManager.Event(new LayoutEvent(layoutControlVisible ? "layout-control-shown" : "layout-control-hidden", this).SetFrameWindow(this));
        }

        void UpdateTripsMonitorVisible() {
            bool previousState = tripsMonitorVisible;

            tripsMonitorVisible = tripsMonitor.Height > 10;
            if (tripsMonitorVisible != previousState)
                EventManager.Event(new LayoutEvent(tripsMonitorVisible ? "trips-monitor-shown" : "trips-monitor-hidden", this).SetFrameWindow(this));
        }

        void UpdateMessageVisible() {
            bool previousState = messageViewVisible;

            messageViewVisible = messageViewer.Height > 10;
            if (messageViewVisible != previousState)
                EventManager.Event(new LayoutEvent(messageViewVisible ? "messages-shown" : "messages-hidden", this).SetFrameWindow(this));
        }

        void UpdateLocomotivesVisible() {
            bool previousState = locomotiveViewVisible;

            locomotiveViewVisible = locomotivesViewer.Width > 10;
            if (locomotiveViewVisible != previousState)
                EventManager.Event(new LayoutEvent(locomotiveViewVisible ? "locomotives-shown" : "locomotives-hidden", this).SetFrameWindow(this));
        }

        [LayoutEvent("show-messages")]
        void ShowMessages(LayoutEvent e) {
            if (e.IsThisFrameWindow(this)) {
                messageViewer.Height = (ClientSize.Height * 25) / 100;
                UpdateMessageVisible();
            }

        }

        [LayoutEvent("hide-messages")]
        void HideMessages(LayoutEvent e) {
            if (e.IsThisFrameWindow(this)) {
                messageViewer.Height = 0;
                UpdateMessageVisible();
            }
        }

        [LayoutEvent("show-locomotives")]
        void ShowLocomotives(LayoutEvent e) {
            if (e.IsThisFrameWindow(this)) {
                locomotivesViewer.Width = 220;
                UpdateLocomotivesVisible();
            }
        }

        [LayoutEvent("hide-locomotives")]
        void HideLocomotives(LayoutEvent e) {
            if (e.IsThisFrameWindow(this)) {
                locomotivesViewer.Width = 0;
                UpdateLocomotivesVisible();
            }
        }

        [LayoutEvent("show-layout-control")]
        void ShowLayoutControl(LayoutEvent e) {
            if (e.IsThisFrameWindow(this)) {
                if (layoutControlViewer.Width < 20) {
                    if (layoutControlWidth > 20)
                        layoutControlViewer.Width = layoutControlWidth;
                    else
                        layoutControlViewer.Width = 220;
                }

                UpdateLayoutControlVisible();
            }
        }

        [LayoutEvent("hide-layout-control")]
        void HideLayoutControl(LayoutEvent e) {
            if (e.IsThisFrameWindow(this)) {
                layoutControlWidth = layoutControlViewer.Width;
                layoutControlViewer.Width = 0;
                UpdateLayoutControlVisible();
            }
        }


        [LayoutEvent("show-trips-monitor")]
        void ShowTripsMonitor(LayoutEvent e) {
            if (e.IsThisFrameWindow(this)) {
                tripsMonitor.Height = (ClientSize.Height * 12) / 100;
                UpdateTripsMonitorVisible();
            }
        }

        [LayoutEvent("hide-trips-monitor")]
        void HideTripsMonitor(LayoutEvent e) {
            if (e.IsThisFrameWindow(this)) {
                tripsMonitor.Height = 0;
                UpdateTripsMonitorVisible();
            }
        }

        [LayoutEvent("messages-shown")]
        private void messagesShown(LayoutEvent e) {
            if (e.IsThisFrameWindow(this))
                toolBarButtonShowMessages.Pushed = true;
        }

        [LayoutEvent("messages-hidden")]
        private void messagesHidden(LayoutEvent e) {
            if (e.IsThisFrameWindow(this))
                toolBarButtonShowMessages.Pushed = false;
        }

        [LayoutEvent("trips-monitor-shown")]
        private void tripsMonitorShown(LayoutEvent e) {
            if (e.IsThisFrameWindow(this))
                toolBarButtonShowTripsMonitor.Pushed = true;
        }

        [LayoutEvent("trips-monitor-hidden")]
        private void tripsMonitorHidden(LayoutEvent e) {
            if (e.IsThisFrameWindow(this))
                toolBarButtonShowTripsMonitor.Pushed = false;
        }

        [LayoutEvent("locomotives-shown")]
        private void locomotivesShown(LayoutEvent e) {
            if (e.IsThisFrameWindow(this))
                toolBarButtonShowLocomotives.Pushed = true;
        }

        [LayoutEvent("locomotives-hidden")]
        private void locomotivesHidden(LayoutEvent e) {
            if (e.IsThisFrameWindow(this))
                toolBarButtonShowLocomotives.Pushed = false;
        }

        [LayoutEvent("layout-control-shown")]
        private void layoutControlShown(LayoutEvent e) {
            if (e.IsThisFrameWindow(this))
                toolBarButtonShowLayoutControl.Pushed = true;
        }

        [LayoutEvent("layout-control-hidden")]
        private void layoutControlHidden(LayoutEvent e) {
            if (e.IsThisFrameWindow(this))
                toolBarButtonShowLayoutControl.Pushed = false;
        }

        #endregion


        [LayoutEvent("area-added")]
        private void areaAdded(LayoutEvent e0) {
            var e = (LayoutEvent<LayoutModelArea>)e0;
            var area = e.Sender;
            var areaPage = new LayoutFrameWindowAreaTabPage(area);

            tabAreas.TabPages.Add(areaPage);
            AddViewToArea(areaPage, "Overview");
        }

        [LayoutEvent("area-removed")]
        private void areaRemoved(LayoutEvent e0) {
            var e = (LayoutEvent<LayoutModelArea>)e0;
            var area = e.Sender;

            foreach (LayoutFrameWindowAreaTabPage areaTabPage in tabAreas.TabPages) {
                if (areaTabPage.Area == area) {
                    tabAreas.TabPages.Remove(areaTabPage);
                    break;
                }
            }
        }

        [LayoutEvent("area-renamed")]
        private void areaRenamed(LayoutEvent e0) {
            var e = (LayoutEvent<LayoutModelArea>)e0;
            var area = e.Sender;

            foreach (LayoutFrameWindowAreaTabPage areaTabPage in tabAreas.TabPages) {
                if (areaTabPage.Area == area) {
                    areaTabPage.Text = area.Name;
                    break;
                }
            }
        }

        private void LayoutView_ModelComponentClick(object sender, LayoutViewEventArgs e) {
            if (activeTool != null)
                activeTool.LayoutView_ModelComponentClick(sender, e);
        }

        private void LayoutView_ModelComponentDragged(object sender, LayoutViewEventArgs e) {
            if (activeTool != null)
                activeTool.LayoutView_ModelComponentDragged(sender, e);
        }

        private void LayoutView_ModelComponentQueryDrop(object sender, LayoutViewEventArgs e) {
            if (activeTool != null)
                activeTool.LayoutView_ModelComponentQueryDrop(sender, e);
        }

        private void LayoutView_ModelComponentDrop(object sender, LayoutViewEventArgs e) {
            if (activeTool != null)
                activeTool.LayoutView_ModelComponentDrop(sender, e);
        }

        private void LayoutView_MouseDown(object sender, MouseEventArgs e) {
            timerDetailsPopup.Stop();

            if (detailsPopupWindow != null) {
                detailsPopupWindow.Close();
                detailsPopupWindow = null;
                if (viewWithMouse != null)
                    viewWithMouse.Capture = false;
            }
        }

        private void LayoutView_MouseMove(object sender, MouseEventArgs e) {
            // Reset timer
            if (new Point(e.X, e.Y) != viewMouseLocation)
                timerDetailsPopup.Stop();

            LayoutView mouseView = sender as LayoutView;

            if (mouseView != viewWithMouse || new Point(e.X, e.Y) != viewMouseLocation) {
                bool closeIt = true;

                if (detailsPopupWindow != null) {
                    if (viewWithMouse != null) {
                        LayoutHitTestResult hit = viewWithMouse.HitTest(new Point(e.X, e.Y));

                        // Retain details popup window as long as mouse is in the same grid location
                        if (hit.ModelLocation == detailsPopupLocationInModelPoints)
                            closeIt = false;
                    }

                    if (closeIt) {
                        detailsPopupWindow.Close();
                        detailsPopupWindow = null;
                        viewWithMouse.Capture = false;
                    }
                }

                if (closeIt) {
                    viewWithMouse = sender as LayoutView;
                    viewMouseLocation = new Point(e.X, e.Y);

                    timerDetailsPopup.Start();          // Restart mouse timer
                }
            }
        }

        private void timerDetailsPopup_Tick(object sender, EventArgs e) {
            // Mouse did not move for enough time, need to show details popup window
            if (detailsPopupWindow == null) {
                if (viewWithMouse != null) {
                    LayoutHitTestResult hitTestResult = viewWithMouse.HitTest(viewMouseLocation);
                    LayoutModelSpotComponentCollection spot = hitTestResult.Area[hitTestResult.ModelLocation, LayoutModel.ActivePhases];
                    Rectangle bounds = new Rectangle(new Point(viewMouseLocation.X - SystemInformation.DragSize.Width / 2, viewMouseLocation.Y - SystemInformation.DragSize.Height / 2),
                        SystemInformation.DragSize);

                    if (spot != null && bounds.Contains(viewWithMouse.PointToClient(Control.MousePosition))) {
                        PopupWindowContainerSection container = new PopupWindowContainerSection(hitTestResult.View);

                        foreach (ModelComponent component in spot)
                            EventManager.Event(new LayoutEvent("add-" + (LayoutController.IsOperationMode ? "operation" : "editing") + "-details-window-sections", component, container).SetFrameWindow(this));

                        if (container.Count > 0) {
                            detailsPopupWindow = new DetailsPopupWindow(hitTestResult, container);
                            detailsPopupWindow.Show(this);
                            viewWithMouse.Capture = true;
                            detailsPopupLocationInModelPoints = hitTestResult.ModelLocation;
                        }
                    }
                }
            }

            timerDetailsPopup.Stop();
        }

        private void menuItemSave_Click(object sender, System.EventArgs e) {
            EventManager.Event(new LayoutEvent("save-layout", this));
        }

        private void menuSaveAs_Click(object sender, System.EventArgs e) {
        }

        private void menuItemOpen_Click(object sender, System.EventArgs e) {
        }

        private void menuItemNewLayout_Click(object sender, System.EventArgs e) {
        }

        class ButtonToMenuMap {
            public ToolBarButton theButton;
            public EventHandler theEvent;

            public ButtonToMenuMap(ToolBarButton button, EventHandler eh) {
                theButton = button;
                theEvent = eh;
            }
        }

        private void toolBarMain_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e) {
            ButtonToMenuMap[] map = new ButtonToMenuMap[] {
                new ButtonToMenuMap(toolBarButtonOpenLayiout, new EventHandler(menuItemOpen_Click)),
                new ButtonToMenuMap(toolBarButtonSaveLayout, new EventHandler(menuItemSave_Click)),
                new ButtonToMenuMap(toolBarButtonShowLocomotives, new EventHandler(menuItemShowLocomotives_Click)),
                new ButtonToMenuMap(toolBarButtonShowLayoutControl, new EventHandler(menuItemShowLayoutControl_Click)),
                new ButtonToMenuMap(toolBarButtonShowMessages, new EventHandler(menuItemShowMessages_Click)),
                new ButtonToMenuMap(toolBarButtonShowTripsMonitor, new EventHandler(menuItemShowTripsMonitor_Click)),
                new ButtonToMenuMap(toolBarButtonDesignMode, new EventHandler(menuItemDesign_Click)),
                new ButtonToMenuMap(toolBarButtonOperationMode, new EventHandler(menuItemOperational_Click)),
                new ButtonToMenuMap(toolBarButtonToggleAllLayoutManualDispatch, new EventHandler(toggleAllLayoutManualDispatch)),
                new ButtonToMenuMap(toolBarButtonStopAllLocomotives, new EventHandler(menuItemSuspendLocomotives_Click)),
                new ButtonToMenuMap(toolBarButtonEmergencyStop, new EventHandler(menuItemEmergencyStop_Click)),
                new ButtonToMenuMap(toolBarButtonZoomFitAll, new EventHandler(menuItemZoomAllArea_Click)),
                new ButtonToMenuMap(toolBarButtonZoom100, new EventHandler(toolbar_Zoom100)),
                new ButtonToMenuMap(toolBarButtonZoomIn, new EventHandler(toolbar_ZoomIn)),
                new ButtonToMenuMap(toolBarButtonZoomOut, new EventHandler(toolbar_ZoomOut)),
                new ButtonToMenuMap(toolBarButtonToggleGrid, new EventHandler(menuItemViewGrid_Click)),
                new ButtonToMenuMap(toolBarButtonSetToDefaultOriginAndZoom, new EventHandler(menuItemRestoreDefaultZoomAndOrigin_Click)),
                new ButtonToMenuMap(toolbarButtonSetAsDefaultOriginAndZoom, new EventHandler(menuItemSaveAsDefaultZoomAndOrigin_Click)),
            };

            if (e.Button.Visible && e.Button.Enabled) {
                if (e.Button == toolBarButtonSimulation) {
                    if (LayoutModel.Components<IModelComponentIsCommandStation>(LayoutPhase.All).Any(cs => cs.LayoutEmulationSupported)) {
                        var entries = GetPhasesEntries(simulate);

                        if (entries.Count == 1)
                            entries[0].Item2(this, new EventArgs());
                        else {
                            var menu = new ContextMenu();

                            foreach (var entry in entries)
                                menu.MenuItems.Add("Simulate " + entry.Item1, entry.Item2);

                            menu.Show(this, new Point(e.Button.Rectangle.Left, e.Button.Rectangle.Bottom));
                        }
                    }
                }
                else {
                    for (int i = 0; i < map.Length; i++) {
                        if (map[i].theButton == e.Button) {
                            map[i].theEvent(sender, e);
                            break;
                        }
                    }
                }
            }
        }

        private void toolbar_Zoom100(object sender, System.EventArgs e) {
            menuItemZoom100.PerformClick();
        }

        private void toolbar_ZoomOut(object sender, System.EventArgs e) {
            float zoom = ActiveView.Zoom - 0.1F;

            if (zoom < 0.1)
                zoom = 0.1F;
            ActiveView.Zoom = zoom;
        }

        private void toolbar_ZoomIn(object sender, System.EventArgs e) {
            float zoom = ActiveView.Zoom + 0.1F;

            if (zoom > 4)
                zoom = 4F;
            ActiveView.Zoom = zoom;
        }

        private void menuItemExit_Click(object sender, System.EventArgs e) {
            ControllerCommand = FrameWindowCommand.CloseLayout;
        }

        private void menuItemUndo_Click(object sender, System.EventArgs e) {
            LayoutController.CommandManager.Undo();
        }

        private void menuItemRedo_Click(object sender, System.EventArgs e) {
            LayoutController.CommandManager.Redo();
        }

        private void menuEdit_Popup(object sender, System.EventArgs e) {
            if (!LayoutController.IsOperationMode && LayoutController.CommandManager.CanUndo) {
                menuItemUndo.Enabled = true;
                menuItemUndo.Text = "&Undo " + LayoutController.CommandManager.UndoCommandName;
            }
            else {
                menuItemUndo.Enabled = false;
                menuItemUndo.Text = "&Undo";
            }

            if (!LayoutController.IsOperationMode && LayoutController.CommandManager.CanRedo) {
                menuItemRedo.Enabled = true;
                menuItemRedo.Text = "&Redo " + LayoutController.CommandManager.RedoCommandName;
            }
            else {
                menuItemRedo.Enabled = false;
                menuItemRedo.Text = "&Redo";
            }

            if (LayoutController.IsOperationMode || LayoutController.UserSelection.Count == 0) {
                menuItemCopy.Enabled = false;
                menuItemCut.Enabled = false;
            }
            else {
                menuItemCopy.Enabled = true;
                menuItemCut.Enabled = true;
            }
        }

        private void menuItemNewArea_Click(object sender, System.EventArgs e) {
            Dialogs.GetAreasName getAreaName = new Dialogs.GetAreasName();

            if (getAreaName.ShowDialog(this) == DialogResult.OK) {
                LayoutController.AddArea(getAreaName.AreaName);
            }
        }

        private void menuItemRenameArea_Click(object sender, System.EventArgs e) {
            Dialogs.GetAreasName getAreaName = new Dialogs.GetAreasName();

            if (getAreaName.ShowDialog(this) == DialogResult.OK)
                ((LayoutFrameWindowAreaTabPage)tabAreas.SelectedTab).Area.Name = getAreaName.AreaName;
        }

        private void menuItemStyleFonts_Click(object sender, System.EventArgs e) {
            Dialogs.StandardFonts standardFonts = new Dialogs.StandardFonts();

            standardFonts.ShowDialog(this);
            LayoutModel.WriteModelXmlInfo();
            LayoutModel.Instance.Redraw();
        }

        private void menuItemStylePositions_Click(object sender, System.EventArgs e) {
            Dialogs.StandardPositions standardPositions = new Dialogs.StandardPositions();

            standardPositions.ShowDialog();
            LayoutModel.WriteModelXmlInfo();
            LayoutModel.Instance.Redraw();
        }

        private void menuItemCopy_Click(object sender, System.EventArgs e) {
            MenuItemCopySelection copySelection = new MenuItemCopySelection(LayoutController.UserSelection.TopLeftLocation);

            copySelection.Copy();
        }

        private void menuItemCut_Click(object sender, System.EventArgs e) {
            MenuItemCutSelection cutSelection = new MenuItemCutSelection(LayoutController.UserSelection.TopLeftLocation);

            cutSelection.Cut();
        }

        private void menuItemModules_Click(object sender, System.EventArgs e) {
            Dialogs.ModuleManagement moduleManager = new Dialogs.ModuleManagement();

            moduleManager.ShowDialog(this);
        }

        private void menuItemTools_Popup(object sender, System.EventArgs e) {
            menuItemTools.MenuItems.Clear();

            EventManager.Event(new LayoutEvent("tools-menu-open-request", this, menuItemTools).SetFrameWindow(this));

            if (menuItemTools.MenuItems.Count == 0) {
                MenuItem noTools = new MenuItem("No tools") {
                    Enabled = false
                };
                menuItemTools.MenuItems.Add(noTools);
            }
        }

        private void menuItemAreaArrange_Click(object sender, System.EventArgs e) {
            Dialogs.ArrangeAreas arrangeAreas = new Dialogs.ArrangeAreas(tabAreas);

            arrangeAreas.ShowDialog(this);
        }

        private void menuItemDeleteArea_Click(object sender, System.EventArgs e) {
            LayoutFrameWindowAreaTabPage selectedAreaPage = (LayoutFrameWindowAreaTabPage)tabAreas.SelectedTab;
            LayoutModelArea selectedArea = selectedAreaPage.Area;

            // Check if area is empty
            if (selectedArea.Grid.Count > 0)
                MessageBox.Show(this, "This area is not empty, first remove all components, then delete the area", "Cannot delete area", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                LayoutModel.Areas.Remove(selectedArea);
        }

        private void menuItemView_Popup(object sender, System.EventArgs e) {
            if (ActiveView != null)
                menuItemViewGrid.Checked = ActiveView.ShowGrid != ShowGridLinesOption.Hide;

            if (messageViewVisible)
                menuItemShowMessages.Text = "Hide &Messages";
            else
                menuItemShowMessages.Text = "Show &Messages";

            if (locomotiveViewVisible)
                menuItemShowLocomotives.Text = "Hide &Locomotives";
            else
                menuItemShowLocomotives.Text = "Show &Locomotives";

            if (tripsMonitorVisible)
                menuItemShowTripsMonitor.Text = "Hide &Trips";
            else
                menuItemShowTripsMonitor.Text = "Show &Trips";

            if (layoutControlVisible)
                menuItemShowLayoutControl.Text = "Hide Layout &Control";
            else
                menuItemShowLayoutControl.Text = "Show Layout &Control";

            if (LayoutController.IsOperationMode) {
                if (LayoutModel.ModelPhases != LayoutModel.ActivePhases) {
                    menuItemViewActivePhases.Visible = true;
                    menuItemViewActivePhases.Checked = ActiveViewPage.ShowActivePhases;
                }
                else
                    menuItemViewActivePhases.Visible = false;
            }
        }


        private void menuItemViewGrid_Click(object sender, System.EventArgs e) {
            if (ActiveView != null) {
                if (ActiveView.ShowGrid != ShowGridLinesOption.Hide) {
                    ActiveView.ShowGrid = ShowGridLinesOption.Hide;
                    toolBarButtonToggleGrid.Pushed = false;
                }
                else {
                    ActiveView.ShowGrid = ShowGridLinesOption.AutoHide;
                    toolBarButtonToggleGrid.Pushed = true;
                }
            }
        }

        public LayoutView AddNewView() {
            Dialogs.GetViewName getViewName = new Dialogs.GetViewName();

            if (getViewName.ShowDialog() == DialogResult.OK) {
                LayoutView oldView = ActiveView;
                var tabViewPage = AddViewToArea(ActiveAreaPage, getViewName.ViewName);
                var newView = tabViewPage.View;

                if (oldView != null) {
                    newView.Zoom = oldView.Zoom;
                    newView.OriginInModelGridUnits = oldView.OriginInModelGridUnits;
                    newView.ShowGrid = oldView.ShowGrid;
                }
                return newView;
            }

            return null;
        }

        private void menuItemViewNew_Click(object sender, System.EventArgs e) {
            AddNewView();
        }

        private void menuItemViewDelete_Click(object sender, System.EventArgs e) {
            if (ActiveViewPage != null)
                ActiveAreaPage.TabViews.TabPages.Remove(ActiveViewPage);
        }

        private void menuItemViewRename_Click(object sender, System.EventArgs e) {
            if (ActiveViewPage != null) {
                Dialogs.GetViewName getViewName = new Dialogs.GetViewName();

                if (getViewName.ShowDialog() == DialogResult.OK)
                    ActiveViewPage.Text = getViewName.ViewName;
            }
        }

        private void menuItemViewArrage_Click(object sender, System.EventArgs e) {
            if (ActiveAreaPage != null) {
                Dialogs.ArrangeViews arrangeViews = new Dialogs.ArrangeViews(this, ActiveAreaPage);

                arrangeViews.ShowDialog();
            }
        }

        struct ZoomEntry {
            public int zoomFactor;
            public MenuItem menuItem;

            public ZoomEntry(int zoomFactor, MenuItem menuItem) {
                this.zoomFactor = zoomFactor;
                this.menuItem = menuItem;
            }
        }

        private void menuItemZoomMenu_Popup(object sender, System.EventArgs e) {
            if (ActiveView != null) {
                int currentZoom = (int)Math.Round(ActiveView.Zoom * 100);
                bool presetZoomFound = false;

                foreach (ZoomEntry zoomEntry in zoomEntries) {
                    zoomEntry.menuItem.Checked = (zoomEntry.zoomFactor == currentZoom);
                    if (zoomEntry.menuItem.Checked)
                        presetZoomFound = true;
                }

                menuItemSetZoom.Text = "Set zoom (" + currentZoom + "%)...";
                menuItemSetZoom.Checked = !presetZoomFound;
            }
        }

        private void menuItemZoomPreset_Click(object sender, System.EventArgs e) {
            MenuItem menuItem = (MenuItem)sender;

            if (ActiveView != null) {
                foreach (ZoomEntry zoomEntry in zoomEntries) {
                    if (zoomEntry.menuItem == menuItem) {
                        ActiveView.Zoom = (float)zoomEntry.zoomFactor / 100;
                        break;
                    }
                }
            }
        }

        private void LayoutController_Deactivate(object sender, System.EventArgs e) {
            EventManager.Event(new LayoutEvent("frame-window-deactivated", this).SetFrameWindow(this));
            timerFreeResources.Enabled = true;
        }

        private void LayoutController_Resize(object sender, System.EventArgs e) {
            if (this.WindowState == FormWindowState.Minimized) {
                EventManager.Event(new LayoutEvent("frame-window-minimized", this).SetFrameWindow(this));
                timerFreeResources.Enabled = true;
            }
            else
                timerFreeResources.Enabled = false;
        }

        private void menuItemSetZoom_Click(object sender, System.EventArgs e) {
            if (ActiveView != null) {
                Dialogs.SetZoom setZoom = new Dialogs.SetZoom {
                    ZoomFactor = (int)Math.Round(ActiveView.Zoom * 100)
                };

                if (setZoom.ShowDialog(this) == DialogResult.OK)
                    ActiveView.Zoom = (float)setZoom.ZoomFactor / 100;
            }
        }

        private void menuItemZoomAllArea_Click(object sender, System.EventArgs e) {
            if (ActiveView != null)
                ActiveView.ShowAllArea();
        }

        private void menuItemShowMessages_Click(object sender, System.EventArgs e) {
            if (messageViewVisible)
                EventManager.Event(new LayoutEvent("hide-messages", this).SetFrameWindow(this));
            else
                EventManager.Event(new LayoutEvent("show-messages", this).SetFrameWindow(this));
        }

        private void menuItemShowTripsMonitor_Click(object sender, System.EventArgs e) {
            if (tripsMonitorVisible)
                EventManager.Event(new LayoutEvent("hide-trips-monitor", this).SetFrameWindow(this));
            else
                EventManager.Event(new LayoutEvent("show-trips-monitor", this).SetFrameWindow(this));
        }

        private void menuItemShowLocomotives_Click(object sender, System.EventArgs e) {
            if (locomotiveViewVisible)
                EventManager.Event(new LayoutEvent("hide-locomotives", this).SetFrameWindow(this));
            else
                EventManager.Event(new LayoutEvent("show-locomotives", this).SetFrameWindow(this));
        }

        private void menuItemShowLayoutControl_Click(object sender, System.EventArgs e) {
            if (layoutControlVisible)
                EventManager.Event(new LayoutEvent("hide-layout-control", this).SetFrameWindow(this));
            else
                EventManager.Event(new LayoutEvent("show-layout-control", this).SetFrameWindow(this));
        }

        private void menuItemOperational_Click(object sender, System.EventArgs e) {
            LayoutController.Instance.EnterOperationModeRequest(new OperationModeParameters() { Phases = LayoutPhase.Operational, Simulation = false });
        }

        private void menuItemDesign_Click(object sender, System.EventArgs e) {
            LayoutController.Instance.EnterDesignModeRequest();
        }

        private void menuItemLocomotiveCatalog_Click(object sender, System.EventArgs e) {
            new Dialogs.LocomotiveCatalog().ShowDialog(this);
        }

        private void splitterMessages_SplitterMoved(object sender, System.Windows.Forms.SplitterEventArgs e) {
            UpdateMessageVisible();
        }

        private void splitterTripsMonitor_SplitterMoved(object sender, System.Windows.Forms.SplitterEventArgs e) {
            UpdateTripsMonitorVisible();
        }

        private void splitterLocomotives_SplitterMoved(object sender, System.Windows.Forms.SplitterEventArgs e) {
            UpdateLocomotivesVisible();
        }

        private void timerFreeResources_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            // 30 seconds (or so) were passed while the application was minimized or deactivated
            // recoverable resources can be disposed.
            Debug.WriteLine("--- LayoutManager application not active - free resources ---");
            EventManager.Event(new LayoutEvent("free-resources", this).SetFrameWindow(this));
            timerFreeResources.Enabled = false;
        }

        private void LayoutController_Activated(object sender, System.EventArgs e) {
            timerFreeResources.Enabled = false;
        }

        private void menuItemManualDispatchRegions_Popup(object sender, System.EventArgs e) {
            menuItemManualDispatchRegions.MenuItems.Clear();

            foreach (ManualDispatchRegionInfo manualDispatchRegion in LayoutModel.StateManager.ManualDispatchRegions)
                menuItemManualDispatchRegions.MenuItems.Add(new ManualDispatchRegionItem(manualDispatchRegion));

            if (menuItemManualDispatchRegions.MenuItems.Count > 0)
                menuItemManualDispatchRegions.MenuItems.Add("-");

            menuItemManualDispatchRegions.MenuItems.Add(new AllLayoutManualDispatchItem());
            menuItemManualDispatchRegions.MenuItems.Add("-");

            menuItemManualDispatchRegions.MenuItems.Add("Manage Manual Dispatch Regions...", new EventHandler(this.onManageManualDispatchRegions));
        }

        private void onManageManualDispatchRegions(object sender, EventArgs e) {
            LayoutManager.Tools.Dialogs.ManualDispatchRegions d = new LayoutManager.Tools.Dialogs.ManualDispatchRegions();

            d.Show();
        }

        private void menuItemPolicies_Click(object sender, System.EventArgs e) {
            Form d = (Form)EventManager.Event(new LayoutEvent("query-policies-definition-dialog", this));

            if (d != null)
                d.Activate();
            else {
                Dialogs.PoliciesDefinition p = new Dialogs.PoliciesDefinition {
                    Owner = this
                };
                p.Show();
            }
        }

        private void menuItemAccelerationProfiles_Click(object sender, System.EventArgs e) {
            LayoutManager.Tools.Dialogs.MotionRamps rampsDialog = new LayoutManager.Tools.Dialogs.MotionRamps(LayoutModel.Instance.Ramps);

            rampsDialog.ShowDialog();
            LayoutModel.WriteModelXmlInfo();
        }

        private void menuItemDefaultDriverParameters_Click(object sender, System.EventArgs e) {
            LayoutManager.Tools.Dialogs.DefaultDrivingParameters d = new LayoutManager.Tools.Dialogs.DefaultDrivingParameters();

            d.ShowDialog(this);
        }

        private void menuItemCommonDestinations_Click(object sender, System.EventArgs e) {
            LayoutManager.Tools.Dialogs.TripPlanCommonDestinations d = new LayoutManager.Tools.Dialogs.TripPlanCommonDestinations {
                Owner = this
            };
            d.Show();
        }

        class ManualDispatchRegionItem : MenuItem {
            readonly ManualDispatchRegionInfo manualDispatchRegion;

            public ManualDispatchRegionItem(ManualDispatchRegionInfo manualDispatchRegion) {
                this.manualDispatchRegion = manualDispatchRegion;

                this.Text = manualDispatchRegion.Name;

                if (LayoutModel.StateManager.AllLayoutManualDispatch)
                    Enabled = false;
                else
                    this.Checked = manualDispatchRegion.Active;
            }

            protected override void OnClick(EventArgs e) {
                try {
                    if (manualDispatchRegion.Active)
                        manualDispatchRegion.Active = false;
                    else
                        manualDispatchRegion.Active = true;

                    EventManager.Event(new LayoutEvent("manual-dispatch-region-status-changed", manualDispatchRegion, manualDispatchRegion.Active));
                }
                catch (LayoutException ex) {
                    MessageBox.Show("Could change manual dispatch mode because: " + ex.Message, "Unable to change Manual Dispatch Mode",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        class AllLayoutManualDispatchItem : MenuItem {
            public AllLayoutManualDispatchItem() {
                Text = "All layout";

                foreach (ManualDispatchRegionInfo manualDispatchRegion in LayoutModel.StateManager.ManualDispatchRegions)
                    if (manualDispatchRegion.Active) {
                        Enabled = false;
                        break;
                    }

                if (Enabled)
                    Checked = LayoutModel.StateManager.AllLayoutManualDispatch;
            }

            protected override void OnClick(EventArgs e) {
                try {
                    LayoutModel.StateManager.AllLayoutManualDispatch = !LayoutModel.StateManager.AllLayoutManualDispatch;
                }
                catch (LayoutException ex) {
                    MessageBox.Show("Could not set all layout to manual dispatch mode because: " + ex.Message, "Unable to set Manual Dispatch Mode",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void toggleAllLayoutManualDispatch(object sender, EventArgs e) {
            new AllLayoutManualDispatchItem().PerformClick();
        }

        private void menuItemSuspendLocomotives_Click(object sender, System.EventArgs e) {
            bool allSuspended = (bool)EventManager.Event(new LayoutEvent("are-all-locomotives-suspended", this));

            if (allSuspended)
                EventManager.Event(new LayoutEvent("resume-all-locomotives", this));
            else
                EventManager.Event(new LayoutEvent("suspend-all-locomotives", this));
        }

        private List<Tuple<string, EventHandler>> GetPhasesEntries(Action<LayoutPhase> doThis) {
            var entries = new List<Tuple<string, EventHandler>>();

            if (LayoutModel.HasPhase(LayoutPhase.Operational))
                entries.Add(new Tuple<string, EventHandler>("Operational region", (s, ea) => { doThis(LayoutPhase.Operational); }));
            if (LayoutModel.HasPhase(LayoutPhase.Construction))
                entries.Add(new Tuple<string, EventHandler>("In construction + Operational regions", (s, ea) => { doThis(LayoutPhase.NotPlanned); }));
            if (LayoutModel.HasPhase(LayoutPhase.Planned))
                entries.Add(new Tuple<string, EventHandler>("All diagram", (s, ea) => { doThis(LayoutPhase.All); }));

            return entries;
        }

        private EventHandler simulateEventHandler;
        private EventHandler verifyLayoutEventHandler;

        private void menuLayout_Popup(object sender, System.EventArgs e) {
            bool allSuspended = (bool)EventManager.Event(new LayoutEvent("are-all-locomotives-suspended", this));

            if (allSuspended)
                menuItemSuspendLocomotives.Text = "&Resume train operation";
            else
                menuItemSuspendLocomotives.Text = "&Stop trains";

            var simulateEntries = GetPhasesEntries(simulate);
            menuItemSimulation.MenuItems.Clear();

            if (LayoutModel.Components<IModelComponentIsCommandStation>(LayoutPhase.All).Any(cs => cs.LayoutEmulationSupported)) {
                if (simulateEntries.Count == 1)
                    simulateEventHandler = simulateEntries[0].Item2;
                else {
                    foreach (var entry in simulateEntries)
                        menuItemSimulation.MenuItems.Add(entry.Item1, entry.Item2);
                }
            }
            else
                menuItemSimulation.Enabled = false;

            var verifyLayoutEntries = GetPhasesEntries(verify);
            menuItemCompile.MenuItems.Clear();

            if (verifyLayoutEntries.Count == 1)
                verifyLayoutEventHandler = verifyLayoutEntries[0].Item2;
            else {
                foreach (var entry in verifyLayoutEntries)
                    menuItemCompile.MenuItems.Add(entry.Item1, entry.Item2);
            }
        }

        private void menuItemSimulation_Click(object sender, EventArgs e) {
            simulateEventHandler(sender, e);
        }

        private void menuItemCompile_Click(object sender, System.EventArgs e) {
            verifyLayoutEventHandler(sender, e);
        }

        private void simulate(LayoutPhase phases) {
            LayoutController.Instance.EnterOperationModeRequest(new OperationModeParameters() { Phases = phases, Simulation = true });
        }

        private void verify(LayoutPhase phases) {
            EventManager.Event(new LayoutEvent("clear-messages", this));
            if ((bool)EventManager.Event(new LayoutEvent("check-layout", LayoutModel.Instance, true).SetPhases(phases)))
                MessageBox.Show(this, "Layout checked, all seems to be OK", "Layout Check Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuItemEmergencyStop_Click(object sender, System.EventArgs e) {
            EventManager.Event(new LayoutEvent<IModelComponentIsCommandStation, string>("emergency-stop-request", null, "User initiated"));
        }

        private void menuItemConnectLayout_Click(object sender, System.EventArgs e) {
            EventManager.Event(new LayoutEvent("connect-layout-to-control-request", this));
        }

        private void menuItemLearnLayout_Click(object sender, EventArgs e) {
            try {
                if (LayoutController.Instance.BeginDesignTimeActivation()) {
                    Form learnLayoutForm = (Form)EventManager.Event(new LayoutEvent("activate-learn-layout", this));

                    if (learnLayoutForm == null) {
                        Dialogs.LearnLayout learnLayout = new LayoutManager.Dialogs.LearnLayout(Id) {
                            Owner = this
                        };
                        learnLayout.Show();
                    }
                    else
                        learnLayoutForm.Activate();
                }
                else
                    throw new LayoutException("Command station does not support powering the layout while in design mode");
            }
            catch (Exception ex) {
                MessageBox.Show(this, "Error when activating layout: " + ex.Message, "Error in layout activation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void menuItemFind_Click(object sender, EventArgs e) {
            Dialogs.FindComponents d = new LayoutManager.Dialogs.FindComponents(ActiveAreaPage.Area);

            d.ShowDialog(this);
        }

        private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e) {
            LayoutFrameWindowAreaTabPage areaPage;
            ILayoutView view;

            if (printState.AllAreas)
                areaPage = (LayoutFrameWindowAreaTabPage)tabAreas.TabPages[printState.AreaIndex];
            else
                areaPage = ActiveAreaPage;

            if (printState.PrintViewScope == PrintViewScope.All)
                view = ((LayoutFrameWindowAreaViewTabPage)areaPage.TabViews.TabPages[printState.ViewIndex]).View;
            else
                view = ((LayoutFrameWindowAreaViewTabPage)areaPage.TabViews.SelectedTab).View;

            GraphicsState gs = e.Graphics.Save();

            Point[] pt = new Point[2];

            // Get printing area in pixels
            pt[0] = e.MarginBounds.Location;
            pt[1] = new Point(e.MarginBounds.Right, e.MarginBounds.Bottom);
            e.Graphics.TransformPoints(CoordinateSpace.Device, CoordinateSpace.Page, pt);

            Rectangle printingArea = Rectangle.FromLTRB(pt[0].X, pt[0].Y, pt[1].X, pt[1].Y);
            Rectangle modelAreaInGridUnits;

            e.Graphics.PageUnit = GraphicsUnit.Pixel;

            if (printState.PrintViewScope != PrintViewScope.BestFit)
                modelAreaInGridUnits = new Rectangle(view.OriginInModelGridUnits, Size.Truncate(view.ClientSizeInModelGridUnits));
            else {
                modelAreaInGridUnits = ActiveAreaPage.Area.Bounds;
                modelAreaInGridUnits.Inflate(2, 2);
            }

            view.Draw(e.Graphics, printingArea, modelAreaInGridUnits, printState.GridLines ? 2 : 0, false);

            e.Graphics.Restore(gs);

            bool moreViews = printState.PrintViewScope == PrintViewScope.All && printState.ViewIndex < areaPage.TabViews.TabPages.Count - 1;

            if (!moreViews) {
                bool moreAreas = printState.AllAreas && printState.AreaIndex < tabAreas.TabPages.Count - 1;

                if (!moreAreas)
                    e.HasMorePages = false;
                else {
                    printState.AreaIndex++;
                    e.HasMorePages = true;
                }
            }
            else {
                printState.ViewIndex++;
                e.HasMorePages = true;
            }
        }

        private void menuItemPrint_Click(object sender, EventArgs e) {
            printState = new PrintState(new Dialogs.Print(printDocument));
            if (printState.PrintDialog.ShowDialog(this) == DialogResult.OK) {
                printDocument.DocumentName = LayoutController.LayoutFilename ?? "Untitled";
                printDocument.Print();
            }
            else
                printState = null;
        }

        private void menuItemRestoreDefaultZoomAndOrigin_Click(object sender, EventArgs e) {
            if (ActiveView != null)
                ActiveView.SetOriginAndZoomToDefault();
        }

        private void menuItemSaveAsDefaultZoomAndOrigin_Click(object sender, EventArgs e) {
            if (ActiveView != null && MessageBox.Show("Do you want to set current position and zoom as the default for this view?", "Set Default Origin & View", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                ActiveView.SetAsDefaultOriginAndZoom();
        }

        private void menuItemSelectUnconnectedComponents_Click(object sender, EventArgs e) {
            var selection = new LayoutSelection();

            foreach (IModelComponentConnectToControl component in LayoutModel.Components<IModelComponentConnectToControl>(LayoutPhase.All))
                if (!component.FullyConnected)
                    selection.Add(component.Id);

            if (selection.Count == 0)
                LayoutModuleBase.Message("All components are wired to control modules");
            else
                LayoutModuleBase.Message(selection, "Components which are not fully wired to control modules");

            EventManager.Event(new LayoutEvent("show-messages", this));
        }

        private void menuItemSelectUnlinkedSignals_Click(object sender, EventArgs e) {
            Dictionary<Guid, LayoutBlockEdgeBase> map = (Dictionary<Guid, LayoutBlockEdgeBase>)EventManager.Event(new LayoutEvent("get-linked-signal-map", this));
            LayoutSelection selection = new LayoutSelection();

            foreach (LayoutSignalComponent signal in LayoutModel.Components<LayoutSignalComponent>(LayoutPhase.All))
                if (!map.ContainsKey(signal.Id))
                    selection.Add(signal);

            if (selection.Count == 0)
                LayoutModuleBase.Message("All signals are linked to logical signals (represented by block edge components)");
            else
                LayoutModuleBase.Message(selection, "Singals which are not linked. You should link block edges to signals");

            EventManager.Event(new LayoutEvent("show-messages", this));
        }

        private void menuItemSelectUnlinkedTrackLinks_Click(object sender, EventArgs e) {
            LayoutSelection selection = new LayoutSelection();

            foreach (LayoutTrackLinkComponent trackLink in LayoutModel.Components<LayoutTrackLinkComponent>(LayoutPhase.All))
                if (trackLink.LinkedComponent == null)
                    selection.Add(trackLink);

            if (selection.Count == 0)
                LayoutModuleBase.Message("All track links are linked");
            else
                LayoutModuleBase.Message(selection, "Track links which are not yet linked");

            EventManager.Event(new LayoutEvent("show-messages", this));
        }

        private void menuItemNewComponentDesignPhase_Click(object sender, EventArgs e) {
            var command = new ChangeDefaultPhaseCommand(LayoutPhase.Planned);

            LayoutController.Do(command);
        }

        private void menuItemNewComponentConstructionPhase_Click(object sender, EventArgs e) {
            var command = new ChangeDefaultPhaseCommand(LayoutPhase.Construction);

            LayoutController.Do(command);
        }

        private void menuItemNewComponentOperationalPhase_Click(object sender, EventArgs e) {
            var command = new ChangeDefaultPhaseCommand(LayoutPhase.Operational);

            LayoutController.Do(command);
        }

        private void menuItemSetToDesignPhase_Click(object sender, EventArgs e) {
            EventManager.Event(new LayoutEventInfoValueType<LayoutSelection, LayoutPhase>("change-phase", null, LayoutPhase.Planned));
        }

        private void menuItemSetToConstructionPhase_Click(object sender, EventArgs e) {
            EventManager.Event(new LayoutEventInfoValueType<LayoutSelection, LayoutPhase>("change-phase", null, LayoutPhase.Construction));
        }

        private void menuItemSetToOperationalPhase_Click(object sender, EventArgs e) {
            EventManager.Event(new LayoutEventInfoValueType<LayoutSelection, LayoutPhase>("change-phase", null, LayoutPhase.Operational));
        }

        private void menuItemDefaultPhase_Popup(object sender, EventArgs e) {
            menuItemNewComponentDesignPhase.Checked = LayoutModel.Instance.DefaultPhase == LayoutPhase.Planned;
            menuItemNewComponentConstructionPhase.Checked = LayoutModel.Instance.DefaultPhase == LayoutPhase.Construction;
            menuItemNewComponentOperationalPhase.Checked = LayoutModel.Instance.DefaultPhase == LayoutPhase.Operational;
        }

        private void menuItemViewActivePhases_Click(object sender, EventArgs e) {
            menuItemViewActivePhases.Checked = !menuItemViewActivePhases.Checked;
            ActiveViewPage.ShowActivePhases = menuItemViewActivePhases.Checked;

            LayoutPhase phases = ActiveViewPage.ShowActivePhases ? LayoutModel.ActivePhases : LayoutPhase.All;

            if (phases != ActiveViewPage.View.Phases) {
                ActiveViewPage.View.Phases = phases;
                ActiveViewPage.View.Invalidate();
            }
        }

        private void menuItemNewWindow_Click(object sender, EventArgs e) {
            ControllerCommand = FrameWindowCommand.NewWindow;
        }

        private void menuItemCloseWindow_Click(object sender, EventArgs e) {
            Close();
        }

        private void menuItemSelectPowerConnectors_Click(object sender, EventArgs e) {
            var selection = new LayoutSelection(LayoutModel.Components<LayoutTrackPowerConnectorComponent>(LayoutPhase.All));

            if (selection.Count == 0)
                LayoutModuleBase.Warning("No track power connectors were found");
            else
                LayoutModuleBase.Message(selection, "Track power connectors");
        }

        private void menuItemSelectReverseLoopModules_Click(object sender, EventArgs e) {
            var selection = new LayoutSelection(LayoutModel.AllComponents<LayoutTrackReverseLoopModule>(LayoutPhase.All));

            if (selection.Count == 0)
                LayoutModuleBase.Warning("No reverse loop modules were found");
            else
                LayoutModuleBase.Message(selection, "Reverse loop modules");

        }

        private void menuItemVerificationOptions_Click(object sender, EventArgs e) {
            var d = new LayoutManager.Tools.Dialogs.LayoutVerificationOptions();

            d.ShowDialog(this);
        }

        private void menuItemTrainTracking_Click(object sender, EventArgs e) {
            var d = new CommonUI.Dialogs.TrainTrackingOptions();

            d.ShowDialog();
        }
    }

    public class LayoutFrameWindowAreaTabPage : TabPage {
        public LayoutFrameWindowAreaTabPage(LayoutModelArea area) {
            this.Area = area;

            TabViews = new TabControl {
                Dock = DockStyle.Fill,
                Alignment = TabAlignment.Bottom
            };
            TabViews.SelectedIndexChanged += new EventHandler(tabViews_SelectedIndexChanged);

            this.Text = Area.Name;
            this.Controls.Add(TabViews);
        }

        public LayoutModelArea Area {
            get;

        }

        public TabControl TabViews {
            get;

        }

        public LayoutFrameWindow FrameWindow => (LayoutFrameWindow)this.FindForm();

        protected override void Dispose(bool disposing) {
            if (disposing)
                TabViews.SelectedIndexChanged -= new EventHandler(tabViews_SelectedIndexChanged);
        }

        void tabViews_SelectedIndexChanged(object sender, EventArgs e) {
            FrameWindow.OnViewChanged();
        }
    }


    public class LayoutFrameWindowAreaViewTabPage : TabPage {
        readonly LayoutView view;

        public LayoutFrameWindowAreaViewTabPage(LayoutView view) {
            this.view = view;
            view.Dock = DockStyle.Fill;
            Controls.Add(view);
            this.BorderStyle = BorderStyle.None;
            ShowActivePhases = false;
        }

        public LayoutView View => view;

        public bool ShowActivePhases {
            get;
            set;
        }

        public void WriteXml(XmlWriter w) {
            w.WriteStartElement("View");
            w.WriteAttributeString("AreaID", XmlConvert.EncodeName(view.Area.AreaGuid.ToString()));
            w.WriteAttributeString("Name", XmlConvert.EncodeLocalName(this.Text));
            this.View.WriteXml(w);
            w.WriteEndElement();
        }
    }

    internal class PrintState {
        readonly Dialogs.Print printDialog;
        int areaIndex;
        int viewIndex;

        public PrintState(Dialogs.Print printDialog) {
            this.printDialog = printDialog;
        }

        public Dialogs.Print PrintDialog => printDialog;

        public bool AllAreas => printDialog.AllAreas;

        public PrintViewScope PrintViewScope => PrintDialog.PrintViewScope;

        public bool GridLines => printDialog.GridLines;

        public int AreaIndex {
            get => areaIndex;

            set {
                areaIndex = value;
                viewIndex = 0;
            }
        }

        public int ViewIndex {
            get => viewIndex;

            set => viewIndex = value;
        }
    }

    public class FrameWindowState : LayoutXmlWrapper {
        public FrameWindowState(XmlElement element)
            : base(element) {
        }

        public FrameWindowState(XmlElement windowStatesElement, LayoutFrameWindow frameWindow) : base(windowStatesElement, "WindowState", alwaysAppend: true) {
            WindowState = frameWindow.WindowState;
            Bounds = frameWindow.Bounds;

            XmlElement areaStatesElement = CreateChildElement("AreaStates");

            foreach (var areaTab in frameWindow.AreaTabs)
                new FrameWindowAreaState(areaStatesElement, areaTab);

            this.ActiveAreaIndex = frameWindow.ActiveAreaIndex;

        }

        public void Restore(List<LayoutFrameWindow> frameWindows) {
            frameWindows.Add(new LayoutFrameWindow(this));
        }

        public void Restore(TabControl tabAreas) {
            foreach (var areaState in AreaStates)
                areaState.Restore(tabAreas);

            tabAreas.SelectedIndex = this.ActiveAreaIndex;
        }

        public FormWindowState WindowState {
            get => AttributeValue("WindowState").Enum<FormWindowState>() ?? FormWindowState.Normal;

            set => SetAttribute("WindowState", value.ToString());
        }

        public Rectangle Bounds {
            get => new Rectangle((int)AttributeValue("Left"), (int)AttributeValue("Top"), (int)AttributeValue("Width"), (int)AttributeValue("Height"));

            set {
                SetAttribute("Left", value.Left);
                SetAttribute("Top", value.Top);
                SetAttribute("Width", value.Width);
                SetAttribute("Height", value.Height);
            }
        }

        public IEnumerable<FrameWindowAreaState> AreaStates {
            get {
                XmlElement areasElement = Element["AreaStates"];

                if (areasElement != null)
                    foreach (XmlElement areaElement in areasElement)
                        yield return new FrameWindowAreaState(areaElement);
            }
        }

        public int ActiveAreaIndex {
            get => (int?)AttributeValue("ActiveAreaIndex") ?? 0;
            set => SetAttribute("ActiveAreaIndex", value);
        }

        public int ControlViewerWidth {
            get => (int?)AttributeValue("ControlViewerWidth") ?? 0;
            set => SetAttribute("ControlViewerWidth", value);
        }

        public int LocomotiveViewerWidth {
            get => (int?)AttributeValue("LocomotiveViewerWidth") ?? 0;
            set => SetAttribute("LocomotiveViewerWidth", value);
        }

        public int TripsViewerHeight {
            get => (int?)AttributeValue("TripsViewerHeight") ?? 0;
            set => SetAttribute("TripsViewerHeight", value);
        }

        public int MessagesViewerHeight {
            get => (int?)AttributeValue("MessagesViewerHeight") ?? 0;
            set => SetAttribute("MessagesViewerHeight", value);
        }
    }

    public class FrameWindowAreaState : LayoutXmlWrapper {
        public FrameWindowAreaState(XmlElement element)
            : base(element) {
        }

        public FrameWindowAreaState(XmlElement areaStatesElement, LayoutFrameWindowAreaTabPage areaTab)
          : base(areaStatesElement, "AreaState", alwaysAppend: true) {
            AreaId = areaTab.Area.AreaGuid;

            XmlElement viewStatesElement = CreateChildElement("ViewStates");

            foreach (LayoutFrameWindowAreaViewTabPage viewTab in areaTab.TabViews.TabPages)
                new FrameWindowViewState(viewStatesElement, viewTab);

            ActiveViewIndex = areaTab.TabViews.SelectedIndex;
        }

        public void Restore(TabControl tabAreas) {
            LayoutModelArea area = LayoutModel.Areas[AreaId];

            if (area != null) {
                LayoutFrameWindowAreaTabPage areaTab = new LayoutFrameWindowAreaTabPage(area);

                tabAreas.TabPages.Add(areaTab);

                foreach (var viewState in ViewStates)
                    viewState.Restore(areaTab);

                areaTab.TabViews.SelectedIndex = ActiveViewIndex;
            }
        }

        public Guid AreaId {
            get => (Guid)AttributeValue("ID");
            set => SetAttribute("ID", value);
        }

        public IEnumerable<FrameWindowViewState> ViewStates {
            get {
                XmlElement viewsElement = Element["ViewStates"];

                if (viewsElement != null)
                    foreach (XmlElement viewElement in viewsElement)
                        yield return new FrameWindowViewState(viewElement);
            }
        }

        public int ActiveViewIndex {
            get => (int?)AttributeValue("ActiveViewIndex") ?? 0;
            set => SetAttribute("ActiveViewIndex", value);
        }

    }

    public class FrameWindowViewState : LayoutXmlWrapper {
        public FrameWindowViewState(XmlElement element)
            : base(element) {
        }

        public FrameWindowViewState(XmlElement viewStatesElement, LayoutFrameWindowAreaViewTabPage viewPage)
         : base(viewStatesElement, "ViewState", alwaysAppend: true) {

            Name = viewPage.Text;
            Zoom = viewPage.View.Zoom;
            DefaultZoom = viewPage.View.DefaultZoom;
            Origin = viewPage.View.OriginInModelGridUnits;
            DefaultOrigin = viewPage.View.DefaultOriginInModelGridUnits;
            Grid = viewPage.View.ShowGrid;
            ShowCoordinates = viewPage.View.ShowCoordinates;
            ShowActivePhases = viewPage.ShowActivePhases;
        }

        public void Restore(LayoutFrameWindowAreaTabPage areaPage) {
            var view = new LayoutView() {
                Area = areaPage.Area,
                Zoom = this.Zoom,
                DefaultZoom = this.DefaultZoom,
                OriginInModelGridUnits = this.Origin,
                DefaultOriginInModelGridUnits = this.DefaultOrigin,
                ShowGrid = this.Grid,
                ShowCoordinates = this.ShowCoordinates,
            };

            areaPage.FrameWindow.AddViewToArea(areaPage, this.Name, view).ShowActivePhases = this.ShowActivePhases;
        }

        public string Name {
            get => GetAttribute("Name");
            set => SetAttribute("Name", value);
        }

        public float Zoom {
            get => (float?)AttributeValue("Zoom") ?? 1.0f;
            set => SetAttribute("Zoom", XmlConvert.ToString(value));
        }

        public float DefaultZoom {
            get => (float?)AttributeValue("DefaultZoom") ?? 1.0f;
            set => SetAttribute("DefaultZoom", XmlConvert.ToString(value));
        }

        public Point Origin {
            get => new Point((int?)AttributeValue("OriginX") ?? 0, (int?)AttributeValue("OriginY") ?? 0);
            set {
                SetAttribute("OriginX", value.X);
                SetAttribute("OriginY", value.Y);
            }
        }

        public Point DefaultOrigin {
            get => new Point((int?)AttributeValue("DefaultOriginX") ?? 0, (int?)AttributeValue("DefaultOriginY") ?? 0);
            set {
                SetAttribute("DefaultOriginX", value.X);
                SetAttribute("DefaultOriginY", value.Y);
            }
        }

        public ShowGridLinesOption Grid {
            get => AttributeValue("Grid").Enum<ShowGridLinesOption>() ?? ShowGridLinesOption.Hide;
            set => SetAttribute("Grid", value.ToString());
        }

        public bool ShowCoordinates {
            get => (bool?)AttributeValue("ShowCoordinates") ?? false;
            set => SetAttribute("ShowCoordinates", XmlConvert.ToString(value));
        }

        public bool ShowActivePhases {
            get => (bool?)AttributeValue("ShowActivePhases") ?? false;
            set => SetAttribute("ShowActivePhases", XmlConvert.ToString(value));
        }

    }

}
