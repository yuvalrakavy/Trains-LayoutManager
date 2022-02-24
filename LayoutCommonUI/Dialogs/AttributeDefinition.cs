using System.Collections;
using System.Collections.Specialized;
using MethodDispatcher;

namespace LayoutManager.CommonUI.Dialogs {
    /// <summary>
    /// Summary description for AttributeDefinition.
    /// </summary>
    public partial class AttributeDefinition : Form {

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Controls.ICheckIfNameUsed checkName;
        private readonly string? originalAttributeName;
        private readonly Type? attributesSource;
        private readonly IDictionary attributesMap = new HybridDictionary();
        private List<AttributesInfo>? attributesList = null;

        public AttributeDefinition(Type? attributeSource, Controls.ICheckIfNameUsed checkName, string? attributeName, object? attributeValue) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.checkName = checkName;
            this.originalAttributeName = attributeName;
            this.attributesSource = attributeSource;

            if (attributeName != null)
                comboBoxName.Text = attributeName;

            if (attributeValue != null) {
                if (attributeValue is bool boolean) {
                    radioButtonTypeBoolean.Checked = true;

                    if (boolean)
                        radioButtonValueTrue.Checked = true;
                    else
                        radioButtonValueFalse.Checked = true;
                }
                else if (attributeValue is int) {
                    radioButtonTypeNumber.Checked = true;
                    textBoxValue.Text = attributeValue.ToString();
                }
                else if (attributeValue is string aString) {
                    radioButtonTypeString.Checked = true;
                    textBoxValue.Text = aString;
                }
                else
                    throw new ArgumentException("Attribute value has invalid type: " + attributeValue.GetType().Name);
            }
            else {
                radioButtonTypeString.Checked = true;
                radioButtonValueTrue.Checked = true;
            }

            UpdateButtons(null, EventArgs.Empty);
        }

        #region Properties

        public string AttributeName => comboBoxName.Text;

        public object Value {
            get {
                if (radioButtonTypeBoolean.Checked)
                    return (bool)radioButtonValueTrue.Checked;
                else return radioButtonTypeNumber.Checked ? int.Parse(textBoxValue.Text) : (object)(string)textBoxValue.Text;
            }
        }

        #endregion

        private void UpdateButtons(object? sender, EventArgs e) {
            if (radioButtonTypeBoolean.Checked) {
                panelTextValue.Visible = false;
                groupBoxBooleanValue.Visible = true;
            }
            else {
                panelTextValue.Visible = true;
                groupBoxBooleanValue.Visible = false;
            }
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
            if (string.IsNullOrWhiteSpace(comboBoxName.Text)) {
                MessageBox.Show(this, "You must specify a name for the attribute", "Missing Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxName.Focus();
                return;
            }

            if (comboBoxName.Text != originalAttributeName && checkName.IsUsed(comboBoxName.Text)) {
                MessageBox.Show(this, "Another attribute already has that name", "Duplicate Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxName.Focus();
                return;
            }

            if (radioButtonTypeNumber.Checked) {
                try {
                    int.Parse(textBoxValue.Text);
                }
                catch (FormatException) {
                    MessageBox.Show(this, "Value is not a valid number", "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxValue.Focus();
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

        private void ComboBoxName_DropDown(object? sender, EventArgs e) {
            if (attributesList == null) {
                attributesList = new List<AttributesInfo>();

                if(attributesSource != null)
                    Dispatch.Call.GetObjectAttributes(attributesList, attributesSource);
            }

            foreach (AttributesInfo attributes in attributesList) {
                foreach (AttributeInfo attribute in attributes) {
                    if (!checkName.IsUsed(attribute.Name) && !attributesMap.Contains(attribute.Name)) {
                        attributesMap.Add(attribute.Name, attribute);
                        comboBoxName.Items.Add(attribute.Name);
                    }
                }
            }
        }

        private void ComboBoxName_SelectionChangeCommitted(object? sender, EventArgs e) {
            var attribute = (AttributeInfo?)attributesMap[comboBoxName.SelectedItem];

            if (attribute != null) {
                switch (attribute.AttributeType) {
                    case AttributeType.Boolean:
                        radioButtonTypeBoolean.Checked = true;
                        break;

                    case AttributeType.Number:
                        radioButtonTypeNumber.Checked = true;
                        break;

                    case AttributeType.String:
                        radioButtonTypeString.Checked = true;
                        break;
                }

                UpdateButtons(null, EventArgs.Empty);
            }
        }
    }
}
