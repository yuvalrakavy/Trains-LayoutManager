using System;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    public partial class CommandStationStopped : Form {
        readonly IModelComponentIsCommandStation _commandStation;

        public CommandStationStopped(IModelComponentIsCommandStation commandStation, string reason) {
            InitializeComponent();

            _commandStation = commandStation;
            labelCommandStationName.Text = commandStation.Name;

            if (reason != null)
                labelReason.Text = reason;

            EventManager.AddObjectSubscriptions(this);
        }

        [LayoutEvent("get-command-station-notification-dialog")]
        private void getCommandStationNotificationDialog(LayoutEvent e) {
            if (e.Sender == _commandStation)
                e.Info = this;
        }

        [LayoutEvent("update-command-station-notification-dialog")]
        private void updateCommandStationNotificationDialog(LayoutEvent e) {
            if (e.Sender == _commandStation) {
                labelReason.Text = (string)e.Info;
            }
        }

        [LayoutEvent("command-station-power-on-notification")]
        private void commandStationPowerOnNotification(LayoutEvent e) {
            if (e.Sender == _commandStation)
                Close();
        }


        private void CommandStationStopped_FormClosing(object sender, FormClosingEventArgs e) {
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        private void buttonPowerOn_Click(object sender, EventArgs e) {
            EventManager.Event(new LayoutEvent("cancel-emergency-stop-request", _commandStation));
        }

        private void buttonAbortTrips_Click(object sender, EventArgs e) {
            EventManager.Event(new LayoutEvent("suspend-all-trips", _commandStation));
        }
    }
}