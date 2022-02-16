using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using MethodDispatcher;
using LayoutManager.CommonUI;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for ManualDispatchRegion.
    /// </summary>
    public partial class ManualDispatchRegion : Form, IModelComponentReceiverDialog, IObjectHasXml {
        /// <summary>
        /// Required designer variable.
        /// </summary>

        private readonly ManualDispatchRegionInfo manualDispatchRegion;
        private readonly LayoutSelection regionSelection;
        private readonly LayoutSelection selectedBlockSelection;

        public ManualDispatchRegion(ManualDispatchRegionInfo manualDispatchRegion) {
        //
        // Required for Windows Form Designer support
        //
            InitializeComponent();

            this.manualDispatchRegion = manualDispatchRegion;

            textBoxName.Text = manualDispatchRegion.Name;
            SetDialogName();

            regionSelection = new LayoutSelection();
            selectedBlockSelection = new LayoutSelection();

            foreach (Guid blockID in manualDispatchRegion.BlockIdList) {
                LayoutBlock block = LayoutModel.Blocks[blockID];

                Debug.Assert(block != null);
                listBoxBlocks.Items.Add(block);
                regionSelection.Add(block.GetSelection());
            }

            UpdateButtons();

            regionSelection.Display(new LayoutSelectionLook(Color.DarkCyan));
            selectedBlockSelection.Display(new LayoutSelectionLook(Color.DarkMagenta));

            EventManager.AddObjectSubscriptions(this);
            Dispatch.AddObjectInstanceDispatcherTargets(this);
        }

        public XmlElement Element => manualDispatchRegion.Element;
        public XmlElement? OptionalElement => Element;

        private void UpdateButtons() {
            LayoutBlock selectedBlock = (LayoutBlock)listBoxBlocks.SelectedItem;

            selectedBlockSelection.Clear();

            if (selectedBlock != null) {
                selectedBlockSelection.Add(selectedBlock.GetSelection());
                buttonRemoveBlock.Enabled = true;
            }
            else
                buttonRemoveBlock.Enabled = false;
        }

        private void SetDialogName() {
            this.Text = "Manual Dispatch Region - " + textBoxName.Text;
        }

        [LayoutEvent("query-edit-manual-dispatch-region-dialog")]
        private void QueryEditManualDispatchRegionDialog(LayoutEvent e) {
            var componentReceiverDialogs = Ensure.NotNull<List <IModelComponentReceiverDialog>>(e.Info, "receiverDialogs");

            componentReceiverDialogs.Add((IModelComponentReceiverDialog)this);
        }

        [DispatchTarget]
        private void OnManualDispatchRegionActivationChanged([DispatchFilter(Type="IsMyId")] ManualDispatchRegionInfo manualDispatchRegion, bool active) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // Implementation of IModelComponentReceiverComponent

        public string DialogName(IModelComponent component) => this.Text;

        public void AddComponent(IModelComponent component) {
            LayoutBlockDefinitionComponent addedBlockInfo = (LayoutBlockDefinitionComponent)component;
            LayoutBlock addedBlock = addedBlockInfo.Block;
            bool blockFound = false;

            // If this block is already in the region, select it, otherwise add it

            foreach (LayoutBlock block in listBoxBlocks.Items) {
                if (block.Id == addedBlock.Id) {
                    listBoxBlocks.SelectedItem = block;
                    blockFound = true;
                    break;
                }
            }

            if (!blockFound) {
                listBoxBlocks.Items.Add(addedBlock);
                regionSelection.Add(addedBlock.GetSelection());
            }

            UpdateButtons();
        }

        private void SelectedIndexChanged(object? sender, EventArgs e) {
            UpdateButtons();
        }

        private void ButtonRemoveBlock_Click(object? sender, System.EventArgs e) {
            LayoutBlock selectedBlock = (LayoutBlock)listBoxBlocks.SelectedItem;

            if (selectedBlock != null) {
                listBoxBlocks.Items.Remove(selectedBlock);
                regionSelection.Remove(selectedBlock.GetSelection());
                UpdateButtons();
            }
        }

        private void TextBoxName_TextChanged(object? sender, System.EventArgs e) {
            SetDialogName();
        }

        private void ButtonAddBlock_Click(object? sender, System.EventArgs e) {
            var menu = new ContextMenuStrip();

            new BuildComponentsMenu<LayoutBlockDefinitionComponent>(
                LayoutModel.ActivePhases,
                (LayoutBlockDefinitionComponent c) => !listBoxBlocks.Items.Contains(c.Block),
                (object? s, EventArgs ea) => AddComponent((Ensure.NotNull<ModelComponentMenuItemBase<LayoutBlockDefinitionComponent>>(s)).Component)).AddComponentMenuItems(new MenuOrMenuItem(menu));

            if (menu.Items.Count > 0)
                menu.Show(this, new Point(buttonAddBlock.Left, buttonAddBlock.Bottom));
        }

        private void ButtonOk_Click(object? sender, System.EventArgs e) {
            manualDispatchRegion.BlockIdList.Clear();

            if (textBoxName.Text.Trim()?.Length == 0) {
                MessageBox.Show(this, "You did not provide name for the region", "Missing Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxName.Focus();
                return;
            }

            if (listBoxBlocks.Items.Count == 0) {
                MessageBox.Show(this, "Region does not contain any block", "Missing Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                listBoxBlocks.Focus();
                return;
            }

            manualDispatchRegion.Name = textBoxName.Text;

            foreach (LayoutBlock block in listBoxBlocks.Items)
                manualDispatchRegion.BlockIdList.Add(block.Id);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ManualDispatchRegion_Closed(object? sender, System.EventArgs e) {
            regionSelection.Hide();
            selectedBlockSelection.Hide();
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        private void ManualDispatchRegion_Closing(object? sender, System.ComponentModel.CancelEventArgs e) {
        }
    }
}
