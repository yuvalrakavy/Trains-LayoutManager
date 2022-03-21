using LayoutManager.Components;
using LayoutManager.Model;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TrackPowerConnectorProperties.
    /// </summary>
    public partial class TrackPowerConnectorProperties : Form, ILayoutComponentPropertiesDialog {
        private readonly LayoutTrackPowerConnectorComponent component;

        public TrackPowerConnectorProperties(ModelComponent modelComponent) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.component = (LayoutTrackPowerConnectorComponent)modelComponent;
            XmlInfo = new LayoutXmlInfo(component);

            var info = new LayoutTrackPowerConnectorInfo(XmlInfo.Element);

            if (info.OptionalElement == null)
                info.Element = LayoutInfo.CreateProviderElement(XmlInfo.Element, "TrackPowerConnector", null);

            foreach (IModelComponentHasPowerOutlets componentWithPowerSources in LayoutModel.Components<IModelComponentHasPowerOutlets>(LayoutPhase.All)) {
                for (int outletIndex = 0; outletIndex < componentWithPowerSources.PowerOutlets.Count; outletIndex++)
                    comboBoxPowerSources.Items.Add(new LayoutComponentPowerOutletDescription(componentWithPowerSources, outletIndex));
            }

            // Locate and select current power source (if any)
            LayoutPowerInlet inlet = info.Inlet;

            if (inlet.IsConnected) {
                foreach (LayoutComponentPowerOutletDescription powerSourceItem in comboBoxPowerSources.Items) {
                    if (powerSourceItem.Component.Id == inlet.OutletComponentId && powerSourceItem.OutletIndex == inlet.OutletIndex) {
                        comboBoxPowerSources.SelectedItem = powerSourceItem;
                        break;
                    }
                }
            }
            else if (comboBoxPowerSources.Items.Count == 1)
                comboBoxPowerSources.SelectedItem = comboBoxPowerSources.Items[0];

            checkBoxDisplayPowerSourceName.Checked = info.Visible;

            trackGaugeSelector.Init();
            trackGaugeSelector.Value = info.TrackGauge;

            checkBoxDetectReverseLoops.Checked = info.CheckReverseLoops;
            UpdateButtons();
        }

        private void UpdateButtons() {
            buttonSettings.Enabled = checkBoxDisplayPowerSourceName.Checked;
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
            if (comboBoxPowerSources.SelectedItem == null) {
                MessageBox.Show(this, "You must specify power source name", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxPowerSources.Focus();
                return;
            }

            LayoutTrackPowerConnectorInfo info = new(XmlInfo.Element);

            LayoutPowerInlet inlet = info.Inlet;
            var powerSourceItem = (LayoutComponentPowerOutletDescription)comboBoxPowerSources.SelectedItem;

            inlet.OutletComponent = powerSourceItem.Component;
            inlet.OutletIndex = powerSourceItem.OutletIndex;

            info.Visible = checkBoxDisplayPowerSourceName.Checked;
            info.TrackGauge = trackGaugeSelector.Value;
            info.CheckReverseLoops = checkBoxDetectReverseLoops.Checked;

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CheckBoxDisplayPowerSourceName_CheckedChanged(object? sender, System.EventArgs e) {
            UpdateButtons();
        }

        private void ButtonSettings_Click(object? sender, System.EventArgs e) {
            LayoutTrackPowerConnectorInfo powerSourceNameInfo = new(XmlInfo.Element) {
                Component = component
            };

            CommonUI.Dialogs.TextProviderSettings settings = new(XmlInfo, powerSourceNameInfo);

            if (powerSourceNameInfo.OptionalElement == null)
                powerSourceNameInfo.Element = LayoutInfo.CreateProviderElement(XmlInfo.Element, "PowerSourceName", null);

            settings.ShowDialog(this);
        }
    }
}
