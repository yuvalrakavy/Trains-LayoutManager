using System.Xml;

namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for WaitForEvent.
    /// </summary>
    public partial class WaitForEvent : Form, IObjectHasXml {
        private const string A_Name = "Name";
        private const string A_LimitToScope = "LimitToScope";

        public WaitForEvent(XmlElement element) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.Element = element;

            comboBoxEvent.Text = (string?)element.AttributeValue(A_Name) ?? "";
            checkBoxLimitedToScope.Checked = (bool?)element.AttributeValue(A_LimitToScope) ?? true;

            // Populate the combobox with possible event that have notification role
            foreach (LayoutEventDefAttribute eventDef in EventManager.Instance.GetEventDefinitions(LayoutEventRole.Notification))
                comboBoxEvent.Items.Add(eventDef.Name);
        }

        public XmlElement Element { get; }
        public XmlElement? OptionalElement => Element;

        
        private void ButtonOK_Click(object? sender, EventArgs e) {
            if (comboBoxEvent.Text.Trim()?.Length == 0) {
                MessageBox.Show(this, "You must specify event name", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxEvent.Focus();
                return;
            }

            Element.SetAttribute(A_Name, comboBoxEvent.Text);
            Element.SetAttributeValue(A_LimitToScope, checkBoxLimitedToScope.Checked, removeIf: true);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object? sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
