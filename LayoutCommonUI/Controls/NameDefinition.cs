
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for NameDefinition.
    /// </summary>
    public partial class NameDefinition : UserControl {
        private LayoutXmlInfo? xmlInfo;

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
                return Ensure.NotNull<LayoutXmlInfo>(xmlInfo);
            }

            set {
                xmlInfo = value;
                if (xmlInfo != null)
                    Initialize();
            }
        }

        public ModelComponent? Component { get; set; } = null;

        private void Initialize() {
            LayoutTextInfo textProvider = GetTextProvider(false);

            textBoxName.Text = textProvider.Text;
            if (textProvider.OptionalElement == null)
                checkBoxVisible.Checked = DefaultIsVisible;
            else
                checkBoxVisible.Checked = textProvider.Visible;

            UpdateDependencies();
        }

        private void UpdateDependencies() {
            buttonSettings.Enabled = checkBoxVisible.Checked;
        }

        private LayoutTextInfo GetTextProvider(bool create) {
            LayoutTextInfo textProvider;

            textProvider = new LayoutTextInfo(XmlInfo.DocumentElement, ElementName);

            if (Component != null)
                textProvider.Component = Component;

            if (textProvider.OptionalElement == null && create)
                textProvider.Element = LayoutInfo.CreateProviderElement(XmlInfo, ElementName, null);

            return textProvider;
        }

        public bool Commit() {
            if (textBoxName.Text.Trim() == "" && !IsOptional) {
                MessageBox.Show(this, "You must enter a value", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Focus();
                return false;
            }

            if (textBoxName.Text.Trim() != "") {
                LayoutTextInfo textProvider = GetTextProvider(true);

                textProvider.Text = textBoxName.Text;
                textProvider.Visible = checkBoxVisible.Checked;
            }

            return true;
        }

        public void Set(string text, bool visible) {
            LayoutTextInfo textProvider = GetTextProvider(true);

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

        private void CheckBoxVisible_CheckedChanged(object? sender, EventArgs e) {
            UpdateDependencies();
        }

        private void ButtonSettings_Click(object? sender, EventArgs e) {
            LayoutTextInfo textProvider = GetTextProvider(true);
            Dialogs.TextProviderSettings textProviderSettings = new(XmlInfo, textProvider);

            textProviderSettings.ShowDialog(this);
        }
    }
}
