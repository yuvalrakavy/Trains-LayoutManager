using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.CommonUI.Controls;

#pragma warning disable IDE0051, IDE0060
#nullable enable
namespace LayoutManager.CommonUI {
    [LayoutModule("Event Script UI Manager", UserControl = false)]
    public class EventScriptUImanager : LayoutModuleBase {
        private const string A_Choice = "Choice";
        private const string A_Weight = "Weight";

        #region Methods to return add menus for various section (events, condition and actions)

        [LayoutEvent("get-event-script-editor-event-container-menu")]
        private void eventContainerAddMenu(LayoutEvent e) {
            var eventContainerNode = Ensure.NotNull<LayoutEventScriptEditorTreeNode>(e.Sender, "eventContainerNode");
            var menu = Ensure.NotNull<Menu>(e.Info, "menu");

            if (eventContainerNode.Element["Events"] == null)
                menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Events", "Events"));
            if (eventContainerNode.SupportConditions && eventContainerNode.Element["Condition"] == null)
                menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Condition", "Condition"));
            if (eventContainerNode.SupportActions && eventContainerNode.Element["Actions"] == null)
                menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Actions", "Actions"));
        }

        [LayoutEvent("get-event-script-editor-insert-event-container-menu")]
        private void getEventScriptEditorInsertEventConatinerMenu(LayoutEvent e) {
            var menu = Ensure.NotNull<Menu>(e.Info, "menu");

            menu.MenuItems.Add(new Controls.EventScriptEditorInsertEventContainerMenuItem(e, "Any", "Any"));
            menu.MenuItems.Add(new Controls.EventScriptEditorInsertEventContainerMenuItem(e, "All", "All"));
            menu.MenuItems.Add(new Controls.EventScriptEditorInsertEventContainerMenuItem(e, "Sequence", "Sequence"));
            menu.MenuItems.Add(new Controls.EventScriptEditorInsertEventContainerMenuItem(e, "Repeat", "Repeat"));
            menu.MenuItems.Add(new Controls.EventScriptEditorInsertEventContainerMenuItem(e, "Task", "Task"));
        }

        [LayoutEvent("get-event-script-editor-event-menu")]
        private void eventAddMenu(LayoutEvent e) {
            var eventNode = Ensure.NotNull<LayoutEventScriptEditorTreeNode>(e.Sender, "eventNode");
            var menu = Ensure.NotNull<Menu>(e.Info, "menu");

            if (eventNode.SupportConditions && eventNode.Element["Condition"] == null)
                menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Condition", "Condition"));
            if (eventNode.SupportActions && eventNode.Element["Actions"] == null)
                menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Actions", "Actions"));
        }

        [LayoutEvent("get-event-script-editor-events-section-menu", Order = 0)]
        private void addEventsSectionAddMenuWithEventContainers(LayoutEvent e) {
            var menu = Ensure.NotNull<Menu>(e.Info, "menu");

            menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Any", "Any"));
            menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "All", "All"));
            menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Sequence", "Sequence"));
            menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Repeat", "Repeat"));
            menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Random Choice", "RandomChoice"));
            menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Task", "Task"));
        }

        [LayoutEvent("get-event-script-editor-events-section-menu", Order = 100)]
        private void addEventsSectionAddMenuWithEvents(LayoutEvent e) {
            var menu = Ensure.NotNull<Menu>(e.Info, "menu");

            if (menu.MenuItems.Count > 0)
                menu.MenuItems.Add("-");

            menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Do now", "DoNow"));
            menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Wait", "Wait"));
            menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Wait for Event", "WaitForEvent"));
        }

        [LayoutEvent("get-event-script-editor-random-choice-menu")]
        private void addRandomChoiceMenu(LayoutEvent e) {
            var menu = Ensure.NotNull<Menu>(e.Info, "menu");

            menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, A_Choice, A_Choice));
        }

        [LayoutEvent("get-event-script-editor-condition-section-menu")]
        private void addConditionSectionAddMenu(LayoutEvent e) {
            var menu = Ensure.NotNull<Menu>(e.Info, "menu");

            menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "And", "And"));
            menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Or", "Or"));
            menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Not", "Not"));
            menu.MenuItems.Add("-");
            menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "If (String)", "IfString"));
            menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "If (Number)", "IfNumber"));
            menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "If (Boolean)", "IfBoolean"));
            menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "If (Defined)", "IfDefined"));
            menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "If (Time)", "IfTime"));
        }

        [LayoutEvent("get-event-script-editor-insert-condition-container-menu")]
        private void getEventScriptEditorInsertConditionContainerMenu(LayoutEvent e) {
            var menu = Ensure.NotNull<Menu>(e.Info, "menu");

            menu.MenuItems.Add(new Controls.EventScriptEditorInsertConditionContainerMenuItem(e, "And", "And"));
            menu.MenuItems.Add(new Controls.EventScriptEditorInsertConditionContainerMenuItem(e, "Or", "Or"));
            menu.MenuItems.Add(new Controls.EventScriptEditorInsertConditionContainerMenuItem(e, "Not", "Not"));
        }

        [LayoutEvent("get-event-script-editor-actions-section-menu")]
        private void getActionsSectionAddMenu(LayoutEvent e) {
            var menu = Ensure.NotNull<Menu>(e.Info, "menu");

            menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Show message", "ShowMessage"));
            menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Set Attribute", "SetAttribute"));
        }

        #endregion

        #region Utilty methods for generating descriptions

        public static bool IsTrivialContainer(XmlElement containerElement) {
            XmlElement eventsElement = containerElement["Events"];

            return eventsElement != null && eventsElement.ChildNodes.Count == 1 && containerElement["Condition"] == null && containerElement["Actions"] == null;
        }

        public static string GetElementDescription(XmlElement element) => (string)EventManager.Event(new LayoutEvent("get-event-script-description", element))!;

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
                element = (XmlElement)element.ChildNodes[0].ChildNodes[0];
                return (string)GetElementDescription(element);
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
                        substrings.Add(" Condition { " + GetElementDescription((XmlElement)childElement.ChildNodes[0]) + " }");
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

        public static void GetConditionContainerDescription(LayoutEvent e, string containerName) {
            var containerElement = Ensure.NotNull<XmlElement>(e.Sender, "containerElement");

            if (containerElement.ChildNodes.Count == 1)     // If there is only one some condition, return its description
                e.Info = GetElementDescription((XmlElement)containerElement.ChildNodes[0]);
            else {
                string[] conditions = new string[containerElement.ChildNodes.Count];

                int i = 0;
                foreach (XmlElement conditionElement in containerElement) {
                    conditions[i] = ((i == 0) ? "" : " " + containerName + " ") + GetElementDescription(conditionElement);
                    i++;
                }

                e.Info = "(" + string.Concat(conditions) + ")";
            }
        }

        #endregion

        #region Tree nodes for event container sections

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "Events")]
        private void getEventsSectionTreeNode(LayoutEvent e) {
            var eventsElement = Ensure.NotNull<XmlElement>(e.Sender, "eventsElement");

            e.Info = new LayoutEventScriptEditorTreeNodeEventsSection(eventsElement);
        }

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "Condition")]
        private void getConditionSectionTreeNode(LayoutEvent e) {
            var conditionElement = Ensure.NotNull<XmlElement>(e.Sender, "conditionElement");

            e.Info = new LayoutEventScriptEditorTreeNodeConditionSection(conditionElement);
        }

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "Actions")]
        private void getActionsSectionTreeNode(LayoutEvent e) {
            var actionsElement = Ensure.NotNull<XmlElement>(e.Sender, "actionsElement");

            e.Info = new LayoutEventScriptEditorTreeNodeActionsSection(actionsElement);
        }

        #endregion

        #region Event Containers

        #region Sequence

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "Sequence")]
        private void getSequenceTreeNode(LayoutEvent e) {
            var sequenceElement = Ensure.NotNull<XmlElement>(e.Sender, "sequenceElement");

            e.Info = new LayoutEventScriptEditorTreeNodeSequence(sequenceElement);
        }

        [LayoutEvent("get-event-script-description", IfSender = "Sequence")]
        private void getSequenceDescription(LayoutEvent e) {
            e.Info = GetEventOrEventContainerDescription(e, "Sequence");
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
        private void getAllTreeNode(LayoutEvent e) {
            var allElement = Ensure.NotNull<XmlElement>(e.Sender, "allElement");

            e.Info = new LayoutEventScriptEditorTreeNodeAll(allElement);
        }

        [LayoutEvent("get-event-script-description", IfSender = "All")]
        private void getAllDescription(LayoutEvent e) {
            e.Info = GetEventOrEventContainerDescription(e, "All");
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
        private void getAnyTreeNode(LayoutEvent e) {
            var anyElement = Ensure.NotNull<XmlElement>(e.Sender, "anyElement");

            e.Info = new LayoutEventScriptEditorTreeNodeAny(anyElement);
        }

        [LayoutEvent("get-event-script-description", IfSender = "Any")]
        private void getAnyDescription(LayoutEvent e) {
            e.Info = GetEventOrEventContainerDescription(e, "Any");
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
        private void getRepeatTreeNode(LayoutEvent e) {
            var repeatElement = Ensure.NotNull<XmlElement>(e.Sender, "repeatElement");

            e.Info = new LayoutEventScriptEditorTreeNodeRepeat(repeatElement);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "Repeat")]
        private void editRepeatTreeNode(LayoutEvent e) {
            var repeatElement = Ensure.NotNull<XmlElement>(e.Sender, "repeatElement");
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            Controls.EventScriptEditorDialogs.RepeatCondition repeatDialog = new Controls.EventScriptEditorDialogs.RepeatCondition(repeatElement);

            if (repeatDialog.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [LayoutEvent("get-event-script-description", IfSender = "Repeat")]
        private void getRepeatDescription(LayoutEvent e) {
            var conditionElement = Ensure.NotNull<XmlElement>(e.Sender, "conditionElement");

            e.Info = GetEventOrEventContainerDescription(e, LayoutEventScriptEditorTreeNodeRepeat.GetDescription(conditionElement) + " { ", " } ");
        }

        private class LayoutEventScriptEditorTreeNodeRepeat : LayoutEventScriptEditorTreeNodeMayBeOptional {
            private const string A_Count = "Count";

            public LayoutEventScriptEditorTreeNodeRepeat(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            public override string AddNodeEventName => "get-event-script-editor-events-section-menu";

            public override string InsertNodeEventName => "get-event-script-editor-insert-event-container-menu";

            protected override int IconIndex => IconEvent;

            static public string GetDescription(XmlElement element) => (int)element.AttributeValue(A_Count) switch
            {
                int c when c < 0 => "Repeat",
                1 => "Repeat once",
                int c => $"Repeat {c} time"
            };

            protected override string Description => GetDescription(Element);

            /// <summary>
            /// Repeat can have no more than one subcondition
            /// </summary>
            public override int MaxSubNodes => 1;

            public override Controls.LayoutEventScriptEditorTreeNode? NodeToEdit => this;        // repeat rule can be edited (changing the count)

            public override bool ShouldInsertAsFirst(XmlElement elementToInsert) {
                return elementToInsert.Name == "Condition";
            }
        }

        #endregion

        #region Random Choice

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "RandomChoice")]
        private void getRandomChoiceTreeNode(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");

            e.Info = new LayoutEventScriptEditorTreeNodeRandomChoice(element);
        }

        [LayoutEvent("get-event-script-description", IfSender = "RandomChoice")]
        private void getRandomChoiceDescription(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");
            var substrings = new List<string> {
                "Random-Choice [ "
            };

            foreach (XmlElement choiceElement in element.GetElementsByTagName(A_Choice)) {
                substrings.Add($"Choice ({(int)choiceElement.AttributeValue(A_Weight)}): {{ {GetElementDescription((XmlElement)choiceElement.ChildNodes[0])} }} ");
            }

            substrings.Add("] ");

            e.Info = string.Concat(substrings);
        }

        private class LayoutEventScriptEditorTreeNodeRandomChoice : LayoutEventScriptEditorTreeNodeMayBeOptional {
            public LayoutEventScriptEditorTreeNodeRandomChoice(XmlElement element) : base(element) {
                AddChildEventScriptTreeNodes();
            }

            public override string AddNodeEventName => "get-event-script-editor-random-choice-menu";

            protected override string Description => "Random-Choice";

            protected override int IconIndex => IconEvent;

            public override Controls.LayoutEventScriptEditorTreeNode? NodeToEdit => null;
        }

        #endregion

        #region Choice Entry

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = A_Choice)]
        private void getRandomChoiceEntryTreeNode(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");

            e.Info = new LayoutEventScriptEditorTreeNodeRandomChoiceEntry(element);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = A_Choice)]
        private void editRandomChoiceEntryTreeNode(LayoutEvent e) {
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            string weightText = CommonUI.Dialogs.InputBox.Show("Choice Weight", "Please enter the weight (probability) for this choice", CommonUI.Dialogs.InputBoxValidationOptions.IntegerNumber);

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

            public override Controls.LayoutEventScriptEditorTreeNode? NodeToEdit => this;        // Choice can be edited
        }

        #endregion

        #region Task

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "Task")]
        private void getTaskTreeNode(LayoutEvent e) {
            var taskElement = Ensure.NotNull<XmlElement>(e.Sender, "taskElement");

            e.Info = new LayoutEventScriptEditorTreeNodeTask(taskElement);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "Task")]
        private void editTaskTreeNode(LayoutEvent e) {
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            //			Controls.EventScriptEditorDialogs.RepeatCondition	repeatDialog = new Controls.EventScriptEditorDialogs.RepeatCondition((XmlElement)e.Sender);

            //			if(repeatDialog.ShowDialog() == DialogResult.OK)
            //				site.EditingDone();
            //			else
            //				site.EditingCancelled();
            site.EditingDone();
        }

        [LayoutEvent("get-event-script-description", IfSender = "Task")]
        private void getTaskDescription(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");
            XmlElement taskElement = (XmlElement)element.ChildNodes[0];

            e.Info = GetEventOrEventContainerDescription(e, LayoutEventScriptEditorTreeNodeTask.GetDescription(taskElement) + " { ", " } ");
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

            public override Controls.LayoutEventScriptEditorTreeNode? NodeToEdit => this;        // repeat rule can be edited (changing the count)

            public override bool ShouldInsertAsFirst(XmlElement elementToInsert) {
                return elementToInsert.Name == "Condition";
            }
        }

        #endregion

        #endregion

        #region Events

        #region Wait

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "Wait")]
        private void getWaitTreeNode(LayoutEvent e) {
            var waitElement = Ensure.NotNull<XmlElement>(e.Sender, "waitElement");

            e.Info = new LayoutEventScriptEditorTreeNodeWait(waitElement);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "Wait")]
        private void editWaitTreeNode(LayoutEvent e) {
            var waitElement = Ensure.NotNull<XmlElement>(e.Sender, "waitElement");
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            Controls.EventScriptEditorDialogs.WaitCondition waitDialog = new Controls.EventScriptEditorDialogs.WaitCondition(waitElement);

            if (waitDialog.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [LayoutEvent("get-event-script-description", IfSender = "Wait")]
        private void getWaitDescription(LayoutEvent e) {
            var waitElement = Ensure.NotNull<XmlElement>(e.Sender, "waitElement");

            e.Info = GetEventOrEventContainerDescription(e, LayoutEventScriptEditorTreeNodeWait.GetWaitDescription(waitElement));
        }

        private class LayoutEventScriptEditorTreeNodeWait : Controls.LayoutEventScriptEditorTreeNodeEvent {
            private const string A_RandomSeconds = "RandomSeconds";
            private const string A_Minutes = "Minutes";
            private const string A_Seconds = "Seconds";
            private const string A_MilliSeconds = "MilliSeconds";

            public LayoutEventScriptEditorTreeNodeWait(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            private static void addWaitString(XmlElement conditionElement, List<string> list, string attrName, string unitSingle, string unitPlural) {
                if (conditionElement.HasAttribute(attrName)) {
                    var v = (int)conditionElement.AttributeValue(attrName);

                    if (v != 0)
                        list.Add((list.Count != 0 ? ", " : "") + v.ToString() + " " + ((v == 1) ? unitSingle : unitPlural));
                }
            }

            public static string GetWaitDescription(XmlElement conditionElement) {
                var delayStrings = new List<string>();
                string s;

                addWaitString(conditionElement, delayStrings, A_Minutes, "minute", "minutes");
                addWaitString(conditionElement, delayStrings, A_Seconds, "second", "seconds");
                addWaitString(conditionElement, delayStrings, A_MilliSeconds, "milli-second", "milli-seconds");

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
        private void getDoNowTreeNode(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");

            e.Info = new LayoutEventScriptEditorTreeNodeDoNow(element);
        }

        [LayoutEvent("get-event-script-description", IfSender = "DoNow")]
        private void getDoNowDescription(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");

            e.Info = GetEventOrEventContainerDescription(e, LayoutEventScriptEditorTreeNodeDoNow.GetDescription(element));
        }

        private class LayoutEventScriptEditorTreeNodeDoNow : Controls.LayoutEventScriptEditorTreeNodeEvent {
            public LayoutEventScriptEditorTreeNodeDoNow(XmlElement conditionElement) : base(conditionElement) {
                AddChildEventScriptTreeNodes();
            }

            public static string GetDescription(XmlElement element) => "Do now";

            public override Controls.LayoutEventScriptEditorTreeNode? NodeToEdit => null;		// Nothing to edit

            protected override string Description => GetDescription(Element);
        }

        #endregion

        #region Wait for event

        [LayoutEvent("get-event-script-editor-tree-node", IfSender = "WaitForEvent")]
        private void getWaitForEventTreeNode(LayoutEvent e) {
            var waitElement = Ensure.NotNull<XmlElement>(e.Sender, "waitElement");

            e.Info = new LayoutEventScriptEditorTreeNodeWaitForEvent(waitElement);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "WaitForEvent")]
        private void editWaitForEventTreeNode(LayoutEvent e) {
            var waitElement = Ensure.NotNull<XmlElement>(e.Sender, "waitElement");
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            Controls.EventScriptEditorDialogs.WaitForEvent d = new Controls.EventScriptEditorDialogs.WaitForEvent(waitElement);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [LayoutEvent("get-event-script-description", IfSender = "WaitForEvent")]
        private void getWaitForEventDescription(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");

            e.Info = GetEventOrEventContainerDescription(e, LayoutEventScriptEditorTreeNodeWaitForEvent.GetDescription(element));
        }

        private class LayoutEventScriptEditorTreeNodeWaitForEvent : Controls.LayoutEventScriptEditorTreeNodeEvent {
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
        private void getAndTreeNode(LayoutEvent e) {
            var AndElement = Ensure.NotNull<XmlElement>(e.Sender, "AndElement");

            e.Info = new LayoutEventScriptEditorTreeNodeAnd(AndElement);
        }

        [LayoutEvent("get-event-script-description", IfSender = "And")]
        private void getAndDescription(LayoutEvent e) {
            GetConditionContainerDescription(e, "And");
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
        private void getOrTreeNode(LayoutEvent e) {
            var OrElement = Ensure.NotNull<XmlElement>(e.Sender, "OrElement");

            e.Info = new LayoutEventScriptEditorTreeNodeOr(OrElement);
        }

        [LayoutEvent("get-event-script-description", IfSender = "Or")]
        private void getOrDescription(LayoutEvent e) {
            GetConditionContainerDescription(e, "Or");
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
        private void getNotTreeNode(LayoutEvent e) {
            var NotElement = Ensure.NotNull<XmlElement>(e.Sender, "NotElement");

            e.Info = new LayoutEventScriptEditnotTreeNodeNot(NotElement);
        }

        [LayoutEvent("get-event-script-description", IfSender = "Not")]
        private void getNotDescription(LayoutEvent e) {
            var notElement = Ensure.NotNull<XmlElement>(e.Sender, "notElement");

            e.Info = "Not " + GetElementDescription((XmlElement)notElement.ChildNodes[0]);
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
        private void getIfStringTreeNode(LayoutEvent e) {
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
        private void editIfStringNode(LayoutEvent e) {
            var ifElement = Ensure.NotNull<XmlElement>(e.Sender, "ifElement");
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            Controls.EventScriptEditorDialogs.IfCondition ifDialog = new Controls.EventScriptEditorDialogs.IfCondition(ifElement, new IfStringCustomizer());

            if (ifDialog.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [LayoutEvent("get-event-script-description", IfSender = "IfString")]
        private void getIfStringDescription(LayoutEvent e) {
            var ifStringElement = Ensure.NotNull<XmlElement>(e.Sender, "ifStringElement");

            e.Info = LayoutEventScriptEditorTreeNodeIfString.GetDescription(ifStringElement);
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
        private void getIfNumberTreeNode(LayoutEvent e) {
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
        private void editIfNumberNode(LayoutEvent e) {
            var ifElement = Ensure.NotNull<XmlElement>(e.Sender, "ifElement");
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            Controls.EventScriptEditorDialogs.IfCondition ifDialog = new Controls.EventScriptEditorDialogs.IfCondition(ifElement, new IfNumberCustomizer());

            if (ifDialog.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [LayoutEvent("get-event-script-description", IfSender = "IfNumber")]
        private void getIfNumberDescription(LayoutEvent e) {
            var ifStringElement = Ensure.NotNull<XmlElement>(e.Sender, "ifStringElement");

            e.Info = LayoutEventScriptEditorTreeNodeIfNumber.GetDescription(ifStringElement);
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
        private void getIfBooleanTreeNode(LayoutEvent e) {
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
        private void editIfBooleanNode(LayoutEvent e) {
            var ifElement = Ensure.NotNull<XmlElement>(e.Sender, "ifElement");
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            Controls.EventScriptEditorDialogs.IfCondition ifDialog = new Controls.EventScriptEditorDialogs.IfCondition(ifElement, new IfBooleanCustomizer());

            if (ifDialog.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [LayoutEvent("get-event-script-description", IfSender = "IfBoolean")]
        private void getIfBooleanDescription(LayoutEvent e) {
            var ifStringElement = Ensure.NotNull<XmlElement>(e.Sender, "ifStringElement");

            e.Info = LayoutEventScriptEditorTreeNodeIfBoolean.GetDescription(ifStringElement);
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
        private void getIfDefinedTreeNode(LayoutEvent e) {
            var ifElement = Ensure.NotNull<XmlElement>(e.Sender, "ifElement");

            e.Info = new LayoutEventScriptEditorTreeNodeIfDefined(ifElement);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "IfDefined")]
        private void editIfDefinedNode(LayoutEvent e) {
            var ifElement = Ensure.NotNull<XmlElement>(e.Sender, "ifElement");
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            Controls.EventScriptEditorDialogs.IfDefined ifDialog = new Controls.EventScriptEditorDialogs.IfDefined(ifElement);

            if (ifDialog.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [LayoutEvent("get-event-script-description", IfSender = "IfDefined")]
        private void getIfDefinedDescription(LayoutEvent e) {
            var ifElement = Ensure.NotNull<XmlElement>(e.Sender, "ifElement");

            e.Info = LayoutEventScriptEditorTreeNodeIfDefined.GetDescription(ifElement);
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
        private void getIfTimeTreeNode(LayoutEvent e) {
            var ifElement = Ensure.NotNull<XmlElement>(e.Sender, "ifElement");

            e.Info = new LayoutEventScriptEditorTreeNodeIfTime(ifElement);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "IfTime")]
        private void editIfTimeNode(LayoutEvent e) {
            var ifElement = Ensure.NotNull<XmlElement>(e.Sender, "ifElement");
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            Controls.EventScriptEditorDialogs.IfTime ifDialog = new Controls.EventScriptEditorDialogs.IfTime(ifElement);

            if (ifDialog.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [LayoutEvent("get-event-script-description", IfSender = "IfTime")]
        private void getIfTimeDescription(LayoutEvent e) {
            var IfTimeElement = Ensure.NotNull<XmlElement>(e.Sender, "IfTimeElement");

            e.Info = LayoutEventScriptEditorTreeNodeIfTime.GetDescription(IfTimeElement);
        }

        public class LayoutEventScriptEditorTreeNodeIfTime : LayoutEventScriptEditorTreeNodeCondition {
            public LayoutEventScriptEditorTreeNodeIfTime(XmlElement conditionElement) : base(conditionElement) {
            }

            private static string? getNodesDescription(XmlElement element, string nodeName, string title) {
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

                if ((s = getNodesDescription(element, "DayOfWeek", "day of week is")) != null)
                    parts.Add(s);
                if ((s = getNodesDescription(element, "Hours", "hour is")) != null)
                    parts.Add((parts.Count > 0 ? " and " : "") + s);
                if ((s = getNodesDescription(element, "Minutes", "minute is")) != null)
                    parts.Add((parts.Count > 0 ? " and " : "") + s);
                if ((s = getNodesDescription(element, "Seconds", "second is")) != null)
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
        private void getShowMessageTreeNode(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");

            e.Info = new LayoutEventScriptEditorTreeNodeShowMessage(element);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "ShowMessage")]
        private void editShowMessageTreeNode(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            Controls.EventScriptEditorDialogs.ShowMessage d = new Controls.EventScriptEditorDialogs.ShowMessage(element);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [LayoutEvent("get-event-script-description", IfSender = "ShowMessage")]
        private void getShowMessageDescription(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");

            e.Info = LayoutEventScriptEditorTreeNodeShowMessage.GetDescription(element);
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
        private void getSetAttributeTreeNode(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");

            e.Info = new LayoutEventScriptEditorTreeNodeSetAttribute(element);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "SetAttribute")]
        private void editSetAttributeTreeNode(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            Controls.EventScriptEditorDialogs.SetAttribute d = new Controls.EventScriptEditorDialogs.SetAttribute(element);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [LayoutEvent("get-event-script-description", IfSender = "SetAttribute")]
        private void getSetAttributeDescription(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");

            e.Info = LayoutEventScriptEditorTreeNodeSetAttribute.GetDescription(element);
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
        private void getGenerateEventTreeNode(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");

            e.Info = new LayoutEventScriptEditorTreeNodeGenerateEvent(element);
        }

        [LayoutEvent("event-script-editor-edit-element", IfSender = "GenerateEvent")]
        private void editGenerateEventTreeNode(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");
            var site = Ensure.NotNull<IEventScriptEditorSite>(e.Info, "site");
            Controls.EventScriptEditorDialogs.GenerateEvent d = new Controls.EventScriptEditorDialogs.GenerateEvent(element);

            if (d.ShowDialog() == DialogResult.OK)
                site.EditingDone();
            else
                site.EditingCancelled();
        }

        [LayoutEvent("get-event-script-description", IfSender = "GenerateEvent")]
        private void getGenerateEventDescription(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");

            e.Info = LayoutEventScriptEditorTreeNodeGenerateEvent.GetDescription(element);
        }

        private class LayoutEventScriptEditorTreeNodeGenerateEvent : CommonUI.Controls.LayoutEventScriptEditorTreeNodeAction {
            public LayoutEventScriptEditorTreeNodeGenerateEvent(XmlElement conditionElement) : base(conditionElement) {
            }

            private static string? getArgumentDescription(XmlElement element, string prefix) {
                string getValueOf(string constant) =>
                    element.GetAttribute($"Type{prefix}") switch
                    {
                        "Boolean" => $"is boolean {constant}",
                        "String" => $"is string \"{constant}\"",
                        "Integer" => $"is integer {constant}",
                        "Double" => $"is double {constant}",
                        _ => $"is unkown type {constant}"
                    };

                return element.GetAttribute(prefix + "Type") switch
                {
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
                var argument = getArgumentDescription(element, "Sender");

                parts.Add("Generate Event " + element.GetAttribute("EventName"));

                if (argument != null)
                    parts.Add("Sender " + argument);

                argument = getArgumentDescription(element, "Info");

                if (argument != null)
                    parts.Add("Info " + argument);

                if (element["Options"] != null) {
                    XmlElement optionsElement = element["Options"];

                    if (optionsElement.ChildNodes.Count > 0) {
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

