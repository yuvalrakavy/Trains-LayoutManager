using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
	public partial class ShiftOrMove : Form {
		public ShiftOrMove() {
			InitializeComponent();
		}

		public int DeltaX {
			get {
				if(radioButtonLeft.Checked)
					return (int)-numericUpDownComponents.Value;
				else if(radioButtonRight.Checked)
					return (int)numericUpDownComponents.Value;
				else
					return 0;
			}
		}

		public int DeltaY {
			get {
				if(radioButtonUp.Checked)
					return (int)-numericUpDownComponents.Value;
				else if(radioButtonDown.Checked)
					return (int)numericUpDownComponents.Value;
				else
					return 0;
			}
		}

		public bool FillGaps {
			get {
				return checkBoxFillGaps.Checked;
			}
		}

		private void buttonOK_Click(object sender, EventArgs e) {
			if(!radioButtonDown.Checked && !radioButtonLeft.Checked && !radioButtonRight.Checked && !radioButtonUp.Checked) {
				MessageBox.Show("Please pick a direction", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
				radioButtonUp.Focus();
				return;
			}

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}