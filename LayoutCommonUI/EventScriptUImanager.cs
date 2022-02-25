using LayoutManager.CommonUI.Controls;
using MethodDispatcher;
using System.Xml;

namespace LayoutManager.CommonUI {
    [LayoutModule("Event Script UI Manager", UserControl = false)]
    public class EventScriptUImanager : LayoutModuleBase {
        private const string A_Choice = "Choice";
        private const string A_Weight = "Weight";

        #region Methods to return add menus for various section (events, condition and actions)

        //[LayoutEvent("get-event-script-editor-event-container-menu")]
        [DispatchTarget]
        private void GetEventScriptEditorEventContainerMenu(LayoutEventScriptEditorTreeNode eventContainerNode, bool hasMenuDefinition, MenuOrMenuItem menu) {
            if (eventContainerNode.Element["Events"] == null)
                menu.Items.Add(new  EventScriptEditorAddMenuItem(eventContainerNode, "Events", "Events"));
            if (eventContainerNode.SupportConditions && eventContainerNode.Element["Condition"] == null)
                menu.Items.Add(new EventScriptEditorAddMenuItem(eventContainerNode, "Condition", "Condition"));
            if (eventContainerNode.SupportActions && eventContainerNode.Element["Actions"] == null)
                menu.Items.Add(new EventScriptEditorAddMenuItem(eventContainerNode, "Actions", "Actions"));
        }

        //[LayoutEvent("get-event-script-editor-insert-event-container-menu")]
        [DispatchTarget]
        private void GetEventScriptEditorInsertEventConatinerMenu(LayoutEventScriptEditorTreeNode node, bool hasBlockDefinition, MenuOrMenuItem menu) {
            menu.Items.Add(new EventScriptEditorInsertEventContainerMenuItem(node, "Any", "Any"));
            menu.Items.Add(new EventScriptEditorInsertEventContainerMenuItem(node, "All", "All"));
            menu.Items.Add(new EventScriptEditorInsertEventContainerMenuItem(node, "Sequence", "Sequence"));
            menu.Items.Add(new EventScriptEditorInsertEventContainerMenuItem(node, "Repeat", "Repeat"));
            menu.Items.Add(new EventScriptEditorInsertEventContainerMenuItem(node, "Task", "Task"));
        }

        //[LayoutEvent("get-event-script-editor-event-menu")]
        [DispatchTarget]
        private void GetEventScriptEditorEventMenu(LayoutEventScriptEditorTreeNode eventNode, bool hasBlockDefinition, MenuOrMenuItem menu) {
            if (eventNode.SupportConditions && eventNode.Element["Condition"] == null)
                menu.Items.Add(new EventScriptEditorAddMenuItem(eventNode, "Condition", "Condition"));
            if (eventNode.SupportActions && eventNode.Element["Actions"] == null)
                menu.Items.Add(new EventScriptEditorAddMenuItem(eventNode, "Actions", "Actions"));
        }

        //[LayoutEvent("get-event-script-editor-events-section-menu", Order = 0)]
        [DispatchTarget(Order = 0)]
        private void GetEventScriptEditorEventSectionMenu_Common(LayoutEventScriptEditorTreeNode eventNode, bool hasBlockDefinition, MenuOrMenuItem menu) {
            menu.Items.Add(new EventScriptEditorAddMenuItem(eventNode, "Any", "Any"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(eventNode, "All", "All"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(eventNode, "Sequence", "Sequence"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(eventNode, "Repeat", "Repeat"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(eventNode, "Random Choice", "RandomChoice"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(eventNode, "Task", "Task"));
        }

        //[LayoutEvent("get-event-script-editor-events-section-menu", Order = 100)]
        [DispatchTarget(Order = 100)]
        private void GetEventScriptEditorEventSectionMenu_WithEvents(LayoutEventScriptEditorTreeNode eventNode, bool hasBlockDefinition, MenuOrMenuItem menu) {
            if (menu.Items.Count > 0)
                menu.Items.Add(new ToolStripSeparator());

            menu.Items.Add(new EventScriptEditorAddMenuItem(eventNode, "Do now", "DoNow"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(eventNode, "Wait", "Wait"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(eventNode, "Wait for Event", "WaitForEvent"));
        }

        //[LayoutEvent("get-event-script-editor-random-choice-menu")]
        [DispatchTarget]
        private void GetEventScriptEditorRandomChoiceMenu(LayoutEventScriptEditorTreeNode eventNode, bool hasBlockDefinition, MenuOrMenuItem menu) {
            menu.Items.Add(new EventScriptEditorAddMenuItem(eventNode, "Choice", "Choice"));
        }

        //[LayoutEvent("get-event-script-editor-condition-section-menu")]
        [DispatchTarget]
        private void GetEventScriptEditorConditionSectionMenu(LayoutEventScriptEditorTreeNode eventNode, bool hasBlockDefinition, MenuOrMenuItem menu) {
            menu.Items.Add(new EventScriptEditorAddMenuItem(eventNode, "And", "And"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(eventNode, "Or", "Or"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(eventNode, "Not", "Not"));
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(new EventScriptEditorAddMenuItem(eventNode, "If (String)", "IfString"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(eventNode, "If (Number)", "IfNumber"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(eventNode, "If (Boolean)", "IfBoolean"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(eventNode, "If (Defined)", "IfDefined"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(eventNode, "If (Time)", "IfTime"));
        }

        //[LayoutEvent("get-event-script-editor-insert-condition-container-menu")]
        [DispatchTarget]
        private void GetEventScriptEditorInsertConditionContainerMenu(LayoutEventScriptEditorTreeNode eventNode, bool hasBlockDefinition, MenuOrMenuItem menu) {
            menu.Items.Add(new EventScriptEditorInsertConditionContainerMenuItem(eventNode, "And", "And"));
            menu.Items.Add(new EventScriptEditorInsertConditionContainerMenuItem(eventNode, "Or", "Or"));
            menu.Items.Add(new EventScriptEditorInsertConditionContainerMenuItem(eventNode, "Not", "Not"));
        }

        //[LayoutEvent("get-event-script-editor-actions-section-menu")]
        [DispatchTarget]
        private void GetEventScriptEditoActionsSectionMenu(LayoutEventScriptEditorTreeNode eventNode, bool hasBlockDefinition, MenuOrMenuItem menu) {
            menu.Items.Add(new EventScriptEditorAddMenuItem(eventNode, "Show message", "ShowMessage"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(eventNode, "Set Attribute", "SetAttribute"));
        }

        #endregion

        #region Utility methods for generating descriptions

        public static bool IsTrivialContainer(XmlElement containerElement) {
            var eventsElement = containerElement["Events"];

            return eventsElement != null && eventsElement.ChildNodes.Count == 1 && containerElement["Condition"] == null && containerElement["Actions"] == null;
        }

        public static string GetElementDescription(XmlElement element) => Dispatch.Call.GetEventScriptDescription(element) ?? $"({element.Name})";

        public static string GetActionsDescription(XmlElement actionsElement) {
            string[] actions = new string[actionsElement.ChildNodes.Count];

            int i = 0;
            foreach (XmlElement actionElement in actionsElement) {
                actions[i] = ((i > 0) ? "; " : "") + GetElementDescription(actionElement);
                i++;
            }

            return String.Concat(actions);
        }

        public static string GetEventOrEventContainerDescription(XmlElement element, string prefix, string? suffix) {
            var substrings = new List<string>();

            if (IsTrivialContainer(element)) {
                var childElement = (XmlElement?)element.ChildNodes[0]?.ChildNodes[0];
                return childElement != null ? (string)GetElementDescription(childElement) : String.Empty;
            }
            else {
                substrings.Add(prefix);

                foreach (XmlElement childElement in element) {
                    if (childElement.Name == "Events") {
                        substrings.Add(" { ");
                        foreach (XmlElement eventElement in childElement)
                            substrings.Add(GetElementDescription(eventElement));
                        substrings.Add(" } ");
                    }
                    else if (childElement.Name == "Condition") {
                        if (substrings.Count > 0)
                            substrings.Add(",");
                        XmlElement? grandChildElement = (XmlElement?)childElement.ChildNodes[0];
                        substrings.Add(" Condition { " + (grandChildElement != null ? GetElementDescription(grandChildElement) : String.Empty) + " }");
                    }
                    else if (childElement.Name == "Actions") {
                        if (substrings.Count > 0)
                            substrings.Add(",");
                        substrings.Add(" Actions { " + GetActionsDescription(childElement) + " } ");
                    }
                    else
                        substrings.Add(GetElementDescription(childElement));
                }

                if (suffix != null)
                    substrings.Add(suffix);

                return String.Concat(substrings);
            }
        }

        public static string GetEventOrEventContainerDescription(XmlElement element, string prefix) => GetEventOrEventContainerDescription(element, prefix, null);

        public static string GetConditionContainerDescription(XmlElement containerElement, string containerName) {
            if (containerElement.ChildNodes.Count == 1)     // If there is only one some condition, return its description
                return GetElementDescription((XmlElement)containerElement.ChildNodes[0]!);
            else {
                string[] conditions = new string[containerElement.ChildNodes.Count];

                int i = 0;
                foreach (XmlElement conditionElement in containerElement) {
                    conditions[i] = ((i == 0) ? "" : " " + containerName + " ") + GetElementDescription(conditionElement);
                    i++;
                }

                return $"({string.Concat(conditions)})";
            }
        }

        #endregion

        #region Tree nodes for event container sections


        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_Events([DispatchFilter("XPath", "Events")] XmlElement element) => new LayoutEventScriptEditorTreeNodeEventsSection(element);

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_Condition([DispatchFilter("XPath", "Condition")] XmlElement element) => new LayoutEventScriptEditorTreeNodeConditionSection(element);

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_Actions([DispatchFilter("XPath", "Actions")] XmlElement element) => new LayoutEventScriptEditorTreeNodeActionsSection(element);

        #endregion

        #region Event Containers

        #region Sequence

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_Sequence([DispatchFilter("XPath", "Sequence")] XmlElement element) => new LayoutEventScriptEditorTreeNodeSequence(element);

        [DispatchTarget]
        private string GetEventScriptDescription_Sequence([DispatchFilter("XPatt", "Sequence")] XmlElement element) => GetEventOrEventContainerDescription(element, "Sequence");

        private class LayoutEventScriptEditorTreeNodeSequence : LayoutEventScriptEditorTreeNodeEventContainer {
            public LayoutEventScriptEditorTreeNodeSequence(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            protected override string Description => "Sequence";

            public override bool ShouldInsertAsFirst(XmlElement elementToInsert) {
                return elementToInsert.Name == "Condition";
            }
        }

        #endregion

        #region All

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_All([DispatchFilter("XPath", "All")] XmlElement element) => new LayoutEventScriptEditorTreeNodeAll(element);

        [DispatchTarget]
        private string GetEventScriptDescription_All([DispatchFilter("XPath", "All")] XmlElement element) => GetEventOrEventContainerDescription(element, "All");

        private class LayoutEventScriptEditorTreeNodeAll : LayoutEventScriptEditorTreeNodeEventContainer {
            public LayoutEventScriptEditorTreeNodeAll(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            protected override string Description => "All";

            public override bool CanHaveOptionalSubItems => true;
        }

        #endregion

        #region Any

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_Any([DispatchFilter("XPath", "Any")] XmlElement element) => new LayoutEventScriptEditorTreeNodeAny(element);

        [DispatchTarget]
        private string GetEventScriptDescription_Any([DispatchFilter("XPath", "Any")] XmlElement element) => GetEventOrEventContainerDescription(element, "Any");

        private class LayoutEventScriptEditorTreeNodeAny : LayoutEventScriptEditorTreeNodeEventContainer {
            public LayoutEventScriptEditorTreeNodeAny(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            protected override string Description => "Any";

            public override bool CanHaveOptionalSubItems => true;
        }

        #endregion

        #region Repeat

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_Repeat([DispatchFilter("XPath", "Repeat")] XmlElement element) => new LayoutEventScriptEditorTreeNodeRepeat(element);

        [DispatchTarget]
        private string GetEventScriptDescription_Repeat([DispatchFilter("XPath", "Repeat")] XmlElement element) => GetEventOrEventContainerDescription(element, "Repeat");

        [DispatchTarget]
        private void EditEventScriptElement_Repeat([DispatchFilter("XPath", "Repeat")] XmlElement element, IEventScriptEditorSite site) {
            Controls.EventScriptEditorDialogs.RepeatCondition repeatDialog = new(element);

            if (repeatDialog.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        private class LayoutEventScriptEditorTreeNodeRepeat : LayoutEventScriptEditorTreeNodeMayBeOptional {
            private const string A_Count = "Count";

            public LayoutEventScriptEditorTreeNodeRepeat(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            public override Action<LayoutEventScriptEditorTreeNode, bool, MenuOrMenuItem> AddNodeMenuFunction => Dispatch.Call.GetEventScriptEditorEventSectionMenu;

            public override Action<LayoutEventScriptEditorTreeNode, bool, MenuOrMenuItem> InsertNodeMenuFunction => Dispatch.Call.GetEventScriptEditorInsertEventConatinerMenu;

            protected override int IconIndex => IconEvent;

            static public string GetDescription(XmlElement element) => (int)element.AttributeValue(A_Count) switch {
                int c when c < 0 => "Repeat",
                1 => "Repeat once",
                int c => $"Repeat {c} time"
            };

            protected override string Description => GetDescription(Element);

            /// <summary>
            /// Repeat can have no more than one subcondition
            /// </summary>
            public override int MaxSubNodes => 1;

            public override LayoutEventScriptEditorTreeNode? NodeToEdit => this;        // repeat rule can be edited (changing the count)

            public override bool ShouldInsertAsFirst(XmlElement elementToInsert) {
                return elementToInsert.Name == "Condition";
            }
        }

        #endregion

        #region Random Choice

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_RandomChoice([DispatchFilter("XPath", "RandomChoice")] XmlElement element) => new LayoutEventScriptEditorTreeNodeRandomChoice(element);

        [DispatchTarget]
        private string GetEventScriptDescription_RandomChoice([DispatchFilter("XPath", "RandomChoice")] XmlElement element) {
            var substrings = new List<string> {
                "Random-Choice [ "
            };

            foreach (XmlElement choiceElement in element.GetElementsByTagName(A_Choice)) {
                substrings.Add($"Choice ({(int)choiceElement.AttributeValue(A_Weight)}): {{ {GetElementDescription((XmlElement)choiceElement.ChildNodes[0]!)} }} ");
            }

            substrings.Add("] ");

            return string.Concat(substrings);
        }

        private class LayoutEventScriptEditorTreeNodeRandomChoice : LayoutEventScriptEditorTreeNodeMayBeOptional {
            public LayoutEventScriptEditorTreeNodeRandomChoice(XmlElement element) : base(element) {
                AddChildEventScriptTreeNodes();
            }

            public override Action<LayoutEventScriptEditorTreeNode, bool, MenuOrMenuItem> AddNodeMenuFunction => Dispatch.Call.GetEventScriptEditorEventContainerMenu;

            protected override string Description => "Random-Choice";

            protected override int IconIndex => IconEvent;

            public override LayoutEventScriptEditorTreeNode? NodeToEdit => null;
        }

        #endregion

        #region Choice Entry

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_Choice([DispatchFilter("XPath", "Choice")] XmlElement element) => new LayoutEventScriptEditorTreeNodeRandomChoiceEntry(element);

        [DispatchTarget]
        private void EditEventScriptElement_Choice([DispatchFilter("XPath", "Choice")] XmlElement element, IEventScriptEditorSite site) {
            var weightText = CommonUI.Dialogs.InputBox.Show("Choice Weight", "Please enter the weight (probability) for this choice", CommonUI.Dialogs.InputBoxValidationOptions.IntegerNumber);

            if (weightText == null)
                site.EditingCancelled();
            else {
                site.EditingDone();
            }
        }

        private class LayoutEventScriptEditorTreeNodeRandomChoiceEntry : LayoutEventScriptEditorTreeNode {
            public LayoutEventScriptEditorTreeNodeRandomChoiceEntry(XmlElement element) : base(element) {
                AddChildEventScriptTreeNodes();
            }

            public override Action<LayoutEventScriptEditorTreeNode, bool, MenuOrMenuItem> AddNodeMenuFunction => Dispatch.Call.GetEventScriptEditorEventSectionMenu;

            protected override string Description => "Choice (" + Element.GetAttribute(EventScriptUImanager.A_Weight) + ")";

            protected override int IconIndex => IconEvent;

            public override int MinSubNodes => 1;

            public override int MaxSubNodes => 1;

            public override LayoutEventScriptEditorTreeNode? NodeToEdit => this;        // Choice can be edited
        }

        #endregion

        #region Task

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_Task([DispatchFilter("XPath", "Task")] XmlElement element) => new LayoutEventScriptEditorTreeNodeTask(element);

        [DispatchTarget]
        private void EditEventScriptElement_Task([DispatchFilter("XPath", "Task")] XmlElement element, IEventScriptEditorSite site) {
            //			Controls.EventScriptEditorDialogs.RepeatCondition	repeatDialog = new Controls.EventScriptEditorDialogs.RepeatCondition((XmlElement)e.Sender);

            //			if(repeatDialog.ShowDialog() == DialogResult.OK)
            //				site.EditingDone();
            //			else
            //				site.EditingCancelled();
            site.EditingDone();
        }

        [DispatchTarget]
        private string GetEventScriptDescription_Task([DispatchFilter(Type = "XPath", Value = "Task")] XmlElement element) {
            var taskElement = (XmlElement)element.ChildNodes[0]!;

            return GetEventOrEventContainerDescription(element, LayoutEventScriptEditorTreeNodeTask.GetDescription(taskElement) + " { ", " } ");
        }

        private class LayoutEventScriptEditorTreeNodeTask : LayoutEventScriptEditorTreeNode {
            public LayoutEventScriptEditorTreeNodeTask(XmlElement scriptElement) : base(scriptElement) {
                AddChildEventScriptTreeNodes();
            }

            public override Action<LayoutEventScriptEditorTreeNode, bool, MenuOrMenuItem> AddNodeMenuFunction => Dispatch.Call.GetEventScriptEditorEventSectionMenu;

            public override Action<LayoutEventScriptEditorTreeNode, bool, MenuOrMenuItem> InsertNodeMenuFunction => Dispatch.Call.GetEventScriptEditorInsertEventConatinerMenu;

            protected override int IconIndex => IconEvent;

            static public string GetDescription(XmlElement element) => "Task";

            protected override string Description => GetDescription(Element);

            /// <summary>
            /// Repeat can have no more than one sub-condition
            /// </summary>
            public override int MaxSubNodes => 1;

            public override LayoutEventScriptEditorTreeNode? NodeToEdit => this;        // repeat rule can be edited (changing the count)

            public override bool ShouldInsertAsFirst(XmlElement elementToInsert) {
                return elementToInsert.Name == "Condition";
            }
        }

        #endregion

        #endregion

        #region Events

        #region Wait

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_Wait([DispatchFilter("XPath", "Wait")] XmlElement element) => new LayoutEventScriptEditorTreeNodeWait(element);

        [DispatchTarget]
        private string GetEventScriptDescription_Wait([DispatchFilter("XPath", "Wait")] XmlElement element) => GetEventOrEventContainerDescription(element, LayoutEventScriptEditorTreeNodeWait.GetWaitDescription(element));

        [DispatchTarget]
        private void EditEventScriptElement_Wait([DispatchFilter("XPath", "Wait")] XmlElement element, IEventScriptEditorSite site) {
            Controls.EventScriptEditorDialogs.WaitCondition waitDialog = new(element);

            if (waitDialog.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        private class LayoutEventScriptEditorTreeNodeWait : LayoutEventScriptEditorTreeNodeEvent {
            private const string A_RandomSeconds = "RandomSeconds";
            private const string A_Minutes = "Minutes";
            private const string A_Seconds = "Seconds";
            private const string A_MilliSeconds = "MilliSeconds";

            public LayoutEventScriptEditorTreeNodeWait(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            private static void AddWaitString(XmlElement conditionElement, List<string> list, string attrName, string unitSingle, string unitPlural) {
                if (conditionElement.HasAttribute(attrName)) {
                    var v = (int)conditionElement.AttributeValue(attrName);

                    if (v != 0)
                        list.Add((list.Count != 0 ? ", " : "") + v.ToString() + " " + ((v == 1) ? unitSingle : unitPlural));
                }
            }

            public static string GetWaitDescription(XmlElement conditionElement) {
                var delayStrings = new List<string>();
                string s;

                AddWaitString(conditionElement, delayStrings, A_Minutes, "minute", "minutes");
                AddWaitString(conditionElement, delayStrings, A_Seconds, "second", "seconds");
                AddWaitString(conditionElement, delayStrings, A_MilliSeconds, "milli-second", "milli-seconds");

                s = "Wait for " + String.Concat(delayStrings);

                if (conditionElement.HasAttribute(A_RandomSeconds))
                    s += " plus random time upto " + (int)conditionElement.AttributeValue(A_RandomSeconds) + " seconds";

                return s;
            }

            protected override string Description => GetWaitDescription(Element);
        }

        #endregion

        #region DoNow

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_DoNow([DispatchFilter("XPath", "DoNow")] XmlElement element) => new LayoutEventScriptEditorTreeNodeDoNow(element);

        [DispatchTarget]
        private string GetEventScriptDescription_DoNow([DispatchFilter("XPath", "DoNow")] XmlElement element) => GetEventOrEventContainerDescription(element, LayoutEventScriptEditorTreeNodeDoNow.GetDescription(element));

        private class LayoutEventScriptEditorTreeNodeDoNow : LayoutEventScriptEditorTreeNodeEvent {
            public LayoutEventScriptEditorTreeNodeDoNow(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            public static string GetDescription(XmlElement element) => "Do now";

            public override LayoutEventScriptEditorTreeNode? NodeToEdit => null;		// Nothing to edit

            protected override string Description => GetDescription(Element);
        }

        #endregion

        #region Wait for event

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_WaitForEvent([DispatchFilter("XPath", "WaitForEvent")] XmlElement element) => new LayoutEventScriptEditorTreeNodeWaitForEvent(element);

        [DispatchTarget]
        private string GetEventScriptDescription_WaitForEvent([DispatchFilter("XPath", "WaitForEvent")] XmlElement element) => GetEventOrEventContainerDescription(element, LayoutEventScriptEditorTreeNodeWaitForEvent.GetDescription(element));

        [DispatchTarget]
        private void EditEventScriptElement_WaitForEvent([DispatchFilter("XPath", "WaitForEvent")] XmlElement element, IEventScriptEditorSite site) {
            Controls.EventScriptEditorDialogs.WaitForEvent d = new(element);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }


        private class LayoutEventScriptEditorTreeNodeWaitForEvent : LayoutEventScriptEditorTreeNodeEvent {
            public LayoutEventScriptEditorTreeNodeWaitForEvent(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            public static string GetDescription(XmlElement element) {
                string name = element.GetAttribute("Name");

                return "Wait for event " + name;
            }

            protected override string Description => GetDescription(Element);
        }

        #endregion

        #endregion

        #region Condition Containers

        #region And

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_And([DispatchFilter("XPath", "And")] XmlElement element) => new LayoutEventScriptEditorTreeNodeAnd(element);

        [DispatchTarget]
        private string GetEventScriptDescription_And([DispatchFilter("XPath", "And")] XmlElement element) => GetConditionContainerDescription(element, "And");

        private class LayoutEventScriptEditorTreeNodeAnd : LayoutEventScriptEditorTreeNodeConditionContainer {
            public LayoutEventScriptEditorTreeNodeAnd(XmlElement andElement) : base(andElement) {
                AddChildEventScriptTreeNodes();
            }

            protected override string Description => "And";
        }

        #endregion

        #region Or

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_Or([DispatchFilter("XPath", "Or")] XmlElement element) => new LayoutEventScriptEditorTreeNodeOr(element);

        [DispatchTarget]
        private string GetEventScriptDescription_Or([DispatchFilter("XPath", "Or")] XmlElement element) => GetConditionContainerDescription(element, "Or");

        private class LayoutEventScriptEditorTreeNodeOr : LayoutEventScriptEditorTreeNodeConditionContainer {
            public LayoutEventScriptEditorTreeNodeOr(XmlElement orElement) : base(orElement) {
                AddChildEventScriptTreeNodes();
            }

            protected override string Description => "Or";
        }

        #endregion

        #region Not

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_Not([DispatchFilter("XPath", "Not")] XmlElement element) => new LayoutEventScriptEditnotTreeNodeNot(element);

        [DispatchTarget]
        private string GetEventScriptDescription_Not([DispatchFilter("XPath", "Not")] XmlElement element) => "Not " + GetElementDescription((XmlElement)element.ChildNodes[0]!);

        private class LayoutEventScriptEditnotTreeNodeNot : LayoutEventScriptEditorTreeNodeConditionContainer {
            public LayoutEventScriptEditnotTreeNodeNot(XmlElement notElement) : base(notElement) {
                AddChildEventScriptTreeNodes();
            }

            protected override string Description => "Not";

            public override int MaxSubNodes => 1;
        }

        #endregion

        #endregion

        #region Conditions

        #region Common tree node for If

        abstract public class LayoutEventScriptEditorTreeNodeIf : LayoutEventScriptEditorTreeNodeCondition {
            protected LayoutEventScriptEditorTreeNodeIf(XmlElement conditionElement) : base(conditionElement) {
            }
        }

        #endregion

        #region If (String)

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_IfString([DispatchFilter("XPath", "IfString")] XmlElement element) => new LayoutEventScriptEditorTreeNodeIfString(element);

        [DispatchTarget]
        private string GetEventScriptDescription_IfString([DispatchFilter("XPath", "IfString")] XmlElement element) => LayoutEventScriptEditorTreeNodeIfString.GetDescription(element);

        private class IfStringCustomizer : Controls.EventScriptEditorDialogs.IIfConditionDialogCustomizer {
            public string Title => "If (string)";

            public string[] OperatorNames => new string[] { "Equal", "NotEqual", "Match" };

            public string[] OperatorDescriptions => new string[] { "=", "<>", "Match" };

            public Type[] AllowedTypes => new Type[] {
                                          typeof(string),
                                          typeof(Enum),
                    };

            public bool ValueIsBoolean => false;
        }

        [DispatchTarget]
        private void EditEventScriptElement_IfString([DispatchFilter("XPath", "IfString")] XmlElement element, IEventScriptEditorSite site) {
            Controls.EventScriptEditorDialogs.IfCondition ifDialog = new(element, new IfStringCustomizer());

            if (ifDialog.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }


        public class LayoutEventScriptEditorTreeNodeIfString : LayoutEventScriptEditorTreeNodeIf {
            public LayoutEventScriptEditorTreeNodeIfString(XmlElement ifStringElement) : base(ifStringElement) {
            }

            public static string GetDescription(XmlElement element) {
                string operatorName = Dispatch.Call.GetEventScriptOperatorName(element);

                return "If (string) " + GetOperandDescription(element, "1", typeof(string)) + " " + operatorName + " " + GetOperandDescription(element, "2", typeof(string));
            }

            protected override string Description => GetDescription(Element);
        }

        #endregion

        #region If (Number)

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_IfNumber([DispatchFilter("XPath", "IfNumber")] XmlElement element) => new LayoutEventScriptEditorTreeNodeIfNumber(element);

        [DispatchTarget]
        private string GetEventScriptDescription_IfNumber([DispatchFilter("XPath", "IfNumber")] XmlElement element) => LayoutEventScriptEditorTreeNodeIfNumber.GetDescription(element);

        private class IfNumberCustomizer : Controls.EventScriptEditorDialogs.IIfConditionDialogCustomizer {
            public string Title => "If (number)";

            public string[] OperatorNames => new string[] { "eq", "ne", "gt", "ge", "le", "lt" };

            public string[] OperatorDescriptions => new string[] { "=", "<>", ">", ">=", "=<", "<" };

            public Type[] AllowedTypes => new Type[] {
                                          typeof(int),
                    };

            public bool ValueIsBoolean => false;
        }

        [DispatchTarget]
        private void EditEventScriptElement_IfNumber([DispatchFilter("XPath", "IfNumber")] XmlElement element, IEventScriptEditorSite site) {
            Controls.EventScriptEditorDialogs.IfCondition ifDialog = new(element, new IfNumberCustomizer());

            if (ifDialog.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        public class LayoutEventScriptEditorTreeNodeIfNumber : LayoutEventScriptEditorTreeNodeIf {
            public LayoutEventScriptEditorTreeNodeIfNumber(XmlElement ifStringElement) : base(ifStringElement) {
            }

            public static string GetDescription(XmlElement element) {
                string operatorName = Dispatch.Call.GetEventScriptOperatorName(element);

                return "If (number) " + GetOperandDescription(element, "1", typeof(int)) + " " + operatorName + " " + GetOperandDescription(element, "2", typeof(int));
            }

            protected override string Description => GetDescription(Element);
        }

        #endregion

        #region If (Boolean)

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_IfBoolean([DispatchFilter("XPath", "IfBoolean")] XmlElement element) => new LayoutEventScriptEditorTreeNodeIfBoolean(element);

        [DispatchTarget]
        private string GetEventScriptDescription_IfBoolean([DispatchFilter("XPath", "IfBoolean")] XmlElement element) => LayoutEventScriptEditorTreeNodeIfBoolean.GetDescription(element);

        private class IfBooleanCustomizer : Controls.EventScriptEditorDialogs.IIfConditionDialogCustomizer {
            public string Title => "If (Boolean)";

            public string[] OperatorNames => new string[] { "Equal", "NotEqual" };

            public string[] OperatorDescriptions => new string[] { "=", "<>" };

            public Type[] AllowedTypes => new Type[] {
                                          typeof(bool),
                    };

            public bool ValueIsBoolean => true;
        }

        [DispatchTarget]
        private void EditEventScriptElement_IfBoolean([DispatchFilter("XPath", "IfBoolean")] XmlElement element, IEventScriptEditorSite site) {
            Controls.EventScriptEditorDialogs.IfCondition ifDialog = new(element, new IfBooleanCustomizer());

            if (ifDialog.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        public class LayoutEventScriptEditorTreeNodeIfBoolean : LayoutEventScriptEditorTreeNodeIf {
            public LayoutEventScriptEditorTreeNodeIfBoolean(XmlElement ifBooleanElement) : base(ifBooleanElement) {
            }

            public static string GetDescription(XmlElement element) {
                string operatorName = Dispatch.Call.GetEventScriptOperatorName(element);

                return "If (Boolean) " + GetOperandDescription(element, "1", typeof(bool)) + " " + operatorName + " " + GetOperandDescription(element, "2", typeof(bool));
            }

            protected override string Description => GetDescription(Element);
        }

        #endregion

        #region If (Defined)

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_IfDefined([DispatchFilter("XPath", "IfDefined")] XmlElement element) => new LayoutEventScriptEditorTreeNodeIfDefined(element);

        [DispatchTarget]
        private string GetEventScriptDescription_IfDefined([DispatchFilter("XPath", "IfDefined")] XmlElement element) => LayoutEventScriptEditorTreeNodeIfDefined.GetDescription(element);

        [DispatchTarget]
        private void EditEventScriptElement_IfDefined([DispatchFilter("XPath", "IfDefined")] XmlElement element, IEventScriptEditorSite site) {
            Controls.EventScriptEditorDialogs.IfDefined ifDialog = new(element);

            if (ifDialog.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }


        public class LayoutEventScriptEditorTreeNodeIfDefined : LayoutEventScriptEditorTreeNodeIf {
            public LayoutEventScriptEditorTreeNodeIfDefined(XmlElement ifElement) : base(ifElement) {
            }

            public static string GetDescription(XmlElement element) {
                string symbol = element.GetAttribute("Symbol");

                if (element.HasAttribute("Attribute")) {
                    string attribute = element.GetAttribute("Attribute");

                    return "If defined attribute " + attribute + " of " + symbol;
                }
                else
                    return "If defined symbol " + symbol;
            }

            protected override string Description => GetDescription(Element);
        }

        #endregion

        #region If (Time)

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_IfTime([DispatchFilter("XPath", "IfTime")] XmlElement element) => new LayoutEventScriptEditorTreeNodeIfTime(element);

        [DispatchTarget]
        private string GetEventScriptDescription_IfTime([DispatchFilter("XPath", "IfTime")] XmlElement element) => LayoutEventScriptEditorTreeNodeIfTime.GetDescription(element);

        [DispatchTarget]
        private void EditEventScriptElement_IfTime([DispatchFilter("XPath", "IfTime")] XmlElement element, IEventScriptEditorSite site) {
            Controls.EventScriptEditorDialogs.IfTime ifDialog = new(element);

            if (ifDialog.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }


        public class LayoutEventScriptEditorTreeNodeIfTime : LayoutEventScriptEditorTreeNodeCondition {
            public LayoutEventScriptEditorTreeNodeIfTime(XmlElement conditionElement) : base(conditionElement) {
            }

            private static string? GetNodesDescription(XmlElement element, string nodeName, string title) {
                var nodes = Dispatch.Call.ParseIfTimeElement(element, nodeName);
                string? d = null;

                if (nodes != null && nodes.Length > 0) {
                    bool first = true;

                    d = title + " ";
                    foreach (IIfTimeNode node in nodes) {
                        if (!first)
                            d += " or ";
                        d += node.Description;
                        first = false;
                    }
                }

                return d;
            }

            public static string GetDescription(XmlElement element) {
                var parts = new List<string>();
                string? s;

                if ((s = GetNodesDescription(element, "DayOfWeek", "day of week is")) != null)
                    parts.Add(s);
                if ((s = GetNodesDescription(element, "Hours", "hour is")) != null)
                    parts.Add((parts.Count > 0 ? " and " : "") + s);
                if ((s = GetNodesDescription(element, "Minutes", "minute is")) != null)
                    parts.Add((parts.Count > 0 ? " and " : "") + s);
                if ((s = GetNodesDescription(element, "Seconds", "second is")) != null)
                    parts.Add((parts.Count > 0 ? " and " : "") + s);

                return string.Concat(parts);
            }

            protected override string Description => GetDescription(Element);
        }

        #endregion

        #endregion

        #region Actions

        #region Show Message

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_ShowMessage([DispatchFilter("XPath", "ShowMessage")] XmlElement element) => new LayoutEventScriptEditorTreeNodeShowMessage(element);

        [DispatchTarget]
        private string GetEventScriptDescription_ShowMessage([DispatchFilter("XPath", "ShowMessage")] XmlElement element) => LayoutEventScriptEditorTreeNodeShowMessage.GetDescription(element);

        [DispatchTarget]
        private void EditEventScriptElement_ShowMessage([DispatchFilter("XPath", "ShowMessage")] XmlElement element, IEventScriptEditorSite site) {
            Controls.EventScriptEditorDialogs.ShowMessage d = new(element);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }


        private class LayoutEventScriptEditorTreeNodeShowMessage : LayoutEventScriptEditorTreeNodeAction {
            public LayoutEventScriptEditorTreeNodeShowMessage(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            public static string GetDescription(XmlElement element) {
                string messageType = "message";

                switch (element.GetAttribute("MessageType")) {
                    case "Message":
                        messageType = "message";
                        break;

                    case "Warning":
                        messageType = "warning";
                        break;

                    case "Error":
                        messageType = "error";
                        break;
                }

                return "Show " + messageType + ": '" + element.GetAttribute("Message") + "'";
            }

            protected override string Description => GetDescription(Element);
        }

        #endregion

        #region Set Attribute

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_SetAttribute([DispatchFilter("XPath", "SetAttribute")] XmlElement element) => new LayoutEventScriptEditorTreeNodeSetAttribute(element);

        [DispatchTarget]
        private string GetEventScriptDescription_SetAttribute([DispatchFilter("XPath", "SetAttribute")] XmlElement element) => LayoutEventScriptEditorTreeNodeSetAttribute.GetDescription(element);

        [DispatchTarget]
        private void EditEventScriptElement_SetAttribute([DispatchFilter("XPath", "SetAttribute")] XmlElement element, IEventScriptEditorSite site) {
            Controls.EventScriptEditorDialogs.SetAttribute d = new(element);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        private class LayoutEventScriptEditorTreeNodeSetAttribute : LayoutEventScriptEditorTreeNodeAction {
            public LayoutEventScriptEditorTreeNodeSetAttribute(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            public static string GetDescription(XmlElement element) {
                string symbol = element.GetAttribute("Symbol");
                string attribute = element.GetAttribute("Attribute");
                string setPrefix = "Set attribute " + attribute + " of " + symbol + " to ";

                switch (element.GetAttribute("SetTo")) {
                    case "Text":
                        return setPrefix + "\"" + element.GetAttribute("Value") + "\"";

                    case "Boolean":
                        return setPrefix + element.GetAttribute("Value");

                    case "Number":
                        return element.GetAttribute("Op") == "Add"
                            ? "Add " + element.GetAttribute("Value") + " to the value of attribute " + attribute + " of symbol " + symbol
                            : setPrefix + element.GetAttribute("Value");

                    case "ValueOf":
                        string access = element.GetAttribute("SymbolToAccess");
                        string valueOfSymbol = element.GetAttribute("SymbolTo");
                        string valueOfName = element.GetAttribute("NameTo");

                        return access == "Property"
                            ? setPrefix + "value of property " + valueOfName + " of symbol " + valueOfSymbol
                            : setPrefix + "value of attribute " + valueOfName + " of symbol " + valueOfSymbol;

                    case "Remove":
                        return "Remove attribute " + attribute + " of symbol " + symbol;

                    default:
                        return "{UNKNOWN SetAttribute Format}";
                }
            }

            protected override string Description => GetDescription(Element);
        }

        #endregion

        #region Generate Event

        [DispatchTarget]
        private LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode_GenerateEvent([DispatchFilter("XPath", "GenerateEvent")] XmlElement element) => new LayoutEventScriptEditorTreeNodeGenerateEvent(element);

        [DispatchTarget]
        private string GetEventScriptDescription_GenerateEvent([DispatchFilter("XPath", "GenerateEvent")] XmlElement element) => LayoutEventScriptEditorTreeNodeGenerateEvent.GetDescription(element);

        [DispatchTarget]
        private void EditEventScriptElement_GenerateEvent([DispatchFilter("XPath", "GenerateEvent")] XmlElement element, IEventScriptEditorSite site) {
            Controls.EventScriptEditorDialogs.GenerateEvent d = new(element);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }


        private class LayoutEventScriptEditorTreeNodeGenerateEvent : LayoutEventScriptEditorTreeNodeAction {
            public LayoutEventScriptEditorTreeNodeGenerateEvent(XmlElement conditionElement) : base(conditionElement) {
            }

            private static string? GetArgumentDescription(XmlElement element, string prefix) {
                string getValueOf(string constant) =>
                    element.GetAttribute($"Type{prefix}") switch {
                        "Boolean" => $"is boolean {constant}",
                        "String" => $"is string \"{constant}\"",
                        "Integer" => $"is integer {constant}",
                        "Double" => $"is double {constant}",
                        _ => $"is unknown type {constant}"
                    };

                return element.GetAttribute(prefix + "Type") switch {
                    "Reference" => $"is {element.GetAttribute($"{prefix}SymbolName")}",
                    "ValueOf" when element.GetAttribute($"Symbol{prefix}Access") == "Value" =>
                      getValueOf(element.GetAttribute($"Value{prefix}")),
                    "ValueOf" => $"is {GetOperandDescription(element, prefix, null)}",
                    "Context" => "is context",
                    _ => null
                };
            }

            public static string GetDescription(XmlElement element) {
                var parts = new List<string>();
                var argument = GetArgumentDescription(element, "Sender");

                parts.Add("Generate Event " + element.GetAttribute("EventName"));

                if (argument != null)
                    parts.Add("Sender " + argument);

                argument = GetArgumentDescription(element, "Info");

                if (argument != null)
                    parts.Add("Info " + argument);

                if (element["Options"] != null) {
                    var optionsElement = element["Options"];

                    if (optionsElement != null && optionsElement.ChildNodes.Count > 0) {
                        parts.Add("Options {");

                        foreach (XmlElement optionElement in optionsElement)
                            parts.Add(optionElement.GetAttribute("Name") + " is " + GetOperandDescription(optionElement, "Option") +
                                (optionElement.NextSibling != null ? "," : ""));

                        parts.Add("}");
                    }
                }

                return string.Join(" ", parts);
            }

            protected override string Description => GetDescription(Element);
        }

        #endregion

        #endregion
    }
}

