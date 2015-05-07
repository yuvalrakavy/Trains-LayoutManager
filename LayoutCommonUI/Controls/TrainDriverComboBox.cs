using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Xml;

using LayoutManager;
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls
{
	/// <summary>
	/// Summary description for TrainDriverComboBox.
	/// </summary>
	public class TrainDriverComboBox : System.Windows.Forms.UserControl
	{
		private ComboBox comboBoxDrivers;
		private Button buttonDriverSettings;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private void endOfDesignerVariables() {}

		TrainCommonInfo		train;

		public TrainCommonInfo Train {
			set {
				train = value;

				if(train != null)
					initialize();
			}

			get {
				return train;
			}
		}

		public bool ValidateInput() {
			if(comboBoxDrivers.SelectedItem == null) {
				MessageBox.Show(this, "No valid driver is selected", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
				comboBoxDrivers.Focus();
				return false;
			}

			return true;
		}

		public bool Commit() {
			if(!ValidateInput())
				return false;

			DriverItem	selected = (DriverItem)comboBoxDrivers.SelectedItem;
	
			train.DriverElement = selected.DriverElement;
			return true;
		}

		public TrainDriverComboBox()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		private void initialize() {
			XmlDocument	driversDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();

			// enum the possible drivers, and build a menu
			driversDoc.LoadXml("<Drivers />");
			EventManager.Event(new LayoutEvent(train, "enum-train-drivers", null, driversDoc.DocumentElement));

			foreach(XmlElement driverElement in driversDoc.DocumentElement)
				comboBoxDrivers.Items.Add(new DriverItem(driverElement));

			foreach(DriverItem driverItem in comboBoxDrivers.Items)
				if(driverItem.Type == Train.Driver.Type) {
					comboBoxDrivers.SelectedItem = driverItem;
					break;
				}

			setDriverSettingButtonState();
		}

		private bool IsSelectedHasSettings() {
			DriverItem	selected = (DriverItem)comboBoxDrivers.SelectedItem;

			if(selected != null) {
				object	oResult = EventManager.Event(new LayoutEvent(selected.DriverElement, "query-driver-setting-dialog"));

				if(oResult != null && (bool)oResult)
					return true;
			}

			return false;
		}

		private void setDriverSettingButtonState() {
			DriverItem	selected = (DriverItem)comboBoxDrivers.SelectedItem;

			buttonDriverSettings.Enabled = IsSelectedHasSettings();
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.comboBoxDrivers = new ComboBox();
			this.buttonDriverSettings = new Button();
			this.SuspendLayout();
			// 
			// comboBoxDrivers
			// 
			this.comboBoxDrivers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxDrivers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxDrivers.Location = new System.Drawing.Point(0, 0);
			this.comboBoxDrivers.Name = "comboBoxDrivers";
			this.comboBoxDrivers.Size = new System.Drawing.Size(121, 21);
			this.comboBoxDrivers.TabIndex = 0;
			this.comboBoxDrivers.SelectedIndexChanged += new System.EventHandler(this.comboBoxDrivers_SelectedIndexChanged);
			// 
			// buttonDriverSettings
			// 
			this.buttonDriverSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDriverSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.buttonDriverSettings.Location = new System.Drawing.Point(120, 0);
			this.buttonDriverSettings.Name = "buttonDriverSettings";
			this.buttonDriverSettings.Size = new System.Drawing.Size(24, 21);
			this.buttonDriverSettings.TabIndex = 1;
			this.buttonDriverSettings.Text = "...";
			this.buttonDriverSettings.Click += new System.EventHandler(this.buttonDriverSettings_Click);
			// 
			// TrainDriverComboBox
			// 
			this.Controls.Add(this.buttonDriverSettings);
			this.Controls.Add(this.comboBoxDrivers);
			this.Name = "TrainDriverComboBox";
			this.Size = new System.Drawing.Size(144, 24);
			this.ResumeLayout(false);

		}
		#endregion

		private void comboBoxDrivers_SelectedIndexChanged(object sender, System.EventArgs e) {
			setDriverSettingButtonState();

			if(IsSelectedHasSettings())
				buttonDriverSettings.PerformClick();
		}

		private void buttonDriverSettings_Click(object sender, System.EventArgs e) {
			DriverItem	selected = (DriverItem)comboBoxDrivers.SelectedItem;

			if(selected != null)
				EventManager.Event(new LayoutEvent(selected.DriverElement, "edit-driver-setting", null, train));
		}

		class DriverItem {
			XmlElement	driverElement;

			public DriverItem(XmlElement driverElement) {
				this.driverElement = driverElement;
			}

            public XmlElement DriverElement => driverElement;

            public override string ToString() => driverElement.GetAttribute("TypeName");

            public string Type => driverElement.GetAttribute("Type");
        }
	}
}
