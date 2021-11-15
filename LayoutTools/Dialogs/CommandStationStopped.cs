using System;
using System.Windows.Forms;
using LayoutManager.Model;
using LayoutManager.Logic;

namespace LayoutManager.Tools.Dialogs {
    public partial class CommandStationStopped : Form {
        private readonly IModelComponentIsCommandStation _commandStation;

        public CommandStationStopped(IModelComponentIsCommandStation commandStation, string reason) {
            InitializeComponent();

            _commandStation = commandStation;
            labelCommandStationName.Text = commandStation.Name;

            if (reason != null)
                labelReason.Text = reason;

            bool hasActiveTrips = Ensure.ValueNotNull<bool>(EventManager.Event("any-active-trip-plan"));
            buttonAbortTrips.Visible = hasActiveTrips;

            EventManager.AddObjectSubscriptions(this);
        }

        [LayoutEvent("get-command-station-notification-dialog")]
        private void GetCommandStationNotificationDialog(LayoutEvent e) {
            if (e.Sender == _commandStation)
                e.Info = this;
        }

        [LayoutEvent("update-command-station-notification-dialog")]
        private void UpdateCommandStationNotificationDialog(LayoutEvent e) {
            if (e.Sender == _commandStation) {
                var reason = Ensure.NotNull<string>(e.Info);
                labelReason.Text = reason;
            }
        }

        [LayoutEvent("command-station-power-on-notification")]
        private void CommandStationPowerOnNotification(LayoutEvent e) {
            if (e.Sender == _commandStation)
                Close();
        }

        private void CommandStationStopped_FormClosing(object? sender, FormClosingEventArgs e) {
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        private void ButtonPowerOn_Click(object? sender, EventArgs e) {
            EventManager.Event(new LayoutEvent("cancel-emergency-stop-request", _commandStation));
        }

        private void ButtonAbortTrips_Click(object? sender, EventArgs e) {
            EventManager.Event(new LayoutEvent("suspend-all-trips", _commandStation));
        }
    }
}