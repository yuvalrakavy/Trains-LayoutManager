using LayoutManager.CommonUI;
using LayoutManager.CommonUI.Controls;
using LayoutManager.Components;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager {
    /// <summary>
    /// Summary description for LocomotivesViewer.
    /// </summary>
    public partial class LocomotivesViewer : UserControl {
        private const string E_Locomotive = "Locomotive";
        private const string A_Id = "ID";
        private const string A_Store = "Store";
        private const string E_Train = "Train";

        private LocomotiveCollectionInfo? locomotiveCollection;
        private bool operationMode = false;

        public LocomotivesViewer() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        public LocomotiveCollectionInfo LocomotiveCollection {
            set {
                locomotiveCollection = value;
                if (locomotiveCollection != null)
                    locomotiveList.ContainerElement = locomotiveCollection.CollectionElement;
                locomotiveList.CurrentListLayoutIndex = 0;
                UpdateButtons();
            }

            get {
                return Ensure.NotNull<LocomotiveCollectionInfo>(locomotiveCollection);
            }
        }

        /// <summary>
        /// Check if an item can be deleted. In operation mode, an item can be deleted only if it is not on track (no state information)
        /// </summary>
        /// <param name="selectedElement"></param>
        /// <returns>True - item can be deleted</returns>
        private bool CanDelete(XmlElement selectedElement) {
            bool result = true;

            if (operationMode) {
                if (selectedElement.Name == E_Locomotive) {
                    LocomotiveInfo loco = new(selectedElement);

                    result = LayoutModel.StateManager.Trains[loco.Id] == null;
                }
                else if (selectedElement.Name == E_Train) {
                    TrainInCollectionInfo train = new(selectedElement);

                    result = LayoutModel.StateManager.Trains[train.Id] == null;
                }
            }

            return result;
        }

        private void UpdateButtons() {
            var selectedElement = locomotiveList.SelectedXmlElement;

            if (selectedElement != null) {
                buttonEdit.Enabled = true;
                buttonDelete.Enabled = CanDelete(selectedElement);
            }
            else {
                buttonEdit.Enabled = false;
                buttonDelete.Enabled = false;
            }
        }

        public void Initialize() {
            EventManager.AddObjectSubscriptions(this);
            Dispatch.AddObjectInstanceDispatcherTargets(this);

            locomotiveList.Initialize();
            locomotiveList.AddLayoutMenuItems(new MenuOrMenuItem(menuItemArrange));
            locomotiveList.AddLayoutMenuItems(new MenuOrMenuItem(contextMenuArrange));
        }

        [DispatchTarget]
        private void OnEnteredDesignMode() {
            operationMode = false;
            buttonOptions.Text = "&Options";
        }

        [DispatchTarget]
        private void OnEnteredOperationMode(OperationModeParameters settings) {
            operationMode = true;
            buttonOptions.Text = "&Arrange";
        }

        [DispatchTarget]
        private void OnTrainCreated(TrainStateInfo train, LayoutBlockDefinitionComponent blockDefinition) {
            UpdateButtons();
        }

        [DispatchTarget]
        private void OnTrainIsRemoved(TrainStateInfo train) {
            UpdateButtons();
        }

        [DispatchTarget]
        private void DisableLocomotiveListUpdate() {
            locomotiveList.DisableUpdate();
        }

        [DispatchTarget]
        private void EnableLocomotiveListUpdate() {
            locomotiveList.EnableUpdate();
        }

        protected void SaveModelDocument() {
            LayoutModel.WriteModelXmlInfo();
        }

        [LayoutEvent("locomotive-set-updated-locomotive-definition")]
        private void LocomotiveDefinitionUpdated(LayoutEvent e) {
            locomotiveCollection?.Save();
        }

        [DispatchTarget]
        private void EditLocomotiveProperties(LocomotiveInfo loco) {
            Dialogs.LocomotiveProperties locoProperties = new(loco);

            if (locoProperties.ShowDialog(this) == DialogResult.OK)
                locomotiveCollection?.Save();
        }

        [DispatchTarget]
        private void AddNewLocomotiveToCollection() {
            if (locomotiveCollection == null)
                return;

            XmlElement locoElement = locomotiveCollection.CollectionDocument.CreateElement(E_Locomotive);

            locoElement.SetAttributeValue(A_Id, Guid.NewGuid());
            locoElement.SetAttributeValue(A_Store, locomotiveCollection.DefaultStore);

            LocomotiveInfo loco = new(locoElement);
            Dialogs.LocomotiveProperties locoProperties = new(loco);

            if (locoProperties.ShowDialog(this) == DialogResult.OK) {
                locomotiveCollection.CollectionElement.AppendChild(loco.Element);
                locomotiveCollection.Save();
            }
        }

        [DispatchTarget]
        private void AddNewTrainToCollection() {
            if (locomotiveCollection == null)
                return;

            XmlElement trainInCollectionElement = locomotiveCollection.CollectionDocument.CreateElement(E_Train);
            TrainInCollectionInfo trainInCollection = new(trainInCollectionElement);

            trainInCollection.Initialize();

            Tools.Dialogs.TrainProperties trainDialog = new(trainInCollection);

            if (trainDialog.ShowDialog(this) == DialogResult.OK) {
                locomotiveCollection.CollectionElement.AppendChild(trainInCollectionElement);
                locomotiveCollection.Save();
            }
        }

        [LayoutEvent("edit-locomotive-collection-item")]
        private void EditLocomotiveCollectionItem(LayoutEvent e) {
            var selectedElement = (XmlElement?)e.Sender;

            if (selectedElement != null) {
                if (selectedElement.Name == E_Locomotive) {
                    LocomotiveInfo loco = new(selectedElement);
                    Dialogs.LocomotiveProperties locoProperties = new(loco);

                    if (locoProperties.ShowDialog(this) == DialogResult.OK)
                        locomotiveCollection?.Save();
                }
                else if (selectedElement.Name == E_Train) {
                    Tools.Dialogs.TrainProperties trainDialog = new(new TrainInCollectionInfo(selectedElement));

                    if (trainDialog.ShowDialog(this) == DialogResult.OK)
                        locomotiveCollection?.Save();
                }
            }
        }

        [LayoutEvent("delete-locomotive-collection-item")]
        private void DeleteLocomotiveCollectionItem(LayoutEvent e) {
            var selectedElement = (XmlElement?)e.Sender;

            if (selectedElement != null && CanDelete(selectedElement)) {
                selectedElement.ParentNode?.RemoveChild(selectedElement);
                locomotiveCollection?.EnsureReferentialIntegrity();
                locomotiveCollection?.Save();
            }
        }

        [DispatchTarget]
        [DispatchFilter("InDesignMode")]
        protected void AddLocomotiveCollectionContextMenuEntries_Design(XmlElement clickedElement, MenuOrMenuItem m) {
            if (clickedElement != null) {
                m.Items.Add(new EditLocomotiveCollectionItemPropertiesMenuItem(clickedElement, "&Properties..."));
                m.Items.Add(new DeleteLocomotiveCollectionItemMenuItem(clickedElement, CanDelete(clickedElement)));
            }
        }

        [DispatchTarget]
        [DispatchFilter("InOperationMode")]
        protected void AddLocomotiveCollectionContextMenuEntries_Operation(XmlElement clickedElement, MenuOrMenuItem m) {
            if (m.Items.Count > 0)
                m.Items.Add(new ToolStripSeparator());

            if (clickedElement.Name == E_Locomotive)
                m.Items.Add(new EditLocomotiveCollectionItemPropertiesMenuItem(clickedElement, "&Locomotive properties..."));
            m.Items.Add(new DeleteLocomotiveCollectionItemMenuItem(clickedElement, CanDelete(clickedElement)));
        }

        #region Context menu items

        private class EditLocomotiveCollectionItemPropertiesMenuItem : LayoutMenuItem {
            private readonly XmlElement element;

            public EditLocomotiveCollectionItemPropertiesMenuItem(XmlElement element, string menuText) {
                this.element = element;
                Text = menuText;
            }

            protected override void OnClick(EventArgs e) {
                EventManager.Event(new LayoutEvent("edit-locomotive-collection-item", element));
            }
        }

        private class DeleteLocomotiveCollectionItemMenuItem : LayoutMenuItem {
            private readonly XmlElement element;

            public DeleteLocomotiveCollectionItemMenuItem(XmlElement element, bool canDelete) {
                this.element = element;
                Text = "&Delete";

                Enabled = canDelete;
            }

            protected override void OnClick(EventArgs e) {
                EventManager.Event(new LayoutEvent("delete-locomotive-collection-item", element));
            }
        }

        #endregion

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

        private void ButtonAdd_Click(object? sender, EventArgs e) {
            contextMenuAdd.Show(this, new Point(buttonAdd.Left, buttonAdd.Bottom + 2));
        }

        private void MenuItemAddLocomotive_Click(object? sender, EventArgs e) => Dispatch.Call.AddNewLocomotiveToCollection();

        private void ButtonOptions_Click(object? sender, EventArgs e) {
            if (!operationMode)
                contextMenuOptions.Show(this, new Point(buttonOptions.Left, buttonOptions.Bottom));
            else
                contextMenuArrange.Show(this, new Point(buttonOptions.Left, buttonOptions.Bottom));
        }

        private void MenuItemStorage_Click(object? sender, EventArgs e) {
            if (locomotiveCollection == null)
                return;

            Dialogs.LocomotiveCollectionStores stores = new("Locomotive Collection",
                locomotiveCollection.Element["Stores"]!, "Locomotive Collection",
                locomotiveCollection.DefaultStoreDirectory, ".LocomotiveCollection") {
                Text = "Locomotive Collection Storage"
            };
            locomotiveCollection.Unload();

            stores.ShowDialog(this);
            locomotiveCollection.Load();
            locomotiveCollection.EnsureReferentialIntegrity();

            // Force to re-layout the list
            locomotiveList.ContainerElement = locomotiveCollection.CollectionElement;
            SaveModelDocument();
        }

        private void LocomotiveList_SelectedIndexChanged(object? sender, EventArgs e) {
            UpdateButtons();
        }

        private void ButtonEdit_Click(object? sender, EventArgs e) {
            var selectedElement = locomotiveList.SelectedXmlElement;

            EventManager.Event(new LayoutEvent("edit-locomotive-collection-item", selectedElement));
        }

        private void ButtonDelete_Click(object? sender, EventArgs e) {
            var selectedElement = locomotiveList.SelectedXmlElement;

            EventManager.Event(new LayoutEvent("delete-locomotive-collection-item", selectedElement));
        }

        private void MenuItemAddTrain_Click(object? sender, EventArgs e) => Dispatch.Call.AddNewTrainToCollection();

        private void LocomotiveList_MouseDown(object? sender, MouseEventArgs e) {
            if ((e.Button & MouseButtons.Right) != 0) {
                int clickedIndex = locomotiveList.IndexFromPoint(e.X, e.Y);

                if (clickedIndex >= 0) {
                    if (locomotiveList.Items[clickedIndex] is IXmlQueryListBoxXmlElementItem clickedItem) {
                        var m = new ContextMenuStrip();

                        Dispatch.Call.AddLocomotiveCollectionContextMenuEntries(clickedItem.Element, new MenuOrMenuItem(m));

                        if (m.Items.Count > 0) {
                            Rectangle itemRect = locomotiveList.GetItemRectangle(clickedIndex);

                            m.Show(this, new Point(itemRect.Left, (itemRect.Top + itemRect.Bottom) / 2));
                        }
                    }
                }
            }
        }

        private void ButtonClose_Click(object? sender, EventArgs e) {
            EventManager.Event(new LayoutEvent("hide-locomotives", this));
        }
    }
}
