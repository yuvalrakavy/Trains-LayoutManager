using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Gui.Wizard {
    /// <summary>
    /// Summary description for WizardHeader.
    /// </summary>
    [Designer("Gui.Wizard.HeaderDesigner", "WizardDesigner.dll")]
    public partial class Header : UserControl {
        /// <summary>
        /// Constructor for Header
        /// </summary>
        public Header() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void ResizeImageAndText() {
            //Resize image 
            picIcon.Size = picIcon.Image.Size;
            //Relocate image according to its size
            picIcon.Top = (this.Height - picIcon.Height) / 2;
            picIcon.Left = this.Width - picIcon.Width - 8;
            //Fit text around picture
            lblTitle.Width = picIcon.Left - lblTitle.Left;
            lblDescription.Width = picIcon.Left - lblDescription.Left;
        }

        private void Header_SizeChanged(object? sender, System.EventArgs e) {
            ResizeImageAndText();
        }

        /// <summary>
        /// Get/Set the title for the wizard page
        /// </summary>
        [Category("Appearance")]
        public string Title {
            get {
                return lblTitle.Text;
            }
            set {
                lblTitle.Text = value;
            }
        }

        /// <summary>
        /// Gets/Sets the
        /// </summary>
        [Category("Appearance")]
        public string Description {
            get {
                return lblDescription.Text;
            }
            set {
                lblDescription.Text = value;
            }
        }

        /// <summary>
        /// Gets/Sets the Icon
        /// </summary>
        [Category("Appearance")]
        public Image Image {
            get {
                return picIcon.Image;
            }
            set {
                picIcon.Image = value;
                ResizeImageAndText();
            }
        }
    }
}
