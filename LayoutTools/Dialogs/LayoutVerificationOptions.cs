using LayoutManager.Model;
using System;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    public partial class LayoutVerificationOptions : Form {
        public LayoutVerificationOptions() {
            InitializeComponent();

            checkBoxIgnoreFeedbackComponents.Checked = LayoutModel.StateManager.VerificationOptions.IgnoreNotConnectedFeedbacks;
            checkBoxIgnorePowerComponents.Checked = LayoutModel.StateManager.VerificationOptions.IgnoreNotConnectedPowerComponents;
        }

        private void ButtonOK_Click(object? sender, EventArgs e) {
            LayoutModel.StateManager.VerificationOptions.IgnoreNotConnectedFeedbacks = checkBoxIgnoreFeedbackComponents.Checked;
            LayoutModel.StateManager.VerificationOptions.IgnoreNotConnectedPowerComponents = checkBoxIgnorePowerComponents.Checked;

            LayoutModel.StateManager.Save();

            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
    }
}
