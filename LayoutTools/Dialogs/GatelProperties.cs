using LayoutManager.Components;
using LayoutManager.Model;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TrackContactProperties.
    /// </summary>
    public partial class GateProperties : Form, ILayoutComponentPropertiesDialog {
        private readonly bool isVertical;

        private class DriverEntry {
            private readonly XmlElement driverElement;

            public DriverEntry(XmlElement driverElement) {
                this.driverElement = driverElement;
            }

            public string DriverName => driverElement.GetAttribute("Name");

            public override string ToString() => driverElement.GetAttribute("Description");
        }

        public GateProperties(ModelComponent component, PlacementInfo placementInfo) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.component = (LayoutGateComponent)component;
            this.XmlInfo = new LayoutXmlInfo(component);

            nameDefinition.XmlInfo = XmlInfo;
            nameDefinition.Component = component;

            LayoutGateComponentInfo info = new(XmlInfo.Element);

            isVertical = LayoutTrackComponent.IsVertical(placementInfo.Track);

            if (isVertical) {
                radioButtonOpenLeft.Visible = false;
                radioButtonOpenRight.Visible = false;

                if (info.OpenUpOrLeft)
                    radioButtonOpenUp.Checked = true;
                else
                    radioButtonOpenDown.Checked = true;
            }
            else {
                radioButtonOpenUp.Visible = false;
                radioButtonOpenDown.Visible = false;

                if (info.OpenUpOrLeft)
                    radioButtonOpenLeft.Checked = true;
                else
                    radioButtonOpenRight.Checked = true;
            }

            textBoxOpenCloseTimeout.Text = info.MotionTimeout.ToString();
            checkBoxReverseDirection.Checked = info.ReverseDirection;
            checkBoxRevreseMotion.Checked = info.ReverseMotion;

            if (info.TwoDirectionRelays)
                radioButtonTwoRelays.Checked = true;
            else
                radioButtonSingleRelay.Checked = true;

            textBoxGateMotionTime.Text = info.MotionTime.ToString();

            panelGateMotionTime.Enabled = false;

            switch (info.FeedbackType) {
                case LayoutGateComponentInfo.FeedbackTypes.NoFeedback:
                    radioButtonNoFeedback.Checked = true;
                    panelGateMotionTime.Enabled = true;
                    break;

                case LayoutGateComponentInfo.FeedbackTypes.OneSensor:
                    radioButtonOneSensor.Checked = true;
                    break;

                case LayoutGateComponentInfo.FeedbackTypes.TwoSensors:
                    radioButtonTwoSensors.Checked = true;
                    break;
            }

            panelGatePreview.Invalidate();
            UpdateControls();
        }

        public LayoutXmlInfo XmlInfo { get; }

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

        private void UpdateControls() {
            groupBoxMotionControl.Enabled = radioButtonSingleRelay.Checked;
        }

        private void ButtonOK_Click(object? sender, System.EventArgs e) {
            LayoutGateComponentInfo info = new(XmlInfo.Element);

            if (!nameDefinition.Commit())
                return;

            if (!int.TryParse(textBoxOpenCloseTimeout.Text, out int timeout)) {
                MessageBox.Show("Invalid timeout value");
                textBoxOpenCloseTimeout.Focus();
                return;
            }

            if (radioButtonNoFeedback.Checked) {
                if (!int.TryParse(textBoxGateMotionTime.Text, out int motionTime)) {
                    MessageBox.Show("Invalid gate motion time value");
                    textBoxGateMotionTime.Focus();
                    return;
                }

                info.MotionTime = motionTime;
                info.FeedbackType = LayoutGateComponentInfo.FeedbackTypes.NoFeedback;
            }

            if (radioButtonOneSensor.Checked)
                info.FeedbackType = LayoutGateComponentInfo.FeedbackTypes.OneSensor;

            if (radioButtonTwoSensors.Checked)
                info.FeedbackType = LayoutGateComponentInfo.FeedbackTypes.TwoSensors;

            info.OpenUpOrLeft = radioButtonOpenUp.Checked || radioButtonOpenLeft.Checked;
            info.ReverseDirection = checkBoxReverseDirection.Checked;
            info.ReverseMotion = checkBoxRevreseMotion.Checked;
            info.MotionTimeout = timeout;

            info.TwoDirectionRelays = radioButtonTwoRelays.Checked;

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void PanelGatePreview_Paint(object? sender, PaintEventArgs e) {
            e.Graphics.FillRectangle(Brushes.White, 3, 3, 64, 64);
            e.Graphics.DrawRectangle(Pens.Black, 3, 3, 64, 64);
            e.Graphics.TranslateTransform(3, 3);

            LayoutComponentConnectionPoint[] cps;

            if (isVertical)
                cps = new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B };
            else
                cps = new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.R };

            LayoutStraightTrackPainter trackPainter = new(new Size(64, 64), cps);

            trackPainter.Paint(e.Graphics);

            LayoutGatePainter gatePainter = new(new Size(64, 64), isVertical, radioButtonOpenUp.Checked || radioButtonOpenLeft.Checked, 60);

            gatePainter.Paint(e.Graphics);
        }

        private void GateOpenDirectionChanged(object? sender, EventArgs e) {
            panelGatePreview.Invalidate();
        }

        private void RadioButtonFeedback_CheckedChanged(object? sender, EventArgs e) {
            panelGateMotionTime.Enabled = radioButtonNoFeedback.Checked;
        }

        private void DirectionControlChanged(object? sender, EventArgs e) => UpdateControls();
    }
}
