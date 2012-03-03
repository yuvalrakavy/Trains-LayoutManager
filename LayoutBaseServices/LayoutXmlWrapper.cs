using System;
using System.Diagnostics;
using System.Xml;

namespace LayoutManager {
	public class LayoutXmlWrapper : IObjectHasXml {
		private XmlElement	_element;

		public LayoutXmlWrapper() {
		}

		public LayoutXmlWrapper(string name) {
            InitElement(name);
		}

		public LayoutXmlWrapper(XmlElement element) {
			this._element = element;
		}

		public LayoutXmlWrapper(XmlElement parent, string elementName, bool alwaysAppend = false) {
			this._element = parent[elementName];

			if(alwaysAppend || _element == null) {
				this._element = parent.OwnerDocument.CreateElement(elementName);

				parent.AppendChild(_element);
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
			_element = doc.DocumentElement;
		}

		public XmlElement Element {
			get {
				return _element;
			}

			set {
				_element = value;
			}
		}

		public XmlElement CreateChildElement(string elementName) {
			XmlElement childElement = Element.OwnerDocument.CreateElement(elementName);

			Element.AppendChild(childElement);
			return childElement;
		}

		public XmlElement this[string elementName, bool createIfNotFound = false] {
			get {
				XmlElement childElement = Element[elementName];

				if(childElement == null && createIfNotFound)
					childElement = CreateChildElement(elementName);
				return childElement;
			}
		}

		public String GetAttribute(string name, string defaultValue) {
			if(_element == null)
				return defaultValue;
			else {
				if(_element.Attributes[name] == null)
					return defaultValue;
				else
					return _element.GetAttribute(name);
			}
		}

		public String GetAttribute(string name) {
			return GetAttribute(name, null);
		}

		public void SetAttribute(string name, string v) {
			_element.SetAttribute(name, v);
		}

		public void SetAttribute(string name, int v) {
			_element.SetAttribute(name, XmlConvert.ToString(v));
		}

		public void SetAttribute(string name, Guid v) {
			_element.SetAttribute(name, XmlConvert.ToString(v));
		}

		public void SetAttribute(string name, bool v) {
			_element.SetAttribute(name, XmlConvert.ToString(v));
		}

		public bool HasAttribute(string name) {
			return Element.HasAttribute(name);
		}

		public Guid Id {
			get {
				if(HasAttribute("ID"))
					return XmlConvert.ToGuid(GetAttribute("ID"));
				else {
					Guid	id = Guid.NewGuid();

					SetAttribute("ID", XmlConvert.ToString(id));
					return id;
				}
			}

			set {
				Debug.Assert(!HasAttribute("ID"));
				SetAttribute("ID", XmlConvert.ToString(value));
			}
		}
	}

	public class LayoutXmlWithIdWrapper : LayoutXmlWrapper, IObjectHasId {
		public LayoutXmlWithIdWrapper(XmlElement element)
			: base(element) {
		}
	}
}
