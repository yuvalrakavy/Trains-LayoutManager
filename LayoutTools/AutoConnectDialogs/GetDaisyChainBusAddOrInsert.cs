using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using LayoutManager.Components;

namespace LayoutManager.Tools.AutoConnectDialogs {
    /// <summary>
    /// Summary description for GetDaisyChainBusAddOrInsert.
    /// </summary>
    public partial class GetDaisyChainBusAddOrInsert : Form {

        public GetDaisyChainBusAddOrInsert(LayoutControlModuleLocationComponent? moduleLocation) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            if(moduleLocation != null)
                labelTitle.Text = Regex.Replace(labelTitle.Text, "LOCATION", moduleLocation.Name);

            radioButtonInsert.Checked = true;
        }

        public bool InsertModule => radioButtonInsert.Checked;

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
    }
}
