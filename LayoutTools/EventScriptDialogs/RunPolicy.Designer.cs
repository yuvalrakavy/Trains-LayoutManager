using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;

namespace LayoutManager.Tools.EventScriptDialogs {
    /// <summary>
    /// Summary description for RunPolicy.
    /// </summary>
    partial class RunPolicy : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.comboBoxPolicy = new ComboBox();
            this.label1 = new Label();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.SuspendLayout();
            // 
            // comboBoxPolicy
            // 
            this.comboBoxPolicy.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right);
            this.comboBoxPolicy.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.comboBoxPolicy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPolicy.Location = new System.Drawing.Point(8, 26);
            this.comboBoxPolicy.MaxDropDownItems = 15;
            this.comboBoxPolicy.Name = "comboBoxPolicy";
            this.comboBoxPolicy.Size = new System.Drawing.Size(272, 21);
            this.comboBoxPolicy.TabIndex = 0;
            this.comboBoxPolicy.SelectedIndexChanged += new EventHandler(this.ComboBoxPolicy_SelectedIndexChanged);
            this.comboBoxPolicy.MeasureItem += new MeasureItemEventHandler(this.ComboBoxPolicy_MeasureItem);
            this.comboBoxPolicy.DrawItem += new DrawItemEventHandler(this.ComboBoxPolicy_DrawItem);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Run policy:";
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonOK.Location = new System.Drawing.Point(128, 58);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(208, 58);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            // 
            // RunPolicy
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(292, 86);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.comboBoxPolicy);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Name = "RunPolicy";
            this.ShowInTaskbar = false;
            this.Text = "Run Policy";
            this.ResumeLayout(false);
        }
        #endregion

        private Label label1;
        private Button buttonOK;
        private Button buttonCancel;
        private ComboBox comboBoxPolicy;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}

