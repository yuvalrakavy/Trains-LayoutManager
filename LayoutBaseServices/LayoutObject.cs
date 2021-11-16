using System;
using System.Xml;
using System.Diagnostics;

#nullable enable
namespace LayoutManager {
    public interface IObjectHasXml {
        XmlElement Element { get; }
        XmlElement? OptionalElement { get; }
    }

    public struct XmlElementWrapper : IObjectHasXml {
        public XmlElementWrapper(XmlElement? element) {
            this.OptionalElement = element;
        }

        public XmlElement? OptionalElement { get; set; }
        public XmlElement Element => Ensure.NotNull<XmlElement>(OptionalElement, nameof(OptionalElement));

        public static implicit operator XmlElement(XmlElementWrapper e) => e.Element;
    }

    public static class IObjectHasXmlExtensions {
        public static ConvertableString AttributeValue(this IObjectHasXml xmlObject, string name) => xmlObject.OptionalElement.AttributeValue(name);

        public static void SetAttributeValue(this IObjectHasXml xmlObject, string name, string? v) {
            if (xmlObject.OptionalElement == null)
                throw new ArgumentNullException($"Trying to set attribute {name} to {v ?? "(null)"} for XmlElement which is null");

            if (v == null)
                xmlObject.OptionalElement?.RemoveAttribute(name);
            else
                xmlObject.OptionalElement?.SetAttribute(name, v);
        }

        public static void SetAttributeValue(this IObjectHasXml xmlObject, string name, string? v, string? removeIf) => xmlObject.Element.SetAttributeValue(name, v, removeIf);

        public static void SetAttributeValue(this IObjectHasXml xmlObject, string name, int v, int removeIf) => xmlObject.Element.SetAttributeValue(name, v, removeIf);

        public static void SetAttributeValue(this IObjectHasXml xmlObject, string name, UInt32 v) => xmlObject.Element.SetAttributeValue(name, v);
        public static void SetAttributeValue(this IObjectHasXml xmlObject, string name, UInt32 v, UInt32 removeIf) => xmlObject.Element.SetAttributeValue(name, v, removeIf);

        public static void SetAttributeValue(this IObjectHasXml xmlObject, string name, int v) => xmlObject.Element.SetAttributeValue(name, v);
        public static void SetAttributeValue(this IObjectHasXml xmlObject, string name, long v) => xmlObject.Element.SetAttributeValue(name, v);

        public static void SetAttributeValue(this IObjectHasXml xmlObject, string name, double v) => xmlObject.Element.SetAttributeValue(name, v);

        public static void SetAttributeValue(this IObjectHasXml xmlObject, string name, Guid v) => xmlObject.Element.SetAttributeValue(name, v);

        public static void SetAttributeValue(this IObjectHasXml xmlObject, string name, Guid v, Guid removeIf) => xmlObject.Element.SetAttributeValue(name, v, removeIf);

        public static void SetAttributeValue(this IObjectHasXml xmlObject, string name, bool v) => xmlObject.Element.SetAttributeValue(name, v);

        public static void SetAttributeValue(this IObjectHasXml xmlObject, string name, bool v, bool removeIf) => xmlObject.Element.SetAttributeValue(name, v, removeIf);

        public static void SetAttributeValue(this IObjectHasXml xmlObject, string name, Enum e) => xmlObject.Element.SetAttributeValue(name, e);
        public static void SetAttributeValue(this IObjectHasXml xmlObject, string name, Enum e, Enum removeIf) => xmlObject.Element.SetAttributeValue(name, e, removeIf);

        public static bool HasAttribute(this IObjectHasXml xmlObject, string name) => xmlObject.Element.HasAttribute(name);
    }

    public static class XmlElementExtensions {
        public static ConvertableString AttributeValue(this XmlElement? e, string name) {
            return e == null || e.Attributes[name] == null
                ? new ConvertableString(null, name, e)
                : new ConvertableString(e.GetAttribute(name), name, e);
        }

        public static void SetAttributeValue(this XmlElement e, string name, string? v, string? removeIf) {
            if (v == null || v == removeIf)
                e.RemoveAttribute(name);
            else
                e.SetAttribute(name, v);
        }

        public static void SetAttributeValue(this XmlElement e, string name, int v, int removeIf) {
            if (v == removeIf)
                e.RemoveAttribute(name);
            else
                e.SetAttribute(name, XmlConvert.ToString(v));
        }

        public static void SetAttributeValue(this XmlElement e, string name, int v) => e.SetAttribute(name, XmlConvert.ToString(v));

        public static void SetAttributeValue(this XmlElement e, string name, UInt32 v, UInt32 removeIf) {
            if (v == removeIf)
                e.RemoveAttribute(name);
            else
                e.SetAttribute(name, XmlConvert.ToString(v));
        }

        public static void SetAttributeValue(this XmlElement e, string name, UInt32 v) => e.SetAttribute(name, XmlConvert.ToString(v));

        public static void SetAttributeValue(this XmlElement e, string name, long v) => e.SetAttribute(name, XmlConvert.ToString(v));

        public static void SetAttributeValue(this XmlElement e, string name, byte v) => e.SetAttribute(name, XmlConvert.ToString(v));

        public static void SetAttributeValue(this XmlElement e, string name, double v) => e.SetAttribute(name, XmlConvert.ToString(v));

        public static void SetAttributeValue(this XmlElement e, string name, decimal v) => e.SetAttribute(name, XmlConvert.ToString(v));

        public static void SetAttributeValue(this XmlElement e, string name, Guid v) => e.SetAttribute(name, XmlConvert.ToString(v));

        public static void SetAttributeValue(this XmlElement e, string name, Guid v, Guid removeIf) {
            if (v == removeIf)
                e.RemoveAttribute(name);
            else
                e.SetAttribute(name, XmlConvert.ToString(v));
        }

        public static void SetAttributeValue(this XmlElement e, string name, bool v) => e.SetAttribute(name, XmlConvert.ToString(v));

        public static void SetAttributeValue(this XmlElement e, string name, bool v, bool removeIf) {
            if (v == removeIf)
                e.RemoveAttribute(name);
            else
                e.SetAttribute(name, XmlConvert.ToString(v));
        }

        public static void SetAttributeValue(this XmlElement e, string name, Enum v) => e.SetAttribute(name, v.ToString());

        public static void SetAttributeValue(this XmlElement e, string name, Enum v, Enum removeIf) {
            if (v == removeIf)
                e.RemoveAttribute(name);
            else
                e.SetAttribute(name, v.ToString());
        }
    }

    public struct ConvertableString {
        private readonly string? v;
        private readonly Func<string> getDescription;

        public ConvertableString(string? v, string attributeName, XmlElement? element) {
            this.v = v;

            this.getDescription = () => {
                const int maxElementStringLength = 30;

                string elementAsString = element?.OuterXml ?? "[Null] element";

                if (elementAsString.Length > maxElementStringLength)
                    elementAsString = string.Concat(elementAsString.AsSpan(0, maxElementStringLength), "...");

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

            return v!;
        }

        public static explicit operator string?(ConvertableString s) {
            return s.v;
        }

#pragma warning disable CS8629
        public static explicit operator int(ConvertableString s) => ((int?)s.MustExist()).Value;
        public static explicit operator uint(ConvertableString s) => ((uint?)s.MustExist()).Value;
        public static explicit operator Int64(ConvertableString s) => ((Int64?)s.MustExist()).Value;
        public static explicit operator byte(ConvertableString s) => ((byte?)s.MustExist()).Value;
        public static explicit operator bool(ConvertableString s) => ((bool?)s.MustExist()).Value;
        public static explicit operator double(ConvertableString s) => ((double?)s.MustExist()).Value;
        public static explicit operator float(ConvertableString s) => ((float?)s.MustExist()).Value;
        public static explicit operator Guid(ConvertableString s) => ((Guid?)s.MustExist()).Value;
#pragma warning restore CS8629

        public static explicit operator int?(ConvertableString s) {
            return s.v == null ? null : (int?)XmlConvert.ToInt32(s.v);
        }

        public static explicit operator uint?(ConvertableString s) {
            return s.v == null ? null : (uint?)XmlConvert.ToUInt32(s.v);
        }

        public static explicit operator Int64?(ConvertableString s) {
            return s.v == null ? null : (long?)XmlConvert.ToInt64(s.v);
        }

        public static explicit operator byte?(ConvertableString s) {
            return s.v == null ? null : (byte?)XmlConvert.ToByte(s.v);
        }

        public static explicit operator bool?(ConvertableString s) {
            return s.v == null ? null : (bool?)XmlConvert.ToBoolean(s.v);
        }

        public static explicit operator double?(ConvertableString s) {
            return s.v == null ? null : (double?)XmlConvert.ToDouble(s.v);
        }

        public static explicit operator float?(ConvertableString s) {
            return s.v == null ? null : (float?)(float)XmlConvert.ToDouble(s.v);
        }

        public static explicit operator Guid?(ConvertableString s) {
            return s.v == null ? null : (Guid?)XmlConvert.ToGuid(s.v);
        }

        public static explicit operator decimal?(ConvertableString s) {
            return s.v == null ? null : (decimal?)XmlConvert.ToDecimal(s.v);
        }

        public T? Enum<T>() where T : struct {
            return v == null ? null : (T?)(T)System.Enum.Parse(typeof(T), v);
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
        private static readonly XmlImplementation _xmlImplementation = new();

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

        public XmlElement DocumentElement => Ensure.NotNull<XmlElement>(xmlDocument.DocumentElement);

        public XmlElement Element => DocumentElement;
        public XmlElement? OptionalElement => DocumentElement;

        /// <summary>
        /// Return a unique ID for this XML document. If the document has no unique ID assigned to it, 
        /// a new ID will be created.
        /// </summary>
        public Guid Id {
            get {
                var idAttribute = (XmlAttribute?)DocumentElement.Attributes.GetNamedItem("ID");

                if (idAttribute == null) {
                    Guid id = Guid.NewGuid();

                    DocumentElement.SetAttributeValue("ID", id);
                    return id;
                }
                else
                    return new Guid(idAttribute.Value);
            }

            set => DocumentElement.SetAttributeValue("ID", value);
        }
    }

    public static class XmlWriterExtensions {
        public static void WriteAttributeString(this XmlWriter w, string name, bool v) => w.WriteAttributeString(name, XmlConvert.ToString(v));
        public static void WriteAttributeString(this XmlWriter w, string name, int v) => w.WriteAttributeString(name, XmlConvert.ToString(v));
        public static void WriteAttributeString(this XmlWriter w, string name, float v) => w.WriteAttributeString(name, XmlConvert.ToString(v));
        public static void WriteAttributeString(this XmlWriter w, string name, Guid v) => w.WriteAttributeString(name, XmlConvert.ToString(v));
        public static void WriteAttributeString(this XmlWriter w, string name, Enum v) => w.WriteAttributeString(name, v.ToString());
    }

    /// <summary>
    /// An object with an attached XML document.
    /// </summary>
    /// <remarks> This is the base class for many of the Layout Manager objects</remarks>
    public class LayoutObject : IObjectHasXml {
        /// <summary>
        /// The object's XML document
        /// </summary>
        public LayoutXmlInfo XmlInfo { get; set; } = new LayoutXmlInfo();

        public XmlDocument XmlDocument => XmlInfo.XmlDocument;

        public XmlElement DocumentElement => XmlInfo.DocumentElement;

        public XmlElement Element => XmlInfo.Element;
        public XmlElement? OptionalElement => Element;

        public LayoutObject() {
        }

        public LayoutObject(String xmlDocument) {
            XmlInfo.XmlDocument.LoadXml(xmlDocument);
        }

        /// <summary>
        /// Return a unique ID for this component. A new ID is created if the component
        /// if the component did not already have a unique ID.
        /// </summary>
        public Guid Id {
            get {
                return XmlInfo.Id;
            }

            set {
                XmlInfo.Id = value;
            }
        }
    };
};
