using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for ArrangeAreas.
    /// </summary>
    public class ArrangeAreas : Form {
        private Button buttonMoveDown;
        private ImageList imageListButtons;
        private Button buttonMoveUp;
        private ListBox listBoxAreas;
        private Button buttonClose;
        private Button buttonNew;
        private Button buttonDelete;
        private Button buttonRename;
        private IContainer components;
        private readonly TabControl tabAreas;
        private bool rebuildTabs = false;

        public ArrangeAreas(TabControl tabAreas) {
            this.tabAreas = tabAreas;

            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            foreach (LayoutFrameWindowAreaTabPage areaPage in tabAreas.TabPages)
                listBoxAreas.Items.Add(areaPage);

            updateButtons();
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

        private void updateButtons() {
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new Container();
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ArrangeAreas));
            this.listBoxAreas = new ListBox();
            this.buttonMoveDown = new Button();
            this.imageListButtons = new ImageList(this.components);
            this.buttonMoveUp = new Button();
            this.buttonClose = new Button();
            this.buttonNew = new Button();
            this.buttonDelete = new Button();
            this.buttonRename = new Button();
            this.SuspendLayout();
            // 
            // listBoxAreas
            // 
            this.listBoxAreas.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.listBoxAreas.DisplayMember = "Text";
            this.listBoxAreas.Location = new System.Drawing.Point(8, 16);
            this.listBoxAreas.Name = "listBoxAreas";
            this.listBoxAreas.Size = new System.Drawing.Size(216, 212);
            this.listBoxAreas.TabIndex = 0;
            this.listBoxAreas.SelectedIndexChanged += this.listBoxAreas_SelectedIndexChanged;
            // 
            // buttonMoveDown
            // 
            this.buttonMoveDown.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.buttonMoveDown.Image = ((System.Drawing.Bitmap)(resources.GetObject("buttonMoveDown.Image")));
            this.buttonMoveDown.ImageIndex = 1;
            this.buttonMoveDown.ImageList = this.imageListButtons;
            this.buttonMoveDown.Location = new System.Drawing.Point(230, 48);
            this.buttonMoveDown.Name = "buttonMoveDown";
            this.buttonMoveDown.Size = new System.Drawing.Size(32, 23);
            this.buttonMoveDown.TabIndex = 2;
            this.buttonMoveDown.Click += this.buttonMoveDown_Click;
            // 
            // imageListButtons
            // 
            this.imageListButtons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListButtons.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListButtons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListButtons.ImageStream")));
            this.imageListButtons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // buttonMoveUp
            // 
            this.buttonMoveUp.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.buttonMoveUp.Image = ((System.Drawing.Bitmap)(resources.GetObject("buttonMoveUp.Image")));
            this.buttonMoveUp.ImageIndex = 0;
            this.buttonMoveUp.ImageList = this.imageListButtons;
            this.buttonMoveUp.Location = new System.Drawing.Point(230, 16);
            this.buttonMoveUp.Name = "buttonMoveUp";
            this.buttonMoveUp.Size = new System.Drawing.Size(32, 23);
            this.buttonMoveUp.TabIndex = 1;
            this.buttonMoveUp.Click += this.buttonMoveUp_Click;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonClose.Location = new System.Drawing.Point(152, 272);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(72, 23);
            this.buttonClose.TabIndex = 6;
            this.buttonClose.Text = "Close";
            this.buttonClose.Click += this.buttonClose_Click;
            // 
            // buttonNew
            // 
            this.buttonNew.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonNew.Location = new System.Drawing.Point(8, 240);
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new System.Drawing.Size(72, 24);
            this.buttonNew.TabIndex = 3;
            this.buttonNew.Text = "New...";
            this.buttonNew.Click += this.buttonNew_Click;
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonDelete.Location = new System.Drawing.Point(80, 240);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(72, 24);
            this.buttonDelete.TabIndex = 4;
            this.buttonDelete.Text = "&Delete";
            this.buttonDelete.Click += this.buttonDelete_Click;
            // 
            // buttonRename
            // 
            this.buttonRename.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonRename.Location = new System.Drawing.Point(152, 240);
            this.buttonRename.Name = "buttonRename";
            this.buttonRename.Size = new System.Drawing.Size(72, 24);
            this.buttonRename.TabIndex = 5;
            this.buttonRename.Text = "Rename...";
            this.buttonRename.Click += this.buttonRename_Click;
            // 
            // ArrangeAreas
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.ClientSize = new System.Drawing.Size(264, 302);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonRename,
                                                                          this.buttonDelete,
                                                                          this.buttonNew,
                                                                          this.buttonClose,
                                                                          this.buttonMoveUp,
                                                                          this.buttonMoveDown,
                                                                          this.listBoxAreas});
            this.Name = "ArrangeAreas";
            this.ShowInTaskbar = false;
            this.Text = "Arrange Areas";
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonMoveDown_Click(object sender, System.EventArgs e) {
            int selectedAreaIndex = listBoxAreas.SelectedIndex;

            if (selectedAreaIndex != -1 && selectedAreaIndex < listBoxAreas.Items.Count - 1) {
                LayoutFrameWindowAreaTabPage movedArea = (LayoutFrameWindowAreaTabPage)listBoxAreas.SelectedItem;

                listBoxAreas.Items.Remove(movedArea);
                listBoxAreas.Items.Insert(selectedAreaIndex + 1, movedArea);
                listBoxAreas.SelectedItem = movedArea;
                rebuildTabs = true;
            }
        }

        private void buttonMoveUp_Click(object sender, System.EventArgs e) {
            int selectedAreaIndex = listBoxAreas.SelectedIndex;

            if (selectedAreaIndex != -1 && selectedAreaIndex > 0) {
                LayoutFrameWindowAreaTabPage movedArea = (LayoutFrameWindowAreaTabPage)listBoxAreas.SelectedItem;

                listBoxAreas.Items.Remove(movedArea);
                listBoxAreas.Items.Insert(selectedAreaIndex - 1, movedArea);
                listBoxAreas.SelectedItem = movedArea;
                rebuildTabs = true;
            }
        }

        private void buttonClose_Click(object sender, System.EventArgs e) {
            if (rebuildTabs) {
                tabAreas.TabPages.Clear();

                foreach (LayoutFrameWindowAreaTabPage areaPage in listBoxAreas.Items)
                    tabAreas.TabPages.Add(areaPage);
            }

            this.Close();
        }

        private void listBoxAreas_SelectedIndexChanged(object sender, System.EventArgs e) {
            updateButtons();
        }

        private void buttonRename_Click(object sender, System.EventArgs e) {
            int selectedIndex = listBoxAreas.SelectedIndex;

            if (selectedIndex != -1) {
                LayoutFrameWindowAreaTabPage selectedArea = (LayoutFrameWindowAreaTabPage)listBoxAreas.SelectedItem;

                Dialogs.GetAreasName getAreaName = new Dialogs.GetAreasName();

                if (getAreaName.ShowDialog(this) == DialogResult.OK) {
                    selectedArea.Area.Name = getAreaName.AreaName;

                    listBoxAreas.Items[selectedIndex] = selectedArea;
                }
            }
        }

        private void buttonNew_Click(object sender, System.EventArgs e) {
            Dialogs.GetAreasName getAreaName = new Dialogs.GetAreasName();

            if (getAreaName.ShowDialog(this) == DialogResult.OK) {
                LayoutModelArea area = LayoutController.AddArea(getAreaName.AreaName);

                foreach (LayoutFrameWindowAreaTabPage areaPage in tabAreas.TabPages) {
                    if (areaPage.Area == area) {
                        listBoxAreas.Items.Insert(tabAreas.TabPages.IndexOf(areaPage), areaPage);
                        listBoxAreas.SelectedItem = areaPage;
                        updateButtons();
                        break;
                    }
                }
            }
        }

        private void buttonDelete_Click(object sender, System.EventArgs e) {
            LayoutFrameWindowAreaTabPage selectedAreaPage = (LayoutFrameWindowAreaTabPage)listBoxAreas.SelectedItem;
            LayoutModelArea selectedArea = selectedAreaPage.Area;

            // Check if area is empty
            if (selectedArea.Grid.Count > 0)
                MessageBox.Show(this, "This area is not empty, first remove all components, then delete the area", "Cannot delete area", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else {
                LayoutModel.Areas.Remove(selectedArea);
                listBoxAreas.Items.Remove(selectedAreaPage);
                updateButtons();
            }
        }
    }
}
