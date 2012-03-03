using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Text.RegularExpressions;

using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs
{
	/// <summary>
	/// Summary description for TrackContactProperties.
	/// </summary>
	public class TurnoutProperties : System.Windows.Forms.Form, ILayoutComponentPropertiesDialog
	{
		private System.Windows.Forms.TabPage tabPageAddress;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private System.Windows.Forms.CheckBox checkBoxReverseLogic;
		private System.Windows.Forms.CheckBox checkBoxHasBuiltinDecoder;
		private System.Windows.Forms.ComboBox comboBoxBuiltinDecoders;
		private CheckBox checkBoxHasFeedback;
		LayoutXmlInfo	xmlInfo;

		private void FixLabel(Control c, string componentName) {
			c.Text = Regex.Replace(c.Text, "COMPONENT", componentName);
		}


		public TurnoutProperties(ModelComponent component)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			string componentName = component is LayoutDoubleSlipTrackComponent ? "Double Slip" : "Turnout";
			IModelComponentIsMultiPath multiPathComponent = component as IModelComponentIsMultiPath;

			FixLabel(this, componentName);
			FixLabel(checkBoxHasBuiltinDecoder, componentName);
			FixLabel(checkBoxReverseLogic, componentName);
			FixLabel(checkBoxHasFeedback, componentName);

			this.xmlInfo = new LayoutXmlInfo(component);

			if(xmlInfo.Element.HasAttribute("ReverseLogic"))
				checkBoxReverseLogic.Checked = XmlConvert.ToBoolean(xmlInfo.Element.GetAttribute("ReverseLogic"));
			else
				checkBoxReverseLogic.Checked = false;

			if(xmlInfo.Element.HasAttribute("HasFeedback"))
				checkBoxHasFeedback.Checked = XmlConvert.ToBoolean(xmlInfo.Element.GetAttribute("HasFeedback"));
			else
				checkBoxHasFeedback.Checked = false;

			IEnumerable<IModelComponentIsCommandStation> commandStations = LayoutModel.Components<IModelComponentIsCommandStation>(LayoutPhase.All);

			foreach(IModelComponentIsCommandStation commandStation in commandStations) {
				foreach(ControlBus bus in LayoutModel.ControlManager.Buses.Buses(commandStation)) {
					IList<string> moduleTypeNames = bus.BusType.GetConnectableControlModuleTypeNames(new ControlConnectionPointDestination(multiPathComponent, multiPathComponent.ControlConnectionDescriptions[0]));

					if(moduleTypeNames.Count > 0) {
						foreach(string moduleTypeName in moduleTypeNames) {
							ControlModuleType	moduleType = LayoutModel.ControlManager.GetModuleType(moduleTypeName);

							if(moduleType.BuiltIn)
								comboBoxBuiltinDecoders.Items.Add(new Item(moduleType));
						}
					}
				}
			}

			if(comboBoxBuiltinDecoders.Items.Count == 0) {
				checkBoxHasBuiltinDecoder.Enabled = false;
				comboBoxBuiltinDecoders.Visible = false;
			}
			else {
				comboBoxBuiltinDecoders.SelectedIndex = 0;

				if(xmlInfo.Element.HasAttribute("BuiltinDecoderTypeName")) {
					string	typeName = xmlInfo.Element.GetAttribute("BuiltinDecoderTypeName");

					foreach(Item item in comboBoxBuiltinDecoders.Items)
						if(item.ModuleType.TypeName == typeName) {
							comboBoxBuiltinDecoders.SelectedItem = item;
							checkBoxHasBuiltinDecoder.Checked = true;
							break;
						}
				}
			}

			updateButtons();
		}

		private void updateButtons() {
			comboBoxBuiltinDecoders.Enabled = checkBoxHasBuiltinDecoder.Checked;
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
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPageAddress = new System.Windows.Forms.TabPage();
			this.comboBoxBuiltinDecoders = new System.Windows.Forms.ComboBox();
			this.checkBoxHasBuiltinDecoder = new System.Windows.Forms.CheckBox();
			this.checkBoxReverseLogic = new System.Windows.Forms.CheckBox();
			this.checkBoxHasFeedback = new System.Windows.Forms.CheckBox();
			this.tabControl.SuspendLayout();
			this.tabPageAddress.SuspendLayout();
			this.SuspendLayout();
// 
// buttonCancel
// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(208, 272);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
// 
// buttonOK
// 
			this.buttonOK.Location = new System.Drawing.Point(120, 272);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
// 
// tabControl
// 
			this.tabControl.Controls.Add(this.tabPageAddress);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Top;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(290, 264);
			this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
			this.tabControl.TabIndex = 0;
// 
// tabPageAddress
// 
			this.tabPageAddress.Controls.Add(this.checkBoxHasFeedback);
			this.tabPageAddress.Controls.Add(this.comboBoxBuiltinDecoders);
			this.tabPageAddress.Controls.Add(this.checkBoxHasBuiltinDecoder);
			this.tabPageAddress.Controls.Add(this.checkBoxReverseLogic);
			this.tabPageAddress.Location = new System.Drawing.Point(4, 22);
			this.tabPageAddress.Name = "tabPageAddress";
			this.tabPageAddress.Size = new System.Drawing.Size(282, 238);
			this.tabPageAddress.TabIndex = 0;
			this.tabPageAddress.Text = "General";
// 
// comboBoxBuiltinDecoders
// 
			this.comboBoxBuiltinDecoders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxBuiltinDecoders.FormattingEnabled = true;
			this.comboBoxBuiltinDecoders.Location = new System.Drawing.Point(32, 64);
			this.comboBoxBuiltinDecoders.Name = "comboBoxBuiltinDecoders";
			this.comboBoxBuiltinDecoders.Size = new System.Drawing.Size(176, 21);
			this.comboBoxBuiltinDecoders.TabIndex = 3;
// 
// checkBoxHasBuiltinDecoder
// 
			this.checkBoxHasBuiltinDecoder.Location = new System.Drawing.Point(16, 40);
			this.checkBoxHasBuiltinDecoder.Name = "checkBoxHasBuiltinDecoder";
			this.checkBoxHasBuiltinDecoder.Size = new System.Drawing.Size(221, 24);
			this.checkBoxHasBuiltinDecoder.TabIndex = 2;
			this.checkBoxHasBuiltinDecoder.Text = "COMPONENT has builtin decoder";
			this.checkBoxHasBuiltinDecoder.CheckedChanged += new System.EventHandler(this.checkBoxHasBuiltinDecoder_CheckedChanged);
// 
// checkBoxReverseLogic
// 
			this.checkBoxReverseLogic.Location = new System.Drawing.Point(16, 16);
			this.checkBoxReverseLogic.Name = "checkBoxReverseLogic";
			this.checkBoxReverseLogic.Size = new System.Drawing.Size(249, 24);
			this.checkBoxReverseLogic.TabIndex = 1;
			this.checkBoxReverseLogic.Text = "Reverse COMPONENT logic";
// 
// checkBoxHasFeedback
// 
			this.checkBoxHasFeedback.AutoSize = true;
			this.checkBoxHasFeedback.Location = new System.Drawing.Point(16, 92);
			this.checkBoxHasFeedback.Name = "checkBoxHasFeedback";
			this.checkBoxHasFeedback.Size = new System.Drawing.Size(182, 17);
			this.checkBoxHasFeedback.TabIndex = 4;
			this.checkBoxHasFeedback.Text = "COMPONENT provides feedback";
// 
// TurnoutProperties
// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(290, 306);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.tabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "TurnoutProperties";
			this.ShowInTaskbar = false;
			this.Text = "COMPONENT Properties";
			this.tabControl.ResumeLayout(false);
			this.tabPageAddress.ResumeLayout(false);
			this.tabPageAddress.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e) {
			xmlInfo.DocumentElement.SetAttribute("ReverseLogic", XmlConvert.ToString(checkBoxReverseLogic.Checked));
			xmlInfo.DocumentElement.SetAttribute("HasFeedback", XmlConvert.ToString(checkBoxHasFeedback.Checked));

			if(checkBoxHasBuiltinDecoder.Checked)
				xmlInfo.Element.SetAttribute("BuiltinDecoderTypeName", ((Item)comboBoxBuiltinDecoders.SelectedItem).ModuleType.TypeName);
			else
				xmlInfo.Element.RemoveAttribute("BuiltinDecoderTypeName");

			this.DialogResult = DialogResult.OK;
			Close();
		}

		private void checkBoxHasBuiltinDecoder_CheckedChanged(object sender, System.EventArgs e) {
			updateButtons();
		}

		class Item {
			ControlModuleType	moduleType;

			public Item(ControlModuleType moduleType) {
				this.moduleType = moduleType;
			}

			public ControlModuleType ModuleType {
				get {
					return moduleType;
				}
			}

			public override string ToString() {
				return moduleType.Name;
			}
		}
	}
}
