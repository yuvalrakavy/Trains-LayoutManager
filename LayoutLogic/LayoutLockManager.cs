using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using LayoutManager.Model;
using LayoutManager.Components;
using System.Threading.Tasks;
using System.Threading;

#nullable enable
#pragma warning disable IDE0051, IDE0060
namespace LayoutManager.Logic {

    [LayoutModule("Layout Lock Manager")]

    class LayoutLockManager : LayoutModuleBase, ILayoutLockManagerServices {
        static readonly LayoutTraceSwitch traceLockManager = new LayoutTraceSwitch("LockManager", "Layout Lock Manager");
        readonly Dictionary<Guid, LockedResourceEntry> lockedResourceMap = new Dictionary<Guid, LockedResourceEntry>();

        /// <summary>
        /// Obtain lock on one or more blocks
        /// </summary>
        /// <param name="e.Sender">A lock request (either as LayoutLockRequest object, or XML)</param>
        [LayoutEvent("request-layout-lock")]
        void requestLayoutLock(LayoutEvent e) {
            var lockRequest = Ensure.NotNull<LayoutLockRequest>(e.Sender, "lockRequest");

            lockRequest.Status = LayoutLockRequest.RequestStatus.NotGranted;

            if (traceLockManager.TraceInfo) {
                Trace.Write("Request layout lock - ");
                dumpLockRequest(lockRequest, 1);
            }

            if (canRequestBeGranted(lockRequest, true)) {
                doLockBlock(lockRequest);
                e.Info = true;                  // Request could be granted immediately 
            }
            else {
                e.Info = false;                 // Request was queued

                // Check if this request should be granted because it will "help" in clearing the way for granting high priority request
                // this will be the case, if the owner of this request is a train that is locking resource that is needed by this "high
                // priority" (oldest) request
                TrainStateInfo train = LayoutModel.StateManager.Trains[lockRequest.OwnerId];

                if (train != null) {
                    foreach (TrainLocationInfo trainLocation in train.Locations) {
                        LockedResourceEntry lockedResourceEntry = (LockedResourceEntry)lockedResourceMap[trainLocation.BlockId];

                        if (lockedResourceEntry.PendingRequests.Length > 0 && lockedResourceEntry.PendingRequests[0].OwnerId != lockRequest.OwnerId) {
                            Trace.WriteIf(traceLockManager.TraceVerbose, " Check if lock request for train " + train.DisplayName + " that blocks higher priority requst - ");

                            if (canRequestBeGranted(lockRequest, false)) {
                                Trace.WriteLineIf(traceLockManager.TraceVerbose, "Request was granted");
                                doLockBlock(lockRequest);
                                e.Info = true;
                            }
                            else
                                Trace.WriteLineIf(traceLockManager.TraceVerbose, "Request was not granted");

                            break;
                        }
                    }
                }
            }


            if (traceLockManager.TraceVerbose) {
                Trace.WriteLine("After processing lock request, locks map:");
                EventManager.Event(new LayoutEvent("dump-locks", this));
            }

        }

        [LayoutAsyncEvent("request-layout-lock-async")]
        Task requestLayoutLockAsync(LayoutEvent e) {
            var lockRequest = Ensure.NotNull<LayoutLockRequest>(e.Sender, "lockRequest");

            if (lockRequest.OnLockGranted != null)
                throw new ArgumentException("request-layout-lock-async LockRequest.OnLockGranted must be null");

            var tcs = new TaskCompletionSource<object>();

            if (lockRequest.CancellationToken.IsCancellationRequested)
                tcs.TrySetCanceled();
            else {
                lockRequest.OnLockGranted = () => { tcs.TrySetResult(false); };

                if (lockRequest.CancellationToken.CanBeCanceled) {
                    lockRequest.CancellationToken.Register(
                        () => {
                            if (lockRequest.Status == LayoutLockRequest.RequestStatus.NotGranted)
                                EventManager.Event(new LayoutEvent<LayoutLockRequest>("cancel-layout-lock", lockRequest));
                            tcs.TrySetCanceled();
                        }
                    );
                }

                requestLayoutLock(e);

                if ((bool)e.Info)       // Lock was granted immediately
                    tcs.TrySetResult(true);
            }

            return tcs.Task;
        }

        [LayoutEvent("request-manual-dispatch-lock")]
        void requestManualDispatchLock(LayoutEvent e) {
            var manualDispatchRegion = Ensure.NotNull<ManualDispatchRegionInfo>(e.Sender, "manualDispatchRegion"); ;
            LayoutSelection activeTrains = new LayoutSelection();
            LayoutSelection alreadyManualDispatch = new LayoutSelection();
            LayoutSelection partialTrains = new LayoutSelection();
            IDictionary reportedTrains = new HybridDictionary();
            LayoutLockRequest manualDispatchRegionLockRequest = new LayoutLockRequest(manualDispatchRegion.Id) {
                Type = LayoutLockType.ManualDispatch
            };

            manualDispatchRegion.CheckIntegrity(this);

            // Check that the region does not contain active trains, and create a lock request for the region
            foreach (Guid blockID in manualDispatchRegion.BlockIdList) {
                LayoutBlock block = LayoutModel.Blocks[blockID];

                if (block.LockRequest != null && block.LockRequest.IsManualDispatchLock)
                    alreadyManualDispatch.Add(block);

                foreach (TrainLocationInfo trainLocation in block.Trains) {
                    if ((bool)EventManager.Event(new LayoutEvent("is-train-in-active-trip", trainLocation.Train)) == true) {
                        if (!reportedTrains.Contains(trainLocation.Train.Id)) {
                            activeTrains.Add(trainLocation.Train);
                            reportedTrains.Add(trainLocation.Train.Id, trainLocation.Train);

                            foreach (TrainLocationInfo trainInBlockLocation in trainLocation.Train.Locations)
                                if (!manualDispatchRegion.BlockIdList.Contains(trainInBlockLocation.BlockId)) {
                                    partialTrains.Add(trainLocation.Train);
                                    break;
                                }
                        }
                    }
                }

                manualDispatchRegionLockRequest.Blocks.Add(block);
            }

            if (activeTrains.Count > 0)
                throw new LayoutException(activeTrains, "Requested manual dispatch region has " + ((activeTrains.Count == 1) ? "a train" : "trains") + " in middle of a trip");

            if (alreadyManualDispatch.Count > 0)
                throw new LayoutException(alreadyManualDispatch, "Part of the request manual dispatch region are already part of another active manual dispatch region");

            if (partialTrains.Count > 0)
                throw new LayoutException(partialTrains, ((partialTrains.Count == 1) ? "Train is " : "Trains are ") + "only partially in the request manual dispatch region - cannot lock region");

            // At that stage there are no active lock request
            foreach (LayoutLockBlockEntry blockEntry in manualDispatchRegionLockRequest.Blocks) {
                var block = blockEntry.Block;

                if (!lockedResourceMap.TryGetValue(block.Id, out LockedResourceEntry lockedResourceEntry)) {
                    lockedResourceEntry = new LockedResourceEntry(manualDispatchRegionLockRequest);
                    lockedResourceMap[block.Id] = lockedResourceEntry;
                }
                else
                    lockedResourceEntry.Request = manualDispatchRegionLockRequest;

                block.LockRequest = manualDispatchRegionLockRequest;
            }

            manualDispatchRegion.SetActive(true);
        }

        [LayoutEvent("free-manual-dispatch-lock")]
        void freeManualDispatchLock(LayoutEvent e) {
            var manualDispatchRegion = Ensure.NotNull<ManualDispatchRegionInfo>(e.Sender, "manualDispatchRegion"); ;
            IDictionary lockedTrains = new HybridDictionary();

            foreach (Guid blockID in manualDispatchRegion.BlockIdList) {
                LayoutBlock block = LayoutModel.Blocks[blockID];

                if (block.Trains.Count > 1)
                    throw new LayoutException(block, "This block contains more than one train - cannot free manual dispatch region");

                foreach (TrainLocationInfo trainLocation in block.Trains) {
                    if (!lockedTrains.Contains(trainLocation.Train.Id)) {
                        LayoutLockRequest trainLockRequest = new LayoutLockRequest(trainLocation.Train.Id) {
                            Status = LayoutLockRequest.RequestStatus.NotGranted
                        };

                        lockedTrains.Add(trainLocation.Train.Id, trainLocation.Train);
                        foreach (TrainLocationInfo lockedTrainLocation in trainLocation.Train.Locations) {
                            LockedResourceEntry lockedResourceEntry = (LockedResourceEntry)lockedResourceMap[lockedTrainLocation.BlockId];

                            Debug.Assert(lockedResourceEntry != null);      // must exist since part of a locked manual dispatch region
                            trainLockRequest.Blocks.Add(lockedTrainLocation.Block);

                            lockedResourceEntry.InsertRequest(trainLockRequest);
                        }
                    }
                }
            }

            EventManager.Event(new LayoutEventInfoValueType<object, Guid>("free-owned-layout-locks", null,manualDispatchRegion.Id).SetOption("ReleasePending", false));
        }

        /// <summary>
        /// Free a lock on a given block
        /// </summary>
        /// <param name="e.Sender">
        /// Either lock request (all requested blocked are freed) or BlockID or array of 
        /// block IDs
        /// </param>
        [LayoutEvent("free-layout-lock")]
        void freeLayoutLock(LayoutEvent e) {
            IDictionary<Guid, LockedResourceEntry> freedLockedResourceEntry = new Dictionary<Guid, LockedResourceEntry>();

            switch (e.Sender) {
                case LayoutLockRequest request:
                    foreach (var blockEntry in request.Blocks)
                        doFreeResource(blockEntry.Block.Id, freedLockedResourceEntry);
                    break;

                case LayoutBlock[] blocks:
                    foreach (var block in blocks)
                        doFreeResource(block.Id, freedLockedResourceEntry);
                    break;

                case Guid[] freeList:
                    foreach (var resourceID in freeList)
                        doFreeResource(resourceID, freedLockedResourceEntry);
                    break;

                case Guid resourceId:
                    doFreeResource(resourceId, freedLockedResourceEntry);
                    break;
            }

            // Check if the unlocking of those blocks caused pending lock request to be granted
            checkFreedResource(freedLockedResourceEntry);

            if (traceLockManager.TraceVerbose) {
                Trace.WriteLine("After freeing resource, locks map:");
                EventManager.Event(new LayoutEvent("dump-locks", this));
            }
        }

        [LayoutEvent("free-owned-layout-locks")]
        void freeOwnedLocks(LayoutEvent e0) {
            var e = (LayoutEventInfoValueType<object, Guid>)e0;
            Guid ownerID = e.Info;
            bool releasePending = e.GetBoolOption("ReleasePending");
            List<Guid> resourceIDsToFree = new List<Guid>();

            foreach (KeyValuePair<Guid, LockedResourceEntry> d in lockedResourceMap) {
                LockedResourceEntry lbe = d.Value;
                Guid resourceID = d.Key;

                if (lbe.Request != null && lbe.Request.OwnerId == ownerID)
                    resourceIDsToFree.Add(resourceID);

                if (releasePending) {
                    foreach (LayoutLockRequest request in lbe.PendingRequests)
                        if (request.OwnerId == ownerID)
                            lbe.RemoveRequest(request);
                }
            }

            EventManager.Event(new LayoutEvent("free-layout-lock", resourceIDsToFree.ToArray()));
        }

        [LayoutEvent("cancel-layout-lock")]
        void cancelLayoutLock(LayoutEvent e0) {
            var e = (LayoutEvent<LayoutLockRequest>)e0;
            var lockRequest = Ensure.NotNull<LayoutLockRequest>(e.Sender, "lockRequest");

            if (lockRequest.Status != LayoutLockRequest.RequestStatus.NotGranted)
                throw new ArgumentException("Lock request status is different from NotGranted, request cannot be canceled");

            foreach (LayoutLockBlockEntry blockEntry in lockRequest.Blocks) {

                if (lockedResourceMap.TryGetValue(blockEntry.Block.Id, out LockedResourceEntry lockedResourceEntry)) {
                    lockedResourceEntry.RemoveRequest(lockRequest);

                    if (lockedResourceEntry.Request == null && lockedResourceEntry.PendingRequests.Length == 0) {
                        Trace.WriteLineIf(traceLockManager.TraceInfo, "LockManager: Removing entry for canceled not-ready block " + blockEntry.BlockDefinition.FullDescription);

                        lockedResourceMap.Remove(blockEntry.Block.Id);
                    }
                }
            }

            foreach (var resourceId in lockRequest.ResourceUseCount.Keys) {

                if (lockedResourceMap.TryGetValue(resourceId, out LockedResourceEntry lockedResourceEntry)) {
                    lockedResourceEntry.RemoveRequest(lockRequest);

                    if (lockedResourceEntry.Request == null && lockedResourceEntry.PendingRequests.Length == 0) {
                        Trace.WriteLineIf(traceLockManager.TraceInfo, "LockManager: Removing entry for canceled not-ready block resource " + LayoutModel.Component<ModelComponent>(resourceId).FullDescription);

                        lockedResourceMap.Remove(resourceId);
                    }
                }
            }

            if (lockRequest.Resources != null) {
                foreach (var resource in lockRequest.Resources) {
                    var resourceId = resource.Id;

                    if (lockedResourceMap.TryGetValue(resourceId, out LockedResourceEntry lockedResourceEntry)) {
                        lockedResourceEntry.RemoveRequest(lockRequest);

                        if (lockedResourceEntry.Request == null && lockedResourceEntry.PendingRequests.Length == 0) {
                            Trace.WriteLineIf(traceLockManager.TraceInfo, "LockManager: Removing entry for canceled not-ready request resource " + resource.FullDescription);

                            lockedResourceMap.Remove(resourceId);
                        }
                    }

                }
            }

            lockRequest.Status = LayoutLockRequest.RequestStatus.NotRequested;
        }

        [LayoutEvent("get-layout-lock-manager-services")]
        void getLayoutLockManagerServices(LayoutEvent e) {
            e.Info = (ILayoutLockManagerServices)this;
        }

        [LayoutEvent("dump-locks")]
        void dumpLocks(LayoutEvent e) {
            Trace.WriteLine("==== ==== ==== Lock Map ==== ==== ====");

            foreach (KeyValuePair<Guid, LockedResourceEntry> d in lockedResourceMap) {
                Guid resourceID = d.Key;
                LockedResourceEntry lockedResourceEntry = d.Value;

                dumpResource(resourceID);
                Trace.Write(" - ");

                if (lockedResourceEntry.Request == null)
                    Trace.WriteLine("Not locked");
                else {
                    Trace.WriteLine("Locked by:");
                    dumpLockRequest(lockedResourceEntry.Request, 2);
                }

                if (lockedResourceEntry.PendingRequests.Length > 0) {
                    Trace.WriteLine("  Pending requests:");
                    foreach (LayoutLockRequest request in lockedResourceEntry.PendingRequests)
                        dumpLockRequest(request, 4);
                }
            }

            Trace.WriteLine("==== ==== ==== ======== ==== ==== ====");
        }

        private void dumpResource(Guid resourceID) {

            if (LayoutModel.Blocks.TryGetValue(resourceID, out LayoutBlock block))
                Trace.Write(block.BlockDefinintion.FullDescription);
            else
                Trace.Write(LayoutModel.Component<ModelComponent>(resourceID, LayoutPhase.All).FullDescription);
        }

        private void dumpLockRequest(LayoutLockRequest request, int indentation) {
            Trace.Write(new string(' ', indentation));

            TrainStateInfo train = LayoutModel.StateManager.Trains[request.OwnerId];

            if (train != null)
                Trace.Write("Train " + train.DisplayName);
            else {
                ManualDispatchRegionInfo manualDispatchRegion = LayoutModel.StateManager.ManualDispatchRegions[request.OwnerId];

                if (manualDispatchRegion != null)
                    Trace.Write("Manual region " + manualDispatchRegion.Name);
                else
                    Trace.Write("** Unknown Owner");
            }

            bool first = true;
            Trace.Write(" blocks: (");
            foreach (LayoutLockBlockEntry blockEntry in request.Blocks) {
                Trace.Write(first ? "" : ", ");
                dumpResource(blockEntry.Block.Id);
                first = false;
            }

            Trace.Write(")");

            first = true;
            if (request.ResourceUseCount.Count > 0) {
                Trace.Write(" resources: (");

                foreach (var resourceIdCountPair in request.ResourceUseCount) {
                    Trace.Write(first ? "" : ", ");
                    dumpResource(resourceIdCountPair.Key);
                    Trace.Write($"={resourceIdCountPair.Value}");
                    first = false;
                }

                Trace.Write(")");
            }

            Trace.WriteLine("");
        }

        public bool HasPendingLocks(Guid resourceID, Guid ofOwner) {

            if (!lockedResourceMap.TryGetValue(resourceID, out LockedResourceEntry lockedResourceEntry))
                return false;

            foreach (LayoutLockRequest request in lockedResourceEntry.PendingRequests)
                if (request.OwnerId == ofOwner)
                    return false;

            return true;
        }

        /// <summary>
        /// Check if a lock request can be granted. A request can be granted if all requested blocks are not
        /// locked, or if they are locked by the same owner as the one owning the checked request. In addition, all the resources
        /// that are requested by the request must be ready
        /// </summary>
        /// <param name="request">The request to check</param>
        /// <param name="oldestRequestOnly">
        /// If true, it will be possible to grant only the "oldest" request
        /// </param>
        /// <returns>true if request can be granted</returns>
        private bool canRequestBeGranted(LayoutLockRequest request, bool oldestRequestOnly) {
            bool canGrant = true;

            foreach (LayoutLockBlockEntry blockEntry in request.Blocks) {

                if (lockedResourceMap.TryGetValue(blockEntry.Block.Id, out LockedResourceEntry lockedResourceEntry)) {
                    if (lockedResourceEntry.Request != null && lockedResourceEntry.Request.OwnerId != request.OwnerId)
                        canGrant = false;
                    else if (lockedResourceEntry.Request == null && oldestRequestOnly && lockedResourceEntry.PendingRequests.Length > 0 && lockedResourceEntry.PendingRequests[0] != request)
                        canGrant = false;
                }
            }

            // Check that all resources are ready
            if (canGrant)
                canGrant = request.ResourceUseCount.Keys.All(resourceId => canGrantLockFor(request, resourceId) && LayoutModel.Component<ILayoutLockResource>(resourceId, LayoutPhase.All).IsResourceReady());

            if (!canGrant) {
                foreach (LayoutLockBlockEntry blockEntry in request.Blocks) {
                    addPendingEntryFor(request, blockEntry.Block.Id);

                    // Add the request as pending for each additional component that this block specify as needed
                    foreach (var resourceInfo in blockEntry.Block.BlockDefinintion.Info.Resources)
                        addPendingEntryFor(request, resourceInfo.ResourceId);
                }

                // Add the request as pending for each additional component specified in the request
                if (request.Resources != null) {
                    foreach (var resource in request.Resources)
                        addPendingEntryFor(request, resource.Id);
                }

                // After adding entries - instruct the resources to make them self ready for locking
                foreach (LayoutLockBlockEntry blockEntry in request.Blocks) {
                    foreach (var resourceInfo in blockEntry.Block.BlockDefinintion.Info.Resources) {
                        var resource = LayoutModel.Component<ILayoutLockResource>(resourceInfo.ResourceId, LayoutPhase.All);

                        if (!resource.IsResourceReady())
                            resource.MakeResourceReady();
                    }
                }

                if (request.Resources != null) {
                    foreach (var resource in request.Resources) {
                        if (!resource.IsResourceReady())
                            resource.MakeResourceReady();
                    }
                }

            }

            return canGrant;
        }

        private void addPendingEntryFor(LayoutLockRequest request, Guid resourceId) {

            if (!lockedResourceMap.TryGetValue(resourceId, out LockedResourceEntry lockedResourceEntry)) {
                lockedResourceEntry = new LockedResourceEntry(null);
                lockedResourceMap.Add(resourceId, lockedResourceEntry);
            }

            lockedResourceEntry.AddRequest(request);
        }

        /// <summary>
        /// Check if a lock can be granted for a component
        /// </summary>
        /// <param name="request">The lock request</param>
        /// <param name="id">component id</param>
        /// <returns>True if the component is not locked or it is locked by the same owner as the request's owner</returns>
        private bool canGrantLockFor(LayoutLockRequest request, Guid id) {

            return !lockedResourceMap.TryGetValue(id, out LockedResourceEntry lockedResourceEntry) || lockedResourceEntry.Request == null || lockedResourceEntry.Request.OwnerId == request.OwnerId;
        }

        /// <summary>
        /// Do the actual operation to lock a block (add an entry to the blockToLock map)
        /// </summary>
        /// <param name="lockRequest">The request to grant</param>
        private void doLockBlock(LayoutLockRequest lockRequest) {
            foreach (LayoutLockBlockEntry blockEntry in lockRequest.Blocks) {
                var block = blockEntry.Block;

                setResourceLock(block.Id, lockRequest);
                block.LockRequest = lockRequest;
                updateBlockSignals(block);
            }

            foreach (var resourceId in lockRequest.ResourceUseCount.Keys)
                setResourceLock(resourceId, lockRequest);

            lockRequest.CancellationToken = CancellationToken.None;
            lockRequest.Status = LayoutLockRequest.RequestStatus.Granted;

            if (lockRequest.OnLockGranted != null) {
                if (traceLockManager.TraceInfo) {
                    Trace.Write("LockManager: Sending event OnLockGranted for lock: ");
                    lockRequest.Dump();
                }

                lockRequest.OnLockGranted();
            }

            EventManager.Event(new LayoutEvent("layout-lock-granted", lockRequest));
        }

        private LockedResourceEntry setResourceLock(Guid id, LayoutLockRequest lockRequest) {
            if (!lockedResourceMap.TryGetValue(id, out LockedResourceEntry lockedResourceEntry))
                lockedResourceMap[id] = new LockedResourceEntry(lockRequest);
            else {
                if (lockedResourceEntry.Request == null || lockedResourceEntry.Request.OwnerId == lockRequest.OwnerId) {
                    lockedResourceEntry.Request = lockRequest;
                    lockedResourceEntry.RemoveRequest(lockRequest);
                }
            }

            return lockedResourceEntry;
        }

        /// <summary>
        /// Do the actual operation of unlocking a block. The LockedResourceEntry is added with the blockID to the
        /// list.
        /// </summary>
        /// <param name="blockID">The ID of the block to unlock</param>
        /// <param name="freedLockBlockEntry">
        /// A dictionary containing the blockID and the LockedResourceEntry it used
        /// to have
        /// </param>
        private void doFreeResource(Guid blockOrResourceId, IDictionary<Guid, LockedResourceEntry> freedLockBlockOrResourceEntries) {

            if (traceLockManager.TraceInfo) {
                Trace.Write("Request to free resource: ");
                dumpResource(blockOrResourceId);
                Trace.WriteLine("");
            }

            if (lockedResourceMap.TryGetValue(blockOrResourceId, out LockedResourceEntry lockedBlockOrResourceEntry)) {
                if (!freedLockBlockOrResourceEntries.ContainsKey(blockOrResourceId))
                    freedLockBlockOrResourceEntries.Add(blockOrResourceId, lockedBlockOrResourceEntry);

                if (lockedBlockOrResourceEntry.Request != null)
                    lockedBlockOrResourceEntry.Request.Status = LayoutLockRequest.RequestStatus.PartiallyUnlocked;


                if (LayoutModel.Blocks.TryGetValue(blockOrResourceId, out LayoutBlock block)) {
                    var lockedBlockEntry = lockedBlockOrResourceEntry;      // Just for clarity

                    Debug.Assert(lockedBlockEntry != null);

                    if (lockedBlockEntry.Request != null) {
                        foreach (var resourceInfo in block.BlockDefinintion.Info.Resources) {
                            var resourceId = resourceInfo.ResourceId;

                            // If resource is locked by the same request as the block, check if all blocks in the request are no longer locked
                            // if so, the resource can also be unlocked
                            //
                            if (lockedResourceMap.TryGetValue(resourceId, out LockedResourceEntry lockedResourceEntry) && lockedResourceEntry.Request == lockedBlockEntry.Request) {
                                Debug.Assert(lockedBlockEntry != null);
                                if (--lockedBlockEntry.Request.ResourceUseCount[resourceId] == 0) {
                                    doFreeResource(resourceId, freedLockBlockOrResourceEntries);
                                    lockedBlockEntry.Request.ResourceUseCount.Remove(resourceId);
                                }
                            }
                        }

                        if (++lockedBlockEntry.Request.UnlockedBlocksCount == lockedBlockEntry.Request.Blocks.Count) {
                            // Last block in the request was freed, free extra resources that where specified in the request
                            if (lockedBlockEntry.Request.Resources != null) {
                                foreach (var resource in lockedBlockEntry.Request.Resources)
                                    doFreeResource(resource.Id, freedLockBlockOrResourceEntries);
                            }
                        }
                    }
                }

                lockedBlockOrResourceEntry.Request = null;

                if (lockedBlockOrResourceEntry.PendingRequests.Length == 0)
                    lockedResourceMap.Remove(blockOrResourceId);        // No pending requests - can free the entry
            }
        }

        /// <summary>
        /// Look for a pending request for a given owner.
        /// </summary>
        /// <param name="ownerID">The request owner</param>
        /// <returns>The request (if one can be found)</returns>
        private LayoutLockRequest? findPendingRequestFor(Guid ownerID) {
            foreach (LockedResourceEntry lockedResourceEntry in lockedResourceMap.Values) {
                foreach (LayoutLockRequest request in lockedResourceEntry.PendingRequests)
                    if (request.OwnerId == ownerID)
                        return request;
            }

            return null;
        }

        /// <summary>
        /// Check each entry in the list. If other lock request are pending on it, check if they can be
        /// granted. Finally if after all the granting of pending request, the block is free, generate
        /// the event.
        /// </summary>
        /// <param name="freedLockBlockEntry"></param>
        private void checkFreedResource(IDictionary<Guid, LockedResourceEntry> freedLockResourceEntry) {
            foreach (KeyValuePair<Guid, LockedResourceEntry> d in freedLockResourceEntry) {
                Guid resourceID = d.Key;
                LockedResourceEntry lockedResourceEntry = d.Value;

                if (lockedResourceEntry.PendingRequests.Length > 0) {
                    LayoutLockRequest pendingRequest = lockedResourceEntry.PendingRequests[0];

                    if (traceLockManager.TraceInfo) {
                        Trace.WriteLine("LockManager: Checking pending requests for freed block: " + LayoutLockBlockEntry.GetDescription(resourceID));
                        Trace.Write("LockManager:  Check request: ");
                        pendingRequest.Dump();
                    }

                    if (canRequestBeGranted(pendingRequest, true)) {
                        Trace.WriteLineIf(traceLockManager.TraceInfo, "LockManager:   Could be granted - grant this lock");
                        doLockBlock(pendingRequest);
                    }
                    else {
                        // The "oldest" and therefore the heighest priority request cannot be granted, check what trains are on occupying resources
                        // that belongs to the heightest priority request, and check if there is another pending lock owned by any of these trains
                        // Check if any of these requests can be granted, this will allow these trains to move and by this clear the way to grant
                        // the high priority request.
                        Trace.WriteLineIf(traceLockManager.TraceInfo, "LockManager:   could not grant to 'oldest' request");

                        foreach (LayoutLockBlockEntry blockEntry in pendingRequest.Blocks) {
                            LayoutBlock block = blockEntry.Block;

                            foreach (TrainLocationInfo trainLocation in block.Trains) {
                                LayoutLockRequest? otherTrainRequest = findPendingRequestFor(trainLocation.Train.Id);

                                Trace.WriteLineIf(traceLockManager.TraceInfo, "LockManager:   Check if can grant request to train " + trainLocation.Train.DisplayName);

                                if (otherTrainRequest != null && canRequestBeGranted(otherTrainRequest, false)) {
                                    Trace.WriteLineIf(traceLockManager.TraceInfo, "LockManager:   Could grant request to train " + trainLocation.Train.DisplayName);
                                    doLockBlock(otherTrainRequest);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            foreach (KeyValuePair<Guid, LockedResourceEntry> d in freedLockResourceEntry) {
                Guid resourceID = d.Key;
                LockedResourceEntry lockedResourceEntry = d.Value;

                if (!lockedResourceMap.ContainsKey(resourceID) || lockedResourceEntry.Request == null) {
                    EventManager.Event(new LayoutEventInfoValueType<object, Guid>("layout-lock-released", null, resourceID));


                    if (LayoutModel.Blocks.TryGetValue(resourceID, out LayoutBlock block)) {
                        block.LockRequest = null;
                        updateBlockSignals(block);
                    }
                    else {
                        var resource = LayoutModel.Component<ILayoutLockResource>(resourceID);

                        if (resource != null)
                            resource.FreeResource();
                    }

                }
            }
        }

        [LayoutEvent("layout-lock-resource-ready")]
        void layoutLockResourceReady(LayoutEvent e) {
            var resource = Ensure.NotNull<ILayoutLockResource>(e.Sender, "resource");

            Trace.WriteLineIf(traceLockManager.TraceVerbose, $"LockManager: Resource {LayoutLockBlockEntry.GetDescription(resource.Id)} sent 'layout-resource-lock-ready'");

            if (lockedResourceMap.TryGetValue(resource.Id, out LockedResourceEntry lockedResourceEntry)) {
                IDictionary<Guid, LockedResourceEntry> testResources = new Dictionary<Guid, LockedResourceEntry> {
                    { resource.Id, lockedResourceEntry }
                };

                // Check if now the lock can be granted.
                checkFreedResource(testResources);
            }
            else
                Trace.WriteLineIf(traceLockManager.TraceInfo, $"LockManager: Resource {LayoutLockBlockEntry.GetDescription(resource.Id)} send unexpected 'layout-resource-lock-ready' event");
        }

        private void updateBlockEdgeSignalState(LayoutBlockEdgeBase blockEdge) {
            LayoutTrackComponent track = blockEdge.Track;
            LayoutBlock block1 = track.GetBlock(track.ConnectionPoints[0]);
            LayoutBlock block2 = track.GetBlock(track.ConnectionPoints[1]);

            if (block1.IsLocked && block2.IsLocked) {
                if (block1.LockRequest!.OwnerId != block2.LockRequest!.OwnerId ||
                    block1.LockRequest.Blocks[block1.Id].ForceRedSignal ||
                    block2.LockRequest.Blocks[block2.Id].ForceRedSignal)
                    blockEdge.SignalState = LayoutSignalState.Red;
                else
                    blockEdge.SignalState = LayoutSignalState.Green;
            }
            else if (block1.IsLocked || block2.IsLocked)
                blockEdge.SignalState = LayoutSignalState.Red;
            else
                blockEdge.RemoveSignalState();
        }

        private void updateBlockSignals(LayoutBlock block) {
            foreach (LayoutBlockEdgeBase blockEdge in block.BlockEdges)
                updateBlockEdgeSignalState(blockEdge);
        }

        [LayoutEvent("rebuild-layout-state", Order = 900)]
        void rebuildLayoutStateClearSignals(LayoutEvent e) {
            LayoutPhase phase = e.GetPhases();
            lockedResourceMap.Clear();

            foreach (LayoutBlockEdgeBase blockEdge in LayoutModel.Components<LayoutBlockEdgeBase>(phase))
                blockEdge.RemoveSignalState();
        }

        [LayoutEvent("rebuild-layout-state", Order = 1100)]
        void rebuildLayoutStateManualDispatchRegions(LayoutEvent e) {
            if (LayoutModel.StateManager.AllLayoutManualDispatch)
                LayoutModel.StateManager.AllLayoutManualDispatch = true;
            else {
                foreach (ManualDispatchRegionInfo manualDispatchRegion in LayoutModel.StateManager.ManualDispatchRegions) {
                    if (manualDispatchRegion.Active) {
                        manualDispatchRegion.SetActive(false);      // Fake, so when setting active property to true, will actually grant the region
                        manualDispatchRegion.Active = true;
                    }
                }
            }
        }

        #region Locked Resource Entry

        class LockedResourceEntry {
            LayoutLockRequest? request;
            List<LayoutLockRequest>? pendingRequests;

            public LockedResourceEntry(LayoutLockRequest? request) {
                this.request = request;
            }

            #region Properties

            public LayoutLockRequest? Request {
                get {
                    return request;
                }

                set {
                    request = value;
                }
            }

            public LayoutLockRequest[] PendingRequests {
                get {
                    if (pendingRequests == null)
                        return new LayoutLockRequest[0];
                    else
                        return pendingRequests.ToArray();
                }
            }

            #endregion

            #region Operations

            public void AddRequest(LayoutLockRequest request) {
                bool add = true;

                if (pendingRequests == null)
                    pendingRequests = new List<LayoutLockRequest>();
                else if (pendingRequests.Contains(request))
                    add = false;

                if (add)
                    pendingRequests.Add(request);
            }

            public void InsertRequest(LayoutLockRequest request) {
                bool insert = true;

                if (pendingRequests == null)
                    pendingRequests = new List<LayoutLockRequest>();
                else if (pendingRequests.Contains(request))
                    insert = false;

                if (insert)
                    pendingRequests.Insert(0, request);
            }

            public void RemoveRequest(LayoutLockRequest request) {
                if (pendingRequests != null)
                    pendingRequests.Remove(request);
            }

            #endregion
        }

        #endregion
    }
}
