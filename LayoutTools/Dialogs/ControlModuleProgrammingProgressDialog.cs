using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
#pragma warning disable IDE0051, IDE0060
    public partial class ControlModuleProgrammingProgressDialog : Form {
        private readonly Func<Task> doProgramming;
        private bool showingProgress = false;
        private bool programmingDone = false;

        public ControlModuleProgrammingProgressDialog(ControlModuleProgrammingState programmingState, Func<Task> doProgramming) {
            InitializeComponent();

            labelConnect.Text = string.Format(labelConnect.Text, programmingState.ControlModule.ModuleType.Name, programmingState.Programmer.NameProvider.Name);
            this.doProgramming = doProgramming;

            foreach (var action in programmingState.ProgrammingActions)
                listViewActions.Items.Add(new ActionItem(action));

            panelConnectModule.Visible = true;
            panelProgress.Visible = false;

            buttonCancelOrClose.Text = "Cancel";

            EventManager.AddObjectSubscriptions(this);
        }

        private void ControlModuleProgrammingProgressDialog_FormClosing(object sender, FormClosingEventArgs e) {
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        [LayoutEvent("action-status-changed")]
        private void actionStatusChanged(LayoutEvent e0) {
            var e = (LayoutEventInfoValueType<ILayoutAction, ActionStatus>)e0;
            ILayoutAction action = e.Sender;

            foreach (ActionItem actionItem in listViewActions.Items) {
                if (actionItem.Id == action.Id) {
                    actionItem.Status = e.Info;
                    break;
                }
            }
        }

        private async void buttonNext_Click(object sender, EventArgs e) {
            if (!showingProgress) {
                showingProgress = true;
                panelConnectModule.Visible = false;
                panelProgress.Visible = true;
                buttonNext.Text = "Program!";
            }
            else {
                buttonCancelOrClose.Text = "Close";
                buttonCancelOrClose.Enabled = false;
                await doProgramming();
                buttonCancelOrClose.Enabled = true;
                programmingDone = true;
            }
        }

        private void buttonCancelOrClose_Click(object sender, EventArgs e) {
            if (programmingDone) {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
            else {
                DialogResult = System.Windows.Forms.DialogResult.Cancel;
                Close();
            }
        }
    }

    internal class ActionItem : ListViewItem {
        public Guid Id { get; }
        public string Description { get; }
        public string StatusText { get; private set; }

        public ActionItem(ILayoutAction action) {
            this.Id = action.Id;
            this.Description = action.ToString();
            Text = Description;

            SubItems.Add("");
            this.Status = action.Status;
        }

        public ActionStatus Status {
            set {
                switch (value) {
                    case ActionStatus.Done: StatusText = "Done"; break;
                    case ActionStatus.Failed: StatusText = "Failed"; break;
                    case ActionStatus.InProgress: StatusText = "In progress"; break;
                    case ActionStatus.Pending: StatusText = "Pending"; break;
                    default: StatusText = "**UNKNOWN**"; break;
                }

                SubItems[1].Text = StatusText;
            }
        }
    }
}
