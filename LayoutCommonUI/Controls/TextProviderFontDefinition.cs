using LayoutManager.Model;
using System.Xml;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for TextProviderFontDefinition.
    /// </summary>
    public partial class TextProviderFontDefinition : UserControl {
        private LayoutXmlInfo? xmlInfo;

        public TextProviderFontDefinition() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitForm call

        }

        public LayoutXmlInfo? OptionalXmlInfo {
            get {
                return xmlInfo;
            }

            set {
                if (value != null) {
                    var fonts = (XmlElement?)LayoutModel.Instance.XmlInfo.DocumentElement.SelectSingleNode("Fonts");

                    layoutInfosComboBoxFonts.InfoType = typeof(LayoutFontInfo);
                    layoutInfosComboBoxFonts.InfoContainer = fonts;
                }

                xmlInfo = value;
            }
        }

        public LayoutXmlInfo XmlInfo {
            get => Ensure.NotNull<LayoutXmlInfo>(OptionalXmlInfo, "XmlInfo");
            set => OptionalXmlInfo = value;
        }

        public string CustomFontElementName { get; set; } = "Font";

        public LayoutFontInfo FontProvider {
            get {
                return radioButtonStandardFont.Checked ? (LayoutFontInfo)layoutInfosComboBoxFonts.SelectedItem : GetCustomFont();
            }

            set {
                if (value.OptionalElement != null && !value.FoundInComponentDocument) {
                    radioButtonStandardFont.Checked = true;
                    layoutInfosComboBoxFonts.SelectedItem = value;
                }
                else {
                    radioButtonCustomFont.Checked = true;
                    labelFontDescription.Text = value.ToString();
                }

                UpdateDependencies();
            }
        }

        private void UpdateDependencies() {
            buttonCustomFontSettings.Enabled = radioButtonCustomFont.Checked;
            layoutInfosComboBoxFonts.Enabled = radioButtonStandardFont.Checked;
        }

        private LayoutFontInfo GetCustomFont() {
            LayoutFontInfo fontProvider = new(XmlInfo.DocumentElement, CustomFontElementName);

            // If font element did not exist, create it
            if (fontProvider.OptionalElement == null)
                fontProvider.Element = LayoutInfo.CreateProviderElement(XmlInfo, CustomFontElementName, null);

            return fontProvider;
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


        private void ButtonCustomFontSettings_Click(object? sender, EventArgs e) {
            LayoutFontInfo fontProvider = GetCustomFont();

            fontDialogCustomSetting.Color = fontProvider.Color;
            fontDialogCustomSetting.ShowColor = true;
            fontDialogCustomSetting.Font = fontProvider.Font;
            fontDialogCustomSetting.ShowDialog();

            fontProvider.Font = fontDialogCustomSetting.Font;
            fontProvider.Color = fontDialogCustomSetting.Color;
            labelFontDescription.Text = fontProvider.ToString();
        }

        private void RadioButtonStandardFont_CheckedChanged(object? sender, EventArgs e) {
            UpdateDependencies();
            labelFontDescription.Text = "";
        }

        private void RadioButtonCustomFont_CheckedChanged(object? sender, EventArgs e) {
            UpdateDependencies();
        }
    }
}
