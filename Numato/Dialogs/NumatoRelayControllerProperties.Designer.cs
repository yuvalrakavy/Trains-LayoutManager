using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.IO.Ports;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

#pragma warning disable IDE0069
namespace NumatoController.Dialogs {
    /// <summary>
    /// Summary description for CentralStationProperties.
    /// </summary>
    partial class NumatoControllerProperties : Form {
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
            this.nameDefinition = new LayoutManager.CommonUI.Controls.NameDefinition();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonIpSettings = new System.Windows.Forms.Button();
            this.textBoxAddress = new System.Windows.Forms.TextBox();
            this.radioButtonTCP = new System.Windows.Forms.RadioButton();
            this.radioButtonSerial = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxUser = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labelRelaysCount = new System.Windows.Forms.Label();
            this.comboBoxRelays = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxPort
            // 
            this.comboBoxPort.FormattingEnabled = true;
            this.comboBoxPort.Location = new System.Drawing.Point(75, 45);
            this.comboBoxPort.Name = "comboBoxPort";
            this.comboBoxPort.Size = new System.Drawing.Size(169, 21);
            this.comboBoxPort.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Port:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonOK
            // 
            this.buttonOK.AutoSize = true;
            this.buttonOK.Location = new System.Drawing.Point(213, 333);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(76, 28);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += this.ButtonOK_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(295, 333);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(76, 28);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            // 
            // buttonCOMsettings
            // 
            this.buttonCOMsettings.Location = new System.Drawing.Point(250, 45);
            this.buttonCOMsettings.Name = "buttonCOMsettings";
            this.buttonCOMsettings.Size = new System.Drawing.Size(75, 21);
            this.buttonCOMsettings.TabIndex = 9;
            this.buttonCOMsettings.Text = "Settings...";
            this.buttonCOMsettings.Click += this.ButtonCOMsettings_Click;
            // 
            // nameDefinition
            // 
            this.nameDefinition.Component = null;
            this.nameDefinition.DefaultIsVisible = true;
            this.nameDefinition.ElementName = "Name";
            this.nameDefinition.IsOptional = false;
            this.nameDefinition.Location = new System.Drawing.Point(8, 8);
            this.nameDefinition.Name = "nameDefinition";
            this.nameDefinition.Size = new System.Drawing.Size(312, 66);
            this.nameDefinition.TabIndex = 0;
            this.nameDefinition.XmlInfo = null;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonIpSettings);
            this.groupBox1.Controls.Add(this.textBoxAddress);
            this.groupBox1.Controls.Add(this.radioButtonTCP);
            this.groupBox1.Controls.Add(this.radioButtonSerial);
            this.groupBox1.Controls.Add(this.buttonCOMsettings);
            this.groupBox1.Controls.Add(this.comboBoxPort);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 110);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(359, 132);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Interface:";
            // 
            // buttonIpSettings
            // 
            this.buttonIpSettings.Location = new System.Drawing.Point(250, 95);
            this.buttonIpSettings.Name = "buttonIpSettings";
            this.buttonIpSettings.Size = new System.Drawing.Size(75, 21);
            this.buttonIpSettings.TabIndex = 13;
            this.buttonIpSettings.Text = "Settings...";
            this.buttonIpSettings.Click += this.ButtonIpSettings_Click;
            // 
            // textBoxAddress
            // 
            this.textBoxAddress.Location = new System.Drawing.Point(75, 95);
            this.textBoxAddress.Name = "textBoxAddress";
            this.textBoxAddress.Size = new System.Drawing.Size(169, 20);
            this.textBoxAddress.TabIndex = 11;
            // 
            // radioButtonTCP
            // 
            this.radioButtonTCP.AutoSize = true;
            this.radioButtonTCP.Location = new System.Drawing.Point(16, 78);
            this.radioButtonTCP.Name = "radioButtonTCP";
            this.radioButtonTCP.Size = new System.Drawing.Size(61, 17);
            this.radioButtonTCP.TabIndex = 10;
            this.radioButtonTCP.TabStop = true;
            this.radioButtonTCP.Text = "TCP/IP";
            this.radioButtonTCP.UseVisualStyleBackColor = true;
            this.radioButtonTCP.CheckedChanged += this.RadioButtonInterfaceType_CheckedChanged;
            // 
            // radioButtonSerial
            // 
            this.radioButtonSerial.AutoSize = true;
            this.radioButtonSerial.Location = new System.Drawing.Point(16, 19);
            this.radioButtonSerial.Name = "radioButtonSerial";
            this.radioButtonSerial.Size = new System.Drawing.Size(239, 17);
            this.radioButtonSerial.TabIndex = 0;
            this.radioButtonSerial.TabStop = true;
            this.radioButtonSerial.Text = "Serial (RS232, USB, Network using RealPort)";
            this.radioButtonSerial.UseVisualStyleBackColor = true;
            this.radioButtonSerial.CheckedChanged += this.RadioButtonInterfaceType_CheckedChanged;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Address:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxPassword);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textBoxUser);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(12, 248);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(359, 79);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Login:";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(68, 44);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(100, 20);
            this.textBoxPassword.TabIndex = 3;
            this.textBoxPassword.Text = "admin";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Password:";
            // 
            // textBoxUser
            // 
            this.textBoxUser.Location = new System.Drawing.Point(68, 18);
            this.textBoxUser.Name = "textBoxUser";
            this.textBoxUser.Size = new System.Drawing.Size(100, 20);
            this.textBoxUser.TabIndex = 1;
            this.textBoxUser.Text = "admin";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "User:";
            // 
            // labelRelaysCount
            // 
            this.labelRelaysCount.AutoSize = true;
            this.labelRelaysCount.Location = new System.Drawing.Point(12, 77);
            this.labelRelaysCount.Name = "labelRelaysCount";
            this.labelRelaysCount.Size = new System.Drawing.Size(89, 13);
            this.labelRelaysCount.TabIndex = 14;
            this.labelRelaysCount.Text = "Number of relays:";
            // 
            // comboBoxRelays
            // 
            this.comboBoxRelays.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRelays.FormattingEnabled = true;
            this.comboBoxRelays.Items.AddRange(new object[] {
            "1",
            "2",
            "4",
            "8",
            "16",
            "32"});
            this.comboBoxRelays.Location = new System.Drawing.Point(107, 74);
            this.comboBoxRelays.Name = "comboBoxRelays";
            this.comboBoxRelays.Size = new System.Drawing.Size(121, 21);
            this.comboBoxRelays.TabIndex = 15;
            // 
            // NumatoControllerProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AcceptButton = this.buttonOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(385, 373);
            this.ControlBox = false;
            this.Controls.Add(this.comboBoxRelays);
            this.Controls.Add(this.labelRelaysCount);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.nameDefinition);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "NumatoControllerProperties";
            this.Text = "Numato Lab Relay Module Properties";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        private ComboBox comboBoxPort;
        private Label label1;
        private Button buttonOK;
        private Button buttonCancel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
        private LayoutManager.CommonUI.Controls.NameDefinition nameDefinition;
        private Button buttonCOMsettings;
        private GroupBox groupBox1;
        private TextBox textBoxAddress;
        private RadioButton radioButtonTCP;
        private RadioButton radioButtonSerial;
        private Label label3;
        private Button buttonIpSettings;
        private GroupBox groupBox2;
        private TextBox textBoxUser;
        private Label label2;
        private TextBox textBoxPassword;
        private Label label4;
        private Label labelRelaysCount;
        private ComboBox comboBoxRelays;
    }
}