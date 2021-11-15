using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using LayoutManager.Components;

namespace LayoutManager.Tools.AutoConnectDialogs {
    /// <summary>
    /// Summary description for GetNoSpaceSelection.
    /// </summary>
    public partial class GetNoSpaceSelection : Form {
        public GetNoSpaceSelection(LayoutControlModuleLocationComponent noSpaceLocation, IEnumerable<LayoutControlModuleLocationComponent?> otherModuleLocations) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            labelTitle.Text = Regex.Replace(labelTitle.Text, "NAME", noSpaceLocation.Name);

            foreach (var moduleLocation in otherModuleLocations)
                if (moduleLocation == null)
                    comboBoxModuleLocation.Items.Add(new Item("(Not in any location)"));
                else
                    comboBoxModuleLocation.Items.Add(moduleLocation.Name);

            if (comboBoxModuleLocation.Items.Count > 0)
                comboBoxModuleLocation.SelectedIndex = 0;

            radioButtonAddNewModule.Checked = true;

            UpdateButtons();
        }

        private void UpdateButtons() {
            comboBoxModuleLocation.Enabled = !radioButtonAddNewModule.Checked;
        }

        public bool AddNewModule => radioButtonAddNewModule.Checked;

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

        private void RadioButtonAddNewModule_CheckedChanged(object? sender, System.EventArgs e) {
            UpdateButtons();
        }

        private void RadioButtonUseModuleLocation_CheckedChanged(object? sender, System.EventArgs e) {
            UpdateButtons();
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
