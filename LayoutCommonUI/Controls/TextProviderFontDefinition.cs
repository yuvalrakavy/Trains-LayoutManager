using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for TextProviderFontDefinition.
    /// </summary>
    public class TextProviderFontDefinition : System.Windows.Forms.UserControl {
        private GroupBox groupBoxFont;
        private RadioButton radioButtonCustomFont;
        private RadioButton radioButtonStandardFont;
        private Label labelFontDescription;
        private Button buttonCustomFontSettings;
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
        private LayoutManager.CommonUI.Controls.LayoutInfosComboBox layoutInfosComboBoxFonts;
        private System.Windows.Forms.FontDialog fontDialogCustomSetting;

        string customFontElementName = "Font";
        LayoutXmlInfo xmlInfo;

        public TextProviderFontDefinition() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitForm call

        }

        public LayoutXmlInfo XmlInfo {
            get {
                return xmlInfo;
            }

            set {
                if (value != null) {
                    XmlElement fonts = (XmlElement)LayoutModel.Instance.XmlInfo.DocumentElement.SelectSingleNode("Fonts");

                    layoutInfosComboBoxFonts.InfoType = typeof(LayoutFontInfo);
                    layoutInfosComboBoxFonts.InfoContainer = fonts;
                }

                xmlInfo = value;
            }
        }

        public string CustomFontElementName {
            get {
                return customFontElementName;
            }

            set {
                customFontElementName = value;
            }
        }

        public LayoutFontInfo FontProvider {
            get {
                if (radioButtonStandardFont.Checked)
                    return (LayoutFontInfo)layoutInfosComboBoxFonts.SelectedItem;
                else
                    return getCustomFont();
            }

            set {
                if (value.Element != null && !value.FoundInComponentDocument) {
                    radioButtonStandardFont.Checked = true;
                    layoutInfosComboBoxFonts.SelectedItem = value;
                }
                else {
                    radioButtonCustomFont.Checked = true;
                    labelFontDescription.Text = value.ToString();
                }

                updateDependencies();
            }
        }

        void updateDependencies() {
            buttonCustomFontSettings.Enabled = radioButtonCustomFont.Checked;
            layoutInfosComboBoxFonts.Enabled = radioButtonStandardFont.Checked;
        }

        LayoutFontInfo getCustomFont() {
            LayoutFontInfo fontProvider = new LayoutFontInfo(xmlInfo.DocumentElement, CustomFontElementName);

            // If font element did not exist, create it
            if (fontProvider.Element == null)
                fontProvider.Element = LayoutInfo.CreateProviderElement(xmlInfo, CustomFontElementName, null);

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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.buttonCustomFontSettings = new Button();
            this.groupBoxFont = new GroupBox();
            this.layoutInfosComboBoxFonts = new LayoutManager.CommonUI.Controls.LayoutInfosComboBox();
            this.radioButtonCustomFont = new RadioButton();
            this.radioButtonStandardFont = new RadioButton();
            this.labelFontDescription = new Label();
            this.fontDialogCustomSetting = new System.Windows.Forms.FontDialog();
            this.groupBoxFont.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCustomFontSettings
            // 
            this.buttonCustomFontSettings.Location = new System.Drawing.Point(112, 45);
            this.buttonCustomFontSettings.Name = "buttonCustomFontSettings";
            this.buttonCustomFontSettings.Size = new System.Drawing.Size(88, 23);
            this.buttonCustomFontSettings.TabIndex = 2;
            this.buttonCustomFontSettings.Text = "Font setting...";
            this.buttonCustomFontSettings.Click += new System.EventHandler(this.buttonCustomFontSettings_Click);
            // 
            // groupBoxFont
            // 
            this.groupBoxFont.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                       this.layoutInfosComboBoxFonts,
                                                                                       this.radioButtonCustomFont,
                                                                                       this.radioButtonStandardFont,
                                                                                       this.labelFontDescription,
                                                                                       this.buttonCustomFontSettings});
            this.groupBoxFont.Location = new System.Drawing.Point(8, 8);
            this.groupBoxFont.Name = "groupBoxFont";
            this.groupBoxFont.Size = new System.Drawing.Size(304, 96);
            this.groupBoxFont.TabIndex = 1;
            this.groupBoxFont.TabStop = false;
            this.groupBoxFont.Text = "Font:";
            // 
            // layoutInfosComboBoxFonts
            // 
            this.layoutInfosComboBoxFonts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.layoutInfosComboBoxFonts.DropDownWidth = 176;
            this.layoutInfosComboBoxFonts.InfoContainer = null;
            this.layoutInfosComboBoxFonts.InfoType = null;
            this.layoutInfosComboBoxFonts.Location = new System.Drawing.Point(112, 21);
            this.layoutInfosComboBoxFonts.Name = "layoutInfosComboBoxFonts";
            this.layoutInfosComboBoxFonts.Size = new System.Drawing.Size(176, 21);
            this.layoutInfosComboBoxFonts.TabIndex = 4;
            // 
            // radioButtonCustomFont
            // 
            this.radioButtonCustomFont.Location = new System.Drawing.Point(16, 48);
            this.radioButtonCustomFont.Name = "radioButtonCustomFont";
            this.radioButtonCustomFont.Size = new System.Drawing.Size(96, 17);
            this.radioButtonCustomFont.TabIndex = 1;
            this.radioButtonCustomFont.Text = "Custom font:";
            this.radioButtonCustomFont.CheckedChanged += new System.EventHandler(this.radioButtonCustomFont_CheckedChanged);
            // 
            // radioButtonStandardFont
            // 
            this.radioButtonStandardFont.Location = new System.Drawing.Point(16, 19);
            this.radioButtonStandardFont.Name = "radioButtonStandardFont";
            this.radioButtonStandardFont.Size = new System.Drawing.Size(96, 24);
            this.radioButtonStandardFont.TabIndex = 0;
            this.radioButtonStandardFont.Text = "Standard font:";
            this.radioButtonStandardFont.CheckedChanged += new System.EventHandler(this.radioButtonStandardFont_CheckedChanged);
            // 
            // labelFontDescription
            // 
            this.labelFontDescription.Location = new System.Drawing.Point(40, 71);
            this.labelFontDescription.Name = "labelFontDescription";
            this.labelFontDescription.Size = new System.Drawing.Size(248, 16);
            this.labelFontDescription.TabIndex = 3;
            // 
            // TextProviderFontDefinition
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.groupBoxFont});
            this.Name = "TextProviderFontDefinition";
            this.Size = new System.Drawing.Size(320, 112);
            this.groupBoxFont.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private void buttonCustomFontSettings_Click(object sender, System.EventArgs e) {
            LayoutFontInfo fontProvider = getCustomFont();

            fontDialogCustomSetting.Color = fontProvider.Color;
            fontDialogCustomSetting.ShowColor = true;
            fontDialogCustomSetting.Font = fontProvider.Font;
            fontDialogCustomSetting.ShowDialog();

            fontProvider.Font = fontDialogCustomSetting.Font;
            fontProvider.Color = fontDialogCustomSetting.Color;
            labelFontDescription.Text = fontProvider.ToString();
        }

        private void radioButtonStandardFont_CheckedChanged(object sender, System.EventArgs e) {
            updateDependencies();
            labelFontDescription.Text = "";
        }

        private void radioButtonCustomFont_CheckedChanged(object sender, System.EventArgs e) {
            updateDependencies();
        }


    }
}
