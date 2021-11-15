using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.AutoConnectDialogs {
    /// <summary>
    /// Summary description for GetCommandStation.
    /// </summary>
    public partial class GetCommandStation : Form {
        public GetCommandStation(IList<IModelComponentIsCommandStation> commandStations) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            comboBoxCommandStations.DisplayMember = "Name";

            foreach (IModelComponentIsCommandStation commandStation in commandStations)
                comboBoxCommandStations.Items.Add(new Item(commandStation));

            if (comboBoxCommandStations.Items.Count > 0)
                comboBoxCommandStations.SelectedIndex = 0;
        }

        public IModelComponentIsCommandStation? CommandStation => comboBoxCommandStations.SelectedItem == null ? null : ((Item)comboBoxCommandStations.SelectedItem).CommandStation;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void ButtonOK_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.OK;
        }

        private class Item {
            public Item(IModelComponentIsCommandStation commandStation) {
                this.CommandStation = commandStation;
            }

            public IModelComponentIsCommandStation CommandStation { get; }

            public override string ToString() => CommandStation.NameProvider.Name.ToString();
        }
    }
}
