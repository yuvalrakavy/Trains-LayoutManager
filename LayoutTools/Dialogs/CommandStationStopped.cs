using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    public partial class CommandStationStopped : Form {
        private readonly IModelComponentIsCommandStation _commandStation;

        public CommandStationStopped(IModelComponentIsCommandStation commandStation, string reason) {
            InitializeComponent();

            _commandStation = commandStation;
            labelCommandStationName.Text = commandStation.Name;

            if (reason != null)
                labelReason.Text = reason;

            buttonAbortTrips.Visible = Dispatch.Call.IsAnyActiveTripPlan();

            EventManager.AddObjectSubscriptions(this);
            Dispatch.AddObjectInstanceDispatcherTargets(this);
        }

        [DispatchTarget]
        private Form? GetCommandStationNotificationDialog(IModelComponentIsCommandStation commandStation) {
            return commandStation == _commandStation ? this : null;
        }

        [DispatchTarget]
        private void UpdateCommandStationNotificationDialog(IModelComponentIsCommandStation commandStation, string reason) {
            if (commandStation == _commandStation)
                labelReason.Text = reason;
        }

        [DispatchTarget]
        private void OnCommandStationPowerOn(IModelComponentIsCommandStation commandStation) {
            if (commandStation == _commandStation)
                Close();
        }

        private void CommandStationStopped_FormClosing(object? sender, FormClosingEventArgs e) {
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
            Dispatch.RemoveObjectInstanceDispatcherTargets(this);
        }

        private void ButtonPowerOn_Click(object? sender, EventArgs e) {
            Dispatch.Call.CancelEmergencyStopRequest(_commandStation);
        }

        private void ButtonAbortTrips_Click(object? sender, EventArgs e) {
            Dispatch.Call.SuspendAllTrips();
        }
    }
}