using LayoutManager.Model;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    public partial class CountComponents : Form {
        public CountComponents(Dictionary<string, int[]> counts) {
            InitializeComponent();

            Dictionary<string, string> typeNames = new() {
                { ControlConnectionPointTypes.Input, "Input (e.g. contact)" },
                { ControlConnectionPointTypes.OutputRelay, "Relay" },
                { ControlConnectionPointTypes.Output, "Output (e.g. Turnout)" }
            };

            foreach (var p in counts) {
                var typeName = typeNames.ContainsKey(p.Key) ? typeNames[p.Key] : p.Key;
                var item = new ListViewItem(new string[] { typeName, p.Value[0].ToString(), p.Value[1].ToString(), (p.Value[0] + p.Value[1]).ToString() });

                listViewCounts.Items.Add(item);
            }
        }

        private void ButtonClose_Click(object? sender, EventArgs e) {
            Close();
        }
    }
}
