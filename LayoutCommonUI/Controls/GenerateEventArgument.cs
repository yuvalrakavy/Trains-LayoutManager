using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using MethodDispatcher;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for GenerateEventArgument.
    /// </summary>
    public partial class GenerateEventArgument : UserControl {
        private const string A_Value = "Value";
        private const string A_ConstantType = "ConstantType";
        private const string A_Type = "Type";

        public GenerateEventArgument() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        public XmlElement? OptionalElement { get; set; }

        public string Prefix { get; set; } = "Arg";

        public void Initialize() {
            if (OptionalElement == null)
                throw new ArgumentException("Element property not set");

            operandValueOf.OptionalElement = OptionalElement;
            operandValueOf.Suffix = Prefix;
            operandValueOf.Initialize();

            Dictionary<string, Type> symbolNameToTypeMap = new();

            Dispatch.Call.AddContextSymbolsAndTypes(symbolNameToTypeMap);

            foreach (string n in symbolNameToTypeMap.Keys)
                comboBoxReferencedObject.Items.Add(n);

            var symbolName = GetAttribute("SymbolName");

            if (symbolName != null) {
                comboBoxReferencedObject.Text = symbolName;

                foreach (string n in comboBoxReferencedObject.Items)
                    if (n == symbolName) {
                        comboBoxReferencedObject.SelectedItem = n;
                        break;
                    }
            }

            switch (GetAttribute(A_ConstantType, "String")) {
                case "String":
                    linkMenuConstantType.SelectedIndex = 0;
                    break;

                case "Integer":
                    linkMenuConstantType.SelectedIndex = 1;
                    break;

                case "Double":
                    linkMenuConstantType.SelectedIndex = 2;
                    break;

                case "Boolean":
                    linkMenuConstantType.SelectedIndex = 3;
                    break;
            }

            string constant = OptionalElement.GetAttribute($"{A_Value}{Prefix}");

            comboBoxBooleanConstant.SelectedIndex = 0;

            if (constant != null) {
                if (linkMenuConstantType.SelectedIndex == 3)
                    comboBoxBooleanConstant.SelectedIndex = bool.Parse(constant) ? 1 : 0;
                else
                    textBoxConstantValue.Text = constant;
            }

            switch (GetAttribute(A_Type, "Null")) {
                case "Null":
                    radioButtonNull.Checked = true;
                    break;

                case "Reference":
                    radioButtonObjectReference.Checked = true;
                    break;

                case "ValueOf":
                    radioButtonConstant.Checked = true;
                    break;

                case "Context":
                    radioButtonContext.Checked = true;
                    break;

                default:
                    throw new ApplicationException("Invalid type");
            }

            UpdateButtons();
        }

        public bool ValidateInput() {
            if (radioButtonObjectReference.Checked) {
                if (comboBoxReferencedObject.Text.Trim() == "") {
                    MessageBox.Show(this, "You must provide a name of a symbol", "Missing Symbol Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    comboBoxReferencedObject.Focus();
                    return false;
                }
            }
            else if (radioButtonConstant.Checked) {
                switch (linkMenuConstantType.SelectedIndex) {
                    case 1:
                        try {
                            int.Parse(textBoxConstantValue.Text);
                        }
                        catch (FormatException) {
                            MessageBox.Show(this, "Invalid integer", "Invalid constant format", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textBoxConstantValue.Focus();
                            return false;
                        }
                        break;

                    case 2:
                        try {
                            double.Parse(textBoxConstantValue.Text);
                        }
                        catch (FormatException) {
                            MessageBox.Show(this, "Invalid double (real) number", "Invalid constant format", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textBoxConstantValue.Focus();
                            return false;
                        }
                        break;
                }
            }
            else if (radioButtonValueOf.Checked) {
                if (!operandValueOf.ValidateInput())
                    return false;
            }

            return true;
        }

        public bool Commit() {
            if (!ValidateInput())
                return false;

            if (radioButtonNull.Checked)
                SetAttribute(A_Type, "Null");
            else if (radioButtonObjectReference.Checked) {
                SetAttribute(A_Type, "Reference");
                SetAttribute("SymbolName", comboBoxReferencedObject.Text);
            }
            else if (radioButtonValueOf.Checked) {
                SetAttribute(A_Type, "ValueOf");
                operandValueOf.Commit();
            }
            else if (radioButtonConstant.Checked) {
                SetAttribute(A_Type, "ValueOf");
                OptionalElement?.SetAttribute("Symbol" + Prefix + "Access", A_Value);

                switch (linkMenuConstantType.SelectedIndex) {
                    case 0:
                        OptionalElement?.SetAttribute(A_Type + Prefix, "String");
                        break;

                    case 1:
                        OptionalElement?.SetAttribute(A_Type + Prefix, "Integer");
                        break;

                    case 2:
                        OptionalElement?.SetAttribute(A_Type + Prefix, "Double");
                        break;

                    case 3:
                        OptionalElement?.SetAttribute(A_Type + Prefix, "Boolean");
                        break;

                    default:
                        throw new ApplicationException("Invalid constant type");
                }

                if (linkMenuConstantType.SelectedIndex == 3)
                    OptionalElement?.SetAttributeValue(A_Value + Prefix, comboBoxBooleanConstant.SelectedIndex == 1);
                else
                    OptionalElement?.SetAttribute(A_Value + Prefix, textBoxConstantValue.Text);
            }
            else if (radioButtonContext.Checked)
                SetAttribute(A_Type, "Context");

            return true;
        }

        private void UpdateButtons() {
            if (linkMenuConstantType.SelectedIndex == 3) {
                textBoxConstantValue.Visible = false;
                comboBoxBooleanConstant.Location = textBoxConstantValue.Location;
                comboBoxBooleanConstant.Visible = true;
            }
            else {
                textBoxConstantValue.Visible = true;
                comboBoxBooleanConstant.Visible = false;
            }
        }

        protected string? GetAttribute(string name, string? defaultValue) {
            string attributeName = Prefix + name;

            return OptionalElement == null || !OptionalElement.HasAttribute(attributeName) ? defaultValue : OptionalElement.GetAttribute(attributeName);
        }

        protected string? GetAttribute(string name) => GetAttribute(name, null);

        protected void SetAttribute(string name, string value) {
            OptionalElement?.SetAttribute(Prefix + name, value);
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

        private void ComboBoxReferencedObject_SelectedIndexChanged(object? sender, EventArgs e) {
            radioButtonObjectReference.Checked = true;
        }

        private void ComboBoxReferencedObject_TextChanged(object? sender, EventArgs e) {
            radioButtonObjectReference.Checked = true;
        }

        private void TextBoxConstantValue_TextChanged(object? sender, EventArgs e) {
            radioButtonConstant.Checked = true;
        }

        private void LinkMenuConstantType_ValueChanged(object? sender, EventArgs e) {
            radioButtonConstant.Checked = true;
            UpdateButtons();
        }

        private void OperandValueOf_ValueChanged(object? sender, EventArgs e) {
            radioButtonValueOf.Checked = true;
        }
    }
}
