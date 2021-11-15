using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    partial class GetTargetSpeed {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.trackBarTargetSpeed = new System.Windows.Forms.TrackBar();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.numericUpDownTargetSpeed = new System.Windows.Forms.NumericUpDown();
            this.labelLowerLimit = new System.Windows.Forms.Label();
            this.labelUpperLimit = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTargetSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTargetSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBarTargetSpeed
            // 
            this.trackBarTargetSpeed.Location = new System.Drawing.Point(42, 20);
            this.trackBarTargetSpeed.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.trackBarTargetSpeed.Name = "trackBarTargetSpeed";
            this.trackBarTargetSpeed.Size = new System.Drawing.Size(582, 90);
            this.trackBarTargetSpeed.TabIndex = 0;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(489, 167);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(146, 57);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(647, 167);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(146, 57);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            // 
            // numericUpDownTargetSpeed
            // 
            this.numericUpDownTargetSpeed.Location = new System.Drawing.Point(666, 27);
            this.numericUpDownTargetSpeed.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.numericUpDownTargetSpeed.Name = "numericUpDownTargetSpeed";
            this.numericUpDownTargetSpeed.Size = new System.Drawing.Size(125, 39);
            this.numericUpDownTargetSpeed.TabIndex = 3;
            // 
            // labelLowerLimit
            // 
            this.labelLowerLimit.Location = new System.Drawing.Point(55, 98);
            this.labelLowerLimit.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.labelLowerLimit.Name = "labelLowerLimit";
            this.labelLowerLimit.Size = new System.Drawing.Size(42, 57);
            this.labelLowerLimit.TabIndex = 1;
            this.labelLowerLimit.Text = "1";
            this.labelLowerLimit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelUpperLimit
            // 
            this.labelUpperLimit.Location = new System.Drawing.Point(541, 98);
            this.labelUpperLimit.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.labelUpperLimit.Name = "labelUpperLimit";
            this.labelUpperLimit.Size = new System.Drawing.Size(83, 57);
            this.labelUpperLimit.TabIndex = 2;
            this.labelUpperLimit.Text = "1";
            this.labelUpperLimit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GetTargetSpeed
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(816, 236);
            this.ControlBox = false;
            this.Controls.Add(this.labelUpperLimit);
            this.Controls.Add(this.labelLowerLimit);
            this.Controls.Add(this.numericUpDownTargetSpeed);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.trackBarTargetSpeed);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "GetTargetSpeed";
            this.Text = "Set Train Target Speed";
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTargetSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTargetSpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private Button buttonOK;
        private Button buttonCancel;
        private System.Windows.Forms.TrackBar trackBarTargetSpeed;
        private NumericUpDown numericUpDownTargetSpeed;
        private Label labelLowerLimit;
        private Label labelUpperLimit;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
    }
}
