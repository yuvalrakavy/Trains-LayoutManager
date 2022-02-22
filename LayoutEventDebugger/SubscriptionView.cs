using LayoutManager;
using System;
using System.Collections;
using System.Windows.Forms;

namespace LayoutEventDebugger {
    /// <summary>
    /// Summary description for SubscriptionView.
    /// </summary>
    public partial class SubscriptionView : Form {
        public SubscriptionView() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            Form parentForm = (Form)LayoutController.ActiveFrameWindow;
            Owner = parentForm;

            ViewByEventName();
            radioButtonViewByEventName.Checked = true;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void ViewByEventName() {
            SortedList byName = new SortedList();

            treeViewSubscriptions.Nodes.Clear();

            foreach (LayoutEventSubscriptionBase subscription in EventManager.Subscriptions) {
                string eventName = subscription.EventName ?? "<Any>";
                var entry = (ArrayList?)byName[eventName];

                if (entry == null) {
                    entry = new ArrayList();
                    byName[eventName] = entry;
                }

                entry.Add(subscription);
            }

            foreach (DictionaryEntry entry in byName) {
                TreeNode n = new TreeNode((String)entry.Key);

                treeViewSubscriptions.Nodes.Add(n);
                if (entry.Value != null) {
                    foreach (LayoutEventSubscriptionBase subscription in (ArrayList)entry.Value)
                        EventDebugger.AddSubscription(n, subscription, TitleFormat.ShowHandlerObject);
                }
            }
        }

        private void ViewByHandlerObject() {
            SortedList byObject = new SortedList();

            treeViewSubscriptions.Nodes.Clear();

            foreach (LayoutEventSubscriptionBase subscription in EventManager.Subscriptions) {
                var objectName = subscription.TargetObject?.GetType()?.FullName;

                if (objectName != null) {
                    var entry = (ArrayList?)byObject[objectName];

                    if (entry == null) {
                        entry = new ArrayList();
                        byObject[objectName] = entry;
                    }

                    entry.Add(subscription);
                }
            }

            foreach (DictionaryEntry entry in byObject) {
                TreeNode n = new TreeNode((String)entry.Key);

                treeViewSubscriptions.Nodes.Add(n);
                if (entry.Value != null) {
                    foreach (LayoutEventSubscriptionBase subscription in (ArrayList)entry.Value)
                        EventDebugger.AddSubscription(n, subscription, TitleFormat.ShowEventName);
                }
            }
        }

        private void ButtonClose_Click(object? sender, System.EventArgs e) {
            this.Close();
        }

        private void RadioButtonViewByEventName_CheckedChanged(object? sender, System.EventArgs e) {
            ViewByEventName();
        }

        private void RadioButtonByEventHandlerObject_CheckedChanged(object? sender, System.EventArgs e) {
            ViewByHandlerObject();
        }

        private void ButtonRefresh_Click(object? sender, System.EventArgs e) {
            if (radioButtonByEventHandlerObject.Checked)
                ViewByHandlerObject();
            else if (radioButtonViewByEventName.Checked)
                ViewByEventName();
        }

        private void SubscriptionView_Closed(object? sender, System.EventArgs e) {
            EventManager.Event(new LayoutEvent("subscription-view-closed", this));
        }
    }
}
