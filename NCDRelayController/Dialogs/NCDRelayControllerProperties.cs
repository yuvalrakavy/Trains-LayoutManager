using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.IO.Ports;

using LayoutManager;
using LayoutManager.Model;

namespace NCDRelayController.Dialogs
{
	/// <summary>
	/// Summary description for CentralStationProperties.
	/// </summary>
	public class NCDRelayControllerProperties : System.Windows.Forms.Form {
		private System.Windows.Forms.ComboBox comboBoxPort;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		NCDRelayController component;
		private LayoutManager.CommonUI.Controls.NameDefinition nameDefinition;
		private Button buttonCOMsettings;
		private Label label2;
		private TextBox textBoxPollingPeriod;
		LayoutXmlInfo		xmlInfo;

		public NCDRelayControllerProperties(NCDRelayController component)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			foreach(var port in SerialPort.GetPortNames())
				comboBoxPort.Items.Add(port);

			this.component = component;
			this.xmlInfo = new LayoutXmlInfo(component);

			nameDefinition.XmlInfo = this.xmlInfo;
			comboBoxPort.Text = xmlInfo.DocumentElement.GetAttribute("Port");

			if(xmlInfo.DocumentElement.HasAttribute("PollingPeriod"))
				textBoxPollingPeriod.Text = xmlInfo.DocumentElement.GetAttribute("PollingPeriod");
		}

		public LayoutXmlInfo XmlInfo {
			get {
				return xmlInfo;
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
			this.comboBoxPort = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonCOMsettings = new System.Windows.Forms.Button();
			this.nameDefinition = new LayoutManager.CommonUI.Controls.NameDefinition();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxPollingPeriod = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// comboBoxPort
			// 
			this.comboBoxPort.FormattingEnabled = true;
			this.comboBoxPort.Location = new System.Drawing.Point(72, 88);
			this.comboBoxPort.Name = "comboBoxPort";
			this.comboBoxPort.Size = new System.Drawing.Size(104, 21);
			this.comboBoxPort.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 88);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Port:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(184, 248);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 6;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(264, 248);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "Cancel";
			// 
			// buttonCOMsettings
			// 
			this.buttonCOMsettings.Location = new System.Drawing.Point(183, 88);
			this.buttonCOMsettings.Name = "buttonCOMsettings";
			this.buttonCOMsettings.Size = new System.Drawing.Size(75, 21);
			this.buttonCOMsettings.TabIndex = 9;
			this.buttonCOMsettings.Text = "Settings...";
			this.buttonCOMsettings.Click += new System.EventHandler(this.buttonCOMsettings_Click);
			// 
			// nameDefinition
			// 
			this.nameDefinition.Component = null;
			this.nameDefinition.DefaultIsVisible = true;
			this.nameDefinition.ElementName = "Name";
			this.nameDefinition.IsOptional = false;
			this.nameDefinition.Location = new System.Drawing.Point(8, 8);
			this.nameDefinition.Name = "nameDefinition";
			this.nameDefinition.Size = new System.Drawing.Size(312, 64);
			this.nameDefinition.TabIndex = 0;
			this.nameDefinition.XmlInfo = null;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(16, 129);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(215, 13);
			this.label2.TabIndex = 10;
			this.label2.Text = "Contact closure polling period (millseconds): ";
			// 
			// textBoxPollingPeriod
			// 
			this.textBoxPollingPeriod.Location = new System.Drawing.Point(234, 126);
			this.textBoxPollingPeriod.Name = "textBoxPollingPeriod";
			this.textBoxPollingPeriod.Size = new System.Drawing.Size(77, 20);
			this.textBoxPollingPeriod.TabIndex = 11;
			this.textBoxPollingPeriod.Text = "500";
			// 
			// NCDRelayControllerProperties
			// 
			this.AcceptButton = this.buttonOK;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(346, 280);
			this.ControlBox = false;
			this.Controls.Add(this.textBoxPollingPeriod);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.buttonCOMsettings);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboBoxPort);
			this.Controls.Add(this.nameDefinition);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "NCDRelayControllerProperties";
			this.Text = "NCD Relay Controller Properties";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e) {
			if(nameDefinition.Commit()) {
				LayoutTextInfo			myName = new LayoutTextInfo(xmlInfo.DocumentElement, "Name");

				foreach(IModelComponentIsCommandStation otherCommandStation in LayoutModel.Components<IModelComponentIsCommandStation>(LayoutPhase.All)) {
					if(otherCommandStation.NameProvider.Name == myName.Name && otherCommandStation.Id != component.Id) {
						MessageBox.Show(this, "The name " + myName.Text + " is already used", "Invalid name", MessageBoxButtons.OK, MessageBoxIcon.Error);
						nameDefinition.Focus();
						return;
					}
				}
			}
			else
				return;

			int pollingPeriod;

			if(!int.TryParse(textBoxPollingPeriod.Text, out pollingPeriod)) {
				MessageBox.Show(this, "Invalid contact closure polling period", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBoxPollingPeriod.Focus();
				return;
			}

			// Commit

			xmlInfo.DocumentElement.SetAttribute("Port", comboBoxPort.Text);
			xmlInfo.DocumentElement.SetAttribute("PollingPeriod", XmlConvert.ToString(pollingPeriod));
			DialogResult = DialogResult.OK;
		}

		private void buttonCOMsettings_Click(object sender, EventArgs e)
		{
			string modeString = xmlInfo.DocumentElement["ModeString"].InnerText;

			LayoutManager.CommonUI.Dialogs.SerialInterfaceParameters d = new LayoutManager.CommonUI.Dialogs.SerialInterfaceParameters(modeString);

			if(d.ShowDialog(this) == DialogResult.OK)
				xmlInfo.DocumentElement["ModeString"].InnerText = d.ModeString;

		}
	}
}
