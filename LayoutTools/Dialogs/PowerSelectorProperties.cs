using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;
using LayoutManager.Components;
using LayoutManager.CommonUI;

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

			if(powerSelectorNameInfo.Element != null)
				checkBoxDisplayPowerSelectorName.Checked = powerSelectorNameInfo.Visible;
			else
				checkBoxDisplayPowerSelectorName.Checked = true;

			if(XmlInfo.Element.HasAttribute("ReverseLogic"))
				checkBoxReverseLogic.Checked = XmlConvert.ToBoolean(XmlInfo.Element.GetAttribute("ReverseLogic"));
			else
				checkBoxReverseLogic.Checked = false;

			UpdateButtons();
		}

		private void InitInletCombobox(ComboBox comboBox, RadioButton radioButtonConnected, RadioButton radioButtonNotConnected, ILayoutPowerInlet inlet) {
			foreach(IModelComponentHasPowerOutlets componentWithPowerSources in LayoutModel.Components<IModelComponentHasPowerOutlets>(LayoutPhase.All)) {
				if(componentWithPowerSources.Id != Component.Id) {
					for(int outletIndex = 0; outletIndex < componentWithPowerSources.PowerOutlets.Count; outletIndex++)
						comboBox.Items.Add(new LayoutComponentPowerOutletDescription(componentWithPowerSources, outletIndex));
				}
			}

			if(inlet.IsConnected) {
				radioButtonConnected.Checked = true;

				foreach(LayoutComponentPowerOutletDescription powerSourceItem in comboBox.Items) {
					if(powerSourceItem.Component.Id == inlet.OutletComponentId && powerSourceItem.OutletIndex == inlet.OutletIndex) {
						comboBox.SelectedItem = powerSourceItem;
						break;
					}
				}
			}
			else
				radioButtonNotConnected.Checked = true;
		}

		private void ApplyInletModification(ComboBox comboBox, RadioButton radioButtonConnected, RadioButton radioButtonNotConnected, ILayoutPowerInlet inlet) {
			if(radioButtonNotConnected.Checked || comboBox.SelectedItem == null)
				inlet.OutletComponentId = Guid.Empty;
			else {
				LayoutComponentPowerOutletDescription selectedOutlet = (LayoutComponentPowerOutletDescription)comboBox.SelectedItem;

				inlet.OutletComponentId = selectedOutlet.Component.Id;
				inlet.OutletIndex = selectedOutlet.OutletIndex;
			}
		}

		LayoutPowerSelectorComponent Component { get; set; }

		#region ILayoutComponentPropertiesDialog Members

		public LayoutXmlInfo XmlInfo {
			get;
			private set;
		}

		#endregion

		private void UpdateButtons() {
			comboBoxInput1.Enabled = radioButtonInput1connected.Checked;
			comboBoxInput2.Enabled = radioButtonInput2connected.Checked;
		}

		private void buttonOK_Click(object sender, EventArgs e) {
			if(string.IsNullOrWhiteSpace(textBoxName.Text)) {
				MessageBox.Show(this, "You should provide a name", "Missing name", MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBoxName.Focus();
				return;
			}

			if(radioButtonInput1disconnected.Checked && radioButtonInput2disconnected.Checked) {
				MessageBox.Show(this, "Both inputs are disconnected, at least one input should be connected", "No input is connected", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			ApplyInletModification(comboBoxInput1, radioButtonInput1connected, radioButtonInput1disconnected, new LayoutPowerInlet(XmlInfo.Element, "Inlet1"));
			ApplyInletModification(comboBoxInput2, radioButtonInput2connected, radioButtonInput2disconnected, new LayoutPowerInlet(XmlInfo.Element, "Inlet2"));

			var powerSelectorNameInfo = new LayoutPowerSelectorComponent.LayoutPowerSelectorNameInfo(XmlInfo.DocumentElement);

			if(powerSelectorNameInfo.Element == null)
				powerSelectorNameInfo.CreateElement(XmlInfo);

			powerSelectorNameInfo.Name = textBoxName.Text;
			powerSelectorNameInfo.Visible = checkBoxDisplayPowerSelectorName.Checked;
			XmlInfo.Element.SetAttribute("ReverseLogic", XmlConvert.ToString(checkBoxReverseLogic.Checked));

			DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void radioButton_Click(object sender, System.EventArgs e) {
			UpdateButtons();
		}

		private void buttonSettings_Click(object sender, System.EventArgs e) {
			var powerSelectorNameInfo = new LayoutPowerSelectorComponent.LayoutPowerSelectorNameInfo(XmlInfo.DocumentElement);

			powerSelectorNameInfo.Component = Component;

			CommonUI.Dialogs.TextProviderSettings settings = new CommonUI.Dialogs.TextProviderSettings(XmlInfo, powerSelectorNameInfo);

			if(powerSelectorNameInfo.Element == null)
				powerSelectorNameInfo.CreateElement(XmlInfo);

			settings.ShowDialog(this);
		}
	}
}
