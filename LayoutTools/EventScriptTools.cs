using LayoutManager.CommonUI;
using LayoutManager.CommonUI.Controls;
using LayoutManager.Components;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;

#pragma warning disable IDE0051, IDE0060, IDE0052, IDE0067
#nullable enable
namespace LayoutManager.Tools {
    [LayoutModule("EventScript Tools", UserControl = false)]
    internal class EventScriptTools : LayoutModuleBase {
        private const string A_From = "From";
        private const string A_TrainSymbol = "TrainSymbol";
        private const string A_Length = "Length";
        private const string A_Comparison = "Comparison";

        #region Symbol name to object type map

        [DispatchTarget]
        private void AddContextSymbolsAndTypes(Dictionary<string, Type> symbolToTypeMap) {
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
        private void GetContextSymbolInfoType(LayoutEvent e) {
            var symbolType = Ensure.NotNull<Type>(e.Sender, "symbolType");

            if (symbolType == typeof(LayoutBlockDefinitionComponent) || symbolType.IsSubclassOf(typeof(LayoutBlockDefinitionComponent)))
                e.Info = typeof(LayoutBlockDefinitionComponentInfo);
            else if (symbolType == typeof(LayoutSignalComponent) || symbolType.IsSubclassOf(typeof(LayoutSignalComponent)))
                e.Info = typeof(LayoutSignalComponentInfo);
        }

        #endregion

        #region Script operator to operator name mapping

        [LayoutEvent("get-event-script-operator-name", IfSender = "IfString")]
        private void GetIfStringOperatorName(LayoutEvent e) {
            var ifStringElement = Ensure.NotNull<XmlElement>(e.Sender, "ifStringElement");
            string compareOperator = ifStringElement.GetAttribute("Operation");

            e.Info = compareOperator switch {
                "Equal" => "=",
                "NotEqual" => "<>",
                "Match" => "Match",
                _ => "?"
            };
        }

        [LayoutEvent("get-event-script-operator-name", IfSender = "IfNumber")]
        private void GetIfNumberOperatorName(LayoutEvent e) {
            var ifNumberElement = Ensure.NotNull<XmlElement>(e.Sender, "ifNumberElement");
            string compareOperator = ifNumberElement.GetAttribute("Operation");

            e.Info = compareOperator switch {
                "eq" => "=",
                "ne" => "<>",
                "gt" => ">",
                "ge" => ">=",
                "le" => "=<",
                "lt" => "<",
                _ => "?"
            };
        }

        [LayoutEvent("get-event-script-operator-name", IfSender = "IfBoolean")]
        private void GetIfBooleanOperatorName(LayoutEvent e) {
            var ifBooleanElement = Ensure.NotNull<XmlElement>(e.Sender, "ifBooleanElement");
            string compareOperator = ifBooleanElement.GetAttribute("Operation");

            e.Info = compareOperator switch {
                "Equal" => "=",
                "NotEqual" => "<>",
                _ => "?"
            };
        }

        #endregion

        #region Event script error handler

        [DispatchTarget]
        private void EventScriptError(LayoutEventScript script, LayoutEventScriptErrorInfo errorInfo) {
            // TODO: Invoke event script debugger pointing exactly on the erroneous node.. for now just complain

            Error($"Error in execution of event script '{errorInfo.Script.Name}' - {errorInfo?.Exception?.Message ?? "Unexpected error"}");
        }

        #endregion

        #region Context builders

        private class TrainToTripResolver : ILayoutScriptContextResolver {
            private readonly TrainStateInfo train;

            public TrainToTripResolver(TrainStateInfo train) {
                this.train = train;
            }

            public object? Resolve(LayoutScriptContext context, string symbolName) {
                var trip = Dispatch.Call.GetActiveTrip(train) ?? Dispatch.Call.GetEditedTrip(train);

                if (trip == null)
                    return null;

                Dispatch.Call.SetScriptContext(trip, context);
                return context[symbolName];
            }
        }

        [DispatchTarget]
        private void SetScriptContext([DispatchFilter] TrainStateInfo train, LayoutScriptContext context) {

            if (!context.Contains("Train", train)) {
                context["Train"] = train;
                context["TrainLocations"] = train.Locations;
                context["TrainLocomotiveLocation"] = train.LocomotiveLocation;
                context["TrainLocomotives"] = train.Locomotives;

                TrainToTripResolver trainToTripResolver = new(train);

                context["Trip"] = trainToTripResolver;
                context["Driver"] = trainToTripResolver;
                context["FinalDestination"] = trainToTripResolver;
                context["CurrentWayPoint"] = trainToTripResolver;
                context["Destination"] = trainToTripResolver;
                context["TripPlan"] = trainToTripResolver;
                context["TripPlanWayPoints"] = trainToTripResolver;
            }
        }

        [DispatchTarget]
        private void SetScriptContext_TripPlanAssignment([DispatchFilter] TripPlanAssignmentInfo trip, LayoutScriptContext context) {

            if (!context.Contains("Trip", trip)) {
                context["Trip"] = trip;
                context["Driver"] = trip.Train.Driver;
                if (trip.TripPlan.Waypoints.Count > 0)
                    context["FinalDestination"] = trip.TripPlan.Waypoints[trip.TripPlan.Waypoints.Count - 1];
                else
                    context.Remove("FinalDestination");

                if (trip.CurrentWaypoint != null) {
                    context["CurrentWayPoint"] = trip.CurrentWaypoint;
                    context["Destination"] = trip.CurrentWaypoint.Destination;
                }
                else {      // Ensure that resolvers (placed by train) is removed to avoid infinite resolving loop
                    context.Remove("CurrentWayPoint");
                    context.Remove("Destination");
                }

                Dispatch.Call.SetScriptContext(trip.TripPlan, context);

                if (!context.Contains("Train", trip.Train))
                    Dispatch.Call.SetScriptContext(trip.Train, context);
            }
        }

        [DispatchTarget]
        private void SetScriptContext_TripPlan([DispatchFilter] TripPlanInfo tripPlan, LayoutScriptContext context) {

            if (!context.Contains("TripPlan", tripPlan)) {
                context["TripPlan"] = tripPlan;
                context["TripPlanWayPoints"] = tripPlan.Waypoints;
            }
        }

        [DispatchTarget]
        private void SetScriptContext_Block([DispatchFilter] LayoutBlock block, LayoutScriptContext context) {

            if (!context.Contains("Block", block)) {
                context["Block"] = block;

                if (block.BlockDefinintion != null)
                    Dispatch.Call.SetScriptContext(block.BlockDefinintion, context);
            }
        }

        [DispatchTarget]
        private void SetScriptContext_BlockDefinition([DispatchFilter] LayoutBlockDefinitionComponent blockDefinition, LayoutScriptContext context) {
            if (!context.Contains("BlockInfo", blockDefinition)) {
                context["BlockInfo"] = blockDefinition;
            }
        }

        #endregion

        #region Wait For Event Reset handlers

        [LayoutEvent("event-script-wait-event-reset", IfSender = "WaitForEvent[@Name='train-in-block']")]
        [LayoutEventDef("train-in-block", Role = LayoutEventRole.Notification, SenderType = typeof(TrainStateInfo), InfoType = typeof(LayoutBlock))]
        private void ResetTrainInBlock(LayoutEvent e) {
            var subscription = Ensure.NotNull<LayoutEventSubscription>(e.Info, "subscription");

            foreach (XmlElement trainElement in LayoutModel.StateManager.Trains.Element) {
                TrainStateInfo train = new(trainElement);

                foreach (TrainLocationInfo trainLocation in train.Locations)
                    subscription.EventHandler?.Invoke(new LayoutEvent("train-in-block", train, trainLocation.Block));
            }
        }

        [DispatchTarget]
        private void OnTrainEnteredBlock(TrainStateInfo train, LayoutBlock block) {
            EventManager.Event(new LayoutEvent("train-in-block", train, block));
        }

        #endregion

        #region Add layout wide policies to tools menu

        [DispatchTarget(Order = 100)]
        [DispatchFilter("InOperationMode")]
        private void ToolsMenuOpenRequest(MenuOrMenuItem toolsMenu) {
            bool anyAdded = false;

            foreach (LayoutPolicyInfo policy in LayoutModel.StateManager.LayoutPolicies)
                if (policy.ShowInMenu) {
                    if (!anyAdded) {
                        if (toolsMenu.Items.Count > 0 && toolsMenu.Items[toolsMenu.Items.Count - 1] is not ToolStripSeparator)
                            toolsMenu.Items.Add(new ToolStripSeparator());
                        anyAdded = true;
                    }

                    toolsMenu.Items.Add(new PolicyMenuItem(policy));
                }
        }

        private class PolicyMenuItem : LayoutMenuItem {
            private readonly LayoutPolicyInfo policy;

            public PolicyMenuItem(LayoutPolicyInfo policy) {
                this.policy = policy;

                Text = policy.Name;
                Checked = policy.IsActive;
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                var activeScript = policy.ActiveScript;

                if (activeScript != null)
                    activeScript.Dispose();
                else if (policy.EventScriptElement != null) {
                    LayoutEventScript runningScript = EventManager.EventScript("Layout policy " + policy.Name, policy.EventScriptElement, Array.Empty<Guid>(), null);
                    runningScript.Id = policy.Id;

                    runningScript.Reset();
                }
            }
        }

        #endregion

        #region Get object attributes

        private void GetLocomotiveAttributes(List<AttributesInfo> attributesList) {
            foreach (var locoElement in LayoutModel.LocomotiveCollection.CollectionElement.GetElementsByTagName("Locomotive")) {
                AttributesOwner attributesOwner = new((XmlElement)locoElement);

                if (attributesOwner.HasAttributes)
                    attributesList.Add(attributesOwner.Attributes);
            }

            if (LayoutModel.LocomotiveCatalog.IsLoaded) {
                foreach (var locoElement in LayoutModel.LocomotiveCatalog.CollectionElement.GetElementsByTagName("Locomotive")) {
                    AttributesOwner attributesOwner = new((XmlElement)locoElement);

                    if (attributesOwner.HasAttributes)
                        attributesList.Add(attributesOwner.Attributes);
                }
            }
        }

        [DispatchTarget]
        private void GetObjectAttributes_Locomotive(List<AttributesInfo> attributesList, [DispatchFilter("==", typeof(LocomotiveInfo))] Type atributeType) => GetLocomotiveAttributes(attributesList);

        [DispatchTarget]
        private void GetObjectAttributes_LocomotiveType(List<AttributesInfo> attributesList, [DispatchFilter("==", typeof(LocomotiveTypeInfo))] Type atributeType) => GetLocomotiveAttributes(attributesList);


        [DispatchTarget]
        private void GetObjectAttributes_TripPlan(List<AttributesInfo> attributesList, [DispatchFilter("==", typeof(TripPlanInfo))] Type atributeType) {
            foreach (TripPlanInfo tripPlan in LayoutModel.StateManager.TripPlansCatalog.TripPlans)
                if (tripPlan.HasAttributes)
                    attributesList.Add(tripPlan.Attributes);
        }

        [DispatchTarget]
        private void GetObjectAttributes_BlockDefinition(List<AttributesInfo> attributesList, [DispatchFilter("==", typeof(LayoutBlockDefinitionComponent))] Type atributeType) {
            foreach (LayoutBlockDefinitionComponent blockInfo in LayoutModel.Components<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases))
                if (blockInfo.HasAttributes)
                    attributesList.Add(blockInfo.Attributes);
        }

        [DispatchTarget]
        private void GetObjectAttributes_TrackContact(List<AttributesInfo> attributesList, [DispatchFilter("==", typeof(LayoutTrackContactComponent))] Type atributeType) {
            foreach (LayoutTrackContactComponent trackContact in LayoutModel.Components<LayoutTrackContactComponent>(LayoutModel.ActivePhases))
                if (trackContact.HasAttributes)
                    attributesList.Add(trackContact.Attributes);
        }

        [DispatchTarget]
        private void GetObjectAttributes_Train(List<AttributesInfo> attributesList, [DispatchFilter("==", typeof(TrainCommonInfo))] Type atributeType) {
            foreach (XmlElement trainElement in LayoutModel.StateManager.Trains.Element) {
                AttributesOwner attributesOwner = new(trainElement);

                if (attributesOwner.HasAttributes)
                    attributesList.Add(attributesOwner.Attributes);
            }

            foreach (XmlElement trainElement in LayoutModel.LocomotiveCollection.CollectionElement.GetElementsByTagName("Train")) {
                AttributesOwner attributesOwner = new(trainElement);

                if (attributesOwner.HasAttributes)
                    attributesList.Add(attributesOwner.Attributes);
            }
        }

#endregion

#region Update menus

        //[LayoutEvent("get-event-script-editor-events-section-menu", Order = 200)]
        [DispatchTarget(Order = 200)]
        private void GetEventScriptEditorEventSectionMenu(LayoutEventScriptEditorTreeNode eventNode, bool hasBlockDefinition, MenuOrMenuItem menu) {
            menu.Items.Add(new CommonUI.Controls.EventScriptEditorAddMenuItem(eventNode, "Do (script)", "RunPolicy"));
        }

        //[LayoutEvent("get-event-script-editor-actions-section-menu", Order = 100)]
        [DispatchTarget(Order = 100)]
        private void GetEventScriptEditoActionsSectionMenu(LayoutEventScriptEditorTreeNode eventNode, bool hasBlockDefinition, MenuOrMenuItem menu) {
            menu.Items.Add(new CommonUI.Controls.EventScriptEditorAddMenuItem(eventNode, "Trigger train function", "TriggerTrainFunction"));
            menu.Items.Add(new CommonUI.Controls.EventScriptEditorAddMenuItem(eventNode, "Set train function", "SetTrainFunction"));
            menu.Items.Add(new CommonUI.Controls.EventScriptEditorAddMenuItem(eventNode, "Change train target speed", "ChangeTrainTargetSpeed"));
            menu.Items.Add(new CommonUI.Controls.EventScriptEditorAddMenuItem(eventNode, "Control train lights", "ControlTrainLights"));
            menu.Items.Add(new CommonUI.Controls.EventScriptEditorAddMenuItem(eventNode, "Execute Trip-plan", "ExecuteTripPlan"));
            menu.Items.Add(new CommonUI.Controls.EventScriptEditorAddMenuItem(eventNode, "Execute random Trip-plan", "ExecuteRandomTripPlan"));
            menu.Items.Add(new CommonUI.Controls.EventScriptEditorAddMenuItem(eventNode, "Generate event", "GenerateEvent"));
        }

        //[LayoutEvent("get-event-script-editor-insert-event-container-menu", Order = 100)]
        [DispatchTarget(Order = 100)]
        private void GetEventScriptEditorInsertEventConatinerMenu(LayoutEventScriptEditorTreeNode eventNode, bool hasBlockDefinition, MenuOrMenuItem menu) {
            menu.Items.Add(new CommonUI.Controls.EventScriptEditorInsertEventContainerMenuItem(eventNode, "For each train", "ForEachTrain"));
        }

        //[LayoutEvent("get-event-script-editor-events-section-menu", Order = 100)]
        [DispatchTarget(Order = 100)]
        private void GetEventScriptEditorEventSectionMenu_ForEachTrain(LayoutEventScriptEditorTreeNode eventNode, bool hasBlockDefinition, MenuOrMenuItem menu) {
            menu.Items.Add(new CommonUI.Controls.EventScriptEditorAddMenuItem(eventNode, "For each train", "ForEachTrain"));
        }

        //[LayoutEvent("get-event-script-editor-condition-section-menu", Order = 200)]
        [DispatchTarget(Order = 200)]
        private void GetEventScriptEditorConditionSectionMenu(LayoutEventScriptEditorTreeNode eventNode, bool hasBlockDefinition, MenuOrMenuItem menu) {
            menu.Items.Add(new ToolStripSeparator());

            if (hasBlockDefinition)
                menu.Items.Add(new CommonUI.Controls.EventScriptEditorAddMenuItem(eventNode, "Train arrives from", "IfTrainArrivesFrom"));

            menu.Items.Add(new CommonUI.Controls.EventScriptEditorAddMenuItem(eventNode, "Train length", "IfTrainLength"));
        }

        #endregion

        #region Events

        #region Run Policy

        #region Runtime

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_RunPolicy([DispatchFilter("XPath", "RunPolicy")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeEventRunPolicy(parseEventInfo);

        private class LayoutEventScriptNodeEventRunPolicy : LayoutEventScriptNodeEvent {
            private const string A_PolicyId = "PolicyID";
            private readonly LayoutPolicyInfo? policy;
            private LayoutEventScript? eventScript;

            public LayoutEventScriptNodeEventRunPolicy(LayoutParseEventScript parseEventScript) : base(parseEventScript) {
                LayoutPoliciesCollection policies = new(LayoutModel.Instance.GlobalPoliciesElement, LayoutModel.StateManager.LayoutPoliciesElement, null);

                policy = policies[(Guid)Element.AttributeValue(A_PolicyId)];

                if (policy == null)
                    throw new LayoutEventScriptException(this, "The policy to run cannot be found. It was probably erased");
            }

            public override void Reset() {
                // Note that the reset operation may set Occurred to true, if the condition is prechecked and is true
                base.Reset();

                if (eventScript != null) {
                    eventScript.Dispose();
                    eventScript = null;
                }

                if (!Occurred && policy != null && policy.EventScriptElement != null) {
                    eventScript = EventManager.EventScript(policy.Name, policy.EventScriptElement, ((LayoutEventScript)Script).ScopeIDs, () => EventManager.Event(new LayoutEvent("run-policy-done", this)));
                    eventScript.ParentContext = Context;

                    eventScript.Reset();
                }
            }

            protected override void Dispose(bool disposing) {
                if (disposing) {
                    Cancel();

                    if (eventScript != null)
                        eventScript.Dispose();
                }

                base.Dispose(disposing);
            }

            [LayoutEvent("run-policy-done")]
            private void RunPolicyDone(LayoutEvent e) {
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

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_RunPolicy([DispatchFilter("XPath", "RunPolicy")] XmlElement element) => new LayoutEventScriptEditorTreeNodeRunPolicy(element);

        [DispatchTarget]
        private string GetEventScriptDescription_RunPolicy([DispatchFilter("XPath", "RunPolicy")] XmlElement element) => EventScriptUImanager.GetEventOrEventContainerDescription(element, LayoutEventScriptEditorTreeNodeRunPolicy.GetDescription(element));

        [DispatchTarget]
        private void EditEventScriptElement_RunPolicy([DispatchFilter("XPath", "RunPolicy")] XmlElement element, IEventScriptEditorSite site) {
            var d = new EventScriptDialogs.RunPolicy(element);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        private class LayoutEventScriptEditorTreeNodeRunPolicy : CommonUI.Controls.LayoutEventScriptEditorTreeNodeEvent {
            private const string A_PolicyId = "PolicyID";

            public LayoutEventScriptEditorTreeNodeRunPolicy(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            public static string GetDescription(XmlElement element) {
                var policy = LayoutModel.StateManager.Policies(null)[(Guid)element.AttributeValue(A_PolicyId)];
                var policyName = policy == null ? "[ERROR: Policy not found]" : policy.Name;
                return $"Do {policyName}";
            }

            protected override string Description => GetDescription(Element);

            public override bool SupportedInGlobalPolicy {
                get {
                    Guid policyID = (Guid)Element.AttributeValue(A_PolicyId);
                    LayoutPoliciesCollection policies = new(LayoutModel.Instance.GlobalPoliciesElement, null, null);

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

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_ForEachTrain([DispatchFilter("XPath", "ForEachTrain")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeEventContainerForEachTrain(parseEventInfo);

        private class LayoutEventScriptNodeEventContainerForEachTrain : LayoutEventScriptNodeEventBase {
            private Guid[]? trainIDs = null;
            private int index = 0;
            private readonly LayoutEventScriptNodeEventBase? repeatedEvent;
            private readonly LayoutScriptContext parentContext;

            public LayoutEventScriptNodeEventContainerForEachTrain(LayoutParseEventScript parseEventScript) : base(parseEventScript) {
                if (Element.ChildNodes.Count < 1)
                    throw ParseErrorException("Missing element to repeat");
                else if (Element.ChildNodes.Count > 1)
                    throw ParseErrorException("Too many elements to repeat");

                var repeatedElement = Ensure.NotNull<XmlElement>(Element.ChildNodes[0]);

                repeatedEvent = Parse(repeatedElement) as LayoutEventScriptNodeEventBase;

                if (repeatedEvent == null)
                    throw ParseErrorException("Error in For each Train body " + repeatedElement.Name + " is not valid event definition");

                parentContext = Context;
            }

            private bool PrepareNextTrain() {
                TrainStateInfo? train = null;

                if (trainIDs != null) {
                    while (index < trainIDs.Length) {
                        train = LayoutModel.StateManager.Trains[trainIDs[index++]];

                        if (train != null)
                            break;
                    }
                }

                if (train == null)
                    return false;           // We are done

                // Set the context with the new train information

                if (repeatedEvent != null)
                    repeatedEvent.Context = new LayoutScriptContext("ForEachTrain", parentContext);

                Dispatch.Call.SetScriptContext(train, Context);

                var tripAssignment = Dispatch.Call.GetActiveTrip(train) ?? Dispatch.Call.GetEditedTrip(train);

                if (tripAssignment != null)
                    Dispatch.Call.SetScriptContext(tripAssignment, Context);

                return true;
            }

            public override void Reset() {
                base.Reset();
                ArrayList ids = new();

                foreach (XmlElement trainElement in LayoutModel.StateManager.Trains.Element) {
                    TrainStateInfo train = new(trainElement);

                    ids.Add(train.Id);
                }

                trainIDs = (Guid[])ids.ToArray(typeof(Guid));

                index = 0;

                if (!PrepareNextTrain())
                    Occurred = true;
                else
                    repeatedEvent?.Reset();
            }

            public override void Recalculate() {
                if (!Occurred) {
                    bool proceed = true;

                    while (proceed && repeatedEvent != null) {
                        repeatedEvent.Recalculate();

                        if (repeatedEvent.Occurred) {
                            if (repeatedEvent.IsErrorState)
                                IsErrorState = true;

                            if (!PrepareNextTrain()) {
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
                repeatedEvent?.Cancel();
            }

            protected override void Dispose(bool disposing) {
                if (disposing) {
                    repeatedEvent?.Dispose();
                }
                base.Dispose(disposing);
            }
        }

#endregion

#region Editing

       [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_ForEachTrain([DispatchFilter("XPath", "ForEachTrain")] XmlElement element) => new LayoutEventScriptEditorTreeNodeForEachTrain(element);

        [DispatchTarget]
        private string GetEventScriptDescription_ForEachTrain([DispatchFilter("XPath", "ForEachTrain")] XmlElement element) => EventScriptUImanager.GetEventOrEventContainerDescription(element, LayoutEventScriptEditorTreeNodeForEachTrain.GetDescription(element));


        private class LayoutEventScriptEditorTreeNodeForEachTrain : CommonUI.Controls.LayoutEventScriptEditorTreeNodeMayBeOptional {
            public LayoutEventScriptEditorTreeNodeForEachTrain(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            public override Action<LayoutEventScriptEditorTreeNode, bool, MenuOrMenuItem> AddNodeMenuFunction => Dispatch.Call.GetEventScriptEditorEventSectionMenu;

            public override Action<LayoutEventScriptEditorTreeNode, bool, MenuOrMenuItem> InsertNodeMenuFunction => Dispatch.Call.GetEventScriptEditorInsertEventConatinerMenu;

            protected override int IconIndex => IconEvent;

            static public string GetDescription(XmlElement element) => "For each train";

            protected override string Description => GetDescription(Element);

            /// <summary>
            /// ForEachTrain can have no more than one subcondition
            /// </summary>
            public override int MaxSubNodes => 1;

            public override CommonUI.Controls.LayoutEventScriptEditorTreeNode? NodeToEdit => null;       // Node cannot be edited
        }

        #endregion

        #endregion

        #endregion

        #region Coniditions

        #region Train arrives from

        #region Runtime

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_IfTrainArrivesFrom([DispatchFilter("XPath", "IfTrainArrivesFrom")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeConditionIfTrainsArrive(parseEventInfo);

        public class LayoutEventScriptNodeConditionIfTrainsArrive : LayoutEventScriptNodeCondition {
            public LayoutEventScriptNodeConditionIfTrainsArrive(LayoutParseEventScript parseEventScript) : base(parseEventScript) {
            }

            public override bool IsTrue {
                get {
                    bool result = false;

                    if (Context["Block"] is LayoutBlock block) {
                        var fromBlock = Context["PreviousBlock"] as LayoutBlock;

                        // This code can run in two contexts:
                        // 1. During layout planning, in this case the train arrives from Context["PreviousBlock"]
                        // 2. During actual trip, in this case TODO
                        if (fromBlock == null) {
                            // Todo: Get previous block
                        }

                        if (fromBlock != null) {
                            LayoutBlockDefinitionComponent blockDefinition = block.BlockDefinintion;
                            LayoutComponentConnectionPoint from = LayoutComponentConnectionPoint.Parse(Element.GetAttribute(A_From));
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

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_IfTrainArrivesFrom([DispatchFilter("XPath", "IfTrainArrivesFrom")] XmlElement element) => new LayoutEventScriptEditorTreeNodeIfTrainArrivesFrom(element);

        [DispatchTarget]
        private string GetEventScriptDescription_IfTrainArrivesFrom([DispatchFilter("XPath", "IfTrainArrivesFrom")] XmlElement element) => EventScriptUImanager.GetEventOrEventContainerDescription(element, LayoutEventScriptEditorTreeNodeIfTrainArrivesFrom.GetDescription(element));

        [DispatchTarget]
        private void EditEventScriptElement_IfTrainArrivesFrom([DispatchFilter("XPath", "IfTrainArrivesFrom")] XmlElement element, IEventScriptEditorSite site) {
            var d = new LayoutManager.Tools.EventScriptDialogs.TrainArrivesFrom(site.BlockDefinition, element);

            if (d.ShowDialog() == DialogResult.OK) {
                element.SetAttribute(A_From, d.From.ToString());
                site.EditingDone();
            }
            else
                site.EditingCancelled();
        }


        public class LayoutEventScriptEditorTreeNodeIfTrainArrivesFrom : LayoutEventScriptEditorTreeNodeCondition {
            public LayoutEventScriptEditorTreeNodeIfTrainArrivesFrom(XmlElement conditionElement)
                : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            protected override string Description => GetDescription(Element);

            internal static string GetDescription(XmlElement element) {
                LayoutComponentConnectionPoint from = LayoutComponentConnectionPoint.Parse(element.GetAttribute(EventScriptTools.A_From));

                var fromText = (int)from switch {
                    LayoutComponentConnectionPoint.B => "bottom",
                    LayoutComponentConnectionPoint.T => "top",
                    LayoutComponentConnectionPoint.L => "left",
                    LayoutComponentConnectionPoint.R => "right",
                    _ => "{UNKNOWN}",
                };
                return "If train arrives from " + fromText;
            }
        }

        #endregion

        #endregion

        #region Train length

        #region Runtime

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_IfTrainLength([DispatchFilter("XPath", "IfTrainLength")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeConditionIfTrainsLength(parseEventInfo);

        public class LayoutEventScriptNodeConditionIfTrainsLength : LayoutEventScriptNodeCondition {
            public LayoutEventScriptNodeConditionIfTrainsLength(LayoutParseEventScript parseEventScript) : base(parseEventScript) {
            }

            public override bool IsTrue {
                get {
                    string trainSymbol;
                    TrainLength length;
                    TrainLengthComparison comparison;

                    if (Element.HasAttribute(A_TrainSymbol))
                        trainSymbol = Element.GetAttribute(A_TrainSymbol);
                    else
                        trainSymbol = "Train";

                    if (Element.HasAttribute(A_Length))
                        length = TrainLength.Parse(Element.GetAttribute(A_Length));
                    else
                        length = TrainLength.VeryLong;

                    if (Element.HasAttribute("Comparison"))
                        comparison = (TrainLengthComparison)Enum.Parse(typeof(TrainLengthComparison), Element.GetAttribute("Comparison"));
                    else
                        comparison = TrainLengthComparison.NotLonger;

                    var oTrain = Context[trainSymbol];

                    if (oTrain is not TrainStateInfo)
                        throw new ArgumentException("Symbol " + trainSymbol + " is not a reference to a valid train object");
                    TrainStateInfo train = (TrainStateInfo)oTrain;

                    return comparison == TrainLengthComparison.NotLonger ? train.Length <= length : train.Length > length;
                }
            }
        }

#endregion

#region Editing

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_IfTrainLength([DispatchFilter("XPath", "IfTrainLength")] XmlElement element) => new LayoutEventScriptEditorTreeNodeIfTrainLength(element);

        [DispatchTarget]
        private string GetEventScriptDescription_IfTrainLength([DispatchFilter("XPath", "IfTrainLength")] XmlElement element) => EventScriptUImanager.GetEventOrEventContainerDescription(element, LayoutEventScriptEditorTreeNodeIfTrainLength.GetDescription(element));

        [DispatchTarget]
        private void EditEventScriptElement_IfTrainLength([DispatchFilter("XPath", "IfTrainLength")] XmlElement element, IEventScriptEditorSite site) {
            var trainSymbol = (string?)element.AttributeValue(A_TrainSymbol) ?? "Train";
            var length = element.AttributeValue(A_Length).Enum<TrainLength>() ?? TrainLength.VeryLong;
            var comparison = element.AttributeValue(A_Comparison).Enum<TrainLengthComparison>() ?? TrainLengthComparison.NotLonger;

            using EventScriptDialogs.IfTrainLength d = new(trainSymbol, length, comparison);

            if (d.ShowDialog() == DialogResult.OK) {
                element.SetAttribute(A_TrainSymbol, d.SymbolName);
                element.SetAttributeValue(A_Length, d.Length);
                element.SetAttributeValue(A_Comparison, d.Comparison);
                site.EditingDone();
            }
            else
                site.EditingCancelled();
        }

        public class LayoutEventScriptEditorTreeNodeIfTrainLength : LayoutEventScriptEditorTreeNodeCondition {
            public LayoutEventScriptEditorTreeNodeIfTrainLength(XmlElement conditionElement)
                : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            protected override string Description => GetDescription(Element);

            internal static string GetDescription(XmlElement element) {
                string symbolName = element.GetAttribute(EventScriptTools.A_TrainSymbol);
                TrainLength length = TrainLength.Parse(element.GetAttribute(EventScriptTools.A_Length));
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

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_TriggerTrainFunction([DispatchFilter("XPath", "TriggerTrainFunction")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeActionTriggerTrainFunction(parseEventInfo);

        private class LayoutEventScriptNodeActionTriggerTrainFunction : LayoutEventScriptNodeAction {
            public LayoutEventScriptNodeActionTriggerTrainFunction(LayoutParseEventScript parseEventScript) : base(parseEventScript) {
            }

            public override void Execute() {
                string functionName = Element.GetAttribute("FunctionName");
                string symbolName = Element.GetAttribute(A_TrainSymbol);
                var oTrain = Context[symbolName];

                if (oTrain is not TrainStateInfo)
                    throw new ArgumentException("Symbol " + symbolName + " is not a reference to a valid train object");
                TrainStateInfo train = (TrainStateInfo)oTrain;

                train.TriggerLocomotiveFunction(functionName, Guid.Empty);
            }
        }

#endregion

#region Editing

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_TriggerTrainFunction([DispatchFilter("XPath", "TriggerTrainFunction")] XmlElement element) => new LayoutEventScriptEditorTreeNodeTriggerTrainFunction(element);

        [DispatchTarget]
        private string GetEventScriptDescription_TriggerTrainFunction([DispatchFilter("XPath", "TriggerTrainFunction")] XmlElement element) => EventScriptUImanager.GetEventOrEventContainerDescription(element, LayoutEventScriptEditorTreeNodeTriggerTrainFunction.GetDescription(element));

        [DispatchTarget]
        private void EditEventScriptElement_TriggerTrainFunction([DispatchFilter("XPath", "TriggerTrainFunction")] XmlElement element, IEventScriptEditorSite site) {
            var d = new EventScriptDialogs.TrainFunctionAction(element);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }


        private class LayoutEventScriptEditorTreeNodeTriggerTrainFunction : CommonUI.Controls.LayoutEventScriptEditorTreeNodeAction {
            public LayoutEventScriptEditorTreeNodeTriggerTrainFunction(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            public static string GetDescription(XmlElement element) {
                string functionName = element.GetAttribute("FunctionName");
                string symbolName = element.GetAttribute(EventScriptTools.A_TrainSymbol);

                return "Trigger function " + functionName + " of " + symbolName;
            }

            protected override string Description => GetDescription(Element);
        }

        #endregion // Editing

        #endregion // TriggerTrainFunction

        #region SetTrainFunction

        #region Runtime

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_SetTrainFunction([DispatchFilter("XPath", "SetTrainFunction")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeActionSetTrainFunction(parseEventInfo);

        private class LayoutEventScriptNodeActionSetTrainFunction : LayoutEventScriptNodeAction {
            public LayoutEventScriptNodeActionSetTrainFunction(LayoutParseEventScript parseEventScript) : base(parseEventScript) {
            }

            public override void Execute() {
                string functionName = Element.GetAttribute("FunctionName");
                string symbolName = Element.GetAttribute(A_TrainSymbol);
                string action = Element.GetAttribute("Action");
                var oTrain = Context[symbolName];

                if (oTrain is not TrainStateInfo)
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

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_SetTrainFunction([DispatchFilter("XPath", "SetTrainFunction")] XmlElement element) => new LayoutEventScriptEditorTreeNodeSetTrainFunction(element);

        [DispatchTarget]
        private string GetEventScriptDescription_SetTrainFunction([DispatchFilter(Type = "XPath", Value = "SetTrainFunction")] XmlElement element) => EventScriptUImanager.GetEventOrEventContainerDescription(element, LayoutEventScriptEditorTreeNodeSetTrainFunction.GetDescription(element));

        [DispatchTarget]
        private void EditEventScriptElement_SetTrainFunction([DispatchFilter("XPath", "SetTrainFunction")] XmlElement element, IEventScriptEditorSite site) {
            var d = new EventScriptDialogs.TrainFunctionAction(element);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        private class LayoutEventScriptEditorTreeNodeSetTrainFunction : CommonUI.Controls.LayoutEventScriptEditorTreeNodeAction {
            public LayoutEventScriptEditorTreeNodeSetTrainFunction(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            public static string GetDescription(XmlElement element) {
                string functionName = element.GetAttribute("FunctionName");
                string symbolName = element.GetAttribute(EventScriptTools.A_TrainSymbol);
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

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_ChangeTrainTargetSpeed([DispatchFilter("XPath", "ChangeTrainTargetSpeed")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeActionChangeTrainTargetSpeed(parseEventInfo);

        private class LayoutEventScriptNodeActionChangeTrainTargetSpeed : LayoutEventScriptNodeAction {
            public LayoutEventScriptNodeActionChangeTrainTargetSpeed(LayoutParseEventScript parseEventScript) : base(parseEventScript) {
            }

            public override void Execute() {
                string symbolName = Element.GetAttribute(A_TrainSymbol);
                string action = Element.GetAttribute("Action");
                var oTrain = Context[symbolName];

                if (oTrain is not TrainStateInfo)
                    throw new ArgumentException("Symbol " + symbolName + " is not a reference to a valid train object");
                TrainStateInfo train = (TrainStateInfo)oTrain;

                var oSpeed = GetOperand("Value");
                int speed = oSpeed switch {
                    string speedText => int.Parse(speedText),
                    int speedValue => speedValue,
                    _ => throw new ArgumentException("Invalid target speed value")
                };

                switch (action) {
                    case "Set":
                        if (speed < 1 || speed >= LayoutModel.Instance.LogicalSpeedSteps)
                            throw new ArgumentException("Invalid target speed " + speed + " - valid values are between 1 and " + LayoutModel.Instance.LogicalSpeedSteps);

                        train.TargetSpeed = speed;
                        break;

                    case "Increase":
                        speed = train.TargetSpeed + speed;
                        SetSpeed(train, speed);
                        break;

                    case "Decrease":
                        speed = train.TargetSpeed - speed;
                        SetSpeed(train, speed);
                        break;
                }
            }

            private void SetSpeed(TrainStateInfo train, int speed) {
                if (speed < 1)
                    speed = 1;
                else if (speed > LayoutModel.Instance.LogicalSpeedSteps)
                    speed = LayoutModel.Instance.LogicalSpeedSteps;

                train.TargetSpeed = speed;
            }
        }

#endregion

#region Editing

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_ChangeTrainTargetSpeed([DispatchFilter("XPath", "ChangeTrainTargetSpeed")] XmlElement element) => new LayoutEventScriptEditorTreeNodeChangeTrainTargetSpeed(element);

        [DispatchTarget]
        private string GetEventScriptDescription_ChangeTrainTargetSpeed([DispatchFilter(Type = "XPath", Value = "ChangeTrainTargetSpeed")] XmlElement element) => EventScriptUImanager.GetEventOrEventContainerDescription(element, LayoutEventScriptEditorTreeNodeChangeTrainTargetSpeed.GetDescription(element));

        [DispatchTarget]
        private void EditEventScriptElement_ChangeTrainTargetSpeed([DispatchFilter("XPath", "ChangeTrainTargetSpeed")] XmlElement element, IEventScriptEditorSite site) {
            var d = new EventScriptDialogs.TrainLightsAction(element);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        private class LayoutEventScriptEditorTreeNodeChangeTrainTargetSpeed : CommonUI.Controls.LayoutEventScriptEditorTreeNodeAction {
            public LayoutEventScriptEditorTreeNodeChangeTrainTargetSpeed(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            public static string GetDescription(XmlElement element) {
                string symbolName = element.GetAttribute(EventScriptTools.A_TrainSymbol);
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

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_ControlTrainLights([DispatchFilter("XPath", "ControlTrainLights")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeActionControlTrainLights(parseEventInfo);

        private class LayoutEventScriptNodeActionControlTrainLights : LayoutEventScriptNodeAction {
            public LayoutEventScriptNodeActionControlTrainLights(LayoutParseEventScript parseEventScript) : base(parseEventScript) {
            }

            public override void Execute() {
                string symbolName = Element.GetAttribute(A_TrainSymbol);
                string action = Element.GetAttribute("Action");
                var oTrain = Context[symbolName];

                if (oTrain is not TrainStateInfo)
                    throw new ArgumentException("Symbol " + symbolName + " is not a reference to a valid train object");

                TrainStateInfo train = (TrainStateInfo)oTrain;

                var oState = GetOperand("Value");
                bool state = oState switch {
                    string stateText => bool.Parse(stateText),
                    bool stateValue => stateValue,
                    _ => throw new ArgumentException("Invalid lights state value")
                };

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

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_ControlTrainLights([DispatchFilter("XPath", "ControlTrainLights")] XmlElement element) => new LayoutEventScriptEditorTreeNodeControlTrainLights(element);

        [DispatchTarget]
        private string GetEventScriptDescription_ControlTrainLights([DispatchFilter(Type = "XPath", Value = "ForEachTrain")] XmlElement element) => EventScriptUImanager.GetEventOrEventContainerDescription(element, LayoutEventScriptEditorTreeNodeForEachTrain.GetDescription(element));

        [DispatchTarget]
        private void EditEventScriptElement_ControlTrainLights([DispatchFilter("XPath", "ControlTrainLights")] XmlElement element, IEventScriptEditorSite site) {
            var d = new EventScriptDialogs.TrainLightsAction(element);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        private class LayoutEventScriptEditorTreeNodeControlTrainLights : CommonUI.Controls.LayoutEventScriptEditorTreeNodeAction {
            public LayoutEventScriptEditorTreeNodeControlTrainLights(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            public static string GetDescription(XmlElement element) {
                string symbolName = element.GetAttribute(EventScriptTools.A_TrainSymbol);
                string action = element.GetAttribute("Action");
                string verb = "";

                switch (action) {
                    case "Set": verb = ""; break;
                    case "Toggle": verb = " toggle"; break;
                }

                var value = GetOperandDescription(element, "Value", typeof(bool));

                if (string.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
                    value = "On";
                else if (string.Equals(value, "false", StringComparison.OrdinalIgnoreCase))
                    value = "Off";

                return $"Set lights of {symbolName} to{verb} {value}";
            }

            protected override string Description => GetDescription(Element);
        }

        #endregion

        #endregion

        #region Execute Trip-plan

        #region Runtime

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_ExecuteTripPlanNode([DispatchFilter("XPath", "ExecuteTripPlan")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeActionExecuteTripPlan(parseEventInfo);

        private class LayoutEventScriptNodeActionExecuteTripPlan : LayoutEventScriptNodeAction {
            private const string A_TripPlanId = "TripPlanID";
            private const string A_ShouldReverse = "ShouldReverse";

            public LayoutEventScriptNodeActionExecuteTripPlan(LayoutParseEventScript parseEventScript) : base(parseEventScript) {
            }

            public override void Execute() {
                var tripPlanID = (Guid)Element.AttributeValue(A_TripPlanId);
                var shouldReverse = (bool)Element.AttributeValue(A_ShouldReverse);
                var symbolName = Element.GetAttribute(A_TrainSymbol);
                var oTrain = Context[symbolName];

                if (oTrain == null || oTrain is not TrainStateInfo)
                    throw new ArgumentException($"Symbol {symbolName} is not a reference to a valid train object");

                var train = (TrainStateInfo)oTrain;
                var catalogTripPlan = LayoutModel.StateManager.TripPlansCatalog.TripPlans[tripPlanID];

                if (catalogTripPlan == null)
                    throw new LayoutEventScriptException(this, "The trip plan to execute cannot be found. It was probably erased");
                else {
                    XmlDocument workingDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();
                    XmlElement tripPlanElement = (XmlElement)workingDoc.ImportNode(catalogTripPlan.Element, true);

                    workingDoc.AppendChild(tripPlanElement);
                    TripPlanInfo tripPlan = new(tripPlanElement);

                    if (shouldReverse)
                        tripPlan.Reverse();

                    Dispatch.Call.ExecuteTripPlan(new TripPlanAssignmentInfo(tripPlan, train));
                }
            }
        }

#endregion

#region Editing

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_ExecuteTripPlanNode([DispatchFilter("XPath", "ExecuteTripPlan")] XmlElement element) => new LayoutEventScriptEditorTreeNodeExecuteTripPlan(element);

        [DispatchTarget]
        private string GetEventScriptDescription_ExecuteTripPlanNode([DispatchFilter(Type = "XPath", Value = "ExecuteTripPlan")] XmlElement element) => EventScriptUImanager.GetEventOrEventContainerDescription(element, LayoutEventScriptEditorTreeNodeExecuteTripPlan.GetDescription(element));


        [DispatchTarget]
        private void EditEventScriptElement_ExecuteTripPlanNode([DispatchFilter("XPath", "ExecuteTripPlan")] XmlElement element, IEventScriptEditorSite site) {
            EventScriptDialogs.ExecuteTripPlan d = new(element);

            new SemiModalDialog(site.Form, d, new SemiModalDialogClosedHandler(OnCloseExecuteTripPlan), site).ShowDialog();
        }

        private void OnCloseExecuteTripPlan(Form dialog, object? info) {
            var site = Ensure.NotNull<IEventScriptEditorSite>(info);

            if (dialog.DialogResult == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        private class LayoutEventScriptEditorTreeNodeExecuteTripPlan : CommonUI.Controls.LayoutEventScriptEditorTreeNodeAction {
            private const string A_TripPlanId = "TripPlanID";
            private const string A_ShouldReverse = "ShouldReverse";

            public LayoutEventScriptEditorTreeNodeExecuteTripPlan(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            public static string GetDescription(XmlElement element) {
                var symbolName = element.GetAttribute(EventScriptTools.A_TrainSymbol);
                var tripPlanID = (Guid)element.AttributeValue(A_TripPlanId);
                var shouldReverse = (bool)element.AttributeValue(A_ShouldReverse);
                var tripPlanName = "[Error: UNKNOWN]";

                var tripPlan = LayoutModel.StateManager.TripPlansCatalog.TripPlans[tripPlanID];

                if (tripPlan != null)
                    tripPlanName = tripPlan.Name;

                return $"{symbolName} executes trip plan {tripPlanName}{(shouldReverse ? " (reversed)" : "")}";
            }

            protected override string Description => GetDescription(Element);

            public override bool SupportedInGlobalPolicy => false;
        }

        #endregion

        #endregion

        #region ExecuteRandomTripPlan

        #region Runtime

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_ExecuteRandomTripPlan([DispatchFilter("XPath", "ExecuteRandomTripPlan")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeActionExecuteRandomTripPlan(parseEventInfo, this);

        [LayoutEventDef("no-applicable-trip-plans-notification", Role = LayoutEventRole.Notification, SenderType = typeof(TrainStateInfo))]
        private class LayoutEventScriptNodeActionExecuteRandomTripPlan : LayoutEventScriptNodeAction {
            private static readonly LayoutTraceSwitch traceRandomTripPlan = new("ExecuteRandomTripPlan", "Execute Random Trip-plan");
            private const string A_StaticGrade = "StaticGrade";
            private const string A_ShouldReverse = "ShouldReverse";
            private const string A_Symbol = "Symbol";
            private const string E_ApplicableTripPlans = "ApplicableTripPlans";
            private const string A_SelectCircularTripPlans = "SelectCircularTripPlans";
            private const string A_ReversedTripPlanSelection = "ReversedTripPlanSelection";
            private const string A_Penalty = "Penalty";
            private const string A_TripPlanId = "TripPlanID";
            private const string A_ClearanceQuality = "ClearanceQuality";
            private const string E_Filter = "Filter";
            private readonly LayoutModuleBase mb;

            public LayoutEventScriptNodeActionExecuteRandomTripPlan(LayoutParseEventScript parseEventScript, LayoutModuleBase mb) : base(parseEventScript) {
                this.mb = mb;
            }

            public override void Execute() {
                var symbol = Element.GetAttribute(A_Symbol);
                if (Context[symbol] is not TrainStateInfo train)
                    throw new ArgumentException($"Symbol {symbol} is not a reference to a valid train object");

                var workingDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();
                var applicableTripPlansElement = workingDoc.CreateElement(E_ApplicableTripPlans);
                var selectCircular = (bool)Element.AttributeValue(A_SelectCircularTripPlans);
                string selectReversedMethod = Element.GetAttribute(A_ReversedTripPlanSelection);
                var tripPlanCandidates = new List<XmlElement>();
                var reversedTripPlanCandidates = new List<XmlElement>();
                var filter = new LayoutConditionScript("Execute Random Trip-plan Filter", Ensure.NotNull<XmlElement>(Element[E_Filter]), true);
                var scriptContext = filter.ScriptContext;

                Dispatch.Call.SetScriptContext(train, scriptContext);

                workingDoc.AppendChild(applicableTripPlansElement);
                applicableTripPlansElement.SetAttributeValue(A_StaticGrade, false);

                EventManager.Event(new LayoutEvent("get-applicable-trip-plans-request", train, applicableTripPlansElement));

                if (traceRandomTripPlan.TraceVerbose) {
                    Trace.WriteLine("*** Execute Random trip plan ***");
                    Trace.WriteLine("Applicable trip plan");
                    Trace.WriteLine(applicableTripPlansElement.OuterXml);
                }

                foreach (XmlElement applicableTripPlanElement in applicableTripPlansElement) {
                    var tripPlan = LayoutModel.StateManager.TripPlansCatalog.TripPlans[(Guid)applicableTripPlanElement.AttributeValue(A_TripPlanId)];
                    var penalty = (int)applicableTripPlanElement.AttributeValue(A_Penalty);
                    var clearanceQuality = applicableTripPlanElement.AttributeValue(A_ClearanceQuality).Enum<RouteClearanceQuality>() ?? throw new ArgumentException("clearanceQuality");
                    RouteQuality routeQuality = new(train.Id, clearanceQuality, penalty);

                    if (tripPlan != null && routeQuality.IsFree) {
                        if (!tripPlan.IsCircular || selectCircular) {
                            Dispatch.Call.SetScriptContext(tripPlan, scriptContext);

                            Application.DoEvents();

                            if (filter.IsTrue) {
                                bool destinationIsFree = (bool)(EventManager.Event(new LayoutEvent("check-trip-plan-destination-request", tripPlan)) ?? false);

                                if ((bool)applicableTripPlanElement.AttributeValue(A_ShouldReverse) || !destinationIsFree)
                                    reversedTripPlanCandidates.Add(applicableTripPlanElement);
                                else
                                    tripPlanCandidates.Add(applicableTripPlanElement);
                            }
                        }
                    }
                }

                int upperLimit = -1;

                switch (selectReversedMethod) {
                    case "Yes":
                        upperLimit = tripPlanCandidates.Count + reversedTripPlanCandidates.Count;
                        break;

                    case "No":
                        upperLimit = tripPlanCandidates.Count;
                        break;

                    case "IfNoAlternative":
                        if (tripPlanCandidates.Count > 0)
                            upperLimit = tripPlanCandidates.Count;
                        else
                            upperLimit = reversedTripPlanCandidates.Count;
                        break;
                }

                if (upperLimit == 0)
                    EventManager.Event(new LayoutEvent("no-applicable-trip-plans-notification", train));
                else {
                    int index = new Random().Next(upperLimit);
                    XmlElement? selectedApplicableTripPlanElement;

                    if (tripPlanCandidates.Count == 0 && selectReversedMethod != "Yes")
                        LayoutModuleBase.Message(train, "No optimal trip-plan are available for selection by 'execute random trip-plan'");

                    if (index >= tripPlanCandidates.Count)
                        selectedApplicableTripPlanElement = (XmlElement)reversedTripPlanCandidates[index - tripPlanCandidates.Count];
                    else
                        selectedApplicableTripPlanElement = (XmlElement)tripPlanCandidates[index];

                    if (selectedApplicableTripPlanElement != null) {
                        var selectedTripPlan = LayoutModel.StateManager.TripPlansCatalog.TripPlans[(Guid)selectedApplicableTripPlanElement.AttributeValue(A_TripPlanId)];
                        var shouldReverse = (bool)selectedApplicableTripPlanElement.AttributeValue(A_ShouldReverse);

                        if (selectedTripPlan != null) {
                            if (traceRandomTripPlan.TraceInfo) {
                                Trace.WriteLine("*** Selected trip: " + selectedApplicableTripPlanElement.OuterXml);
                                selectedTripPlan.Dump();
                            }

                            // Now submit the trip plan
                            XmlDocument tripPlanDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();
                            XmlElement tripPlanElement = (XmlElement)tripPlanDoc.ImportNode(selectedTripPlan.Element, true);

                            tripPlanDoc.AppendChild(tripPlanElement);
                            TripPlanInfo tripPlan = new(tripPlanElement);

                            if (shouldReverse)
                                tripPlan.Reverse();

                            Dispatch.Call.ExecuteTripPlan(new TripPlanAssignmentInfo(tripPlan, train));
                        }
                    }
                }
            }
        }

#endregion

#region Editing

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_ExecuteRandomTripPlan([DispatchFilter("XPath", "ExecuteRandomTripPlan")] XmlElement element) => new LayoutEventScriptEditorTreeNodeExecuteRandomTripPlan(element);

        [DispatchTarget]
        private string GetEventScriptDescription_ExecuteRandomTripPlan([DispatchFilter(Type = "XPath", Value = "ExecuteRandomTripPlan")] XmlElement element) => EventScriptUImanager.GetEventOrEventContainerDescription(element, LayoutEventScriptEditorTreeNodeExecuteRandomTripPlan.GetDescription(element));

        [DispatchTarget]
        private void EditEventScriptElement_ExecuteRandomTripPlan([DispatchFilter("XPath", "ExecuteRandomTripPlan")] XmlElement element, IEventScriptEditorSite site) {
            var d = new EventScriptDialogs.ExecuteRandomTripPlan(element);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        private class LayoutEventScriptEditorTreeNodeExecuteRandomTripPlan : CommonUI.Controls.LayoutEventScriptEditorTreeNodeAction {
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
