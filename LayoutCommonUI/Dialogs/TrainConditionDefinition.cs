using System.Xml;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.CommonUI.Dialogs {
    /// <summary>
    /// Summary description for PolicyDefinition.
    /// </summary>
    public partial class TrainConditionDefinition : Form {
        private readonly TripPlanTrainConditionInfo trainCondition;

        public TrainConditionDefinition(LayoutBlockDefinitionComponent blockDefinition, TripPlanTrainConditionInfo inTrainCondition) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            XmlDocument scriptDocument = LayoutXmlInfo.XmlImplementation.CreateDocument();
            XmlElement? conditionBodyElement = null;

            trainCondition = new TripPlanTrainConditionInfo((XmlElement)scriptDocument.ImportNode(inTrainCondition.Element, true));

            scriptDocument.AppendChild(trainCondition.Element);

            if (!trainCondition.IsConditionEmpty)
                conditionBodyElement = trainCondition.ConditionBodyElement;

            if (!trainCondition.IsConditionEmpty) {
                radioButtonCondition.Checked = true;
                eventScriptEditor.Enabled = true;
            }
            else {
                radioButtonNoCondition.Checked = true;
                conditionBodyElement = scriptDocument.CreateElement("And");
                trainCondition.Element.AppendChild(conditionBodyElement);
                eventScriptEditor.Enabled = false;
            }

            eventScriptEditor.EventScriptElement = conditionBodyElement;
            eventScriptEditor.BlockDefinition = blockDefinition;

            if (trainCondition.ConditionScope == TripPlanTrainConditionScope.AllowIfTrue)
                linkMenuConditionScope.SelectedIndex = 0;
            else
                linkMenuConditionScope.SelectedIndex = 1;
        }

        public TripPlanTrainConditionInfo TrainCondition {
            get {
                if (radioButtonNoCondition.Checked) {
                    if (!trainCondition.IsConditionEmpty)
                        trainCondition.Element.RemoveChild(trainCondition.ConditionBodyElement);
                }

                return trainCondition;
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

            if (linkMenuConditionScope.SelectedIndex == 0)
                trainCondition.ConditionScope = TripPlanTrainConditionScope.AllowIfTrue;
            else
                trainCondition.ConditionScope = TripPlanTrainConditionScope.DisallowIfTrue;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object? sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void RadioButtonNoCondition_CheckedChanged(object? sender, EventArgs e) {
            eventScriptEditor.Enabled = false;
            linkMenuConditionScope.Enabled = false;
        }

        private void RadioButtonCondition_CheckedChanged(object? sender, EventArgs e) {
            eventScriptEditor.Enabled = true;
            linkMenuConditionScope.Enabled = true;
        }
    }
}
