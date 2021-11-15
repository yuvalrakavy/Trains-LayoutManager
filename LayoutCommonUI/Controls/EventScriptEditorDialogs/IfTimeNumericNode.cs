namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for IfTimeNumericNode.
    /// </summary>
    public partial class IfTimeNumericNode : Form {
        private readonly IIfTimeNode node;

        public IfTimeNumericNode(string title, IIfTimeNode node, int minValue, int maxValue) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.node = node;

            Text = title;

            numericUpDownFrom.Minimum = minValue;
            numericUpDownFrom.Maximum = maxValue;
            numericUpDownTo.Minimum = minValue;
            numericUpDownTo.Maximum = maxValue;
            numericUpDownValue.Minimum = minValue;
            numericUpDownValue.Maximum = maxValue;

            if (node.IsRange) {
                radioButtonRange.Checked = true;
                numericUpDownFrom.Value = node.From;
                numericUpDownTo.Value = node.To;
                numericUpDownFrom.Focus();
            }
            else {
                radioButtonValue.Checked = true;
                numericUpDownValue.Value = node.Value;
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

        private void NumericUpDownValue_ValueChanged(object? sender, EventArgs e) {
            radioButtonValue.Checked = true;
        }

        private void NumericUpDownFromOrTo_ValueChanged(object? sender, EventArgs e) {
            radioButtonRange.Checked = true;
        }

        private void ButtonOK_Click(object? sender, EventArgs e) {
            if (radioButtonRange.Checked) {
                if (numericUpDownFrom.Value > numericUpDownTo.Value) {
                    MessageBox.Show(this, "Invalid range, \"from\" is larger than \"to\"", "Invalid Range", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    numericUpDownFrom.Focus();
                    return;
                }

                node.From = (int)numericUpDownFrom.Value;
                node.To = (int)numericUpDownTo.Value;
            }
            else
                node.Value = (int)numericUpDownValue.Value;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object? sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void NumericUpDownValue_KeyDown(object? sender, KeyEventArgs e) {
            radioButtonValue.Checked = true;
        }

        private void NumericUpDownFromOrTo_KeyDown(object? sender, KeyEventArgs e) {
            radioButtonRange.Checked = true;
        }
    }
}
