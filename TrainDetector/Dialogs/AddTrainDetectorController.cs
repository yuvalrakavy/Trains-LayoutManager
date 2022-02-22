using System;
using System.Windows.Forms;

#nullable enable
namespace TrainDetector.Dialogs {
    public partial class AddTrainDetectorController : Form {
        readonly Func<string, string?>? validateFunction;

        public AddTrainDetectorController(Func<string, string?>? validateFunction) {
            InitializeComponent();
            this.validateFunction = validateFunction;
        }

        public int SensorsCount {
            get => (int)numericUpDownSensorsCount.Value;
            set => numericUpDownSensorsCount.Value = value;
        }

        public string ControllerName {
            get => textBoxName.Text;
            set => textBoxName.Text = value;
        }

        private void ButtonOK_Click(object? sender, EventArgs e) {
            var validationError = validateFunction != null ? validateFunction(ControllerName) : null;

            if (validationError != null) {
                MessageBox.Show(validationError, "Invalid value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxName.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
        }
    }
}
