using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;
using System.Diagnostics;
using LayoutManager.Model;
using LayoutManager.Components;

#nullable enable
namespace LayoutManager.Logic {
    [LayoutModule("Disptacher")]
    internal class Dispatcher : LayoutModuleBase {
        private static readonly LayoutTraceSwitch traceDispatcher = new LayoutTraceSwitch("Dispatcher", "Master Dispatcher");
        private static readonly LayoutTraceSubsystem traceUnlockingManager = new LayoutTraceSubsystem(traceDispatcher, "DispatcherUnlockingManager", "Layout Block Unlock Manager");
        private static readonly LayoutTraceSubsystem traceDispatching = new LayoutTraceSubsystem(traceDispatcher, "Dispatching", "Displatching operation");
        private static readonly LayoutTraceSubsystem traceDeadlock = new LayoutTraceSubsystem(traceDispatcher, "DeadlockIdentification", "Deadlock identification");

        #region Definitions and Explanations

        //
        // In order to facilitate better understanding of the code by others (including myself...) some terms need to be
        // defined:
        //
        // Waitabile block - A block in which a train is allowed to wait (stop)
        // Non waitable block - A block in which the train is not allowed to stop
        //
        // By default blocks that contain turnout or crosses are non waitabile whereas linear blocks (containing 
        // only straight tracks) are waitabile. It is possible to override this default via the Block Info component
        // properties.
        //
        // Crossing block - A block in which there is a possibility of two trains crossing each other. Crossing block
        // is a block that has a merge point (a turnout in which multiple (usually two) tracks merge into one), or a cross.
        //
        // Locking region - A region that should be locked in order to allow safe trip of a train from its current position
        // towards the destination. The following method is used to figure out the next locking region:
        //
        //  a) Figure out the best route from the train current position to the destination. The exact meaning of "best route"
        //     is defined later
        //
        //  b) Include the train current block in the lock region
        //
        //  c) Follow the route and until arriving either to the destination or to a Crossing block. While tracking the route
        //     keep a note of the last traversed waitable block.
        //
        //  c.1) If arrived to the destination, then include all blocks from the train position to the destination in the 
        //       locking region.
        //
        //  c.2) If arrived to a crossing block, include all the blocks from the current train position until (and including)
        //       the last traversed waitable block.
        //
        // Thus, the train will lock all the blocks until just before there is a possibility that it will cross the pass of
        // another train (or until the destination). When the train arrives to the last block in the region (that must be
        // a waitable block), it will try to obtain lock on the next locking region. If successful, the train will just
        // continue. If not, the train will be stopped on the waitable block and wait for the lock (this usually means that
        // the train will wait until another train has crossed the crossing block). This process continues until the train
        // has reached the destination.
        //

        #endregion

        #region Traceing predicates

        #endregion

        #region Data structures

        private class ActiveTripInfo : TripPlanAssignmentInfo {
            private LayoutEventScript[]? activePolicies;

            public ActiveTripInfo(XmlElement element) : base(element) {
                Queue = new BlockEntryQueue(this);
            }

            public BlockEntryQueue Queue { get; }

            public LayoutEventScript? PendingStartCondition { get; set; }

            public LayoutEventScript? PendingDriverInstructions { get; set; }

            public LayoutLockRequest? PendingLockRequest { get; set; }

            public Guid NextSectionStartBlockId { get; set; } = Guid.Empty;

            internal TripStatus SuspendedStatus { get; set; } = TripStatus.NotSubmitted;

            public LayoutEvent? SuspendedEvent { get; set; }

            public LayoutEventScript[]? ActivePolicies {
                get {
                    return activePolicies;
                }

                set {
                    activePolicies = value;
                }
            }

            /// <summary>
            /// Cancel a trip - drop all pending requests
            /// </summary>
            internal void Cancel() {
                if (PendingStartCondition != null) {
                    PendingStartCondition.Dispose();
                    PendingStartCondition = null;
                }

                if (PendingDriverInstructions != null) {
                    PendingDriverInstructions.Dispose();
                    PendingDriverInstructions = null;
                }

                if (PendingLockRequest != null) {
                    EventManager.Event(new LayoutEvent<LayoutLockRequest>("cancel-layout-lock", PendingLockRequest));
                    PendingLockRequest = null;

                    if (traceUnlockingManager.TraceVerbose)
                        EventManager.Event(new LayoutEvent("dump-locks", this));
                }
            }
        }

        private class ActiveTripsCollection : Hashtable {
            public ActiveTripInfo this[Guid trainID] => (ActiveTripInfo)base[trainID];
        }

        [Flags]
        private enum BlockAction {
            Go = 0x0000,                    // Just continue going
            Stop = 0x0001,                  // stop the train in this block
            PrepareStop = 0x0002,           // The train will be stopped on the next block
            PrepareNextSection = 0x0004,    // Prepare next trip section
            AtWaypoint = 0x0008,            // Train is at way point
            AbortTrip = 0x0010,             // Abort trip when arriving to this block
            SuspendTrip = 0x0020,           // Suspend trip when arriving to this block
        }

        private class BlockEntry {
            private readonly bool waitable;

            public BlockEntry(LayoutBlock block, List<LayoutBlock>? crossedBlocks, LayoutComponentConnectionPoint front, bool hasMergePoint, bool waitable) {
                this.Block = block;
                this.Action = BlockAction.Go;
                this.Front = front;
                this.HasMergePoint = hasMergePoint;
                this.waitable = waitable;

                if (crossedBlocks != null && crossedBlocks.Count != 0)
                    this.CrossedBlocks = crossedBlocks.ToArray();
            }

            public BlockEntry(TrainStateInfo train) {
                this.Block = train.LocomotiveBlock!;
                this.Action = BlockAction.Go;
                this.waitable = false;
                if (train.LocomotiveLocation != null)
                    this.Front = train.LocomotiveLocation.DisplayFront;
            }

            public LayoutBlock Block { get; }

            public BlockAction Action { get; set; }

            public LayoutBlock[]? CrossedBlocks { get; }

            public LayoutComponentConnectionPoint Front { get; }

            public void AddAction(BlockAction action) {
                this.Action |= action;
            }

            public void RemoveAction(BlockAction action) {
                this.Action &= ~action;
            }

            public bool HasMergePoint { get; set; }

            public bool Waitable => waitable;
        }

        private class BlockEntryQueue : Queue {
            private readonly IDictionary blockEntriesInQueue = new HybridDictionary();
            private readonly ActiveTripInfo trip;

            internal BlockEntryQueue(ActiveTripInfo trip) {
                this.trip = trip;
            }

            public void Enqueue(BlockEntry blockEntry) {
                base.Enqueue(blockEntry);
                if (!blockEntriesInQueue.Contains(blockEntry.Block.Id))         //?? Check if this need to be removed (blockEntriesInQueue)
                    blockEntriesInQueue.Add(blockEntry.Block.Id, blockEntry);
            }

            public new BlockEntry Dequeue() {
                BlockEntry blockEntry = (BlockEntry)base.Dequeue();

                blockEntriesInQueue.Remove(blockEntry.Block.Id);
                return blockEntry;
            }

            public new BlockEntry? Peek() {
                if (base.Count == 0)
                    return null;
                else
                    return (BlockEntry)base.Peek();
            }

            public bool Contains(Guid blockID) => blockEntriesInQueue.Contains(blockID);

            public BlockEntry this[Guid blockID] => (BlockEntry)blockEntriesInQueue[blockID];

            public bool Contains(LayoutBlock block) => Contains(block.Id);

            public new void Clear() {
                blockEntriesInQueue.Clear();
                base.Clear();
            }

            public void Flush() {
                ArrayList blockIDs = new ArrayList();

                Trace.WriteLineIf(traceDispatcher.TraceVerbose, "*** Flushing queue");

                // Unlock all blocks which are still in the queue
                while (Count > 0) {
                    BlockEntry blockEntry = Dequeue();

                    if (blockEntry.Block.LockRequest != null && blockEntry.Block.LockRequest.OwnerId == trip.Train.Id) {
                        blockIDs.Add(blockEntry.Block.Id);

                        if (blockEntry.CrossedBlocks != null)
                            foreach (LayoutBlock crossedBlock in blockEntry.CrossedBlocks)
                                blockIDs.Add(crossedBlock.Id);
                    }
                }

                EventManager.Event(new LayoutEvent("free-layout-lock", (Guid[])blockIDs.ToArray(typeof(Guid))));
            }

            public void ClearStopAction() {
                foreach (BlockEntry blockEntry in blockEntriesInQueue.Values) {
                    blockEntry.RemoveAction(BlockAction.Stop);
                    blockEntry.RemoveAction(BlockAction.PrepareStop);
                }
            }
        }

        private class BlockUnlockingManager : Dictionary<Guid, LayoutDelayedEvent> {
            private bool operationMode;

            public BlockUnlockingManager() {
                EventManager.AddObjectSubscriptions(this);
            }

            public void Add(LayoutBlock block, int timeout) {
                Remove(block.Id);

                Trace.WriteLineIf(traceUnlockingManager.TraceInfo, $"UnlockingManager: Adding {block} timeout {timeout}");
                base.Add(block.Id, EventManager.DelayedEvent(timeout, new LayoutEvent("dispatcher-free-block-layout-lock", block, block.LockRequest)));
            }

            public new void Remove(Guid blockID) {
                if (TryGetValue(blockID, out LayoutDelayedEvent delayedEvent)) {
                    Trace.WriteLineIf(traceUnlockingManager.TraceInfo, $"UnlockingManager: Removing {LayoutModel.Blocks[blockID]}");
                    delayedEvent.Cancel();

                    base.Remove(blockID);
                }
            }

            public void Remove(LayoutLockRequest lockRequest) {
                foreach (LayoutLockBlockEntry blockEntry in lockRequest.Blocks)
                    Remove(blockEntry.Block.Id);
            }

            public void Dump(string header = "") {
                Trace.WriteLine($"Unlock Manager - {header} pending unlocks for:");
                foreach (var blockId in Keys)
                    Trace.WriteLine($"  {LayoutModel.Blocks[blockId]}");
            }

            public void RemoveByOwner(Guid trainID) {
                List<Guid> removeList = new List<Guid>();

                foreach (Guid blockID in Keys) {
                    LayoutBlock block = LayoutModel.Blocks[blockID];

                    Debug.Assert(block.LockRequest != null);
                    if (block.LockRequest?.OwnerId == trainID)
                        removeList.Add(block.Id);
                }

                foreach (Guid blockID in removeList)
                    Remove(blockID);
            }

            #pragma warning disable IDE0051, IDE0060

            [LayoutEvent("exit-operation-mode")]
            private void exitOperationMode(LayoutEvent e) {
                operationMode = false;

                foreach (LayoutDelayedEvent delayedEvent in Values)
                    delayedEvent.Cancel();
                Clear();
            }

            [LayoutEvent("enter-operation-mode")]
            private void enterOperationMode(LayoutEvent e) {
                operationMode = true;
            }

            [LayoutEvent("layout-lock-released")]
            private void layoutLockReleased(LayoutEvent e0) {
                var e = (LayoutEventInfoValueType<object, Guid>)e0;

                // Remove block from unlock manager since it is no longer locked
                Remove(e.Info);
            }

            [LayoutEvent("dispatcher-free-block-layout-lock")]
            private void dispatcherFreeBlockLayoutLock(LayoutEvent e) {
                if (!operationMode)
                    return;             // some kind of a timeout that happend just before exiting operation mode, ignore it

                var block = Ensure.NotNull<LayoutBlock>(e.Sender, "block");
                var lockRequest = (LayoutLockRequest?)e.Info;

                List<Guid> resourceIDsToUnlock = new List<Guid>();
                bool retryWaitingTrains = false;

                Trace.WriteLineIf(traceUnlockingManager.TraceInfo, $"UnlockingManager: Unlock on timeout {block}");

                /*Debug.Assert(block.LockRequest != null);*/        // Otherwise block was unlocked by someone else!

                // If request was not removed from the queue, and the block is still locked by this request
                // (and not by another newer one)
                if (this.ContainsKey(block.Id)) {
                    Remove(block.Id);

                    if (block.LockRequest != null && block.LockRequest == lockRequest) {
                        resourceIDsToUnlock.Add(block.Id);

                        foreach (TrackEdge edge in block.TrackEdges) {
                            if (edge.Track is LayoutDoubleTrackComponent && ((LayoutDoubleTrackComponent)edge.Track).IsCross) {
                                LayoutBlock crossedBlock = ((LayoutDoubleTrackComponent)edge.Track).GetOtherBlock(edge.ConnectionPoint);

                                retryWaitingTrains = true;

                                if (crossedBlock.LockRequest != null && crossedBlock.LockRequest.OwnerId == block.LockRequest.OwnerId) {
                                    resourceIDsToUnlock.Add(crossedBlock.Id);
                                    Remove(crossedBlock.Id);
                                }
                            }
                            else if (edge.Track is IModelComponentIsMultiPath)
                                retryWaitingTrains = true;
                        }

#if OLD
                        // Unlock any other resources that are associated with this block
                        if (block.BlockDefinintion != null) {
                            foreach (ResourceInfo resourceInfo in block.BlockDefinintion.Info.Resources)
                                resourceIDsToUnlock.Add(resourceInfo.ResourceId);
                        }
#endif
                    }
                }
                else
                    Trace.WriteLineIf(traceUnlockingManager.TraceInfo, "UnlockingManager: Block was already removed");

                EventManager.Event(new LayoutEvent("free-layout-lock", resourceIDsToUnlock.ToArray()));
                if (traceUnlockingManager.TraceVerbose)
                    EventManager.Event(new LayoutEvent("dump-locks", this));

                if (retryWaitingTrains) {
                    Trace.WriteLineIf(traceUnlockingManager.TraceInfo, $"UnlockingManager: Unlocked block with turnouts {block} retry routes for waiting trains");
                    EventManager.Event(new LayoutEvent("dispatcher-retry-waiting-trains", null, (bool)false));
                }
            }
        }

        /// <summary>
        /// Describe a trip section. A trip section is part of a trip upto the destination or a position where the train
        /// may have to wait because it may cross the path of another train
        /// </summary>
        private class TripSection {
            internal TripSection(ActiveTripInfo trip, ITripRoute route, List<BlockEntry> blockEntries, bool arrivedToDestination) {
                this.Trip = trip;
                this.Route = route;
                this.BlockEntries = blockEntries;
                this.ArrivedToDestination = arrivedToDestination;
            }

            public ActiveTripInfo Trip { get; }

            public ITripRoute Route { get; }

            public IList<BlockEntry> BlockEntries { get; }

            public bool ArrivedToDestination { get; }

            public LayoutLockRequest? RequestLayoutLock(ActiveTripInfo trip, BlockUnlockingManager blockUnlockingManager) {
                LayoutLockRequest lockRequest = new LayoutLockRequest(trip.TrainId);
                bool gotLock;

                foreach (BlockEntry entry in BlockEntries) {
                    lockRequest.Blocks.Add(entry.Block);

                    if (entry.CrossedBlocks != null)
                        foreach (LayoutBlock crossedBlock in entry.CrossedBlocks) {
                            if (!lockRequest.Blocks.ContainsKey(crossedBlock.Id))
                                lockRequest.Blocks.Add(crossedBlock, LayoutLockBlockEntry.LockBlockOptions.ForceRedSignal);
                        }

#if OLD
                    // Lock any resource associated with this block
                    if(entry.Block.BlockDefinintion != null) {
						foreach(ResourceInfo resourceInfo in entry.Block.BlockDefinintion.Info.Resources)
							lockRequest.ResourceEntries.Add(resourceInfo.GetResource(LayoutPhase.Operational));
					}
#endif
                }

                if (traceDispatching.TraceInfo) {
                    Trace.Write($"Request lock for train {trip.Train.DisplayName}:");
                    lockRequest.Dump();
                }

                gotLock = (bool)EventManager.Event(new LayoutEvent("request-layout-lock", lockRequest));

                if (!gotLock) {
                    Trace.WriteLineIf(traceDispatching.TraceInfo, " Pending - event is sent when lock is obtained");
                    // Lock could not obtained at once, train may need to stop
                    return lockRequest;
                }
                else {
                    // Lock was obtain, make sure that the delayed unlocking process will not unlock those blocks
                    blockUnlockingManager.Remove(lockRequest);
                    Trace.WriteLineIf(traceDispatching.TraceInfo, " Lock obtained");
                }

                return null;
            }
        }

        #endregion

        /// <summary>
        /// activeTrips map from train ID to active trip
        /// </summary>
        private readonly ActiveTripsCollection activeTrips = new ActiveTripsCollection();
        private readonly BlockUnlockingManager blockUnlockingManager = new BlockUnlockingManager();
        private IRoutePlanningServices? _tripPlanningServices;
        private ILayoutLockManagerServices? _layoutLockManagerServices;
        private ILayoutTopologyServices? _layoutTopologyServices;
        private bool allSuspended;

        #region External Interface Event Handlers

        [LayoutEvent("execute-trip-plan")]
        [LayoutEventDef("trip-added", Role = LayoutEventRole.Notification, SenderType = typeof(TripPlanAssignmentInfo))]
        private void executeTripPlan(LayoutEvent e) {
            var tripPlanAssignment = Ensure.NotNull<TripPlanAssignmentInfo>(e.Sender, "tripPlanAssignment");
            object? form = e.Info;
            ActiveTripInfo existingTrip = activeTrips[tripPlanAssignment.TrainId];
            ActiveTripInfo newTrip = new ActiveTripInfo(tripPlanAssignment.Element);

            if (existingTrip != null) {
                existingTrip.Cancel();          // Cancel all pending events etc.
                activeTrips.Remove(existingTrip.TrainId);
            }

            // Assign driver to the trip.
            if (!(bool)EventManager.Event(new LayoutEvent("driver-assignment", newTrip.Train, form))) {
                e.Info = false;
                return;
            }

            // Since driver assignment may take, time and in the meanwhile the train may have moved,
            // Validate the new trip (yet again) to verify that the train did
            // not move to a place where it is no longer valid
            var validationResult = Ensure.NotNull<ITripRouteValidationResult>(EventManager.Event(new LayoutEvent("validate-trip-plan-route", newTrip.TripPlan, newTrip.Train)), "validate-trip-plan-roure");

            if (validationResult.CanBeFixed) {
                foreach (ITripRouteValidationAction fixAction in validationResult.Actions)
                    fixAction.Apply(newTrip.TripPlan);
            }
            else
                throw new LayoutException("Train " + newTrip.Train.DisplayName + " cannot execute the trip plan (i.e. No route)");

            if (traceDispatching.TraceInfo)
                newTrip.Dump();

            activeTrips.Add(newTrip.TrainId, newTrip);

            // Start active policies
            LayoutEventScript[] activePolicies = new LayoutEventScript[newTrip.TripPlan.Policies.Count];
            IDictionary<Guid, LayoutPolicyInfo> policyMap = LayoutModel.StateManager.TripPlanPolicies.IdToPolicyMap;

            int i = 0;
            foreach (Guid policyID in newTrip.TripPlan.Policies) {
                LayoutPolicyInfo policy = (LayoutPolicyInfo)policyMap[policyID];

                if (policy != null) {
                    LayoutEventScript activePolicy = EventManager.EventScript("trip policy: " + policy.Name, policy.EventScriptElement,
                        new Guid[] { newTrip.TrainId, newTrip.Id }, null);

                    activePolicy.ScriptSubject = newTrip.Train;
                    EventManager.Event(new LayoutEvent("set-script-context", newTrip, activePolicy.ScriptContext));

                    activePolicies[i++] = activePolicy;
                    activePolicy.Reset();
                }
            }

            newTrip.ActivePolicies = activePolicies;

            newTrip.CurrentWaypointIndex = 0;
            if (isTrainInWaypoint(newTrip, 0))
                newTrip.CurrentWaypointIndex = getNextWaypointIndex(newTrip);

            newTrip.Train.Trip = newTrip;

            trainStopped(newTrip, true);
            EventManager.Event(new LayoutEvent("trip-added", newTrip));

            e.Info = true;
        }

        [LayoutEvent("abort-trip")]
        [LayoutEventDef("driver-stop", Role = LayoutEventRole.Notification, SenderType = typeof(TrainStateInfo))]
        [LayoutEventDef("driver-emergency-stop", Role = LayoutEventRole.Notification, SenderType = typeof(TrainStateInfo))]
        [LayoutEventDef("trip-aborted", Role = LayoutEventRole.Notification, SenderType = typeof(TripPlanAssignmentInfo))]
        private void abortTrip(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");
            bool emergency = (bool)e.Info;
            ActiveTripInfo trip = activeTrips[train.Id];

            if (trip != null) {
                if (emergency) {
                    Trace.WriteLineIf(traceDispatching.TraceInfo, $"Trip for train {train.DisplayName} emergency abort");
                    if (train.Managed) {
                        train.SpeedInSteps = 0;     // Stop the train at once
                        EventManager.Event(new LayoutEvent("driver-emergency-stop", trip.Train));
                    }

                    trip.Status = TripStatus.Aborted;
                    trip.Cancel();                      // Drop pending lock or pending waits
                    trip.Queue.Flush();
                }
                else {
                    Trace.WriteLineIf(traceDispatching.TraceInfo, $"Trip for train {train.DisplayName} aborted");
                    if ((train.Managed && train.Speed != 0) && (trip.Status == TripStatus.Go || trip.Status == TripStatus.PrepareStop))
                        causeTrainToStop(trip, BlockAction.AbortTrip);
                    else {
                        EventManager.Event(new LayoutEvent("driver-stop", trip.Train));
                        trip.Status = TripStatus.Aborted;
                        trip.Cancel();                      // Drop pending lock or pending waits
                        trip.Queue.Flush();
                    }
                }

                EventManager.Event(new LayoutEvent("trip-aborted", trip));
            }
            else
                throw new LayoutException(train, "Trip plan execution was aborted for train that was not assigned to one");
        }

        [LayoutEvent("suspend-trip")]
        private void suspendTrip(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");
            bool emergency = (bool)e.Info;
            ActiveTripInfo? trip = activeTrips[train.Id];

            if (trip != null && !canClearTrip(trip) && trip.Status != TripStatus.Suspended) {
                if (emergency) {
                    Trace.WriteLineIf(traceDispatching.TraceInfo, $"Trip for train {train.DisplayName} emergency suspend");

                    if (train.Managed) {
                        train.SpeedInSteps = 0;     // Stop the train at once
                        EventManager.Event(new LayoutEvent("driver-emergency-stop", trip.Train));
                    }

                    trip.SuspendedStatus = trip.Status;
                    trip.Status = TripStatus.Suspended;
                    trip.SuspendedEvent = new LayoutEvent("dispatcher-resume-suspended", trip);
                }
                else {
                    if ((train.Managed && train.Speed != 0) && (trip.Status == TripStatus.Go || trip.Status == TripStatus.PrepareStop))
                        causeTrainToStop(trip, BlockAction.SuspendTrip);
                    else {
                        trip.SuspendedStatus = trip.Status;
                        trip.Status = TripStatus.Suspended;
                        trip.SuspendedEvent = new LayoutEvent("dispatcher-resume-suspended", trip);
                    }
                }
            }
        }

        [LayoutEvent("resume-trip")]
        private void resumeTrip(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");
            ActiveTripInfo? trip = activeTrips[train.Id];

            if (trip != null) {
                if (trip.Status == TripStatus.Suspended) {
                    Debug.Assert(trip.SuspendedEvent != null);
                    trip.Status = trip.SuspendedStatus;
                    EventManager.Event(trip.SuspendedEvent);
                    trip.SuspendedEvent = null;
                    AllSuspended = false;
                }
                else
                    throw new LayoutException(trip.Train, "A resume operation was requested for a trip-plan for this train. It was not suspended");
            }
        }

        [LayoutEvent("clear-trip")]
        [LayoutEventDef("trip-cleared", Role = LayoutEventRole.Notification, SenderType = typeof(TripPlanAssignmentInfo))]
        private void clearTripPlan(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");
            ActiveTripInfo? trip = activeTrips[train.Id];

            if (trip != null && trip.CanBeCleared) {
                train.Trip = null;
                train.Redraw();

                activeTrips.Remove(train.Id);
                EventManager.Event(new LayoutEvent("trip-cleared", trip));
            }
        }

        [LayoutEvent("suspend-all-locomotives")]
        [LayoutEvent("suspend-all-trips")]
        private void suspendAllLocomotives(LayoutEvent e) {
            foreach (XmlElement trainElement in LayoutModel.StateManager.Trains.Element) {
                TrainStateInfo train = new TrainStateInfo(trainElement);

                ActiveTripInfo trip = activeTrips[train.Id];

                if (trip == null)
                    train.SpeedInSteps = 0;
                else
                    EventManager.Event(new LayoutEvent("suspend-trip", trip.Train, true));
            }

            AllSuspended = true;
        }

        [LayoutEvent("resume-all-locomotives")]
        [LayoutEvent("resume-all-trips")]
        private void resumeAllLocomotives(LayoutEvent e) {
            foreach (ActiveTripInfo trip in activeTrips.Values) {
                if (trip.Status == TripStatus.Suspended)
                    EventManager.Event(new LayoutEvent("resume-trip", trip.Train));
            }
        }

        [LayoutEvent("is-train-in-active-trip")]
        private void isTrainInActiveTrip(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");
            ActiveTripInfo? trip = activeTrips[train.Id];

            if (trip == null || trip.Status == TripStatus.Aborted || trip.Status == TripStatus.Done)
                e.Info = false;
            else {
                if (trip.Status == TripStatus.Done)
                    e.Info = false;
                else
                    e.Info = true;
            }
        }

        [LayoutEvent("get-train-active-trip")]
        private void getTrainActiveTrip(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");
            ActiveTripInfo? trip = activeTrips[train.Id];

            if (trip == null || trip.Status == TripStatus.Aborted || trip.Status == TripStatus.Done)
                e.Info = null;
            else
                e.Info = trip;
        }

        [LayoutEvent("get-active-trips")]
        private void getActiveTrips(LayoutEvent e) {
            ArrayList trips = new ArrayList();

            foreach (ActiveTripInfo trip in activeTrips.Values)
                if (trip.Status != TripStatus.Aborted && trip.Status != TripStatus.Done)
                    trips.Add(trip);

            e.Info = trips.ToArray(typeof(ActiveTripInfo));
        }

        [LayoutEvent("are-all-locomotives-suspended")]
        private void areAllLocomotivesSuspended(LayoutEvent e) {
            e.Info = AllSuspended;
        }

        [LayoutEvent("any-active-trip-plan")]
        private void AnyActiveTripPlan(LayoutEvent e) {
            e.Info = activeTrips.Count > 0;
        }

        #endregion

        #region Services getter properties

        private IRoutePlanningServices TripPlanningServices {
            get {
                if (_tripPlanningServices == null) {
                    _tripPlanningServices = (IRoutePlanningServices)EventManager.Event(new LayoutEvent("get-route-planning-services", this))!;
                    Debug.Assert(_tripPlanningServices != null);
                }

                return _tripPlanningServices;
            }
        }

        private ILayoutLockManagerServices LayoutLockManagerServices {
            get {
                return _layoutLockManagerServices ?? (_layoutLockManagerServices = (ILayoutLockManagerServices)EventManager.Event(new LayoutEvent("get-layout-lock-manager-services", this))!);
            }
        }

        private ILayoutTopologyServices LayoutTopologyServices {
            get {
                return _layoutTopologyServices ?? (_layoutTopologyServices = (ILayoutTopologyServices)EventManager.Event(new LayoutEvent("get-topology-services", this))!);
            }
        }

        private bool AllSuspended {
            get {
                return allSuspended;
            }

            set {
                if (value != allSuspended) {
                    allSuspended = value;
                    EventManager.Event(new LayoutEvent("all-locomotives-suspended-status-changed", this, allSuspended));
                }
            }
        }

        #endregion

        #region Utility methods

        #region Finding Best Route

        [LayoutEvent("dispatcher-get-block-unlocking-manager")]
        private void dispatcherGetBlockUnlockingManager(LayoutEvent e) {
            e.Info = blockUnlockingManager;
        }

        /// <summary>
        /// Return the best route from a given position (and orientation) to the current destination.
        /// </summary>
        private TripBestRouteResult findBestRoute(Guid routeOwner, TripPlanDestinationInfo destination, ModelComponent sourceComponent, LayoutComponentConnectionPoint front, LocomotiveOrientation direction, bool trainStopping) {
            BestRoute bestRoute = TripPlanningServices.FindBestRoute(sourceComponent, front, direction, destination, routeOwner, trainStopping);

            return new TripBestRouteResult(bestRoute.DestinationTarget == null ? null : bestRoute, bestRoute.Quality, false);
        }

        private TripBestRouteResult findBestRoute(ActiveTripInfo trip, ModelComponent sourceComponent, LayoutComponentConnectionPoint front) => findBestRoute(trip.TrainId, trip.CurrentWaypoint.Destination, sourceComponent, front, trip.CurrentWaypoint.Direction, trip.CurrentWaypoint.TrainStopping);

        [LayoutEvent("find-best-route-request")]
        private void findBestRouteRequest(LayoutEvent e) {
            TripBestRouteRequest r = Ensure.NotNull<TripBestRouteRequest>(e.Sender, "r");

            e.Info = findBestRoute(r.RouteOwner, r.Destination, r.Source, r.Front, r.Direction, r.TrainStopping);
        }

        #endregion

        #region Get Trip Section

        private static void causeTrainToStop(ActiveTripInfo trip, BlockAction actionWhenStopped) {
            var blockEntries = new List<BlockEntry>();

            while (trip.Queue.Count > 0) {
                BlockEntry blockEntry = trip.Queue.Dequeue();

                Debug.Assert(blockEntry.Block.LockRequest != null && blockEntry.Block.LockRequest.OwnerId == trip.Train.Id);
                blockEntries.Add(blockEntry);

                if (blockEntry.Waitable)
                    break;
            }

            trip.Queue.Flush();         // Flush whatever is left in the queue

            int i = blockEntries.Count - 1;

            if (i > 0)
                ((BlockEntry)blockEntries[i - 1]).Action = BlockAction.PrepareStop;

            ((BlockEntry)blockEntries[i]).Action = (BlockAction.Stop | actionWhenStopped);

            foreach (BlockEntry blockEntry in blockEntries)
                trip.Queue.Enqueue(blockEntry);
        }

        private static bool isManualDispatchBlock(LayoutBlock block) {
            if (block.LockRequest != null && block.LockRequest.IsManualDispatchLock)
                return true;
            return false;
        }

        private static bool hasTrain(LayoutBlock block, TrainStateInfo train) {
            foreach (TrainLocationInfo trainLocation in block.Trains)
                if (trainLocation.Train.Id == train.Id)
                    return true;
            return false;
        }

        private static bool canClearTrip(ActiveTripInfo trip) => trip.Status == TripStatus.Done || trip.Status == TripStatus.Aborted;

        /// <summary>
        /// Return the next trip section. For explaination regarding how the trip section is figured out
        /// see the comment on the top of the file.
        /// </summary>
        /// <param name="trip">The trip for which the lock request is needed</param>
        /// <param name="route">The route which the train is about to make</param>
        private TripSection getTripSection(ActiveTripInfo trip, ITripRoute route, Guid startSectionBlockID) {
            List<BlockEntry> blockEntries = new List<BlockEntry>();
            List<LayoutBlock> crossedBlocks = new List<LayoutBlock>();  // Blocks that are crossed by the current route
            TrackEdge edge = route.SourceEdge;
            TrackEdge destinationEdge = route.DestinationEdge;
            IList<int> switchStates = route.SwitchStates;
            int switchStateIndex = 0;
            bool waitableBlockFound = false;
            LayoutBlock? currentBlock = null;
            bool currentBlockHasMergePoint = false;
            bool crossing = false;      // True if the route may cross the path of another train
            bool foundTripSectionBoundry = false;
            bool isNanualDispatchRegion = false;
            bool endOfTripSection = false;
            bool reachedDestination = false;
            bool sectionStarted = false;
            bool blockEdgeIsDetectable = true;  // The block edge that leads to the current block can be detected (i.e. connected track contact)
            LayoutComponentConnectionPoint blockInfoFront = route.SourceFront;
            IList<LayoutBlock> routeBlocks = route.Blocks;

            bool canStartSection = false;
            Guid canStartSectionID = startSectionBlockID;

            Trace.WriteLineIf(traceDispatching.TraceVerbose, "- Getting start of section, checking blocks along the route");
            TrackEdge trainEdge = route.SourceEdge;

            while (true) {
                var blockDefinition = trainEdge.Track.BlockDefinitionComponent;

                if (blockDefinition != null) {
                    if (canStartSectionID == Guid.Empty || blockDefinition.Block.Id == canStartSectionID)
                        canStartSection = true;

                    bool blockHasTrain = false;

                    foreach (TrainLocationInfo trainLocation in blockDefinition.Block.Trains)
                        if (trainLocation.Train.Id == trip.Train.Id &&
                            ((route.Direction == LocomotiveOrientation.Forward && trainLocation.DisplayFront == trainEdge.OtherConnectionPoint) ||
                            (route.Direction == LocomotiveOrientation.Backward && trainLocation.DisplayFront == trainEdge.ConnectionPoint))) {
                            blockHasTrain = true;
                            break;
                        }

                    if (blockHasTrain)
                        startSectionBlockID = blockDefinition.Block.Id;

                    if (canStartSection && !blockHasTrain) {
                        Trace.WriteLineIf(traceDispatching.TraceInfo, $"-- Section will start in: {LayoutModel.Blocks[startSectionBlockID]}");
                        break;
                    }
                }

                trainEdge = LayoutTopologyServices.FindTrackConnectingAt(new TrackEdge(trainEdge.Track, trainEdge.Track.ConnectTo(trainEdge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0]));
            }

#if OLD
			foreach(LayoutBlock block in routeBlocks) {
				currentBlock = block;

				if(canStartSectionID == Guid.Empty || block.Id == canStartSectionID)
					canStartSection = true;

				Trace.WriteLineIf(traceDispatching.TraceVerbose, $"- Checking block: {block} canStartSection is {canStartSection}");

				bool	blockHasTrain = hasTrain(currentBlock, trip.Train);

				if(blockHasTrain)
					startSectionBlockID = block.Id;

				if(canStartSection && !blockHasTrain) {
					Trace.WriteLineIf(traceDispatching.TraceInfo, $"-- Section will start in: {Model.Blocks[startSectionBlockID]}");
					break;
				}
			}
#endif

            currentBlock = edge.Track.GetBlock(edge.ConnectionPoint);
            isNanualDispatchRegion = isManualDispatchBlock(currentBlock);

            while (!endOfTripSection && !reachedDestination && !isNanualDispatchRegion) {
                LayoutBlock newBlock = edge.Track.GetBlock(edge.ConnectionPoint);
                bool newBlockEdgeIsDetectable = false;

                var blockEdge = edge.Track.BlockEdgeBase;

                if (blockEdge != null) {
                    newBlock = edge.Track.GetBlock(edge.Track.ConnectTo(edge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0]);
                    newBlockEdgeIsDetectable = blockEdge.IsTrackContact();
                }

                // Check if a new block starts
                if (newBlock != currentBlock || edge.Track == destinationEdge.Track) {
                    bool waitableBlock = false;

                    // If stopped because reached to destination, ensure that the block info front is calculated
                    if (newBlock.Id == currentBlock.Id && edge.Track.BlockDefinitionComponent != null) {
                        if (route.Direction == LocomotiveOrientation.Backward)
                            blockInfoFront = edge.ConnectionPoint;
                        else
                            blockInfoFront = edge.OtherSide.ConnectionPoint;
                    }

                    if (sectionStarted) {
                        // A train can wait if the edge leading to the block is detectable (e.g. track contact) or if the block is an active occupancy detection block
                        if (blockEdgeIsDetectable || (currentBlock.BlockDefinintion.Info.IsOccupancyDetectionBlock && currentBlock.BlockDefinintion.FullyConnected)) {
                            if ((currentBlock.BlockDefinintion != null && currentBlock.BlockDefinintion.Info.CanTrainWait) || currentBlock.CanTrainWaitDefault)
                                waitableBlock = true;
                        }
                    }

                    if (waitableBlock)
                        waitableBlockFound = true;

                    if ((crossing || foundTripSectionBoundry) && waitableBlockFound)
                        endOfTripSection = true;
                    else {
                        if (sectionStarted)
                            blockEntries.Add(new BlockEntry(currentBlock, crossedBlocks, blockInfoFront, currentBlockHasMergePoint, waitableBlock));

                        if (edge.Track == destinationEdge.Track)    //**
                            reachedDestination = true;
                    }

                    if (currentBlock.Id == startSectionBlockID || startSectionBlockID == Guid.Empty)
                        sectionStarted = true;

                    crossing = false;
                    currentBlock = newBlock;
                    blockEdgeIsDetectable = newBlockEdgeIsDetectable;
                    currentBlockHasMergePoint = false;
                    crossedBlocks.Clear();

                    isNanualDispatchRegion = isManualDispatchBlock(currentBlock);
                }

                // Check if the route may cross the path of another train.
                var turnout = edge.Track as IModelComponentIsMultiPath;

                if (turnout != null && turnout.IsMergePoint(edge.ConnectionPoint)) {
                    crossing = true;
                    currentBlockHasMergePoint = true;
                }
                else {
                    if (edge.Track is LayoutDoubleTrackComponent doubleTrack && doubleTrack.IsCross) {
                        crossing = true;
                        crossedBlocks.Add(((LayoutDoubleTrackComponent)edge.Track).GetOtherBlock(edge.ConnectionPoint));
                    }
                }

                LayoutComponentConnectionPoint front;

                if (turnout != null)
                    front = turnout.ConnectTo(edge.ConnectionPoint, switchStates[switchStateIndex++]);
                else
                    front = edge.Track.ConnectTo(edge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0];

                var blockDefinition = edge.Track.BlockDefinitionComponent;

                if (blockDefinition != null) {
                    if (route.Direction == LocomotiveOrientation.Backward)
                        blockInfoFront = edge.Track.ConnectTo(front, LayoutComponentConnectionType.Passage)[0];
                    else
                        blockInfoFront = front;

                    if (blockEntries.Count > 0) {
                        // Check if the block definition defines trip section boundry for this train
                        LayoutBlockDefinitionComponentInfo info = blockDefinition.Info;

                        if ((info.IsTripSectionBoundry(0) && edge.Track.ConnectionPoints[0] == edge.ConnectionPoint) ||
                            (info.IsTripSectionBoundry(1) && edge.Track.ConnectionPoints[1] == edge.ConnectionPoint))
                            foundTripSectionBoundry = true;
                    }
                }

                edge = LayoutTopologyServices.FindTrackConnectingAt(new TrackEdge(edge.Track, front));
            }

            // Check if locking this section will not create a deadlock situation. A deadlock situation
            // is the condition of the route until the destination containing a train, and this train 
            // cannot "get out" of the way (it has no split point in the way).
            //
            // In this case, trim the section until the deadlock situation is avoided
            if (!reachedDestination) {
                bool done = false;
                bool deadlock = true;

                currentBlock = null;

                Trace.WriteLineIf(traceDeadlock.TraceInfo, "Check for deadlock situation for train " + trip.Train.DisplayName);

                // Continue to follow the route until either the destination is reached, a merge point
                // is found (which mean that a train in front has an option to get out of the way)
                // or until a block with train is found. If a train is found, then eleminate blocks
                // until either the section is empty, or a merge point if found.
                while (!done) {
                    LayoutBlock newBlock = edge.Track.GetBlock(edge.ConnectionPoint);

                    if (edge.Track.BlockEdgeBase != null)
                        newBlock = edge.Track.GetBlock(edge.Track.ConnectTo(edge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0]);

                    if (newBlock != currentBlock) {
                        Trace.WriteLineIf(traceDeadlock.TraceInfo, $"--Block {newBlock}");

                        if (newBlock.HasTrains && newBlock.Trains[0].Train.Id != trip.TrainId) {
                            Trace.WriteLineIf(traceDeadlock.TraceInfo, $"-Found another train {newBlock.Trains[0].Train.DisplayName} in block {newBlock}");
                            done = true;
                        }

                        currentBlock = newBlock;
                    }

                    if (edge.Track == destinationEdge.Track) {
                        Trace.WriteLineIf(traceDeadlock.TraceInfo, "-Reached destination - no deadlock");
                        done = true;
                        deadlock = false;           // reached destinaion
                    }

                    LayoutComponentConnectionPoint front;

                    if (edge.Track is IModelComponentIsMultiPath turnout) {
                        if (turnout.IsMergePoint(edge.ConnectionPoint)) {
                            Trace.WriteLineIf(traceDeadlock.TraceInfo, $"-Found merge point at {edge.Track.FullDescription} not a deadlock");
                            done = true;
                            deadlock = false;
                        }

                        front = turnout.ConnectTo(edge.ConnectionPoint, switchStates[switchStateIndex++]);
                    }
                    else
                        front = edge.Track.ConnectTo(edge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0];

                    edge = LayoutTopologyServices.FindTrackConnectingAt(new TrackEdge(edge.Track, front));
                }

                if (deadlock) {
                    Trace.WriteLineIf(traceDeadlock.TraceInfo, $"Deadlock condition found for train {trip.Train.DisplayName} trimming trip section to avoid deadlock");

                    for (int i = blockEntries.Count - 1; i >= 0; i--) {
                        BlockEntry blockEntry = (BlockEntry)blockEntries[i];

                        blockEntries.RemoveAt(i);
                        if (blockEntry.HasMergePoint)
                            break;          // a block with merge point is no longer in the section, the other train will have a way out...
                    }
                }
            }

            // If waitable block was found, trim blocks from the end until the last one is waitable
            for (int i = blockEntries.Count - 1; i >= 0; i--) {
                if (!blockEntries[i].Waitable)
                    blockEntries.RemoveAt(i);
                else
                    break;
            }

            if (traceDispatching.TraceVerbose) {
                Trace.WriteLine("Next trip section contains blocks: ");
                foreach (BlockEntry blockEntry in blockEntries)
                    Trace.Write($" {blockEntry.Block} (front {blockEntry.Front.ToString()})");
                Trace.WriteLine("");
            }

            return new TripSection(trip, route, blockEntries, reachedDestination);
        }

        private static int getNextWaypointIndex(ActiveTripInfo trip) {
            int nextWayPointIndex = trip.CurrentWaypointIndex;

            if (++nextWayPointIndex == trip.TripPlan.Waypoints.Count) {
                if (trip.TripPlan.IsCircular)
                    nextWayPointIndex = 0;
                else
                    nextWayPointIndex = -1;
            }

            return nextWayPointIndex;
        }

        private static TripPlanWaypointInfo? getNextWaypoint(ActiveTripInfo trip) {
            int n = getNextWaypointIndex(trip);

            if (n < 0)
                return null;
            else
                return trip.TripPlan.Waypoints[n];
        }

        private static bool isTrainInWaypoint(ActiveTripInfo trip, int wayPointIndex) {
            if (wayPointIndex >= trip.TripPlan.Waypoints.Count)
                return false;

            TripPlanDestinationInfo destination = trip.TripPlan.Waypoints[wayPointIndex].Destination;

            foreach (Guid blockID in destination.BlockIdList)
                foreach (TrainLocationInfo trainLocation in trip.Train.Locations)
                    if (blockID == trainLocation.BlockId)
                        return true;

            return false;
        }

        /// <summary>
        /// Prepare the next trip section
        /// </summary>
        /// <param name="trip">The trip</param>
        /// <param name="currentBlockEntry">The block entry that was flagged with BlockAction.PrepareNextSection</param>
        /// <param name="nextBlockEntry">(optional)The next block entry</param>
        /// <returns>True if the train can move, false if waiting for lock</returns>
        private bool prepareNextTripSection(ActiveTripInfo trip, BlockEntry currentBlockEntry, BlockEntry? nextBlockEntry) {
            bool trainCanMove = false;
            TripBestRouteResult tripBestRouteResult;
            ;
            Trace.WriteLineIf(traceDispatching.TraceInfo, $"Prepare Next Trip Section for train {trip.Train.DisplayName} Current way point is {trip.CurrentWaypointIndex} direction is {trip.CurrentWaypoint.Direction} current block is {currentBlockEntry.Block}");

            Debug.Assert(currentBlockEntry.Block.BlockDefinintion != null);

            if (nextBlockEntry != null)
                tripBestRouteResult = findBestRoute(trip, nextBlockEntry.Block.BlockDefinintion, nextBlockEntry.Front);
            else
                tripBestRouteResult = findBestRoute(trip, currentBlockEntry.Block.BlockDefinintion, currentBlockEntry.Front);

            if (tripBestRouteResult.BestRoute != null && tripBestRouteResult.Quality.IsValidRoute) {
                bool stopAtEnd = false;
                TripSection tripSection = getTripSection(trip, tripBestRouteResult.BestRoute, trip.NextSectionStartBlockId);

                // Check if the train should stop at the end of this section
                if (tripSection.ArrivedToDestination) {
                    // Check what is the next way point
                    var nextWayPoint = getNextWaypoint(trip);

                    if (nextWayPoint == null || nextWayPoint.StartCondition != null || trip.CurrentWaypoint.Direction != nextWayPoint.Direction) {
                        // Next way point has a start condition, or train will move in opposite direction, train need to stop 
                        stopAtEnd = true;
                    }
                }

                if (stopAtEnd) {
                    // Place PrepareStop action on the block before the block in which the train need to stop
                    // If the preceeding block is designated as SlowDownRegion, the prepareStop action is moved
                    // to it. This allow the user to control where the train will slow down by designating
                    // the flagging blocks as slow down region.
                    if (tripSection.BlockEntries.Count >= 2) {
                        tripSection.BlockEntries[tripSection.BlockEntries.Count - 2].AddAction(BlockAction.PrepareStop);

                        for (int i = tripSection.BlockEntries.Count - 3; i >= 0; i--) {
                            LayoutBlockDefinitionComponent blockInfo = tripSection.BlockEntries[i].Block.BlockDefinintion;

                            if (blockInfo != null && blockInfo.Info.IsSlowdownRegion) {
                                tripSection.BlockEntries[i + 1].RemoveAction(BlockAction.PrepareStop);
                                tripSection.BlockEntries[i].AddAction(BlockAction.PrepareStop);
                            }
                            else
                                break;
                        }
                    }

                    int lastIndex = tripSection.BlockEntries.Count - 1;

                    tripSection.BlockEntries[lastIndex].AddAction(BlockAction.Stop);

                    if (tripSection.ArrivedToDestination)
                        tripSection.BlockEntries[lastIndex].AddAction(BlockAction.AtWaypoint);
                }
                else {
                    int index;

                    if (tripSection.BlockEntries.Count > 2) {
                        index = tripSection.BlockEntries.Count - 2;
                        if (tripSection.BlockEntries[index].Block.BlockDefinintion == null)//?? || tripSection.BlockEntries[index].Block.BlockInfo.Info.NoRecalc)
                            index++;        // cannot calculate route from block with no block definition
                    }
                    else
                        index = tripSection.BlockEntries.Count - 1;

                    if (index >= 0) {
                        tripSection.BlockEntries[index].AddAction(BlockAction.PrepareNextSection);
                        if (tripSection.ArrivedToDestination)
                            tripSection.BlockEntries[index].AddAction(BlockAction.AtWaypoint);
                    }
                }

                trip.NextSectionStartBlockId = Guid.Empty;

                foreach (BlockEntry entry in tripSection.BlockEntries) {
                    Trace.WriteLineIf(traceDispatching.TraceVerbose, $"  Add block {entry.Block} action {entry.Action} to queue of train {trip.Train.DisplayName}");
                    trip.Queue.Enqueue(entry);
                    trip.NextSectionStartBlockId = entry.Block.Id;
                }

                trip.PendingLockRequest = tripSection.RequestLayoutLock(trip, blockUnlockingManager);
                if (traceUnlockingManager.TraceVerbose)
                    EventManager.Event(new LayoutEvent("dump-locks", this));

                if (trip.PendingLockRequest != null) {
                    trip.PendingLockRequest.LockedEvent = new LayoutEvent("dispatcher-layout-lock-obtained", trip, tripBestRouteResult.BestRoute);

                    if (nextBlockEntry != null) {
                        currentBlockEntry.AddAction(BlockAction.PrepareStop);
                        nextBlockEntry.AddAction(BlockAction.Stop);
                    }
                    else
                        currentBlockEntry.AddAction(BlockAction.Stop);      // Stop at once

                    trainCanMove = false;
                }
                else {
                    setSwitchesForTripSection(trip, tripBestRouteResult.BestRoute);
                    trainCanMove = true;
                }
            }
            else {
                if (tripBestRouteResult.BestRoute == null) {
                    Error(trip.Train, "No route can be found to " + trip.CurrentWaypoint.Destination.Name + " - trip aborted");
                    EventManager.Event(new LayoutEvent("abort-trip", trip.Train, true));
                }
                else {
                    Warning(trip.Train, "All possible routes are blocked, possible by non-movable trains. Try to fix this");
                    trainCanMove = false;
                }
            }

            if (trip.Queue.Count == 0)
                trainCanMove = false;       // Queue is empty - train cannot move

            return trainCanMove;
        }

        #endregion

        private void setSwitchesForTripSection(ActiveTripInfo trip, ITripRoute route) {
            if (trip.Queue.Count > 0) {
                TrackEdge edge = route.SourceEdge;
                BlockEntry[] queue = new BlockEntry[trip.Queue.Count];
                TrackEdge destinationEdge = route.DestinationEdge;
                var switchStates = route.SwitchStates;
                //				Dictionary<Guid, object> alreadySwitched = new Dictionary<Guid, object>(switchStates.Count);
                int queueIndex = 0;
                int switchStateIndex = 0;
                var switchingCommands = new List<SwitchingCommand>();
                var powerConnectors = new Dictionary<Guid, LayoutTrackPowerConnectorComponent>();

                trip.Queue.CopyTo(queue, 0);

                LayoutBlock firstBlockInQueue = queue[0].Block;
                bool processingQueue = false;

                while (edge.Track != destinationEdge.Track) {
                    LayoutBlock block = edge.Track.GetBlock(edge.ConnectionPoint);

                    // Power connector are lock resources. Blocks that reside in a switchable power region should specify the power connector as a locked resource
                    // When the block will be locked power will be connected (and when the block will be unlocked, power will be disconnected)
                    //
                    // However this code ensure that power is connected to any block that is in the dispatch region to ensure that the train is not sent to tracks without power...
                    //
                    var powerConnector = block?.BlockDefinintion?.PowerConnector;

                    // Make sure that tracks are connected to digital power
                    if (powerConnector != null && !powerConnectors.ContainsKey(powerConnector.Id)) {
                        powerConnectors.Add(powerConnector.Id, powerConnector);
                        powerConnector.Inlet.ConnectedOutlet.SelectPower(LayoutPowerType.Digital, switchingCommands);
                    }

                    if (block == firstBlockInQueue)
                        processingQueue = true;

                    if (processingQueue && block.Id != queue[queueIndex].Block.Id) {
                        queueIndex++;

                        if (queueIndex >= queue.Length)
                            break;

                        Debug.Assert(block.Id == queue[queueIndex].Block.Id);
                        Debug.Assert(block.LockRequest != null && block.LockRequest.OwnerId == trip.TrainId);
                    }

                    if (edge.Track is IModelComponentIsMultiPath multipath) {
                        // Change 13-Feb: Change switch state only on Split point. Do not change switch on merge point
                        // this avoid possible derailments on reverse loop
                        //if(multipath.IsSplitPoint(edge.ConnectionPoint) || !alreadySwitched.ContainsKey(multipath.Id)) {
                        if (multipath.IsSplitPoint(edge.ConnectionPoint)) {
                            multipath.AddSwitchingCommands(switchingCommands, switchStates[switchStateIndex]);
                            //							multipath.CurrentSwitchState = switchStates[switchStateIndex];
                            //							alreadySwitched.Add(multipath.Id, null);
                        }

                        edge = LayoutTopologyServices.FindTrackConnectingAt(new TrackEdge(edge.Track, multipath.ConnectTo(edge.ConnectionPoint, switchStates[switchStateIndex++])));
                    }
                    else
                        edge = LayoutTopologyServices.FindTrackConnectingAt(new TrackEdge(edge.Track, edge.Track.ConnectTo(edge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0]));
                }

                if (switchingCommands.Count > 0)
                    EventManager.AsyncEvent(new LayoutEvent("set-track-components-state", this, switchingCommands));
            }
        }

        [LayoutEvent("dispatcher-set-switches")]
        private void dispatcherSetSwitches(LayoutEvent e) {
            var ownerID = (Guid)e.Sender;
            var route = Ensure.NotNull<ITripRoute>(e.Info, "route");
            TrackEdge edge = route.SourceEdge;
            TrackEdge destinationEdge = new TrackEdge(route.DestinationTrack, route.DestinationFront);
            IList<int> switchStates = route.SwitchStates;
            //			Dictionary<Guid, object> alreadySwitched = new Dictionary<Guid, object>(switchStates.Count);
            int switchStateIndex = 0;
            bool completed = true;
            List<SwitchingCommand> switchingCommands = new List<SwitchingCommand>();
            var powerConnectors = new Dictionary<Guid, LayoutTrackPowerConnectorComponent>();

            while (edge.Track != destinationEdge.Track) {
                LayoutBlock block = edge.Track.GetBlock(edge.ConnectionPoint);

                if (block.LockRequest == null || block.LockRequest.OwnerId != ownerID) {
                    completed = false;
                    break;
                }

                var powerConnector = block?.BlockDefinintion?.PowerConnector;

                // Make sure that tracks are connected to digital power
                if (powerConnector != null && !powerConnectors.ContainsKey(powerConnector.Id)) {
                    powerConnectors.Add(powerConnector.Id, powerConnector);
                    powerConnector.Inlet.ConnectedOutlet.SelectPower(LayoutPowerType.Digital, switchingCommands);
                }

                if (edge.Track is IModelComponentIsMultiPath multipath) {
                    // 13-Feb: Change, change switch only if Split point, this avoid derailment because of changing
                    // switch on reverse loop
                    //if(multipath.IsSplitPoint(edge.ConnectionPoint) || !alreadySwitched.ContainsKey(multipath.Id)) {
                    if (multipath.IsSplitPoint(edge.ConnectionPoint)) {
                        multipath.AddSwitchingCommands(switchingCommands, switchStates[switchStateIndex]);
                        //multipath.CurrentSwitchState = switchStates[switchStateIndex];
                        //alreadySwitched.Add(multipath.Id, null);
                    }

                    edge = LayoutTopologyServices.FindTrackConnectingAt(new TrackEdge(edge.Track, multipath.ConnectTo(edge.ConnectionPoint, switchStates[switchStateIndex++])));
                }
                else
                    edge = LayoutTopologyServices.FindTrackConnectingAt(new TrackEdge(edge.Track, edge.Track.ConnectTo(edge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0]));
            }

            if (switchingCommands.Count > 0)
                EventManager.AsyncEvent(new LayoutEvent("set-track-components-state", this, switchingCommands));

            e.Info = completed;
        }

        /// <summary>
        /// Set the switch state of turnouts and other multi-path components along the trip. This is done as long as
        /// the turnout is in a block locked by the given train
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="route"></param>
        private void setSwitchStateOnRoute(ActiveTripInfo trip, ITripRoute route) {
            EventManager.Event(new LayoutEvent("dispatcher-set-switches", trip.TrainId, route));
        }

        private void retryTrip(ActiveTripInfo trip) {
            if (trip.Status != TripStatus.Done && trip.Status != TripStatus.Aborted) {
                Trace.WriteLineIf(traceDispatching.TraceInfo, $"-- retry to find route for train {trip.Train.DisplayName}");

                if (trip.PendingLockRequest != null) {
                    if (traceDispatching.TraceVerbose) {
                        Trace.Write("Cancel lock request for");
                        trip.PendingLockRequest.Dump();
                    }

                    EventManager.Event(new LayoutEvent<LayoutLockRequest>("cancel-layout-lock", trip.PendingLockRequest));

                    if (traceUnlockingManager.TraceVerbose)
                        EventManager.Event(new LayoutEvent("dump-locks", this));
                }

                trip.PendingLockRequest = null;

                var blockIDtoUnlock = new List<Guid>();

                bool unlockingBlocks = false;

                while (trip.Queue.Count > 0) {
                    BlockEntry blockEntry = (BlockEntry)trip.Queue.Dequeue();

                    if (blockEntry.Block.LockRequest != null && blockEntry.Block.LockRequest.OwnerId == trip.Train.Id) {
                        if (!blockEntry.Block.IsTrainInBlock(trip.Train.Id)) {
                            if (!unlockingBlocks) {
                                Trace.WriteLineIf(traceDispatching.TraceVerbose, "/ Unlocking blocks for trying trip for train " + trip.Train.DisplayName);
                                unlockingBlocks = true;
                            }

                            Trace.WriteLineIf(traceDispatching.TraceVerbose, $"| {blockEntry.Block}");
                            blockUnlockingManager.Remove(blockEntry.Block.Id);
                            blockIDtoUnlock.Add(blockEntry.Block.Id);
                        }
                    }
                }

                if (unlockingBlocks)
                    Trace.WriteLineIf(traceDispatching.TraceVerbose, "\\");

                trip.NextSectionStartBlockId = Guid.Empty;

                if (blockIDtoUnlock.Count > 0) {
                    EventManager.Event(new LayoutEvent("free-layout-lock", blockIDtoUnlock.ToArray()));
                    if (traceUnlockingManager.TraceVerbose)
                        EventManager.Event(new LayoutEvent("dump-locks", this));
                }

                if (prepareNextTripSection(trip, new BlockEntry(trip.Train), null) && trip.Queue.Count > 0)
                    trainStart(trip);           // If train can move right away, start
                else
                    trainStopped(trip, false);
            }
        }

        /// <summary>
        /// Check if it is worthwhile to retry finding a trip for a train that is waiting for lock
        /// </summary>
        /// <param name="trip">The trip</param>
        /// <returns>True - the trip should be retried</returns>
        private bool shouldRetryTrip(ActiveTripInfo trip) {
            var locoBlock = trip.Train.LocomotiveBlock;
            var locoLocation = trip.Train.LocomotiveLocation;

            Debug.Assert(locoBlock != null && locoLocation != null);
            TripBestRouteResult tripBestRouteResult = findBestRoute(trip, locoBlock.BlockDefinintion, locoLocation.DisplayFront);

            Trace.WriteLineIf(traceDispatching.TraceVerbose, $" Check if should retry planning for train {trip.Train.DisplayName}");

            if (!tripBestRouteResult.Quality.IsValidRoute || tripBestRouteResult.BestRoute == null) {
                Trace.WriteLineIf(traceDispatching.TraceVerbose, "  No - could not find suitable route");
                return false;
            }

            IList<LayoutBlock> bestRouteBlocks = tripBestRouteResult.BestRoute.Blocks;
            Guid startSectionBlockID = Guid.Empty;

            if (bestRouteBlocks.Count >= 2)
                startSectionBlockID = bestRouteBlocks[1].Id;

            TripSection tripSection = getTripSection(trip, tripBestRouteResult.BestRoute, startSectionBlockID);

            if (tripSection.BlockEntries.Count == 0) {
                Trace.WriteLineIf(traceDispatching.TraceVerbose, "  No - next section is empty");
                return false;
            }

            // Check that it would possible to lock the next section of the alternative route, if not, it is worth while to keep waiting for the current route
            foreach (BlockEntry blockEntry in tripSection.BlockEntries) {
                LayoutBlock block = blockEntry.Block;

                if ((block.LockRequest != null && block.LockRequest.OwnerId != trip.Train.Id) ||
                    (block.LockRequest == null && LayoutLockManagerServices.HasPendingLocks(block.Id, trip.TrainId))) {
                    Trace.WriteLineIf(traceDispatching.TraceVerbose, "  No - would not be to lock the route");
                    return false;
                }
            }

            // An alternate route was found, it would probably possible to lock this route
            Trace.WriteLineIf(traceDispatching.TraceVerbose, "  Yes - an alternative route is found!");
            return true;
        }

        private void retryTripsWaitingForLocks(bool retryAllTrains) {
            Trace.WriteLineIf(traceDispatching.TraceInfo, $"-Retry trips for {(retryAllTrains ? "all trains" : "waiting trains")}");

            foreach (ActiveTripInfo trip in activeTrips.Values) {
                if ((retryAllTrains ||
                    ((trip.PendingLockRequest != null || trip.Status == TripStatus.WaitLock) && shouldRetryTrip(trip))) && trip.Status != TripStatus.Done)
                    retryTrip(trip);
            }
        }

        [LayoutEventDef("trip-done", Role = LayoutEventRole.Notification, SenderType = typeof(TripPlanAssignmentInfo))]
        private void trainStopped(ActiveTripInfo trip, bool atWayPoint) {
            trip.Status = TripStatus.Stopped;

            EventManager.Event(new LayoutEvent("driver-stop", trip.Train));

            if (atWayPoint) {
                Trace.WriteLineIf(traceDispatching.TraceInfo, $"Train {trip.Train.DisplayName} stopped on way point");

                if (trip.PendingDriverInstructions != null) {
                    trip.PendingDriverInstructions.Dispose();
                    trip.PendingDriverInstructions = null;
                }

                if (trip.CurrentWaypoint != null) {
                    if (trip.CurrentWaypoint.StartCondition != null) {
                        trip.Status = TripStatus.WaitStartCondition;

                        Trace.WriteLineIf(traceDispatching.TraceInfo, "Start waiting for start condition");
                        trip.PendingStartCondition = EventManager.EventScript("Start condition for " + trip.Train.DisplayName + " for going to " + trip.CurrentWaypoint.Destination.Name,
                            trip.CurrentWaypoint.StartCondition, new Guid[] { trip.TrainId, trip.Id }, new LayoutEvent("dispatcher-start-condition-occurred", trip));

                        LayoutScriptContext context = trip.PendingStartCondition.ScriptContext;

                        EventManager.Event(new LayoutEvent("set-script-context", trip, context));
                        trip.PendingStartCondition.Reset();
                    }
                    else {
                        if (trip.PendingLockRequest == null) {
                            Trace.WriteLineIf(traceDispatching.TraceInfo, $"Train {trip.Train.DisplayName} train stopped at way point - no start condition - prepare start");
                            trainPrepareStart(trip);
                        }
                        else
                            Trace.WriteLineIf(traceDispatching.TraceInfo, $"Train {trip.Train.DisplayName} train stopped at way point - waiting for lock");
                    }
                }
                else {
                    trip.Status = TripStatus.Done;
                    EventManager.Event(new LayoutEvent("trip-done", trip));
                }
            }
            else {
                Trace.WriteLineIf(traceDispatching.TraceInfo, $"Train {trip.Train.DisplayName} stopped NOT on way point");
                trip.Status = TripStatus.WaitLock;
            }
        }

        private void trainPrepareStart(ActiveTripInfo trip) {
            Trace.WriteLineIf(traceDispatching.TraceInfo, $"Train {trip.Train.DisplayName} Prepare Start - calling prepareNextTripSection");
            if (prepareNextTripSection(trip, new BlockEntry(trip.Train), null))
                trainStart(trip);           // If train can move right away, start
            else if (trip.Queue.Count == 0)
                trainStopped(trip, false);
        }

        [LayoutEventDef("driver-train-go", Role = LayoutEventRole.Notification, SenderType = typeof(TrainStateInfo), InfoType = typeof(LocomotiveOrientation))]
        private void trainStart(ActiveTripInfo trip) {
            Trace.WriteLineIf(traceDispatching.TraceInfo, $"Start moving - trip of train {trip.Train.DisplayName}");

            if (trip.CurrentWaypoint.DriverInstructions != null) {
                Trace.WriteLineIf(traceDispatcher.TraceInfo, $"Train {trip.Train.DisplayName} Waypoint {trip.CurrentWaypoint.Name} Start driving instructions: {trip.CurrentWaypoint.DriverInstructionsDescription}");

                trip.PendingDriverInstructions = EventManager.EventScript($"Driver instructions for {trip.Train.DisplayName} for going to {trip.CurrentWaypoint.Destination.Name}",
                    trip.CurrentWaypoint.DriverInstructions, new Guid[] { trip.TrainId, trip.Id }, null);

                LayoutScriptContext context = trip.PendingDriverInstructions.ScriptContext;

                EventManager.Event(new LayoutEvent("set-script-context", trip, context));
                trip.PendingDriverInstructions.Reset();
            }

            trip.Status = TripStatus.Go;
            EventManager.Event(new LayoutEvent("driver-train-go", trip.Train, trip.CurrentWaypoint.Direction));
        }

        #endregion

        #region Layout Event Handlers

        [LayoutEvent("prepare-enter-operation-mode")]
        private void dispatcherEnterOperationMode(LayoutEvent e) {
            activeTrips.Clear();
        }

        [LayoutEvent("trip-status-changed", Order = 1000)]
        private void tripStatusChanged(LayoutEvent e) {
            if (e.Sender is ActiveTripInfo trip) {
                if ((trip.Status == TripStatus.Done || trip.Status == TripStatus.Aborted) && trip.ActivePolicies != null) {
                    foreach (LayoutEventScript activePolicy in trip.ActivePolicies)
                        activePolicy.Dispose();
                }
            }
        }

        [LayoutEvent("train-target-speed-changed")]
        private void trainTargetSpeedChanged(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");
            ActiveTripInfo? trip = activeTrips[train.Id];

            if (trip != null)
                EventManager.Event(new LayoutEvent("driver-target-speed-changed", trip.Train));
        }

        [LayoutEvent("train-is-removed")]
        private void canRemoveTrain(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");

            if (activeTrips.Contains(train.Id))
                EventManager.Event(new LayoutEvent("abort-trip", train, true));
        }

        [LayoutEvent("train-is-removed")]
        private void trainRemoved(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");
            ActiveTripInfo? trip = activeTrips[train.Id];

            if (trip != null)
                EventManager.Event(new LayoutEvent("abort-trip", train, true));

            blockUnlockingManager.RemoveByOwner(train.Id);

            // Free all locks or pending locks that are owned by this train
            EventManager.Event(new LayoutEventInfoValueType<object, Guid>("free-owned-layout-locks", null, train.Id).SetOption("ReleasePending", true));
        }

        [LayoutEvent("train-enter-block", Order = 1000)]
        [LayoutEventDef("driver-prepare-stop", Role = LayoutEventRole.Notification, SenderType = typeof(TrainStateInfo))]
        private void trainEnterBlock(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");
            var block = Ensure.NotNull<LayoutBlock>(e.Info, "block");
            ActiveTripInfo? trip = activeTrips[train.Id];

            if (trip != null) {
                if (trip.Queue.Count > 0 && block.Id == (trip.Queue.Peek()!).Block.Id) {
                    BlockEntry blockEntry = trip.Queue.Dequeue();

                    Trace.WriteLineIf(traceDispatching.TraceInfo, $"Train {trip.Train.DisplayName} entered the expected block: {blockEntry.Block} action {blockEntry.Action}");

                    // If arrived to way point, move to next one
                    if ((blockEntry.Action & BlockAction.AtWaypoint) != 0)
                        trip.CurrentWaypointIndex = getNextWaypointIndex(trip);

                    if ((blockEntry.Action & BlockAction.PrepareNextSection) != 0) {
                        BlockEntry? nextBlockEntry = null;

                        if (trip.Queue.Count > 0)
                            nextBlockEntry = trip.Queue.Peek();

                        Trace.WriteLineIf(traceDispatching.TraceInfo, $"Prepare next trip section - trip of train {trip.Train.DisplayName}");

                        prepareNextTripSection(trip, blockEntry, nextBlockEntry);
                        blockEntry.RemoveAction(BlockAction.PrepareNextSection);
                    }

                    if ((blockEntry.Action & BlockAction.PrepareStop) != 0) {
                        trip.Status = TripStatus.PrepareStop;
                        Trace.WriteLineIf(traceDispatching.TraceInfo, $"Prepare stop - trip of train {trip.Train.DisplayName}");
                        EventManager.Event(new LayoutEvent("driver-prepare-stop", trip.Train));
                    }

                    if ((blockEntry.Action & BlockAction.Stop) != 0 || trip.Queue.Count == 0) {
                        trip.Status = TripStatus.Stopped;
                        Trace.WriteLineIf(traceDispatching.TraceInfo, $"Stop - trip of train {trip.Train.DisplayName}");

                        trainStopped(trip, (blockEntry.Action & BlockAction.AtWaypoint) != 0);
                    }

                    if ((blockEntry.Action & BlockAction.AbortTrip) != 0) {
                        trip.Status = TripStatus.Aborted;
                        trip.Cancel();
                    }

                    if ((blockEntry.Action & BlockAction.SuspendTrip) != 0) {
                        trip.SuspendedStatus = trip.Status;
                        trip.SuspendedEvent = new LayoutEvent("dispatcher-resume-suspended", trip);
                        trip.Status = TripStatus.Suspended;
                    }

                    if (trip.Status == TripStatus.Go || trip.Status == TripStatus.Done)
                        EventManager.Event(new LayoutEvent("driver-update-speed", trip.Train));
                }
                else {
                    Trace.WriteLineIf(traceDispatching.TraceInfo, $"Train {trip.Train.DisplayName} entered unexpected block: {block}");
                    if (trip.Queue.Count == 0)
                        Trace.WriteLineIf(traceDispatching.TraceInfo, "  The trip queue is empty");
                    else
                        Trace.WriteLineIf(traceDispatching.TraceInfo, $"  The expected block is: {(trip.Queue.Peek()!).Block}");

                    if (block.LockRequest == null)
                        Error(block, $"Train {train.DisplayName} entered to an block that was not allocated to any train");
                    else {
                        if (block.LockRequest.IsManualDispatchLock)
                            Error(block, $"Train {train.DisplayName} entered block which is part of a manual dispatch region");
                        else {
                            var otherTrain = LayoutModel.StateManager.Trains[block.LockRequest.OwnerId];

                            if (train.Managed)
                                train.Speed = 0;            // Stop the train at once

                            if (otherTrain != null) {
                                if (otherTrain.Id != train.Id) {
                                    Error(block, $"Train {train.DisplayName} entered a block which is already allocated to train {otherTrain.DisplayName}");
                                    if (otherTrain.Managed)
                                        otherTrain.Speed = 0;
                                }
                                //      Ignore re-detection of block that belongs to this train
                                //								else
                                //									Error(block, "Train " + train.Name + " entered wrong block (e.g. wrong turnout turn)");
                            }
                            else
                                Error(block, "Train " + train.DisplayName + " entered a block which is already allocated");
                        }
                    }
                }
            }
            else {
                bool generateError = true;

                // Train is not associated with an active trip

                if (block.LockRequest == null) {
                    LayoutLockRequest lockRequest = new LayoutLockRequest(train.Id);

                    lockRequest.Blocks.Add(block);
                    EventManager.Event(new LayoutEvent("request-layout-lock", lockRequest));
                }
                else {
                    if (!block.LockRequest.IsManualDispatchLock) {
                        var otherTrain = LayoutModel.StateManager.Trains[block.LockRequest.OwnerId];

                        if (otherTrain != null) {
                            if (otherTrain.Id != train.Id) {
                                if (otherTrain.Managed)
                                    otherTrain.Speed = 0;
                            }
                        }
                    }
                    else
                        generateError = false;
                }

                if (generateError) {
                    Error(train, "Train " + train.DisplayName + " is not associated with a trip-plan, and is not in a manual dispatch region. It is not allowed to move!");

                    if (train.Managed)
                        train.Speed = 0;
                }
            }
        }

        [LayoutEvent("train-leaving-block-details")]
        private void trainLeavingBlockDetails(LayoutEvent e) {
            var trainChangingBlock = Ensure.NotNull<TrainChangingBlock>(e.Info, "trainChangingBlock");
            ActiveTripInfo? trip = activeTrips[trainChangingBlock.Train.Id];

            if (trip != null) {
                if (!trip.Queue.Contains(trainChangingBlock.FromBlock))
                    trainChangingBlock.BlockEdge.SignalState = LayoutSignalState.Red;
            }
        }

        [LayoutEvent("train-leave-block")]
        private void trainLeaveBlock(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");
            var block = Ensure.NotNull<LayoutBlock>(e.Info, "block");

            Trace.WriteLineIf(traceDispatching.TraceInfo, $"Train {train.DisplayName} leaves block {block}");

            if (block.LockRequest != null) {
                if (block.LockRequest.OwnerId == train.Id) {
                    int timeout = 12000;

                    if (block.BlockDefinintion.Info.IsOccupancyDetectionBlock) {
                        if (block.BlockDefinintion.Info.TrainDetected == false) {
                            if (train.LastCarDetectedByOccupancyBlock)
                                timeout = 1000;
                        }
                    }
                    else if (block.BlockDefinintion.Info.NoFeedback)
                        timeout = 1500;
                    else if (train.LastCarTriggerBlockEdge)
                        timeout = 5000;

                    ActiveTripInfo trip = activeTrips[train.Id];

                    if (trip == null || !trip.Queue.Contains(block.Id))
                        blockUnlockingManager.Add(block, timeout);          // Unlock block 5 seconds if train has magnet on last car
                    else
                        Trace.WriteLineIf(traceDispatching.TraceVerbose, $" Not adding block {block} to unlocking manager because it is in queue of train {train.DisplayName}");
                }
            }

            // Handle the special case of no feedback loop - leaving this block does not generate a "train-leaving-block-details"
            // event.
            if (block.BlockDefinintion.Info.NoFeedback && block.LockRequest != null && block.LockRequest.OwnerId == train.Id)
                blockUnlockingManager.Add(block, 3000);         // Unlock block 3 seconds after it has been "left".
        }

        [LayoutEvent("manual-dispatch-region-activation-changed")]
        private void manualDisptachRegionActivationChanged(LayoutEvent e) {
            bool active = (bool)e.Info;

            Trace.WriteLineIf(traceDispatching.TraceInfo, $"Manual dispatch region was {(active ? "granted" : "freed")} recalculate trips");

            // If manual dispatch was freed, need to retry only waiting, if it was granted, recalc all trains, since it may
            // block a route which is already calculated
            EventManager.Event(new LayoutEvent("dispatcher-retry-waiting-trains", null, (bool)active));
        }

        [LayoutEvent("train-detection-block-free")]
        private void trainDetectionBlockFree(LayoutEvent e) {
            Trace.WriteLineIf(traceDispatching.TraceInfo, "Occupancy block freed recalculate trips - retry waiting trains");

            EventManager.Event(new LayoutEvent("dispatcher-retry-waiting-trains", null, false));
        }

        [LayoutEvent("train-detection-block-will-be-occupied")]
        private void trainDetectionBlockOccupied(LayoutEvent e) {
            var occupancyBlock = Ensure.NotNull<LayoutOccupancyBlock>(e.Sender, "occupancyBlock");

            if (occupancyBlock.BlockDefinintion.Info.UnexpectedTrainDetected) {
                Trace.WriteLineIf(traceDispatching.TraceInfo, "Unexpected train detected - recalculate all train trips");
                EventManager.Event(new LayoutEvent("dispatcher-retry-waiting-trains", null, true));
            }
        }

        #endregion

        #region Internal Dispatcher States Event Handlers

        [LayoutEvent("dispatcher-start-condition-occurred")]
        private void dispatcherStartConditionOccurred(LayoutEvent e) {
            var trip = Ensure.NotNull<ActiveTripInfo>(e.Sender, "trip");

            if (trip.Status == TripStatus.Suspended) {
                Trace.WriteLineIf(traceDispatching.TraceInfo, $"Suspending {e.EventName} for train {trip.Train.DisplayName}");
                trip.SuspendedEvent = e;
            }
            else {
                trip.PendingStartCondition?.Dispose();
                trip.PendingStartCondition = null;

                Trace.WriteLineIf(traceDispatching.TraceInfo, $"Train {trip.Train.DisplayName} Start condition occurred - prepare start");
                trainPrepareStart(trip);
            }
        }

        [LayoutEvent("dispatcher-layout-lock-obtained")]
        private void dispatcherLayoutLockObtained(LayoutEvent e) {
            var trip = Ensure.NotNull<ActiveTripInfo>(e.Sender, "trip");

            if (trip.Status == TripStatus.Suspended) {
                Trace.WriteLineIf(traceDispatching.TraceInfo, $"Suspending {e.EventName} for train {trip.Train.DisplayName}");
                trip.SuspendedEvent = e;
            }
            else if (trip.Status != TripStatus.Done) {
                var route = Ensure.NotNull<ITripRoute>(e.Info, "route");
                LayoutLockRequest? lockRequest = trip.PendingLockRequest;

                trip.PendingLockRequest = null;         // Request was granted

                if (lockRequest != null) {
                    if (traceDispatching.TraceInfo) {
                        Trace.Write($"Dispatcher lock obtained for train {trip.Train.DisplayName} ");
                        lockRequest.Dump();
                    }

                    // Make sure that the delay lock removal process will not remove those newly locked blocks. Cancel
                    // removal of any block that lock was granted and is still in the queue
                    foreach (LayoutLockBlockEntry blockEntry in lockRequest.Blocks)
                        if (trip.Queue.Contains(blockEntry.Block.Id))
                            blockUnlockingManager.Remove(blockEntry.Block.Id);
                }

                BlockEntry? nextBlockEntry = trip.Queue.Peek();

                if (nextBlockEntry != null && (nextBlockEntry.Action & BlockAction.Stop) != 0) {
                    Trace.WriteLineIf(traceDispatching.TraceVerbose, "  When lock is obtained, the next block in the queue has STOP action, remove it");
                    nextBlockEntry.RemoveAction(BlockAction.Stop);
                }

                setSwitchesForTripSection(trip, route);
                trainStart(trip);
            }
        }

        [LayoutEvent("dispatcher-retry-waiting-trains")]
        private void retryWaitingTrains(LayoutEvent e) {
            bool retryAllTrains = (bool)(e.Info ?? true);

            retryTripsWaitingForLocks(retryAllTrains);
        }

        [LayoutEvent("dispatcher-resume-suspended")]
        private void resumeSuspendedTrain(LayoutEvent e) {
            var trip = Ensure.NotNull<ActiveTripInfo>(e.Sender, "trip");

            if (trip.Status == TripStatus.Go || trip.Status == TripStatus.PrepareStop)
                trainPrepareStart(trip);
        }

        #endregion

        #region Validate trip route

        private class CheckRoutesResult {
            public IDictionary<Guid, TrackEdge> ReachableDestinationEdges = new Dictionary<Guid, TrackEdge>();
            public List<TrackEdge> ValidSourceEdges = new List<TrackEdge>();
            public List<TrackEdge> InvalidSourceEdges = new List<TrackEdge>();
        }

        private CheckRoutesResult checkRoutes(List<TrackEdge> sourceEdges, List<LayoutBlockDefinitionComponent> destinations, LocomotiveOrientation direction, TrainStateInfo train, bool trainStopping) {
            CheckRoutesResult result = new CheckRoutesResult();

            foreach (TrackEdge sourceEdge in sourceEdges) {
                bool sourceIsValid = false;

                foreach (LayoutBlockDefinitionComponent destination in destinations) {
                    BestRoute bestRoute = TripPlanningServices.FindBestRoute(sourceEdge.Track, sourceEdge.ConnectionPoint, direction, destination, train.Id, trainStopping);

                    if (bestRoute.Quality.IsValidRoute) {
                        LayoutTrackComponent track = destination.Track;

                        sourceIsValid = true;

                        if (!result.ReachableDestinationEdges.ContainsKey(destination.Id))
                            result.ReachableDestinationEdges.Add(destination.Id, new TrackEdge(track, bestRoute.DestinationFront));
                    }
                }

                if (sourceIsValid)
                    result.ValidSourceEdges.Add(sourceEdge);
                else
                    result.InvalidSourceEdges.Add(sourceEdge);
            }

            return result;
        }

        [LayoutEvent("validate-trip-plan-route")]
        private void validateTripPlanRoute(LayoutEvent e) {
            var tripPlan = Ensure.NotNull<TripPlanInfo>(e.Sender, "tripPlan");
            var train = Ensure.NotNull<TrainStateInfo>(e.Info, "train");
            List <TrackEdge> sourceEdges = new List<TrackEdge>();
            List<ITripRouteValidationAction> actions = new List<ITripRouteValidationAction>();
            bool canBeFixed = true;

            var locoBlock = train.LocomotiveBlock;
            var locoLocation = train.LocomotiveLocation;

            Debug.Assert(locoBlock != null && locoLocation != null);
            sourceEdges.Add(new TrackEdge(locoBlock.BlockDefinintion.Track, locoLocation.DisplayFront));

            for (int wayPointIndex = 0; wayPointIndex < tripPlan.Waypoints.Count; wayPointIndex++) {
                TripPlanWaypointInfo wayPoint = tripPlan.Waypoints[wayPointIndex];
                LocomotiveOrientation direction = wayPoint.Direction;
                List<LayoutBlockDefinitionComponent> destinations = new List<LayoutBlockDefinitionComponent>();

                // try to find route from each source to each destination. If no route can be found to any destination,
                // remove this source 
                foreach (LayoutBlockDefinitionComponent blockInfo in wayPoint.Destination.BlockInfoList)
                    destinations.Add(blockInfo);

                CheckRoutesResult result = checkRoutes(sourceEdges, destinations, direction, train, wayPoint.TrainStopping);

                if (result.ValidSourceEdges.Count == 0) {       // Could not get to anywhere with this direction
                                                                // Try getting some where with the opposite direction
                    direction = (direction == LocomotiveOrientation.Forward) ? LocomotiveOrientation.Backward : LocomotiveOrientation.Forward;

                    result = checkRoutes(sourceEdges, destinations, direction, train, wayPoint.TrainStopping);

                    if (result.ValidSourceEdges.Count == 0) {
                        // There is no path between the source and destination in any direction
                        canBeFixed = false;
                        actions.Add(new RouteValidationActionNoPath(wayPointIndex));
                        break;
                    }
                    else
                        actions.Add(new RouteValidationActionSwitchDirection(wayPointIndex, direction));
                }

                if (result.InvalidSourceEdges.Count > 0) {
                    // Remove all sources that could not lead to any destination

                    if (wayPointIndex > 0) {
                        foreach (TrackEdge edge in result.InvalidSourceEdges) {
                            var blockInfo = edge.Track.BlockDefinitionComponent;

                            Debug.Assert(blockInfo != null);
                            actions.Add(new RouteValidationActionRemoveLocation(wayPointIndex - 1, blockInfo));
                        }

                        Debug.Assert(tripPlan.Waypoints[wayPointIndex - 1].Destination.Count != 0);
                    }
                }

                sourceEdges.Clear();

                // Recommend removing from the trip plan, any destination that could not be reached
                foreach (LayoutBlockDefinitionComponent blockInfo in wayPoint.Destination.BlockInfoList) {
                    if (result.ReachableDestinationEdges.ContainsKey(blockInfo.Id))
                        sourceEdges.Add(result.ReachableDestinationEdges[blockInfo.Id]);
                    else
                        actions.Add(new RouteValidationActionRemoveLocation(wayPointIndex, blockInfo));
                }
            }

            e.Info = new RouteValidationResult(canBeFixed, actions);
        }

        #region Trip route validation action classes

        private class RouteValidationResult : ITripRouteValidationResult {
            public RouteValidationResult(bool canBeFixed, List<ITripRouteValidationAction> actions) {
                this.CanBeFixed = canBeFixed;
                this.Actions = actions.AsReadOnly();
            }

            public bool CanBeFixed { get; }

            public IList<ITripRouteValidationAction> Actions { get; }
        }

        private class RouteValidationActionBase {
            protected int waypointIndex;

            public RouteValidationActionBase(int wayPointIndex) {
                this.waypointIndex = wayPointIndex;
            }

            public int WaypointIndex => waypointIndex;
        }

        private class RouteValidationActionNoPath : RouteValidationActionBase, ITripRouteValidationAction {
            public RouteValidationActionNoPath(int wayPointIndex) : base(wayPointIndex) {
            }

            public string Description => "No path from previous way point to this one";

            public void Apply(TripPlanInfo tripPlan) {
                Debug.Assert(false, "Cannot fix no path problem");
            }
        }

        private class RouteValidationActionSwitchDirection : RouteValidationActionBase, ITripRouteValidationAction {
            private readonly LocomotiveOrientation direction;

            public RouteValidationActionSwitchDirection(int wayPointIndex, LocomotiveOrientation direction) : base(wayPointIndex) {
                this.direction = direction;
            }

            public string Description => "Switch direction from " + ((direction == LocomotiveOrientation.Forward) ? "backward" : "forward") + " to " +
                        ((direction == LocomotiveOrientation.Forward) ? "forward" : "backward");

            public void Apply(TripPlanInfo tripPlan) {
                tripPlan.Waypoints[waypointIndex].Direction = direction;
            }
        }

        private class RouteValidationActionRemoveLocation : RouteValidationActionBase, ITripRouteValidationAction {
            private readonly LayoutBlockDefinitionComponent blockInfo;

            public RouteValidationActionRemoveLocation(int wayPointIndex, LayoutBlockDefinitionComponent blockInfo) : base(wayPointIndex) {
                this.blockInfo = blockInfo;
            }

            public string Description {
                get {
                    string name = blockInfo.Name;

                    if (string.IsNullOrEmpty(name))
                        name = "<Unnamed location>";

                    return "Remove location " + name + " from way point destination ";
                }
            }

            public void Apply(TripPlanInfo tripPlan) {
                tripPlan.Waypoints[waypointIndex].Destination.Remove(blockInfo.Id);
            }
        }

        #endregion

        #endregion

    }
}

