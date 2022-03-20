using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Tools.EventScriptDialogs {
    /// <summary>
    /// Summary description for TrainTargetSpeedAction.
    /// </summary>
    partial class TrainLightsAction : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.comboBoxTrain = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.operandValue = new LayoutManager.CommonUI.Controls.Operand();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.radioButtonSet = new System.Windows.Forms.RadioButton();
            this.radioButtonToggle = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
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
            this.groupBox1.Controls.Add(this.operandValue);
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
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(213, 168);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(295, 168);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new EventHandler(this.ButtonCancel_Click);
            // 
            // radioButtonSet
            // 
            this.radioButtonSet.Location = new System.Drawing.Point(242, 77);
            this.radioButtonSet.Name = "radioButtonSet";
            this.radioButtonSet.Size = new System.Drawing.Size(104, 24);
            this.radioButtonSet.TabIndex = 6;
            this.radioButtonSet.Text = "Set to value";
            // 
            // radioButtonToggle
            // 
            this.radioButtonToggle.Location = new System.Drawing.Point(242, 101);
            this.radioButtonToggle.Name = "radioButtonToggle";
            this.radioButtonToggle.Size = new System.Drawing.Size(112, 24);
            this.radioButtonToggle.TabIndex = 7;
            this.radioButtonToggle.Text = "Toggle value";
            // 
            // TrainLightsAction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AcceptButton = this.buttonOK;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(376, 198);
            this.ControlBox = false;
            this.Controls.Add(this.radioButtonSet);
            this.Controls.Add(this.radioButtonToggle);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.comboBoxTrain);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "TrainLightsAction";
            this.ShowInTaskbar = false;
            this.Text = "Set Train Lights";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private ComboBox comboBoxTrain;
        private Label label1;
        private GroupBox groupBox1;
        private LayoutManager.CommonUI.Controls.Operand operandValue;
        private Button buttonOK;
        private Button buttonCancel;
        private RadioButton radioButtonSet;
        private RadioButton radioButtonToggle;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}

