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
		private System.Windows.Forms.MenuItem menuItemShowAddresses;
		private System.Windows.Forms.MenuItem menuItemShowMessages;
		private System.Windows.Forms.MenuItem menuItemShowLocomotives;
		private System.Windows.Forms.MenuItem menuSeperator4;
		private System.Windows.Forms.MenuItem menuItemViewArrage;
		private System.Windows.Forms.MenuItem menuItemOperational;
		private System.Windows.Forms.MenuItem menuItemTools;
		private System.Windows.Forms.MenuItem menuItemDummy;
		private System.Windows.Forms.MenuItem menuLayout;
		private System.Windows.Forms.MenuItem menuItemDesign;
		private System.Windows.Forms.MenuItem menuItemSeperator2;
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
		ZoomEntry[]						zoomEntries;
		LayoutModuleManager				moduleManager;

		bool							operationMode = false;
		bool							showAddresses = true;
		bool							messageViewVisible = false;
		bool							locomotiveViewVisible = false;
		bool							tripsMonitorVisible = false;
		int								tripsMonitorHeightInOperationMode = 0;

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

			messageViewer.Height = 0;
			tripsMonitor.Height = 0;
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
				new ToolbarSettingEntry(toolBarButtonOpenLayiout, UIitemSetting.Hidden),
			};

			designModeUIsetting = new UIsettingEntry[] {
				new MenuSettingEntry(menuItemDesign, UIitemSetting.Disabled),
				new MenuSettingEntry(menuItemCommonDestinations, UIitemSetting.Hidden),
				new MenuSettingEntry(menuItemManualDispatchRegions, UIitemSetting.Hidden),
				new MenuSettingEntry(menuItemShowTripsMonitor, UIitemSetting.Hidden),
			    new MenuSettingEntry(menuItemEmergencyStop, UIitemSetting.Hidden),
				new MenuSettingEntry(menuItemSuspendLocomotives, UIitemSetting.Hidden),
				new ToolbarSettingEntry(toolBarButtonShowTripsMonitor, UIitemSetting.Hidden),
				new ToolbarSettingEntry(toolBarButtonToggleAllLayoutManualDispatch, UIitemSetting.Hidden),
				new ToolbarSettingEntry(toolBarButtonShowLocomotives, UIitemSetting.Hidden),
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

			messageViewer.EventManager = eventManager;
			locomotivesViewer.EventManager = eventManager;
			tripsMonitor.EventManager = eventManager;

			// Initial state is to show a new layout
			initializeModel();
			createNewLayout();

#if LAYERS
			initializeLayerSelector();
#else
			ActiveLayer = model.Layers[0];
#endif
			doUIsetting(designModeUIsetting);

			userSelection = new LayoutSelectionWithUndo(this);
			userSelection.Display(new LayoutManager.View.LayoutSelectionLook(Color.Blue));

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

		public LayoutModuleManager ModuleManager {
			get {
				return moduleManager;
			}
		}

		public bool ShowAddresses {
			get {
				return showAddresses;
			}

			set {
				showAddresses = value;
				Invalidate();
			}
		}

		public bool IsOperationMode {
			get {
				return operationMode;
			}
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
		}

		[LayoutEvent("tools-menu-open-request")]
		private void OnToolsMenuOpenRequest(LayoutEvent e) {
			ILayoutEmulatorServices	layoutEmulationServices = (ILayoutEmulatorServices)EventManager.Event(new LayoutEvent(this, "get-layout-emulation-services"));

			if(layoutEmulationServices != null) {
				Menu toolsMenu = (Menu)e.Info;

				toolsMenu.MenuItems.Add(new MenuItem("Reset layout emulation", new EventHandler(onResetLayoutEmulation)));
			}
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
					if(MessageBox.Show(this, "Problems were detected in the layout. Are you sure you want to switch to operation?", "Layout Check Results", 
						MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.Yes)
						switchMode = true;
					else
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

			w.WriteAttributeString("ShowComponentsAddress", XmlConvert.ToString(showAddresses));

			foreach(LayoutFrameWindowAreaTabPage areaTabPage in tabAreas.TabPages) {
				foreach(LayoutFrameWindowAreaViewTabPage viewTabPage in areaTabPage.TabViews.TabPages) {
					viewTabPage.WriteXml(w);
				}
			}

			w.WriteEndElement();
		}

		// On entry <AreaTabs>
		void ReadViewsXml(XmlReader r) {
			string	showAddressesValue = r["ShowComponentsAddress"];

			ShowAddresses = showAddressesValue == null ? true : XmlConvert.ToBoolean(showAddressesValue);

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(LayoutController));
			this.toolBarMain = new System.Windows.Forms.ToolBar();
			this.toolBarButtonOpenLayiout = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonSaveLayout = new System.Windows.Forms.ToolBarButton();
			this.Sep1 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonDesignMode = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonOperationMode = new System.Windows.Forms.ToolBarButton();
			this.sep2 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonShowLocomotives = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonShowMessages = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonShowTripsMonitor = new System.Windows.Forms.ToolBarButton();
			this.sep3 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonToggleAllLayoutManualDispatch = new System.Windows.Forms.ToolBarButton();
			this.sep4 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonStopAllLocomotives = new System.Windows.Forms.ToolBarButton();
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
			this.menuItemShowAddresses = new System.Windows.Forms.MenuItem();
			this.menuItemShowMessages = new System.Windows.Forms.MenuItem();
			this.menuItemShowLocomotives = new System.Windows.Forms.MenuItem();
			this.menuItemShowTripsMonitor = new System.Windows.Forms.MenuItem();
			this.menuSeperator4 = new System.Windows.Forms.MenuItem();
			this.menuItemViewArrage = new System.Windows.Forms.MenuItem();
			this.menuItemOperational = new System.Windows.Forms.MenuItem();
			this.menuItemTools = new System.Windows.Forms.MenuItem();
			this.menuItemDummy = new System.Windows.Forms.MenuItem();
			this.menuLayout = new System.Windows.Forms.MenuItem();
			this.menuItemDesign = new System.Windows.Forms.MenuItem();
			this.menuItemSeperator2 = new System.Windows.Forms.MenuItem();
			this.menuItemCompile = new System.Windows.Forms.MenuItem();
			this.menuItemEmergencyStop = new System.Windows.Forms.MenuItem();
			this.menuItemSuspendLocomotives = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuItemPolicies = new System.Windows.Forms.MenuItem();
			this.menuItemDefaultDriverParameters = new System.Windows.Forms.MenuItem();
			this.menuItemCommonDestinations = new System.Windows.Forms.MenuItem();
			this.menuItemManualDispatchRegions = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.mainMenu = new System.Windows.Forms.MainMenu();
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
			this.splitterMessages = new System.Windows.Forms.Splitter();
			this.splitterLocomotives = new System.Windows.Forms.Splitter();
			this.panelLocomotiveViewer = new System.Windows.Forms.Panel();
			this.timerFreeResources = new System.Timers.Timer();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelOperation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelMode)).BeginInit();
			this.panelMessageViewer.SuspendLayout();
			this.panelTripsMonitor.SuspendLayout();
			this.panelLocomotiveViewer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.timerFreeResources)).BeginInit();
			this.SuspendLayout();
			// 
			// toolBarMain
			// 
			this.toolBarMain.AccessibleDescription = ((string)(resources.GetObject("toolBarMain.AccessibleDescription")));
			this.toolBarMain.AccessibleName = ((string)(resources.GetObject("toolBarMain.AccessibleName")));
			this.toolBarMain.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("toolBarMain.Anchor")));
			this.toolBarMain.Appearance = ((System.Windows.Forms.ToolBarAppearance)(resources.GetObject("toolBarMain.Appearance")));
			this.toolBarMain.AutoSize = ((bool)(resources.GetObject("toolBarMain.AutoSize")));
			this.toolBarMain.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("toolBarMain.BackgroundImage")));
			this.toolBarMain.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																						   this.toolBarButtonOpenLayiout,
																						   this.toolBarButtonSaveLayout,
																						   this.Sep1,
																						   this.toolBarButtonDesignMode,
																						   this.toolBarButtonOperationMode,
																						   this.sep2,
																						   this.toolBarButtonShowLocomotives,
																						   this.toolBarButtonShowMessages,
																						   this.toolBarButtonShowTripsMonitor,
																						   this.sep3,
																						   this.toolBarButtonToggleAllLayoutManualDispatch,
																						   this.sep4,
																						   this.toolBarButtonStopAllLocomotives});
			this.toolBarMain.ButtonSize = ((System.Drawing.Size)(resources.GetObject("toolBarMain.ButtonSize")));
			this.toolBarMain.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("toolBarMain.Dock")));
			this.toolBarMain.DropDownArrows = ((bool)(resources.GetObject("toolBarMain.DropDownArrows")));
			this.toolBarMain.Enabled = ((bool)(resources.GetObject("toolBarMain.Enabled")));
			this.toolBarMain.Font = ((System.Drawing.Font)(resources.GetObject("toolBarMain.Font")));
			this.toolBarMain.ImageList = this.imageListTools;
			this.toolBarMain.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("toolBarMain.ImeMode")));
			this.toolBarMain.Location = ((System.Drawing.Point)(resources.GetObject("toolBarMain.Location")));
			this.toolBarMain.Name = "toolBarMain";
			this.toolBarMain.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("toolBarMain.RightToLeft")));
			this.toolBarMain.ShowToolTips = ((bool)(resources.GetObject("toolBarMain.ShowToolTips")));
			this.toolBarMain.Size = ((System.Drawing.Size)(resources.GetObject("toolBarMain.Size")));
			this.toolBarMain.TabIndex = ((int)(resources.GetObject("toolBarMain.TabIndex")));
			this.toolBarMain.TextAlign = ((System.Windows.Forms.ToolBarTextAlign)(resources.GetObject("toolBarMain.TextAlign")));
			this.toolBarMain.Visible = ((bool)(resources.GetObject("toolBarMain.Visible")));
			this.toolBarMain.Wrappable = ((bool)(resources.GetObject("toolBarMain.Wrappable")));
			this.toolBarMain.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBarMain_ButtonClick);
			// 
			// toolBarButtonOpenLayiout
			// 
			this.toolBarButtonOpenLayiout.Enabled = ((bool)(resources.GetObject("toolBarButtonOpenLayiout.Enabled")));
			this.toolBarButtonOpenLayiout.ImageIndex = ((int)(resources.GetObject("toolBarButtonOpenLayiout.ImageIndex")));
			this.toolBarButtonOpenLayiout.Text = resources.GetString("toolBarButtonOpenLayiout.Text");
			this.toolBarButtonOpenLayiout.ToolTipText = resources.GetString("toolBarButtonOpenLayiout.ToolTipText");
			this.toolBarButtonOpenLayiout.Visible = ((bool)(resources.GetObject("toolBarButtonOpenLayiout.Visible")));
			// 
			// toolBarButtonSaveLayout
			// 
			this.toolBarButtonSaveLayout.Enabled = ((bool)(resources.GetObject("toolBarButtonSaveLayout.Enabled")));
			this.toolBarButtonSaveLayout.ImageIndex = ((int)(resources.GetObject("toolBarButtonSaveLayout.ImageIndex")));
			this.toolBarButtonSaveLayout.Text = resources.GetString("toolBarButtonSaveLayout.Text");
			this.toolBarButtonSaveLayout.ToolTipText = resources.GetString("toolBarButtonSaveLayout.ToolTipText");
			this.toolBarButtonSaveLayout.Visible = ((bool)(resources.GetObject("toolBarButtonSaveLayout.Visible")));
			// 
			// Sep1
			// 
			this.Sep1.Enabled = ((bool)(resources.GetObject("Sep1.Enabled")));
			this.Sep1.ImageIndex = ((int)(resources.GetObject("Sep1.ImageIndex")));
			this.Sep1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			this.Sep1.Text = resources.GetString("Sep1.Text");
			this.Sep1.ToolTipText = resources.GetString("Sep1.ToolTipText");
			this.Sep1.Visible = ((bool)(resources.GetObject("Sep1.Visible")));
			// 
			// toolBarButtonDesignMode
			// 
			this.toolBarButtonDesignMode.Enabled = ((bool)(resources.GetObject("toolBarButtonDesignMode.Enabled")));
			this.toolBarButtonDesignMode.ImageIndex = ((int)(resources.GetObject("toolBarButtonDesignMode.ImageIndex")));
			this.toolBarButtonDesignMode.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.toolBarButtonDesignMode.Text = resources.GetString("toolBarButtonDesignMode.Text");
			this.toolBarButtonDesignMode.ToolTipText = resources.GetString("toolBarButtonDesignMode.ToolTipText");
			this.toolBarButtonDesignMode.Visible = ((bool)(resources.GetObject("toolBarButtonDesignMode.Visible")));
			// 
			// toolBarButtonOperationMode
			// 
			this.toolBarButtonOperationMode.Enabled = ((bool)(resources.GetObject("toolBarButtonOperationMode.Enabled")));
			this.toolBarButtonOperationMode.ImageIndex = ((int)(resources.GetObject("toolBarButtonOperationMode.ImageIndex")));
			this.toolBarButtonOperationMode.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.toolBarButtonOperationMode.Text = resources.GetString("toolBarButtonOperationMode.Text");
			this.toolBarButtonOperationMode.ToolTipText = resources.GetString("toolBarButtonOperationMode.ToolTipText");
			this.toolBarButtonOperationMode.Visible = ((bool)(resources.GetObject("toolBarButtonOperationMode.Visible")));
			// 
			// sep2
			// 
			this.sep2.Enabled = ((bool)(resources.GetObject("sep2.Enabled")));
			this.sep2.ImageIndex = ((int)(resources.GetObject("sep2.ImageIndex")));
			this.sep2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			this.sep2.Text = resources.GetString("sep2.Text");
			this.sep2.ToolTipText = resources.GetString("sep2.ToolTipText");
			this.sep2.Visible = ((bool)(resources.GetObject("sep2.Visible")));
			// 
			// toolBarButtonShowLocomotives
			// 
			this.toolBarButtonShowLocomotives.Enabled = ((bool)(resources.GetObject("toolBarButtonShowLocomotives.Enabled")));
			this.toolBarButtonShowLocomotives.ImageIndex = ((int)(resources.GetObject("toolBarButtonShowLocomotives.ImageIndex")));
			this.toolBarButtonShowLocomotives.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.toolBarButtonShowLocomotives.Text = resources.GetString("toolBarButtonShowLocomotives.Text");
			this.toolBarButtonShowLocomotives.ToolTipText = resources.GetString("toolBarButtonShowLocomotives.ToolTipText");
			this.toolBarButtonShowLocomotives.Visible = ((bool)(resources.GetObject("toolBarButtonShowLocomotives.Visible")));
			// 
			// toolBarButtonShowMessages
			// 
			this.toolBarButtonShowMessages.Enabled = ((bool)(resources.GetObject("toolBarButtonShowMessages.Enabled")));
			this.toolBarButtonShowMessages.ImageIndex = ((int)(resources.GetObject("toolBarButtonShowMessages.ImageIndex")));
			this.toolBarButtonShowMessages.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.toolBarButtonShowMessages.Text = resources.GetString("toolBarButtonShowMessages.Text");
			this.toolBarButtonShowMessages.ToolTipText = resources.GetString("toolBarButtonShowMessages.ToolTipText");
			this.toolBarButtonShowMessages.Visible = ((bool)(resources.GetObject("toolBarButtonShowMessages.Visible")));
			// 
			// toolBarButtonShowTripsMonitor
			// 
			this.toolBarButtonShowTripsMonitor.Enabled = ((bool)(resources.GetObject("toolBarButtonShowTripsMonitor.Enabled")));
			this.toolBarButtonShowTripsMonitor.ImageIndex = ((int)(resources.GetObject("toolBarButtonShowTripsMonitor.ImageIndex")));
			this.toolBarButtonShowTripsMonitor.Text = resources.GetString("toolBarButtonShowTripsMonitor.Text");
			this.toolBarButtonShowTripsMonitor.ToolTipText = resources.GetString("toolBarButtonShowTripsMonitor.ToolTipText");
			this.toolBarButtonShowTripsMonitor.Visible = ((bool)(resources.GetObject("toolBarButtonShowTripsMonitor.Visible")));
			// 
			// sep3
			// 
			this.sep3.Enabled = ((bool)(resources.GetObject("sep3.Enabled")));
			this.sep3.ImageIndex = ((int)(resources.GetObject("sep3.ImageIndex")));
			this.sep3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			this.sep3.Text = resources.GetString("sep3.Text");
			this.sep3.ToolTipText = resources.GetString("sep3.ToolTipText");
			this.sep3.Visible = ((bool)(resources.GetObject("sep3.Visible")));
			// 
			// toolBarButtonToggleAllLayoutManualDispatch
			// 
			this.toolBarButtonToggleAllLayoutManualDispatch.Enabled = ((bool)(resources.GetObject("toolBarButtonToggleAllLayoutManualDispatch.Enabled")));
			this.toolBarButtonToggleAllLayoutManualDispatch.ImageIndex = ((int)(resources.GetObject("toolBarButtonToggleAllLayoutManualDispatch.ImageIndex")));
			this.toolBarButtonToggleAllLayoutManualDispatch.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.toolBarButtonToggleAllLayoutManualDispatch.Text = resources.GetString("toolBarButtonToggleAllLayoutManualDispatch.Text");
			this.toolBarButtonToggleAllLayoutManualDispatch.ToolTipText = resources.GetString("toolBarButtonToggleAllLayoutManualDispatch.ToolTipText");
			this.toolBarButtonToggleAllLayoutManualDispatch.Visible = ((bool)(resources.GetObject("toolBarButtonToggleAllLayoutManualDispatch.Visible")));
			// 
			// sep4
			// 
			this.sep4.Enabled = ((bool)(resources.GetObject("sep4.Enabled")));
			this.sep4.ImageIndex = ((int)(resources.GetObject("sep4.ImageIndex")));
			this.sep4.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			this.sep4.Text = resources.GetString("sep4.Text");
			this.sep4.ToolTipText = resources.GetString("sep4.ToolTipText");
			this.sep4.Visible = ((bool)(resources.GetObject("sep4.Visible")));
			// 
			// toolBarButtonStopAllLocomotives
			// 
			this.toolBarButtonStopAllLocomotives.Enabled = ((bool)(resources.GetObject("toolBarButtonStopAllLocomotives.Enabled")));
			this.toolBarButtonStopAllLocomotives.ImageIndex = ((int)(resources.GetObject("toolBarButtonStopAllLocomotives.ImageIndex")));
			this.toolBarButtonStopAllLocomotives.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.toolBarButtonStopAllLocomotives.Text = resources.GetString("toolBarButtonStopAllLocomotives.Text");
			this.toolBarButtonStopAllLocomotives.ToolTipText = resources.GetString("toolBarButtonStopAllLocomotives.ToolTipText");
			this.toolBarButtonStopAllLocomotives.Visible = ((bool)(resources.GetObject("toolBarButtonStopAllLocomotives.Visible")));
			// 
			// imageListTools
			// 
			this.imageListTools.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageListTools.ImageSize = ((System.Drawing.Size)(resources.GetObject("imageListTools.ImageSize")));
			this.imageListTools.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTools.ImageStream")));
			this.imageListTools.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// statusBar
			// 
			this.statusBar.AccessibleDescription = ((string)(resources.GetObject("statusBar.AccessibleDescription")));
			this.statusBar.AccessibleName = ((string)(resources.GetObject("statusBar.AccessibleName")));
			this.statusBar.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("statusBar.Anchor")));
			this.statusBar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("statusBar.BackgroundImage")));
			this.statusBar.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("statusBar.Dock")));
			this.statusBar.Enabled = ((bool)(resources.GetObject("statusBar.Enabled")));
			this.statusBar.Font = ((System.Drawing.Font)(resources.GetObject("statusBar.Font")));
			this.statusBar.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("statusBar.ImeMode")));
			this.statusBar.Location = ((System.Drawing.Point)(resources.GetObject("statusBar.Location")));
			this.statusBar.Name = "statusBar";
			this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																						 this.statusBarPanelOperation,
																						 this.statusBarPanelMode});
			this.statusBar.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("statusBar.RightToLeft")));
			this.statusBar.ShowPanels = true;
			this.statusBar.Size = ((System.Drawing.Size)(resources.GetObject("statusBar.Size")));
			this.statusBar.TabIndex = ((int)(resources.GetObject("statusBar.TabIndex")));
			this.statusBar.Text = resources.GetString("statusBar.Text");
			this.statusBar.Visible = ((bool)(resources.GetObject("statusBar.Visible")));
			// 
			// statusBarPanelOperation
			// 
			this.statusBarPanelOperation.Alignment = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("statusBarPanelOperation.Alignment")));
			this.statusBarPanelOperation.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.statusBarPanelOperation.Icon = ((System.Drawing.Icon)(resources.GetObject("statusBarPanelOperation.Icon")));
			this.statusBarPanelOperation.MinWidth = ((int)(resources.GetObject("statusBarPanelOperation.MinWidth")));
			this.statusBarPanelOperation.Text = resources.GetString("statusBarPanelOperation.Text");
			this.statusBarPanelOperation.ToolTipText = resources.GetString("statusBarPanelOperation.ToolTipText");
			this.statusBarPanelOperation.Width = ((int)(resources.GetObject("statusBarPanelOperation.Width")));
			// 
			// statusBarPanelMode
			// 
			this.statusBarPanelMode.Alignment = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("statusBarPanelMode.Alignment")));
			this.statusBarPanelMode.Icon = ((System.Drawing.Icon)(resources.GetObject("statusBarPanelMode.Icon")));
			this.statusBarPanelMode.MinWidth = ((int)(resources.GetObject("statusBarPanelMode.MinWidth")));
			this.statusBarPanelMode.Text = resources.GetString("statusBarPanelMode.Text");
			this.statusBarPanelMode.ToolTipText = resources.GetString("statusBarPanelMode.ToolTipText");
			this.statusBarPanelMode.Width = ((int)(resources.GetObject("statusBarPanelMode.Width")));
			// 
			// menuItem2
			// 
			this.menuItem2.Enabled = ((bool)(resources.GetObject("menuItem2.Enabled")));
			this.menuItem2.Index = -1;
			this.menuItem2.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem2.Shortcut")));
			this.menuItem2.ShowShortcut = ((bool)(resources.GetObject("menuItem2.ShowShortcut")));
			this.menuItem2.Text = resources.GetString("menuItem2.Text");
			this.menuItem2.Visible = ((bool)(resources.GetObject("menuItem2.Visible")));
			// 
			// menuItemView
			// 
			this.menuItemView.Enabled = ((bool)(resources.GetObject("menuItemView.Enabled")));
			this.menuItemView.Index = 2;
			this.menuItemView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuItemViewNew,
																						 this.menuItemViewDelete,
																						 this.menuItemViewRename,
																						 this.menuItemSeperator6,
																						 this.menuItemZoomMenu,
																						 this.menuItemViewGrid,
																						 this.menuItemShowAddresses,
																						 this.menuItemShowMessages,
																						 this.menuItemShowLocomotives,
																						 this.menuItemShowTripsMonitor,
																						 this.menuSeperator4,
																						 this.menuItemViewArrage});
			this.menuItemView.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemView.Shortcut")));
			this.menuItemView.ShowShortcut = ((bool)(resources.GetObject("menuItemView.ShowShortcut")));
			this.menuItemView.Text = resources.GetString("menuItemView.Text");
			this.menuItemView.Visible = ((bool)(resources.GetObject("menuItemView.Visible")));
			this.menuItemView.Popup += new System.EventHandler(this.menuItemView_Popup);
			// 
			// menuItemViewNew
			// 
			this.menuItemViewNew.Enabled = ((bool)(resources.GetObject("menuItemViewNew.Enabled")));
			this.menuItemViewNew.Index = 0;
			this.menuItemViewNew.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemViewNew.Shortcut")));
			this.menuItemViewNew.ShowShortcut = ((bool)(resources.GetObject("menuItemViewNew.ShowShortcut")));
			this.menuItemViewNew.Text = resources.GetString("menuItemViewNew.Text");
			this.menuItemViewNew.Visible = ((bool)(resources.GetObject("menuItemViewNew.Visible")));
			this.menuItemViewNew.Click += new System.EventHandler(this.menuItemViewNew_Click);
			// 
			// menuItemViewDelete
			// 
			this.menuItemViewDelete.Enabled = ((bool)(resources.GetObject("menuItemViewDelete.Enabled")));
			this.menuItemViewDelete.Index = 1;
			this.menuItemViewDelete.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemViewDelete.Shortcut")));
			this.menuItemViewDelete.ShowShortcut = ((bool)(resources.GetObject("menuItemViewDelete.ShowShortcut")));
			this.menuItemViewDelete.Text = resources.GetString("menuItemViewDelete.Text");
			this.menuItemViewDelete.Visible = ((bool)(resources.GetObject("menuItemViewDelete.Visible")));
			this.menuItemViewDelete.Click += new System.EventHandler(this.menuItemViewDelete_Click);
			// 
			// menuItemViewRename
			// 
			this.menuItemViewRename.Enabled = ((bool)(resources.GetObject("menuItemViewRename.Enabled")));
			this.menuItemViewRename.Index = 2;
			this.menuItemViewRename.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemViewRename.Shortcut")));
			this.menuItemViewRename.ShowShortcut = ((bool)(resources.GetObject("menuItemViewRename.ShowShortcut")));
			this.menuItemViewRename.Text = resources.GetString("menuItemViewRename.Text");
			this.menuItemViewRename.Visible = ((bool)(resources.GetObject("menuItemViewRename.Visible")));
			this.menuItemViewRename.Click += new System.EventHandler(this.menuItemViewRename_Click);
			// 
			// menuItemSeperator6
			// 
			this.menuItemSeperator6.Enabled = ((bool)(resources.GetObject("menuItemSeperator6.Enabled")));
			this.menuItemSeperator6.Index = 3;
			this.menuItemSeperator6.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemSeperator6.Shortcut")));
			this.menuItemSeperator6.ShowShortcut = ((bool)(resources.GetObject("menuItemSeperator6.ShowShortcut")));
			this.menuItemSeperator6.Text = resources.GetString("menuItemSeperator6.Text");
			this.menuItemSeperator6.Visible = ((bool)(resources.GetObject("menuItemSeperator6.Visible")));
			// 
			// menuItemZoomMenu
			// 
			this.menuItemZoomMenu.Enabled = ((bool)(resources.GetObject("menuItemZoomMenu.Enabled")));
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
			this.menuItemZoomMenu.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemZoomMenu.Shortcut")));
			this.menuItemZoomMenu.ShowShortcut = ((bool)(resources.GetObject("menuItemZoomMenu.ShowShortcut")));
			this.menuItemZoomMenu.Text = resources.GetString("menuItemZoomMenu.Text");
			this.menuItemZoomMenu.Visible = ((bool)(resources.GetObject("menuItemZoomMenu.Visible")));
			this.menuItemZoomMenu.Popup += new System.EventHandler(this.menuItemZoomMenu_Popup);
			// 
			// menuItemSetZoom
			// 
			this.menuItemSetZoom.Enabled = ((bool)(resources.GetObject("menuItemSetZoom.Enabled")));
			this.menuItemSetZoom.Index = 0;
			this.menuItemSetZoom.RadioCheck = true;
			this.menuItemSetZoom.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemSetZoom.Shortcut")));
			this.menuItemSetZoom.ShowShortcut = ((bool)(resources.GetObject("menuItemSetZoom.ShowShortcut")));
			this.menuItemSetZoom.Text = resources.GetString("menuItemSetZoom.Text");
			this.menuItemSetZoom.Visible = ((bool)(resources.GetObject("menuItemSetZoom.Visible")));
			this.menuItemSetZoom.Click += new System.EventHandler(this.menuItemSetZoom_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Enabled = ((bool)(resources.GetObject("menuItem5.Enabled")));
			this.menuItem5.Index = 1;
			this.menuItem5.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem5.Shortcut")));
			this.menuItem5.ShowShortcut = ((bool)(resources.GetObject("menuItem5.ShowShortcut")));
			this.menuItem5.Text = resources.GetString("menuItem5.Text");
			this.menuItem5.Visible = ((bool)(resources.GetObject("menuItem5.Visible")));
			// 
			// menuItemZoom200
			// 
			this.menuItemZoom200.Enabled = ((bool)(resources.GetObject("menuItemZoom200.Enabled")));
			this.menuItemZoom200.Index = 2;
			this.menuItemZoom200.RadioCheck = true;
			this.menuItemZoom200.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemZoom200.Shortcut")));
			this.menuItemZoom200.ShowShortcut = ((bool)(resources.GetObject("menuItemZoom200.ShowShortcut")));
			this.menuItemZoom200.Text = resources.GetString("menuItemZoom200.Text");
			this.menuItemZoom200.Visible = ((bool)(resources.GetObject("menuItemZoom200.Visible")));
			this.menuItemZoom200.Click += new System.EventHandler(this.menuItemZoomPreset_Click);
			// 
			// menuItemZoom150
			// 
			this.menuItemZoom150.Enabled = ((bool)(resources.GetObject("menuItemZoom150.Enabled")));
			this.menuItemZoom150.Index = 3;
			this.menuItemZoom150.RadioCheck = true;
			this.menuItemZoom150.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemZoom150.Shortcut")));
			this.menuItemZoom150.ShowShortcut = ((bool)(resources.GetObject("menuItemZoom150.ShowShortcut")));
			this.menuItemZoom150.Text = resources.GetString("menuItemZoom150.Text");
			this.menuItemZoom150.Visible = ((bool)(resources.GetObject("menuItemZoom150.Visible")));
			this.menuItemZoom150.Click += new System.EventHandler(this.menuItemZoomPreset_Click);
			// 
			// menuItemZoom100
			// 
			this.menuItemZoom100.Enabled = ((bool)(resources.GetObject("menuItemZoom100.Enabled")));
			this.menuItemZoom100.Index = 4;
			this.menuItemZoom100.RadioCheck = true;
			this.menuItemZoom100.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemZoom100.Shortcut")));
			this.menuItemZoom100.ShowShortcut = ((bool)(resources.GetObject("menuItemZoom100.ShowShortcut")));
			this.menuItemZoom100.Text = resources.GetString("menuItemZoom100.Text");
			this.menuItemZoom100.Visible = ((bool)(resources.GetObject("menuItemZoom100.Visible")));
			this.menuItemZoom100.Click += new System.EventHandler(this.menuItemZoomPreset_Click);
			// 
			// menuItemZoom75
			// 
			this.menuItemZoom75.Enabled = ((bool)(resources.GetObject("menuItemZoom75.Enabled")));
			this.menuItemZoom75.Index = 5;
			this.menuItemZoom75.RadioCheck = true;
			this.menuItemZoom75.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemZoom75.Shortcut")));
			this.menuItemZoom75.ShowShortcut = ((bool)(resources.GetObject("menuItemZoom75.ShowShortcut")));
			this.menuItemZoom75.Text = resources.GetString("menuItemZoom75.Text");
			this.menuItemZoom75.Visible = ((bool)(resources.GetObject("menuItemZoom75.Visible")));
			this.menuItemZoom75.Click += new System.EventHandler(this.menuItemZoomPreset_Click);
			// 
			// menuItemZoom50
			// 
			this.menuItemZoom50.Enabled = ((bool)(resources.GetObject("menuItemZoom50.Enabled")));
			this.menuItemZoom50.Index = 6;
			this.menuItemZoom50.RadioCheck = true;
			this.menuItemZoom50.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemZoom50.Shortcut")));
			this.menuItemZoom50.ShowShortcut = ((bool)(resources.GetObject("menuItemZoom50.ShowShortcut")));
			this.menuItemZoom50.Text = resources.GetString("menuItemZoom50.Text");
			this.menuItemZoom50.Visible = ((bool)(resources.GetObject("menuItemZoom50.Visible")));
			this.menuItemZoom50.Click += new System.EventHandler(this.menuItemZoomPreset_Click);
			// 
			// menuItemZoom25
			// 
			this.menuItemZoom25.Enabled = ((bool)(resources.GetObject("menuItemZoom25.Enabled")));
			this.menuItemZoom25.Index = 7;
			this.menuItemZoom25.RadioCheck = true;
			this.menuItemZoom25.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemZoom25.Shortcut")));
			this.menuItemZoom25.ShowShortcut = ((bool)(resources.GetObject("menuItemZoom25.ShowShortcut")));
			this.menuItemZoom25.Text = resources.GetString("menuItemZoom25.Text");
			this.menuItemZoom25.Visible = ((bool)(resources.GetObject("menuItemZoom25.Visible")));
			this.menuItemZoom25.Click += new System.EventHandler(this.menuItemZoomPreset_Click);
			// 
			// menuItem12
			// 
			this.menuItem12.Enabled = ((bool)(resources.GetObject("menuItem12.Enabled")));
			this.menuItem12.Index = 8;
			this.menuItem12.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem12.Shortcut")));
			this.menuItem12.ShowShortcut = ((bool)(resources.GetObject("menuItem12.ShowShortcut")));
			this.menuItem12.Text = resources.GetString("menuItem12.Text");
			this.menuItem12.Visible = ((bool)(resources.GetObject("menuItem12.Visible")));
			// 
			// menuItem1ZoomAllArea
			// 
			this.menuItem1ZoomAllArea.Enabled = ((bool)(resources.GetObject("menuItem1ZoomAllArea.Enabled")));
			this.menuItem1ZoomAllArea.Index = 9;
			this.menuItem1ZoomAllArea.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem1ZoomAllArea.Shortcut")));
			this.menuItem1ZoomAllArea.ShowShortcut = ((bool)(resources.GetObject("menuItem1ZoomAllArea.ShowShortcut")));
			this.menuItem1ZoomAllArea.Text = resources.GetString("menuItem1ZoomAllArea.Text");
			this.menuItem1ZoomAllArea.Visible = ((bool)(resources.GetObject("menuItem1ZoomAllArea.Visible")));
			this.menuItem1ZoomAllArea.Click += new System.EventHandler(this.menuItemZoomAllArea_Click);
			// 
			// menuItemViewGrid
			// 
			this.menuItemViewGrid.Checked = true;
			this.menuItemViewGrid.Enabled = ((bool)(resources.GetObject("menuItemViewGrid.Enabled")));
			this.menuItemViewGrid.Index = 5;
			this.menuItemViewGrid.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemViewGrid.Shortcut")));
			this.menuItemViewGrid.ShowShortcut = ((bool)(resources.GetObject("menuItemViewGrid.ShowShortcut")));
			this.menuItemViewGrid.Text = resources.GetString("menuItemViewGrid.Text");
			this.menuItemViewGrid.Visible = ((bool)(resources.GetObject("menuItemViewGrid.Visible")));
			this.menuItemViewGrid.Click += new System.EventHandler(this.menuItemViewGrid_Click);
			// 
			// menuItemShowAddresses
			// 
			this.menuItemShowAddresses.Enabled = ((bool)(resources.GetObject("menuItemShowAddresses.Enabled")));
			this.menuItemShowAddresses.Index = 6;
			this.menuItemShowAddresses.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemShowAddresses.Shortcut")));
			this.menuItemShowAddresses.ShowShortcut = ((bool)(resources.GetObject("menuItemShowAddresses.ShowShortcut")));
			this.menuItemShowAddresses.Text = resources.GetString("menuItemShowAddresses.Text");
			this.menuItemShowAddresses.Visible = ((bool)(resources.GetObject("menuItemShowAddresses.Visible")));
			this.menuItemShowAddresses.Click += new System.EventHandler(this.menuItemShowAddresses_Click);
			// 
			// menuItemShowMessages
			// 
			this.menuItemShowMessages.Enabled = ((bool)(resources.GetObject("menuItemShowMessages.Enabled")));
			this.menuItemShowMessages.Index = 7;
			this.menuItemShowMessages.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemShowMessages.Shortcut")));
			this.menuItemShowMessages.ShowShortcut = ((bool)(resources.GetObject("menuItemShowMessages.ShowShortcut")));
			this.menuItemShowMessages.Text = resources.GetString("menuItemShowMessages.Text");
			this.menuItemShowMessages.Visible = ((bool)(resources.GetObject("menuItemShowMessages.Visible")));
			this.menuItemShowMessages.Click += new System.EventHandler(this.menuItemShowMessages_Click);
			// 
			// menuItemShowLocomotives
			// 
			this.menuItemShowLocomotives.Enabled = ((bool)(resources.GetObject("menuItemShowLocomotives.Enabled")));
			this.menuItemShowLocomotives.Index = 8;
			this.menuItemShowLocomotives.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemShowLocomotives.Shortcut")));
			this.menuItemShowLocomotives.ShowShortcut = ((bool)(resources.GetObject("menuItemShowLocomotives.ShowShortcut")));
			this.menuItemShowLocomotives.Text = resources.GetString("menuItemShowLocomotives.Text");
			this.menuItemShowLocomotives.Visible = ((bool)(resources.GetObject("menuItemShowLocomotives.Visible")));
			this.menuItemShowLocomotives.Click += new System.EventHandler(this.menuItemShowLocomotives_Click);
			// 
			// menuItemShowTripsMonitor
			// 
			this.menuItemShowTripsMonitor.Enabled = ((bool)(resources.GetObject("menuItemShowTripsMonitor.Enabled")));
			this.menuItemShowTripsMonitor.Index = 9;
			this.menuItemShowTripsMonitor.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemShowTripsMonitor.Shortcut")));
			this.menuItemShowTripsMonitor.ShowShortcut = ((bool)(resources.GetObject("menuItemShowTripsMonitor.ShowShortcut")));
			this.menuItemShowTripsMonitor.Text = resources.GetString("menuItemShowTripsMonitor.Text");
			this.menuItemShowTripsMonitor.Visible = ((bool)(resources.GetObject("menuItemShowTripsMonitor.Visible")));
			this.menuItemShowTripsMonitor.Click += new System.EventHandler(this.menuItemShowTripsMonitor_Click);
			// 
			// menuSeperator4
			// 
			this.menuSeperator4.Enabled = ((bool)(resources.GetObject("menuSeperator4.Enabled")));
			this.menuSeperator4.Index = 10;
			this.menuSeperator4.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuSeperator4.Shortcut")));
			this.menuSeperator4.ShowShortcut = ((bool)(resources.GetObject("menuSeperator4.ShowShortcut")));
			this.menuSeperator4.Text = resources.GetString("menuSeperator4.Text");
			this.menuSeperator4.Visible = ((bool)(resources.GetObject("menuSeperator4.Visible")));
			// 
			// menuItemViewArrage
			// 
			this.menuItemViewArrage.Enabled = ((bool)(resources.GetObject("menuItemViewArrage.Enabled")));
			this.menuItemViewArrage.Index = 11;
			this.menuItemViewArrage.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemViewArrage.Shortcut")));
			this.menuItemViewArrage.ShowShortcut = ((bool)(resources.GetObject("menuItemViewArrage.ShowShortcut")));
			this.menuItemViewArrage.Text = resources.GetString("menuItemViewArrage.Text");
			this.menuItemViewArrage.Visible = ((bool)(resources.GetObject("menuItemViewArrage.Visible")));
			this.menuItemViewArrage.Click += new System.EventHandler(this.menuItemViewArrage_Click);
			// 
			// menuItemOperational
			// 
			this.menuItemOperational.Enabled = ((bool)(resources.GetObject("menuItemOperational.Enabled")));
			this.menuItemOperational.Index = 1;
			this.menuItemOperational.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemOperational.Shortcut")));
			this.menuItemOperational.ShowShortcut = ((bool)(resources.GetObject("menuItemOperational.ShowShortcut")));
			this.menuItemOperational.Text = resources.GetString("menuItemOperational.Text");
			this.menuItemOperational.Visible = ((bool)(resources.GetObject("menuItemOperational.Visible")));
			this.menuItemOperational.Click += new System.EventHandler(this.menuItemOperational_Click);
			// 
			// menuItemTools
			// 
			this.menuItemTools.Enabled = ((bool)(resources.GetObject("menuItemTools.Enabled")));
			this.menuItemTools.Index = 5;
			this.menuItemTools.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						  this.menuItemDummy});
			this.menuItemTools.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemTools.Shortcut")));
			this.menuItemTools.ShowShortcut = ((bool)(resources.GetObject("menuItemTools.ShowShortcut")));
			this.menuItemTools.Text = resources.GetString("menuItemTools.Text");
			this.menuItemTools.Visible = ((bool)(resources.GetObject("menuItemTools.Visible")));
			this.menuItemTools.Popup += new System.EventHandler(this.menuItemTools_Popup);
			// 
			// menuItemDummy
			// 
			this.menuItemDummy.Enabled = ((bool)(resources.GetObject("menuItemDummy.Enabled")));
			this.menuItemDummy.Index = 0;
			this.menuItemDummy.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemDummy.Shortcut")));
			this.menuItemDummy.ShowShortcut = ((bool)(resources.GetObject("menuItemDummy.ShowShortcut")));
			this.menuItemDummy.Text = resources.GetString("menuItemDummy.Text");
			this.menuItemDummy.Visible = ((bool)(resources.GetObject("menuItemDummy.Visible")));
			// 
			// menuLayout
			// 
			this.menuLayout.Enabled = ((bool)(resources.GetObject("menuLayout.Enabled")));
			this.menuLayout.Index = 4;
			this.menuLayout.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.menuItemDesign,
																					   this.menuItemOperational,
																					   this.menuItemSeperator2,
																					   this.menuItemCompile,
																					   this.menuItemEmergencyStop,
																					   this.menuItemSuspendLocomotives,
																					   this.menuItem6,
																					   this.menuItemPolicies,
																					   this.menuItemDefaultDriverParameters,
																					   this.menuItemCommonDestinations,
																					   this.menuItemManualDispatchRegions});
			this.menuLayout.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuLayout.Shortcut")));
			this.menuLayout.ShowShortcut = ((bool)(resources.GetObject("menuLayout.ShowShortcut")));
			this.menuLayout.Text = resources.GetString("menuLayout.Text");
			this.menuLayout.Visible = ((bool)(resources.GetObject("menuLayout.Visible")));
			this.menuLayout.Popup += new System.EventHandler(this.menuLayout_Popup);
			// 
			// menuItemDesign
			// 
			this.menuItemDesign.Enabled = ((bool)(resources.GetObject("menuItemDesign.Enabled")));
			this.menuItemDesign.Index = 0;
			this.menuItemDesign.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemDesign.Shortcut")));
			this.menuItemDesign.ShowShortcut = ((bool)(resources.GetObject("menuItemDesign.ShowShortcut")));
			this.menuItemDesign.Text = resources.GetString("menuItemDesign.Text");
			this.menuItemDesign.Visible = ((bool)(resources.GetObject("menuItemDesign.Visible")));
			this.menuItemDesign.Click += new System.EventHandler(this.menuItemDesign_Click);
			// 
			// menuItemSeperator2
			// 
			this.menuItemSeperator2.Enabled = ((bool)(resources.GetObject("menuItemSeperator2.Enabled")));
			this.menuItemSeperator2.Index = 2;
			this.menuItemSeperator2.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemSeperator2.Shortcut")));
			this.menuItemSeperator2.ShowShortcut = ((bool)(resources.GetObject("menuItemSeperator2.ShowShortcut")));
			this.menuItemSeperator2.Text = resources.GetString("menuItemSeperator2.Text");
			this.menuItemSeperator2.Visible = ((bool)(resources.GetObject("menuItemSeperator2.Visible")));
			// 
			// menuItemCompile
			// 
			this.menuItemCompile.Enabled = ((bool)(resources.GetObject("menuItemCompile.Enabled")));
			this.menuItemCompile.Index = 3;
			this.menuItemCompile.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemCompile.Shortcut")));
			this.menuItemCompile.ShowShortcut = ((bool)(resources.GetObject("menuItemCompile.ShowShortcut")));
			this.menuItemCompile.Text = resources.GetString("menuItemCompile.Text");
			this.menuItemCompile.Visible = ((bool)(resources.GetObject("menuItemCompile.Visible")));
			this.menuItemCompile.Click += new System.EventHandler(this.menuItemCompile_Click);
			// 
			// menuItemEmergencyStop
			// 
			this.menuItemEmergencyStop.Enabled = ((bool)(resources.GetObject("menuItemEmergencyStop.Enabled")));
			this.menuItemEmergencyStop.Index = 4;
			this.menuItemEmergencyStop.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemEmergencyStop.Shortcut")));
			this.menuItemEmergencyStop.ShowShortcut = ((bool)(resources.GetObject("menuItemEmergencyStop.ShowShortcut")));
			this.menuItemEmergencyStop.Text = resources.GetString("menuItemEmergencyStop.Text");
			this.menuItemEmergencyStop.Visible = ((bool)(resources.GetObject("menuItemEmergencyStop.Visible")));
			// 
			// menuItemSuspendLocomotives
			// 
			this.menuItemSuspendLocomotives.Enabled = ((bool)(resources.GetObject("menuItemSuspendLocomotives.Enabled")));
			this.menuItemSuspendLocomotives.Index = 5;
			this.menuItemSuspendLocomotives.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemSuspendLocomotives.Shortcut")));
			this.menuItemSuspendLocomotives.ShowShortcut = ((bool)(resources.GetObject("menuItemSuspendLocomotives.ShowShortcut")));
			this.menuItemSuspendLocomotives.Text = resources.GetString("menuItemSuspendLocomotives.Text");
			this.menuItemSuspendLocomotives.Visible = ((bool)(resources.GetObject("menuItemSuspendLocomotives.Visible")));
			this.menuItemSuspendLocomotives.Click += new System.EventHandler(this.menuItemSuspendLocomotives_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Enabled = ((bool)(resources.GetObject("menuItem6.Enabled")));
			this.menuItem6.Index = 6;
			this.menuItem6.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem6.Shortcut")));
			this.menuItem6.ShowShortcut = ((bool)(resources.GetObject("menuItem6.ShowShortcut")));
			this.menuItem6.Text = resources.GetString("menuItem6.Text");
			this.menuItem6.Visible = ((bool)(resources.GetObject("menuItem6.Visible")));
			// 
			// menuItemPolicies
			// 
			this.menuItemPolicies.Enabled = ((bool)(resources.GetObject("menuItemPolicies.Enabled")));
			this.menuItemPolicies.Index = 7;
			this.menuItemPolicies.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemPolicies.Shortcut")));
			this.menuItemPolicies.ShowShortcut = ((bool)(resources.GetObject("menuItemPolicies.ShowShortcut")));
			this.menuItemPolicies.Text = resources.GetString("menuItemPolicies.Text");
			this.menuItemPolicies.Visible = ((bool)(resources.GetObject("menuItemPolicies.Visible")));
			this.menuItemPolicies.Click += new System.EventHandler(this.menuItemPolicies_Click);
			// 
			// menuItemDefaultDriverParameters
			// 
			this.menuItemDefaultDriverParameters.Enabled = ((bool)(resources.GetObject("menuItemDefaultDriverParameters.Enabled")));
			this.menuItemDefaultDriverParameters.Index = 8;
			this.menuItemDefaultDriverParameters.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemDefaultDriverParameters.Shortcut")));
			this.menuItemDefaultDriverParameters.ShowShortcut = ((bool)(resources.GetObject("menuItemDefaultDriverParameters.ShowShortcut")));
			this.menuItemDefaultDriverParameters.Text = resources.GetString("menuItemDefaultDriverParameters.Text");
			this.menuItemDefaultDriverParameters.Visible = ((bool)(resources.GetObject("menuItemDefaultDriverParameters.Visible")));
			this.menuItemDefaultDriverParameters.Click += new System.EventHandler(this.menuItemDefaultDriverParameters_Click);
			// 
			// menuItemCommonDestinations
			// 
			this.menuItemCommonDestinations.Enabled = ((bool)(resources.GetObject("menuItemCommonDestinations.Enabled")));
			this.menuItemCommonDestinations.Index = 9;
			this.menuItemCommonDestinations.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemCommonDestinations.Shortcut")));
			this.menuItemCommonDestinations.ShowShortcut = ((bool)(resources.GetObject("menuItemCommonDestinations.ShowShortcut")));
			this.menuItemCommonDestinations.Text = resources.GetString("menuItemCommonDestinations.Text");
			this.menuItemCommonDestinations.Visible = ((bool)(resources.GetObject("menuItemCommonDestinations.Visible")));
			this.menuItemCommonDestinations.Click += new System.EventHandler(this.menuItemCommonDestinations_Click);
			// 
			// menuItemManualDispatchRegions
			// 
			this.menuItemManualDispatchRegions.Enabled = ((bool)(resources.GetObject("menuItemManualDispatchRegions.Enabled")));
			this.menuItemManualDispatchRegions.Index = 10;
			this.menuItemManualDispatchRegions.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																										  this.menuItem4});
			this.menuItemManualDispatchRegions.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemManualDispatchRegions.Shortcut")));
			this.menuItemManualDispatchRegions.ShowShortcut = ((bool)(resources.GetObject("menuItemManualDispatchRegions.ShowShortcut")));
			this.menuItemManualDispatchRegions.Text = resources.GetString("menuItemManualDispatchRegions.Text");
			this.menuItemManualDispatchRegions.Visible = ((bool)(resources.GetObject("menuItemManualDispatchRegions.Visible")));
			this.menuItemManualDispatchRegions.Popup += new System.EventHandler(this.menuItemManualDispatchRegions_Popup);
			// 
			// menuItem4
			// 
			this.menuItem4.Enabled = ((bool)(resources.GetObject("menuItem4.Enabled")));
			this.menuItem4.Index = 0;
			this.menuItem4.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem4.Shortcut")));
			this.menuItem4.ShowShortcut = ((bool)(resources.GetObject("menuItem4.ShowShortcut")));
			this.menuItem4.Text = resources.GetString("menuItem4.Text");
			this.menuItem4.Visible = ((bool)(resources.GetObject("menuItem4.Visible")));
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
			this.mainMenu.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("mainMenu.RightToLeft")));
			// 
			// menuFile
			// 
			this.menuFile.Enabled = ((bool)(resources.GetObject("menuFile.Enabled")));
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
			this.menuFile.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuFile.Shortcut")));
			this.menuFile.ShowShortcut = ((bool)(resources.GetObject("menuFile.ShowShortcut")));
			this.menuFile.Text = resources.GetString("menuFile.Text");
			this.menuFile.Visible = ((bool)(resources.GetObject("menuFile.Visible")));
			// 
			// menuItemNewLayout
			// 
			this.menuItemNewLayout.Enabled = ((bool)(resources.GetObject("menuItemNewLayout.Enabled")));
			this.menuItemNewLayout.Index = 0;
			this.menuItemNewLayout.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemNewLayout.Shortcut")));
			this.menuItemNewLayout.ShowShortcut = ((bool)(resources.GetObject("menuItemNewLayout.ShowShortcut")));
			this.menuItemNewLayout.Text = resources.GetString("menuItemNewLayout.Text");
			this.menuItemNewLayout.Visible = ((bool)(resources.GetObject("menuItemNewLayout.Visible")));
			this.menuItemNewLayout.Click += new System.EventHandler(this.menuItemNewLayout_Click);
			// 
			// menuItemOpen
			// 
			this.menuItemOpen.Enabled = ((bool)(resources.GetObject("menuItemOpen.Enabled")));
			this.menuItemOpen.Index = 1;
			this.menuItemOpen.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemOpen.Shortcut")));
			this.menuItemOpen.ShowShortcut = ((bool)(resources.GetObject("menuItemOpen.ShowShortcut")));
			this.menuItemOpen.Text = resources.GetString("menuItemOpen.Text");
			this.menuItemOpen.Visible = ((bool)(resources.GetObject("menuItemOpen.Visible")));
			this.menuItemOpen.Click += new System.EventHandler(this.menuItemOpen_Click);
			// 
			// menuItemSave
			// 
			this.menuItemSave.Enabled = ((bool)(resources.GetObject("menuItemSave.Enabled")));
			this.menuItemSave.Index = 2;
			this.menuItemSave.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemSave.Shortcut")));
			this.menuItemSave.ShowShortcut = ((bool)(resources.GetObject("menuItemSave.ShowShortcut")));
			this.menuItemSave.Text = resources.GetString("menuItemSave.Text");
			this.menuItemSave.Visible = ((bool)(resources.GetObject("menuItemSave.Visible")));
			this.menuItemSave.Click += new System.EventHandler(this.menuItemSave_Click);
			// 
			// menuSaveAs
			// 
			this.menuSaveAs.Enabled = ((bool)(resources.GetObject("menuSaveAs.Enabled")));
			this.menuSaveAs.Index = 3;
			this.menuSaveAs.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuSaveAs.Shortcut")));
			this.menuSaveAs.ShowShortcut = ((bool)(resources.GetObject("menuSaveAs.ShowShortcut")));
			this.menuSaveAs.Text = resources.GetString("menuSaveAs.Text");
			this.menuSaveAs.Visible = ((bool)(resources.GetObject("menuSaveAs.Visible")));
			this.menuSaveAs.Click += new System.EventHandler(this.menuSaveAs_Click);
			// 
			// menuItemSeperator1
			// 
			this.menuItemSeperator1.Enabled = ((bool)(resources.GetObject("menuItemSeperator1.Enabled")));
			this.menuItemSeperator1.Index = 4;
			this.menuItemSeperator1.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemSeperator1.Shortcut")));
			this.menuItemSeperator1.ShowShortcut = ((bool)(resources.GetObject("menuItemSeperator1.ShowShortcut")));
			this.menuItemSeperator1.Text = resources.GetString("menuItemSeperator1.Text");
			this.menuItemSeperator1.Visible = ((bool)(resources.GetObject("menuItemSeperator1.Visible")));
			// 
			// menuItemModules
			// 
			this.menuItemModules.Enabled = ((bool)(resources.GetObject("menuItemModules.Enabled")));
			this.menuItemModules.Index = 5;
			this.menuItemModules.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemModules.Shortcut")));
			this.menuItemModules.ShowShortcut = ((bool)(resources.GetObject("menuItemModules.ShowShortcut")));
			this.menuItemModules.Text = resources.GetString("menuItemModules.Text");
			this.menuItemModules.Visible = ((bool)(resources.GetObject("menuItemModules.Visible")));
			this.menuItemModules.Click += new System.EventHandler(this.menuItemModules_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Enabled = ((bool)(resources.GetObject("menuItem3.Enabled")));
			this.menuItem3.Index = 6;
			this.menuItem3.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem3.Shortcut")));
			this.menuItem3.ShowShortcut = ((bool)(resources.GetObject("menuItem3.ShowShortcut")));
			this.menuItem3.Text = resources.GetString("menuItem3.Text");
			this.menuItem3.Visible = ((bool)(resources.GetObject("menuItem3.Visible")));
			// 
			// menuItemExit
			// 
			this.menuItemExit.Enabled = ((bool)(resources.GetObject("menuItemExit.Enabled")));
			this.menuItemExit.Index = 7;
			this.menuItemExit.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemExit.Shortcut")));
			this.menuItemExit.ShowShortcut = ((bool)(resources.GetObject("menuItemExit.ShowShortcut")));
			this.menuItemExit.Text = resources.GetString("menuItemExit.Text");
			this.menuItemExit.Visible = ((bool)(resources.GetObject("menuItemExit.Visible")));
			this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
			// 
			// menuEdit
			// 
			this.menuEdit.Enabled = ((bool)(resources.GetObject("menuEdit.Enabled")));
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
			this.menuEdit.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuEdit.Shortcut")));
			this.menuEdit.ShowShortcut = ((bool)(resources.GetObject("menuEdit.ShowShortcut")));
			this.menuEdit.Text = resources.GetString("menuEdit.Text");
			this.menuEdit.Visible = ((bool)(resources.GetObject("menuEdit.Visible")));
			this.menuEdit.Popup += new System.EventHandler(this.menuEdit_Popup);
			// 
			// menuItemUndo
			// 
			this.menuItemUndo.Enabled = ((bool)(resources.GetObject("menuItemUndo.Enabled")));
			this.menuItemUndo.Index = 0;
			this.menuItemUndo.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemUndo.Shortcut")));
			this.menuItemUndo.ShowShortcut = ((bool)(resources.GetObject("menuItemUndo.ShowShortcut")));
			this.menuItemUndo.Text = resources.GetString("menuItemUndo.Text");
			this.menuItemUndo.Visible = ((bool)(resources.GetObject("menuItemUndo.Visible")));
			this.menuItemUndo.Click += new System.EventHandler(this.menuItemUndo_Click);
			// 
			// menuItemRedo
			// 
			this.menuItemRedo.Enabled = ((bool)(resources.GetObject("menuItemRedo.Enabled")));
			this.menuItemRedo.Index = 1;
			this.menuItemRedo.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemRedo.Shortcut")));
			this.menuItemRedo.ShowShortcut = ((bool)(resources.GetObject("menuItemRedo.ShowShortcut")));
			this.menuItemRedo.Text = resources.GetString("menuItemRedo.Text");
			this.menuItemRedo.Visible = ((bool)(resources.GetObject("menuItemRedo.Visible")));
			this.menuItemRedo.Click += new System.EventHandler(this.menuItemRedo_Click);
			// 
			// menuItemSeparator
			// 
			this.menuItemSeparator.Enabled = ((bool)(resources.GetObject("menuItemSeparator.Enabled")));
			this.menuItemSeparator.Index = 2;
			this.menuItemSeparator.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemSeparator.Shortcut")));
			this.menuItemSeparator.ShowShortcut = ((bool)(resources.GetObject("menuItemSeparator.ShowShortcut")));
			this.menuItemSeparator.Text = resources.GetString("menuItemSeparator.Text");
			this.menuItemSeparator.Visible = ((bool)(resources.GetObject("menuItemSeparator.Visible")));
			// 
			// menuItemCopy
			// 
			this.menuItemCopy.Enabled = ((bool)(resources.GetObject("menuItemCopy.Enabled")));
			this.menuItemCopy.Index = 3;
			this.menuItemCopy.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemCopy.Shortcut")));
			this.menuItemCopy.ShowShortcut = ((bool)(resources.GetObject("menuItemCopy.ShowShortcut")));
			this.menuItemCopy.Text = resources.GetString("menuItemCopy.Text");
			this.menuItemCopy.Visible = ((bool)(resources.GetObject("menuItemCopy.Visible")));
			this.menuItemCopy.Click += new System.EventHandler(this.menuItemCopy_Click);
			// 
			// menuItemCut
			// 
			this.menuItemCut.Enabled = ((bool)(resources.GetObject("menuItemCut.Enabled")));
			this.menuItemCut.Index = 4;
			this.menuItemCut.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemCut.Shortcut")));
			this.menuItemCut.ShowShortcut = ((bool)(resources.GetObject("menuItemCut.ShowShortcut")));
			this.menuItemCut.Text = resources.GetString("menuItemCut.Text");
			this.menuItemCut.Visible = ((bool)(resources.GetObject("menuItemCut.Visible")));
			this.menuItemCut.Click += new System.EventHandler(this.menuItemCut_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Enabled = ((bool)(resources.GetObject("menuItem1.Enabled")));
			this.menuItem1.Index = 5;
			this.menuItem1.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem1.Shortcut")));
			this.menuItem1.ShowShortcut = ((bool)(resources.GetObject("menuItem1.ShowShortcut")));
			this.menuItem1.Text = resources.GetString("menuItem1.Text");
			this.menuItem1.Visible = ((bool)(resources.GetObject("menuItem1.Visible")));
			// 
			// menuItemStyles
			// 
			this.menuItemStyles.Enabled = ((bool)(resources.GetObject("menuItemStyles.Enabled")));
			this.menuItemStyles.Index = 6;
			this.menuItemStyles.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						   this.menuItemStyleFonts,
																						   this.menuItemStylePositions});
			this.menuItemStyles.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemStyles.Shortcut")));
			this.menuItemStyles.ShowShortcut = ((bool)(resources.GetObject("menuItemStyles.ShowShortcut")));
			this.menuItemStyles.Text = resources.GetString("menuItemStyles.Text");
			this.menuItemStyles.Visible = ((bool)(resources.GetObject("menuItemStyles.Visible")));
			// 
			// menuItemStyleFonts
			// 
			this.menuItemStyleFonts.Enabled = ((bool)(resources.GetObject("menuItemStyleFonts.Enabled")));
			this.menuItemStyleFonts.Index = 0;
			this.menuItemStyleFonts.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemStyleFonts.Shortcut")));
			this.menuItemStyleFonts.ShowShortcut = ((bool)(resources.GetObject("menuItemStyleFonts.ShowShortcut")));
			this.menuItemStyleFonts.Text = resources.GetString("menuItemStyleFonts.Text");
			this.menuItemStyleFonts.Visible = ((bool)(resources.GetObject("menuItemStyleFonts.Visible")));
			this.menuItemStyleFonts.Click += new System.EventHandler(this.menuItemStyleFonts_Click);
			// 
			// menuItemStylePositions
			// 
			this.menuItemStylePositions.Enabled = ((bool)(resources.GetObject("menuItemStylePositions.Enabled")));
			this.menuItemStylePositions.Index = 1;
			this.menuItemStylePositions.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemStylePositions.Shortcut")));
			this.menuItemStylePositions.ShowShortcut = ((bool)(resources.GetObject("menuItemStylePositions.ShowShortcut")));
			this.menuItemStylePositions.Text = resources.GetString("menuItemStylePositions.Text");
			this.menuItemStylePositions.Visible = ((bool)(resources.GetObject("menuItemStylePositions.Visible")));
			this.menuItemStylePositions.Click += new System.EventHandler(this.menuItemStylePositions_Click);
			// 
			// menuItemLocomotiveCatalog
			// 
			this.menuItemLocomotiveCatalog.Enabled = ((bool)(resources.GetObject("menuItemLocomotiveCatalog.Enabled")));
			this.menuItemLocomotiveCatalog.Index = 7;
			this.menuItemLocomotiveCatalog.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemLocomotiveCatalog.Shortcut")));
			this.menuItemLocomotiveCatalog.ShowShortcut = ((bool)(resources.GetObject("menuItemLocomotiveCatalog.ShowShortcut")));
			this.menuItemLocomotiveCatalog.Text = resources.GetString("menuItemLocomotiveCatalog.Text");
			this.menuItemLocomotiveCatalog.Visible = ((bool)(resources.GetObject("menuItemLocomotiveCatalog.Visible")));
			this.menuItemLocomotiveCatalog.Click += new System.EventHandler(this.menuItemLocomotiveCatalog_Click);
			// 
			// menuItemAccelerationProfiles
			// 
			this.menuItemAccelerationProfiles.Enabled = ((bool)(resources.GetObject("menuItemAccelerationProfiles.Enabled")));
			this.menuItemAccelerationProfiles.Index = 8;
			this.menuItemAccelerationProfiles.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemAccelerationProfiles.Shortcut")));
			this.menuItemAccelerationProfiles.ShowShortcut = ((bool)(resources.GetObject("menuItemAccelerationProfiles.ShowShortcut")));
			this.menuItemAccelerationProfiles.Text = resources.GetString("menuItemAccelerationProfiles.Text");
			this.menuItemAccelerationProfiles.Visible = ((bool)(resources.GetObject("menuItemAccelerationProfiles.Visible")));
			this.menuItemAccelerationProfiles.Click += new System.EventHandler(this.menuItemAccelerationProfiles_Click);
			// 
			// menuItemArea
			// 
			this.menuItemArea.Enabled = ((bool)(resources.GetObject("menuItemArea.Enabled")));
			this.menuItemArea.Index = 3;
			this.menuItemArea.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuItemNewArea,
																						 this.menuItemDeleteArea,
																						 this.menuItemRenameArea,
																						 this.menuSeperator2,
																						 this.menuItemAreaArrange});
			this.menuItemArea.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemArea.Shortcut")));
			this.menuItemArea.ShowShortcut = ((bool)(resources.GetObject("menuItemArea.ShowShortcut")));
			this.menuItemArea.Text = resources.GetString("menuItemArea.Text");
			this.menuItemArea.Visible = ((bool)(resources.GetObject("menuItemArea.Visible")));
			// 
			// menuItemNewArea
			// 
			this.menuItemNewArea.Enabled = ((bool)(resources.GetObject("menuItemNewArea.Enabled")));
			this.menuItemNewArea.Index = 0;
			this.menuItemNewArea.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemNewArea.Shortcut")));
			this.menuItemNewArea.ShowShortcut = ((bool)(resources.GetObject("menuItemNewArea.ShowShortcut")));
			this.menuItemNewArea.Text = resources.GetString("menuItemNewArea.Text");
			this.menuItemNewArea.Visible = ((bool)(resources.GetObject("menuItemNewArea.Visible")));
			this.menuItemNewArea.Click += new System.EventHandler(this.menuItemNewArea_Click);
			// 
			// menuItemDeleteArea
			// 
			this.menuItemDeleteArea.Enabled = ((bool)(resources.GetObject("menuItemDeleteArea.Enabled")));
			this.menuItemDeleteArea.Index = 1;
			this.menuItemDeleteArea.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemDeleteArea.Shortcut")));
			this.menuItemDeleteArea.ShowShortcut = ((bool)(resources.GetObject("menuItemDeleteArea.ShowShortcut")));
			this.menuItemDeleteArea.Text = resources.GetString("menuItemDeleteArea.Text");
			this.menuItemDeleteArea.Visible = ((bool)(resources.GetObject("menuItemDeleteArea.Visible")));
			this.menuItemDeleteArea.Click += new System.EventHandler(this.menuItemDeleteArea_Click);
			// 
			// menuItemRenameArea
			// 
			this.menuItemRenameArea.Enabled = ((bool)(resources.GetObject("menuItemRenameArea.Enabled")));
			this.menuItemRenameArea.Index = 2;
			this.menuItemRenameArea.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemRenameArea.Shortcut")));
			this.menuItemRenameArea.ShowShortcut = ((bool)(resources.GetObject("menuItemRenameArea.ShowShortcut")));
			this.menuItemRenameArea.Text = resources.GetString("menuItemRenameArea.Text");
			this.menuItemRenameArea.Visible = ((bool)(resources.GetObject("menuItemRenameArea.Visible")));
			this.menuItemRenameArea.Click += new System.EventHandler(this.menuItemRenameArea_Click);
			// 
			// menuSeperator2
			// 
			this.menuSeperator2.Enabled = ((bool)(resources.GetObject("menuSeperator2.Enabled")));
			this.menuSeperator2.Index = 3;
			this.menuSeperator2.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuSeperator2.Shortcut")));
			this.menuSeperator2.ShowShortcut = ((bool)(resources.GetObject("menuSeperator2.ShowShortcut")));
			this.menuSeperator2.Text = resources.GetString("menuSeperator2.Text");
			this.menuSeperator2.Visible = ((bool)(resources.GetObject("menuSeperator2.Visible")));
			// 
			// menuItemAreaArrange
			// 
			this.menuItemAreaArrange.Enabled = ((bool)(resources.GetObject("menuItemAreaArrange.Enabled")));
			this.menuItemAreaArrange.Index = 4;
			this.menuItemAreaArrange.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemAreaArrange.Shortcut")));
			this.menuItemAreaArrange.ShowShortcut = ((bool)(resources.GetObject("menuItemAreaArrange.ShowShortcut")));
			this.menuItemAreaArrange.Text = resources.GetString("menuItemAreaArrange.Text");
			this.menuItemAreaArrange.Visible = ((bool)(resources.GetObject("menuItemAreaArrange.Visible")));
			this.menuItemAreaArrange.Click += new System.EventHandler(this.menuItemAreaArrange_Click);
			// 
			// panelMessageViewer
			// 
			this.panelMessageViewer.AccessibleDescription = ((string)(resources.GetObject("panelMessageViewer.AccessibleDescription")));
			this.panelMessageViewer.AccessibleName = ((string)(resources.GetObject("panelMessageViewer.AccessibleName")));
			this.panelMessageViewer.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("panelMessageViewer.Anchor")));
			this.panelMessageViewer.AutoScroll = ((bool)(resources.GetObject("panelMessageViewer.AutoScroll")));
			this.panelMessageViewer.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("panelMessageViewer.AutoScrollMargin")));
			this.panelMessageViewer.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("panelMessageViewer.AutoScrollMinSize")));
			this.panelMessageViewer.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelMessageViewer.BackgroundImage")));
			this.panelMessageViewer.Controls.AddRange(new System.Windows.Forms.Control[] {
																							 this.panelTripsMonitor,
																							 this.splitterMessages});
			this.panelMessageViewer.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("panelMessageViewer.Dock")));
			this.panelMessageViewer.Enabled = ((bool)(resources.GetObject("panelMessageViewer.Enabled")));
			this.panelMessageViewer.Font = ((System.Drawing.Font)(resources.GetObject("panelMessageViewer.Font")));
			this.panelMessageViewer.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("panelMessageViewer.ImeMode")));
			this.panelMessageViewer.Location = ((System.Drawing.Point)(resources.GetObject("panelMessageViewer.Location")));
			this.panelMessageViewer.Name = "panelMessageViewer";
			this.panelMessageViewer.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("panelMessageViewer.RightToLeft")));
			this.panelMessageViewer.Size = ((System.Drawing.Size)(resources.GetObject("panelMessageViewer.Size")));
			this.panelMessageViewer.TabIndex = ((int)(resources.GetObject("panelMessageViewer.TabIndex")));
			this.panelMessageViewer.Text = resources.GetString("panelMessageViewer.Text");
			this.panelMessageViewer.Visible = ((bool)(resources.GetObject("panelMessageViewer.Visible")));
			// 
			// panelTripsMonitor
			// 
			this.panelTripsMonitor.AccessibleDescription = ((string)(resources.GetObject("panelTripsMonitor.AccessibleDescription")));
			this.panelTripsMonitor.AccessibleName = ((string)(resources.GetObject("panelTripsMonitor.AccessibleName")));
			this.panelTripsMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("panelTripsMonitor.Anchor")));
			this.panelTripsMonitor.AutoScroll = ((bool)(resources.GetObject("panelTripsMonitor.AutoScroll")));
			this.panelTripsMonitor.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("panelTripsMonitor.AutoScrollMargin")));
			this.panelTripsMonitor.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("panelTripsMonitor.AutoScrollMinSize")));
			this.panelTripsMonitor.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelTripsMonitor.BackgroundImage")));
			this.panelTripsMonitor.Controls.AddRange(new System.Windows.Forms.Control[] {
																							this.splitterTripsMonitor,
																							this.tabAreas});
			this.panelTripsMonitor.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("panelTripsMonitor.Dock")));
			this.panelTripsMonitor.Enabled = ((bool)(resources.GetObject("panelTripsMonitor.Enabled")));
			this.panelTripsMonitor.Font = ((System.Drawing.Font)(resources.GetObject("panelTripsMonitor.Font")));
			this.panelTripsMonitor.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("panelTripsMonitor.ImeMode")));
			this.panelTripsMonitor.Location = ((System.Drawing.Point)(resources.GetObject("panelTripsMonitor.Location")));
			this.panelTripsMonitor.Name = "panelTripsMonitor";
			this.panelTripsMonitor.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("panelTripsMonitor.RightToLeft")));
			this.panelTripsMonitor.Size = ((System.Drawing.Size)(resources.GetObject("panelTripsMonitor.Size")));
			this.panelTripsMonitor.TabIndex = ((int)(resources.GetObject("panelTripsMonitor.TabIndex")));
			this.panelTripsMonitor.Text = resources.GetString("panelTripsMonitor.Text");
			this.panelTripsMonitor.Visible = ((bool)(resources.GetObject("panelTripsMonitor.Visible")));
			// 
			// splitterTripsMonitor
			// 
			this.splitterTripsMonitor.AccessibleDescription = ((string)(resources.GetObject("splitterTripsMonitor.AccessibleDescription")));
			this.splitterTripsMonitor.AccessibleName = ((string)(resources.GetObject("splitterTripsMonitor.AccessibleName")));
			this.splitterTripsMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("splitterTripsMonitor.Anchor")));
			this.splitterTripsMonitor.BackColor = System.Drawing.SystemColors.WindowFrame;
			this.splitterTripsMonitor.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("splitterTripsMonitor.BackgroundImage")));
			this.splitterTripsMonitor.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("splitterTripsMonitor.Dock")));
			this.splitterTripsMonitor.Enabled = ((bool)(resources.GetObject("splitterTripsMonitor.Enabled")));
			this.splitterTripsMonitor.Font = ((System.Drawing.Font)(resources.GetObject("splitterTripsMonitor.Font")));
			this.splitterTripsMonitor.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("splitterTripsMonitor.ImeMode")));
			this.splitterTripsMonitor.Location = ((System.Drawing.Point)(resources.GetObject("splitterTripsMonitor.Location")));
			this.splitterTripsMonitor.MinExtra = ((int)(resources.GetObject("splitterTripsMonitor.MinExtra")));
			this.splitterTripsMonitor.MinSize = ((int)(resources.GetObject("splitterTripsMonitor.MinSize")));
			this.splitterTripsMonitor.Name = "splitterTripsMonitor";
			this.splitterTripsMonitor.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("splitterTripsMonitor.RightToLeft")));
			this.splitterTripsMonitor.Size = ((System.Drawing.Size)(resources.GetObject("splitterTripsMonitor.Size")));
			this.splitterTripsMonitor.TabIndex = ((int)(resources.GetObject("splitterTripsMonitor.TabIndex")));
			this.splitterTripsMonitor.TabStop = false;
			this.splitterTripsMonitor.Visible = ((bool)(resources.GetObject("splitterTripsMonitor.Visible")));
			this.splitterTripsMonitor.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitterTripsMonitor_SplitterMoved);
			// 
			// tabAreas
			// 
			this.tabAreas.AccessibleDescription = ((string)(resources.GetObject("tabAreas.AccessibleDescription")));
			this.tabAreas.AccessibleName = ((string)(resources.GetObject("tabAreas.AccessibleName")));
			this.tabAreas.Alignment = ((System.Windows.Forms.TabAlignment)(resources.GetObject("tabAreas.Alignment")));
			this.tabAreas.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tabAreas.Anchor")));
			this.tabAreas.Appearance = ((System.Windows.Forms.TabAppearance)(resources.GetObject("tabAreas.Appearance")));
			this.tabAreas.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabAreas.BackgroundImage")));
			this.tabAreas.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tabAreas.Dock")));
			this.tabAreas.Enabled = ((bool)(resources.GetObject("tabAreas.Enabled")));
			this.tabAreas.Font = ((System.Drawing.Font)(resources.GetObject("tabAreas.Font")));
			this.tabAreas.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tabAreas.ImeMode")));
			this.tabAreas.ItemSize = ((System.Drawing.Size)(resources.GetObject("tabAreas.ItemSize")));
			this.tabAreas.Location = ((System.Drawing.Point)(resources.GetObject("tabAreas.Location")));
			this.tabAreas.Multiline = true;
			this.tabAreas.Name = "tabAreas";
			this.tabAreas.Padding = ((System.Drawing.Point)(resources.GetObject("tabAreas.Padding")));
			this.tabAreas.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tabAreas.RightToLeft")));
			this.tabAreas.SelectedIndex = 0;
			this.tabAreas.ShowToolTips = ((bool)(resources.GetObject("tabAreas.ShowToolTips")));
			this.tabAreas.Size = ((System.Drawing.Size)(resources.GetObject("tabAreas.Size")));
			this.tabAreas.TabIndex = ((int)(resources.GetObject("tabAreas.TabIndex")));
			this.tabAreas.Text = resources.GetString("tabAreas.Text");
			this.tabAreas.Visible = ((bool)(resources.GetObject("tabAreas.Visible")));
			// 
			// splitterMessages
			// 
			this.splitterMessages.AccessibleDescription = ((string)(resources.GetObject("splitterMessages.AccessibleDescription")));
			this.splitterMessages.AccessibleName = ((string)(resources.GetObject("splitterMessages.AccessibleName")));
			this.splitterMessages.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("splitterMessages.Anchor")));
			this.splitterMessages.BackColor = System.Drawing.SystemColors.WindowFrame;
			this.splitterMessages.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("splitterMessages.BackgroundImage")));
			this.splitterMessages.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("splitterMessages.Dock")));
			this.splitterMessages.Enabled = ((bool)(resources.GetObject("splitterMessages.Enabled")));
			this.splitterMessages.Font = ((System.Drawing.Font)(resources.GetObject("splitterMessages.Font")));
			this.splitterMessages.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("splitterMessages.ImeMode")));
			this.splitterMessages.Location = ((System.Drawing.Point)(resources.GetObject("splitterMessages.Location")));
			this.splitterMessages.MinExtra = ((int)(resources.GetObject("splitterMessages.MinExtra")));
			this.splitterMessages.MinSize = ((int)(resources.GetObject("splitterMessages.MinSize")));
			this.splitterMessages.Name = "splitterMessages";
			this.splitterMessages.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("splitterMessages.RightToLeft")));
			this.splitterMessages.Size = ((System.Drawing.Size)(resources.GetObject("splitterMessages.Size")));
			this.splitterMessages.TabIndex = ((int)(resources.GetObject("splitterMessages.TabIndex")));
			this.splitterMessages.TabStop = false;
			this.splitterMessages.Visible = ((bool)(resources.GetObject("splitterMessages.Visible")));
			this.splitterMessages.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitterMessages_SplitterMoved);
			// 
			// splitterLocomotives
			// 
			this.splitterLocomotives.AccessibleDescription = ((string)(resources.GetObject("splitterLocomotives.AccessibleDescription")));
			this.splitterLocomotives.AccessibleName = ((string)(resources.GetObject("splitterLocomotives.AccessibleName")));
			this.splitterLocomotives.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("splitterLocomotives.Anchor")));
			this.splitterLocomotives.BackColor = System.Drawing.SystemColors.WindowText;
			this.splitterLocomotives.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("splitterLocomotives.BackgroundImage")));
			this.splitterLocomotives.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("splitterLocomotives.Dock")));
			this.splitterLocomotives.Enabled = ((bool)(resources.GetObject("splitterLocomotives.Enabled")));
			this.splitterLocomotives.Font = ((System.Drawing.Font)(resources.GetObject("splitterLocomotives.Font")));
			this.splitterLocomotives.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("splitterLocomotives.ImeMode")));
			this.splitterLocomotives.Location = ((System.Drawing.Point)(resources.GetObject("splitterLocomotives.Location")));
			this.splitterLocomotives.MinExtra = ((int)(resources.GetObject("splitterLocomotives.MinExtra")));
			this.splitterLocomotives.MinSize = ((int)(resources.GetObject("splitterLocomotives.MinSize")));
			this.splitterLocomotives.Name = "splitterLocomotives";
			this.splitterLocomotives.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("splitterLocomotives.RightToLeft")));
			this.splitterLocomotives.Size = ((System.Drawing.Size)(resources.GetObject("splitterLocomotives.Size")));
			this.splitterLocomotives.TabIndex = ((int)(resources.GetObject("splitterLocomotives.TabIndex")));
			this.splitterLocomotives.TabStop = false;
			this.splitterLocomotives.Visible = ((bool)(resources.GetObject("splitterLocomotives.Visible")));
			this.splitterLocomotives.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitterLocomotives_SplitterMoved);
			// 
			// panelLocomotiveViewer
			// 
			this.panelLocomotiveViewer.AccessibleDescription = ((string)(resources.GetObject("panelLocomotiveViewer.AccessibleDescription")));
			this.panelLocomotiveViewer.AccessibleName = ((string)(resources.GetObject("panelLocomotiveViewer.AccessibleName")));
			this.panelLocomotiveViewer.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("panelLocomotiveViewer.Anchor")));
			this.panelLocomotiveViewer.AutoScroll = ((bool)(resources.GetObject("panelLocomotiveViewer.AutoScroll")));
			this.panelLocomotiveViewer.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("panelLocomotiveViewer.AutoScrollMargin")));
			this.panelLocomotiveViewer.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("panelLocomotiveViewer.AutoScrollMinSize")));
			this.panelLocomotiveViewer.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelLocomotiveViewer.BackgroundImage")));
			this.panelLocomotiveViewer.Controls.AddRange(new System.Windows.Forms.Control[] {
																								this.panelMessageViewer,
																								this.splitterLocomotives});
			this.panelLocomotiveViewer.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("panelLocomotiveViewer.Dock")));
			this.panelLocomotiveViewer.Enabled = ((bool)(resources.GetObject("panelLocomotiveViewer.Enabled")));
			this.panelLocomotiveViewer.Font = ((System.Drawing.Font)(resources.GetObject("panelLocomotiveViewer.Font")));
			this.panelLocomotiveViewer.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("panelLocomotiveViewer.ImeMode")));
			this.panelLocomotiveViewer.Location = ((System.Drawing.Point)(resources.GetObject("panelLocomotiveViewer.Location")));
			this.panelLocomotiveViewer.Name = "panelLocomotiveViewer";
			this.panelLocomotiveViewer.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("panelLocomotiveViewer.RightToLeft")));
			this.panelLocomotiveViewer.Size = ((System.Drawing.Size)(resources.GetObject("panelLocomotiveViewer.Size")));
			this.panelLocomotiveViewer.TabIndex = ((int)(resources.GetObject("panelLocomotiveViewer.TabIndex")));
			this.panelLocomotiveViewer.Text = resources.GetString("panelLocomotiveViewer.Text");
			this.panelLocomotiveViewer.Visible = ((bool)(resources.GetObject("panelLocomotiveViewer.Visible")));
			// 
			// timerFreeResources
			// 
			this.timerFreeResources.Interval = 30000;
			this.timerFreeResources.SynchronizingObject = this;
			this.timerFreeResources.Elapsed += new System.Timers.ElapsedEventHandler(this.timerFreeResources_Elapsed);
			// 
			// LayoutController
			// 
			this.AccessibleDescription = ((string)(resources.GetObject("$this.AccessibleDescription")));
			this.AccessibleName = ((string)(resources.GetObject("$this.AccessibleName")));
			this.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("$this.Anchor")));
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.panelLocomotiveViewer,
																		  this.toolBarMain,
																		  this.statusBar});
			this.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("$this.Dock")));
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.Menu = this.mainMenu;
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "LayoutController";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.Visible = ((bool)(resources.GetObject("$this.Visible")));
			this.Resize += new System.EventHandler(this.LayoutController_Resize);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.LayoutController_Closing);
			this.Activated += new System.EventHandler(this.LayoutController_Activated);
			this.Deactivate += new System.EventHandler(this.LayoutController_Deactivate);
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelOperation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelMode)).EndInit();
			this.panelMessageViewer.ResumeLayout(false);
			this.panelTripsMonitor.ResumeLayout(false);
			this.panelLocomotiveViewer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.timerFreeResources)).EndInit();
			this.ResumeLayout(false);

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

		void UpdateTripsMonitorVisible() {
			bool	previousState = tripsMonitorVisible;

			tripsMonitorVisible = tripsMonitor.Height > 10;
			if(tripsMonitorVisible != previousState)
				eventManager.Event(new LayoutEvent(this, tripsMonitorVisible ? "trips-monitor-shown" : "trips-monitor-hidden"));
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

		#endregion

		#region Event handlers

		public void Areas_Added(object sender, EventArgs e) {
			LayoutModelArea	area = (LayoutModelArea)sender;

			tabAreas.TabPages.Add(new LayoutFrameWindowAreaTabPage(area));
		}

		public void Areas_Removed(object sender, EventArgs e) {
			LayoutModelArea	area = (LayoutModelArea)sender;

			foreach(LayoutFrameWindowAreaTabPage areaTabPage in tabAreas.TabPages) {
				if(areaTabPage.Area == area) {
					tabAreas.TabPages.Remove(areaTabPage);
					break;
				}
			}
		}

		public void Areas_Renamed(object sender, EventArgs e) {
			LayoutModelArea	area = (LayoutModelArea)sender;

			foreach(LayoutFrameWindowAreaTabPage areaTabPage in tabAreas.TabPages) {
				if(areaTabPage.Area == area) {
					areaTabPage.Text = area.Name;
					break;
				}
			}
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
				new ButtonToMenuMap(toolBarButtonShowMessages, new EventHandler(menuItemShowMessages_Click)),
				new ButtonToMenuMap(toolBarButtonShowTripsMonitor, new EventHandler(menuItemShowTripsMonitor_Click)),
				new ButtonToMenuMap(toolBarButtonDesignMode, new EventHandler(menuItemDesign_Click)),
				new ButtonToMenuMap(toolBarButtonOperationMode, new EventHandler(menuItemOperational_Click)),
				new ButtonToMenuMap(toolBarButtonToggleAllLayoutManualDispatch, new EventHandler(toggleAllLayoutManualDispatch)),
				new ButtonToMenuMap(toolBarButtonStopAllLocomotives, new EventHandler(menuItemSuspendLocomotives_Click)),
			};

			for(int i = 0; i < map.Length; i++) {
				if(map[i].theButton == e.Button) {
					map[i].theEvent(sender, e);
					break;
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

			if(showAddresses)
				menuItemShowAddresses.Text = "Hide Components &Address";
			else
				menuItemShowAddresses.Text = "Show Components &Address";
				
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

		#endregion

		private void menuItemShowAddresses_Click(object sender, System.EventArgs e) {
			showAddresses = !showAddresses;

			model.Redraw();
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
			Dialogs.PoliciesDefinition	p = new Dialogs.PoliciesDefinition(model);

			p.ShowDialog(this);
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
