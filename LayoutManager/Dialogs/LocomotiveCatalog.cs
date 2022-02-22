using LayoutManager.CommonUI;
using LayoutManager.Model;
using System.Drawing;
using System.Windows.Forms;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveCatalog.
    /// </summary>
    public partial class LocomotiveCatalog : Form {
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
            locomotiveTypeList.AddLayoutMenuItems(new MenuOrMenuItem(menuItemArrange));

            UpdateButtons();
        }

        private void UpdateButtons() {
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

        private void MenuItemStorage_Click(object? sender, System.EventArgs e) {
            if (catalog.IsLoaded)
                catalog.Unload();

            new LocomotiveCollectionStores("Locomotive Catalog", catalog.Element["Stores"]!, "Locomotive Catalog", catalog.DefaultStoreDirectory, ".LocomotiveCatalog").ShowDialog(this);

            catalog.Load();
            locomotiveTypeList.ContainerElement = catalog.CollectionElement;
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
                locomotiveTypeList.ToggleSelectedExpansion();

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
            contextMenuOptions.Show(this, new Point(buttonOptions.Left, buttonOptions.Bottom));
        }

        private void MenuItemStandardLocomotiveFunctions_Click(object? sender, System.EventArgs e) {
            new StandardFunctions().ShowDialog(this);
        }
    }
}
