using System.Xml;

namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for RepeatCondition.
    /// </summary>
    public partial class RepeatCondition : Form {
        private const string A_Count = "Count";
        private readonly XmlElement element;

        public RepeatCondition(XmlElement element) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.element = element;

            int count = (int?)element.AttributeValue(A_Count) ?? -1;

            if (count >= 0) {
                radioButtonRepeatCount.Checked = true;
                numericUpDownCount.Value = count;
            }
            else {
                radioButtonRepeatForever.Checked = true;
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


        private void ButtonOk_Click(object? sender, EventArgs e) {
            element.SetAttributeValue(A_Count, radioButtonRepeatForever.Checked ? -1 : (int)numericUpDownCount.Value);
            DialogResult = DialogResult.OK;
        }

        private void NumericUpDownCount_Enter(object? sender, EventArgs e) {
            radioButtonRepeatCount.Checked = true;
        }
    }
}
