using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Linq;
using LayoutManager.CommonUI;
using LayoutManager.Model;
using LayoutManager.Components;

#pragma warning disable IDE0051, IDE0060
namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TripPlanManageCommonDestinations.
    /// </summary>
    public class TripPlanCommonDestinations : Form {
        private TreeView treeViewDestinations;
        private Button buttonDelete;
        private Button buttonClose;
        private Button buttonEdit;
        private Button buttonAdd;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private void endOfDesignerVariables() { }

        readonly TripPlanCatalogInfo tripPlanCatalog;
        readonly LayoutSelection destinationSelection;
        readonly ILayoutSelectionLook destinationSelectionLook = new LayoutSelectionLook(Color.Orange);
        readonly LayoutSelection selectedLocationSelection;
        readonly ILayoutSelectionLook selectedLocationSelectionLook = new LayoutSelectionLook(Color.OrangeRed);
        DestinationNode editedDestinationNode = null;
        XmlDocument destinationDoc = null;

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

            updateButtons();
        }

        private DestinationNode getSelectedDestination() {
            TreeNode selected = treeViewDestinations.SelectedNode;

            if (selected is DestinationNode)
                return (DestinationNode)selected;
            else if (selected is DestinationEntryNode)
                return (DestinationNode)selected.Parent;
            else
                return null;
        }

        private void updateButtons() {
            TreeNode selected = treeViewDestinations.SelectedNode;

            if (selected is DestinationNode) {
                buttonDelete.Enabled = true;
                buttonEdit.Enabled = true;
                buttonAdd.Enabled = true;
            }
            else {
                buttonDelete.Enabled = false;
                buttonEdit.Enabled = false;
                buttonAdd.Enabled = (selected == null);
            }

            destinationSelection.Clear();
            selectedLocationSelection.Clear();

            if (selected != null) {
                DestinationNode selectedDestinationNode = null;
                DestinationEntryNode selectedDestinationEntryNode = null;

                if (selected is DestinationNode)
                    selectedDestinationNode = (DestinationNode)selected;
                else if (selected is DestinationEntryNode) {
                    selectedDestinationEntryNode = (DestinationEntryNode)selected;
                    selectedDestinationNode = (DestinationNode)selected.Parent;
                }

                if (selectedDestinationNode != null)
                    destinationSelection.Add(selectedDestinationNode.Destination.Selection);
                if (selectedDestinationEntryNode != null)
                    selectedLocationSelection.Add(selectedDestinationEntryNode.BlockInfo);

                ModelComponent component = null;

                if (selectedLocationSelection.Count > 0)
                    component = selectedLocationSelection.Components.First();
                else if (destinationSelection.Count > 0)
                    component = destinationSelection.Components.First();

                if (component != null)
                    EventManager.Event(new LayoutEvent("ensure-component-visible", component, false));
            }
        }

        private void editDestination(DestinationNode destinationNode) {
            destinationDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();
            XmlElement destinationsElement = destinationDoc.CreateElement("Destinations");
            XmlElement destinationElement = (XmlElement)destinationDoc.ImportNode(destinationNode.Destination.Element, true);

            editedDestinationNode = destinationNode;
            destinationDoc.AppendChild(destinationsElement);
            destinationsElement.AppendChild(destinationElement);
            TripPlanDestinationInfo destination = new TripPlanDestinationInfo(destinationElement);

            Dialogs.DestinationEditor destinationEditor = new Dialogs.DestinationEditor(destination, "Edit Destination") {
                HideSaveButton = true
            };

            destinationSelection.Hide();
            selectedLocationSelection.Hide();

            new SemiModalDialog(this, destinationEditor, new SemiModalDialogClosedHandler(onDestinationEditorClosed), null).ShowDialog();
        }

        private void onDestinationEditorClosed(Form dialog, object info) {
            Dialogs.DestinationEditor destinationEditor = (Dialogs.DestinationEditor)dialog;

            if (destinationEditor.DialogResult == DialogResult.OK) {
                XmlElement destinationElement = (XmlElement)LayoutModel.StateManager.TripPlansCatalog.DestinationsElement.OwnerDocument.ImportNode(destinationDoc.DocumentElement["Destination"], true);
                TripPlanDestinationInfo destination = new TripPlanDestinationInfo(destinationElement);

                LayoutModel.StateManager.TripPlansCatalog.Destinations.Remove(destination.Id);
                LayoutModel.StateManager.TripPlansCatalog.Destinations.Add(destination);

                editedDestinationNode.Destination = destination;
                editedDestinationNode.UpdateBlocks();
            }

            destinationSelection.Display(destinationSelectionLook);
            selectedLocationSelection.Display(selectedLocationSelectionLook);

            updateButtons();

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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.treeViewDestinations = new TreeView();
            this.buttonDelete = new Button();
            this.buttonEdit = new Button();
            this.buttonClose = new Button();
            this.buttonAdd = new Button();
            this.SuspendLayout();
            // 
            // treeViewDestinations
            // 
            this.treeViewDestinations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewDestinations.HideSelection = false;
            this.treeViewDestinations.ImageIndex = -1;
            this.treeViewDestinations.Location = new System.Drawing.Point(8, 8);
            this.treeViewDestinations.Name = "treeViewDestinations";
            this.treeViewDestinations.SelectedImageIndex = -1;
            this.treeViewDestinations.ShowLines = false;
            this.treeViewDestinations.Size = new System.Drawing.Size(320, 208);
            this.treeViewDestinations.TabIndex = 0;
            this.treeViewDestinations.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDestinations_AfterSelect);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDelete.Location = new System.Drawing.Point(136, 224);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(56, 19);
            this.buttonDelete.TabIndex = 3;
            this.buttonDelete.Text = "&Delete";
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonEdit.Location = new System.Drawing.Point(72, 224);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(56, 19);
            this.buttonEdit.TabIndex = 2;
            this.buttonEdit.Text = "&Edit...";
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(272, 224);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(56, 19);
            this.buttonClose.TabIndex = 4;
            this.buttonClose.Text = "&Close";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAdd.Location = new System.Drawing.Point(8, 224);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(56, 19);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "&Add...";
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // TripPlanCommonDestinations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.ClientSize = new System.Drawing.Size(336, 254);
            this.ControlBox = false;
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.treeViewDestinations);
            this.Controls.Add(this.buttonEdit);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonAdd);
            this.Name = "TripPlanCommonDestinations";
            this.ShowInTaskbar = false;
            this.Text = "\"Smart\" Destinations";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.TripPlanCommonDestinations_Closing);
            this.ResumeLayout(false);

        }
        #endregion

        private void buttonClose_Click(object sender, System.EventArgs e) {
            destinationSelection.Hide();
            destinationSelection.Clear();
            selectedLocationSelection.Hide();
            selectedLocationSelection.Clear();

            this.Close();
        }

        private void treeViewDestinations_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e) {
            updateButtons();
        }

        private void buttonDelete_Click(object sender, System.EventArgs e) {
            DestinationNode selected = getSelectedDestination();

            if (selected != null) {
                tripPlanCatalog.Destinations.Remove(selected.Destination);
                selected.Remove();
            }

            updateButtons();
        }

        private void buttonEdit_Click(object sender, System.EventArgs e) {
            DestinationNode selected = getSelectedDestination();

            if (selected != null) {
                destinationSelection.Hide();
                selectedLocationSelection.Hide();

                editDestination(selected);
            }
        }

        private void buttonAdd_Click(object sender, System.EventArgs e) {
            destinationDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();
            XmlElement destinationsElement = destinationDoc.CreateElement("Destinations");
            XmlElement destinationElement = destinationDoc.CreateElement("Destination");

            destinationDoc.AppendChild(destinationsElement);
            destinationsElement.AppendChild(destinationElement);

            TripPlanDestinationInfo addedDestination = new TripPlanDestinationInfo(destinationElement);
            Dialogs.DestinationEditor destinationEditor = new Dialogs.DestinationEditor(addedDestination, "New Destination") {
                HideSaveButton = true
            };

            new SemiModalDialog(this, destinationEditor, new SemiModalDialogClosedHandler(onAddDestinationEditorClosed), null).ShowDialog();
        }

        private void onAddDestinationEditorClosed(Form dialog, object info) {
            Dialogs.DestinationEditor destinationEditor = (Dialogs.DestinationEditor)dialog;

            if (destinationEditor.DialogResult == DialogResult.OK) {
                XmlElement destinationElement = (XmlElement)LayoutModel.StateManager.TripPlansCatalog.DestinationsElement.OwnerDocument.ImportNode(destinationDoc.DocumentElement["Destination"], true);
                TripPlanDestinationInfo destination = new TripPlanDestinationInfo(destinationElement);

                LayoutModel.StateManager.TripPlansCatalog.Destinations.Add(destination);
                treeViewDestinations.Nodes.Add(new DestinationNode(destination));
            }

            destinationDoc = null;
        }

        private void TripPlanCommonDestinations_Closing(object sender, CancelEventArgs e) {
            if (Owner != null)
                Owner.Activate();
        }

        class DestinationNode : TreeNode {
            TripPlanDestinationInfo destination;

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

        class DestinationEntryNode : TreeNode {
            readonly TripPlanDestinationEntryInfo entry;

            public DestinationEntryNode(TripPlanDestinationEntryInfo entry) {
                this.entry = entry;

                LayoutBlock block = LayoutModel.Blocks[entry.BlockId];
                string condition = "";

                if (!entry.IsConditionEmpty)
                    condition = " [" + entry.GetDescription() + "]";

                if (block != null)
                    Text = block.BlockDefinintion.Name + condition;
                else
                    Text = "*UNKNOWN*" + condition;
            }

            public TripPlanDestinationEntryInfo Entry => entry;

            public LayoutBlockDefinitionComponent BlockInfo {
                get {
                    LayoutBlock block = LayoutModel.Blocks[entry.BlockId];

                    if (block != null)
                        return block.BlockDefinintion;
                    return null;
                }
            }
        }
    }

}
