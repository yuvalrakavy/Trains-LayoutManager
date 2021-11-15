using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TrackContactProperties.
    /// </summary>
    partial class GateProperties : Form, ILayoutComponentPropertiesDialog {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonOpenLeft = new System.Windows.Forms.RadioButton();
            this.radioButtonOpenRight = new System.Windows.Forms.RadioButton();
            this.radioButtonOpenDown = new System.Windows.Forms.RadioButton();
            this.panelGatePreview = new System.Windows.Forms.Panel();
            this.radioButtonOpenUp = new System.Windows.Forms.RadioButton();
            this.nameDefinition = new LayoutManager.CommonUI.Controls.NameDefinition();
            this.tabPageMotion = new System.Windows.Forms.TabPage();
            this.groupBoxMotionControl = new System.Windows.Forms.GroupBox();
            this.checkBoxRevreseMotion = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxReverseDirection = new System.Windows.Forms.CheckBox();
            this.radioButtonTwoRelays = new System.Windows.Forms.RadioButton();
            this.radioButtonSingleRelay = new System.Windows.Forms.RadioButton();
            this.tabPageFeedback = new System.Windows.Forms.TabPage();
            this.panelGateMotionTime = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxGateMotionTime = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonNoFeedback = new System.Windows.Forms.RadioButton();
            this.radioButtonOneSensor = new System.Windows.Forms.RadioButton();
            this.radioButtonTwoSensors = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxOpenCloseTimeout = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPageMotion.SuspendLayout();
            this.groupBoxMotionControl.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPageFeedback.SuspendLayout();
            this.panelGateMotionTime.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(208, 272);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(120, 272);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += this.ButtonOK_Click;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageGeneral);
            this.tabControl.Controls.Add(this.tabPageMotion);
            this.tabControl.Controls.Add(this.tabPageFeedback);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(290, 264);
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControl.TabIndex = 0;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.groupBox1);
            this.tabPageGeneral.Controls.Add(this.nameDefinition);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 29);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Size = new System.Drawing.Size(282, 231);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonOpenLeft);
            this.groupBox1.Controls.Add(this.radioButtonOpenRight);
            this.groupBox1.Controls.Add(this.radioButtonOpenDown);
            this.groupBox1.Controls.Add(this.panelGatePreview);
            this.groupBox1.Controls.Add(this.radioButtonOpenUp);
            this.groupBox1.Location = new System.Drawing.Point(25, 76);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(240, 137);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Gate opening direction:";
            // 
            // radioButtonOpenLeft
            // 
            this.radioButtonOpenLeft.AutoSize = true;
            this.radioButtonOpenLeft.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.radioButtonOpenLeft.Location = new System.Drawing.Point(40, 65);
            this.radioButtonOpenLeft.Name = "radioButtonOpenLeft";
            this.radioButtonOpenLeft.Size = new System.Drawing.Size(62, 24);
            this.radioButtonOpenLeft.TabIndex = 6;
            this.radioButtonOpenLeft.Text = "&Left";
            this.radioButtonOpenLeft.CheckedChanged += this.GateOpenDirectionChanged;
            // 
            // radioButtonOpenRight
            // 
            this.radioButtonOpenRight.AutoSize = true;
            this.radioButtonOpenRight.Location = new System.Drawing.Point(161, 65);
            this.radioButtonOpenRight.Name = "radioButtonOpenRight";
            this.radioButtonOpenRight.Size = new System.Drawing.Size(72, 24);
            this.radioButtonOpenRight.TabIndex = 5;
            this.radioButtonOpenRight.Text = "&Right";
            this.radioButtonOpenRight.CheckedChanged += this.GateOpenDirectionChanged;
            // 
            // radioButtonOpenDown
            // 
            this.radioButtonOpenDown.AutoSize = true;
            this.radioButtonOpenDown.Location = new System.Drawing.Point(115, 111);
            this.radioButtonOpenDown.Name = "radioButtonOpenDown";
            this.radioButtonOpenDown.Size = new System.Drawing.Size(75, 24);
            this.radioButtonOpenDown.TabIndex = 4;
            this.radioButtonOpenDown.Text = "&Down";
            this.radioButtonOpenDown.CheckedChanged += this.GateOpenDirectionChanged;
            // 
            // panelGatePreview
            // 
            this.panelGatePreview.Location = new System.Drawing.Point(85, 39);
            this.panelGatePreview.Name = "panelGatePreview";
            this.panelGatePreview.Size = new System.Drawing.Size(70, 70);
            this.panelGatePreview.TabIndex = 3;
            this.panelGatePreview.Paint += this.PanelGatePreview_Paint;
            // 
            // radioButtonOpenUp
            // 
            this.radioButtonOpenUp.AutoSize = true;
            this.radioButtonOpenUp.Location = new System.Drawing.Point(115, 20);
            this.radioButtonOpenUp.Name = "radioButtonOpenUp";
            this.radioButtonOpenUp.Size = new System.Drawing.Size(55, 24);
            this.radioButtonOpenUp.TabIndex = 2;
            this.radioButtonOpenUp.Text = "&Up";
            this.radioButtonOpenUp.CheckedChanged += this.GateOpenDirectionChanged;
            // 
            // nameDefinition
            // 
            this.nameDefinition.Component = null;
            this.nameDefinition.DefaultIsVisible = true;
            this.nameDefinition.ElementName = "Name";
            this.nameDefinition.IsOptional = false;
            this.nameDefinition.Location = new System.Drawing.Point(-1, 3);
            this.nameDefinition.Name = "nameDefinition";
            this.nameDefinition.Size = new System.Drawing.Size(280, 53);
            this.nameDefinition.TabIndex = 0;
            this.nameDefinition.XmlInfo = null;
            // 
            // tabPageMotion
            // 
            this.tabPageMotion.Controls.Add(this.groupBoxMotionControl);
            this.tabPageMotion.Controls.Add(this.groupBox2);
            this.tabPageMotion.Location = new System.Drawing.Point(4, 29);
            this.tabPageMotion.Name = "tabPageMotion";
            this.tabPageMotion.Size = new System.Drawing.Size(282, 231);
            this.tabPageMotion.TabIndex = 2;
            this.tabPageMotion.Text = "Motion";
            this.tabPageMotion.UseVisualStyleBackColor = true;
            // 
            // groupBoxMotionControl
            // 
            this.groupBoxMotionControl.Controls.Add(this.checkBoxRevreseMotion);
            this.groupBoxMotionControl.Location = new System.Drawing.Point(8, 109);
            this.groupBoxMotionControl.Name = "groupBoxMotionControl";
            this.groupBoxMotionControl.Size = new System.Drawing.Size(266, 100);
            this.groupBoxMotionControl.TabIndex = 1;
            this.groupBoxMotionControl.TabStop = false;
            this.groupBoxMotionControl.Text = "Motion Control";
            // 
            // checkBoxRevreseMotion
            // 
            this.checkBoxRevreseMotion.AutoSize = true;
            this.checkBoxRevreseMotion.Location = new System.Drawing.Point(6, 19);
            this.checkBoxRevreseMotion.Name = "checkBoxRevreseMotion";
            this.checkBoxRevreseMotion.Size = new System.Drawing.Size(198, 24);
            this.checkBoxRevreseMotion.TabIndex = 0;
            this.checkBoxRevreseMotion.Text = "Reverse motion control";
            this.checkBoxRevreseMotion.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxReverseDirection);
            this.groupBox2.Controls.Add(this.radioButtonTwoRelays);
            this.groupBox2.Controls.Add(this.radioButtonSingleRelay);
            this.groupBox2.Location = new System.Drawing.Point(8, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(266, 100);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Direction Control";
            // 
            // checkBoxReverseDirection
            // 
            this.checkBoxReverseDirection.AutoSize = true;
            this.checkBoxReverseDirection.Location = new System.Drawing.Point(6, 77);
            this.checkBoxReverseDirection.Name = "checkBoxReverseDirection";
            this.checkBoxReverseDirection.Size = new System.Drawing.Size(285, 24);
            this.checkBoxReverseDirection.TabIndex = 6;
            this.checkBoxReverseDirection.Text = "Reverse open/close command logic";
            this.checkBoxReverseDirection.UseVisualStyleBackColor = true;
            // 
            // radioButtonTwoRelays
            // 
            this.radioButtonTwoRelays.AutoSize = true;
            this.radioButtonTwoRelays.Location = new System.Drawing.Point(6, 42);
            this.radioButtonTwoRelays.Name = "radioButtonTwoRelays";
            this.radioButtonTwoRelays.Size = new System.Drawing.Size(108, 24);
            this.radioButtonTwoRelays.TabIndex = 1;
            this.radioButtonTwoRelays.TabStop = true;
            this.radioButtonTwoRelays.Text = "Two relays";
            this.radioButtonTwoRelays.UseVisualStyleBackColor = true;
            this.radioButtonTwoRelays.CheckedChanged += this.DirectionControlChanged;
            // 
            // radioButtonSingleRelay
            // 
            this.radioButtonSingleRelay.AutoSize = true;
            this.radioButtonSingleRelay.Location = new System.Drawing.Point(6, 19);
            this.radioButtonSingleRelay.Name = "radioButtonSingleRelay";
            this.radioButtonSingleRelay.Size = new System.Drawing.Size(115, 24);
            this.radioButtonSingleRelay.TabIndex = 0;
            this.radioButtonSingleRelay.TabStop = true;
            this.radioButtonSingleRelay.Text = "Single relay";
            this.radioButtonSingleRelay.UseVisualStyleBackColor = true;
            this.radioButtonSingleRelay.CheckedChanged += this.DirectionControlChanged;
            // 
            // tabPageFeedback
            // 
            this.tabPageFeedback.Controls.Add(this.panelGateMotionTime);
            this.tabPageFeedback.Controls.Add(this.radioButtonNoFeedback);
            this.tabPageFeedback.Controls.Add(this.radioButtonOneSensor);
            this.tabPageFeedback.Controls.Add(this.radioButtonTwoSensors);
            this.tabPageFeedback.Controls.Add(this.label3);
            this.tabPageFeedback.Controls.Add(this.textBoxOpenCloseTimeout);
            this.tabPageFeedback.Controls.Add(this.label2);
            this.tabPageFeedback.Location = new System.Drawing.Point(4, 29);
            this.tabPageFeedback.Name = "tabPageFeedback";
            this.tabPageFeedback.Size = new System.Drawing.Size(282, 231);
            this.tabPageFeedback.TabIndex = 1;
            this.tabPageFeedback.Text = "Feedback";
            this.tabPageFeedback.UseVisualStyleBackColor = true;
            // 
            // panelGateMotionTime
            // 
            this.panelGateMotionTime.Controls.Add(this.label4);
            this.panelGateMotionTime.Controls.Add(this.textBoxGateMotionTime);
            this.panelGateMotionTime.Controls.Add(this.label1);
            this.panelGateMotionTime.Location = new System.Drawing.Point(1, 30);
            this.panelGateMotionTime.Name = "panelGateMotionTime";
            this.panelGateMotionTime.Size = new System.Drawing.Size(271, 26);
            this.panelGateMotionTime.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(202, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 20);
            this.label4.TabIndex = 12;
            this.label4.Text = "seconds";
            // 
            // textBoxGateMotionTime
            // 
            this.textBoxGateMotionTime.Location = new System.Drawing.Point(141, 2);
            this.textBoxGateMotionTime.Name = "textBoxGateMotionTime";
            this.textBoxGateMotionTime.Size = new System.Drawing.Size(58, 26);
            this.textBoxGateMotionTime.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(164, 20);
            this.label1.TabIndex = 10;
            this.label1.Text = "Gate open/close time:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // radioButtonNoFeedback
            // 
            this.radioButtonNoFeedback.AutoSize = true;
            this.radioButtonNoFeedback.Location = new System.Drawing.Point(11, 12);
            this.radioButtonNoFeedback.Name = "radioButtonNoFeedback";
            this.radioButtonNoFeedback.Size = new System.Drawing.Size(124, 24);
            this.radioButtonNoFeedback.TabIndex = 7;
            this.radioButtonNoFeedback.TabStop = true;
            this.radioButtonNoFeedback.Text = "No feedback";
            this.radioButtonNoFeedback.UseVisualStyleBackColor = true;
            this.radioButtonNoFeedback.CheckedChanged += this.RadioButtonFeedback_CheckedChanged;
            // 
            // radioButtonOneSensor
            // 
            this.radioButtonOneSensor.AutoSize = true;
            this.radioButtonOneSensor.Location = new System.Drawing.Point(11, 82);
            this.radioButtonOneSensor.Name = "radioButtonOneSensor";
            this.radioButtonOneSensor.Size = new System.Drawing.Size(254, 24);
            this.radioButtonOneSensor.TabIndex = 6;
            this.radioButtonOneSensor.TabStop = true;
            this.radioButtonOneSensor.Text = "One sensor (gate motion done)";
            this.radioButtonOneSensor.UseVisualStyleBackColor = true;
            this.radioButtonOneSensor.CheckedChanged += this.RadioButtonFeedback_CheckedChanged;
            // 
            // radioButtonTwoSensors
            // 
            this.radioButtonTwoSensors.AutoSize = true;
            this.radioButtonTwoSensors.Location = new System.Drawing.Point(11, 59);
            this.radioButtonTwoSensors.Name = "radioButtonTwoSensors";
            this.radioButtonTwoSensors.Size = new System.Drawing.Size(274, 24);
            this.radioButtonTwoSensors.TabIndex = 5;
            this.radioButtonTwoSensors.TabStop = true;
            this.radioButtonTwoSensors.Text = "Separate Opened/Closed sensors";
            this.radioButtonTwoSensors.UseVisualStyleBackColor = true;
            this.radioButtonTwoSensors.CheckedChanged += this.RadioButtonFeedback_CheckedChanged;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(197, 168);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "seconds";
            // 
            // textBoxOpenCloseTimeout
            // 
            this.textBoxOpenCloseTimeout.Location = new System.Drawing.Point(136, 165);
            this.textBoxOpenCloseTimeout.Name = "textBoxOpenCloseTimeout";
            this.textBoxOpenCloseTimeout.Size = new System.Drawing.Size(58, 26);
            this.textBoxOpenCloseTimeout.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 168);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(191, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Gate open/close timeout: ";
            // 
            // GateProperties
            // 
            this.ClientSize = new System.Drawing.Size(290, 306);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "GateProperties";
            this.ShowInTaskbar = false;
            this.Text = "Gate Properties";
            this.tabControl.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPageMotion.ResumeLayout(false);
            this.groupBoxMotionControl.ResumeLayout(false);
            this.groupBoxMotionControl.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPageFeedback.ResumeLayout(false);
            this.tabPageFeedback.PerformLayout();
            this.panelGateMotionTime.ResumeLayout(false);
            this.panelGateMotionTime.PerformLayout();
            this.ResumeLayout(false);
        }
        #endregion
        private TabPage tabPageGeneral;
        private TabControl tabControl;
        private Button buttonOK;
        private Button buttonCancel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private LayoutManager.CommonUI.Controls.NameDefinition nameDefinition;
        private GroupBox groupBox1;
        private Panel panelGatePreview;
        private RadioButton radioButtonOpenUp;
        private RadioButton radioButtonOpenDown;
        private RadioButton radioButtonOpenRight;
        private RadioButton radioButtonOpenLeft;
        private readonly LayoutGateComponent component;
        private TabPage tabPageFeedback;
        private Label label3;
        private TextBox textBoxOpenCloseTimeout;
        private Label label2;
        private TabPage tabPageMotion;
        private GroupBox groupBox2;
        private CheckBox checkBoxReverseDirection;
        private RadioButton radioButtonTwoRelays;
        private RadioButton radioButtonSingleRelay;
        private GroupBox groupBoxMotionControl;
        private CheckBox checkBoxRevreseMotion;
        private RadioButton radioButtonOneSensor;
        private RadioButton radioButtonTwoSensors;
        private Panel panelGateMotionTime;
        private Label label4;
        private TextBox textBoxGateMotionTime;
        private Label label1;
        private RadioButton radioButtonNoFeedback;
    }
}

