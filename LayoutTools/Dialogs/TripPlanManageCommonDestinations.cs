using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TripPlanManageCommonDestinations.
    /// </summary>
    public partial class TripPlanManageCommonDestinations : Form {
        private readonly TripPlanCatalogInfo tripPlanCatalog;
        private readonly LayoutSelection destinationSelection;
        private readonly ILayoutSelectionLook destinationSelectionLook = new LayoutSelectionLook(Color.Orange);
        private readonly LayoutSelection selectedLocationSelection;
        private readonly ILayoutSelectionLook selectedLocationSelectionLook = new LayoutSelectionLook(Color.OrangeRed);

        public TripPlanManageCommonDestinations() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            tripPlanCatalog = LayoutModel.StateManager.TripPlansCatalog;

            foreach (TripPlanDestinationInfo destination in tripPlanCatalog.Destinations)
                treeViewDestinations.Nodes.Add(new DestinationNode(destination));

            destinationSelection = new LayoutSelection();
            selectedLocationSelection = new LayoutSelection();

            destinationSelection.Display(destinationSelectionLook);
            selectedLocationSelection.Display(selectedLocationSelectionLook);

            UpdateButtons();
        }

        private DestinationNode? GetSelectedDestination() {
            TreeNode selected = treeViewDestinations.SelectedNode;

            if (selected is DestinationNode desintation)
                return desintation;
            else return selected is DestinationEntryNode ? (DestinationNode)selected.Parent : null;
        }

        private void UpdateButtons() {
            TreeNode selected = treeViewDestinations.SelectedNode;

            if (selected != null && selected is DestinationNode) {
                buttonDelete.Enabled = true;
                buttonRename.Enabled = true;
            }
            else {
                buttonDelete.Enabled = false;
                buttonRename.Enabled = false;
            }

            destinationSelection.Clear();
            selectedLocationSelection.Clear();

            if (selected != null) {
                DestinationNode?selectedDestinationNode = null;
                DestinationEntryNode? selectedDestinationEntryNode = null;

                if (selected is DestinationNode destination)
                    selectedDestinationNode = destination;
                else if (selected is DestinationEntryNode destinationEntry) {
                    selectedDestinationEntryNode = destinationEntry;
                    selectedDestinationNode = (DestinationNode)selected.Parent;
                }

                if (selectedDestinationNode != null)
                    destinationSelection.Add(selectedDestinationNode.Destination.Selection);
                if (selectedDestinationEntryNode != null)
                    selectedLocationSelection.Add(selectedDestinationEntryNode.BlockInfo);

                ModelComponent? component = null;

                if (selectedLocationSelection.Count > 0)
                    component = selectedLocationSelection.Components.First();
                else if (destinationSelection.Count > 0)
                    component = destinationSelection.Components.First();

                if (component != null)
                    EventManager.Event(new LayoutEvent("ensure-component-visible", component, false));
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void ButtonClose_Click(object? sender, System.EventArgs e) {
            selectedLocationSelection.Hide();
            selectedLocationSelection.Clear();
            selectedLocationSelection.Hide();
            selectedLocationSelection.Clear();

            this.Close();
        }

        private void TreeViewDestinations_AfterSelect(object? sender, System.Windows.Forms.TreeViewEventArgs e) {
            UpdateButtons();
        }

        private void ButtonDelete_Click(object? sender, System.EventArgs e) {
            var selected = GetSelectedDestination();

            if (selected != null) {
                tripPlanCatalog.Destinations.Remove(selected.Destination);
                selected.Remove();
            }

            UpdateButtons();
        }

        private void ButtonRename_Click(object? sender, System.EventArgs e) {
            var selected = GetSelectedDestination();

            if (selected != null) {
                var name = CommonUI.Dialogs.InputBox.Show("Enter new name", "Rename Destination");

                if (name != null) {
                    TripPlanDestinationInfo existingDestination = tripPlanCatalog.Destinations[name];

                    if (existingDestination != null && existingDestination != selected.Destination)
                        MessageBox.Show(this, "A destination with this name already exists", "Invalid name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else {
                        selected.Destination.Name = name;
                        selected.Text = name;
                    }
                }
            }
        }

        private class DestinationNode : TreeNode {
            private TripPlanDestinationInfo destination;

            public DestinationNode(TripPlanDestinationInfo destination) {
                this.destination = destination;

                this.Text = destination.Name;

                UpdateBlocks();
            }

            public TripPlanDestinationInfo Destination {
                get {
                    return destination;
                }

                set {
                    destination = value;
                    Text = destination.Name;
                }
            }

            public void UpdateBlocks() {
                Nodes.Clear();

                foreach (TripPlanDestinationEntryInfo entry in destination.Entries)
                    Nodes.Add(new DestinationEntryNode(entry));
            }
        }

        private class DestinationEntryNode : TreeNode {
            public DestinationEntryNode(TripPlanDestinationEntryInfo entry) {
                this.Entry = entry;

                LayoutBlock block = LayoutModel.Blocks[entry.BlockId];
                string condition = "";

                if (!entry.IsConditionEmpty)
                    condition = " [" + entry.GetDescription() + "]";

                if (block != null)
                    Text = block.BlockDefinintion.Name + condition;
                else
                    Text = "*UNKNOWN*" + condition;
            }

            public TripPlanDestinationEntryInfo Entry { get; }

            public LayoutBlockDefinitionComponent BlockInfo {
                get {
                    LayoutBlock block = LayoutModel.Blocks[Entry.BlockId];

                    return Ensure.NotNull<LayoutBlockDefinitionComponent>(block?.BlockDefinintion);
                }
            }
        }
    }
}
