using System.Xml;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for NumericValue.
    /// </summary>
    public partial class NumericValue : UserControl {
        private XmlElement? element;

        public NumericValue() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call

        }

        public XmlElement? Element {
            get {
                return element;
            }

            set {
                element = value;

                if (element != null) {
                    if (element.HasAttribute("Op") && element.GetAttribute("Op") == "Add")
                        linkMenuOperation.SelectedIndex = 1;
                    else
                        linkMenuOperation.SelectedIndex = 0;

                    if (element.HasAttribute("Value"))
                        textBoxValue.Text = element.GetAttribute("Value");
                }
            }
        }

        public bool ValidateInput() {
            if (textBoxValue.Text.Trim() == "") {
                MessageBox.Show(this, "Missing value", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxValue.Focus();
                return false;
            }

            try {
                int.Parse(textBoxValue.Text);
            }
            catch (FormatException) {
                MessageBox.Show(this, "Invalid number", "Illegal Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxValue.Focus();
                return false;
            }

            return true;
        }

        public bool Commit() {
            if (!ValidateInput())
                return false;

            if (linkMenuOperation.SelectedIndex == 1)
                element?.SetAttribute("Op", "Add");
            else
                element?.SetAttribute("Op", "Set");

            element?.SetAttribute("Value", textBoxValue.Text);

            return true;
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

    }
}
