using LayoutManager.Components;
using LayoutManager.Model;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LayoutManager.Tools.AutoConnectDialogs {
    /// <summary>
    /// Summary description for AnnounceCreateNewModule.
    /// </summary>
    public partial class AnnounceCreateNewModule : Form {
        public AnnounceCreateNewModule(ControlModuleType moduleType, LayoutControlModuleLocationComponent? moduleLocation) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            labelModuleName.Text = Regex.Replace(labelModuleName.Text, "MODULETYPE", moduleType.Name);

            if (moduleLocation != null)
                labelModuleLocation.Text = Regex.Replace(labelModuleLocation.Text, "LOCATION", moduleLocation.Name);
            else
                labelModuleLocation.Visible = false;
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

        private void ButtonOK_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.OK;
        }
    }
}
