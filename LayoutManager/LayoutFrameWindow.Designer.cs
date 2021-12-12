using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Collections;

namespace LayoutManager {
	public partial class LayoutFrameWindow {
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
            this.menuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemView = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemViewNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemViewDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemViewRename = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemZoomMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSetZoom = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSeperator5 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemZoom200 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemZoom150 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemZoom100 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemZoom75 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemZoom50 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemZoom25 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSeperator12 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItem1ZoomAllArea = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemViewGrid = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemViewActivePhases = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSeperator7 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemRestoreDefaultZoomAndOrigin = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSaveAsDefaultZoomAndOrigin = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSeperator10 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemShowLayoutControl = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemShowTripsMonitor = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemShowMessages = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemShowLocomotives = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSeperator4 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemViewArrage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSeperator6 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemOperational = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemTools = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemDummy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLayout = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemDesign = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSimulation = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemDesignTimeOnlySeperator = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemCompile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemVerificationOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSeperator8 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemLearnLayout = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemConnectLayout = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEmergencyStop = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSuspendLocomotives = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemPolicies = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemDefaultDriverParameters = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemTrainTracking = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemCommonDestinations = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemManualDispatchRegions = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemNewWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSeperator9 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemPrint = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemModules = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSeperator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemCloseWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSeperator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemCut = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFind = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSelectUnconnectedComponents = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSelectUnlinkedSignals = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSelectUnlinkedTrackLinks = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSelectPowerConnectors = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSelectReverseLoopModules = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSeperator11 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemDefaultPhase = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemNewComponentDesignPhase = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemNewComponentConstructionPhase = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemNewComponentOperationalPhase = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSetComponentPhase = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSetToDesignPhase = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSetToConstructionPhase = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSetToOperationalPhase = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSeperator14 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemStyles = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemStyleFonts = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemStylePositions = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemLocomotiveCatalog = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAccelerationProfiles = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemArea = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemNewArea = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemDeleteArea = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemRenameArea = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSeperator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemAreaArrange = new System.Windows.Forms.ToolStripMenuItem();
            this.panelMessageViewer = new System.Windows.Forms.Panel();
            this.panelTripsMonitor = new System.Windows.Forms.Panel();
            this.tabAreas = new System.Windows.Forms.TabControl();
            this.splitterTripsMonitor = new System.Windows.Forms.Splitter();
            this.tripsMonitor = new LayoutManager.Tools.Controls.TripsMonitor();
            this.splitterMessages = new System.Windows.Forms.Splitter();
            this.messageViewer = new LayoutManager.MessageViewer();
            this.splitterLocomotives = new System.Windows.Forms.Splitter();
            this.panelLocomotiveViewer = new System.Windows.Forms.Panel();
            this.locomotivesViewer = new LayoutManager.LocomotivesViewer();
            this.panelLayoutControl = new System.Windows.Forms.Panel();
            this.layoutControlViewer = new LayoutManager.CommonUI.Controls.LayoutControlViewer();
            this.splitterLayoutControl = new System.Windows.Forms.Splitter();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.timerFreeResources = new System.Timers.Timer();
            this.timerDetailsPopup = new System.Windows.Forms.Timer(this.components);
            this.printDocument = new System.Drawing.Printing.PrintDocument();
            this.menuItem13 = new System.Windows.Forms.ToolStripMenuItem();
            this.panelMessageViewer.SuspendLayout();
            this.panelTripsMonitor.SuspendLayout();
            this.panelLocomotiveViewer.SuspendLayout();
            this.panelLayoutControl.SuspendLayout();
            this.mainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timerFreeResources)).BeginInit();
            this.SuspendLayout();
            // 
            // menuItem2
            // 
            this.menuItem2.Name = "menuItem2";
            resources.ApplyResources(this.menuItem2, "menuItem2");
            // 
            // menuItemView
            // 
            this.menuItemView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemViewNew,
            this.menuItemViewDelete,
            this.menuItemViewRename,
            this.menuItemZoomMenu,
            this.menuItemViewGrid,
            this.menuItemViewActivePhases,
            this.menuItemSeperator7,
            this.menuItemRestoreDefaultZoomAndOrigin,
            this.menuItemSaveAsDefaultZoomAndOrigin,
            this.menuItemSeperator10,
            this.menuItemShowLayoutControl,
            this.menuItemShowTripsMonitor,
            this.menuItemShowMessages,
            this.menuItemShowLocomotives,
            this.menuItemSeperator4,
            this.menuItemViewArrage});
            this.menuItemView.Name = "menuItemView";
            resources.ApplyResources(this.menuItemView, "menuItemView");
            this.menuItemView.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.MenuItemView_Popup);
            // 
            // menuItemViewNew
            // 
            this.menuItemViewNew.Name = "menuItemViewNew";
            resources.ApplyResources(this.menuItemViewNew, "menuItemViewNew");
            this.menuItemViewNew.Click += new System.EventHandler(this.MenuItemViewNew_Click);
            // 
            // menuItemViewDelete
            // 
            this.menuItemViewDelete.Name = "menuItemViewDelete";
            resources.ApplyResources(this.menuItemViewDelete, "menuItemViewDelete");
            this.menuItemViewDelete.Click += new System.EventHandler(this.MenuItemViewDelete_Click);
            // 
            // menuItemViewRename
            // 
            this.menuItemViewRename.Name = "menuItemViewRename";
            resources.ApplyResources(this.menuItemViewRename, "menuItemViewRename");
            this.menuItemViewRename.Click += new System.EventHandler(this.MenuItemViewRename_Click);
            // 
            // menuItemZoomMenu
            // 
            this.menuItemZoomMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemSetZoom,
            this.menuItemSeperator5,
            this.menuItemZoom200,
            this.menuItemZoom150,
            this.menuItemZoom100,
            this.menuItemZoom75,
            this.menuItemZoom50,
            this.menuItemZoom25,
            this.menuItemSeperator12,
            this.menuItem1ZoomAllArea});
            this.menuItemZoomMenu.Name = "menuItemZoomMenu";
            resources.ApplyResources(this.menuItemZoomMenu, "menuItemZoomMenu");
            this.menuItemZoomMenu.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.MenuItemZoomMenu_Popup);
            // 
            // menuItemSetZoom
            // 
            this.menuItemSetZoom.Name = "menuItemSetZoom";
            resources.ApplyResources(this.menuItemSetZoom, "menuItemSetZoom");
            this.menuItemSetZoom.Click += new System.EventHandler(this.MenuItemSetZoom_Click);
            // 
            // menuItemSeperator5
            // 
            this.menuItemSeperator5.Name = "menuItemSeperator5";
            resources.ApplyResources(this.menuItemSeperator5, "menuItemSeperator5");
            // 
            // menuItemZoom200
            // 
            this.menuItemZoom200.Name = "menuItemZoom200";
            resources.ApplyResources(this.menuItemZoom200, "menuItemZoom200");
            this.menuItemZoom200.Click += new System.EventHandler(this.MenuItemZoomPreset_Click);
            // 
            // menuItemZoom150
            // 
            this.menuItemZoom150.Name = "menuItemZoom150";
            resources.ApplyResources(this.menuItemZoom150, "menuItemZoom150");
            this.menuItemZoom150.Click += new System.EventHandler(this.MenuItemZoomPreset_Click);
            // 
            // menuItemZoom100
            // 
            this.menuItemZoom100.Name = "menuItemZoom100";
            resources.ApplyResources(this.menuItemZoom100, "menuItemZoom100");
            this.menuItemZoom100.Click += new System.EventHandler(this.MenuItemZoomPreset_Click);
            // 
            // menuItemZoom75
            // 
            this.menuItemZoom75.Name = "menuItemZoom75";
            resources.ApplyResources(this.menuItemZoom75, "menuItemZoom75");
            this.menuItemZoom75.Click += new System.EventHandler(this.MenuItemZoomPreset_Click);
            // 
            // menuItemZoom50
            // 
            this.menuItemZoom50.Name = "menuItemZoom50";
            resources.ApplyResources(this.menuItemZoom50, "menuItemZoom50");
            this.menuItemZoom50.Click += new System.EventHandler(this.MenuItemZoomPreset_Click);
            // 
            // menuItemZoom25
            // 
            this.menuItemZoom25.Name = "menuItemZoom25";
            resources.ApplyResources(this.menuItemZoom25, "menuItemZoom25");
            this.menuItemZoom25.Click += new System.EventHandler(this.MenuItemZoomPreset_Click);
            // 
            // menuItemSeperator12
            // 
            this.menuItemSeperator12.Name = "menuItemSeperator12";
            resources.ApplyResources(this.menuItemSeperator12, "menuItemSeperator12");
            // 
            // menuItem1ZoomAllArea
            // 
            this.menuItem1ZoomAllArea.Name = "menuItem1ZoomAllArea";
            resources.ApplyResources(this.menuItem1ZoomAllArea, "menuItem1ZoomAllArea");
            this.menuItem1ZoomAllArea.Click += new System.EventHandler(this.MenuItemZoomAllArea_Click);
            // 
            // menuItemViewGrid
            // 
            this.menuItemViewGrid.Name = "menuItemViewGrid";
            resources.ApplyResources(this.menuItemViewGrid, "menuItemViewGrid");
            this.menuItemViewGrid.Click += new System.EventHandler(this.MenuItemViewGrid_Click);
            // 
            // menuItemViewActivePhases
            // 
            this.menuItemViewActivePhases.Name = "menuItemViewActivePhases";
            resources.ApplyResources(this.menuItemViewActivePhases, "menuItemViewActivePhases");
            this.menuItemViewActivePhases.Click += new System.EventHandler(this.MenuItemViewActivePhases_Click);
            // 
            // menuItemSeperator7
            // 
            this.menuItemSeperator7.Name = "menuItemSeperator7";
            resources.ApplyResources(this.menuItemSeperator7, "menuItemSeperator7");
            // 
            // menuItemRestoreDefaultZoomAndOrigin
            // 
            this.menuItemRestoreDefaultZoomAndOrigin.Name = "menuItemRestoreDefaultZoomAndOrigin";
            resources.ApplyResources(this.menuItemRestoreDefaultZoomAndOrigin, "menuItemRestoreDefaultZoomAndOrigin");
            this.menuItemRestoreDefaultZoomAndOrigin.Click += new System.EventHandler(this.MenuItemRestoreDefaultZoomAndOrigin_Click);
            // 
            // menuItemSaveAsDefaultZoomAndOrigin
            // 
            this.menuItemSaveAsDefaultZoomAndOrigin.Name = "menuItemSaveAsDefaultZoomAndOrigin";
            resources.ApplyResources(this.menuItemSaveAsDefaultZoomAndOrigin, "menuItemSaveAsDefaultZoomAndOrigin");
            this.menuItemSaveAsDefaultZoomAndOrigin.Click += new System.EventHandler(this.MenuItemSaveAsDefaultZoomAndOrigin_Click);
            // 
            // menuItemSeperator10
            // 
            this.menuItemSeperator10.Name = "menuItemSeperator10";
            resources.ApplyResources(this.menuItemSeperator10, "menuItemSeperator10");
            // 
            // menuItemShowLayoutControl
            // 
            this.menuItemShowLayoutControl.Name = "menuItemShowLayoutControl";
            resources.ApplyResources(this.menuItemShowLayoutControl, "menuItemShowLayoutControl");
            this.menuItemShowLayoutControl.Click += new System.EventHandler(this.MenuItemShowLayoutControl_Click);
            // 
            // menuItemShowTripsMonitor
            // 
            this.menuItemShowTripsMonitor.Name = "menuItemShowTripsMonitor";
            resources.ApplyResources(this.menuItemShowTripsMonitor, "menuItemShowTripsMonitor");
            this.menuItemShowTripsMonitor.Click += new System.EventHandler(this.MenuItemShowTripsMonitor_Click);
            // 
            // menuItemShowMessages
            // 
            this.menuItemShowMessages.Name = "menuItemShowMessages";
            resources.ApplyResources(this.menuItemShowMessages, "menuItemShowMessages");
            this.menuItemShowMessages.Click += new System.EventHandler(this.MenuItemShowMessages_Click);
            // 
            // menuItemShowLocomotives
            // 
            this.menuItemShowLocomotives.Name = "menuItemShowLocomotives";
            resources.ApplyResources(this.menuItemShowLocomotives, "menuItemShowLocomotives");
            this.menuItemShowLocomotives.Click += new System.EventHandler(this.MenuItemShowLocomotives_Click);
            // 
            // menuItemSeperator4
            // 
            this.menuItemSeperator4.Name = "menuItemSeperator4";
            resources.ApplyResources(this.menuItemSeperator4, "menuItemSeperator4");
            // 
            // menuItemViewArrage
            // 
            this.menuItemViewArrage.Name = "menuItemViewArrage";
            resources.ApplyResources(this.menuItemViewArrage, "menuItemViewArrage");
            this.menuItemViewArrage.Click += new System.EventHandler(this.MenuItemViewArrage_Click);
            // 
            // menuItemSeperator6
            // 
            this.menuItemSeperator6.Name = "menuItemSeperator6";
            resources.ApplyResources(this.menuItemSeperator6, "menuItemSeperator6");
            // 
            // menuItemOperational
            // 
            this.menuItemOperational.Name = "menuItemOperational";
            resources.ApplyResources(this.menuItemOperational, "menuItemOperational");
            this.menuItemOperational.Click += new System.EventHandler(this.MenuItemOperational_Click);
            // 
            // menuItemTools
            // 
            this.menuItemTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemDummy});
            this.menuItemTools.Name = "menuItemTools";
            resources.ApplyResources(this.menuItemTools, "menuItemTools");
            this.menuItemTools.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.MenuItemTools_Popup);
            // 
            // menuItemDummy
            // 
            this.menuItemDummy.Name = "menuItemDummy";
            resources.ApplyResources(this.menuItemDummy, "menuItemDummy");
            // 
            // menuLayout
            // 
            this.menuLayout.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemDesign,
            this.menuItemOperational,
            this.menuItemSimulation,
            this.menuItemDesignTimeOnlySeperator,
            this.menuItemCompile,
            this.menuItemVerificationOptions,
            this.menuItemSeperator8,
            this.menuItemLearnLayout,
            this.menuItemConnectLayout,
            this.menuItemEmergencyStop,
            this.menuItemSuspendLocomotives,
            this.menuItemSeperator6,
            this.menuItemPolicies,
            this.menuItemDefaultDriverParameters,
            this.menuItemTrainTracking,
            this.menuItemCommonDestinations,
            this.menuItemManualDispatchRegions});
            this.menuLayout.Name = "menuLayout";
            resources.ApplyResources(this.menuLayout, "menuLayout");
            this.menuLayout.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.MenuLayout_Popup);
            // 
            // menuItemDesign
            // 
            this.menuItemDesign.Name = "menuItemDesign";
            resources.ApplyResources(this.menuItemDesign, "menuItemDesign");
            this.menuItemDesign.Click += new System.EventHandler(this.MenuItemDesign_Click);
            // 
            // menuItemSimulation
            // 
            this.menuItemSimulation.Name = "menuItemSimulation";
            resources.ApplyResources(this.menuItemSimulation, "menuItemSimulation");
            this.menuItemSimulation.Click += new System.EventHandler(this.MenuItemSimulation_Click);
            // 
            // menuItemDesignTimeOnlySeperator
            // 
            this.menuItemDesignTimeOnlySeperator.Name = "menuItemDesignTimeOnlySeperator";
            resources.ApplyResources(this.menuItemDesignTimeOnlySeperator, "menuItemDesignTimeOnlySeperator");
            // 
            // menuItemCompile
            // 
            this.menuItemCompile.Name = "menuItemCompile";
            resources.ApplyResources(this.menuItemCompile, "menuItemCompile");
            this.menuItemCompile.Click += new System.EventHandler(this.MenuItemCompile_Click);
            // 
            // menuItemVerificationOptions
            // 
            this.menuItemVerificationOptions.Name = "menuItemVerificationOptions";
            resources.ApplyResources(this.menuItemVerificationOptions, "menuItemVerificationOptions");
            this.menuItemVerificationOptions.Click += new System.EventHandler(this.MenuItemVerificationOptions_Click);
            // 
            // menuItemSeperator8
            // 
            this.menuItemSeperator8.Name = "menuItemSeperator8";
            resources.ApplyResources(this.menuItemSeperator8, "menuItemSeperator8");
            // 
            // menuItemLearnLayout
            // 
            this.menuItemLearnLayout.Name = "menuItemLearnLayout";
            resources.ApplyResources(this.menuItemLearnLayout, "menuItemLearnLayout");
            this.menuItemLearnLayout.Click += new System.EventHandler(this.MenuItemLearnLayout_Click);
            // 
            // menuItemConnectLayout
            // 
            this.menuItemConnectLayout.Name = "menuItemConnectLayout";
            resources.ApplyResources(this.menuItemConnectLayout, "menuItemConnectLayout");
            this.menuItemConnectLayout.Click += new System.EventHandler(this.MenuItemConnectLayout_Click);
            // 
            // menuItemEmergencyStop
            // 
            this.menuItemEmergencyStop.Name = "menuItemEmergencyStop";
            resources.ApplyResources(this.menuItemEmergencyStop, "menuItemEmergencyStop");
            this.menuItemEmergencyStop.Click += new System.EventHandler(this.MenuItemEmergencyStop_Click);
            // 
            // menuItemSuspendLocomotives
            // 
            this.menuItemSuspendLocomotives.Name = "menuItemSuspendLocomotives";
            resources.ApplyResources(this.menuItemSuspendLocomotives, "menuItemSuspendLocomotives");
            this.menuItemSuspendLocomotives.Click += new System.EventHandler(this.MenuItemSuspendLocomotives_Click);
            // 
            // menuItemPolicies
            // 
            this.menuItemPolicies.Name = "menuItemPolicies";
            resources.ApplyResources(this.menuItemPolicies, "menuItemPolicies");
            this.menuItemPolicies.Click += new System.EventHandler(this.MenuItemPolicies_Click);
            // 
            // menuItemDefaultDriverParameters
            // 
            this.menuItemDefaultDriverParameters.Name = "menuItemDefaultDriverParameters";
            resources.ApplyResources(this.menuItemDefaultDriverParameters, "menuItemDefaultDriverParameters");
            this.menuItemDefaultDriverParameters.Click += new System.EventHandler(this.MenuItemDefaultDriverParameters_Click);
            // 
            // menuItemTrainTracking
            // 
            this.menuItemTrainTracking.Name = "menuItemTrainTracking";
            resources.ApplyResources(this.menuItemTrainTracking, "menuItemTrainTracking");
            this.menuItemTrainTracking.Click += new System.EventHandler(this.MenuItemTrainTracking_Click);
            // 
            // menuItemCommonDestinations
            // 
            this.menuItemCommonDestinations.Name = "menuItemCommonDestinations";
            resources.ApplyResources(this.menuItemCommonDestinations, "menuItemCommonDestinations");
            this.menuItemCommonDestinations.Click += new System.EventHandler(this.MenuItemCommonDestinations_Click);
            // 
            // menuItemManualDispatchRegions
            // 
            this.menuItemManualDispatchRegions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem4});
            this.menuItemManualDispatchRegions.Name = "menuItemManualDispatchRegions";
            resources.ApplyResources(this.menuItemManualDispatchRegions, "menuItemManualDispatchRegions");
            this.menuItemManualDispatchRegions.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.MenuItemManualDispatchRegions_Popup);
            // 
            // menuItem4
            // 
            this.menuItem4.Name = "menuItem4";
            resources.ApplyResources(this.menuItem4, "menuItem4");
            // 
            // menuItem6
            // 
            this.menuItem6.Name = "menuItem6";
            resources.ApplyResources(this.menuItem6, "menuItem6");
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemNewWindow,
            this.menuItemSave,
            this.menuItemSeperator9,
            this.menuItemPrint,
            this.menuItemModules,
            this.menuItemSeperator3,
            this.menuItemCloseWindow,
            this.menuItemExit});
            this.menuFile.Name = "menuFile";
            resources.ApplyResources(this.menuFile, "menuFile");
            // 
            // menuItemNewWindow
            // 
            this.menuItemNewWindow.Name = "menuItemNewWindow";
            resources.ApplyResources(this.menuItemNewWindow, "menuItemNewWindow");
            this.menuItemNewWindow.Click += new System.EventHandler(this.MenuItemNewWindow_Click);
            // 
            // menuItemSave
            // 
            this.menuItemSave.Name = "menuItemSave";
            resources.ApplyResources(this.menuItemSave, "menuItemSave");
            this.menuItemSave.Click += new System.EventHandler(this.MenuItemSave_Click);
            // 
            // menuItemSeperator9
            // 
            this.menuItemSeperator9.Name = "menuItemSeperator9";
            resources.ApplyResources(this.menuItemSeperator9, "menuItemSeperator9");
            // 
            // menuItemPrint
            // 
            this.menuItemPrint.Name = "menuItemPrint";
            resources.ApplyResources(this.menuItemPrint, "menuItemPrint");
            this.menuItemPrint.Click += new System.EventHandler(this.MenuItemPrint_Click);
            // 
            // menuItemModules
            // 
            this.menuItemModules.Name = "menuItemModules";
            resources.ApplyResources(this.menuItemModules, "menuItemModules");
            this.menuItemModules.Click += new System.EventHandler(this.MenuItemModules_Click);
            // 
            // menuItemSeperator3
            // 
            this.menuItemSeperator3.Name = "menuItemSeperator3";
            resources.ApplyResources(this.menuItemSeperator3, "menuItemSeperator3");
            // 
            // menuItemCloseWindow
            // 
            this.menuItemCloseWindow.Name = "menuItemCloseWindow";
            resources.ApplyResources(this.menuItemCloseWindow, "menuItemCloseWindow");
            this.menuItemCloseWindow.Click += new System.EventHandler(this.MenuItemCloseWindow_Click);
            // 
            // menuItemExit
            // 
            this.menuItemExit.Name = "menuItemExit";
            resources.ApplyResources(this.menuItemExit, "menuItemExit");
            this.menuItemExit.Click += new System.EventHandler(this.MenuItemExit_Click);
            // 
            // menuItemSeperator1
            // 
            this.menuItemSeperator1.Name = "menuItemSeperator1";
            resources.ApplyResources(this.menuItemSeperator1, "menuItemSeperator1");
            // 
            // menuEdit
            // 
            this.menuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemUndo,
            this.menuItemRedo,
            this.menuItemSeparator,
            this.menuItemCopy,
            this.menuItemCut,
            this.menuItemFind,
            this.menuItemSeperator1,
            this.menuItemSelect,
            this.menuItemSeperator11,
            this.menuItemDefaultPhase,
            this.menuItemSetComponentPhase,
            this.menuItemSeperator14,
            this.menuItemStyles,
            this.menuItemLocomotiveCatalog,
            this.menuItemAccelerationProfiles});
            this.menuEdit.Name = "menuEdit";
            resources.ApplyResources(this.menuEdit, "menuEdit");
            this.menuEdit.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.MenuEdit_Popup);
            // 
            // menuItemUndo
            // 
            this.menuItemUndo.Name = "menuItemUndo";
            resources.ApplyResources(this.menuItemUndo, "menuItemUndo");
            this.menuItemUndo.Click += new System.EventHandler(this.MenuItemUndo_Click);
            // 
            // menuItemRedo
            // 
            this.menuItemRedo.Name = "menuItemRedo";
            resources.ApplyResources(this.menuItemRedo, "menuItemRedo");
            this.menuItemRedo.Click += new System.EventHandler(this.MenuItemRedo_Click);
            // 
            // menuItemSeparator
            // 
            this.menuItemSeparator.Name = "menuItemSeparator";
            resources.ApplyResources(this.menuItemSeparator, "menuItemSeparator");
            // 
            // menuItemCopy
            // 
            this.menuItemCopy.Name = "menuItemCopy";
            resources.ApplyResources(this.menuItemCopy, "menuItemCopy");
            this.menuItemCopy.Click += new System.EventHandler(this.MenuItemCopy_Click);
            // 
            // menuItemCut
            // 
            this.menuItemCut.Name = "menuItemCut";
            resources.ApplyResources(this.menuItemCut, "menuItemCut");
            this.menuItemCut.Click += new System.EventHandler(this.MenuItemCut_Click);
            // 
            // menuItemFind
            // 
            this.menuItemFind.Name = "menuItemFind";
            resources.ApplyResources(this.menuItemFind, "menuItemFind");
            this.menuItemFind.Click += new System.EventHandler(this.MenuItemFind_Click);
            // 
            // menuItemSelect
            // 
            this.menuItemSelect.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemSelectUnconnectedComponents,
            this.menuItemSelectUnlinkedSignals,
            this.menuItemSelectUnlinkedTrackLinks,
            this.menuItemSelectPowerConnectors,
            this.menuItemSelectReverseLoopModules});
            this.menuItemSelect.Name = "menuItemSelect";
            resources.ApplyResources(this.menuItemSelect, "menuItemSelect");
            // 
            // menuItemSelectUnconnectedComponents
            // 
            this.menuItemSelectUnconnectedComponents.Name = "menuItemSelectUnconnectedComponents";
            resources.ApplyResources(this.menuItemSelectUnconnectedComponents, "menuItemSelectUnconnectedComponents");
            this.menuItemSelectUnconnectedComponents.Click += new System.EventHandler(this.MenuItemSelectUnconnectedComponents_Click);
            // 
            // menuItemSelectUnlinkedSignals
            // 
            this.menuItemSelectUnlinkedSignals.Name = "menuItemSelectUnlinkedSignals";
            resources.ApplyResources(this.menuItemSelectUnlinkedSignals, "menuItemSelectUnlinkedSignals");
            this.menuItemSelectUnlinkedSignals.Click += new System.EventHandler(this.MenuItemSelectUnlinkedSignals_Click);
            // 
            // menuItemSelectUnlinkedTrackLinks
            // 
            this.menuItemSelectUnlinkedTrackLinks.Name = "menuItemSelectUnlinkedTrackLinks";
            resources.ApplyResources(this.menuItemSelectUnlinkedTrackLinks, "menuItemSelectUnlinkedTrackLinks");
            this.menuItemSelectUnlinkedTrackLinks.Click += new System.EventHandler(this.MenuItemSelectUnlinkedTrackLinks_Click);
            // 
            // menuItemSelectPowerConnectors
            // 
            this.menuItemSelectPowerConnectors.Name = "menuItemSelectPowerConnectors";
            resources.ApplyResources(this.menuItemSelectPowerConnectors, "menuItemSelectPowerConnectors");
            this.menuItemSelectPowerConnectors.Click += new System.EventHandler(this.MenuItemSelectPowerConnectors_Click);
            // 
            // menuItemSelectReverseLoopModules
            // 
            this.menuItemSelectReverseLoopModules.Name = "menuItemSelectReverseLoopModules";
            resources.ApplyResources(this.menuItemSelectReverseLoopModules, "menuItemSelectReverseLoopModules");
            this.menuItemSelectReverseLoopModules.Click += new System.EventHandler(this.MenuItemSelectReverseLoopModules_Click);
            // 
            // menuItemSeperator11
            // 
            this.menuItemSeperator11.Name = "menuItemSeperator11";
            resources.ApplyResources(this.menuItemSeperator11, "menuItemSeperator11");
            // 
            // menuItemDefaultPhase
            // 
            this.menuItemDefaultPhase.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemNewComponentDesignPhase,
            this.menuItemNewComponentConstructionPhase,
            this.menuItemNewComponentOperationalPhase});
            this.menuItemDefaultPhase.Name = "menuItemDefaultPhase";
            resources.ApplyResources(this.menuItemDefaultPhase, "menuItemDefaultPhase");
            this.menuItemDefaultPhase.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.MenuItemDefaultPhase_Popup);
            // 
            // menuItemNewComponentDesignPhase
            // 
            this.menuItemNewComponentDesignPhase.Name = "menuItemNewComponentDesignPhase";
            resources.ApplyResources(this.menuItemNewComponentDesignPhase, "menuItemNewComponentDesignPhase");
            this.menuItemNewComponentDesignPhase.Click += new System.EventHandler(this.MenuItemNewComponentDesignPhase_Click);
            // 
            // menuItemNewComponentConstructionPhase
            // 
            this.menuItemNewComponentConstructionPhase.Name = "menuItemNewComponentConstructionPhase";
            resources.ApplyResources(this.menuItemNewComponentConstructionPhase, "menuItemNewComponentConstructionPhase");
            this.menuItemNewComponentConstructionPhase.Click += new System.EventHandler(this.MenuItemNewComponentConstructionPhase_Click);
            // 
            // menuItemNewComponentOperationalPhase
            // 
            this.menuItemNewComponentOperationalPhase.Name = "menuItemNewComponentOperationalPhase";
            resources.ApplyResources(this.menuItemNewComponentOperationalPhase, "menuItemNewComponentOperationalPhase");
            this.menuItemNewComponentOperationalPhase.Click += new System.EventHandler(this.MenuItemNewComponentOperationalPhase_Click);
            // 
            // menuItemSetComponentPhase
            // 
            this.menuItemSetComponentPhase.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemSetToDesignPhase,
            this.menuItemSetToConstructionPhase,
            this.menuItemSetToOperationalPhase});
            this.menuItemSetComponentPhase.Name = "menuItemSetComponentPhase";
            resources.ApplyResources(this.menuItemSetComponentPhase, "menuItemSetComponentPhase");
            // 
            // menuItemSetToDesignPhase
            // 
            this.menuItemSetToDesignPhase.Name = "menuItemSetToDesignPhase";
            resources.ApplyResources(this.menuItemSetToDesignPhase, "menuItemSetToDesignPhase");
            this.menuItemSetToDesignPhase.Click += new System.EventHandler(this.MenuItemSetToDesignPhase_Click);
            // 
            // menuItemSetToConstructionPhase
            // 
            this.menuItemSetToConstructionPhase.Name = "menuItemSetToConstructionPhase";
            resources.ApplyResources(this.menuItemSetToConstructionPhase, "menuItemSetToConstructionPhase");
            this.menuItemSetToConstructionPhase.Click += new System.EventHandler(this.MenuItemSetToConstructionPhase_Click);
            // 
            // menuItemSetToOperationalPhase
            // 
            this.menuItemSetToOperationalPhase.Name = "menuItemSetToOperationalPhase";
            resources.ApplyResources(this.menuItemSetToOperationalPhase, "menuItemSetToOperationalPhase");
            this.menuItemSetToOperationalPhase.Click += new System.EventHandler(this.MenuItemSetToOperationalPhase_Click);
            // 
            // menuItemSeperator14
            // 
            this.menuItemSeperator14.Name = "menuItemSeperator14";
            resources.ApplyResources(this.menuItemSeperator14, "menuItemSeperator14");
            // 
            // menuItemStyles
            // 
            this.menuItemStyles.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemStyleFonts,
            this.menuItemStylePositions});
            this.menuItemStyles.Name = "menuItemStyles";
            resources.ApplyResources(this.menuItemStyles, "menuItemStyles");
            // 
            // menuItemStyleFonts
            // 
            this.menuItemStyleFonts.Name = "menuItemStyleFonts";
            resources.ApplyResources(this.menuItemStyleFonts, "menuItemStyleFonts");
            this.menuItemStyleFonts.Click += new System.EventHandler(this.MenuItemStyleFonts_Click);
            // 
            // menuItemStylePositions
            // 
            this.menuItemStylePositions.Name = "menuItemStylePositions";
            resources.ApplyResources(this.menuItemStylePositions, "menuItemStylePositions");
            this.menuItemStylePositions.Click += new System.EventHandler(this.MenuItemStylePositions_Click);
            // 
            // menuItemLocomotiveCatalog
            // 
            this.menuItemLocomotiveCatalog.Name = "menuItemLocomotiveCatalog";
            resources.ApplyResources(this.menuItemLocomotiveCatalog, "menuItemLocomotiveCatalog");
            this.menuItemLocomotiveCatalog.Click += new System.EventHandler(this.MenuItemLocomotiveCatalog_Click);
            // 
            // menuItemAccelerationProfiles
            // 
            this.menuItemAccelerationProfiles.Name = "menuItemAccelerationProfiles";
            resources.ApplyResources(this.menuItemAccelerationProfiles, "menuItemAccelerationProfiles");
            this.menuItemAccelerationProfiles.Click += new System.EventHandler(this.MenuItemAccelerationProfiles_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Name = "menuItem1";
            resources.ApplyResources(this.menuItem1, "menuItem1");
            // 
            // menuItemArea
            // 
            this.menuItemArea.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemNewArea,
            this.menuItemDeleteArea,
            this.menuItemRenameArea,
            this.menuSeperator2,
            this.menuItemAreaArrange});
            this.menuItemArea.Name = "menuItemArea";
            resources.ApplyResources(this.menuItemArea, "menuItemArea");
            // 
            // menuItemNewArea
            // 
            this.menuItemNewArea.Name = "menuItemNewArea";
            resources.ApplyResources(this.menuItemNewArea, "menuItemNewArea");
            this.menuItemNewArea.Click += new System.EventHandler(this.MenuItemNewArea_Click);
            // 
            // menuItemDeleteArea
            // 
            this.menuItemDeleteArea.Name = "menuItemDeleteArea";
            resources.ApplyResources(this.menuItemDeleteArea, "menuItemDeleteArea");
            this.menuItemDeleteArea.Click += new System.EventHandler(this.MenuItemDeleteArea_Click);
            // 
            // menuItemRenameArea
            // 
            this.menuItemRenameArea.Name = "menuItemRenameArea";
            resources.ApplyResources(this.menuItemRenameArea, "menuItemRenameArea");
            this.menuItemRenameArea.Click += new System.EventHandler(this.MenuItemRenameArea_Click);
            // 
            // menuSeperator2
            // 
            this.menuSeperator2.Name = "menuSeperator2";
            resources.ApplyResources(this.menuSeperator2, "menuSeperator2");
            // 
            // menuItemAreaArrange
            // 
            this.menuItemAreaArrange.Name = "menuItemAreaArrange";
            resources.ApplyResources(this.menuItemAreaArrange, "menuItemAreaArrange");
            this.menuItemAreaArrange.Click += new System.EventHandler(this.MenuItemAreaArrange_Click);
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
            this.panelTripsMonitor.Controls.Add(this.tabAreas);
            this.panelTripsMonitor.Controls.Add(this.splitterTripsMonitor);
            this.panelTripsMonitor.Controls.Add(this.tripsMonitor);
            resources.ApplyResources(this.panelTripsMonitor, "panelTripsMonitor");
            this.panelTripsMonitor.Name = "panelTripsMonitor";
            // 
            // tabAreas
            // 
            resources.ApplyResources(this.tabAreas, "tabAreas");
            this.tabAreas.Multiline = true;
            this.tabAreas.Name = "tabAreas";
            this.tabAreas.SelectedIndex = 0;
            // 
            // splitterTripsMonitor
            // 
            this.splitterTripsMonitor.BackColor = System.Drawing.SystemColors.WindowFrame;
            resources.ApplyResources(this.splitterTripsMonitor, "splitterTripsMonitor");
            this.splitterTripsMonitor.Name = "splitterTripsMonitor";
            this.splitterTripsMonitor.TabStop = false;
            this.splitterTripsMonitor.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.SplitterTripsMonitor_SplitterMoved);
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
            this.locomotivesViewer.Name = "locomotivesViewer";
            // 
            // panelLayoutControl
            // 
            this.panelLayoutControl.Controls.Add(this.layoutControlViewer);
            this.panelLayoutControl.Controls.Add(this.splitterLayoutControl);
            this.panelLayoutControl.Controls.Add(this.panelLocomotiveViewer);
            resources.ApplyResources(this.panelLayoutControl, "panelLayoutControl");
            this.panelLayoutControl.Name = "panelLayoutControl";
            // 
            // layoutControlViewer
            // 
            resources.ApplyResources(this.layoutControlViewer, "layoutControlViewer");
            this.layoutControlViewer.Name = "layoutControlViewer";
            // 
            // splitterLayoutControl
            // 
            this.splitterLayoutControl.BackColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.splitterLayoutControl, "splitterLayoutControl");
            this.splitterLayoutControl.Name = "splitterLayoutControl";
            this.splitterLayoutControl.TabStop = false;
            this.splitterLayoutControl.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.SplitterLayoutControl_SplitterMoved);
            // 
            // mainMenu
            // 
            this.mainMenu.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuEdit,
            this.menuItemView,
            this.menuItemArea,
            this.menuLayout,
            this.menuItemTools});
            resources.ApplyResources(this.mainMenu, "mainMenu");
            this.mainMenu.Name = "mainMenu";
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
            this.timerDetailsPopup.Tick += new System.EventHandler(this.TimerDetailsPopup_Tick);
            // 
            // printDocument
            // 
            this.printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.PrintDocument_PrintPage);
            // 
            // menuItem13
            // 
            this.menuItem13.Name = "menuItem13";
            resources.ApplyResources(this.menuItem13, "menuItem13");
            // 
            // LayoutFrameWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainMenu);
            this.Controls.Add(this.panelLayoutControl);
            this.Name = "LayoutFrameWindow";
            this.Activated += new System.EventHandler(this.LayoutController_Activated);
            this.Deactivate += new System.EventHandler(this.LayoutController_Deactivate);
            this.Resize += new System.EventHandler(this.LayoutController_Resize);
            this.panelMessageViewer.ResumeLayout(false);
            this.panelTripsMonitor.ResumeLayout(false);
            this.panelLocomotiveViewer.ResumeLayout(false);
            this.panelLayoutControl.ResumeLayout(false);
            this.panelLayoutControl.PerformLayout();
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timerFreeResources)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
        #endregion

        private IContainer components;
        private System.Windows.Forms.ToolStripMenuItem menuItem2;
		private System.Windows.Forms.ToolStripMenuItem menuItemView;
		private System.Windows.Forms.ToolStripMenuItem menuItemViewNew;
		private System.Windows.Forms.ToolStripMenuItem menuItemViewDelete;
		private System.Windows.Forms.ToolStripMenuItem menuItemViewRename;
		private System.Windows.Forms.ToolStripSeparator menuItemSeperator6;
		private System.Windows.Forms.ToolStripMenuItem menuItemZoomMenu;
		private System.Windows.Forms.ToolStripMenuItem menuItemSetZoom;
		private System.Windows.Forms.ToolStripSeparator menuItemSeperator5;
		private System.Windows.Forms.ToolStripMenuItem menuItemZoom200;
		private System.Windows.Forms.ToolStripMenuItem menuItemZoom150;
		private System.Windows.Forms.ToolStripMenuItem menuItemZoom100;
		private System.Windows.Forms.ToolStripMenuItem menuItemZoom75;
		private System.Windows.Forms.ToolStripMenuItem menuItemZoom50;
		private System.Windows.Forms.ToolStripMenuItem menuItemZoom25;
		private System.Windows.Forms.ToolStripSeparator menuItemSeperator12;
		private System.Windows.Forms.ToolStripMenuItem menuItem1ZoomAllArea;
		private System.Windows.Forms.ToolStripMenuItem menuItemViewGrid;
		private System.Windows.Forms.ToolStripMenuItem menuItemShowMessages;
		private System.Windows.Forms.ToolStripMenuItem menuItemShowLocomotives;
		private System.Windows.Forms.ToolStripSeparator menuItemSeperator4;
		private System.Windows.Forms.ToolStripMenuItem menuItemViewArrage;
		private System.Windows.Forms.ToolStripMenuItem menuItemOperational;
		private System.Windows.Forms.ToolStripMenuItem menuItemTools;
		private System.Windows.Forms.ToolStripMenuItem menuItemDummy;
		private System.Windows.Forms.ToolStripMenuItem menuLayout;
		private System.Windows.Forms.ToolStripMenuItem menuItemDesign;
		private System.Windows.Forms.ToolStripMenuItem menuItemCompile;
		private System.Windows.Forms.MenuStrip mainMenu;
		private System.Windows.Forms.ToolStripMenuItem menuFile;
		private System.Windows.Forms.ToolStripMenuItem menuItemSave;
		private System.Windows.Forms.ToolStripSeparator menuItemSeperator1;
		private System.Windows.Forms.ToolStripMenuItem menuItemModules;
		private System.Windows.Forms.ToolStripSeparator menuItemSeperator3;
		private System.Windows.Forms.ToolStripMenuItem menuItemExit;
		private System.Windows.Forms.ToolStripMenuItem menuEdit;
		private System.Windows.Forms.ToolStripMenuItem menuItemUndo;
		private System.Windows.Forms.ToolStripMenuItem menuItemRedo;
		private System.Windows.Forms.ToolStripSeparator menuItemSeparator;
		private System.Windows.Forms.ToolStripMenuItem menuItemCopy;
		private System.Windows.Forms.ToolStripMenuItem menuItemCut;
		private System.Windows.Forms.ToolStripMenuItem menuItem1;
		private System.Windows.Forms.ToolStripMenuItem menuItemStyles;
		private System.Windows.Forms.ToolStripMenuItem menuItemStyleFonts;
		private System.Windows.Forms.ToolStripMenuItem menuItemStylePositions;
		private System.Windows.Forms.ToolStripMenuItem menuItemLocomotiveCatalog;
		private System.Windows.Forms.ToolStripMenuItem menuItemArea;
		private System.Windows.Forms.ToolStripMenuItem menuItemNewArea;
		private System.Windows.Forms.ToolStripMenuItem menuItemDeleteArea;
		private System.Windows.Forms.ToolStripMenuItem menuItemRenameArea;
		private System.Windows.Forms.ToolStripSeparator menuSeperator2;
		private System.Windows.Forms.ToolStripMenuItem menuItemAreaArrange;
		private System.Windows.Forms.Splitter splitterLocomotives;
		private System.Windows.Forms.TabControl tabAreas;
		private System.Windows.Forms.Splitter splitterMessages;
		private LayoutManager.MessageViewer messageViewer;
		private System.Windows.Forms.Splitter splitterTripsMonitor;
		private LayoutManager.Tools.Controls.TripsMonitor tripsMonitor;
		private System.Windows.Forms.Splitter splitterLayoutControl;
		private LayoutManager.LocomotivesViewer locomotivesViewer;
		private System.Windows.Forms.ToolStripMenuItem menuItemManualDispatchRegions;
		private System.Windows.Forms.ToolStripMenuItem menuItem4;
		private System.Windows.Forms.Panel panelMessageViewer;
		private System.Windows.Forms.Panel panelLocomotiveViewer;
		private System.Windows.Forms.Panel panelTripsMonitor;
		private System.Windows.Forms.Panel panelLayoutControl;
		private LayoutManager.CommonUI.Controls.LayoutControlViewer layoutControlViewer;
		private System.Windows.Forms.ToolStripMenuItem menuItemShowTripsMonitor;
		private System.Windows.Forms.ToolStripMenuItem menuItemPolicies;
		private System.Windows.Forms.ToolStripMenuItem menuItem6;
		private System.Windows.Forms.ToolStripMenuItem menuItemAccelerationProfiles;
		private System.Windows.Forms.ToolStripMenuItem menuItemDefaultDriverParameters;
		private System.Windows.Forms.ToolStripMenuItem menuItemCommonDestinations;
		private System.Windows.Forms.ToolStripMenuItem menuItemEmergencyStop;
		private System.Windows.Forms.ToolStripMenuItem menuItemSuspendLocomotives;
		private System.Windows.Forms.ToolStripSeparator menuItemSeperator7;
		private System.Windows.Forms.ToolStripMenuItem menuItemShowLayoutControl;
		private System.Windows.Forms.ToolStripMenuItem menuItemConnectLayout;
		private System.Windows.Forms.ToolStripSeparator menuItemSeperator8;
		private System.Windows.Forms.ToolStripMenuItem menuItemLearnLayout;
		private System.Windows.Forms.ToolStripSeparator menuItemDesignTimeOnlySeperator;
		private System.Windows.Forms.ToolStripMenuItem menuItemFind;
		private System.Windows.Forms.ToolStripMenuItem menuItemPrint;
		private System.Windows.Forms.ToolStripSeparator menuItemSeperator9;
		private System.Windows.Forms.ToolStripMenuItem menuItemRestoreDefaultZoomAndOrigin;
		private System.Windows.Forms.ToolStripMenuItem menuItemSaveAsDefaultZoomAndOrigin;
		private System.Windows.Forms.ToolStripSeparator menuItemSeperator10;
		private System.Windows.Forms.ToolStripMenuItem menuItemSelect;
		private System.Windows.Forms.ToolStripMenuItem menuItemSelectUnconnectedComponents;
		private System.Windows.Forms.ToolStripMenuItem menuItemSelectUnlinkedSignals;
		private System.Windows.Forms.ToolStripSeparator menuItemSeperator11;
		private System.Windows.Forms.ToolStripMenuItem menuItemSelectUnlinkedTrackLinks;
		private System.Windows.Forms.ToolStripMenuItem menuItem13;
		private System.Windows.Forms.ToolStripMenuItem menuItemDefaultPhase;
		private System.Windows.Forms.ToolStripMenuItem menuItemNewComponentDesignPhase;
		private System.Windows.Forms.ToolStripMenuItem menuItemNewComponentConstructionPhase;
		private System.Windows.Forms.ToolStripMenuItem menuItemNewComponentOperationalPhase;
		private System.Windows.Forms.ToolStripMenuItem menuItemSetComponentPhase;
		private System.Windows.Forms.ToolStripMenuItem menuItemSetToDesignPhase;
		private System.Windows.Forms.ToolStripMenuItem menuItemSetToConstructionPhase;
		private System.Windows.Forms.ToolStripMenuItem menuItemSetToOperationalPhase;
		private System.Windows.Forms.ToolStripSeparator menuItemSeperator14;
		private System.Windows.Forms.ToolStripMenuItem menuItemSimulation;
		private System.Windows.Forms.ToolStripMenuItem menuItemViewActivePhases;
		private Timer timerDetailsPopup;
		private System.Drawing.Printing.PrintDocument printDocument;
		private System.Timers.Timer timerFreeResources;
		private System.Windows.Forms.ToolStripMenuItem menuItemNewWindow;
		private System.Windows.Forms.ToolStripMenuItem menuItemCloseWindow;
		private System.Windows.Forms.ToolStripMenuItem menuItemSelectPowerConnectors;
		private System.Windows.Forms.ToolStripMenuItem menuItemSelectReverseLoopModules;
        private System.Windows.Forms.ToolStripMenuItem menuItemVerificationOptions;
        private System.Windows.Forms.ToolStripMenuItem menuItemTrainTracking;
    }
}
