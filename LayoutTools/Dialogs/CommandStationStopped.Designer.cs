namespace LayoutManager.Tools.Dialogs {
	partial class CommandStationStopped {
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
            this.labelCommandStationName = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelReason = new System.Windows.Forms.Label();
            this.buttonPowerOn = new System.Windows.Forms.Button();
            this.buttonAbortTrips = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(66, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(207, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Power was turned off by command station:";
            // 
            // labelCommandStationName
            // 
            this.labelCommandStationName.AutoSize = true;
            this.labelCommandStationName.Location = new System.Drawing.Point(279, 12);
            this.labelCommandStationName.Name = "labelCommandStationName";
            this.labelCommandStationName.Size = new System.Drawing.Size(38, 13);
            this.labelCommandStationName.TabIndex = 1;
            this.labelCommandStationName.Text = "NAME";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::LayoutManager.Tools.Properties.Resources.warning;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(38, 40);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // labelReason
            // 
            this.labelReason.Location = new System.Drawing.Point(66, 39);
            this.labelReason.Name = "labelReason";
            this.labelReason.Size = new System.Drawing.Size(327, 49);
            this.labelReason.TabIndex = 3;
            // 
            // buttonPowerOn
            // 
            this.buttonPowerOn.Location = new System.Drawing.Point(290, 102);
            this.buttonPowerOn.Name = "buttonPowerOn";
            this.buttonPowerOn.Size = new System.Drawing.Size(103, 23);
            this.buttonPowerOn.TabIndex = 4;
            this.buttonPowerOn.Text = "&Power On!";
            this.buttonPowerOn.UseVisualStyleBackColor = true;
            this.buttonPowerOn.Click += new System.EventHandler(this.buttonPowerOn_Click);
            // 
            // buttonAbortTrips
            // 
            this.buttonAbortTrips.Location = new System.Drawing.Point(181, 102);
            this.buttonAbortTrips.Name = "buttonAbortTrips";
            this.buttonAbortTrips.Size = new System.Drawing.Size(103, 23);
            this.buttonAbortTrips.TabIndex = 6;
            this.buttonAbortTrips.Text = "&Suspend train trips";
            this.buttonAbortTrips.UseVisualStyleBackColor = true;
            this.buttonAbortTrips.Click += new System.EventHandler(this.buttonAbortTrips_Click);
            // 
            // CommandStationStopped
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 137);
            this.ControlBox = false;
            this.Controls.Add(this.buttonAbortTrips);
            this.Controls.Add(this.buttonPowerOn);
            this.Controls.Add(this.labelReason);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.labelCommandStationName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "CommandStationStopped";
            this.ShowInTaskbar = false;
            this.Text = "Command Station Alert";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CommandStationStopped_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelCommandStationName;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label labelReason;
		private System.Windows.Forms.Button buttonPowerOn;
		private System.Windows.Forms.Button buttonAbortTrips;
	}
}