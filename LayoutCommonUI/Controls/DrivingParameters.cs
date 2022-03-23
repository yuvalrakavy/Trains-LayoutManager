using LayoutManager.Model;
using System.Xml;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for DrivingParameters.
    /// </summary>
    public partial class DrivingParameters : UserControl, IObjectHasXml {
        private const string A_SpeedLimit = "SpeedLimit";
        private const string A_SlowDownSpeed = "SlowDownSpeed";

        /// <summary> 
        /// Required designer variable.
        /// </summary>

        private XmlElement? element;

        public DrivingParameters() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        public XmlElement? OptionalElement {
            get {
                return element;
            }

            set {
                element = value;

                if (element != null)
                    Initialize();
            }
        }

        public XmlElement Element {
            get => Ensure.NotNull<XmlElement>(OptionalElement, "Element");
            set => OptionalElement = value;
        }

        private void Initialize() {
            if (Element.HasAttribute(A_SpeedLimit))
                textBoxSpeedLimit.Text = Element.GetAttribute(A_SpeedLimit);
            labelSpeedLimitValues.Text = "(1-" + LayoutModel.Instance.LogicalSpeedSteps + ", empty for default)";

            motionRampSelectorAcceleration.Title = "Acceleration Profile:";
            motionRampSelectorAcceleration.Role = "Acceleration";
            motionRampSelectorAcceleration.Element = Element;

            motionRampSelectorDeceleration.Title = "Deceleration Profile:";
            motionRampSelectorDeceleration.Role = "Deaceleration";
            motionRampSelectorDeceleration.Element = Element;

            if (Element.HasAttribute(A_SlowDownSpeed))
                textBoxSlowDownSpeed.Text = Element.GetAttribute(A_SlowDownSpeed);
            labelSlowDownSpeedValues.Text = "(1-" + LayoutModel.Instance.LogicalSpeedSteps + ")";

            motionRampSelectorSlowdown.Title = "Slow-down Profile:";
            motionRampSelectorSlowdown.Role = "SlowDown";
            motionRampSelectorSlowdown.Element = Element;

            motionRampSelectorStop.Title = "Stop Profile:";
            motionRampSelectorStop.Role = "Stop";
            motionRampSelectorStop.Element = Element;
        }

        private void InvalidSpeedValue(string title) {
            MessageBox.Show(this, "Invalid " + title + " value (must be empty or an integer number between 1 and " + LayoutModel.Instance.LogicalSpeedSteps + ")", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public bool ValidateValues() {
            if (textBoxSpeedLimit.Text.Trim() != "") {
                int speedLimit;

                try {
                    speedLimit = int.Parse(textBoxSpeedLimit.Text);
                }
                catch (FormatException) {
                    InvalidSpeedValue("speed limit");
                    textBoxSpeedLimit.Focus();
                    return false;
                }

                if (speedLimit < 1 || speedLimit > LayoutModel.Instance.LogicalSpeedSteps) {
                    InvalidSpeedValue("speed limit");
                    textBoxSpeedLimit.Focus();
                    return false;
                }
            }

            if (!motionRampSelectorAcceleration.ValidateValues() ||
                !motionRampSelectorDeceleration.ValidateValues())
                return false;

            if (textBoxSlowDownSpeed.Text.Trim() != "") {
                int slowDownSpeed;

                try {
                    slowDownSpeed = int.Parse(textBoxSlowDownSpeed.Text);
                }
                catch (FormatException) {
                    InvalidSpeedValue("slow-down speed");
                    textBoxSlowDownSpeed.Focus();
                    return false;
                }

                if (slowDownSpeed < 1 || slowDownSpeed > LayoutModel.Instance.LogicalSpeedSteps) {
                    InvalidSpeedValue("slow-down speed");
                    textBoxSlowDownSpeed.Focus();
                    return false;
                }
            }

            return motionRampSelectorSlowdown.ValidateValues();
        }

        public bool Commit() {
            if (!ValidateValues())
                return false;

            if (textBoxSpeedLimit.Text.Trim() != "") {
                int speedLimit = int.Parse(textBoxSpeedLimit.Text);

                Element.SetAttributeValue(A_SpeedLimit, speedLimit);
            }
            else
                Element.RemoveAttribute(A_SpeedLimit);

            if (textBoxSlowDownSpeed.Text.Trim() != "") {
                int slowDownSpeed = int.Parse(textBoxSlowDownSpeed.Text);

                Element.SetAttributeValue(A_SlowDownSpeed, slowDownSpeed);
            }
            else
                Element.RemoveAttribute(A_SlowDownSpeed);

            motionRampSelectorAcceleration.Commit();
            motionRampSelectorDeceleration.Commit();
            motionRampSelectorSlowdown.Commit();

            return true;
        }

    }
}
