using System.Xml;

namespace LayoutManager.CommonUI.Controls.XmlInfoEditorForms {
    /// <summary>
    /// Summary description for AttributeInfo.
    /// </summary>
    public partial class AttributeInfo : Form {
        public AttributeInfo() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        public AttributeInfo(XmlAttribute a) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            textBoxName.Text = a.Name;
            textBoxValue.Text = a.Value;
        }

        public string AttributeName => textBoxName.Text;

        public string AttributeValue => textBoxValue.Text;

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
            if (textBoxName.Text.Trim() == "") {
                MessageBox.Show(this, "Attribute name must be entered", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxName.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
        }
    }
}
