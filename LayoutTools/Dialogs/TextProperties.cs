using LayoutManager.Model;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for LabelProperties.
    /// </summary>
    public partial class TextProperties : Form, ILayoutComponentPropertiesDialog {
        public TextProperties(ModelComponent component) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            XmlInfo = new LayoutXmlInfo(component);

            LayoutTextInfo textProvider = new(XmlInfo.DocumentElement, "Text") {
                Component = component
            };
            textBoxText.Text = textProvider.Text;

            textProviderFontDefinition.XmlInfo = XmlInfo;
            textProviderFontDefinition.FontProvider = textProvider.FontProvider;

            textProviderPositionDefinition.XmlInfo = XmlInfo;
            textProviderPositionDefinition.PositionProvider = textProvider.PositionProvider;
        }

        public LayoutXmlInfo XmlInfo { get; }

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

        private void ButtonOK_Click(object? sender, System.EventArgs e) {
            if (textBoxText.Text.Trim() == "") {
                MessageBox.Show(this, "Text should not be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxText.Focus();
                return;
            }

            LayoutTextInfo textProvider = new(XmlInfo.DocumentElement, "Text");
            if (textProvider.OptionalElement == null)
                textProvider.Element = LayoutInfo.CreateProviderElement(XmlInfo.DocumentElement, "Text", null);

            textProvider.Text = textBoxText.Text;
            textProvider.FontElementPath = textProviderFontDefinition.FontProvider.ElementPath;
            textProvider.PositionElementPath = textProviderPositionDefinition.PositionProvider.ElementPath;
            textProvider.Visible = true;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
