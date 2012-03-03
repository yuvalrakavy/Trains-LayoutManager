namespace LayoutManager.CommonUI.Controls {
	partial class TrainLengthDiagram {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.buttonNotLonger = new System.Windows.Forms.Button();
			this.buttonLonger = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.linkMenuTrainLength = new LayoutManager.CommonUI.Controls.LinkMenu();
			this.SuspendLayout();
			// 
			// buttonNotLonger
			// 
			this.buttonNotLonger.Location = new System.Drawing.Point(3, 30);
			this.buttonNotLonger.Name = "buttonNotLonger";
			this.buttonNotLonger.Size = new System.Drawing.Size(16, 20);
			this.buttonNotLonger.TabIndex = 1;
			this.buttonNotLonger.Text = "<";
			this.toolTip1.SetToolTip(this.buttonNotLonger, "Not longer than");
			this.buttonNotLonger.UseVisualStyleBackColor = true;
			this.buttonNotLonger.Click += new System.EventHandler(this.buttonNotLonger_Click);
			// 
			// buttonLonger
			// 
			this.buttonLonger.Location = new System.Drawing.Point(161, 30);
			this.buttonLonger.Name = "buttonLonger";
			this.buttonLonger.Size = new System.Drawing.Size(16, 20);
			this.buttonLonger.TabIndex = 2;
			this.buttonLonger.Text = ">";
			this.toolTip1.SetToolTip(this.buttonLonger, "Longer than");
			this.buttonLonger.UseVisualStyleBackColor = true;
			this.buttonLonger.Click += new System.EventHandler(this.buttonLonger_Click);
			// 
			// linkMenuTrainLength
			// 
			this.linkMenuTrainLength.Location = new System.Drawing.Point(20, 32);
			this.linkMenuTrainLength.Name = "linkMenuTrainLength";
			this.linkMenuTrainLength.Options = new string[0];
			this.linkMenuTrainLength.SelectedIndex = -1;
			this.linkMenuTrainLength.Size = new System.Drawing.Size(140, 16);
			this.linkMenuTrainLength.TabIndex = 3;
			this.linkMenuTrainLength.TabStop = true;
			this.linkMenuTrainLength.Text = "linkMenu1";
			this.linkMenuTrainLength.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.linkMenuTrainLength.ValueChanged += new System.EventHandler(this.linkMenuTrainLength_ValueChanged);
			// 
			// TrainLengthDiagram
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.linkMenuTrainLength);
			this.Controls.Add(this.buttonLonger);
			this.Controls.Add(this.buttonNotLonger);
			this.Name = "TrainLengthDiagram";
			this.Size = new System.Drawing.Size(180, 52);
			this.toolTip1.SetToolTip(this, "Click to set train length");
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonNotLonger;
		private System.Windows.Forms.Button buttonLonger;
		private System.Windows.Forms.ToolTip toolTip1;
		private LayoutManager.CommonUI.Controls.LinkMenu linkMenuTrainLength;
	}
}
