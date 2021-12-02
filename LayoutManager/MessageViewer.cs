using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Linq;

using LayoutManager.Model;

namespace LayoutManager {
    /// <summary>
    /// Summary description for MessageViewer.
    /// </summary>
    public partial class MessageViewer : UserControl {
        private readonly ListViewItem? lastMessageMarker = null;

        private LayoutSelection? currentMessageSelection = null;
        private LayoutSelection? currentComponentSelection = null;
        private bool clearOnNewMessage = false;

        private static readonly LayoutTraceSwitch traceMessages = new("Messages", "Trace errors, warning etc.");


        public MessageViewer() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitForm call
            lastMessageMarker = new ListViewItem();
            listViewMessages.Items.Add(lastMessageMarker);
        }

        public void Initialize() {
            EventManager.AddObjectSubscriptions(this);
        }

        [LayoutEvent("add-message")]
        private void AddMessage(LayoutEvent e) {
            var message = Ensure.NotNull<String>(e.Info);

            AddMessageItem(new MessageItem(MessageSeverity.Message, e.Sender, message));
            columnHeaderArea.Width = -1;
        }

        [LayoutEvent("add-warning")]
        private void AddWarning(LayoutEvent e) {
            var message = Ensure.NotNull<String>(e.Info);

            AddMessageItem(new MessageItem(MessageSeverity.Warning, e.Sender, message));
            columnHeaderArea.Width = -1;
        }

        [LayoutEvent("add-error")]
        private void AddError(LayoutEvent e) {
            var message = Ensure.NotNull<String>(e.Info);

            AddMessageItem(new MessageItem(MessageSeverity.Error, e.Sender, message));
        }

        private void AddMessageItem(MessageItem item) {
            item.TraceMessage();

            if (clearOnNewMessage) {
                EventManager.Event(new LayoutEvent("clear-messages", this));
                clearOnNewMessage = false;
            }

            listViewMessages.Items.Insert(listViewMessages.Items.Count - 1, item);
            columnHeaderArea.Width = -1;

            if (item.Severity != MessageSeverity.Message)
                EventManager.Event(new LayoutEvent("show-messages", this));

            if (listViewMessages.SelectedItems.Count == 1 && listViewMessages.SelectedItems[0] == lastMessageMarker)
                listViewMessages.EnsureVisible(lastMessageMarker.Index);

            UpdateButtons();
        }

        protected void UpdateButtons() {
            if (listViewMessages.SelectedIndices.Count == 0) {
                buttonPrevMessage.Enabled = false;
                buttonNextMessage.Enabled = false;
            }
            else {
                int selectedIndex = listViewMessages.SelectedIndices[0];

                buttonPrevMessage.Enabled = selectedIndex > 0;
                buttonNextMessage.Enabled = selectedIndex < listViewMessages.Items.Count - 1;
            }

            if (currentMessageSelection != null && currentMessageSelection.Count > 1) {
                buttonNextComponent.Visible = true;
                buttonPreviousComponent.Visible = true;
            }
            else {
                buttonNextComponent.Visible = false;
                buttonPreviousComponent.Visible = false;
            }
        }

        protected void HideSelections() {
            if (currentMessageSelection != null) {
                currentMessageSelection.Hide();
                currentMessageSelection = null;
            }

            if (currentComponentSelection != null) {
                currentComponentSelection.Hide();
                currentComponentSelection = null;
            }
        }

        protected void UpdateSelections() {
            HideSelections();

            MessageItem? item = null;
            if (listViewMessages.SelectedItems.Count > 0)
                item = listViewMessages.SelectedItems[0] as MessageItem;

            if (item != null && item.Selection != null) {
                currentMessageSelection = item.Selection;

                currentMessageSelection.Display(new LayoutSelectionLook(Color.LightPink));

                if (currentMessageSelection.Count > 0) {
                    currentComponentSelection = new LayoutSelection();
                    SetComponentSelection(currentMessageSelection.Components.First());
                    currentComponentSelection.Display(new LayoutSelectionLook(Color.DarkRed));
                }
            }
        }

        private void SetComponentSelection(ModelComponent component) {
            currentComponentSelection?.Clear();
            currentComponentSelection?.Add(component);
            EventManager.Event(new LayoutEvent("ensure-component-visible", component, false));
        }

        [LayoutEvent("messages-hidden")]
        private void MessagesHidden(LayoutEvent e) {
            clearOnNewMessage = true;
            HideSelections();
        }

        [LayoutEvent("messages-shown")]
        private void MessagesShown(LayoutEvent e) {
            UpdateSelections();
        }

        [LayoutEvent("clear-messages")]
        private void ClearMessages(LayoutEvent e) {
            listViewMessages.Items.Clear();
            listViewMessages.Items.Add(lastMessageMarker);

            UpdateSelections();
            UpdateButtons();
        }

        #region Class that represent a message item

        private enum MessageSeverity {
            Message,
            Warning,
            Error
        };

        private class MessageItem : ListViewItem {
            private LayoutSelection? selection;

            public MessageItem(MessageSeverity severity, Object? messageSubject, string message) {
                string areaNames = "";

                this.Text = message;
                this.Severity = severity;

                if (messageSubject != null) {
                    SelectMessageSubject(messageSubject);

                    // Create areas string
                    Hashtable areas = new();

                    if (selection != null) {
                        foreach (ModelComponent component in selection)
                            if (!areas.Contains(component.Spot.Area))
                                areas.Add(component.Spot.Area, component);
                    }

                    foreach (LayoutModelArea area in areas.Keys) {
                        if (areaNames.Length > 0)
                            areaNames += ", ";
                        areaNames += area.Name;
                    }

                    if (selection != null && selection.Count > 1)
                        areaNames = selection.Count + " components in " + areaNames;
                }

                this.SubItems.Add(areaNames);

                switch (severity) {
                    case MessageSeverity.Message:
                        this.ImageIndex = 0;
                        break;

                    case MessageSeverity.Warning:
                        this.ImageIndex = 1;
                        break;

                    case MessageSeverity.Error:
                        this.ImageIndex = 2;
                        break;
                }
            }

            private void SelectMessageSubject(object messageSubject) {
                if (messageSubject is LayoutSelection selection)
                    this.selection = selection;
                else if (messageSubject is ModelComponent) {
                    ModelComponent component = (ModelComponent)messageSubject;

                    this.selection = new LayoutSelection(new ModelComponent[] { component });
                }
                else if (messageSubject is TrainStateInfo) {
                    TrainStateInfo train = (TrainStateInfo)messageSubject;

                    this.selection = new LayoutSelection {
                        train
                    };
                }
                else if (messageSubject is LayoutBlock) {
                    LayoutBlock block = (LayoutBlock)messageSubject;

                    if (block.OptionalBlockDefinition != null)
                        this.selection = new LayoutSelection(new ModelComponent[] { block.BlockDefinintion });
                    else {
                        this.selection = new LayoutSelection();

                        foreach (TrackEdge edge in block.TrackEdges)
                            this.selection.Add(edge.Track);
                    }
                }
                else if (messageSubject is LayoutOccupancyBlock aBlock) {
                    this.selection = new LayoutSelection();

                    foreach (TrackEdge edge in aBlock.TrackEdges)
                        this.selection.Add(edge.Track);
                }
                else if (messageSubject is TrainStateInfo[] trains) {
                    this.selection = new LayoutSelection();

                    foreach (TrainStateInfo train in trains)
                        this.selection.Add(train);
                }
                else if (messageSubject is Guid id) {
                    var component = LayoutModel.Component<ModelComponent>(id, LayoutPhase.All);

                    if (component != null)
                        SelectMessageSubject(component);
                    else {
                        LayoutBlock block = LayoutModel.Blocks[id];

                        if (block != null)
                            SelectMessageSubject(block);
                        else {
                            var train = LayoutModel.StateManager.Trains[id];

                            if (train != null)
                                SelectMessageSubject(train);
                            else
                                throw new ArgumentException("The ID " + id.ToString() + " given as message subject can not be associated with an object");
                        }
                    }
                }
                else
                    throw new ArgumentException("Invalid messageSubject - can be either selection or component", messageSubject.ToString());
            }

            public LayoutSelection? Selection => selection;

            public MessageSeverity Severity { get; }

            public void TraceMessage() {
                bool show = false;

                if (Severity == MessageSeverity.Error && traceMessages.TraceError)
                    show = true;
                else if (Severity == MessageSeverity.Warning && traceMessages.TraceWarning)
                    show = true;
                else if (Severity == MessageSeverity.Message && traceMessages.TraceInfo)
                    show = true;

                if (show) {
                    bool first = true;

                    Trace.WriteLine("*** " + Severity.ToString() + " -- " + Text);
                    Trace.Write("    at: ");

                    if (Selection != null) {
                        foreach (ModelComponent component in Selection)
                            Trace.Write((first ? "" : ", ") + component.FullDescription);
                    }
                    Trace.WriteLine("");
                }
            }
        }

        #endregion

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void ButtonClose_Click(object? sender, EventArgs e) {
            EventManager.Event(new LayoutEvent("hide-messages", this));
        }

        private void ListViewMessages_SelectedIndexChanged(object? sender, EventArgs e) {
            UpdateSelections();
            UpdateButtons();
        }

        private void ButtonNextComponent_Click(object? sender, EventArgs e) {
            if (currentComponentSelection != null && currentMessageSelection != null) {
                IList<ModelComponent> messageComponents = currentMessageSelection.Components.ToList();
                ModelComponent currentComponent = currentComponentSelection.Components.FirstOrDefault() ?? messageComponents[0];
                int currentComponentIndex = -1;

                for (int i = 0; i < messageComponents.Count; i++)
                    if (messageComponents[i] == currentComponent) {
                        currentComponentIndex = i;
                        break;
                    }

                if (currentComponentIndex >= 0) {
                    if (++currentComponentIndex == messageComponents.Count)
                        currentComponentIndex = 0;

                    SetComponentSelection(messageComponents[currentComponentIndex]);
                }
            }
        }

        private void ButtonPreviousComponent_Click(object? sender, EventArgs e) {
            if (currentComponentSelection != null && currentMessageSelection != null) {
                IList<ModelComponent> messageComponents = currentMessageSelection.Components.ToList();
                ModelComponent currentComponent = currentComponentSelection.Components.FirstOrDefault() ?? messageComponents[0];
                int currentComponentIndex = -1;

                for (int i = 0; i < messageComponents.Count; i++)
                    if (messageComponents[i] == currentComponent) {
                        currentComponentIndex = i;
                        break;
                    }

                if (currentComponentIndex >= 0) {
                    if (currentComponentIndex-- == 0)
                        currentComponentIndex = messageComponents.Count - 1;

                    SetComponentSelection(messageComponents[currentComponentIndex]);
                }
            }
        }

        private void ButtonNextMessage_Click(object? sender, EventArgs e) {
            if (listViewMessages.SelectedItems.Count > 0) {
                int i;

                i = listViewMessages.SelectedIndices[0];

                if (++i < listViewMessages.Items.Count) {
                    listViewMessages.SelectedItems[0].Selected = false;
                    listViewMessages.Items[i].Selected = true;
                }
            }
        }

        private void ButtonPrevMessage_Click(object? sender, EventArgs e) {
            if (listViewMessages.SelectedItems.Count > 0) {
                int i;

                i = listViewMessages.SelectedIndices[0];

                if (i > 0) {
                    i--;
                    listViewMessages.SelectedItems[0].Selected = false;
                    listViewMessages.Items[i].Selected = true;
                }
            }
        }

        private void ButtonClear_Click(object? sender, EventArgs e) {
            EventManager.Event("clear-messages", this);
        }
    }
}
