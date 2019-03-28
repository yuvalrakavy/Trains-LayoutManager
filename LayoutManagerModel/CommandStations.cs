using System;
using System.Xml;

#nullable enable
namespace LayoutManager.Model {
    #region Address format

    /// <summary>
    /// What entity this address is for
    /// </summary>
    public enum AddressUsage {
        Locomotive,
        Turnout,
        Signal,
        TrackContact,
        TrainDetectionBlock,
    }

    /// <summary>
    /// Does this command support setting a specific function#
    /// 
    ///  None - feature is not supported
    ///  FunctionNumber - can set a function but without associated state
    ///  FunctionNumberAndBooleanState - can set function with an associated state (On/Off)
    /// </summary>
    public enum SetFunctionNumberSupport {
        None,
        FunctionNumber,
        FunctionNumberAndBooleanState,
    }

    /// <summary>
    /// Wrapper for Xml element returned by command station component when it is asked to return the address
    /// format for various usage. For example, the address definition UI control does that in order to adapt itself
    /// for the requirment for a specific command station
    /// </summary>
    public class AddressFormatInfo : LayoutInfo {

        public AddressFormatInfo(XmlElement element) : base(element) {
        }

        public AddressFormatInfo() {
            XmlDocument doc = LayoutXmlInfo.XmlImplementation.CreateDocument();

            Element = doc.CreateElement("AddressFormat");
            doc.AppendChild(Element);
        }

        public string Namespace {
            get {
                return GetOptionalAttribute("Namespace") ??  "Default";
            }

            set {
                SetAttribute("Namespace", value);
            }
        }

        public int UnitMin {
            get {
                return XmlConvert.ToInt32(GetOptionalAttribute("UnitMin") ??  "1");
            }

            set {
                SetAttribute("UnitMin", XmlConvert.ToString(value));
            }
        }

        public int UnitMax {
            get {
                return XmlConvert.ToInt32(GetOptionalAttribute("UnitMax") ??  "256");
            }

            set {
                SetAttribute("UnitMax", XmlConvert.ToString(value));
            }
        }

        public bool ShowSubunit {
            get {
                return XmlConvert.ToBoolean(GetOptionalAttribute("ShowSubunit") ??  "false");
            }

            set {
                SetAttribute("ShowSubunit", XmlConvert.ToString(value));
            }
        }

        public enum SubunitFormatValue {
            Number,
            Alphabet
        }

        public SubunitFormatValue SubunitFormat {
            get {
                return (SubunitFormatValue)Enum.Parse(typeof(SubunitFormatValue), GetOptionalAttribute("SubunitFormat") ??  "Number");
            }

            set {
                SetAttribute("SubunitFormat", value.ToString());
            }
        }
        public int SubunitMin {
            get {
                return XmlConvert.ToInt32(GetOptionalAttribute("SubunitMin") ??  "1");
            }

            set {
                SetAttribute("SubunitMin", XmlConvert.ToString(value));
            }
        }

        public int SubunitMax {
            get {
                return XmlConvert.ToInt32(GetOptionalAttribute("SubunitMax") ??  "256");
            }

            set {
                SetAttribute("SubunitMax", XmlConvert.ToString(value));
            }
        }
    }

    public class CommandStationSetFunctionNumberSupportInfo {

        public SetFunctionNumberSupport SetFunctionNumberSupport { get; set; } = SetFunctionNumberSupport.None;

        public int MinFunctionNumber { get; set; } = 1;

        public int MaxFunctionNumber { get; set; } = 12;
    }

    #endregion
}
