using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Tools.EventScriptDialogs {
    /// <summary>
    /// Summary description for TrainTargetSpeedAction.
    /// </summary>
    public class TrainTargetSpeedAction : Form {
        private ComboBox comboBoxTrain;
        private Label label1;
        private GroupBox groupBox1;
        private LayoutManager.CommonUI.Controls.Operand operandValue;
        private GroupBox groupBox2;
        private RadioButton radioButtonSet;
        private RadioButton radioButtonDecrease;
        private Button buttonOK;
        private Button buttonCancel;
        private RadioButton radioButtonIncrease;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private void endOfDesignerVariables() { }

        private readonly XmlElement element;

        public TrainTargetSpeedAction(XmlElement element) {
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
            operandValue.AllowedTypes = new Type[] { typeof(int) };
            operandValue.Initialize();

            if (element.HasAttribute("Action")) {
                switch (element.GetAttribute("Action")) {
                    case "Set": radioButtonSet.Checked = true; break;
                    case "Increase": radioButtonIncrease.Checked = true; break;
                    case "Decrease": radioButtonDecrease.Checked = true; break;
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.comboBoxTrain = new ComboBox();
            this.label1 = new Label();
            this.groupBox1 = new GroupBox();
            this.operandValue = new LayoutManager.CommonUI.Controls.Operand();
            this.groupBox2 = new GroupBox();
            this.radioButtonSet = new RadioButton();
            this.radioButtonIncrease = new RadioButton();
            this.radioButtonDecrease = new RadioButton();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxTrain
            // 
            this.comboBoxTrain.Items.AddRange(new object[] {
                                                               "Train",
                                                               "Script:Train"});
            this.comboBoxTrain.Location = new System.Drawing.Point(134, 8);
            this.comboBoxTrain.Name = "comboBoxTrain";
            this.comboBoxTrain.Size = new System.Drawing.Size(160, 21);
            this.comboBoxTrain.TabIndex = 1;
            this.comboBoxTrain.Text = "Train";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(-2, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Train defined by symbol: ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                    this.operandValue});
            this.groupBox1.Location = new System.Drawing.Point(9, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(216, 120);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Value:";
            // 
            // operandValue
            // 
            this.operandValue.AllowedTypes = null;
            this.operandValue.DefaultAccess = "Property";
            this.operandValue.Element = null;
            this.operandValue.Location = new System.Drawing.Point(8, 16);
            this.operandValue.Name = "operandValue";
            this.operandValue.Size = new System.Drawing.Size(200, 96);
            this.operandValue.Suffix = "";
            this.operandValue.TabIndex = 0;
            this.operandValue.ValueIsBoolean = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                    this.radioButtonSet,
                                                                                    this.radioButtonIncrease,
                                                                                    this.radioButtonDecrease});
            this.groupBox2.Location = new System.Drawing.Point(233, 40);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(136, 120);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Change Target Speed:";
            // 
            // radioButtonSet
            // 
            this.radioButtonSet.Location = new System.Drawing.Point(8, 16);
            this.radioButtonSet.Name = "radioButtonSet";
            this.radioButtonSet.TabIndex = 0;
            this.radioButtonSet.Text = "Set to value";
            // 
            // radioButtonIncrease
            // 
            this.radioButtonIncrease.Location = new System.Drawing.Point(8, 40);
            this.radioButtonIncrease.Name = "radioButtonIncrease";
            this.radioButtonIncrease.Size = new System.Drawing.Size(112, 24);
            this.radioButtonIncrease.TabIndex = 1;
            this.radioButtonIncrease.Text = "Increase by value";
            // 
            // radioButtonDecrease
            // 
            this.radioButtonDecrease.Location = new System.Drawing.Point(8, 64);
            this.radioButtonDecrease.Name = "radioButtonDecrease";
            this.radioButtonDecrease.Size = new System.Drawing.Size(120, 24);
            this.radioButtonDecrease.TabIndex = 2;
            this.radioButtonDecrease.Text = "Decrease by value";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(213, 168);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += this.buttonOK_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(295, 168);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += this.buttonCancel_Click;
            // 
            // TrainTargetSpeedAction
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(376, 198);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonOK,
                                                                          this.groupBox2,
                                                                          this.groupBox1,
                                                                          this.comboBoxTrain,
                                                                          this.label1,
                                                                          this.buttonCancel});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "TrainTargetSpeedAction";
            this.ShowInTaskbar = false;
            this.Text = "Change Train Target Speed";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
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

            if (!operandValue.Commit())
                return;

            element.SetAttribute("TrainSymbol", comboBoxTrain.Text);

            string action = "Set";

            if (radioButtonDecrease.Checked)
                action = "Decrease";
            else if (radioButtonIncrease.Checked)
                action = "Increase";

            element.SetAttribute("Action", action);
            DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
