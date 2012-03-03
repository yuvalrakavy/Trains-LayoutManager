using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.Model;
using LayoutManager.Components;
using LayoutManager.CommonUI;

namespace LayoutManager.Tools.Dialogs
{
	/// <summary>
	/// Summary description for TrackPowerConnectorProperties.
	/// </summary>
	public class TrackPowerConnectorProperties : System.Windows.Forms.Form, ILayoutComponentPropertiesDialog
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxPowerSources;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.CheckBox checkBoxDisplayPowerSourceName;
		private System.Windows.Forms.Button buttonSettings;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		LayoutXmlInfo xmlInfo;
		private CommonUI.Controls.TrackGuageSelector trackGaugeSelector;
		private Label label2;
		private CheckBox checkBoxDetectReverseLoops;
		LayoutTrackPowerConnectorComponent	component;

		public TrackPowerConnectorProperties(ModelComponent modelComponent)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.component = (LayoutTrackPowerConnectorComponent)modelComponent;
			xmlInfo = new LayoutXmlInfo(component);

			var info = new LayoutTrackPowerConnectorInfo(xmlInfo.Element);

			if(info.Element == null)
				info.Element = LayoutInfo.CreateProviderElement(xmlInfo.Element, "TrackPowerConnector", null);

			foreach(IModelComponentHasPowerOutlets componentWithPowerSources in LayoutModel.Components<IModelComponentHasPowerOutlets>(LayoutPhase.All)) {
				for(int outletIndex = 0; outletIndex < componentWithPowerSources.PowerOutlets.Count; outletIndex++)
					comboBoxPowerSources.Items.Add(new LayoutComponentPowerOutletDescription(componentWithPowerSources, outletIndex));
			}

			// Locate and select current power source (if any)
			LayoutPowerInlet inlet = info.Inlet;

			if(inlet.IsConnected) {
				foreach(LayoutComponentPowerOutletDescription powerSourceItem in comboBoxPowerSources.Items) {
					if(powerSourceItem.Component.Id == inlet.OutletComponentId && powerSourceItem.OutletIndex== inlet.OutletIndex) {
						comboBoxPowerSources.SelectedItem = powerSourceItem;
						break;
					}
				}
			}
			else if(comboBoxPowerSources.Items.Count == 1)
				comboBoxPowerSources.SelectedItem = comboBoxPowerSources.Items[0];

			checkBoxDisplayPowerSourceName.Checked = info.Visible;

			trackGaugeSelector.Init();
			trackGaugeSelector.Value = info.TrackGauge;

			checkBoxDetectReverseLoops.Checked = info.CheckReverseLoops;
			updateButtons();
		}

		private void updateButtons() {
			buttonSettings.Enabled = checkBoxDisplayPowerSourceName.Checked;
		}

		public LayoutXmlInfo XmlInfo {
			get {
				return xmlInfo;
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
			this.label1 = new System.Windows.Forms.Label();
			this.comboBoxPowerSources = new System.Windows.Forms.ComboBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.checkBoxDisplayPowerSourceName = new System.Windows.Forms.CheckBox();
			this.buttonSettings = new System.Windows.Forms.Button();
			this.trackGaugeSelector = new LayoutManager.CommonUI.Controls.TrackGuageSelector();
			this.label2 = new System.Windows.Forms.Label();
			this.checkBoxDetectReverseLoops = new System.Windows.Forms.CheckBox();
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
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
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
			this.checkBoxDisplayPowerSourceName.CheckedChanged += new System.EventHandler(this.checkBoxDisplayPowerSourceName_CheckedChanged);
			// 
			// buttonSettings
			// 
			this.buttonSettings.Location = new System.Drawing.Point(179, 49);
			this.buttonSettings.Name = "buttonSettings";
			this.buttonSettings.Size = new System.Drawing.Size(75, 23);
			this.buttonSettings.TabIndex = 3;
			this.buttonSettings.Text = "Settings...";
			this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click);
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

		private void buttonOK_Click(object sender, System.EventArgs e) {
			if(comboBoxPowerSources.SelectedItem == null) {
				MessageBox.Show(this, "You must specify power source name", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				comboBoxPowerSources.Focus();
				return;
			}

			LayoutTrackPowerConnectorInfo info = new LayoutTrackPowerConnectorInfo(xmlInfo.Element);

			LayoutPowerInlet inlet = info.Inlet;
			var powerSourceItem = (LayoutComponentPowerOutletDescription)comboBoxPowerSources.SelectedItem;

			inlet.OutletComponent = powerSourceItem.Component;
			inlet.OutletIndex = powerSourceItem.OutletIndex;

			info.Visible = checkBoxDisplayPowerSourceName.Checked;
			info.TrackGauge = trackGaugeSelector.Value;
			info.CheckReverseLoops = checkBoxDetectReverseLoops.Checked;

			DialogResult = DialogResult.OK;
			this.Close();
		}

		private void checkBoxDisplayPowerSourceName_CheckedChanged(object sender, System.EventArgs e) {
			updateButtons();
		}

		private void buttonSettings_Click(object sender, System.EventArgs e) {
			LayoutTrackPowerConnectorInfo				powerSourceNameInfo  = new LayoutTrackPowerConnectorInfo(xmlInfo.Element);

			powerSourceNameInfo.Component = component;

			CommonUI.Dialogs.TextProviderSettings	settings = new CommonUI.Dialogs.TextProviderSettings(xmlInfo, powerSourceNameInfo);

			if(powerSourceNameInfo.Element == null)
				powerSourceNameInfo.Element = LayoutInfo.CreateProviderElement(xmlInfo.Element, "PowerSourceName", null);

			settings.ShowDialog(this);
		}
	}
}
