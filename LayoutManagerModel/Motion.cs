using System;
using System.Collections.Generic;
using System.Xml;

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
		public MotionRampInfo(XmlElement element) : base(element) {
		}

		public MotionRampInfo(XmlNode parent, MotionRampType rampType, int parameter) {
			Initialize(parent, rampType, parameter);
		}

		public MotionRampInfo(MotionRampType rampType, int parameter) {
			XmlDocument	doc = LayoutXmlInfo.XmlImplementation.CreateDocument();

			Initialize(doc, rampType, parameter);
		}

		public string Name {
			get {
				return GetAttribute("Name", "");
			}

			set {
				SetAttribute("Name", value);
			}
		}

		public string Role {
			get {
				return GetAttribute("Role", null);
			}

			set {
                SetAttribute("Role", value, removeIf: null);
			}
		}

		public bool UseInTrainControllerDialog {
			get {
				return XmlConvert.ToBoolean(GetAttribute("UseInTrainController", "false"));
			}

			set {
                SetAttribute("UseInTrainController", value, removeIf: false);
			}
		}
		
		public MotionRampType RampType {
			get {
				return (MotionRampType)Enum.Parse(typeof(MotionRampType), GetAttribute("RampType"));
			}

			set {
				SetAttribute("RampType", value.ToString());
			}
		}
		
		/// <summary>
		/// The total time of the speed change, the value is given in milli-seconds
		/// </summary>
		public int MotionTime {
			get {
				return XmlConvert.ToInt32(GetAttribute("MotionTime", "0"));
			}

			set {
				SetAttribute("MotionTime", XmlConvert.ToString(value));
			}
		}


		public int SpeedChangeRate {
			get {
				return XmlConvert.ToInt32(GetAttribute("SpeedChangeRate", "0"));
			}

			set {
				SetAttribute("SpeedChangeRate", XmlConvert.ToString(value));
			}
		}

		public string Description {
			get {
				if(Element.HasAttribute("Name"))
					return Name + " (" + GetRampDescription() + ")";
				else
					return GetRampDescription();
			}
		}

		private string GetRampDescription() {
			if(RampType == MotionRampType.RateFixed)
				return "Rate " + SpeedChangeRate.ToString() + " steps/second";
			else if(RampType == MotionRampType.TimeFixed) {
				double	t = (double)MotionTime / 1000;

				return "Length " + t.ToString() + " seconds";
			}
			else if(RampType == MotionRampType.LocomotiveHardware)
				return "locomotive native value";
			else
				return "*Unknown Ramp Type*";
		}

		protected void Initialize(XmlNode parent, MotionRampType rampType, int parameter) {
			XmlDocument doc = parent as XmlDocument;

			if(doc == null)
				doc = parent.OwnerDocument;

			Element = doc.CreateElement("Ramp");

			RampType = rampType;
			if(rampType == MotionRampType.TimeFixed)
				MotionTime = parameter;
			else if(rampType == MotionRampType.RateFixed)
				SpeedChangeRate = parameter;
			else if(rampType != MotionRampType.LocomotiveHardware)
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
		int				requestedSpeed;
		MotionRampInfo	ramp;

		public TrainSpeedChangeParameters(int requestedSpeed, MotionRampInfo ramp) {
			this.requestedSpeed = requestedSpeed;
			this.ramp = ramp;
		}

        public int RequestedSpeed => requestedSpeed;

        public MotionRampInfo Ramp => ramp;
    }

	/// <summary>
	/// This structure is returned by command station in response to get-command-station-capabilities event
	/// </summary>
	public class CommandStationCapabilitiesInfo : LayoutInfo {
		public CommandStationCapabilitiesInfo(XmlElement element) : base(element) {
		}

		public CommandStationCapabilitiesInfo() {
			CreateDocument();
		}

		protected void CreateDocument() {
			XmlDocument	doc = LayoutXmlInfo.XmlImplementation.CreateDocument();
			
			Element = doc.CreateElement("CommandStationCapabilities");
			doc.AppendChild(Element);
		}

		public int MinTimeBetweenSpeedSteps {
			get {
				return XmlConvert.ToInt32(Element.GetAttribute("MinTimeBetweenSpeedSteps"));
			}

			set {
				SetAttribute("MinTimeBetweenSpeedSteps", XmlConvert.ToString(value));
			}
		}
	}

	/// <summary>
	/// Holds the various defaults for driver parameters
	/// </summary>
	public class DefaultDriverParametersInfo : LayoutStateInfoBase {

		public DefaultDriverParametersInfo(LayoutStateManager stateManager, XmlElement element) : base(element) {
			if(element.ChildNodes.Count == 0)
				initialize();
		}

		/// <summary>
		/// Initialize with default values
		/// </summary>
		private void initialize() {
			int	speedSteps = LayoutModel.Instance.LogicalSpeedSteps;

			MotionRampInfo	acceleration = new MotionRampInfo(Element, MotionRampType.RateFixed, speedSteps / 2);
			MotionRampInfo	deceleration = new MotionRampInfo(Element, MotionRampType.RateFixed, speedSteps / 2);
			MotionRampInfo	slowdown = new MotionRampInfo(Element, MotionRampType.RateFixed, speedSteps / 3);
			MotionRampInfo	stop = new MotionRampInfo(Element, MotionRampType.RateFixed,  speedSteps);

			acceleration.Role = "Acceleration";
			deceleration.Role = "Deceleration";
			slowdown.Role = "SlowDown";
			stop.Role = "Stop";

			SpeedLimit = speedSteps;
			SlowdownSpeed = speedSteps / 4;
		}

		public int SpeedLimit {
			get {
				return XmlConvert.ToInt32(GetAttribute("SpeedLimit", "0"));
			}

			set {
				SetAttribute("SpeedLimit", XmlConvert.ToString(value));
			}
		}

		public int SlowdownSpeed {
			get {
				return XmlConvert.ToInt32(GetAttribute("SlowDownSpeed", "1"));
			}

			set {
				SetAttribute("SlowDownSpeed", XmlConvert.ToString(value));
			}
		}

		private MotionRampInfo getRamp(string role) {
			XmlElement	rampElement = (XmlElement)Element.SelectSingleNode("Ramp[@Role='" + role + "']");

			if(rampElement == null)
				return null;
			else
				return new MotionRampInfo(rampElement);
		}

        public MotionRampInfo AccelerationProfile => getRamp("Acceleration");

        public MotionRampInfo DecelerationProfile => getRamp("Deceleration");

        public MotionRampInfo SlowdownProfile => getRamp("SlowDown");

        public MotionRampInfo StopProfile => getRamp("Stop");
    }
}
