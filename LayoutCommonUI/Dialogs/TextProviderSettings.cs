using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.Model;

namespace LayoutManager.CommonUI.Dialogs
{
	/// <summary>
	/// Summary description for TextProviderSettings.
	/// </summary>
	public class TextProviderSettings : Form {
		private Button buttonOK;
		private Button buttonCancel;
		private LayoutManager.CommonUI.Controls.TextProviderFontDefinition textProviderFontDefinition1;
		private LayoutManager.CommonUI.Controls.TextProviderPositionDefinition TextProviderPositionDefinition1;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		LayoutTextInfo	textProvider;

		public TextProviderSettings(LayoutXmlInfo xmlInfo, LayoutTextInfo textProvider)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.textProvider = textProvider;

			textProviderFontDefinition1.XmlInfo = xmlInfo;
			textProviderFontDefinition1.FontProvider = textProvider.FontProvider;

			TextProviderPositionDefinition1.XmlInfo = xmlInfo;
			TextProviderPositionDefinition1.PositionProvider = textProvider.PositionProvider;

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.TextProviderPositionDefinition1 = new LayoutManager.CommonUI.Controls.TextProviderPositionDefinition();
            this.textProviderFontDefinition1 = new LayoutManager.CommonUI.Controls.TextProviderFontDefinition();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(512, 687);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(240, 55);
            this.buttonOK.TabIndex = 7;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // TextProviderPositionDefinition1
            // 
            this.TextProviderPositionDefinition1.CustomPositionElementName = "Position";
            this.TextProviderPositionDefinition1.Location = new System.Drawing.Point(13, 267);
            this.TextProviderPositionDefinition1.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.TextProviderPositionDefinition1.Name = "TextProviderPositionDefinition1";
            this.TextProviderPositionDefinition1.Size = new System.Drawing.Size(1024, 410);
            this.TextProviderPositionDefinition1.TabIndex = 10;
            this.TextProviderPositionDefinition1.XmlInfo = null;
            // 
            // textProviderFontDefinition1
            // 
            this.textProviderFontDefinition1.CustomFontElementName = "Font";
            this.textProviderFontDefinition1.Location = new System.Drawing.Point(13, 10);
            this.textProviderFontDefinition1.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.textProviderFontDefinition1.Name = "textProviderFontDefinition1";
            this.textProviderFontDefinition1.Size = new System.Drawing.Size(1024, 258);
            this.textProviderFontDefinition1.TabIndex = 9;
            this.textProviderFontDefinition1.XmlInfo = null;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(768, 687);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(240, 55);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "Cancel";
            // 
            // TextProviderSettings
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(1075, 777);
            this.ControlBox = false;
            this.Controls.Add(this.TextProviderPositionDefinition1);
            this.Controls.Add(this.textProviderFontDefinition1);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TextProviderSettings";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Appearance";
            this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e) {
			textProvider.FontElementPath = textProviderFontDefinition1.FontProvider.ElementPath;
			textProvider.PositionElementPath = TextProviderPositionDefinition1.PositionProvider.ElementPath;
			this.Close();
		}
	}
}
