using LayoutManager.Model;
using System;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for MotionRamps.
    /// </summary>
    public partial class MotionRamps : Form {

        private readonly MotionRampCollection ramps;

        public MotionRamps(MotionRampCollection ramps) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.ramps = ramps;

            foreach (MotionRampInfo ramp in ramps)
                listViewRamps.Items.Add(new RampItem(ramp));

            UpdateButtons(null, EventArgs.Empty);
        }

        private void UpdateButtons(object? sender, EventArgs e) {
            if (listViewRamps.SelectedItems.Count == 0) {
                buttonEdit.Enabled = false;
                buttonRemove.Enabled = false;
            }
            else {
                buttonEdit.Enabled = true;
                buttonRemove.Enabled = true;
            }

            if (listViewRamps.SelectedItems.Count > 0) {
                int selectedItemIndex = listViewRamps.SelectedIndices[0];

                buttonMoveUp.Enabled = selectedItemIndex > 0;
                buttonMoveDown.Enabled = selectedItemIndex < listViewRamps.Items.Count - 1;
            }
            else {
                buttonMoveUp.Enabled = false;
                buttonMoveDown.Enabled = false;
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

        private void ButtonAdd_Click(object? sender, System.EventArgs e) {
            MotionRampInfo ramp = new(MotionRampType.TimeFixed, 1500);
            Dialogs.MotionRampDefinition motionRampDefinition = new(ramp);

            if (motionRampDefinition.ShowDialog(this) == DialogResult.OK) {
                ramps.Add(ramp);
                listViewRamps.Items.Add(new RampItem(ramp));
                UpdateButtons(null, EventArgs.Empty);
            }
        }

        private RampItem? GetSelected() {
            return listViewRamps.SelectedItems.Count == 0 ? null : (RampItem)listViewRamps.SelectedItems[0];
        }

        private void ButtonEdit_Click(object? sender, System.EventArgs e) {
            var selected = GetSelected();

            if (selected != null) {
                Dialogs.MotionRampDefinition motionRampDefinition = new(selected.Ramp);

                if (motionRampDefinition.ShowDialog(this) == DialogResult.OK) {
                    selected.Update();
                    UpdateButtons(null, EventArgs.Empty);
                }
            }
        }

        private void ButtonRemove_Click(object? sender, System.EventArgs e) {
            var selected = GetSelected();

            if (selected != null) {
                ramps.Remove(selected.Ramp);
                listViewRamps.Items.Remove(selected);
                UpdateButtons(null, EventArgs.Empty);
            }
        }

        private void ButtonClose_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonMoveUp_Click(object? sender, System.EventArgs e) {
            if (listViewRamps.SelectedItems.Count > 0) {
                int selectedIndex = listViewRamps.SelectedIndices[0];

                if (selectedIndex > 0) {
                    RampItem selected = (RampItem)listViewRamps.SelectedItems[0];
                    MotionRampInfo selectedRamp = selected.Ramp;
                    var previousElement = selectedRamp.Element.PreviousSibling;

                    selectedRamp.Element.ParentNode?.RemoveChild(selectedRamp.Element);
                    previousElement?.ParentNode?.InsertBefore(selectedRamp.Element, previousElement);

                    listViewRamps.Items.Remove(selected);
                    listViewRamps.Items.Insert(selectedIndex - 1, selected);
                }
            }
        }

        private void ButtonMoveDown_Click(object? sender, System.EventArgs e) {
            if (listViewRamps.SelectedItems.Count > 0) {
                int selectedIndex = listViewRamps.SelectedIndices[0];

                if (selectedIndex < listViewRamps.Items.Count - 1) {
                    RampItem selected = (RampItem)listViewRamps.SelectedItems[0];
                    MotionRampInfo selectedRamp = selected.Ramp;
                    var nextElement = selectedRamp.Element.NextSibling;

                    selectedRamp.Element.ParentNode?.RemoveChild(selectedRamp.Element);
                    nextElement?.ParentNode?.InsertAfter(selectedRamp.Element, nextElement);

                    listViewRamps.Items.Remove(selected);
                    listViewRamps.Items.Insert(selectedIndex + 1, selected);
                }
            }
        }

        private class RampItem : ListViewItem {
            public RampItem(MotionRampInfo ramp) {
                this.Ramp = ramp;

                SubItems.Add("");
                Update();
            }

            public void Update() {
                Text = Ramp.Description;

                if (Ramp.UseInTrainControllerDialog)
                    SubItems[1].Text = "Train Controller";
                else
                    SubItems[1].Text = "";
            }

            public MotionRampInfo Ramp { get; }
        }
    }
}
