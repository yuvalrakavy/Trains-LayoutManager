using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using MethodDispatcher;
using LayoutManager.Model;
using LayoutManager.Components;
using LayoutManager.CommonUI;

namespace LayoutManager.Tools {
    /// <summary>
    /// Summary description for LocomotiveTools.
    /// </summary>
    [LayoutModule("Locomotive Operation Tools", UserControl = false)]
    public class LocomotiveOperationTools : ILayoutModuleSetup {
        private enum PlacementProblem {
            NoProblem, NoAddress, AddressAlreadyUsed, Unknown
        };

        /// <summary>
        /// Build the menu with suggested blocks on which locomotives can be placed on track
        /// If the blocks are all within a single area, a one level menu is builts. If the blocks
        /// are in more than one area, then two level menu is builts. The first with the area names
        /// and the second level is with the block names.
        /// </summary>
        [LayoutEvent("add-placeble-blocks-menu-entries")]
        private void AddPlacebleBlocksMenuEntries(LayoutEvent e) {
            var placedElement = Ensure.NotNull<XmlElement>(e.Sender);
            var m = Ensure.ValueNotNull<MenuOrMenuItem>(e.Info);
            var result = Ensure.NotNull<CanPlaceTrainResult>(EventManager.Event("can-locomotive-be-placed-on-track", placedElement));

            if (result.Status != CanPlaceTrainStatus.CanPlaceTrain) {
                var problemItem = new LayoutMenuItem(result.ToString()) {
                    Enabled = false
                };
                m.Items.Add(problemItem);
                return;
            }

            PlacementProblem problem = PlacementProblem.NoProblem;
            string? errorMessage = null;
            var areaToBlockList = new Dictionary<LayoutModelArea, List<LayoutBlockDefinitionComponent>>();

            foreach (LayoutBlockDefinitionComponent blockInfo in LayoutModel.Components<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases)) {
                if (!blockInfo.Block.HasTrains && blockInfo.Info.SuggestForPlacement) {
                    result = EventManager.Event<XmlElement, LayoutBlockDefinitionComponent, CanPlaceTrainResult>("can-locomotive-be-placed", placedElement, blockInfo)!;

                    problem = PlacementProblem.NoProblem;

                    if (!result.CanBeResolved) {
                        problem = PlacementProblem.Unknown;
                        errorMessage = result.ToString();
                        break;
                    }
                    else {
                        if (!areaToBlockList.TryGetValue(blockInfo.Spot.Area, out List<LayoutBlockDefinitionComponent>? blocksInArea)) {
                            blocksInArea = new List<LayoutBlockDefinitionComponent>();
                            areaToBlockList[blockInfo.Spot.Area] = blocksInArea;
                        }

                        blocksInArea.Add(blockInfo);
                        errorMessage = null;
                    }
                }
            }

            if (problem == PlacementProblem.NoProblem) {
                if (areaToBlockList.Count == 0) {
                    var problemItem = new LayoutMenuItem("No empty block was found") {
                        Enabled = false
                    };
                    m.Items.Add(problemItem);
                }
                else if (areaToBlockList.Count == 1) {
                    // If there is only block info in one area, just add those to the menu
                    foreach (var blockInfoList in areaToBlockList.Values) {
                        blockInfoList.Sort((b1, b2) => b1.NameProvider.Name.CompareTo(b2.NameProvider.Name));

                        foreach (var blockInfo in blockInfoList)
                            m.Items.Add(new PlaceOnTrackMenuItem(placedElement, blockInfo));
                    }
                }
                else {
                    LayoutModelArea[] areas = new LayoutModelArea[areaToBlockList.Count];

                    areaToBlockList.Keys.CopyTo(areas, 0);

                    Array.Sort(areas, (a1, a2) => a1.Name.CompareTo(a2.Name));

                    foreach (LayoutModelArea area in areas) {
                        var areaMenuItem = new LayoutMenuItem(area.Name);
                        var blockInfoList = areaToBlockList[area];

                        blockInfoList.Sort((b1, b2) => b1.NameProvider.Name.CompareTo(b2.NameProvider.Name));

                        foreach (LayoutBlockDefinitionComponent blockInfo in blockInfoList)
                            areaMenuItem.DropDownItems.Add(new PlaceOnTrackMenuItem(placedElement, blockInfo));

                        m.Items.Add(areaMenuItem);
                    }
                }
            }
            else {
                var problemItem = new LayoutMenuItem(errorMessage ?? "bug: errorMessage was not specified") {
                    Enabled = false
                };
                m.Items.Add(problemItem);
            }
        }

        [LayoutEvent("show-locomotive-controller")]
        private void ShowLocomotiveController(LayoutEvent e) {
            TrainStateInfo trainState = EventManager.Event<object, object, TrainStateInfo>("extract-train-state", e.Sender)!;

            if (trainState.NotManaged)
                throw new LocomotiveNotManagedException(trainState.Locomotives[0].Locomotive);

            if (EventManager.Event(new LayoutEvent("activate-locomotive-controller", trainState)) == null) {
                Dialogs.LocomotiveController locoController = new(trainState);

                locoController.Show();
            }
        }

        /// <summary>
        /// Default handler for adding menu items to the locomotive collection context
        /// menu.
        /// </summary>
        [LayoutEvent("add-locomotive-collection-operation-context-menu-entries")]
        protected void AddLocomotiveCollectionOperationContextMenuEntries(LayoutEvent e) {
            var placedElement = Ensure.NotNull<XmlElement>(e.Sender);
            var m = Ensure.ValueNotNull<MenuOrMenuItem>(e.Info);
            var train = LayoutModel.StateManager.Trains[LayoutModel.LocomotiveCollection.GetElementId(placedElement)];

            if (train == null) {
                var context = LayoutOperationContext.GetPendingOperation("TrainPlacement", new LayoutXmlWithIdWrapper(placedElement));

                if (context == null) {
                    var placeOnTrackItem = new LayoutMenuItem("Place on track");

                    EventManager.Event(new LayoutEvent("add-placeble-blocks-menu-entries", placedElement, new MenuOrMenuItem(placeOnTrackItem)));
                    placeOnTrackItem.Enabled = placeOnTrackItem.DropDownItems.Count > 0;
                    m.Items.Add(placeOnTrackItem);
                }
                else
                    m.Items.Add(new LayoutMenuItem($"Cancel {context.Description}", null, (_, _) => context.Cancel()));
            }
            else {
                var context = LayoutOperationContext.GetPendingOperation("TrainPlacement", new LayoutXmlWithIdWrapper(placedElement));

                if (context != null)
                    m.Items.Add(new LayoutMenuItem($"Cancel {context.Description}", null, (_, _) => context.Cancel()));

                if (train.IsPowered)
                    m.Items.Add(new ShowLocomotiveControllerMenuItem(train));

                m.Items.Add(new TrainPropertiesMenuItem(train));

                if (train.OnTrack)
                    m.Items.Add(new LocateLocomotiveMenuItem(train.LocomotiveBlock!.BlockDefinintion));

                if (LayoutOperationContext.HasPendingOperation("TrainRemoval", train))
                    m.Items.Add(new CancelTrainRemovalMenuItem(train));
                else if (train.OnTrack)
                    m.Items.Add(new RemoveFromTrackMenuItem(train));
            }

            if (placedElement.Name == "Locomotive") {
                var locomotive = new LocomotiveInfo(placedElement);
                var context = LayoutOperationContext.GetPendingOperation("LocomotiveProgramming", locomotive);

                if (context != null)
                    m.Items.Add(new LayoutMenuItem($"Cancel {context.Description}", null, (_, _) => context.Cancel()));
                else {
                    var locomotiveProgrammingMenuItem = new LayoutMenuItem("&Program locomotive");

                    EventManager.Event(new LayoutEvent("add-locomotive-programming-menu-entries", locomotive, new MenuOrMenuItem(locomotiveProgrammingMenuItem)));

                    if (locomotiveProgrammingMenuItem.DropDownItems.Count == 1)
                        m.Items.Add(locomotiveProgrammingMenuItem.DropDownItems[0]);
                    else if (locomotiveProgrammingMenuItem.DropDownItems.Count > 1)
                        m.Items.Add(locomotiveProgrammingMenuItem);
                }
            }
        }

        [LayoutEvent("edit-train-properties")]
        private void EditTrainProperties(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender);
            Dialogs.TrainProperties trainProperties = new(train);

            trainProperties.ShowDialog();
        }

        private static string ExtractTrainDisplayName(object something) {
            return something switch
            {
                XmlElement element => element.Name switch
                {
                    "Locomotive" => new LocomotiveInfo(element).DisplayName,
                    "Train" => new TrainInCollectionInfo(element).DisplayName,
                    "TrainState" => new TrainStateInfo(element).DisplayName,
                    _ => throw new ArgumentException("Invalid placed element")
                },
                string displayName => displayName,
                TrainStateInfo train => train.DisplayName,
                _ => throw new ArgumentException("Invalid locomotive name")
            };
        }

        [DispatchTarget]
        private LayoutComponentConnectionPoint? GetLocomotiveFront(LayoutBlockDefinitionComponent blockDefinition, object nameObject) {
            var name = ExtractTrainDisplayName(nameObject);

            EventManager.Event(new LayoutEvent("ensure-component-visible", blockDefinition, false));

            Dialogs.LocomotiveFront locoFront = new(blockDefinition, name);
            return locoFront.ShowDialog() == DialogResult.OK ? locoFront.Front : null;
        }

        [DispatchTarget]
        private LayoutComponentConnectionPoint? GetWaypointFront(LayoutStraightTrackComponent track) {
            Dialogs.LocomotiveFront locoFront = new(track, "", "");

            return (locoFront.ShowDialog() == DialogResult.OK) ? locoFront.Front : null;
        }

        [DispatchTarget]
        private TrainFrontAndLength? GetTrainFrontAndLength(LayoutBlockDefinitionComponent blockDefinition, XmlElement collectionElement) { 
            var name = ExtractTrainDisplayName(collectionElement);

            EventManager.Event(new LayoutEvent("ensure-component-visible", blockDefinition, false));

            Dialogs.TrainFrontAndLength trainFrontAndLength = new(blockDefinition, name);
            return trainFrontAndLength.ShowDialog() == DialogResult.OK ? new TrainFrontAndLength(trainFrontAndLength.Front, trainFrontAndLength.Length) : null;
        }

        [LayoutEvent("get-programming-location")]
        private void GetProgrammingLocation(LayoutEvent e) {
            var possibleLocations = from blockDefinition in LayoutModel.Components<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases) where blockDefinition.Info.SuggestForProgramming select blockDefinition;
            int count = possibleLocations.Count();

            if (count == 0) {
                var selection = new LayoutSelection(from blockDefinition in LayoutModel.Components<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases) where blockDefinition.PowerConnector.Inlet.ConnectedOutlet.ObtainablePowers.Any(p => p.Type == LayoutPowerType.Programmer) select blockDefinition);

                if (selection.Count == 0)
                    LayoutModuleBase.Error("This operation need track that can receive command station's programming power - no such track is defined");
                else
                    LayoutModuleBase.Error(selection, "No block is designated as suggested for programming - you should edit at least one block's properties and set it as 'Suggest for programming'");

                e.Info = null;
            }
            else if (count == 1)
                e.Info = possibleLocations.First();
            else {
                var d = new Dialogs.SelectProgrammingLocation(possibleLocations);

                if (d.ShowDialog() == DialogResult.OK)
                    e.Info = d.SelectedProgrammingLocation;
                else
                    e.Info = null;
            }
        }

        #region Locomoative Programming menu entries

        [LayoutEvent("add-locomotive-programming-menu-entries")]
        private void AddSetLocomotiveSetAddress(LayoutEvent e) {
            var locomotive = Ensure.NotNull<LocomotiveInfo>(e.Sender);
            var m = Ensure.ValueNotNull<MenuOrMenuItem>(e.Info);

            if (locomotive.DecoderType is DecoderWithNumericAddressTypeInfo)
                m.Items.Add(new LayoutMenuItem("Set address...", null, (_, _) => DoSetLocomotiveAddress(locomotive)));
        }

        private async void DoSetLocomotiveAddress(LocomotiveInfo locomotive) {
            var d = new Dialogs.ChangeLocomotiveAddress(locomotive);

            if (d.ShowDialog() == DialogResult.OK) {
                var programmingState = new LocomotiveProgrammingState(locomotive);

                programmingState.ProgrammingActions = new LayoutActionContainer<LocomotiveInfo>(programmingState.Locomotive);
                var changeAddressAction = (ILayoutLocomotiveAddressChangeAction?)programmingState.ProgrammingActions.Add("set-address");

                if (changeAddressAction != null) {
                    changeAddressAction.Address = d.Address;
                    changeAddressAction.SpeedSteps = d.SpeedSteps;

                    if (d.ProgramLocomotive) {
                        try {
                            using var context = new LayoutOperationContext("LocomotiveProgramming", "set locomotive address", locomotive);
                            var train = (TrainStateInfo)await (Task<object>)EventManager.AsyncEvent(new LayoutEvent("program-locomotive", programmingState).SetOperationContext(context));
                        }
                        catch (LayoutException) { }
                    }
                    else
                        programmingState.ProgrammingActions.Commit();
                }
            }
        }

        #endregion

        #region Menu Items

        #region Locate Locomotive menu item

        private class LocateLocomotiveMenuItem : LayoutMenuItem {
            private readonly LayoutBlockDefinitionComponent blockInfo;

            public LocateLocomotiveMenuItem(LayoutBlockDefinitionComponent blockInfo) {
                this.blockInfo = blockInfo;

                Text = "Locate locomotive";
                Enabled = blockInfo != null;
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                EventManager.Event(new LayoutEvent("ensure-component-visible", blockInfo, true));
            }
        }

        #endregion

        #region Place on track menu item

        /// <summary>
        /// Menu item for placing locomotive on track
        /// </summary>
        private class PlaceOnTrackMenuItem : LayoutMenuItem {
            private readonly XmlElement placedElement;
            private readonly LayoutBlockDefinitionComponent blockInfo;

            public PlaceOnTrackMenuItem(XmlElement placedElement, LayoutBlockDefinitionComponent blockInfo) {
                this.placedElement = placedElement;
                this.blockInfo = blockInfo;

                this.Text = blockInfo.NameProvider.Text;
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                ComponentOperationTools.DoValidateAndPlaceTrain(new LayoutEvent<LayoutBlockDefinitionComponent, XmlElement>("validate-and-place-train-request", blockInfo, placedElement));
            }
        }

        #endregion

        #region Remove from track menu item

        public class RemoveFromTrackMenuItem : LayoutMenuItem {
            private readonly TrainStateInfo train;

            public RemoveFromTrackMenuItem(TrainStateInfo train) {
                this.train = train;

                this.Text = "Remove from track";
            }

            protected override async void OnClick(EventArgs e) {
                try {
                    using var context = new LayoutOperationContext("TrainRemoval", "removal of " + train.Name + " from track", train);
                    await EventManager.AsyncEvent(new LayoutEvent<object, string>("remove-from-track-request", train, "Remove train from track").SetOperationContext(context));
                }
                catch (LayoutException lex) {
                    lex.Report();
                }
                catch (OperationCanceledException) {
                    Trace.WriteLine("Removal of train " + train.Name + " canceled");
                }
            }
        }

        public class CancelTrainRemovalMenuItem : LayoutMenuItem {
            private readonly TrainStateInfo train;

            public CancelTrainRemovalMenuItem(TrainStateInfo train) {
                this.train = train;
                Text = "Cancel removal of train " + train.Name;
            }

            protected override void OnClick(EventArgs e) {
                var context = LayoutOperationContext.GetPendingOperation("TrainRemoval", train);

                if (context != null)
                    context.Cancel();
            }
        }

        #endregion

        #region Train properties menu item

        public class TrainPropertiesMenuItem : LayoutMenuItem {
            private readonly TrainStateInfo train;

            public TrainPropertiesMenuItem(TrainStateInfo train) {
                this.train = train;

                this.Text = "&Train properties...";
            }

            protected override void OnClick(EventArgs e) {
                EventManager.Event(new LayoutEvent("edit-train-properties", train));
            }
        }

        #endregion

        #region Show locomotive controller menu item

        public class ShowLocomotiveControllerMenuItem : LayoutMenuItem {
            private readonly TrainStateInfo state;

            public ShowLocomotiveControllerMenuItem(TrainStateInfo state) {
                this.state = state;
                this.Text = "Show &Controller";

                if (state.NotManaged || state.Locomotives.Count == 0)
                    this.Enabled = false;
            }

            protected override void OnClick(EventArgs e) {
                try {
                    EventManager.Event(new LayoutEvent("show-locomotive-controller", state));
                }
                catch (LayoutException lex) {
                    lex.Report();
                }
            }
        }

        #endregion

        #endregion
    }
}
