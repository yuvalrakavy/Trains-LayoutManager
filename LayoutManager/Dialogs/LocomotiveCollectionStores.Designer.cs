using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveCollectionStores.
    /// </summary>
    partial class LocomotiveCollectionStores : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.buttonClose = new Button();
            this.listViewStores = new ListView();
            this.columnHeaderName = new ColumnHeader();
            this.columnHeaderFile = new ColumnHeader();
            this.buttonNew = new Button();
            this.buttonEdit = new Button();
            this.buttonRemove = new Button();
            this.buttonSetAsDefault = new Button();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonClose.Location = new Point(496, 232);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.TabIndex = 5;
            this.buttonClose.Text = "&Close";
            this.buttonClose.Click += new EventHandler(this.ButtonClose_Click);
            // 
            // listViewStores
            // 
            this.listViewStores.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.listViewStores.Columns.AddRange(new ColumnHeader[] {
                                                                                             this.columnHeaderName,
                                                                                             this.columnHeaderFile});
            this.listViewStores.FullRowSelect = true;
            this.listViewStores.GridLines = true;
            this.listViewStores.HideSelection = false;
            this.listViewStores.Location = new Point(8, 8);
            this.listViewStores.Name = "listViewStores";
            this.listViewStores.Size = new Size(560, 192);
            this.listViewStores.TabIndex = 0;
            this.listViewStores.View = System.Windows.Forms.View.Details;
            this.listViewStores.SelectedIndexChanged += new EventHandler(this.ListViewStores_SelectedIndexChanged);
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 121;
            // 
            // columnHeaderFile
            // 
            this.columnHeaderFile.Text = "Stored in file";
            this.columnHeaderFile.Width = 430;
            // 
            // buttonNew
            // 
            this.buttonNew.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonNew.Location = new Point(8, 208);
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new Size(88, 23);
            this.buttonNew.TabIndex = 1;
            this.buttonNew.Text = "&New...";
            this.buttonNew.Click += new EventHandler(this.ButtonNew_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonEdit.Location = new Point(104, 208);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new Size(88, 23);
            this.buttonEdit.TabIndex = 2;
            this.buttonEdit.Text = "&Edit...";
            this.buttonEdit.Click += new EventHandler(this.ButtonEdit_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonRemove.Location = new Point(200, 208);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new Size(88, 23);
            this.buttonRemove.TabIndex = 3;
            this.buttonRemove.Text = "&Delete";
            this.buttonRemove.Click += new EventHandler(this.ButtonRemove_Click);
            // 
            // buttonSetAsDefault
            // 
            this.buttonSetAsDefault.Location = new Point(296, 208);
            this.buttonSetAsDefault.Name = "buttonSetAsDefault";
            this.buttonSetAsDefault.Size = new Size(88, 23);
            this.buttonSetAsDefault.TabIndex = 4;
            this.buttonSetAsDefault.Text = "Set as default";
            this.buttonSetAsDefault.Click += new EventHandler(this.ButtonSetAsDefault_Click);
            // 
            // LocomotiveCollectionStores
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new Size(576, 262);
            this.ControlBox = false;
            this.Controls.AddRange(new Control[] {
                                                                          this.buttonSetAsDefault,
                                                                          this.buttonNew,
                                                                          this.listViewStores,
                                                                          this.buttonClose,
                                                                          this.buttonEdit,
                                                                          this.buttonRemove});
            this.Name = "LocomotiveCollectionStores";
            this.ShowInTaskbar = false;
            this.Text = "Locomotive Catalog Storage";
            this.ResumeLayout(false);
        }
        #endregion

        private Button buttonClose;
        private ListView listViewStores;
        private Button buttonNew;
        private Button buttonEdit;
        private Button buttonRemove;
        private ColumnHeader columnHeaderName;
        private ColumnHeader columnHeaderFile;
        private Button buttonSetAsDefault;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}

