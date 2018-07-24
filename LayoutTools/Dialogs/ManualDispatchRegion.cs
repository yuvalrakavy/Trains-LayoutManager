using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using LayoutManager.CommonUI;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for ManualDispatchRegion.
    /// </summary>
    public class ManualDispatchRegion : Form, IModelComponentReceiverDialog, IObjectHasXml
	{
		private ListBox listBoxBlocks;
		private Button buttonAddBlock;
		private Button buttonRemoveBlock;
		private Label label1;
		private TextBox textBoxName;
		private Button buttonOk;
		private Button buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private void endOfDesignerVariables() { }

		ManualDispatchRegionInfo	manualDispatchRegion;
		LayoutSelection				regionSelection;
		LayoutSelection				selectedBlockSelection;

		public ManualDispatchRegion(ManualDispatchRegionInfo manualDispatchRegion)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.manualDispatchRegion = manualDispatchRegion;

			textBoxName.Text = manualDispatchRegion.Name;
			setDialogName();

			regionSelection = new LayoutSelection();
			selectedBlockSelection = new LayoutSelection();

			foreach(Guid blockID in manualDispatchRegion.BlockIdList) {
				LayoutBlock	block = LayoutModel.Blocks[blockID];

				Debug.Assert(block != null);
				listBoxBlocks.Items.Add(block);
				regionSelection.Add(block.GetSelection());
			}

			updateButtons(null, null);

			regionSelection.Display(new LayoutSelectionLook(Color.DarkCyan));
			selectedBlockSelection.Display(new LayoutSelectionLook(Color.DarkMagenta));

			EventManager.AddObjectSubscriptions(this);
		}

        public XmlElement Element => manualDispatchRegion.Element;

        private void updateButtons(object sender, EventArgs e) {
			LayoutBlock	selectedBlock = (LayoutBlock)listBoxBlocks.SelectedItem;

			selectedBlockSelection.Clear();

			if(selectedBlock != null) {
				selectedBlockSelection.Add(selectedBlock.GetSelection());
				buttonRemoveBlock.Enabled = true;
			}
			else
				buttonRemoveBlock.Enabled = false;
		}

		void setDialogName() {
			this.Text = "Manual Dispatch Region - " + textBoxName.Text;
		}

		[LayoutEvent("query-edit-manual-dispatch-region-dialog")]
		private void queryEditManualDispatchRegionDialog(LayoutEvent e0) {
			var e = (LayoutEvent<LayoutBlockDefinitionComponent, List<IModelComponentReceiverDialog>>)e0;

			e.Info.Add((IModelComponentReceiverDialog)this);
		}

		[LayoutEvent("manual-dispatch-region-activation-changed", IfSender="*[@ID='`string(@ID)`']")]
		private void manualDispatchRegionActivationChanged(LayoutEvent e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}

        // Impelementation of IModelComponentReceiverComponent

        public string DialogName(IModelComponent component) => this.Text;

        public void AddComponent(IModelComponent component) {
			LayoutBlockDefinitionComponent	addedBlockInfo = (LayoutBlockDefinitionComponent)component;
			LayoutBlock					addedBlock = addedBlockInfo.Block;
			bool						blockFound = false;

			// If this block is already in the region, select it, otherwise add it

			foreach(LayoutBlock block in listBoxBlocks.Items) {
				if(block.Id == addedBlock.Id) {
					listBoxBlocks.SelectedItem = block;
					blockFound = true;
					break;
				}
			}

			if(!blockFound) {
				listBoxBlocks.Items.Add(addedBlock);
				regionSelection.Add(addedBlock.GetSelection());
			}

			updateButtons(null, null);
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
			this.listBoxBlocks = new ListBox();
			this.buttonAddBlock = new Button();
			this.buttonRemoveBlock = new Button();
			this.buttonOk = new Button();
			this.label1 = new Label();
			this.textBoxName = new TextBox();
			this.buttonCancel = new Button();
			this.SuspendLayout();
			// 
			// listBoxBlocks
			// 
			this.listBoxBlocks.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.listBoxBlocks.DisplayMember = "Name";
			this.listBoxBlocks.Location = new System.Drawing.Point(8, 32);
			this.listBoxBlocks.Name = "listBoxBlocks";
			this.listBoxBlocks.Size = new System.Drawing.Size(216, 199);
			this.listBoxBlocks.TabIndex = 0;
			this.listBoxBlocks.SelectedIndexChanged += new System.EventHandler(this.updateButtons);
			// 
			// buttonAddBlock
			// 
			this.buttonAddBlock.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonAddBlock.Location = new System.Drawing.Point(8, 240);
			this.buttonAddBlock.Name = "buttonAddBlock";
			this.buttonAddBlock.Size = new System.Drawing.Size(67, 22);
			this.buttonAddBlock.TabIndex = 1;
			this.buttonAddBlock.Text = "&Add";
			this.buttonAddBlock.Click += new System.EventHandler(this.buttonAddBlock_Click);
			// 
			// buttonRemoveBlock
			// 
			this.buttonRemoveBlock.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonRemoveBlock.Location = new System.Drawing.Point(80, 240);
			this.buttonRemoveBlock.Name = "buttonRemoveBlock";
			this.buttonRemoveBlock.Size = new System.Drawing.Size(67, 22);
			this.buttonRemoveBlock.TabIndex = 2;
			this.buttonRemoveBlock.Text = "&Remove";
			this.buttonRemoveBlock.Click += new System.EventHandler(this.buttonRemoveBlock_Click);
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.buttonOk.Location = new System.Drawing.Point(88, 274);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(67, 22);
			this.buttonOk.TabIndex = 3;
			this.buttonOk.Text = "OK";
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 16);
			this.label1.TabIndex = 4;
			this.label1.Text = "Region Name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxName
			// 
			this.textBoxName.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.textBoxName.Location = new System.Drawing.Point(88, 8);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(136, 20);
			this.textBoxName.TabIndex = 5;
			this.textBoxName.Text = "";
			this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(160, 274);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(67, 22);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// ManualDispatchRegion
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(232, 302);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.textBoxName,
																		  this.label1,
																		  this.buttonAddBlock,
																		  this.listBoxBlocks,
																		  this.buttonRemoveBlock,
																		  this.buttonOk,
																		  this.buttonCancel});
			this.Name = "ManualDispatchRegion";
			this.ShowInTaskbar = false;
			this.Text = "Manual Dispatch Region";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.ManualDispatchRegion_Closing);
			this.Closed += new System.EventHandler(this.ManualDispatchRegion_Closed);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonRemoveBlock_Click(object sender, System.EventArgs e) {
			LayoutBlock	selectedBlock = (LayoutBlock)listBoxBlocks.SelectedItem;

			if(selectedBlock != null) {
				listBoxBlocks.Items.Remove(selectedBlock);
				regionSelection.Remove(selectedBlock.GetSelection());
				updateButtons(null, null);
			}
		}

		private void textBoxName_TextChanged(object sender, System.EventArgs e) {
			setDialogName();		
		}

		private void buttonAddBlock_Click(object sender, System.EventArgs e) {
			ContextMenu	menu = new ContextMenu();

			new BuildComponentsMenu<LayoutBlockDefinitionComponent>(
				LayoutModel.ActivePhases,
				delegate(LayoutBlockDefinitionComponent c) { return !listBoxBlocks.Items.Contains(c.Block); },
				delegate(object s, EventArgs ea) { AddComponent(((ModelComponentMenuItemBase<LayoutBlockDefinitionComponent>)s).Component); }).AddComponentMenuItems(menu);

			if(menu.MenuItems.Count > 0)
				menu.Show(this, new Point(buttonAddBlock.Left, buttonAddBlock.Bottom));
		}

		private void buttonOk_Click(object sender, System.EventArgs e) {
			manualDispatchRegion.BlockIdList.Clear();

			if(textBoxName.Text.Trim() == "") {
				MessageBox.Show(this, "You did not provide name for the region", "Missing Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBoxName.Focus();
				return;
			}

			if(listBoxBlocks.Items.Count == 0) {
				MessageBox.Show(this, "Region does not contain any block", "Missing Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
				listBoxBlocks.Focus();
				return;
			}

			manualDispatchRegion.Name = textBoxName.Text;

			foreach(LayoutBlock block in listBoxBlocks.Items)
				manualDispatchRegion.BlockIdList.Add(block.Id);

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void ManualDispatchRegion_Closed(object sender, System.EventArgs e) {
			regionSelection.Hide();
			selectedBlockSelection.Hide();
			EventManager.Subscriptions.RemoveObjectSubscriptions(this);
		}

		private void ManualDispatchRegion_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
		}
	}
}
