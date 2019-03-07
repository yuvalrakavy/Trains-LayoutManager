using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for GenerateEventArgument.
    /// </summary>
    public class GenerateEventArgument : System.Windows.Forms.UserControl {
        private RadioButton radioButtonNull;
        private RadioButton radioButtonObjectReference;
        private ComboBox comboBoxReferencedObject;
        private RadioButton radioButtonValueOf;
        private RadioButton radioButtonConstant;
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenuConstantType;
        private TextBox textBoxConstantValue;
        private RadioButton radioButtonContext;
        private LayoutManager.CommonUI.Controls.OperandValueOf operandValueOf;
        private ComboBox comboBoxBooleanConstant;
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private void endOfDesignerVariables() { }

        XmlElement element;
        string prefix = "Arg";

        public GenerateEventArgument() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        public XmlElement Element {
            get {
                return element;
            }

            set {
                element = value;
            }
        }

        public string Prefix {
            get {
                return prefix;
            }

            set {
                prefix = value;
            }
        }

        public void Initialize() {
            if (element == null)
                throw new ArgumentException("Element property not set");

            operandValueOf.Element = element;
            operandValueOf.Suffix = prefix;
            operandValueOf.Initialize();

            IDictionary symbolNameToTypeMap = new HybridDictionary();

            EventManager.Event(new LayoutEvent(this, "add-context-symbols-and-types", null, symbolNameToTypeMap));

            foreach (string n in symbolNameToTypeMap.Keys)
                comboBoxReferencedObject.Items.Add(n);

            string symbolName = GetAttribute("SymbolName");

            if (symbolName != null) {
                comboBoxReferencedObject.Text = symbolName;

                foreach (string n in comboBoxReferencedObject.Items)
                    if (n == symbolName) {
                        comboBoxReferencedObject.SelectedItem = n;
                        break;
                    }
            }

            switch (GetAttribute("ConstantType", "String")) {

                case "String":
                    linkMenuConstantType.SelectedIndex = 0;
                    break;

                case "Integer":
                    linkMenuConstantType.SelectedIndex = 1;
                    break;

                case "Double":
                    linkMenuConstantType.SelectedIndex = 2;
                    break;

                case "Boolean":
                    linkMenuConstantType.SelectedIndex = 3;
                    break;

            }

            string constant = element.GetAttribute("Value" + prefix);

            comboBoxBooleanConstant.SelectedIndex = 0;

            if (constant != null) {
                if (linkMenuConstantType.SelectedIndex == 3)
                    comboBoxBooleanConstant.SelectedIndex = XmlConvert.ToBoolean(constant) == true ? 1 : 0;
                else
                    textBoxConstantValue.Text = constant;
            }

            switch (GetAttribute("Type", "Null")) {

                case "Null":
                    radioButtonNull.Checked = true;
                    break;

                case "Reference":
                    radioButtonObjectReference.Checked = true;
                    break;

                case "ValueOf":
                    radioButtonConstant.Checked = true;
                    break;

                case "Context":
                    radioButtonContext.Checked = true;
                    break;

                default:
                    throw new ApplicationException("Invalid type");
            }

            updateButtons();
        }

        public bool ValidateInput() {
            if (radioButtonObjectReference.Checked) {
                if (comboBoxReferencedObject.Text.Trim() == "") {
                    MessageBox.Show(this, "You must provide a name of a symbol", "Missing Symbol Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    comboBoxReferencedObject.Focus();
                    return false;
                }
            }
            else if (radioButtonConstant.Checked) {
                switch (linkMenuConstantType.SelectedIndex) {
                    case 1:
                        try {
                            int.Parse(textBoxConstantValue.Text);
                        }
                        catch (FormatException) {
                            MessageBox.Show(this, "Invalid integer", "Invalid constant format", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textBoxConstantValue.Focus();
                            return false;
                        }
                        break;

                    case 2:
                        try {
                            double.Parse(textBoxConstantValue.Text);
                        }
                        catch (FormatException) {
                            MessageBox.Show(this, "Invalid double (real) number", "Invalid constant format", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textBoxConstantValue.Focus();
                            return false;
                        }
                        break;
                }
            }
            else if (radioButtonValueOf.Checked) {
                if (!operandValueOf.ValidateInput())
                    return false;
            }

            return true;
        }

        public bool Commit() {
            if (!ValidateInput())
                return false;

            if (radioButtonNull.Checked)
                SetAttribute("Type", "Null");
            else if (radioButtonObjectReference.Checked) {
                SetAttribute("Type", "Reference");
                SetAttribute("SymbolName", comboBoxReferencedObject.Text);
            }
            else if (radioButtonValueOf.Checked) {
                SetAttribute("Type", "ValueOf");
                operandValueOf.Commit();
            }
            else if (radioButtonConstant.Checked) {
                SetAttribute("Type", "ValueOf");
                element.SetAttribute("Symbol" + prefix + "Access", "Value");

                switch (linkMenuConstantType.SelectedIndex) {
                    case 0:
                        element.SetAttribute("Type" + prefix, "String");
                        break;

                    case 1:
                        element.SetAttribute("Type" + prefix, "Integer");
                        break;

                    case 2:
                        element.SetAttribute("Type" + prefix, "Double");
                        break;

                    case 3:
                        element.SetAttribute("Type" + prefix, "Boolean");
                        break;

                    default:
                        throw new ApplicationException("Invalid constant type");
                }

                if (linkMenuConstantType.SelectedIndex == 3)
                    element.SetAttribute("Value" + prefix, XmlConvert.ToString(comboBoxBooleanConstant.SelectedIndex == 1));
                else
                    element.SetAttribute("Value" + prefix, textBoxConstantValue.Text);

            }
            else if (radioButtonContext.Checked)
                SetAttribute("Type", "Context");

            return true;
        }

        private void updateButtons() {
            if (linkMenuConstantType.SelectedIndex == 3) {
                textBoxConstantValue.Visible = false;
                comboBoxBooleanConstant.Location = textBoxConstantValue.Location;
                comboBoxBooleanConstant.Visible = true;
            }
            else {
                textBoxConstantValue.Visible = true;
                comboBoxBooleanConstant.Visible = false;
            }
        }

        protected string GetAttribute(string name, string defaultValue) {
            string attributeName = prefix + name;

            if (!Element.HasAttribute(attributeName))
                return defaultValue;
            return Element.GetAttribute(attributeName);
        }

        protected string GetAttribute(string name) => GetAttribute(name, null);

        protected void SetAttribute(string name, string value) {
            element.SetAttribute(prefix + name, value);
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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.radioButtonNull = new RadioButton();
            this.radioButtonObjectReference = new RadioButton();
            this.comboBoxReferencedObject = new ComboBox();
            this.operandValueOf = new LayoutManager.CommonUI.Controls.OperandValueOf();
            this.radioButtonValueOf = new RadioButton();
            this.radioButtonConstant = new RadioButton();
            this.linkMenuConstantType = new LayoutManager.CommonUI.Controls.LinkMenu();
            this.textBoxConstantValue = new TextBox();
            this.radioButtonContext = new RadioButton();
            this.comboBoxBooleanConstant = new ComboBox();
            this.SuspendLayout();
            // 
            // radioButtonNull
            // 
            this.radioButtonNull.Location = new System.Drawing.Point(7, 1);
            this.radioButtonNull.Name = "radioButtonNull";
            this.radioButtonNull.TabIndex = 0;
            this.radioButtonNull.Text = "Nothing (null)";
            // 
            // radioButtonObjectReference
            // 
            this.radioButtonObjectReference.Location = new System.Drawing.Point(7, 27);
            this.radioButtonObjectReference.Name = "radioButtonObjectReference";
            this.radioButtonObjectReference.Size = new System.Drawing.Size(128, 24);
            this.radioButtonObjectReference.TabIndex = 1;
            this.radioButtonObjectReference.Text = "Reference to object: ";
            // 
            // comboBoxReferencedObject
            // 
            this.comboBoxReferencedObject.Location = new System.Drawing.Point(135, 29);
            this.comboBoxReferencedObject.Name = "comboBoxReferencedObject";
            this.comboBoxReferencedObject.Size = new System.Drawing.Size(121, 21);
            this.comboBoxReferencedObject.Sorted = true;
            this.comboBoxReferencedObject.TabIndex = 2;
            this.comboBoxReferencedObject.TextChanged += new System.EventHandler(this.comboBoxReferencedObject_TextChanged);
            this.comboBoxReferencedObject.SelectedIndexChanged += new System.EventHandler(this.comboBoxReferencedObject_SelectedIndexChanged);
            // 
            // operandValueOf
            // 
            this.operandValueOf.AllowedTypes = null;
            this.operandValueOf.DefaultAccess = "Property";
            this.operandValueOf.Element = null;
            this.operandValueOf.Location = new System.Drawing.Point(8, 45);
            this.operandValueOf.Name = "operandValueOf";
            this.operandValueOf.Size = new System.Drawing.Size(176, 64);
            this.operandValueOf.Suffix = "";
            this.operandValueOf.TabIndex = 4;
            this.operandValueOf.ValueChanged += new System.EventHandler(this.operandValueOf_ValueChanged);
            // 
            // radioButtonValueOf
            // 
            this.radioButtonValueOf.Location = new System.Drawing.Point(7, 55);
            this.radioButtonValueOf.Name = "radioButtonValueOf";
            this.radioButtonValueOf.Size = new System.Drawing.Size(16, 24);
            this.radioButtonValueOf.TabIndex = 3;
            // 
            // radioButtonConstant
            // 
            this.radioButtonConstant.Location = new System.Drawing.Point(7, 112);
            this.radioButtonConstant.Name = "radioButtonConstant";
            this.radioButtonConstant.Size = new System.Drawing.Size(16, 24);
            this.radioButtonConstant.TabIndex = 5;
            // 
            // linkMenuConstantType
            // 
            this.linkMenuConstantType.Location = new System.Drawing.Point(24, 115);
            this.linkMenuConstantType.Name = "linkMenuConstantType";
            this.linkMenuConstantType.Options = new string[] {
                                                                 "String",
                                                                 "Integer",
                                                                 "Double",
                                                                 "Boolean"};
            this.linkMenuConstantType.SelectedIndex = -1;
            this.linkMenuConstantType.Size = new System.Drawing.Size(48, 17);
            this.linkMenuConstantType.TabIndex = 6;
            this.linkMenuConstantType.TabStop = true;
            this.linkMenuConstantType.Text = "String";
            this.linkMenuConstantType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkMenuConstantType.ValueChanged += new System.EventHandler(this.linkMenuConstantType_ValueChanged);
            // 
            // textBoxConstantValue
            // 
            this.textBoxConstantValue.Location = new System.Drawing.Point(78, 113);
            this.textBoxConstantValue.Name = "textBoxConstantValue";
            this.textBoxConstantValue.Size = new System.Drawing.Size(178, 20);
            this.textBoxConstantValue.TabIndex = 7;
            this.textBoxConstantValue.Text = "";
            this.textBoxConstantValue.TextChanged += new System.EventHandler(this.textBoxConstantValue_TextChanged);
            // 
            // radioButtonContext
            // 
            this.radioButtonContext.Location = new System.Drawing.Point(7, 137);
            this.radioButtonContext.Name = "radioButtonContext";
            this.radioButtonContext.Size = new System.Drawing.Size(177, 24);
            this.radioButtonContext.TabIndex = 8;
            this.radioButtonContext.Text = "Reference to current context";
            // 
            // comboBoxBooleanConstant
            // 
            this.comboBoxBooleanConstant.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBooleanConstant.Items.AddRange(new object[] {
                                                                         "False",
                                                                         "True"});
            this.comboBoxBooleanConstant.Location = new System.Drawing.Point(176, 80);
            this.comboBoxBooleanConstant.Name = "comboBoxBooleanConstant";
            this.comboBoxBooleanConstant.Size = new System.Drawing.Size(121, 21);
            this.comboBoxBooleanConstant.TabIndex = 9;
            // 
            // GenerateEventArgument
            // 
            this.Controls.Add(this.comboBoxBooleanConstant);
            this.Controls.Add(this.radioButtonContext);
            this.Controls.Add(this.textBoxConstantValue);
            this.Controls.Add(this.linkMenuConstantType);
            this.Controls.Add(this.radioButtonConstant);
            this.Controls.Add(this.radioButtonValueOf);
            this.Controls.Add(this.comboBoxReferencedObject);
            this.Controls.Add(this.radioButtonObjectReference);
            this.Controls.Add(this.radioButtonNull);
            this.Controls.Add(this.operandValueOf);
            this.Name = "GenerateEventArgument";
            this.Size = new System.Drawing.Size(264, 160);
            this.ResumeLayout(false);

        }
        #endregion

        private void comboBoxReferencedObject_SelectedIndexChanged(object sender, System.EventArgs e) {
            radioButtonObjectReference.Checked = true;
        }

        private void comboBoxReferencedObject_TextChanged(object sender, System.EventArgs e) {
            radioButtonObjectReference.Checked = true;
        }

        private void textBoxConstantValue_TextChanged(object sender, System.EventArgs e) {
            radioButtonConstant.Checked = true;
        }

        private void linkMenuConstantType_ValueChanged(object sender, System.EventArgs e) {
            radioButtonConstant.Checked = true;
            updateButtons();
        }

        private void operandValueOf_ValueChanged(object sender, System.EventArgs e) {
            radioButtonValueOf.Checked = true;
        }

    }
}
