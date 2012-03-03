namespace LayoutManager.Tools.Dialogs {
	partial class ChangeLocomotiveAddress {
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
			this.radioButtonSetAddress = new System.Windows.Forms.RadioButton();
			this.radioButtonUnknownAddress = new System.Windows.Forms.RadioButton();
			this.textBoxAddress = new System.Windows.Forms.TextBox();
			this.buttonAllocateAddress = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonSaveOnly = new System.Windows.Forms.Button();
			this.buttonSaveAndProgram = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// radioButtonSetAddress
			// 
			this.radioButtonSetAddress.AutoSize = true;
			this.radioButtonSetAddress.Location = new System.Drawing.Point(12, 12);
			this.radioButtonSetAddress.Name = "radioButtonSetAddress";
			this.radioButtonSetAddress.Size = new System.Drawing.Size(99, 17);
			this.radioButtonSetAddress.TabIndex = 0;
			this.radioButtonSetAddress.TabStop = true;
			this.radioButtonSetAddress.Text = "Set address to: ";
			this.radioButtonSetAddress.UseVisualStyleBackColor = true;
			this.radioButtonSetAddress.CheckedChanged += new System.EventHandler(this.radioButtonSetAddress_CheckedChanged);
			// 
			// radioButtonUnknownAddress
			// 
			this.radioButtonUnknownAddress.AutoSize = true;
			this.radioButtonUnknownAddress.Location = new System.Drawing.Point(12, 35);
			this.radioButtonUnknownAddress.Name = "radioButtonUnknownAddress";
			this.radioButtonUnknownAddress.Size = new System.Drawing.Size(226, 17);
			this.radioButtonUnknownAddress.TabIndex = 3;
			this.radioButtonUnknownAddress.TabStop = true;
			this.radioButtonUnknownAddress.Text = "The address of this locomotive is unknown";
			this.radioButtonUnknownAddress.UseVisualStyleBackColor = true;
			this.radioButtonUnknownAddress.CheckedChanged += new System.EventHandler(this.radioButtonUnknownAddress_CheckedChanged);
			// 
			// textBoxAddress
			// 
			this.textBoxAddress.Location = new System.Drawing.Point(104, 11);
			this.textBoxAddress.Name = "textBoxAddress";
			this.textBoxAddress.Size = new System.Drawing.Size(63, 20);
			this.textBoxAddress.TabIndex = 1;
			// 
			// buttonAllocateAddress
			// 
			this.buttonAllocateAddress.Location = new System.Drawing.Point(173, 9);
			this.buttonAllocateAddress.Name = "buttonAllocateAddress";
			this.buttonAllocateAddress.Size = new System.Drawing.Size(101, 23);
			this.buttonAllocateAddress.TabIndex = 2;
			this.buttonAllocateAddress.Text = "&Allocate address";
			this.buttonAllocateAddress.UseVisualStyleBackColor = true;
			this.buttonAllocateAddress.Click += new System.EventHandler(this.buttonAllocateAddress_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Location = new System.Drawing.Point(226, 116);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonSaveOnly
			// 
			this.buttonSaveOnly.Location = new System.Drawing.Point(9, 116);
			this.buttonSaveOnly.Name = "buttonSaveOnly";
			this.buttonSaveOnly.Size = new System.Drawing.Size(211, 23);
			this.buttonSaveOnly.TabIndex = 5;
			this.buttonSaveOnly.Text = "&Save without programming locomotive";
			this.buttonSaveOnly.UseVisualStyleBackColor = true;
			this.buttonSaveOnly.Click += new System.EventHandler(this.buttonSaveOnly_Click);
			// 
			// buttonSaveAndProgram
			// 
			this.buttonSaveAndProgram.Location = new System.Drawing.Point(9, 87);
			this.buttonSaveAndProgram.Name = "buttonSaveAndProgram";
			this.buttonSaveAndProgram.Size = new System.Drawing.Size(211, 23);
			this.buttonSaveAndProgram.TabIndex = 4;
			this.buttonSaveAndProgram.Text = "Save and &program locomotive";
			this.buttonSaveAndProgram.UseVisualStyleBackColor = true;
			this.buttonSaveAndProgram.Click += new System.EventHandler(this.buttonSaveAndProgram_Click);
			// 
			// ChangeLocomotiveAddress
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(313, 151);
			this.ControlBox = false;
			this.Controls.Add(this.buttonSaveAndProgram);
			this.Controls.Add(this.buttonSaveOnly);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonAllocateAddress);
			this.Controls.Add(this.textBoxAddress);
			this.Controls.Add(this.radioButtonUnknownAddress);
			this.Controls.Add(this.radioButtonSetAddress);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "ChangeLocomotiveAddress";
			this.ShowInTaskbar = false;
			this.Text = "Change Address of {0}";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton radioButtonSetAddress;
		private System.Windows.Forms.RadioButton radioButtonUnknownAddress;
		private System.Windows.Forms.TextBox textBoxAddress;
		private System.Windows.Forms.Button buttonAllocateAddress;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonSaveOnly;
		private System.Windows.Forms.Button buttonSaveAndProgram;
	}
}