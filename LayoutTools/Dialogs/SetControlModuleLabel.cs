using LayoutManager.Model;
using System.Windows.Forms;
using MethodDispatcher;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for SetControlModuleLabel.
    /// </summary>
    public partial class SetControlModuleLabel : Form {
        public SetControlModuleLabel(ControlModule module) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.module = module;

            if (module.Label == null)
                textBoxLabel.Text = "";
            else
                textBoxLabel.Text = module.Label;
        }

        public string? Label => string.IsNullOrWhiteSpace(textBoxLabel.Text) ? null : textBoxLabel.Text;

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
            var validationError = Dispatch.Call.ValidateControlModuleLabel(module, textBoxLabel.Text);

            if (validationError != null) {
                MessageBox.Show(validationError, "Invalid label", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxLabel.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
        }
    }
}
