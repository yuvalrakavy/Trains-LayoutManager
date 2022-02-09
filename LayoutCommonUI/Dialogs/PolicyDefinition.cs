using System.Xml;
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Dialogs {
    /// <summary>
    /// Summary description for PolicyDefinition.
    /// </summary>
    public partial class PolicyDefinition : Form {

        private readonly XmlElement scriptElement;
        private readonly LayoutPolicyInfo policy;

        public PolicyDefinition(LayoutPolicyInfo policy) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.policy = policy;

            XmlDocument scriptDocument = LayoutXmlInfo.XmlImplementation.CreateDocument();

            if (policy.EventScriptElement == null)
                scriptElement = scriptDocument.CreateElement("Sequence");
            else
                scriptElement = (XmlElement)scriptDocument.ImportNode(policy.EventScriptElement, true);

            scriptDocument.AppendChild(scriptElement);

            textBoxName.Text = policy.Name;
            checkBoxApply.Checked = policy.Apply;
            checkBoxGlobalPolicy.Checked = policy.GlobalPolicy;

            if (policy.Scope != "Global" && policy.Scope != "TripPlan")
                checkBoxApply.Visible = false;

            if (policy.Scope == "Global")
                checkBoxShowInMenu.Checked = policy.ShowInMenu;
            else
                checkBoxShowInMenu.Visible = false;

            eventScriptEditor.EventScriptElement = scriptElement;
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
                MessageBox.Show(this, "You must provide name for the policy", "Missing Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxName.Focus();
                return;
            }

            if (!eventScriptEditor.ValidateScript(checkBoxGlobalPolicy.Checked)) {
                eventScriptEditor.Focus();
                return;
            }

            var scriptElement = eventScriptEditor.EventScriptElement;

            if (scriptElement != null) {
                policy.Name = textBoxName.Text;
                policy.Apply = checkBoxApply.Checked;
                policy.EventScriptElement = scriptElement;
                policy.GlobalPolicy = checkBoxGlobalPolicy.Checked;
                policy.ShowInMenu = checkBoxShowInMenu.Checked;
                DialogResult = DialogResult.OK;
            }
            else
                DialogResult = DialogResult.Cancel;

            Close();
        }

        private void ButtonCancel_Click(object? sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
