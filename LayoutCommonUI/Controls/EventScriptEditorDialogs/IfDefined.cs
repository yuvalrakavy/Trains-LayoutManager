using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using MethodDispatcher;

namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for IfExist.
    /// </summary>
    public partial class IfDefined : Form {
        private readonly XmlElement element;
        private readonly Dictionary<string, Type> symbolNameToTypeMap = new();

        public IfDefined(XmlElement element) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.element = element;

            Dispatch.Call.AddContextSymbolsAndTypes(symbolNameToTypeMap);

            comboBoxSymbol.Sorted = true;
            foreach (string symbolName in symbolNameToTypeMap.Keys)
                comboBoxSymbol.Items.Add(symbolName);

            if (element.HasAttribute("Symbol"))
                comboBoxSymbol.Text = element.GetAttribute("Symbol");

            if (element.HasAttribute("Attribute")) {
                checkBoxAttribute.Checked = true;
                comboBoxAttribute.Text = element.GetAttribute("Attribute");
            }

            UpdateButtons();
        }

        private void InitializeAttributesList() {
            var symbolType = (Type?)symbolNameToTypeMap[comboBoxSymbol.Text];

            comboBoxAttribute.Items.Clear();

            if (symbolType != null) {
                var attributesList = new List<AttributesInfo>();
                var attributesMap = new Dictionary<string, AttributeInfo>();

                Dispatch.Call.GetObjectAttributes(attributesList, symbolType);

                foreach (var attributes in attributesList) {
                    foreach (AttributeInfo attribute in attributes) {
                        if (!attributesMap.ContainsKey(attribute.Name)) {
                            attributesMap.Add(attribute.Name, attribute);
                            comboBoxAttribute.Items.Add(attribute.Name);
                        }
                    }
                }
            }
        }

        private void UpdateButtons() {
            comboBoxAttribute.Enabled = checkBoxAttribute.Checked;
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


        private void CheckBoxAttribute_CheckedChanged(object? sender, EventArgs e) {
            UpdateButtons();
        }

        private void ButtonOK_Click(object? sender, EventArgs e) {
            if (comboBoxSymbol.Text.Trim() == "") {
                MessageBox.Show(this, "You did not provide a name of a symbol", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxSymbol.Focus();
                return;
            }

            if (checkBoxAttribute.Checked && comboBoxAttribute.Text.Trim() == "") {
                MessageBox.Show(this, "You did not provide a name of an attribute", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxAttribute.Focus();
                return;
            }

            element.SetAttribute("Symbol", comboBoxSymbol.Text);

            if (checkBoxAttribute.Checked)
                element.SetAttribute("Attribute", comboBoxAttribute.Text);
            else
                element.RemoveAttribute("Attribute");

            DialogResult = DialogResult.OK;
        }

        private void ComboBoxAttribute_DropDown(object? sender, EventArgs e) {
            InitializeAttributesList();
        }
    }
}
