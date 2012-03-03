using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Xml;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;

using LayoutManager.Model;
using LayoutManager.View;

namespace LayoutManager
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class LayoutController : System.Windows.Forms.Form, ILayoutController, ILayoutSelectionManager, ILayoutInvoker
	{
		private System.Windows.Forms.ToolBar toolBarMain;
		private System.ComponentModel.IContainer components;
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
		private System.Windows.Forms.MenuItem menuItemSeperator;
		private System.Windows.Forms.MenuItem menuItemCompile;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuFile;
		private System.Windows.Forms.MenuItem menuItemNewLayout;
		private System.Windows.Forms.MenuItem menuItemOpen;
		private System.Windows.Forms.MenuItem menuItemSave;
		private System.Windows.Forms.MenuItem menuSaveAs;
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
		private System.Timers.Timer timerFreeResources;
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
		private System.Windows.Forms.StatusBar statusBar;

		private void EndOfDesignerVariables() { }

		/// <summary>
		/// The layout model
		/// </summary>
		LayoutModel						model;
		LayoutModelLayer				activeLayer;

		String							layoutFilename = null;
		const String					fileTypeFilter = "Layout files (*.layout)|*.layout";

		LayoutCommandManager			commandManager;
		LayoutTool						designTool = null;
		LayoutTool						operationTool = null;
		LayoutTool						activeTool = null;
		ArrayList						displayedSelections = new ArrayList();
		LayoutSelectionWithUndo			userSelection;
		LayoutEventManager				eventManager;
		PreviewRouteManager				previewRouteManager = new PreviewRouteManager();
		ZoomEntry[]						zoomEntries;
		LayoutModuleManager				moduleManager;
		int								layoutDesignTimeActivationNesting = 0;

		bool							operationMode = false;
		bool							layoutControlVisible = false;
		bool							tripsMonitorVisible = false;
		bool							messageViewVisible = false;
		bool							locomotiveViewVisible = false;
		int								tripsMonitorHeightInOperationMode = 0;
		int								layoutControlWidth = 0;

		enum UIitemSetting {
			Hidden,
			Disabled
		};

		abstract class UIsettingEntry {
			protected UIitemSetting	setting;

			public UIitemSetting Setting {
				get {
					return setting;
				}
			}
		}

		class MenuSettingEntry : UIsettingEntry {
			MenuItem		menuItem;

			public MenuSettingEntry(MenuItem menuItem, UIitemSetting setting) {
				this.menuItem = menuItem;
				this.setting = setting;
			}

			public MenuItem MenuItem {
				get {
					return menuItem;
				}
			}
		}

		class ToolbarSettingEntry : UIsettingEntry {
			ToolBarButton	button;

			public ToolbarSettingEntry (ToolBarButton button, UIitemSetting setting) {
				this.button = button;
				this.setting = setting;
			}

			public ToolBarButton Button {
				get {
					return button;
				}
			}
		}

		UIsettingEntry[]	operationModeUIsetting;
		UIsettingEntry[]	designModeUIsetting;

		public LayoutController()
		{
			eventManager = new LayoutEventManager();
			moduleManager = new LayoutModuleManager(eventManager);
			commandManager = new LayoutCommandManager(eventManager);

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			layoutControlViewer.Width = 0;
			tripsMonitor.Height = 0;
			messageViewer.Height = 0;
			locomotivesViewer.Width = 0;

			// Initialize the buildit zoom preset table
			zoomEntries =  new ZoomEntry[] { 
											   new ZoomEntry(200, menuItemZoom200),
											   new ZoomEntry(150, menuItemZoom150),
											   new ZoomEntry(100, menuItemZoom100),
											   new ZoomEntry(70, menuItemZoom75),
											   new ZoomEntry(50, menuItemZoom50),
											   new ZoomEntry(20, menuItemZoom25),
			};

			// Initialize the menu setting tables
			operationModeUIsetting = new UIsettingEntry[] {
				new MenuSettingEntry(menuItemNewLayout, UIitemSetting.Disabled),
				new MenuSettingEntry(menuItemOpen, UIitemSetting.Disabled),
				new MenuSettingEntry(menuItemArea, UIitemSetting.Hidden),
				new MenuSettingEntry(menuItemOperational, UIitemSetting.Disabled),
				new MenuSettingEntry(menuItemCompile, UIitemSetting.Hidden),
				new MenuSettingEntry(menuItemSeparator, UIitemSetting.Hidden),
				new MenuSettingEntry(menuItemLearnLayout, UIitemSetting.Hidden),
				new MenuSettingEntry(menuItemConnectLayout, UIitemSetting.Hidden),
				new ToolbarSettingEntry(toolBarButtonOpenLayiout, UIitemSetting.Hidden),
			};

			designModeUIsetting = new UIsettingEntry[] {
				new MenuSettingEntry(menuItemDesign, UIitemSetting.Disabled),
				new MenuSettingEntry(menuItemCommonDestinations, UIitemSetting.Hidden),
				new MenuSettingEntry(menuItemManualDispatchRegions, UIitemSetting.Hidden),
				new MenuSettingEntry(menuItemShowTripsMonitor, UIitemSetting.Hidden),
			    new MenuSettingEntry(menuItemEmergencyStop, UIitemSetting.Hidden),
				new MenuSettingEntry(menuItemSuspendLocomotives, UIitemSetting.Hidden),
				new MenuSettingEntry(menuItemEmergencyStop, UIitemSetting.Hidden),
				new ToolbarSettingEntry(toolBarButtonShowTripsMonitor, UIitemSetting.Hidden),
				new ToolbarSettingEntry(toolBarButtonToggleAllLayoutManualDispatch, UIitemSetting.Hidden),
				new ToolbarSettingEntry(toolBarButtonStopAllLocomotives, UIitemSetting.Hidden),
				new ToolbarSettingEntry(toolBarButtonEmergencyStop, UIitemSetting.Hidden),
			};

			//
			// Initialize the module manager
			//
			moduleManager.DocumentFilename = CommonDataFolderName + Path.DirectorySeparatorChar + "ModuleManager.xml";

			//
			// dummy references to assemblies that contains only layout modules. This is needed, otherwise
			// those assemblies are not automatically referenced and the user will have to use the file/modules
			// menu to add them.
			//
			String		s;
			
			s = LayoutManager.Logic.LayoutTopologyServices.TopologyServicesVersion;
			s = LayoutManager.Tools.ComponentEditingTools.ComponentEditingToolsVersion;

			// Initialize built in modules

			AssemblyName[]	referenced = this.GetType().Assembly.GetReferencedAssemblies();

			// Load all the assemblies which are referenced by the application
			foreach(AssemblyName assemblyName in referenced) {
				Assembly	refAssembly = Assembly.Load(assemblyName);

				moduleManager.LayoutAssemblies.Add(new LayoutAssembly(eventManager, refAssembly));
			}

			moduleManager.LayoutAssemblies.Add(new LayoutAssembly(eventManager, this.GetType().Assembly));


			// Load assemblies that were defined by the user
			moduleManager.LoadState();

			eventManager.AddObjectSubscriptions(this);

			initializeModel();

			messageViewer.EventManager = eventManager;
			locomotivesViewer.EventManager = eventManager;
			tripsMonitor.EventManager = eventManager;
			layoutControlViewer.EventManager = eventManager;

			// Initial state is to show a new layout
			createNewLayout();

#if LAYERS
			initializeLayerSelector();
#else
			ActiveLayer = model.Layers[0];
#endif
			doUIsetting(designModeUIsetting);

			userSelection = new LayoutSelectionWithUndo(this);
			userSelection.Display(new LayoutSelectionLook(Color.Blue));

			designTool = new LayoutEditingTool(this);
			operationTool = new LayoutOperationTool(this);

			locomotivesViewer.LocomotiveCollection = model.LocomotiveCollection;
			locomotivesViewer.StateManager = model.StateManager;

			try {
				ApplicationStateInfo	applicationState = new ApplicationStateInfo(CommonDataFolderName + Path.DirectorySeparatorChar + "ApplicationState.xml");
				string					layoutToOpen = null;
				bool					newLayout = false;
				bool					initializeWindow = true;
				bool					first = true;

				foreach(string arg in Environment.GetCommandLineArgs()) {
					if(first) {
						first = false;			// skip the executable filename
						continue;
					}

					if(arg[0] == '-' || arg[0] == '/') {
						switch( arg.ToLower()[1]) {
							case 'n':
								newLayout = true;
								break;

							case 'w':
								initializeWindow = false;
								break;

							case 'o':
								applicationState.State = ApplicationStateInfo.LayoutState.Operation;
								break;

							case 'd':
								applicationState.State = ApplicationStateInfo.LayoutState.Design;
								break;

							default:
								MessageBox.Show(this, "Invalid command line option " + arg + "\n\n" +
									"Valid options are /new, /window, /design or /operation", "Invalid Command Line Option", MessageBoxButtons.OK, MessageBoxIcon.Error);
								break;
						}
					}
					else {
						if(layoutToOpen == null) {
							layoutToOpen = arg;
							if(layoutToOpen.IndexOf(".") < 0)
								layoutToOpen += ".Layout";
						}
					}
				}

				if(newLayout)
					layoutToOpen = null;
				else if(layoutToOpen == null)
					layoutToOpen = applicationState.LayoutFilename;

				CreateControl();
				System.Threading.Thread.Sleep(200);

				if(initializeWindow) {
					StartPosition = FormStartPosition.Manual;
					SetDesktopBounds(applicationState.Bounds.Left, applicationState.Bounds.Top, applicationState.Bounds.Width, applicationState.Bounds.Height);
					if(applicationState.WindowState == FormWindowState.Maximized)
						WindowState = applicationState.WindowState;
				}


				if(layoutToOpen != null)
					openLayout(layoutToOpen);

				if(applicationState.State == ApplicationStateInfo.LayoutState.Operation)
					eventManager.Event(new LayoutEvent(this, "enter-operation-mode-request"));
				else
					eventManager.Event(new LayoutEvent(this, "enter-design-mode-request"));
			} catch(Exception ex) {
				Trace.WriteLine("Application state could not be restored " + ex.Message);

				eventManager.Event(new LayoutEvent(this, "enter-design-mode-request"));
			}
		}

		#region Implementation of ILayoutController

		/// <summary>
		/// The layer on which editing operation take place
		/// </summary>
		public LayoutModelLayer ActiveLayer {
			get {
				return activeLayer;
			}

			set {
				activeLayer = value;
			}
		}

		/// <summary>
		/// Execute a command object
		/// </summary>
		/// <param name="command"></param>
		public void Do(ILayoutCommand command) {
			commandManager.Do(command);
		}

		/// <summary>
		/// The model
		/// </summary>
		public LayoutModel Model {
			get {
				return model;
			}
		}

		/// <summary>
		/// The selection of components selected by the user
		/// </summary>
		public LayoutSelectionWithUndo UserSelection {
			get {
				return userSelection;
			}
		}

		public LayoutEventManager EventManager {
			get {
				return eventManager;
			}
		}

		public PreviewRouteManager PreviewRouteManager {
			get {
				return previewRouteManager;
			}
		}

		public LayoutModuleManager ModuleManager {
			get {
				return moduleManager;
			}
		}

		public bool IsOperationMode {
			get {
				return operationMode;
			}
		}

		public void LayoutModified() {
			commandManager.ChangeLevel++;
		}

		#endregion

		#region Implementation of ILayoutInvoker

		public bool CanInvoke {
			get {
				return IsHandleCreated;
			}
		}

		private delegate object LayoutEventCaller(LayoutEvent e);

		public object Event(LayoutEvent e) {
			if(InvokeRequired) {
				if(CanInvoke) {
					return Invoke(new LayoutEventCaller(EventManager.Event), new object[] { e } );
				}
				else
					throw new ApplicationException("Need to invoke LayoutEventManager.Event, however CanInvoke returns false");
			}
			else
				return EventManager.Event(e);
		}

		#endregion

		#region Implementation of ILayoutSelectionManager

		public void DisplaySelection(LayoutSelection selection, int zOrder) {
			if(zOrder == LayoutSelection.ZOrderBottom)
				displayedSelections.Add(selection);
			else
				displayedSelections.Insert(0, selection);
		}

		public void HideSelection(LayoutSelection selection) {
			displayedSelections.Remove(selection);
		}

		public LayoutSelection[] DisplayedSelections {
			get {
				return (LayoutSelection[])displayedSelections.ToArray(typeof(LayoutSelection));
			}
		}

		#endregion

		#region active Area/View properties

		LayoutFrameWindowAreaTabPage ActiveAreaPage {
			get {
				return (LayoutFrameWindowAreaTabPage)tabAreas.SelectedTab;
			}
		}

		LayoutFrameWindowAreaViewTabPage ActiveViewPage {
			get {
				if(ActiveAreaPage == null)
					return null;
				return (LayoutFrameWindowAreaViewTabPage)ActiveAreaPage.TabViews.SelectedTab;
			}
		}

		LayoutView ActiveView {
			get {
				if(ActiveViewPage == null)
					return null;
				return ActiveViewPage.View;
			}
		}

		#endregion

		#region Get data folder name

		String CommonDataFolderName {
			get {
				return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + Application.ProductName;
			}
		}

		#endregion

		void initializeModel() {
			model = (LayoutModel)eventManager.Event(new LayoutEvent(this, "get-model"));
			eventManager.Event(new LayoutEvent(this, "modules-initialized", null, model), LayoutEventScope.SystemWide);

			if(!model.ReadModelXmlInfo(CommonDataFolderName)) {
				MessageBox.Show("The system wide definition files could not be found. Default definitions for " +
					"fonts etc. will be used", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

			// Wire areas collection events
			model.Areas.AreaAdded += new EventHandler(Areas_Added);
			model.Areas.AreaRemoved += new EventHandler(Areas_Removed);
			model.Areas.AreaRenamed += new EventHandler(Areas_Renamed);
		}

		/// <summary>
		/// Create a new layout, that is a new model, and one default view called overview
		/// </summary>
		void createNewLayout() {
			model.Clear();
			tabAreas.TabPages.Clear();

			LayoutModelArea	area = new LayoutModelArea();

			// Create the default area
			area.Name = "Layout area";
			model.Areas.Add(area);
			AddViewToArea(area, "Overview");

			layoutFilename = null;
			commandManager.ChangeLevel = 0;
			eventManager.Event(new LayoutEvent(model, "new-layout-document", null, layoutFilename));
			updateFormTitle();
		}

		/// <summary>
		/// Add a view to an area. A new tab page is created to show the view
		/// </summary>
		/// <param name="area">The area to which to assign the view</param>
		/// <param name="name">The view's name</param>
		/// <param name="view">The view object</param>
		public LayoutView AddViewToArea(LayoutModelArea area, String name, LayoutView view) {
			LayoutFrameWindowAreaTabPage	areaTabPage = null;

			foreach(LayoutFrameWindowAreaTabPage atp in tabAreas.TabPages) {
				if(atp.Area == area) {
					areaTabPage = atp;
					break;
				}
			}

			if(areaTabPage == null)
				throw new ApplicationException("Invalid area - no tab page found");

			view.Controller = this;
			view.Area = area;

			// Wire events in order to reflect them to the current tool
			view.MouseDown += new MouseEventHandler(this.LayoutView_MouseDown);

			LayoutFrameWindowAreaViewTabPage	viewTabPage = new LayoutFrameWindowAreaViewTabPage(view);

			viewTabPage.Text = name;
			areaTabPage.TabViews.TabPages.Add(viewTabPage);

			return view;
		}

		public LayoutView AddViewToArea(LayoutModelArea area, String name) {
			return AddViewToArea(area, name, new LayoutView());
		}

		/// <summary>
		/// Update the form title based on the layout file used
		/// </summary>
		private void updateFormTitle() {
			string	layoutName;
			string	modified = "";

			if(layoutFilename == null)
				layoutName = "Untitled";
			else
				layoutName = layoutFilename;

			if(commandManager.ChangeLevel != 0)
				modified = "*";

			this.Text = Path.GetFileName(layoutName) + modified + " - YR Layout Manager";
		}

		[LayoutEvent("model-modified")]
		[LayoutEvent("model-not-modified")]
		private void modelModificationStateChanged(LayoutEvent e) {
			updateFormTitle();
		}

		/// <summary>
		/// Add a new area to the model
		/// </summary>
		/// <returns>The newly created area</returns>
		public LayoutModelArea AddArea(String areaName) {
			LayoutModelArea	area = new LayoutModelArea();

			area.Name = areaName;
			model.Areas.Add(area);
			AddViewToArea(area, "Overview");

			return area;
		}

		[LayoutEvent("test")]
		private void test(LayoutEvent e) {
			Trace.WriteLine("Got event test");
			Trace.WriteLine("Sender is " + (e.Sender == null ? "Null" : e.Sender.ToString()));
			Trace.WriteLine("Info is " + (e.Info == null ? "Null" : e.Info.ToString()));

			XmlElement	optionsElement = e.Element["Options"];

			if(optionsElement != null) {
				Trace.WriteLine("Options:");

				foreach(XmlAttribute a in optionsElement.Attributes)
					Trace.WriteLine("  Option " + a .Name + " is " + a.Value);
			}
		}

		[LayoutEvent("tools-menu-open-request")]
		private void OnToolsMenuOpenRequest(LayoutEvent e) {
			ILayoutEmulatorServices	layoutEmulationServices = (ILayoutEmulatorServices)EventManager.Event(new LayoutEvent(this, "get-layout-emulation-services"));

			if(layoutEmulationServices != null) {
				Menu toolsMenu = (Menu)e.Info;

				toolsMenu.MenuItems.Add(new MenuItem("Reset layout emulation", new EventHandler(onResetLayoutEmulation)));
			}
		}

		[LayoutEvent("show-routing-table-generation-dialog")]
		private void showRoutingTableGenerationDialog(LayoutEvent e) {
			int									nTurnouts = (int)e.Info;
			Dialogs.RoutingTableCalcProgress	d = new Dialogs.RoutingTableCalcProgress(eventManager, nTurnouts);

			d.ShowDialog(this);
		}

		private void onResetLayoutEmulation(object sender, EventArgs e) {
			EventManager.Event(new LayoutEvent(this, "reset-layout-emulation"));
		}

		[LayoutEvent("get-module-manager")]
		private void getModuleManager(LayoutEvent e) {
			e.Info = moduleManager;
		}

		[LayoutEvent("enter-design-mode")]
		private void enterDesignMode(LayoutEvent e) {
			statusBarPanelMode.Text = "Design";
			toolBarButtonDesignMode.Pushed = true;
			toolBarButtonOperationMode.Pushed = false;
			tripsMonitorHeightInOperationMode = tripsMonitor.Height;
			eventManager.Event(new LayoutEvent(this, "hide-trips-monitor"));
			splitterTripsMonitor.Visible = false;
			layoutDesignTimeActivationNesting = 0;
		}

		[LayoutEvent("enter-operation-mode")]
		private void enterOperationMode(LayoutEvent e) {
			statusBarPanelMode.Text = "Operation";
			toolBarButtonDesignMode.Pushed = false;
			toolBarButtonOperationMode.Pushed = true;
			if(tripsMonitorHeightInOperationMode > 0) {
				tripsMonitor.Height = tripsMonitorHeightInOperationMode;
				UpdateTripsMonitorVisible();
			}
			splitterTripsMonitor.Visible = true;
		}

		[LayoutEvent("enter-design-mode-request")]
		void enterDesignModeRequest(LayoutEvent e) {
			if(operationMode)
				eventManager.Event(new LayoutEvent(this, "exit-operation-mode-request"), LayoutEventScope.SystemWide);

			doUIsetting(designModeUIsetting);
			
			activeTool = designTool;
			operationMode = false;
			eventManager.Event(new LayoutEvent(this, "enter-design-mode"), LayoutEventScope.SystemWide);

			model.Redraw();

			e.Info = true;
		}

		[LayoutEvent("enter-operation-mode-request")]
		void enterOpertationModeRequest(LayoutEvent e) {
			bool	switchMode = true;

			if(switchMode) {
				if(commandManager.ChangeLevel != 0) {
					if(!saveLayout(false)) {
						MessageBox.Show(this, "The layout must be saved before switching to operation mode", "Layout not saved", MessageBoxButtons.OK, MessageBoxIcon.Error);
						switchMode = false;
					}
				}
			}

			eventManager.Event(new LayoutEvent(this, "clear-messages"));

			if(switchMode) {
				if(!(bool)eventManager.Event(new LayoutEvent(model, "check-layout", null, true), LayoutEventScope.Model)) {
					MessageBox.Show(this, "Problems were detected in the layout. Please fix those problems before entering operation mode", "Layout Check Results", 
						MessageBoxButtons.OK, MessageBoxIcon.Stop);
					switchMode = false;
				}
			}

			if(switchMode) {
				if(!(bool)eventManager.Event(new LayoutEvent(model, "rebuild-layout-state"), LayoutEventScope.Model)) {
					if(MessageBox.Show(this, "The layout design or locomotive collection were modified. The previous state could not be fully restored.\n\n" +
						"Would you like to continue with the partially restored state?\n\nSelecting \"No\" will clear the state. In this case, you will " +
						"have to indicate the locomotive positions again", "Locomotive state cannot be restored",
						MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
						eventManager.Event(new LayoutEvent(model, "clear-layout-state"));
				}

				try {
					if(layoutDesignTimeActivationNesting > 0) {
						eventManager.Event(new LayoutEvent(this, "end-design-time-layout-activation"));
						layoutDesignTimeActivationNesting = 0;
					}

					// Switch to operation mode.
					if(!operationMode) {
						eventManager.Event(new LayoutEvent(this, "exit-design-mode"), LayoutEventScope.SystemWide);
						undoUIsetting(designModeUIsetting);
					}

					doUIsetting(operationModeUIsetting);
					commandManager.Clear();			// Clear undo stack

					activeTool = operationTool;
					eventManager.Event(new LayoutEvent(this, "enter-operation-mode"), LayoutEventScope.SystemWide);

					operationMode = true;

					model.Redraw();

					e.Info = true;
				} catch(Exception ex) {
					EventManager.Event(new LayoutEvent(null, "add-error", null, "Could not enter operational mode - " + ex.Message));
					undoUIsetting(operationModeUIsetting);
					toolBarButtonOperationMode.Pushed = false;

					ExitOperationMode(e);
					EventManager.Event(new LayoutEvent(this, "enter-design-mode-request"));
					e.Info = false;
				}
			}
			else {
				toolBarButtonOperationMode.Pushed = false;
				e.Info = false;
			}
		}

		[LayoutEvent("exit-operation-mode-request")]
		void ExitOperationMode(LayoutEvent e) {
			eventManager.Event(new LayoutEvent(this, "exit-operation-mode"), LayoutEventScope.SystemWide);
			undoUIsetting(operationModeUIsetting);
		}

		[LayoutEvent("begin-design-time-layout-activation-request")]
		private void beginDesignTimeLayoutActivationRequest(LayoutEvent e) {
			if(layoutDesignTimeActivationNesting == 0) {
				object result = eventManager.Event(new LayoutEvent(this, "begin-design-time-layout-activation"));

				if(result == null || !(result is bool))
					e.Info = false;
				else
					e.Info = result;

				layoutDesignTimeActivationNesting++;
			}
			else
				layoutDesignTimeActivationNesting++;		// nested request
		}

		[LayoutEvent("end-design-time-layout-activation-request")]
		private void endDesignTimeLayoutActivationRequest(LayoutEvent e) {
			if(layoutDesignTimeActivationNesting <= 0)
				throw new ApplicationException("unbalanced enter/exit design time layout activation");

			if(--layoutDesignTimeActivationNesting == 0)
				eventManager.Event(new LayoutEvent(this, "end-design-time-layout-activation"));
		}

		#region Menu Editing functions

		/// <summary>
		/// Apply the setting (do) to menu items based on instructions in a provided table
		/// </summary>
		/// <param name="menuSetting">Instructions for the menu item setting</param>
		private void doUIsetting(UIsettingEntry[] uiSetting) {
			foreach(UIsettingEntry aEntry in uiSetting) {
				if(aEntry is MenuSettingEntry) {
					MenuSettingEntry	entry = (MenuSettingEntry)aEntry;

					if(entry.Setting == UIitemSetting.Hidden)
						entry.MenuItem.Visible = false;
					else if(entry.Setting == UIitemSetting.Disabled)
						entry.MenuItem.Enabled = false;
				}
				else if(aEntry is ToolbarSettingEntry) {
					ToolbarSettingEntry	entry = (ToolbarSettingEntry)aEntry;

					if(entry.Setting == UIitemSetting.Hidden)
						entry.Button.Visible = false;
					else if(entry.Setting == UIitemSetting.Disabled)
						entry.Button.Enabled = false;
				}
			}
		}

		/// <summary>
		/// Undo the setting to menu items. The setting are described in the passed table
		/// </summary>
		/// <param name="menuSetting"0>Instructions for the menu item setting</param>
		private void undoUIsetting(UIsettingEntry[] uiSetting) {
			foreach(UIsettingEntry aEntry in uiSetting) {
				if(aEntry is MenuSettingEntry) {
					MenuSettingEntry	entry = (MenuSettingEntry)aEntry;

					if(entry.Setting == UIitemSetting.Hidden)
						entry.MenuItem.Visible = true;
					else if(entry.Setting == UIitemSetting.Disabled)
						entry.MenuItem.Enabled = true;
				}
				else if(aEntry is ToolbarSettingEntry) {
					ToolbarSettingEntry	entry = (ToolbarSettingEntry)aEntry;

					if(entry.Setting == UIitemSetting.Hidden)
						entry.Button.Visible = true;
					else if(entry.Setting == UIitemSetting.Disabled)
						entry.Button.Enabled = true;
				}
			}
		}

		#endregion

		#region Layout saving/loading

		void writeViewsXml(XmlWriter w) {
			w.WriteStartElement("Views");

			foreach(LayoutFrameWindowAreaTabPage areaTabPage in tabAreas.TabPages) {
				foreach(LayoutFrameWindowAreaViewTabPage viewTabPage in areaTabPage.TabViews.TabPages) {
					viewTabPage.WriteXml(w);
				}
			}

			w.WriteEndElement();
		}

		// On entry <AreaTabs>
		void ReadViewsXml(XmlReader r) {
			if(r.IsEmptyElement)
				r.Read();
			else {
				r.Read();		// <AreaTabs>

				while(r.NodeType == XmlNodeType.Element) {
					if(r.Name == "View") {
						LayoutModelArea	area = model.Areas[new Guid(XmlConvert.DecodeName(r.GetAttribute("AreaID")))];
						String			name = XmlConvert.DecodeName(r.GetAttribute("Name"));
						LayoutView		view = new LayoutView();

						r.Read();
						view.ReadXml(r);

						r.Read();		// </View>

						AddViewToArea(area, name, view);
					}
					else
						r.Skip();
				}
			}

			r.Read();
		}

		void writeXmlDocument(String filename) {
			XmlTextWriter	w = new XmlTextWriter(filename, new System.Text.UTF8Encoding());
			LayoutModelArea[]	areas = new LayoutModelArea[tabAreas.TabPages.Count];

			for(int i = 0; i < tabAreas.TabPages.Count; i++)
				areas[i] = ((LayoutFrameWindowAreaTabPage)tabAreas.TabPages[i]).Area;

			w.WriteStartDocument();
			w.WriteStartElement("LayoutManager");
			model.WriteXml(w, areas);
			writeViewsXml(w);
			w.WriteEndElement();
			w.WriteEndDocument();

			w.Close();
		}

		void readXmlDocument(String filename) {
			XmlTextReader	r = new LayoutXmlTextReader(filename, Model, LayoutReadXmlContext.LoadingModel);

			r.WhitespaceHandling = WhitespaceHandling.None;
			tabAreas.TabPages.Clear();		// Remove all areas

			// Skip all kind of declaration stuff
			while(r.Read() && r.NodeType == XmlNodeType.XmlDeclaration)
				;

			r.Read();		// <LayoutManager>

			while(r.NodeType == XmlNodeType.Element) {
				if(r.Name == "LayoutModel")
					model.ReadXml(r);
				else if(r.Name == "Views")
					ReadViewsXml(r);
				else
					r.Skip();
			}

			r.Close();
		}

		/// <summary>
		/// Save layout to a file.
		/// </summary>
		/// <param name="saveAs">if true, the user is prompted for filename</param>
		bool saveLayout(bool saveAs) {
			String		saveFilename = layoutFilename;
			bool		needToSave = true;
			bool		layoutSaved = false;

			if(saveAs || layoutFilename == null) {
				using(SaveFileDialog sf = new SaveFileDialog()) {
					sf.DefaultExt = ".Layout";
					sf.Filter = fileTypeFilter;
					sf.AddExtension = true;
					sf.CheckPathExists = true;
					sf.OverwritePrompt = true;
					if(sf.ShowDialog() == DialogResult.OK)
						saveFilename = sf.FileName;
					else
						needToSave = false;
				}
			}

			if(needToSave) {
				try {
					writeXmlDocument(saveFilename);

					if(layoutFilename == null) {
						layoutFilename = saveFilename;
						updateFormTitle();
						eventManager.Event(new LayoutEvent(model, "new-layout-document", null, layoutFilename));
					}

					commandManager.ChangeLevel = 0;				// Layout saved
					layoutSaved = true;
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message, "Error while saving layout", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				}
			}

			return layoutSaved;
		}

		void openLayout(string filename) {
			model.Clear();
			readXmlDocument(filename);

#if LAYERS
					initializeLayerSelector();
#else
			ActiveLayer = model.Layers[0];
#endif
			layoutFilename = filename;

			updateFormTitle();
			model.StateManager.Load();
			eventManager.Event(new LayoutEvent(model, "new-layout-document", null, layoutFilename));
		}

		/// <summary>
		/// Open layout file and deseriale its content
		/// </summary>
		void openLayout() {
			if(commandManager.ChangeLevel != 0) {
				saveLayout(false);
				commandManager.Clear();
			}

			OpenFileDialog	of = new OpenFileDialog();

			of.AddExtension = true;
			of.CheckFileExists = true;
			of.DefaultExt = ".layout";
			of.Filter = fileTypeFilter;
			
			if(of.ShowDialog() == DialogResult.OK) {
				try {
					openLayout(of.FileName);
				} catch(Exception ex) {
					MessageBox.Show(ex.Message, "Error while loading layout", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				}
			}

			of.Dispose();
		}

		#endregion

		#region Application state persistence management

		private void saveApplicationState() {
			ApplicationStateInfo	applicationState = new ApplicationStateInfo();

			applicationState.LayoutFilename = layoutFilename;
			applicationState.WindowState = WindowState;
			applicationState.Bounds = DesktopBounds;
			applicationState.State = IsOperationMode ? ApplicationStateInfo.LayoutState.Operation : ApplicationStateInfo.LayoutState.Design;

			applicationState.Element.OwnerDocument.Save(CommonDataFolderName + Path.DirectorySeparatorChar + "ApplicationState.xml");
		}

		#endregion

		#region Layer management

#if LAYERS
		void initializeLayerSelector() {
			comboLayers.Items.Clear();
			comboLayers.Items.AddRange(model.Layers);
			comboLayers.SelectedIndex = 0;
			this.ActiveLayer = (LayoutModelLayer)comboLayers.SelectedItem;
		}
#endif

		#endregion

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayoutController));
			this.toolBarMain = new System.Windows.Forms.ToolBar();
			this.toolBarButtonOpenLayiout = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonSaveLayout = new System.Windows.Forms.ToolBarButton();
			this.Sep1 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonDesignMode = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonOperationMode = new System.Windows.Forms.ToolBarButton();
			this.sep2 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonShowLayoutControl = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonShowTripsMonitor = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonShowMessages = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonShowLocomotives = new System.Windows.Forms.ToolBarButton();
			this.sep3 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonToggleAllLayoutManualDispatch = new System.Windows.Forms.ToolBarButton();
			this.sep4 = new System.Windows.Forms.ToolBarButton();
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
			this.menuItem7 = new System.Windows.Forms.MenuItem();
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
			this.menuItemSeperator = new System.Windows.Forms.MenuItem();
			this.menuItemCompile = new System.Windows.Forms.MenuItem();
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
			this.menuItemNewLayout = new System.Windows.Forms.MenuItem();
			this.menuItemOpen = new System.Windows.Forms.MenuItem();
			this.menuItemSave = new System.Windows.Forms.MenuItem();
			this.menuSaveAs = new System.Windows.Forms.MenuItem();
			this.menuItemSeperator1 = new System.Windows.Forms.MenuItem();
			this.menuItemModules = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItemExit = new System.Windows.Forms.MenuItem();
			this.menuEdit = new System.Windows.Forms.MenuItem();
			this.menuItemUndo = new System.Windows.Forms.MenuItem();
			this.menuItemRedo = new System.Windows.Forms.MenuItem();
			this.menuItemSeparator = new System.Windows.Forms.MenuItem();
			this.menuItemCopy = new System.Windows.Forms.MenuItem();
			this.menuItemCut = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
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
            this.sep2,
            this.toolBarButtonShowLayoutControl,
            this.toolBarButtonShowTripsMonitor,
            this.toolBarButtonShowMessages,
            this.toolBarButtonShowLocomotives,
            this.sep3,
            this.toolBarButtonToggleAllLayoutManualDispatch,
            this.sep4,
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
// 
// toolBarButtonSaveLayout
// 
			resources.ApplyResources(this.toolBarButtonSaveLayout, "toolBarButtonSaveLayout");
// 
// Sep1
// 
			resources.ApplyResources(this.Sep1, "Sep1");
// 
// toolBarButtonDesignMode
// 
			resources.ApplyResources(this.toolBarButtonDesignMode, "toolBarButtonDesignMode");
// 
// toolBarButtonOperationMode
// 
			resources.ApplyResources(this.toolBarButtonOperationMode, "toolBarButtonOperationMode");
// 
// sep2
// 
			resources.ApplyResources(this.sep2, "sep2");
// 
// toolBarButtonShowLayoutControl
// 
			resources.ApplyResources(this.toolBarButtonShowLayoutControl, "toolBarButtonShowLayoutControl");
// 
// toolBarButtonShowTripsMonitor
// 
			resources.ApplyResources(this.toolBarButtonShowTripsMonitor, "toolBarButtonShowTripsMonitor");
// 
// toolBarButtonShowMessages
// 
			resources.ApplyResources(this.toolBarButtonShowMessages, "toolBarButtonShowMessages");
// 
// toolBarButtonShowLocomotives
// 
			resources.ApplyResources(this.toolBarButtonShowLocomotives, "toolBarButtonShowLocomotives");
// 
// sep3
// 
			resources.ApplyResources(this.sep3, "sep3");
// 
// toolBarButtonToggleAllLayoutManualDispatch
// 
			resources.ApplyResources(this.toolBarButtonToggleAllLayoutManualDispatch, "toolBarButtonToggleAllLayoutManualDispatch");
// 
// sep4
// 
			resources.ApplyResources(this.sep4, "sep4");
// 
// toolBarButtonStopAllLocomotives
// 
			resources.ApplyResources(this.toolBarButtonStopAllLocomotives, "toolBarButtonStopAllLocomotives");
// 
// toolBarButtonEmergencyStop
// 
			resources.ApplyResources(this.toolBarButtonEmergencyStop, "toolBarButtonEmergencyStop");
// 
// imageListTools
// 
			resources.ApplyResources(this.imageListTools, "imageListTools");
			this.imageListTools.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTools.ImageStream")));
			this.imageListTools.TransparentColor = System.Drawing.Color.Transparent;
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
			resources.ApplyResources(this.statusBarPanelOperation, "statusBarPanelOperation");
// 
// statusBarPanelMode
// 
			resources.ApplyResources(this.statusBarPanelMode, "statusBarPanelMode");
// 
// menuItem2
// 
			this.menuItem2.Index = -1;
			this.menuItem2.Name = "menuItem2";
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
            this.menuItem7,
            this.menuItemShowLayoutControl,
            this.menuItemShowTripsMonitor,
            this.menuItemShowMessages,
            this.menuItemShowLocomotives,
            this.menuSeperator4,
            this.menuItemViewArrage});
			this.menuItemView.Name = "menuItemView";
			resources.ApplyResources(this.menuItemView, "menuItemView");
			this.menuItemView.Popup += new System.EventHandler(this.menuItemView_Popup);
// 
// menuItemViewNew
// 
			this.menuItemViewNew.Index = 0;
			this.menuItemViewNew.Name = "menuItemViewNew";
			resources.ApplyResources(this.menuItemViewNew, "menuItemViewNew");
			this.menuItemViewNew.Click += new System.EventHandler(this.menuItemViewNew_Click);
// 
// menuItemViewDelete
// 
			this.menuItemViewDelete.Index = 1;
			this.menuItemViewDelete.Name = "menuItemViewDelete";
			resources.ApplyResources(this.menuItemViewDelete, "menuItemViewDelete");
			this.menuItemViewDelete.Click += new System.EventHandler(this.menuItemViewDelete_Click);
// 
// menuItemViewRename
// 
			this.menuItemViewRename.Index = 2;
			this.menuItemViewRename.Name = "menuItemViewRename";
			resources.ApplyResources(this.menuItemViewRename, "menuItemViewRename");
			this.menuItemViewRename.Click += new System.EventHandler(this.menuItemViewRename_Click);
// 
// menuItemSeperator6
// 
			this.menuItemSeperator6.Index = 3;
			this.menuItemSeperator6.Name = "menuItemSeperator6";
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
			this.menuItemZoomMenu.Name = "menuItemZoomMenu";
			resources.ApplyResources(this.menuItemZoomMenu, "menuItemZoomMenu");
			this.menuItemZoomMenu.Popup += new System.EventHandler(this.menuItemZoomMenu_Popup);
// 
// menuItemSetZoom
// 
			this.menuItemSetZoom.Index = 0;
			this.menuItemSetZoom.Name = "menuItemSetZoom";
			this.menuItemSetZoom.RadioCheck = true;
			resources.ApplyResources(this.menuItemSetZoom, "menuItemSetZoom");
			this.menuItemSetZoom.Click += new System.EventHandler(this.menuItemSetZoom_Click);
// 
// menuItem5
// 
			this.menuItem5.Index = 1;
			this.menuItem5.Name = "menuItem5";
			resources.ApplyResources(this.menuItem5, "menuItem5");
// 
// menuItemZoom200
// 
			this.menuItemZoom200.Index = 2;
			this.menuItemZoom200.Name = "menuItemZoom200";
			this.menuItemZoom200.RadioCheck = true;
			resources.ApplyResources(this.menuItemZoom200, "menuItemZoom200");
			this.menuItemZoom200.Click += new System.EventHandler(this.menuItemZoomPreset_Click);
// 
// menuItemZoom150
// 
			this.menuItemZoom150.Index = 3;
			this.menuItemZoom150.Name = "menuItemZoom150";
			this.menuItemZoom150.RadioCheck = true;
			resources.ApplyResources(this.menuItemZoom150, "menuItemZoom150");
			this.menuItemZoom150.Click += new System.EventHandler(this.menuItemZoomPreset_Click);
// 
// menuItemZoom100
// 
			this.menuItemZoom100.Index = 4;
			this.menuItemZoom100.Name = "menuItemZoom100";
			this.menuItemZoom100.RadioCheck = true;
			resources.ApplyResources(this.menuItemZoom100, "menuItemZoom100");
			this.menuItemZoom100.Click += new System.EventHandler(this.menuItemZoomPreset_Click);
// 
// menuItemZoom75
// 
			this.menuItemZoom75.Index = 5;
			this.menuItemZoom75.Name = "menuItemZoom75";
			this.menuItemZoom75.RadioCheck = true;
			resources.ApplyResources(this.menuItemZoom75, "menuItemZoom75");
			this.menuItemZoom75.Click += new System.EventHandler(this.menuItemZoomPreset_Click);
// 
// menuItemZoom50
// 
			this.menuItemZoom50.Index = 6;
			this.menuItemZoom50.Name = "menuItemZoom50";
			this.menuItemZoom50.RadioCheck = true;
			resources.ApplyResources(this.menuItemZoom50, "menuItemZoom50");
			this.menuItemZoom50.Click += new System.EventHandler(this.menuItemZoomPreset_Click);
// 
// menuItemZoom25
// 
			this.menuItemZoom25.Index = 7;
			this.menuItemZoom25.Name = "menuItemZoom25";
			this.menuItemZoom25.RadioCheck = true;
			resources.ApplyResources(this.menuItemZoom25, "menuItemZoom25");
			this.menuItemZoom25.Click += new System.EventHandler(this.menuItemZoomPreset_Click);
// 
// menuItem12
// 
			this.menuItem12.Index = 8;
			this.menuItem12.Name = "menuItem12";
			resources.ApplyResources(this.menuItem12, "menuItem12");
// 
// menuItem1ZoomAllArea
// 
			this.menuItem1ZoomAllArea.Index = 9;
			this.menuItem1ZoomAllArea.Name = "menuItem1ZoomAllArea";
			resources.ApplyResources(this.menuItem1ZoomAllArea, "menuItem1ZoomAllArea");
			this.menuItem1ZoomAllArea.Click += new System.EventHandler(this.menuItemZoomAllArea_Click);
// 
// menuItemViewGrid
// 
			this.menuItemViewGrid.Checked = true;
			this.menuItemViewGrid.Index = 5;
			this.menuItemViewGrid.Name = "menuItemViewGrid";
			resources.ApplyResources(this.menuItemViewGrid, "menuItemViewGrid");
			this.menuItemViewGrid.Click += new System.EventHandler(this.menuItemViewGrid_Click);
// 
// menuItem7
// 
			this.menuItem7.Index = 6;
			this.menuItem7.Name = "menuItem7";
			resources.ApplyResources(this.menuItem7, "menuItem7");
// 
// menuItemShowLayoutControl
// 
			this.menuItemShowLayoutControl.Index = 7;
			this.menuItemShowLayoutControl.Name = "menuItemShowLayoutControl";
			resources.ApplyResources(this.menuItemShowLayoutControl, "menuItemShowLayoutControl");
			this.menuItemShowLayoutControl.Click += new System.EventHandler(this.menuItemShowLayoutControl_Click);
// 
// menuItemShowTripsMonitor
// 
			this.menuItemShowTripsMonitor.Index = 8;
			this.menuItemShowTripsMonitor.Name = "menuItemShowTripsMonitor";
			resources.ApplyResources(this.menuItemShowTripsMonitor, "menuItemShowTripsMonitor");
			this.menuItemShowTripsMonitor.Click += new System.EventHandler(this.menuItemShowTripsMonitor_Click);
// 
// menuItemShowMessages
// 
			this.menuItemShowMessages.Index = 9;
			this.menuItemShowMessages.Name = "menuItemShowMessages";
			resources.ApplyResources(this.menuItemShowMessages, "menuItemShowMessages");
			this.menuItemShowMessages.Click += new System.EventHandler(this.menuItemShowMessages_Click);
// 
// menuItemShowLocomotives
// 
			this.menuItemShowLocomotives.Index = 10;
			this.menuItemShowLocomotives.Name = "menuItemShowLocomotives";
			resources.ApplyResources(this.menuItemShowLocomotives, "menuItemShowLocomotives");
			this.menuItemShowLocomotives.Click += new System.EventHandler(this.menuItemShowLocomotives_Click);
// 
// menuSeperator4
// 
			this.menuSeperator4.Index = 11;
			this.menuSeperator4.Name = "menuSeperator4";
			resources.ApplyResources(this.menuSeperator4, "menuSeperator4");
// 
// menuItemViewArrage
// 
			this.menuItemViewArrage.Index = 12;
			this.menuItemViewArrage.Name = "menuItemViewArrage";
			resources.ApplyResources(this.menuItemViewArrage, "menuItemViewArrage");
			this.menuItemViewArrage.Click += new System.EventHandler(this.menuItemViewArrage_Click);
// 
// menuItemOperational
// 
			this.menuItemOperational.Index = 1;
			this.menuItemOperational.Name = "menuItemOperational";
			resources.ApplyResources(this.menuItemOperational, "menuItemOperational");
			this.menuItemOperational.Click += new System.EventHandler(this.menuItemOperational_Click);
// 
// menuItemTools
// 
			this.menuItemTools.Index = 5;
			this.menuItemTools.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
				this.menuItemDummy
			});
			this.menuItemTools.Name = "menuItemTools";
			resources.ApplyResources(this.menuItemTools, "menuItemTools");
			this.menuItemTools.Popup += new System.EventHandler(this.menuItemTools_Popup);
// 
// menuItemDummy
// 
			this.menuItemDummy.Index = 0;
			this.menuItemDummy.Name = "menuItemDummy";
			resources.ApplyResources(this.menuItemDummy, "menuItemDummy");
// 
// menuLayout
// 
			this.menuLayout.Index = 4;
			this.menuLayout.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemDesign,
            this.menuItemOperational,
            this.menuItemSeperator,
            this.menuItemCompile,
            this.menuItem8,
            this.menuItemLearnLayout,
            this.menuItemConnectLayout,
            this.menuItemEmergencyStop,
            this.menuItemSuspendLocomotives,
            this.menuItem6,
            this.menuItemPolicies,
            this.menuItemDefaultDriverParameters,
            this.menuItemCommonDestinations,
            this.menuItemManualDispatchRegions});
			this.menuLayout.Name = "menuLayout";
			resources.ApplyResources(this.menuLayout, "menuLayout");
			this.menuLayout.Popup += new System.EventHandler(this.menuLayout_Popup);
// 
// menuItemDesign
// 
			this.menuItemDesign.Index = 0;
			this.menuItemDesign.Name = "menuItemDesign";
			resources.ApplyResources(this.menuItemDesign, "menuItemDesign");
			this.menuItemDesign.Click += new System.EventHandler(this.menuItemDesign_Click);
// 
// menuItemSeperator
// 
			this.menuItemSeperator.Index = 2;
			this.menuItemSeperator.Name = "menuItemSeperator2";
			resources.ApplyResources(this.menuItemSeperator, "menuItemSeperator");
// 
// menuItemCompile
// 
			this.menuItemCompile.Index = 3;
			this.menuItemCompile.Name = "menuItemCompile";
			resources.ApplyResources(this.menuItemCompile, "menuItemCompile");
			this.menuItemCompile.Click += new System.EventHandler(this.menuItemCompile_Click);
// 
// menuItem8
// 
			this.menuItem8.Index = 4;
			this.menuItem8.Name = "menuItem8";
			resources.ApplyResources(this.menuItem8, "menuItem8");
// 
// menuItemLearnLayout
// 
			this.menuItemLearnLayout.Index = 5;
			this.menuItemLearnLayout.Name = "menuItemLearnLayout";
			resources.ApplyResources(this.menuItemLearnLayout, "menuItemLearnLayout");
			this.menuItemLearnLayout.Click += new System.EventHandler(this.menuItemLearnLayout_Click);
// 
// menuItemConnectLayout
// 
			this.menuItemConnectLayout.Index = 6;
			this.menuItemConnectLayout.Name = "menuItemConnectLayout";
			resources.ApplyResources(this.menuItemConnectLayout, "menuItemConnectLayout");
			this.menuItemConnectLayout.Click += new System.EventHandler(this.menuItemConnectLayout_Click);
// 
// menuItemEmergencyStop
// 
			this.menuItemEmergencyStop.Index = 7;
			this.menuItemEmergencyStop.Name = "menuItemEmergencyStop";
			resources.ApplyResources(this.menuItemEmergencyStop, "menuItemEmergencyStop");
			this.menuItemEmergencyStop.Click += new System.EventHandler(this.menuItemEmergencyStop_Click);
// 
// menuItemSuspendLocomotives
// 
			this.menuItemSuspendLocomotives.Index = 8;
			this.menuItemSuspendLocomotives.Name = "menuItemSuspendLocomotives";
			resources.ApplyResources(this.menuItemSuspendLocomotives, "menuItemSuspendLocomotives");
			this.menuItemSuspendLocomotives.Click += new System.EventHandler(this.menuItemSuspendLocomotives_Click);
// 
// menuItem6
// 
			this.menuItem6.Index = 9;
			this.menuItem6.Name = "menuItem6";
			resources.ApplyResources(this.menuItem6, "menuItem6");
// 
// menuItemPolicies
// 
			this.menuItemPolicies.Index = 10;
			this.menuItemPolicies.Name = "menuItemPolicies";
			resources.ApplyResources(this.menuItemPolicies, "menuItemPolicies");
			this.menuItemPolicies.Click += new System.EventHandler(this.menuItemPolicies_Click);
// 
// menuItemDefaultDriverParameters
// 
			this.menuItemDefaultDriverParameters.Index = 11;
			this.menuItemDefaultDriverParameters.Name = "menuItemDefaultDriverParameters";
			resources.ApplyResources(this.menuItemDefaultDriverParameters, "menuItemDefaultDriverParameters");
			this.menuItemDefaultDriverParameters.Click += new System.EventHandler(this.menuItemDefaultDriverParameters_Click);
// 
// menuItemCommonDestinations
// 
			this.menuItemCommonDestinations.Index = 12;
			this.menuItemCommonDestinations.Name = "menuItemCommonDestinations";
			resources.ApplyResources(this.menuItemCommonDestinations, "menuItemCommonDestinations");
			this.menuItemCommonDestinations.Click += new System.EventHandler(this.menuItemCommonDestinations_Click);
// 
// menuItemManualDispatchRegions
// 
			this.menuItemManualDispatchRegions.Index = 13;
			this.menuItemManualDispatchRegions.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
				this.menuItem4
			});
			this.menuItemManualDispatchRegions.Name = "menuItemManualDispatchRegions";
			resources.ApplyResources(this.menuItemManualDispatchRegions, "menuItemManualDispatchRegions");
			this.menuItemManualDispatchRegions.Popup += new System.EventHandler(this.menuItemManualDispatchRegions_Popup);
// 
// menuItem4
// 
			this.menuItem4.Index = 0;
			this.menuItem4.Name = "menuItem4";
			resources.ApplyResources(this.menuItem4, "menuItem4");
// 
// menuFile
// 
			this.menuFile.Index = 0;
			this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemNewLayout,
            this.menuItemOpen,
            this.menuItemSave,
            this.menuSaveAs,
            this.menuItemSeperator1,
            this.menuItemModules,
            this.menuItem3,
            this.menuItemExit});
			this.menuFile.Name = "menuFile";
			resources.ApplyResources(this.menuFile, "menuFile");
// 
// menuItemNewLayout
// 
			this.menuItemNewLayout.Index = 0;
			this.menuItemNewLayout.Name = "menuItemNewLayout";
			resources.ApplyResources(this.menuItemNewLayout, "menuItemNewLayout");
			this.menuItemNewLayout.Click += new System.EventHandler(this.menuItemNewLayout_Click);
// 
// menuItemOpen
// 
			this.menuItemOpen.Index = 1;
			this.menuItemOpen.Name = "menuItemOpen";
			resources.ApplyResources(this.menuItemOpen, "menuItemOpen");
			this.menuItemOpen.Click += new System.EventHandler(this.menuItemOpen_Click);
// 
// menuItemSave
// 
			this.menuItemSave.Index = 2;
			this.menuItemSave.Name = "menuItemSave";
			resources.ApplyResources(this.menuItemSave, "menuItemSave");
			this.menuItemSave.Click += new System.EventHandler(this.menuItemSave_Click);
// 
// menuSaveAs
// 
			this.menuSaveAs.Index = 3;
			this.menuSaveAs.Name = "menuSaveAs";
			resources.ApplyResources(this.menuSaveAs, "menuSaveAs");
			this.menuSaveAs.Click += new System.EventHandler(this.menuSaveAs_Click);
// 
// menuItemSeperator1
// 
			this.menuItemSeperator1.Index = 4;
			this.menuItemSeperator1.Name = "menuItemSeperator1";
			resources.ApplyResources(this.menuItemSeperator1, "menuItemSeperator1");
// 
// menuItemModules
// 
			this.menuItemModules.Index = 5;
			this.menuItemModules.Name = "menuItemModules";
			resources.ApplyResources(this.menuItemModules, "menuItemModules");
			this.menuItemModules.Click += new System.EventHandler(this.menuItemModules_Click);
// 
// menuItem3
// 
			this.menuItem3.Index = 6;
			this.menuItem3.Name = "menuItem3";
			resources.ApplyResources(this.menuItem3, "menuItem3");
// 
// menuItemExit
// 
			this.menuItemExit.Index = 7;
			this.menuItemExit.Name = "menuItemExit";
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
            this.menuItem1,
            this.menuItemStyles,
            this.menuItemLocomotiveCatalog,
            this.menuItemAccelerationProfiles});
			this.menuEdit.Name = "menuEdit";
			resources.ApplyResources(this.menuEdit, "menuEdit");
			this.menuEdit.Popup += new System.EventHandler(this.menuEdit_Popup);
// 
// menuItemUndo
// 
			this.menuItemUndo.Index = 0;
			this.menuItemUndo.Name = "menuItemUndo";
			resources.ApplyResources(this.menuItemUndo, "menuItemUndo");
			this.menuItemUndo.Click += new System.EventHandler(this.menuItemUndo_Click);
// 
// menuItemRedo
// 
			this.menuItemRedo.Index = 1;
			this.menuItemRedo.Name = "menuItemRedo";
			resources.ApplyResources(this.menuItemRedo, "menuItemRedo");
			this.menuItemRedo.Click += new System.EventHandler(this.menuItemRedo_Click);
// 
// menuItemSeparator
// 
			this.menuItemSeparator.Index = 2;
			this.menuItemSeparator.Name = "menuItemSeparator";
			resources.ApplyResources(this.menuItemSeparator, "menuItemSeparator");
// 
// menuItemCopy
// 
			this.menuItemCopy.Index = 3;
			this.menuItemCopy.Name = "menuItemCopy";
			resources.ApplyResources(this.menuItemCopy, "menuItemCopy");
			this.menuItemCopy.Click += new System.EventHandler(this.menuItemCopy_Click);
// 
// menuItemCut
// 
			this.menuItemCut.Index = 4;
			this.menuItemCut.Name = "menuItemCut";
			resources.ApplyResources(this.menuItemCut, "menuItemCut");
			this.menuItemCut.Click += new System.EventHandler(this.menuItemCut_Click);
// 
// menuItem1
// 
			this.menuItem1.Index = 5;
			this.menuItem1.Name = "menuItem1";
			resources.ApplyResources(this.menuItem1, "menuItem1");
// 
// menuItemStyles
// 
			this.menuItemStyles.Index = 6;
			this.menuItemStyles.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemStyleFonts,
            this.menuItemStylePositions});
			this.menuItemStyles.Name = "menuItemStyles";
			resources.ApplyResources(this.menuItemStyles, "menuItemStyles");
// 
// menuItemStyleFonts
// 
			this.menuItemStyleFonts.Index = 0;
			this.menuItemStyleFonts.Name = "menuItemStyleFonts";
			resources.ApplyResources(this.menuItemStyleFonts, "menuItemStyleFonts");
			this.menuItemStyleFonts.Click += new System.EventHandler(this.menuItemStyleFonts_Click);
// 
// menuItemStylePositions
// 
			this.menuItemStylePositions.Index = 1;
			this.menuItemStylePositions.Name = "menuItemStylePositions";
			resources.ApplyResources(this.menuItemStylePositions, "menuItemStylePositions");
			this.menuItemStylePositions.Click += new System.EventHandler(this.menuItemStylePositions_Click);
// 
// menuItemLocomotiveCatalog
// 
			this.menuItemLocomotiveCatalog.Index = 7;
			this.menuItemLocomotiveCatalog.Name = "menuItemLocomotiveCatalog";
			resources.ApplyResources(this.menuItemLocomotiveCatalog, "menuItemLocomotiveCatalog");
			this.menuItemLocomotiveCatalog.Click += new System.EventHandler(this.menuItemLocomotiveCatalog_Click);
// 
// menuItemAccelerationProfiles
// 
			this.menuItemAccelerationProfiles.Index = 8;
			this.menuItemAccelerationProfiles.Name = "menuItemAccelerationProfiles";
			resources.ApplyResources(this.menuItemAccelerationProfiles, "menuItemAccelerationProfiles");
			this.menuItemAccelerationProfiles.Click += new System.EventHandler(this.menuItemAccelerationProfiles_Click);
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
			this.menuItemArea.Name = "menuItemArea";
			resources.ApplyResources(this.menuItemArea, "menuItemArea");
// 
// menuItemNewArea
// 
			this.menuItemNewArea.Index = 0;
			this.menuItemNewArea.Name = "menuItemNewArea";
			resources.ApplyResources(this.menuItemNewArea, "menuItemNewArea");
			this.menuItemNewArea.Click += new System.EventHandler(this.menuItemNewArea_Click);
// 
// menuItemDeleteArea
// 
			this.menuItemDeleteArea.Index = 1;
			this.menuItemDeleteArea.Name = "menuItemDeleteArea";
			resources.ApplyResources(this.menuItemDeleteArea, "menuItemDeleteArea");
			this.menuItemDeleteArea.Click += new System.EventHandler(this.menuItemDeleteArea_Click);
// 
// menuItemRenameArea
// 
			this.menuItemRenameArea.Index = 2;
			this.menuItemRenameArea.Name = "menuItemRenameArea";
			resources.ApplyResources(this.menuItemRenameArea, "menuItemRenameArea");
			this.menuItemRenameArea.Click += new System.EventHandler(this.menuItemRenameArea_Click);
// 
// menuSeperator2
// 
			this.menuSeperator2.Index = 3;
			this.menuSeperator2.Name = "menuSeperator2";
			resources.ApplyResources(this.menuSeperator2, "menuSeperator2");
// 
// menuItemAreaArrange
// 
			this.menuItemAreaArrange.Index = 4;
			this.menuItemAreaArrange.Name = "menuItemAreaArrange";
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
			this.splitterTripsMonitor.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitterTripsMonitor_SplitterMoved);
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
			this.tripsMonitor.AccessibleDescription = resources.GetString("tripsMonitor.AccessibleDescription");
			this.tripsMonitor.AccessibleName = resources.GetString("tripsMonitor.AccessibleName");
			this.tripsMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tripsMonitor.Anchor")));
			this.tripsMonitor.AutoScroll = ((bool)(resources.GetObject("tripsMonitor.AutoScroll")));
			this.tripsMonitor.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tripsMonitor.AutoScrollMargin")));
			this.tripsMonitor.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tripsMonitor.AutoScrollMinSize")));
			this.tripsMonitor.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tripsMonitor.BackgroundImage")));
			this.tripsMonitor.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tripsMonitor.Dock")));
			this.tripsMonitor.Enabled = ((bool)(resources.GetObject("tripsMonitor.Enabled")));
			this.tripsMonitor.EventManager = null;
			this.tripsMonitor.Font = ((System.Drawing.Font)(resources.GetObject("tripsMonitor.Font")));
			this.tripsMonitor.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tripsMonitor.ImeMode")));
			this.tripsMonitor.Location = ((System.Drawing.Point)(resources.GetObject("tripsMonitor.Location")));
			this.tripsMonitor.Name = "tripsMonitor";
			this.tripsMonitor.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tripsMonitor.RightToLeft")));
			this.tripsMonitor.Size = ((System.Drawing.Size)(resources.GetObject("tripsMonitor.Size")));
			this.tripsMonitor.TabIndex = ((int)(resources.GetObject("tripsMonitor.TabIndex")));
			this.tripsMonitor.Visible = ((bool)(resources.GetObject("tripsMonitor.Visible")));
// 
// splitterMessages
// 
			this.splitterMessages.BackColor = System.Drawing.SystemColors.WindowFrame;
			resources.ApplyResources(this.splitterMessages, "splitterMessages");
			this.splitterMessages.Name = "splitterMessages";
			this.splitterMessages.TabStop = false;
			this.splitterMessages.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitterMessages_SplitterMoved);
// 
// messageViewer
// 
			resources.ApplyResources(this.messageViewer, "messageViewer");
			this.messageViewer.EventManager = null;
			this.messageViewer.Name = "messageViewer";
// 
// splitterLocomotives
// 
			this.splitterLocomotives.BackColor = System.Drawing.SystemColors.WindowText;
			resources.ApplyResources(this.splitterLocomotives, "splitterLocomotives");
			this.splitterLocomotives.Name = "splitterLocomotives";
			this.splitterLocomotives.TabStop = false;
			this.splitterLocomotives.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitterLocomotives_SplitterMoved);
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
			this.locomotivesViewer.EventManager = null;
			this.locomotivesViewer.LocomotiveCollection = null;
			this.locomotivesViewer.Name = "locomotivesViewer";
			this.locomotivesViewer.StateManager = null;
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
			this.layoutControlViewer.AccessibleDescription = resources.GetString("layoutControlViewer.AccessibleDescription");
			this.layoutControlViewer.AccessibleName = resources.GetString("layoutControlViewer.AccessibleName");
			this.layoutControlViewer.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("layoutControlViewer.Anchor")));
			this.layoutControlViewer.AutoScroll = ((bool)(resources.GetObject("layoutControlViewer.AutoScroll")));
			this.layoutControlViewer.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("layoutControlViewer.AutoScrollMargin")));
			this.layoutControlViewer.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("layoutControlViewer.AutoScrollMinSize")));
			this.layoutControlViewer.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("layoutControlViewer.BackgroundImage")));
			this.layoutControlViewer.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("layoutControlViewer.Dock")));
			this.layoutControlViewer.Enabled = ((bool)(resources.GetObject("layoutControlViewer.Enabled")));
			this.layoutControlViewer.EventManager = null;
			this.layoutControlViewer.Font = ((System.Drawing.Font)(resources.GetObject("layoutControlViewer.Font")));
			this.layoutControlViewer.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("layoutControlViewer.ImeMode")));
			this.layoutControlViewer.Location = ((System.Drawing.Point)(resources.GetObject("layoutControlViewer.Location")));
			this.layoutControlViewer.Name = "layoutControlViewer";
			this.layoutControlViewer.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("layoutControlViewer.RightToLeft")));
			this.layoutControlViewer.Size = ((System.Drawing.Size)(resources.GetObject("layoutControlViewer.Size")));
			this.layoutControlViewer.TabIndex = ((int)(resources.GetObject("layoutControlViewer.TabIndex")));
			this.layoutControlViewer.Visible = ((bool)(resources.GetObject("layoutControlViewer.Visible")));
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
			this.mainMenu.Name = "mainMenu";
// 
// timerFreeResources
// 
			this.timerFreeResources.Interval = 30000;
			this.timerFreeResources.SynchronizingObject = this;
			this.timerFreeResources.Elapsed += new System.Timers.ElapsedEventHandler(this.timerFreeResources_Elapsed);
// 
// LayoutController
// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.panelLayoutControl);
			this.Controls.Add(this.toolBarMain);
			this.Controls.Add(this.statusBar);
			this.Menu = this.mainMenu;
			this.Name = "LayoutController";
			this.Deactivate += new System.EventHandler(this.LayoutController_Deactivate);
			this.Resize += new System.EventHandler(this.LayoutController_Resize);
			this.Activated += new System.EventHandler(this.LayoutController_Activated);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.LayoutController_Closing);
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

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new LayoutController());
		}

		#region Layout Event handlers

		[LayoutEvent("get-controller")]
		void GetController(LayoutEvent e) {
			e.Info = this;
		}

		[LayoutEvent("get-invoker")]
		void GetInvoker(LayoutEvent e) {
			e.Info = (ILayoutInvoker)this;
		}

		[LayoutEvent("get-active-layer")]
		void GetActiveLayer(LayoutEvent e) {
			e.Info = activeLayer;
		}

		[LayoutEvent("get-user-selection")]
		void GetUserSelection(LayoutEvent e) {
			e.Info = UserSelection;
		}

		[LayoutEvent("show-control-connection-point")]
		private void showControlConnectionPoint(LayoutEvent e) {
			ControlConnectionPoint	connectionPoint = (ControlConnectionPoint)e.Sender;

			EventManager.Event(new LayoutEvent(this, "show-layout-control"));
			layoutControlViewer.EnsureVisible(new ControlConnectionPointReference(connectionPoint), true);
		}

		[LayoutEvent("is-operation-mode")]
		void IsOperationModeHandler(LayoutEvent e) {
			e.Info = operationMode;
		}

		[LayoutEvent("is-design-mode")]
		void IsDesignModeHandler(LayoutEvent e) {
			e.Info = !operationMode;
		}

		[LayoutEvent("get-layout-document-name")]
		private void getLayoutDocumentName(LayoutEvent e) {
			e.Info = layoutFilename;
		}

		[LayoutEvent("get-layout-directory-name")]
		private void getLayoutDirectoryName(LayoutEvent e) {
			if(layoutFilename == null)
				e.Info = null;
			else
				e.Info = Path.GetDirectoryName(layoutFilename);
		}

		[LayoutEvent("get-layout-state-document-name")]
		private void getLayoutStateDocumentName(LayoutEvent e) {
			string	layoutFilename = (string)EventManager.Event(new LayoutEvent(this, "get-layout-document-name"));
			
			e.Info = Path.GetDirectoryName(layoutFilename) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(layoutFilename) + ".state";
		}

		[LayoutEvent("get-layout-routing-document-name")]
		private void getLayoutRoutingDocumentName(LayoutEvent e) {
			string	layoutFilename = (string)EventManager.Event(new LayoutEvent(this, "get-layout-document-name"));
			
			e.Info = Path.GetDirectoryName(layoutFilename) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(layoutFilename) + ".routing";
		}

		[LayoutEvent("ensure-component-visible")]
		private void ensureComponentVisible(LayoutEvent e) {
			ModelComponent					component = (ModelComponent)e.Sender;
			LayoutModelArea					area = component.Spot.Area;
			LayoutFrameWindowAreaTabPage	areaPage = null;
			bool							markComponent = (bool)e.Info;

			foreach(LayoutFrameWindowAreaTabPage ap in tabAreas.TabPages)
				if(ap.Area == area) {
					areaPage = ap;
					break;
				}

			if(areaPage != null) {
				LayoutFrameWindowAreaViewTabPage	bestViewPage = null;
				LayoutView.VisibleResult			bestVisibleResult = LayoutView.VisibleResult.No;

				// Figure out what view in this area shows the component best

				foreach(LayoutFrameWindowAreaViewTabPage vp in areaPage.TabViews.TabPages) {
					LayoutView.VisibleResult	visibleResult = vp.View.IsComponentVisible(component);

					if((int)visibleResult > (int)bestVisibleResult) {
						bestVisibleResult = visibleResult;
						bestViewPage = vp;
					}
					else if(visibleResult != LayoutView.VisibleResult.No && 
						visibleResult == bestVisibleResult && vp.View.Zoom > bestViewPage.View.Zoom) {
						bestVisibleResult = visibleResult;
						bestViewPage = vp;
					}
				}

				if(bestViewPage == null)
					bestViewPage = (LayoutFrameWindowAreaViewTabPage)areaPage.TabViews.TabPages[0];

				tabAreas.SelectedTab = areaPage;
				areaPage.TabViews.SelectedTab = bestViewPage;

				bestViewPage.View.EnsureVisible(component.Location);

				if(markComponent)
					eventManager.Event(new LayoutEvent(component, "show-marker"));
			}
		}

		[LayoutEvent("all-layout-manual-dispatch-mode-status-changed")]
		private void AllLayoutManualDispatchModeStatusChanged(LayoutEvent e) {
			bool	active = (bool)e.Info;

			toolBarButtonToggleAllLayoutManualDispatch.Pushed = active;
		}

		[LayoutEvent("manual-dispatch-region-status-changed")]
		private void manualDispatchRegionStatusChanged(LayoutEvent e) {
			bool	enableAllLayoutManualDispatch = true;

			foreach(ManualDispatchRegionInfo manualDispatchRegion in Model.StateManager.ManualDispatchRegions)
				if(manualDispatchRegion.Active)
					enableAllLayoutManualDispatch = false;

			toolBarButtonToggleAllLayoutManualDispatch.Enabled = enableAllLayoutManualDispatch;
		}

		[LayoutEvent("all-locomotives-suspended-status-changed")]
		private void allLocomotivesSuspendedStatusChanged(LayoutEvent e) {
			bool	allSuspended = (bool)e.Info;

			toolBarButtonStopAllLocomotives.Pushed = allSuspended;
		}

		[LayoutEvent("show-marker")]
		private void showMarker(LayoutEvent e) {
			LayoutSelection	markerSelection = null;

			if(e.Sender is ModelComponent) {
				markerSelection = new LayoutSelection(eventManager);

				markerSelection.Add((ModelComponent)e.Sender);
			}
			else if(e.Sender is LayoutSelection)
				markerSelection = (LayoutSelection)e.Sender;
			else
				throw new ArgumentException("Invalid marker (can be either component or selection");

			new Marker(markerSelection);
		}

		#region Marker class

		class MarkerLook : ILayoutSelectionLook {
			public Color Color {
				get {
					return Color.IndianRed;
				}
			}
		}

		class Marker {
			Timer				hideMarkerTimer;
			LayoutSelection		markerSelection;

			public Marker(LayoutSelection markerSelection) {
				this.markerSelection = markerSelection;

				markerSelection.Display(new MarkerLook());
				hideMarkerTimer = new Timer();
				hideMarkerTimer.Interval = 500;
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
			bool	previousState = layoutControlVisible;

			layoutControlVisible = layoutControlViewer.Width > 10;
			if(layoutControlVisible != previousState)
				eventManager.Event(new LayoutEvent(this, layoutControlVisible ? "layout-control-shown" : "layout-control-hidden"));
		}

		void UpdateTripsMonitorVisible() {
			bool	previousState = tripsMonitorVisible;

			tripsMonitorVisible = tripsMonitor.Height > 10;
			if(tripsMonitorVisible != previousState)
				eventManager.Event(new LayoutEvent(this, tripsMonitorVisible ? "trips-monitor-shown" : "trips-monitor-hidden"));
		}

		void UpdateMessageVisible() {
			bool	previousState = messageViewVisible;

			messageViewVisible = messageViewer.Height > 10;
			if(messageViewVisible != previousState)
				eventManager.Event(new LayoutEvent(this, messageViewVisible ? "messages-shown" : "messages-hidden"));
		}

		void UpdateLocomotivesVisible() {
			bool	previousState = locomotiveViewVisible;

			locomotiveViewVisible = locomotivesViewer.Width > 10;
			if(locomotiveViewVisible != previousState)
				eventManager.Event(new LayoutEvent(this, locomotiveViewVisible ? "locomotives-shown" : "locomotives-hidden"));
		}

		[LayoutEvent("show-messages")]
		void ShowMessages(LayoutEvent e) {
			messageViewer.Height = (ClientSize.Height * 25) / 100;
			UpdateMessageVisible();
		}

		[LayoutEvent("hide-messages")]
		void HideMessages(LayoutEvent e) {
			messageViewer.Height = 0;
			UpdateMessageVisible();
		}

		[LayoutEvent("show-locomotives")]
		void ShowLocomotives(LayoutEvent e) {
			locomotivesViewer.Width = 220;
			UpdateLocomotivesVisible();
		}

		[LayoutEvent("hide-locomotives")]
		void HideLocomotives(LayoutEvent e) {
			locomotivesViewer.Width = 0;
			UpdateLocomotivesVisible();
		}

		[LayoutEvent("show-layout-control")]
		void ShowLayoutControl(LayoutEvent e) {
			if(layoutControlViewer.Width < 20) {
				if(layoutControlWidth > 20)
					layoutControlViewer.Width = layoutControlWidth;
				else
					layoutControlViewer.Width = 220;
			}

			UpdateLayoutControlVisible();
		}

		[LayoutEvent("hide-layout-control")]
		void HideLayoutControl(LayoutEvent e) {
			layoutControlWidth = layoutControlViewer.Width;
			layoutControlViewer.Width = 0;
			UpdateLayoutControlVisible();
		}


		[LayoutEvent("show-trips-monitor")]
		void ShowTripsMonitor(LayoutEvent e) {
			tripsMonitor.Height = (ClientSize.Height * 25) / 100;
			UpdateTripsMonitorVisible();
		}

		[LayoutEvent("hide-trips-monitor")]
		void HideTripsMonitor(LayoutEvent e) {
			tripsMonitor.Height = 0;
			UpdateTripsMonitorVisible();
		}

		[LayoutEvent("messages-shown")]
		private void messagesShown(LayoutEvent e) {
			toolBarButtonShowMessages.Pushed = true;
		}

		[LayoutEvent("messages-hidden")]
		private void messagesHidden(LayoutEvent e) {
			toolBarButtonShowMessages.Pushed = false;
		}

		[LayoutEvent("trips-monitor-shown")]
		private void tripsMonitorShown(LayoutEvent e) {
			toolBarButtonShowTripsMonitor.Pushed = true;
		}

		[LayoutEvent("trips-monitor-hidden")]
		private void tripsMonitorHidden(LayoutEvent e) {
			toolBarButtonShowTripsMonitor.Pushed = false;
		}

		[LayoutEvent("locomotives-shown")]
		private void locomotivesShown(LayoutEvent e) {
			toolBarButtonShowLocomotives.Pushed = true;
		}

		[LayoutEvent("locomotives-hidden")]
		private void locomotivesHidden(LayoutEvent e) {
			toolBarButtonShowLocomotives.Pushed = false;
		}

		[LayoutEvent("layout-control-shown")]
		private void layoutControlShown(LayoutEvent e) {
			toolBarButtonShowLayoutControl.Pushed = true;
		}

		[LayoutEvent("layout-control-hidden")]
		private void layoutControlHidden(LayoutEvent e) {
			toolBarButtonShowLayoutControl.Pushed = false;
		}

		#endregion

		#region Event handlers

		public void Areas_Added(object sender, EventArgs e) {
			LayoutModelArea	area = (LayoutModelArea)sender;

			tabAreas.TabPages.Add(new LayoutFrameWindowAreaTabPage(area));
			LayoutModified();
		}

		public void Areas_Removed(object sender, EventArgs e) {
			LayoutModelArea	area = (LayoutModelArea)sender;

			foreach(LayoutFrameWindowAreaTabPage areaTabPage in tabAreas.TabPages) {
				if(areaTabPage.Area == area) {
					tabAreas.TabPages.Remove(areaTabPage);
					break;
				}
			}
			LayoutModified();
		}

		public void Areas_Renamed(object sender, EventArgs e) {
			LayoutModelArea	area = (LayoutModelArea)sender;

			foreach(LayoutFrameWindowAreaTabPage areaTabPage in tabAreas.TabPages) {
				if(areaTabPage.Area == area) {
					areaTabPage.Text = area.Name;
					break;
				}
			}
			LayoutModified();
		}

		private void LayoutView_MouseDown(object sender, MouseEventArgs e) {
			if(activeTool != null)
				activeTool.LayoutView_MouseDown(sender, e);
		}

		private void menuItemSave_Click(object sender, System.EventArgs e) {
			saveLayout(false);

			if(operationMode)
				Model.StateManager.Save();
		}

		private void menuSaveAs_Click(object sender, System.EventArgs e) {
			saveLayout(true);
		}

		private void menuItemOpen_Click(object sender, System.EventArgs e) {
			openLayout();
		}

		private void menuItemNewLayout_Click(object sender, System.EventArgs e) {
			createNewLayout();		
		}

		class ButtonToMenuMap {
			public ToolBarButton	theButton;
			public EventHandler		theEvent;

			public ButtonToMenuMap(ToolBarButton button, EventHandler eh) {
				theButton = button;
				theEvent = eh;
			}
		}

		private void toolBarMain_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e) {
			ButtonToMenuMap[]	map = new ButtonToMenuMap[] {
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
			};

			if(e.Button.Visible && e.Button.Enabled) {
				for(int i = 0; i < map.Length; i++) {
					if(map[i].theButton == e.Button) {
						map[i].theEvent(sender, e);
						break;
					}
				}
			}
		}

		private void menuItemExit_Click(object sender, System.EventArgs e) {
			this.Close();
		}

#if LAYERS
		private void comboLayers_SelectedIndexChanged(object sender, System.EventArgs e) {
			this.ActiveLayer = (LayoutModelLayer)comboLayers.SelectedItem;
		}
#endif

		private void menuItemUndo_Click(object sender, System.EventArgs e) {
			commandManager.Undo();
		}

		private void menuItemRedo_Click(object sender, System.EventArgs e) {
			commandManager.Redo();
		}

		private void menuEdit_Popup(object sender, System.EventArgs e) {
			if(!operationMode && commandManager.CanUndo) {
				menuItemUndo.Enabled = true;
				menuItemUndo.Text = "&Undo " + commandManager.UndoCommandName;
			}
			else {
				menuItemUndo.Enabled = false;
				menuItemUndo.Text = "&Undo";
			}

			if(!operationMode && commandManager.CanRedo) {
				menuItemRedo.Enabled = true;
				menuItemRedo.Text = "&Redo " + commandManager.RedoCommandName;
			}
			else {
				menuItemRedo.Enabled = false;
				menuItemRedo.Text = "&Redo";
			}

			if(operationMode || UserSelection.Count == 0) {
				menuItemCopy.Enabled = false;
				menuItemCut.Enabled = false;
			}
			else {
				menuItemCopy.Enabled = true;
				menuItemCut.Enabled = true;
			}
		}

		private void menuItemCompile_Click(object sender, System.EventArgs e) {
			eventManager.Event(new LayoutEvent(this, "clear-messages"));
			if((bool)eventManager.Event(new LayoutEvent(model, "check-layout", null, true), LayoutEventScope.Model))
				MessageBox.Show(this, "Layout checked, all seems to be OK", "Layout Check Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void menuItemNewArea_Click(object sender, System.EventArgs e) {
			Dialogs.GetAreasName	getAreaName = new Dialogs.GetAreasName();

			if(getAreaName.ShowDialog(this) == DialogResult.OK) {
				AddArea(getAreaName.AreaName);
			}
		}

		private void menuItemRenameArea_Click(object sender, System.EventArgs e) {
			Dialogs.GetAreasName	getAreaName = new Dialogs.GetAreasName();

			if(getAreaName.ShowDialog(this) == DialogResult.OK)
				((LayoutFrameWindowAreaTabPage)tabAreas.SelectedTab).Area.Name = getAreaName.AreaName;
		}

		private void menuItemStyleFonts_Click(object sender, System.EventArgs e) {
			Dialogs.StadardFonts	standardFonts = new Dialogs.StadardFonts(Model);

			standardFonts.ShowDialog(this);
			model.WriteModelXmlInfo();
			model.Redraw();
		}

		private void menuItemStylePositions_Click(object sender, System.EventArgs e) {
			Dialogs.StandardPositions	standardPositions = new Dialogs.StandardPositions(Model);

			standardPositions.ShowDialog();
			model.WriteModelXmlInfo();
			model.Redraw();
		}

		private void menuItemCopy_Click(object sender, System.EventArgs e) {
			MenuItemCopySelection	copySelection = new MenuItemCopySelection(this, UserSelection.TopLeftLocation);

			copySelection.Copy();
		}

		private void menuItemCut_Click(object sender, System.EventArgs e) {
			MenuItemCutSelection	cutSelection = new MenuItemCutSelection(this, UserSelection.TopLeftLocation);

			cutSelection.Cut();
		}

		private void menuItemModules_Click(object sender, System.EventArgs e) {
			Dialogs.ModuleManagement	moduleManager = new Dialogs.ModuleManagement(this);

			moduleManager.ShowDialog(this);
		}

		private void menuItemTools_Popup(object sender, System.EventArgs e) {
			menuItemTools.MenuItems.Clear();

			eventManager.Event(new LayoutEvent(this, "tools-menu-open-request", null, menuItemTools), 
				LayoutEventScope.MyProcess);

			if(menuItemTools.MenuItems.Count == 0) {
				MenuItem	noTools = new MenuItem("No tools");

				noTools.Enabled = false;
				menuItemTools.MenuItems.Add(noTools);
			}
		}

		private void menuItemAreaArrange_Click(object sender, System.EventArgs e) {
			Dialogs.ArrangeAreas	arrangeAreas = new Dialogs.ArrangeAreas(this, tabAreas);

			arrangeAreas.ShowDialog();
		}

		private void menuItemDeleteArea_Click(object sender, System.EventArgs e) {
			LayoutFrameWindowAreaTabPage	selectedAreaPage = (LayoutFrameWindowAreaTabPage)tabAreas.SelectedTab;
			LayoutModelArea					selectedArea = selectedAreaPage.Area;

			// Check if area is empty
			if(selectedArea.Grid.Count > 0)
				MessageBox.Show(this, "This area is not empty, first remove all components, then delete the area", "Cannot delete area", MessageBoxButtons.OK, MessageBoxIcon.Error);
			else
				model.Areas.Remove(selectedArea);
		}

		private void menuItemView_Popup(object sender, System.EventArgs e) {
			if(ActiveView != null)
				menuItemViewGrid.Checked = ActiveView.ShowGrid != LayoutViewShowGridLines.Hide;

			if(messageViewVisible)
				menuItemShowMessages.Text = "Hide &Messages";
			else
				menuItemShowMessages.Text = "Show &Messages";

			if(locomotiveViewVisible)
				menuItemShowLocomotives.Text = "Hide &Locomotives";
			else
				menuItemShowLocomotives.Text = "Show &Locomotives";

			if(tripsMonitorVisible)
				menuItemShowTripsMonitor.Text = "Hide &Trips";
			else
				menuItemShowTripsMonitor.Text = "Show &Trips";

			if(layoutControlVisible)
				menuItemShowLayoutControl.Text = "Hide Layout &Control";
			else
				menuItemShowLayoutControl.Text = "Show Layout &Control";
		}

		private void menuItemViewGrid_Click(object sender, System.EventArgs e) {
			if(ActiveView != null) {
				if(menuItemViewGrid.Checked)
					ActiveView.ShowGrid = LayoutViewShowGridLines.Hide;
				else
					ActiveView.ShowGrid = LayoutViewShowGridLines.AutoHide;
			}
		}

		public LayoutView AddNewView() {
			Dialogs.GetViewName	getViewName = new Dialogs.GetViewName();

			if(getViewName.ShowDialog() == DialogResult.OK) {
				LayoutView	oldView = ActiveView;

				LayoutView newView = AddViewToArea(ActiveAreaPage.Area, getViewName.ViewName);
			
				if(oldView != null) {
					newView.Zoom = oldView.Zoom;
					newView.Origin = oldView.Origin;
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
			if(ActiveViewPage != null)
				ActiveAreaPage.TabViews.TabPages.Remove(ActiveViewPage);
		}

		private void menuItemViewRename_Click(object sender, System.EventArgs e) {
			if(ActiveViewPage != null) {
				Dialogs.GetViewName	getViewName = new Dialogs.GetViewName();

				if(getViewName.ShowDialog() == DialogResult.OK)
					ActiveViewPage.Text = getViewName.ViewName;
			}
		}

		private void menuItemViewArrage_Click(object sender, System.EventArgs e) {
			if(ActiveAreaPage != null) {
				Dialogs.ArrangeViews	arrangeViews = new Dialogs.ArrangeViews(this, ActiveAreaPage);

				arrangeViews.ShowDialog();
			}
		}

		struct ZoomEntry {
			public int			zoomFactor;
			public MenuItem		menuItem;

			public ZoomEntry(int zoomFactor, MenuItem menuItem) {
				this.zoomFactor = zoomFactor;
				this.menuItem = menuItem;
			}
		}

		private void menuItemZoomMenu_Popup(object sender, System.EventArgs e) {
			if(ActiveView != null) {
				int		currentZoom = (int)Math.Round(ActiveView.Zoom * 100);
				bool	presetZoomFound = false;

				foreach(ZoomEntry zoomEntry in zoomEntries) {
					zoomEntry.menuItem.Checked = (zoomEntry.zoomFactor == currentZoom);
					if(zoomEntry.menuItem.Checked)
						presetZoomFound = true;
				}

				menuItemSetZoom.Text = "Set zoom (" + currentZoom + "%)...";
				menuItemSetZoom.Checked = !presetZoomFound;
			}
		}

		private void menuItemZoomPreset_Click(object sender, System.EventArgs e) {
			MenuItem	menuItem = (MenuItem)sender;

			if(ActiveView != null) {
				foreach(ZoomEntry zoomEntry in zoomEntries) {
					if(zoomEntry.menuItem == menuItem) {
						ActiveView.Zoom = (float)zoomEntry.zoomFactor / 100;
						break;
					}
				}
			}
		}

		private void LayoutController_Deactivate(object sender, System.EventArgs e) {
			eventManager.Event(new LayoutEvent(this, "main-window-deactivated"));
			timerFreeResources.Enabled = true;
		}

		private void LayoutController_Resize(object sender, System.EventArgs e) {
			if(this.WindowState == FormWindowState.Minimized) {
				eventManager.Event(new LayoutEvent(this, "main-window-minimized"));
				timerFreeResources.Enabled = true;
			}
			else
				timerFreeResources.Enabled = false;
		}

		private void menuItemSetZoom_Click(object sender, System.EventArgs e) {
			if(ActiveView != null) {
				Dialogs.SetZoom	setZoom = new Dialogs.SetZoom();

				setZoom.ZoomFactor = (int)Math.Round(ActiveView.Zoom * 100);

				if(setZoom.ShowDialog(this) == DialogResult.OK)
					ActiveView.Zoom = (float)setZoom.ZoomFactor / 100;
			}
		}

		private void menuItemZoomAllArea_Click(object sender, System.EventArgs e) {
			if(ActiveView != null)
				ActiveView.ShowAllArea();
		}

		private void menuItemShowMessages_Click(object sender, System.EventArgs e) {
			if(messageViewVisible)
				EventManager.Event(new LayoutEvent(this, "hide-messages"));
			else
				EventManager.Event(new LayoutEvent(this, "show-messages"));
		}

		private void menuItemShowTripsMonitor_Click(object sender, System.EventArgs e) {
			if(tripsMonitorVisible)
				EventManager.Event(new LayoutEvent(this, "hide-trips-monitor"));
			else
				EventManager.Event(new LayoutEvent(this, "show-trips-monitor"));
		}

		private void menuItemShowLocomotives_Click(object sender, System.EventArgs e) {
			if(locomotiveViewVisible)
				EventManager.Event(new LayoutEvent(this, "hide-locomotives"));
			else
				EventManager.Event(new LayoutEvent(this, "show-locomotives"));
		
		}

		private void menuItemShowLayoutControl_Click(object sender, System.EventArgs e) {
			if(layoutControlVisible)		
				EventManager.Event(new LayoutEvent(this, "hide-layout-control"));
			else
				EventManager.Event(new LayoutEvent(this, "show-layout-control"));
		}

		private void menuItemOperational_Click(object sender, System.EventArgs e) {
			eventManager.Event(new LayoutEvent(this, "enter-operation-mode-request"));
		}

		private void menuItemDesign_Click(object sender, System.EventArgs e) {
			eventManager.Event(new LayoutEvent(this, "enter-design-mode-request"));
		}

		private void LayoutController_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			saveApplicationState();

			eventManager.Event(new LayoutEvent(this, "exit-operation-mode-request"));

			if(commandManager.ChangeLevel != 0)
				saveLayout(false);
		}

		private void menuItemLocomotiveCatalog_Click(object sender, System.EventArgs e) {
			new Dialogs.LocomotiveCatalog(eventManager).ShowDialog(this);
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
			eventManager.Event(new LayoutEvent(this, "free-resources"));
			timerFreeResources.Enabled = false;
		}

		private void LayoutController_Activated(object sender, System.EventArgs e) {
			timerFreeResources.Enabled = false;
		}

		private void menuItemManualDispatchRegions_Popup(object sender, System.EventArgs e) {
			menuItemManualDispatchRegions.MenuItems.Clear();

			foreach(ManualDispatchRegionInfo manualDispatchRegion in Model.StateManager.ManualDispatchRegions)
				menuItemManualDispatchRegions.MenuItems.Add(new ManualDispatchRegionItem(manualDispatchRegion));

			if(menuItemManualDispatchRegions.MenuItems.Count > 0)
				menuItemManualDispatchRegions.MenuItems.Add("-");

			menuItemManualDispatchRegions.MenuItems.Add(new AllLayoutManualDispatchItem(Model.StateManager));
			menuItemManualDispatchRegions.MenuItems.Add("-");

			menuItemManualDispatchRegions.MenuItems.Add("Manage Manual Dispatch Regions...", new EventHandler(this.onManageManualDispatchRegions));
		}

		private void onManageManualDispatchRegions(object sender, EventArgs e) {
			LayoutManager.Tools.Dialogs.ManualDispatchRegions	d = new LayoutManager.Tools.Dialogs.ManualDispatchRegions(Model.StateManager);

			d.Show();
		}

		private void menuItemPolicies_Click(object sender, System.EventArgs e) {
			Form	d = (Form)eventManager.Event(new LayoutEvent(this, "query-policies-definition-dialog"));

			if(d != null)
				d.Activate();
			else {
				Dialogs.PoliciesDefinition	p = new Dialogs.PoliciesDefinition(eventManager);

				p.Owner = this;
				p.Show();
			}
		}

		private void menuItemAccelerationProfiles_Click(object sender, System.EventArgs e) {
			LayoutManager.Tools.Dialogs.MotionRamps	rampsDialog = new LayoutManager.Tools.Dialogs.MotionRamps(EventManager, Model.Ramps);

			rampsDialog.ShowDialog();
			Model.WriteModelXmlInfo();
		}

		private void menuItemDefaultDriverParameters_Click(object sender, System.EventArgs e) {
			LayoutManager.Tools.Dialogs.DefaultDrivingParameters	d = new LayoutManager.Tools.Dialogs.DefaultDrivingParameters(Model.StateManager);

			d.ShowDialog(this);
		}

		private void menuItemCommonDestinations_Click(object sender, System.EventArgs e) {
			LayoutManager.Tools.Dialogs.TripPlanCommonDestinations	d = new LayoutManager.Tools.Dialogs.TripPlanCommonDestinations(eventManager);

			d.Owner = this;
			d.Show();
		}

		class ManualDispatchRegionItem : MenuItem {
			ManualDispatchRegionInfo	manualDispatchRegion;

			public ManualDispatchRegionItem(ManualDispatchRegionInfo manualDispatchRegion) {
				this.manualDispatchRegion = manualDispatchRegion;

				this.Text = manualDispatchRegion.Name;

				if(manualDispatchRegion.StateManager.AllLayoutManualDispatch)
					Enabled = false;
				else
					this.Checked = manualDispatchRegion.Active;
			}

			protected override void OnClick(EventArgs e) {
				try {
					if(manualDispatchRegion.Active)
						manualDispatchRegion.Active = false;
					else
						manualDispatchRegion.Active = true;

					manualDispatchRegion.StateManager.EventManager.Event(new LayoutEvent(manualDispatchRegion, "manual-dispatch-region-status-changed", null, manualDispatchRegion.Active));
				} catch(LayoutException ex) {
					MessageBox.Show("Could change manual dispatch mode because: " + ex.Message, "Unable to change Manual Dispatch Mode",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		class AllLayoutManualDispatchItem : MenuItem {
			LayoutStateManager	stateManager;

			public AllLayoutManualDispatchItem(LayoutStateManager stateManager) {
				this.stateManager = stateManager;

				Text = "All layout";

				foreach(ManualDispatchRegionInfo manualDispatchRegion in stateManager.ManualDispatchRegions)
					if(manualDispatchRegion.Active) {
						Enabled = false;
						break;
					}

				if(Enabled)
					Checked = stateManager.AllLayoutManualDispatch;
			}

			protected override void OnClick(EventArgs e) {
				try {
					stateManager.AllLayoutManualDispatch = !stateManager.AllLayoutManualDispatch;
				} catch(LayoutException ex) {
					MessageBox.Show("Could not set all layout to manual dispatch mode because: " + ex.Message, "Unable to set Manual Dispatch Mode",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void toggleAllLayoutManualDispatch(object sender, EventArgs e) {
			new AllLayoutManualDispatchItem(Model.StateManager).PerformClick();
		}

		private void menuItemSuspendLocomotives_Click(object sender, System.EventArgs e) {
			bool	allSuspended = (bool)EventManager.Event(new LayoutEvent(this, "are-all-locomotives-suspended"));

			if(allSuspended)
				EventManager.Event(new LayoutEvent(this, "resume-all-locomotives"));
			else
				EventManager.Event(new LayoutEvent(this, "suspend-all-locomotives"));
		}

		private void menuLayout_Popup(object sender, System.EventArgs e) {
			bool	allSuspended = (bool)EventManager.Event(new LayoutEvent(this, "are-all-locomotives-suspended"));

			if(allSuspended)
				menuItemSuspendLocomotives.Text = "&Resume train operation";
			else
				menuItemSuspendLocomotives.Text = "&Stop trains";
		}

		private void menuItemEmergencyStop_Click(object sender, System.EventArgs e) {
			EventManager.Event(new LayoutEvent(this, "emergency-stop-request"));
		}

		private void menuItemConnectLayout_Click(object sender, System.EventArgs e) {
			EventManager.Event(new LayoutEvent(this, "connect-layout-to-control-request"));
		}

		private void menuItemLearnLayout_Click(object sender, EventArgs e) {
			ModelComponent[] commandStations = Model[typeof(IModelComponentIsCommandStation)];

			if(commandStations.Length == 0)
				MessageBox.Show(this, "The layout does not contain any command station component. " +
					"Please add a command station component, and connect it to the tracks by adding a track power connection component", "No command stations are defined",
					MessageBoxButtons.OK, MessageBoxIcon.Error);

			object result = EventManager.Event(new LayoutEvent(this, "begin-design-time-layout-activation-request"));

			if(result != null && (bool)result) {
				if(EventManager.Event(new LayoutEvent(this, "activate-learn-layout")) == null) {
					Dialogs.LearnLayout learnLayout = new LayoutManager.Dialogs.LearnLayout(EventManager);

					learnLayout.Show(this);
				}
			}
			else
				MessageBox.Show(this, "Command station does not support powering the layout while in design mode", "Design time layout activation not supported",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		#endregion

	}

	public class LayoutFrameWindowAreaTabPage : TabPage {
		LayoutModelArea	area;
		TabControl		tabViews;

		public LayoutFrameWindowAreaTabPage(LayoutModelArea area) {
			this.area = area;

			tabViews = new TabControl();
			tabViews.Dock = DockStyle.Fill;
			tabViews.Alignment = TabAlignment.Bottom;

			this.Text = Area.Name;
			this.Controls.Add(tabViews);
		}

		public LayoutModelArea Area {
			get {
				return area;
			}
		}

		public TabControl TabViews {
			get {
				return tabViews;
			}
		}
	}


	public class LayoutFrameWindowAreaViewTabPage : TabPage {
		LayoutView	view;

		public LayoutFrameWindowAreaViewTabPage(LayoutView view) {
			this.view = view;
			view.Dock = DockStyle.Fill;
			Controls.Add(view);
			this.BorderStyle = BorderStyle.None;
		}

		public LayoutView View {
			get {
				return view;
			}
		}

		public void WriteXml(XmlWriter w) {
			w.WriteStartElement("View");
			w.WriteAttributeString("AreaID", XmlConvert.EncodeName(view.Area.AreaGuid.ToString()));
			w.WriteAttributeString("Name", XmlConvert.EncodeLocalName(this.Text));
			this.View.WriteXml(w);
			w.WriteEndElement();
		}
	}

}
