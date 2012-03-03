using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace LayoutManager.Logic {
	[LayoutModule("Trains Manager", UserControl = false)]
	class TrainsManager : LayoutModuleBase {
		#region Internal properties and helper functions

		#endregion

		#region Locomotive action handlers

		[LayoutEvent("query-action", IfEvent="LayoutEvent[Options/@Action='set-address']", SenderType=typeof(LocomotiveInfo))]
		private void querySetLocoAddress(LayoutEvent e0) {
			var e = (LayoutEvent<IHasDecoder, bool, bool>)e0;

			if(e.Sender.DecoderType is DccDecoderTypeInfo)
				e.Result = true;
		}

		[LayoutEvent("get-action", IfSender = "Action[@Type='set-address']", InfoType = typeof(LocomotiveInfo))]
		private void GetActionSetAddress(LayoutEvent e) {
			XmlElement actionElement = (XmlElement)e.Sender;
			LocomotiveInfo locomotive = e.Info as LocomotiveInfo;

			if(locomotive != null) {
				if(locomotive.DecoderType is DccDecoderTypeInfo)
					e.Info = new LayoutDccChangeLocomotiveAddressAction(actionElement, locomotive);
			}
		}

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
			XmlElement placeableElement = (XmlElement)e.Sender;
			ILayoutPower power;
			TrainStateInfo train = null;
			CanPlaceTrainResult result = new CanPlaceTrainResult();

			if(e.Info is LayoutBlock)
				power = ((LayoutBlock)e.Info).Power;
			else if(e.Info is LayoutBlockDefinitionComponent)
				power = ((LayoutBlockDefinitionComponent)e.Info).Block.Power;
			else if(e.Info is ILayoutPowerOutlet)
				power = ((ILayoutPowerOutlet)e.Info).Power;
			else if(e.Info is ILayoutPower)
				power = (ILayoutPower)e.Info;
			else if(e.Info is TrainStateInfo) {
				train = (TrainStateInfo)e.Info;
				power = train.LocomotiveBlock.Power;
			}
			else
				throw new ArgumentException("Invalid power argument");

			try {
				if(power.Type == LayoutPowerType.Digital) {
					//
					// Locomotive-Set address checks (applied only if the placed object is a locomotive set)
					//
					// a) If there is more than one member assigned to an address, and not all the members
					//    that are assigned to a given address are in the same orientation, generate
					//    an error event.
					// b) (TODO) Check that all members have identical decoder setting.
					//
					if(placeableElement.Name == "Train") {
						TrainInCollectionInfo trainInCollection = new TrainInCollectionInfo(placeableElement);
						IList<TrainLocomotiveInfo> locomotives = trainInCollection.Locomotives;

						for(int i = 0; i < locomotives.Count; i++) {
							LocomotiveInfo loco1 = locomotives[i].Locomotive;

							if(loco1.AddressProvider.Element == null) {
								result.Status = CanPlaceTrainStatus.LocomotiveHasNoAddress;
								result.Locomotive = loco1;
								result.CanBeResolved = true;
								result.ResolveMethod = CanPlaceTrainResolveMethod.ReprogramAddress;
								return;
							}

							if((loco1.DecoderType.SupportedDigitalPowerFormats & power.DigitalFormats) == 0) {
								result.Locomotive = loco1;
								result.Status = CanPlaceTrainStatus.LocomotiveDigitalFormatNotCompatible;
								result.CanBeResolved = false;
								result.ResolveMethod = CanPlaceTrainResolveMethod.NotPossible;
								return;
							}

							if(loco1.Kind == LocomotiveKind.SoundUnit)
								continue;

							for(int j = i + 1; j < locomotives.Count; j++) {
								LocomotiveInfo loco2 = locomotives[j].Locomotive;

								if(loco2.Kind == LocomotiveKind.SoundUnit)
									continue;

								if(loco1.AddressProvider.Unit == loco2.AddressProvider.Unit && locomotives[i].Orientation != locomotives[j].Orientation) {
									result.Status = CanPlaceTrainStatus.LocomotiveDuplicateAddress;
									result.Loco1 = loco1;
									result.Loco2 = loco2;

									result.CanBeResolved = true;		// Can be resolved if loco1 address is changed
									result.ResolveMethod = CanPlaceTrainResolveMethod.ReprogramAddress;
									return;
								}
							}
						}

						// Got to this point, each member has a unit address, and no invalid duplicates
						// within the locomotive set
						var addressMap = (OnTrackLocomotiveAddressMap)EventManager.Event(new LayoutEvent(power, "get-on-track-locomotive-address-map"));

						foreach(TrainLocomotiveInfo trainLocomotive in locomotives) {
							OnTrackLocomotiveAddressMapEntry addressMapEntry = addressMap[trainLocomotive.Locomotive.AddressProvider.Unit];

							if(addressMapEntry != null) {
								result.Status = CanPlaceTrainStatus.LocomotiveAddressAlreadyUsed;
								result.Loco1 = addressMapEntry.Locomotive;
								result.Train = addressMapEntry.Train;

								result.CanBeResolved = true;
								result.ResolveMethod = CanPlaceTrainResolveMethod.ReprogramAddress;
								return;
							}
						}
					}
					else if(placeableElement.Name == "Locomotive") {
						LocomotiveInfo loco = new LocomotiveInfo(placeableElement);

						if(!loco.NotManaged) {
							if(loco.AddressProvider.Element == null) {
								result.Status = CanPlaceTrainStatus.LocomotiveHasNoAddress;
								result.Locomotive = loco;
								result.CanBeResolved = true;
								result.ResolveMethod = CanPlaceTrainResolveMethod.ReprogramAddress;
								return;
							}

							var addressMap = (OnTrackLocomotiveAddressMap)EventManager.Event(new LayoutEvent(power, "get-on-track-locomotive-address-map"));
							var addressMapEntry = addressMap[loco.AddressProvider.Unit];

							// Check that the address is not already used.
							if(addressMapEntry != null) {
								bool validDuplicate = false;

								if(train != null && addressMapEntry.Train.Id == train.Id) {
									// Check that the locomotive in the train that uses the same address has the
									// same orientation as the placed locomotive
									if(loco.Kind != LocomotiveKind.SoundUnit) {
										LocomotiveOrientation orientation = LocomotiveOrientation.Forward;
										XmlElement orientationElement = e.Element["Orientation"];

										if(orientationElement != null)
											orientation = (LocomotiveOrientation)Enum.Parse(typeof(LocomotiveOrientation), orientationElement.GetAttribute("Value"));

										// Look for the locomotive having the same unit address
										foreach(TrainLocomotiveInfo trainLoco in train.Locomotives) {
											if(trainLoco.Locomotive.Id == loco.Id) {
												validDuplicate = false;
												break;
											}

											if(trainLoco.Locomotive.AddressProvider.Unit == loco.AddressProvider.Unit) {
												if(trainLoco.Orientation == orientation)
													// It is valid to have two locomotive with the same address
													// as long as they have the same orientation
													validDuplicate = true;
												if(trainLoco.Locomotive.Kind == LocomotiveKind.SoundUnit)
													validDuplicate = true;
											}
										}

									}
									else
										validDuplicate = true;
								}

								if(!validDuplicate) {
									result.Status = CanPlaceTrainStatus.LocomotiveAddressAlreadyUsed;
									result.Loco1 = loco;
									result.Train = addressMapEntry.Train;
									result.CanBeResolved = true;
									result.ResolveMethod = CanPlaceTrainResolveMethod.ReprogramAddress;
									return;
								}
							}
						}
					}
					else
						throw new ArgumentException("Invalid placeable element");
				}	// Block power is digital
			}
			finally {
				e.Info = result;
			}
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
			XmlElement placeableElement = (XmlElement)e.Sender;
			LayoutBlockDefinitionComponent blockDefinition = e.Info as LayoutBlockDefinitionComponent;
			CanPlaceTrainResult result = new CanPlaceTrainResult();

			if(placeableElement.Name == "Train") {
				TrainInCollectionInfo trainInCollection = new TrainInCollectionInfo(placeableElement);
				TrainStateInfo oldTrainState = LayoutModel.StateManager.Trains[trainInCollection.Id];

				if(oldTrainState != null)
					result.Train = oldTrainState;
				else {
					// The locomotive set is not on the track, ensure that non of its members is on the tracks
					// Also verify that all members have the same number of speed steps
					foreach(TrainLocomotiveInfo trainLocomotive in trainInCollection.Locomotives) {
						LocomotiveInfo loco = trainLocomotive.Locomotive;
						TrainStateInfo memberTrainState = LayoutModel.StateManager.Trains[trainLocomotive.LocomotiveId];

						if(blockDefinition != null && (loco.Guage & blockDefinition.Guage) == 0) {
							result.Status = CanPlaceTrainStatus.LocomotiveGuageNotCompatibleWithTrack;
							result.Locomotive = loco;
							result.BlockDefinition = blockDefinition;
							result.CanBeResolved = false;
							result.ResolveMethod = CanPlaceTrainResolveMethod.NotPossible;
						}
						else if(memberTrainState != null) {
							result.Status = CanPlaceTrainStatus.TrainLocomotiveAlreadyUsed;
							result.Locomotive = loco;
							result.Train = memberTrainState;
							result.CanBeResolved = false;
							result.ResolveMethod = CanPlaceTrainResolveMethod.NotPossible;
						}
					}
				}
			}
			else if(placeableElement.Name == "Locomotive") {
				Guid id = LayoutModel.LocomotiveCollection.GetElementId(placeableElement);
				TrainStateInfo oldTrainState = LayoutModel.StateManager.Trains[id];
				LocomotiveInfo loco = new LocomotiveInfo(placeableElement);

				if(oldTrainState != null) {
					// The locomotive is already on track
					if(oldTrainState.Locomotives.Count > 1) {
						result.Status = CanPlaceTrainStatus.LocomotiveAddressAlreadyUsed;
						result.Locomotive = loco;
						result.Train = oldTrainState;
						result.CanBeResolved = true;
						result.ResolveMethod = CanPlaceTrainResolveMethod.ReprogramAddress;
					}
					else
						result.Train = oldTrainState;
				}
				else if(blockDefinition != null && (loco.Guage & blockDefinition.Guage) == 0) {
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
		private void CanLocomotiveBePlaced(LayoutEvent e) {
			CanPlaceTrainResult result;
			object info = e.Info;

			result = (CanPlaceTrainResult)EventManager.Event(new LayoutEvent("can-locomotive-be-placed-on-track", e));

			if(result.Status == CanPlaceTrainStatus.CanPlaceTrain) {
				e.Info = info;
				result = (CanPlaceTrainResult)EventManager.Event(new LayoutEvent("is-locomotive-address-valid", e));
			}

			e.Info = result;
		}

		#endregion

		#region Locomotive Address map

		private static void AddTrains(LocomotiveAddressMap addressMap, IEnumerable<LayoutBlock> blocks, Func<LocomotiveInfo, TrainStateInfo, OnTrackLocomotiveAddressMapEntry> entryAllocator) {
			foreach(var train in blocks.SelectMany(block => from trainLocation in block.Trains select trainLocation.Train))
				foreach(var locomotive in from trainLocomotive in train.Locomotives select trainLocomotive.Locomotive)
					if(!addressMap.ContainsKey(locomotive.AddressProvider.Unit))
						addressMap.Add(locomotive.AddressProvider.Unit, entryAllocator(locomotive, train));
		}

		private static IModelComponentIsCommandStation ExtractCommandStation(LayoutEvent e) {
			IModelComponentIsCommandStation commandStation = null;

			if(e.Sender is LayoutBlockDefinitionComponent) {
				var blockDefinition = (LayoutBlockDefinitionComponent)e.Sender;

				commandStation = blockDefinition.Power.PowerOriginComponent as IModelComponentIsCommandStation;
			}
			else if(e.Sender is IModelComponentIsCommandStation)
				commandStation = (IModelComponentIsCommandStation)e.Sender;
			else if(e.Sender is ILayoutPower) {
				ILayoutPower power = e.Sender as ILayoutPower;

				commandStation = power.PowerOriginComponent as IModelComponentIsCommandStation;
			}

			return commandStation;
		}

		[LayoutEvent("add-on-powered-tracks-locomotives-to-address-map")]
		private void AddOnPoweredTracksLocomotivesToAddressMap(LayoutEvent e) {
			LocomotiveAddressMap addressMap = (LocomotiveAddressMap)e.Info;
			IModelComponentIsCommandStation commandStation = ExtractCommandStation(e);

			if(commandStation != null)
				AddTrains(addressMap,
					from blockDefinition in LayoutModel.Components<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases) where blockDefinition.Power.PowerOriginComponentId == commandStation.Id && blockDefinition.Power.Type == LayoutPowerType.Digital && blockDefinition.Block.HasTrains select blockDefinition.Block,
					(loco, train) => new OnPoweredTrackLocomotiveAddressMapEntry(loco, train)
				);
		}

		[LayoutEvent("add-on-non-powered-tracks-locomotives-to-adddress-map")]
		private void AddOnNonPoweredTracksLocomotivesToAddressMap(LayoutEvent e) {
			LocomotiveAddressMap addressMap = (LocomotiveAddressMap)e.Info;

			AddTrains(addressMap,
				from blockDefinition in LayoutModel.Components<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases) where blockDefinition.Power.Type == LayoutPowerType.Disconnected && blockDefinition.Block.HasTrains select blockDefinition.Block,
				(loco, train) => new OnDisconnectedTrackLocomotiveAddressMapEntry(loco, train)
			);
		}

		[LayoutEvent("add-on-shelf-locomotives-to-address-map")]
		private void AddOnShelfLocomotivesToAddressMap(LayoutEvent e) {
			LocomotiveAddressMap addressMap = (LocomotiveAddressMap)e.Info;
			IModelComponentIsCommandStation commandStation = ExtractCommandStation(e);
			TrackGauges gauges = 0;

			if(commandStation != null) {
				var trackConnectors = from trackPowerConnector in LayoutModel.Components<LayoutTrackPowerConnectorComponent>(LayoutModel.ActivePhases) where trackPowerConnector.Inlet.ConnectedOutlet.ObtainablePowers.Any(power => power.PowerOriginComponentId == commandStation.Id) select trackPowerConnector;

				gauges = trackConnectors.Aggregate<LayoutTrackPowerConnectorComponent, TrackGauges>(0, (g, powerConnector) => g | powerConnector.Info.TrackGauge);

				// Pass on all locomotives in collection
				foreach(XmlElement locomotiveElement in LayoutModel.LocomotiveCollection.CollectionElement.GetElementsByTagName("Locomotive")) {
					LocomotiveInfo locomotive = new LocomotiveInfo(locomotiveElement);
					int locomotiveAddress = locomotive.AddressProvider.Unit;

					if(locomotiveAddress >= 0 && (locomotive.Guage & gauges) != 0 && !addressMap.ContainsKey(locomotiveAddress))
						addressMap.Add(locomotiveAddress, new OnShelfLocomotiveAddressMapEntry(locomotive));
				}
			}
		}

		Dictionary<IModelComponentIsCommandStation, OnTrackLocomotiveAddressMap> addressMapCache = new Dictionary<IModelComponentIsCommandStation, OnTrackLocomotiveAddressMap>();

		[LayoutEvent("get-on-track-locomotive-address-map")]
		private void GetOnTrackLocomotiveAddressMap(LayoutEvent e) {
			IModelComponentIsCommandStation commandStation = ExtractCommandStation(e);
			OnTrackLocomotiveAddressMap addressMap = null;

			if(commandStation != null) {
				if(!addressMapCache.TryGetValue(commandStation, out addressMap)) {
					addressMap = new OnTrackLocomotiveAddressMap();

					EventManager.Event(new LayoutEvent(commandStation, "add-on-powered-tracks-locomotives-to-address-map", null, addressMap));
					addressMapCache.Add(commandStation, addressMap);
				}
			}

			e.Info = addressMap;
		}

		// Trap events that invalidate the locomotive address map cache
		[LayoutEvent("train-is-removed")]
		[LayoutEvent("train-placed-on-track")]
		private void InvalidateAddressMap(LayoutEvent e) {
			addressMapCache.Clear();
		}

		[LayoutEvent("allocate-locomotive-address")]
		private void AllocateLocomotiveAddress(LayoutEvent e) {
			IModelComponentIsCommandStation commandStation = ExtractCommandStation(e);
			LocomotiveInfo locomotive = (LocomotiveInfo)e.Info;
			LocomotiveAddressMap addressMap = new LocomotiveAddressMap();
			DecoderWithNumericAddressTypeInfo decoderType = locomotive.DecoderType as DecoderWithNumericAddressTypeInfo;

			e.Info = (int)-1;		// Could not allocate address

			if(decoderType != null) {
				// Build address map of used addresses
				if(commandStation != null)
					BuildAddressMap(commandStation, addressMap);
				else {
					var commandStations = LayoutModel.Components<IModelComponentIsCommandStation>(LayoutModel.ActivePhases);

					// If there is only one command station in the layout (very probable)
					commandStation = commandStations.SingleOrDefault();

					foreach(var c in commandStations)
						BuildAddressMap(c, addressMap);
				}

				// At that stage the address map is complete
				int minAddress = locomotive.GetLowestAddress(commandStation);
				int maxAddress = locomotive.GetHighestAddress(commandStation);
				int numberOfAddresses = maxAddress - minAddress + 1;

				int address = minAddress;
				int bestOnNonPowerTrackAddress = -1;			// Address to use if need to reuse an address of a locomotive on a non-powered track
				int bestOnShelfAddress = -1;					// Address to use if need to reuse an address of a locomotive that is not on track

				for(int count = 0; count < numberOfAddresses; count++) {
					LocomotiveAddressMapEntryBase addressMapEntry;

					if(!addressMap.TryGetValue(address, out addressMapEntry)) {
						// Found unused address - this is the best
						e.Info = address;
						return;
					}
					else if(!(addressMapEntry is LocomotiveBusModuleAddressMapEntry) && !(addressMapEntry is OnPoweredTrackLocomotiveAddressMapEntry)) {
						var locomotiveAddressMapEntry = addressMapEntry as LocomotiveAddressMapEntry;

						if(locomotiveAddressMapEntry != null) {
							if(locomotiveAddressMapEntry is OnDisconnectedTrackLocomotiveAddressMapEntry)
								bestOnNonPowerTrackAddress = address;
							else if(locomotiveAddressMapEntry is OnShelfLocomotiveAddressMapEntry) {
								// TODO: The best one is the one that was placed on the shelf for the longest time...
								bestOnShelfAddress = address;
							}
						}
					}

					if(++address == maxAddress)
						address = minAddress;
				}
			}
		}

		private static void BuildAddressMap(IModelComponentIsCommandStation commandStation, LocomotiveAddressMap addressMap) {
			EventManager.Event(new LayoutEvent(commandStation, "add-on-powered-tracks-locomotives-to-address-map", null, addressMap));
			EventManager.Event(new LayoutEvent(commandStation, "add-command-station-loco-bus-to-address-map", null, addressMap));
			EventManager.Event(new LayoutEvent(commandStation, "add-on-non-powered-tracks-locomotives-to-adddress-map", null, addressMap));
			EventManager.Event(new LayoutEvent(commandStation, "add-on-shelf-locomotives-to-address-map", null, addressMap));
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
		private void extractLocomotiveState(LayoutEvent e) {
			TrainStateInfo trainState = null;

			if(e.Sender is TrainStateInfo)
				trainState = (TrainStateInfo)e.Sender;
			else if(e.Sender is TrainCommonInfo) {
				trainState = LayoutModel.StateManager.Trains[((TrainCommonInfo)e.Sender).Id];
				if(trainState == null)
					throw new TrainNotOnTrackException((TrainCommonInfo)e.Sender);
			}
			else if(e.Sender is XmlElement) {
				XmlElement element = (XmlElement)e.Sender;

				if(element.Name == "TrainState")
					trainState = new TrainStateInfo(element);
				else if(element.Name == "Locomotive") {
					LocomotiveInfo loco = new LocomotiveInfo(element);

					trainState = (TrainStateInfo)EventManager.Event(new LayoutEvent(loco, "extract-train-state"));
				}
				else if(element.Name == "Train") {
					TrainCommonInfo train = new TrainCommonInfo(element);

					trainState = (TrainStateInfo)EventManager.Event(new LayoutEvent(train, "extract-train-state"));
				}
				else
					throw new ArgumentException("Invalid XML element (not LocomotiveState, Locomotive or Train)");
			}
			else if(e.Sender is LocomotiveInfo) {
				trainState = LayoutModel.StateManager.Trains[((LocomotiveInfo)e.Sender).Id];
				if(trainState == null)
					throw new LocomotiveNotOnTrackException((LocomotiveInfo)e.Sender);
			}
			else
				throw new ArgumentException("Invalid argument (Not XmlElement, LocomotiveInfo, TrainCommonInfo, TrainInCollectionInfo or TrainStateInfo)");

			e.Info = trainState;
		}


		/// <summary>
		/// Remove an object (locomotive or locomotive set) from track
		/// </summary>
		[LayoutAsyncEvent("remove-from-track-request")]
		private async Task removeFromTrackRequest(LayoutEvent e0) {
			var e = (LayoutEvent<object, string>)e0;
			TrainStateInfo trainState = (TrainStateInfo)EventManager.Event(new LayoutEvent(e.Sender, "extract-train-state"));
			var ballonText = e.Info;

			if(ballonText != null) {
				var ballon = new LayoutBlockBallon() { Text = ballonText, RemoveOnClick = true, CancellationToken = e.GetCancellationToken() };

				LayoutBlockBallon.Show(trainState.LocomotiveBlock.BlockDefinintion, ballon);
				await ballon.Task;
			}

			LayoutModel.StateManager.Trains.Remove(trainState);
		}

		[LayoutAsyncEvent("wait-for-locomotive-placement")]
		private async Task waitForPlacement(LayoutEvent e0) {
			var e = (LayoutEvent<LayoutBlockDefinitionComponent, string>)e0;
			var blockDefinition = e.Sender;
			var message = e.Info;
			var trainId = XmlConvert.ToGuid(e.GetOption("TrainID"));

			var placeTrainBallon = new LayoutBlockBallon();

			placeTrainBallon.Text = message;
			placeTrainBallon.CancellationToken = e.GetCancellationToken();
			placeTrainBallon.FillColor = System.Drawing.Color.LightGreen;
			placeTrainBallon.TextColor = System.Drawing.Color.Black;
			placeTrainBallon.RemoveOnClick = true;
			placeTrainBallon.RemoveOnTrainDetected = true;
			LayoutBlockBallon.Show(blockDefinition, placeTrainBallon);

			try {
				await placeTrainBallon.Task;
			}
			catch(OperationCanceledException) {
				TrainStateInfo train = LayoutModel.StateManager.Trains[trainId];

				if(train != null)
					LayoutModel.StateManager.Trains.RemoveState(train);

				EventManager.Event(new LayoutEvent<Guid, bool>("free-owned-layout-locks", trainId, true));
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
			var blockDefinition = e.Sender;
			var collectionElement = e.Info;
			LayoutComponentConnectionPoint front = LayoutComponentConnectionPoint.T;
			string trainName = null;
			TrainLength trainLength;

			e.Info = null;

			if(e.HasOption("Train", "Length"))
				trainLength = TrainLength.Parse(e.GetOption(elementName: "Train", optionName: "Length"));
			else
				trainLength = TrainLength.Standard;

			if(e.HasOption("Train", "Front"))
				front = LayoutComponentConnectionPoint.Parse(e.GetOption(elementName: "Train", optionName: "Front"));
			else {
				if(collectionElement.Name == "Train") {
					object frontObject = EventManager.Event(new LayoutEvent(blockDefinition, "get-locomotive-front", null, collectionElement));

					if(frontObject == null)
						throw new OperationCanceledException("get-locomotive-front");

					front = (LayoutComponentConnectionPoint)frontObject;

					TrainInCollectionInfo trainInCollection = new TrainInCollectionInfo(collectionElement);

					trainLength = trainInCollection.Length;
					trainName = trainInCollection.Name;
				}
				else {
					Object trainFronAndLengthObject = EventManager.Event(new LayoutEvent(blockDefinition, "get-train-front-and-length", null, collectionElement));

					if(trainFronAndLengthObject == null)
						throw new OperationCanceledException("get-train-front-and-length");			// User canceled.

					TrainFrontAndLength t = (TrainFrontAndLength)trainFronAndLengthObject;

					front = t.Front;
					trainLength = t.Length;
					trainName = new LocomotiveInfo(collectionElement).Name;
				}
			}

			// Figure out the train name
			trainName = e.GetOption(elementName: "Train", optionName: "Name");

			if(trainName == null) {
				switch(collectionElement.Name) {
					case "Locomotive": trainName = new LocomotiveInfo(collectionElement).Name; break;
					case "Train": trainName = new TrainInCollectionInfo(collectionElement).Name; break;
					default:
						throw new LayoutException(EventManager.Instance, "No name was given to the new train");
				}
			}

			Guid trainID = Guid.Empty;

			if(collectionElement.Name == "Train") {
				TrainInCollectionInfo trainInCollection = new TrainInCollectionInfo(collectionElement);

				trainID = trainInCollection.Id;
			}

			TrainStateInfo train = LayoutModel.StateManager.Trains.Add(trainID);

			train.Name = trainName;
			train.Length = trainLength;

			if(collectionElement != null)
				train.AddLocomotiveCollectionElement(collectionElement, blockDefinition.Block, e.GetBoolOption("ValidateAddress", true));

			e.SetOption(elementName: "Train", optionName: "Front", value: front.ToString());
			e.Result = train;
		}

		[LayoutEvent("place-train-in-block")]
		private void placeTrainInBlock(LayoutEvent e0) {
			var e = (LayoutEvent<TrainStateInfo, LayoutBlockDefinitionComponent>)e0;
			var train = e.Sender;
			var blockDefinition = e.Info;
			var front = LayoutComponentConnectionPoint.Parse(e.GetOption(elementName: "Train", optionName: "Front"));

			TrainLocationInfo trainLocation = train.PlaceInBlock(blockDefinition.Block, front);

			// Figure out a block edge which the train could have crossed.
			LayoutBlockEdgeBase[] origins = blockDefinition.GetBlockEdges(blockDefinition.GetOtherConnectionPointIndex(front));

			if(origins.Length == 0) {
				origins = blockDefinition.GetBlockEdges(blockDefinition.GetConnectionPointIndex(front));

				if(origins.Length > 0) {
					trainLocation.BlockEdge = origins[0];
					train.LastBlockEdgeCrossingSpeed = -1;		// Did reverse from block edge in the front
				}
			}
			else {
				trainLocation.BlockEdge = origins[0];
				train.LastBlockEdgeCrossingSpeed = 1;			// Positive speed coming from block edge in other direction than front
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
		async Task<object> placeTrainRequest(LayoutEvent e) {
			LayoutBlockDefinitionComponent blockDefinition = (LayoutBlockDefinitionComponent)e.Sender;
			XmlElement collectionElement = (XmlElement)e.Info;

			e.Info = null;

			// Get lock on the block that the train is about to be placed, when the lock is obtained, the train is actually placed in the block
			LayoutLockRequest lockRequest = null;

			var createTrainEvent = new LayoutEvent<LayoutBlockDefinitionComponent, XmlElement, TrainStateInfo>("create-train", blockDefinition, collectionElement).CopyOptions(e, "Train");
			var train = (TrainStateInfo)EventManager.Event(createTrainEvent);

			if(blockDefinition.Block.LockRequest == null || !blockDefinition.Block.LockRequest.IsManualDispatchLock) {
				lockRequest = new LayoutLockRequest(train.Id);

				lockRequest.CancellationToken = e.GetCancellationToken();
				lockRequest.ResourceEntries.Add(blockDefinition.Block);
			}

			Task lockTask = EventManager.AsyncEvent(new LayoutEvent(lockRequest, "request-layout-lock-async").CopyOperationContext(e));

			if(!lockTask.IsCompleted)
				LayoutBlockBallon.Show(blockDefinition,
					new LayoutBlockBallon() { Text = "Do not place train on track yet!", CancellationToken = e.GetCancellationToken(),
						FillColor = System.Drawing.Color.Red, TextColor = System.Drawing.Color.Yellow });

			try {
				await lockTask;
			}
			catch(OperationCanceledException) {
				if(train != null)
					LayoutModel.StateManager.Trains.RemoveState(train);
				throw;
			}

			EventManager.Event(new LayoutEvent<TrainStateInfo, LayoutBlockDefinitionComponent>("place-train-in-block", train, blockDefinition).CopyOptions(createTrainEvent, "Train"));
			await EventManager.AsyncEvent(new LayoutEvent<LayoutBlockDefinitionComponent, string>("wait-for-locomotive-placement", blockDefinition, "Please place train " + train.Name + " on track").CopyOperationContext(e).SetOption("TrainID", train.Id));

			// At this stage the place where the train is about to be placed is locked, and train is supposed to be on track (at least the user has indicated that by
			// clicking on the ballon, or a train was detected in this block)

			EventManager.Event(new LayoutEvent<TrainStateInfo, LayoutBlockDefinitionComponent>("train-created", train, blockDefinition));
			EventManager.Event(new LayoutEvent<TrainStateInfo>("train-placed-on-track", train));

			return train;
		}

		[LayoutAsyncEvent("validate-and-place-train-request")]
		private async Task<object> validateAndPlaceTrain(LayoutEvent e0) {
			var e = (LayoutEvent<LayoutBlockDefinitionComponent, XmlElement>)e0;
			LayoutBlockDefinitionComponent blockDefinition = e.Sender;
			XmlElement placedElement = e.Info;
			CanPlaceTrainResult result;
			TrainStateInfo train = null;

			result = (CanPlaceTrainResult)EventManager.Event(new LayoutEvent(placedElement, "can-locomotive-be-placed", null, blockDefinition));

			if(result.ResolveMethod == CanPlaceTrainResolveMethod.ReprogramAddress) {
				var programmingState = new PlacedLocomotiveProgrammingState(placedElement, result.Locomotive, blockDefinition);

				train = (TrainStateInfo)await (Task<object>)EventManager.AsyncEvent(new LayoutEvent<PlacedLocomotiveProgrammingState>("placed-locomotive-address-programming", programmingState).CopyOptions(e, "Train").CopyOperationContext(e));

				if(train == null)
					result = (CanPlaceTrainResult)EventManager.Event(new LayoutEvent(placedElement, "can-locomotive-be-placed", null, blockDefinition));
			}

			if(result.ResolveMethod == CanPlaceTrainResolveMethod.Resolved) {
				train = (TrainStateInfo)await (Task<object>)EventManager.AsyncEvent(new LayoutEvent(blockDefinition, "place-train-request", null, placedElement).CopyOptions(e, "Train").CopyOptions(e).CopyOperationContext(e));

				EventManager.Event(new LayoutEvent(train, "set-train-initial-motion-direction-request"));
			}
			else
				throw new LayoutException("Train/Locomotive cannot be placed");

			return train;
		}

		[LayoutEvent("relocate-train-request")]
		private void relocateTrainRequest(LayoutEvent e) {
			TrainStateInfo train;
			LayoutBlockDefinitionComponent blockDefinition = (LayoutBlockDefinitionComponent)e.Info;
			LayoutComponentConnectionPoint front;

			if(e.Sender is XmlElement)
				train = new TrainStateInfo((XmlElement)e.Sender);
			else
				train = (TrainStateInfo)e.Sender;

			if(e.HasOption("Train", "Front"))
				front = LayoutComponentConnectionPoint.Parse(e.GetOption("Front", "Train"));
			else {
				Object frontObject = EventManager.Event(new LayoutEvent(blockDefinition, "get-locomotive-front", null, train.Element));

				if(frontObject == null)
					return;			// User canceled.

				front = (LayoutComponentConnectionPoint)frontObject;
			}

			train.EraseImage();

			IList<TrainLocationInfo> oldLocations = new List<TrainLocationInfo>(train.Locations);
			LayoutBlock[] blocksToUnlock = new LayoutBlock[oldLocations.Count];

			int i = 0;
			foreach(TrainLocationInfo oldTrainLocation in oldLocations) {
				blocksToUnlock[i++] = oldTrainLocation.Block;
				train.LeaveBlock(oldTrainLocation.Block, false);
			}

			EventManager.Event(new LayoutEvent(blocksToUnlock, "free-layout-lock"));

			// Get lock on the block that the train is about to be placed
			LayoutLockRequest lockRequest = new LayoutLockRequest(train.Id);

			lockRequest.ResourceEntries.Add(blockDefinition.Block);
			EventManager.Event(new LayoutEvent(lockRequest, "request-layout-lock"));

			// Create the new location for the train
			TrainLocationInfo trainLocation = train.PlaceInBlock(blockDefinition.Block, front);

			trainLocation.DisplayFront = front;
			EventManager.Event(new LayoutEvent(train, "train-relocated", null, blockDefinition.Block));

			// Figure out a block edge which the train could have crossed.
			LayoutBlockEdgeBase[] origins = blockDefinition.GetBlockEdges(blockDefinition.GetOtherConnectionPointIndex(front));

			if(origins.Length == 0) {
				origins = blockDefinition.GetBlockEdges(blockDefinition.GetConnectionPointIndex(front));

				if(origins.Length > 0) {
					trainLocation.BlockEdge = origins[0];
					train.LastBlockEdgeCrossingSpeed = -1;		// Did reverse from block edge in the front
				}
			}
			else {
				trainLocation.BlockEdge = origins[0];
				train.LastBlockEdgeCrossingSpeed = 1;			// Positive speed coming from block edge in other direction than front
			}

			train.FlushCachedValues();
			train.RefreshSpeedLimit();

			AutoExtendTrain(train, blockDefinition);
		}

		#endregion

		#region Locomotive address reprogramming handling

		[LayoutAsyncEvent("placed-locomotive-address-programming")]
		private async Task<object> placedLocomotiveAddressReprogramming(LayoutEvent e) {
			var programmingState = (PlacedLocomotiveProgrammingState)e.Sender;

			// Figure out the new address to assign to the locomotive
			object addressObject = EventManager.Event(new LayoutEvent(programmingState.PlacementLocation, "allocate-locomotive-address", null, programmingState.Locomotive));

			if(addressObject is Int32) {
				int address = (int)addressObject;

				programmingState.ProgrammingActions = new LayoutActionContainer<LocomotiveInfo>(programmingState.Locomotive);

				var changeAddressAction = (ILayoutLocomotiveAddressChangeAction)programmingState.ProgrammingActions.Add("set-address");

				changeAddressAction.Address = address;
				var train = (TrainStateInfo)await (Task<object>)EventManager.AsyncEvent(new LayoutEvent(programmingState, "program-locomotive").CopyOptions(e, "Train").CopyOperationContext(e));

				return train;
			}
			else
				throw new LayoutException("Unable to find a valid new address for locomotive '" + programmingState.Locomotive.Name);
		}

		#endregion

		#region Locomotive Programming

		enum CanUseForProgrammingResult {
			No, Yes, YesButHasOtherTrains
		}

		private CanUseForProgrammingResult CanUseForProgramming(LayoutBlockDefinitionComponent blockDefinition, LocomotiveInfo targetLocomotive = null) {
			if(blockDefinition != null && blockDefinition.PowerConnector.Inlet.ConnectedOutlet.ObtainablePowers.Any(p => p.Type == LayoutPowerType.Programmer)) {
				if(blockDefinition.PowerConnector.Locomotives.Any()) {
					// Region connected to the power has locomotives, if it has only one and this is the one about to get programmed that ok,
					// otherwise, return that this block can be used for programming, but it contains other trains.
					if(blockDefinition.PowerConnector.Locomotives.All(trainLocomotive => targetLocomotive != null && trainLocomotive.LocomotiveId == targetLocomotive.Id))
						return CanUseForProgrammingResult.Yes;
					else
						return CanUseForProgrammingResult.YesButHasOtherTrains;
				}
				else
					return CanUseForProgrammingResult.Yes;	// Can get power, and has no locomotives
			}
			else
				return CanUseForProgrammingResult.No;		// Block cannot get programming power.
		}

		private static Task ObtainProgrammingTracksLock(TrainStateInfo train, LayoutBlockDefinitionComponent programmingLocation, string ballonText, CancellationToken cancellationToken) {
			// Lock the blocks that get power from the inlet that the programming location gets its power from
			var lockRequest = GetProgrammingTracksLock(train.Id, programmingLocation, cancellationToken);

			var lockTask = EventManager.AsyncEvent(new LayoutEvent(lockRequest, "request-layout-lock-async"));

			if(!lockTask.IsCompleted) {
				var waitLockBallon = new LayoutBlockBallon() { Text = ballonText, CancellationToken = cancellationToken, FillColor = System.Drawing.Color.Red, TextColor = System.Drawing.Color.Yellow };

				LayoutBlockBallon.Show(programmingLocation, waitLockBallon);
			}

			// If block is locked to this train, but with a different lock request, release the old lock
			if(programmingLocation.Block.LockRequest != lockRequest && programmingLocation.Block.LockRequest.OwnerId == train.Id)
				EventManager.Event(new LayoutEvent<Guid, bool>("free-owned-layout-locks", train.Id, false));

			return lockTask;
		}

		private static LayoutLockRequest GetProgrammingTracksLock(Guid ownerId, LayoutBlockDefinitionComponent programmingLocation, CancellationToken canellationToken) {
			var lockRequest = new LayoutLockRequest(ownerId);

			lockRequest.Type = LayoutLockType.Programming;
			lockRequest.CancellationToken = canellationToken;
			lockRequest.ResourceEntries.Add(programmingLocation.PowerConnector.Blocks);

			var commandStationResource = (from power in programmingLocation.PowerConnector.Inlet.ConnectedOutlet.ObtainablePowers where power.Type == LayoutPowerType.Programmer select power.PowerOriginComponent).FirstOrDefault() as ILayoutLockResource;

			Debug.Assert(commandStationResource != null);
			lockRequest.ResourceEntries.Add(commandStationResource);
			return lockRequest;
		}

		[LayoutAsyncEvent("program-locomotive")]
		private async Task<object> programLocomotive(LayoutEvent e) {
			var programmingState = (LocomotiveProgrammingState)e.Sender;
			TrainStateInfo train = LayoutModel.StateManager.Trains[programmingState.Locomotive.Id];
			bool usePOM = true;

			if(train == null || !train.OnTrack)
				usePOM = false;				// Train not on track - no program on main
			else if(programmingState.ProgrammingActions.IsProgrammingTrackRequired())
				usePOM = false;

			if(!usePOM && programmingState.ProgrammingActions.IsTrackRequired()) {	// Need to be placed on track with programming power
				// Check if preferred placement location can be used for programming
				CanUseForProgrammingResult canUseForProgrammingResult = CanUseForProgramming(programmingState.PlacementLocation, programmingState.Locomotive);

				if(canUseForProgrammingResult == CanUseForProgrammingResult.Yes)
					programmingState.ProgrammingLocation = programmingState.PlacementLocation;
				else
					programmingState.ProgrammingLocation = (LayoutBlockDefinitionComponent)EventManager.Event(new LayoutEvent(this, "get-programming-location"));	// Ask the user where should the programming take place

				if(programmingState.ProgrammingLocation == null)
					throw new OperationCanceledException("program-locomotive");

				LayoutBlockDefinitionComponent programmingLocation = programmingState.ProgrammingLocation;
				ILayoutPower originalPower = programmingLocation.Power;

				if(train != null && programmingLocation.Block.Id != train.LocomotiveBlock.Id) {
					await EventManager.AsyncEvent(new LayoutEvent<object, string>("remove-from-track-request", train, "Remove locomotive to be programmed from track (and place it on programming track)").CopyOperationContext(e));
					train = null;
				}

				try {
					await EventManager.AsyncEvent(new LayoutEvent<object, object>("set-power", programmingLocation, LayoutPowerType.Programmer).CopyOperationContext(e));

					if(train == null) {
						var createTrainEvent = new LayoutEvent<LayoutBlockDefinitionComponent, XmlElement, TrainStateInfo>("create-train", programmingLocation, programmingState.Locomotive.Element).CopyOptions(e, "Train").SetOption("ValidateAddress", false);

						if(programmingState.PlacementLocation != null && programmingLocation.Id == programmingState.PlacementLocation.Id)
							createTrainEvent.CopyOptions(e, "Train");
						else {
							createTrainEvent.SetOption(elementName: "Train", optionName: "Front", value: programmingLocation.Track.ConnectionPoints[0].ToString());
							createTrainEvent.SetOption(elementName: "Train", optionName: "Length", value: TrainLength.LocomotiveOnly.ToString());

							e.GetOperationContext().AddPariticpant(programmingLocation);
						}

						train = (TrainStateInfo)EventManager.Event(createTrainEvent);
						await ObtainProgrammingTracksLock(train, programmingLocation, "Do not yet place locomotive on programming track", e.GetCancellationToken());
						EventManager.Event(new LayoutEvent<TrainStateInfo, LayoutBlockDefinitionComponent>("place-train-in-block", train, programmingLocation).CopyOptions(createTrainEvent, "Train").CopyOperationContext(e));
					}
					else
						await ObtainProgrammingTracksLock(train, programmingLocation, "Waiting to obtain lock on programming track region", e.GetCancellationToken());


					// Ask the user to put the locomotive on track (note that lockRequest is null, since the track is already locked)
					await EventManager.AsyncEvent(new LayoutEvent<LayoutBlockDefinitionComponent, string>("wait-for-locomotive-placement", programmingLocation, "Place locomotive on programming track").CopyOptions(e).SetOption("TrainID", train.Id).CopyOperationContext(e));

					// When track is detected or when the user hit the ballon, wait 500ms and then start the actual programming
					await Task.Delay(500, e.GetCancellationToken());

					var commandStation = programmingLocation.Power.PowerOriginComponent as IModelComponentCanProgramLocomotives;

					var r = (LayoutActionFailure)await (Task<object>)EventManager.AsyncEvent(new LayoutEvent<ILayoutActionContainer, IModelComponentCanProgramLocomotives>("do-command-station-actions", programmingState.ProgrammingActions, commandStation).SetOption("UsePOM", false));

					if(r != null)
						Error(r.ToString());

					// If programming location is not same as original location (or where the train is supposed to be), remove the train from the programming location
					if(programmingState.PlacementLocation == null || programmingState.PlacementLocation.Id != programmingLocation.Id) {
						await EventManager.AsyncEvent(new LayoutEvent<object, string>("remove-from-track-request", train, "Remove locomotive from programming track"));
						train = null;
					}

					if(r != null)
						throw new LayoutException("Locomotive programming failed");
				}
				finally {
					if(originalPower != programmingLocation.Power) {
						Trace.WriteLine("Set power back to " + originalPower.Name + " type: " + originalPower.Type.ToString());
						Task t = EventManager.AsyncEvent(new LayoutEvent<object, object>("set-power", programmingLocation, originalPower));
					}

					Trace.WriteLine("Free programming locked");

					EventManager.Event(new LayoutEvent(GetProgrammingTracksLock(Guid.Empty, programmingLocation, CancellationToken.None), "free-layout-lock"));

					if(programmingState.PlacementLocation == null || programmingLocation.Id != programmingState.PlacementLocation.Id)
						e.GetOperationContext().RemoveParticipant(programmingLocation);
				}
			}
			else {
				IModelComponentCanProgramLocomotives commandStation = null;

				if(train != null)
					commandStation = train.CommandStation as IModelComponentCanProgramLocomotives;

				var r = (LayoutActionFailure)await (Task<object>)EventManager.AsyncEvent(new LayoutEvent<ILayoutActionContainer, IModelComponentCanProgramLocomotives>("do-command-station-actions", programmingState.ProgrammingActions, commandStation).SetOption("UsePOM", true));

				if(r != null)
					Error(r.ToString());
			}

			return train;
		}

		#endregion

		#region Train Management

		private void AutoExtendTrain(TrainStateInfo train, LayoutBlock oldBlock, LayoutBlockEdgeBase blockEdge) {
			LayoutBlock newBlock = oldBlock.OtherBlock(blockEdge);

			if(newBlock.BlockDefinintion.Info.IsOccupancyDetectionBlock && !newBlock.HasTrains && (newBlock.BlockDefinintion.Info.TrainDetected || newBlock.BlockDefinintion.Info.TrainWillBeDetected) && !newBlock.IsLocked) {
				// Get lock on the block that the train is about to be placed
				LayoutLockRequest lockRequest = new LayoutLockRequest(train.Id);

				lockRequest.ResourceEntries.Add(newBlock);

				bool granted = (bool)EventManager.Event(new LayoutEvent(lockRequest, "request-layout-lock"));

				Debug.Assert(granted);

				TrainLocationInfo oldTrainLocation = train.LocationOfBlock(oldBlock);
				TrainLocationInfo newTrainLocation = null;

				train.LastBlockEdgeCrossingSpeed = 1;

				if(oldBlock.BlockDefinintion.ContainsBlockEdge(oldTrainLocation.DisplayFront, blockEdge)) {
					newTrainLocation = train.EnterBlock(TrainPart.Locomotive, newBlock, blockEdge, "train-extended");

					train.LastCrossedBlockEdge = blockEdge;

					// Extended train location front should point to the direction opposite to the one of the block edge
					// and direction should be Backward
					if(newBlock.BlockDefinintion.ContainsBlockEdge(0, blockEdge))
						newTrainLocation.DisplayFront = newBlock.BlockDefinintion.Track.ConnectionPoints[1];
					else
						newTrainLocation.DisplayFront = newBlock.BlockDefinintion.Track.ConnectionPoints[0];
				}
				else {
					newTrainLocation = train.EnterBlock(TrainPart.LastCar, newBlock, blockEdge, "train-extended");

					// Extended train location front should point to the track contact direction and the track contact
					// state direction should be forward
					if(newBlock.BlockDefinintion.ContainsBlockEdge(0, blockEdge))
						newTrainLocation.DisplayFront = newBlock.BlockDefinintion.Track.ConnectionPoints[0];
					else
						newTrainLocation.DisplayFront = newBlock.BlockDefinintion.Track.ConnectionPoints[1];

					int iCpIndex = newBlock.BlockDefinintion.GetConnectionPointIndex(newTrainLocation.DisplayFront);
					LayoutBlockEdgeBase[] otherBlockEdges = newBlock.BlockDefinintion.GetBlockEdges(1 - iCpIndex);

					if(otherBlockEdges == null)
						newTrainLocation.BlockEdge = null;
					else
						newTrainLocation.BlockEdge = otherBlockEdges[0];
				}

				foreach(LayoutBlockEdgeBase newBlockEdge in newBlock.BlockEdges)
					AutoExtendTrain(train, newBlock, newBlockEdge);
			}
		}

		private void AutoExtendTrain(TrainStateInfo train, LayoutBlockDefinitionComponent blockDefinition) {
			if(blockDefinition.Info.IsOccupancyDetectionBlock) {
				// Automatically extend train to neighboring occupacy detection blocks in which train is detected
				foreach(LayoutBlockEdgeBase blockEdge in blockDefinition.Block.BlockEdges)
					AutoExtendTrain(train, blockDefinition.Block, blockEdge);
			}
		}

		[LayoutEvent("request-auto-train-extend")]
		private void requestAutoTrainExtend(LayoutEvent e) {
			TrainStateInfo train = (TrainStateInfo)e.Sender;
			LayoutBlockDefinitionComponent blockDefinition = (LayoutBlockDefinitionComponent)e.Info;

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
		[LayoutEvent("set-train-speed-request")]
		private void setLocomotiveSpeedRequest(LayoutEvent e) {
			TrainStateInfo train = (TrainStateInfo)EventManager.Event(new LayoutEvent(e.Sender, "extract-train-state"));
			int speed = (int)e.Info;

			foreach(TrainLocomotiveInfo trainLoco in train.Locomotives)
				EventManager.Event(new LayoutEvent(trainLoco.Locomotive, "locomotive-motion-command", null,
					(trainLoco.Orientation == LocomotiveOrientation.Forward) ? speed : -speed).SetCommandStation(train));

			train.SetSpeedValue(speed);
		}

		/// <summary>
		/// Event which is sent to reverse the train motion direction.
		/// </summary>
		/// <param name="e.Sender">The train to be reversed</param>
		[LayoutEvent("reverse-train-motion-direction-request")]
		private void reverseTrainMotionDirectionRequest(LayoutEvent e) {
			TrainStateInfo trainState = (TrainStateInfo)EventManager.Event(new LayoutEvent(e.Sender, "extract-train-state"));

			foreach(TrainLocomotiveInfo trainLoco in trainState.Locomotives)
				EventManager.Event(new LayoutEvent(trainLoco.Locomotive, "locomotive-reverse-motion-direction-command"));
		}

		/// <summary>
		/// 
		/// </summary>
		[LayoutEvent("set-train-initial-motion-direction-request")]
		private void setTrainInitialDirection(LayoutEvent e) {
			TrainStateInfo trainState = (TrainStateInfo)EventManager.Event(new LayoutEvent(e.Sender, "extract-train-state"));

			foreach(TrainLocomotiveInfo trainLoco in trainState.Locomotives) {
				if(trainLoco.Orientation == LocomotiveOrientation.Backward)
					EventManager.Event(new LayoutEvent(trainLoco.Locomotive, "locomotive-reverse-motion-direction-command"));
			}

			trainState.MotionDirection = LocomotiveOrientation.Forward;
		}


		/// <summary>
		/// This event is sent by the command station component when it get an idiciation of a change
		/// in a locomotive motion parameters
		/// </summary>
		/// <param name="e.Sender">The command station component</param>
		/// <param name="e.Info">The speed</param>
		/// The event document has a element named Address with an attribute Unit which indicate the
		/// locomotive address whose address is being changed
		[LayoutEvent("locomotive-motion-notification")]
		private void locomotiveMotionNotification(LayoutEvent e) {
			int unit = XmlConvert.ToInt32(e.GetOption("Unit", "Address"));
			int speed = (int)e.Info;
			IModelComponentIsCommandStation commandStation = (IModelComponentIsCommandStation)e.Sender;

			var addressMap = (OnTrackLocomotiveAddressMap)EventManager.Event(new LayoutEvent(commandStation, "get-on-track-locomotive-address-map"));
			var addressMapEntry = addressMap[unit];

			if(addressMapEntry == null)
				Error("Command station " + commandStation.NameProvider.Name + " reported motion of locomotive with address " + unit + ". This locomotive is not registered as being on the layout!");
			else {
				if(addressMapEntry.Train.Locomotives.Count > 1)
					Warning("Command station " + commandStation.NameProvider.Name + " reported change in speed/direction of a single locomotive set member (address " + unit + ")");

				addressMapEntry.Train.SetSpeedValue(speed);
			}
		}

		[LayoutEvent("train-speed-changed")]
		private void trainSpeedChanged(LayoutEvent e) {
			TrainStateInfo trainState = (TrainStateInfo)e.Sender;

			foreach(TrainLocationInfo trainLocation in trainState.Locations) {
				LayoutBlock block = trainLocation.Block;

				if(block.BlockDefinintion != null)
					block.BlockDefinintion.OnComponentChanged();
			}
		}

		#endregion

		#region Lights & Functions

		[LayoutEvent("set-train-lights-request")]
		private void setLocomotiveLightsRequest(LayoutEvent e) {
			TrainStateInfo trainState = (TrainStateInfo)EventManager.Event(new LayoutEvent(e.Sender, "extract-train-state"));
			bool lights = (bool)e.Info;

			if(lights != trainState.Lights) {
				foreach(TrainLocomotiveInfo trainLoco in trainState.Locomotives) {
					LocomotiveInfo loco = trainLoco.Locomotive;

					trainState.SetLightsValue(lights);

					if(loco.HasLights)
						EventManager.Event(new LayoutEvent(loco, "set-locomotive-lights-command", null, lights).SetCommandStation(trainState));
				}
			}
		}

		[LayoutEvent("toggle-locomotive-lights-notification")]
		private void toggleLocomotiveLightsNotification(LayoutEvent e) {
			int unit = XmlConvert.ToInt32(e.GetOption("Unit", "Address"));
			IModelComponentIsCommandStation commandStation = (IModelComponentIsCommandStation)e.Sender;

			var addressMap = (OnTrackLocomotiveAddressMap)EventManager.Event(new LayoutEvent(commandStation, "get-on-track-locomotive-address-map"));
			var addressMapEntry = addressMap[unit];

			if(addressMapEntry == null)
				Error("Command station " + commandStation.NameProvider.Name + " reported light status of locomotive with address " + unit + ". This locomotive is not registered as being on the layout!");
			else {
				TrainStateInfo train = addressMapEntry.Train;

				if(train.Locomotives.Count > 1)
					Warning("Command station " + commandStation.NameProvider.Name + " reported change in light status of a single locomotive set member (address " + unit + ")");

				train.SetLightsValue(!train.Lights);
			}
		}

		[LayoutEvent("set-locomotive-lights-notification")]
		private void setLocomotiveLightsNotification(LayoutEvent e) {
			int unit = XmlConvert.ToInt32(e.GetOption("Unit", "Address"));
			bool lights = (bool)e.Info;
			IModelComponentIsCommandStation commandStation = (IModelComponentIsCommandStation)e.Sender;

			var addressMap = (OnTrackLocomotiveAddressMap)EventManager.Event(new LayoutEvent(commandStation, "get-on-track-locomotive-address-map"));
			var addressMapEntry = addressMap[unit];

			if(addressMapEntry == null)
				Error("Command station " + commandStation.NameProvider.Name + " reported light status of locomotive with address " + unit + ". This locomotive is not registered as being on the layout!");
			else {
				TrainStateInfo train = addressMapEntry.Train;

				if(train.Locomotives.Count > 1)
					Warning("Command station " + commandStation.NameProvider.Name + " reported change in light status of a single locomotive set member (address " + unit + ")");

				train.SetLightsValue(lights);
			}
		}

		[LayoutEvent("set-locomotive-function-state-request")]
		private void setLocomotiveFunctionStateRequest(LayoutEvent e) {
			TrainStateInfo train = (TrainStateInfo)EventManager.Event(new LayoutEvent(e.Sender, "extract-train-state"));
			bool state = (bool)e.Info;
			XmlElement functionElement = e.Element["Function"];
			String functionName = functionElement.GetAttribute("Name");
			Guid locomotiveId = Guid.Empty;

			if(functionElement.HasAttribute("LocomotiveID"))
				locomotiveId = XmlConvert.ToGuid(functionElement.GetAttribute("LocomotiveID"));

			if(locomotiveId == Guid.Empty || state != train.GetFunctionState(functionName, locomotiveId, false)) {
				foreach(TrainLocomotiveInfo trainLoco in train.Locomotives) {
					LocomotiveInfo loco = trainLoco.Locomotive;

					if(locomotiveId == Guid.Empty || locomotiveId == loco.Id) {
						train.SetLocomotiveFunctionStateValue(functionName, locomotiveId, state);
						EventManager.Event(new LayoutEvent(loco, "set-locomotive-function-state-command", null, functionName).SetOption("FunctionState", state).SetCommandStation(train));

						if(locomotiveId != Guid.Empty)
							break;
					}
				}
			}
		}

		[LayoutEvent("trigger-locomotive-function-request")]
		private void triggerLocomotiveFunctionRequest(LayoutEvent e) {
			TrainStateInfo train = (TrainStateInfo)EventManager.Event(new LayoutEvent(e.Sender, "extract-train-state"));
			XmlElement functionElement = e.Element["Function"];
			String functionName = functionElement.GetAttribute("Name");
			Guid locomotiveId = Guid.Empty;

			if(functionElement.HasAttribute("LocomotiveID"))
				locomotiveId = XmlConvert.ToGuid(functionElement.GetAttribute("LocomotiveID"));

			foreach(TrainLocomotiveInfo trainLoco in train.Locomotives) {
				LocomotiveInfo loco = trainLoco.Locomotive;

				if(locomotiveId == Guid.Empty || locomotiveId == loco.Id)
					EventManager.Event(new LayoutEvent(loco, "trigger-locomotive-function-command", null, functionName).SetCommandStation(train));
			}
		}

		[LayoutEvent("set-locomotive-function-state-notification")]
		private void setLocomotiveFunctionStateNotification(LayoutEvent e) {
			int unit = XmlConvert.ToInt32(e.GetOption("Unit", "Address"));
			bool state = (bool)e.Info;
			IModelComponentIsCommandStation commandStation = (IModelComponentIsCommandStation)e.Sender;

			var addressMap = (OnTrackLocomotiveAddressMap)EventManager.Event(new LayoutEvent(commandStation, "get-on-track-locomotive-address-map"));
			var addressMapEntry = addressMap[unit];

			if(addressMapEntry == null)
				Error("Command station " + commandStation.NameProvider.Name + " reported light status of locomotive with address " + unit + ". This locomotive is not registered as being on the layout!");
			else {
				TrainStateInfo train = addressMapEntry.Train;

				foreach(TrainLocomotiveInfo trainLoco in train.Locomotives) {
					if(trainLoco.Locomotive.AddressProvider.Unit == unit) {
						train.SetLocomotiveFunctionStateValue(e.GetIntOption("Function", "Number"), trainLoco.LocomotiveId, state);
						break;
					}
				}
			}
		}

		#endregion

		#region Speed limit

		[LayoutEvent("locomotive-configuration-changed")]
		private void locomotiveConfigurationChanged(LayoutEvent e) {
			LocomotiveInfo loco = (LocomotiveInfo)e.Sender;

			if(LayoutController.IsOperationMode) {
				foreach(XmlElement trainElement in LayoutModel.StateManager.Trains.Element) {
					TrainStateInfo train = new TrainStateInfo(trainElement);

					foreach(TrainLocomotiveInfo trainLocomotive in train.Locomotives) {
						if(trainLocomotive.LocomotiveId == loco.Id)
							train.FlushCachedValues();
					}
				}
			}

			foreach(XmlElement trainElement in LayoutModel.LocomotiveCollection.CollectionElement.GetElementsByTagName("Train")) {
				TrainStateInfo train = new TrainStateInfo(trainElement);

				foreach(TrainLocomotiveInfo trainLocomotive in train.Locomotives) {
					if(trainLocomotive.LocomotiveId == loco.Id)
						train.FlushCachedValues();
				}
			}
		}


		#endregion

		#region Train controller handling

		[LayoutEvent("train-controller-activated")]
		private void trainControllerActivated(LayoutEvent e0) {
			var e = (LayoutEvent<TrainStateInfo>)e0;
			var train = e.Sender;

			foreach(var trainLoco in train.Locomotives)
				EventManager.Event(new LayoutEvent<TrainLocomotiveInfo, TrainStateInfo>("locomotive-controller-activated", trainLoco, train));
		}

		[LayoutEvent("train-controller-deactivated")]
		private void trainControllerDeactivated(LayoutEvent e0) {
			var e = (LayoutEvent<TrainStateInfo>)e0;
			var train = e.Sender;

			foreach(var trainLoco in train.Locomotives)
				EventManager.Event(new LayoutEvent<TrainLocomotiveInfo, TrainStateInfo>("locomotive-controller-deactivated", trainLoco, train));
		}

		#endregion

		#region State reconstuction operations

		class TrackContactCrossTimeSorter : IComparer {

			public int Compare(object oLocation1, object oLocation2) {
				TrainLocationInfo l1 = (TrainLocationInfo)oLocation1;
				TrainLocationInfo l2 = (TrainLocationInfo)oLocation2;

				if(l1.BlockEdgeCrossingTime > l2.BlockEdgeCrossingTime)
					return 1;
				else if(l1.BlockEdgeCrossingTime < l2.BlockEdgeCrossingTime)
					return -1;
				else
					return 0;
			}
		}

		[LayoutEvent("rebuild-layout-state", Order = 0)]
		private void rebuildComponents(LayoutEvent e) {
			var phase = e.GetPhases();
			bool ok = true;

			if(!rebuildComponentsState(phase))
				ok = false;

			e.Info = ok;
		}

		[LayoutEvent("rebuild-layout-state", Order = 1000)]
		private void rebuildLayoutState(LayoutEvent e) {
			LayoutPhase phase = e.GetPhases();
			bool ok = true;

			try {
				if(!rebuildTrainState(phase))
					ok = false;

				if(!verifyTrackContactState(phase))
					ok = false;

				if(!clearTrainDetectionBlocks(phase))
					ok = false;
			}
			catch(Exception ex) {
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
			foreach(LayoutBlock block in LayoutModel.Blocks)
				block.ClearTrains();

			foreach(XmlElement trainStateElement in LayoutModel.StateManager.Trains.Element) {
				bool validTrainState = true;
				TrainStateInfo trainState = new TrainStateInfo(trainStateElement);

				foreach(TrainLocomotiveInfo trainLoco in trainState.Locomotives) {
					if(trainLoco.Locomotive == null && LayoutModel.LocomotiveCollection[trainLoco.CollectionElementId] == null) {
						// Locomotive not found
						Warning("Locomotive " + trainState.Name + " cannot be found in your locomotive collection, it has been removed from track");
						invalidStateElements.Add(trainStateElement);
						validTrainState = false;
					}
				}


				if(validTrainState) {
					foreach(TrainLocationInfo trainLocation in trainState.Locations) {
						if(trainLocation.Block == null || trainLocation.BlockEdge == null) {
							Warning("The location where " + trainState.DisplayName + " used to be cannot be found. Please indicate its position again (or remove it from track)");
							invalidStateElements.Add(trainStateElement);
							validTrainState = false;
						}
					}
				}

				if(validTrainState) {
					foreach(TrainLocationInfo trainLocation in trainState.Locations) {
						if(trainLocation.BlockEdgeId == Guid.Empty)
							noTrackContactCrossTrainLocations.Add(trainLocation);
						else
							trainLocations.Add(trainLocation);
					}
				}
			}

			foreach(XmlElement invalidStateElement in invalidStateElements) {
				if(invalidStateElement.ParentNode != null)
					invalidStateElement.ParentNode.RemoveChild(invalidStateElement);
			}

			// Now place all the locomotives that should be placed. In order to ensure correct placement, sort the train locations
			// in acending order based on track contact crossing time. locations which were placed will be placed first
			foreach(TrainLocationInfo trainLocation in noTrackContactCrossTrainLocations) {
				// Get lock on the block that the train is about to be placed
				LayoutLockRequest lockRequest = new LayoutLockRequest(trainLocation.Train.Id);

				lockRequest.ResourceEntries.Add(trainLocation.Block);
				EventManager.Event(new LayoutEvent(lockRequest, "request-layout-lock"));

				trainLocation.Block.AddTrain(trainLocation);
			}

			trainLocations.Sort(new TrackContactCrossTimeSorter());

			foreach(TrainLocationInfo trainLocation in trainLocations) {
				LayoutLockRequest lockRequest = new LayoutLockRequest(trainLocation.Train.Id);

				lockRequest.ResourceEntries.Add(trainLocation.Block);
				EventManager.Event(new LayoutEvent(lockRequest, "request-layout-lock"));

				trainLocation.Block.AddTrain(trainLocation);

				if(trainLocation.Block.BlockDefinintion != null)
					trainLocation.Train.RefreshSpeedLimit();
			}

			LayoutModel.StateManager.Trains.RebuildIdMap();

			return (invalidStateElements.Count == 0);	// Fail if any invaid element was found
		}

		[LayoutEvent("enter-operation-mode", Order = 20000)]
		private void enterOperationModeAndInitializeTrains(LayoutEvent e) {
			foreach(XmlElement trainStateElement in LayoutModel.StateManager.Trains.Element) {
				TrainStateInfo train = new TrainStateInfo(trainStateElement);

				if(train.Speed != 0)
					train.SpeedInSteps = 0;
			}
		}

		public bool rebuildComponentsState(LayoutPhase phase) {
			ArrayList removeList = new ArrayList();

			LayoutModel.StateManager.Components.IdToComponentStateElement.Clear();

			foreach(XmlElement componentStateElement in LayoutModel.StateManager.Components.Element) {
				Guid componentID = XmlConvert.ToGuid(componentStateElement.GetAttribute("ID"));
				var component = LayoutModel.Component<ModelComponent>(componentID, phase);
				bool validStateComponent = true;
				var topicRemoveList = new List<XmlElement>();

				if(component != null) {
					foreach(XmlElement componentStateTopicElement in componentStateElement) {
						if((bool)EventManager.Event(new LayoutEvent(componentStateTopicElement, "verify-component-state-topic", null, true)) == false)
							topicRemoveList.Add(componentStateTopicElement);
					}

					foreach(XmlElement topicElementToRemove in topicRemoveList)
						componentStateElement.RemoveChild(topicElementToRemove);

					if(componentStateElement.ChildNodes.Count == 0)		// No topics, remove it
						validStateComponent = false;

					if(validStateComponent)
						LayoutModel.StateManager.Components.IdToComponentStateElement.Add(componentID, componentStateElement);
				}
				else {
					Warning("Component was removed from layout - its runtime state is ignored");
					validStateComponent = false;
				}

				if(!validStateComponent)
					removeList.Add(componentStateElement);
			}

			foreach(XmlElement componentStateElement in removeList)
				LayoutModel.StateManager.Components.Element.RemoveChild(componentStateElement);

			return removeList.Count == 0;
		}

		private bool verifyTrackContactState(LayoutPhase phase) {
			bool ok = true;

			foreach(LayoutTrackContactComponent trackContact in LayoutModel.Components<LayoutTrackContactComponent>(phase)) {
				if(LayoutModel.StateManager.Components.Contains(trackContact, "TrainPassing")) {
					XmlElement trackContactPassingElement = LayoutModel.StateManager.Components.StateOf(trackContact, "TrainPassing");
					TrackContactPassingStateInfo trackContactPassingState = new TrackContactPassingStateInfo(trackContactPassingElement);

					if(trackContactPassingState.Train == null) {
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
			foreach(LayoutBlockDefinitionComponent blockDefinition in LayoutModel.Components<LayoutBlockDefinitionComponent>(phase)) {
				if(blockDefinition.Info.IsOccupancyDetectionBlock) {
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
			foreach(LayoutBlock block in LayoutModel.Blocks)
				block.ClearTrains();

			LayoutModel.StateManager.DocumentElement.RemoveAll();
		}

		#endregion
	}
}
