using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveType.
    /// </summary>
    public class LocomotiveTypeProperties : LocomotiveBasePropertiesForm
	{
		private TabControl tabControl1;
		private Button buttonCancel;
		private Button buttonOk;
		private Label label1;
		private TextBox textBoxName;
		private GroupBox groupBoxOrigin;
		private RadioButton radioButtonOriginEurope;
		private RadioButton radioButtonOriginUS;
		private Label label2;
		private Button buttonFunctionAdd;
		private Button buttonFunctionEdit;
		private Button buttonFunctionRemove;
		private CheckBox checkBoxHasLights;
		private TabPage tabPageGeneral;
		private TabPage tabPageFunctions;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;
		private GroupBox groupBoxKind;
		private RadioButton radioButtonKindElectric;
		private RadioButton radioButtonKindDiesel;
		private RadioButton radioButtonKindSteam;
		private ComboBox comboBoxStore;
		private Button buttonCopyFrom;
		private GroupBox groupBox1;
		private ListView listViewFunctions;
		private ColumnHeader columnHeaderFunctionNumber;
		private ColumnHeader columnHeaderType;
		private ColumnHeader columnHeaderName;
		private ColumnHeader columnHeaderFunctionDescription;
		private Label label3;
		private LayoutManager.CommonUI.Controls.LengthInput lengthInput;
		private TabPage tabPageAttributes;
		private LayoutManager.CommonUI.Controls.AttributesEditor attributesEditor;
		private Label label4;
		private TextBox textBoxSpeedLimit;
		private TabPage tabPageDecoder;
		private ComboBox comboBoxDecoderType;
		private Label label5;
		private CommonUI.Controls.TrackGuageSelector trackGuageSelector;
		private CheckBox checkBoxHasBuiltinDecoder;
        private RadioButton radioButtonKindSoundUnit;
        private LayoutManager.CommonUI.Controls.ImageGetter imageGetter;

		private void EndOfDesignerVariables() { }

		LocomotiveTypeInfo		inLocoType;
		LocomotiveTypeInfo		locoType;
		LocomotiveCatalogInfo	catalog;

		public LocomotiveTypeProperties(LocomotiveTypeInfo inLocoType) {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.inLocoType = inLocoType;
			this.catalog = Catalog;
			lengthInput.Initialize();

			// Make a copy for editing
			locoType = new LocomotiveTypeInfo((XmlElement)inLocoType.Element.CloneNode(true));

			InitializeControls(locoType.Element, catalog.Element["Stores"]);

			textBoxName.Text = locoType.TypeName;

			checkBoxHasBuiltinDecoder.Checked = locoType.DecoderTypeName != null;
			comboBoxDecoderType.Enabled = checkBoxHasBuiltinDecoder.Checked;

			UpdateButtons();
		}

		public LocomotiveTypeProperties() {
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
            this.label5 = new System.Windows.Forms.Label();
            this.trackGuageSelector = new LayoutManager.CommonUI.Controls.TrackGuageSelector();
            this.textBoxSpeedLimit = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lengthInput = new LayoutManager.CommonUI.Controls.LengthInput();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.imageGetter = new LayoutManager.CommonUI.Controls.ImageGetter();
            this.comboBoxStore = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBoxOrigin = new System.Windows.Forms.GroupBox();
            this.radioButtonOriginUS = new System.Windows.Forms.RadioButton();
            this.radioButtonOriginEurope = new System.Windows.Forms.RadioButton();
            this.groupBoxKind = new System.Windows.Forms.GroupBox();
            this.radioButtonKindSoundUnit = new System.Windows.Forms.RadioButton();
            this.radioButtonKindElectric = new System.Windows.Forms.RadioButton();
            this.radioButtonKindDiesel = new System.Windows.Forms.RadioButton();
            this.radioButtonKindSteam = new System.Windows.Forms.RadioButton();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageFunctions = new System.Windows.Forms.TabPage();
            this.listViewFunctions = new System.Windows.Forms.ListView();
            this.columnHeaderFunctionNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderFunctionDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonCopyFrom = new System.Windows.Forms.Button();
            this.checkBoxHasLights = new System.Windows.Forms.CheckBox();
            this.buttonFunctionAdd = new System.Windows.Forms.Button();
            this.buttonFunctionEdit = new System.Windows.Forms.Button();
            this.buttonFunctionRemove = new System.Windows.Forms.Button();
            this.tabPageDecoder = new System.Windows.Forms.TabPage();
            this.checkBoxHasBuiltinDecoder = new System.Windows.Forms.CheckBox();
            this.comboBoxDecoderType = new System.Windows.Forms.ComboBox();
            this.tabPageAttributes = new System.Windows.Forms.TabPage();
            this.attributesEditor = new LayoutManager.CommonUI.Controls.AttributesEditor();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
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
            this.tabControl1.Controls.Add(this.tabPageFunctions);
            this.tabControl1.Controls.Add(this.tabPageDecoder);
            this.tabControl1.Controls.Add(this.tabPageAttributes);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(329, 262);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.label5);
            this.tabPageGeneral.Controls.Add(this.trackGuageSelector);
            this.tabPageGeneral.Controls.Add(this.textBoxSpeedLimit);
            this.tabPageGeneral.Controls.Add(this.label4);
            this.tabPageGeneral.Controls.Add(this.lengthInput);
            this.tabPageGeneral.Controls.Add(this.label3);
            this.tabPageGeneral.Controls.Add(this.groupBox1);
            this.tabPageGeneral.Controls.Add(this.comboBoxStore);
            this.tabPageGeneral.Controls.Add(this.label2);
            this.tabPageGeneral.Controls.Add(this.groupBoxOrigin);
            this.tabPageGeneral.Controls.Add(this.groupBoxKind);
            this.tabPageGeneral.Controls.Add(this.textBoxName);
            this.tabPageGeneral.Controls.Add(this.label1);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Size = new System.Drawing.Size(321, 236);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(102, 141);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 16);
            this.label5.TabIndex = 20;
            this.label5.Text = "Guage:";
            // 
            // trackGuageSelector
            // 
            this.trackGuageSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.trackGuageSelector.FormattingEnabled = true;
            this.trackGuageSelector.IncludeGuageSet = false;
            this.trackGuageSelector.Location = new System.Drawing.Point(172, 138);
            this.trackGuageSelector.Name = "trackGuageSelector";
            this.trackGuageSelector.Size = new System.Drawing.Size(100, 21);
            this.trackGuageSelector.TabIndex = 19;
            this.trackGuageSelector.Value = LayoutManager.Model.TrackGauges.HO;
            // 
            // textBoxSpeedLimit
            // 
            this.textBoxSpeedLimit.Location = new System.Drawing.Point(172, 186);
            this.textBoxSpeedLimit.Name = "textBoxSpeedLimit";
            this.textBoxSpeedLimit.Size = new System.Drawing.Size(40, 20);
            this.textBoxSpeedLimit.TabIndex = 18;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(102, 189);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 14);
            this.label4.TabIndex = 17;
            this.label4.Text = "Speed limit:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lengthInput
            // 
            this.lengthInput.IsEmpty = false;
            this.lengthInput.Location = new System.Drawing.Point(165, 161);
            this.lengthInput.Name = "lengthInput";
            this.lengthInput.NeutralValue = 0D;
            this.lengthInput.ReadOnly = false;
            this.lengthInput.Size = new System.Drawing.Size(95, 24);
            this.lengthInput.TabIndex = 16;
            this.lengthInput.UnitValue = 0D;
            this.lengthInput.Load += new System.EventHandler(this.lengthInput1_Load);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(101, 165);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 16);
            this.label3.TabIndex = 15;
            this.label3.Text = "Length:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.imageGetter);
            this.groupBox1.Location = new System.Drawing.Point(104, 34);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(206, 100);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Picture:";
            // 
            // imageGetter
            // 
            this.imageGetter.DefaultImage = null;
            this.imageGetter.Image = null;
            this.imageGetter.Location = new System.Drawing.Point(6, 16);
            this.imageGetter.Name = "imageGetter";
            this.imageGetter.RequiredImageSize = new System.Drawing.Size(46, 34);
            this.imageGetter.Size = new System.Drawing.Size(197, 78);
            this.imageGetter.TabIndex = 0;
            // 
            // comboBoxStore
            // 
            this.comboBoxStore.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStore.Location = new System.Drawing.Point(104, 211);
            this.comboBoxStore.Name = "comboBoxStore";
            this.comboBoxStore.Size = new System.Drawing.Size(168, 21);
            this.comboBoxStore.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(18, 213);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Saved in a list:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBoxOrigin
            // 
            this.groupBoxOrigin.Controls.Add(this.radioButtonOriginUS);
            this.groupBoxOrigin.Controls.Add(this.radioButtonOriginEurope);
            this.groupBoxOrigin.Location = new System.Drawing.Point(8, 139);
            this.groupBoxOrigin.Name = "groupBoxOrigin";
            this.groupBoxOrigin.Size = new System.Drawing.Size(88, 64);
            this.groupBoxOrigin.TabIndex = 3;
            this.groupBoxOrigin.TabStop = false;
            this.groupBoxOrigin.Text = "Origin:";
            // 
            // radioButtonOriginUS
            // 
            this.radioButtonOriginUS.Location = new System.Drawing.Point(16, 36);
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
            this.groupBoxKind.Location = new System.Drawing.Point(8, 34);
            this.groupBoxKind.Name = "groupBoxKind";
            this.groupBoxKind.Size = new System.Drawing.Size(88, 100);
            this.groupBoxKind.TabIndex = 2;
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
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(105, 10);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(205, 20);
            this.textBoxName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabPageFunctions
            // 
            this.tabPageFunctions.Controls.Add(this.listViewFunctions);
            this.tabPageFunctions.Controls.Add(this.buttonCopyFrom);
            this.tabPageFunctions.Controls.Add(this.checkBoxHasLights);
            this.tabPageFunctions.Controls.Add(this.buttonFunctionAdd);
            this.tabPageFunctions.Controls.Add(this.buttonFunctionEdit);
            this.tabPageFunctions.Controls.Add(this.buttonFunctionRemove);
            this.tabPageFunctions.Location = new System.Drawing.Point(4, 22);
            this.tabPageFunctions.Name = "tabPageFunctions";
            this.tabPageFunctions.Size = new System.Drawing.Size(284, 236);
            this.tabPageFunctions.TabIndex = 1;
            this.tabPageFunctions.Text = "Functions";
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
            this.listViewFunctions.Location = new System.Drawing.Point(9, 6);
            this.listViewFunctions.MultiSelect = false;
            this.listViewFunctions.Name = "listViewFunctions";
            this.listViewFunctions.Size = new System.Drawing.Size(264, 128);
            this.listViewFunctions.TabIndex = 6;
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
            this.tabPageDecoder.Controls.Add(this.checkBoxHasBuiltinDecoder);
            this.tabPageDecoder.Controls.Add(this.comboBoxDecoderType);
            this.tabPageDecoder.Location = new System.Drawing.Point(4, 22);
            this.tabPageDecoder.Name = "tabPageDecoder";
            this.tabPageDecoder.Size = new System.Drawing.Size(284, 236);
            this.tabPageDecoder.TabIndex = 3;
            this.tabPageDecoder.Text = "Decoder";
            // 
            // checkBoxHasBuiltinDecoder
            // 
            this.checkBoxHasBuiltinDecoder.AutoSize = true;
            this.checkBoxHasBuiltinDecoder.Location = new System.Drawing.Point(8, 13);
            this.checkBoxHasBuiltinDecoder.Name = "checkBoxHasBuiltinDecoder";
            this.checkBoxHasBuiltinDecoder.Size = new System.Drawing.Size(120, 17);
            this.checkBoxHasBuiltinDecoder.TabIndex = 4;
            this.checkBoxHasBuiltinDecoder.Text = "Has built in decoder";
            this.checkBoxHasBuiltinDecoder.UseVisualStyleBackColor = true;
            this.checkBoxHasBuiltinDecoder.CheckedChanged += new System.EventHandler(this.checkBoxHasBuiltinDecoder_CheckedChanged);
            // 
            // comboBoxDecoderType
            // 
            this.comboBoxDecoderType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDecoderType.FormattingEnabled = true;
            this.comboBoxDecoderType.Location = new System.Drawing.Point(26, 36);
            this.comboBoxDecoderType.Name = "comboBoxDecoderType";
            this.comboBoxDecoderType.Size = new System.Drawing.Size(246, 21);
            this.comboBoxDecoderType.TabIndex = 1;
            // 
            // tabPageAttributes
            // 
            this.tabPageAttributes.Controls.Add(this.attributesEditor);
            this.tabPageAttributes.Location = new System.Drawing.Point(4, 22);
            this.tabPageAttributes.Name = "tabPageAttributes";
            this.tabPageAttributes.Size = new System.Drawing.Size(284, 236);
            this.tabPageAttributes.TabIndex = 2;
            this.tabPageAttributes.Text = "Attributes";
            // 
            // attributesEditor
            // 
            this.attributesEditor.AttributesOwner = null;
            this.attributesEditor.AttributesSource = null;
            this.attributesEditor.Location = new System.Drawing.Point(0, 8);
            this.attributesEditor.Name = "attributesEditor";
            this.attributesEditor.Size = new System.Drawing.Size(280, 192);
            this.attributesEditor.TabIndex = 0;
            this.attributesEditor.ViewOnly = false;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(254, 270);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(174, 270);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 1;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // LocomotiveTypeProperties
            // 
            this.AcceptButton = this.buttonOk;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(334, 296);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "LocomotiveTypeProperties";
            this.ShowInTaskbar = false;
            this.Text = "Locomotive Type";
            this.tabControl1.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageGeneral.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBoxOrigin.ResumeLayout(false);
            this.groupBoxKind.ResumeLayout(false);
            this.tabPageFunctions.ResumeLayout(false);
            this.tabPageDecoder.ResumeLayout(false);
            this.tabPageDecoder.PerformLayout();
            this.tabPageAttributes.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void buttonOk_Click(object sender, System.EventArgs e) {
			if(textBoxName.Text.Trim() == "") {
				tabControl1.SelectedTab = tabPageGeneral;
				MessageBox.Show(this, "You must enter a name", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBoxName.Focus();
				return;
			}

			if(checkBoxHasBuiltinDecoder.Checked && comboBoxDecoderType.SelectedItem == null) {
				tabControl1.SelectedTab = tabPageDecoder;
				MessageBox.Show(this, "You have indicated that loco has built in decoder, but you have not specified its type", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				comboBoxDecoderType.Focus();
				return;
			}

			if(!GetLocomotiveTypeFields())
				return;

			locoType.TypeName = textBoxName.Text;

			if(checkBoxHasBuiltinDecoder.Checked)
				locoType.DecoderTypeName = ((DecoderTypeItem)comboBoxDecoderType.SelectedItem).DecoderType.TypeName;
			else
				locoType.DecoderTypeName = null;

			if(inLocoType.Element.ParentNode != null) {
				// Replace the input with copy made for editing
				inLocoType.Element.ParentNode.ReplaceChild(locoType.Element, inLocoType.Element);
			}

			inLocoType.Element = locoType.Element;

			EventManager.Event(new LayoutEvent(locoType.Element, "locomotive-type-updated", null, false));
			DialogResult = DialogResult.OK;
		}

		private void lengthInput1_Load(object sender, System.EventArgs e) {
		
		}

		private void checkBoxHasBuiltinDecoder_CheckedChanged(object sender, EventArgs e) {
			comboBoxDecoderType.Enabled = checkBoxHasBuiltinDecoder.Checked;
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

		private void listViewFunctions_SelectedIndexChanged_1(object sender, EventArgs e) {
			listViewFunctions_SelectedIndexChanged(sender, e);
		}


	}
}
