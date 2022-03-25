using LayoutManager;
using LayoutManager.Components;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Xml;

#nullable enable
#pragma warning disable IDE0051, IDE0060
namespace LayoutEmulation {
    #region Interfaces

    /// <summary>
    /// The ILayoutEmulationEnvironment is used by internal classes to access common services
    /// </summary>
    /// 
    public interface ILayoutEmulationEnvironment {
        /// <summary>
        /// Number of speed steps for the emulated locomotives
        /// </summary>
        int SpeedSteps { get; }

        /// <summary>
        /// Number of timer ticks each speed steps adds to the time between motions
        /// </summary>
        int TicksPerSpeedStep { get; }

        /// <summary>
        /// Object used for synchronizing access to the emulation data structures
        /// </summary>
        object Sync { get; }

        /// <summary>
        /// Topology services
        /// </summary>
        ILayoutTopologyServices TopologyServices { get; }

        ////// Operations
        ///
        /// <summary>
        /// Return the connection point to which split point from is to be connected. This is based on the current setting of the turnout
        /// </summary>
        LayoutComponentConnectionPoint FindRouteThroughMultiPathComponent(IModelComponentIsMultiPath turnout, LayoutComponentConnectionPoint from);

        ////// Callback methods - called from operations in order to report events

        /// <summary>
        /// Callback that is called if the locomotive fall from the track
        /// </summary>
        /// <param name="locomotive"></param>
        void FireLocomotiveFallFromTrack(LocomotiveState locomotive, LocomotiveOrientation direction, int speed);

        /// <summary>
        /// Called when a locomotive is moved
        /// </summary>
        /// <param name="locomotive">The locomotive</param>
        /// <param name="location">The new location, connection point is the front</param>
        /// <param name="rear">The rear of the locomotive</param>
        void FireLocomotiveMoved(LocomotiveState locomotive, LocomotiveOrientation direction, int speed);
    }

    #endregion

    [LayoutModule("Layout Emulator")]
    public class LayoutEmulator : LayoutModuleBase, ILayoutEmulationEnvironment, ILayoutEmulatorServices {
        private ILayoutTopologyServices? _topologyServices;
        private readonly Dictionary<Guid, CommandStationObjects> commandStations = new Dictionary<Guid, CommandStationObjects>();
        private Timer? emulationTimer = null;
        private int operationNesting = 0;
        private int tickTime = 200;
        private bool emulateTrainMotion = true;

        #region Implementation of ILayoutEmulationEnvironment

        #region Properties

        public ILayoutTopologyServices TopologyServices => _topologyServices ??= Dispatch.Call.GetTopologyServices();

        public int SpeedSteps => 15;            // 0 - 14

        public int TicksPerSpeedStep => 1;

        public object Sync => this;

        #endregion

        #region Operations

        public LayoutComponentConnectionPoint FindRouteThroughMultiPathComponent(IModelComponentIsMultiPath turnout, LayoutComponentConnectionPoint from) {
            int switchState = 0;

            if (turnout is LayoutThreeWayTurnoutComponent) {
                int stateRight = GetConnectionPointSwitchState(Ensure.NotNull<ControlConnectionPoint>(LayoutModel.ControlManager.ConnectionPoints[turnout, "ControlRight"], "controlRight"));
                int stateLeft = GetConnectionPointSwitchState(Ensure.NotNull<ControlConnectionPoint>(LayoutModel.ControlManager.ConnectionPoints[turnout, "ControlLeft"], "controlLeft"));

                if (stateRight == 0 && stateLeft == 0)
                    switchState = 0;
                else if (stateRight == 1 && stateLeft == 0)
                    switchState = 1;
                else if (stateRight == 0 && stateLeft == 1)
                    switchState = 2;
            }
            else {
                var connectionPoint = LayoutModel.ControlManager.ConnectionPoints[turnout]?[0];

                if (connectionPoint != null)
                    switchState = GetConnectionPointSwitchState(connectionPoint);
            }

            if (switchState < 0)
                return turnout.ConnectTo(from, LayoutComponentConnectionType.Passage)[0];
            else {
                if (turnout is IModelComponentHasReverseLogic turnoutHasReverseLogic && turnoutHasReverseLogic.ReverseLogic)
                    switchState = 1 - switchState;

                return turnout.ConnectTo(from, switchState);
            }
        }

        private int GetConnectionPointSwitchState(ControlConnectionPoint connectionPoint) {
            int address = connectionPoint.Module.Address + connectionPoint.Index;

            int switchState = GetTurnoutSwitchState(connectionPoint.Module.Bus.BusProviderId, address);
            return switchState;
        }

        #endregion

        #region Callbacks

        public void FireLocomotiveFallFromTrack(LocomotiveState locomotive, LocomotiveOrientation direction, int speed) {
            LocomotiveFallFromTrack?.Invoke(this, new LocomotiveMovedEventArgs(locomotive.CommandStationId, locomotive.Unit, locomotive.Location, direction, speed));
        }

        public void FireLocomotiveMoved(LocomotiveState locomotive, LocomotiveOrientation direction, int speed) {
            LocomotiveMoved?.Invoke(this, new LocomotiveMovedEventArgs(locomotive.CommandStationId, locomotive.Unit, locomotive.Location, direction, speed));
        }

        #endregion

        #endregion

        #region Implementation of ILayoutEmulationServices

        public void StartEmulation() {
            lock (Sync) {
                if (operationNesting == 0 && emulateTrainMotion) {
                    emulationTimer = new Timer(new TimerCallback(this.OnEmulationTick), null, 0, tickTime);
                    operationNesting++;
                }
            }
        }

        public void StopEmulation() {
            lock (Sync) {
                if (--operationNesting == 0) {
                    emulationTimer?.Dispose();
                    emulationTimer = null;
                }
            }
        }

        public void SetLocomotiveSpeed(Guid commandStationId, int unit, int speed) {
            lock (Sync) {
                var locomotive = GetLocomotive(commandStationId, unit);

                if (locomotive != null)
                    locomotive.Speed = speed;
            }
        }

        public void SetLocomotiveDirection(Guid commandStationId, int unit, LocomotiveOrientation direction) {
            lock (Sync) {
                var locomotive = GetLocomotive(commandStationId, unit);

                if (locomotive != null)
                    locomotive.Direction = direction;
            }
        }

        public void ToggleLocomotiveDirection(Guid commandStationId, int unit) {
            lock (Sync) {
                var locomotive = GetLocomotive(commandStationId, unit);

                if (locomotive != null)
                    locomotive.Direction = (locomotive.Direction == LocomotiveOrientation.Forward) ? LocomotiveOrientation.Backward : LocomotiveOrientation.Forward;
            }
        }

        private CommandStationObjects GetCommandStationObjects(Guid commandStationId) {
            commandStations.TryGetValue(commandStationId, out CommandStationObjects? commandStationObjects);

            if (commandStationObjects == null) {
                commandStationObjects = new CommandStationObjects(this, commandStationId);

                commandStations[commandStationId] = commandStationObjects;
            }

            return commandStationObjects;
        }

        public void SetTurnoutState(Guid commandStationId, int unit, int switchState) {
            lock (Sync) {
                CommandStationObjects commandStationObjects = GetCommandStationObjects(commandStationId);

                commandStationObjects.SetTurnoutSwitchState(unit, switchState);
            }
        }

        public ILocomotiveLocation GetLocomotiveLocation(Guid commandStationId, int unit) {
            lock (Sync) {
                var locomotive = GetLocomotive(commandStationId, unit);

                if (locomotive == null) {
                    var commandStation = LayoutModel.Component<IModelComponentIsCommandStation>(commandStationId, LayoutModel.ActivePhases);
                    var commandStationName = commandStation != null ? commandStation.Name : "**UNDEFINED COMMAND STATION**";

                    throw new ArgumentException("GetLocomotiveLocation of undefined locomotive (commandStation: " + commandStationName + ", unit: " + unit + ")");
                }

                return locomotive.Location;
            }
        }

        public IList<ILocomotiveLocation> GetLocomotiveLocations(Guid commandStationId) {
            lock (Sync) {
                commandStations.TryGetValue(commandStationId, out CommandStationObjects? commandStationObjects);

                return commandStationObjects == null
                    ? Array.AsReadOnly<ILocomotiveLocation>(Array.Empty<LocomotiveLocation>())
                    : commandStationObjects.GetLocomotiveLocations();
            }
        }

        public event EventHandler<LocomotiveMovedEventArgs>? LocomotiveMoved;
        public event EventHandler<LocomotiveMovedEventArgs>? LocomotiveFallFromTrack;

        #endregion

        #region Helper methods

        public LocomotiveState AddLocomotive(Guid commandStationId, int unit, TrackEdge location) {
            CommandStationObjects commandStationObjects = GetCommandStationObjects(commandStationId);

            return commandStationObjects.AddLocomotive(unit, location);
        }

        public LocomotiveState? GetLocomotive(Guid commandStationId, int unit) {
            LocomotiveState? locomotive = null;

            var commandStationObjects = commandStations[commandStationId];

            if (commandStationObjects != null)
                locomotive = commandStationObjects.GetLocomotive(unit);

            return locomotive;
        }

        private void RemoveLocomotive(Guid commandStationId, int unit) {
            if (commandStations.TryGetValue(commandStationId, out CommandStationObjects? commandStationObjects))
                commandStationObjects.RemoveLocomotive(unit);
        }

        private int GetTurnoutSwitchState(Guid commandStationId, int unit) {
            var commandStationObjects = commandStations[commandStationId];

            return commandStationObjects != null ? commandStationObjects.GetTurnoutSwitchState(unit) : -1;
        }

        private void ResetEmulator() {
            lock (Sync) {
                commandStations.Clear();

                foreach (XmlElement trainStateElement in LayoutModel.StateManager.Trains.Element) {
                    var train = new TrainStateInfo(trainStateElement);

                    if (train.CommandStation != null) {
                        Guid commandStationId = train.CommandStation.Id;

                        foreach (TrainLocomotiveInfo trainLocomotive in train.Locomotives) {
                            var loco = trainLocomotive.Locomotive;
                            var locoLocation = train.LocomotiveLocation;
                            var locoBlock = train.LocomotiveBlock;

                            if (loco != null && locoLocation != null && locoBlock != null) {
                                LayoutTrackComponent locoTrack = locoLocation.Block.BlockDefinintion.Track;
                                ILayoutPower locoPower = locoTrack.GetPower(locoTrack.ConnectionPoints[0]);

                                LocomotiveState locomotive = AddLocomotive(commandStationId, loco.AddressProvider.Unit,
                                    new TrackEdge(locoBlock.BlockDefinintion.Track, locoLocation.DisplayFront));

                                locomotive.Direction = train.MotionDirection;
                                if (trainLocomotive.Orientation == LocomotiveOrientation.Backward)
                                    locomotive.Direction = locomotive.Direction == LocomotiveOrientation.Forward ? LocomotiveOrientation.Backward :
                                        LocomotiveOrientation.Forward;
                            }
                            else
                                Warning($"A previously used locomotive is no longer defined");
                        }
                    }
                }

                foreach (IModelComponentIsMultiPath turnout in LayoutModel.Components<IModelComponentIsMultiPath>(LayoutModel.ActivePhases)) {
                    if (LayoutModel.StateManager.Components.Contains(turnout.Id, "SwitchState")) {
                        var connectionPoint = Ensure.NotNull<ControlConnectionPoint>(LayoutModel.ControlManager.ConnectionPoints[turnout]?[0], "turnout[0]");
                        int address = connectionPoint.Module.Address + connectionPoint.Index;

                        SetTurnoutState(connectionPoint.Module.Bus.BusProviderId, address, turnout.CurrentSwitchState);
                    }
                }
            }
        }

        private void OnEmulationTick(object? state) {
            lock (Sync) {
                foreach (CommandStationObjects commandStationObjects in commandStations.Values)
                    commandStationObjects.Tick();
            }
        }

        #endregion

        #region External Operations

        [DispatchTarget]
        private void InitializeLayoutEmulation(bool emulateTrainMotion, int emulationTickTime) {
            this.emulateTrainMotion = emulateTrainMotion;
            if (emulateTrainMotion)
                tickTime = emulationTickTime;
        }

        [DispatchTarget]
        private ILayoutEmulatorServices GetLayoutEmulationServices() => this;

        [DispatchTarget]
        private void ResetLayoutEmulation() {
            ResetEmulator();
        }

        #endregion

        #region Trapped events

        [DispatchTarget]
        private void OnTrainCreated(TrainStateInfo train, LayoutBlockDefinitionComponent blockDefinition) {

            lock (Sync) {
                if (train.CommandStation != null) {
                    Guid commandStationId = train.CommandStation.Id;

                    foreach (TrainLocomotiveInfo trainLocomotive in train.Locomotives) {
                        var loco = trainLocomotive.Locomotive;
                        var locoBlock = train.LocomotiveBlock;
                        var locoLocation = train.LocomotiveLocation;

                        if (locoBlock == null || locoLocation == null)
                            throw new LayoutException($"Cannot locate locomotive for train {train.Name}");

                        AddLocomotive(commandStationId, loco.AddressProvider.Unit, new TrackEdge(locoBlock.BlockDefinintion.Track,
                            locoLocation.DisplayFront));
                    }
                }
            }
        }

        [DispatchTarget]
        private void OnTrainIsRemoved(TrainStateInfo train) {
            lock (Sync) {
                if (train.CommandStation != null) {
                    var commandStationId = train.CommandStation.Id;

                    foreach (TrainLocomotiveInfo trainLocomotive in train.Locomotives) {
                        LocomotiveInfo loco = trainLocomotive.Locomotive;

                        RemoveLocomotive(commandStationId, loco.AddressProvider.Unit);
                    }
                }
            }
        }

        [DispatchTarget]
        private void OnTrainRelocated(TrainStateInfo train, LayoutBlockDefinitionComponent blockDefinition) {
            OnTrainIsRemoved(train);
            OnTrainCreated(train, blockDefinition);
        }

        #endregion
    }

    #region Data structures

    internal class CommandStationObjects {
        private readonly ILayoutEmulationEnvironment environment;
        private readonly Guid commandStationId;
        private readonly Dictionary<int, LocomotiveState> locomotives = new Dictionary<int, LocomotiveState>();
        private readonly Dictionary<int, int> turnouts = new Dictionary<int, int>();

        public CommandStationObjects(ILayoutEmulationEnvironment environment, Guid commandStationId) {
            this.environment = environment;
            this.commandStationId = commandStationId;
        }

        public LocomotiveState AddLocomotive(int unit, TrackEdge location) {
            locomotives.TryGetValue(unit, out LocomotiveState? locomotive);

            if (locomotive == null) {
                locomotive = new LocomotiveState(environment, commandStationId, unit, location);
                locomotives.Add(unit, locomotive);
            }
            else
                locomotive.Location = new LocomotiveLocation(location);

            return locomotive;
        }

        public LocomotiveState? GetLocomotive(int unit) {
            return !locomotives.TryGetValue(unit, out LocomotiveState? loco) ? null : loco;
        }

        public void RemoveLocomotive(int unit) {
            locomotives.Remove(unit);
        }

        public IList<ILocomotiveLocation> GetLocomotiveLocations() {
            LocomotiveLocation[] locations = new LocomotiveLocation[locomotives.Count];

            int i = 0;
            foreach (LocomotiveState locomotive in locomotives.Values)
                locations[i++] = locomotive.Location;

            return Array.AsReadOnly<ILocomotiveLocation>(locations);
        }

        public void SetTurnoutSwitchState(int unit, int switchState) {
            turnouts[unit] = switchState;
        }

        public int GetTurnoutSwitchState(int unit) {
            return turnouts.TryGetValue(unit, out int switchState) ? switchState : -1;
        }

        // Apply emulation "tick" on each locomotive
        public void Tick() {
            foreach (LocomotiveState locomotive in locomotives.Values)
                locomotive.Tick();
        }
    }

    public class LocomotiveLocation : ILocomotiveLocation {
        public LocomotiveLocation(LayoutTrackComponent track, LayoutComponentConnectionPoint front, LayoutComponentConnectionPoint rear) {
            Track = track;
            Front = front;
            Rear = rear;
        }

        public LocomotiveLocation(TrackEdge edge) {
            Track = edge.Track;
            Front = edge.ConnectionPoint;
            Rear = Track.ConnectTo(Front, LayoutComponentConnectionType.Passage)[0];
        }

        public LayoutTrackComponent Track { get; }

        public LayoutComponentConnectionPoint Front { get; }

        public LayoutComponentConnectionPoint Rear { get; }

        public TrackEdge Edge => new TrackEdge(Track, Front);
    }

    public class LocomotiveState {
        private readonly ILayoutEmulationEnvironment _environment;
        private readonly Guid _commandStationId;
        private int _speed;             // the locomotive current speed (and direction)
        private LocomotiveLocation _location;           // Where the locomotive is on the layout (when there is support for longer trains, this will become an array)
                                                        //		LayoutComponentConnectionPoint	rear;				// Where is the rear of the train
        private LocomotiveOrientation _direction = LocomotiveOrientation.Forward;

        private int ticksPerMotion;     // Number of timer ticks between each locomotive motion
        private int tickCount;          // Number of ticks remaining until the next locomotive motion

        public LocomotiveState(ILayoutEmulationEnvironment environment, Guid commandStationId, int unit, TrackEdge edge) {
            this._environment = environment;
            this._commandStationId = commandStationId;
            this.Unit = unit;

            _location = new LocomotiveLocation(edge);
            Speed = 0;
        }

        public Guid CommandStationId => _commandStationId;

        public int Unit { get; }

        public int Speed {
            get {
                return _speed;
            }

            set {
                lock (_environment.Sync) {
                    int previousSpeed = this._speed;

                    this._speed = value;

                    if (_speed == 0) {
                        ticksPerMotion = 0;
                        tickCount = 0;
                    }
                    else {
                        ticksPerMotion = (_environment.SpeedSteps - Math.Abs(_speed)) * _environment.TicksPerSpeedStep;

                        if (tickCount > ticksPerMotion)
                            tickCount = ticksPerMotion;
                    }

                    if (previousSpeed == 0 && _speed != 0)
                        _environment.FireLocomotiveMoved(this, _direction, _speed);
                }
            }
        }

        public LocomotiveOrientation Direction {
            get {
                return _direction;
            }

            set {
                lock (_environment.Sync) {
                    _direction = value;
                }
            }
        }

        public LocomotiveLocation Location {
            get {
                return _location;
            }

            set {
                lock (_environment.Sync) {
                    _location = value;
                    Debug.Assert(_location.Track is LayoutStraightTrackComponent);
                    //					rear = location.Track.ConnectTo(location.ConnectionPoint, LayoutComponentConnectionType.Passage)[0];
                }
            }
        }

        public void Tick() {
            if (ticksPerMotion > 0) {
                if (--tickCount <= 0) {
                    TrackEdge nextEdge;

                    // Locomotive needs to move
                    if (_direction == LocomotiveOrientation.Forward)
                        nextEdge = _environment.TopologyServices.FindTrackConnectingAt(_location.Edge);
                    else
                        nextEdge = _environment.TopologyServices.FindTrackConnectingAt(new TrackEdge(_location.Track, _location.Rear));

                    if (nextEdge == TrackEdge.Empty) {
                        _environment.FireLocomotiveFallFromTrack(this, _direction, _speed);
                        Speed = 0;
                    }
                    else {
                        LayoutComponentConnectionPoint cp;

                        if (nextEdge.Track is IModelComponentIsMultiPath turnout) {
                            if (turnout.IsSplitPoint(nextEdge.ConnectionPoint))
                                cp = _environment.FindRouteThroughMultiPathComponent(turnout, nextEdge.ConnectionPoint);
                            else
                                cp = nextEdge.Track.ConnectTo(nextEdge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0];
                        }
                        else
                            cp = nextEdge.Track.ConnectTo(nextEdge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0];

                        if (Direction == LocomotiveOrientation.Forward)
                            _location = new LocomotiveLocation(nextEdge.Track, cp, nextEdge.ConnectionPoint);
                        else
                            _location = new LocomotiveLocation(nextEdge.Track, nextEdge.ConnectionPoint, cp);

                        _environment.FireLocomotiveMoved(this, _direction, _speed);
                    }

                    tickCount = ticksPerMotion;
                }
            }
        }
    }

    #endregion

}
