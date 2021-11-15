namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for AttributesEditor.
    /// </summary>
    partial class AttributesEditor : System.Windows.Forms.UserControl, ICheckIfNameUsed, IControlSupportViewOnly {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.listViewAttributes = new ListView();
            this.columnHeaderName = new ColumnHeader();
            this.columnHeaderType = new ColumnHeader();
            this.columnHeaderValue = new ColumnHeader();
            this.buttonAdd = new Button();
            this.buttonEdit = new Button();
            this.buttonRemove = new Button();
            this.SuspendLayout();
            // 
            // listViewAttributes
            // 
            this.listViewAttributes.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.listViewAttributes.Columns.AddRange(new ColumnHeader[] {
                                                                                                 this.columnHeaderName,
                                                                                                 this.columnHeaderType,
                                                                                                 this.columnHeaderValue});
            this.listViewAttributes.FullRowSelect = true;
            this.listViewAttributes.GridLines = true;
            this.listViewAttributes.Location = new System.Drawing.Point(8, 8);
            this.listViewAttributes.MultiSelect = false;
            this.listViewAttributes.Name = "listViewAttributes";
            this.listViewAttributes.Size = new System.Drawing.Size(240, 160);
            this.listViewAttributes.TabIndex = 0;
            this.listViewAttributes.View = System.Windows.Forms.View.Details;
            this.listViewAttributes.DoubleClick += this.ButtonEdit_Click;
            this.listViewAttributes.SelectedIndexChanged += this.UpdateButtons;
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 90;
            // 
            // columnHeaderType
            // 
            this.columnHeaderType.Text = "Type";
            this.columnHeaderType.Width = 56;
            // 
            // columnHeaderValue
            // 
            this.columnHeaderValue.Text = "Value";
            this.columnHeaderValue.Width = 90;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonAdd.Location = new System.Drawing.Point(8, 176);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(56, 20);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "&Add";
            this.buttonAdd.Click += this.ButtonAdd_Click;
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonEdit.Location = new System.Drawing.Point(72, 176);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(56, 20);
            this.buttonEdit.TabIndex = 1;
            this.buttonEdit.Text = "&Edit";
            this.buttonEdit.Click += this.ButtonEdit_Click;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonRemove.Location = new System.Drawing.Point(136, 176);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(56, 20);
            this.buttonRemove.TabIndex = 1;
            this.buttonRemove.Text = "&Remove";
            this.buttonRemove.Click += this.ButtonRemove_Click;
            // 
            // AttributesEditor
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonAdd,
                                                                          this.listViewAttributes,
                                                                          this.buttonEdit,
                                                                          this.buttonRemove});
            this.Name = "AttributesEditor";
            this.Size = new System.Drawing.Size(256, 200);
            this.ResumeLayout(false);
        }
        #endregion

        private ListView listViewAttributes;
        private Button buttonAdd;
        private Button buttonEdit;
        private Button buttonRemove;
        private ColumnHeader columnHeaderName;
        private ColumnHeader columnHeaderType;
        private ColumnHeader columnHeaderValue;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
    }
}
