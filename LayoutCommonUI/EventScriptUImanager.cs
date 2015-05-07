using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Xml;

using LayoutManager;
using LayoutManager.CommonUI.Controls;

namespace LayoutManager.CommonUI {

	[LayoutModule("Event Script UI Manager", UserControl=false)]
	public class EventScriptUImanager : LayoutModuleBase {

		#region Methods to return add menus for various section (events, condition and actions)

		[LayoutEvent("get-event-script-editor-event-container-menu")]
		private void eventContainerAddMenu(LayoutEvent e) {
			LayoutEventScriptEditorTreeNode	eventContainerNode = (LayoutEventScriptEditorTreeNode)e.Sender;
			Menu							menu = (Menu)e.Info;

			if(eventContainerNode.Element["Events"] == null)
				menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Events", "Events"));
			if(eventContainerNode.SupportConditions && eventContainerNode.Element["Condition"] == null)
				menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Condition", "Condition"));
			if(eventContainerNode.SupportActions && eventContainerNode.Element["Actions"] == null)
				menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Actions", "Actions"));
		}

		[LayoutEvent("get-event-script-editor-insert-event-container-menu")]
		private void getEventScriptEditorInsertEventConatinerMenu(LayoutEvent e) {
			Menu				menu = (Menu)e.Info;

			menu.MenuItems.Add(new Controls.EventScriptEditorInsertEventContainerMenuItem(e, "Any", "Any"));
			menu.MenuItems.Add(new Controls.EventScriptEditorInsertEventContainerMenuItem(e, "All", "All"));
			menu.MenuItems.Add(new Controls.EventScriptEditorInsertEventContainerMenuItem(e, "Sequence", "Sequence"));
			menu.MenuItems.Add(new Controls.EventScriptEditorInsertEventContainerMenuItem(e, "Repeat", "Repeat"));
			menu.MenuItems.Add(new Controls.EventScriptEditorInsertEventContainerMenuItem(e, "Task", "Task"));
		}

		[LayoutEvent("get-event-script-editor-event-menu")]
		private void eventAddMenu(LayoutEvent e) {
			LayoutEventScriptEditorTreeNode	eventNode = (LayoutEventScriptEditorTreeNode)e.Sender;
			Menu							menu = (Menu)e.Info;

			if(eventNode.SupportConditions && eventNode.Element["Condition"] == null)
				menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Condition", "Condition"));
			if(eventNode.SupportActions && eventNode.Element["Actions"] == null)
				menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Actions", "Actions"));
		}

		[LayoutEvent("get-event-script-editor-events-section-menu", Order=0)]
		private void addEventsSectionAddMenuWithEventContainers(LayoutEvent e) {
			Menu	menu = (Menu)e.Info;

			menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Any", "Any"));
			menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "All", "All"));
			menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Sequence", "Sequence"));
			menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Repeat", "Repeat"));
			menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Random Choice", "RandomChoice"));
			menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Task", "Task"));
		}

		[LayoutEvent("get-event-script-editor-events-section-menu", Order=100)]
		private void addEventsSectionAddMenuWithEvents(LayoutEvent e) {
			Menu	menu = (Menu)e.Info;

			if(menu.MenuItems.Count > 0)
				menu.MenuItems.Add("-");

			menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Do now", "DoNow"));
			menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Wait", "Wait"));
			menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Wait for Event", "WaitForEvent"));
		}

		[LayoutEvent("get-event-script-editor-random-choice-menu")]
		private void addRandomChoiceMenu(LayoutEvent e) {
			Menu		menu = (Menu)e.Info;

			menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Choice", "Choice"));
		}

		[LayoutEvent("get-event-script-editor-condition-section-menu")]
		private void addConditionSectionAddMenu(LayoutEvent e) {
			Menu			menu = (Menu)e.Info;

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
			Menu			menu = (Menu)e.Info;

			menu.MenuItems.Add(new Controls.EventScriptEditorInsertConditionContainerMenuItem(e, "And", "And"));
			menu.MenuItems.Add(new Controls.EventScriptEditorInsertConditionContainerMenuItem(e, "Or", "Or"));
			menu.MenuItems.Add(new Controls.EventScriptEditorInsertConditionContainerMenuItem(e, "Not", "Not"));
		}

		[LayoutEvent("get-event-script-editor-actions-section-menu")]
		private void getActionsSectionAddMenu(LayoutEvent e) {
			Menu			menu = (Menu)e.Info;

			menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Show message", "ShowMessage"));
			menu.MenuItems.Add(new Controls.EventScriptEditorAddMenuItem(e, "Set Attribute", "SetAttribute"));
		}


		#endregion

		#region Utilty methods for generating descriptions

		public static bool IsTrivialContainer(XmlElement containerElement) {
			XmlElement	eventsElement = containerElement["Events"];

			return eventsElement != null && eventsElement.ChildNodes.Count == 1 && containerElement["Condition"] == null && containerElement["Actions"] == null;
		}

        public static string GetElementDescription(XmlElement element) => (string)EventManager.Event(new LayoutEvent(element, "get-event-script-description"));

        public static string GetActionsDescription(XmlElement actionsElement) {
			string[]	actions = new string[actionsElement.ChildNodes.Count];

			int	i = 0;
			foreach(XmlElement actionElement in actionsElement) {
				actions[i] = ((i > 0) ? "; " : "") + GetElementDescription(actionElement);
				i++;
			}

			return String.Concat(actions);
		}

		public static string GetEventOrEventContainerDescription(XmlElement element, string prefix, string suffix) {
			ArrayList	substrings = new ArrayList();

			if(IsTrivialContainer(element)) {
				element = (XmlElement)element.ChildNodes[0].ChildNodes[0];
				return (string)GetElementDescription(element);
			}
			else {
				substrings.Add(prefix);

				foreach(XmlElement childElement in element) {
					if(childElement.Name == "Events") {
						substrings.Add(" { ");
						foreach(XmlElement eventElement in childElement)
							substrings.Add(GetElementDescription(eventElement));
						substrings.Add(" } ");
					}
					else if(childElement.Name == "Condition") {
						if(substrings.Count > 0)
							substrings.Add(",");
						substrings.Add(" Condition { " + GetElementDescription((XmlElement)childElement.ChildNodes[0]) + " }");
					}
					else if(childElement.Name == "Actions") {
						if(substrings.Count > 0)
							substrings.Add(",");
						substrings.Add(" Actions { " + GetActionsDescription(childElement) + " } ");
					}
					else
						substrings.Add(GetElementDescription(childElement));
				}

				if(suffix != null)
					substrings.Add(suffix);
				
				return String.Concat((string[] )substrings.ToArray(typeof(string)));
			}
		}

        public static string GetEventOrEventContainerDescription(XmlElement element, string prefix) => GetEventOrEventContainerDescription(element, prefix, null);

        public static string GetEventOrEventContainerDescription(LayoutEvent e, string prefix, string suffix) => GetEventOrEventContainerDescription((XmlElement)e.Sender, prefix, suffix);

        public static string GetEventOrEventContainerDescription(LayoutEvent e, string prefix) => GetEventOrEventContainerDescription(e, prefix, null);

        public static void getConditionContainerDescription(LayoutEvent e, string containerName) {
			XmlElement	containerElement = (XmlElement)e.Sender;

			if(containerElement.ChildNodes.Count == 1)		// If there is only one some condition, return its description
				e.Info = GetElementDescription((XmlElement)containerElement.ChildNodes[0]);
			else {
				string[]	conditions = new string[containerElement.ChildNodes.Count];

				int	i = 0;
				foreach(XmlElement conditionElement in containerElement) {
					conditions[i] = ((i == 0) ? "" : " " + containerName + " ") + GetElementDescription(conditionElement);
					i++;
				}

				e.Info = "(" + string.Concat(conditions) + ")";
			}
		}

		#endregion

		#region Tree nodes for event container sections

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="Events")]
		private void getEventsSectionTreeNode(LayoutEvent e) {
			XmlElement	eventsElement = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeEventsSection(eventsElement);
		}

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="Condition")]
		private void getConditionSectionTreeNode(LayoutEvent e) {
			XmlElement	conditionElement = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeConditionSection(conditionElement);
		}

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="Actions")]
		private void getActionsSectionTreeNode(LayoutEvent e) {
			XmlElement	actionsElement = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeActionsSection(actionsElement);
		}

		#endregion

		#region Event Containers

		#region Sequence

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="Sequence")]
		private void getSequenceTreeNode(LayoutEvent e) {
			XmlElement	sequenceElement = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeSequence(sequenceElement);
		}

		[LayoutEvent("get-event-script-description", IfSender="Sequence")]
		private void getSequenceDescription(LayoutEvent e) {
			e.Info = GetEventOrEventContainerDescription(e, "Sequence");
		}

		class LayoutEventScriptEditorTreeNodeSequence : LayoutEventScriptEditorTreeNodeEventContainer {
			public LayoutEventScriptEditorTreeNodeSequence(XmlElement conditionElement) : base(conditionElement) {
				AddChildEventScriptTreeNodes();
			}

            protected override string Description => "Sequence";

            public override bool ShouldInsertAsFirst(XmlElement elementToInsert) {
				if(elementToInsert.Name == "Condition")
					return true;
				return false;
			}
		}

		#endregion

		#region All

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="All")]
		private void getAllTreeNode(LayoutEvent e) {
			XmlElement	allElement = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeAll(allElement);
		}

		[LayoutEvent("get-event-script-description", IfSender="All")]
		private void getAllDescription(LayoutEvent e) {
			e.Info = GetEventOrEventContainerDescription(e, "All");
		}

		class LayoutEventScriptEditorTreeNodeAll : LayoutEventScriptEditorTreeNodeEventContainer {
			public LayoutEventScriptEditorTreeNodeAll(XmlElement conditionElement) : base(conditionElement) {
				AddChildEventScriptTreeNodes();
			}

            protected override string Description => "All";

            public override bool CanHaveOptionalSubItems => true;
        }

		#endregion

		#region Any

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="Any")]
		private void getAnyTreeNode(LayoutEvent e) {
			XmlElement	anyElement = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeAny(anyElement);
		}

		[LayoutEvent("get-event-script-description", IfSender="Any")]
		private void getAnyDescription(LayoutEvent e) {
			e.Info = GetEventOrEventContainerDescription(e, "Any");
		}

		class LayoutEventScriptEditorTreeNodeAny : LayoutEventScriptEditorTreeNodeEventContainer {
			public LayoutEventScriptEditorTreeNodeAny(XmlElement conditionElement) : base(conditionElement) {
				AddChildEventScriptTreeNodes();
			}

            protected override string Description => "Any";

            public override bool CanHaveOptionalSubItems => true;
        }

		#endregion

		#region Repeat

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="Repeat")]
		private void getRepeatTreeNode(LayoutEvent e) {
			XmlElement	repeatElement = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeRepeat(repeatElement);
		}

		[LayoutEvent("event-script-editor-edit-element", IfSender="Repeat")]
		private void editRepeatTreeNode(LayoutEvent e) {
			IEventScriptEditorSite								site = (IEventScriptEditorSite)e.Info;
			Controls.EventScriptEditorDialogs.RepeatCondition	repeatDialog = new Controls.EventScriptEditorDialogs.RepeatCondition((XmlElement)e.Sender);

			if(repeatDialog.ShowDialog() == DialogResult.OK)
				site.EditingDone();
			else
				site.EditingCancelled();
		}

		[LayoutEvent("get-event-script-description", IfSender="Repeat")]
		private void getRepeatDescription(LayoutEvent e) {
			XmlElement	conditionElement = (XmlElement)e.Sender;
			XmlElement	repeatedConditionElement = (XmlElement)conditionElement.ChildNodes[0];

			int		count = XmlConvert.ToInt32(conditionElement.GetAttribute("Count"));

			e.Info = GetEventOrEventContainerDescription(e, LayoutEventScriptEditorTreeNodeRepeat.GetDescription(conditionElement)  + " { ", " } ");
		}

		class LayoutEventScriptEditorTreeNodeRepeat : LayoutEventScriptEditorTreeNodeMayBeOptional {
			public LayoutEventScriptEditorTreeNodeRepeat(XmlElement conditionElement) : base(conditionElement) {
				AddChildEventScriptTreeNodes();
			}

            public override string AddNodeEventName => "get-event-script-editor-events-section-menu";

            public override string InsertNodeEventName => "get-event-script-editor-insert-event-container-menu";

            protected override int IconIndex => IconEvent;

            static public string GetDescription(XmlElement element) {
				int	count = XmlConvert.ToInt32(element.GetAttribute("Count"));

				if(count < 0)
					return "Repeat";
				else if(count == 1)
					return "Repeat once";
				else
					return "Repeat " + count + " times";
			}

            protected override string Description => GetDescription(Element);

            /// <summary>
            /// Repeat can have no more than one subcondition
            /// </summary>
            public override int MaxSubNodes => 1;

            public override Controls.LayoutEventScriptEditorTreeNode NodeToEdit => this;        // repeat rule can be edited (changing the count)

            public override bool ShouldInsertAsFirst(XmlElement elementToInsert) {
				if(elementToInsert.Name == "Condition")
					return true;
				return false;
			}
		}

		#endregion

		#region Random Choice

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="RandomChoice")]
		private void getRandomChoiceTreeNode(LayoutEvent e) {
			XmlElement	element = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeRandomChoice(element);
		}

		[LayoutEvent("get-event-script-description", IfSender="RandomChoice")]
		private void getRandomChoiceDescription(LayoutEvent e) {
			XmlElement	element = (XmlElement)e.Sender;
			ArrayList	substrings = new ArrayList();

			substrings.Add("Random-Choice [ ");

			foreach(XmlElement choiceElement in element.GetElementsByTagName("Choice")) {
				int	weight = XmlConvert.ToInt32(choiceElement.GetAttribute("Weight"));

				substrings.Add("Choice (" + weight + "): { " + GetElementDescription((XmlElement)choiceElement.ChildNodes[0]) + " } ");
			}

			substrings.Add("] ");

			e.Info = string.Concat((string[] )substrings.ToArray(typeof(string)));
		}

		class LayoutEventScriptEditorTreeNodeRandomChoice : LayoutEventScriptEditorTreeNodeMayBeOptional {
			public LayoutEventScriptEditorTreeNodeRandomChoice(XmlElement element) : base(element) {
				AddChildEventScriptTreeNodes();
			}

            public override string AddNodeEventName => "get-event-script-editor-random-choice-menu";

            protected override string Description => "Random-Choice";

            protected override int IconIndex => IconEvent;

            public override Controls.LayoutEventScriptEditorTreeNode NodeToEdit => null;

        }

		#endregion

		#region Choice Entry

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="Choice")]
		private void getRandomChoiceEntryTreeNode(LayoutEvent e) {
			XmlElement	element = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeRandomChoiceEntry(element);
		}

		[LayoutEvent("event-script-editor-edit-element", IfSender="Choice")]
		private void editRandomChoiceEntryTreeNode(LayoutEvent e) {
			IEventScriptEditorSite	site = (IEventScriptEditorSite)e.Info;
			string					weightText = CommonUI.Dialogs.InputBox.Show("Choice Weight", "Please enter the weight (probability) for this choice", CommonUI.Dialogs.InputBoxValidationOptions.IntegerNumber);

			if(weightText == null)
				site.EditingCancelled();
			else {
				XmlElement	element = (XmlElement)e.Sender;

				element.SetAttribute("Weight", XmlConvert.ToString(int.Parse(weightText)));
				site.EditingDone();
			}
		}

		class LayoutEventScriptEditorTreeNodeRandomChoiceEntry : LayoutEventScriptEditorTreeNode {
			public LayoutEventScriptEditorTreeNodeRandomChoiceEntry(XmlElement element) : base(element) {
				AddChildEventScriptTreeNodes();
			}

            public override string AddNodeEventName => "get-event-script-editor-events-section-menu";

            protected override string Description => "Choice (" + Element.GetAttribute("Weight") + ")";

            protected override int IconIndex => IconEvent;

            public override int MinSubNodes => 1;

            public override int MaxSubNodes => 1;

            public override Controls.LayoutEventScriptEditorTreeNode NodeToEdit => this;        // Choice can be edited
        }

		#endregion

		#region Task

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="Task")]
		private void getTaskTreeNode(LayoutEvent e) {
			XmlElement	taskElement = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeTask(taskElement);
		}

		[LayoutEvent("event-script-editor-edit-element", IfSender="Task")]
		private void editTaskTreeNode(LayoutEvent e) {
			IEventScriptEditorSite								site = (IEventScriptEditorSite)e.Info;
//			Controls.EventScriptEditorDialogs.RepeatCondition	repeatDialog = new Controls.EventScriptEditorDialogs.RepeatCondition((XmlElement)e.Sender);

//			if(repeatDialog.ShowDialog() == DialogResult.OK)
//				site.EditingDone();
//			else
//				site.EditingCancelled();
			site.EditingDone();
		}

		[LayoutEvent("get-event-script-description", IfSender="Task")]
		private void getTaskDescription(LayoutEvent e) {
			XmlElement	element = (XmlElement)e.Sender;
			XmlElement	taskElement = (XmlElement)element.ChildNodes[0];

			e.Info = GetEventOrEventContainerDescription(e, LayoutEventScriptEditorTreeNodeTask.GetDescription(taskElement)  + " { ", " } ");
		}

		class LayoutEventScriptEditorTreeNodeTask : LayoutEventScriptEditorTreeNode {
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

            public override Controls.LayoutEventScriptEditorTreeNode NodeToEdit => this;        // repeat rule can be edited (changing the count)

            public override bool ShouldInsertAsFirst(XmlElement elementToInsert) {
				if(elementToInsert.Name == "Condition")
					return true;
				return false;
			}
		}

		#endregion

		#endregion

		#region Events

		#region Wait

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="Wait")]
		private void getWaitTreeNode(LayoutEvent e) {
			XmlElement	waitElement = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeWait(waitElement);
		}

		[LayoutEvent("event-script-editor-edit-element", IfSender="Wait")]
		private void editWaitTreeNode(LayoutEvent e) {
			IEventScriptEditorSite							site = (IEventScriptEditorSite)e.Info;
			Controls.EventScriptEditorDialogs.WaitCondition	waitDialog = new Controls.EventScriptEditorDialogs.WaitCondition((XmlElement)e.Sender);

			if(waitDialog.ShowDialog() == DialogResult.OK)
				site.EditingDone();
			else
				site.EditingCancelled();
		}

		[LayoutEvent("get-event-script-description", IfSender="Wait")]
		private void getWaitDescription(LayoutEvent e) {
			e.Info = GetEventOrEventContainerDescription(e, LayoutEventScriptEditorTreeNodeWait.GetWaitDescription((XmlElement)e.Sender));
		}

		class LayoutEventScriptEditorTreeNodeWait : Controls.LayoutEventScriptEditorTreeNodeEvent {

			public LayoutEventScriptEditorTreeNodeWait(XmlElement conditionElement) : base(conditionElement) {
				AddChildEventScriptTreeNodes();
			}

			private static void addWaitString(XmlElement conditionElement, ArrayList list, string attrName, string unitSingle, string unitPlural) {
				if(conditionElement.HasAttribute(attrName)) {
					int	v = XmlConvert.ToInt32(conditionElement.GetAttribute(attrName));

					if(v != 0)
						list.Add((list.Count != 0 ? ", " : "") + v.ToString() + " " + ((v == 1) ? unitSingle : unitPlural));
				}
			}

			public static string GetWaitDescription(XmlElement conditionElement) {
				ArrayList	delayStrings = new ArrayList();
				string		s;

				addWaitString(conditionElement, delayStrings, "Minutes", "minute", "minutes");
				addWaitString(conditionElement, delayStrings, "Seconds", "second", "seconds");
				addWaitString(conditionElement, delayStrings, "MilliSeconds", "milli-second", "milli-seconds");

				s = "Wait for " + String.Concat((string[])delayStrings.ToArray(typeof(string)));

				if(conditionElement.HasAttribute("RandomSeconds"))
					s += " plus random time upto " + XmlConvert.ToInt32(conditionElement.GetAttribute("RandomSeconds")) + " seconds";

				return s;
			}

            protected override string Description => GetWaitDescription(Element);
        }

		#endregion

		#region DoNow

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="DoNow")]
		private void getDoNowTreeNode(LayoutEvent e) {
			XmlElement	element = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeDoNow(element);
		}

		[LayoutEvent("get-event-script-description", IfSender="DoNow")]
		private void getDoNowDescription(LayoutEvent e) {
			e.Info = GetEventOrEventContainerDescription(e, LayoutEventScriptEditorTreeNodeDoNow.GetDescription((XmlElement)e.Sender));
		}

		class LayoutEventScriptEditorTreeNodeDoNow : Controls.LayoutEventScriptEditorTreeNodeEvent {

			public LayoutEventScriptEditorTreeNodeDoNow(XmlElement conditionElement) : base(conditionElement) {
				AddChildEventScriptTreeNodes();
			}

            public static string GetDescription(XmlElement element) => "Do now";

            public override Controls.LayoutEventScriptEditorTreeNode NodeToEdit => null;		// Nothing to edit

            protected override string Description => GetDescription(Element);
        }

		#endregion

		#region Wait for event

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="WaitForEvent")]
		private void getWaitForEventTreeNode(LayoutEvent e) {
			XmlElement	waitElement = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeWaitForEvent(waitElement);
		}

		[LayoutEvent("event-script-editor-edit-element", IfSender="WaitForEvent")]
		private void editWaitForEventTreeNode(LayoutEvent e) {
			XmlElement										waitElement = (XmlElement)e.Sender;
			IEventScriptEditorSite							site = (IEventScriptEditorSite)e.Info;
			Controls.EventScriptEditorDialogs.WaitForEvent	d = new Controls.EventScriptEditorDialogs.WaitForEvent(waitElement); 

			if(d.ShowDialog() == DialogResult.OK)
				site.EditingDone();
			else
				site.EditingCancelled();
		}

		[LayoutEvent("get-event-script-description", IfSender="WaitForEvent")]
		private void getWaitForEventDescription(LayoutEvent e) {
			e.Info = GetEventOrEventContainerDescription(e, LayoutEventScriptEditorTreeNodeWaitForEvent.GetDescription((XmlElement)e.Sender));
		}

		class LayoutEventScriptEditorTreeNodeWaitForEvent : Controls.LayoutEventScriptEditorTreeNodeEvent {

			public LayoutEventScriptEditorTreeNodeWaitForEvent(XmlElement conditionElement) : base(conditionElement) {
				AddChildEventScriptTreeNodes();
			}

			public static string GetDescription(XmlElement element) {
				string	name = element.GetAttribute("Name");

				return "Wait for event " + name;
			}

            protected override string Description => GetDescription(Element);
        }

		#endregion

		#endregion

		#region Condition Containers

		#region And

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="And")]
		private void getAndTreeNode(LayoutEvent e) {
			XmlElement	AndElement = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeAnd(AndElement);
		}

		[LayoutEvent("get-event-script-description", IfSender="And")]
		private void getAndDescription(LayoutEvent e) {
			getConditionContainerDescription(e, "And");
		}

		class LayoutEventScriptEditorTreeNodeAnd : LayoutEventScriptEditorTreeNodeConditionContainer {
			public LayoutEventScriptEditorTreeNodeAnd(XmlElement andElement) : base(andElement) {
				AddChildEventScriptTreeNodes();
			}


            protected override string Description => "And";
        }

		#endregion

		#region Or

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="Or")]
		private void getOrTreeNode(LayoutEvent e) {
			XmlElement	OrElement = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeOr(OrElement);
		}

		[LayoutEvent("get-event-script-description", IfSender="Or")]
		private void getOrDescription(LayoutEvent e) {
			getConditionContainerDescription(e, "Or");
		}

		class LayoutEventScriptEditorTreeNodeOr : LayoutEventScriptEditorTreeNodeConditionContainer {
			public LayoutEventScriptEditorTreeNodeOr(XmlElement orElement) : base(orElement) {
				AddChildEventScriptTreeNodes();
			}


            protected override string Description => "Or";
        }

		#endregion

		#region Not

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="Not")]
		private void getNotTreeNode(LayoutEvent e) {
			XmlElement	NotElement = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditnotTreeNodeNot(NotElement);
		}

		[LayoutEvent("get-event-script-description", IfSender="Not")]
		private void getNotDescription(LayoutEvent e) {
			XmlElement	notElement = (XmlElement)e.Sender;

			e.Info = "Not " + GetElementDescription((XmlElement)notElement.ChildNodes[0]);
		}

		class LayoutEventScriptEditnotTreeNodeNot : LayoutEventScriptEditorTreeNodeConditionContainer {
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
			public LayoutEventScriptEditorTreeNodeIf(XmlElement conditionElement) : base(conditionElement) {
			}
		}

		#endregion

		#region If (String)

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="IfString")]
		private void getIfStringTreeNode(LayoutEvent e) {
			XmlElement	ifElement = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeIfString(ifElement);
		}

		class IfStringCustomizer : Controls.EventScriptEditorDialogs.IIfConditionDialogCustomizer {
            public string Title => "If (string)";

            public string[] OperatorNames => new string[] { "Equal", "NotEqual", "Match" };

            public string[] OperatorDescriptions => new string[] { "=", "<>", "Match" };

            public Type[] AllowedTypes => new Type[] {
                                          typeof(string),
                                          typeof(Enum),
                    };

            public bool ValueIsBoolean => false;
        }

		[LayoutEvent("event-script-editor-edit-element", IfSender="IfString")]
		private void editIfStringNode(LayoutEvent e) {
			XmlElement										ifElement = (XmlElement)e.Sender;
			IEventScriptEditorSite							site = (IEventScriptEditorSite)e.Info;
			Controls.EventScriptEditorDialogs.IfCondition	ifDialog = new Controls.EventScriptEditorDialogs.IfCondition(ifElement, new IfStringCustomizer());

			if(ifDialog.ShowDialog() == DialogResult.OK)
				site.EditingDone();
			else
				site.EditingCancelled();
		}

		[LayoutEvent("get-event-script-description", IfSender="IfString")]
		private void getIfStringDescription(LayoutEvent e) {
			XmlElement	ifStringElement = (XmlElement)e.Sender;

			e.Info = LayoutEventScriptEditorTreeNodeIfString.GetDescription(ifStringElement);
		}

		public class LayoutEventScriptEditorTreeNodeIfString : LayoutEventScriptEditorTreeNodeIf {
			public LayoutEventScriptEditorTreeNodeIfString(XmlElement ifStringElement) : base(ifStringElement) {
			}

			public static string GetDescription(XmlElement element) {
				string	operatorName = (string)EventManager.Event(new LayoutEvent(element, "get-event-script-operator-name"));

				return "If (string) " + GetOperandDescription(element, "1", typeof(string)) + " " + operatorName + " " + GetOperandDescription(element, "2", typeof(string));
			}

            protected override string Description => GetDescription(Element);
        }

		#endregion

		#region If (Number)

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="IfNumber")]
		private void getIfNumberTreeNode(LayoutEvent e) {
			XmlElement	ifElement = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeIfNumber(ifElement);
		}

		class IfNumberCustomizer : Controls.EventScriptEditorDialogs.IIfConditionDialogCustomizer {
            public string Title => "If (number)";

            public string[] OperatorNames => new string[] { "eq", "ne", "gt", "ge", "le", "lt" };

            public string[] OperatorDescriptions => new string[] { "=", "<>", ">", ">=", "=<", "<" };

            public Type[] AllowedTypes => new Type[] {
                                          typeof(int),
                    };

            public bool ValueIsBoolean => false;
        }

		[LayoutEvent("event-script-editor-edit-element", IfSender="IfNumber")]
		private void editIfNumberNode(LayoutEvent e) {
			XmlElement										ifElement = (XmlElement)e.Sender;
			IEventScriptEditorSite							site = (IEventScriptEditorSite)e.Info;
			Controls.EventScriptEditorDialogs.IfCondition	ifDialog = new Controls.EventScriptEditorDialogs.IfCondition(ifElement, new IfNumberCustomizer());

			if(ifDialog.ShowDialog() == DialogResult.OK)
				site.EditingDone();
			else
				site.EditingCancelled();
		}

		[LayoutEvent("get-event-script-description", IfSender="IfNumber")]
		private void getIfNumberDescription(LayoutEvent e) {
			XmlElement	ifStringElement = (XmlElement)e.Sender;

			e.Info = LayoutEventScriptEditorTreeNodeIfNumber.GetDescription(ifStringElement);
		}

		public class LayoutEventScriptEditorTreeNodeIfNumber : LayoutEventScriptEditorTreeNodeIf {
			public LayoutEventScriptEditorTreeNodeIfNumber(XmlElement ifStringElement) : base(ifStringElement) {
			}

			public static string GetDescription(XmlElement element) {
				string	operatorName = (string)EventManager.Event(new LayoutEvent(element, "get-event-script-operator-name"));

				return "If (number) " + GetOperandDescription(element, "1", typeof(int)) + " " + operatorName + " " + GetOperandDescription(element, "2", typeof(int));
			}

            protected override string Description => GetDescription(Element);
        }

		#endregion

		#region If (Boolean)

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="IfBoolean")]
		private void getIfBooleanTreeNode(LayoutEvent e) {
			XmlElement	ifElement = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeIfBoolean(ifElement);
		}

		class IfBooleanCustomizer : Controls.EventScriptEditorDialogs.IIfConditionDialogCustomizer {
            public string Title => "If (Boolean)";

            public string[] OperatorNames => new string[] { "Equal", "NotEqual" };

            public string[] OperatorDescriptions => new string[] { "=", "<>" };

            public Type[] AllowedTypes => new Type[] {
                                          typeof(bool),
                    };

            public bool ValueIsBoolean => true;
        }

		[LayoutEvent("event-script-editor-edit-element", IfSender="IfBoolean")]
		private void editIfBooleanNode(LayoutEvent e) {
			XmlElement										ifElement = (XmlElement)e.Sender;
			IEventScriptEditorSite							site = (IEventScriptEditorSite)e.Info;
			Controls.EventScriptEditorDialogs.IfCondition	ifDialog = new Controls.EventScriptEditorDialogs.IfCondition(ifElement, new IfBooleanCustomizer());

			if(ifDialog.ShowDialog() == DialogResult.OK)
				site.EditingDone();
			else
				site.EditingCancelled();
		}

		[LayoutEvent("get-event-script-description", IfSender="IfBoolean")]
		private void getIfBooleanDescription(LayoutEvent e) {
			XmlElement	ifStringElement = (XmlElement)e.Sender;

			e.Info = LayoutEventScriptEditorTreeNodeIfBoolean.GetDescription(ifStringElement);
		}

		public class LayoutEventScriptEditorTreeNodeIfBoolean : LayoutEventScriptEditorTreeNodeIf {
			public LayoutEventScriptEditorTreeNodeIfBoolean(XmlElement ifBooleanElement) : base(ifBooleanElement) {
			}

			public static string GetDescription(XmlElement element) {
				string	operatorName = (string)EventManager.Event(new LayoutEvent(element, "get-event-script-operator-name"));

				return "If (Boolean) " + GetOperandDescription(element, "1", typeof(bool)) + " " + operatorName + " " + GetOperandDescription(element, "2", typeof(bool));
			}

            protected override string Description => GetDescription(Element);
        }

		#endregion

		#region If (Defined)

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="IfDefined")]
		private void getIfDefinedTreeNode(LayoutEvent e) {
			XmlElement	ifElement = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeIfDefined(ifElement);
		}

		[LayoutEvent("event-script-editor-edit-element", IfSender="IfDefined")]
		private void editIfDefinedNode(LayoutEvent e) {
			XmlElement										ifElement = (XmlElement)e.Sender;
			IEventScriptEditorSite							site = (IEventScriptEditorSite)e.Info;
			Controls.EventScriptEditorDialogs.IfDefined		ifDialog = new Controls.EventScriptEditorDialogs.IfDefined(ifElement);

			if(ifDialog.ShowDialog() == DialogResult.OK)
				site.EditingDone();
			else
				site.EditingCancelled();
		}

		[LayoutEvent("get-event-script-description", IfSender="IfDefined")]
		private void getIfDefinedDescription(LayoutEvent e) {
			XmlElement	ifElement = (XmlElement)e.Sender;

			e.Info = LayoutEventScriptEditorTreeNodeIfDefined.GetDescription(ifElement);
		}

		public class LayoutEventScriptEditorTreeNodeIfDefined : LayoutEventScriptEditorTreeNodeIf {
			public LayoutEventScriptEditorTreeNodeIfDefined(XmlElement ifElement) : base(ifElement) {
			}

			public static string GetDescription(XmlElement element) {
				string	symbol = element.GetAttribute("Symbol");

				if(element.HasAttribute("Attribute")) {
					string	attribute = element.GetAttribute("Attribute");

					return "If defined attribute " + attribute + " of " + symbol;
				}
				else
					return "If defined symbol " + symbol;
			}

            protected override string Description => GetDescription(Element);
        }

		#endregion

		#region If (Time)

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="IfTime")]
		private void getIfTimeTreeNode(LayoutEvent e) {
			XmlElement	ifElement = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeIfTime(ifElement);
		}

		[LayoutEvent("event-script-editor-edit-element", IfSender="IfTime")]
		private void editIfTimeNode(LayoutEvent e) {
			XmlElement										ifElement = (XmlElement)e.Sender;
			IEventScriptEditorSite							site = (IEventScriptEditorSite)e.Info;
			Controls.EventScriptEditorDialogs.IfTime		ifDialog = new Controls.EventScriptEditorDialogs.IfTime(ifElement);

			if(ifDialog.ShowDialog() == DialogResult.OK)
				site.EditingDone();
			else
				site.EditingCancelled();
		}

		[LayoutEvent("get-event-script-description", IfSender="IfTime")]
		private void getIfTimeDescription(LayoutEvent e) {
			XmlElement	IfTimeElement = (XmlElement)e.Sender;

			e.Info = LayoutEventScriptEditorTreeNodeIfTime.GetDescription(IfTimeElement);
		}

		public class LayoutEventScriptEditorTreeNodeIfTime : LayoutEventScriptEditorTreeNodeCondition {
			public LayoutEventScriptEditorTreeNodeIfTime(XmlElement conditionElement) : base(conditionElement) {
			}

			static string getNodesDescription(XmlElement element, string nodeName, string title) {
				IIfTimeNode[]	nodes = (IIfTimeNode[])EventManager.Event(new LayoutEvent(element, "parse-if-time-element", null, nodeName));
				string			d = null;

				if(nodes.Length > 0) {
					bool	first = true;

					d = title + " ";
					foreach(IIfTimeNode node in nodes) {
						if(!first)
							d += " or ";
						d += node.Description;
						first = false;
					}
				}

				return d;
			}

			public static string GetDescription(XmlElement element) {
				ArrayList	parts = new ArrayList();
				string		s;

				if((s = getNodesDescription(element, "DayOfWeek", "day of week is")) != null)
					parts.Add(s);
				if((s = getNodesDescription(element, "Hours", "hour is")) != null)
					parts.Add((parts.Count > 0 ? " and " : "") + s);
				if((s = getNodesDescription(element, "Minutes", "minute is")) != null)
					parts.Add((parts.Count > 0 ? " and " : "") + s);
				if((s = getNodesDescription(element, "Seconds", "second is")) != null)
					parts.Add((parts.Count > 0 ? " and " : "") + s);

				return string.Concat((string[] )parts.ToArray(typeof(string)));
			}

            protected override string Description => GetDescription(Element);
        }

		#endregion

		#endregion

		#region Actions

		#region Show Message

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="ShowMessage")]
		private void getShowMessageTreeNode(LayoutEvent e) {
			XmlElement	element = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeShowMessage(element);
		}

		[LayoutEvent("event-script-editor-edit-element", IfSender="ShowMessage")]
		private void editShowMessageTreeNode(LayoutEvent e) {
			XmlElement										element = (XmlElement)e.Sender;
			IEventScriptEditorSite							site = (IEventScriptEditorSite)e.Info;
			Controls.EventScriptEditorDialogs.ShowMessage	d = new Controls.EventScriptEditorDialogs.ShowMessage(element); 

			if(d.ShowDialog() == DialogResult.OK)
				site.EditingDone();
			else
				site.EditingCancelled();
		}

		[LayoutEvent("get-event-script-description", IfSender="ShowMessage")]
		private void getShowMessageDescription(LayoutEvent e) {
			e.Info = LayoutEventScriptEditorTreeNodeShowMessage.GetDescription((XmlElement)e.Sender);
		}

		class LayoutEventScriptEditorTreeNodeShowMessage : LayoutEventScriptEditorTreeNodeAction {
			public LayoutEventScriptEditorTreeNodeShowMessage(XmlElement conditionElement) : base(conditionElement) {
				AddChildEventScriptTreeNodes();
			}

			public static string GetDescription(XmlElement element) {
				string	name = element.GetAttribute("Name");
				string	messageType = "message";

				switch(element.GetAttribute("MessageType")) {
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

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="SetAttribute")]
		private void getSetAttributeTreeNode(LayoutEvent e) {
			XmlElement	element = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeSetAttribute(element);
		}

		[LayoutEvent("event-script-editor-edit-element", IfSender="SetAttribute")]
		private void editSetAttributeTreeNode(LayoutEvent e) {
			XmlElement										element = (XmlElement)e.Sender;
			IEventScriptEditorSite							site = (IEventScriptEditorSite)e.Info;
			Controls.EventScriptEditorDialogs.SetAttribute	d = new Controls.EventScriptEditorDialogs.SetAttribute(element); 

			if(d.ShowDialog() == DialogResult.OK)
				site.EditingDone();
			else
				site.EditingCancelled();
		}

		[LayoutEvent("get-event-script-description", IfSender="SetAttribute")]
		private void getSetAttributeDescription(LayoutEvent e) {
			e.Info = LayoutEventScriptEditorTreeNodeSetAttribute.GetDescription((XmlElement)e.Sender);
		}

		class LayoutEventScriptEditorTreeNodeSetAttribute : LayoutEventScriptEditorTreeNodeAction {
			public LayoutEventScriptEditorTreeNodeSetAttribute(XmlElement conditionElement) : base(conditionElement) {
				AddChildEventScriptTreeNodes();
			}

			public static string GetDescription(XmlElement element) {
				string	symbol = element.GetAttribute("Symbol");
				string	attribute = element.GetAttribute("Attribute");
				string	setPrefix = "Set attribute " + attribute + " of " + symbol + " to ";

				switch(element.GetAttribute("SetTo")) {
					case "Text":
						return setPrefix + "\"" + element.GetAttribute("Value") + "\"";

					case "Boolean":
						return setPrefix + element.GetAttribute("Value");

					case "Number":
						if(element.GetAttribute("Op") == "Add")
							return "Add " + element.GetAttribute("Value") + " to the value of attribute " + attribute + " of symbol " + symbol;
						else
							return setPrefix + element.GetAttribute("Value");

					case "ValueOf":
						string	access = element.GetAttribute("SymbolToAccess");
						string	valueOfSymbol = element.GetAttribute("SymbolTo");
						string	valueOfName = element.GetAttribute("NameTo");

						if(access == "Property")
							return setPrefix + "value of property " + valueOfName + " of symbol " + valueOfSymbol;
						else
							return setPrefix + "value of attribute " + valueOfName + " of symbol " + valueOfSymbol;

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

		[LayoutEvent("get-event-script-editor-tree-node", IfSender="GenerateEvent")]
		private void getGenerateEventTreeNode(LayoutEvent e) {
			XmlElement	element = (XmlElement)e.Sender;

			e.Info = new LayoutEventScriptEditorTreeNodeGenerateEvent(element);
		}

		[LayoutEvent("event-script-editor-edit-element", IfSender="GenerateEvent")]
		private void editGenerateEventTreeNode(LayoutEvent e) {
			XmlElement										element = (XmlElement)e.Sender;
			IEventScriptEditorSite							site = (IEventScriptEditorSite)e.Info;
			Controls.EventScriptEditorDialogs.GenerateEvent	d = new Controls.EventScriptEditorDialogs.GenerateEvent(element);

			if(d.ShowDialog() == DialogResult.OK)
				site.EditingDone();
			else
				site.EditingCancelled();
		}

		[LayoutEvent("get-event-script-description", IfSender="GenerateEvent")]
		private void getGenerateEventDescription(LayoutEvent e) {
			e.Info = LayoutEventScriptEditorTreeNodeGenerateEvent.GetDescription((XmlElement)e.Sender);
		}

		class LayoutEventScriptEditorTreeNodeGenerateEvent : CommonUI.Controls.LayoutEventScriptEditorTreeNodeAction {
			public LayoutEventScriptEditorTreeNodeGenerateEvent(XmlElement conditionElement) : base(conditionElement) {
			}

			static string getArgumentDescription(XmlElement element, string prefix) {
				switch(element.GetAttribute(prefix + "Type")) {

					case "Null":
					default:
						return null;

					case "Reference":
						return "is " + element.GetAttribute(prefix + "SymbolName");

					case "ValueOf":
						if(element.GetAttribute("Symbol" + prefix + "Access") == "Value") {
							string	constant = element.GetAttribute("Value" + prefix);

							switch(element.GetAttribute("Type" + prefix)) {
								case "Boolean":
									return "is boolean " + constant;

								case "String":
									return "is string \"" + constant + "\"";

								case "Integer":
									return "is integer " + constant;

								case "Double":
									return "is double " + constant;

								default:
									return "is unknown type " + constant;
							}
						}
						else
							return "is " + GetOperandDescription(element, prefix, null);

					case "Context":
						return "is context";
				}
			}

			public static string GetDescription(XmlElement element) {
				ArrayList	parts = new ArrayList();
				string		argument;

				parts.Add("Generate Event " + element.GetAttribute("EventName"));
				argument = getArgumentDescription(element, "Sender");

				if(argument != null)
					parts.Add("Sender " + argument);

				argument = getArgumentDescription(element, "Info");

				if(argument != null)
					parts.Add("Info " + argument);
				
				if(element["Options"] != null) {
					XmlElement	optionsElement = element["Options"];

					if(optionsElement.ChildNodes.Count > 0) {
						parts.Add("Options {");

						foreach(XmlElement optionElement in optionsElement)
							parts.Add(optionElement.GetAttribute("Name") + " is " + GetOperandDescription(optionElement, "Option") +
								(optionElement.NextSibling != null ? "," : ""));

						parts.Add("}");
					}
				}

				return string.Join(" ", (string[] )parts.ToArray(typeof(string)));
			}

            protected override string Description => GetDescription(Element);
        }

		#endregion

		#endregion
	}
}

