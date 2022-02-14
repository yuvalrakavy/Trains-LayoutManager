using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MethodDispatcher;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

#pragma warning disable 
namespace LayoutLGB {
    /// <summary>
    /// Summary description for MTScomponents.
    /// </summary>
    public class MTScentralStation : LayoutCommandStationComponent {
        private const string A_OverlappedIO = "OverlappedIO";
        private const string A_ReadIntervalTimeout = "ReadIntervalTimeout";
        private const string A_ReadTotalTimeoutConstant = "ReadTotalTimeoutConstant";
        private const string A_XbusId = "XbusID";
        private const string A_MinTimeBetweenSpeedSteps = "MinTimeBetweenSpeedSteps";
        private static readonly LayoutTraceSwitch traceMTS = new LayoutTraceSwitch("LGBMTS", "LGB Multi-Train-System Central Station");
        private static readonly LayoutTraceSwitch traceRawInput = new LayoutTraceSwitch("MTSrawInput", "Trace raw MTS central station input");
        private OutputManager commandStationManager;

        private readonly byte[] inputBuffer = new byte[4];
        private readonly AutoResetEvent readAbortedEvent = new AutoResetEvent(false);

        private ControlBus _LGBbus = null;
        private ControlBus _DCCbus = null;

        public MTScentralStation() {
            this.XmlDocument.LoadXml(
                "<CommandStation>" +
                "<ModeString>baud=9600 parity=N data=8 octs=on odsr=on</ModeString>" +
                "</CommandStation>"
                );
        }

        public int XbusID => (int?)Element.AttributeValue(A_XbusId) ?? 1;

        public ControlBus LGBbus => _LGBbus ?? (_LGBbus = LayoutModel.ControlManager.Buses.GetBus(this, "LGBBUS"));

        public ControlBus DCCbus => _DCCbus ?? (_DCCbus = LayoutModel.ControlManager.Buses.GetBus(this, "DCC"));

        #region Specific implementation to base class overrideble methods and properties

        public override IList<string> BusTypeNames => Array.AsReadOnly<string>(new string[] { "LGBDCC", "LGBBUS" });

        public override DigitalPowerFormats SupportedDigitalPowerFormats => DigitalPowerFormats.NRMA;

        protected override void OnCommunicationSetup() {
            base.OnCommunicationSetup();

            if (!Element.HasAttribute(A_OverlappedIO))
                Element.SetAttributeValue(A_OverlappedIO, true);
            if (!Element.HasAttribute(A_ReadIntervalTimeout))
                Element.SetAttributeValue(A_ReadIntervalTimeout, 100);
            if (!Element.HasAttribute(A_ReadTotalTimeoutConstant))
                Element.SetAttributeValue(A_ReadTotalTimeoutConstant, 5000);
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            _LGBbus = null;
            _DCCbus = null;

            commandStationManager = new OutputManager(Name, 1);
            commandStationManager.Start();

            // And finally begin an asynchronous read
            CommunicationStream.BeginRead(inputBuffer, 0, inputBuffer.Length, new AsyncCallback(this.OnReadDone), null);
        }

        protected override async Task OnTerminateCommunication() {
            await base.OnTerminateCommunication();

            if (commandStationManager != null) {
                await commandStationManager.WaitForIdle();
                commandStationManager.Terminate();
                commandStationManager = null;
            }
        }

        public override bool LayoutEmulationSupported => true;

        public override bool DesignTimeLayoutActivationSupported => true;

        public override int GetHighestLocomotiveAddress(DigitalPowerFormats format) => 22;      // Poor thing...

        protected override ILayoutCommandStationEmulator CreateCommandStationEmulator(string pipeName) => new MTScommandStationEmulator(this, pipeName);

        #endregion

        #region Request Event Handlers
#pragma warning disable IDE0051

        [LayoutEvent("get-command-station-capabilities", IfEvent = "*[CommandStation/@Name='`string(Name)`']")]
        private void GetCommandStationCapabilities(LayoutEvent e) {
            CommandStationCapabilitiesInfo cap = new CommandStationCapabilitiesInfo {
                MinTimeBetweenSpeedSteps = (int?)Element.AttributeValue(A_MinTimeBetweenSpeedSteps) ?? 100
            };
            e.Info = cap.Element;
        }

        [LayoutEvent("disconnect-power-request")]
        private void PowerDisconnectRequest(LayoutEvent e) {
            if (e.Sender == null || e.Sender == this) {
                commandStationManager.AddCommand(new MTSpowerDisconnect(CommunicationStream));
                commandStationManager.AddCommand(new MTSresetStation(CommunicationStream));
            }

            PowerOff();
        }

        [LayoutEvent("connect-power-request")]
        private void PowerConnectRequest(LayoutEvent e) {
            if (e.Sender == null || e.Sender == this) {
                commandStationManager.AddCommand(new MTSresetStation(CommunicationStream));
                commandStationManager.AddCommand(new MTSpowerConnect(CommunicationStream, XbusID));
            }

            PowerOn();
        }

        #region Set function number support
        [LayoutEvent("get-command-station-set-function-number-support", IfEvent = "*[CommandStation/@Name='`string(Name)`']")]
        private void getSetFunctionNumberSupport(LayoutEvent e) {
            e.Info = new CommandStationSetFunctionNumberSupportInfo() {
                SetFunctionNumberSupport = SetFunctionNumberSupport.FunctionNumber
            };
        }
        #endregion
        // Implement command events
        [DispatchTarget]
        private Task ChangeTrackComponentStateCommand([DispatchFilter(Type = "IsMyId")] IModelComponentHasNameAndId commandStation, ControlConnectionPointReference connectionPointRef, int state) {
            int address = connectionPointRef.Module.Address + connectionPointRef.Index;

            var task = commandStationManager.AddCommand(new MTSchangeAccessoryState(CommunicationStream, address, state));
            DCCbusNotification(address, state);

            return task;
        }

        [LayoutEvent("change-signal-state-command", IfEvent = "*[CommandStation/@ID='`string(@ID)`']")]
        private void changeSignalStateCommand(LayoutEvent e) {
            ControlConnectionPoint connectionPoint = (ControlConnectionPoint)e.Sender;
            LayoutSignalState state = (LayoutSignalState)e.Info;
            int address = connectionPoint.Module.Address + connectionPoint.Index;
            int v;

            if (state == LayoutSignalState.Green)
                v = 1;
            else
                v = 0;

            commandStationManager.AddCommand(new MTSchangeAccessoryState(CommunicationStream, (byte)address, state == LayoutSignalState.Green ? 1 : 0));
            DCCbusNotification(address, v);
        }

        [DispatchTarget]
        private void LocomotiveMotionCommand([DispatchFilter(Type = "IsMyId")] IModelComponentHasNameAndId commandStation, LocomotiveInfo loco, int speed) {
            commandStationManager.AddCommand(new MTSlocomotiveMotion(CommunicationStream, (byte)loco.AddressProvider.Unit, speed));
        }

        [DispatchTarget]
        private void SetLocomotiveLightsCommand([DispatchFilter(Type = "IsMyId")] IModelComponentHasNameAndId commandStation, LocomotiveInfo loco, bool lights) {
            commandStationManager.AddCommand(new MTSlocomotiveFunction(CommunicationStream, loco.AddressProvider.Unit, 0x80));
        }

        [DispatchTarget(Name = "TriggerLocomotiveFunctionStateCommand")]
        [DispatchTarget]
        void SetLocomotiveFunctionStateCommand([DispatchFilter(Type = "IsMyId")] IModelComponentHasNameAndId commandStation, LocomotiveInfo locomotive, string functionName, bool functionState) {
            LocomotiveFunctionInfo function = locomotive.GetFunctionByName(functionName);

            commandStationManager.AddCommand(new MTSlocomotiveFunction(CommunicationStream, locomotive.AddressProvider.Unit, function.Number));
        }

        [LayoutEvent("trigger-locomotive-function-number", IfEvent = "*[CommandStation/@Name='`string(Name)`']")]
        private void triggerLocomotiveFunctionNumber(LayoutEvent e) {
            if (e.Sender is LocomotiveInfo loco && e.Info is int functionNumber) {
                commandStationManager.AddCommand(new MTSlocomotiveFunction(CommunicationStream, loco.AddressProvider.Unit, functionNumber));
            }
        }

        #endregion

        #region Generate notification events

        private void LGBbusNotification(int address, int state) {
            if (OperationMode) {
                ControlConnectionPointReference connectionPointRef;
                ControlModule lgbBusModule = LGBbus.GetModuleUsingAddress(address);

                if (lgbBusModule != null) {
                    if (lgbBusModule.ModuleTypeName == "LGB55070") {            // Feedback module
                        connectionPointRef = new ControlConnectionPointReference(lgbBusModule, ((address - lgbBusModule.Address) * 2) + state);
                        state = 1;
                    }
                    else
                        connectionPointRef = new ControlConnectionPointReference(lgbBusModule, address - lgbBusModule.Address);

                    Dispatch.Notification.OnControlConnectionPointStateChanged(connectionPointRef, state);
                }
            }
        }

        private void DCCbusNotification(int address, int state) {
            if (OperationMode)
                Dispatch.Notification.OnControlConnectionPointStateChanged(new ControlConnectionPointReference(DCCbus, address), state);
        }

        #endregion

        #region MTS interface input handling

        [LayoutEvent("parse-MTS-message")]
        private void parseInputMessage(LayoutEvent e) {
            if (e.Sender == this) {
                MTSmessage message = (MTSmessage)e.Info;

                switch (message.Command) {
                    case MTScommand.TurnoutControl:
                        if (OperationMode) {
                            if (message.Address <= 128)
                                DCCbusNotification(message.Address, message.Value);
                            LGBbusNotification(message.Address, message.Value);
                        }
                        else if (LayoutController.IsDesignTimeActivation)
                            EventManager.Event(new LayoutEvent("design-time-command-station-event", this, new CommandStationInputEvent(this, message.Address <= 128 ? DCCbus : LGBbus, message.Address, message.Value),
                                null));
                        break;

                    case MTScommand.ResetStation:
                        if (message.Value == 0x80) {
                            PowerOff();
                            MessageBox.Show(LayoutController.ActiveFrameWindow,
                                "Emergency stop has been engaged. Click on OK to reactivate layout", "Emergency Stop",
                                MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            EventManager.Event(new LayoutEvent("connect-power-request", this));
                        }
                        else if (message.Value != 0x81)
                            Warning("Unexpected reset station message (value " + message.Value + ")");
                        break;

                    case MTScommand.LocomotiveFunction:
                        if (OperationMode && message.Value == 0x80)
                            Dispatch.Notification.OnLocomotiveLightsToggled(this, message.Address);
                        break;

                    case MTScommand.LocomotiveMotion: {
                            if (OperationMode) {
                                int speed = message.Value >= 0x20 ? message.Value - 0x20 : -(int)message.Value;

                                Dispatch.Notification.OnLocomotiveMotion(this, speed, message.Address);
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
        private void OnReadDone(IAsyncResult asyncReadResult) {
            if (CommunicationStream != null) {
                try {
                    int count = CommunicationStream.EndRead(asyncReadResult);

                    if (traceRawInput.TraceInfo) {
                        Trace.WriteLineIf(traceMTS.TraceInfo, "MTS Central Station - got " + count + " bytes: ");
                        for (int i = 0; i < count; i++)
                            Trace.WriteIf(traceMTS.TraceInfo, String.Format(" {0:x}", inputBuffer[i]));
                        Trace.WriteLineIf(traceMTS.TraceInfo, "");
                    }

                    for (int offset = 0; offset < count; offset += 4)
                        InterThreadEventInvoker.QueueEvent(new LayoutEvent("parse-MTS-message", this, new MTSmessage(inputBuffer, offset)));

                    // Start the next read
                    CommunicationStream.BeginRead(inputBuffer, 0, inputBuffer.Length, new AsyncCallback(this.OnReadDone), null);
                }
                catch (Exception ex) {
                    Trace.WriteLineIf(traceMTS.TraceInfo, "Pending read aborted (" + ex.Message + ")");
                    readAbortedEvent.Set();
                }
            }
        }

        #endregion
    }

    #region MTS Command Classes

    internal class MTScommandBase : OutputCommandBase {
        private readonly MTSmessage mtsMessage = null;

        public MTScommandBase(Stream stream) {
            this.Stream = stream;
        }

        public MTScommandBase(Stream stream, MTSmessage mtsMessage) {
            this.Stream = stream;
            this.mtsMessage = mtsMessage;
        }

        public Stream Stream { get; }

        public override void Do() {
            mtsMessage?.Send(Stream);
        }
    }

    internal class MTSresetStation : MTScommandBase {
        public MTSresetStation(Stream stream) : base(stream) {
            DefaultWaitPeriod = 250;
        }

        public override void Do() {
            new MTSmessage(MTScommand.ResetStation, 0x00, 0x80).Send(Stream);
        }
    }

    internal class MTSpowerDisconnect : MTScommandBase {
        public MTSpowerDisconnect(Stream stream) : base(stream) {
            DefaultWaitPeriod = 250;
        }

        public override void Do() {
            new MTSmessage(MTScommand.EmergencyStop, 0, 0).Send(Stream);
        }
    }

    internal class MTSpowerConnect : MTScommandBase {
        private readonly int xbusID;

        public MTSpowerConnect(Stream stream, int xbusID) : base(stream) {
            this.xbusID = xbusID;
        }

        public override void Do() {
            new MTSmessage(MTScommand.PowerStation, 0, (byte)xbusID).Send(Stream);
        }
    }

    internal class MTSchangeAccessoryState : MTScommandBase {
        private readonly int unit;
        private readonly int state;

        public MTSchangeAccessoryState(Stream stream, int unit, int state) : base(stream) {
            this.unit = unit;
            this.state = state;
        }

        public override void Do() {
            new MTSmessage(MTScommand.TurnoutControl, (byte)unit, (byte)state).Send(Stream);
        }
    }

    internal class MTSlocomotiveMotion : MTScommandBase {
        private readonly byte unit;
        private readonly int speed;

        public MTSlocomotiveMotion(Stream stream, byte unit, int speed) : base(stream) {
            this.unit = unit;
            this.speed = speed;
        }

        public override void Do() {
            byte speedValue;

            if (speed > 0)
                speedValue = (byte)(speed + 0x21);
            else
                speedValue = (byte)(-speed + 1);

            new MTSmessage(MTScommand.LocomotiveMotion, unit, speedValue).Send(Stream);

            if (speed == 0) {
                Thread.Sleep(100);
                new MTSmessage(MTScommand.LocomotiveMotion, unit, speedValue).Send(Stream);
                Thread.Sleep(100);
                new MTSmessage(MTScommand.ResetLocomotive, unit, 0x01).Send(Stream);
            }
        }
    }

    internal class MTSlocomotiveFunction : MTScommandBase {
        private readonly byte unit;
        private readonly byte functionNumber;

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
        PowerStation = 20,              // 0x14
        EmergencyStop = 22,     // 0x16
    }

    /// <summary>
    /// Encapsulate a message sent over the XBUS (LGB BUS)
    /// </summary>
    public class MTSmessage {
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
            Buffer[0] = (byte)command;
            Buffer[1] = p1;
            Buffer[2] = p2;
            UpdateChecksum();
        }

        private void buildMessageFromBuffer(byte[] buffer, int offset) {
            if (offset + 4 > buffer.Length)
                throw new ArgumentOutOfRangeException("buffer/offset", "MTS message is at least four bytes");

            for (int i = 0; i < 4; i++)
                Buffer[i] = buffer[offset + i];

            // Verify that the checksum is correct
            if ((Buffer[0] ^ Buffer[1] ^ Buffer[2]) != Buffer[3])
                throw new ArgumentException("buffer", "MTS Message has bad checksum");
        }

        protected void UpdateChecksum() {
            Buffer[3] = (byte)(Buffer[0] ^ Buffer[1] ^ Buffer[2]);
        }

        public void Send(Stream s) {
            s.Write(Buffer, 0, Buffer.Length);
            s.Flush();
        }

        public MTScommand Command {
            get {
                return (MTScommand)Buffer[0];
            }

            set {
                Buffer[0] = (byte)value;
                UpdateChecksum();
            }
        }

        public byte Address {
            get {
                return Buffer[1];
            }

            set {
                Buffer[1] = value;
                UpdateChecksum();
            }
        }

        public byte Value {
            get {
                return Buffer[2];
            }

            set {
                Buffer[2] = value;
                UpdateChecksum();
            }
        }

        public byte[] Buffer { get; } = new byte[4];

        public override string ToString() {
            MTScommand cmd = (MTScommand)Buffer[0];

            return String.Format("MTS command: {0} {1:x} {2:x} (Checksum {3:x})", cmd, Buffer[1], Buffer[2], Buffer[3]);
        }
    }

    #endregion
}
