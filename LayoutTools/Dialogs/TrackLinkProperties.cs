using LayoutManager.Components;
using LayoutManager.Model;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TrackLinkForm.
    /// </summary>
    public partial class TrackLinkProperties : Form {
        public TrackLinkProperties(LayoutModelArea area, LayoutTrackLinkComponent trackLinkComponent) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            // Set initial fields
            XmlInfo = new LayoutXmlInfo(trackLinkComponent);
            trackLinkTree.ThisComponentLink = new LayoutTrackLink(area.AreaGuid, trackLinkComponent.TrackLinkGuid);

            nameDefinition.DefaultIsVisible = true;
            nameDefinition.XmlInfo = XmlInfo;
            nameDefinition.Component = trackLinkComponent;

            if (trackLinkComponent.Link != null) {
                radioButtonLinked.Checked = true;

                trackLinkTree.SelectedTrackLink = trackLinkComponent.Link;
            }
            else
                radioButtonNotLinked.Checked = true;

            UpdateDependencies();
        }

        /// <summary>
        /// Make sure that all dependencies between controls are maintained
        /// </summary>
        private void UpdateDependencies() {
            trackLinkTree.Enabled = radioButtonLinked.Checked;
        }

        public LayoutXmlInfo XmlInfo { get; }

        public LayoutTrackLink? TrackLink => radioButtonLinked.Checked ? trackLinkTree.SelectedTrackLink : null;

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

        private void CheckBoxVisible_CheckedChanged(object? sender, System.EventArgs e) {
            UpdateDependencies();
        }

        private void RadioButtonNotLinked_CheckedChanged(object? sender, System.EventArgs e) {
            UpdateDependencies();
        }

        private void RadioButtonLinked_CheckedChanged(object? sender, System.EventArgs e) {
            UpdateDependencies();
        }

        private void ButtonOK_Click(object? sender, System.EventArgs e) {
            // Validate the dialog
            if (!nameDefinition.Commit())
                return;

            if (radioButtonLinked.Checked) {
                if (trackLinkTree.SelectedTrackLink == null) {
                    MessageBox.Show(this, "You must select a valid track-link", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    trackLinkTree.Focus();
                    return;
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
