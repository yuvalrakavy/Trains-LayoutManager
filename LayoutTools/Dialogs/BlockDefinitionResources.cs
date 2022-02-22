using LayoutManager.CommonUI;
using LayoutManager.Components;
using LayoutManager.Model;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Tools.Dialogs {
    public partial class BlockDefinitionResources : Form {
        private readonly LayoutBlockDefinitionComponentInfo info;

        public BlockDefinitionResources(LayoutBlockDefinitionComponentInfo info) {
            InitializeComponent();

            this.info = info;
            info.Resources.CheckIntegrity(new LayoutModuleBase(), LayoutPhase.All);

            foreach (ResourceInfo resourceInfo in info.Resources)
                listBoxResources.Items.Add(new ResourceItem(resourceInfo));

            UpdateButtons(null, EventArgs.Empty);
        }

        private void UpdateButtons(object? sender, EventArgs e) {
            if (listBoxResources.SelectedItem == null)
                buttonResourceRemove.Enabled = false;
            else
                buttonResourceRemove.Enabled = true;
        }

        private void ButtonResourceAdd_Click(object? sender, System.EventArgs e) {
            var menu = new ContextMenuStrip();

            new BuildComponentsMenu<IModelComponentLayoutLockResource>(LayoutModel.ActivePhases, new BuildComponentsMenuComponentFilter<IModelComponentLayoutLockResource>(this.AddResourceFilter),
                new EventHandler(OnAddResourceComponent)).AddComponentMenuItems(new MenuOrMenuItem(menu));

            if (menu.Items.Count > 0)
                menu.Show(buttonResourceAdd.Parent, new Point(buttonResourceAdd.Left, buttonResourceAdd.Bottom));
        }

        private bool AddResourceFilter(IModelComponentLayoutLockResource component) {
            foreach (ResourceItem resourceItem in listBoxResources.Items) {
                if (resourceItem.ResourceInfo.ResourceId == component.Id)
                    return false;
            }

            return true;
        }

        private void OnAddResourceComponent(object? sender, EventArgs e) {
            var menuItem = Ensure.NotNull<ModelComponentMenuItemBase<IModelComponentLayoutLockResource>>(sender);
            XmlElement resourceElement = info.Element.OwnerDocument.CreateElement("Resource");

            if (menuItem.Component != null) {
                ResourceInfo resourceInfo = new(
                    resourceElement) {
                    ResourceId = menuItem.Component.Id
                };

                listBoxResources.Items.Add(new ResourceItem(resourceInfo));
            }
        }

        private void ButtonResourceRemove_Click(object? sender, System.EventArgs e) {
            ResourceItem selected = (ResourceItem)listBoxResources.SelectedItem;

            if (selected != null)
                listBoxResources.Items.Remove(selected);
            UpdateButtons(null, EventArgs.Empty);
        }

        private void ButtonOK_Click(object? sender, EventArgs e) {
            ResourceCollection resourceCollection = info.Resources;

            resourceCollection.Clear();

            foreach (ResourceItem resourceItem in listBoxResources.Items)
                resourceCollection.Add(resourceItem.ResourceInfo.ResourceId);
            Close();
        }

        private class ResourceItem {
            public ResourceItem(ResourceInfo resourceInfo) {
                this.ResourceInfo = resourceInfo;
            }

            public ResourceInfo ResourceInfo { get; }

            public override string ToString() {
                string result;
                var resource = ResourceInfo.GetResource(LayoutPhase.All);

                if (resource is ModelComponent component) {
                    result = component.ToString();

                    if (resource is IModelComponentHasName name)
                        result += ": " + name.NameProvider.Text;
                }
                else
                    result = "Unknown resource type";

                return result;
            }
        }
    }
}