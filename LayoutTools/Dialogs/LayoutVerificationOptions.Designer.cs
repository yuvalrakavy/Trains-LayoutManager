namespace LayoutManager.Tools.Dialogs {
    partial class LayoutVerificationOptions {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.checkBoxIgnoreFeedbackComponents = new System.Windows.Forms.CheckBox();
            this.checkBoxIgnorePowerComponents = new System.Windows.Forms.CheckBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkBoxIgnoreFeedbackComponents
            // 
            this.checkBoxIgnoreFeedbackComponents.AutoSize = true;
            this.checkBoxIgnoreFeedbackComponents.Location = new System.Drawing.Point(12, 12);
            this.checkBoxIgnoreFeedbackComponents.Name = "checkBoxIgnoreFeedbackComponents";
            this.checkBoxIgnoreFeedbackComponents.Size = new System.Drawing.Size(335, 17);
            this.checkBoxIgnoreFeedbackComponents.TabIndex = 0;
            this.checkBoxIgnoreFeedbackComponents.Text = "Ignore not connected feedback components (e.g. track contacts)";
            this.checkBoxIgnoreFeedbackComponents.UseVisualStyleBackColor = true;
            // 
            // checkBoxIgnorePowerComponents
            // 
            this.checkBoxIgnorePowerComponents.AutoSize = true;
            this.checkBoxIgnorePowerComponents.Location = new System.Drawing.Point(12, 35);
            this.checkBoxIgnorePowerComponents.Name = "checkBoxIgnorePowerComponents";
            this.checkBoxIgnorePowerComponents.Size = new System.Drawing.Size(371, 17);
            this.checkBoxIgnorePowerComponents.TabIndex = 1;
            this.checkBoxIgnorePowerComponents.Text = "Ignore not connected power components (e.g. power switches/selectors)";
            this.checkBoxIgnorePowerComponents.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(309, 83);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(228, 83);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // LayoutVerificationOptions
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(396, 118);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.checkBoxIgnorePowerComponents);
            this.Controls.Add(this.checkBoxIgnoreFeedbackComponents);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "LayoutVerificationOptions";
            this.ShowInTaskbar = false;
            this.Text = "Layout Verificiation Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxIgnoreFeedbackComponents;
        private System.Windows.Forms.CheckBox checkBoxIgnorePowerComponents;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
    }
}