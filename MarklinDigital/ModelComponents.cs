using LayoutManager;
using LayoutManager.CommonUI;
using LayoutManager.Components;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace MarklinDigital {
    /// <summary>
    /// Summary description for MTScomponents.
    /// </summary>
#pragma warning disable IDE0051, IDE0060, IDE0052

    public class MarklinDigitalCentralStation : LayoutCommandStationComponent {
        private static readonly LayoutTraceSwitch traceMarklinDigital = new("MarkinDigital", "Marklin Digital Interface (P50)");
        private OutputManager? commandStationManager;

        private ControlBus? _S88bus = null;

        private const int queuePowerCommands = 0;               // Queue for power on/off etc.
        private const int queueLayoutSwitchingCommands = 1; // Queue for turnout/lights control commands
        private const int queueLocoCommands = 2;                // Queue for locomotive control commands
        private const int feedbackUnitPollingTime = 20;     // time in milliseconds for polling feedback unit
        private const string A_ReadIntervalTimeout = "ReadIntervalTimeout";
        private const string A_ReadTotalTimeoutConstant = "ReadTotalTimeoutConstant";
        private const string A_FeedbackPolling = "FeedbackPolling";
        private const string A_MinTimeBetweenSpeedSteps = "MinTimeBetweenSpeedSteps";

        public MarklinDigitalCentralStation() {
            this.XmlDocument.LoadXml(
                "<MarklinDigital FeedbackPolling='10' ReadIntervalTimeout='20' ReadTotalTimeoutConstant='5000'>" +
                "<ModeString>baud=2400 parity=N data=8 stop=2 octs=off odsr=off</ModeString>" +
                "</MarklinDigital>"
                );
        }

        public int FeedbackPolling => (int)Element.AttributeValue(A_FeedbackPolling);

        public ControlBus S88Bus => _S88bus ??= Ensure.NotNull<ControlBus>(LayoutModel.ControlManager.Buses.GetBus(this, "S88BUS"));

        #region Specific implementation to base class overrideble methods and properties

        public override IList<string> BusTypeNames => Array.AsReadOnly<string>(new string[] { "Motorola", "S88BUS" });

        protected override void OnCommunicationSetup() {
            base.OnCommunicationSetup();

            if (!Element.HasAttribute(A_ReadIntervalTimeout))
                Element.SetAttributeValue(A_ReadIntervalTimeout, 20);
            if (!Element.HasAttribute(A_ReadTotalTimeoutConstant))
                Element.SetAttributeValue(A_ReadTotalTimeoutConstant, 5000);
        }

        public override DigitalPowerFormats SupportedDigitalPowerFormats => DigitalPowerFormats.Motorola;

        private int GetNumberOfFeedbackUnits() {
            int maxAddress = 0;

            foreach (ControlModule module in S88Bus.Modules)
                if (module.Address > maxAddress)
                    maxAddress = module.Address;

            return maxAddress;
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            _S88bus = null;

            commandStationManager = new OutputManager(A_FeedbackPolling, 3);

            int feedbackUnits = GetNumberOfFeedbackUnits();

            if (feedbackUnits > 0) {
                int waitPeriod = (1000 - (feedbackUnits * feedbackUnitPollingTime)) / FeedbackPolling;

                if (waitPeriod < 0)
                    waitPeriod = 0;

                Trace.WriteLine(" " + feedbackUnits + " feedback units, time between polling each unit is " + waitPeriod + " milliseconds");

                for (int unit = 1; unit <= feedbackUnits; unit++)
                    commandStationManager.AddIdleCommand(new MarklinGetFeedback(this, InterThreadEventInvoker, unit, waitPeriod));
            }

            commandStationManager.AddIdleCommand(new EndTrainsAnalysisCommandStationCommand(this, 10));
            commandStationManager.Start();
        }

        protected override void OnCleanup() {
            base.OnCleanup();

            if (OperationMode) {
                // Set all trains to non-reverse state
                foreach (XmlElement trainStateElement in LayoutModel.StateManager.Trains.Element) {
                    var train = new TrainStateInfo(trainStateElement) {
                        SpeedInSteps = 0
                    };
                    var commandStation = Dispatch.Call.GetCommandStation(train);

                    if (train.MotionDirection == LocomotiveOrientation.Backward) {
                        foreach (TrainLocomotiveInfo trainLoco in train.Locomotives)
                            if (trainLoco.Orientation == LocomotiveOrientation.Forward)
                                Dispatch.Call.LocomotiveReverseMotionDirectionCommand(commandStation, trainLoco.Locomotive);
                    }
                }
            }
        }

        protected async override Task OnTerminateCommunication() {
            await base.OnTerminateCommunication();

            if (commandStationManager != null) {
                await commandStationManager.WaitForIdle();
                if (commandStationManager != null) {
                    commandStationManager.Terminate();
                    commandStationManager = null;
                }
            }
        }

        public override bool LayoutEmulationSupported => true;

        public override bool TrainsAnalysisSupported => true;

        protected override ILayoutCommandStationEmulator CreateCommandStationEmulator(string pipeName) => new MarklinCommandStationEmulator(this, pipeName, EmulationTickTime);

        public override bool DesignTimeLayoutActivationSupported => true;

        #endregion

        #region Notification Event Handlers

        [DispatchTarget]
        private void OnTrainIsRemoved(TrainStateInfo train) {
            var commandStation = Dispatch.Call.GetCommandStation(train);

            if (train.MotionDirection == LocomotiveOrientation.Backward) {
                foreach (TrainLocomotiveInfo trainLoco in train.Locomotives)
                    if (trainLoco.Orientation == LocomotiveOrientation.Forward)
                        Dispatch.Call.LocomotiveReverseMotionDirectionCommand(commandStation, trainLoco.Locomotive);
            }
        }

        #endregion

        #region Request event handlers

        [DispatchTarget(Order = 100)]
        private void AddToTrainOperationMenu(TrainStateInfo train, MenuOrMenuItem menu) {
            bool trainInActiveTrip = Dispatch.Call.IsTrainInActiveTrip(train);

            if (train.CommandStation != null && train.CommandStation.Id == Id) {
                if (train.Locomotives.Count == 1) {
                    LocomotiveInfo loco = train.Locomotives[0].Locomotive;

                    menu.Items.Add(new ToggleLocoDirectionMenuItem(loco, "Toggle Locomotive Direction"));
                }
                else {
                    var toggleLocoDirection = new LayoutMenuItem("Toggle Locomotive direction");

                    foreach (TrainLocomotiveInfo trainLoco in train.Locomotives)
                        toggleLocoDirection.DropDownItems.Add(new ToggleLocoDirectionMenuItem(trainLoco.Locomotive, trainLoco.Name));

                    if (toggleLocoDirection.DropDownItems.Count > 0)
                        menu.Items.Add(toggleLocoDirection);
                }

                if (!trainInActiveTrip)
                    menu.Items.Add(new ToggleLocomotiveManagement(train));

            }
        }

        [DispatchTarget]
        private void DisconnectPowerRequest([DispatchFilter(Type = "IsMyId")] IModelComponentHasNameAndId commandStation) {
            commandStationManager?.AddCommand(queuePowerCommands, new MarklinStopCommand(CommunicationStream));
            PowerOff();
        }

        [DispatchTarget]
        private void ConnectPowerRequest([DispatchFilter(Type = "IsMyId")] IModelComponentHasNameAndId commandStation) {
            commandStationManager?.AddCommand(queuePowerCommands, new MarklinGoCommand(CommunicationStream));
            PowerOn();
        }

        [DispatchTarget]
        private XmlElement GetCommandStationCapabilities([DispatchFilter(Type = "IsMyId")] IModelComponentHasNameAndId commandStation) {
            var minTimeBetweenSpeedSteps = (int?)Element.AttributeValue(A_MinTimeBetweenSpeedSteps) ?? 20;
            return new CommandStationCapabilitiesInfo {
                MinTimeBetweenSpeedSteps = minTimeBetweenSpeedSteps
            }.Element;
        }

        // Implement command events
        [DispatchTarget]
        private Task ChangeTrackComponentStateCommand([DispatchFilter(Type = "MyId")] IModelComponentHasNameAndId commandStation, ControlConnectionPointReference connectionPointRef, int state) {
            if (commandStationManager == null)
                throw new NullReferenceException(nameof(commandStationManager));

            int address = connectionPointRef.Module.Address + connectionPointRef.Index;
            var tasks = new List<Task> {
                commandStationManager.AddCommand(queueLayoutSwitchingCommands, new MarklinSwitchAccessoryCommand(CommunicationStream, address, state)),
                commandStationManager.AddCommand(queueLayoutSwitchingCommands, new MarklinEndSwitchingProcedure(CommunicationStream))
            };

            if (OperationMode)
                Dispatch.Notification.OnControlConnectionPointStateChanged(connectionPointRef, state);

            return Task.WhenAll(tasks);
        }

        [DispatchTarget]
        private void ChangeSignalStateCommand([DispatchFilter(Type = "IsMyId")] IModelComponentHasNameAndId commandStation, ControlConnectionPointReference connectionPointRef, LayoutSignalState state) {
            if (commandStationManager == null)
                throw new NullReferenceException(nameof(commandStationManager));

            int address = connectionPointRef.Module.Address + connectionPointRef.Index;
            int v;

            if (state == LayoutSignalState.Green)
                v = 0;
            else
                v = 1;

            commandStationManager.AddCommand(queueLayoutSwitchingCommands, new MarklinSwitchAccessoryCommand(CommunicationStream, address, v));
            commandStationManager.AddCommand(queueLayoutSwitchingCommands, new MarklinEndSwitchingProcedure(CommunicationStream));

            if (OperationMode)
                Dispatch.Notification.OnControlConnectionPointStateChanged(connectionPointRef, state == LayoutSignalState.Green ? 1 : 0);
        }

        [DispatchTarget]
        private void LocomotiveReverseMotionDirectionCommand([DispatchFilter(Type = "MyId")] IModelComponentHasPowerOutlets commandStation, LocomotiveInfo loco) {
            if (commandStationManager == null)
                throw new NullReferenceException(nameof(commandStationManager));

            commandStationManager.AddCommand(queueLocoCommands, new MarklinReverseLocomotiveDirection(CommunicationStream, loco.AddressProvider.Unit));
        }

        [DispatchTarget]
        private void LocomotiveMotionCommand([DispatchFilter(Type = "IsMyId")] IModelComponentHasNameAndId commandStation, LocomotiveInfo loco, int speed) {
            if (commandStationManager == null)
                throw new NullReferenceException(nameof(commandStationManager));

            var train = Ensure.NotNull<TrainStateInfo>(LayoutModel.StateManager.Trains[loco.Id]);
            bool auxFunction;

            var locoAuxFunction = loco.GetFunctionByNumber(0);

            if (locoAuxFunction?.Type == LocomotiveFunctionType.OnOff)
                auxFunction = train.GetFunctionState(locoAuxFunction.Name, loco.Id, false);
            else
                auxFunction = loco.HasLights && train.Lights;

            commandStationManager.AddCommand(queueLocoCommands, new MarklinLocomotiveMotion(CommunicationStream, loco.AddressProvider.Unit, speed, auxFunction));
        }

        [LayoutEvent("set-locomotive-lights-command", IfEvent = "*[CommandStation/@Name='`string(Name)`']")]
        private void SetLocomotiveLightsCommand(LayoutEvent e) {
            var loco = Ensure.NotNull<LocomotiveInfo>(e.Sender);
            var train = Ensure.NotNull<TrainStateInfo>(LayoutModel.StateManager.Trains[loco.Id]);

            Dispatch.Call.LocomotiveMotionCommand(Dispatch.Call.GetCommandStation(train), loco, train.SpeedInSteps);
        }

        private byte GetFunctionMask(LocomotiveInfo loco) {
            var train = Ensure.NotNull<TrainStateInfo>(LayoutModel.StateManager.Trains[loco.Id]);
            byte functionMask = 0;

            for (int functionNumber = 0; functionNumber < 4; functionNumber++) {
                var functionDef = loco.GetFunctionByNumber(functionNumber + 1);

                if (functionDef?.Type == LocomotiveFunctionType.OnOff && train.GetFunctionState(functionDef.Name, loco.Id, false))
                    functionMask |= (byte)(1 << functionNumber);
            }

            return functionMask;
        }

        [DispatchTarget]
        void SetLocomotiveFunctionStateCommand([DispatchFilter(Type = "IsMyId")] IModelComponentHasNameAndId commandStation, LocomotiveInfo locomotive, string functionName, bool functionState) {
            if (commandStationManager == null)
                throw new NullReferenceException(nameof(commandStationManager));

            var functionDef = locomotive.GetFunctionByName(functionName);

            if (functionDef == null || functionDef.Number != 0)
                commandStationManager.AddCommand(queueLocoCommands, new MarklinSetFunctions(CommunicationStream, locomotive.AddressProvider.Unit, GetFunctionMask(locomotive)));
            else {
                var train = Ensure.NotNull<TrainStateInfo>(LayoutModel.StateManager.Trains[locomotive.Id]);

                commandStationManager.AddCommand(queueLocoCommands, new MarklinLocomotiveMotion(CommunicationStream, locomotive.AddressProvider.Unit, train.SpeedInSteps,
                    train.GetFunctionState(functionDef.Name, locomotive.Id, false)));
            }
        }

        [DispatchTarget]
        void TriggerLocomotiveFunctionStateCommand([DispatchFilter(Type = "IsMyId")] IModelComponentHasNameAndId commandStation, LocomotiveInfo locomotive, string functionName, bool functionState) {
            var train = Ensure.NotNull<TrainStateInfo>(LayoutModel.StateManager.Trains[locomotive.Id]);
            bool state = train.GetFunctionState(functionName, locomotive.Id, false);

            state = !state;
            train.SetLocomotiveFunctionState(functionName, locomotive.Id, state);
        }

        #endregion

        #region Menu Items

        private class ToggleLocoDirectionMenuItem : LayoutMenuItem {
            private readonly LocomotiveInfo loco;

            public ToggleLocoDirectionMenuItem(LocomotiveInfo loco, string title) {
                this.loco = loco;
                Text = title;
            }

            protected override void OnClick(EventArgs e) {
                Dispatch.Call.LocomotiveReverseMotionDirectionCommand(Dispatch.Call.GetCommandStation(loco), loco);
            }
        }

        private class ToggleLocomotiveManagement : LayoutMenuItem {
            private readonly TrainStateInfo train;

            public ToggleLocomotiveManagement(TrainStateInfo train) {
                this.train = train;

                if (train.Managed)
                    Text = "Disable computer control";
                else
                    Text = "Enable computer control";
            }

            protected override void OnClick(EventArgs e) {
                bool managed = train.Managed;

                if (!managed) {
                    if (train.LocomotiveBlock != null) {
                        var front = Dispatch.Call.GetLocomotiveFront(train.LocomotiveBlock.BlockDefinintion, train.Element);

                        TrainLocationInfo? trainLocationInfo = train.LocationOfBlock(train.LocomotiveBlock);

                        if (trainLocationInfo != null)
                            trainLocationInfo.DisplayFront = front ?? train.LocomotiveBlock.TrackEdges[0].ConnectionPoint;
                    }
                }

                foreach (TrainLocomotiveInfo trainLoco in train.Locomotives)
                    trainLoco.Locomotive.NotManaged = managed;
            }
        }

        #endregion
    }

    #region Marklin Digial Command Classes

    internal abstract class MarklinCommand : OutputCommandBase {
        protected MarklinCommand(Stream? stream) {
            this.Stream = stream;
        }

        public Stream? Stream { get; }

        protected void Output(byte v) {
            Thread.Sleep(10);
            Stream?.WriteByte(v);
            Stream?.Flush();
        }

        protected void Flush() {
            Stream?.Flush();
        }
    }

    internal class MarklinGoCommand : MarklinCommand {
        public MarklinGoCommand(Stream? comm) : base(comm) {
        }

        public override void Do() {
            Output(96);
            Flush();
        }
    }

    internal class MarklinStopCommand : MarklinCommand {
        public MarklinStopCommand(Stream? comm) : base(comm) {
        }

        public override void Do() {
            Output(97);
            Flush();
        }
    }

    internal class MarklinSwitchAccessoryCommand : MarklinCommand {
        private readonly int address;
        private readonly int state;

        public MarklinSwitchAccessoryCommand(Stream? comm, int address, int state) : base(comm) {
            this.address = address;
            this.state = state;
        }

        public override void Do() {
            if (state == 0)
                Output(33);
            else if (state == 1)
                Output(34);
            else
                throw new ArgumentException("Invalid marklin accessory state: " + state + " address: " + address);
            Output((byte)address);
            Flush();
        }

        // Wait 100 milliseconds before sending next command from this queue
        public override int WaitPeriod => 100;
    }

    internal class MarklinEndSwitchingProcedure : MarklinCommand {
        public MarklinEndSwitchingProcedure(Stream? comm) : base(comm) {
        }

        public override void Do() {
            Output(32);
            Flush();
        }
    }

    internal class MarklinReverseLocomotiveDirection : MarklinCommand {
        private readonly int address;

        public MarklinReverseLocomotiveDirection(Stream? comm, int address) : base(comm) {
            this.address = address;
        }

        public override void Do() {
            Output(15);
            Output((byte)address);
            Flush();
        }
    }

    internal class MarklinLocomotiveMotion : MarklinCommand {
        private readonly int address;
        private readonly byte speed;
        private readonly bool auxFunction;

        public MarklinLocomotiveMotion(Stream? comm, int address, int speed, bool auxFunction) : base(comm) {
            this.address = address;

            if (speed > 0)
                this.speed = (byte)speed;
            else
                this.speed = (byte)-speed;

            this.auxFunction = auxFunction;
        }

        public override void Do() {
            byte v = speed;

            if (auxFunction)
                v |= 0x10;
            Output(v);
            Output((byte)address);
            Flush();
        }
    }

    internal class MarklinSetFunctions : MarklinCommand {
        private readonly int address;
        private readonly byte functionMask;

        public MarklinSetFunctions(Stream? comm, int address, byte functionMask) : base(comm) {
            this.address = address;
            this.functionMask = functionMask;
        }

        public MarklinSetFunctions(Stream comm, int address, byte functionMask, int waitPeriod) : base(comm) {
            this.address = address;
            this.functionMask = functionMask;
            DefaultWaitPeriod = waitPeriod;
        }

        public override void Do() {
            byte v = (byte)((functionMask & 0x0f) | 0x40);

            Output(v);
            Output((byte)address);
            Flush();
        }
    }

    internal struct MarklinFeedbackContactResult {
        public int ContactNo;
        public bool IsSet;

        public MarklinFeedbackContactResult(int contactNo, bool isSet) {
            this.ContactNo = contactNo;
            this.IsSet = isSet;
        }
    };

    internal struct MarklinFeedbackResult {
        public int Unit;
        public MarklinFeedbackContactResult[] Contacts;

        public MarklinFeedbackResult(int unit) {
            this.Unit = unit;
            this.Contacts = new MarklinFeedbackContactResult[16];
        }
    }

    internal delegate void MarklinFeedbackResultCallback(MarklinFeedbackResult feedbackResult);

    internal class MarklinGetFeedback : MarklinCommand, IOutputIdlecommand {
        private readonly ILayoutInterThreadEventInvoker invoker;
        private readonly int unit;
        private int feedback = 0;
        private readonly MarklinDigitalCentralStation commandStation;

        public MarklinGetFeedback(MarklinDigitalCentralStation commandStation, ILayoutInterThreadEventInvoker invoker, int unit, int waitPeriod) : base(commandStation.CommunicationStream) {
            this.commandStation = commandStation;
            this.invoker = invoker;
            this.unit = unit;
            DefaultWaitPeriod = waitPeriod;
        }

        private void ProcessFeedback(MarklinFeedbackResult feedbackResult) {
            for (int i = 0; i < 16 && feedbackResult.Contacts[i].ContactNo > 0; i++) {
                if (commandStation.OperationMode)
                    invoker.Queue(() =>
                      Dispatch.Notification.OnControlConnectionPointStateChanged(
                        new ControlConnectionPointReference(commandStation.S88Bus, feedbackResult.Unit, feedbackResult.Contacts[i].ContactNo - 1), feedbackResult.Contacts[i].IsSet ? 1 : 0)
                    );
                else
                    invoker.Queue(() => Dispatch.Notification.OnDesignTimeCommandStationEvent(new CommandStationInputEvent(commandStation, commandStation.S88Bus,
                        (feedbackResult.Unit * 16) + feedbackResult.Contacts[i].ContactNo - 1, feedbackResult.Contacts[i].IsSet ? 1 : 0)));
            }
        }

        public override void Do() {
            if (Stream == null)
                return;

            Output((byte)(192 + unit));
            Flush();

            byte[] buffer = new Byte[2];
            int bytesRead = Stream.Read(buffer, 0, 2);

            if (bytesRead == 2) {
                int low = (int)buffer[0];
                int high = (int)buffer[1];
                int newValue = (low << 8) | high;

                if (newValue != feedback) {
                    var feedbackResult = new MarklinFeedbackResult(unit);
                    int changeIndex = 0;

                    for (int contactNo = 1; contactNo <= 16; contactNo++) {
                        int mask = 1 << (16 - contactNo);

                        if ((feedback & mask) != (newValue & mask))
                            feedbackResult.Contacts[changeIndex++] = new MarklinFeedbackContactResult(contactNo, (newValue & mask) != 0);
                    }

                    if (changeIndex < 16)
                        feedbackResult.Contacts[changeIndex] = new MarklinFeedbackContactResult(0, false);      // Mark last one

                    ProcessFeedback(feedbackResult);
                    feedback = newValue;
                }
            }
            else
                Trace.WriteLine("Read feedback returned " + bytesRead + " bytes (expected 2 bytes), ignoring data");
        }

        #region ICommandStationIdlecommand Members

        public bool RemoveFromQueue => false;

        #endregion
    }

    #endregion
}
