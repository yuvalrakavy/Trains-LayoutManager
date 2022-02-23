#region Using directives

using LayoutManager.CommonUI;
using LayoutManager.Components;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Resources;
using System.Windows.Forms;

#endregion

namespace LayoutManager.Dialogs {
    internal partial class LearnLayout : Form {
        private static bool enableSound = false;
        private long lastSoundTime = 0;
        private const long soundGapThreshold = 4000 * TimeSpan.TicksPerMillisecond;
        private readonly Guid frameWindowId;

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
        private void DesignTimeCommandStationEvent(LayoutEvent e) {
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
        private void ActivateLearnLayout(LayoutEvent e) {
            if (WindowState == FormWindowState.Minimized)
                WindowState = FormWindowState.Normal;

            e.Info = this;
        }

        [LayoutEvent("exit-design-mode")]
        private void ExitDesignMode(LayoutEvent e) {
            buttonClose.PerformClick();
        }

        private void UpdateItems() {
            foreach (EventItem item in listViewEvents.Items)
                item.UpdateItem();
            UpdateButtons();
        }

        [DispatchTarget]
        private void OnControlModuleProgrammingRequiredChanged(ControlModule module) => UpdateItems();

        [DispatchTarget]
        private void OnUserActionRequiredChanged(IControlSupportUserAction subject) => UpdateItems();

        [DispatchTarget]
        private void OnControlModuleLabelChanged(ControlModule module, string? label) => UpdateItems();

        [DispatchTarget]
        private void OnControlBusConnected(ControlBus bus) => UpdateItems();

        [DispatchTarget]
        private void OnControlModuleLocationChanged(ControlModule module, Guid? locationId) => UpdateItems();

        [DispatchTarget]
        private void OnControlModuleAddressChanged(ControlModule module, int address) => UpdateItems();

        [DispatchTarget]
        private void OnControlBusRemoved(IModelComponentIsBusProvider busProvider) => UpdateItems();

        [DispatchTarget]
        private void OnControlBusAdded(IModelComponentIsBusProvider busProvider) => UpdateItems();

        [DispatchTarget]
        private void OnControlModuleRemoved(ControlModule module) => UpdateItems();

        [DispatchTarget]
        private void OnControlModuleAdded(ControlModule module) => UpdateItems();

        [DispatchTarget]
        private void OnComponentConnectedToControlModule(IModelComponentConnectToControl component, ControlConnectionPoint connetionPoint) => UpdateItems();

        [DispatchTarget]
        private void OnComponentDisconnectedFromControlModule(ModelComponent component, ControlConnectionPoint connectionPoint) => UpdateItems();

        [DispatchTarget]
        private void OnComponentConfigurationChanged(ModelComponent component) => UpdateItems();

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

            public string StatusText => Status switch {
                CommandStationEventStatus.Connected => "Connected to component",
                CommandStationEventStatus.NotConnected => "Not Connected to component",
                _ => "No control module",
            };

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
            Dispatch.Call.DeselectControlObjects(frameWindowId);
            LayoutController.Instance.EndDesignTimeActivation();
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        private void ButtonClose_Click(object? sender, EventArgs e) {
            Close();
        }

        private void ButtonRemove_Click(object? sender, EventArgs e) {
            if (listViewEvents.SelectedItems.Count > 0) {
                EventItem selected = (EventItem)listViewEvents.SelectedItems[0];

                listViewEvents.Items.Remove(selected);
            }
        }

        private void ButtonRemoveAll_Click(object? sender, EventArgs e) {
            listViewEvents.Items.Clear();
            UpdateButtons();
        }

        private void ListViewEvents_SelectedIndexChanged(object? sender, EventArgs e) {
            UpdateButtons();
        }

        private void UpdateButtons() {
            if (listViewEvents.SelectedItems.Count == 0) {
                Dispatch.Call.DeselectControlObjects(frameWindowId);
                buttonAction.Enabled = false;
            }
            else {
                EventItem selected = (EventItem)listViewEvents.SelectedItems[0];

                switch (selected.Status) {
                    case CommandStationEventStatus.Connected:
                        if (selected.CommandStationEvent.ConnectionPointRef != null)
                            Dispatch.Call.ShowControlConnectionPoint(frameWindowId, selected.CommandStationEvent.ConnectionPointRef);
                        buttonAction.Enabled = false;   // Already connected
                        break;

                    case CommandStationEventStatus.NotConnected:
                        var moduleReference = selected.CommandStationEvent.ConnectionPointRef?.ModuleReference;

                        if (moduleReference != null)
                            Dispatch.Call.ShowControlModule(frameWindowId, moduleReference);

                        buttonAction.Enabled = true;    // Need to connect
                        break;

                    case CommandStationEventStatus.NoControlModule:
                        Dispatch.Call.DeselectControlObjects(frameWindowId);
                        buttonAction.Enabled = true;    // Need to connect
                        break;
                }
            }
        }

        private void ButtonAction_Click(object? sender, EventArgs e) {
            if (listViewEvents.SelectedItems.Count > 0) {
                EventItem selected = (EventItem)listViewEvents.SelectedItems[0];
                Tools.Dialogs.PickComponentToConnectToAddress pickDialog = new(selected.CommandStationEvent);

                new SemiModalDialog(this, pickDialog, (Form dialog, object? info) => {
                    if (pickDialog.DialogResult == DialogResult.OK) {
                        var connectionPoint = (ControlConnectionPoint?)EventManager.Event(new LayoutEvent("connect-component-to-control-module-address-request", pickDialog.ConnectionDestination, selected.CommandStationEvent));

                        if (connectionPoint != null)
                            Dispatch.Call.ShowControlConnectionPoint(frameWindowId, new ControlConnectionPointReference(connectionPoint));
                    }
                }, null).ShowDialog();
            }
        }

        private void ListViewEvents_DoubleClick(object? sender, EventArgs e) {
            if (buttonAction.Enabled)
                buttonAction.PerformClick();
        }

        private void CheckBoxEnableSound_CheckedChanged(object? sender, EventArgs e) {
            enableSound = checkBoxEnableSound.Checked;
        }
    }
}
