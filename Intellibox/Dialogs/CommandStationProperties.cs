using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager;
using LayoutManager.Model;

namespace Intellibox.Dialogs
{
	/// <summary>
	/// Summary description for CentralStationProperties.
	/// </summary>
	public class CentralStationProperties : Form {
		private LayoutManager.CommonUI.Controls.NameDefinition nameDefinition;
		private ComboBox comboBoxPort;
		private Label label1;
		private Button buttonOK;
		private Button buttonCancel;
		private IContainer components = null;

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

		IntelliboxComponent _component;
		LayoutXmlInfo _xmlInfo;
		private GroupBox groupBox2;
		private Label label7;
		private TextBox textBoxOperationModeDebounceCount;
		private Label label6;
		private TextBox textBoxDesignTimeDebounceCount;
		SOcollection _SOcollection;

		public CentralStationProperties(IntelliboxComponent component)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this._component = component;
			this._xmlInfo = new LayoutXmlInfo(component);
			this._SOcollection = new SOcollection(_xmlInfo.Element);

            IntelliboxComponentInfo Info = new IntelliboxComponentInfo(component, _xmlInfo.Element);

			nameDefinition.XmlInfo = this._xmlInfo;

			if(component.LayoutEmulationSupported)
				layoutEmulationSetup.Element = _xmlInfo.DocumentElement;
			else
				layoutEmulationSetup.Visible = false;

            comboBoxPort.Text = Info.Port;
			textBoxPollingPeriod.Text = Info.PollingPeriod.ToString();
            textBoxSwitchingTime.Text = Info.AccessoryCommandTime.ToString();
			textBoxOperationModeDebounceCount.Text = Info.OperationModeDebounceCount.ToString();
			textBoxDesignTimeDebounceCount.Text = Info.DesignTimeDebounceCount.ToString();

			foreach(SOinfo so in _SOcollection)
				listViewSO.Items.Add(new SOitem(so));

			UpdateButtons();
		}

		private void UpdateButtons() {
			if(listViewSO.SelectedItems.Count == 0) {
				buttonSOdelete.Enabled = false;
				buttonSOedit.Enabled = false;
			}
			else {
				buttonSOdelete.Enabled = true;
				buttonSOedit.Enabled = true;
			}
		}

		public LayoutXmlInfo XmlInfo {
			get {
				return _xmlInfo;
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
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
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
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
			this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click);
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
			this.listViewSO.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
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
			this.listViewSO.DoubleClick += new System.EventHandler(this.listViewSO_DoubleClick);
			this.listViewSO.SelectedIndexChanged += new System.EventHandler(this.listViewSO_SelectedIndexChanged);
			// 
			// buttonSOdelete
			// 
			this.buttonSOdelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonSOdelete.Location = new System.Drawing.Point(169, 127);
			this.buttonSOdelete.Name = "buttonSOdelete";
			this.buttonSOdelete.Size = new System.Drawing.Size(75, 23);
			this.buttonSOdelete.TabIndex = 2;
			this.buttonSOdelete.Text = "&Delete";
			this.buttonSOdelete.UseVisualStyleBackColor = true;
			this.buttonSOdelete.Click += new System.EventHandler(this.buttonSOdelete_Click);
			// 
			// buttonSOedit
			// 
			this.buttonSOedit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonSOedit.Location = new System.Drawing.Point(88, 127);
			this.buttonSOedit.Name = "buttonSOedit";
			this.buttonSOedit.Size = new System.Drawing.Size(75, 23);
			this.buttonSOedit.TabIndex = 1;
			this.buttonSOedit.Text = "&Edit...";
			this.buttonSOedit.UseVisualStyleBackColor = true;
			this.buttonSOedit.Click += new System.EventHandler(this.buttonSOedit_Click);
			// 
			// buttonSOadd
			// 
			this.buttonSOadd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonSOadd.Location = new System.Drawing.Point(7, 127);
			this.buttonSOadd.Name = "buttonSOadd";
			this.buttonSOadd.Size = new System.Drawing.Size(75, 23);
			this.buttonSOadd.TabIndex = 0;
			this.buttonSOadd.Text = "&Add...";
			this.buttonSOadd.UseVisualStyleBackColor = true;
			this.buttonSOadd.Click += new System.EventHandler(this.buttonSOadd_Click);
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

		private void buttonOK_Click(object sender, System.EventArgs e) {
			if(nameDefinition.Commit()) {
				LayoutTextInfo			myName = new LayoutTextInfo(_xmlInfo.DocumentElement, "Name");

				foreach(IModelComponentIsCommandStation otherCommandStation in LayoutModel.Components<IModelComponentIsCommandStation>(LayoutPhase.All)) {
					if(otherCommandStation.NameProvider.Name == myName.Name && otherCommandStation.Id != _component.Id) {
						MessageBox.Show(this, "The name " + myName.Text + " is already used", "Invalid name", MessageBoxButtons.OK, MessageBoxIcon.Error);
						nameDefinition.Focus();
						return;
					}
				}
			}
			else
				return;

			if(_component.LayoutEmulationSupported) {
				if(!layoutEmulationSetup.ValidateInput())
					return;
			}

            IntelliboxComponentInfo info = new IntelliboxComponentInfo(_component, _xmlInfo.Element);

            int pollingPeriod;

            if(!int.TryParse(textBoxPollingPeriod.Text, out pollingPeriod)) {
				MessageBox.Show(this, "Invalid number", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBoxPollingPeriod.Focus();
				return;
			}

            int switchingTime;

            if(!int.TryParse(textBoxSwitchingTime.Text, out switchingTime)) {
                MessageBox.Show(this, "Invalid number", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxSwitchingTime.Focus();
                return;
            }

			byte operationDebounceCount;

			if(!byte.TryParse(textBoxOperationModeDebounceCount.Text, out operationDebounceCount)) {
                MessageBox.Show(this, "Invalid number", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxOperationModeDebounceCount.Focus();
                return;
            }

			byte designTimeDebounceCount;

			if(!byte.TryParse(textBoxDesignTimeDebounceCount.Text, out designTimeDebounceCount)) {
				MessageBox.Show(this, "Invalid number", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBoxDesignTimeDebounceCount.Focus();
				return;
			}

			// Commit
            info.PollingPeriod = pollingPeriod;
            info.AccessoryCommandTime = switchingTime;
			info.OperationModeDebounceCount = operationDebounceCount;
			info.DesignTimeDebounceCount = designTimeDebounceCount;
            info.Port = comboBoxPort.Text;

            if(_component.LayoutEmulationSupported)
				layoutEmulationSetup.Commit();

			DialogResult = DialogResult.OK;
		}

		private void buttonSettings_Click(object sender, EventArgs e)
		{
			string modeString = _xmlInfo.DocumentElement["ModeString"].InnerText;

            using (LayoutManager.CommonUI.Dialogs.SerialInterfaceParameters d = new LayoutManager.CommonUI.Dialogs.SerialInterfaceParameters(modeString)) {
                if (d.ShowDialog(this) == DialogResult.OK)
                    _xmlInfo.DocumentElement["ModeString"].InnerText = d.ModeString;
            }
		}

		#region SOitem

		class SOitem : ListViewItem {
			SOinfo _so;

			public SOitem(SOinfo so) {
				_so = so;
				SubItems.Add("");
				SubItems.Add("");
				Update();
			}

			public void Update() {
				SubItems[0].Text = _so.Number.ToString();
				SubItems[1].Text = _so.Value.ToString();
				SubItems[2].Text = _so.Description;
			}

			public SOinfo SOinfo {
				get {
					return _so;
				}
			}
		}

		#endregion

		private void buttonSOadd_Click(object sender, EventArgs e) {
			SOinfo so = new SOinfo();

            using (Dialogs.SOdefinition d = new SOdefinition(so)) {
                if (d.ShowDialog(this) == DialogResult.OK)
                    listViewSO.Items.Add(new SOitem(_SOcollection.Add(so)));
                UpdateButtons();
            }
		}

		private void buttonSOedit_Click(object sender, EventArgs e) {
			if(listViewSO.SelectedItems.Count > 0) {
				SOitem item = (SOitem)listViewSO.SelectedItems[0];

				// Make a temporary copy
				XmlDocument doc = LayoutXmlInfo.XmlImplementation.CreateDocument();
				doc.AppendChild(doc.ImportNode(item.SOinfo.Element, true));

				SOinfo tempSOinfo = new SOinfo(doc.DocumentElement);

                using (var d = new Dialogs.SOdefinition(tempSOinfo)) {
                    if (d.ShowDialog(this) == DialogResult.OK) {
                        XmlElement oldElement = item.SOinfo.Element;

                        item.SOinfo.Element = (XmlElement)oldElement.OwnerDocument.ImportNode(tempSOinfo.Element, true);
                        oldElement.ParentNode.ReplaceChild(item.SOinfo.Element, oldElement);
                        item.Update();
                    }
                }
			}
			UpdateButtons();
		}

		private void buttonSOdelete_Click(object sender, EventArgs e) {
			SOitem[] deletedItems = new SOitem[listViewSO.SelectedItems.Count];

			listViewSO.SelectedItems.CopyTo(deletedItems, 0);
			foreach(SOitem item in deletedItems) {
				_SOcollection.Remove(item.SOinfo);
				listViewSO.Items.Remove(item);
			}

			UpdateButtons();
		}

		private void listViewSO_DoubleClick(object sender, EventArgs e) {
			if(listViewSO.SelectedItems.Count > 0)
				buttonSOedit.PerformClick();
		}

		private void listViewSO_SelectedIndexChanged(object sender, EventArgs e) {
			UpdateButtons();
		}

	}
}
