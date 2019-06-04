using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Collections.Generic;

#pragma warning disable IDE0051, IDE0060
namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for OperandValueOf.
    /// </summary>
    public class OperandValueOf : System.Windows.Forms.UserControl {
        private ComboBox comboBoxTag;
        private ComboBox comboBoxSymbol;
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenuPropertyOrAttribute;
        private Label label1;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private void endOfDesignerVariables() { }
        private IDictionary symbolNameToTypeMap = null;
        private string propertiesOfSymbol = "";
        private bool propertyOrAttributeChanged = false;

        public event EventHandler ValueChanged;

        public OperandValueOf() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        #region Properties

        public string Suffix { get; set; } = "";

        public XmlElement Element { get; set; } = null;

        public string DefaultAccess { get; set; } = "Property";

        public Type[] AllowedTypes { get; set; } = null;

        #endregion

        public void Initialize() {
            if (Element == null)
                throw new ArgumentException("Element not set");

            string symbolAccess = Element.GetAttribute("Symbol" + Suffix + "Access");

            if (symbolAccess == null || symbolAccess == "")
                symbolAccess = DefaultAccess;

            InitializePropertyOrAttributeSelector();

            comboBoxSymbol.Text = Element.GetAttribute("Symbol" + Suffix);
            comboBoxTag.Text = Element.GetAttribute("Name" + Suffix);

            if (symbolAccess == "Property")
                linkMenuPropertyOrAttribute.SelectedIndex = 0;
            else
                linkMenuPropertyOrAttribute.SelectedIndex = 1;
        }

        public bool ValidateInput() {
            if (comboBoxSymbol.Text.Trim() == null) {
                MessageBox.Show(this, "You have to specify symbol name", "Missing symbol name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxSymbol.Focus();
                return false;
            }

            if (comboBoxTag.Text.Trim() == "") {
                MessageBox.Show(this, "You have to specify " + (linkMenuPropertyOrAttribute.SelectedIndex == 0 ? "property" : "attribute") + " name", "Missing value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxTag.Focus();
                return false;
            }

            return true;
        }

        public bool Commit() {
            string accessAttribute = "Symbol" + Suffix + "Access";
            bool ok = true;

            if (!ValidateInput())
                return false;

            if (linkMenuPropertyOrAttribute.SelectedIndex == 0)
                Element.SetAttribute(accessAttribute, "Property");
            else
                Element.SetAttribute(accessAttribute, "Attribute");

            if (comboBoxTag.SelectedItem == null) {
                foreach (TagEntry entry in comboBoxTag.Items) {
                    if (entry.Name == comboBoxTag.Text) {
                        comboBoxTag.SelectedItem = entry;
                        break;
                    }
                }
            }

            Element.SetAttribute("Symbol" + Suffix, comboBoxSymbol.Text);
            Element.SetAttribute("Name" + Suffix, comboBoxTag.Text);

            if (comboBoxTag.SelectedItem != null)
                Element.SetAttribute("Type" + Suffix, ((TagEntry)comboBoxTag.SelectedItem).TypeName);

            return ok;
        }

        protected void InitializePropertyOrAttributeSelector() {
            if (symbolNameToTypeMap == null) {
                symbolNameToTypeMap = new HybridDictionary();

                EventManager.Event(new LayoutEvent("add-context-symbols-and-types", this, symbolNameToTypeMap));

                comboBoxSymbol.Sorted = true;
                foreach (string symbolName in symbolNameToTypeMap.Keys)
                    comboBoxSymbol.Items.Add(symbolName);
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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.comboBoxTag = new ComboBox();
            this.comboBoxSymbol = new ComboBox();
            this.linkMenuPropertyOrAttribute = new LayoutManager.CommonUI.Controls.LinkMenu();
            this.label1 = new Label();
            this.SuspendLayout();
            // 
            // comboBoxTag
            // 
            this.comboBoxTag.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxTag.Location = new System.Drawing.Point(70, 35);
            this.comboBoxTag.Name = "comboBoxTag";
            this.comboBoxTag.Size = new System.Drawing.Size(100, 21);
            this.comboBoxTag.TabIndex = 6;
            this.comboBoxTag.DropDown += this.comboBoxTag_DropDown;
            // 
            // comboBoxSymbol
            // 
            this.comboBoxSymbol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSymbol.Location = new System.Drawing.Point(70, 10);
            this.comboBoxSymbol.Name = "comboBoxSymbol";
            this.comboBoxSymbol.Size = new System.Drawing.Size(100, 21);
            this.comboBoxSymbol.TabIndex = 5;
            this.comboBoxSymbol.SelectedIndexChanged += this.comboBoxSymbol_SelectedIndexChanged;
            // 
            // linkMenuPropertyOrAttribute
            // 
            this.linkMenuPropertyOrAttribute.Location = new System.Drawing.Point(15, 34);
            this.linkMenuPropertyOrAttribute.Name = "linkMenuPropertyOrAttribute";
            this.linkMenuPropertyOrAttribute.Options = new string[] {
                                                                        "property",
                                                                        "attribute"};
            this.linkMenuPropertyOrAttribute.SelectedIndex = 0;
            this.linkMenuPropertyOrAttribute.Size = new System.Drawing.Size(48, 23);
            this.linkMenuPropertyOrAttribute.TabIndex = 4;
            this.linkMenuPropertyOrAttribute.TabStop = true;
            this.linkMenuPropertyOrAttribute.Text = "property";
            this.linkMenuPropertyOrAttribute.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkMenuPropertyOrAttribute.ValueChanged += this.linkMenuPropertyOrAttribute_ValueChanged;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 23);
            this.label1.TabIndex = 7;
            this.label1.Text = "Value of:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // OperandValueOf
            // 
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxTag);
            this.Controls.Add(this.comboBoxSymbol);
            this.Controls.Add(this.linkMenuPropertyOrAttribute);
            this.Name = "OperandValueOf";
            this.Size = new System.Drawing.Size(176, 64);
            this.ResumeLayout(false);
        }
        #endregion

        private void linkMenuPropertyOrAttribute_ValueChanged(object sender, System.EventArgs e) {
            propertyOrAttributeChanged = true;

            ValueChanged?.Invoke(this, null);
        }

        private bool isAllowedType(Type typeToCheck) {
            if (AllowedTypes == null)
                return true;
            else {
                foreach (Type type in AllowedTypes)
                    if (typeToCheck.IsSubclassOf(type) || typeToCheck == type)
                        return true;

                return false;
            }
        }

        private void comboBoxTag_DropDown(object sender, System.EventArgs e) {
            if (comboBoxSymbol.Text != propertiesOfSymbol || propertyOrAttributeChanged) {
                propertiesOfSymbol = comboBoxSymbol.Text;
                propertyOrAttributeChanged = false;

                comboBoxTag.Items.Clear();

                Type symbolType = (Type)symbolNameToTypeMap[propertiesOfSymbol];

                if (symbolType != null) {
                    if (linkMenuPropertyOrAttribute.SelectedIndex == 0) {   // Property
                        PropertyInfo[] properties = symbolType.GetProperties();

                        comboBoxTag.Sorted = true;

                        foreach (PropertyInfo property in properties) {
                            BrowsableAttribute[] browsables = (BrowsableAttribute[])property.GetCustomAttributes(typeof(BrowsableAttribute), true);
                            bool propertyAllowed = false;

                            if (browsables.Length > 0 && !browsables[0].Browsable)
                                propertyAllowed = false;
                            else
                                propertyAllowed = isAllowedType(property.PropertyType);

                            if (propertyAllowed)
                                comboBoxTag.Items.Add(new TagEntry(property.Name, property.PropertyType));
                        }

                        Type infoType = (Type)EventManager.Event(new LayoutEvent("get-context-symbol-info-type", symbolType));

                        if (infoType != null) {
                            PropertyInfo[] infoProperties = infoType.GetProperties();

                            foreach (PropertyInfo property in infoProperties) {
                                BrowsableAttribute[] browsables = (BrowsableAttribute[])property.GetCustomAttributes(typeof(BrowsableAttribute), true);
                                bool propertyAllowed = false;

                                if (browsables.Length > 0 && !browsables[0].Browsable)
                                    propertyAllowed = false;
                                else
                                    propertyAllowed = isAllowedType(property.PropertyType);

                                bool nameExist = false;

                                foreach (TagEntry entry in comboBoxTag.Items)
                                    if (entry.Name == property.Name) {
                                        nameExist = true;
                                        break;
                                    }

                                if (propertyAllowed && !nameExist)
                                    comboBoxTag.Items.Add(new TagEntry(property.Name, property.PropertyType));
                            }
                        }
                    }
                    else {          // Attribute
                        var attributesList = new List<AttributesInfo>();
                        var attributesMap = new Dictionary<string, AttributeInfo>();

                        EventManager.Event(new LayoutEvent("get-object-attributes", symbolType, attributesList));

                        foreach (AttributesInfo attributes in attributesList) {
                            foreach (AttributeInfo attribute in attributes) {
                                if (!attributesMap.ContainsKey(attribute.Name) && isAllowedType(attribute.Value.GetType())) {
                                    attributesMap.Add(attribute.Name, attribute);
                                    comboBoxTag.Items.Add(new TagEntry(attribute.Name, attribute.AttributeType));
                                }
                            }
                        }
                    }
                }
            }

            ValueChanged?.Invoke(this, null);
        }

        private void comboBoxSymbol_SelectedIndexChanged(object sender, System.EventArgs e) {
            ValueChanged?.Invoke(this, null);
        }

        private class TagEntry {
            public string Name;
            public Type Type;

            public TagEntry(string name, Type type) {
                this.Name = name;
                this.Type = type;
            }

            public TagEntry(string name, AttributeType attributeType) {
                this.Name = name;

                switch (attributeType) {
                    case AttributeType.Boolean:
                        Type = typeof(bool);
                        break;

                    case AttributeType.Number:
                        Type = typeof(int);
                        break;

                    case AttributeType.String:
                    default:
                        Type = typeof(string);
                        break;
                }
            }

            public string TypeName {
                get {
                    if (Type == typeof(bool))
                        return "Boolean";
                    else if (Type == typeof(int))
                        return "Integer";
                    else if (Type == typeof(double))
                        return "Double";
                    else return Type == typeof(string) ? "String" : Type.Name;
                }
            }

            public override string ToString() => Name;
        }
    }
}
