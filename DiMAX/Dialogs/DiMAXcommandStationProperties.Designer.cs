using System;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager;
using LayoutManager.Model;

namespace DiMAX.Dialogs {
    /// <summary>
    /// Summary description for CentralStationProperties.
    /// </summary>
    partial class DiMAXcommandlStationProperties : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.comboBoxPort = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonCOMsettings = new System.Windows.Forms.Button();
            this.layoutEmulationSetup = new LayoutManager.CommonUI.Controls.LayoutEmulationSetup();
            this.nameDefinition = new LayoutManager.CommonUI.Controls.NameDefinition();
            this.SuspendLayout();
            // 
            // comboBoxPort
            // 
            this.comboBoxPort.FormattingEnabled = true;
            this.comboBoxPort.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4"});
            this.comboBoxPort.Location = new System.Drawing.Point(187, 217);
            this.comboBoxPort.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.comboBoxPort.Name = "comboBoxPort";
            this.comboBoxPort.Size = new System.Drawing.Size(264, 40);
            this.comboBoxPort.TabIndex = 1;
            this.comboBoxPort.Text = "COM1";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(42, 217);
            this.label1.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 39);
            this.label1.TabIndex = 2;
            this.label1.Text = "Port:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(478, 610);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(195, 57);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(686, 610);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(195, 57);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            // 
            // buttonCOMsettings
            // 
            this.buttonCOMsettings.Location = new System.Drawing.Point(476, 217);
            this.buttonCOMsettings.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonCOMsettings.Name = "buttonCOMsettings";
            this.buttonCOMsettings.Size = new System.Drawing.Size(195, 52);
            this.buttonCOMsettings.TabIndex = 9;
            this.buttonCOMsettings.Text = "Settings...";
            this.buttonCOMsettings.Click += new System.EventHandler(this.ButtonCOMsettings_Click);
            // 
            // layoutEmulationSetup
            // 
            this.layoutEmulationSetup.Element = null;
            this.layoutEmulationSetup.Location = new System.Drawing.Point(10, 374);
            this.layoutEmulationSetup.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.layoutEmulationSetup.Name = "layoutEmulationSetup";
            this.layoutEmulationSetup.Size = new System.Drawing.Size(879, 219);
            this.layoutEmulationSetup.TabIndex = 8;
            // 
            // nameDefinition
            // 
            this.nameDefinition.Component = null;
            this.nameDefinition.DefaultIsVisible = true;
            this.nameDefinition.ElementName = "Name";
            this.nameDefinition.IsOptional = false;
            this.nameDefinition.Location = new System.Drawing.Point(21, 20);
            this.nameDefinition.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.nameDefinition.Name = "nameDefinition";
            this.nameDefinition.Size = new System.Drawing.Size(811, 158);
            this.nameDefinition.TabIndex = 0;
            // 
            // DiMAXcommandlStationProperties
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(900, 689);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCOMsettings);
            this.Controls.Add(this.layoutEmulationSetup);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxPort);
            this.Controls.Add(this.nameDefinition);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "DiMAXcommandlStationProperties";
            this.Text = "Massoth DiMAX Command Station Properties";
            this.ResumeLayout(false);

        }
        #endregion

        private ComboBox comboBoxPort;
        private Label label1;
        private Button buttonOK;
        private Button buttonCancel;
        private Button buttonCOMsettings;
        private LayoutManager.CommonUI.Controls.LayoutEmulationSetup layoutEmulationSetup;
        private LayoutManager.CommonUI.Controls.NameDefinition nameDefinition;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}

