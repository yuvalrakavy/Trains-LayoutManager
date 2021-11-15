using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for GetNumberOfModules.
    /// </summary>
    public partial class GetNumberOfModules : Form {
        public GetNumberOfModules(string moduleName, int maxModules) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            labelQuestion.Text = Regex.Replace(labelQuestion.Text, "MODULENAME", moduleName);
            Text = Regex.Replace(Text, "MODULENAME", moduleName);

            numericUpDownModuleCount.Maximum = maxModules;
        }

        public int Count => (int)numericUpDownModuleCount.Value;

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
