using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Xml;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

namespace NCDRelayController {
	public class NCDRelayController : ModelComponent, IModelComponentIsBusProvider {
		public static LayoutTraceSwitch TraceNCD = new LayoutTraceSwitch("NCDRelayController", "NCD Relay controller");

		ControlBus _relayBus = null;
		ControlBus _inputBus = null;
		bool simulation;
		bool operationMode;
		FileStream commStream;
		OutputManager outputManager;

		public NCDRelayController() {
			this.XmlDocument.LoadXml(
				"<RelayController>" +
				"<ModeString>baud=115200 parity=N data=8</ModeString>" +
				"</RelayController>"
				);
		}

		public ControlBus RelayBus {
			get {
				if(_relayBus == null)
					_relayBus = LayoutModel.ControlManager.Buses.GetBus(this, "NCDRelayBus");
				return _relayBus;
			}
		}

		public ControlBus InputBus {
			get {
				if(_inputBus == null)
					_inputBus = LayoutModel.ControlManager.Buses.GetBus(this, "NCDInputBus");
				return _inputBus;
			}
		}

		OutputManager OutputManager {
			get { return outputManager; }
		}

		Stream CommStream {
			get { return commStream;  }
		}

		#region Communication init/cleanup

		void OnInitialize() {
			_relayBus = null;
			_inputBus = null;

			TraceNCD.Level = TraceLevel.Verbose;

			outputManager = new OutputManager(NameProvider.Name, 1);
			outputManager.Start();

			outputManager.AddCommand(new EnableReportingCommand(this));

			if(InputBus.Modules.Count > 0)
				outputManager.AddIdleCommand(new PollContactClosuresCommand(this, XmlConvert.ToInt32(Element.GetAttribute("PollingPeriod"))));
		}

		void OnCleanup() {
		}

		void OnCommunicationSetup() {
			if(!Element.HasAttribute("OverlappedIO"))
				Element.SetAttribute("OverlappedIO", XmlConvert.ToString(true));
			if(!Element.HasAttribute("ReadIntervalTimeout"))
				Element.SetAttribute("ReadIntervalTimeout", XmlConvert.ToString(3500));
			if(!Element.HasAttribute("ReadTotalTimeoutConstant"))
				Element.SetAttribute("ReadTotalTimeoutConstant", XmlConvert.ToString(6000));
			if(!Element.HasAttribute("WriteTotalTimeoutConstant"))
				Element.SetAttribute("WriteTotalTimeoutConstant", XmlConvert.ToString(6000));
		}

		void OpenCommunicationStream() {
			commStream = (FileStream)EventManager.Event(new LayoutEvent(Element, "open-serial-communication-device-request"));
		}

		void CloseCommunicationStream() {
			commStream.Close();
			commStream = null;
		}

		async Task OnTerminateCommunication() {
			if(outputManager != null) {
				await outputManager.WaitForIdle();
				outputManager.Terminate();
				outputManager = null;
			}
		}

		#endregion

		#region Model Component overrides

		public override void OnAddedToModel() {
			base.OnAddedToModel();

			LayoutModel.ControlManager.Buses.AddBusProvider(this);
		}

		public override void OnRemovingFromModel() {
			base.OnRemovingFromModel();

			LayoutModel.ControlManager.Buses.RemoveBusProvider(this);
		}

		public override ModelComponentKind Kind {
			get { return ModelComponentKind.ControlComponent; }
		}

		public override bool DrawOutOfGrid {
			get {
				return NameProvider.Element != null && NameProvider.Visible == true;
			}
		}

		#endregion

		#region IModelComponentIsBusProvider Members

		public IList<string> BusTypeNames {
			get {
				return Array.AsReadOnly<string>(new string[] { "NCDRelayBus", "NCDInputBus" });
			}
		}

		public bool BatchMultipathSwitchingSupported {
			get { return false; }
		}

		#endregion

		#region IObjectHasName Members

		public LayoutTextInfo NameProvider {
			get {
				return new LayoutTextInfo(this);
			}
		}

		#endregion

		#region Event Handlers

		[LayoutEvent("enter-operation-mode")]
		protected virtual void EnterOperationMode(LayoutEvent e0) {
			var e = (LayoutEvent<OperationModeParameters>)e0;

			simulation= e.Sender.Simulation;

			OnCommunicationSetup();
			OpenCommunicationStream();

			operationMode = true;
			OnInitialize();
		}

		[LayoutEvent("begin-design-time-layout-activation")]
		private void beginDesignTimeLayoutActivation(LayoutEvent e) {
			OnCommunicationSetup();
			OpenCommunicationStream();
			OnInitialize();
			e.Info = true;
		}

		[LayoutAsyncEvent("exit-operation-mode-async")]
		protected async virtual Task ExitOperationModeAsync(LayoutEvent e) {
			if(operationMode) {
				OnCleanup();

				await OnTerminateCommunication();
				CloseCommunicationStream();

				operationMode = false;
			}
		}

		[LayoutEvent("end-design-time-layout-activation")]
		private void EndDesignTimeLayoutActivation(LayoutEvent e) {
			OnCleanup();

			await OnTerminateCommunication();
			CloseCommunicationStream();
			e.Info = true;
		}


		[LayoutAsyncEvent("change-track-component-state-command", IfEvent="*[CommandStation/@ID='`string(@ID)`']")]
		private Task changeTrackComponentStateCommand(LayoutEvent e0) {
			var e = (LayoutEvent<ControlConnectionPointReference, int>)e0;
			var connectionPointRef = e.Sender;

			int iRelay = connectionPointRef.Module.Address + connectionPointRef.Index;
			bool on = e.Info != 0;

			return outputManager.AddCommand(new SetRelayCommand(this, iRelay, on));
		}

		[LayoutEvent("NCD-invoke-events", IfEvent = "*[CommandStation/@ID='`string(@ID)`']")]
		private void ncdInvokeEvents(LayoutEvent e0) {
			var e = (LayoutEvent<IList<LayoutEvent>>)e0;

			foreach(var anEvent in e.Sender)
				EventManager.Event(anEvent);
		}

		#endregion

		#region Command classes

		public abstract class NCDcommandBase : OutputSynchronousCommandBase, IOutputCommandReply {
			byte[] replyBuffer;

			protected NCDRelayController RelayController { get; private set; }
			protected int ReplySize { get; set; }

			public NCDcommandBase(NCDRelayController relayController, int replySize = 1) {
				this.RelayController = relayController;
				this.ReplySize = replySize;

				if(replySize > 0)
					replyBuffer = new byte[replySize];

			}

			public override void OnReply(object replyPacket) {
			}

			protected abstract byte[] Command { get;  }

			protected void SendCommand(Stream stream) {
				byte[] command = this.Command;

				if(TraceNCD.TraceInfo) {
					string message = "NCDRelayController: Sending " + command.Length + " bytes:";

					foreach(byte b in command)
						message += " " + b.ToString();
					Trace.WriteLine(message);
				}

				try {
					stream.Write(command, 0, command.Length);
					stream.Flush();
				}
				catch(OperationCanceledException) { }
			}

			protected virtual void OnReply(byte[] replyBuffer) {
				if(ReplySize > 0 && replyBuffer[0] != 85)
					Trace.WriteLine(TraceNCD.TraceError, "Unexpected reply from controller: " + (int)replyBuffer[0]);
			}

			public override void Do() {
				SendCommand(RelayController.CommStream);

				if(ReplySize > 0) {
					int count = RelayController.CommStream.Read(replyBuffer, 0, ReplySize);

					if(TraceNCD.TraceVerbose) {
						string message = "NCDRelayController: Got " + count + " bytes:";

						foreach(byte b in replyBuffer)
							message += " " + b.ToString();
						Trace.WriteLine(message);
					}

					RelayController.OutputManager.SetReply(this);
					OnReply(replyBuffer);
				}
			}
		}

		class EnableReportingCommand : NCDcommandBase {

			public EnableReportingCommand(NCDRelayController relayController) : base(relayController) {
			}

			protected override byte[] Command {
				get { return new byte[2] { 254, 27 }; }
			}
		}	
		
		class SetRelayCommand : NCDcommandBase {
			int iRelay;
			bool on;

			public SetRelayCommand(NCDRelayController relayController, int iRelay, bool on) : base(relayController) {
				this.iRelay = iRelay;
				this.on = on;
			}

			protected override byte[] Command {
				get {
					byte bank = (byte)(iRelay / 8 + 1);
					byte command;

					if(on)
						command = (byte)(108 + (iRelay % 8));
					else
						command = (byte)(100 + (iRelay % 8));


					byte[] outputBuffer = new byte[3];
					outputBuffer[0] = 254;
					outputBuffer[1] = command;
					outputBuffer[2] = (byte)bank;

					return outputBuffer;
				}
			}

			public override string ToString() {
				return "Set relay " + iRelay + " to " + (on ? "ON" : "OFF");
			}
		}

		class PollContactClosuresCommand : OutputSynchronousCommandBase, IOutputCommandReply, IOutputIdlecommand {
			protected NCDRelayController RelayController { get; private set; }
			ContactClosureBankData[] contactClosureData;

			public PollContactClosuresCommand(NCDRelayController relayController, int pollingPeriod) {
				this.RelayController = relayController;
				this.WaitPeriod = pollingPeriod;
				this.contactClosureData = new ContactClosureBankData[relayController.InputBus.Modules.Sum(module => module.ModuleType.NumberOfConnectionPoints / 8)];
			}

			public override void Do() {
				var events = new List<LayoutEvent>();

				int moduleNumber = 0;
				foreach(var module in RelayController.InputBus.Modules) {
					int bank = module.Address / 8;

					for(int connectionPointIndex = 0; connectionPointIndex < module.ModuleType.NumberOfConnectionPoints; connectionPointIndex += 8) {
						//TODO: Poll bank if not in operation mode (design mode learn) or if operation mode and at least one connection point is actually connected 
						PollContactClosureBank(events, module, moduleNumber, bank, connectionPointIndex);
						bank++;
					}

					moduleNumber++;
				}

				if(events.Count > 0)
					EventManager.Instance.InterThreadEventInvoker.QueueEvent(new LayoutEvent<IList<LayoutEvent>>("NCD-invoke-events", events).SetCommandStation(RelayController));

				RelayController.OutputManager.SetReply(this);
			}

			private void PollContactClosureBank(IList<LayoutEvent> events, ControlModule module, int moduleNumber, int bank, int connectionPointIndex) {
				byte[] command = new byte[] { 254, 175, (byte)bank };
				byte[] reply = new byte[1];

				RelayController.CommStream.Write(command, 0, command.Length);
				RelayController.CommStream.Read(reply, 0, reply.Length);

				Trace.WriteLineIf(TraceNCD.TraceVerbose, "Bank " + bank + " value: " + reply[0].ToString("x"));
				contactClosureData[bank].NewData(events, module, moduleNumber, connectionPointIndex, reply[0]);
			}

			public override void OnReply(object replyPacket) {
			}

			#region IOutputIdlecommand Members

			public bool RemoveFromQueue {
				get { return false; }
			}

			#endregion

			struct ContactClosureBankData {
				byte currentState;

				public void NewData(IList<LayoutEvent> events, ControlModule module, int moduleNumber, int connectionPointIndex, byte data) {
					byte changedBits = (byte)(currentState ^ data);
					byte mask = 1;
					IModelComponentIsBusProvider busProvider = module.Bus.BusProvider;

					for(int i = 0; i < 8; i++) {
						if((changedBits & mask) != 0) {
							int isSet = (data & mask) != 0 ? 1 : 0;

							if(LayoutController.IsOperationMode)
								events.Add(new LayoutEvent(new ControlConnectionPointReference(module.Bus, moduleNumber + 1, i), "control-connection-point-state-changed-notification", null, isSet));
							else if(LayoutController.IsDesignTimeActivation)
								events.Add(new LayoutEvent(busProvider, "design-time-command-station-event", null,
									new CommandStationInputEvent((ModelComponent)busProvider, module.Bus, moduleNumber + 1, i, isSet)));
						}

						mask <<= 1;
					}

					currentState = data;
				}
			}
		}

		#endregion
	}
}
