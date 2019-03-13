using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.IO.Ports;

using LayoutManager;
using LayoutManager.Model;

namespace NCDRelayController.Dialogs {
    /// <summary>
    /// Summary description for CentralStationProperties.
    /// </summary>
    public class NCDRelayControllerProperties : Form {
        private ComboBox comboBoxPort;
        private Label label1;
        private Button buttonOK;
        private Button buttonCancel;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
        readonly NCDRelayController component;
        private LayoutManager.CommonUI.Controls.NameDefinition nameDefinition;
        private Button buttonCOMsettings;
        private Label label2;
        private TextBox textBoxPollingPeriod;
        private GroupBox groupBox1;
        private TextBox textBoxAddress;
        private RadioButton radioButtonTCP;
        private RadioButton radioButtonSerial;
        private Label label3;
        private Button buttonIpSettings;
        private Button buttonBrowseForDigiDevices;
        readonly LayoutXmlInfo xmlInfo;

        public NCDRelayControllerProperties(NCDRelayController component) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            foreach (var port in SerialPort.GetPortNames())
                comboBoxPort.Items.Add(port);

            this.component = component;
            this.xmlInfo = new LayoutXmlInfo(component);

            InterfaceType interfaceType = InterfaceType.Serial;

            if (xmlInfo.DocumentElement.HasAttribute("InterfaceType"))
                interfaceType = (InterfaceType)Enum.Parse(typeof(InterfaceType), xmlInfo.DocumentElement.GetAttribute("InterfaceType"));

            if (interfaceType == InterfaceType.Serial)
                radioButtonSerial.Checked = true;
            else
                radioButtonTCP.Checked = true;

            nameDefinition.XmlInfo = this.xmlInfo;
            comboBoxPort.Text = xmlInfo.DocumentElement.GetAttribute("Port");

            if (xmlInfo.DocumentElement.HasAttribute("Address"))
                textBoxAddress.Text = xmlInfo.DocumentElement.GetAttribute("Address");

            if (xmlInfo.DocumentElement.HasAttribute("PollingPeriod"))
                textBoxPollingPeriod.Text = xmlInfo.DocumentElement.GetAttribute("PollingPeriod");

            radioButtonInterfaceType_CheckedChanged(null, new EventArgs());
        }

        public LayoutXmlInfo XmlInfo => xmlInfo;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.comboBoxPort = new ComboBox();
            this.label1 = new Label();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.buttonCOMsettings = new Button();
            this.nameDefinition = new LayoutManager.CommonUI.Controls.NameDefinition();
            this.label2 = new Label();
            this.textBoxPollingPeriod = new TextBox();
            this.groupBox1 = new GroupBox();
            this.textBoxAddress = new TextBox();
            this.radioButtonTCP = new RadioButton();
            this.radioButtonSerial = new RadioButton();
            this.label3 = new Label();
            this.buttonBrowseForDigiDevices = new Button();
            this.buttonIpSettings = new Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxPort
            // 
            this.comboBoxPort.FormattingEnabled = true;
            this.comboBoxPort.Location = new System.Drawing.Point(61, 40);
            this.comboBoxPort.Name = "comboBoxPort";
            this.comboBoxPort.Size = new System.Drawing.Size(104, 21);
            this.comboBoxPort.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(33, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Port:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(225, 245);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(305, 245);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            // 
            // buttonCOMsettings
            // 
            this.buttonCOMsettings.Location = new System.Drawing.Point(172, 40);
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
            this.label2.Location = new System.Drawing.Point(25, 212);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(215, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Contact closure polling period (millseconds): ";
            // 
            // textBoxPollingPeriod
            // 
            this.textBoxPollingPeriod.Location = new System.Drawing.Point(264, 209);
            this.textBoxPollingPeriod.Name = "textBoxPollingPeriod";
            this.textBoxPollingPeriod.Size = new System.Drawing.Size(56, 20);
            this.textBoxPollingPeriod.TabIndex = 11;
            this.textBoxPollingPeriod.Text = "500";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonIpSettings);
            this.groupBox1.Controls.Add(this.buttonBrowseForDigiDevices);
            this.groupBox1.Controls.Add(this.textBoxAddress);
            this.groupBox1.Controls.Add(this.radioButtonTCP);
            this.groupBox1.Controls.Add(this.radioButtonSerial);
            this.groupBox1.Controls.Add(this.buttonCOMsettings);
            this.groupBox1.Controls.Add(this.comboBoxPort);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(8, 69);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(372, 134);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Interface:";
            // 
            // textBoxAddress
            // 
            this.textBoxAddress.Location = new System.Drawing.Point(83, 92);
            this.textBoxAddress.Name = "textBoxAddress";
            this.textBoxAddress.Size = new System.Drawing.Size(164, 20);
            this.textBoxAddress.TabIndex = 11;
            // 
            // radioButtonTCP
            // 
            this.radioButtonTCP.AutoSize = true;
            this.radioButtonTCP.Location = new System.Drawing.Point(16, 76);
            this.radioButtonTCP.Name = "radioButtonTCP";
            this.radioButtonTCP.Size = new System.Drawing.Size(61, 17);
            this.radioButtonTCP.TabIndex = 10;
            this.radioButtonTCP.TabStop = true;
            this.radioButtonTCP.Text = "TCP/IP";
            this.radioButtonTCP.UseVisualStyleBackColor = true;
            this.radioButtonTCP.CheckedChanged += new System.EventHandler(this.radioButtonInterfaceType_CheckedChanged);
            // 
            // radioButtonSerial
            // 
            this.radioButtonSerial.AutoSize = true;
            this.radioButtonSerial.Location = new System.Drawing.Point(16, 19);
            this.radioButtonSerial.Name = "radioButtonSerial";
            this.radioButtonSerial.Size = new System.Drawing.Size(241, 17);
            this.radioButtonSerial.TabIndex = 0;
            this.radioButtonSerial.TabStop = true;
            this.radioButtonSerial.Text = "Serial (RS232, USB, Network using RealPort)l";
            this.radioButtonSerial.UseVisualStyleBackColor = true;
            this.radioButtonSerial.CheckedChanged += new System.EventHandler(this.radioButtonInterfaceType_CheckedChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(33, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Address:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonBrowseForDigiDevices
            // 
            this.buttonBrowseForDigiDevices.Location = new System.Drawing.Point(250, 89);
            this.buttonBrowseForDigiDevices.Name = "buttonBrowseForDigiDevices";
            this.buttonBrowseForDigiDevices.Size = new System.Drawing.Size(28, 23);
            this.buttonBrowseForDigiDevices.TabIndex = 12;
            this.buttonBrowseForDigiDevices.Text = "...";
            this.buttonBrowseForDigiDevices.UseVisualStyleBackColor = true;
            this.buttonBrowseForDigiDevices.Click += new System.EventHandler(this.buttonBrowseForDigiDevices_Click);
            // 
            // buttonIpSettings
            // 
            this.buttonIpSettings.Location = new System.Drawing.Point(281, 91);
            this.buttonIpSettings.Name = "buttonIpSettings";
            this.buttonIpSettings.Size = new System.Drawing.Size(75, 21);
            this.buttonIpSettings.TabIndex = 13;
            this.buttonIpSettings.Text = "Settings...";
            this.buttonIpSettings.Click += new System.EventHandler(this.buttonIpSettings_Click);
            // 
            // NCDRelayControllerProperties
            // 
            this.AcceptButton = this.buttonOK;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(392, 280);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxPollingPeriod);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.nameDefinition);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "NCDRelayControllerProperties";
            this.Text = "NCD Relay Controller Properties";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void buttonOK_Click(object sender, System.EventArgs e) {
            if (nameDefinition.Commit()) {
                LayoutTextInfo myName = new LayoutTextInfo(xmlInfo.DocumentElement, "Name");

                foreach (IModelComponentIsCommandStation otherCommandStation in LayoutModel.Components<IModelComponentIsCommandStation>(LayoutPhase.All)) {
                    if (otherCommandStation.NameProvider.Name == myName.Name && otherCommandStation.Id != component.Id) {
                        MessageBox.Show(this, "The name " + myName.Text + " is already used", "Invalid name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        nameDefinition.Focus();
                        return;
                    }
                }
            }
            else
                return;


            if (!int.TryParse(textBoxPollingPeriod.Text, out int pollingPeriod)) {
                MessageBox.Show(this, "Invalid contact closure polling period", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxPollingPeriod.Focus();
                return;
            }

            // Commit

            if (radioButtonSerial.Checked) {
                xmlInfo.DocumentElement.SetAttribute("InterfaceType", InterfaceType.Serial.ToString());
                xmlInfo.DocumentElement.SetAttribute("Port", comboBoxPort.Text);
            }
            else {
                xmlInfo.DocumentElement.SetAttribute("InterfaceType", InterfaceType.TCP.ToString());
                xmlInfo.DocumentElement.SetAttribute("Address", textBoxAddress.Text);
            }

            xmlInfo.DocumentElement.SetAttribute("PollingPeriod", XmlConvert.ToString(pollingPeriod));
            DialogResult = DialogResult.OK;
        }

        private void buttonCOMsettings_Click(object sender, EventArgs e) {
            string modeString = xmlInfo.DocumentElement["ModeString"].InnerText;

            LayoutManager.CommonUI.Dialogs.SerialInterfaceParameters d = new LayoutManager.CommonUI.Dialogs.SerialInterfaceParameters(modeString);

            if (d.ShowDialog(this) == DialogResult.OK)
                xmlInfo.DocumentElement["ModeString"].InnerText = d.ModeString;

        }

        private void radioButtonInterfaceType_CheckedChanged(object sender, EventArgs e) {
            if (radioButtonSerial.Checked) {
                comboBoxPort.Enabled = true;
                buttonCOMsettings.Enabled = true;
                textBoxAddress.Enabled = false;
                buttonBrowseForDigiDevices.Enabled = false;
                buttonIpSettings.Enabled = false;
            }
            else if (radioButtonTCP.Checked) {
                comboBoxPort.Enabled = false;
                buttonCOMsettings.Enabled = false;
                textBoxAddress.Enabled = true;
                buttonBrowseForDigiDevices.Enabled = true;
                buttonIpSettings.Enabled = true;
            }

        }

        private void buttonIpSettings_Click(object sender, EventArgs e) {
            if (!string.IsNullOrWhiteSpace(textBoxAddress.Text)) {
                string url = "http://" + textBoxAddress.Text;

                System.Diagnostics.Process.Start(url);
            }
            else
                MessageBox.Show(this, "No valid IP address was entered", "No address was entered", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void buttonBrowseForDigiDevices_Click(object sender, EventArgs e) {
            var d = new Dialogs.DiscoverDigiDevicesDialog();

            if (d.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                textBoxAddress.Text = d.SelectedAddress;
        }
    }
}
