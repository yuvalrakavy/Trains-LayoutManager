using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;

namespace LayoutManager.Tools.EventScriptDialogs {
    /// <summary>
    /// Summary description for TrainFunctionAction.
    /// </summary>
    public class TrainFunctionAction : Form {
        private Label label1;
        private Label label2;
        private ComboBox comboBoxTrain;
        private ComboBox comboBoxFunctionName;
        private GroupBox groupBoxFunctionState;
        private Button buttonOK;
        private Button buttonCancel;
        private RadioButton radioButtonToggle;
        private RadioButton radioButtonOn;
        private RadioButton radioButtonOff;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private void endOfDesignerVariables() { }

        private readonly XmlElement element;

        public TrainFunctionAction(XmlElement element) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.element = element;

            XmlElement commonFunctions = LayoutModel.LocomotiveCatalog.LocomotiveFunctionNames;
            string functionType = element.Name == "TriggerTrainFunction" ? "Trigger" : "OnOff";
            XmlNodeList relevantFunctionElements = commonFunctions.SelectNodes("Function[@Type='" + functionType + "']");

            foreach (XmlElement functionElement in relevantFunctionElements)
                comboBoxFunctionName.Items.Add(functionElement.GetAttribute("Name"));

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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new Label();
            this.comboBoxTrain = new ComboBox();
            this.label2 = new Label();
            this.comboBoxFunctionName = new ComboBox();
            this.groupBoxFunctionState = new GroupBox();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.radioButtonOn = new RadioButton();
            this.radioButtonOff = new RadioButton();
            this.radioButtonToggle = new RadioButton();
            this.groupBoxFunctionState.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Train defined by symbol: ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxTrain
            // 
            this.comboBoxTrain.Items.AddRange(new object[] {
                                                               "Train",
                                                               "Script:Train"});
            this.comboBoxTrain.Location = new System.Drawing.Point(144, 14);
            this.comboBoxTrain.Name = "comboBoxTrain";
            this.comboBoxTrain.Size = new System.Drawing.Size(160, 21);
            this.comboBoxTrain.TabIndex = 1;
            this.comboBoxTrain.Text = "Train";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Function name:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxFunctionName
            // 
            this.comboBoxFunctionName.Location = new System.Drawing.Point(144, 40);
            this.comboBoxFunctionName.Name = "comboBoxFunctionName";
            this.comboBoxFunctionName.Size = new System.Drawing.Size(160, 21);
            this.comboBoxFunctionName.TabIndex = 3;
            // 
            // groupBoxFunctionState
            // 
            this.groupBoxFunctionState.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                                this.radioButtonOn,
                                                                                                this.radioButtonOff,
                                                                                                this.radioButtonToggle});
            this.groupBoxFunctionState.Location = new System.Drawing.Point(8, 68);
            this.groupBoxFunctionState.Name = "groupBoxFunctionState";
            this.groupBoxFunctionState.Size = new System.Drawing.Size(120, 67);
            this.groupBoxFunctionState.TabIndex = 4;
            this.groupBoxFunctionState.TabStop = false;
            this.groupBoxFunctionState.Text = "Function State:";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(232, 84);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(232, 112);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // radioButtonOn
            // 
            this.radioButtonOn.Location = new System.Drawing.Point(8, 14);
            this.radioButtonOn.Name = "radioButtonOn";
            this.radioButtonOn.Size = new System.Drawing.Size(104, 16);
            this.radioButtonOn.TabIndex = 0;
            this.radioButtonOn.Text = "On";
            // 
            // radioButtonOff
            // 
            this.radioButtonOff.Location = new System.Drawing.Point(8, 31);
            this.radioButtonOff.Name = "radioButtonOff";
            this.radioButtonOff.Size = new System.Drawing.Size(104, 16);
            this.radioButtonOff.TabIndex = 1;
            this.radioButtonOff.Text = "Off";
            // 
            // radioButtonToggle
            // 
            this.radioButtonToggle.Location = new System.Drawing.Point(8, 47);
            this.radioButtonToggle.Name = "radioButtonToggle";
            this.radioButtonToggle.Size = new System.Drawing.Size(104, 16);
            this.radioButtonToggle.TabIndex = 2;
            this.radioButtonToggle.Text = "Toggle";
            // 
            // TrainFunctionAction
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(312, 142);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonOK,
                                                                          this.groupBoxFunctionState,
                                                                          this.comboBoxTrain,
                                                                          this.label1,
                                                                          this.label2,
                                                                          this.comboBoxFunctionName,
                                                                          this.buttonCancel});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "TrainFunctionAction";
            this.ShowInTaskbar = false;
            this.Text = "Train Function";
            this.groupBoxFunctionState.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonOK_Click(object sender, System.EventArgs e) {
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
                string action = "";

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

        private void buttonCancel_Click(object sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
