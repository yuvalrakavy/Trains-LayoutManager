namespace LayoutManager.Tools.EventScriptDialogs {
	partial class IfTrainLength {
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
			this.comboBoxTrain = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.trainLengthDiagram = new LayoutManager.CommonUI.Controls.TrainLengthDiagram();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// comboBoxTrain
			// 
			this.comboBoxTrain.Items.AddRange(new object[] {
            "Train",
            "Script:Train"});
			this.comboBoxTrain.Location = new System.Drawing.Point(134, 8);
			this.comboBoxTrain.Name = "comboBoxTrain";
			this.comboBoxTrain.Size = new System.Drawing.Size(160, 21);
			this.comboBoxTrain.TabIndex = 1;
			this.comboBoxTrain.Text = "Train";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(-2, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(128, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Train defined by symbol: ";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// trainLengthDiagram
			// 
			this.trainLengthDiagram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.trainLengthDiagram.Comparison = LayoutManager.Model.TrainLengthComparison.NotLonger;
			this.trainLengthDiagram.Length = LayoutManager.Model.TrainLength.Standard;
			this.trainLengthDiagram.Location = new System.Drawing.Point(12, 46);
			this.trainLengthDiagram.Name = "trainLengthDiagram";
			this.trainLengthDiagram.Size = new System.Drawing.Size(180, 52);
			this.trainLengthDiagram.TabIndex = 2;
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(217, 46);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 3;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.ButtonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(217, 75);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 4;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// IfTrainLength
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(304, 112);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.trainLengthDiagram);
			this.Controls.Add(this.comboBoxTrain);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "IfTrainLength";
			this.Text = "If Train Length";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ComboBox comboBoxTrain;
		private System.Windows.Forms.Label label1;
		private LayoutManager.CommonUI.Controls.TrainLengthDiagram trainLengthDiagram;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
	}
}