using System.Xml;

namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for WaitCondition.
    /// </summary>
    public partial class WaitCondition : Form {
        private const string A_Minutes = "Minutes";
        private const string A_Seconds = "Seconds";
        private const string A_RandomSeconds = "RandomSeconds";
        private const string A_IsError = "IsError";
        private readonly XmlElement conditionElement;

        public WaitCondition(XmlElement conditionElement) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.conditionElement = conditionElement;

            numericUpDownMinutes.Value = (decimal?)conditionElement.AttributeValue(A_Minutes) ?? 0;
            numericUpDownSeconds.Value = (decimal?)conditionElement.AttributeValue(A_Seconds) ?? 0;

            if (conditionElement.HasAttribute(A_RandomSeconds)) {
                checkBoxRadomWait.Checked = true;
                numericUpDownRandomSeconds.Value = (decimal?)conditionElement.AttributeValue(A_RandomSeconds) ?? 0;
            }
            else
                checkBoxRadomWait.Checked = false;

            checkBoxErrorState.Checked = (bool?)conditionElement.AttributeValue(A_IsError) ?? false;
            UpdateButtons(null, EventArgs.Empty);
        }

        private void UpdateButtons(object? sender, EventArgs e) {
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


        private void NumericUpDownRandomSeconds_ValueChanged(object? sender, EventArgs e) {
            checkBoxRadomWait.Checked = true;
        }

        private void ButtonOk_Click(object? sender, EventArgs e) {
            conditionElement.SetAttributeValue(A_Minutes, (int)numericUpDownMinutes.Value, removeIf: 0);
            conditionElement.SetAttributeValue(A_Seconds, (int)numericUpDownRandomSeconds.Value, removeIf: 0);
            conditionElement.SetAttributeValue(A_RandomSeconds, (int)numericUpDownRandomSeconds.Value);

            if (checkBoxRadomWait.Checked)
                conditionElement.SetAttributeValue(A_RandomSeconds, (int)numericUpDownRandomSeconds.Value);
            else
                conditionElement.RemoveAttribute(A_RandomSeconds);

            conditionElement.SetAttributeValue(A_IsError, true, removeIf: !checkBoxErrorState.Checked);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
