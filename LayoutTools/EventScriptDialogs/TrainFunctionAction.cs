using LayoutManager.Model;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Tools.EventScriptDialogs {
    /// <summary>
    /// Summary description for TrainFunctionAction.
    /// </summary>
    public partial class TrainFunctionAction : Form {
        private readonly XmlElement element;

        public TrainFunctionAction(XmlElement element) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.element = element;

            XmlElement commonFunctions = LayoutModel.LocomotiveCatalog.LocomotiveFunctionNames;
            string functionType = element.Name == "TriggerTrainFunction" ? "Trigger" : "OnOff";
            var relevantFunctionElements = commonFunctions.SelectNodes("Function[@Type='" + functionType + "']");

            if (relevantFunctionElements != null) {
                foreach (XmlElement functionElement in relevantFunctionElements)
                    comboBoxFunctionName.Items.Add(functionElement.GetAttribute("Name"));
            }

            if (element.HasAttribute("FunctionName"))
                comboBoxFunctionName.Text = element.GetAttribute("FunctionName");

            if (element.HasAttribute("TrainSymbol"))
                comboBoxTrain.Text = element.GetAttribute("TrainSymbol");

            if (element.Name == "TriggerTrainFunction")
                groupBoxFunctionState.Visible = false;
            else {
                if (element.HasAttribute("Action")) {
                    switch (element.GetAttribute("Action")) {
                        case "On":
                            radioButtonOn.Checked = true;
                            break;

                        case "Off":
                            radioButtonOff.Checked = true;
                            break;

                        case "Toggle":
                            radioButtonToggle.Checked = true;
                            break;
                    }
                }
                else
                    radioButtonToggle.Checked = true;
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

        private void ButtonOK_Click(object? sender, System.EventArgs e) {
            if (comboBoxTrain.Text.Trim() == "") {
                MessageBox.Show(this, "The name of symbol indicating the train cannot be empty. It most probably need to be set to 'Train'",
                    "Missing Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxTrain.Focus();
                return;
            }

            if (comboBoxFunctionName.Text.Trim() == "") {
                MessageBox.Show(this, "You have to specify a name of a train function",
                    "Missing Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxFunctionName.Focus();
                return;
            }

            element.SetAttribute("TrainSymbol", comboBoxTrain.Text);
            element.SetAttribute("FunctionName", comboBoxFunctionName.Text);

            if (groupBoxFunctionState.Visible) {
                string action;

                if (radioButtonOn.Checked)
                    action = "On";
                else if (radioButtonOff.Checked)
                    action = "Off";
                else
                    action = "Toggle";

                element.SetAttribute("Action", action);
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
