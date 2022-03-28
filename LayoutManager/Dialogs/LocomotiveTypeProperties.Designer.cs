using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;

#pragma warning disable IDE0051, IDE0060
namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveType.
    /// </summary>
    partial class LocomotiveTypeProperties : LocomotiveBasePropertiesForm {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.tabControl1 = new TabControl();
            this.tabPageGeneral = new TabPage();
            this.label5 = new Label();
            this.trackGuageSelector = new CommonUI.Controls.TrackGuageSelector();
            this.textBoxSpeedLimit = new TextBox();
            this.label4 = new Label();
            this.lengthInput = new CommonUI.Controls.LengthInput();
            this.label3 = new Label();
            this.groupBox1 = new GroupBox();
            this.imageGetter = new CommonUI.Controls.ImageGetter();
            this.comboBoxStore = new ComboBox();
            this.label2 = new Label();
            this.groupBoxOrigin = new GroupBox();
            this.radioButtonOriginUS = new RadioButton();
            this.radioButtonOriginEurope = new RadioButton();
            this.groupBoxKind = new GroupBox();
            this.radioButtonKindSoundUnit = new RadioButton();
            this.radioButtonKindElectric = new RadioButton();
            this.radioButtonKindDiesel = new RadioButton();
            this.radioButtonKindSteam = new RadioButton();
            this.textBoxName = new TextBox();
            this.label1 = new Label();
            this.tabPageFunctions = new TabPage();
            this.listViewFunctions = new ListView();
            this.columnHeaderFunctionNumber = (ColumnHeader)new ColumnHeader();
            this.columnHeaderType = (ColumnHeader)new ColumnHeader();
            this.columnHeaderName = (ColumnHeader)new ColumnHeader();
            this.columnHeaderFunctionDescription = (ColumnHeader)new ColumnHeader();
            this.buttonCopyFrom = new Button();
            this.checkBoxHasLights = new CheckBox();
            this.buttonFunctionAdd = new Button();
            this.buttonFunctionEdit = new Button();
            this.buttonFunctionRemove = new Button();
            this.tabPageDecoder = new TabPage();
            this.checkBoxHasBuiltinDecoder = new CheckBox();
            this.comboBoxDecoderType = new ComboBox();
            this.tabPageAttributes = new TabPage();
            this.attributesEditor = new CommonUI.Controls.AttributesEditor();
            this.buttonCancel = new Button();
            this.buttonOk = new Button();
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
            this.tabControl1.Anchor = (AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
            | System.Windows.Forms.AnchorStyles.Left
            | System.Windows.Forms.AnchorStyles.Right);
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
            this.trackGuageSelector.SelectedIndexChanged += new EventHandler(this.TrackGuageSelector_SelectedIndexChanged);
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
            this.lengthInput.Load += new EventHandler(this.LengthInput1_Load);
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
            this.tabPageFunctions.Size = new System.Drawing.Size(321, 236);
            this.tabPageFunctions.TabIndex = 1;
            this.tabPageFunctions.Text = "Functions";
            // 
            // listViewFunctions
            // 
            this.listViewFunctions.Columns.AddRange(new ColumnHeader[] {
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
            this.listViewFunctions.SelectedIndexChanged += new EventHandler(this.ListViewFunctions_SelectedIndexChanged_1);
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
            this.buttonCopyFrom.Click += new EventHandler(this.ButtonCopyFrom_Click_1);
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
            this.buttonFunctionAdd.Click += new EventHandler(this.ButtonFunctionAdd_Click_1);
            // 
            // buttonFunctionEdit
            // 
            this.buttonFunctionEdit.Location = new System.Drawing.Point(84, 144);
            this.buttonFunctionEdit.Name = "buttonFunctionEdit";
            this.buttonFunctionEdit.Size = new System.Drawing.Size(72, 24);
            this.buttonFunctionEdit.TabIndex = 2;
            this.buttonFunctionEdit.Text = "&Edit...";
            this.buttonFunctionEdit.Click += new EventHandler(this.ButtonFunctionEdit_Click_1);
            // 
            // buttonFunctionRemove
            // 
            this.buttonFunctionRemove.Location = new System.Drawing.Point(160, 144);
            this.buttonFunctionRemove.Name = "buttonFunctionRemove";
            this.buttonFunctionRemove.Size = new System.Drawing.Size(72, 24);
            this.buttonFunctionRemove.TabIndex = 3;
            this.buttonFunctionRemove.Text = "&Remove...";
            this.buttonFunctionRemove.Click += new EventHandler(this.ButtonFunctionRemove_Click_1);
            // 
            // tabPageDecoder
            // 
            this.tabPageDecoder.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageDecoder.Controls.Add(this.checkBoxHasBuiltinDecoder);
            this.tabPageDecoder.Controls.Add(this.comboBoxDecoderType);
            this.tabPageDecoder.Location = new System.Drawing.Point(4, 22);
            this.tabPageDecoder.Name = "tabPageDecoder";
            this.tabPageDecoder.Size = new System.Drawing.Size(321, 236);
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
            this.checkBoxHasBuiltinDecoder.CheckedChanged += new EventHandler(this.CheckBoxHasBuiltinDecoder_CheckedChanged);
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
            this.tabPageAttributes.Size = new System.Drawing.Size(321, 236);
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
            this.buttonCancel.Anchor = (AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(254, 270);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = (AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonOk.Location = new System.Drawing.Point(174, 270);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 1;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += new EventHandler(this.ButtonOk_Click);
            // 
            // LocomotiveTypeProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.AcceptButton = this.buttonOk;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(400, 350);
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
        private CommonUI.Controls.LengthInput lengthInput;
        private TabPage tabPageAttributes;
        private CommonUI.Controls.AttributesEditor attributesEditor;
        private Label label4;
        private TextBox textBoxSpeedLimit;
        private TabPage tabPageDecoder;
        private ComboBox comboBoxDecoderType;
        private Label label5;
        private CommonUI.Controls.TrackGuageSelector trackGuageSelector;
        private CheckBox checkBoxHasBuiltinDecoder;
        private RadioButton radioButtonKindSoundUnit;
        private CommonUI.Controls.ImageGetter imageGetter;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}

