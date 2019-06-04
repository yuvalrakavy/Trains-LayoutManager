using System;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Linq;
using LayoutManager.Model;
using LayoutManager.Components;
using LayoutManager.CommonUI;
using System.Collections.Generic;

#pragma warning disable IDE0051, IDE0060
namespace LayoutManager.Tools.Controls {
    /// <summary>
    /// Summary description for TripPlanEditor.
    /// </summary>
    public class TripPlanEditor : System.Windows.Forms.UserControl, ITripPlanEditorDialog, CommonUI.Controls.IPolicyListCustomizer {
        private const string A_PolicyID = "PolicyID";
        private const string E_RunPolicy = "RunPolicy";
        private MenuItem menuItemGoHumanDriver;
        private ContextMenu contextMenuEdit;
        private MenuItem menuItemEditDestination;
        private MenuItem menuItemEditDirection;
        private MenuItem menuItemEditDirectionForward;
        private MenuItem menuItemEditDirectionBackward;
        private MenuItem menuItemEditStartCondition;
        private MenuItem menuItemEditDriverInstructions;
        private MenuItem menuItemGoAutoDriver;
        private ImageList imageListButttons;
        private CommonUI.CheckBoxWithViewOnly checkBoxTripPlanCircular;
        private GroupBox groupBoxWayPoints;
        private Button buttonWayPointMoveDown;
        private Button buttonWayPointMoveUp;
        private Button buttonAddWaypoint;
        private Button buttonEditWaypoint;
        private Button buttonRemoveWaypoint;
        private ListView listViewWayPoints;
        private ColumnHeader columnHeaderName;
        private ColumnHeader columnHeaderDirection;
        private ColumnHeader columnHeaderStartCondition;
        private TabControl tabControl1;
        private TabPage tabPageTrip;
        private TabPage tabPagePolicies;
        private TabPage tabPageAttributes;
        private LayoutManager.CommonUI.Controls.AttributesEditor attributesEditor;
        private LayoutManager.CommonUI.Controls.PolicyList policyList;
        private ColumnHeader columnHeaderDriverInstructions;
        private ImageList imageListWayPointStatus;
        private IContainer components;

        private void endOfDesignerVariables() { }

        private TripPlanInfo tripPlan = null;
        private TrainStateInfo train = null;
        private bool initialized = false;
        private bool viewOnly = false;
        private bool buildStartConditionMenu = true;
        private bool buildDriverInstructionsMenu = true;
        private string dialogName = null;
        private Dialogs.DestinationEditor activeDestinationEditor = null;
        private readonly ArrayList previewRequests = new ArrayList();
        private IDictionary<Guid, string> displayedBallons = new Dictionary<Guid, string>();

        public event EventHandler WayPointCountChanged = null;
        private DialogEditing changeToViewOnly = null;

        private LayoutSelection tripPlanSelection;
        private LayoutSelection selectedWaypointSelection;

        public TripPlanEditor() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        #region Exposed properties / methods

        public void Initialize() {
            EventManager.AddObjectSubscriptions(this);
        }

        public LayoutBlock LocomotiveBlock { get; set; } = null;

        public LayoutComponentConnectionPoint Front { get; set; } = LayoutComponentConnectionPoint.Empty;

        public TrainStateInfo Train {
            get {
                return train;
            }

            set {
                if (value != null) {
                    train = value;
                    LocomotiveBlock = train.LocomotiveBlock;
                    Front = train.LocomotiveLocation.DisplayFront;
                }
            }
        }

        public bool EnablePreview { get; set; } = true;

        public int TrainTargetWaypoint { get; set; } = -1;

        [Browsable(false)]
        public TripPlanInfo TripPlan {
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

                        clearSelections();

                        tripPlanSelection = tripPlan.Selection;
                        selectedWaypointSelection = new LayoutSelection();

                        displaySelections();

                        attributesEditor.AttributesSource = typeof(TripPlanInfo);
                        attributesEditor.AttributesOwner = new AttributesOwner(tripPlan.Element);

                        policyList.Customizer = this;
                        policyList.Scope = "TripPlan";
                        policyList.Policies = LayoutModel.StateManager.TripPlanPolicies;
                        policyList.Initialize();

                        UpdateButtons(null, null);
                        initialized = true;
                    }
                    else {
                        tripPlan = null;
                        clearSelections();
                        UpdateButtons(null, null);
                    }
                }
            }

            get {
                if (!DesignMode)
                    tripPlan.IsCircular = checkBoxTripPlanCircular.Checked;
                return tripPlan;
            }
        }

        private void displaySelections() {
            if (tripPlanSelection != null)
                tripPlanSelection.Display(new LayoutSelectionLook(Color.Orange));
            if (selectedWaypointSelection != null)
                selectedWaypointSelection.Display(new LayoutSelectionLook(Color.OrangeRed));
        }

        private void hideSelections() {
            if (tripPlanSelection != null)
                tripPlanSelection.Hide();
            if (selectedWaypointSelection != null)
                selectedWaypointSelection.Hide();
        }

        private void clearSelections() {
            if (tripPlanSelection != null) {
                tripPlanSelection.Hide();
                tripPlanSelection.Clear();
            }

            if (selectedWaypointSelection != null) {
                selectedWaypointSelection.Hide();
                selectedWaypointSelection.Clear();
            }
        }

        protected void UpdateButtons(object sender, EventArgs e) {
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

            updatePreviewRequests();
        }

        #region Preview requests handlings

        private void clearPreviewRequests() {
            foreach (RoutePreviewRequest previewRequest in previewRequests)
                LayoutController.PreviewRouteManager.Remove(previewRequest);

            previewRequests.Clear();
        }

        private TripBestRouteResult findBestRoute(TripBestRouteRequest bestRouteRequest) {
            TripBestRouteResult bestRoute = (TripBestRouteResult)EventManager.Event(new LayoutEvent("find-best-route-request", bestRouteRequest));

            if (bestRoute.BestRoute == null) {
                bestRouteRequest.Direction = (bestRouteRequest.Direction == LocomotiveOrientation.Forward) ? LocomotiveOrientation.Backward : LocomotiveOrientation.Forward;

                bestRoute = (TripBestRouteResult)EventManager.Event(new LayoutEvent("find-best-route-request", bestRouteRequest));
                if (bestRoute.BestRoute != null)
                    bestRoute.ShouldReverse = true;
            }

            return bestRoute;
        }

        private bool addPreviewRequest(TripBestRouteResult result, WayPointItem wayPointItem, IDictionary ballons, bool anySelected, bool selected) {
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

                            s = s.Substring(0, i) + "...";
                        }

                        if (!ballons.Contains(blockInfo.Id))
                            ballons.Add(blockInfo.Id, s);
                    }
                }

                return true;
            }
            else {
                wayPointItem.NoPass = true;
                return false;
            }
        }

        private void updateBallons(IDictionary<Guid, string> newBallons) {
            foreach (var d in newBallons) {
                displayedBallons.TryGetValue(d.Key, out string displayedBallonText);

                if (displayedBallonText != d.Value) {
                    var blockDefinition = LayoutModel.Component<LayoutBlockDefinitionComponent>((Guid)d.Key, LayoutModel.ActivePhases);
                    var ballon = new LayoutBlockBallon();

                    Debug.Assert(blockDefinition != null);

                    ballon.FillColor = Color.Yellow;
                    ballon.TextColor = Color.Black;
                    ballon.FontSize = 10f;
                    ballon.Text = d.Value;

                    LayoutBlockBallon.Show(blockDefinition, ballon);
                }

                if (displayedBallonText != null)
                    displayedBallons.Remove(d.Key);
            }

            // At this stage, the only entries in displayedBallons are entries that do not exist in newBallons and
            // therefore should be erased
            foreach (var d in displayedBallons) {
                var blockDefinition = LayoutModel.Component<LayoutBlockDefinitionComponent>((Guid)d.Key, LayoutPhase.All);

                LayoutBlockBallon.Remove(blockDefinition, LayoutBlockBallon.TerminationReason.Hidden);
            }

            displayedBallons = newBallons;
        }

        private void updatePreviewRequests() {
            clearPreviewRequests();

            if (listViewWayPoints.Items.Count > 0 && EnablePreview) {
                int wayPointIndex = (TrainTargetWaypoint < 0) ? 0 : TrainTargetWaypoint;
                WayPointItem wayPointItem = (WayPointItem)listViewWayPoints.Items[wayPointIndex];
                TripPlanWaypointInfo wayPoint = wayPointItem.WayPoint;
                TripBestRouteResult result;
                Guid routeOwner = (train != null) ? Train.Id : Guid.Empty;
                bool addMore;
                int selectedIndex = listViewWayPoints.SelectedItems.Count > 0 ? listViewWayPoints.SelectedIndices[0] : -1;
                var ballons = new Dictionary<Guid, string>();

                if (LocomotiveBlock != null) {
                    TripBestRouteRequest request = new TripBestRouteRequest(routeOwner, wayPoint.Destination, LocomotiveBlock.BlockDefinintion.Track,
                        Front, wayPoint.Direction, wayPoint.TrainStopping);

                    result = findBestRoute(request);
                    addMore = addPreviewRequest(result, wayPointItem, ballons, selectedIndex != -1, selectedIndex == wayPointIndex);
                }
                else
                    throw new ArgumentException("No train/locomotive-block is defined");

                // Now continue with the rest of the trip plan
                for (wayPointIndex++; addMore && wayPointIndex < listViewWayPoints.Items.Count; wayPointIndex++) {
                    wayPointItem = (WayPointItem)listViewWayPoints.Items[wayPointIndex];
                    wayPoint = wayPointItem.WayPoint;

                    TripBestRouteRequest request = new TripBestRouteRequest(routeOwner, wayPoint.Destination, result.BestRoute.DestinationEdge.Track,
                        result.BestRoute.DestinationFront, wayPoint.Direction, wayPoint.TrainStopping);

                    result = findBestRoute(request);
                    addMore = addPreviewRequest(result, wayPointItem, ballons, selectedIndex != -1, selectedIndex == wayPointIndex);
                }

                if (addMore) {
                    // If the trip is circular, add another segment to the first way point
                    if (checkBoxTripPlanCircular.Checked) {
                        wayPointIndex = 0;
                        wayPointItem = (WayPointItem)listViewWayPoints.Items[wayPointIndex];
                        wayPoint = wayPointItem.WayPoint;

                        TripBestRouteRequest request = new TripBestRouteRequest(routeOwner, wayPoint.Destination, result.BestRoute.DestinationEdge.Track,
                            result.BestRoute.DestinationFront, wayPoint.Direction, wayPoint.TrainStopping);

                        result = findBestRoute(request);
                        addPreviewRequest(result, wayPointItem, ballons, selectedIndex != -1, selectedIndex == wayPointIndex);
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

                    item.TrainTarget = (i == TrainTargetWaypoint);
                }

                listViewWayPoints.Invalidate();
                updateBallons(ballons);
            }
        }

        #endregion

        // Implementation of IPolicyListCustomizer

        public bool IsPolicyChecked(LayoutPolicyInfo policy) => tripPlan.Policies.Contains(policy.Id);

        public void SetPolicyChecked(LayoutPolicyInfo policy, bool checkValue) {
            if (checkValue)
                tripPlan.Policies.Add(policy.Id);
            else
                tripPlan.Policies.Remove(policy.Id);
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
                    changeToViewOnly.Undo();
                    changeToViewOnly.ViewOnly = false;
                    changeToViewOnly = null;
                }

                viewOnly = value;
            }
        }

        public string DialogName => dialogName;

        public void SetDialogName(string dialogName) {
            this.dialogName = dialogName;
        }

        public string TripPlanName { get; set; } = null;

        public Form ActiveForm {
            get {
                if (activeDestinationEditor != null)
                    return activeDestinationEditor;
                else return ParentForm.Enabled ? ParentForm : null;
            }
        }

        public int Count => listViewWayPoints.Items.Count;

        public void AddWayPoint(TripPlanWaypointInfo newWaypoint) {
            listViewWayPoints.Items.Add(new WayPointItem(newWaypoint));
            tripPlanSelection.Add(newWaypoint.Destination.Selection);

            UpdateButtons(null, null);

            WayPointCountChanged?.Invoke(this, null);
        }

        public void AddWayPoint(LayoutBlockDefinitionComponent blockInfo) {
            TripPlanWaypointInfo newWaypoint = tripPlan.Add(blockInfo);

            AddWayPoint(newWaypoint);
        }

        public void AddWayPoint(TripPlanDestinationInfo destination) {
            XmlElement addedDestinationElement = (XmlElement)tripPlan.Element.OwnerDocument.ImportNode(destination.Element, true);
            TripPlanDestinationInfo addedDestination = new TripPlanDestinationInfo(addedDestinationElement);
            TripPlanWaypointInfo newWaypoint = tripPlan.Add(addedDestination);

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

        [LayoutEvent("query-edit-trip-plan-dialog")]
        private void queryEditTripPlanDialog(LayoutEvent e) {
            List<ITripPlanEditorDialog> dialogs = (List<ITripPlanEditorDialog>)e.Info;

            if (Enabled && !viewOnly)
                dialogs.Add((ITripPlanEditorDialog)this);
        }

        [LayoutEvent("get-train-active-trip", Order = 100)]
        private void getTrainActiveTrip(LayoutEvent e) {
            TrainStateInfo train = (TrainStateInfo)e.Sender;

            if (e.Info == null && train.Id == this.Train.Id) {
                TripPlanAssignmentInfo tripPlanAssignment = new TripPlanAssignmentInfo(TripPlan, Train);

                e.Info = tripPlanAssignment;
            }
        }

        [LayoutEvent("policy-added-to-policies-collection")]
        [LayoutEvent("policy-removed-from-policies-collection")]
        private void policyCollectionUpdated(LayoutEvent e) {
            LayoutPoliciesCollection policiesCollection = (LayoutPoliciesCollection)e.Sender;

            if (policiesCollection == LayoutModel.StateManager.RideStartPolicies)
                buildStartConditionMenu = true;
            else if (policiesCollection == LayoutModel.StateManager.DriverInstructionsPolicies)
                buildDriverInstructionsMenu = true;
        }

        #endregion

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                EventManager.Subscriptions.RemoveObjectSubscriptions(this);

                if (initialized) {
                    policyList.Dispose();

                    hideSelections();

                    tripPlanSelection.Clear();
                    tripPlanSelection = null;

                    selectedWaypointSelection.Clear();
                    selectedWaypointSelection = null;

                    if (activeDestinationEditor != null)
                        activeDestinationEditor.Close();

                    clearPreviewRequests();
                    updateBallons(new Dictionary<Guid, string>());
                }

                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TripPlanEditor));
            this.menuItemGoHumanDriver = new MenuItem();
            this.contextMenuEdit = new ContextMenu();
            this.menuItemEditDestination = new MenuItem();
            this.menuItemEditDirection = new MenuItem();
            this.menuItemEditDirectionForward = new MenuItem();
            this.menuItemEditDirectionBackward = new MenuItem();
            this.menuItemEditStartCondition = new MenuItem();
            this.menuItemEditDriverInstructions = new MenuItem();
            this.menuItemGoAutoDriver = new MenuItem();
            this.imageListButttons = new ImageList(this.components);
            this.checkBoxTripPlanCircular = new LayoutManager.CommonUI.CheckBoxWithViewOnly();
            this.groupBoxWayPoints = new GroupBox();
            this.buttonWayPointMoveDown = new Button();
            this.buttonWayPointMoveUp = new Button();
            this.buttonAddWaypoint = new Button();
            this.buttonEditWaypoint = new Button();
            this.buttonRemoveWaypoint = new Button();
            this.listViewWayPoints = new ListView();
            this.columnHeaderName = new ColumnHeader();
            this.columnHeaderDirection = new ColumnHeader();
            this.columnHeaderStartCondition = new ColumnHeader();
            this.columnHeaderDriverInstructions = new ColumnHeader();
            this.imageListWayPointStatus = new ImageList(this.components);
            this.tabControl1 = new TabControl();
            this.tabPageTrip = new TabPage();
            this.tabPagePolicies = new TabPage();
            this.policyList = new LayoutManager.CommonUI.Controls.PolicyList();
            this.tabPageAttributes = new TabPage();
            this.attributesEditor = new LayoutManager.CommonUI.Controls.AttributesEditor();
            this.groupBoxWayPoints.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageTrip.SuspendLayout();
            this.tabPagePolicies.SuspendLayout();
            this.tabPageAttributes.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuItemGoHumanDriver
            // 
            this.menuItemGoHumanDriver.Index = -1;
            this.menuItemGoHumanDriver.Text = "&Human driver";
            // 
            // contextMenuEdit
            // 
            this.contextMenuEdit.MenuItems.AddRange(new MenuItem[] {
            this.menuItemEditDestination,
            this.menuItemEditDirection,
            this.menuItemEditStartCondition,
            this.menuItemEditDriverInstructions});
            // 
            // menuItemEditDestination
            // 
            this.menuItemEditDestination.Index = 0;
            this.menuItemEditDestination.Text = "&Destination...";
            this.menuItemEditDestination.Click += this.menuItemEditDestination_Click;
            // 
            // menuItemEditDirection
            // 
            this.menuItemEditDirection.Index = 1;
            this.menuItemEditDirection.MenuItems.AddRange(new MenuItem[] {
            this.menuItemEditDirectionForward,
            this.menuItemEditDirectionBackward});
            this.menuItemEditDirection.Text = "D&irection";
            // 
            // menuItemEditDirectionForward
            // 
            this.menuItemEditDirectionForward.Index = 0;
            this.menuItemEditDirectionForward.Text = "&Forward";
            this.menuItemEditDirectionForward.Click += this.menuItemEditDirectionForward_Click;
            // 
            // menuItemEditDirectionBackward
            // 
            this.menuItemEditDirectionBackward.Index = 1;
            this.menuItemEditDirectionBackward.Text = "&Backward";
            this.menuItemEditDirectionBackward.Click += this.menuItemEditDirectionBackward_Click;
            // 
            // menuItemEditStartCondition
            // 
            this.menuItemEditStartCondition.Index = 2;
            this.menuItemEditStartCondition.Text = "Start &Condition";
            this.menuItemEditStartCondition.Click += this.menuItemEditStartCondition_Click;
            this.menuItemEditStartCondition.Popup += this.menuItemEditStartCondition_Popup;
            // 
            // menuItemEditDriverInstructions
            // 
            this.menuItemEditDriverInstructions.Index = 3;
            this.menuItemEditDriverInstructions.Text = "Driver &Instructions";
            this.menuItemEditDriverInstructions.Click += this.menuItemEditDriverInstructions_Click;
            // 
            // menuItemGoAutoDriver
            // 
            this.menuItemGoAutoDriver.Index = -1;
            this.menuItemGoAutoDriver.Text = "&Auto driver";
            // 
            // imageListButttons
            // 
            this.imageListButttons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListButttons.ImageStream")));
            this.imageListButttons.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListButttons.Images.SetKeyName(0, "");
            this.imageListButttons.Images.SetKeyName(1, "");
            // 
            // checkBoxTripPlanCircular
            // 
            this.checkBoxTripPlanCircular.Location = new System.Drawing.Point(8, 8);
            this.checkBoxTripPlanCircular.Name = "checkBoxTripPlanCircular";
            this.checkBoxTripPlanCircular.Size = new System.Drawing.Size(128, 16);
            this.checkBoxTripPlanCircular.TabIndex = 2;
            this.checkBoxTripPlanCircular.Text = "Trip plan is circular";
            this.checkBoxTripPlanCircular.ViewOnly = false;
            this.checkBoxTripPlanCircular.Click += this.UpdateButtons;
            // 
            // groupBoxWayPoints
            // 
            this.groupBoxWayPoints.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxWayPoints.Controls.Add(this.buttonWayPointMoveDown);
            this.groupBoxWayPoints.Controls.Add(this.buttonWayPointMoveUp);
            this.groupBoxWayPoints.Controls.Add(this.buttonAddWaypoint);
            this.groupBoxWayPoints.Controls.Add(this.buttonEditWaypoint);
            this.groupBoxWayPoints.Controls.Add(this.buttonRemoveWaypoint);
            this.groupBoxWayPoints.Controls.Add(this.listViewWayPoints);
            this.groupBoxWayPoints.Location = new System.Drawing.Point(8, 32);
            this.groupBoxWayPoints.Name = "groupBoxWayPoints";
            this.groupBoxWayPoints.Size = new System.Drawing.Size(312, 158);
            this.groupBoxWayPoints.TabIndex = 5;
            this.groupBoxWayPoints.TabStop = false;
            this.groupBoxWayPoints.Text = "Way points:";
            // 
            // buttonWayPointMoveDown
            // 
            this.buttonWayPointMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonWayPointMoveDown.ImageIndex = 0;
            this.buttonWayPointMoveDown.ImageList = this.imageListButttons;
            this.buttonWayPointMoveDown.Location = new System.Drawing.Point(251, 133);
            this.buttonWayPointMoveDown.Name = "buttonWayPointMoveDown";
            this.buttonWayPointMoveDown.Size = new System.Drawing.Size(24, 20);
            this.buttonWayPointMoveDown.TabIndex = 4;
            this.buttonWayPointMoveDown.Click += this.buttonWayPointMoveDown_Click;
            // 
            // buttonWayPointMoveUp
            // 
            this.buttonWayPointMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonWayPointMoveUp.ImageIndex = 1;
            this.buttonWayPointMoveUp.ImageList = this.imageListButttons;
            this.buttonWayPointMoveUp.Location = new System.Drawing.Point(280, 133);
            this.buttonWayPointMoveUp.Name = "buttonWayPointMoveUp";
            this.buttonWayPointMoveUp.Size = new System.Drawing.Size(24, 20);
            this.buttonWayPointMoveUp.TabIndex = 5;
            this.buttonWayPointMoveUp.Click += this.buttonWayPointMoveUp_Click;
            // 
            // buttonAddWaypoint
            // 
            this.buttonAddWaypoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAddWaypoint.Location = new System.Drawing.Point(8, 133);
            this.buttonAddWaypoint.Name = "buttonAddWaypoint";
            this.buttonAddWaypoint.Size = new System.Drawing.Size(56, 20);
            this.buttonAddWaypoint.TabIndex = 1;
            this.buttonAddWaypoint.Text = "&Add";
            this.buttonAddWaypoint.Click += this.buttonAddWaypoint_Click;
            // 
            // buttonEditWaypoint
            // 
            this.buttonEditWaypoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonEditWaypoint.Location = new System.Drawing.Point(72, 133);
            this.buttonEditWaypoint.Name = "buttonEditWaypoint";
            this.buttonEditWaypoint.Size = new System.Drawing.Size(56, 20);
            this.buttonEditWaypoint.TabIndex = 2;
            this.buttonEditWaypoint.Text = "&Edit";
            this.buttonEditWaypoint.Click += this.buttonEditWaypoint_Click;
            // 
            // buttonRemoveWaypoint
            // 
            this.buttonRemoveWaypoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRemoveWaypoint.Location = new System.Drawing.Point(136, 133);
            this.buttonRemoveWaypoint.Name = "buttonRemoveWaypoint";
            this.buttonRemoveWaypoint.Size = new System.Drawing.Size(56, 20);
            this.buttonRemoveWaypoint.TabIndex = 3;
            this.buttonRemoveWaypoint.Text = "&Remove";
            this.buttonRemoveWaypoint.Click += this.buttonRemoveWaypoint_Click;
            // 
            // listViewWayPoints
            // 
            this.listViewWayPoints.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewWayPoints.Columns.AddRange(new ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderDirection,
            this.columnHeaderStartCondition,
            this.columnHeaderDriverInstructions});
            this.listViewWayPoints.FullRowSelect = true;
            this.listViewWayPoints.GridLines = true;
            this.listViewWayPoints.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewWayPoints.HideSelection = false;
            this.listViewWayPoints.Location = new System.Drawing.Point(8, 24);
            this.listViewWayPoints.MultiSelect = false;
            this.listViewWayPoints.Name = "listViewWayPoints";
            this.listViewWayPoints.Size = new System.Drawing.Size(296, 104);
            this.listViewWayPoints.SmallImageList = this.imageListWayPointStatus;
            this.listViewWayPoints.TabIndex = 0;
            this.listViewWayPoints.UseCompatibleStateImageBehavior = false;
            this.listViewWayPoints.View = System.Windows.Forms.View.Details;
            this.listViewWayPoints.DoubleClick += this.listViewWayPoints_DoubleClick;
            this.listViewWayPoints.SelectedIndexChanged += this.listViewWayPoints_SelectedIndexChanged;
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Destination";
            this.columnHeaderName.Width = 100;
            // 
            // columnHeaderDirection
            // 
            this.columnHeaderDirection.Text = "Direction";
            // 
            // columnHeaderStartCondition
            // 
            this.columnHeaderStartCondition.Text = "Starting condition";
            this.columnHeaderStartCondition.Width = 120;
            // 
            // columnHeaderDriverInstructions
            // 
            this.columnHeaderDriverInstructions.Text = "Driver Instructions";
            this.columnHeaderDriverInstructions.Width = 120;
            // 
            // imageListWayPointStatus
            // 
            this.imageListWayPointStatus.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListWayPointStatus.ImageStream")));
            this.imageListWayPointStatus.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListWayPointStatus.Images.SetKeyName(0, "");
            this.imageListWayPointStatus.Images.SetKeyName(1, "");
            this.imageListWayPointStatus.Images.SetKeyName(2, "");
            this.imageListWayPointStatus.Images.SetKeyName(3, "");
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageTrip);
            this.tabControl1.Controls.Add(this.tabPagePolicies);
            this.tabControl1.Controls.Add(this.tabPageAttributes);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(336, 224);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPageTrip
            // 
            this.tabPageTrip.Controls.Add(this.checkBoxTripPlanCircular);
            this.tabPageTrip.Controls.Add(this.groupBoxWayPoints);
            this.tabPageTrip.Location = new System.Drawing.Point(4, 22);
            this.tabPageTrip.Name = "tabPageTrip";
            this.tabPageTrip.Size = new System.Drawing.Size(328, 198);
            this.tabPageTrip.TabIndex = 0;
            this.tabPageTrip.Text = "Route";
            // 
            // tabPagePolicies
            // 
            this.tabPagePolicies.Controls.Add(this.policyList);
            this.tabPagePolicies.Location = new System.Drawing.Point(4, 22);
            this.tabPagePolicies.Name = "tabPagePolicies";
            this.tabPagePolicies.Size = new System.Drawing.Size(328, 198);
            this.tabPagePolicies.TabIndex = 1;
            this.tabPagePolicies.Text = "Actions";
            // 
            // policyList
            // 
            this.policyList.Customizer = this.policyList;
            this.policyList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.policyList.Location = new System.Drawing.Point(0, 0);
            this.policyList.Name = "policyList";
            this.policyList.Policies = null;
            this.policyList.Scope = "TripPlan";
            this.policyList.ShowIfRunning = false;
            this.policyList.ShowPolicyDefinition = false;
            this.policyList.Size = new System.Drawing.Size(328, 198);
            this.policyList.TabIndex = 0;
            this.policyList.ViewOnly = false;
            // 
            // tabPageAttributes
            // 
            this.tabPageAttributes.Controls.Add(this.attributesEditor);
            this.tabPageAttributes.Location = new System.Drawing.Point(4, 22);
            this.tabPageAttributes.Name = "tabPageAttributes";
            this.tabPageAttributes.Size = new System.Drawing.Size(328, 198);
            this.tabPageAttributes.TabIndex = 2;
            this.tabPageAttributes.Text = "Attributes";
            // 
            // attributesEditor
            // 
            this.attributesEditor.AttributesOwner = null;
            this.attributesEditor.AttributesSource = null;
            this.attributesEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attributesEditor.Location = new System.Drawing.Point(0, 0);
            this.attributesEditor.Name = "attributesEditor";
            this.attributesEditor.Size = new System.Drawing.Size(328, 198);
            this.attributesEditor.TabIndex = 0;
            this.attributesEditor.ViewOnly = false;
            // 
            // TripPlanEditor
            // 
            this.Controls.Add(this.tabControl1);
            this.Name = "TripPlanEditor";
            this.Size = new System.Drawing.Size(336, 224);
            this.groupBoxWayPoints.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageTrip.ResumeLayout(false);
            this.tabPagePolicies.ResumeLayout(false);
            this.tabPageAttributes.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        #region Event Handlers

        private void listViewWayPoints_SelectedIndexChanged(object sender, System.EventArgs e) {
            selectedWaypointSelection.Clear();

            foreach (WayPointItem wayPointItem in listViewWayPoints.SelectedItems)
                selectedWaypointSelection.Add(wayPointItem.WayPoint.Destination.Selection);

            if (selectedWaypointSelection.Count > 0)
                EventManager.Event(new LayoutEvent("ensure-component-visible", selectedWaypointSelection.Components.First(), false).SetFrameWindow(LayoutController.ActiveFrameWindow));

            UpdateButtons(sender, e);
        }

        private void buttonRemoveWaypoint_Click(object sender, System.EventArgs e) {
            if (listViewWayPoints.SelectedItems.Count > 0) {
                WayPointItem selected = (WayPointItem)listViewWayPoints.SelectedItems[0];

                listViewWayPoints.Items.Remove(selected);
                tripPlanSelection.Remove(selected.WayPoint.Destination.Selection);
                tripPlan.Remove(selected.WayPoint);
                UpdateButtons(null, null);

                WayPointCountChanged?.Invoke(this, null);
            }
        }

        private void buttonWayPointMoveDown_Click(object sender, System.EventArgs e) {
            if (listViewWayPoints.SelectedItems.Count > 0) {
                int selectedIndex = listViewWayPoints.SelectedIndices[0];

                if (selectedIndex < listViewWayPoints.Items.Count - 1) {
                    WayPointItem selected = (WayPointItem)listViewWayPoints.SelectedItems[0];
                    TripPlanWaypointInfo selectedWaypoint = selected.WayPoint;
                    XmlNode nextElement = selectedWaypoint.Element.NextSibling;

                    selectedWaypoint.Element.ParentNode.RemoveChild(selectedWaypoint.Element);
                    nextElement.ParentNode.InsertAfter(selectedWaypoint.Element, nextElement);

                    listViewWayPoints.Items.Remove(selected);
                    listViewWayPoints.Items.Insert(selectedIndex + 1, selected);
                    UpdateButtons(null, null);
                }
            }
        }

        private void buttonWayPointMoveUp_Click(object sender, System.EventArgs e) {
            if (listViewWayPoints.SelectedItems.Count > 0) {
                int selectedIndex = listViewWayPoints.SelectedIndices[0];

                if (selectedIndex > 0) {
                    WayPointItem selected = (WayPointItem)listViewWayPoints.SelectedItems[0];
                    TripPlanWaypointInfo selectedWaypoint = selected.WayPoint;
                    XmlNode previousElement = selectedWaypoint.Element.PreviousSibling;

                    selectedWaypoint.Element.ParentNode.RemoveChild(selectedWaypoint.Element);
                    previousElement.ParentNode.InsertBefore(selectedWaypoint.Element, previousElement);

                    listViewWayPoints.Items.Remove(selected);
                    listViewWayPoints.Items.Insert(selectedIndex - 1, selected);
                    UpdateButtons(null, null);
                }
            }
        }

        private WayPointItem GetSelectedWaypoint() {
            return listViewWayPoints.SelectedItems.Count == 0 ? null : (WayPointItem)listViewWayPoints.SelectedItems[0];
        }

        private void buttonEditWaypoint_Click(object sender, System.EventArgs e) {
            WayPointItem selected = GetSelectedWaypoint();
            bool canSetDriverInstructions;

            if (selected.Index == 0)
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

        private void BuildDriverInstructionsMenu(MenuItem m) {
            m.MenuItems.Clear();

            m.MenuItems.Add("No instructions", (object s, EventArgs e) => GetSelectedWaypoint().DriverInstructions = null);

            m.MenuItems.Add("-");

            if (LayoutModel.StateManager.DriverInstructionsPolicies.Count > 0) {
                foreach (LayoutPolicyInfo policy in LayoutModel.StateManager.DriverInstructionsPolicies) {
                    LayoutPolicyInfo p = policy;

                    m.MenuItems.Add(p.Name, (object s, EventArgs e) => GetSelectedWaypoint().DriverInstructions = GenerateUsePolicy(GetSelectedWaypoint(), p));
                }

                m.MenuItems.Add("-");
            }

            m.MenuItems.Add("Edit...", menuItemEditDriverInstructions_Click);
        }

        private void BuildStartConditionMenu(MenuItem m) {
            m.MenuItems.Clear();

            m.MenuItems.Add("At once", (object s, EventArgs e) => GetSelectedWaypoint().DriverInstructions = null);

            m.MenuItems.Add("-");

            if (LayoutModel.StateManager.RideStartPolicies.Count > 0) {
                foreach (LayoutPolicyInfo policy in LayoutModel.StateManager.RideStartPolicies) {
                    LayoutPolicyInfo p = policy;

                    m.MenuItems.Add(p.Name, (object s, EventArgs e) => GetSelectedWaypoint().StartCondition = GenerateUsePolicy(GetSelectedWaypoint(), p));
                }

                m.MenuItems.Add("-");
            }

            m.MenuItems.Add("Edit...", menuItemEditStartCondition_Click);
        }

        private XmlElement GenerateUsePolicy(WayPointItem waypoint, LayoutPolicyInfo policy) {
            XmlElement runPolicy = waypoint.WayPoint.Element.OwnerDocument.CreateElement(E_RunPolicy);

            runPolicy.SetAttribute(A_PolicyID, policy.Id);
            return runPolicy;
        }

        private void menuItemEditDirectionForward_Click(object sender, System.EventArgs e) {
            WayPointItem selected = (WayPointItem)listViewWayPoints.SelectedItems[0];

            selected.WayPoint.Direction = LocomotiveOrientation.Forward;
            selected.Update();
            UpdateButtons(null, null);
        }

        private void menuItemEditDirectionBackward_Click(object sender, System.EventArgs e) {
            WayPointItem selected = (WayPointItem)listViewWayPoints.SelectedItems[0];

            selected.WayPoint.Direction = LocomotiveOrientation.Backward;
            selected.Update();
            UpdateButtons(null, null);
        }

        private void menuItemEditStartCondition_Click(object sender, System.EventArgs e) {
            WayPointItem selected = (WayPointItem)listViewWayPoints.SelectedItems[0];

            Dialogs.TripPlanWaypointStartCondition startConditionDialog = new Dialogs.TripPlanWaypointStartCondition(selected.WayPoint) {
                ViewOnly = this.ViewOnly
            };

            new SemiModalDialog(FindForm(), startConditionDialog,
                new SemiModalDialogClosedHandler(onStartConditionDialogClosed), selected).ShowDialog();
        }

        private void onStartConditionDialogClosed(Form dialog, object info) {
            if (dialog.DialogResult == DialogResult.OK) {
                ((WayPointItem)info).Update();
                UpdateButtons(null, null);
            }
        }

        private void menuItemEditDriverInstructions_Click(object sender, System.EventArgs e) {
            WayPointItem selected = (WayPointItem)listViewWayPoints.SelectedItems[0];
            Dialogs.TripPlanWayPointDriverInstructions d = new Dialogs.TripPlanWayPointDriverInstructions(selected.WayPoint) {
                ViewOnly = this.ViewOnly
            };

            if (d.ShowDialog(this) == DialogResult.OK)
                selected.Update();
        }

        private void menuItemEditDestination_Click(object sender, System.EventArgs e) {
            WayPointItem selected = (WayPointItem)listViewWayPoints.SelectedItems[0];

            hideSelections();

            if (ViewOnly) {
                Dialogs.DestinationEditor destinationEditor = new Dialogs.DestinationEditor(selected.WayPoint.Destination, "Destination for " + TripPlanName) {
                    ViewOnly = true
                };
                destinationEditor.ShowDialog(this);
                displaySelections();
            }
            else {
                activeDestinationEditor = new Dialogs.DestinationEditor(selected.WayPoint.Destination, "Destination for " + TripPlanName);

                new SemiModalDialog(FindForm(), activeDestinationEditor, new SemiModalDialogClosedHandler(onDestinationEditorClosed), null).ShowDialog();
            }
        }

        private void onDestinationEditorClosed(Form dialog, object info) {
            WayPointItem selected = (WayPointItem)listViewWayPoints.SelectedItems[0];

            selected.Update();
            displaySelections();
            UpdateButtons(null, null);
        }

        private void buttonAddWaypoint_Click(object sender, System.EventArgs e) {
            TripPlanCatalogInfo tripPlanCatalog = LayoutModel.StateManager.TripPlansCatalog;
            ContextMenu menu = new ContextMenu();

            if (tripPlanCatalog.Destinations.Count > 0) {
                MenuItem savedDestinations = new MenuItem("\"Smart\" Destinations");

                foreach (TripPlanDestinationInfo destination in tripPlanCatalog.Destinations)
                    savedDestinations.MenuItems.Add(new AddSavedDestinationMenuItem(this, destination));

                menu.MenuItems.Add(savedDestinations);
                menu.MenuItems.Add("-");
            }

            int nItems = menu.MenuItems.Count;

            new BuildComponentsMenu<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases, "*[@SuggestForDestination='true']", new EventHandler(onAddComponentMenuItemClick)).AddComponentMenuItems(menu);

            if (tripPlanCatalog.Destinations.Count > 0) {
                if (menu.MenuItems.Count > nItems)
                    menu.MenuItems.Add("-");
                menu.MenuItems.Add(new MenuItem("Manage \"Smart\" Destinations...", new EventHandler(this.onManagerCommonDestinations)));
            }

            menu.Show(buttonAddWaypoint.Parent, new Point(buttonAddWaypoint.Left, buttonAddWaypoint.Bottom));
        }

        private void onManagerCommonDestinations(object sender, EventArgs e) {
            Dialogs.TripPlanManageCommonDestinations dialog = new Dialogs.TripPlanManageCommonDestinations();

            hideSelections();
            dialog.ShowDialog(this);
            displaySelections();
        }

        private class AddSavedDestinationMenuItem : MenuItem {
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

        private void onAddComponentMenuItemClick(object sender, EventArgs e) {
            ModelComponentMenuItemBase<LayoutBlockDefinitionComponent> item = (ModelComponentMenuItemBase<LayoutBlockDefinitionComponent>)sender;

            AddWayPoint(item.Component);
        }

        private void listViewWayPoints_DoubleClick(object sender, System.EventArgs e) {
            if (listViewWayPoints.SelectedItems.Count > 0 && !ViewOnly) {
                WayPointItem selected = (WayPointItem)listViewWayPoints.SelectedItems[0];

                if (selected.WayPoint.Direction == LocomotiveOrientation.Forward)
                    selected.WayPoint.Direction = LocomotiveOrientation.Backward;
                else
                    selected.WayPoint.Direction = LocomotiveOrientation.Forward;

                selected.Update();
                UpdateButtons(null, null);
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

            protected string DestinationDescription {
                get {
                    return WayPoint.Destination.Count > 1 ? "{ " + WayPoint.Destination.Name + " }" : WayPoint.Destination.Name;
                }
            }

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

            public XmlElement StartCondition {
                get {
                    return WayPoint.StartCondition;
                }

                set {
                    WayPoint.StartCondition = value;
                    Update();
                }
            }

            public XmlElement DriverInstructions {
                get {
                    return WayPoint.DriverInstructions;
                }

                set {
                    WayPoint.DriverInstructions = value;
                    Update();
                }
            }
        }

        private void menuItemEditStartCondition_Popup(object sender, EventArgs e) {
            Debug.WriteLine("menuItemEditStartCondition_Popup");
        }
    }
}
