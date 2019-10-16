using System;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

#pragma warning disable IDE0051, IDE0069
namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for SetControlModuleAddress.
    /// </summary>
    public class SetControlModuleAddress : Form {
        private Label label1;
        private TextBox textBoxAddress;
        private Button buttonOK;
        private Button buttonCancel;
        private ComboBox comboBoxAddress;
        private CheckBox checkBoxSetUserActionRequired;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;


        private readonly ControlBus bus;
        private readonly ControlModuleType moduleType;
        private readonly ControlModule module;

        private const int possibleAddressLimit = 70;     // If possible address is above this limit, use text box and not combo box
        private readonly bool useTextBox = false;
        private int address = -1;

        public SetControlModuleAddress(ControlBus bus, ControlModuleType moduleType, Guid controlModuleLocationId, ControlModule module) {
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
                ControlModule moduleUsingAddress = null;

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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new Label();
            this.comboBoxAddress = new ComboBox();
            this.textBoxAddress = new TextBox();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.checkBoxSetUserActionRequired = new CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Set module to: ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxAddress
            // 
            this.comboBoxAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAddress.Location = new System.Drawing.Point(96, 17);
            this.comboBoxAddress.Name = "comboBoxAddress";
            this.comboBoxAddress.Size = new System.Drawing.Size(112, 21);
            this.comboBoxAddress.TabIndex = 1;
            // 
            // textBoxAddress
            // 
            this.textBoxAddress.Location = new System.Drawing.Point(128, 48);
            this.textBoxAddress.Name = "textBoxAddress";
            this.textBoxAddress.TabIndex = 2;
            this.textBoxAddress.Text = "";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(232, 16);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += this.buttonOK_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(232, 48);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            // 
            // checkBoxSetUserActionRequired
            // 
            this.checkBoxSetUserActionRequired.Checked = true;
            this.checkBoxSetUserActionRequired.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSetUserActionRequired.Location = new System.Drawing.Point(8, 48);
            this.checkBoxSetUserActionRequired.Name = "checkBoxSetUserActionRequired";
            this.checkBoxSetUserActionRequired.Size = new System.Drawing.Size(192, 24);
            this.checkBoxSetUserActionRequired.TabIndex = 5;
            this.checkBoxSetUserActionRequired.Text = "Set \"User Action Required\" flag";
            // 
            // SetControlModuleAddress
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(312, 78);
            this.ControlBox = false;
            this.Controls.Add(this.checkBoxSetUserActionRequired);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBoxAddress);
            this.Controls.Add(this.comboBoxAddress);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SetControlModuleAddress";
            this.ShowInTaskbar = false;
            this.Text = "SetControlModuleAddress";
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonOK_Click(object sender, System.EventArgs e) {
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
                    ControlModule moduleUsingAddress = bus.GetModuleUsingAddress(a);

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
