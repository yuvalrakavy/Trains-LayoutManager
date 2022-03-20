namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for SelectTripPlanIcon.
    /// </summary>
    partial class SelectTripPlanIcon : System.Windows.Forms.UserControl {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.panelIcons = new LayoutManager.CommonUI.Controls.SelectTripPlanIcon.NoBackgroundErasePanel();
            this.hScrollBar = new System.Windows.Forms.HScrollBar();
            this.buttonAdd = new Button();
            this.buttonDelete = new Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.panelIcons.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelIcons
            // 
            this.panelIcons.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right);
            this.panelIcons.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelIcons.Controls.Add(this.hScrollBar);
            this.panelIcons.Location = new System.Drawing.Point(2, 2);
            this.panelIcons.Name = "panelIcons";
            this.panelIcons.Size = new System.Drawing.Size(244, 60);
            this.panelIcons.TabIndex = 0;
            this.panelIcons.Resize += new EventHandler(this.PanelIcons_Resize);
            this.panelIcons.Paint += new PaintEventHandler(this.PanelIcons_Paint);
            this.panelIcons.MouseDown += new MouseEventHandler(this.PanelIcons_MouseDown);
            // 
            // hScrollBar
            // 
            this.hScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hScrollBar.Location = new System.Drawing.Point(0, 46);
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.Size = new System.Drawing.Size(242, 12);
            this.hScrollBar.TabIndex = 0;
            this.hScrollBar.Scroll += new ScrollEventHandler(this.HScrollBar_Scroll);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(4, 66);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(53, 17);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "Add...";
            this.buttonAdd.Click += new EventHandler(this.ButtonAdd_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(61, 66);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(53, 17);
            this.buttonDelete.TabIndex = 1;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.Click += new EventHandler(this.ButtonDelete_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "ico";
            this.openFileDialog.Filter = "Icon files|*.ico|All files|*.*";
            this.openFileDialog.Multiselect = true;
            // 
            // SelectTripPlanIcon
            // 
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.panelIcons);
            this.Controls.Add(this.buttonDelete);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoSize = true;
            this.Name = "SelectTripPlanIcon";
            this.Size = new System.Drawing.Size(248, 86);
            this.panelIcons.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion
        private NoBackgroundErasePanel panelIcons;
        private Button buttonAdd;
        private Button buttonDelete;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.HScrollBar hScrollBar;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
    }
}
