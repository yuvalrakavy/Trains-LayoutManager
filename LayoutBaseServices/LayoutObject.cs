using System;
using System.Xml;

namespace LayoutManager {
	public interface IObjectHasXml {
		XmlElement Element { get; }
	}

	/// <summary>
	/// If a component implements this interface, then it is possible to obtain a
	/// reference to this component via its ID.
	/// </summary>
	public interface IObjectHasId {
		Guid Id {
			get;
		}
	}

	public class LayoutXmlInfo : IObjectHasXml {
		/// <summary>
		/// This Xml implementation is used for creating Xml DOM for all the objects.
		/// This mean that all of them share the same name table, which make it much more effiecent to parse
		/// and store (all the strings are using the same atom table, instead of each component allocating
		/// it own strings).
		/// </summary>
		private static XmlImplementation _xmlImplementation = new XmlImplementation();

        public static XmlImplementation XmlImplementation => LayoutXmlInfo._xmlImplementation;

        /// <summary>
        /// The xml document
        /// </summary>
        internal XmlDocument		xmlDocument;

		/// <summary>
		/// Construct an empty document
		/// </summary>
		public LayoutXmlInfo() {
			xmlDocument = _xmlImplementation.CreateDocument();
		}

		/// <summary>
		/// Create XmlInfo with a copy of the content of a XML document of another object
		/// </summary>
		/// <param name="layoutObject">The object to copy</param>
		public LayoutXmlInfo(LayoutObject layoutObject) {
			xmlDocument = layoutObject.XmlInfo.XmlDocument.Implementation.CreateDocument();

			xmlDocument = (XmlDocument)layoutObject.XmlInfo.XmlDocument.CloneNode(true);
		}

		public LayoutXmlInfo(XmlElement element) {
			xmlDocument.ImportNode(element, true);
		}

		public XmlDocument XmlDocument {
			get {
				return xmlDocument;
			}

			set {
				xmlDocument = value;
			}
		}

        public XmlElement DocumentElement => xmlDocument.DocumentElement;

        public XmlElement Element => DocumentElement;

        /// <summary>
        /// Return a unique ID for this XML document. If the document has no unique ID assigned to it, 
        /// a new ID will be created.
        /// </summary>
        public Guid Id {
			get {
				XmlAttribute	idAttribute = (XmlAttribute)DocumentElement.Attributes.GetNamedItem("ID");

				if(idAttribute == null) {
					Guid	id = Guid.NewGuid();

					DocumentElement.SetAttribute("ID", XmlConvert.ToString(id));
					return id;
				}
				else
					return new Guid(idAttribute.Value);
			}

			set {
				DocumentElement.SetAttribute("ID", XmlConvert.ToString(value));
			}
		}
	}

	/// <summary>
	/// An object with an attached XML document.
	/// </summary>
	/// <remarks> This is the base class for many of the Layout Manager objects</remarks>
	public class LayoutObject : IObjectHasXml {
		/// <summary>
		/// The Object's Xml document
		/// </summary>
		LayoutXmlInfo	_xmlInfo = new LayoutXmlInfo();

		/// <summary>
		/// The object's XML document
		/// </summary>
		public LayoutXmlInfo XmlInfo {
			get {
				return _xmlInfo;
			}

			set {
				_xmlInfo = value;
			}
		}

        public XmlDocument XmlDocument => _xmlInfo.XmlDocument;

        public XmlElement DocumentElement => _xmlInfo.DocumentElement;

        public XmlElement Element => _xmlInfo.Element;

        public LayoutObject() {
		}

		public LayoutObject(String xmlDocument) {
			_xmlInfo.XmlDocument.LoadXml(xmlDocument);
		}

		/// <summary>
		/// Return a unique ID for this component. A new ID is created if the component
		/// if the component did not already have a unique ID.
		/// </summary>
		public Guid Id {
			get {
				return _xmlInfo.Id;
			}

			set {
				_xmlInfo.Id = value;
			}
		}
	};
};
