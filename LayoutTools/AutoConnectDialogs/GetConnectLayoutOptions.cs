using LayoutManager.Model;
using System;
using System.Linq;
using System.Windows.Forms;

namespace LayoutManager.Tools.AutoConnectDialogs {
    /// <summary>
    /// Summary description for GetConnectLayoutOptions.
    /// </summary>
    public partial class GetConnectLayoutOptions : Form {
        public GetConnectLayoutOptions() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            radioButtonConnectNotConnected.Checked = true;
            checkBoxSetUserActionRequired.Checked = true;
            radioButtonConnectAllLayout.Checked = true;
            checkBoxSetProgrammingRequired.Checked = true;
            checkBoxSetUserActionRequired.Checked = true;

            UpdateButtons();
        }

        public bool ConnectAllLayout => radioButtonConnectAllLayout.Checked;

        public bool SetUserActionRequired => checkBoxSetUserActionRequired.Checked;

        public bool SetProgrammingRequired => checkBoxSetProgrammingRequired.Checked;

        public LayoutPhase Phase {
            get {
                if (radioButtonScopeAll.Checked)
                    return LayoutPhase.All;
                if (radioButtonScopNotPlanned.Checked)
                    return LayoutPhase.NotPlanned;
                else if (radioButtonScopeOperational.Checked)
                    return LayoutPhase.Operational;
                else
                    throw new ApplicationException("Cannot figure connect scope");
            }
        }

        private void UpdateButtons() {
            if (radioButtonScopeOperational.Checked) {
                var anyOperationalSpots = LayoutModel.Components<IModelComponentConnectToControl>(Phase).Select(c => c.Spot).Where(s => s.Phase == LayoutPhase.Operational).Any();

                groupBoxWarning.Visible = radioButtonConnectAllLayout.Checked && anyOperationalSpots;
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

        private void RadioButtonConnectNotConnected_CheckedChanged(object? sender, System.EventArgs e) {
            checkBoxSetUserActionRequired.Checked = true;
            UpdateButtons();
        }

        private void RadioButtonConnectAllLayout_CheckedChanged(object? sender, System.EventArgs e) {
            checkBoxSetUserActionRequired.Checked = false;
            UpdateButtons();
        }

        private void ButtonOK_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.OK;
        }

        private void RadioButtonScope_CheckedChanged(object? sender, EventArgs e) {
            UpdateButtons();
        }
    }
}
