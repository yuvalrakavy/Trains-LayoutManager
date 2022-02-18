namespace DiMAX.Dialogs {
	partial class TestLocoSelect {
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
			this.checkBoxSelect = new System.Windows.Forms.CheckBox();
			this.checkBoxActive = new System.Windows.Forms.CheckBox();
			this.checkBoxUnconditional = new System.Windows.Forms.CheckBox();
			this.textBoxAddress = new System.Windows.Forms.TextBox();
			this.buttonSendCommand = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(105, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Locomotive address:";
			// 
			// checkBoxSelect
			// 
			this.checkBoxSelect.AutoSize = true;
			this.checkBoxSelect.Location = new System.Drawing.Point(12, 44);
			this.checkBoxSelect.Name = "checkBoxSelect";
			this.checkBoxSelect.Size = new System.Drawing.Size(56, 17);
			this.checkBoxSelect.TabIndex = 1;
			this.checkBoxSelect.Text = "Select";
			this.checkBoxSelect.UseVisualStyleBackColor = true;
			// 
			// checkBoxActive
			// 
			this.checkBoxActive.AutoSize = true;
			this.checkBoxActive.Location = new System.Drawing.Point(12, 67);
			this.checkBoxActive.Name = "checkBoxActive";
			this.checkBoxActive.Size = new System.Drawing.Size(56, 17);
			this.checkBoxActive.TabIndex = 2;
			this.checkBoxActive.Text = "Active";
			this.checkBoxActive.UseVisualStyleBackColor = true;
			// 
			// checkBoxUnconditional
			// 
			this.checkBoxUnconditional.AutoSize = true;
			this.checkBoxUnconditional.Location = new System.Drawing.Point(12, 90);
			this.checkBoxUnconditional.Name = "checkBoxUnconditional";
			this.checkBoxUnconditional.Size = new System.Drawing.Size(91, 17);
			this.checkBoxUnconditional.TabIndex = 3;
			this.checkBoxUnconditional.Text = "Unconditional";
			this.checkBoxUnconditional.UseVisualStyleBackColor = true;
			// 
			// textBoxAddress
			// 
			this.textBoxAddress.Location = new System.Drawing.Point(123, 6);
			this.textBoxAddress.Name = "textBoxAddress";
			this.textBoxAddress.Size = new System.Drawing.Size(100, 20);
			this.textBoxAddress.TabIndex = 4;
			// 
			// buttonSendCommand
			// 
			this.buttonSendCommand.Location = new System.Drawing.Point(168, 86);
			this.buttonSendCommand.Name = "buttonSendCommand";
			this.buttonSendCommand.Size = new System.Drawing.Size(104, 23);
			this.buttonSendCommand.TabIndex = 5;
			this.buttonSendCommand.Text = "Send command";
			this.buttonSendCommand.UseVisualStyleBackColor = true;
			this.buttonSendCommand.Click += new System.EventHandler(this.ButtonSendCommand_Click);
			// 
			// TestLocoSelect
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 122);
			this.Controls.Add(this.buttonSendCommand);
			this.Controls.Add(this.textBoxAddress);
			this.Controls.Add(this.checkBoxUnconditional);
			this.Controls.Add(this.checkBoxActive);
			this.Controls.Add(this.checkBoxSelect);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TestLocoSelect";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Test Locomotive Select";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkBoxSelect;
		private System.Windows.Forms.CheckBox checkBoxActive;
		private System.Windows.Forms.CheckBox checkBoxUnconditional;
		private System.Windows.Forms.TextBox textBoxAddress;
		private System.Windows.Forms.Button buttonSendCommand;
	}
}