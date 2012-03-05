using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using LayoutManager;

namespace DiMAX.Dialogs {
    public partial class TestLocoDrive : Form {
		DiMAXcommandStation commandStation;

		public TestLocoDrive (DiMAXcommandStation commandStation) {
			InitializeComponent();

			this.commandStation = commandStation;
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            int address;
            int speed;

            if (!int.TryParse(textBoxLocoAddress.Text, out address)) {
                MessageBox.Show("Invalid loco address");
                return;
            }

            if(!int.TryParse(textBoxSpeed.Text, out speed)) {
                MessageBox.Show("Invalid speed");
                return;
            }

            EventManager.AsyncEvent(new LayoutEvent(this, "test-loco-speed").SetOption("Address", address).SetOption("Speed", speed).SetCommandStation(commandStation));
        }
    }
}
