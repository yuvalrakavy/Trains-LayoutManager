using LayoutManager.CommonUI.Controls;
using MethodDispatcher;
using System.Xml;

namespace LayoutManager.CommonUI {
    [LayoutModule("Event Script UI Manager", UserControl = false)]
    public class EventScriptUImanager : LayoutModuleBase {
        private const string A_Choice = "Choice";
        private const string A_Weight = "Weight";

        #region Methods to return add menus for various section (events, condition and actions)

        [LayoutEvent("get-event-script-editor-event-container-menu")]
        private void EventContainerAddMenu(LayoutEvent e) {
            var eventContainerNode = Ensure.NotNull<LayoutEventScriptEditorTreeNode>(e.Sender, "eventContainerNode");
            var menu = Ensure.NotNull<ToolStripDropDownMenu>(e.Info, "menu");

            if (eventContainerNode.Element["Events"] == null)
                menu.Items.Add(new EventScriptEditorAddMenuItem(e, "Events", "Events"));
            if (eventContainerNode.SupportConditions && eventContainerNode.Element["Condition"] == null)
                menu.Items.Add(new EventScriptEditorAddMenuItem(e, "Condition", "Condition"));
            if (eventContainerNode.SupportActions && eventContainerNode.Element["Actions"] == null)
                menu.Items.Add(new EventScriptEditorAddMenuItem(e, "Actions", "Actions"));
        }

        [LayoutEvent("get-event-script-editor-insert-event-container-menu")]
        private void GetEventScriptEditorInsertEventConatinerMenu(LayoutEvent e) {
            var menu = Ensure.NotNull<ToolStripDropDownMenu>(e.Info, "menu");

            menu.Items.Add(new EventScriptEditorInsertEventContainerMenuItem(e, "Any", "Any"));
            menu.Items.Add(new EventScriptEditorInsertEventContainerMenuItem(e, "All", "All"));
            menu.Items.Add(new EventScriptEditorInsertEventContainerMenuItem(e, "Sequence", "Sequence"));
            menu.Items.Add(new EventScriptEditorInsertEventContainerMenuItem(e, "Repeat", "Repeat"));
            menu.Items.Add(new EventScriptEditorInsertEventContainerMenuItem(e, "Task", "Task"));
        }

        [LayoutEvent("get-event-script-editor-event-menu")]
        private void EventAddMenu(LayoutEvent e) {
            var eventNode = Ensure.NotNull<LayoutEventScriptEditorTreeNode>(e.Sender, "eventNode");
            var menu = Ensure.NotNull<ToolStripDropDownMenu>(e.Info, "menu");

            if (eventNode.SupportConditions && eventNode.Element["Condition"] == null)
                menu.Items.Add(new EventScriptEditorAddMenuItem(e, "Condition", "Condition"));
            if (eventNode.SupportActions && eventNode.Element["Actions"] == null)
                menu.Items.Add(new EventScriptEditorAddMenuItem(e, "Actions", "Actions"));
        }

        [LayoutEvent("get-event-script-editor-events-section-menu", Order = 0)]
        private void AddEventsSectionAddMenuWithEventContainers(LayoutEvent e) {
            var menu = Ensure.ValueNotNull<MenuOrMenuItem>(e.Info, "menu");

            menu.Items.Add(new EventScriptEditorAddMenuItem(e, "Any", "Any"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(e, "All", "All"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(e, "Sequence", "Sequence"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(e, "Repeat", "Repeat"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(e, "Random Choice", "RandomChoice"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(e, "Task", "Task"));
        }

        [LayoutEvent("get-event-script-editor-events-section-menu", Order = 100)]
        private void AddEventsSectionAddMenuWithEvents(LayoutEvent e) {
            var menu = Ensure.ValueNotNull<MenuOrMenuItem>(e.Info, "menu");

            if (menu.Items.Count > 0)
                menu.Items.Add(new ToolStripSeparator());

            menu.Items.Add(new EventScriptEditorAddMenuItem(e, "Do now", "DoNow"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(e, "Wait", "Wait"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(e, "Wait for Event", "WaitForEvent"));
        }

        [LayoutEvent("get-event-script-editor-random-choice-menu")]
        private void AddRandomChoiceMenu(LayoutEvent e) {
            var menu = Ensure.NotNull<ToolStripDropDownMenu>(e.Info, "menu");

            menu.Items.Add(new EventScriptEditorAddMenuItem(e, A_Choice, A_Choice));
        }

        [LayoutEvent("get-event-script-editor-condition-section-menu")]
        private void AddConditionSectionAddMenu(LayoutEvent e) {
            var menu = Ensure.NotNull<ToolStripDropDownMenu>(e.Info, "menu");

            menu.Items.Add(new EventScriptEditorAddMenuItem(e, "And", "And"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(e, "Or", "Or"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(e, "Not", "Not"));
            menu.Items.Add("-");
            menu.Items.Add(new EventScriptEditorAddMenuItem(e, "If (String)", "IfString"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(e, "If (Number)", "IfNumber"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(e, "If (Boolean)", "IfBoolean"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(e, "If (Defined)", "IfDefined"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(e, "If (Time)", "IfTime"));
        }

        [LayoutEvent("get-event-script-editor-insert-condition-container-menu")]
        private void GetEventScriptEditorInsertConditionContainerMenu(LayoutEvent e) {
            var menu = Ensure.NotNull<ToolStripDropDownMenu>(e.Info, "menu");

            menu.Items.Add(new EventScriptEditorInsertConditionContainerMenuItem(e, "And", "And"));
            menu.Items.Add(new EventScriptEditorInsertConditionContainerMenuItem(e, "Or", "Or"));
            menu.Items.Add(new EventScriptEditorInsertConditionContainerMenuItem(e, "Not", "Not"));
        }
        [LayoutEvent("get-event-script-editor-actions-section-menu")]
        private void GetActionsSectionAddMenu(LayoutEvent e) {
            var menu = Ensure.NotNull<ToolStripDropDownMenu>(e.Info, "menu");

            menu.Items.Add(new EventScriptEditorAddMenuItem(e, "Show message", "ShowMessage"));
            menu.Items.Add(new EventScriptEditorAddMenuItem(e, "Set Attribute", "SetAttribute"));
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

        public static string GetEventOrEventContainerDescription(LayoutEvent e, string prefix, string suffix) =>
            GetEventOrEventContainerDescription(Ensure.NotNull<XmlElement>(e.Sender, "element"), prefix, suffix);

        public static string GetEventOrEventContainerDescription(LayoutEvent e, string prefix) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");

            return GetEventOrEventContainerDescription(element, prefix);
        }

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

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "Events")]
        private void GetEventsSectionTreeNode(LayoutEvent e) {
            var eventsElement = Ensure.NotNull<XmlElement>(e.Sender, "eventsElement");

            e.Info = new LayoutEventScriptEditorTreeNodeEventsSection(eventsElement);
        }

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "Condition")]
        private void GetConditionSectionTreeNode(LayoutEvent e) {
            var conditionElement = Ensure.NotNull<XmlElement>(e.Sender, "conditionElement");

            e.Info = new LayoutEventScriptEditorTreeNodeConditionSection(conditionElement);
        }

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "Actions")]
        private void GetActionsSectionTreeNode(LayoutEvent e) {
            var actionsElement = Ensure.NotNull<XmlElement>(e.Sender, "actionsElement");

            e.Info = new LayoutEventScriptEditorTreeNodeActionsSection(actionsElement);
        }

        #endregion

        #region Event Containers

        #region Sequence

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "Sequence")]
        private void GetSequenceTreeNode(LayoutEvent e) {
            var sequenceElement = Ensure.NotNull<XmlElement>(e.Sender, "sequenceElement");

            e.Info = new LayoutEventScriptEditorTreeNodeSequence(sequenceElement);
        }

        [DispatchTarget]
        private string GetEventScriptDescription_Sequence([DispatchFilter(Type = "XPatt", Value = "Sequence")] XmlElement element) {
            return GetEventOrEventContainerDescription(element, "Sequence");
        }

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

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "All")]
        private void GetAllTreeNode(LayoutEvent e) {
            var allElement = Ensure.NotNull<XmlElement>(e.Sender, "allElement");

            e.Info = new LayoutEventScriptEditorTreeNodeAll(allElement);
        }

        [DispatchTarget]
        private string GetEventScriptDescription_All([DispatchFilter(Type = "XPath", Value = "All")] XmlElement element) {
            return GetEventOrEventContainerDescription(element, "All");
        }

        private class LayoutEventScriptEditorTreeNodeAll : LayoutEventScriptEditorTreeNodeEventContainer {
            public LayoutEventScriptEditorTreeNodeAll(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            protected override string Description => "All";

            public override bool CanHaveOptionalSubItems => true;
        }

        #endregion

        #region Any

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "Any")]
        private void GetAnyTreeNode(LayoutEvent e) {
            var anyElement = Ensure.NotNull<XmlElement>(e.Sender, "anyElement");

            e.Info = new LayoutEventScriptEditorTreeNodeAny(anyElement);
        }

        [DispatchTarget]
        private string GetEventScriptDescription_Any([DispatchFilter(Type = "XPath", Value = "Any")] XmlElement element) {
            return GetEventOrEventContainerDescription(element, "Any");
        }

        private class LayoutEventScriptEditorTreeNodeAny : LayoutEventScriptEditorTreeNodeEventContainer {
            public LayoutEventScriptEditorTreeNodeAny(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            protected override string Description => "Any";

            public override bool CanHaveOptionalSubItems => true;
        }

        #endregion

        #region Repeat

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "Repeat")]
        private void GetRepeatTreeNode(LayoutEvent e) {
            var repeatElement = Ensure.NotNull<XmlElement>(e.Sender, "repeatElement");

            e.Info = new LayoutEventScriptEditorTreeNodeRepeat(repeatElement);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "Repeat")]
        private void EditRepeatTreeNode(LayoutEvent e) {
            var repeatElement = Ensure.NotNull<XmlElement>(e.Sender, "repeatElement");
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            Controls.EventScriptEditorDialogs.RepeatCondition repeatDialog = new(repeatElement);

            if (repeatDialog.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        private string GetEventScriptDescription_Repeat([DispatchFilter(Type = "XPath", Value = "Repeat")] XmlElement element) {
            return GetEventOrEventContainerDescription(element, "Repeat");
        }

        private class LayoutEventScriptEditorTreeNodeRepeat : LayoutEventScriptEditorTreeNodeMayBeOptional {
            private const string A_Count = "Count";

            public LayoutEventScriptEditorTreeNodeRepeat(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            public override string AddNodeEventName => "get-event-script-editor-events-section-menu";

            public override string InsertNodeEventName => "get-event-script-editor-insert-event-container-menu";

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

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "RandomChoice")]
        private void GetRandomChoiceTreeNode(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");

            e.Info = new LayoutEventScriptEditorTreeNodeRandomChoice(element);
        }

        [DispatchTarget]
        private string GetEventScriptDescription_RandomChoice([DispatchFilter(Type = "XPath", Value = "RandomChoice")] XmlElement element) {
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

            public override string AddNodeEventName => "get-event-script-editor-random-choice-menu";

            protected override string Description => "Random-Choice";

            protected override int IconIndex => IconEvent;

            public override LayoutEventScriptEditorTreeNode? NodeToEdit => null;
        }

        #endregion

        #region Choice Entry

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = A_Choice)]
        private void GetRandomChoiceEntryTreeNode(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");

            e.Info = new LayoutEventScriptEditorTreeNodeRandomChoiceEntry(element);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = A_Choice)]
        private void EditRandomChoiceEntryTreeNode(LayoutEvent e) {
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            var weightText = CommonUI.Dialogs.InputBox.Show("Choice Weight", "Please enter the weight (probability) for this choice", CommonUI.Dialogs.InputBoxValidationOptions.IntegerNumber);

            if (weightText == null)
                site.EditingCancelled();
            else {
                var element = Ensure.NotNull<XmlElement>(e.Sender, "element");

                element.SetAttributeValue(A_Weight, int.Parse(weightText));
                site.EditingDone();
            }
        }

        private class LayoutEventScriptEditorTreeNodeRandomChoiceEntry : LayoutEventScriptEditorTreeNode {
            public LayoutEventScriptEditorTreeNodeRandomChoiceEntry(XmlElement element) : base(element) {
                AddChildEventScriptTreeNodes();
            }

            public override string AddNodeEventName => "get-event-script-editor-events-section-menu";

            protected override string Description => "Choice (" + Element.GetAttribute(EventScriptUImanager.A_Weight) + ")";

            protected override int IconIndex => IconEvent;

            public override int MinSubNodes => 1;

            public override int MaxSubNodes => 1;

            public override LayoutEventScriptEditorTreeNode? NodeToEdit => this;        // Choice can be edited
        }

        #endregion

        #region Task

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "Task")]
        private void GetTaskTreeNode(LayoutEvent e) {
            var taskElement = Ensure.NotNull<XmlElement>(e.Sender, "taskElement");

            e.Info = new LayoutEventScriptEditorTreeNodeTask(taskElement);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "Task")]
        private void EditTaskTreeNode(LayoutEvent e) {
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
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

            public override string AddNodeEventName => "get-event-script-editor-events-section-menu";

            public override string InsertNodeEventName => "get-event-script-editor-insert-event-container-menu";

            protected override int IconIndex => IconEvent;

            static public string GetDescription(XmlElement element) => "Task";

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

        #endregion

        #region Events

        #region Wait

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "Wait")]
        private void GetWaitTreeNode(LayoutEvent e) {
            var waitElement = Ensure.NotNull<XmlElement>(e.Sender, "waitElement");

            e.Info = new LayoutEventScriptEditorTreeNodeWait(waitElement);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "Wait")]
        private void EditWaitTreeNode(LayoutEvent e) {
            var waitElement = Ensure.NotNull<XmlElement>(e.Sender, "waitElement");
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            Controls.EventScriptEditorDialogs.WaitCondition waitDialog = new(waitElement);

            if (waitDialog.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }


        [DispatchTarget]
        private string GetEventScriptDescription_Wait([DispatchFilter(Type = "XPath", Value = "Wait")] XmlElement element) {
            return GetEventOrEventContainerDescription(element, LayoutEventScriptEditorTreeNodeWait.GetWaitDescription(element));
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

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "DoNow")]
        private void GetDoNowTreeNode(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");

            e.Info = new LayoutEventScriptEditorTreeNodeDoNow(element);
        }

        [DispatchTarget]
        private string GetEventScriptDescription_DoNow([DispatchFilter(Type = "XPath", Value = "DoNow")] XmlElement element) {
            return GetEventOrEventContainerDescription(element, LayoutEventScriptEditorTreeNodeDoNow.GetDescription(element));
        }

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

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "WaitForEvent")]
        private void GetWaitForEventTreeNode(LayoutEvent e) {
            var waitElement = Ensure.NotNull<XmlElement>(e.Sender, "waitElement");

            e.Info = new LayoutEventScriptEditorTreeNodeWaitForEvent(waitElement);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "WaitForEvent")]
        private void EditWaitForEventTreeNode(LayoutEvent e) {
            var waitElement = Ensure.NotNull<XmlElement>(e.Sender, "waitElement");
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            Controls.EventScriptEditorDialogs.WaitForEvent d = new(waitElement);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [DispatchTarget]
        private string GetEventScriptDescription_WaitForEvent([DispatchFilter(Type = "XPath", Value = "WaitForEvent")] XmlElement element) {
            return GetEventOrEventContainerDescription(element, LayoutEventScriptEditorTreeNodeWaitForEvent.GetDescription(element));
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

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "And")]
        private void GetAndTreeNode(LayoutEvent e) {
            var AndElement = Ensure.NotNull<XmlElement>(e.Sender, "AndElement");

            e.Info = new LayoutEventScriptEditorTreeNodeAnd(AndElement);
        }

        [DispatchTarget]
        private string GetEventScriptDescription_And([DispatchFilter(Type = "XPath", Value = "And")] XmlElement element) {
            return GetConditionContainerDescription(element, "And");
        }

        private class LayoutEventScriptEditorTreeNodeAnd : LayoutEventScriptEditorTreeNodeConditionContainer {
            public LayoutEventScriptEditorTreeNodeAnd(XmlElement andElement) : base(andElement) {
                AddChildEventScriptTreeNodes();
            }

            protected override string Description => "And";
        }

        #endregion

        #region Or

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "Or")]
        private void GetOrTreeNode(LayoutEvent e) {
            var OrElement = Ensure.NotNull<XmlElement>(e.Sender, "OrElement");

            e.Info = new LayoutEventScriptEditorTreeNodeOr(OrElement);
        }

        [DispatchTarget]
        private string GetEventScriptDescription_Or([DispatchFilter(Type = "XPath", Value = "Or")] XmlElement element) {
            return GetConditionContainerDescription(element, "Or");
        }

        private class LayoutEventScriptEditorTreeNodeOr : LayoutEventScriptEditorTreeNodeConditionContainer {
            public LayoutEventScriptEditorTreeNodeOr(XmlElement orElement) : base(orElement) {
                AddChildEventScriptTreeNodes();
            }

            protected override string Description => "Or";
        }

        #endregion

        #region Not

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "Not")]
        private void GetNotTreeNode(LayoutEvent e) {
            var NotElement = Ensure.NotNull<XmlElement>(e.Sender, "NotElement");

            e.Info = new LayoutEventScriptEditnotTreeNodeNot(NotElement);
        }

        [DispatchTarget]
        private string GetEventScriptDescription_Not([DispatchFilter(Type = "XPath", Value = "Not")] XmlElement element) {
            return "Not " + GetElementDescription((XmlElement)element.ChildNodes[0]!);
        }

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

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "IfString")]
        private void GetIfStringTreeNode(LayoutEvent e) {
            var ifElement = Ensure.NotNull<XmlElement>(e.Sender, "ifElement");

            e.Info = new LayoutEventScriptEditorTreeNodeIfString(ifElement);
        }

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

        [LayoutEvent("event-script-editor-edit-element", IfSender = "IfString")]
        private void EditIfStringNode(LayoutEvent e) {
            var ifElement = Ensure.NotNull<XmlElement>(e.Sender, "ifElement");
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            Controls.EventScriptEditorDialogs.IfCondition ifDialog = new(ifElement, new IfStringCustomizer());

            if (ifDialog.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [DispatchTarget]
        private string GetEventScriptDescription_IfString([DispatchFilter(Type = "XPath", Value = "IfString")] XmlElement element) {
            return LayoutEventScriptEditorTreeNodeIfString.GetDescription(element);
        }

        public class LayoutEventScriptEditorTreeNodeIfString : LayoutEventScriptEditorTreeNodeIf {
            public LayoutEventScriptEditorTreeNodeIfString(XmlElement ifStringElement) : base(ifStringElement) {
            }

            public static string GetDescription(XmlElement element) {
                string operatorName = (string)EventManager.Event(new LayoutEvent("get-event-script-operator-name", element))!;

                return "If (string) " + GetOperandDescription(element, "1", typeof(string)) + " " + operatorName + " " + GetOperandDescription(element, "2", typeof(string));
            }

            protected override string Description => GetDescription(Element);
        }

        #endregion

        #region If (Number)

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "IfNumber")]
        private void GetIfNumberTreeNode(LayoutEvent e) {
            var ifElement = Ensure.NotNull<XmlElement>(e.Sender, "ifElement");

            e.Info = new LayoutEventScriptEditorTreeNodeIfNumber(ifElement);
        }

        private class IfNumberCustomizer : Controls.EventScriptEditorDialogs.IIfConditionDialogCustomizer {
            public string Title => "If (number)";

            public string[] OperatorNames => new string[] { "eq", "ne", "gt", "ge", "le", "lt" };

            public string[] OperatorDescriptions => new string[] { "=", "<>", ">", ">=", "=<", "<" };

            public Type[] AllowedTypes => new Type[] {
                                          typeof(int),
                    };

            public bool ValueIsBoolean => false;
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "IfNumber")]
        private void EditIfNumberNode(LayoutEvent e) {
            var ifElement = Ensure.NotNull<XmlElement>(e.Sender, "ifElement");
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            Controls.EventScriptEditorDialogs.IfCondition ifDialog = new(ifElement, new IfNumberCustomizer());

            if (ifDialog.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [DispatchTarget]
        private string GetEventScriptDescription_IfNumber([DispatchFilter(Type = "XPath", Value = "IfNumber")] XmlElement element) {
            return LayoutEventScriptEditorTreeNodeIfNumber.GetDescription(element);
        }

        public class LayoutEventScriptEditorTreeNodeIfNumber : LayoutEventScriptEditorTreeNodeIf {
            public LayoutEventScriptEditorTreeNodeIfNumber(XmlElement ifStringElement) : base(ifStringElement) {
            }

            public static string GetDescription(XmlElement element) {
                string operatorName = (string)EventManager.Event(new LayoutEvent("get-event-script-operator-name", element))!;

                return "If (number) " + GetOperandDescription(element, "1", typeof(int)) + " " + operatorName + " " + GetOperandDescription(element, "2", typeof(int));
            }

            protected override string Description => GetDescription(Element);
        }

        #endregion

        #region If (Boolean)

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "IfBoolean")]
        private void GetIfBooleanTreeNode(LayoutEvent e) {
            var ifElement = Ensure.NotNull<XmlElement>(e.Sender, "ifElement");

            e.Info = new LayoutEventScriptEditorTreeNodeIfBoolean(ifElement);
        }

        private class IfBooleanCustomizer : Controls.EventScriptEditorDialogs.IIfConditionDialogCustomizer {
            public string Title => "If (Boolean)";

            public string[] OperatorNames => new string[] { "Equal", "NotEqual" };

            public string[] OperatorDescriptions => new string[] { "=", "<>" };

            public Type[] AllowedTypes => new Type[] {
                                          typeof(bool),
                    };

            public bool ValueIsBoolean => true;
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "IfBoolean")]
        private void EditIfBooleanNode(LayoutEvent e) {
            var ifElement = Ensure.NotNull<XmlElement>(e.Sender, "ifElement");
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            Controls.EventScriptEditorDialogs.IfCondition ifDialog = new(ifElement, new IfBooleanCustomizer());

            if (ifDialog.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [DispatchTarget]
        private string GetEventScriptDescription_IfBoolean([DispatchFilter(Type = "XPath", Value = "IfBoolean")] XmlElement element) {
            return LayoutEventScriptEditorTreeNodeIfBoolean.GetDescription(element);
        }

        public class LayoutEventScriptEditorTreeNodeIfBoolean : LayoutEventScriptEditorTreeNodeIf {
            public LayoutEventScriptEditorTreeNodeIfBoolean(XmlElement ifBooleanElement) : base(ifBooleanElement) {
            }

            public static string GetDescription(XmlElement element) {
                string operatorName = (string)EventManager.Event(new LayoutEvent("get-event-script-operator-name", element))!;

                return "If (Boolean) " + GetOperandDescription(element, "1", typeof(bool)) + " " + operatorName + " " + GetOperandDescription(element, "2", typeof(bool));
            }

            protected override string Description => GetDescription(Element);
        }

        #endregion

        #region If (Defined)

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "IfDefined")]
        private void GetIfDefinedTreeNode(LayoutEvent e) {
            var ifElement = Ensure.NotNull<XmlElement>(e.Sender, "ifElement");

            e.Info = new LayoutEventScriptEditorTreeNodeIfDefined(ifElement);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "IfDefined")]
        private void EditIfDefinedNode(LayoutEvent e) {
            var ifElement = Ensure.NotNull<XmlElement>(e.Sender, "ifElement");
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            Controls.EventScriptEditorDialogs.IfDefined ifDialog = new(ifElement);

            if (ifDialog.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [DispatchTarget]
        private string GetEventScriptDescription_IfDefined([DispatchFilter(Type = "XPath", Value = "IfDefined")] XmlElement element) {
            return LayoutEventScriptEditorTreeNodeIfDefined.GetDescription(element);
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

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "IfTime")]
        private void GetIfTimeTreeNode(LayoutEvent e) {
            var ifElement = Ensure.NotNull<XmlElement>(e.Sender, "ifElement");

            e.Info = new LayoutEventScriptEditorTreeNodeIfTime(ifElement);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "IfTime")]
        private void EditIfTimeNode(LayoutEvent e) {
            var ifElement = Ensure.NotNull<XmlElement>(e.Sender, "ifElement");
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            Controls.EventScriptEditorDialogs.IfTime ifDialog = new(ifElement);

            if (ifDialog.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [DispatchTarget]
        private string GetEventScriptDescription_IfTime([DispatchFilter(Type = "XPath", Value = "IfTime")] XmlElement element) {
            return LayoutEventScriptEditorTreeNodeIfTime.GetDescription(element);
        }

        public class LayoutEventScriptEditorTreeNodeIfTime : LayoutEventScriptEditorTreeNodeCondition {
            public LayoutEventScriptEditorTreeNodeIfTime(XmlElement conditionElement) : base(conditionElement) {
            }

            private static string? GetNodesDescription(XmlElement element, string nodeName, string title) {
                var nodes = (IIfTimeNode[]?)EventManager.Event(new LayoutEvent("parse-if-time-element", element, nodeName));
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

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "ShowMessage")]
        private void GetShowMessageTreeNode(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");

            e.Info = new LayoutEventScriptEditorTreeNodeShowMessage(element);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "ShowMessage")]
        private void EditShowMessageTreeNode(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            Controls.EventScriptEditorDialogs.ShowMessage d = new(element);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [DispatchTarget]
        private string GetEventScriptDescription_ShowMessage([DispatchFilter(Type = "XPath", Value = "ShowMessage")] XmlElement element) {
            return LayoutEventScriptEditorTreeNodeShowMessage.GetDescription(element);
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

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "SetAttribute")]
        private void GetSetAttributeTreeNode(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");

            e.Info = new LayoutEventScriptEditorTreeNodeSetAttribute(element);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "SetAttribute")]
        private void EditSetAttributeTreeNode(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            Controls.EventScriptEditorDialogs.SetAttribute d = new(element);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [DispatchTarget]
        private string GetEventScriptDescription_SetAttribute([DispatchFilter(Type = "XPath", Value = "SetAttribute")] XmlElement element) {
            return LayoutEventScriptEditorTreeNodeSetAttribute.GetDescription(element);
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

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "GenerateEvent")]
        private void GetGenerateEventTreeNode(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");

            e.Info = new LayoutEventScriptEditorTreeNodeGenerateEvent(element);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "GenerateEvent")]
        private void EditGenerateEventTreeNode(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            Controls.EventScriptEditorDialogs.GenerateEvent d = new(element);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [DispatchTarget]
        private string GetEventScriptDescription_GenerateEvent([DispatchFilter(Type = "XPath", Value = "GenerateEvent")] XmlElement element) {
            return LayoutEventScriptEditorTreeNodeGenerateEvent.GetDescription(element);
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
                        _ => $"is unkown type {constant}"
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

