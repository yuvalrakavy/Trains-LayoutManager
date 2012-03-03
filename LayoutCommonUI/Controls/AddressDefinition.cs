using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;
using LayoutManager.Components;
using System.Collections.Generic;

namespace LayoutManager.CommonUI.Controls
{
	/// <summary>
	/// Summary description for AddressDefinition.
	/// </summary>
	public class AddressDefinition : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxCommandStations;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxUnit;
		private System.Windows.Forms.CheckBox checkBoxShowAddress;
		private System.Windows.Forms.Button buttonDisplaySettings;
		private System.Windows.Forms.ComboBox comboBoxSubunit;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private void endOfDesignerVariables() { }

		String				elementName = "Address";
		LayoutModel			model = null;
		LayoutXmlInfo		xmlInfo;
		AddressUsage		usage = AddressUsage.Locomotive;
		bool				defaultIsShowAddressAlways = false;
		string				currentLayoutCommandStation = "";
		AddressFormatInfo	addressFormat = null;

		public AddressDefinition()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		public String ElementName {
			get {
				return elementName;
			}

			set {
				elementName = value;
			}
		}

		public bool DefaultIsShowAddressAlways {
			set {
				defaultIsShowAddressAlways = value;
			}

			get {
				return defaultIsShowAddressAlways;
			}
		}

		public AddressUsage Usage {
			get {
				return usage;
			}

			set {
				usage = value;
			}
		}

		public LayoutModel Model {
			get {
				return model;
			}
			set {
				model = value;
			}
		}

		public LayoutXmlInfo XmlInfo {
			get {
				return xmlInfo;
			}

			set {
				xmlInfo = value;
				if(xmlInfo != null)
					initialize();
			}
		}

		LayoutAddressInfo getAddressProvider(bool create) {
			LayoutAddressInfo	addressProvider = new LayoutAddressInfo(xmlInfo.DocumentElement, model, elementName);

			if(addressProvider.Element == null && create)
				addressProvider.Element = LayoutInfo.CreateProviderElement(xmlInfo, null, elementName);

			return addressProvider;
		}

		void initialize() {
			LayoutAddressInfo	addressProvider = getAddressProvider(false);

			foreach(ILayoutPowerSource powerSource in model.Components<ILayoutPowerSource>())
				comboBoxCommandStations.Items.Add(((IModelComponentHasName)powerSource).NameProvider.Text);

            comboBoxCommandStations.Text = addressProvider.CommandStation;
			updateLayout();

			if(addressProvider.Element != null) 
				textBoxUnit.Text = addressProvider.Unit.ToString();
			else
				textBoxUnit.Text = "";

			if(addressProvider.Element == null)
				checkBoxShowAddress.Checked  = defaultIsShowAddressAlways;
			else
				checkBoxShowAddress.Checked = addressProvider.Visible;

			comboBoxSubunit.SelectedIndex = addressProvider.Subunit - addressFormat.SubunitMin;
		}

		public bool ValidateValue() {
			if(comboBoxCommandStations.Text.Trim() != "" && textBoxUnit.Text.Trim() != "") {
				try {
					int	unit = Int32.Parse(textBoxUnit.Text);

					if(unit < addressFormat.UnitMin || unit > addressFormat.UnitMax)
						throw new FormatException("must be between " + addressFormat.UnitMin + " and " + addressFormat.UnitMax);
				} catch(FormatException ex) {
					MessageBox.Show(this, "Invalid unit address: " + ex.Message, "Invalid Unit Address", MessageBoxButtons.OK, MessageBoxIcon.Error);
					textBoxUnit.Focus();
					return false;
				}
			}


			return true;
		}

		public void updateLayout() {
			if(comboBoxCommandStations.Text != currentLayoutCommandStation) {
				object	result = Model.EventManager.Event(new LayoutEvent(Usage, "get-named-command-station-address-format",
					"<CommandStation Name='" + XmlConvert.EncodeName(comboBoxCommandStations.Text) + "' />"));

				// If could not get address for the currently named command station, then try to get one for the one (or first...) command
				// station component that is already on the layout. If no command station is on the layout, try to get any command station
				// address format, hoping that at least one command station handler module is loaded.
				if(result == null) {
					IList<IModelComponentIsCommandStation> commandStations = Model.Components<IModelComponentIsCommandStation>();

					if(commandStations.Count == 0)
						result = Model.EventManager.Event(new LayoutEvent(Usage, "get-command-station-address-format", "<CommandStation Type='Any' />"));
					else
						result = Model.EventManager.Event(new LayoutEvent(Usage, "get-named-command-station-address-format", "<CommandStation Name='" + XmlConvert.EncodeName(((IModelComponentIsCommandStation)commandStations[0]).NameProvider.Name) + "' />"));
				}

				if(result != null) {
					if(result is AddressFormatInfo)
						addressFormat = (AddressFormatInfo)result;
					else if(result is XmlElement)
						addressFormat = new AddressFormatInfo((XmlElement)result);
					else
						throw new ApplicationException("Invalid address information returned for command station" + comboBoxCommandStations.Text);

					currentLayoutCommandStation = comboBoxCommandStations.Text;

					comboBoxSubunit.Visible = addressFormat.ShowSubunit;

					if(addressFormat.ShowSubunit) {
						comboBoxSubunit.Items.Clear();

						for(int i = addressFormat.SubunitMin; i <= addressFormat.SubunitMax; i++) {
							if(addressFormat.SubunitFormat == AddressFormatInfo.SubunitFormatValue.Number)
								comboBoxSubunit.Items.Add(i.ToString());
							else
								comboBoxSubunit.Items.Add(new String(Convert.ToChar(i + 'a'), 1));
						}
					}
				}
			}
		}

		public bool Commit() {
			if(!ValidateValue())
				return false;

			if(comboBoxCommandStations.Text.Trim() != "" && textBoxUnit.Text.Trim() != "") {
				LayoutAddressInfo	addressProvider = getAddressProvider(true);

				addressProvider.Namespace = addressFormat.Namespace;
				addressProvider.CommandStation = comboBoxCommandStations.Text;
				addressProvider.Unit = Int32.Parse(textBoxUnit.Text);

				if(comboBoxSubunit.Visible)
					addressProvider.Subunit = comboBoxSubunit.SelectedIndex + addressFormat.SubunitMin;

				addressProvider.Visible = checkBoxShowAddress.Checked;

				string	subunitString = "";

				if(addressFormat.ShowSubunit) {
					if(addressFormat.SubunitFormat == AddressFormatInfo.SubunitFormatValue.Number)
						subunitString = "[" + addressProvider.Subunit + "]";
					else
						subunitString = new string(Convert.ToChar(addressProvider.Subunit + 'a'), 1);
				}

				addressProvider.Text = addressProvider.CommandStation + "/" + addressProvider.Unit + subunitString;
			}

			return true;
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
			this.label1 = new System.Windows.Forms.Label();
			this.comboBoxCommandStations = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxUnit = new System.Windows.Forms.TextBox();
			this.checkBoxShowAddress = new System.Windows.Forms.CheckBox();
			this.buttonDisplaySettings = new System.Windows.Forms.Button();
			this.comboBoxSubunit = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 4);
			this.label1.Name = "label1";
			this.label1.TabIndex = 0;
			this.label1.Text = "Command station:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBoxCommandStations
			// 
			this.comboBoxCommandStations.Location = new System.Drawing.Point(120, 5);
			this.comboBoxCommandStations.Name = "comboBoxCommandStations";
			this.comboBoxCommandStations.Size = new System.Drawing.Size(144, 21);
			this.comboBoxCommandStations.TabIndex = 1;
			this.comboBoxCommandStations.SelectedIndexChanged += new System.EventHandler(this.comboBoxCommandStations_SelectedIndexChanged);
			this.comboBoxCommandStations.Leave += new System.EventHandler(this.comboBoxCommandStations_Leave);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 29);
			this.label2.Name = "label2";
			this.label2.TabIndex = 2;
			this.label2.Text = "Address:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxUnit
			// 
			this.textBoxUnit.Location = new System.Drawing.Point(120, 30);
			this.textBoxUnit.Name = "textBoxUnit";
			this.textBoxUnit.Size = new System.Drawing.Size(40, 20);
			this.textBoxUnit.TabIndex = 3;
			this.textBoxUnit.Text = "";
			// 
			// checkBoxShowAddress
			// 
			this.checkBoxShowAddress.Location = new System.Drawing.Point(17, 60);
			this.checkBoxShowAddress.Name = "checkBoxShowAddress";
			this.checkBoxShowAddress.Size = new System.Drawing.Size(138, 20);
			this.checkBoxShowAddress.TabIndex = 5;
			this.checkBoxShowAddress.Text = "Address always shown";
			// 
			// buttonDisplaySettings
			// 
			this.buttonDisplaySettings.Location = new System.Drawing.Point(160, 60);
			this.buttonDisplaySettings.Name = "buttonDisplaySettings";
			this.buttonDisplaySettings.Size = new System.Drawing.Size(104, 23);
			this.buttonDisplaySettings.TabIndex = 6;
			this.buttonDisplaySettings.Text = "Display settings...";
			this.buttonDisplaySettings.Click += new System.EventHandler(this.buttonDisplaySettings_Click);
			// 
			// comboBoxSubunit
			// 
			this.comboBoxSubunit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxSubunit.Items.AddRange(new object[] {
																 "a",
																 "b"});
			this.comboBoxSubunit.Location = new System.Drawing.Point(164, 30);
			this.comboBoxSubunit.Name = "comboBoxSubunit";
			this.comboBoxSubunit.Size = new System.Drawing.Size(40, 21);
			this.comboBoxSubunit.TabIndex = 4;
			// 
			// AddressDefinition
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.comboBoxSubunit,
																		  this.buttonDisplaySettings,
																		  this.checkBoxShowAddress,
																		  this.textBoxUnit,
																		  this.label2,
																		  this.comboBoxCommandStations,
																		  this.label1});
			this.Name = "AddressDefinition";
			this.Size = new System.Drawing.Size(272, 88);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonDisplaySettings_Click(object sender, System.EventArgs e) {
			LayoutAddressInfo				addressProvider = getAddressProvider(true);
			Dialogs.TextProviderSettings	settingDialog = new Dialogs.TextProviderSettings(model, xmlInfo, addressProvider);

			settingDialog.ShowDialog();
		}

		private void comboBoxCommandStations_SelectedIndexChanged(object sender, System.EventArgs e) {
			updateLayout();
		}

		private void comboBoxCommandStations_Leave(object sender, System.EventArgs e) {
			updateLayout();
		}
	}
}
