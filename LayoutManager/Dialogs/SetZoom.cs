using System;
using System.Windows.Forms;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for SetZoom.
    /// </summary>
    public partial class SetZoom : Form {
        public SetZoom() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        public int ZoomFactor {
            set {
                textBoxZoomFactor.Text = value.ToString();
            }

            get {
                return System.Int32.Parse(textBoxZoomFactor.Text);
            }
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

        private void ButtonOK_Click(object? sender, EventArgs e) {
            try {
                int zoomFactor = zoomFactor = System.Int32.Parse(textBoxZoomFactor.Text);

                if (zoomFactor == 0) {
                    MessageBox.Show(this, "Zoom factor cannot be zero", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxZoomFactor.Focus();
                    DialogResult = DialogResult.None;
                    return;
                }
            }
            catch (Exception) {
                MessageBox.Show(this, "Invalid zoom factor", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxZoomFactor.Focus();
                DialogResult = DialogResult.None;
                return;
            }

            this.Close();
        }
    }
}
