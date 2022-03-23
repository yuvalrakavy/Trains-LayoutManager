using System;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for DefaultDrivingParameters.
    /// </summary>
    partial class DefaultDrivingParameters : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new Label();
            this.textBoxSpeedLimit = new TextBox();
            this.labelSpeedLimitValues = new Label();
            this.groupBox1 = new GroupBox();
            this.motionRampWithCopyEditorAcceleration = new LayoutManager.CommonUI.Controls.MotionRampWithCopyEditor();
            this.groupBox2 = new GroupBox();
            this.motionRampWithCopyEditorDeceleration = new LayoutManager.CommonUI.Controls.MotionRampWithCopyEditor();
            this.label3 = new Label();
            this.textBoxSlowdown = new TextBox();
            this.labelSlowdownValues = new Label();
            this.groupBox3 = new GroupBox();
            this.motionRampWithCopyEditorSlowdown = new LayoutManager.CommonUI.Controls.MotionRampWithCopyEditor();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.groupBox4 = new GroupBox();
            this.motionRampWithCopyEditorStop = new LayoutManager.CommonUI.Controls.MotionRampWithCopyEditor();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Default speed limit:";
            // 
            // textBoxSpeedLimit
            // 
            this.textBoxSpeedLimit.Location = new System.Drawing.Point(120, 14);
            this.textBoxSpeedLimit.Name = "textBoxSpeedLimit";
            this.textBoxSpeedLimit.Size = new System.Drawing.Size(48, 20);
            this.textBoxSpeedLimit.TabIndex = 1;
            this.textBoxSpeedLimit.Text = "";
            // 
            // labelSpeedLimitValues
            // 
            this.labelSpeedLimitValues.Location = new System.Drawing.Point(176, 16);
            this.labelSpeedLimitValues.Name = "labelSpeedLimitValues";
            this.labelSpeedLimitValues.Size = new System.Drawing.Size(100, 16);
            this.labelSpeedLimitValues.TabIndex = 2;
            this.labelSpeedLimitValues.Text = "(1-<Limit>)";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                    this.motionRampWithCopyEditorAcceleration});
            this.groupBox1.Location = new System.Drawing.Point(10, 48);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(240, 152);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Default Acceleration Profile:";
            // 
            // motionRampWithCopyEditorAcceleration
            // 
            this.motionRampWithCopyEditorAcceleration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.motionRampWithCopyEditorAcceleration.Location = new System.Drawing.Point(3, 16);
            this.motionRampWithCopyEditorAcceleration.Name = "motionRampWithCopyEditorAcceleration";
            this.motionRampWithCopyEditorAcceleration.Ramp = null;
            this.motionRampWithCopyEditorAcceleration.Size = new System.Drawing.Size(234, 133);
            this.motionRampWithCopyEditorAcceleration.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                    this.motionRampWithCopyEditorDeceleration});
            this.groupBox2.Location = new System.Drawing.Point(258, 48);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(240, 152);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Default Deceleration Profile:";
            // 
            // motionRampWithCopyEditorDeceleration
            // 
            this.motionRampWithCopyEditorDeceleration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.motionRampWithCopyEditorDeceleration.Location = new System.Drawing.Point(3, 16);
            this.motionRampWithCopyEditorDeceleration.Name = "motionRampWithCopyEditorDeceleration";
            this.motionRampWithCopyEditorDeceleration.Ramp = null;
            this.motionRampWithCopyEditorDeceleration.Size = new System.Drawing.Size(234, 133);
            this.motionRampWithCopyEditorDeceleration.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(16, 228);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(322, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "By default, when slowing down (before stopping), slow down to:";
            // 
            // textBoxSlowdown
            // 
            this.textBoxSlowdown.Location = new System.Drawing.Point(338, 226);
            this.textBoxSlowdown.Name = "textBoxSlowdown";
            this.textBoxSlowdown.Size = new System.Drawing.Size(48, 20);
            this.textBoxSlowdown.TabIndex = 1;
            this.textBoxSlowdown.Text = "";
            // 
            // labelSlowdownValues
            // 
            this.labelSlowdownValues.Location = new System.Drawing.Point(394, 226);
            this.labelSlowdownValues.Name = "labelSlowdownValues";
            this.labelSlowdownValues.Size = new System.Drawing.Size(100, 20);
            this.labelSlowdownValues.TabIndex = 2;
            this.labelSlowdownValues.Text = "(1-<Limit>)";
            this.labelSlowdownValues.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                    this.motionRampWithCopyEditorSlowdown});
            this.groupBox3.Location = new System.Drawing.Point(8, 256);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(240, 152);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Default Slowdown Deceleration Profile:";
            // 
            // motionRampWithCopyEditorSlowdown
            // 
            this.motionRampWithCopyEditorSlowdown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.motionRampWithCopyEditorSlowdown.Location = new System.Drawing.Point(3, 16);
            this.motionRampWithCopyEditorSlowdown.Name = "motionRampWithCopyEditorSlowdown";
            this.motionRampWithCopyEditorSlowdown.Ramp = null;
            this.motionRampWithCopyEditorSlowdown.Size = new System.Drawing.Size(234, 133);
            this.motionRampWithCopyEditorSlowdown.TabIndex = 0;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(344, 416);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(429, 416);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new EventHandler(this.ButtonCancel_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                    this.motionRampWithCopyEditorStop});
            this.groupBox4.Location = new System.Drawing.Point(264, 256);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(240, 152);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Default Stop Profile:";
            // 
            // motionRampWithCopyEditorStop
            // 
            this.motionRampWithCopyEditorStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.motionRampWithCopyEditorStop.Location = new System.Drawing.Point(3, 16);
            this.motionRampWithCopyEditorStop.Name = "motionRampWithCopyEditorStop";
            this.motionRampWithCopyEditorStop.Ramp = null;
            this.motionRampWithCopyEditorStop.Size = new System.Drawing.Size(234, 133);
            this.motionRampWithCopyEditorStop.TabIndex = 0;
            // 
            // DefaultDrivingParameters
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(512, 448);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonOK,
                                                                          this.label3,
                                                                          this.groupBox1,
                                                                          this.labelSpeedLimitValues,
                                                                          this.textBoxSpeedLimit,
                                                                          this.label1,
                                                                          this.groupBox2,
                                                                          this.textBoxSlowdown,
                                                                          this.labelSlowdownValues,
                                                                          this.groupBox3,
                                                                          this.buttonCancel,
                                                                          this.groupBox4});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DefaultDrivingParameters";
            this.Text = "Default Train Driving Parameters";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion
        private Label label1;
        private TextBox textBoxSpeedLimit;
        private Label labelSpeedLimitValues;
        private GroupBox groupBox1;
        private LayoutManager.CommonUI.Controls.MotionRampWithCopyEditor motionRampWithCopyEditorAcceleration;
        private GroupBox groupBox2;
        private LayoutManager.CommonUI.Controls.MotionRampWithCopyEditor motionRampWithCopyEditorDeceleration;
        private Label label3;
        private TextBox textBoxSlowdown;
        private Label labelSlowdownValues;
        private GroupBox groupBox3;
        private LayoutManager.CommonUI.Controls.MotionRampWithCopyEditor motionRampWithCopyEditorSlowdown;
        private Button buttonOK;
        private Button buttonCancel;
        private GroupBox groupBox4;
        private LayoutManager.CommonUI.Controls.MotionRampWithCopyEditor motionRampWithCopyEditorStop;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}