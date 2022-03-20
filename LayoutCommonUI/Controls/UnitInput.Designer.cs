namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for UnitInput.
    /// </summary>
    public partial class UnitInput : System.Windows.Forms.UserControl {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.textBoxValue = new TextBox();
            this.linkMenuUnits = new LayoutManager.CommonUI.Controls.LinkMenu();
            this.SuspendLayout();
            // 
            // textBoxValue
            // 
            this.textBoxValue.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.textBoxValue.Location = new System.Drawing.Point(7, 2);
            this.textBoxValue.Name = "textBoxValue";
            this.textBoxValue.Size = new System.Drawing.Size(48, 20);
            this.textBoxValue.TabIndex = 0;
            this.textBoxValue.Text = "";
            // 
            // linkMenuUnits
            // 
            this.linkMenuUnits.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Right;
            this.linkMenuUnits.Location = new System.Drawing.Point(60, 2);
            this.linkMenuUnits.Name = "linkMenuUnits";
            this.linkMenuUnits.Options = new string[0];
            this.linkMenuUnits.SelectedIndex = -1;
            this.linkMenuUnits.Size = new System.Drawing.Size(40, 20);
            this.linkMenuUnits.TabIndex = 1;
            this.linkMenuUnits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkMenuUnits.ValueChanged += new EventHandler(this.LinkMenuUnits_ValueChanged);
            // 
            // UnitInput
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.linkMenuUnits,
                                                                          this.textBoxValue});
            this.Name = "UnitInput";
            this.Size = new System.Drawing.Size(104, 24);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoSize = true;
            this.ResumeLayout(false);
        }
        #endregion
        private TextBox textBoxValue;
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenuUnits;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
    }
}