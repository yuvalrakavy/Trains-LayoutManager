using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    public partial class CountComponents : Form {
        public CountComponents(Dictionary<string, int[]> counts) {
            InitializeComponent();

            Dictionary<string, string> typeNames = new Dictionary<string, string>();

            typeNames.Add(ControlConnectionPointTypes.Input, "Input (e.g. contact)");
            typeNames.Add(ControlConnectionPointTypes.OutputRelay, "Relay");
            typeNames.Add(ControlConnectionPointTypes.Output, "Output (e.g. Turnout)");

            foreach (var p in counts) {
                var typeName = typeNames.ContainsKey(p.Key) ? typeNames[p.Key] : p.Key;
                var item = new ListViewItem(new string[] { typeName, p.Value[0].ToString(), p.Value[1].ToString(), (p.Value[0] + p.Value[1]).ToString() });

                listViewCounts.Items.Add(item);
            }
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
