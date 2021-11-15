namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for IfTimeNumericNode.
    /// </summary>
    public partial class IfTimeDayOfWeekNode : Form {
        private readonly IIfTimeNode node;

        public IfTimeDayOfWeekNode(string title, IIfTimeNode node) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.node = node;

            Text = title;

            if (node.IsRange) {
                radioButtonRange.Checked = true;
                comboBoxFrom.SelectedIndex = node.From;
                comboBoxTo.SelectedIndex = node.To;
            }
            else {
                radioButtonValue.Checked = true;
                comboBoxValue.SelectedIndex = node.Value;
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

        private void ButtonOK_Click(object? sender, EventArgs e) {
            if (radioButtonRange.Checked) {
                if (comboBoxFrom.SelectedIndex > comboBoxTo.SelectedIndex) {
                    MessageBox.Show(this, "Invalid range, \"from\" is larger than \"to\"", "Invalid Range", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    comboBoxFrom.Focus();
                    return;
                }

                node.From = comboBoxFrom.SelectedIndex;
                node.To = comboBoxTo.SelectedIndex;
                comboBoxFrom.Focus();
            }
            else
                node.Value = comboBoxValue.SelectedIndex;

            DialogResult = DialogResult.OK;
            Close();
        }


        private void ButtonCancel_Click(object? sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ComboBoxValue_Changed(object? sender, EventArgs e) {
            radioButtonValue.Checked = true;
        }

        private void ComboBoxFromOrTo_Changed(object? sender, EventArgs e) {
            radioButtonRange.Checked = true;
        }
    }
}
