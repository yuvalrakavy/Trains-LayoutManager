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
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxName = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonTypeString = new System.Windows.Forms.RadioButton();
            this.radioButtonTypeNumber = new System.Windows.Forms.RadioButton();
            this.radioButtonTypeBoolean = new System.Windows.Forms.RadioButton();
            this.panelTextValue = new System.Windows.Forms.Panel();
            this.textBoxValue = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBoxBooleanValue = new System.Windows.Forms.GroupBox();
            this.radioButtonValueTrue = new System.Windows.Forms.RadioButton();
            this.radioButtonValueFalse = new System.Windows.Forms.RadioButton();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.panelTextValue.SuspendLayout();
            this.groupBoxBooleanValue.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(42, 39);
            this.label1.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 57);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxName
            // 
            this.comboBoxName.Location = new System.Drawing.Point(187, 42);
            this.comboBoxName.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.comboBoxName.Name = "comboBoxName";
            this.comboBoxName.Size = new System.Drawing.Size(410, 40);
            this.comboBoxName.Sorted = true;
            this.comboBoxName.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonTypeString);
            this.groupBox1.Controls.Add(this.radioButtonTypeNumber);
            this.groupBox1.Controls.Add(this.radioButtonTypeBoolean);
            this.groupBox1.Location = new System.Drawing.Point(57, 138);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox1.Size = new System.Drawing.Size(416, 197);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Type:";
            // 
            // radioButtonTypeString
            // 
            this.radioButtonTypeString.Location = new System.Drawing.Point(42, 39);
            this.radioButtonTypeString.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonTypeString.Name = "radioButtonTypeString";
            this.radioButtonTypeString.Size = new System.Drawing.Size(270, 49);
            this.radioButtonTypeString.TabIndex = 0;
            this.radioButtonTypeString.Text = "Text (string)";
            // 
            // radioButtonTypeNumber
            // 
            this.radioButtonTypeNumber.Location = new System.Drawing.Point(42, 89);
            this.radioButtonTypeNumber.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonTypeNumber.Name = "radioButtonTypeNumber";
            this.radioButtonTypeNumber.Size = new System.Drawing.Size(291, 49);
            this.radioButtonTypeNumber.TabIndex = 0;
            this.radioButtonTypeNumber.Text = "Number (integer)";
            // 
            // radioButtonTypeBoolean
            // 
            this.radioButtonTypeBoolean.Location = new System.Drawing.Point(42, 138);
            this.radioButtonTypeBoolean.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonTypeBoolean.Name = "radioButtonTypeBoolean";
            this.radioButtonTypeBoolean.Size = new System.Drawing.Size(354, 49);
            this.radioButtonTypeBoolean.TabIndex = 0;
            this.radioButtonTypeBoolean.Text = "Boolean (True/False)";
            // 
            // panelTextValue
            // 
            this.panelTextValue.Controls.Add(this.textBoxValue);
            this.panelTextValue.Controls.Add(this.label2);
            this.panelTextValue.Location = new System.Drawing.Point(57, 352);
            this.panelTextValue.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.panelTextValue.Name = "panelTextValue";
            this.panelTextValue.Size = new System.Drawing.Size(525, 79);
            this.panelTextValue.TabIndex = 3;
            // 
            // textBoxValue
            // 
            this.textBoxValue.Location = new System.Drawing.Point(146, 15);
            this.textBoxValue.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.textBoxValue.Name = "textBoxValue";
            this.textBoxValue.Size = new System.Drawing.Size(347, 39);
            this.textBoxValue.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(21, 12);
            this.label2.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 57);
            this.label2.TabIndex = 1;
            this.label2.Text = "Value:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBoxBooleanValue
            // 
            this.groupBoxBooleanValue.Controls.Add(this.radioButtonValueTrue);
            this.groupBoxBooleanValue.Controls.Add(this.radioButtonValueFalse);
            this.groupBoxBooleanValue.Location = new System.Drawing.Point(57, 352);
            this.groupBoxBooleanValue.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBoxBooleanValue.Name = "groupBoxBooleanValue";
            this.groupBoxBooleanValue.Padding = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBoxBooleanValue.Size = new System.Drawing.Size(395, 138);
            this.groupBoxBooleanValue.TabIndex = 4;
            this.groupBoxBooleanValue.TabStop = false;
            this.groupBoxBooleanValue.Text = "Value:";
            this.groupBoxBooleanValue.Visible = false;
            // 
            // radioButtonValueTrue
            // 
            this.radioButtonValueTrue.Location = new System.Drawing.Point(21, 30);
            this.radioButtonValueTrue.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonValueTrue.Name = "radioButtonValueTrue";
            this.radioButtonValueTrue.Size = new System.Drawing.Size(166, 49);
            this.radioButtonValueTrue.TabIndex = 1;
            this.radioButtonValueTrue.Text = "True";
            // 
            // radioButtonValueFalse
            // 
            this.radioButtonValueFalse.Location = new System.Drawing.Point(21, 79);
            this.radioButtonValueFalse.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonValueFalse.Name = "radioButtonValueFalse";
            this.radioButtonValueFalse.Size = new System.Drawing.Size(166, 49);
            this.radioButtonValueFalse.TabIndex = 1;
            this.radioButtonValueFalse.Text = "False";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(603, 433);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(146, 57);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(603, 354);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(146, 57);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // AttributeDefinition
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(770, 507);
            this.ControlBox = false;
            this.Controls.Add(this.groupBoxBooleanValue);
            this.Controls.Add(this.panelTextValue);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.comboBoxName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "AttributeDefinition";
            this.Text = "Attribute Definition";
            this.groupBox1.ResumeLayout(false);
            this.panelTextValue.ResumeLayout(false);
            this.panelTextValue.PerformLayout();
            this.groupBoxBooleanValue.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

    }
}

