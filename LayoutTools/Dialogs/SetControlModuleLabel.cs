using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs
{
	/// <summary>
	/// Summary description for SetControlModuleLabel.
	/// </summary>
	public class SetControlModuleLabel : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxLabel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SetControlModuleLabel(string label)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			if(label == null)
				textBoxLabel.Text = "";
			else
				textBoxLabel.Text = label;
		}

		public string Label {
			get {
				if(textBoxLabel.Text.Trim() == "")
					return null;
				else
					return textBoxLabel.Text;
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
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxLabel = new System.Windows.Forms.TextBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(248, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Enter a label to attach to the control module:";
			// 
			// textBoxLabel
			// 
			this.textBoxLabel.Location = new System.Drawing.Point(24, 40);
			this.textBoxLabel.Name = "textBoxLabel";
			this.textBoxLabel.Size = new System.Drawing.Size(240, 20);
			this.textBoxLabel.TabIndex = 1;
			this.textBoxLabel.Text = "";
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(109, 72);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(189, 72);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			// 
			// SetControlModuleLabel
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(272, 102);
			this.ControlBox = false;
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.textBoxLabel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "SetControlModuleLabel";
			this.ShowInTaskbar = false;
			this.Text = "Set Control Module Label";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.OK;
		}
	}
}
