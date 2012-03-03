using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace LayoutManager {
	/// <summary>
	/// This interface is implemented by objects that have user attributes
	/// </summary>
	public interface IObjectHasAttributes {
		bool HasAttributes { get; }

		AttributesInfo Attributes { get; }
	}

	public enum AttributeType {
		String, Number, Boolean
	}

	public class AttributeInfo : IObjectHasXml {
		XmlElement	element;

		public AttributeInfo(XmlElement element) {
			this.element = element;
		}

		public XmlElement Element {
			get {
				return element;
			}
		}

		public string Name {
			get {
				return Element.GetAttribute("Name");
			}

			set {
				Element.SetAttribute("Name", value);
			}
		}

		public AttributeType Type {
			get {
				return (AttributeType)Enum.Parse(typeof(AttributeType), Element.GetAttribute("Type"));
			}

			set {
				Element.SetAttribute("Type", value.ToString());
			}
		}

		public object Value {
			get {
				string	v = Element.GetAttribute("Value");

				switch(Type) {
					case AttributeType.String:
						return v;

					case AttributeType.Number:
						return XmlConvert.ToInt32(v);

					case AttributeType.Boolean:
						return XmlConvert.ToBoolean(v);

					default:
						throw new ArgumentException("Invalid attribute type " + Type);
				}
			}

			set {
				string			v;
				Type			type = value.GetType();

				if(type == typeof(int)) {
					Type = AttributeType.Number;
					v = XmlConvert.ToString((int)value);
				}
				else if(type == typeof(Enum)) {
					Type = AttributeType.String;
					v = value.ToString();
				}
				else if(type == typeof(string)) {
					Type = AttributeType.String;
					v = (string)value;
				}
				else if(type == typeof(bool)) {
					Type = AttributeType.Boolean;
					v = XmlConvert.ToString((bool)value);
				}
				else
					throw new ArgumentException("Unsupported type for an attribute: " + value.GetType().Name);

				Element.SetAttribute("Value", v);
			}
		}

		public string ValueAsString {
			get {
				return Element.GetAttribute("Value");
			}
		}
	}

	public class AttributesInfo : XmlIndexedCollection<AttributeInfo, string> {
		public AttributesInfo(XmlElement element) : base(element) {
		}

		protected override XmlElement CreateElement(AttributeInfo item) {
			Debug.Assert(item.Element.OwnerDocument != Element.OwnerDocument);

			return (XmlElement)Element.OwnerDocument.ImportNode(item.Element, true);
		}

  		protected override AttributeInfo FromElement(XmlElement itemElement) {
			return new AttributeInfo(itemElement);
		}

  		protected override string GetItemKey(AttributeInfo item) {
			return item.Name;
		}

		public new object this[string name] {
			get {
				AttributeInfo	attribute = base[name];

				if(attribute == null)
					return null;

				return attribute.Value;
			}

			set {
				AttributeInfo	attribute = base[name];

				if(attribute == null) {
					XmlElement	attributeElement = Element.OwnerDocument.CreateElement("Attribute");

					attribute = new AttributeInfo(attributeElement);
					attribute.Name = name;

					base.Add(attribute);
				}

				attribute.Value = value;
			}
		}
	}

	public class AttributesOwner : IObjectHasXml, IObjectHasAttributes {
		XmlElement	element;

		public AttributesOwner(XmlElement element) {
			this.element = element;
		}

		public XmlElement Element {
			get {
				return element;
			}
		}

		public bool HasAttributes {
			get {
				return Element["Attributes"] != null;
			}
		}

		public AttributesInfo Attributes {
			get {
				XmlElement	attributesElement = Element["Attributes"];

				if(attributesElement == null) {
					attributesElement = Element.OwnerDocument.CreateElement("Attributes");
					Element.AppendChild(attributesElement);
				}

				return new AttributesInfo(attributesElement);
			}
		}
	}
}
