using LayoutManager.CommonUI;
using LayoutManager.Model;
using System.Drawing;
using System.Windows.Forms;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for SelectLocomotiveType.
    /// </summary>
    public partial class SelectLocomotiveType : Form {
        private readonly LocomotiveCatalogInfo catalog;

        public SelectLocomotiveType() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            catalog = LayoutModel.LocomotiveCatalog;
            catalog.Load();

            locomotiveTypeList.Initialize();
            locomotiveTypeList.ContainerElement = catalog.CollectionElement;
            locomotiveTypeList.CurrentListLayoutIndex = 0;

            UpdateButtons();
        }

        private void UpdateButtons() {
            buttonSelect.Enabled = locomotiveTypeList.SelectedLocomotiveType != null;
        }

        public LocomotiveTypeInfo? SelectedLocomotiveType => locomotiveTypeList.SelectedLocomotiveType;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }

                if (catalog != null && catalog.IsLoaded)
                    catalog.Unload();
            }
            base.Dispose(disposing);
        }

        private void LocomotiveTypeList_SelectedIndexChanged(object? sender, System.EventArgs e) {
            UpdateButtons();
        }

        private void ButtonCancel_Click(object? sender, System.EventArgs e) {
        }

        private void ButtonSelect_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.OK;
        }

        private void ButtonArrangeBy_Click(object? sender, System.EventArgs e) {
            var m = new ContextMenuStrip();

            locomotiveTypeList.AddLayoutMenuItems(new MenuOrMenuItem(m));
            m.Show(this, new Point(buttonArrangeBy.Left, buttonArrangeBy.Bottom));
        }
    }
}
