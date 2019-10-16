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
        private const string E_AddressFormat = "AddressFormat";
        private const string A_Namespace = "Namespace";
        private const string A_UnitMin = "UnitMin";
        private const string A_UnitMax = "UnitMax";
        private const string A_ShowSubunit = "ShowSubunit";
        private const string A_SubunitFormat = "SubunitFormat";
        private const string A_SubunitMin = "SubunitMin";
        private const string A_SubunitMax = "SubunitMax";

        public AddressFormatInfo(XmlElement element) : base(element) {
        }

        public AddressFormatInfo() {
            XmlDocument doc = LayoutXmlInfo.XmlImplementation.CreateDocument();

            Element = doc.CreateElement(E_AddressFormat);
            doc.AppendChild(Element);
        }

        public string Namespace {
            get => GetOptionalAttribute(A_Namespace) ?? "Default";
            set => SetAttributeValue(A_Namespace, value);
        }

        public int UnitMin {
            get => (int?)AttributeValue(A_UnitMin) ?? 1;
            set => SetAttributeValue(A_UnitMin, value);
        }

        public int UnitMax {
            get => (int?)AttributeValue(A_UnitMax) ?? 256;
            set => SetAttributeValue(A_UnitMax, value);
        }

        public bool ShowSubunit {
            get => (bool?)AttributeValue(A_ShowSubunit) ?? false;
            set => SetAttributeValue(A_ShowSubunit, value);
        }

        public enum SubunitFormatValue {
            Number,
            Alphabet
        }

        public SubunitFormatValue SubunitFormat {
            get => AttributeValue(A_SubunitFormat).Enum<SubunitFormatValue>() ?? SubunitFormatValue.Number;
            set => SetAttributeValue(A_SubunitFormat, value);
        }

        public int SubunitMin {
            get => (int?)AttributeValue(A_SubunitMin) ?? 1;
            set => SetAttributeValue(A_SubunitMin, value);
        }

        public int SubunitMax {
            get => (int?)AttributeValue(A_SubunitMax) ?? 256;
            set => SetAttributeValue(A_SubunitMax, value);
        }
    }

    public class CommandStationSetFunctionNumberSupportInfo {
        public SetFunctionNumberSupport SetFunctionNumberSupport { get; set; } = SetFunctionNumberSupport.None;

        public int MinFunctionNumber { get; set; } = 1;

        public int MaxFunctionNumber { get; set; } = 12;
    }

    #endregion
}
