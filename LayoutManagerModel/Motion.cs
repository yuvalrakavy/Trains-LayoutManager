using System;
using System.Collections.Generic;
using System.Xml;

#nullable enable
namespace LayoutManager.Model {
    public enum MotionRampType {
        /// <summary>
        /// The speed change will take this amount of time
        /// </summary>
        TimeFixed,

        /// <summary>
        /// The speed change will be at this rate (speed change commands/second)
        /// </summary>
        RateFixed,

        /// <summary>
        /// Just set the new speeed, the locomotive's hardware will do the acceleration/deceleration
        /// </summary>
        LocomotiveHardware,
    }

    public class MotionRampInfo : LayoutInfo {
        private const string A_Name = "Name";
        private const string A_Role = "Role";
        private const string A_UseInTrainController = "UseInTrainController";
        private const string A_RampType = "RampType";
        private const string A_MotionTime = "MotionTime";
        private const string A_SpeedChangeRate = "SpeedChangeRate";
        private const string E_Ramp = "Ramp";

        public MotionRampInfo(XmlElement element) : base(element) {
        }

        public MotionRampInfo(XmlNode parent, MotionRampType rampType, int parameter) {
            Initialize(parent, rampType, parameter);
        }

        public MotionRampInfo(MotionRampType rampType, int parameter) {
            XmlDocument doc = LayoutXmlInfo.XmlImplementation.CreateDocument();

            Initialize(doc, rampType, parameter);
        }

        public string Name {
            get => GetOptionalAttribute(A_Name) ?? "";
            set => SetAttribute(A_Name, value);
        }

        public string? Role {
            get => GetOptionalAttribute(A_Role);
            set => SetAttribute(A_Role, value, removeIf: null);
        }

        public bool UseInTrainControllerDialog {
            get => (bool?)AttributeValue(A_UseInTrainController) ?? false;
            set => SetAttribute(A_UseInTrainController, value, removeIf: false);
        }

        public MotionRampType RampType {
            get => AttributeValue(A_RampType).Enum<MotionRampType>() ?? MotionRampType.RateFixed;
            set => SetAttribute(A_RampType, value);
        }

        /// <summary>
        /// The total time of the speed change, the value is given in milli-seconds
        /// </summary>
        public int MotionTime {
            get => (int?)AttributeValue(A_MotionTime) ?? 0;
            set => SetAttribute(A_MotionTime, value);
        }

        public int SpeedChangeRate {
            get => (int?)AttributeValue(A_SpeedChangeRate) ?? 0;
            set => SetAttribute(A_SpeedChangeRate, value);
        }

        public string Description {
            get {
                if (Element.HasAttribute(A_Name))
                    return Name + " (" + GetRampDescription() + ")";
                else
                    return GetRampDescription();
            }
        }

        private string GetRampDescription() {
            if (RampType == MotionRampType.RateFixed)
                return "Rate " + SpeedChangeRate.ToString() + " steps/second";
            else if (RampType == MotionRampType.TimeFixed) {
                double t = (double)MotionTime / 1000;

                return "Length " + t.ToString() + " seconds";
            }
            else if (RampType == MotionRampType.LocomotiveHardware)
                return "locomotive native value";
            else
                return "*Unknown Ramp Type*";
        }

        protected void Initialize(XmlNode parent, MotionRampType rampType, int parameter) {
            if (!(parent is XmlDocument doc))
                doc = parent.OwnerDocument;

            Element = doc.CreateElement(E_Ramp);

            RampType = rampType;
            if (rampType == MotionRampType.TimeFixed)
                MotionTime = parameter;
            else if (rampType == MotionRampType.RateFixed)
                SpeedChangeRate = parameter;
            else if (rampType != MotionRampType.LocomotiveHardware)
                throw new ArgumentException("Invalid motion ramp type" + rampType);

            parent.AppendChild(Element);
        }
    }

    public class MotionRampCollection : XmlCollection<MotionRampInfo>, ICollection<MotionRampInfo> {
        public MotionRampCollection(XmlElement motionRampsElement) : base(motionRampsElement) {
        }

        protected override XmlElement CreateElement(MotionRampInfo item) {
            throw new NotImplementedException();
        }

        protected override MotionRampInfo FromElement(XmlElement itemElement) => new MotionRampInfo(itemElement);
    }

    public class TrainSpeedChangeParameters {
        public TrainSpeedChangeParameters(int requestedSpeed, MotionRampInfo ramp) {
            this.RequestedSpeed = requestedSpeed;
            this.Ramp = ramp;
        }

        public int RequestedSpeed { get; }

        public MotionRampInfo Ramp { get; }
    }

    /// <summary>
    /// This structure is returned by command station in response to get-command-station-capabilities event
    /// </summary>
    public class CommandStationCapabilitiesInfo : LayoutInfo {
        private const string E_CommandStationCapabilities = "CommandStationCapabilities";
        private const string A_MinTimeBetweenSpeedSteps = "MinTimeBetweenSpeedSteps";

        public CommandStationCapabilitiesInfo(XmlElement element) : base(element) {
        }

        public CommandStationCapabilitiesInfo() {
            CreateDocument();
        }

        protected void CreateDocument() {
            XmlDocument doc = LayoutXmlInfo.XmlImplementation.CreateDocument();

            Element = doc.CreateElement(E_CommandStationCapabilities);
            doc.AppendChild(Element);
        }

        public int MinTimeBetweenSpeedSteps {
            get => (int?)AttributeValue(A_MinTimeBetweenSpeedSteps) ?? 0;
            set => SetAttribute(A_MinTimeBetweenSpeedSteps, value);
        }
    }

    /// <summary>
    /// Holds the various defaults for driver parameters
    /// </summary>
    public class DefaultDriverParametersInfo : LayoutStateInfoBase {
        private const string A_SpeedLimit = "SpeedLimit";
        private const string A_SlowDownSpeed = "SlowDownSpeed";

        public DefaultDriverParametersInfo(XmlElement element) : base(element) {
            if (element.ChildNodes.Count == 0)
                initialize();
        }

        /// <summary>
        /// Initialize with default values
        /// </summary>
        private void initialize() {
            int speedSteps = LayoutModel.Instance.LogicalSpeedSteps;

            MotionRampInfo acceleration = new MotionRampInfo(Element, MotionRampType.RateFixed, speedSteps / 2);
            MotionRampInfo deceleration = new MotionRampInfo(Element, MotionRampType.RateFixed, speedSteps / 2);
            MotionRampInfo slowdown = new MotionRampInfo(Element, MotionRampType.RateFixed, speedSteps / 3);
            MotionRampInfo stop = new MotionRampInfo(Element, MotionRampType.RateFixed, speedSteps);

            acceleration.Role = "Acceleration";
            deceleration.Role = "Deceleration";
            slowdown.Role = "SlowDown";
            stop.Role = "Stop";

            SpeedLimit = speedSteps;
            SlowdownSpeed = speedSteps / 4;
        }

        public int SpeedLimit {
            get => (int?)AttributeValue(A_SpeedLimit) ?? 0;
            set => SetAttribute(A_SpeedLimit, value);
        }

        public int SlowdownSpeed {
            get => (int?)AttributeValue(A_SlowDownSpeed) ?? 1;
            set => SetAttribute(A_SlowDownSpeed, value);
        }

        private MotionRampInfo? getRamp(string role) {
            XmlElement rampElement = (XmlElement)Element.SelectSingleNode("Ramp[@Role='" + role + "']");

            return rampElement == null ? null : new MotionRampInfo(rampElement);
        }

        public MotionRampInfo? AccelerationProfile => getRamp("Acceleration");

        public MotionRampInfo? DecelerationProfile => getRamp("Deceleration");

        public MotionRampInfo? SlowdownProfile => getRamp("SlowDown");

        public MotionRampInfo? StopProfile => getRamp("Stop");
    }
}
