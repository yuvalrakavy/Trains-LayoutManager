using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TripPlanManageCommonDestinations.
    /// </summary>
    partial class TripPlanManageCommonDestinations : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.treeViewDestinations = new TreeView();
            this.buttonDelete = new Button();
            this.buttonRename = new Button();
            this.buttonClose = new Button();
            this.SuspendLayout();
            // 
            // treeViewDestinations
            // 
            this.treeViewDestinations.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right);
            this.treeViewDestinations.HideSelection = false;
            this.treeViewDestinations.ImageIndex = -1;
            this.treeViewDestinations.Location = new System.Drawing.Point(8, 8);
            this.treeViewDestinations.Name = "treeViewDestinations";
            this.treeViewDestinations.SelectedImageIndex = -1;
            this.treeViewDestinations.ShowLines = false;
            this.treeViewDestinations.Size = new System.Drawing.Size(216, 208);
            this.treeViewDestinations.TabIndex = 0;
            this.treeViewDestinations.AfterSelect += this.TreeViewDestinations_AfterSelect;
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonDelete.Location = new System.Drawing.Point(8, 224);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(56, 19);
            this.buttonDelete.TabIndex = 1;
            this.buttonDelete.Text = "&Delete";
            this.buttonDelete.Click += this.ButtonDelete_Click;
            // 
            // buttonRename
            // 
            this.buttonRename.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonRename.Location = new System.Drawing.Point(72, 224);
            this.buttonRename.Name = "buttonRename";
            this.buttonRename.Size = new System.Drawing.Size(56, 19);
            this.buttonRename.TabIndex = 1;
            this.buttonRename.Text = "&Rename";
            this.buttonRename.Click += this.ButtonRename_Click;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonClose.Location = new System.Drawing.Point(168, 224);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(56, 19);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "&Close";
            this.buttonClose.Click += this.ButtonClose_Click;
            // 
            // TripPlanManageCommonDestinations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(232, 254);
            this.ControlBox = false;
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.treeViewDestinations);
            this.Controls.Add(this.buttonRename);
            this.Controls.Add(this.buttonClose);
            this.Name = "TripPlanManageCommonDestinations";
            this.ShowInTaskbar = false;
            this.Text = "Manage \"Smart\" Destinations";
            this.ResumeLayout(false);
        }
        #endregion

        private TreeView treeViewDestinations;
        private Button buttonDelete;
        private Button buttonRename;
        private Button buttonClose;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}
