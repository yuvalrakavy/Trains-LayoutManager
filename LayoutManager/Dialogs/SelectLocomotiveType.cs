using LayoutManager.CommonUI;
using LayoutManager.CommonUI.Controls;
using LayoutManager.Model;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for SelectLocomotiveType.
    /// </summary>
    public partial class SelectLocomotiveType : Form {
        private readonly LocomotiveCatalogInfo catalog;
        readonly LocomotiveTypeList locomotiveTypeList;

        public SelectLocomotiveType() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            catalog = LayoutModel.LocomotiveCatalog;
            catalog.Load();

            locomotiveTypeList = new(xmlQueryList);
            locomotiveTypeList.Initialize();

            xmlQueryList.ContainerElement = catalog.CollectionElement;
            xmlQueryList.CurrentListLayoutIndex = 0;
            xmlQueryList.ListBox.SelectedIndexChanged += new EventHandler(this.LocomotiveTypeList_SelectedIndexChanged);

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

            xmlQueryList.AddLayoutMenuItems(new MenuOrMenuItem(m));
            m.Show(this, new Point(buttonArrangeBy.Left, buttonArrangeBy.Bottom));
        }
    }
}
