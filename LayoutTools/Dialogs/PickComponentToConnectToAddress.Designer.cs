namespace LayoutManager.Tools.Dialogs {
	partial class PickComponentToConnectToAddress {
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
			this.labelClickOnComponent = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
// 
// labelClickOnComponent
// 
			this.labelClickOnComponent.AutoSize = true;
			this.labelClickOnComponent.Location = new System.Drawing.Point(4, 2);
			this.labelClickOnComponent.Name = "labelClickOnComponent";
			this.labelClickOnComponent.Size = new System.Drawing.Size(144, 27);
			this.labelClickOnComponent.TabIndex = 0;
			this.labelClickOnComponent.Text = "Click on component to \r\nconnect to BUS: ADDRESS";
// 
// buttonCancel
// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.Location = new System.Drawing.Point(49, 36);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(60, 23);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
// 
// PickComponentToConnectToAddress
// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(159, 67);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.labelClickOnComponent);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "PickComponentToConnectToAddress";
			this.Text = "Connect Component";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PickComponentToConnectToAddress_FormClosed);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelClickOnComponent;
		private System.Windows.Forms.Button buttonCancel;
	}
}