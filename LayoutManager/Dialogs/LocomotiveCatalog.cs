using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveCatalog.
    /// </summary>
    public class LocomotiveCatalog : Form {
        private Button buttonClose;
        private Button buttonAdd;
        private Button buttonEdit;
        private Button buttonRemove;
        private LayoutManager.CommonUI.Controls.LocomotiveTypeList locomotiveTypeList;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private ContextMenu contextMenuOptions;
        private MenuItem menuItemArrange;
        private MenuItem menuItemStorage;
        private MenuItem menuItemStandardImages;
        private Button buttonOptions;
        private MenuItem menuItemStandardLocomotiveFunctions;
        private readonly LocomotiveCatalogInfo catalog;

        public LocomotiveCatalog() {
            catalog = LayoutModel.LocomotiveCatalog;

            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            catalog.Load();
            locomotiveTypeList.Initialize();
            locomotiveTypeList.ContainerElement = catalog.CollectionElement;
            locomotiveTypeList.DefaultSortField = "TypeName";
            locomotiveTypeList.CurrentListLayoutIndex = 0;
            locomotiveTypeList.AddLayoutMenuItems(menuItemArrange);

            updateButtons();
        }

        private void updateButtons() {
            if (locomotiveTypeList.SelectedItem == null) {
                buttonEdit.Text = "Edit...";

                buttonEdit.Visible = true;
                buttonRemove.Visible = true;

                buttonEdit.Enabled = false;
                buttonRemove.Enabled = false;
            }
            else {
                if (locomotiveTypeList.SelectedLocomotiveType != null) {
                    buttonEdit.Text = "Edit...";

                    buttonRemove.Visible = true;
                    buttonEdit.Enabled = true;
                    buttonRemove.Enabled = true;
                }
                else {
                    buttonEdit.Enabled = true;
                    buttonRemove.Visible = false;

                    if (locomotiveTypeList.IsSelectedExpanded())
                        buttonEdit.Text = "Collapse";
                    else
                        buttonEdit.Text = "Expand";
                }
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
            this.buttonAdd = new Button();
            this.buttonEdit = new Button();
            this.buttonRemove = new Button();
            this.locomotiveTypeList = new LayoutManager.CommonUI.Controls.LocomotiveTypeList();
            this.contextMenuOptions = new ContextMenu();
            this.menuItemArrange = new MenuItem();
            this.menuItemStorage = new MenuItem();
            this.menuItemStandardImages = new MenuItem();
            this.buttonOptions = new Button();
            this.menuItemStandardLocomotiveFunctions = new MenuItem();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new System.Drawing.Point(392, 464);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.TabIndex = 6;
            this.buttonClose.Text = "&Close";
            this.buttonClose.Click += this.buttonClose_Click;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonAdd.Location = new System.Drawing.Point(8, 424);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "&Add...";
            this.buttonAdd.Click += this.buttonAdd_Click;
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonEdit.Location = new System.Drawing.Point(88, 424);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.TabIndex = 2;
            this.buttonEdit.Text = "&Edit...";
            this.buttonEdit.Click += this.buttonEdit_Click;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonRemove.Location = new System.Drawing.Point(168, 424);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.TabIndex = 3;
            this.buttonRemove.Text = "&Remove";
            this.buttonRemove.Click += this.buttonRemove_Click;
            // 
            // locomotiveTypeList
            // 
            this.locomotiveTypeList.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.locomotiveTypeList.ContainerElement = null;
            this.locomotiveTypeList.CurrentListLayout = null;
            this.locomotiveTypeList.CurrentListLayoutIndex = -1;
            this.locomotiveTypeList.DefaultSortField = null;
            this.locomotiveTypeList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.locomotiveTypeList.Location = new System.Drawing.Point(8, 8);
            this.locomotiveTypeList.Name = "locomotiveTypeList";
            this.locomotiveTypeList.Size = new System.Drawing.Size(464, 408);
            this.locomotiveTypeList.TabIndex = 0;
            this.locomotiveTypeList.DoubleClick += this.locomotiveTypeList_DoubleClick;
            this.locomotiveTypeList.SelectedIndexChanged += this.locomotiveTypeList_SelectedIndexChanged;
            // 
            // contextMenuOptions
            // 
            this.contextMenuOptions.MenuItems.AddRange(new MenuItem[] {
                                                                                               this.menuItemArrange,
                                                                                               this.menuItemStorage,
                                                                                               this.menuItemStandardImages,
                                                                                               this.menuItemStandardLocomotiveFunctions});
            // 
            // menuItemArrange
            // 
            this.menuItemArrange.Index = 0;
            this.menuItemArrange.Text = "Arrange by";
            // 
            // menuItemStorage
            // 
            this.menuItemStorage.Index = 1;
            this.menuItemStorage.Text = "Storage...";
            this.menuItemStorage.Click += this.menuItemStorage_Click;
            // 
            // menuItemStandardImages
            // 
            this.menuItemStandardImages.Index = 2;
            this.menuItemStandardImages.Text = "Standard images...";
            this.menuItemStandardImages.Click += this.menuItemStandardImages_Click;
            // 
            // buttonOptions
            // 
            this.buttonOptions.Location = new System.Drawing.Point(8, 464);
            this.buttonOptions.Name = "buttonOptions";
            this.buttonOptions.TabIndex = 7;
            this.buttonOptions.Text = "&Options";
            this.buttonOptions.Click += this.buttonOptions_Click;
            // 
            // menuItemStandardLocomotiveFunctions
            // 
            this.menuItemStandardLocomotiveFunctions.Index = 3;
            this.menuItemStandardLocomotiveFunctions.Text = "Standard Locomotive Functions...";
            this.menuItemStandardLocomotiveFunctions.Click += this.menuItemStandardLocomotiveFunctions_Click;
            // 
            // LocomotiveCatalog
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.ClientSize = new System.Drawing.Size(480, 494);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonOptions,
                                                                          this.locomotiveTypeList,
                                                                          this.buttonRemove,
                                                                          this.buttonEdit,
                                                                          this.buttonAdd,
                                                                          this.buttonClose});
            this.Name = "LocomotiveCatalog";
            this.ShowInTaskbar = false;
            this.Text = "Locomotive Catalog";
            this.ResumeLayout(false);
        }
        #endregion

        private void menuItemStorage_Click(object sender, System.EventArgs e) {
            if (catalog.IsLoaded)
                catalog.Unload();

            new Dialogs.LocomotiveCollectionStores("Locomotive Catalog", catalog.Element["Stores"], "Locomotive Catalog", catalog.DefaultStoreDirectory, ".LocomotiveCatalog").ShowDialog(this);

            catalog.Load();
            locomotiveTypeList.ContainerElement = catalog.CollectionElement;
        }

        private void buttonClose_Click(object sender, System.EventArgs e) {
            if (catalog.IsLoaded)
                catalog.Unload();

            LayoutModel.WriteModelXmlInfo();
        }

        private void buttonAdd_Click(object sender, System.EventArgs e) {
            LocomotiveTypeInfo locomotiveType = new LocomotiveTypeInfo(catalog.CreateCollectionElement());

            if (new Dialogs.LocomotiveTypeProperties(locomotiveType).ShowDialog(this) == DialogResult.OK)
                catalog.CollectionElement.AppendChild(locomotiveType.Element);
        }

        private void buttonEdit_Click(object sender, System.EventArgs e) {
            if (locomotiveTypeList.SelectedLocomotiveType != null)
                new Dialogs.LocomotiveTypeProperties(locomotiveTypeList.SelectedLocomotiveType).ShowDialog(this);
            else
                locomotiveTypeList.ToggleSelectedExpansion();

            updateButtons();
        }

        private void locomotiveTypeList_DoubleClick(object sender, System.EventArgs e) {
            if (locomotiveTypeList.SelectedLocomotiveType != null)
                new Dialogs.LocomotiveTypeProperties(locomotiveTypeList.SelectedLocomotiveType).ShowDialog(this);

            updateButtons();
        }

        private void buttonRemove_Click(object sender, System.EventArgs e) {
            if (locomotiveTypeList.SelectedLocomotiveType != null) {
                LocomotiveTypeInfo s = locomotiveTypeList.SelectedLocomotiveType;

                s.Element.ParentNode.RemoveChild(s.Element);
            }

            updateButtons();
        }

        private void locomotiveTypeList_SelectedIndexChanged(object sender, System.EventArgs e) {
            updateButtons();
        }

        private void menuItemStandardImages_Click(object sender, System.EventArgs e) {
            new Dialogs.LocomotiveCatalogStandardImages(catalog).ShowDialog(this);
        }

        private void buttonOptions_Click(object sender, System.EventArgs e) {
            contextMenuOptions.Show(this, new Point(buttonOptions.Left, buttonOptions.Bottom));
        }

        private void menuItemStandardLocomotiveFunctions_Click(object sender, System.EventArgs e) {
            new Dialogs.StandardFunctions().ShowDialog(this);
        }
    }
}
