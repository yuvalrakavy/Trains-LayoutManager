using LayoutManager.CommonUI;
using LayoutManager.Components;
using LayoutManager.Model;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for ControlModuleLocationProperties.
    /// </summary>
    public partial class ControlModuleLocationProperties : Form, ILayoutComponentPropertiesDialog {

        public ControlModuleLocationProperties(ModelComponent component) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.Component = (LayoutControlModuleLocationComponent)component;
            this.XmlInfo = new LayoutXmlInfo(component);

            nameDefinition.XmlInfo = XmlInfo;
            nameDefinition.Component = component;

            LayoutControlModuleLocationComponentInfo info = new(Component, XmlInfo.Element);

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

            UpdateButtons();
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

        private void UpdateButtons() {
            if (listViewDefaults.SelectedItems.Count == 0) {
                buttonEdit.Enabled = false;
                buttonRemove.Enabled = false;
            }
            else {
                buttonEdit.Enabled = true;
                buttonRemove.Enabled = true;
            }
        }

        private void ButtonOK_Click(object? sender, System.EventArgs e) {
            if (nameDefinition.IsEmptyName) {
                MessageBox.Show(this, "You have to provide a name for control module location",
                    "Missing Control module location Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                nameDefinition.Focus();
                return;
            }

            if (!nameDefinition.Commit())
                return;

            LayoutControlModuleLocationComponentInfo _ = new(Component, XmlInfo.Element) {
                CommandStationId = ((CommandStationItem)comboBoxCommandStation.SelectedItem).ID
            };

            DialogResult = DialogResult.OK;
        }

        private void ButtonAdd_Click(object? sender, System.EventArgs e) {
            var menu = new ContextMenuStrip();

            foreach (XmlElement busElement in LayoutModel.ControlManager.BusesElement) {
                ControlBus bus = new(LayoutModel.ControlManager, busElement);
                bool found = false;

                foreach (Item item in listViewDefaults.Items)
                    if (item.BusDefault.BusId == bus.Id) {
                        found = true;
                        break;
                    }

                if (!found)
                    menu.Items.Add(new AddDefaultMenuItem(this, bus));
            }

            if (menu.Items.Count == 0) {
                var item = new LayoutMenuItem("No more control bus can be added") {
                    Enabled = false
                };
                menu.Items.Add(item);
            }

            menu.Show(buttonAdd.Parent, new Point(buttonAdd.Left, buttonAdd.Bottom));
        }

        private void ButtonEdit_Click(object? sender, System.EventArgs e) {
            if (listViewDefaults.SelectedItems.Count > 0) {
                var selected = (Item)listViewDefaults.SelectedItems[0]!;
                Dialogs.BusDefaultProperties d = new(selected.BusDefault.Bus, selected.BusDefault.DefaultModuleTypeName, selected.BusDefault.DefaultStartAddress);

                if (d.ShowDialog() == DialogResult.OK) {
                    selected.BusDefault.DefaultModuleTypeName = d.ModuleTypeName ?? String.Empty;
                    selected.BusDefault.DefaultStartAddress = d.StartingAddress;

                    selected.Update();
                    UpdateButtons();
                }
            }
        }

        private void ButtonRemove_Click(object? sender, System.EventArgs e) {
            if (listViewDefaults.SelectedItems.Count > 0) {
                Item selected = (Item)listViewDefaults.SelectedItems[0];
                LayoutControlModuleLocationComponentInfo info = new(Component, XmlInfo.Element);

                info.Defaults.Remove(selected.BusDefault.BusId);
                listViewDefaults.Items.Remove(selected);
                UpdateButtons();
            }
        }

        private void ListViewDefaults_SelectedIndexChanged(object? sender, System.EventArgs e) {
            UpdateButtons();
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

        private class AddDefaultMenuItem : LayoutMenuItem {
            private readonly ControlModuleLocationProperties dialog;
            private readonly ControlBus bus;

            public AddDefaultMenuItem(ControlModuleLocationProperties dialog, ControlBus bus) {
                this.dialog = dialog;
                this.bus = bus;

                Text = bus.BusProvider.NameProvider.Name + " - " + bus.BusType.Name;
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                Dialogs.BusDefaultProperties d = new(bus, null, -1);

                if (d.ShowDialog() == DialogResult.OK) {
                    LayoutControlModuleLocationComponentInfo info = new(dialog.Component, dialog.XmlInfo.Element);
                    ControlModuleLocationBusDefaultInfo busDefault = info.Defaults.Add(bus.Id, d.ModuleTypeName ?? String.Empty, d.StartingAddress);

                    dialog.listViewDefaults.Items.Add(new Item(busDefault));
                }
            }
        }

        private class CommandStationItem {
            private readonly Guid id;
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
