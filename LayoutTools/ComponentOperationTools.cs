using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Xml;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Linq;
using LayoutManager.Model;
using LayoutManager.Components;
using LayoutManager.CommonUI.Controls;
using LayoutManager.View;

namespace LayoutManager.Tools {

    /// <summary>
    /// Summary description for ComponentTools.
    /// </summary>
    [LayoutModule("Components Operation Tools", UserControl = false)]
#pragma warning disable IDE0051, IDE0060
    public class ComponentOperationTools : System.ComponentModel.Component, ILayoutModuleSetup {
        /// <summary>
        /// Required designer variable.
        /// </summary>

        #region General services

        [LayoutEventDef("emergency-stop-request", Role = LayoutEventRole.Request)]
        [LayoutEvent("emergency-stop-request")]
        private void emergencyStopRequest(LayoutEvent e0) {
            var e = (LayoutEvent<IModelComponentIsCommandStation, string>)e0;

            if (e.Sender == null) {
                foreach (IModelComponentIsCommandStation commandStation in LayoutModel.Components<IModelComponentIsCommandStation>(LayoutModel.ActivePhases))
                    EventManager.Event(new LayoutEvent<IModelComponentIsCommandStation, string>("emergency-stop-request", commandStation, e.Info));
            }
            else {
                var commandStation = e.Sender;

                EventManager.Event(new LayoutEvent("disconnect-power-request", commandStation));
                EventManager.Event(new LayoutEvent("command-station-emergency-stop-notification", commandStation, e.Info));
            }
        }

        [LayoutEvent("command-station-emergency-stop-notification")]
        private void commandStationEmergencyStopNotification(LayoutEvent e) {
            IModelComponentIsCommandStation commandStation = (IModelComponentIsCommandStation)e.Sender;

            Form activeNotification = (Form)EventManager.Event(new LayoutEvent("get-command-station-notification-dialog", commandStation));

            if (activeNotification != null) {
                if (e.Info != null)
                    EventManager.Event(new LayoutEvent("update-command-station-notification-dialog", commandStation, (string)e.Info));

                activeNotification.Activate();
            }
            else {
                ILayoutFrameWindow frameWindow = LayoutController.ActiveFrameWindow;

                activeNotification = new Dialogs.CommandStationStopped(commandStation, (string)e.Info) {
                    Owner = frameWindow as Form
                };

                activeNotification.Show(LayoutController.ActiveFrameWindow);
            }
        }

        [LayoutEventDef("cancel-emergency-stop-request", Role = LayoutEventRole.Request)]
        [LayoutEvent("cancel-emergency-stop-request")]
        private void cancelEmergencyStopRequest(LayoutEvent e) {
            if (e.Sender == null) {
                foreach (IModelComponentIsCommandStation commandStation in LayoutModel.Components<IModelComponentIsCommandStation>(LayoutModel.ActivePhases))
                    EventManager.Event(new LayoutEvent("cancel-emergency-stop-request", commandStation));
            }
            else {
                IModelComponentIsCommandStation commandStation = (IModelComponentIsCommandStation)e.Sender;

                EventManager.Event(new LayoutEvent("connect-power-request", commandStation));
                EventManager.Event(new LayoutEvent("command-station-cancel-emergency-stop-notification", commandStation, "User initiated"));
            }
        }

        #endregion

        #region Utility Functions

        private void SetSwitchState(IModelComponentHasSwitchingState component, int switchState) {
            List<SwitchingCommand> switchingCommands = new List<SwitchingCommand>();

            component.AddSwitchingCommands(switchingCommands, switchState);
            EventManager.AsyncEvent(new LayoutEvent("set-track-components-state", this, switchingCommands, null));
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

        [LayoutEvent("query-component-operation-context-menu", SenderType = typeof(IModelComponentIsDualState))]
        void QueryDualStateComponentContextMenu(LayoutEvent e) {
            e.Info = e.Sender;
        }

        [LayoutEvent("add-component-operation-context-menu-entries", SenderType = typeof(IModelComponentIsDualState))]
        void AddTurnoutContextMenu(LayoutEvent e) {
            ModelComponent component = (ModelComponent)e.Sender;
            IModelComponentIsDualState multipath = (IModelComponentIsDualState)component;

            Menu menu = (Menu)e.Info;
            MenuItem item = new LayoutComponentMenuItem((ModelComponent)e.Sender, "&Toggle " + component.ToString(), new EventHandler(this.OnToggleDualStateComponent)) {
                DefaultItem = true
            };
            menu.MenuItems.Add(item);
        }

        void OnToggleDualStateComponent(Object sender, EventArgs e) {
            IModelComponentIsDualState multipath = (IModelComponentIsDualState)((LayoutComponentMenuItem)sender).Component;

            toggleDualStateComponent(multipath);
        }

        [LayoutEvent("default-action-command", SenderType = typeof(IModelComponentIsDualState))]
        private void dualStateComponentDefaultAction(LayoutEvent e) {
            IModelComponentIsDualState multipath = (IModelComponentIsDualState)e.Sender;

            toggleDualStateComponent(multipath);
        }

        private void toggleDualStateComponent(IModelComponentIsDualState multipath) {
            LayoutTrackComponent track = (LayoutTrackComponent)multipath;
            LayoutBlock block = track.GetBlock(track.ConnectionPoints[0]);
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

        [LayoutEvent("query-component-operation-context-menu", SenderType = typeof(LayoutSignalComponent))]
        void QuerySignalContextMenu(LayoutEvent e) {
            e.Info = e.Sender;
        }

        [LayoutEvent("add-component-operation-context-menu-entries", SenderType = typeof(LayoutSignalComponent))]
        void AddSignalContextMenu(LayoutEvent e) {
            Menu menu = (Menu)e.Info;
            MenuItem item = new LayoutComponentMenuItem((ModelComponent)e.Sender, "&Toggle signal", new EventHandler(this.OnToggleSignal)) {
                DefaultItem = true
            };
            menu.MenuItems.Add(item);
        }

        void OnToggleSignal(Object sender, EventArgs e) {
            LayoutSignalComponent signal = (LayoutSignalComponent)((LayoutComponentMenuItem)sender).Component;
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

        private LayoutBlockDefinitionComponent GetTrackBlockDefinition(LayoutStraightTrackComponent track) {
            if (track.BlockDefinitionComponent == null && track.TrackLinkComponent == null) {
                LayoutBlock block1 = track.GetBlock(track.ConnectionPoints[0]);
                LayoutBlock block2 = track.GetBlock(track.ConnectionPoints[1]);

                if (block1 == block2)
                    return block1.BlockDefinintion;
            }

            return null;
        }

        private bool ReflectEventToBlockDefinition(LayoutEvent e) {
            LayoutBlockDefinitionComponent blockDefinition = GetTrackBlockDefinition((LayoutStraightTrackComponent)e.Sender);

            if (blockDefinition != null) {
                object previousSender = e.Sender;

                e.Sender = blockDefinition;
                EventManager.Event(e);

                e.Sender = previousSender;
                return true;
            }
            else
                return false;
        }

        [LayoutEvent("query-component-operation-context-menu", SenderType = typeof(LayoutStraightTrackComponent))]
        private void queryStraightTrackContextMenu(LayoutEvent e) {
            if (!ReflectEventToBlockDefinition(e))
                e.Info = false;
        }

        [LayoutEvent("query-operation-drop", SenderType = typeof(LayoutStraightTrackComponent))]
        [LayoutEvent("do-operation-drop", SenderType = typeof(LayoutStraightTrackComponent))]
        [LayoutEvent("query-operation-drag", SenderType = typeof(LayoutStraightTrackComponent))]
        [LayoutEvent("get-component-operation-properties-menu-name", SenderType = typeof(LayoutStraightTrackComponent))]
        [LayoutEvent("add-component-operation-context-menu-entries", SenderType = typeof(LayoutStraightTrackComponent))]
        [LayoutEvent("add-operation-details-window-sections", SenderType = typeof(LayoutStraightTrackComponent))]
        [LayoutEvent("default-action-command", SenderType = typeof(LayoutStraightTrackComponent))]
        private void doRefelectEventToBlockDefinition(LayoutEvent e) {
            ReflectEventToBlockDefinition(e);
        }


        #endregion

        #region Block Definition Component

        [LayoutEvent("query-component-operation-context-menu", SenderType = typeof(LayoutBlockDefinitionComponent))]
        private void queryBlockInfoContextMenu(LayoutEvent e) {
            e.Info = e.Sender;
        }

        private void addBlockInfoMenuEntries(Menu m, LayoutBlockDefinitionComponent blockDefinition, IList<TrainStateInfo> trains) {
            bool defaultSet = false;
            LayoutBlock block = blockDefinition.Block;

            // Check if there any dialog that could take a block info for a way point
            List<ITripPlanEditorDialog> tripPlanEditorDialogs = new List<ITripPlanEditorDialog>();

            EventManager.Event(new LayoutEvent("query-edit-trip-plan-dialog", blockDefinition, tripPlanEditorDialogs, null));

            if (tripPlanEditorDialogs.Count == 1) {
                ITripPlanEditorDialog tripPlanEditorDialog = tripPlanEditorDialogs[0];

                MenuItem item = new AddBlockInfoToTripPlanEditorDialog(blockDefinition, tripPlanEditorDialog, "Add &Waypoint") {
                    DefaultItem = true
                };
                defaultSet = true;
                m.MenuItems.Add(item);
            }
            else if (tripPlanEditorDialogs.Count > 1) {
                MenuItem addWayPoint = new MenuItem("Add &Waypoint");

                foreach (ITripPlanEditorDialog tripPlanEditorDialog in tripPlanEditorDialogs)
                    addWayPoint.MenuItems.Add(new AddBlockInfoToTripPlanEditorDialog(blockDefinition, tripPlanEditorDialog, tripPlanEditorDialog.DialogName));

                addWayPoint.DefaultItem = true;
                defaultSet = true;
                m.MenuItems.Add(addWayPoint);
            }

            // Check if there any dialog that could take a block info for a trip-plan origin
            var dialogs = new List<IModelComponentReceiverDialog>();

            dialogs.Clear();

            EventManager.Event(new LayoutEvent<LayoutBlockDefinitionComponent, List<IModelComponentReceiverDialog>>("query-execute-trip-plan-dialog", blockDefinition, dialogs));

            if (dialogs.Count == 1) {
                var dialog = dialogs[0];

                MenuItem item = new AddBlockInfoToDialogMenuItem(blockDefinition, dialog, "Add &Trip-plan start location");

                if (!defaultSet) {
                    item.DefaultItem = true;
                    defaultSet = true;
                }

                m.MenuItems.Add(item);
            }
            else if (dialogs.Count > 1) {
                MenuItem addLocation = new MenuItem("Add &Trip-plan start location");

                foreach (var dialog in dialogs)
                    addLocation.MenuItems.Add(new AddBlockInfoToDialogMenuItem(blockDefinition, dialog, dialog.DialogName(blockDefinition)));

                m.MenuItems.Add(addLocation);
            }

            // Check if there any dialog that could take a block info for a destination location
            dialogs.Clear();

            EventManager.Event(new LayoutEvent<LayoutBlockDefinitionComponent, List<IModelComponentReceiverDialog>>("query-edit-destination-dialog", blockDefinition, dialogs));

            if (dialogs.Count == 1) {
                var dialog = dialogs[0];
                var item = new AddBlockInfoToDialogMenuItem(blockDefinition, dialog, "Add &Location");

                if (!defaultSet) {
                    item.DefaultItem = true;
                    defaultSet = true;
                }

                m.MenuItems.Add(item);
            }
            else if (dialogs.Count > 1) {
                MenuItem addLocation = new MenuItem("Add &Location");

                foreach (IModelComponentReceiverDialog dialog in dialogs)
                    addLocation.MenuItems.Add(new AddBlockInfoToDialogMenuItem(blockDefinition, dialog, dialog.DialogName(blockDefinition)));

                m.MenuItems.Add(addLocation);
            }

            // Check if there any dialog that could take a block info for a destination location
            dialogs.Clear();

            EventManager.Event(new LayoutEvent<LayoutBlockDefinitionComponent, List<IModelComponentReceiverDialog>>("query-edit-manual-dispatch-region-dialog", blockDefinition, dialogs));

            Debug.Assert(dialogs.Count < 2);

            if (dialogs.Count == 1) {
                var dialog = dialogs[0];
                var item = new AddBlockInfoToDialogMenuItem(blockDefinition, dialog, "Add Block to &Manual Dispatch Region");

                if (!defaultSet) {
                    item.DefaultItem = true;
                    defaultSet = true;
                }

                m.MenuItems.Add(item);
            }

            if (block.LockRequest != null && block.LockRequest.IsManualDispatchLock) {
                if (m.MenuItems.Count > 0)
                    m.MenuItems.Add("-");

                addManualDispatchRouteMenuEntries(blockDefinition, m);
                m.MenuItems.Add("-");
            }

            // Check of block belongs to region that can accept more than one power
            if (blockDefinition.PowerConnector.Inlet.ConnectedOutlet.ObtainablePowers.Count() > 1)
                addPowerConnectionEntries(blockDefinition, m);

            // Check if this block info has trains in it.

            if (trains.Count == 0) {
                var context = LayoutOperationContext.GetPendingOperation("TrainPlacement", blockDefinition);

                if (context == null) {
                    m.MenuItems.Add("Place on track...",
                        delegate (object sender, EventArgs e) {
                            Dialogs.SelectTrainToPlace d = new LayoutManager.Tools.Dialogs.SelectTrainToPlace(blockDefinition);
                            if (d.ShowDialog() == DialogResult.OK) {
                                DoValidateAndPlaceTrain(new LayoutEvent<LayoutBlockDefinitionComponent, XmlElement>("validate-and-place-train-request", blockDefinition, d.Selected.Element).SetOption("Train", "Front", d.Front.ToString()).SetOption("Train", "Length", d.Length.ToString()));
                            }
                        });
                }
                else
                    m.MenuItems.Add("Cancel " + context.Description, (s, ea) => context.Cancel());

                if (blockDefinition.Info.UnexpectedTrainDetected) {
                    MenuItem identifyTrainMenuItem = getRelocateTrainMenuItem(blockDefinition);

                    if (identifyTrainMenuItem.MenuItems.Count > 0)
                        m.MenuItems.Add(identifyTrainMenuItem);
                }
                else {
                    var otherTrains = new List<TrainStateInfo>();

                    if (!block.IsLocked || block.LockRequest.IsManualDispatchLock)
                        otherTrains.AddRange(LayoutModel.StateManager.Trains);
                    else {
                        var train = LayoutModel.StateManager.Trains[block.LockRequest.OwnerId];

                        if (train != null)
                            otherTrains.Add(train);
                    }

                    if (otherTrains.Count == 1)
                        m.MenuItems.Add($"Fix: train '{otherTrains.First().DisplayName}' located here",
                            (sender, e1) => fixTrainLocation(otherTrains.First(), blockDefinition));
                    else if (otherTrains.Count > 1) {
                        var fixTrainLocationMenu = new MenuItem("Fix: train which is located here");

                        foreach (var train in otherTrains)
                            fixTrainLocationMenu.MenuItems.Add(train.DisplayName, (sender, e1) => fixTrainLocation(train, blockDefinition));

                        m.MenuItems.Add(fixTrainLocationMenu);
                    }
                }

                // Check if trains in neighboring blocks can be extended
                MenuItem extendTrainMenuItem = (MenuItem)EventManager.Event(new LayoutEvent("get-extend-train-menu", blockDefinition));

                if (extendTrainMenuItem != null)
                    m.MenuItems.Add(extendTrainMenuItem);

            }
            else {
                if (trains.Count == 1) {
                    TrainStateInfo train = trains[0];

                    if (train.IsPowered) {
                        if (blockDefinition.Block.LockRequest == null || !blockDefinition.Block.LockRequest.IsManualDispatchLock) {
                            MenuItem item = new NewTripPlanMenuItem("New &Trip plan", train, blockDefinition);

                            if (!defaultSet && train.Speed == 0) {
                                item.DefaultItem = true;
                                defaultSet = true;
                            }

                            m.MenuItems.Add(item);

                            m.MenuItems.Add(new TripPlanCatalogMenuItem("&Saved Trip-plans...", trains[0]));
                            m.MenuItems.Add("-");
                        }
                    }

                    EventManager.Event(new LayoutEvent<TrainStateInfo, Menu>("add-train-operation-menu", trains[0], m));
                }
                else {
                    foreach (TrainStateInfo train in trains) {
                        MenuItem trainItem = new MenuItem(train.DisplayName);

                        if (train.IsPowered) {

                            if (blockDefinition.Block.LockRequest == null || !blockDefinition.Block.LockRequest.IsManualDispatchLock) {
                                trainItem.MenuItems.Add(new NewTripPlanMenuItem(train.DisplayName, train, blockDefinition));
                                trainItem.MenuItems.Add(new TripPlanCatalogMenuItem("&Saved Trip-plans...", train));
                                trainItem.MenuItems.Add("-");
                            }
                        }

                        EventManager.Event(new LayoutEvent<TrainStateInfo, Menu>("add-train-operation-menu", train, trainItem));

                        if (trainItem.MenuItems.Count > 0)
                            m.MenuItems.Add(trainItem);
                    }
                }
            }

            if (blockDefinition.Info.IsOccupancyDetectionBlock)
                m.MenuItems.Add(new ToggleTrainDetectionStateMenuItem(blockDefinition));
        }

        [LayoutEvent("add-train-operation-menu")]
        private void addFixTrainOrientationMenu(LayoutEvent e0) {
            var e = (LayoutEvent<TrainStateInfo, Menu>)e0;
            var train = e.Sender;
            var m = e.Info;

            m.MenuItems.Add("Fix: reverse train orientation", (sender, e1) => {
                foreach (var trainLocation in train.Locations) {
                    if (trainLocation.IsDisplayFrontKnown) {
                        var reversedFront = trainLocation.Block.BlockDefinintion.Track.ConnectTo(trainLocation.DisplayFront, LayoutComponentConnectionType.Passage).FirstOrDefault();

                        trainLocation.DisplayFront = reversedFront;
                        trainLocation.Block.BlockDefinintion.OnComponentChanged();
                    }
                }
            });
        }

        void fixTrainLocation(TrainStateInfo train, LayoutBlockDefinitionComponent blockDefinition) {
            var getFrontDialog = new Dialogs.TrainFront(blockDefinition, train.DisplayName);

            if (getFrontDialog.ShowDialog() == DialogResult.OK) {
                // Verify that can lock this block for new train
                if (!blockDefinition.Block.IsLocked ||
                    blockDefinition.Block.LockRequest.IsManualDispatchLock || blockDefinition.Block.LockRequest.OwnerId == train.Id) {

                    // Remove train from old position
                    LayoutModel.StateManager.Trains.RemoveTrainFromTrack(train);
                    EventManager.Event(new LayoutEventInfoValueType<object, Guid>("free-owned-layout-locks", null, train.Id).SetOption("ReleasePending", true));

                    // Place the train in the new location

                    // Obtain lock for train if needed
                    if (!blockDefinition.Block.IsLocked) {
                        LayoutLockRequest lockRequest = new LayoutLockRequest(train.Id);

                        lockRequest.Blocks.Add(blockDefinition.Block);
                        EventManager.Event(new LayoutEvent("request-layout-lock", lockRequest));
                    }

                    EventManager.Event(
                        new LayoutEvent<TrainStateInfo, LayoutBlockDefinitionComponent>(
                            "place-train-in-block", train, blockDefinition).SetOption("Train", "Front", getFrontDialog.Front.ToString()
                        )
                    );
                }
            }
        }

        private void addPowerConnectionEntries(LayoutBlockDefinitionComponent blockDefinition, Menu m) {
            var menuItems = new List<MenuItem>();

            foreach (var power in blockDefinition.PowerConnector.Inlet.ConnectedOutlet.ObtainablePowers) {
                if (power.PowerOriginComponentId != blockDefinition.Power.PowerOriginComponentId || power.Type != blockDefinition.Power.Type) {
                    ILayoutPower p = power;
                    void setPower(object s, EventArgs ea) => EventManager.AsyncEvent(new LayoutEvent<LayoutBlockDefinitionComponent, ILayoutPower>("set-power", blockDefinition, p));

                    switch (p.Type) {
                        case LayoutPowerType.Disconnected:
                            menuItems.Add(new MenuItem("Disconnect from power", setPower));
                            break;

                        case LayoutPowerType.Programmer:
                            // Add option for this, only if all the power region is a manual dispatch region
                            if (blockDefinition.PowerConnector.Blocks.All(b => b.LockRequest != null && b.LockRequest.IsManualDispatchLock))
                                menuItems.Add(new MenuItem("Connect to programming power", setPower));
                            break;

                        default:
                            menuItems.Add(new MenuItem("Connect to " + p.PowerOriginComponent.NameProvider.Name + " " + p.Name + " power", setPower));
                            break;
                    }
                }
            }

            if (menuItems.Count > 0) {
                if (m.MenuItems.Count > 0 && m.MenuItems[m.MenuItems.Count - 1].Text != "-")
                    m.MenuItems.Add("-");
                m.MenuItems.AddRange(menuItems.ToArray());
                m.MenuItems.Add("-");
            }
        }

        [LayoutEvent("add-train-operation-menu")]
        private void addTrainOperationMenu(LayoutEvent e0) {
            var e = (LayoutEvent<TrainStateInfo, Menu>)e0;
            TrainStateInfo train = e.Sender;
            Menu m = e.Info;
            bool isDefaultSet = false;

            foreach (MenuItem item in m.MenuItems)
                if (item.DefaultItem) {
                    isDefaultSet = true;
                    break;
                }

            if (train.IsPowered) {
                MenuItem showControllerItem = new LocomotiveOperationTools.ShowLocomotiveControllerMenuItem(train);

                m.MenuItems.Add(showControllerItem);
                if (showControllerItem.Enabled && !isDefaultSet) {
                    showControllerItem.DefaultItem = true;
                }
            }

            m.MenuItems.Add(new SaveTrainInCollectionMenuItem(train));
            m.MenuItems.Add(new LocomotiveOperationTools.TrainPropertiesMenuItem(train));
            m.MenuItems.Add("-");
            if (LayoutOperationContext.HasPendingOperation("TrainRemoval", train))
                m.MenuItems.Add(new LocomotiveOperationTools.CancelTrainRemovalMenuItem(train));
            else
                m.MenuItems.Add(new LocomotiveOperationTools.RemoveFromTrackMenuItem(train));
            m.MenuItems.Add("-");
        }

        [LayoutEvent("query-operation-drop", SenderType = typeof(LayoutBlockDefinitionComponent))]
        private void blockDefinitionQueryDrop(LayoutEvent e) {
            LayoutBlockDefinitionComponent blockDefinition = (LayoutBlockDefinitionComponent)e.Sender;
            DragEventArgs dragEvent = (DragEventArgs)e.Info;

            if (dragEvent.Data.GetData(typeof(XmlElement)) is XmlElement element) {
                if (element.Name == "Locomotive" || element.Name == "Train") {
                    if (!blockDefinition.Block.HasTrains) {
                        TrainStateInfo train = LayoutModel.StateManager.Trains[element];

                        if (train == null) {        // Not already on track
                            CanPlaceTrainResult result;

                            result = (CanPlaceTrainResult)EventManager.Event(new LayoutEvent("can-locomotive-be-placed", element, blockDefinition, null));

                            if (result.CanBeResolved)
                                dragEvent.Effect = DragDropEffects.Link;
                        }
                    }
                }
                else if (element.Name == "TrainState") {
                    TrainStateInfo train = new TrainStateInfo(element);

                    if (IsTrainOnSamePower(train, blockDefinition)) {
                        // Check whether this is a train identification case (the block with the train
                        // is an occupancy detection block with no train detected) or whether it is a shortcut
                        // for moving the train from this poinr to the next
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
                                dragEvent.Effect = DragDropEffects.Copy;
                        }
                        else if (blockDefinition.Block.CanTrainWait)
                            dragEvent.Effect = DragDropEffects.Move;
                    }
                }
            }
        }

        public static async void DoValidateAndPlaceTrain(LayoutEvent e0) {
            var e = (LayoutEvent<LayoutBlockDefinitionComponent, XmlElement>)e0;
            var placedElement = e.Info;
            var blockDefinition = e.Sender;
            string name = placedElement.Name == "Locomotive" ? new LocomotiveInfo(placedElement).Name : new TrainInCollectionInfo(placedElement).Name;

            using (var context = new LayoutOperationContext("TrainPlacement", "Placing " + name + " on track", new LayoutXmlWithIdWrapper(placedElement), blockDefinition)) {
                try {
                    await EventManager.AsyncEvent(e.SetOperationContext(context));
                }
                catch (LayoutException ex) {
                    ex.Report();
                }
                catch (OperationCanceledException) { }
            }
        }

        [LayoutEvent("do-operation-drop", SenderType = typeof(LayoutBlockDefinitionComponent))]
        private void blockDefinitionDoDrop(LayoutEvent e) {
            LayoutBlockDefinitionComponent blockDefinition = (LayoutBlockDefinitionComponent)e.Sender;
            DragEventArgs dragEvent = (DragEventArgs)e.Info;

            if (dragEvent.Data.GetData(typeof(XmlElement)) is XmlElement element) {
                if (element.Name == "Locomotive" || element.Name == "Train") {
                    if (!blockDefinition.Block.HasTrains) {
                        TrainStateInfo train = LayoutModel.StateManager.Trains[element];

                        // Check that train is not already on track and that there are not other pending placement operation on this loco/train or block
                        if (train == null &&
                         !LayoutOperationContext.HasPendingOperation("TrainPlacement", new LayoutXmlWithIdWrapper(element)) &&
                         !LayoutOperationContext.HasPendingOperation("TrainPlacement", blockDefinition))
                            DoValidateAndPlaceTrain(new LayoutEvent<LayoutBlockDefinitionComponent, XmlElement>("validate-and-place-train-request", blockDefinition, element));
                    }
                }
                else if (element.Name == "TrainState") {
                    TrainStateInfo train = new TrainStateInfo(element);

                    if (dragEvent.Effect == DragDropEffects.Copy) {
                        EventManager.Event(new LayoutEvent("relocate-train-request", train, blockDefinition, null));
                    }
                    else if (dragEvent.Effect == DragDropEffects.Move) {
                        // Create trip plan to move train
                        TripPlanInfo tripPlan = new TripPlanInfo();

                        tripPlan.Add(blockDefinition);

                        Dialogs.TripPlanEditor tripPlanEditor = new Dialogs.TripPlanEditor(tripPlan, train);

                        tripPlanEditor.Show();
                    }
                }
            }
        }

        [LayoutEvent("get-block-smart-destination-list")]
        private void getBlockSmartDestinationList(LayoutEvent e) {
            LayoutBlockDefinitionComponent blockDefinition = (LayoutBlockDefinitionComponent)e.Sender;
            List<TripPlanDestinationInfo> destinations = (List<TripPlanDestinationInfo>)e.Info;

            foreach (TripPlanDestinationInfo tripPlanDestination in LayoutModel.StateManager.TripPlansCatalog.Destinations) {
                foreach (LayoutBlockDefinitionComponent destinationBlockDefinition in tripPlanDestination.BlockInfoList)
                    if (blockDefinition.Id == destinationBlockDefinition.Id) {
                        destinations.Add(tripPlanDestination);
                        break;
                    }
            }
        }

        [LayoutEvent("query-operation-drag", SenderType = typeof(LayoutBlockDefinitionComponent))]
        private void blockDefinitionQueryOperationDrag(LayoutEvent e) {
            LayoutBlockDefinitionComponent blockDefinition = (LayoutBlockDefinitionComponent)e.Sender;

            if (blockDefinition.Block.Trains.Count == 1)
                e.Info = blockDefinition.Block.Trains[0].Train.Element;
        }

        [LayoutEvent("get-component-operation-properties-menu-name", SenderType = typeof(LayoutBlockDefinitionComponent))]
        private void getBlockInfoPropertiesMenuName(LayoutEvent e) {
            LayoutBlockDefinitionComponent blockInfo = (LayoutBlockDefinitionComponent)e.Sender;
            string menuName = (string)e.Info;

            if (LayoutModel.StateManager.Trains[blockInfo.Block].Count > 0)
                e.Info = "Block " + menuName;
        }

        [LayoutEvent("add-component-operation-context-menu-entries", SenderType = typeof(LayoutBlockDefinitionComponent))]
        private void addBlockInfoContextMenu(LayoutEvent e) {
            Menu m = (Menu)e.Info;
            LayoutBlockDefinitionComponent blockInfo = (LayoutBlockDefinitionComponent)e.Sender;
            IList<TrainStateInfo> trains = LayoutModel.StateManager.Trains[blockInfo.Block];

            addBlockInfoMenuEntries(m, blockInfo, trains);
        }

        [LayoutEvent("add-operation-details-window-sections", SenderType = typeof(LayoutBlockDefinitionComponent), Order = 1000)]
        private void addBlockDefinitionTrainsDetailsWindowSection(LayoutEvent e) {
            LayoutBlockDefinitionComponent blockDefinition = (LayoutBlockDefinitionComponent)e.Sender;
            PopupWindowContainerSection container = (PopupWindowContainerSection)e.Info;

            if (blockDefinition.Block.HasTrains)
                foreach (TrainLocationInfo trainLocation in blockDefinition.Block.Trains)
                    EventManager.Event(new LayoutEvent("add-train-details-window-section", trainLocation.Train, container, null));
            else {
                LayoutLockRequest lockRequest = blockDefinition.Block.LockRequest;

                if (lockRequest != null) {
                    if (lockRequest.IsManualDispatchLock) {
                        ManualDispatchRegionInfo manualDispatchRegion = LayoutModel.StateManager.ManualDispatchRegions[lockRequest.OwnerId];

                        if (manualDispatchRegion != null)
                            container.AddText("Part of manual dispatch region: " + manualDispatchRegion.Name);
                        else
                            container.AddText("Part of manual dispatch region");
                    }
                    else {
                        TrainStateInfo train = LayoutModel.StateManager.Trains[lockRequest.OwnerId];

                        if (train != null) {
                            container.AddText("Block allocated to:");

                            EventManager.Event(new LayoutEvent("add-train-details-window-section", train, container, null));
                        }
                    }
                }
            }
        }

        [LayoutEvent("add-train-details-window-section", SenderType = typeof(TrainStateInfo))]
        private void addTrainDetailsWindowSetion(LayoutEvent e) {
            TrainStateInfo train = (TrainStateInfo)e.Sender;
            PopupWindowContainerSection container = (PopupWindowContainerSection)e.Info;
            PopupWindowContainerSection trainContainer = container.CreateContainer();

            trainContainer.AddVerticalSection(new PopupWindowTextSection(train.Length.ToDisplayString() + " train: " + train.DisplayName + ": " + train.StatusText));

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

            addTrainDetailsPopupSections(trainContainer, train);
            container.AddVerticalSection(trainContainer);

            if (train.Attributes.Count > 0)
                container.AddVerticalSection(new PopupWindowAttributesSection("Train attributes: ", train));

            if (train.SpeedLimit > 0)
                container.AddText("Train speed limit: " + train.SpeedLimit);
        }


        private void addTrainDetailsPopupSections(PopupWindowContainerSection container, TrainStateInfo train) {
            foreach (TrainLocomotiveInfo trainLocomotive in train.Locomotives) {
                PopupWindowContainerSection locoContainer = container.CreateContainer();

                addLocomotiveDetailsPopupSections(locoContainer, trainLocomotive);
                container.AddHorizontalSection(locoContainer);
            }

            //TODO: Add train attributes
        }

        private void addLocomotiveDetailsPopupSections(PopupWindowContainerSection locoContainer, TrainLocomotiveInfo trainLocomotive) {
            Image locoImage = trainLocomotive.Locomotive.Image;

            if (locoImage == null)
                locoImage = LayoutModel.LocomotiveCatalog.GetStandardImage(trainLocomotive.Locomotive.Kind, trainLocomotive.Locomotive.Origin);

            if (trainLocomotive.Orientation == LocomotiveOrientation.Backward)
                locoImage.RotateFlip(RotateFlipType.RotateNoneFlipX);

            PopupWindowContainerSection locoImageContainer = locoContainer.CreateContainer();

            locoImageContainer.BorderPen = Pens.Black;
            locoImageContainer.OuterMargins = new Size(0, 0);
            locoImageContainer.InnerMargins = new Size(1, 1);
            locoImageContainer.AddVerticalSection(new PopupWindowImageSection(locoImage));
            locoContainer.AddVerticalSection(locoImageContainer);
        }

        #region Find route on manual dispatch region

        class ManualDispatchRouteSetting {
            LayoutBlockDefinitionComponent sourceBlockInfo = null;
            Guid sourceRegionID = Guid.Empty;

            public LayoutBlockDefinitionComponent SourceBlockInfo {
                get {
                    return sourceBlockInfo;
                }

                set {
                    sourceBlockInfo = value;
                    if (sourceBlockInfo == null)
                        sourceRegionID = Guid.Empty;
                    else
                        sourceRegionID = sourceBlockInfo.Block.LockRequest.OwnerId;
                }
            }

            public Guid SourceRegionID => sourceRegionID;
        }

        readonly ManualDispatchRouteSetting manualDispatchRouteSetting = new ManualDispatchRouteSetting();

        [LayoutEvent("exit-operation-mode")]
        private void resetRouteSource(LayoutEvent e) {
            manualDispatchRouteSetting.SourceBlockInfo = null;
        }

        private void addManualDispatchRouteMenuEntries(LayoutBlockDefinitionComponent blockInfo, Menu m) {
            LayoutLockRequest lockRequest = blockInfo.Block.LockRequest;

            if (lockRequest != null && lockRequest.IsManualDispatchLock) {
                if (manualDispatchRouteSetting.SourceBlockInfo != null && manualDispatchRouteSetting.SourceRegionID == lockRequest.OwnerId && manualDispatchRouteSetting.SourceBlockInfo.Id != blockInfo.Id)
                    m.MenuItems.Add(new CreateManualDispatchRouteMenuItem(manualDispatchRouteSetting, blockInfo));
                m.MenuItems.Add(new SetManualDispatchRouteSourceMenuItem(manualDispatchRouteSetting, blockInfo));
            }
        }

        #region Route creation menu items

        class SetManualDispatchRouteSourceMenuItem : MenuItem {
            readonly ManualDispatchRouteSetting manualDispatchRouteSetting;
            readonly LayoutBlockDefinitionComponent blockInfo;

            public SetManualDispatchRouteSourceMenuItem(ManualDispatchRouteSetting manualDispatchRouteSetting, LayoutBlockDefinitionComponent blockInfo) {
                this.manualDispatchRouteSetting = manualDispatchRouteSetting;
                this.blockInfo = blockInfo;

                Text = "&Set Route Origin";
            }

            protected override void OnClick(EventArgs e) {
                manualDispatchRouteSetting.SourceBlockInfo = blockInfo;
            }
        }

        class CreateManualDispatchRouteMenuItem : MenuItem {
            readonly ManualDispatchRouteSetting manualDispatchRouteSetting;
            readonly LayoutBlockDefinitionComponent blockInfo;
            readonly LocomotiveOrientation direction = LocomotiveOrientation.Unknown;

            public CreateManualDispatchRouteMenuItem(ManualDispatchRouteSetting manualDispatchRouteSetting, LayoutBlockDefinitionComponent blockInfo) {
                this.manualDispatchRouteSetting = manualDispatchRouteSetting;
                this.blockInfo = blockInfo;

                Text = "&Create route";
                MenuItems.Add(new CreateManualDispatchRouteMenuItem(manualDispatchRouteSetting, blockInfo, LocomotiveOrientation.Forward));
                MenuItems.Add(new CreateManualDispatchRouteMenuItem(manualDispatchRouteSetting, blockInfo, LocomotiveOrientation.Backward));
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
                IRoutePlanningServices ts = (IRoutePlanningServices)EventManager.Event(new LayoutEvent("get-route-planning-services", this));
                IList<TrainLocationInfo> trainLocations = manualDispatchRouteSetting.SourceBlockInfo.Block.Trains;
                LayoutComponentConnectionPoint front = LayoutComponentConnectionPoint.Empty;
                bool findRoute = true;

                if (trainLocations.Count > 0)
                    front = trainLocations[0].DisplayFront;
                else {
                    object oFront = EventManager.Event(new LayoutEvent("get-locomotive-front", manualDispatchRouteSetting.SourceBlockInfo, manualDispatchRouteSetting.SourceBlockInfo.Name));

                    if (oFront == null)
                        findRoute = false;
                    else
                        front = (LayoutComponentConnectionPoint)oFront;
                }

                if (findRoute) {
                    BestRoute bestRoute = ts.FindBestRoute(manualDispatchRouteSetting.SourceBlockInfo, front, direction, blockInfo, manualDispatchRouteSetting.SourceRegionID, true);

                    if (bestRoute.Quality.IsValidRoute) {
                        bool completed = (bool)EventManager.Event(new LayoutEvent("dispatcher-set-switches", manualDispatchRouteSetting.SourceRegionID, bestRoute));

                        if (!completed) {
                            LayoutSelection selection = new LayoutSelection(new ModelComponent[] { manualDispatchRouteSetting.SourceBlockInfo, blockInfo });

                            LayoutModuleBase.Error(selection, "Could not find route that is within the manual dispatch region");
                        }
                    }
                    else {
                        LayoutSelection selection = new LayoutSelection(new ModelComponent[] { manualDispatchRouteSetting.SourceBlockInfo, blockInfo });

                        LayoutModuleBase.Error(selection, "No route could be found");
                    }
                }
            }
        }

        #endregion

        #endregion

        class AddBlockInfoToTripPlanEditorDialog : MenuItem {
            readonly LayoutBlockDefinitionComponent blockDefinition;
            readonly ITripPlanEditorDialog tripPlanEditorDialog;

            public AddBlockInfoToTripPlanEditorDialog(LayoutBlockDefinitionComponent blockDefinition, ITripPlanEditorDialog tripPlanEditorDialog, string title) {
                List<TripPlanDestinationInfo> smartDestinations = new List<TripPlanDestinationInfo>();

                Text = title;
                this.blockDefinition = blockDefinition;
                this.tripPlanEditorDialog = tripPlanEditorDialog;

                EventManager.Event(new LayoutEvent("get-block-smart-destination-list", blockDefinition, smartDestinations, null));

                if (smartDestinations.Count > 0) {
                    MenuItems.Add("This specific block", delegate (object sender, EventArgs e) { tripPlanEditorDialog.AddWayPoint(blockDefinition); });
                    MenuItems.Add("-");

                    foreach (TripPlanDestinationInfo tripPlanDestination in smartDestinations) {
                        TripPlanDestinationInfo destination = tripPlanDestination;

                        MenuItems.Add(tripPlanDestination.Name, delegate (object sender, EventArgs e) { tripPlanEditorDialog.AddWayPoint(destination); });
                    }
                }
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                tripPlanEditorDialog.AddWayPoint(blockDefinition);
            }
        }

        #region Add waypoint to trip plan

        class AddBlockInfoToDialogMenuItem : MenuItem {
            readonly LayoutBlockDefinitionComponent blockInfo;
            readonly IModelComponentReceiverDialog dialog;

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


        private MenuItem getUseSavedTripPlanMenuItem(LayoutBlockDefinitionComponent blockInfo, TrainStateInfo train) {
            ArrayList tripPlansAtOrigin = new ArrayList();
            MenuItem m;

            XmlDocument applicableTripPlansDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();
            XmlElement applicableTripPlansElement = applicableTripPlansDoc.CreateElement("ApplicableTripPlans");

            applicableTripPlansDoc.AppendChild(applicableTripPlansElement);
            EventManager.Event(new LayoutEvent("get-applicable-trip-plans-request", train, applicableTripPlansElement, null).SetOption("CalculatePenalty", false));

            foreach (XmlElement applicableTripPlanElement in applicableTripPlansElement) {
                TripPlanInfo tripPlan = LayoutModel.StateManager.TripPlansCatalog.TripPlans[XmlConvert.ToGuid(applicableTripPlanElement.GetAttribute("TripPlanID"))];
                bool shouldReverse = XmlConvert.ToBoolean(applicableTripPlanElement.GetAttribute("ShouldReverse"));

                tripPlansAtOrigin.Add(new UseExistingTripPlanMenuItem(tripPlan, shouldReverse, train));
            }

            MenuItem tripPlanCatalog = new TripPlanCatalogMenuItem("&Saved Trip-plans...", train);

            if (tripPlansAtOrigin.Count == 0)
                m = tripPlanCatalog;
            else {
                m = new MenuItem("Use existing trip plan");

                m.MenuItems.Add(tripPlanCatalog);
                m.MenuItems.Add("-");

                foreach (MenuItem menuItem in tripPlansAtOrigin)
                    m.MenuItems.Add(menuItem);


                return m;
            }

            if (train.Locomotives.Count == 0)
                m.Enabled = false;

            return m;
        }

        class NewTripPlanMenuItem : MenuItem {
            readonly TrainStateInfo train;

            public NewTripPlanMenuItem(string title, TrainStateInfo train, LayoutBlockDefinitionComponent blockInfo) {
                this.Text = title;
                this.train = train;

                if (train.Locomotives.Count == 0)
                    Enabled = false;
            }

            protected override void OnClick(EventArgs e) {
                Form dialog = (Form)EventManager.Event(new LayoutEvent("query-edit-trip-plan-for-train", train));

                if (dialog != null)
                    dialog.Activate();
                else {
                    TripPlanInfo tripPlan = new TripPlanInfo();

                    Dialogs.TripPlanEditor tripPlanEditor = new Dialogs.TripPlanEditor(tripPlan, train);

                    tripPlanEditor.Show();
                }
            }
        }

        class TripPlanCatalogMenuItem : MenuItem {
            readonly TrainStateInfo train;

            public TripPlanCatalogMenuItem(string title, TrainStateInfo train) {
                this.Text = title;
                this.train = train;

                if (train.Locomotives.Count == 0)
                    Enabled = false;
            }

            protected override void OnClick(EventArgs e) {
                if ((bool)EventManager.Event(new LayoutEvent("show-saved-trip-plans-for-train", train, false, null)) == false) {
                    Dialogs.TripPlanCatalog tripPlanCatalog = new Dialogs.TripPlanCatalog(train);

                    tripPlanCatalog.Show();
                }
            }
        }

        class UseExistingTripPlanMenuItem : MenuItem {
            readonly TripPlanInfo tripPlan;
            readonly TrainStateInfo train;
            readonly bool shouldReverse;

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

                TripPlanInfo newTripPlan = new TripPlanInfo(tripPlanDoc.DocumentElement);

                if (shouldReverse)
                    newTripPlan.Reverse();

                newTripPlan.FromCatalog = true;

                Dialogs.TripPlanEditor tripPlanEditor = new Dialogs.TripPlanEditor(newTripPlan, train);

                tripPlanEditor.Show();
            }
        }

        #endregion

        #region Save train in collection

        class SaveTrainInCollectionMenuItem : MenuItem {
            readonly TrainStateInfo train;

            public SaveTrainInCollectionMenuItem(TrainStateInfo train) {
                this.train = train;

                Text = "&Save Train in Collection";
            }

            protected override void OnClick(EventArgs e) {
                LocomotiveCollectionInfo collection = LayoutModel.LocomotiveCollection;
                XmlElement trainInCollectionElement = collection["Train", train.Name];
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
                EventManager.Event(new LayoutEvent("train-saved-in-collection", trainInCollection));
                collection.Save();
            }
        }

        #endregion

        #region Locomotive owner draw menu item base class

        class LocomotiveMenuItem : MenuItem {
            readonly XmlElement placedElement;
            readonly LayoutBlock block;

            public LocomotiveMenuItem(XmlElement placedElement, LayoutBlock block) {
                this.placedElement = placedElement;
                this.block = block;

                this.OwnerDraw = true;
                this.Text = placedElement["Name"].InnerText;
            }

            protected XmlElement PlacedElement => placedElement;

            protected LayoutBlock Block => block;

            protected override void OnMeasureItem(MeasureItemEventArgs e) {
                SizeF textSize;
                int imageWidth;
                int textShift = 0;
                String name;

                base.OnMeasureItem(e);
                e.ItemHeight = 50;

                if (placedElement.Name == "Locomotive") {
                    LocomotiveInfo loco = new LocomotiveInfo(placedElement);

                    name = loco.DisplayName;
                    imageWidth = 50;
                    textShift = 55;
                }
                else {
                    TrainInCollectionInfo trainInCollection = new TrainInCollectionInfo(placedElement);

                    name = trainInCollection.DisplayName;
                    imageWidth = 2 + (28 + 2) * trainInCollection.Locomotives.Count;
                }

                using (Font titleFont = new Font("Arial", 8, FontStyle.Bold)) {
                    textSize = e.Graphics.MeasureString(name, titleFont);
                }

                e.ItemWidth = Math.Max((int)textSize.Width + textShift, imageWidth);
            }

            protected override void OnDrawItem(DrawItemEventArgs e) {
                base.OnDrawItem(e);

                using (Brush br = new SolidBrush((e.State & DrawItemState.Selected) != 0 ? SystemColors.Highlight : SystemColors.Menu))
                    e.Graphics.FillRectangle(br, e.Bounds);

                e.DrawFocusRectangle();

                GraphicsState gs = e.Graphics.Save();

                e.Graphics.TranslateTransform(e.Bounds.Left, e.Bounds.Top);

                // Draw the locomotive image and name. There are two possible layouts one
                // for locomotive and one for locomotive set
                int xText;
                float yText;
                String name;

                if (placedElement.Name == "Locomotive") {
                    LocomotiveInfo loco = new LocomotiveInfo(placedElement);

                    using (LocomotiveImagePainter locoPainter = new LocomotiveImagePainter(LayoutModel.LocomotiveCatalog)) {
                        locoPainter.Draw(e.Graphics, new Point(2, 2), new Size(50, 36), loco.Element);
                    };

                    yText = 2;
                    xText = 55;
                    name = loco.DisplayName;
                }
                else if (placedElement.Name == "Train" || placedElement.Name == "TrainState") {
                    TrainCommonInfo train = new TrainCommonInfo(placedElement);

                    using (LocomotiveImagePainter locoPainter = new LocomotiveImagePainter(LayoutModel.LocomotiveCatalog)) {
                        int x = 2;

                        locoPainter.FrameSize = new Size(28, 20);

                        foreach (TrainLocomotiveInfo trainLocomotive in train.Locomotives) {

                            locoPainter.LocomotiveElement = trainLocomotive.Locomotive.Element;
                            locoPainter.FlipImage = (trainLocomotive.Orientation == LocomotiveOrientation.Backward);
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

                using (Brush textBrush = new SolidBrush((e.State & DrawItemState.Selected) != 0 ? SystemColors.HighlightText : SystemColors.MenuText)) {
                    SizeF textSize;

                    using (Font titleFont = new Font("Arial", 8, FontStyle.Bold)) {
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

        private MenuItem getRelocateTrainMenuItem(LayoutBlockDefinitionComponent blockInfo) {
            MenuItem menu = new MenuItem("Identify train");
            IDictionary addedTrains = new HybridDictionary();

            foreach (XmlElement trainElement in LayoutModel.StateManager.Trains.Element) {
                TrainStateInfo train = new TrainStateInfo(trainElement);

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
                        menu.MenuItems.Add(new RelocateTrainMenuItem(trainElement, blockInfo.Block));
                    }
                }
            }

            if (menu.MenuItems.Count > 0)
                menu.MenuItems.Add("-");

            foreach (XmlElement trainElement in LayoutModel.StateManager.Trains.Element) {
                TrainStateInfo train = new TrainStateInfo(trainElement);

                if (IsTrainOnSamePower(train, blockInfo) && !addedTrains.Contains(train.Id))
                    menu.MenuItems.Add(new RelocateTrainMenuItem(trainElement, blockInfo.Block));
            }

            return menu;
        }

        class RelocateTrainMenuItem : LocomotiveMenuItem {
            public RelocateTrainMenuItem(XmlElement placedElement, LayoutBlock block) : base(placedElement, block) {
            }

            protected override void OnClick(EventArgs e) {
                EventManager.Event(new LayoutEvent("relocate-train-request", PlacedElement, Block.BlockDefinintion, null));
            }
        }

        #endregion

        #region Toggle train detection state

        class ToggleTrainDetectionStateMenuItem : MenuItem {
            readonly LayoutBlockDefinitionComponent blockInfo;

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

        #region Remove ballon if track is detected

        [LayoutEvent("train-detection-block-occupied")]
        private void removeBallonWhenTrainIsDetected(LayoutEvent e) {
            LayoutOccupancyBlock occupancyBlock = (LayoutOccupancyBlock)e.Sender;

            // Check if any contained block contains train. If so, then the detection was of this train, and nothing should be done
            foreach (LayoutBlock block in occupancyBlock.ContainedBlocks) {
                LayoutBlockDefinitionComponent blockDefinition = block.BlockDefinintion;

                if (LayoutBlockBallon.IsDisplayed(blockDefinition) && LayoutBlockBallon.Get(blockDefinition).RemoveOnTrainDetected)
                    LayoutBlockBallon.Remove(blockDefinition, LayoutBlockBallon.TerminationReason.TrainDetected);
            }
        }

        #endregion

        [LayoutEvent("default-action-command", SenderType = typeof(LayoutBlockDefinitionComponent))]
        private void blockDefinitionDefaultAction(LayoutEvent e) {
            LayoutHitTestResult hitTestResult = (LayoutHitTestResult)e.Info;
            LayoutBlockDefinitionComponent blockInfo = null;
            List<TrainStateInfo> trains = new List<TrainStateInfo>();

            foreach (ModelComponent component in hitTestResult.Selection)
                if (component is LayoutBlockDefinitionComponent) {
                    blockInfo = (LayoutBlockDefinitionComponent)component;
                    break;
                }

            if (blockInfo == null) {
                blockInfo = (LayoutBlockDefinitionComponent)e.Sender;

                foreach (TrainLocationInfo trainLocation in blockInfo.Block.Trains)
                    trains.Add(trainLocation.Train);
            }

            ContextMenu m = new ContextMenu();

            addBlockInfoMenuEntries(m, blockInfo, trains.ToArray());

            foreach (MenuItem item in m.MenuItems) {
                if (item.DefaultItem) {
                    if (item.MenuItems.Count > 0) {
                        List<MenuItem> subitems = new List<MenuItem>();

                        foreach (MenuItem subitem in item.MenuItems)
                            subitems.Add(subitem);

                        ContextMenu contextMenu = new ContextMenu(subitems.ToArray());
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

        [LayoutEvent("add-operation-details-window-sections", SenderType = typeof(LayoutTrackPowerConnectorComponent), Order = 1000)]
        private void addTrackPowerConnectorDetailsWindowSection(LayoutEvent e) {
            var component = (LayoutTrackPowerConnectorComponent)e.Sender;
            PopupWindowContainerSection container = (PopupWindowContainerSection)e.Info;

            if (component.Inlet.IsConnected) {
                ILayoutPower power = component.Inlet.ConnectedOutlet.Power;
                string powerDescription;
                string origin;

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

        [LayoutEvent("get-extend-train-menu")]
        private void getExtendTrainMenu(LayoutEvent e) {
            LayoutBlockDefinitionComponent blockInfo = (LayoutBlockDefinitionComponent)e.Sender;
            LayoutBlock block = blockInfo.Block;
            ArrayList extendableTrains = new ArrayList();

            foreach (TrackEdge edge in block.TrackEdges) {
                LayoutComponentConnectionPoint otherBlockCp = (edge.ConnectionPoint == edge.Track.ConnectionPoints[0]) ? edge.Track.ConnectionPoints[1] : edge.Track.ConnectionPoints[0];
                LayoutBlock otherBlock = edge.Track.GetBlock(otherBlockCp);

                if (otherBlock.Trains.Count == 1 && (otherBlock.Trains[0].Train.TrackContactTriggerCount > 1 || block.OccupancyBlock != null)) {
                    extendableTrains.Add(new ExtendableTrainInfo(otherBlock.Trains[0].Train, edge, otherBlock));
                }
            }

            if (extendableTrains.Count == 0)
                e.Info = null;
            else if (extendableTrains.Count == 1) {
                ExtendableTrainInfo extendableTrain = (ExtendableTrainInfo)extendableTrains[0];

                e.Info = new ExtendTrainMenuItem("Extend train (" + extendableTrain.Train.DisplayName + ")", block, extendableTrain);
            }
            else {
                MenuItem extendTrainMenuItem = new MenuItem("&Extend Train");

                foreach (ExtendableTrainInfo extendableTrain in extendableTrains)
                    extendTrainMenuItem.MenuItems.Add(new ExtendTrainMenuItem(extendableTrain.Train.DisplayName, block, extendableTrain));

                e.Info = extendTrainMenuItem;
            }
        }

        class ExtendableTrainInfo {
            public TrainStateInfo Train;
            public LayoutBlockEdgeBase blockEdge;
            public LayoutBlock Block;

            public ExtendableTrainInfo(TrainStateInfo train, TrackEdge edge, LayoutBlock otherBlock) {
                this.Train = train;
                this.blockEdge = (LayoutBlockEdgeBase)edge.Track.BlockEdgeBase;
                this.Block = otherBlock;
            }
        }

        class ExtendTrainMenuItem : MenuItem {
            readonly LayoutBlock block;
            readonly ExtendableTrainInfo extendableTrainInfo;

            public ExtendTrainMenuItem(String title, LayoutBlock block, ExtendableTrainInfo extendableTrainInfo) {
                Text = title;
                this.block = block;
                this.extendableTrainInfo = extendableTrainInfo;
            }

            [LayoutEventDef("train-extended", Role = LayoutEventRole.Notification, SenderType = typeof(TrainStateInfo), InfoType = typeof(LayoutBlock))]
            protected override void OnClick(EventArgs e) {
                int fromCount = 1, toCount = 1;

                if (extendableTrainInfo.blockEdge.IsTrackContact() && extendableTrainInfo.Train.TrackContactTriggerCount > 2) {
                    Dialogs.ExtendTrainSettings extendTrainSettings = new Dialogs.ExtendTrainSettings(extendableTrainInfo.Train);

                    if (extendTrainSettings.ShowDialog() != DialogResult.OK)
                        return;

                    fromCount = extendTrainSettings.FromCount;
                    toCount = extendTrainSettings.ToCount;
                }

                // Get lock on the block that the train is about to be placed
                LayoutLockRequest lockRequest = new LayoutLockRequest(extendableTrainInfo.Train.Id);

                lockRequest.Blocks.Add(block);
                EventManager.Event(new LayoutEvent("request-layout-lock", lockRequest));

                TrackContactPassingStateInfo trackContactPassingState = null;

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

                TrainLocationInfo extendedTrainLocation = extendableTrainInfo.Train.LocationOfBlock(extendableTrainInfo.Block);
                TrainLocationInfo newTrainLocation;

                extendableTrainInfo.Train.LastBlockEdgeCrossingSpeed = 1;

                if (extendableTrainInfo.Block.BlockDefinintion.ContainsBlockEdge(extendedTrainLocation.DisplayFront, extendableTrainInfo.blockEdge)) {
                    newTrainLocation = extendableTrainInfo.Train.EnterBlock(TrainPart.Locomotive, block, extendableTrainInfo.blockEdge, "train-extended");

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
                    newTrainLocation = extendableTrainInfo.Train.EnterBlock(TrainPart.LastCar, block, extendableTrainInfo.blockEdge, "train-extended");

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

        [LayoutEvent("query-component-operation-context-menu", SenderType = typeof(LayoutTrackContactComponent))]
        void QueryTrackContactContextMenu(LayoutEvent e) {
            e.Info = e.Sender;
        }

        [LayoutEvent("add-component-operation-context-menu-entries", SenderType = typeof(LayoutTrackContactComponent))]
        void AddTrackContactContextMenu(LayoutEvent e) {
            Menu menu = (Menu)e.Info;
            MenuItem item = new LayoutComponentMenuItem((ModelComponent)e.Sender, "&Trigger contact", new EventHandler(this.onTriggetTrackContact));

            menu.MenuItems.Add(item);
        }

        private void onTriggetTrackContact(Object sender, EventArgs e) {
            LayoutTrackContactComponent trackContact = (LayoutTrackContactComponent)((LayoutComponentMenuItem)sender).Component;

            EventManager.Event(new LayoutEvent("track-contact-triggered-notification", trackContact));
        }

        #endregion

        #region Gate component

        [LayoutEvent("query-component-operation-context-menu", SenderType = typeof(LayoutGateComponent))]
        void QueryGateContextMenu(LayoutEvent e) {
            e.Info = e.Sender;
        }

        [LayoutEvent("add-component-operation-context-menu-entries", SenderType = typeof(LayoutGateComponent))]
        void AddGateContextMenu(LayoutEvent e) {
            LayoutGateComponent gateComponent = (LayoutGateComponent)e.Sender;
            Menu menu = (Menu)e.Info;

            if (gateComponent.GateState != LayoutGateState.Open)
                menu.MenuItems.Add("Open gate", delegate (object sender, EventArgs a) { EventManager.Event(new LayoutEvent("open-gate-request", gateComponent)); });

            if (gateComponent.GateState != LayoutGateState.Close)
                menu.MenuItems.Add("Close gate", delegate (object sender, EventArgs a) { EventManager.Event(new LayoutEvent("close-gate-request", gateComponent)); });
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
