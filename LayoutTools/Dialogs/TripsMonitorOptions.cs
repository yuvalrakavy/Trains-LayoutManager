using System;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TripsMonitorOptions.
    /// </summary>
    public partial class TripsMonitorOptions : Form {

        public TripsMonitorOptions(int autoClearTimeout) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            if (autoClearTimeout < 0) {
                checkBoxEnableAutoClear.Checked = false;
                numericUpDownAutoClearTimeout.Value = 15;
            }
            else {
                checkBoxEnableAutoClear.Checked = true;
                numericUpDownAutoClearTimeout.Value = autoClearTimeout;
            }

            UpdateButtons(null, EventArgs.Empty);
        }

        private void UpdateButtons(object? sender, EventArgs e) {
            if (checkBoxEnableAutoClear.Checked) {
                labelAutoClear1.Enabled = true;
                labelAutoClear2.Enabled = true;
                numericUpDownAutoClearTimeout.Enabled = true;
            }
            else {
                labelAutoClear1.Enabled = false;
                labelAutoClear2.Enabled = false;
                numericUpDownAutoClearTimeout.Enabled = false;
            }
        }

        public int AutoClearTimeout => checkBoxEnableAutoClear.Checked ? (int)numericUpDownAutoClearTimeout.Value : -1;

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

        private void ButtonOk_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
