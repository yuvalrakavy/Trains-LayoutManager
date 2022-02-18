using System;
using System.Windows.Forms;
using MethodDispatcher;
using LayoutManager;

namespace DiMAX.Dialogs {
    public partial class TestLocoSelect : Form {
        private readonly DiMAXcommandStation commandStation;

        public TestLocoSelect(DiMAXcommandStation commandStation) {
            InitializeComponent();

            this.commandStation = commandStation;
        }

        private void ButtonSendCommand_Click(object? sender, EventArgs e) {
            if (!int.TryParse(textBoxAddress.Text, out int address)) {
                MessageBox.Show("Invalid loco address");
                return;
            }

            Dispatch.Call.DiMAXtestLocoSelect(commandStation, address, checkBoxSelect.Checked, checkBoxActive.Checked, checkBoxUnconditional.Checked);
        }
    }
}
