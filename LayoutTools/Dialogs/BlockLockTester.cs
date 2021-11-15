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
    public partial class BlockLockTester : Form {
        public BlockLockTester(LayoutBlockDefinitionComponent blockInfo) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            AddBlockInfo(blockInfo);
        }

        public void AddBlockInfo(LayoutBlockDefinitionComponent blockInfo) {
            listBoxBlockInfo.Items.Add(blockInfo);
            UpdateButtons();
        }

        private void UpdateButtons() {
            if (listBoxBlockInfo.SelectedItems.Count > 0) {
                buttonRemove.Enabled = true;
                buttonUnlock.Enabled = true;
            }
            else {
                buttonRemove.Enabled = false;
                buttonUnlock.Enabled = false;
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void ListBoxBlockInfo_SelectedIndexChanged(object? sender, System.EventArgs e) {
            UpdateButtons();
        }

        private void ButtonRemove_Click(object? sender, System.EventArgs e) {
            ArrayList removeList = new();

            foreach (LayoutBlockDefinitionComponent blockInfo in listBoxBlockInfo.SelectedItems)
                removeList.Add(blockInfo);
            foreach (LayoutBlockDefinitionComponent blockInfo in removeList)
                listBoxBlockInfo.Items.Remove(blockInfo);
        }

        private void ButtonUnlock_Click(object? sender, System.EventArgs e) {
            Guid[] blockIDs = new Guid[listBoxBlockInfo.SelectedItems.Count];

            int i = 0;
            foreach (LayoutBlockDefinitionComponent blockInfo in listBoxBlockInfo.SelectedItems)
                blockIDs[i++] = blockInfo.Block.Id;

            EventManager.Event(new LayoutEvent("free-layout-lock", blockIDs));
        }

        private void ButtonLock_Click(object? sender, System.EventArgs e) {
            LayoutLockRequest request = new();

            foreach (LayoutBlockDefinitionComponent blockInfo in listBoxBlockInfo.Items)
                request.Blocks.Add(blockInfo.Block);

            request.OwnerId = Guid.NewGuid();

            EventManager.Event(new LayoutEvent("request-layout-lock", request));
        }

        private void ButtonClose_Click(object? sender, System.EventArgs e) {
            EventManager.Event(new LayoutEvent("block-tester-dialog-closed", this));
            Close();
        }
    }
}
