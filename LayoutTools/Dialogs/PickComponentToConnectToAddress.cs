#region Using directives

using LayoutManager.Components;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#endregion

namespace LayoutManager.Tools.Dialogs {
    public partial class PickComponentToConnectToAddress : Form, IControlConnectionPointDestinationReceiverDialog {
        private readonly CommandStationInputEvent csEvent;
        private ControlConnectionPointDestination? selectedDestination = null;

        public PickComponentToConnectToAddress(CommandStationInputEvent csEvent) {
            InitializeComponent();

            this.csEvent = csEvent;

            labelClickOnComponent.Text = Regex.Replace(Regex.Replace(labelClickOnComponent.Text, "BUS", csEvent.Bus.Name),
                "ADDRESS", csEvent.AddressText);

            EventManager.AddObjectSubscriptions(this);
            Dispatch.AddObjectInstanceDispatcherTargets(this);
        }

        public ControlConnectionPointDestination? ConnectionDestination => selectedDestination;

        [DispatchTarget]
        void QueryLearnLayoutPickComponentDialog(IModelComponentConnectToControl component, List<IControlConnectionPointDestinationReceiverDialog> dialogs) {
            dialogs.Add(this);
        }

        private void ButtonCancel_Click(object? sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void PickComponentToConnectToAddress_FormClosed(object? sender, FormClosedEventArgs e) {
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        #region IControlConnectionPointDestinationReceiverDialog Members

        public string DialogName(ControlConnectionPointDestination connectionDestination) => csEvent.Bus.Name + ": " + csEvent.GetAddressTextForComponent(connectionDestination);

        public ControlBus Bus => csEvent.Bus;

        public void AddControlConnectionPointDestination(ControlConnectionPointDestination connectionDestination) {
            selectedDestination = connectionDestination;
            DialogResult = DialogResult.OK;
            Close();
        }

        #endregion
    }
}