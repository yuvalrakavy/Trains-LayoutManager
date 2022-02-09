using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Net.Sockets;
using MethodDispatcher;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

namespace NCDRelayController {
    public enum InterfaceType {
        Serial,
        TCP,
    };

    public class NCDRelayController : ModelComponent, IModelComponentIsBusProvider {
        private const string A_InterfaceType = "InterfaceType";
        private const string A_OverlappedIO = "OverlappedIO";
        private const string A_ReadIntervalTimeout = "ReadIntervalTimeout";
        private const string A_ReadTotalTimeoutConstant = "ReadTotalTimeoutConstant";
        private const string A_WriteTotalTimeoutConstant = "WriteTotalTimeoutConstant";
        private const string A_PollingPeriod = "PollingPeriod";
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible", Justification = "<Pending>")]
        public static LayoutTraceSwitch TraceNCD = new("NCDRelayController", "NCD Relay controller");

        private ControlBus? _relayBus = null;
        private ControlBus? _inputBus = null;
        private bool simulation;
        private bool operationMode;
        private Stream? commStream;
        private OutputManager? outputManager;

        public NCDRelayController() {
            this.XmlDocument.LoadXml(
                "<RelayController InterfaceType=\"Serial\">" +
                "<ModeString>baud=115200 parity=N data=8</ModeString>" +
                "</RelayController>"
                );
        }

        public bool LayoutEmulationSupported => false;

        public ControlBus RelayBus => _relayBus ??= Ensure.NotNull<ControlBus>(LayoutModel.ControlManager.Buses.GetBus(this, "NCDRelayBus"));

        public ControlBus InputBus => _inputBus ??= Ensure.NotNull<ControlBus>(LayoutModel.ControlManager.Buses.GetBus(this, "NCDInputBus"));

        private OutputManager OutputManager => Ensure.NotNull<OutputManager>(outputManager);

        private InterfaceType InterfaceType {
            get => Element.AttributeValue(A_InterfaceType).Enum<InterfaceType>() ?? InterfaceType.Serial;
            set => Element.SetAttributeValue(A_InterfaceType, value);
        }

        private Stream CommStream => Ensure.NotNull<Stream>(commStream);

        #region Communication init/cleanup

        private void OnInitialize() {
            _relayBus = null;
            _inputBus = null;

            TraceNCD.Level = TraceLevel.Off;

            outputManager = new OutputManager(NameProvider.Name, 1);
            outputManager.Start();

            outputManager.AddCommand(new EnableReportingCommand(this));

            if (InputBus.Modules.Count > 0)
                outputManager.AddIdleCommand(new PollContactClosuresCommand(this, (int)Element.AttributeValue(A_PollingPeriod)));
        }

        private void OnCleanup() {
        }

        private void OnCommunicationSetup() {
            if (!Element.HasAttribute(A_OverlappedIO))
                Element.SetAttributeValue(A_OverlappedIO, true);
            if (!Element.HasAttribute(A_ReadIntervalTimeout))
                Element.SetAttributeValue(A_ReadIntervalTimeout, 3500);
            if (!Element.HasAttribute(A_ReadTotalTimeoutConstant))
                Element.SetAttributeValue(A_ReadTotalTimeoutConstant, 6000);
            if (!Element.HasAttribute(A_WriteTotalTimeoutConstant))
                Element.SetAttributeValue(A_WriteTotalTimeoutConstant, 6000);
        }

        private void OpenCommunicationStream() {
            if (InterfaceType == InterfaceType.Serial) {
                commStream = Dispatch.Call.OpenSerialCommunicationDeviceRequest(Element);
            }
            else {
                string address = XmlInfo.DocumentElement.GetAttribute("Address");
                int portIndex = address.IndexOf(':');
                int port;

                if (portIndex < 0)
                    port = 2101;
                else {
                    _ = int.TryParse(address.AsSpan(portIndex + 1), out port);
                    address = address[..portIndex];
                }

                using var tcpClient = new TcpClient(address, port);

                commStream = tcpClient.GetStream();
            }
        }

        private void CloseCommunicationStream() {
            commStream?.Close();
            commStream = null;
        }

        private async Task OnTerminateCommunication() {
            if (outputManager != null) {
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

        public override ModelComponentKind Kind => ModelComponentKind.ControlComponent;

        public override bool DrawOutOfGrid => NameProvider.Element != null && NameProvider.Visible;

        #endregion

        #region IModelComponentIsBusProvider Members

        public IList<string> BusTypeNames => Array.AsReadOnly<string>(new string[] { "NCDRelayBus", "NCDInputBus" });

        public bool BatchMultipathSwitchingSupported => false;

        #endregion

        #region IObjectHasName Members

        public LayoutTextInfo NameProvider => new(this);

        #endregion

        #region Event Handlers

        [LayoutEvent("enter-operation-mode")]
        protected virtual void EnterOperationMode(LayoutEvent e) {
            simulation = Ensure.NotNull<OperationModeParameters>(e.Sender).Simulation;

            OnCommunicationSetup();
            OpenCommunicationStream();

            operationMode = true;
            OnInitialize();
        }

        [LayoutEvent("begin-design-time-layout-activation")]
        private void BeginDesignTimeLayoutActivation(LayoutEvent e) {
            OnCommunicationSetup();
            OpenCommunicationStream();
            OnInitialize();
            e.Info = true;
        }

        [LayoutAsyncEvent("exit-operation-mode-async")]
        protected async virtual Task ExitOperationModeAsync(LayoutEvent e) {
            if (operationMode) {
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

        [LayoutAsyncEvent("change-track-component-state-command", IfEvent = "*[CommandStation/@ID='`string(@ID)`']")]
        private Task ChangeTrackComponentStateCommand(LayoutEvent e) {
            var connectionPointRef = Ensure.NotNull<ControlConnectionPointReference>(e.Sender);
            bool on = Ensure.ValueNotNull<int>(e.Info) != 0;

            int iRelay = connectionPointRef.Module.Address + connectionPointRef.Index;

            return OutputManager.AddCommand(new SetRelayCommand(this, iRelay, on));
        }

        [LayoutEvent("NCD-invoke-events", IfEvent = "*[CommandStation/@ID='`string(@ID)`']")]
        private void NcdInvokeEvents(LayoutEvent e) {
            var events = Ensure.NotNull<IList<LayoutEvent>>(e.Sender);

            foreach (var anEvent in events)
                EventManager.Event(anEvent);
        }

        #endregion

        #region Command classes

        internal abstract class NCDcommandBase : OutputSynchronousCommandBase {
            private readonly byte[]? replyBuffer;

            protected NCDRelayController RelayController { get; }
            protected int ReplySize { get; set; }

            protected NCDcommandBase(NCDRelayController relayController, int replySize = 1) {
                this.RelayController = relayController;
                this.ReplySize = replySize;

                if (replySize > 0)
                    replyBuffer = new byte[replySize];
               
            }

            public override void OnReply(object replyPacket) {
            }

            protected abstract byte[] Command { get; }

            protected void SendCommand(Stream stream) {
                byte[] command = this.Command;

                if (TraceNCD.TraceInfo) {
                    string message = "NCDRelayController: Sending " + command.Length + " bytes:";

                    foreach (byte b in command)
                        message += " " + b.ToString();
                    Trace.WriteLine(message);
                }

                try {
                    stream.Write(command, 0, command.Length);
                    stream.Flush();
                }
                catch (OperationCanceledException) { }
            }

            protected virtual void OnReply(byte[] replyBuffer) {
                if (ReplySize > 0 && replyBuffer[0] != 85)
                    Trace.WriteLine(TraceNCD.TraceError, "Unexpected reply from controller: " + (int)replyBuffer[0]);
            }

            public override void Do() {
                SendCommand(RelayController.CommStream);

                if (ReplySize > 0 && replyBuffer != null) {
                    int count = RelayController.CommStream.Read(replyBuffer, 0, ReplySize);

                    if (TraceNCD.TraceVerbose) {
                        string message = "NCDRelayController: Got " + count + " bytes:";

                        foreach (byte b in replyBuffer)
                            message += " " + b.ToString();
                        Trace.WriteLine(message);
                    }

                    RelayController.OutputManager.SetReply(replyBuffer);
                }
            }
        }

        private class EnableReportingCommand : NCDcommandBase {
            public EnableReportingCommand(NCDRelayController relayController) : base(relayController) {
            }

            protected override byte[] Command => new byte[2] { 254, 27 };
        }

        private class SetRelayCommand : NCDcommandBase {
            private readonly int iRelay;
            private readonly bool on;

            public SetRelayCommand(NCDRelayController relayController, int iRelay, bool on) : base(relayController) {
                this.iRelay = iRelay;
                this.on = on;
            }

            protected override byte[] Command {
                get {
                    byte bank = (byte)((iRelay / 8) + 1);
                    byte command;

                    if (on)
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

            public override void OnReply(object replyPacket) {
                base.OnReply(replyPacket);

                EventManager.Instance.InterThreadEventInvoker.QueueEvent(new LayoutEvent<IList<LayoutEvent>>("NCD-invoke-events",
                    new LayoutEvent[] {
                        new LayoutEventInfoValueType<ControlConnectionPointReference, int>("control-connection-point-state-changed-notification", new ControlConnectionPointReference(RelayController.RelayBus, iRelay), on ? 1 : 0)
                    }).SetCommandStation(RelayController));
            }
        }

        private class PollContactClosuresCommand : OutputSynchronousCommandBase, IOutputIdlecommand {
            protected NCDRelayController RelayController { get; }

            private readonly ContactClosureBankData[] contactClosureData;
            private readonly int pollingPeriod;

            public PollContactClosuresCommand(NCDRelayController relayController, int pollingPeriod) {
                this.RelayController = relayController;
                this.pollingPeriod = pollingPeriod;
                this.contactClosureData = new ContactClosureBankData[relayController.InputBus.Modules.Sum(module => module.ModuleType.NumberOfConnectionPoints / 8)];
            }

            public override int Timeout => pollingPeriod;

            public override void Do() {
                var events = new List<LayoutEvent>();

                int moduleNumber = 0;
                foreach (var module in RelayController.InputBus.Modules) {
                    int bank = module.Address / 8;

                    for (int connectionPointIndex = 0; connectionPointIndex < module.ModuleType.NumberOfConnectionPoints; connectionPointIndex += 8) {
                        //TODO: Poll bank if not in operation mode (design mode learn) or if operation mode and at least one connection point is actually connected 
                        PollContactClosureBank(events, module, moduleNumber, bank);
                        bank++;
                    }

                    moduleNumber++;
                }

                if (events.Count > 0)
                    EventManager.Instance.InterThreadEventInvoker.QueueEvent(new LayoutEvent<IList<LayoutEvent>>("NCD-invoke-events", events).SetCommandStation(RelayController));

                RelayController.OutputManager.SetReply(null);
            }

            private void PollContactClosureBank(IList<LayoutEvent> events, ControlModule module, int moduleNumber, int bank) {
                byte[] command = new byte[] { 254, 175, (byte)bank };
                byte[] reply = new byte[1];

                RelayController.CommStream.Write(command, 0, command.Length);
                RelayController.CommStream.Read(reply, 0, reply.Length);

                Trace.WriteLineIf(TraceNCD.TraceVerbose, "Bank " + bank + " value: " + reply[0].ToString("x"));
                contactClosureData[bank].NewData(events, module, moduleNumber, reply[0]);
            }

            public override void OnReply(object replyPacket) {
            }

            #region IOutputIdlecommand Members

            public bool RemoveFromQueue => false;

            #endregion

            private struct ContactClosureBankData {
                private byte currentState;

                public void NewData(IList<LayoutEvent> events, ControlModule module, int moduleNumber, byte data) {
                    byte changedBits = (byte)(currentState ^ data);
                    byte mask = 1;
                    IModelComponentIsBusProvider busProvider = module.Bus.BusProvider;

                    for (int i = 0; i < 8; i++) {
                        if ((changedBits & mask) != 0) {
                            int isSet = (data & mask) != 0 ? 1 : 0;

                            if (LayoutController.IsOperationMode)
                                events.Add(new LayoutEvent("control-connection-point-state-changed-notification", new ControlConnectionPointReference(module.Bus, moduleNumber + 1, i), isSet));
                            else if (LayoutController.IsDesignTimeActivation)
                                events.Add(new LayoutEvent("design-time-command-station-event", busProvider, new CommandStationInputEvent((ModelComponent)busProvider, module.Bus, moduleNumber + 1, i, isSet),
                                    null));
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
