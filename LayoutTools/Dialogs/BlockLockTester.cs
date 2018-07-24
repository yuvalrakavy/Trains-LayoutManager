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
    public class BlockLockTester : Form {
		private ListBox listBoxBlockInfo;
		private Button buttonLock;
		private Button buttonClose;
		private Button buttonUnlock;
		private Button buttonRemove;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public BlockLockTester(LayoutBlockDefinitionComponent blockInfo)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			AddBlockInfo(blockInfo);
		}

		public void AddBlockInfo(LayoutBlockDefinitionComponent blockInfo) {
			listBoxBlockInfo.Items.Add(blockInfo);
			updateButtons();
		}

		private void updateButtons() {
			if(listBoxBlockInfo.SelectedItems.Count > 0) {
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
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
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
			this.listBoxBlockInfo.SelectedIndexChanged += new System.EventHandler(this.listBoxBlockInfo_SelectedIndexChanged);
			// 
			// buttonLock
			// 
			this.buttonLock.Location = new System.Drawing.Point(8, 240);
			this.buttonLock.Name = "buttonLock";
			this.buttonLock.Size = new System.Drawing.Size(64, 24);
			this.buttonLock.TabIndex = 3;
			this.buttonLock.Text = "Lock";
			this.buttonLock.Click += new System.EventHandler(this.buttonLock_Click);
			// 
			// buttonClose
			// 
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonClose.Location = new System.Drawing.Point(80, 240);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(64, 24);
			this.buttonClose.TabIndex = 4;
			this.buttonClose.Text = "Close";
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// buttonUnlock
			// 
			this.buttonUnlock.Location = new System.Drawing.Point(80, 200);
			this.buttonUnlock.Name = "buttonUnlock";
			this.buttonUnlock.Size = new System.Drawing.Size(64, 24);
			this.buttonUnlock.TabIndex = 2;
			this.buttonUnlock.Text = "Unlock";
			this.buttonUnlock.Click += new System.EventHandler(this.buttonUnlock_Click);
			// 
			// buttonRemove
			// 
			this.buttonRemove.Location = new System.Drawing.Point(8, 200);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(64, 24);
			this.buttonRemove.TabIndex = 1;
			this.buttonRemove.Text = "Remove";
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			// 
			// BlockLockTester
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
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

		private void listBoxBlockInfo_SelectedIndexChanged(object sender, System.EventArgs e) {
			updateButtons();
		}

		private void buttonRemove_Click(object sender, System.EventArgs e) {
			ArrayList	removeList = new ArrayList();

			foreach(LayoutBlockDefinitionComponent blockInfo in listBoxBlockInfo.SelectedItems)
				removeList.Add(blockInfo);
			foreach(LayoutBlockDefinitionComponent blockInfo in removeList)
				listBoxBlockInfo.Items.Remove(blockInfo);
		}

		private void buttonUnlock_Click(object sender, System.EventArgs e) {
			Guid[]	blockIDs = new Guid[listBoxBlockInfo.SelectedItems.Count];

			int		i = 0;
			foreach(LayoutBlockDefinitionComponent blockInfo in listBoxBlockInfo.SelectedItems)
				blockIDs[i++] = blockInfo.Block.Id;

			EventManager.Event(new LayoutEvent(blockIDs, "free-layout-lock"));
		}

		private void buttonLock_Click(object sender, System.EventArgs e) {
			LayoutLockRequest	request = new LayoutLockRequest();

			foreach(LayoutBlockDefinitionComponent blockInfo in listBoxBlockInfo.Items)
				request.Blocks.Add(blockInfo.Block);

			request.OwnerId = Guid.NewGuid();

			EventManager.Event(new LayoutEvent(request, "request-layout-lock"));
		}

		private void buttonClose_Click(object sender, System.EventArgs e) {
			EventManager.Event(new LayoutEvent(this, "block-tester-dialog-closed"));
			Close();
		}
	}


}
