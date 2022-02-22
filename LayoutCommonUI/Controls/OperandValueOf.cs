using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Xml;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for OperandValueOf.
    /// </summary>
    public partial class OperandValueOf : UserControl {
        private IDictionary? symbolNameToTypeMap = null;
        private string propertiesOfSymbol = "";
        private bool propertyOrAttributeChanged = false;

        public event EventHandler? ValueChanged;

        public OperandValueOf() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        #region Properties

        public string Suffix { get; set; } = "";

        public XmlElement? Element { get; set; } = null;

        public string DefaultAccess { get; set; } = "Property";

        public Type[]? AllowedTypes { get; set; } = null;

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
                Element?.SetAttribute(accessAttribute, "Property");
            else
                Element?.SetAttribute(accessAttribute, "Attribute");

            if (comboBoxTag.SelectedItem == null) {
                foreach (TagEntry entry in comboBoxTag.Items) {
                    if (entry.Name == comboBoxTag.Text) {
                        comboBoxTag.SelectedItem = entry;
                        break;
                    }
                }
            }

            Element?.SetAttribute("Symbol" + Suffix, comboBoxSymbol.Text);
            Element?.SetAttribute("Name" + Suffix, comboBoxTag.Text);

            if (comboBoxTag.SelectedItem != null)
                Element?.SetAttribute("Type" + Suffix, ((TagEntry)comboBoxTag.SelectedItem).TypeName);

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

        private void LinkMenuPropertyOrAttribute_ValueChanged(object? sender, EventArgs e) {
            propertyOrAttributeChanged = true;

            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private bool IsAllowedType(Type typeToCheck) {
            if (AllowedTypes == null)
                return true;
            else {
                foreach (Type type in AllowedTypes)
                    if (typeToCheck.IsSubclassOf(type) || typeToCheck == type)
                        return true;

                return false;
            }
        }

        private void ComboBoxTag_DropDown(object? sender, EventArgs e) {
            if (comboBoxSymbol.Text != propertiesOfSymbol || propertyOrAttributeChanged) {
                propertiesOfSymbol = comboBoxSymbol.Text;
                propertyOrAttributeChanged = false;

                comboBoxTag.Items.Clear();

                var symbolType = (Type?)symbolNameToTypeMap?[propertiesOfSymbol];

                if (symbolType != null) {
                    if (linkMenuPropertyOrAttribute.SelectedIndex == 0) {   // Property
                        PropertyInfo[] properties = symbolType.GetProperties();

                        comboBoxTag.Sorted = true;

                        foreach (PropertyInfo property in properties) {
                            BrowsableAttribute[] browsables = (BrowsableAttribute[])property.GetCustomAttributes(typeof(BrowsableAttribute), true);
                            bool propertyAllowed;

                            if (browsables.Length > 0 && !browsables[0].Browsable)
                                propertyAllowed = false;
                            else
                                propertyAllowed = IsAllowedType(property.PropertyType);

                            if (propertyAllowed)
                                comboBoxTag.Items.Add(new TagEntry(property.Name, property.PropertyType));
                        }

                        var infoType = (Type?)EventManager.Event(new LayoutEvent("get-context-symbol-info-type", symbolType));

                        if (infoType != null) {
                            PropertyInfo[] infoProperties = infoType.GetProperties();

                            foreach (PropertyInfo property in infoProperties) {
                                BrowsableAttribute[] browsables = (BrowsableAttribute[])property.GetCustomAttributes(typeof(BrowsableAttribute), true);
                                bool propertyAllowed;

                                if (browsables.Length > 0 && !browsables[0].Browsable)
                                    propertyAllowed = false;
                                else
                                    propertyAllowed = IsAllowedType(property.PropertyType);

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
                                if (!attributesMap.ContainsKey(attribute.Name) && IsAllowedType(attribute.Value.GetType())) {
                                    attributesMap.Add(attribute.Name, attribute);
                                    comboBoxTag.Items.Add(new TagEntry(attribute.Name, attribute.AttributeType));
                                }
                            }
                        }
                    }
                }
            }

            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ComboBoxSymbol_SelectedIndexChanged(object? sender, EventArgs e) {
            ValueChanged?.Invoke(this, EventArgs.Empty);
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

                Type =attributeType switch {
                    AttributeType.Boolean => typeof(bool),
                    AttributeType.Number => typeof(int),
                    _ => typeof(string),
                };
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
