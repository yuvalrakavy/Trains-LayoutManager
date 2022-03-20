using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for BlockLockTester.
    /// </summary>
    partial class BlockLockTester : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.listBoxBlockInfo = new ListBox();
            this.buttonLock = new Button();
            this.buttonClose = new Button();
            this.buttonUnlock = new Button();
            this.buttonRemove = new Button();
            this.SuspendLayout();
            // 
            // listBoxBlockInfo
            // 
            this.listBoxBlockInfo.DisplayMember = "Name";
            this.listBoxBlockInfo.Location = new System.Drawing.Point(8, 16);
            this.listBoxBlockInfo.Name = "listBoxBlockInfo";
            this.listBoxBlockInfo.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxBlockInfo.Size = new System.Drawing.Size(144, 173);
            this.listBoxBlockInfo.TabIndex = 0;
            this.listBoxBlockInfo.SelectedIndexChanged += new EventHandler(this.ListBoxBlockInfo_SelectedIndexChanged);
            // 
            // buttonLock
            // 
            this.buttonLock.Location = new System.Drawing.Point(8, 240);
            this.buttonLock.Name = "buttonLock";
            this.buttonLock.Size = new System.Drawing.Size(64, 24);
            this.buttonLock.TabIndex = 3;
            this.buttonLock.Text = "Lock";
            this.buttonLock.Click += new EventHandler(this.ButtonLock_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(80, 240);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(64, 24);
            this.buttonClose.TabIndex = 4;
            this.buttonClose.Text = "Close";
            this.buttonClose.Click += new EventHandler(this.ButtonClose_Click);
            // 
            // buttonUnlock
            // 
            this.buttonUnlock.Location = new System.Drawing.Point(80, 200);
            this.buttonUnlock.Name = "buttonUnlock";
            this.buttonUnlock.Size = new System.Drawing.Size(64, 24);
            this.buttonUnlock.TabIndex = 2;
            this.buttonUnlock.Text = "Unlock";
            this.buttonUnlock.Click += new EventHandler(this.ButtonUnlock_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Location = new System.Drawing.Point(8, 200);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(64, 24);
            this.buttonRemove.TabIndex = 1;
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.Click += new EventHandler(this.ButtonRemove_Click);
            // 
            // BlockLockTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(160, 273);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonLock,
                                                                          this.listBoxBlockInfo,
                                                                          this.buttonClose,
                                                                          this.buttonUnlock,
                                                                          this.buttonRemove});
            this.Name = "BlockLockTester";
            this.Text = "Block Lock Testing";
            this.ResumeLayout(false);
        }
        #endregion
        private ListBox listBoxBlockInfo;
        private Button buttonLock;
        private Button buttonClose;
        private Button buttonUnlock;
        private Button buttonRemove;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}
