using System.Xml;

namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for GenerateEventOption.
    /// </summary>
    public partial class GenerateEventOption : Form {
        private readonly XmlElement optionsElement;
        private readonly XmlElement element;

        public GenerateEventOption(XmlElement element, XmlElement optionsElement) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.element = element;
            this.optionsElement = optionsElement;

            if (element.HasAttribute("Name"))
                textBoxName.Text = element.GetAttribute("Name");

            operand.Element = element;
            operand.Suffix = "Option";
            operand.DefaultAccess = "Value";
            operand.AllowedTypes = new Type[] { typeof(string), typeof(int), typeof(bool), typeof(double), typeof(Enum) };
            operand.Initialize();
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
            if (textBoxName.Text.Trim() == "") {
                MessageBox.Show(this, "You have to provide an option name", "Missing option name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxName.Focus();
                return;
            }

            if (optionsElement != null) {
                foreach (XmlElement optionElement in optionsElement) {
                    if (optionElement != element && optionElement.GetAttribute("Name") == textBoxName.Text) {
                        MessageBox.Show(this, "This name is already used by another option", "Duplicate option name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBoxName.Focus();
                        return;
                    }
                }
            }

            if (!operand.ValidateInput())
                return;

            element.SetAttribute("Name", textBoxName.Text);
            operand.Commit();

            DialogResult = DialogResult.OK;
        }
    }
}
