using LayoutManager.Components;
using LayoutManager.Model;
using System.Drawing;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for AdvancedBlockInfoProperties.
    /// </summary>
    public partial class AdvancedBlockInfoProperties : Form {
        private readonly LayoutBlockDefinitionComponentInfo info;
        private readonly PlacementInfo placementInfo;

        public AdvancedBlockInfoProperties(LayoutBlockDefinitionComponentInfo info,  PlacementInfo placementInfo) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.info = info;
            this.placementInfo = placementInfo;

            checkBoxOverrideWaitable.Checked = !info.UseDefaultCanTrainWait;
            checkBoxWaitable.Enabled = checkBoxOverrideWaitable.Checked;
            if (checkBoxOverrideWaitable.Checked)
                checkBoxWaitable.Checked = info.CanTrainWait;
            checkBoxSlowDownRegion.Checked = info.IsSlowdownRegion;

            if (LayoutStraightTrackComponent.IsVertical(placementInfo.Track)) {
                checkBoxFromLeft.Visible = false;
                checkBoxFromRight.Visible = false;

                if (info.IsTripSectionBoundry(0)) {
                    if (placementInfo.Track.ConnectionPoints[0] == LayoutComponentConnectionPoint.T)
                        checkBoxFromTop.Checked = true;
                    else
                        checkBoxFromBottom.Checked = true;
                }

                if (info.IsTripSectionBoundry(1)) {
                    if (placementInfo.Track.ConnectionPoints[1] == LayoutComponentConnectionPoint.T)
                        checkBoxFromTop.Checked = true;
                    else
                        checkBoxFromBottom.Checked = true;
                }
            }
            else {
                checkBoxFromTop.Visible = false;
                checkBoxFromBottom.Visible = false;

                if (info.IsTripSectionBoundry(0)) {
                    if (placementInfo.Track.ConnectionPoints[0] == LayoutComponentConnectionPoint.L)
                        checkBoxFromLeft.Checked = true;
                    else
                        checkBoxFromRight.Checked = true;
                }

                if (info.IsTripSectionBoundry(1)) {
                    if (placementInfo.Track.ConnectionPoints[1] == LayoutComponentConnectionPoint.L)
                        checkBoxFromLeft.Checked = true;
                    else
                        checkBoxFromRight.Checked = true;
                }
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

        private void ButtonOK_Click(object? sender, System.EventArgs e) {
            info.UseDefaultCanTrainWait = !checkBoxOverrideWaitable.Checked;
            if (!info.UseDefaultCanTrainWait)
                info.CanTrainWait = checkBoxWaitable.Checked;

            info.IsSlowdownRegion = checkBoxSlowDownRegion.Checked;

            switch (placementInfo.Track.ConnectionPoints[0]) {
                case LayoutComponentConnectionPoint.T: info.SetTripSectionBoundry(0, checkBoxFromTop.Checked); break;
                case LayoutComponentConnectionPoint.B: info.SetTripSectionBoundry(0, checkBoxFromBottom.Checked); break;
                case LayoutComponentConnectionPoint.L: info.SetTripSectionBoundry(0, checkBoxFromLeft.Checked); break;
                case LayoutComponentConnectionPoint.R: info.SetTripSectionBoundry(0, checkBoxFromRight.Checked); break;
            }

            switch (placementInfo.Track.ConnectionPoints[1]) {
                case LayoutComponentConnectionPoint.T: info.SetTripSectionBoundry(1, checkBoxFromTop.Checked); break;
                case LayoutComponentConnectionPoint.B: info.SetTripSectionBoundry(1, checkBoxFromBottom.Checked); break;
                case LayoutComponentConnectionPoint.L: info.SetTripSectionBoundry(1, checkBoxFromLeft.Checked); break;
                case LayoutComponentConnectionPoint.R: info.SetTripSectionBoundry(1, checkBoxFromRight.Checked); break;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void CheckBoxOverrideWaitable_CheckedChanged(object? sender, System.EventArgs e) {
            checkBoxWaitable.Enabled = checkBoxOverrideWaitable.Checked;
        }

        private void PanelTripSectonBoundry_Paint(object? sender, PaintEventArgs e) {
            e.Graphics.FillRectangle(Brushes.White, 3, 3, 64, 64);
            e.Graphics.DrawRectangle(Pens.Black, 3, 3, 64, 64);
            e.Graphics.TranslateTransform(3, 3);

            LayoutStraightTrackPainter painter = new(new Size(64, 64), placementInfo.Track.ConnectionPoints);

            painter.Paint(e.Graphics);
        }
    }
}
