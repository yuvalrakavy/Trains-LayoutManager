namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
 
    /// <summary>
    /// Summary description for IfCondition.
    /// </summary>
    partial class IfCondition : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.groupBox1 = new GroupBox();
            this.operand1 = new LayoutManager.CommonUI.Controls.Operand();
            this.groupBox2 = new GroupBox();
            this.operand2 = new LayoutManager.CommonUI.Controls.Operand();
            this.comboBoxOperation = new ComboBox();
            this.buttonOk = new Button();
            this.buttonCancel = new Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                    this.operand1});
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 128);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // operand1
            // 
            this.operand1.AllowedTypes = null;
            this.operand1.DefaultAccess = "Property";
            this.operand1.Element = null;
            this.operand1.Location = new System.Drawing.Point(8, 24);
            this.operand1.Name = "operand1";
            this.operand1.Size = new System.Drawing.Size(184, 96);
            this.operand1.Suffix = "";
            this.operand1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                    this.operand2});
            this.groupBox2.Location = new System.Drawing.Point(312, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 128);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            // 
            // operand2
            // 
            this.operand2.AllowedTypes = null;
            this.operand2.DefaultAccess = "Property";
            this.operand2.Element = null;
            this.operand2.Location = new System.Drawing.Point(8, 24);
            this.operand2.Name = "operand2";
            this.operand2.Size = new System.Drawing.Size(184, 96);
            this.operand2.Suffix = "";
            this.operand2.TabIndex = 0;
            // 
            // comboBoxOperation
            // 
            this.comboBoxOperation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxOperation.Location = new System.Drawing.Point(216, 64);
            this.comboBoxOperation.Name = "comboBoxOperation";
            this.comboBoxOperation.Size = new System.Drawing.Size(88, 21);
            this.comboBoxOperation.TabIndex = 1;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(352, 144);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += this.ButtonOk_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(437, 144);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += this.ButtonCancel_Click;
            // 
            // IfCondition
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(520, 174);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonOk,
                                                                          this.comboBoxOperation,
                                                                          this.groupBox1,
                                                                          this.groupBox2,
                                                                          this.buttonCancel});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "IfCondition";
            this.ShowInTaskbar = false;
            this.Text = "If Condition";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private GroupBox groupBox1;
        private LayoutManager.CommonUI.Controls.Operand operand1;
        private GroupBox groupBox2;
        private LayoutManager.CommonUI.Controls.Operand operand2;
        private ComboBox comboBoxOperation;
        private Button buttonOk;
        private Button buttonCancel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
    }
}