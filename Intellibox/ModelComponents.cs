using LayoutManager;
using LayoutManager.Components;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace Intellibox {
    public class IntelliboxComponentInfo : LayoutInfo {
        private const string A_PollingPeriod = "PollingPeriod";
        private const string A_AccessorySwitchingTime = "AccessorySwitchingTime";
        private const string A_OperationModeDebounceCount = "OperationModeDebounceCount";
        private const string A_DesignTimeDebounceCount = "DesignTimeDebounceCount";

        public IntelliboxComponentInfo(IntelliboxComponent commandStation, XmlElement element) : base(element) {
            this.CommandStation = commandStation;
        }

        public IntelliboxComponent CommandStation { get; }

        public string Port {
            get => Element.GetAttribute(LayoutIOServices.A_Port);
            set => Element.SetAttribute(LayoutIOServices.A_Port, value);
        }

        public int PollingPeriod {
            get => (int?)Element.AttributeValue(A_PollingPeriod) ?? 20;
            set => Element.SetAttributeValue(A_PollingPeriod, value);
        }

        public int AccessoryCommandTime {
            get => (int?)Element.AttributeValue(A_AccessorySwitchingTime) ?? 100;
            set => Element.SetAttributeValue(A_AccessorySwitchingTime, value);
        }

        public byte OperationModeDebounceCount {
            get => (byte?)Element.AttributeValue(A_OperationModeDebounceCount) ?? 1;
            set => Element.SetAttributeValue(A_OperationModeDebounceCount, value);
        }

        public byte DesignTimeDebounceCount {
            get => (byte?)Element.AttributeValue(A_DesignTimeDebounceCount) ?? 1;
            set => Element.SetAttributeValue(A_DesignTimeDebounceCount, value);
        }
    }

    /// <summary>
    /// Implementation of Intellibox interface
    /// </summary>
    public class IntelliboxComponent : LayoutCommandStationComponent {
        private const string A_MinTimeBetweenSpeedSteps = "MinTimeBetweenSpeedSteps";
        //private static read only LayoutTraceSwitch traceIntellibox = new LayoutTraceSwitch("Intellibox", "Intellibox Command Station");
        private OutputManager? commandStationManager;

        private ControlBus? _motorolaBus = null;
        private ControlBus? _S88bus = null;

        private const int queuePowerCommands = 0;               // Queue for power on/off etc.
        private const int queueLayoutSwitchingCommands = 1; // Queue for turnout/lights control commands
        private const int queueLocoCommands = 2;                // Queue for locomotive control commands

        private const int switchBatchSize = 10;             // Change the state of upto 10 solenoids

        public IntelliboxComponent() {
            this.XmlDocument.LoadXml(
                "<CommandStation PollingPeriod='20'>" +
                "<ModeString>baud=19200 parity=N data=8 stop=2 octs=off odsr=off</ModeString>" +
                "</CommandStation>"
                );

            new SOcollection(Element).Add(33, 0, "Disable echo to I2C bus");
        }

        public IntelliboxComponentInfo Info => new(this, Element);

        public ControlBus MotorolaBus => _motorolaBus ??= Ensure.NotNull<ControlBus>(LayoutModel.ControlManager.Buses.GetBus(this, "Motorola"));

        public ControlBus S88Bus => _S88bus ??= Ensure.NotNull<ControlBus>(LayoutModel.ControlManager.Buses.GetBus(this, "S88BUS"));

        internal byte CachedOperationModeDebounceCount { get; set; }

        internal byte CachedDesignTimeDebounceCount { get; set; }

        #region Specific implementation to base class overrideble methods and properties

        public override IList<string> BusTypeNames => Array.AsReadOnly<string>(new string[] { "Motorola", "S88BUS" });

        //TODO: Add DigitalPowerFormats.NRMA when support for this is added. (Intellibox support mutiple on track formats)
        public override DigitalPowerFormats SupportedDigitalPowerFormats => DigitalPowerFormats.Motorola;

        protected override void OnCommunicationSetup() {
            base.OnCommunicationSetup();

            CachedOperationModeDebounceCount = Info.OperationModeDebounceCount;
            CachedDesignTimeDebounceCount = Info.DesignTimeDebounceCount;

            if (!Element.HasAttribute(LayoutIOServices.A_ReadIntervalTimeout))
                Element.SetAttributeValue(LayoutIOServices.A_ReadIntervalTimeout, 100);
            if (!Element.HasAttribute(LayoutIOServices.A_ReadTotalTimeoutConstant))
                Element.SetAttributeValue(LayoutIOServices.A_ReadTotalTimeoutConstant, 5000);
            if (!Element.HasAttribute(LayoutIOServices.A_BufferSize))
                Element.SetAttributeValue(LayoutIOServices.A_BufferSize, 1);
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            _motorolaBus = null;
            _S88bus = null;

            commandStationManager = new OutputManager(Name, 3);
            commandStationManager.Start();

            commandStationManager.AddIdleCommand(new InterlliboxProcessEventsCommand(this, Info.PollingPeriod));
            commandStationManager.AddIdleCommand(new EndTrainsAnalysisCommandStationCommand(this, 10));

            foreach (SOinfo so in new SOcollection(Element))
                commandStationManager.AddCommand(queuePowerCommands, new IntelliboxSetSOcommand(this, so.Number, so.Value));

            if (OperationMode)
                commandStationManager.AddCommand(queuePowerCommands, new IntelliboxForceSensorEventCommand(this));
        }

        protected override async Task OnTerminateCommunication() {
            await base.OnTerminateCommunication();

            if (commandStationManager != null) {
                await commandStationManager.WaitForIdle();
                commandStationManager.Terminate();
                commandStationManager = null;
            }
        }

        public override bool LayoutEmulationSupported => false;

        public override bool DesignTimeLayoutActivationSupported => true;

        public override bool BatchMultipathSwitchingSupported => true;

        public override bool TrainsAnalysisSupported => true;

        protected override ILayoutCommandStationEmulator? CreateCommandStationEmulator(string pipeName) => null;

        #endregion

        #region Request Event Handlers

        [DispatchTarget]
        private XmlElement GetCommandStationCapabilities([DispatchFilter(Type = "IsMyId")] IModelComponentHasNameAndId commandStation) {
            return new CommandStationCapabilitiesInfo {
                MinTimeBetweenSpeedSteps = (int?)Element.AttributeValue(A_MinTimeBetweenSpeedSteps) ?? 100
            }.Element;
        }

        [DispatchTarget]
        private void DisconnectPowerRequest([DispatchFilter(Type = "IsMyId")] IModelComponentHasNameAndId commandStation) {
            commandStationManager?.AddCommand(queuePowerCommands, new IntelliboxPowerOffCommand(this));
            PowerOff();
        }

        [DispatchTarget]
        private void ConnectPowerRequest([DispatchFilter(Type = "IsMyId")] IModelComponentHasNameAndId commandStation) {
            commandStationManager?.AddCommand(queuePowerCommands, new IntelliboxPowerOnCommand(this));
            PowerOn();
        }

        // Implement command events
        [DispatchTarget]
        private Task ChangeTrackComponentStateCommand([DispatchFilter(Type = "IsMyId")] IModelComponentHasNameAndId commandStation, ControlConnectionPointReference connectionPointRef, int state) {
            if (commandStationManager == null)
                throw new NullReferenceException(nameof(commandStationManager));

            int address = connectionPointRef.Module.Address + connectionPointRef.Index;

            var tasks = new List<Task> {
                commandStationManager.AddCommand(queueLayoutSwitchingCommands, new IntelliboxAccessoryCommand(this, address, state)),
                commandStationManager.AddCommand(queueLayoutSwitchingCommands, new IntelliboxEndAccessoryCommand(this, address, state))
            };

            if (OperationMode)
                Dispatch.Notification.OnControlConnectionPointStateChanged(connectionPointRef, state);

            return Task.WhenAll(tasks);
        }

        [DispatchTarget]
        private Task ChangeBatchOfTrackComponentStateCommand([DispatchFilter(Type = "IsMyId")] Guid commandStationId, List<SwitchingCommand> switchingCommands) {
            if (commandStationManager == null)
                throw new NullReferenceException(nameof(commandStationManager));

            var tasks = new List<Task>();
            int count;

            for (int i = 0; i < switchingCommands.Count; i += count) {
                if (switchingCommands.Count - i > switchBatchSize)
                    count = switchBatchSize;
                else
                    count = switchingCommands.Count - i;

                tasks.Add(commandStationManager.AddCommand(queueLayoutSwitchingCommands, new IntelliboxAccessoryCommand(this, switchingCommands, i, count)));
                tasks.Add(commandStationManager.AddCommand(queueLayoutSwitchingCommands, new IntelliboxEndAccessoryCommand(this, switchingCommands, i, count)));

                if (OperationMode) {
                    for (int switchingCommandIndex = 0; switchingCommandIndex < count; switchingCommandIndex++) {
                        SwitchingCommand switchingCommand = switchingCommands[i + switchingCommandIndex];

                        Dispatch.Notification.OnControlConnectionPointStateChanged(switchingCommand.ControlPointReference, switchingCommand.SwitchState);
                    }
                }
            }

            return Task.WhenAll(tasks);
        }

        [DispatchTarget]
        private void ChangeSignalStateCommand([DispatchFilter(Type = "IsMyId")] IModelComponentHasNameAndId commandStation, ControlConnectionPointReference connectionPointRef, LayoutSignalState state) {
            if (commandStationManager == null)
                throw new NullReferenceException(nameof(commandStationManager));

            int address = connectionPointRef.Module.Address + connectionPointRef.Index;
            byte v;

            if (state == LayoutSignalState.Green)
                v = 0;
            else
                v = 1;

            commandStationManager.AddCommand(queueLayoutSwitchingCommands, new IntelliboxAccessoryCommand(this, address, v));
            commandStationManager.AddCommand(queueLayoutSwitchingCommands, new IntelliboxEndAccessoryCommand(this, address, v));

            if (OperationMode)
                Dispatch.Notification.OnControlConnectionPointStateChanged(connectionPointRef, state == LayoutSignalState.Green ? 1 : 0);
        }

        [DispatchTarget]
        private void LocomotiveMotionCommand([DispatchFilter(Type = "IsMyId")] IModelComponentHasNameAndId commandStation, LocomotiveInfo loco, int speed) {
            if (commandStationManager == null)
                throw new NullReferenceException(nameof(commandStationManager));

            var train = Ensure.NotNull<TrainStateInfo>(LayoutModel.StateManager.Trains[loco.Id]);
            LocomotiveOrientation direction = LocomotiveOrientation.Forward;
            int logicalSpeed;

            if (speed < 0) {
                speed = -speed;
                direction = LocomotiveOrientation.Backward;
            }

            if (speed == 0)
                logicalSpeed = 0;
            else {
                double factor = 126 / loco.SpeedSteps;

                logicalSpeed = (int)(1 + (speed * factor));
            }

            commandStationManager.AddCommand(queueLocoCommands, new IntelliboxLocomotiveCommand(this, loco.AddressProvider.Unit, logicalSpeed, direction, train.Lights));
        }

        [DispatchTarget]
        private void SetLocomotiveLightsCommand([DispatchFilter(Type = "IsMyId")] IModelComponentHasNameAndId commandStation, LocomotiveInfo loco, bool lights) {
            var train = Ensure.NotNull<TrainStateInfo>(LayoutModel.StateManager.Trains[loco.Id]);

            Dispatch.Call.LocomotiveMotionCommand(Dispatch.Call.GetCommandStation(train), loco, train.SpeedInSteps);
        }

        private byte GetFunctionMask(LocomotiveInfo loco) {
            var train = Ensure.NotNull<TrainStateInfo>(LayoutModel.StateManager.Trains[loco.Id]);
            byte functionMask = 0;

            for (int functionNumber = 0; functionNumber < 8; functionNumber++) {
                LocomotiveFunctionInfo? functionDef = loco.GetFunctionByNumber(functionNumber + 1);

                if (functionDef != null && train.GetFunctionState(functionDef.Name, loco.Id, false))
                    functionMask |= (byte)(1 << functionNumber);
            }

            return functionMask;
        }

        [DispatchTarget]
        void SetLocomotiveFunctionStateCommand([DispatchFilter(Type = "IsMyId")] IModelComponentHasNameAndId commandStation, LocomotiveInfo locomotive, string functionName, bool functionState) {
            if (commandStationManager == null)
                throw new NullReferenceException(nameof(commandStationManager));
            commandStationManager.AddCommand(queueLocoCommands, new IntelliboxLocomotiveFunctionsCommand(this, locomotive.AddressProvider.Unit, GetFunctionMask(locomotive)));
        }

        [DispatchTarget]
        void TriggerLocomotiveFunctionStateCommand([DispatchFilter(Type = "IsMyId")] IModelComponentHasNameAndId commandStation, LocomotiveInfo locomotive, string functionName, bool functionState) {
            var train = Ensure.NotNull<TrainStateInfo>(LayoutModel.StateManager.Trains[locomotive.Id]);
            var state = train.GetFunctionState(functionName, locomotive.Id, false);

            state = !state;
            train.SetLocomotiveFunctionState(functionName, locomotive.Id, state);
        }

        #endregion
    }

    #region SO and SO collection classes

    public class SOinfo : LayoutXmlWrapper {
        private const string A_Number = "Number";
        private const string A_Value = "Value";
        private const string A_Description = "Description";

        public SOinfo(XmlElement element)
            : base(element) {
        }

        public SOinfo()
            : base("SO") {
        }

        public int Number {
            get => (int)AttributeValue(A_Number);
            set => SetAttributeValue(A_Number, value);
        }

        public int Value {
            get => (int)AttributeValue(A_Value);
            set => SetAttributeValue(A_Value, value);
        }

        public string Description {
            get => GetAttribute(A_Description);
            set => SetAttributeValue(A_Description, value);
        }
    }

    public class SOcollection : LayoutXmlWrapper, IEnumerable<SOinfo> {
        public SOcollection(XmlElement parentElement) {
            OptionalElement = parentElement["SpecialOptions"];

            if (OptionalElement == null) {
                Element = parentElement.OwnerDocument.CreateElement("SpecialOptions");
                parentElement.AppendChild(Element);
            }
        }

        public SOinfo Add(SOinfo item) {
            XmlElement e;

            if (item.Element.OwnerDocument != Element.OwnerDocument)
                e = (XmlElement)Element.OwnerDocument.ImportNode(item.Element, true);
            else
                throw new ArgumentException("Adding SO already in collection");

            Element.AppendChild(e);
            return new SOinfo(e);
        }

        public SOinfo Add(int number, int v, string description) {
            SOinfo so = new() {
                Number = number,
                Value = v,
                Description = description
            };
            return Add(so);
        }

        public int Count => Element.ChildNodes.Count;

        public bool Remove(SOinfo item) {
            if (item.Element.OwnerDocument == Element.OwnerDocument) {
                Element.RemoveChild(item.Element);
                return true;
            }
            else
                return false;
        }

        #region IEnumerable Members

        public IEnumerator<SOinfo> GetEnumerator() {
            foreach (XmlElement e in Element)
                yield return new SOinfo(e);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }

    #endregion

    #region Intellibox Command Classes

    internal abstract class IntelliboxCommand : OutputCommandBase {
        public static LayoutTraceSwitch traceIntelliboxRaw = new("Intellibox Raw data", "Intellibox raw input/output");

        protected IntelliboxCommand(IntelliboxComponent commandStation) {
            this.CommandStation = commandStation;
            this.Stream = commandStation.CommunicationStream;
        }

        protected IntelliboxComponent CommandStation { get; }

        protected Stream? Stream { get; }

        protected void BeginCommand() {
            Stream?.WriteByte((byte)'x');
        }

        protected void EndCommand() {
            Stream?.Flush();
        }

        protected byte GetResponse() {
            byte[] buffer = new byte[1];

            Stream?.Read(buffer, 0, 1);

            Trace.WriteLineIf(traceIntelliboxRaw.TraceVerbose, "  Response " + Convert.ToString(buffer[0], 16));
            return buffer[0];
        }

        /// <summary>
        /// Send a command in a buffer
        /// </summary>
        /// <param name="command">The command bytes</param>
        /// <returns>The reply's 1st byte</returns>
        protected byte Command(byte[] command) {
            if (traceIntelliboxRaw.TraceVerbose) {
                Trace.Write("Send command [" + ToString() + "] - x + ");
                foreach (byte b in command)
                    Trace.Write(Convert.ToString(b, 16) + " ");
                Trace.WriteLine("");
            }

            BeginCommand();
            foreach (byte b in command)
                Stream?.WriteByte(b);
            EndCommand();

            // Read reply
            byte response = GetResponse();

            return response;
        }

        /// <summary>
        /// Send a single byte command
        /// </summary>
        /// <param name="b">The command byte</param>
        /// <returns>The reply (1st byte) recieved for the command</returns>
        protected byte Command(byte b) => Command(new byte[] { b });

        protected void ReportError(byte errorCode) {
            string? message = null;

            message = errorCode switch {
                0x02 => "Bad parameter value",
                0x06 => "Command station power is off",
                0x08 => "Locomotive command buffer out of space",
                0x09 => "Turnout FIFO is full",
                0x0b => "No locomotive slot is available",
                0x0c => "Invalid locomotive address",
                0x0d => "Locomotive already controlled by another device",
                0x0e => "Illegal turnout address",
                0x10 => "I2C FIFO is full",

#if WARN_FIFI_75_FULL
				case 0x40:
					message = "Turnout FIFO is 75% full";
					break;
#endif

                0x41 => "Intellibox is in 'Halt' mode",
                0x42 => "Intellibox is powered off",
                _ => "Unknown error 0x" + errorCode.ToString("x"),
            };

            if (message != null)
                CommandStation.Error(this.CommandStation, "Command: [" + ToString() + "] - " + message);
        }
    }

    internal struct ExternalLocomotiveEventInfo {
        public int Unit;
        public byte LogicalSpeed;
        public byte FunctionMask;
        public LocomotiveOrientation Direction;
        public bool Lights;

        public ExternalLocomotiveEventInfo(int unit, byte logicalSpeed, byte functionMask, LocomotiveOrientation direction, bool lights) {
            Unit = unit;
            LogicalSpeed = logicalSpeed;
            FunctionMask = functionMask;
            Direction = direction;
            Lights = lights;
        }
    }

    internal class IntelliboxSetSOcommand : IntelliboxCommand {
        private readonly int soNumber;
        private readonly int soValue;

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

    internal class IntelliboxPowerOnCommand : IntelliboxCommand {
        public IntelliboxPowerOnCommand(IntelliboxComponent commandStation) : base(commandStation) {
        }

        public override void Do() {
            Command(0xa7);
        }

        public override string ToString() => "Power On";
    }

    internal class IntelliboxPowerOffCommand : IntelliboxCommand {
        public IntelliboxPowerOffCommand(IntelliboxComponent commandStation) : base(commandStation) {
        }

        public override void Do() {
            Command(0xa6);
        }

        public override string ToString() => "Power off";
    }

    internal class IntelliboxTrackPowerOnCommand : IntelliboxCommand {
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
    internal class IntelliboxForceSensorEventCommand : IntelliboxCommand {
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
    internal class InterlliboxProcessEventsCommand : IntelliboxCommand, IOutputIdlecommand {
        private ControlBus? _motorolaBus = null;
        private ControlBus? _S88bus = null;
        private byte[]? previousReply = null;
        private readonly List<FeedbackData> feedbackData = new();

        public InterlliboxProcessEventsCommand(IntelliboxComponent commandStation, int pollingPeriod) : base(commandStation) {
            DefaultWaitPeriod = pollingPeriod;
        }

        protected ControlBus MotorolaBus => _motorolaBus ??= Ensure.NotNull<ControlBus>(LayoutModel.ControlManager.Buses.GetBus(CommandStation, "Motorola"));

        protected ControlBus S88Bus => _S88bus ??= Ensure.NotNull<ControlBus>(LayoutModel.ControlManager.Buses.GetBus(CommandStation, "S88BUS"));

        private static void IntelliboxNotifiyLocomotiveState(IntelliboxComponent component, ExternalLocomotiveEventInfo info) {
            var addressMap = Dispatch.Call.GetOnTrackLocomotiveAddressMap(component);
            var addressMapEntry = addressMap?[info.Unit];

            if (addressMapEntry == null)
                LayoutModuleBase.Warning("Locomotive status reported for a unrecognized locomotive (unit " + info.Unit + ")");
            else {
                TrainStateInfo train = addressMapEntry.Train;

                int speedInSteps = 0;

                if (info.LogicalSpeed > 1) {
                    double factor = (double)train.SpeedSteps / 126;

                    speedInSteps = (int)Math.Round((info.LogicalSpeed - 2) * factor);

                    if (speedInSteps == 0)
                        speedInSteps = 1;
                    else if (speedInSteps > train.SpeedSteps)
                        speedInSteps = train.SpeedSteps;

                    if (info.Direction == LocomotiveOrientation.Backward)
                        speedInSteps = -speedInSteps;
                }

                Dispatch.Notification.OnLocomotiveMotion(component, speedInSteps, info.Unit);

                if (info.Lights != train.Lights)
                    Dispatch.Notification.OnLocomotiveLightsChanged(component, info.Unit, info.Lights);
            }
        }


        public override void Do() {
            List<Action> events = new();

            byte[] reply = new byte[3];

            reply[0] = reply[1] = reply[2] = 0;

            reply[0] = Command(0xc8);

            if ((reply[0] & 0x80) != 0) {
                reply[1] = GetResponse();

                if ((reply[1] & 0x80) != 0)
                    reply[2] = GetResponse();
            }

            if ((reply[0] & 0x20) != 0) {           // at least one turnout was changed manually
                byte count = Command(0xca);

                for (int i = 0; i < count; i++) {
                    byte addressLow = GetResponse();
                    byte addressHighAndMisc = GetResponse();
                    int address = addressLow | ((addressHighAndMisc & 0x7) << 8);
                    int state = ((addressHighAndMisc & 0x80) != 0) ? 1 : 0;

                    if (LayoutController.IsOperationMode)
                        Dispatch.Notification.OnControlConnectionPointStateChanged(new ControlConnectionPointReference(MotorolaBus, address), state);
                    else if (LayoutController.IsDesignTimeActivation)
                        events.Add(() => Dispatch.Notification.OnDesignTimeCommandStationEvent(new CommandStationInputEvent(CommandStation, MotorolaBus, address, state)));
                }
            }

            if ((reply[0] & 0x04) != 0) {           // At least one sensor changed its state
                byte moduleNo = Command(0xcb);

                while (moduleNo > 0) {
                    UInt16 mask = GetResponse();
                    mask = (UInt16)((mask << 8) | GetResponse());

                    while (moduleNo > feedbackData.Count)
                        feedbackData.Add(new FeedbackData());

                    feedbackData[moduleNo - 1].NewData(CommandStation, moduleNo - 1, mask);
                    moduleNo = GetResponse();
                }
            }

            if ((reply[0] & 0x01) != 0) {           // At least one locomotive state changed not by this program
                byte logicalSpeed = Command(0xc9);

                while (logicalSpeed < 0x80) {
                    byte functionMask = GetResponse();
                    byte lowAddress = GetResponse();
                    byte highAddressAndDir = GetResponse();
                    byte _ = GetResponse();     // realSpeed

                    int unit = lowAddress | ((highAddressAndDir & 0x3f) << 8);
                    LocomotiveOrientation direction = ((highAddressAndDir & 0x80) != 0) ? LocomotiveOrientation.Forward : LocomotiveOrientation.Backward;
                    bool lights = (highAddressAndDir & 0x40) != 0;

                    if (CommandStation.OperationMode)
                        events.Add(() => IntelliboxNotifiyLocomotiveState(CommandStation, new ExternalLocomotiveEventInfo(unit, logicalSpeed, functionMask, direction, lights)));

                    logicalSpeed = GetResponse();
                }
            }

            if ((reply[1] & 0x20) != 0) {
                if (previousReply == null || (previousReply[1] & 0x20) == 0)
                    CommandStation.Warning(CommandStation, "Command station reports on overheating condition!");
            }
            else if (previousReply != null && (previousReply[1] & 0x20) != 0)
                CommandStation.Warning(CommandStation, "Command station does no longer report on overheating condition");

            if ((reply[1] & 0x10) != 0) {
                if (previousReply == null || (previousReply[1] & 0x10) == 0)
                    CommandStation.Warning(CommandStation, "Command station reports on forbidden electrical connection between tracks and programming track");
            }
            else if (previousReply != null && (previousReply[1] & 0x10) != 0)
                CommandStation.Warning(CommandStation, "Command station does no longer report on forbidden connection between tracks and programming track");

            if ((reply[1] & 0x08) != 0) {
                if (previousReply == null || (previousReply[1] & 0x08) == 0)
                    CommandStation.Warning(CommandStation, "Command station reports on current overload condition for DCC booster");
            }
            else if (previousReply != null && (previousReply[1] & 0x08) != 0)
                CommandStation.Warning(CommandStation, "Command station does no longer report on current overload condition for DCC booster");

            if ((reply[1] & 0x04) != 0) {
                if (previousReply == null || (previousReply[1] & 0x04) == 0)
                    CommandStation.Warning(CommandStation, "Command station reports on current overload condition for internal booster");
            }
            else if (previousReply != null && (previousReply[1] & 0x04) != 0)
                CommandStation.Warning(CommandStation, "Command station does no longer report on current overload condition for internal booster");

            if ((reply[1] & 0x02) != 0) {
                if (previousReply == null || (previousReply[1] & 0x02) == 0)
                    CommandStation.Warning(CommandStation, "Command station reports on current overload condition for loco-mouse bus");
            }
            else if (previousReply != null && (previousReply[1] & 0x02) != 0)
                CommandStation.Warning(CommandStation, "Command station does no longer report on current overload condition for loco-mouse bus");

            if ((reply[1] & 0x01) != 0) {
                if (previousReply == null || (previousReply[1] & 0x01) == 0)
                    CommandStation.Warning(CommandStation, "Command station reports on current overload condition for external booster");
            }
            else if (previousReply != null && (previousReply[1] & 0x01) != 0)
                CommandStation.Warning(CommandStation, "Command station does no longer report on current overload condition for external booster");

            if (previousReply == null)
                previousReply = new byte[3];

            reply.CopyTo(previousReply, 0);

            for (int i = 0; i < feedbackData.Count; i++)
                feedbackData[i].Tick(CommandStation, i, events);

            CommandStation.InterThreadEventInvoker.Queue(() => events.ForEach(action => action()));
        }

        public override string ToString() => "Process Events";

        private class FeedbackData {
            private UInt16 currentData = 0;
            private readonly byte[] debounceCounters = new byte[16];

            /// <summary>
            /// Called when new data is read for this feedback decoder. It will set the debouncing counter for each
            /// changed bit.
            /// </summary>
            /// <param name="data">Data read from the decoder</param>
            public void NewData(IntelliboxComponent commandStation, int moduleNumber, UInt16 data) {
                UInt16 changedBits = (UInt16)(currentData ^ data);

                for (int i = 0; i < 16; i++) {
                    if ((changedBits & 1) != 0) {
                        if (debounceCounters[i] > 0)
                            commandStation.InterThreadEventInvoker.Queue(() => Dispatch.Notification.OnControlConnectionPointStateUnstable(new ControlConnectionPointReference(commandStation.S88Bus, moduleNumber + 1, 15 - i)));
                        debounceCounters[i] = commandStation.OperationMode ? commandStation.CachedOperationModeDebounceCount : commandStation.CachedDesignTimeDebounceCount;            // Start counting
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
            public void Tick(IntelliboxComponent commandStation, int moduleNumber, List<Action> events) {
                for (int i = 0; i < 16; i++) {
                    if (debounceCounters[i] > 0) {
                        if (--debounceCounters[i] == 0) {
                            bool isSet = (currentData & (1 << i)) != 0;

                            if (LayoutController.IsOperationMode)
                                Dispatch.Notification.OnControlConnectionPointStateChanged(new ControlConnectionPointReference(commandStation.S88Bus, moduleNumber + 1, 15 - i), isSet ? 1 : 0);
                            else if (LayoutController.IsDesignTimeActivation)
                                events.Add(() => Dispatch.Notification.OnDesignTimeCommandStationEvent(new CommandStationInputEvent(commandStation, commandStation.S88Bus, moduleNumber + 1, 15 - i, isSet ? 1 : 0)));
                        }
                    }
                }
            }
        }

        #region ICommandStationIdlecommand Members

        public bool RemoveFromQueue => false;

        #endregion
    }

    internal abstract class IntelliboxAccessoryCommandBase : IntelliboxCommand {
        protected List<KeyValuePair<int, int>> listOfunitAndState = new();

        protected IntelliboxAccessoryCommandBase(IntelliboxComponent commandStation, int unit, int state) : base(commandStation) {
            listOfunitAndState.Add(new KeyValuePair<int, int>(unit, 1 - state));
        }

        protected IntelliboxAccessoryCommandBase(IntelliboxComponent commandStation, List<SwitchingCommand> switchingCommands, int startAt, int count) : base(commandStation) {
            for (int i = 0; i < count; i++) {
                SwitchingCommand switchingCommand = switchingCommands[startAt + i];
                int unit = switchingCommand.ControlPointReference.Module.Address + switchingCommand.ControlPointReference.Index;

                listOfunitAndState.Add(new KeyValuePair<int, int>(unit, 1 - switchingCommand.SwitchState));
            }
        }
    }

    internal class IntelliboxAccessoryCommand : IntelliboxAccessoryCommandBase {
        public IntelliboxAccessoryCommand(IntelliboxComponent commandStation, int unit, int state)
            : base(commandStation, unit, state) {
        }

        public IntelliboxAccessoryCommand(IntelliboxComponent commandStation, List<SwitchingCommand> switchingCommands, int startAt, int count)
            : base(commandStation, switchingCommands, startAt, count) {
        }

        public override void Do() {
            foreach (KeyValuePair<int, int> unitAndState in listOfunitAndState) {
                int unit = unitAndState.Key;
                int state = unitAndState.Value;
                byte[] command = new byte[3];

                command[0] = 0x90;
                command[1] = (byte)(unit & 0xff);
                command[2] = (byte)((state << 7) | 0x40 | ((unit >> 8) & 0xff));

                byte reply = Command(command);

                if (reply != 0)
                    ReportError(reply);
            }
        }

        // Wait 100 milliseconds before sending next command from this queue
        public override int WaitPeriod => CommandStation.Info.AccessoryCommandTime;

        public override string ToString() {
            string s;

            if (listOfunitAndState.Count > 1)
                s = "Begin set turnouts: ";
            else
                s = "Begin set turnout: ";

            foreach (KeyValuePair<int, int> unitAndState in listOfunitAndState)
                s += " #" + unitAndState.Key + " to state: " + unitAndState.Value + "; ";
            return s;
        }
    }

    internal class IntelliboxEndAccessoryCommand : IntelliboxAccessoryCommandBase {
        public IntelliboxEndAccessoryCommand(IntelliboxComponent commandStation, int unit, int state)
            : base(commandStation, unit, state) {
        }

        public IntelliboxEndAccessoryCommand(IntelliboxComponent commandStation, List<SwitchingCommand> switchingCommands, int startAt, int count)
            : base(commandStation, switchingCommands, startAt, count) {
        }

        public override void Do() {
            foreach (KeyValuePair<int, int> unitAndState in listOfunitAndState) {
                int unit = unitAndState.Key;
                int state = unitAndState.Value;
                byte[] command = new byte[3];

                command[0] = 0x90;
                command[1] = (byte)(unit & 0xff);
                command[2] = (byte)((state << 8) | ((unit >> 8) & 0xff));

                byte reply = Command(command);

                if (reply != 0)
                    ReportError(reply);
            }
        }

        public override string ToString() {
            string s;

            if (listOfunitAndState.Count > 1)
                s = "End set turnouts: ";
            else
                s = "End set turnout: ";

            foreach (KeyValuePair<int, int> unitAndState in listOfunitAndState)
                s += " #" + unitAndState.Key + " to state: " + unitAndState.Value + "; ";
            return s;
        }
    }

    internal class IntelliboxLocomotiveCommand : IntelliboxCommand {
        private readonly int unit;
        private readonly byte speed;
        private readonly byte direction = 0;
        private readonly byte lights = 0;

        public IntelliboxLocomotiveCommand(IntelliboxComponent commandStation, int unit, int logicalSpeed, LocomotiveOrientation direction, bool lights) : base(commandStation) {
            this.unit = unit;
            speed = (byte)logicalSpeed;

            if (direction == LocomotiveOrientation.Forward)
                this.direction = 0x20;
            if (lights)
                this.lights = 0x10;
        }

        public override void Do() {
            byte[] command = new byte[5];

            command[0] = 0x80;
            command[1] = (byte)(unit & 0xff);
            command[2] = (byte)((unit >> 8) & 0xff);
            command[3] = speed;
            command[4] = (byte)(direction | lights);

            byte reply = Command(command);

            if (reply != 0)
                ReportError(reply);
        }

        public override string ToString() => "Locomotive: " + unit + " speed: " + speed + " direction: " + ((direction & 0x20) != 0 ? "forward" : "backward") + " lights: " + (lights != 0 ? "On" : "Off");
    }

    internal class IntelliboxLocomotiveFunctionsCommand : IntelliboxCommand {
        private readonly int unit;
        private readonly byte functionMask;

        public IntelliboxLocomotiveFunctionsCommand(IntelliboxComponent commandStation, int unit, byte functionMask) : base(commandStation) {
            this.unit = unit;
            this.functionMask = functionMask;
        }

        public override void Do() {
            byte[] command = new byte[4];

            command[0] = 0x88;
            command[1] = (byte)(unit & 0xff);
            command[2] = (byte)((unit >> 8) & 0xff);
            command[3] = functionMask;

            byte reply = Command(command);

            if (reply != 0)
                ReportError(reply);
        }

        public override string ToString() => "Locomotive: " + unit + " set functions: " + functionMask;
    }

    #endregion
}
