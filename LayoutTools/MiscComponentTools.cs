
using System;
using System.Collections;
using System.Xml;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;
using LayoutManager.Tools;

namespace LayoutManager.Tools {
	public class LayoutTrackLinkComponentTool : LayoutComponentTool {

		public override String ComponentName {
			get {
				return "Track link";
			}
		}

		public override bool HasContextMenu {
			get {
				return true;
			}
		}

		public override bool OnCreateComponent(ILayoutController controller, LayoutModelArea area, Point ml) {
			Dialogs.TrackLinkProperties	trackLinkProperties = new Dialogs.TrackLinkProperties(area, (LayoutTrackLinkComponent)Component);

			if(trackLinkProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				Component.XmlInfo.XmlDocument = trackLinkProperties.XmlInfo.XmlDocument;

				LayoutCompoundCommand	addCommand = new LayoutCompoundCommand("Add track-link");

				addCommand.Add(new LayoutComponentPlacmentCommand(area, ml, Component, "Add track link"));

				if(trackLinkProperties.TrackLink != null) {
					LayoutTrackLinkComponent	destinationTrackLinkComponent = trackLinkProperties.TrackLink.ResolveLink(area.Model);

					if(destinationTrackLinkComponent.Link != null)
						addCommand.Add(new LayoutComponentUnlinkCommand(area.Model, destinationTrackLinkComponent));

					addCommand.Add(new LayoutComponentLinkCommand(area.Model, (LayoutTrackLinkComponent)Component, trackLinkProperties.TrackLink));
				}

				controller.Do(addCommand);
			}

			return false;			// Do not place component, it is already placed
		}

		public override bool AddContextMenuItems(ILayoutController controller, Menu menu) {
			menu.MenuItems.Add(new LayoutTrackLinkComponentTool.MenuItemProperties(controller, (LayoutTrackLinkComponent)Component));
			return true;
		}

		#region Context menu operations

		class MenuItemProperties : MenuItem {
			ILayoutController			controller;
			LayoutTrackLinkComponent	trackLinkComponent;

			internal MenuItemProperties(ILayoutController controller, LayoutTrackLinkComponent trackLinkComponent) {
				this.controller = controller;
				this.trackLinkComponent = trackLinkComponent;
				this.Text = "&Properties";
			}

			protected override void OnClick(EventArgs e) {
				Dialogs.TrackLinkProperties	trackLinkProperties = new Dialogs.TrackLinkProperties(trackLinkComponent.Spot.Area, trackLinkComponent);
				LayoutModel					model = trackLinkComponent.Spot.Area.Model;

				if(trackLinkProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					LayoutCompoundCommand	editProperties = new LayoutCompoundCommand("edit " + trackLinkComponent.ToString() + " properties");

					LayoutModifyComponentDocumentCommand	modifyComponentDocumentCommand = 
						new LayoutModifyComponentDocumentCommand(trackLinkComponent, trackLinkProperties.XmlInfo);

					editProperties.Add(modifyComponentDocumentCommand);

					// If this component is to be linked
					if(trackLinkProperties.TrackLink != null) {
						// Unlink this component if it is already linked
						if(trackLinkComponent.Link != null) {
							LayoutComponentUnlinkCommand	unlinkCommand = new LayoutComponentUnlinkCommand(model, trackLinkComponent);

							editProperties.Add(unlinkCommand);
						}

						LayoutTrackLinkComponent	destinationTrackLinkComponent = trackLinkProperties.TrackLink.ResolveLink(model);

						// Unlink destination component if it is linked
						if(destinationTrackLinkComponent.Link != null) {
							LayoutComponentUnlinkCommand	unlinkCommand = new LayoutComponentUnlinkCommand(model, destinationTrackLinkComponent);

							editProperties.Add(unlinkCommand);
						}

						// Link to the destination component
						LayoutComponentLinkCommand	linkCommand = new LayoutComponentLinkCommand(model, trackLinkComponent, trackLinkProperties.TrackLink);

						editProperties.Add(linkCommand);
					}
					else {
						// If component was linked, remove the link
						if(trackLinkComponent.Link != null) {
							LayoutComponentUnlinkCommand	unlinkCommand = new LayoutComponentUnlinkCommand(model, trackLinkComponent);

							editProperties.Add(unlinkCommand);
						}
					}

					controller.Do(editProperties);
				}
			}
		}

		#endregion
	}

	public class LayoutTextComponentTool : LayoutComponentTool {

		public override String ComponentName {
			get {
				LayoutTextInfo	textProvider = new LayoutTextInfo(Component, "Text");

				return "Text: " + textProvider.Text;
			}
		}

		public override bool HasContextMenu {
			get {
				return true;
			}
		}

		public override bool OnCreateComponent(ILayoutController controller, LayoutModelArea area, Point ml) {
			Dialogs.TextProperties	textProperties = new Dialogs.TextProperties(controller.Model, (LayoutTextComponent)Component);

			if(textProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				Component.XmlInfo.XmlDocument = textProperties.XmlInfo.XmlDocument;
			else
				return false;		// Do not place the component

			return true;
		}

		public override bool AddContextMenuItems(ILayoutController controller, Menu menu) {
			menu.MenuItems.Add(new LayoutTextComponentTool.MenuItemProperties(controller, (LayoutTextComponent)Component));
			return true;
		}

		#region Context menu operations

		class MenuItemProperties : MenuItem {
			ILayoutController			controller;
			LayoutTextComponent			textComponent;

			internal MenuItemProperties(ILayoutController controller, LayoutTextComponent textComponent) {
				this.controller = controller;
				this.textComponent = textComponent;
				this.Text = "&Properties";
			}

			protected override void OnClick(EventArgs e) {
				Dialogs.TextProperties	textProperties = new Dialogs.TextProperties(controller.Model, textComponent);

				if(textProperties.ShowDialog() == DialogResult.OK) {
					LayoutModifyComponentDocumentCommand	modifyComponentDocumentCommand = 
						new LayoutModifyComponentDocumentCommand(textComponent, textProperties.XmlInfo);

					controller.Do(modifyComponentDocumentCommand);
				}
			}
		}

		#endregion

	}

	public class LayoutTrackContactComponentTool : LayoutComponentTool {

		public override String ComponentName {
			get {
				return "Track contact";
			}
		}

		public override bool HasContextMenu {
			get {
				return true;
			}
		}

		public override bool OnCreateComponent(ILayoutController controller, LayoutModelArea area, Point ml) {
			Dialogs.TrackContactProperties	trackContactProperties = new Dialogs.TrackContactProperties(area.Model, (LayoutTrackContactComponent)this.Component);

			if(trackContactProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				Component.XmlInfo.XmlDocument = trackContactProperties.XmlInfo.XmlDocument;
				return true;		// Place component
			}

			return false;			// Do not place component
		}

		public override bool AddContextMenuItems(ILayoutController controller, Menu menu) {
			menu.MenuItems.Add(new LayoutTrackContactComponentTool.MenuItemProperties(controller, (LayoutTrackContactComponent)Component));
			return true;
		}

		#region Context menu operations

		class MenuItemProperties : MenuItem {
			ILayoutController			controller;
			LayoutTrackContactComponent	trackContactComponent;

			internal MenuItemProperties(ILayoutController controller, LayoutTrackContactComponent trackContactComponent) {
				this.controller = controller;
				this.trackContactComponent = trackContactComponent;
				this.Text = "&Properties";
			}

			protected override void OnClick(EventArgs e) {
				LayoutModel						model = trackContactComponent.Spot.Area.Model;
				Dialogs.TrackContactProperties	trackContactProperties = new Dialogs.TrackContactProperties(model, trackContactComponent);

				if(trackContactProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					LayoutModifyComponentDocumentCommand	modifyComponentDocumentCommand = 
						new LayoutModifyComponentDocumentCommand(trackContactComponent, trackContactProperties.XmlInfo);


					controller.Do(modifyComponentDocumentCommand);
				}
			}
		}

		#endregion
	}

}
