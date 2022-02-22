using LayoutManager.Components;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LayoutManager.Logic {
    /// <summary>
    /// Handle locomotive tracking LocomotiveTracking.
    /// </summary>
    [LayoutModule("Locomotive Tracker", UserControl = false)]
    public class LocomotiveTracking : LayoutModuleBase {
        public const string Option_FromBlock = "FromBlock";
        public const string Option_ToBlock = "ToBlock";
        public const string Option_Direction = "Direction";

        private ILayoutTopologyServices? _topologyServices;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible", Justification = "<Pending>")]
        public static LayoutTraceSwitch traceLocomotiveTracking = new("LocomotiveTracking", "Locomotive Tracking");
        private static readonly LayoutBooleanSwitch switchBreakOnBadLocomotiveTracking = new("BreakOnBadLocomotiveTracking", "Locomotive Tracking Break");

        #region Common properties

        protected ILayoutTopologyServices TopologyServices => _topologyServices ??= Dispatch.Call.GetTopologyServices();

        #endregion

        #region Block edge based tracking

        [DispatchTarget]
        private void OnBlockEdgeSensorActive(LayoutTriggerableBlockEdgeBase blockEdge) {
            if (blockEdge.IsEmergencySensor)
                HandleEmergencyBlockEdgeActiveated(blockEdge);
            else
                HandleBlockEdgeActivated(blockEdge);
        }


        private static void HandleEmergencyBlockEdgeActiveated(LayoutTriggerableBlockEdgeBase blockEdge) {
            Dispatch.Call.EmergencyStopRequest($"Emergency sensor {blockEdge.NameProvider.Name} has been triggered");
        }

        private void HandleBlockEdgeActivated(LayoutTriggerableBlockEdgeBase blockEdge) {
            Trace.WriteLineIf(traceLocomotiveTracking.TraceInfo, "Block edge activated at " + blockEdge.FullDescription);

            try {
                TrainStateInfo? train = null;

                var trackingResult = TrackLocomotivePosition(blockEdge, false);

                // If tracking result is null - crossing was within manual dispatch region, and is completely ignored
                if (trackingResult != null) {
                    train = trackingResult.Train;

                    train.LastCrossedBlockEdge = trackingResult.BlockEdge;
                    if (train.Managed)
                        train.LastBlockEdgeCrossingSpeed = train.Speed;

                    var fromLocationTrainPart = train.LocationOfBlock(trackingResult.FromBlock)!.TrainPart;

                    train.EnterBlock(fromLocationTrainPart,
                        trackingResult.ToBlock, trackingResult.BlockEdge, (train, block) => Dispatch.Notification.OnTrainEnteredBlock(train, block));

                    Dispatch.Notification.OnTrackedTrainCrossedBlockEdge(trackingResult);
                }
            }
            catch (LayoutException lex) {
                lex.Report();
                if (switchBreakOnBadLocomotiveTracking.Enabled)
                    Debugger.Break();

                if (blockEdge is LayoutTrackContactComponent trackContact)
                    trackContact.IsTriggered = false;

                Dispatch.Call.EmergencyStopRequest($"Train location tracking error: {lex.Message}");
            }
        }

#if NOT_USED
        [LayoutEvent("block-edge-sensor-not-active")]
        private void BlockEdgeSensorNotActive(LayoutEvent e) {
            var blockEdge = Ensure.NotNull<LayoutTriggerableBlockEdgeBase>(e.Sender, "blockEdge");
            var track = blockEdge.Track;

            // Get the blocks from which the train should be removed:
            //
            // Any block that contains the train for which there is not any trigerrable edge that is active.
            // If block's edge is not trigerrable (logical block edge), the test is extended to the edges of the neighboring edge (as if
            // the logical block edge is ignored and the two blocks are treated as one larger block)
            //
            static bool GetBlocksWithTraintoRemove(LayoutBlock block, LayoutBlockEdgeBase originEdge, TrainStateInfo train, List<LayoutBlock> blocksWithTrainToRemove) {
                if (block.HasTrains && block.Trains[0].Train.Id == train.Id) {
                    bool hasTriggedEdge = false;

                    foreach (var bEdge in block.BlockEdges) {
                        if (bEdge.Id != originEdge.Id) {
                            if (bEdge is LayoutTriggerableBlockEdgeBase sensorEdge) {
                                if (sensorEdge.IsTriggered) {
                                    hasTriggedEdge = true;
                                    break;
                                }
                            }
                            else {
                                var neighboringBlock = bEdge.GetNeighboringBlock(block);
                                if (GetBlocksWithTraintoRemove(neighboringBlock, bEdge, train, blocksWithTrainToRemove)) {
                                    hasTriggedEdge = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!hasTriggedEdge)
                        blocksWithTrainToRemove.Add(block);

                    return hasTriggedEdge;
                }
                else
                    return false;
            }

            var surroundingBlocks = new LayoutBlock[] { track.GetBlock(track.ConnectionPoints[0]), track.GetBlock(track.ConnectionPoints[1]) };
            var blocksWithTrainToRemove = new List<LayoutBlock>();

            foreach (var block in surroundingBlocks) {
                var trainLocation = block.Trains.FirstOrDefault();

                if (trainLocation != null)
                    GetBlocksWithTraintoRemove(block, blockEdge, trainLocation.Train, blocksWithTrainToRemove);
            }

            if (blocksWithTrainToRemove.Count == 0)
                Warning(blockEdge, "Block edge sensor was deactivated, however there are no trains in surrounding blocks");
            else {
                var train = blocksWithTrainToRemove[0].Trains[0].Train;

                if (blocksWithTrainToRemove.All(b => b.Trains[0].Train.Id == train.Id)) {
                    bool retainLatestBlockWithTrain = train.Locations.Count == blocksWithTrainToRemove.Count;

                    // Sort based on train entry time, so latest block is last
                    blocksWithTrainToRemove.Sort((b1, b2) => (int)((b1.TrainEntryTime ?? 0) - (b2.TrainEntryTime ?? 0)));

                    if (retainLatestBlockWithTrain && traceLocomotiveTracking.TraceVerbose) {
                        Trace.WriteLine("All blocks from which train should be removed");
                        foreach (var b in blocksWithTrainToRemove)
                            Trace.WriteLine($"   entry time {b.TrainEntryTime} block {b} ");
                    }

                    if (retainLatestBlockWithTrain)
                        blocksWithTrainToRemove.RemoveAt(blocksWithTrainToRemove.Count - 1);

                    if (traceLocomotiveTracking.TraceInfo) {
                        Trace.WriteLine(traceLocomotiveTracking.TraceInfo, "Train will be removed from");
                        foreach (var b in blocksWithTrainToRemove)
                            Trace.WriteLine($"   entry time {b.TrainEntryTime} block {b} ");
                    }

                    foreach (var b in blocksWithTrainToRemove)
                        train.LeaveBlock(b, true);
                }
                else
                    Error(blockEdge, "Unexpected: blocks with train to remove refer to more than one train");
            }
        }
#endif
        #region Track Contact Component

        [DispatchTarget]
        private void OnTrackContactTriggered(LayoutTrackContactComponent trackContact) {
            if (LayoutModel.StateManager.Components.Contains(trackContact.Id, "TrainPassing")) {
                TrackContactPassingStateInfo trackContactPassingState =
                    new(LayoutModel.StateManager.Components.OptionalStateOf(trackContact.Id, "TrainPassing")!);

                // Track contact has state. This means that a train with multiple triggers is passing on top of this contact
                // If the train continue to move in the same direction, one less triggers is expected to come for the train
                // which is still in the "from" block, and one more is expected from the train portion that is on the "to" block.
                // If the number of triggers that are expected from the "from" block is 0, then the train had actually left this
                // block, furthermore, the train is no longer "on top" of the track contact and therefore its state information
                // is remove.
                // The same logic but reversing the "to" and "from" is applied if the train moves in an opposite direction
                var train = trackContactPassingState.Train;

                Debug.Assert(trackContact.IsTriggered);

                LocomotiveOrientation direction = (train.Speed >= 0) ? LocomotiveOrientation.Forward : LocomotiveOrientation.Backward;

                if (direction == trackContactPassingState.Direction) {
                    trackContactPassingState.FromBlockTriggerCount--;
                    trackContactPassingState.ToBlockTriggerCount++;

                    if (trackContactPassingState.FromBlockTriggerCount == 0) {
                        trackContact.IsTriggered = false;
                        trackContactPassingState.Remove();
                    }
                }
                else {
                    trackContactPassingState.ToBlockTriggerCount--;
                    trackContactPassingState.FromBlockTriggerCount++;

                    if (trackContactPassingState.ToBlockTriggerCount == 0) {
                        trackContact.IsTriggered = false;
                        trackContactPassingState.Remove();
                    }
                }
            }
            else
                trackContact.IsTriggered = true;
        }

        [DispatchTarget]
        private void OnTrackedTrainCrossedBlockEdge(LocomotiveTrackingResult trackingResult) {
            if (trackingResult.BlockEdge is LayoutTrackContactComponent trackContact) {
                Debug.Assert(trackContact.IsTriggered);

                // If the train has more than one contact then create a new track contact state to track the passage of the
                // train on top of the contact. Otherwise, since there is only one contact trigger, the trigger of a contact
                // implies that the train had left the "from" block
                if (trackingResult.Train.TrackContactTriggerCount > 1) {
                    _ = new TrackContactPassingStateInfo(LayoutModel.StateManager.Components.StateOf(trackContact, "TrainPassing")) {
                        TrainId = trackingResult.TrainId,
                        FromBlockId = trackingResult.FromBlockId,
                        FromBlockTriggerCount = trackingResult.Train.TrackContactTriggerCount - 1,
                        ToBlockId = trackingResult.ToBlockId,
                        ToBlockTriggerCount = 1,
                        Direction = trackingResult.Train.Speed >= 0 ? LocomotiveOrientation.Forward : LocomotiveOrientation.Backward
                    };
                }
                else
                    trackContact.IsTriggered = false;
            }

            Dispatch.Notification.OnTrainCrossedBlockEdge(trackingResult.Train, trackingResult.BlockEdge);
        }
        #endregion

        #endregion

        #region Occupancy Block (train detection) based train tracking

        [DispatchTarget]
        private void OnTrainDetectionBlockWillBeOccupied(LayoutOccupancyBlock occupancyBlock) {
            Trace.WriteLineIf(traceLocomotiveTracking.TraceInfo, $"===== Train detection block becomes occupied, block: {occupancyBlock.BlockDefinintion.FullDescription}");

            // Check if any contained block contains train. If so, then the detection was of this train, and nothing should be done
            foreach (LayoutBlock block in occupancyBlock.ContainedBlocks) {
                if (block.HasTrains)
                    return;

                if (block.CheckForTrainRedetection()) {
                    Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, " Train re-detected in a block that it has left than 250ms ago");
                    return;
                }
            }

            var extendedTrainBlock = IsExtendingTrain(occupancyBlock);

            if (extendedTrainBlock != null)
                Dispatch.Call.RequestAutoExtendTrain(extendedTrainBlock.Trains[0].Train, extendedTrainBlock.BlockDefinintion);
            else {
                try {
                    ArrayList trackingResults = new();

                    // The contained blocks are empty, yet a train is detected. Look in all the neighboring blocks and check which one
                    // contain trains which may have moved to this block
                    foreach (LayoutBlockEdgeComponent blockEdge in occupancyBlock.BlockEdges) {
                        try {
                            var track = blockEdge.Track;

                            if (track != null) {
                                var destinationBlock = track.GetBlock(track.ConnectionPoints[0]);

                                if (destinationBlock.OccupancyBlock == null || destinationBlock.OccupancyBlock.Id != occupancyBlock.Id)
                                    destinationBlock = track.GetBlock(track.ConnectionPoints[1]);

                                var trainLockingDestination = GetTrainLockingBlock(destinationBlock);

                                Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, $">>> Checking if train crossed {blockEdge.FullDescription} to get to occupancy block {occupancyBlock.BlockDefinintion.FullDescription}");
                                LocomotiveTrackingResult trackingResult = TrackTrainMovingTo(destinationBlock, trainLockingDestination, blockEdge);

                                if (trackingResult != null) {
                                    if (trainLockingDestination != null && trackingResult.Train.Id == trainLockingDestination.Id) {
                                        Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, ">>> Found tracking result for the train locking destination, use only this one!");

                                        foreach (LocomotiveTrackingResult removedResult in trackingResults)
                                            removedResult.Rollback();
                                        trackingResults.Clear();
                                        trackingResults.Add(trackingResult);
                                        break;
                                    }
                                    else {
                                        Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, ">>> It is possible");
                                        trackingResults.Add(trackingResult);
                                    }
                                }
                            }
                        }
                        catch (BlockEdgeCrossingException ex) {
                            Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, $">>> Train did not cross {blockEdge.FullDescription} to get to {occupancyBlock.BlockDefinintion.FullDescription} because {ex.Message}");
                        }
                    }

                    if (trackingResults.Count == 1) {
                        LocomotiveTrackingResult trackingResult = (LocomotiveTrackingResult)trackingResults[0]!;

                        trackingResult.Commit();        // Send the events for entering all the non-feedback blocks (if any) that were entered

                        TrainStateInfo train = trackingResult.Train;

                        train.LastCrossedBlockEdge = trackingResult.BlockEdge;
                        if (train.Managed)
                            train.LastBlockEdgeCrossingSpeed = train.Speed;

                        TrainPart fromLocationTrainPart = train.LocationOfBlock(trackingResult.FromBlock)!.TrainPart;

                        train.EnterBlock(fromLocationTrainPart,
                            trackingResult.ToBlock, trackingResult.BlockEdge, null);

                        RemovePhantomTrainLocations(train, fromLocationTrainPart);

                        Dispatch.Notification.OnTrainEnteredBlock(trackingResult.Train, trackingResult.ToBlock);
                        Dispatch.Notification.OnTrainCrossedBlockEdge(train, trackingResult.BlockEdge);
                        Dispatch.Notification.OnOccupancyBlockEdgeCrossed(trackingResult.BlockEdge, train);
                    }
                    else if (trackingResults.Count == 0) {
                        if (switchBreakOnBadLocomotiveTracking.Enabled)
                            Debugger.Break();

                        // Try to figure out which train is the "run-away" train. This is done by looking at the neighboring
                        // blocks and see if any of them is locked by a train, if it is, check if the train is moving, if so
                        // trace back to see which multi-path component seems to have function wrong.
                        //
                        // If the train is found, it is stopped, if it was moved due an active trip, the trip is aborted

                        // FindRunawayTrain will throw exception if it will identify the runaway train
                        foreach (LayoutBlock block in occupancyBlock.ContainedBlocks)
                            FindRunawayTrain(block);

                        throw new UnexpectedTrainDetectionException(occupancyBlock);
                    }
                    else if (trackingResults.Count > 1) {
                        if (switchBreakOnBadLocomotiveTracking.Enabled)
                            Debugger.Break();
                        throw new AmbiguousTrainDetectionException(occupancyBlock);
                    }
                }
                catch (TrainDetectionException ex) {
                    ex.Report();
                }
            }
        }

        /// <summary>
        /// Check if block become occupied because a train is being extended.
        /// </summary>
        /// <param name="occupancyBlock">The block in which new train was detected</param>
        /// <returns>The block containing the train being extended or null if the train detection is not due to extending a train</returns>
        private LayoutBlock? IsExtendingTrain(LayoutOccupancyBlock occupancyBlock) {
            LayoutBlockDefinitionComponent blockDefinition = occupancyBlock.BlockDefinintion;
            LayoutBlock? blockWithExtendedTrain = null;
            TrainStateInfo? lockingTrain = null;

            foreach (LayoutBlock block in occupancyBlock.ContainedBlocks) {
                if (block.LockRequest != null)
                    lockingTrain = LayoutModel.StateManager.Trains[block.LockRequest.OwnerId];

                if (lockingTrain != null)
                    break;
            }

            foreach (LayoutBlockEdgeBase blockEdge in blockDefinition.Block.BlockEdges) {
                var otherBlock = occupancyBlock.OtherBlock(blockEdge);

                if (otherBlock != null) {
                    if (otherBlock.Trains.Count > 1)
                        return null;
                    else if (otherBlock.Trains.Count == 1) {
                        var train = otherBlock.Trains[0].Train;
                        var trainBeingExtended = blockWithExtendedTrain?.Trains[0].Train;

                        if (trainBeingExtended != null && trainBeingExtended.Id != train.Id)
                            return null;        // More than one, bail out

                        if (train.Speed == 0)
                            blockWithExtendedTrain = otherBlock;
                    }
                }
            }

            if (blockWithExtendedTrain != null && lockingTrain != null && blockWithExtendedTrain.Trains[0].Id != lockingTrain.Id)
                return null;        // Block is locked, but the suspected extended train is not the one locking the block.

            return blockWithExtendedTrain;
        }

        private void FindRunawayTrain(LayoutBlock block) {
            foreach (LayoutBlockEdgeBase blockEdge in block.BlockEdges) {
                LayoutBlock otherBlock = blockEdge.GetNeighboringBlock(block);

                if (otherBlock.LockRequest != null && !otherBlock.LockRequest.IsManualDispatchLock) {
                    var train = LayoutModel.StateManager.Trains[otherBlock.LockRequest.OwnerId];

                    if (train != null && train.Speed > 0) {
                        // Stop the train, and abort any trip that this train belongs to
                        train.Speed = 0;
                        Dispatch.Call.AbortTrip(train, true);

                        Dispatch.Notification.OnTrainEnteredBlock(train, block);
                        Dispatch.Notification.OnTrainCrossedBlockEdge(train, blockEdge);
                        Dispatch.Notification.OnOccupancyBlockEdgeCrossed(blockEdge, train);

                        if (train != null) {
                            var turnout = FindFaultyTurnout(blockEdge.GetBlockTrackEdge(block), train);

                            if (turnout == null)
                                throw new DetectedRunawayTrainException(block.OccupancyBlock, train);
                            else
                                throw new DetectedRunawayTrainAndFaultyTurnout(block.OccupancyBlock, train, turnout);
                        }
                    }
                }
            }
        }

        private IModelComponentIsMultiPath? FindFaultyTurnout(TrackEdge scanFrom, TrainStateInfo train) {
            Stack<TrackEdge> scanStack = new();

            scanStack.Push(scanFrom);

            while (scanStack.Count > 0) {
                TrackEdge edge = scanStack.Pop();
                bool continueScan = true;

                do {
                    var blockDefinition = edge.Track.BlockDefinitionComponent;

                    if (blockDefinition != null) {
                        var lockRequest = blockDefinition.Block.LockRequest;

                        // If got to block not locked by the train, we are not on the way to the faulty turnout...
                        if (lockRequest == null || lockRequest.OwnerId != train.Id)
                            continueScan = false;
                        if (blockDefinition.Block.IsTrainInBlock(train))
                            continueScan = false;
                    }
                    else {
                        if (edge.Track is IModelComponentIsMultiPath turnout) {
                            // Since this is merge point in a block locked by the train, it had to be set correctly
                            // if arrived to this from the wrong block, its setting was incorrect
                            if (turnout.IsSplitPoint(edge.ConnectionPoint) && turnout.IsMergePoint(edge.ConnectionPoint))
                                return turnout;
                            else if (turnout.IsMergePoint(edge.ConnectionPoint)) {
                                LayoutComponentConnectionPoint tip = turnout.ConnectTo(edge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0];

                                // The turnout switch state does not connect from the tip 
                                if (turnout.ConnectTo(tip, turnout.CurrentSwitchState) != edge.ConnectionPoint)
                                    return turnout;
                            }
                            else        // Split point...
                                scanStack.Push(turnout.GetConnectedComponentEdge(turnout.ConnectTo(edge.ConnectionPoint, LayoutComponentConnectionType.Passage)[1]));
                        }
                    }

                    if (continueScan)
                        edge = edge.Track.GetConnectedComponentEdge(edge.Track.ConnectTo(edge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0]);

                    if (edge == TrackEdge.Empty)
                        continueScan = false;

                } while (continueScan);
            }

            return null;
        }

        [DispatchTarget]
        private void OnTrainDetectionBlockFree(LayoutOccupancyBlock occupancyBlock) {

            foreach (LayoutBlock block in occupancyBlock.ContainedBlocks) {
                List<TrainLocationInfo> blockTrains = new(block.Trains);    // Take snapshot to avoid changing the collection while iterating

                foreach (TrainLocationInfo trainLocation in blockTrains) {
                    TrainStateInfo train = trainLocation.Train;

                    // The train is removed from the block as long as it will not cause it to be removed
                    // completely
                    if (train.Locations.Count > 1) {
                        var blockEdge = train.LastCrossedBlockEdge;
                        LayoutBlock toBlock;

                        Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, $" Removing train {train.DisplayName} because block is free: {trainLocation.Block.BlockDefinintion.FullDescription}");
                        train.LeaveBlock(block);

                        if (blockEdge?.OptionalTrack != null) {
                            if (blockEdge.Track.GetBlock(blockEdge.Track.ConnectionPoints[0]).Id == block.Id)
                                toBlock = blockEdge.Track.GetBlock(blockEdge.Track.ConnectionPoints[1]);
                            else
                                toBlock = blockEdge.Track.GetBlock(blockEdge.Track.ConnectionPoints[0]);

                            Dispatch.Call.TrainLeavingBlockDetails(blockEdge, new TrainChangingBlock(blockEdge, train, block, toBlock));
                        }

                        bool trainLocationRemoved;

                        // Repeat the loop until not removing any train location. Removal of head/tail train location (non TrainPart.Car)
                        // may cause another train location to become head/tail of the train.
                        do {
                            trainLocationRemoved = false;

                            foreach (TrainLocationInfo trainLocationInTrain in train.Locations) {
                                LayoutBlockDefinitionComponentInfo info = trainLocationInTrain.Block.BlockDefinintion.Info;

                                if (info.NoFeedback && trainLocationInTrain.TrainPart != TrainPart.Car) {
                                    Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, " Removing train non-car from " + train.DisplayName + " block " + trainLocation.Block.BlockDefinintion.FullDescription);
                                    train.LeaveBlock(trainLocationInTrain.Block);
                                    trainLocationRemoved = true;
                                }
                            }
                        } while (trainLocationRemoved);
                    }
                    else
                        block.Redraw();     // Mark this train as no longer detected
                }
            }
        }

        #endregion

        #region Tracking Logic

        private IEnumerable<(LayoutBlock block, int distance, LayoutBlockEdgeBase blockEdge)> EnumBlocksByDistance(LayoutBlock block, LayoutBlockEdgeBase originBlockEdge, int distance = 0) {
            yield return (block, distance, originBlockEdge);
            foreach (var blockEdge in block.BlockEdges.Where(edge => edge != originBlockEdge)) {
                var otherBlock = blockEdge.GetNeighboringBlock(block);

                foreach (var result in EnumBlocksByDistance(otherBlock, blockEdge, distance + 1))
                    yield return result;
            }
        }

        /// <summary>
        /// No train was actually found in any linked block (blocks around the trigged track contact). However a train was found in a nearby block,
        /// In order for the tracking to work correctly this train needs to be relocated to the linked block (with correct orientation i.e. front pointing to the
        /// correct edge). 
        /// </summary>
        /// <param name="linkedBlock">The linked block in which the train should be</param>
        /// <param name="linkedBlockedEdge">The block edge from which the train was found</param>
        /// <param name="blockWithTrain">The block in which the train was actually found</param>
        /// <param name="distance">How many block away (from the linked block) the train was found</param>
        private void RelocateTrainToLinkedBlock(LayoutBlock linkedBlock, LayoutBlockEdgeBase trackContact, LayoutBlock blockWithTrain, int distance) {
            Debug.Assert(blockWithTrain.HasTrains);
            var trainLocationInfo = blockWithTrain.Trains[0];
            var pathFromTrain = (from b in EnumBlocksByDistance(linkedBlock, trackContact) select b).Take(distance + 1).Reverse().ToArray();

            Debug.Assert(pathFromTrain.Length > 0 && pathFromTrain[0].block.HasTrains);

            var blockDefWithTrain = pathFromTrain[0].block.BlockDefinintion;
            var trackEdge = new TrackEdge(blockDefWithTrain.Track,
                blockDefWithTrain.Track.ConnectionPoints[blockDefWithTrain.GetConnectionPointIndex(pathFromTrain[0].blockEdge)]);
            var forwardOrientation = trackEdge.ConnectionPoint == blockWithTrain.Trains[0].DisplayFront;
            var destinationTrack = linkedBlock.BlockDefinintion.Track;
            var front = blockWithTrain.Trains[0].DisplayFront;
            int blockDefinitionsCount = 0;

            while (trackEdge.Track != destinationTrack) {
                var nextTrackEdge = TopologyServices.FindTrackConnectingAt(trackEdge);

                if (nextTrackEdge.Track.BlockDefinitionComponent != null) {
                    blockDefinitionsCount++;
                    Debug.Assert(blockDefinitionsCount <= distance);
                }

                nextTrackEdge = new TrackEdge(nextTrackEdge.Track, nextTrackEdge.OtherConnectionPoint);
                front = forwardOrientation ? nextTrackEdge.ConnectionPoint : nextTrackEdge.OtherConnectionPoint;
                trackEdge = nextTrackEdge;
            }

            Debug.Assert(trackEdge.Track.BlockDefinitionComponent != null && trackEdge.Track.BlockDefinitionComponent!.Id == linkedBlock.BlockDefinintion.Id);

            Trace.WriteLineIf(traceLocomotiveTracking.TraceInfo,
                $"No train was found in blocks around {trackContact.FullDescription} however a train was found in {trainLocationInfo.Block.FullDescription} (distance {distance}), relocating it to {linkedBlock.FullDescription}");
            Dispatch.Call.RelocateTrainRequest(trainLocationInfo.Train, linkedBlock.BlockDefinintion, front);
        }

        private LocomotiveTrackingResult? TrackLocomotivePosition(LayoutBlockEdgeBase trackContact, bool enableFuzzyTracking) {
            var track = trackContact.Track;
            IList<TrainLocationInfo> trains1, trains2;

            TrainMotionListManager motionListManager = new();

            var block1 = track.GetOptionalBlock(track.ConnectionPoints[0]);
            var block2 = track.GetOptionalBlock(track.ConnectionPoints[1]);

            if (enableFuzzyTracking &&
                (block1 == null || block1.Trains.Count == 0) && (block2 == null || block2.Trains.Count == 0)) {
                Trace.WriteLineIf(traceLocomotiveTracking.TraceInfo, $"No trains found around {trackContact.FullDescription} looking for trains in nearby region, assuming train position was not correct");

                const int maxDistance = 2;

                var (fuzzyBlock1, distance1, _) = block1 != null ?
                    (from b in EnumBlocksByDistance(block1, trackContact).TakeWhile(b => b.distance < maxDistance) where b.block.HasTrains select b).FirstOrDefault() :
                    (null, 0, null);
                var (fuzzyBlock2, distance2, _) = block2 != null ?
                    (from b in EnumBlocksByDistance(block2, trackContact).TakeWhile(b => b.distance < maxDistance) where b.block.HasTrains select b).FirstOrDefault() :
                    (null, 0, null);

                if ((fuzzyBlock1 != null || fuzzyBlock2 != null) && (distance1 > 0 || distance2 > 0)) {
                    RelocateTrainToLinkedBlock(linkedBlock: distance1 > 0 ? block1! : block2!,
                                               trackContact: trackContact,
                                               blockWithTrain: distance1 > 0 ? fuzzyBlock1! : fuzzyBlock2!,
                                               distance: distance1 > 0 ? distance1 : distance2);
                }

                block1 ??= fuzzyBlock1;
                block2 ??= fuzzyBlock2;
            }

            Debug.Assert(block1 != null && block2 != null);
            if (block1 == null || block2 == null)
                throw new NullReferenceException("internal: trackLocmotivePosition block1 or block2 are null");

            var block1IsManualDispatch = block1.LockRequest?.IsManualDispatchLock ?? LayoutModel.StateManager.AllLayoutManualDispatch;
            var block2isManualDispatch = block2.LockRequest?.IsManualDispatchLock ?? LayoutModel.StateManager.AllLayoutManualDispatch;

            if ((!block1IsManualDispatch && !block2isManualDispatch) ||
              LayoutModel.StateManager.TrainTrackingOptions.TrainTrackingInManualDispatchRegion == TrainTrackingInManualDispatchRegion.Normal) {
                trains1 = block1.Trains;
                trains2 = block2.Trains;

                if (trains1.Count == 0 && trains2.Count == 0) {
                    // A track contact was triggered, however, both surrounding blocks have no trains
                    trains1 = GetTrainInNoFeedbackBlock(block1, block2, GetTrainLockingBlock(block2), trackContact, motionListManager, new HashSet<Guid>());
                    trains2 = GetTrainInNoFeedbackBlock(block2, block1, GetTrainLockingBlock(block1), trackContact, motionListManager, new HashSet<Guid>());
                }

                // Figure out which case we are dealing with.
                try {
                    LocomotiveTrackingResult? trackingResult = null;

                    if (trains1.Count == 0 && trains2.Count > 0) {
                        trackingResult = MovingFrom(trackContact, block2, block1);
                    }
                    else if (trains2.Count == 0 && trains1.Count > 0) {
                        trackingResult = MovingFrom(trackContact, block1, block2);
                    }
                    else if (trains1.Count > 0 && trains2.Count > 0)
                        trackingResult = AmbiguousMove(trackContact, block1, block2);
                    else {
                        if (enableFuzzyTracking)        // Fuzzy tracking was enabled and yet no locomotive could be found
                            throw new UnexpectedBlockCrossingException(trackContact);
                        else {
                            Trace.WriteLineIf(traceLocomotiveTracking.TraceInfo, $"Could not identify which train crossed track contact {trackContact.FullDescription} - try again with fuzzy tracking enabled");
                            trackingResult = TrackLocomotivePosition(trackContact, true);
                        }
                    }

                    motionListManager.Do();     // By default, commit motions to no feedback loops
                    return trackingResult;
                }
                catch (LayoutException) {
                    motionListManager.Undo();       // Undo motions to no feedback loop
                    throw;
                }
            }
            else if (!block1IsManualDispatch || !block2isManualDispatch)
                throw new CrossingFromManualDispatchRegion(trackContact);

            return null;
        }

        [DispatchTarget(Order = -100)]
        private void OnTrainEnteredBlock(TrainStateInfo train, LayoutBlock block) {
            train.RefreshSpeedLimit();
        }

        [DispatchTarget]
        private void OnTrainEnteredBlock_SetTrainSpeedLimit(TrainStateInfo train, LayoutBlock block) {
            train.RefreshSpeedLimit();
        }

        private LocomotiveTrackingResult TrackTrainMovingTo(LayoutBlock destinationBlock, TrainStateInfo? trainLockingDestination, LayoutBlockEdgeBase blockEdge) {
            var track = blockEdge.Track;
            LayoutBlock sourceBlock;
            IList<TrainLocationInfo> trains;

            TrainMotionListManager motionListManager = new();

            sourceBlock = track.GetBlock(track.ConnectionPoints[0]);
            if (sourceBlock.Id == destinationBlock.Id)
                sourceBlock = track.GetBlock(track.ConnectionPoints[1]);

            trains = GetTrains(sourceBlock, destinationBlock, trainLockingDestination, blockEdge, motionListManager, new HashSet<Guid>());

            // Figure out which case we are dealing with.
            try {
                LocomotiveTrackingResult? trackingResult = null;

                if (trains.Count > 0)
                    trackingResult = MovingFrom(blockEdge, sourceBlock, destinationBlock);
                else
                    throw new UnexpectedBlockCrossingException(blockEdge);

                trackingResult.MotionListManager = motionListManager;

                return trackingResult;
            }
            catch (LayoutException) {
                motionListManager.Undo();       // Undo motions to no feedback loop
                throw;
            }
        }

        private TrainStateInfo? GetTrainLockingBlock(LayoutBlock block) {
            TrainStateInfo? trainLockingBlock = null;

            // Figure out which train (if any) locks the destination block
            if (block.LockRequest != null) {
                trainLockingBlock = LayoutModel.StateManager.Trains[block.LockRequest.OwnerId];

                if (trainLockingBlock != null)
                    Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, $"  train {trainLockingBlock.DisplayName} is locking destination block {block.BlockDefinintion.FullDescription}");
            }

            return trainLockingBlock;
        }

        private static void RemovePhantomTrainLocation(TrainStateInfo train, TrainLocationInfo trainLocation) {
            LayoutBlockDefinitionComponentInfo info = trainLocation.Block.BlockDefinintion.Info;

            if (info.NoFeedback || (info.IsOccupancyDetectionBlock && !info.TrainWillBeDetected)) {
                Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, $" Removing train {train.DisplayName} phantom location {trainLocation.Block.BlockDefinintion.FullDescription}");
                train.LeaveBlock(trainLocation.Block);
            }
        }

        private static void RemovePhantomTrainLocations(TrainStateInfo train, TrainPart addedTrainPart) {
            // Check the number of non-phantom (i.e. locations with actual feedback) of this train
            int nonPhantomTrainLocations = 0;

            foreach (TrainLocationInfo trainLocation in train.Locations) {
                LayoutBlockDefinitionComponentInfo info = trainLocation.Block.BlockDefinintion.Info;

                if (info.IsOccupancyDetectionBlock && info.TrainWillBeDetected)
                    nonPhantomTrainLocations++;
            }

            // If this train has one non-phantom location, remove it from all the phantom locations
            if (nonPhantomTrainLocations == 1) {
                IList<TrainLocationInfo> locations = train.Locations;

                if (addedTrainPart == TrainPart.Locomotive) {
                    // Pass on train starting from the last car towards the first
                    for (int i = locations.Count - 1; i >= 0; i--)
                        RemovePhantomTrainLocation(train, locations[i]);
                }
                else {
                    for (int i = 0; i < locations.Count; i++)
                        RemovePhantomTrainLocation(train, locations[i]);
                }
            }
        }

        /// <summary>
        /// Get the trains in blockWithTrains
        /// </summary>
        /// <param name="blockWithTrains"></param>
        /// <param name="destinationBlock"></param>
        /// <param name="blockEdge"></param>
        /// <returns></returns>
        private IList<TrainLocationInfo> GetTrains(LayoutBlock blockWithTrains, LayoutBlock destinationBlock, TrainStateInfo? trainLockingDestination,
          LayoutBlockEdgeBase blockEdge, TrainMotionListManager motionListManager, HashSet<Guid> visitedBlocks) {
            if (blockWithTrains.BlockDefinintion.Info.NoFeedback && !visitedBlocks.Contains(blockWithTrains.Id)) {
                visitedBlocks.Add(blockWithTrains.Id);
                return GetTrainInNoFeedbackBlock(blockWithTrains, destinationBlock, trainLockingDestination, blockEdge, motionListManager, visitedBlocks);
            }
            else
                return blockWithTrains.Trains;
        }

        /// <summary>
        /// Get the trains that may enter the block on which there is no feedback. Those trains in turn are the candidate to come out from this
        /// block to the destination block.
        /// </summary>
        private IList<TrainLocationInfo> GetTrainInNoFeedbackBlock(LayoutBlock noFeedbackBlock, LayoutBlock destinationBlock, TrainStateInfo? trainLockingDestination,
          LayoutBlockEdgeBase noFeedbackBlockEdge, TrainMotionListManager motionListManager, HashSet<Guid> visitedBlocks) {
            try {
                Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, $"-- getting train in no feedback block {noFeedbackBlock.Name} for finding train in {destinationBlock.Name} crossing {noFeedbackBlockEdge.FullDescription}");

                if (noFeedbackBlock.Trains.Count == 0) {
                    var trackingResults = new List<LocomotiveTrackingResult>();
                    int cpIndex = noFeedbackBlock.BlockDefinintion.GetOtherConnectionPointIndex(noFeedbackBlockEdge);
                    LayoutBlockEdgeBase[] blockEdges = noFeedbackBlock.BlockDefinintion.GetBlockEdges(cpIndex);
                    var otherBlocks = new List<Tuple<LayoutBlock, LayoutBlockEdgeBase>>();

                    foreach (var blockEdge in blockEdges) {
                        if (blockEdge.IsTrackContact())
                            continue;				// The train could not cross track contact without being detected

                        var track = blockEdge.Track!;
                        LayoutBlock otherBlock = track.GetBlock(track.ConnectionPoints[0]);

                        if (otherBlock.Id == noFeedbackBlock.Id)
                            otherBlock = track.GetBlock(track.ConnectionPoints[1]);

                        if (otherBlock.Id != destinationBlock.Id) {
                            otherBlocks.Add(new Tuple<LayoutBlock, LayoutBlockEdgeBase>(otherBlock, blockEdge));

                            if (otherBlock.LockRequest != null && trainLockingDestination != null && otherBlock.LockRequest.OwnerId == trainLockingDestination.Id) {
                                // This block is the most probable candidate, so make sure it is the first in the list
                                var temp = otherBlocks[0];

                                otherBlocks[0] = otherBlocks[^1];
                                otherBlocks[^1] = temp;
                            }
                        }
                        else
                            Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, $"Do not check train originating from {otherBlock.BlockDefinintion.FullDescription}");
                    }

                    foreach (var otherBlockAndEdge in otherBlocks) {
                        LayoutBlock otherBlock = otherBlockAndEdge.Item1;
                        LayoutBlockEdgeBase blockEdge = otherBlockAndEdge.Item2;

                        IList<TrainLocationInfo> otherBlockTrains = GetTrains(otherBlock, noFeedbackBlock, trainLockingDestination, blockEdge, motionListManager, visitedBlocks);

                        try {
                            if (otherBlockTrains.Count > 0) {
                                LocomotiveTrackingResult tracking = MovingFrom(blockEdge, otherBlock, noFeedbackBlock);

                                Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, $" Train {tracking.Train.DisplayName} moving into no-feedback block {tracking.ToBlock.BlockDefinintion.FullDescription} from {tracking.FromBlock.Name} crossing {tracking.BlockEdge.FullDescription}");

                                // If the train that locks the destination can enter this no feedback block, then this is the train!
                                if (trainLockingDestination != null && tracking.TrainId == trainLockingDestination.Id) {
                                    Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, $"  train {trainLockingDestination.DisplayName} locking destination block can enter no-feedback block - selecting only that train");
                                    trackingResults.Clear();
                                    trackingResults.Add(tracking);
                                    break;
                                }
                                else
                                    trackingResults.Add(tracking);
                            }
                        }
                        catch (LayoutException ex) {
                            Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, $"Train could not move because: {ex.Message}");
                        }
                    }

                    if (trackingResults.Count == 1)
                        motionListManager.Add((LocomotiveTrackingResult)trackingResults[0]);
                    else if (trackingResults.Count > 1)
                        throw new AmbiguousForecastForNoFeedbackException(noFeedbackBlock);
                }
            }
            catch (TrainDetectionException ex) {
                motionListManager.Undo();
                ex.Report();
            }

            Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, $"-- Total of {noFeedbackBlock.Trains.Count} trains");

            return noFeedbackBlock.Trains;
        }

        private bool ValidateTrackingResult(LocomotiveTrackingResult trackingResult) {
            // Track each train and see that indeed it gets to the block edge in the tracking result. If not, then this tracking
            // result is not logical (the turnout setting will not drive the train to that block edge).

            if (trackingResult.Train.Speed != 0) {
                ILayoutTopologyServices ts = TopologyServices;
                TrackEdge edge = new(trackingResult.FromBlock.BlockDefinintion.Track, trackingResult.Train.LocationOfBlock(trackingResult.FromBlock)!.DisplayFront);

                //				trackingResult.Dump();

                if (trackingResult.Train.Speed > 0)
                    edge = edge.OtherSide;

                while (edge != TrackEdge.Empty && edge.Track.BlockEdgeBase == null) {
                    if (edge.Track is IModelComponentIsMultiPath turnout) {
                        LayoutComponentConnectionPoint nextCp = turnout.ConnectTo(edge.ConnectionPoint, turnout.CurrentSwitchState);
                        edge = ts.FindTrackConnectingAt(new TrackEdge(edge.Track, nextCp));
                    }
                    else
                        edge = ts.FindTrackConnectingAt(new TrackEdge(edge.Track, edge.Track.ConnectTo(edge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0]));
                }

                if (edge != TrackEdge.Empty) {
                    var blockEdge = edge.Track.BlockEdgeBase;

                    if (blockEdge != null && blockEdge.Id != trackingResult.BlockEdgeId)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Since fromBlock contains at least one locomotive and toBlock contains no locomotives, it is known
        /// that a locomotive is moving from fromBlock to toBlock. Now it is remains to figure out which
        /// locomotive it is...
        /// </summary>
        /// <param name="blockEdge">The block edge that was crossed</param>
        /// <param name="fromBlock">The block from which locomotive is moving</param>
        /// <param name="toBlock">The block to which locomotive is moving</param>
        private LocomotiveTrackingResult MovingFrom(LayoutBlockEdgeBase blockEdge, LayoutBlock fromBlock, LayoutBlock toBlock) {
            LocomotiveTrackingResult trackingResult;

            // Check the simple and common case where fromBlock has only one locomotive. In this case it is obvious
            // which locomotive is moving.
            if (fromBlock.Trains.Count == 1) {
                TrainLocationInfo trainLocation = fromBlock.Trains[0];
                TrainStateInfo trainState = trainLocation.Train;

                if (CanLocomotiveCrossBlockEdge(trainLocation, blockEdge)) {
                    trackingResult = new LocomotiveTrackingResult(blockEdge, trainState, trainLocation, fromBlock, toBlock);
                    if (!ValidateTrackingResult(trackingResult))
                        throw new LocomotiveMotionNotConsistentWithTurnoutSettingException(blockEdge, trainState, fromBlock, toBlock);
                    return trackingResult;
                }
                else
                    throw new InconsistentLocomotiveBlockCrossingException(blockEdge, trainState);
            }
            else {
                TrainLocationInfo? bestCandidate = null;
                long bestCrossingTime = 0;

                if (fromBlock.IsLinear) {
                    // The fromBlock does not contain any turnout, therefore there are two cases:
                    //  a) locomotive crossed from toBlock to fromBlock and now is coming back. In this case
                    //     it must be the locomotive with the latest crossing time.
                    //  b) the crossing locomotive did not originated from toBlock, but from the "other side" of
                    //     fromBlock. In this case this locomotive must be the one with the earliest crossing time
                    // Since this is a linear block, and locomotives cannot bypass each other, only those two cases
                    // exist.

                    // Check for case (a) - if there is a locomotive that crossed the current track contact
                    foreach (TrainLocationInfo trainLocation in fromBlock.Trains) {
                        if (trainLocation.BlockEdgeId == blockEdge.Id) {
                            if (bestCandidate == null || trainLocation.BlockEdgeCrossingTime > bestCrossingTime) {
                                bestCandidate = trainLocation;
                                bestCrossingTime = trainLocation.BlockEdgeCrossingTime;
                            }
                        }
                    }

                    if (bestCandidate == null) {        // No locomotive originated from toBlock
                        foreach (TrainLocationInfo trainLocation in fromBlock.Trains) {
                            TrainStateInfo trainState = new(trainLocation.LocomotiveStateElement);

                            if (trainLocation.BlockEdgeId != Guid.Empty) {
                                if (bestCandidate == null || trainLocation.BlockEdgeCrossingTime < bestCrossingTime) {
                                    bestCandidate = trainLocation;
                                    bestCrossingTime = trainLocation.BlockEdgeCrossingTime;
                                }
                            }
                        }
                    }

                    if (bestCandidate == null)
                        throw new LayoutException("Internal: movingFrom could not find what to move");

                    TrainStateInfo bestCandidateLocoState = new(bestCandidate.LocomotiveStateElement);

                    if (CanLocomotiveCrossBlockEdge(bestCandidate, blockEdge)) {
                        trackingResult = new LocomotiveTrackingResult(blockEdge, bestCandidateLocoState, bestCandidate, fromBlock, toBlock);
                        if (!ValidateTrackingResult(trackingResult))
                            throw new LocomotiveMotionNotConsistentWithTurnoutSettingException(blockEdge, bestCandidateLocoState, fromBlock, toBlock);
                        return trackingResult;
                    }
                    else
                        throw new InconsistentLocomotiveBlockCrossingException(blockEdge, bestCandidateLocoState);
                }
                else {              // Non linear block. Check if there is one locomotive that can trigger the contact
                    int nValid = 0;

                    // Non linear block with more than one locomotive, check if only one locomotive is moving.
                    foreach (TrainLocationInfo trainLocation in fromBlock.Trains) {
                        if (CanLocomotiveCrossBlockEdge(trainLocation, blockEdge)) {
                            bestCandidate = trainLocation;
                            nValid++;
                        }

                        if (nValid > 1)
                            break;
                    }

                    if (nValid == 1 && bestCandidate != null) {
                        // Only one locomotive is moving, it has to be the one...
                        TrainStateInfo bestCandidateLocoState = new(bestCandidate.LocomotiveStateElement);

                        trackingResult = new LocomotiveTrackingResult(blockEdge, bestCandidateLocoState, bestCandidate, fromBlock, toBlock);
                        if (!ValidateTrackingResult(trackingResult))
                            throw new LocomotiveMotionNotConsistentWithTurnoutSettingException(blockEdge, bestCandidateLocoState, fromBlock, toBlock);
                        return trackingResult;
                    }
                    else
                        throw new AmbiguousBlockCrossingException(blockEdge);
                }
            }
        }

        private LocomotiveTrackingResult AmbiguousMove(LayoutBlockEdgeBase blockEdge, LayoutBlock block1, LayoutBlock block2) {
            // Check if any of the block do not contain any locomotive that could trigger the contact and the other
            // block contains at least one, then the loco had to move from the second block to the first one. Otherwise
            // it is an ambiguous.
            LayoutBlock[] blocks = new LayoutBlock[2];
            int[] nValids = new int[2];

            blocks[0] = block1;
            blocks[1] = block2;

            for (int iBlock = 0; iBlock < 2; iBlock++) {
                nValids[iBlock] = 0;

                foreach (TrainLocationInfo trainLocation in blocks[iBlock].Trains) {
                    if (CanLocomotiveCrossBlockEdge(trainLocation, blockEdge))
                        nValids[iBlock]++;
                }
            }

            if (nValids[0] == 0 && nValids[1] > 0)
                return MovingFrom(blockEdge, blocks[1], blocks[0]);
            else if (nValids[0] > 0 && nValids[1] == 0)
                return MovingFrom(blockEdge, blocks[0], blocks[1]);
            else if (nValids[0] == 0 && nValids[1] == 0)
                throw new UnexpectedBlockCrossingException(blockEdge);
            else
                throw new AmbiguousBlockCrossingException(blockEdge);
        }

        /// <summary>
        /// Check if a given locomotive could have triggered a given track contact.
        /// </summary>
        /// <param name="trainState">The locomotive</param>
        /// <param name="blockEdge">The block edge</param>
        /// <returns>True: it could be that locomotive that triggered this track contact</returns>
        private bool CanLocomotiveCrossBlockEdge(TrainLocationInfo trainLocation, LayoutBlockEdgeBase blockEdge) {
            bool result;

            TrainStateInfo trainState = new(trainLocation.LocomotiveStateElement);

            Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, $"! check if train {trainState.DisplayName}({trainState.StatusText}) can cross {blockEdge.FullDescription}");

            if (trainState.NotManaged)
                return true;            // No information, so it might be able to cross this block edge

            if (trainState.Speed == 0) {
                Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "!  No - train speed is 0");
                return false;           // loco is not moving, therefore it could not cross the block edge
            }

            LayoutBlock block = trainLocation.Block;

            Debug.Assert(blockEdge.Track.GetBlock(blockEdge.Track.ConnectionPoints[0]) == block ||
                blockEdge.Track.GetBlock(blockEdge.Track.ConnectionPoints[1]) == block);

            if (block.IsLinear) {
                if (trainState.IsLastBlockEdgeCrossingSpeedKnown) {
                    Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "!  Linear block - last block edge crossing speed is known");
                    Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "!                 testing if locomotive could cross " + blockEdge.FullDescription + " the last block edge crossed is: " + (trainLocation.BlockEdge == null ? "None" : trainLocation.BlockEdge.FullDescription));
                    Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "!                 Train speed " + trainState.Speed + " last crossing speed " + trainState.LastBlockEdgeCrossingSpeed);

                    // There are two cases, the track contact being triggered is the same as the previous one. In this
                    // case the locomotive must go in a reverse direction to the one its crossed the contact.
                    // The other case is that the locomotive is crossing the other track contact, in this case it must
                    // continue in the same direction as the one it had when it crossed the previous one.
                    if (trainLocation.BlockEdgeId == blockEdge.Id)
                        result = (trainState.Speed > 0) != (trainState.LastBlockEdgeCrossingSpeed > 0);
                    else
                        result = (trainState.Speed > 0) == (trainState.LastBlockEdgeCrossingSpeed > 0);
                }
                else if (trainLocation.IsDisplayFrontKnown && block.BlockDefinintion != null) {
                    // Last track contact crossing speed is not know (a locomotive that was just placed on the track
                    // and did not yet crossed any track contact. However, its front was specified. Here there are two
                    // cases:
                    //  (a) The crossed track contact is the one that the locomotive front is pointing to. In this case
                    //      the locomotive must move forward.
                    //  (b) the crossed track contacted is not the one that the locomotive front is pointing to. In this
                    //      case the locomotive must move backward.
                    LayoutBlockDefinitionComponent blockInfo = block.BlockDefinintion;
                    int iFront;

                    if (trainLocation.DisplayFront == blockInfo.Track.ConnectionPoints[0])
                        iFront = 0;
                    else
                        iFront = 1;

                    bool blockEdgeIsInFront = blockInfo.ContainsBlockEdge(iFront, blockEdge);

                    Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "!  Linear block - last block edge crossing speed is not known - front is known");
                    Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "!                 testing if locomotive could cross " + blockEdge.FullDescription);
                    Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "!                 Train speed " + trainState.Speed + (blockEdgeIsInFront ? " block edge is in front" : " block edge is not in front"));

                    if (blockEdgeIsInFront)
                        result = trainState.Speed > 0;
                    else
                        result = trainState.Speed < 0;
                }
                else
                    result = true;      // Nothing can be checked, so it might be the one...
            }
            else {
                LayoutBlockDefinitionComponent blockInfo = block.BlockDefinintion;
                bool blockEdgeInFront = blockInfo.ContainsBlockEdge(trainLocation.DisplayFront, blockEdge);

                Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "!  Non Linear block - testing if locomotive could cross " + blockEdge.FullDescription);
                Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "!                     Train speed " + trainState.Speed + (blockEdgeInFront ? " block edge is in front" : " block edge is not in front"));

                // Check if the block edge that was crossed is in the same edge group as the front. If it is, the locomotive must move backward
                // otherwise it has to move forward
                if (blockEdgeInFront)
                    result = trainState.Speed > 0;
                else
                    result = trainState.Speed < 0;
            }

            Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, $"!!! Train could {(result ? "" : "not ")}cross");

            return result;
        }

        #endregion
    }
}
