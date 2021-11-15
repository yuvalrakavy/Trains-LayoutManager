namespace LayoutManager.CommonUI.Dialogs {
    partial class TrainTrackingOptions {
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonManualDispatchNormalTracking = new System.Windows.Forms.RadioButton();
            this.radioButtonManualDispatchNoTracking = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(12, 108);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(79, 26);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(97, 108);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(79, 26);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonManualDispatchNoTracking);
            this.groupBox1.Controls.Add(this.radioButtonManualDispatchNormalTracking);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(163, 80);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "In manual dispatch region";
            // 
            // radioButtonManualDispatchNormalTracking
            // 
            this.radioButtonManualDispatchNormalTracking.AutoSize = true;
            this.radioButtonManualDispatchNormalTracking.Location = new System.Drawing.Point(6, 19);
            this.radioButtonManualDispatchNormalTracking.Name = "radioButtonManualDispatchNormalTracking";
            this.radioButtonManualDispatchNormalTracking.Size = new System.Drawing.Size(99, 17);
            this.radioButtonManualDispatchNormalTracking.TabIndex = 0;
            this.radioButtonManualDispatchNormalTracking.TabStop = true;
            this.radioButtonManualDispatchNormalTracking.Text = "Normal tracking";
            this.radioButtonManualDispatchNormalTracking.UseVisualStyleBackColor = true;
            // 
            // radioButtonManualDispatchNoTracking
            // 
            this.radioButtonManualDispatchNoTracking.AutoSize = true;
            this.radioButtonManualDispatchNoTracking.Location = new System.Drawing.Point(6, 42);
            this.radioButtonManualDispatchNoTracking.Name = "radioButtonManualDispatchNoTracking";
            this.radioButtonManualDispatchNoTracking.Size = new System.Drawing.Size(80, 17);
            this.radioButtonManualDispatchNoTracking.TabIndex = 1;
            this.radioButtonManualDispatchNoTracking.TabStop = true;
            this.radioButtonManualDispatchNoTracking.Text = "No tracking";
            this.radioButtonManualDispatchNoTracking.UseVisualStyleBackColor = true;
            // 
            // TrainTrackingOptions
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(185, 145);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "TrainTrackingOptions";
            this.Text = "Train tracking options";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonManualDispatchNoTracking;
        private System.Windows.Forms.RadioButton radioButtonManualDispatchNormalTracking;
    }
}