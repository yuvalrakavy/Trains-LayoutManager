using System;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for BusDefaultProperties.
    /// </summary>
    public class BusDefaultProperties : Form {
        private Label label2;
        private ComboBox comboBoxModuleType;
        private Label label3;
        private Label label4;
        private TextBox textBoxStartAddress;
        private Button buttonOK;
        private Button buttonCancel;
        private Label labelStartAddress1;
        private Label labelStartAddress2;
        private Label labelStartAddress3;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private void endOfDesignerVariables() { }

        readonly ControlBus bus;
        readonly ControlModuleType defaultModuleType;


        public BusDefaultProperties(ControlBus bus, string moduleTypeName, int startingAddress) {
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

        public string ModuleTypeName {
            get {
                ModuleTypeItem selected = (ModuleTypeItem)comboBoxModuleType.SelectedItem;

                if (selected.ModuleType == null)
                    return null;
                else
                    return selected.ModuleType.TypeName;
            }
        }

        public int StartingAddress {
            get {
                if (textBoxStartAddress.Text.Trim() == "")
                    return -1;

                return int.Parse(textBoxStartAddress.Text);
            }
        }

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
            this.label2 = new Label();
            this.comboBoxModuleType = new ComboBox();
            this.label3 = new Label();
            this.label4 = new Label();
            this.labelStartAddress1 = new Label();
            this.textBoxStartAddress = new TextBox();
            this.labelStartAddress2 = new Label();
            this.labelStartAddress3 = new Label();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 23);
            this.label2.TabIndex = 4;
            this.label2.Text = "Add module of type:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxModuleType
            // 
            this.comboBoxModuleType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxModuleType.Location = new System.Drawing.Point(118, 50);
            this.comboBoxModuleType.Name = "comboBoxModuleType";
            this.comboBoxModuleType.Size = new System.Drawing.Size(180, 21);
            this.comboBoxModuleType.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(300, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 23);
            this.label3.TabIndex = 6;
            this.label3.Text = "When new";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(312, 23);
            this.label4.TabIndex = 7;
            this.label4.Text = "modules need to added for performing automatic connection";
            // 
            // labelStartAddress1
            // 
            this.labelStartAddress1.Location = new System.Drawing.Point(6, 3);
            this.labelStartAddress1.Name = "labelStartAddress1";
            this.labelStartAddress1.Size = new System.Drawing.Size(152, 23);
            this.labelStartAddress1.TabIndex = 0;
            this.labelStartAddress1.Text = "Assign address starting from:";
            this.labelStartAddress1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxStartAddress
            // 
            this.textBoxStartAddress.Location = new System.Drawing.Point(166, 4);
            this.textBoxStartAddress.Name = "textBoxStartAddress";
            this.textBoxStartAddress.Size = new System.Drawing.Size(56, 20);
            this.textBoxStartAddress.TabIndex = 1;
            // 
            // labelStartAddress2
            // 
            this.labelStartAddress2.Location = new System.Drawing.Point(238, 3);
            this.labelStartAddress2.Name = "labelStartAddress2";
            this.labelStartAddress2.Size = new System.Drawing.Size(141, 23);
            this.labelStartAddress2.TabIndex = 2;
            this.labelStartAddress2.Text = "to modules  added in this ";
            this.labelStartAddress2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelStartAddress3
            // 
            this.labelStartAddress3.Location = new System.Drawing.Point(4, 25);
            this.labelStartAddress3.Name = "labelStartAddress3";
            this.labelStartAddress3.Size = new System.Drawing.Size(272, 16);
            this.labelStartAddress3.TabIndex = 3;
            this.labelStartAddress3.Text = "module location";
            this.labelStartAddress3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(210, 106);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 8;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(290, 106);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            // 
            // BusDefaultProperties
            // 
            this.AcceptButton = this.buttonOK;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(373, 135);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.labelStartAddress3);
            this.Controls.Add(this.labelStartAddress2);
            this.Controls.Add(this.textBoxStartAddress);
            this.Controls.Add(this.labelStartAddress1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxModuleType);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "BusDefaultProperties";
            this.ShowInTaskbar = false;
            this.Text = "Adding Modules Defaults";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void buttonOK_Click(object sender, System.EventArgs e) {
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

        class ModuleTypeItem {
            readonly ControlModuleType moduleType;

            public ModuleTypeItem(ControlModuleType moduleType) {
                this.moduleType = moduleType;
            }

            public ControlModuleType ModuleType => moduleType;

            public override string ToString() {
                if (moduleType == null)
                    return "(Prompt)";
                else
                    return moduleType.Name;
            }
        }


    }
}
