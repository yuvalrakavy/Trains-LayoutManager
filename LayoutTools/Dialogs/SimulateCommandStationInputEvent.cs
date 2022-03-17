#region Using directives

using LayoutManager.Components;
using LayoutManager.Model;
using System;
using System.Windows.Forms;
using MethodDispatcher;

#endregion

namespace LayoutManager.Tools.Dialogs {
    internal partial class SimulateCommandStationInputEvent : Form {
        public SimulateCommandStationInputEvent() {
            InitializeComponent();

            foreach (IModelComponentIsCommandStation commandStation in LayoutModel.Components<IModelComponentIsCommandStation>(LayoutPhase.All))
                if (commandStation.DesignTimeLayoutActivationSupported)
                    comboBoxCommandStation.Items.Add(commandStation);

            comboBoxCommandStation.SelectedIndex = 0;
        }

        private void ComboBoxCommandStation_SelectedIndexChanged(object? sender, EventArgs e) {
            IModelComponentIsCommandStation selected = (IModelComponentIsCommandStation)comboBoxCommandStation.SelectedItem;

            comboBoxBus.Items.Clear();

            if (selected != null) {
                foreach (ControlBus bus in LayoutModel.ControlManager.Buses.Buses(selected))
                    comboBoxBus.Items.Add(bus);

                comboBoxBus.SelectedIndex = 0;
            }
        }

        private void ComboBoxBus_SelectedIndexChanged(object? sender, EventArgs e) {
            ControlBus bus = (ControlBus)comboBoxBus.SelectedItem;

            if (bus != null) {
                if (bus.BusType.AddressingMethod == ControlAddressingMethod.DirectConnectionPointAddressing) {
                    labelIndex.Visible = false;
                    textBoxIndex.Visible = false;
                }
                else {
                    labelIndex.Visible = true;
                    textBoxIndex.Visible = true;
                }
            }
        }

        private void ButtonOK_Click(object? sender, EventArgs e) {
            IModelComponentIsCommandStation commandStation = (IModelComponentIsCommandStation)comboBoxCommandStation.SelectedItem;

            if (commandStation == null) {
                MessageBox.Show(this, "You did not select a command station", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxCommandStation.Focus();
                return;
            }

            ControlBus bus = (ControlBus)comboBoxBus.SelectedItem;

            if (bus == null) {
                MessageBox.Show(this, "You did not select command station bus", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxBus.Focus();
                return;
            }

            int index = -1;
            int state = 0;

            if (!int.TryParse(textBoxAddress.Text, out int address) || address < bus.BusType.FirstAddress || address > bus.BusType.LastAddress) {
                MessageBox.Show(this, "You have entered an invalid address", "Missing or malformed input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxAddress.Focus();
                return;
            }

            if (bus.BusType.AddressingMethod == ControlAddressingMethod.ModuleConnectionPointAddressing) {
                if (!int.TryParse(textBoxIndex.Text, out index)) {
                    MessageBox.Show(this, "You have entered an invalid index", "Missing or malformed input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxIndex.Focus();
                    return;
                }
            }

            if (textBoxState.Text.Trim().Length > 0 && !int.TryParse(textBoxState.Text, out state)) {
                MessageBox.Show(this, "You have entered an invalid state", "Mela-formed input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxAddress.Focus();
                return;
            }

            Dispatch.Notification.OnDesignTimeCommandStationEvent(new((ModelComponent)commandStation, bus, address, index, state));

            DialogResult = DialogResult.OK;
        }
    }
}
