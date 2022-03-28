using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.Model;
using System;

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
            this.components = new System.ComponentModel.Container();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.contextMenuOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemArrange = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemStorage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemStandardImages = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemStandardLocomotiveFunctions = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonOptions = new System.Windows.Forms.Button();
            this.xmlQueryList = new LayoutManager.CommonUI.Controls.XmlQueryList();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelLine1Buttons = new System.Windows.Forms.Panel();
            this.panelLine2Buttons = new System.Windows.Forms.Panel();
            this.contextMenuOptions.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelLine1Buttons.SuspendLayout();
            this.panelLine2Buttons.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new System.Drawing.Point(938, 7);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(195, 57);
            this.buttonClose.TabIndex = 6;
            this.buttonClose.Text = "&Close";
            this.buttonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAdd.Location = new System.Drawing.Point(13, 7);
            this.buttonAdd.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(195, 57);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "&Add...";
            this.buttonAdd.Click += new System.EventHandler(this.ButtonAdd_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonEdit.Location = new System.Drawing.Point(224, 7);
            this.buttonEdit.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(195, 57);
            this.buttonEdit.TabIndex = 2;
            this.buttonEdit.Text = "&Edit...";
            this.buttonEdit.Click += new System.EventHandler(this.ButtonEdit_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRemove.Location = new System.Drawing.Point(435, 7);
            this.buttonRemove.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(195, 57);
            this.buttonRemove.TabIndex = 3;
            this.buttonRemove.Text = "&Remove";
            this.buttonRemove.Click += new System.EventHandler(this.ButtonRemove_Click);
            // 
            // contextMenuOptions
            // 
            this.contextMenuOptions.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemArrange,
            this.menuItemStorage,
            this.menuItemStandardImages,
            this.menuItemStandardLocomotiveFunctions});
            this.contextMenuOptions.Name = "contextMenuOptions";
            this.contextMenuOptions.Size = new System.Drawing.Size(439, 156);
            // 
            // menuItemArrange
            // 
            this.menuItemArrange.Name = "menuItemArrange";
            this.menuItemArrange.Size = new System.Drawing.Size(438, 38);
            this.menuItemArrange.Text = "Arrange by";
            // 
            // menuItemStorage
            // 
            this.menuItemStorage.Name = "menuItemStorage";
            this.menuItemStorage.Size = new System.Drawing.Size(438, 38);
            this.menuItemStorage.Text = "Storage...";
            this.menuItemStorage.Click += new System.EventHandler(this.MenuItemStorage_Click);
            // 
            // menuItemStandardImages
            // 
            this.menuItemStandardImages.Name = "menuItemStandardImages";
            this.menuItemStandardImages.Size = new System.Drawing.Size(438, 38);
            this.menuItemStandardImages.Text = "Standard images...";
            this.menuItemStandardImages.Click += new System.EventHandler(this.MenuItemStandardImages_Click);
            // 
            // menuItemStandardLocomotiveFunctions
            // 
            this.menuItemStandardLocomotiveFunctions.Name = "menuItemStandardLocomotiveFunctions";
            this.menuItemStandardLocomotiveFunctions.Size = new System.Drawing.Size(438, 38);
            this.menuItemStandardLocomotiveFunctions.Text = "Standard Locomotive Functions...";
            this.menuItemStandardLocomotiveFunctions.Click += new System.EventHandler(this.MenuItemStandardLocomotiveFunctions_Click);
            // 
            // buttonOptions
            // 
            this.buttonOptions.Location = new System.Drawing.Point(14, 7);
            this.buttonOptions.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonOptions.Name = "buttonOptions";
            this.buttonOptions.Size = new System.Drawing.Size(195, 57);
            this.buttonOptions.TabIndex = 7;
            this.buttonOptions.Text = "&Options";
            this.buttonOptions.Click += new System.EventHandler(this.ButtonOptions_Click);
            // 
            // xmlQueryList
            // 
            this.xmlQueryList.ContainerElement = null;
            this.xmlQueryList.CurrentListLayout = null;
            this.xmlQueryList.CurrentListLayoutIndex = -1;
            this.xmlQueryList.DefaultSortField = null;
            this.xmlQueryList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xmlQueryList.Location = new System.Drawing.Point(16, 7);
            this.xmlQueryList.Margin = new System.Windows.Forms.Padding(16, 7, 16, 7);
            this.xmlQueryList.Name = "xmlQueryList";
            this.xmlQueryList.Size = new System.Drawing.Size(1120, 1042);
            this.xmlQueryList.TabIndex = 8;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.xmlQueryList, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panelLine1Buttons, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panelLine2Buttons, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1152, 1216);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // panelLine1Buttons
            // 
            this.panelLine1Buttons.Controls.Add(this.buttonAdd);
            this.panelLine1Buttons.Controls.Add(this.buttonEdit);
            this.panelLine1Buttons.Controls.Add(this.buttonRemove);
            this.panelLine1Buttons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLine1Buttons.Location = new System.Drawing.Point(3, 1059);
            this.panelLine1Buttons.Name = "panelLine1Buttons";
            this.panelLine1Buttons.Size = new System.Drawing.Size(1146, 74);
            this.panelLine1Buttons.TabIndex = 10;
            // 
            // panelLine2Buttons
            // 
            this.panelLine2Buttons.Controls.Add(this.buttonOptions);
            this.panelLine2Buttons.Controls.Add(this.buttonClose);
            this.panelLine2Buttons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLine2Buttons.Location = new System.Drawing.Point(3, 1139);
            this.panelLine2Buttons.Name = "panelLine2Buttons";
            this.panelLine2Buttons.Size = new System.Drawing.Size(1146, 74);
            this.panelLine2Buttons.TabIndex = 11;
            // 
            // LocomotiveCatalog
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1152, 1216);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "LocomotiveCatalog";
            this.ShowInTaskbar = false;
            this.Text = "Locomotive Catalog";
            this.contextMenuOptions.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panelLine1Buttons.ResumeLayout(false);
            this.panelLine2Buttons.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private Button buttonClose;
        private Button buttonAdd;
        private Button buttonEdit;
        private Button buttonRemove;

        private ContextMenuStrip contextMenuOptions;
        private ToolStripMenuItem menuItemArrange;
        private ToolStripMenuItem menuItemStorage;
        private ToolStripMenuItem menuItemStandardImages;
        private Button buttonOptions;
        private ToolStripMenuItem menuItemStandardLocomotiveFunctions;
        private IContainer components;
        private CommonUI.Controls.XmlQueryList xmlQueryList;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panelLine1Buttons;
        private Panel panelLine2Buttons;
    }
}
