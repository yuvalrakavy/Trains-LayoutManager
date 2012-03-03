namespace LayoutManager.Tools.Dialogs {
	partial class GetSavedPolicyName {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
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
			this.label1 = new System.Windows.Forms.Label();
			this.checkBoxGlobalPolicy = new System.Windows.Forms.CheckBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.comboBoxName = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Name:";
			// 
			// checkBoxGlobalPolicy
			// 
			this.checkBoxGlobalPolicy.Location = new System.Drawing.Point(15, 32);
			this.checkBoxGlobalPolicy.Name = "checkBoxGlobalPolicy";
			this.checkBoxGlobalPolicy.Size = new System.Drawing.Size(248, 18);
			this.checkBoxGlobalPolicy.TabIndex = 4;
			this.checkBoxGlobalPolicy.Text = "This policy is available in all layouts";
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(126, 56);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 5;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(207, 56);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// comboBoxName
			// 
			this.comboBoxName.DropDownHeight = 200;
			this.comboBoxName.FormattingEnabled = true;
			this.comboBoxName.IntegralHeight = false;
			this.comboBoxName.Location = new System.Drawing.Point(56, 6);
			this.comboBoxName.Name = "comboBoxName";
			this.comboBoxName.Size = new System.Drawing.Size(224, 21);
			this.comboBoxName.TabIndex = 7;
			// 
			// GetSavedPolicyName
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(292, 87);
			this.ControlBox = false;
			this.Controls.Add(this.comboBoxName);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.checkBoxGlobalPolicy);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "GetSavedPolicyName";
			this.Text = "Save Definition";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkBoxGlobalPolicy;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.ComboBox comboBoxName;
	}
}