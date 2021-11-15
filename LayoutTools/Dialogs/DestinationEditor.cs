using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;
using LayoutManager.Components;
using LayoutManager.CommonUI;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for DestinationEditor.
    /// </summary>
    public partial class DestinationEditor : Form, IModelComponentReceiverDialog, IControlSupportViewOnly {
        private readonly TripPlanDestinationInfo inDestination;
        private readonly TripPlanDestinationInfo destination;
        private DialogEditing? changeToViewonly = null;
        private readonly LayoutSelection? destinationSelection = null;
        private readonly LayoutSelection? selectedBlockInfoSelection = null;

        public DestinationEditor(TripPlanDestinationInfo inDestination, string title) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.Text = title;
            this.inDestination = inDestination;

            XmlDocument tempDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();
            tempDoc.AppendChild(tempDoc.ImportNode(inDestination.Element, true));
            destination = new TripPlanDestinationInfo(tempDoc.DocumentElement);

            if (destination.HasName)
                textBoxName.Text = destination.Name;

            if (destination.SelectionMethod == TripPlanLocationSelectionMethod.ListOrder)
                radioButtonSelectionListOrder.Checked = true;
            else
                radioButtonSelectionRandom.Checked = true;

            foreach (TripPlanDestinationEntryInfo entry in destination.Entries)
                listViewLocations.Items.Add(new LocationItem(entry));

            EventManager.AddObjectSubscriptions(this);

            destinationSelection = new LayoutSelection();
            selectedBlockInfoSelection = new LayoutSelection();

            destinationSelection.Add(destination.Selection);

            destinationSelection.Display(new LayoutSelectionLook(Color.Orange));
            selectedBlockInfoSelection.Display(new LayoutSelectionLook(Color.OrangeRed));
        }

        private void UpdateButtons(object? sender, EventArgs e) {
            if (selectedBlockInfoSelection == null)
                return;

            selectedBlockInfoSelection.Clear();

            foreach (LocationItem item in listViewLocations.SelectedItems) {
                LayoutBlockDefinitionComponent blockInfo = item.BlockDefinition;

                if (blockInfo != null)
                    selectedBlockInfoSelection.Add(blockInfo);
            }

            if (listViewLocations.SelectedItems.Count > 0) {
                buttonRemoveLocation.Enabled = true;

                int selectedIndex = listViewLocations.SelectedIndices[0];

                buttonMoveLocationUp.Enabled = selectedIndex > 0;
                buttonMoveLocationDown.Enabled = selectedIndex < listViewLocations.Items.Count - 1;
                buttonCondition.Enabled = true;
            }
            else {
                buttonRemoveLocation.Enabled = false;
                buttonCondition.Enabled = false;

                buttonMoveLocationUp.Enabled = false;
                buttonMoveLocationDown.Enabled = false;
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

        [LayoutEvent("query-edit-destination-dialog")]
        private void QueryEditDestinationDialog(LayoutEvent e) {
            var blockDefinition = Ensure.NotNull<LayoutBlockDefinitionComponent>(e.Sender);
            var dialogs = Ensure.NotNull<List<IModelComponentReceiverDialog>>(e.Info);
            bool locationInList = false;

            foreach (LocationItem item in listViewLocations.Items)
                if (item.Entry.BlockId == blockDefinition.Block.Id) {
                    locationInList = true;
                    break;
                }

            if (Enabled && !ViewOnly && !locationInList)
                dialogs.Add(this);
        }

        // Implementation of IModelComponentReceiverComponent

        public string DialogName(IModelComponent component) => this.Text;

        public void AddComponent(IModelComponent component) {
            LayoutBlockDefinitionComponent blockInfo = (LayoutBlockDefinitionComponent)component;

            TripPlanDestinationEntryInfo newEntry = destination.Add(blockInfo);
            listViewLocations.Items.Add(new LocationItem(newEntry));
            destinationSelection?.Add(blockInfo);
            UpdateButtons(null, EventArgs.Empty);
        }

        public bool ViewOnly {
            get {
                return changeToViewonly != null;
            }

            set {
                if (value && !ViewOnly) {
                    Size newSize = new(buttonMoveLocationUp.Right - listViewLocations.Left, buttonAddLocation.Bottom - listViewLocations.Top);

                    changeToViewonly = new DialogEditing(this,
                        new DialogEditingCommandBase[] {
                                                           new DialogEditingRemoveControl(buttonAddLocation),
                                                           new DialogEditingRemoveControl(buttonMoveLocationDown),
                                                           new DialogEditingRemoveControl(buttonMoveLocationUp),
                                                           new DialogEditingRemoveControl(buttonCondition),
                                                           new DialogEditingRemoveControl(buttonOk),
                                                           new DialogEditingRemoveControl(buttonRemoveLocation),
                                                           new DialogEditingChangeText(buttonCancel, "Close"),
                                                           new DialogEditingResizeControl(listViewLocations, newSize)
                                                       });
                    changeToViewonly.Do();
                    changeToViewonly.ViewOnly = true;
                    this.ControlBox = false;
                    this.ShowInTaskbar = false;
                }
                else if (!value && ViewOnly) {
                    if (changeToViewonly != null) {
                        changeToViewonly.Undo();
                        changeToViewonly.ViewOnly = false;
                        changeToViewonly = null;
                    }
                    this.ControlBox = true;
                    this.ShowInTaskbar = true;
                }
            }
        }

        public bool HideSaveButton {
            get {
                return buttonSave.Visible;
            }

            set {
                buttonSave.Visible = false;
            }
        }

        private void ButtonOk_Click(object? sender, System.EventArgs e) {
            if (listViewLocations.Items.Count == 0) {
                MessageBox.Show(this, "Destination must contain at least one location", "Invalid destination", MessageBoxButtons.OK, MessageBoxIcon.Error);
                listViewLocations.Focus();
                return;
            }

            if (textBoxName.Text.Trim() == "")
                destination.HasName = false;
            else
                destination.Name = textBoxName.Text;

            if (radioButtonSelectionListOrder.Checked)
                destination.SelectionMethod = TripPlanLocationSelectionMethod.ListOrder;
            else
                destination.SelectionMethod = TripPlanLocationSelectionMethod.Random;

            destination.Clear();
            foreach (LocationItem item in listViewLocations.Items)
                destination.Add(item.Entry);

            var inParent = (XmlElement)inDestination.Element.ParentNode!;

            inParent.RemoveChild(inDestination.Element);
            inParent.AppendChild(inParent.OwnerDocument.ImportNode(destination.Element, true));

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void DestinationEditor_Closed(object? sender, System.EventArgs e) {
            destinationSelection?.Hide();
            destinationSelection?.Clear();

            selectedBlockInfoSelection?.Hide();
            selectedBlockInfoSelection?.Clear();

            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        private void ButtonMoveLocationUp_Click(object? sender, System.EventArgs e) {
            if (listViewLocations.SelectedItems.Count > 0) {
                LocationItem selected = (LocationItem)listViewLocations.SelectedItems[0];
                int selectedIndex = listViewLocations.SelectedIndices[0];

                if (selectedIndex > 0) {
                    listViewLocations.Items.Remove(selected);
                    listViewLocations.Items.Insert(selectedIndex - 1, selected);
                    selected.Selected = true;
                }
            }
        }

        private void ButtonMoveLocationDown_Click(object? sender, System.EventArgs e) {
            if (listViewLocations.SelectedItems.Count > 0) {
                LocationItem selected = (LocationItem)listViewLocations.SelectedItems[0];
                int selectedIndex = listViewLocations.SelectedIndices[0];

                if (selectedIndex < listViewLocations.Items.Count - 1) {
                    listViewLocations.Items.Remove(selected);
                    listViewLocations.Items.Insert(selectedIndex + 1, selected);
                    selected.Selected = true;
                }
            }
        }

        private void ButtonRemoveLocation_Click(object? sender, System.EventArgs e) {
            if (listViewLocations.SelectedItems.Count > 0) {
                LocationItem selected = (LocationItem)listViewLocations.SelectedItems[0];

                listViewLocations.Items.Remove(selected);
                destinationSelection?.Remove(selected.BlockDefinition);
                UpdateButtons(null, EventArgs.Empty);
            }
        }

        private void ButtonAddLocation_Click(object? sender, System.EventArgs e) {
            var menu = new ContextMenuStrip();

            new BuildComponentsMenu<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases, "*[@SuggestForDestination='true']",
                    new CommonUI.BuildComponentsMenuComponentFilter<LayoutBlockDefinitionComponent>(this.AddMenuFilter),
                    (s, _) => AddComponent((Ensure.NotNull<ModelComponentMenuItemBase<LayoutBlockDefinitionComponent>>(s)).Component)).AddComponentMenuItems(new MenuOrMenuItem(menu));

            if (menu.Items.Count > 0)
                menu.Show(groupBoxLocations, new Point(buttonAddLocation.Left, buttonAddLocation.Bottom));
        }

        private bool AddMenuFilter(LayoutBlockDefinitionComponent component) {
            foreach (LocationItem item in listViewLocations.Items)
                if (item.BlockDefinition.Id == component.Id)
                    return false;

            return true;
        }

        private void ButtonSave_Click(object? sender, System.EventArgs e) {
            CommonUI.Dialogs.InputBox inBox = new("Save Destination", "Enter destination name:") {
                Input = textBoxName.Text
            };

            if (inBox.ShowDialog(this) == DialogResult.OK) {
                bool doSave = true;
                string name = inBox.Input;
                TripPlanCatalogInfo tripPlanCatalog = LayoutModel.StateManager.TripPlansCatalog;

                TripPlanDestinationInfo existingDestination = tripPlanCatalog.Destinations[name];

                if (existingDestination != null) {
                    if (MessageBox.Show(this, "A destination with name already exists, would you like to replace it?",
                        "Destination already exist", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        doSave = false;
                    else
                        tripPlanCatalog.Destinations.Remove(existingDestination);
                }

                if (doSave) {
                    XmlElement catalogDestinationElement = LayoutModel.StateManager.TripPlansCatalog.Element.OwnerDocument.CreateElement("Destination");
                    TripPlanDestinationInfo catalogDestination = new(catalogDestinationElement);

                    foreach (LocationItem item in listViewLocations.Items) {
                        XmlElement catalogDestinationEntryElement = (XmlElement)catalogDestinationElement.OwnerDocument.ImportNode(item.Entry.Element, true);

                        catalogDestination.Add(new TripPlanDestinationEntryInfo(catalogDestinationEntryElement));
                    }

                    catalogDestination.Name = name;

                    if (radioButtonSelectionListOrder.Checked)
                        catalogDestination.SelectionMethod = TripPlanLocationSelectionMethod.ListOrder;
                    else
                        catalogDestination.SelectionMethod = TripPlanLocationSelectionMethod.Random;

                    LayoutModel.StateManager.TripPlansCatalog.Destinations.Add(catalogDestination);
                }
            }
        }

        private void DestinationEditor_Closing(object? sender, System.ComponentModel.CancelEventArgs e) {
            Owner = null;
        }

        private void ButtonCondition_Click(object? sender, System.EventArgs e) {
            if (listViewLocations.SelectedItems.Count > 0) {
                LocationItem selected = (LocationItem)listViewLocations.SelectedItems[0];

                selected.Edit(this);
            }
        }

        private void ListViewLocations_SelectedIndexChanged(object? sender, System.EventArgs e) {
            UpdateButtons(null, EventArgs.Empty);
        }

        private class LocationItem : ListViewItem {
            private TripPlanDestinationEntryInfo entry;

            public LocationItem(TripPlanDestinationEntryInfo entry) {
                this.entry = entry;

                SubItems.Add("");
                Update();
            }

            public void Update() {
                LayoutBlock block = LayoutModel.Blocks[entry.BlockId];

                if (block != null)
                    SubItems[0].Text = block.BlockDefinintion.Name;
                else
                    SubItems[0].Text = "*UNKNOWN*";

                SubItems[1].Text = entry.GetDescription();
            }

            public TripPlanDestinationEntryInfo Entry => entry;

            public LayoutBlockDefinitionComponent BlockDefinition {
                get {
                    var block = Ensure.NotNull<LayoutBlock>(LayoutModel.Blocks[entry.BlockId]);

                    return block.BlockDefinintion;
                }
            }

            public bool Edit(IWin32Window owner) {
                CommonUI.Dialogs.TrainConditionDefinition d = new(BlockDefinition, entry);

                if (d.ShowDialog(owner) == DialogResult.OK) {
                    var parentElement = (XmlElement?)entry.Element.ParentNode;
                    XmlElement newEntryElement = (XmlElement)entry.Element.OwnerDocument.ImportNode(d.TrainCondition.Element, true);

                    parentElement?.ReplaceChild(newEntryElement, entry.Element);
                    entry = new TripPlanDestinationEntryInfo(newEntryElement);

                    Update();
                    return true;
                }
                else
                    return false;
            }
        }
    }
}
