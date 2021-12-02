using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;
using LayoutManager.CommonUI.Controls;
using LayoutManager.CommonUI;

namespace LayoutManager {
    /// <summary>
    /// Summary description for LocomotivesViewer.
    /// </summary>
    partial class LocomotivesViewer : UserControl {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(LocomotivesViewer));
            this.contextMenuAdd = new ContextMenuStrip();
            this.menuItemAddLocomotive = new ToolStripMenuItem();
            this.menuItemAddTrain = new ToolStripMenuItem();
            this.contextMenuOptions = new ContextMenuStrip();
            this.menuItemArrange = new ToolStripMenuItem();
            this.menuItemStorage = new ToolStripMenuItem();
            this.buttonAdd = new Button();
            this.buttonEdit = new Button();
            this.buttonDelete = new Button();
            this.buttonOptions = new Button();
            this.panel1 = new Panel();
            this.buttonClose = new Button();
            this.imageListCloseButton = new ImageList(this.components);
            this.contextMenuArrange = new ContextMenuStrip();
            this.locomotiveList = new LocomotiveList();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuAdd
            // 
            this.contextMenuAdd.Items.AddRange(new ToolStripMenuItem[] {
                                                                                           this.menuItemAddLocomotive,
                                                                                           this.menuItemAddTrain});
            // 
            // menuItemAddLocomotive
            // 
            this.menuItemAddLocomotive.Text = "&Locomotive...";
            this.menuItemAddLocomotive.Click += this.MenuItemAddLocomotive_Click;
            // 
            // menuItemAddTrain
            // 
            this.menuItemAddTrain.Text = "&Train...";
            this.menuItemAddTrain.Click += this.MenuItemAddTrain_Click;
            // 
            // contextMenuOptions
            // 
            this.contextMenuOptions.Items.AddRange(new ToolStripMenuItem[] {
                                                                                               this.menuItemArrange,
                                                                                               this.menuItemStorage});
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
            // buttonAdd
            // 
            this.buttonAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonAdd.Location = new Point(12, 464);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new Size(56, 20);
            this.buttonAdd.TabIndex = 5;
            this.buttonAdd.Text = "&Add";
            this.buttonAdd.Click += this.ButtonAdd_Click;
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonEdit.Location = new Point(76, 464);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new Size(56, 20);
            this.buttonEdit.TabIndex = 6;
            this.buttonEdit.Text = "&Edit...";
            this.buttonEdit.Click += this.ButtonEdit_Click;
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonDelete.Location = new Point(140, 464);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new Size(56, 20);
            this.buttonDelete.TabIndex = 3;
            this.buttonDelete.Text = "&Delete";
            this.buttonDelete.Click += this.ButtonDelete_Click;
            // 
            // buttonOptions
            // 
            this.buttonOptions.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonOptions.Location = new Point(134, 488);
            this.buttonOptions.Name = "buttonOptions";
            this.buttonOptions.Size = new Size(62, 20);
            this.buttonOptions.TabIndex = 4;
            this.buttonOptions.Text = "&Options...";
            this.buttonOptions.Click += this.ButtonOptions_Click;
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel1.Controls.AddRange(new Control[] {
                                                                                 this.buttonClose});
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(208, 20);
            this.panel1.TabIndex = 7;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClose.Image = (Bitmap)resources.GetObject("buttonClose.Image");
            this.buttonClose.ImageIndex = 0;
            this.buttonClose.ImageList = this.imageListCloseButton;
            this.buttonClose.Location = new Point(186, 1);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new Size(16, 16);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Click += this.ButtonClose_Click;
            // 
            // imageListCloseButton
            // 
            this.imageListCloseButton.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListCloseButton.ImageSize = new Size(16, 16);
            this.imageListCloseButton.ImageStream = (ImageListStreamer)resources.GetObject("imageListCloseButton.ImageStream");
            this.imageListCloseButton.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // locomotiveList
            // 
            this.locomotiveList.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.locomotiveList.ContainerElement = null;
            this.locomotiveList.CurrentListLayout = null;
            this.locomotiveList.CurrentListLayoutIndex = -1;
            this.locomotiveList.DefaultSortField = "Name";
            this.locomotiveList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.locomotiveList.Location = new Point(4, 24);
            this.locomotiveList.Name = "locomotiveList";
            this.locomotiveList.ShowOnlyLocomotives = false;
            this.locomotiveList.Size = new Size(196, 432);
            this.locomotiveList.TabIndex = 8;
            this.locomotiveList.MouseDown += this.LocomotiveList_MouseDown;
            this.locomotiveList.SelectedIndexChanged += this.LocomotiveList_SelectedIndexChanged;
            // 
            // LocomotivesViewer
            // 
            this.Controls.AddRange(new Control[] {
                                                                          this.locomotiveList,
                                                                          this.panel1,
                                                                          this.buttonAdd,
                                                                          this.buttonEdit,
                                                                          this.buttonDelete,
                                                                          this.buttonOptions});
            this.Name = "LocomotivesViewer";
            this.Size = new Size(208, 512);
            this.AutoScaleDimensions = new SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private ContextMenuStrip contextMenuAdd;
        private ToolStripMenuItem menuItemAddLocomotive;
        private IContainer components;
        private ContextMenuStrip contextMenuOptions;
        private ToolStripMenuItem menuItemArrange;
        private Button buttonAdd;
        private Button buttonEdit;
        private Button buttonDelete;
        private Button buttonOptions;
        private Panel panel1;
        private Button buttonClose;
        private ImageList imageListCloseButton;
        private ContextMenuStrip contextMenuArrange;
        private LocomotiveList locomotiveList;
        private ToolStripMenuItem menuItemAddTrain;
        private ToolStripMenuItem menuItemStorage;
    }
}

