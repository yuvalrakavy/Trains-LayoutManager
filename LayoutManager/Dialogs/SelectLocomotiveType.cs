using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for SelectLocomotiveType.
    /// </summary>
    public class SelectLocomotiveType : Form {
        private LayoutManager.CommonUI.Controls.LocomotiveTypeList locomotiveTypeList;
        private Button buttonSelect;
        private Button buttonCancel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private Button buttonArrangeBy;
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

            updateButtons();
        }

        private void updateButtons() {
            buttonSelect.Enabled = locomotiveTypeList.SelectedLocomotiveType != null;
        }

        public LocomotiveTypeInfo SelectedLocomotiveType => locomotiveTypeList.SelectedLocomotiveType;

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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.locomotiveTypeList = new LayoutManager.CommonUI.Controls.LocomotiveTypeList();
            this.buttonSelect = new Button();
            this.buttonCancel = new Button();
            this.buttonArrangeBy = new Button();
            this.SuspendLayout();
            // 
            // locomotiveTypeList
            // 
            this.locomotiveTypeList.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.locomotiveTypeList.ContainerElement = null;
            this.locomotiveTypeList.CurrentListLayout = null;
            this.locomotiveTypeList.CurrentListLayoutIndex = -1;
            this.locomotiveTypeList.DefaultSortField = "TypeName";
            this.locomotiveTypeList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.locomotiveTypeList.Location = new System.Drawing.Point(8, 8);
            this.locomotiveTypeList.Name = "locomotiveTypeList";
            this.locomotiveTypeList.Size = new System.Drawing.Size(376, 376);
            this.locomotiveTypeList.TabIndex = 0;
            this.locomotiveTypeList.SelectedIndexChanged += this.locomotiveTypeList_SelectedIndexChanged;
            // 
            // buttonSelect
            // 
            this.buttonSelect.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonSelect.Location = new System.Drawing.Point(252, 392);
            this.buttonSelect.Name = "buttonSelect";
            this.buttonSelect.Size = new System.Drawing.Size(64, 23);
            this.buttonSelect.TabIndex = 2;
            this.buttonSelect.Text = "&Select";
            this.buttonSelect.Click += this.buttonSelect_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(320, 392);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(64, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += this.buttonCancel_Click;
            // 
            // buttonArrangeBy
            // 
            this.buttonArrangeBy.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonArrangeBy.Location = new System.Drawing.Point(8, 392);
            this.buttonArrangeBy.Name = "buttonArrangeBy";
            this.buttonArrangeBy.TabIndex = 1;
            this.buttonArrangeBy.Text = "&Arrange by";
            this.buttonArrangeBy.Click += this.buttonArrangeBy_Click;
            // 
            // SelectLocomotiveType
            // 
            this.AcceptButton = this.buttonSelect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(392, 422);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonArrangeBy,
                                                                          this.buttonSelect,
                                                                          this.locomotiveTypeList,
                                                                          this.buttonCancel});
            this.Name = "SelectLocomotiveType";
            this.ShowInTaskbar = false;
            this.Text = "Select Locomotive Type";
            this.ResumeLayout(false);
        }
        #endregion

        private void locomotiveTypeList_SelectedIndexChanged(object sender, System.EventArgs e) {
            updateButtons();
        }

        private void buttonCancel_Click(object sender, System.EventArgs e) {
        }

        private void buttonSelect_Click(object sender, System.EventArgs e) {
            DialogResult = DialogResult.OK;
        }

        private void buttonArrangeBy_Click(object sender, System.EventArgs e) {
            ContextMenu m = new ContextMenu();

            locomotiveTypeList.AddLayoutMenuItems(m);
            m.Show(this, new Point(buttonArrangeBy.Left, buttonArrangeBy.Bottom));
        }
    }
}
