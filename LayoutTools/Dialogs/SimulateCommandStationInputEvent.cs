#region Using directives

using System;
using System.Windows.Forms;
using LayoutManager.Model;
using LayoutManager.Components;

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

        private void comboBoxCommandStation_SelectedIndexChanged(object sender, EventArgs e) {
            IModelComponentIsCommandStation selected = (IModelComponentIsCommandStation)comboBoxCommandStation.SelectedItem;

            comboBoxBus.Items.Clear();

            if (selected != null) {
                foreach (ControlBus bus in LayoutModel.ControlManager.Buses.Buses(selected))
                    comboBoxBus.Items.Add(bus);

                comboBoxBus.SelectedIndex = 0;
            }
        }

        private void comboBoxBus_SelectedIndexChanged(object sender, EventArgs e) {
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

        private void buttonOK_Click(object sender, EventArgs e) {
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
                MessageBox.Show(this, "You have entered an invalid addres", "Missing or melaformed input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxAddress.Focus();
                return;
            }

            if (bus.BusType.AddressingMethod == ControlAddressingMethod.ModuleConnectionPointAddressing) {
                if (!int.TryParse(textBoxIndex.Text, out index)) {
                    MessageBox.Show(this, "You have entered an invalid index", "Missing or melaformed input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxIndex.Focus();
                    return;
                }
            }

            if (textBoxState.Text.Trim().Length > 0 && !int.TryParse(textBoxState.Text, out state)) {
                MessageBox.Show(this, "You have entered an invalid state", "Melaformed input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxAddress.Focus();
                return;
            }

            CommandStationInputEvent commandStationEvent = new CommandStationInputEvent((ModelComponent)commandStation, bus, address, index, state);

            EventManager.Event(new LayoutEvent("design-time-command-station-event", this, commandStationEvent));

            DialogResult = DialogResult.OK;
        }
    }
}
