using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveFunction.
    /// </summary>
    partial class LocomotiveFunction : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new Label();
            this.numericUpDownFunctionNumber = new NumericUpDown();
            this.label2 = new Label();
            this.buttonOk = new Button();
            this.buttonCancel = new Button();
            this.comboBoxFunctionName = new ComboBox();
            this.label3 = new Label();
            this.comboBoxFunctionType = new ComboBox();
            this.label4 = new Label();
            this.textBoxFunctionDescription = new TextBox();
            ((ISupportInitialize)this.numericUpDownFunctionNumber).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Function number:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numericUpDownFunctionNumber
            // 
            this.numericUpDownFunctionNumber.Location = new System.Drawing.Point(112, 17);
            this.numericUpDownFunctionNumber.Name = "numericUpDownFunctionNumber";
            this.numericUpDownFunctionNumber.Size = new System.Drawing.Size(40, 20);
            this.numericUpDownFunctionNumber.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Name:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(112, 112);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 8;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += this.ButtonOk_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(192, 112);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            // 
            // comboBoxFunctionName
            // 
            this.comboBoxFunctionName.Location = new System.Drawing.Point(112, 48);
            this.comboBoxFunctionName.Name = "comboBoxFunctionName";
            this.comboBoxFunctionName.Size = new System.Drawing.Size(160, 21);
            this.comboBoxFunctionName.TabIndex = 5;
            this.comboBoxFunctionName.SelectionChangeCommitted += this.ComboBoxFunctionName_SelectionChangeCommitted;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(156, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Type:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxFunctionType
            // 
            this.comboBoxFunctionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFunctionType.Items.AddRange(new object[] {
            "Trigger",
            "On/Off"});
            this.comboBoxFunctionType.Location = new System.Drawing.Point(194, 16);
            this.comboBoxFunctionType.Name = "comboBoxFunctionType";
            this.comboBoxFunctionType.Size = new System.Drawing.Size(78, 21);
            this.comboBoxFunctionType.TabIndex = 3;
            this.comboBoxFunctionType.SelectedIndexChanged += this.ComboBoxFunctionType_SelectedIndexChanged;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 16);
            this.label4.TabIndex = 6;
            this.label4.Text = "Description:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxFunctionDescription
            // 
            this.textBoxFunctionDescription.Location = new System.Drawing.Point(80, 80);
            this.textBoxFunctionDescription.Name = "textBoxFunctionDescription";
            this.textBoxFunctionDescription.Size = new System.Drawing.Size(192, 20);
            this.textBoxFunctionDescription.TabIndex = 7;
            this.textBoxFunctionDescription.TextChanged += this.TextBoxFunctionDescription_TextChanged;
            // 
            // LocomotiveFunction
            // 
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AcceptButton = this.buttonOk;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(288, 144);
            this.ControlBox = false;
            this.Controls.Add(this.textBoxFunctionDescription);
            this.Controls.Add(this.comboBoxFunctionType);
            this.Controls.Add(this.comboBoxFunctionName);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.numericUpDownFunctionNumber);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "LocomotiveFunction";
            this.ShowInTaskbar = false;
            this.Text = "Locomotive Function";
            ((ISupportInitialize)this.numericUpDownFunctionNumber).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        private Label label1;
        private Label label2;
        private NumericUpDown numericUpDownFunctionNumber;
        private Button buttonOk;
        private Button buttonCancel;
        private ComboBox comboBoxFunctionName;
        private Label label3;
        private Label label4;
        private TextBox textBoxFunctionDescription;
        private ComboBox comboBoxFunctionType;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}

