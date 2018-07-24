using System;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.EventScriptDialogs {
    public partial class IfTrainLength : Form {
		public IfTrainLength(string symbolName, TrainLength length, TrainLengthComparison comparison) {
			InitializeComponent();

			if(symbolName != null)
				comboBoxTrain.Text = symbolName;
			else
				comboBoxTrain.Text = "Train";

			trainLengthDiagram.Length = length;
			trainLengthDiagram.Comparison = comparison;
		}

		private void buttonOK_Click(object sender, EventArgs e) {
			if(string.IsNullOrEmpty(comboBoxTrain.Text)) {
				MessageBox.Show(this, "You have to define a train symbol name", "Missing input", MessageBoxButtons.OK, MessageBoxIcon.Error);
				comboBoxTrain.Focus();
				return;
			}

			DialogResult = DialogResult.OK;
			Close();
		}

        public string SymbolName => comboBoxTrain.Text;

        public TrainLength Length => trainLengthDiagram.Length;

        public TrainLengthComparison Comparison => trainLengthDiagram.Comparison;
    }
}