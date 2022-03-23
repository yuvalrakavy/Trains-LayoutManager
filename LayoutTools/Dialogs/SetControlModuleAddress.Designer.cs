using System;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for SetControlModuleAddress.
    /// </summary>
    partial class SetControlModuleAddress : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new Label();
            this.comboBoxAddress = new ComboBox();
            this.textBoxAddress = new TextBox();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.checkBoxSetUserActionRequired = new CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Set module to: ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxAddress
            // 
            this.comboBoxAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAddress.Location = new System.Drawing.Point(96, 17);
            this.comboBoxAddress.Name = "comboBoxAddress";
            this.comboBoxAddress.Size = new System.Drawing.Size(112, 21);
            this.comboBoxAddress.TabIndex = 1;
            // 
            // textBoxAddress
            // 
            this.textBoxAddress.Location = new System.Drawing.Point(128, 48);
            this.textBoxAddress.Name = "textBoxAddress";
            this.textBoxAddress.TabIndex = 2;
            this.textBoxAddress.Text = "";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(232, 16);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(232, 48);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            // 
            // checkBoxSetUserActionRequired
            // 
            this.checkBoxSetUserActionRequired.Checked = true;
            this.checkBoxSetUserActionRequired.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSetUserActionRequired.Location = new System.Drawing.Point(8, 48);
            this.checkBoxSetUserActionRequired.Name = "checkBoxSetUserActionRequired";
            this.checkBoxSetUserActionRequired.Size = new System.Drawing.Size(192, 24);
            this.checkBoxSetUserActionRequired.TabIndex = 5;
            this.checkBoxSetUserActionRequired.Text = "Set \"User Action Required\" flag";
            // 
            // SetControlModuleAddress
            // 
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoSize = true;
            this.AcceptButton = this.buttonOK;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(312, 78);
            this.ControlBox = false;
            this.Controls.Add(this.checkBoxSetUserActionRequired);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBoxAddress);
            this.Controls.Add(this.comboBoxAddress);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SetControlModuleAddress";
            this.ShowInTaskbar = false;
            this.Text = "SetControlModuleAddress";
            this.ResumeLayout(false);
        }
        #endregion
        private Label label1;
        private TextBox textBoxAddress;
        private Button buttonOK;
        private Button buttonCancel;
        private ComboBox comboBoxAddress;
        private CheckBox checkBoxSetUserActionRequired;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}
