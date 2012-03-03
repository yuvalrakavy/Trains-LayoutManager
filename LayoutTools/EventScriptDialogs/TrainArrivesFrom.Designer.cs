namespace LayoutManager.Tools.EventScriptDialogs {
	partial class TrainArrivesFrom {
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
			this.radioButtonLeft = new System.Windows.Forms.RadioButton();
			this.radioButtonRight = new System.Windows.Forms.RadioButton();
			this.radioButtonTop = new System.Windows.Forms.RadioButton();
			this.radioButtonBottom = new System.Windows.Forms.RadioButton();
			this.panelArrow = new System.Windows.Forms.Panel();
			this.labelCondition = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// radioButtonLeft
			// 
			this.radioButtonLeft.AutoSize = true;
			this.radioButtonLeft.Location = new System.Drawing.Point(10, 57);
			this.radioButtonLeft.Name = "radioButtonLeft";
			this.radioButtonLeft.Size = new System.Drawing.Size(14, 13);
			this.radioButtonLeft.TabIndex = 1;
			this.radioButtonLeft.TabStop = true;
			this.radioButtonLeft.UseVisualStyleBackColor = true;
			this.radioButtonLeft.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
			// 
			// radioButtonRight
			// 
			this.radioButtonRight.AutoSize = true;
			this.radioButtonRight.Location = new System.Drawing.Point(100, 57);
			this.radioButtonRight.Name = "radioButtonRight";
			this.radioButtonRight.Size = new System.Drawing.Size(14, 13);
			this.radioButtonRight.TabIndex = 2;
			this.radioButtonRight.TabStop = true;
			this.radioButtonRight.UseVisualStyleBackColor = true;
			this.radioButtonRight.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
			// 
			// radioButtonTop
			// 
			this.radioButtonTop.AutoSize = true;
			this.radioButtonTop.Location = new System.Drawing.Point(55, 12);
			this.radioButtonTop.Name = "radioButtonTop";
			this.radioButtonTop.Size = new System.Drawing.Size(14, 13);
			this.radioButtonTop.TabIndex = 0;
			this.radioButtonTop.TabStop = true;
			this.radioButtonTop.UseVisualStyleBackColor = true;
			this.radioButtonTop.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
			// 
			// radioButtonBottom
			// 
			this.radioButtonBottom.AutoSize = true;
			this.radioButtonBottom.Location = new System.Drawing.Point(55, 100);
			this.radioButtonBottom.Name = "radioButtonBottom";
			this.radioButtonBottom.Size = new System.Drawing.Size(14, 13);
			this.radioButtonBottom.TabIndex = 3;
			this.radioButtonBottom.TabStop = true;
			this.radioButtonBottom.UseVisualStyleBackColor = true;
			this.radioButtonBottom.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
			// 
			// panelArrow
			// 
			this.panelArrow.Location = new System.Drawing.Point(30, 31);
			this.panelArrow.Name = "panelArrow";
			this.panelArrow.Size = new System.Drawing.Size(64, 64);
			this.panelArrow.TabIndex = 4;
			this.panelArrow.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelArrow_MouseClick);
			this.panelArrow.Paint += new System.Windows.Forms.PaintEventHandler(this.panelArrow_Paint);
			// 
			// labelCondition
			// 
			this.labelCondition.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelCondition.Location = new System.Drawing.Point(10, 119);
			this.labelCondition.Name = "labelCondition";
			this.labelCondition.Size = new System.Drawing.Size(195, 19);
			this.labelCondition.TabIndex = 5;
			this.labelCondition.Text = "If train arrives from";
			this.labelCondition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(130, 41);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(130, 12);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 6;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// TrainArrivesFrom
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(217, 146);
			this.ControlBox = false;
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.labelCondition);
			this.Controls.Add(this.panelArrow);
			this.Controls.Add(this.radioButtonBottom);
			this.Controls.Add(this.radioButtonTop);
			this.Controls.Add(this.radioButtonRight);
			this.Controls.Add(this.radioButtonLeft);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "TrainArrivesFrom";
			this.ShowInTaskbar = false;
			this.Text = "If Train Arrives From";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton radioButtonLeft;
		private System.Windows.Forms.RadioButton radioButtonRight;
		private System.Windows.Forms.RadioButton radioButtonTop;
		private System.Windows.Forms.RadioButton radioButtonBottom;
		private System.Windows.Forms.Panel panelArrow;
		private System.Windows.Forms.Label labelCondition;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
	}
}