#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

#endregion

namespace LayoutManager.Tools.Dialogs {
	public partial class PickComponentToConnectToAddress : Form, IControlConnectionPointDestinationReceiverDialog {
		CommandStationInputEvent csEvent;
		ControlConnectionPointDestination selectedDestination = null;

		public PickComponentToConnectToAddress(CommandStationInputEvent csEvent) {
			InitializeComponent();

			this.csEvent = csEvent;

			labelClickOnComponent.Text = Regex.Replace(Regex.Replace(labelClickOnComponent.Text, "BUS", csEvent.Bus.Name),
				"ADDRESS", csEvent.AddressText);

			EventManager.AddObjectSubscriptions(this);
		}

		public ControlConnectionPointDestination ConnectionDestination {
			get {
				return selectedDestination;
			}
		}

		[LayoutEvent("query-learn-layout-pick-component-dialog")]
		private void queryLearnLayoutPickComponentDialog(LayoutEvent e) {
			List<IControlConnectionPointDestinationReceiverDialog> dialogs = (List<IControlConnectionPointDestinationReceiverDialog>)e.Info;

			dialogs.Add(this);
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void PickComponentToConnectToAddress_FormClosed(object sender, FormClosedEventArgs e)
		{
			EventManager.Subscriptions.RemoveObjectSubscriptions(this);
		}

		#region IControlConnectionPointDestinationReceiverDialog Members

		public string DialogName(ControlConnectionPointDestination connectionDestination) {
			return csEvent.Bus.Name + ": " + csEvent.GetAddressTextForComponent(connectionDestination);
		}

		public ControlBus Bus {
			get { return csEvent.Bus; }
		}


		public void AddControlConnectionPointDestination(ControlConnectionPointDestination connectionDestination) {
			selectedDestination = connectionDestination;
			DialogResult = DialogResult.OK;
			Close();
		}

#endregion
	}
}