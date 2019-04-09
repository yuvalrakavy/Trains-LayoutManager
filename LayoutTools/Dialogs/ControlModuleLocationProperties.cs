using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for ControlModuleLocationProperties.
    /// </summary>
    public class ControlModuleLocationProperties : Form, ILayoutComponentPropertiesDialog {
        private LayoutManager.CommonUI.Controls.NameDefinition nameDefinition;
        private GroupBox groupBox1;
        private ListView listViewDefaults;
        private Button buttonCancel;
        private Button buttonOK;
        private ColumnHeader columnHeaderBusName;
        private ColumnHeader columnHeaderModuleType;
        private ColumnHeader columnHeaderStartAddress;
        private Button buttonAdd;
        private Button buttonEdit;
        private Button buttonRemove;
        private Label label1;
        private ComboBox comboBoxCommandStation;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private void EndOfDesignerVariables() { }

        public ControlModuleLocationProperties(ModelComponent component) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.Component = (LayoutControlModuleLocationComponent)component;
            this.XmlInfo = new LayoutXmlInfo(component);

            nameDefinition.XmlInfo = XmlInfo;
            nameDefinition.Component = component;

            LayoutControlModuleLocationComponentInfo info = new LayoutControlModuleLocationComponentInfo(Component, XmlInfo.Element);

            comboBoxCommandStation.Items.Add(new CommandStationItem("(Prompt)"));

            foreach (IModelComponentIsCommandStation commandStation in LayoutModel.Components<IModelComponentIsCommandStation>(LayoutPhase.All))
                comboBoxCommandStation.Items.Add(new CommandStationItem(commandStation));

            foreach (CommandStationItem item in comboBoxCommandStation.Items)
                if (item.ID == info.CommandStationId) {
                    comboBoxCommandStation.SelectedItem = item;
                    break;
                }

            foreach (ControlModuleLocationBusDefaultInfo busDefault in info.Defaults)
                listViewDefaults.Items.Add(new Item(busDefault));

            updateButtons();
        }

        public LayoutXmlInfo XmlInfo { get; }

        public LayoutControlModuleLocationComponent Component { get; }

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

        private void updateButtons() {
            if (listViewDefaults.SelectedItems.Count == 0) {
                buttonEdit.Enabled = false;
                buttonRemove.Enabled = false;
            }
            else {
                buttonEdit.Enabled = true;
                buttonRemove.Enabled = true;
            }
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.nameDefinition = new LayoutManager.CommonUI.Controls.NameDefinition();
            this.groupBox1 = new GroupBox();
            this.buttonEdit = new Button();
            this.buttonAdd = new Button();
            this.listViewDefaults = new ListView();
            this.columnHeaderBusName = new ColumnHeader();
            this.columnHeaderModuleType = new ColumnHeader();
            this.columnHeaderStartAddress = new ColumnHeader();
            this.buttonRemove = new Button();
            this.buttonCancel = new Button();
            this.buttonOK = new Button();
            this.label1 = new Label();
            this.comboBoxCommandStation = new ComboBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // nameDefinition
            // 
            this.nameDefinition.DefaultIsVisible = true;
            this.nameDefinition.ElementName = "Name";
            this.nameDefinition.IsOptional = false;
            this.nameDefinition.Location = new System.Drawing.Point(3, 8);
            this.nameDefinition.Name = "nameDefinition";
            this.nameDefinition.Size = new System.Drawing.Size(373, 64);
            this.nameDefinition.TabIndex = 0;
            this.nameDefinition.XmlInfo = null;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonEdit);
            this.groupBox1.Controls.Add(this.buttonAdd);
            this.groupBox1.Controls.Add(this.listViewDefaults);
            this.groupBox1.Controls.Add(this.buttonRemove);
            this.groupBox1.Location = new System.Drawing.Point(8, 96);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(368, 192);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Defaults:";
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonEdit.Location = new System.Drawing.Point(88, 166);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(75, 20);
            this.buttonEdit.TabIndex = 2;
            this.buttonEdit.Text = "&Edit";
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAdd.Location = new System.Drawing.Point(8, 166);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 20);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "&Add";
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // listViewDefaults
            // 
            this.listViewDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewDefaults.Columns.AddRange(new ColumnHeader[] {
                                                                                               this.columnHeaderBusName,
                                                                                               this.columnHeaderModuleType,
                                                                                               this.columnHeaderStartAddress});
            this.listViewDefaults.FullRowSelect = true;
            this.listViewDefaults.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewDefaults.Location = new System.Drawing.Point(8, 16);
            this.listViewDefaults.MultiSelect = false;
            this.listViewDefaults.Name = "listViewDefaults";
            this.listViewDefaults.Size = new System.Drawing.Size(352, 144);
            this.listViewDefaults.TabIndex = 0;
            this.listViewDefaults.View = System.Windows.Forms.View.Details;
            this.listViewDefaults.SelectedIndexChanged += new System.EventHandler(this.listViewDefaults_SelectedIndexChanged);
            // 
            // columnHeaderBusName
            // 
            this.columnHeaderBusName.Text = "Connection";
            this.columnHeaderBusName.Width = 141;
            // 
            // columnHeaderModuleType
            // 
            this.columnHeaderModuleType.Text = "Module";
            this.columnHeaderModuleType.Width = 126;
            // 
            // columnHeaderStartAddress
            // 
            this.columnHeaderStartAddress.Text = "Start Address";
            this.columnHeaderStartAddress.Width = 81;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRemove.Location = new System.Drawing.Point(168, 166);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(75, 20);
            this.buttonRemove.TabIndex = 3;
            this.buttonRemove.Text = "&Remove";
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(301, 296);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(221, 296);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "By default new modules will be connected to: ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxCommandStation
            // 
            this.comboBoxCommandStation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCommandStation.Location = new System.Drawing.Point(248, 64);
            this.comboBoxCommandStation.Name = "comboBoxCommandStation";
            this.comboBoxCommandStation.Size = new System.Drawing.Size(128, 21);
            this.comboBoxCommandStation.TabIndex = 2;
            // 
            // ControlModuleLocationProperties
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(386, 325);
            this.ControlBox = false;
            this.Controls.Add(this.comboBoxCommandStation);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.nameDefinition);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ControlModuleLocationProperties";
            this.Text = "Control Modules Location Properties";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonOK_Click(object sender, System.EventArgs e) {
            if (nameDefinition.IsEmptyName) {
                MessageBox.Show(this, "You have to provide a name for control module location",
                    "Missing Control module location Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                nameDefinition.Focus();
                return;
            }

            if (!nameDefinition.Commit())
                return;

            LayoutControlModuleLocationComponentInfo info = new LayoutControlModuleLocationComponentInfo(Component, XmlInfo.Element) {
                CommandStationId = ((CommandStationItem)comboBoxCommandStation.SelectedItem).ID
            };

            DialogResult = DialogResult.OK;
        }

        private void buttonAdd_Click(object sender, System.EventArgs e) {
            ContextMenu menu = new ContextMenu();

            foreach (XmlElement busElement in LayoutModel.ControlManager.BusesElement) {
                ControlBus bus = new ControlBus(LayoutModel.ControlManager, busElement);
                bool found = false;

                foreach (Item item in listViewDefaults.Items)
                    if (item.BusDefault.BusId == bus.Id) {
                        found = true;
                        break;
                    }

                if (!found)
                    menu.MenuItems.Add(new AddDefaultMenuItem(this, bus));
            }

            if (menu.MenuItems.Count == 0) {
                MenuItem item = new MenuItem("No more control bus can be added") {
                    Enabled = false
                };
                menu.MenuItems.Add(item);
            }

            menu.Show(buttonAdd.Parent, new Point(buttonAdd.Left, buttonAdd.Bottom));
        }

        private void buttonEdit_Click(object sender, System.EventArgs e) {
            if (listViewDefaults.SelectedItems.Count > 0) {
                Item selected = (Item)listViewDefaults.SelectedItems[0];
                Dialogs.BusDefaultProperties d = new Dialogs.BusDefaultProperties(selected.BusDefault.Bus, selected.BusDefault.DefaultModuleTypeName, selected.BusDefault.DefaultStartAddress);

                if (d.ShowDialog() == DialogResult.OK) {
                    selected.BusDefault.DefaultModuleTypeName = d.ModuleTypeName;
                    selected.BusDefault.DefaultStartAddress = d.StartingAddress;

                    selected.Update();
                    updateButtons();
                }
            }
        }

        private void buttonRemove_Click(object sender, System.EventArgs e) {
            if (listViewDefaults.SelectedItems.Count > 0) {
                Item selected = (Item)listViewDefaults.SelectedItems[0];
                LayoutControlModuleLocationComponentInfo info = new LayoutControlModuleLocationComponentInfo(Component, XmlInfo.Element);

                info.Defaults.Remove(selected.BusDefault.BusId);
                listViewDefaults.Items.Remove(selected);
                updateButtons();
            }
        }

        private void listViewDefaults_SelectedIndexChanged(object sender, System.EventArgs e) {
            updateButtons();
        }

        private class Item : ListViewItem {
            public Item(ControlModuleLocationBusDefaultInfo busDefault) {
                this.BusDefault = busDefault;

                Text = busDefault.Bus.BusProvider.NameProvider.Name + " - " + busDefault.Bus.BusType.Name;
                SubItems.Add("");
                SubItems.Add("");

                Update();
            }

            public ControlModuleLocationBusDefaultInfo BusDefault { get; }

            public void Update() {
                if (BusDefault.DefaultModuleTypeName != null)
                    SubItems[1].Text = BusDefault.DefaultModuleType.Name;
                else
                    SubItems[1].Text = "";

                if (BusDefault.DefaultStartAddress < 0)
                    SubItems[2].Text = "";
                else
                    SubItems[2].Text = BusDefault.DefaultStartAddress.ToString();
            }
        }

        private class AddDefaultMenuItem : MenuItem {
            private readonly ControlModuleLocationProperties dialog;
            private readonly ControlBus bus;

            public AddDefaultMenuItem(ControlModuleLocationProperties dialog, ControlBus bus) {
                this.dialog = dialog;
                this.bus = bus;

                Text = bus.BusProvider.NameProvider.Name + " - " + bus.BusType.Name;
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                Dialogs.BusDefaultProperties d = new Dialogs.BusDefaultProperties(bus, null, -1);

                if (d.ShowDialog() == DialogResult.OK) {
                    LayoutControlModuleLocationComponentInfo info = new LayoutControlModuleLocationComponentInfo(dialog.Component, dialog.XmlInfo.Element);
                    ControlModuleLocationBusDefaultInfo busDefault = info.Defaults.Add(bus.Id, d.ModuleTypeName, d.StartingAddress);

                    dialog.listViewDefaults.Items.Add(new Item(busDefault));
                }
            }
        }

        private class CommandStationItem {
            private Guid id;
            private readonly string text;

            public CommandStationItem(IModelComponentIsCommandStation commandStation) {
                this.id = commandStation.Id;
                this.text = commandStation.NameProvider.Name;
            }

            public CommandStationItem(string text) {
                this.id = Guid.Empty;
                this.text = text;
            }

            public override string ToString() => text;

            public Guid ID => id;
        }
    }
}
