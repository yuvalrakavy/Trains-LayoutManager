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

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutLGB {

	/// <summary>
	/// Summary description for MTScomponents.
	/// </summary>
	public class MTScentralStation : LayoutCommandStationComponent {
		static LayoutTraceSwitch		traceMTS = new LayoutTraceSwitch("LGBMTS", "LGB Multi-Train-System Central Station");
		static LayoutTraceSwitch		traceRawInput = new LayoutTraceSwitch("MTSrawInput", "Trace raw MTS central station input");
		OutputManager	commandStationManager;

		IAsyncResult			pendingReadInfo = null;
		byte[]					inputBuffer = new byte[4];
		AutoResetEvent			readAbortedEvent = new AutoResetEvent(false);
		
		delegate				void messageParserDelegate(MTSmessage message);

		ControlBus				_LGBbus = null;
		ControlBus				_DCCbus = null;

		public MTScentralStation() {
			this.XmlDocument.LoadXml(
				"<CommandStation>" +
				"<ModeString>baud=9600 parity=N data=8 octs=on odsr=on</ModeString>" +
				"</CommandStation>"
				);
		}

		public int XbusID {
			get {
				if(Element.HasAttribute("XbusID"))
					return XmlConvert.ToInt32(Element.GetAttribute("XbusID"));
				return 1;
			}
		}

		public ControlBus LGBbus {
			get {
				if(_LGBbus == null)
					_LGBbus = LayoutModel.ControlManager.Buses.GetBus(this, "LGBBUS");
				return _LGBbus;
			}
		}

		public ControlBus DCCbus {
			get {
				if(_DCCbus == null)
					_DCCbus = LayoutModel.ControlManager.Buses.GetBus(this, "DCC");
				return _DCCbus;
			}
		}

		#region Specific implementation to base class overrideble methods and properties

		public override IList<string> BusTypeNames {
			get {
				return Array.AsReadOnly<string>(new string[] { "LGBDCC", "LGBBUS" });
			}
		}

		public override DigitalPowerFormats SupportedDigitalPowerFormats {
			get { return DigitalPowerFormats.NRMA; }
		}

		protected override void OnCommunicationSetup() {
			base.OnCommunicationSetup ();

			if(!Element.HasAttribute("OverlappedIO"))
				Element.SetAttribute("OverlappedIO", XmlConvert.ToString(true));
			if(!Element.HasAttribute("ReadIntervalTimeout"))
				Element.SetAttribute("ReadIntervalTimeout", XmlConvert.ToString(100));
			if(!Element.HasAttribute("ReadTotalTimeoutConstant"))
				Element.SetAttribute("ReadTotalTimeoutConstant", XmlConvert.ToString(5000));
		}

		protected override void OnInitialize() {
			base.OnInitialize ();

			_LGBbus = null;
			_DCCbus = null;

			commandStationManager = new OutputManager(Name, 1);
			commandStationManager.Start();

			// And finally begin an asynchronous read
			pendingReadInfo = CommunicationStream.BeginRead(inputBuffer, 0, inputBuffer.Length, new AsyncCallback(this.OnReadDone), null);
		}

		protected override async Task OnTerminateCommunication() {
			await base.OnTerminateCommunication ();

			if(commandStationManager != null) {
				await commandStationManager.WaitForIdle();
				commandStationManager.Terminate();
				commandStationManager = null;
			}
		}

		public override bool LayoutEmulationSupported {
			get {return true; }
		}

		public override bool DesignTimeLayoutActivationSupported {
			get { return true; }
		}

		public override int GetHighestLocomotiveAddress(DigitalPowerFormats format) {
			return 22;		// Poor thing...
		}

		protected override ILayoutCommandStationEmulator CreateCommandStationEmulator(string pipeName) {
			return new MTScommandStationEmulator(this, pipeName, EmulationTickTime);
		}

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
			if(e.Sender == null || e.Sender == this) {
				commandStationManager.AddCommand(new MTSpowerDisconnect(CommunicationStream));
				commandStationManager.AddCommand(new MTSresetStation(CommunicationStream));
			}

			PowerOff();
		}

		[LayoutEvent("connect-power-request")]
		void PowerConnectRequest(LayoutEvent e) {
			if(e.Sender == null || e.Sender == this) {
				commandStationManager.AddCommand(new MTSresetStation(CommunicationStream));
				commandStationManager.AddCommand(new MTSpowerConnect(CommunicationStream, XbusID));
			}

			PowerOn();
		}

		// Implement command events
		[LayoutAsyncEvent("change-track-component-state-command", IfEvent="*[CommandStation/@Name='`string(Name)`']")]
		Task ChangeTurnoutState(LayoutEvent e0) {
			var e = (LayoutEvent<ControlConnectionPointReference, int>)e0;
			ControlConnectionPointReference		connectionPointRef = e.Sender;
			int									state = e.Info;
			int									address = connectionPointRef.Module.Address + connectionPointRef.Index;

			var task = commandStationManager.AddCommand(new MTSchangeAccessoryState(CommunicationStream, address, state));
			DCCbusNotification(address, state);

			return task;
		}

		[LayoutEvent("change-signal-state-command", IfEvent="*[CommandStation/@ID='`string(@ID)`']")]
		private void changeSignalStateCommand(LayoutEvent e) {
			ControlConnectionPoint		connectionPoint = (ControlConnectionPoint)e.Sender;
			LayoutSignalState			state = (LayoutSignalState)e.Info;
			int							address = connectionPoint.Module.Address + connectionPoint.Index;
			int							v;

			if(state == LayoutSignalState.Green)
				v = 1;
			else
				v = 0;

			commandStationManager.AddCommand(new MTSchangeAccessoryState(CommunicationStream, (byte)address, state == LayoutSignalState.Green ? 1 : 0));
			DCCbusNotification(address, v);
		}

		[LayoutEvent("locomotive-motion-command", IfEvent="*[CommandStation/@Name='`string(Name)`']")]
		private void locomotiveMotionCommand(LayoutEvent e) {
			LocomotiveInfo	loco = (LocomotiveInfo)e.Sender;
			int				speed = (int)e.Info;

			commandStationManager.AddCommand(new MTSlocomotiveMotion(CommunicationStream, (byte)loco.AddressProvider.Unit, speed));
		}

		[LayoutEvent("set-locomotive-lights-command", IfEvent="*[CommandStation/@Name='`string(Name)`']")]
		private void setLocomotiveLightsCommand(LayoutEvent e) {
			LocomotiveInfo	loco = (LocomotiveInfo)e.Sender;

			commandStationManager.AddCommand(new MTSlocomotiveFunction(CommunicationStream, loco.AddressProvider.Unit, 0x80));
		}

		[LayoutEvent("set-locomotive-function-state-command", IfEvent="*[CommandStation/@Name='`string(Name)`']")]
		[LayoutEvent("trigger-locomotive-function-command", IfEvent="*[CommandStation/@Name='`string(Name)`']")]
		private void setLocomotiveFunctionStateCommand(LayoutEvent e) {
			LocomotiveInfo			loco = (LocomotiveInfo)e.Sender;
			String					functionName = (String)e.Info;
			LocomotiveFunctionInfo	function = loco.GetFunctionByName(functionName);

			commandStationManager.AddCommand(new MTSlocomotiveFunction(CommunicationStream, loco.AddressProvider.Unit, function.Number));
		}

		#endregion

		#region Generate notification events

		private void LGBbusNotification(int address, int state) {
			if(OperationMode) {
				ControlConnectionPointReference connectionPointRef;
				ControlModule lgbBusModule = LGBbus.GetModuleUsingAddress(address);

				if(lgbBusModule != null) {
					if(lgbBusModule.ModuleTypeName == "LGB55070") {			// Feedback module
						connectionPointRef = new ControlConnectionPointReference(lgbBusModule, (address - lgbBusModule.Address) * 2 + state);
						state = 1;
					}
					else
						connectionPointRef = new ControlConnectionPointReference(lgbBusModule, address - lgbBusModule.Address);

					EventManager.Event(new LayoutEvent(connectionPointRef, "control-connection-point-state-changed-notification", null, state));
				}
			}
		}

		private void DCCbusNotification(int address, int state) {
			if(OperationMode)
				EventManager.Event(new LayoutEvent(new ControlConnectionPointReference(DCCbus, address), "control-connection-point-state-changed-notification", null, state));
		}

		#endregion

		#region MTS interface input handling

		[LayoutEvent("parse-MTS-message")]
		private void parseInputMessage(LayoutEvent e) {
			if(e.Sender == this) {
				MTSmessage message = (MTSmessage)e.Info;

				switch(message.Command) {

					case MTScommand.TurnoutControl:
						if(OperationMode) {
							if(message.Address <= 128)
								DCCbusNotification(message.Address, message.Value);
							LGBbusNotification(message.Address, message.Value);
						}
						else if(LayoutController.IsDesignTimeActivation)
							EventManager.Event(new LayoutEvent(this, "design-time-command-station-event", null,
								new CommandStationInputEvent(this, message.Address <= 128 ? DCCbus : LGBbus, message.Address, message.Value)));
						break;

					case MTScommand.ResetStation:
						if(message.Value == 0x80) {
							PowerOff();
							MessageBox.Show(LayoutController.ActiveFrameWindow,
								"Emergency stop has been engaged. Click on OK to reactivate layout", "Emergency Stop",
								MessageBoxButtons.OK, MessageBoxIcon.Stop);
							EventManager.Event(new LayoutEvent(this, "connect-power-request"));
						}
						else if(message.Value != 0x81)
							Warning("Unexpected reset station message (value " + message.Value + ")");
						break;

					case MTScommand.LocomotiveFunction:
						if(OperationMode) {
							if(message.Value == 0x80) {
								EventManager.Event(new LayoutEvent(this, "toggle-locomotive-lights-notification",
									"<Address Unit='" + message.Address + "' />"));
							}
						}
						break;

					case MTScommand.LocomotiveMotion: {
							if(OperationMode) {
								int speed = message.Value >= 0x20 ? message.Value - 0x20 : -(int)message.Value;

								EventManager.Event(new LayoutEvent(this, "locomotive-motion-notification",
									"<Address Unit='" + message.Address + "' />", speed));
							}
						}
						break;
				}
			}
		}

		/// <summary>
		/// Called when an asynchronous read is done
		/// </summary>
		/// <param name="asyncReadResult"></param>
		void OnReadDone(IAsyncResult asyncReadResult) {
			if(CommunicationStream != null) {
				try {
					int	count = CommunicationStream.EndRead(asyncReadResult);

					if(traceRawInput.TraceInfo) {
						Trace.WriteLineIf(traceMTS.TraceInfo, "MTS Central Station - got " + count + " bytes: ");
						for(int i = 0; i < count; i++)
							Trace.WriteIf(traceMTS.TraceInfo, String.Format(" {0:x}", inputBuffer[i]));
						Trace.WriteLineIf(traceMTS.TraceInfo, "");
					}

					for(int offset = 0; offset < count; offset += 4)
						InterThreadEventInvoker.QueueEvent(new LayoutEvent(this, "parse-MTS-message", null, new MTSmessage(inputBuffer, offset)));

					// Start the next read
					pendingReadInfo = CommunicationStream.BeginRead(inputBuffer, 0, inputBuffer.Length, new AsyncCallback(this.OnReadDone), null);
				} catch(Exception ex) {
					Trace.WriteLineIf(traceMTS.TraceInfo, "Pending read aborted (" + ex.Message + ")");
					readAbortedEvent.Set();
				}
			}
		}

		#endregion
	}

	#region MTS Command Classes

	class MTScommandBase : OutputCommandBase {
		Stream		stream;
		MTSmessage	mtsMessage = null;

		public MTScommandBase(Stream stream) {
			this.stream = stream;
		}

		public MTScommandBase(Stream stream, MTSmessage mtsMessage) {
			this.stream = stream;
			this.mtsMessage = mtsMessage;
		}

		public Stream Stream {
			get {
				return stream;
			}
		}

		public override void Do() {
			if(mtsMessage != null)
				mtsMessage.Send(Stream);
		}
	}

	class MTSresetStation : MTScommandBase {
		public MTSresetStation(Stream stream) : base(stream) {
			WaitPeriod = 250;
		}

		public override void Do() {
			new MTSmessage(MTScommand.ResetStation, 0x00, 0x80).Send(Stream);
		}
	}

	class MTSpowerDisconnect : MTScommandBase {
		public MTSpowerDisconnect(Stream stream) : base(stream) {
			WaitPeriod = 250;
		}

		public override void Do() {
			new MTSmessage(MTScommand.EmergencyStop, 0, 0).Send(Stream);
		}
	}

	class MTSpowerConnect : MTScommandBase {
		int	xbusID;

		public MTSpowerConnect(Stream stream, int xbusID) : base(stream) {
			this.xbusID = xbusID;
		}

		public override void Do() {
			new MTSmessage(MTScommand.PowerStation, 0, (byte)xbusID).Send(Stream);
		}
	}

	class MTSchangeAccessoryState : MTScommandBase {
		int		unit;
		int		state;

		public MTSchangeAccessoryState(Stream stream, int unit, int state) : base(stream) {
			this.unit = unit;
			this.state = state;
		}

		public override void Do() {
			new MTSmessage(MTScommand.TurnoutControl, (byte)unit, (byte)state).Send(Stream);
		}
	}

	class MTSlocomotiveMotion : MTScommandBase {
		byte	unit;
		int		speed;

		public MTSlocomotiveMotion(Stream stream, byte unit, int speed) : base(stream) {
			this.unit = unit;
			this.speed = speed;
		}

		public override void Do() {
			byte				speedValue;

			if(speed > 0)
				speedValue = (byte)(speed + 0x21);
			else
				speedValue = (byte)(-speed + 1);

			new MTSmessage(MTScommand.LocomotiveMotion, unit, speedValue).Send(Stream);

			if(speed == 0) {
				Thread.Sleep(100);
				new MTSmessage(MTScommand.LocomotiveMotion, unit, speedValue).Send(Stream);
				Thread.Sleep(100);
				new MTSmessage(MTScommand.ResetLocomotive, unit, 0x01).Send(Stream);
			}
		}
	}

	class MTSlocomotiveFunction : MTScommandBase {
		byte	unit;
		byte	functionNumber;

		public MTSlocomotiveFunction(Stream stream, int unit, int functionNumber) : base(stream) {
			this.unit = (byte)unit;
			this.functionNumber = (byte)functionNumber;
		}

		public override void Do() {
			new MTSmessage(MTScommand.LocomotiveFunction, (byte)(unit | 0x80), functionNumber).Send(Stream);
		}
	}

	#endregion

	#region MTS command definition, and class encupsolating the format of a 4 byte packet that is sent/reveived to MTS

	public enum MTScommand {
		GetInfo = 0,
		LocomotiveMotion = 1,
		LocomotiveFunction = 2,
		TurnoutControl = 3,
		TrackContactStatus = 4,
		ResetLocomotive = 6,
		DefineDoubleHeading = 9,
		ResetStation = 7,
		PowerStation = 20,				// 0x14
		EmergencyStop = 22,		// 0x16
	}

	/// <summary>
	/// Encapsulate a message sent over the XBUS (LGB BUS)
	/// </summary>
	public class MTSmessage {
		byte[]	message = new byte[4];

		public MTSmessage(MTScommand command, byte p1, byte p2) {
			buildMessage(command, p1, p2);
		}
	
		public MTSmessage(byte[] buffer) {
			buildMessageFromBuffer(buffer, 0);
		}

		public MTSmessage(byte[] buffer, int offset) {
			buildMessageFromBuffer(buffer, offset);
		}

		private void buildMessage(MTScommand command, byte p1, byte p2) {
			message[0] = (byte)command;
			message[1] = p1;
			message[2] = p2;
			UpdateChecksum();
		}

		private void buildMessageFromBuffer(byte[] buffer, int offset) {
			if(offset+4 > buffer.Length)
				throw new ArgumentOutOfRangeException("buffer/offset", "MTS message is at least four bytes");

			for(int i = 0; i < 4; i++)
				message[i] = buffer[offset+i];

			// Verify that the checksum is correct
			if((message[0] ^ message[1] ^ message[2]) != message[3])
				throw new ArgumentException("buffer", "MTS Message has bad checksum");
		}

		protected void UpdateChecksum() {
			message[3] = (byte)(message[0] ^ message[1] ^ message[2]);
		}

		public void Send(Stream s) {
			s.Write(message, 0, message.Length);
			s.Flush();
		}

		public MTScommand Command {
			get {
				return (MTScommand)message[0];
			}

			set {
				message[0] = (byte)value;
				UpdateChecksum();
			}
		}

		public byte Address {
			get {
				return message[1];
			}

			set {
				message[1] = value;
				UpdateChecksum();
			}
		}

		public byte Value {
			get {
				return message[2];
			}

			set {
				message[2] = value;
				UpdateChecksum();
			}
		}

		public byte[] Buffer {
			get {
				return message;
			}
		}

		public override String ToString() {
			MTScommand	cmd = (MTScommand)message[0];

			return String.Format("MTS command: {0} {1:x} {2:x} (Checksum {3:x})", cmd, message[1], message[2], message[3]);
		}
	}

	#endregion
}
