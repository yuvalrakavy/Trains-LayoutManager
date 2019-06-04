using System;
using System.Diagnostics;
using System.Xml;
using LayoutManager;

#nullable enable
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
        private const string A_Name = "Name";
        private const string A_Type = "Type";
        private const string A_Value = "Value";

        public AttributeInfo(XmlElement element) {
            this.Element = element;
        }

        public XmlElement Element { get; }
        public XmlElement? OptionalElement => Element;

        public string Name {
            get => Element.GetAttribute(A_Name);
            set => Element.SetAttribute(A_Name, value);
        }

        public AttributeType AttributeType {
            get => this.AttributeValue(A_Type).Enum<AttributeType>() ?? AttributeType.String;
            set => Element.SetAttribute(A_Type, value);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public object Value {
            get => AttributeType switch
            {
                AttributeType.String => (object)this.AttributeValue(A_Value),
                AttributeType.Number => (object)((int?)this.AttributeValue(A_Value) ?? 0),
                AttributeType.Boolean => (object)((bool?)this.AttributeValue(A_Value) ?? false),
                _ => new ArgumentException("Invalid attribute type " + AttributeType)
            };

            set {
                switch (value) {
                    case int v: this.SetAttribute(A_Value, v); break;
                    case Enum e: this.SetAttribute(A_Value, e); break;
                    case string s: this.SetAttribute(A_Value, s); break;
                    case bool b: this.SetAttribute(A_Value, b); break;
                    default: throw new ArgumentException("Unsupported type for an attribute: " + value.GetType().Name);
                }
            }
        }

        public string ValueAsString => Element.GetAttribute(A_Value);
    }

    public class AttributesInfo : XmlIndexedCollection<AttributeInfo, string> {
        public AttributesInfo(XmlElement element) : base(element) {
        }

        protected override XmlElement CreateElement(AttributeInfo item) {
            Debug.Assert(item.Element.OwnerDocument != Element.OwnerDocument);

            return (XmlElement)Element.OwnerDocument.ImportNode(item.Element, true);
        }

        protected override AttributeInfo FromElement(XmlElement itemElement) => new AttributeInfo(itemElement);

        protected override string GetItemKey(AttributeInfo item) => item.Name;

        public new object? this[string name] {
            get {
                var attribute = base[name];

                return attribute?.Value;
            }

            set {
                var attribute = base[name];

                if (value != null) {
                    if (attribute == null) {
                        XmlElement attributeElement = Element.OwnerDocument.CreateElement("Attribute");

                        attribute = new AttributeInfo(attributeElement) {
                            Name = name
                        };

                        base.Add(attribute);
                    }
                }
                else if (attribute != null)
                    base.Remove(attribute);
            }
        }
    }

    public class AttributesOwner : IObjectHasXml, IObjectHasAttributes {
        public AttributesOwner(XmlElement element) {
            this.Element = element;
        }

        public XmlElement Element { get; }
        public XmlElement? OptionalElement => Element;

        public bool HasAttributes => Element["Attributes"] != null;

        public AttributesInfo Attributes {
            get {
                XmlElement attributesElement = Element["Attributes"];

                if (attributesElement == null) {
                    attributesElement = Element.OwnerDocument.CreateElement("Attributes");
                    Element.AppendChild(attributesElement);
                }

                return new AttributesInfo(attributesElement);
            }
        }
    }
}
