namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for SetAttribute.
    /// </summary>
    partial class SetAttribute : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.groupBox1 = new GroupBox();
            this.label1 = new Label();
            this.comboBoxSymbol = new ComboBox();
            this.label2 = new Label();
            this.comboBoxAttribute = new ComboBox();
            this.groupBox2 = new GroupBox();
            this.operandValueOf = new LayoutManager.CommonUI.Controls.OperandValueOf();
            this.groupBoxValueBoolean = new GroupBox();
            this.radioButtonValueTrue = new RadioButton();
            this.radioButtonValueFalse = new RadioButton();
            this.textBoxTextValue = new TextBox();
            this.linkMenuType = new LayoutManager.CommonUI.Controls.LinkMenu();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.numericValue = new LayoutManager.CommonUI.Controls.NumericValue();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxValueBoolean.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comboBoxSymbol);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.comboBoxAttribute);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(216, 144);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Set:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(15, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Symbol:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxSymbol
            // 
            this.comboBoxSymbol.Location = new System.Drawing.Point(71, 24);
            this.comboBoxSymbol.Name = "comboBoxSymbol";
            this.comboBoxSymbol.Size = new System.Drawing.Size(139, 21);
            this.comboBoxSymbol.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(7, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "Attribute:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxAttribute
            // 
            this.comboBoxAttribute.Location = new System.Drawing.Point(71, 56);
            this.comboBoxAttribute.Name = "comboBoxAttribute";
            this.comboBoxAttribute.Size = new System.Drawing.Size(139, 21);
            this.comboBoxAttribute.TabIndex = 0;
            this.comboBoxAttribute.DropDown += new EventHandler(this.ComboBoxAttribute_DropDown);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numericValue);
            this.groupBox2.Controls.Add(this.operandValueOf);
            this.groupBox2.Controls.Add(this.groupBoxValueBoolean);
            this.groupBox2.Controls.Add(this.textBoxTextValue);
            this.groupBox2.Controls.Add(this.linkMenuType);
            this.groupBox2.Location = new System.Drawing.Point(231, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(224, 144);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "To:";
            // 
            // operandValueOf
            // 
            this.operandValueOf.AllowedTypes = null;
            this.operandValueOf.DefaultAccess = "Property";
            this.operandValueOf.OptionalElement = null;
            this.operandValueOf.Location = new System.Drawing.Point(24, 36);
            this.operandValueOf.Name = "operandValueOf";
            this.operandValueOf.Size = new System.Drawing.Size(200, 72);
            this.operandValueOf.Suffix = "";
            this.operandValueOf.TabIndex = 4;
            // 
            // groupBoxValueBoolean
            // 
            this.groupBoxValueBoolean.Controls.Add(this.radioButtonValueTrue);
            this.groupBoxValueBoolean.Controls.Add(this.radioButtonValueFalse);
            this.groupBoxValueBoolean.Location = new System.Drawing.Point(8, 40);
            this.groupBoxValueBoolean.Name = "groupBoxValueBoolean";
            this.groupBoxValueBoolean.Size = new System.Drawing.Size(144, 72);
            this.groupBoxValueBoolean.TabIndex = 2;
            this.groupBoxValueBoolean.TabStop = false;
            // 
            // radioButtonValueTrue
            // 
            this.radioButtonValueTrue.Location = new System.Drawing.Point(8, 16);
            this.radioButtonValueTrue.Name = "radioButtonValueTrue";
            this.radioButtonValueTrue.TabIndex = 0;
            this.radioButtonValueTrue.Text = "True";
            // 
            // radioButtonValueFalse
            // 
            this.radioButtonValueFalse.Location = new System.Drawing.Point(8, 40);
            this.radioButtonValueFalse.Name = "radioButtonValueFalse";
            this.radioButtonValueFalse.TabIndex = 0;
            this.radioButtonValueFalse.Text = "False";
            // 
            // textBoxTextValue
            // 
            this.textBoxTextValue.Location = new System.Drawing.Point(8, 48);
            this.textBoxTextValue.Name = "textBoxTextValue";
            this.textBoxTextValue.Size = new System.Drawing.Size(152, 20);
            this.textBoxTextValue.TabIndex = 1;
            this.textBoxTextValue.Text = "";
            // 
            // linkMenuType
            // 
            this.linkMenuType.Location = new System.Drawing.Point(8, 24);
            this.linkMenuType.Name = "linkMenuType";
            this.linkMenuType.Options = new string[] {
                                                         "Text",
                                                         "Number",
                                                         "Boolean",
                                                         "Value of",
                                                         "Remove attribute"};
            this.linkMenuType.SelectedIndex = -1;
            this.linkMenuType.Size = new System.Drawing.Size(100, 16);
            this.linkMenuType.TabIndex = 0;
            this.linkMenuType.TabStop = true;
            this.linkMenuType.Text = "Text";
            this.linkMenuType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkMenuType.ValueChanged += new EventHandler(this.LinkMenuType_ValueChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(304, 160);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(384, 160);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            // 
            // numericValue
            // 
            this.numericValue.Element = null;
            this.numericValue.Location = new System.Drawing.Point(24, 96);
            this.numericValue.Name = "numericValue";
            this.numericValue.Size = new System.Drawing.Size(200, 27);
            this.numericValue.TabIndex = 5;
            // 
            // SetAttribute
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(464, 190);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SetAttribute";
            this.ShowInTaskbar = false;
            this.Text = "Set Attribute";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBoxValueBoolean.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion
        private GroupBox groupBox1;
        private ComboBox comboBoxSymbol;
        private Label label1;
        private Label label2;
        private ComboBox comboBoxAttribute;
        private GroupBox groupBox2;
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenuType;
        private TextBox textBoxTextValue;
        private GroupBox groupBoxValueBoolean;
        private RadioButton radioButtonValueTrue;
        private RadioButton radioButtonValueFalse;
        private Button buttonOK;
        private Button buttonCancel;
        private LayoutManager.CommonUI.Controls.OperandValueOf operandValueOf;
        private LayoutManager.CommonUI.Controls.NumericValue numericValue;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
    }
}
