using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Xml;

using LayoutManager;
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls
{
	/// <summary>
	/// Summary description for OperandValueOf.
	/// </summary>
	public class OperandValueOf : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.ComboBox comboBoxTag;
		private System.Windows.Forms.ComboBox comboBoxSymbol;
		private LayoutManager.CommonUI.Controls.LinkMenu linkMenuPropertyOrAttribute;
		private System.Windows.Forms.Label label1;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private void endOfDesignerVariables() { }

		string				suffix = "";
		XmlElement			element = null;
		string				defaultAccess = "Property";
		IDictionary			symbolNameToTypeMap = null;
		string				propertiesOfSymbol = "";
		Type[]				allowedTypes = null;
		bool				propertyOrAttributeChanged = false;

		public event EventHandler ValueChanged;

		public OperandValueOf()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

		}

		#region Properties

		public string Suffix {
			get {
				return suffix;
			}

			set {
				suffix = value;
			}
		}

		public XmlElement Element {
			get {
				return element;
			}

			set {
				element = value;
			}
		}

		public string DefaultAccess {
			get {
				return defaultAccess;
			}

			set {
				defaultAccess = value;
			}
		}

		public Type[] AllowedTypes {
			get {
				return allowedTypes;
			}

			set {
				allowedTypes = value;
			}
		}

		#endregion

		public void Initialize() {
			if(Element == null)
				throw new ArgumentException("Element not set");

			string	symbolAccess = Element.GetAttribute("Symbol" + suffix + "Access");

			if(symbolAccess == null || symbolAccess == "")
				symbolAccess = defaultAccess;

			InitializePropertyOrAttributeSelector();

			comboBoxSymbol.Text = Element.GetAttribute("Symbol" + suffix);
			comboBoxTag.Text = Element.GetAttribute("Name" + suffix);

			if(symbolAccess == "Property")
				linkMenuPropertyOrAttribute.SelectedIndex = 0;
			else
				linkMenuPropertyOrAttribute.SelectedIndex = 1;
		}

		public bool ValidateInput() {
			if(comboBoxSymbol.Text.Trim() == null) {
				MessageBox.Show(this, "You have to specify symbol name", "Missing symbol name", MessageBoxButtons.OK, MessageBoxIcon.Error);
				comboBoxSymbol.Focus();
				return false;
			}

			if(comboBoxTag.Text.Trim() == "") {
				MessageBox.Show(this, "You have to specify " + (linkMenuPropertyOrAttribute.SelectedIndex == 0 ? "property" : "attribute") + " name", "Missing value", MessageBoxButtons.OK, MessageBoxIcon.Error);
				comboBoxTag.Focus();
				return false;
			}

			return true;
		}

		public bool Commit() {
			string	accessAttribute = "Symbol" + suffix + "Access";
			bool	ok = true;

			if(!ValidateInput())
				return false;

			if(linkMenuPropertyOrAttribute.SelectedIndex == 0)
				Element.SetAttribute(accessAttribute, "Property");
			else
				Element.SetAttribute(accessAttribute, "Attribute");

			if(comboBoxTag.SelectedItem == null) {
				foreach(TagEntry entry in comboBoxTag.Items) {
					if(entry.Name == comboBoxTag.Text) {
						comboBoxTag.SelectedItem = entry;
						break;
					}
				}
			}

			Element.SetAttribute("Symbol" + suffix, comboBoxSymbol.Text);
			Element.SetAttribute("Name" + suffix, comboBoxTag.Text);

			if(comboBoxTag.SelectedItem != null)
				Element.SetAttribute("Type" + suffix, ((TagEntry)comboBoxTag.SelectedItem).TypeName);

			return ok;
		}

		protected void InitializePropertyOrAttributeSelector() {
			if(symbolNameToTypeMap == null) {
				symbolNameToTypeMap = new HybridDictionary();

				EventManager.Event(new LayoutEvent(this, "add-context-symbols-and-types", null, symbolNameToTypeMap));

				comboBoxSymbol.Sorted = true;
				foreach(string symbolName in symbolNameToTypeMap.Keys)
					comboBoxSymbol.Items.Add(symbolName);
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.comboBoxTag = new System.Windows.Forms.ComboBox();
			this.comboBoxSymbol = new System.Windows.Forms.ComboBox();
			this.linkMenuPropertyOrAttribute = new LayoutManager.CommonUI.Controls.LinkMenu();
			this.label1 = new System.Windows.Forms.Label();
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
			this.comboBoxTag.DropDown += new System.EventHandler(this.comboBoxTag_DropDown);
			// 
			// comboBoxSymbol
			// 
			this.comboBoxSymbol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxSymbol.Location = new System.Drawing.Point(70, 10);
			this.comboBoxSymbol.Name = "comboBoxSymbol";
			this.comboBoxSymbol.Size = new System.Drawing.Size(100, 21);
			this.comboBoxSymbol.TabIndex = 5;
			this.comboBoxSymbol.SelectedIndexChanged += new System.EventHandler(this.comboBoxSymbol_SelectedIndexChanged);
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
			this.linkMenuPropertyOrAttribute.ValueChanged += new System.EventHandler(this.linkMenuPropertyOrAttribute_ValueChanged);
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

			if(ValueChanged != null)
				ValueChanged(this, null);
		}

		private bool isAllowedType(Type typeToCheck) {
			if(allowedTypes == null)
				return true;
			else {
				foreach(Type type in allowedTypes)
					if(typeToCheck.IsSubclassOf(type) || typeToCheck == type)
						return true;

				return false;
			}
		}

		private void comboBoxTag_DropDown(object sender, System.EventArgs e) {
			if(comboBoxSymbol.Text != propertiesOfSymbol || propertyOrAttributeChanged) {
				propertiesOfSymbol = comboBoxSymbol.Text;
				propertyOrAttributeChanged = false;

				comboBoxTag.Items.Clear();

				Type	symbolType = (Type)symbolNameToTypeMap[propertiesOfSymbol];

				if(symbolType != null) {
					if(linkMenuPropertyOrAttribute.SelectedIndex == 0) {	// Property

						PropertyInfo[]	properties = symbolType.GetProperties();

						comboBoxTag.Sorted = true;

						foreach(PropertyInfo property in properties) {
							BrowsableAttribute[]	browsables = (BrowsableAttribute[])property.GetCustomAttributes(typeof(BrowsableAttribute), true);
							bool					propertyAllowed = false;

							if(browsables.Length > 0 && browsables[0].Browsable == false)
								propertyAllowed = false;
							else
								propertyAllowed = isAllowedType(property.PropertyType);

							if(propertyAllowed)
								comboBoxTag.Items.Add(new TagEntry(property.Name, property.PropertyType));
						}

						Type	infoType = (Type)EventManager.Event(new LayoutEvent(symbolType, "get-context-symbol-info-type"));

						if(infoType != null) {
							PropertyInfo[]	infoProperties = infoType.GetProperties();

							foreach(PropertyInfo property in infoProperties) {
								BrowsableAttribute[]	browsables = (BrowsableAttribute[])property.GetCustomAttributes(typeof(BrowsableAttribute), true);
								bool					propertyAllowed = false;

								if(browsables.Length > 0 && browsables[0].Browsable == false)
									propertyAllowed = false;
								else
									propertyAllowed = isAllowedType(property.PropertyType);

								bool	nameExist = false;
								
								foreach(TagEntry entry in comboBoxTag.Items)
									if(entry.Name == property.Name) {
										nameExist = true;
										break;
									}

								if(propertyAllowed && !nameExist)
									comboBoxTag.Items.Add(new TagEntry(property.Name, property.PropertyType));
							}
						}
					}
					else {			// Attribute
						ArrayList	attributesList = new ArrayList();
						IDictionary	attributesMap = new HybridDictionary();

						EventManager.Event(new LayoutEvent(symbolType, "get-object-attributes", null, attributesList));

						foreach(AttributesInfo attributes in attributesList) {
							foreach(AttributeInfo attribute in attributes) {
								if(!attributesMap.Contains(attribute.Name) && isAllowedType(attribute.Value.GetType())) {
									attributesMap.Add(attribute.Name, attribute);
									comboBoxTag.Items.Add(new TagEntry(attribute.Name, attribute.Type));
								}
							}
						}
					}
				}
			}

			if(ValueChanged != null)
				ValueChanged(this, null);
		}

		private void comboBoxSymbol_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(ValueChanged != null)
				ValueChanged(this, null);
		}

		class TagEntry {
			public string	Name;
			public Type		Type;

			public TagEntry(string name, Type type) {
				this.Name = name;
				this.Type = type;
			}

			public TagEntry(string name, AttributeType attributeType) {
				this.Name = name;

				switch(attributeType) {

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
					if(Type == typeof(bool))
						return "Boolean";
					else if(Type == typeof(int))
						return "Integer";
					else if(Type == typeof(double))
						return "Double";
					else if(Type == typeof(string))
						return "String";
					else
						return Type.Name;
				}
			}

			public override string ToString() {
				return Name;
			}
		}
	}
}
