using System;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveFront.
    /// </summary>
    partial class LocomotiveFront : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelBlockName = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.locomotiveFrontControl = new CommonUI.Controls.LocomotiveFront();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(23, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(707, 98);
            this.label1.TabIndex = 1;
            this.label1.Text = "The locomotive orientation need to be specified. Please click on the point to whi" +
    "ch the locomotive front is directed";
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(166, 512);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(195, 57);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "OK";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(395, 512);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(195, 57);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            // 
            // labelBlockName
            // 
            this.labelBlockName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labelBlockName.Location = new System.Drawing.Point(148, 455);
            this.labelBlockName.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.labelBlockName.Name = "labelBlockName";
            this.labelBlockName.Size = new System.Drawing.Size(458, 39);
            this.labelBlockName.TabIndex = 3;
            this.labelBlockName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Location = new System.Drawing.Point(190, 138);
            this.panel1.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(368, 309);
            this.panel1.TabIndex = 4;
            // 
            // LocomotiveFront
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(754, 588);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labelBlockName);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "LocomotiveFront";
            this.ShowInTaskbar = false;
            this.Text = "Specify Locomotive Front";
            this.ResumeLayout(false);

        }
        #endregion
        private Label label1;
        private Button buttonOk;
        private Button buttonCancel;
        private Label labelBlockName;
        private Panel panel1;
        private LayoutManager.CommonUI.Controls.LocomotiveFront locomotiveFrontControl;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}

