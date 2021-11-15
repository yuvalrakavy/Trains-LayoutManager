namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for IfExist.
    /// </summary>
    partial class IfDefined : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.groupBox1 = new GroupBox();
            this.comboBoxSymbol = new ComboBox();
            this.label1 = new Label();
            this.checkBoxAttribute = new CheckBox();
            this.comboBoxAttribute = new ComboBox();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxAttribute);
            this.groupBox1.Controls.Add(this.checkBoxAttribute);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comboBoxSymbol);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(264, 80);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Check if the current context has definition for:";
            // 
            // comboBoxSymbol
            // 
            this.comboBoxSymbol.Location = new System.Drawing.Point(66, 18);
            this.comboBoxSymbol.Name = "comboBoxSymbol";
            this.comboBoxSymbol.Size = new System.Drawing.Size(118, 21);
            this.comboBoxSymbol.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Symbol:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkBoxAttribute
            // 
            this.checkBoxAttribute.Location = new System.Drawing.Point(18, 48);
            this.checkBoxAttribute.Name = "checkBoxAttribute";
            this.checkBoxAttribute.Size = new System.Drawing.Size(72, 16);
            this.checkBoxAttribute.TabIndex = 2;
            this.checkBoxAttribute.Text = "Attribute:";
            this.checkBoxAttribute.CheckedChanged += this.CheckBoxAttribute_CheckedChanged;
            // 
            // comboBoxAttribute
            // 
            this.comboBoxAttribute.Location = new System.Drawing.Point(98, 46);
            this.comboBoxAttribute.Name = "comboBoxAttribute";
            this.comboBoxAttribute.Size = new System.Drawing.Size(118, 21);
            this.comboBoxAttribute.TabIndex = 3;
            this.comboBoxAttribute.DropDown += this.ComboBoxAttribute_DropDown;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(118, 96);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += this.ButtonOK_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(198, 96);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            // 
            // IfDefined
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(280, 126);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "IfDefined";
            this.ShowInTaskbar = false;
            this.Text = "If (Defined)";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion
        private GroupBox groupBox1;
        private ComboBox comboBoxSymbol;
        private Label label1;
        private CheckBox checkBoxAttribute;
        private ComboBox comboBoxAttribute;
        private Button buttonOK;
        private Button buttonCancel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
    }
}
