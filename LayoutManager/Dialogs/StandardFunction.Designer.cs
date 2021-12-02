using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Dialog box for creating/editing locomotive function template
    /// </summary>
    partial class StandardFunction : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new Label();
            this.comboBoxFunctionType = new ComboBox();
            this.label2 = new Label();
            this.textBoxFunctionName = new TextBox();
            this.label3 = new Label();
            this.textBoxDescription = new TextBox();
            this.buttonOk = new Button();
            this.buttonCancel = new Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(23, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Type:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxFunctionType
            // 
            this.comboBoxFunctionType.Items.AddRange(new object[] {
                                                                      "Trigger",
                                                                      "On/Off"});
            this.comboBoxFunctionType.Location = new System.Drawing.Point(80, 7);
            this.comboBoxFunctionType.Name = "comboBoxFunctionType";
            this.comboBoxFunctionType.Size = new System.Drawing.Size(78, 21);
            this.comboBoxFunctionType.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(23, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "Name:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxFunctionName
            // 
            this.textBoxFunctionName.Location = new System.Drawing.Point(80, 38);
            this.textBoxFunctionName.Name = "textBoxFunctionName";
            this.textBoxFunctionName.Size = new System.Drawing.Size(136, 20);
            this.textBoxFunctionName.TabIndex = 2;
            this.textBoxFunctionName.Text = "";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "Description:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Location = new System.Drawing.Point(80, 69);
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(168, 20);
            this.textBoxDescription.TabIndex = 2;
            this.textBoxDescription.Text = "";
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(224, 6);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(64, 23);
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += this.ButtonOk_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(224, 35);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(64, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            // 
            // StandardFunction
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(292, 96);
            this.ControlBox = false;
            this.Controls.AddRange(new Control[] {
                                                                          this.buttonOk,
                                                                          this.textBoxFunctionName,
                                                                          this.comboBoxFunctionType,
                                                                          this.label1,
                                                                          this.label2,
                                                                          this.label3,
                                                                          this.textBoxDescription,
                                                                          this.buttonCancel});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "StandardFunction";
            this.ShowInTaskbar = false;
            this.Text = "Locomotive Function Template";
            this.ResumeLayout(false);
        }
        #endregion

        private Label label1;
        private Label label2;
        private ComboBox comboBoxFunctionType;
        private TextBox textBoxFunctionName;
        private Label label3;
        private TextBox textBoxDescription;
        private Button buttonOk;
        private Button buttonCancel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
    }
}

