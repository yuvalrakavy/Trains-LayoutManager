using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using MethodDispatcher;

namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for SetAttribute.
    /// </summary>
    public partial class SetAttribute : Form {
        private const string A_Value = "Value";
        private const string A_SetTo = "SetTo";

        private readonly XmlElement element;
        private readonly Dictionary<string, Type>? symbolNameToTypeMap = null;

        public SetAttribute(XmlElement element) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.element = element;

            symbolNameToTypeMap = new();

            Dispatch.Call.AddContextSymbolsAndTypes(symbolNameToTypeMap);

            comboBoxSymbol.Sorted = true;
            foreach (string symbolName in symbolNameToTypeMap.Keys)
                comboBoxSymbol.Items.Add(symbolName);

            if (element.HasAttribute("Symbol"))
                comboBoxSymbol.Text = element.GetAttribute("Symbol");

            if (element.HasAttribute("Attribute"))
                comboBoxAttribute.Text = element.GetAttribute("Attribute");

            string setTo = "Text";

            if (element.HasAttribute(A_SetTo))
                setTo = element.GetAttribute(A_SetTo);

            switch (setTo) {
                case "Text":
                    linkMenuType.SelectedIndex = 0;
                    if (element.HasAttribute(A_Value))
                        textBoxTextValue.Text = element.GetAttribute(A_Value);
                    break;

                case "Number":
                    linkMenuType.SelectedIndex = 1;
                    break;

                case "Boolean":
                    if ((bool)element.AttributeValue(A_Value))
                        radioButtonValueTrue.Checked = true;
                    else
                        radioButtonValueFalse.Checked = true;

                    linkMenuType.SelectedIndex = 2;
                    break;

                case "ValueOf":
                    linkMenuType.SelectedIndex = 3;
                    break;

                case "Remove":
                    linkMenuType.SelectedIndex = 4;
                    break;
            }

            numericValue.Element = element;
            operandValueOf.OptionalElement = element;
            operandValueOf.Suffix = "To";
            operandValueOf.AllowedTypes = new Type[] { typeof(string), typeof(Enum), typeof(int) };
            operandValueOf.Initialize();

            UpdateControls();
        }

        private void UpdateControls() {
            switch (linkMenuType.SelectedIndex) {
                case 0:     // Text
                    textBoxTextValue.Visible = true;
                    numericValue.Visible = false;
                    groupBoxValueBoolean.Visible = false;
                    operandValueOf.Visible = false;
                    break;

                case 1:     // Number
                    SuspendLayout();
                    textBoxTextValue.Visible = false;
                    numericValue.Visible = true;
                    numericValue.Location = textBoxTextValue.Location;
                    groupBoxValueBoolean.Visible = false;
                    operandValueOf.Visible = false;
                    ResumeLayout();
                    break;

                case 2:     // Boolean
                    SuspendLayout();
                    textBoxTextValue.Visible = false;
                    numericValue.Visible = false;
                    groupBoxValueBoolean.Location = textBoxTextValue.Location;
                    groupBoxValueBoolean.Visible = true;
                    if (!radioButtonValueFalse.Checked && !radioButtonValueTrue.Checked)
                        radioButtonValueFalse.Checked = true;
                    operandValueOf.Visible = false;
                    ResumeLayout();
                    break;

                case 3:     // Value of
                    SuspendLayout();
                    textBoxTextValue.Visible = false;
                    numericValue.Visible = false;
                    groupBoxValueBoolean.Visible = false;
                    operandValueOf.Location = textBoxTextValue.Location;
                    operandValueOf.Visible = true;
                    ResumeLayout();
                    break;

                case 4:     // Remove
                    textBoxTextValue.Visible = false;
                    numericValue.Visible = false;
                    groupBoxValueBoolean.Visible = false;
                    operandValueOf.Visible = false;
                    break;
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

        private void LinkMenuType_ValueChanged(object? sender, EventArgs e) {
            UpdateControls();
        }

        private void ComboBoxAttribute_DropDown(object? sender, EventArgs e) {
            comboBoxAttribute.Items.Clear();

            var symbolType = (Type?)Ensure.NotNull<HybridDictionary>(symbolNameToTypeMap)[comboBoxSymbol.Text];

            if (symbolType != null) {
                var attributesList = new List<AttributesInfo>();
                var attributesMap = new Dictionary<string, AttributeInfo>();

                Dispatch.Call.GetObjectAttributes(attributesList, symbolType);

                foreach (var attributes in attributesList) {
                    foreach (var attribute in attributes) {
                        if (!attributesMap.ContainsKey(attribute.Name)) {
                            attributesMap.Add(attribute.Name, attribute);
                            comboBoxAttribute.Items.Add(attribute.Name);
                        }
                    }
                }
            }
        }

        private void ButtonOK_Click(object? sender, EventArgs e) {
            if (comboBoxSymbol.Text.Trim() == "") {
                MessageBox.Show(this, "You did not provide a name of a symbol", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxSymbol.Focus();
                return;
            }

            if (comboBoxAttribute.Text.Trim() == "") {
                MessageBox.Show(this, "You did not provide a name of an attribute", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxAttribute.Focus();
                return;
            }

            if (linkMenuType.SelectedIndex == 1) {
                if (!numericValue.ValidateInput())
                    return;
            }

            element.SetAttribute("Symbol", comboBoxSymbol.Text);
            element.SetAttribute("Attribute", comboBoxAttribute.Text);

            switch (linkMenuType.SelectedIndex) {
                case 0:
                    element.SetAttribute(A_SetTo, "Text");
                    element.SetAttribute(A_Value, textBoxTextValue.Text);
                    break;

                case 1:
                    element.SetAttribute(A_SetTo, "Number");
                    numericValue.Commit();
                    break;

                case 2:
                    element.SetAttribute(A_SetTo, "Boolean");
                    element.SetAttributeValue(A_Value, radioButtonValueTrue.Checked);
                    break;

                case 3:
                    element.SetAttribute(A_SetTo, "ValueOf");
                    operandValueOf.Commit();
                    break;

                case 4:
                    element.SetAttribute(A_SetTo, "Remove");
                    break;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
