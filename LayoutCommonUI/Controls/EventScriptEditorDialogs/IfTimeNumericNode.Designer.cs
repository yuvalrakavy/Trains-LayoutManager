﻿namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for IfTimeNumericNode.
    /// </summary>
    partial class IfTimeNumericNode : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.radioButtonValue = new RadioButton();
            this.numericUpDownValue = new NumericUpDown();
            this.radioButtonRange = new RadioButton();
            this.numericUpDownFrom = new NumericUpDown();
            this.label1 = new Label();
            this.numericUpDownTo = new NumericUpDown();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownValue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownFrom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownTo).BeginInit();
            this.SuspendLayout();
            // 
            // radioButtonValue
            // 
            this.radioButtonValue.Location = new System.Drawing.Point(8, 16);
            this.radioButtonValue.Name = "radioButtonValue";
            this.radioButtonValue.Size = new System.Drawing.Size(56, 24);
            this.radioButtonValue.TabIndex = 0;
            this.radioButtonValue.Text = "Value:";
            // 
            // numericUpDownValue
            // 
            this.numericUpDownValue.Location = new System.Drawing.Point(64, 18);
            this.numericUpDownValue.Name = "numericUpDownValue";
            this.numericUpDownValue.Size = new System.Drawing.Size(48, 20);
            this.numericUpDownValue.TabIndex = 1;
            this.numericUpDownValue.KeyDown += this.NumericUpDownValue_KeyDown;
            this.numericUpDownValue.ValueChanged += this.NumericUpDownValue_ValueChanged;
            // 
            // radioButtonRange
            // 
            this.radioButtonRange.Location = new System.Drawing.Point(8, 48);
            this.radioButtonRange.Name = "radioButtonRange";
            this.radioButtonRange.Size = new System.Drawing.Size(56, 24);
            this.radioButtonRange.TabIndex = 2;
            this.radioButtonRange.Text = "From:";
            // 
            // numericUpDownFrom
            // 
            this.numericUpDownFrom.Location = new System.Drawing.Point(64, 50);
            this.numericUpDownFrom.Name = "numericUpDownFrom";
            this.numericUpDownFrom.Size = new System.Drawing.Size(48, 20);
            this.numericUpDownFrom.TabIndex = 3;
            this.numericUpDownFrom.KeyDown += this.NumericUpDownFromOrTo_KeyDown;
            this.numericUpDownFrom.ValueChanged += this.NumericUpDownFromOrTo_ValueChanged;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(117, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 23);
            this.label1.TabIndex = 4;
            this.label1.Text = "To:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numericUpDownTo
            // 
            this.numericUpDownTo.Location = new System.Drawing.Point(145, 50);
            this.numericUpDownTo.Name = "numericUpDownTo";
            this.numericUpDownTo.Size = new System.Drawing.Size(48, 20);
            this.numericUpDownTo.TabIndex = 5;
            this.numericUpDownTo.KeyDown += this.NumericUpDownFromOrTo_KeyDown;
            this.numericUpDownTo.ValueChanged += this.NumericUpDownFromOrTo_ValueChanged;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(58, 83);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += this.ButtonOK_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(138, 83);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += this.ButtonCancel_Click;
            // 
            // IfTimeNumericNode
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(216, 110);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.radioButtonRange);
            this.Controls.Add(this.numericUpDownValue);
            this.Controls.Add(this.radioButtonValue);
            this.Controls.Add(this.numericUpDownFrom);
            this.Controls.Add(this.numericUpDownTo);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "IfTimeNumericNode";
            this.ShowInTaskbar = false;
            this.Text = "IfTimeNumericNode";
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownValue).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownFrom).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownTo).EndInit();
            this.ResumeLayout(false);
        }
        #endregion

        private RadioButton radioButtonValue;
        private RadioButton radioButtonRange;
        private Label label1;
        private Button buttonOK;
        private Button buttonCancel;
        private NumericUpDown numericUpDownValue;
        private NumericUpDown numericUpDownFrom;
        private NumericUpDown numericUpDownTo;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
    }
}
