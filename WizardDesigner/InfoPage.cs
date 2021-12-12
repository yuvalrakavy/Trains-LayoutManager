using System.ComponentModel;
using System.Windows.Forms;

namespace Gui.Wizard {
    /// <summary>
    /// An inherited <see cref="InfoContainer"/> that contains a <see cref="Label"/> 
    /// with the description of the page.
    /// </summary>
    public partial class InfoPage : InfoContainer {

        /// <summary>
        /// Default Constructor
        /// </summary>
        public InfoPage() {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call
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


        /// <summary>
        /// Gets/Sets the text on the info page
        /// </summary>
        [Category("Appearance")]
        public string PageText {
            get {
                return lblDescription.Text;
            }
            set {
                lblDescription.Text = value;
            }
        }
    }
}

