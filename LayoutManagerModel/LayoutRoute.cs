using System;
using System.Collections.Generic;
using LayoutManager.Components;

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
        RouteClearanceQuality clearanceQuality = RouteClearanceQuality.FreeNow;
        int penalty;
        Guid routeOwner = Guid.Empty;

        static int willBeFreeClearancePenaltyFactor = 20;
        static ILayoutLockManagerServices _lockManagerServices = null;

        public RouteQuality(Guid routeOwner, RouteClearanceQuality clearanceQuality, int penalty) {
            initialize(routeOwner);
            this.clearanceQuality = clearanceQuality;
            this.penalty = penalty;
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
            clearanceQuality = copyFrom.ClearanceQuality;
            penalty = copyFrom.Penalty;
        }

        private void initialize(Guid routeOwner) {
            this.routeOwner = routeOwner;
        }

        public RouteClearanceQuality ClearanceQuality {
            get {
                return clearanceQuality;
            }

            set {
                clearanceQuality = value;
            }
        }

        public int Penalty {
            get {
                return penalty;
            }

            set {
                penalty = value;
            }
        }

        public static int WillBeFreeClearancePenaltyFactor {
            get {
                return willBeFreeClearancePenaltyFactor;
            }

            set {
                willBeFreeClearancePenaltyFactor = value;
            }
        }

        public Guid RouteOwner => routeOwner;

        protected static ILayoutLockManagerServices LockManagerServices {
            get {
                if (_lockManagerServices == null)
                    _lockManagerServices = (ILayoutLockManagerServices)EventManager.Event(new LayoutEvent(null, "get-layout-lock-manager-services"));
                return _lockManagerServices;
            }
        }

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
            this.penalty += blockInfo.Info.LengthInCM;
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

        public override string ToString() => "Clearance: " + clearanceQuality + " Penalty: " + penalty;

        #region IComparable<RouteQuality> Members

        public int CompareTo(RouteQuality other) {
            // If are "free" then penalize one that willBeFree by a factor, so a route
            // which is FreeNow will be preferred if even if its penalty is bit larger
            if (IsFree && other.IsFree) {
                int myPenalty = Penalty;
                int qPenalty = other.Penalty;

                if (ClearanceQuality == RouteClearanceQuality.WillBeFree)
                    myPenalty = ((100 + willBeFreeClearancePenaltyFactor) * Penalty) / 100;
                if (other.ClearanceQuality == RouteClearanceQuality.WillBeFree)
                    qPenalty = ((100 + willBeFreeClearancePenaltyFactor) * other.Penalty) / 100;

                return myPenalty - qPenalty;
            }
            else if (ClearanceQuality == other.ClearanceQuality)
                return Penalty - other.Penalty;
            else
                return (int)ClearanceQuality - (int)other.ClearanceQuality;
        }

        public bool Equals(RouteQuality other) => clearanceQuality == other.clearanceQuality && penalty == other.penalty && routeOwner == other.routeOwner;

        #endregion
    }

    public class RouteTarget {
        readonly TripPlanDestinationEntryInfo destinationEntryInfo;   // The final train destination info
        readonly LayoutTrackComponent destinationTrack;     // The final train destination for this target
        TrackEdge targetEdge;             // The destination to arrive (the split point)
        readonly int switchState;            // The switch state to set in this turnout
        readonly RouteQuality quality;              // The penalty involved in going from this target to the final destination
        bool arrived;
        readonly LayoutComponentConnectionPoint front;
        readonly LocomotiveOrientation direction;

        public RouteTarget(TripPlanDestinationEntryInfo destinationEntryInfo, LayoutTrackComponent destinationTrack, TrackEdge targetEdge, int switchState,
            LayoutComponentConnectionPoint front, LocomotiveOrientation direction, RouteQuality quality) {
            this.destinationEntryInfo = destinationEntryInfo;
            this.destinationTrack = destinationTrack;
            this.targetEdge = targetEdge;
            this.switchState = switchState;
            this.front = front;
            this.direction = direction;
            this.quality = new RouteQuality(quality);
        }

        public TripPlanDestinationEntryInfo DestinationEntryInfo => destinationEntryInfo;

        public LayoutTrackComponent DestinationTrack => destinationTrack;

        public TrackEdge DestinationEdge => new TrackEdge(DestinationTrack, front);

        public TrackEdge TargetEdge => targetEdge;

        public TrackEdgeId TargetEdgeId => new TrackEdgeId(targetEdge);

        public int SwitchState => switchState;

        public LayoutComponentConnectionPoint Front => front;

        public LocomotiveOrientation Direction => direction;

        public RouteQuality Quality => quality;

        public override bool Equals(object o) {

            if (o is RouteTarget otherTarget)
                return otherTarget.TargetEdge == TargetEdge && otherTarget.SwitchState == switchState;
            return false;
        }

        public override int GetHashCode() => targetEdge.GetHashCode();

        public bool Arrived {
            get {
                return arrived;
            }

            set {
                arrived = value;
            }
        }
    }

    public class BestRoute : ITripRoute {
        readonly ModelComponent sourceComponent;
        RouteTarget destinationTarget;
        readonly LayoutComponentConnectionPoint sourceFront;
        LayoutComponentConnectionPoint destinationFront;
        readonly LocomotiveOrientation direction;
        Guid routeOwner;
        RouteQuality quality;
        readonly List<RouteTarget> targets = new List<RouteTarget>();
        List<int> switchStates;
        TrainStateInfo train;

        public BestRoute(ModelComponent sourceComponent, LayoutComponentConnectionPoint front, LocomotiveOrientation direction, Guid routeOwner) {
            this.sourceComponent = sourceComponent;
            this.sourceFront = front;
            this.direction = direction;
            this.routeOwner = routeOwner;
            this.quality = new RouteQuality(routeOwner);
        }

        #region public Properties

        public ModelComponent SourceComponent => sourceComponent;

        public LayoutTrackComponent SourceTrack => TrackOf(sourceComponent);

        public LayoutComponentConnectionPoint SourceFront => sourceFront;

        public LayoutTrackComponent DestinationTrack {
            get {
                if (destinationTarget == null)
                    return null;
                return destinationTarget.DestinationTrack;
            }
        }

        public RouteTarget DestinationTarget {
            get {
                return destinationTarget;
            }

            set {
                destinationTarget = value;
            }
        }

        public TrackEdge DestinationEdge => new TrackEdge(DestinationTrack, DestinationFront).OtherSide;

        public TrackEdge SourceEdge {
            get {
                if (Direction == LocomotiveOrientation.Forward)
                    return new TrackEdge(SourceTrack, SourceFront).OtherSide;
                else
                    return new TrackEdge(SourceTrack, SourceFront);
            }
        }

        public LayoutComponentConnectionPoint DestinationFront {
            get {
                return destinationFront;
            }

            set {
                destinationFront = value;
            }
        }

        public LocomotiveOrientation Direction => direction;

        public Guid RouteOwner => routeOwner;

        public RouteQuality Quality {
            get {
                return quality;
            }

            set {
                quality = value;
            }
        }

        public IList<RouteTarget> Targets => targets;

        public IList<int> SwitchStates => switchStates.AsReadOnly();

        public void SetSwitchStates(IList<int> switchStates) {
            this.switchStates = new List<int>(switchStates);
        }

        public int Count => SwitchStates.Count;

        public IList<TrackEdge> TrackEdges {
            get {
                List<TrackEdge> edges = new List<TrackEdge>();
                TrackEdge edge = SourceEdge;
                IList<int> switchStates = SwitchStates;
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
                    LayoutBlockDefinitionComponent blockInfo = edge.Track.BlockDefinitionComponent;

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
                    LayoutBlockEdgeBase blockEdge = edge.Track.BlockEdgeBase;

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
        public TrainStateInfo Train {
            get {
                if (train == null)
                    train = LayoutModel.StateManager.Trains[routeOwner] as TrainStateInfo;

                return train;
            }
        }

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
