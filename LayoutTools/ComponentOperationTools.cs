using LayoutManager.CommonUI;
using LayoutManager.CommonUI.Controls;
using LayoutManager.Components;
using LayoutManager.Model;
using LayoutManager.View;
using MethodDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Tools {
    /// <summary>
    /// Summary description for ComponentTools.
    /// </summary>
    [LayoutModule("Components Operation Tools", UserControl = false)]
    public class ComponentOperationTools : System.ComponentModel.Component, ILayoutModuleSetup {
        /// <summary>
        /// Required designer variable.
        /// </summary>

        #region General services

        [DispatchTarget]
        private void EmergencyStopRequest(string reason, IModelComponentIsCommandStation? initiatingCommandStation) {
            if (initiatingCommandStation == null) {
                foreach (IModelComponentIsCommandStation commandStation in LayoutModel.Components<IModelComponentIsCommandStation>(LayoutModel.ActivePhases))
                    Dispatch.Call.EmergencyStopRequest(reason, commandStation);
            }
            else {
                Dispatch.Call.DisconnectPowerRequest(initiatingCommandStation);
                Dispatch.Notification.OnCommandStationEmergencyStop(initiatingCommandStation, reason);
            }
        }

        [DispatchTarget]
        private void OnCommandStationEmergencyStop(IModelComponentIsCommandStation commandStation, string reason) {
            var activeNotification = Dispatch.Call.GetCommandStationNotificationDialog(commandStation);

            if (activeNotification != null) {
                Dispatch.Call.UpdateCommandStationNotificationDialog(commandStation, reason);
                activeNotification.Activate();
            }
            else {
                ILayoutFrameWindow frameWindow = LayoutController.ActiveFrameWindow;
                using (activeNotification = new Dialogs.CommandStationStopped(commandStation, reason) {
                    Owner = frameWindow as Form
                }) {
                    activeNotification.Show(LayoutController.ActiveFrameWindow);
                }
            }
        }

        [DispatchTarget]
        private void CancelEmergencyStopRequest(IModelComponentIsCommandStation? commandStation) {
            if (commandStation == null) {
                foreach (IModelComponentIsCommandStation aCommandStation in LayoutModel.Components<IModelComponentIsCommandStation>(LayoutModel.ActivePhases))
                    Dispatch.Call.CancelEmergencyStopRequest(aCommandStation);
            }
            else {
                Dispatch.Call.ConnectPowerRequest(commandStation);
                Dispatch.Notification.OnCommandStationEmergencyStop(commandStation, "User initiated");
            }
        }

        #endregion

        #region Utility Functions

        private void SetSwitchState(IModelComponentHasSwitchingState component, int switchState) {
            List<SwitchingCommand> switchingCommands = new();

            component.AddSwitchingCommands(switchingCommands, switchState);
            Dispatch.Call.SetTrackComponentsState(switchingCommands);
        }

        #endregion

        #region Constructors

        public ComponentOperationTools(IContainer container) {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            container.Add(this);
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        public ComponentOperationTools() {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        #endregion

        #region IModelComponentIsMultiPath components

        [DispatchTarget]
        [DispatchFilter("InOperationMode")]
        private bool IncludeInComponentContextMenu([DispatchFilter] IModelComponentIsDualState component) => true;

        [DispatchTarget]
        [DispatchFilter(Type = "InOperationMode")]
        private void AddComponentContextMenuEntries(Guid frameWindowId, [DispatchFilter] IModelComponentIsDualState component, MenuOrMenuItem menu) {
            var multipath = (IModelComponentIsDualState)component;

            var item = new LayoutComponentMenuItem((ModelComponent)component, "&Toggle " + component.ToString(), new EventHandler(this.OnToggleDualStateComponent)) {
                DefaultItem = true
            };
            menu.Items.Add(item);
        }

        private void OnToggleDualStateComponent(Object? sender, EventArgs e) {
            var componentMenuItem = Ensure.NotNull<LayoutComponentMenuItem>(sender);
            var multipath = (IModelComponentIsDualState)componentMenuItem.Component;

            ToggleDualStateComponent(multipath);
        }

        [DispatchTarget]
        void DefaultActionCommand_DualState([DispatchFilter] IModelComponentIsDualState component, LayoutHitTestResult hitTestResult) {
            ToggleDualStateComponent(component);
        }

        private void ToggleDualStateComponent(IModelComponentIsDualState multipath) {
            var track = Ensure.NotNull<LayoutTrackComponent>(multipath, "track");
            var block = track.GetBlock(track.ConnectionPoints[0]);
            bool doit = true;

            if (block.LockRequest == null || !block.LockRequest.IsManualDispatchLock) {
                if (block.LockRequest != null) {
                    MessageBox.Show("You cannot manually change a setting of a " + track.ToString() + " which is allocated to a train", track.ToString() + " is allocated to train", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    doit = false;
                }
                else
                    LayoutModuleBase.Warning(track, "You are trying to modify the state of a " + track.ToString() + " that is not part of a manual dispatch block!");
            }

            if (doit)
                SetSwitchState(multipath, 1 - multipath.CurrentSwitchState);
        }

        #endregion

        #region Signal Component

        [DispatchTarget]
        [DispatchFilter("InOperationMode")]
        private bool IncludeInComponentContextMenu([DispatchFilter] LayoutSignalComponent component) => true;

        [DispatchTarget]
        [DispatchFilter(Type = "InOperationMode")]
        private void AddComponentContextMenuEntries(Guid frameWindowId, [DispatchFilter] LayoutSignalComponent component, MenuOrMenuItem menu) {
            var item = new LayoutComponentMenuItem(component, "&Toggle signal", new EventHandler(this.OnToggleSignal)) {
                DefaultItem = true
            };

            menu.Items.Add(item);
        }

        private void OnToggleSignal(Object? sender, EventArgs e) {
            var componentMenuItem = Ensure.NotNull<LayoutComponentMenuItem>(sender);
            var signal = (LayoutSignalComponent)componentMenuItem.Component;

            LayoutSignalState oldState = signal.SignalState;
            LayoutSignalState newState;

            if (oldState == LayoutSignalState.Green)
                newState = LayoutSignalState.Red;
            else
                newState = LayoutSignalState.Green;

            signal.SignalState = newState;
        }

        #endregion

        #region Straight track component

        private LayoutBlockDefinitionComponent? GetTrackBlockDefinition(LayoutStraightTrackComponent track) {
            if (track.BlockDefinitionComponent == null && track.TrackLinkComponent == null) {
                LayoutBlock block1 = track.GetBlock(track.ConnectionPoints[0]);
                LayoutBlock block2 = track.GetBlock(track.ConnectionPoints[1]);

                if (block1 == block2)
                    return block1.BlockDefinintion;
            }

            return null;
        }

        [DispatchTarget]
        private string? GetComponentPropertiesMenuName_StraigtTrack([DispatchFilter] LayoutStraightTrackComponent component) {
            if (component.BlockDefinitionComponent != null)
                return Dispatch.Call.GetComponentPropertiesMenuName(component.BlockDefinitionComponent);
            else
                return null;
        }

        [DispatchTarget]
        void DefaultActionCommand_StraightTrack([DispatchFilter] LayoutStraightTrackComponent component, LayoutHitTestResult hitTestResult) {
            if (component.BlockDefinitionComponent != null)
                Dispatch.Call.DefaultActionCommand(component.BlockDefinitionComponent, hitTestResult);
        }


        [DispatchTarget]
        [DispatchFilter("InOperationMode")]
        private bool IncludeInComponentContextMenu([DispatchFilter] LayoutStraightTrackComponent component) => component.BlockDefinitionComponent != null && Dispatch.Call.IncludeInComponentContextMenu(component.BlockDefinitionComponent);


        [DispatchTarget]
        [DispatchFilter("InOperationMode")]
        private void AddComponentContextMenuEntries(Guid frameWindowId, [DispatchFilter] LayoutStraightTrackComponent component, MenuOrMenuItem menu) {
            if (component.BlockDefinitionComponent != null)
                Dispatch.Call.AddComponentContextMenuEntries(frameWindowId, component.BlockDefinitionComponent, menu);
        }

        [DispatchTarget]
        [DispatchFilter("InOperationMode")]
        private void AddDetailsWindowSections_StraigtTrack([DispatchFilter] LayoutStraightTrackComponent track, PopupWindowContainerSection container) {
            if (track.BlockDefinitionComponent != null)
                Dispatch.Call.AddDetailsWindowSections(track.BlockDefinitionComponent, container);
        }

        [DispatchTarget]
        [DispatchFilter("InOperationMode")]
        private void QueryDrop([DispatchFilter] LayoutStraightTrackComponent track, DragEventArgs dragEventArgs) {
            if (track.BlockDefinitionComponent != null)
                Dispatch.Call.QueryDrop(track.BlockDefinitionComponent, dragEventArgs);
        }
        [DispatchTarget]
        private void DoDrop([DispatchFilter] LayoutStraightTrackComponent track, DragEventArgs dragEventArgs) {
            if (track.BlockDefinitionComponent != null)
                Dispatch.Call.DoDrop(track.BlockDefinitionComponent, dragEventArgs);
        }

        [DispatchTarget]
        [DispatchFilter("InOperationMode")]
        private object? QueryDrag([DispatchFilter] LayoutStraightTrackComponent track) {
            return track.BlockDefinitionComponent != null ? Dispatch.Call.QueryDrag(track.BlockDefinitionComponent) : null;
        }

        #endregion

        #region Block Definition Component

        [DispatchTarget]
        [DispatchFilter("InOperationMode")]
        private bool IncludeInComponentContextMenu([DispatchFilter] LayoutBlockDefinitionComponent component) => true;

        private void AddBlockInfoMenuEntries(MenuOrMenuItem m, LayoutBlockDefinitionComponent blockDefinition, IList<TrainStateInfo> trains) {
            bool defaultSet = false;
            LayoutBlock block = blockDefinition.Block;

            // Check if there any dialog that could take a block info for a way point
            List<ITripPlanEditorDialog> tripPlanEditorDialogs = new();

            Dispatch.Call.QueryEditTripPlanDialog(blockDefinition, tripPlanEditorDialogs);

            if (tripPlanEditorDialogs.Count == 1) {
                ITripPlanEditorDialog tripPlanEditorDialog = tripPlanEditorDialogs[0];

                var item = new AddBlockInfoToTripPlanEditorDialog(blockDefinition, tripPlanEditorDialog, "Add &Way-point");

                item.Font = new Font(item.Font, item.Font.Style | FontStyle.Bold);
                defaultSet = true;
                m.Items.Add(item);
            }
            else if (tripPlanEditorDialogs.Count > 1) {
                var addWayPoint = new LayoutMenuItem("Add &Way-point");

                foreach (ITripPlanEditorDialog tripPlanEditorDialog in tripPlanEditorDialogs)
                    addWayPoint.DropDownItems.Add(new AddBlockInfoToTripPlanEditorDialog(blockDefinition, tripPlanEditorDialog, tripPlanEditorDialog.DialogName));

                addWayPoint.Font = new Font(addWayPoint.Font, addWayPoint.Font.Style | FontStyle.Bold);
                defaultSet = true;
                m.Items.Add(addWayPoint);
            }

            // Check if there any dialog that could take a block info for a trip-plan origin
            var dialogs = new List<IModelComponentReceiverDialog>();

            dialogs.Clear();

            Dispatch.Call.QueryExecuteTripPlanDialog(blockDefinition, dialogs);

            if (dialogs.Count == 1) {
                var dialog = dialogs[0];

                var item = new AddBlockInfoToDialogMenuItem(blockDefinition, dialog, "Add &Trip-plan start location");

                if (!defaultSet) {
                    item.DefaultItem = true;
                    defaultSet = true;
                }

                m.Items.Add(item);
            }
            else if (dialogs.Count > 1) {
                var addLocation = new LayoutMenuItem("Add &Trip-plan start location");

                foreach (var dialog in dialogs)
                    addLocation.DropDownItems.Add(new AddBlockInfoToDialogMenuItem(blockDefinition, dialog, dialog.DialogName(blockDefinition)));

                m.Items.Add(addLocation);
            }

            // Check if there any dialog that could take a block info for a destination location
            dialogs.Clear();

            Dispatch.Call.QueryEditDestinationDialog(blockDefinition, dialogs);

            if (dialogs.Count == 1) {
                var dialog = dialogs[0];
                var item = new AddBlockInfoToDialogMenuItem(blockDefinition, dialog, "Add &Location");

                if (!defaultSet) {
                    item.DefaultItem = true;
                    defaultSet = true;
                }

                m.Items.Add(item);
            }
            else if (dialogs.Count > 1) {
                var addLocation = new LayoutMenuItem("Add &Location");

                foreach (IModelComponentReceiverDialog dialog in dialogs)
                    addLocation.DropDownItems.Add(new AddBlockInfoToDialogMenuItem(blockDefinition, dialog, dialog.DialogName(blockDefinition)));

                m.Items.Add(addLocation);
            }

            // Check if there any dialog that could take a block info for a destination location
            dialogs.Clear();

            Dispatch.Call.QueryEditManualDispatchRegionDialog(blockDefinition, dialogs);
            Debug.Assert(dialogs.Count < 2);

            if (dialogs.Count == 1) {
                var dialog = dialogs[0];
                var item = new AddBlockInfoToDialogMenuItem(blockDefinition, dialog, "Add Block to &Manual Dispatch Region");

                if (!defaultSet) {
                    item.DefaultItem = true;
                    defaultSet = true;
                }

                m.Items.Add(item);
            }

            if (block.LockRequest != null && block.LockRequest.IsManualDispatchLock) {
                if (m.Items.Count > 0)
                    m.Items.Add(new ToolStripSeparator());

                AddManualDispatchRouteMenuEntries(blockDefinition, m);
                m.Items.Add(new ToolStripSeparator());
            }

            // Check of block belongs to region that can accept more than one power
            if (blockDefinition.PowerConnector.Inlet.ConnectedOutlet.ObtainablePowers.Count() > 1)
                AddPowerConnectionEntries(blockDefinition, m);

            // Check if this block info has trains in it.

            if (trains.Count == 0) {
                var context = LayoutOperationContext.GetPendingOperation("TrainPlacement", blockDefinition);

                if (context == null) {
                    m.Items.Add("Place on track...", null,
                        (object? sender, EventArgs e) => {
                            Dialogs.SelectTrainToPlace d = new(blockDefinition);
                            if (d.ShowDialog() == DialogResult.OK && d.Selected?.Element != null) {
                                DoValidateAndPlaceTrain(blockDefinition, d.Selected.Element, new CreateTrainSettings() { Front = d.Front, Length = d.Length });
                            }
                        });
                }
                else
                    m.Items.Add("Cancel " + context.Description, null, (s, ea) => context.Cancel());

                if (blockDefinition.Info.UnexpectedTrainDetected) {
                    using var identifyTrainMenuItem = GetRelocateTrainMenuItem(blockDefinition);
                    if (identifyTrainMenuItem.DropDownItems.Count > 0)
                        m.Items.Add(identifyTrainMenuItem);
                }
                else {
                    var otherTrains = new List<TrainStateInfo>();

                    if (block.LockRequest == null || block.LockRequest.IsManualDispatchLock)
                        otherTrains.AddRange(LayoutModel.StateManager.Trains);
                    else {
                        var train = LayoutModel.StateManager.Trains[block.LockRequest.OwnerId];

                        if (train != null)
                            otherTrains.Add(train);
                    }

                    if (otherTrains.Count == 1)
                        m.Items.Add($"Fix: train '{otherTrains[0].DisplayName}' located here", null,
                            (sender, e1) => Dispatch.Call.RelocateTrainRequest(otherTrains[0], blockDefinition));
                    else if (otherTrains.Count > 1) {
                        var fixTrainLocationMenu = new LayoutMenuItem("Fix: train which is located here");

                        foreach (var train in otherTrains)
                            fixTrainLocationMenu.DropDownItems.Add(train.DisplayName, null, (sender, e1) => Dispatch.Call.RelocateTrainRequest(train, blockDefinition));

                        m.Items.Add(fixTrainLocationMenu);
                    }
                }

                // Check if trains in neighboring blocks can be extended
                var extendTrainMenuItem = Dispatch.Call.GetExtendTrainMenu(blockDefinition);

                if (extendTrainMenuItem != null)
                    m.Items.Add(extendTrainMenuItem);
            }
            else {
                if (trains.Count == 1) {
                    TrainStateInfo train = trains[0];

                    if (train.IsPowered) {
                        if (blockDefinition.Block.LockRequest == null || !blockDefinition.Block.LockRequest.IsManualDispatchLock) {
                            var item = new NewTripPlanMenuItem("New &Trip plan", train, blockDefinition);

                            if (!defaultSet && train.Speed == 0) {
                                item.DefaultItem = true;
                                defaultSet = true;
                            }

                            m.Items.Add(item);

                            m.Items.Add(new TripPlanCatalogMenuItem("&Saved Trip-plans...", trains[0]));
                            m.Items.Add(new ToolStripSeparator());
                        }
                    }

                    Dispatch.Call.AddToTrainOperationMenu(trains[0], m);
                }
                else {
                    foreach (TrainStateInfo train in trains) {
                        var trainItem = new LayoutMenuItem(train.DisplayName);

                        if (train.IsPowered) {
                            if (blockDefinition.Block.LockRequest == null || !blockDefinition.Block.LockRequest.IsManualDispatchLock) {
                                trainItem.DropDownItems.Add(new NewTripPlanMenuItem(train.DisplayName, train, blockDefinition));
                                trainItem.DropDownItems.Add(new TripPlanCatalogMenuItem("&Saved Trip-plans...", train));
                                trainItem.DropDownItems.Add("-");
                            }
                        }

                        Dispatch.Call.AddToTrainOperationMenu(train, new MenuOrMenuItem(trainItem));

                        if (trainItem.DropDownItems.Count > 0)
                            m.Items.Add(trainItem);
                    }
                }
            }

            if (blockDefinition.Info.IsOccupancyDetectionBlock)
                m.Items.Add(new ToggleTrainDetectionStateMenuItem(blockDefinition));
        }

        [DispatchTarget]
        private void AddToTrainOperationMenu_Orientation(TrainStateInfo train, MenuOrMenuItem menu) {
            var item = new LayoutMenuItem("Fix: reverse train orientation", null, (sender, e1) => {
                foreach (var trainLocation in train.Locations) {
                    if (trainLocation.IsDisplayFrontKnown) {
                        var reversedFront = trainLocation.Block.BlockDefinintion.Track.ConnectTo(trainLocation.DisplayFront, LayoutComponentConnectionType.Passage).FirstOrDefault();

                        trainLocation.DisplayFront = reversedFront;
                        trainLocation.Block.BlockDefinintion.OnComponentChanged();
                    }
                }
            });

            item.AddMeTo(menu);
        }

        private void AddPowerConnectionEntries(LayoutBlockDefinitionComponent blockDefinition, MenuOrMenuItem m) {
            var menuItems = new List<ToolStripMenuItem>();

            foreach (var power in blockDefinition.PowerConnector.Inlet.ConnectedOutlet.ObtainablePowers) {
                if (power.PowerOriginComponentId != blockDefinition.Power.PowerOriginComponentId || power.Type != blockDefinition.Power.Type) {
                    ILayoutPower p = power;
                    void setPower(object? s, EventArgs ea) {
                        using var context = new LayoutOperationContext("Power on", $"Set {p.Name}", blockDefinition);
                        Dispatch.Call.SetPower(blockDefinition, p, context);
                    };

                    switch (p.Type) {
                        case LayoutPowerType.Disconnected:
                            menuItems.Add(new LayoutMenuItem("Disconnect from power", null, setPower));
                            break;

                        case LayoutPowerType.Programmer:
                            // Add option for this, only if all the power region is a manual dispatch region
                            if (blockDefinition.PowerConnector.Blocks.All(b => b.LockRequest != null && b.LockRequest.IsManualDispatchLock))
                                menuItems.Add(new LayoutMenuItem("Connect to programming power", null, setPower));
                            break;

                        default:
                            menuItems.Add(new LayoutMenuItem($"Connect to {p.PowerOriginComponent.NameProvider.Name} {p.Name} power", null, setPower));
                            break;
                    }
                }
            }

            if (menuItems.Count > 0) {
                if (m.Items.Count > 0 && m.Items[m.Items.Count - 1] is not ToolStripSeparator)
                    m.Items.Add(new ToolStripSeparator());
                m.Items.AddRange(menuItems.ToArray());
                m.Items.Add(new ToolStripSeparator());
            }
        }

        [DispatchTarget]
        private void AddToTrainOperationMenu(TrainStateInfo train, MenuOrMenuItem menu) {
            bool isDefaultSet = false;

            foreach(var item in menu.Items) {
                if (item is LayoutMenuItem lmi && lmi.DefaultItem) {
                    isDefaultSet = true;
                    break;
                }
            }

            if (train.IsPowered) {
                var showControllerItem = new LocomotiveOperationTools.ShowLocomotiveControllerMenuItem(train);

                if (showControllerItem.Enabled && !isDefaultSet)
                    showControllerItem.DefaultItem = true;

                showControllerItem.AddMeTo(menu);
            }

            new SaveTrainInCollectionMenuItem(train).AddMeTo(menu);
            new ToolStripSeparator().AddMeTo(menu);
            new LocomotiveOperationTools.TrainPropertiesMenuItem(train).AddMeTo(menu);
            if (LayoutOperationContext.HasPendingOperation("TrainRemoval", train))
                new LocomotiveOperationTools.CancelTrainRemovalMenuItem(train).AddMeTo(menu);
            else
                new LocomotiveOperationTools.RemoveFromTrackMenuItem(train).AddMeTo(menu);
            new ToolStripSeparator().AddMeTo(menu);
        }

        [DispatchTarget]
        [DispatchFilter("InOperationMode")]
        private void QueryDrop([DispatchFilter] LayoutBlockDefinitionComponent blockDefinition, DragEventArgs dragEventArgs) {
            if (dragEventArgs.Data?.GetData(typeof(XmlElement)) is XmlElement element) {
                if (element.Name == "Locomotive" || element.Name == "Train") {
                    if (!blockDefinition.Block.HasTrains) {
                        var train = LayoutModel.StateManager.Trains[element];

                        if (train == null) {        // Not already on track
                            var result = Dispatch.Call.CanLocomotiveBePlaced(element, blockDefinition);

                            if (result.CanBeResolved)
                                dragEventArgs.Effect = DragDropEffects.Link;
                        }
                    }
                }
                else if (element.Name == "TrainState") {
                    var train = new TrainStateInfo(element);

                    if (IsTrainOnSamePower(train, blockDefinition)) {
                        // Check whether this is a train identification case (the block with the train
                        // is an occupancy detection block with no train detected) or whether it is a shortcut
                        // for moving the train from this point to the next
                        bool relocateTrain = false;

                        foreach (TrainLocationInfo trainLocation in train.Locations) {
                            if (trainLocation.Block.BlockDefinintion.Info.IsOccupancyDetectionBlock &&
                                !trainLocation.Block.BlockDefinintion.Info.TrainDetected) {
                                relocateTrain = true;
                                break;
                            }
                        }

                        if (relocateTrain) {
                            if (blockDefinition.Info.UnexpectedTrainDetected)
                                dragEventArgs.Effect = DragDropEffects.Copy;
                        }
                        else if (blockDefinition.Block.CanTrainWait)
                            dragEventArgs.Effect = DragDropEffects.Move;
                    }
                }
            }
        }

        public static async void DoValidateAndPlaceTrain(LayoutBlockDefinitionComponent blockDefinition, XmlElement placedElement, CreateTrainSettings settings) {
            string name = placedElement.Name == "Locomotive" ? new LocomotiveInfo(placedElement).Name : new TrainInCollectionInfo(placedElement).Name;

            using var context = new LayoutOperationContext("TrainPlacement", $"Placing {name} on track", new LayoutXmlWithIdWrapper(placedElement), blockDefinition);
            try {
                await Dispatch.Call.ValidateAndPlaceTrainRequest(blockDefinition, placedElement, settings, context);
            }
            catch (LayoutException ex) {
                ex.Report();
            }
            catch (OperationCanceledException) { }
        }

        [DispatchTarget]
        [DispatchFilter("InOperationMode")]
        private void DoDrop_BlockDefinition([DispatchFilter] LayoutBlockDefinitionComponent blockDefinition, DragEventArgs dragEventArgs) {
            if (dragEventArgs.Data?.GetData(typeof(XmlElement)) is XmlElement element) {
                if (element.Name == "Locomotive" || element.Name == "Train") {
                    if (!blockDefinition.Block.HasTrains) {
                        var train = LayoutModel.StateManager.Trains[element];

                        // Check that train is not already on track and that there are not other pending placement operation on this loco/train or block
                        if (train == null &&
                         !LayoutOperationContext.HasPendingOperation("TrainPlacement", new LayoutXmlWithIdWrapper(element)) &&
                         !LayoutOperationContext.HasPendingOperation("TrainPlacement", blockDefinition))
                            DoValidateAndPlaceTrain(blockDefinition, element, new CreateTrainSettings());
                    }
                }
                else if (element.Name == "TrainState") {
                    TrainStateInfo train = new(element);

                    if (dragEventArgs.Effect == DragDropEffects.Copy) {
                        Dispatch.Call.RelocateTrainRequest(train, blockDefinition);
                    }
                    else if (dragEventArgs.Effect == DragDropEffects.Move) {
                        // Create trip plan to move train
                        TripPlanInfo tripPlan = new();

                        tripPlan.Add(blockDefinition);

                        using Dialogs.TripPlanEditor tripPlanEditor = new(tripPlan, train);

                        tripPlanEditor.Show();
                    }
                }
            }
        }

        [DispatchTarget]
        private void GetSmartDestinationList(LayoutBlockDefinitionComponent blockDefinition, List<TripPlanDestinationInfo> destinations) {
            foreach (TripPlanDestinationInfo tripPlanDestination in LayoutModel.StateManager.TripPlansCatalog.Destinations) {
                foreach (LayoutBlockDefinitionComponent destinationBlockDefinition in tripPlanDestination.BlockInfoList)
                    if (blockDefinition.Id == destinationBlockDefinition.Id) {
                        destinations.Add(tripPlanDestination);
                        break;
                    }
            }
        }

        [DispatchTarget]
        [DispatchFilter("InOperationMode")]
        private object? QueryDrag([DispatchFilter] LayoutBlockDefinitionComponent blockDefinition) {
            return (blockDefinition.Block.Trains.Count == 1) ? blockDefinition.Block.Trains[0].Train.Element : null;
        }

        [DispatchTarget]
        [DispatchFilter("InOperationMode")]
        private string? GetComponentPropertiesMenuName([DispatchFilter] LayoutBlockDefinitionComponent blockInfo) {
            if (LayoutModel.StateManager.Trains[blockInfo.Block].Count > 0)
                return "Block &Properties...";
            return null;
        }

        [DispatchTarget]
        [DispatchFilter(Type = "InOperationMode")]
        private void AddComponentContextMenuEntries(Guid frameWindowId, [DispatchFilter] LayoutBlockDefinitionComponent component, MenuOrMenuItem menu) {
            IList<TrainStateInfo> trains = LayoutModel.StateManager.Trains[component.Block];

            AddBlockInfoMenuEntries(menu, component, trains);
        }

        [DispatchTarget(Order = 1000)]
        [DispatchFilter(Type = "InOperationMode")]
        private void AddDetailsWindowSections([DispatchFilter] LayoutBlockDefinitionComponent blockDefinition, PopupWindowContainerSection container) {
            if (blockDefinition.Block.HasTrains)
                foreach (TrainLocationInfo trainLocation in blockDefinition.Block.Trains)
                    Dispatch.Call.AddTrainDetailsWindowSection(trainLocation.Train, container);
            else {
                var lockRequest = blockDefinition.Block.LockRequest;

                if (lockRequest != null) {
                    if (lockRequest.IsManualDispatchLock) {
                        var manualDispatchRegion = LayoutModel.StateManager.ManualDispatchRegions[lockRequest.OwnerId];

                        if (manualDispatchRegion != null)
                            container.AddText("Part of manual dispatch region: " + manualDispatchRegion.Name);
                        else
                            container.AddText("Part of manual dispatch region");
                    }
                    else {
                        var train = LayoutModel.StateManager.Trains[lockRequest.OwnerId];

                        if (train != null) {
                            container.AddText("Block allocated to:");

                            Dispatch.Call.AddTrainDetailsWindowSection(train, container);
                        }
                    }
                }
            }
        }

        [DispatchTarget]
        private void AddTrainDetailsWindowSection(TrainStateInfo train, PopupWindowContainerSection container) {
            PopupWindowContainerSection trainContainer = container.CreateContainer();

            trainContainer.AddVerticalSection(new PopupWindowTextSection($"{train.Length.ToDisplayString()} train: {train.DisplayName}: {train.StatusText}"));

            if (train.Trip != null) {
                if (train.Driver.Type == "Automatic")
                    trainContainer.AddText("Driven by the computer");
                else if (train.Driver.Type.StartsWith("Manual"))
                    trainContainer.AddText("Driven manually");

                switch (train.Trip.Status) {
                    case TripStatus.PrepareStop:
                        trainContainer.AddText("Slowing down before stopping");
                        break;

                    case TripStatus.Suspended:
                        trainContainer.AddText("Train ride is suspended");
                        break;

                    case TripStatus.WaitLock:
                        trainContainer.AddText("Waiting for green light");
                        break;

                    case TripStatus.WaitStartCondition:
                        trainContainer.AddText("Waiting for start condition");
                        break;
                }
            }

            AddTrainDetailsPopupSections(trainContainer, train);
            container.AddVerticalSection(trainContainer);

            if (train.Attributes.Count > 0)
                container.AddVerticalSection(new PopupWindowAttributesSection("Train attributes: ", train));

            if (train.SpeedLimit > 0)
                container.AddText("Train speed limit: " + train.SpeedLimit);
        }

        private void AddTrainDetailsPopupSections(PopupWindowContainerSection container, TrainStateInfo train) {
            foreach (TrainLocomotiveInfo trainLocomotive in train.Locomotives) {
                PopupWindowContainerSection locoContainer = container.CreateContainer();

                AddLocomotiveDetailsPopupSections(locoContainer, trainLocomotive);
                container.AddHorizontalSection(locoContainer);
            }

            //TODO: Add train attributes
        }

        private void AddLocomotiveDetailsPopupSections(PopupWindowContainerSection locoContainer, TrainLocomotiveInfo trainLocomotive) {
            var locoImage = trainLocomotive.Locomotive.Image ?? LayoutModel.LocomotiveCatalog.GetStandardImage(trainLocomotive.Locomotive.Kind, trainLocomotive.Locomotive.Origin);

            if (locoImage != null) {
                if (trainLocomotive.Orientation == LocomotiveOrientation.Backward)
                    locoImage.RotateFlip(RotateFlipType.RotateNoneFlipX);

                PopupWindowContainerSection locoImageContainer = locoContainer.CreateContainer();

                locoImageContainer.BorderPen = Pens.Black;
                locoImageContainer.OuterMargins = new Size(0, 0);
                locoImageContainer.InnerMargins = new Size(1, 1);
                locoImageContainer.AddVerticalSection(new PopupWindowImageSection(locoImage));
                locoContainer.AddVerticalSection(locoImageContainer);
            }
        }

        #region Find route on manual dispatch region

        private class ManualDispatchRouteSetting {
            private LayoutBlockDefinitionComponent? sourceBlockInfo = null;
            private Guid sourceRegionID = Guid.Empty;

            public LayoutBlockDefinitionComponent? SourceBlockInfo {
                get {
                    return sourceBlockInfo;
                }

                set {
                    sourceBlockInfo = value;
                    if (sourceBlockInfo == null || sourceBlockInfo.Block.LockRequest == null)
                        sourceRegionID = Guid.Empty;
                    else
                        sourceRegionID = sourceBlockInfo.Block.LockRequest.OwnerId;
                }
            }

            public Guid SourceRegionID => sourceRegionID;
        }

        //private const string E_ApplicableTripPlans = "ApplicableTripPlans";
        //private const string A_TripPlanId = "TripPlanID";
        //private const string A_ShouldReverse = "ShouldReverse";
        private readonly ManualDispatchRouteSetting manualDispatchRouteSetting = new();

        [DispatchTarget]
        private void OnExitOperationMode() => manualDispatchRouteSetting.SourceBlockInfo = null;

        private void AddManualDispatchRouteMenuEntries(LayoutBlockDefinitionComponent blockInfo, MenuOrMenuItem m) {
            var lockRequest = blockInfo.Block.LockRequest;

            if (lockRequest != null && lockRequest.IsManualDispatchLock) {
                if (manualDispatchRouteSetting.SourceBlockInfo != null && manualDispatchRouteSetting.SourceRegionID == lockRequest.OwnerId && manualDispatchRouteSetting.SourceBlockInfo.Id != blockInfo.Id)
                    m.Items.Add(new CreateManualDispatchRouteMenuItem(manualDispatchRouteSetting, blockInfo));
                m.Items.Add(new SetManualDispatchRouteSourceMenuItem(manualDispatchRouteSetting, blockInfo));
            }
        }

        #region Route creation menu items

        private class SetManualDispatchRouteSourceMenuItem : LayoutMenuItem {
            private readonly ManualDispatchRouteSetting manualDispatchRouteSetting;
            private readonly LayoutBlockDefinitionComponent blockInfo;

            public SetManualDispatchRouteSourceMenuItem(ManualDispatchRouteSetting manualDispatchRouteSetting, LayoutBlockDefinitionComponent blockInfo) {
                this.manualDispatchRouteSetting = manualDispatchRouteSetting;
                this.blockInfo = blockInfo;

                Text = "&Set Route Origin";
            }

            protected override void OnClick(EventArgs e) {
                manualDispatchRouteSetting.SourceBlockInfo = blockInfo;
            }
        }

        private class CreateManualDispatchRouteMenuItem : LayoutMenuItem {
            private readonly ManualDispatchRouteSetting manualDispatchRouteSetting;
            private readonly LayoutBlockDefinitionComponent blockInfo;
            private readonly LocomotiveOrientation direction = LocomotiveOrientation.Unknown;

            public CreateManualDispatchRouteMenuItem(ManualDispatchRouteSetting manualDispatchRouteSetting, LayoutBlockDefinitionComponent blockInfo) {
                this.manualDispatchRouteSetting = manualDispatchRouteSetting;
                this.blockInfo = blockInfo;

                Text = "&Create route";
                DropDownItems.Add(new CreateManualDispatchRouteMenuItem(manualDispatchRouteSetting, blockInfo, LocomotiveOrientation.Forward));
                DropDownItems.Add(new CreateManualDispatchRouteMenuItem(manualDispatchRouteSetting, blockInfo, LocomotiveOrientation.Backward));
            }

            public CreateManualDispatchRouteMenuItem(ManualDispatchRouteSetting manualDispatchRouteSetting, LayoutBlockDefinitionComponent blockInfo, LocomotiveOrientation direction) {
                this.manualDispatchRouteSetting = manualDispatchRouteSetting;
                this.blockInfo = blockInfo;
                this.direction = direction;

                if (direction == LocomotiveOrientation.Forward)
                    Text = "&Forward";
                else
                    Text = "&Backward";
            }

            protected override void OnClick(EventArgs e) {
                IRoutePlanningServices ts = Dispatch.Call.GetRoutePlanningServices();
                var trainLocations = manualDispatchRouteSetting.SourceBlockInfo?.Block.Trains;
                LayoutComponentConnectionPoint front = LayoutComponentConnectionPoint.Empty;
                bool findRoute = true;

                if (trainLocations != null && trainLocations.Count > 0)
                    front = trainLocations[0].DisplayFront;
                else {
                    if (manualDispatchRouteSetting.SourceBlockInfo != null) {
                        front = Dispatch.Call.GetLocomotiveFront(manualDispatchRouteSetting.SourceBlockInfo, manualDispatchRouteSetting.SourceBlockInfo.Name) ?? LayoutComponentConnectionPoint.Empty;

                        if (front == LayoutComponentConnectionPoint.Empty)
                            findRoute = false;
                    }
                    else
                        findRoute = false;
                }

                if (findRoute && manualDispatchRouteSetting.SourceBlockInfo != null) {
                    BestRoute bestRoute = ts.FindBestRoute(manualDispatchRouteSetting.SourceBlockInfo, front, direction, blockInfo, manualDispatchRouteSetting.SourceRegionID, true);

                    if (bestRoute.Quality.IsValidRoute) {
                        bool completed = Dispatch.Call.DispatcherSetSwitches(manualDispatchRouteSetting.SourceRegionID, bestRoute);

                        if (!completed) {
                            LayoutSelection selection = new(new ModelComponent[] { manualDispatchRouteSetting.SourceBlockInfo, blockInfo });

                            LayoutModuleBase.Error(selection, "Could not find route that is within the manual dispatch region");
                        }
                    }
                    else {
                        LayoutSelection selection = new(new ModelComponent[] { manualDispatchRouteSetting.SourceBlockInfo, blockInfo });

                        LayoutModuleBase.Error(selection, "No route could be found");
                    }
                }
            }
        }

        #endregion

        #endregion

        private class AddBlockInfoToTripPlanEditorDialog : LayoutMenuItem {
            private readonly LayoutBlockDefinitionComponent blockDefinition;
            private readonly ITripPlanEditorDialog tripPlanEditorDialog;

            public AddBlockInfoToTripPlanEditorDialog(LayoutBlockDefinitionComponent blockDefinition, ITripPlanEditorDialog tripPlanEditorDialog, string title) {
                List<TripPlanDestinationInfo> smartDestinations = new();

                Text = title;
                this.blockDefinition = blockDefinition;
                this.tripPlanEditorDialog = tripPlanEditorDialog;

                Dispatch.Call.GetSmartDestinationList(blockDefinition, smartDestinations);

                if (smartDestinations.Count > 0) {
                    DropDownItems.Add("This specific block", null, (object? sender, EventArgs e) => tripPlanEditorDialog.AddWayPoint(blockDefinition));
                    DropDownItems.Add(new ToolStripSeparator());

                    foreach (TripPlanDestinationInfo tripPlanDestination in smartDestinations) {
                        TripPlanDestinationInfo destination = tripPlanDestination;

                        DropDownItems.Add(tripPlanDestination.Name, null, (object? sender, EventArgs e) => tripPlanEditorDialog.AddWayPoint(destination));
                    }
                }
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                tripPlanEditorDialog.AddWayPoint(blockDefinition);
            }
        }

        #region Add waypoint to trip plan

        private class AddBlockInfoToDialogMenuItem : LayoutMenuItem {
            private readonly LayoutBlockDefinitionComponent blockInfo;
            private readonly IModelComponentReceiverDialog dialog;

            public AddBlockInfoToDialogMenuItem(LayoutBlockDefinitionComponent blockInfo, IModelComponentReceiverDialog dialog, string title) {
                this.Text = title;
                this.dialog = dialog;
                this.blockInfo = blockInfo;
            }

            protected override void OnClick(EventArgs e) {
                dialog.AddComponent(blockInfo);
            }
        }

        #endregion

        #region New/Use trip plan

        private class NewTripPlanMenuItem : LayoutMenuItem {
            private readonly TrainStateInfo train;

            public NewTripPlanMenuItem(string title, TrainStateInfo train, LayoutBlockDefinitionComponent _) {
                this.Text = title;
                this.train = train;

                if (train.Locomotives.Count == 0)
                    Enabled = false;
            }

            protected override void OnClick(EventArgs e) {
                var dialog = Dispatch.Call.QueryEditTripPlanForTrain(train);

                if (dialog != null)
                    dialog.Activate();
                else {
                    TripPlanInfo tripPlan = new();

                    var tripPlanEditor = new Dialogs.TripPlanEditor(tripPlan, train);

                    tripPlanEditor.Show();
                }
            }
        }

        private class TripPlanCatalogMenuItem : LayoutMenuItem {
            private readonly TrainStateInfo train;

            public TripPlanCatalogMenuItem(string title, TrainStateInfo train) {
                this.Text = title;
                this.train = train;

                if (train.Locomotives.Count == 0)
                    Enabled = false;
            }

            protected override void OnClick(EventArgs e) {
                if (!Dispatch.Call.ShowSavedTripPlansForTrain(train)) {
                    var tripPlanCatalog = new Dialogs.TripPlanCatalog(train);

                    tripPlanCatalog.Show();
                }
            }
        }

        private class UseExistingTripPlanMenuItem : LayoutMenuItem {
            private readonly TripPlanInfo tripPlan;
            private readonly TrainStateInfo train;
            private readonly bool shouldReverse;

            public UseExistingTripPlanMenuItem(TripPlanInfo tripPlan, bool shouldReverse, TrainStateInfo train) {
                this.tripPlan = tripPlan;
                this.shouldReverse = shouldReverse;
                this.train = train;

                if (shouldReverse)
                    Text = "Reversed - " + tripPlan.Name;
                else
                    Text = tripPlan.Name;
            }

            protected override void OnClick(EventArgs e) {
                XmlDocument tripPlanDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();

                tripPlanDoc.AppendChild(tripPlanDoc.ImportNode(tripPlan.Element, true));

                TripPlanInfo newTripPlan = new(tripPlanDoc.DocumentElement!);

                if (shouldReverse)
                    newTripPlan.Reverse();

                newTripPlan.FromCatalog = true;

                using Dialogs.TripPlanEditor tripPlanEditor = new(newTripPlan, train);

                tripPlanEditor.Show();
            }
        }

        #endregion

        #region Save train in collection

        private class SaveTrainInCollectionMenuItem : LayoutMenuItem {
            private readonly TrainStateInfo train;

            public SaveTrainInCollectionMenuItem(TrainStateInfo train) {
                this.train = train;

                Text = "&Save Train in Collection";
            }

            protected override void OnClick(EventArgs e) {
                LocomotiveCollectionInfo collection = LayoutModel.LocomotiveCollection;
                var trainInCollectionElement = collection["Train", train.Name];
                TrainInCollectionInfo trainInCollection;

                if (trainInCollectionElement == null) {
                    trainInCollectionElement = collection.CollectionElement.OwnerDocument.CreateElement("Train");

                    trainInCollection = new TrainInCollectionInfo(trainInCollectionElement);
                    trainInCollection.Initialize();
                    collection.CollectionElement.AppendChild(trainInCollectionElement);
                }
                else
                    trainInCollection = new TrainInCollectionInfo(trainInCollectionElement);

                trainInCollection.Name = train.Name;
                trainInCollection.CopyFrom(train, true);
                Dispatch.Notification.OnTrainSavedInCollection(trainInCollection);
                collection.Save();
            }
        }

        #endregion

        #region Locomotive owner draw menu item base class

        private class LocomotiveMenuItem : LayoutMenuItem {
            public LocomotiveMenuItem(XmlElement placedElement, LayoutBlock block) {
                this.PlacedElement = placedElement;
                this.Block = block;

                this.AutoSize = false;
                this.Text = placedElement["Name"]?.InnerText ?? "Unknown-Name";
            }

            protected XmlElement PlacedElement { get; }

            protected LayoutBlock Block { get; }

            protected override Size DefaultSize {
                get {
                    SizeF textSize;
                    int imageWidth;
                    int textShift = 0;
                    string name;

                    if (PlacedElement.Name == "Locomotive") {
                        LocomotiveInfo loco = new(PlacedElement);

                        name = loco.DisplayName;
                        imageWidth = 50;
                        textShift = 55;
                    }
                    else {
                        TrainInCollectionInfo trainInCollection = new(PlacedElement);

                        name = trainInCollection.DisplayName;
                        imageWidth = 2 + ((28 + 2) * trainInCollection.Locomotives.Count);
                    }

                    using (Graphics g = Owner.CreateGraphics())
                    using (Font titleFont = new("Arial", 8, FontStyle.Bold))
                        textSize = g.MeasureString(name, titleFont);

                    return new Size(Math.Max((int)textSize.Width + textShift, imageWidth), 50);
                }
            }

            protected override void OnPaint(PaintEventArgs e) {
                base.OnPaint(e);
                using (Brush br = new SolidBrush(Selected ? SystemColors.Highlight : SystemColors.Menu))
                    e.Graphics.FillRectangle(br, e.ClipRectangle);

                Owner.Renderer.DrawItemBackground(new(e.Graphics, this));

                GraphicsState gs = e.Graphics.Save();
                e.Graphics.TranslateTransform(e.ClipRectangle.Left, e.ClipRectangle.Top);

                // Draw the locomotive image and name. There are two possible layouts one
                // for locomotive and one for locomotive set
                int xText;
                float yText;
                string name;

                if (PlacedElement.Name == "Locomotive") {
                    LocomotiveInfo loco = new(PlacedElement);

                    using (LocomotiveImagePainter locoPainter = new(LayoutModel.LocomotiveCatalog)) {
                        locoPainter.Draw(e.Graphics, new Point(2, 2), new Size(50, 36), loco.Element);
                    }

                    yText = 2;
                    xText = 55;
                    name = loco.DisplayName;
                }
                else if (PlacedElement.Name == "Train" || PlacedElement.Name == "TrainState") {
                    TrainCommonInfo train = new(PlacedElement);

                    using (LocomotiveImagePainter locoPainter = new(LayoutModel.LocomotiveCatalog)) {
                        int x = 2;

                        locoPainter.FrameSize = new Size(28, 20);

                        foreach (TrainLocomotiveInfo trainLocomotive in train.Locomotives) {
                            locoPainter.LocomotiveElement = trainLocomotive.Locomotive.Element;
                            locoPainter.FlipImage = trainLocomotive.Orientation == LocomotiveOrientation.Backward;
                            locoPainter.Origin = new Point(x, 2);
                            locoPainter.Draw(e.Graphics);

                            x += locoPainter.FrameSize.Width + 2;
                        }
                    }

                    xText = 2;
                    yText = 24;
                    name = train.DisplayName;
                }
                else
                    throw new ApplicationException("Invalid placed element");

                using (Brush textBrush = new SolidBrush(Selected ? SystemColors.HighlightText : SystemColors.MenuText)) {
                    SizeF textSize;

                    using (Font titleFont = new("Arial", 8, FontStyle.Bold)) {
                        textSize = e.Graphics.MeasureString(name, titleFont);
                        e.Graphics.DrawString(name, titleFont, textBrush, new PointF(xText, yText));
                    }

                    yText += textSize.Height;
                }

                e.Graphics.Restore(gs);
            }
        }

        #endregion

        #region Relocate train menu item

        private bool IsTrainOnSamePower(TrainStateInfo train, LayoutBlockDefinitionComponent blockDefinition) {
            if (train.Locomotives.Count == 0)
                return true;            // No problem to relocate train without any locomotives

            return train.Power == blockDefinition.Power;
        }

        private LayoutMenuItem GetRelocateTrainMenuItem(LayoutBlockDefinitionComponent blockInfo) {
            var menu = new LayoutMenuItem("Identify train");
            IDictionary addedTrains = new HybridDictionary();

            foreach (XmlElement trainElement in LayoutModel.StateManager.Trains.Element) {
                var train = new TrainStateInfo(trainElement);

                if (IsTrainOnSamePower(train, blockInfo)) {
                    bool phantomTrain = false;

                    // Check if this train location is on block which does not report on train detection
                    foreach (TrainLocationInfo trainLocation in train.Locations) {
                        LayoutBlock block = trainLocation.Block;

                        if (block.BlockDefinintion.Info.IsOccupancyDetectionBlock && !block.BlockDefinintion.Info.TrainDetected) {
                            phantomTrain = true;
                            break;
                        }
                    }

                    if (phantomTrain) {
                        addedTrains.Add(train.Id, trainElement);
                        menu.DropDownItems.Add(new RelocateTrainMenuItem(trainElement, blockInfo.Block));
                    }
                }
            }

            if (menu.DropDownItems.Count > 0)
                menu.DropDownItems.Add(new ToolStripSeparator());

            foreach (XmlElement trainElement in LayoutModel.StateManager.Trains.Element) {
                var train = new TrainStateInfo(trainElement);

                if (IsTrainOnSamePower(train, blockInfo) && !addedTrains.Contains(train.Id))
                    menu.DropDownItems.Add(new RelocateTrainMenuItem(trainElement, blockInfo.Block));
            }

            return menu;
        }

        private class RelocateTrainMenuItem : LocomotiveMenuItem {
            public RelocateTrainMenuItem(XmlElement placedElement, LayoutBlock block) : base(placedElement, block) {
            }

            protected override void OnClick(EventArgs e) {
                var train = Dispatch.Call.ExtractTrainState(PlacedElement);

                Dispatch.Call.RelocateTrainRequest(train, Block.BlockDefinintion);
            }
        }

        #endregion

        #region Toggle train detection state

        private class ToggleTrainDetectionStateMenuItem : LayoutMenuItem {
            private readonly LayoutBlockDefinitionComponent blockInfo;

            public ToggleTrainDetectionStateMenuItem(LayoutBlockDefinitionComponent blockInfo) {
                this.blockInfo = blockInfo;

                Text = "Train &Detected";
                Checked = blockInfo.Info.TrainDetected;
            }

            protected override void OnClick(EventArgs e) {
                blockInfo.EraseImage();
                blockInfo.Info.TrainDetected = !blockInfo.Info.TrainDetected;
                blockInfo.Redraw();
            }
        }

        #endregion

        #region Remove balloon if track is detected

        [DispatchTarget]
        private void OnTrainDetectionBlockOccupied(LayoutOccupancyBlock occupancyBlock) {
            // Check if any contained block contains train. If so, then the detection was of this train, and nothing should be done
            foreach (LayoutBlock block in occupancyBlock.ContainedBlocks) {
                LayoutBlockDefinitionComponent blockDefinition = block.BlockDefinintion;

                if (LayoutBlockBalloon.IsDisplayed(blockDefinition) && LayoutBlockBalloon.Get(blockDefinition).RemoveOnTrainDetected)
                    LayoutBlockBalloon.Remove(blockDefinition, LayoutBlockBalloon.TerminationReason.TrainDetected);
            }
        }

        #endregion

        [DispatchTarget]
        void DefaultActionCommand_BlockDefinition([DispatchFilter] LayoutBlockDefinitionComponent inBlockInfo, LayoutHitTestResult hitTestResult) {
            LayoutBlockDefinitionComponent? blockInfo = null;
            var trains = new List<TrainStateInfo>();

            foreach (ModelComponent component in hitTestResult.Selection)
                if (component is LayoutBlockDefinitionComponent blockDefinition) {
                    blockInfo = blockDefinition;
                    break;
                }

            if (blockInfo == null) {
                blockInfo = inBlockInfo;

                foreach (TrainLocationInfo trainLocation in blockInfo.Block.Trains)
                    trains.Add(trainLocation.Train);
            }

            using var m = new ContextMenuStrip();
            AddBlockInfoMenuEntries(new MenuOrMenuItem(m), blockInfo, trains.ToArray());

            foreach (var entry in m.Items) {
                if (entry is LayoutMenuItem item && item.DefaultItem) {
                    if (item.DropDownItems.Count > 0) {
                        var subitems = new List<LayoutMenuItem>();

                        foreach (var subEntry in item.DropDownItems)
                            if (subEntry is LayoutMenuItem subitem)
                                subitems.Add(subitem);

                        var contextMenu = new ContextMenuStrip();
                        contextMenu.Items.AddRange(subitems.ToArray());
                        Form parentForm = hitTestResult.View.FindForm();
                        Point where = parentForm.PointToClient(Control.MousePosition);

                        contextMenu.Show(parentForm, where);
                    }
                    else
                        item.PerformClick();

                    break;
                }
            }
        }

        #region Track Power Connector component

        [DispatchTarget(Order = 1000)]
        private void AddDetailsWindowSections_PowerConnector([DispatchFilter] LayoutTrackPowerConnectorComponent component, PopupWindowContainerSection container) {
            if (!LayoutController.IsOperationMode)
                return;

            if (component.Inlet.IsConnected) {
                ILayoutPower power = component.Inlet.ConnectedOutlet.Power;
                string powerDescription;
                string? origin;

                if (power.PowerOriginComponent != null)
                    origin = " from " + power.PowerOriginComponent.NameProvider.Name;
                else
                    origin = null;

                switch (power.Type) {
                    case LayoutPowerType.Analog:
                        powerDescription = "Analog power" + origin;
                        break;

                    case LayoutPowerType.Digital:
                        powerDescription = "Power/Digital commands" + origin;
                        break;

                    case LayoutPowerType.Disconnected:
                        if (origin == null)
                            powerDescription = "Disconnected";
                        else
                            powerDescription = "No power" + origin;
                        break;

                    case LayoutPowerType.Programmer:
                        powerDescription = "Programming commands" + origin;
                        break;

                    default:
                        powerDescription = power.Type.ToString() + (origin ?? "");
                        break;
                }

                container.AddText("Power: " + powerDescription);
            }
            else
                container.AddText("Not connected to any power source");
        }

        #endregion

        #region Extend train

        [DispatchTarget]
        private LayoutMenuItem? GetExtendTrainMenu(LayoutBlockDefinitionComponent blockDefinition) {
            LayoutBlock block = blockDefinition.Block;
            var extendableTrains = new List<ExtendableTrainInfo>();

            foreach (TrackEdge edge in block.TrackEdges) {
                LayoutComponentConnectionPoint otherBlockCp = (edge.ConnectionPoint == edge.Track.ConnectionPoints[0]) ? edge.Track.ConnectionPoints[1] : edge.Track.ConnectionPoints[0];
                LayoutBlock otherBlock = edge.Track.GetBlock(otherBlockCp);

                if (otherBlock.Trains.Count == 1 && (otherBlock.Trains[0].Train.TrackContactTriggerCount > 1 || block.OccupancyBlock != null)) {
                    extendableTrains.Add(new ExtendableTrainInfo(otherBlock.Trains[0].Train, edge, otherBlock));
                }
            }

            if (extendableTrains.Count == 0)
                return null;
            else if (extendableTrains.Count == 1) {
                ExtendableTrainInfo extendableTrain = extendableTrains[0];

                return new ExtendTrainMenuItem("Extend train (" + extendableTrain.Train.DisplayName + ")", block, extendableTrain);
            }
            else {
                var extendTrainMenuItem = new LayoutMenuItem("&Extend Train");

                foreach (ExtendableTrainInfo extendableTrain in extendableTrains)
                    extendTrainMenuItem.DropDownItems.Add(new ExtendTrainMenuItem(extendableTrain.Train.DisplayName, block, extendableTrain));

                return extendTrainMenuItem;
            }
        }

        private class ExtendableTrainInfo {
            public TrainStateInfo Train;
            public LayoutBlockEdgeBase blockEdge;
            public LayoutBlock Block;

            public ExtendableTrainInfo(TrainStateInfo train, TrackEdge edge, LayoutBlock otherBlock) {
                this.Train = train;

                var blockEdge = edge.Track.BlockEdgeBase;

                this.blockEdge = blockEdge ?? throw new NullReferenceException(nameof(blockEdge));
                this.Block = otherBlock;
            }
        }

        private class ExtendTrainMenuItem : LayoutMenuItem {
            private readonly LayoutBlock block;
            private readonly ExtendableTrainInfo extendableTrainInfo;

            public ExtendTrainMenuItem(String title, LayoutBlock block, ExtendableTrainInfo extendableTrainInfo) {
                Text = title;
                this.block = block;
                this.extendableTrainInfo = extendableTrainInfo;
            }

            protected override void OnClick(EventArgs e) {
                int fromCount = 1, toCount = 1;

                if (extendableTrainInfo.blockEdge.IsTrackContact() && extendableTrainInfo.Train.TrackContactTriggerCount > 2) {
                    var extendTrainSettings = new Dialogs.ExtendTrainSettings(extendableTrainInfo.Train);
                    if (extendTrainSettings.ShowDialog() != DialogResult.OK)
                        return;

                    fromCount = extendTrainSettings.FromCount;
                    toCount = extendTrainSettings.ToCount;
                }

                // Get lock on the block that the train is about to be placed
                LayoutLockRequest lockRequest = new(extendableTrainInfo.Train.Id);

                lockRequest.Blocks.Add(block);
                Dispatch.Call.RequestLayoutLock(lockRequest);

                TrackContactPassingStateInfo? trackContactPassingState = null;

                if (extendableTrainInfo.blockEdge.IsTrackContact()) {
                    trackContactPassingState = new TrackContactPassingStateInfo(LayoutModel.StateManager.Components.StateOf(extendableTrainInfo.blockEdge, "TrainPassing")) {
                        Train = extendableTrainInfo.Train,
                        FromBlock = block,
                        ToBlock = extendableTrainInfo.Block,
                        FromBlockTriggerCount = fromCount,
                        ToBlockTriggerCount = toCount,
                        Direction = LocomotiveOrientation.Forward
                    };
                }

                var extendedTrainLocation = extendableTrainInfo.Train.LocationOfBlock(extendableTrainInfo.Block);
                TrainLocationInfo newTrainLocation;

                extendableTrainInfo.Train.LastBlockEdgeCrossingSpeed = 1;

                if (extendedTrainLocation != null && extendableTrainInfo.Block.BlockDefinintion.ContainsBlockEdge(extendedTrainLocation.DisplayFront, extendableTrainInfo.blockEdge)) {
                    newTrainLocation = extendableTrainInfo.Train.EnterBlock(TrainPart.Locomotive, block, extendableTrainInfo.blockEdge, (train, block) => Dispatch.Notification.OnTrainExtended(train, block));

                    extendableTrainInfo.Train.LastCrossedBlockEdge = extendableTrainInfo.blockEdge;

                    // Extended train location front should point to the direction opposite to the one of the track contact
                    // and direction should be Backward
                    if (block.BlockDefinintion.ContainsBlockEdge(0, extendableTrainInfo.blockEdge))
                        newTrainLocation.DisplayFront = block.BlockDefinintion.Track.ConnectionPoints[1];
                    else
                        newTrainLocation.DisplayFront = block.BlockDefinintion.Track.ConnectionPoints[0];

                    if (trackContactPassingState != null)
                        trackContactPassingState.Direction = LocomotiveOrientation.Forward;
                }
                else {
                    newTrainLocation = extendableTrainInfo.Train.EnterBlock(TrainPart.LastCar, block, extendableTrainInfo.blockEdge, (train, block) => Dispatch.Notification.OnTrainExtended(train, block));

                    // Extended train location front should point to the track contact direction and the track contact
                    // state direction should be forward
                    if (block.BlockDefinintion.ContainsBlockEdge(0, extendableTrainInfo.blockEdge))
                        newTrainLocation.DisplayFront = block.BlockDefinintion.Track.ConnectionPoints[0];
                    else
                        newTrainLocation.DisplayFront = block.BlockDefinintion.Track.ConnectionPoints[1];

                    int iCpIndex = block.BlockDefinintion.GetConnectionPointIndex(newTrainLocation.DisplayFront);
                    LayoutBlockEdgeBase[] otherBlockEdges = block.BlockDefinintion.GetBlockEdges(1 - iCpIndex);

                    if (otherBlockEdges == null || otherBlockEdges.Length == 0)
                        newTrainLocation.BlockEdge = null;
                    else
                        newTrainLocation.BlockEdge = otherBlockEdges[0];
                }
            }
        }

        #endregion

        #endregion

        #region Track contact component

        [DispatchTarget]
        [DispatchFilter("InOperationMode")]
        private bool IncludeInComponentContextMenu([DispatchFilter] LayoutTrackContactComponent component) => true;

        [DispatchTarget]
        [DispatchFilter(Type = "InOperationMode")]
        private void AddComponentContextMenuEntries(Guid frameWindowId, [DispatchFilter] LayoutTrackContactComponent component, MenuOrMenuItem menu) {
            menu.Items.Add(new LayoutComponentMenuItem(component, "&Trigger contact", (s, e) => Dispatch.Notification.OnTrackContactTriggered(component)));
        }

        #endregion

        #region Proximity Sensor component

        [DispatchTarget]
        [DispatchFilter("InOperationMode")]
        private bool IncludeInComponentContextMenu([DispatchFilter] LayoutProximitySensorComponent component) => true;

        [DispatchTarget]
        [DispatchFilter(Type = "InOperationMode")]
        private void AddComponentContextMenuEntries(Guid frameWindowId, [DispatchFilter] LayoutProximitySensorComponent component, MenuOrMenuItem menu) {
            menu.Items.Add(new LayoutComponentMenuItem(component, "&Proximity sensor active", (s, e) => Dispatch.Notification.OnProximitySensorStateChanged(component, !component.IsTriggered)) {
                Checked = component.IsTriggered
            });
        }

        #endregion

        #region Gate component

        [DispatchTarget]
        [DispatchFilter("InOperationMode")]
        private bool IncludeInComponentContextMenu([DispatchFilter] LayoutGateComponent component) => true;

        [DispatchTarget]
        [DispatchFilter(Type = "InOperationMode")]
        private void AddComponentContextMenuEntries(Guid frameWindowId, [DispatchFilter] LayoutGateComponent component, MenuOrMenuItem menu) {
            if (component.GateState != LayoutGateState.Open)
                menu.Items.Add("Open gate", null, (object? sender, EventArgs a) => Dispatch.Call.OpenGateRequest(component));

            if (component.GateState != LayoutGateState.Close)
                menu.Items.Add("Close gate", null, (object? sender, EventArgs a) => Dispatch.Call.CloseGateRequest(component));
        }

        #endregion

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
        }
        #endregion
    }
}
