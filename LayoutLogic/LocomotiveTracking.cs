using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Logic {
	/// <summary>
	/// Handle locomotive tracking LocomotiveTracking.
	/// </summary>
	[LayoutModule("Locomotive Tracker", UserControl=false)]
	public class LocomotiveTracking : LayoutModuleBase {
		ILayoutTopologyServices	_topologyServices;
		public static LayoutTraceSwitch		traceLocomotiveTracking = new LayoutTraceSwitch("LocomotiveTracking", "Locomotive Tracking");
		static LayoutBooleanSwitch	switchBreakOnBadLocomotiveTracking = new LayoutBooleanSwitch("BreakOnBadLocomotiveTracking", "Locomotive Tracking Break");

		#region Common properties

		protected ILayoutTopologyServices TopologyServices {
			get {
				if(_topologyServices == null)
					_topologyServices = (ILayoutTopologyServices)EventManager.Event(new LayoutEvent(this, "get-topology-services"));
				return _topologyServices;
			}
		}

		#endregion

		#region Track Contact Trigger based tracking

		[LayoutEvent("anonymous-track-contact-triggerd")]
		[LayoutEventDef("train-leaving-block-details", Role=LayoutEventRole.Notification, SenderType=typeof(LayoutTrackContactComponent), InfoType=typeof(TrainChangingBlock))]
		[LayoutEventDef("train-enter-block", Role=LayoutEventRole.Notification, SenderType=typeof(TrainStateInfo), InfoType=typeof(LayoutBlock))]
		[LayoutEventDef("train-crossed-block", Role=LayoutEventRole.Notification, SenderType=typeof(TrainStateInfo), InfoType=typeof(LayoutBlockEdgeBase))]
		[LayoutEventDef("track-contact-triggered", Role=LayoutEventRole.Notification, SenderType=typeof(LayoutTrackContactComponent), InfoType=typeof(TrainStateInfo))]
		private void anonymousTrackContactTriggered(LayoutEvent e) {
			LayoutTrackContactComponent	trackContact = (LayoutTrackContactComponent)e.Sender;

			Trace.WriteLineIf(traceLocomotiveTracking.TraceInfo, "Track contact trigger at " + trackContact.FullDescription);

			try {
				TrainStateInfo					train = null;

				if(LayoutModel.StateManager.Components.Contains(trackContact.Id, "TrainPassing")) {
					TrackContactPassingStateInfo	trackContactPassingState = 
						new TrackContactPassingStateInfo(LayoutModel.StateManager.Components.StateOf(trackContact.Id, "TrainPassing"));

					// Track contact has state. This means that a train with multiple triggers is passing on top of this contact
					// If the train continue to move in the same direction, one less triggers is expected to come for the train
					// which is still in the "from" block, and one more is expected from the train portion that is on the "to" block.
					// If the number of triggers that are expected from the "from" block is 0, then the train had actually left this
					// block, futhermore, the train is no longer "on top" of the track contact and therefore its state information
					// is remove.
					// The same logic but reversing the "to" and "from" is applied if the train moves in an opposite direction
					train = trackContactPassingState.Train;

					LocomotiveOrientation	direction = (train.Speed >= 0) ? LocomotiveOrientation.Forward : LocomotiveOrientation.Backward;

					if(direction == trackContactPassingState.Direction) {
						trackContactPassingState.FromBlockTriggerCount--;
						trackContactPassingState.ToBlockTriggerCount++;

						if(trackContactPassingState.FromBlockTriggerCount == 0) {
							removeTrainFromPhantomBlocks(trackContactPassingState.FromBlock, train);
							train.LeaveBlock(trackContactPassingState.FromBlock);
							trackContactPassingState.Remove();
							EventManager.Event(new LayoutEvent(trackContact, "train-leaving-block-details", 
								null, new TrainChangingBlock(trackContact, train, trackContactPassingState.FromBlock, trackContactPassingState.ToBlock)));
						}
					}
					else {
						trackContactPassingState.ToBlockTriggerCount--;
						trackContactPassingState.FromBlockTriggerCount++;

						if(trackContactPassingState.ToBlockTriggerCount == 0) {
							removeTrainFromPhantomBlocks(trackContactPassingState.ToBlock, train);
							train.LeaveBlock(trackContactPassingState.ToBlock);
							trackContactPassingState.Remove();
							EventManager.Event(new LayoutEvent(trackContact, "train-leaving-block-details", 
								null, new TrainChangingBlock(trackContact, train, trackContactPassingState.FromBlock, trackContactPassingState.ToBlock)));
						}
					}
				}
				else {
					LocomotiveTrackingResult	trackingResult = (LocomotiveTrackingResult)EventManager.Event(new LayoutEvent(trackContact, "track-locomotive-position"));

					train = trackingResult.Train;

					train.LastCrossedBlockEdge = trackingResult.BlockEdge;
					if(train.Managed)
						train.LastBlockEdgeCrossingSpeed = train.Speed;

					TrainPart	fromLocationTrainPart = train.LocationOfBlock(trackingResult.FromBlock).TrainPart;

					// If the train has more than one contact then create a new track contact state to track the passage of the
					// train on top of the contact. Otherwise, since there is only one contact trigger, the trigger of a contact
					// implies that the train had left the "from" block
					if(train.TrackContactTriggerCount > 1) {
						TrackContactPassingStateInfo	trackContactPassingState = new TrackContactPassingStateInfo(LayoutModel.StateManager.Components.StateOf(trackingResult.BlockEdge, "TrainPassing"));

						trackContactPassingState.TrainId = trackingResult.TrainId;
						trackContactPassingState.FromBlockId = trackingResult.FromBlockId;
						trackContactPassingState.FromBlockTriggerCount = trackingResult.Train.TrackContactTriggerCount - 1;
						trackContactPassingState.ToBlockId = trackingResult.ToBlockId;
						trackContactPassingState.ToBlockTriggerCount = 1;
						trackContactPassingState.Direction = trackingResult.Train.Speed >= 0 ? LocomotiveOrientation.Forward : LocomotiveOrientation.Backward;
					}
					else {
						removeTrainFromPhantomBlocks(trackingResult.FromBlock, trackingResult.Train);
						trackingResult.Train.LeaveBlock(trackingResult.FromBlock);
						EventManager.Event(new LayoutEvent(trackContact, "train-leaving-block-details", null, 
							new TrainChangingBlock(trackContact, train, trackingResult.FromBlock, trackingResult.ToBlock)));
					}

					train.EnterBlock(fromLocationTrainPart,
						trackingResult.ToBlock, trackingResult.BlockEdge, "train-enter-block");

				}

				// For now only locomotive tracking is implemented
				EventManager.Event(new LayoutEvent(train, "train-crossed-block", null, trackContact));
				EventManager.Event(new LayoutEvent(trackContact, "track-contact-triggered", null, train));
			} catch(LayoutException lex) {
				lex.Report();
				if(switchBreakOnBadLocomotiveTracking.Enabled)
					Debugger.Break();
			}
		}

		/// <summary>
		/// Remove trains from blocks which surround the given block and are connected via a non-track contact block edge
		/// </summary>
		/// <param name="block">The block from which train is about to be removed</param>
		/// <param name="train">The train that is about to be removed</param>
		private void removeTrainFromPhantomBlocks(IDictionary visitedBlocks, LayoutBlock block, TrainStateInfo train) {
			visitedBlocks.Add(block.Id, block);

			foreach(LayoutBlockEdgeBase blockEdge in block.BlockEdges) {
				if(!blockEdge.IsTrackContact()) {
					LayoutBlock	otherBlock = block.OtherBlock(blockEdge);

					if(!visitedBlocks.Contains(otherBlock.Id)) {
						if(otherBlock.IsTrainInBlock(train)) {
							removeTrainFromPhantomBlocks(visitedBlocks, otherBlock, train);

							train.LeaveBlock(otherBlock);
							EventManager.Event(new LayoutEvent(blockEdge, "train-leaving-block-details", 
								null, new TrainChangingBlock(blockEdge, train, otherBlock, block)));
						}
					}
				}
			}
		}

		private void removeTrainFromPhantomBlocks(LayoutBlock block, TrainStateInfo train) {
			removeTrainFromPhantomBlocks(new Hashtable(), block, train);
		}

		#endregion

		#region Occupancy Block (train detection) based train tracking

		[LayoutEvent("train-detection-block-will-be-occupied")]
		[LayoutEventDef("occupancy-block-edge-crossed", Role=LayoutEventRole.Notification, SenderType=typeof(LayoutBlockEdgeComponent), InfoType=typeof(TrainStateInfo))]
		private void trainDetectionBlockOccupied(LayoutEvent e) {
			LayoutOccupancyBlock	occupancyBlock = (LayoutOccupancyBlock)e.Sender;

			Trace.WriteLineIf(traceLocomotiveTracking.TraceInfo, "===== Train detection block becomes occupied, block: " + occupancyBlock.BlockDefinintion.FullDescription);

            
			// Check if any contained block contains train. If so, then the detection was of this train, and nothing should be done
            foreach(LayoutBlock block in occupancyBlock.ContainedBlocks) {
                if(block.HasTrains)
                    return;

                if(block.CheckForTrainRedetection()) {
                    Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, " Train redetected in a block that it has left than 250ms ago");
                    return;
                }
            }

			LayoutBlock extendedTrainBlock = IsExtendingTrain(occupancyBlock);

			if(extendedTrainBlock != null)
				EventManager.Event(new LayoutEvent(extendedTrainBlock.Trains[0].Train, "request-auto-train-extend", null, extendedTrainBlock.BlockDefinintion));
			else {
				try {
					ArrayList trackingResults = new ArrayList();

					// The contained blocks are empty, yet a train is detected. Look in all the neighboring blocks and check which one
					// contain trains which may have moved to this block
					foreach(LayoutBlockEdgeComponent blockEdge in occupancyBlock.BlockEdges) {
						try {
							LayoutTrackComponent track = blockEdge.Track;

							LayoutBlock destinationBlock = blockEdge.Track.GetBlock(track.ConnectionPoints[0]);

							if(destinationBlock.OccupancyBlock == null || destinationBlock.OccupancyBlock.Id != occupancyBlock.Id)
								destinationBlock = blockEdge.Track.GetBlock(track.ConnectionPoints[1]);

							TrainStateInfo trainLockingDestination = getTrainLockingBlock(destinationBlock);

							Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, ">>> Checking if train crossed " + blockEdge.FullDescription + " to get to occupancy block " + occupancyBlock.BlockDefinintion.FullDescription);
							LocomotiveTrackingResult trackingResult = trackTrainMovingTo(destinationBlock, trainLockingDestination, blockEdge);

							if(trackingResult != null) {
								if(trainLockingDestination != null && trackingResult.Train.Id == trainLockingDestination.Id) {
									Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, ">>> Found tracking result for the train locking destination, use only this one!");

									foreach(LocomotiveTrackingResult removedResult in trackingResults)
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
						catch(BlockEdgeCrossingException ex) {
							Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, ">>> Train did not cross " + blockEdge.FullDescription + " to get to " + occupancyBlock.BlockDefinintion.FullDescription + " because " + ex.Message);
						}
					}

					if(trackingResults.Count == 1) {
						LocomotiveTrackingResult trackingResult = (LocomotiveTrackingResult)trackingResults[0];

						trackingResult.Commit();		// Send the events for entering all the non-feedback blocks (if any) that were entered

						TrainStateInfo train = trackingResult.Train;

						train.LastCrossedBlockEdge = trackingResult.BlockEdge;
						if(train.Managed)
							train.LastBlockEdgeCrossingSpeed = train.Speed;

						TrainPart fromLocationTrainPart = train.LocationOfBlock(trackingResult.FromBlock).TrainPart;

						train.EnterBlock(fromLocationTrainPart,
							trackingResult.ToBlock, trackingResult.BlockEdge, null);

						removePhantomTrainLocations(train, fromLocationTrainPart);

						EventManager.Event(new LayoutEvent(trackingResult.Train, "train-enter-block", null, trackingResult.ToBlock));
						EventManager.Event(new LayoutEvent(train, "train-crossed-block", null, trackingResult.BlockEdge));
						EventManager.Event(new LayoutEvent((LayoutBlockEdgeComponent)trackingResult.BlockEdge,
							"occupancy-block-edge-crossed", null, train));

					}
					else if(trackingResults.Count == 0) {
						if(switchBreakOnBadLocomotiveTracking.Enabled)
							Debugger.Break();

						// Try to figure out which train is the "run-away" train. This is done by looking at the neigboring
						// blocks and see if any of them is locked by a train, if it is, check if the train is moving, if so
						// trace back to see which multi-path component seems to have function wrong.
						//
						// If the train is found, it is stopped, if it was moved due an active trip, the trip is abored

						// FindRunawayTrain will throw execption if it will indentify the runaway train
						foreach(LayoutBlock block in occupancyBlock.ContainedBlocks)
							findRunawayTrain(block);

						throw new UnexpectedTrainDetectionException(occupancyBlock);
					}
					else if(trackingResults.Count > 1) {
						if(switchBreakOnBadLocomotiveTracking.Enabled)
							Debugger.Break();
						throw new AmbiguousTrainDetectionException(occupancyBlock);
					}
				}
				catch(TrainDetectionException ex) {
					ex.Report();
				}
			}
		}

		/// <summary>
		/// Check if block become occupied because a train is being extended.
		/// </summary>
		/// <param name="occupancyBlock">The block in which new train was detected</param>
		/// <returns>The block containing the train being extended or null if the train detection is not due to extending a train</returns>
		private LayoutBlock IsExtendingTrain(LayoutOccupancyBlock occupancyBlock) {
			LayoutBlockDefinitionComponent blockDefinition = occupancyBlock.BlockDefinintion;
			LayoutBlock blockWithExtendedTrain = null;
			TrainStateInfo lockingTrain = null;

			foreach(LayoutBlock block in occupancyBlock.ContainedBlocks) {
				if(block.LockRequest != null)
					lockingTrain = LayoutModel.StateManager.Trains[block.LockRequest.OwnerId];

				if(lockingTrain != null)
					break;
			}

			foreach(LayoutBlockEdgeBase blockEdge in blockDefinition.Block.BlockEdges) {
				LayoutBlock otherBlock = occupancyBlock.OtherBlock(blockEdge);

				if(otherBlock != null) {
					if(otherBlock.Trains.Count > 1)
						return null;
					else if(otherBlock.Trains.Count == 1) {
						TrainStateInfo train = otherBlock.Trains[0].Train;
						TrainStateInfo trainBeingExtended = blockWithExtendedTrain == null ? null : blockWithExtendedTrain.Trains[0].Train;

						if(trainBeingExtended != null && trainBeingExtended.Id != train.Id)
							return null;		// More than one, bail out

						if(train.Speed == 0)
							blockWithExtendedTrain = otherBlock;
					}
				}
			}

			if(blockWithExtendedTrain != null && lockingTrain != null && blockWithExtendedTrain.Trains[0].Id != lockingTrain.Id)
				return null;		// Block is locked, but the suspected extended train is not the one locking the block.

			return blockWithExtendedTrain;
		}

		private void findRunawayTrain(LayoutBlock block) {
			foreach(LayoutBlockEdgeBase blockEdge in block.BlockEdges) {
				LayoutBlock otherBlock = blockEdge.GetNeighboringBlock(block);

				if(otherBlock.LockRequest != null && !otherBlock.LockRequest.IsManualDispatchLock) {
					TrainStateInfo train = LayoutModel.StateManager.Trains[otherBlock.LockRequest.OwnerId];

					if(train.Speed > 0) {
						// Stop the train, and abort any trip that this train belongs to
						train.Speed = 0;
						EventManager.Event(new LayoutEvent(train, "abort-trip", null, true));

						EventManager.Event(new LayoutEvent(train, "train-enter-block", null, block));
						EventManager.Event(new LayoutEvent(train, "train-crossed-block", null, blockEdge));
						EventManager.Event(new LayoutEvent((LayoutBlockEdgeComponent)blockEdge,
							"occupancy-block-edge-crossed", null, train));

						if(train != null) {
							IModelComponentIsMultiPath turnout = findFaultyTurnout(blockEdge.GetBlockTrackEdge(block), train);

							if(turnout == null)
								throw new DetectedRunawayTrainException(block.OccupancyBlock, train);
							else
								throw new DetectedRunawayTrainAndFaultyTurnout(block.OccupancyBlock, train, turnout);
						}
					}
				}
			}
		}

		private IModelComponentIsMultiPath findFaultyTurnout(TrackEdge scanFrom, TrainStateInfo train) {
			Stack<TrackEdge> scanStack = new Stack<TrackEdge>();

			scanStack.Push(scanFrom);

			while(scanStack.Count > 0) {
				TrackEdge edge = scanStack.Pop();
				bool continueScan = true;

				do {
					if(edge.Track.BlockDefinitionComponent != null) {
						// If got to block not locked by the train, we are not on the way to the faulty turnout...
						if(edge.Track.BlockDefinitionComponent.Block.LockRequest == null || edge.Track.BlockDefinitionComponent.Block.LockRequest.OwnerId != train.Id)
							continueScan = false;
						if(edge.Track.BlockDefinitionComponent.Block.IsTrainInBlock(train))
							continueScan = false;
					}
					else {
						IModelComponentIsMultiPath turnout = edge.Track as IModelComponentIsMultiPath;

						if(turnout != null) {
							// Since this is merge point in a block locked by the train, it had to be set correctly
							// if arrived to this from the wrong block, its setting was incorrect
							if(turnout.IsSplitPoint(edge.ConnectionPoint) && turnout.IsMergePoint(edge.ConnectionPoint))
								return turnout;
							else if(turnout.IsMergePoint(edge.ConnectionPoint)) {
								LayoutComponentConnectionPoint tip = turnout.ConnectTo(edge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0];

								// The turnout switch state does not connect from the tip 
								if(turnout.ConnectTo(tip, turnout.CurrentSwitchState) != edge.ConnectionPoint)
									return turnout;
							}
							else		// Split point...
								scanStack.Push(turnout.GetConnectedComponentEdge(turnout.ConnectTo(edge.ConnectionPoint, LayoutComponentConnectionType.Passage)[1]));
						}
					}

					if(continueScan)
						edge = edge.Track.GetConnectedComponentEdge(edge.Track.ConnectTo(edge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0]);

					if(edge == TrackEdge.Empty)
						continueScan = false;

				} while(continueScan);
			}

			return null;
		}

		[LayoutEvent("train-detection-block-free")]
		private void trainDetectionBlockFree(LayoutEvent e) {
			LayoutOccupancyBlock	occupancyBlock = (LayoutOccupancyBlock)e.Sender;

			foreach(LayoutBlock block in occupancyBlock.ContainedBlocks) {
				List<TrainLocationInfo> blockTrains = new List<TrainLocationInfo>(block.Trains);	// Take snapshot to avoid changing the collection while iterating

				foreach(TrainLocationInfo trainLocation in blockTrains) {
					TrainStateInfo				train = trainLocation.Train;

					// The train is removed from the block as long as it will not cause it to be removed
					// completely
					if(train.Locations.Count > 1) {
						LayoutBlockEdgeBase	blockEdge = train.LastCrossedBlockEdge;
						LayoutBlock					toBlock;

						Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, " Removing train " + train.DisplayName + " because block is free: " + trainLocation.Block.BlockDefinintion.FullDescription);
						train.LeaveBlock(block);

						if(blockEdge != null) {
							if(blockEdge.Track.GetBlock(blockEdge.Track.ConnectionPoints[0]).Id == block.Id)
								toBlock = blockEdge.Track.GetBlock(blockEdge.Track.ConnectionPoints[1]);
							else
								toBlock = blockEdge.Track.GetBlock(blockEdge.Track.ConnectionPoints[0]);

							EventManager.Event(new LayoutEvent(blockEdge, "train-leaving-block-details", null, 
								new TrainChangingBlock(blockEdge, train, block, toBlock)));
						}

						bool	trainLocationRemoved;

						// Repeat the loop until not removing any train location. Removal of head/tain train location (non TrainPart.Car)
						// may cause another train location to become head/tail of the train.
						do {
							trainLocationRemoved = false;

							foreach(TrainLocationInfo trainLocationInTrain in train.Locations) {
								LayoutBlockDefinitionComponentInfo	info = trainLocationInTrain.Block.BlockDefinintion.Info;

								if(info.NoFeedback && trainLocationInTrain.TrainPart != TrainPart.Car) {
									Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, " Removing train non-car from " + train.DisplayName + " block " + trainLocation.Block.BlockDefinintion.FullDescription);
									train.LeaveBlock(trainLocationInTrain.Block);
									trainLocationRemoved = true;
								}
							}
						} while(trainLocationRemoved);
					}
					else
						block.Redraw();		// Mark this train as no longer detected
				}
			}
		}

		#endregion

		#region Tracking Logic

		[LayoutEvent("track-locomotive-position")]
		private void trackLocomotivePosition(LayoutEvent e) {
			LayoutBlockEdgeBase		blockEdge = (LayoutBlockEdgeBase)e.Sender;
			LayoutStraightTrackComponent	track = blockEdge.Track;
			LayoutBlock						block1, block2;
			IList<TrainLocationInfo>		trains1, trains2;

			TrainMotionListManager	motionListManager = new TrainMotionListManager();

			block1 = track.GetBlock(track.ConnectionPoints[0]);
			block2 = track.GetBlock(track.ConnectionPoints[1]);

			trains1 = block1.Trains;
			trains2 = block2.Trains;

			if(trains1.Count == 0 && trains2.Count == 0) {
				// A track contact was triggered, however, both surrounding blocks have no trains
				trains1 = getTrainInNoFeedbackBlock(block1, block2, getTrainLockingBlock(block2), blockEdge, motionListManager);
				trains2 = getTrainInNoFeedbackBlock(block2, block1, getTrainLockingBlock(block1), blockEdge, motionListManager);
			}

			// Figure out which case we are dealing with.
			try {
				LocomotiveTrackingResult	trackingResult = null;

				if(trains1.Count == 0 && trains2.Count > 0) {
					trackingResult = movingFrom(blockEdge, block2, block1);
				}
				else if(trains2.Count == 0 && trains1.Count > 0) {
					trackingResult = movingFrom(blockEdge, block1, block2);
				}
				else if(trains1.Count > 0 && trains2.Count > 0)
					trackingResult = ambiguousMove(blockEdge, block1, block2);
				else
					throw new UnexpectedBlockCrossingException(blockEdge);

				e.Info = trackingResult;

				motionListManager.Do();		// By default, commit motions to no feedback loops
			} catch(LayoutException) {
				motionListManager.Undo();		// Undo motions to no feedback loop
				throw;
			}
		}

		[LayoutEvent("train-enter-block", Order=-100)]
		[LayoutEvent("train-leave-block", Order=-100)]
		private void setTrainSpeedLimitOnBlockEntrance(LayoutEvent e) {
			TrainStateInfo	train = (TrainStateInfo)e.Sender;

			train.RefreshSpeedLimit();
		}

		private LocomotiveTrackingResult trackTrainMovingTo(LayoutBlock destinationBlock, TrainStateInfo trainLockingDestination, LayoutBlockEdgeBase blockEdge) {
			LayoutStraightTrackComponent	track = blockEdge.Track;
			LayoutBlock						sourceBlock;
			IList<TrainLocationInfo>		trains;

			TrainMotionListManager	motionListManager = new TrainMotionListManager();

			sourceBlock = track.GetBlock(track.ConnectionPoints[0]);
			if(sourceBlock.Id == destinationBlock.Id)
				sourceBlock = track.GetBlock(track.ConnectionPoints[1]);

			trains = getTrains(sourceBlock, destinationBlock, trainLockingDestination, blockEdge, motionListManager);

			// Figure out which case we are dealing with.
			try {
				LocomotiveTrackingResult	trackingResult = null;

				if(trains.Count > 0)
					trackingResult = movingFrom(blockEdge, sourceBlock, destinationBlock);
				else
					throw new UnexpectedBlockCrossingException(blockEdge);

				trackingResult.MotionListManager = motionListManager;

				return trackingResult;
			} catch(LayoutException) {
				motionListManager.Undo();		// Undo motions to no feedback loop
				throw;
			}
		}

		private TrainStateInfo getTrainLockingBlock(LayoutBlock block) {
			TrainStateInfo	trainLockingBlock = null;

			// Figure out which train (if any) locks the destination block
			if(block.LockRequest != null) {
				trainLockingBlock = LayoutModel.StateManager.Trains[block.LockRequest.OwnerId];

				if(trainLockingBlock != null)
					Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "  train " + trainLockingBlock.DisplayName + " is locking destination block " + block.BlockDefinintion.FullDescription);
			}

			return trainLockingBlock;
		}

		private static void removePhantomTrainLocation(TrainStateInfo train, TrainLocationInfo trainLocation) {
			LayoutBlockDefinitionComponentInfo	info = trainLocation.Block.BlockDefinintion.Info;

			if(info.NoFeedback || (info.IsOccupancyDetectionBlock && !info.TrainWillBeDetected)) {
				Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, " Removing train " + train.DisplayName + " phantom location " + trainLocation.Block.BlockDefinintion.FullDescription);
				train.LeaveBlock(trainLocation.Block);
			}
		}

		private static void removePhantomTrainLocations(TrainStateInfo train, TrainPart addedTrainPart) {
			// Check the number of non-phantom (i.e. locations with actual feedback) of this train
			int		nonPhantomTrainLocations = 0;

			foreach(TrainLocationInfo trainLocation in train.Locations) {
				LayoutBlockDefinitionComponentInfo	info = trainLocation.Block.BlockDefinintion.Info;

				if(info.IsOccupancyDetectionBlock && info.TrainWillBeDetected)
					nonPhantomTrainLocations++;
			}

			// If this train has one non-phantom location, remove it from all the phantom locations
			if(nonPhantomTrainLocations == 1) {
				IList<TrainLocationInfo>	locations = train.Locations;

				if(addedTrainPart == TrainPart.Locomotive) {
					// Pass on train starting from the last car towards the first
					for(int i = locations.Count-1; i >= 0; i--)
						removePhantomTrainLocation(train, locations[i]);
				}
				else {
					for(int i = 0; i < locations.Count; i++)
						removePhantomTrainLocation(train, locations[i]);
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
		private IList<TrainLocationInfo> getTrains(LayoutBlock blockWithTrains, LayoutBlock destinationBlock, TrainStateInfo trainLockingDestination, LayoutBlockEdgeBase blockEdge, TrainMotionListManager motionListManager) {
            if(blockWithTrains.BlockDefinintion.Info.NoFeedback)
                return getTrainInNoFeedbackBlock(blockWithTrains, destinationBlock, trainLockingDestination, blockEdge, motionListManager);
            return blockWithTrains.Trains;
		}

		/// <summary>
		/// Get the trains that may enter the block on which there is no feedback. Those trains in turn are the cadidate to come out from this
		/// block to the destination block.
		/// </summary>
		IList<TrainLocationInfo> getTrainInNoFeedbackBlock(LayoutBlock noFeedbackBlock, LayoutBlock destinationBlock, TrainStateInfo trainLockingDestination, LayoutBlockEdgeBase noFeedbackBlockEdge, TrainMotionListManager motionListManager) {
			try {
				Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "-- getting train in no feedback block " + noFeedbackBlock.Name + " for finding train in " + destinationBlock.Name + " crossing " + noFeedbackBlockEdge.FullDescription);

				if(noFeedbackBlock.Trains.Count  == 0) {
					ArrayList				trackingResults = new ArrayList();
					int						cpIndex = noFeedbackBlock.BlockDefinintion.GetOtherConnectionPointIndex(noFeedbackBlockEdge);
					LayoutBlockEdgeBase[]	blockEdges = noFeedbackBlock.BlockDefinintion.GetBlockEdges(cpIndex);

					// The contained blocks are empty, yet a train is detected. Look in the neighboring blocks that can be reached from 
					// the "opposite side" of that of the block edge that was crossed and check which one
					// contain trains which may have moved to this block
					foreach(LayoutBlockEdgeBase blockEdge in blockEdges) {
						if(blockEdge.IsTrackContact())
							continue;				// The train could not cross track contact without being detected

						LayoutTrackComponent	track = blockEdge.Track;
						LayoutBlock				otherBlock = track.GetBlock(track.ConnectionPoints[0]);

						if(otherBlock.Id == noFeedbackBlock.Id)
							otherBlock = track.GetBlock(track.ConnectionPoints[1]);

						Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "Checking trains in " + otherBlock.BlockDefinintion.FullDescription + " it has " + otherBlock.Trains.Count + " trains");

						if(otherBlock.Id != destinationBlock.Id) {
							IList<TrainLocationInfo>	otherBlockTrains = getTrains(otherBlock, noFeedbackBlock, trainLockingDestination, blockEdge, motionListManager);

							try {
								if(otherBlockTrains.Count > 0) {
									LocomotiveTrackingResult	tracking = movingFrom(blockEdge, otherBlock, noFeedbackBlock);

									Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, " Train " + tracking.Train.DisplayName + " moving into nofeedback block " + tracking.ToBlock.BlockDefinintion.FullDescription + " from " + tracking.FromBlock.Name + " crossing " + tracking.BlockEdge.FullDescription);

									// If the train that locks the destination can enter this no feedback block, then this is the train!
									if(trainLockingDestination != null && tracking.TrainId == trainLockingDestination.Id) {
										Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "  train " + trainLockingDestination.DisplayName + " locking destination block can enter no-feedback block - selecting only that train");
										trackingResults.Clear();
										trackingResults.Add(tracking);
										break;
									}
									else
										trackingResults.Add(tracking);
								}
							} catch(LayoutException ex) {
								Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "Train could not move because: " + ex.Message);
							}
						}
						else
							Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "Do not check train originating from " + otherBlock.BlockDefinintion.FullDescription);
					}

					if(trackingResults.Count == 1)
						motionListManager.Add((LocomotiveTrackingResult)trackingResults[0]);
					else if(trackingResults.Count > 1)
						throw new AmbiguousForecastForNoFeedbackException(noFeedbackBlock);
				}
			} catch(TrainDetectionException ex) {
				motionListManager.Undo();
				ex.Report();
			}

			Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "-- Total of " + noFeedbackBlock.Trains.Count + " trains");

			return noFeedbackBlock.Trains;
		}

		private bool validateTrackingResult(LocomotiveTrackingResult trackingResult) {
			// Track each train and see that indeed it gets to the block edge in the tracking result. If not, then this tracking
			// result is not logical (the turnout setting will not drive the train to that block edge).

			if(trackingResult.Train.Speed != 0) {
				ILayoutTopologyServices	ts = TopologyServices;
				TrackEdge				edge = new TrackEdge(trackingResult.FromBlock.BlockDefinintion.Track, trackingResult.Train.LocationOfBlock(trackingResult.FromBlock).DisplayFront);

//				trackingResult.Dump();

				if(trackingResult.Train.Speed > 0)
					edge = edge.OtherSide;

				while(edge != TrackEdge.Empty && edge.Track.BlockEdgeBase == null) {
					if(edge.Track is IModelComponentIsMultiPath) {
						IModelComponentIsMultiPath		turnout = (IModelComponentIsMultiPath)edge.Track;
						LayoutComponentConnectionPoint	nextCp;

						nextCp = turnout.ConnectTo(edge.ConnectionPoint, turnout.CurrentSwitchState);
						edge = ts.FindTrackConnectingAt(new TrackEdge(edge.Track, nextCp));
					}
					else
						edge = ts.FindTrackConnectingAt(new TrackEdge(edge.Track, edge.Track.ConnectTo(edge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0]));
				}

				if(edge != TrackEdge.Empty && edge.Track.BlockEdgeBase.Id != trackingResult.BlockEdgeId)
					return false;
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
		private LocomotiveTrackingResult movingFrom(LayoutBlockEdgeBase blockEdge, LayoutBlock fromBlock, LayoutBlock toBlock) {
			LocomotiveTrackingResult	trackingResult;

			// Check the simple and common case where fromBlock has only one locomotive. In this case it is obvious
			// which locomotive is moving.
			if(fromBlock.Trains.Count == 1) {
				TrainLocationInfo	trainLocation = fromBlock.Trains[0];
				TrainStateInfo		trainState = trainLocation.Train;

				if(canLocomotiveCrossBlockEdge(trainLocation, blockEdge)) {
					trackingResult = new LocomotiveTrackingResult(blockEdge, trainState, trainLocation, fromBlock, toBlock);
					if(!validateTrackingResult(trackingResult))
						throw new LocomotiveMotionNotConsistentWithTurnoutSettingException(blockEdge, trainState, fromBlock, toBlock);
					return trackingResult;
				}
				else
					throw new InconsistentLocomotiveBlockCrossingException(blockEdge, trainState);
			}
			else {
				TrainLocationInfo	bestCandidate = null;
				long				bestCrossingTime = 0;

				if(fromBlock.IsLinear) {
					// The fromBlock does not contain any turnout, therefore there are two cases:
					//  a) locomotive crossed from toBlock to fromBlock and now is coming back. In this case
					//     it must be the locomotive with the latest crossing time.
					//  b) the crossing locomotive did not originated from toBlock, but from the "other side" of
					//     fromBlock. In this case this locomotive must be the one with the earliest crossing time
					// Since this is a linear block, and locomotives cannot bypass each other, only those two cases
					// exist.

					// Check for case (a) - if there is a locomotive that crossed the current track contact
					foreach(TrainLocationInfo trainLocation in fromBlock.Trains) {
						if(trainLocation.BlockEdgeId == blockEdge.Id) {
							if(bestCandidate == null || trainLocation.BlockEdgeCrossingTime > bestCrossingTime) {
								bestCandidate = trainLocation;
								bestCrossingTime = trainLocation.BlockEdgeCrossingTime;
							}
						}
					}

					if(bestCandidate == null) {		// No locomotive originated from toBlock
						foreach(TrainLocationInfo trainLocation in fromBlock.Trains) {
							TrainStateInfo	trainState = new TrainStateInfo(trainLocation.LocomotiveStateElement);

							if(trainLocation.BlockEdgeId != Guid.Empty) {
								if(bestCandidate == null || trainLocation.BlockEdgeCrossingTime < bestCrossingTime) {
									bestCandidate = trainLocation;
									bestCrossingTime = trainLocation.BlockEdgeCrossingTime;
								}
							}
						}
					}

					Debug.Assert(bestCandidate != null);
					TrainStateInfo		bestCandidateLocoState = new TrainStateInfo(bestCandidate.LocomotiveStateElement);

					if(canLocomotiveCrossBlockEdge(bestCandidate, blockEdge)) {
						trackingResult = new LocomotiveTrackingResult(blockEdge, bestCandidateLocoState, bestCandidate, fromBlock, toBlock);
						if(!validateTrackingResult(trackingResult))
							throw new LocomotiveMotionNotConsistentWithTurnoutSettingException(blockEdge, bestCandidateLocoState, fromBlock, toBlock);
						return trackingResult;
					}
					else
						throw new InconsistentLocomotiveBlockCrossingException(blockEdge, bestCandidateLocoState);
				}
				else {				// Non linear block. Check if there is one locomotive that can trigger the contact
					int		nValid = 0;

					// Non linear block with more than one locomotive, check if only one locomotive is moving.
					foreach(TrainLocationInfo trainLocation in fromBlock.Trains) {
						if(canLocomotiveCrossBlockEdge(trainLocation, blockEdge)) {
							bestCandidate = trainLocation;
							nValid++;
						}

						if(nValid > 1)
							break;
					}

					if(nValid == 1) {
						// Only one locomotive is moving, it has to be the one...
						TrainStateInfo		bestCandidateLocoState = new TrainStateInfo(bestCandidate.LocomotiveStateElement);
	
						trackingResult = new LocomotiveTrackingResult(blockEdge, bestCandidateLocoState, bestCandidate, fromBlock, toBlock);
						if(!validateTrackingResult(trackingResult))
							throw new LocomotiveMotionNotConsistentWithTurnoutSettingException(blockEdge, bestCandidateLocoState, fromBlock, toBlock);
						return trackingResult;
					}
					else
						throw new AmbiguousBlockCrossingException(blockEdge);
				}
			}
		}

		private LocomotiveTrackingResult ambiguousMove(LayoutBlockEdgeBase blockEdge, LayoutBlock block1, LayoutBlock block2) {
			// Check if any of the block do not contain any locomotive that could trigger the contact and the other
			// block contains at least one, then the loco had to move from the second block to the first one. Otherwise
			// it is an ambiguous.
			LayoutBlock[]			blocks = new LayoutBlock[2];
			int[]					nValids = new int[2];

			blocks[0] = block1;
			blocks[1] = block2;

			for(int iBlock = 0; iBlock < 2; iBlock++) {
				nValids[iBlock] = 0;

				foreach(TrainLocationInfo trainLocation in blocks[iBlock].Trains) {
					if(canLocomotiveCrossBlockEdge(trainLocation, blockEdge))
						nValids[iBlock]++;
				}
			}

			if(nValids[0] == 0 && nValids[1] > 0)
				return movingFrom(blockEdge, blocks[1], blocks[0]);
			else if(nValids[0] > 0 && nValids[1] == 0)
				return movingFrom(blockEdge, blocks[0], blocks[1]);
			else if(nValids[0] == 0 && nValids[1] == 0)
				throw new UnexpectedBlockCrossingException(blockEdge);
			else
				throw new AmbiguousBlockCrossingException(blockEdge);
		}

		/// <summary>
		/// Check if a given locomotive could have triggered a given track contact.
		/// </summary>
		/// <param name="trainState">The locomotive</param>
		/// <param name="blockEdge">The block edge</param>
		/// <returns>True: it could be that locomotive that triggerd this track contact</returns>
		private bool canLocomotiveCrossBlockEdge(TrainLocationInfo trainLocation, LayoutBlockEdgeBase blockEdge) {
			bool	result;

			TrainStateInfo	trainState = new TrainStateInfo(trainLocation.LocomotiveStateElement);

			Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "! check if train " + trainState.DisplayName + "(" + trainState.StatusText + ") can cross " + blockEdge.FullDescription);

			if(trainState.NotManaged)
				return true;			// No information, so it might be able to cross this block edge

			if(trainState.Speed == 0) {
				Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "!  No - train speed is 0");
				return false;			// loco is not moving, therefore it could not cross the block edge
			}

			LayoutBlock	block = trainLocation.Block;

			Debug.Assert(blockEdge.Track.GetBlock(blockEdge.Track.ConnectionPoints[0]) == block ||
				blockEdge.Track.GetBlock(blockEdge.Track.ConnectionPoints[1]) == block);

			if(block.IsLinear) {
				if(trainState.IsLastBlockEdgeCrossingSpeedKnown) {

					Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "!  Linear block - last block edge crossing speed is known");
					Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "!                 testing if locomotive could cross " + blockEdge.FullDescription + " the last block edge crossed is: " + (trainLocation.BlockEdge == null ? "None" : trainLocation.BlockEdge.FullDescription));
					Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "!                 Train speed " + trainState.Speed + " last crossing speed " + trainState.LastBlockEdgeCrossingSpeed);

					// There are two cases, the track contact being triggered is the same as the previous one. In this
					// case the locomotive must go in a reverse direction to the one its crossed the contact.
					// The other case is that the locomotive is crossing the other track contact, in this case it must
					// continue in the same direction as the one it had when it crossed the previous one.
					if(trainLocation.BlockEdgeId == blockEdge.Id)
						result = (trainState.Speed > 0) != (trainState.LastBlockEdgeCrossingSpeed > 0);
					else
						result = (trainState.Speed > 0) == (trainState.LastBlockEdgeCrossingSpeed > 0);
				}
				else if(trainLocation.IsDisplayFrontKnown && block.BlockDefinintion != null) {
					// Last track contact crossing speed is not know (a locomotive that was just placed on the track
					// and did not yet crossed any track contact. However, its front was specifid. Here there are two
					// cases:
					//  (a) The crossed track contact is the one that the locomotive front is pointing to. In this case
					//      the locomotive must move forward.
					//  (b) the crossed track contacted is not the one that the locomotive front is pointing to. In this
					//      case the locomotive must move backward.
					LayoutBlockDefinitionComponent	blockInfo = block.BlockDefinintion;
					int							iFront;

					if(trainLocation.DisplayFront == blockInfo.Track.ConnectionPoints[0])
						iFront = 0;
					else
						iFront = 1;

					bool		blockEdgeIsInFront = blockInfo.ContainsBlockEdge(iFront, blockEdge);

					Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "!  Linear block - last block edge crossing speed is not known - front is known");
					Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "!                 testing if locomotive could cross " + blockEdge.FullDescription);
					Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "!                 Train speed " + trainState.Speed + (blockEdgeIsInFront ? " block edge is in front" : " block edge is not in front"));

					if(blockEdgeIsInFront)
						result = trainState.Speed > 0;
					else
						result = trainState.Speed < 0;
				}
				else
					result = true;		// Nothing can be checked, so it might be the one...
			}
			else {
				LayoutBlockDefinitionComponent	blockInfo = block.BlockDefinintion;
				bool						blockEdgeInFront = blockInfo.ContainsBlockEdge(trainLocation.DisplayFront, blockEdge);

				Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "!  Non Linear block - testing if locomotive could cross " + blockEdge.FullDescription);
				Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "!                     Train speed " + trainState.Speed + (blockEdgeInFront ? " block edge is in front" : " block edge is not in front"));

				// Check if the block edge that was crossed is in the same edge group as the front. If it is, the locomotive must move backward
				// otherwise it has to move forward
				if(blockEdgeInFront)
					result = trainState.Speed > 0;
				else
					result = trainState.Speed < 0;
			}

			Trace.WriteLineIf(traceLocomotiveTracking.TraceVerbose, "!!! Train could " + (result ? "" : "not ") + "cross");

			return result;
		}

		#endregion
	}
}
