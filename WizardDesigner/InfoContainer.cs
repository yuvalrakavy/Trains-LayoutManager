using System.ComponentModel;
using System.Drawing;

namespace Gui.Wizard {
    /// <summary>
    /// Summary description for UserControl1.
    /// </summary>
    [Designer(typeof(InfoContainerDesigner))]
    public partial class InfoContainer : System.Windows.Forms.UserControl {
        /// <summary>
        /// 
        /// </summary>
        public InfoContainer() {
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


        private void InfoContainer_Load(object? sender, System.EventArgs e) {
            //Handle really irating resize that doesn't take account of Anchor
            lblTitle.Left = picImage.Width + 8;
            lblTitle.Width = this.Width - 4 - lblTitle.Left;
        }

        /// <summary>
        /// Get/Set the title for the info page
        /// </summary>
        [Category("Appearance")]
        public string PageTitle {
            get {
                return lblTitle.Text;
            }
            set {
                lblTitle.Text = value;
            }
        }

        /// <summary>
        /// Gets/Sets the Icon
        /// </summary>
        [Category("Appearance")]
        public Image Image {
            get {
                return picImage.Image;
            }
            set {
                picImage.Image = value;
            }
        }
    }
}
