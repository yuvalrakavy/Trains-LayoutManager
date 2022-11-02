using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LayoutManager.Dialogs {
    public partial class LaunchDialog : Form {
        private readonly TaskCompletionSource<LaunchAction> tcs = new();
        private bool actionWasSet = false;

        public LaunchDialog(string? lastLayoutFilename) {
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
                if(!actionWasSet) { 
                    tcs.SetResult(value);
                    actionWasSet = true;
                    }
            }
        }

        public Task<LaunchAction> Task => tcs.Task;

        public string LayoutFilename {
            get;
            private set;
        } = String.Empty;

        public bool ResetToDefaultDisplayLayout => checkBoxResetToDefaultDisplayLayout.Checked;

        public bool UseLastOpenLayout {
            get;
            private set;
        }

        private void ButtonExit_Click(object? sender, EventArgs e) {
            Close();
            Action = LaunchAction.Exit;
        }

        private void LaunchDialog_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e) {
            Action = LaunchAction.Exit;
        }


        private void ButtonNew_Click(object? sender, EventArgs e) {
            if (saveFileDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK) {
                LayoutFilename = saveFileDialog.FileName;
                Action = LaunchAction.NewLayout;
                Close();
            }
        }

        private void ButtonOpen_Click(object? sender, EventArgs e) {
            if (openFileDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK) {
                LayoutFilename = openFileDialog.FileName;
                Action = LaunchAction.OpenLayout;
                Close();
            }
        }

        private void LinkLabelLastLayoutName_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e) {
            LayoutFilename = linkLabelLastLayoutName.Text;
            UseLastOpenLayout = true;
            Action = LaunchAction.OpenLayout;
            Close();
        }
    }
}
