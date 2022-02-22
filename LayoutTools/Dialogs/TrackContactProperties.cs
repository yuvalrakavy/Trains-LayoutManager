using LayoutManager.Components;
using LayoutManager.Model;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TrackContactProperties.
    /// </summary>
    public partial class TriggerableBlockEdgeProperties : Form, ILayoutComponentPropertiesDialog {
        public TriggerableBlockEdgeProperties(ModelComponent component) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            var t = component.ToString();
            this.Text = $"{char.ToUpper(t[0]) + t[1..]} properties";

            this.XmlInfo = new LayoutXmlInfo(component);

            nameDefinition.XmlInfo = XmlInfo;
            nameDefinition.IsOptional = true;
            nameDefinition.Component = component;

            checkBoxEmergencyContact.Checked = IsEmergencyContact;

            attributesEditor.AttributesSource = typeof(LayoutTriggerableBlockEdgeBase);
            attributesEditor.AttributesOwner = new AttributesOwner(XmlInfo.Element);
        }

        private bool IsEmergencyContact {
            get => (bool?)XmlInfo.Element.AttributeValue(LayoutTriggerableBlockEdgeBase.A_EmergencyContact) ?? false;
            set => XmlInfo.Element.SetAttributeValue(LayoutTriggerableBlockEdgeBase.A_EmergencyContact, value, removeIf: false);
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
            if (nameDefinition.Commit() && attributesEditor.Commit()) {
                this.DialogResult = DialogResult.OK;
                Close();
            }

            IsEmergencyContact = checkBoxEmergencyContact.Checked;
        }
    }
}
