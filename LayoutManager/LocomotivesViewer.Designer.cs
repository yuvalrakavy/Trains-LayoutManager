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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LocomotivesViewer));
            this.contextMenuAdd = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemAddLocomotive = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAddTrain = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemArrange = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemStorage = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonClose = new System.Windows.Forms.Button();
            this.imageListCloseButton = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuArrange = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.xmlQueryList = new LayoutManager.CommonUI.Controls.XmlQueryList();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonOptions = new System.Windows.Forms.Button();
            this.contextMenuAdd.SuspendLayout();
            this.contextMenuOptions.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuAdd
            // 
            this.contextMenuAdd.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuAdd.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemAddLocomotive,
            this.menuItemAddTrain});
            this.contextMenuAdd.Name = "contextMenuAdd";
            this.contextMenuAdd.Size = new System.Drawing.Size(228, 80);
            // 
            // menuItemAddLocomotive
            // 
            this.menuItemAddLocomotive.Name = "menuItemAddLocomotive";
            this.menuItemAddLocomotive.Size = new System.Drawing.Size(227, 38);
            this.menuItemAddLocomotive.Text = "&Locomotive...";
            this.menuItemAddLocomotive.Click += new System.EventHandler(this.MenuItemAddLocomotive_Click);
            // 
            // menuItemAddTrain
            // 
            this.menuItemAddTrain.Name = "menuItemAddTrain";
            this.menuItemAddTrain.Size = new System.Drawing.Size(227, 38);
            this.menuItemAddTrain.Text = "&Train...";
            this.menuItemAddTrain.Click += new System.EventHandler(this.MenuItemAddTrain_Click);
            // 
            // contextMenuOptions
            // 
            this.contextMenuOptions.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemArrange,
            this.menuItemStorage});
            this.contextMenuOptions.Name = "contextMenuOptions";
            this.contextMenuOptions.Size = new System.Drawing.Size(206, 80);
            // 
            // menuItemArrange
            // 
            this.menuItemArrange.Name = "menuItemArrange";
            this.menuItemArrange.Size = new System.Drawing.Size(205, 38);
            this.menuItemArrange.Text = "Arrange by";
            // 
            // menuItemStorage
            // 
            this.menuItemStorage.Name = "menuItemStorage";
            this.menuItemStorage.Size = new System.Drawing.Size(205, 38);
            this.menuItemStorage.Text = "Storage...";
            this.menuItemStorage.Click += new System.EventHandler(this.MenuItemStorage_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAdd.AutoSize = true;
            this.buttonAdd.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonAdd.Location = new System.Drawing.Point(15, 869);
            this.buttonAdd.Margin = new System.Windows.Forms.Padding(5);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(131, 42);
            this.buttonAdd.TabIndex = 5;
            this.buttonAdd.Text = "&Add";
            this.buttonAdd.Click += new System.EventHandler(this.ButtonAdd_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEdit.AutoSize = true;
            this.buttonEdit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonEdit.Location = new System.Drawing.Point(156, 869);
            this.buttonEdit.Margin = new System.Windows.Forms.Padding(5);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(131, 42);
            this.buttonEdit.TabIndex = 6;
            this.buttonEdit.Text = "&Edit...";
            this.buttonEdit.Click += new System.EventHandler(this.ButtonEdit_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDelete.AutoSize = true;
            this.buttonDelete.Location = new System.Drawing.Point(297, 869);
            this.buttonDelete.Margin = new System.Windows.Forms.Padding(5);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(132, 42);
            this.buttonDelete.TabIndex = 3;
            this.buttonDelete.Text = "&Delete";
            this.buttonDelete.Click += new System.EventHandler(this.ButtonDelete_Click);
            // 
            // panel1
            // 
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 5);
            this.panel1.Controls.Add(this.buttonClose);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(446, 40);
            this.panel1.TabIndex = 7;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClose.ImageIndex = 0;
            this.buttonClose.ImageList = this.imageListCloseButton;
            this.buttonClose.Location = new System.Drawing.Point(409, 2);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(143, 103, 143, 103);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(32, 32);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // imageListCloseButton
            // 
            this.imageListCloseButton.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListCloseButton.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListCloseButton.ImageStream")));
            this.imageListCloseButton.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListCloseButton.Images.SetKeyName(0, "");
            // 
            // contextMenuArrange
            // 
            this.contextMenuArrange.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuArrange.Name = "contextMenuArrange";
            this.contextMenuArrange.Size = new System.Drawing.Size(61, 4);
            // 
            // xmlQueryList
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.xmlQueryList, 3);
            this.xmlQueryList.ContainerElement = null;
            this.xmlQueryList.CurrentListLayout = null;
            this.xmlQueryList.CurrentListLayoutIndex = -1;
            this.xmlQueryList.DefaultSortField = null;
            this.xmlQueryList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xmlQueryList.Location = new System.Drawing.Point(10, 50);
            this.xmlQueryList.Margin = new System.Windows.Forms.Padding(0);
            this.xmlQueryList.Name = "xmlQueryList";
            this.xmlQueryList.Size = new System.Drawing.Size(424, 805);
            this.xmlQueryList.TabIndex = 8;
            this.xmlQueryList.Load += new System.EventHandler(this.LocomotiveList_Load);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.23263F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.23263F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.53474F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.Controls.Add(this.xmlQueryList, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonAdd, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.buttonEdit, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.buttonDelete, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.buttonOptions, 1, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(446, 1000);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // buttonOptions
            // 
            this.buttonOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOptions.AutoSize = true;
            this.buttonOptions.Location = new System.Drawing.Point(15, 929);
            this.buttonOptions.Margin = new System.Windows.Forms.Padding(5);
            this.buttonOptions.Name = "buttonOptions";
            this.buttonOptions.Size = new System.Drawing.Size(131, 42);
            this.buttonOptions.TabIndex = 4;
            this.buttonOptions.Text = "&Options...";
            this.buttonOptions.Click += new System.EventHandler(this.ButtonOptions_Click);
            // 
            // LocomotivesViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(143, 103, 143, 103);
            this.Name = "LocomotivesViewer";
            this.Size = new System.Drawing.Size(446, 1000);
            this.Load += new System.EventHandler(this.LocomotivesViewer_Load);
            this.contextMenuAdd.ResumeLayout(false);
            this.contextMenuOptions.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private Panel panel1;
        private Button buttonClose;
        private ImageList imageListCloseButton;
        private ContextMenuStrip contextMenuArrange;
        private ToolStripMenuItem menuItemAddTrain;
        private ToolStripMenuItem menuItemStorage;
        private XmlQueryList xmlQueryList;
        private TableLayoutPanel tableLayoutPanel1;
        private Button buttonOptions;
    }
}

