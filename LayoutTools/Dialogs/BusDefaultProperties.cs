using LayoutManager.Model;
using System;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for BusDefaultProperties.
    /// </summary>
    public partial class BusDefaultProperties : Form {


        private readonly ControlBus bus;
        private readonly ControlModuleType? defaultModuleType;

        public BusDefaultProperties(ControlBus bus, string? moduleTypeName, int startingAddress) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.bus = bus;
            Text = "Defaults for " + bus.BusProvider.NameProvider.Name + " - " + bus.BusType.Name;

            if (startingAddress < 0)
                textBoxStartAddress.Text = "";
            else
                textBoxStartAddress.Text = startingAddress.ToString();

            comboBoxModuleType.Items.Add(new ModuleTypeItem(null));

            foreach (ControlModuleType moduleType in bus.BusType.ModuleTypes)
                comboBoxModuleType.Items.Add(new ModuleTypeItem(moduleType));

            if (moduleTypeName == null)
                comboBoxModuleType.SelectedIndex = 0;
            else {
                foreach (ModuleTypeItem item in comboBoxModuleType.Items)
                    if (item.ModuleType != null && item.ModuleType.TypeName == moduleTypeName) {
                        defaultModuleType = item.ModuleType;
                        comboBoxModuleType.SelectedItem = item;
                        break;
                    }
            }

            if (bus.BusType.Topology == ControlBusTopology.DaisyChain) {
                labelStartAddress1.Enabled = false;
                labelStartAddress2.Enabled = false;
                labelStartAddress3.Enabled = false;
                textBoxStartAddress.Enabled = false;
            }
        }

        public string? ModuleTypeName {
            get {
                ModuleTypeItem selected = (ModuleTypeItem)comboBoxModuleType.SelectedItem;

                return selected.ModuleType?.TypeName;
            }
        }

        public int StartingAddress => textBoxStartAddress.Text.Trim() == "" ? -1 : int.Parse(textBoxStartAddress.Text);

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
            if (textBoxStartAddress.Enabled) {
                if (textBoxStartAddress.Text.Trim() != "") {
                    try {
                        int.Parse(textBoxStartAddress.Text);
                    }
                    catch (FormatException) {
                        MessageBox.Show(this, "Illegal address format (must be a number)", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBoxStartAddress.Focus();
                        return;
                    }

                    int firstAddress = (defaultModuleType == null || defaultModuleType.FirstAddress < 0) ? bus.BusType.FirstAddress : defaultModuleType.FirstAddress;
                    int lastAddress = (defaultModuleType == null || defaultModuleType.LastAddress < 0) ? bus.BusType.LastAddress : defaultModuleType.LastAddress;

                    if (StartingAddress < firstAddress || StartingAddress > lastAddress) {
                        MessageBox.Show(this, "Address not in range (" + firstAddress + " to " + lastAddress + ")", "Invalid address", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBoxStartAddress.Focus();
                        return;
                    }

                    if (StartingAddress < bus.BusType.RecommendedStartAddress) {
                        if (MessageBox.Show("Warning: The default start address you have specified is smaller then the recoemmended minimum address for modules on this bus (" + bus.BusType.RecommendedStartAddress + "). Do you want to use this value?",
                            "Start address smaller then Recommended address", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) {
                            textBoxStartAddress.Focus();
                            return;
                        }
                    }
                }
            }

            DialogResult = DialogResult.OK;
        }

        private class ModuleTypeItem {
            public ModuleTypeItem(ControlModuleType? moduleType) {
                this.ModuleType = moduleType;
            }

            public ControlModuleType? ModuleType { get; }

            public override string ToString() {
                return ModuleType == null ? "(Prompt)" : ModuleType.Name;
            }
        }
    }
}
