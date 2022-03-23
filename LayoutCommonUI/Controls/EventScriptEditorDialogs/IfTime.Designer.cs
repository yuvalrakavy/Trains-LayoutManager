
namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for IfTime.
    /// </summary>
    partial class IfTime : System.Windows.Forms.Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.treeView = new TreeView();
            this.buttonAdd = new Button();
            this.buttonEdit = new Button();
            this.buttonDelete = new Button();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.contextMenuAdd = new ContextMenuStrip();
            this.menuItemSeconds = new LayoutMenuItem();
            this.menuItemMinutes = new LayoutMenuItem();
            this.menuItemHours = new LayoutMenuItem();
            this.menuItemDayOfWeek = new LayoutMenuItem();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right);
            this.treeView.ImageIndex = -1;
            this.treeView.Location = new System.Drawing.Point(8, 8);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = -1;
            this.treeView.Size = new System.Drawing.Size(200, 200);
            this.treeView.TabIndex = 0;
            this.treeView.DoubleClick += new System.EventHandler(this.TreeView_DoubleClick);
            this.treeView.AfterSelect += new TreeViewEventHandler(this.TreeView_AfterSelect);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonAdd.Location = new System.Drawing.Point(8, 212);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(61, 19);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "&Add";
            this.buttonAdd.Click += new System.EventHandler(this.ButtonAdd_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonEdit.Location = new System.Drawing.Point(72, 212);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(61, 19);
            this.buttonEdit.TabIndex = 2;
            this.buttonEdit.Text = "&Edit...";
            this.buttonEdit.Click += new System.EventHandler(this.ButtonEdit_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonDelete.Location = new System.Drawing.Point(136, 212);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(61, 19);
            this.buttonDelete.TabIndex = 3;
            this.buttonDelete.Text = "&Delete";
            this.buttonDelete.Click += new System.EventHandler(this.ButtonDelete_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonOK.Location = new System.Drawing.Point(73, 250);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(64, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(145, 250);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(64, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // contextMenuAdd
            // 
            this.contextMenuAdd.Items.AddRange(new ToolStripMenuItem[] {
                                                                                           this.menuItemSeconds,
                                                                                           this.menuItemMinutes,
                                                                                           this.menuItemHours,
                                                                                           this.menuItemDayOfWeek});
            // 
            // menuItemSeconds
            // 
            this.menuItemSeconds.Text = "Seconds...";
            this.menuItemSeconds.Click += new System.EventHandler(this.MenuItemSeconds_Click);
            // 
            // menuItemMinutes
            // 
            this.menuItemMinutes.Text = "Minutes...";
            this.menuItemMinutes.Click += new System.EventHandler(this.MenuItemMinutes_Click);
            // 
            // menuItemHours
            // 
            this.menuItemHours.Text = "Hours...";
            this.menuItemHours.Click += new System.EventHandler(this.MenuItemHours_Click);
            // 
            // menuItemDayOfWeek
            // 
            this.menuItemDayOfWeek.Text = "Day of week...";
            this.menuItemDayOfWeek.Click += new System.EventHandler(this.MenuItemDayOfWeek_Click);
            // 
            // IfTime
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(216, 278);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.buttonEdit);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonCancel);
            this.Name = "IfTime";
            this.ShowInTaskbar = false;
            this.Text = "If (Time)";
            this.ResumeLayout(false);
        }
        #endregion

        private TreeView treeView;
        private Button buttonAdd;
        private Button buttonEdit;
        private Button buttonDelete;
        private Button buttonOK;
        private Button buttonCancel;
        private ToolStripMenuItem menuItemSeconds;
        private ToolStripMenuItem menuItemMinutes;
        private ToolStripMenuItem menuItemHours;
        private ToolStripMenuItem menuItemDayOfWeek;
        private ContextMenuStrip contextMenuAdd;
    }
}
