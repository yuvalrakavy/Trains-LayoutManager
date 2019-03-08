using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;
using LayoutManager.Components;
using LayoutManager.CommonUI;
using LayoutManager.CommonUI.Controls;

namespace LayoutManager.Tools {

    [LayoutModule("EventScript Tools", UserControl = false)]
    class EventScriptTools : LayoutModuleBase {

        #region Symbol name to object type map

        [LayoutEvent("add-context-symbols-and-types")]
        private void addContextSymbolsAndTypes(LayoutEvent e) {
            IDictionary symbolToTypeMap = (IDictionary)e.Info;

            symbolToTypeMap.Add("Train", typeof(TrainStateInfo));
            symbolToTypeMap.Add("TrainLocations", typeof(TrainLocationInfo));
            symbolToTypeMap.Add("TrainLocomotiveLocation", typeof(TrainLocationInfo));
            symbolToTypeMap.Add("TrainLocomotives", typeof(TrainLocomotiveInfo));

            symbolToTypeMap.Add("Trip", typeof(TripPlanAssignmentInfo));
            symbolToTypeMap.Add("Driver", typeof(TrainDriverInfo));
            symbolToTypeMap.Add("FinalDestination", typeof(TripPlanWaypointInfo));
            symbolToTypeMap.Add("Destination", typeof(TripPlanWaypointInfo));

            symbolToTypeMap.Add("TripPlan", typeof(TripPlanInfo));
            symbolToTypeMap.Add("TripPlanWayPoints", typeof(TripPlanWaypointInfo));
            symbolToTypeMap.Add("CurrentWayPoint", typeof(TripPlanWaypointInfo));

            symbolToTypeMap.Add("Block", typeof(LayoutBlock));
            symbolToTypeMap.Add("BlockInfo", typeof(LayoutBlockDefinitionComponent));

            symbolToTypeMap.Add("PreviousBlock", typeof(LayoutBlock));
            symbolToTypeMap.Add("PreviousBlockInfo", typeof(LayoutBlockDefinitionComponent));
        }

        /// <summary>
        /// Several model components have property called info which return a XML wrapper object
        /// that allow access to their XML values. This is hidden from the user, and the user see
        /// all the object properties whether they belong to the object derived from ModelComponent
        /// or from LayoutObject (which is returned by the component Info property). This handler
        /// returns for a given ModelComponent based type, the type that contains its info
        /// </summary>
        [LayoutEvent("get-context-symbol-info-type")]
        private void getContextSymbolInfoType(LayoutEvent e) {
            Type symbolType = (Type)e.Sender;

            if (symbolType == typeof(LayoutBlockDefinitionComponent) || symbolType.IsSubclassOf(typeof(LayoutBlockDefinitionComponent)))
                e.Info = typeof(LayoutBlockDefinitionComponentInfo);
            else if (symbolType == typeof(LayoutSignalComponent) || symbolType.IsSubclassOf(typeof(LayoutSignalComponent)))
                e.Info = typeof(LayoutSignalComponentInfo);
        }

        #endregion

        #region Script operator to operator name mapping

        [LayoutEvent("get-event-script-operator-name", IfSender = "IfString")]
        private void getIfStringOperatorName(LayoutEvent e) {
            XmlElement ifStringElement = (XmlElement)e.Sender;
            string compareOperator = ifStringElement.GetAttribute("Operation");

            switch (compareOperator) {
                case "Equal": e.Info = "="; break;
                case "NotEqual": e.Info = "<>"; break;
                case "Match": e.Info = "Match"; break;
                default: e.Info = "?"; break;
            }
        }

        [LayoutEvent("get-event-script-operator-name", IfSender = "IfNumber")]
        private void getIfNumberOperatorName(LayoutEvent e) {
            XmlElement ifNumberElement = (XmlElement)e.Sender;
            string compareOperator = ifNumberElement.GetAttribute("Operation");

            switch (compareOperator) {
                case "eq": e.Info = "="; break;
                case "ne": e.Info = "<>"; break;
                case "gt": e.Info = ">"; break;
                case "ge": e.Info = ">="; break;
                case "le": e.Info = "=<"; break;
                case "lt": e.Info = "<"; break;
                default: e.Info = "?"; break;
            }
        }

        [LayoutEvent("get-event-script-operator-name", IfSender = "IfBoolean")]
        private void getIfBooleanOperatorName(LayoutEvent e) {
            XmlElement ifBooleanElement = (XmlElement)e.Sender;
            string compareOperator = ifBooleanElement.GetAttribute("Operation");

            switch (compareOperator) {
                case "Equal": e.Info = "="; break;
                case "NotEqual": e.Info = "<>"; break;
                default: e.Info = "?"; break;
            }
        }

        #endregion

        #region Event script error handler

        [LayoutEvent("event-script-error")]
        private void eventScriptError(LayoutEvent e) {
            LayoutEventScriptErrorInfo errorInfo = (LayoutEventScriptErrorInfo)e.Info;

            // TODO: Invoke event script debugger pointing exactly on the errornous node.. for now just complain

            Error("Error in execution of event script '" + errorInfo.Script.Name + "' - " + errorInfo.Exception.Message ?? "Unexpected error");
        }

        #endregion

        #region Context builders

        class TrainToTripResolver : ILayoutScriptContextResolver {
            readonly TrainStateInfo train;

            public TrainToTripResolver(TrainStateInfo train) {
                this.train = train;
            }

            public object Resolve(LayoutScriptContext context, string symbolName) {
                TripPlanAssignmentInfo trip = (TripPlanAssignmentInfo)EventManager.Event(new LayoutEvent("get-train-active-trip", train));

                if (trip == null)
                    return null;

                EventManager.Event(new LayoutEvent("set-script-context", trip, context, null));
                return context[symbolName];
            }
        }

        [LayoutEvent("set-script-context", SenderType = typeof(TrainStateInfo))]
        private void setContextTrain(LayoutEvent e) {
            TrainStateInfo train = (TrainStateInfo)e.Sender;
            LayoutScriptContext context = (LayoutScriptContext)e.Info;

            if (!context.Contains("Train", train)) {
                context["Train"] = train;
                context["TrainLocations"] = train.Locations;
                context["TrainLocomotiveLocation"] = train.LocomotiveLocation;
                context["TrainLocomotives"] = train.Locomotives;

                TrainToTripResolver trainToTripResolver = new TrainToTripResolver(train);

                context["Trip"] = trainToTripResolver;
                context["Driver"] = trainToTripResolver;
                context["FinalDestination"] = trainToTripResolver;
                context["CurrentWayPoint"] = trainToTripResolver;
                context["Destinaion"] = trainToTripResolver;
                context["TripPlan"] = trainToTripResolver;
                context["TripPlanWayPoints"] = trainToTripResolver;
            }
        }

        [LayoutEvent("set-script-context", SenderType = typeof(TripPlanAssignmentInfo))]
        private void setContextTripPlanAssignment(LayoutEvent e) {
            TripPlanAssignmentInfo trip = (TripPlanAssignmentInfo)e.Sender;
            LayoutScriptContext context = (LayoutScriptContext)e.Info;

            if (!context.Contains("Trip", trip)) {
                context["Trip"] = trip;
                context["Driver"] = trip.Train.Driver;
                if (trip.TripPlan.Waypoints.Count > 0)
                    context["FinalDestination"] = trip.TripPlan.Waypoints[trip.TripPlan.Waypoints.Count - 1];
                else
                    context.Remove("FinalDestination");

                if (trip.CurrentWaypoint != null) {
                    context["CurrentWayPoint"] = trip.CurrentWaypoint;
                    context["Destinaion"] = trip.CurrentWaypoint.Destination;
                }
                else {      // Ensure that resolvers (placed by train) is removed to avoid infinite resolving loop
                    context.Remove("CurrentWayPoint");
                    context.Remove("Destination");
                }


                EventManager.Event(new LayoutEvent("set-script-context", trip.TripPlan, context, null));

                if (!context.Contains("Train", trip.Train))
                    EventManager.Event(new LayoutEvent("set-script-context", trip.Train, context, null));
            }
        }

        [LayoutEvent("set-script-context", SenderType = typeof(TripPlanInfo))]
        private void setContextTripPlan(LayoutEvent e) {
            TripPlanInfo tripPlan = (TripPlanInfo)e.Sender;
            LayoutScriptContext context = (LayoutScriptContext)e.Info;

            if (!context.Contains("TripPlan", tripPlan)) {
                context["TripPlan"] = tripPlan;
                context["TripPlanWayPoints"] = tripPlan.Waypoints;
            }
        }

        [LayoutEvent("set-script-context", SenderType = typeof(LayoutBlock))]
        private void setContextBlock(LayoutEvent e) {
            LayoutBlock block = (LayoutBlock)e.Sender;
            LayoutScriptContext context = (LayoutScriptContext)e.Info;

            if (!context.Contains("Block", block)) {
                context["Block"] = block;

                if (block.BlockDefinintion != null)
                    EventManager.Event(new LayoutEvent("set-script-context", block.BlockDefinintion, context, null));
            }
        }

        [LayoutEvent("set-script-context", SenderType = typeof(LayoutBlockDefinitionComponent))]
        private void setContextBlockInfo(LayoutEvent e) {
            LayoutBlockDefinitionComponent blockInfo = (LayoutBlockDefinitionComponent)e.Sender;
            LayoutScriptContext context = (LayoutScriptContext)e.Info;

            if (!context.Contains("BlockInfo", blockInfo)) {
                context["BlockInfo"] = blockInfo;
            }
        }

        #endregion

        #region Wait For Event Reset handlers

        [LayoutEvent("event-script-wait-event-reset", IfSender = "WaitForEvent[@Name='train-in-block']")]
        [LayoutEventDef("train-in-block", Role = LayoutEventRole.Notification, SenderType = typeof(TrainStateInfo), InfoType = typeof(LayoutBlock))]
        private void resetTrainInBlock(LayoutEvent e) {
            LayoutEventSubscription subscription = (LayoutEventSubscription)e.Info;

            foreach (XmlElement trainElement in LayoutModel.StateManager.Trains.Element) {
                TrainStateInfo train = new TrainStateInfo(trainElement);

                foreach (TrainLocationInfo trainLocation in train.Locations)
                    subscription.EventHandler(new LayoutEvent("train-in-block", train, trainLocation.Block, null));
            }
        }

        [LayoutEvent("train-enter-block")]
        private void trainEnterBlock(LayoutEvent e) {
            EventManager.Event(new LayoutEvent("train-in-block", e.Sender, e.Info, null));
        }

        #endregion

        #region Add layout wide policies to tools menu

        [LayoutEvent("tools-menu-open-request", Order = 100)]
        private void onToolsMenuOpenRequest(LayoutEvent e) {
            if (LayoutController.IsOperationMode) {
                Menu toolsMenu = (Menu)e.Info;
                bool anyAdded = false;

                foreach (LayoutPolicyInfo policy in LayoutModel.StateManager.LayoutPolicies)
                    if (policy.ShowInMenu) {
                        if (!anyAdded) {
                            if (toolsMenu.MenuItems.Count > 0 && toolsMenu.MenuItems[toolsMenu.MenuItems.Count - 1].Text != "-")
                                toolsMenu.MenuItems.Add("-");
                            anyAdded = true;
                        }

                        toolsMenu.MenuItems.Add(new PolicyMenuItem(policy));
                    }
            }
        }

        class PolicyMenuItem : MenuItem {
            readonly LayoutPolicyInfo policy;

            public PolicyMenuItem(LayoutPolicyInfo policy) {
                this.policy = policy;

                Text = policy.Name;
                Checked = policy.IsActive;
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                LayoutEventScript activeScript = policy.ActiveScript;

                if (activeScript != null)
                    activeScript.Dispose();
                else {
                    LayoutEventScript runningScript = EventManager.EventScript("Layout policy " + policy.Name, policy.EventScriptElement, new Guid[] { }, null);
                    runningScript.Id = policy.Id;

                    runningScript.Reset();
                }
            }
        }

        #endregion

        #region Get object attributes

        [LayoutEvent("get-object-attributes", SenderType = typeof(LocomotiveInfo))]
        [LayoutEvent("get-object-attributes", SenderType = typeof(LocomotiveTypeInfo))]
        private void getLocomotiveAttributes(LayoutEvent e) {
            ArrayList attributesList = (ArrayList)e.Info;

            foreach (XmlElement locoElement in LayoutModel.LocomotiveCollection.CollectionElement.GetElementsByTagName("Locomotive")) {
                AttributesOwner attributesOwner = new AttributesOwner(locoElement);

                if (attributesOwner.HasAttributes)
                    attributesList.Add(attributesOwner.Attributes);
            }

            if (LayoutModel.LocomotiveCatalog.IsLoaded) {
                foreach (XmlElement locoElement in LayoutModel.LocomotiveCatalog.CollectionElement.GetElementsByTagName("Locomotive")) {
                    AttributesOwner attributesOwner = new AttributesOwner(locoElement);

                    if (attributesOwner.HasAttributes)
                        attributesList.Add(attributesOwner.Attributes);
                }
            }
        }

        [LayoutEvent("get-object-attributes", SenderType = typeof(TripPlanInfo))]
        private void getTripPlanAttributes(LayoutEvent e) {
            ArrayList attributesList = (ArrayList)e.Info;

            foreach (TripPlanInfo tripPlan in LayoutModel.StateManager.TripPlansCatalog.TripPlans)
                if (tripPlan.HasAttributes)
                    attributesList.Add(tripPlan.Attributes);
        }

        [LayoutEvent("get-object-attributes", SenderType = typeof(LayoutBlockDefinitionComponent))]
        private void getBlockInfoAttributes(LayoutEvent e) {
            ArrayList attributesList = (ArrayList)e.Info;

            foreach (LayoutBlockDefinitionComponent blockInfo in LayoutModel.Components<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases))
                if (blockInfo.HasAttributes)
                    attributesList.Add(blockInfo.Attributes);
        }

        [LayoutEvent("get-object-attributes", SenderType = typeof(LayoutTrackContactComponent))]
        private void getTrackContactsAttributes(LayoutEvent e) {
            ArrayList attributesList = (ArrayList)e.Info;

            foreach (LayoutTrackContactComponent trackContact in LayoutModel.Components<LayoutTrackContactComponent>(LayoutModel.ActivePhases))
                if (trackContact.HasAttributes)
                    attributesList.Add(trackContact.Attributes);
        }

        [LayoutEvent("get-object-attributes", SenderType = typeof(TrainCommonInfo))]
        private void getTrainsAttributes(LayoutEvent e) {
            ArrayList attributesList = (ArrayList)e.Info;

            foreach (XmlElement trainElement in LayoutModel.StateManager.Trains.Element) {
                AttributesOwner attributesOwner = new AttributesOwner(trainElement);

                if (attributesOwner.HasAttributes)
                    attributesList.Add(attributesOwner.Attributes);
            }

            foreach (XmlElement trainElement in LayoutModel.LocomotiveCollection.CollectionElement.GetElementsByTagName("Train")) {
                AttributesOwner attributesOwner = new AttributesOwner(trainElement);

                if (attributesOwner.HasAttributes)
                    attributesList.Add(attributesOwner.Attributes);
            }
        }

        #endregion

        #region Update menus

        [LayoutEvent("get-event-script-editor-events-section-menu", Order = 200)]
        private void addEventsSectionAddMenuWithEvents(LayoutEvent e) {
            Menu menu = (Menu)e.Info;

            menu.MenuItems.Add(new CommonUI.Controls.EventScriptEditorAddMenuItem(e, "Do (script)", "RunPolicy"));
        }

        [LayoutEvent("get-event-script-editor-actions-section-menu", Order = 100)]
        private void getActionsSectionAddMenu(LayoutEvent e) {
            Menu menu = (Menu)e.Info;

            menu.MenuItems.Add(new CommonUI.Controls.EventScriptEditorAddMenuItem(e, "Trigger train function", "TriggerTrainFunction"));
            menu.MenuItems.Add(new CommonUI.Controls.EventScriptEditorAddMenuItem(e, "Set train function", "SetTrainFunction"));
            menu.MenuItems.Add(new CommonUI.Controls.EventScriptEditorAddMenuItem(e, "Change train target speed", "ChangeTrainTargetSpeed"));
            menu.MenuItems.Add(new CommonUI.Controls.EventScriptEditorAddMenuItem(e, "Control train lights", "ControlTrainLights"));
            menu.MenuItems.Add(new CommonUI.Controls.EventScriptEditorAddMenuItem(e, "Execute Trip-plan", "ExecuteTripPlan"));
            menu.MenuItems.Add(new CommonUI.Controls.EventScriptEditorAddMenuItem(e, "Execute random Trip-plan", "ExecuteRandomTripPlan"));
            menu.MenuItems.Add(new CommonUI.Controls.EventScriptEditorAddMenuItem(e, "Generate event", "GenerateEvent"));
        }

        [LayoutEvent("get-event-script-editor-insert-event-container-menu", Order = 100)]
        private void getEventScriptEditorInsertEventConatinerMenu(LayoutEvent e) {
            Menu menu = (Menu)e.Info;

            menu.MenuItems.Add(new CommonUI.Controls.EventScriptEditorInsertEventContainerMenuItem(e, "For each train", "ForEachTrain"));
        }

        [LayoutEvent("get-event-script-editor-events-section-menu", Order = 100)]
        private void addEventsSectionAddMenuWithEventContainers(LayoutEvent e) {
            Menu menu = (Menu)e.Info;

            menu.MenuItems.Add(new CommonUI.Controls.EventScriptEditorAddMenuItem(e, "For each train", "ForEachTrain"));
        }

        [LayoutEvent("get-event-script-editor-condition-section-menu", Order = 200)]
        private void addConditionSectionAddMenu(LayoutEvent e) {
            Menu menu = (Menu)e.Info;

            menu.MenuItems.Add("-");

            if (e.HasOption("BlockDefinitionID"))
                menu.MenuItems.Add(new CommonUI.Controls.EventScriptEditorAddMenuItem(e, "Train arrives from", "IfTrainArrivesFrom"));

            menu.MenuItems.Add(new CommonUI.Controls.EventScriptEditorAddMenuItem(e, "Train length", "IfTrainLength"));
        }


        #endregion

        #region Events

        #region Run Policy

        #region Runtime

        [LayoutEvent("parse-event-script-definition", IfSender = "RunPolicy")]
        private void parseRunPolicy(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeEventRunPolicy(e);
        }

        class LayoutEventScriptNodeEventRunPolicy : LayoutEventScriptNodeEvent {
            readonly LayoutPolicyInfo policy = null;
            LayoutEventScript eventScript = null;

            public LayoutEventScriptNodeEventRunPolicy(LayoutEvent e) : base(e) {
                LayoutPoliciesCollection policies = new LayoutPoliciesCollection(LayoutModel.Instance.GlobalPoliciesElement, LayoutModel.StateManager.LayoutPoliciesElement, null);

                policy = policies[XmlConvert.ToGuid(Element.GetAttribute("PolicyID"))];

                if (policy == null)
                    throw new LayoutEventScriptException(this, "The policy to run cannot be found. It was probably erased");
            }

            public override void Reset() {
                // Note that the reset operation may set Occured to true, if the condition is prechecked and is true
                base.Reset();

                if (eventScript != null) {
                    eventScript.Dispose();
                    eventScript = null;
                }

                if (!Occurred) {
                    eventScript = EventManager.EventScript(policy.Name, policy.EventScriptElement, ((LayoutEventScript)Script).ScopeIDs, new LayoutEvent("run-policy-done", this));
                    eventScript.ParentContext = Context;

                    eventScript.Reset();
                }
            }

            public override void Dispose() {
                Cancel();

                if (eventScript != null)
                    eventScript.Dispose();

                base.Dispose();
            }

            [LayoutEvent("run-policy-done")]
            private void runPolicyDone(LayoutEvent e) {
                if (e.Sender == this) {
                    if (eventScript != null) {          // Make sure that the event was not canceled (race condition)
                        if (IsConditionTrue)
                            Occurred = true;
                        else        // Condition is not true, RunPolicy again and check condition again
                            eventScript.Reset();
                    }
                }
            }
        }

        #endregion

        #region Editing

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "RunPolicy")]
        private void getRunPolicyTreeNode(LayoutEvent e) {
            XmlElement element = (XmlElement)e.Sender;

            e.Info = new LayoutEventScriptEditorTreeNodeRunPolicy(element);
        }

        [LayoutEvent("get-event-script-description", IfSender = "RunPolicy")]
        private void getRunPolicyDescription(LayoutEvent e) {
            e.Info = EventScriptUImanager.GetEventOrEventContainerDescription(e, LayoutEventScriptEditorTreeNodeRunPolicy.GetDescription((XmlElement)e.Sender));
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "RunPolicy")]
        private void editRunPolicyTreeNode(LayoutEvent e) {
            XmlElement element = (XmlElement)e.Sender;
            IEventScriptEditorSite site = (IEventScriptEditorSite)e.Info;
            EventScriptDialogs.RunPolicy d = new EventScriptDialogs.RunPolicy(element);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        class LayoutEventScriptEditorTreeNodeRunPolicy : CommonUI.Controls.LayoutEventScriptEditorTreeNodeEvent {

            public LayoutEventScriptEditorTreeNodeRunPolicy(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            public static string GetDescription(XmlElement element) {
                LayoutPolicyInfo policy = LayoutModel.StateManager.Policies(null)[XmlConvert.ToGuid(element.GetAttribute("PolicyID"))];
                string policyName;

                if (policy == null)
                    policyName = "[ERROR: Policy not found]";
                else
                    policyName = policy.Name;

                return "Do " + policyName;
            }

            protected override string Description => GetDescription(Element);

            public override bool SupportedInGlobalPolicy {
                get {
                    Guid policyID = XmlConvert.ToGuid(Element.GetAttribute("PolicyID"));
                    LayoutPoliciesCollection policies = new LayoutPoliciesCollection(LayoutModel.Instance.GlobalPoliciesElement, null, null);

                    return policies[policyID] != null;
                }
            }
        }

        #endregion

        #endregion

        #endregion

        #region Containers

        #region ForEachTrain

        #region Runtime

        [LayoutEvent("parse-event-script-definition", IfSender = "ForEachTrain")]
        private void ParseForEachTrain(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeEventContainerForEachTrain(e);
        }

        class LayoutEventScriptNodeEventContainerForEachTrain : LayoutEventScriptNodeEventBase {
            Guid[] trainIDs = null;
            int index = 0;
            readonly LayoutEventScriptNodeEventBase repeatedEvent;
            readonly LayoutScriptContext parentContext;

            public LayoutEventScriptNodeEventContainerForEachTrain(LayoutEvent e) : base(e) {
                if (Element.ChildNodes.Count < 1)
                    throw ParseErrorException("Missing element to repeat");
                else if (Element.ChildNodes.Count > 1)
                    throw ParseErrorException("Too many elements to repeat");

                XmlElement repeatedElement = (XmlElement)Element.ChildNodes[0];

                repeatedEvent = Parse(repeatedElement) as LayoutEventScriptNodeEventBase;

                if (repeatedEvent == null)
                    throw ParseErrorException("Error in For each Train body " + repeatedElement.Name + " is not valid event definition");

                parentContext = Context;
            }

            private bool prepareNextTrain() {
                TrainStateInfo train = null;

                while (index < trainIDs.Length) {
                    train = LayoutModel.StateManager.Trains[trainIDs[index++]];

                    if (train != null)
                        break;
                }

                if (train == null)
                    return false;           // We are done

                // Set the context with the new train information

                repeatedEvent.Context = new LayoutScriptContext("ForEachTrain", parentContext);

                EventManager.Event(new LayoutEvent("set-script-context", train, Context, null));

                TripPlanAssignmentInfo tripAssignment = (TripPlanAssignmentInfo)EventManager.Event(new LayoutEvent("get-train-active-trip", train));

                if (tripAssignment != null)
                    EventManager.Event(new LayoutEvent("set-script-context", tripAssignment, Context, null));

                return true;
            }

            public override void Reset() {
                base.Reset();
                ArrayList ids = new ArrayList();

                foreach (XmlElement trainElement in LayoutModel.StateManager.Trains.Element) {
                    TrainStateInfo train = new TrainStateInfo(trainElement);

                    ids.Add(train.Id);
                }

                trainIDs = (Guid[])ids.ToArray(typeof(Guid));

                index = 0;

                if (!prepareNextTrain())
                    Occurred = true;
                else
                    repeatedEvent.Reset();
            }

            public override void Recalculate() {
                if (!Occurred) {
                    bool proceed = true;

                    while (proceed) {
                        repeatedEvent.Recalculate();

                        if (repeatedEvent.Occurred) {
                            if (repeatedEvent.IsErrorState)
                                IsErrorState = true;

                            if (!prepareNextTrain()) {
                                Occurred = true;
                                proceed = false;
                            }
                            else
                                repeatedEvent.Reset();
                        }
                        else
                            proceed = false;
                    }
                }
            }

            public override void Cancel() {
                repeatedEvent.Cancel();
            }

            public override void Dispose() {
                repeatedEvent.Dispose();
                base.Dispose();
            }
        }

        #endregion

        #region Editing

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "ForEachTrain")]
        private void getForEachTrainTreeNode(LayoutEvent e) {
            XmlElement repeatElement = (XmlElement)e.Sender;

            e.Info = new LayoutEventScriptEditorTreeNodeForEachTrain(repeatElement);
        }

        [LayoutEvent("get-event-script-description", IfSender = "ForEachTrain")]
        private void getForEachTrainDescription(LayoutEvent e) {
            XmlElement element = (XmlElement)e.Sender;

            e.Info = LayoutManager.CommonUI.EventScriptUImanager.GetEventOrEventContainerDescription(e, LayoutEventScriptEditorTreeNodeForEachTrain.GetDescription(element) + " { ", " } ");
        }

        class LayoutEventScriptEditorTreeNodeForEachTrain : CommonUI.Controls.LayoutEventScriptEditorTreeNodeMayBeOptional {
            public LayoutEventScriptEditorTreeNodeForEachTrain(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            public override string AddNodeEventName => "get-event-script-editor-events-section-menu";

            public override string InsertNodeEventName => "get-event-script-editor-insert-event-container-menu";

            protected override int IconIndex => IconEvent;

            static public string GetDescription(XmlElement element) => "For each train";

            protected override string Description => GetDescription(Element);

            /// <summary>
            /// ForEachTrain can have no more than one subcondition
            /// </summary>
            public override int MaxSubNodes => 1;

            public override CommonUI.Controls.LayoutEventScriptEditorTreeNode NodeToEdit => null;       // Node cannot be edited
        }

        #endregion

        #endregion

        #endregion

        #region Coniditions

        #region Train arrives from

        #region Runtime

        [LayoutEvent("parse-event-script-definition", IfSender = "IfTrainArrivesFrom")]
        private void parseIfTrainArrivesFrom(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeConditionIfTrainsArrive(e);
        }

        public class LayoutEventScriptNodeConditionIfTrainsArrive : LayoutEventScriptNodeCondition {
            public LayoutEventScriptNodeConditionIfTrainsArrive(LayoutEvent e)
                : base(e) {
            }

            public override bool IsTrue {
                get {
                    bool result = false;

                    if (Context["Block"] is LayoutBlock block) {
                        LayoutBlock fromBlock = Context["PreviousBlock"] as LayoutBlock;

                        // This code can run in two contexts:
                        // 1. During layout planning, in this case the train arrives from Context["PreviousBlock"]
                        // 2. During actual trip, in this case TODO
                        if (fromBlock == null) {
                            // Todo: Get previous block
                        }

                        if (fromBlock != null) {
                            LayoutBlockDefinitionComponent blockDefinition = block.BlockDefinintion;
                            LayoutComponentConnectionPoint from = LayoutComponentConnectionPoint.Parse(Element.GetAttribute("From"));
                            int fromCpIndex = blockDefinition.GetConnectionPointIndex(from);
                            LayoutBlockEdgeBase[] fromBlockEdges = blockDefinition.GetBlockEdges(fromCpIndex);

                            foreach (LayoutBlockEdgeBase fromBlockEdge in fromBlockEdges) {
                                if (block.OtherBlock(fromBlockEdge) == fromBlock) {
                                    result = true;
                                    break;
                                }
                            }
                        }
                    }

                    return result;
                }
            }
        }

        #endregion

        #region Editing

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "IfTrainArrivesFrom")]
        private void getIfTrainArrivesFromTreeNode(LayoutEvent e) {
            XmlElement element = (XmlElement)e.Sender;

            e.Info = new LayoutEventScriptEditorTreeNodeIfTrainArrivesFrom(element);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "IfTrainArrivesFrom")]
        private void editIfTrainsArriveFromTreeNode(LayoutEvent e) {
            XmlElement element = (XmlElement)e.Sender;
            IEventScriptEditorSite site = (IEventScriptEditorSite)e.Info;
            EventScriptDialogs.TrainArrivesFrom d = new LayoutManager.Tools.EventScriptDialogs.TrainArrivesFrom(site.BlockDefinition, element);

            if (d.ShowDialog() == DialogResult.OK) {
                element.SetAttribute("From", d.From.ToString());
                site.EditingDone();
            }
            else
                site.EditingCancelled();
        }

        [LayoutEvent("get-event-script-description", IfSender = "IfTrainArrivesFrom")]
        private void getIfTrainsArriveFromDescription(LayoutEvent e) {
            e.Info = LayoutEventScriptEditorTreeNodeIfTrainArrivesFrom.GetDescription((XmlElement)e.Sender);
        }


        public class LayoutEventScriptEditorTreeNodeIfTrainArrivesFrom : LayoutEventScriptEditorTreeNodeCondition {
            public LayoutEventScriptEditorTreeNodeIfTrainArrivesFrom(XmlElement conditionElement)
                : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            protected override string Description => GetDescription(Element);

            internal static string GetDescription(XmlElement element) {
                LayoutComponentConnectionPoint from = LayoutComponentConnectionPoint.Parse(element.GetAttribute("From"));
                string fromText;

                switch (from) {
                    case LayoutComponentConnectionPoint.B: fromText = "bottom"; break;
                    case LayoutComponentConnectionPoint.T: fromText = "top"; break;
                    case LayoutComponentConnectionPoint.L: fromText = "left"; break;
                    case LayoutComponentConnectionPoint.R: fromText = "right"; break;

                    default: fromText = "{UNKNOWN}"; break;
                }

                return "If train arrives from " + fromText;
            }
        }

        #endregion

        #endregion

        #region Train length

        #region Runtime

        [LayoutEvent("parse-event-script-definition", IfSender = "IfTrainLength")]
        private void parseIfTrainLength(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeConditionIfTrainsLength(e);
        }


        public class LayoutEventScriptNodeConditionIfTrainsLength : LayoutEventScriptNodeCondition {
            public LayoutEventScriptNodeConditionIfTrainsLength(LayoutEvent e)
                : base(e) {
            }

            public override bool IsTrue {
                get {
                    string trainSymbol;
                    TrainLength length;
                    TrainLengthComparison comparison;

                    if (Element.HasAttribute("TrainSymbol"))
                        trainSymbol = Element.GetAttribute("TrainSymbol");
                    else
                        trainSymbol = "Train";

                    if (Element.HasAttribute("Length"))
                        length = TrainLength.Parse(Element.GetAttribute("Length"));
                    else
                        length = TrainLength.VeryLong;

                    if (Element.HasAttribute("Comparison"))
                        comparison = (TrainLengthComparison)Enum.Parse(typeof(TrainLengthComparison), Element.GetAttribute("Comparison"));
                    else
                        comparison = TrainLengthComparison.NotLonger;

                    object oTrain = Context[trainSymbol];

                    if (!(oTrain is TrainStateInfo))
                        throw new ArgumentException("Symbol " + trainSymbol + " is not a reference to a valid train object");
                    TrainStateInfo train = (TrainStateInfo)oTrain;

                    if (comparison == TrainLengthComparison.NotLonger)
                        return train.Length <= length;
                    else
                        return train.Length > length;
                }
            }
        }

        #endregion

        #region Editing

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "IfTrainLength")]
        private void getIfTrainLengthTreeNode(LayoutEvent e) {
            XmlElement element = (XmlElement)e.Sender;

            e.Info = new LayoutEventScriptEditorTreeNodeIfTrainLength(element);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "IfTrainLength")]
        private void editIfTrainLengthFromTreeNode(LayoutEvent e) {
            XmlElement element = (XmlElement)e.Sender;
            IEventScriptEditorSite site = (IEventScriptEditorSite)e.Info;

            string trainSymbol;
            TrainLength length;
            TrainLengthComparison comparison;

            if (element.HasAttribute("TrainSymbol"))
                trainSymbol = element.GetAttribute("TrainSymbol");
            else
                trainSymbol = "Train";

            if (element.HasAttribute("Length"))
                length = TrainLength.Parse(element.GetAttribute("Length"));
            else
                length = TrainLength.VeryLong;

            if (element.HasAttribute("Comparison"))
                comparison = (TrainLengthComparison)Enum.Parse(typeof(TrainLengthComparison), element.GetAttribute("Comparison"));
            else
                comparison = TrainLengthComparison.NotLonger;

            EventScriptDialogs.IfTrainLength d = new LayoutManager.Tools.EventScriptDialogs.IfTrainLength(trainSymbol, length, comparison);

            if (d.ShowDialog() == DialogResult.OK) {
                element.SetAttribute("TrainSymbol", d.SymbolName);
                element.SetAttribute("Length", d.Length.ToString());
                element.SetAttribute("Comparison", d.Comparison.ToString());
                site.EditingDone();
            }
            else
                site.EditingCancelled();
        }

        [LayoutEvent("get-event-script-description", IfSender = "IfTrainLength")]
        private void getIfTrainLengthDescription(LayoutEvent e) {
            e.Info = LayoutEventScriptEditorTreeNodeIfTrainLength.GetDescription((XmlElement)e.Sender);
        }


        public class LayoutEventScriptEditorTreeNodeIfTrainLength : LayoutEventScriptEditorTreeNodeCondition {
            public LayoutEventScriptEditorTreeNodeIfTrainLength(XmlElement conditionElement)
                : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            protected override string Description => GetDescription(Element);

            internal static string GetDescription(XmlElement element) {
                string symbolName = element.GetAttribute("TrainSymbol");
                TrainLength length = TrainLength.Parse(element.GetAttribute("Length"));
                TrainLengthComparison comparison = (TrainLengthComparison)Enum.Parse(typeof(TrainLengthComparison), element.GetAttribute("Comparison"));

                string c = "";

                switch (comparison) {
                    case TrainLengthComparison.Longer: c = "longer than"; break;
                    case TrainLengthComparison.NotLonger: c = "not longer than"; break;
                }

                return "If " + symbolName + " is " + c + " " + length.ToDisplayString(true);
            }
        }

        #endregion

        #endregion

        #endregion

        #region Actions

        #region TriggerTrainFunction

        #region Runtime

        [LayoutEvent("parse-event-script-definition", IfSender = "TriggerTrainFunction")]
        private void parseTriggerTrainFunction(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeActionTriggerTrainFunction(e);
        }

        class LayoutEventScriptNodeActionTriggerTrainFunction : LayoutEventScriptNodeAction {

            public LayoutEventScriptNodeActionTriggerTrainFunction(LayoutEvent e) : base(e) {
            }

            public override void Execute() {
                string functionName = Element.GetAttribute("FunctionName");
                string symbolName = Element.GetAttribute("TrainSymbol");
                object oTrain = Context[symbolName];

                if (!(oTrain is TrainStateInfo))
                    throw new ArgumentException("Symbol " + symbolName + " is not a reference to a valid train object");
                TrainStateInfo train = (TrainStateInfo)oTrain;

                train.TriggerLocomotiveFunction(functionName, Guid.Empty);
            }
        }

        #endregion

        #region Editing

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "TriggerTrainFunction")]
        private void getTriggerTrainFunctionTreeNode(LayoutEvent e) {
            XmlElement element = (XmlElement)e.Sender;

            e.Info = new LayoutEventScriptEditorTreeNodeTriggerTrainFunction(element);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "TriggerTrainFunction")]
        private void editTriggerTrainFunctionTreeNode(LayoutEvent e) {
            XmlElement element = (XmlElement)e.Sender;
            IEventScriptEditorSite site = (IEventScriptEditorSite)e.Info;
            EventScriptDialogs.TrainFunctionAction d = new EventScriptDialogs.TrainFunctionAction(element);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [LayoutEvent("get-event-script-description", IfSender = "TriggerTrainFunction")]
        private void getTriggerTrainFunctionDescription(LayoutEvent e) {
            e.Info = LayoutEventScriptEditorTreeNodeTriggerTrainFunction.GetDescription((XmlElement)e.Sender);
        }

        class LayoutEventScriptEditorTreeNodeTriggerTrainFunction : CommonUI.Controls.LayoutEventScriptEditorTreeNodeAction {
            public LayoutEventScriptEditorTreeNodeTriggerTrainFunction(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            public static string GetDescription(XmlElement element) {
                string functionName = element.GetAttribute("FunctionName");
                string symbolName = element.GetAttribute("TrainSymbol");

                return "Trigger function " + functionName + " of " + symbolName;
            }

            protected override string Description => GetDescription(Element);
        }

        #endregion // Editing

        #endregion // TriggerTrainFunction

        #region SetTrainFunction

        #region Runtime

        [LayoutEvent("parse-event-script-definition", IfSender = "SetTrainFunction")]
        private void parseSetTrainFunction(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeActionSetTrainFunction(e);
        }

        class LayoutEventScriptNodeActionSetTrainFunction : LayoutEventScriptNodeAction {

            public LayoutEventScriptNodeActionSetTrainFunction(LayoutEvent e) : base(e) {
            }

            public override void Execute() {
                string functionName = Element.GetAttribute("FunctionName");
                string symbolName = Element.GetAttribute("TrainSymbol");
                string action = Element.GetAttribute("Action");
                object oTrain = Context[symbolName];

                if (!(oTrain is TrainStateInfo))
                    throw new ArgumentException("Symbol " + symbolName + " is not a reference to a valid train object");
                TrainStateInfo train = (TrainStateInfo)oTrain;

                switch (action) {
                    case "On":
                        train.SetLocomotiveFunctionState(functionName, Guid.Empty, true);
                        break;

                    case "Off":
                        train.SetLocomotiveFunctionState(functionName, Guid.Empty, true);
                        break;

                    case "Toggle":
                        bool state = train.GetFunctionState(functionName, Guid.Empty, false);

                        state = !state;
                        train.SetLocomotiveFunctionState(functionName, Guid.Empty, state);
                        break;
                }
            }
        }

        #endregion

        #region Editing

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "SetTrainFunction")]
        private void getSetTrainFunctionTreeNode(LayoutEvent e) {
            XmlElement element = (XmlElement)e.Sender;

            e.Info = new LayoutEventScriptEditorTreeNodeSetTrainFunction(element);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "SetTrainFunction")]
        private void editSetTrainFunctionTreeNode(LayoutEvent e) {
            XmlElement element = (XmlElement)e.Sender;
            IEventScriptEditorSite site = (IEventScriptEditorSite)e.Info;
            EventScriptDialogs.TrainFunctionAction d = new EventScriptDialogs.TrainFunctionAction(element);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [LayoutEvent("get-event-script-description", IfSender = "SetTrainFunction")]
        private void getSetTrainFunctionDescription(LayoutEvent e) {
            e.Info = LayoutEventScriptEditorTreeNodeSetTrainFunction.GetDescription((XmlElement)e.Sender);
        }

        class LayoutEventScriptEditorTreeNodeSetTrainFunction : CommonUI.Controls.LayoutEventScriptEditorTreeNodeAction {
            public LayoutEventScriptEditorTreeNodeSetTrainFunction(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            public static string GetDescription(XmlElement element) {
                string functionName = element.GetAttribute("FunctionName");
                string symbolName = element.GetAttribute("TrainSymbol");
                string action = element.GetAttribute("Action");
                string verb = "";

                switch (action) {
                    case "On": verb = "Set"; break;
                    case "Off": verb = "Reset"; break;
                    case "Toggle": verb = "Toggle"; break;
                }

                return verb + " function " + functionName + " of " + symbolName;
            }

            protected override string Description => GetDescription(Element);
        }

        #endregion // Editing

        #endregion // SetTrainFunction

        #region Change Target Speed

        #region Runtime

        [LayoutEvent("parse-event-script-definition", IfSender = "ChangeTrainTargetSpeed")]
        private void parseChangeTrainTargetSpeed(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeActionChangeTrainTargetSpeed(e);
        }

        class LayoutEventScriptNodeActionChangeTrainTargetSpeed : LayoutEventScriptNodeAction {

            public LayoutEventScriptNodeActionChangeTrainTargetSpeed(LayoutEvent e) : base(e) {
            }

            public override void Execute() {
                string symbolName = Element.GetAttribute("TrainSymbol");
                string action = Element.GetAttribute("Action");
                object oTrain = Context[symbolName];

                if (!(oTrain is TrainStateInfo))
                    throw new ArgumentException("Symbol " + symbolName + " is not a reference to a valid train object");
                TrainStateInfo train = (TrainStateInfo)oTrain;

                object oSpeed = GetOperand("Value");
                int speed;

                if (oSpeed is string)
                    speed = int.Parse((string)oSpeed);
                else if (oSpeed is int)
                    speed = (int)oSpeed;
                else
                    throw new ArgumentException("Invalid target speed value");

                switch (action) {
                    case "Set":
                        if (speed < 1 || speed >= LayoutModel.Instance.LogicalSpeedSteps)
                            throw new ArgumentException("Invalid target speed " + speed + " - valid values are between 1 and " + LayoutModel.Instance.LogicalSpeedSteps);

                        train.TargetSpeed = speed;
                        break;

                    case "Increase":
                        speed = train.TargetSpeed + speed;
                        setSpeed(train, speed);
                        break;

                    case "Decrease":
                        speed = train.TargetSpeed - speed;
                        setSpeed(train, speed);
                        break;
                }
            }

            private void setSpeed(TrainStateInfo train, int speed) {
                if (speed < 1)
                    speed = 1;
                else if (speed > LayoutModel.Instance.LogicalSpeedSteps)
                    speed = LayoutModel.Instance.LogicalSpeedSteps;

                train.TargetSpeed = speed;
            }
        }

        #endregion

        #region Editing

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "ChangeTrainTargetSpeed")]
        private void getChangeTrainTargetSpeedTreeNode(LayoutEvent e) {
            XmlElement element = (XmlElement)e.Sender;

            e.Info = new LayoutEventScriptEditorTreeNodeChangeTrainTargetSpeed(element);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "ChangeTrainTargetSpeed")]
        private void editChangeTrainTargetSpeedTreeNode(LayoutEvent e) {
            XmlElement element = (XmlElement)e.Sender;
            IEventScriptEditorSite site = (IEventScriptEditorSite)e.Info;
            EventScriptDialogs.TrainLightsAction d = new EventScriptDialogs.TrainLightsAction(element);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [LayoutEvent("get-event-script-description", IfSender = "ChangeTrainTargetSpeed")]
        private void getChangeTrainTargetSpeedDescription(LayoutEvent e) {
            e.Info = LayoutEventScriptEditorTreeNodeChangeTrainTargetSpeed.GetDescription((XmlElement)e.Sender);
        }

        class LayoutEventScriptEditorTreeNodeChangeTrainTargetSpeed : CommonUI.Controls.LayoutEventScriptEditorTreeNodeAction {
            public LayoutEventScriptEditorTreeNodeChangeTrainTargetSpeed(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            public static string GetDescription(XmlElement element) {
                string symbolName = element.GetAttribute("TrainSymbol");
                string action = element.GetAttribute("Action");
                string verb = "";
                string relation = "to";

                switch (action) {
                    case "Set": verb = "Set"; break;
                    case "Increase": verb = "Increase"; relation = "by"; break;
                    case "Decrease": verb = "Decrease"; relation = "by"; break;
                }

                return $"{verb} target speed of {symbolName} {relation} {GetOperandDescription(element, "Value", typeof(int))}";
            }

            protected override string Description => GetDescription(Element);
        }

        #endregion

        #endregion

        #region Control train lights

        #region Runtime

        [LayoutEvent("parse-event-script-definition", IfSender = "ControlTrainLights")]
        private void parseControlTrainLights(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeActionControlTrainLights(e);
        }

        class LayoutEventScriptNodeActionControlTrainLights : LayoutEventScriptNodeAction {

            public LayoutEventScriptNodeActionControlTrainLights(LayoutEvent e) : base(e) {
            }

            public override void Execute() {
                string symbolName = Element.GetAttribute("TrainSymbol");
                string action = Element.GetAttribute("Action");
                object oTrain = Context[symbolName];

                if (!(oTrain is TrainStateInfo))
                    throw new ArgumentException("Symbol " + symbolName + " is not a reference to a valid train object");

                TrainStateInfo train = (TrainStateInfo)oTrain;

                object oState = GetOperand("Value");
                bool state;

                if (oState is string)
                    state = bool.Parse((string)oState);
                else if (oState is bool)
                    state = (bool)oState;
                else
                    throw new ArgumentException("Invalid lights state value");

                switch (action) {
                    case "Set":
                        train.Lights = state;
                        break;

                    case "Toggle":
                        train.Lights = !state;
                        break;
                }
            }
        }

        #endregion

        #region Editing

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "ControlTrainLights")]
        private void getChangeControlTrainLightsTreeNode(LayoutEvent e) {
            XmlElement element = (XmlElement)e.Sender;

            e.Info = new LayoutEventScriptEditorTreeNodeControlTrainLights(element);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "ControlTrainLights")]
        private void editControlTrainLightsTreeNode(LayoutEvent e) {
            XmlElement element = (XmlElement)e.Sender;
            IEventScriptEditorSite site = (IEventScriptEditorSite)e.Info;
            var d = new EventScriptDialogs.TrainLightsAction(element);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [LayoutEvent("get-event-script-description", IfSender = "ControlTrainLights")]
        private void getControlTrainLightsDescription(LayoutEvent e) {
            e.Info = LayoutEventScriptEditorTreeNodeControlTrainLights.GetDescription((XmlElement)e.Sender);
        }


        class LayoutEventScriptEditorTreeNodeControlTrainLights : CommonUI.Controls.LayoutEventScriptEditorTreeNodeAction {
            public LayoutEventScriptEditorTreeNodeControlTrainLights(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            public static string GetDescription(XmlElement element) {
                string symbolName = element.GetAttribute("TrainSymbol");
                string action = element.GetAttribute("Action");
                string verb = "";

                switch (action) {
                    case "Set": verb = ""; break;
                    case "Toggle": verb = " toggle"; break;
                }

                var value = GetOperandDescription(element, "Value", typeof(bool));

                if (value.ToLower() == "true")
                    value = "On";
                else if (value.ToLower() == "false")
                    value = "Off";

                return $"Set lights of {symbolName} to{verb} {value}";
            }

            protected override string Description => GetDescription(Element);
        }

        #endregion

        #endregion

        #region Execute Trip-plan

        #region Runtime

        [LayoutEvent("parse-event-script-definition", IfSender = "ExecuteTripPlan")]
        private void parseExecuteTripPlan(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeActionExecuteTripPlan(e);
        }

        class LayoutEventScriptNodeActionExecuteTripPlan : LayoutEventScriptNodeAction {

            public LayoutEventScriptNodeActionExecuteTripPlan(LayoutEvent e) : base(e) {
            }

            public override void Execute() {
                Guid tripPlanID = XmlConvert.ToGuid(Element.GetAttribute("TripPlanID"));
                bool shouldReverse = XmlConvert.ToBoolean(Element.GetAttribute("ShouldReverse"));
                string symbolName = Element.GetAttribute("TrainSymbol");
                object oTrain = Context[symbolName];

                if (oTrain == null || !(oTrain is TrainStateInfo))
                    throw new ArgumentException("Symbol " + symbolName + " is not a reference to a valid train object");

                TrainStateInfo train = (TrainStateInfo)oTrain;
                TripPlanInfo catalogTripPlan = LayoutModel.StateManager.TripPlansCatalog.TripPlans[tripPlanID];

                if (catalogTripPlan == null)
                    throw new LayoutEventScriptException(this, "The trip plan to execute cannot be found. It was probably erased");
                else {
                    XmlDocument workingDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();
                    XmlElement tripPlanElement = (XmlElement)workingDoc.ImportNode(catalogTripPlan.Element, true);

                    workingDoc.AppendChild(tripPlanElement);
                    TripPlanInfo tripPlan = new TripPlanInfo(tripPlanElement);

                    if (shouldReverse)
                        tripPlan.Reverse();

                    EventManager.Event(new LayoutEvent("execute-trip-plan", new TripPlanAssignmentInfo(tripPlan, train)));
                }
            }
        }

        #endregion

        #region Editing

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "ExecuteTripPlan")]
        private void getExecuteTripPlanTreeNode(LayoutEvent e) {
            XmlElement element = (XmlElement)e.Sender;

            e.Info = new LayoutEventScriptEditorTreeNodeExecuteTripPlan(element);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "ExecuteTripPlan")]
        private void editExecuteTripPlanTreeNode(LayoutEvent e) {
            XmlElement element = (XmlElement)e.Sender;
            IEventScriptEditorSite site = (IEventScriptEditorSite)e.Info;
            EventScriptDialogs.ExecuteTripPlan d = new EventScriptDialogs.ExecuteTripPlan(element);

            new SemiModalDialog(site.Form, d, new SemiModalDialogClosedHandler(onCloseExecuteTripPlan), site).ShowDialog();
        }

        private void onCloseExecuteTripPlan(Form dialog, object info) {
            IEventScriptEditorSite site = (IEventScriptEditorSite)info;

            if (dialog.DialogResult == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [LayoutEvent("get-event-script-description", IfSender = "ExecuteTripPlan")]
        private void getExecuteTripPlanDescription(LayoutEvent e) {
            e.Info = LayoutEventScriptEditorTreeNodeExecuteTripPlan.GetDescription((XmlElement)e.Sender);
        }

        class LayoutEventScriptEditorTreeNodeExecuteTripPlan : CommonUI.Controls.LayoutEventScriptEditorTreeNodeAction {
            public LayoutEventScriptEditorTreeNodeExecuteTripPlan(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            public static string GetDescription(XmlElement element) {
                string symbolName = element.GetAttribute("TrainSymbol");
                Guid tripPlanID = XmlConvert.ToGuid(element.GetAttribute("TripPlanID"));
                bool shouldReverse = XmlConvert.ToBoolean(element.GetAttribute("ShouldReverse"));
                string tripPlanName = "[Error: UNKNOWN]";

                TripPlanInfo tripPlan = LayoutModel.StateManager.TripPlansCatalog.TripPlans[tripPlanID];

                if (tripPlan != null)
                    tripPlanName = tripPlan.Name;

                return symbolName + " executes trip plan " + tripPlanName + (shouldReverse ? " (reversed)" : "");
            }

            protected override string Description => GetDescription(Element);

            public override bool SupportedInGlobalPolicy => false;

        }

        #endregion

        #endregion

        #region ExecuteRandomTripPlan

        #region Runtime

        [LayoutEvent("parse-event-script-definition", IfSender = "ExecuteRandomTripPlan")]
        private void parseExecuteRandomTripPlan(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeActionExecuteRandomTripPlan(e, this);
        }

        [LayoutEventDef("no-applicable-trip-plans-notification", Role = LayoutEventRole.Notification, SenderType = typeof(TrainStateInfo))]
        class LayoutEventScriptNodeActionExecuteRandomTripPlan : LayoutEventScriptNodeAction {
            static readonly LayoutTraceSwitch traceRandomTripPlan = new LayoutTraceSwitch("ExecuteRandomTripPlan", "Execute Random Trip-plan");
            const int PenaltyThreashold = 1000;
            readonly LayoutModuleBase mb;

            public LayoutEventScriptNodeActionExecuteRandomTripPlan(LayoutEvent e, LayoutModuleBase mb) : base(e) {
                this.mb = mb;
            }

            public override void Execute() {
                string symbol = Element.GetAttribute("Symbol");
                object oTrain = Context[symbol];

                if (oTrain == null || !(oTrain is TrainStateInfo))
                    throw new ArgumentException("Symbol " + symbol + " is not a reference to a valid train object");

                TrainStateInfo train = (TrainStateInfo)oTrain;

                XmlDocument workingDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();
                XmlElement applicableTripPlansElement = workingDoc.CreateElement("ApplicableTripPlans");
                bool selectCircular = XmlConvert.ToBoolean(Element.GetAttribute("SelectCircularTripPlans"));
                string selectReversedMethod = Element.GetAttribute("ReversedTripPlanSelection");
                ArrayList tripPlanCanidates = new ArrayList();
                ArrayList reversedTripPlanCandidates = new ArrayList();
                LayoutConditionScript filter = new LayoutConditionScript("Execute Random Tripplan Filter", Element["Filter"], true);
                LayoutScriptContext scriptContext = filter.ScriptContext;

                EventManager.Event(new LayoutEvent("set-script-context", train, scriptContext, null));

                workingDoc.AppendChild(applicableTripPlansElement);
                applicableTripPlansElement.SetAttribute("StaticGrade", XmlConvert.ToString(false));

                EventManager.Event(new LayoutEvent("get-applicable-trip-plans-request", train, applicableTripPlansElement, null));

                if (traceRandomTripPlan.TraceVerbose) {
                    Trace.WriteLine("*** Execute Random trip plan ***");
                    Trace.WriteLine("Applicable trip plan");
                    Trace.WriteLine(applicableTripPlansElement.OuterXml);
                }

                foreach (XmlElement applicableTripPlanElement in applicableTripPlansElement) {
                    TripPlanInfo tripPlan = LayoutModel.StateManager.TripPlansCatalog.TripPlans[XmlConvert.ToGuid(applicableTripPlanElement.GetAttribute("TripPlanID"))];
                    int penalty = XmlConvert.ToInt32(applicableTripPlanElement.GetAttribute("Penalty"));
                    RouteClearanceQuality clearanceQuality = (RouteClearanceQuality)Enum.Parse(typeof(RouteClearanceQuality), applicableTripPlanElement.GetAttribute("ClearanceQuality"));
                    RouteQuality routeQuality = new RouteQuality(train.Id, clearanceQuality, penalty);

                    if (tripPlan != null && routeQuality.IsFree) {
                        if (!tripPlan.IsCircular || selectCircular) {
                            EventManager.Event(new LayoutEvent("set-script-context", tripPlan, scriptContext, null));

                            Application.DoEvents();

                            if (filter.IsTrue) {
                                bool destinationIsFree = (bool)EventManager.Event(new LayoutEvent("check-trip-plan-destination-request", tripPlan));

                                if (XmlConvert.ToBoolean(applicableTripPlanElement.GetAttribute("ShouldReverse")) || !destinationIsFree)
                                    reversedTripPlanCandidates.Add(applicableTripPlanElement);
                                else
                                    tripPlanCanidates.Add(applicableTripPlanElement);
                            }
                        }
                    }
                }

                int upperLimit = -1;

                switch (selectReversedMethod) {
                    case "Yes":
                        upperLimit = tripPlanCanidates.Count + reversedTripPlanCandidates.Count;
                        break;

                    case "No":
                        upperLimit = tripPlanCanidates.Count;
                        break;

                    case "IfNoAlternative":
                        if (tripPlanCanidates.Count > 0)
                            upperLimit = tripPlanCanidates.Count;
                        else
                            upperLimit = reversedTripPlanCandidates.Count;
                        break;
                }


                if (upperLimit == 0)
                    EventManager.Event(new LayoutEvent("no-applicable-trip-plans-notification", train));
                else {
                    int index = new Random().Next(upperLimit);
                    XmlElement selectedApplicableTripPlanElement = null;

                    if (tripPlanCanidates.Count == 0 && selectReversedMethod != "Yes")
                        LayoutModuleBase.Message(train, "No optimal trip-plan are available for selection by 'execute random trip-plan'");

                    if (index >= tripPlanCanidates.Count)
                        selectedApplicableTripPlanElement = (XmlElement)reversedTripPlanCandidates[index - tripPlanCanidates.Count];
                    else
                        selectedApplicableTripPlanElement = (XmlElement)tripPlanCanidates[index];

                    if (selectedApplicableTripPlanElement != null) {
                        TripPlanInfo selectedTripPlan = LayoutModel.StateManager.TripPlansCatalog.TripPlans[XmlConvert.ToGuid(selectedApplicableTripPlanElement.GetAttribute("TripPlanID"))];
                        bool shouldReverse = XmlConvert.ToBoolean(selectedApplicableTripPlanElement.GetAttribute("ShouldReverse"));

                        if (traceRandomTripPlan.TraceInfo) {
                            Trace.WriteLine("*** Selected trip: " + selectedApplicableTripPlanElement.OuterXml);
                            selectedTripPlan.Dump();
                        }

                        // Now submit the trip plan
                        XmlDocument tripPlanDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();
                        XmlElement tripPlanElement = (XmlElement)tripPlanDoc.ImportNode(selectedTripPlan.Element, true);

                        tripPlanDoc.AppendChild(tripPlanElement);
                        TripPlanInfo tripPlan = new TripPlanInfo(tripPlanElement);

                        if (shouldReverse)
                            tripPlan.Reverse();

                        EventManager.Event(new LayoutEvent("execute-trip-plan", new TripPlanAssignmentInfo(tripPlan, train)));
                    }
                }
            }
        }

        #endregion

        #region Editing

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "ExecuteRandomTripPlan")]
        private void getExecuteRandomTripPlanTreeNode(LayoutEvent e) {
            XmlElement element = (XmlElement)e.Sender;

            e.Info = new LayoutEventScriptEditorTreeNodeExecuteRandomTripPlan(element);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "ExecuteRandomTripPlan")]
        private void editExecuteRandomTripPlanTreeNode(LayoutEvent e) {
            XmlElement element = (XmlElement)e.Sender;
            IEventScriptEditorSite site = (IEventScriptEditorSite)e.Info;
            EventScriptDialogs.ExecuteRandomTripPlan d = new EventScriptDialogs.ExecuteRandomTripPlan(element);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [LayoutEvent("get-event-script-description", IfSender = "ExecuteRandomTripPlan")]
        private void getExecuteRandomTripPlanDescription(LayoutEvent e) {
            e.Info = LayoutEventScriptEditorTreeNodeExecuteRandomTripPlan.GetDescription((XmlElement)e.Sender);
        }

        class LayoutEventScriptEditorTreeNodeExecuteRandomTripPlan : CommonUI.Controls.LayoutEventScriptEditorTreeNodeAction {
            public LayoutEventScriptEditorTreeNodeExecuteRandomTripPlan(XmlElement conditionElement) : base(conditionElement) {
            }

            public static string GetDescription(XmlElement element) => element.GetAttribute("Symbol") + " executes random trip-plan";

            protected override string Description => GetDescription(Element);
        }

        #endregion

        #endregion

        #endregion // Actions
    }
}
