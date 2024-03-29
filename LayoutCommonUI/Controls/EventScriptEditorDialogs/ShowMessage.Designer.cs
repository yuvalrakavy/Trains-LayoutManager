﻿namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for ShowMessage.
    /// </summary>
    partial class ShowMessage : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new Label();
            this.textBoxMessage = new TextBox();
            this.groupBox1 = new GroupBox();
            this.radioButtonMessage = new RadioButton();
            this.radioButtonWanrning = new RadioButton();
            this.radioButtonError = new RadioButton();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.label2 = new Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Text: ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Location = new System.Drawing.Point(48, 17);
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.Size = new System.Drawing.Size(320, 20);
            this.textBoxMessage.TabIndex = 1;
            this.textBoxMessage.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonMessage);
            this.groupBox1.Controls.Add(this.radioButtonWanrning);
            this.groupBox1.Controls.Add(this.radioButtonError);
            this.groupBox1.Location = new System.Drawing.Point(48, 96);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(112, 80);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Message Type:";
            // 
            // radioButtonMessage
            // 
            this.radioButtonMessage.Location = new System.Drawing.Point(8, 16);
            this.radioButtonMessage.Name = "radioButtonMessage";
            this.radioButtonMessage.Size = new System.Drawing.Size(90, 19);
            this.radioButtonMessage.TabIndex = 0;
            this.radioButtonMessage.Text = "Message";
            // 
            // radioButtonWanrning
            // 
            this.radioButtonWanrning.Location = new System.Drawing.Point(8, 36);
            this.radioButtonWanrning.Name = "radioButtonWanrning";
            this.radioButtonWanrning.Size = new System.Drawing.Size(90, 19);
            this.radioButtonWanrning.TabIndex = 1;
            this.radioButtonWanrning.Text = "Warning";
            // 
            // radioButtonError
            // 
            this.radioButtonError.Location = new System.Drawing.Point(8, 56);
            this.radioButtonError.Name = "radioButtonError";
            this.radioButtonError.Size = new System.Drawing.Size(90, 19);
            this.radioButtonError.TabIndex = 2;
            this.radioButtonError.Text = "Error";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(293, 123);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(293, 153);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.Info;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.ForeColor = System.Drawing.SystemColors.InfoText;
            this.label2.Location = new System.Drawing.Point(48, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(320, 48);
            this.label2.TabIndex = 5;
            this.label2.Text = "Note: You can use [Symbol.Property] to show value of a property or <Symbol.Attrib" +
                "ute> to show value of an attribute. For example: [Train.Name] or <Train.MyCustom" +
                "Attribute>.";
            // 
            // ShowMessage
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(374, 184);
            this.ControlBox = false;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ShowMessage";
            this.ShowInTaskbar = false;
            this.Text = "Show Message";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion
        private Label label1;
        private TextBox textBoxMessage;
        private GroupBox groupBox1;
        private RadioButton radioButtonMessage;
        private RadioButton radioButtonWanrning;
        private Button buttonOK;
        private Button buttonCancel;
        private RadioButton radioButtonError;
        private Label label2;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
    }
}

