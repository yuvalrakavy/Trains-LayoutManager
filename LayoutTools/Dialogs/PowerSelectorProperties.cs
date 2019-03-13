using System;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {

    public partial class PowerSelectorProperties : Form, ILayoutComponentPropertiesDialog {
        public PowerSelectorProperties(ModelComponent component) {
            InitializeComponent();

            this.Component = (LayoutPowerSelectorComponent)component;
            this.XmlInfo = new LayoutXmlInfo(Component);

            InitInletCombobox(comboBoxInput1, radioButtonInput1connected, radioButtonInput1disconnected, Component.Inlet1);
            InitInletCombobox(comboBoxInput2, radioButtonInput2connected, radioButtonInput2disconnected, Component.Inlet2);

            var powerSelectorNameInfo = new LayoutPowerSelectorComponent.LayoutPowerSelectorNameInfo(XmlInfo.DocumentElement);

            textBoxName.Text = powerSelectorNameInfo.Name;

            if (powerSelectorNameInfo.Element != null)
                checkBoxDisplayPowerSelectorName.Checked = powerSelectorNameInfo.Visible;
            else
                checkBoxDisplayPowerSelectorName.Checked = true;

            if (XmlInfo.Element.HasAttribute("ReverseLogic"))
                checkBoxReverseLogic.Checked = XmlConvert.ToBoolean(XmlInfo.Element.GetAttribute("ReverseLogic"));
            else
                checkBoxReverseLogic.Checked = false;

            if (!XmlInfo.Element.HasAttribute(LayoutPowerSelectorComponent.SwitchingMethodAttribute) ||
                (LayoutPowerSelectorComponent.RelaySwitchingMethod)Enum.Parse(typeof(LayoutPowerSelectorComponent.RelaySwitchingMethod), XmlInfo.Element.GetAttribute(LayoutPowerSelectorComponent.SwitchingMethodAttribute)) == LayoutPowerSelectorComponent.RelaySwitchingMethod.DPDSrelay)
                radioButtonDTDPrelay.Checked = true;
            else {
                radioButtonSTDP.Checked = true;

                if (XmlInfo.Element.HasAttribute(LayoutPowerSelectorComponent.HasOnOffRelayAttribute) && XmlConvert.ToBoolean(XmlInfo.Element.GetAttribute(LayoutPowerSelectorComponent.HasOnOffRelayAttribute)))
                    checkBoxHasOnOffRelay.Checked = true;
                else
                    checkBoxHasOnOffRelay.Checked = false;

                if (XmlInfo.Element.HasAttribute(LayoutPowerSelectorComponent.ReverseOnOffRelayAttribute) && XmlConvert.ToBoolean(XmlInfo.Element.GetAttribute(LayoutPowerSelectorComponent.ReverseOnOffRelayAttribute)))
                    checkBoxReverseOnOffRelay.Checked = true;
                else
                    checkBoxReverseOnOffRelay.Checked = false;
            }

            UpdateButtons();
        }

        private void InitInletCombobox(ComboBox comboBox, RadioButton radioButtonConnected, RadioButton radioButtonNotConnected, ILayoutPowerInlet inlet) {
            foreach (IModelComponentHasPowerOutlets componentWithPowerSources in LayoutModel.Components<IModelComponentHasPowerOutlets>(LayoutPhase.All)) {
                if (componentWithPowerSources.Id != Component.Id) {
                    for (int outletIndex = 0; outletIndex < componentWithPowerSources.PowerOutlets.Count; outletIndex++)
                        comboBox.Items.Add(new LayoutComponentPowerOutletDescription(componentWithPowerSources, outletIndex));
                }
            }

            if (inlet.IsConnected) {
                radioButtonConnected.Checked = true;

                foreach (LayoutComponentPowerOutletDescription powerSourceItem in comboBox.Items) {
                    if (powerSourceItem.Component.Id == inlet.OutletComponentId && powerSourceItem.OutletIndex == inlet.OutletIndex) {
                        comboBox.SelectedItem = powerSourceItem;
                        break;
                    }
                }
            }
            else
                radioButtonNotConnected.Checked = true;
        }

        private bool IsPowerSelector() => radioButtonInput1connected.Checked && radioButtonInput2connected.Checked;

        private void ApplyInletModification(ComboBox comboBox, RadioButton radioButtonConnected, RadioButton radioButtonNotConnected, ILayoutPowerInlet inlet) {
            if (radioButtonNotConnected.Checked || comboBox.SelectedItem == null)
                inlet.OutletComponentId = Guid.Empty;
            else {
                LayoutComponentPowerOutletDescription selectedOutlet = (LayoutComponentPowerOutletDescription)comboBox.SelectedItem;

                inlet.OutletComponentId = selectedOutlet.Component.Id;
                inlet.OutletIndex = selectedOutlet.OutletIndex;
            }
        }

        LayoutPowerSelectorComponent Component { get; }

        #region ILayoutComponentPropertiesDialog Members

        public LayoutXmlInfo XmlInfo {
            get;

        }

        #endregion

        private void UpdateButtons() {
            comboBoxInput1.Enabled = radioButtonInput1connected.Checked;
            comboBoxInput2.Enabled = radioButtonInput2connected.Checked;

            groupBoxSwitchingMethod.Enabled = IsPowerSelector();

            if (groupBoxSwitchingMethod.Enabled) {
                if (radioButtonSTDP.Checked) {
                    checkBoxHasOnOffRelay.Checked = true;
                    checkBoxHasOnOffRelay.Enabled = false;
                }
                else
                    checkBoxHasOnOffRelay.Enabled = true;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            if (string.IsNullOrWhiteSpace(textBoxName.Text)) {
                MessageBox.Show(this, "You should provide a name", "Missing name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxName.Focus();
                return;
            }

            if (radioButtonInput1disconnected.Checked && radioButtonInput2disconnected.Checked) {
                MessageBox.Show(this, "Both inputs are disconnected, at least one input should be connected", "No input is connected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ApplyInletModification(comboBoxInput1, radioButtonInput1connected, radioButtonInput1disconnected, new LayoutPowerInlet(XmlInfo.Element, "Inlet1"));
            ApplyInletModification(comboBoxInput2, radioButtonInput2connected, radioButtonInput2disconnected, new LayoutPowerInlet(XmlInfo.Element, "Inlet2"));

            var powerSelectorNameInfo = new LayoutPowerSelectorComponent.LayoutPowerSelectorNameInfo(XmlInfo.DocumentElement);

            if (powerSelectorNameInfo.Element == null)
                powerSelectorNameInfo.CreateElement(XmlInfo);

            powerSelectorNameInfo.Name = textBoxName.Text;
            powerSelectorNameInfo.Visible = checkBoxDisplayPowerSelectorName.Checked;
            XmlInfo.Element.SetAttribute("ReverseLogic", XmlConvert.ToString(checkBoxReverseLogic.Checked));

            XmlInfo.Element.SetAttribute(LayoutPowerSelectorComponent.SwitchingMethodAttribute,
                (radioButtonDTDPrelay.Checked ? LayoutPowerSelectorComponent.RelaySwitchingMethod.DPDSrelay : LayoutPowerSelectorComponent.RelaySwitchingMethod.TwoSPDSrelays).ToString());

            XmlInfo.Element.SetAttribute(LayoutPowerSelectorComponent.HasOnOffRelayAttribute, XmlConvert.ToString(checkBoxHasOnOffRelay.Checked));
            XmlInfo.Element.SetAttribute(LayoutPowerSelectorComponent.ReverseOnOffRelayAttribute, XmlConvert.ToString(checkBoxReverseOnOffRelay.Checked));

            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void radioButton_Click(object sender, System.EventArgs e) {
            UpdateButtons();
        }

        private void buttonSettings_Click(object sender, System.EventArgs e) {
            var powerSelectorNameInfo = new LayoutPowerSelectorComponent.LayoutPowerSelectorNameInfo(XmlInfo.DocumentElement) {
                Component = Component
            };

            CommonUI.Dialogs.TextProviderSettings settings = new CommonUI.Dialogs.TextProviderSettings(XmlInfo, powerSelectorNameInfo);

            if (powerSelectorNameInfo.Element == null)
                powerSelectorNameInfo.CreateElement(XmlInfo);

            settings.ShowDialog(this);
        }
    }
}
