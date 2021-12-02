using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for StandardFunctions.
    /// </summary>
    partial class StandardFunctions : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.listViewFunctionInfo = new ListView();
            this.buttonNew = new Button();
            this.buttonEdit = new Button();
            this.buttonDelete = new Button();
            this.buttonClose = new Button();
            this.columnHeaderType = new ColumnHeader();
            this.columnHeaderName = new ColumnHeader();
            this.columnHeaderDescription = new ColumnHeader();
            this.SuspendLayout();
            // 
            // listViewFunctionInfo
            // 
            this.listViewFunctionInfo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.listViewFunctionInfo.Columns.AddRange(new ColumnHeader[] {
                                                                                                   this.columnHeaderType,
                                                                                                   this.columnHeaderName,
                                                                                                   this.columnHeaderDescription});
            this.listViewFunctionInfo.FullRowSelect = true;
            this.listViewFunctionInfo.GridLines = true;
            this.listViewFunctionInfo.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewFunctionInfo.HideSelection = false;
            this.listViewFunctionInfo.Location = new System.Drawing.Point(8, 8);
            this.listViewFunctionInfo.MultiSelect = false;
            this.listViewFunctionInfo.Name = "listViewFunctionInfo";
            this.listViewFunctionInfo.Size = new System.Drawing.Size(312, 176);
            this.listViewFunctionInfo.TabIndex = 0;
            this.listViewFunctionInfo.View = System.Windows.Forms.View.Details;
            this.listViewFunctionInfo.SelectedIndexChanged += this.ListViewFunctionInfo_SelectedIndexChanged;
            // 
            // buttonNew
            // 
            this.buttonNew.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonNew.Location = new System.Drawing.Point(8, 188);
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new System.Drawing.Size(70, 23);
            this.buttonNew.TabIndex = 1;
            this.buttonNew.Text = "&New";
            this.buttonNew.Click += this.ButtonNew_Click;
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonEdit.Location = new System.Drawing.Point(83, 188);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(70, 23);
            this.buttonEdit.TabIndex = 2;
            this.buttonEdit.Text = "&Edit";
            this.buttonEdit.Click += this.ButtonEdit_Click;
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonDelete.Location = new System.Drawing.Point(159, 188);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(70, 23);
            this.buttonDelete.TabIndex = 3;
            this.buttonDelete.Text = "&Delete";
            this.buttonDelete.Click += this.ButtonDelete_Click;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonClose.Location = new System.Drawing.Point(250, 188);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(70, 23);
            this.buttonClose.TabIndex = 4;
            this.buttonClose.Text = "Close";
            this.buttonClose.Click += this.ButtonClose_Click;
            // 
            // columnHeaderType
            // 
            this.columnHeaderType.Text = "Type";
            this.columnHeaderType.Width = 50;
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 90;
            // 
            // columnHeaderDescription
            // 
            this.columnHeaderDescription.Text = "Description";
            this.columnHeaderDescription.Width = 168;
            // 
            // StandardFunctions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 222);
            this.ControlBox = false;
            this.Controls.AddRange(new Control[] {
                                                                          this.buttonClose,
                                                                          this.buttonNew,
                                                                          this.listViewFunctionInfo,
                                                                          this.buttonEdit,
                                                                          this.buttonDelete});
            this.MinimumSize = new System.Drawing.Size(336, 256);
            this.Name = "StandardFunctions";
            this.ShowInTaskbar = false;
            this.Text = "Common Locomotive Functions";
            this.ResumeLayout(false);
        }
        #endregion

        private ListView listViewFunctionInfo;
        private Button buttonNew;
        private Button buttonEdit;
        private Button buttonDelete;
        private Button buttonClose;
        private ColumnHeader columnHeaderType;
        private ColumnHeader columnHeaderName;
        private ColumnHeader columnHeaderDescription;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}
