using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager;

namespace LayoutEventDebugger {
    /// <summary>
    /// Summary description for SubscriptionView.
    /// </summary>
    partial class SubscriptionView : Form {
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
            this.treeViewSubscriptions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.treeViewSubscriptions.ImageIndex = -1;
            this.treeViewSubscriptions.Location = new System.Drawing.Point(8, 8);
            this.treeViewSubscriptions.Name = "treeViewSubscriptions";
            this.treeViewSubscriptions.SelectedImageIndex = -1;
            this.treeViewSubscriptions.Size = new System.Drawing.Size(352, 272);
            this.treeViewSubscriptions.TabIndex = 0;
            // 
            // groupBoxViewBy
            // 
            this.groupBoxViewBy.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
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
            this.radioButtonByEventHandlerObject.CheckedChanged += this.RadioButtonByEventHandlerObject_CheckedChanged;
            // 
            // radioButtonViewByEventName
            // 
            this.radioButtonViewByEventName.Location = new System.Drawing.Point(8, 12);
            this.radioButtonViewByEventName.Name = "radioButtonViewByEventName";
            this.radioButtonViewByEventName.TabIndex = 0;
            this.radioButtonViewByEventName.Text = "Event name";
            this.radioButtonViewByEventName.CheckedChanged += this.RadioButtonViewByEventName_CheckedChanged;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new System.Drawing.Point(280, 320);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(80, 24);
            this.buttonClose.TabIndex = 3;
            this.buttonClose.Text = "Close";
            this.buttonClose.Click += this.ButtonClose_Click;
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonRefresh.Location = new System.Drawing.Point(184, 320);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(80, 24);
            this.buttonRefresh.TabIndex = 2;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.Click += this.ButtonRefresh_Click;
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
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Name = "SubscriptionView";
            this.Text = "Event Subscription View";
            this.Closed += this.SubscriptionView_Closed;
            this.groupBoxViewBy.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

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

    }
}
