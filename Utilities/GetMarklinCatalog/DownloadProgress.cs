using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace GetMarklinCatalog
{
	/// <summary>
	/// Summary description for DownloadProgress.
	/// </summary>
	public class DownloadProgress : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label labelDescription;
		private System.Windows.Forms.Button buttonAbort;
		private System.Windows.Forms.ProgressBar progressBar;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private void endOfDesignerVariables() { }

		bool	aborted = false;

		public DownloadProgress()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

		}

		public bool Aborted {
			get {
				return aborted;
			}
		}

		public ProgressBar ProgressBar {
			get {
				return progressBar;
			}
		}

		public string Description {
			get {
				return labelDescription.Text;
			}

			set {
				labelDescription.Text = value;
			}
		}
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.labelDescription = new System.Windows.Forms.Label();
			this.buttonAbort = new System.Windows.Forms.Button();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.SuspendLayout();
			// 
			// labelDescription
			// 
			this.labelDescription.Location = new System.Drawing.Point(16, 16);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new System.Drawing.Size(264, 23);
			this.labelDescription.TabIndex = 0;
			this.labelDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// buttonAbort
			// 
			this.buttonAbort.Location = new System.Drawing.Point(112, 104);
			this.buttonAbort.Name = "buttonAbort";
			this.buttonAbort.TabIndex = 1;
			this.buttonAbort.Text = "&Abort";
			this.buttonAbort.Click += new System.EventHandler(this.buttonAbort_Click);
			// 
			// progressBar
			// 
			this.progressBar.Location = new System.Drawing.Point(8, 56);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(272, 23);
			this.progressBar.TabIndex = 2;
			// 
			// DownloadProgress
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 134);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.progressBar,
																		  this.buttonAbort,
																		  this.labelDescription});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "DownloadProgress";
			this.Text = "Downloading Catalog...";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonAbort_Click(object sender, System.EventArgs e) {
			aborted = true;
		}
	}
}
