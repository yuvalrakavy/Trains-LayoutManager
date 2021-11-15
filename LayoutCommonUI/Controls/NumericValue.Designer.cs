namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for NumericValue.
    /// </summary>
    public partial class NumericValue : System.Windows.Forms.UserControl {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.linkMenuOperation = new LayoutManager.CommonUI.Controls.LinkMenu();
            this.textBoxValue = new TextBox();
            this.SuspendLayout();
            // 
            // linkMenuOperation
            // 
            this.linkMenuOperation.Location = new System.Drawing.Point(8, 2);
            this.linkMenuOperation.Name = "linkMenuOperation";
            this.linkMenuOperation.Options = new string[] {
                                                              "Set to",
                                                              "Add"};
            this.linkMenuOperation.SelectedIndex = 0;
            this.linkMenuOperation.Size = new System.Drawing.Size(48, 23);
            this.linkMenuOperation.TabIndex = 5;
            this.linkMenuOperation.TabStop = true;
            this.linkMenuOperation.Text = "Set to";
            this.linkMenuOperation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxValue
            // 
            this.textBoxValue.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right);
            this.textBoxValue.Location = new System.Drawing.Point(48, 3);
            this.textBoxValue.Name = "textBoxValue";
            this.textBoxValue.Size = new System.Drawing.Size(111, 20);
            this.textBoxValue.TabIndex = 6;
            this.textBoxValue.Text = "";
            // 
            // NumericValue
            // 
            this.Controls.Add(this.textBoxValue);
            this.Controls.Add(this.linkMenuOperation);
            this.Name = "NumericValue";
            this.Size = new System.Drawing.Size(168, 27);
            this.ResumeLayout(false);
        }
        #endregion
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenuOperation;
        private TextBox textBoxValue;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;


    }
}