﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using LayoutManager.Model;

namespace LayoutManager.CommonUI.Dialogs {
    public partial class TrainTrackingOptions : Form {
        public TrainTrackingOptions() {
            InitializeComponent();

            switch (LayoutModel.StateManager.TrainTrackingOptions.TrainTrackingInManualDispatchRegion) {
                case TrainTrackingInManualDispatchRegion.None: radioButtonManualDispatchNoTracking.Checked = true; break;
                case TrainTrackingInManualDispatchRegion.Normal: radioButtonManualDispatchNormalTracking.Checked = true; break;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            if (radioButtonManualDispatchNormalTracking.Checked)
                LayoutModel.StateManager.TrainTrackingOptions.TrainTrackingInManualDispatchRegion = TrainTrackingInManualDispatchRegion.Normal;
            else if (radioButtonManualDispatchNoTracking.Checked)
                LayoutModel.StateManager.TrainTrackingOptions.TrainTrackingInManualDispatchRegion = TrainTrackingInManualDispatchRegion.None;

            LayoutModel.StateManager.Save();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
