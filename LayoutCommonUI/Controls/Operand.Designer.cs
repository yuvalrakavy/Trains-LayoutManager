namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for Operand.
    /// </summary>
    partial class Operand : System.Windows.Forms.UserControl, IObjectHasXml {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.radioButtonValue = new RadioButton();
            this.textBoxValue = new TextBox();
            this.linkMenu1Boolean = new LayoutManager.CommonUI.Controls.LinkMenu();
            this.radioButtonPropertyOrAttribute = new RadioButton();
            this.operandValueOf = new LayoutManager.CommonUI.Controls.OperandValueOf();
            this.SuspendLayout();
            // 
            // radioButtonValue
            // 
            this.radioButtonValue.Location = new System.Drawing.Point(8, 8);
            this.radioButtonValue.Name = "radioButtonValue";
            this.radioButtonValue.Size = new System.Drawing.Size(56, 24);
            this.radioButtonValue.TabIndex = 0;
            this.radioButtonValue.Text = "Value:";
            this.radioButtonValue.CheckedChanged += new EventHandler(this.RadioButtonValue_CheckedChanged);
            // 
            // textBoxValue
            // 
            this.textBoxValue.Location = new System.Drawing.Point(80, 10);
            this.textBoxValue.Name = "textBoxValue";
            this.textBoxValue.Size = new System.Drawing.Size(80, 20);
            this.textBoxValue.TabIndex = 4;
            this.textBoxValue.Text = "";
            this.textBoxValue.TextChanged += new EventHandler(this.TextBoxValue_TextChanged);
            // 
            // linkMenu1Boolean
            // 
            this.linkMenu1Boolean.Location = new System.Drawing.Point(112, 9);
            this.linkMenu1Boolean.Name = "linkMenu1Boolean";
            this.linkMenu1Boolean.Options = new string[] {
                                                             "True",
                                                             "False"};
            this.linkMenu1Boolean.SelectedIndex = 0;
            this.linkMenu1Boolean.Size = new System.Drawing.Size(80, 24);
            this.linkMenu1Boolean.TabIndex = 5;
            this.linkMenu1Boolean.TabStop = true;
            this.linkMenu1Boolean.Text = "True";
            this.linkMenu1Boolean.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkMenu1Boolean.Visible = false;
            // 
            // radioButtonPropertyOrAttribute
            // 
            this.radioButtonPropertyOrAttribute.Location = new System.Drawing.Point(8, 34);
            this.radioButtonPropertyOrAttribute.Name = "radioButtonPropertyOrAttribute";
            this.radioButtonPropertyOrAttribute.Size = new System.Drawing.Size(16, 24);
            this.radioButtonPropertyOrAttribute.TabIndex = 6;
            // 
            // operandValueOf
            // 
            this.operandValueOf.AllowedTypes = null;
            this.operandValueOf.DefaultAccess = "Property";
            this.operandValueOf.Element = null;
            this.operandValueOf.Location = new System.Drawing.Point(9, 24);
            this.operandValueOf.Name = "operandValueOf";
            this.operandValueOf.Size = new System.Drawing.Size(176, 64);
            this.operandValueOf.Suffix = "";
            this.operandValueOf.TabIndex = 7;
            this.operandValueOf.ValueChanged += new EventHandler(this.OperandValueOf_ValueChanged);
            // 
            // Operand
            // 
            this.Controls.Add(this.radioButtonPropertyOrAttribute);
            this.Controls.Add(this.linkMenu1Boolean);
            this.Controls.Add(this.radioButtonValue);
            this.Controls.Add(this.textBoxValue);
            this.Controls.Add(this.operandValueOf);
            this.Name = "Operand";
            this.Size = new System.Drawing.Size(200, 96);
            this.AutoSize = true;
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.ResumeLayout(false);
        }
        #endregion
        private RadioButton radioButtonValue;
        private TextBox textBoxValue;
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenu1Boolean;
        private RadioButton radioButtonPropertyOrAttribute;
        private LayoutManager.CommonUI.Controls.OperandValueOf operandValueOf;
    }
}

