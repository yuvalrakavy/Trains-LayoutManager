namespace LayoutManager.Tools.Dialogs {
	partial class ShiftOrMove {
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
			this.numericUpDownComponents = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioButtonLeft = new System.Windows.Forms.RadioButton();
			this.radioButtonRight = new System.Windows.Forms.RadioButton();
			this.radioButtonDown = new System.Windows.Forms.RadioButton();
			this.radioButtonUp = new System.Windows.Forms.RadioButton();
			this.checkBoxFillGaps = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownComponents)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(16, 98);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(18, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "By:";
			// 
			// numericUpDownComponents
			// 
			this.numericUpDownComponents.Location = new System.Drawing.Point(40, 95);
			this.numericUpDownComponents.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownComponents.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownComponents.Name = "numericUpDownComponents";
			this.numericUpDownComponents.Size = new System.Drawing.Size(43, 20);
			this.numericUpDownComponents.TabIndex = 2;
			this.numericUpDownComponents.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(89, 98);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(62, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Components";
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(167, 12);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(167, 41);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioButtonLeft);
			this.groupBox1.Controls.Add(this.radioButtonRight);
			this.groupBox1.Controls.Add(this.radioButtonDown);
			this.groupBox1.Controls.Add(this.radioButtonUp);
			this.groupBox1.Location = new System.Drawing.Point(8, 11);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(153, 78);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Direction:";
			// 
			// radioButtonLeft
			// 
			this.radioButtonLeft.AutoSize = true;
			this.radioButtonLeft.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioButtonLeft.Location = new System.Drawing.Point(10, 31);
			this.radioButtonLeft.Name = "radioButtonLeft";
			this.radioButtonLeft.Size = new System.Drawing.Size(39, 17);
			this.radioButtonLeft.TabIndex = 1;
			this.radioButtonLeft.Text = "&Left";
			// 
			// radioButtonRight
			// 
			this.radioButtonRight.AutoSize = true;
			this.radioButtonRight.Location = new System.Drawing.Point(96, 31);
			this.radioButtonRight.Name = "radioButtonRight";
			this.radioButtonRight.Size = new System.Drawing.Size(46, 17);
			this.radioButtonRight.TabIndex = 2;
			this.radioButtonRight.Text = "&Right";
			// 
			// radioButtonDown
			// 
			this.radioButtonDown.AutoSize = true;
			this.radioButtonDown.Location = new System.Drawing.Point(66, 52);
			this.radioButtonDown.Name = "radioButtonDown";
			this.radioButtonDown.Size = new System.Drawing.Size(49, 17);
			this.radioButtonDown.TabIndex = 3;
			this.radioButtonDown.Text = "&Down";
			// 
			// radioButtonUp
			// 
			this.radioButtonUp.AutoSize = true;
			this.radioButtonUp.Location = new System.Drawing.Point(66, 13);
			this.radioButtonUp.Name = "radioButtonUp";
			this.radioButtonUp.Size = new System.Drawing.Size(35, 17);
			this.radioButtonUp.TabIndex = 0;
			this.radioButtonUp.Text = "&Up";
			// 
			// checkBoxFillGaps
			// 
			this.checkBoxFillGaps.AutoSize = true;
			this.checkBoxFillGaps.Checked = true;
			this.checkBoxFillGaps.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxFillGaps.Location = new System.Drawing.Point(17, 121);
			this.checkBoxFillGaps.Name = "checkBoxFillGaps";
			this.checkBoxFillGaps.Size = new System.Drawing.Size(60, 17);
			this.checkBoxFillGaps.TabIndex = 6;
			this.checkBoxFillGaps.Text = "Fill gaps";
			// 
			// ShiftOrMove
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(254, 147);
			this.ControlBox = false;
			this.Controls.Add(this.checkBoxFillGaps);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.numericUpDownComponents);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "ShiftOrMove";
			this.ShowInTaskbar = false;
			this.Text = "ShiftOrMove";
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownComponents)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown numericUpDownComponents;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonLeft;
		private System.Windows.Forms.RadioButton radioButtonRight;
		private System.Windows.Forms.RadioButton radioButtonDown;
		private System.Windows.Forms.RadioButton radioButtonUp;
		private System.Windows.Forms.CheckBox checkBoxFillGaps;
	}
}