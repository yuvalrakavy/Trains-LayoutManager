using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveCollectionStores.
    /// </summary>
    public class LocomotiveCollectionStores : Form {
        private Button buttonClose;
        private ListView listViewStores;
        private Button buttonNew;
        private Button buttonEdit;
        private Button buttonRemove;
        private ColumnHeader columnHeaderName;
        private ColumnHeader columnHeaderFile;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
        private Button buttonSetAsDefault;
        readonly XmlElement storesElement;
        readonly String storesName;
        readonly string collectionDescription;
        readonly string defaultDirectory;
        readonly string defaultExtension;

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

            if (storesElement.HasAttribute("DefaultStore")) {
                StoreItem defaultItem = (StoreItem)listViewStores.Items[XmlConvert.ToInt32(storesElement.GetAttribute("DefaultStore"))];

                defaultItem.IsDefault = true;
                defaultItem.Update();
            }

            updateButtons();
        }

        private void updateButtons() {
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.buttonClose = new Button();
            this.listViewStores = new ListView();
            this.columnHeaderName = new ColumnHeader();
            this.columnHeaderFile = new ColumnHeader();
            this.buttonNew = new Button();
            this.buttonEdit = new Button();
            this.buttonRemove = new Button();
            this.buttonSetAsDefault = new Button();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonClose.Location = new System.Drawing.Point(496, 232);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.TabIndex = 5;
            this.buttonClose.Text = "&Close";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // listViewStores
            // 
            this.listViewStores.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.listViewStores.Columns.AddRange(new ColumnHeader[] {
                                                                                             this.columnHeaderName,
                                                                                             this.columnHeaderFile});
            this.listViewStores.FullRowSelect = true;
            this.listViewStores.GridLines = true;
            this.listViewStores.HideSelection = false;
            this.listViewStores.Location = new System.Drawing.Point(8, 8);
            this.listViewStores.Name = "listViewStores";
            this.listViewStores.Size = new System.Drawing.Size(560, 192);
            this.listViewStores.TabIndex = 0;
            this.listViewStores.View = System.Windows.Forms.View.Details;
            this.listViewStores.SelectedIndexChanged += new System.EventHandler(this.listViewStores_SelectedIndexChanged);
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 121;
            // 
            // columnHeaderFile
            // 
            this.columnHeaderFile.Text = "Stored in file";
            this.columnHeaderFile.Width = 430;
            // 
            // buttonNew
            // 
            this.buttonNew.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonNew.Location = new System.Drawing.Point(8, 208);
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new System.Drawing.Size(88, 23);
            this.buttonNew.TabIndex = 1;
            this.buttonNew.Text = "&New...";
            this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonEdit.Location = new System.Drawing.Point(104, 208);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(88, 23);
            this.buttonEdit.TabIndex = 2;
            this.buttonEdit.Text = "&Edit...";
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonRemove.Location = new System.Drawing.Point(200, 208);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(88, 23);
            this.buttonRemove.TabIndex = 3;
            this.buttonRemove.Text = "&Delete";
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonSetAsDefault
            // 
            this.buttonSetAsDefault.Location = new System.Drawing.Point(296, 208);
            this.buttonSetAsDefault.Name = "buttonSetAsDefault";
            this.buttonSetAsDefault.Size = new System.Drawing.Size(88, 23);
            this.buttonSetAsDefault.TabIndex = 4;
            this.buttonSetAsDefault.Text = "Set as default";
            this.buttonSetAsDefault.Click += new System.EventHandler(this.buttonSetAsDefault_Click);
            // 
            // LocomotiveCollectionStores
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.ClientSize = new System.Drawing.Size(576, 262);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonSetAsDefault,
                                                                          this.buttonNew,
                                                                          this.listViewStores,
                                                                          this.buttonClose,
                                                                          this.buttonEdit,
                                                                          this.buttonRemove});
            this.Name = "LocomotiveCollectionStores";
            this.ShowInTaskbar = false;
            this.Text = "Locomotive Catalog Storage";
            this.ResumeLayout(false);

        }
        #endregion

        private void buttonNew_Click(object sender, System.EventArgs e) {
            XmlElement storeElement = storesElement.OwnerDocument.CreateElement("Store");

            if (new Dialogs.LocomotiveCollectionStore(storesName, storeElement, collectionDescription, defaultDirectory, defaultExtension).ShowDialog(this) == DialogResult.OK) {
                storesElement.AppendChild(storeElement);
                listViewStores.Items.Add(new StoreItem(storeElement));
                updateButtons();
            }
        }

        private void buttonEdit_Click(object sender, System.EventArgs e) {
            if (listViewStores.SelectedItems.Count > 0) {
                StoreItem selected = (StoreItem)listViewStores.SelectedItems[0];

                if (new Dialogs.LocomotiveCollectionStore(storesName, selected.StoreElement, collectionDescription, defaultDirectory, defaultExtension).ShowDialog(this) == DialogResult.OK)
                    selected.Update();
            }
        }

        private void listViewStores_SelectedIndexChanged(object sender, System.EventArgs e) {
            updateButtons();
        }

        private void buttonRemove_Click(object sender, System.EventArgs e) {
            if (listViewStores.SelectedItems.Count > 0) {
                StoreItem selected = (StoreItem)listViewStores.SelectedItems[0];

                selected.StoreElement.ParentNode.RemoveChild(selected.StoreElement);
                listViewStores.Items.Remove(selected);
                updateButtons();
            }
        }

        private void buttonSetAsDefault_Click(object sender, System.EventArgs e) {
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

        private void buttonClose_Click(object sender, System.EventArgs e) {
            bool defaultFound = false;
            int defaultStoreIndex = 0;

            foreach (StoreItem item in listViewStores.Items) {
                if (item.IsDefault) {
                    storesElement.SetAttribute("DefaultStore", XmlConvert.ToString(defaultStoreIndex));
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
                    storesElement.SetAttribute("DefaultStore", XmlConvert.ToString(0));
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

        class StoreItem : ListViewItem {
            readonly XmlElement storeElement;
            bool isDefault = false;

            public StoreItem(XmlElement storeElement) {
                this.storeElement = storeElement;

                Text = storeElement.GetAttribute("Name");
                SubItems.Add(storeElement.GetAttribute("File"));
            }

            public void Update() {
                Text = storeElement.GetAttribute("Name");
                SubItems[1].Text = storeElement.GetAttribute("File");

                if (isDefault)
                    Font = new Font(ListView.Font, ListView.Font.Style | FontStyle.Bold);
                else
                    Font = ListView.Font;
            }

            public XmlElement StoreElement => storeElement;

            public bool IsDefault {
                get {
                    return isDefault;
                }

                set {
                    isDefault = value;
                }
            }

        }
    }
}
