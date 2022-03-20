namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for OperandValueOf.
    /// </summary>
    public partial class OperandValueOf : System.Windows.Forms.UserControl {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.comboBoxTag = new ComboBox();
            this.comboBoxSymbol = new ComboBox();
            this.linkMenuPropertyOrAttribute = new LayoutManager.CommonUI.Controls.LinkMenu();
            this.label1 = new Label();
            this.SuspendLayout();
            // 
            // comboBoxTag
            // 
            this.comboBoxTag.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right);
            this.comboBoxTag.Location = new System.Drawing.Point(70, 35);
            this.comboBoxTag.Name = "comboBoxTag";
            this.comboBoxTag.Size = new System.Drawing.Size(100, 21);
            this.comboBoxTag.TabIndex = 6;
            this.comboBoxTag.DropDown += new EventHandler(this.ComboBoxTag_DropDown);
            // 
            // comboBoxSymbol
            // 
            this.comboBoxSymbol.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right);
            this.comboBoxSymbol.Location = new System.Drawing.Point(70, 10);
            this.comboBoxSymbol.Name = "comboBoxSymbol";
            this.comboBoxSymbol.Size = new System.Drawing.Size(100, 21);
            this.comboBoxSymbol.TabIndex = 5;
            this.comboBoxSymbol.SelectedIndexChanged += new EventHandler(this.ComboBoxSymbol_SelectedIndexChanged);
            // 
            // linkMenuPropertyOrAttribute
            // 
            this.linkMenuPropertyOrAttribute.Location = new System.Drawing.Point(15, 34);
            this.linkMenuPropertyOrAttribute.Name = "linkMenuPropertyOrAttribute";
            this.linkMenuPropertyOrAttribute.Options = new string[] {
                                                                        "property",
                                                                        "attribute"};
            this.linkMenuPropertyOrAttribute.SelectedIndex = 0;
            this.linkMenuPropertyOrAttribute.Size = new System.Drawing.Size(48, 23);
            this.linkMenuPropertyOrAttribute.TabIndex = 4;
            this.linkMenuPropertyOrAttribute.TabStop = true;
            this.linkMenuPropertyOrAttribute.Text = "property";
            this.linkMenuPropertyOrAttribute.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkMenuPropertyOrAttribute.ValueChanged += new EventHandler(this.LinkMenuPropertyOrAttribute_ValueChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 23);
            this.label1.TabIndex = 7;
            this.label1.Text = "Value of:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // OperandValueOf
            // 
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxTag);
            this.Controls.Add(this.comboBoxSymbol);
            this.Controls.Add(this.linkMenuPropertyOrAttribute);
            this.Name = "OperandValueOf";
            this.Size = new System.Drawing.Size(176, 64);
            this.AutoScaleDimensions = new SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.ResumeLayout(false);
        }
        #endregion
        private ComboBox comboBoxTag;
        private ComboBox comboBoxSymbol;
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenuPropertyOrAttribute;
        private Label label1;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
    }
}