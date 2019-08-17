using System;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for NameDefinition.
    /// </summary>
    public class NameDefinition : System.Windows.Forms.UserControl {
        private CheckBox checkBoxVisible;
        private TextBox textBoxName;
        private Button buttonSettings;
        private Label labelName;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
        private LayoutXmlInfo xmlInfo;

        public NameDefinition() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitForm call

        }

        public string ElementName { get; set; } = "Name";

        public bool IsOptional { get; set; } = false;

        public bool IsEmptyName => textBoxName.Text.Trim() == "";

        public bool DefaultIsVisible { set; get; } = true;

        public LayoutXmlInfo XmlInfo {
            get {
                return xmlInfo;
            }

            set {
                xmlInfo = value;
                if (xmlInfo != null)
                    initialize();
            }
        }

        public ModelComponent Component { get; set; } = null;

        private void initialize() {
            LayoutTextInfo textProvider = getTextProvider(false);

            textBoxName.Text = textProvider.Text;
            if (textProvider.OptionalElement == null)
                checkBoxVisible.Checked = DefaultIsVisible;
            else
                checkBoxVisible.Checked = textProvider.Visible;

            updateDependencies();
        }

        private void updateDependencies() {
            buttonSettings.Enabled = checkBoxVisible.Checked;
        }

        private LayoutTextInfo getTextProvider(bool create) {
            LayoutTextInfo textProvider;

            textProvider = new LayoutTextInfo(xmlInfo.DocumentElement, ElementName);

            if (Component != null)
                textProvider.Component = Component;

            if (textProvider.OptionalElement == null && create)
                textProvider.Element = LayoutInfo.CreateProviderElement(xmlInfo, ElementName, null);

            return textProvider;
        }

        public bool Commit() {
            if (textBoxName.Text.Trim() == "" && !IsOptional) {
                MessageBox.Show(this, "You must enter a value", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Focus();
                return false;
            }

            if (textBoxName.Text.Trim() != "") {
                LayoutTextInfo textProvider = getTextProvider(true);

                textProvider.Text = textBoxName.Text;
                textProvider.Visible = checkBoxVisible.Checked;
            }

            return true;
        }

        public void Set(string text, bool visible) {
            LayoutTextInfo textProvider = getTextProvider(true);

            textProvider.Text = text;
            textProvider.Visible = visible;
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
            this.checkBoxVisible = new CheckBox();
            this.textBoxName = new TextBox();
            this.buttonSettings = new Button();
            this.labelName = new Label();
            this.SuspendLayout();
            // 
            // checkBoxVisible
            // 
            this.checkBoxVisible.Location = new System.Drawing.Point(64, 28);
            this.checkBoxVisible.Name = "checkBoxVisible";
            this.checkBoxVisible.Size = new System.Drawing.Size(64, 24);
            this.checkBoxVisible.TabIndex = 2;
            this.checkBoxVisible.Text = "Visible";
            this.checkBoxVisible.CheckedChanged += this.checkBoxVisible_CheckedChanged;
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.textBoxName.Location = new System.Drawing.Point(64, 4);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(208, 20);
            this.textBoxName.TabIndex = 1;
            this.textBoxName.Text = "";
            // 
            // buttonSettings
            // 
            this.buttonSettings.Location = new System.Drawing.Point(136, 28);
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.TabIndex = 3;
            this.buttonSettings.Text = "Settings...";
            this.buttonSettings.Click += this.buttonSettings_Click;
            // 
            // labelName
            // 
            this.labelName.Location = new System.Drawing.Point(8, 6);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(48, 16);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "Name:";
            this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NameDefinition
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.checkBoxVisible,
                                                                          this.textBoxName,
                                                                          this.buttonSettings,
                                                                          this.labelName});
            this.Name = "NameDefinition";
            this.Size = new System.Drawing.Size(280, 53);
            this.ResumeLayout(false);
        }
        #endregion

        private void checkBoxVisible_CheckedChanged(object sender, System.EventArgs e) {
            updateDependencies();
        }

        private void buttonSettings_Click(object sender, System.EventArgs e) {
            LayoutTextInfo textProvider = getTextProvider(true);
            Dialogs.TextProviderSettings textProviderSettings = new Dialogs.TextProviderSettings(xmlInfo, textProvider);

            textProviderSettings.ShowDialog(this);
        }
    }
}
