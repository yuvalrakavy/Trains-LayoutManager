using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Linq;
using LayoutManager.CommonUI;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TripPlanManageCommonDestinations.
    /// </summary>
    partial class TripPlanCommonDestinations : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.treeViewDestinations = new TreeView();
            this.buttonDelete = new Button();
            this.buttonEdit = new Button();
            this.buttonClose = new Button();
            this.buttonAdd = new Button();
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
            this.treeViewDestinations.Size = new System.Drawing.Size(320, 208);
            this.treeViewDestinations.TabIndex = 0;
            this.treeViewDestinations.AfterSelect += this.TreeViewDestinations_AfterSelect;
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonDelete.Location = new System.Drawing.Point(136, 224);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(56, 19);
            this.buttonDelete.TabIndex = 3;
            this.buttonDelete.Text = "&Delete";
            this.buttonDelete.Click += this.ButtonDelete_Click;
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonEdit.Location = new System.Drawing.Point(72, 224);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(56, 19);
            this.buttonEdit.TabIndex = 2;
            this.buttonEdit.Text = "&Edit...";
            this.buttonEdit.Click += this.ButtonEdit_Click;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonClose.Location = new System.Drawing.Point(272, 224);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(56, 19);
            this.buttonClose.TabIndex = 4;
            this.buttonClose.Text = "&Close";
            this.buttonClose.Click += this.ButtonClose_Click;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonAdd.Location = new System.Drawing.Point(8, 224);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(56, 19);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "&Add...";
            this.buttonAdd.Click += this.ButtonAdd_Click;
            // 
            // TripPlanCommonDestinations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 254);
            this.ControlBox = false;
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.treeViewDestinations);
            this.Controls.Add(this.buttonEdit);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonAdd);
            this.Name = "TripPlanCommonDestinations";
            this.ShowInTaskbar = false;
            this.Text = "\"Smart\" Destinations";
            this.Closing += this.TripPlanCommonDestinations_Closing;
            this.ResumeLayout(false);
        }
        #endregion

        private TreeView treeViewDestinations;
        private Button buttonDelete;
        private Button buttonClose;
        private Button buttonEdit;
        private Button buttonAdd;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

    }
}

