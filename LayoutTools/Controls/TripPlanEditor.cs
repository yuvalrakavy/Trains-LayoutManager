using LayoutManager.CommonUI;
using LayoutManager.Components;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Tools.Controls {
    /// <summary>
    /// Summary description for TripPlanEditor.
    /// </summary>
    public partial class TripPlanEditor : System.Windows.Forms.UserControl, ITripPlanEditorDialog, CommonUI.Controls.IPolicyListCustomizer {
        private const string A_PolicyID = "PolicyID";
        private const string E_RunPolicy = "RunPolicy";

        private TripPlanInfo? tripPlan = null;
        private TrainStateInfo? train = null;
        private bool initialized = false;
        private bool viewOnly = false;
        private bool buildStartConditionMenu = true;
        private bool buildDriverInstructionsMenu = true;
        private string? dialogName = null;
        private Dialogs.DestinationEditor? activeDestinationEditor = null;
        private readonly List<RoutePreviewRequest> previewRequests = new();
        private IDictionary<Guid, string> displayedBalloons = new Dictionary<Guid, string>();

        public event EventHandler? WayPointCountChanged = null;
        private DialogEditing? changeToViewOnly = null;

        private LayoutSelection? tripPlanSelection;
        private LayoutSelection? selectedWaypointSelection;

        public TripPlanEditor() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        #region Exposed properties / methods

        public void Initialize() {
            Dispatch.AddObjectInstanceDispatcherTargets(this);
        }

        public LayoutBlock? LocomotiveBlock { get; set; } = null;

        public LayoutComponentConnectionPoint Front { get; set; } = LayoutComponentConnectionPoint.Empty;

        public TrainStateInfo Train { get => Ensure.NotNull<TrainStateInfo>(OptionalTrain); set => OptionalTrain = value; }

        public TrainStateInfo? OptionalTrain {
            get => train;

            set {
                if (value != null) {
                    train = value;
                    LocomotiveBlock = train.LocomotiveBlock;
                    Front = train.LocomotiveLocation?.DisplayFront ?? LayoutComponentConnectionPoint.Empty;
                }
            }
        }

        public bool EnablePreview { get; set; } = true;

        public int TrainTargetWaypoint { get; set; } = -1;

        public TripPlanInfo TripPlan { get => Ensure.NotNull<TripPlanInfo>(OptionalTripPlan); set => OptionalTripPlan = value; }

        [Browsable(false)]
        public TripPlanInfo? OptionalTripPlan {
            set {
                if (!DesignMode) {
                    listViewWayPoints.Items.Clear();

                    if (value != null) {
                        tripPlan = value;

                        if (value != null) {
                            this.checkBoxTripPlanCircular.Checked = tripPlan.IsCircular;

                            foreach (TripPlanWaypointInfo wayPoint in tripPlan.Waypoints)
                                listViewWayPoints.Items.Add(new WayPointItem(wayPoint));
                        }

                        ClearSelections();

                        tripPlanSelection = tripPlan.Selection;
                        selectedWaypointSelection = new LayoutSelection();

                        DisplaySelections();

                        attributesEditor.AttributesSource = typeof(TripPlanInfo);
                        attributesEditor.AttributesOwner = new AttributesOwner(tripPlan.Element);

                        policyList.Customizer = this;
                        policyList.Scope = "TripPlan";
                        policyList.Policies = LayoutModel.StateManager.TripPlanPolicies;
                        policyList.Initialize();

                        UpdateButtons(null, EventArgs.Empty);
                        initialized = true;
                    }
                    else {
                        tripPlan = null;
                        ClearSelections();
                        UpdateButtons(null, EventArgs.Empty);
                    }
                }
            }

            get {
                if (!DesignMode && tripPlan != null)
                    tripPlan.IsCircular = checkBoxTripPlanCircular.Checked;
                return tripPlan;
            }
        }

        private void DisplaySelections() {
            if (tripPlanSelection != null)
                tripPlanSelection.Display(new LayoutSelectionLook(Color.Orange));
            if (selectedWaypointSelection != null)
                selectedWaypointSelection.Display(new LayoutSelectionLook(Color.OrangeRed));
        }

        private void HideSelections() {
            if (tripPlanSelection != null)
                tripPlanSelection.Hide();
            if (selectedWaypointSelection != null)
                selectedWaypointSelection.Hide();
        }

        private void ClearSelections() {
            if (tripPlanSelection != null) {
                tripPlanSelection.Hide();
                tripPlanSelection.Clear();
            }

            if (selectedWaypointSelection != null) {
                selectedWaypointSelection.Hide();
                selectedWaypointSelection.Clear();
            }
        }

        protected void UpdateButtons(object? sender, EventArgs e) {
            if (listViewWayPoints.SelectedItems.Count == 0) {
                buttonEditWaypoint.Enabled = false;
                buttonRemoveWaypoint.Enabled = false;
            }
            else {
                buttonEditWaypoint.Enabled = true;
                buttonRemoveWaypoint.Enabled = true;
            }

            if (listViewWayPoints.SelectedItems.Count > 0) {
                int selectedItemIndex = listViewWayPoints.SelectedIndices[0];

                buttonWayPointMoveUp.Enabled = selectedItemIndex > 0;
                buttonWayPointMoveDown.Enabled = selectedItemIndex < listViewWayPoints.Items.Count - 1;
            }
            else {
                buttonWayPointMoveUp.Enabled = false;
                buttonWayPointMoveDown.Enabled = false;
            }

            UpdatePreviewRequests();
        }

        #region Preview requests handlings

        private void ClearPreviewRequests() {
            foreach (RoutePreviewRequest previewRequest in previewRequests)
                LayoutController.PreviewRouteManager.Remove(previewRequest);

            previewRequests.Clear();
        }

        private TripBestRouteResult FindBestRoute(TripBestRouteRequest bestRouteRequest) {
            var bestRoute = Dispatch.Call.FindBestRouteRequest(bestRouteRequest);

            if (bestRoute.BestRoute == null) {
                bestRouteRequest.Direction = (bestRouteRequest.Direction == LocomotiveOrientation.Forward) ? LocomotiveOrientation.Backward : LocomotiveOrientation.Forward;

                bestRoute = Dispatch.Call.FindBestRouteRequest(bestRouteRequest);

                if (bestRoute.BestRoute != null)
                    bestRoute.ShouldReverse = true;
            }

            return bestRoute;
        }

        private bool AddPreviewRequest(TripBestRouteResult result, WayPointItem wayPointItem, IDictionary balloons, bool anySelected, bool selected) {
            if (result.BestRoute != null) {
                RoutePreviewRequest request;

                wayPointItem.NoPass = false;
                if (result.ShouldReverse) {
                    wayPointItem.ShouldReverese = true;
                    request = new RoutePreviewRequest(Color.Red, result.BestRoute, 3);
                }
                else {
                    wayPointItem.ShouldReverese = false;

                    request = new RoutePreviewRequest(
                        (result.BestRoute.Direction == LocomotiveOrientation.Forward) ? Color.LightGreen : Color.Green, result.BestRoute, (!anySelected || selected) ? 3 : -1);
                }

                request.Selected = selected;
                previewRequests.Add(request);

                if (wayPointItem.WayPoint.StartCondition != null) {
                    LayoutTrackComponent track = result.BestRoute.SourceTrack;
                    const int chopAfter = 40;

                    if (track.Spot[ModelComponentKind.BlockInfo] is LayoutBlockDefinitionComponent blockInfo) {
                        string s = wayPointItem.WayPoint.StartConditionDescription;

                        if (s.Length > chopAfter) {
                            int i;

                            for (i = chopAfter; i < s.Length && Char.IsLetterOrDigit(s, i); i++)
                                ;

                            s = string.Concat(s.AsSpan(0, i), "...");
                        }

                        if (!balloons.Contains(blockInfo.Id))
                            balloons.Add(blockInfo.Id, s);
                    }
                }

                return true;
            }
            else {
                wayPointItem.NoPass = true;
                return false;
            }
        }

        private void UpdateBalloons(IDictionary<Guid, string> newBalloons) {
            foreach (var d in newBalloons) {
                displayedBalloons.TryGetValue(d.Key, out string? displayedBalloonText);

                if (displayedBalloonText != d.Value) {
                    var blockDefinition = LayoutModel.Component<LayoutBlockDefinitionComponent>((Guid)d.Key, LayoutModel.ActivePhases);
                    var balloon = new LayoutBlockBalloon();

                    Debug.Assert(blockDefinition != null);

                    balloon.FillColor = Color.Yellow;
                    balloon.TextColor = Color.Black;
                    balloon.FontSize = 10f;
                    balloon.Text = d.Value;

                    LayoutBlockBalloon.Show(blockDefinition, balloon);
                }

                if (displayedBalloonText != null)
                    displayedBalloons.Remove(d.Key);
            }

            // At this stage, the only entries in displayedBalloons are entries that do not exist in newBalloons and
            // therefore should be erased
            foreach (var d in displayedBalloons) {
                var blockDefinition = LayoutModel.Component<LayoutBlockDefinitionComponent>((Guid)d.Key, LayoutPhase.All);

                if (blockDefinition != null)
                    LayoutBlockBalloon.Remove(blockDefinition, LayoutBlockBalloon.TerminationReason.Hidden);
            }

            displayedBalloons = newBalloons;
        }

        private void UpdatePreviewRequests() {
            ClearPreviewRequests();

            if (listViewWayPoints.Items.Count > 0 && EnablePreview) {
                int wayPointIndex = (TrainTargetWaypoint < 0) ? 0 : TrainTargetWaypoint;
                WayPointItem wayPointItem = (WayPointItem)listViewWayPoints.Items[wayPointIndex];
                TripPlanWaypointInfo wayPoint = wayPointItem.WayPoint;
                TripBestRouteResult result;
                Guid routeOwner = (train != null) ? Train.Id : Guid.Empty;
                bool addMore;
                int selectedIndex = listViewWayPoints.SelectedItems.Count > 0 ? listViewWayPoints.SelectedIndices[0] : -1;
                var balloons = new Dictionary<Guid, string>();

                if (LocomotiveBlock != null) {
                    TripBestRouteRequest request = new(routeOwner, wayPoint.Destination, LocomotiveBlock.BlockDefinintion.Track,
                        Front, wayPoint.Direction, wayPoint.TrainStopping);

                    result = FindBestRoute(request);
                    addMore = AddPreviewRequest(result, wayPointItem, balloons, selectedIndex != -1, selectedIndex == wayPointIndex);
                }
                else
                    throw new ArgumentException("No train/locomotive-block is defined");

                // Now continue with the rest of the trip plan
                for (wayPointIndex++; addMore && wayPointIndex < listViewWayPoints.Items.Count; wayPointIndex++) {
                    wayPointItem = (WayPointItem)listViewWayPoints.Items[wayPointIndex];
                    wayPoint = wayPointItem.WayPoint;

                    TripBestRouteRequest request = new(routeOwner, wayPoint.Destination, result.BestRoute.DestinationEdge.Track,
                        result.BestRoute.DestinationFront, wayPoint.Direction, wayPoint.TrainStopping);

                    result = FindBestRoute(request);
                    addMore = AddPreviewRequest(result, wayPointItem, balloons, selectedIndex != -1, selectedIndex == wayPointIndex);
                }

                if (addMore) {
                    // If the trip is circular, add another segment to the first way point
                    if (checkBoxTripPlanCircular.Checked) {
                        wayPointIndex = 0;
                        wayPointItem = (WayPointItem)listViewWayPoints.Items[wayPointIndex];
                        wayPoint = wayPointItem.WayPoint;

                        TripBestRouteRequest request = new(routeOwner, wayPoint.Destination, result.BestRoute.DestinationEdge.Track,
                            result.BestRoute.DestinationFront, wayPoint.Direction, wayPoint.TrainStopping);

                        result = FindBestRoute(request);
                        AddPreviewRequest(result, wayPointItem, balloons, selectedIndex != -1, selectedIndex == wayPointIndex);
                    }
                }

                foreach (RoutePreviewRequest request in previewRequests) {
                    if (!request.Selected)
                        LayoutController.PreviewRouteManager.Add(request);
                }

                foreach (RoutePreviewRequest request in previewRequests) {
                    if (request.Selected)
                        LayoutController.PreviewRouteManager.Add(request);
                }

                for (int i = 0; i < listViewWayPoints.Items.Count; i++) {
                    WayPointItem item = (WayPointItem)listViewWayPoints.Items[i];

                    item.TrainTarget = i == TrainTargetWaypoint;
                }

                listViewWayPoints.Invalidate();
                UpdateBalloons(balloons);
            }
        }

        #endregion

        // Implementation of IPolicyListCustomizer

        public bool IsPolicyChecked(LayoutPolicyInfo policy) => TripPlan.Policies.Contains(policy.Id);

        public void SetPolicyChecked(LayoutPolicyInfo policy, bool checkValue) {
            if (checkValue)
                TripPlan.Policies.Add(policy.Id);
            else
                TripPlan.Policies.Remove(policy.Id);
        }

        public bool ViewOnly {
            get {
                return viewOnly;
            }

            set {
                if (!viewOnly && value) {
                    changeToViewOnly = new DialogEditing(this, new DialogEditingCommandBase[] {
                                                                                                  new DialogEditingRemoveControl(buttonAddWaypoint),
                                                                                                  new DialogEditingRemoveControl(buttonRemoveWaypoint),
                                                                                                  new DialogEditingRemoveControl(buttonWayPointMoveDown),
                                                                                                  new DialogEditingRemoveControl(buttonWayPointMoveUp),
                                                                                                  new DialogEditingMoveControl(buttonEditWaypoint, new Point(groupBoxWayPoints.Left, buttonEditWaypoint.Top)),
                                                                                                  new DialogEditingRemoveMenuEntry(contextMenuEdit, menuItemEditDirection),
                                                                                                  new DialogEditingChangeText(buttonEditWaypoint, "&View")
                                                                                              }
                        );
                    changeToViewOnly.Do();
                    changeToViewOnly.ViewOnly = true;
                }
                else if (viewOnly && !value) {
                    if (changeToViewOnly != null) {
                        changeToViewOnly.Undo();
                        changeToViewOnly.ViewOnly = false;
                        changeToViewOnly = null;
                    }
                }

                viewOnly = value;
            }
        }

        public string DialogName => dialogName ?? String.Empty;

        public void SetDialogName(string dialogName) {
            this.dialogName = dialogName;
        }

        public string? TripPlanName { get; set; } = null;

        public Form? ActiveForm {
            get {
                return activeDestinationEditor ?? (ParentForm.Enabled ? ParentForm : null);
            }
        }

        public int Count => listViewWayPoints.Items.Count;

        public void AddWayPoint(TripPlanWaypointInfo newWaypoint) {
            listViewWayPoints.Items.Add(new WayPointItem(newWaypoint));

            if (tripPlanSelection != null)
                tripPlanSelection.Add(newWaypoint.Destination.Selection);

            UpdateButtons(null, EventArgs.Empty);

            WayPointCountChanged?.Invoke(this, EventArgs.Empty);
        }

        public void AddWayPoint(LayoutBlockDefinitionComponent blockInfo) {
            TripPlanWaypointInfo newWaypoint = TripPlan.Add(blockInfo);

            AddWayPoint(newWaypoint);
        }

        public void AddWayPoint(TripPlanDestinationInfo destination) {
            XmlElement addedDestinationElement = (XmlElement)TripPlan.Element.OwnerDocument.ImportNode(destination.Element, true);
            TripPlanDestinationInfo addedDestination = new(addedDestinationElement);
            TripPlanWaypointInfo newWaypoint = TripPlan.Add(addedDestination);

            AddWayPoint(newWaypoint);
        }

        public bool Check() {
            foreach (WayPointItem wayPointItem in listViewWayPoints.Items) {
                TripPlanWaypointInfo wayPoint = wayPointItem.WayPoint;

                foreach (LayoutBlockDefinitionComponent blockInfo in wayPoint.Destination.BlockInfoList) {
                    // Check that each destinaion is a waitable block
                    if (!blockInfo.Info.CanTrainWait) {
                        wayPointItem.Selected = true;
                        listViewWayPoints.Focus();

                        MessageBox.Show(this, "Destination must be a block in which the train is allowed to wait", "Invalid Destination", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }

            attributesEditor.Commit();

            // TODO: Validate routes, check that a route can be formed
            return true;
        }

        public void UpdateAll() {
            foreach (WayPointItem wayPointItem in listViewWayPoints.Items)
                wayPointItem.Update();
        }

        public void SelectWayPoint(int wayPointIndex) {
            listViewWayPoints.Items[wayPointIndex].Selected = true;
        }

        #endregion

        #region Layout Event Handler

        [DispatchTarget]
        private void QueryEditTripPlanDialog(LayoutBlockDefinitionComponent blockDefinitions, List<ITripPlanEditorDialog> dialogs) {
            if (Enabled && !viewOnly)
                dialogs.Add(this);
        }

        [DispatchTarget]
        private TripPlanAssignmentInfo? GetEditedTrip(TrainStateInfo train) {
            return train.Id == this.Train.Id ? new TripPlanAssignmentInfo(TripPlan, train) : null;
        }

        private void PolicyCollectionUpdated(LayoutPoliciesCollection policiesCollection) {
            if (policiesCollection == LayoutModel.StateManager.RideStartPolicies)
                buildStartConditionMenu = true;
            else if (policiesCollection == LayoutModel.StateManager.DriverInstructionsPolicies)
                buildDriverInstructionsMenu = true;
        }

        [DispatchTarget]
        private void OnPolicyAddedToPoliciesCollection(LayoutPoliciesCollection policies, LayoutPolicyInfo policy) => PolicyCollectionUpdated(policies);

        [DispatchTarget]
        void OnRemovingPolicyFromPoliciesCollection(LayoutPoliciesCollection policies, LayoutPolicyInfo policy) => PolicyCollectionUpdated(policies);

        #endregion

            /// <summary> 
            /// Clean up any resources being used.
            /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                EventManager.Subscriptions.RemoveObjectSubscriptions(this);

                if (initialized) {
                    policyList.Dispose();

                    HideSelections();

                    tripPlanSelection?.Clear();
                    tripPlanSelection = null;

                    selectedWaypointSelection?.Clear();
                    selectedWaypointSelection = null;

                    if (activeDestinationEditor != null)
                        activeDestinationEditor.Close();

                    ClearPreviewRequests();
                    UpdateBalloons(new Dictionary<Guid, string>());
                }

                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }


        #region Event Handlers

        private void ListViewWayPoints_SelectedIndexChanged(object? sender, System.EventArgs e) {
            if (selectedWaypointSelection != null) {
                selectedWaypointSelection.Clear();

                foreach (WayPointItem wayPointItem in listViewWayPoints.SelectedItems)
                    selectedWaypointSelection.Add(wayPointItem.WayPoint.Destination.Selection);

                if (selectedWaypointSelection.Count > 0)
                    Dispatch.Call.EnsureComponentVisible(LayoutController.ActiveFrameWindow.Id, selectedWaypointSelection.Components.First(), false);

                UpdateButtons(sender, e);
            }
        }

        private void ButtonRemoveWaypoint_Click(object? sender, System.EventArgs e) {
            if (listViewWayPoints.SelectedItems.Count > 0) {
                WayPointItem selected = (WayPointItem)listViewWayPoints.SelectedItems[0];

                listViewWayPoints.Items.Remove(selected);
                tripPlanSelection?.Remove(selected.WayPoint.Destination.Selection);
                tripPlan?.Remove(selected.WayPoint);
                UpdateButtons(null, EventArgs.Empty);

                WayPointCountChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void ButtonWayPointMoveDown_Click(object? sender, System.EventArgs e) {
            if (listViewWayPoints.SelectedItems.Count > 0) {
                int selectedIndex = listViewWayPoints.SelectedIndices[0];

                if (selectedIndex < listViewWayPoints.Items.Count - 1) {
                    WayPointItem selected = (WayPointItem)listViewWayPoints.SelectedItems[0];
                    TripPlanWaypointInfo selectedWaypoint = selected.WayPoint;
                    var nextElement = selectedWaypoint.Element.NextSibling;

                    selectedWaypoint.Element.ParentNode?.RemoveChild(selectedWaypoint.Element);
                    nextElement?.ParentNode?.InsertAfter(selectedWaypoint.Element, nextElement);

                    listViewWayPoints.Items.Remove(selected);
                    listViewWayPoints.Items.Insert(selectedIndex + 1, selected);
                    UpdateButtons(null, EventArgs.Empty);
                }
            }
        }

        private void ButtonWayPointMoveUp_Click(object? sender, System.EventArgs e) {
            if (listViewWayPoints.SelectedItems.Count > 0) {
                int selectedIndex = listViewWayPoints.SelectedIndices[0];

                if (selectedIndex > 0) {
                    WayPointItem selected = (WayPointItem)listViewWayPoints.SelectedItems[0];
                    TripPlanWaypointInfo selectedWaypoint = selected.WayPoint;
                    var previousElement = selectedWaypoint.Element.PreviousSibling;

                    selectedWaypoint.Element.ParentNode?.RemoveChild(selectedWaypoint.Element);
                    previousElement?.ParentNode?.InsertBefore(selectedWaypoint.Element, previousElement);

                    listViewWayPoints.Items.Remove(selected);
                    listViewWayPoints.Items.Insert(selectedIndex - 1, selected);
                    UpdateButtons(null, EventArgs.Empty);
                }
            }
        }

        private WayPointItem? GetSelectedWaypoint() {
            return listViewWayPoints.SelectedItems.Count == 0 ? null : (WayPointItem)listViewWayPoints.SelectedItems[0];
        }

        private void ButtonEditWaypoint_Click(object? sender, System.EventArgs e) {
            var selected = GetSelectedWaypoint();
            bool canSetDriverInstructions;

            if (selected == null || selected.Index == 0)
                canSetDriverInstructions = true;
            else {
                WayPointItem previous = (WayPointItem)listViewWayPoints.Items[selected.Index - 1];

                if (selected.WayPoint.StartCondition != null)
                    canSetDriverInstructions = true;
                else if (selected.WayPoint.Direction != previous.WayPoint.Direction)
                    canSetDriverInstructions = true;
                else
                    canSetDriverInstructions = false;
            }

            if (buildStartConditionMenu) {
                BuildStartConditionMenu(menuItemEditStartCondition);
                buildStartConditionMenu = false;
            }

            if (canSetDriverInstructions && buildDriverInstructionsMenu) {
                BuildDriverInstructionsMenu(menuItemEditDriverInstructions);
                buildDriverInstructionsMenu = false;
            }

            menuItemEditDriverInstructions.Enabled = canSetDriverInstructions;

            contextMenuEdit.Show(groupBoxWayPoints, new Point(buttonEditWaypoint.Left, buttonEditWaypoint.Bottom));
        }

        private void BuildDriverInstructionsMenu(ToolStripMenuItem m) {
            var selectedWaypoint = GetSelectedWaypoint();

            if (selectedWaypoint != null) {
                m.DropDownItems.Clear();
                m.DropDownItems.Add("No instructions", null, (object? s, EventArgs e) => selectedWaypoint.DriverInstructions = null);
                m.DropDownItems.Add(new ToolStripSeparator());

                if (LayoutModel.StateManager.DriverInstructionsPolicies.Count > 0) {
                    foreach (LayoutPolicyInfo policy in LayoutModel.StateManager.DriverInstructionsPolicies) {
                        LayoutPolicyInfo p = policy;

                        m.DropDownItems.Add(p.Name, null, (object? s, EventArgs e) => selectedWaypoint.DriverInstructions = GenerateUsePolicy(selectedWaypoint, p));
                    }

                    m.DropDownItems.Add(new ToolStripSeparator());
                }
            }

            m.DropDownItems.Add("Edit...", null, MenuItemEditDriverInstructions_Click);
        }

        private void BuildStartConditionMenu(LayoutMenuItem m) {
            var selectedWaypoint = GetSelectedWaypoint();
            m.DropDownItems.Clear();

            if (selectedWaypoint != null) {
                m.DropDownItems.Add("At once", null, (object? s, EventArgs e) => selectedWaypoint.DriverInstructions = null);
                m.DropDownItems.Add(new ToolStripSeparator());

                if (LayoutModel.StateManager.RideStartPolicies.Count > 0) {
                    foreach (LayoutPolicyInfo policy in LayoutModel.StateManager.RideStartPolicies) {
                        LayoutPolicyInfo p = policy;

                        m.DropDownItems.Add(p.Name, null, (object? s, EventArgs e) => selectedWaypoint.StartCondition = GenerateUsePolicy(selectedWaypoint, p));
                    }

                    m.DropDownItems.Add(new ToolStripSeparator());
                }
            }

            m.DropDownItems.Add("Edit...", null, MenuItemEditStartCondition_Click);
        }

        private XmlElement GenerateUsePolicy(WayPointItem waypoint, LayoutPolicyInfo policy) {
            XmlElement runPolicy = waypoint.WayPoint.Element.OwnerDocument.CreateElement(E_RunPolicy);

            runPolicy.SetAttributeValue(A_PolicyID, policy.Id);
            return runPolicy;
        }

        private void MenuItemEditDirectionForward_Click(object? sender, System.EventArgs e) {
            WayPointItem selected = (WayPointItem)listViewWayPoints.SelectedItems[0];

            selected.WayPoint.Direction = LocomotiveOrientation.Forward;
            selected.Update();
            UpdateButtons(null, EventArgs.Empty);
        }

        private void MenuItemEditDirectionBackward_Click(object? sender, System.EventArgs e) {
            WayPointItem selected = (WayPointItem)listViewWayPoints.SelectedItems[0];

            selected.WayPoint.Direction = LocomotiveOrientation.Backward;
            selected.Update();
            UpdateButtons(null, EventArgs.Empty);
        }

        private void MenuItemEditStartCondition_Click(object? sender, System.EventArgs e) {
            WayPointItem selected = (WayPointItem)listViewWayPoints.SelectedItems[0];

            Dialogs.TripPlanWaypointStartCondition startConditionDialog = new(selected.WayPoint) {
                ViewOnly = this.ViewOnly
            };

            new SemiModalDialog(FindForm(), startConditionDialog,
                new SemiModalDialogClosedHandler(OnStartConditionDialogClosed), selected).ShowDialog();
        }

        private void OnStartConditionDialogClosed(Form dialog, object? info) {
            var waypointItem = Ensure.NotNull<WayPointItem>(info);
            if (dialog.DialogResult == DialogResult.OK) {
                waypointItem.Update();
                UpdateButtons(null, EventArgs.Empty);
            }
        }

        private void MenuItemEditDriverInstructions_Click(object? sender, System.EventArgs e) {
            WayPointItem selected = (WayPointItem)listViewWayPoints.SelectedItems[0];
            var d = new Dialogs.TripPlanWayPointDriverInstructions(selected.WayPoint) {
                ViewOnly = this.ViewOnly
            };

            if (d.ShowDialog(this) == DialogResult.OK)
                selected.Update();
        }

        private void MenuItemEditDestination_Click(object? sender, System.EventArgs e) {
            WayPointItem selected = (WayPointItem)listViewWayPoints.SelectedItems[0];

            HideSelections();

            if (ViewOnly) {
                var destinationEditor = new Dialogs.DestinationEditor(selected.WayPoint.Destination, "Destination for " + TripPlanName) {
                    ViewOnly = true
                };
                destinationEditor.ShowDialog(this);
                DisplaySelections();
            }
            else {
                activeDestinationEditor = new Dialogs.DestinationEditor(selected.WayPoint.Destination, "Destination for " + TripPlanName);

                new SemiModalDialog(FindForm(), activeDestinationEditor, new SemiModalDialogClosedHandler(OnDestinationEditorClosed), null).ShowDialog();
            }
        }

        private void OnDestinationEditorClosed(Form dialog, object? info) {
            WayPointItem selected = (WayPointItem)listViewWayPoints.SelectedItems[0];

            selected.Update();
            DisplaySelections();
            UpdateButtons(null, EventArgs.Empty);
        }

        private void ButtonAddWaypoint_Click(object? sender, System.EventArgs e) {
            TripPlanCatalogInfo tripPlanCatalog = LayoutModel.StateManager.TripPlansCatalog;
            var menu = new ContextMenuStrip();

            if (tripPlanCatalog.Destinations.Count > 0) {
                var savedDestinations = new LayoutMenuItem("\"Smart\" Destinations");

                foreach (TripPlanDestinationInfo destination in tripPlanCatalog.Destinations)
                    savedDestinations.DropDownItems.Add(new AddSavedDestinationMenuItem(this, destination));

                menu.Items.Add(savedDestinations);
                menu.Items.Add(new ToolStripSeparator());
            }

            int nItems = menu.Items.Count;

            new BuildComponentsMenu<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases, "*[@SuggestForDestination='true']", new EventHandler(OnAddComponentMenuItemClick)).AddComponentMenuItems(new MenuOrMenuItem(menu));

            if (tripPlanCatalog.Destinations.Count > 0) {
                if (menu.Items.Count > nItems)
                    menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(new LayoutMenuItem("Manage \"Smart\" Destinations...", null, new EventHandler(this.OnManagerCommonDestinations)));
            }

            menu.Show(buttonAddWaypoint.Parent, new Point(buttonAddWaypoint.Left, buttonAddWaypoint.Bottom));
        }

        private void OnManagerCommonDestinations(object? sender, EventArgs e) {
            var dialog = new Dialogs.TripPlanManageCommonDestinations();

            HideSelections();
            dialog.ShowDialog(this);
            DisplaySelections();
        }

        private class AddSavedDestinationMenuItem : LayoutMenuItem {
            private readonly TripPlanEditor tripPlanEditor;
            private readonly TripPlanDestinationInfo destination;

            public AddSavedDestinationMenuItem(TripPlanEditor tripPlanEditor, TripPlanDestinationInfo destination) {
                this.tripPlanEditor = tripPlanEditor;
                this.destination = destination;

                this.Text = destination.Name;
            }

            protected override void OnClick(EventArgs e) {
                tripPlanEditor.AddWayPoint(destination);
            }
        }

        private void OnAddComponentMenuItemClick(object? sender, EventArgs e) {
            var item = Ensure.NotNull<ModelComponentMenuItemBase<LayoutBlockDefinitionComponent>>(sender);

            if (item.Component != null)
                AddWayPoint(item.Component);
        }

        private void ListViewWayPoints_DoubleClick(object? sender, System.EventArgs e) {
            if (listViewWayPoints.SelectedItems.Count > 0 && !ViewOnly) {
                WayPointItem selected = (WayPointItem)listViewWayPoints.SelectedItems[0];

                if (selected.WayPoint.Direction == LocomotiveOrientation.Forward)
                    selected.WayPoint.Direction = LocomotiveOrientation.Backward;
                else
                    selected.WayPoint.Direction = LocomotiveOrientation.Forward;

                selected.Update();
                UpdateButtons(null, EventArgs.Empty);
            }
        }

        #endregion

        private class WayPointItem : ListViewItem {
            private bool shouldReverese = false;
            private bool noPass = false;
            private bool trainTarget = false;

            public WayPointItem(TripPlanWaypointInfo wayPoint) {
                this.WayPoint = wayPoint;

                this.Text = DestinationDescription;
                this.SubItems.Add(wayPoint.Direction.ToString());
                this.SubItems.Add(wayPoint.StartConditionDescription);
                this.SubItems.Add(wayPoint.DriverInstructionsDescription);
            }

            public void Update() {
                this.SubItems[0].Text = DestinationDescription;
                this.SubItems[1].Text = WayPoint.Direction.ToString();
                this.SubItems[2].Text = WayPoint.StartConditionDescription;
                this.SubItems[3].Text = WayPoint.DriverInstructionsDescription;

                for (int i = 0; i < SubItems.Count; i++) {
                    if (noPass) {
                        SubItems[i].BackColor = Color.Red;
                        SubItems[i].ForeColor = Color.Black;
                    }
                    else {
                        SubItems[i].BackColor = SystemColors.Window;
                        SubItems[i].ForeColor = shouldReverese ? Color.Red : SystemColors.WindowText;
                    }
                }

                if (noPass)
                    ImageIndex = 2;
                else if (ShouldReverese)
                    ImageIndex = 1;
                else if (trainTarget)
                    ImageIndex = 3;
                else
                    ImageIndex = 0;
            }

            protected string DestinationDescription => WayPoint.Destination.Count > 1 ? "{ " + WayPoint.Destination.Name + " }" : WayPoint.Destination.Name;

            public TripPlanWaypointInfo WayPoint { get; }

            public bool ShouldReverese {
                get {
                    return shouldReverese;
                }

                set {
                    shouldReverese = value;
                    Update();
                }
            }

            public bool NoPass {
                get {
                    return noPass;
                }

                set {
                    noPass = value;
                    Update();
                }
            }

            public bool TrainTarget {
                get {
                    return trainTarget;
                }

                set {
                    if (value != trainTarget) {
                        trainTarget = value;
                        Update();
                    }
                }
            }

            public XmlElement? StartCondition {
                get {
                    return WayPoint.StartCondition;
                }

                set {
                    WayPoint.StartCondition = value;
                    Update();
                }
            }

            public XmlElement? DriverInstructions {
                get {
                    return WayPoint.DriverInstructions;
                }

                set {
                    WayPoint.DriverInstructions = value;
                    Update();
                }
            }
        }

        private void MenuItemEditStartCondition_Popup(object? sender, EventArgs e) {
            Debug.WriteLine("menuItemEditStartCondition_Popup");
        }
    }
}
