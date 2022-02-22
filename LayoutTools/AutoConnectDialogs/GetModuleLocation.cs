using LayoutManager.Components;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LayoutManager.Tools.AutoConnectDialogs {
    /// <summary>
    /// Summary description for GetModuleLocation.
    /// </summary>
    public partial class GetModuleLocation : Form {
        public GetModuleLocation(IEnumerable<LayoutControlModuleLocationComponent> moduleLocations) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            comboBoxModuleLocation.Items.Add(new Item("(Any)"));

            foreach (LayoutControlModuleLocationComponent moduleLocation in moduleLocations)
                comboBoxModuleLocation.Items.Add(new Item(moduleLocation));

            if (comboBoxModuleLocation.Items.Count > 0)
                comboBoxModuleLocation.SelectedIndex = 0;
        }

        public LayoutControlModuleLocationComponent? ModuleLocation => comboBoxModuleLocation.SelectedItem == null ? null : ((Item)comboBoxModuleLocation.SelectedItem).ModuleLocation;

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
            DialogResult = DialogResult.OK;
        }

        private class Item {
            private readonly string text;

            public Item(LayoutControlModuleLocationComponent moduleLocation) {
                this.ModuleLocation = moduleLocation;
                text = moduleLocation.Name;
            }

            public Item(string text) {
                this.ModuleLocation = null;
                this.text = text;
            }

            public LayoutControlModuleLocationComponent? ModuleLocation { get; }

            public override string ToString() => text;
        }
    }
}
