using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;
using System.Diagnostics;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;
using System.Threading.Tasks;
using System.Threading;

namespace LayoutManager.Logic {

	[LayoutModule("Layout Lock Manager")]
	class LayoutLockManager : LayoutModuleBase, ILayoutLockManagerServices {
		static LayoutTraceSwitch	traceLockManager = new LayoutTraceSwitch("LockManager", "Layout Lock Manager");

		Dictionary<Guid, LockedResourceEntry> lockedResourceMap = new Dictionary<Guid, LockedResourceEntry>();
		
		/// <summary>
		/// Obtain lock on one or more blocks
		/// </summary>
		/// <param name="e.Sender">A lock request (either as LayoutLockRequest object, or XML)</param>
		[LayoutEvent("request-layout-lock")]
		void requestLayoutLock(LayoutEvent e) {
			LayoutLockRequest	lockRequest = (LayoutLockRequest)e.Sender;

			lockRequest.Status = LayoutLockRequest.RequestStatus.NotGranted;

			if(traceLockManager.TraceInfo) {
				Trace.Write("Request layout lock - ");
				dumpLockRequest(lockRequest, 1);
			}

			if(canRequestBeGranted(lockRequest, true)) {
				doLockResource(lockRequest);
				e.Info = true;					// Request could be granted immediately 
			}
			else {
				e.Info = false;					// Request was queued

				// Check if this request should be granted because it will "help" in clearing the way for granting high priority request
				// this will be the case, if the owner of this request is a train that is locking resource that is needed by this "high
				// priority" (oldest) request
				TrainStateInfo	train = LayoutModel.StateManager.Trains[lockRequest.OwnerId];

				if(train != null) {
					foreach(TrainLocationInfo trainLocation in train.Locations) {
						LockedResourceEntry	lockedResourceEntry = (LockedResourceEntry)lockedResourceMap[trainLocation.BlockId];

						if(lockedResourceEntry.PendingRequests.Length > 0 && lockedResourceEntry.PendingRequests[0].OwnerId != lockRequest.OwnerId) {
							Trace.WriteIf(traceLockManager.TraceVerbose, " Check if lock request for train " + train.DisplayName + " that blocks higher priority requst - ");

							if(canRequestBeGranted(lockRequest, false)) {
								Trace.WriteLineIf(traceLockManager.TraceVerbose, "Request was granted");
								doLockResource(lockRequest);
								e.Info = true;
							}
							else
								Trace.WriteLineIf(traceLockManager.TraceVerbose, "Request was not granted");

							break;
						}
					}
				}
			}


			if(traceLockManager.TraceVerbose) {
				Trace.WriteLine("After processing lock request, locks map:");
				EventManager.Event(new LayoutEvent(this, "dump-locks"));
			}

		}

		[LayoutAsyncEvent("request-layout-lock-async")]
		Task requestLayoutLockAsync(LayoutEvent e) {
			LayoutLockRequest lockRequest = (LayoutLockRequest)e.Sender;

			if(lockRequest.OnLockGranted != null)
				throw new ArgumentException("request-layout-lock-async LockRequest.OnLockGranted must be null");

			var tcs = new TaskCompletionSource<object>();

			if(lockRequest.CancellationToken.IsCancellationRequested)
				tcs.TrySetCanceled();
			else {
				lockRequest.OnLockGranted = () => { tcs.TrySetResult(false); };

				if(lockRequest.CancellationToken.CanBeCanceled) {
					lockRequest.CancellationToken.Register(
						() =>
						{
							if(lockRequest.Status == LayoutLockRequest.RequestStatus.NotGranted)
								EventManager.Event(new LayoutEvent<LayoutLockRequest>("cancel-layout-lock", lockRequest));
							tcs.TrySetCanceled();
						}
					);
				}

				requestLayoutLock(e);

				if((bool)e.Info)		// Lock was granted immediately
					tcs.TrySetResult(true);
			}

			return tcs.Task;
		}

		[LayoutEvent("request-manual-dispatch-lock")]
		void requestManualDispatchLock(LayoutEvent e) {
			ManualDispatchRegionInfo	manualDispatchRegion = (ManualDispatchRegionInfo)e.Sender;
			LayoutSelection				activeTrains = new LayoutSelection();
			LayoutSelection				alreadyManualDispatch = new LayoutSelection();
			LayoutSelection				partialTrains = new LayoutSelection();
			IDictionary					reportedTrains = new HybridDictionary();
			LayoutLockRequest			manualDispatchRegionLockRequest = new LayoutLockRequest(manualDispatchRegion.Id);

			manualDispatchRegionLockRequest.Type = LayoutLockType.ManualDispatch;
			manualDispatchRegion.CheckIntegrity(this);

			// Check that the region does not contain active trains, and create a lock request for the region
			foreach(Guid blockID in manualDispatchRegion.BlockIdList) {
				LayoutBlock	block = LayoutModel.Blocks[blockID];

				if(block.LockRequest != null && block.LockRequest.IsManualDispatchLock)
					alreadyManualDispatch.Add(block);

				foreach(TrainLocationInfo trainLocation in block.Trains) {
					if((bool)EventManager.Event(new LayoutEvent(trainLocation.Train, "is-train-in-active-trip")) == true) {
						if(!reportedTrains.Contains(trainLocation.Train.Id)) {
							activeTrains.Add(trainLocation.Train);
							reportedTrains.Add(trainLocation.Train.Id, trainLocation.Train);

							foreach(TrainLocationInfo trainInBlockLocation in trainLocation.Train.Locations)
								if(!manualDispatchRegion.BlockIdList.Contains(trainInBlockLocation.BlockId)) {
									partialTrains.Add(trainLocation.Train);
									break;
								}
						}
					}
				}

				manualDispatchRegionLockRequest.ResourceEntries.Add(block);
			}

			if(activeTrains.Count > 0)
				throw new LayoutException(activeTrains, "Requested manual dispatch region has " + ((activeTrains.Count == 1) ? "a train" : "trains") + " in middle of a trip");

			if(alreadyManualDispatch.Count > 0)
				throw new LayoutException(alreadyManualDispatch, "Part of the request manual dispatch region are already part of another active manual dispatch region");

			if(partialTrains.Count > 0)
				throw new LayoutException(partialTrains, ((partialTrains.Count == 1) ? "Train is " : "Trains are ") + "only partially in the request manual dispatch region - cannot lock region");

			// At that stage there are no active lock request
			foreach(LayoutLockResourceEntry resourceEntry in manualDispatchRegionLockRequest.ResourceEntries) {
				LockedResourceEntry	lockedResourceEntry;

				if(!lockedResourceMap.TryGetValue(resourceEntry.Resource.Id, out lockedResourceEntry)) {
					lockedResourceEntry = new LockedResourceEntry(manualDispatchRegionLockRequest);
					lockedResourceMap[resourceEntry.Resource.Id] = lockedResourceEntry;
				}
				else
					lockedResourceEntry.Request = manualDispatchRegionLockRequest;

				resourceEntry.Resource.LockRequest = manualDispatchRegionLockRequest;

				if(resourceEntry.Resource is LayoutBlock)
					updateBlockSignals((LayoutBlock)resourceEntry.Resource);
			}

			manualDispatchRegion.SetActive(true);
		}

		[LayoutEvent("free-manual-dispatch-lock")]
		void freeManualDispatchLock(LayoutEvent e) {
			ManualDispatchRegionInfo	manualDispatchRegion = (ManualDispatchRegionInfo)e.Sender;
			IDictionary					lockedTrains = new HybridDictionary();

			foreach(Guid blockID in manualDispatchRegion.BlockIdList) {
				LayoutBlock	block = LayoutModel.Blocks[blockID];

				if(block.Trains.Count  > 1)
					throw new LayoutException(block, "This block contains more than one train - cannot free manual dispatch region");

				foreach(TrainLocationInfo trainLocation in block.Trains) {
					if(!lockedTrains.Contains(trainLocation.Train.Id)) {
						LayoutLockRequest	trainLockRequest = new LayoutLockRequest(trainLocation.Train.Id);

						trainLockRequest.Status = LayoutLockRequest.RequestStatus.NotGranted;

						lockedTrains.Add(trainLocation.Train.Id, trainLocation.Train);
						foreach(TrainLocationInfo lockedTrainLocation in trainLocation.Train.Locations) {
							LockedResourceEntry	lockedResourceEntry = (LockedResourceEntry)lockedResourceMap[lockedTrainLocation.BlockId];

							Debug.Assert(lockedResourceEntry != null);		// must exist since part of a locked manual dispatch region
							trainLockRequest.ResourceEntries.Add(lockedTrainLocation.Block);

							lockedResourceEntry.InsertRequest(trainLockRequest);
						}
					}
				}
			}

			EventManager.Event(new LayoutEvent(manualDispatchRegion.Id, "free-owned-layout-locks", null, false));
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

			if(e.Sender is LayoutLockRequest) {
				LayoutLockRequest	request = (LayoutLockRequest)e.Sender;

				foreach(LayoutLockResourceEntry resourceEntry in request.ResourceEntries)
					doFreeResource(resourceEntry.Resource.Id, freedLockedResourceEntry);
			}
			else if(e.Sender is LayoutBlock[]) {
				foreach(LayoutBlock block in (LayoutBlock[])e.Sender)
					doFreeResource(block.Id, freedLockedResourceEntry);
			}
			else if(e.Sender is Guid[]) {
				Guid[]	freeList = (Guid[])e.Sender;

				foreach(Guid resourceID in freeList)
					doFreeResource(resourceID, freedLockedResourceEntry);
			}
			else if(e.Sender is Guid)
				doFreeResource((Guid)e.Sender, freedLockedResourceEntry);

			// Check if the unlocking of those blocks caused pending lock request to be granted
			checkFreedResource(freedLockedResourceEntry);

			if(traceLockManager.TraceVerbose) {
				Trace.WriteLine("After freeing resource, locks map:");
				EventManager.Event(new LayoutEvent(this, "dump-locks"));
			}
		}

		[LayoutEvent("free-owned-layout-locks")]
		void freeOwnedLocks(LayoutEvent e) {
			Guid		ownerID = (Guid)e.Sender;
			bool		releasePending = (bool)e.Info;
			List<Guid> resourceIDsToFree = new List<Guid>();

			foreach(KeyValuePair<Guid, LockedResourceEntry> d in lockedResourceMap) {
				LockedResourceEntry	lbe = d.Value;
				Guid				resourceID = d.Key;

				if(lbe.Request != null && lbe.Request.OwnerId == ownerID)
					resourceIDsToFree.Add(resourceID);

				if(releasePending) {
					foreach(LayoutLockRequest request in lbe.PendingRequests)
						if(request.OwnerId == ownerID)
							lbe.RemoveRequest(request);
				}
			}

			EventManager.Event(new LayoutEvent(resourceIDsToFree.ToArray(), "free-layout-lock"));
		}

		[LayoutEvent("cancel-layout-lock")]
		void cancelLayoutLock(LayoutEvent e) {
			LayoutLockRequest	lockRequest = (LayoutLockRequest)e.Sender;

			if(lockRequest.Status != LayoutLockRequest.RequestStatus.NotGranted)
				throw new ArgumentException("Lock request status is different from NotGranted, request cannot be canceled");

			foreach(LayoutLockResourceEntry resourceEntry in lockRequest.ResourceEntries) {
				LockedResourceEntry	lockedResourceEntry = (LockedResourceEntry)lockedResourceMap[resourceEntry.Resource.Id];

				if(lockedResourceEntry != null) {
					lockedResourceEntry.RemoveRequest(lockRequest);

					if(lockedResourceEntry.Request == null && lockedResourceEntry.PendingRequests.Length == 0) {
						Trace.WriteLineIf(traceLockManager.TraceInfo, "LockManager: Removing entry for canceled not-ready resource " + resourceEntry.GetDescription());

						lockedResourceMap.Remove(resourceEntry.Resource.Id);
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

			foreach(KeyValuePair<Guid, LockedResourceEntry> d in lockedResourceMap) {
				Guid				resourceID = d.Key;
				LockedResourceEntry	lockedResourceEntry = d.Value;
				
				dumpResource(resourceID);
				Trace.Write(" - ");

				if(lockedResourceEntry.Request == null)
					Trace.WriteLine("Not locked");
				else {
					Trace.WriteLine("Locked by:");
					dumpLockRequest(lockedResourceEntry.Request, 2);
				}

				if(lockedResourceEntry.PendingRequests.Length > 0) {
					Trace.WriteLine("  Pending requests:");
					foreach(LayoutLockRequest request in lockedResourceEntry.PendingRequests)
						dumpLockRequest(request, 4);
				}
			}

			Trace.WriteLine("==== ==== ==== ======== ==== ==== ====");
		}

		private void dumpResource(Guid resourceID) {
			LayoutBlock	block = LayoutModel.Blocks[resourceID];

			if(block != null) {
				if(string.IsNullOrEmpty(block.Name))
					Trace.Write("Block at " + block.BlockDefinintion.Location);
				else
					Trace.Write("Block: " + block.Name);
			}
			else {
				var component = LayoutModel.Component<ModelComponent>(resourceID, LayoutPhase.All);

				if(component != null)
					Trace.Write("Component " + component.FullDescription);
				else
					Trace.Write("** Unknown resource");
			}
		}

		private void dumpLockRequest(LayoutLockRequest request, int indentation) {
			Trace.Write(new string(' ', indentation));

			TrainStateInfo	train = LayoutModel.StateManager.Trains[request.OwnerId];

			if(train != null)
				Trace.Write("Train " + train.DisplayName);
			else {
				ManualDispatchRegionInfo	manualDispatchRegion = LayoutModel.StateManager.ManualDispatchRegions[request.OwnerId];

				if(manualDispatchRegion != null)
					Trace.Write("Manual region " + manualDispatchRegion.Name);
				else
					Trace.Write("** Unknown Owner");
			}

			Trace.Write(" resources: (");
			foreach(LayoutLockResourceEntry resourceEntry in request.ResourceEntries) {
				dumpResource(resourceEntry.Resource.Id);
				Trace.Write(" ");
			}

			Trace.WriteLine(")");
		}

		public bool HasPendingLocks(Guid resourceID, Guid ofOwner) {
			LockedResourceEntry lockedResourceEntry;

			if(!lockedResourceMap.TryGetValue(resourceID, out lockedResourceEntry))
				return false;

			foreach(LayoutLockRequest request in lockedResourceEntry.PendingRequests)
				if(request.OwnerId == ofOwner)
					return false;

			return true;
		}

		/// <summary>
		/// Check if a lock request can be granted. A request can be granted if all requested blocks are not
		/// locked, or if they are locked by the same owner as the one owning the checked request
		/// </summary>
		/// <param name="request">The request to check</param>
		/// <param name="addPending">
		/// If true, then if a request cannot be granted it will be queued so it will
		/// be re-checked when the block is unlocked</param>
		/// <returns>true if request can be granted</returns>
		private bool canRequestBeGranted(LayoutLockRequest request, bool oldestRequestOnly) {
			bool	canGrant = true;

			foreach(LayoutLockResourceEntry resourceEntry in request.ResourceEntries) {
				LockedResourceEntry lockedResourceEntry;

				if(lockedResourceMap.TryGetValue(resourceEntry.Resource.Id, out lockedResourceEntry)) {
					if(lockedResourceEntry.Request != null && lockedResourceEntry.Request.OwnerId != request.OwnerId)
						canGrant = false;
					else if(lockedResourceEntry.Request == null) {
						if(!resourceEntry.Resource.IsResourceReady()) {
							Trace.WriteLineIf(traceLockManager.TraceInfo, "LockManager: Resource " + resourceEntry.GetDescription() + " is not ready - cannot grant");
							canGrant = false;
						}
						else if(oldestRequestOnly && lockedResourceEntry.PendingRequests.Length > 0 && lockedResourceEntry.PendingRequests[0] != request) {
							Trace.WriteLineIf(traceLockManager.TraceInfo, "LockManager: Resource " + resourceEntry.GetDescription() + " has a previous pending request - cannot grant this request");
							canGrant = false;
						}
					}
				}
				else {
					if(!resourceEntry.Resource.IsResourceReady()) {
						// No request was granted, but one is pending...
						Trace.WriteLineIf(traceLockManager.TraceInfo, "LockManager: Resource " + resourceEntry.GetDescription() + " is not ready - queuing its lock request");
						canGrant = false;
					}
				}
			}

			if(!canGrant) {
				foreach(LayoutLockResourceEntry resourceEntry in request.ResourceEntries) {
					LockedResourceEntry lockedResourceEntry;

					if(!lockedResourceMap.TryGetValue(resourceEntry.Resource.Id, out lockedResourceEntry)) {
						lockedResourceEntry = new LockedResourceEntry(null);

						lockedResourceMap.Add(resourceEntry.Resource.Id, lockedResourceEntry);
					}

					lockedResourceEntry.AddRequest(request);
				}
			}

			return canGrant;
		}

		/// <summary>
		/// Do the actual operation to lock a block (add an entry to the blockToLock map)
		/// </summary>
		/// <param name="lockRequest">The request to grant</param>
		private void doLockResource(LayoutLockRequest lockRequest) {
			foreach(LayoutLockResourceEntry resourceEntry in lockRequest.ResourceEntries) {
				LockedResourceEntry lockedResourceEntry;
				bool				lockIt = false;

				if(!lockedResourceMap.TryGetValue(resourceEntry.Resource.Id, out lockedResourceEntry)) {
					lockedResourceMap[resourceEntry.Resource.Id] = new LockedResourceEntry(lockRequest);

					lockIt = true;
				}
				else {
					if(lockedResourceEntry.Request != null && lockedResourceEntry.Request.OwnerId == lockRequest.OwnerId)
						lockIt = true;		// New lock request, replace the old one
					else if(lockedResourceEntry.Request == null && resourceEntry.Resource.IsResourceReady()) {
						Trace.WriteLineIf(traceLockManager.TraceInfo, "LockManager: Resource " + resourceEntry.GetDescription() + " is ready - locking it");
						lockedResourceEntry.Request = lockRequest;
						lockedResourceEntry.RemoveRequest(lockRequest);

						lockIt = true;
					}
				}

				if(lockIt)
					resourceEntry.Resource.LockRequest = lockRequest;

				if(resourceEntry.Resource is LayoutBlock)
					updateBlockSignals((LayoutBlock)resourceEntry.Resource);
			}

			lockRequest.CancellationToken = CancellationToken.None;
			lockRequest.Status = LayoutLockRequest.RequestStatus.Granted;

			if(lockRequest.OnLockGranted != null) {
				if(traceLockManager.TraceInfo) {
					Trace.Write("LockManager: Sending event OnLockGranted for lock: ");
					lockRequest.Dump();
				}

				lockRequest.OnLockGranted();
			}

			EventManager.Event(new LayoutEvent(lockRequest, "layout-lock-granted"));
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
		private void doFreeResource(Guid resourceID, IDictionary<Guid, LockedResourceEntry> freedLockResourceEntry) {
			LockedResourceEntry	lockedResourceEntry;

			if(traceLockManager.TraceInfo) {
				Trace.Write("Request to free resource: ");
				dumpResource(resourceID);
				Trace.WriteLine("");
			}

			if(lockedResourceMap.TryGetValue(resourceID, out lockedResourceEntry)) {
				if(!freedLockResourceEntry.ContainsKey(resourceID))
					freedLockResourceEntry.Add(resourceID, lockedResourceEntry);

				if(lockedResourceEntry.Request != null)
					lockedResourceEntry.Request.Status = LayoutLockRequest.RequestStatus.PartiallyUnlocked;
				lockedResourceEntry.Request = null;

				if(lockedResourceEntry.PendingRequests.Length == 0)
					lockedResourceMap.Remove(resourceID);		// No longer locked!
			}
		}

		/// <summary>
		/// Look for a pending request for a given owner.
		/// </summary>
		/// <param name="ownerID">The request owner</param>
		/// <returns>The request (if one can be found)</returns>
		private LayoutLockRequest findPendingRequestFor(Guid ownerID) {
			foreach(LockedResourceEntry lockedResourceEntry in lockedResourceMap.Values) {
				foreach(LayoutLockRequest request in lockedResourceEntry.PendingRequests)
					if(request.OwnerId == ownerID)
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
			foreach(KeyValuePair<Guid, LockedResourceEntry> d in freedLockResourceEntry) {
				Guid				resourceID = d.Key;
				LockedResourceEntry	lockedResourceEntry = d.Value;

				if(lockedResourceEntry.PendingRequests.Length > 0) {
					LayoutLockRequest	pendingRequest = lockedResourceEntry.PendingRequests[0];

					if(traceLockManager.TraceInfo) {
						Trace.WriteLine("LockManager: Checking pending requests for freed block: " + LayoutLockResourceEntry.GetDescription(resourceID));
						Trace.Write("LockManager:  Check request: ");
						pendingRequest.Dump();
					}

					if(canRequestBeGranted(pendingRequest, true)) {
						Trace.WriteLineIf(traceLockManager.TraceInfo, "LockManager:   Could be granted - grant this lock");
						doLockResource(pendingRequest);
					}
					else {
						// The "oldest" and therefore the heighest priority request cannot be granted, check what trains are on occupying resources
						// that belongs to the heightest priority request, and check if there is another pending lock owned by any of these trains
						// Check if any of these requests can be granted, this will allow these trains to move and by this clear the way to grant
						// the high priority request.
						Trace.WriteLineIf(traceLockManager.TraceInfo, "LockManager:   could not grant to 'oldest' request");

						foreach(LayoutLockResourceEntry resource in pendingRequest.ResourceEntries) {
							LayoutBlock block;

							if(LayoutModel.Blocks.TryGetValue(resource.Resource.Id, out block)) {
								foreach(TrainLocationInfo trainLocation in block.Trains) {
									LayoutLockRequest	otherTrainRequest = findPendingRequestFor(trainLocation.Train.Id);

									Trace.WriteLineIf(traceLockManager.TraceInfo, "LockManager:   Check if can grant request to train " + trainLocation.Train.DisplayName);

									if(otherTrainRequest != null && canRequestBeGranted(otherTrainRequest, false)) {
										Trace.WriteLineIf(traceLockManager.TraceInfo, "LockManager:   Could grant request to train " + trainLocation.Train.DisplayName);
										doLockResource(otherTrainRequest);
										break;
									}
								}
							}
						}
					}
				}
			}

			foreach(KeyValuePair<Guid, LockedResourceEntry> d in freedLockResourceEntry) {
				Guid				resourceID = d.Key;
				LockedResourceEntry	lockedResourceEntry = d.Value;

				if(!lockedResourceMap.ContainsKey(resourceID) || lockedResourceEntry.Request == null) {
					EventManager.Event(new LayoutEvent(resourceID, "layout-lock-released"));

					LayoutBlock	block;

					if(LayoutModel.Blocks.TryGetValue(resourceID, out block)) {
						block.LockRequest = null;
						updateBlockSignals(block);
					}
					else {
						var resource = LayoutModel.Component<ModelComponent>(resourceID, LayoutModel.ActivePhases) as ILayoutLockResource;

						if(resource != null)
							resource.LockRequest = null;
					}
				}
			}
		}

		[LayoutEvent("layout-lock-resource-ready")]
		void layoutLockResourceReady(LayoutEvent e) {
			ILayoutLockResource		resource = (ILayoutLockResource)e.Sender;
			LockedResourceEntry		lockedResourceEntry;

			if(lockedResourceMap.TryGetValue(resource.Id, out lockedResourceEntry)) {
				IDictionary<Guid, LockedResourceEntry>			testResources = new Dictionary<Guid, LockedResourceEntry>();

				testResources.Add(resource.Id, lockedResourceEntry);

				// Check if now the lock can be granted.
				checkFreedResource(testResources);
			}
			else
				Trace.WriteLineIf(traceLockManager.TraceInfo, "LockManager: Resource " + LayoutLockResourceEntry.GetDescription(resource.Id) + " send unexpected 'layout-resource-lock-ready' event");
   		}

		private void updateBlockEdgeSignalState(LayoutBlockEdgeBase blockEdge) {
			LayoutTrackComponent	track = blockEdge.Track;
			LayoutBlock				block1 = track.GetBlock(track.ConnectionPoints[0]);
			LayoutBlock				block2 = track.GetBlock(track.ConnectionPoints[1]);

			if(block1.IsLocked && block2.IsLocked) {
				if(block1.LockRequest.OwnerId != block2.LockRequest.OwnerId ||
					block1.LockRequest.ResourceEntries[block1.Id].ForceRedSignal || 
					block2.LockRequest.ResourceEntries[block2.Id].ForceRedSignal)
					blockEdge.SignalState = LayoutSignalState.Red;
				else
					blockEdge.SignalState = LayoutSignalState.Green;
			}
			else if(block1.IsLocked || block2.IsLocked)
				blockEdge.SignalState = LayoutSignalState.Red;
			else
				blockEdge.RemoveSignalState();
		}

		private void updateBlockSignals(LayoutBlock block) {
			foreach(LayoutBlockEdgeBase blockEdge in block.BlockEdges)
				updateBlockEdgeSignalState(blockEdge);
		}

		[LayoutEvent("rebuild-layout-state", Order=900)]
		void rebuildLayoutStateClearSignals(LayoutEvent e) {
			LayoutPhase phase = e.GetPhases();
			lockedResourceMap.Clear();

			foreach(LayoutBlockEdgeBase blockEdge in LayoutModel.Components<LayoutBlockEdgeBase>(phase))
				blockEdge.RemoveSignalState();
		}

		[LayoutEvent("rebuild-layout-state", Order=1100)]
		void rebuildLayoutStateManualDispatchRegions(LayoutEvent e) {
			LayoutPhase phase = e.GetPhases();

			if(LayoutModel.StateManager.AllLayoutManualDispatch)
				LayoutModel.StateManager.AllLayoutManualDispatch = true;
			else {
				foreach(ManualDispatchRegionInfo manualDispatchRegion in LayoutModel.StateManager.ManualDispatchRegions) {
					if(manualDispatchRegion.Active) {
						manualDispatchRegion.SetActive(false);		// Fake, so when setting active property to true, will actually grant the region
						manualDispatchRegion.Active = true;
					}
				}
			}
		}

		#region Locked Resource Entry

		class LockedResourceEntry {
			LayoutLockRequest	request;
			List<LayoutLockRequest>	pendingRequests;

			public LockedResourceEntry(LayoutLockRequest request) {
				this.request = request;
			}

			#region Properties

			public LayoutLockRequest Request {
				get {
					return request;
				}

				set {
					request = value;
				}
			}

			public LayoutLockRequest[] PendingRequests {
				get {
					if(pendingRequests == null)
						return new LayoutLockRequest[0];
					else
						return pendingRequests.ToArray();
				}
			}

			#endregion

			#region Operations

			public void AddRequest(LayoutLockRequest request) {
				bool	add = true;

				if(pendingRequests == null)
					pendingRequests = new List<LayoutLockRequest>();
				else if(pendingRequests.Contains(request))
					add = false;

				if(add)
					pendingRequests.Add(request);
			}

			public void InsertRequest(LayoutLockRequest request) {
				bool	insert = true;

				if(pendingRequests == null)
					pendingRequests = new List<LayoutLockRequest>();
				else if(pendingRequests.Contains(request))
					insert = false;

				if(insert)
					pendingRequests.Insert(0, request);
			}

			public void RemoveRequest(LayoutLockRequest request) {
				if(pendingRequests != null)
					pendingRequests.Remove(request);
			}

			#endregion
		}

		#endregion
	}
}
