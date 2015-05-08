﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Net.Sockets;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

namespace NumatoController {
    public enum InterfaceType {
        Serial,
        TCP,
    };

	public class NumatoController : ModelComponent, IModelComponentIsBusProvider {

        internal const string interfaceTypeAttribute = "InterfaceType";
        internal const string addressAttribute = "Address";
        internal const string userAttribute = "User";
        internal const string passwordAttribute = "Password";

        public static LayoutTraceSwitch TraceNumato = new LayoutTraceSwitch("NumatoController", "Numato Relay controller");

		ControlBus _relayBus = null;
		bool simulation;
		bool operationMode;
        Stream commStream;

		public NumatoController() {
			this.XmlDocument.LoadXml(
				$"<NumatoController {interfaceTypeAttribute}=\"TCP\" {addressAttribute}=\"169.254.1.1\" {userAttribute}=\"admin\" {passwordAttribute}=\"admin\">" +
				"<ModeString>baud=115200 parity=N data=8</ModeString>" +
				"</NumatoController>"
				);
		}

		public ControlBus RelayBus {
			get {
				if(_relayBus == null)
					_relayBus = LayoutModel.ControlManager.Buses.GetBus(this, "NumatoRelayBus");
				return _relayBus;
			}
		}

        OutputManager OutputManager { get; set; }

        InterfaceType InterfaceType {
            get {
                if (Element.HasAttribute(interfaceTypeAttribute))
                    return (InterfaceType)Enum.Parse(typeof(InterfaceType), Element.GetAttribute(interfaceTypeAttribute));
                else
                    return InterfaceType.Serial;
            }

            set {
                Element.SetAttribute(interfaceTypeAttribute, value.ToString());
            }
        }

        
        string User {
            get { return Element.GetAttribute(userAttribute); }
            set { Element.SetAttribute(userAttribute, value); }
        }


        string Password {
            get { return Element.GetAttribute(passwordAttribute);  }
            set { Element.SetAttribute(passwordAttribute, value); }
        }

        string Address {
            get { return Element.GetAttribute(addressAttribute); }
            set { Element.SetAttribute(addressAttribute, value); }
        }

        Stream CommStream => commStream;

        #region Communication init/cleanup

        void OnInitialize() {
			_relayBus = null;

			TraceNumato.Level = TraceLevel.Verbose;

			OutputManager = new OutputManager(NameProvider.Name, 1);
			OutputManager.Start();
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
            if (InterfaceType == InterfaceType.Serial)
                commStream = (FileStream)EventManager.Event(new LayoutEvent(Element, "open-serial-communication-device-request"));
            else
                commStream = new TcpClient(Address, 23).GetStream();
		}

		void CloseCommunicationStream() {
			commStream.Close();
			commStream = null;
		}

		async Task OnTerminateCommunication() {
			if(OutputManager != null) {
				await OutputManager.WaitForIdle();
				OutputManager.Terminate();
				OutputManager = null;
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

        public override ModelComponentKind Kind => ModelComponentKind.ControlComponent;

        public override bool DrawOutOfGrid => NameProvider.Element != null && NameProvider.Visible == true;

        #endregion

        #region IModelComponentIsBusProvider Members

        public IList<string> BusTypeNames => Array.AsReadOnly<string>(new string[] { "NumatoRelayBus" });

        public bool BatchMultipathSwitchingSupported => false;

        #endregion

        #region IObjectHasName Members

        public LayoutTextInfo NameProvider => new LayoutTextInfo(this);

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

			OnTerminateCommunication().Wait(500);
			CloseCommunicationStream();
			e.Info = true;
		}


		[LayoutAsyncEvent("change-track-component-state-command", IfEvent="*[CommandStation/@ID='`string(@ID)`']")]
		private Task changeTrackComponentStateCommand(LayoutEvent e0) {
			var e = (LayoutEvent<ControlConnectionPointReference, int>)e0;
			var connectionPointRef = e.Sender;

			int iRelay = connectionPointRef.Module.Address + connectionPointRef.Index;
			bool on = e.Info != 0;

			return OutputManager.AddCommand(new SetRelayCommand(this, iRelay, on));
		}

		[LayoutEvent("Numato-invoke-events", IfEvent = "*[CommandStation/@ID='`string(@ID)`']")]
		private void ncdInvokeEvents(LayoutEvent e0) {
			var e = (LayoutEvent<IList<LayoutEvent>>)e0;

			foreach(var anEvent in e.Sender)
				EventManager.Event(anEvent);
		}

		#endregion

		#region Command classes

		public abstract class NumatoCommandBase : OutputSynchronousCommandBase, IOutputCommandReply {
			byte[] replyBuffer;

			protected NumatoController RelayController { get; private set; }
			protected int ReplySize { get; set; }

			public NumatoCommandBase(NumatoController relayController, int replySize = 1) {
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

				if(TraceNumato.TraceInfo) {
					string message = $"NumatoRelayController: Sending {command.Length} bytes:";

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
					Trace.WriteLine(TraceNumato.TraceError, "Unexpected reply from controller: " + (int)replyBuffer[0]);
			}

			public override void Do() {
				SendCommand(RelayController.CommStream);

				if(ReplySize > 0) {
					int count = RelayController.CommStream.Read(replyBuffer, 0, ReplySize);

					if(TraceNumato.TraceVerbose) {
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

		class EnableReportingCommand : NumatoCommandBase {

			public EnableReportingCommand(NumatoController relayController) : base(relayController) {
			}

            protected override byte[] Command => new byte[2] { 254, 27 };
        }	
		
		class SetRelayCommand : NumatoCommandBase {
			int iRelay;
			bool on;

			public SetRelayCommand(NumatoController relayController, int iRelay, bool on) : base(relayController) {
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

            public override string ToString() => "Set relay " + iRelay + " to " + (on ? "ON" : "OFF");
        }

		class PollContactClosuresCommand : OutputSynchronousCommandBase, IOutputCommandReply, IOutputIdlecommand {
			protected NumatoController RelayController { get; private set; }
			ContactClosureBankData[] contactClosureData;
            int pollingPeriod;

			public PollContactClosuresCommand(NumatoController relayController, int pollingPeriod) {
				this.RelayController = relayController;
				this.pollingPeriod = pollingPeriod;
				this.contactClosureData = new ContactClosureBankData[relayController.InputBus.Modules.Sum(module => module.ModuleType.NumberOfConnectionPoints / 8)];
			}

            public override int Timeout => pollingPeriod;

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

				Trace.WriteLineIf(TraceNumato.TraceVerbose, "Bank " + bank + " value: " + reply[0].ToString("x"));
				contactClosureData[bank].NewData(events, module, moduleNumber, connectionPointIndex, reply[0]);
			}

			public override void OnReply(object replyPacket) {
			}

            #region IOutputIdlecommand Members

            public bool RemoveFromQueue => false;

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