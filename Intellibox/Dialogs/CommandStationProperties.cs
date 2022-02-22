using LayoutManager;
using LayoutManager.Model;
using System;
using System.Windows.Forms;
using System.Xml;

namespace Intellibox.Dialogs {
    /// <summary>
    /// Summary description for CentralStationProperties.
    /// </summary>
    public partial class CentralStationProperties : Form {
        const string E_ModeString = "ModeString";
        private readonly SOcollection _SOcollection;
        private readonly IntelliboxComponent _component;

        public CentralStationProperties(IntelliboxComponent component) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this._component = component;
            this.XmlInfo = new LayoutXmlInfo(component);
            this._SOcollection = new SOcollection(XmlInfo.Element);

            IntelliboxComponentInfo Info = new(component, XmlInfo.Element);

            nameDefinition.XmlInfo = this.XmlInfo;

            if (component.LayoutEmulationSupported)
                layoutEmulationSetup.Element = XmlInfo.DocumentElement;
            else
                layoutEmulationSetup.Visible = false;

            comboBoxPort.Text = Info.Port;
            textBoxPollingPeriod.Text = Info.PollingPeriod.ToString();
            textBoxSwitchingTime.Text = Info.AccessoryCommandTime.ToString();
            textBoxOperationModeDebounceCount.Text = Info.OperationModeDebounceCount.ToString();
            textBoxDesignTimeDebounceCount.Text = Info.DesignTimeDebounceCount.ToString();

            foreach (SOinfo so in _SOcollection)
                listViewSO.Items.Add(new SOitem(so));

            UpdateButtons();
        }

        private void UpdateButtons() {
            if (listViewSO.SelectedItems.Count == 0) {
                buttonSOdelete.Enabled = false;
                buttonSOedit.Enabled = false;
            }
            else {
                buttonSOdelete.Enabled = true;
                buttonSOedit.Enabled = true;
            }
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
                LayoutTextInfo myName = new(XmlInfo.DocumentElement, "Name");

                foreach (IModelComponentIsCommandStation otherCommandStation in LayoutModel.Components<IModelComponentIsCommandStation>(LayoutPhase.All)) {
                    if (otherCommandStation.NameProvider.Name == myName.Name && otherCommandStation.Id != _component.Id) {
                        MessageBox.Show(this, "The name " + myName.Text + " is already used", "Invalid name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        nameDefinition.Focus();
                        return;
                    }
                }
            }
            else
                return;

            if (_component.LayoutEmulationSupported) {
                if (!layoutEmulationSetup.ValidateInput())
                    return;
            }

            IntelliboxComponentInfo info = new(_component, XmlInfo.Element);

            if (!int.TryParse(textBoxPollingPeriod.Text, out int pollingPeriod)) {
                MessageBox.Show(this, "Invalid number", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxPollingPeriod.Focus();
                return;
            }

            if (!int.TryParse(textBoxSwitchingTime.Text, out int switchingTime)) {
                MessageBox.Show(this, "Invalid number", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxSwitchingTime.Focus();
                return;
            }

            if (!byte.TryParse(textBoxOperationModeDebounceCount.Text, out byte operationDebounceCount)) {
                MessageBox.Show(this, "Invalid number", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxOperationModeDebounceCount.Focus();
                return;
            }

            if (!byte.TryParse(textBoxDesignTimeDebounceCount.Text, out byte designTimeDebounceCount)) {
                MessageBox.Show(this, "Invalid number", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxDesignTimeDebounceCount.Focus();
                return;
            }

            // Commit
            info.PollingPeriod = pollingPeriod;
            info.AccessoryCommandTime = switchingTime;
            info.OperationModeDebounceCount = operationDebounceCount;
            info.DesignTimeDebounceCount = designTimeDebounceCount;
            info.Port = comboBoxPort.Text;

            if (_component.LayoutEmulationSupported)
                layoutEmulationSetup.Commit();

            DialogResult = DialogResult.OK;
        }

        private void ButtonSettings_Click(object? sender, EventArgs e) {
            string modeString = XmlInfo.DocumentElement[E_ModeString]?.InnerText ?? String.Empty;

            var d = new LayoutManager.CommonUI.Dialogs.SerialInterfaceParameters(modeString);

            if (XmlInfo.DocumentElement[E_ModeString] == null) {
                var modeStringElement = XmlInfo.XmlDocument.CreateElement(E_ModeString);
                XmlInfo.DocumentElement.AppendChild(modeStringElement);
            }

            if (d.ShowDialog(this) == DialogResult.OK)
                XmlInfo.DocumentElement[E_ModeString]!.InnerText = d.ModeString;
        }

        #region SOitem

        private class SOitem : ListViewItem {
            public SOitem(SOinfo so) {
                SOinfo = so;
                SubItems.Add("");
                SubItems.Add("");
                Update();
            }

            public void Update() {
                SubItems[0].Text = SOinfo.Number.ToString();
                SubItems[1].Text = SOinfo.Value.ToString();
                SubItems[2].Text = SOinfo.Description;
            }

            public SOinfo SOinfo { get; }
        }

        #endregion

        private void ButtonSOadd_Click(object? sender, EventArgs e) {
            SOinfo so = new();

            using Dialogs.SOdefinition d = new(so);
            if (d.ShowDialog(this) == DialogResult.OK)
                listViewSO.Items.Add(new SOitem(_SOcollection.Add(so)));
            UpdateButtons();
        }

        private void ButtonSOedit_Click(object? sender, EventArgs e) {
            if (listViewSO.SelectedItems.Count > 0) {
                SOitem item = (SOitem)listViewSO.SelectedItems[0];

                // Make a temporary copy
                var doc = Ensure.NotNull<XmlDocument>(LayoutXmlInfo.XmlImplementation.CreateDocument());
                doc.AppendChild(doc.ImportNode(item.SOinfo.Element, true));

                SOinfo tempSOinfo = new(doc.DocumentElement!);

                var d = new Dialogs.SOdefinition(tempSOinfo);
                if (d.ShowDialog(this) == DialogResult.OK) {
                    XmlElement oldElement = item.SOinfo.Element;

                    item.SOinfo.Element = (XmlElement)oldElement.OwnerDocument.ImportNode(tempSOinfo.Element, true);
                    oldElement.ParentNode?.ReplaceChild(item.SOinfo.Element, oldElement);
                    item.Update();
                }
            }
            UpdateButtons();
        }

        private void ButtonSOdelete_Click(object? sender, EventArgs e) {
            SOitem[] deletedItems = new SOitem[listViewSO.SelectedItems.Count];

            listViewSO.SelectedItems.CopyTo(deletedItems, 0);
            foreach (SOitem item in deletedItems) {
                _SOcollection.Remove(item.SOinfo);
                listViewSO.Items.Remove(item);
            }

            UpdateButtons();
        }

        private void ListViewSO_DoubleClick(object? sender, EventArgs e) {
            if (listViewSO.SelectedItems.Count > 0)
                buttonSOedit.PerformClick();
        }

        private void ListViewSO_SelectedIndexChanged(object? sender, EventArgs e) {
            UpdateButtons();
        }
    }
}
