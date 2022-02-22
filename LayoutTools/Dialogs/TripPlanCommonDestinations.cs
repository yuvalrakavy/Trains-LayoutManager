using LayoutManager.CommonUI;
using LayoutManager.Components;
using LayoutManager.Model;
using MethodDispatcher;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TripPlanManageCommonDestinations.
    /// </summary>
    public partial class TripPlanCommonDestinations : Form {
        const string E_Destination = "Destination";
        const string E_Desinations = "Destinations";
        private readonly TripPlanCatalogInfo tripPlanCatalog;
        private readonly LayoutSelection destinationSelection;
        private readonly ILayoutSelectionLook destinationSelectionLook = new LayoutSelectionLook(Color.Orange);
        private readonly LayoutSelection selectedLocationSelection;
        private readonly ILayoutSelectionLook selectedLocationSelectionLook = new LayoutSelectionLook(Color.OrangeRed);
        private DestinationNode? editedDestinationNode = null;
        private XmlDocument? destinationDoc = null;

        public TripPlanCommonDestinations() {
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

            this.Owner = LayoutController.ActiveFrameWindow as Form;

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

            if (selected is DestinationNode) {
                buttonDelete.Enabled = true;
                buttonEdit.Enabled = true;
                buttonAdd.Enabled = true;
            }
            else {
                buttonDelete.Enabled = false;
                buttonEdit.Enabled = false;
                buttonAdd.Enabled = selected == null;
            }

            destinationSelection.Clear();
            selectedLocationSelection.Clear();

            if (selected != null) {
                DestinationNode? selectedDestinationNode = null;
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
                    Dispatch.Call.EnsureComponentVisible(LayoutController.ActiveFrameWindow.Id, component);
            }
        }

        private void EditDestination(DestinationNode destinationNode) {
            destinationDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();
            XmlElement destinationsElement = destinationDoc.CreateElement(E_Desinations);
            XmlElement destinationElement = (XmlElement)destinationDoc.ImportNode(destinationNode.Destination.Element, true);

            editedDestinationNode = destinationNode;
            destinationDoc.AppendChild(destinationsElement);
            destinationsElement.AppendChild(destinationElement);
            TripPlanDestinationInfo destination = new(destinationElement);

            Dialogs.DestinationEditor destinationEditor = new(destination, "Edit Destination") {
                HideSaveButton = true
            };

            destinationSelection.Hide();
            selectedLocationSelection.Hide();

            new SemiModalDialog(this, destinationEditor, new SemiModalDialogClosedHandler(OnDestinationEditorClosed), null).ShowDialog();
        }

        private void OnDestinationEditorClosed(Form dialog, object? info) {
            Dialogs.DestinationEditor destinationEditor = (Dialogs.DestinationEditor)dialog;

            if (destinationEditor.DialogResult == DialogResult.OK) {
                var originalElement = destinationDoc?.DocumentElement?[E_Destination];

                if (originalElement != null) {
                    var destinationElement = (XmlElement)LayoutModel.StateManager.TripPlansCatalog.DestinationsElement.OwnerDocument.ImportNode(originalElement, true);
                    TripPlanDestinationInfo destination = new(destinationElement);

                    LayoutModel.StateManager.TripPlansCatalog.Destinations.Remove(destination.Id);
                    LayoutModel.StateManager.TripPlansCatalog.Destinations.Add(destination);

                    if (editedDestinationNode != null) {
                        editedDestinationNode.Destination = destination;
                        editedDestinationNode.UpdateBlocks();
                    }
                }
            }

            destinationSelection.Display(destinationSelectionLook);
            selectedLocationSelection.Display(selectedLocationSelectionLook);

            UpdateButtons();

            editedDestinationNode = null;
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
            destinationSelection.Hide();
            destinationSelection.Clear();
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

        private void ButtonEdit_Click(object? sender, System.EventArgs e) {
            var selected = GetSelectedDestination();

            if (selected != null) {
                destinationSelection.Hide();
                selectedLocationSelection.Hide();

                EditDestination(selected);
            }
        }

        private void ButtonAdd_Click(object? sender, System.EventArgs e) {
            destinationDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();
            XmlElement destinationsElement = destinationDoc.CreateElement(E_Desinations);
            XmlElement destinationElement = destinationDoc.CreateElement(E_Destination);

            destinationDoc.AppendChild(destinationsElement);
            destinationsElement.AppendChild(destinationElement);

            TripPlanDestinationInfo addedDestination = new(destinationElement);
            Dialogs.DestinationEditor destinationEditor = new(addedDestination, "New Destination") {
                HideSaveButton = true
            };

            new SemiModalDialog(this, destinationEditor, new SemiModalDialogClosedHandler(OnAddDestinationEditorClosed), null).ShowDialog();
        }

        private void OnAddDestinationEditorClosed(Form dialog, object? info) {
            Dialogs.DestinationEditor destinationEditor = (Dialogs.DestinationEditor)dialog;

            if (destinationEditor.DialogResult == DialogResult.OK) {
                var originalElement = destinationDoc?.DocumentElement?[E_Destination];

                if (originalElement != null) {
                    var destinationElement = Ensure.NotNull<XmlElement>(LayoutModel.StateManager.TripPlansCatalog.DestinationsElement.OwnerDocument.ImportNode(originalElement, true));
                    TripPlanDestinationInfo destination = new(destinationElement);

                    LayoutModel.StateManager.TripPlansCatalog.Destinations.Add(destination);
                    treeViewDestinations.Nodes.Add(new DestinationNode(destination));
                }
            }

            destinationDoc = null;
        }

        private void TripPlanCommonDestinations_Closing(object? sender, CancelEventArgs e) {
            if (Owner != null)
                Owner.Activate();
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
                    var block = Ensure.NotNull<LayoutBlock>(LayoutModel.Blocks[Entry.BlockId]);

                    return block.BlockDefinintion;
                }
            }
        }
    }
}
