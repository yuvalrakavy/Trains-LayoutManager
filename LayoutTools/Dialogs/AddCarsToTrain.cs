using LayoutManager.Model;
using System;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for AddCarsToTrain.
    /// </summary>
    public partial class AddCarsToTrain : Form {
        private readonly TrainCarsInfo trainCars;

        public AddCarsToTrain(TrainCarsInfo trainCars) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            lengthInput.Initialize();
            this.trainCars = trainCars;

            comboBoxDescription.Text = trainCars.Description;
            numericUpDownCount.Value = trainCars.Count;

            SetLength(trainCars.CarLength);
        }

        private void SetLength(double carLength) {
            bool found = false;

            foreach (Control c in groupBoxLength.Controls) {
                if (c is RadioButton rb) {
                    int underline = rb.Name.IndexOf('_');

                    if (underline >= 0) {
                        double rbLength = double.Parse(rb.Name[(underline + 1)..]);

                        if (Math.Abs(carLength - rbLength) < 0.1) {
                            rb.Checked = true;
                            found = true;
                            break;
                        }
                    }
                }
            }

            if (!found) {
                radioButtonOther.Checked = true;
                lengthInput.NeutralValue = carLength;
            }
        }

        private double GetLength() {
            if (radioButtonOther.Checked)
                return lengthInput.NeutralValue;
            else {
                foreach (Control c in groupBoxLength.Controls) {
                    if (c is RadioButton rb && rb.Checked) {
                        int underline = rb.Name.IndexOf('_');

                        if (underline >= 0)
                            return double.Parse(rb.Name[(underline + 1)..]);
                    }
                }

                throw new ApplicationException("Cannot figure out car length");
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

        private void LengthInput_Enter(object? sender, System.EventArgs e) {
            radioButtonOther.Checked = true;
        }

        private void ButtonOK_Click(object? sender, System.EventArgs e) {
            if (comboBoxDescription.Text.Trim() == "") {
                MessageBox.Show(this, "Missing value", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxDescription.Focus();
                return;
            }

            trainCars.Description = comboBoxDescription.Text;
            trainCars.Count = (int)numericUpDownCount.Value;
            trainCars.CarLength = GetLength();

            DialogResult = DialogResult.OK;
        }
    }
}
