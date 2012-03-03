using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

namespace GetMarklinCatalog
{
	/// <summary>
	/// Summary description for EditTranslation.
	/// </summary>
	public class EditTranslation : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxFrom;
		private System.Windows.Forms.TextBox textBoxTo;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		XmlElement	translateElement;

		public EditTranslation(XmlElement translateElement)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.translateElement = translateElement;
			if(translateElement.HasAttribute("From"))
				textBoxFrom.Text = translateElement.GetAttribute("From");
			if(translateElement.HasAttribute("To"))
				textBoxTo.Text = translateElement.GetAttribute("To");
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
			this.textBoxFrom = new System.Windows.Forms.TextBox();
			this.textBoxTo = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "From:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxFrom
			// 
			this.textBoxFrom.Location = new System.Drawing.Point(72, 16);
			this.textBoxFrom.Name = "textBoxFrom";
			this.textBoxFrom.Size = new System.Drawing.Size(208, 20);
			this.textBoxFrom.TabIndex = 1;
			this.textBoxFrom.Text = "";
			// 
			// textBoxTo
			// 
			this.textBoxTo.Location = new System.Drawing.Point(72, 48);
			this.textBoxTo.Name = "textBoxTo";
			this.textBoxTo.Size = new System.Drawing.Size(208, 20);
			this.textBoxTo.TabIndex = 1;
			this.textBoxTo.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 16);
			this.label2.TabIndex = 0;
			this.label2.Text = "To:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(128, 80);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(208, 80);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// EditTranslation
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(292, 110);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonOK,
																		  this.textBoxFrom,
																		  this.label1,
																		  this.textBoxTo,
																		  this.label2,
																		  this.buttonCancel});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "EditTranslation";
			this.ShowInTaskbar = false;
			this.Text = "EditTranslation";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e) {
			if(textBoxFrom.Text.Trim() == "" || textBoxTo.Text.Trim() == "") {
				MessageBox.Show(this, "Value cannot be empty", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			translateElement.SetAttribute("From", textBoxFrom.Text);
			translateElement.SetAttribute("To", textBoxTo.Text);
			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
