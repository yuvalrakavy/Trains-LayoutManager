namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for GenerateEventArgument.
    /// </summary>
    partial class GenerateEventArgument : System.Windows.Forms.UserControl {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.radioButtonNull = new RadioButton();
            this.radioButtonObjectReference = new RadioButton();
            this.comboBoxReferencedObject = new ComboBox();
            this.operandValueOf = new LayoutManager.CommonUI.Controls.OperandValueOf();
            this.radioButtonValueOf = new RadioButton();
            this.radioButtonConstant = new RadioButton();
            this.linkMenuConstantType = new LayoutManager.CommonUI.Controls.LinkMenu();
            this.textBoxConstantValue = new TextBox();
            this.radioButtonContext = new RadioButton();
            this.comboBoxBooleanConstant = new ComboBox();
            this.SuspendLayout();
            // 
            // radioButtonNull
            // 
            this.radioButtonNull.Location = new System.Drawing.Point(7, 1);
            this.radioButtonNull.Name = "radioButtonNull";
            this.radioButtonNull.TabIndex = 0;
            this.radioButtonNull.Text = "Nothing (null)";
            // 
            // radioButtonObjectReference
            // 
            this.radioButtonObjectReference.Location = new System.Drawing.Point(7, 27);
            this.radioButtonObjectReference.Name = "radioButtonObjectReference";
            this.radioButtonObjectReference.Size = new System.Drawing.Size(128, 24);
            this.radioButtonObjectReference.TabIndex = 1;
            this.radioButtonObjectReference.Text = "Reference to object: ";
            // 
            // comboBoxReferencedObject
            // 
            this.comboBoxReferencedObject.Location = new System.Drawing.Point(135, 29);
            this.comboBoxReferencedObject.Name = "comboBoxReferencedObject";
            this.comboBoxReferencedObject.Size = new System.Drawing.Size(121, 21);
            this.comboBoxReferencedObject.Sorted = true;
            this.comboBoxReferencedObject.TabIndex = 2;
            this.comboBoxReferencedObject.TextChanged += this.ComboBoxReferencedObject_TextChanged;
            this.comboBoxReferencedObject.SelectedIndexChanged += this.ComboBoxReferencedObject_SelectedIndexChanged;
            // 
            // operandValueOf
            // 
            this.operandValueOf.AllowedTypes = null;
            this.operandValueOf.DefaultAccess = "Property";
            this.operandValueOf.Element = null;
            this.operandValueOf.Location = new System.Drawing.Point(8, 45);
            this.operandValueOf.Name = "operandValueOf";
            this.operandValueOf.Size = new System.Drawing.Size(176, 64);
            this.operandValueOf.Suffix = "";
            this.operandValueOf.TabIndex = 4;
            this.operandValueOf.ValueChanged += this.OperandValueOf_ValueChanged;
            // 
            // radioButtonValueOf
            // 
            this.radioButtonValueOf.Location = new System.Drawing.Point(7, 55);
            this.radioButtonValueOf.Name = "radioButtonValueOf";
            this.radioButtonValueOf.Size = new System.Drawing.Size(16, 24);
            this.radioButtonValueOf.TabIndex = 3;
            // 
            // radioButtonConstant
            // 
            this.radioButtonConstant.Location = new System.Drawing.Point(7, 112);
            this.radioButtonConstant.Name = "radioButtonConstant";
            this.radioButtonConstant.Size = new System.Drawing.Size(16, 24);
            this.radioButtonConstant.TabIndex = 5;
            // 
            // linkMenuConstantType
            // 
            this.linkMenuConstantType.Location = new System.Drawing.Point(24, 115);
            this.linkMenuConstantType.Name = "linkMenuConstantType";
            this.linkMenuConstantType.Options = new string[] {
                                                                 "String",
                                                                 "Integer",
                                                                 "Double",
                                                                 "Boolean"};
            this.linkMenuConstantType.SelectedIndex = -1;
            this.linkMenuConstantType.Size = new System.Drawing.Size(48, 17);
            this.linkMenuConstantType.TabIndex = 6;
            this.linkMenuConstantType.TabStop = true;
            this.linkMenuConstantType.Text = "String";
            this.linkMenuConstantType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkMenuConstantType.ValueChanged += this.LinkMenuConstantType_ValueChanged;
            // 
            // textBoxConstantValue
            // 
            this.textBoxConstantValue.Location = new System.Drawing.Point(78, 113);
            this.textBoxConstantValue.Name = "textBoxConstantValue";
            this.textBoxConstantValue.Size = new System.Drawing.Size(178, 20);
            this.textBoxConstantValue.TabIndex = 7;
            this.textBoxConstantValue.Text = "";
            this.textBoxConstantValue.TextChanged += this.TextBoxConstantValue_TextChanged;
            // 
            // radioButtonContext
            // 
            this.radioButtonContext.Location = new System.Drawing.Point(7, 137);
            this.radioButtonContext.Name = "radioButtonContext";
            this.radioButtonContext.Size = new System.Drawing.Size(177, 24);
            this.radioButtonContext.TabIndex = 8;
            this.radioButtonContext.Text = "Reference to current context";
            // 
            // comboBoxBooleanConstant
            // 
            this.comboBoxBooleanConstant.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBooleanConstant.Items.AddRange(new object[] {
                                                                         "False",
                                                                         "True"});
            this.comboBoxBooleanConstant.Location = new System.Drawing.Point(176, 80);
            this.comboBoxBooleanConstant.Name = "comboBoxBooleanConstant";
            this.comboBoxBooleanConstant.Size = new System.Drawing.Size(121, 21);
            this.comboBoxBooleanConstant.TabIndex = 9;
            // 
            // GenerateEventArgument
            // 
            this.Controls.Add(this.comboBoxBooleanConstant);
            this.Controls.Add(this.radioButtonContext);
            this.Controls.Add(this.textBoxConstantValue);
            this.Controls.Add(this.linkMenuConstantType);
            this.Controls.Add(this.radioButtonConstant);
            this.Controls.Add(this.radioButtonValueOf);
            this.Controls.Add(this.comboBoxReferencedObject);
            this.Controls.Add(this.radioButtonObjectReference);
            this.Controls.Add(this.radioButtonNull);
            this.Controls.Add(this.operandValueOf);
            this.Name = "GenerateEventArgument";
            this.Size = new System.Drawing.Size(264, 160);
            this.ResumeLayout(false);
        }

        #endregion
        private RadioButton radioButtonNull;
        private RadioButton radioButtonObjectReference;
        private ComboBox comboBoxReferencedObject;
        private RadioButton radioButtonValueOf;
        private RadioButton radioButtonConstant;
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenuConstantType;
        private TextBox textBoxConstantValue;
        private RadioButton radioButtonContext;
        private LayoutManager.CommonUI.Controls.OperandValueOf operandValueOf;
        private ComboBox comboBoxBooleanConstant;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
    }
}
