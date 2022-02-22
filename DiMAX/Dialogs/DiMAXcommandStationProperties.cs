using LayoutManager;
using LayoutManager.Model;
using System;
using System.Windows.Forms;

namespace DiMAX.Dialogs {
    /// <summary>
    /// Summary description for CentralStationProperties.
    /// </summary>
    public partial class DiMAXcommandlStationProperties : Form {
        const string E_ModeString = "ModeString";
        private readonly DiMAXcommandStation component;

        public DiMAXcommandlStationProperties(DiMAXcommandStation component) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.component = component;
            this.XmlInfo = new LayoutXmlInfo(component);

            nameDefinition.XmlInfo = this.XmlInfo;

            if (component.LayoutEmulationSupported)
                layoutEmulationSetup.Element = XmlInfo.DocumentElement;
            else
                layoutEmulationSetup.Visible = false;

            this.comboBoxPort.SelectedIndex = -1;
            this.comboBoxPort.Text = XmlInfo.DocumentElement.GetAttribute("Port");
        }

        public LayoutXmlInfo XmlInfo { get; }

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

            if (!layoutEmulationSetup.ValidateInput())
                return;

            // Commit

            XmlInfo.DocumentElement.SetAttribute("Port", comboBoxPort.Text);

            if (component.LayoutEmulationSupported)
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
