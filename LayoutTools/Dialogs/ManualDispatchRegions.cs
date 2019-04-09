using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.CommonUI;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for ManualDispatchRegions.
    /// </summary>
    public class ManualDispatchRegions : Form {
        private ListBox listBoxRegions;
        private Button buttonNewRegion;
        private Button buttonEditRegion;
        private Button buttonDeleteRegion;
        private Button buttonToggleRegionActivation;
        private Button buttonClose;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private void endOfDesignerVariables() { }

        private readonly LayoutSelection selectedRegionSelection;
        private readonly LayoutSelectionLook selectedRegionLook = new LayoutSelectionLook(Color.DarkCyan);
        private ManualDispatchRegionInfo editedRegion = null;
        private bool editingNewRegion = false;

        public ManualDispatchRegions() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            foreach (ManualDispatchRegionInfo manualDispatchRegion in LayoutModel.StateManager.ManualDispatchRegions)
                listBoxRegions.Items.Add(manualDispatchRegion);

            this.Owner = LayoutController.ActiveFrameWindow as Form;

            selectedRegionSelection = new LayoutSelection();

            selectedRegionSelection.Display(selectedRegionLook);

            updateButtons(null, null);
        }

        private void updateButtons(object sender, EventArgs e) {
            ManualDispatchRegionInfo manualDispatchRegion = (ManualDispatchRegionInfo)listBoxRegions.SelectedItem;

            selectedRegionSelection.Clear();

            if (manualDispatchRegion == null) {
                buttonEditRegion.Enabled = false;
                buttonDeleteRegion.Enabled = false;
                buttonToggleRegionActivation.Enabled = false;
            }
            else {
                selectedRegionSelection.Add(manualDispatchRegion.Selection);

                buttonToggleRegionActivation.Enabled = true;

                if (manualDispatchRegion.Active) {
                    buttonToggleRegionActivation.Text = "Deactivate";
                    buttonEditRegion.Enabled = false;
                    buttonDeleteRegion.Enabled = false;
                }
                else {
                    buttonToggleRegionActivation.Text = "Activate";
                    buttonEditRegion.Enabled = true;
                    buttonDeleteRegion.Enabled = true;
                }
            }
        }

        private void edit(ManualDispatchRegionInfo manualDispatchRegion, bool editingNewRegion) {
            Dialogs.ManualDispatchRegion d = new Dialogs.ManualDispatchRegion(manualDispatchRegion);

            editedRegion = manualDispatchRegion;
            this.editingNewRegion = editingNewRegion;

            selectedRegionSelection.Hide();

            new SemiModalDialog(this, d, new SemiModalDialogClosedHandler(onEditRegionClosed), null).ShowDialog();
        }

        private void onEditRegionClosed(Form dialog, object info) {
            Dialogs.ManualDispatchRegion d = (Dialogs.ManualDispatchRegion)dialog;

            selectedRegionSelection.Display(selectedRegionLook);

            if (d.DialogResult == DialogResult.Cancel) {
                if (this.editingNewRegion)
                    LayoutModel.StateManager.ManualDispatchRegions.Remove(editedRegion);
            }
            else if (d.DialogResult == DialogResult.OK) {
                if (this.editingNewRegion)
                    listBoxRegions.Items.Add(editedRegion);
                else {
                    int i = listBoxRegions.Items.IndexOf(editedRegion);

                    listBoxRegions.Invalidate(listBoxRegions.GetItemRectangle(i));
                }
            }

            updateButtons(null, null);
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
            this.listBoxRegions = new ListBox();
            this.buttonNewRegion = new Button();
            this.buttonEditRegion = new Button();
            this.buttonDeleteRegion = new Button();
            this.buttonToggleRegionActivation = new Button();
            this.buttonClose = new Button();
            this.SuspendLayout();
            // 
            // listBoxRegions
            // 
            this.listBoxRegions.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.listBoxRegions.DisplayMember = "NameAndStatus";
            this.listBoxRegions.Location = new System.Drawing.Point(6, 8);
            this.listBoxRegions.Name = "listBoxRegions";
            this.listBoxRegions.Size = new System.Drawing.Size(234, 199);
            this.listBoxRegions.TabIndex = 0;
            this.listBoxRegions.SelectedIndexChanged += new System.EventHandler(this.updateButtons);
            // 
            // buttonNewRegion
            // 
            this.buttonNewRegion.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonNewRegion.Location = new System.Drawing.Point(6, 216);
            this.buttonNewRegion.Name = "buttonNewRegion";
            this.buttonNewRegion.Size = new System.Drawing.Size(48, 18);
            this.buttonNewRegion.TabIndex = 1;
            this.buttonNewRegion.Text = "&New";
            this.buttonNewRegion.Click += new System.EventHandler(this.buttonNewRegion_Click);
            // 
            // buttonEditRegion
            // 
            this.buttonEditRegion.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonEditRegion.Location = new System.Drawing.Point(58, 216);
            this.buttonEditRegion.Name = "buttonEditRegion";
            this.buttonEditRegion.Size = new System.Drawing.Size(48, 18);
            this.buttonEditRegion.TabIndex = 1;
            this.buttonEditRegion.Text = "&Edit";
            this.buttonEditRegion.Click += new System.EventHandler(this.buttonEditRegion_Click);
            // 
            // buttonDeleteRegion
            // 
            this.buttonDeleteRegion.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonDeleteRegion.Location = new System.Drawing.Point(111, 216);
            this.buttonDeleteRegion.Name = "buttonDeleteRegion";
            this.buttonDeleteRegion.Size = new System.Drawing.Size(48, 18);
            this.buttonDeleteRegion.TabIndex = 1;
            this.buttonDeleteRegion.Text = "&Delete";
            this.buttonDeleteRegion.Click += new System.EventHandler(this.buttonDeleteRegion_Click);
            // 
            // buttonToggleRegionActivation
            // 
            this.buttonToggleRegionActivation.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonToggleRegionActivation.Location = new System.Drawing.Point(164, 216);
            this.buttonToggleRegionActivation.Name = "buttonToggleRegionActivation";
            this.buttonToggleRegionActivation.Size = new System.Drawing.Size(76, 18);
            this.buttonToggleRegionActivation.TabIndex = 1;
            this.buttonToggleRegionActivation.Text = "&Activate";
            this.buttonToggleRegionActivation.Click += new System.EventHandler(this.buttonToggleRegionActivation_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonClose.Location = new System.Drawing.Point(178, 245);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(64, 23);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "Close";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // ManualDispatchRegions
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.ClientSize = new System.Drawing.Size(248, 270);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonClose,
                                                                          this.buttonNewRegion,
                                                                          this.listBoxRegions,
                                                                          this.buttonEditRegion,
                                                                          this.buttonDeleteRegion,
                                                                          this.buttonToggleRegionActivation});
            this.Name = "ManualDispatchRegions";
            this.ShowInTaskbar = false;
            this.Text = "Manual Dispatch Regions";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.ManualDispatchRegions_Closing);
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonClose_Click(object sender, System.EventArgs e) {
            selectedRegionSelection.Hide();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonNewRegion_Click(object sender, System.EventArgs e) {
            ManualDispatchRegionInfo newRegion = LayoutModel.StateManager.ManualDispatchRegions.Create();

            edit(newRegion, true);
        }

        private void buttonEditRegion_Click(object sender, System.EventArgs e) {
            ManualDispatchRegionInfo editedRegion = (ManualDispatchRegionInfo)listBoxRegions.SelectedItem;

            if (editedRegion != null)
                edit(editedRegion, false);
        }

        private void buttonDeleteRegion_Click(object sender, System.EventArgs e) {
            ManualDispatchRegionInfo selectedRegion = (ManualDispatchRegionInfo)listBoxRegions.SelectedItem;

            if (selectedRegion != null) {
                listBoxRegions.Items.Remove(selectedRegion);
                LayoutModel.StateManager.ManualDispatchRegions.Remove(selectedRegion);
            }

            updateButtons(null, null);
        }

        private void buttonToggleRegionActivation_Click(object sender, System.EventArgs e) {
            ManualDispatchRegionInfo selectedRegion = (ManualDispatchRegionInfo)listBoxRegions.SelectedItem;

            try {
                if (selectedRegion != null) {
                    if (selectedRegion.Active)
                        selectedRegion.Active = false;
                    else
                        selectedRegion.Active = true;
                }

                int i = listBoxRegions.Items.IndexOf(selectedRegion);

                listBoxRegions.Items.RemoveAt(i);
                listBoxRegions.Items.Insert(i, selectedRegion);
                listBoxRegions.SelectedIndex = i;
            }
            catch (LayoutException ex) {
                ex.Report();
            }

            updateButtons(null, null);
        }

        private void ManualDispatchRegions_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (Owner != null)
                Owner.Activate();
        }
    }
}
