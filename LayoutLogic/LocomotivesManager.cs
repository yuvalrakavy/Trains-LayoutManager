using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

using MethodDispatcher;
using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Logic {

    [LayoutModule("Trains Manager", UserControl = false)]
    internal class TrainsManager : LayoutModuleBase {
        #region Internal properties and helper functions

        #endregion

        #region Locomotive action handlers

        //[LayoutEvent("query-action", IfEvent = "LayoutEvent[Options/@Action='set-address']", SenderType = typeof(LocomotiveInfo))]
        [DispatchTarget]
        private bool QueryAction_SetLocoAddress([DispatchFilter] LocomotiveInfo target, [DispatchFilter] string actionName = "set-address") {
            return (target.DecoderType is DccDecoderTypeInfo);
        }

        //[LayoutEvent("get-action", IfSender = "Action[@Type='set-address']", InfoType = typeof(LocomotiveInfo))]
        [DispatchTarget]
        private LayoutAction? GetAction_SetAddress([DispatchFilter(Type = "XPath", Value = "Action[@Type='set-address']")] XmlElement actionElement, [DispatchFilter] LocomotiveInfo locomotive) {
            return locomotive.DecoderType is DccDecoderTypeInfo ? new LayoutDccChangeLocomotiveAddressAction(actionElement, locomotive) : null;
        }

        private const string E_Train = "Train";
        private const string E_Locomotive = "Locomotive";
        private const string E_TrainState = "TrainState";
        private const string Option_LocomotiveAddress = "LocoAddress";
        private const string E_Orientation = "Orientation";
        private const string A_Value = "Value";
        private const string Option_TrainId = "TrainID";
        private const string E_Function = "Function";
        private const string A_Name = "Name";
        private const string A_LocomotiveId = "LocomotiveID";
        private const string Option_FunctionState = "FunctionState";
        private const string Option_Unit = "Unit";
        private const string OptionElement_Address = "Address";
        private const string A_Id = "ID";

        #endregion

        #region Placement validation event handlers

        /// <summary>
        /// Check if a given locomotive (locomotive set members) have valid address for placement on a given.
        /// block If not
        /// an exception is thrown
        /// </summary>
        /// <param name="e.Sender">The XML element of the placed locomotive/locomotive set</param>
        /// <param name="e.Info">
        /// The block (LayoutBlock) where the locomotive is to be placed or the power
        /// (LayoutPower), the power that is applied to the area where the locomotive will need to reside
        /// </param>
        [DispatchTarget]
        private CanPlaceTrainResult IsLocomotiveAddressValid(XmlElement placeableElement, object powerObject, IsLocomotiveAddressValidSettings settings) {
            TrainStateInfo? train = null;

            var power = powerObject switch
            {
                ILayoutPower thePower => thePower,
                LayoutBlock block => block.Power,
                LayoutBlockDefinitionComponent blockDefinition => blockDefinition.Block.Power,
                ILayoutPowerOutlet outlet => outlet.Power,
                TrainStateInfo aTrain when aTrain.LocomotiveBlock != null => aTrain.LocomotiveBlock.Power,
                _ => throw new ArgumentException("Invalid power argument")
            };

            if (power?.Type == LayoutPowerType.Digital) {
                //
                // Locomotive-Set address checks (applied only if the placed object is a locomotive set)
                //
                // a) If there is more than one member assigned to an address, and not all the members
                //    that are assigned to a given address are in the same orientation, generate
                //    an error event.
                // b) (TODO) Check that all members have identical decoder setting.
                //
                if (placeableElement.Name == E_Train) {
                    TrainInCollectionInfo trainInCollection = new(placeableElement);
                    IList<TrainLocomotiveInfo> locomotives = trainInCollection.Locomotives;

                    for (int i = 0; i < locomotives.Count; i++) {
                        LocomotiveInfo loco1 = locomotives[i].Locomotive;

                        if (loco1.AddressProvider.Element == null) {
                            return new CanPlaceTrainResult {
                                Status = CanPlaceTrainStatus.LocomotiveHasNoAddress,
                                Locomotive = loco1,
                                CanBeResolved = true,
                                ResolveMethod = CanPlaceTrainResolveMethod.ReprogramAddress
                            };
                        }

                        if ((loco1.DecoderType.SupportedDigitalPowerFormats & power.DigitalFormats) == 0) {
                            return new CanPlaceTrainResult {
                                Locomotive = loco1,
                                Status = CanPlaceTrainStatus.LocomotiveDigitalFormatNotCompatible,
                                CanBeResolved = false,
                                ResolveMethod = CanPlaceTrainResolveMethod.NotPossible
                            };
                        }

                        if (loco1.Kind == LocomotiveKind.SoundUnit)
                            continue;

                        for (int j = i + 1; j < locomotives.Count; j++) {
                            LocomotiveInfo loco2 = locomotives[j].Locomotive;

                            if (loco2.Kind == LocomotiveKind.SoundUnit)
                                continue;

                            if (loco1.AddressProvider.Unit == loco2.AddressProvider.Unit && locomotives[i].Orientation != locomotives[j].Orientation) {
                                return new CanPlaceTrainResult() {
                                    Status = CanPlaceTrainStatus.LocomotiveDuplicateAddress,
                                    Loco1 = loco1,
                                    Loco2 = loco2,
                                    CanBeResolved = true,
                                    ResolveMethod = CanPlaceTrainResolveMethod.ReprogramAddress
                                };
                            }
                        }
                    }

                    // Got to this point, each member has a unit address, and no invalid duplicates
                    // within the locomotive set
                    var addressMap = GetOnTrackLocomotiveAddressMap(power);

                    foreach (TrainLocomotiveInfo trainLocomotive in locomotives) {
                        var addressMapEntry = addressMap[trainLocomotive.Locomotive.AddressProvider.Unit];

                        if (addressMapEntry != null) {
                            return new CanPlaceTrainResult() {
                                Status = CanPlaceTrainStatus.LocomotiveAddressAlreadyUsed,
                                Loco1 = addressMapEntry.Locomotive,
                                Train = addressMapEntry.Train,
                                CanBeResolved = true,
                                ResolveMethod = CanPlaceTrainResolveMethod.ReprogramAddress
                            };
                        }
                    }
                }
                else if (placeableElement.Name == E_Locomotive) {
                    LocomotiveInfo loco = new(placeableElement);

                    if (!loco.NotManaged) {
                        if (loco.AddressProvider.Element == null || loco.AddressProvider.Unit <= 0) {
                            return new CanPlaceTrainResult() {
                                Status = CanPlaceTrainStatus.LocomotiveHasNoAddress,
                                Locomotive = loco,
                                CanBeResolved = true,
                                ResolveMethod = CanPlaceTrainResolveMethod.ReprogramAddress
                            };
                        }

                        var locoAddress = settings.LocomotiveAddress ?? loco.AddressProvider.Unit;
                        var addressMap = GetOnTrackLocomotiveAddressMap(power);
                        var addressMapEntry = addressMap[locoAddress];

                        // Check that the address is not already used.
                        if (addressMapEntry != null) {
                            bool validDuplicate = false;

                            if (train != null) {
                                if (addressMapEntry.Train.Id == train.Id) {
                                    // Check that the locomotive in the train that uses the same address has the
                                    // same orientation as the placed locomotive
                                    if (loco.Kind != LocomotiveKind.SoundUnit) {
                                        // Look for the locomotive having the same unit address
                                        foreach (TrainLocomotiveInfo trainLoco in train.Locomotives) {
                                            if (trainLoco.Locomotive.Id == loco.Id) {
                                                validDuplicate = false;
                                                break;
                                            }

                                            if (trainLoco.Locomotive.AddressProvider.Unit == loco.AddressProvider.Unit) {
                                                if (trainLoco.Orientation == settings.Orientation)
                                                    // It is valid to have two locomotive with the same address
                                                    // as long as they have the same orientation
                                                    validDuplicate = true;
                                                if (trainLoco.Locomotive.Kind == LocomotiveKind.SoundUnit)
                                                    validDuplicate = true;
                                            }
                                        }
                                    }
                                    else
                                        validDuplicate = true;
                                }
                            }

                            if (!validDuplicate) {
                                return new CanPlaceTrainResult() {
                                    Status = CanPlaceTrainStatus.LocomotiveAddressAlreadyUsed,
                                    Loco1 = loco,
                                    Train = addressMapEntry.Train,
                                    CanBeResolved = true,
                                    ResolveMethod = CanPlaceTrainResolveMethod.ReprogramAddress
                                };
                            }
                        }
                    }
                }
                else
                    throw new ArgumentException("Invalid placeable element");
            }   // Block power is digital

            return new CanPlaceTrainResult() {
                Status = CanPlaceTrainStatus.CanPlaceTrain,
                ResolveMethod = CanPlaceTrainResolveMethod.Resolved,
                CanBeResolved = true
            };
        }

        /// <summary>
        /// Check if a locomotive can be placed on track. If not an exception is thrown
        /// if a locomotive can be placed, exception is not thrown. e.Info will either
        /// be null if the locomotive/locomotive set is not on the track or will contain already
        /// existing locomotive trainState
        /// </summary>
        /// <param name="placeableElement">The XmlElement of the locomotive/locomotive set to be checked</param>
        [DispatchTarget]
        private CanPlaceTrainResult CanLocomotiveBePlacedOnTrack(XmlElement placeableElement, LayoutBlockDefinitionComponent? blockDefinition) {
            var result = new CanPlaceTrainResult();

            if (placeableElement.Name == E_Train) {
                TrainInCollectionInfo trainInCollection = new(placeableElement);
                var oldTrainState = LayoutModel.StateManager.Trains[trainInCollection.Id];

                if (oldTrainState != null)
                    result.Train = oldTrainState;
                else {
                    // The locomotive set is not on the track, ensure that non of its members is on the tracks
                    // Also verify that all members have the same number of speed steps
                    foreach (TrainLocomotiveInfo trainLocomotive in trainInCollection.Locomotives) {
                        LocomotiveInfo loco = trainLocomotive.Locomotive;
                        var memberTrainState = LayoutModel.StateManager.Trains[trainLocomotive.LocomotiveId];

                        if (blockDefinition != null && (loco.Guage & blockDefinition.Guage) == 0) {
                            result.Status = CanPlaceTrainStatus.LocomotiveGuageNotCompatibleWithTrack;
                            result.Locomotive = loco;
                            result.BlockDefinition = blockDefinition;
                            result.CanBeResolved = false;
                            result.ResolveMethod = CanPlaceTrainResolveMethod.NotPossible;
                        }
                        else if (memberTrainState != null) {
                            result.Status = CanPlaceTrainStatus.TrainLocomotiveAlreadyUsed;
                            result.Locomotive = loco;
                            result.Train = memberTrainState;
                            result.CanBeResolved = false;
                            result.ResolveMethod = CanPlaceTrainResolveMethod.NotPossible;
                        }
                    }
                }
            }
            else if (placeableElement.Name == E_Locomotive) {
                Guid id = LayoutModel.LocomotiveCollection.GetElementId(placeableElement);
                var oldTrainState = LayoutModel.StateManager.Trains[id];
                LocomotiveInfo loco = new(placeableElement);

                if (oldTrainState != null) {
                    // The locomotive is already on track
                    if (oldTrainState.Locomotives.Count > 1) {
                        result.Status = CanPlaceTrainStatus.LocomotiveAddressAlreadyUsed;
                        result.Locomotive = loco;
                        result.Train = oldTrainState;
                        result.CanBeResolved = true;
                        result.ResolveMethod = CanPlaceTrainResolveMethod.ReprogramAddress;
                    }
                    else
                        result.Train = oldTrainState;
                }
                else if (blockDefinition != null && (loco.Guage & blockDefinition.Guage) == 0) {
                    result.Status = CanPlaceTrainStatus.LocomotiveGuageNotCompatibleWithTrack;
                    result.Locomotive = loco;
                    result.BlockDefinition = blockDefinition;
                    result.CanBeResolved = false;
                    result.ResolveMethod = CanPlaceTrainResolveMethod.NotPossible;
                }
            }

            return result;
        }

        [DispatchTarget]
        private CanPlaceTrainResult CanLocomotiveBePlaced(XmlElement placedElement, LayoutBlockDefinitionComponent blockDefinition) {
            var result = Dispatch.Call.CanLocomotiveBePlacedOnTrack(placedElement, blockDefinition);
            if (result.Status == CanPlaceTrainStatus.CanPlaceTrain)
                result = Dispatch.Call.IsLocomotiveAddressValid(placedElement, blockDefinition, new IsLocomotiveAddressValidSettings());

            return result;
        }

        #endregion

        #region Locomotive Address map

        private static void AddTrains(LocomotiveAddressMap addressMap, IEnumerable<LayoutBlock> blocks, Func<LocomotiveInfo, TrainStateInfo, OnTrackLocomotiveAddressMapEntry> entryAllocator) {
            foreach (var train in blocks.SelectMany(block => from trainLocation in block.Trains select trainLocation.Train))
                foreach (var locomotive in from trainLocomotive in train.Locomotives select trainLocomotive.Locomotive)
                    if (!addressMap.ContainsKey(locomotive.AddressProvider.Unit))
                        addressMap.Add(locomotive.AddressProvider.Unit, entryAllocator(locomotive, train));
        }

        private static IModelComponentIsCommandStation? ExtractCommandStation(object commandStationObject) => commandStationObject switch
        {
            LayoutBlockDefinitionComponent blockDefinition => blockDefinition.Power.PowerOriginComponent as IModelComponentIsCommandStation,
            IModelComponentIsCommandStation commandStation => commandStation,
            ILayoutPower power => power.PowerOriginComponent as IModelComponentIsCommandStation,
            _ => null
        };

        private void AddOnPoweredTracksLocomotivesToAddressMap(LocomotiveAddressMap addressMap, IModelComponentIsCommandStation commandStation) {
            AddTrains(addressMap,
                from blockDefinition in LayoutModel.Components<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases) where blockDefinition.Power.PowerOriginComponentId == commandStation.Id && blockDefinition.Power.Type == LayoutPowerType.Digital && blockDefinition.Block.HasTrains select blockDefinition.Block,
                (loco, train) => new OnPoweredTrackLocomotiveAddressMapEntry(loco, train)
            );
        }

        private void AddOnNonPoweredTracksLocomotivesToAddressMap(LocomotiveAddressMap addressMap) {
            AddTrains(addressMap,
                from blockDefinition in LayoutModel.Components<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases) where blockDefinition.Power.Type == LayoutPowerType.Disconnected && blockDefinition.Block.HasTrains select blockDefinition.Block,
                (loco, train) => new OnDisconnectedTrackLocomotiveAddressMapEntry(loco, train)
            );
        }

        private void AddOnShelfLocomotivesToAddressMap(LocomotiveAddressMap addressMap, IModelComponentIsCommandStation commandStation) {
            TrackGauges gauges = 0;
            var trackConnectors = from trackPowerConnector in LayoutModel.Components<LayoutTrackPowerConnectorComponent>(LayoutModel.ActivePhases) where trackPowerConnector.Inlet.ConnectedOutlet.ObtainablePowers.Any(power => power.PowerOriginComponentId == commandStation.Id) select trackPowerConnector;

            gauges = trackConnectors.Aggregate<LayoutTrackPowerConnectorComponent, TrackGauges>(0, (g, powerConnector) => g | powerConnector.Info.TrackGauge);

            // Pass on all locomotives in collection
            foreach (XmlElement locomotiveElement in LayoutModel.LocomotiveCollection.CollectionElement.GetElementsByTagName(E_Locomotive)) {
                LocomotiveInfo locomotive = new(locomotiveElement);
                int locomotiveAddress = locomotive.AddressProvider.Unit;

                if (locomotiveAddress >= 0 && (locomotive.Guage & gauges) != 0 && !addressMap.ContainsKey(locomotiveAddress))
                    addressMap.Add(locomotiveAddress, new OnShelfLocomotiveAddressMapEntry(locomotive));
            }
        }

        private readonly Dictionary<IModelComponentIsCommandStation, OnTrackLocomotiveAddressMap> addressMapCache = new();

        [DispatchTarget]
        private OnTrackLocomotiveAddressMap GetOnTrackLocomotiveAddressMap(object commandStationObject) {
            var commandStation = ExtractCommandStation(commandStationObject);
            OnTrackLocomotiveAddressMap? addressMap;

            if (commandStation != null) {
                if (!addressMapCache.TryGetValue(commandStation, out addressMap)) {
                    addressMap = new OnTrackLocomotiveAddressMap();

                    AddOnPoweredTracksLocomotivesToAddressMap(addressMap, commandStation);
                    addressMapCache.Add(commandStation, addressMap);
                }
            }
            else
                addressMap = new OnTrackLocomotiveAddressMap();

            return addressMap;
        }

        // Trap events that invalidate the locomotive address map cache
        //[LayoutEvent("train-is-removed")]
        [DispatchTarget(Name = "OnTrainPlacedOnTrack")]
        //[LayoutEvent("locomotive-power-changed")]
        private void InvalidateAddressMap(TrainStateInfo train) {
            addressMapCache.Clear();
        }

        [DispatchTarget]
        private int? AllocateLocomotiveAddress(LocomotiveInfo locomotive) {
            if (locomotive.DecoderType is DecoderWithNumericAddressTypeInfo) {
                // Build address map of used addresses
                LocomotiveAddressMap addressMap = new();
                var commandStations = LayoutModel.Components<IModelComponentIsCommandStation>(LayoutModel.ActivePhases);

                // If there is only one command station in the layout (very probable)
                var commandStation = Ensure.NotNull<IModelComponentIsCommandStation>(commandStations.SingleOrDefault());

                foreach (var c in commandStations)
                    BuildAddressMap(c, addressMap);

                // At that stage the address map is complete
                int minAddress = locomotive.GetLowestAddress(commandStation);
                int maxAddress = locomotive.GetHighestAddress(commandStation);
                int numberOfAddresses = maxAddress - minAddress + 1;

                int address = minAddress;
                int bestOnNonPowerTrackAddress = -1;            // Address to use if need to reuse an address of a locomotive on a non-powered track
                int bestOnShelfAddress = -1;                    // Address to use if need to reuse an address of a locomotive that is not on track

                for (int count = 0; count < numberOfAddresses; count++) {
                    if (!addressMap.TryGetValue(address, out LocomotiveAddressMapEntryBase? addressMapEntry)) {
                        // Found unused address - this is the best
                        return address;
                    }
                    else if (addressMapEntry is not LocomotiveBusModuleAddressMapEntry && addressMapEntry is not OnPoweredTrackLocomotiveAddressMapEntry) {
                        if (addressMapEntry is LocomotiveAddressMapEntry locomotiveAddressMapEntry) {
                            if (locomotiveAddressMapEntry is OnDisconnectedTrackLocomotiveAddressMapEntry)
                                bestOnNonPowerTrackAddress = address;
                            else if (locomotiveAddressMapEntry is OnShelfLocomotiveAddressMapEntry) {
                                // TODO: The best one is the one that was placed on the shelf for the longest time...
                                bestOnShelfAddress = address;
                            }
                        }
                    }

                    if (++address == maxAddress)
                        address = minAddress;
                }

                // Got to here, need to reuse an address
                if (bestOnShelfAddress >= 0)
                    address = bestOnShelfAddress;
                else if (bestOnNonPowerTrackAddress >= 0)
                    address = bestOnNonPowerTrackAddress;

                return address;
            }
            else
                return null;
        }

        private void BuildAddressMap(IModelComponentIsCommandStation commandStation, LocomotiveAddressMap addressMap) {
            AddOnPoweredTracksLocomotivesToAddressMap(addressMap, commandStation);
            Dispatch.Call.AddCommandStationLocomotiveBusToAddressMap(commandStation, addressMap);
            AddOnNonPoweredTracksLocomotivesToAddressMap(addressMap);
            AddOnShelfLocomotivesToAddressMap(addressMap, commandStation);
        }

        #endregion

        #region Locomotive placement/removal handlers

        /// <summary>
        /// Extract a provided locomotive trainState. The trainState can be either directly or indirectly provided.
        /// For example, the trainState can be the one of a given locomotive or locomotive set. An exception
        /// is thrown if trainState of an object which is not on the track is requested.
        /// </summary>
        [DispatchTarget]
        private TrainStateInfo ExtractTrainState(object trainObject) => trainObject switch {
            TrainStateInfo train => train,
            TrainCommonInfo trainCommonInfo => LayoutModel.StateManager.Trains[trainCommonInfo.Id] ?? throw new TrainNotOnTrackException(trainCommonInfo),
            XmlElement element => element.Name switch {
                E_TrainState => new TrainStateInfo(element),
                E_Locomotive => ExtractTrainState(new LocomotiveInfo(element)),
                _ => throw new ArgumentException("Invalid XML element (not LocomotiveState, Locomotive or Train)"),
            },
            LocomotiveInfo loco => LayoutModel.StateManager.Trains[loco.Id] ?? throw new LocomotiveNotOnTrackException(loco),
            _ => throw new ArgumentException("Invalid argument (Not XmlElement, LocomotiveInfo, TrainCommonInfo, TrainInCollectionInfo or TrainStateInfo)"),
        };

        /// <summary>
        /// Remove an object (locomotive or locomotive set) from track
        /// </summary>
        [DispatchTarget]
        private async Task RemoveFromTrackRequest(TrainStateInfo train, string balloonText, LayoutOperationContext operationContext) {
            if (balloonText != null) {
                var balloon = new LayoutBlockBalloon() { Text = balloonText, RemoveOnClick = true, CancellationToken = operationContext.CancellationToken };

                if (train.LocomotiveBlock != null) {
                    LayoutBlockBalloon.Show(train.LocomotiveBlock.BlockDefinintion, balloon);
                    await balloon.Task;
                }
            }

            LayoutModel.StateManager.Trains.Remove(train);
        }

        private async Task WaitForPlacement(TrainStateInfo train, LayoutBlockDefinitionComponent blockDefinition, string message, LayoutOperationContext operationContext) {

            var placeTrainBalloon = new LayoutBlockBalloon {
                Text = message,
                CancellationToken = operationContext.CancellationToken,
                FillColor = System.Drawing.Color.LightGreen,
                TextColor = System.Drawing.Color.Black,
                RemoveOnClick = true,
                RemoveOnTrainDetected = true
            };
            LayoutBlockBalloon.Show(blockDefinition, placeTrainBalloon);

            try {
                await placeTrainBalloon.Task;
            }
            catch (OperationCanceledException) {
                LayoutModel.StateManager.Trains.RemoveTrainState(train);
                Dispatch.Call.FreeOwnedLayoutLocks(train.Id, true);
                throw;
            }
        }
        
        /// <summary>
        /// Create train (runtime state) from locomotive or train element in the collection 
        /// </summary>
        /// <remarks>
        /// The event Train/Front option is set to contain the train's front. This will be needed by the place-train-on-block event handler
        /// </remarks>
        [DispatchTarget]
        private TrainStateInfo CreateTrain(LayoutBlockDefinitionComponent blockDefinition, XmlElement collectionElement, CreateTrainSettings options) {
            string? trainName = null;
            TrainLength trainLength = TrainLength.Standard;

            if(options.Front == null) {
                if (collectionElement.Name == E_Train) {
                    options.Front = Dispatch.Call.GetLocomotiveFront(blockDefinition, collectionElement);

                    if (options.Front == null)
                        throw new OperationCanceledException("get-locomotive-front");

                    TrainInCollectionInfo trainInCollection = new(collectionElement);

                    trainLength = trainInCollection.Length;
                    trainName = trainInCollection.Name;
                }
                else {
                    var t = Dispatch.Call.GetTrainFrontAndLength(blockDefinition, collectionElement);

                    if (t == null)
                        throw new OperationCanceledException("get-train-front-and-length");         // User canceled.

                    options.Front = t.Front;
                    trainLength = t.Length;
                    trainName = new LocomotiveInfo(collectionElement).Name;
                }
            }

            // Figure out the train name
            trainName = options.TrainName ?? trainName;

            if (trainName == null) {
                trainName = collectionElement.Name switch
                {
                    E_Locomotive => new LocomotiveInfo(collectionElement).Name,
                    E_Train => new TrainInCollectionInfo(collectionElement).Name,
                    _ => throw new LayoutException(EventManager.Instance, "No name was given to the new train"),
                };
            }

            Guid trainID = Guid.Empty;

            if (collectionElement.Name == E_Train) {
                TrainInCollectionInfo trainInCollection = new(collectionElement);

                trainID = trainInCollection.Id;
            }

            TrainStateInfo train = LayoutModel.StateManager.Trains.Add(trainID);

            train.Name = trainName;
            train.Length = trainLength;

            if (collectionElement != null)
                train.AddLocomotiveCollectionElement(collectionElement, blockDefinition.Block, options.ValidateAddress);

            return train;
        }

        private void PlaceTrainInBlock(TrainStateInfo train, LayoutBlockDefinitionComponent blockDefinition, LayoutComponentConnectionPoint front) {
            TrainLocationInfo trainLocation = train.PlaceInBlock(blockDefinition.Block, front);

            // Figure out a block edge which the train could have crossed.
            LayoutBlockEdgeBase[] origins = blockDefinition.GetBlockEdges(blockDefinition.GetOtherConnectionPointIndex(front));

            if (origins.Length == 0) {
                origins = blockDefinition.GetBlockEdges(blockDefinition.GetConnectionPointIndex(front));

                if (origins.Length > 0) {
                    trainLocation.BlockEdge = origins[0];
                    train.LastBlockEdgeCrossingSpeed = -1;      // Did reverse from block edge in the front
                }
            }
            else {
                trainLocation.BlockEdge = origins[0];
                train.LastBlockEdgeCrossingSpeed = 1;           // Positive speed coming from block edge in other direction than front
            }

            train.FlushCachedValues();
            train.RefreshSpeedLimit();
        }

        /// <summary>
        /// Create a new train. The train is initially empty, no locomotives and no cars
        /// </summary>
        /// <param name="e.Sender">A block definition component of the block where the train is to be created</param>
        /// <param name="e.Info">An optional locomotive collection element to initially add to the new train</param>
        [DispatchTarget]
        private async Task<TrainStateInfo> PlaceTrainRequest(LayoutBlockDefinitionComponent blockDefinition, XmlElement collectionElement, CreateTrainSettings options, LayoutOperationContext operationContext) {
            // Get lock on the block that the train is about to be placed, when the lock is obtained, the train is actually placed in the block
            LayoutLockRequest lockRequest;

            var train = Dispatch.Call.CreateTrain(blockDefinition, collectionElement, options);

            if (blockDefinition.Block.LockRequest == null || !blockDefinition.Block.LockRequest.IsManualDispatchLock) {
                lockRequest = new LayoutLockRequest(train.Id) {
                    CancellationToken = operationContext.CancellationToken
                };
                lockRequest.Blocks.Add(blockDefinition.Block);

                Task lockTask = Dispatch.Call.RequestLayoutLockAsync(lockRequest);

                if (!lockTask.IsCompleted)
                    LayoutBlockBalloon.Show(blockDefinition,
                        new LayoutBlockBalloon() {
                            Text = "Do not place train on track yet!",
                            CancellationToken = operationContext.CancellationToken,
                            FillColor = System.Drawing.Color.Red,
                            TextColor = System.Drawing.Color.Yellow
                        });

                try {
                    await lockTask;
                }
                catch (OperationCanceledException) {
                    if (train != null)
                        LayoutModel.StateManager.Trains.RemoveTrainState(train);
                    throw;
                }
            }

            PlaceTrainInBlock(train, blockDefinition, options.Front ?? blockDefinition.Track.ConnectionPoints[0]);
            await WaitForPlacement(train, blockDefinition, $"Please place train {train.Name} on track", operationContext);

            // At this stage the place where the train is about to be placed is locked, and train is supposed to be on track (at least the user has indicated that by
            // clicking on the balloon, or a train was detected in this block)

            Dispatch.Notification.OnTrainCreated(train, blockDefinition);
            Dispatch.Notification.OnTrainPlacedOnTrack(train);

            return train;
        }

        [DispatchTarget]
        private async Task<TrainStateInfo> ValidateAndPlaceTrainRequest(LayoutBlockDefinitionComponent blockDefinition, XmlElement placedElement, CreateTrainSettings options, LayoutOperationContext operationContext) {
            TrainStateInfo? train;

            var result = Dispatch.Call.CanLocomotiveBePlaced(placedElement, blockDefinition);

            if (result.ResolveMethod == CanPlaceTrainResolveMethod.ReprogramAddress) {
                var programmingState = new PlacedLocomotiveProgrammingState(placedElement, result.Locomotive, blockDefinition);

                train = await PlacedLocomotiveAddressReprogramming(programmingState, options, operationContext);

                if (train == null)
                    result = Dispatch.Call.CanLocomotiveBePlaced(placedElement, blockDefinition);
            }

            if (result.ResolveMethod == CanPlaceTrainResolveMethod.Resolved) {
                train = await PlaceTrainRequest(blockDefinition, placedElement, options, operationContext);

                SetTrainInitialDirectionRequest(train);
            }
            else
                throw new LayoutException("Train/Locomotive cannot be placed");

            return train;
        }

        [DispatchTarget]
        private void RelocateTrainRequest(TrainStateInfo train, LayoutBlockDefinitionComponent blockDefinition, LayoutComponentConnectionPoint? front) {
            if (front == null) {
                front = Dispatch.Call.GetLocomotiveFront(blockDefinition, train);

                if (front == null)
                    return;         // User canceled.
            }

            train.EraseImage();

            var oldLocations = new List<TrainLocationInfo>(train.Locations);
            var blocksToUnlock = new List<LayoutBlock>(oldLocations.Count);

            foreach (TrainLocationInfo oldTrainLocation in oldLocations) {
                if (oldTrainLocation.Block.LockRequest != null && !oldTrainLocation.Block.LockRequest.IsManualDispatchLock)
                    blocksToUnlock.Add(oldTrainLocation.Block);
                train.LeaveBlock(oldTrainLocation.Block, false);
            }

            if (blocksToUnlock.Count > 0)
                Dispatch.Call.FreeLayoutLock(blocksToUnlock);

            // Get lock on the block that the train is about to be placed
            LayoutLockRequest lockRequest = new(train.Id);

            if (blockDefinition.Block.LockRequest == null || !blockDefinition.Block.LockRequest.IsManualDispatchLock) {
                lockRequest.Blocks.Add(blockDefinition.Block);
                Dispatch.Call.RequestLayoutLock(lockRequest);
            }

            // Create the new location for the train
            TrainLocationInfo trainLocation = train.PlaceInBlock(blockDefinition.Block, front.Value);

            trainLocation.DisplayFront = front.Value;
            train.LastCrossedBlockEdge = null;

            Dispatch.Notification.OnTrainRelocated(train, blockDefinition);

            // Figure out a block edge which the train could have crossed.
            LayoutBlockEdgeBase[] origins = blockDefinition.GetBlockEdges(blockDefinition.GetOtherConnectionPointIndex(front.Value));

            if (origins.Length == 0) {
                origins = blockDefinition.GetBlockEdges(blockDefinition.GetConnectionPointIndex(front.Value));

                if (origins.Length > 0) {
                    trainLocation.BlockEdge = origins[0];
                    train.LastBlockEdgeCrossingSpeed = -1;      // Did reverse from block edge in the front
                }
            }
            else {
                trainLocation.BlockEdge = origins[0];
                train.LastBlockEdgeCrossingSpeed = 1;           // Positive speed coming from block edge in other direction than front
            }

            train.FlushCachedValues();
            train.RefreshSpeedLimit();

            RequestAutoExtendTrain(train, blockDefinition);
        }

        #endregion

        #region Locomotive address reprogramming handling

        [DispatchTarget]
        private async Task<TrainStateInfo?> PlacedLocomotiveAddressReprogramming(PlacedLocomotiveProgrammingState programmingState, CreateTrainSettings settings, LayoutOperationContext operationContext) {
            var address = AllocateLocomotiveAddress(programmingState.Locomotive);

            if (address.HasValue) {
                programmingState.ProgrammingActions = new LayoutActionContainer<LocomotiveInfo>(programmingState.Locomotive);

                var changeAddressAction = (ILayoutLocomotiveAddressChangeAction?)programmingState.ProgrammingActions.Add("set-address");

                if (changeAddressAction != null) {
                    changeAddressAction.Address = address.Value;
                    changeAddressAction.SpeedSteps = programmingState.Locomotive.SpeedSteps;
                    return await ProgramLocomotive(programmingState, settings, operationContext);
                }
                else
                    throw new LayoutException("Unable to obtain locomotive programming action");
            }
            else
                throw new LayoutException("Unable to find a valid new address for locomotive '" + programmingState.Locomotive.Name);
        }

        #endregion

        #region Locomotive Programming

        private enum CanUseForProgrammingResult {
            No, Yes, YesButHasOtherTrains
        }

        private CanUseForProgrammingResult CanUseForProgramming(LayoutBlockDefinitionComponent? blockDefinition, LocomotiveInfo? targetLocomotive = null) {
            if (blockDefinition != null && blockDefinition.PowerConnector.Inlet.ConnectedOutlet.ObtainablePowers.Any(p => p.Type == LayoutPowerType.Programmer)) {
                if (blockDefinition.PowerConnector.Locomotives.Any()) {
                    // Region connected to the power has locomotives, if it has only one and this is the one about to get programmed that ok,
                    // otherwise, return that this block can be used for programming, but it contains other trains.
                    return blockDefinition.PowerConnector.Locomotives.All(trainLocomotive => targetLocomotive != null && trainLocomotive.LocomotiveId == targetLocomotive.Id)
                        ? CanUseForProgrammingResult.Yes
                        : CanUseForProgrammingResult.YesButHasOtherTrains;
                }
                else
                    return CanUseForProgrammingResult.Yes;  // Can get power, and has no locomotives
            }
            else
                return CanUseForProgrammingResult.No;       // Block cannot get programming power.
        }

        private static Task ObtainProgrammingTracksLock(TrainStateInfo train, LayoutBlockDefinitionComponent programmingLocation, string balloonText, CancellationToken cancellationToken) {
            // Lock the blocks that get power from the inlet that the programming location gets its power from
            var lockRequest = GetProgrammingTracksLock(train.Id, programmingLocation, cancellationToken);

            var lockTask = Dispatch.Call.RequestLayoutLockAsync(lockRequest);

            if (!lockTask.IsCompleted) {
                var waitLockBalloon = new LayoutBlockBalloon() { Text = balloonText, CancellationToken = cancellationToken, FillColor = System.Drawing.Color.Red, TextColor = System.Drawing.Color.Yellow };

                LayoutBlockBalloon.Show(programmingLocation, waitLockBalloon);
            }

            // If block is locked to this train, but with a different lock request, release the old lock
            if (programmingLocation.Block.LockRequest != null && programmingLocation.Block.LockRequest != lockRequest && programmingLocation.Block.LockRequest.OwnerId == train.Id)
                Dispatch.Call.FreeOwnedLayoutLocks(train.Id, false);

            return lockTask;
        }

        private static LayoutLockRequest GetProgrammingTracksLock(Guid ownerId, LayoutBlockDefinitionComponent programmingLocation, CancellationToken canellationToken) {
            var lockRequest = new LayoutLockRequest(ownerId) {
                Type = LayoutLockType.Programming,
                CancellationToken = canellationToken
            };
            lockRequest.Blocks.Add(programmingLocation.PowerConnector.Blocks);

            if ((
                from power in programmingLocation.PowerConnector.Inlet.ConnectedOutlet.ObtainablePowers 
                where power.Type == LayoutPowerType.Programmer
                select power.PowerOriginComponent).FirstOrDefault() is not ILayoutLockResource commandStationResource
            )
                throw new LayoutException($"GetProgrammingTrackLock for {programmingLocation.FullDescription} - command station is null");

            lockRequest.Resources = new ILayoutLockResource[] { commandStationResource };
            return lockRequest;
        }

        [DispatchTarget]
        private async Task<TrainStateInfo?> ProgramLocomotive(LocomotiveProgrammingState programmingState, CreateTrainSettings settings, LayoutOperationContext operationContext) {
            TrainStateInfo? train = LayoutModel.StateManager.Trains[programmingState.Locomotive.Id];
            bool usePOM = true;

            if (train == null || !train.OnTrack)
                usePOM = false;             // Train not on track - no program on main
            else if (programmingState.ProgrammingActions.IsProgrammingTrackRequired())
                usePOM = false;

            if (!usePOM && programmingState.ProgrammingActions.IsTrackRequired()) { // Need to be placed on track with programming power
                                                                                    // Check if preferred placement location can be used for programming
                CanUseForProgrammingResult canUseForProgrammingResult = CanUseForProgramming(programmingState.PlacementLocation, programmingState.Locomotive);

                if (canUseForProgrammingResult == CanUseForProgrammingResult.Yes)
                    programmingState.ProgrammingLocation = programmingState.PlacementLocation;
                else
                    programmingState.ProgrammingLocation = Dispatch.Call.GetProgrammingLocation();   // Ask the user where should the programming take place

                if (programmingState.ProgrammingLocation == null)
                    throw new OperationCanceledException(nameof(ProgramLocomotive));

                LayoutBlockDefinitionComponent programmingLocation = programmingState.ProgrammingLocation;
                ILayoutPower originalPower = programmingLocation.Power;

                if (train != null && (train.LocomotiveBlock == null || programmingLocation.Block.Id != train.LocomotiveBlock.Id)) {
                    await RemoveFromTrackRequest(train, "Remove locomotive to be programmed from track (and place it on programming track)", operationContext);
                    train = null;
                }

                try {
                    await Dispatch.Call.SetPower(programmingLocation, LayoutPowerType.Programmer, operationContext);

                    if (train == null) {
                        if (programmingState.PlacementLocation == null || programmingLocation.Id != programmingState.PlacementLocation.Id) {
                            settings.Front = programmingLocation.Track.ConnectionPoints[0];
                            settings.Length = TrainLength.LocomotiveOnly;
                            operationContext.AddPariticpant(programmingLocation);
                        }

                        train = CreateTrain(programmingLocation, programmingState.Locomotive.Element, settings);
                        await ObtainProgrammingTracksLock(train, programmingLocation, "Do not yet place locomotive on programming track", operationContext.CancellationToken);
                        PlaceTrainInBlock(train, programmingLocation, programmingLocation.Track.ConnectionPoints[0]);
                    }
                    else
                        await ObtainProgrammingTracksLock(train, programmingLocation, "Waiting to obtain lock on programming track region", operationContext.CancellationToken);

                    // Ask the user to put the locomotive on track (note that lockRequest is null, since the track is already locked)
                    await WaitForPlacement(train, programmingLocation, "Place locomotive on programming track", operationContext);

                    // When track is detected or when the user hit the balloon, wait 500ms and then start the actual programming
                    await Task.Delay(500, operationContext.CancellationToken);

                    var commandStation = programmingLocation.Power.PowerOriginComponent as IModelComponentCanProgramLocomotives;

                    if (commandStation != null) {
                        var r = await Dispatch.Call.DoCommandStationActions(commandStation, programmingState.ProgrammingActions, usePOM: false);

                        if (r != null)
                            Error(r.ToString());

                        // If programming location is not same as original location (or where the train is supposed to be), remove the train from the programming location
                        if (programmingState.PlacementLocation == null || programmingState.PlacementLocation.Id != programmingLocation.Id) {
                            await RemoveFromTrackRequest(train, "Remove locomotive from programming track", operationContext);
                            train = null;
                        }

                        if (r != null)
                            throw new LayoutException("Locomotive programming failed");
                    }
                    else
                        Error(commandStation, "Is unable to program locomotive settings");

                }
                finally {
                    if (originalPower != programmingLocation.Power) {
                        Trace.WriteLine("Set power back to " + originalPower.Name + " type: " + originalPower.Type.ToString());
                        Task _ = Dispatch.Call.SetPower(programmingLocation, originalPower, operationContext);
                    }

                    Trace.WriteLine("Free programming locked");

                    Dispatch.Call.FreeLayoutLock(GetProgrammingTracksLock(Guid.Empty, programmingLocation, CancellationToken.None));

                    if (programmingState.PlacementLocation == null || programmingLocation.Id != programmingState.PlacementLocation.Id)
                        operationContext.RemoveParticipant(programmingLocation);
                }
            }
            else {
                if (train?.CommandStation is IModelComponentCanProgramLocomotives commandStation) {
                    var r = await Dispatch.Call.DoCommandStationActions(commandStation, programmingState.ProgrammingActions, usePOM: true);

                    if (r != null)
                        Error(r.ToString());
                }
                else
                    Error($"Unable to program train");
            }

            return train;
        }

        #endregion

        #region Train Management

        private void AutoExtendTrain(TrainStateInfo train, LayoutBlock oldBlock, LayoutBlockEdgeBase blockEdge) {
            LayoutBlock newBlock = oldBlock.OtherBlock(blockEdge);

            if (newBlock.BlockDefinintion.Info.IsOccupancyDetectionBlock && !newBlock.HasTrains && (newBlock.BlockDefinintion.Info.TrainDetected || newBlock.BlockDefinintion.Info.TrainWillBeDetected) && !newBlock.IsLocked) {
                // Get lock on the block that the train is about to be placed
                LayoutLockRequest lockRequest = new(train.Id);

                lockRequest.Blocks.Add(newBlock);

                bool granted = Dispatch.Call.RequestLayoutLock(lockRequest);

                Debug.Assert(granted);

                var oldTrainLocation = train.LocationOfBlock(oldBlock);

                if (oldTrainLocation != null) {
                    TrainLocationInfo newTrainLocation;

                    train.LastBlockEdgeCrossingSpeed = 1;

                    if (oldBlock.BlockDefinintion.ContainsBlockEdge(oldTrainLocation.DisplayFront, blockEdge)) {
                        newTrainLocation = train.EnterBlock(TrainPart.Locomotive, newBlock, blockEdge, "train-extended");

                        train.LastCrossedBlockEdge = blockEdge;

                        // Extended train location front should point to the direction opposite to the one of the block edge
                        // and direction should be Backward
                        if (newBlock.BlockDefinintion.ContainsBlockEdge(0, blockEdge))
                            newTrainLocation.DisplayFront = newBlock.BlockDefinintion.Track.ConnectionPoints[1];
                        else
                            newTrainLocation.DisplayFront = newBlock.BlockDefinintion.Track.ConnectionPoints[0];
                    }
                    else {
                        newTrainLocation = train.EnterBlock(TrainPart.LastCar, newBlock, blockEdge, "train-extended");

                        // Extended train location front should point to the track contact direction and the track contact
                        // state direction should be forward
                        if (newBlock.BlockDefinintion.ContainsBlockEdge(0, blockEdge))
                            newTrainLocation.DisplayFront = newBlock.BlockDefinintion.Track.ConnectionPoints[0];
                        else
                            newTrainLocation.DisplayFront = newBlock.BlockDefinintion.Track.ConnectionPoints[1];

                        int iCpIndex = newBlock.BlockDefinintion.GetConnectionPointIndex(newTrainLocation.DisplayFront);
                        LayoutBlockEdgeBase[] otherBlockEdges = newBlock.BlockDefinintion.GetBlockEdges(1 - iCpIndex);

                        if (otherBlockEdges == null)
                            newTrainLocation.BlockEdge = null;
                        else
                            newTrainLocation.BlockEdge = otherBlockEdges[0];
                    }

                    foreach (LayoutBlockEdgeBase newBlockEdge in newBlock.BlockEdges)
                        AutoExtendTrain(train, newBlock, newBlockEdge);
                }
            }
        }

        [DispatchTarget]
        private void RequestAutoExtendTrain(TrainStateInfo train, LayoutBlockDefinitionComponent blockDefinition) {
            if (blockDefinition.Info.IsOccupancyDetectionBlock) {
                // Automatically extend train to neighboring occupancy detection blocks in which train is detected
                foreach (LayoutBlockEdgeBase blockEdge in blockDefinition.Block.BlockEdges)
                    AutoExtendTrain(train, blockDefinition.Block, blockEdge);
            }
        }

        #endregion

        #region Locomotive speed and direction handlers

        [DispatchTarget]
        private IModelComponentHasNameAndId GetCommandStation(object commandStationObject) {
            return commandStationObject switch {
                IModelComponentIsCommandStation commandStation => commandStation,
                TrainStateInfo train => train.CommandStation ?? throw new LayoutException($"Train {train.Name} has no associated command station (is it on the track?)"),
                ControlBus bus => bus.BusProvider,
                ControlConnectionPointReference connectionPointRef => connectionPointRef.Module.Bus.BusProvider,
                LocomotiveInfo loco => GetCommandStation(Ensure.NotNull<TrainStateInfo>(LayoutModel.StateManager.Trains[loco.Id])),
                _ => throw new LayoutException("Cannot figure out which command station to use")
            };
        }

        /// <summary>
        /// Event which is sent to request a change in a locomotive motion properties (speed and direction)
        /// </summary>
        /// <param name="e.Sender">Indicate the locomotive or locomotive set. It can be either locomotiveState
        /// provider object or element, Locomotive provider object or element or Train object
        /// or element
        /// </param>
        /// <param name="speed">The speed. Positive value for forward motion, Negative value for backward motion
        /// </param>
        [DispatchTarget]
        private void SetLocomotiveSpeedRequest(TrainStateInfo train, int speed) {
            var commandStation = GetCommandStation(train);

            foreach (TrainLocomotiveInfo trainLoco in train.Locomotives)
                Dispatch.Call.LocomotiveMotionCommand(commandStation, trainLoco.Locomotive, (trainLoco.Orientation == LocomotiveOrientation.Forward) ? speed : -speed);

            train.SetSpeedValue(speed);
        }

        /// <summary>
        /// Event which is sent to reverse the train motion direction.
        /// </summary>
        /// <param name="trainObject">The train to be reversed</param>
        [DispatchTarget]
        private void ReverseTrainMotionDirectionRequest(object trainObject) {
            var train = ExtractTrainState(trainObject);
            var commandStation = GetCommandStation(train);

            foreach (TrainLocomotiveInfo trainLoco in train.Locomotives)
                Dispatch.Call.LocomotiveReverseMotionDirectionCommand(commandStation, trainLoco.Locomotive);
        }

        private void SetTrainInitialDirectionRequest(TrainStateInfo train) {
            var commandStation = GetCommandStation(train);

            foreach (TrainLocomotiveInfo trainLoco in train.Locomotives) {
                if (trainLoco.Orientation == LocomotiveOrientation.Backward)
                    Dispatch.Call.LocomotiveReverseMotionDirectionCommand(commandStation, trainLoco.Locomotive);
            }

            train.MotionDirection = LocomotiveOrientation.Forward;
        }

        /// <summary>
        /// This event is sent by the command station component when it get an indication of a change
        /// in a locomotive motion parameters
        /// </summary>
        /// <param name="commandStation">The command station component</param>
        /// <param name="spped">The speed</param>
        /// <param name="unit">The unit (locomotive address)</param>
        /// The event document has a element named Address with an attribute Unit which indicate the
        /// locomotive address whose address is being changed
        [DispatchTarget]
        private void OnLocomotiveMotion(IModelComponentIsCommandStation commandStation, int speed, int unit) {
            var addressMap = GetOnTrackLocomotiveAddressMap(commandStation);
            var addressMapEntry = addressMap[unit];

            if (addressMapEntry == null)
                Error($"Command station {commandStation.NameProvider.Name} reported motion of locomotive with address {unit}. This locomotive is not registered as being on the layout!");
            else {
                if (addressMapEntry.Train.Locomotives.Count > 1)
                    Warning($"Command station {commandStation.NameProvider.Name} reported change in speed/direction of a single locomotive set member (address {unit})");

                addressMapEntry.Train.SetSpeedValue(speed);
            }
        }

        [DispatchTarget]
        private void OnTrainSpeedChanged(TrainStateInfo train, int speed) {
            foreach (TrainLocationInfo trainLocation in train.Locations) {
                LayoutBlock block = trainLocation.Block;

                if (block.BlockDefinintion != null)
                    block.BlockDefinintion.OnComponentChanged();
            }
        }

        #endregion

        #region Lights & Functions

        [DispatchTarget]
        private void SetLocomotiveLightsRequest(TrainStateInfo train, bool lights) {

            if (train.CommandStation != null && lights != train.Lights) {
                foreach (TrainLocomotiveInfo trainLoco in train.Locomotives) {
                    LocomotiveInfo loco = trainLoco.Locomotive;

                    train.SetLightsValue(lights);

                    if (loco.HasLights)
                        Dispatch.Call.SetLocomotiveLightCommand(train.CommandStation, loco, lights);
                }
            }
        }

        [DispatchTarget]
        private void OnLocomotiveLightsToggled(IModelComponentHasNameAndId commandStation, int unit) {
            var addressMap = GetOnTrackLocomotiveAddressMap(commandStation);
            var addressMapEntry = addressMap[unit];

            if (addressMapEntry == null)
                Error($"Command station {commandStation.NameProvider.Name} reported light status of locomotive with address {unit}. This locomotive is not registered as being on the layout!");
            else {
                TrainStateInfo train = addressMapEntry.Train;

                if (train.Locomotives.Count > 1)
                    Warning($"Command station {commandStation.NameProvider.Name} reported change in light status of a single locomotive set member (address {unit})");

                train.SetLightsValue(!train.Lights);
            }
        }

        [DispatchTarget]
        private void OnLocomotiveLightsChanged(IModelComponentHasNameAndId commandStation, int unit, bool lights) {
            var addressMap = GetOnTrackLocomotiveAddressMap(commandStation);
            var addressMapEntry = addressMap[unit];

            if (addressMapEntry == null)
                Error($"Command station {commandStation.NameProvider.Name} reported light status of locomotive with address {unit}. This locomotive is not registered as being on the layout!");
            else {
                TrainStateInfo train = addressMapEntry.Train;

                if (train.Locomotives.Count > 1)
                    Warning($"Command station {commandStation.NameProvider.Name} reported change in light status of a single locomotive set member (address {unit})");

                train.SetLightsValue(lights);
            }
        }

        [DispatchTarget]
        private void SetLocomotiveFunctionStateRequest(TrainStateInfo train, Guid locomotiveId, string functionName, bool functionState) {
            var commandStation = GetCommandStation(train);

            if (locomotiveId == Guid.Empty || functionState != train.GetFunctionState(functionName, locomotiveId, false)) {
                foreach (TrainLocomotiveInfo trainLoco in train.Locomotives) {
                    LocomotiveInfo loco = trainLoco.Locomotive;

                    if (locomotiveId == Guid.Empty || locomotiveId == loco.Id) {
                        train.SetLocomotiveFunctionStateValue(functionName, locomotiveId, functionState);
                        Dispatch.Call.SetLocomotiveFunctionStateCommand(commandStation, loco, functionName, functionState);

                        if (locomotiveId != Guid.Empty)
                            break;
                    }
                }
            }
        }

        [DispatchTarget]
        private void TriggerLocomotiveFunctionStateRequest(TrainStateInfo train, Guid locomotiveId, string functionName) {
            var commandStation = GetCommandStation(train);

            foreach (TrainLocomotiveInfo trainLoco in train.Locomotives) {
                LocomotiveInfo loco = trainLoco.Locomotive;

                if (locomotiveId == Guid.Empty || locomotiveId == loco.Id) {
                    bool state = !train.GetFunctionState(functionName, loco.Id);        // Flip state

                    Dispatch.Call.TriggerLocomotiveFunctionStateCommand(commandStation, loco, functionName, state);
                    train.SetLocomotiveFunctionStateValue(functionName, loco.Id, state);
                }
            }
        }

        [DispatchTarget]
        private void OnLocomotiveFunctionStateChanged(IModelComponentHasNameAndId commandStation, int unit, int functionNumber, bool functionState) {
            var addressMap = GetOnTrackLocomotiveAddressMap(commandStation);
            var addressMapEntry = addressMap[unit];

            if (addressMapEntry == null)
                Error($"Command station {commandStation.NameProvider.Name} reported light status of locomotive with address {unit}. This locomotive is not registered as being on the layout!");
            else {
                TrainStateInfo train = addressMapEntry.Train;

                foreach (TrainLocomotiveInfo trainLoco in train.Locomotives) {
                    if (trainLoco.Locomotive.AddressProvider.Unit == unit) {
                        train.SetLocomotiveFunctionStateValue(functionNumber, trainLoco.LocomotiveId, functionState);
                        break;
                    }
                }
            }
        }

        #endregion

        #region Speed limit

        [DispatchTarget]
        private void OnLocomotiveConfigurationChanged(LocomotiveInfo loco) {
            if (LayoutController.IsOperationMode) {
                foreach (XmlElement trainElement in LayoutModel.StateManager.Trains.Element) {
                    TrainStateInfo train = new(trainElement);

                    foreach (TrainLocomotiveInfo trainLocomotive in train.Locomotives) {
                        if (trainLocomotive.LocomotiveId == loco.Id)
                            train.FlushCachedValues();
                    }
                }
            }

            foreach (XmlElement trainElement in LayoutModel.LocomotiveCollection.CollectionElement.GetElementsByTagName(E_Train)) {
                TrainStateInfo train = new(trainElement);

                foreach (TrainLocomotiveInfo trainLocomotive in train.Locomotives) {
                    if (trainLocomotive.LocomotiveId == loco.Id)
                        train.FlushCachedValues();
                }
            }
        }

        #endregion

        #region Train controller handling

        [DispatchTarget]
        private void OnTrainControllerActivated(TrainStateInfo train) {
            foreach (var trainLoco in train.Locomotives)
                Dispatch.Notification.OnLocomotiveControllerActivated(train, trainLoco);
        }

        [DispatchTarget]
        private void OnTrainControllerDeactivated(TrainStateInfo train) {
            foreach (var trainLoco in train.Locomotives)
                Dispatch.Notification.OnLocomotiveControllerDeactivated(train, trainLoco);
        }

        #endregion

        #region State reconstruction operations

        private class TrackContactCrossTimeSorter : IComparer {
            public int Compare(object? oLocation1, object? oLocation2) {
                var l1 = Ensure.NotNull<TrainLocationInfo>(oLocation1);
                var l2 = Ensure.NotNull<TrainLocationInfo>(oLocation2);

                if (l1.BlockEdgeCrossingTime > l2.BlockEdgeCrossingTime)
                    return 1;
                else return l1.BlockEdgeCrossingTime < l2.BlockEdgeCrossingTime ? -1 : 0;
            }
        }

        [DispatchTarget(Order = 0)]
        private bool RebuildLayoutState_Components(LayoutPhase phase) {
            return RebuildComponentsState(phase);
        }

        [DispatchTarget(Order = 1000)]
        private bool RebuildLayoutState(LayoutPhase phase) {
            bool ok = true;

            try {
                if (!RebuildTrainState(phase))
                    ok = false;

                if (!VerifyTrackContactState(phase))
                    ok = false;

                if (!ClearTrainDetectionBlocks(phase))
                    ok = false;
            }
            catch (Exception ex) {
                Warning("Problem when restoring layout state: " + ex.Message);
                ok = false;
            }

            return ok;
        }

        /// <summary>
        /// Check that the train state is valid. Check that each referred block, and
        /// referred locomotive (or locomotive set) can actually be accessed. Invalid trainState
        /// entries are removed. Also all locomotives are "placed" again, so block locomotive
        /// array and locomotive cound are corrected.
        /// </summary>
        private bool RebuildTrainState(LayoutPhase phase) {
            ArrayList invalidStateElements = new();
            ArrayList noTrackContactCrossTrainLocations = new();
            ArrayList trainLocations = new();

            // Remove all locomotives from all blocks
            foreach (LayoutBlock block in LayoutModel.Blocks)
                block.ClearTrains();

            foreach (XmlElement trainStateElement in LayoutModel.StateManager.Trains.Element) {
                bool validTrainState = true;
                TrainStateInfo trainState = new(trainStateElement);

                foreach (TrainLocomotiveInfo trainLoco in trainState.Locomotives) {
                    if (trainLoco.Locomotive == null && LayoutModel.LocomotiveCollection[trainLoco.CollectionElementId] == null) {
                        // Locomotive not found
                        Warning("Locomotive " + trainState.Name + " cannot be found in your locomotive collection, it has been removed from track");
                        invalidStateElements.Add(trainStateElement);
                        validTrainState = false;
                    }
                }

                if (validTrainState) {
                    foreach (TrainLocationInfo trainLocation in trainState.Locations) {
                        if (trainLocation.Block == null || trainLocation.BlockEdge == null) {
                            Warning("The location where " + trainState.DisplayName + " used to be cannot be found. Please indicate its position again (or remove it from track)");
                            invalidStateElements.Add(trainStateElement);
                            validTrainState = false;
                        }
                    }
                }

                if (validTrainState) {
                    foreach (TrainLocationInfo trainLocation in trainState.Locations) {
                        if (trainLocation.BlockEdgeId == Guid.Empty)
                            noTrackContactCrossTrainLocations.Add(trainLocation);
                        else
                            trainLocations.Add(trainLocation);
                    }
                }
            }

            foreach (XmlElement invalidStateElement in invalidStateElements) {
                if (invalidStateElement.ParentNode != null)
                    invalidStateElement.ParentNode.RemoveChild(invalidStateElement);
            }

            // Now place all the locomotives that should be placed. In order to ensure correct placement, sort the train locations
            // in ascending order based on track contact crossing time. locations which were placed will be placed first
            foreach (TrainLocationInfo trainLocation in noTrackContactCrossTrainLocations) {
                // Get lock on the block that the train is about to be placed
                LayoutLockRequest lockRequest = new(trainLocation.Train.Id);

                lockRequest.Blocks.Add(trainLocation.Block);
                Dispatch.Call.RequestLayoutLock(lockRequest);

                trainLocation.Block.AddTrain(trainLocation);
            }

            trainLocations.Sort(new TrackContactCrossTimeSorter());

            foreach (TrainLocationInfo trainLocation in trainLocations) {
                trainLocation.Block.AddTrain(trainLocation);

                if (trainLocation.Block.BlockDefinintion != null)
                    trainLocation.Train.RefreshSpeedLimit();
            }

            LayoutModel.StateManager.Trains.RebuildIdMap();

            foreach (TrainLocationInfo trainLocation in trainLocations) {
                LayoutLockRequest lockRequest = new(trainLocation.Train.Id);

                lockRequest.Blocks.Add(trainLocation.Block);
                Dispatch.Call.RequestLayoutLock(lockRequest);
            }

            return invalidStateElements.Count == 0;   // Fail if any invaid element was found
        }

        [DispatchTarget(Order =2000)]
        private void OnEnteredOperationMode(OperationModeParameters settings) {
            foreach (XmlElement trainStateElement in LayoutModel.StateManager.Trains.Element) {
                TrainStateInfo train = new(trainStateElement);

                if (train.Speed != 0)
                    train.SpeedInSteps = 0;
            }
        }

        public bool RebuildComponentsState(LayoutPhase phase) {
            ArrayList removeList = new();

            LayoutModel.StateManager.Components.IdToComponentStateElement.Clear();

            foreach (XmlElement componentStateElement in LayoutModel.StateManager.Components.Element) {
                var componentID = (Guid)componentStateElement.AttributeValue(A_Id);
                var component = LayoutModel.Component<ModelComponent>(componentID, LayoutPhase.All);
                bool validStateComponent = true;
                var topicRemoveList = new List<XmlElement>();

                if (component != null) {
                    foreach (XmlElement componentStateTopicElement in componentStateElement) {
                        if(!Dispatch.Call.VerifyComponentStateTopic(componentStateTopicElement))
                            topicRemoveList.Add(componentStateTopicElement);
                    }

                    foreach (XmlElement topicElementToRemove in topicRemoveList)
                        componentStateElement.RemoveChild(topicElementToRemove);

                    if (componentStateElement.ChildNodes.Count == 0)        // No topics, remove it
                        validStateComponent = false;

                    if (validStateComponent)
                        LayoutModel.StateManager.Components.IdToComponentStateElement.Add(componentID, componentStateElement);
                }
                else {
                    Warning("Component was removed from layout - its runtime state is ignored");
                    validStateComponent = false;
                }

                if (!validStateComponent)
                    removeList.Add(componentStateElement);
            }

            foreach (XmlElement componentStateElement in removeList)
                LayoutModel.StateManager.Components.Element.RemoveChild(componentStateElement);

            return removeList.Count == 0;
        }

        private bool VerifyTrackContactState(LayoutPhase phase) {
            bool ok = true;

            foreach (LayoutTrackContactComponent trackContact in LayoutModel.Components<LayoutTrackContactComponent>(phase)) {
                if (LayoutModel.StateManager.Components.Contains(trackContact, "TrainPassing")) {
                    var trackContactPassingElement = LayoutModel.StateManager.Components.StateOf(trackContact, "TrainPassing");
                    TrackContactPassingStateInfo trackContactPassingState = new(trackContactPassingElement);

                    if (trackContactPassingState.Train == null) {
                        Warning(trackContactPassingState.TrackContact, "The train that was above this train contact cannot be found");
                        ok = false;
                    }
                }
            }

            return ok;
        }

        /// <summary>
        /// Remove all unexpected trains block. The command station will re-detect all trains which are standing
        /// on the tracks, so there is no point in remembering the previous state
        /// </summary>
        /// <returns>True if all is OK</returns>
        private bool ClearTrainDetectionBlocks(LayoutPhase phase) {
            foreach (LayoutBlockDefinitionComponent blockDefinition in LayoutModel.Components<LayoutBlockDefinitionComponent>(phase)) {
                if (blockDefinition.Info.IsOccupancyDetectionBlock) {
                    blockDefinition.Info.SetTrainWillBeDetected(false);
                    blockDefinition.Info.SetTrainDetected(false);
                }
            }

            return true;
        }

        [DispatchTarget]
        private bool VerifyComponentStateTopic_Balloon([DispatchFilter(Type = "XPath", Value = "Balloon")] XmlElement componentStateTopicElement) => false;

        /// <summary>
        /// Clear the locomotive trainState (removing all locomotives)
        /// </summary>
        [DispatchTarget]
        private void ClearLayoutState() {
            // Remove all locomotives from all blocks
            foreach (LayoutBlock block in LayoutModel.Blocks)
                block.ClearTrains();

            LayoutModel.StateManager.DocumentElement.RemoveAll();
        }

        #endregion
    }
}
