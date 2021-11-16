using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Intellibox.Dialogs {
    partial class CentralStationProperties : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.columnHeaderSOnumber = new ColumnHeader();
            this.columnHeaderSOvalue = new ColumnHeader();
            this.columnHeaderSOdescription = new ColumnHeader();
            this.comboBoxPort = new ComboBox();
            this.label1 = new Label();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.buttonSettings = new Button();
            this.tabControl1 = new TabControl();
            this.tabPageGeneral = new TabPage();
            this.nameDefinition = new LayoutManager.CommonUI.Controls.NameDefinition();
            this.layoutEmulationSetup = new LayoutManager.CommonUI.Controls.LayoutEmulationSetup();
            this.tabPageAdvanced = new TabPage();
            this.groupBox1 = new GroupBox();
            this.listViewSO = new ListView();
            this.buttonSOdelete = new Button();
            this.buttonSOedit = new Button();
            this.buttonSOadd = new Button();
            this.label5 = new Label();
            this.label3 = new Label();
            this.textBoxSwitchingTime = new TextBox();
            this.label4 = new Label();
            this.textBoxPollingPeriod = new TextBox();
            this.label2 = new Label();
            this.groupBox2 = new GroupBox();
            this.label6 = new Label();
            this.textBoxOperationModeDebounceCount = new TextBox();
            this.label7 = new Label();
            this.textBoxDesignTimeDebounceCount = new TextBox();
            this.tabControl1.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.tabPageAdvanced.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // columnHeaderSOnumber
            // 
            this.columnHeaderSOnumber.Text = "#";
            // 
            // columnHeaderSOvalue
            // 
            this.columnHeaderSOvalue.Text = "Value";
            // 
            // columnHeaderSOdescription
            // 
            this.columnHeaderSOdescription.Text = "Description";
            this.columnHeaderSOdescription.Width = 188;
            // 
            // comboBoxPort
            // 
            this.comboBoxPort.FormattingEnabled = true;
            this.comboBoxPort.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9"});
            this.comboBoxPort.Location = new System.Drawing.Point(70, 80);
            this.comboBoxPort.Name = "comboBoxPort";
            this.comboBoxPort.Size = new System.Drawing.Size(104, 21);
            this.comboBoxPort.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(14, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Port:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(213, 316);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 8;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += this.ButtonOK_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(294, 316);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            // 
            // buttonSettings
            // 
            this.buttonSettings.Location = new System.Drawing.Point(182, 80);
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.Size = new System.Drawing.Size(75, 21);
            this.buttonSettings.TabIndex = 3;
            this.buttonSettings.Text = "Settings...";
            this.buttonSettings.Click += this.ButtonSettings_Click;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageGeneral);
            this.tabControl1.Controls.Add(this.tabPageAdvanced);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(358, 298);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.nameDefinition);
            this.tabPageGeneral.Controls.Add(this.comboBoxPort);
            this.tabPageGeneral.Controls.Add(this.label1);
            this.tabPageGeneral.Controls.Add(this.buttonSettings);
            this.tabPageGeneral.Controls.Add(this.layoutEmulationSetup);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGeneral.Size = new System.Drawing.Size(350, 272);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // nameDefinition
            // 
            this.nameDefinition.Component = null;
            this.nameDefinition.DefaultIsVisible = true;
            this.nameDefinition.ElementName = "Name";
            this.nameDefinition.IsOptional = false;
            this.nameDefinition.Location = new System.Drawing.Point(6, 2);
            this.nameDefinition.Name = "nameDefinition";
            this.nameDefinition.Size = new System.Drawing.Size(334, 64);
            this.nameDefinition.TabIndex = 0;
            this.nameDefinition.XmlInfo = null;
            // 
            // layoutEmulationSetup
            // 
            this.layoutEmulationSetup.Element = null;
            this.layoutEmulationSetup.Location = new System.Drawing.Point(2, 170);
            this.layoutEmulationSetup.Name = "layoutEmulationSetup";
            this.layoutEmulationSetup.Size = new System.Drawing.Size(338, 89);
            this.layoutEmulationSetup.TabIndex = 7;
            // 
            // tabPageAdvanced
            // 
            this.tabPageAdvanced.Controls.Add(this.groupBox2);
            this.tabPageAdvanced.Controls.Add(this.groupBox1);
            this.tabPageAdvanced.Controls.Add(this.label5);
            this.tabPageAdvanced.Controls.Add(this.label3);
            this.tabPageAdvanced.Controls.Add(this.textBoxSwitchingTime);
            this.tabPageAdvanced.Controls.Add(this.label4);
            this.tabPageAdvanced.Controls.Add(this.textBoxPollingPeriod);
            this.tabPageAdvanced.Controls.Add(this.label2);
            this.tabPageAdvanced.Location = new System.Drawing.Point(4, 22);
            this.tabPageAdvanced.Name = "tabPageAdvanced";
            this.tabPageAdvanced.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAdvanced.Size = new System.Drawing.Size(350, 272);
            this.tabPageAdvanced.TabIndex = 1;
            this.tabPageAdvanced.Text = "Advanced";
            this.tabPageAdvanced.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listViewSO);
            this.groupBox1.Controls.Add(this.buttonSOdelete);
            this.groupBox1.Controls.Add(this.buttonSOedit);
            this.groupBox1.Controls.Add(this.buttonSOadd);
            this.groupBox1.Location = new System.Drawing.Point(3, 110);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(341, 156);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Special Functions (SO):";
            // 
            // listViewSO
            // 
            this.listViewSO.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                        | System.Windows.Forms.AnchorStyles.Left
                        | System.Windows.Forms.AnchorStyles.Right);
            this.listViewSO.Columns.AddRange(new ColumnHeader[] {
            this.columnHeaderSOnumber,
            this.columnHeaderSOvalue,
            this.columnHeaderSOdescription});
            this.listViewSO.FullRowSelect = true;
            this.listViewSO.GridLines = true;
            this.listViewSO.Location = new System.Drawing.Point(6, 19);
            this.listViewSO.Name = "listViewSO";
            this.listViewSO.Size = new System.Drawing.Size(322, 102);
            this.listViewSO.TabIndex = 3;
            this.listViewSO.UseCompatibleStateImageBehavior = false;
            this.listViewSO.View = System.Windows.Forms.View.Details;
            this.listViewSO.DoubleClick += this.ListViewSO_DoubleClick;
            this.listViewSO.SelectedIndexChanged += this.ListViewSO_SelectedIndexChanged;
            // 
            // buttonSOdelete
            // 
            this.buttonSOdelete.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonSOdelete.Location = new System.Drawing.Point(169, 127);
            this.buttonSOdelete.Name = "buttonSOdelete";
            this.buttonSOdelete.Size = new System.Drawing.Size(75, 23);
            this.buttonSOdelete.TabIndex = 2;
            this.buttonSOdelete.Text = "&Delete";
            this.buttonSOdelete.UseVisualStyleBackColor = true;
            this.buttonSOdelete.Click += this.ButtonSOdelete_Click;
            // 
            // buttonSOedit
            // 
            this.buttonSOedit.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonSOedit.Location = new System.Drawing.Point(88, 127);
            this.buttonSOedit.Name = "buttonSOedit";
            this.buttonSOedit.Size = new System.Drawing.Size(75, 23);
            this.buttonSOedit.TabIndex = 1;
            this.buttonSOedit.Text = "&Edit...";
            this.buttonSOedit.UseVisualStyleBackColor = true;
            this.buttonSOedit.Click += this.ButtonSOedit_Click;
            // 
            // buttonSOadd
            // 
            this.buttonSOadd.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonSOadd.Location = new System.Drawing.Point(7, 127);
            this.buttonSOadd.Name = "buttonSOadd";
            this.buttonSOadd.Size = new System.Drawing.Size(75, 23);
            this.buttonSOadd.TabIndex = 0;
            this.buttonSOadd.Text = "&Add...";
            this.buttonSOadd.UseVisualStyleBackColor = true;
            this.buttonSOadd.Click += this.ButtonSOadd_Click;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(267, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Milliseconds";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(267, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Milliseconds";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxSwitchingTime
            // 
            this.textBoxSwitchingTime.Location = new System.Drawing.Point(221, 29);
            this.textBoxSwitchingTime.Name = "textBoxSwitchingTime";
            this.textBoxSwitchingTime.Size = new System.Drawing.Size(40, 20);
            this.textBoxSwitchingTime.TabIndex = 5;
            this.textBoxSwitchingTime.Text = "50";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(191, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Accessory (e.g. turnout) switching time:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxPollingPeriod
            // 
            this.textBoxPollingPeriod.Location = new System.Drawing.Point(221, 6);
            this.textBoxPollingPeriod.Name = "textBoxPollingPeriod";
            this.textBoxPollingPeriod.Size = new System.Drawing.Size(40, 20);
            this.textBoxPollingPeriod.TabIndex = 1;
            this.textBoxPollingPeriod.Text = "50";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(185, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Check command station status each: ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxDesignTimeDebounceCount);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.textBoxOperationModeDebounceCount);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(3, 53);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(341, 51);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Feedback Debounce:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Operation mode:";
            // 
            // textBoxOperationModeDebounceCount
            // 
            this.textBoxOperationModeDebounceCount.Location = new System.Drawing.Point(94, 21);
            this.textBoxOperationModeDebounceCount.Name = "textBoxOperationModeDebounceCount";
            this.textBoxOperationModeDebounceCount.Size = new System.Drawing.Size(40, 20);
            this.textBoxOperationModeDebounceCount.TabIndex = 2;
            this.textBoxOperationModeDebounceCount.Text = "1";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(151, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Design time:";
            // 
            // textBoxDesignTimeDebounceCount
            // 
            this.textBoxDesignTimeDebounceCount.Location = new System.Drawing.Point(222, 21);
            this.textBoxDesignTimeDebounceCount.Name = "textBoxDesignTimeDebounceCount";
            this.textBoxDesignTimeDebounceCount.Size = new System.Drawing.Size(40, 20);
            this.textBoxDesignTimeDebounceCount.TabIndex = 4;
            this.textBoxDesignTimeDebounceCount.Text = "1";
            // 
            // CentralStationProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 346);
            this.ControlBox = false;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "CentralStationProperties";
            this.Text = "IntelliBox Properties";
            this.tabControl1.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageAdvanced.ResumeLayout(false);
            this.tabPageAdvanced.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
        }
        #endregion

        private LayoutManager.CommonUI.Controls.NameDefinition nameDefinition;
        private ComboBox comboBoxPort;
        private Label label1;
        private Button buttonOK;
        private Button buttonCancel;
        private readonly System.ComponentModel.IContainer components = null;

        private Button buttonSettings;
        private LayoutManager.CommonUI.Controls.LayoutEmulationSetup layoutEmulationSetup;
        private TabControl tabControl1;
        private TabPage tabPageGeneral;
        private TabPage tabPageAdvanced;
        private GroupBox groupBox1;
        private Button buttonSOdelete;
        private Button buttonSOedit;
        private Button buttonSOadd;
        private Label label5;
        private Label label3;
        private TextBox textBoxSwitchingTime;
        private Label label4;
        private TextBox textBoxPollingPeriod;
        private Label label2;
        private ListView listViewSO;
        private ColumnHeader columnHeaderSOnumber;
        private ColumnHeader columnHeaderSOvalue;
        private ColumnHeader columnHeaderSOdescription;
        private GroupBox groupBox2;
        private Label label7;
        private TextBox textBoxOperationModeDebounceCount;
        private Label label6;
        private TextBox textBoxDesignTimeDebounceCount;
    }
}
