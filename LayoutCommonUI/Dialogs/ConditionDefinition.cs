using System.Xml;

namespace LayoutManager.CommonUI.Dialogs {
    /// <summary>
    /// Summary description for PolicyDefinition.
    /// </summary>
    public partial class ConditionDefinition : Form {

        private readonly XmlElement conditionElement;

        public ConditionDefinition(XmlElement inConditionElement) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.conditionElement = inConditionElement;

            XmlDocument scriptDocument = LayoutXmlInfo.XmlImplementation.CreateDocument();
            XmlElement? conditionBodyElement = null;

            conditionElement = (XmlElement)scriptDocument.ImportNode(inConditionElement, true);

            scriptDocument.AppendChild(conditionElement);

            if (conditionElement.ChildNodes.Count > 0)
                conditionBodyElement = (XmlElement)conditionElement.ChildNodes[0]!;

            if (conditionBodyElement != null) {
                radioButtonCondition.Checked = true;
                eventScriptEditor.Enabled = true;
            }
            else {
                radioButtonNoCondition.Checked = true;
                conditionBodyElement = scriptDocument.CreateElement("And");
                conditionElement.AppendChild(conditionBodyElement);
                eventScriptEditor.Enabled = false;
            }

            eventScriptEditor.EventScriptElement = conditionBodyElement;
        }

        public XmlElement ConditionElement {
            get {
                if (radioButtonNoCondition.Checked) {
                    if (conditionElement.ChildNodes.Count > 0)
                        conditionElement.RemoveChild(conditionElement.ChildNodes[0]!);
                }

                return conditionElement;
            }
        }

        public bool NoCondition => radioButtonNoCondition.Checked;

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
            if (radioButtonCondition.Checked) {
                if (!eventScriptEditor.ValidateScript()) {
                    eventScriptEditor.Focus();
                    return;
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object? sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void RadioButtonNoCondition_CheckedChanged(object? sender, EventArgs e) {
            eventScriptEditor.Enabled = false;
        }

        private void RadioButtonCondition_CheckedChanged(object? sender, EventArgs e) {
            eventScriptEditor.Enabled = true;
        }
    }
}
