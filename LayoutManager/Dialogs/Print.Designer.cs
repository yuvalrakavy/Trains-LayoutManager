namespace LayoutManager.Dialogs {
	partial class Print {
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
			this.comboBoxPrinters = new System.Windows.Forms.ComboBox();
			this.buttonProperties = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioButtonAllAreas = new System.Windows.Forms.RadioButton();
			this.radioButtonCurrentArea = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radioButtonBestFit = new System.Windows.Forms.RadioButton();
			this.radioButtonAllViews = new System.Windows.Forms.RadioButton();
			this.radioButtonActiveView = new System.Windows.Forms.RadioButton();
			this.checkBoxGridLines = new System.Windows.Forms.CheckBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
// 
// label1
// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(41, 14);
			this.label1.TabIndex = 0;
			this.label1.Text = "Printer:";
// 
// comboBoxPrinters
// 
			this.comboBoxPrinters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxPrinters.FormattingEnabled = true;
			this.comboBoxPrinters.Location = new System.Drawing.Point(61, 10);
			this.comboBoxPrinters.Name = "comboBoxPrinters";
			this.comboBoxPrinters.Size = new System.Drawing.Size(219, 21);
			this.comboBoxPrinters.TabIndex = 1;
// 
// buttonProperties
// 
			this.buttonProperties.Location = new System.Drawing.Point(287, 8);
			this.buttonProperties.Name = "buttonProperties";
			this.buttonProperties.TabIndex = 2;
			this.buttonProperties.Text = "&Properties...";
			this.buttonProperties.Click += new System.EventHandler(this.buttonProperties_Click);
// 
// groupBox1
// 
			this.groupBox1.Controls.Add(this.radioButtonAllAreas);
			this.groupBox1.Controls.Add(this.radioButtonCurrentArea);
			this.groupBox1.Location = new System.Drawing.Point(13, 38);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(127, 95);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Print:";
// 
// radioButtonAllAreas
// 
			this.radioButtonAllAreas.AutoSize = true;
			this.radioButtonAllAreas.Location = new System.Drawing.Point(7, 44);
			this.radioButtonAllAreas.Name = "radioButtonAllAreas";
			this.radioButtonAllAreas.Size = new System.Drawing.Size(61, 17);
			this.radioButtonAllAreas.TabIndex = 1;
			this.radioButtonAllAreas.Text = "All areas";
// 
// radioButtonCurrentArea
// 
			this.radioButtonCurrentArea.AutoSize = true;
			this.radioButtonCurrentArea.Checked = true;
			this.radioButtonCurrentArea.Location = new System.Drawing.Point(7, 20);
			this.radioButtonCurrentArea.Name = "radioButtonCurrentArea";
			this.radioButtonCurrentArea.Size = new System.Drawing.Size(79, 17);
			this.radioButtonCurrentArea.TabIndex = 0;
			this.radioButtonCurrentArea.TabStop = true;
			this.radioButtonCurrentArea.Text = "Current area";
// 
// groupBox2
// 
			this.groupBox2.Controls.Add(this.radioButtonBestFit);
			this.groupBox2.Controls.Add(this.radioButtonAllViews);
			this.groupBox2.Controls.Add(this.radioButtonActiveView);
			this.groupBox2.Location = new System.Drawing.Point(153, 38);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(127, 95);
			this.groupBox2.TabIndex = 4;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "In each area, print:";
// 
// radioButtonBestFit
// 
			this.radioButtonBestFit.AutoSize = true;
			this.radioButtonBestFit.Checked = true;
			this.radioButtonBestFit.Location = new System.Drawing.Point(7, 20);
			this.radioButtonBestFit.Name = "radioButtonBestFit";
			this.radioButtonBestFit.Size = new System.Drawing.Size(69, 17);
			this.radioButtonBestFit.TabIndex = 0;
			this.radioButtonBestFit.TabStop = true;
			this.radioButtonBestFit.Text = "Fit all area";
// 
// radioButtonAllViews
// 
			this.radioButtonAllViews.AutoSize = true;
			this.radioButtonAllViews.Location = new System.Drawing.Point(7, 68);
			this.radioButtonAllViews.Name = "radioButtonAllViews";
			this.radioButtonAllViews.Size = new System.Drawing.Size(62, 17);
			this.radioButtonAllViews.TabIndex = 2;
			this.radioButtonAllViews.Text = "All views";
// 
// radioButtonActiveView
// 
			this.radioButtonActiveView.AutoSize = true;
			this.radioButtonActiveView.Location = new System.Drawing.Point(7, 44);
			this.radioButtonActiveView.Name = "radioButtonActiveView";
			this.radioButtonActiveView.Size = new System.Drawing.Size(76, 17);
			this.radioButtonActiveView.TabIndex = 1;
			this.radioButtonActiveView.Text = "Active view";
// 
// checkBoxGridLines
// 
			this.checkBoxGridLines.AutoSize = true;
			this.checkBoxGridLines.Location = new System.Drawing.Point(20, 140);
			this.checkBoxGridLines.Name = "checkBoxGridLines";
			this.checkBoxGridLines.Size = new System.Drawing.Size(87, 17);
			this.checkBoxGridLines.TabIndex = 5;
			this.checkBoxGridLines.Text = "Print grid lines";
// 
// buttonOK
// 
			this.buttonOK.Location = new System.Drawing.Point(205, 171);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 6;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
// 
// buttonCancel
// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(289, 171);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "Cancel";
// 
// Print
// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(376, 206);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.checkBoxGridLines);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonProperties);
			this.Controls.Add(this.comboBoxPrinters);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "Print";
			this.ShowInTaskbar = false;
			this.Text = "Print";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxPrinters;
		private System.Windows.Forms.Button buttonProperties;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonCurrentArea;
		private System.Windows.Forms.RadioButton radioButtonAllAreas;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioButtonActiveView;
		private System.Windows.Forms.RadioButton radioButtonAllViews;
		private System.Windows.Forms.CheckBox checkBoxGridLines;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.RadioButton radioButtonBestFit;
	}
}