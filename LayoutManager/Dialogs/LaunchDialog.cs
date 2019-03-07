using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.IO;

namespace LayoutManager.Dialogs {
    public partial class LaunchDialog : Form {
        readonly TaskCompletionSource<LaunchAction> tcs = new TaskCompletionSource<LaunchAction>();

        public LaunchDialog(string lastLayoutFilename) {
            InitializeComponent();

            if (string.IsNullOrWhiteSpace(lastLayoutFilename) || !File.Exists(lastLayoutFilename))
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

        public Task<LaunchAction> Task => tcs.Task;

        public string LayoutFilename {
            get;
            private set;
        }

        public bool ResetToDefaultDisplayLayout => checkBoxResetToDefaultDisplayLayout.Checked;

        public bool UseLastOpenLayout {
            get;
            private set;
        }

        private void buttonExit_Click(object sender, EventArgs e) {
            Close();
            Action = LaunchAction.Exit;
        }

        private void buttonNew_Click(object sender, EventArgs e) {
            if (saveFileDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK) {
                LayoutFilename = saveFileDialog.FileName;
                Close();
                Action = LaunchAction.NewLayout;
            }
        }

        private void buttonOpen_Click(object sender, EventArgs e) {
            if (openFileDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK) {
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
