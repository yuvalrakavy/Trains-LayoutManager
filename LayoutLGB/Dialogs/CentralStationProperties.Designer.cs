﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager;
using LayoutManager.Components;
using LayoutManager.Model;

namespace LayoutLGB.Dialogs {
    /// <summary>
    /// Summary description for CentralStationProperties.
    /// </summary>
    partial class CentralStationProperties : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.comboBoxPort = new ComboBox();
            this.label1 = new Label();
            this.label2 = new Label();
            this.numericUpDownXbusID = new NumericUpDown();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.layoutEmulationSetup = new LayoutManager.CommonUI.Controls.LayoutEmulationSetup();
            this.nameDefinition = new LayoutManager.CommonUI.Controls.NameDefinition();
            this.buttonCOMsettings = new Button();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownXbusID).BeginInit();
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
            this.comboBoxPort.Location = new System.Drawing.Point(72, 88);
            this.comboBoxPort.Name = "comboBoxPort";
            this.comboBoxPort.Size = new System.Drawing.Size(104, 21);
            this.comboBoxPort.TabIndex = 1;
            this.comboBoxPort.Text = "COM1";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Port:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 116);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "XBUS ID:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numericUpDownXbusID
            // 
            this.numericUpDownXbusID.Location = new System.Drawing.Point(72, 116);
            this.numericUpDownXbusID.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numericUpDownXbusID.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownXbusID.Name = "numericUpDownXbusID";
            this.numericUpDownXbusID.Size = new System.Drawing.Size(48, 20);
            this.numericUpDownXbusID.TabIndex = 5;
            this.numericUpDownXbusID.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(184, 248);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(264, 248);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            // 
            // layoutEmulationSetup
            // 
            this.layoutEmulationSetup.Element = null;
            this.layoutEmulationSetup.Location = new System.Drawing.Point(4, 152);
            this.layoutEmulationSetup.Name = "layoutEmulationSetup";
            this.layoutEmulationSetup.Size = new System.Drawing.Size(338, 89);
            this.layoutEmulationSetup.TabIndex = 8;
            // 
            // nameDefinition
            // 
            this.nameDefinition.Component = null;
            this.nameDefinition.DefaultIsVisible = true;
            this.nameDefinition.ElementName = "Name";
            this.nameDefinition.IsOptional = false;
            this.nameDefinition.Location = new System.Drawing.Point(8, 8);
            this.nameDefinition.Name = "nameDefinition";
            this.nameDefinition.Size = new System.Drawing.Size(312, 64);
            this.nameDefinition.TabIndex = 0;
            this.nameDefinition.XmlInfo = null;
            // 
            // buttonCOMsettings
            // 
            this.buttonCOMsettings.Location = new System.Drawing.Point(183, 88);
            this.buttonCOMsettings.Name = "buttonCOMsettings";
            this.buttonCOMsettings.Size = new System.Drawing.Size(75, 21);
            this.buttonCOMsettings.TabIndex = 9;
            this.buttonCOMsettings.Text = "Settings...";
            this.buttonCOMsettings.Click += new EventHandler(this.ButtonCOMsettings_Click);
            // 
            // CentralStationProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(346, 280);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCOMsettings);
            this.Controls.Add(this.layoutEmulationSetup);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.numericUpDownXbusID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxPort);
            this.Controls.Add(this.nameDefinition);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "CentralStationProperties";
            this.Text = "LGB Central Station Properties";
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownXbusID).EndInit();
            this.ResumeLayout(false);
        }
        #endregion

        private const string A_XbusId = "XbusID";
        private ComboBox comboBoxPort;
        private Label label1;
        private Label label2;
        private NumericUpDown numericUpDownXbusID;
        private Button buttonOK;
        private Button buttonCancel;
        private LayoutManager.CommonUI.Controls.LayoutEmulationSetup layoutEmulationSetup;
        private LayoutManager.CommonUI.Controls.NameDefinition nameDefinition;
        private Button buttonCOMsettings;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;
    }
}

