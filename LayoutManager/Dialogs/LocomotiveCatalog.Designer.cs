using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveCatalog.
    /// </summary>
    partial class LocomotiveCatalog : Form {
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
            this.locomotiveTypeList = new CommonUI.Controls.LocomotiveTypeList();
            this.contextMenuOptions = new ContextMenuStrip();
            this.menuItemArrange = new ToolStripMenuItem();
            this.menuItemStorage = new ToolStripMenuItem();
            this.menuItemStandardImages = new ToolStripMenuItem();
            this.buttonOptions = new Button();
            this.menuItemStandardLocomotiveFunctions = new ToolStripMenuItem();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new Point(392, 464);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.TabIndex = 6;
            this.buttonClose.Text = "&Close";
            this.buttonClose.Click += this.ButtonClose_Click;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonAdd.Location = new Point(8, 424);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "&Add...";
            this.buttonAdd.Click += this.ButtonAdd_Click;
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonEdit.Location = new Point(88, 424);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.TabIndex = 2;
            this.buttonEdit.Text = "&Edit...";
            this.buttonEdit.Click += this.ButtonEdit_Click;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonRemove.Location = new Point(168, 424);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.TabIndex = 3;
            this.buttonRemove.Text = "&Remove";
            this.buttonRemove.Click += this.ButtonRemove_Click;
            // 
            // locomotiveTypeList
            // 
            this.locomotiveTypeList.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.locomotiveTypeList.ContainerElement = null;
            this.locomotiveTypeList.CurrentListLayout = null;
            this.locomotiveTypeList.CurrentListLayoutIndex = -1;
            this.locomotiveTypeList.DefaultSortField = null;
            this.locomotiveTypeList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.locomotiveTypeList.Location = new Point(8, 8);
            this.locomotiveTypeList.Name = "locomotiveTypeList";
            this.locomotiveTypeList.Size = new Size(464, 408);
            this.locomotiveTypeList.TabIndex = 0;
            this.locomotiveTypeList.DoubleClick += this.LocomotiveTypeList_DoubleClick;
            this.locomotiveTypeList.SelectedIndexChanged += this.LocomotiveTypeList_SelectedIndexChanged;
            // 
            // contextMenuOptions
            // 
            this.contextMenuOptions.Items.AddRange(new ToolStripMenuItem[] {
                                                                                               this.menuItemArrange,
                                                                                               this.menuItemStorage,
                                                                                               this.menuItemStandardImages,
                                                                                               this.menuItemStandardLocomotiveFunctions});
            // 
            // menuItemArrange
            // 
            this.menuItemArrange.Text = "Arrange by";
            // 
            // menuItemStorage
            // 
            this.menuItemStorage.Text = "Storage...";
            this.menuItemStorage.Click += this.MenuItemStorage_Click;
            // 
            // menuItemStandardImages
            // 
            this.menuItemStandardImages.Text = "Standard images...";
            this.menuItemStandardImages.Click += this.MenuItemStandardImages_Click;
            // 
            // buttonOptions
            // 
            this.buttonOptions.Location = new Point(8, 464);
            this.buttonOptions.Name = "buttonOptions";
            this.buttonOptions.TabIndex = 7;
            this.buttonOptions.Text = "&Options";
            this.buttonOptions.Click += this.ButtonOptions_Click;
            // 
            // menuItemStandardLocomotiveFunctions
            // 
            this.menuItemStandardLocomotiveFunctions.Text = "Standard Locomotive Functions...";
            this.menuItemStandardLocomotiveFunctions.Click += this.MenuItemStandardLocomotiveFunctions_Click;
            // 
            // LocomotiveCatalog
            // 
            this.AutoScaleDimensions = new SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new SizeF(5, 13);
            this.ClientSize = new Size(480, 494);
            this.ControlBox = false;
            this.Controls.AddRange(new Control[] {
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

        private Button buttonClose;
        private Button buttonAdd;
        private Button buttonEdit;
        private Button buttonRemove;
        private CommonUI.Controls.LocomotiveTypeList locomotiveTypeList;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private ContextMenuStrip contextMenuOptions;
        private ToolStripMenuItem menuItemArrange;
        private ToolStripMenuItem menuItemStorage;
        private ToolStripMenuItem menuItemStandardImages;
        private Button buttonOptions;
        private ToolStripMenuItem menuItemStandardLocomotiveFunctions;
    }
}
