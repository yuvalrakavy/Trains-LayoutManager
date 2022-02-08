#region Using directives

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Resources;
using System.IO;
using System.Diagnostics;
using System.Linq;
using LayoutManager.Model;
using LayoutManager.Components;
using LayoutManager.CommonUI;
using System.Media;
using MethodDispatcher;

#endregion

#pragma warning disable IDE0051, IDE0060
namespace LayoutManager.Dialogs {
    internal partial class LearnLayout : Form {
        private static bool enableSound = false;
        private long lastSoundTime = 0;
        private const long soundGapThreshold = 4000 * TimeSpan.TicksPerMillisecond;
        private Guid frameWindowId;

        public LearnLayout(Guid frameWindowId) {
            InitializeComponent();

            this.frameWindowId = frameWindowId;
            EventManager.AddObjectSubscriptions(this);
            Dispatch.AddObjectInstanceDispatcherTargets(this);

            checkBoxEnableSound.Checked = enableSound;

            if (LayoutModel.Components<IModelComponentIsCommandStation>(LayoutModel.ActivePhases).DefaultIfEmpty() != null)
                listViewEvents.Columns.Add("Command Station", 110);

            UpdateButtons();
        }

        [LayoutEvent("design-time-command-station-event")]
        private void designTimeCommandStationEvent(LayoutEvent e) {
            var csEvent = Ensure.NotNull<CommandStationInputEvent>(e.Info);
            EventItem? eventItem = null;

            foreach (EventItem item in listViewEvents.Items)
                if (item.CommandStationEvent.Equals(csEvent)) {
                    eventItem = item;
                    break;
                }

            if (eventItem == null) {
                eventItem = new EventItem(csEvent);

                if (eventItem.Status != CommandStationEventStatus.Connected || !checkBoxOnlyShowNotConnected.Checked)
                    listViewEvents.Items.Add(eventItem);
            }
            else
                eventItem.CommandStationEvent = csEvent;

            eventItem.Selected = true;

            if (enableSound) {
                long diff = DateTime.Now.Ticks - lastSoundTime;

                if (diff > soundGapThreshold) {
                    string? resourceName = null;

                    Trace.WriteLine("Sending sound " + DateTime.Now.Ticks + " " + lastSoundTime + " diff " + diff + " threadshold " + soundGapThreshold);

                    switch (eventItem.Status) {
                        case CommandStationEventStatus.Connected: resourceName = "soundLearnLayoutConnected"; break;
                        case CommandStationEventStatus.NotConnected: resourceName = "soundLearnLayoutNotConnected"; break;
                        case CommandStationEventStatus.NoControlModule: resourceName = "soundLearnLayoutNoControlModule"; break;
                    }

                    if (resourceName != null) {
                        ResourceManager rm = new("LayoutManager.Sounds", this.GetType().Assembly);
                        var soundData = (byte[]?)rm.GetObject(resourceName);

                        if (soundData != null) {
                            MemoryStream stream = new(soundData);
                            SoundPlayer soundPlayer = new(stream);

                            soundPlayer.Stop();
                            soundPlayer.Play();
                        }
                    }

                    lastSoundTime = DateTime.Now.Ticks;
                }
                else
                    Trace.WriteLine("No sound, too close");
            }
        }

        [LayoutEvent("activate-learn-layout")]
        private void activateLearnLayout(LayoutEvent e) {
            if (WindowState == FormWindowState.Minimized)
                WindowState = FormWindowState.Normal;

            e.Info = this;
        }

        [LayoutEvent("exit-design-mode")]
        private void exitDesignMode(LayoutEvent e) {
            buttonClose.PerformClick();
        }

        [LayoutEvent("control-module-removed")]
        [LayoutEvent("control-module-added")]
        [LayoutEvent("control-module-address-changed")]
        [LayoutEvent("control-module-location-changed")]
        [LayoutEvent("control-bus-reconnected")]
        [LayoutEvent("component-disconnected-from-control-module")]
        [LayoutEvent("component-connected-to-control-module")]
        [LayoutEvent("component-configuration-changed")]
        [LayoutEvent("control-module-label-changed")]
        [LayoutEvent("control-user-action-required-changed")]
        [LayoutEvent("control-address-programming-required-changed")]
        [LayoutEvent("control-buses-added")]
        [LayoutEvent("control-buses-removed")]
        private void updateItems(LayoutEvent e) {
            foreach (EventItem item in listViewEvents.Items)
                item.UpdateItem();
            UpdateButtons();
        }

        #region Event Item

        public enum CommandStationEventStatus {
            NoControlModule,
            NotConnected,
            Connected
        };

        private class EventItem : ListViewItem {
            private CommandStationInputEvent csEvent;

            public EventItem(CommandStationInputEvent csEvent) {
                this.csEvent = csEvent;

                Text = "";          // Address
                SubItems.Add("");   // Connection
                SubItems.Add("");   // State
                SubItems.Add("");   // Status
                SubItems.Add("");   // Command station

                UseItemStyleForSubItems = false;

                UpdateItem();
            }

            public CommandStationInputEvent CommandStationEvent {
                get {
                    return csEvent;
                }

                set {
                    csEvent = value;
                    UpdateItem();
                }
            }

            public CommandStationEventStatus Status {
                get {
                    var cpr = csEvent.ConnectionPointRef;

                    if (cpr == null || !cpr.IsModuleDefined())
                        return CommandStationEventStatus.NoControlModule;
                    else return cpr.IsConnected ? CommandStationEventStatus.Connected : CommandStationEventStatus.NotConnected;
                }
            }

            public string StatusText {
                get {
                    switch (Status) {
                        case CommandStationEventStatus.Connected: return "Connected to component";
                        case CommandStationEventStatus.NotConnected: return "Not Connected to component";
                        default:
                        case CommandStationEventStatus.NoControlModule: return "No control module";
                    }
                }
            }

            public void UpdateItem() {
                SubItems[0].Text = csEvent.AddressText;
                SubItems[1].Text = csEvent.Bus.Name;

                if (csEvent.Bus.BusType.AddressingMethod == ControlAddressingMethod.DirectConnectionPointAddressing &&
                    csEvent.ConnectionPointRef != null && csEvent.ConnectionPointRef.Module.ModuleType.ConnectionPointsPerAddress > 1) {
                    SubItems[2].Text = "";
                    SubItems[2].BackColor = BackColor;
                }
                else {
                    if (csEvent.State == 0) {
                        SubItems[2].Text = "Off";
                        SubItems[2].BackColor = Color.Green;
                    }
                    else {
                        SubItems[2].Text = "On";
                        SubItems[2].BackColor = Color.Red;
                    }
                }

                SubItems[3].Text = StatusText;
                SubItems[4].Text = ((IModelComponentIsBusProvider)csEvent.CommandStation).NameProvider.Name;

                switch (Status) {
                    case CommandStationEventStatus.Connected: ImageIndex = 0; break;
                    case CommandStationEventStatus.NotConnected: ImageIndex = 1; break;
                    case CommandStationEventStatus.NoControlModule: ImageIndex = 2; break;
                }
            }
        }

        #endregion

        private void LearnLayout_FormClosed(object? sender, FormClosedEventArgs e) {
            EventManager.Event(new LayoutEvent("deselect-control-objects", this).SetFrameWindow(frameWindowId));
            LayoutController.Instance.EndDesignTimeActivation();
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        private void buttonClose_Click(object? sender, EventArgs e) {
            Close();
        }

        private void buttonRemove_Click(object? sender, EventArgs e) {
            if (listViewEvents.SelectedItems.Count > 0) {
                EventItem selected = (EventItem)listViewEvents.SelectedItems[0];

                listViewEvents.Items.Remove(selected);
            }
        }

        private void buttonRemoveAll_Click(object? sender, EventArgs e) {
            listViewEvents.Items.Clear();
            UpdateButtons();
        }

        private void listViewEvents_SelectedIndexChanged(object? sender, EventArgs e) {
            UpdateButtons();
        }

        private void UpdateButtons() {
            if (listViewEvents.SelectedItems.Count == 0) {
                EventManager.Event(new LayoutEvent("deselect-control-objects", this).SetFrameWindow(frameWindowId));
                buttonAction.Enabled = false;
            }
            else {
                EventItem selected = (EventItem)listViewEvents.SelectedItems[0];

                switch (selected.Status) {
                    case CommandStationEventStatus.Connected:
                        EventManager.Event(new LayoutEvent("show-control-connection-point", selected.CommandStationEvent.ConnectionPointRef).SetFrameWindow(frameWindowId));
                        buttonAction.Enabled = false;   // Already connected
                        break;

                    case CommandStationEventStatus.NotConnected:
                        var moduleReference = selected.CommandStationEvent.ConnectionPointRef?.ModuleReference;

                        if(moduleReference != null)
                            EventManager.Event(new LayoutEvent("show-control-module", moduleReference).SetFrameWindow(frameWindowId));

                        buttonAction.Enabled = true;    // Need to connect
                        break;

                    case CommandStationEventStatus.NoControlModule:
                        EventManager.Event(new LayoutEvent("deselect-control-objects", this).SetFrameWindow(frameWindowId));
                        buttonAction.Enabled = true;    // Need to connect
                        break;
                }
            }
        }

        private void buttonAction_Click(object? sender, EventArgs e) {
            if (listViewEvents.SelectedItems.Count > 0) {
                EventItem selected = (EventItem)listViewEvents.SelectedItems[0];
                Tools.Dialogs.PickComponentToConnectToAddress pickDialog = new(selected.CommandStationEvent);

                new SemiModalDialog(this, pickDialog, (Form dialog, object? info) => {
                    if (pickDialog.DialogResult == DialogResult.OK) {
                        var connectionPoint = (ControlConnectionPoint?)EventManager.Event(new LayoutEvent("connect-component-to-control-module-address-request", pickDialog.ConnectionDestination, selected.CommandStationEvent));

                        if (connectionPoint != null)
                            EventManager.Event(new LayoutEvent("show-control-connection-point", new ControlConnectionPointReference(connectionPoint)).SetFrameWindow(frameWindowId));
                    }
                }, null).ShowDialog();
            }
        }

        private void listViewEvents_DoubleClick(object? sender, EventArgs e) {
            if (buttonAction.Enabled)
                buttonAction.PerformClick();
        }

        private void checkBoxEnableSound_CheckedChanged(object? sender, EventArgs e) {
            enableSound = checkBoxEnableSound.Checked;
        }
    }
}
