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

namespace LayoutManager.CommonUI.Controls
{
	/// <summary>
	/// Summary description for Operand.
	/// </summary>
	public class Operand : System.Windows.Forms.UserControl, IObjectHasXml
	{
		private RadioButton radioButtonValue;
		private TextBox textBoxValue;
		private LayoutManager.CommonUI.Controls.LinkMenu linkMenu1Boolean;
		private RadioButton radioButtonPropertyOrAttribute;
		private LayoutManager.CommonUI.Controls.OperandValueOf operandValueOf;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private void endOfDesignerVariables() { }

		string				suffix = "";
		XmlElement			element = null;
		string				defaultAccess = "Property";
		Type[]				allowedTypes = null;

		public Operand()
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

		public bool ValueIsBoolean { get; set; }

        public bool ValueIsOnOff { set; get; }

		#endregion

		public void Initialize() {
			if(Element == null)
				throw new ArgumentException("Element not set");

			operandValueOf.Element = element;
			operandValueOf.AllowedTypes = allowedTypes;
			operandValueOf.Suffix = suffix;
			operandValueOf.Initialize();

			if(ValueIsBoolean) {
				textBoxValue.Visible = false;
				linkMenu1Boolean.Visible = true;
				radioButtonValue.Text = "";
				linkMenu1Boolean.Location = textBoxValue.Location;

                if (ValueIsOnOff)
                    linkMenu1Boolean.Options = new string[] { "On", "Off" };
			}
			else
				linkMenu1Boolean.Visible = false;

			string	symbolAccess = Element.GetAttribute("Symbol" + suffix + "Access");

			if(symbolAccess == null || symbolAccess == "")
				symbolAccess = defaultAccess;

			if(symbolAccess == "Value") {
				radioButtonValue.Checked = true;

				if(Element.HasAttribute("Value" + suffix)) {
                    if (ValueIsBoolean)
                        linkMenu1Boolean.SelectedIndex = (XmlConvert.ToBoolean(Element.GetAttribute("Value" + suffix)) ? 0 : 1);
                    else
                        textBoxValue.Text = Element.GetAttribute("Value" + suffix);
				}
				else
					textBoxValue.Text = "";

                linkMenu1Boolean.Text = linkMenu1Boolean.Options[linkMenu1Boolean.SelectedIndex];
                linkMenu1Boolean.Enabled = true;
			}
			else
				radioButtonPropertyOrAttribute.Checked = true;
		}

		public bool ValidateInput() {
			if(radioButtonPropertyOrAttribute.Checked) {
				if(!operandValueOf.ValidateInput())
					return false;
			}

			return true;
		}

		public bool Commit() {
			string	accessAttribute = "Symbol" + suffix + "Access";
			bool	ok = true;

			if(radioButtonValue.Checked) {
				Element.SetAttribute(accessAttribute, "Value");

				if(ValueIsBoolean)
					Element.SetAttribute("Value" + suffix, XmlConvert.ToString(linkMenu1Boolean.SelectedIndex == 0));
				else
					Element.SetAttribute("Value" + suffix, textBoxValue.Text);

				if(ValueIsBoolean)
					Element.SetAttribute("Type" + suffix, "Booelan");
				else
					Element.SetAttribute("Type" + suffix, "String");

			}
			else
				operandValueOf.Commit();

			return ok;
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
			this.radioButtonValue = new RadioButton();
			this.textBoxValue = new TextBox();
			this.linkMenu1Boolean = new LayoutManager.CommonUI.Controls.LinkMenu();
			this.radioButtonPropertyOrAttribute = new RadioButton();
			this.operandValueOf = new LayoutManager.CommonUI.Controls.OperandValueOf();
			this.SuspendLayout();
			// 
			// radioButtonValue
			// 
			this.radioButtonValue.Location = new System.Drawing.Point(8, 8);
			this.radioButtonValue.Name = "radioButtonValue";
			this.radioButtonValue.Size = new System.Drawing.Size(56, 24);
			this.radioButtonValue.TabIndex = 0;
			this.radioButtonValue.Text = "Value:";
			this.radioButtonValue.CheckedChanged += new System.EventHandler(this.radioButtonValue_CheckedChanged);
			// 
			// textBoxValue
			// 
			this.textBoxValue.Location = new System.Drawing.Point(80, 10);
			this.textBoxValue.Name = "textBoxValue";
			this.textBoxValue.Size = new System.Drawing.Size(80, 20);
			this.textBoxValue.TabIndex = 4;
			this.textBoxValue.Text = "";
			this.textBoxValue.TextChanged += new System.EventHandler(this.textBoxValue_TextChanged);
			// 
			// linkMenu1Boolean
			// 
			this.linkMenu1Boolean.Location = new System.Drawing.Point(112, 9);
			this.linkMenu1Boolean.Name = "linkMenu1Boolean";
			this.linkMenu1Boolean.Options = new string[] {
															 "True",
															 "False"};
			this.linkMenu1Boolean.SelectedIndex = 0;
			this.linkMenu1Boolean.Size = new System.Drawing.Size(80, 24);
			this.linkMenu1Boolean.TabIndex = 5;
			this.linkMenu1Boolean.TabStop = true;
			this.linkMenu1Boolean.Text = "True";
			this.linkMenu1Boolean.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.linkMenu1Boolean.Visible = false;
			// 
			// radioButtonPropertyOrAttribute
			// 
			this.radioButtonPropertyOrAttribute.Location = new System.Drawing.Point(8, 34);
			this.radioButtonPropertyOrAttribute.Name = "radioButtonPropertyOrAttribute";
			this.radioButtonPropertyOrAttribute.Size = new System.Drawing.Size(16, 24);
			this.radioButtonPropertyOrAttribute.TabIndex = 6;
			// 
			// operandValueOf
			// 
			this.operandValueOf.AllowedTypes = null;
			this.operandValueOf.DefaultAccess = "Property";
			this.operandValueOf.Element = null;
			this.operandValueOf.Location = new System.Drawing.Point(9, 24);
			this.operandValueOf.Name = "operandValueOf";
			this.operandValueOf.Size = new System.Drawing.Size(176, 64);
			this.operandValueOf.Suffix = "";
			this.operandValueOf.TabIndex = 7;
			this.operandValueOf.ValueChanged += new System.EventHandler(this.operandValueOf_ValueChanged);
			// 
			// Operand
			// 
			this.Controls.Add(this.radioButtonPropertyOrAttribute);
			this.Controls.Add(this.linkMenu1Boolean);
			this.Controls.Add(this.radioButtonValue);
			this.Controls.Add(this.textBoxValue);
			this.Controls.Add(this.operandValueOf);
			this.Name = "Operand";
			this.Size = new System.Drawing.Size(200, 96);
			this.ResumeLayout(false);

		}
		#endregion

		private void radioButtonValue_CheckedChanged(object sender, System.EventArgs e) {
#if OLD
			linkMenu1Boolean.Enabled = true;
			textBoxValue.Enabled = true;
			comboBoxSymbol.Enabled = false;
			comboBoxTag.Enabled = false;
			linkMenuPropertyOrAttribute.Enabled = false;
#endif
		}

		private void radioButtonPropertyOrAttribute_CheckedChanged(object sender, System.EventArgs e) {
#if OLD
			InitializePropertyOrAttributeSelector();
		
			linkMenu1Boolean.Enabled = false;
			textBoxValue.Enabled = false;
			comboBoxSymbol.Enabled = true;
			comboBoxTag.Enabled = true;
			linkMenuPropertyOrAttribute.Enabled = true;
#endif
		}

		private void textBoxValue_TextChanged(object sender, System.EventArgs e) {
			radioButtonValue.Checked = true;
		}

		private void operandValueOf_ValueChanged(object sender, System.EventArgs e) {
			radioButtonPropertyOrAttribute.Checked = true;
		}
	}
}
