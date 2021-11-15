using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Tools.EventScriptDialogs {
    /// <summary>
    /// Summary description for ExecuteRandomTripPlan.
    /// </summary>
    public partial class ExecuteRandomTripPlan : Form {
        private const string A_Symbol = "Symbol";
        private const string A_SelectCircularTripPlans = "SelectCircularTripPlans";
        private const string A_ReversedTripPlanSelection = "ReversedTripPlanSelection";
        private const string E_Filter = "Filter";

        private readonly XmlElement element;

        private XmlElement? newFilterElement = null;
        private bool conditionEdited = false;

        public ExecuteRandomTripPlan(XmlElement element) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.element = element;

            if (element.HasAttribute(A_Symbol))
                comboBoxSymbol.Text = element.GetAttribute(A_Symbol);

            checkBoxCircularMayBeSelected.Checked = (bool?)element.AttributeValue(A_SelectCircularTripPlans) ?? false;

            if (element.HasAttribute(A_ReversedTripPlanSelection)) {
                switch (element.GetAttribute(A_ReversedTripPlanSelection)) {
                    case "Yes":
                        radioButtonReversedSelected.Checked = true;
                        break;

                    case "No":
                        radioButtonRevresedNotSelected.Checked = true;
                        break;

                    case "IfNoAlternative":
                    default:
                        radioButtonReversedMayBeSelected.Checked = true;
                        break;
                }
            }
            else
                radioButtonReversedMayBeSelected.Checked = true;

            UpdateConditionDescription(element[E_Filter]);
        }

        private void UpdateConditionDescription(XmlElement? conditionElement) {
            if (conditionElement == null || conditionElement.ChildNodes.Count < 1)
                textBoxCondition.Text = "<No condition>";
            else
                textBoxCondition.Text = Ensure.NotNull<string>(EventManager.Event(new LayoutEvent("get-event-script-description", conditionElement.ChildNodes[0])));
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

        private void ButtonOK_Click(object? sender, System.EventArgs e) {
            if (comboBoxSymbol.Text.Trim() == null) {
                MessageBox.Show(this, "You did not provide symbol name", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxSymbol.Focus();
                return;
            }

            if (conditionEdited) {
                XmlElement? filterElement = element[E_Filter];

                if (filterElement != null)
                    element.RemoveChild(filterElement);

                if (newFilterElement != null) {
                    filterElement = (XmlElement)element.OwnerDocument.ImportNode(newFilterElement, true);

                    element.AppendChild(filterElement);
                }
            }

            element.SetAttribute(A_Symbol, comboBoxSymbol.Text);
            element.SetAttributeValue(A_SelectCircularTripPlans, checkBoxCircularMayBeSelected.Checked);

            string v;

            if (radioButtonReversedSelected.Checked)
                v = "Yes";
            else if (radioButtonRevresedNotSelected.Checked)
                v = "No";
            else
                v = "IfNoAlternative";

            element.SetAttribute(A_ReversedTripPlanSelection, v);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonConditionEdit_Click(object? sender, System.EventArgs e) {
            XmlElement? filterElement;

            if (newFilterElement == null) {
                filterElement = element[E_Filter];

                if (filterElement == null) {
                    XmlDocument dummyDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();

                    filterElement = dummyDoc.CreateElement(E_Filter);
                    dummyDoc.AppendChild(filterElement);
                }
            }
            else
                filterElement = newFilterElement;

            LayoutManager.CommonUI.Dialogs.ConditionDefinition d = new(filterElement);

            if (d.ShowDialog(this) == DialogResult.OK) {
                if (d.NoCondition)
                    newFilterElement = null;
                else
                    newFilterElement = d.ConditionElement;

                UpdateConditionDescription(newFilterElement);
                conditionEdited = true;
            }
        }
    }
}
