using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveFunctionsCopyFrom.
    /// </summary>
    partial class LocomotiveFunctionsCopyFrom : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new Label();
            this.xmlQueryComboboxCopyFrom = new CommonUI.Controls.XmlQueryCombobox();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Copy locomotive functions from: ";
            // 
            // xmlQueryComboboxCopyFrom
            // 
            this.xmlQueryComboboxCopyFrom.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.xmlQueryComboboxCopyFrom.ContainerElement = null;
            this.xmlQueryComboboxCopyFrom.Extract = "string(Name)";
            this.xmlQueryComboboxCopyFrom.Location = new System.Drawing.Point(16, 24);
            this.xmlQueryComboboxCopyFrom.Name = "xmlQueryComboboxCopyFrom";
            this.xmlQueryComboboxCopyFrom.Query = "*[contains(Name, \'<TEXT>\')]";
            this.xmlQueryComboboxCopyFrom.Size = new System.Drawing.Size(184, 21);
            this.xmlQueryComboboxCopyFrom.TabIndex = 1;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonOK.Location = new System.Drawing.Point(208, 8);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(208, 40);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            // 
            // LocomotiveFunctionsCopyFrom
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(292, 70);
            this.ControlBox = false;
            this.Controls.AddRange(new Control[] {
                                                                          this.buttonOK,
                                                                          this.xmlQueryComboboxCopyFrom,
                                                                          this.label1,
                                                                          this.buttonCancel});
            this.Name = "LocomotiveFunctionsCopyFrom";
            this.ShowInTaskbar = false;
            this.Text = "Copy functions from";
            this.ResumeLayout(false);
        }
        #endregion

        private Label label1;
        private Button buttonOK;
        private Button buttonCancel;
        private CommonUI.Controls.XmlQueryCombobox xmlQueryComboboxCopyFrom;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}

