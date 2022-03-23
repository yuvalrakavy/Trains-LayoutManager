namespace LayoutManager.CommonUI.Dialogs {
	public partial class SerialInterfaceParameters {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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
			this.comboBoxBaudrate = new System.Windows.Forms.ComboBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radioButtonStopBits2 = new System.Windows.Forms.RadioButton();
			this.radioButtonStopBits15 = new System.Windows.Forms.RadioButton();
			this.radioButtonStopBits1 = new System.Windows.Forms.RadioButton();
			this.checkBoxXonXoff = new System.Windows.Forms.CheckBox();
			this.checkBoxDSRhandshake = new System.Windows.Forms.CheckBox();
			this.checkBoxCTShandshake = new System.Windows.Forms.CheckBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.radioButtonDTRhandshake = new System.Windows.Forms.RadioButton();
			this.radioButtonDTRon = new System.Windows.Forms.RadioButton();
			this.radioButtonDTRoff = new System.Windows.Forms.RadioButton();
			this.checkBoxNoTimeout = new System.Windows.Forms.CheckBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.radioButtonRTStoggle = new System.Windows.Forms.RadioButton();
			this.radioButtonRTShandshake = new System.Windows.Forms.RadioButton();
			this.radioButtonRTSon = new System.Windows.Forms.RadioButton();
			this.radioButtonRTSoff = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.comboBoxParity = new System.Windows.Forms.ComboBox();
			this.checkBoxDSR = new System.Windows.Forms.CheckBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioButtonData5 = new System.Windows.Forms.RadioButton();
			this.radioButtonData6 = new System.Windows.Forms.RadioButton();
			this.radioButtonData7 = new System.Windows.Forms.RadioButton();
			this.radioButtonData8 = new System.Windows.Forms.RadioButton();
			this.groupBox2.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(97, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Baud rate (speed): ";
			// 
			// comboBoxBaudrate
			// 
			this.comboBoxBaudrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxBaudrate.FormattingEnabled = true;
			this.comboBoxBaudrate.Location = new System.Drawing.Point(118, 10);
			this.comboBoxBaudrate.Name = "comboBoxBaudrate";
			this.comboBoxBaudrate.Size = new System.Drawing.Size(98, 21);
			this.comboBoxBaudrate.TabIndex = 1;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.radioButtonStopBits2);
			this.groupBox2.Controls.Add(this.radioButtonStopBits15);
			this.groupBox2.Controls.Add(this.radioButtonStopBits1);
			this.groupBox2.Location = new System.Drawing.Point(118, 69);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(98, 117);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Stop bits:";
			// 
			// radioButtonStopBits2
			// 
			this.radioButtonStopBits2.AutoSize = true;
			this.radioButtonStopBits2.Location = new System.Drawing.Point(7, 68);
			this.radioButtonStopBits2.Name = "radioButtonStopBits2";
			this.radioButtonStopBits2.Size = new System.Drawing.Size(73, 17);
			this.radioButtonStopBits2.TabIndex = 2;
			this.radioButtonStopBits2.Text = "2 stop bits";
			// 
			// radioButtonStopBits15
			// 
			this.radioButtonStopBits15.AutoSize = true;
			this.radioButtonStopBits15.Location = new System.Drawing.Point(7, 44);
			this.radioButtonStopBits15.Name = "radioButtonStopBits15";
			this.radioButtonStopBits15.Size = new System.Drawing.Size(82, 17);
			this.radioButtonStopBits15.TabIndex = 1;
			this.radioButtonStopBits15.Text = "1.5 stop bits";
			// 
			// radioButtonStopBits1
			// 
			this.radioButtonStopBits1.AutoSize = true;
			this.radioButtonStopBits1.Location = new System.Drawing.Point(7, 20);
			this.radioButtonStopBits1.Name = "radioButtonStopBits1";
			this.radioButtonStopBits1.Size = new System.Drawing.Size(70, 17);
			this.radioButtonStopBits1.TabIndex = 0;
			this.radioButtonStopBits1.Text = "1 Stop bit";
			// 
			// checkBoxXonXoff
			// 
			this.checkBoxXonXoff.AutoSize = true;
			this.checkBoxXonXoff.Location = new System.Drawing.Point(13, 341);
			this.checkBoxXonXoff.Name = "checkBoxXonXoff";
			this.checkBoxXonXoff.Size = new System.Drawing.Size(178, 17);
			this.checkBoxXonXoff.TabIndex = 9;
			this.checkBoxXonXoff.Text = "Software Handshake (Xon/Xoff)";
			// 
			// checkBoxDSRhandshake
			// 
			this.checkBoxDSRhandshake.AutoSize = true;
			this.checkBoxDSRhandshake.Location = new System.Drawing.Point(13, 365);
			this.checkBoxDSRhandshake.Name = "checkBoxDSRhandshake";
			this.checkBoxDSRhandshake.Size = new System.Drawing.Size(170, 17);
			this.checkBoxDSRhandshake.TabIndex = 10;
			this.checkBoxDSRhandshake.Text = "Output Handshake using DSR";
			// 
			// checkBoxCTShandshake
			// 
			this.checkBoxCTShandshake.AutoSize = true;
			this.checkBoxCTShandshake.Location = new System.Drawing.Point(13, 389);
			this.checkBoxCTShandshake.Name = "checkBoxCTShandshake";
			this.checkBoxCTShandshake.Size = new System.Drawing.Size(168, 17);
			this.checkBoxCTShandshake.TabIndex = 11;
			this.checkBoxCTShandshake.Text = "Output Handshake using CTS";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.radioButtonDTRhandshake);
			this.groupBox4.Controls.Add(this.radioButtonDTRon);
			this.groupBox4.Controls.Add(this.radioButtonDTRoff);
			this.groupBox4.Location = new System.Drawing.Point(13, 193);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(98, 117);
			this.groupBox4.TabIndex = 6;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "DTR:";
			// 
			// radioButtonDTRhandshake
			// 
			this.radioButtonDTRhandshake.AutoSize = true;
			this.radioButtonDTRhandshake.Location = new System.Drawing.Point(7, 68);
			this.radioButtonDTRhandshake.Name = "radioButtonDTRhandshake";
			this.radioButtonDTRhandshake.Size = new System.Drawing.Size(80, 17);
			this.radioButtonDTRhandshake.TabIndex = 2;
			this.radioButtonDTRhandshake.Text = "Handshake";
			// 
			// radioButtonDTRon
			// 
			this.radioButtonDTRon.AutoSize = true;
			this.radioButtonDTRon.Location = new System.Drawing.Point(7, 44);
			this.radioButtonDTRon.Name = "radioButtonDTRon";
			this.radioButtonDTRon.Size = new System.Drawing.Size(39, 17);
			this.radioButtonDTRon.TabIndex = 1;
			this.radioButtonDTRon.Text = "On";
			// 
			// radioButtonDTRoff
			// 
			this.radioButtonDTRoff.AutoSize = true;
			this.radioButtonDTRoff.Location = new System.Drawing.Point(7, 20);
			this.radioButtonDTRoff.Name = "radioButtonDTRoff";
			this.radioButtonDTRoff.Size = new System.Drawing.Size(39, 17);
			this.radioButtonDTRoff.TabIndex = 0;
			this.radioButtonDTRoff.Text = "Off";
			// 
			// checkBoxNoTimeout
			// 
			this.checkBoxNoTimeout.AutoSize = true;
			this.checkBoxNoTimeout.Location = new System.Drawing.Point(13, 317);
			this.checkBoxNoTimeout.Name = "checkBoxNoTimeout";
			this.checkBoxNoTimeout.Size = new System.Drawing.Size(98, 17);
			this.checkBoxNoTimeout.TabIndex = 8;
			this.checkBoxNoTimeout.Text = "Infinite Timeout";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.radioButtonRTStoggle);
			this.groupBox3.Controls.Add(this.radioButtonRTShandshake);
			this.groupBox3.Controls.Add(this.radioButtonRTSon);
			this.groupBox3.Controls.Add(this.radioButtonRTSoff);
			this.groupBox3.Location = new System.Drawing.Point(118, 193);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(98, 117);
			this.groupBox3.TabIndex = 7;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "RTS:";
			// 
			// radioButtonRTStoggle
			// 
			this.radioButtonRTStoggle.AutoSize = true;
			this.radioButtonRTStoggle.Location = new System.Drawing.Point(7, 92);
			this.radioButtonRTStoggle.Name = "radioButtonRTStoggle";
			this.radioButtonRTStoggle.Size = new System.Drawing.Size(58, 17);
			this.radioButtonRTStoggle.TabIndex = 3;
			this.radioButtonRTStoggle.Text = "Toggle";
			// 
			// radioButtonRTShandshake
			// 
			this.radioButtonRTShandshake.AutoSize = true;
			this.radioButtonRTShandshake.Location = new System.Drawing.Point(7, 68);
			this.radioButtonRTShandshake.Name = "radioButtonRTShandshake";
			this.radioButtonRTShandshake.Size = new System.Drawing.Size(80, 17);
			this.radioButtonRTShandshake.TabIndex = 2;
			this.radioButtonRTShandshake.Text = "Handshake";
			// 
			// radioButtonRTSon
			// 
			this.radioButtonRTSon.AutoSize = true;
			this.radioButtonRTSon.Location = new System.Drawing.Point(7, 44);
			this.radioButtonRTSon.Name = "radioButtonRTSon";
			this.radioButtonRTSon.Size = new System.Drawing.Size(39, 17);
			this.radioButtonRTSon.TabIndex = 1;
			this.radioButtonRTSon.Text = "On";
			// 
			// radioButtonRTSoff
			// 
			this.radioButtonRTSoff.AutoSize = true;
			this.radioButtonRTSoff.Location = new System.Drawing.Point(7, 20);
			this.radioButtonRTSoff.Name = "radioButtonRTSoff";
			this.radioButtonRTSoff.Size = new System.Drawing.Size(39, 17);
			this.radioButtonRTSoff.TabIndex = 0;
			this.radioButtonRTSoff.Text = "Off";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 38);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(36, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Parity:";
			// 
			// comboBoxParity
			// 
			this.comboBoxParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxParity.FormattingEnabled = true;
			this.comboBoxParity.Items.AddRange(new object[] {
            "None",
            "Even",
            "Odd",
            "Mark",
            "Space"});
			this.comboBoxParity.Location = new System.Drawing.Point(118, 35);
			this.comboBoxParity.Name = "comboBoxParity";
			this.comboBoxParity.Size = new System.Drawing.Size(98, 21);
			this.comboBoxParity.TabIndex = 3;
			// 
			// checkBoxDSR
			// 
			this.checkBoxDSR.AutoSize = true;
			this.checkBoxDSR.Location = new System.Drawing.Point(13, 413);
			this.checkBoxDSR.Name = "checkBoxDSR";
			this.checkBoxDSR.Size = new System.Drawing.Size(99, 17);
			this.checkBoxDSR.TabIndex = 12;
			this.checkBoxDSR.Text = "DSR Sensitivity";
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(229, 10);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 13;
			this.buttonOK.Text = "OK";
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(229, 40);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 14;
			this.buttonCancel.Text = "Cancel";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioButtonData5);
			this.groupBox1.Controls.Add(this.radioButtonData6);
			this.groupBox1.Controls.Add(this.radioButtonData7);
			this.groupBox1.Controls.Add(this.radioButtonData8);
			this.groupBox1.Location = new System.Drawing.Point(13, 69);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(98, 117);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Data bits:";
			// 
			// radioButtonData5
			// 
			this.radioButtonData5.AutoSize = true;
			this.radioButtonData5.Location = new System.Drawing.Point(7, 92);
			this.radioButtonData5.Name = "radioButtonData5";
			this.radioButtonData5.Size = new System.Drawing.Size(50, 17);
			this.radioButtonData5.TabIndex = 3;
			this.radioButtonData5.Text = "5 bits";
			// 
			// radioButtonData6
			// 
			this.radioButtonData6.AutoSize = true;
			this.radioButtonData6.Location = new System.Drawing.Point(7, 68);
			this.radioButtonData6.Name = "radioButtonData6";
			this.radioButtonData6.Size = new System.Drawing.Size(50, 17);
			this.radioButtonData6.TabIndex = 2;
			this.radioButtonData6.Text = "6 bits";
			// 
			// radioButtonData7
			// 
			this.radioButtonData7.AutoSize = true;
			this.radioButtonData7.Location = new System.Drawing.Point(7, 44);
			this.radioButtonData7.Name = "radioButtonData7";
			this.radioButtonData7.Size = new System.Drawing.Size(50, 17);
			this.radioButtonData7.TabIndex = 1;
			this.radioButtonData7.Text = "7 bits";
			// 
			// radioButtonData8
			// 
			this.radioButtonData8.AutoSize = true;
			this.radioButtonData8.Location = new System.Drawing.Point(7, 20);
			this.radioButtonData8.Name = "radioButtonData8";
			this.radioButtonData8.Size = new System.Drawing.Size(50, 17);
			this.radioButtonData8.TabIndex = 0;
			this.radioButtonData8.Text = "8 bits";
			// 
			// SerialInterfaceParameters
			// 
			this.AcceptButton = this.buttonOK;
			this.CancelButton = this.buttonCancel;
			this.AutoScaleMode = AutoScaleMode.Font;
			this.AutoScaleDimensions = new SizeF(5, 13);
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(316, 441);
			this.ControlBox = false;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.checkBoxDSR);
			this.Controls.Add(this.comboBoxParity);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.checkBoxNoTimeout);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.checkBoxCTShandshake);
			this.Controls.Add(this.checkBoxDSRhandshake);
			this.Controls.Add(this.checkBoxXonXoff);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.comboBoxBaudrate);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "SerialInterfaceParameters";
			this.Text = "Serial Port Settings";
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxBaudrate;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioButtonStopBits15;
		private System.Windows.Forms.RadioButton radioButtonStopBits1;
		private System.Windows.Forms.CheckBox checkBoxXonXoff;
		private System.Windows.Forms.CheckBox checkBoxDSRhandshake;
		private System.Windows.Forms.CheckBox checkBoxCTShandshake;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.RadioButton radioButtonDTRhandshake;
		private System.Windows.Forms.RadioButton radioButtonDTRon;
		private System.Windows.Forms.RadioButton radioButtonDTRoff;
		private System.Windows.Forms.CheckBox checkBoxNoTimeout;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton radioButtonRTShandshake;
		private System.Windows.Forms.RadioButton radioButtonRTSon;
		private System.Windows.Forms.RadioButton radioButtonRTSoff;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBoxParity;
		private System.Windows.Forms.RadioButton radioButtonRTStoggle;
		private System.Windows.Forms.CheckBox checkBoxDSR;
		private System.Windows.Forms.RadioButton radioButtonStopBits2;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonData6;
		private System.Windows.Forms.RadioButton radioButtonData7;
		private System.Windows.Forms.RadioButton radioButtonData8;
		private System.Windows.Forms.RadioButton radioButtonData5;
	}
}