using System;
using System.Xml;

using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager {
    public class LayoutLocomotiveException : LayoutException {
        public LayoutLocomotiveException(LocomotiveInfo loco, string message) : base(message) {
            this.Locomotive = loco;
        }

        public LocomotiveInfo Locomotive { get; }
    }

    #region Locomotive Manager Exception classess

    public class LocomotiveHasNoAddressException : LayoutLocomotiveException {
        public LocomotiveHasNoAddressException(LocomotiveInfo loco) :
            base(loco, "Locomotive " + loco.DisplayName + " has no assigned address") {
        }
    }

    public class LocomotiveAddressAlreadyUsedException : LayoutLocomotiveException {
        public LocomotiveAddressAlreadyUsedException(LocomotiveInfo loco, TrainStateInfo usedBy) :
            base(loco, "Locomotive " + loco.DisplayName + " address is already used by another locomotive (" + usedBy.DisplayName + ")") {
            this.UsedBy = usedBy;
        }

        public TrainStateInfo UsedBy { get; }
    }

    public class TrainLocomotiveAlreadyUsedException : LayoutLocomotiveException {
        public TrainLocomotiveAlreadyUsedException(LocomotiveInfo loco, TrainInCollectionInfo trainInCollection, TrainStateInfo trainState) :
            base(loco, "Locomotive '" + loco.DisplayName + "' which is member of '" + trainInCollection.DisplayName + "' is already used in train '" + trainState.DisplayName + "'") {
            this.TrainInCollection = trainInCollection;
            this.Train = trainState;
        }

        public TrainInCollectionInfo TrainInCollection { get; }

        public TrainStateInfo Train { get; }
    }

    public class LocomotiveAlreadyUsedException : LayoutLocomotiveException {
        public LocomotiveAlreadyUsedException(LocomotiveInfo loco, TrainStateInfo trainState) :
            base(loco, "Locomotive '" + loco.DisplayName + "' is already used in train '" + trainState.DisplayName + "'") {
            this.Train = trainState;
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

        public TrainInCollectionInfo Train { get; }

        public LocomotiveInfo Locomotive1 { get; }

        public LocomotiveInfo Locomotive2 { get; }
    }

    public abstract class ElementNotOnTrackException : LayoutException {
        protected ElementNotOnTrackException(XmlElement element, string message) : base(message) {
            this.Element = element;
        }

        public XmlElement Element { get; }

        public bool IsLocomotive => Element.Name == "Locomotive";

        public bool IsTrain => Element.Name == "Train";
    }

    public class LocomotiveNotOnTrackException : ElementNotOnTrackException {
        public LocomotiveNotOnTrackException(LocomotiveInfo loco) :
            base(loco.Element, "Locomotive '" + loco.DisplayName + "' is not placed on track") {
        }

        public LocomotiveInfo Locomotive {
            get {
                if (IsLocomotive)
                    return new LocomotiveInfo(Element);
                return null;
            }
        }
    }

    public class TrainNotOnTrackException : ElementNotOnTrackException {
        public TrainNotOnTrackException(TrainCommonInfo train) :
            base(train.Element, "Locomotive set '" + train.DisplayName + "' is not placed on track") {
        }

        public TrainInCollectionInfo Train {
            get {
                if (IsTrain)
                    return new TrainInCollectionInfo(Element);
                return null;
            }
        }
    }

    public class LocomotiveNotCompatibleException : LayoutLocomotiveException {
        public LocomotiveNotCompatibleException(LocomotiveInfo loco, TrainCommonInfo train) :
            base(loco, "Locomotive " + loco.DisplayName + " is not compatible with locomotives in train '" + train.DisplayName + "' (e.g. # of speed steps)") {
            this.Train = train;
        }

        public TrainCommonInfo Train { get; }
    }

    public class LocomotiveNotManagedException : LayoutLocomotiveException {
        public LocomotiveNotManagedException(LocomotiveInfo loco) :
            base(loco, "Locomotive " + loco.DisplayName + " is not managed by software - operation is not possible") {
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

        public LayoutBlockEdgeBase BlockEdge { get; protected set; }
    }

    public class UnexpectedBlockCrossingException : BlockEdgeCrossingException {
        public UnexpectedBlockCrossingException(LayoutBlockEdgeBase blockEdge) :
            base(blockEdge, "Unexpected block crossing. No locomotives in surrounding blocks") {
        }

        public override string DefaultMessageType => "warning";
    }

    public class AmbiguousBlockCrossingException : BlockEdgeCrossingException {
        public AmbiguousBlockCrossingException(LayoutBlockEdgeBase blockEdge) :
            base(blockEdge, "Unable to decide which locomotive crossed the block") {
        }
    }

    public class CrossingFromManualDispatchRegion : BlockEdgeCrossingException {
        public CrossingFromManualDispatchRegion(LayoutBlockEdgeBase blockEdge) :
            base(blockEdge, "Unexpected crossing from manual dispatch region to non-manual dispatch resion") {
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

        public TrainStateInfo LocomotiveState { get; }
    }

    public class LocomotiveMotionNotConsistentWithTurnoutSettingException : BlockEdgeCrossingException {
        public LocomotiveMotionNotConsistentWithTurnoutSettingException(LayoutBlockEdgeBase blockEdge, TrainStateInfo train, LayoutBlock fromBlock, LayoutBlock toBlock) :
            base(blockEdge, "Train " + train.DisplayName + " motion is not consistent with the turnout settings, moving from " + fromBlock.Name + " to " + toBlock.Name) {
            this.Train = train;
            this.FromBlock = fromBlock;
            this.ToBlock = toBlock;
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

        public LayoutOccupancyBlock OccupancyBlock { get; }
    }

    public class UnexpectedTrainDetectionException : TrainDetectionException {
        public UnexpectedTrainDetectionException(LayoutOccupancyBlock occupancyBlock) :
            base(occupancyBlock, "This train detection block has detected a train. However it is not possible to figure out which train is in the block") {
        }
    }

    public class AmbiguousTrainDetectionException : TrainDetectionException {
        public AmbiguousTrainDetectionException(LayoutOccupancyBlock occupancyBlock) :
            base(occupancyBlock, "This train detection block has detected a train. However more than one train may have entered this block") {
        }
    }

    public class AmbiguousForecastForNoFeedbackException : LayoutException {
        public AmbiguousForecastForNoFeedbackException(LayoutBlock block) :
            base(block, "It is not possible to figure out which train enters this no-feedback block") {
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

        public TrainStateInfo Train { get; }
    }

    public class DetectedRunawayTrainAndFaultyTurnout : DetectedRunawayTrainException {
        public DetectedRunawayTrainAndFaultyTurnout(LayoutOccupancyBlock occupancyBlock, TrainStateInfo train, IModelComponentIsMultiPath turnout)
            :
            base(occupancyBlock, train, "A faulty turnout caused unexpected train " + train.DisplayName + " to be detected in this block, check turnout") {
            this.Turnout = turnout;
            Subject = new LayoutSelection(new ModelComponent[] { occupancyBlock.BlockDefinintion, (ModelComponent)turnout });
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

        public string ImageFilename { get; }
    }

    #endregion

    #region Origin editing related exceptions

    public class LayoutNoPathFromOriginException : LayoutException {
        public LayoutNoPathFromOriginException(LayoutBlockDefinitionComponent blockInfo, TripPlanInfo tripPlan) : base(blockInfo, "No path exists between this location and the trip plan's first destination") {
            this.TripPlan = tripPlan;
        }

        public LayoutBlockDefinitionComponent BlockInfo => (LayoutBlockDefinitionComponent)Subject;

        public TripPlanInfo TripPlan { get; }
    }

    #endregion
}