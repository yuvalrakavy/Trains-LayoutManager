using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.CommonUI.Controls;

namespace LayoutManager.Tools.Dialogs
{
	/// <summary>
	/// Summary description for TrainProperties.
	/// </summary>
	public class TrainProperties : System.Windows.Forms.Form, IObjectHasXml
	{
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.TabPage tabPageGeneral;
		private System.Windows.Forms.TabPage tabPageLocomotives;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.ListView listViewLocomotives;
		private System.Windows.Forms.ColumnHeader columnHeaderLocoName;
		private System.Windows.Forms.ColumnHeader columnAddress;
		private System.Windows.Forms.ColumnHeader columnHeaderLocoOrientation;
		private System.Windows.Forms.ColumnHeader columnHeaderLocoType;
		private System.Windows.Forms.Button buttonLocoAdd;
		private System.Windows.Forms.Button buttonLocoRemove;
		private System.Windows.Forms.Button buttonLocoEdit;
		private System.Windows.Forms.ContextMenu contextMenuEditLocomotive;
		private System.Windows.Forms.MenuItem menuItemLocoOrientation;
		private System.Windows.Forms.MenuItem menuItemLocoOrientationForward;
		private System.Windows.Forms.MenuItem menuItemLocoOrientationBackward;
		private System.Windows.Forms.MenuItem menuItemEditLocoDefinition;
		private System.Windows.Forms.Button buttonLocoMoveDown;
		private System.Windows.Forms.ImageList imageListButttons;
		private System.Windows.Forms.Button buttonLocoMoveUp;
		private System.Windows.Forms.Panel panelLocoImages;
		private System.Windows.Forms.TabPage tabPageAttributes;
		private LayoutManager.CommonUI.Controls.AttributesEditor attributesEditor;
		private System.Windows.Forms.TabPage tabPageDriver;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox checkBoxMagnetOnLastCar;
		private System.Windows.Forms.CheckBox checkBoxLastCarDetected;
		private System.Windows.Forms.Label label2;
		private LayoutManager.CommonUI.Controls.TrainDriverComboBox trainDriverComboBox;
		private LayoutManager.CommonUI.Controls.DrivingParameters drivingParameters;
		private LayoutManager.CommonUI.Controls.TrainLengthDiagram trainLengthDiagram;
		private GroupBox groupBox1;
		private System.ComponentModel.IContainer components;

		private void endOfDesignerVariables() { }

		TrainCommonInfo		train;
		ArrayList			locomotiveCancelList = new ArrayList();
		ArrayList			carsCancelList = new ArrayList();
		bool				locomotiveEdited = false;

		public TrainProperties(TrainCommonInfo train)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.train = train;

			trainLengthDiagram.Length = train.Length;
			trainDriverComboBox.Train = train;

			foreach(TrainLocomotiveInfo trainLocomotive in train.Locomotives)
				locomotiveCancelList.Add(new TrainLocomotiveCancelInfo(trainLocomotive));

			setTitleBar();
			textBoxName.Text = train.Name;

			checkBoxMagnetOnLastCar.Checked = train.LastCarTriggerBlockEdge;
			checkBoxLastCarDetected.Checked = train.LastCarDetectedByOccupancyBlock;

			foreach(TrainLocomotiveInfo trainLoco in train.Locomotives)
				listViewLocomotives.Items.Add(new TrainLocomotiveItem(trainLoco));

			drivingParameters.Element = train.Element;

			attributesEditor.AttributesSource = typeof(TrainStateInfo);
			attributesEditor.AttributesOwner = train;

			updateControls();

			EventManager.AddObjectSubscriptions(this);
		}

		public XmlElement Element {
			get {
				return train.Element;
			}
		}

		private void updateControls(Object sender, EventArgs e) {
			updateControls();
		}

		private void updateControls() {
			if(listViewLocomotives.SelectedItems.Count != 0) {
				buttonLocoEdit.Enabled = true;
				buttonLocoRemove.Enabled = true;
				buttonLocoMoveUp.Enabled = (listViewLocomotives.SelectedIndices[0] > 0);
				buttonLocoMoveDown.Enabled = (listViewLocomotives.SelectedIndices[0] < listViewLocomotives.Items.Count-1);
			}
			else {
				buttonLocoEdit.Enabled = false;
				buttonLocoRemove.Enabled = false;
				buttonLocoMoveDown.Enabled = false;
				buttonLocoMoveUp.Enabled = false;
			}

			panelLocoImages.Invalidate();
		}

		private void setTitleBar() {
			this.Text = "Train " + train.DisplayName + " Properties";
		}

		[LayoutEvent("train-configuration-changed", IfSender="*[@ID='`string(@ID)`']")]
		private void trainNameChanged(LayoutEvent e) {
			setTitleBar();
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrainProperties));
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPageGeneral = new System.Windows.Forms.TabPage();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.checkBoxLastCarDetected = new System.Windows.Forms.CheckBox();
			this.checkBoxMagnetOnLastCar = new System.Windows.Forms.CheckBox();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tabPageLocomotives = new System.Windows.Forms.TabPage();
			this.panelLocoImages = new System.Windows.Forms.Panel();
			this.buttonLocoMoveDown = new System.Windows.Forms.Button();
			this.imageListButttons = new System.Windows.Forms.ImageList(this.components);
			this.buttonLocoAdd = new System.Windows.Forms.Button();
			this.listViewLocomotives = new System.Windows.Forms.ListView();
			this.columnHeaderLocoName = new System.Windows.Forms.ColumnHeader();
			this.columnAddress = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderLocoOrientation = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderLocoType = new System.Windows.Forms.ColumnHeader();
			this.buttonLocoRemove = new System.Windows.Forms.Button();
			this.buttonLocoEdit = new System.Windows.Forms.Button();
			this.buttonLocoMoveUp = new System.Windows.Forms.Button();
			this.tabPageDriver = new System.Windows.Forms.TabPage();
			this.tabPageAttributes = new System.Windows.Forms.TabPage();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.contextMenuEditLocomotive = new System.Windows.Forms.ContextMenu();
			this.menuItemLocoOrientation = new System.Windows.Forms.MenuItem();
			this.menuItemLocoOrientationForward = new System.Windows.Forms.MenuItem();
			this.menuItemLocoOrientationBackward = new System.Windows.Forms.MenuItem();
			this.menuItemEditLocoDefinition = new System.Windows.Forms.MenuItem();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.trainLengthDiagram = new LayoutManager.CommonUI.Controls.TrainLengthDiagram();
			this.trainDriverComboBox = new LayoutManager.CommonUI.Controls.TrainDriverComboBox();
			this.drivingParameters = new LayoutManager.CommonUI.Controls.DrivingParameters();
			this.attributesEditor = new LayoutManager.CommonUI.Controls.AttributesEditor();
			this.tabControl1.SuspendLayout();
			this.tabPageGeneral.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.tabPageLocomotives.SuspendLayout();
			this.tabPageDriver.SuspendLayout();
			this.tabPageAttributes.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPageGeneral);
			this.tabControl1.Controls.Add(this.tabPageLocomotives);
			this.tabControl1.Controls.Add(this.tabPageDriver);
			this.tabControl1.Controls.Add(this.tabPageAttributes);
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(296, 272);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPageGeneral
			// 
			this.tabPageGeneral.Controls.Add(this.groupBox1);
			this.tabPageGeneral.Controls.Add(this.trainDriverComboBox);
			this.tabPageGeneral.Controls.Add(this.label2);
			this.tabPageGeneral.Controls.Add(this.groupBox2);
			this.tabPageGeneral.Controls.Add(this.textBoxName);
			this.tabPageGeneral.Controls.Add(this.label1);
			this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
			this.tabPageGeneral.Name = "tabPageGeneral";
			this.tabPageGeneral.Size = new System.Drawing.Size(288, 246);
			this.tabPageGeneral.TabIndex = 0;
			this.tabPageGeneral.Text = "General";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 202);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(264, 16);
			this.label2.TabIndex = 7;
			this.label2.Text = "When driven according to a trip-plan, train is driven:";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.checkBoxLastCarDetected);
			this.groupBox2.Controls.Add(this.checkBoxMagnetOnLastCar);
			this.groupBox2.Location = new System.Drawing.Point(8, 132);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(272, 64);
			this.groupBox2.TabIndex = 6;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Train last car:";
			// 
			// checkBoxLastCarDetected
			// 
			this.checkBoxLastCarDetected.Location = new System.Drawing.Point(7, 32);
			this.checkBoxLastCarDetected.Name = "checkBoxLastCarDetected";
			this.checkBoxLastCarDetected.Size = new System.Drawing.Size(256, 24);
			this.checkBoxLastCarDetected.TabIndex = 7;
			this.checkBoxLastCarDetected.Text = "Detected by train occupancy detection blocks";
			// 
			// checkBoxMagnetOnLastCar
			// 
			this.checkBoxMagnetOnLastCar.Location = new System.Drawing.Point(7, 15);
			this.checkBoxMagnetOnLastCar.Name = "checkBoxMagnetOnLastCar";
			this.checkBoxMagnetOnLastCar.Size = new System.Drawing.Size(240, 18);
			this.checkBoxMagnetOnLastCar.TabIndex = 6;
			this.checkBoxMagnetOnLastCar.Text = "Triggers track contact sensor";
			// 
			// textBoxName
			// 
			this.textBoxName.Location = new System.Drawing.Point(72, 22);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(176, 20);
			this.textBoxName.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(120, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Name:";
			// 
			// tabPageLocomotives
			// 
			this.tabPageLocomotives.Controls.Add(this.panelLocoImages);
			this.tabPageLocomotives.Controls.Add(this.buttonLocoMoveDown);
			this.tabPageLocomotives.Controls.Add(this.buttonLocoAdd);
			this.tabPageLocomotives.Controls.Add(this.listViewLocomotives);
			this.tabPageLocomotives.Controls.Add(this.buttonLocoRemove);
			this.tabPageLocomotives.Controls.Add(this.buttonLocoEdit);
			this.tabPageLocomotives.Controls.Add(this.buttonLocoMoveUp);
			this.tabPageLocomotives.Location = new System.Drawing.Point(4, 22);
			this.tabPageLocomotives.Name = "tabPageLocomotives";
			this.tabPageLocomotives.Size = new System.Drawing.Size(288, 246);
			this.tabPageLocomotives.TabIndex = 1;
			this.tabPageLocomotives.Text = "Locomotives";
			// 
			// panelLocoImages
			// 
			this.panelLocoImages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panelLocoImages.Location = new System.Drawing.Point(8, 6);
			this.panelLocoImages.Name = "panelLocoImages";
			this.panelLocoImages.Size = new System.Drawing.Size(272, 42);
			this.panelLocoImages.TabIndex = 5;
			this.panelLocoImages.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelLocoImages_MouseDown);
			this.panelLocoImages.Paint += new System.Windows.Forms.PaintEventHandler(this.panelLocoImages_Paint);
			// 
			// buttonLocoMoveDown
			// 
			this.buttonLocoMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonLocoMoveDown.ImageIndex = 0;
			this.buttonLocoMoveDown.ImageList = this.imageListButttons;
			this.buttonLocoMoveDown.Location = new System.Drawing.Point(222, 222);
			this.buttonLocoMoveDown.Name = "buttonLocoMoveDown";
			this.buttonLocoMoveDown.Size = new System.Drawing.Size(29, 21);
			this.buttonLocoMoveDown.TabIndex = 4;
			this.buttonLocoMoveDown.Click += new System.EventHandler(this.buttonLocoMoveDown_Click);
			// 
			// imageListButttons
			// 
			this.imageListButttons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListButttons.ImageStream")));
			this.imageListButttons.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListButttons.Images.SetKeyName(0, "");
			this.imageListButttons.Images.SetKeyName(1, "");
			// 
			// buttonLocoAdd
			// 
			this.buttonLocoAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonLocoAdd.Location = new System.Drawing.Point(8, 222);
			this.buttonLocoAdd.Name = "buttonLocoAdd";
			this.buttonLocoAdd.Size = new System.Drawing.Size(64, 21);
			this.buttonLocoAdd.TabIndex = 1;
			this.buttonLocoAdd.Text = "&Add...";
			this.buttonLocoAdd.Click += new System.EventHandler(this.buttonLocoAdd_Click);
			// 
			// listViewLocomotives
			// 
			this.listViewLocomotives.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listViewLocomotives.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderLocoName,
            this.columnAddress,
            this.columnHeaderLocoOrientation,
            this.columnHeaderLocoType});
			this.listViewLocomotives.FullRowSelect = true;
			this.listViewLocomotives.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewLocomotives.HideSelection = false;
			this.listViewLocomotives.Location = new System.Drawing.Point(8, 56);
			this.listViewLocomotives.MultiSelect = false;
			this.listViewLocomotives.Name = "listViewLocomotives";
			this.listViewLocomotives.Size = new System.Drawing.Size(272, 162);
			this.listViewLocomotives.TabIndex = 0;
			this.listViewLocomotives.UseCompatibleStateImageBehavior = false;
			this.listViewLocomotives.View = System.Windows.Forms.View.Details;
			this.listViewLocomotives.SelectedIndexChanged += new System.EventHandler(this.updateControls);
			// 
			// columnHeaderLocoName
			// 
			this.columnHeaderLocoName.Text = "Name";
			this.columnHeaderLocoName.Width = 78;
			// 
			// columnAddress
			// 
			this.columnAddress.Text = "Address";
			this.columnAddress.Width = 51;
			// 
			// columnHeaderLocoOrientation
			// 
			this.columnHeaderLocoOrientation.Text = "Orientation";
			this.columnHeaderLocoOrientation.Width = 65;
			// 
			// columnHeaderLocoType
			// 
			this.columnHeaderLocoType.Text = "Type";
			this.columnHeaderLocoType.Width = 73;
			// 
			// buttonLocoRemove
			// 
			this.buttonLocoRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonLocoRemove.Location = new System.Drawing.Point(80, 222);
			this.buttonLocoRemove.Name = "buttonLocoRemove";
			this.buttonLocoRemove.Size = new System.Drawing.Size(64, 21);
			this.buttonLocoRemove.TabIndex = 2;
			this.buttonLocoRemove.Text = "&Remove";
			this.buttonLocoRemove.Click += new System.EventHandler(this.buttonLocoRemove_Click);
			// 
			// buttonLocoEdit
			// 
			this.buttonLocoEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonLocoEdit.Location = new System.Drawing.Point(152, 222);
			this.buttonLocoEdit.Name = "buttonLocoEdit";
			this.buttonLocoEdit.Size = new System.Drawing.Size(64, 21);
			this.buttonLocoEdit.TabIndex = 3;
			this.buttonLocoEdit.Text = "&Edit...";
			this.buttonLocoEdit.Click += new System.EventHandler(this.buttonLocoEdit_Click);
			// 
			// buttonLocoMoveUp
			// 
			this.buttonLocoMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonLocoMoveUp.ImageIndex = 1;
			this.buttonLocoMoveUp.ImageList = this.imageListButttons;
			this.buttonLocoMoveUp.Location = new System.Drawing.Point(251, 222);
			this.buttonLocoMoveUp.Name = "buttonLocoMoveUp";
			this.buttonLocoMoveUp.Size = new System.Drawing.Size(29, 21);
			this.buttonLocoMoveUp.TabIndex = 4;
			this.buttonLocoMoveUp.Click += new System.EventHandler(this.buttonLocoMoveUp_Click);
			// 
			// tabPageDriver
			// 
			this.tabPageDriver.Controls.Add(this.drivingParameters);
			this.tabPageDriver.Location = new System.Drawing.Point(4, 22);
			this.tabPageDriver.Name = "tabPageDriver";
			this.tabPageDriver.Size = new System.Drawing.Size(288, 246);
			this.tabPageDriver.TabIndex = 4;
			this.tabPageDriver.Text = "Driver";
			// 
			// tabPageAttributes
			// 
			this.tabPageAttributes.Controls.Add(this.attributesEditor);
			this.tabPageAttributes.Location = new System.Drawing.Point(4, 22);
			this.tabPageAttributes.Name = "tabPageAttributes";
			this.tabPageAttributes.Size = new System.Drawing.Size(288, 246);
			this.tabPageAttributes.TabIndex = 3;
			this.tabPageAttributes.Text = "Attributes";
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.Location = new System.Drawing.Point(111, 280);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 21);
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(191, 280);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 21);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// contextMenuEditLocomotive
			// 
			this.contextMenuEditLocomotive.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemLocoOrientation,
            this.menuItemEditLocoDefinition});
			// 
			// menuItemLocoOrientation
			// 
			this.menuItemLocoOrientation.Index = 0;
			this.menuItemLocoOrientation.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemLocoOrientationForward,
            this.menuItemLocoOrientationBackward});
			this.menuItemLocoOrientation.Text = "Orientation";
			// 
			// menuItemLocoOrientationForward
			// 
			this.menuItemLocoOrientationForward.Index = 0;
			this.menuItemLocoOrientationForward.Text = "Forward";
			this.menuItemLocoOrientationForward.Click += new System.EventHandler(this.menuItemLocoOrientationForward_Click);
			// 
			// menuItemLocoOrientationBackward
			// 
			this.menuItemLocoOrientationBackward.Index = 1;
			this.menuItemLocoOrientationBackward.Text = "Backward";
			this.menuItemLocoOrientationBackward.Click += new System.EventHandler(this.menuItemLocoOrientationBackward_Click);
			// 
			// menuItemEditLocoDefinition
			// 
			this.menuItemEditLocoDefinition.Index = 1;
			this.menuItemEditLocoDefinition.Text = "Definition...";
			this.menuItemEditLocoDefinition.Click += new System.EventHandler(this.menuItemEditLocoDefinition_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.trainLengthDiagram);
			this.groupBox1.Location = new System.Drawing.Point(8, 48);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(272, 78);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Train Length:";
			// 
			// trainLengthDiagram
			// 
			this.trainLengthDiagram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.trainLengthDiagram.Comparison = LayoutManager.Model.TrainLengthComparison.None;
			this.trainLengthDiagram.Length = LayoutManager.Model.TrainLength.Standard;
			this.trainLengthDiagram.Location = new System.Drawing.Point(45, 19);
			this.trainLengthDiagram.Name = "trainLengthDiagram";
			this.trainLengthDiagram.Size = new System.Drawing.Size(180, 52);
			this.trainLengthDiagram.TabIndex = 9;
			// 
			// trainDriverComboBox
			// 
			this.trainDriverComboBox.Location = new System.Drawing.Point(8, 220);
			this.trainDriverComboBox.Name = "trainDriverComboBox";
			this.trainDriverComboBox.Size = new System.Drawing.Size(272, 24);
			this.trainDriverComboBox.TabIndex = 8;
			this.trainDriverComboBox.Train = null;
			// 
			// drivingParameters
			// 
			this.drivingParameters.Dock = System.Windows.Forms.DockStyle.Fill;
			this.drivingParameters.Element = null;
			this.drivingParameters.Location = new System.Drawing.Point(0, 0);
			this.drivingParameters.Name = "drivingParameters";
			this.drivingParameters.Size = new System.Drawing.Size(288, 246);
			this.drivingParameters.TabIndex = 0;
			// 
			// attributesEditor
			// 
			this.attributesEditor.AttributesOwner = null;
			this.attributesEditor.AttributesSource = null;
			this.attributesEditor.Location = new System.Drawing.Point(8, 8);
			this.attributesEditor.Name = "attributesEditor";
			this.attributesEditor.Size = new System.Drawing.Size(272, 224);
			this.attributesEditor.TabIndex = 0;
			this.attributesEditor.ViewOnly = false;
			// 
			// TrainProperties
			// 
			this.ClientSize = new System.Drawing.Size(292, 310);
			this.ControlBox = false;
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.buttonCancel);
			this.MinimumSize = new System.Drawing.Size(300, 300);
			this.Name = "TrainProperties";
			this.ShowInTaskbar = false;
			this.Text = "Train Properties";
			this.Closed += new System.EventHandler(this.TrainProperties_Closed);
			this.tabControl1.ResumeLayout(false);
			this.tabPageGeneral.ResumeLayout(false);
			this.tabPageGeneral.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.tabPageLocomotives.ResumeLayout(false);
			this.tabPageDriver.ResumeLayout(false);
			this.tabPageAttributes.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		[LayoutEventDef("train-configuration-changed", Role=LayoutEventRole.Notification, SenderType=typeof(TrainStateInfo), InfoType=typeof(string))]
		private void buttonOK_Click(object sender, System.EventArgs e) {
			// Validate

			if(textBoxName.Text.Trim() == "") {
				tabControl1.SelectedTab = tabPageGeneral;
				MessageBox.Show(this, "Train name cannot be empty", "Missing value", MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBoxName.Focus();
				return;
			}

			if(!trainDriverComboBox.ValidateInput()) {
				tabControl1.SelectedTab = tabPageGeneral;
				trainDriverComboBox.Focus();
			}

			if(!drivingParameters.ValidateValues())
				return;

			// Commit

			if(textBoxName.Text != train.Name)
				train.Name = textBoxName.Text;

			train.Length = trainLengthDiagram.Length;
			train.LastCarTriggerBlockEdge = checkBoxMagnetOnLastCar.Checked;
			train.LastCarDetectedByOccupancyBlock = checkBoxLastCarDetected.Checked;

			trainDriverComboBox.Commit();
			drivingParameters.Commit();
			attributesEditor.Commit();

			EventManager.Event(new LayoutEvent(train, "train-configuration-changed", null, train.Name));
			train.RefreshSpeedLimit();
			train.Redraw();

			DialogResult = DialogResult.OK;
		}

		private void TrainProperties_Closed(object sender, System.EventArgs e) {
			EventManager.Subscriptions.RemoveObjectSubscriptions(this);
		}

		private void buttonCancel_Click(object sender, System.EventArgs e) {
			if(locomotiveEdited) {
				try {
					// Undo locomotive collection editing. Remove locomotives, and re-insert original list
					foreach(TrainLocomotiveInfo trainLocomotive in train.Locomotives)
						train.RemoveLocomotive(trainLocomotive);

					foreach(TrainLocomotiveCancelInfo trainLocoCancelInfo in locomotiveCancelList) {
						CanPlaceTrainResult result = train.AddLocomotive(new LocomotiveInfo(LayoutModel.LocomotiveCollection[trainLocoCancelInfo.LocomotiveID]), trainLocoCancelInfo.Orientation, null, false);

						if(result.Status != CanPlaceTrainStatus.CanPlaceTrain)
							throw new LayoutException(result.ToString());
					}
				} catch(LayoutException lex) {
					lex.Report();
				}
			}
		}

		private void buttonLocoEdit_Click(object sender, System.EventArgs e) {
			contextMenuEditLocomotive.Show(tabPageLocomotives, new Point(buttonLocoEdit.Left, buttonLocoEdit.Bottom));
		}

		private void setLocomotiveOrientation(LocomotiveOrientation orientation) {
			TrainLocomotiveItem	selected = ((TrainLocomotiveItem)listViewLocomotives.SelectedItems[0]);
			TrainLocomotiveInfo	selectedTrainLoco = selected.TrainLocomotive;

			// Verify that it is a valid setting
			foreach(TrainLocomotiveInfo trainLoco in train.Locomotives) {
				if(trainLoco.LocomotiveId != selectedTrainLoco.LocomotiveId) {
					if(trainLoco.Locomotive.AddressProvider.Unit == selectedTrainLoco.Locomotive.AddressProvider.Unit &&
						trainLoco.Orientation != orientation) {
						MessageBox.Show(this, "You cannot change locomotive orientation. This train already has another locomotive with the same address. " +
							"All locomotives with a given address must have the same orientation", "Cannot Change Orientation",
							MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
				}
			}

			selectedTrainLoco.Orientation = orientation;
			selected.Update();
			locomotiveEdited = true;
		}

		private void menuItemLocoOrientationForward_Click(object sender, System.EventArgs e) {
			setLocomotiveOrientation(LocomotiveOrientation.Forward);
		}

		private void menuItemLocoOrientationBackward_Click(object sender, System.EventArgs e) {
			setLocomotiveOrientation(LocomotiveOrientation.Backward);
		}

		private void menuItemEditLocoDefinition_Click(object sender, System.EventArgs e) {
			TrainLocomotiveItem	selected = ((TrainLocomotiveItem)listViewLocomotives.SelectedItems[0]);
			TrainLocomotiveInfo	trainLoco = selected.TrainLocomotive;

			EventManager.Event(new LayoutEvent(trainLoco.Locomotive, "edit-locomotive-properties"));
			locomotiveEdited = true;
			train.FlushCachedValues();
			selected.Update();
			updateControls();
		}

		private void buttonLocoRemove_Click(object sender, System.EventArgs e) {
			TrainLocomotiveItem	selected = ((TrainLocomotiveItem)listViewLocomotives.SelectedItems[0]);
			TrainLocomotiveInfo	trainLoco = selected.TrainLocomotive;

			train.RemoveLocomotive(trainLoco);
			listViewLocomotives.Items.Remove(selected);
			locomotiveEdited = true;
		}

		private void buttonLocoAdd_Click(object sender, System.EventArgs e) {
			Dialogs.AddLocomotiveToTrain	addLoco = new Dialogs.AddLocomotiveToTrain(train);

			if(addLoco.ShowDialog() == DialogResult.OK) {
				CanPlaceTrainResult result = train.AddLocomotive(addLoco.Locomotive, addLoco.Orientation, null, true);

				if(result.Status == CanPlaceTrainStatus.CanPlaceTrain) {
					TrainLocomotiveInfo trainLoco = result.TrainLocomotive;

					listViewLocomotives.Items.Add(new TrainLocomotiveItem(trainLoco));
					locomotiveEdited = true;
					updateControls();
				}
			}
		}

		private void buttonLocoMoveDown_Click(object sender, System.EventArgs e) {
			int					selectedIndex = listViewLocomotives.SelectedIndices[0];

			if(selectedIndex < listViewLocomotives.Items.Count - 1) {
				TrainLocomotiveItem	selected = ((TrainLocomotiveItem)listViewLocomotives.SelectedItems[0]);
				TrainLocomotiveInfo	trainLoco = selected.TrainLocomotive;

				XmlNode	nextElement = trainLoco.Element.NextSibling;
				trainLoco.Element.ParentNode.RemoveChild(trainLoco.Element);
				nextElement.ParentNode.InsertAfter(trainLoco.Element, nextElement);

				listViewLocomotives.Items.Remove(selected);
				listViewLocomotives.Items.Insert(selectedIndex+1, selected);
				locomotiveEdited = true;
			}
		}

		private void buttonLocoMoveUp_Click(object sender, System.EventArgs e) {
			int					selectedIndex = listViewLocomotives.SelectedIndices[0];

			if(selectedIndex > 0) {
				TrainLocomotiveItem	selected = ((TrainLocomotiveItem)listViewLocomotives.SelectedItems[0]);
				TrainLocomotiveInfo	trainLoco = selected.TrainLocomotive;

				XmlNode	previousElement = trainLoco.Element.PreviousSibling;
				trainLoco.Element.ParentNode.RemoveChild(trainLoco.Element);
				previousElement.ParentNode.InsertBefore(trainLoco.Element, previousElement);

				listViewLocomotives.Items.Remove(selected);
				listViewLocomotives.Items.Insert(selectedIndex-1, selected);
				locomotiveEdited = true;
			}
		}

		private int ImageWidth {
			get {

				return ((panelLocoImages.Height - 8) * 50) / 36;
			}
		}

		private void panelLocoImages_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			int						x = 2;
			int						w = ImageWidth;
			LocomotiveImagePainter	locoPainter = new LocomotiveImagePainter();

			locoPainter.FrameSize = new Size(w, panelLocoImages.Height - 8);

			foreach(TrainLocomotiveItem item in listViewLocomotives.Items) {
				TrainLocomotiveInfo	trainLoco = item.TrainLocomotive;

				locoPainter.Origin = new Point(x, 2);
				locoPainter.LocomotiveElement = trainLoco.Locomotive.Element;

				if(trainLoco.Orientation == LocomotiveOrientation.Backward)
					locoPainter.FlipImage = true;
				else
					locoPainter.FlipImage = false;

				if(item.Selected)
					locoPainter.FramePen = new Pen(Color.Red, 2.0F);
				else
					locoPainter.FramePen = new Pen(Color.Black);

				locoPainter.Draw(e.Graphics);
				x += w + 4;
			}

			locoPainter.Dispose();
		}

		private void panelLocoImages_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			int		iImage = (e.X - 2) / ImageWidth;

			if(iImage < listViewLocomotives.Items.Count)
				listViewLocomotives.Items[iImage].Selected = true;
		}

		class TrainLocomotiveItem : ListViewItem {
			TrainLocomotiveInfo	trainLocomotive;

			public TrainLocomotiveItem(TrainLocomotiveInfo trainLocomotive) {
				LocomotiveInfo	loco = trainLocomotive.Locomotive;

				this.trainLocomotive = trainLocomotive;

				Text = loco.DisplayName;
				SubItems.Add(loco.AddressProvider.Unit.ToString());
				SubItems.Add(trainLocomotive.Orientation.ToString());
				SubItems.Add(loco.TypeName);
			}

			public void Update() {
				LocomotiveInfo	loco = trainLocomotive.Locomotive;

				SubItems[0].Text = loco.DisplayName;
				SubItems[1].Text = loco.AddressProvider.Unit.ToString();
				SubItems[2].Text = trainLocomotive.Orientation.ToString();
				SubItems[3].Text = loco.TypeName;
			}

			public TrainLocomotiveInfo TrainLocomotive {
				get {
					return trainLocomotive;
				}
			}
		}

		class TrainLocomotiveCancelInfo {
			public Guid						LocomotiveID;
			public LocomotiveOrientation	Orientation;

			public TrainLocomotiveCancelInfo(TrainLocomotiveInfo trainLocomotive) {
				this.LocomotiveID = trainLocomotive.LocomotiveId;
				this.Orientation = trainLocomotive.Orientation;
			}
		}
	}
}
