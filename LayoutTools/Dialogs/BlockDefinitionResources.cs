using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;
using LayoutManager.Components;
using LayoutManager.CommonUI;

namespace LayoutManager.Tools.Dialogs {
	public partial class BlockDefinitionResources : Form {
		LayoutBlockDefinitionComponentInfo info;

		public BlockDefinitionResources(LayoutBlockDefinitionComponentInfo info) {
			InitializeComponent();

			this.info = info;
			info.Resources.CheckIntegrity(new LayoutModuleBase(), LayoutPhase.All);

			foreach(ResourceInfo resourceInfo in info.Resources)
				listBoxResources.Items.Add(new ResourceItem(resourceInfo));

			updateButtons(null, null);
		}

		private void updateButtons(object sender, EventArgs e) {
			if(listBoxResources.SelectedItem == null)
				buttonResourceRemove.Enabled = false;
			else
				buttonResourceRemove.Enabled = true;
		}

		private void buttonResourceAdd_Click(object sender, System.EventArgs e) {
			ContextMenu menu = new ContextMenu();

			new BuildComponentsMenu<IModelComponentLayoutLockResource>(LayoutModel.ActivePhases, new BuildComponentsMenuComponentFilter<IModelComponentLayoutLockResource>(this.addResourceFilter), 
				new EventHandler(onAddResourceComponent)).AddComponentMenuItems(menu);

			if(menu.MenuItems.Count > 0)
				menu.Show(buttonResourceAdd.Parent, new Point(buttonResourceAdd.Left, buttonResourceAdd.Bottom));
		}

		private bool addResourceFilter(IModelComponentLayoutLockResource component) {
			foreach(ResourceItem resourceItem in listBoxResources.Items) {
				if(resourceItem.ResourceInfo.ResourceId == component.Id)
					return false;
			}

			return true;
		}

		private void onAddResourceComponent(object sender, EventArgs e) {
			ModelComponentMenuItemBase<IModelComponentLayoutLockResource> menuItem = (ModelComponentMenuItemBase<IModelComponentLayoutLockResource>)sender;
			XmlElement resourceElement = info.Element.OwnerDocument.CreateElement("Resource");

			ResourceInfo resourceInfo = new ResourceInfo(
				new LayoutBlockDefinitionComponentInfo(info.BlockDefinition, info.Element), resourceElement);

			resourceInfo.ResourceId = menuItem.Component.Id;

			listBoxResources.Items.Add(new ResourceItem(resourceInfo));
		}

		private void buttonResourceRemove_Click(object sender, System.EventArgs e) {
			ResourceItem selected = (ResourceItem)listBoxResources.SelectedItem;

			if(selected != null)
				listBoxResources.Items.Remove(selected);
			updateButtons(null, null);
		}

		private void buttonOK_Click(object sender, EventArgs e) {
			ResourceCollection resourceCollection = info.Resources;

			resourceCollection.Clear();

			foreach(ResourceItem resourceItem in listBoxResources.Items)
				resourceCollection.Add(resourceItem.ResourceInfo.ResourceId);
            Close();
		}

		class ResourceItem {
			ResourceInfo resourceInfo;

			public ResourceItem(ResourceInfo resourceInfo) {
				this.resourceInfo = resourceInfo;
			}

            public ResourceInfo ResourceInfo => resourceInfo;

            public override string ToString() {
				string result;
				ILayoutLockResource resource = resourceInfo.GetResource(LayoutPhase.All);

				if(resource is ModelComponent) {
					result = ((ModelComponent)resource).ToString();

					if(resource is IModelComponentHasName)
						result += ": " + ((IModelComponentHasName)resource).NameProvider.Text;
				}
				else
					result = "Unknown resource type";

				return result;
			}
		}
	}
}