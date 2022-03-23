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
            this.label1 = new System.Windows.Forms.Label();
            this.panelSpeedSteps = new System.Windows.Forms.Panel();
            this.radioButton28steps = new System.Windows.Forms.RadioButton();
            this.radioButton14steps = new System.Windows.Forms.RadioButton();
            this.panelSpeedSteps.SuspendLayout();
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
            this.radioButtonSetAddress.CheckedChanged += new System.EventHandler(this.RadioButtonSetAddress_CheckedChanged);
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
            this.radioButtonUnknownAddress.CheckedChanged += new System.EventHandler(this.RadioButtonUnknownAddress_CheckedChanged);
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
            this.buttonAllocateAddress.Click += new System.EventHandler(this.ButtonAllocateAddress_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(226, 147);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonSaveOnly
            // 
            this.buttonSaveOnly.Location = new System.Drawing.Point(9, 147);
            this.buttonSaveOnly.Name = "buttonSaveOnly";
            this.buttonSaveOnly.Size = new System.Drawing.Size(211, 23);
            this.buttonSaveOnly.TabIndex = 8;
            this.buttonSaveOnly.Text = "&Save without programming locomotive";
            this.buttonSaveOnly.UseVisualStyleBackColor = true;
            this.buttonSaveOnly.Click += new System.EventHandler(this.ButtonSaveOnly_Click);
            // 
            // buttonSaveAndProgram
            // 
            this.buttonSaveAndProgram.Location = new System.Drawing.Point(9, 118);
            this.buttonSaveAndProgram.Name = "buttonSaveAndProgram";
            this.buttonSaveAndProgram.Size = new System.Drawing.Size(211, 23);
            this.buttonSaveAndProgram.TabIndex = 7;
            this.buttonSaveAndProgram.Text = "Save and &program locomotive";
            this.buttonSaveAndProgram.UseVisualStyleBackColor = true;
            this.buttonSaveAndProgram.Click += new System.EventHandler(this.ButtonSaveAndProgram_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Speed steps:";
            // 
            // panelSpeedSteps
            // 
            this.panelSpeedSteps.Controls.Add(this.radioButton28steps);
            this.panelSpeedSteps.Controls.Add(this.radioButton14steps);
            this.panelSpeedSteps.Location = new System.Drawing.Point(81, 58);
            this.panelSpeedSteps.Name = "panelSpeedSteps";
            this.panelSpeedSteps.Size = new System.Drawing.Size(181, 36);
            this.panelSpeedSteps.TabIndex = 10;
            // 
            // radioButton28steps
            // 
            this.radioButton28steps.AutoSize = true;
            this.radioButton28steps.Location = new System.Drawing.Point(94, 9);
            this.radioButton28steps.Name = "radioButton28steps";
            this.radioButton28steps.Size = new System.Drawing.Size(65, 17);
            this.radioButton28steps.TabIndex = 8;
            this.radioButton28steps.TabStop = true;
            this.radioButton28steps.Text = "28 steps";
            this.radioButton28steps.UseVisualStyleBackColor = true;
            // 
            // radioButton14steps
            // 
            this.radioButton14steps.AutoSize = true;
            this.radioButton14steps.Location = new System.Drawing.Point(23, 9);
            this.radioButton14steps.Name = "radioButton14steps";
            this.radioButton14steps.Size = new System.Drawing.Size(65, 17);
            this.radioButton14steps.TabIndex = 7;
            this.radioButton14steps.TabStop = true;
            this.radioButton14steps.Text = "14 steps";
            this.radioButton14steps.UseVisualStyleBackColor = true;
            // 
            // ChangeLocomotiveAddress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(313, 182);
            this.ControlBox = false;
            this.Controls.Add(this.panelSpeedSteps);
            this.Controls.Add(this.label1);
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
            this.panelSpeedSteps.ResumeLayout(false);
            this.panelSpeedSteps.PerformLayout();
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelSpeedSteps;
        private System.Windows.Forms.RadioButton radioButton28steps;
        private System.Windows.Forms.RadioButton radioButton14steps;
    }
}