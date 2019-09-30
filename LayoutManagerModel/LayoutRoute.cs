using System;
using System.Collections.Generic;
using LayoutManager.Components;

#nullable enable
namespace LayoutManager.Model {
    /// <summary>
    /// Route Clearance quality are numerically ordered from the best to the worst
    /// </summary>
    public enum RouteClearanceQuality {
        /// <summary>
        /// Route is currently free
        /// </summary>
        FreeNow,

        /// <summary>
        /// Route will soon be free, (it has a lock that is going to be released, and
        /// no one has a pending lock on the route
        /// </summary>
        WillBeFree,

        /// <summary>
        /// RouteQuality which have numeric value less than this one indicate that the route is free
        /// entries which have numeric value larger than this, indicates the the route is blocked
        /// </summary>
        Free,               //******************

        /// <summary>
        /// Route is free from train, still have locks now, but it has a pending lock, that is
        /// it will be locked again
        /// </summary>
        WillBeLocked,

        /// <summary>
        /// Route is blocked by a train that is moving
        /// </summary>
        BlockedByMovingTrain,

        /// <summary>
        /// Route is blocked by a train that is not moving
        /// </summary>
        BlockedByStandingTrain,

        /// <summary>
        /// Any route with clearance quality less than this is valid (it may be blocked,
        /// but it is valid)
        /// </summary>
        ValidRoute,

        /// <summary>
        /// Selecting this path will cause this train to block other trains when it will
        /// arrive
        /// </summary>
        DestinationWillBlockOtherTrains,

        /// <summary>
        /// Route crosses a region which is a manual dispatch region
        /// </summary>
        CrossManualDispatchRegion,

        /// <summary>
        /// Blocked by a region in which a block occupancy detects something, but the train
        /// tracking system is not capable to identify by which train
        /// </summary>
        BlockedByUnexpectedTrain,

        /// <summary>
        /// Route is forbidden due to block train pass condition
        /// </summary>
        ForbiddenByBlockPassCondition,

        /// <summary>
        /// Route is forbidden due to block train stop condition
        /// </summary>
        ForbiddenByBlockStopCondition,

        /// <summary>
        /// Route is forbidden by destination condition
        /// </summary>
        ForbiddenByDestinationCondition,

        /// <summary>
        /// Route is forbidden because train is too long for stopping in destination block
        /// </summary>
        ForbiddenByTrainTooLong,

        /// <summary>
        /// Forbidden since train that is reaching that target will not be detected. For example, the block is a non-
        /// occupancy detection block and the block edge is not a sensor.
        /// </summary>
        TrainWillNotBeDetected,

        /// <summary>
        /// No path
        /// </summary>
        NoPath,
    }

    public class RouteQuality : IComparable<RouteQuality>, ICloneable {
        private Guid routeOwner = Guid.Empty;
        private static ILayoutLockManagerServices? _lockManagerServices = null;

        public RouteQuality(Guid routeOwner, RouteClearanceQuality clearanceQuality, int penalty) {
            initialize(routeOwner);
            this.ClearanceQuality = clearanceQuality;
            this.Penalty = penalty;
        }

        public RouteQuality(Guid routeOwner) {
            initialize(routeOwner);
        }

        public RouteQuality(Guid routeOwner, LayoutBlock block) {
            initialize(routeOwner);
            AddQuality(block.BlockDefinintion);
        }

        public RouteQuality(Guid routeOwner, LayoutBlockDefinitionComponent blockInfo) {
            initialize(routeOwner);
            AddQuality(blockInfo);
        }

        public RouteQuality(RouteQuality copyFrom) {
            initialize(copyFrom.RouteOwner);
            ClearanceQuality = copyFrom.ClearanceQuality;
            Penalty = copyFrom.Penalty;
        }

        private void initialize(Guid routeOwner) {
            this.routeOwner = routeOwner;
        }

        public RouteClearanceQuality ClearanceQuality { get; set; } = RouteClearanceQuality.FreeNow;

        public int Penalty { get; set; }

        public static int WillBeFreeClearancePenaltyFactor { get; set; } = 20;

        public Guid RouteOwner => routeOwner;

        protected static ILayoutLockManagerServices LockManagerServices => _lockManagerServices ?? (_lockManagerServices = (ILayoutLockManagerServices)EventManager.Event(new LayoutEvent("get-layout-lock-manager-services"))!);

        public void AddClearanceQuality(RouteClearanceQuality q) {
            if ((int)q > (int)ClearanceQuality)
                ClearanceQuality = q;
        }

        public void AddClearanceQuality(LayoutBlock block) {
            AddClearanceQuality(block.BlockDefinintion);
        }

        public void AddClearanceQuality(LayoutBlockDefinitionComponent blockInfo) {
            AddClearanceQuality(GetBlockClearanceQuality(routeOwner, blockInfo));
        }

        public void AddQuality(LayoutBlock block) {
            AddQuality(block.BlockDefinintion);
        }

        public void AddQuality(LayoutBlockDefinitionComponent blockInfo) {
            AddClearanceQuality(blockInfo);
            this.Penalty += blockInfo.Info.LengthInCM;
        }

        public static RouteClearanceQuality GetBlockClearanceQuality(Guid routeOwner, LayoutBlock block) => GetBlockClearanceQuality(routeOwner, block.BlockDefinintion);

        public static RouteClearanceQuality GetBlockClearanceQuality(Guid routeOwner, LayoutBlockDefinitionComponent blockInfo) {
            LayoutBlock block = blockInfo.Block;
            RouteClearanceQuality q;

            if (blockInfo.Info.UnexpectedTrainDetected)
                q = RouteClearanceQuality.BlockedByUnexpectedTrain;
            else if (block.LockRequest != null && block.LockRequest.OwnerId != routeOwner) {
                if (block.LockRequest.IsManualDispatchLock)
                    q = RouteClearanceQuality.CrossManualDispatchRegion;
                else if (blockInfo.Block.HasTrains) {
                    q = RouteClearanceQuality.BlockedByMovingTrain;

                    foreach (TrainLocationInfo trainLocation in block.Trains)
                        if (trainLocation.Train.Speed == 0) {
                            q = RouteClearanceQuality.BlockedByStandingTrain;
                            break;
                        }
                }
                else if (LockManagerServices.HasPendingLocks(block.Id, routeOwner))
                    q = RouteClearanceQuality.WillBeLocked;
                else
                    q = RouteClearanceQuality.WillBeFree;
            }
            else
                q = RouteClearanceQuality.FreeNow;

            return q;
        }

        public bool IsBetterThan(RouteClearanceQuality q) => (int)this.ClearanceQuality < (int)q;

        public bool IsWorseThen(RouteClearanceQuality q) => (int)this.ClearanceQuality > (int)q;

        public bool IsFree => IsBetterThan(RouteClearanceQuality.Free);

        public bool IsValidRoute => IsBetterThan(RouteClearanceQuality.ValidRoute);

        #region ICloneable Members

        public object Clone() => new RouteQuality(routeOwner, ClearanceQuality, Penalty);

        #endregion

        public override string ToString() => "Clearance: " + ClearanceQuality + " Penalty: " + Penalty;

        #region IComparable<RouteQuality> Members

        public int CompareTo(RouteQuality other) {
            // If are "free" then penalize one that willBeFree by a factor, so a route
            // which is FreeNow will be preferred if even if its penalty is bit larger
            if (IsFree && other.IsFree) {
                int myPenalty = Penalty;
                int qPenalty = other.Penalty;

                if (ClearanceQuality == RouteClearanceQuality.WillBeFree)
                    myPenalty = (100 + WillBeFreeClearancePenaltyFactor) * Penalty / 100;
                if (other.ClearanceQuality == RouteClearanceQuality.WillBeFree)
                    qPenalty = (100 + WillBeFreeClearancePenaltyFactor) * other.Penalty / 100;

                return myPenalty - qPenalty;
            }
            else return ClearanceQuality == other.ClearanceQuality ? Penalty - other.Penalty : (int)ClearanceQuality - (int)other.ClearanceQuality;
        }

        public bool Equals(RouteQuality other) => ClearanceQuality == other.ClearanceQuality && Penalty == other.Penalty && routeOwner == other.routeOwner;

        #endregion
    }

    public class RouteTarget {
        public RouteTarget(TripPlanDestinationEntryInfo? destinationEntryInfo, LayoutTrackComponent destinationTrack, TrackEdge targetEdge, int switchState,
            LayoutComponentConnectionPoint front, LocomotiveOrientation direction, RouteQuality quality) {
            this.DestinationEntryInfo = destinationEntryInfo;
            this.DestinationTrack = destinationTrack;
            this.TargetEdge = targetEdge;
            this.SwitchState = switchState;
            this.Front = front;
            this.Direction = direction;
            this.Quality = new RouteQuality(quality);
        }

        public TripPlanDestinationEntryInfo? DestinationEntryInfo { get; }

        public LayoutTrackComponent DestinationTrack { get; }

        public TrackEdge DestinationEdge => new TrackEdge(DestinationTrack, Front);

        public TrackEdge TargetEdge { get; }

        public TrackEdgeId TargetEdgeId => new TrackEdgeId(TargetEdge);

        public int SwitchState { get; }

        public LayoutComponentConnectionPoint Front { get; }

        public LocomotiveOrientation Direction { get; }

        public RouteQuality Quality { get; }

        public override bool Equals(object obj) {
            return obj is RouteTarget otherTarget && otherTarget.TargetEdge == TargetEdge && otherTarget.SwitchState == SwitchState;
        }

        public override int GetHashCode() => TargetEdge.GetHashCode();

        public bool Arrived { get; set; }
    }

    public class BestRoute : ITripRoute {
        private Guid routeOwner;
        private readonly List<RouteTarget> targets = new List<RouteTarget>();
        private List<int>? switchStates;
        private TrainStateInfo? train;

        public BestRoute(ModelComponent sourceComponent, LayoutComponentConnectionPoint front, LocomotiveOrientation direction, Guid routeOwner) {
            this.SourceComponent = sourceComponent;
            this.SourceFront = front;
            this.Direction = direction;
            this.routeOwner = routeOwner;
            this.Quality = new RouteQuality(routeOwner);
        }

        #region public Properties

        public ModelComponent SourceComponent { get; }

        public LayoutTrackComponent SourceTrack => TrackOf(SourceComponent);

        public LayoutComponentConnectionPoint SourceFront { get; }

        public LayoutTrackComponent? DestinationTrack => DestinationTarget?.DestinationTrack;

        public RouteTarget? DestinationTarget { get; set; }

        public TrackEdge DestinationEdge => new TrackEdge(Ensure.NotNull<LayoutTrackComponent>(DestinationTrack, nameof(DestinationTrack)), DestinationFront).OtherSide;

        public TrackEdge SourceEdge => Direction == LocomotiveOrientation.Forward
                    ? new TrackEdge(SourceTrack, SourceFront).OtherSide
                    : new TrackEdge(SourceTrack, SourceFront);

        public LayoutComponentConnectionPoint DestinationFront { get; set; }

        public LocomotiveOrientation Direction { get; }

        public Guid RouteOwner => routeOwner;

        public RouteQuality Quality { get; set; }

        public IList<RouteTarget> Targets => targets;

        public IList<int> SwitchStates => switchStates ?? new List<int>();

        public void SetSwitchStates(IList<int> switchStates) {
            this.switchStates = new List<int>(switchStates);
        }

        public int Count => Ensure.NotNull<IList<int>>(SwitchStates, "switchState").Count;

        public IList<TrackEdge> TrackEdges {
            get {
                List<TrackEdge> edges = new List<TrackEdge>();
                TrackEdge edge = SourceEdge;
                var switchStates = Ensure.NotNull<IList<int>>(SwitchStates, "switchState");
                int switchStateIndex = 0;
                bool continueScanning = true;

                do {
                    edges.Add(edge);

                    if (edge.Track == DestinationEdge.Track)
                        continueScanning = false;
                    else {
                        if (edge.Track is IModelComponentIsMultiPath turnout)
                            edge = edge.Track.GetConnectedComponentEdge(turnout.ConnectTo(edge.ConnectionPoint, switchStates[switchStateIndex++]));
                        else
                            edge = edge.Track.GetConnectedComponentEdge(edge.Track.ConnectTo(edge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0]);
                    }
                } while (continueScanning);

                return edges.AsReadOnly();
            }
        }

        /// <summary>
        /// Return a selection containing all components in the route
        /// </summary>
        public LayoutSelection Selection {
            get {
                LayoutSelection selection = new LayoutSelection();

                foreach (TrackEdge edge in TrackEdges)
                    selection.Add(edge.Track);

                return selection;
            }
        }

        public IList<LayoutBlock> Blocks {
            get {
                List<LayoutBlock> blocks = new List<LayoutBlock>();

                foreach (TrackEdge edge in TrackEdges) {
                    var blockInfo = edge.Track.BlockDefinitionComponent;

                    if (blockInfo != null)
                        blocks.Add(blockInfo.Block);
                }

                return blocks.AsReadOnly();
            }
        }

        public IList<LayoutBlockEdgeBase> BlockEdges {
            get {
                List<LayoutBlockEdgeBase> blockEdges = new List<LayoutBlockEdgeBase>();

                foreach (TrackEdge edge in TrackEdges) {
                    var blockEdge = edge.Track.BlockEdgeBase;

                    if (blockEdge != null)
                        blockEdges.Add(blockEdge);
                }

                return blockEdges.AsReadOnly();
            }
        }

        /// <summary>
        /// Return the train for which this route is calculated. Please note that it is possible
        /// that the route is not calculated for a specific train (e.g. in a manual dispatch resgion).
        /// In this case null is returned
        /// </summary>
        public TrainStateInfo? Train => train ?? (train = LayoutModel.StateManager.Trains[routeOwner] as TrainStateInfo);

        #endregion

        #region Utilities

        public static LayoutTrackComponent TrackOf(ModelComponent component) {
            LayoutTrackComponent track = component.Spot.Track;

            if (track == null)
                throw new ArgumentException("This component is not on track: " + component.FullDescription);
            return track;
        }

        #endregion
    }
}
