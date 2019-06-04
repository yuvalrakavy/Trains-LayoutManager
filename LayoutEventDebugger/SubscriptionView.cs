using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager;

namespace LayoutEventDebugger {
    /// <summary>
    /// Summary description for SubscriptionView.
    /// </summary>
    public class SubscriptionView : Form {
        private TreeView treeViewSubscriptions;
        private GroupBox groupBoxViewBy;
        private RadioButton radioButtonViewByEventName;
        private RadioButton radioButtonByEventHandlerObject;
        private Button buttonClose;
        private Button buttonRefresh;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        public SubscriptionView() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            Form parentForm = LayoutController.ActiveFrameWindow as Form;
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
                string eventName = (subscription.EventName == null) ? "<Any>" : subscription.EventName;
                ArrayList entry = (ArrayList)byName[eventName];

                if (entry == null) {
                    entry = new ArrayList();
                    byName[eventName] = entry;
                }

                entry.Add(subscription);
            }

            foreach (DictionaryEntry entry in byName) {
                TreeNode n = new TreeNode((String)entry.Key);

                treeViewSubscriptions.Nodes.Add(n);
                foreach (LayoutEventSubscriptionBase subscription in (ArrayList)entry.Value)
                    EventDebugger.AddSubscription(n, subscription, TitleFormat.ShowHandlerObject);
            }
        }

        private void ViewByHandlerObject() {
            SortedList byObject = new SortedList();

            treeViewSubscriptions.Nodes.Clear();

            foreach (LayoutEventSubscriptionBase subscription in EventManager.Subscriptions) {
                string objectName = subscription.TargetObject.GetType().FullName;
                ArrayList entry = (ArrayList)byObject[objectName];

                if (entry == null) {
                    entry = new ArrayList();
                    byObject[objectName] = entry;
                }

                entry.Add(subscription);
            }

            foreach (DictionaryEntry entry in byObject) {
                TreeNode n = new TreeNode((String)entry.Key);

                treeViewSubscriptions.Nodes.Add(n);
                foreach (LayoutEventSubscriptionBase subscription in (ArrayList)entry.Value)
                    EventDebugger.AddSubscription(n, subscription, TitleFormat.ShowEventName);
            }
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.treeViewSubscriptions = new TreeView();
            this.groupBoxViewBy = new GroupBox();
            this.radioButtonByEventHandlerObject = new RadioButton();
            this.radioButtonViewByEventName = new RadioButton();
            this.buttonClose = new Button();
            this.buttonRefresh = new Button();
            this.groupBoxViewBy.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewSubscriptions
            // 
            this.treeViewSubscriptions.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.treeViewSubscriptions.ImageIndex = -1;
            this.treeViewSubscriptions.Location = new System.Drawing.Point(8, 8);
            this.treeViewSubscriptions.Name = "treeViewSubscriptions";
            this.treeViewSubscriptions.SelectedImageIndex = -1;
            this.treeViewSubscriptions.Size = new System.Drawing.Size(352, 272);
            this.treeViewSubscriptions.TabIndex = 0;
            // 
            // groupBoxViewBy
            // 
            this.groupBoxViewBy.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.groupBoxViewBy.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                         this.radioButtonByEventHandlerObject,
                                                                                         this.radioButtonViewByEventName});
            this.groupBoxViewBy.Location = new System.Drawing.Point(8, 288);
            this.groupBoxViewBy.Name = "groupBoxViewBy";
            this.groupBoxViewBy.Size = new System.Drawing.Size(160, 56);
            this.groupBoxViewBy.TabIndex = 1;
            this.groupBoxViewBy.TabStop = false;
            this.groupBoxViewBy.Text = "View by:";
            // 
            // radioButtonByEventHandlerObject
            // 
            this.radioButtonByEventHandlerObject.Location = new System.Drawing.Point(8, 33);
            this.radioButtonByEventHandlerObject.Name = "radioButtonByEventHandlerObject";
            this.radioButtonByEventHandlerObject.Size = new System.Drawing.Size(128, 17);
            this.radioButtonByEventHandlerObject.TabIndex = 1;
            this.radioButtonByEventHandlerObject.Text = "Event handler object";
            this.radioButtonByEventHandlerObject.CheckedChanged += this.radioButtonByEventHandlerObject_CheckedChanged;
            // 
            // radioButtonViewByEventName
            // 
            this.radioButtonViewByEventName.Location = new System.Drawing.Point(8, 12);
            this.radioButtonViewByEventName.Name = "radioButtonViewByEventName";
            this.radioButtonViewByEventName.TabIndex = 0;
            this.radioButtonViewByEventName.Text = "Event name";
            this.radioButtonViewByEventName.CheckedChanged += this.radioButtonViewByEventName_CheckedChanged;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new System.Drawing.Point(280, 320);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(80, 24);
            this.buttonClose.TabIndex = 3;
            this.buttonClose.Text = "Close";
            this.buttonClose.Click += this.buttonClose_Click;
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonRefresh.Location = new System.Drawing.Point(184, 320);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(80, 24);
            this.buttonRefresh.TabIndex = 2;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.Click += this.buttonRefresh_Click;
            // 
            // SubscriptionView
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.ClientSize = new System.Drawing.Size(368, 349);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonRefresh,
                                                                          this.buttonClose,
                                                                          this.groupBoxViewBy,
                                                                          this.treeViewSubscriptions});
            this.MinimumSize = new System.Drawing.Size(370, 0);
            this.Name = "SubscriptionView";
            this.Text = "Event Subscription View";
            this.Closed += this.SubscriptionView_Closed;
            this.groupBoxViewBy.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonClose_Click(object sender, System.EventArgs e) {
            this.Close();
        }

        private void radioButtonViewByEventName_CheckedChanged(object sender, System.EventArgs e) {
            ViewByEventName();
        }

        private void radioButtonByEventHandlerObject_CheckedChanged(object sender, System.EventArgs e) {
            ViewByHandlerObject();
        }

        private void buttonRefresh_Click(object sender, System.EventArgs e) {
            if (radioButtonByEventHandlerObject.Checked)
                ViewByHandlerObject();
            else if (radioButtonViewByEventName.Checked)
                ViewByEventName();
        }

        private void SubscriptionView_Closed(object sender, System.EventArgs e) {
            EventManager.Event(new LayoutEvent("subscription-view-closed", this));
        }
    }
}
