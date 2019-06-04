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

namespace NumatoController {
    public enum InterfaceType {
        Serial,
        TCP,
    };

    public class NumatoController : LayoutBusProviderSupport, IModelComponentIsBusProvider {
        internal const string A_Relays = "Relays";
        internal const string A_User = "User";
        internal const string A_Password = "Password";
        private const string A_OverlappedIO = "OverlappedIO";
        private const string A_ReadIntervalTimeout = "ReadIntervalTimeout";
        private const string A_ReadTotalTimeoutConstant = "ReadTotalTimeoutConstant";
        private const string A_WriteTotalTimeoutConstant = "WriteTotalTimeoutConstant";
        public static LayoutTraceSwitch TraceNumato = new LayoutTraceSwitch("NumatoController", "Numato Relay controller");

        private ControlBus _relayBus = null;

        public NumatoController() {
            this.XmlDocument.LoadXml(
                $"<NumatoController {A_InterfaceType}=\"TCP\" {A_Address}=\"169.254.1.1\" {A_User}=\"admin\" {A_Password}=\"admin\">" +
                "<ModeString>baud=115200 parity=N data=8</ModeString>" +
                "</NumatoController>"
                );
        }

        public bool Simulation { get; set; }

        public override bool LayoutEmulationSupported => true;

        public ControlBus RelayBus {
            get {
                return _relayBus ?? (_relayBus = LayoutModel.ControlManager.Buses.GetBus(this, "NumatoRelayBus"));
            }
        }

        protected override ILayoutCommandStationEmulator CreateCommandStationEmulator(string pipeName) => new NumatorEmulator(this, pipeName);

        private OutputManager OutputManager { get; set; }

        public int RelaysCount => (int)Element.AttributeValue(A_Relays);

        private string User {
            get { return Element.GetAttribute(A_User); }
            set { Element.SetAttribute(A_User, value); }
        }

        private string Password {
            get { return Element.GetAttribute(A_Password); }
            set { Element.SetAttribute(A_Password, value); }
        }

        #region Communication init/cleanup

        override protected void OnInitialize() {
            _relayBus = null;

            OutputManager = new OutputManager(NameProvider.Name, 1);
            OutputManager.Start();

            if (InterfaceType == CommunicationInterfaceType.TCP)
                OutputManager.AddCommand(new LoginCommand(this, User, Password));
            else
                OutputManager.AddCommand(new EmptyCommand(this));
        }

        override protected void OnCommunicationSetup() {
            if (!Element.HasAttribute(A_OverlappedIO))
                Element.SetAttribute(A_OverlappedIO, true);
            if (!Element.HasAttribute(A_ReadIntervalTimeout))
                Element.SetAttribute(A_ReadIntervalTimeout, 3500);
            if (!Element.HasAttribute(A_ReadTotalTimeoutConstant))
                Element.SetAttribute(A_ReadTotalTimeoutConstant, 6000);
            if (!Element.HasAttribute(A_WriteTotalTimeoutConstant))
                Element.SetAttribute(A_WriteTotalTimeoutConstant, 6000);
        }

        override protected async Task OnTerminateCommunication() {
            if (OutputManager != null) {
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

        public override bool DrawOutOfGrid => NameProvider.Element != null && NameProvider.Visible;

        #endregion

        #region IModelComponentIsBusProvider Members

        public override IList<string> BusTypeNames => Array.AsReadOnly<string>(new string[] { "NumatoRelayBus" });

        #endregion

        #region IObjectHasName Members

        #endregion

        #region Event Handlers
#pragma warning disable IDE0051, IDE0060

        [LayoutEvent("begin-design-time-layout-activation")]
        private void beginDesignTimeLayoutActivation(LayoutEvent e) {
            OnCommunicationSetup();
            OpenCommunicationStream();
            OnInitialize();
            e.Info = true;
        }

        [LayoutEvent("end-design-time-layout-activation")]
        private void EndDesignTimeLayoutActivation(LayoutEvent e) {
            OnCleanup();

            OnTerminateCommunication().Wait(100);
            CloseCommunicationStream();
            e.Info = true;
        }

        [LayoutAsyncEvent("change-track-component-state-command", IfEvent = "*[CommandStation/@ID='`string(@ID)`']")]
        private Task changeTrackComponentStateCommand(LayoutEvent e0) {
            var e = (LayoutEventInfoValueType<ControlConnectionPointReference, int>)e0;
            var connectionPointRef = e.Sender;

            int iRelay = connectionPointRef.Module.Address + connectionPointRef.Index;
            bool on = e.Info != 0;

            return OutputManager.AddCommand(new SetRelayCommand(this, iRelay, on));
        }

        [LayoutEvent("numato-invoke-events", IfEvent = "*[CommandStation/@ID='`string(@ID)`']")]
        private void numatoInvokeEvents(LayoutEvent e0) {
            var e = (LayoutEvent<IList<LayoutEvent>>)e0;

            foreach (var anEvent in e.Sender)
                EventManager.Event(anEvent);
        }

        #endregion

        #region Command classes

        public abstract class NumatoCommandBase : OutputSynchronousCommandBase {
            protected NumatoController RelayController { get; }

            protected NumatoCommandBase(NumatoController relayController) {
                this.RelayController = relayController;
            }

            public override void OnReply(object replyPacket) {
            }

            protected virtual string Command { get; } = "";

            protected void Send(byte[] command) {
                Trace.WriteLineIf(TraceNumato.TraceInfo, $"NumatoRelayController: Sending: {Encoding.UTF8.GetString(command)}");

                RelayController.CommunicationStream.Write(command, 0, command.Length);
            }

            protected void Send(string whatToSend) => Send(Encoding.UTF8.GetBytes(whatToSend));

            protected void SendCommand() => Send(this.Command);

            protected virtual void OnReply(byte[] replyBuffer) {
            }

            private const int maxReplySize = 1024;

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
                var _ = expectToGet.Length; // expectLength
                var index = 0;

                do {
                    RelayController.CommunicationStream.Read(reply, index, 1);
                    index++;            // Read an additional byte

                    if (isSuffixOf(expectToGet, reply, index))
                        break;

                } while (index < reply.Length);

                if (index == reply.Length)
                    throw new FormatException("Invalid Numator Relay controller reply (too long, probably junk)");

                return reply.ToString();
            }

            protected string CollectReply(string expectToGet) => CollectReply(Encoding.UTF8.GetBytes(expectToGet));

            public override void Do() {
                SendCommand();
                RelayController.OutputManager.SetReply(CollectReply(">"));
            }
        }

        private class EmptyCommand : NumatoCommandBase {
            public EmptyCommand(NumatoController relayController) : base(relayController) { }

            protected override string Command => "\r";
            public override string ToString() => "Empty command (just \\r)";
        }

        private class LoginCommand : NumatoCommandBase {
            private string User { get; }
            private string Password { get; }

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

        private class SetRelayCommand : NumatoCommandBase {
            private readonly int iRelay;
            private readonly bool on;

            public SetRelayCommand(NumatoController relayController, int iRelay, bool on) : base(relayController) {
                this.iRelay = iRelay;
                this.on = on;
            }

            private static char RelayNumberCharacter(int iRelay) => (char)(iRelay < 10 ? iRelay + '0' : ((iRelay - 10) + 'A'));
            protected override string Command => $"relay {(on ? "on" : "off")} {RelayNumberCharacter(iRelay)}\r\n";

            public override string ToString() => $"Set relay {iRelay} to {(on ? "ON" : "OFF")}";

            public override void OnReply(object replyPacket) {
                base.OnReply(replyPacket);

                EventManager.Instance.InterThreadEventInvoker.QueueEvent(new LayoutEvent<IList<LayoutEvent>>("numato-invoke-events", new LayoutEvent[] {
                    new LayoutEventInfoValueType<ControlConnectionPointReference, int>("control-connection-point-state-changed-notification", new ControlConnectionPointReference(RelayController.RelayBus, iRelay), on ? 1 : 0)
                }).SetCommandStation(RelayController));
            }
        }

        #endregion
    }
}
