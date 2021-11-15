namespace LayoutManager.CommonUI.Dialogs {
    /// <summary>
    /// Summary description for AttributeDefinition.
    /// </summary>
    partial class AttributeDefinition : Form {
        private readonly System.ComponentModel.Container components = null;

        private Label label1;
        private GroupBox groupBox1;
        private Label label2;
        private ComboBox comboBoxName;
        private RadioButton radioButtonTypeString;
        private RadioButton radioButtonTypeNumber;
        private RadioButton radioButtonTypeBoolean;
        private Panel panelTextValue;
        private TextBox textBoxValue;
        private GroupBox groupBoxBooleanValue;
        private RadioButton radioButtonValueTrue;
        private RadioButton radioButtonValueFalse;
        private Button buttonCancel;
        private Button buttonOK;

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new Label();
            this.comboBoxName = new ComboBox();
            this.groupBox1 = new GroupBox();
            this.radioButtonTypeString = new RadioButton();
            this.radioButtonTypeNumber = new RadioButton();
            this.radioButtonTypeBoolean = new RadioButton();
            this.panelTextValue = new Panel();
            this.textBoxValue = new TextBox();
            this.label2 = new Label();
            this.groupBoxBooleanValue = new GroupBox();
            this.radioButtonValueTrue = new RadioButton();
            this.radioButtonValueFalse = new RadioButton();
            this.buttonCancel = new Button();
            this.buttonOK = new Button();
            this.groupBox1.SuspendLayout();
            this.panelTextValue.SuspendLayout();
            this.groupBoxBooleanValue.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxName
            // 
            this.comboBoxName.Location = new System.Drawing.Point(72, 17);
            this.comboBoxName.Name = "comboBoxName";
            this.comboBoxName.Size = new System.Drawing.Size(160, 21);
            this.comboBoxName.Sorted = true;
            this.comboBoxName.TabIndex = 1;
            this.comboBoxName.DropDown += this.ComboBoxName_DropDown;
            this.comboBoxName.SelectionChangeCommitted += this.ComboBoxName_SelectionChangeCommitted;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                    this.radioButtonTypeString,
                                                                                    this.radioButtonTypeNumber,
                                                                                    this.radioButtonTypeBoolean});
            this.groupBox1.Location = new System.Drawing.Point(22, 56);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(160, 80);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Type:";
            // 
            // radioButtonTypeString
            // 
            this.radioButtonTypeString.Location = new System.Drawing.Point(16, 16);
            this.radioButtonTypeString.Name = "radioButtonTypeString";
            this.radioButtonTypeString.Size = new System.Drawing.Size(104, 20);
            this.radioButtonTypeString.TabIndex = 0;
            this.radioButtonTypeString.Text = "Text (string)";
            this.radioButtonTypeString.CheckedChanged += this.UpdateButtons;
            // 
            // radioButtonTypeNumber
            // 
            this.radioButtonTypeNumber.Location = new System.Drawing.Point(16, 36);
            this.radioButtonTypeNumber.Name = "radioButtonTypeNumber";
            this.radioButtonTypeNumber.Size = new System.Drawing.Size(112, 20);
            this.radioButtonTypeNumber.TabIndex = 0;
            this.radioButtonTypeNumber.Text = "Number (integer)";
            this.radioButtonTypeNumber.CheckedChanged += this.UpdateButtons;
            // 
            // radioButtonTypeBoolean
            // 
            this.radioButtonTypeBoolean.Location = new System.Drawing.Point(16, 56);
            this.radioButtonTypeBoolean.Name = "radioButtonTypeBoolean";
            this.radioButtonTypeBoolean.Size = new System.Drawing.Size(136, 20);
            this.radioButtonTypeBoolean.TabIndex = 0;
            this.radioButtonTypeBoolean.Text = "Boolean (True/False)";
            this.radioButtonTypeBoolean.CheckedChanged += this.UpdateButtons;
            // 
            // panelTextValue
            // 
            this.panelTextValue.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                         this.textBoxValue,
                                                                                         this.label2});
            this.panelTextValue.Location = new System.Drawing.Point(22, 143);
            this.panelTextValue.Name = "panelTextValue";
            this.panelTextValue.Size = new System.Drawing.Size(202, 32);
            this.panelTextValue.TabIndex = 3;
            // 
            // textBoxValue
            // 
            this.textBoxValue.Location = new System.Drawing.Point(56, 6);
            this.textBoxValue.Name = "textBoxValue";
            this.textBoxValue.Size = new System.Drawing.Size(136, 20);
            this.textBoxValue.TabIndex = 2;
            this.textBoxValue.Text = "";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "Value:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBoxBooleanValue
            // 
            this.groupBoxBooleanValue.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                               this.radioButtonValueTrue,
                                                                                               this.radioButtonValueFalse});
            this.groupBoxBooleanValue.Location = new System.Drawing.Point(22, 143);
            this.groupBoxBooleanValue.Name = "groupBoxBooleanValue";
            this.groupBoxBooleanValue.Size = new System.Drawing.Size(152, 56);
            this.groupBoxBooleanValue.TabIndex = 4;
            this.groupBoxBooleanValue.TabStop = false;
            this.groupBoxBooleanValue.Text = "Value:";
            this.groupBoxBooleanValue.Visible = false;
            // 
            // radioButtonValueTrue
            // 
            this.radioButtonValueTrue.Location = new System.Drawing.Point(8, 12);
            this.radioButtonValueTrue.Name = "radioButtonValueTrue";
            this.radioButtonValueTrue.Size = new System.Drawing.Size(64, 20);
            this.radioButtonValueTrue.TabIndex = 1;
            this.radioButtonValueTrue.Text = "True";
            // 
            // radioButtonValueFalse
            // 
            this.radioButtonValueFalse.Location = new System.Drawing.Point(8, 32);
            this.radioButtonValueFalse.Name = "radioButtonValueFalse";
            this.radioButtonValueFalse.Size = new System.Drawing.Size(64, 20);
            this.radioButtonValueFalse.TabIndex = 1;
            this.radioButtonValueFalse.Text = "False";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(232, 176);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(56, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += this.ButtonCancel_Click;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(232, 144);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(56, 23);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += this.ButtonOK_Click;
            // 
            // AttributeDefinition
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(296, 206);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.groupBoxBooleanValue,
                                                                          this.panelTextValue,
                                                                          this.groupBox1,
                                                                          this.comboBoxName,
                                                                          this.label1,
                                                                          this.buttonCancel,
                                                                          this.buttonOK});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AttributeDefinition";
            this.Text = "Attribute Definition";
            this.groupBox1.ResumeLayout(false);
            this.panelTextValue.ResumeLayout(false);
            this.groupBoxBooleanValue.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

    }
}

