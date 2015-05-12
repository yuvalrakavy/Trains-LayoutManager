using System;
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

        internal const string relaysCountAttribute = "Relays";
        internal const string interfaceTypeAttribute = "InterfaceType";
        internal const string portAttribute = "Port";
        internal const string addressAttribute = "Address";
        internal const string userAttribute = "User";
        internal const string passwordAttribute = "Password";

        public static LayoutTraceSwitch TraceNumato = new LayoutTraceSwitch("NumatoController", "Numato Relay controller");

        ControlBus _relayBus = null;
        bool operationMode;
        Stream commStream;

        public NumatoController() {
            this.XmlDocument.LoadXml(
                $"<NumatoController {interfaceTypeAttribute}=\"TCP\" {addressAttribute}=\"169.254.1.1\" {userAttribute}=\"admin\" {passwordAttribute}=\"admin\">" +
                "<ModeString>baud=115200 parity=N data=8</ModeString>" +
                "</NumatoController>"
                );
        }

        public bool Simulation { get; set; }

        public ControlBus RelayBus {
            get {
                if (_relayBus == null)
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

        public int RelaysCount {
            get { return XmlConvert.ToInt32(Element.GetAttribute(relaysCountAttribute)); }
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

            if (InterfaceType == InterfaceType.TCP)
                OutputManager.AddCommand(new LoginCommand(this, User, Password));
            else
                OutputManager.AddCommand(new EmptyCommand(this));
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
            if (!Simulation) {
                if (InterfaceType == InterfaceType.Serial)
                    commStream = (FileStream)EventManager.Event(new LayoutEvent(Element, "open-serial-communication-device-request"));
                else
                    commStream = new TcpClient(Address, 23).GetStream();
            }
		}

		void CloseCommunicationStream() {
            if (!Simulation) {
                commStream.Close();
                commStream = null;
            }
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

			Simulation= e.Sender.Simulation;

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

		[LayoutEvent("numato-invoke-events", IfEvent = "*[CommandStation/@ID='`string(@ID)`']")]
		private void ncdInvokeEvents(LayoutEvent e0) {
			var e = (LayoutEvent<IList<LayoutEvent>>)e0;

			foreach(var anEvent in e.Sender)
				EventManager.Event(anEvent);
		}

		#endregion

		#region Command classes

		public abstract class NumatoCommandBase : OutputSynchronousCommandBase {
			protected NumatoController RelayController { get; }

			public NumatoCommandBase(NumatoController relayController) {
				this.RelayController = relayController;
			}

			public override void OnReply(object reply) {
			}

			protected abstract string Command { get;  }

			protected void SendCommand(Stream stream) {
				if(TraceNumato.TraceInfo)
					Trace.WriteLine($"NumatoRelayController: Sending: {Command}");

                if (!this.RelayController.Simulation) {
                    try {
                        byte[] commandBuffer = Encoding.UTF8.GetBytes(this.Command);

                        stream.Write(commandBuffer, 0, commandBuffer.Length);
                        stream.Flush();
                    }
                    catch (OperationCanceledException) { }
                }
			}

			protected virtual void OnReply(byte[] replyBuffer) {
			}

            const int maxReplySize = 100;

			public override void Do() {
                var reply = new StringBuilder();
                var replyLength = 0;
				SendCommand(RelayController.CommStream);

                if (!RelayController.Simulation) {
                    do {
                        var b = RelayController.CommStream.ReadByte();

                        if (b == '>')       // If got command prompt, reply has been received
                            break;

                        reply.Append(Convert.ToChar(b));
                        replyLength++;

                    } while (replyLength < maxReplySize);        // Some sanity check

                    if (replyLength == maxReplySize)
                        throw new FormatException("Invalid Numator Relay controller reply (too long, probably junk)");
                }

                RelayController.OutputManager.SetReply(reply.ToString());
			}
		}

        class EmptyCommand : NumatoCommandBase {
            public EmptyCommand(NumatoController relayController) : base(relayController) { }

            protected override string Command => "\r";
            public override string ToString() => "Empty command (just \\r)";

        }

        class LoginCommand : NumatoCommandBase {
            string User { get; }
            string Password { get; }

            public LoginCommand(NumatoController relayController, string user, string password) : base(relayController) {
                this.User = user;
                this.Password = password;
            }

            protected override string Command => $"{User}\r{Password}\r";

            public override string ToString() => $"Login as {User}, password {Password}";
        }
		
		class SetRelayCommand : NumatoCommandBase {
			int iRelay;
			bool on;

			public SetRelayCommand(NumatoController relayController, int iRelay, bool on) : base(relayController) {
				this.iRelay = iRelay;
				this.on = on;
			}

            protected override string Command => $"relay {(on ? "on" : "off")} {iRelay}\r";

            public override string ToString() => $"Set relay {iRelay} to {(on ? "ON" : "OFF")}";

            public override void OnReply(object reply) {
                base.OnReply(reply);

                EventManager.Instance.InterThreadEventInvoker.QueueEvent(new LayoutEvent<IList<LayoutEvent>>("numato-invoke-events", new LayoutEvent[] {
                    new LayoutEvent<ControlConnectionPointReference, int>("control-connection-point-state-changed-notification", new ControlConnectionPointReference(RelayController.RelayBus, iRelay), on ? 1 : 0)
                }).SetCommandStation(RelayController));
            }
        }

		#endregion
	}
}
