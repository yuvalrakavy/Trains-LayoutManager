using LayoutManager.Model;
using System.Collections.Generic;
using System.Windows.Forms;
using MethodDispatcher;

namespace LayoutManager.Tools.AutoConnectDialogs {
    /// <summary>
    /// Summary description for GetModuleType.
    /// </summary>
    public partial class GetModuleType : Form {
        public GetModuleType(IList<string> moduleTypeNames) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            foreach (string moduleTypeName in moduleTypeNames) {
                ControlModuleType moduleType = Dispatch.Call.GetControlModuleType(moduleTypeName);

                comboBoxModuleTypes.Items.Add(new Item(moduleType));
            }

            if (comboBoxModuleTypes.Items.Count > 0)
                comboBoxModuleTypes.SelectedIndex = 0;
            checkBoxUseAsDefault.Checked = true;
        }

        public ControlModuleType? ModuleType => comboBoxModuleTypes.SelectedItem == null ? null : ((Item)comboBoxModuleTypes.SelectedItem).ModuleType;

        public bool UseAsDefault => checkBoxUseAsDefault.Checked;

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
            public Item(ControlModuleType moduleType) {
                this.ModuleType = moduleType;
            }

            public ControlModuleType ModuleType { get; }

            public override string ToString() => ModuleType.Name;
        }
    }
}
