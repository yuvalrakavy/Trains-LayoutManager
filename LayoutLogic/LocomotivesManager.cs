using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using LayoutManager.Model;
using LayoutManager.Components;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

#nullable enable
#pragma warning disable IDE0051, IDE0060
namespace LayoutManager.Logic {
    [LayoutModule("Trains Manager", UserControl = false)]
    internal class TrainsManager : LayoutModuleBase {
        #region Internal properties and helper functions

        #endregion

        #region Locomotive action handlers

        [LayoutEvent("query-action", IfEvent = "LayoutEvent[Options/@Action='set-address']", SenderType = typeof(LocomotiveInfo))]
        private void querySetLocoAddress(LayoutEvent e0) {
            var e = (LayoutEventInfoResultValueType<IHasDecoder, bool, bool>)e0;

            if (e.Sender?.DecoderType is DccDecoderTypeInfo)
                e.Result = true;
        }

        [LayoutEvent("get-action", IfSender = "Action[@Type='set-address']", InfoType = typeof(LocomotiveInfo))]
        private void GetActionSetAddress(LayoutEvent e) {
            var actionElement = Ensure.NotNull<XmlElement>(e.Sender, "actionElement");

            if (e.Info is LocomotiveInfo locomotive) {
                if (locomotive.DecoderType is DccDecoderTypeInfo)
                    e.Info = new LayoutDccChangeLocomotiveAddressAction(actionElement, locomotive);
            }
        }

        private const string E_Train = "Train";
        private const string E_Locomotive = "Locomotive";
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
        /// Check if a given locomotive (locomotive set memebrs) have valid address for placement on a given.
        /// block If not
        /// an exception is thrown
        /// </summary>
        /// <param name="e.Sender">The Xml element of the placed locomotive/locomotive set</param>
        /// <param name="e.Info">
        /// The block (LayoutBlock) where the locomotive is to be placed or the power
        /// (LayoutPower), the power that is applied to the area where the locomotive will need to reside
        /// </param>
        [LayoutEvent("is-locomotive-address-valid")]
        private void isLocomotiveAddressValid(LayoutEvent e) {
            var placeableElement = Ensure.NotNull<XmlElement>(e.Sender, "placeableElement");
            TrainStateInfo? train = null;

            var power = e.Info switch
            {
                ILayoutPower thePower => thePower,
                LayoutBlock block => block.Power,
                LayoutBlockDefinitionComponent blockDefinition => blockDefinition.Block.Power,
                ILayoutPowerOutlet outlet => outlet.Power,
                TrainStateInfo aTrain => aTrain.LocomotiveBlock?.Power,
                _ => throw new ArgumentException("Invalid power argument")
            };

            if (power.Type == LayoutPowerType.Digital) {
                //
                // Locomotive-Set address checks (applied only if the placed object is a locomotive set)
                //
                // a) If there is more than one member assigned to an address, and not all the members
                //    that are assigned to a given address are in the same orientation, generate
                //    an error event.
                // b) (TODO) Check that all members have identical decoder setting.
                //
                if (placeableElement.Name == E_Train) {
                    TrainInCollectionInfo trainInCollection = new TrainInCollectionInfo(placeableElement);
                    IList<TrainLocomotiveInfo> locomotives = trainInCollection.Locomotives;

                    for (int i = 0; i < locomotives.Count; i++) {
                        LocomotiveInfo loco1 = locomotives[i].Locomotive;

                        if (loco1.AddressProvider.Element == null) {
                            e.Info = new CanPlaceTrainResult {
                                Status = CanPlaceTrainStatus.LocomotiveHasNoAddress,
                                Locomotive = loco1,
                                CanBeResolved = true,
                                ResolveMethod = CanPlaceTrainResolveMethod.ReprogramAddress
                            };
                            return;
                        }

                        if ((loco1.DecoderType.SupportedDigitalPowerFormats & power.DigitalFormats) == 0) {
                            e.Info = new CanPlaceTrainResult {
                                Locomotive = loco1,
                                Status = CanPlaceTrainStatus.LocomotiveDigitalFormatNotCompatible,
                                CanBeResolved = false,
                                ResolveMethod = CanPlaceTrainResolveMethod.NotPossible
                            };
                            return;
                        }

                        if (loco1.Kind == LocomotiveKind.SoundUnit)
                            continue;

                        for (int j = i + 1; j < locomotives.Count; j++) {
                            LocomotiveInfo loco2 = locomotives[j].Locomotive;

                            if (loco2.Kind == LocomotiveKind.SoundUnit)
                                continue;

                            if (loco1.AddressProvider.Unit == loco2.AddressProvider.Unit && locomotives[i].Orientation != locomotives[j].Orientation) {
                                e.Info = new CanPlaceTrainResult() {
                                    Status = CanPlaceTrainStatus.LocomotiveDuplicateAddress,
                                    Loco1 = loco1,
                                    Loco2 = loco2,
                                    CanBeResolved = true,
                                    ResolveMethod = CanPlaceTrainResolveMethod.ReprogramAddress
                                };
                                return;
                            }
                        }
                    }

                    // Got to this point, each member has a unit address, and no invalid duplicates
                    // within the locomotive set
                    var addressMap = EventManager.Event<object, object, OnTrackLocomotiveAddressMap>("get-on-track-locomotive-address-map", power)!;

                    foreach (TrainLocomotiveInfo trainLocomotive in locomotives) {
                        var addressMapEntry = addressMap[trainLocomotive.Locomotive.AddressProvider.Unit];

                        if (addressMapEntry != null) {
                            e.Info = new CanPlaceTrainResult() {
                                Status = CanPlaceTrainStatus.LocomotiveAddressAlreadyUsed,
                                Loco1 = addressMapEntry.Locomotive,
                                Train = addressMapEntry.Train,
                                CanBeResolved = true,
                                ResolveMethod = CanPlaceTrainResolveMethod.ReprogramAddress
                            };
                            return;
                        }
                    }
                }
                else if (placeableElement.Name == E_Locomotive) {
                    LocomotiveInfo loco = new LocomotiveInfo(placeableElement);

                    if (!loco.NotManaged) {
                        if (loco.AddressProvider.Element == null || loco.AddressProvider.Unit <= 0) {
                            e.Info = new CanPlaceTrainResult() {
                                Status = CanPlaceTrainStatus.LocomotiveHasNoAddress,
                                Locomotive = loco,
                                CanBeResolved = true,
                                ResolveMethod = CanPlaceTrainResolveMethod.ReprogramAddress
                            };
                            return;
                        }

                        var locoAddress = (int?)e.GetOption(Option_LocomotiveAddress) ?? loco.AddressProvider.Unit;
                        var addressMap = EventManager.Event<object, object, OnTrackLocomotiveAddressMap>("get-on-track-locomotive-address-map", power)!;
                        var addressMapEntry = addressMap[locoAddress];

                        // Check that the address is not already used.
                        if (addressMapEntry != null) {
                            bool validDuplicate = false;

                            if (train != null && addressMapEntry.Train.Id == train.Id) {
                                // Check that the locomotive in the train that uses the same address has the
                                // same orientation as the placed locomotive
                                if (loco.Kind != LocomotiveKind.SoundUnit) {
                                    LocomotiveOrientation orientation = LocomotiveOrientation.Forward;
                                    var orientationElement = new XmlElementWrapper(e.Element[E_Orientation]);

                                    if (orientationElement.OptionalElement != null)
                                        orientation = orientationElement.AttributeValue(A_Value).Enum<LocomotiveOrientation>() ?? LocomotiveOrientation.Forward;

                                    // Look for the locomotive having the same unit address
                                    foreach (TrainLocomotiveInfo trainLoco in train.Locomotives) {
                                        if (trainLoco.Locomotive.Id == loco.Id) {
                                            validDuplicate = false;
                                            break;
                                        }

                                        if (trainLoco.Locomotive.AddressProvider.Unit == loco.AddressProvider.Unit) {
                                            if (trainLoco.Orientation == orientation)
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

                            if (!validDuplicate) {
                                e.Info = new CanPlaceTrainResult() {
                                    Status = CanPlaceTrainStatus.LocomotiveAddressAlreadyUsed,
                                    Loco1 = loco,
                                    Train = addressMapEntry.Train,
                                    CanBeResolved = true,
                                    ResolveMethod = CanPlaceTrainResolveMethod.ReprogramAddress
                                };
                                return;
                            }
                        }
                    }
                }
                else
                    throw new ArgumentException("Invalid placeable element");
            }   // Block power is digital

            e.Info = new CanPlaceTrainResult() {
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
        /// <param name="e.Sender">The XmlElement of the locomotive/locomotive set to be checked</param>
        [LayoutEvent("can-locomotive-be-placed-on-track")]
        private void canPlaceLocomotiveOnTrack(LayoutEvent e) {
            var placeableElement = Ensure.NotNull<XmlElement>(e.Sender, "placeableElement");
            var blockDefinition = e.Info as LayoutBlockDefinitionComponent;
            var result = new CanPlaceTrainResult();

            if (placeableElement.Name == E_Train) {
                TrainInCollectionInfo trainInCollection = new TrainInCollectionInfo(placeableElement);
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
                LocomotiveInfo loco = new LocomotiveInfo(placeableElement);

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

            e.Info = result;
        }

        [LayoutEvent("can-locomotive-be-placed")]
        private void CanLocomotiveBePlaced(LayoutEvent e0) {
            var e = (LayoutEvent<XmlElement, LayoutBlockDefinitionComponent, CanPlaceTrainResult>)e0;
            CanPlaceTrainResult result;
            var blockDefinition = e.Info;

            result = Ensure.NotNull<CanPlaceTrainResult>(EventManager.Event(new LayoutEvent("can-locomotive-be-placed-on-track", e)), "can-locomotive-be-placed-on-track");

            if (result.Status == CanPlaceTrainStatus.CanPlaceTrain) {
                e.Info = blockDefinition;
                result = Ensure.NotNull<CanPlaceTrainResult>(EventManager.Event(new LayoutEvent("is-locomotive-address-valid", e)), "is-locomotive-address-valid");
            }

            e.Result = result;
        }

        #endregion

        #region Locomotive Address map

        private static void AddTrains(LocomotiveAddressMap addressMap, IEnumerable<LayoutBlock> blocks, Func<LocomotiveInfo, TrainStateInfo, OnTrackLocomotiveAddressMapEntry> entryAllocator) {
            foreach (var train in blocks.SelectMany(block => from trainLocation in block.Trains select trainLocation.Train))
                foreach (var locomotive in from trainLocomotive in train.Locomotives select trainLocomotive.Locomotive)
                    if (!addressMap.ContainsKey(locomotive.AddressProvider.Unit))
                        addressMap.Add(locomotive.AddressProvider.Unit, entryAllocator(locomotive, train));
        }

        private static IModelComponentIsCommandStation? ExtractCommandStation(LayoutEvent e) => e.Sender switch
        {
            LayoutBlockDefinitionComponent blockDefinition => blockDefinition.Power.PowerOriginComponent as IModelComponentIsCommandStation,
            IModelComponentIsCommandStation commandStation => commandStation,
            ILayoutPower power => power.PowerOriginComponent as IModelComponentIsCommandStation,
            _ => null
        };

        [LayoutEvent("add-on-powered-tracks-locomotives-to-address-map")]
        private void AddOnPoweredTracksLocomotivesToAddressMap(LayoutEvent e) {
            var addressMap = Ensure.NotNull<LocomotiveAddressMap>(e.Info, "addressMap");
            var commandStation = ExtractCommandStation(e);

            if (commandStation != null) {
                AddTrains(addressMap,
                    from blockDefinition in LayoutModel.Components<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases) where blockDefinition.Power.PowerOriginComponentId == commandStation.Id && blockDefinition.Power.Type == LayoutPowerType.Digital && blockDefinition.Block.HasTrains select blockDefinition.Block,
                    (loco, train) => new OnPoweredTrackLocomotiveAddressMapEntry(loco, train)
                );
            }
        }

        [LayoutEvent("add-on-non-powered-tracks-locomotives-to-adddress-map")]
        private void AddOnNonPoweredTracksLocomotivesToAddressMap(LayoutEvent e) {
            var addressMap = Ensure.NotNull<LocomotiveAddressMap>(e.Info, "addressMap");

            AddTrains(addressMap,
                from blockDefinition in LayoutModel.Components<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases) where blockDefinition.Power.Type == LayoutPowerType.Disconnected && blockDefinition.Block.HasTrains select blockDefinition.Block,
                (loco, train) => new OnDisconnectedTrackLocomotiveAddressMapEntry(loco, train)
            );
        }

        [LayoutEvent("add-on-shelf-locomotives-to-address-map")]
        private void AddOnShelfLocomotivesToAddressMap(LayoutEvent e) {
            var addressMap = Ensure.NotNull<LocomotiveAddressMap>(e.Info, "addressMap");
            var commandStation = ExtractCommandStation(e);
            TrackGauges gauges = 0;

            if (commandStation != null) {
                var trackConnectors = from trackPowerConnector in LayoutModel.Components<LayoutTrackPowerConnectorComponent>(LayoutModel.ActivePhases) where trackPowerConnector.Inlet.ConnectedOutlet.ObtainablePowers.Any(power => power.PowerOriginComponentId == commandStation.Id) select trackPowerConnector;

                gauges = trackConnectors.Aggregate<LayoutTrackPowerConnectorComponent, TrackGauges>(0, (g, powerConnector) => g | powerConnector.Info.TrackGauge);

                // Pass on all locomotives in collection
                foreach (XmlElement locomotiveElement in LayoutModel.LocomotiveCollection.CollectionElement.GetElementsByTagName(E_Locomotive)) {
                    LocomotiveInfo locomotive = new LocomotiveInfo(locomotiveElement);
                    int locomotiveAddress = locomotive.AddressProvider.Unit;

                    if (locomotiveAddress >= 0 && (locomotive.Guage & gauges) != 0 && !addressMap.ContainsKey(locomotiveAddress))
                        addressMap.Add(locomotiveAddress, new OnShelfLocomotiveAddressMapEntry(locomotive));
                }
            }
        }

        private readonly Dictionary<IModelComponentIsCommandStation, OnTrackLocomotiveAddressMap> addressMapCache = new Dictionary<IModelComponentIsCommandStation, OnTrackLocomotiveAddressMap>();

        [LayoutEvent("get-on-track-locomotive-address-map")]
        private void GetOnTrackLocomotiveAddressMap(LayoutEvent e0) {
            var e = (LayoutEvent<object, object, OnTrackLocomotiveAddressMap>)e0;
            var commandStation = ExtractCommandStation(e);
            OnTrackLocomotiveAddressMap addressMap;

            if (commandStation != null) {
                if (!addressMapCache.TryGetValue(commandStation, out addressMap)) {
                    addressMap = new OnTrackLocomotiveAddressMap();

                    EventManager.Event(new LayoutEvent("add-on-powered-tracks-locomotives-to-address-map", commandStation, addressMap));
                    addressMapCache.Add(commandStation, addressMap);
                }
            }
            else
                addressMap = new OnTrackLocomotiveAddressMap();

            e.Result = addressMap;
        }

        // Trap events that invalidate the locomotive address map cache
        [LayoutEvent("train-is-removed")]
        [LayoutEvent("train-placed-on-track")]
        [LayoutEvent("locomotive-power-changed")]
        private void InvalidateAddressMap(LayoutEvent e) {
            addressMapCache.Clear();
        }

        [LayoutEvent("allocate-locomotive-address")]
        private void AllocateLocomotiveAddress(LayoutEvent e0) {
            var e = (LayoutEventResultValueType<LayoutBlockDefinitionComponent, LocomotiveInfo, int>)e0;
            var commandStation = ExtractCommandStation(e);
            var locomotive = Ensure.NotNull<LocomotiveInfo>(e.Info, "locomotive");
            LocomotiveAddressMap addressMap = new LocomotiveAddressMap();

            e.Result = default;

            if (locomotive.DecoderType is DecoderWithNumericAddressTypeInfo) {
                // Build address map of used addresses
                if (commandStation != null)
                    BuildAddressMap(commandStation, addressMap);
                else {
                    var commandStations = LayoutModel.Components<IModelComponentIsCommandStation>(LayoutModel.ActivePhases);

                    // If there is only one command station in the layout (very probable)
                    commandStation = commandStations.SingleOrDefault();

                    foreach (var c in commandStations)
                        BuildAddressMap(c, addressMap);
                }

                // At that stage the address map is complete
                int minAddress = locomotive.GetLowestAddress(commandStation);
                int maxAddress = locomotive.GetHighestAddress(commandStation);
                int numberOfAddresses = maxAddress - minAddress + 1;

                int address = minAddress;
                int bestOnNonPowerTrackAddress = -1;            // Address to use if need to reuse an address of a locomotive on a non-powered track
                int bestOnShelfAddress = -1;                    // Address to use if need to reuse an address of a locomotive that is not on track

                for (int count = 0; count < numberOfAddresses; count++) {
                    if (!addressMap.TryGetValue(address, out LocomotiveAddressMapEntryBase addressMapEntry)) {
                        // Found unused address - this is the best
                        e.Result = address;
                        return;
                    }
                    else if (!(addressMapEntry is LocomotiveBusModuleAddressMapEntry) && !(addressMapEntry is OnPoweredTrackLocomotiveAddressMapEntry)) {
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

                e.Result = address;
            }
        }

        private static void BuildAddressMap(IModelComponentIsCommandStation commandStation, LocomotiveAddressMap addressMap) {
            EventManager.Event(new LayoutEvent("add-on-powered-tracks-locomotives-to-address-map", commandStation, addressMap));
            EventManager.Event(new LayoutEvent("add-command-station-loco-bus-to-address-map", commandStation, addressMap));
            EventManager.Event(new LayoutEvent("add-on-non-powered-tracks-locomotives-to-adddress-map", commandStation, addressMap));
            EventManager.Event(new LayoutEvent("add-on-shelf-locomotives-to-address-map", commandStation, addressMap));
        }

        #region

        #endregion
        #endregion

        #region Locomotive placement/removal handlers

        /// <summary>
        /// Extract a provided locomotive trainState. The trainState can be either directly or indirectly provided.
        /// For example, the trainState can be the one of a given locomotive or locomotive set. An exception
        /// is thrown if trainState of an object which is not on the track is requested.
        /// </summary>
        [LayoutEvent("extract-train-state")]
        private void ExtractLocomotiveStateHandler(LayoutEvent e0) {
            var e = (LayoutEvent<object, object, TrainStateInfo>)e0;

            switch(e.Sender) {
                case TrainStateInfo train:
                    e.Result = train;
                    break;

                case TrainCommonInfo trainCommonInfo:
                    e.Result = LayoutModel.StateManager.Trains[trainCommonInfo.Id];
                    if (e.Result == null)
                        throw new TrainNotOnTrackException(trainCommonInfo);
                    break;

                case XmlElement element:
                    switch(element.Name) {
                        case "TrainState": e.Result = new TrainStateInfo(element); break;
                        case E_Locomotive: {
                                var loco = new LocomotiveInfo(element);
                                e.Result = EventManager.Event<object, object, TrainStateInfo>("extract-train-state", loco);
                            }
                            break;
                        default: throw new ArgumentException("Invalid XML element (not LocomotiveState, Locomotive or Train)");
                    }
                    break;

                case LocomotiveInfo loco:
                    e.Result = LayoutModel.StateManager.Trains[loco.Id];
                    if (e.Result == null)
                        throw new LocomotiveNotOnTrackException(loco);
                    break;

                default:
                    throw new ArgumentException("Invalid argument (Not XmlElement, LocomotiveInfo, TrainCommonInfo, TrainInCollectionInfo or TrainStateInfo)");
            }
        }

        public static TrainStateInfo ExtractTrainState(object? something) {
            return EventManager.Event<object, object, TrainStateInfo>("extract-train-state", something)!;
        }

        /// <summary>
        /// Remove an object (locomotive or locomotive set) from track
        /// </summary>
        [LayoutEventDef("remove-from-track-request", Role = LayoutEventRole.AsyncRequest, SenderType = typeof(TrainStateInfo), InfoType = typeof(string))]
        [LayoutAsyncEvent("remove-from-track-request")]
        private async Task removeFromTrackRequest(LayoutEvent e0) {
            var e = (LayoutEvent<object, string>)e0;
            var trainState = TrainsManager.ExtractTrainState(e.Sender);
            var ballonText = e.Info;

            if (ballonText != null) {
                var ballon = new LayoutBlockBallon() { Text = ballonText, RemoveOnClick = true, CancellationToken = e.GetCancellationToken() };

                if (trainState.LocomotiveBlock != null) {
                    LayoutBlockBallon.Show(trainState.LocomotiveBlock.BlockDefinintion, ballon);
                    await ballon.Task;
                }
            }

            LayoutModel.StateManager.Trains.Remove(trainState);
        }

        [LayoutAsyncEvent("wait-for-locomotive-placement")]
        private async Task waitForPlacement(LayoutEvent e) {
            var blockDefinition = Ensure.NotNull<LayoutBlockDefinitionComponent>(e.Sender, "blockDefinition");
            var message = Ensure.NotNull<string>(e.Info, "message");
            var trainId = (Guid)e.GetOption(Option_TrainId);

            var placeTrainBallon = new LayoutBlockBallon {
                Text = message,
                CancellationToken = e.GetCancellationToken(),
                FillColor = System.Drawing.Color.LightGreen,
                TextColor = System.Drawing.Color.Black,
                RemoveOnClick = true,
                RemoveOnTrainDetected = true
            };
            LayoutBlockBallon.Show(blockDefinition, placeTrainBallon);

            try {
                await placeTrainBallon.Task;
            }
            catch (OperationCanceledException) {
                var train = LayoutModel.StateManager.Trains[trainId];

                if (train != null)
                    LayoutModel.StateManager.Trains.RemoveTrainState(train);

                EventManager.Event(new LayoutEventInfoValueType<object, Guid>("free-owned-layout-locks", null, trainId).SetOption("ReleasePending", true));
                throw;
            }
        }

        /// <summary>
        /// Create train (runtime state) from locomotive or train element in the collection 
        /// </summary>
        /// <remarks>
        /// The event Train/Front option is set to contain the train's front. This will be needed by the place-train-on-block event handler
        /// </remarks>
        [LayoutEvent("create-train")]
        private void createTrain(LayoutEvent e0) {
            var e = (LayoutEvent<LayoutBlockDefinitionComponent, XmlElement, TrainStateInfo>)e0;
            var blockDefinition = Ensure.NotNull<LayoutBlockDefinitionComponent>(e.Sender, "blockDefinition");
            var collectionElement = Ensure.NotNull<XmlElement>(e.Info, "collectionElement");
            LayoutComponentConnectionPoint? front;
            string? trainName = null;
            TrainLength trainLength;

            e.Info = null;
            trainLength = e.GetOption(optionName: "Length", elementName: E_Train).ToTrainLength() ?? TrainLength.Standard;

            if (e.HasOption(E_Train, "Front"))
                front = e.GetOption(optionName: "Front", elementName: E_Train).ToComponentConnectionPoint();
            else {
                if (collectionElement.Name == E_Train) {
                    front = EventManager.EventResultValueType<LayoutBlockDefinitionComponent, object, LayoutComponentConnectionPoint>("get-locomotive-front", blockDefinition, collectionElement);

                    if (front == null)
                        throw new OperationCanceledException("get-locomotive-front");

                    TrainInCollectionInfo trainInCollection = new TrainInCollectionInfo(collectionElement);

                    trainLength = trainInCollection.Length;
                    trainName = trainInCollection.Name;
                }
                else {
                    var t = EventManager.Event<LayoutBlockDefinitionComponent, object, TrainFrontAndLength>("get-train-front-and-length", blockDefinition, collectionElement);

                    if (t == null)
                        throw new OperationCanceledException("get-train-front-and-length");         // User canceled.

                    front = t.Front;
                    trainLength = t.Length;
                    trainName = new LocomotiveInfo(collectionElement).Name;
                }
            }

            // Figure out the train name
            trainName = (string?)e.GetOption(optionName: A_Name, elementName: E_Train) ?? trainName;

            if (trainName == null) {
                switch (collectionElement.Name) {
                    case E_Locomotive: trainName = new LocomotiveInfo(collectionElement).Name; break;
                    case E_Train: trainName = new TrainInCollectionInfo(collectionElement).Name; break;
                    default:
                        throw new LayoutException(EventManager.Instance, "No name was given to the new train");
                }
            }

            Guid trainID = Guid.Empty;

            if (collectionElement.Name == E_Train) {
                TrainInCollectionInfo trainInCollection = new TrainInCollectionInfo(collectionElement);

                trainID = trainInCollection.Id;
            }

            TrainStateInfo train = LayoutModel.StateManager.Trains.Add(trainID);

            train.Name = trainName;
            train.Length = trainLength;

            if (collectionElement != null)
                train.AddLocomotiveCollectionElement(collectionElement, blockDefinition.Block, (bool?)e.GetOption("ValidateAddress") ?? true);

            e.SetOption(elementName: E_Train, optionName: "Front", value: front.ToString());
            e.Result = train;
        }

        [LayoutEvent("place-train-in-block")]
        private void placeTrainInBlock(LayoutEvent e0) {
            var e = (LayoutEvent<TrainStateInfo, LayoutBlockDefinitionComponent>)e0;
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");
            var blockDefinition = Ensure.NotNull<LayoutBlockDefinitionComponent>(e.Info, "blockDefinition");
            var front = e.GetOption(optionName: "Front", elementName: E_Train).ToComponentConnectionPoint();

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
        [LayoutAsyncEvent("place-train-request")]
        [LayoutEventDef("train-created", Role = LayoutEventRole.Notification, SenderType = typeof(TrainStateInfo), InfoType = typeof(LayoutBlock))]
        [LayoutEventDef("train-placed-on-track", Role = LayoutEventRole.Notification, SenderType = typeof(TrainStateInfo))]
        private async Task<object> placeTrainRequest(LayoutEvent e) {
            var blockDefinition = Ensure.NotNull<LayoutBlockDefinitionComponent>(e.Sender, "blockDefinition");
            var collectionElement = Ensure.NotNull<XmlElement>(e.Info, "collectionElement");

            e.Info = null;

            // Get lock on the block that the train is about to be placed, when the lock is obtained, the train is actually placed in the block
            LayoutLockRequest lockRequest;

            var createTrainEvent = new LayoutEvent<LayoutBlockDefinitionComponent, XmlElement, TrainStateInfo>("create-train", blockDefinition, collectionElement);
            var train = Ensure.NotNull<TrainStateInfo>(EventManager.DoEvent(createTrainEvent.CopyOptions(e, E_Train)).Result, "train");

            if (blockDefinition.Block.LockRequest == null || !blockDefinition.Block.LockRequest.IsManualDispatchLock) {
                lockRequest = new LayoutLockRequest(train.Id) {
                    CancellationToken = e.GetCancellationToken()
                };
                lockRequest.Blocks.Add(blockDefinition.Block);

                Task lockTask = EventManager.AsyncEvent(new LayoutEvent("request-layout-lock-async", lockRequest).CopyOperationContext(e));

                if (!lockTask.IsCompleted)
                    LayoutBlockBallon.Show(blockDefinition,
                        new LayoutBlockBallon() {
                            Text = "Do not place train on track yet!",
                            CancellationToken = e.GetCancellationToken(),
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

            EventManager.Event(new LayoutEvent<TrainStateInfo, LayoutBlockDefinitionComponent>("place-train-in-block", train, blockDefinition).CopyOptions(createTrainEvent, E_Train));
            await EventManager.AsyncEvent(new LayoutEvent<LayoutBlockDefinitionComponent, string>("wait-for-locomotive-placement", blockDefinition, "Please place train " + train.Name + " on track").CopyOperationContext(e).SetOption(Option_TrainId, train.Id));

            // At this stage the place where the train is about to be placed is locked, and train is supposed to be on track (at least the user has indicated that by
            // clicking on the ballon, or a train was detected in this block)

            EventManager.Event(new LayoutEvent<TrainStateInfo, LayoutBlockDefinitionComponent>("train-created", train, blockDefinition));
            EventManager.Event(new LayoutEvent<TrainStateInfo>("train-placed-on-track", train));

            return train;
        }

        [LayoutAsyncEvent("validate-and-place-train-request")]
        private async Task<object> validateAndPlaceTrain(LayoutEvent e0) {
            var e = (LayoutEvent<LayoutBlockDefinitionComponent, XmlElement>)e0;
            var blockDefinition = Ensure.NotNull<LayoutBlockDefinitionComponent>(e.Sender, "blockDefinition");
            var placedElement = Ensure.NotNull<XmlElement>(e.Info, "placedElement");
            TrainStateInfo train;

            var result = EventManager.Event<XmlElement, LayoutBlockDefinitionComponent, CanPlaceTrainResult>("can-locomotive-be-placed", placedElement, blockDefinition)!;

            if (result.ResolveMethod == CanPlaceTrainResolveMethod.ReprogramAddress) {
                var programmingState = new PlacedLocomotiveProgrammingState(placedElement, result.Locomotive, blockDefinition);

                train = (TrainStateInfo?)await (Task<object>)EventManager.AsyncEvent(new LayoutEvent<PlacedLocomotiveProgrammingState>("placed-locomotive-address-programming", programmingState).CopyOptions(e, E_Train).CopyOperationContext(e));

                if (train == null)
                    result = EventManager.Event<XmlElement, LayoutBlockDefinitionComponent, CanPlaceTrainResult>("can-locomotive-be-placed", placedElement, blockDefinition)!;
            }

            if (result.ResolveMethod == CanPlaceTrainResolveMethod.Resolved) {
                train = (TrainStateInfo)await (Task<object>)EventManager.AsyncEvent(new LayoutEvent("place-train-request", blockDefinition, placedElement).CopyOptions(e, E_Train).CopyOptions(e).CopyOperationContext(e));

                EventManager.Event(new LayoutEvent("set-train-initial-motion-direction-request", train));
            }
            else
                throw new LayoutException("Train/Locomotive cannot be placed");

            return train;
        }

        [LayoutEventDef("relocate-train-request", Role = LayoutEventRole.Request, SenderType = typeof(TrainStateInfo), InfoType = typeof(LayoutBlockDefinitionComponent))]
        [LayoutEvent("relocate-train-request")]
        private void relocateTrainRequest(LayoutEvent e) {
            var train = TrainsManager.ExtractTrainState(e.Sender);
            var blockDefinition = Ensure.NotNull<LayoutBlockDefinitionComponent>(e.Info, "blockDefinition");
            LayoutComponentConnectionPoint? front;

            if (e.HasOption(E_Train, "Front"))
                front = e.GetOption("Front", E_Train).ToComponentConnectionPoint();
            else {
                front = EventManager.EventResultValueType<LayoutBlockDefinitionComponent, object, LayoutComponentConnectionPoint>("get-locomotive-front", blockDefinition, train);

                if (front == null)
                    return;         // User canceled.
            }

            train.EraseImage();

            var oldLocations = new List<TrainLocationInfo>(train.Locations);
            var blocksToUnlock = new List<LayoutBlock>(oldLocations.Count);

            foreach (TrainLocationInfo oldTrainLocation in oldLocations) {
                if(oldTrainLocation.Block.LockRequest != null && !oldTrainLocation.Block.LockRequest.IsManualDispatchLock)
                    blocksToUnlock.Add(oldTrainLocation.Block);
                train.LeaveBlock(oldTrainLocation.Block, false);
            }

            if(blocksToUnlock.Count > 0)
                EventManager.Event(new LayoutEvent("free-layout-lock", blocksToUnlock));

            // Get lock on the block that the train is about to be placed
            LayoutLockRequest lockRequest = new LayoutLockRequest(train.Id);

            if (blockDefinition.Block.LockRequest == null || !blockDefinition.Block.LockRequest.IsManualDispatchLock) {
                lockRequest.Blocks.Add(blockDefinition.Block);
                EventManager.Event(new LayoutEvent("request-layout-lock", lockRequest));
            }

            // Create the new location for the train
            TrainLocationInfo trainLocation = train.PlaceInBlock(blockDefinition.Block, front.Value);

            trainLocation.DisplayFront = front.Value;
            train.LastCrossedBlockEdge = null;

            EventManager.Event(new LayoutEvent("train-relocated", train, blockDefinition.Block));

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

            AutoExtendTrain(train, blockDefinition);
        }

        #endregion

        #region Locomotive address reprogramming handling

        [LayoutAsyncEvent("placed-locomotive-address-programming")]
        private async Task<object> placedLocomotiveAddressReprogramming(LayoutEvent e) {
            var programmingState = Ensure.NotNull<PlacedLocomotiveProgrammingState>(e.Sender, "programmingState");

            var address = EventManager.EventResultValueType<LayoutBlockDefinitionComponent, LocomotiveInfo, int>("allocate-locomotive-address", programmingState.PlacementLocation, programmingState.Locomotive);

            if (address.HasValue) {
                programmingState.ProgrammingActions = new LayoutActionContainer<LocomotiveInfo>(programmingState.Locomotive);

                var changeAddressAction = (ILayoutLocomotiveAddressChangeAction?)programmingState.ProgrammingActions.Add("set-address");

                if (changeAddressAction != null) {
                    changeAddressAction.Address = address.Value;
                    changeAddressAction.SpeedSteps = programmingState.Locomotive.SpeedSteps;
                    var train = (TrainStateInfo)await (Task<object>)EventManager.AsyncEvent(new LayoutEvent("program-locomotive", programmingState).CopyOptions(e, E_Train).CopyOperationContext(e));

                    return train;
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
                    if (blockDefinition.PowerConnector.Locomotives.All(trainLocomotive => targetLocomotive != null && trainLocomotive.LocomotiveId == targetLocomotive.Id))
                        return CanUseForProgrammingResult.Yes;
                    else
                        return CanUseForProgrammingResult.YesButHasOtherTrains;
                }
                else
                    return CanUseForProgrammingResult.Yes;  // Can get power, and has no locomotives
            }
            else
                return CanUseForProgrammingResult.No;       // Block cannot get programming power.
        }

        private static Task ObtainProgrammingTracksLock(TrainStateInfo train, LayoutBlockDefinitionComponent programmingLocation, string ballonText, CancellationToken cancellationToken) {
            // Lock the blocks that get power from the inlet that the programming location gets its power from
            var lockRequest = GetProgrammingTracksLock(train.Id, programmingLocation, cancellationToken);

            var lockTask = EventManager.AsyncEvent(new LayoutEvent("request-layout-lock-async", lockRequest));

            if (!lockTask.IsCompleted) {
                var waitLockBallon = new LayoutBlockBallon() { Text = ballonText, CancellationToken = cancellationToken, FillColor = System.Drawing.Color.Red, TextColor = System.Drawing.Color.Yellow };

                LayoutBlockBallon.Show(programmingLocation, waitLockBallon);
            }

            // If block is locked to this train, but with a different lock request, release the old lock
            if (programmingLocation.Block.LockRequest != null && programmingLocation.Block.LockRequest != lockRequest && programmingLocation.Block.LockRequest.OwnerId == train.Id)
                EventManager.Event(new LayoutEventInfoValueType<object, Guid>("free-owned-layout-locks", null, train.Id).SetOption("ReleasePending", false));

            return lockTask;
        }

        private static LayoutLockRequest GetProgrammingTracksLock(Guid ownerId, LayoutBlockDefinitionComponent programmingLocation, CancellationToken canellationToken) {
            var lockRequest = new LayoutLockRequest(ownerId) {
                Type = LayoutLockType.Programming,
                CancellationToken = canellationToken
            };
            lockRequest.Blocks.Add(programmingLocation.PowerConnector.Blocks);

            var commandStationResource = (from power in programmingLocation.PowerConnector.Inlet.ConnectedOutlet.ObtainablePowers where power.Type == LayoutPowerType.Programmer select power.PowerOriginComponent).FirstOrDefault() as ILayoutLockResource;

            Debug.Assert(commandStationResource != null);

            lockRequest.Resources = new ILayoutLockResource[] { commandStationResource };
            return lockRequest;
        }

        [LayoutAsyncEvent("program-locomotive")]
        private async Task<object?> programLocomotive(LayoutEvent e) {
            var programmingState = Ensure.NotNull<LocomotiveProgrammingState>(e.Sender, "programmingState");
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
                    programmingState.ProgrammingLocation = (LayoutBlockDefinitionComponent?)EventManager.Event(new LayoutEvent("get-programming-location", this));   // Ask the user where should the programming take place

                if (programmingState.ProgrammingLocation == null)
                    throw new OperationCanceledException("program-locomotive");

                LayoutBlockDefinitionComponent programmingLocation = programmingState.ProgrammingLocation;
                ILayoutPower originalPower = programmingLocation.Power;

                if (train != null && (train.LocomotiveBlock == null || programmingLocation.Block.Id != train.LocomotiveBlock.Id)) {
                    await EventManager.AsyncEvent(new LayoutEvent<object, string>("remove-from-track-request", train, "Remove locomotive to be programmed from track (and place it on programming track)").CopyOperationContext(e));
                    train = null;
                }

                try {
                    await EventManager.AsyncEvent(new LayoutEvent<object, object>("set-power", programmingLocation, LayoutPowerType.Programmer).CopyOperationContext(e));

                    if (train == null) {
                        var createTrainEvent = new LayoutEvent<LayoutBlockDefinitionComponent, XmlElement, TrainStateInfo>("create-train", programmingLocation, programmingState.Locomotive.Element).CopyOptions(e, E_Train).SetOption("ValidateAddress", false);

                        if (programmingState.PlacementLocation != null && programmingLocation.Id == programmingState.PlacementLocation.Id)
                            createTrainEvent.CopyOptions(e, E_Train);
                        else {
                            createTrainEvent.SetOption(elementName: E_Train, optionName: "Front", value: programmingLocation.Track.ConnectionPoints[0].ToString());
                            createTrainEvent.SetOption(elementName: E_Train, optionName: "Length", value: TrainLength.LocomotiveOnly.ToString());

                            e.GetOperationContext()?.AddPariticpant(programmingLocation);
                        }

                        train = EventManager.DoEvent(createTrainEvent).Result!;
                        await ObtainProgrammingTracksLock(train, programmingLocation, "Do not yet place locomotive on programming track", e.GetCancellationToken());
                        EventManager.Event(new LayoutEvent<TrainStateInfo, LayoutBlockDefinitionComponent>("place-train-in-block", train, programmingLocation).CopyOptions(createTrainEvent, E_Train).CopyOperationContext(e));
                    }
                    else
                        await ObtainProgrammingTracksLock(train, programmingLocation, "Waiting to obtain lock on programming track region", e.GetCancellationToken());

                    // Ask the user to put the locomotive on track (note that lockRequest is null, since the track is already locked)
                    await EventManager.AsyncEvent(new LayoutEvent<LayoutBlockDefinitionComponent, string>("wait-for-locomotive-placement", programmingLocation, "Place locomotive on programming track").CopyOptions(e).SetOption(Option_TrainId, train.Id).CopyOperationContext(e));

                    // When track is detected or when the user hit the ballon, wait 500ms and then start the actual programming
                    await Task.Delay(500, e.GetCancellationToken());

                    var commandStation = programmingLocation.Power.PowerOriginComponent as IModelComponentCanProgramLocomotives;

                    var r = (LayoutActionFailure)await (Task<object>)EventManager.AsyncEvent(new LayoutEvent<ILayoutActionContainer, IModelComponentCanProgramLocomotives>("do-command-station-actions", programmingState.ProgrammingActions, commandStation).SetOption("UsePOM", false));

                    if (r != null)
                        Error(r.ToString());

                    // If programming location is not same as original location (or where the train is supposed to be), remove the train from the programming location
                    if (programmingState.PlacementLocation == null || programmingState.PlacementLocation.Id != programmingLocation.Id) {
                        await EventManager.AsyncEvent(new LayoutEvent<object, string>("remove-from-track-request", train, "Remove locomotive from programming track"));
                        train = null;
                    }

                    if (r != null)
                        throw new LayoutException("Locomotive programming failed");
                }
                finally {
                    if (originalPower != programmingLocation.Power) {
                        Trace.WriteLine("Set power back to " + originalPower.Name + " type: " + originalPower.Type.ToString());
                        Task _ = EventManager.AsyncEvent(new LayoutEvent<object, object>("set-power", programmingLocation, originalPower));
                    }

                    Trace.WriteLine("Free programming locked");

                    EventManager.Event(new LayoutEvent("free-layout-lock", GetProgrammingTracksLock(Guid.Empty, programmingLocation, CancellationToken.None)));

                    if (programmingState.PlacementLocation == null || programmingLocation.Id != programmingState.PlacementLocation.Id)
                        e.GetOperationContext()?.RemoveParticipant(programmingLocation);
                }
            }
            else {
                if (train?.CommandStation is IModelComponentCanProgramLocomotives commandStation) {
                    var r = (LayoutActionFailure)await (Task<object>)EventManager.AsyncEvent(new LayoutEvent<ILayoutActionContainer, IModelComponentCanProgramLocomotives>("do-command-station-actions", programmingState.ProgrammingActions, commandStation).SetOption("UsePOM", true));

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
                LayoutLockRequest lockRequest = new LayoutLockRequest(train.Id);

                lockRequest.Blocks.Add(newBlock);

                bool granted = (bool)EventManager.Event(new LayoutEvent("request-layout-lock", lockRequest));

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

        private void AutoExtendTrain(TrainStateInfo train, LayoutBlockDefinitionComponent blockDefinition) {
            if (blockDefinition.Info.IsOccupancyDetectionBlock) {
                // Automatically extend train to neighboring occupacy detection blocks in which train is detected
                foreach (LayoutBlockEdgeBase blockEdge in blockDefinition.Block.BlockEdges)
                    AutoExtendTrain(train, blockDefinition.Block, blockEdge);
            }
        }

        [LayoutEvent("request-auto-train-extend")]
        private void requestAutoTrainExtend(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");
            var blockDefinition = Ensure.NotNull<LayoutBlockDefinitionComponent>(e.Info, "blockDefinition");

            AutoExtendTrain(train, blockDefinition);
        }

        #endregion

        #region Locomotive speed and direction handlers

        /// <summary>
        /// Event which is sent to request a change in a locomotive motion properties (speed and direction)
        /// </summary>
        /// <param name="e.Sender">Indicate the locomotive or locomotive set. It can be either locomotiveState
        /// provider object or element, Locomotive provider object or element or Train object
        /// or element
        /// </param>
        /// <param name="e.Info">The speed. Positive value for forward motion, Negative value for backward motion
        /// </param>
        [LayoutEventDef("set-train-speed-request", SenderType = typeof(TrainStateInfo), InfoType = typeof(int))]
        [LayoutEvent("set-train-speed-request")]
        private void setLocomotiveSpeedRequest(LayoutEvent e) {
            var train = TrainsManager.ExtractTrainState(e.Sender);
            int speed = (int)e.Info;

            foreach (TrainLocomotiveInfo trainLoco in train.Locomotives)
                EventManager.Event(new LayoutEvent("locomotive-motion-command", trainLoco.Locomotive, (trainLoco.Orientation == LocomotiveOrientation.Forward) ? speed : -speed,
                    null).SetCommandStation(train));

            train.SetSpeedValue(speed);
        }

        /// <summary>
        /// Event which is sent to reverse the train motion direction.
        /// </summary>
        /// <param name="e.Sender">The train to be reversed</param>
        [LayoutEventDef("reverse-train-motion-direction-request", SenderType = typeof(TrainStateInfo))]
        [LayoutEvent("reverse-train-motion-direction-request")]
        private void reverseTrainMotionDirectionRequest(LayoutEvent e) {
            var train = TrainsManager.ExtractTrainState(e.Sender);

            foreach (TrainLocomotiveInfo trainLoco in train.Locomotives)
                EventManager.Event(new LayoutEvent("locomotive-reverse-motion-direction-command", trainLoco.Locomotive));
        }

        [LayoutEvent("set-train-initial-motion-direction-request")]
        private void setTrainInitialDirection(LayoutEvent e) {
            var train = TrainsManager.ExtractTrainState(e.Sender);

            foreach (TrainLocomotiveInfo trainLoco in train.Locomotives) {
                if (trainLoco.Orientation == LocomotiveOrientation.Backward)
                    EventManager.Event(new LayoutEvent("locomotive-reverse-motion-direction-command", trainLoco.Locomotive));
            }

            train.MotionDirection = LocomotiveOrientation.Forward;
        }

        /// <summary>
        /// This event is sent by the command station component when it get an indiciation of a change
        /// in a locomotive motion parameters
        /// </summary>
        /// <param name="e.Sender">The command station component</param>
        /// <param name="e.Info">The speed</param>
        /// The event document has a element named Address with an attribute Unit which indicate the
        /// locomotive address whose address is being changed
        [LayoutEvent("locomotive-motion-notification")]
        private void locomotiveMotionNotification(LayoutEvent e) {
            var unit = (int)e.GetOption(Option_Unit, OptionElement_Address);
            int speed = (int)e.Info;
            var commandStation = Ensure.NotNull<IModelComponentIsCommandStation>(e.Sender, "commandStation");

            var addressMap = EventManager.Event<object, object, OnTrackLocomotiveAddressMap>("get-on-track-locomotive-address-map", commandStation)!;
            var addressMapEntry = addressMap[unit];

            if (addressMapEntry == null)
                Error("Command station " + commandStation.NameProvider.Name + " reported motion of locomotive with address " + unit + ". This locomotive is not registered as being on the layout!");
            else {
                if (addressMapEntry.Train.Locomotives.Count > 1)
                    Warning("Command station " + commandStation.NameProvider.Name + " reported change in speed/direction of a single locomotive set member (address " + unit + ")");

                addressMapEntry.Train.SetSpeedValue(speed);
            }
        }

        [LayoutEvent("train-speed-changed")]
        private void trainSpeedChanged(LayoutEvent e) {
            var trainState = Ensure.NotNull<TrainStateInfo>(e.Sender, "trainState");

            foreach (TrainLocationInfo trainLocation in trainState.Locations) {
                LayoutBlock block = trainLocation.Block;

                if (block.BlockDefinintion != null)
                    block.BlockDefinintion.OnComponentChanged();
            }

#if NO
            if (trainState.SpeedInSteps == 0) {
                var edgeCrossingTime = new DateTime(trainState.LastBlockEdgeCrossingTime);
                var timeSinceEdgeCrossing = DateTime.Now - edgeCrossingTime;

                trainState.TrainStoppedOnBlockEdgeCrossing = timeSinceEdgeCrossing.Milliseconds < 500;

                Trace.WriteLine("Train stopped " + timeSinceEdgeCrossing.Milliseconds + " after crossing block edge");

                if (trainState.TrainStoppedOnBlockEdgeCrossing) /****/
                    Trace.WriteLine("*** Train stopped on block edge crossing");
            }
            else {
                if (trainState.TrainStoppedOnBlockEdgeCrossing && trainState.LastBlockEdgeCrossingSpeed != trainState.SpeedInSteps) {
                    trainState.TrainStoppedOnBlockEdgeCrossing = false;

                    if (trainState.LastCrossedBlockEdge != null) {
                        EventManager.Event(new LayoutEvent("anonymous-track-contact-triggerd", trainState.LastCrossedBlockEdge));
                        Trace.WriteLine("*** Triggered block edge, " + trainState.LastCrossedBlockEdge.FullDescription + " because train " + trainState.Name + " was standing on it and started to go in reverse direction");
                    }
                }
            }
#endif
        }

#endregion

#region Lights & Functions

        [LayoutEventDef("set-train-lights-request", Role = LayoutEventRole.Request, SenderType = typeof(TrainStateInfo), InfoType = typeof(bool))]
        [LayoutEvent("set-train-lights-request", Role = LayoutEventRole.Request, InfoType = typeof(bool), SenderType = typeof(TrainStateInfo))]
        private void setLocomotiveLightsRequest(LayoutEvent e) {
            var train = TrainsManager.ExtractTrainState(e.Sender);
            bool lights = (bool)e.Info;

            if (lights != train.Lights) {
                foreach (TrainLocomotiveInfo trainLoco in train.Locomotives) {
                    LocomotiveInfo loco = trainLoco.Locomotive;

                    train.SetLightsValue(lights);

                    if (loco.HasLights)
                        EventManager.Event(new LayoutEvent("set-locomotive-lights-command", loco, lights).SetCommandStation(train));
                }
            }
        }

        [LayoutEvent("toggle-locomotive-lights-notification")]
        private void toggleLocomotiveLightsNotification(LayoutEvent e) {
            var unit = (int)e.GetOption(Option_Unit, OptionElement_Address);
            var commandStation = Ensure.NotNull<IModelComponentIsCommandStation>(e.Sender, "commandStation");

            var addressMap = EventManager.Event<object, object, OnTrackLocomotiveAddressMap>("get-on-track-locomotive-address-map", commandStation)!;
            var addressMapEntry = addressMap[unit];

            if (addressMapEntry == null)
                Error("Command station " + commandStation.NameProvider.Name + " reported light status of locomotive with address " + unit + ". This locomotive is not registered as being on the layout!");
            else {
                TrainStateInfo train = addressMapEntry.Train;

                if (train.Locomotives.Count > 1)
                    Warning("Command station " + commandStation.NameProvider.Name + " reported change in light status of a single locomotive set member (address " + unit + ")");

                train.SetLightsValue(!train.Lights);
            }
        }

        [LayoutEvent("set-locomotive-lights-notification")]
        private void setLocomotiveLightsNotification(LayoutEvent e) {
            var commandStation = Ensure.NotNull<IModelComponentIsCommandStation>(e.Sender, "commandStation");
            var unit = (int)e.GetOption(Option_Unit, OptionElement_Address);
            var lights = (bool)e.Info;

            var addressMap = EventManager.Event<object, object, OnTrackLocomotiveAddressMap>("get-on-track-locomotive-address-map", commandStation)!;
            var addressMapEntry = addressMap[unit];

            if (addressMapEntry == null)
                Error("Command station " + commandStation.NameProvider.Name + " reported light status of locomotive with address " + unit + ". This locomotive is not registered as being on the layout!");
            else {
                TrainStateInfo train = addressMapEntry.Train;

                if (train.Locomotives.Count > 1)
                    Warning("Command station " + commandStation.NameProvider.Name + " reported change in light status of a single locomotive set member (address " + unit + ")");

                train.SetLightsValue(lights);
            }
        }

        [LayoutEvent("set-locomotive-function-state-request")]
        private void setLocomotiveFunctionStateRequest(LayoutEvent e) {
            var train = TrainsManager.ExtractTrainState(e.Sender);
            bool state = (bool)e.Info;
            XmlElement functionElement = e.Element[E_Function];
            string functionName = functionElement.GetAttribute(A_Name);
            var locomotiveId = (Guid?)functionElement.AttributeValue(A_LocomotiveId) ?? Guid.Empty;

            if (locomotiveId == Guid.Empty || state != train.GetFunctionState(functionName, locomotiveId, false)) {
                foreach (TrainLocomotiveInfo trainLoco in train.Locomotives) {
                    LocomotiveInfo loco = trainLoco.Locomotive;

                    if (locomotiveId == Guid.Empty || locomotiveId == loco.Id) {
                        train.SetLocomotiveFunctionStateValue(functionName, locomotiveId, state);
                        EventManager.Event(new LayoutEvent("set-locomotive-function-state-command", loco, functionName).SetOption(Option_FunctionState, state).SetCommandStation(train));

                        if (locomotiveId != Guid.Empty)
                            break;
                    }
                }
            }
        }

        [LayoutEvent("trigger-locomotive-function-request")]
        private void triggerLocomotiveFunctionRequest(LayoutEvent e) {
            var train = TrainsManager.ExtractTrainState(e.Sender);
            XmlElement functionElement = e.Element[E_Function];
            string functionName = functionElement.GetAttribute(A_Name);
            var locomotiveId = (Guid?)functionElement.AttributeValue(A_LocomotiveId) ?? Guid.Empty;

            foreach (TrainLocomotiveInfo trainLoco in train.Locomotives) {
                LocomotiveInfo loco = trainLoco.Locomotive;

                if (locomotiveId == Guid.Empty || locomotiveId == loco.Id) {
                    bool state = !train.GetFunctionState(functionName, loco.Id);        // Flip state

                    EventManager.Event(new LayoutEvent("trigger-locomotive-function-command", loco, functionName).SetCommandStation(train).SetOption(Option_FunctionState, state));
                    train.SetLocomotiveFunctionStateValue(functionName, loco.Id, state);
                }
            }
        }

        [LayoutEvent("set-locomotive-function-state-notification")]
        private void setLocomotiveFunctionStateNotification(LayoutEvent e) {
            var unit = (int)e.GetOption(Option_Unit, OptionElement_Address);
            var state = (bool)e.Info;
            var commandStation = Ensure.NotNull<IModelComponentIsCommandStation>(e.Sender, "commandStation");

            var addressMap = EventManager.Event<object, object, OnTrackLocomotiveAddressMap>("get-on-track-locomotive-address-map", commandStation)!;
            var addressMapEntry = addressMap[unit];

            if (addressMapEntry == null)
                Error("Command station " + commandStation.NameProvider.Name + " reported light status of locomotive with address " + unit + ". This locomotive is not registered as being on the layout!");
            else {
                TrainStateInfo train = addressMapEntry.Train;

                foreach (TrainLocomotiveInfo trainLoco in train.Locomotives) {
                    if (trainLoco.Locomotive.AddressProvider.Unit == unit) {
                        train.SetLocomotiveFunctionStateValue((int)e.GetOption(E_Function, "Number"), trainLoco.LocomotiveId, state);
                        break;
                    }
                }
            }
        }

#endregion

#region Speed limit

        [LayoutEvent("locomotive-configuration-changed")]
        private void locomotiveConfigurationChanged(LayoutEvent e) {
            var loco = Ensure.NotNull<LocomotiveInfo>(e.Sender, "loco");

            if (LayoutController.IsOperationMode) {
                foreach (XmlElement trainElement in LayoutModel.StateManager.Trains.Element) {
                    TrainStateInfo train = new TrainStateInfo(trainElement);

                    foreach (TrainLocomotiveInfo trainLocomotive in train.Locomotives) {
                        if (trainLocomotive.LocomotiveId == loco.Id)
                            train.FlushCachedValues();
                    }
                }
            }

            foreach (XmlElement trainElement in LayoutModel.LocomotiveCollection.CollectionElement.GetElementsByTagName(E_Train)) {
                TrainStateInfo train = new TrainStateInfo(trainElement);

                foreach (TrainLocomotiveInfo trainLocomotive in train.Locomotives) {
                    if (trainLocomotive.LocomotiveId == loco.Id)
                        train.FlushCachedValues();
                }
            }
        }

#endregion

#region Train controller handling

        [LayoutEvent("train-controller-activated")]
        private void trainControllerActivated(LayoutEvent e0) {
            var e = (LayoutEvent<TrainStateInfo>)e0;
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");

            foreach (var trainLoco in train.Locomotives)
                EventManager.Event(new LayoutEvent<TrainLocomotiveInfo, TrainStateInfo>("locomotive-controller-activated", trainLoco, train));
        }

        [LayoutEvent("train-controller-deactivated")]
        private void trainControllerDeactivated(LayoutEvent e0) {
            var e = (LayoutEvent<TrainStateInfo>)e0;
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");

            foreach (var trainLoco in train.Locomotives)
                EventManager.Event(new LayoutEvent<TrainLocomotiveInfo, TrainStateInfo>("locomotive-controller-deactivated", trainLoco, train));
        }

#endregion

#region State reconstuction operations

        private class TrackContactCrossTimeSorter : IComparer {
            public int Compare(object oLocation1, object oLocation2) {
                TrainLocationInfo l1 = (TrainLocationInfo)oLocation1;
                TrainLocationInfo l2 = (TrainLocationInfo)oLocation2;

                if (l1.BlockEdgeCrossingTime > l2.BlockEdgeCrossingTime)
                    return 1;
                else if (l1.BlockEdgeCrossingTime < l2.BlockEdgeCrossingTime)
                    return -1;
                else
                    return 0;
            }
        }

        [LayoutEvent("rebuild-layout-state", Order = 0)]
        private void rebuildComponents(LayoutEvent e) {
            var phase = e.GetPhases();
            bool ok = true;

            if (!RebuildComponentsState(phase))
                ok = false;

            e.Info = ok;
        }

        [LayoutEvent("rebuild-layout-state", Order = 1000)]
        private void rebuildLayoutState(LayoutEvent e) {
            LayoutPhase phase = e.GetPhases();
            bool ok = true;

            try {
                if (!rebuildTrainState(phase))
                    ok = false;

                if (!verifyTrackContactState(phase))
                    ok = false;

                if (!clearTrainDetectionBlocks(phase))
                    ok = false;
            }
            catch (Exception ex) {
                Warning("Problem when restoring layout state: " + ex.Message);
                ok = false;
            }

            e.Info = ok;
        }

        /// <summary>
        /// Check that the train state is valid. Check that each referred block, and
        /// referred locomotive (or locomotive set) can actually be accessed. Invalid trainState
        /// entries are removed. Also all locomotives are "placed" again, so block locomotive
        /// array and locomotive cound are corrected.
        /// </summary>
        private bool rebuildTrainState(LayoutPhase phase) {
            ArrayList invalidStateElements = new ArrayList();
            ArrayList noTrackContactCrossTrainLocations = new ArrayList();
            ArrayList trainLocations = new ArrayList();

            // Remove all locomotives from all blocks
            foreach (LayoutBlock block in LayoutModel.Blocks)
                block.ClearTrains();

            foreach (XmlElement trainStateElement in LayoutModel.StateManager.Trains.Element) {
                bool validTrainState = true;
                TrainStateInfo trainState = new TrainStateInfo(trainStateElement);

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
            // in acending order based on track contact crossing time. locations which were placed will be placed first
            foreach (TrainLocationInfo trainLocation in noTrackContactCrossTrainLocations) {
                // Get lock on the block that the train is about to be placed
                LayoutLockRequest lockRequest = new LayoutLockRequest(trainLocation.Train.Id);

                lockRequest.Blocks.Add(trainLocation.Block);
                EventManager.Event(new LayoutEvent("request-layout-lock", lockRequest));

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
                LayoutLockRequest lockRequest = new LayoutLockRequest(trainLocation.Train.Id);

                lockRequest.Blocks.Add(trainLocation.Block);
                EventManager.Event(new LayoutEvent("request-layout-lock", lockRequest).SetOption("DoNotLockResources", true));
            }

            return invalidStateElements.Count == 0;   // Fail if any invaid element was found
        }

        [LayoutEvent("enter-operation-mode", Order = 20000)]
        private void enterOperationModeAndInitializeTrains(LayoutEvent e) {
            foreach (XmlElement trainStateElement in LayoutModel.StateManager.Trains.Element) {
                TrainStateInfo train = new TrainStateInfo(trainStateElement);

                if (train.Speed != 0)
                    train.SpeedInSteps = 0;
            }
        }

        public bool RebuildComponentsState(LayoutPhase phase) {
            ArrayList removeList = new ArrayList();

            LayoutModel.StateManager.Components.IdToComponentStateElement.Clear();

            foreach (XmlElement componentStateElement in LayoutModel.StateManager.Components.Element) {
                var componentID = (Guid)componentStateElement.AttributeValue(A_Id);
                var component = LayoutModel.Component<ModelComponent>(componentID, LayoutPhase.All);
                bool validStateComponent = true;
                var topicRemoveList = new List<XmlElement>();

                if (component != null) {
                    foreach (XmlElement componentStateTopicElement in componentStateElement) {
                        if (!(bool)EventManager.Event(new LayoutEvent("verify-component-state-topic", componentStateTopicElement, true)))
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

        private bool verifyTrackContactState(LayoutPhase phase) {
            bool ok = true;

            foreach (LayoutTrackContactComponent trackContact in LayoutModel.Components<LayoutTrackContactComponent>(phase)) {
                if (LayoutModel.StateManager.Components.Contains(trackContact, "TrainPassing")) {
                    var trackContactPassingElement = LayoutModel.StateManager.Components.StateOf(trackContact, "TrainPassing", create: true);
                    TrackContactPassingStateInfo trackContactPassingState = new TrackContactPassingStateInfo(trackContactPassingElement);

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
        /// on the tracks, so there is no point in remebering the previous state
        /// </summary>
        /// <returns>True if all is ok</returns>
        private bool clearTrainDetectionBlocks(LayoutPhase phase) {
            foreach (LayoutBlockDefinitionComponent blockDefinition in LayoutModel.Components<LayoutBlockDefinitionComponent>(phase)) {
                if (blockDefinition.Info.IsOccupancyDetectionBlock) {
                    blockDefinition.Info.SetTrainWillBeDetected(false);
                    blockDefinition.Info.SetTrainDetected(false);
                }
            }

            return true;
        }

        [LayoutEvent("verify-component-state-topic", IfSender = "Ballon")]
        private void removeBallonTopics(LayoutEvent e) {
            e.Info = false;
        }

        /// <summary>
        /// Clear the locomotive trainState (removing all locomotives)
        /// </summary>
        [LayoutEvent("clear-layout-state")]
        private void clearLocomotiveState(LayoutEvent e) {
            // Remove all locomotives from all blocks
            foreach (LayoutBlock block in LayoutModel.Blocks)
                block.ClearTrains();

            LayoutModel.StateManager.DocumentElement.RemoveAll();
        }

#endregion
    }
}
