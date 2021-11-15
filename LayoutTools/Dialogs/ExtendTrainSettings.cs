using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for ExtendTrainSettings.
    /// </summary>
    public partial class ExtendTrainSettings : Form {
        private readonly TrainStateInfo train;

        public ExtendTrainSettings(TrainStateInfo train) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.train = train;

            labelTrainInfo.Text = "The train '" + train.DisplayName + "' has " + train.TrackContactTriggerCount +
                " track contact triggers (track magnets).";

            numericUpDownTo.Maximum = train.TrackContactTriggerCount - 1;
            numericUpDownTo.Minimum = 1;

            numericUpDownFrom.Maximum = train.TrackContactTriggerCount - 1;
            numericUpDownFrom.Minimum = 1;

            numericUpDownTo.Value = 1;
            numericUpDownFrom.Value = train.TrackContactTriggerCount - 1;
        }

        public int FromCount => (int)numericUpDownFrom.Value;

        public int ToCount => (int)numericUpDownTo.Value;

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

        private void NumericUpDownFrom_ValueChanged(object? sender, System.EventArgs e) {
            numericUpDownTo.Value = train.TrackContactTriggerCount - numericUpDownFrom.Value;
        }

        private void NumericUpDownTo_ValueChanged(object? sender, System.EventArgs e) {
            numericUpDownFrom.Value = train.TrackContactTriggerCount - numericUpDownTo.Value;
        }

        private void ButtonOK_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.OK;
        }
    }
}
