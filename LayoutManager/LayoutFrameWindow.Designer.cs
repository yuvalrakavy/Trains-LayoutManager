using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Collections;

namespace LayoutManager {
	public partial class LayoutFrameWindow {
		private System.ComponentModel.IContainer components;

		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayoutFrameWindow));
            this.toolBarMain = new System.Windows.Forms.ToolBar();
            this.toolBarButtonOpenLayiout = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonSaveLayout = new System.Windows.Forms.ToolBarButton();
            this.Sep1 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonDesignMode = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonOperationMode = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonSimulation = new System.Windows.Forms.ToolBarButton();
            this.sep2 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonShowLayoutControl = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonShowTripsMonitor = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonShowMessages = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonShowLocomotives = new System.Windows.Forms.ToolBarButton();
            this.sep3 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonSetToDefaultOriginAndZoom = new System.Windows.Forms.ToolBarButton();
            this.toolbarButtonSetAsDefaultOriginAndZoom = new System.Windows.Forms.ToolBarButton();
            this.set6 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonToggleGrid = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonZoom100 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonZoomIn = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonZoomOut = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonZoomFitAll = new System.Windows.Forms.ToolBarButton();
            this.sep4 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonToggleAllLayoutManualDispatch = new System.Windows.Forms.ToolBarButton();
            this.sep5 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonStopAllLocomotives = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonEmergencyStop = new System.Windows.Forms.ToolBarButton();
            this.imageListTools = new System.Windows.Forms.ImageList(this.components);
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.statusBarPanelOperation = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanelMode = new System.Windows.Forms.StatusBarPanel();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItemView = new System.Windows.Forms.MenuItem();
            this.menuItemViewNew = new System.Windows.Forms.MenuItem();
            this.menuItemViewDelete = new System.Windows.Forms.MenuItem();
            this.menuItemViewRename = new System.Windows.Forms.MenuItem();
            this.menuItemSeperator6 = new System.Windows.Forms.MenuItem();
            this.menuItemZoomMenu = new System.Windows.Forms.MenuItem();
            this.menuItemSetZoom = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItemZoom200 = new System.Windows.Forms.MenuItem();
            this.menuItemZoom150 = new System.Windows.Forms.MenuItem();
            this.menuItemZoom100 = new System.Windows.Forms.MenuItem();
            this.menuItemZoom75 = new System.Windows.Forms.MenuItem();
            this.menuItemZoom50 = new System.Windows.Forms.MenuItem();
            this.menuItemZoom25 = new System.Windows.Forms.MenuItem();
            this.menuItem12 = new System.Windows.Forms.MenuItem();
            this.menuItem1ZoomAllArea = new System.Windows.Forms.MenuItem();
            this.menuItemViewGrid = new System.Windows.Forms.MenuItem();
            this.menuItemViewActivePhases = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.menuItemRestoreDefaultZoomAndOrigin = new System.Windows.Forms.MenuItem();
            this.menuItemSaveAsDefaultZoomAndOrigin = new System.Windows.Forms.MenuItem();
            this.menuItem10 = new System.Windows.Forms.MenuItem();
            this.menuItemShowLayoutControl = new System.Windows.Forms.MenuItem();
            this.menuItemShowTripsMonitor = new System.Windows.Forms.MenuItem();
            this.menuItemShowMessages = new System.Windows.Forms.MenuItem();
            this.menuItemShowLocomotives = new System.Windows.Forms.MenuItem();
            this.menuSeperator4 = new System.Windows.Forms.MenuItem();
            this.menuItemViewArrage = new System.Windows.Forms.MenuItem();
            this.menuItemOperational = new System.Windows.Forms.MenuItem();
            this.menuItemTools = new System.Windows.Forms.MenuItem();
            this.menuItemDummy = new System.Windows.Forms.MenuItem();
            this.menuLayout = new System.Windows.Forms.MenuItem();
            this.menuItemDesign = new System.Windows.Forms.MenuItem();
            this.menuItemSimulation = new System.Windows.Forms.MenuItem();
            this.menuItemDesignTimeOnlySeperator = new System.Windows.Forms.MenuItem();
            this.menuItemCompile = new System.Windows.Forms.MenuItem();
            this.menuItemVerificationOptions = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.menuItemLearnLayout = new System.Windows.Forms.MenuItem();
            this.menuItemConnectLayout = new System.Windows.Forms.MenuItem();
            this.menuItemEmergencyStop = new System.Windows.Forms.MenuItem();
            this.menuItemSuspendLocomotives = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItemPolicies = new System.Windows.Forms.MenuItem();
            this.menuItemDefaultDriverParameters = new System.Windows.Forms.MenuItem();
            this.menuItemCommonDestinations = new System.Windows.Forms.MenuItem();
            this.menuItemManualDispatchRegions = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuFile = new System.Windows.Forms.MenuItem();
            this.menuItemNewWindow = new System.Windows.Forms.MenuItem();
            this.menuItemSave = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.menuItemPrint = new System.Windows.Forms.MenuItem();
            this.menuItemSeperator1 = new System.Windows.Forms.MenuItem();
            this.menuItemModules = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItemCloseWindow = new System.Windows.Forms.MenuItem();
            this.menuItemExit = new System.Windows.Forms.MenuItem();
            this.menuEdit = new System.Windows.Forms.MenuItem();
            this.menuItemUndo = new System.Windows.Forms.MenuItem();
            this.menuItemRedo = new System.Windows.Forms.MenuItem();
            this.menuItemSeparator = new System.Windows.Forms.MenuItem();
            this.menuItemCopy = new System.Windows.Forms.MenuItem();
            this.menuItemCut = new System.Windows.Forms.MenuItem();
            this.menuItemFind = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItemSelect = new System.Windows.Forms.MenuItem();
            this.menuItemSelectUnconnectedComponents = new System.Windows.Forms.MenuItem();
            this.menuItemSelectUnlinkedSignals = new System.Windows.Forms.MenuItem();
            this.menuItemSelectUnlinkedTrackLinks = new System.Windows.Forms.MenuItem();
            this.menuItemSelectPowerConnectors = new System.Windows.Forms.MenuItem();
            this.menuItemSelectReverseLoopModules = new System.Windows.Forms.MenuItem();
            this.menuItem11 = new System.Windows.Forms.MenuItem();
            this.menuItemDefaultPhase = new System.Windows.Forms.MenuItem();
            this.menuItemNewComponentDesignPhase = new System.Windows.Forms.MenuItem();
            this.menuItemNewComponentConstructionPhase = new System.Windows.Forms.MenuItem();
            this.menuItemNewComponentOperationalPhase = new System.Windows.Forms.MenuItem();
            this.menuItemSetComponentPhase = new System.Windows.Forms.MenuItem();
            this.menuItemSetToDesignPhase = new System.Windows.Forms.MenuItem();
            this.menuItemSetToConstructionPhase = new System.Windows.Forms.MenuItem();
            this.menuItemSetToOperationalPhase = new System.Windows.Forms.MenuItem();
            this.menuItem14 = new System.Windows.Forms.MenuItem();
            this.menuItemStyles = new System.Windows.Forms.MenuItem();
            this.menuItemStyleFonts = new System.Windows.Forms.MenuItem();
            this.menuItemStylePositions = new System.Windows.Forms.MenuItem();
            this.menuItemLocomotiveCatalog = new System.Windows.Forms.MenuItem();
            this.menuItemAccelerationProfiles = new System.Windows.Forms.MenuItem();
            this.menuItemArea = new System.Windows.Forms.MenuItem();
            this.menuItemNewArea = new System.Windows.Forms.MenuItem();
            this.menuItemDeleteArea = new System.Windows.Forms.MenuItem();
            this.menuItemRenameArea = new System.Windows.Forms.MenuItem();
            this.menuSeperator2 = new System.Windows.Forms.MenuItem();
            this.menuItemAreaArrange = new System.Windows.Forms.MenuItem();
            this.panelMessageViewer = new System.Windows.Forms.Panel();
            this.panelTripsMonitor = new System.Windows.Forms.Panel();
            this.splitterTripsMonitor = new System.Windows.Forms.Splitter();
            this.tabAreas = new System.Windows.Forms.TabControl();
            this.tripsMonitor = new LayoutManager.Tools.Controls.TripsMonitor();
            this.splitterMessages = new System.Windows.Forms.Splitter();
            this.messageViewer = new LayoutManager.MessageViewer();
            this.splitterLocomotives = new System.Windows.Forms.Splitter();
            this.panelLocomotiveViewer = new System.Windows.Forms.Panel();
            this.locomotivesViewer = new LayoutManager.LocomotivesViewer();
            this.panelLayoutControl = new System.Windows.Forms.Panel();
            this.splitterLayoutControl = new System.Windows.Forms.Splitter();
            this.layoutControlViewer = new LayoutManager.CommonUI.Controls.LayoutControlViewer();
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.timerFreeResources = new System.Timers.Timer();
            this.timerDetailsPopup = new System.Windows.Forms.Timer(this.components);
            this.printDocument = new System.Drawing.Printing.PrintDocument();
            this.menuItem13 = new System.Windows.Forms.MenuItem();
            this.menuItemTrainTracking = new System.Windows.Forms.MenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanelOperation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanelMode)).BeginInit();
            this.panelMessageViewer.SuspendLayout();
            this.panelTripsMonitor.SuspendLayout();
            this.panelLocomotiveViewer.SuspendLayout();
            this.panelLayoutControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timerFreeResources)).BeginInit();
            this.SuspendLayout();
            // 
            // toolBarMain
            // 
            this.toolBarMain.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.toolBarButtonOpenLayiout,
            this.toolBarButtonSaveLayout,
            this.Sep1,
            this.toolBarButtonDesignMode,
            this.toolBarButtonOperationMode,
            this.toolBarButtonSimulation,
            this.sep2,
            this.toolBarButtonShowLayoutControl,
            this.toolBarButtonShowTripsMonitor,
            this.toolBarButtonShowMessages,
            this.toolBarButtonShowLocomotives,
            this.sep3,
            this.toolBarButtonSetToDefaultOriginAndZoom,
            this.toolbarButtonSetAsDefaultOriginAndZoom,
            this.set6,
            this.toolBarButtonToggleGrid,
            this.toolBarButtonZoom100,
            this.toolBarButtonZoomIn,
            this.toolBarButtonZoomOut,
            this.toolBarButtonZoomFitAll,
            this.sep4,
            this.toolBarButtonToggleAllLayoutManualDispatch,
            this.sep5,
            this.toolBarButtonStopAllLocomotives,
            this.toolBarButtonEmergencyStop});
            resources.ApplyResources(this.toolBarMain, "toolBarMain");
            this.toolBarMain.ImageList = this.imageListTools;
            this.toolBarMain.Name = "toolBarMain";
            this.toolBarMain.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBarMain_ButtonClick);
            // 
            // toolBarButtonOpenLayiout
            // 
            resources.ApplyResources(this.toolBarButtonOpenLayiout, "toolBarButtonOpenLayiout");
            this.toolBarButtonOpenLayiout.Name = "toolBarButtonOpenLayiout";
            // 
            // toolBarButtonSaveLayout
            // 
            resources.ApplyResources(this.toolBarButtonSaveLayout, "toolBarButtonSaveLayout");
            this.toolBarButtonSaveLayout.Name = "toolBarButtonSaveLayout";
            // 
            // Sep1
            // 
            this.Sep1.Name = "Sep1";
            this.Sep1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // toolBarButtonDesignMode
            // 
            resources.ApplyResources(this.toolBarButtonDesignMode, "toolBarButtonDesignMode");
            this.toolBarButtonDesignMode.Name = "toolBarButtonDesignMode";
            this.toolBarButtonDesignMode.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            // 
            // toolBarButtonOperationMode
            // 
            resources.ApplyResources(this.toolBarButtonOperationMode, "toolBarButtonOperationMode");
            this.toolBarButtonOperationMode.Name = "toolBarButtonOperationMode";
            this.toolBarButtonOperationMode.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            // 
            // toolBarButtonSimulation
            // 
            resources.ApplyResources(this.toolBarButtonSimulation, "toolBarButtonSimulation");
            this.toolBarButtonSimulation.Name = "toolBarButtonSimulation";
            this.toolBarButtonSimulation.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            // 
            // sep2
            // 
            this.sep2.Name = "sep2";
            this.sep2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // toolBarButtonShowLayoutControl
            // 
            resources.ApplyResources(this.toolBarButtonShowLayoutControl, "toolBarButtonShowLayoutControl");
            this.toolBarButtonShowLayoutControl.Name = "toolBarButtonShowLayoutControl";
            // 
            // toolBarButtonShowTripsMonitor
            // 
            resources.ApplyResources(this.toolBarButtonShowTripsMonitor, "toolBarButtonShowTripsMonitor");
            this.toolBarButtonShowTripsMonitor.Name = "toolBarButtonShowTripsMonitor";
            // 
            // toolBarButtonShowMessages
            // 
            resources.ApplyResources(this.toolBarButtonShowMessages, "toolBarButtonShowMessages");
            this.toolBarButtonShowMessages.Name = "toolBarButtonShowMessages";
            this.toolBarButtonShowMessages.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            // 
            // toolBarButtonShowLocomotives
            // 
            resources.ApplyResources(this.toolBarButtonShowLocomotives, "toolBarButtonShowLocomotives");
            this.toolBarButtonShowLocomotives.Name = "toolBarButtonShowLocomotives";
            this.toolBarButtonShowLocomotives.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            // 
            // sep3
            // 
            this.sep3.Name = "sep3";
            this.sep3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // toolBarButtonSetToDefaultOriginAndZoom
            // 
            resources.ApplyResources(this.toolBarButtonSetToDefaultOriginAndZoom, "toolBarButtonSetToDefaultOriginAndZoom");
            this.toolBarButtonSetToDefaultOriginAndZoom.Name = "toolBarButtonSetToDefaultOriginAndZoom";
            // 
            // toolbarButtonSetAsDefaultOriginAndZoom
            // 
            resources.ApplyResources(this.toolbarButtonSetAsDefaultOriginAndZoom, "toolbarButtonSetAsDefaultOriginAndZoom");
            this.toolbarButtonSetAsDefaultOriginAndZoom.Name = "toolbarButtonSetAsDefaultOriginAndZoom";
            // 
            // set6
            // 
            this.set6.Name = "set6";
            this.set6.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // toolBarButtonToggleGrid
            // 
            resources.ApplyResources(this.toolBarButtonToggleGrid, "toolBarButtonToggleGrid");
            this.toolBarButtonToggleGrid.Name = "toolBarButtonToggleGrid";
            this.toolBarButtonToggleGrid.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            // 
            // toolBarButtonZoom100
            // 
            resources.ApplyResources(this.toolBarButtonZoom100, "toolBarButtonZoom100");
            this.toolBarButtonZoom100.Name = "toolBarButtonZoom100";
            // 
            // toolBarButtonZoomIn
            // 
            resources.ApplyResources(this.toolBarButtonZoomIn, "toolBarButtonZoomIn");
            this.toolBarButtonZoomIn.Name = "toolBarButtonZoomIn";
            // 
            // toolBarButtonZoomOut
            // 
            resources.ApplyResources(this.toolBarButtonZoomOut, "toolBarButtonZoomOut");
            this.toolBarButtonZoomOut.Name = "toolBarButtonZoomOut";
            // 
            // toolBarButtonZoomFitAll
            // 
            resources.ApplyResources(this.toolBarButtonZoomFitAll, "toolBarButtonZoomFitAll");
            this.toolBarButtonZoomFitAll.Name = "toolBarButtonZoomFitAll";
            // 
            // sep4
            // 
            this.sep4.Name = "sep4";
            this.sep4.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // toolBarButtonToggleAllLayoutManualDispatch
            // 
            resources.ApplyResources(this.toolBarButtonToggleAllLayoutManualDispatch, "toolBarButtonToggleAllLayoutManualDispatch");
            this.toolBarButtonToggleAllLayoutManualDispatch.Name = "toolBarButtonToggleAllLayoutManualDispatch";
            this.toolBarButtonToggleAllLayoutManualDispatch.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            // 
            // sep5
            // 
            this.sep5.Name = "sep5";
            this.sep5.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // toolBarButtonStopAllLocomotives
            // 
            resources.ApplyResources(this.toolBarButtonStopAllLocomotives, "toolBarButtonStopAllLocomotives");
            this.toolBarButtonStopAllLocomotives.Name = "toolBarButtonStopAllLocomotives";
            this.toolBarButtonStopAllLocomotives.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            // 
            // toolBarButtonEmergencyStop
            // 
            resources.ApplyResources(this.toolBarButtonEmergencyStop, "toolBarButtonEmergencyStop");
            this.toolBarButtonEmergencyStop.Name = "toolBarButtonEmergencyStop";
            // 
            // imageListTools
            // 
            this.imageListTools.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTools.ImageStream")));
            this.imageListTools.TransparentColor = System.Drawing.Color.Silver;
            this.imageListTools.Images.SetKeyName(0, "");
            this.imageListTools.Images.SetKeyName(1, "");
            this.imageListTools.Images.SetKeyName(2, "");
            this.imageListTools.Images.SetKeyName(3, "");
            this.imageListTools.Images.SetKeyName(4, "");
            this.imageListTools.Images.SetKeyName(5, "");
            this.imageListTools.Images.SetKeyName(6, "");
            this.imageListTools.Images.SetKeyName(7, "");
            this.imageListTools.Images.SetKeyName(8, "");
            this.imageListTools.Images.SetKeyName(9, "");
            this.imageListTools.Images.SetKeyName(10, "");
            this.imageListTools.Images.SetKeyName(11, "");
            this.imageListTools.Images.SetKeyName(12, "");
            this.imageListTools.Images.SetKeyName(13, "");
            this.imageListTools.Images.SetKeyName(14, "");
            this.imageListTools.Images.SetKeyName(15, "");
            this.imageListTools.Images.SetKeyName(16, "SetToDefaultOriginAndZoom.bmp");
            this.imageListTools.Images.SetKeyName(17, "SetAsDefaultOriginAndZoom.bmp");
            this.imageListTools.Images.SetKeyName(18, "Simulate.bmp");
            // 
            // statusBar
            // 
            resources.ApplyResources(this.statusBar, "statusBar");
            this.statusBar.Name = "statusBar";
            this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPanelOperation,
            this.statusBarPanelMode});
            this.statusBar.ShowPanels = true;
            // 
            // statusBarPanelOperation
            // 
            this.statusBarPanelOperation.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            resources.ApplyResources(this.statusBarPanelOperation, "statusBarPanelOperation");
            // 
            // statusBarPanelMode
            // 
            resources.ApplyResources(this.statusBarPanelMode, "statusBarPanelMode");
            // 
            // menuItem2
            // 
            this.menuItem2.Index = -1;
            resources.ApplyResources(this.menuItem2, "menuItem2");
            // 
            // menuItemView
            // 
            this.menuItemView.Index = 2;
            this.menuItemView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemViewNew,
            this.menuItemViewDelete,
            this.menuItemViewRename,
            this.menuItemSeperator6,
            this.menuItemZoomMenu,
            this.menuItemViewGrid,
            this.menuItemViewActivePhases,
            this.menuItem7,
            this.menuItemRestoreDefaultZoomAndOrigin,
            this.menuItemSaveAsDefaultZoomAndOrigin,
            this.menuItem10,
            this.menuItemShowLayoutControl,
            this.menuItemShowTripsMonitor,
            this.menuItemShowMessages,
            this.menuItemShowLocomotives,
            this.menuSeperator4,
            this.menuItemViewArrage});
            resources.ApplyResources(this.menuItemView, "menuItemView");
            this.menuItemView.Popup += new System.EventHandler(this.menuItemView_Popup);
            // 
            // menuItemViewNew
            // 
            this.menuItemViewNew.Index = 0;
            resources.ApplyResources(this.menuItemViewNew, "menuItemViewNew");
            this.menuItemViewNew.Click += new System.EventHandler(this.menuItemViewNew_Click);
            // 
            // menuItemViewDelete
            // 
            this.menuItemViewDelete.Index = 1;
            resources.ApplyResources(this.menuItemViewDelete, "menuItemViewDelete");
            this.menuItemViewDelete.Click += new System.EventHandler(this.menuItemViewDelete_Click);
            // 
            // menuItemViewRename
            // 
            this.menuItemViewRename.Index = 2;
            resources.ApplyResources(this.menuItemViewRename, "menuItemViewRename");
            this.menuItemViewRename.Click += new System.EventHandler(this.menuItemViewRename_Click);
            // 
            // menuItemSeperator6
            // 
            this.menuItemSeperator6.Index = 3;
            resources.ApplyResources(this.menuItemSeperator6, "menuItemSeperator6");
            // 
            // menuItemZoomMenu
            // 
            this.menuItemZoomMenu.Index = 4;
            this.menuItemZoomMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemSetZoom,
            this.menuItem5,
            this.menuItemZoom200,
            this.menuItemZoom150,
            this.menuItemZoom100,
            this.menuItemZoom75,
            this.menuItemZoom50,
            this.menuItemZoom25,
            this.menuItem12,
            this.menuItem1ZoomAllArea});
            resources.ApplyResources(this.menuItemZoomMenu, "menuItemZoomMenu");
            this.menuItemZoomMenu.Popup += new System.EventHandler(this.menuItemZoomMenu_Popup);
            // 
            // menuItemSetZoom
            // 
            this.menuItemSetZoom.Index = 0;
            this.menuItemSetZoom.RadioCheck = true;
            resources.ApplyResources(this.menuItemSetZoom, "menuItemSetZoom");
            this.menuItemSetZoom.Click += new System.EventHandler(this.menuItemSetZoom_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 1;
            resources.ApplyResources(this.menuItem5, "menuItem5");
            // 
            // menuItemZoom200
            // 
            this.menuItemZoom200.Index = 2;
            this.menuItemZoom200.RadioCheck = true;
            resources.ApplyResources(this.menuItemZoom200, "menuItemZoom200");
            this.menuItemZoom200.Click += new System.EventHandler(this.menuItemZoomPreset_Click);
            // 
            // menuItemZoom150
            // 
            this.menuItemZoom150.Index = 3;
            this.menuItemZoom150.RadioCheck = true;
            resources.ApplyResources(this.menuItemZoom150, "menuItemZoom150");
            this.menuItemZoom150.Click += new System.EventHandler(this.menuItemZoomPreset_Click);
            // 
            // menuItemZoom100
            // 
            this.menuItemZoom100.Index = 4;
            this.menuItemZoom100.RadioCheck = true;
            resources.ApplyResources(this.menuItemZoom100, "menuItemZoom100");
            this.menuItemZoom100.Click += new System.EventHandler(this.menuItemZoomPreset_Click);
            // 
            // menuItemZoom75
            // 
            this.menuItemZoom75.Index = 5;
            this.menuItemZoom75.RadioCheck = true;
            resources.ApplyResources(this.menuItemZoom75, "menuItemZoom75");
            this.menuItemZoom75.Click += new System.EventHandler(this.menuItemZoomPreset_Click);
            // 
            // menuItemZoom50
            // 
            this.menuItemZoom50.Index = 6;
            this.menuItemZoom50.RadioCheck = true;
            resources.ApplyResources(this.menuItemZoom50, "menuItemZoom50");
            this.menuItemZoom50.Click += new System.EventHandler(this.menuItemZoomPreset_Click);
            // 
            // menuItemZoom25
            // 
            this.menuItemZoom25.Index = 7;
            this.menuItemZoom25.RadioCheck = true;
            resources.ApplyResources(this.menuItemZoom25, "menuItemZoom25");
            this.menuItemZoom25.Click += new System.EventHandler(this.menuItemZoomPreset_Click);
            // 
            // menuItem12
            // 
            this.menuItem12.Index = 8;
            resources.ApplyResources(this.menuItem12, "menuItem12");
            // 
            // menuItem1ZoomAllArea
            // 
            this.menuItem1ZoomAllArea.Index = 9;
            resources.ApplyResources(this.menuItem1ZoomAllArea, "menuItem1ZoomAllArea");
            this.menuItem1ZoomAllArea.Click += new System.EventHandler(this.menuItemZoomAllArea_Click);
            // 
            // menuItemViewGrid
            // 
            this.menuItemViewGrid.Checked = true;
            this.menuItemViewGrid.Index = 5;
            resources.ApplyResources(this.menuItemViewGrid, "menuItemViewGrid");
            this.menuItemViewGrid.Click += new System.EventHandler(this.menuItemViewGrid_Click);
            // 
            // menuItemViewActivePhases
            // 
            this.menuItemViewActivePhases.Index = 6;
            resources.ApplyResources(this.menuItemViewActivePhases, "menuItemViewActivePhases");
            this.menuItemViewActivePhases.Click += new System.EventHandler(this.menuItemViewActivePhases_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 7;
            resources.ApplyResources(this.menuItem7, "menuItem7");
            // 
            // menuItemRestoreDefaultZoomAndOrigin
            // 
            this.menuItemRestoreDefaultZoomAndOrigin.Index = 8;
            resources.ApplyResources(this.menuItemRestoreDefaultZoomAndOrigin, "menuItemRestoreDefaultZoomAndOrigin");
            this.menuItemRestoreDefaultZoomAndOrigin.Click += new System.EventHandler(this.menuItemRestoreDefaultZoomAndOrigin_Click);
            // 
            // menuItemSaveAsDefaultZoomAndOrigin
            // 
            this.menuItemSaveAsDefaultZoomAndOrigin.Index = 9;
            resources.ApplyResources(this.menuItemSaveAsDefaultZoomAndOrigin, "menuItemSaveAsDefaultZoomAndOrigin");
            this.menuItemSaveAsDefaultZoomAndOrigin.Click += new System.EventHandler(this.menuItemSaveAsDefaultZoomAndOrigin_Click);
            // 
            // menuItem10
            // 
            this.menuItem10.Index = 10;
            resources.ApplyResources(this.menuItem10, "menuItem10");
            // 
            // menuItemShowLayoutControl
            // 
            this.menuItemShowLayoutControl.Index = 11;
            resources.ApplyResources(this.menuItemShowLayoutControl, "menuItemShowLayoutControl");
            this.menuItemShowLayoutControl.Click += new System.EventHandler(this.MenuItemShowLayoutControl_Click);
            // 
            // menuItemShowTripsMonitor
            // 
            this.menuItemShowTripsMonitor.Index = 12;
            resources.ApplyResources(this.menuItemShowTripsMonitor, "menuItemShowTripsMonitor");
            this.menuItemShowTripsMonitor.Click += new System.EventHandler(this.menuItemShowTripsMonitor_Click);
            // 
            // menuItemShowMessages
            // 
            this.menuItemShowMessages.Index = 13;
            resources.ApplyResources(this.menuItemShowMessages, "menuItemShowMessages");
            this.menuItemShowMessages.Click += new System.EventHandler(this.menuItemShowMessages_Click);
            // 
            // menuItemShowLocomotives
            // 
            this.menuItemShowLocomotives.Index = 14;
            resources.ApplyResources(this.menuItemShowLocomotives, "menuItemShowLocomotives");
            this.menuItemShowLocomotives.Click += new System.EventHandler(this.menuItemShowLocomotives_Click);
            // 
            // menuSeperator4
            // 
            this.menuSeperator4.Index = 15;
            resources.ApplyResources(this.menuSeperator4, "menuSeperator4");
            // 
            // menuItemViewArrage
            // 
            this.menuItemViewArrage.Index = 16;
            resources.ApplyResources(this.menuItemViewArrage, "menuItemViewArrage");
            this.menuItemViewArrage.Click += new System.EventHandler(this.menuItemViewArrage_Click);
            // 
            // menuItemOperational
            // 
            this.menuItemOperational.Index = 1;
            resources.ApplyResources(this.menuItemOperational, "menuItemOperational");
            this.menuItemOperational.Click += new System.EventHandler(this.MenuItemOperational_Click);
            // 
            // menuItemTools
            // 
            this.menuItemTools.Index = 5;
            this.menuItemTools.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemDummy});
            resources.ApplyResources(this.menuItemTools, "menuItemTools");
            this.menuItemTools.Popup += new System.EventHandler(this.menuItemTools_Popup);
            // 
            // menuItemDummy
            // 
            this.menuItemDummy.Index = 0;
            resources.ApplyResources(this.menuItemDummy, "menuItemDummy");
            // 
            // menuLayout
            // 
            this.menuLayout.Index = 4;
            this.menuLayout.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemDesign,
            this.menuItemOperational,
            this.menuItemSimulation,
            this.menuItemDesignTimeOnlySeperator,
            this.menuItemCompile,
            this.menuItemVerificationOptions,
            this.menuItem8,
            this.menuItemLearnLayout,
            this.menuItemConnectLayout,
            this.menuItemEmergencyStop,
            this.menuItemSuspendLocomotives,
            this.menuItem6,
            this.menuItemPolicies,
            this.menuItemDefaultDriverParameters,
            this.menuItemTrainTracking,
            this.menuItemCommonDestinations,
            this.menuItemManualDispatchRegions});
            resources.ApplyResources(this.menuLayout, "menuLayout");
            this.menuLayout.Popup += new System.EventHandler(this.menuLayout_Popup);
            // 
            // menuItemDesign
            // 
            this.menuItemDesign.Index = 0;
            resources.ApplyResources(this.menuItemDesign, "menuItemDesign");
            this.menuItemDesign.Click += new System.EventHandler(this.MenuItemDesign_Click);
            // 
            // menuItemSimulation
            // 
            this.menuItemSimulation.Index = 2;
            resources.ApplyResources(this.menuItemSimulation, "menuItemSimulation");
            this.menuItemSimulation.Click += new System.EventHandler(this.menuItemSimulation_Click);
            // 
            // menuItemDesignTimeOnlySeperator
            // 
            this.menuItemDesignTimeOnlySeperator.Index = 3;
            resources.ApplyResources(this.menuItemDesignTimeOnlySeperator, "menuItemDesignTimeOnlySeperator");
            // 
            // menuItemCompile
            // 
            this.menuItemCompile.Index = 4;
            resources.ApplyResources(this.menuItemCompile, "menuItemCompile");
            this.menuItemCompile.Click += new System.EventHandler(this.menuItemCompile_Click);
            // 
            // menuItemVerificationOptions
            // 
            this.menuItemVerificationOptions.Index = 5;
            resources.ApplyResources(this.menuItemVerificationOptions, "menuItemVerificationOptions");
            this.menuItemVerificationOptions.Click += new System.EventHandler(this.menuItemVerificationOptions_Click);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 6;
            resources.ApplyResources(this.menuItem8, "menuItem8");
            // 
            // menuItemLearnLayout
            // 
            this.menuItemLearnLayout.Index = 7;
            resources.ApplyResources(this.menuItemLearnLayout, "menuItemLearnLayout");
            this.menuItemLearnLayout.Click += new System.EventHandler(this.menuItemLearnLayout_Click);
            // 
            // menuItemConnectLayout
            // 
            this.menuItemConnectLayout.Index = 8;
            resources.ApplyResources(this.menuItemConnectLayout, "menuItemConnectLayout");
            this.menuItemConnectLayout.Click += new System.EventHandler(this.menuItemConnectLayout_Click);
            // 
            // menuItemEmergencyStop
            // 
            this.menuItemEmergencyStop.Index = 9;
            resources.ApplyResources(this.menuItemEmergencyStop, "menuItemEmergencyStop");
            this.menuItemEmergencyStop.Click += new System.EventHandler(this.menuItemEmergencyStop_Click);
            // 
            // menuItemSuspendLocomotives
            // 
            this.menuItemSuspendLocomotives.Index = 10;
            resources.ApplyResources(this.menuItemSuspendLocomotives, "menuItemSuspendLocomotives");
            this.menuItemSuspendLocomotives.Click += new System.EventHandler(this.menuItemSuspendLocomotives_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 11;
            resources.ApplyResources(this.menuItem6, "menuItem6");
            // 
            // menuItemPolicies
            // 
            this.menuItemPolicies.Index = 12;
            resources.ApplyResources(this.menuItemPolicies, "menuItemPolicies");
            this.menuItemPolicies.Click += new System.EventHandler(this.MenuItemPolicies_Click);
            // 
            // menuItemDefaultDriverParameters
            // 
            this.menuItemDefaultDriverParameters.Index = 13;
            resources.ApplyResources(this.menuItemDefaultDriverParameters, "menuItemDefaultDriverParameters");
            this.menuItemDefaultDriverParameters.Click += new System.EventHandler(this.MenuItemDefaultDriverParameters_Click);
            // 
            // menuItemCommonDestinations
            // 
            this.menuItemCommonDestinations.Index = 15;
            resources.ApplyResources(this.menuItemCommonDestinations, "menuItemCommonDestinations");
            this.menuItemCommonDestinations.Click += new System.EventHandler(this.MenuItemCommonDestinations_Click);
            // 
            // menuItemManualDispatchRegions
            // 
            this.menuItemManualDispatchRegions.Index = 16;
            this.menuItemManualDispatchRegions.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem4});
            resources.ApplyResources(this.menuItemManualDispatchRegions, "menuItemManualDispatchRegions");
            this.menuItemManualDispatchRegions.Popup += new System.EventHandler(this.MenuItemManualDispatchRegions_Popup);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 0;
            resources.ApplyResources(this.menuItem4, "menuItem4");
            // 
            // menuFile
            // 
            this.menuFile.Index = 0;
            this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemNewWindow,
            this.menuItemSave,
            this.menuItem9,
            this.menuItemPrint,
            this.menuItemSeperator1,
            this.menuItemModules,
            this.menuItem3,
            this.menuItemCloseWindow,
            this.menuItemExit});
            resources.ApplyResources(this.menuFile, "menuFile");
            // 
            // menuItemNewWindow
            // 
            this.menuItemNewWindow.Index = 0;
            resources.ApplyResources(this.menuItemNewWindow, "menuItemNewWindow");
            this.menuItemNewWindow.Click += new System.EventHandler(this.menuItemNewWindow_Click);
            // 
            // menuItemSave
            // 
            this.menuItemSave.Index = 1;
            resources.ApplyResources(this.menuItemSave, "menuItemSave");
            this.menuItemSave.Click += new System.EventHandler(this.menuItemSave_Click);
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 2;
            resources.ApplyResources(this.menuItem9, "menuItem9");
            // 
            // menuItemPrint
            // 
            this.menuItemPrint.Index = 3;
            resources.ApplyResources(this.menuItemPrint, "menuItemPrint");
            this.menuItemPrint.Click += new System.EventHandler(this.menuItemPrint_Click);
            // 
            // menuItemSeperator1
            // 
            this.menuItemSeperator1.Index = 4;
            resources.ApplyResources(this.menuItemSeperator1, "menuItemSeperator1");
            // 
            // menuItemModules
            // 
            this.menuItemModules.Index = 5;
            resources.ApplyResources(this.menuItemModules, "menuItemModules");
            this.menuItemModules.Click += new System.EventHandler(this.menuItemModules_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 6;
            resources.ApplyResources(this.menuItem3, "menuItem3");
            // 
            // menuItemCloseWindow
            // 
            this.menuItemCloseWindow.Index = 7;
            resources.ApplyResources(this.menuItemCloseWindow, "menuItemCloseWindow");
            this.menuItemCloseWindow.Click += new System.EventHandler(this.menuItemCloseWindow_Click);
            // 
            // menuItemExit
            // 
            this.menuItemExit.Index = 8;
            resources.ApplyResources(this.menuItemExit, "menuItemExit");
            this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
            // 
            // menuEdit
            // 
            this.menuEdit.Index = 1;
            this.menuEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemUndo,
            this.menuItemRedo,
            this.menuItemSeparator,
            this.menuItemCopy,
            this.menuItemCut,
            this.menuItemFind,
            this.menuItem1,
            this.menuItemSelect,
            this.menuItem11,
            this.menuItemDefaultPhase,
            this.menuItemSetComponentPhase,
            this.menuItem14,
            this.menuItemStyles,
            this.menuItemLocomotiveCatalog,
            this.menuItemAccelerationProfiles});
            resources.ApplyResources(this.menuEdit, "menuEdit");
            this.menuEdit.Popup += new System.EventHandler(this.menuEdit_Popup);
            // 
            // menuItemUndo
            // 
            this.menuItemUndo.Index = 0;
            resources.ApplyResources(this.menuItemUndo, "menuItemUndo");
            this.menuItemUndo.Click += new System.EventHandler(this.menuItemUndo_Click);
            // 
            // menuItemRedo
            // 
            this.menuItemRedo.Index = 1;
            resources.ApplyResources(this.menuItemRedo, "menuItemRedo");
            this.menuItemRedo.Click += new System.EventHandler(this.menuItemRedo_Click);
            // 
            // menuItemSeparator
            // 
            this.menuItemSeparator.Index = 2;
            resources.ApplyResources(this.menuItemSeparator, "menuItemSeparator");
            // 
            // menuItemCopy
            // 
            this.menuItemCopy.Index = 3;
            resources.ApplyResources(this.menuItemCopy, "menuItemCopy");
            this.menuItemCopy.Click += new System.EventHandler(this.menuItemCopy_Click);
            // 
            // menuItemCut
            // 
            this.menuItemCut.Index = 4;
            resources.ApplyResources(this.menuItemCut, "menuItemCut");
            this.menuItemCut.Click += new System.EventHandler(this.menuItemCut_Click);
            // 
            // menuItemFind
            // 
            this.menuItemFind.Index = 5;
            resources.ApplyResources(this.menuItemFind, "menuItemFind");
            this.menuItemFind.Click += new System.EventHandler(this.menuItemFind_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 6;
            resources.ApplyResources(this.menuItem1, "menuItem1");
            // 
            // menuItemSelect
            // 
            this.menuItemSelect.Index = 7;
            this.menuItemSelect.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemSelectUnconnectedComponents,
            this.menuItemSelectUnlinkedSignals,
            this.menuItemSelectUnlinkedTrackLinks,
            this.menuItemSelectPowerConnectors,
            this.menuItemSelectReverseLoopModules});
            resources.ApplyResources(this.menuItemSelect, "menuItemSelect");
            // 
            // menuItemSelectUnconnectedComponents
            // 
            this.menuItemSelectUnconnectedComponents.Index = 0;
            resources.ApplyResources(this.menuItemSelectUnconnectedComponents, "menuItemSelectUnconnectedComponents");
            this.menuItemSelectUnconnectedComponents.Click += new System.EventHandler(this.menuItemSelectUnconnectedComponents_Click);
            // 
            // menuItemSelectUnlinkedSignals
            // 
            this.menuItemSelectUnlinkedSignals.Index = 1;
            resources.ApplyResources(this.menuItemSelectUnlinkedSignals, "menuItemSelectUnlinkedSignals");
            this.menuItemSelectUnlinkedSignals.Click += new System.EventHandler(this.menuItemSelectUnlinkedSignals_Click);
            // 
            // menuItemSelectUnlinkedTrackLinks
            // 
            this.menuItemSelectUnlinkedTrackLinks.Index = 2;
            resources.ApplyResources(this.menuItemSelectUnlinkedTrackLinks, "menuItemSelectUnlinkedTrackLinks");
            this.menuItemSelectUnlinkedTrackLinks.Click += new System.EventHandler(this.menuItemSelectUnlinkedTrackLinks_Click);
            // 
            // menuItemSelectPowerConnectors
            // 
            this.menuItemSelectPowerConnectors.Index = 3;
            resources.ApplyResources(this.menuItemSelectPowerConnectors, "menuItemSelectPowerConnectors");
            this.menuItemSelectPowerConnectors.Click += new System.EventHandler(this.menuItemSelectPowerConnectors_Click);
            // 
            // menuItemSelectReverseLoopModules
            // 
            this.menuItemSelectReverseLoopModules.Index = 4;
            resources.ApplyResources(this.menuItemSelectReverseLoopModules, "menuItemSelectReverseLoopModules");
            this.menuItemSelectReverseLoopModules.Click += new System.EventHandler(this.menuItemSelectReverseLoopModules_Click);
            // 
            // menuItem11
            // 
            this.menuItem11.Index = 8;
            resources.ApplyResources(this.menuItem11, "menuItem11");
            // 
            // menuItemDefaultPhase
            // 
            this.menuItemDefaultPhase.Index = 9;
            this.menuItemDefaultPhase.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemNewComponentDesignPhase,
            this.menuItemNewComponentConstructionPhase,
            this.menuItemNewComponentOperationalPhase});
            resources.ApplyResources(this.menuItemDefaultPhase, "menuItemDefaultPhase");
            this.menuItemDefaultPhase.Popup += new System.EventHandler(this.menuItemDefaultPhase_Popup);
            // 
            // menuItemNewComponentDesignPhase
            // 
            this.menuItemNewComponentDesignPhase.Index = 0;
            this.menuItemNewComponentDesignPhase.RadioCheck = true;
            resources.ApplyResources(this.menuItemNewComponentDesignPhase, "menuItemNewComponentDesignPhase");
            this.menuItemNewComponentDesignPhase.Click += new System.EventHandler(this.menuItemNewComponentDesignPhase_Click);
            // 
            // menuItemNewComponentConstructionPhase
            // 
            this.menuItemNewComponentConstructionPhase.Index = 1;
            this.menuItemNewComponentConstructionPhase.RadioCheck = true;
            resources.ApplyResources(this.menuItemNewComponentConstructionPhase, "menuItemNewComponentConstructionPhase");
            this.menuItemNewComponentConstructionPhase.Click += new System.EventHandler(this.menuItemNewComponentConstructionPhase_Click);
            // 
            // menuItemNewComponentOperationalPhase
            // 
            this.menuItemNewComponentOperationalPhase.Index = 2;
            this.menuItemNewComponentOperationalPhase.RadioCheck = true;
            resources.ApplyResources(this.menuItemNewComponentOperationalPhase, "menuItemNewComponentOperationalPhase");
            this.menuItemNewComponentOperationalPhase.Click += new System.EventHandler(this.menuItemNewComponentOperationalPhase_Click);
            // 
            // menuItemSetComponentPhase
            // 
            this.menuItemSetComponentPhase.Index = 10;
            this.menuItemSetComponentPhase.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemSetToDesignPhase,
            this.menuItemSetToConstructionPhase,
            this.menuItemSetToOperationalPhase});
            resources.ApplyResources(this.menuItemSetComponentPhase, "menuItemSetComponentPhase");
            // 
            // menuItemSetToDesignPhase
            // 
            this.menuItemSetToDesignPhase.Index = 0;
            resources.ApplyResources(this.menuItemSetToDesignPhase, "menuItemSetToDesignPhase");
            this.menuItemSetToDesignPhase.Click += new System.EventHandler(this.menuItemSetToDesignPhase_Click);
            // 
            // menuItemSetToConstructionPhase
            // 
            this.menuItemSetToConstructionPhase.Index = 1;
            resources.ApplyResources(this.menuItemSetToConstructionPhase, "menuItemSetToConstructionPhase");
            this.menuItemSetToConstructionPhase.Click += new System.EventHandler(this.menuItemSetToConstructionPhase_Click);
            // 
            // menuItemSetToOperationalPhase
            // 
            this.menuItemSetToOperationalPhase.Index = 2;
            resources.ApplyResources(this.menuItemSetToOperationalPhase, "menuItemSetToOperationalPhase");
            this.menuItemSetToOperationalPhase.Click += new System.EventHandler(this.menuItemSetToOperationalPhase_Click);
            // 
            // menuItem14
            // 
            this.menuItem14.Index = 11;
            resources.ApplyResources(this.menuItem14, "menuItem14");
            // 
            // menuItemStyles
            // 
            this.menuItemStyles.Index = 12;
            this.menuItemStyles.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemStyleFonts,
            this.menuItemStylePositions});
            resources.ApplyResources(this.menuItemStyles, "menuItemStyles");
            // 
            // menuItemStyleFonts
            // 
            this.menuItemStyleFonts.Index = 0;
            resources.ApplyResources(this.menuItemStyleFonts, "menuItemStyleFonts");
            this.menuItemStyleFonts.Click += new System.EventHandler(this.menuItemStyleFonts_Click);
            // 
            // menuItemStylePositions
            // 
            this.menuItemStylePositions.Index = 1;
            resources.ApplyResources(this.menuItemStylePositions, "menuItemStylePositions");
            this.menuItemStylePositions.Click += new System.EventHandler(this.menuItemStylePositions_Click);
            // 
            // menuItemLocomotiveCatalog
            // 
            this.menuItemLocomotiveCatalog.Index = 13;
            resources.ApplyResources(this.menuItemLocomotiveCatalog, "menuItemLocomotiveCatalog");
            this.menuItemLocomotiveCatalog.Click += new System.EventHandler(this.MenuItemLocomotiveCatalog_Click);
            // 
            // menuItemAccelerationProfiles
            // 
            this.menuItemAccelerationProfiles.Index = 14;
            resources.ApplyResources(this.menuItemAccelerationProfiles, "menuItemAccelerationProfiles");
            this.menuItemAccelerationProfiles.Click += new System.EventHandler(this.MenuItemAccelerationProfiles_Click);
            // 
            // menuItemArea
            // 
            this.menuItemArea.Index = 3;
            this.menuItemArea.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemNewArea,
            this.menuItemDeleteArea,
            this.menuItemRenameArea,
            this.menuSeperator2,
            this.menuItemAreaArrange});
            resources.ApplyResources(this.menuItemArea, "menuItemArea");
            // 
            // menuItemNewArea
            // 
            this.menuItemNewArea.Index = 0;
            resources.ApplyResources(this.menuItemNewArea, "menuItemNewArea");
            this.menuItemNewArea.Click += new System.EventHandler(this.menuItemNewArea_Click);
            // 
            // menuItemDeleteArea
            // 
            this.menuItemDeleteArea.Index = 1;
            resources.ApplyResources(this.menuItemDeleteArea, "menuItemDeleteArea");
            this.menuItemDeleteArea.Click += new System.EventHandler(this.menuItemDeleteArea_Click);
            // 
            // menuItemRenameArea
            // 
            this.menuItemRenameArea.Index = 2;
            resources.ApplyResources(this.menuItemRenameArea, "menuItemRenameArea");
            this.menuItemRenameArea.Click += new System.EventHandler(this.menuItemRenameArea_Click);
            // 
            // menuSeperator2
            // 
            this.menuSeperator2.Index = 3;
            resources.ApplyResources(this.menuSeperator2, "menuSeperator2");
            // 
            // menuItemAreaArrange
            // 
            this.menuItemAreaArrange.Index = 4;
            resources.ApplyResources(this.menuItemAreaArrange, "menuItemAreaArrange");
            this.menuItemAreaArrange.Click += new System.EventHandler(this.menuItemAreaArrange_Click);
            // 
            // panelMessageViewer
            // 
            this.panelMessageViewer.Controls.Add(this.panelTripsMonitor);
            this.panelMessageViewer.Controls.Add(this.splitterMessages);
            this.panelMessageViewer.Controls.Add(this.messageViewer);
            resources.ApplyResources(this.panelMessageViewer, "panelMessageViewer");
            this.panelMessageViewer.Name = "panelMessageViewer";
            // 
            // panelTripsMonitor
            // 
            this.panelTripsMonitor.Controls.Add(this.splitterTripsMonitor);
            this.panelTripsMonitor.Controls.Add(this.tabAreas);
            this.panelTripsMonitor.Controls.Add(this.tripsMonitor);
            resources.ApplyResources(this.panelTripsMonitor, "panelTripsMonitor");
            this.panelTripsMonitor.Name = "panelTripsMonitor";
            // 
            // splitterTripsMonitor
            // 
            this.splitterTripsMonitor.BackColor = System.Drawing.SystemColors.WindowFrame;
            resources.ApplyResources(this.splitterTripsMonitor, "splitterTripsMonitor");
            this.splitterTripsMonitor.Name = "splitterTripsMonitor";
            this.splitterTripsMonitor.TabStop = false;
            this.splitterTripsMonitor.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.SplitterTripsMonitor_SplitterMoved);
            // 
            // tabAreas
            // 
            resources.ApplyResources(this.tabAreas, "tabAreas");
            this.tabAreas.Multiline = true;
            this.tabAreas.Name = "tabAreas";
            this.tabAreas.SelectedIndex = 0;
            // 
            // tripsMonitor
            // 
            resources.ApplyResources(this.tripsMonitor, "tripsMonitor");
            this.tripsMonitor.Name = "tripsMonitor";
            // 
            // splitterMessages
            // 
            this.splitterMessages.BackColor = System.Drawing.SystemColors.WindowFrame;
            resources.ApplyResources(this.splitterMessages, "splitterMessages");
            this.splitterMessages.Name = "splitterMessages";
            this.splitterMessages.TabStop = false;
            this.splitterMessages.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.SplitterMessages_SplitterMoved);
            // 
            // messageViewer
            // 
            resources.ApplyResources(this.messageViewer, "messageViewer");
            this.messageViewer.Name = "messageViewer";
            // 
            // splitterLocomotives
            // 
            this.splitterLocomotives.BackColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(this.splitterLocomotives, "splitterLocomotives");
            this.splitterLocomotives.Name = "splitterLocomotives";
            this.splitterLocomotives.TabStop = false;
            this.splitterLocomotives.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.SplitterLocomotives_SplitterMoved);
            // 
            // panelLocomotiveViewer
            // 
            this.panelLocomotiveViewer.Controls.Add(this.panelMessageViewer);
            this.panelLocomotiveViewer.Controls.Add(this.splitterLocomotives);
            this.panelLocomotiveViewer.Controls.Add(this.locomotivesViewer);
            resources.ApplyResources(this.panelLocomotiveViewer, "panelLocomotiveViewer");
            this.panelLocomotiveViewer.Name = "panelLocomotiveViewer";
            // 
            // locomotivesViewer
            // 
            resources.ApplyResources(this.locomotivesViewer, "locomotivesViewer");
            this.locomotivesViewer.LocomotiveCollection = null;
            this.locomotivesViewer.Name = "locomotivesViewer";
            // 
            // panelLayoutControl
            // 
            this.panelLayoutControl.Controls.Add(this.panelLocomotiveViewer);
            this.panelLayoutControl.Controls.Add(this.splitterLayoutControl);
            this.panelLayoutControl.Controls.Add(this.layoutControlViewer);
            resources.ApplyResources(this.panelLayoutControl, "panelLayoutControl");
            this.panelLayoutControl.Name = "panelLayoutControl";
            // 
            // splitterLayoutControl
            // 
            this.splitterLayoutControl.BackColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.splitterLayoutControl, "splitterLayoutControl");
            this.splitterLayoutControl.Name = "splitterLayoutControl";
            this.splitterLayoutControl.TabStop = false;
            // 
            // layoutControlViewer
            // 
            resources.ApplyResources(this.layoutControlViewer, "layoutControlViewer");
            this.layoutControlViewer.Name = "layoutControlViewer";
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuFile,
            this.menuEdit,
            this.menuItemView,
            this.menuItemArea,
            this.menuLayout,
            this.menuItemTools});
            // 
            // timerFreeResources
            // 
            this.timerFreeResources.Interval = 30000D;
            this.timerFreeResources.SynchronizingObject = this;
            this.timerFreeResources.Elapsed += new System.Timers.ElapsedEventHandler(this.TimerFreeResources_Elapsed);
            // 
            // timerDetailsPopup
            // 
            this.timerDetailsPopup.Interval = 750;
            this.timerDetailsPopup.Tick += new System.EventHandler(this.timerDetailsPopup_Tick);
            // 
            // printDocument
            // 
            this.printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument_PrintPage);
            // 
            // menuItem13
            // 
            this.menuItem13.Index = -1;
            resources.ApplyResources(this.menuItem13, "menuItem13");
            // 
            // menuItemTrainTracking
            // 
            this.menuItemTrainTracking.Index = 14;
            resources.ApplyResources(this.menuItemTrainTracking, "menuItemTrainTracking");
            this.menuItemTrainTracking.Click += new System.EventHandler(this.menuItemTrainTracking_Click);
            // 
            // LayoutFrameWindow
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panelLayoutControl);
            this.Controls.Add(this.toolBarMain);
            this.Controls.Add(this.statusBar);
            this.Menu = this.mainMenu;
            this.Name = "LayoutFrameWindow";
            this.Activated += new System.EventHandler(this.LayoutController_Activated);
            this.Deactivate += new System.EventHandler(this.LayoutController_Deactivate);
            this.Resize += new System.EventHandler(this.LayoutController_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanelOperation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanelMode)).EndInit();
            this.panelMessageViewer.ResumeLayout(false);
            this.panelTripsMonitor.ResumeLayout(false);
            this.panelLocomotiveViewer.ResumeLayout(false);
            this.panelLayoutControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.timerFreeResources)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.ToolBar toolBarMain;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItemView;
		private System.Windows.Forms.MenuItem menuItemViewNew;
		private System.Windows.Forms.MenuItem menuItemViewDelete;
		private System.Windows.Forms.MenuItem menuItemViewRename;
		private System.Windows.Forms.MenuItem menuItemSeperator6;
		private System.Windows.Forms.MenuItem menuItemZoomMenu;
		private System.Windows.Forms.MenuItem menuItemSetZoom;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItemZoom200;
		private System.Windows.Forms.MenuItem menuItemZoom150;
		private System.Windows.Forms.MenuItem menuItemZoom100;
		private System.Windows.Forms.MenuItem menuItemZoom75;
		private System.Windows.Forms.MenuItem menuItemZoom50;
		private System.Windows.Forms.MenuItem menuItemZoom25;
		private System.Windows.Forms.MenuItem menuItem12;
		private System.Windows.Forms.MenuItem menuItem1ZoomAllArea;
		private System.Windows.Forms.MenuItem menuItemViewGrid;
		private System.Windows.Forms.MenuItem menuItemShowMessages;
		private System.Windows.Forms.MenuItem menuItemShowLocomotives;
		private System.Windows.Forms.MenuItem menuSeperator4;
		private System.Windows.Forms.MenuItem menuItemViewArrage;
		private System.Windows.Forms.MenuItem menuItemOperational;
		private System.Windows.Forms.MenuItem menuItemTools;
		private System.Windows.Forms.MenuItem menuItemDummy;
		private System.Windows.Forms.MenuItem menuLayout;
		private System.Windows.Forms.MenuItem menuItemDesign;
		private System.Windows.Forms.MenuItem menuItemCompile;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuFile;
		private System.Windows.Forms.MenuItem menuItemSave;
		private System.Windows.Forms.MenuItem menuItemSeperator1;
		private System.Windows.Forms.MenuItem menuItemModules;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItemExit;
		private System.Windows.Forms.MenuItem menuEdit;
		private System.Windows.Forms.MenuItem menuItemUndo;
		private System.Windows.Forms.MenuItem menuItemRedo;
		private System.Windows.Forms.MenuItem menuItemSeparator;
		private System.Windows.Forms.MenuItem menuItemCopy;
		private System.Windows.Forms.MenuItem menuItemCut;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItemStyles;
		private System.Windows.Forms.MenuItem menuItemStyleFonts;
		private System.Windows.Forms.MenuItem menuItemStylePositions;
		private System.Windows.Forms.MenuItem menuItemLocomotiveCatalog;
		private System.Windows.Forms.MenuItem menuItemArea;
		private System.Windows.Forms.MenuItem menuItemNewArea;
		private System.Windows.Forms.MenuItem menuItemDeleteArea;
		private System.Windows.Forms.MenuItem menuItemRenameArea;
		private System.Windows.Forms.MenuItem menuSeperator2;
		private System.Windows.Forms.MenuItem menuItemAreaArrange;
		private System.Windows.Forms.StatusBarPanel statusBarPanelMode;
		private System.Windows.Forms.StatusBarPanel statusBarPanelOperation;
		private System.Windows.Forms.ImageList imageListTools;
		private System.Windows.Forms.Splitter splitterLocomotives;
		private System.Windows.Forms.TabControl tabAreas;
		private System.Windows.Forms.Splitter splitterMessages;
		private LayoutManager.MessageViewer messageViewer;
		private System.Windows.Forms.Splitter splitterTripsMonitor;
		private LayoutManager.Tools.Controls.TripsMonitor tripsMonitor;
		private System.Windows.Forms.Splitter splitterLayoutControl;
		private LayoutManager.LocomotivesViewer locomotivesViewer;
		private System.Windows.Forms.ToolBarButton toolBarButtonOpenLayiout;
		private System.Windows.Forms.ToolBarButton toolBarButtonSaveLayout;
		private System.Windows.Forms.ToolBarButton toolBarButtonShowLocomotives;
		private System.Windows.Forms.ToolBarButton toolBarButtonShowMessages;
		private System.Windows.Forms.ToolBarButton Sep1;
		private System.Windows.Forms.ToolBarButton toolBarButtonDesignMode;
		private System.Windows.Forms.ToolBarButton toolBarButtonOperationMode;
		private System.Windows.Forms.ToolBarButton sep2;
		private System.Windows.Forms.MenuItem menuItemManualDispatchRegions;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.Panel panelMessageViewer;
		private System.Windows.Forms.Panel panelLocomotiveViewer;
		private System.Windows.Forms.Panel panelTripsMonitor;
		private System.Windows.Forms.Panel panelLayoutControl;
		private LayoutManager.CommonUI.Controls.LayoutControlViewer layoutControlViewer;
		private System.Windows.Forms.ToolBarButton toolBarButtonShowTripsMonitor;
		private System.Windows.Forms.MenuItem menuItemShowTripsMonitor;
		private System.Windows.Forms.MenuItem menuItemPolicies;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItemAccelerationProfiles;
		private System.Windows.Forms.MenuItem menuItemDefaultDriverParameters;
		private System.Windows.Forms.MenuItem menuItemCommonDestinations;
		private System.Windows.Forms.ToolBarButton sep3;
		private System.Windows.Forms.ToolBarButton toolBarButtonToggleAllLayoutManualDispatch;
		private System.Windows.Forms.MenuItem menuItemEmergencyStop;
		private System.Windows.Forms.MenuItem menuItemSuspendLocomotives;
		private System.Windows.Forms.ToolBarButton sep4;
		private System.Windows.Forms.ToolBarButton toolBarButtonStopAllLocomotives;
		private System.Windows.Forms.ToolBarButton toolBarButtonEmergencyStop;
		private System.Windows.Forms.ToolBarButton toolBarButtonShowLayoutControl;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuItemShowLayoutControl;
		private System.Windows.Forms.MenuItem menuItemConnectLayout;
		private MenuItem menuItem8;
		private MenuItem menuItemLearnLayout;
		private MenuItem menuItemDesignTimeOnlySeperator;
		private ToolBarButton toolBarButtonZoom100;
		private ToolBarButton sep5;
		private ToolBarButton toolBarButtonZoomIn;
		private ToolBarButton toolBarButtonZoomOut;
		private ToolBarButton toolBarButtonZoomFitAll;
		private ToolBarButton toolBarButtonToggleGrid;
		private MenuItem menuItemFind;
		private MenuItem menuItemPrint;
		private MenuItem menuItem9;
		private System.Windows.Forms.StatusBar statusBar;
		private MenuItem menuItemRestoreDefaultZoomAndOrigin;
		private MenuItem menuItemSaveAsDefaultZoomAndOrigin;
		private MenuItem menuItem10;
		private ToolBarButton toolBarButtonSetToDefaultOriginAndZoom;
		private ToolBarButton toolbarButtonSetAsDefaultOriginAndZoom;
		private ToolBarButton set6;
		private MenuItem menuItemSelect;
		private MenuItem menuItemSelectUnconnectedComponents;
		private MenuItem menuItemSelectUnlinkedSignals;
		private MenuItem menuItem11;
		private MenuItem menuItemSelectUnlinkedTrackLinks;
		private MenuItem menuItem13;
		private MenuItem menuItemDefaultPhase;
		private MenuItem menuItemNewComponentDesignPhase;
		private MenuItem menuItemNewComponentConstructionPhase;
		private MenuItem menuItemNewComponentOperationalPhase;
		private MenuItem menuItemSetComponentPhase;
		private MenuItem menuItemSetToDesignPhase;
		private MenuItem menuItemSetToConstructionPhase;
		private MenuItem menuItemSetToOperationalPhase;
		private MenuItem menuItem14;
		private MenuItem menuItemSimulation;
		private ToolBarButton toolBarButtonSimulation;
		private MenuItem menuItemViewActivePhases;
		private Timer timerDetailsPopup;
		private System.Drawing.Printing.PrintDocument printDocument;
		private System.Timers.Timer timerFreeResources;
		private MenuItem menuItemNewWindow;
		private MenuItem menuItemCloseWindow;
		private MenuItem menuItemSelectPowerConnectors;
		private MenuItem menuItemSelectReverseLoopModules;
        private MenuItem menuItemVerificationOptions;
        private MenuItem menuItemTrainTracking;
    }
}
