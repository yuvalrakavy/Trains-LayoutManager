using System;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Tools.EventScriptDialogs {
    /// <summary>
    /// Summary description for TrainTargetSpeedAction.
    /// </summary>
    public partial class TrainLightsAction : Form {
        private readonly XmlElement element;

        public TrainLightsAction(XmlElement element) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.element = element;

            if (element.HasAttribute("TrainSymbol"))
                comboBoxTrain.Text = element.GetAttribute("TrainSymbol");

            operandValue.Suffix = "Value";
            operandValue.Element = element;
            operandValue.DefaultAccess = "Value";
            operandValue.ValueIsBoolean = true;
            operandValue.ValueIsOnOff = true;
            operandValue.AllowedTypes = new Type[] { typeof(bool) };
            operandValue.Initialize();

            if (element.HasAttribute("Action")) {
                switch (element.GetAttribute("Action")) {
                    case "Set": radioButtonSet.Checked = true; break;
                    case "Toggle": radioButtonToggle.Checked = true; break;
                }
            }
            else
                radioButtonSet.Checked = true;
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
            if (comboBoxTrain.Text.Trim() == "") {
                MessageBox.Show(this, "The name of symbol indicating the train cannot be empty. It most probably need to be set to 'Train'",
                    "Missing Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxTrain.Focus();
                return;
            }

            if (!operandValue.Commit())
                return;

            element.SetAttribute("TrainSymbol", comboBoxTrain.Text);

            string action = "Set";

            if (radioButtonToggle.Checked)
                action = "Toggle";

            element.SetAttribute("Action", action);
            DialogResult = DialogResult.OK;
        }

        private void ButtonCancel_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
