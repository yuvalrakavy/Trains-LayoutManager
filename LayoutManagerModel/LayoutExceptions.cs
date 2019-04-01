using System;
using System.Xml;

using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager {

    public class LayoutLocomotiveException : LayoutException {
        readonly LocomotiveInfo loco;

        public LayoutLocomotiveException(LocomotiveInfo loco, string message) : base(message) {
            this.loco = loco;
        }

        public LocomotiveInfo Locomotive => loco;
    }

    #region Locomotive Manager Exception classess

    public class LocomotiveHasNoAddressException : LayoutLocomotiveException {
        public LocomotiveHasNoAddressException(LocomotiveInfo loco) :
            base(loco, "Locomotive " + loco.DisplayName + " has no assigned address") {
        }
    }

    public class LocomotiveAddressAlreadyUsedException : LayoutLocomotiveException {
        readonly TrainStateInfo usedBy;

        public LocomotiveAddressAlreadyUsedException(LocomotiveInfo loco, TrainStateInfo usedBy) :
            base(loco, "Locomotive " + loco.DisplayName + " address is already used by another locomotive (" + usedBy.DisplayName + ")") {
            this.usedBy = usedBy;
        }

        public TrainStateInfo UsedBy => usedBy;
    }

    public class TrainLocomotiveAlreadyUsedException : LayoutLocomotiveException {
        readonly TrainInCollectionInfo trainInCollection;
        readonly TrainStateInfo trainState;

        public TrainLocomotiveAlreadyUsedException(LocomotiveInfo loco, TrainInCollectionInfo trainInCollection, TrainStateInfo trainState) :
            base(loco, "Locomotive '" + loco.DisplayName + "' which is member of '" + trainInCollection.DisplayName + "' is already used in train '" + trainState.DisplayName + "'") {
            this.trainInCollection = trainInCollection;
            this.trainState = trainState;
        }

        public TrainInCollectionInfo TrainInCollection => trainInCollection;

        public TrainStateInfo Train => trainState;
    }

    public class LocomotiveAlreadyUsedException : LayoutLocomotiveException {
        readonly TrainStateInfo trainState;

        public LocomotiveAlreadyUsedException(LocomotiveInfo loco, TrainStateInfo trainState) :
            base(loco, "Locomotive '" + loco.DisplayName + "' is already used in train '" + trainState.DisplayName + "'") {
            this.trainState = trainState;
        }

        public TrainStateInfo Train => trainState;
    }

    public class TrainLocomotiveDuplicateAddressException : LayoutException {
        readonly TrainInCollectionInfo trainInCollection;
        readonly LocomotiveInfo loco1;
        readonly LocomotiveInfo loco2;

        public TrainLocomotiveDuplicateAddressException(
            TrainInCollectionInfo trainInCollection, LocomotiveInfo loco1, LocomotiveInfo loco2) :
            base("Train " + trainInCollection.DisplayName + " members: '" + loco1.DisplayName + "' and '" + loco2.DisplayName + "' have the same address") {
            this.trainInCollection = trainInCollection;
            this.loco1 = loco1;
            this.loco2 = loco2;
        }

        public TrainInCollectionInfo Train => trainInCollection;

        public LocomotiveInfo Locomotive1 => loco1;

        public LocomotiveInfo Locomotive2 => loco2;
    }

    public abstract class ElementNotOnTrackException : LayoutException {
        readonly XmlElement _element;

        protected ElementNotOnTrackException(XmlElement element, string message) : base(message) {
            this._element = element;
        }

        public XmlElement Element => _element;

        public bool IsLocomotive => _element.Name == "Locomotive";

        public bool IsTrain => _element.Name == "Train";
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
        readonly TrainCommonInfo train;

        public LocomotiveNotCompatibleException(LocomotiveInfo loco, TrainCommonInfo train) :
            base(loco, "Locomotive " + loco.DisplayName + " is not compatible with locomotives in train '" + train.DisplayName + "' (e.g. # of speed steps)") {
            this.train = train;
        }

        public TrainCommonInfo Train => train;
    }

    public class LocomotiveNotManagedException : LayoutLocomotiveException {
        public LocomotiveNotManagedException(LocomotiveInfo loco) :
            base(loco, "Locomotive " + loco.DisplayName + " is not managed by software - operation is not possible") {
        }
    }

    #endregion

    #region Locomotive tracking related exceptions

    public class BlockEdgeCrossingException : LayoutException {
        LayoutBlockEdgeBase _blockEdge;

        protected BlockEdgeCrossingException(LayoutBlockEdgeBase blockEdge, string message) :
            base(blockEdge, message) {
            this._blockEdge = blockEdge;
        }

        protected BlockEdgeCrossingException(String message) :
            base(message) {
        }

        public LayoutBlockEdgeBase BlockEdge {
            get {
                return _blockEdge;
            }

            protected set {
                _blockEdge = value;
            }
        }
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
        readonly TrainStateInfo trainState;

        public InconsistentLocomotiveBlockCrossingException(LayoutBlockEdgeBase blockEdge, TrainStateInfo trainState) :
            base("Locomotive seems to cross between blocks, however this locomotive is either not moving or moving in the other direction") {
            LayoutSelection selection = new LayoutSelection();

            this.trainState = trainState;
            BlockEdge = blockEdge;

            selection.Add(blockEdge);
            if (trainState.LocomotiveBlock.BlockDefinintion != null)
                selection.Add(trainState.LocomotiveBlock.BlockDefinintion);

            Subject = selection;
        }

        public TrainStateInfo LocomotiveState => trainState;
    }

    public class LocomotiveMotionNotConsistentWithTurnoutSettingException : BlockEdgeCrossingException {
        readonly TrainStateInfo train;
        readonly LayoutBlock fromBlock;
        readonly LayoutBlock toBlock;

        public LocomotiveMotionNotConsistentWithTurnoutSettingException(LayoutBlockEdgeBase blockEdge, TrainStateInfo train, LayoutBlock fromBlock, LayoutBlock toBlock) :
            base(blockEdge, "Train " + train.DisplayName + " motion is not consistent with the turnout settings, moving from " + fromBlock.Name + " to " + toBlock.Name) {
            this.train = train;
            this.fromBlock = fromBlock;
            this.toBlock = toBlock;
        }

        public TrainStateInfo Train => train;

        public LayoutBlock FromBlock => fromBlock;

        public LayoutBlock ToBlock => toBlock;
    }


    public class TrainDetectionException : LayoutException {
        readonly LayoutOccupancyBlock occupancyBlock;

        public TrainDetectionException(LayoutOccupancyBlock occupancyBlock, string message) :
            base(occupancyBlock, message) {
            this.occupancyBlock = occupancyBlock;
        }

        public LayoutOccupancyBlock OccupancyBlock => occupancyBlock;
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
        readonly TrainStateInfo train;

        public DetectedRunawayTrainException(LayoutOccupancyBlock occupancyBlock, TrainStateInfo train, string message)
            : base(occupancyBlock, message) {
            this.train = train;
        }

        public DetectedRunawayTrainException(LayoutOccupancyBlock occupancyBlock, TrainStateInfo train)
            : base(occupancyBlock, "Unexpected train " + train.DisplayName + " detected in this block") {
            this.train = train;
        }

        public TrainStateInfo Train => train;
    }

    public class DetectedRunawayTrainAndFaultyTurnout : DetectedRunawayTrainException {
        readonly IModelComponentIsMultiPath turnout;

        public DetectedRunawayTrainAndFaultyTurnout(LayoutOccupancyBlock occupancyBlock, TrainStateInfo train, IModelComponentIsMultiPath turnout)
            :
            base(occupancyBlock, train, "A faulty turnout caused unexpected train " + train.DisplayName + " to be detected in this block, check turnout") {
            this.turnout = turnout;
            Subject = new LayoutSelection(new ModelComponent[] { occupancyBlock.BlockDefinintion, (ModelComponent)turnout });
        }

        public IModelComponentIsMultiPath Turnout => turnout;
    }

    #endregion

    #region Image related rexception

    public class ImageLoadException : LayoutException {
        readonly string imageFilename;

        public ImageLoadException(String imageFilename, object subject, Exception ex) :
          base(subject, "Loading image from " + imageFilename + " - " + ex.Message, ex) {
            this.imageFilename = imageFilename;
        }

        public string ImageFilename => imageFilename;
    }

    #endregion

    #region Origin editing related exceptions

    public class LayoutNoPathFromOriginException : LayoutException {
        readonly TripPlanInfo tripPlan;

        public LayoutNoPathFromOriginException(LayoutBlockDefinitionComponent blockInfo, TripPlanInfo tripPlan) : base(blockInfo, "No path exists between this location and the trip plan's first destination") {
            this.tripPlan = tripPlan;
        }

        public LayoutBlockDefinitionComponent BlockInfo => (LayoutBlockDefinitionComponent)Subject;

        public TripPlanInfo TripPlan => tripPlan;
    }

    #endregion
}