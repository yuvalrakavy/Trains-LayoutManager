using LayoutManager.Model;
using System.Windows.Forms;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for StandardPositionProperties.
    /// </summary>
    public partial class StandardPositionProperties : Form {
        public StandardPositionProperties(LayoutPositionInfo positionProvider) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            positionDefinition.Set(positionProvider);
            textBoxTitle.Text = positionProvider.GetAttribute("Title");
            textBoxRef.Text = positionProvider.Ref;
        }

        public void Get(LayoutPositionInfo positionProvider) {
            positionDefinition.Get(positionProvider);
            positionProvider.SetAttributeValue("Title", textBoxTitle.Text);
            if (textBoxRef.Text.Trim() == "")
                positionProvider.Ref = null;
            else
                positionProvider.Ref = textBoxRef.Text;
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
            if (textBoxTitle.Text.Trim() == "") {
                MessageBox.Show(this, "You must specify title", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxTitle.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
