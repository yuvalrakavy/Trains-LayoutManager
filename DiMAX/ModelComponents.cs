using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Threading;
using System.Threading.Tasks;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

#pragma warning disable IDE0050, IE0060
#nullable enable
namespace DiMAX {

    /// <summary>
    /// Summary description for MTScomponents.
    /// </summary>
    public class DiMAXcommandStation : LayoutCommandStationComponent, IModelComponentCanProgramLocomotives {
        public static LayoutTraceSwitch TraceDiMAX = new LayoutTraceSwitch("DiMAX", "Massoth DiMAX Central Station");
        public static LayoutTraceSwitch TraceRawData = new LayoutTraceSwitch("DiMAXrawData", "Trace raw DiMAX central station input/output");

        // Mfg ID for this software (TODO: Ask Massoth for one)
        public const UInt16 MID_LayoutManager = 0x39f4;

        OutputManager? outputManager;
        readonly byte[] lengthAndCommand = new byte[1];
        readonly byte[] directAnswerLength = new byte[2];
        byte[]? inputBuffer;
        readonly AutoResetEvent readAbortedEvent = new AutoResetEvent(false);

        ControlBus? _DiMAXbus = null;
        ControlBus? _DCCbus = null;
        ControlBus? _locoBus = null;
        readonly DiMAXstatus dimaxStatus = new DiMAXstatus();
        readonly Dictionary<int, ActiveLocomotiveInfo> registeredLocomotives = new Dictionary<int, ActiveLocomotiveInfo>();

        enum LocomotiveSelection {
            Selected, DeselectedActive, DeselectedPassive
        }

        class ActiveLocomotiveInfo {
            public LocomotiveInfo Locomotive { get; }
            public int SelectNesting { get; set; }
            public bool ActiveDeselection { get; set; }

            public bool SerialFunctionState { get; set; }

            public ActiveLocomotiveInfo(LocomotiveInfo locomotive) {
                this.Locomotive = locomotive;
                SelectNesting = 0;
                ActiveDeselection = false;
                SerialFunctionState = false;
            }
        }

        public DiMAXcommandStation() {
            this.XmlDocument.LoadXml(
                "<CommandStation>" +
                "<ModeString>baud=57600 parity=N data=8 octs=on odsr=on</ModeString>" +
                "</CommandStation>"
                );
        }

        public ControlBus DiMAXbus {
            get {
                if (_DiMAXbus == null) {
                    _DiMAXbus = LayoutModel.ControlManager.Buses.GetBus(this, "DiMAXBUS");
                    Debug.Assert(_DiMAXbus != null);
                }

                return _DiMAXbus;
            }
        }

        public ControlBus DCCbus {
            get {
                if (_DCCbus == null) {
                    _DCCbus = LayoutModel.ControlManager.Buses.GetBus(this, "DiMAXDCC");
                    Debug.Assert(_DCCbus != null);
                }
                return _DCCbus;
            }
        }

        public ControlBus LocoBus {
            get {
                if (_locoBus == null) {
                    _locoBus = LayoutModel.ControlManager.Buses.GetBus(this, "DiMAXLocoBus");
                    Debug.Assert(_locoBus != null);
                }
                return _locoBus;
            }
        }

        OutputManager OutputManager {
            get {
                return Ensure.NotNull<OutputManager>(this.outputManager, "outputManager");
            }
        }

        #region Specific implementation to base class overrideble methods and properties

        public override IList<string> BusTypeNames => Array.AsReadOnly<string>(new string[] { "DiMAXDCC", "DiMAXBUS", "DiMAXLocoBus" });

        public bool POMprogrammingSupported => true;

        public override DigitalPowerFormats SupportedDigitalPowerFormats => DigitalPowerFormats.NRMA;

        #endregion

        protected override void OnCommunicationSetup() {
            base.OnCommunicationSetup();

            if (!Element.HasAttribute("OverlappedIO"))
                Element.SetAttribute("OverlappedIO", XmlConvert.ToString(true));
            if (!Element.HasAttribute("ReadIntervalTimeout"))
                Element.SetAttribute("ReadIntervalTimeout", XmlConvert.ToString(3500));
            if (!Element.HasAttribute("ReadTotalTimeoutConstant"))
                Element.SetAttribute("ReadTotalTimeoutConstant", XmlConvert.ToString(6000));
            if (!Element.HasAttribute("WriteTotalTimeoutConstant"))
                Element.SetAttribute("WriteTotalTimeoutConstant", XmlConvert.ToString(6000));
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            _DiMAXbus = null;
            _DCCbus = null;
            _locoBus = null;

            outputManager = new OutputManager(Name, 1);
            OutputManager.Start();

            // Until every one is really happy...
            TraceDiMAX.Level = TraceLevel.Off;
            TraceRawData.Level = TraceLevel.Off;

            // And finally begin an asynchronous read
            CommunicationStream?.BeginRead(lengthAndCommand, 0, lengthAndCommand.Length, new AsyncCallback(this.OnReadLengthAndCommandDone), null);

            OutputManager.AddCommand(new DiMAXinterfaceAnnouncement(this, false));

        }

        protected override async Task OnTerminateCommunication() {
            if (outputManager != null) {
                await OutputManager.WaitForIdle();
                OutputManager.Terminate();
                outputManager = null;
            }
        }

        public override bool LayoutEmulationSupported => true;

        public override bool DesignTimeLayoutActivationSupported => true;

        protected override ILayoutCommandStationEmulator CreateCommandStationEmulator(string pipeName) => new DiMAXcommandStationEmulator(this, pipeName, EmulationTickTime);

        protected override void OnEnteredOperationMode() {
            base.OnEnteredOperationMode();

            foreach (var train in LayoutModel.StateManager.Trains) {
                if (train.CommandStation!= null && train.CommandStation.Id == Id && train.IsPowered) {
                    foreach (var trainLoco in train.Locomotives)
                        if (trainLoco.Locomotive != null)
                            RegisterLocomotive(trainLoco.Locomotive);
                }
            }
        }

        #region Request Event Handlers
        #pragma warning disable IDE0051, IDE0060

        [LayoutEvent("get-command-station-capabilities", IfEvent = "*[CommandStation/@Name='`string(Name)`']")]
        void GetCommandStationCapabilities(LayoutEvent e) {
            CommandStationCapabilitiesInfo cap = new CommandStationCapabilitiesInfo();
            int minTimeBetweenSpeedSteps = 100;

            if (Element.HasAttribute("MinTimeBetweenSpeedSteps"))
                minTimeBetweenSpeedSteps = XmlConvert.ToInt32(Element.GetAttribute("MinTimeBetweenSpeedSteps"));

            cap.MinTimeBetweenSpeedSteps = minTimeBetweenSpeedSteps;

            e.Info = cap.Element;
        }

        [LayoutEvent("disconnect-power-request")]
        void PowerDisconnectRequest(LayoutEvent e) {
            if (e.Sender == null || e.Sender == this)
                OutputManager.AddCommand(new DiMAXpowerDisconnect(this));

            PowerOff();
        }

        [LayoutEvent("connect-power-request")]
        void PowerConnectRequest(LayoutEvent e) {
            if (e.Sender == null || e.Sender == this)
                OutputManager.AddCommand(new DiMAXpowerConnect(this));

            PowerOn();
        }

        [LayoutEvent("get-command-station-set-function-number-support", IfEvent = "*[CommandStation/@ID='`string(@ID)`']")]
        private void getSetFunctionNumberSupport(LayoutEvent e) {
            e.Info = new CommandStationSetFunctionNumberSupportInfo() {
                SetFunctionNumberSupport = SetFunctionNumberSupport.FunctionNumberAndBooleanState
            };
        }

        // Implement command events
        [LayoutAsyncEvent("change-track-component-state-command", IfEvent = "*[CommandStation/@ID='`string(@ID)`']")]
        Task ChangeTurnoutState(LayoutEvent e) {
            var connectionPointRef = Ensure.NotNull<ControlConnectionPointReference>(e.Sender, "connectionPointRef");
            var module = Ensure.NotNull<ControlModule>(connectionPointRef.Module, "module");
            int state = (int)e.Info;
            int address = module.Address + connectionPointRef.Index;

            var task = OutputManager.AddCommand(new DiMAXchangeAccessoryState(this, address, state));
            DCCbusNotification(address, state);

            return task;
        }

        [LayoutEvent("change-signal-state-command", IfEvent = "*[CommandStation/@ID='`string(@ID)`']")]
        private void changeSignalStateCommand(LayoutEvent e) {
            var connectionPoint = Ensure.NotNull<ControlConnectionPoint>(e.Sender, "connectionPoint");
            var state = (LayoutSignalState)e.Info;
            int address = connectionPoint.Module.Address + connectionPoint.Index;
            int v;

            if (state == LayoutSignalState.Green)
                v = 1;
            else
                v = 0;

            OutputManager.AddCommand(new DiMAXchangeAccessoryState(this, address, state == LayoutSignalState.Green ? 1 : 0));
            DCCbusNotification(address, v);
        }

        [LayoutEvent("locomotive-motion-command", IfEvent = "*[CommandStation/@Name='`string(Name)`']")]
        private void locomotiveMotionCommand(LayoutEvent e) {
            var loco = Ensure.NotNull<LocomotiveInfo>(e.Sender, "loco");
            int speedInSteps = (int)e.Info;

            OutputManager.AddCommand(new DiMAXlocomotiveMotion(this, loco.AddressProvider.Unit, speedInSteps));
        }

        [LayoutEvent("set-locomotive-lights-command", IfEvent = "*[CommandStation/@Name='`string(Name)`']")]
        private void setLocomotiveLightsCommand(LayoutEvent e) {
            var loco = Ensure.NotNull<LocomotiveInfo>(e.Sender, "loco");
            var lights = (bool)e.Info;

            OutputManager.AddCommand(new DiMAXlocomotiveFunction(this, loco.AddressProvider.Unit, 0, false, lights));
            if (loco.DecoderType is DccDecoderTypeInfo decoder && !decoder.ParallelFunctionSupport)
                OutputManager.AddCommand(new DiMAXlocomotiveFunction(this, loco.AddressProvider.Unit, 9, false, false));
        }

        [LayoutEvent("set-locomotive-function-state-command", IfEvent = "*[CommandStation/@Name='`string(Name)`']")]
        [LayoutEvent("trigger-locomotive-function-command", IfEvent = "*[CommandStation/@Name='`string(Name)`']")]
        private void setLocomotiveFunctionStateCommand(LayoutEvent e) {
            var loco = Ensure.NotNull<LocomotiveInfo>(e.Sender, "loco");
            var functionName = Ensure.NotNull<String>(e.Info, "functionName");
            var function = loco.GetFunctionByName(functionName);
            var train = LayoutModel.StateManager.Trains[loco.Id];

            if (train != null && function != null) {
                if (loco.DecoderType is DccDecoderTypeInfo decoder) {
                    if (decoder.ParallelFunctionSupport)
                        OutputManager.AddCommand(new DiMAXlocomotiveFunction(this, loco.AddressProvider.Unit, function.Number, e.GetBoolOption("FunctionState"), train.Lights));
                    else
                        generateSerialFunctionCommands(train, loco, function.Number);
                }
            }
        }

        [LayoutEvent("trigger-locomotive-function-number", IfEvent = "*[CommandStation/@Name='`string(Name)`']")]
        private void triggerLocomotiveFunctionNumber(LayoutEvent e) {
            if (e.Sender is LocomotiveInfo loco && e.Info is int functionNumber) {
                var train = LayoutModel.StateManager.Trains[loco.Id];

                if (train != null) {
                    if (loco.DecoderType is DccDecoderTypeInfo decoder) {
                        if (decoder.ParallelFunctionSupport)
                            OutputManager.AddCommand(new DiMAXlocomotiveFunction(this, loco.AddressProvider.Unit, functionNumber, e.GetBoolOption("FunctionState"), train.Lights));
                        else
                            generateSerialFunctionCommands(train, loco, functionNumber);
                    }
                }
            }
        }

        private void generateSerialFunctionCommands(TrainStateInfo train, LocomotiveInfo loco, int functionNumber) {
            var state = registeredLocomotives[loco.AddressProvider.Unit].SerialFunctionState;

            for (var i = 0; i < functionNumber; i++) {
                var command = new DiMAXlocomotiveFunction(this, loco.AddressProvider.Unit, 1, state, train.Lights) {
                    DefaultWaitPeriod = 200
                };

                state = !state;
                OutputManager.AddCommand(command);
            }

            registeredLocomotives[loco.AddressProvider.Unit].SerialFunctionState = state;
        }

        [LayoutEvent("add-command-station-loco-bus-to-address-map", IfSender = "*[@ID='`string(@ID)`']")]
        private void AddCommandStationLocoBusToAddressMap(LayoutEvent e) {
            var addressMap = Ensure.NotNull<LocomotiveAddressMap>(e.Info, "addressMap");

            foreach (ControlModule module in LocoBus.Modules) {
                for (int i = 0; i < module.ModuleType.NumberOfAddresses; i++) {
                    int address = module.Address + i;

                    if (!addressMap.ContainsKey(address))
                        addressMap.Add(address, new LocomotiveBusModuleAddressMapEntry(module));
                }
            }
        }

        [LayoutAsyncEvent("enable-DiMAX-status-update", IfEvent = "*[CommandStation/@Name='`string(Name)`']")]
        private Task enableDiMAXstatusUpdate(LayoutEvent e) => OutputManager.AddCommand(new DiMAXinterfaceAnnouncement(this, true));

        [LayoutAsyncEvent("disable-DiMAX-status-update", IfEvent = "*[CommandStation/@Name='`string(Name)`']")]
        private Task disableDiMAXstatusUpdate(LayoutEvent e) => OutputManager.AddCommand(new DiMAXinterfaceAnnouncement(this, false));

        [LayoutAsyncEvent("program-CV-direct-request", IfEvent = "*[CommandStation/@Name='`string(Name)`']")]
        private async Task<Object> programCVdirectRequest(LayoutEvent e) {
            var cv = Ensure.NotNull<DccProgrammingCV>(e.Sender, "register");

            return await (Task<object>)OutputManager.AddCommand(new DiMAXprogramCV(this, cv.Number, cv.Value));
        }

        private async Task<LayoutActionResult> SetRegister(int register, byte value) => (LayoutActionResult)await (Task<object>)OutputManager.AddCommand(new DiMAXprogramRegister(this, (byte)register, value));

        [LayoutAsyncEvent("program-CV-register-requst", IfEvent = "*[CommandStation/@Name='`string(Name)`']")]
        private async Task<Object> programCVregisterRequst(LayoutEvent e) {
            var cv = Ensure.NotNull<DccProgrammingCV>(e.Sender, "cv");

            if (cv.Number < 1)
                throw new ArgumentException("Invalid CV number: " + cv.Number);

            if (cv.Number <= 8)
                return await SetRegister(cv.Number, cv.Value);
            else {
                LayoutActionResult r = await SetRegister(6, (byte)cv.Number);

                if (r == LayoutActionResult.Ok)
                    return await SetRegister(5, cv.Value);
                else
                    return r;
            }
        }

        [LayoutAsyncEvent("program-CV-POM-request", IfEvent = "*[CommandStation/@Name='`string(Name)`']")]
        private async Task<Object> programCVPOMrequest(LayoutEvent e) {
            var cv = Ensure.NotNull<DccProgrammingCV>(e.Sender, "cv");

            return await (Task<object>)OutputManager.AddCommand(new DiMAXprogramCVonTrack(this, e.GetIntOption("Address"), cv.Number, cv.Value));
        }

        [LayoutEvent("locomotive-power-changed")]
        private void locomotivePowerChanged(LayoutEvent e) {
            var trainLocomotive = Ensure.NotNull<TrainLocomotiveInfo>(e.Sender, "trainLocomotive");
            var power = Ensure.NotNull<ILayoutPower>(e.Info, "power");
            var locomotive = trainLocomotive.Locomotive;
            int address = locomotive.AddressProvider.Unit;

            if (power.Type != LayoutPowerType.Digital) {

                if (registeredLocomotives.TryGetValue(address, out ActiveLocomotiveInfo activeLocomotive)) {
                    Debug.Assert(activeLocomotive.SelectNesting == 0);

                    if (activeLocomotive.ActiveDeselection)
                        OutputManager.AddCommand(new DiMAXlocomotiveSelection(this, address, select: false, deselectActive: false, unconditional: false));

                    OutputManager.AddCommand(new DiMAXlocomotiveUnregister(this, address));
                    registeredLocomotives.Remove(address);
                }
            }
            else
                RegisterLocomotive(locomotive);
        }

        private void RegisterLocomotive(LocomotiveInfo locomotive) {

            if (locomotive.DecoderType is DccDecoderTypeInfo decoder) {
                int navigatorImage = 0;

                if (locomotive.Attributes.ContainsKey("NavigatorImage")) {
                    object? value = locomotive.Attributes["NavigatorImage"];

                    if (value is int)
                        navigatorImage = (int)value;
                    else if (value is string)
                        int.TryParse((string)value, out navigatorImage);
                }

                OutputManager.AddCommand(new DiMAXlocomotiveRegistration(this, locomotive.AddressProvider.Unit,
                    parallelFunctions: decoder.ParallelFunctionSupport, speedSteps: locomotive.SpeedSteps, image: navigatorImage));

                if (!registeredLocomotives.ContainsKey(locomotive.AddressProvider.Unit))
                    registeredLocomotives.Add(locomotive.AddressProvider.Unit, new ActiveLocomotiveInfo(locomotive));
            }
        }

        private void SelectLocomotive(LocomotiveInfo locomotive) {
            int address = locomotive.AddressProvider.Unit;

            if (registeredLocomotives.TryGetValue(address, out ActiveLocomotiveInfo activeLocomotive)) {
                if (activeLocomotive.SelectNesting++ == 0)
                    OutputManager.AddCommand(new DiMAXlocomotiveSelection(this, address, select: true, deselectActive: false, unconditional: false));
            }
            else
                Error("Controller activated for locomotive " + locomotive.Name + " (" + address + ") which is not registered");
        }

        /// <summary>
        /// Check if a locomotive is completly inactive. 
        /// </summary>
        bool IsLocomotiveActive(TrainStateInfo train, LocomotiveInfo locomotive) => train.Speed != 0 || train.Lights || train.HasActiveFunction(locomotive);

        private void DeselectLocomotive(TrainStateInfo train, LocomotiveInfo locomotive) {
            int address = locomotive.AddressProvider.Unit;

            if (registeredLocomotives.TryGetValue(address, out ActiveLocomotiveInfo activeLocomotive)) {
                if (activeLocomotive.SelectNesting == 0 || --activeLocomotive.SelectNesting == 0) {
                    activeLocomotive.ActiveDeselection = IsLocomotiveActive(train, locomotive);

                    OutputManager.AddCommand(new DiMAXlocomotiveSelection(this, address, select: false, deselectActive: activeLocomotive.ActiveDeselection, unconditional: false));
                }
            }
        }

        [LayoutEvent("locomotive-controller-activated")]
        private void locomotiveControllerActivated(LayoutEvent e) {
            var trainLocomotive = Ensure.NotNull<TrainLocomotiveInfo>(e.Sender, "trainLocomotive");
            var locomotive = trainLocomotive.Locomotive;

            SelectLocomotive(locomotive);
        }

        [LayoutEvent("locomotive-controller-deactivated")]
        private void locomotiveControllerDeactivated(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Info, "train");
            var locomotive = Ensure.NotNull<TrainLocomotiveInfo>(e.Sender, "locomotive").Locomotive;

            DeselectLocomotive(train, locomotive);
        }


        [LayoutEvent("trip-added")]
        private void tripAdded(LayoutEvent e) {
            var trip = Ensure.NotNull<TripPlanAssignmentInfo>(e.Sender, "trip");

            if (trip.Train.Driver.ComputerDriven) {
                // The train is driven by computer (either automatic, or by train controller)
                foreach (var trainLoco in trip.Train.Locomotives)
                    SelectLocomotive(trainLoco.Locomotive);
            }
        }

        [LayoutEvent("trip-aborted")]
        [LayoutEvent("trip-done")]
        private void tripDone(LayoutEvent e) {
            var trip = Ensure.NotNull<TripPlanAssignmentInfo>(e.Sender, "trip");

            if (trip.Train.Driver.ComputerDriven) {
                // The train is driven by computer (either automatic, or by train controller)
                foreach (var trainLoco in trip.Train.Locomotives)
                    DeselectLocomotive(trip.Train, trainLoco.Locomotive);
            }
        }

        [LayoutAsyncEvent("test-loco-select", IfEvent = "*[CommandStation/@Name='`string(Name)`']")]
        private Task testLocoSelect(LayoutEvent e0) {
            var e = e0;

            int address = e.GetIntOption("Address");
            bool select = e.GetBoolOption("Select");
            bool active = e.GetBoolOption("Active");
            bool unconditional = e.GetBoolOption("Unconditional");

            return OutputManager.AddCommand(new DiMAXlocomotiveSelection(this, address, select, active, unconditional));
        }

        [LayoutAsyncEvent("test-loco-speed", IfEvent = "*[CommandStation/@Name='`string(Name)`']")]
        private Task testLocoSpeed(LayoutEvent e0) {
            var e = e0;

            int address = e.GetIntOption("Address");
            int speed = e.GetIntOption("Speed");

            return OutputManager.AddCommand(new DiMAXlocomotiveMotion(this, address, speed));
        }

        #endregion

        #region Generate notification events

        private void DiMAXbusNotification(int address, int state) {
            if (OperationMode) {
                var connectionPointRef = new ControlConnectionPointReference(DiMAXbus, address, state);

                EventManager.Event(new LayoutEvent("control-connection-point-state-changed-notification", connectionPointRef, 1));
            }
        }

        private void DCCbusNotification(int address, int state) {
            if (OperationMode)
                EventManager.Event(new LayoutEvent("control-connection-point-state-changed-notification", new ControlConnectionPointReference(DCCbus, address), state));
        }

        #endregion

        #region DiMAX input handling

        [LayoutEvent("processes-DiMAX-packet")]
        private void processDiMAXpacket(LayoutEvent e) {
            if (e.Sender == this) {
                var packet = Ensure.NotNull<DiMAXpacket>(e.Info, "packet");

                if (TraceDiMAX.TraceVerbose)
                    packet.Dump("Processing DiMAX packet");

                switch (packet.CommandCode) {

                    case DiMAXcommandCode.TurnoutControl: {
                            DiMAXturnoutOrFeedbackControlPacket p = new DiMAXturnoutOrFeedbackControlPacket(packet);

                            if (OperationMode)
                                DCCbusNotification(p.Address, p.State);
                            else if (LayoutController.IsDesignTimeActivation)
                                EventManager.Event(new LayoutEvent("design-time-command-station-event", this, new CommandStationInputEvent(this, DCCbus, p.Address, p.State),
                                    null));
                        }
                        break;

                    case DiMAXcommandCode.FeedbackControl: {
                            DiMAXturnoutOrFeedbackControlPacket p = new DiMAXturnoutOrFeedbackControlPacket(packet);

                            if (OperationMode)
                                DiMAXbusNotification(p.Address, p.State);
                            else if (LayoutController.IsDesignTimeActivation)
                                EventManager.Event(new LayoutEvent("design-time-command-station-event", this, new CommandStationInputEvent(this, DiMAXbus, p.Address, p.State),
                                    null));
                        }
                        break;

                    case DiMAXcommandCode.LocoSpeedControl: {
                            if (OperationMode) {
                                DiMAXlocoSpeedControlPacket p = new DiMAXlocoSpeedControlPacket(packet);

                                EventManager.Event(
                                    new LayoutEvent("locomotive-motion-notification", this, p.SpeedInSteps).SetOption("Address", "Unit", p.Unit));
                            }
                        }
                        break;

                    case DiMAXcommandCode.LocoFunctionControl: {
                            if (OperationMode) {
                                DiMAXlocoFunctionControlPacket p = new DiMAXlocoFunctionControlPacket(packet);

                                if (p.FunctionNumber == 0)
                                    EventManager.Event(new LayoutEvent("set-locomotive-lights-notification", this, p.Lights).SetOption("Address", "Unit", p.Unit));
                                else
                                    EventManager.Event
                                        (new LayoutEvent("set-locomotive-function-state-notification", this, p.FunctionActive).SetOption("Address", "Unit", p.Unit).SetOption("Function", "Number", p.FunctionNumber));
                            }
                        }
                        break;

                    case DiMAXcommandCode.ExtendedSystemStatus:
                        dimaxStatus.ExtendedStatusUpdate(this, packet);
                        break;

                    case DiMAXcommandCode.SystemStatus:
                        dimaxStatus.SystemStatusUpdate(this, packet);
                        break;

                    case DiMAXcommandCode.EmergencyStopBegin:
                        EventManager.Event(new LayoutEvent<IModelComponentIsCommandStation, string>("emergency-stop-request", this, "Initiated by external controller"));
                        break;

                    case DiMAXcommandCode.EmergencyStopFinish:
                        PowerOn();
                        break;

                    case DiMAXcommandCode.LocoSelection:
                        HandleLocomotiveSelection(packet);
                        break;

                    case DiMAXcommandCode.LocoConfig:
                        break;

                    default:
                        Warning(this, "Unknown command code received from DiMAX: " + packet.CommandCode.ToString());
                        break;
                }
            }
        }

        private void HandleLocomotiveSelection(DiMAXpacket packet) {
            if (packet.ParameterCount == 3 && packet.Parameters != null) {
                int address = ((packet.Parameters[0] & 0x3f) << 8) | packet.Parameters[1];
                bool select = (packet.Parameters[2] & 0x10) != 0;

                if (select) {

                    if (registeredLocomotives.TryGetValue(address, out ActiveLocomotiveInfo activeLocomotive)) {
                        var train = LayoutModel.StateManager.Trains[activeLocomotive.Locomotive.Id];

                        if (train != null) {
                            string trainName = train.Name + " (" + address + ") ";

                            if (train.LocomotiveBlock?.LockRequest == null || !train.LocomotiveBlock.LockRequest.IsManualDispatchLock)
                                Warning("Train" + trainName + "was selected by external controller. Train is not on a manual dispatch region");
                            else if (train.Locomotives.Count > 1)
                                Warning("A locomotive belonging to train" + trainName + "was selected by external controller. This train has more than one locomotive");
                        }
                    }
                    else
                        Warning("Locomotive with address: " + address + " was selected by external controller, locomotive with this address is not on the layout");
                }
            }

        }

        /// <summary>
        /// Called when an asynchronous read is done
        /// </summary>
        /// <param name="asyncReadResult"></param>
        void OnReadLengthAndCommandDone(IAsyncResult asyncReadResult) {
            if (CommunicationStream != null) {
                try {
                    int count = CommunicationStream.EndRead(asyncReadResult);

                    if (count == 1) {
                        if ((lengthAndCommand[0] & 0x1f) == 0) {
                            Trace.WriteLineIf(TraceRawData.TraceVerbose, "DiMAX - got status or direct andwer");
                            CommunicationStream.BeginRead(directAnswerLength, 0, 2, OnReadAnswerLengthDone, null);
                        }
                        else {
                            byte length = (byte)(lengthAndCommand[0] >> 5);
                            int restOfPacketLength = 1 + length;

                            if (inputBuffer == null || inputBuffer.Length != restOfPacketLength)
                                inputBuffer = new byte[restOfPacketLength];

                            Trace.WriteLineIf(TraceRawData.TraceInfo, "DiMAX - got packet length and command: " + lengthAndCommand[0].ToString("x2") + " Length: " + length + " command code: " + (lengthAndCommand[0] & 0x1f));
                            CommunicationStream.BeginRead(inputBuffer, 0, restOfPacketLength, OnReadRestOfPacketDone, null);
                        }
                    }
                    else if (count == 0)
                        CommunicationStream.BeginRead(lengthAndCommand, 0, lengthAndCommand.Length, this.OnReadLengthAndCommandDone, null);
                    else
                        throw new DiMAXException("LengthAndCommand not one byte");
                }
                catch (Exception ex) {
                    Trace.WriteLineIf(TraceDiMAX.TraceInfo, "Pending read aborted (" + ex.Message + ")");
                    readAbortedEvent.Set();
                }
            }
        }

        void OnReadAnswerLengthDone(IAsyncResult asyncReadResult) {
            if (CommunicationStream != null) {
                try {
                    int count = CommunicationStream.EndRead(asyncReadResult);

                    if (count == 2) {
                        int restOfPacketLength = directAnswerLength[1];

                        if (inputBuffer == null || inputBuffer.Length != restOfPacketLength + 2)
                            inputBuffer = new byte[2 + restOfPacketLength];

                        inputBuffer[0] = directAnswerLength[0];
                        inputBuffer[1] = directAnswerLength[1];

                        Trace.WriteLineIf(TraceRawData.TraceInfo, "DiMAX - got direct answer packet length: " + directAnswerLength[1].ToString());
                        CommunicationStream.BeginRead(inputBuffer, 2, restOfPacketLength, OnReadRestOfPacketDone, null);
                    }
                    else
                        throw new DiMAXException("directAnswerLength not 2 bytes");

                }
                catch (Exception ex) {
                    Trace.WriteLineIf(TraceDiMAX.TraceInfo, "Pending read aborted (" + ex.Message + ")");
                    readAbortedEvent.Set();
                }
            }
        }

        void OnReadRestOfPacketDone(IAsyncResult asyncReadResult) {
            if (CommunicationStream != null) {
                try {
                    int count = CommunicationStream.EndRead(asyncReadResult);

                    Debug.Assert(inputBuffer != null);

                    if (TraceRawData.TraceInfo) {
                        Trace.Write("Received DiMAX packet - XOR byte: " + inputBuffer[0].ToString("x2") + " Parameters:");
                        for (int i = 1; i < inputBuffer.Length; i++)
                            Trace.Write(" " + inputBuffer[i].ToString("x2"));
                        Trace.WriteLine("");
                    }

                    DiMAXpacket packet = new DiMAXpacket(lengthAndCommand[0], inputBuffer);

                    if (packet.CommandCode == DiMAXcommandCode.DirectReply) {
                        if (OutputManager.IsWaitingForReply)
                            OutputManager.SetReply(packet);
                        else
                            Trace.WriteLine("Ignored direct reply - no command requiring direct reply was sent");
                    }
                    else
                        InterThreadEventInvoker.QueueEvent(new LayoutEvent("processes-DiMAX-packet", this, packet));

                    CommunicationStream.BeginRead(lengthAndCommand, 0, lengthAndCommand.Length, new AsyncCallback(this.OnReadLengthAndCommandDone), null);
                }
                catch (Exception ex) {
                    Trace.WriteLineIf(TraceDiMAX.TraceInfo, "Pending read aborted (" + ex.Message + ")");
                    readAbortedEvent.Set();
                }
            }
        }

        #endregion
    }

    #region DiMAX status

    public class DiMAXstatus {
        public enum DiMAXcondition {
            Unknown,
            Normal,
            EmergencryStop,
            Reset,
        }

        public DateTime LastUpdate { get; private set; }
        public string? CommandStationType { get; private set; }
        public double CurrentLimit { get; private set; }
        public double CurrentLoad { get; private set; }
        public double FirmwareVersion { get; private set; }
        public int FreeLocomotiveSlots { get; private set; }
        public bool DataTransmissionError { get; private set; }
        public bool UpdateMode { get; private set; }
        public bool SoftwareReset { get; private set; }
        public DiMAXcondition Condition { get; private set; }


        internal DiMAXstatus() {
            LastUpdate = DateTime.MinValue;
            Condition = DiMAXcondition.Unknown;
        }

        internal void SystemStatusUpdate(DiMAXcommandStation commandStation, DiMAXpacket statusPacket) {
            byte s = statusPacket.Parameters[0];

            DataTransmissionError = (s & 0x10) != 0;
            UpdateMode = (s & 0x08) != 0;
            SoftwareReset = (s & 0x04) != 0;

            switch (s & 0x03) {
                case 0: Condition = DiMAXcondition.Normal; break;
                case 1: Condition = DiMAXcondition.EmergencryStop; break;
                case 2: Condition = DiMAXcondition.Reset; break;
                default: Condition = DiMAXcondition.Unknown; break;
            }

            EventManager.Event(new LayoutEvent<DiMAXcommandStation, DiMAXstatus>("DiMAX-status-updated", commandStation, this).SetCommandStation(commandStation));
        }

        internal void ExtendedStatusUpdate(DiMAXcommandStation commandStation, DiMAXpacket statusPacket) {
            switch (statusPacket.Parameters[2] & 0x3f) {
                case 0x01: CommandStationType = "DiMAX 1200Z"; break;
                case 0x02: CommandStationType = "DiMAX 800Z"; break;
                default: CommandStationType = "Unknown type"; break;
            }

            CurrentLimit = statusPacket.Parameters[0] & 0x0f;
            CurrentLoad = (double)statusPacket.Parameters[1] * 0.1;
            FirmwareVersion = ((statusPacket.Parameters[3] & 0xf0) >> 4) + (double)(statusPacket.Parameters[3] & 0x0f) / 10;
            FreeLocomotiveSlots = statusPacket.Parameters[4] & 0x3f;
            LastUpdate = DateTime.Now;

            EventManager.Event(new LayoutEvent<DiMAXcommandStation, DiMAXstatus>("DiMAX-status-updated", commandStation, this).SetCommandStation(commandStation));
        }
    }

    #endregion

    #region DiMAX command codes and command packet definition

    public class DiMAXException : Exception {
        public DiMAXException() {
        }

        public DiMAXException(string message)
            : base(message) {
        }

        public DiMAXException(string message, Exception inner)
            : base(message, inner) {
        }
    }

    public enum DiMAXcommandCode {
        DirectReply = 0x00,         // Packet is direct reply of some command

        LocoSpeedControl = 0x01,
        LocoFunctionControl = 0x02,
        LocoSelection = 0x04,
        LocoConfig = 0x05,

        TurnoutControl = 0x0a,
        FeedbackControl = 0x0b,

        EmergencyStopFinish = 0x10,
        EmergencyStopBegin = 0x11,
        EmergencyStopWithTrackPower = 0x12,

        SaveAllConfigInCommandStation = 0x1d,
        InterfaceAnnouncement = 0x18,

        ProgrammingRegister = 0x14,
        ProgrammingCV = 0x15,
        ProgrammingCVPOM = 0x15,
        ReadCV = 0x16,
        ReadNewLocoData = 0x17,

        AddressFeedback = 0x09,


        StatusBase = 0x100,             // Status messages are above this number 

        SystemStatus = 0x101,
        UpdateMessage = 0x103,
        MessageSent = 0x1ff,
        ExtendedSystemStatus = 0x105,
    }

    /// <summary>
    /// Encode DiMAX command/reply packet
    /// </summary>
    public class DiMAXpacket {
        readonly DiMAXcommandCode commandCode;       // Command code
        readonly byte parameterCount;                // Number of parameters (0..7)
        readonly byte[] parameters;                 // The parameters

        public DiMAXpacket(DiMAXcommandCode commandCode) {
            this.commandCode = commandCode;
            this.parameterCount = 0;
            parameters = new byte[0];
        }

        public DiMAXpacket(DiMAXcommandCode commandCode, params byte[] parameters) {
            if (parameters.Length > 7)
                throw new ArgumentException("Too many DiMAX command parameters");

            this.parameterCount = (byte)parameters.Length;
            this.commandCode = commandCode;
            this.parameters = new byte[this.parameterCount];
            parameters.CopyTo(this.parameters, 0);
        }

        public DiMAXpacket(DiMAXcommandCode commandCode, int parameterCount) {
            if (parameterCount < 0 || parameterCount > 7)
                throw new ArgumentException("Invalid number of DiMAX command parameters");

            this.parameterCount = (byte)parameterCount;
            this.commandCode = commandCode;
            this.parameters = new byte[parameterCount];
        }

        static void CheckPacket(byte lengthAndCommandCode, byte[] buffer) {
            byte x = lengthAndCommandCode;
            foreach (byte b in buffer)
                x ^= b;

            if (x != 0)
                throw new DiMAXException("DiMAX packet error (Xor test failed)");
        }

        public DiMAXpacket(byte lengthAndCommandCode, byte[] buffer) {
            CheckPacket(lengthAndCommandCode, buffer);

            if (lengthAndCommandCode == 0) {
                // Status message
                this.parameterCount = (byte)buffer[1];

                if (parameterCount == 1)
                    commandCode = (buffer[2] & 0xe0) == 0x80 ? DiMAXcommandCode.SystemStatus : DiMAXcommandCode.MessageSent;
                else
                    commandCode = (DiMAXcommandCode)(parameterCount + DiMAXcommandCode.StatusBase);

                this.parameters = new byte[parameterCount];
                for (int i = 0; i < parameterCount; i++)
                    parameters[i] = buffer[i + 2];
            }
            else {
                byte commandValue = (byte)(lengthAndCommandCode & 0x1f);

                commandCode = (DiMAXcommandCode)commandValue;

                if (commandCode != DiMAXcommandCode.DirectReply) {
                    // Command packet
                    this.parameterCount = (byte)((lengthAndCommandCode >> 5) & 0x07);

                    if (buffer.Length != parameterCount + 1)
                        throw new DiMAXException("Buffer length mismatch");

                    this.parameters = new byte[parameterCount];

                    for (int i = 1; i < buffer.Length; i++)
                        parameters[i - 1] = buffer[i];
                }
                else {
                    // Direct reply packet
                    commandCode = DiMAXcommandCode.DirectReply;

                    this.parameterCount = (byte)buffer[1];
                    this.parameters = new byte[parameterCount];

                    for (int i = 0; i < parameterCount; i++)
                        parameters[i] = buffer[i + 2];
                }
            }
        }

        /// <summary>
        /// Return byte buffer encoding this command.
        /// </summary>
        /// <returns>A byte array with the packet bytes</returns>
        public byte[] GetBuffer() {
            byte[] buffer = new byte[2 + parameterCount];

            buffer[0] = (byte)(parameterCount << 5 | (byte)commandCode);
            if (parameters != null)
                parameters.CopyTo(buffer, 2);

            // Calculate Xor of all byte (command/length byte and parameter bytes)
            buffer[1] = buffer[0];
            for (int i = 2; i < buffer.Length; i++)
                buffer[1] ^= buffer[i];

            return buffer;
        }

        /// <summary>
        /// The DiMAX command code of this packet
        /// </summary>
        public DiMAXcommandCode CommandCode => commandCode;

        public int ParameterCount => parameterCount;

        public byte[] Parameters => parameters;

        internal void Dump(string textMessage) {
            Trace.Write(textMessage + ", type " + CommandCode.ToString() + " Parameters: ");
            foreach (byte p in Parameters)
                Trace.Write(" " + p.ToString("x2"));
            Trace.WriteLine("");
        }
    }

    public class DiMAXencodedPacket {
        readonly DiMAXpacket packet;

        public DiMAXencodedPacket(DiMAXpacket packet) {
            this.packet = packet;
        }

        public DiMAXpacket Packet => packet;
    }

    public class DiMAXturnoutOrFeedbackControlPacket : DiMAXencodedPacket {

        public DiMAXturnoutOrFeedbackControlPacket(DiMAXpacket packet) : base(packet) {
        }

        public int Address => ((Packet.Parameters[0] & 0x1f) << 6) | ((Packet.Parameters[1] >> 2) & 0x3f);

        public int State => Packet.Parameters[1] & 1;
    }

    public class DiMAXlocoControlPacket : DiMAXencodedPacket {
        public DiMAXlocoControlPacket(DiMAXpacket packet)
            : base(packet) {
        }

        public int Unit => ((Packet.Parameters[0] & 0x3f) << 8) | Packet.Parameters[1];
    }

    public class DiMAXlocoSpeedControlPacket : DiMAXlocoControlPacket {
        public DiMAXlocoSpeedControlPacket(DiMAXpacket packet)
            : base(packet) {
        }


        public int SpeedInSteps {
            get {
                if ((Packet.Parameters[2] & 0x80) == 0)
                    return -(int)(Packet.Parameters[2] & 0x7f);
                else
                    return Packet.Parameters[2] & 0x7f;
            }
        }
    }

    public class DiMAXlocoFunctionControlPacket : DiMAXlocoControlPacket {
        public DiMAXlocoFunctionControlPacket(DiMAXpacket packet)
            : base(packet) {
        }

        public bool Lights => (Packet.Parameters[2] & 0x80) != 0;

        public bool FunctionActive => (Packet.Parameters[2] & 0x10) != 0;

        public int FunctionNumber => (Packet.Parameters[2] & 0x0f);
    }

    #endregion

    #region DiMAX Command Classes

    class DiMAXcommandBase : OutputCommandBase {

        public DiMAXcommandBase(DiMAXcommandStation commandStation) {
            this.CommandStation = commandStation;
        }

        public DiMAXcommandBase(DiMAXcommandStation commandStation, DiMAXpacket dimaxPacket) {
            this.CommandStation = commandStation;
            this.Packet = dimaxPacket;
        }

        public DiMAXcommandStation CommandStation { get; }

        public Stream? Stream => CommandStation.CommunicationStream;

        public DiMAXpacket? Packet {
            get; protected set;
        }

        public override void Do() {
            if (Packet != null && Stream != null) {
                byte[] buffer = Packet.GetBuffer();

                if (DiMAXcommandStation.TraceRawData.Level == TraceLevel.Verbose) {
                    Trace.Write("Sending DiMAX packet (length " + buffer.Length + "):");
                    foreach (var b in buffer)
                        Trace.Write(" " + b.ToString("x2"));
                    Trace.WriteLine("");
                }

                try {
                    Stream.Write(buffer, 0, buffer.Length);
                    Stream.Flush();
                }
                catch (OperationCanceledException) { }
            }
        }
    }

    class DiMAXsynchronousCommandBase : DiMAXcommandBase, IOutputCommandWithReply {
        public DiMAXsynchronousCommandBase(DiMAXcommandStation commandStation, DiMAXpacket packet)
            : base(commandStation, packet) {
            DefaultWaitPeriod = 250;
        }

        #region ICommandStationSynchronousCommand Members

        public int Timeout => 2000;

        public virtual void OnTimeout() {
            Trace.WriteLine("Reply was not received on time for " + this.GetType().Name + " DiMAX command");
        }

        public void OnReply(object replyPacket) {
            OnReplyPacket((DiMAXpacket)replyPacket);
        }

        public virtual void OnReplyPacket(DiMAXpacket replyPacket) {
            Completed(null);
        }

        #endregion
    }

    class DiMAXpowerDisconnect : DiMAXcommandBase {
        public DiMAXpowerDisconnect(DiMAXcommandStation commandStation)
            : base(commandStation, new DiMAXpacket(DiMAXcommandCode.EmergencyStopBegin)) {
        }
    }

    class DiMAXpowerConnect : DiMAXcommandBase {
        public DiMAXpowerConnect(DiMAXcommandStation commandStation)
            : base(commandStation, new DiMAXpacket(DiMAXcommandCode.EmergencyStopFinish)) {
        }
    }

    class DiMAXchangeAccessoryState : DiMAXcommandBase {
        readonly string description;

        public DiMAXchangeAccessoryState(DiMAXcommandStation commandStation, int unit, int state)
            : base(commandStation, new DiMAXpacket(DiMAXcommandCode.TurnoutControl, new byte[] { (byte)(unit >> 6), (byte)(((unit & 0x3f) << 2) | 2 | state) })) {

            description = $"Set accessory {unit} state to {state}";
        }

        public override string ToString() => description;
    }

    class DiMAXlocomotiveMotion : DiMAXcommandBase {
        readonly string description;

        public DiMAXlocomotiveMotion(DiMAXcommandStation commandStation, int unit, int speed)
            : base(commandStation, new DiMAXpacket(DiMAXcommandCode.LocoSpeedControl,
            new byte[] { (byte)(unit >> 8), (byte)(unit & 0xff), (byte)((speed >= 0 ? 0x80 : 0x00) | Math.Abs(speed)) })) {
            description = $"Locomotive {unit} set speed to {speed}";
        }

        public override string ToString() => description;
    }

    class DiMAXlocomotiveFunction : DiMAXcommandBase {
        readonly string description;

        public DiMAXlocomotiveFunction(DiMAXcommandStation commandStation, int unit, int functionNumber, bool functionState, bool lightsOn) :
            base(commandStation, new DiMAXpacket(DiMAXcommandCode.LocoFunctionControl,
            new byte[] { (byte)(unit >> 8), (byte)(unit & 0xff), (byte)((lightsOn ? 0x80 : 0) | (functionState ? 0x20 : 0) | (functionNumber & 0x1f)) })) {
            description = $"Loco function, unit {unit} func# {functionNumber} state: {functionState} lights: {lightsOn}";
        }

        public override string ToString() => description;
    }

    class DiMAXprogrammingCommandBase : DiMAXsynchronousCommandBase {
        public DiMAXprogrammingCommandBase(DiMAXcommandStation commandStation, DiMAXpacket commandPacket) :
            base(commandStation, commandPacket) {
        }

        public override void OnReplyPacket(DiMAXpacket replyPacket) {
            Completed(GetProgrammingResult(replyPacket));
        }

        protected static LayoutActionResult GetProgrammingResult(DiMAXpacket replyPacket) {
            switch ((replyPacket.Parameters[0] & 0x1c) >> 2) {
                case 0: return LayoutActionResult.NoResponse;
                case 1: return LayoutActionResult.Overload;
                case 4: return LayoutActionResult.Ok;
                default: return LayoutActionResult.UnexpectedReply;
            }
        }
    }

    class DiMAXinterfaceAnnouncement : DiMAXcommandBase {
        public DiMAXinterfaceAnnouncement(DiMAXcommandStation commandStation, bool enableStatusMessages) :
            base(commandStation, new DiMAXpacket(DiMAXcommandCode.InterfaceAnnouncement, new byte[] { (byte)(enableStatusMessages ? 0x01 : 0x00), 0x00, 0x00, (byte)(DiMAXcommandStation.MID_LayoutManager >> 8), (byte)(DiMAXcommandStation.MID_LayoutManager & 0xff) })) {
        }
    }


    class DiMAXprogramRegister : DiMAXprogrammingCommandBase {
        readonly string description;

        public DiMAXprogramRegister(DiMAXcommandStation commandStation, byte registerNumber, byte value) :
            base(commandStation, new DiMAXpacket(DiMAXcommandCode.ProgrammingRegister, new byte[] { (byte)(registerNumber - 1), value })) {
            description = $"Program register {registerNumber} value {value}";
        }

        public override string ToString() => description;
    }

    class DiMAXprogramCV : DiMAXprogrammingCommandBase {
        readonly string description;

        public DiMAXprogramCV(DiMAXcommandStation commandStation, int cvNumber, byte data)
            : base(commandStation, new DiMAXpacket(DiMAXcommandCode.ProgrammingCV, new byte[] { (byte)((cvNumber - 1) >> 8), (byte)((cvNumber - 1) & 0xff), data })) {
            description = $"Program CV {cvNumber} data {data}";
        }

        public override string ToString() => description;
    }

    class DiMAXprogramCVonTrack : DiMAXprogrammingCommandBase {
        readonly string description;

        public DiMAXprogramCVonTrack(DiMAXcommandStation commandStation, int address, int cvNumber, byte data) :
            base(commandStation, new DiMAXpacket(DiMAXcommandCode.ProgrammingCVPOM, new byte[] { (byte)((cvNumber - 1) >> 8), (byte)((cvNumber - 1) & 0xff), data, (byte)(address >> 8), (byte)(address & 0xff) })) {
            description = $"Program address {address} CV {cvNumber} data {data}";
        }

        public override string ToString() => description;
    }

    class DiMAXreadCV : DiMAXprogrammingCommandBase {
        readonly string description;

        public DiMAXreadCV(DiMAXcommandStation commandStation, int cvNumber, Action<LayoutActionResult, int, byte> doneCallback) : base(commandStation, new DiMAXpacket(DiMAXcommandCode.ReadCV, new byte[] { (byte)((cvNumber - 1) >> 8), (byte)((cvNumber - 1) & 0xff) })) {
            description = $"Read cv {cvNumber}";
        }

        public override void OnReplyPacket(DiMAXpacket replyPacket) {
            Completed(new ReadCVresult(GetProgrammingResult(replyPacket), ((replyPacket.Parameters[0] & 3) << 8) | replyPacket.Parameters[1], replyPacket.Parameters[2]));
        }

        public override string ToString() => description;
    }

    class DiMAXlocomotiveSelection : DiMAXsynchronousCommandBase {
        readonly string description;

        public DiMAXlocomotiveSelection(DiMAXcommandStation commandStation, int address, bool select, bool deselectActive, bool unconditional) :
            base(commandStation, new DiMAXpacket(DiMAXcommandCode.LocoSelection, new byte[] { (byte)(address >> 8), (byte)(address & 0xff),
                (byte)((unconditional ? 0x80 : 0) | (deselectActive ? 0x40 : 0)  | (select ? 0x10 : 0)) })) {
            description = $"select locomotive {address} select {select} deselect-active {deselectActive} unconditional {unconditional}";
        }

        public override void OnReplyPacket(DiMAXpacket replyPacket) {
            base.OnReplyPacket(replyPacket);
        }

        public override string ToString() => description;
    }

    class DiMAXlocomotiveRegistration : DiMAXcommandBase {
        readonly string description;

        public DiMAXlocomotiveRegistration(DiMAXcommandStation commandStation, int address, bool storeInNonVolatile = false, bool addressIsMotorola = false, bool parallelFunctions = true, int speedSteps = 28, int image = 0) :
            base(commandStation, new DiMAXpacket(DiMAXcommandCode.LocoConfig, new byte[] {
                (byte)((address & 0x3f00) >> 8), (byte)(address & 0xff),
                (byte)((storeInNonVolatile ? 0x80 : 0) | (addressIsMotorola ? 0x08 : 0) | (parallelFunctions ? 0x04 : 0) | GetSpeedStepsMask(speedSteps)),
                (byte)image
            })) {
            description = $"Register locomotive - address {address} storeInNonVolatile {storeInNonVolatile} addressIsMotorola {addressIsMotorola} parallelFunctions {parallelFunctions} speedSteps {speedSteps} image {image}";
        }

        static byte GetSpeedStepsMask(int speedSteps) {
            switch (speedSteps) {
                case 14: return 0x0;
                case 28: return 0x1;
                case 128: return 0x2;
                default: throw new ArgumentException("Invalid speed steps value (can be 14, 28 or 128)");
            }
        }

        public override string ToString() => description;
    }

    class DiMAXlocomotiveUnregister : DiMAXcommandBase {
        readonly string description;

        public DiMAXlocomotiveUnregister(DiMAXcommandStation commandStation, int address) :
            base(commandStation, new DiMAXpacket(DiMAXcommandCode.LocoConfig, new byte[] { (byte)((address & 0x3f00) >> 8), (byte)(address & 0xff) })) {
            description = $"Unregister locomotive {address}";
        }

        public override string ToString() => description;
    }

    #endregion
}
