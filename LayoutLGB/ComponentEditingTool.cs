using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;

using LayoutManager;
using LayoutManager.Model;

namespace LayoutLGB
{
	/// <summary>
	/// Summary description for ComponentTool.
	/// </summary>
	[LayoutModule("LGB MTS Component Editing Tool", UserControl=false)]
	public class ComponentTool : System.ComponentModel.Component, ILayoutModuleSetup
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
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

		public ComponentTool(System.ComponentModel.IContainer container)
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

		[LayoutEvent("model-component-placement-request", SenderType=typeof(MTScentralStation))]
		void PlaceTrackContactRequest(LayoutEvent e) {
			MTScentralStation	component = (MTScentralStation)e.Sender;
			Dialogs.CentralStationProperties csProperties = new Dialogs.CentralStationProperties(component);

			if(csProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				component.XmlInfo.XmlDocument = csProperties.XmlInfo.XmlDocument;
				e.Info = true;		// Place component
			}
			else
				e.Info = false;		// Do not place component
		}

		[LayoutEvent("query-component-editing-context-menu", SenderType=typeof(MTScentralStation))]
		void QueryTrackContactMenu(LayoutEvent e) {
			e.Info = e.Sender;
		}

		[LayoutEvent("add-component-editing-context-menu-entries", SenderType=typeof(MTScentralStation))]
		void AddTrackContactContextMenuEntries(LayoutEvent e) {
			Menu						menu = (Menu)e.Info;
			MTScentralStation			component = (MTScentralStation)e.Sender;

			menu.MenuItems.Add(new MTScentralStationMenuItemProperties(component));
		}

		#region MTS Central Station Menu Item Classes

		class MTScentralStationMenuItemProperties : MenuItem {
			MTScentralStation			component;

			internal MTScentralStationMenuItemProperties(MTScentralStation component) {
				this.component = component;
				this.Text = "&Properties";
			}

			protected override void OnClick(EventArgs e) {
				Dialogs.CentralStationProperties	csProperties = new Dialogs.CentralStationProperties(component);

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
			components = new System.ComponentModel.Container();
		}
		#endregion
	}
}
