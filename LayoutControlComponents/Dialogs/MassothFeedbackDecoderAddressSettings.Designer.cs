namespace LayoutManager.ControlComponents.Dialogs {
	partial class MassothFeedbackDecoderAddressSettings {
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
			this.radioButtonMaster = new System.Windows.Forms.RadioButton();
			this.radioButtonSlave = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxBusId = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.panelBusId = new System.Windows.Forms.Panel();
			this.panelBusId.SuspendLayout();
			this.SuspendLayout();
			// 
			// radioButtonMaster
			// 
			this.radioButtonMaster.AutoSize = true;
			this.radioButtonMaster.Location = new System.Drawing.Point(12, 12);
			this.radioButtonMaster.Name = "radioButtonMaster";
			this.radioButtonMaster.Size = new System.Drawing.Size(304, 17);
			this.radioButtonMaster.TabIndex = 0;
			this.radioButtonMaster.TabStop = true;
			this.radioButtonMaster.Text = "Master - Feedback module is connected to DiMAX bus port";
			this.radioButtonMaster.UseVisualStyleBackColor = true;
			this.radioButtonMaster.CheckedChanged += new System.EventHandler(this.radioButtonMaster_CheckedChanged);
			// 
			// radioButtonSlave
			// 
			this.radioButtonSlave.AutoSize = true;
			this.radioButtonSlave.Location = new System.Drawing.Point(12, 67);
			this.radioButtonSlave.Name = "radioButtonSlave";
			this.radioButtonSlave.Size = new System.Drawing.Size(346, 17);
			this.radioButtonSlave.TabIndex = 1;
			this.radioButtonSlave.TabStop = true;
			this.radioButtonSlave.Text = "Slave - Feedback module is connected to another feedback module";
			this.radioButtonSlave.UseVisualStyleBackColor = true;
			this.radioButtonSlave.CheckedChanged += new System.EventHandler(this.radioButtonSlave_CheckedChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(2, 6);
			this.label1.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(42, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Bus ID:";
			// 
			// textBoxBusId
			// 
			this.textBoxBusId.Location = new System.Drawing.Point(50, 3);
			this.textBoxBusId.Name = "textBoxBusId";
			this.textBoxBusId.Size = new System.Drawing.Size(49, 20);
			this.textBoxBusId.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(105, 6);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(276, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "(11-20: Must be unique anong master feedback modules)";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(29, 87);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(404, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "(Note: Upto 4 slave feedback modules can be connected to a single master module)";
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(277, 126);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 6;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Location = new System.Drawing.Point(358, 126);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// panelBusId
			// 
			this.panelBusId.Controls.Add(this.label2);
			this.panelBusId.Controls.Add(this.label1);
			this.panelBusId.Controls.Add(this.textBoxBusId);
			this.panelBusId.Location = new System.Drawing.Point(32, 29);
			this.panelBusId.Margin = new System.Windows.Forms.Padding(0);
			this.panelBusId.Name = "panelBusId";
			this.panelBusId.Size = new System.Drawing.Size(393, 32);
			this.panelBusId.TabIndex = 8;
			// 
			// MassothFeedbackDecoderAddressSettings
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(445, 154);
			this.ControlBox = false;
			this.Controls.Add(this.panelBusId);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.radioButtonSlave);
			this.Controls.Add(this.radioButtonMaster);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "MassothFeedbackDecoderAddressSettings";
			this.ShowInTaskbar = false;
			this.Text = "Massoth Feedback Decoder Address Settings";
			this.panelBusId.ResumeLayout(false);
			this.panelBusId.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton radioButtonMaster;
		private System.Windows.Forms.RadioButton radioButtonSlave;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxBusId;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Panel panelBusId;
	}
}