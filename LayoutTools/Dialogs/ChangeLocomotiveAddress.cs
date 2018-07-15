using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

using LayoutManager;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
	public partial class ChangeLocomotiveAddress : Form {
		LocomotiveInfo Locomotive { get; }
        public IModelComponentIsCommandStation CommandStation { get; }
        public int Address { get; private set; }
		public bool ProgramLocomotive { get; private set; }
        public int SpeedSteps { get; private set; }

		public ChangeLocomotiveAddress(LocomotiveInfo locomotive, IModelComponentIsCommandStation commandStation = null) {
			InitializeComponent();
			this.Locomotive = locomotive;

			if(Locomotive.AddressProvider.Unit <= 0)
				radioButtonUnknownAddress.Checked = true;
			else {
				radioButtonSetAddress.Checked = true;
				textBoxAddress.Text = Locomotive.AddressProvider.Unit.ToString();
			}

			if(commandStation == null)
				CommandStation = LayoutModel.Components<IModelComponentIsCommandStation>(LayoutModel.ActivePhases).SingleOrDefault();
			else
				this.CommandStation = commandStation;

            radioButton14steps.Checked = locomotive.SpeedSteps == 14;
            radioButton28steps.Checked = locomotive.SpeedSteps == 28;

			Text = string.Format(Text, locomotive.Name);
			SetButtons();
		}

		private void SetButtons() {
			textBoxAddress.Enabled = radioButtonSetAddress.Checked;
			buttonAllocateAddress.Enabled = radioButtonSetAddress.Checked;
			buttonSaveAndProgram.Enabled = radioButtonSetAddress.Checked && LayoutModel.Components<IModelComponentCanProgramLocomotives>(LayoutModel.ActivePhases).SingleOrDefault() != null;

			AcceptButton = buttonSaveAndProgram.Enabled ? buttonSaveAndProgram : buttonSaveOnly;
		}

		private void radioButtonSetAddress_CheckedChanged(object sender, EventArgs e) {
			SetButtons();
		}

		private void radioButtonUnknownAddress_CheckedChanged(object sender, EventArgs e) {
			SetButtons();
		}

		private void buttonAllocateAddress_Click(object sender, EventArgs e) {
			int address = (int)EventManager.Event(new LayoutEvent<object, LocomotiveInfo>("allocate-locomotive-address", null, Locomotive));

			if(address != -1)
				textBoxAddress.Text = address.ToString();
			else
				MessageBox.Show(this, "Cannot allocate address", "It is not possible to allocate unused locomotive address", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private bool ValidateAddress(out int address) {
			if(!int.TryParse(textBoxAddress.Text, out address)) {
				MessageBox.Show(this, "Invalid address", "You have entered invalid locomotive address", MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBoxAddress.Focus();
				return false;
			}

			if(address < Locomotive.GetLowestAddress(CommandStation)) {
				MessageBox.Show(this, "Invalid address", "Address is below minimum allowed address", MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBoxAddress.Focus();
				return false;
			}

			if(address > Locomotive.GetHighestAddress(CommandStation)) {
				MessageBox.Show(this, "Invalid address", "Address is above maximum allowed address", MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBoxAddress.Focus();
				return false;
			}

			if(CommandStation != null) {
				var power = (from powerOutlet in CommandStation.PowerOutlets where powerOutlet.Power.Type == LayoutPowerType.Digital select powerOutlet.Power).FirstOrDefault();

				if(power != null) {
					var result = (CanPlaceTrainResult)EventManager.Event(new LayoutEvent<XmlElement, ILayoutPower>("is-locomotive-address-valid", Locomotive.Element, power).SetOption("LocoAddress", address));

					if(result.ResolveMethod != CanPlaceTrainResolveMethod.Resolved)
						return MessageBox.Show(this, "Address warning", result.ToString() + "\n\nDo you want to use this address?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes;
				}
			}

			this.Address = address;
            this.SpeedSteps = radioButton28steps.Checked ? 28 : 14;
 
			return true;
		}

		private void buttonSaveOnly_Click(object sender, EventArgs e) {
			int address = 0;
			
			if(radioButtonUnknownAddress.Checked || ValidateAddress(out address)) {
				ProgramLocomotive = false;
				DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
		}

		private void buttonSaveAndProgram_Click(object sender, EventArgs e) {
			int address = 0;

			if(radioButtonUnknownAddress.Checked || ValidateAddress(out address)) {
				ProgramLocomotive = true;
				DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
		}
	}
}
