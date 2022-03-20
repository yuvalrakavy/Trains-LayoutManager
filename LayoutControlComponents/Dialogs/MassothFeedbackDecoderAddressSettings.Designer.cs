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
            this.radioButtonMaster.Location = new System.Drawing.Point(26, 30);
            this.radioButtonMaster.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.radioButtonMaster.Name = "radioButtonMaster";
            this.radioButtonMaster.Size = new System.Drawing.Size(680, 36);
            this.radioButtonMaster.TabIndex = 0;
            this.radioButtonMaster.TabStop = true;
            this.radioButtonMaster.Text = "Master - Feedback module is connected to DiMAX bus port";
            this.radioButtonMaster.UseVisualStyleBackColor = true;
            this.radioButtonMaster.CheckedChanged += new System.EventHandler(this.RadioButtonMaster_CheckedChanged);
            // 
            // radioButtonSlave
            // 
            this.radioButtonSlave.AutoSize = true;
            this.radioButtonSlave.Location = new System.Drawing.Point(26, 165);
            this.radioButtonSlave.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.radioButtonSlave.Name = "radioButtonSlave";
            this.radioButtonSlave.Size = new System.Drawing.Size(769, 36);
            this.radioButtonSlave.TabIndex = 1;
            this.radioButtonSlave.TabStop = true;
            this.radioButtonSlave.Text = "Slave - Feedback module is connected to another feedback module";
            this.radioButtonSlave.UseVisualStyleBackColor = true;
            this.radioButtonSlave.CheckedChanged += new System.EventHandler(this.RadioButtonSlave_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 32);
            this.label1.TabIndex = 2;
            this.label1.Text = "Bus ID:";
            // 
            // textBoxBusId
            // 
            this.textBoxBusId.Location = new System.Drawing.Point(108, 7);
            this.textBoxBusId.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.textBoxBusId.Name = "textBoxBusId";
            this.textBoxBusId.Size = new System.Drawing.Size(102, 39);
            this.textBoxBusId.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(228, 15);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(629, 32);
            this.label2.TabIndex = 4;
            this.label2.Text = "(11-20: Must be unique among master feedback modules)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(63, 214);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(911, 32);
            this.label3.TabIndex = 5;
            this.label3.Text = "(Note: Up to 4 slave feedback modules can be connected to a single master module)";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(600, 310);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(162, 57);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(776, 310);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(162, 57);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // panelBusId
            // 
            this.panelBusId.Controls.Add(this.label2);
            this.panelBusId.Controls.Add(this.label1);
            this.panelBusId.Controls.Add(this.textBoxBusId);
            this.panelBusId.Location = new System.Drawing.Point(69, 71);
            this.panelBusId.Margin = new System.Windows.Forms.Padding(0);
            this.panelBusId.Name = "panelBusId";
            this.panelBusId.Size = new System.Drawing.Size(852, 79);
            this.panelBusId.TabIndex = 8;
            // 
            // MassothFeedbackDecoderAddressSettings
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(964, 379);
            this.ControlBox = false;
            this.Controls.Add(this.panelBusId);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.radioButtonSlave);
            this.Controls.Add(this.radioButtonMaster);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
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