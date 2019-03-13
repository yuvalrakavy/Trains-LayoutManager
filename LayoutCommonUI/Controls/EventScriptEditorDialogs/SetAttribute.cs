using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

#pragma warning disable IDE0051, IDE0060
namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for SetAttribute.
    /// </summary>
    public class SetAttribute : Form {
        private GroupBox groupBox1;
        private ComboBox comboBoxSymbol;
        private Label label1;
        private Label label2;
        private ComboBox comboBoxAttribute;
        private GroupBox groupBox2;
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenuType;
        private TextBox textBoxTextValue;
        private GroupBox groupBoxValueBoolean;
        private RadioButton radioButtonValueTrue;
        private RadioButton radioButtonValueFalse;
        private Button buttonOK;
        private Button buttonCancel;
        private LayoutManager.CommonUI.Controls.OperandValueOf operandValueOf;
        private LayoutManager.CommonUI.Controls.NumericValue numericValue;


        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private void endOfDesignerVariables() { }

        readonly XmlElement element;
        readonly IDictionary symbolNameToTypeMap = null;

        public SetAttribute(XmlElement element) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.element = element;

            symbolNameToTypeMap = new HybridDictionary();

            EventManager.Event(new LayoutEvent("add-context-symbols-and-types", this, symbolNameToTypeMap));

            comboBoxSymbol.Sorted = true;
            foreach (string symbolName in symbolNameToTypeMap.Keys)
                comboBoxSymbol.Items.Add(symbolName);

            if (element.HasAttribute("Symbol"))
                comboBoxSymbol.Text = element.GetAttribute("Symbol");

            if (element.HasAttribute("Attribute"))
                comboBoxAttribute.Text = element.GetAttribute("Attribute");

            string setTo = "Text";

            if (element.HasAttribute("SetTo"))
                setTo = element.GetAttribute("SetTo");

            switch (setTo) {

                case "Text":
                    linkMenuType.SelectedIndex = 0;
                    if (element.HasAttribute("Value"))
                        textBoxTextValue.Text = element.GetAttribute("Value");
                    break;

                case "Number":
                    linkMenuType.SelectedIndex = 1;
                    break;

                case "Boolean":
                    if (XmlConvert.ToBoolean(element.GetAttribute("Value")))
                        radioButtonValueTrue.Checked = true;
                    else
                        radioButtonValueFalse.Checked = true;

                    linkMenuType.SelectedIndex = 2;
                    break;

                case "ValueOf":
                    linkMenuType.SelectedIndex = 3;
                    break;

                case "Remove":
                    linkMenuType.SelectedIndex = 4;
                    break;
            }

            numericValue.Element = element;
            operandValueOf.Element = element;
            operandValueOf.Suffix = "To";
            operandValueOf.AllowedTypes = new Type[] { typeof(string), typeof(Enum), typeof(int) };
            operandValueOf.Initialize();

            updateControls();
        }

        private void updateControls() {

            switch (linkMenuType.SelectedIndex) {

                case 0:     // Text
                    textBoxTextValue.Visible = true;
                    numericValue.Visible = false;
                    groupBoxValueBoolean.Visible = false;
                    operandValueOf.Visible = false;
                    break;

                case 1:     // Number
                    SuspendLayout();
                    textBoxTextValue.Visible = false;
                    numericValue.Visible = true;
                    numericValue.Location = textBoxTextValue.Location;
                    groupBoxValueBoolean.Visible = false;
                    operandValueOf.Visible = false;
                    ResumeLayout();
                    break;

                case 2:     // Boolean
                    SuspendLayout();
                    textBoxTextValue.Visible = false;
                    numericValue.Visible = false;
                    groupBoxValueBoolean.Location = textBoxTextValue.Location;
                    groupBoxValueBoolean.Visible = true;
                    if (!radioButtonValueFalse.Checked && !radioButtonValueTrue.Checked)
                        radioButtonValueFalse.Checked = true;
                    operandValueOf.Visible = false;
                    ResumeLayout();
                    break;

                case 3:     // Value of
                    SuspendLayout();
                    textBoxTextValue.Visible = false;
                    numericValue.Visible = false;
                    groupBoxValueBoolean.Visible = false;
                    operandValueOf.Location = textBoxTextValue.Location;
                    operandValueOf.Visible = true;
                    ResumeLayout();
                    break;

                case 4:     // Remove
                    textBoxTextValue.Visible = false;
                    numericValue.Visible = false;
                    groupBoxValueBoolean.Visible = false;
                    operandValueOf.Visible = false;
                    break;

            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.groupBox1 = new GroupBox();
            this.label1 = new Label();
            this.comboBoxSymbol = new ComboBox();
            this.label2 = new Label();
            this.comboBoxAttribute = new ComboBox();
            this.groupBox2 = new GroupBox();
            this.operandValueOf = new LayoutManager.CommonUI.Controls.OperandValueOf();
            this.groupBoxValueBoolean = new GroupBox();
            this.radioButtonValueTrue = new RadioButton();
            this.radioButtonValueFalse = new RadioButton();
            this.textBoxTextValue = new TextBox();
            this.linkMenuType = new LayoutManager.CommonUI.Controls.LinkMenu();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.numericValue = new LayoutManager.CommonUI.Controls.NumericValue();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxValueBoolean.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comboBoxSymbol);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.comboBoxAttribute);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(216, 144);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Set:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(15, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Symbol:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxSymbol
            // 
            this.comboBoxSymbol.Location = new System.Drawing.Point(71, 24);
            this.comboBoxSymbol.Name = "comboBoxSymbol";
            this.comboBoxSymbol.Size = new System.Drawing.Size(139, 21);
            this.comboBoxSymbol.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(7, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "Attribute:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxAttribute
            // 
            this.comboBoxAttribute.Location = new System.Drawing.Point(71, 56);
            this.comboBoxAttribute.Name = "comboBoxAttribute";
            this.comboBoxAttribute.Size = new System.Drawing.Size(139, 21);
            this.comboBoxAttribute.TabIndex = 0;
            this.comboBoxAttribute.DropDown += new System.EventHandler(this.comboBoxAttribute_DropDown);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numericValue);
            this.groupBox2.Controls.Add(this.operandValueOf);
            this.groupBox2.Controls.Add(this.groupBoxValueBoolean);
            this.groupBox2.Controls.Add(this.textBoxTextValue);
            this.groupBox2.Controls.Add(this.linkMenuType);
            this.groupBox2.Location = new System.Drawing.Point(231, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(224, 144);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "To:";
            // 
            // operandValueOf
            // 
            this.operandValueOf.AllowedTypes = null;
            this.operandValueOf.DefaultAccess = "Property";
            this.operandValueOf.Element = null;
            this.operandValueOf.Location = new System.Drawing.Point(24, 36);
            this.operandValueOf.Name = "operandValueOf";
            this.operandValueOf.Size = new System.Drawing.Size(200, 72);
            this.operandValueOf.Suffix = "";
            this.operandValueOf.TabIndex = 4;
            // 
            // groupBoxValueBoolean
            // 
            this.groupBoxValueBoolean.Controls.Add(this.radioButtonValueTrue);
            this.groupBoxValueBoolean.Controls.Add(this.radioButtonValueFalse);
            this.groupBoxValueBoolean.Location = new System.Drawing.Point(8, 40);
            this.groupBoxValueBoolean.Name = "groupBoxValueBoolean";
            this.groupBoxValueBoolean.Size = new System.Drawing.Size(144, 72);
            this.groupBoxValueBoolean.TabIndex = 2;
            this.groupBoxValueBoolean.TabStop = false;
            // 
            // radioButtonValueTrue
            // 
            this.radioButtonValueTrue.Location = new System.Drawing.Point(8, 16);
            this.radioButtonValueTrue.Name = "radioButtonValueTrue";
            this.radioButtonValueTrue.TabIndex = 0;
            this.radioButtonValueTrue.Text = "True";
            // 
            // radioButtonValueFalse
            // 
            this.radioButtonValueFalse.Location = new System.Drawing.Point(8, 40);
            this.radioButtonValueFalse.Name = "radioButtonValueFalse";
            this.radioButtonValueFalse.TabIndex = 0;
            this.radioButtonValueFalse.Text = "False";
            // 
            // textBoxTextValue
            // 
            this.textBoxTextValue.Location = new System.Drawing.Point(8, 48);
            this.textBoxTextValue.Name = "textBoxTextValue";
            this.textBoxTextValue.Size = new System.Drawing.Size(152, 20);
            this.textBoxTextValue.TabIndex = 1;
            this.textBoxTextValue.Text = "";
            // 
            // linkMenuType
            // 
            this.linkMenuType.Location = new System.Drawing.Point(8, 24);
            this.linkMenuType.Name = "linkMenuType";
            this.linkMenuType.Options = new string[] {
                                                         "Text",
                                                         "Number",
                                                         "Boolean",
                                                         "Value of",
                                                         "Remove attribute"};
            this.linkMenuType.SelectedIndex = -1;
            this.linkMenuType.Size = new System.Drawing.Size(100, 16);
            this.linkMenuType.TabIndex = 0;
            this.linkMenuType.TabStop = true;
            this.linkMenuType.Text = "Text";
            this.linkMenuType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkMenuType.ValueChanged += new System.EventHandler(this.linkMenuType_ValueChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(304, 160);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(384, 160);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            // 
            // numericValue
            // 
            this.numericValue.Element = null;
            this.numericValue.Location = new System.Drawing.Point(24, 96);
            this.numericValue.Name = "numericValue";
            this.numericValue.Size = new System.Drawing.Size(200, 27);
            this.numericValue.TabIndex = 5;
            // 
            // SetAttribute
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(464, 190);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SetAttribute";
            this.ShowInTaskbar = false;
            this.Text = "Set Attribute";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBoxValueBoolean.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private void linkMenuType_ValueChanged(object sender, System.EventArgs e) {
            updateControls();
        }

        private void comboBoxAttribute_DropDown(object sender, System.EventArgs e) {
            comboBoxAttribute.Items.Clear();

            Type symbolType = (Type)symbolNameToTypeMap[comboBoxSymbol.Text];

            if (symbolType != null) {
                var attributesList = new List<AttributesInfo>();
                var attributesMap = new Dictionary<string, AttributeInfo>();

                EventManager.Event(new LayoutEvent("get-object-attributes", symbolType, attributesList));

                foreach (var attributes in attributesList) {
                    foreach (var attribute in attributes) {
                        if (!attributesMap.ContainsKey(attribute.Name)) {
                            attributesMap.Add(attribute.Name, attribute);
                            comboBoxAttribute.Items.Add(attribute.Name);
                        }
                    }
                }
            }
        }

        private void buttonOK_Click(object sender, System.EventArgs e) {
            if (comboBoxSymbol.Text.Trim() == "") {
                MessageBox.Show(this, "You did not provide a name of a symbol", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxSymbol.Focus();
                return;
            }

            if (comboBoxAttribute.Text.Trim() == "") {
                MessageBox.Show(this, "You did not provide a name of an attribute", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxAttribute.Focus();
                return;
            }

            if (linkMenuType.SelectedIndex == 1) {
                if (!numericValue.ValidateInput())
                    return;
            }

            element.SetAttribute("Symbol", comboBoxSymbol.Text);
            element.SetAttribute("Attribute", comboBoxAttribute.Text);

            switch (linkMenuType.SelectedIndex) {
                case 0:
                    element.SetAttribute("SetTo", "Text");
                    element.SetAttribute("Value", textBoxTextValue.Text);
                    break;

                case 1:
                    element.SetAttribute("SetTo", "Number");
                    numericValue.Commit();
                    break;

                case 2:
                    element.SetAttribute("SetTo", "Boolean");
                    element.SetAttribute("Value", XmlConvert.ToString(radioButtonValueTrue.Checked));
                    break;

                case 3:
                    element.SetAttribute("SetTo", "ValueOf");
                    operandValueOf.Commit();
                    break;

                case 4:
                    element.SetAttribute("SetTo", "Remove");
                    break;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
