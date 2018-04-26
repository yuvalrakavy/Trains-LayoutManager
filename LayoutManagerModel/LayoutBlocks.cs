using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Xml;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using LayoutManager.Components;
using System.Threading;

namespace LayoutManager.Model {

	#region Block related classes

	public class LayoutBlockBase : IDisposable {
		TrackEdgeCollection				trackEdges = new TrackEdgeCollection();
		List<LayoutBlockEdgeBase>	blockEdges;
		LayoutBlockDefinitionComponent	blockInfo;
		Guid						id;

        public TrackEdgeCollection TrackEdges => trackEdges;

        /// <summary>
        /// Return an array of all the track contacts that bounds current block
        /// </summary>
        public IList<LayoutBlockEdgeBase> BlockEdges {
			get {
				if(blockEdges == null) {
					blockEdges = new List<LayoutBlockEdgeBase>();

					foreach(TrackEdge edge in TrackEdges)
						if(edge.Track.BlockEdgeBase!= null)
							blockEdges.Add(edge.Track.BlockEdgeBase);
				}

				return blockEdges;
			}
		}

		public LayoutBlockDefinitionComponent BlockDefinintion {
			get {
				return blockInfo;
			}

			set {
				blockInfo = value;
			}
		}

		public Guid Id {
			get {
				if(BlockDefinintion != null)
					return BlockDefinintion.Id;
				else {
					if(id == Guid.Empty)
						id = Guid.NewGuid();
					return id;
				}
			}
		}

		public LayoutBlock OtherBlock(LayoutBlockEdgeBase blockEdge) {
			LayoutStraightTrackComponent track = blockEdge.Track;

			if(track.GetBlock(track.ConnectionPoints[0]).Id == Id)
				return track.GetBlock(track.ConnectionPoints[1]);
			else if(track.GetBlock(track.ConnectionPoints[1]).Id == Id)
				return track.GetBlock(track.ConnectionPoints[0]);
			else
				throw new ArgumentException("Block edge " + blockEdge.FullDescription + " does not bound block " + BlockDefinintion.FullDescription);
		}

		public virtual void Dispose() {
			trackEdges.Clear();
			blockEdges = null;
			blockInfo = null;
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
		List<TrainLocationInfo>			trainsInBlock = new List<TrainLocationInfo>();
		LayoutOccupancyBlock			occupancyBlock;
		bool							isLinear = true;
		bool							isLinearCalculated;
		bool							canTrainWaitDefault;
		bool							canTrainWaitDefaultCalculated;
		TrainStateInfo                  trainLeavingBlock;
		LayoutComponentConnectionPoint  trainLeavingBlockFront;
		int                             whenTrainLeftBlock = 0;


		public LayoutBlock() {
		}

		/// <summary>
		/// The train occupancy detection block that this block is part of.
		/// </summary>
		public LayoutOccupancyBlock OccupancyBlock {
			get {
				return occupancyBlock;
			}

			set {
				occupancyBlock = value;
			}
		}

        public LayoutLockRequest LockRequest { get; set; }

		/// <summary>
		/// Check if a block is a simple linear block. That is, the block does not contain any
		/// turnouts
		/// </summary>
		/// <param name="block"></param>
		/// <returns></returns>
		public bool IsLinear {
			get {
				if(!isLinearCalculated) {
					isLinear = true;

					foreach(TrackEdge edge in TrackEdges) {
						if(edge.Track is IModelComponentIsMultiPath) {
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
				if(!canTrainWaitDefaultCalculated) {
					bool	canTrainWait = true;

					// By default a block is non waitable if it contains turnout or a cross
					foreach(TrackEdge edge in TrackEdges) {
						if(edge.Track is IModelComponentIsMultiPath) {
							canTrainWait = false;
							break;
						}
						else if(edge.Track is LayoutDoubleTrackComponent && ((LayoutDoubleTrackComponent)edge.Track).IsCross) {
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
				if(BlockDefinintion != null)
					return BlockDefinintion.Info.CanTrainWait;
				else
					return CanTrainWaitDefault;
			}
		}

        public String Name => BlockDefinintion?.Name;

        public string FullDescription => BlockDefinintion?.FullDescription;


        public override string ToString() => FullDescription;

        /// <summary>
        /// Return the block's power source. It is defined as the power source of the
        /// block info component is one exists, or the power source for the first
        /// track edge in the block.
        /// </summary>
        public ILayoutPower Power {
			get {
				if(BlockDefinintion != null)
					return BlockDefinintion.Track.GetPower(BlockDefinintion.Track.ConnectionPoints[0]);
				else if(TrackEdges.Count > 0)
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
			foreach(TrackEdge edge in TrackEdges)
				edge.Track.Redraw();

			if(BlockDefinintion != null)
				BlockDefinintion.Redraw();
		}

		public void AddTrain(TrainLocationInfo trainLocation) {
			if(BlockDefinintion != null)
				BlockDefinintion.EraseImage();

			if(trainLocation.BlockEdgeId == Guid.Empty || BlockDefinintion == null)
				trainsInBlock.Add(trainLocation);
			else {
				// Figure out where to add the train part so it will be drawn correctly on the block info
				LayoutComponentConnectionPoint cpEntry;

				try {
					cpEntry = BlockDefinintion.Track.ConnectionPoints[BlockDefinintion.GetConnectionPointIndex(trainLocation.BlockEdge)];
				}
				catch(ArgumentException) {
					cpEntry = 0;
				}

				trainsInBlock.Add(trainLocation);

				TrainStateInfo	trainState = new TrainStateInfo(trainLocation.LocomotiveStateElement);

				if(trainState.Managed) {
					if(!trainLocation.IsDisplayFrontKnown) {
						if(trainLocation.Train.LastBlockEdgeCrossingSpeed < 0)
							trainLocation.DisplayFront = cpEntry;
						else {
							IModelComponentIsMultiPath multiPath = BlockDefinintion.Track as IModelComponentIsMultiPath;

							if(multiPath != null)
								trainLocation.DisplayFront = multiPath.ConnectTo(cpEntry, multiPath.CurrentSwitchState);
							else
								trainLocation.DisplayFront = BlockDefinintion.Track.ConnectTo(cpEntry, LayoutComponentConnectionType.Passage)[0];
						}
					}
				}
			}

			if(BlockDefinintion != null)
				BlockDefinintion.OnComponentChanged();
		}

		public void RemoveTrain(TrainLocationInfo trainLocation) {
			if(BlockDefinintion != null)
				BlockDefinintion.EraseImage();

			TrainLocationInfo	trainLocationToRemove = null;

			foreach(TrainLocationInfo location in trainsInBlock) {
				if(location.Element == trainLocation.Element) {
					trainLocationToRemove = location;
					break;
				}
			}

			Debug.Assert(trainLocationToRemove != null);
			trainsInBlock.Remove(trainLocationToRemove);

			trainLeavingBlock = trainLocationToRemove.Train;
			trainLeavingBlockFront = trainLocationToRemove.DisplayFront;
			whenTrainLeftBlock = Environment.TickCount;

			if(BlockDefinintion != null)
				BlockDefinintion.OnComponentChanged();
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
			foreach(TrainLocationInfo trainLocation in trainsInBlock)
				if(trainId == trainLocation.Train.Id)
					return true;
			return false;
		}

        public bool IsTrainInBlock(TrainStateInfo train) => IsTrainInBlock(train.Id);

        public bool IsLocked => LockRequest != null;

        public LayoutSelection GetSelection() {
			LayoutSelection	selection = new LayoutSelection();

			foreach(TrackEdge edge in TrackEdges)
				if(edge.Track.BlockEdgeBase == null)
					selection.Add(edge.Track);
			
			return selection;
		}

		public override void Dispose() {
			base.Dispose();

			trainsInBlock.Clear();
			isLinearCalculated = false;
			canTrainWaitDefaultCalculated = false;
		}

		public bool CheckForTrainRedetection() {
			if(!BlockDefinintion.Info.IsOccupancyDetectionBlock)
				return false;

			if(trainLeavingBlock != null && Environment.TickCount - whenTrainLeftBlock < 1000) {
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
		List<LayoutBlock>	containedBlocks ;
		LayoutBlock			myBlock;

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

			if(myBlock == null)
				myBlock = block;
			else if(containedBlocks == null) {
				containedBlocks = new List<LayoutBlock>(2);

				containedBlocks.Add(myBlock);
				containedBlocks.Add(block);
			}
			else
				containedBlocks.Add(block);
		}

		/// <summary>
		/// Return an array of the logical blocks contained in the train occupancy detection block
		/// </summary>
		public IList<LayoutBlock> ContainedBlocks {
			get {
				IList<LayoutBlock> result;

				if(myBlock == null)
					result = Array.AsReadOnly<LayoutBlock>(new LayoutBlock[] { });
				else if(containedBlocks == null)
					result = Array.AsReadOnly<LayoutBlock>(new LayoutBlock[1] { myBlock });
				else
					result = containedBlocks.AsReadOnly();

				return result;
			}
		}
	}

	public class LayoutModelBlockDictionary : Dictionary<Guid, LayoutBlock>, IEnumerable<LayoutBlock>, IDisposable {
		public void Add(LayoutBlock block) {
			Add(block.Id, block);
		}

		public void Dispose() {
			foreach(LayoutBlock block in Values)
				block.Dispose();
			this.Clear();
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

		LockBlockOptions		options;
		LayoutBlock	block;

		internal LayoutLockBlockEntry(LayoutBlock block, LockBlockOptions options) {
			this.block = block;
			this.options = options;
		}

        public LayoutBlock Block => block;

        public LayoutBlockDefinitionComponent BlockDefinition => block.BlockDefinintion;

        public ResourceCollection Resources => BlockDefinition.Info.Resources;

        public LockBlockOptions Options => options;

        public bool ForceRedSignal {
			get {
				return (options & LockBlockOptions.ForceRedSignal) != 0;
			}

			set {
				if(value)
					options |= LockBlockOptions.ForceRedSignal;
				else
					options &= ~LockBlockOptions.ForceRedSignal;
			}
		}

		static public string GetDescription(Guid resourceId) {
			LayoutBlock	block;

			if(LayoutModel.Blocks.TryGetValue(resourceId, out block))
				return "Block: " + block.Name;
			else {
				var component = LayoutModel.Component<ModelComponent>(resourceId, LayoutModel.ActivePhases);

				if(component != null)
					return "Component: " + component.FullDescription;
				else
					return "<Unknown resource!!>";
			}
		}

        public string GetDescription() => GetDescription(block.Id);
    };

    #region ResourceEntry dictionary

    /// <summary>
    /// In addition to track blocks, each lock request may specify additional resources that must be ready in order for the lock to be granted
    /// The resources are freed when all blocks in the lock request that specified them are no longer locked
    /// </summary>
    public class ResourceUseCountMap : Dictionary<Guid, int> {

    }

	public class LayoutLockBlockEntryDictionary : Dictionary<Guid, LayoutLockBlockEntry>, IEnumerable<LayoutLockBlockEntry> {
        // Map with resources needed by blocks in this lock request. The value is the number of blocks in the request that needs this resource
        ResourceUseCountMap resourceUseCount = new ResourceUseCountMap();

		public void Add(LayoutBlock block, LayoutLockBlockEntry.LockBlockOptions options = LayoutLockBlockEntry.LockBlockOptions.None) {
			this[block.Id] = new LayoutLockBlockEntry(block, options);
            foreach (var resourceInfo in block.BlockDefinintion.Info.Resources)
                addResource(resourceInfo.ResourceId);
        }

        public ResourceUseCountMap ResourcesUseCount => resourceUseCount;

		public void Add(IEnumerable<LayoutBlock> blocks, LayoutLockBlockEntry.LockBlockOptions options = LayoutLockBlockEntry.LockBlockOptions.None) {
			foreach(var block in blocks)
				Add(block, options);
        }

        private Guid addResource(Guid resourceId) {
            if (resourceUseCount.ContainsKey(resourceId))
                resourceUseCount[resourceId]++;
            else
                resourceUseCount[resourceId] = 1;
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
        LayoutLockBlockEntryDictionary blockEntries = new LayoutLockBlockEntryDictionary();
        IList<ILayoutLockResource> resources = null;

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
        public Action OnLockGranted { get; set; }

        public LayoutLockBlockEntryDictionary Blocks => blockEntries;

        public IList<ILayoutLockResource> Resources
        {
            get { return resources; }

            set {
                Trace.Assert(resources == null);
                resources = value;
                blockEntries.AddResources(resources);
            }
        }

        public ResourceUseCountMap ResourceUseCount => blockEntries.ResourcesUseCount;

        public int UnlockedBlocksCount { get; set; }

        public LayoutEvent LockedEvent {
			set {
				OnLockGranted = () => EventManager.Event(value);
			}
		}

        public bool IsManualDispatchLock => Type == LayoutLockType.ManualDispatch;

        #endregion

        #region Operations

        public void Dump() {
			bool		first = true;

			Trace.Write("== BEGIN Lock Request (" + Status.ToString() + "), owner: ");
			
			TrainStateInfo	train = LayoutModel.StateManager.Trains[OwnerId];

			if(train != null)
				Trace.Write("train " + train.DisplayName);
			else {
				ManualDispatchRegionInfo	manualDispatchRegion = LayoutModel.StateManager.ManualDispatchRegions[OwnerId];

				if(manualDispatchRegion != null)
					Trace.Write("manual dispatch region " + manualDispatchRegion.Name);
				else
					Trace.Write("ownerID " + OwnerId.ToString());
			}

			Trace.Write(" for ");

			foreach(LayoutLockBlockEntry blockEntry in Blocks) {
                LayoutBlock block = blockEntry.Block;

                Trace.Write($"{(first ? "" : ", ")}{(block.BlockDefinintion == null ? "UNNAMED" : block.BlockDefinintion.FullDescription)}");
				first = false;
			}

			Trace.WriteLine("");

            if (ResourceUseCount.Count > 0) {
                Trace.Write(" Needed Resources ");
                first = true;

                foreach (var resourceId in ResourceUseCount.Keys) {
                    Trace.Write($"{(first ? "" : ", ")} {LayoutModel.Component<ModelComponent>(resourceId, LayoutPhase.All).FullDescription}={ResourceUseCount[resourceId]}");
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

	class TrainMotionListEntry {
		public LocomotiveTrackingResult	TrackingResult;
		public int						PreviousCrossingSpeed;

		public TrainMotionListEntry(LocomotiveTrackingResult trackingResult, int previousCrossingSpeed) {
			this.TrackingResult = trackingResult;
			this.PreviousCrossingSpeed = previousCrossingSpeed;
		}
	}

	public class TrainMotionListManager {
		List<TrainMotionListEntry> motions = new List<TrainMotionListEntry>();
		Dictionary<Guid, int> trainSpeeds = new Dictionary<Guid, int>();

		public void Add(LocomotiveTrackingResult trackingResult) {
			TrainStateInfo				train = trackingResult.Train;

			motions.Add(new TrainMotionListEntry(trackingResult, train.LastBlockEdgeCrossingSpeed));

			if(!trainSpeeds.ContainsKey(trackingResult.TrainId))
				trainSpeeds.Add(train.Id, train.LastBlockEdgeCrossingSpeed);

			if(train.Managed)
				train.LastBlockEdgeCrossingSpeed = train.Speed;

			TrainPart	fromLocationTrainPart = train.LocationOfBlock(trackingResult.FromBlock).TrainPart;

			train.EnterBlock(fromLocationTrainPart,	trackingResult.ToBlock, trackingResult.BlockEdge, null);
		}

		public void Do() {
			foreach(TrainMotionListEntry motionListEntry in motions) {
				LocomotiveTrackingResult trackingResult = motionListEntry.TrackingResult;

				EventManager.Event(new LayoutEvent(trackingResult.Train, "train-enter-block", null, trackingResult.ToBlock));
				EventManager.Event(new LayoutEvent(trackingResult.Train, "train-crossed-block", null, trackingResult.BlockEdge));
                if (trackingResult.BlockEdge is LayoutBlockEdgeComponent) {
                    EventManager.Event(new LayoutEvent((LayoutBlockEdgeComponent)trackingResult.BlockEdge,
                        "occupancy-block-edge-crossed", null, trackingResult.Train));
                }
			}
		}

		public void Undo() {
			motions.Reverse();

			foreach(TrainMotionListEntry motionListEntry in motions) {
				LocomotiveTrackingResult	trackingResult = motionListEntry.TrackingResult;

				trackingResult.Train.LeaveBlock(trackingResult.ToBlock, false);
			}

			foreach(KeyValuePair<Guid, int> d in trainSpeeds) {
				Guid				trainID = d.Key;
				int					speed = d.Value;
				TrainStateInfo		train = LayoutModel.StateManager.Trains[trainID];

				if(train != null)
					train.LastBlockEdgeCrossingSpeed = speed;
			}
		}
	}

	public class LocomotiveTrackingResult : LayoutObject {
		TrainLocationInfo		trainLocation;
		TrainMotionListManager	motionListManager;

		public LocomotiveTrackingResult(LayoutBlockEdgeBase blockEdge, TrainStateInfo trainState, 
			TrainLocationInfo trainLocation, LayoutBlock fromBlock, LayoutBlock toBlock) {
			initialize(blockEdge, trainState, trainLocation, fromBlock, toBlock, null);
		}

		public LocomotiveTrackingResult(LayoutBlockEdgeBase blockEdge, TrainStateInfo trainState, 
			TrainLocationInfo trainLocation, LayoutBlock fromBlock, LayoutBlock toBlock, TrainMotionListManager motionListManager) {
			initialize(blockEdge, trainState, trainLocation, fromBlock, toBlock, motionListManager);
		}

		private void initialize(LayoutBlockEdgeBase blockEdge, TrainStateInfo trainState, 
			TrainLocationInfo trainLocation, LayoutBlock fromBlock, LayoutBlock toBlock, TrainMotionListManager motionListManager) {

			XmlDocument.LoadXml("<TrackingResult />");

			BlockEdge = blockEdge;
			Train = trainState;
			FromBlock = fromBlock;
			ToBlock = toBlock;
			this.motionListManager = motionListManager;

			this.trainLocation = trainLocation;
		}

		public Guid BlockEdgeId {
			get {
				return XmlConvert.ToGuid(Element.GetAttribute("BlockEdgeID"));
			}

			set {
				Element.SetAttribute("BlockEdgeID", XmlConvert.ToString(value));
			}
		}

		public LayoutBlockEdgeBase BlockEdge {
			get {
				return LayoutModel.Component<LayoutBlockEdgeBase>(BlockEdgeId, LayoutModel.ActivePhases);
			}

			set {
				BlockEdgeId = value.Id;
			}
		}

		public Guid TrainId {
			get {
				return XmlConvert.ToGuid(Element.GetAttribute("TrainID"));
			}

			set {
				Element.SetAttribute("TrainID", XmlConvert.ToString(value));
			}
		}

		public TrainStateInfo Train {
			get {
				return LayoutModel.StateManager.Trains[TrainId];
			}

			set {
				TrainId = value.Id;
			}
		}

        public TrainLocationInfo TrainLocation => trainLocation;

        public Guid FromBlockId {
			get {
				return XmlConvert.ToGuid(Element.GetAttribute("FromBlockID"));
			}

			set {
				Element.SetAttribute("FromBlockID", XmlConvert.ToString(value));
			}
		}

		public LayoutBlock FromBlock {
			get {
				return LayoutModel.Blocks[FromBlockId];
			}

			set {
				FromBlockId = value.Id;
			}
		}

		public Guid ToBlockId {
			get {
				return XmlConvert.ToGuid(Element.GetAttribute("ToBlockID"));
			}

			set {
				Element.SetAttribute("ToBlockID", XmlConvert.ToString(value));
			}
		}

		public LayoutBlock ToBlock {
			get {
				return LayoutModel.Blocks[ToBlockId];
			}

			set {
				ToBlockId = value.Id;
			}
		}

		public TrainMotionListManager MotionListManager {
			get {
				return motionListManager;
			}

			set {
				motionListManager = value;
			}
		}

		public void Commit() {
			if(motionListManager != null)
				motionListManager.Do();
		}

		public void Rollback() {
			if(motionListManager != null)
				MotionListManager.Undo();
		}

		public void Dump() {
			Trace.WriteLine("Tracking result for train " + Train.DisplayName + "(Speed: " + Train.Speed + ")\n  from: " + FromBlock.BlockDefinintion.FullDescription + "\n  to: " + ToBlock.BlockDefinintion.FullDescription);
			Trace.WriteLine("  Crossing edge: " + BlockEdge.FullDescription);
		}
	}

	public class TrainChangingBlock {
		LayoutBlockEdgeBase _blockEdge;
		TrainStateInfo _train;
		LayoutBlock _fromBlock;
		LayoutBlock _toBlock;

		public TrainChangingBlock(LayoutBlockEdgeBase blockEdge, TrainStateInfo train, LayoutBlock fromBlock, LayoutBlock toBlock) {
			_blockEdge = blockEdge;
			_train = train;
			_fromBlock = fromBlock;
			_toBlock = toBlock;
		}

        public LayoutBlockEdgeBase BlockEdge => _blockEdge;

        public TrainStateInfo Train => _train;

        public LayoutBlock FromBlock => _fromBlock;

        public LayoutBlock ToBlock => _toBlock;
    }

	#endregion
}
