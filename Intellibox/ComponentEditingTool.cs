using System;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager;

namespace Intellibox {
    /// <summary>
    /// Summary description for ComponentTool.
    /// </summary>
    [LayoutModule("Intellibox Component Editing Tool", UserControl=false)]
	public class ComponentTool : System.ComponentModel.Component, ILayoutModuleSetup
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;
		LayoutModule		_module;

		#region Implementation of ILayoutModuleSetup

		public LayoutModule Module {
			set {
				_module = value;
			}

			get {
				return _module;
			}
		}

		#endregion

		#region Constructors

		public ComponentTool(IContainer container)
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			container.Add(this);
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public ComponentTool()
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		#endregion

		[LayoutEvent("model-component-placement-request", SenderType=typeof(IntelliboxComponent))]
		void PlaceTrackContactRequest(LayoutEvent e) {
			IntelliboxComponent	component = (IntelliboxComponent)e.Sender;
            using (Dialogs.CentralStationProperties csProperties = new Dialogs.CentralStationProperties(component)) {

                if (csProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    component.XmlInfo.XmlDocument = csProperties.XmlInfo.XmlDocument;
                    e.Info = true;		// Place component
                }
                else
                    e.Info = false;		// Do not place component
            }
		}

		[LayoutEvent("query-component-editing-context-menu", SenderType=typeof(IntelliboxComponent))]
		void QueryTrackContactMenu(LayoutEvent e) {
			e.Info = e.Sender;
		}

		[LayoutEvent("add-component-editing-context-menu-entries", SenderType=typeof(IntelliboxComponent))]
		void AddTrackContactContextMenuEntries(LayoutEvent e) {
			Menu						menu = (Menu)e.Info;
			IntelliboxComponent			component = (IntelliboxComponent)e.Sender;

			menu.MenuItems.Add(new IntelliboxMenuItemProperties(component));
		}

		#region Intellibox Menu Item Classes

		class IntelliboxMenuItemProperties : MenuItem {
			IntelliboxComponent			component;

			internal IntelliboxMenuItemProperties(IntelliboxComponent component) {
				this.component = component;
				this.Text = "&Properties";
			}

			protected override void OnClick(EventArgs e) {
				Dialogs.CentralStationProperties csProperties = new Dialogs.CentralStationProperties(component);

				if(csProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					LayoutModifyComponentDocumentCommand	modifyComponentDocumentCommand = 
						new LayoutModifyComponentDocumentCommand(component, csProperties.XmlInfo);

					LayoutController.Do(modifyComponentDocumentCommand);
				}
			}
		}

		#endregion

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new Container();
		}
		#endregion
	}
}
