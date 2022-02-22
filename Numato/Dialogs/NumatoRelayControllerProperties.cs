using LayoutManager;
using LayoutManager.Components;
using LayoutManager.Model;
using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace NumatoController.Dialogs {
    /// <summary>
    /// Summary description for CentralStationProperties.
    /// </summary>
    public partial class NumatoControllerProperties : Form {
        const string E_ModeString = "ModeString";
        private const string A_InterfaceType = "InterfaceType";
        private const string A_Port = "Port";
        private const string A_Address = "Address";

        private readonly NumatoController component;

        public NumatoControllerProperties(NumatoController component) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            foreach (var port in SerialPort.GetPortNames())
                comboBoxPort.Items.Add(port);

            this.component = component;
            this.XmlInfo = new LayoutXmlInfo(component);

            if (XmlInfo.DocumentElement.HasAttribute(NumatoController.A_Relays)) {
                var nRelays = XmlInfo.DocumentElement.GetAttribute(NumatoController.A_Relays);

                foreach (var item in comboBoxRelays.Items)
                    if ((string)item == nRelays) {
                        comboBoxRelays.SelectedItem = item;
                        break;
                    }

                labelRelaysCount.Enabled = false;
                comboBoxRelays.Enabled = false;           // Show number of relays, but it cannot be changed
            }

            InterfaceType interfaceType = InterfaceType.Serial;

            if (XmlInfo.DocumentElement.HasAttribute(LayoutBusProviderWithStreamCommunicationSupport.A_InterfaceType))
                interfaceType = (InterfaceType)Enum.Parse(typeof(InterfaceType), XmlInfo.DocumentElement.GetAttribute(LayoutBusProviderWithStreamCommunicationSupport.A_InterfaceType));

            if (interfaceType == InterfaceType.Serial)
                radioButtonSerial.Checked = true;
            else
                radioButtonTCP.Checked = true;

            nameDefinition.XmlInfo = this.XmlInfo;
            comboBoxPort.Text = XmlInfo.DocumentElement.GetAttribute(LayoutBusProviderWithStreamCommunicationSupport.A_Port);

            if (XmlInfo.DocumentElement.HasAttribute(LayoutBusProviderWithStreamCommunicationSupport.A_Address))
                textBoxAddress.Text = XmlInfo.DocumentElement.GetAttribute(LayoutBusProviderWithStreamCommunicationSupport.A_Address);

            if (XmlInfo.DocumentElement.HasAttribute(NumatoController.A_User))
                textBoxUser.Text = XmlInfo.DocumentElement.GetAttribute(NumatoController.A_User);

            if (XmlInfo.DocumentElement.HasAttribute(NumatoController.A_Password))
                textBoxPassword.Text = XmlInfo.DocumentElement.GetAttribute(NumatoController.A_Password);

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

            // Commit

            if (!int.TryParse(comboBoxRelays.SelectedItem.ToString(), out int relayCount)) {
                MessageBox.Show(this, "Select the number of relays on the module", "Missing relays count", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxRelays.Focus();
                return;
            }

            XmlInfo.DocumentElement.SetAttributeValue(NumatoController.A_Relays, relayCount);

            if (radioButtonSerial.Checked) {
                XmlInfo.DocumentElement.SetAttributeValue(A_InterfaceType, InterfaceType.Serial);
                XmlInfo.DocumentElement.SetAttribute(A_Port, comboBoxPort.Text);
            }
            else {
                XmlInfo.DocumentElement.SetAttributeValue(A_InterfaceType, InterfaceType.TCP);
                XmlInfo.DocumentElement.SetAttribute(A_Address, textBoxAddress.Text);
            }

            if (string.IsNullOrWhiteSpace(textBoxUser.Text)) {
                MessageBox.Show(this, "Missing user name", "Missing User Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxUser.Focus();
                return;
            }

            XmlInfo.DocumentElement.SetAttribute(NumatoController.A_User, textBoxUser.Text);

            if (string.IsNullOrWhiteSpace(textBoxPassword.Text)) {
                MessageBox.Show(this, "Missing password", "Missing password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxPassword.Focus();
                return;
            }

            XmlInfo.DocumentElement.SetAttribute(NumatoController.A_Password, textBoxPassword.Text);

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
                buttonIpSettings.Enabled = false;
            }
            else if (radioButtonTCP.Checked) {
                comboBoxPort.Enabled = false;
                buttonCOMsettings.Enabled = false;
                textBoxAddress.Enabled = true;
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
    }
}
