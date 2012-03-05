using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace LayoutManager.Dialogs {
	public partial class LaunchDialog : Form {
		TaskCompletionSource<LaunchAction> tcs = new TaskCompletionSource<LaunchAction>();

		public LaunchDialog(string lastLayoutFilename) {
			InitializeComponent();

			if(string.IsNullOrWhiteSpace(lastLayoutFilename))
				linkLabelLastLayoutName.Visible = false;
			else
				linkLabelLastLayoutName.Text = lastLayoutFilename;

			checkBoxResetToDefaultDisplayLayout.Checked = false;
			UseLastOpenLayout = false;
		}

		private LaunchAction Action {
			set {
				tcs.SetResult(value);
			}
		}

		public Task<LaunchAction> Task {
			get {
				return tcs.Task;
			}
		}

		public string LayoutFilename {
			get;
			private set;
		}

		public bool ResetToDefaultDisplayLayout {
			get {
				return checkBoxResetToDefaultDisplayLayout.Checked;
			}
		}

		public bool UseLastOpenLayout {
			get;
			private set;
		}

		private void buttonExit_Click(object sender, EventArgs e) {
            Close();
            Action = LaunchAction.Exit;
		}

		private void buttonNew_Click(object sender, EventArgs e) {
			if(saveFileDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK) {
                LayoutFilename = saveFileDialog.FileName;
                Close();
                Action = LaunchAction.NewLayout;
			}
		}

		private void buttonOpen_Click(object sender, EventArgs e) {
			if(openFileDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK) {
				LayoutFilename = openFileDialog.FileName;
				Close();
                Action = LaunchAction.OpenLayout;
            }
		}

		private void linkLabelLastLayoutName_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			LayoutFilename = linkLabelLastLayoutName.Text;
			UseLastOpenLayout = true;
            Close();
            Action = LaunchAction.OpenLayout;
		}
	}
}
