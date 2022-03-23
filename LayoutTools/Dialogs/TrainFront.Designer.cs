using System;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveFront.
    /// </summary>
    partial class TrainFront : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrainFront));
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelBlockName = new System.Windows.Forms.Label();
            this.locomotiveFrontControl = new LayoutManager.CommonUI.Controls.LocomotiveFront();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(9, 148);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += new EventHandler(this.ButtonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(90, 148);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            // 
            // labelBlockName
            // 
            this.labelBlockName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (byte)177);
            this.labelBlockName.Location = new System.Drawing.Point(57, 185);
            this.labelBlockName.Name = "labelBlockName";
            this.labelBlockName.Size = new System.Drawing.Size(176, 16);
            this.labelBlockName.TabIndex = 3;
            this.labelBlockName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // locomotiveFrontControl
            // 
            this.locomotiveFrontControl.ConnectionPoints = (System.Collections.Generic.IList<LayoutManager.Model.LayoutComponentConnectionPoint>)resources.GetObject("locomotiveFrontControl.ConnectionPoints");
            this.locomotiveFrontControl.Front = (LayoutManager.Model.LayoutComponentConnectionPoint)resources.GetObject("locomotiveFrontControl.Front");
            this.locomotiveFrontControl.Location = new System.Drawing.Point(26, 25);
            this.locomotiveFrontControl.LocomotiveName = null;
            this.locomotiveFrontControl.Name = "locomotiveFrontControl";
            this.locomotiveFrontControl.Size = new System.Drawing.Size(117, 105);
            this.locomotiveFrontControl.TabIndex = 1;
            this.locomotiveFrontControl.Text = "locomotiveFront1";
            // 
            // TrainFront
            // 
            this.AcceptButton = this.buttonOk;
            this.CancelButton = this.buttonCancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(177, 179);
            this.ControlBox = false;
            this.Controls.Add(this.locomotiveFrontControl);
            this.Controls.Add(this.labelBlockName);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "TrainFront";
            this.ShowInTaskbar = false;
            this.Text = "Set train orientation";
            this.ResumeLayout(false);
        }
        #endregion

        private Button buttonOk;
        private Button buttonCancel;
        private Label labelBlockName;
        private LayoutManager.CommonUI.Controls.LocomotiveFront locomotiveFrontControl;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

    }
}

