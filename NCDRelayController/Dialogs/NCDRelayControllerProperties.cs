using LayoutManager;
using LayoutManager.Model;
using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace NCDRelayController.Dialogs {
    /// <summary>
    /// Summary description for CentralStationProperties.
    /// </summary>
    public partial class NCDRelayControllerProperties : Form {
        private const string A_InterfaceType = "InterfaceType";
        private const string A_Port = "Port";
        private const string A_Address = "Address";
        private const string A_PollingPeriod = "PollingPeriod";
        private const string E_ModeString = "ModeString";

        private readonly NCDRelayController component;

        public NCDRelayControllerProperties(NCDRelayController component) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            foreach (var port in SerialPort.GetPortNames())
                comboBoxPort.Items.Add(port);

            this.component = component;
            this.XmlInfo = new LayoutXmlInfo(component);

            var interfaceType = XmlInfo.DocumentElement.AttributeValue(A_InterfaceType).Enum<InterfaceType>() ?? InterfaceType.Serial;

            if (interfaceType == InterfaceType.Serial)
                radioButtonSerial.Checked = true;
            else
                radioButtonTCP.Checked = true;

            nameDefinition.XmlInfo = this.XmlInfo;
            comboBoxPort.Text = XmlInfo.DocumentElement.GetAttribute(A_Port);

            if (XmlInfo.DocumentElement.HasAttribute(A_Address))
                textBoxAddress.Text = XmlInfo.DocumentElement.GetAttribute(A_Address);

            if (XmlInfo.DocumentElement.HasAttribute(A_PollingPeriod))
                textBoxPollingPeriod.Text = XmlInfo.DocumentElement.GetAttribute(A_PollingPeriod);

            RadioButtonInterfaceType_CheckedChanged(null, EventArgs.Empty);
        }

        public LayoutXmlInfo XmlInfo { get; }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void ButtonOK_Click(object? sender, System.EventArgs e) {
            if (nameDefinition.Commit()) {
                var myName = new LayoutTextInfo(XmlInfo.DocumentElement, "Name");

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
                XmlInfo.DocumentElement.SetAttributeValue(A_InterfaceType, InterfaceType.Serial);
                XmlInfo.DocumentElement.SetAttribute(A_Port, comboBoxPort.Text);
            }
            else {
                XmlInfo.DocumentElement.SetAttributeValue(A_InterfaceType, InterfaceType.TCP);
                XmlInfo.DocumentElement.SetAttribute(A_Address, textBoxAddress.Text);
            }

            XmlInfo.DocumentElement.SetAttributeValue(A_PollingPeriod, pollingPeriod);
            DialogResult = DialogResult.OK;
        }

        private void ButtonCOMsettings_Click(object? sender, EventArgs e) {
            string modeString = XmlInfo.DocumentElement[E_ModeString]?.InnerText ?? String.Empty;

            var d = new LayoutManager.CommonUI.Dialogs.SerialInterfaceParameters(modeString);

            if (XmlInfo.DocumentElement[E_ModeString] == null) {
                var modeStringElement = XmlInfo.XmlDocument.CreateElement(E_ModeString);
                XmlInfo.DocumentElement.AppendChild(modeStringElement);
            }

            if (d.ShowDialog(this) == DialogResult.OK)
                XmlInfo.DocumentElement[E_ModeString]!.InnerText = d.ModeString;
        }

        private void RadioButtonInterfaceType_CheckedChanged(object? sender, EventArgs e) {
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

        private void ButtonIpSettings_Click(object? sender, EventArgs e) {
            if (!string.IsNullOrWhiteSpace(textBoxAddress.Text)) {
                string url = "http://" + textBoxAddress.Text;

                System.Diagnostics.Process.Start(url);
            }
            else
                MessageBox.Show(this, "No valid IP address was entered", "No address was entered", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ButtonBrowseForDigiDevices_Click(object? sender, EventArgs e) {
            var d = new Dialogs.DiscoverDigiDevicesDialog();

            if (d.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                textBoxAddress.Text = d.SelectedAddress;
        }
    }
}
