using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for StadardFonts.
    /// </summary>
    partial class StandardFonts : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.columnHeaderTitle = new ColumnHeader();
            this.buttonDelete = new Button();
            this.buttonEdit = new Button();
            this.buttonClose = new Button();
            this.buttonAdd = new Button();
            this.listViewFonts = new ListView();
            this.columnHeaderDescription = new ColumnHeader();
            this.contextMenuEdit = new ContextMenuStrip();
            this.menuItemEditSettings = new ToolStripMenuItem();
            this.menuItemEditTitle = new ToolStripMenuItem();
            this.menuItemFontID = new ToolStripMenuItem();
            this.SuspendLayout();
            // 
            // columnHeaderTitle
            // 
            this.columnHeaderTitle.Text = "Title";
            this.columnHeaderTitle.Width = 80;
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new Point(168, 224);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new Size(72, 23);
            this.buttonDelete.TabIndex = 4;
            this.buttonDelete.Text = "&Delete";
            this.buttonDelete.Click += this.ButtonDelete_Click;
            // 
            // buttonEdit
            // 
            this.buttonEdit.Location = new Point(88, 224);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new Size(72, 23);
            this.buttonEdit.TabIndex = 2;
            this.buttonEdit.Text = "&Edit";
            this.buttonEdit.Click += this.ButtonEdit_Click;
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new Point(168, 256);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new Size(72, 23);
            this.buttonClose.TabIndex = 5;
            this.buttonClose.Text = "&Close";
            this.buttonClose.Click += this.ButtonClose_Click;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new Point(8, 224);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new Size(72, 23);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "&New";
            this.buttonAdd.Click += this.ButtonAdd_Click;
            // 
            // listViewFonts
            // 
            this.listViewFonts.Columns.AddRange(new ColumnHeader[] {
            this.columnHeaderTitle,
            this.columnHeaderDescription});
            this.listViewFonts.HideSelection = false;
            this.listViewFonts.LabelEdit = true;
            this.listViewFonts.Location = new Point(8, 16);
            this.listViewFonts.Name = "listViewFonts";
            this.listViewFonts.Size = new Size(240, 200);
            this.listViewFonts.TabIndex = 0;
            this.listViewFonts.View = System.Windows.Forms.View.Details;
            this.listViewFonts.DoubleClick += this.ButtonEdit_Click;
            this.listViewFonts.AfterLabelEdit += this.ListViewFonts_AfterLabelEdit;
            // 
            // columnHeaderDescription
            // 
            this.columnHeaderDescription.Text = "Description";
            this.columnHeaderDescription.Width = 188;
            // 
            // contextMenuEdit
            // 
            this.contextMenuEdit.Items.AddRange(new ToolStripMenuItem[] {
            this.menuItemEditSettings,
            this.menuItemEditTitle,
            this.menuItemFontID});
            // 
            // menuItemEditSettings
            // 
            this.menuItemEditSettings.Text = "Settings...";
            this.menuItemEditSettings.Click += this.MenuItemEditSettings_Click;
            // 
            // menuItemEditTitle
            // 
            this.menuItemEditTitle.Text = "Title...";
            this.menuItemEditTitle.Click += this.MenuItemEditTitle_Click;
            // 
            // menuItemFontID
            // 
            this.menuItemFontID.Text = "Font Ref...";
            this.menuItemFontID.Click += this.MenuItemFontID_Click;
            // 
            // StadardFonts
            // 
            this.AutoScaleDimensions = new SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AcceptButton = this.buttonClose;
            this.ClientSize = new Size(258, 288);
            this.ControlBox = false;
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonEdit);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.listViewFonts);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "StadardFonts";
            this.ShowInTaskbar = false;
            this.Text = "Standard Fonts";
            this.ResumeLayout(false);
        }
        #endregion

        private ListView listViewFonts;
        private ColumnHeader columnHeaderTitle;
        private ColumnHeader columnHeaderDescription;
        private Button buttonAdd;
        private Button buttonEdit;
        private Button buttonDelete;
        private Button buttonClose;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
        private ContextMenuStrip contextMenuEdit;
        private ToolStripMenuItem menuItemEditSettings;
        private ToolStripMenuItem menuItemEditTitle;
        private ToolStripMenuItem menuItemFontID;
    }
}

