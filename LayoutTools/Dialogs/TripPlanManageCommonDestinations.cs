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
    public class TripPlanManageCommonDestinations : Form {
        private TreeView treeViewDestinations;
        private Button buttonDelete;
        private Button buttonRename;
        private Button buttonClose;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
        readonly TripPlanCatalogInfo tripPlanCatalog;
        readonly LayoutSelection destinationSelection;
        readonly ILayoutSelectionLook destinationSelectionLook = new LayoutSelectionLook(Color.Orange);
        readonly LayoutSelection selectedLocationSelection;
        readonly ILayoutSelectionLook selectedLocationSelectionLook = new LayoutSelectionLook(Color.OrangeRed);

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
                    EventManager.Event(new LayoutEvent(component, "ensure-component-visible", null, false));
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.treeViewDestinations = new TreeView();
            this.buttonDelete = new Button();
            this.buttonRename = new Button();
            this.buttonClose = new Button();
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
            this.treeViewDestinations.Size = new System.Drawing.Size(216, 208);
            this.treeViewDestinations.TabIndex = 0;
            this.treeViewDestinations.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDestinations_AfterSelect);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDelete.Location = new System.Drawing.Point(8, 224);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(56, 19);
            this.buttonDelete.TabIndex = 1;
            this.buttonDelete.Text = "&Delete";
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonRename
            // 
            this.buttonRename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRename.Location = new System.Drawing.Point(72, 224);
            this.buttonRename.Name = "buttonRename";
            this.buttonRename.Size = new System.Drawing.Size(56, 19);
            this.buttonRename.TabIndex = 1;
            this.buttonRename.Text = "&Rename";
            this.buttonRename.Click += new System.EventHandler(this.buttonRename_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(168, 224);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(56, 19);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "&Close";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // TripPlanManageCommonDestinations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.ClientSize = new System.Drawing.Size(232, 254);
            this.ControlBox = false;
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.treeViewDestinations);
            this.Controls.Add(this.buttonRename);
            this.Controls.Add(this.buttonClose);
            this.Name = "TripPlanManageCommonDestinations";
            this.ShowInTaskbar = false;
            this.Text = "Manage \"Smart\" Destinations";
            this.ResumeLayout(false);

        }
        #endregion

        private void buttonClose_Click(object sender, System.EventArgs e) {
            selectedLocationSelection.Hide();
            selectedLocationSelection.Clear();
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

        private void buttonRename_Click(object sender, System.EventArgs e) {
            DestinationNode selected = getSelectedDestination();

            if (selected != null) {
                string name = CommonUI.Dialogs.InputBox.Show("Enter new name", "Rename Destination");

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
