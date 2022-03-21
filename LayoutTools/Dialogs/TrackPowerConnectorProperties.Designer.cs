using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.Model;
using LayoutManager.Components;
using System;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TrackPowerConnectorProperties.
    /// </summary>
    partial class TrackPowerConnectorProperties : Form, ILayoutComponentPropertiesDialog {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new Label();
            this.comboBoxPowerSources = new ComboBox();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.checkBoxDisplayPowerSourceName = new CheckBox();
            this.buttonSettings = new Button();
            this.trackGaugeSelector = new LayoutManager.CommonUI.Controls.TrackGuageSelector();
            this.label2 = new Label();
            this.checkBoxDetectReverseLoops = new CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Power source:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxPowerSources
            // 
            this.comboBoxPowerSources.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPowerSources.Location = new System.Drawing.Point(99, 16);
            this.comboBoxPowerSources.Name = "comboBoxPowerSources";
            this.comboBoxPowerSources.Size = new System.Drawing.Size(256, 21);
            this.comboBoxPowerSources.TabIndex = 1;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(192, 187);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(280, 187);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            // 
            // checkBoxDisplayPowerSourceName
            // 
            this.checkBoxDisplayPowerSourceName.Location = new System.Drawing.Point(19, 48);
            this.checkBoxDisplayPowerSourceName.Name = "checkBoxDisplayPowerSourceName";
            this.checkBoxDisplayPowerSourceName.Size = new System.Drawing.Size(168, 24);
            this.checkBoxDisplayPowerSourceName.TabIndex = 2;
            this.checkBoxDisplayPowerSourceName.Text = "Display power source name";
            this.checkBoxDisplayPowerSourceName.CheckedChanged += new EventHandler(this.CheckBoxDisplayPowerSourceName_CheckedChanged);
            // 
            // buttonSettings
            // 
            this.buttonSettings.Location = new System.Drawing.Point(179, 49);
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.Size = new System.Drawing.Size(75, 23);
            this.buttonSettings.TabIndex = 3;
            this.buttonSettings.Text = "Settings...";
            this.buttonSettings.Click += new EventHandler(this.ButtonSettings_Click);
            // 
            // trackGaugeSelector
            // 
            this.trackGaugeSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.trackGaugeSelector.FormattingEnabled = true;
            this.trackGaugeSelector.IncludeGuageSet = true;
            this.trackGaugeSelector.Location = new System.Drawing.Point(179, 87);
            this.trackGaugeSelector.Name = "trackGaugeSelector";
            this.trackGaugeSelector.Size = new System.Drawing.Size(121, 21);
            this.trackGaugeSelector.TabIndex = 6;
            this.trackGaugeSelector.Value = LayoutManager.Model.TrackGauges.HO;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(96, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Track Guage:";
            // 
            // checkBoxDetectReverseLoops
            // 
            this.checkBoxDetectReverseLoops.AutoSize = true;
            this.checkBoxDetectReverseLoops.Location = new System.Drawing.Point(19, 122);
            this.checkBoxDetectReverseLoops.Name = "checkBoxDetectReverseLoops";
            this.checkBoxDetectReverseLoops.Size = new System.Drawing.Size(124, 17);
            this.checkBoxDetectReverseLoops.TabIndex = 8;
            this.checkBoxDetectReverseLoops.Text = "Detect reverse loops";
            this.checkBoxDetectReverseLoops.UseVisualStyleBackColor = true;
            // 
            // TrackPowerConnectorProperties
            // 
            this.AcceptButton = this.buttonOK;
            this.CancelButton = this.buttonCancel;

            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(364, 222);
            this.ControlBox = false;
            this.Controls.Add(this.checkBoxDetectReverseLoops);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.trackGaugeSelector);
            this.Controls.Add(this.buttonSettings);
            this.Controls.Add(this.checkBoxDisplayPowerSourceName);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.comboBoxPowerSources);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "TrackPowerConnectorProperties";
            this.ShowInTaskbar = false;
            this.Text = "Track Power Connector";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        private Label label1;
        private ComboBox comboBoxPowerSources;
        private Button buttonOK;
        private Button buttonCancel;
        private CheckBox checkBoxDisplayPowerSourceName;
        private Button buttonSettings;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
        private CommonUI.Controls.TrackGuageSelector trackGaugeSelector;
        private Label label2;
        private CheckBox checkBoxDetectReverseLoops;
    }
}

