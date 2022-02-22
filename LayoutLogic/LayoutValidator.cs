using LayoutManager.Components;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LayoutManager.Logic {
    /// <summary>
    /// Summary description for LayoutValidator.
    /// </summary>
    [LayoutModule("Layout Validator", UserControl = false)]
    public class LayoutValidator : LayoutModuleBase {
        [DispatchTarget(Order = 0)]
        private bool CheckLayout(LayoutPhase phase) {
            bool ok = true;

            ResetTrackConnections();
            ResetTrackAnnotation();
            SetTrackConnections(phase);

            if (!CheckCommandStations(phase))
                ok = false;

            if (!CheckTrackLinks(phase)) {
                ok = false;
            }

            if (!CheckAddresses(phase))
                ok = false;

            if (!CheckSignalLinks(phase))
                ok = false;

            if (!CheckBlockResources(phase))
                ok = false;

            return ok;
        }


        [DispatchTarget(Order = 10000)]
        private void OnExitOperationMode() {
            ResetTrackConnections();
        }

        private void ResetTrackAnnotation() {
            foreach (LayoutModelArea area in LayoutModel.Areas) {
                foreach (LayoutModelSpotComponentCollection spot in area.Grid.Values) {
                    var track = spot.Track;

                    if (track != null) {
                        if (track is LayoutStraightTrackComponent straightTrack)
                            straightTrack.SetTrackAnnotation();
                    }
                }
            }
        }

        private static void SetTrackConnections(LayoutPhase phase) {
            foreach (LayoutModelArea area in LayoutModel.Areas) {
                foreach (LayoutModelSpotComponentCollection spot in area.Grid.Values) {
                    if ((spot.Phase & phase) != 0) {
                        var track = spot.Track;

                        if (track != null) {
                            foreach (LayoutComponentConnectionPoint cp in track.ConnectionPoints) {
                                System.Drawing.Point ml = spot.Location + LayoutTrackComponent.GetConnectionOffset(cp);
                                var otherSpot = area[ml, phase];
                                LayoutTrackComponent? otherTrack = null;

                                if (otherSpot != null && (otherSpot.Phase & phase) != 0)
                                    otherTrack = otherSpot.Track;

                                if (otherTrack != null && otherTrack.HasConnectionPoint(LayoutTrackComponent.GetPointConnectingTo(cp)))
                                    track.SetConnectedComponentEdge(cp, new TrackEdge(otherTrack, LayoutTrackComponent.GetPointConnectingTo(cp)));
                            }
                        }
                    }
                }
            }
        }

        private static void ResetTrackConnections() {
            foreach (LayoutModelArea area in LayoutModel.Areas) {
                foreach (LayoutModelSpotComponentCollection spot in area.Grid.Values) {
                    var track = spot.Track;

                    if (track != null)
                        track.CleanupComponentEdgeConnections();
                }
            }
        }

        private bool CheckCommandStations(LayoutPhase phase) {
            if (!LayoutModel.Components<IModelComponentIsCommandStation>(phase).Any()) {
                Error("At least one command station component should be included in the layout");
                return false;
            }

            return true;
        }

        private static LayoutComponentConnectionPoint GetLinkedConnectionPoint(LayoutTrackComponent track, LayoutPhase phase) {
            LayoutComponentConnectionPoint linkedConnectionPoint = LayoutComponentConnectionPoint.Empty;

            foreach (LayoutComponentConnectionPoint cp in track.ConnectionPoints) {
                System.Drawing.Point ml = track.Location + LayoutTrackComponent.GetConnectionOffset(cp);
                var otherSpot = track.Spot.Area[ml, phase];
                LayoutTrackComponent? otherTrack = null;

                if (otherSpot != null)
                    otherTrack = otherSpot.Track;

                if (otherTrack == null || !otherTrack.HasConnectionPoint(LayoutTrackComponent.GetPointConnectingTo(cp))) {
                    linkedConnectionPoint = cp;
                    break;
                }
            }

            return linkedConnectionPoint;
        }

        private bool CheckTrackLinks(LayoutPhase phase) {
            LayoutSelection notLinked = new();
            LayoutSelection invalidLinks = new();

            foreach (LayoutTrackLinkComponent trackLink in LayoutModel.Components<LayoutTrackLinkComponent>(phase)) {
                if (trackLink.Link == null)
                    notLinked.Add(trackLink);
                else {
                    LayoutTrackComponent thisTrack = trackLink.Track;
                    LayoutComponentConnectionPoint thisCp = GetLinkedConnectionPoint(thisTrack, phase);

                    if (thisCp == LayoutComponentConnectionPoint.Empty)
                        invalidLinks.Add(trackLink);
                    else {
                        var otherTrack = trackLink.LinkedTrack;

                        if (otherTrack == null || (otherTrack.Spot.Phase & phase) == 0)
                            invalidLinks.Add(trackLink);
                        else {
                            LayoutComponentConnectionPoint otherCp = GetLinkedConnectionPoint(otherTrack, phase);

                            if (otherCp != LayoutComponentConnectionPoint.Empty)
                                thisTrack.SetConnectedComponentEdge(thisCp, new TrackEdge(otherTrack, otherCp));
                        }
                    }
                }
            }

            if (notLinked.Count > 0) {
                Error(notLinked, String.Format("Unlinked track link component{0}", notLinked.Count > 1 ? "s" : ""));
                return false;
            }

            if (invalidLinks.Count > 0) {
                Error(invalidLinks, "Track Link cannot be in middle of track.");
                return false;
            }

            return true;
        }

        private bool CheckAddresses(LayoutPhase phase) {
            LayoutSelection nonConnectedComponents = new();
            IEnumerable<IModelComponentConnectToControl> connectableComponents = LayoutModel.Components<IModelComponentConnectToControl>(phase);
            bool result = true;
            bool ignoreFeedbackComponents = LayoutModel.StateManager.VerificationOptions.IgnoreNotConnectedFeedbacks;

            foreach (IModelComponentConnectToControl component in connectableComponents) {
                if (!component.FullyConnected) {
                    if (ignoreFeedbackComponents && (component is LayoutTrackContactComponent || component is LayoutBlockDefinitionComponent))
                        continue;

                    nonConnectedComponents.Add((ModelComponent)component);
                    result = false;
                }
            }

            if (!result)
                Error(nonConnectedComponents, "One or more components are not fully connected to control modules.");

            return result;
        }

        private bool CheckSignalLinks(LayoutPhase phase) {
            bool ok = true;

            foreach (var blockEdge in LayoutModel.Components<LayoutBlockEdgeBase>(phase)) {
                var removeList = new List<LinkedSignalInfo>();

                foreach (LinkedSignalInfo linkedSignal in blockEdge.LinkedSignals) {
                    if (LayoutModel.Component<ModelComponent>(linkedSignal.SignalId, phase) == null) {
                        // If the signal cannot be found at all on the layout (not just on the given phase), remove the link
                        if (LayoutModel.Component<ModelComponent>(linkedSignal.SignalId, LayoutPhase.All) == null) {
                            Warning(blockEdge, $"A signal that was linked to this {blockEdge} cannot be found in the layout");
                            removeList.Add(linkedSignal);
                        }
                        else
                            Error(blockEdge, $"The signal linked to this {blockEdge} is marked either as Planned or In construction phase");

                        ok = false;
                    }
                }

                foreach (var linkedSignal in removeList)
                    blockEdge.LinkedSignals.Remove(linkedSignal.SignalId);
            }

            return ok;
        }

        private bool CheckBlockResources(LayoutPhase phase) => LayoutModel.Components<LayoutBlockDefinitionComponent>(phase).All(blockInfo => blockInfo.Info.Resources.CheckIntegrity(this, phase));

        [DispatchTarget]
        private void PerformTrainsAnalysis() {
            var phantomTrains = new LayoutSelection();
            var unexpectedTrains = new LayoutSelection();
            var processedBlocks = new Dictionary<Guid, object?>();

            foreach (LayoutBlock block in LayoutModel.Blocks) {
                if (!processedBlocks.ContainsKey(block.Id)) {
                    if (block.HasTrains) {
                        if (!block.BlockDefinintion.Info.TrainDetected) {
                            // For each train, checks all the blocks in which the train is detected. If there is no block
                            // in which the train is detected, this is a phantom train
                            foreach (TrainLocationInfo trainLocation in block.Trains) {
                                TrainStateInfo train = trainLocation.Train;
                                bool detected = false;

                                foreach (TrainLocationInfo otherLocations in train.Locations) {
                                    if (!processedBlocks.ContainsKey(otherLocations.BlockId))
                                        processedBlocks.Add(otherLocations.BlockId, null);
                                    if (otherLocations.Block.BlockDefinintion.Info.TrainDetected)
                                        detected = true;
                                }

                                if (!detected)
                                    phantomTrains.Add(block);
                            }
                        }
                    }
                    else if (block.BlockDefinintion.Info.UnexpectedTrainDetected) {
                        Stack<LayoutBlock> blocksStack = new();
                        bool detected = false;

                        blocksStack.Push(block);

                        while (blocksStack.Count > 0 && !detected) {
                            LayoutBlock otherBlock = blocksStack.Pop();

                            if (otherBlock.BlockDefinintion.Info.UnexpectedTrainDetected) {
                                if (!processedBlocks.ContainsKey(otherBlock.Id))
                                    processedBlocks.Add(otherBlock.Id, null);

                                foreach (LayoutBlockEdgeBase blockEgde in otherBlock.BlockEdges) {
                                    LayoutBlock neighborBlock = otherBlock.OtherBlock(blockEgde);

                                    if (!processedBlocks.ContainsKey(neighborBlock.Id))
                                        blocksStack.Push(neighborBlock);
                                }
                            }
                            else if (otherBlock.BlockDefinintion.Info.TrainDetected) {
                                if (otherBlock.Trains.Count == 1) {
                                    TrainStateInfo train = otherBlock.Trains[0].Train;

                                    // Train was detected, extend the block to include the one that the train
                                    Dispatch.Call.RequestAutoExtendTrain(train, otherBlock.BlockDefinintion);
                                    detected = true;
                                }
                            }
                        }

                        if (!detected)
                            unexpectedTrains.Add(block);
                    }
                }
            }

            if (phantomTrains.Count > 0)
                Warning(phantomTrains, "Trains are no longer detected, please identify or remove these trains");

            if (unexpectedTrains.Count > 0)
                Warning(unexpectedTrains, "Occupied blocks without trains. Identify or place trains, or physically remove trains from the layout");
        }
    }
}
