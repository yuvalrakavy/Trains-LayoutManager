using LayoutManager.CommonUI;
using LayoutManager.Model;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for ManualDispatchRegions.
    /// </summary>
    public partial class ManualDispatchRegions : Form {

        private readonly LayoutSelection selectedRegionSelection;
        private readonly LayoutSelectionLook selectedRegionLook = new(Color.DarkCyan);
        private ManualDispatchRegionInfo? editedRegion = null;
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

            UpdateButtons(null, EventArgs.Empty);
        }

        private void UpdateButtons(object? sender, EventArgs e) {
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

        private void Edit(ManualDispatchRegionInfo manualDispatchRegion, bool editingNewRegion) {
            Dialogs.ManualDispatchRegion d = new(manualDispatchRegion);

            editedRegion = manualDispatchRegion;
            this.editingNewRegion = editingNewRegion;

            selectedRegionSelection.Hide();

            new SemiModalDialog(this, d, new SemiModalDialogClosedHandler(OnEditRegionClosed), null).ShowDialog();
        }

        private void OnEditRegionClosed(Form dialog, object? info) {
            Dialogs.ManualDispatchRegion d = (Dialogs.ManualDispatchRegion)dialog;

            selectedRegionSelection.Display(selectedRegionLook);

            if (d.DialogResult == DialogResult.Cancel) {
                if (editedRegion != null && this.editingNewRegion)
                    LayoutModel.StateManager.ManualDispatchRegions.Remove(editedRegion);
            }
            else if (d.DialogResult == DialogResult.OK) {
                if (editedRegion != null) {
                    if (this.editingNewRegion)
                        listBoxRegions.Items.Add(editedRegion);
                    else {
                        int i = listBoxRegions.Items.IndexOf(editedRegion);

                        listBoxRegions.Invalidate(listBoxRegions.GetItemRectangle(i));
                    }
                }
            }

            UpdateButtons(null, EventArgs.Empty);
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
            selectedRegionSelection.Hide();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonNewRegion_Click(object? sender, System.EventArgs e) {
            ManualDispatchRegionInfo newRegion = LayoutModel.StateManager.ManualDispatchRegions.Create();

            Edit(newRegion, true);
        }

        private void ButtonEditRegion_Click(object? sender, System.EventArgs e) {
            ManualDispatchRegionInfo editedRegion = (ManualDispatchRegionInfo)listBoxRegions.SelectedItem;

            if (editedRegion != null)
                Edit(editedRegion, false);
        }

        private void ButtonDeleteRegion_Click(object? sender, System.EventArgs e) {
            ManualDispatchRegionInfo selectedRegion = (ManualDispatchRegionInfo)listBoxRegions.SelectedItem;

            if (selectedRegion != null) {
                listBoxRegions.Items.Remove(selectedRegion);
                LayoutModel.StateManager.ManualDispatchRegions.Remove(selectedRegion);
            }

            UpdateButtons(null, EventArgs.Empty);
        }

        private void ButtonToggleRegionActivation_Click(object? sender, System.EventArgs e) {
            var selectedRegion = (ManualDispatchRegionInfo?)listBoxRegions.SelectedItem;

            try {
                if (selectedRegion != null) {
                    if (selectedRegion.Active)
                        selectedRegion.Active = false;
                    else
                        selectedRegion.Active = true;

                    int i = listBoxRegions.Items.IndexOf(selectedRegion);

                    listBoxRegions.Items.RemoveAt(i);
                    listBoxRegions.Items.Insert(i, selectedRegion);
                    listBoxRegions.SelectedIndex = i;
                }
            }
            catch (LayoutException ex) {
                ex.Report();
            }

            UpdateButtons(null, EventArgs.Empty);
        }

        private void ManualDispatchRegions_Closing(object? sender, CancelEventArgs e) {
            if (Owner != null)
                Owner.Activate();
        }
    }
}
