using System;
using System.Windows.Forms;

using LayoutManager;

namespace DiMAX.Dialogs {
    public partial class TestLocoDrive : Form {
        private readonly DiMAXcommandStation commandStation;

        public TestLocoDrive(DiMAXcommandStation commandStation) {
            InitializeComponent();

            this.commandStation = commandStation;
        }

        private void buttonOK_Click(object? sender, EventArgs e) {
            if (!int.TryParse(textBoxLocoAddress.Text, out int address)) {
                MessageBox.Show(this, "Invalid loco address", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(textBoxSpeed.Text, out int speed)) {
                MessageBox.Show("Invalid speed");
                return;
            }

            EventManager.AsyncEvent(new LayoutEvent("test-loco-speed", this).SetOption("Address", address).SetOption("Speed", speed).SetCommandStation(commandStation));
        }
    }
}
