﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    public partial class CountComponents : Form {
        public CountComponents(Dictionary<string, int[]> counts) {
            InitializeComponent();

            Dictionary<string, string> typeNames = new Dictionary<string, string>();

            typeNames.Add("DryContact", "Input (e.g. contact)");
            typeNames.Add("Solenoid", "Output (e.g. Turnout)");
            typeNames.Add("Relay", "Relay");

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
