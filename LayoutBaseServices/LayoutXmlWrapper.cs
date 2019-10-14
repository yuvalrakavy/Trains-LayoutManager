using System;
using System.Diagnostics;
using System.Xml;

#nullable enable
namespace LayoutManager {
    public class LayoutXmlWrapper : IObjectHasXml {
        public LayoutXmlWrapper() {
        }

        public LayoutXmlWrapper(string name) {
            InitElement(name);
        }

        public LayoutXmlWrapper(XmlElement element) {
            this.OptionalElement = element;
        }

        public LayoutXmlWrapper(XmlElement parent, string elementName, bool alwaysAppend = false) {
            this.OptionalElement = parent[elementName];

            if (alwaysAppend || OptionalElement == null) {
                this.OptionalElement = parent.OwnerDocument.CreateElement(elementName);

                parent.AppendChild(OptionalElement);
            }
        }

        public void InitElement(string elementName) {
            XmlDocument doc = LayoutXmlInfo.XmlImplementation.CreateDocument();

            Element = doc.CreateElement(elementName);
            doc.AppendChild(Element);
        }

        public void Load(string filename) {
            XmlDocument doc = LayoutXmlInfo.XmlImplementation.CreateDocument();

            doc.Load(filename);
            OptionalElement = doc.DocumentElement;
        }

        public XmlElement Element {
            get {
                if (OptionalElement == null)
                    throw new ArgumentNullException("Element is (null)");
                return OptionalElement;
            }

            set {
                OptionalElement = value;
            }
        }

        public XmlElement? OptionalElement { get; set; }

        public XmlElement CreateChildElement(string elementName) {
            XmlElement childElement = Element.OwnerDocument.CreateElement(elementName);

            Element.AppendChild(childElement);
            return childElement;
        }

        public XmlElement? this[string elementName, bool createIfNotFound = false] {
            get {
                XmlElement childElement = Element[elementName];

                if (childElement == null && createIfNotFound)
                    childElement = CreateChildElement(elementName);
                return childElement;
            }
        }

        public ConvertableString AttributeValue(string name) => ((IObjectHasXml)this).AttributeValue(name);

        public String? GetOptionalAttribute(string name) => (string?)AttributeValue(name);

        public string GetAttribute(string name) => AttributeValue(name).ValidString();

        public void SetAttribute(string name, string? v) => ((IObjectHasXml)this).SetAttribute(name, v);

        public void SetAttribute(string name, string? v, string? removeIf) => ((IObjectHasXml)this).SetAttribute(name, v, removeIf);

        public void SetAttribute(string name, int v, int removeIf) => ((IObjectHasXml)this).SetAttribute(name, v, removeIf);

        public void SetAttribute(string name, int v) => ((IObjectHasXml)this).SetAttribute(name, v);

        public void SetAttribute(string name, UInt32 v, UInt32 removeIf) => ((IObjectHasXml)this).SetAttribute(name, v, removeIf);

        public void SetAttribute(string name, UInt32 v) => ((IObjectHasXml)this).SetAttribute(name, v);

        public void SetAttribute(string name, double v) => ((IObjectHasXml)this).SetAttribute(name, v);

        public void SetAttribute(string name, Guid v) => ((IObjectHasXml)this).SetAttribute(name, v);

        public void SetAttribute(string name, Guid v, Guid removeIf) => ((IObjectHasXml)this).SetAttribute(name, v, removeIf);

        public void SetAttribute(string name, bool v) => ((IObjectHasXml)this).SetAttribute(name, v);

        public void SetAttribute(string name, bool v, bool removeIf) => ((IObjectHasXml)this).SetAttribute(name, v, removeIf);

        public void SetAttribute(string name, Enum e) => ((IObjectHasXml)this).SetAttribute(name, e);

        public void SetAttribute(string name, Enum e, Enum removeIf) => ((IObjectHasXml)this).SetAttribute(name, e, removeIf);

        public bool HasAttribute(string name) => ((IObjectHasXml)this).HasAttribute(name);

        public Guid Id {
            get {
                var id = (Guid?)this.AttributeValue("ID");

                if (!id.HasValue) {
                    Guid newId = Guid.NewGuid();

                    this.SetAttribute("ID", newId);
                    return newId;
                }
                else
                    return id.Value;
            }

            set {
                Debug.Assert(!this.HasAttribute("ID"));
                this.SetAttribute("ID", value);
            }
        }
    }

    public class LayoutXmlWithIdWrapper : LayoutXmlWrapper, IObjectHasId {
        public LayoutXmlWithIdWrapper(XmlElement element)
            : base(element) {
        }
    }
}
