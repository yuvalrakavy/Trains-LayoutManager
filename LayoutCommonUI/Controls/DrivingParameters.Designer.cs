﻿using LayoutManager.Model;
using System.Xml;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for DrivingParameters.
    /// </summary>
    partial class DrivingParameters : UserControl, IObjectHasXml {
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
            this.motionRampSelectorAcceleration = new MotionRampSelector();
            this.motionRampSelectorDeceleration = new MotionRampSelector();
            this.motionRampSelectorSlowdown = new MotionRampSelector();
            this.labelSlowDownSpeedValues = new Label();
            this.motionRampSelectorStop = new MotionRampSelector();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new Point(6, 4);
            this.label1.Name = "label1";
            this.label1.Size = new Size(64, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Speed limit:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new Point(4, 121);
            this.label2.Name = "label2";
            this.label2.Size = new Size(161, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Before stopping, slow down to:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxSpeedLimit
            // 
            this.textBoxSpeedLimit.Location = new Point(77, 4);
            this.textBoxSpeedLimit.Name = "textBoxSpeedLimit";
            this.textBoxSpeedLimit.Size = new Size(48, 20);
            this.textBoxSpeedLimit.TabIndex = 1;
            this.textBoxSpeedLimit.Text = "";
            // 
            // labelSpeedLimitValues
            // 
            this.labelSpeedLimitValues.Location = new Point(129, 4);
            this.labelSpeedLimitValues.Name = "labelSpeedLimitValues";
            this.labelSpeedLimitValues.Size = new Size(139, 20);
            this.labelSpeedLimitValues.TabIndex = 2;
            this.labelSpeedLimitValues.Text = "(1-<Limit>)";
            this.labelSpeedLimitValues.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxSlowDownSpeed
            // 
            this.textBoxSlowDownSpeed.Location = new Point(165, 121);
            this.textBoxSlowDownSpeed.Name = "textBoxSlowDownSpeed";
            this.textBoxSlowDownSpeed.Size = new Size(48, 20);
            this.textBoxSlowDownSpeed.TabIndex = 6;
            this.textBoxSlowDownSpeed.Text = "";
            // 
            // motionRampSelectorAcceleration
            // 
            this.motionRampSelectorAcceleration.OptionalElement = null;
            this.motionRampSelectorAcceleration.Location = new Point(5, 27);
            this.motionRampSelectorAcceleration.Name = "motionRampSelectorAcceleration";
            this.motionRampSelectorAcceleration.Role = null;
            this.motionRampSelectorAcceleration.Size = new Size(272, 43);
            this.motionRampSelectorAcceleration.TabIndex = 3;
            this.motionRampSelectorAcceleration.Title = "Acceleration Profile:";
            // 
            // motionRampSelectorDeceleration
            // 
            this.motionRampSelectorDeceleration.OptionalElement = null;
            this.motionRampSelectorDeceleration.Location = new Point(5, 72);
            this.motionRampSelectorDeceleration.Name = "motionRampSelectorDeceleration";
            this.motionRampSelectorDeceleration.Role = null;
            this.motionRampSelectorDeceleration.Size = new Size(272, 40);
            this.motionRampSelectorDeceleration.TabIndex = 4;
            this.motionRampSelectorDeceleration.Title = "Acceleration Profile:";
            // 
            // motionRampSelectorSlowdown
            // 
            this.motionRampSelectorSlowdown.OptionalElement = null;
            this.motionRampSelectorSlowdown.Location = new Point(5, 140);
            this.motionRampSelectorSlowdown.Name = "motionRampSelectorSlowdown";
            this.motionRampSelectorSlowdown.Role = null;
            this.motionRampSelectorSlowdown.Size = new Size(272, 48);
            this.motionRampSelectorSlowdown.TabIndex = 8;
            this.motionRampSelectorSlowdown.Title = "Acceleration Profile:";
            // 
            // labelSlowDownSpeedValues
            // 
            this.labelSlowDownSpeedValues.Location = new Point(221, 121);
            this.labelSlowDownSpeedValues.Name = "labelSlowDownSpeedValues";
            this.labelSlowDownSpeedValues.Size = new Size(64, 16);
            this.labelSlowDownSpeedValues.TabIndex = 7;
            this.labelSlowDownSpeedValues.Text = "(1-<Limit>)";
            this.labelSlowDownSpeedValues.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // motionRampSelectorStop
            // 
            this.motionRampSelectorStop.OptionalElement = null;
            this.motionRampSelectorStop.Location = new Point(5, 185);
            this.motionRampSelectorStop.Name = "motionRampSelectorStop";
            this.motionRampSelectorStop.Role = null;
            this.motionRampSelectorStop.Size = new Size(272, 41);
            this.motionRampSelectorStop.TabIndex = 9;
            this.motionRampSelectorStop.Title = "Acceleration Profile:";
            // 
            // DrivingParameters
            // 
            this.Controls.AddRange(new Control[] {
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
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoSize = true;
            this.Size = new Size(280, 246);
            this.ResumeLayout(false);
        }
        #endregion
        private Label label1;
        private Label label2;
        private TextBox textBoxSpeedLimit;
        private Label labelSpeedLimitValues;
        private TextBox textBoxSlowDownSpeed;
        private MotionRampSelector motionRampSelectorAcceleration;
        private MotionRampSelector motionRampSelectorDeceleration;
        private MotionRampSelector motionRampSelectorSlowdown;
        private Label labelSlowDownSpeedValues;
        private MotionRampSelector motionRampSelectorStop;
    }
}
