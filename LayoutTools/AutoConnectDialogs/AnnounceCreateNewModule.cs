using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.AutoConnectDialogs {
    /// <summary>
    /// Summary description for AnnounceCreateNewModule.
    /// </summary>
    public class AnnounceCreateNewModule : Form {
		private Label labelModuleName;
		private Label labelModuleLocation;
		private Button buttonOK;
		private Button buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public AnnounceCreateNewModule(ControlModuleType moduleType, LayoutControlModuleLocationComponent moduleLocation)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			labelModuleName.Text = Regex.Replace(labelModuleName.Text, "MODULETYPE", moduleType.Name);

			if(moduleLocation != null)
				labelModuleLocation.Text = Regex.Replace(labelModuleLocation.Text, "LOCATION", moduleLocation.Name);
			else
				labelModuleLocation.Visible = false;
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
			this.labelModuleName = new Label();
			this.labelModuleLocation = new Label();
			this.buttonOK = new Button();
			this.buttonCancel = new Button();
			this.SuspendLayout();
			// 
			// labelModuleName
			// 
			this.labelModuleName.Location = new System.Drawing.Point(8, 8);
			this.labelModuleName.Name = "labelModuleName";
			this.labelModuleName.Size = new System.Drawing.Size(288, 40);
			this.labelModuleName.TabIndex = 0;
			this.labelModuleName.Text = "A new MODULETYPE control module  is added.";
			// 
			// labelModuleLocation
			// 
			this.labelModuleLocation.Location = new System.Drawing.Point(8, 48);
			this.labelModuleLocation.Name = "labelModuleLocation";
			this.labelModuleLocation.Size = new System.Drawing.Size(288, 32);
			this.labelModuleLocation.TabIndex = 1;
			this.labelModuleLocation.Text = "This module is placed in control module location: LOCATION";
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(144, 104);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "Continue";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(224, 104);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			// 
			// AnnounceCreateNewModule
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(304, 134);
			this.ControlBox = false;
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.labelModuleLocation);
			this.Controls.Add(this.labelModuleName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "AnnounceCreateNewModule";
			this.ShowInTaskbar = false;
			this.Text = "Adding new Control Module";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.OK;
		}
	}
}
