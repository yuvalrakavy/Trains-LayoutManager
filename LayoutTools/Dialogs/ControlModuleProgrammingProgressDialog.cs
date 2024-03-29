﻿using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
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

            Dispatch.AddObjectInstanceDispatcherTargets(this);
        }

        private void ControlModuleProgrammingProgressDialog_FormClosing(object? sender, FormClosingEventArgs e) {
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        [DispatchTarget]
        private void OnActionStatusChanged(LayoutAction action, ActionStatus status) {
            foreach (ActionItem actionItem in listViewActions.Items) {
                if (actionItem.Id == action.Id) {
                    actionItem.Status = status;
                    break;
                }
            }
        }

        private async void ButtonNext_Click(object? sender, EventArgs e) {
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

        private void ButtonCancelOrClose_Click(object? sender, EventArgs e) {
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
            this.Description = action.ToString() ?? String.Empty;
            this.StatusText = String.Empty;
            Text = Description;

            SubItems.Add("");
            this.Status = action.Status;
        }

        public ActionStatus Status {
            set => SubItems[1].Text =value switch {
                ActionStatus.Done => "Done",
                ActionStatus.Failed => "Failed",
                ActionStatus.InProgress => "In progress",
                ActionStatus.Pending => "Pending",
                _ => "**UNKNOWN**",
            };
        }
    }
}
