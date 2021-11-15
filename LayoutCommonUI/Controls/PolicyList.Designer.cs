namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for PolicyList.
    /// </summary>
    partial class PolicyList : System.Windows.Forms.UserControl, IPolicyListCustomizer, IControlSupportViewOnly {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.listViewPolicies = new ListView();
            this.columnHeaderName = new ColumnHeader();
            this.buttonNew = new Button();
            this.buttonRemove = new Button();
            this.buttonEdit = new Button();
            this.SuspendLayout();
            // 
            // listViewPolicies
            // 
            this.listViewPolicies.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right);
            this.listViewPolicies.CheckBoxes = true;
            this.listViewPolicies.Columns.AddRange(new ColumnHeader[] {
                                                                                               this.columnHeaderName});
            this.listViewPolicies.FullRowSelect = true;
            this.listViewPolicies.GridLines = true;
            this.listViewPolicies.HideSelection = false;
            this.listViewPolicies.Location = new System.Drawing.Point(8, 8);
            this.listViewPolicies.MultiSelect = false;
            this.listViewPolicies.Name = "listViewPolicies";
            this.listViewPolicies.Size = new System.Drawing.Size(368, 112);
            this.listViewPolicies.TabIndex = 0;
            this.listViewPolicies.View = System.Windows.Forms.View.Details;
            this.listViewPolicies.MouseUp += this.ListViewPolicies_MouseUp;
            this.listViewPolicies.SelectedIndexChanged += this.UpdateButtons;
            this.listViewPolicies.ItemCheck += this.ListViewPolicies_ItemCheck;
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 92;
            // 
            // buttonNew
            // 
            this.buttonNew.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonNew.Location = new System.Drawing.Point(8, 128);
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new System.Drawing.Size(72, 23);
            this.buttonNew.TabIndex = 1;
            this.buttonNew.Text = "&New...";
            this.buttonNew.Click += this.ButtonNew_Click;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonRemove.Location = new System.Drawing.Point(168, 128);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(72, 23);
            this.buttonRemove.TabIndex = 3;
            this.buttonRemove.Text = "&Remove";
            this.buttonRemove.Click += this.ButtonRemove_Click;
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonEdit.Location = new System.Drawing.Point(88, 128);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(72, 23);
            this.buttonEdit.TabIndex = 2;
            this.buttonEdit.Text = "&Edit";
            this.buttonEdit.Click += this.ButtonEdit_Click;
            // 
            // PolicyList
            // 
            this.Controls.Add(this.buttonNew);
            this.Controls.Add(this.listViewPolicies);
            this.Controls.Add(this.buttonRemove);
            this.Controls.Add(this.buttonEdit);
            this.Name = "PolicyList";
            this.Size = new System.Drawing.Size(384, 160);
            this.ResumeLayout(false);
        }
        #endregion
        private ListView listViewPolicies;
        private ColumnHeader columnHeaderName;
        private Button buttonNew;
        private Button buttonRemove;
        private Button buttonEdit;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;

    }
}

