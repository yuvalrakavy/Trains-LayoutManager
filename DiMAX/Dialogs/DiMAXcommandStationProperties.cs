using System;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager;
using LayoutManager.Model;

namespace DiMAX.Dialogs {
    /// <summary>
    /// Summary description for CentralStationProperties.
    /// </summary>
    public class DiMAXcommandlStationProperties : Form {
		private ComboBox comboBoxPort;
		private Label label1;
		private Button buttonOK;
		private Button buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		DiMAXcommandStation component;
		private LayoutManager.CommonUI.Controls.LayoutEmulationSetup layoutEmulationSetup;
		private LayoutManager.CommonUI.Controls.NameDefinition nameDefinition;
		private Button buttonCOMsettings;
		LayoutXmlInfo		xmlInfo;

		public DiMAXcommandlStationProperties(DiMAXcommandStation component)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.component = component;
			this.xmlInfo = new LayoutXmlInfo(component);

			nameDefinition.XmlInfo = this.xmlInfo;

			if(component.LayoutEmulationSupported)
				layoutEmulationSetup.Element = xmlInfo.DocumentElement;
			else
				layoutEmulationSetup.Visible = false;

            this.comboBoxPort.SelectedIndex = -1;
			this.comboBoxPort.Text = xmlInfo.DocumentElement.GetAttribute("Port");
		}

        public LayoutXmlInfo XmlInfo => xmlInfo;

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
			this.comboBoxPort = new ComboBox();
			this.label1 = new Label();
			this.buttonOK = new Button();
			this.buttonCancel = new Button();
			this.buttonCOMsettings = new Button();
			this.layoutEmulationSetup = new LayoutManager.CommonUI.Controls.LayoutEmulationSetup();
			this.nameDefinition = new LayoutManager.CommonUI.Controls.NameDefinition();
			this.SuspendLayout();
			// 
			// comboBoxPort
			// 
			this.comboBoxPort.FormattingEnabled = true;
			this.comboBoxPort.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4"});
			this.comboBoxPort.Location = new System.Drawing.Point(72, 88);
			this.comboBoxPort.Name = "comboBoxPort";
			this.comboBoxPort.Size = new System.Drawing.Size(104, 21);
			this.comboBoxPort.TabIndex = 1;
			this.comboBoxPort.Text = "COM1";
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
			// layoutEmulationSetup
			// 
			this.layoutEmulationSetup.Element = null;
			this.layoutEmulationSetup.Location = new System.Drawing.Point(4, 152);
			this.layoutEmulationSetup.Name = "layoutEmulationSetup";
			this.layoutEmulationSetup.Size = new System.Drawing.Size(338, 89);
			this.layoutEmulationSetup.TabIndex = 8;
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
			// DiMAXcommandlStationProperties
			// 
			this.AcceptButton = this.buttonOK;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(346, 280);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCOMsettings);
			this.Controls.Add(this.layoutEmulationSetup);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboBoxPort);
			this.Controls.Add(this.nameDefinition);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "DiMAXcommandlStationProperties";
			this.Text = "Massoth DiMAX Command Station Properties";
			this.ResumeLayout(false);

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

			if(!layoutEmulationSetup.ValidateInput())
				return;

			// Commit

			xmlInfo.DocumentElement.SetAttribute("Port", comboBoxPort.Text);

			if(component.LayoutEmulationSupported)
				layoutEmulationSetup.Commit();

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
