using System;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for SetControlModuleAddress.
    /// </summary>
    public partial class SetControlModuleAddress : Form {


        private readonly ControlBus bus;
        private readonly ControlModuleType moduleType;
        private readonly ControlModule? module;

        private const int possibleAddressLimit = 70;     // If possible address is above this limit, use text box and not combo box
        private readonly bool useTextBox = false;
        private int address = -1;

        public SetControlModuleAddress(ControlBus bus, ControlModuleType moduleType, Guid controlModuleLocationId, ControlModule? module) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.bus = bus;
            this.moduleType = moduleType;
            this.module = module;

            Text = "Set address of " + moduleType.Name;

            // Start building a list of possible addresses
            int firstAddress = GetFirstAddress(bus, moduleType);
            int lastAddress = GetLastAddress(bus, moduleType);
            int address = bus.BusType.GetAlignedAddress(moduleType, firstAddress);

            while (address <= lastAddress) {
                ControlModule? moduleUsingAddress = null;

                for (int a = address; a < address + moduleType.NumberOfAddresses; a++) {
                    moduleUsingAddress = bus.GetModuleUsingAddress(a);

                    if (moduleUsingAddress != null)
                        break;
                }

                if (moduleUsingAddress == null || (module != null && module.Address == address)) {
                    comboBoxAddress.Items.Add(new AddressEntry(address, moduleType.NumberOfAddresses));

                    if (comboBoxAddress.Items.Count > possibleAddressLimit) {
                        useTextBox = true;
                        break;
                    }

                    address += moduleType.NumberOfAddresses;
                }
                else
                    address = bus.BusType.GetAlignedAddress(moduleType, address + moduleUsingAddress.ModuleType.NumberOfAddresses);
            }

            if (useTextBox) {
                this.SuspendLayout();
                textBoxAddress.Location = comboBoxAddress.Location;
                comboBoxAddress.Visible = false;
                this.ResumeLayout();
            }
            else
                textBoxAddress.Visible = false;

            // Figure out default address to suggest

            int defaultAddress;

            if (module != null)
                defaultAddress = module.Address;
            else
                defaultAddress = bus.AllocateFreeAddress(moduleType, controlModuleLocationId);

            if (defaultAddress >= 0) {
                if (useTextBox)
                    textBoxAddress.Text = defaultAddress.ToString();
                else {
                    foreach (AddressEntry e in comboBoxAddress.Items)
                        if (e.Address == defaultAddress) {
                            comboBoxAddress.SelectedItem = e;
                            break;
                        }
                }
            }
        }

        private static int GetLastAddress(ControlBus bus, ControlModuleType moduleType) => moduleType.LastAddress < 0 ? bus.BusType.LastAddress : moduleType.LastAddress;

        private static int GetFirstAddress(ControlBus bus, ControlModuleType moduleType) => moduleType.FirstAddress < 0 ? bus.BusType.FirstAddress : moduleType.FirstAddress;

        public int Address => address;

        public bool UserActionRequired => checkBoxSetUserActionRequired.Checked;

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
            if (useTextBox) {
                try {
                    address = int.Parse(textBoxAddress.Text);
                }
                catch (FormatException) {
                    MessageBox.Show(this, "Invalid address", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxAddress.Focus();
                    return;
                }

                int firstAddress = GetFirstAddress(bus, moduleType);
                int lastAddress = GetLastAddress(bus, moduleType);

                // Check if address is properly aligned
                if ((address - firstAddress) % moduleType.AddressAlignment != 0) {
                    MessageBox.Show(this, "Address must be a multiple of " + moduleType.AddressAlignment, "Address Alignment error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxAddress.Focus();
                    return;
                }

                if (address < firstAddress || address > lastAddress) {
                    MessageBox.Show(this, "Address is not in the allowed range (" + firstAddress + " to " + lastAddress + ")", "Address Range Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxAddress.Focus();
                    return;
                }

                for (int a = address; a < address + moduleType.NumberOfAddresses; a++) {
                    var moduleUsingAddress = bus.GetModuleUsingAddress(a);

                    if (moduleUsingAddress != null && (module == null || module.Element != moduleUsingAddress.Element)) {
                        MessageBox.Show(this, "Address is already used by another module", "Address already used", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBoxAddress.Focus();
                        return;
                    }
                }
            }
            else
                address = ((AddressEntry)comboBoxAddress.SelectedItem).Address;

            DialogResult = DialogResult.OK;
            Close();
        }

        private class AddressEntry {
            private readonly int numberOfAddress;

            public AddressEntry(int address, int numberOfAddress) {
                this.Address = address;
                this.numberOfAddress = numberOfAddress;
            }

            public int Address { get; }

            public override string ToString() {
                if (numberOfAddress == 1)
                    return Address.ToString();
                else {
                    int lastAddress = Address + numberOfAddress - 1;

                    return Address.ToString() + " - " + lastAddress.ToString();
                }
            }
        }
    }
}
