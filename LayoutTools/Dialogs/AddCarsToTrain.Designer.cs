using System;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for AddCarsToTrain.
    /// </summary>
    partial class AddCarsToTrain : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.comboBoxDescription = new ComboBox();
            this.label1 = new Label();
            this.label2 = new Label();
            this.numericUpDownCount = new NumericUpDown();
            this.groupBoxLength = new GroupBox();
            this.lengthInput = new LayoutManager.CommonUI.Controls.LengthInput();
            this.radioButtonLength_65 = new RadioButton();
            this.radioButtonLength_76 = new RadioButton();
            this.radioButtonLength_46 = new RadioButton();
            this.radioButtonLength_41 = new RadioButton();
            this.radioButtonLength_30 = new RadioButton();
            this.radioButtonOther = new RadioButton();
            this.ButtonOK = new Button();
            this.buttonCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownCount).BeginInit();
            this.groupBoxLength.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxDescription
            // 
            this.comboBoxDescription.Location = new System.Drawing.Point(104, 16);
            this.comboBoxDescription.Name = "comboBoxDescription";
            this.comboBoxDescription.Size = new System.Drawing.Size(208, 21);
            this.comboBoxDescription.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(32, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Description:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Number of cars:";
            // 
            // numericUpDownCount
            // 
            this.numericUpDownCount.Location = new System.Drawing.Point(104, 45);
            this.numericUpDownCount.Minimum = new System.Decimal(new int[] {
                                                                               1,
                                                                               0,
                                                                               0,
                                                                               0});
            this.numericUpDownCount.Name = "numericUpDownCount";
            this.numericUpDownCount.Size = new System.Drawing.Size(56, 20);
            this.numericUpDownCount.TabIndex = 3;
            this.numericUpDownCount.Value = new System.Decimal(new int[] {
                                                                             1,
                                                                             0,
                                                                             0,
                                                                             0});
            // 
            // groupBoxLength
            // 
            this.groupBoxLength.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                         this.lengthInput,
                                                                                         this.radioButtonLength_65,
                                                                                         this.radioButtonLength_76,
                                                                                         this.radioButtonLength_46,
                                                                                         this.radioButtonLength_41,
                                                                                         this.radioButtonLength_30,
                                                                                         this.radioButtonOther});
            this.groupBoxLength.Location = new System.Drawing.Point(16, 80);
            this.groupBoxLength.Name = "groupBoxLength";
            this.groupBoxLength.Size = new System.Drawing.Size(184, 168);
            this.groupBoxLength.TabIndex = 4;
            this.groupBoxLength.TabStop = false;
            this.groupBoxLength.Text = "Car length:";
            // 
            // lengthInput
            // 
            this.lengthInput.IsEmpty = false;
            this.lengthInput.Location = new System.Drawing.Point(72, 136);
            this.lengthInput.Name = "lengthInput";
            this.lengthInput.NeutralValue = 0;
            this.lengthInput.ReadOnly = false;
            this.lengthInput.Size = new System.Drawing.Size(104, 24);
            this.lengthInput.TabIndex = 6;
            this.lengthInput.UnitValue = 0;
            this.lengthInput.Enter += this.LengthInput_Enter;
            // 
            // radioButtonLength_65
            // 
            this.radioButtonLength_65.Location = new System.Drawing.Point(16, 40);
            this.radioButtonLength_65.Name = "radioButtonLength_65";
            this.radioButtonLength_65.TabIndex = 1;
            this.radioButtonLength_65.Text = "Long (65cm)";
            // 
            // radioButtonLength_76
            // 
            this.radioButtonLength_76.Location = new System.Drawing.Point(16, 16);
            this.radioButtonLength_76.Name = "radioButtonLength_76";
            this.radioButtonLength_76.Size = new System.Drawing.Size(120, 24);
            this.radioButtonLength_76.TabIndex = 0;
            this.radioButtonLength_76.Text = "Very long (76cm)";
            // 
            // radioButtonLength_46
            // 
            this.radioButtonLength_46.Location = new System.Drawing.Point(16, 64);
            this.radioButtonLength_46.Name = "radioButtonLength_46";
            this.radioButtonLength_46.TabIndex = 2;
            this.radioButtonLength_46.Text = "Medium (46cm)";
            // 
            // radioButtonLength_41
            // 
            this.radioButtonLength_41.Location = new System.Drawing.Point(16, 88);
            this.radioButtonLength_41.Name = "radioButtonLength_41";
            this.radioButtonLength_41.TabIndex = 3;
            this.radioButtonLength_41.Text = "Short (41cm)";
            // 
            // radioButtonLength_30
            // 
            this.radioButtonLength_30.Location = new System.Drawing.Point(16, 112);
            this.radioButtonLength_30.Name = "radioButtonLength_30";
            this.radioButtonLength_30.Size = new System.Drawing.Size(136, 24);
            this.radioButtonLength_30.TabIndex = 4;
            this.radioButtonLength_30.Text = "Very short (30cm)";
            // 
            // radioButtonOther
            // 
            this.radioButtonOther.Location = new System.Drawing.Point(16, 136);
            this.radioButtonOther.Name = "radioButtonOther";
            this.radioButtonOther.Size = new System.Drawing.Size(56, 24);
            this.radioButtonOther.TabIndex = 5;
            this.radioButtonOther.Text = "Other: ";
            // 
            // ButtonOK
            // 
            this.ButtonOK.Location = new System.Drawing.Point(240, 192);
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.TabIndex = 5;
            this.ButtonOK.Text = "OK";
            this.ButtonOK.Click += this.ButtonOK_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(240, 224);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            // 
            // AddCarsToTrain
            // 
            this.AcceptButton = this.ButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(336, 266);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.ButtonOK,
                                                                          this.groupBoxLength,
                                                                          this.numericUpDownCount,
                                                                          this.label2,
                                                                          this.label1,
                                                                          this.comboBoxDescription,
                                                                          this.buttonCancel});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AddCarsToTrain";
            this.ShowInTaskbar = false;
            this.Text = "Train Cars";
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownCount).EndInit();
            this.groupBoxLength.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion
        private ComboBox comboBoxDescription;
        private Label label1;
        private Label label2;
        private NumericUpDown numericUpDownCount;
        private RadioButton radioButtonLength_76;
        private RadioButton radioButtonLength_65;
        private RadioButton radioButtonLength_46;
        private RadioButton radioButtonLength_41;
        private RadioButton radioButtonLength_30;
        private RadioButton radioButtonOther;
        private LayoutManager.CommonUI.Controls.LengthInput lengthInput;
        private Button ButtonOK;
        private Button buttonCancel;
        private GroupBox groupBoxLength;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}

