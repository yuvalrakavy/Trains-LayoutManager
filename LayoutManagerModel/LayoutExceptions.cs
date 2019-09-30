using System;
using System.Xml;

using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager {
    public class LayoutLocomotiveException : LayoutException {
        public LayoutLocomotiveException(LocomotiveInfo loco, string message) : base(message) {
            this.Locomotive = loco;
        }

        public LayoutLocomotiveException() : base() {
        }

        public LayoutLocomotiveException(string message) : base(message) {
        }

        public LayoutLocomotiveException(object subject, string message) : base(subject, message) {
        }

        public LayoutLocomotiveException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }

        public LocomotiveInfo Locomotive { get; }
    }

    #region Locomotive Manager Exception classess

    public class LocomotiveHasNoAddressException : LayoutLocomotiveException {
        public LocomotiveHasNoAddressException(LocomotiveInfo loco) :
            base(loco, "Locomotive " + loco.DisplayName + " has no assigned address") {
        }

        public LocomotiveHasNoAddressException(LocomotiveInfo loco, string message) : base(loco, message) {
        }

        public LocomotiveHasNoAddressException() : base() {
        }

        public LocomotiveHasNoAddressException(string message) : base(message) {
        }

        public LocomotiveHasNoAddressException(object subject, string message) : base(subject, message) {
        }

        public LocomotiveHasNoAddressException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }
    }

    public class LocomotiveAddressAlreadyUsedException : LayoutLocomotiveException {
        public LocomotiveAddressAlreadyUsedException(LocomotiveInfo loco, TrainStateInfo usedBy) :
            base(loco, "Locomotive " + loco.DisplayName + " address is already used by another locomotive (" + usedBy.DisplayName + ")") {
            this.UsedBy = usedBy;
        }

        public LocomotiveAddressAlreadyUsedException(LocomotiveInfo loco, string message) : base(loco, message) {
        }

        public LocomotiveAddressAlreadyUsedException() : base() {
        }

        public LocomotiveAddressAlreadyUsedException(string message) : base(message) {
        }

        public LocomotiveAddressAlreadyUsedException(object subject, string message) : base(subject, message) {
        }

        public LocomotiveAddressAlreadyUsedException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }

        public TrainStateInfo UsedBy { get; }
    }

    public class TrainLocomotiveAlreadyUsedException : LayoutLocomotiveException {
        public TrainLocomotiveAlreadyUsedException(LocomotiveInfo loco, TrainInCollectionInfo trainInCollection, TrainStateInfo trainState) :
            base(loco, "Locomotive '" + loco.DisplayName + "' which is member of '" + trainInCollection.DisplayName + "' is already used in train '" + trainState.DisplayName + "'") {
            this.TrainInCollection = trainInCollection;
            this.Train = trainState;
        }

        public TrainLocomotiveAlreadyUsedException(LocomotiveInfo loco, string message) : base(loco, message) {
        }

        public TrainLocomotiveAlreadyUsedException() : base() {
        }

        public TrainLocomotiveAlreadyUsedException(string message) : base(message) {
        }

        public TrainLocomotiveAlreadyUsedException(object subject, string message) : base(subject, message) {
        }

        public TrainLocomotiveAlreadyUsedException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }

        public TrainInCollectionInfo TrainInCollection { get; }

        public TrainStateInfo Train { get; }
    }

    public class LocomotiveAlreadyUsedException : LayoutLocomotiveException {
        public LocomotiveAlreadyUsedException(LocomotiveInfo loco, TrainStateInfo trainState) :
            base(loco, "Locomotive '" + loco.DisplayName + "' is already used in train '" + trainState.DisplayName + "'") {
            this.Train = trainState;
        }

        public LocomotiveAlreadyUsedException(LocomotiveInfo loco, string message) : base(loco, message) {
        }

        public LocomotiveAlreadyUsedException() : base() {
        }

        public LocomotiveAlreadyUsedException(string message) : base(message) {
        }

        public LocomotiveAlreadyUsedException(object subject, string message) : base(subject, message) {
        }

        public LocomotiveAlreadyUsedException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }

        public TrainStateInfo Train { get; }
    }

    public class TrainLocomotiveDuplicateAddressException : LayoutException {
        public TrainLocomotiveDuplicateAddressException(
            TrainInCollectionInfo trainInCollection, LocomotiveInfo loco1, LocomotiveInfo loco2) :
            base("Train " + trainInCollection.DisplayName + " members: '" + loco1.DisplayName + "' and '" + loco2.DisplayName + "' have the same address") {
            this.Train = trainInCollection;
            this.Locomotive1 = loco1;
            this.Locomotive2 = loco2;
        }

        public TrainLocomotiveDuplicateAddressException() : base() {
        }

        public TrainLocomotiveDuplicateAddressException(string message) : base(message) {
        }

        public TrainLocomotiveDuplicateAddressException(object subject, string message) : base(subject, message) {
        }

        public TrainLocomotiveDuplicateAddressException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }

        public TrainInCollectionInfo Train { get; }

        public LocomotiveInfo Locomotive1 { get; }

        public LocomotiveInfo Locomotive2 { get; }
    }

    public abstract class ElementNotOnTrackException : LayoutException {
        protected ElementNotOnTrackException(XmlElement element, string message) : base(message) {
            this.Element = element;
        }

        protected ElementNotOnTrackException() : base() {
        }

        protected ElementNotOnTrackException(string message) : base(message) {
        }

        protected ElementNotOnTrackException(object subject, string message) : base(subject, message) {
        }

        protected ElementNotOnTrackException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }

        public XmlElement Element { get; }

        public bool IsLocomotive => Element.Name == "Locomotive";

        public bool IsTrain => Element.Name == "Train";
    }

    public class LocomotiveNotOnTrackException : ElementNotOnTrackException {
        public LocomotiveNotOnTrackException(LocomotiveInfo loco) :
            base(loco.Element, "Locomotive '" + loco.DisplayName + "' is not placed on track") {
        }

        protected LocomotiveNotOnTrackException(XmlElement element, string message) : base(element, message) {
        }

        public LocomotiveNotOnTrackException() : base() {
        }

        public LocomotiveNotOnTrackException(string message) : base(message) {
        }

        public LocomotiveNotOnTrackException(object subject, string message) : base(subject, message) {
        }

        public LocomotiveNotOnTrackException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }

        public LocomotiveInfo Locomotive => IsLocomotive ? new LocomotiveInfo(Element) : null;
    }

    public class TrainNotOnTrackException : ElementNotOnTrackException {
        public TrainNotOnTrackException(TrainCommonInfo train) :
            base(train.Element, "Locomotive set '" + train.DisplayName + "' is not placed on track") {
        }

        protected TrainNotOnTrackException(XmlElement element, string message) : base(element, message) {
        }

        public TrainNotOnTrackException() : base() {
        }

        public TrainNotOnTrackException(string message) : base(message) {
        }

        public TrainNotOnTrackException(object subject, string message) : base(subject, message) {
        }

        public TrainNotOnTrackException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }

        public TrainInCollectionInfo Train => IsTrain ? new TrainInCollectionInfo(Element) : null;
    }

    public class LocomotiveNotCompatibleException : LayoutLocomotiveException {
        public LocomotiveNotCompatibleException(LocomotiveInfo loco, TrainCommonInfo train) :
            base(loco, "Locomotive " + loco.DisplayName + " is not compatible with locomotives in train '" + train.DisplayName + "' (e.g. # of speed steps)") {
            this.Train = train;
        }

        public LocomotiveNotCompatibleException(LocomotiveInfo loco, string message) : base(loco, message) {
        }

        public LocomotiveNotCompatibleException() : base() {
        }

        public LocomotiveNotCompatibleException(string message) : base(message) {
        }

        public LocomotiveNotCompatibleException(object subject, string message) : base(subject, message) {
        }

        public LocomotiveNotCompatibleException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }

        public TrainCommonInfo Train { get; }
    }

    public class LocomotiveNotManagedException : LayoutLocomotiveException {
        public LocomotiveNotManagedException(LocomotiveInfo loco) :
            base(loco, "Locomotive " + loco.DisplayName + " is not managed by software - operation is not possible") {
        }

        public LocomotiveNotManagedException(LocomotiveInfo loco, string message) : base(loco, message) {
        }

        public LocomotiveNotManagedException() : base() {
        }

        public LocomotiveNotManagedException(string message) : base(message) {
        }

        public LocomotiveNotManagedException(object subject, string message) : base(subject, message) {
        }

        public LocomotiveNotManagedException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }
    }

    #endregion

    #region Locomotive tracking related exceptions

    public class BlockEdgeCrossingException : LayoutException {
        protected BlockEdgeCrossingException(LayoutBlockEdgeBase blockEdge, string message) :
            base(blockEdge, message) {
            this.BlockEdge = blockEdge;
        }

        protected BlockEdgeCrossingException(String message) :
            base(message) {
        }

        public BlockEdgeCrossingException() : base() {
        }

        public BlockEdgeCrossingException(object subject, string message) : base(subject, message) {
        }

        public BlockEdgeCrossingException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }

        public LayoutBlockEdgeBase BlockEdge { get; protected set; }
    }

    public class UnexpectedBlockCrossingException : BlockEdgeCrossingException {
        public UnexpectedBlockCrossingException(LayoutBlockEdgeBase blockEdge) :
            base(blockEdge, "Unexpected block crossing. No locomotives in surrounding blocks") {
        }

        protected UnexpectedBlockCrossingException(LayoutBlockEdgeBase blockEdge, string message) : base(blockEdge, message) {
        }

        protected UnexpectedBlockCrossingException(string message) : base(message) {
        }

        public UnexpectedBlockCrossingException() : base() {
        }

        public UnexpectedBlockCrossingException(object subject, string message) : base(subject, message) {
        }

        public UnexpectedBlockCrossingException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }

        public override string DefaultMessageType => "warning";
    }

    public class AmbiguousBlockCrossingException : BlockEdgeCrossingException {
        public AmbiguousBlockCrossingException(LayoutBlockEdgeBase blockEdge) :
            base(blockEdge, "Unable to decide which locomotive crossed the block") {
        }

        protected AmbiguousBlockCrossingException(LayoutBlockEdgeBase blockEdge, string message) : base(blockEdge, message) {
        }

        protected AmbiguousBlockCrossingException(string message) : base(message) {
        }

        public AmbiguousBlockCrossingException() : base() {
        }

        public AmbiguousBlockCrossingException(object subject, string message) : base(subject, message) {
        }

        public AmbiguousBlockCrossingException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }
    }

    public class CrossingFromManualDispatchRegion : BlockEdgeCrossingException {
        public CrossingFromManualDispatchRegion(LayoutBlockEdgeBase blockEdge) :
            base(blockEdge, "Unexpected crossing from manual dispatch region to non-manual dispatch resion") {
        }

        protected CrossingFromManualDispatchRegion(LayoutBlockEdgeBase blockEdge, string message) : base(blockEdge, message) {
        }

        protected CrossingFromManualDispatchRegion(string message) : base(message) {
        }

        public CrossingFromManualDispatchRegion() : base() {
        }

        public CrossingFromManualDispatchRegion(object subject, string message) : base(subject, message) {
        }

        public CrossingFromManualDispatchRegion(object subject, string message, Exception inner) : base(subject, message, inner) {
        }
    }

    public class InconsistentLocomotiveBlockCrossingException : BlockEdgeCrossingException {
        public InconsistentLocomotiveBlockCrossingException(LayoutBlockEdgeBase blockEdge, TrainStateInfo trainState) :
            base("Locomotive seems to cross between blocks, however this locomotive is either not moving or moving in the other direction") {
            LayoutSelection selection = new LayoutSelection();

            this.LocomotiveState = trainState;
            BlockEdge = blockEdge;

            selection.Add(blockEdge);
            if (trainState.LocomotiveBlock.BlockDefinintion != null)
                selection.Add(trainState.LocomotiveBlock.BlockDefinintion);

            Subject = selection;
        }

        protected InconsistentLocomotiveBlockCrossingException(LayoutBlockEdgeBase blockEdge, string message) : base(blockEdge, message) {
        }

        protected InconsistentLocomotiveBlockCrossingException(string message) : base(message) {
        }

        public InconsistentLocomotiveBlockCrossingException() : base() {
        }

        public InconsistentLocomotiveBlockCrossingException(object subject, string message) : base(subject, message) {
        }

        public InconsistentLocomotiveBlockCrossingException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }

        public TrainStateInfo LocomotiveState { get; }
    }

    public class LocomotiveMotionNotConsistentWithTurnoutSettingException : BlockEdgeCrossingException {
        public LocomotiveMotionNotConsistentWithTurnoutSettingException(LayoutBlockEdgeBase blockEdge, TrainStateInfo train, LayoutBlock fromBlock, LayoutBlock toBlock) :
            base(blockEdge, "Train " + train.DisplayName + " motion is not consistent with the turnout settings, moving from " + fromBlock.Name + " to " + toBlock.Name) {
            this.Train = train;
            this.FromBlock = fromBlock;
            this.ToBlock = toBlock;
        }

        protected LocomotiveMotionNotConsistentWithTurnoutSettingException(LayoutBlockEdgeBase blockEdge, string message) : base(blockEdge, message) {
        }

        protected LocomotiveMotionNotConsistentWithTurnoutSettingException(string message) : base(message) {
        }

        public LocomotiveMotionNotConsistentWithTurnoutSettingException() : base() {
        }

        public LocomotiveMotionNotConsistentWithTurnoutSettingException(object subject, string message) : base(subject, message) {
        }

        public LocomotiveMotionNotConsistentWithTurnoutSettingException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }

        public TrainStateInfo Train { get; }

        public LayoutBlock FromBlock { get; }

        public LayoutBlock ToBlock { get; }
    }

    public class TrainDetectionException : LayoutException {
        public TrainDetectionException(LayoutOccupancyBlock occupancyBlock, string message) :
            base(occupancyBlock, message) {
            this.OccupancyBlock = occupancyBlock;
        }

        public TrainDetectionException() : base() {
        }

        public TrainDetectionException(string message) : base(message) {
        }

        public TrainDetectionException(object subject, string message) : base(subject, message) {
        }

        public TrainDetectionException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }

        public LayoutOccupancyBlock OccupancyBlock { get; }
    }

    public class UnexpectedTrainDetectionException : TrainDetectionException {
        public UnexpectedTrainDetectionException(LayoutOccupancyBlock occupancyBlock) :
            base(occupancyBlock, "This train detection block has detected a train. However it is not possible to figure out which train is in the block") {
        }

        public UnexpectedTrainDetectionException(LayoutOccupancyBlock occupancyBlock, string message) : base(occupancyBlock, message) {
        }

        public UnexpectedTrainDetectionException() : base() {
        }

        public UnexpectedTrainDetectionException(string message) : base(message) {
        }

        public UnexpectedTrainDetectionException(object subject, string message) : base(subject, message) {
        }

        public UnexpectedTrainDetectionException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }
    }

    public class AmbiguousTrainDetectionException : TrainDetectionException {
        public AmbiguousTrainDetectionException(LayoutOccupancyBlock occupancyBlock) :
            base(occupancyBlock, "This train detection block has detected a train. However more than one train may have entered this block") {
        }

        public AmbiguousTrainDetectionException(LayoutOccupancyBlock occupancyBlock, string message) : base(occupancyBlock, message) {
        }

        public AmbiguousTrainDetectionException() : base() {
        }

        public AmbiguousTrainDetectionException(string message) : base(message) {
        }

        public AmbiguousTrainDetectionException(object subject, string message) : base(subject, message) {
        }

        public AmbiguousTrainDetectionException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }
    }

    public class AmbiguousForecastForNoFeedbackException : LayoutException {
        public AmbiguousForecastForNoFeedbackException(LayoutBlock block) :
            base(block, "It is not possible to figure out which train enters this no-feedback block") {
        }

        public AmbiguousForecastForNoFeedbackException() : base() {
        }

        public AmbiguousForecastForNoFeedbackException(string message) : base(message) {
        }

        public AmbiguousForecastForNoFeedbackException(object subject, string message) : base(subject, message) {
        }

        public AmbiguousForecastForNoFeedbackException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }

        public LayoutBlock Block => (LayoutBlock)Subject;
    }

    public class DetectedRunawayTrainException : TrainDetectionException {
        public DetectedRunawayTrainException(LayoutOccupancyBlock occupancyBlock, TrainStateInfo train, string message)
            : base(occupancyBlock, message) {
            this.Train = train;
        }

        public DetectedRunawayTrainException(LayoutOccupancyBlock occupancyBlock, TrainStateInfo train)
            : base(occupancyBlock, "Unexpected train " + train.DisplayName + " detected in this block") {
            this.Train = train;
        }

        public DetectedRunawayTrainException(LayoutOccupancyBlock occupancyBlock, string message) : base(occupancyBlock, message) {
        }

        public DetectedRunawayTrainException() : base() {
        }

        public DetectedRunawayTrainException(string message) : base(message) {
        }

        public DetectedRunawayTrainException(object subject, string message) : base(subject, message) {
        }

        public DetectedRunawayTrainException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }

        public TrainStateInfo Train { get; }
    }

    public class DetectedRunawayTrainAndFaultyTurnout : DetectedRunawayTrainException {
        public DetectedRunawayTrainAndFaultyTurnout(LayoutOccupancyBlock occupancyBlock, TrainStateInfo train, IModelComponentIsMultiPath turnout)
            :
            base(occupancyBlock, train, "A faulty turnout caused unexpected train " + train.DisplayName + " to be detected in this block, check turnout") {
            this.Turnout = turnout;
            Subject = new LayoutSelection(new ModelComponent[] { occupancyBlock.BlockDefinintion, (ModelComponent)turnout });
        }

        public DetectedRunawayTrainAndFaultyTurnout(LayoutOccupancyBlock occupancyBlock, TrainStateInfo train, string message) : base(occupancyBlock, train, message) {
        }

        public DetectedRunawayTrainAndFaultyTurnout(LayoutOccupancyBlock occupancyBlock, TrainStateInfo train) : base(occupancyBlock, train) {
        }

        public DetectedRunawayTrainAndFaultyTurnout(LayoutOccupancyBlock occupancyBlock, string message) : base(occupancyBlock, message) {
        }

        public DetectedRunawayTrainAndFaultyTurnout() : base() {
        }

        public DetectedRunawayTrainAndFaultyTurnout(string message) : base(message) {
        }

        public DetectedRunawayTrainAndFaultyTurnout(object subject, string message) : base(subject, message) {
        }

        public DetectedRunawayTrainAndFaultyTurnout(object subject, string message, Exception inner) : base(subject, message, inner) {
        }

        public IModelComponentIsMultiPath Turnout { get; }
    }

    #endregion

    #region Image related rexception

    public class ImageLoadException : LayoutException {
        public ImageLoadException(String imageFilename, object subject, Exception ex) :
          base(subject, "Loading image from " + imageFilename + " - " + ex.Message, ex) {
            this.ImageFilename = imageFilename;
        }

        public ImageLoadException() : base() {
        }

        public ImageLoadException(string message) : base(message) {
        }

        public ImageLoadException(object subject, string message) : base(subject, message) {
        }

        public ImageLoadException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }

        public string ImageFilename { get; }
    }

    #endregion

    #region Origin editing related exceptions

    public class LayoutNoPathFromOriginException : LayoutException {
        public LayoutNoPathFromOriginException(LayoutBlockDefinitionComponent blockInfo, TripPlanInfo tripPlan) : base(blockInfo, "No path exists between this location and the trip plan's first destination") {
            this.TripPlan = tripPlan;
        }

        public LayoutNoPathFromOriginException() : base() {
        }

        public LayoutNoPathFromOriginException(string message) : base(message) {
        }

        public LayoutNoPathFromOriginException(object subject, string message) : base(subject, message) {
        }

        public LayoutNoPathFromOriginException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }

        public LayoutBlockDefinitionComponent BlockInfo => (LayoutBlockDefinitionComponent)Subject;

        public TripPlanInfo TripPlan { get; }
    }

    #endregion
}