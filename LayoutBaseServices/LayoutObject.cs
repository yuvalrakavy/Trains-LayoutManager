using System;
using System.Xml;
using System.Diagnostics;

#nullable enable
namespace LayoutManager {
    public interface IObjectHasXml {
        XmlElement Element { get; }
        XmlElement? OptionalElement { get;  }
    }

    public struct XmlElementWrapper : IObjectHasXml {
        public XmlElementWrapper(XmlElement element) {
            this.OptionalElement = element;
        }

        public XmlElement? OptionalElement { get; private set; }
        public XmlElement Element => Ensure.NotNull<XmlElement>(OptionalElement, nameof(OptionalElement));

        public static implicit operator XmlElement(XmlElementWrapper e) => e.Element;
    }

    public static class IObjectHasXmlExtensions {
        public static ConvertableString AttributeValue(this IObjectHasXml xmlObject, string name) {
            if (xmlObject.OptionalElement == null)
                return new ConvertableString(null, name, xmlObject.OptionalElement);

            if (xmlObject.OptionalElement.Attributes[name] == null)
                return new ConvertableString(null, name, xmlObject.OptionalElement);
            return new ConvertableString(xmlObject.OptionalElement.GetAttribute(name), name, xmlObject.Element);
        }


        public static void SetAttribute(this IObjectHasXml xmlObject, string name, string? v) {
            Debug.Assert(xmlObject.OptionalElement != null);
            if (v == null)
                xmlObject.OptionalElement.RemoveAttribute(name);
            else
                xmlObject.OptionalElement.SetAttribute(name, v);
        }

        public static void SetAttribute(this IObjectHasXml xmlObject, string name, string? v, string? removeIf) {
            Debug.Assert(xmlObject.OptionalElement != null);
            if (v == null || v == removeIf)
                xmlObject.OptionalElement.RemoveAttribute(name);
            else
                xmlObject.OptionalElement.SetAttribute(name, v);
        }

        public static void SetAttribute(this IObjectHasXml xmlObject, string name, int v, int removeIf) {
            Debug.Assert(xmlObject.OptionalElement != null);
            if (v == removeIf)
                xmlObject.OptionalElement.RemoveAttribute(name);
            else
                xmlObject.OptionalElement.SetAttribute(name, XmlConvert.ToString(v));
        }

        public static void SetAttribute(this IObjectHasXml xmlObject, string name, int v) {
            Debug.Assert(xmlObject.OptionalElement != null);
            xmlObject.OptionalElement.SetAttribute(name, XmlConvert.ToString(v));
        }

        public static void SetAttribute(this IObjectHasXml xmlObject, string name, double v) {
            Debug.Assert(xmlObject.OptionalElement != null);
            xmlObject.OptionalElement.SetAttribute(name, XmlConvert.ToString(v));
        }

        public static void SetAttribute(this IObjectHasXml xmlObject, string name, Guid v) {
            Debug.Assert(xmlObject.OptionalElement != null);
            xmlObject.OptionalElement.SetAttribute(name, XmlConvert.ToString(v));
        }

        public static void SetAttribute(this IObjectHasXml xmlObject, string name, Guid v, Guid removeIf) {
            Debug.Assert(xmlObject.OptionalElement != null);
            if (v == removeIf)
                xmlObject.OptionalElement.RemoveAttribute(name);
            else
                xmlObject.OptionalElement.SetAttribute(name, XmlConvert.ToString(v));
        }

        public static void SetAttribute(this IObjectHasXml xmlObject, string name, bool v) {
            Debug.Assert(xmlObject.OptionalElement != null);
            xmlObject.OptionalElement.SetAttribute(name, XmlConvert.ToString(v));
        }

        public static void SetAttribute(this IObjectHasXml xmlObject, string name, bool v, bool removeIf) {
            Debug.Assert(xmlObject.OptionalElement != null);
            if (v == removeIf)
                xmlObject.OptionalElement.RemoveAttribute(name);
            else
                xmlObject.OptionalElement.SetAttribute(name, XmlConvert.ToString(v));
        }

        public static bool HasAttribute(this IObjectHasXml xmlObject, string name) => xmlObject.Element.HasAttribute(name);
    }

    public struct ConvertableString {
        readonly string? v;
        readonly Func<string> getDescription;

        public ConvertableString(string? v, string attributeName, XmlElement? element) {
            this.v = v;

            this.getDescription = () => {
                const int maxElementStringLength = 30;

                string elementAsString = element?.OuterXml ?? "[Null] element";

                if (elementAsString.Length > maxElementStringLength)
                    elementAsString = elementAsString.Substring(0, maxElementStringLength) + "...";

                return $"Invalid or missing value for attribute {attributeName} for ({elementAsString})";
            };
        }


        public ConvertableString(string? v, string desciption) {
            this.v = v;
            this.getDescription = () => desciption;
        }

        public ConvertableString MustExist() {
            if (v != null)
                return this;

            throw new ArgumentNullException(getDescription());
        }

        public string ValidString() {
            MustExist();
            Debug.Assert(v != null);

            return v;
        }

        public static implicit operator string? (ConvertableString s) {
            return s.v;
        }

#pragma warning disable CS8629
        public static explicit operator int(ConvertableString s) => ((int?)s.MustExist()).Value;
        public static explicit operator bool(ConvertableString s) => ((bool?)s.MustExist()).Value;
        public static explicit operator double(ConvertableString s) => ((double?)s.MustExist()).Value;
        public static explicit operator float(ConvertableString s) => ((float?)s.MustExist()).Value;
        public static explicit operator Guid(ConvertableString s) => ((Guid?)s.MustExist()).Value;
#pragma warning restore CS8629

        public static explicit operator int? (ConvertableString s) {
            if (s.v == null)
                return null;

            return XmlConvert.ToInt32(s.v);
        }

        public static explicit operator bool? (ConvertableString s) {
            if (s.v == null)
                return null;

            return XmlConvert.ToBoolean(s.v);
        }

        public static explicit operator double? (ConvertableString s) {
            if (s.v == null)
                return null;

            return XmlConvert.ToDouble(s.v);
        }

        public static explicit operator float? (ConvertableString s) {
            if (s.v == null)
                return null;

            return (float)XmlConvert.ToDouble(s.v);
        }

        public static explicit operator Guid? (ConvertableString s) {
            if (s.v == null)
                return null;

            return XmlConvert.ToGuid(s.v);
        }

        public T? Enum<T>() where T : struct {
            if (v == null)
                return null;
            else
                return (T)System.Enum.Parse(typeof(T), v);
        }
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
        private static readonly XmlImplementation _xmlImplementation = new XmlImplementation();

        public static XmlImplementation XmlImplementation => LayoutXmlInfo._xmlImplementation;

        /// <summary>
        /// The xml document
        /// </summary>
        internal XmlDocument xmlDocument;

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
        public XmlElement? OptionalElement => DocumentElement;

        /// <summary>
        /// Return a unique ID for this XML document. If the document has no unique ID assigned to it, 
        /// a new ID will be created.
        /// </summary>
        public Guid Id {
            get {
                XmlAttribute idAttribute = (XmlAttribute)DocumentElement.Attributes.GetNamedItem("ID");

                if (idAttribute == null) {
                    Guid id = Guid.NewGuid();

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
        LayoutXmlInfo _xmlInfo = new LayoutXmlInfo();

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
        public XmlElement? OptionalElement => Element;

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
