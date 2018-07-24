using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Threading.Tasks;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

namespace Intellibox {

    interface ICommandStationServices {
		void Event(LayoutEvent e);

		void Error(string message);
	}

    public class IntelliboxComponentInfo : LayoutInfo {
        IntelliboxComponent _commandStation;

        public IntelliboxComponentInfo(IntelliboxComponent commandStation, XmlElement element) : base(element) {
            this._commandStation = commandStation;
        }

        public IntelliboxComponent CommandStation => _commandStation;

        public string Port {
            get {
                return Element.GetAttribute("Port");
            }

            set {
                Element.SetAttribute("Port", value);
            }
        }

        public int PollingPeriod {
            get {
                if(Element.HasAttribute("PollingPeriod"))
                    return XmlConvert.ToInt32(Element.GetAttribute("PollingPeriod"));
                else
                    return 20;
            }

            set {
                Element.SetAttribute("PollingPeriod", XmlConvert.ToString(value));
            }
        }

        public int AccessoryCommandTime {
            get {
                if(Element.HasAttribute("AccessorySwitchingTime"))
                    return XmlConvert.ToInt32(Element.GetAttribute("AccessorySwitchingTime"));
                return 100;     // Default
            }

            set {
                Element.SetAttribute("AccessorySwitchingTime", XmlConvert.ToString(value));
            }
        }

		public byte OperationModeDebounceCount {
			get {
				if(Element.HasAttribute("OperationModeDebounceCount"))
					return XmlConvert.ToByte(Element.GetAttribute("OperationModeDebounceCount"));
				else
					return 1;
			}

			set {
				Element.SetAttribute("OperationModeDebounceCount", XmlConvert.ToString(value));
			}
		}

		public byte DesignTimeDebounceCount {
			get {
				if(Element.HasAttribute("DesignTimeDebounceCount"))
					return XmlConvert.ToByte(Element.GetAttribute("DesignTimeDebounceCount"));
				else
					return 1;
			}

			set {
				Element.SetAttribute("DesignTimeDebounceCount", XmlConvert.ToString(value));
			}
		}
    }

	/// <summary>
	/// Implementation of Intellibox interface
	/// </summary>
	public class IntelliboxComponent : LayoutCommandStationComponent {
		static LayoutTraceSwitch		traceIntellibox = new LayoutTraceSwitch("Intellibox", "Intellibox Command Station");
		OutputManager	commandStationManager;

		ControlBus				_motorolaBus = null;
		ControlBus				_S88bus = null;
        byte                    _operationModeDebounceCount;
        byte                    _designTimeDebounceCount;

		const int				queuePowerCommands = 0;				// Queue for power on/off etc.
		const int				queueLayoutSwitchingCommands = 1;	// Queue for turnout/lights control commands
		const int				queueLocoCommands = 2;				// Queue for locomotive control commands

		const int				switchBatchSize = 10;				// Change the state of upto 10 solenoids

		public IntelliboxComponent() {
			this.XmlDocument.LoadXml(
				"<CommandStation PollingPeriod='20'>" +
				"<ModeString>baud=19200 parity=N data=8 stop=2 octs=off odsr=off</ModeString>" +
				"</CommandStation>"
				);

			new SOcollection(Element).Add(33, 0, "Disable echo to I2C bus");
		}

        public IntelliboxComponentInfo Info => new IntelliboxComponentInfo(this, Element);

        public ControlBus MotorolaBus {
			get {
				if(_motorolaBus == null)
					_motorolaBus = LayoutModel.ControlManager.Buses.GetBus(this, "Motorola");
				return _motorolaBus;
			}
		}

		public ControlBus S88Bus {
			get {
				if(_S88bus == null)
					_S88bus = LayoutModel.ControlManager.Buses.GetBus(this, "S88BUS");
				return _S88bus;
			}
		}

        internal byte CachedOperationModeDebounceCount {
            get {
                return _operationModeDebounceCount;
            }

            set {
                _operationModeDebounceCount = value;
            }
        }

        internal byte CachedDesignTimeDebounceCount {
            get {
                return _designTimeDebounceCount;
            }

            set {
                _designTimeDebounceCount = value;
            }
        }


        #region Specific implementation to base class overrideble methods and properties

        public override IList<string> BusTypeNames => Array.AsReadOnly<string>(new string[] { "Motorola", "S88BUS" });

        //TODO: Add DigitalPowerFormats.NRMA when support for this is added. (Intellibox support mutiple on track formats)
        public override DigitalPowerFormats SupportedDigitalPowerFormats => DigitalPowerFormats.Motorola;

        protected override void OnCommunicationSetup() {
			base.OnCommunicationSetup ();

            CachedOperationModeDebounceCount = Info.OperationModeDebounceCount;
            CachedDesignTimeDebounceCount = Info.DesignTimeDebounceCount;

			if(!Element.HasAttribute("ReadIntervalTimeout"))
				Element.SetAttribute("ReadIntervalTimeout", XmlConvert.ToString(100));
			if(!Element.HasAttribute("ReadTotalTimeoutConstant"))
				Element.SetAttribute("ReadTotalTimeoutConstant", XmlConvert.ToString(5000));
			if(!Element.HasAttribute("BufferSize"))
				Element.SetAttribute("BufferSize", XmlConvert.ToString(1));
		}

		protected override void OnInitialize() {
			base.OnInitialize ();

			_motorolaBus = null;
			_S88bus = null;

			commandStationManager = new OutputManager(Name, 3);
			commandStationManager.Start();

			commandStationManager.AddIdleCommand(new InterlliboxProcessEventsCommand(this, Info.PollingPeriod));
			commandStationManager.AddIdleCommand(new EndTrainsAnalysisCommandStationCommand(this, 10));

			foreach(SOinfo so in new SOcollection(Element))
				commandStationManager.AddCommand(queuePowerCommands, new IntelliboxSetSOcommand(this, so.Number, so.Value));

			if(OperationMode)
				commandStationManager.AddCommand(queuePowerCommands, new IntelliboxForceSensorEventCommand(this));
		}

		protected override async Task OnTerminateCommunication() {
			await base.OnTerminateCommunication ();

			await commandStationManager.WaitForIdle();
			if(commandStationManager != null) {
				commandStationManager.Terminate();
				commandStationManager = null;
			}
		}

        public override bool LayoutEmulationSupported => false;

        public override bool DesignTimeLayoutActivationSupported => true;

        public override bool BatchMultipathSwitchingSupported => true;

        public override bool TrainsAnalysisSupported => true;

        protected override ILayoutCommandStationEmulator CreateCommandStationEmulator(string pipeName) => null;

        #endregion

        #region Request Event Handlers

        [LayoutEvent("get-command-station-capabilities", IfEvent="*[CommandStation/@Name='`string(Name)`']")]
		void GetCommandStationCapabilities(LayoutEvent e) {
			CommandStationCapabilitiesInfo	cap = new CommandStationCapabilitiesInfo();
			int								minTimeBetweenSpeedSteps = 100;

			if(Element.HasAttribute("MinTimeBetweenSpeedSteps"))
				minTimeBetweenSpeedSteps = XmlConvert.ToInt32(Element.GetAttribute("MinTimeBetweenSpeedSteps"));

			cap.MinTimeBetweenSpeedSteps = minTimeBetweenSpeedSteps;
			
			e.Info = cap.Element;
		}

		[LayoutEvent("disconnect-power-request")]
		void PowerDisconnectRequest(LayoutEvent e) {
			if(e.Sender == null || e.Sender == this)
				commandStationManager.AddCommand(queuePowerCommands, new IntelliboxPowerOffCommand(this));

			PowerOff();
		}

		[LayoutEvent("connect-power-request")]
		void PowerConnectRequest(LayoutEvent e) {
			if(e.Sender == null || e.Sender == this)
				commandStationManager.AddCommand(queuePowerCommands, new IntelliboxPowerOnCommand(this));

			PowerOn();
		}


		protected override void OnConnectTrackPower(LayoutEvent e) {
			if(e.Sender == this)
				commandStationManager.AddCommand(queuePowerCommands, new IntelliboxTrackPowerOnCommand(this));

			PowerOn();
		}

		// Implement command events
		[LayoutAsyncEvent("change-track-component-state-command", IfEvent="*[CommandStation/@ID='`string(@ID)`']")]
		Task ChangeTurnoutState(LayoutEvent e0) {
			var e = (LayoutEvent<ControlConnectionPointReference, int>)e0;
			ControlConnectionPointReference connectionPointRef = e.Sender;
			int							state = e.Info;
			int							address = connectionPointRef.Module.Address + connectionPointRef.Index;
			var							tasks = new List<Task>();

			tasks.Add(commandStationManager.AddCommand(queueLayoutSwitchingCommands, new IntelliboxAccessoryCommand(this, address, state)));
			tasks.Add(commandStationManager.AddCommand(queueLayoutSwitchingCommands, new IntelliboxEndAccessoryCommand(this, address, state)));

			if(OperationMode)
				EventManager.Event(new LayoutEvent(connectionPointRef, "control-connection-point-state-changed-notification", null, state));

			return Task.WhenAll(tasks);
		}

		[LayoutAsyncEvent("change-batch-of-track-component-state-command", IfEvent = "LayoutEvent[Options/@CommandStationID='`string(@ID)`']")]
		private Task ChangeBatchOfTurnoutStates(LayoutEvent e) {
			List<SwitchingCommand> switchingCommands = (List<SwitchingCommand>)e.Info;
			var tasks = new List<Task>();
			int count;

			for(int i = 0; i < switchingCommands.Count; i += count) {
				if(switchingCommands.Count - i > switchBatchSize)
					count = switchBatchSize;
				else
					count = switchingCommands.Count - i;

				tasks.Add(commandStationManager.AddCommand(queueLayoutSwitchingCommands, new IntelliboxAccessoryCommand(this, switchingCommands, i, count)));
				tasks.Add(commandStationManager.AddCommand(queueLayoutSwitchingCommands, new IntelliboxEndAccessoryCommand(this, switchingCommands, i, count)));

                if(OperationMode) {
                    for(int switchingCommandIndex = 0; switchingCommandIndex < count; switchingCommandIndex++) {
                        SwitchingCommand switchingCommand = switchingCommands[i + switchingCommandIndex];

                        EventManager.Event(new LayoutEvent(switchingCommand.ControlPointReference, "control-connection-point-state-changed-notification", null, switchingCommand.SwitchState));
                    }
                }
			}

			return Task.WhenAll(tasks);
		}

		[LayoutEvent("change-signal-state-command", IfEvent = "*[CommandStation/@ID='`string(@ID)`']")]
		private void changeSignalStateCommand(LayoutEvent e) {
			ControlConnectionPointReference connectionPointRef = (ControlConnectionPointReference)e.Sender;
			LayoutSignalState state = (LayoutSignalState)e.Info;
			int address = connectionPointRef.Module.Address + connectionPointRef.Index;
			byte v;

			if(state == LayoutSignalState.Green)
				v = 0;
			else
				v = 1;

			commandStationManager.AddCommand(queueLayoutSwitchingCommands, new IntelliboxAccessoryCommand(this, address, v));
			commandStationManager.AddCommand(queueLayoutSwitchingCommands, new IntelliboxEndAccessoryCommand(this, address, v));

			if(OperationMode)
				EventManager.Event(new LayoutEvent(connectionPointRef, "control-connection-point-state-changed-notification", null, state == LayoutSignalState.Green ? 1 : 0));
		}

		[LayoutEvent("locomotive-motion-command", IfEvent="*[CommandStation/@Name='`string(Name)`']")]
		private void locomotiveMotionCommand(LayoutEvent e) {
			LocomotiveInfo			loco = (LocomotiveInfo)e.Sender;
			int						speed = (int)e.Info;
			TrainStateInfo			train = LayoutModel.StateManager.Trains[loco.Id];
			LocomotiveOrientation	direction = LocomotiveOrientation.Forward;
			int						logicalSpeed;

			if(speed < 0) {
				speed = -speed;
				direction = LocomotiveOrientation.Backward;
			}

			if(speed == 0)
				logicalSpeed = 0;
			else {
				double	factor = 126 / loco.SpeedSteps;

				logicalSpeed = (int)(1 + speed * factor);
			}

			commandStationManager.AddCommand(queueLocoCommands, new IntelliboxLocomotiveCommand(this, loco.AddressProvider.Unit, logicalSpeed, direction, train.Lights));
		}

		[LayoutEvent("set-locomotive-lights-command", IfEvent="*[CommandStation/@Name='`string(Name)`']")]
		private void setLocomotiveLightsCommand(LayoutEvent e) {
			LocomotiveInfo	loco = (LocomotiveInfo)e.Sender;
			TrainStateInfo	train = LayoutModel.StateManager.Trains[loco.Id];

			EventManager.Event(new LayoutEvent(loco, "locomotive-motion-command", null, train.SpeedInSteps).SetCommandStation(train));
		}

		private byte getFunctionMask(LocomotiveInfo loco) {
			TrainStateInfo	train = LayoutModel.StateManager.Trains[loco.Id];
			byte			functionMask = 0;

			for(int functionNumber = 0; functionNumber < 8; functionNumber++) {
				LocomotiveFunctionInfo	functionDef = loco.GetFunctionByNumber(functionNumber+1);

				if(functionDef != null && train.GetFunctionState(functionDef.Name, loco.Id, false))
					functionMask |= (byte)(1 << functionNumber);
			}

			return functionMask;
		}

		[LayoutEvent("set-locomotive-function-state-command", IfEvent="*[CommandStation/@Name='`string(Name)`']")]
		private void setLocomotiveFunctionStateCommand(LayoutEvent e) {
			LocomotiveInfo			loco = (LocomotiveInfo)e.Sender;

			commandStationManager.AddCommand(queueLocoCommands, new IntelliboxLocomotiveFunctionsCommand(this, loco.AddressProvider.Unit, getFunctionMask(loco)));
		}

		[LayoutEvent("trigger-locomotive-function-command", IfEvent="*[CommandStation/@Name='`string(Name)`']")]
		private void triggerLocomotiveFunctionCommand(LayoutEvent e) {
			LocomotiveInfo		loco = (LocomotiveInfo)e.Sender;
			string				functionName = (string)e.Info;
			TrainStateInfo		train = LayoutModel.StateManager.Trains[loco.Id];
			bool				state = train.GetFunctionState(functionName, loco.Id, false);

			state = !state;
			train.SetLocomotiveFunctionState(functionName, loco.Id, state);
		}

		#endregion
	}

	#region SO and SO collection classes

	public class SOinfo : LayoutXmlWrapper {
		public SOinfo(XmlElement element)
			: base(element) {
		}

		public SOinfo()
			: base("SO") {
		}

		public int Number {
			get {
				return XmlConvert.ToInt32(GetAttribute("Number"));
			}

			set {
				SetAttribute("Number", XmlConvert.ToString(value));
			}
		}

		public int Value {
			get {
				return XmlConvert.ToInt32(GetAttribute("Value"));
			}

			set {
				SetAttribute("Value", XmlConvert.ToString(value));
			}
		}

		public string Description {
			get {
				return GetAttribute("Description");
			}

			set {
				SetAttribute("Description", value);
			}
		}
	}

	public class SOcollection : LayoutXmlWrapper, IEnumerable<SOinfo> {
		public SOcollection(XmlElement parentElement) {
			Element = parentElement["SpecialOptions"];

			if(Element == null) {
				Element = parentElement.OwnerDocument.CreateElement("SpecialOptions");
				parentElement.AppendChild(Element);
			}
		}

		public SOinfo Add(SOinfo item) {
			XmlElement e;

			if(item.Element.OwnerDocument != Element.OwnerDocument)
				e = (XmlElement)Element.OwnerDocument.ImportNode(item.Element, true);
			else
				throw new ArgumentException("Adding SO already in collection");

			Element.AppendChild(e);
			return new SOinfo(e);
		}

		public SOinfo Add(int number, int v, string description) {
			SOinfo so = new SOinfo();

			so.Number = number;
			so.Value = v;
			so.Description = description;
			return Add(so);
		}

        public int Count => Element.ChildNodes.Count;

        public bool Remove(SOinfo item) {
			if(item.Element.OwnerDocument == Element.OwnerDocument) {
				Element.RemoveChild(item.Element);
				return true;
			}
			else
				return false;
		}


		#region IEnumerable Members

		public IEnumerator<SOinfo> GetEnumerator() {
			foreach(XmlElement e in Element)
				yield return new SOinfo(e);
		}

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }

	#endregion

	#region Intellibox Command Classes

	abstract class IntelliboxCommand : OutputCommandBase {
		IntelliboxComponent	            commandStation;
		Stream							stream;
		public static LayoutTraceSwitch		traceIntelliboxRaw = new LayoutTraceSwitch("Intellibox Raw data", "Intellibox raw input/output");

		public IntelliboxCommand(IntelliboxComponent commandStation) {
			this.commandStation = commandStation;
			this.stream = commandStation.CommunicationStream;
		}

        protected IntelliboxComponent CommandStation => commandStation;

        protected Stream Stream => stream;

        protected void BeginCommand() {
			Stream.WriteByte((byte)'x');
		}

		protected void EndCommand() {
			Stream.Flush();
		}

		protected byte GetResponse() {
			byte[]	buffer = new byte[1];

			Stream.Read(buffer, 0, 1);

			Trace.WriteLineIf(traceIntelliboxRaw.TraceVerbose, "  Response " + Convert.ToString(buffer[0], 16));
			return buffer[0];
		}

		/// <summary>
		/// Send a command in a buffer
		/// </summary>
		/// <param name="command">The command bytes</param>
		/// <returns>The reply's 1st byte</returns>
		protected byte Command(byte[] command) {
			if(traceIntelliboxRaw.TraceVerbose) {
				Trace.Write("Send command [" + ToString() + "] - x + ");
				foreach(byte b in command)
					Trace.Write(Convert.ToString(b, 16) + " ");
				Trace.WriteLine("");
			}

			BeginCommand();
			foreach(byte b in command)
				Stream.WriteByte(b);
			EndCommand();

			// Read reply
			byte	response = GetResponse();

			return response;
		}

        /// <summary>
        /// Send a single byte command
        /// </summary>
        /// <param name="b">The command byte</param>
        /// <returns>The reply (1st byte) recieved for the command</returns>
        protected byte Command(byte b) => Command(new byte[] { b });

        protected void ReportError(byte errorCode) {
			string message = null;

			switch(errorCode) {
				case 0x02:
					message = "Bad parameter value";
					break;

				case 0x06:
					message = "Command station power is off";
					break;

				case 0x08:
					message = "Locomotive command buffer out of space";
					break;

				case 0x09:
					message = "Turnout FIFO is full";
					break;

				case 0x0b:
					message = "No locomotive slot is available";
					break;

				case 0x0c:
					message = "Invalid locomotive address";
					break;

				case 0x0d:
					message = "Locomotive already controlled by another device";
					break;

				case 0x0e:
					message = "Illegal turnout address";
					break;

				case 0x10:
					message = "I2C FIFO is full";
					break;

#if WARN_FIFI_75_FULL
				case 0x40:
					message = "Turnout FIFO is 75% full";
					break;
#endif

				case 0x41:
					message = "Intellibox is in 'Halt' mode";
					break;

				case 0x42:
					message = "Intellibox is powered off";
					break;

				default:
					message = "Unknown error 0x" + errorCode.ToString("x"); ;
					break;
			}

			if(message != null)
				commandStation.Error(this.CommandStation, "Command: [" + ToString() + "] - " + message);
		}
	}

	struct ExternalLocomotiveEventInfo {
		public int						Unit;
		public byte						LogicalSpeed;
		public byte						FunctionMask;
		public LocomotiveOrientation	Direction;
		public bool						Lights;

		public ExternalLocomotiveEventInfo(int unit, byte logicalSpeed, byte functionMask, LocomotiveOrientation direction, bool lights) {
			Unit = unit;
			LogicalSpeed = logicalSpeed;
			FunctionMask = functionMask;
			Direction = direction;
			Lights = lights;
		}
	}

	class IntelliboxSetSOcommand : IntelliboxCommand {
		int soNumber;
		int soValue;

		public IntelliboxSetSOcommand(IntelliboxComponent commandStation, int soNumber, int soValue) : base(commandStation) {
			this.soNumber = soNumber;
			this.soValue = soValue;
		}

		public override void Do() {
			byte[] command = new byte[] {
				0xa3,
				(byte)(soNumber & 0xff), 
				(byte)((soNumber >> 8) & 0xff),
				(byte)soValue
			};

			Command(command);
		}

        public override string ToString() => "Set SO#" + soNumber + " to " + soValue;
    }


	class IntelliboxPowerOnCommand : IntelliboxCommand {
		public IntelliboxPowerOnCommand(IntelliboxComponent commandStation) : base(commandStation) {
		}

		public override void Do() {
			Command(0xa7);
		}

        public override string ToString() => "Power On";
    }

	class IntelliboxPowerOffCommand : IntelliboxCommand {
		public IntelliboxPowerOffCommand(IntelliboxComponent commandStation) : base(commandStation) {
		}

		public override void Do() {
			Command(0xa6);
		}

        public override string ToString() => "Power off";
    }

	class IntelliboxTrackPowerOnCommand : IntelliboxCommand {
		public IntelliboxTrackPowerOnCommand(IntelliboxComponent commandStation) : base(commandStation) {
		}


		public override void Do() {
			Command(0xa7);
			Command(0xa5);
		}

        public override string ToString() => "Track power On";
    }

	/// <summary>
	/// Force all sensors that are not off to report event. Used to set the initial state
	/// </summary>
	class IntelliboxForceSensorEventCommand : IntelliboxCommand {
		public IntelliboxForceSensorEventCommand(IntelliboxComponent commandStation) : base(commandStation) {
		}

		public override void Do() {
			Command(0x99);
		}

        public override string ToString() => "Force reporting of Sensor Events";
    }

	/// <summary>
	/// Send command for checking whether any event is reported. If events are reported, interrogate for the event details
	/// and report those events to the main thread. This command is sent as idle command.
	/// </summary>
	class InterlliboxProcessEventsCommand : IntelliboxCommand, IOutputIdlecommand {
		ControlBus	_motorolaBus = null;
		ControlBus	_S88bus = null;
		byte[]		previousReply = null;
		List<FeedbackData> feedbackData = new List<FeedbackData>();

		public InterlliboxProcessEventsCommand(IntelliboxComponent commandStation, int pollingPeriod) : base(commandStation) {
			DefaultWaitPeriod = pollingPeriod;
		}

		protected ControlBus MotorolaBus {
			get {
				if(_motorolaBus == null)
					_motorolaBus = LayoutModel.ControlManager.Buses.GetBus(CommandStation, "Motorola");
				return _motorolaBus;
			}
		}

		protected ControlBus S88Bus {
			get {
				if(_S88bus == null)
					_S88bus = LayoutModel.ControlManager.Buses.GetBus(CommandStation, "S88BUS");
				return _S88bus;
			}
		}

		public override void Do() {
			List<LayoutEvent>	events = new List<LayoutEvent>();

			byte[]	reply = new byte[3];

			reply[0] = reply[1] = reply[2] = 0;

			reply[0] = Command(0xc8);

			if((reply[0] & 0x80) != 0) {
				reply[1] = GetResponse();

				if((reply[1] & 0x80) != 0)
					reply[2] = GetResponse();
			}

			if((reply[0] & 0x20) != 0) {			// at least one turnout was changed manually
				byte	count = Command(0xca);

				for(int i = 0; i < count; i++) {
					byte	addressLow = GetResponse();
					byte	addressHighAndMisc = GetResponse();
					int		address = addressLow | ((addressHighAndMisc & 0x7) << 8);
					int		state = ((addressHighAndMisc & 0x80) != 0) ? 1 : 0;

					if(LayoutController.IsOperationMode)
						events.Add(new LayoutEvent(new ControlConnectionPointReference(MotorolaBus, address), "control-connection-point-state-changed-notification", null, state));
					else if(LayoutController.IsDesignTimeActivation)
						events.Add(new LayoutEvent(CommandStation, "design-time-command-station-event", null,
							new CommandStationInputEvent(CommandStation, MotorolaBus, address, state)));
				}
			}

			if((reply[0] & 0x04) != 0) {			// At least one sensor changed its state
				byte	moduleNo = Command(0xcb);

				while(moduleNo > 0) {
					UInt16	mask;

					mask = GetResponse();
					mask = (UInt16)((mask << 8) | GetResponse());

					while(moduleNo > feedbackData.Count)
						feedbackData.Add(new FeedbackData());

					feedbackData[moduleNo - 1].NewData(CommandStation, moduleNo - 1, mask);
					moduleNo = GetResponse();
				}
			}

			if((reply[0] & 0x01) != 0) {			// At least one locomotive state changed not by this program
				byte	logicalSpeed = Command(0xc9);

				while(logicalSpeed < 0x80) {
					byte	functionMask = GetResponse();
					byte	lowAddress = GetResponse();
					byte	highAddressAndDir = GetResponse();
					byte	realSpeed = GetResponse();

					int						unit = lowAddress | ((highAddressAndDir & 0x3f) << 8);
					LocomotiveOrientation	direction = ((highAddressAndDir & 0x80) != 0) ? LocomotiveOrientation.Forward : LocomotiveOrientation.Backward;
					bool					lights = (highAddressAndDir & 0x40) != 0;
	
					if(CommandStation.OperationMode)
						events.Add(new LayoutEvent(CommandStation, "intellibox-notify-locomotive-state", null,
							new ExternalLocomotiveEventInfo(unit, logicalSpeed, functionMask, direction, lights)));

					logicalSpeed = GetResponse();
				}
			}
			
			if((reply[1] & 0x20) != 0) {
				if(previousReply == null || (previousReply[1] & 0x20) == 0)
					CommandStation.Warning(CommandStation, "Command station reports on overheating condition!");
			}
			else if(previousReply != null && (previousReply[1] & 0x20) != 0)
				CommandStation.Warning(CommandStation, "Command station does no longer report on overheating condition");

			if((reply[1] & 0x10) != 0) {
				if(previousReply == null || (previousReply[1] & 0x10) == 0)
					CommandStation.Warning(CommandStation, "Command station reports on forbidden electrical connection between tracks and programming track");
			}
			else if(previousReply != null && (previousReply[1] & 0x10) != 0)
				CommandStation.Warning(CommandStation, "Command station does no longer report on forbidden connection between tracks and programming track");

			if((reply[1] & 0x08) != 0) {
				if(previousReply == null || (previousReply[1] & 0x08) == 0)
					CommandStation.Warning(CommandStation, "Command station reports on current overload condition for DCC booster");
			}
			else if(previousReply != null && (previousReply[1] & 0x08) != 0)
				CommandStation.Warning(CommandStation, "Command station does no longer report on current overload condition for DCC booster");

			if((reply[1] & 0x04) != 0) {
				if(previousReply == null || (previousReply[1] & 0x04) == 0)
					CommandStation.Warning(CommandStation, "Command station reports on current overload condition for internal booster");
			}
			else if(previousReply != null && (previousReply[1] & 0x04) != 0)
				CommandStation.Warning(CommandStation, "Command station does no longer report on current overload condition for internal booster");

			if((reply[1] & 0x02) != 0) {
				if(previousReply == null || (previousReply[1] & 0x02) == 0)
					CommandStation.Warning(CommandStation, "Command station reports on current overload condition for loco-mouse bus");
			}
			else if(previousReply != null && (previousReply[1] & 0x02) != 0)
				CommandStation.Warning(CommandStation, "Command station does no longer report on current overload condition for loco-mouse bus");

			if((reply[1] & 0x01) != 0) {
				if(previousReply == null || (previousReply[1] & 0x01) == 0)
					CommandStation.Warning(CommandStation, "Command station reports on current overload condition for external booster");
			}
			else if(previousReply != null && (previousReply[1] & 0x01) != 0)
				CommandStation.Warning(CommandStation, "Command station does no longer report on current overload condition for external booster");

			if(previousReply == null)
				previousReply = new byte[3];

			reply.CopyTo(previousReply, 0);

			for(int i = 0; i < feedbackData.Count; i++)
				feedbackData[i].Tick(CommandStation, i, events);

			CommandStation.InterThreadEventInvoker.QueueEvent(new LayoutEvent(CommandStation, "intellibox-invoke-events", null, events));
		}

        public override string ToString() => "Process Events";

        class FeedbackData {
			UInt16 currentData = 0;
			byte[] debounceCounters = new byte[16];

			/// <summary>
			/// Called when new data is read for this feedback decoder. It will set the debouncing counter for each
			/// changed bit.
			/// </summary>
			/// <param name="data">Data read from the decoder</param>
			public void NewData(IntelliboxComponent commandStation, int moduleNumber, UInt16 data) {
				UInt16 changedBits = (UInt16)(currentData ^ data);

				for(int i = 0; i < 16; i++) {
					if((changedBits & 1) != 0) {
						if(debounceCounters[i] > 0)
							commandStation.InterThreadEventInvoker.QueueEvent(new LayoutEvent(new ControlConnectionPointReference(commandStation.S88Bus, moduleNumber+1, 15-i), "control-connection-point-state-unstable-notification", null, commandStation));
						debounceCounters[i] = commandStation.OperationMode ? commandStation.CachedOperationModeDebounceCount : commandStation.CachedDesignTimeDebounceCount;			// Start counting
					}
					changedBits >>= 1;
				}

				currentData = data;
			}

			/// <summary>
			/// Called each time a process events command is executed. This will generate events for each feedback bit that was not changed
			/// for initialDebounceCounter times.
			/// </summary>
			/// <param name="commandStation">The command station</param>
			/// <param name="moduleNumber">Module number (0 based)</param>
			/// <param name="events">List of events to which to add the event</param>
			public void Tick(IntelliboxComponent commandStation, int moduleNumber, List<LayoutEvent> events) {
				for(int i = 0; i < 16; i++) {
					if(debounceCounters[i] > 0) {
						if(--debounceCounters[i] == 0) {
							bool isSet = (currentData & (1 << i)) != 0;

							if(LayoutController.IsOperationMode)
								events.Add(new LayoutEvent(new ControlConnectionPointReference(commandStation.S88Bus, moduleNumber+1, 15-i), "control-connection-point-state-changed-notification", null, isSet ? 1 : 0));
							else if(LayoutController.IsDesignTimeActivation)
								events.Add(new LayoutEvent(commandStation, "design-time-command-station-event", null,
									new CommandStationInputEvent(commandStation, commandStation.S88Bus, moduleNumber+1, 15-i, isSet ? 1 : 0)));
						}
					}
				}
			}
		}

        #region ICommandStationIdlecommand Members

        public bool RemoveFromQueue => false;

        #endregion
    }

	abstract class IntelliboxAccessoryCommandBase : IntelliboxCommand {
		protected List<KeyValuePair<int, int>> listOfunitAndState = new List<KeyValuePair<int, int>>();

		public IntelliboxAccessoryCommandBase(IntelliboxComponent commandStation, int unit, int state) : base(commandStation) {
			listOfunitAndState.Add(new KeyValuePair<int, int>(unit, 1-state));
		}

		public IntelliboxAccessoryCommandBase(IntelliboxComponent commandStation, List<SwitchingCommand> switchingCommands, int startAt, int count) : base(commandStation) {
			for(int i = 0; i < count; i++) {
				SwitchingCommand switchingCommand = switchingCommands[startAt + i];
				int unit = switchingCommand.ControlPointReference.Module.Address + switchingCommand.ControlPointReference.Index;
				
				listOfunitAndState.Add(new KeyValuePair<int,int>(unit, 1 - switchingCommand.SwitchState));
			}
		}
	}

	class IntelliboxAccessoryCommand : IntelliboxAccessoryCommandBase {
		public IntelliboxAccessoryCommand(IntelliboxComponent commandStation, int unit, int state)
			: base(commandStation, unit, state) {
		}

		public IntelliboxAccessoryCommand(IntelliboxComponent commandStation, List<SwitchingCommand> switchingCommands, int startAt, int count)
			: base(commandStation, switchingCommands, startAt, count) {
		}

		public override void Do() {
			foreach(KeyValuePair<int, int> unitAndState in listOfunitAndState) {
				int unit = unitAndState.Key;
				int state = unitAndState.Value;
				byte[] command = new byte[3];

				command[0] = 0x90;
				command[1] = (byte)(unit & 0xff);
				command[2] = (byte)((state << 7) | 0x40 | ((unit >> 8) & 0xff));

				byte reply = Command(command);

				if(reply != 0)
					ReportError(reply);
			}
		}

        // Wait 100 milliseconds before sending next command from this queue
        public override int WaitPeriod => CommandStation.Info.AccessoryCommandTime;

        public override string ToString() {
			string s;

			if(listOfunitAndState.Count > 1)
				s = "Begin set turnouts: ";
			else
				s = "Begin set turnout: ";

			foreach(KeyValuePair<int, int> unitAndState in listOfunitAndState)
				s += " #" + unitAndState.Key + " to state: " + unitAndState.Value + "; ";
			return s;
		}
	}

	class IntelliboxEndAccessoryCommand : IntelliboxAccessoryCommandBase {
		public IntelliboxEndAccessoryCommand(IntelliboxComponent commandStation, int unit, int state)
			: base(commandStation, unit, state) {
		}

		public IntelliboxEndAccessoryCommand(IntelliboxComponent commandStation, List<SwitchingCommand> switchingCommands, int startAt, int count)
			: base(commandStation, switchingCommands, startAt, count) {
		}

		public override void Do() {
			foreach(KeyValuePair<int, int> unitAndState in listOfunitAndState) {
				int unit = unitAndState.Key;
				int state = unitAndState.Value;
				byte[] command = new byte[3];

				command[0] = 0x90;
				command[1] = (byte)(unit & 0xff);
				command[2] = (byte)((state << 8) | ((unit >> 8) & 0xff));

				byte reply = Command(command);

				if(reply != 0)
					ReportError(reply);
			}
		}

		public override string ToString() {
			string s;

			if(listOfunitAndState.Count > 1)
				s = "End set turnouts: ";
			else
				s = "End set turnout: ";

			foreach(KeyValuePair<int, int> unitAndState in listOfunitAndState)
				s += " #" + unitAndState.Key + " to state: " + unitAndState.Value + "; ";
			return s;
		}
	}

	class IntelliboxLocomotiveCommand : IntelliboxCommand {
		int		unit;
		byte	speed;
		byte	direction = 0;
		byte	lights = 0;

		public IntelliboxLocomotiveCommand(IntelliboxComponent commandStation, int unit, int logicalSpeed, LocomotiveOrientation direction, bool lights) : base(commandStation) {
			this.unit = unit;
			speed = (byte)logicalSpeed;

			if(direction == LocomotiveOrientation.Forward)
				this.direction = 0x20;
			if(lights)
				this.lights = 0x10;
		}

		public override void Do() {
			byte[]	command = new byte[5];

			command[0] = 0x80;
			command[1] = (byte)(unit & 0xff);
			command[2] = (byte)((unit >> 8) & 0xff);
			command[3] = speed;
			command[4] = (byte)(direction | lights);

			byte	reply = Command(command);

			if(reply != 0)
				ReportError(reply);
		}

        public override string ToString() => "Locomotive: " + unit + " speed: " + speed + " direction: " + ((direction & 0x20) != 0 ? "forward" : "backward") + " lights: " + (lights != 0 ? "On" : "Off");
    }

	class IntelliboxLocomotiveFunctionsCommand : IntelliboxCommand {
		int		unit;
		byte	functionMask;

		public IntelliboxLocomotiveFunctionsCommand(IntelliboxComponent commandStation, int unit, byte functionMask) : base(commandStation) {
			this.unit = unit;
			this.functionMask = functionMask;
		}

		public override void Do() {
			byte[]	command = new byte[4];

			command[0] = 0x88;
			command[1] = (byte)(unit & 0xff);
			command[2] = (byte)((unit >> 8) & 0xff);
			command[3] = functionMask;

			byte	reply = Command(command);

			if(reply != 0)
				ReportError(reply);
		}

        public override string ToString() => "Locomotive: " + unit + " set functions: " + functionMask;
    }

	#endregion
}
