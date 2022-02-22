using LayoutManager.Model;
using System.Windows.Forms;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for ArrangeAreas.
    /// </summary>
    public partial class ArrangeAreas : Form {
        private bool rebuildTabs = false;

        public ArrangeAreas(TabControl tabAreas) {
            this.tabAreas = tabAreas;

            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            foreach (LayoutFrameWindowAreaTabPage areaPage in tabAreas.TabPages)
                listBoxAreas.Items.Add(areaPage);

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
            int selectedAreaIndex = listBoxAreas.SelectedIndex;

            if (selectedAreaIndex == -1) {
                buttonDelete.Enabled = false;
                buttonRename.Enabled = false;
                buttonMoveUp.Enabled = false;
                buttonMoveDown.Enabled = false;
            }
            else {
                buttonDelete.Enabled = LayoutController.IsDesignMode;
                buttonRename.Enabled = LayoutController.IsDesignMode;

                buttonMoveUp.Enabled = selectedAreaIndex > 0;
                buttonMoveDown.Enabled = selectedAreaIndex < listBoxAreas.Items.Count - 1;
            }
        }

        private void ButtonMoveDown_Click(object? sender, System.EventArgs e) {
            int selectedAreaIndex = listBoxAreas.SelectedIndex;

            if (selectedAreaIndex != -1 && selectedAreaIndex < listBoxAreas.Items.Count - 1) {
                LayoutFrameWindowAreaTabPage movedArea = (LayoutFrameWindowAreaTabPage)listBoxAreas.SelectedItem;

                listBoxAreas.Items.Remove(movedArea);
                listBoxAreas.Items.Insert(selectedAreaIndex + 1, movedArea);
                listBoxAreas.SelectedItem = movedArea;
                rebuildTabs = true;
            }
        }

        private void ButtonMoveUp_Click(object? sender, System.EventArgs e) {
            int selectedAreaIndex = listBoxAreas.SelectedIndex;

            if (selectedAreaIndex != -1 && selectedAreaIndex > 0) {
                LayoutFrameWindowAreaTabPage movedArea = (LayoutFrameWindowAreaTabPage)listBoxAreas.SelectedItem;

                listBoxAreas.Items.Remove(movedArea);
                listBoxAreas.Items.Insert(selectedAreaIndex - 1, movedArea);
                listBoxAreas.SelectedItem = movedArea;
                rebuildTabs = true;
            }
        }

        private void ButtonClose_Click(object? sender, System.EventArgs e) {
            if (rebuildTabs) {
                tabAreas.TabPages.Clear();

                foreach (LayoutFrameWindowAreaTabPage areaPage in listBoxAreas.Items)
                    tabAreas.TabPages.Add(areaPage);
            }

            this.Close();
        }

        private void ListBoxAreas_SelectedIndexChanged(object? sender, System.EventArgs e) {
            UpdateButtons();
        }

        private void ButtonRename_Click(object? sender, System.EventArgs e) {
            int selectedIndex = listBoxAreas.SelectedIndex;

            if (selectedIndex != -1) {
                LayoutFrameWindowAreaTabPage selectedArea = (LayoutFrameWindowAreaTabPage)listBoxAreas.SelectedItem;

                GetAreasName getAreaName = new();

                if (getAreaName.ShowDialog(this) == DialogResult.OK) {
                    selectedArea.Area.Name = getAreaName.AreaName;

                    listBoxAreas.Items[selectedIndex] = selectedArea;
                }
            }
        }

        private void ButtonNew_Click(object? sender, System.EventArgs e) {
            GetAreasName getAreaName = new();

            if (getAreaName.ShowDialog(this) == DialogResult.OK) {
                LayoutModelArea area = LayoutController.AddArea(getAreaName.AreaName);

                foreach (LayoutFrameWindowAreaTabPage areaPage in tabAreas.TabPages) {
                    if (areaPage.Area == area) {
                        listBoxAreas.Items.Insert(tabAreas.TabPages.IndexOf(areaPage), areaPage);
                        listBoxAreas.SelectedItem = areaPage;
                        UpdateButtons();
                        break;
                    }
                }
            }
        }

        private void ButtonDelete_Click(object? sender, System.EventArgs e) {
            LayoutFrameWindowAreaTabPage selectedAreaPage = (LayoutFrameWindowAreaTabPage)listBoxAreas.SelectedItem;
            LayoutModelArea selectedArea = selectedAreaPage.Area;

            // Check if area is empty
            if (selectedArea.Grid.Count > 0)
                MessageBox.Show(this, "This area is not empty, first remove all components, then delete the area", "Cannot delete area", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else {
                LayoutModel.Areas.Remove(selectedArea);
                listBoxAreas.Items.Remove(selectedAreaPage);
                UpdateButtons();
            }
        }
    }
}
