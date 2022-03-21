using LayoutManager.Components;
using LayoutManager.Model;
using System;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    public partial class PowerSelectorProperties : Form, ILayoutComponentPropertiesDialog {
        private const string A_ReverseLogic = "ReverseLogic";

        public PowerSelectorProperties(ModelComponent component) {
            InitializeComponent();

            this.Component = (LayoutPowerSelectorComponent)component;
            this.XmlInfo = new LayoutXmlInfo(Component);

            InitInletCombobox(comboBoxInput1, radioButtonInput1connected, radioButtonInput1disconnected, Component.Inlet1);
            InitInletCombobox(comboBoxInput2, radioButtonInput2connected, radioButtonInput2disconnected, Component.Inlet2);

            var powerSelectorNameInfo = new LayoutPowerSelectorComponent.LayoutPowerSelectorNameInfo(XmlInfo.DocumentElement);

            textBoxName.Text = powerSelectorNameInfo.Name;

            if (powerSelectorNameInfo.OptionalElement != null)
                checkBoxDisplayPowerSelectorName.Checked = powerSelectorNameInfo.Visible;
            else
                checkBoxDisplayPowerSelectorName.Checked = true;

            if (XmlInfo.Element.HasAttribute(A_ReverseLogic))
                checkBoxReverseLogic.Checked = (bool)XmlInfo.Element.AttributeValue(A_ReverseLogic);
            else
                checkBoxReverseLogic.Checked = false;

            if (!XmlInfo.Element.HasAttribute(LayoutPowerSelectorComponent.A_SwitchingMethod) ||
                (LayoutPowerSelectorComponent.RelaySwitchingMethod)Enum.Parse(typeof(LayoutPowerSelectorComponent.RelaySwitchingMethod), XmlInfo.Element.GetAttribute(LayoutPowerSelectorComponent.A_SwitchingMethod)) == LayoutPowerSelectorComponent.RelaySwitchingMethod.DPDSrelay)
                radioButtonDTDPrelay.Checked = true;
            else {
                radioButtonSTDP.Checked = true;

                checkBoxHasOnOffRelay.Checked = (bool?)XmlInfo.Element.AttributeValue(LayoutPowerSelectorComponent.A_HasOnOffRelay) ?? false;
                checkBoxReverseOnOffRelay.Checked = (bool?)XmlInfo.Element.AttributeValue(LayoutPowerSelectorComponent.A_ReverseOnOffRelay) ?? false;
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

        private void ApplyInletModification(ComboBox comboBox, RadioButton radioButtonNotConnected, ILayoutPowerInlet inlet) {
            if (radioButtonNotConnected.Checked || comboBox.SelectedItem == null)
                inlet.OutletComponentId = Guid.Empty;
            else {
                LayoutComponentPowerOutletDescription selectedOutlet = (LayoutComponentPowerOutletDescription)comboBox.SelectedItem;

                inlet.OutletComponentId = selectedOutlet.Component.Id;
                inlet.OutletIndex = selectedOutlet.OutletIndex;
            }
        }

        private LayoutPowerSelectorComponent Component { get; }

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

        private void ButtonOK_Click(object? sender, EventArgs e) {
            if (string.IsNullOrWhiteSpace(textBoxName.Text)) {
                MessageBox.Show(this, "You should provide a name", "Missing name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxName.Focus();
                return;
            }

            if (radioButtonInput1disconnected.Checked && radioButtonInput2disconnected.Checked) {
                MessageBox.Show(this, "Both inputs are disconnected, at least one input should be connected", "No input is connected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ApplyInletModification(comboBoxInput1, radioButtonInput1disconnected, new LayoutPowerInlet(XmlInfo.Element, "Inlet1"));
            ApplyInletModification(comboBoxInput2, radioButtonInput2disconnected, new LayoutPowerInlet(XmlInfo.Element, "Inlet2"));

            var powerSelectorNameInfo = new LayoutPowerSelectorComponent.LayoutPowerSelectorNameInfo(XmlInfo.DocumentElement);

            if (powerSelectorNameInfo.OptionalElement == null)
                powerSelectorNameInfo.CreateElement(XmlInfo);

            powerSelectorNameInfo.Name = textBoxName.Text;
            powerSelectorNameInfo.Visible = checkBoxDisplayPowerSelectorName.Checked;
            XmlInfo.Element.SetAttributeValue(A_ReverseLogic, checkBoxReverseLogic.Checked);

            XmlInfo.Element.SetAttributeValue(LayoutPowerSelectorComponent.A_SwitchingMethod,
                radioButtonDTDPrelay.Checked ? LayoutPowerSelectorComponent.RelaySwitchingMethod.DPDSrelay : LayoutPowerSelectorComponent.RelaySwitchingMethod.TwoSPDSrelays);

            XmlInfo.Element.SetAttributeValue(LayoutPowerSelectorComponent.A_HasOnOffRelay, checkBoxHasOnOffRelay.Checked);
            XmlInfo.Element.SetAttributeValue(LayoutPowerSelectorComponent.A_ReverseOnOffRelay, checkBoxReverseOnOffRelay.Checked);

            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void RadioButton_Click(object? sender, System.EventArgs e) {
            UpdateButtons();
        }

        private void ButtonSettings_Click(object? sender, System.EventArgs e) {
            var powerSelectorNameInfo = new LayoutPowerSelectorComponent.LayoutPowerSelectorNameInfo(XmlInfo.DocumentElement) {
                Component = Component
            };

            CommonUI.Dialogs.TextProviderSettings settings = new(XmlInfo, powerSelectorNameInfo);

            if (powerSelectorNameInfo.OptionalElement == null)
                powerSelectorNameInfo.CreateElement(XmlInfo);

            settings.ShowDialog(this);
        }
    }
}
