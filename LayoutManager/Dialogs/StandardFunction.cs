using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Dialog box for creating/editing locomotive function template
    /// </summary>
    public partial class StandardFunction : Form {
        private const string A_Type = "Type";
        private const string A_Name = "Name";
        private const string A_Description = "Description";
        private readonly XmlElementWrapper functionInfoElement;

        public StandardFunction(XmlElement rawFunctionInfoElement) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.functionInfoElement = new XmlElementWrapper(rawFunctionInfoElement);

            comboBoxFunctionType.SelectedIndex = (int)(functionInfoElement.AttributeValue(A_Type).Enum<LocomotiveFunctionType>() ?? LocomotiveFunctionType.Trigger);
            textBoxFunctionName.Text = (string?)functionInfoElement.AttributeValue(A_Name) ?? String.Empty;
            textBoxDescription.Text = (string?)functionInfoElement.AttributeValue(A_Description) ?? String.Empty;
        }

        private void ButtonOk_Click(object? sender, EventArgs e) {
            if (textBoxFunctionName.Text.Trim()?.Length == 0) {
                MessageBox.Show(this, "Missing value", "You have to specifiy function name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxFunctionName.Focus();
                return;
            }

            functionInfoElement.SetAttributeValue(A_Type, (LocomotiveFunctionType)comboBoxFunctionType.SelectedIndex);
            functionInfoElement.SetAttributeValue(A_Name, textBoxFunctionName.Text);
            functionInfoElement.SetAttributeValue(A_Description, textBoxDescription.Text);

            DialogResult = DialogResult.OK;
        }
    }
}
