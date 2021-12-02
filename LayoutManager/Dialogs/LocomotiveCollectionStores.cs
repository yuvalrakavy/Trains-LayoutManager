using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveCollectionStores.
    /// </summary>
    public partial class LocomotiveCollectionStores : Form {
        private const string A_DefaultStore = "DefaultStore";

        private readonly XmlElement storesElement;
        private readonly string storesName;
        private readonly string collectionDescription;
        private readonly string defaultDirectory;
        private readonly string defaultExtension;

        public LocomotiveCollectionStores(string storesName, XmlElement storesElement, string collectionDescription, string defaultDirectory, string defaultExtension) {
            this.collectionDescription = collectionDescription;
            this.defaultDirectory = defaultDirectory;
            this.defaultExtension = defaultExtension;
            this.storesName = storesName;
            this.storesElement = storesElement;

            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.Text = storesName + " Storage";

            foreach (XmlElement storeElement in storesElement)
                listViewStores.Items.Add(new StoreItem(storeElement));

            if (storesElement.HasAttribute(A_DefaultStore)) {
                StoreItem defaultItem = (StoreItem)listViewStores.Items[(int)storesElement.AttributeValue(A_DefaultStore)];

                defaultItem.IsDefault = true;
                defaultItem.Update();
            }

            UpdateButtons();
        }

        private void UpdateButtons() {
            if (listViewStores.SelectedItems.Count > 0) {
                buttonEdit.Enabled = true;
                buttonRemove.Enabled = true;
                buttonSetAsDefault.Enabled = true;
            }
            else {
                buttonEdit.Enabled = false;
                buttonRemove.Enabled = false;
                buttonSetAsDefault.Enabled = false;
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

        private void ButtonNew_Click(object? sender, EventArgs e) {
            XmlElement storeElement = storesElement.OwnerDocument.CreateElement("Store");

            if (new LocomotiveCollectionStore(storesName, storeElement, collectionDescription, defaultDirectory, defaultExtension).ShowDialog(this) == DialogResult.OK) {
                storesElement.AppendChild(storeElement);
                listViewStores.Items.Add(new StoreItem(storeElement));
                UpdateButtons();
            }
        }

        private void ButtonEdit_Click(object? sender, EventArgs e) {
            if (listViewStores.SelectedItems.Count > 0) {
                StoreItem selected = (StoreItem)listViewStores.SelectedItems[0];

                if (new LocomotiveCollectionStore(storesName, selected.StoreElement, collectionDescription, defaultDirectory, defaultExtension).ShowDialog(this) == DialogResult.OK)
                    selected.Update();
            }
        }

        private void ListViewStores_SelectedIndexChanged(object? sender, EventArgs e) {
            UpdateButtons();
        }

        private void ButtonRemove_Click(object? sender, EventArgs e) {
            if (listViewStores.SelectedItems.Count > 0) {
                StoreItem selected = (StoreItem)listViewStores.SelectedItems[0];

                selected.StoreElement.ParentNode?.RemoveChild(selected.StoreElement);
                listViewStores.Items.Remove(selected);
                UpdateButtons();
            }
        }

        private void ButtonSetAsDefault_Click(object? sender, EventArgs e) {
            if (listViewStores.SelectedItems.Count > 0) {
                StoreItem selected = (StoreItem)listViewStores.SelectedItems[0];

                foreach (StoreItem item in listViewStores.Items) {
                    if (item.IsDefault) {
                        item.IsDefault = false;
                        item.Update();
                        break;
                    }
                }

                selected.IsDefault = true;
                selected.Update();
            }
        }

        private void ButtonClose_Click(object? sender, EventArgs e) {
            bool defaultFound = false;
            int defaultStoreIndex = 0;

            foreach (StoreItem item in listViewStores.Items) {
                if (item.IsDefault) {
                    storesElement.SetAttributeValue(A_DefaultStore, defaultStoreIndex);
                    defaultFound = true;
                    break;
                }

                defaultStoreIndex++;
            }

            if (!defaultFound) {
                bool errorCondition = true;

                if (listViewStores.Items.Count == 0) {
                    MessageBox.Show(this, "You must specify at least one storage location",
                        "No storage location specified", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (listViewStores.Items.Count == 1) {
                    storesElement.SetAttributeValue(A_DefaultStore, 0);
                    errorCondition = false;
                }
                else {
                    MessageBox.Show(this, "No storage is assigned as default for new items. Use the 'Set Default' button to select a default storage",
                        "No default storage", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (errorCondition) {
                    listViewStores.Focus();
                    return;
                }
            }

            DialogResult = DialogResult.OK;
        }

        private class StoreItem : ListViewItem {
            public StoreItem(XmlElement storeElement) {
                this.StoreElement = storeElement;

                Text = storeElement.GetAttribute("Name");
                SubItems.Add(storeElement.GetAttribute("File"));
            }

            public void Update() {
                Text = StoreElement.GetAttribute("Name");
                SubItems[1].Text = StoreElement.GetAttribute("File");

                if (IsDefault)
                    Font = new Font(ListView.Font, ListView.Font.Style | FontStyle.Bold);
                else
                    Font = ListView.Font;
            }

            public XmlElement StoreElement { get; }

            public bool IsDefault { get; set; } = false;
        }
    }
}
