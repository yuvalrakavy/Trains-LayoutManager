﻿namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for GenerateEventOption.
    /// </summary>
    partial class GenerateEventOption : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new Label();
            this.textBoxName = new TextBox();
            this.groupBox1 = new GroupBox();
            this.operand = new LayoutManager.CommonUI.Controls.Operand();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Option name:";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(80, 8);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(136, 20);
            this.textBoxName.TabIndex = 1;
            this.textBoxName.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.operand);
            this.groupBox1.Location = new System.Drawing.Point(8, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(208, 112);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Option value:";
            // 
            // operand
            // 
            this.operand.AllowedTypes = null;
            this.operand.DefaultAccess = "Property";
            this.operand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.operand.Element = null;
            this.operand.Location = new System.Drawing.Point(3, 16);
            this.operand.Name = "operand";
            this.operand.Size = new System.Drawing.Size(202, 93);
            this.operand.Suffix = "";
            this.operand.TabIndex = 0;
            this.operand.ValueIsBoolean = false;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(62, 162);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(142, 162);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            // 
            // GenerateEventOption
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(226, 190);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "GenerateEventOption";
            this.ShowInTaskbar = false;
            this.Text = "Generate Event Option";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private Label label1;
        private TextBox textBoxName;
        private GroupBox groupBox1;
        private Button buttonOK;
        private Button buttonCancel;
        private LayoutManager.CommonUI.Controls.Operand operand;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
    }
}