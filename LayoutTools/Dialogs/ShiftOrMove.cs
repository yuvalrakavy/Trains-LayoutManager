using System;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    public partial class ShiftOrMove : Form {
        public ShiftOrMove() {
            InitializeComponent();
        }

        public int DeltaX {
            get {
                if (radioButtonLeft.Checked)
                    return (int)-numericUpDownComponents.Value;
                else return radioButtonRight.Checked ? (int)numericUpDownComponents.Value : 0;
            }
        }

        public int DeltaY {
            get {
                if (radioButtonUp.Checked)
                    return (int)-numericUpDownComponents.Value;
                else return radioButtonDown.Checked ? (int)numericUpDownComponents.Value : 0;
            }
        }

        public bool FillGaps => checkBoxFillGaps.Checked;

        private void ButtonOK_Click(object? sender, EventArgs e) {
            if (!radioButtonDown.Checked && !radioButtonLeft.Checked && !radioButtonRight.Checked && !radioButtonUp.Checked) {
                MessageBox.Show("Please pick a direction", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                radioButtonUp.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}