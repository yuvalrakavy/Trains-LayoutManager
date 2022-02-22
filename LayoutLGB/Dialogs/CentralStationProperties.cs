using LayoutManager;
using LayoutManager.Components;
using LayoutManager.Model;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LayoutLGB.Dialogs {
    /// <summary>
    /// Summary description for CentralStationProperties.
    /// </summary>
    public partial class CentralStationProperties : Form {
        const string E_ModeString = "ModeString";
        private readonly MTScentralStation component;

        public CentralStationProperties(MTScentralStation component) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.component = component;
            this.XmlInfo = new LayoutXmlInfo(component);

            nameDefinition.XmlInfo = this.XmlInfo;

            layoutEmulationSetup.Element = XmlInfo.DocumentElement;

            comboBoxPort.Text = XmlInfo.DocumentElement.GetAttribute(LayoutIOServices.A_Port);
            numericUpDownXbusID.Value = (decimal?)XmlInfo.DocumentElement.AttributeValue(A_XbusId) ?? 0;
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

                IEnumerable<IModelComponentIsCommandStation> commandStations = LayoutModel.Components<IModelComponentIsCommandStation>(LayoutPhase.All);

                foreach (IModelComponentIsCommandStation otherCommandStation in commandStations) {
                    if (otherCommandStation.NameProvider.Name == myName.Name && otherCommandStation.Id != component.Id) {
                        MessageBox.Show(this, "The name " + myName.Text + " is already used", "Invalid name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        nameDefinition.Focus();
                        return;
                    }
                }
            }
            else
                return;

            if (!layoutEmulationSetup.ValidateInput())
                return;

            // Commit

            XmlInfo.DocumentElement.SetAttribute(LayoutIOServices.A_Port, comboBoxPort.Text);
            XmlInfo.DocumentElement.SetAttributeValue(A_XbusId, (int)numericUpDownXbusID.Value);
            layoutEmulationSetup.Commit();

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
    }
}
