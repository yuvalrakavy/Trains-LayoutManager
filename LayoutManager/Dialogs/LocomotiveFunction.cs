using LayoutManager.Model;
using System;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveFunction.
    /// </summary>
    public partial class LocomotiveFunction : Form {
        private const string A_Name = "Name";
        private const string A_Description = "Description";
        private const string A_Type = "Type";

        private readonly LocomotiveCatalogInfo catalog;
        private readonly XmlElement functionsElement;
        private readonly XmlElement functionElement;
        private bool typeChanged = false;
        private bool descriptionChanged = false;

        public LocomotiveFunction(LocomotiveCatalogInfo catalog, XmlElement functionsElement, XmlElement functionElement) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.catalog = catalog;
            this.functionsElement = functionsElement;
            this.functionElement = functionElement;

            LocomotiveFunctionInfo function = new(functionElement);

            numericUpDownFunctionNumber.Value = function.Number;
            comboBoxFunctionName.Text = function.Name;
            textBoxFunctionDescription.Text = function.Description;
            comboBoxFunctionType.SelectedIndex = (int)function.Type;

            foreach (XmlElement f in catalog.LocomotiveFunctionNames)
                comboBoxFunctionName.Items.Add(f.GetAttribute(A_Name));

            typeChanged = false;
            descriptionChanged = false;
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

        private void ButtonOk_Click(object? sender, EventArgs e) {
            foreach (XmlElement fElement in functionsElement) {
                LocomotiveFunctionInfo f = new(fElement);

                if (fElement != functionElement && f.Number == numericUpDownFunctionNumber.Value) {
                    MessageBox.Show(this, "Function number " + numericUpDownFunctionNumber.Value + " is already used", "Duplicate function number",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    numericUpDownFunctionNumber.Focus();
                    return;
                }

                if (fElement != functionElement && f.Name == comboBoxFunctionName.Text) {
                    MessageBox.Show(this, "Function name '" + comboBoxFunctionName.Text + "' is already used", "Duplicate function name",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    comboBoxFunctionName.Focus();
                    return;
                }
            }

            LocomotiveFunctionInfo _ = new(functionElement) {
                Number = (int)numericUpDownFunctionNumber.Value,
                Type = (LocomotiveFunctionType)comboBoxFunctionType.SelectedIndex,
                Name = comboBoxFunctionName.Text,
                Description = textBoxFunctionDescription.Text
            };

            if (comboBoxFunctionName.FindStringExact(comboBoxFunctionName.Text) < 0) {
                XmlElement element = catalog.Element.OwnerDocument.CreateElement("Function");

                element.SetAttribute(A_Name, comboBoxFunctionName.Text);
                element.SetAttribute(A_Description, textBoxFunctionDescription.Text);
                element.SetAttributeValue(A_Type, (LocomotiveFunctionType)comboBoxFunctionType.SelectedIndex);
                catalog.LocomotiveFunctionNames.AppendChild(element);
            }

            DialogResult = DialogResult.OK;
        }

        private void ComboBoxFunctionType_SelectedIndexChanged(object? sender, EventArgs e) {
            typeChanged = true;
        }

        private void TextBoxFunctionDescription_TextChanged(object? sender, EventArgs e) {
            descriptionChanged = true;
        }

        private void ComboBoxFunctionName_SelectionChangeCommitted(object? sender, EventArgs e) {
            var functionInfoElement = new XmlElementWrapper((XmlElement?)catalog.LocomotiveFunctionNames.GetElementsByTagName("Function")[comboBoxFunctionName.SelectedIndex]);

            if (!typeChanged) {
                comboBoxFunctionType.SelectedIndex = (int)(functionInfoElement.AttributeValue(A_Type).Enum<LocomotiveFunctionType>() ?? LocomotiveFunctionType.OnOff);
                typeChanged = false;
            }

            if (!descriptionChanged) {
                textBoxFunctionDescription.Text = (string?)functionInfoElement.AttributeValue(A_Description) ?? String.Empty;
                descriptionChanged = false;
            }
        }
    }
}
