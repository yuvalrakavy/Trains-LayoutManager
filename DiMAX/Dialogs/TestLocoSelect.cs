using System;
using System.Windows.Forms;

using LayoutManager;

namespace DiMAX.Dialogs {
    public partial class TestLocoSelect : Form {
		DiMAXcommandStation commandStation;

		public TestLocoSelect(DiMAXcommandStation commandStation) {
			InitializeComponent();

			this.commandStation = commandStation;
		}

		private void buttonSendCommand_Click(object sender, EventArgs e) {
			int address;

			if(!int.TryParse(textBoxAddress.Text, out address)) {
				MessageBox.Show("Invalid loco address");
				return;
			}

			EventManager.AsyncEvent(new LayoutEvent(this, "test-loco-select").SetOption("Address", address).SetOption("Select", checkBoxSelect.Checked).SetOption("Active", checkBoxActive.Checked).SetOption("Unconditional", checkBoxUnconditional.Checked).SetCommandStation(commandStation));
		}


	}
}
