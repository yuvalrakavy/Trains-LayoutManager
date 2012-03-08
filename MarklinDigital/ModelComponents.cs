using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.View;
using LayoutManager.Components;
using LayoutManager.CommonUI;

namespace MarklinDigital {

	/// <summary>
	/// Summary description for MTScomponents.
	/// </summary>
	public class MarklinDigitalCentralStation : LayoutCommandStationComponent {
		static LayoutTraceSwitch			traceMarklinDigital = new LayoutTraceSwitch("MarkinDigital", "Marklin Digital Interface (P50)");
		OutputManager		commandStationManager;

		ControlBus					_S88bus = null;

		const int				queuePowerCommands = 0;				// Queue for power on/off etc.
		const int				queueLayoutSwitchingCommands = 1;	// Queue for turnout/lights control commands
		const int				queueLocoCommands = 2;				// Queue for locomotive control commands
		const int				feedbackUnitPollingTime = 20;		// time in milliseconds for polling feedback unit
		
		public MarklinDigitalCentralStation() {
			this.XmlDocument.LoadXml(
				"<MarklinDigital FeedbackPolling='10' ReadIntervalTimeout='20' ReadTotalTimeoutConstant='5000'>" +
				"<ModeString>baud=2400 parity=N data=8 stop=2 octs=off odsr=off</ModeString>" +
				"</MarklinDigital>"
				);
		}

		public int FeedbackPolling {
			get {
				return XmlConvert.ToInt32(Element.GetAttribute("FeedbackPolling"));
			}
		}

		public ControlBus S88Bus {
			get {
				if(_S88bus == null)
					_S88bus = LayoutModel.ControlManager.Buses.GetBus(this, "S88BUS");
				return _S88bus;
			}
		}

		#region Specific implementation to base class overrideble methods and properties

		public override IList<string> BusTypeNames {
			get {
				return Array.AsReadOnly<string>(new string[] { "Motorola", "S88BUS" });
			}
		}

		protected override void OnCommunicationSetup() {
			base.OnCommunicationSetup ();

			if(!Element.HasAttribute("ReadIntervalTimeout"))
				Element.SetAttribute("ReadIntervalTimeout", XmlConvert.ToString(20));
			if(!Element.HasAttribute("ReadTotalTimeoutConstant"))
				Element.SetAttribute("ReadTotalTimeoutConstant", XmlConvert.ToString(5000));
		}

		public override DigitalPowerFormats SupportedDigitalPowerFormats {
			get { return DigitalPowerFormats.Motorola; }
		}

		private int getNumberOfFeedbackUnits() {
			int		maxAddress = 0;

			foreach(ControlModule module in S88Bus.Modules)
				if(module.Address > maxAddress)
					maxAddress = module.Address;

			return maxAddress;
		}

		protected override void OnInitialize() {
			base.OnInitialize ();

			_S88bus = null;

			commandStationManager = new OutputManager(Name, 3);

			int	feedbackUnits = getNumberOfFeedbackUnits();

			if(feedbackUnits > 0) {
				int	waitPeriod = (1000 - feedbackUnits*feedbackUnitPollingTime) / FeedbackPolling;

				if(waitPeriod < 0)
					waitPeriod = 0;

				Trace.WriteLine(" " + feedbackUnits + " feedback units, time between polling each unit is " + waitPeriod + " milliseconds");

				for(int unit = 1; unit <= feedbackUnits; unit++)
					commandStationManager.AddIdleCommand(new MarklinGetFeedback(this, InterThreadEventInvoker, unit, waitPeriod));

			}

			commandStationManager.AddIdleCommand(new EndTrainsAnalysisCommandStationCommand(this, 10));
			commandStationManager.Start();
		}

		protected override void OnCleanup() {
			base.OnCleanup ();

			if(OperationMode) {
				// Set all trains to non-reverse state
				foreach(XmlElement trainStateElement in LayoutModel.StateManager.Trains.Element) {
					TrainStateInfo train = new TrainStateInfo(trainStateElement);

					train.SpeedInSteps = 0;

					if(train.MotionDirection == LocomotiveOrientation.Backward) {
						foreach(TrainLocomotiveInfo trainLoco in train.Locomotives)
							if(trainLoco.Orientation == LocomotiveOrientation.Forward)
								EventManager.Event(new LayoutEvent(trainLoco.Locomotive, "locomotive-reverse-motion-direction-command"));
					}
				}
			}
		}

		protected async override Task OnTerminateCommunication() {
			await base.OnTerminateCommunication ();

			if(commandStationManager != null) {
				await commandStationManager.WaitForIdle();
				if(commandStationManager != null) {
					commandStationManager.Terminate();
					commandStationManager = null;
				}
			}
		}

		public override bool LayoutEmulationSupported {
			get {
				return true;
			}
		}

		public override bool TrainsAnalysisSupported {
			get {
				return true;
			}
		}

		protected override ILayoutCommandStationEmulator CreateCommandStationEmulator(string pipeName) {
			return new MarkliinCommandStationEmulator(this, pipeName, EmulationTickTime);			
		}

		public override bool DesignTimeLayoutActivationSupported {
			get { return true; }
		}

		#endregion

		#region Notification Event Handlers

		[LayoutEvent("train-is-removed")]
		private void trainRemoved(LayoutEvent e) {
			TrainStateInfo	train = (TrainStateInfo)e.Sender;

			if(train.MotionDirection == LocomotiveOrientation.Backward) {
				foreach(TrainLocomotiveInfo trainLoco in train.Locomotives)
					if(trainLoco.Orientation == LocomotiveOrientation.Forward)
						EventManager.Event(new LayoutEvent(trainLoco.Locomotive, "locomotive-reverse-motion-direction-command"));
			}
		}

		#endregion

		#region Request event handlers
	
		[LayoutEvent("add-train-operation-menu", Order=100)]
		private void addTrainOperationMenu(LayoutEvent e) {
			TrainStateInfo	train = (TrainStateInfo)e.Sender;
			Menu			menu = (Menu)e.Info;
			bool			trainInActiveTrip = (bool)EventManager.Event(new LayoutEvent(train, "is-train-in-active-trip"));

			if(train.CommandStation != null && train.CommandStation.Id == Id) {
				if(train.Locomotives.Count == 1) {
					LocomotiveInfo loco = train.Locomotives[0].Locomotive;

					menu.MenuItems.Add(new ToggleLocoDirectionMenuItem(loco, "Toggle Locomotive Direction"));
				}
				else {
					MenuItem toggleLocoDirection = new MenuItem("Toggle Locomotive direction");

					foreach(TrainLocomotiveInfo trainLoco in train.Locomotives)
						toggleLocoDirection.MenuItems.Add(new ToggleLocoDirectionMenuItem(trainLoco.Locomotive, trainLoco.Name));

					if(toggleLocoDirection.MenuItems.Count > 0)
						menu.MenuItems.Add(toggleLocoDirection);
				}

				if(!trainInActiveTrip)
					menu.MenuItems.Add(new ToggleLocomotiveManagement(train));
			}
		}

		[LayoutEvent("disconnect-power-request")]
		void PowerDisconnectRequest(LayoutEvent e) {
			if(e.Sender == null || e.Sender == this)
				commandStationManager.AddCommand(queuePowerCommands, new MarklinStopCommand(CommunicationStream));

			PowerOff();
		}

		[LayoutEvent("connect-power-request")]
		void PowerConnectRequest(LayoutEvent e) {
			if(e.Sender == null || e.Sender == this)
				commandStationManager.AddCommand(queuePowerCommands, new MarklinGoCommand(CommunicationStream));

			PowerOn();
		}

		[LayoutEvent("get-command-station-capabilities", IfEvent="*[CommandStation/@ID='`string(@ID)`']")]
		void GetCommandStationCapabilities(LayoutEvent e) {
			CommandStationCapabilitiesInfo	cap = new CommandStationCapabilitiesInfo();
			int								minTimeBetweenSpeedSteps = 20;

			if(Element.HasAttribute("MinTimeBetweenSpeedSteps"))
				minTimeBetweenSpeedSteps = XmlConvert.ToInt32(Element.GetAttribute("MinTimeBetweenSpeedSteps"));

			cap.MinTimeBetweenSpeedSteps = minTimeBetweenSpeedSteps;
			
			e.Info = cap.Element;
		}

		// Implement command events
		[LayoutAsyncEvent("change-track-component-state-command", IfEvent="*[CommandStation/@ID='`string(@ID)`']")]
		Task ChangeTurnoutState(LayoutEvent e0) {
			var e = (LayoutEvent<ControlConnectionPointReference, int>)e0;
			ControlConnectionPointReference connectionPointRef = e.Sender;
			int state = e.Info;
			int address = connectionPointRef.Module.Address + connectionPointRef.Index;
			var tasks = new List<Task>();

			tasks.Add(commandStationManager.AddCommand(queueLayoutSwitchingCommands, new MarklinSwitchAccessoryCommand(CommunicationStream, address, state)));
			tasks.Add(commandStationManager.AddCommand(queueLayoutSwitchingCommands, new MarklinEndSwitchingProcedure(CommunicationStream)));

			if(OperationMode)
				EventManager.Event(new LayoutEvent(connectionPointRef, "control-connection-point-state-changed-notification", null, state));

			return Task.WhenAll(tasks);
		}

		[LayoutEvent("change-signal-state-command", IfEvent="*[CommandStation/@ID='`string(@ID)`']")]
		private void changeSignalStateCommand(LayoutEvent e) {
			ControlConnectionPointReference	connectionPointRef = (ControlConnectionPointReference)e.Sender;
			LayoutSignalState		state = (LayoutSignalState)e.Info;
			int						address = connectionPointRef.Module.Address + connectionPointRef.Index;
			int						v;

			if(state == LayoutSignalState.Green)
				v = 0;
			else
				v = 1;

			commandStationManager.AddCommand(queueLayoutSwitchingCommands, new MarklinSwitchAccessoryCommand(CommunicationStream, address, v));
			commandStationManager.AddCommand(queueLayoutSwitchingCommands, new MarklinEndSwitchingProcedure(CommunicationStream));

			if(OperationMode)
				EventManager.Event(new LayoutEvent(connectionPointRef, "control-connection-point-state-changed-notification", null, state == LayoutSignalState.Green ? 1 : 0));
		}

		[LayoutEvent("locomotive-reverse-motion-direction-command", IfEvent="*[CommandStation/@Name='`string(Name)`']")]
		private void locomotiveReverseMotionDirectionCommand(LayoutEvent e) {
			LocomotiveInfo	loco = (LocomotiveInfo)e.Sender;

			commandStationManager.AddCommand(queueLocoCommands, new MarklinReverseLocomotiveDirection(CommunicationStream, loco.AddressProvider.Unit));
		}

		[LayoutEvent("locomotive-motion-command", IfEvent="*[CommandStation/@Name='`string(Name)`']")]
		private void locomotiveMotionCommand(LayoutEvent e) {
			LocomotiveInfo	loco = (LocomotiveInfo)e.Sender;
			TrainStateInfo	train = LayoutModel.StateManager.Trains[loco.Id];
			int				speed = (int)e.Info;
			bool			auxFunction = false;

			LocomotiveFunctionInfo locoAuxFunction = loco.GetFunctionByNumber(0);

			if(locoAuxFunction != null && locoAuxFunction.Type == LocomotiveFunctionType.OnOff)
				auxFunction = train.GetFunctionState(locoAuxFunction.Name, loco.Id, false);
			else
				auxFunction = loco.HasLights && train.Lights;

			commandStationManager.AddCommand(queueLocoCommands, new MarklinLocomotiveMotion(CommunicationStream, loco.AddressProvider.Unit, speed, auxFunction));
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

			for(int functionNumber = 0; functionNumber < 4; functionNumber++) {
				LocomotiveFunctionInfo	functionDef = loco.GetFunctionByNumber(functionNumber+1);

				if(functionDef != null && functionDef.Type == LocomotiveFunctionType.OnOff && train.GetFunctionState(functionDef.Name, loco.Id, false))
					functionMask |= (byte)(1 << functionNumber);
			}

			return functionMask;
		}

		[LayoutEvent("set-locomotive-function-state-command", IfEvent="*[CommandStation/@Name='`string(Name)`']")]
		private void setLocomotiveFunctionStateCommand(LayoutEvent e) {
			LocomotiveInfo		loco = (LocomotiveInfo)e.Sender;
			string				functionName = (string)e.Info;

			LocomotiveFunctionInfo	functionDef = loco.GetFunctionByName(functionName);

			if(functionDef == null || functionDef.Number != 0)
				commandStationManager.AddCommand(queueLocoCommands, new MarklinSetFunctions(CommunicationStream, loco.AddressProvider.Unit, getFunctionMask(loco)));
			else {
				TrainStateInfo	train = LayoutModel.StateManager.Trains[loco.Id];

				commandStationManager.AddCommand(queueLocoCommands, new MarklinLocomotiveMotion(CommunicationStream, loco.AddressProvider.Unit, train.SpeedInSteps, 
					train.GetFunctionState(functionDef.Name, loco.Id, false)));
			}
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

		#region Menu Items

		class ToggleLocoDirectionMenuItem : MenuItem {
			LocomotiveInfo		loco;

			public ToggleLocoDirectionMenuItem(LocomotiveInfo loco, string title) {
				this.loco = loco;
				Text = title;
			}

			protected override void OnClick(EventArgs e) {
				EventManager.Event(new LayoutEvent(loco, "locomotive-reverse-motion-direction-command"));
			}
		}

		class ToggleLocomotiveManagement : MenuItem {
			TrainStateInfo		train;

			public ToggleLocomotiveManagement(TrainStateInfo train) {
				this.train = train;

				if(train.Managed)
					Text = "Disable computer control";
				else
					Text = "Enable computer control";
			}

			protected override void OnClick(EventArgs e) {
				bool	managed = train.Managed;

				if(!managed) {
					LayoutComponentConnectionPoint	front = (LayoutComponentConnectionPoint)EventManager.Event(new LayoutEvent(train.LocomotiveBlock.BlockDefinintion,
						"get-locomotive-front", null, train.Element));

					train.LocationOfBlock(train.LocomotiveBlock).DisplayFront = front;
				}

				foreach(TrainLocomotiveInfo trainLoco in train.Locomotives)
					trainLoco.Locomotive.NotManaged = managed;
			}
		}

		#endregion
	}

	#region Marklin Digial Command Classes

	abstract class MarklinCommand : OutputCommandBase {
		Stream		stream;

		public MarklinCommand(Stream stream) {
			this.stream = stream;
		}

		public Stream Stream {
			get {
				return stream;
			}
		}

		protected void Output(byte v) {
			Thread.Sleep(10);
			Stream.WriteByte(v);
			Stream.Flush();
		}
	}

	class MarklinGoCommand : MarklinCommand {
		public MarklinGoCommand(Stream comm) : base(comm) {
		}

		public override void Do() {
			Output(96);
			Stream.Flush();
		}
	}

	class MarklinStopCommand : MarklinCommand {
		public MarklinStopCommand(Stream comm) : base(comm) {
		}

		public override void Do() {
			Output(97);
			Stream.Flush();
		}
	}

	class MarklinSwitchAccessoryCommand : MarklinCommand {
		int		address;
		int		state;

		public MarklinSwitchAccessoryCommand(Stream comm, int address, int state) : base(comm) {
			this.address = address;
			this.state = state;
		}

		public override void Do() {
			if(state == 0)
				Output(33);
			else if(state == 1)
				Output(34);
			else
				throw new ArgumentException("Invalid marklin accessory state: " + state + " address: " + address);
			Output((byte)address);
			Stream.Flush();
		}

		// Wait 100 milliseconds before sending next command from this queue
		public override int WaitPeriod {
			get {
				return 100;
			}
		}
	}

	class MarklinEndSwitchingProcedure : MarklinCommand {
		public MarklinEndSwitchingProcedure(Stream comm) : base(comm) {
		}

		public override void Do() {
			Output(32);
			Stream.Flush();
		}
	}

	class MarklinReverseLocomotiveDirection : MarklinCommand {
		int		address;

		public MarklinReverseLocomotiveDirection(Stream comm, int address) : base(comm) {
			this.address = address;
		}

		public override void Do() {
			Output(15);
			Output((byte)address);
			Stream.Flush();
		}
	}

	class MarklinLocomotiveMotion : MarklinCommand {
		int		address;
		byte	speed;
		bool	auxFunction;

		public MarklinLocomotiveMotion(Stream comm, int address, int speed, bool auxFunction) : base(comm) {
			this.address = address;

			if(speed > 0)
				this.speed = (byte)speed;
			else
				this.speed = (byte)-speed;

			this.auxFunction = auxFunction;
		}

		public override void Do() {
			byte	v = speed;

			if(auxFunction)
				v |= 0x10;
			Output(v);
			Output((byte)address);
			Stream.Flush();
		}
	}

	class MarklinSetFunctions : MarklinCommand {
		int		address;
		byte	functionMask;

		public MarklinSetFunctions(Stream comm, int address, byte functionMask) : base(comm) {
			this.address = address;
			this.functionMask = functionMask;
		}

		public MarklinSetFunctions(Stream comm, int address, byte functionMask, int waitPeriod) : base(comm) {
			this.address = address;
			this.functionMask = functionMask;
			DefaultWaitPeriod = waitPeriod;
		}

		public override void Do() {
			byte	v = (byte)((functionMask & 0x0f) | 0x40);

			Output(v);
			Output((byte)address);
			Stream.Flush();
		}
	}

	struct MarklinFeedbackContactResult {
		public int	ContactNo;
		public bool	IsSet;

		public MarklinFeedbackContactResult(int contactNo, bool isSet) {
			this.ContactNo = contactNo;
			this.IsSet = isSet;
		}
	};

	struct MarklinFeedbackResult {
		public int								Unit;
		public MarklinFeedbackContactResult[]	Contacts;

		public MarklinFeedbackResult(int unit) {
			this.Unit = unit;
			this.Contacts = new MarklinFeedbackContactResult[16];
		}
	}

	delegate void MarklinFeedbackResultCallback(MarklinFeedbackResult feedbackResult);
		
	class MarklinGetFeedback : MarklinCommand, IOutputIdlecommand {
		ILayoutInterThreadEventInvoker					invoker;
		int								unit;
		int								feedback = 0;
		MarklinDigitalCentralStation    commandStation;
		
		public MarklinGetFeedback(MarklinDigitalCentralStation commandStation, ILayoutInterThreadEventInvoker invoker, int unit, int waitPeriod) : base(commandStation.CommunicationStream) {
			this.commandStation = commandStation;
			this.invoker = invoker;
			this.unit = unit;
			DefaultWaitPeriod = waitPeriod;
		}

		private void ProcessFeedback(MarklinFeedbackResult feedbackResult) {
			for(int i = 0; i < 16 && feedbackResult.Contacts[i].ContactNo > 0; i++) {
				if(commandStation.OperationMode)
					invoker.QueueEvent(new LayoutEvent(new ControlConnectionPointReference(commandStation.S88Bus, feedbackResult.Unit, feedbackResult.Contacts[i].ContactNo - 1),
						"control-connection-point-state-changed-notification", null, feedbackResult.Contacts[i].IsSet ? 1 : 0));
				else
					invoker.QueueEvent(new LayoutEvent(this, "design-time-command-station-event", null,
						new CommandStationInputEvent(commandStation, commandStation.S88Bus,
						feedbackResult.Unit * 16 + feedbackResult.Contacts[i].ContactNo - 1, feedbackResult.Contacts[i].IsSet ? 1 : 0)));
			}
		}

		public override void Do() {
			Output((byte)(192+unit));
			Stream.Flush();

			byte[]	buffer = new Byte[2];
			int		bytesRead = Stream.Read(buffer, 0, 2);

			if(bytesRead == 2) {
				int	low = (int)buffer[0];
				int	high = (int)buffer[1];
				int	newValue = (low << 8) | high;

				if(newValue != feedback) {

					MarklinFeedbackResult	feedbackResult = new MarklinFeedbackResult(unit);
					int						changeIndex = 0;

					for(int contactNo = 1; contactNo <= 16; contactNo++) {
						int	mask = (1 << (16-contactNo));

						if((feedback & mask) != (newValue & mask))
							feedbackResult.Contacts[changeIndex++] = new MarklinFeedbackContactResult(contactNo, (newValue & mask) != 0);
					}

					if(changeIndex < 16)
						feedbackResult.Contacts[changeIndex] = new MarklinFeedbackContactResult(0, false);		// Mark last one

					ProcessFeedback(feedbackResult);
					feedback = newValue;
				}
			}
			else
				Trace.WriteLine("Read feedback returned " + bytesRead + " bytes (expected 2 bytes), ignoring data");
		}

		#region ICommandStationIdlecommand Members

		public bool RemoveFromQueue {
			get { return false; }
		}

		#endregion
	}

	#endregion
}
