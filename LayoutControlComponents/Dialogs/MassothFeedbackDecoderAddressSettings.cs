using System;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.ControlComponents.Dialogs {
    public partial class MassothFeedbackDecoderAddressSettings : Form {
        private readonly IMassothFeedbackDecoderSetAddress action;
        private readonly MassothFeedbackModule feedbackModule;

        public MassothFeedbackDecoderAddressSettings(IMassothFeedbackDecoderSetAddress action, ControlModule module) {
            var feedbackModule = new MassothFeedbackModule(module);

            InitializeComponent();

            this.action = action;
            this.feedbackModule = feedbackModule;

            if (feedbackModule.HasDiMAX_BusConnectionMethod) {
                radioButtonMaster.Checked = feedbackModule.DiMAX_BusConnectionMethod == MassothFeedbackDecoderBusConnectionMethod.Master;
                radioButtonSlave.Checked = feedbackModule.DiMAX_BusConnectionMethod == MassothFeedbackDecoderBusConnectionMethod.Slave;
            }
            else {
                radioButtonMaster.Checked = false;
                radioButtonSlave.Checked = true;
            }

            if (feedbackModule.HasDiMAX_BusId)
                textBoxBusId.Text = feedbackModule.DiMAX_BusId.ToString();
            else
                textBoxBusId.Text = MassothFeedbackModule.AllocateDiMAX_BusID(feedbackModule.Bus.BusProvider).ToString();

            UpdateForm();
        }

        private void UpdateForm() {
            panelBusId.Enabled = radioButtonMaster.Checked;
        }

        private void radioButtonMaster_CheckedChanged(object sender, EventArgs e) {
            UpdateForm();
        }

        private void radioButtonSlave_CheckedChanged(object sender, EventArgs e) {
            UpdateForm();
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            if (radioButtonMaster.Checked) {
                if (!int.TryParse(textBoxBusId.Text, out int busId) || busId < 11 || busId > 20) {
                    MessageBox.Show(this, "Invalid Bus ID", "Bus ID should be a number between 11 and 20", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxBusId.Focus();
                    return;
                }

                var masterWithBusId = MassothFeedbackModule.GetMasterUsingBusId(feedbackModule.Bus.BusProvider, busId);

                if (masterWithBusId != null && masterWithBusId.Id != feedbackModule.Id) {
                    MessageBox.Show(this, "Invalid Bus ID", "This bus ID (" + busId + ") is used by another feedback module.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxBusId.Focus();
                    return;
                }

                action.BusConnectionMethod = MassothFeedbackDecoderBusConnectionMethod.Master;
                action.BusId = busId;
            }
            else
                action.BusConnectionMethod = MassothFeedbackDecoderBusConnectionMethod.Slave;

            DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
