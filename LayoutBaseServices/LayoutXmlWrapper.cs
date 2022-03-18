using MethodDispatcher;
using System;
using System.Diagnostics;
using System.Xml;

#nullable enable
namespace LayoutManager {
    static class DispatcherXPathFilter {
        static XmlElement? ExtractXml(object? obj) {
            return obj switch {
                XmlElement element => element,
                IObjectHasXml hasXml => hasXml.OptionalElement,
                _ => null,
            };
        }

        static string ExpandXPath(string xPath, XmlElement instanceElement) {
            if (xPath.IndexOf('`') < 0)
                return xPath;

            var xpn = instanceElement.CreateNavigator() ?? throw new DispatchFilterException("Invalid instance object XML");

            System.Text.StringBuilder result = new(xPath.Length + 60);
            int pos = 0;

            for (int nextPos; (nextPos = xPath.IndexOf('`', pos)) >= 0; pos = nextPos) {
                result.Append(xPath, pos, nextPos - pos);

                if (xPath[nextPos + 1] == '`') {
                    result.Append('`');     // `` is converted to a single `
                    nextPos += 2;
                }
                else {
                    int s = nextPos;    // s is first ` index
                    string expandXpath;

                    nextPos = xPath.IndexOf('`', nextPos + 1);      // nextPos is the index of the terminating `
                    if (nextPos < 0)
                        throw new DispatchFilterException("XPath missing a ` character for expanded string");

                    expandXpath = xPath.Substring(s + 1, nextPos - s - 1);

                    var expandedXpath = xpn.Evaluate(expandXpath).ToString();
                    result.Append(expandedXpath);

                    nextPos++;      // skip the closing `
                }
            }

            result.Append(xPath, pos, xPath.Length - pos);

            return result.ToString();
        }

        static private bool XPathFilter(object? filterValue, object? targetObject, object? parameterValue) {
            if (filterValue == null)
                throw new DispatchFilterException("Missing Value");

            if (filterValue is string xPathValue) {
                var element = ExtractXml(parameterValue);

                if (element == null)
                    return false;

                var instanceElement = ExtractXml(targetObject);
                var xPath = instanceElement != null ? ExpandXPath(xPathValue, instanceElement) : xPathValue;

                return element.CreateNavigator()?.Matches(xPath) ?? false;
            }
            else
                throw new DispatchFilterException("XPath Value must be string");
        }

        [DispatchTarget]
        public static void AddDispatcherFilters() {
            Dispatch.AddCustomParameterFilter("XPath", XPathFilter);
        }
    }

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
                var childElement = Element[elementName];

                if (childElement == null && createIfNotFound)
                    childElement = CreateChildElement(elementName);
                return childElement;
            }
        }

        public ConvertableString AttributeValue(string name) => Element.AttributeValue(name);

        public String? GetOptionalAttribute(string name) => (string?)OptionalElement?.AttributeValue(name);

        public string GetAttribute(string name) => AttributeValue(name).ValidString();

        public void SetAttributeValue(string name, string? v) => Element.SetAttributeValue(name, v, null);

        public void SetAttributValue(string name, string? v, string? removeIf) => Element.SetAttributeValue(name, v, removeIf);

        public void SetAttributeValue(string name, int v, int removeIf) => Element.SetAttributeValue(name, v, removeIf);

        public void SetAttributeValue(string name, int v) => Element.SetAttributeValue(name, v);

        public void SetAttributeValue(string name, UInt32 v, UInt32 removeIf) => Element.SetAttributeValue(name, v, removeIf);

        public void SetAttributeValue(string name, UInt32 v) => Element.SetAttributeValue(name, v);

        public void SetAttributeValue(string name, double v) => Element.SetAttributeValue(name, v);

        public void SetAttributeValue(string name, Guid v) => Element.SetAttributeValue(name, v);

        public void SetAttributeValue(string name, Guid v, Guid removeIf) => Element.SetAttributeValue(name, v, removeIf);

        public void SetAttributeValue(string name, bool v) => Element.SetAttributeValue(name, v);

        public void SetAttributeValue(string name, bool v, bool removeIf) => Element.SetAttributeValue(name, v, removeIf);

        public void SetAttributeValue(string name, Enum e) => Element.SetAttributeValue(name, e);

        public void SetAttribute(string name, Enum e, Enum removeIf) => Element.SetAttributeValue(name, e, removeIf);

        public bool HasAttribute(string name) => Element.HasAttribute(name);

        public void RemoveAttribute(string name) => Element.RemoveAttribute(name);

        public Guid Id {
            get {
                var id = (Guid?)this.AttributeValue("ID");

                if (!id.HasValue) {
                    Guid newId = Guid.NewGuid();

                    this.SetAttributeValue("ID", newId);
                    return newId;
                }
                else
                    return id.Value;
            }

            set {
                Debug.Assert(!this.HasAttribute("ID"));
                this.SetAttributeValue("ID", value);
            }
        }
    }

    public class LayoutXmlWithIdWrapper : LayoutXmlWrapper, IObjectHasId {
        public LayoutXmlWithIdWrapper(XmlElement element)
            : base(element) {
        }
    }
}
