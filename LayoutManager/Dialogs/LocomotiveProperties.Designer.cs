using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveType.
    /// </summary>
    partial class LocomotiveProperties : LocomotiveBasePropertiesForm {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
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
            this.imageGetter = new LayoutManager.CommonUI.Controls.ImageGetter();
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
            this.columnHeaderFunctionNumber = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderType = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderName = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderFunctionDescription = new System.Windows.Forms.ColumnHeader();
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
            this.tabControl1.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(931, 571);
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
            this.tabPageGeneral.Location = new System.Drawing.Point(8, 46);
            this.tabPageGeneral.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Size = new System.Drawing.Size(915, 517);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            // 
            // textBoxCollectionID
            // 
            this.textBoxCollectionID.Location = new System.Drawing.Point(608, 281);
            this.textBoxCollectionID.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.textBoxCollectionID.Name = "textBoxCollectionID";
            this.textBoxCollectionID.Size = new System.Drawing.Size(98, 39);
            this.textBoxCollectionID.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(426, 278);
            this.label2.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(187, 49);
            this.label2.TabIndex = 5;
            this.label2.Text = "Collection ID:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkBoxTriggerTrackContact
            // 
            this.checkBoxTriggerTrackContact.Location = new System.Drawing.Point(21, 281);
            this.checkBoxTriggerTrackContact.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.checkBoxTriggerTrackContact.Name = "checkBoxTriggerTrackContact";
            this.checkBoxTriggerTrackContact.Size = new System.Drawing.Size(354, 49);
            this.checkBoxTriggerTrackContact.TabIndex = 4;
            this.checkBoxTriggerTrackContact.Text = "Trigger track contact";
            // 
            // comboBoxStore
            // 
            this.comboBoxStore.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStore.Location = new System.Drawing.Point(250, 438);
            this.comboBoxStore.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.comboBoxStore.Name = "comboBoxStore";
            this.comboBoxStore.Size = new System.Drawing.Size(451, 40);
            this.comboBoxStore.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(21, 446);
            this.label3.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(208, 37);
            this.label3.TabIndex = 7;
            this.label3.Text = "Saved in a list:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxLinkedToType);
            this.groupBox2.Controls.Add(this.linkLabelTypeName);
            this.groupBox2.Controls.Add(this.buttonSelectType);
            this.groupBox2.Location = new System.Drawing.Point(21, 79);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox2.Size = new System.Drawing.Size(858, 187);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Based on locomotve type:";
            // 
            // checkBoxLinkedToType
            // 
            this.checkBoxLinkedToType.Location = new System.Drawing.Point(31, 98);
            this.checkBoxLinkedToType.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.checkBoxLinkedToType.Name = "checkBoxLinkedToType";
            this.checkBoxLinkedToType.Size = new System.Drawing.Size(811, 79);
            this.checkBoxLinkedToType.TabIndex = 2;
            this.checkBoxLinkedToType.Text = "Automatic update locomotive definition when locomotive type definition changes";
            // 
            // linkLabelTypeName
            // 
            this.linkLabelTypeName.Location = new System.Drawing.Point(166, 39);
            this.linkLabelTypeName.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.linkLabelTypeName.Name = "linkLabelTypeName";
            this.linkLabelTypeName.Size = new System.Drawing.Size(520, 57);
            this.linkLabelTypeName.TabIndex = 1;
            this.linkLabelTypeName.TabStop = true;
            this.linkLabelTypeName.Text = "No type selected";
            this.linkLabelTypeName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonSelectType
            // 
            this.buttonSelectType.Location = new System.Drawing.Point(21, 39);
            this.buttonSelectType.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonSelectType.Name = "buttonSelectType";
            this.buttonSelectType.Size = new System.Drawing.Size(125, 57);
            this.buttonSelectType.TabIndex = 0;
            this.buttonSelectType.Text = "&Select";
            this.buttonSelectType.Click += new System.EventHandler(this.ButtonSelectType_Click);
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(166, 17);
            this.textBoxName.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(706, 39);
            this.textBoxName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(21, 22);
            this.label1.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 39);
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
            this.tabPageProperties.Location = new System.Drawing.Point(8, 46);
            this.tabPageProperties.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.tabPageProperties.Name = "tabPageProperties";
            this.tabPageProperties.Size = new System.Drawing.Size(915, 517);
            this.tabPageProperties.TabIndex = 2;
            this.tabPageProperties.Text = "Properties";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(273, 322);
            this.label6.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(151, 39);
            this.label6.TabIndex = 22;
            this.label6.Text = "Guage:";
            // 
            // trackGuageSelector
            // 
            this.trackGuageSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.trackGuageSelector.FormattingEnabled = true;
            this.trackGuageSelector.IncludeGuageSet = false;
            this.trackGuageSelector.Location = new System.Drawing.Point(455, 315);
            this.trackGuageSelector.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.trackGuageSelector.Name = "trackGuageSelector";
            this.trackGuageSelector.Size = new System.Drawing.Size(254, 40);
            this.trackGuageSelector.TabIndex = 21;
            this.trackGuageSelector.Value = LayoutManager.Model.TrackGauges.HO;
            this.trackGuageSelector.SelectedIndexChanged += new System.EventHandler(this.TrackGuageSelector_SelectedIndexChanged);
            // 
            // textBoxSpeedLimit
            // 
            this.textBoxSpeedLimit.Location = new System.Drawing.Point(458, 441);
            this.textBoxSpeedLimit.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.textBoxSpeedLimit.Name = "textBoxSpeedLimit";
            this.textBoxSpeedLimit.Size = new System.Drawing.Size(98, 39);
            this.textBoxSpeedLimit.TabIndex = 20;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(270, 441);
            this.label5.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(172, 34);
            this.label5.TabIndex = 19;
            this.label5.Text = "Speed limit:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lengthInput
            // 
            this.lengthInput.AutoSize = true;
            this.lengthInput.IsEmpty = false;
            this.lengthInput.Location = new System.Drawing.Point(439, 377);
            this.lengthInput.Margin = new System.Windows.Forms.Padding(21, 17, 21, 17);
            this.lengthInput.Name = "lengthInput";
            this.lengthInput.NeutralValue = 0D;
            this.lengthInput.ReadOnly = false;
            this.lengthInput.Size = new System.Drawing.Size(395, 145);
            this.lengthInput.TabIndex = 18;
            this.lengthInput.UnitValue = 0D;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(270, 386);
            this.label4.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 39);
            this.label4.TabIndex = 17;
            this.label4.Text = "Length:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.imageGetter);
            this.groupBox1.Location = new System.Drawing.Point(291, 20);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox1.Size = new System.Drawing.Size(588, 276);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Picture:";
            // 
            // imageGetter
            // 
            this.imageGetter.AutoSize = true;
            this.imageGetter.DefaultImage = null;
            this.imageGetter.Image = null;
            this.imageGetter.Location = new System.Drawing.Point(16, 44);
            this.imageGetter.Margin = new System.Windows.Forms.Padding(16, 17, 16, 17);
            this.imageGetter.Name = "imageGetter";
            this.imageGetter.RequiredImageSize = new System.Drawing.Size(46, 34);
            this.imageGetter.Size = new System.Drawing.Size(720, 231);
            this.imageGetter.TabIndex = 0;
            // 
            // groupBoxOrigin
            // 
            this.groupBoxOrigin.Controls.Add(this.radioButtonOriginUS);
            this.groupBoxOrigin.Controls.Add(this.radioButtonOriginEurope);
            this.groupBoxOrigin.Location = new System.Drawing.Point(21, 300);
            this.groupBoxOrigin.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBoxOrigin.Name = "groupBoxOrigin";
            this.groupBoxOrigin.Padding = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBoxOrigin.Size = new System.Drawing.Size(229, 158);
            this.groupBoxOrigin.TabIndex = 5;
            this.groupBoxOrigin.TabStop = false;
            this.groupBoxOrigin.Text = "Origin:";
            // 
            // radioButtonOriginUS
            // 
            this.radioButtonOriginUS.Location = new System.Drawing.Point(42, 94);
            this.radioButtonOriginUS.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonOriginUS.Name = "radioButtonOriginUS";
            this.radioButtonOriginUS.Size = new System.Drawing.Size(166, 39);
            this.radioButtonOriginUS.TabIndex = 1;
            this.radioButtonOriginUS.Text = "US";
            // 
            // radioButtonOriginEurope
            // 
            this.radioButtonOriginEurope.Location = new System.Drawing.Point(42, 39);
            this.radioButtonOriginEurope.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonOriginEurope.Name = "radioButtonOriginEurope";
            this.radioButtonOriginEurope.Size = new System.Drawing.Size(166, 39);
            this.radioButtonOriginEurope.TabIndex = 0;
            this.radioButtonOriginEurope.Text = "European";
            // 
            // groupBoxKind
            // 
            this.groupBoxKind.Controls.Add(this.radioButtonKindSoundUnit);
            this.groupBoxKind.Controls.Add(this.radioButtonKindElectric);
            this.groupBoxKind.Controls.Add(this.radioButtonKindDiesel);
            this.groupBoxKind.Controls.Add(this.radioButtonKindSteam);
            this.groupBoxKind.Location = new System.Drawing.Point(21, 20);
            this.groupBoxKind.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBoxKind.Name = "groupBoxKind";
            this.groupBoxKind.Padding = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBoxKind.Size = new System.Drawing.Size(229, 276);
            this.groupBoxKind.TabIndex = 4;
            this.groupBoxKind.TabStop = false;
            this.groupBoxKind.Text = "Type:";
            // 
            // radioButtonKindSoundUnit
            // 
            this.radioButtonKindSoundUnit.Location = new System.Drawing.Point(42, 187);
            this.radioButtonKindSoundUnit.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonKindSoundUnit.Name = "radioButtonKindSoundUnit";
            this.radioButtonKindSoundUnit.Size = new System.Drawing.Size(166, 39);
            this.radioButtonKindSoundUnit.TabIndex = 3;
            this.radioButtonKindSoundUnit.Text = "Sound";
            // 
            // radioButtonKindElectric
            // 
            this.radioButtonKindElectric.Location = new System.Drawing.Point(42, 138);
            this.radioButtonKindElectric.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonKindElectric.Name = "radioButtonKindElectric";
            this.radioButtonKindElectric.Size = new System.Drawing.Size(166, 39);
            this.radioButtonKindElectric.TabIndex = 2;
            this.radioButtonKindElectric.Text = "Electric";
            // 
            // radioButtonKindDiesel
            // 
            this.radioButtonKindDiesel.Location = new System.Drawing.Point(42, 89);
            this.radioButtonKindDiesel.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonKindDiesel.Name = "radioButtonKindDiesel";
            this.radioButtonKindDiesel.Size = new System.Drawing.Size(166, 39);
            this.radioButtonKindDiesel.TabIndex = 1;
            this.radioButtonKindDiesel.Text = "Diesel";
            // 
            // radioButtonKindSteam
            // 
            this.radioButtonKindSteam.Location = new System.Drawing.Point(42, 39);
            this.radioButtonKindSteam.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonKindSteam.Name = "radioButtonKindSteam";
            this.radioButtonKindSteam.Size = new System.Drawing.Size(166, 39);
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
            this.tabPageFunctions.Location = new System.Drawing.Point(8, 46);
            this.tabPageFunctions.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.tabPageFunctions.Name = "tabPageFunctions";
            this.tabPageFunctions.Size = new System.Drawing.Size(915, 517);
            this.tabPageFunctions.TabIndex = 1;
            this.tabPageFunctions.Text = "Functions";
            // 
            // buttonCopyFrom
            // 
            this.buttonCopyFrom.Location = new System.Drawing.Point(520, 433);
            this.buttonCopyFrom.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonCopyFrom.Name = "buttonCopyFrom";
            this.buttonCopyFrom.Size = new System.Drawing.Size(195, 57);
            this.buttonCopyFrom.TabIndex = 5;
            this.buttonCopyFrom.Text = "&Copy from...";
            this.buttonCopyFrom.Click += new System.EventHandler(this.ButtonCopyFrom_Click_1);
            // 
            // checkBoxHasLights
            // 
            this.checkBoxHasLights.Location = new System.Drawing.Point(21, 433);
            this.checkBoxHasLights.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.checkBoxHasLights.Name = "checkBoxHasLights";
            this.checkBoxHasLights.Size = new System.Drawing.Size(354, 59);
            this.checkBoxHasLights.TabIndex = 4;
            this.checkBoxHasLights.Text = "Locomotive has lights";
            // 
            // buttonFunctionAdd
            // 
            this.buttonFunctionAdd.Location = new System.Drawing.Point(21, 354);
            this.buttonFunctionAdd.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonFunctionAdd.Name = "buttonFunctionAdd";
            this.buttonFunctionAdd.Size = new System.Drawing.Size(187, 59);
            this.buttonFunctionAdd.TabIndex = 1;
            this.buttonFunctionAdd.Text = "&Add...";
            this.buttonFunctionAdd.Click += new System.EventHandler(this.ButtonFunctionAdd_Click_1);
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
            this.listViewFunctions.Location = new System.Drawing.Point(21, 20);
            this.listViewFunctions.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.listViewFunctions.MultiSelect = false;
            this.listViewFunctions.Name = "listViewFunctions";
            this.listViewFunctions.Size = new System.Drawing.Size(680, 309);
            this.listViewFunctions.TabIndex = 0;
            this.listViewFunctions.UseCompatibleStateImageBehavior = false;
            this.listViewFunctions.View = System.Windows.Forms.View.Details;
            this.listViewFunctions.SelectedIndexChanged += new System.EventHandler(this.ListViewFunctions_SelectedIndexChanged_1);
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
            this.buttonFunctionEdit.Location = new System.Drawing.Point(218, 354);
            this.buttonFunctionEdit.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonFunctionEdit.Name = "buttonFunctionEdit";
            this.buttonFunctionEdit.Size = new System.Drawing.Size(187, 59);
            this.buttonFunctionEdit.TabIndex = 2;
            this.buttonFunctionEdit.Text = "&Edit...";
            this.buttonFunctionEdit.Click += new System.EventHandler(this.ButtonFunctionEdit_Click_1);
            // 
            // buttonFunctionRemove
            // 
            this.buttonFunctionRemove.Location = new System.Drawing.Point(416, 354);
            this.buttonFunctionRemove.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonFunctionRemove.Name = "buttonFunctionRemove";
            this.buttonFunctionRemove.Size = new System.Drawing.Size(187, 59);
            this.buttonFunctionRemove.TabIndex = 3;
            this.buttonFunctionRemove.Text = "&Remove...";
            this.buttonFunctionRemove.Click += new System.EventHandler(this.ButtonFunctionRemove_Click_1);
            // 
            // tabPageDecoder
            // 
            this.tabPageDecoder.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageDecoder.Controls.Add(this.label7);
            this.tabPageDecoder.Controls.Add(this.comboBoxDecoderType);
            this.tabPageDecoder.Location = new System.Drawing.Point(8, 46);
            this.tabPageDecoder.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.tabPageDecoder.Name = "tabPageDecoder";
            this.tabPageDecoder.Size = new System.Drawing.Size(915, 517);
            this.tabPageDecoder.TabIndex = 4;
            this.tabPageDecoder.Text = "Decoder";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(21, 39);
            this.label7.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(163, 32);
            this.label7.TabIndex = 3;
            this.label7.Text = "Decoder type:";
            // 
            // comboBoxDecoderType
            // 
            this.comboBoxDecoderType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDecoderType.FormattingEnabled = true;
            this.comboBoxDecoderType.Location = new System.Drawing.Point(239, 32);
            this.comboBoxDecoderType.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.comboBoxDecoderType.Name = "comboBoxDecoderType";
            this.comboBoxDecoderType.Size = new System.Drawing.Size(633, 40);
            this.comboBoxDecoderType.TabIndex = 2;
            // 
            // tabPageAttributes
            // 
            this.tabPageAttributes.Controls.Add(this.attributesEditor);
            this.tabPageAttributes.Location = new System.Drawing.Point(8, 46);
            this.tabPageAttributes.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.tabPageAttributes.Name = "tabPageAttributes";
            this.tabPageAttributes.Size = new System.Drawing.Size(915, 517);
            this.tabPageAttributes.TabIndex = 3;
            this.tabPageAttributes.Text = "Attributes";
            // 
            // attributesEditor
            // 
            this.attributesEditor.AttributesSource = null;
            this.attributesEditor.AutoSize = true;
            this.attributesEditor.Location = new System.Drawing.Point(0, 20);
            this.attributesEditor.Margin = new System.Windows.Forms.Padding(21, 17, 21, 17);
            this.attributesEditor.Name = "attributesEditor";
            this.attributesEditor.Size = new System.Drawing.Size(1321, 492);
            this.attributesEditor.TabIndex = 0;
            this.attributesEditor.ViewOnly = false;
            // 
            // tabPageLog
            // 
            this.tabPageLog.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageLog.Location = new System.Drawing.Point(8, 46);
            this.tabPageLog.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.tabPageLog.Name = "tabPageLog";
            this.tabPageLog.Size = new System.Drawing.Size(915, 517);
            this.tabPageLog.TabIndex = 5;
            this.tabPageLog.Text = "Log";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(728, 585);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(195, 57);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(520, 585);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(195, 57);
            this.buttonOk.TabIndex = 1;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // LocomotiveProperties
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(988, 812);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
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
            this.groupBox1.PerformLayout();
            this.groupBoxOrigin.ResumeLayout(false);
            this.groupBoxKind.ResumeLayout(false);
            this.tabPageFunctions.ResumeLayout(false);
            this.tabPageDecoder.ResumeLayout(false);
            this.tabPageDecoder.PerformLayout();
            this.tabPageAttributes.ResumeLayout(false);
            this.tabPageAttributes.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private TabControl tabControl1;
        private Button buttonCancel;
        private Button buttonOk;
        private Label label1;
        private TextBox textBoxName;
        private ColumnHeader columnHeaderFunctionNumber;
        private ColumnHeader columnHeaderFunctionDescription;
        private Button buttonFunctionAdd;
        private Button buttonFunctionEdit;
        private Button buttonFunctionRemove;
        private CheckBox checkBoxHasLights;
        private TabPage tabPageGeneral;
        private TabPage tabPageFunctions;
        private ListView listViewFunctions;
        private Button buttonCopyFrom;
        private GroupBox groupBoxOrigin;
        private RadioButton radioButtonOriginUS;
        private RadioButton radioButtonOriginEurope;
        private GroupBox groupBoxKind;
        private RadioButton radioButtonKindSoundUnit;
        private RadioButton radioButtonKindElectric;
        private RadioButton radioButtonKindDiesel;
        private RadioButton radioButtonKindSteam;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private LinkLabel linkLabelTypeName;
        private Button buttonSelectType;
        private CheckBox checkBoxLinkedToType;
        private ComboBox comboBoxStore;
        private ColumnHeader columnHeaderName;
        private ColumnHeader columnHeaderType;
        private CommonUI.Controls.LengthInput lengthInput;
        private Label label4;
        private CheckBox checkBoxTriggerTrackContact;
        private TextBox textBoxCollectionID;
        private Label label2;
        private TabPage tabPageProperties;
        private TabPage tabPageAttributes;
        private CommonUI.Controls.AttributesEditor attributesEditor;
        private Label label5;
        private TextBox textBoxSpeedLimit;
        private Label label6;
        private CommonUI.Controls.TrackGuageSelector trackGuageSelector;
        private TabPage tabPageDecoder;
        private TabPage tabPageLog;
        private ComboBox comboBoxDecoderType;
        private Label label7;
        private Label label3;
        private CommonUI.Controls.ImageGetter imageGetter;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}

