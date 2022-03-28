using LayoutManager.CommonUI;
using LayoutManager.CommonUI.Controls;
using LayoutManager.Model;
using System.Drawing;
using System.Windows.Forms;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveCatalog.
    /// </summary>
    public partial class LocomotiveCatalog : Form {
        private readonly LocomotiveCatalogInfo catalog;
        private readonly LocomotiveTypeList locomotiveTypeList;

        public LocomotiveCatalog() {
            catalog = LayoutModel.LocomotiveCatalog;

            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            catalog.Load();
            locomotiveTypeList = new(xmlQueryList);

            locomotiveTypeList.Initialize();
            xmlQueryList.ContainerElement = catalog.CollectionElement;
            xmlQueryList.DefaultSortField = "TypeName";
            xmlQueryList.CurrentListLayoutIndex = 0;
            xmlQueryList.AddLayoutMenuItems(new MenuOrMenuItem(menuItemArrange));
            xmlQueryList.ListBox.DoubleClick += LocomotiveTypeList_DoubleClick;
            xmlQueryList.ListBox.SelectedIndexChanged += LocomotiveTypeList_SelectedIndexChanged;
            UpdateButtons();
        }

        private void UpdateButtons() {
            if (xmlQueryList.SelectedXmlItem == null) {
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

                    if (xmlQueryList.IsSelectedExpanded())
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

        private void MenuItemStorage_Click(object? sender, System.EventArgs e) {
            if (catalog.IsLoaded)
                catalog.Unload();

            new LocomotiveCollectionStores("Locomotive Catalog", catalog.Element["Stores"]!, "Locomotive Catalog", catalog.DefaultStoreDirectory, ".LocomotiveCatalog").ShowDialog(this);

            catalog.Load();
            xmlQueryList.ContainerElement = catalog.CollectionElement;
        }

        private void ButtonClose_Click(object? sender, System.EventArgs e) {
            if (catalog.IsLoaded)
                catalog.Unload();

            LayoutModel.WriteModelXmlInfo();
        }

        private void ButtonAdd_Click(object? sender, System.EventArgs e) {
            LocomotiveTypeInfo locomotiveType = new(catalog.CreateCollectionElement());

            if (new LocomotiveTypeProperties(locomotiveType).ShowDialog(this) == DialogResult.OK)
                catalog.CollectionElement.AppendChild(locomotiveType.Element);
        }

        private void ButtonEdit_Click(object? sender, System.EventArgs e) {
            if (locomotiveTypeList.SelectedLocomotiveType != null)
                new LocomotiveTypeProperties(locomotiveTypeList.SelectedLocomotiveType).ShowDialog(this);
            else
                xmlQueryList.ToggleSelectedExpansion();

            UpdateButtons();
        }

        private void LocomotiveTypeList_DoubleClick(object? sender, System.EventArgs e) {
            if (locomotiveTypeList.SelectedLocomotiveType != null)
                new LocomotiveTypeProperties(locomotiveTypeList.SelectedLocomotiveType).ShowDialog(this);

            UpdateButtons();
        }

        private void ButtonRemove_Click(object? sender, System.EventArgs e) {
            if (locomotiveTypeList.SelectedLocomotiveType != null) {
                LocomotiveTypeInfo s = locomotiveTypeList.SelectedLocomotiveType;

                s.Element.ParentNode?.RemoveChild(s.Element);
            }

            UpdateButtons();
        }

        private void LocomotiveTypeList_SelectedIndexChanged(object? sender, System.EventArgs e) {
            UpdateButtons();
        }

        private void MenuItemStandardImages_Click(object? sender, System.EventArgs e) {
            new LocomotiveCatalogStandardImages(catalog).ShowDialog(this);
        }

        private void ButtonOptions_Click(object? sender, System.EventArgs e) {
            contextMenuOptions.Show(buttonOptions, new Point(buttonOptions.Left, buttonOptions.Bottom));
        }

        private void MenuItemStandardLocomotiveFunctions_Click(object? sender, System.EventArgs e) {
            new StandardFunctions().ShowDialog(this);
        }
    }
}
