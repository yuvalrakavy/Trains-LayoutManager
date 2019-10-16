using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

using LayoutManager.Model;

#nullable enable
#pragma warning disable RCS1163
namespace LayoutManager.Components {
    #region Block Edge base classes

    public enum LayoutSignalState {
        Red, Yellow, Green
    };

    /// <summary>
    /// Signal that is linked to block edge
    /// </summary>
    public class LinkedSignalInfo : LayoutInfo {
        private const string A_SignalId = "SignalID";

        internal LinkedSignalInfo(XmlElement element) : base(element) {
        }

        internal LinkedSignalInfo(XmlElement linkedSignalsElement, LayoutSignalComponent signalComponent) : base(linkedSignalsElement.OwnerDocument.CreateElement("LinkedSignal")) {
            SignalId = signalComponent.Id;
        }

        public Guid SignalId {
            get => (Guid)Element.AttributeValue(A_SignalId);
            set => SetAttributeValue(A_SignalId, value);
        }
    }

    /// <summary>
    /// Collection of all signals that are linked to a block edge
    /// </summary>
    public class LinkedSignalsCollection : XmlIndexedCollection<LinkedSignalInfo, Guid>, ICollection<LinkedSignalInfo> {
        private readonly LayoutBlockEdgeBase blockEdge;

        public LinkedSignalsCollection(LayoutBlockEdgeBase blockEdge) : base(blockEdge.LinkedSignalsElement) {
            this.blockEdge = blockEdge;
        }

        protected override Guid GetItemKey(LinkedSignalInfo item) => item.SignalId;

        protected override XmlElement CreateElement(LinkedSignalInfo item) {
            throw new NotImplementedException();
        }

        protected override LinkedSignalInfo FromElement(XmlElement itemElement) => new LinkedSignalInfo(itemElement);

        public LinkedSignalInfo Add(LayoutSignalComponent signalComponent) {
            LinkedSignalInfo newLinkedSignal = new LinkedSignalInfo(blockEdge.LinkedSignalsElement, signalComponent);

            Add(newLinkedSignal);
            EventManager.Event(new LayoutEvent("signal-component-linked", blockEdge, signalComponent));
            return newLinkedSignal;
        }

        public void Remove(LayoutSignalComponent signalComponent) {
            EventManager.Event(new LayoutEvent("signal-component-unlinked", blockEdge, signalComponent));
            Remove(signalComponent.Id);
        }
    }

    /// <summary>
    /// Abstract base class for components that act as boundries to blocks. A block boundry function as logical signal
    /// for trains that specify whether the train can move between the two blocks. The logical signal can optionally be
    /// linked with a physical signal component
    /// </summary>
    public abstract class LayoutBlockEdgeBase : ModelComponent, IModelComponentHasId, IModelComponentHasAttributes {
        private const string A_State = "State";

        public override ModelComponentKind Kind => ModelComponentKind.BlockEdge;

        public LayoutStraightTrackComponent? OptionalTrack => this.Spot.Track as LayoutStraightTrackComponent;

        public LayoutStraightTrackComponent Track => Ensure.NotNull<LayoutStraightTrackComponent>(OptionalTrack, "track");

        public XmlElement LinkedSignalsElement {
            get {
                XmlElement linkedSignalsElement = Element["LinkedSignals"];

                if (linkedSignalsElement == null) {
                    linkedSignalsElement = Element.OwnerDocument.CreateElement("LinkedSignals");
                    Element.AppendChild(linkedSignalsElement);
                }

                return linkedSignalsElement;
            }
        }

        public LinkedSignalsCollection LinkedSignals => new LinkedSignalsCollection(this);

        public LayoutSignalState SignalState {
            get => LayoutModel.StateManager.Components.StateOf(this.Id, "Signal").AttributeValue(A_State).Enum<LayoutSignalState>() ?? LayoutSignalState.Yellow;

            [LayoutEventDef("logical-signal-state-changed", Role = LayoutEventRole.Notification, SenderType = typeof(LayoutBlockEdgeBase), InfoType = typeof(LayoutSignalState))]
            set {
                LayoutModel.StateManager.Components.StateOf(this, "Signal", create: true).SetAttributeValue(A_State, value);
                Redraw();
                EventManager.Event(new LayoutEvent("logical-signal-state-changed", this, value));
            }
        }

        public void RemoveSignalState() {
            LayoutModel.StateManager.Components.Remove(this.Id, "Signal");
            Redraw();
            EventManager.Event(new LayoutEvent("signal-state-removed", this));
        }

        /// <summary>
        /// Get the block that is the neighbor of a given block
        /// </summary>
        /// <param name="block">The block for which you want to get the neighbor block</param>
        /// <returns>The neighbor block</returns>
        public LayoutBlock GetNeighboringBlock(LayoutBlock block) {
            var track = this.Track;

            if (track != null) {
                if (track.GetBlock(track.ConnectionPoints[0]) == block)
                    return track.GetBlock(track.ConnectionPoints[1]);
                else if (track.GetBlock(track.ConnectionPoints[1]) == block)
                    return track.GetBlock(track.ConnectionPoints[0]);
            }

            throw new ArgumentException(FullDescription + " is not boundry of block " + block.BlockDefinintion.FullDescription);
        }

        /// <summary>
        /// Get the track edge for a given block
        /// </summary>
        /// <param name="block">The block for which to get the track edge</param>
        /// <returns>The track edge</returns>
        public TrackEdge GetBlockTrackEdge(LayoutBlock block) {
            var track = this.Track;

            if (track != null) {
                if (track.GetBlock(track.ConnectionPoints[0]) == block)
                    return new TrackEdge(Track, track.ConnectionPoints[0]);
                else if (track.GetBlock(track.ConnectionPoints[1]) == block)
                    return new TrackEdge(Track, track.ConnectionPoints[1]);
            }

            throw new ArgumentException(FullDescription + " is not boundry of block " + block.BlockDefinintion.FullDescription);
        }

        /// <summary>
        /// Return true if this block edge is a track contact, which is connected to control module (i.e. it will be triggered)
        /// </summary>
        /// <returns>True - this component is an active track contact</returns>
        public virtual bool IsTrackContact() => false;
    }

    #endregion

    #region LayoutTriggerableBlockEdgeBase

    public abstract class LayoutTriggerableBlockEdgeBase : LayoutBlockEdgeBase, IModelComponentHasName, IModelComponentConnectToControl {
        private const string Topic_TriggeredState = "Triggered";

        public LayoutTextInfo NameProvider => new LayoutTextInfo(this);

        public abstract IList<ModelComponentControlConnectionDescription> ControlConnectionDescriptions { get; }

        public LayoutAddressInfo AddressProvider => new LayoutAddressInfo(this);

        public override bool DrawOutOfGrid {
            get {
                LayoutTextInfo text = new LayoutTextInfo(this);

                return text.OptionalElement != null;
            }
        }

        public const string A_EmergencyContact = "EmergencyContact";
        private const string A_Value = "Value";

        public bool IsEmergencySensor {
            get => (bool?)Element.AttributeValue(A_EmergencyContact) ?? false;
            set => Element.SetAttributeValue(A_EmergencyContact, value, removeIf: false);
        }

        public bool IsTriggered {
            get => LayoutModel.StateManager.Components.Contains(this, Topic_TriggeredState);

            set {
                if (value != IsTriggered) {
                    if (value)
                        LayoutModel.StateManager.Components.StateOf(this, Topic_TriggeredState, create: true).SetAttribute(A_Value, "True");
                    else
                        LayoutModel.StateManager.Components.Remove(this, Topic_TriggeredState);

                    EventManager.Event("block-edge-sensor-changed", this, value);
                    EventManager.Event(value ? "block-edge-sensor-active" : "block-edge-sensor-not-active", this);
                    OnComponentChanged();
                }
            }
        }
    }

    #endregion

    #region Track Contact component

    /// <summary>
    /// Track contact component is a component that is triggered when a locomotive (or car)
    /// equiped with a track magnet passes over it
    /// </summary>
    public class LayoutTrackContactComponent : LayoutTriggerableBlockEdgeBase {
        private static readonly IList<ModelComponentControlConnectionDescription> controlConnections = Array.AsReadOnly<ModelComponentControlConnectionDescription>(
            new ModelComponentControlConnectionDescription[] { new ModelComponentControlConnectionDescription(ControlConnectionPointTypes.InputDryTrigger, "TrackContact", "track contact feedback") });

        public LayoutTrackContactComponent() {
            this.XmlInfo.XmlDocument.LoadXml("<TrackContact/>");
        }

        public override string ToString() => IsEmergencySensor ? "emergency track contact" : "track contact";

        public override bool IsTrackContact() => FullyConnected;

        public override IList<ModelComponentControlConnectionDescription> ControlConnectionDescriptions => controlConnections;
    }

    #endregion

    #region Block Edge component

    public class LayoutBlockEdgeComponent : LayoutBlockEdgeBase, IObjectHasId {
        public LayoutBlockEdgeComponent() {
            XmlInfo.XmlDocument.LoadXml("<OccupancyDetectionBlockEdge />");
        }

        public override string ToString() => "edge of a block";

        public override bool DrawOutOfGrid => false;
    }

    #endregion

    #region LayoutProximitySensorComponent

    /// <summary>
    /// Track contact component is a component that is triggered when a locomotive (or car)
    /// equiped with a track magnet passes over it
    /// </summary>
    public class LayoutProximitySensorComponent : LayoutTriggerableBlockEdgeBase {
        private static readonly IList<ModelComponentControlConnectionDescription> controlConnections = Array.AsReadOnly<ModelComponentControlConnectionDescription>(
            new ModelComponentControlConnectionDescription[] { new ModelComponentControlConnectionDescription("Level", "ProximitySensor", "proximity sensor feedback") });

        public LayoutProximitySensorComponent() {
            this.XmlInfo.XmlDocument.LoadXml("<ProximitySensor/>");
        }

        public override string ToString() => IsEmergencySensor ? "emergency proximity sensor" : "proximity sensor";

        public override bool IsTrackContact() => FullyConnected;

        public override IList<ModelComponentControlConnectionDescription> ControlConnectionDescriptions => controlConnections;
    }

    #endregion

    #region Block definition component

    #region Property type classes (ResourceInfo and ResourceCollection)

    /// <summary>
    /// Resource that is associated with the block
    /// </summary>
    public class ResourceInfo : LayoutXmlWrapper {
        private const string A_ResourceID = "ResourceID";

        public ResourceInfo(XmlElement element) : base(element) {
        }

        public Guid ResourceId {
            get => (Guid)AttributeValue(A_ResourceID);
            set => SetAttributeValue(A_ResourceID, value);
        }

        public IModelComponentLayoutLockResource? GetResource(LayoutPhase phase) => LayoutModel.Component<IModelComponentLayoutLockResource>(ResourceId, phase);
    }

    /// <summary>
    /// Collection of all resources associated with block
    /// </summary>
    public class ResourceCollection : XmlIndexedCollection<ResourceInfo, Guid>, ICollection<ResourceInfo> {
        private readonly LayoutBlockDefinitionComponentInfo blockDefinition;

        public ResourceCollection(LayoutBlockDefinitionComponentInfo blockDefinition) : base(blockDefinition.ResourcesElement) {
            this.blockDefinition = blockDefinition;
        }

        protected override XmlElement CreateElement(ResourceInfo item) {
            throw new NotImplementedException();
        }

        protected override ResourceInfo FromElement(XmlElement itemElement) => new ResourceInfo(itemElement);

        protected override Guid GetItemKey(ResourceInfo item) => item.ResourceId;

        public ResourceInfo Add(Guid resourceId) {
            XmlElement resourceElement = blockDefinition.Element.OwnerDocument.CreateElement("Resource");
            ResourceInfo resourceInfo = new ResourceInfo(resourceElement) {
                ResourceId = resourceId
            };
            Add(resourceInfo);

            return resourceInfo;
        }

        public ResourceInfo Add(ILayoutLockResource resource) => Add(resource.Id);

        public bool CheckIntegrity(LayoutModuleBase moduleBase, LayoutPhase phase) {
            bool ok = true;
            ArrayList removeList = new ArrayList();

            foreach (ResourceInfo resourceInfo in this) {
                if (resourceInfo.GetResource(phase) == null) {
                    if (resourceInfo.GetResource(LayoutPhase.All) == null) {
                        LayoutModuleBase.Warning(blockDefinition.BlockDefinition, "Resource of this block cannot be found - resource removed");
                        removeList.Add(resourceInfo);
                    }
                    else
                        LayoutModuleBase.Error(blockDefinition.BlockDefinition, "Resource required by this block is marked either as 'Planned' or 'In constuction' phase");

                    ok = false;
                }
            }

            foreach (ResourceInfo resourceInfo in removeList)
                Remove(resourceInfo.ResourceId);

            return ok;
        }
    }

    #endregion

    public class LayoutBlockDefinitionComponentInfo : LayoutInfo {
        private const string A_SuggestForPlacement = "SuggestForPlacement";
        private const string A_SuggestForDestination = "SuggestForDestination";
        private const string A_SuggestForProgramming = "SuggestForProgramming";
        private const string A_TrainLengthLimit = "TrainLengthLimit";
        private const string A_NoFeedback = "NoFeedback";
        private const string A_SlowDownRegion = "SlowDownRegion";
        private const string A_IsWaitable = "IsWaitable";
        private const string A_OccupancyDetectionBlock = "OccupancyDetectionBlock";
        private const string A_Length = "Length";
        private const string A_SpeedLimit = "SpeedLimit";
        private const string A_SlowDownSpeed = "SlowDownSpeed";
        private const string E_Resources = "Resources";
        private const string E_Policies = "Policies";
        private const string A_TrainDetected = "TrainDetected";
        private const string A_TrainWillBeDetected = "TrainWillBeDetected";

        public LayoutBlockDefinitionComponentInfo(LayoutBlockDefinitionComponent blockDefinition, XmlElement element) : base(element) {
            this.BlockDefinition = blockDefinition;
        }

        public LayoutBlockDefinitionComponent BlockDefinition { get; }

        public bool SuggestForPlacement {
            get => (bool?)AttributeValue(A_SuggestForPlacement) ?? false;
            set => SetAttributeValue(A_SuggestForPlacement, value);
        }

        public bool SuggestForDestination {
            get => (bool?)AttributeValue(A_SuggestForDestination) ?? false;
            set => SetAttributeValue(A_SuggestForDestination, value, removeIf: false);
        }

        public bool SuggestForProgramming {
            get => (bool?)AttributeValue(A_SuggestForProgramming) ?? false;
            set => SetAttributeValue(A_SuggestForProgramming, value, removeIf: false);
        }

        public TrainLength TrainLengthLimit {
            get => AttributeValue(A_TrainLengthLimit).ToTrainLength() ?? TrainLength.VeryLong;
            set => SetAttribute(A_TrainLengthLimit, value, removeIf: TrainLength.VeryLong);
        }

        public bool NoFeedback {
            get => (bool?)AttributeValue(A_NoFeedback) ?? false;
            set => SetAttributeValue(A_NoFeedback, value, removeIf: false);
        }

        public bool IsSlowdownRegion {
            get => (bool?)AttributeValue(A_SlowDownRegion) ?? false;
            set => SetAttributeValue(A_SlowDownRegion, value, removeIf: false);
        }

        public bool UseDefaultCanTrainWait {
            get => !Element.HasAttribute(A_IsWaitable);

            set {
                if (value)
                    Element.RemoveAttribute(A_IsWaitable);
                else
                    Element.SetAttributeValue(A_IsWaitable, BlockDefinition.Block != null ? BlockDefinition.Block.CanTrainWaitDefault : value);
            }
        }

        public bool CanTrainWait {
            get => (bool?)AttributeValue(A_IsWaitable) ?? BlockDefinition.Block.CanTrainWaitDefault;
            set => Element.SetAttributeValue(A_IsWaitable, value);
        }

        public bool IsOccupancyDetectionBlock {
            get => (bool?)AttributeValue(A_OccupancyDetectionBlock) ?? false;
            set => SetAttributeValue(A_OccupancyDetectionBlock, value);
        }

        public LayoutAddressInfo? AddressProvider => Element["Address"] != null ? new LayoutAddressInfo(BlockDefinition) : null;

        public double Length {
            get => (double?)AttributeValue(A_Length) ?? 100.0;
            set => SetAttributeValue(A_Length, value);
        }

        public int LengthInCM => (int)Length;

        public int SpeedLimit {
            get => (int?)AttributeValue(A_SpeedLimit) ?? 0;

            set {
                SetAttributeValue(A_SpeedLimit, value, removeIf: 0);
                RefreshSpeedLimit();
            }
        }

        public int SlowdownSpeed {
            get => (int?)AttributeValue(A_SlowDownSpeed) ?? 0;

            set {
                SetAttributeValue(A_SlowDownSpeed, value, removeIf: 0);
                RefreshSpeedLimit();
            }
        }

        public void RefreshSpeedLimit() {
            if (BlockDefinition != null && BlockDefinition.Block != null) {
                foreach (TrainLocationInfo trainLocation in BlockDefinition.Block.Trains)
                    trainLocation.Train.RefreshSpeedLimit();
            }
        }

        public XmlElement ResourcesElement {
            get {
                XmlElement resourcesElement = Element[E_Resources];

                if (resourcesElement == null) {
                    resourcesElement = Element.OwnerDocument.CreateElement(E_Resources);
                    Element.AppendChild(resourcesElement);
                }

                return resourcesElement;
            }
        }

        public XmlElement PoliciesElement {
            get {
                XmlElement policiesElement = Element[E_Policies];

                if (policiesElement == null) {
                    policiesElement = Element.OwnerDocument.CreateElement(E_Policies);
                    Element.AppendChild(policiesElement);
                }

                return policiesElement;
            }
        }

        public bool IsTripSectionBoundry(int cpIndex) => (bool?)AttributeValue($"TripSectonBoundry{cpIndex}") ?? false;

        public void SetTripSectionBoundry(int cpIndex, bool isBoundry) => SetAttributeValue($"TripSectonBoundry{cpIndex}", isBoundry, removeIf: false);

        /// <summary>
        /// Set the state of the train detected state. Please note that this method just set the attribute
        /// it does not generate events.
        /// </summary>
        /// <param name="detected">Train detection status</param>
        public void SetTrainDetected(bool detected) {
            var trainDetectionElement = LayoutModel.StateManager.Components.StateOf(BlockDefinition, "TrainDetection", create: true);

            trainDetectionElement.SetAttributeValue(A_TrainDetected, detected);
        }

        public void SetTrainWillBeDetected(bool detected) {
            var trainDetectionElement = LayoutModel.StateManager.Components.StateOf(BlockDefinition, "TrainDetection", create: true);

            trainDetectionElement.SetAttributeValue(A_TrainWillBeDetected, detected);
        }

        public bool TrainWillBeDetected {
            get {
                if (IsOccupancyDetectionBlock)
                    return (bool?)LayoutModel.StateManager.Components.StateOf(BlockDefinition.Id, "TrainDetection").AttributeValue(A_TrainWillBeDetected) ?? false;
                else {
                    LayoutBlock block = BlockDefinition.Block;

                    if (block.OccupancyBlock == null)
                        return block.HasTrains;
                    else {
                        LayoutBlockDefinitionComponent occupancyBlockInfo = block.OccupancyBlock.BlockDefinintion;

                        Debug.Assert(occupancyBlockInfo.Info.IsOccupancyDetectionBlock);
                        return occupancyBlockInfo.Info.TrainWillBeDetected;
                    }
                }
            }

            set {
                if (IsOccupancyDetectionBlock) {
                    if (value != TrainWillBeDetected) {
                        var occupancyBlock = BlockDefinition.Block.OccupancyBlock;

                        if (occupancyBlock != null) {
                            SetTrainWillBeDetected(value);

                            if (!LayoutController.TrainsAnalysisPhase) {
                                if (value)
                                    EventManager.Event(new LayoutEvent("train-detection-block-will-be-occupied", occupancyBlock));
                                else
                                    EventManager.Event(new LayoutEvent("train-detection-block-will-be-free", occupancyBlock));
                            }
                        }
                    }
                }
                else {
                    var occupancyBlock = BlockDefinition.Block.OccupancyBlock;

                    if (occupancyBlock != null) {
                        LayoutBlockDefinitionComponent occupancyBlockInfo = occupancyBlock.BlockDefinintion;

                        Debug.Assert(occupancyBlockInfo.Info.IsOccupancyDetectionBlock);
                        occupancyBlockInfo.Info.TrainWillBeDetected = value;
                    }
                }
            }
        }

        /// <summary>
        /// Return true if a train is detected in this block. This is valid only if the block is or part of train occupancy
        /// detection block.
        /// </summary>
        public bool TrainDetected {
            get {
                if (IsOccupancyDetectionBlock)
                    return (bool?)LayoutModel.StateManager.Components.StateOf(BlockDefinition.Id, "TrainDetection").AttributeValue(A_TrainDetected) ?? false;
                else {
                    LayoutBlock block = BlockDefinition.Block;

                    if (block.OccupancyBlock == null)
                        return block.HasTrains;
                    else {
                        LayoutBlockDefinitionComponent occupancyBlockInfo = block.OccupancyBlock.BlockDefinintion;

                        Debug.Assert(occupancyBlockInfo.Info.IsOccupancyDetectionBlock);
                        return occupancyBlockInfo.Info.TrainDetected;
                    }
                }
            }

            [LayoutEventDef("train-detection-block-occupied", Role = LayoutEventRole.Notification, SenderType = typeof(LayoutOccupancyBlock))]
            [LayoutEventDef("train-detection-block-free", Role = LayoutEventRole.Notification, SenderType = typeof(LayoutOccupancyBlock))]
            [LayoutEventDef("train-detection-block-will-be-occupied", Role = LayoutEventRole.Notification, SenderType = typeof(LayoutOccupancyBlock))]
            [LayoutEventDef("train-detection-block-will-be-free", Role = LayoutEventRole.Notification, SenderType = typeof(LayoutOccupancyBlock))]
            set {
                BlockDefinition.EraseImage();

                if (IsOccupancyDetectionBlock) {
                    if (value != TrainDetected) {
                        var occupancyBlock = BlockDefinition.Block.OccupancyBlock;

                        Debug.Assert(occupancyBlock != null);
                        TrainWillBeDetected = value;

                        SetTrainDetected(value);

                        if (!LayoutController.TrainsAnalysisPhase) {
                            if (value)
                                EventManager.Event(new LayoutEvent("train-detection-block-occupied", occupancyBlock));
                            else
                                EventManager.Event(new LayoutEvent("train-detection-block-free", occupancyBlock));
                        }
                    }
                }
                else {
                    var occupancyBlock = BlockDefinition.Block.OccupancyBlock;

                    if (occupancyBlock != null) {
                        LayoutBlockDefinitionComponent occupancyBlockInfo = occupancyBlock.BlockDefinintion;

                        Debug.Assert(occupancyBlockInfo.Info.IsOccupancyDetectionBlock);
                        occupancyBlockInfo.Info.TrainDetected = value;
                    }
                }

                BlockDefinition.Redraw();
            }
        }

        public bool UnexpectedTrainDetected {
            get {
                if (!IsOccupancyDetectionBlock || !TrainDetected)
                    return false;
                else {
                    if (BlockDefinition.Block.OccupancyBlock != null) {
                        // This is a train occupancy block and a train is detected.
                        // if all logical blocks do not contain any train, then this train is unexpected
                        foreach (LayoutBlock block in BlockDefinition.Block.OccupancyBlock.ContainedBlocks)
                            if (block.HasTrains)
                                return false;
                    }
                    return true;
                }
            }
        }

        public ResourceCollection Resources => new ResourceCollection(this);

        public LayoutPolicyIdCollection Policies => new LayoutPolicyIdCollection(PoliciesElement);

        public TripPlanTrainConditionInfo TrainPassCondition => new TripPlanTrainConditionInfo(Element, "TrainPassCondition");

        public TripPlanTrainConditionInfo TrainStopCondition => new TripPlanTrainConditionInfo(Element, "TrainStopCondition");
    }

    public class LayoutBlockDefinitionComponent : ModelComponent, IObjectHasId, IModelComponentHasName, IObjectHasAttributes, IModelComponentConnectToControl {
        private List<LayoutBlockEdgeBase>[] blockEdges;

        private static readonly IList<ModelComponentControlConnectionDescription> controlConnectionsOfOccupancyBlock = Array.AsReadOnly<ModelComponentControlConnectionDescription>(
          new ModelComponentControlConnectionDescription[] {
            new ModelComponentControlConnectionDescription(ControlConnectionPointTypes.Level, "FeedbackBlock", "block occupied feedback")
        });

        private static readonly IList<ModelComponentControlConnectionDescription> controlConnectionsOfNormalBlock = new List<ModelComponentControlConnectionDescription>().AsReadOnly();

        public LayoutBlockDefinitionComponent() {
            blockEdges = Array.Empty<List<LayoutBlockEdgeBase>>();
            XmlDocument.LoadXml("<BlockInfo />");
        }

        public override ModelComponentKind Kind => ModelComponentKind.BlockInfo;

        public LayoutTextInfo NameProvider => new LayoutTextInfo(this);

        public override bool DrawOutOfGrid => NameProvider.Element != null && NameProvider.Visible;

        public string Name => NameProvider.Name;

        public LayoutBlockDefinitionComponentInfo Info => new LayoutBlockDefinitionComponentInfo(this, Element);

        public LayoutTrackComponent Track => Spot.Track;

        public LayoutBlock Block => Track.GetBlock(Track.ConnectionPoints[0]);

        public LayoutBlock? OptionalBlock => Track.GetOptionalBlock(Track.ConnectionPoints[0]);

        public LayoutTrackPowerConnectorComponent PowerConnector {
            get {
                var powerConnector = Track.GetPowerConnector(Track.ConnectionPoints[0]);

                return powerConnector ?? throw new LayoutException($"Block {FullDescription} has no power connector");
            }
        }

        public ILayoutPower Power => Track.GetPower(Track.ConnectionPoints[0]);

        public TrackGauges Guage => PowerConnector.Info.TrackGauge;

        public void ClearBlockEdges() {
            if (blockEdges.Length != Track.ConnectionPoints.Count) {
                blockEdges = new List<LayoutBlockEdgeBase>[Track.ConnectionPoints.Count];

                for (int i = 0; i < blockEdges.Length; i++)
                    blockEdges[i] = new List<LayoutBlockEdgeBase>();
            }

            for (int cpIndex = 0; cpIndex < blockEdges.Length; cpIndex++)
                blockEdges[cpIndex].Clear();
        }

        public void AddBlockEdge(int reachableFromConnectionPointIndex, LayoutBlockEdgeBase blockEdge) {
            if (reachableFromConnectionPointIndex < 0 || reachableFromConnectionPointIndex >= blockEdges.Length)
                throw new ArgumentException("Invalid value for reachableFromConnectionPointIndex");
            blockEdges[reachableFromConnectionPointIndex].Add(blockEdge);
        }

        public LayoutBlockEdgeBase[] GetBlockEdges(int reachableFromConnectionPointIndex) => blockEdges[reachableFromConnectionPointIndex].ToArray();

        public bool ContainsBlockEdge(int cpIndex, LayoutBlockEdgeBase blockEdge) => blockEdges[cpIndex].Contains(blockEdge);

        /// <summary>
        /// Get the connection index of a connection point based on the side of the block info component that this connection
        /// point describes
        /// </summary>
        /// <param name="cp">The connection point</param>
        /// <returns>0 or 1 based on side of the block info that is described by the connection point</returns>
        public int GetConnectionPointIndex(LayoutComponentConnectionPoint cp) {
            for (int cpIndex = 0; cpIndex < Track.ConnectionPoints.Count; cpIndex++)
                if (Track.ConnectionPoints[cpIndex] == cp)
                    return cpIndex;

            throw new ArgumentException("BlockInfo " + FullDescription + " does not have the given connection point: " + cp.ToString());
        }

        /// <summary>
        /// Given a block edge, return the connection index (0, 1) of this edge. This is the "side" of the block to which this block edge
        /// borders
        /// </summary>
        /// <param name="blockEdge">The block edge</param>
        /// <returns>0 or 1</returns>
        public int GetConnectionPointIndex(LayoutBlockEdgeBase blockEdge) {
            for (int cpIndex = 0; cpIndex < Track.ConnectionPoints.Count; cpIndex++)
                if (ContainsBlockEdge(cpIndex, blockEdge))
                    return cpIndex;

            throw new ArgumentException("Block edge " + blockEdge.FullDescription + " does not border block " + FullDescription);
        }

        public int GetOtherConnectionPointIndex(LayoutComponentConnectionPoint cp) => GetOtherConnectionPointIndex(GetConnectionPointIndex(cp));

        public int GetOtherConnectionPointIndex(int cpIndex) {
            TrackEdge edge = new TrackEdge(Track, Track.ConnectionPoints[cpIndex]);

            return GetConnectionPointIndex(edge.OtherConnectionPoint);
        }

        public int GetOtherConnectionPointIndex(LayoutBlockEdgeBase blockEdge) => GetOtherConnectionPointIndex(GetConnectionPointIndex(blockEdge));

        public bool ContainsBlockEdge(LayoutComponentConnectionPoint cp, LayoutBlockEdgeBase blockEdge) => ContainsBlockEdge(GetConnectionPointIndex(cp), blockEdge);

        public override string ToString() => "Block information";

        protected void DoRedraw() {
            base.Redraw();
        }

        protected void DoEraseImage() {
            base.EraseImage();
        }

        public override void Redraw() {
            if (!LayoutController.IsOperationMode)
                base.Redraw();
            else {
                LayoutBlock block = Block;

                if (block.OccupancyBlock == null)
                    base.Redraw();
                else {
                    foreach (LayoutBlock containedBlock in block.OccupancyBlock.ContainedBlocks)
                        if (containedBlock.BlockDefinintion != null)
                            containedBlock.BlockDefinintion.DoRedraw();
                }
            }
        }

        public override void EraseImage() {
            if (!LayoutController.IsOperationMode)
                base.EraseImage();
            else {
                LayoutBlock block = Block;

                if (block.OccupancyBlock == null)
                    base.Redraw();
                else {
                    foreach (LayoutBlock containedBlock in block.OccupancyBlock.ContainedBlocks)
                        if (containedBlock.BlockDefinintion != null)
                            containedBlock.BlockDefinintion.DoEraseImage();
                }
            }
        }

        #region IModelComponentConnectToControl Members

        public IList<ModelComponentControlConnectionDescription> ControlConnectionDescriptions => Info.IsOccupancyDetectionBlock ? controlConnectionsOfOccupancyBlock : controlConnectionsOfNormalBlock;

        #endregion

    }

    #endregion

    #region Signal Component

    public enum LayoutSignalType {
        Lights, Semaphore, Distance
    };

    public class LayoutReverseLogicInfo : LayoutInfo {
        private const string A_ReverseLogic = "ReverseLogic";

        public LayoutReverseLogicInfo(XmlElement element)
            : base(element) {
        }

        public bool ReverseLogic {
            get => (bool?)AttributeValue(A_ReverseLogic) ?? false;
            set => SetAttributeValue(A_ReverseLogic, value, removeIf: false);
        }
    }

    public class LayoutSignalComponentInfo : LayoutReverseLogicInfo {
        private const string A_SignalType = "SignalType";

        public LayoutSignalComponentInfo(XmlElement element) : base(element) {
        }

        public LayoutSignalType SignalType {
            get => AttributeValue(A_SignalType).Enum<LayoutSignalType>() ?? LayoutSignalType.Lights;
            set => Element.SetAttribute(A_SignalType, value.ToString());
        }
    }

    public class LayoutSignalComponent : ModelComponent, IModelComponentHasReverseLogic, IModelComponentConnectToControl {
        public LayoutSignalComponent() {
            XmlDocument.LoadXml("<Signal />");
        }

        public override ModelComponentKind Kind => ModelComponentKind.Signal;

        public LayoutAddressInfo AddressProvider => new LayoutAddressInfo(this);

        public LayoutSignalComponentInfo Info => new LayoutSignalComponentInfo(Element);

        public bool ReverseLogic {
            get => Info.ReverseLogic;

            set => Info.ReverseLogic = value;
        }

        public LayoutStraightTrackComponent? Track => Spot.Track as LayoutStraightTrackComponent;

        public override bool DrawOutOfGrid => true;

        /// <summary>
        /// Get or set the signal state
        /// </summary>
        public LayoutSignalState SignalState {
            get => LayoutModel.StateManager.Components.StateOf(this, "SignalState").AttributeValue("Value").Enum<LayoutSignalState>() ?? LayoutSignalState.Yellow;

            set {
                var connectionPoint = LayoutModel.ControlManager.ConnectionPoints[this]?[0];
                var signalState = value;

                if (Info.ReverseLogic)
                    signalState = (signalState == LayoutSignalState.Green) ? LayoutSignalState.Red : LayoutSignalState.Green;

                if (connectionPoint != null)
                    EventManager.Event(new LayoutEvent("change-signal-state-command", new ControlConnectionPointReference(connectionPoint), signalState).SetCommandStation(connectionPoint.Module.Bus));
            }
        }

        /// <summary>
        /// This method actually change the turnout run-time state. It should be called only from the turnout state changed
        /// notification handler, and not directly.
        /// </summary>
        /// <param name="switchState">The new switch state</param>
        public void SetSignalState(LayoutSignalState state) {
            // TODO: Complaint if a switch state is changed not either when there is no lock, or not by lock owner.
            LayoutModel.StateManager.Components.StateOf(this, "SignalState", create: true).SetAttribute("Value", state.ToString());
            OnComponentChanged();
        }

        public override string ToString() => "Signal";

        #region IModelComponentConnectToControl Members

        public IList<ModelComponentControlConnectionDescription> ControlConnectionDescriptions {
            get {
                string friendlyName = Info.SignalType switch
                {
                    LayoutSignalType.Distance => "distance signal control",
                    LayoutSignalType.Lights => "signal control",
                    LayoutSignalType.Semaphore => "semaphore",
                    _ => throw new ArgumentException("Invalid signal type")
                };

                return Array.AsReadOnly<ModelComponentControlConnectionDescription>(new ModelComponentControlConnectionDescription[] {
                    new ModelComponentControlConnectionDescription(ControlConnectionPointTypes.Output, "Signal", friendlyName)
                });
            }
        }

        #endregion
    }

    #endregion

    #region Control Module Location component

    public class LayoutControlModuleLocationComponentInfo : LayoutInfo {
        private const string E_Defaults = "Defaults";
        private const string A_CommandStationId = "CommandStationID";

        public LayoutControlModuleLocationComponentInfo(LayoutControlModuleLocationComponent controlModuleLocation, XmlElement element) : base(element) {
            this.ControlModuleLocation = controlModuleLocation;
        }

        public LayoutControlModuleLocationComponent ControlModuleLocation { get; }

        /// <summary>
        /// Element containing defaults for each bus at this location
        /// </summary>
        public XmlElement DefaultsElement {
            get {
                XmlElement defaultsElement = Element[E_Defaults];

                if (defaultsElement == null) {
                    defaultsElement = Element.OwnerDocument.CreateElement(E_Defaults);
                    Element.AppendChild(defaultsElement);
                }

                return defaultsElement;
            }
        }

        public ControlModuleLocationBusDefaultSettingCollection Defaults => new ControlModuleLocationBusDefaultSettingCollection(DefaultsElement);

        public Guid CommandStationId {
            get => (Guid?)AttributeValue(A_CommandStationId) ?? Guid.Empty;
            set => SetAttributeValue(A_CommandStationId, value, removeIf: Guid.Empty);
        }

        public IModelComponentIsCommandStation? CommandStation {
            get => CommandStationId != Guid.Empty ? LayoutModel.Component<IModelComponentIsCommandStation>(CommandStationId, LayoutModel.ActivePhases) : null;
            set => CommandStationId = value != null ? value.Id : Guid.Empty;
        }
    }

    public class ControlModuleLocationBusDefaultSettingCollection : XmlIndexedCollection<ControlModuleLocationBusDefaultInfo, Guid> {
        public ControlModuleLocationBusDefaultSettingCollection(XmlElement element) : base(element) {
        }

        protected override XmlElement CreateElement(ControlModuleLocationBusDefaultInfo item) => Element.OwnerDocument.CreateElement("BusDefault");

        protected override ControlModuleLocationBusDefaultInfo FromElement(XmlElement itemElement) => new ControlModuleLocationBusDefaultInfo(itemElement);

        protected override Guid GetItemKey(ControlModuleLocationBusDefaultInfo item) => item.BusId;

        public ControlModuleLocationBusDefaultInfo Add(Guid busId, string defaultModuleTypeName, int defaultStartAddress) {
            XmlElement busDefaultElement = Element.OwnerDocument.CreateElement("BusDefault");

            ControlModuleLocationBusDefaultInfo busDefault = new ControlModuleLocationBusDefaultInfo(busDefaultElement) {
                BusId = busId,
                DefaultModuleTypeName = defaultModuleTypeName,
                DefaultStartAddress = defaultStartAddress
            };

            Add(busDefault);
            return busDefault;
        }

        public ControlModuleLocationBusDefaultInfo Add(ControlBus bus, ControlModuleType defaultType, int defaultStartAddress) => Add(bus.Id, defaultType.TypeName, defaultStartAddress);

        public void Remove(ControlBus bus) {
            Remove(bus.Id);
        }
    }
    #endregion

    /// <summary>
    /// Provide defaults for a given bus at a given control module location
    /// </summary>
    public class ControlModuleLocationBusDefaultInfo : LayoutXmlWrapper {
        private const string A_BusId = "BusID";
        private const string A_DefaultModuleTypeName = "DefaultModuleTypeName";
        private const string A_DefaultStartAddress = "DefaultStartAddress";

        public ControlModuleLocationBusDefaultInfo(XmlElement element) : base(element) {
        }

        /// <summary>
        /// The ID of the bus
        /// </summary>
        public Guid BusId {
            get => (Guid)AttributeValue(A_BusId);
            set => SetAttributeValue(A_BusId, value);
        }

        public ControlBus? Bus => LayoutModel.ControlManager.Buses[BusId];

        /// <summary>
        /// The component type name of the default component to add this bus when new module is needed to be added to this location
        /// </summary>
        public string DefaultModuleTypeName {
            get => GetAttribute(A_DefaultModuleTypeName);

            set {
                if (string.IsNullOrEmpty(value))
                    Element.RemoveAttribute(A_DefaultModuleTypeName);
                else
                    SetAttributeValue(A_DefaultModuleTypeName, value);
            }
        }

        public ControlModuleType DefaultModuleType => LayoutControlManager.GetModuleType(DefaultModuleTypeName);

        /// <summary>
        /// Start allocating address from this number when adding components at this location
        /// </summary>
        public int DefaultStartAddress {
            get => (int?)AttributeValue(A_DefaultStartAddress) ?? -1;
            set => SetAttributeValue(A_DefaultStartAddress, value, removeIf: -1);
        }
    }

    public class LayoutControlModuleLocationComponent : ModelComponent, IModelComponentHasId, IModelComponentHasName {
        private LayoutControlModuleLocationComponentInfo? _info;

        public LayoutControlModuleLocationComponent() {
            XmlDocument.LoadXml("<ControlModuleLocation />");
        }

        public override ModelComponentKind Kind => ModelComponentKind.ControlComponent;

        public LayoutTextInfo NameProvider => new LayoutTextInfo(this);

        public string Name => NameProvider.Name;

        public override bool DrawOutOfGrid => NameProvider.Element != null;

        public LayoutControlModuleLocationComponentInfo Info => _info ?? (_info = new LayoutControlModuleLocationComponentInfo(this, Element));
    }
}
