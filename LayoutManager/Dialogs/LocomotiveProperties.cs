using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;

namespace LayoutManager.Dialogs
{
	/// <summary>
	/// Summary description for LocomotiveType.
	/// </summary>
	public class LocomotiveProperties : LocomotiveBasePropertiesForm
	{
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.ColumnHeader columnHeaderFunctionNumber;
		private System.Windows.Forms.ColumnHeader columnHeaderFunctionDescription;
		private System.Windows.Forms.Button buttonFunctionAdd;
		private System.Windows.Forms.Button buttonFunctionEdit;
		private System.Windows.Forms.Button buttonFunctionRemove;
		private System.Windows.Forms.CheckBox checkBoxHasLights;
		private System.Windows.Forms.TabPage tabPageGeneral;
		private System.Windows.Forms.TabPage tabPageFunctions;
		private System.Windows.Forms.ListView listViewFunctions;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private System.Windows.Forms.Button buttonCopyFrom;
		private System.Windows.Forms.GroupBox groupBoxOrigin;
		private System.Windows.Forms.RadioButton radioButtonOriginUS;
		private System.Windows.Forms.RadioButton radioButtonOriginEurope;
		private System.Windows.Forms.GroupBox groupBoxKind;
		private System.Windows.Forms.RadioButton radioButtonKindSoundUnit;
		private System.Windows.Forms.RadioButton radioButtonKindElectric;
		private System.Windows.Forms.RadioButton radioButtonKindDiesel;
		private System.Windows.Forms.RadioButton radioButtonKindSteam;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button buttonImageClear;
		private System.Windows.Forms.Button buttonImageSet;
		private System.Windows.Forms.PictureBox pictureBoxImage;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.LinkLabel linkLabelTypeName;
		private System.Windows.Forms.Button buttonSelectType;
		private System.Windows.Forms.CheckBox checkBoxLinkedToType;
		private System.Windows.Forms.ComboBox comboBoxStore;
		private System.Windows.Forms.ColumnHeader columnHeaderName;
		private System.Windows.Forms.ColumnHeader columnHeaderType;
		private LayoutManager.CommonUI.Controls.LengthInput lengthInput;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox checkBoxTriggerTrackContact;
		private System.Windows.Forms.TextBox textBoxCollectionID;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TabPage tabPageProperties;
		private System.Windows.Forms.TabPage tabPageAttributes;
		private LayoutManager.CommonUI.Controls.AttributesEditor attributesEditor;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBoxSpeedLimit;
		private Label label6;
		private CommonUI.Controls.TrackGuageSelector trackGuageSelector;
		private TabPage tabPageDecoder;
		private TabPage tabPageLog;
		private ComboBox comboBoxDecoderType;
		private Label label7;
		private System.Windows.Forms.Label label3;

		private void EndOfDesignerVariables() { }

		LocomotiveInfo			inLoco;
		LocomotiveInfo			loco;

		public LocomotiveProperties(LocomotiveInfo inLoco) : base() {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.inLoco = inLoco;

			// Make a copy for editing
			loco = new LocomotiveInfo((XmlElement)inLoco.Element.CloneNode(true));

			textBoxName.Text = loco.Name;

			lengthInput.Initialize();
			InitializeControls(loco.Element, LocomotiveCollection.Element["Stores"]);

			if(loco.TypeName != "")
				linkLabelTypeName.Text = loco.TypeName;
			else
				linkLabelTypeName.Enabled = false;

			checkBoxLinkedToType.Checked = loco.LinkedToType;
			checkBoxTriggerTrackContact.Checked = loco.CanTriggerTrackContact;

			textBoxCollectionID.Text = loco.CollectionId;

			UpdateButtons();
		}

		protected LocomotiveCollectionInfo LocomotiveCollection {
			get {
				return LayoutModel.LocomotiveCollection;
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPageGeneral = new System.Windows.Forms.TabPage();
			this.textBoxCollectionID = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.checkBoxTriggerTrackContact = new System.Windows.Forms.CheckBox();
			this.comboBoxStore = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.checkBoxLinkedToType = new System.Windows.Forms.CheckBox();
			this.linkLabelTypeName = new System.Windows.Forms.LinkLabel();
			this.buttonSelectType = new System.Windows.Forms.Button();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tabPageProperties = new System.Windows.Forms.TabPage();
			this.label6 = new System.Windows.Forms.Label();
			this.trackGuageSelector = new LayoutManager.CommonUI.Controls.TrackGuageSelector();
			this.textBoxSpeedLimit = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.lengthInput = new LayoutManager.CommonUI.Controls.LengthInput();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.buttonImageClear = new System.Windows.Forms.Button();
			this.buttonImageSet = new System.Windows.Forms.Button();
			this.pictureBoxImage = new System.Windows.Forms.PictureBox();
			this.groupBoxOrigin = new System.Windows.Forms.GroupBox();
			this.radioButtonOriginUS = new System.Windows.Forms.RadioButton();
			this.radioButtonOriginEurope = new System.Windows.Forms.RadioButton();
			this.groupBoxKind = new System.Windows.Forms.GroupBox();
			this.radioButtonKindSoundUnit = new System.Windows.Forms.RadioButton();
			this.radioButtonKindElectric = new System.Windows.Forms.RadioButton();
			this.radioButtonKindDiesel = new System.Windows.Forms.RadioButton();
			this.radioButtonKindSteam = new System.Windows.Forms.RadioButton();
			this.tabPageFunctions = new System.Windows.Forms.TabPage();
			this.buttonCopyFrom = new System.Windows.Forms.Button();
			this.checkBoxHasLights = new System.Windows.Forms.CheckBox();
			this.buttonFunctionAdd = new System.Windows.Forms.Button();
			this.listViewFunctions = new System.Windows.Forms.ListView();
			this.columnHeaderFunctionNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeaderType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeaderFunctionDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonFunctionEdit = new System.Windows.Forms.Button();
			this.buttonFunctionRemove = new System.Windows.Forms.Button();
			this.tabPageDecoder = new System.Windows.Forms.TabPage();
			this.label7 = new System.Windows.Forms.Label();
			this.comboBoxDecoderType = new System.Windows.Forms.ComboBox();
			this.tabPageAttributes = new System.Windows.Forms.TabPage();
			this.attributesEditor = new LayoutManager.CommonUI.Controls.AttributesEditor();
			this.tabPageLog = new System.Windows.Forms.TabPage();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.tabControl1.SuspendLayout();
			this.tabPageGeneral.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.tabPageProperties.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).BeginInit();
			this.groupBoxOrigin.SuspendLayout();
			this.groupBoxKind.SuspendLayout();
			this.tabPageFunctions.SuspendLayout();
			this.tabPageDecoder.SuspendLayout();
			this.tabPageAttributes.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPageGeneral);
			this.tabControl1.Controls.Add(this.tabPageProperties);
			this.tabControl1.Controls.Add(this.tabPageFunctions);
			this.tabControl1.Controls.Add(this.tabPageDecoder);
			this.tabControl1.Controls.Add(this.tabPageAttributes);
			this.tabControl1.Controls.Add(this.tabPageLog);
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(358, 232);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPageGeneral
			// 
			this.tabPageGeneral.Controls.Add(this.textBoxCollectionID);
			this.tabPageGeneral.Controls.Add(this.label2);
			this.tabPageGeneral.Controls.Add(this.checkBoxTriggerTrackContact);
			this.tabPageGeneral.Controls.Add(this.comboBoxStore);
			this.tabPageGeneral.Controls.Add(this.label3);
			this.tabPageGeneral.Controls.Add(this.groupBox2);
			this.tabPageGeneral.Controls.Add(this.textBoxName);
			this.tabPageGeneral.Controls.Add(this.label1);
			this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
			this.tabPageGeneral.Name = "tabPageGeneral";
			this.tabPageGeneral.Size = new System.Drawing.Size(350, 206);
			this.tabPageGeneral.TabIndex = 0;
			this.tabPageGeneral.Text = "General";
			// 
			// textBoxCollectionID
			// 
			this.textBoxCollectionID.Location = new System.Drawing.Point(234, 114);
			this.textBoxCollectionID.Name = "textBoxCollectionID";
			this.textBoxCollectionID.Size = new System.Drawing.Size(40, 20);
			this.textBoxCollectionID.TabIndex = 6;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(164, 113);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 20);
			this.label2.TabIndex = 5;
			this.label2.Text = "Collection ID:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkBoxTriggerTrackContact
			// 
			this.checkBoxTriggerTrackContact.Location = new System.Drawing.Point(8, 114);
			this.checkBoxTriggerTrackContact.Name = "checkBoxTriggerTrackContact";
			this.checkBoxTriggerTrackContact.Size = new System.Drawing.Size(136, 20);
			this.checkBoxTriggerTrackContact.TabIndex = 4;
			this.checkBoxTriggerTrackContact.Text = "Trigger track contact";
			// 
			// comboBoxStore
			// 
			this.comboBoxStore.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxStore.Location = new System.Drawing.Point(96, 178);
			this.comboBoxStore.Name = "comboBoxStore";
			this.comboBoxStore.Size = new System.Drawing.Size(176, 21);
			this.comboBoxStore.TabIndex = 8;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 181);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 15);
			this.label3.TabIndex = 7;
			this.label3.Text = "Saved in a list:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.checkBoxLinkedToType);
			this.groupBox2.Controls.Add(this.linkLabelTypeName);
			this.groupBox2.Controls.Add(this.buttonSelectType);
			this.groupBox2.Location = new System.Drawing.Point(8, 32);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(330, 76);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Based on locomotve type:";
			// 
			// checkBoxLinkedToType
			// 
			this.checkBoxLinkedToType.Location = new System.Drawing.Point(12, 40);
			this.checkBoxLinkedToType.Name = "checkBoxLinkedToType";
			this.checkBoxLinkedToType.Size = new System.Drawing.Size(312, 32);
			this.checkBoxLinkedToType.TabIndex = 2;
			this.checkBoxLinkedToType.Text = "Automatic update locomotive definition when locomotive type definition changes";
			// 
			// linkLabelTypeName
			// 
			this.linkLabelTypeName.Location = new System.Drawing.Point(64, 16);
			this.linkLabelTypeName.Name = "linkLabelTypeName";
			this.linkLabelTypeName.Size = new System.Drawing.Size(200, 23);
			this.linkLabelTypeName.TabIndex = 1;
			this.linkLabelTypeName.TabStop = true;
			this.linkLabelTypeName.Text = "No type selected";
			this.linkLabelTypeName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// buttonSelectType
			// 
			this.buttonSelectType.Location = new System.Drawing.Point(8, 16);
			this.buttonSelectType.Name = "buttonSelectType";
			this.buttonSelectType.Size = new System.Drawing.Size(48, 23);
			this.buttonSelectType.TabIndex = 0;
			this.buttonSelectType.Text = "&Select";
			this.buttonSelectType.Click += new System.EventHandler(this.buttonSelectType_Click);
			// 
			// textBoxName
			// 
			this.textBoxName.Location = new System.Drawing.Point(64, 7);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(274, 20);
			this.textBoxName.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tabPageProperties
			// 
			this.tabPageProperties.Controls.Add(this.label6);
			this.tabPageProperties.Controls.Add(this.trackGuageSelector);
			this.tabPageProperties.Controls.Add(this.textBoxSpeedLimit);
			this.tabPageProperties.Controls.Add(this.label5);
			this.tabPageProperties.Controls.Add(this.lengthInput);
			this.tabPageProperties.Controls.Add(this.label4);
			this.tabPageProperties.Controls.Add(this.groupBox1);
			this.tabPageProperties.Controls.Add(this.groupBoxOrigin);
			this.tabPageProperties.Controls.Add(this.groupBoxKind);
			this.tabPageProperties.Location = new System.Drawing.Point(4, 22);
			this.tabPageProperties.Name = "tabPageProperties";
			this.tabPageProperties.Size = new System.Drawing.Size(350, 206);
			this.tabPageProperties.TabIndex = 2;
			this.tabPageProperties.Text = "Properties";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(105, 131);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(58, 16);
			this.label6.TabIndex = 22;
			this.label6.Text = "Guage:";
			// 
			// trackGuageSelector
			// 
			this.trackGuageSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.trackGuageSelector.FormattingEnabled = true;
			this.trackGuageSelector.IncludeGuageSet = false;
			this.trackGuageSelector.Location = new System.Drawing.Point(175, 128);
			this.trackGuageSelector.Name = "trackGuageSelector";
			this.trackGuageSelector.Size = new System.Drawing.Size(100, 21);
			this.trackGuageSelector.TabIndex = 21;
			this.trackGuageSelector.Value = LayoutManager.Model.TrackGauges.HO;
			// 
			// textBoxSpeedLimit
			// 
			this.textBoxSpeedLimit.Location = new System.Drawing.Point(176, 179);
			this.textBoxSpeedLimit.Name = "textBoxSpeedLimit";
			this.textBoxSpeedLimit.Size = new System.Drawing.Size(40, 20);
			this.textBoxSpeedLimit.TabIndex = 20;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(104, 179);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(66, 14);
			this.label5.TabIndex = 19;
			this.label5.Text = "Speed limit:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lengthInput
			// 
			this.lengthInput.IsEmpty = false;
			this.lengthInput.Location = new System.Drawing.Point(169, 153);
			this.lengthInput.Name = "lengthInput";
			this.lengthInput.NeutralValue = 0D;
			this.lengthInput.ReadOnly = false;
			this.lengthInput.Size = new System.Drawing.Size(97, 24);
			this.lengthInput.TabIndex = 18;
			this.lengthInput.UnitValue = 0D;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(104, 157);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(40, 16);
			this.label4.TabIndex = 17;
			this.label4.Text = "Length:";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.buttonImageClear);
			this.groupBox1.Controls.Add(this.buttonImageSet);
			this.groupBox1.Controls.Add(this.pictureBoxImage);
			this.groupBox1.Location = new System.Drawing.Point(112, 20);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(120, 100);
			this.groupBox1.TabIndex = 15;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Picture:";
			// 
			// buttonImageClear
			// 
			this.buttonImageClear.Location = new System.Drawing.Point(60, 72);
			this.buttonImageClear.Name = "buttonImageClear";
			this.buttonImageClear.Size = new System.Drawing.Size(48, 19);
			this.buttonImageClear.TabIndex = 15;
			this.buttonImageClear.Text = "Clear";
			this.buttonImageClear.Click += new System.EventHandler(this.buttonImageClear_Click_1);
			// 
			// buttonImageSet
			// 
			this.buttonImageSet.Location = new System.Drawing.Point(8, 72);
			this.buttonImageSet.Name = "buttonImageSet";
			this.buttonImageSet.Size = new System.Drawing.Size(48, 19);
			this.buttonImageSet.TabIndex = 16;
			this.buttonImageSet.Text = "Set";
			this.buttonImageSet.Click += new System.EventHandler(this.buttonImageSet_Click_1);
			// 
			// pictureBoxImage
			// 
			this.pictureBoxImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBoxImage.Location = new System.Drawing.Point(8, 18);
			this.pictureBoxImage.Name = "pictureBoxImage";
			this.pictureBoxImage.Size = new System.Drawing.Size(100, 50);
			this.pictureBoxImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBoxImage.TabIndex = 14;
			this.pictureBoxImage.TabStop = false;
			// 
			// groupBoxOrigin
			// 
			this.groupBoxOrigin.Controls.Add(this.radioButtonOriginUS);
			this.groupBoxOrigin.Controls.Add(this.radioButtonOriginEurope);
			this.groupBoxOrigin.Location = new System.Drawing.Point(8, 122);
			this.groupBoxOrigin.Name = "groupBoxOrigin";
			this.groupBoxOrigin.Size = new System.Drawing.Size(88, 64);
			this.groupBoxOrigin.TabIndex = 5;
			this.groupBoxOrigin.TabStop = false;
			this.groupBoxOrigin.Text = "Origin:";
			// 
			// radioButtonOriginUS
			// 
			this.radioButtonOriginUS.Location = new System.Drawing.Point(16, 38);
			this.radioButtonOriginUS.Name = "radioButtonOriginUS";
			this.radioButtonOriginUS.Size = new System.Drawing.Size(64, 16);
			this.radioButtonOriginUS.TabIndex = 1;
			this.radioButtonOriginUS.Text = "US";
			// 
			// radioButtonOriginEurope
			// 
			this.radioButtonOriginEurope.Location = new System.Drawing.Point(16, 16);
			this.radioButtonOriginEurope.Name = "radioButtonOriginEurope";
			this.radioButtonOriginEurope.Size = new System.Drawing.Size(64, 16);
			this.radioButtonOriginEurope.TabIndex = 0;
			this.radioButtonOriginEurope.Text = "European";
			// 
			// groupBoxKind
			// 
			this.groupBoxKind.Controls.Add(this.radioButtonKindSoundUnit);
			this.groupBoxKind.Controls.Add(this.radioButtonKindElectric);
			this.groupBoxKind.Controls.Add(this.radioButtonKindDiesel);
			this.groupBoxKind.Controls.Add(this.radioButtonKindSteam);
			this.groupBoxKind.Location = new System.Drawing.Point(8, 20);
			this.groupBoxKind.Name = "groupBoxKind";
			this.groupBoxKind.Size = new System.Drawing.Size(88, 100);
			this.groupBoxKind.TabIndex = 4;
			this.groupBoxKind.TabStop = false;
			this.groupBoxKind.Text = "Type:";
			// 
			// radioButtonKindSoundUnit
			// 
			this.radioButtonKindSoundUnit.Location = new System.Drawing.Point(16, 76);
			this.radioButtonKindSoundUnit.Name = "radioButtonKindSoundUnit";
			this.radioButtonKindSoundUnit.Size = new System.Drawing.Size(64, 16);
			this.radioButtonKindSoundUnit.TabIndex = 3;
			this.radioButtonKindSoundUnit.Text = "Sound";
			// 
			// radioButtonKindElectric
			// 
			this.radioButtonKindElectric.Location = new System.Drawing.Point(16, 56);
			this.radioButtonKindElectric.Name = "radioButtonKindElectric";
			this.radioButtonKindElectric.Size = new System.Drawing.Size(64, 16);
			this.radioButtonKindElectric.TabIndex = 2;
			this.radioButtonKindElectric.Text = "Electric";
			// 
			// radioButtonKindDiesel
			// 
			this.radioButtonKindDiesel.Location = new System.Drawing.Point(16, 36);
			this.radioButtonKindDiesel.Name = "radioButtonKindDiesel";
			this.radioButtonKindDiesel.Size = new System.Drawing.Size(64, 16);
			this.radioButtonKindDiesel.TabIndex = 1;
			this.radioButtonKindDiesel.Text = "Diesel";
			// 
			// radioButtonKindSteam
			// 
			this.radioButtonKindSteam.Location = new System.Drawing.Point(16, 16);
			this.radioButtonKindSteam.Name = "radioButtonKindSteam";
			this.radioButtonKindSteam.Size = new System.Drawing.Size(64, 16);
			this.radioButtonKindSteam.TabIndex = 0;
			this.radioButtonKindSteam.Text = "Steam";
			// 
			// tabPageFunctions
			// 
			this.tabPageFunctions.Controls.Add(this.buttonCopyFrom);
			this.tabPageFunctions.Controls.Add(this.checkBoxHasLights);
			this.tabPageFunctions.Controls.Add(this.buttonFunctionAdd);
			this.tabPageFunctions.Controls.Add(this.listViewFunctions);
			this.tabPageFunctions.Controls.Add(this.buttonFunctionEdit);
			this.tabPageFunctions.Controls.Add(this.buttonFunctionRemove);
			this.tabPageFunctions.Location = new System.Drawing.Point(4, 22);
			this.tabPageFunctions.Name = "tabPageFunctions";
			this.tabPageFunctions.Size = new System.Drawing.Size(350, 206);
			this.tabPageFunctions.TabIndex = 1;
			this.tabPageFunctions.Text = "Functions";
			// 
			// buttonCopyFrom
			// 
			this.buttonCopyFrom.Location = new System.Drawing.Point(200, 176);
			this.buttonCopyFrom.Name = "buttonCopyFrom";
			this.buttonCopyFrom.Size = new System.Drawing.Size(75, 23);
			this.buttonCopyFrom.TabIndex = 5;
			this.buttonCopyFrom.Text = "&Copy from...";
			this.buttonCopyFrom.Click += new System.EventHandler(this.buttonCopyFrom_Click_1);
			// 
			// checkBoxHasLights
			// 
			this.checkBoxHasLights.Location = new System.Drawing.Point(8, 176);
			this.checkBoxHasLights.Name = "checkBoxHasLights";
			this.checkBoxHasLights.Size = new System.Drawing.Size(136, 24);
			this.checkBoxHasLights.TabIndex = 4;
			this.checkBoxHasLights.Text = "Locomotive has lights";
			// 
			// buttonFunctionAdd
			// 
			this.buttonFunctionAdd.Location = new System.Drawing.Point(8, 144);
			this.buttonFunctionAdd.Name = "buttonFunctionAdd";
			this.buttonFunctionAdd.Size = new System.Drawing.Size(72, 24);
			this.buttonFunctionAdd.TabIndex = 1;
			this.buttonFunctionAdd.Text = "&Add...";
			this.buttonFunctionAdd.Click += new System.EventHandler(this.buttonFunctionAdd_Click_1);
			// 
			// listViewFunctions
			// 
			this.listViewFunctions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderFunctionNumber,
            this.columnHeaderType,
            this.columnHeaderName,
            this.columnHeaderFunctionDescription});
			this.listViewFunctions.FullRowSelect = true;
			this.listViewFunctions.GridLines = true;
			this.listViewFunctions.HideSelection = false;
			this.listViewFunctions.Location = new System.Drawing.Point(8, 8);
			this.listViewFunctions.MultiSelect = false;
			this.listViewFunctions.Name = "listViewFunctions";
			this.listViewFunctions.Size = new System.Drawing.Size(264, 128);
			this.listViewFunctions.TabIndex = 0;
			this.listViewFunctions.UseCompatibleStateImageBehavior = false;
			this.listViewFunctions.View = System.Windows.Forms.View.Details;
			this.listViewFunctions.SelectedIndexChanged += new System.EventHandler(this.listViewFunctions_SelectedIndexChanged_1);
			// 
			// columnHeaderFunctionNumber
			// 
			this.columnHeaderFunctionNumber.Text = "#";
			this.columnHeaderFunctionNumber.Width = 30;
			// 
			// columnHeaderType
			// 
			this.columnHeaderType.Text = "Type";
			this.columnHeaderType.Width = 50;
			// 
			// columnHeaderName
			// 
			this.columnHeaderName.Text = "Name";
			this.columnHeaderName.Width = 70;
			// 
			// columnHeaderFunctionDescription
			// 
			this.columnHeaderFunctionDescription.Text = "Description";
			this.columnHeaderFunctionDescription.Width = 196;
			// 
			// buttonFunctionEdit
			// 
			this.buttonFunctionEdit.Location = new System.Drawing.Point(84, 144);
			this.buttonFunctionEdit.Name = "buttonFunctionEdit";
			this.buttonFunctionEdit.Size = new System.Drawing.Size(72, 24);
			this.buttonFunctionEdit.TabIndex = 2;
			this.buttonFunctionEdit.Text = "&Edit...";
			this.buttonFunctionEdit.Click += new System.EventHandler(this.buttonFunctionEdit_Click_1);
			// 
			// buttonFunctionRemove
			// 
			this.buttonFunctionRemove.Location = new System.Drawing.Point(160, 144);
			this.buttonFunctionRemove.Name = "buttonFunctionRemove";
			this.buttonFunctionRemove.Size = new System.Drawing.Size(72, 24);
			this.buttonFunctionRemove.TabIndex = 3;
			this.buttonFunctionRemove.Text = "&Remove...";
			this.buttonFunctionRemove.Click += new System.EventHandler(this.buttonFunctionRemove_Click_1);
			// 
			// tabPageDecoder
			// 
			this.tabPageDecoder.BackColor = System.Drawing.SystemColors.Control;
			this.tabPageDecoder.Controls.Add(this.label7);
			this.tabPageDecoder.Controls.Add(this.comboBoxDecoderType);
			this.tabPageDecoder.Location = new System.Drawing.Point(4, 22);
			this.tabPageDecoder.Name = "tabPageDecoder";
			this.tabPageDecoder.Size = new System.Drawing.Size(350, 206);
			this.tabPageDecoder.TabIndex = 4;
			this.tabPageDecoder.Text = "Decoder";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(8, 16);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(74, 13);
			this.label7.TabIndex = 3;
			this.label7.Text = "Decoder type:";
			// 
			// comboBoxDecoderType
			// 
			this.comboBoxDecoderType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxDecoderType.FormattingEnabled = true;
			this.comboBoxDecoderType.Location = new System.Drawing.Point(92, 13);
			this.comboBoxDecoderType.Name = "comboBoxDecoderType";
			this.comboBoxDecoderType.Size = new System.Drawing.Size(246, 21);
			this.comboBoxDecoderType.TabIndex = 2;
			// 
			// tabPageAttributes
			// 
			this.tabPageAttributes.Controls.Add(this.attributesEditor);
			this.tabPageAttributes.Location = new System.Drawing.Point(4, 22);
			this.tabPageAttributes.Name = "tabPageAttributes";
			this.tabPageAttributes.Size = new System.Drawing.Size(350, 206);
			this.tabPageAttributes.TabIndex = 3;
			this.tabPageAttributes.Text = "Attributes";
			// 
			// attributesEditor
			// 
			this.attributesEditor.AttributesOwner = null;
			this.attributesEditor.AttributesSource = null;
			this.attributesEditor.Location = new System.Drawing.Point(0, 8);
			this.attributesEditor.Name = "attributesEditor";
			this.attributesEditor.Size = new System.Drawing.Size(280, 200);
			this.attributesEditor.TabIndex = 0;
			this.attributesEditor.ViewOnly = false;
			// 
			// tabPageLog
			// 
			this.tabPageLog.BackColor = System.Drawing.SystemColors.Control;
			this.tabPageLog.Location = new System.Drawing.Point(4, 22);
			this.tabPageLog.Name = "tabPageLog";
			this.tabPageLog.Size = new System.Drawing.Size(350, 206);
			this.tabPageLog.TabIndex = 5;
			this.tabPageLog.Text = "Log";
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(267, 238);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			// 
			// buttonOk
			// 
			this.buttonOk.Location = new System.Drawing.Point(187, 238);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(75, 23);
			this.buttonOk.TabIndex = 1;
			this.buttonOk.Text = "OK";
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// LocomotiveProperties
			// 
			this.AcceptButton = this.buttonOk;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(354, 266);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.buttonOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "LocomotiveProperties";
			this.ShowInTaskbar = false;
			this.Text = "Locomotive Properties";
			this.tabControl1.ResumeLayout(false);
			this.tabPageGeneral.ResumeLayout(false);
			this.tabPageGeneral.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.tabPageProperties.ResumeLayout(false);
			this.tabPageProperties.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).EndInit();
			this.groupBoxOrigin.ResumeLayout(false);
			this.groupBoxKind.ResumeLayout(false);
			this.tabPageFunctions.ResumeLayout(false);
			this.tabPageDecoder.ResumeLayout(false);
			this.tabPageDecoder.PerformLayout();
			this.tabPageAttributes.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		[LayoutEventDef("locomotive-configuration-changed", Role=LayoutEventRole.Notification, SenderType=typeof(LocomotiveInfo))]
		private void buttonOk_Click(object sender, System.EventArgs e) {
			if(textBoxName.Text.Trim() == "") {
				tabControl1.SelectedTab = tabPageGeneral;
				MessageBox.Show(this, "You must enter a name", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBoxName.Focus();
				return;
			}

			if(comboBoxDecoderType.SelectedItem == null) {
				tabControl1.SelectedTab = tabPageDecoder;
				MessageBox.Show(this, "You must specify the type of decoder that is install in this locomotive", "Missing decoder type", MessageBoxButtons.OK, MessageBoxIcon.Error);
				comboBoxDecoderType.Focus();
				return;
			}

			if(!GetLocomotiveTypeFields())
				return;

			loco.DecoderTypeName = ((DecoderTypeItem)comboBoxDecoderType.SelectedItem).DecoderType.TypeName;

			loco.Name = textBoxName.Text;
			loco.CollectionId = textBoxCollectionID.Text;

			loco.LinkedToType = checkBoxLinkedToType.Checked;
			loco.CanTriggerTrackContact = checkBoxTriggerTrackContact.Checked;

			if(inLoco.Element.ParentNode != null) {
				// Replace the input with copy made for editing
				inLoco.Element.ParentNode.ReplaceChild(loco.Element, inLoco.Element);
			}

			inLoco.Element = loco.Element;

			EventManager.Event(new LayoutEvent(inLoco, "locomotive-configuration-changed"));

			DialogResult = DialogResult.OK;
		}

		private void buttonSelectType_Click(object sender, System.EventArgs e) {
			Dialogs.SelectLocomotiveType	selectType = new Dialogs.SelectLocomotiveType();

			if(selectType.ShowDialog(this) == DialogResult.OK) {
				if(selectType.SelectedLocomotiveType != null) {
					loco.UpdateFromLocomotiveType(selectType.SelectedLocomotiveType);
					SetLocomotiveTypeFields();
					linkLabelTypeName.Text = loco.TypeName;
					linkLabelTypeName.Enabled = true;
				}
			}

			selectType.Dispose();
		}

		private void buttonFunctionAdd_Click_1(object sender, EventArgs e) {
			buttonFunctionAdd_Click(sender, e);
		}

		private void buttonFunctionEdit_Click_1(object sender, EventArgs e) {
			buttonFunctionEdit_Click(sender, e);
		}

		private void buttonFunctionRemove_Click_1(object sender, EventArgs e) {
			buttonFunctionRemove_Click(sender, e);
		}

		private void buttonCopyFrom_Click_1(object sender, EventArgs e) {
			buttonCopyFrom_Click(sender, e);
		}

		private void buttonImageSet_Click_1(object sender, EventArgs e) {
			buttonImageSet_Click(sender, e);
		}

		private void buttonImageClear_Click_1(object sender, EventArgs e) {
			buttonImageSet_Click(sender, e);
		}

		private void listViewFunctions_SelectedIndexChanged_1(object sender, EventArgs e) {
			listViewFunctions_SelectedIndexChanged(sender, e);
		}
	}
}
