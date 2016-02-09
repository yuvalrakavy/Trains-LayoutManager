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

        public int RelaysCount => XmlConvert.ToInt32(Element.GetAttribute(relaysCountAttribute));

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

        public override void Error(object subject, string message) {
           EventManager.Instance.InterThreadEventInvoker.QueueEvent(new LayoutEvent(subject, "add-error", null, message));
        }

        #region Communication init/cleanup

        void OnInitialize() {
			_relayBus = null;

			TraceNumato.Level = TraceLevel.Off;

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

			OnTerminateCommunication().Wait(100);
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

			protected virtual string Command { get;  } = "";

            protected void Send(byte[] command) {
                Trace.WriteLineIf(TraceNumato.TraceInfo, $"NumatoRelayController: Sending: {Encoding.UTF8.GetString(command)}");

                if (!this.RelayController.Simulation)
                    RelayController.commStream.Write(command, 0, command.Length);

            }

            protected void Send(string whatToSend) => Send(Encoding.UTF8.GetBytes(whatToSend));

            protected void SendCommand() => Send(this.Command);

			protected virtual void OnReply(byte[] replyBuffer) {
			}

            const int maxReplySize = 1024;

            private static bool isSuffixOf(byte[] suffix, byte[] buffer, int endOfBufferIndex) {
                if (endOfBufferIndex < suffix.Length)
                    return false;

                int offset = endOfBufferIndex - suffix.Length;

                for (int i = 0; i < suffix.Length; i++)
                    if (suffix[i] != buffer[offset + i])
                        return false;

                return true;
            }

            protected string CollectReply(byte[] expectToGet) {
                byte[] reply = new byte[maxReplySize];
                var expectLength = expectToGet.Length;
                var index = 0;

                if (!RelayController.Simulation) {
                    do {
                        RelayController.CommStream.Read(reply, index, 1);
                        index++;            // Read an additional byte

                        if(isSuffixOf(expectToGet, reply, index))
                            break;

                    } while (index < reply.Length);

                    if (index == reply.Length)
                        throw new FormatException("Invalid Numator Relay controller reply (too long, probably junk)");
                }

                return reply.ToString();
            }

            protected string CollectReply(string expectToGet) => CollectReply(Encoding.UTF8.GetBytes(expectToGet));

			public override void Do() {
				SendCommand();
                RelayController.OutputManager.SetReply(CollectReply(">"));
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

            public override void Do() {
                try {
                    CollectReply("Name: ");
                    Send($"{this.User}\r\n");
                    // Expect password: prompt + DO Suppress Echo Telnet command
                    CollectReply(Encoding.UTF8.GetBytes("Password: ").Concat(new byte[] { 0xff, 0xfd, 0x2d }).ToArray());
                    Send(new byte[] { 0xff, 0xfc, 0x2d });      // Send Telnet WON'T (Suppress echo) Do reply
                    Send($"{Password}\r\n");
                    CollectReply(">>");

                    RelayController.OutputManager.SetReply("Login");
                }
                catch (EndOfStreamException) {
                    RelayController.Error($"Login command failed for {this.RelayController}");
                }
            }

            public override string ToString() => $"Login user {User}";
        }

        class SetRelayCommand : NumatoCommandBase {
			int iRelay;
			bool on;

			public SetRelayCommand(NumatoController relayController, int iRelay, bool on) : base(relayController) {
				this.iRelay = iRelay;
				this.on = on;
			}


            private static char RelayNumberCharacter(int iRelay) => (char)(iRelay < 10 ? iRelay + '0' : ((iRelay - 10) + 'A'));
            protected override string Command => $"relay {(on ? "on" : "off")} {RelayNumberCharacter(iRelay)}\r\n";

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
