﻿
using System;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    partial class BusDefaultProperties {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label2 = new Label();
            this.comboBoxModuleType = new ComboBox();
            this.label3 = new Label();
            this.label4 = new Label();
            this.labelStartAddress1 = new Label();
            this.textBoxStartAddress = new TextBox();
            this.labelStartAddress2 = new Label();
            this.labelStartAddress3 = new Label();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 23);
            this.label2.TabIndex = 4;
            this.label2.Text = "Add module of type:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxModuleType
            // 
            this.comboBoxModuleType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxModuleType.Location = new System.Drawing.Point(118, 50);
            this.comboBoxModuleType.Name = "comboBoxModuleType";
            this.comboBoxModuleType.Size = new System.Drawing.Size(180, 21);
            this.comboBoxModuleType.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(300, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 23);
            this.label3.TabIndex = 6;
            this.label3.Text = "When new";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(312, 23);
            this.label4.TabIndex = 7;
            this.label4.Text = "modules need to added for performing automatic connection";
            // 
            // labelStartAddress1
            // 
            this.labelStartAddress1.Location = new System.Drawing.Point(6, 3);
            this.labelStartAddress1.Name = "labelStartAddress1";
            this.labelStartAddress1.Size = new System.Drawing.Size(152, 23);
            this.labelStartAddress1.TabIndex = 0;
            this.labelStartAddress1.Text = "Assign address starting from:";
            this.labelStartAddress1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxStartAddress
            // 
            this.textBoxStartAddress.Location = new System.Drawing.Point(166, 4);
            this.textBoxStartAddress.Name = "textBoxStartAddress";
            this.textBoxStartAddress.Size = new System.Drawing.Size(56, 20);
            this.textBoxStartAddress.TabIndex = 1;
            // 
            // labelStartAddress2
            // 
            this.labelStartAddress2.Location = new System.Drawing.Point(238, 3);
            this.labelStartAddress2.Name = "labelStartAddress2";
            this.labelStartAddress2.Size = new System.Drawing.Size(141, 23);
            this.labelStartAddress2.TabIndex = 2;
            this.labelStartAddress2.Text = "to modules  added in this ";
            this.labelStartAddress2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelStartAddress3
            // 
            this.labelStartAddress3.Location = new System.Drawing.Point(4, 25);
            this.labelStartAddress3.Name = "labelStartAddress3";
            this.labelStartAddress3.Size = new System.Drawing.Size(272, 16);
            this.labelStartAddress3.TabIndex = 3;
            this.labelStartAddress3.Text = "module location";
            this.labelStartAddress3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(210, 106);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 8;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(290, 106);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            // 
            // BusDefaultProperties
            // 
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoSize = true;
            this.AcceptButton = this.buttonOK;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(373, 135);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.labelStartAddress3);
            this.Controls.Add(this.labelStartAddress2);
            this.Controls.Add(this.textBoxStartAddress);
            this.Controls.Add(this.labelStartAddress1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxModuleType);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "BusDefaultProperties";
            this.ShowInTaskbar = false;
            this.Text = "Adding Modules Defaults";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion
        private Label label2;
        private ComboBox comboBoxModuleType;
        private Label label3;
        private Label label4;
        private TextBox textBoxStartAddress;
        private Button buttonOK;
        private Button buttonCancel;
        private Label labelStartAddress1;
        private Label labelStartAddress2;
        private Label labelStartAddress3;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
    }
}
