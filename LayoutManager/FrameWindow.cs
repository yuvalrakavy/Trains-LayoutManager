using LayoutManager.CommonUI;
using LayoutManager.Components;
using LayoutManager.Model;
using LayoutManager.View;
using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager {
    public partial class FrameWindow : Form, ILayoutFrameWindow {
        private TaskCompletionSource<FrameWindowAction>? tcs;

        private LayoutTool? designTool = null;
        private LayoutTool? operationTool = null;
        private LayoutTool? activeTool = null;
        private ZoomEntry[] zoomEntries = Array.Empty<ZoomEntry>();

        private bool layoutControlVisible = false;
        private bool tripsMonitorVisible = false;
        private bool messageViewVisible = false;
        private bool locomotiveViewVisible = false;

        private int tripsMonitorHeightInOperationMode = 0;

        private PrintState? printState = null;

        private DetailsPopupWindow? detailsPopupWindow;
        private LayoutView? viewWithMouse;

        private Point viewMouseLocation = new(0, 0);
        private Point detailsPopupLocationInModelPoints = new(0, 0);

        private OperationModeParameters? uiMode = null;

        #region UI setting classes

        private enum UIitemSetting {
            Hidden,
            Disabled
        };

        private abstract class UIsettingEntry {
            protected UIitemSetting setting;

            public UIitemSetting Setting => setting;
        }

        private class DoSettingEntry : UIsettingEntry {
            public UIsettingEntry[] DoThis { get; }

            public DoSettingEntry(UIsettingEntry[] doThis) {
                this.DoThis = doThis;
            }
        }

        private class MenuSettingEntry : UIsettingEntry {
            public MenuSettingEntry(ToolStripItem menuItem, UIitemSetting setting) {
                this.MenuItem = menuItem;
                this.setting = setting;
            }

            public ToolStripItem MenuItem { get; }
        }

        private class ToolbarSettingEntry : UIsettingEntry {
            public ToolbarSettingEntry(ToolStripButton button, UIitemSetting setting) {
                this.Button = button;
                this.setting = setting;
            }

            public ToolStripButton Button { get; }
        }

        private class SeparatorSettingEntry : UIsettingEntry {
            public SeparatorSettingEntry(ToolStripSeparator separator, UIitemSetting setting) {
                this.Separator = separator;
                this.setting = setting;
            }

            public ToolStripSeparator Separator { get; }
        }

        private struct ZoomEntry {
            public int zoomFactor;
            public ToolStripMenuItem menuItem;

            public ZoomEntry(int zoomFactor, ToolStripMenuItem menuItem) {
                this.zoomFactor = zoomFactor;
                this.menuItem = menuItem;
            }
        }


        #endregion

        private UIsettingEntry[] operationBaseUIsetting = Array.Empty<UIsettingEntry>();
        private UIsettingEntry[] operationModeUIsetting = Array.Empty<UIsettingEntry>();
        private UIsettingEntry[] simulationModeUIsetting = Array.Empty<UIsettingEntry>();

        private UIsettingEntry[] designModeUIsetting = Array.Empty<UIsettingEntry>();
        private UIsettingEntry[]? activeUIsetting;

#if  pureForm
        public FrameWindow() {
            InitializeComponent();
        }
#else
        /// <summary>
        /// Initialize frame window with all areas in the model, and with one default view page in each area
        /// </summary>
        public FrameWindow() {
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
        /// <param name="windowStateElement">XML element describing the frame window state</param>
        public FrameWindow(FrameWindowState frameWindowState) {
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

            splitContainerMessageViewer.SplitterDistance = frameWindowState.MessagesViewerHeight;
            splitContainerLocomotivesViewer.SplitterDistance = frameWindowState.LocomotiveViewerWidth;
            splitControlViewer.SplitterDistance = frameWindowState.ControlViewerWidth;
            splitContainerTripsMonitor.SplitterDistance = frameWindowState.TripsViewerHeight;

            messageViewVisible = frameWindowState.MessagesViewerVisible;
            locomotiveViewVisible = frameWindowState.LocomotiveViewerVisble;
            layoutControlVisible = frameWindowState.ControlViewerVisible;

            UpdateMessageVisible();
            UpdateLayoutControlVisible();
            UpdateLocomotivesVisible();

            tripsMonitorHeightInOperationMode = frameWindowState.TripsViewerHeight;
        }

        #region Initialization

        private void CommonInitialization() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            Dispatch.AddObjectInstanceDispatcherTargets(this);
            EventManager.AddObjectSubscriptions(this);      // Will become obsolete

            Id = Guid.NewGuid();

            layoutControlViewer.Width = 0;
            tripsMonitor.Height = 0;
            messageViewer.Height = 0;
            locomotivesViewer.Width = 0;

            // Initialize the built-in zoom preset table
            zoomEntries = new ZoomEntry[] {
                                               new ZoomEntry(200, setZoom200toolStripMenuItem),
                                               new ZoomEntry(150, setZoom150toolStripMenuItem),
                                               new ZoomEntry(100, setZoom100toolStripMenuItem),
                                               new ZoomEntry(70, setZoom75toolStripMenuItem),
                                               new ZoomEntry(50, setZoom50toolStripMenuItem),
                                               new ZoomEntry(20, setZoom25toolStripMenuItem),
            };

            operationBaseUIsetting = new UIsettingEntry[] {
                new MenuSettingEntry(areaToolStripMenuItem, UIitemSetting.Hidden),
                new MenuSettingEntry(operationToolStripMenuItem, UIitemSetting.Disabled),
                new MenuSettingEntry(simulationToolStripMenuItem, UIitemSetting.Disabled),
                new MenuSettingEntry(verifyLayoutToolStripMenuItem, UIitemSetting.Hidden),
                new MenuSettingEntry(toolStripSeparatorDesignTimeOnly, UIitemSetting.Hidden),
                new MenuSettingEntry(learnLayoutToolStripMenuItem, UIitemSetting.Hidden),
                new MenuSettingEntry(connectLayoutToolStripMenuItem, UIitemSetting.Hidden),
                new MenuSettingEntry(newComponentsPhaseToolStripMenuItem, UIitemSetting.Hidden),
                new MenuSettingEntry(setComponentsPhaseToToolStripMenuItem, UIitemSetting.Hidden),
                new MenuSettingEntry(newAreaToolStripMenuItem, UIitemSetting.Disabled),
                new MenuSettingEntry(deleteAreaToolStripMenuItem, UIitemSetting.Disabled),
                new ToolbarSettingEntry(toolBarButtonOpenLayout, UIitemSetting.Hidden),
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
                new MenuSettingEntry(designToolStripMenuItem, UIitemSetting.Disabled),
                new MenuSettingEntry(smartDestinationsToolStripMenuItem, UIitemSetting.Hidden),
                new MenuSettingEntry(manualDispatchRegionsToolStripMenuItem, UIitemSetting.Hidden),
                new MenuSettingEntry(showTripsToolStripMenuItem, UIitemSetting.Hidden),
                new MenuSettingEntry(emergencyStopToolStripMenuItem, UIitemSetting.Hidden),
                new MenuSettingEntry(stopLocomotivesToolStripMenuItem, UIitemSetting.Hidden),
                new MenuSettingEntry(emergencyStopToolStripMenuItem, UIitemSetting.Hidden),
                new MenuSettingEntry(onlyActiveLayoutPhasesToolStripMenuItem, UIitemSetting.Hidden),
                new ToolbarSettingEntry(toolBarButtonShowTripsMonitor, UIitemSetting.Hidden),
                new ToolbarSettingEntry(toolBarButtonToggleAllLayoutManualDispatch, UIitemSetting.Hidden),
                new ToolbarSettingEntry(toolBarButtonStopAllLocomotives, UIitemSetting.Hidden),
                new ToolbarSettingEntry(toolBarButtonEmergencyStop, UIitemSetting.Hidden),
                new SeparatorSettingEntry(toolStripInOperateMode1, UIitemSetting.Hidden),
                new SeparatorSettingEntry(toolStripInOperateMode2, UIitemSetting.Hidden),
            };

            messageViewer.Initialize();
            locomotivesViewer.Initialize();
            tripsMonitor.Initialize();
            layoutControlViewer.Initialize();

            DoUIsetting(designModeUIsetting);
            uiMode = null;

            designTool = new LayoutEditingTool();
            operationTool = new LayoutOperationTool();

            locomotivesViewer.LocomotiveCollection = LayoutModel.LocomotiveCollection;

            InitializeTask();
            ForceSetUserInterfaceMode(LayoutController.OptionalOperationModeSettings);
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
        /// <param name="windowStateElement">The XML element in which the state is saved</param>
        public void SaveFrameWindowState(FrameWindowState frameWindowState) {
            frameWindowState.LocomotiveViewerVisble = locomotiveViewVisible;
            frameWindowState.LocomotiveViewerWidth = splitContainerLocomotivesViewer.SplitterDistance;

            frameWindowState.ControlViewerVisible = layoutControlVisible;
            frameWindowState.ControlViewerWidth = splitControlViewer.SplitterDistance;

            frameWindowState.MessagesViewerVisible = messageViewVisible;
            frameWindowState.MessagesViewerHeight = splitContainerMessageViewer.SplitterDistance;

            frameWindowState.TripsViewerHeight = splitContainerTripsMonitor.SplitterDistance;
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
        public Task<FrameWindowAction> Task => Ensure.NotNull<Task<FrameWindowAction>>(tcs?.Task);

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

        private LayoutFrameWindowAreaTabPage ActiveAreaPage => (LayoutFrameWindowAreaTabPage)tabAreas.SelectedTab;

        private LayoutFrameWindowAreaViewTabPage? ActiveViewPage => ActiveAreaPage == null ? null : (LayoutFrameWindowAreaViewTabPage)ActiveAreaPage.TabViews.SelectedTab;

        private LayoutView? ActiveView => ActiveViewPage?.View;

        #endregion

        #region Setting UI for various mode (design/operational/simulation)

        private void ForceSetUserInterfaceMode(OperationModeParameters? settings) {
            UndoUIsetting();

            if (settings == null) {
                DoUIsetting(designModeUIsetting);
                activeTool = designTool;

                toolBarButtonDesignMode.Checked = true;
                toolBarButtonOperationMode.Checked = false;
                toolBarButtonSimulation.Checked = false;
                //SUSPENDED
                //statusBarPanelMode.Text = "Design";

                tripsMonitorHeightInOperationMode = tripsMonitor.Height;
                Dispatch.Call.HideTripsMonitor(this.Id);
                splitContainerTripsMonitor.Panel1Collapsed = true;

                ChangeAllViewsPhases(LayoutPhase.All);      // In editing show all phases
            }
            else {
                toolBarButtonDesignMode.Checked = false;

                if (settings.Simulation) {
                    DoUIsetting(simulationModeUIsetting);
                    toolBarButtonOperationMode.Checked = false;
                    toolBarButtonSimulation.Checked = true;

                    string s = "Simulation";

                    switch (settings.Phases) {
                        case LayoutPhase.Operational: s += " on operational region"; break;
                        case LayoutPhase.NotPlanned: s += " on operational+in construction regions"; break;
                    }

                    //SUSPENDED
                    //statusBarPanelMode.Text = s;

                }
                else {
                    DoUIsetting(operationModeUIsetting);
                    toolBarButtonOperationMode.Checked = true;
                    toolBarButtonSimulation.Checked = false;
                    //SUSPENDED
                    //statusBarPanelMode.Text = "Operation";
                }

                if (tripsMonitorHeightInOperationMode > 0) {
                    tripsMonitor.Height = tripsMonitorHeightInOperationMode;
                    UpdateTripsMonitorVisible();
                }

                splitContainerTripsMonitor.Panel1Collapsed = false;

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

        private void SetUserInterfaceMode(OperationModeParameters? settings) {
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
        public LayoutFrameWindowAreaViewTabPage AddViewToArea(LayoutFrameWindowAreaTabPage areaTabPage, string name, LayoutView view) {
            view.Area = areaTabPage.Area;

            // Wire events in order to reflect them to the current tool
            view.MouseDown += this.LayoutView_MouseDown;
            view.MouseMove += this.LayoutView_MouseMove;
            view.ModelComponentClick += this.LayoutView_ModelComponentClick;
            view.ModelComponentDragged += this.LayoutView_ModelComponentDragged;
            view.ModelComponentQueryDrop += LayoutView_ModelComponentQueryDrop;
            view.ModelComponentDrop += LayoutView_ModelComponentDrop;

            LayoutFrameWindowAreaViewTabPage viewTabPage = new(view) {
                Text = name
            };
            areaTabPage.TabViews.TabPages.Add(viewTabPage);

            return viewTabPage;
        }

        public LayoutFrameWindowAreaViewTabPage AddViewToArea(LayoutFrameWindowAreaTabPage areaPage, string name) => AddViewToArea(areaPage, name, new LayoutView());

        private void ChangeAllViewsPhases(LayoutPhase phases) {
            foreach (LayoutFrameWindowAreaTabPage atp in tabAreas.TabPages) {
                foreach (LayoutFrameWindowAreaViewTabPage vtp in atp.TabViews.TabPages) {
                    if (vtp.View.Phases != phases) {
                        vtp.View.Phases = phases;
                        vtp.View.Invalidate();
                    }
                }
            }
        }

        private void AdjustAllViewsPhases() {
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
        private void UpdateFormTitle() {
            string layoutName;
            string modified = "";

            layoutName = LayoutController.LayoutFilename;

            if (LayoutController.CommandManager.ChangeLevel != 0)
                modified = "*";

            this.Text = Path.GetFileName(layoutName) + modified + " - VillaRakavy Layout Manager";
        }

        [DispatchTarget]
        private void CloseAllFrameWindows() {
            Close();
        }

        [LayoutEvent("model-modified")]
        [LayoutEvent("model-not-modified")]
        private void ModelModificationStateChanged(LayoutEvent e) {
            UpdateFormTitle();
        }

        internal void OnViewChanged() {
            if (ActiveView != null)
                toolBarButtonToggleGrid.Checked = ActiveView.ShowGrid != ShowGridLinesOption.Hide;
        }

        [LayoutEvent("test")]
        private void Test(LayoutEvent e) {
            Trace.WriteLine("Got event test");
            Trace.WriteLine("Sender is " + (e.Sender == null ? "Null" : e.Sender.ToString()));
            Trace.WriteLine("Info is " + (e.Info == null ? "Null" : e.Info.ToString()));

            var optionsElement = e.Element["Options"];

            if (optionsElement != null) {
                Trace.WriteLine("Options:");

                foreach (XmlAttribute a in optionsElement.Attributes)
                    Trace.WriteLine("  Option " + a.Name + " is " + a.Value);
            }
        }

        [DispatchTarget]
        private void ToolsMenuOpenRequest(MenuOrMenuItem toolsMenu) {
            var layoutEmulationServices = Dispatch.Call.GetLayoutEmulationServices();

            if (layoutEmulationServices != null) {
                toolsMenu.Items.Add(new LayoutMenuItem("Reset layout emulation", null, new EventHandler(OnResetLayoutEmulation)));
            }
        }

        [DispatchTarget]
        private void ShowRoutingTableGenerationDialog(int nTurnouts) {
            using Dialogs.RoutingTableCalcProgress d = new(nTurnouts);

            d.ShowDialog(this);
        }

        private void OnResetLayoutEmulation(object? sender, EventArgs e) {
            Dispatch.Call.ResetLayoutEmulation();
        }

        [LayoutEvent("enter-design-mode")]
        private void EnterDesignMode(LayoutEvent e) {
            SetUserInterfaceMode(null);
        }

        [DispatchTarget]
        private void OnPrepareEnterOperationMode(OperationModeParameters settings) {
            SetUserInterfaceMode(settings);
        }

        [LayoutEvent("begin-trains-analysis-phase")]
        private void BeginTrainsAnalysisPhase(LayoutEvent e) {
            //SUSPENDED
            //statusBarPanelOperation.Text = "Analyzing trains status...";
        }

        [LayoutEvent("end-trains-analysis-phase")]
        private void EndTrainsAnalysisPhase(LayoutEvent e) {
            //SUSPENDED
            //statusBarPanelOperation.Text = "Ready";
        }

        #region Menu Editing functions

        /// <summary>
        /// Apply the setting (do) to menu items based on instructions in a provided table
        /// </summary>
        /// <param name="menuSetting">Instructions for the menu item setting</param>
        private void DoUIsetting(UIsettingEntry[] uiSetting) {
            foreach (UIsettingEntry aEntry in uiSetting) {
                if (aEntry is DoSettingEntry doSettingEntry)
                    DoUIsetting(doSettingEntry.DoThis);
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
                else if (aEntry is SeparatorSettingEntry separatorSettingEntry) {
                    if (separatorSettingEntry.Setting == UIitemSetting.Hidden)
                        separatorSettingEntry.Separator.Visible = false;
                }
            }

            activeUIsetting = uiSetting;
        }

        /// <summary>
        /// Undo the setting to menu items. The setting are described in the passed table
        /// </summary>
        /// <param name="menuSetting"0>Instructions for the menu item setting</param>
        private void UndoUIsetting(UIsettingEntry[] uiSetting) {
            foreach (UIsettingEntry aEntry in uiSetting) {
                if (aEntry is DoSettingEntry doSettingEntry)
                    UndoUIsetting(doSettingEntry.DoThis);
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

        private void UndoUIsetting() {
            if (activeUIsetting != null) {
                UndoUIsetting(activeUIsetting);
                activeUIsetting = null;
            }
        }

        #endregion

        #region Layout Event handlers

        [DispatchTarget]
        private void ShowControlConnectionPoint([DispatchFilter(Type = "IsMyId")] Guid frameWindowId, ControlConnectionPointReference connectionPointRef) {
            Dispatch.Call.ShowLayoutControl(frameWindowId);
            layoutControlViewer.EnsureVisible(connectionPointRef, true);
        }

        [DispatchTarget]
        private void ShowControlModule([DispatchFilter(Type = "IsMyId")] Guid frameWindowId, ControlModuleReference moduleRef) {
            Dispatch.Call.ShowLayoutControl(frameWindowId);
            layoutControlViewer.EnsureVisible(moduleRef, true);
        }

        [DispatchTarget]
        private void DeselectControlObjects([DispatchFilter(Type = "IsMyId")] Guid frameWindowId) {
            layoutControlViewer.DeselectAll();
        }

        [DispatchTarget]
        private void EnsureComponentVisible([DispatchFilter(Type = "IsMyId")] Guid frameWindowId, ModelComponent component, bool markComponent) {
            LayoutModelArea area = component.Spot.Area;
            LayoutFrameWindowAreaTabPage? areaPage = null;

            foreach (LayoutFrameWindowAreaTabPage ap in tabAreas.TabPages)
                if (ap.Area == area) {
                    areaPage = ap;
                    break;
                }

            if (areaPage != null) {
                LayoutFrameWindowAreaViewTabPage? bestViewPage = null;
                LayoutView.VisibleResult bestVisibleResult = LayoutView.VisibleResult.No;

                // Figure out what view in this area shows the component best

                foreach (LayoutFrameWindowAreaViewTabPage vp in areaPage.TabViews.TabPages) {
                    LayoutView.VisibleResult visibleResult = vp.View.IsComponentVisible(component);

                    if ((int)visibleResult > (int)bestVisibleResult) {
                        bestVisibleResult = visibleResult;
                        bestViewPage = vp;
                    }
                    else if (visibleResult != LayoutView.VisibleResult.No &&
                        visibleResult == bestVisibleResult && bestViewPage != null && vp.View.Zoom > bestViewPage.View.Zoom) {
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
                    Dispatch.Call.ShowMarker(frameWindowId, component);
            }
        }

        [DispatchTarget]
        private void OnAllLayoutManualDispatchModeStatusChanged(bool active) {
            toolBarButtonToggleAllLayoutManualDispatch.Checked = active;
        }

        [LayoutEvent("manual-dispatch-region-status-changed")]
        private void ManualDispatchRegionStatusChanged(LayoutEvent e) {
            bool enableAllLayoutManualDispatch = true;

            foreach (ManualDispatchRegionInfo manualDispatchRegion in LayoutModel.StateManager.ManualDispatchRegions)
                if (manualDispatchRegion.Active)
                    enableAllLayoutManualDispatch = false;

            toolBarButtonToggleAllLayoutManualDispatch.Checked = enableAllLayoutManualDispatch;
        }

        [DispatchTarget]
        private void OnAllTripsSuspendedStatusChanged(bool allSuspended) {
            toolBarButtonStopAllLocomotives.Checked = allSuspended;
        }

        [DispatchTarget]
        private void ShowMarker([DispatchFilter(Type = "IsMyId")] Guid frameWindowId, object markedObject) {
            var markerSelection = markedObject switch {
                ModelComponent component => new LayoutSelection {
                    component
                },
                LayoutSelection selection => selection,
                _ => throw new ArgumentException("Invalid marker (can be either component or selection"),
            };

            _ = new Marker(markerSelection);
        }

        #region Marker class

        private class MarkerLook : ILayoutSelectionLook {
            public Color Color => Color.IndianRed;
        }

        private class Marker {
            private Timer? hideMarkerTimer;
            private readonly LayoutSelection markerSelection;

            public Marker(LayoutSelection markerSelection) {
                this.markerSelection = markerSelection;

                markerSelection.Display(new MarkerLook());
                hideMarkerTimer = new Timer {
                    Interval = 500
                };
                hideMarkerTimer.Tick += this.OnHideMarker;
                hideMarkerTimer.Start();
            }

            private void OnHideMarker(object? sender, EventArgs e) {
                hideMarkerTimer?.Dispose();
                hideMarkerTimer = null;
                markerSelection.Hide();
            }
        }

        #endregion

        private void UpdateLayoutControlVisible() {
            splitControlViewer.Panel1Collapsed = !layoutControlVisible;
            if (layoutControlVisible)
                Dispatch.Notification.OnLayoutControlShown(Id);
            else
                Dispatch.Notification.OnLayoutControlHidden(Id);
        }

        private void UpdateTripsMonitorVisible() {
            bool previousState = tripsMonitorVisible;

            tripsMonitorVisible = tripsMonitor.Height > 10;
            if (tripsMonitorVisible != previousState) {
                if (tripsMonitorVisible)
                    Dispatch.Notification.OnTripsMonitorShown(Id);
                else
                    Dispatch.Notification.OnTripsMonitorHidden(Id);
            }
        }

        private void UpdateMessageVisible() {
            splitContainerMessageViewer.Panel2Collapsed = !messageViewVisible;

            if (messageViewVisible)
                Dispatch.Notification.OnMessagesShown(Id);
            else
                Dispatch.Notification.OnMessagesHidden(Id);
        }

        private void UpdateLocomotivesVisible() {
            splitContainerLocomotivesViewer.Panel2Collapsed = !locomotiveViewVisible;
            if (locomotiveViewVisible)
                Dispatch.Notification.OnLocomotivesShown(Id);
            else
                Dispatch.Notification.OnLocomotivesHidden(Id);
        }

        [DispatchTarget]
        private void ShowMessages(Guid frameWindowId) {
            if (frameWindowId == Id || frameWindowId == Guid.Empty) {
                messageViewVisible = true;
                UpdateMessageVisible();
            }
        }

        [DispatchTarget]
        private void HideMessages(Guid frameWindowId) {
            if (frameWindowId == Id || frameWindowId == Guid.Empty) {
                messageViewVisible = false;
                UpdateMessageVisible();
            }
        }

        [DispatchTarget]
        private void ShowLocomotives(Guid frameWindowId) {
            if (frameWindowId == Id || frameWindowId == Guid.Empty) {
                locomotiveViewVisible = true;
                UpdateLocomotivesVisible();
            }
        }

        [DispatchTarget]
        private void HideLocomotives(Guid frameWindowId) {
            if (frameWindowId == Id || frameWindowId == Guid.Empty) {
                locomotiveViewVisible = false;
                UpdateLocomotivesVisible();
            }
        }

        [DispatchTarget]
        private void ShowLayoutControl([DispatchFilter(Type = "IsMyId")] Guid frameWindowId) {
            layoutControlVisible = true;
            UpdateLayoutControlVisible();
        }

        [DispatchTarget]
        private void HideLayoutControl([DispatchFilter(Type = "IsMyId")] Guid frameWindowId) {
            layoutControlVisible = false;
            UpdateLayoutControlVisible();
        }

        [DispatchTarget]
        private void ShowTripsMonitor([DispatchFilter(Type = "IsMyId")] Guid frameWindowId) {
            tripsMonitor.Height = ClientSize.Height * 12 / 100;
            UpdateTripsMonitorVisible();
        }

        [DispatchTarget]
        private void HideTripsMonitor([DispatchFilter(Type = "IsMyId")] Guid frameWindowId) {
            tripsMonitor.Height = 0;
            UpdateTripsMonitorVisible();
        }

        [DispatchTarget]
        private void OnMessagesShown([DispatchFilter(Type = "IsMyId")] Guid frameWindowId) {
            toolBarButtonShowMessages.Checked = true;
        }

        [DispatchTarget]
        private void OnMessagesHidden([DispatchFilter(Type = "IsMyId")] Guid frameWindowId) {
            toolBarButtonShowMessages.Checked = false;
        }

        [DispatchTarget]
        private void OnTripsMonitorShown([DispatchFilter(Type = "IsMyId")] Guid frameWindowId) {
            toolBarButtonShowTripsMonitor.Checked = true;
        }

        [DispatchTarget]
        private void OnTripsMonitorHidden([DispatchFilter(Type = "IsMyId")] Guid frameWindowId) {
            toolBarButtonShowTripsMonitor.Checked = false;
        }

        [DispatchTarget]
        private void OnLocomotivesShown([DispatchFilter(Type = "IsMyId")] Guid frameWindowId) {
            toolBarButtonShowLocomotives.Checked = true;
        }

        [DispatchTarget]
        private void OnLocomotivesHidden([DispatchFilter(Type = "IsMyId")] Guid frameWindowId) {
            toolBarButtonShowLocomotives.Checked = false;
        }

        [DispatchTarget]
        private void OnLayoutControlShown([DispatchFilter(Type = "IsMyId")] Guid frameWindowId) {
            toolBarButtonShowLayoutControl.Checked = true;
        }

        [DispatchTarget]
        private void OnLayoutControlHidden([DispatchFilter(Type = "IsMyId")] Guid frameWindowId) {
            toolBarButtonShowLayoutControl.Checked = false;
        }

        #endregion

        [LayoutEvent("area-added")]
        private void AreaAdded(LayoutEvent e) {
            var area = Ensure.NotNull<LayoutModelArea>(e.Sender);
            var areaPage = new LayoutFrameWindowAreaTabPage(area);

            tabAreas.TabPages.Add(areaPage);
            AddViewToArea(areaPage, "Overview");
        }

        [LayoutEvent("area-removed")]
        private void AreaRemoved(LayoutEvent e) {
            var area = Ensure.NotNull<LayoutModelArea>(e.Sender);

            foreach (LayoutFrameWindowAreaTabPage areaTabPage in tabAreas.TabPages) {
                if (areaTabPage.Area == area) {
                    tabAreas.TabPages.Remove(areaTabPage);
                    break;
                }
            }
        }

        [LayoutEvent("area-renamed")]
        private void AreaRenamed(LayoutEvent e0) {
            var e = (LayoutEvent<LayoutModelArea>)e0;
            var area = e.Sender;

            foreach (LayoutFrameWindowAreaTabPage areaTabPage in tabAreas.TabPages) {
                if (areaTabPage.Area == area) {
                    areaTabPage.Text = area.Name;
                    break;
                }
            }
        }

        private void LayoutView_ModelComponentClick(object? sender, LayoutViewEventArgs e) {
            if (activeTool != null)
                activeTool.LayoutView_ModelComponentClick(sender, e);
        }

        private void LayoutView_ModelComponentDragged(object? sender, LayoutViewEventArgs e) {
            if (activeTool != null)
                activeTool.LayoutView_ModelComponentDragged(sender, e);
        }

        private void LayoutView_ModelComponentQueryDrop(object? sender, LayoutViewEventArgs e) {
            if (activeTool != null)
                activeTool.LayoutView_ModelComponentQueryDrop(sender, e);
        }

        private void LayoutView_ModelComponentDrop(object? sender, LayoutViewEventArgs e) {
            if (activeTool != null)
                activeTool.LayoutView_ModelComponentDrop(sender, e);
        }

        private void LayoutView_MouseDown(object? sender, MouseEventArgs e) {
            timerDetailsPopup.Stop();

            if (detailsPopupWindow != null) {
                detailsPopupWindow.Close();
                detailsPopupWindow = null;
                if (viewWithMouse != null)
                    viewWithMouse.Capture = false;
            }
        }

        private void LayoutView_MouseMove(object? sender, MouseEventArgs e) {
            // Reset timer
            if (new Point(e.X, e.Y) != viewMouseLocation)
                timerDetailsPopup.Stop();

            var mouseView = sender as LayoutView;

            if (mouseView != viewWithMouse || new Point(e.X, e.Y) != viewMouseLocation) {
                bool closeIt = true;

                if (detailsPopupWindow != null) {
                    if (viewWithMouse != null) {
                        LayoutHitTestResult hit = viewWithMouse.HitTest(new Point(e.X, e.Y));

                        // Retain details pop up window as long as mouse is in the same grid location
                        if (hit.ModelLocation == detailsPopupLocationInModelPoints)
                            closeIt = false;
                    }

                    if (closeIt) {
                        detailsPopupWindow.Close();
                        detailsPopupWindow = null;
                        if (viewWithMouse != null)
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

        private void TimerDetailsPopup_Tick(object? sender, EventArgs e) {
            // Mouse did not move for enough time, need to show details pop up window
            if (detailsPopupWindow == null) {
                if (viewWithMouse != null) {
                    LayoutHitTestResult hitTestResult = viewWithMouse.HitTest(viewMouseLocation);
                    var spot = hitTestResult.Area[hitTestResult.ModelLocation, LayoutModel.ActivePhases];
                    Rectangle bounds = new(new Point(viewMouseLocation.X - (SystemInformation.DragSize.Width / 2), viewMouseLocation.Y - (SystemInformation.DragSize.Height / 2)),
                        SystemInformation.DragSize);

                    if (spot != null && bounds.Contains(viewWithMouse.PointToClient(Control.MousePosition))) {
                        PopupWindowContainerSection container = new(hitTestResult.View);

                        foreach (ModelComponent component in spot)
                            Dispatch.Call.AddDetailsWindowSections(component, container);

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

        private void MenuItemSave_Click(object? sender, EventArgs e) {
            EventManager.Event(new LayoutEvent("save-layout", this));
        }

        private void MenuSaveAs_Click(object? sender, EventArgs e) {
        }

        private void MenuItemOpen_Click(object? sender, EventArgs e) {
        }

        private void MenuItemNewLayout_Click(object? sender, EventArgs e) {
        }

        private class ButtonToMenuMap {
            public ToolStripButton theButton;
            public EventHandler theEvent;

            public ButtonToMenuMap(ToolStripButton button, EventHandler eh) {
                theButton = button;
                theEvent = eh;
            }
        }

        private void ToolBarMain_ButtonClick(object? sender, EventHandler e) {
            ButtonToMenuMap[] map = Array.Empty<ButtonToMenuMap>();

#if SUSPENDED
            if (e.Button.Visible && e.Button.Enabled) {
                if (e.Button == toolBarButtonSimulation) {
                    if (LayoutModel.Components<IModelComponentIsCommandStation>(LayoutPhase.All).Any(cs => cs.LayoutEmulationSupported)) {
                        var entries = GetPhasesEntries(simulate);

                        if (entries.Count == 1)
                            entries[0].Item2(this, EventArgs.Empty);
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
#endif
        }

        private void Toolbar_Zoom100(object? sender, EventArgs e) {
            setZoom100toolStripMenuItem.PerformClick();
        }

        private void Toolbar_ZoomOut(object? sender, EventArgs e) {
            if (ActiveView != null) {
                float zoom = ActiveView.Zoom - 0.1F;

                if (zoom < 0.1)
                    zoom = 0.1F;
                ActiveView.Zoom = zoom;
            }
        }

        private void Toolbar_ZoomIn(object? sender, EventArgs e) {
            if (ActiveView != null) {
                float zoom = ActiveView.Zoom + 0.1F;

                if (zoom > 4)
                    zoom = 4F;
                ActiveView.Zoom = zoom;
            }
        }

        private void MenuItemExit_Click(object? sender, EventArgs e) {
            ControllerCommand = FrameWindowCommand.CloseLayout;
        }

        private void MenuItemUndo_Click(object? sender, EventArgs e) {
            LayoutController.CommandManager.Undo();
        }

        private void MenuItemRedo_Click(object? sender, EventArgs e) {
            LayoutController.CommandManager.Redo();
        }

        private void EditToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            if (!LayoutController.IsOperationMode && LayoutController.CommandManager.CanUndo) {
                undoToolStripMenuItem.Enabled = true;
                undoToolStripMenuItem.Text = $"&Undo {LayoutController.CommandManager.UndoCommandName}";
            }
            else {
                undoToolStripMenuItem.Enabled = false;
                undoToolStripMenuItem.Text = "&Undo";
            }

            if (!LayoutController.IsOperationMode && LayoutController.CommandManager.CanRedo) {
                redoToolStripMenuItem.Enabled = true;
                redoToolStripMenuItem.Text = $"&Redo {LayoutController.CommandManager.RedoCommandName}";
            }
            else {
                redoToolStripMenuItem.Enabled = false;
                redoToolStripMenuItem.Text = "&Redo";
            }

            if (LayoutController.IsOperationMode || LayoutController.UserSelection.Count == 0) {
                copyToolStripMenuItem.Enabled = false;
                cutToolStripMenuItem.Enabled = false;
            }
            else {
                copyToolStripMenuItem.Enabled = true;
                cutToolStripMenuItem.Enabled = true;
            }
        }

        private void MenuItemNewArea_Click(object? sender, EventArgs e) {
            var getAreaName = new Dialogs.GetAreasName();

            if (getAreaName.ShowDialog(this) == DialogResult.OK) {
                LayoutController.AddArea(getAreaName.AreaName);
            }
        }

        private void MenuItemRenameArea_Click(object? sender, EventArgs e) {
            var getAreaName = new Dialogs.GetAreasName();

            if (getAreaName.ShowDialog(this) == DialogResult.OK)
                ((LayoutFrameWindowAreaTabPage)tabAreas.SelectedTab).Area.Name = getAreaName.AreaName;
        }

        private void MenuItemStyleFonts_Click(object? sender, EventArgs e) {
            var standardFonts = new Dialogs.StandardFonts();

            standardFonts.ShowDialog(this);
            LayoutModel.WriteModelXmlInfo();
            LayoutModel.Instance.Redraw();
        }

        private void MenuItemStylePositions_Click(object? sender, EventArgs e) {
            var standardPositions = new Dialogs.StandardPositions();

            standardPositions.ShowDialog();
            LayoutModel.WriteModelXmlInfo();
            LayoutModel.Instance.Redraw();
        }

        private void MenuItemCopy_Click(object? sender, EventArgs e) {
            var copySelection = new MenuItemCopySelection(LayoutController.UserSelection.TopLeftLocation);

            copySelection.Copy();
        }

        private void MenuItemCut_Click(object? sender, EventArgs e) {
            var cutSelection = new MenuItemCutSelection(LayoutController.UserSelection.TopLeftLocation);

            cutSelection.Cut();
        }

        private void MenuItemModules_Click(object? sender, EventArgs e) {
            var moduleManager = new Dialogs.ModuleManagement();

            moduleManager.ShowDialog(this);
        }

        private void ToolsToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            toolsToolStripMenuItem.DropDownItems.Clear();

            Dispatch.Call.ToolsMenuOpenRequest(new MenuOrMenuItem(toolsToolStripMenuItem));

            if (toolsToolStripMenuItem.DropDownItems.Count == 0) {
                var noTools = new ToolStripMenuItem("No tools") {
                    Enabled = false
                };
                toolsToolStripMenuItem.DropDownItems.Add(noTools);
            }
        }

        private void MenuItemAreaArrange_Click(object? sender, EventArgs e) {
            var arrangeAreas = new Dialogs.ArrangeAreas(tabAreas);

            arrangeAreas.ShowDialog(this);
        }

        private void MenuItemDeleteArea_Click(object? sender, EventArgs e) {
            LayoutFrameWindowAreaTabPage selectedAreaPage = (LayoutFrameWindowAreaTabPage)tabAreas.SelectedTab;
            LayoutModelArea selectedArea = selectedAreaPage.Area;

            // Check if area is empty
            if (selectedArea.Grid.Count > 0)
                MessageBox.Show(this, "This area is not empty, first remove all components, then delete the area", "Cannot delete area", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                LayoutModel.Areas.Remove(selectedArea);
        }

        private void ViewToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            if (ActiveView != null)
                gridToolStripMenuItem.Checked = ActiveView.ShowGrid != ShowGridLinesOption.Hide;

            showMessagesToolStripMenuItem.Text = messageViewVisible ? "Hide &Messages" : "Show &Messages";
            showLocomotivesToolStripMenuItem.Text = locomotiveViewVisible ? "Hide &Locomotives" : "Show &Locomotives";
            showTripsToolStripMenuItem.Text = tripsMonitorVisible ? "Hide &Trips" : "Show &Trips";
            showLayoutControlToolStripMenuItem.Text = layoutControlVisible ? "Hide Layout &Control" : "Show Layout &Control";

            if (LayoutController.IsOperationMode) {
                if (LayoutModel.ModelPhases != LayoutModel.ActivePhases) {
                    onlyActiveLayoutPhasesToolStripMenuItem.Visible = true;
                    if (ActiveViewPage != null)
                        onlyActiveLayoutPhasesToolStripMenuItem.Checked = ActiveViewPage.ShowActivePhases;
                }
                else
                    onlyActiveLayoutPhasesToolStripMenuItem.Visible = false;
            }
        }

        private void MenuItemViewGrid_Click(object? sender, EventArgs e) {
            if (ActiveView != null) {
                if (ActiveView.ShowGrid != ShowGridLinesOption.Hide) {
                    ActiveView.ShowGrid = ShowGridLinesOption.Hide;
                    toolBarButtonToggleGrid.Checked = false;
                }
                else {
                    ActiveView.ShowGrid = ShowGridLinesOption.AutoHide;
                    toolBarButtonToggleGrid.Checked = true;
                }
            }
        }

        public LayoutView? AddNewView() {
            var getViewName = new Dialogs.GetViewName();

            if (getViewName.ShowDialog() == DialogResult.OK) {
                var oldView = ActiveView;
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

        private void MenuItemViewNew_Click(object? sender, EventArgs e) {
            AddNewView();
        }

        private void MenuItemViewDelete_Click(object? sender, EventArgs e) {
            if (ActiveViewPage != null)
                ActiveAreaPage.TabViews.TabPages.Remove(ActiveViewPage);
        }

        private void MenuItemViewRename_Click(object? sender, EventArgs e) {
            if (ActiveViewPage != null) {
                var getViewName = new Dialogs.GetViewName();

                if (getViewName.ShowDialog() == DialogResult.OK)
                    ActiveViewPage.Text = getViewName.ViewName;
            }
        }

        private void MenuItemViewArrage_Click(object? sender, EventArgs e) {
            if (ActiveAreaPage != null) {
                var arrangeViews = new Dialogs.ArrangeViews(this, ActiveAreaPage);

                arrangeViews.ShowDialog();
            }
        }

        private void ZoomToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            if (ActiveView != null) {
                int currentZoom = (int)Math.Round(ActiveView.Zoom * 100);
                bool presetZoomFound = false;

                foreach (ZoomEntry zoomEntry in zoomEntries) {
                    zoomEntry.menuItem.Checked = zoomEntry.zoomFactor == currentZoom;
                    if (zoomEntry.menuItem.Checked)
                        presetZoomFound = true;
                }

                setZoomToolStripMenuItem.Text = "Set zoom (" + currentZoom + "%)...";
                setZoomToolStripMenuItem.Checked = !presetZoomFound;
            }
        }

        private void MenuItemZoomPreset_Click(object? sender, EventArgs e) {
            var menuItem = (ToolStripMenuItem?)sender;

            if (ActiveView != null) {
                foreach (ZoomEntry zoomEntry in zoomEntries) {
                    if (zoomEntry.menuItem == menuItem) {
                        ActiveView.Zoom = (float)zoomEntry.zoomFactor / 100;
                        break;
                    }
                }
            }
        }

        private void LayoutController_Deactivate(object? sender, EventArgs e) {
            timerFreeResources.Enabled = true;
        }

        private void LayoutController_Resize(object? sender, EventArgs e) {
            timerFreeResources.Enabled = WindowState == FormWindowState.Minimized;
        }

        private void MenuItemSetZoom_Click(object? sender, EventArgs e) {
            if (ActiveView != null) {
                var setZoom = new Dialogs.SetZoom {
                    ZoomFactor = (int)Math.Round(ActiveView.Zoom * 100)
                };

                if (setZoom.ShowDialog(this) == DialogResult.OK)
                    ActiveView.Zoom = (float)setZoom.ZoomFactor / 100;
            }
        }

        private void MenuItemZoomAllArea_Click(object? sender, EventArgs e) {
            if (ActiveView != null)
                ActiveView.ShowAllArea();
        }

        private void MenuItemShowMessages_Click(object? sender, EventArgs e) {
            if (messageViewVisible)
                Dispatch.Call.HideMessages(Id);
            else
                Dispatch.Call.ShowMessages(Id);
        }

        private void MenuItemShowTripsMonitor_Click(object? sender, EventArgs e) {
            if (tripsMonitorVisible)
                Dispatch.Call.HideTripsMonitor(Id);
            else
                Dispatch.Call.ShowTripsMonitor(Id);
        }

        private void MenuItemShowLocomotives_Click(object? sender, EventArgs e) {
            if (locomotiveViewVisible)
                Dispatch.Call.HideLocomotives(Id);
            else
                Dispatch.Call.ShowLocomotives(Id);
        }

        private void MenuItemShowLayoutControl_Click(object? sender, EventArgs e) {
            if (layoutControlVisible)
                Dispatch.Call.HideLayoutControl(Id);
            else
                Dispatch.Call.ShowLayoutControl(Id);
        }

        private void MenuItemOperational_Click(object? sender, EventArgs e) {
            LayoutController.Instance.EnterOperationModeRequest(new OperationModeParameters() { Phases = LayoutPhase.Operational, Simulation = false });
        }

        private void MenuItemDesign_Click(object? sender, EventArgs e) {
            LayoutController.Instance.EnterDesignModeRequest();
        }

        private void MenuItemLocomotiveCatalog_Click(object? sender, EventArgs e) {
            var d = new Dialogs.LocomotiveCatalog();
            d.ShowDialog(this);
        }

        private void SplitterMessages_SplitterMoved(object? sender, SplitterEventArgs e) {
            UpdateMessageVisible();
        }

        private void SplitterTripsMonitor_SplitterMoved(object? sender, SplitterEventArgs e) {
            UpdateTripsMonitorVisible();
        }

        private void SplitterLocomotives_SplitterMoved(object? sender, SplitterEventArgs e) {
            UpdateLocomotivesVisible();
        }

        private void TimerFreeResources_Elapsed(object? sender, System.Timers.ElapsedEventArgs e) {
            // 30 seconds (or so) were passed while the application was minimized or deactivated
            // recoverable resources can be disposed.
            Debug.WriteLine("--- LayoutManager application not active - free resources ---");
            Dispatch.Call.FreeResources();
            timerFreeResources.Enabled = false;
        }

        private void LayoutController_Activated(object? sender, EventArgs e) {
            timerFreeResources.Enabled = false;
        }

        private void OnManageManualDispatchRegions(object? sender, EventArgs e) {
            var d = new Tools.Dialogs.ManualDispatchRegions();

            d.Show();
        }

        private void MenuItemPolicies_Click(object? sender, EventArgs e) {
            var d = (Form?)EventManager.Event(new LayoutEvent("query-policies-definition-dialog", this));

            if (d != null)
                d.Activate();
            else {
                var p = new Dialogs.PoliciesDefinition {
                    Owner = this
                };
                p.Show();
            }
        }

        private void MenuItemAccelerationProfiles_Click(object? sender, EventArgs e) {
            var rampsDialog = new Tools.Dialogs.MotionRamps(LayoutModel.Instance.Ramps);

            rampsDialog.ShowDialog();
            LayoutModel.WriteModelXmlInfo();
        }

        private void MenuItemDefaultDriverParameters_Click(object? sender, EventArgs e) {
            var d = new Tools.Dialogs.DefaultDrivingParameters();

            d.ShowDialog(this);
        }

        private void MenuItemCommonDestinations_Click(object? sender, EventArgs e) {
            var d = new Tools.Dialogs.TripPlanCommonDestinations {
                Owner = this
            };
            d.Show();
        }

        private class ManualDispatchRegionItem : LayoutMenuItem {
            private readonly ManualDispatchRegionInfo manualDispatchRegion;

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
                    manualDispatchRegion.Active = !manualDispatchRegion.Active;

                    EventManager.Event(new LayoutEvent("manual-dispatch-region-status-changed", manualDispatchRegion, manualDispatchRegion.Active));
                }
                catch (LayoutException ex) {
                    MessageBox.Show("Could change manual dispatch mode because: " + ex.Message, "Unable to change Manual Dispatch Mode",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private class AllLayoutManualDispatchItem : LayoutMenuItem {
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

        private void ToggleAllLayoutManualDispatch(object? sender, EventArgs e) {
            var item = new AllLayoutManualDispatchItem();
            item.PerformClick();
        }

        private void MenuItemSuspendLocomotives_Click(object? sender, EventArgs e) {
            var allSuspended = Dispatch.Call.AreAllTripsSuspended();

            if (allSuspended)
                Dispatch.Call.ResumeAllTrips();
            else
                Dispatch.Call.SuspendAllTrips();
        }

        private List<Tuple<string, EventHandler>> GetPhasesEntries(Action<LayoutPhase> doThis) {
            var entries = new List<Tuple<string, EventHandler>>();

            if (LayoutModel.HasPhase(LayoutPhase.Operational))
                entries.Add(new Tuple<string, EventHandler>("Operational region", (s, ea) => doThis(LayoutPhase.Operational)));
            if (LayoutModel.HasPhase(LayoutPhase.Construction))
                entries.Add(new Tuple<string, EventHandler>("In construction + Operational regions", (s, ea) => doThis(LayoutPhase.NotPlanned)));
            if (LayoutModel.HasPhase(LayoutPhase.Planned))
                entries.Add(new Tuple<string, EventHandler>("All diagram", (s, ea) => doThis(LayoutPhase.All)));

            return entries;
        }

        private EventHandler? simulateEventHandler;
        private EventHandler? verifyLayoutEventHandler;

        private void LayoutToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            bool allSuspended = Dispatch.Call.AreAllTripsSuspended();

            if (allSuspended)
                stopLocomotivesToolStripMenuItem.Text = "&Resume train operation";
            else
                stopLocomotivesToolStripMenuItem.Text = "&Stop trains";

            var simulateEntries = GetPhasesEntries(Simulate);
            simulationToolStripMenuItem.DropDownItems.Clear();

            if (LayoutModel.Components<IModelComponentIsCommandStation>(LayoutPhase.All).Any(cs => cs.LayoutEmulationSupported)) {
                if (simulateEntries.Count == 1)
                    simulateEventHandler = simulateEntries[0].Item2;
                else {
                    foreach (var entry in simulateEntries)
                        simulationToolStripMenuItem.DropDownItems.Add(entry.Item1, null, entry.Item2);
                }
            }
            else
                simulationToolStripMenuItem.Enabled = false;

            var verifyLayoutEntries = GetPhasesEntries(Verify);
            verifyLayoutToolStripMenuItem.DropDownItems.Clear();

            if (verifyLayoutEntries.Count == 1)
                verifyLayoutEventHandler = verifyLayoutEntries[0].Item2;
            else {
                foreach (var entry in verifyLayoutEntries)
                    verifyLayoutToolStripMenuItem.DropDownItems.Add(entry.Item1, null, entry.Item2);
            }
        }

        private void MenuItemSimulation_Click(object? sender, EventArgs e) {
            simulateEventHandler?.Invoke(sender, e);
        }

        private void MenuItemCompile_Click(object? sender, EventArgs e) {
            verifyLayoutEventHandler?.Invoke(sender, e);
        }

        private void Simulate(LayoutPhase phases) {
            LayoutController.Instance.EnterOperationModeRequest(new OperationModeParameters() { Phases = phases, Simulation = true });
        }

        private void Verify(LayoutPhase phases) {
            Dispatch.Call.ClearMessages();

            if (Dispatch.Call.CheckLayout(phases))
                MessageBox.Show(this, "Layout checked, all seems to be OK", "Layout Check Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MenuItemEmergencyStop_Click(object? sender, EventArgs e) {
            Dispatch.Call.EmergencyStopRequest("User initiated");
        }

        private void MenuItemConnectLayout_Click(object? sender, EventArgs e) => EventManager.Event(new LayoutEvent("connect-layout-to-control-request", this));

        private void MenuItemLearnLayout_Click(object? sender, EventArgs e) {
            try {
                if (LayoutController.Instance.BeginDesignTimeActivation()) {
                    var learnLayoutForm = (Form?)EventManager.Event(new LayoutEvent("activate-learn-layout", this));

                    if (learnLayoutForm == null) {
                        var learnLayout = new Dialogs.LearnLayout(Id) {
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

        private void MenuItemFind_Click(object? sender, EventArgs e) {
            var d = new Dialogs.FindComponents(ActiveAreaPage.Area);

            d.ShowDialog(this);
        }

        private void PrintDocument_PrintPage(object? sender, System.Drawing.Printing.PrintPageEventArgs e) {
            LayoutFrameWindowAreaTabPage areaPage;
            ILayoutView view;

            if (printState == null || e.Graphics == null)
                return;

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

        private void MenuItemPrint_Click(object? sender, EventArgs e) {
            printState = new PrintState(new Dialogs.Print(printDocument));
            if (printState.PrintDialog.ShowDialog(this) == DialogResult.OK) {
                printDocument.DocumentName = LayoutController.LayoutFilename ?? "Untitled";
                printDocument.Print();
            }
            else
                printState = null;
        }

        private void MenuItemRestoreDefaultZoomAndOrigin_Click(object? sender, EventArgs e) {
            if (ActiveView != null)
                ActiveView.SetOriginAndZoomToDefault();
        }

        private void MenuItemSaveAsDefaultZoomAndOrigin_Click(object? sender, EventArgs e) {
            if (ActiveView != null && MessageBox.Show("Do you want to set current position and zoom as the default for this view?", "Set Default Origin & View", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                ActiveView.SetAsDefaultOriginAndZoom();
        }

        private void MenuItemSelectUnconnectedComponents_Click(object? sender, EventArgs e) {
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

        private void MenuItemSelectUnlinkedSignals_Click(object? sender, EventArgs e) {
            var map = Dispatch.Call.GetLinkedSignalMap();
            LayoutSelection selection = new();

            foreach (LayoutSignalComponent signal in LayoutModel.Components<LayoutSignalComponent>(LayoutPhase.All))
                if (!map.ContainsKey(signal.Id))
                    selection.Add(signal);

            if (selection.Count == 0)
                LayoutModuleBase.Message("All signals are linked to logical signals (represented by block edge components)");
            else
                LayoutModuleBase.Message(selection, "Signals which are not linked. You should link block edges to signals");

            EventManager.Event(new LayoutEvent("show-messages", this));
        }

        private void MenuItemSelectUnlinkedTrackLinks_Click(object? sender, EventArgs e) {
            LayoutSelection selection = new();

            foreach (LayoutTrackLinkComponent trackLink in LayoutModel.Components<LayoutTrackLinkComponent>(LayoutPhase.All))
                if (trackLink.LinkedComponent == null)
                    selection.Add(trackLink);

            if (selection.Count == 0)
                LayoutModuleBase.Message("All track links are linked");
            else
                LayoutModuleBase.Message(selection, "Track links which are not yet linked");

            EventManager.Event(new LayoutEvent("show-messages", this));
        }

        private void MenuItemNewComponentDesignPhase_Click(object? sender, EventArgs e) {
            var command = new ChangeDefaultPhaseCommand(LayoutPhase.Planned);

            LayoutController.Do(command);
        }

        private void MenuItemNewComponentConstructionPhase_Click(object? sender, EventArgs e) {
            var command = new ChangeDefaultPhaseCommand(LayoutPhase.Construction);

            LayoutController.Do(command);
        }

        private void MenuItemNewComponentOperationalPhase_Click(object? sender, EventArgs e) {
            var command = new ChangeDefaultPhaseCommand(LayoutPhase.Operational);

            LayoutController.Do(command);
        }

        private void MenuItemSetToDesignPhase_Click(object? sender, EventArgs e) {
            EventManager.Event(new LayoutEventInfoValueType<LayoutSelection, LayoutPhase>("change-phase", null, LayoutPhase.Planned));
        }

        private void MenuItemSetToConstructionPhase_Click(object? sender, EventArgs e) {
            EventManager.Event(new LayoutEventInfoValueType<LayoutSelection, LayoutPhase>("change-phase", null, LayoutPhase.Construction));
        }

        private void MenuItemSetToOperationalPhase_Click(object? sender, EventArgs e) {
            EventManager.Event(new LayoutEventInfoValueType<LayoutSelection, LayoutPhase>("change-phase", null, LayoutPhase.Operational));
        }

        private void NewComponentsPhaseToolStripMenuItem_DropDownOpening(object? sender, EventArgs e) {
            newComponentPlanPhaseToolStripMenuItem.Checked = LayoutModel.Instance.DefaultPhase == LayoutPhase.Planned;
            newComponentConstructionPhaseToolStripMenuItem.Checked = LayoutModel.Instance.DefaultPhase == LayoutPhase.Construction;
            newComponentOperationalPhaseToolStripMenuItem.Checked = LayoutModel.Instance.DefaultPhase == LayoutPhase.Operational;
        }

        private void MenuItemViewActivePhases_Click(object? sender, EventArgs e) {
            if (ActiveViewPage != null) {
                onlyActiveLayoutPhasesToolStripMenuItem.Checked = !onlyActiveLayoutPhasesToolStripMenuItem.Checked;
                ActiveViewPage.ShowActivePhases = onlyActiveLayoutPhasesToolStripMenuItem.Checked;

                LayoutPhase phases = ActiveViewPage.ShowActivePhases ? LayoutModel.ActivePhases : LayoutPhase.All;

                if (phases != ActiveViewPage.View.Phases) {
                    ActiveViewPage.View.Phases = phases;
                    ActiveViewPage.View.Invalidate();
                }
            }
        }

        private void NewWindowToolStripMenuItem_Click(object? sender, EventArgs e) {
            ControllerCommand = FrameWindowCommand.NewWindow;
        }

        private void MenuItemCloseWindow_Click(object? sender, EventArgs e) {
            Close();
        }

        private void MenuItemSelectPowerConnectors_Click(object? sender, EventArgs e) {
            var selection = new LayoutSelection(LayoutModel.Components<LayoutTrackPowerConnectorComponent>(LayoutPhase.All));

            if (selection.Count == 0)
                LayoutModuleBase.Warning("No track power connectors were found");
            else
                LayoutModuleBase.Message(selection, "Track power connectors");
        }

        private void MenuItemSelectReverseLoopModules_Click(object? sender, EventArgs e) {
            var selection = new LayoutSelection(LayoutModel.AllComponents<LayoutTrackReverseLoopModule>(LayoutPhase.All));

            if (selection.Count == 0)
                LayoutModuleBase.Warning("No reverse loop modules were found");
            else
                LayoutModuleBase.Message(selection, "Reverse loop modules");
        }

        private void MenuItemVerificationOptions_Click(object? sender, EventArgs e) {
            var d = new Tools.Dialogs.LayoutVerificationOptions();

            d.ShowDialog(this);
        }

        private void MenuItemTrainTracking_Click(object? sender, EventArgs e) {
            var d = new CommonUI.Dialogs.TrainTrackingOptions();

            d.ShowDialog();
        }

        private void SplitterLayoutControl_SplitterMoved(object sender, SplitterEventArgs e) {

        }

    }

    public class LayoutFrameWindowAreaTabPage : TabPage {
        public LayoutFrameWindowAreaTabPage(LayoutModelArea area) {
            this.Area = area;

            TabViews = new TabControl {
                Dock = DockStyle.Fill,
                Alignment = TabAlignment.Bottom
            };
            TabViews.SelectedIndexChanged += TabViews_SelectedIndexChanged;

            this.Text = Area.Name;
            this.Controls.Add(TabViews);
        }

        public LayoutModelArea Area {
            get;
        }

        public TabControl TabViews {
            get;
        }

        public FrameWindow FrameWindow => (FrameWindow)this.FindForm();

        protected override void Dispose(bool disposing) {
            if (disposing)
                TabViews.SelectedIndexChanged -= TabViews_SelectedIndexChanged;
        }

        private void TabViews_SelectedIndexChanged(object? sender, EventArgs e) {
            FrameWindow.OnViewChanged();
        }
#endif
    }
    public class LayoutFrameWindowAreaViewTabPage : TabPage {
        public LayoutFrameWindowAreaViewTabPage(LayoutView view) {
            this.View = view;
            view.Dock = DockStyle.Fill;
            Controls.Add(view);
            this.BorderStyle = BorderStyle.None;
            ShowActivePhases = false;
        }

        public LayoutView View { get; }

        public bool ShowActivePhases {
            get;
            set;
        }

        public void WriteXml(XmlWriter w) {
            w.WriteStartElement("View");
            w.WriteAttributeString("AreaID", XmlConvert.EncodeName(View.Area.AreaGuid.ToString()));
            w.WriteAttributeString("Name", XmlConvert.EncodeLocalName(this.Text));
            this.View.WriteXml(w);
            w.WriteEndElement();
        }
    }

    internal class PrintState {
        private int areaIndex;

        public PrintState(Dialogs.Print printDialog) {
            this.PrintDialog = printDialog;
        }

        public Dialogs.Print PrintDialog { get; }

        public bool AllAreas => PrintDialog.AllAreas;

        public PrintViewScope PrintViewScope => PrintDialog.PrintViewScope;

        public bool GridLines => PrintDialog.GridLines;

        public int AreaIndex {
            get => areaIndex;

            set {
                areaIndex = value;
                ViewIndex = 0;
            }
        }

        public int ViewIndex { get; set; }
    }

    public class FrameWindowState : LayoutXmlWrapper {
        private const string A_WindowState = "WindowState";
        private const string A_Left = "Left";
        private const string A_Top = "Top";
        private const string A_Width = "Width";
        private const string A_Height = "Height";
        private const string A_AreaStates = "AreaStates";
        private const string A_ActiveAreaIndex = "ActiveAreaIndex";
        private const string A_ControlViewerWidth = "ControlViewerWidth";
        private const string A_LocomotiveViewerWidth = "LocomotiveViewerWidth";
        private const string A_TripsViewerHeight = "TripsViewerHeight";
        private const string A_MessagesViewerHeight = "MessagesViewerHeight";
        private const string A_ControlViewerVisible = "ControlViewerVisible";
        private const string A_MessagesViewerVisible = "MessagesViewerVisible";
        private const string A_LocomotiveViewerVisible = "LocomotiveViewerVisible";
        private const string A_TripsViewerVisible = "TripsViewerVisible";

        public FrameWindowState(XmlElement element)
            : base(element) {
        }

        public FrameWindowState(XmlElement windowStatesElement, FrameWindow frameWindow) : base(windowStatesElement, A_WindowState, alwaysAppend: true) {
            WindowState = frameWindow.WindowState;
            Bounds = frameWindow.Bounds;

            XmlElement areaStatesElement = CreateChildElement(A_AreaStates);

            foreach (var areaTab in frameWindow.AreaTabs)
                _ = new FrameWindowAreaState(areaStatesElement, areaTab);

            this.ActiveAreaIndex = frameWindow.ActiveAreaIndex;
        }

        public void Restore(List<FrameWindow> frameWindows) {
            frameWindows.Add(new FrameWindow(this));
        }

        public void Restore(TabControl tabAreas) {
            foreach (var areaState in AreaStates)
                areaState.Restore(tabAreas);

            tabAreas.SelectedIndex = this.ActiveAreaIndex;
        }

        public FormWindowState WindowState {
            get => AttributeValue(A_WindowState).Enum<FormWindowState>() ?? FormWindowState.Normal;
            set => SetAttributeValue(A_WindowState, value);
        }

        public Rectangle Bounds {
            get => new((int)AttributeValue(A_Left), (int)AttributeValue(A_Top), (int)AttributeValue(A_Width), (int)AttributeValue(A_Height));

            set {
                SetAttributeValue(A_Left, value.Left);
                SetAttributeValue(A_Top, value.Top);
                SetAttributeValue(A_Width, value.Width);
                SetAttributeValue(A_Height, value.Height);
            }
        }

        public IEnumerable<FrameWindowAreaState> AreaStates {
            get {
                var areasElement = Element[A_AreaStates];

                if (areasElement != null)
                    foreach (XmlElement areaElement in areasElement)
                        yield return new FrameWindowAreaState(areaElement);
            }
        }

        public int ActiveAreaIndex {
            get => (int?)AttributeValue(A_ActiveAreaIndex) ?? 0;
            set => SetAttributeValue(A_ActiveAreaIndex, value);
        }

        public bool ControlViewerVisible {
            get => (bool?)AttributeValue(A_ControlViewerVisible) ?? false;
            set => SetAttributeValue(A_ControlViewerVisible, value);

        }

        public int ControlViewerWidth {
            get => (int?)AttributeValue(A_ControlViewerWidth) ?? 0;
            set => SetAttributeValue(A_ControlViewerWidth, value);
        }

        public bool LocomotiveViewerVisble {
            get => (bool?)AttributeValue(A_LocomotiveViewerVisible) ?? true;
            set => SetAttributeValue(A_LocomotiveViewerVisible, value);
        }

        public int LocomotiveViewerWidth {
            get => (int?)AttributeValue(A_LocomotiveViewerWidth) ?? 0;
            set => SetAttributeValue(A_LocomotiveViewerWidth, value);
        }

        public bool TripsViewerVisible {
            get => (bool?)AttributeValue(A_TripsViewerVisible) ?? false;
            set => SetAttributeValue(A_TripsViewerVisible, value);
        }

        public int TripsViewerHeight {
            get => (int?)AttributeValue(A_TripsViewerHeight) ?? 0;
            set => SetAttributeValue(A_TripsViewerHeight, value);
        }

        public bool MessagesViewerVisible {
            get => (bool?)AttributeValue(A_MessagesViewerVisible) ?? false;
            set => SetAttributeValue(A_MessagesViewerVisible, value);
        }

        public int MessagesViewerHeight {
            get => (int?)AttributeValue(A_MessagesViewerHeight) ?? 0;
            set => SetAttributeValue(A_MessagesViewerHeight, value);
        }
    }

    public class FrameWindowAreaState : LayoutXmlWrapper {
        private const string E_AreaState = "AreaState";
        private const string E_ViewStates = "ViewStates";
        private const string A_Id = "ID";
        private const string A_ActiveViewIndex = "ActiveViewIndex";

        public FrameWindowAreaState(XmlElement element)
            : base(element) {
        }

        public FrameWindowAreaState(XmlElement areaStatesElement, LayoutFrameWindowAreaTabPage areaTab)
          : base(areaStatesElement, E_AreaState, alwaysAppend: true) {
            AreaId = areaTab.Area.AreaGuid;

            XmlElement viewStatesElement = CreateChildElement(E_ViewStates);

            foreach (LayoutFrameWindowAreaViewTabPage viewTab in areaTab.TabViews.TabPages)
                _ = new FrameWindowViewState(viewStatesElement, viewTab);

            ActiveViewIndex = areaTab.TabViews.SelectedIndex;
        }

        public void Restore(TabControl tabAreas) {
            LayoutModelArea area = LayoutModel.Areas[AreaId];

            if (area != null) {
                LayoutFrameWindowAreaTabPage areaTab = new(area);

                tabAreas.TabPages.Add(areaTab);

                foreach (var viewState in ViewStates)
                    viewState.Restore(areaTab);

                areaTab.TabViews.SelectedIndex = ActiveViewIndex;
            }
        }

        public Guid AreaId {
            get => (Guid)AttributeValue(A_Id);
            set => SetAttributeValue(A_Id, value);
        }

        public IEnumerable<FrameWindowViewState> ViewStates {
            get {
                var viewsElement = Element[E_ViewStates];

                if (viewsElement != null)
                    foreach (XmlElement viewElement in viewsElement)
                        yield return new FrameWindowViewState(viewElement);
            }
        }

        public int ActiveViewIndex {
            get => (int?)AttributeValue(A_ActiveViewIndex) ?? 0;
            set => SetAttributeValue(A_ActiveViewIndex, value);
        }
    }

    public class FrameWindowViewState : LayoutXmlWrapper {
        private const string A_Name = "Name";
        private const string A_Zoom = "Zoom";
        private const string A_DefaultZoom = "DefaultZoom";
        private const string A_OriginX = "OriginX";
        private const string A_OriginY = "OriginY";
        private const string A_DefaultOriginX = "DefaultOriginX";
        private const string A_DefaultOriginY = "DefaultOriginY";
        private const string A_Grid = "Grid";
        private const string A_ShowCoordinates = "ShowCoordinates";
        private const string A_ShowActivePhases = "ShowActivePhases";

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
            get => GetAttribute(A_Name);
            set => SetAttributeValue(A_Name, value);
        }

        public float Zoom {
            get => (float?)AttributeValue(A_Zoom) ?? 1.0f;
            set => SetAttributeValue(A_Zoom, value);
        }

        public float DefaultZoom {
            get => (float?)AttributeValue(A_DefaultZoom) ?? 1.0f;
            set => SetAttributeValue(A_DefaultZoom, value);
        }

        public Point Origin {
            get => new((int?)AttributeValue(A_OriginX) ?? 0, (int?)AttributeValue(A_OriginY) ?? 0);
            set {
                SetAttributeValue(A_OriginX, value.X);
                SetAttributeValue(A_OriginY, value.Y);
            }
        }

        public Point DefaultOrigin {
            get => new((int?)AttributeValue(A_DefaultOriginX) ?? 0, (int?)AttributeValue(A_DefaultOriginY) ?? 0);
            set {
                SetAttributeValue(A_DefaultOriginX, value.X);
                SetAttributeValue(A_DefaultOriginY, value.Y);
            }
        }

        public ShowGridLinesOption Grid {
            get => AttributeValue(A_Grid).Enum<ShowGridLinesOption>() ?? ShowGridLinesOption.Hide;
            set => SetAttributeValue(A_Grid, value);
        }

        public bool ShowCoordinates {
            get => (bool?)AttributeValue(A_ShowCoordinates) ?? false;
            set => SetAttributeValue(A_ShowCoordinates, value);
        }

        public bool ShowActivePhases {
            get => (bool?)AttributeValue(A_ShowActivePhases) ?? false;
            set => SetAttributeValue(A_ShowActivePhases, value);
        }
    }
}

