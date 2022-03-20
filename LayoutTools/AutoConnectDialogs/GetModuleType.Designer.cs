using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.AutoConnectDialogs {
    /// <summary>
    /// Summary description for GetModuleType.
    /// </summary>
    partial class GetModuleType : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new Label();
            this.comboBoxModuleTypes = new ComboBox();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.checkBoxUseAsDefault = new CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(264, 48);
            this.label1.TabIndex = 0;
            this.label1.Text = "More than one module type can be used to control this component. Please select th" +
                "e control module type you would like to add:";
            // 
            // comboBoxModuleTypes
            // 
            this.comboBoxModuleTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxModuleTypes.Location = new System.Drawing.Point(16, 56);
            this.comboBoxModuleTypes.Name = "comboBoxModuleTypes";
            this.comboBoxModuleTypes.Size = new System.Drawing.Size(252, 21);
            this.comboBoxModuleTypes.Sorted = true;
            this.comboBoxModuleTypes.TabIndex = 1;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(113, 116);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "Continue";
            this.buttonOK.Click += new EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(193, 116);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            // 
            // checkBoxUseAsDefault
            // 
            this.checkBoxUseAsDefault.AutoSize = true;
            this.checkBoxUseAsDefault.Location = new System.Drawing.Point(19, 83);
            this.checkBoxUseAsDefault.Name = "checkBoxUseAsDefault";
            this.checkBoxUseAsDefault.Size = new System.Drawing.Size(150, 17);
            this.checkBoxUseAsDefault.TabIndex = 4;
            this.checkBoxUseAsDefault.Text = "Use this module as default";
            this.checkBoxUseAsDefault.UseVisualStyleBackColor = true;
            // 
            // GetModuleType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AcceptButton = this.buttonOK;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(290, 151);
            this.ControlBox = false;
            this.Controls.Add(this.checkBoxUseAsDefault);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.comboBoxModuleTypes);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "GetModuleType";
            this.ShowInTaskbar = false;
            this.Text = "Select Control Module Type to Add";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        private Label label1;
        private Button buttonOK;
        private Button buttonCancel;
        private ComboBox comboBoxModuleTypes;
        private CheckBox checkBoxUseAsDefault;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

    }
}

