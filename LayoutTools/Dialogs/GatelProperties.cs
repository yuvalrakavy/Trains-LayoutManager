using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs
{
	/// <summary>
	/// Summary description for TrackContactProperties.
	/// </summary>
	public class GateProperties : System.Windows.Forms.Form, ILayoutComponentPropertiesDialog
	{
		private System.Windows.Forms.TabPage tabPageGeneral;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private LayoutManager.CommonUI.Controls.NameDefinition nameDefinition;
		private GroupBox groupBox1;
		private Panel panelGatePreview;
		private RadioButton radioButtonOpenUp;
		private RadioButton radioButtonOpenDown;
		private RadioButton radioButtonOpenRight;
		private RadioButton radioButtonOpenLeft;
		LayoutGateComponent component;
		LayoutXmlInfo xmlInfo;
		private TabPage tabPageSetup;
		private CheckBox checkBoxReverseLogic;
		private Label label3;
		private TextBox textBoxOpenCloseTimeout;
		private CheckBox checkBoxHasGateClosedSensor;
		private Label label2;
		private CheckBox checkBoxHasGateOpenSensor;
		private CheckBox checkBoxGateClosedAcriveState;
		private CheckBox checkBoxGateOpenActiveState;
		bool isVertical;

		class DriverEntry {
			XmlElement driverElement;

			public DriverEntry(XmlElement driverElement) {
				this.driverElement = driverElement;
			}

			public string DriverName {
				get {
					return driverElement.GetAttribute("Name");
				}
			}

			public override string ToString() {
				return driverElement.GetAttribute("Description");
			}
		}

		public GateProperties(ModelComponent component, PlacementInfo placementInfo)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.component = (LayoutGateComponent)component;
			this.xmlInfo = new LayoutXmlInfo(component);

			nameDefinition.XmlInfo = xmlInfo;
			nameDefinition.Component = component;

			LayoutGateComponentInfo info = new LayoutGateComponentInfo(this.component, xmlInfo.Element);
			XmlDocument driversDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();

			isVertical = LayoutTrackComponent.IsVertical(placementInfo.Track);

			if(isVertical) {
				radioButtonOpenLeft.Visible = false;
				radioButtonOpenRight.Visible = false;

				if(info.OpenUpOrLeft)
					radioButtonOpenUp.Checked = true;
				else
					radioButtonOpenDown.Checked = true;
			}
			else {
				radioButtonOpenUp.Visible = false;
				radioButtonOpenDown.Visible = false;

				if(info.OpenUpOrLeft)
					radioButtonOpenLeft.Checked = true;
				else
					radioButtonOpenRight.Checked = true;
			}

			textBoxOpenCloseTimeout.Text = info.OpenCloseTimeout.ToString();
			checkBoxReverseLogic.Checked = info.ReverseLogic;
			checkBoxHasGateClosedSensor.Checked = info.HasGateClosedSensor;
			checkBoxHasGateOpenSensor.Checked = info.HasGateOpenSensor;
			checkBoxGateOpenActiveState.Checked = info.GateOpenSensorActiveState;
			checkBoxGateClosedAcriveState.Checked = info.GateClosedSensorActiveState;

			panelGatePreview.Invalidate();

			UpdateStates();
		}

		private void UpdateStates() {
			checkBoxGateClosedAcriveState.Enabled = checkBoxHasGateClosedSensor.Checked;
			checkBoxGateOpenActiveState.Enabled = checkBoxHasGateOpenSensor.Checked;
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
			this.tabPageGeneral = new System.Windows.Forms.TabPage();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioButtonOpenLeft = new System.Windows.Forms.RadioButton();
			this.radioButtonOpenRight = new System.Windows.Forms.RadioButton();
			this.radioButtonOpenDown = new System.Windows.Forms.RadioButton();
			this.panelGatePreview = new System.Windows.Forms.Panel();
			this.radioButtonOpenUp = new System.Windows.Forms.RadioButton();
			this.nameDefinition = new LayoutManager.CommonUI.Controls.NameDefinition();
			this.tabPageSetup = new System.Windows.Forms.TabPage();
			this.checkBoxGateClosedAcriveState = new System.Windows.Forms.CheckBox();
			this.checkBoxGateOpenActiveState = new System.Windows.Forms.CheckBox();
			this.checkBoxHasGateOpenSensor = new System.Windows.Forms.CheckBox();
			this.checkBoxReverseLogic = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxOpenCloseTimeout = new System.Windows.Forms.TextBox();
			this.checkBoxHasGateClosedSensor = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tabControl.SuspendLayout();
			this.tabPageGeneral.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabPageSetup.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(208, 272);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(120, 272);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabPageGeneral);
			this.tabControl.Controls.Add(this.tabPageSetup);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Top;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(290, 264);
			this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
			this.tabControl.TabIndex = 0;
			// 
			// tabPageGeneral
			// 
			this.tabPageGeneral.Controls.Add(this.groupBox1);
			this.tabPageGeneral.Controls.Add(this.nameDefinition);
			this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
			this.tabPageGeneral.Name = "tabPageGeneral";
			this.tabPageGeneral.Size = new System.Drawing.Size(282, 238);
			this.tabPageGeneral.TabIndex = 0;
			this.tabPageGeneral.Text = "General";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioButtonOpenLeft);
			this.groupBox1.Controls.Add(this.radioButtonOpenRight);
			this.groupBox1.Controls.Add(this.radioButtonOpenDown);
			this.groupBox1.Controls.Add(this.panelGatePreview);
			this.groupBox1.Controls.Add(this.radioButtonOpenUp);
			this.groupBox1.Location = new System.Drawing.Point(25, 76);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(240, 137);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Gate opening direction:";
			// 
			// radioButtonOpenLeft
			// 
			this.radioButtonOpenLeft.AutoSize = true;
			this.radioButtonOpenLeft.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioButtonOpenLeft.Location = new System.Drawing.Point(40, 65);
			this.radioButtonOpenLeft.Name = "radioButtonOpenLeft";
			this.radioButtonOpenLeft.Size = new System.Drawing.Size(43, 17);
			this.radioButtonOpenLeft.TabIndex = 6;
			this.radioButtonOpenLeft.Text = "&Left";
			this.radioButtonOpenLeft.CheckedChanged += new System.EventHandler(this.gateOpenDirectionChanged);
			// 
			// radioButtonOpenRight
			// 
			this.radioButtonOpenRight.AutoSize = true;
			this.radioButtonOpenRight.Location = new System.Drawing.Point(161, 65);
			this.radioButtonOpenRight.Name = "radioButtonOpenRight";
			this.radioButtonOpenRight.Size = new System.Drawing.Size(50, 17);
			this.radioButtonOpenRight.TabIndex = 5;
			this.radioButtonOpenRight.Text = "&Right";
			this.radioButtonOpenRight.CheckedChanged += new System.EventHandler(this.gateOpenDirectionChanged);
			// 
			// radioButtonOpenDown
			// 
			this.radioButtonOpenDown.AutoSize = true;
			this.radioButtonOpenDown.Location = new System.Drawing.Point(115, 111);
			this.radioButtonOpenDown.Name = "radioButtonOpenDown";
			this.radioButtonOpenDown.Size = new System.Drawing.Size(53, 17);
			this.radioButtonOpenDown.TabIndex = 4;
			this.radioButtonOpenDown.Text = "&Down";
			this.radioButtonOpenDown.CheckedChanged += new System.EventHandler(this.gateOpenDirectionChanged);
			// 
			// panelGatePreview
			// 
			this.panelGatePreview.Location = new System.Drawing.Point(85, 39);
			this.panelGatePreview.Name = "panelGatePreview";
			this.panelGatePreview.Size = new System.Drawing.Size(70, 70);
			this.panelGatePreview.TabIndex = 3;
			this.panelGatePreview.Paint += new System.Windows.Forms.PaintEventHandler(this.panelGatePreview_Paint);
			// 
			// radioButtonOpenUp
			// 
			this.radioButtonOpenUp.AutoSize = true;
			this.radioButtonOpenUp.Location = new System.Drawing.Point(115, 20);
			this.radioButtonOpenUp.Name = "radioButtonOpenUp";
			this.radioButtonOpenUp.Size = new System.Drawing.Size(39, 17);
			this.radioButtonOpenUp.TabIndex = 2;
			this.radioButtonOpenUp.Text = "&Up";
			this.radioButtonOpenUp.CheckedChanged += new System.EventHandler(this.gateOpenDirectionChanged);
			// 
			// nameDefinition
			// 
			this.nameDefinition.Component = null;
			this.nameDefinition.DefaultIsVisible = true;
			this.nameDefinition.ElementName = "Name";
			this.nameDefinition.IsOptional = false;
			this.nameDefinition.Location = new System.Drawing.Point(-1, 3);
			this.nameDefinition.Name = "nameDefinition";
			this.nameDefinition.Size = new System.Drawing.Size(280, 53);
			this.nameDefinition.TabIndex = 0;
			this.nameDefinition.XmlInfo = null;
			// 
			// tabPageSetup
			// 
			this.tabPageSetup.Controls.Add(this.checkBoxGateClosedAcriveState);
			this.tabPageSetup.Controls.Add(this.checkBoxGateOpenActiveState);
			this.tabPageSetup.Controls.Add(this.checkBoxHasGateOpenSensor);
			this.tabPageSetup.Controls.Add(this.checkBoxReverseLogic);
			this.tabPageSetup.Controls.Add(this.label3);
			this.tabPageSetup.Controls.Add(this.textBoxOpenCloseTimeout);
			this.tabPageSetup.Controls.Add(this.checkBoxHasGateClosedSensor);
			this.tabPageSetup.Controls.Add(this.label2);
			this.tabPageSetup.Location = new System.Drawing.Point(4, 22);
			this.tabPageSetup.Name = "tabPageSetup";
			this.tabPageSetup.Size = new System.Drawing.Size(282, 238);
			this.tabPageSetup.TabIndex = 1;
			this.tabPageSetup.Text = "Setup";
			this.tabPageSetup.UseVisualStyleBackColor = true;
			// 
			// checkBoxGateClosedAcriveState
			// 
			this.checkBoxGateClosedAcriveState.AutoSize = true;
			this.checkBoxGateClosedAcriveState.Location = new System.Drawing.Point(28, 120);
			this.checkBoxGateClosedAcriveState.Name = "checkBoxGateClosedAcriveState";
			this.checkBoxGateClosedAcriveState.Size = new System.Drawing.Size(185, 17);
			this.checkBoxGateClosedAcriveState.TabIndex = 8;
			this.checkBoxGateClosedAcriveState.Text = "Sensor is ON when gate is closed";
			this.checkBoxGateClosedAcriveState.UseVisualStyleBackColor = true;
			// 
			// checkBoxGateOpenActiveState
			// 
			this.checkBoxGateOpenActiveState.AutoSize = true;
			this.checkBoxGateOpenActiveState.Location = new System.Drawing.Point(28, 72);
			this.checkBoxGateOpenActiveState.Name = "checkBoxGateOpenActiveState";
			this.checkBoxGateOpenActiveState.Size = new System.Drawing.Size(178, 17);
			this.checkBoxGateOpenActiveState.TabIndex = 7;
			this.checkBoxGateOpenActiveState.Text = "Sensor is ON when gate is open";
			this.checkBoxGateOpenActiveState.UseVisualStyleBackColor = true;
			// 
			// checkBoxHasGateOpenSensor
			// 
			this.checkBoxHasGateOpenSensor.AutoSize = true;
			this.checkBoxHasGateOpenSensor.Location = new System.Drawing.Point(8, 51);
			this.checkBoxHasGateOpenSensor.Name = "checkBoxHasGateOpenSensor";
			this.checkBoxHasGateOpenSensor.Size = new System.Drawing.Size(171, 17);
			this.checkBoxHasGateOpenSensor.TabIndex = 6;
			this.checkBoxHasGateOpenSensor.Text = "Has sensor for gate open state";
			this.checkBoxHasGateOpenSensor.UseVisualStyleBackColor = true;
			this.checkBoxHasGateOpenSensor.CheckedChanged += new System.EventHandler(this.checkBoxHasSensor_CheckedChanged);
			// 
			// checkBoxReverseLogic
			// 
			this.checkBoxReverseLogic.AutoSize = true;
			this.checkBoxReverseLogic.Location = new System.Drawing.Point(8, 15);
			this.checkBoxReverseLogic.Name = "checkBoxReverseLogic";
			this.checkBoxReverseLogic.Size = new System.Drawing.Size(197, 17);
			this.checkBoxReverseLogic.TabIndex = 5;
			this.checkBoxReverseLogic.Text = "Reverse open/close command logic";
			this.checkBoxReverseLogic.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(197, 168);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(47, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "seconds";
			// 
			// textBoxOpenCloseTimeout
			// 
			this.textBoxOpenCloseTimeout.Location = new System.Drawing.Point(136, 165);
			this.textBoxOpenCloseTimeout.Name = "textBoxOpenCloseTimeout";
			this.textBoxOpenCloseTimeout.Size = new System.Drawing.Size(58, 20);
			this.textBoxOpenCloseTimeout.TabIndex = 3;
			// 
			// checkBoxHasGateClosedSensor
			// 
			this.checkBoxHasGateClosedSensor.AutoSize = true;
			this.checkBoxHasGateClosedSensor.Location = new System.Drawing.Point(8, 98);
			this.checkBoxHasGateClosedSensor.Name = "checkBoxHasGateClosedSensor";
			this.checkBoxHasGateClosedSensor.Size = new System.Drawing.Size(178, 17);
			this.checkBoxHasGateClosedSensor.TabIndex = 2;
			this.checkBoxHasGateClosedSensor.Text = "Has sensor for gate closed state";
			this.checkBoxHasGateClosedSensor.UseVisualStyleBackColor = true;
			this.checkBoxHasGateClosedSensor.CheckedChanged += new System.EventHandler(this.checkBoxHasSensor_CheckedChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 168);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(130, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Gate open/close timeout: ";
			// 
			// GateProperties
			// 
			this.ClientSize = new System.Drawing.Size(290, 306);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.tabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "GateProperties";
			this.ShowInTaskbar = false;
			this.Text = "Gate Properties";
			this.tabControl.ResumeLayout(false);
			this.tabPageGeneral.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tabPageSetup.ResumeLayout(false);
			this.tabPageSetup.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e) {
			LayoutGateComponentInfo info = new LayoutGateComponentInfo(component, xmlInfo.Element);

			if(!nameDefinition.Commit())
				return;

			int timeout;

			if(!int.TryParse(textBoxOpenCloseTimeout.Text, out timeout)) {
				MessageBox.Show("Invalid timeout value");
				textBoxOpenCloseTimeout.Focus();
				return;
			}

			info.OpenUpOrLeft = radioButtonOpenUp.Checked || radioButtonOpenLeft.Checked;
			info.HasGateOpenSensor = checkBoxHasGateOpenSensor.Checked;
			info.HasGateClosedSensor = checkBoxHasGateClosedSensor.Checked;
			info.ReverseLogic = checkBoxReverseLogic.Checked;
			info.GateOpenSensorActiveState = checkBoxGateOpenActiveState.Checked;
			info.GateClosedSensorActiveState = checkBoxGateClosedAcriveState.Checked;
			info.OpenCloseTimeout = timeout;

			this.DialogResult = DialogResult.OK;
			Close();
		}

		private void panelGatePreview_Paint(object sender, PaintEventArgs e) {
			e.Graphics.FillRectangle(Brushes.White, 3, 3, 64, 64);
			e.Graphics.DrawRectangle(Pens.Black, 3, 3, 64, 64);
			e.Graphics.TranslateTransform(3, 3);

			LayoutComponentConnectionPoint[] cps;

			if(isVertical)
				cps = new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B };
			else
				cps = new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.R };

			LayoutStraightTrackPainter trackPainter = new LayoutStraightTrackPainter(new Size(64, 64), cps);

			trackPainter.Paint(e.Graphics);

			LayoutGatePainter gatePainter = new LayoutGatePainter(new Size(64, 64), isVertical, radioButtonOpenUp.Checked || radioButtonOpenLeft.Checked, 60);

			gatePainter.Paint(e.Graphics);
		}

		private void gateOpenDirectionChanged(object sender, EventArgs e) {
			panelGatePreview.Invalidate();
		}

		private void buttonSetup_Click(object sender, EventArgs e) {

		}

		private void checkBoxHasSensor_CheckedChanged(object sender, EventArgs e) {
			UpdateStates();
		}
	}
}
