using System;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveFront.
    /// </summary>
    public partial class TrainFront : Form {
        public TrainFront(LayoutBlockDefinitionComponent blockInfo, string name) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            locomotiveFrontControl.ConnectionPoints = blockInfo.Track.ConnectionPoints;
            locomotiveFrontControl.LocomotiveName = name;
            labelBlockName.Text = blockInfo.NameProvider.Name;
        }

        public TrainFront(LayoutStraightTrackComponent track, string locoName, string trackName) {
            InitializeComponent();

            locomotiveFrontControl.ConnectionPoints = track.ConnectionPoints;
            locomotiveFrontControl.LocomotiveName = locoName;
            labelBlockName.Text = trackName;
        }

        public LayoutComponentConnectionPoint Front {
            get {
                return locomotiveFrontControl.Front;
            }

            set {
                locomotiveFrontControl.Front = value;
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

        private void ButtonOk_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
