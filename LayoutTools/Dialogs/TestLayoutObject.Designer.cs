namespace LayoutManager.Tools.Dialogs {
	partial class TestLayoutObject {
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
            this.panelIllustration = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxToggle = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownToggleTime = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonFailed = new System.Windows.Forms.Button();
            this.radioButtonOn = new System.Windows.Forms.RadioButton();
            this.buttonPassed = new System.Windows.Forms.Button();
            this.checkBoxReverseLogic = new System.Windows.Forms.CheckBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.radioButtonOff = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToggleTime)).BeginInit();
            this.SuspendLayout();
            // 
            // panelIllustration
            // 
            this.panelIllustration.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelIllustration.Location = new System.Drawing.Point(11, 63);
            this.panelIllustration.Name = "panelIllustration";
            this.panelIllustration.Size = new System.Drawing.Size(64, 64);
            this.panelIllustration.TabIndex = 1;
            this.panelIllustration.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelIllustration_Paint);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxToggle);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numericUpDownToggleTime);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(11, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 44);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // checkBoxToggle
            // 
            this.checkBoxToggle.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxToggle.AutoSize = true;
            this.checkBoxToggle.Location = new System.Drawing.Point(8, 13);
            this.checkBoxToggle.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.checkBoxToggle.Name = "checkBoxToggle";
            this.checkBoxToggle.Size = new System.Drawing.Size(96, 42);
            this.checkBoxToggle.TabIndex = 0;
            this.checkBoxToggle.Text = "&Toggle";
            this.checkBoxToggle.CheckedChanged += new System.EventHandler(this.CheckBoxToggle_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(140, 18);
            this.label2.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 32);
            this.label2.TabIndex = 3;
            this.label2.Text = "seconds";
            // 
            // numericUpDownToggleTime
            // 
            this.numericUpDownToggleTime.Location = new System.Drawing.Point(98, 15);
            this.numericUpDownToggleTime.Margin = new System.Windows.Forms.Padding(0, 0, 1, 3);
            this.numericUpDownToggleTime.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numericUpDownToggleTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownToggleTime.Name = "numericUpDownToggleTime";
            this.numericUpDownToggleTime.Size = new System.Drawing.Size(40, 39);
            this.numericUpDownToggleTime.TabIndex = 2;
            this.numericUpDownToggleTime.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(61, 18);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 1, 1, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 32);
            this.label1.TabIndex = 1;
            this.label1.Text = "Every:";
            // 
            // buttonFailed
            // 
            this.buttonFailed.Location = new System.Drawing.Point(142, 134);
            this.buttonFailed.Margin = new System.Windows.Forms.Padding(3, 1, 3, 3);
            this.buttonFailed.Name = "buttonFailed";
            this.buttonFailed.Size = new System.Drawing.Size(69, 23);
            this.buttonFailed.TabIndex = 6;
            this.buttonFailed.Text = "&Failed";
            this.buttonFailed.Click += new System.EventHandler(this.ButtonFailed_Click);
            // 
            // radioButtonOn
            // 
            this.radioButtonOn.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonOn.BackColor = System.Drawing.Color.Red;
            this.radioButtonOn.Location = new System.Drawing.Point(84, 63);
            this.radioButtonOn.Margin = new System.Windows.Forms.Padding(3, 3, 3, 1);
            this.radioButtonOn.Name = "radioButtonOn";
            this.radioButtonOn.Size = new System.Drawing.Size(40, 23);
            this.radioButtonOn.TabIndex = 2;
            this.radioButtonOn.Text = "O&n";
            this.radioButtonOn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonOn.UseVisualStyleBackColor = false;
            this.radioButtonOn.Click += new System.EventHandler(this.RadioButtonOn_Click);
            // 
            // buttonPassed
            // 
            this.buttonPassed.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonPassed.Location = new System.Drawing.Point(142, 104);
            this.buttonPassed.Name = "buttonPassed";
            this.buttonPassed.Size = new System.Drawing.Size(69, 23);
            this.buttonPassed.TabIndex = 5;
            this.buttonPassed.Text = "&Passed";
            this.buttonPassed.Click += new System.EventHandler(this.ButtonPassed_Click);
            // 
            // checkBoxReverseLogic
            // 
            this.checkBoxReverseLogic.AutoSize = true;
            this.checkBoxReverseLogic.Location = new System.Drawing.Point(11, 134);
            this.checkBoxReverseLogic.Name = "checkBoxReverseLogic";
            this.checkBoxReverseLogic.Size = new System.Drawing.Size(227, 36);
            this.checkBoxReverseLogic.TabIndex = 4;
            this.checkBoxReverseLogic.Text = "Use reverse logic";
            this.checkBoxReverseLogic.CheckedChanged += new System.EventHandler(this.CheckBoxReverseLogic_CheckedChanged);
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(142, 63);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(69, 23);
            this.buttonConnect.TabIndex = 7;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.Click += new System.EventHandler(this.ButtonConnect_Click);
            // 
            // radioButtonOff
            // 
            this.radioButtonOff.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonOff.BackColor = System.Drawing.Color.LightGreen;
            this.radioButtonOff.Location = new System.Drawing.Point(84, 97);
            this.radioButtonOff.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.radioButtonOff.Name = "radioButtonOff";
            this.radioButtonOff.Size = new System.Drawing.Size(40, 24);
            this.radioButtonOff.TabIndex = 8;
            this.radioButtonOff.Text = "Off";
            this.radioButtonOff.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonOff.UseVisualStyleBackColor = false;
            this.radioButtonOff.Click += new System.EventHandler(this.RadioButtonOff_Click);
            // 
            // TestLayoutObject
            // 
            this.AcceptButton = this.buttonPassed;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(223, 168);
            this.Controls.Add(this.radioButtonOff);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.checkBoxReverseLogic);
            this.Controls.Add(this.buttonPassed);
            this.Controls.Add(this.radioButtonOn);
            this.Controls.Add(this.buttonFailed);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panelIllustration);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TestLayoutObject";
            this.Text = "TestLayoutObject";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TestLayoutObject_FormClosed);
            this.Load += new System.EventHandler(this.TestLayoutObject_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToggleTime)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel panelIllustration;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericUpDownToggleTime;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonFailed;
		private System.Windows.Forms.CheckBox checkBoxToggle;
		private System.Windows.Forms.RadioButton radioButtonOn;
		private System.Windows.Forms.Button buttonPassed;
		private System.Windows.Forms.CheckBox checkBoxReverseLogic;
		private System.Windows.Forms.Button buttonConnect;
		private System.Windows.Forms.RadioButton radioButtonOff;
	}
}