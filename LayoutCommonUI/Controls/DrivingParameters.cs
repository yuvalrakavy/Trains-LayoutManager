using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;

#nullable enable
namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for DrivingParameters.
    /// </summary>
    public class DrivingParameters : System.Windows.Forms.UserControl, IObjectHasXml {
        private const string A_SpeedLimit = "SpeedLimit";
        private const string A_SlowDownSpeed = "SlowDownSpeed";
        private Label label1;
        private Label label2;
        private TextBox textBoxSpeedLimit;
        private Label labelSpeedLimitValues;
        private TextBox textBoxSlowDownSpeed;
        private LayoutManager.CommonUI.Controls.MotionRampSelector motionRampSelectorAcceleration;
        private LayoutManager.CommonUI.Controls.MotionRampSelector motionRampSelectorDeceleration;
        private LayoutManager.CommonUI.Controls.MotionRampSelector motionRampSelectorSlowdown;
        private Label labelSlowDownSpeedValues;
        private LayoutManager.CommonUI.Controls.MotionRampSelector motionRampSelectorStop;

        /// <summary> 
        /// Required designer variable.
        /// </summary>

        private XmlElement? element;

#nullable disable
        public DrivingParameters() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }
#nullable enable

        public XmlElement? OptionalElement {
            get {
                return element;
            }

            set {
                element = value;

                if (element != null)
                    initialize();
            }
        }

        public XmlElement Element {
            get => Ensure.NotNull<XmlElement>(OptionalElement, "Element");
            set => OptionalElement = value;
        }

        private void initialize() {
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

        private void invalidSpeedValue(string title) {
            MessageBox.Show(this, "Invalid " + title + " value (must be empty or an integer number between 1 and " + LayoutModel.Instance.LogicalSpeedSteps + ")", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public bool ValidateValues() {
            if (textBoxSpeedLimit.Text.Trim() != "") {
                int speedLimit;

                try {
                    speedLimit = int.Parse(textBoxSpeedLimit.Text);
                }
                catch (FormatException) {
                    invalidSpeedValue("speed limit");
                    textBoxSpeedLimit.Focus();
                    return false;
                }

                if (speedLimit < 1 || speedLimit > LayoutModel.Instance.LogicalSpeedSteps) {
                    invalidSpeedValue("speed limit");
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
                    invalidSpeedValue("slow-down speed");
                    textBoxSlowDownSpeed.Focus();
                    return false;
                }

                if (slowDownSpeed < 1 || slowDownSpeed > LayoutModel.Instance.LogicalSpeedSteps) {
                    invalidSpeedValue("slow-down speed");
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

                Element.SetAttribute(A_SpeedLimit, speedLimit);
            }
            else
                Element.RemoveAttribute(A_SpeedLimit);

            if (textBoxSlowDownSpeed.Text.Trim() != "") {
                int slowDownSpeed = int.Parse(textBoxSlowDownSpeed.Text);

                Element.SetAttribute(A_SlowDownSpeed, slowDownSpeed);
            }
            else
                Element.RemoveAttribute(A_SlowDownSpeed);

            motionRampSelectorAcceleration.Commit();
            motionRampSelectorDeceleration.Commit();
            motionRampSelectorSlowdown.Commit();

            return true;
        }

#nullable disable
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new Label();
            this.label2 = new Label();
            this.textBoxSpeedLimit = new TextBox();
            this.labelSpeedLimitValues = new Label();
            this.textBoxSlowDownSpeed = new TextBox();
            this.motionRampSelectorAcceleration = new LayoutManager.CommonUI.Controls.MotionRampSelector();
            this.motionRampSelectorDeceleration = new LayoutManager.CommonUI.Controls.MotionRampSelector();
            this.motionRampSelectorSlowdown = new LayoutManager.CommonUI.Controls.MotionRampSelector();
            this.labelSlowDownSpeedValues = new Label();
            this.motionRampSelectorStop = new LayoutManager.CommonUI.Controls.MotionRampSelector();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Speed limit:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(4, 121);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(161, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Before stopping, slow down to:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxSpeedLimit
            // 
            this.textBoxSpeedLimit.Location = new System.Drawing.Point(77, 4);
            this.textBoxSpeedLimit.Name = "textBoxSpeedLimit";
            this.textBoxSpeedLimit.Size = new System.Drawing.Size(48, 20);
            this.textBoxSpeedLimit.TabIndex = 1;
            this.textBoxSpeedLimit.Text = "";
            // 
            // labelSpeedLimitValues
            // 
            this.labelSpeedLimitValues.Location = new System.Drawing.Point(129, 4);
            this.labelSpeedLimitValues.Name = "labelSpeedLimitValues";
            this.labelSpeedLimitValues.Size = new System.Drawing.Size(139, 20);
            this.labelSpeedLimitValues.TabIndex = 2;
            this.labelSpeedLimitValues.Text = "(1-<Limit>)";
            this.labelSpeedLimitValues.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxSlowDownSpeed
            // 
            this.textBoxSlowDownSpeed.Location = new System.Drawing.Point(165, 121);
            this.textBoxSlowDownSpeed.Name = "textBoxSlowDownSpeed";
            this.textBoxSlowDownSpeed.Size = new System.Drawing.Size(48, 20);
            this.textBoxSlowDownSpeed.TabIndex = 6;
            this.textBoxSlowDownSpeed.Text = "";
            // 
            // motionRampSelectorAcceleration
            // 
            this.motionRampSelectorAcceleration.Element = null;
            this.motionRampSelectorAcceleration.Location = new System.Drawing.Point(5, 27);
            this.motionRampSelectorAcceleration.Name = "motionRampSelectorAcceleration";
            this.motionRampSelectorAcceleration.Role = null;
            this.motionRampSelectorAcceleration.Size = new System.Drawing.Size(272, 43);
            this.motionRampSelectorAcceleration.TabIndex = 3;
            this.motionRampSelectorAcceleration.Title = "Acceleration Profile:";
            // 
            // motionRampSelectorDeceleration
            // 
            this.motionRampSelectorDeceleration.Element = null;
            this.motionRampSelectorDeceleration.Location = new System.Drawing.Point(5, 72);
            this.motionRampSelectorDeceleration.Name = "motionRampSelectorDeceleration";
            this.motionRampSelectorDeceleration.Role = null;
            this.motionRampSelectorDeceleration.Size = new System.Drawing.Size(272, 40);
            this.motionRampSelectorDeceleration.TabIndex = 4;
            this.motionRampSelectorDeceleration.Title = "Acceleration Profile:";
            // 
            // motionRampSelectorSlowdown
            // 
            this.motionRampSelectorSlowdown.Element = null;
            this.motionRampSelectorSlowdown.Location = new System.Drawing.Point(5, 140);
            this.motionRampSelectorSlowdown.Name = "motionRampSelectorSlowdown";
            this.motionRampSelectorSlowdown.Role = null;
            this.motionRampSelectorSlowdown.Size = new System.Drawing.Size(272, 48);
            this.motionRampSelectorSlowdown.TabIndex = 8;
            this.motionRampSelectorSlowdown.Title = "Acceleration Profile:";
            // 
            // labelSlowDownSpeedValues
            // 
            this.labelSlowDownSpeedValues.Location = new System.Drawing.Point(221, 121);
            this.labelSlowDownSpeedValues.Name = "labelSlowDownSpeedValues";
            this.labelSlowDownSpeedValues.Size = new System.Drawing.Size(64, 16);
            this.labelSlowDownSpeedValues.TabIndex = 7;
            this.labelSlowDownSpeedValues.Text = "(1-<Limit>)";
            this.labelSlowDownSpeedValues.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // motionRampSelectorStop
            // 
            this.motionRampSelectorStop.Element = null;
            this.motionRampSelectorStop.Location = new System.Drawing.Point(5, 185);
            this.motionRampSelectorStop.Name = "motionRampSelectorStop";
            this.motionRampSelectorStop.Role = null;
            this.motionRampSelectorStop.Size = new System.Drawing.Size(272, 41);
            this.motionRampSelectorStop.TabIndex = 9;
            this.motionRampSelectorStop.Title = "Acceleration Profile:";
            // 
            // DrivingParameters
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.motionRampSelectorStop,
                                                                          this.textBoxSpeedLimit,
                                                                          this.motionRampSelectorAcceleration,
                                                                          this.labelSpeedLimitValues,
                                                                          this.label1,
                                                                          this.label2,
                                                                          this.textBoxSlowDownSpeed,
                                                                          this.motionRampSelectorDeceleration,
                                                                          this.motionRampSelectorSlowdown,
                                                                          this.labelSlowDownSpeedValues});
            this.Name = "DrivingParameters";
            this.Size = new System.Drawing.Size(280, 246);
            this.ResumeLayout(false);
        }
        #endregion
#nullable enable
    }
}
