using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

using LayoutManager.Model;
using LayoutManager.Components;

#nullable enable
#pragma warning disable IDE0051
namespace LayoutManager.Logic {

    [LayoutModule("Layout Power Manager", UserControl = false)]
    class LayoutPowerManager : LayoutModuleBase {
        static ILayoutTopologyServices _topologyServices;
        readonly Dictionary<Guid, Guid> _pendingPowerConnectedLockReady = new Dictionary<Guid, Guid>();

        static ILayoutTopologyServices TopologyServices {
            get {
                if (_topologyServices == null)
                    _topologyServices = (ILayoutTopologyServices)EventManager.Event(new LayoutEvent("get-topology-services", sender: null))!;
                return _topologyServices;
            }
        }

        /// <summary>
        /// Event that indicates that a power source state was changed. Check if this power source is connected to any track power connector.
        /// If it does, propagate the new power through the tracks.
        /// </summary>
        /// <param name="e"></param>
        [LayoutEvent("power-outlet-changed-state-notification")]
        void PowerOutletChangedState(LayoutEvent e0) {
            var e = (LayoutEvent<ILayoutPowerOutlet, ILayoutPower>)e0;

            Debug.Assert(e.Sender != null && e.Info != null);

            // Redraw all tracks
            foreach (LayoutTrackComponent track in LayoutModel.AllComponents<LayoutTrackComponent>(LayoutModel.ActivePhases))
                track.Redraw();

            var outletComponentId = e.Sender.OutletComponent.Id;

            // If this outlet was connected to power connector which needs to notify that is can be locked.
            if (_pendingPowerConnectedLockReady.ContainsKey(outletComponentId) && e.Info.Type != LayoutPowerType.Disconnected) {
                var component = LayoutModel.Component<ILayoutLockResource>(_pendingPowerConnectedLockReady[outletComponentId]);

                if (component != null)
                    EventManager.Event(new LayoutEvent("layout-lock-resource-ready", component));
                _pendingPowerConnectedLockReady.Remove(outletComponentId);
            }
        }

        [LayoutEvent("register-power-connected-lock")]
        private void registerPowerConnectedLock(LayoutEvent e0) {
            var e = (LayoutEvent<LayoutTrackPowerConnectorComponent>)e0;
            var outletComponent = e.Sender?.Info?.Inlet?.ConnectedOutlet?.OutletComponent;

            Debug.Assert(outletComponent != null);

            if (outletComponent != null) {
                var id = outletComponent.Id;

                if (!_pendingPowerConnectedLockReady.ContainsKey(id))
                    _pendingPowerConnectedLockReady.Add(id, e.Sender!.Id);
            }
        }

        [LayoutEvent("check-layout", Order = 101)]
        void CheckForReverseLoops(LayoutEvent e) {
            LayoutPhase phase = e.GetPhases();

            if (checkForReverseLoopsForPowerSection(phase))
                e.Info = false;
        }

        struct ReverseLoopStackEntry : IComparable<ReverseLoopStackEntry> {
            public TrackEdge Edge { get; }
            public bool FromSplit { get; }

            public ReverseLoopStackEntry(TrackEdge edge, bool forward) : this() {
                this.Edge = edge;
                this.FromSplit = forward;
            }

            #region IComparable<ReverseLoopStackEntry> Members

            public int CompareTo(ReverseLoopStackEntry other) {
                int i = Edge.CompareTo(other.Edge);

                if (i == 0) {
                    if (FromSplit == other.FromSplit)
                        return 0;
                    else
                        return FromSplit ? 1 : -1;
                }
                else
                    return i;
            }

            #endregion
        }

        class ReverseLoopScanStack : Stack<ReverseLoopStackEntry> {

            public void Push(TrackEdge splitEdge, bool fromSplit) {
                TrackEdge tipEdge;

                if (splitEdge.Split.IsSplitPoint(splitEdge.ConnectionPoint))
                    tipEdge = splitEdge;
                else {
                    tipEdge = new TrackEdge(splitEdge.Track, splitEdge.Split.ConnectTo(splitEdge.ConnectionPoint, LayoutComponentConnectionType.ReverseLoop)[0]);
                    fromSplit = !fromSplit;
                }

                foreach (TrackEdge nextEdge in TopologyServices.FindConnectingTracks(tipEdge, LayoutComponentConnectionType.ReverseLoop))
                    Push(new ReverseLoopStackEntry(nextEdge, fromSplit));
            }

            public void Push(IModelComponentIsMultiPath split, bool fromSplit = true) {
                foreach (var cp in split.ConnectionPoints.Where(p => split.IsSplitPoint(p)))
                    Push(new TrackEdge(split, cp), fromSplit);
            }

        }

        private bool checkForReverseLoopsForPowerSection(LayoutPhase phase) {
            var scanStack = new ReverseLoopScanStack();

            foreach (var split in LayoutModel.Components<IModelComponentIsMultiPath>(phase)) {
                // If this split was not scanned, scan from it
                var powerConnector = ((LayoutTrackComponent)split).GetPowerConnector(split.ConnectionPoints[0]);
                var splits = new HashSet<IModelComponentIsMultiPath>();

                if (powerConnector == null || powerConnector.Info.CheckReverseLoops) {
                    scanStack.Push(split);
                    splits.Add(split);
                }

                while (scanStack.Any()) {
                    var s = scanStack.Pop();
                    var edge = s.Edge;
                    var fromSplit = s.FromSplit;

                    if (edge.Track is IModelComponentIsMultiPath splitTrack) {
                        bool isSplitPoint = splitTrack.IsSplitPoint(edge.ConnectionPoint);

                        if (splitTrack == split) {
                            if ((fromSplit && !isSplitPoint) || (!fromSplit && isSplitPoint)) {
                                Error(edge.Track, "Power reverse loop detected. This will cause short-circuit, you must resolve this before power can be connected");
                                return true;
                            }
                        }
                        else {
                            if (!splits.Contains(splitTrack)) {
                                scanStack.Push(edge, fromSplit);
                                splits.Add(splitTrack);
                            }
                        }
                    }
                    else if (edge.Track.Spot[ModelComponentKind.TrackIsolation] == null)
                        foreach (TrackEdge nextEdge in TopologyServices.FindConnectingTracks(edge, LayoutComponentConnectionType.ReverseLoop))
                            scanStack.Push(new ReverseLoopStackEntry(nextEdge, fromSplit));
                }
            }

            return false;
        }

        [LayoutEvent("add-power-connector-as-resource")]
        private void addPowerConnectorAsResource(LayoutEvent e0) {
            var e = (LayoutEvent<LayoutTrackPowerConnectorComponent, Action<LayoutBlockDefinitionComponent>>)e0;
            var powerConnector = e.Sender;

            Debug.Assert(powerConnector != null);

            if (powerConnector != null && powerConnector.Inlet.IsConnected && powerConnector.Inlet.ConnectedOutlet.ObtainablePowers.Count() > 1) {
                foreach (var blockDefinition in powerConnector.BlockDefinitions) {
                    if (!blockDefinition.Info.Resources.Any(resourceInfo => resourceInfo.ResourceId == powerConnector.Id)) {
                        // This block does not have this power connector as a resource
                        Trace.WriteLine($"Will add {powerConnector.FullDescription} as resource to {blockDefinition.FullDescription}");
                        Debug.Assert(e.Info != null);
                        e.Info?.Invoke(blockDefinition);
                    }
                }
            }
        }

        [LayoutEvent("check-layout", Order = 100)]
        void AssignPowerSource(LayoutEvent e) {
            LayoutPhase phase = e.GetPhases();
            IEnumerable<LayoutTrackPowerConnectorComponent> powerConnectors = LayoutModel.Components<LayoutTrackPowerConnectorComponent>(phase);
            LayoutSelection duplicatedPowerConnectors = new LayoutSelection();

            if (!powerConnectors.Any()) {
                Error("No power-source (e.g. command station) is connected to the tracks - use track power connector component");
                e.Info = false;
            }
            else {
                resetPowerConnectors();

                foreach (LayoutTrackPowerConnectorComponent powerConnector in powerConnectors) {
                    LayoutTrackComponent track = powerConnector.Spot.Track;

                    if (track == null) {
                        Error(powerConnector, "power connector is not on track component");
                        e.Info = false;
                    }
                    else {
                        if (track.GetPowerConnector(track.ConnectionPoints[0]) != null)
                            duplicatedPowerConnectors.Add(powerConnector);
                        else {
                            if (powerConnector.Inlet.OutletComponentId == Guid.Empty) {
                                Error(powerConnector, "This power connector is not assigned to any power source");
                                e.Info = false;
                            }
                            else if (powerConnector.Inlet.GetOutletComponent(phase) == null) {
                                if (powerConnector.Inlet.GetOutletComponent(LayoutPhase.All) == null)
                                    Error(powerConnector, "The component supplying power to this power connector cannot be found");
                                else
                                    Error(powerConnector, "The component supplying power to this power connector is marked as 'Planned' or 'In construction'");
                                e.Info = false;
                            }
                            else
                                propagatePowerConnector(powerConnector);
                        }
                    }
                }

                if (duplicatedPowerConnectors.Count > 0) {
                    Error(duplicatedPowerConnectors, "Duplicate track power connector - use track isolation component to separate the circuits");
                    e.Info = false;
                }

                LayoutSelection tracksWithNoPower = new LayoutSelection();

                foreach (LayoutModelArea area in LayoutModel.Areas) {
                    foreach (LayoutModelSpotComponentCollection spot in area.Grid.Values) {
                        if ((spot.Phase & phase) != 0) {
                            LayoutTrackComponent track = spot.Track;

                            if (track != null) {
                                foreach (LayoutComponentConnectionPoint cp in track.ConnectionPoints) {
                                    if (track.GetPowerConnector(cp) == null && track.Spot[ModelComponentKind.TrackLink] == null)
                                        tracksWithNoPower.Add(track);
                                }
                            }
                        }
                    }
                }

                if (tracksWithNoPower.Count > 0) {
                    Error(tracksWithNoPower, "Those tracks are not powered - use power connector component to supply power");
                    e.Info = false;
                }
                else if ((bool)e.Info) {
                    // Check that programming power can be obtained for all blocks that are marked as Suggest for programming
                    var invalidProgrammingBlocks = new LayoutSelection(from blockDefinition in LayoutModel.Components<LayoutBlockDefinitionComponent>(phase)
                                                                       where blockDefinition.Info.SuggestForProgramming &&
                                                                       !blockDefinition.Track.GetPowerConnector(blockDefinition.Track.ConnectionPoints[0])!.Inlet.ConnectedOutlet.ObtainablePowers.Any(p => p.Type == LayoutPowerType.Programmer)
                                                                       select blockDefinition);

                    if (invalidProgrammingBlocks.Any()) {
                        Error(invalidProgrammingBlocks,
                            invalidProgrammingBlocks.Count == 1 ?
                            "This block is suggested for programming, however it cannot receive track programming power" :
                            "These blocks are suggested for programming, however they cannot receive track programming power");
                        e.Info = false;
                    }
                }
            }
        }

        [LayoutEvent("check-layout", Order = 102)]
        private void checkPowerSelectors(LayoutEvent e) {
            LayoutPhase phase = e.GetPhases();

            var powerSelectorComponents = LayoutModel.Components<LayoutPowerSelectorComponent>(phase);

            foreach (var powerSelector in powerSelectorComponents) {
                if (!checkInlet(powerSelector, powerSelector.Inlet1, "Inlet 1"))
                    e.Info = false;
                if (!checkInlet(powerSelector, powerSelector.Inlet2, "Inlet 2"))
                    e.Info = false;
            }
        }

        private bool checkInlet(LayoutPowerSelectorComponent powerSelector, ILayoutPowerInlet inlet, string inletName) {
            if (inlet.OutletComponentId != Guid.Empty && inlet.OutletComponent == null) {
                if (LayoutModel.Component<IModelComponentHasPowerOutlets>(inlet.OutletComponentId, LayoutPhase.All) == null) {
                    Error(powerSelector, "Inlet " + inletName + " was connected to component that is no longer on the layout");
                    inlet.OutletComponentId = Guid.Empty;
                }
                else
                    Error(powerSelector, "Inlet " + inletName + " is connected to component that is 'Planned' or 'In construction'");

                return false;
            }
            else
                return true;
        }

        private static void resetPowerConnectors() {
            foreach (LayoutModelArea area in LayoutModel.Areas) {
                foreach (LayoutModelSpotComponentCollection spot in area.Grid.Values) {
                    LayoutTrackComponent track = spot.Track;

                    if (track != null) {
                        if (track.Spot[ModelComponentKind.TrackLink] == null) {
                            foreach (LayoutComponentConnectionPoint cp in track.ConnectionPoints)
                                track.SetPowerConnector(cp, null);
                        }
                    }
                }
            }
        }

        private void propagatePowerConnector(LayoutTrackPowerConnectorComponent powerConnector) {
            Stack<TrackEdge> scanStack = new Stack<TrackEdge>(10);
            LayoutTrackComponent track = powerConnector.Spot.Track;
            TrackEdgeDictionary scannedEdges = new TrackEdgeDictionary();

            foreach (LayoutComponentConnectionPoint cp in track.ConnectionPoints) {
                scanStack.Push(new TrackEdge(track, cp));

                foreach (TrackEdge edge in TopologyServices.FindConnectingTracks(new TrackEdge(track, cp), LayoutComponentConnectionType.Electrical))
                    scanStack.Push(edge);
            }

            while (scanStack.Count > 0) {
                TrackEdge edge = scanStack.Pop();

                if (!scannedEdges.ContainsKey(edge)) {
                    scannedEdges.Add(edge, null);

                    edge.Track.SetPowerConnector(edge.ConnectionPoint, powerConnector);
                    edge.Track.Redraw();

                    foreach (TrackEdge nextEdge in TopologyServices.FindConnectingTracks(edge, LayoutComponentConnectionType.Electrical))
                        scanStack.Push(nextEdge);
                }
            }
        }

        private LayoutTrackPowerConnectorComponent ExtractPowerConnector(object powerRegionObject) {
            var powerConnector = powerRegionObject switch
            {
                TrainStateInfo train => train?.LocomotiveBlock?.BlockDefinintion.PowerConnector,
                LayoutBlockDefinitionComponent blockDefinition => blockDefinition.PowerConnector,
                LayoutTrackPowerConnectorComponent connector => connector,
                _ => throw new ArgumentException("Invalid power region")
            };

            Debug.Assert(powerConnector != null);
            return powerConnector;
        }

        [LayoutEventDef("train-power-changed", Role = LayoutEventRole.Notification, SenderType = typeof(TrainStateInfo), InfoType = typeof(ILayoutPower))]
        [LayoutEventDef("locomotive-power-changed", Role = LayoutEventRole.Notification, SenderType = typeof(TrainLocomotiveInfo), InfoType = typeof(ILayoutPower))]
        void OnTrainPowerChanged(TrainStateInfo train, ILayoutPower power) {
            EventManager.Event(new LayoutEvent<TrainStateInfo, ILayoutPower>("train-power-changed", train, power));

            foreach (var trainLoco in train.Locomotives)
                EventManager.Event(new LayoutEvent<TrainLocomotiveInfo, ILayoutPower>("locomotive-power-changed", trainLoco, power));

        }

        [LayoutEvent("change-train-power")]
        private void changeTrainPower(LayoutEvent e0) {
            var e = (LayoutEvent<TrainStateInfo, ILayoutPower>)e0;

            Debug.Assert(e.Sender != null && e.Info != null);
            if(e.Sender != null && e.Info != null)
                OnTrainPowerChanged(e.Sender, e.Info);
        }

        [LayoutAsyncEvent("set-power")]
        private async Task setPower(LayoutEvent e) {
            Debug.Assert(e.Sender != null);
            LayoutTrackPowerConnectorComponent powerConnector = ExtractPowerConnector(e.Sender);
            ILayoutPower power;

            if (e.Info is ILayoutPower)
                power = (ILayoutPower)e.Info;
            else if (e.Info is LayoutPowerType)
                power = (from p in powerConnector.Inlet.ConnectedOutlet.ObtainablePowers where p.Type == (LayoutPowerType)e.Info select p).FirstOrDefault();
            else
                throw new ArgumentException("Invalid power for set-power");

            bool isPossible = powerConnector.Inlet.ConnectedOutlet.ObtainablePowers.Any(p => p.Type == power.Type);

            if (!isPossible)
                throw new LayoutException(e.Sender, power.Type == LayoutPowerType.Disconnected ? "It is not possible to turn power off to this region" : "It is not possible to assign this power type to that region");

            if (powerConnector.Inlet.ConnectedOutlet.Power.Type != power.Type) {

                // Validate that this type of power can be connected to that region
                switch (power.Type) {
                    case LayoutPowerType.Disconnected:
                        foreach (var trainLocation in powerConnector.TrainLocations) {
                            if (trainLocation.Train.Trip != null)
                                throw new LayoutException(trainLocation.Train, "train is currently in active trip, it cannot be disconnected from power");

                            if (trainLocation.Train.Speed != 0)
                                throw new LayoutException(trainLocation.Train, "train is currently moving, it cannot be disconnected from power");
                        }

                        var lockedBlockDefinitions = from b in powerConnector.Blocks where b.LockRequest != null && !b.LockRequest.IsManualDispatchLock && !b.HasTrains select b.BlockDefinintion;

                        if (lockedBlockDefinitions.Any())
                            throw new LayoutException(lockedBlockDefinitions, "some blocks are locked to trains, section cannot be disconnected from power");

                        break;

                    case LayoutPowerType.Digital:
                        // Make sure that all locomotives that are placed on the section about to get powered can be safely powered (without address
                        // collision).
                        foreach (var trainLocation in powerConnector.TrainLocations) {
                            foreach (var trainLoco in trainLocation.Train.Locomotives) {
                                var result = (CanPlaceTrainResult?)EventManager.Event(new LayoutEvent<XmlElement, object>("is-locomotive-address-valid", trainLoco.Locomotive.Element, power));

                                if (result != null) {
                                    if (result.ResolveMethod == CanPlaceTrainResolveMethod.NotPossible)
                                        throw new LayoutException(trainLocation.Train, "Cannot connect power to this train: " + result.ToString());
                                    else if (result.ResolveMethod == CanPlaceTrainResolveMethod.ReprogramAddress) {
                                        // This can be done in place if there is only one locomotive in the power region, and this power region
                                        // can be connected to programming power
                                        if (!powerConnector.Inlet.ConnectedOutlet.ObtainablePowers.Any(p => p.Type == LayoutPowerType.Programmer))
                                            throw new LayoutException(trainLocation.Train, "Locomotive has invalid address, cannot connect it to power without first assigning it with a new address");

                                        if (powerConnector.Locomotives.Count() > 1)
                                            throw new LayoutException(trainLocation.Train, "Train locomotive has invalid address, a new address cannot reprogrammed since there are other locomotives in the power region");

                                        // There is only one locomotive on a region that can be connected to programming power
                                        var programmingState = new PlacedLocomotiveProgrammingState(trainLocation.Train.Element, result.Locomotive, trainLocation.Block.BlockDefinintion);

                                        var programmedTrain = (TrainStateInfo)await (Task<object>)EventManager.AsyncEvent(new LayoutEvent<PlacedLocomotiveProgrammingState>("placed-locomotive-address-programming", programmingState).CopyOperationContext(e));

                                        Debug.Assert(programmedTrain != null);
                                    }
                                }
                                else
                                    throw new LayoutException(trainLocation.Train, $"Unable to determine if locomotive {trainLoco.Name} has valid address");
                            }
                        }
                        break;

                    case LayoutPowerType.Programmer:
                        break;
                }

                // If got to here, everything is valid, connect power
                var switchingCommands = new List<SwitchingCommand>();

                powerConnector.Inlet.ConnectedOutlet.SelectPower(power.Type, switchingCommands);
                await EventManager.AsyncEvent(new LayoutEvent<object, List<SwitchingCommand>>("set-track-components-state", sender: null, info: switchingCommands));

                foreach (var train in powerConnector.Trains)
                    OnTrainPowerChanged(train, power);
            }
        }

        [LayoutEvent("command-station-power-on-notification")]
        private void onCommandStationPowerOn(LayoutEvent e0) {
            var e = (LayoutEvent<IModelComponentIsCommandStation>)e0;
            var commandStation = e.Sender;

            if (LayoutController.IsOperationMode) {
                foreach (var powerConnector in (from p in LayoutModel.Components<LayoutTrackPowerConnectorComponent>(LayoutModel.ActivePhases) where p.Inlet.ConnectedOutlet.Power.PowerOriginComponentId == commandStation.Id select p))
                    foreach (var train in powerConnector.Trains)
                        OnTrainPowerChanged(train, powerConnector.Inlet.ConnectedOutlet.Power);
            }
        }

        [LayoutEvent("train-placed-on-track")]
        private void onTrainPlaced(LayoutEvent e0) {
            var e = (LayoutEvent<TrainStateInfo>)e0;
            var train = e.Sender;

            Debug.Assert(train != null);
            if(train != null && train.LocomotiveBlock != null)
                OnTrainPowerChanged(train, train.LocomotiveBlock.BlockDefinintion.Power);
        }

        [LayoutEvent("train-is-removed")]
        private void onCanRemoveTrain(LayoutEvent e0) {
            var e = (LayoutEvent<TrainStateInfo>)e0;
            var train = e.Sender;
            var power = train?.LocomotiveBlock?.BlockDefinintion.Power;

            Debug.Assert(train != null && power != null);
            if (power != null) {
                var disconnectedPower = new LayoutPower(power.PowerOriginComponent, LayoutPowerType.Disconnected, power.DigitalFormats, "Disconnected");

                OnTrainPowerChanged(train, disconnectedPower);
            }
        }
    }
}
