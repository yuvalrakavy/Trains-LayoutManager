using System.Xml;

namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    public interface IIfConditionDialogCustomizer {
        string[] OperatorNames { get; }
        string[] OperatorDescriptions { get; }
        Type[] AllowedTypes { get; }
        bool ValueIsBoolean { get; }

        string Title { get; }
    }

    /// <summary>
    /// Summary description for IfCondition.
    /// </summary>
    public partial class IfCondition : Form {
        private readonly IIfConditionDialogCustomizer customizer;

        public IfCondition(XmlElement conditionElement, IIfConditionDialogCustomizer customizer) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.customizer = customizer;
            Text = customizer.Title;

            operand1.Element = conditionElement;
            operand1.Suffix = "1";
            operand1.DefaultAccess = "Property";
            operand1.AllowedTypes = customizer.AllowedTypes;
            operand1.ValueIsBoolean = customizer.ValueIsBoolean;

            operand2.Element = conditionElement;
            operand2.Suffix = "2";
            operand2.DefaultAccess = "Value";
            operand2.AllowedTypes = customizer.AllowedTypes;
            operand2.ValueIsBoolean = customizer.ValueIsBoolean;

            operand1.Initialize();
            operand2.Initialize();

            foreach (string operatorDescription in customizer.OperatorDescriptions)
                comboBoxOperation.Items.Add(operatorDescription);
            comboBoxOperation.SelectedIndex = 0;
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
            if (comboBoxOperation.SelectedIndex < 0) {
                MessageBox.Show(this, "You must selection operation", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxOperation.Focus();
                return;
            }

            if (!operand1.ValidateInput() || !operand2.ValidateInput())
                return;

            operand1.Commit();
            operand2.Commit();

            operand1.Element.SetAttribute("Operation", customizer.OperatorNames[comboBoxOperation.SelectedIndex]);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object? sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
