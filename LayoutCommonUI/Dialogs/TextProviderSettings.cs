
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Dialogs {
    /// <summary>
    /// Summary description for TextProviderSettings.
    /// </summary>
    public partial class TextProviderSettings : Form {
        private readonly LayoutTextInfo textProvider;

        public TextProviderSettings(LayoutXmlInfo xmlInfo, LayoutTextInfo textProvider) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.textProvider = textProvider;

            textProviderFontDefinition1.OptionalXmlInfo = xmlInfo;
            textProviderFontDefinition1.FontProvider = textProvider.FontProvider;

            TextProviderPositionDefinition1.XmlInfo = xmlInfo;
            TextProviderPositionDefinition1.PositionProvider = textProvider.PositionProvider;

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
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
            textProvider.FontElementPath = textProviderFontDefinition1.FontProvider.ElementPath;
            textProvider.PositionElementPath = TextProviderPositionDefinition1.PositionProvider.ElementPath;
            this.Close();
        }
    }
}
