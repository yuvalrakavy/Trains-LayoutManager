namespace TrainDetector.Dialogs {
    partial class TrainDetectorProperties {
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
            this.nameDefinition = new LayoutManager.CommonUI.Controls.NameDefinition();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxAutoDetect = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // nameDefinition
            // 
            this.nameDefinition.Component = null;
            this.nameDefinition.DefaultIsVisible = true;
            this.nameDefinition.ElementName = "Name";
            this.nameDefinition.IsOptional = false;
            this.nameDefinition.Location = new System.Drawing.Point(19, 8);
            this.nameDefinition.Name = "nameDefinition";
            this.nameDefinition.Size = new System.Drawing.Size(280, 66);
            this.nameDefinition.TabIndex = 0;
            this.nameDefinition.XmlInfo = null;
            this.nameDefinition.Load += new System.EventHandler(this.nameDefinition_Load);
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(166, 114);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(246, 114);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // checkBoxAutoDetect
            // 
            this.checkBoxAutoDetect.AutoSize = true;
            this.checkBoxAutoDetect.Checked = true;
            this.checkBoxAutoDetect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoDetect.Location = new System.Drawing.Point(20, 92);
            this.checkBoxAutoDetect.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxAutoDetect.Name = "checkBoxAutoDetect";
            this.checkBoxAutoDetect.Size = new System.Drawing.Size(279, 17);
            this.checkBoxAutoDetect.TabIndex = 2;
            this.checkBoxAutoDetect.Text = "Automatically detect and add train detector controllers";
            this.checkBoxAutoDetect.UseVisualStyleBackColor = true;
            // 
            // TrainDetectorProperties
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(329, 161);
            this.ControlBox = false;
            this.Controls.Add(this.checkBoxAutoDetect);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.nameDefinition);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "TrainDetectorProperties";
            this.ShowInTaskbar = false;
            this.Text = "Train Detector Properties";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LayoutManager.CommonUI.Controls.NameDefinition nameDefinition;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxAutoDetect;
    }
}