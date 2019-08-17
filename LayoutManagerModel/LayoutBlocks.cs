using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;
using System.Linq;

using LayoutManager.Components;
using System.Threading;

#nullable enable
namespace LayoutManager.Model {
    #region Block related classes

    public class LayoutBlockBase {
        protected const string State_TrainEntry = "TrainEntry";
        protected const string A_Time = "Time";

        private List<LayoutBlockEdgeBase>? blockEdges;
        private LayoutBlockDefinitionComponent? blockInfo;
        private Guid id;

        public TrackEdgeCollection TrackEdges { get; } = new TrackEdgeCollection();

        /// <summary>
        /// Return an array of all the track contacts that bounds current block
        /// </summary>
        public IList<LayoutBlockEdgeBase> BlockEdges {
            get {
                if (blockEdges == null) {
                    blockEdges = new List<LayoutBlockEdgeBase>();

                    foreach (TrackEdge edge in TrackEdges) {
                        var blockEdge = edge.Track.BlockEdgeBase;

                        if (blockEdge != null)
                            blockEdges.Add(blockEdge);
                    }
                }

                return blockEdges;
            }
        }

        /// <summary>
        /// Use this if you are sure that block definition is defined
        /// </summary>
        public LayoutBlockDefinitionComponent BlockDefinintion {
            get => blockInfo ?? throw new ArgumentNullException("block.BlockDefinition is null");
            set => blockInfo = value;
        }

        /// <summary>
        /// This may be used if block definition can be null (was not yet set)
        /// </summary>
        public LayoutBlockDefinitionComponent? OptionalBlockDefinition {
            get { return blockInfo; }
        }

        public Guid Id {
            get {
                if (BlockDefinintion != null)
                    return BlockDefinintion.Id;
                else {
                    if (id == Guid.Empty)
                        id = Guid.NewGuid();
                    return id;
                }
            }
        }

        public LayoutBlock OtherBlock(LayoutBlockEdgeBase blockEdge) {
            var track = blockEdge.Track;

            if (track != null) {
                if (track.GetBlock(track.ConnectionPoints[0]).Id == Id)
                    return track.GetBlock(track.ConnectionPoints[1]);
                else if (track.GetBlock(track.ConnectionPoints[1]).Id == Id)
                    return track.GetBlock(track.ConnectionPoints[0]);
            }

            throw new ArgumentException("Block edge " + blockEdge.FullDescription + " does not bound block " + BlockDefinintion.FullDescription);
        }
    }

    /// <summary>
    /// A layout Block is the collection of tracks bounded by track with track contacts
    /// or end of track. The track Blocks are assigned during the layout compilation phase.
    /// During operation Blocks are used for train tracking. A Block can "contains" zero
    /// or more locomotives. When a track contact is triggered, the train tracking algorithm
    /// figure out what locomotive has triggered the track contact and for which Block to which
    /// Block the locomotive is passing.
    /// </summary>
    public class LayoutBlock : LayoutBlockBase {
        private readonly List<TrainLocationInfo> trainsInBlock = new List<TrainLocationInfo>();
        private LayoutLockRequest? lockRequest;
        private bool isLinear = true;
        private bool isLinearCalculated;
        private bool canTrainWaitDefault;
        private bool canTrainWaitDefaultCalculated;
        private TrainStateInfo? trainLeavingBlock;
        private LayoutComponentConnectionPoint trainLeavingBlockFront;
        private int whenTrainLeftBlock = 0;

        public LayoutBlock() {
        }

        /// <summary>
        /// The train occupancy detection block that this block is part of.
        /// </summary>
        public LayoutOccupancyBlock? OccupancyBlock { get; set; }

        public LayoutLockRequest? LockRequest {
            get => this.lockRequest;

            set {
                this.lockRequest = value;
                Redraw();
            }
        }

        /// <summary>
        /// Check if a block is a simple linear block. That is, the block does not contain any
        /// turnouts
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public bool IsLinear {
            get {
                if (!isLinearCalculated) {
                    isLinear = true;

                    foreach (TrackEdge edge in TrackEdges) {
                        if (edge.Track is IModelComponentIsMultiPath) {
                            isLinear = false;
                            break;
                        }
                    }

                    isLinearCalculated = true;
                }
                return isLinear;
            }
        }

        /// <summary>
        /// Get the default value for whether trains can wait on this block
        /// </summary>
        public bool CanTrainWaitDefault {
            get {
                if (!canTrainWaitDefaultCalculated) {
                    bool canTrainWait = true;

                    // By default a block is non waitable if it contains turnout or a cross
                    foreach (TrackEdge edge in TrackEdges) {
                        if (edge.Track is IModelComponentIsMultiPath) {
                            canTrainWait = false;
                            break;
                        }
                        else if (edge.Track is LayoutDoubleTrackComponent && ((LayoutDoubleTrackComponent)edge.Track).IsCross) {
                            canTrainWait = false;
                            break;
                        }
                    }

                    canTrainWaitDefault = canTrainWait;
                    canTrainWaitDefaultCalculated = true;
                }

                return canTrainWaitDefault;
            }
        }

        public bool CanTrainWait {
            get {
                return BlockDefinintion != null ? BlockDefinintion.Info.CanTrainWait : CanTrainWaitDefault;
            }
        }

        public string Name => BlockDefinintion.Name;

        public string FullDescription => BlockDefinintion.FullDescription;

        public override string ToString() => FullDescription;

        /// <summary>
        /// Return the block's power source. It is defined as the power source of the
        /// block info component is one exists, or the power source for the first
        /// track edge in the block.
        /// </summary>
        public ILayoutPower? Power {
            get {
                if (BlockDefinintion != null)
                    return BlockDefinintion.Track.GetPower(BlockDefinintion.Track.ConnectionPoints[0]);
                else if (TrackEdges.Count > 0)
                    return TrackEdges[0].Track.GetPower(TrackEdges[0].ConnectionPoint);
                return null;
            }
        }

        /// <summary>
        /// Associate a given track with this block.
        /// </summary>
        /// <param name="trackEdge">The track and connection point to be associated with the block</param>
        public void Add(TrackEdge trackEdge) {
            trackEdge.Track.SetBlock(trackEdge.ConnectionPoint, this);
            TrackEdges.Add(trackEdge);
        }

        /// <summary>
        /// Redraw all tracks belonging to this block
        /// </summary>
        public void Redraw() {
            foreach (TrackEdge edge in TrackEdges)
                edge.Track.Redraw();

            if (BlockDefinintion != null)
                BlockDefinintion.Redraw();
        }

        public void AddTrain(TrainLocationInfo trainLocation) {
            BlockDefinintion.EraseImage();

            if (trainLocation.BlockEdgeId == Guid.Empty)
                trainsInBlock.Add(trainLocation);
            else {
                // Figure out where to add the train part so it will be drawn correctly on the block info
                LayoutComponentConnectionPoint cpEntry;

                try {
                    Debug.Assert(trainLocation.BlockEdge != null);
                    cpEntry = BlockDefinintion.Track.ConnectionPoints[BlockDefinintion.GetConnectionPointIndex(trainLocation.BlockEdge)];
                }
                catch (ArgumentException) {
                    cpEntry = 0;
                }

                trainsInBlock.Add(trainLocation);

                TrainStateInfo trainState = new TrainStateInfo(trainLocation.LocomotiveStateElement);

                if (trainState.Managed) {
                    if (!trainLocation.IsDisplayFrontKnown) {
                        if (trainLocation.Train.LastBlockEdgeCrossingSpeed < 0)
                            trainLocation.DisplayFront = cpEntry;
                        else {
                            if (BlockDefinintion.Track is IModelComponentIsMultiPath multiPath)
                                trainLocation.DisplayFront = multiPath.ConnectTo(cpEntry, multiPath.CurrentSwitchState);
                            else
                                trainLocation.DisplayFront = BlockDefinintion.Track.ConnectTo(cpEntry, LayoutComponentConnectionType.Passage)![0];
                        }
                    }
                }
            }

            LayoutModel.StateManager.Components.StateOf(BlockDefinintion, State_TrainEntry, create: true).SetAttribute(A_Time, DateTime.Now.Ticks);

            BlockDefinintion.OnComponentChanged();
        }

        public void RemoveTrain(TrainLocationInfo trainLocation) {
            BlockDefinintion.EraseImage();

            TrainLocationInfo? trainLocationToRemove = null;

            foreach (TrainLocationInfo location in trainsInBlock) {
                if (location.Element == trainLocation.Element) {
                    trainLocationToRemove = location;
                    break;
                }
            }

            Debug.Assert(trainLocationToRemove != null);
            trainsInBlock.Remove(trainLocationToRemove);

            trainLeavingBlock = trainLocationToRemove.Train;
            trainLeavingBlockFront = trainLocationToRemove.DisplayFront;
            whenTrainLeftBlock = Environment.TickCount;

            LayoutModel.StateManager.Components.Remove(BlockDefinintion, State_TrainEntry);

            BlockDefinintion.OnComponentChanged();
        }

        public long? TrainEntryTime {
            get => (long?)LayoutModel.StateManager.Components.StateOf(BlockDefinintion, State_TrainEntry)?.AttributeValue(A_Time);
        }

        public void ClearTrains() {
            trainsInBlock.Clear();
        }

        public IList<TrainLocationInfo> Trains => trainsInBlock.AsReadOnly();

        public bool HasTrains => trainsInBlock.Count > 0;

        /// <summary>
        /// All the locomotives in trains in this block
        /// </summary>
        public IEnumerable<TrainLocomotiveInfo> Locomotives => Trains.SelectMany(t => t.Train.Locomotives);

        public bool IsTrainInBlock(Guid trainId) {
            foreach (TrainLocationInfo trainLocation in trainsInBlock)
                if (trainId == trainLocation.Train.Id)
                    return true;
            return false;
        }

        public bool IsTrainInBlock(TrainStateInfo train) => IsTrainInBlock(train.Id);

        public bool IsLocked => LockRequest != null;

        public LayoutSelection GetSelection() {
            LayoutSelection selection = new LayoutSelection();

            foreach (TrackEdge edge in TrackEdges)
                if (edge.Track.BlockEdgeBase == null)
                    selection.Add(edge.Track);

            return selection;
        }

       public bool CheckForTrainRedetection() {
            if (!BlockDefinintion.Info.IsOccupancyDetectionBlock)
                return false;

            if (trainLeavingBlock != null && Environment.TickCount - whenTrainLeftBlock < 1000) {
                trainLeavingBlock.PlaceInBlock(this, trainLeavingBlockFront);
                Trace.WriteLine("Redetection of train " + trainLeavingBlock.DisplayName);
                trainLeavingBlock = null;
                return true;
            }

            trainLeavingBlock = null;
            return false;
        }
    }

    /// <summary>
    /// Train occupancy block are blocks that are linked to train detection hardware. A train detection block may contain more than
    /// one logical block. In most cases, it will contain one logical block which is idetical to the train detection block, but if for
    /// example, the train detection block has a track contact, then the train occupancy block contains two logical block.
    /// </summary>
    public class LayoutOccupancyBlock : LayoutBlockBase {
        private List<LayoutBlock>? containedBlocks;
        private LayoutBlock? myBlock;

        /// <summary>
        /// Add a logical block that is contained in the train occupancy block
        /// </summary>
        /// <param name="block">The block to be added</param>
        /// <remarks>
        /// Since an occupancy block that contains one logical block is the very common case, it is handled seperatly to save
        /// the overhead of holding variable array.
        /// </remarks>
        public void AddBlock(LayoutBlock block) {
            block.OccupancyBlock = this;

            if (myBlock == null)
                myBlock = block;
            else if (containedBlocks == null) {
                containedBlocks = new List<LayoutBlock>(2) {
                    myBlock,
                    block
                };
            }
            else
                containedBlocks.Add(block);
        }

        /// <summary>
        /// Return an array of the logical blocks contained in the train occupancy detection block
        /// </summary>
        public IList<LayoutBlock> ContainedBlocks {
            get {
                if (myBlock == null)
                    return Array.AsReadOnly<LayoutBlock>(new LayoutBlock[] { });
                else return containedBlocks == null ? Array.AsReadOnly<LayoutBlock>(new LayoutBlock[1] { myBlock }) : containedBlocks.AsReadOnly();
            }
        }
    }

    public class LayoutModelBlockDictionary : Dictionary<Guid, LayoutBlock>, IEnumerable<LayoutBlock> {
        public void Add(LayoutBlock block) {
            Add(block.Id, block);
        }

        public new IEnumerator<LayoutBlock> GetEnumerator() => Values.GetEnumerator();
    }

    #endregion

    #region Locks related classes

    public class LayoutLockBlockEntry {
        [Flags]
        public enum LockBlockOptions {
            None = 0,
            ForceRedSignal = 0x0001
        };

        private LockBlockOptions options;

        internal LayoutLockBlockEntry(LayoutBlock block, LockBlockOptions options) {
            this.Block = block;
            this.options = options;
        }

        public LayoutBlock Block { get; }

        public LayoutBlockDefinitionComponent BlockDefinition => Block.BlockDefinintion;

        public ResourceCollection Resources => BlockDefinition.Info.Resources;

        public LockBlockOptions Options => options;

        public bool ForceRedSignal {
            get => (options & LockBlockOptions.ForceRedSignal) != 0;

            set {
                if (value)
                    options |= LockBlockOptions.ForceRedSignal;
                else
                    options &= ~LockBlockOptions.ForceRedSignal;
            }
        }

        static public string GetDescription(Guid resourceId) {
            if (LayoutModel.Blocks.TryGetValue(resourceId, out LayoutBlock block))
                return "Block: " + block.Name;
            else {
                var component = LayoutModel.Component<ModelComponent>(resourceId, LayoutModel.ActivePhases);

                return component != null ? "Component: " + component.FullDescription : "<Unknown resource!!>";
            }
        }

        public string GetDescription() => GetDescription(Block.Id);
    };

    #region ResourceEntry dictionary

    /// <summary>
    /// In addition to track blocks, each lock request may specify additional resources that must be ready in order for the lock to be granted
    /// The resources are freed when all blocks in the lock request that specified them are no longer locked
    /// </summary>
    public class ResourceUseCountMap : Dictionary<Guid, int> {
    }

    public class LayoutLockBlockEntryDictionary : Dictionary<Guid, LayoutLockBlockEntry>, IEnumerable<LayoutLockBlockEntry> {
        public void Add(LayoutBlock block, LayoutLockBlockEntry.LockBlockOptions options = LayoutLockBlockEntry.LockBlockOptions.None) {
            this[block.Id] = new LayoutLockBlockEntry(block, options);
            foreach (var resourceInfo in block.BlockDefinintion.Info.Resources)
                addResource(resourceInfo.ResourceId);
        }

        public ResourceUseCountMap ResourcesUseCount { get; } = new ResourceUseCountMap();

        public void Add(IEnumerable<LayoutBlock> blocks, LayoutLockBlockEntry.LockBlockOptions options = LayoutLockBlockEntry.LockBlockOptions.None) {
            foreach (var block in blocks)
                Add(block, options);
        }

        private Guid addResource(Guid resourceId) {
            if (ResourcesUseCount.ContainsKey(resourceId))
                ResourcesUseCount[resourceId]++;
            else
                ResourcesUseCount[resourceId] = 1;
            return resourceId;
        }

        public void AddResources(IList<ILayoutLockResource> resources) {
            foreach (var r in resources)
                addResource(r.Id);
        }

        public new IEnumerator<LayoutLockBlockEntry> GetEnumerator() => Values.GetEnumerator();
    }

    #endregion

    public enum LayoutLockType {
        Train, ManualDispatch, Programming
    };

    public class LayoutLockRequest {
        private IList<ILayoutLockResource>? resources = null;

        public enum RequestStatus { NotRequested, NotGranted, Granted, PartiallyUnlocked };

        public LayoutLockRequest() {
            CancellationToken = CancellationToken.None;
            Status = RequestStatus.NotRequested;
        }

        public LayoutLockRequest(Guid ownerId) {
            this.OwnerId = ownerId;
        }

        public LayoutLockRequest(Guid ownerId, Action onLockGranted)
            : this(ownerId) {
            this.OnLockGranted = onLockGranted;
        }

        public LayoutLockRequest(Guid ownerId, LayoutEvent lockedEvent) : this(ownerId) {
            this.LockedEvent = lockedEvent;
        }

        #region Properties

        public Guid OwnerId { get; set; }
        public LayoutLockType Type { get; set; }
        public RequestStatus Status { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public Action? OnLockGranted { get; set; }

        public LayoutLockBlockEntryDictionary Blocks { get; } = new LayoutLockBlockEntryDictionary();

        public IList<ILayoutLockResource>? Resources {
            get => resources;

            set {
                Trace.Assert(resources == null);
                if (value != null) {
                    resources = value;
                    Blocks.AddResources(resources);
                }
            }
        }

        public ResourceUseCountMap ResourceUseCount => Blocks.ResourcesUseCount;

        public int UnlockedBlocksCount { get; set; }

        public LayoutEvent LockedEvent {
            set => OnLockGranted = () => EventManager.Event(value);
        }

        public bool IsManualDispatchLock => Type == LayoutLockType.ManualDispatch;

        #endregion

        #region Operations

        public void Dump() {
            bool first = true;

            Trace.Write("== BEGIN Lock Request (" + Status.ToString() + "), owner: ");

            var train = LayoutModel.StateManager.Trains[OwnerId];

            if (train != null)
                Trace.Write("train " + train.DisplayName);
            else {
                var manualDispatchRegion = LayoutModel.StateManager.ManualDispatchRegions[OwnerId];

                if (manualDispatchRegion != null)
                    Trace.Write("manual dispatch region " + manualDispatchRegion.Name);
                else
                    Trace.Write("ownerID " + OwnerId.ToString());
            }

            Trace.Write(" for ");

            foreach (LayoutLockBlockEntry blockEntry in Blocks) {
                LayoutBlock block = blockEntry.Block;

                Trace.Write($"{(first ? "" : ", ")}{(block.BlockDefinintion == null ? "UNNAMED" : block.BlockDefinintion.FullDescription)}");
                first = false;
            }

            Trace.WriteLine("");

            if (ResourceUseCount.Count > 0) {
                Trace.Write(" Needed Resources ");
                first = true;

                foreach (var resourceId in ResourceUseCount.Keys) {
                    Trace.Write($"{(first ? "" : ", ")} {LayoutModel.Component<ModelComponent>(resourceId, LayoutPhase.All)?.FullDescription ?? "(null)"}={ResourceUseCount[resourceId]}");
                    first = false;
                }

                Trace.WriteLine("");
            }

            Trace.WriteLine("== END lock Request");
        }

        #endregion
    }

    #endregion

    #region Train tracking related classes

    internal class TrainMotionListEntry {
        public LocomotiveTrackingResult TrackingResult;
        public int PreviousCrossingSpeed;

        public TrainMotionListEntry(LocomotiveTrackingResult trackingResult, int previousCrossingSpeed) {
            this.TrackingResult = trackingResult;
            this.PreviousCrossingSpeed = previousCrossingSpeed;
        }
    }

    public class TrainMotionListManager {
        private readonly List<TrainMotionListEntry> motions = new List<TrainMotionListEntry>();
        private readonly Dictionary<Guid, int> trainSpeeds = new Dictionary<Guid, int>();

        public void Add(LocomotiveTrackingResult trackingResult) {
            TrainStateInfo train = trackingResult.Train;

            motions.Add(new TrainMotionListEntry(trackingResult, train.LastBlockEdgeCrossingSpeed));

            if (!trainSpeeds.ContainsKey(trackingResult.TrainId))
                trainSpeeds.Add(train.Id, train.LastBlockEdgeCrossingSpeed);

            if (train.Managed)
                train.LastBlockEdgeCrossingSpeed = train.Speed;

            var fromLocationTrainPart = train.LocationOfBlock(trackingResult.FromBlock)!.TrainPart;

            train.EnterBlock(fromLocationTrainPart, trackingResult.ToBlock, trackingResult.BlockEdge, null);
        }

        public void Do() {
            foreach (TrainMotionListEntry motionListEntry in motions) {
                LocomotiveTrackingResult trackingResult = motionListEntry.TrackingResult;

                EventManager.Event(new LayoutEvent("train-enter-block", trackingResult.Train, trackingResult.ToBlock));
                EventManager.Event(new LayoutEvent("train-crossed-block-edge", trackingResult.Train, trackingResult.BlockEdge));
                if (trackingResult.BlockEdge is LayoutBlockEdgeComponent) {
                    EventManager.Event(new LayoutEvent("occupancy-block-edge-crossed",
                        (LayoutBlockEdgeComponent)trackingResult.BlockEdge, trackingResult.Train, null));
                }
            }
        }

        public void Undo() {
            motions.Reverse();

            foreach (TrainMotionListEntry motionListEntry in motions) {
                LocomotiveTrackingResult trackingResult = motionListEntry.TrackingResult;

                trackingResult.Train.LeaveBlock(trackingResult.ToBlock, false);
            }

            foreach (KeyValuePair<Guid, int> d in trainSpeeds) {
                Guid trainID = d.Key;
                int speed = d.Value;
                var train = LayoutModel.StateManager.Trains[trainID];

                if (train != null)
                    train.LastBlockEdgeCrossingSpeed = speed;
            }
        }
    }

    public class LocomotiveTrackingResult : LayoutObject {
        private const string A_BlockEdgeID = "BlockEdgeID";
        private const string A_TrainID = "TrainID";
        private const string A_FromBlockID = "FromBlockID";
        private const string A_ToBlockID = "ToBlockID";
        private TrainLocationInfo? trainLocation;

        public LocomotiveTrackingResult(LayoutBlockEdgeBase blockEdge, TrainStateInfo trainState,
            TrainLocationInfo trainLocation, LayoutBlock fromBlock, LayoutBlock toBlock) {
            initialize(blockEdge, trainState, trainLocation, fromBlock, toBlock, null);
        }

        public LocomotiveTrackingResult(LayoutBlockEdgeBase blockEdge, TrainStateInfo trainState,
            TrainLocationInfo trainLocation, LayoutBlock fromBlock, LayoutBlock toBlock, TrainMotionListManager motionListManager) {
            initialize(blockEdge, trainState, trainLocation, fromBlock, toBlock, motionListManager);
        }

        private void initialize(LayoutBlockEdgeBase blockEdge, TrainStateInfo trainState,
            TrainLocationInfo trainLocation, LayoutBlock fromBlock, LayoutBlock toBlock, TrainMotionListManager? motionListManager) {
            XmlDocument.LoadXml("<TrackingResult />");

            BlockEdge = blockEdge;
            Train = trainState;
            FromBlock = fromBlock;
            ToBlock = toBlock;
            this.MotionListManager = motionListManager;

            this.trainLocation = trainLocation;
        }

        public Guid BlockEdgeId {
            get => (Guid)Element.AttributeValue(A_BlockEdgeID);
            set => Element.SetAttribute(A_BlockEdgeID, value);
        }

        public LayoutBlockEdgeBase BlockEdge {
            get => Ensure.NotNull<LayoutBlockEdgeBase>(LayoutModel.Component<LayoutBlockEdgeBase>(BlockEdgeId, LayoutModel.ActivePhases), $"BlockEdge of id {BlockEdgeId}");
            set => BlockEdgeId = value.Id;
        }

        public Guid TrainId {
            get => (Guid)Element.AttributeValue(A_TrainID);
            set => Element.SetAttribute(A_TrainID, value);
        }

        public TrainStateInfo Train {
            get => LayoutModel.StateManager.Trains[TrainId]!;
            set => TrainId = value.Id;
        }

        public TrainLocationInfo? TrainLocation => trainLocation;

        public Guid FromBlockId {
            get => (Guid)Element.AttributeValue(A_FromBlockID);
            set => Element.SetAttribute(A_FromBlockID, value);
        }

        public LayoutBlock FromBlock {
            get => LayoutModel.Blocks[FromBlockId];
            set => FromBlockId = value.Id;
        }

        public Guid ToBlockId {
            get => (Guid)Element.AttributeValue(A_ToBlockID);
            set => Element.SetAttribute(A_ToBlockID, value);
        }

        public LayoutBlock ToBlock {
            get => LayoutModel.Blocks[ToBlockId];
            set => ToBlockId = value.Id;
        }

        public TrainMotionListManager? MotionListManager { get; set; }

        public void Commit() => MotionListManager?.Do();

        public void Rollback() => MotionListManager?.Undo();

        public void Dump() {
            Trace.WriteLine("Tracking result for train " + Train.DisplayName + "(Speed: " + Train.Speed + ")\n  from: " + FromBlock.BlockDefinintion.FullDescription + "\n  to: " + ToBlock.BlockDefinintion.FullDescription);
            Trace.WriteLine("  Crossing edge: " + BlockEdge.FullDescription);
        }
    }

    public class TrainChangingBlock {
        public TrainChangingBlock(LayoutBlockEdgeBase blockEdge, TrainStateInfo train, LayoutBlock fromBlock, LayoutBlock toBlock) {
            BlockEdge = blockEdge;
            Train = train;
            FromBlock = fromBlock;
            ToBlock = toBlock;
        }

        public LayoutBlockEdgeBase BlockEdge { get; }

        public TrainStateInfo Train { get; }

        public LayoutBlock FromBlock { get; }

        public LayoutBlock ToBlock { get; }
    }

    #endregion
}
