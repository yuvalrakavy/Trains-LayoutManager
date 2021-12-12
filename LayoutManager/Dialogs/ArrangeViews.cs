using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.View;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for ArrangeAreas.
    /// </summary>
    public partial class ArrangeViews : Form {
        private readonly LayoutFrameWindowAreaTabPage areaPage;
        private bool rebuildTabs = false;
        private readonly FrameWindow controller;

        public ArrangeViews(FrameWindow controller, LayoutFrameWindowAreaTabPage areaPage) {
            this.controller = controller;
            this.areaPage = areaPage;

            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            foreach (LayoutFrameWindowAreaViewTabPage viewPage in areaPage.TabViews.TabPages)
                listBoxViews.Items.Add(viewPage);

            UpdateButtons();
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

        private void UpdateButtons() {
            int selectedViewIndex = listBoxViews.SelectedIndex;

            if (selectedViewIndex == -1) {
                buttonDelete.Enabled = false;
                buttonRename.Enabled = false;
                buttonMoveUp.Enabled = false;
                buttonMoveDown.Enabled = false;
            }
            else {
                buttonDelete.Enabled = true;
                buttonRename.Enabled = true;

                buttonMoveUp.Enabled = selectedViewIndex > 0;
                buttonMoveDown.Enabled = selectedViewIndex < listBoxViews.Items.Count - 1;
            }
        }

        private void ButtonMoveDown_Click(object? sender, System.EventArgs e) {
            int selectedViewIndex = listBoxViews.SelectedIndex;

            if (selectedViewIndex != -1 && selectedViewIndex < listBoxViews.Items.Count - 1) {
                LayoutFrameWindowAreaViewTabPage movedView = (LayoutFrameWindowAreaViewTabPage)listBoxViews.SelectedItem;

                listBoxViews.Items.Remove(movedView);
                listBoxViews.Items.Insert(selectedViewIndex + 1, movedView);
                listBoxViews.SelectedItem = movedView;
                rebuildTabs = true;
            }
        }

        private void ButtonMoveUp_Click(object? sender, System.EventArgs e) {
            int selectedViewIndex = listBoxViews.SelectedIndex;

            if (selectedViewIndex != -1 && selectedViewIndex > 0) {
                LayoutFrameWindowAreaViewTabPage movedView = (LayoutFrameWindowAreaViewTabPage)listBoxViews.SelectedItem;

                listBoxViews.Items.Remove(movedView);
                listBoxViews.Items.Insert(selectedViewIndex - 1, movedView);
                listBoxViews.SelectedItem = movedView;
                rebuildTabs = true;
            }
        }

        private void ButtonClose_Click(object? sender, System.EventArgs e) {
            if (rebuildTabs) {
                areaPage.TabViews.TabPages.Clear();

                foreach (LayoutFrameWindowAreaViewTabPage viewPage in listBoxViews.Items)
                    areaPage.TabViews.TabPages.Add(viewPage);
            }

            this.Close();
        }

        private void ListBoxViews_SelectedIndexChanged(object? sender, System.EventArgs e) {
            UpdateButtons();
        }

        private void ButtonRename_Click(object? sender, System.EventArgs e) {
            int selectedIndex = listBoxViews.SelectedIndex;

            if (selectedIndex != -1) {
                LayoutFrameWindowAreaViewTabPage selectedView = (LayoutFrameWindowAreaViewTabPage)listBoxViews.SelectedItem;

                GetViewName getViewName = new();

                if (getViewName.ShowDialog(this) == DialogResult.OK) {
                    selectedView.Text = getViewName.ViewName;

                    listBoxViews.Items[selectedIndex] = selectedView;
                }
            }
        }

        private void ButtonNew_Click(object? sender, System.EventArgs e) {
            var newView = controller.AddNewView();

            foreach (LayoutFrameWindowAreaViewTabPage viewPage in areaPage.TabViews.TabPages) {
                if (viewPage.View == newView) {
                    listBoxViews.Items.Add(viewPage);
                    break;
                }
            }
        }

        private void ButtonDelete_Click(object? sender, System.EventArgs e) {
            LayoutFrameWindowAreaViewTabPage selectedViewPage = (LayoutFrameWindowAreaViewTabPage)listBoxViews.SelectedItem;

            if (selectedViewPage != null) {
                areaPage.TabViews.TabPages.Remove(selectedViewPage);
                listBoxViews.Items.Remove(selectedViewPage);
                UpdateButtons();
            }
        }
    }
}
