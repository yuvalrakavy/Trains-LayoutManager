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
using MethodDispatcher;

namespace LayoutManager.Logic {
    [LayoutModule("Layout Lock Manager")]

    internal class LayoutLockManager : LayoutModuleBase, ILayoutLockManagerServices {
        private static readonly LayoutTraceSwitch traceLockManager = new("LockManager", "Layout Lock Manager");
        private readonly Dictionary<Guid, LockedResourceEntry> lockedResourceMap = new();

        /// <summary>
        /// Obtain lock on one or more blocks
        /// </summary>
        /// <param name="e.Sender">A lock request (either as LayoutLockRequest object, or XML)</param>
        [DispatchTarget]
        private bool RequestLayoutLock(LayoutLockRequest lockRequest) {
            bool lockGranted;

            lockRequest.Status = LayoutLockRequest.RequestStatus.NotGranted;

            if (traceLockManager.TraceInfo) {
                Trace.Write("Request layout lock - ");
                DumpLockRequest(lockRequest, 1);
            }

            if (CanRequestBeGranted(lockRequest, true)) {
                DoLockBlock(lockRequest);
                lockGranted = true;                  // Request could be granted immediately 
            }
            else {
                lockGranted = false;                 // Request was queued

                // Check if this request should be granted because it will "help" in clearing the way for granting high priority request
                // this will be the case, if the owner of this request is a train that is locking resource that is needed by this "high
                // priority" (oldest) request
                var train = LayoutModel.StateManager.Trains[lockRequest.OwnerId];

                if (train != null) {
                    foreach (TrainLocationInfo trainLocation in train.Locations) {
                        LockedResourceEntry lockedResourceEntry = (LockedResourceEntry)lockedResourceMap[trainLocation.BlockId];

                        if (lockedResourceEntry.PendingRequests.Length > 0 && lockedResourceEntry.PendingRequests[0].OwnerId != lockRequest.OwnerId) {
                            Trace.WriteIf(traceLockManager.TraceVerbose, " Check if lock request for train " + train.DisplayName + " that blocks higher priority requst - ");

                            if (CanRequestBeGranted(lockRequest, false)) {
                                Trace.WriteLineIf(traceLockManager.TraceVerbose, "Request was granted");
                                DoLockBlock(lockRequest);
                                lockGranted = true;
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
                DumpLocks();
            }

            return lockGranted;
        }

        [DispatchTarget]
        private Task<bool> RequestLayoutLockAsync(LayoutLockRequest lockRequest) {
            if (lockRequest.OnLockGranted != null)
                throw new ArgumentException("RequestLayoutLockAsync LockRequest.OnLockGranted must be null");

            var tcs = new TaskCompletionSource<bool>();

            if (lockRequest.CancellationToken.IsCancellationRequested)
                tcs.TrySetCanceled();
            else {
                lockRequest.OnLockGranted = () => tcs.TrySetResult(false);

                if (lockRequest.CancellationToken.CanBeCanceled) {
                    lockRequest.CancellationToken.Register(
                        () => {
                            if (lockRequest.Status == LayoutLockRequest.RequestStatus.NotGranted)
                                CancelLayoutLock(lockRequest);
                            tcs.TrySetCanceled();
                        }
                    );
                }

                if(RequestLayoutLock(lockRequest))
                    tcs.TrySetResult(true);
            }

            return tcs.Task;
        }

        [DispatchTarget]
        private void RequestManualDispatchLock(ManualDispatchRegionInfo manualDispatchRegion) {
            LayoutSelection activeTrains = new();
            LayoutSelection alreadyManualDispatch = new();
            LayoutSelection partialTrains = new();
            IDictionary reportedTrains = new HybridDictionary();
            LayoutLockRequest manualDispatchRegionLockRequest = new(manualDispatchRegion.Id) {
                Type = LayoutLockType.ManualDispatch
            };

            manualDispatchRegion.CheckIntegrity(this);

            // Check that the region does not contain active trains, and create a lock request for the region
            foreach (Guid blockID in manualDispatchRegion.BlockIdList) {
                LayoutBlock block = LayoutModel.Blocks[blockID];

                if (block.LockRequest != null && block.LockRequest.IsManualDispatchLock)
                    alreadyManualDispatch.Add(block);

                foreach (TrainLocationInfo trainLocation in block.Trains) {
                    if (Dispatch.Call.IsTrainInActiveTrip(trainLocation.Train)) {
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
                throw new LayoutException(activeTrains, $"Requested manual dispatch region has {((activeTrains.Count == 1) ? "a train" : "trains")} in middle of a trip");

            if (alreadyManualDispatch.Count > 0)
                throw new LayoutException(alreadyManualDispatch, "Part of the request manual dispatch region are already part of another active manual dispatch region");

            if (partialTrains.Count > 0)
                throw new LayoutException(partialTrains, $"{((partialTrains.Count == 1) ? "Train is " : "Trains are ")}only partially in the request manual dispatch region - cannot lock region");

            // At that stage there are no active lock request
            foreach (var blockEntry in manualDispatchRegionLockRequest.Blocks) {
                var block = blockEntry.Block;

                if (!lockedResourceMap.TryGetValue(block.Id, out LockedResourceEntry? lockedResourceEntry)) {
                    lockedResourceEntry = new LockedResourceEntry(manualDispatchRegionLockRequest);
                    lockedResourceMap[block.Id] = lockedResourceEntry;
                }
                else
                    lockedResourceEntry.Request = manualDispatchRegionLockRequest;

                block.LockRequest = manualDispatchRegionLockRequest;
            }

            foreach (var blockEntry in manualDispatchRegionLockRequest.Blocks)
                UpdateBlockSignals(blockEntry.Block);

            manualDispatchRegion.SetActive(true);
        }

        [DispatchTarget]
        private void FreeManualDispatchLock(ManualDispatchRegionInfo manualDispatchRegion) {
            IDictionary lockedTrains = new HybridDictionary();

            foreach (Guid blockID in manualDispatchRegion.BlockIdList) {
                LayoutBlock block = LayoutModel.Blocks[blockID];

                if (block.Trains.Count > 1)
                    throw new LayoutException(block, "This block contains more than one train - cannot free manual dispatch region");

                foreach (TrainLocationInfo trainLocation in block.Trains) {
                    if (!lockedTrains.Contains(trainLocation.Train.Id)) {
                        LayoutLockRequest trainLockRequest = new(trainLocation.Train.Id) {
                            Status = LayoutLockRequest.RequestStatus.NotGranted
                        };

                        lockedTrains.Add(trainLocation.Train.Id, trainLocation.Train);
                        foreach (TrainLocationInfo lockedTrainLocation in trainLocation.Train.Locations) {
                            LockedResourceEntry lockedResourceEntry = (LockedResourceEntry)lockedResourceMap[lockedTrainLocation.BlockId];

                            trainLockRequest.Blocks.Add(lockedTrainLocation.Block);
                            lockedResourceEntry.InsertRequest(trainLockRequest);
                        }
                    }
                }
            }

            FreeOwnedLayoutLocks(manualDispatchRegion.Id, false);
        }

        /// <summary>
        /// Free a lock on a given block
        /// </summary>
        /// <param name="e.Sender">
        /// Either lock request (all requested blocked are freed) or BlockID or array of 
        /// block IDs
        /// </param>
        [DispatchTarget]
        private void FreeLayoutLock(object lockedObject) {
            IDictionary<Guid, LockedResourceEntry> freedLockedResourceEntry = new Dictionary<Guid, LockedResourceEntry>();

            switch (lockedObject) {
                case LayoutLockRequest request:
                    foreach (var blockEntry in request.Blocks)
                        DoFreeResource(blockEntry.Block.Id, freedLockedResourceEntry);
                    break;

                case IList<LayoutBlock> blocks:
                    foreach (var block in blocks)
                        DoFreeResource(block.Id, freedLockedResourceEntry);
                    break;

                case IList<Guid> freeList:
                    foreach (var resourceID in freeList)
                        DoFreeResource(resourceID, freedLockedResourceEntry);
                    break;

                case Guid resourceId:
                    DoFreeResource(resourceId, freedLockedResourceEntry);
                    break;
            }

            // Check if the unlocking of those blocks caused pending lock request to be granted
            CheckFreedResource(freedLockedResourceEntry);

            if (traceLockManager.TraceVerbose) {
                Trace.WriteLine("After freeing resource, locks map:");
                DumpLocks();
            }
        }

        [DispatchTarget]
        private void FreeOwnedLayoutLocks(Guid ownerId, bool releasePending) {
            List<Guid> resourceIDsToFree = new();

            foreach (KeyValuePair<Guid, LockedResourceEntry> d in lockedResourceMap) {
                LockedResourceEntry lbe = d.Value;
                Guid resourceID = d.Key;

                if (lbe.Request != null && lbe.Request.OwnerId == ownerId)
                    resourceIDsToFree.Add(resourceID);

                if (releasePending) {
                    foreach (LayoutLockRequest request in lbe.PendingRequests)
                        if (request.OwnerId == ownerId)
                            lbe.RemoveRequest(request);
                }
            }

            FreeLayoutLock(resourceIDsToFree.ToArray());
        }

        [DispatchTarget]
        private void CancelLayoutLock(LayoutLockRequest lockRequest) {
            if (lockRequest.Status != LayoutLockRequest.RequestStatus.NotGranted)
                throw new ArgumentException("Lock request status is different from NotGranted, request cannot be canceled");

            foreach (LayoutLockBlockEntry blockEntry in lockRequest.Blocks) {
                if (lockedResourceMap.TryGetValue(blockEntry.Block.Id, out LockedResourceEntry? lockedResourceEntry)) {
                    lockedResourceEntry.RemoveRequest(lockRequest);

                    if (lockedResourceEntry.Request == null && lockedResourceEntry.PendingRequests.Length == 0) {
                        Trace.WriteLineIf(traceLockManager.TraceInfo, $"LockManager: Removing entry for canceled not-ready block {blockEntry.BlockDefinition.FullDescription}");

                        lockedResourceMap.Remove(blockEntry.Block.Id);
                    }
                }
            }

            foreach (var resourceId in lockRequest.ResourceUseCount.Keys) {
                if (lockedResourceMap.TryGetValue(resourceId, out LockedResourceEntry? lockedResourceEntry)) {
                    lockedResourceEntry.RemoveRequest(lockRequest);

                    if (lockedResourceEntry.Request == null && lockedResourceEntry.PendingRequests.Length == 0) {
                        Trace.WriteLineIf(traceLockManager.TraceInfo,
                            $"LockManager: Removing entry for canceled not-ready block resource {LayoutModel.Component<ModelComponent>(resourceId)?.FullDescription}" ?? $"Missing component {resourceId}");

                        lockedResourceMap.Remove(resourceId);
                    }
                }
            }

            if (lockRequest.Resources != null) {
                foreach (var resource in lockRequest.Resources) {
                    var resourceId = resource.Id;

                    if (lockedResourceMap.TryGetValue(resourceId, out LockedResourceEntry? lockedResourceEntry)) {
                        lockedResourceEntry.RemoveRequest(lockRequest);

                        if (lockedResourceEntry.Request == null && lockedResourceEntry.PendingRequests.Length == 0) {
                            Trace.WriteLineIf(traceLockManager.TraceInfo, $"LockManager: Removing entry for canceled not-ready request resource {resource.FullDescription}");

                            lockedResourceMap.Remove(resourceId);
                        }
                    }
                }
            }

            lockRequest.Status = LayoutLockRequest.RequestStatus.NotRequested;
        }

        [DispatchTarget]
        private ILayoutLockManagerServices GetLayoutLockManagerServices() => (ILayoutLockManagerServices)this;


        [DispatchTarget]
        private void DumpLocks() {
            Trace.WriteLine("==== ==== ==== Lock Map ==== ==== ====");

            foreach (KeyValuePair<Guid, LockedResourceEntry> d in lockedResourceMap) {
                Guid resourceID = d.Key;
                LockedResourceEntry lockedResourceEntry = d.Value;

                DumpResource(resourceID);
                Trace.Write(" - ");

                if (lockedResourceEntry.Request == null)
                    Trace.WriteLine("Not locked");
                else {
                    Trace.WriteLine("Locked by:");
                    DumpLockRequest(lockedResourceEntry.Request, 2);
                }

                if (lockedResourceEntry.PendingRequests.Length > 0) {
                    Trace.WriteLine("  Pending requests:");
                    foreach (LayoutLockRequest request in lockedResourceEntry.PendingRequests)
                        DumpLockRequest(request, 4);
                }
            }

            Trace.WriteLine("==== ==== ==== ======== ==== ==== ====");
        }

        private void DumpResource(Guid resourceID) {
            if (LayoutModel.Blocks.TryGetValue(resourceID, out LayoutBlock? block))
                Trace.Write(block.BlockDefinintion.FullDescription);
            else
                Trace.Write(LayoutModel.Component<ModelComponent>(resourceID, LayoutPhase.All)?.FullDescription ?? $"Missing resource component {resourceID}");
        }

        private void DumpLockRequest(LayoutLockRequest request, int indentation) {
            Trace.Write(new string(' ', indentation));

            var train = LayoutModel.StateManager.Trains[request.OwnerId];

            if (train != null)
                Trace.Write("Train " + train.DisplayName);
            else {
                var manualDispatchRegion = LayoutModel.StateManager.ManualDispatchRegions[request.OwnerId];

                if (manualDispatchRegion != null)
                    Trace.Write("Manual region " + manualDispatchRegion.Name);
                else
                    Trace.Write("** Unknown Owner");
            }

            bool first = true;
            Trace.Write(" blocks: (");
            foreach (LayoutLockBlockEntry blockEntry in request.Blocks) {
                Trace.Write(first ? "" : ", ");
                DumpResource(blockEntry.Block.Id);
                first = false;
            }

            Trace.Write(")");

            first = true;
            if (request.ResourceUseCount.Count > 0) {
                Trace.Write(" resources: (");

                foreach (var resourceIdCountPair in request.ResourceUseCount) {
                    Trace.Write(first ? "" : ", ");
                    DumpResource(resourceIdCountPair.Key);
                    Trace.Write($"={resourceIdCountPair.Value}");
                    first = false;
                }

                Trace.Write(")");
            }

            Trace.WriteLine("");
        }

        public bool HasPendingLocks(Guid resourceID, Guid ofOwner) {
            if (!lockedResourceMap.TryGetValue(resourceID, out LockedResourceEntry? lockedResourceEntry))
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
        private bool CanRequestBeGranted(LayoutLockRequest request, bool oldestRequestOnly) {
            bool canGrant = true;

            foreach (LayoutLockBlockEntry blockEntry in request.Blocks) {
                if (lockedResourceMap.TryGetValue(blockEntry.Block.Id, out LockedResourceEntry? lockedResourceEntry)) {
                    if (lockedResourceEntry.Request != null && lockedResourceEntry.Request.OwnerId != request.OwnerId)
                        canGrant = false;
                    else if (lockedResourceEntry.Request == null && oldestRequestOnly && lockedResourceEntry.PendingRequests.Length > 0 && lockedResourceEntry.PendingRequests[0] != request)
                        canGrant = false;
                }
            }

            // Check that all resources are ready
            if (canGrant)
                canGrant = request.ResourceUseCount.Keys.All(resourceId => {
                    bool IsResourceReady(Guid id) {
                        var resource = LayoutModel.Component<ILayoutLockResource>(resourceId, LayoutPhase.All);

                        return resource?.IsResourceReady() ?? false;
                    }

                    return CanGrantLockFor(request, resourceId) && IsResourceReady(resourceId);
                });

            if (!canGrant) {
                foreach (LayoutLockBlockEntry blockEntry in request.Blocks) {
                    AddPendingEntryFor(request, blockEntry.Block.Id);

                    // Add the request as pending for each additional component that this block specify as needed
                    foreach (var resourceInfo in blockEntry.Block.BlockDefinintion.Info.Resources)
                        AddPendingEntryFor(request, resourceInfo.ResourceId);
                }

                // Add the request as pending for each additional component specified in the request
                if (request.Resources != null) {
                    foreach (var resource in request.Resources)
                        AddPendingEntryFor(request, resource.Id);
                }

                // After adding entries - instruct the resources to make them self ready for locking
                foreach (LayoutLockBlockEntry blockEntry in request.Blocks) {
                    foreach (var resourceInfo in blockEntry.Block.BlockDefinintion.Info.Resources) {
                        var resource = LayoutModel.Component<ILayoutLockResource>(resourceInfo.ResourceId, LayoutPhase.All);

                        if (resource != null && !resource.IsResourceReady())
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

        private void AddPendingEntryFor(LayoutLockRequest request, Guid resourceId) {
            if (!lockedResourceMap.TryGetValue(resourceId, out LockedResourceEntry? lockedResourceEntry)) {
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
        private bool CanGrantLockFor(LayoutLockRequest request, Guid id) {
            return !lockedResourceMap.TryGetValue(id, out LockedResourceEntry? lockedResourceEntry) || lockedResourceEntry.Request == null || lockedResourceEntry.Request.OwnerId == request.OwnerId;
        }

        /// <summary>
        /// Do the actual operation to lock a block (add an entry to the blockToLock map)
        /// </summary>
        /// <param name="lockRequest">The request to grant</param>
        private void DoLockBlock(LayoutLockRequest lockRequest) {
            foreach (LayoutLockBlockEntry blockEntry in lockRequest.Blocks) {
                var block = blockEntry.Block;

                SetResourceLock(block.Id, lockRequest);
                block.LockRequest = lockRequest;
                UpdateBlockSignals(block);
            }

            foreach (var resourceId in lockRequest.ResourceUseCount.Keys)
                SetResourceLock(resourceId, lockRequest);

            lockRequest.CancellationToken = CancellationToken.None;
            lockRequest.Status = LayoutLockRequest.RequestStatus.Granted;

            if (lockRequest.OnLockGranted != null) {
                if (traceLockManager.TraceInfo) {
                    Trace.Write("LockManager: Sending event OnLockGranted for lock: ");
                    lockRequest.Dump();
                }

                lockRequest.OnLockGranted();
            }

            Dispatch.Notification.OnLayoutLockGranted(lockRequest);
        }

        private LockedResourceEntry? SetResourceLock(Guid id, LayoutLockRequest lockRequest) {
            if (!lockedResourceMap.TryGetValue(id, out LockedResourceEntry? lockedResourceEntry))
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
        private void DoFreeResource(Guid blockOrResourceId, IDictionary<Guid, LockedResourceEntry> freedLockBlockOrResourceEntries) {
            if (traceLockManager.TraceInfo) {
                Trace.Write("Request to free resource: ");
                DumpResource(blockOrResourceId);
                Trace.WriteLine("");
            }

            if (lockedResourceMap.TryGetValue(blockOrResourceId, out LockedResourceEntry? lockedBlockOrResourceEntry)) {
                if (!freedLockBlockOrResourceEntries.ContainsKey(blockOrResourceId))
                    freedLockBlockOrResourceEntries.Add(blockOrResourceId, lockedBlockOrResourceEntry);

                if (lockedBlockOrResourceEntry.Request != null)
                    lockedBlockOrResourceEntry.Request.Status = LayoutLockRequest.RequestStatus.PartiallyUnlocked;

                if (LayoutModel.Blocks.TryGetValue(blockOrResourceId, out LayoutBlock? block)) {
                    var lockedBlockEntry = lockedBlockOrResourceEntry;      // Just for clarity

                    if (lockedBlockEntry.Request != null) {
                        foreach (var resourceInfo in block.BlockDefinintion.Info.Resources) {
                            var resourceId = resourceInfo.ResourceId;

                            // If resource is locked by the same request as the block, check if all blocks in the request are no longer locked
                            // if so, the resource can also be unlocked
                            //
                            if (lockedResourceMap.TryGetValue(resourceId, out LockedResourceEntry? lockedResourceEntry) && lockedResourceEntry.Request == lockedBlockEntry.Request) {
                                if (--lockedBlockEntry.Request.ResourceUseCount[resourceId] == 0) {
                                    DoFreeResource(resourceId, freedLockBlockOrResourceEntries);
                                    lockedBlockEntry.Request.ResourceUseCount.Remove(resourceId);
                                }
                            }
                        }

                        if (++lockedBlockEntry.Request.UnlockedBlocksCount == lockedBlockEntry.Request.Blocks.Count) {
                            // Last block in the request was freed, free extra resources that where specified in the request
                            if (lockedBlockEntry.Request.Resources != null) {
                                foreach (var resource in lockedBlockEntry.Request.Resources)
                                    DoFreeResource(resource.Id, freedLockBlockOrResourceEntries);
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
        private LayoutLockRequest? FindPendingRequestFor(Guid ownerID) {
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
        private void CheckFreedResource(IDictionary<Guid, LockedResourceEntry> freedLockResourceEntry) {
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

                    if (CanRequestBeGranted(pendingRequest, true)) {
                        Trace.WriteLineIf(traceLockManager.TraceInfo, "LockManager:   Could be granted - grant this lock");
                        DoLockBlock(pendingRequest);
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
                                LayoutLockRequest? otherTrainRequest = FindPendingRequestFor(trainLocation.Train.Id);

                                Trace.WriteLineIf(traceLockManager.TraceInfo, "LockManager:   Check if can grant request to train " + trainLocation.Train.DisplayName);

                                if (otherTrainRequest != null && CanRequestBeGranted(otherTrainRequest, false)) {
                                    Trace.WriteLineIf(traceLockManager.TraceInfo, "LockManager:   Could grant request to train " + trainLocation.Train.DisplayName);
                                    DoLockBlock(otherTrainRequest);
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
                    Dispatch.Notification.OnLayoutLockReleased(resourceID);

                    if (LayoutModel.Blocks.TryGetValue(resourceID, out LayoutBlock? block)) {
                        block.LockRequest = null;
                        UpdateBlockSignals(block);
                    }
                    else {
                        var resource = LayoutModel.Component<ILayoutLockResource>(resourceID);

                        if (resource != null)
                            resource.FreeResource();
                    }
                }
            }
        }


        [DispatchTarget]
        private void LayoutLockResourceReady(ILayoutLockResource resource) {
            Trace.WriteLineIf(traceLockManager.TraceVerbose, $"LockManager: Resource {LayoutLockBlockEntry.GetDescription(resource.Id)} sent 'layout-resource-lock-ready'");

            if (lockedResourceMap.TryGetValue(resource.Id, out LockedResourceEntry? lockedResourceEntry)) {
                IDictionary<Guid, LockedResourceEntry> testResources = new Dictionary<Guid, LockedResourceEntry> {
                    { resource.Id, lockedResourceEntry }
                };

                // Check if now the lock can be granted.
                CheckFreedResource(testResources);
            }
            else
                Trace.WriteLineIf(traceLockManager.TraceInfo, $"LockManager: Resource {LayoutLockBlockEntry.GetDescription(resource.Id)} send unexpected 'layout-resource-lock-ready' event");
        }

        private void UpdateBlockEdgeSignalState(LayoutBlockEdgeBase blockEdge) {
            var track = blockEdge.Track;

            if (track != null) {
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
        }

        private void UpdateBlockSignals(LayoutBlock block) {
            foreach (LayoutBlockEdgeBase blockEdge in block.BlockEdges)
                UpdateBlockEdgeSignalState(blockEdge);
        }

        [DispatchTarget(Order =900)]
        private bool RebuildLayoutState_ClearSignals(LayoutPhase phase) {
            lockedResourceMap.Clear();

            foreach (LayoutBlockEdgeBase blockEdge in LayoutModel.Components<LayoutBlockEdgeBase>(phase))
                blockEdge.RemoveSignalState();
            return true;
        }

        [DispatchTarget(Order = 1100)]
        private bool RebuildLayoutState_ManualDispatchRegions(LayoutPhase phase) {
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

            return true;
        }

        #region Locked Resource Entry

        private class LockedResourceEntry {
            private List<LayoutLockRequest>? pendingRequests;

            public LockedResourceEntry(LayoutLockRequest? request) {
                this.Request = request;
            }

            #region Properties

            public LayoutLockRequest? Request { get; set; }

            public LayoutLockRequest[] PendingRequests => pendingRequests == null ? (Array.Empty<LayoutLockRequest>()) : pendingRequests.ToArray();

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
