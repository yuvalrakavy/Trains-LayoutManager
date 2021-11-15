namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Edit the parameters of MotionRampInfo
    /// </summary>
    partial class MotionRampEditor : System.Windows.Forms.UserControl {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.groupBox1 = new GroupBox();
            this.label1 = new Label();
            this.textBoxSpeedChangeLength = new TextBox();
            this.radioButtonSpeedChangeLength = new RadioButton();
            this.radioButtonSpeedChangeRate = new RadioButton();
            this.textBoxSpeedChangeRate = new TextBox();
            this.label2 = new Label();
            this.radioButtonLocomotiveHardware = new RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                    this.radioButtonLocomotiveHardware,
                                                                                    this.label1,
                                                                                    this.textBoxSpeedChangeLength,
                                                                                    this.radioButtonSpeedChangeLength,
                                                                                    this.radioButtonSpeedChangeRate,
                                                                                    this.textBoxSpeedChangeRate,
                                                                                    this.label2});
            this.groupBox1.Location = new System.Drawing.Point(7, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(208, 88);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Set speed change parameter:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(128, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 18);
            this.label1.TabIndex = 2;
            this.label1.Text = "seconds";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxSpeedChangeLength
            // 
            this.textBoxSpeedChangeLength.Location = new System.Drawing.Point(74, 18);
            this.textBoxSpeedChangeLength.Name = "textBoxSpeedChangeLength";
            this.textBoxSpeedChangeLength.Size = new System.Drawing.Size(48, 20);
            this.textBoxSpeedChangeLength.TabIndex = 1;
            this.textBoxSpeedChangeLength.Text = "";
            this.textBoxSpeedChangeLength.TextChanged += this.TextBoxSpeedChangeLength_TextChanged;
            // 
            // radioButtonSpeedChangeLength
            // 
            this.radioButtonSpeedChangeLength.Location = new System.Drawing.Point(8, 18);
            this.radioButtonSpeedChangeLength.Name = "radioButtonSpeedChangeLength";
            this.radioButtonSpeedChangeLength.Size = new System.Drawing.Size(61, 20);
            this.radioButtonSpeedChangeLength.TabIndex = 0;
            this.radioButtonSpeedChangeLength.Text = "Length:";
            this.radioButtonSpeedChangeLength.CheckedChanged += this.RadioButtonSpeedChangeLength_CheckedChanged;
            // 
            // radioButtonSpeedChangeRate
            // 
            this.radioButtonSpeedChangeRate.Location = new System.Drawing.Point(8, 40);
            this.radioButtonSpeedChangeRate.Name = "radioButtonSpeedChangeRate";
            this.radioButtonSpeedChangeRate.Size = new System.Drawing.Size(61, 20);
            this.radioButtonSpeedChangeRate.TabIndex = 3;
            this.radioButtonSpeedChangeRate.Text = "Rate:";
            this.radioButtonSpeedChangeRate.CheckedChanged += this.RadioButtonSpeedChangeRate_CheckedChanged;
            // 
            // textBoxSpeedChangeRate
            // 
            this.textBoxSpeedChangeRate.Location = new System.Drawing.Point(74, 40);
            this.textBoxSpeedChangeRate.Name = "textBoxSpeedChangeRate";
            this.textBoxSpeedChangeRate.Size = new System.Drawing.Size(48, 20);
            this.textBoxSpeedChangeRate.TabIndex = 4;
            this.textBoxSpeedChangeRate.Text = "";
            this.textBoxSpeedChangeRate.TextChanged += this.TextBoxSpeedChangeRate_TextChanged;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(128, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 18);
            this.label2.TabIndex = 5;
            this.label2.Text = "steps/second";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // radioButtonLocomotiveHardware
            // 
            this.radioButtonLocomotiveHardware.Location = new System.Drawing.Point(8, 61);
            this.radioButtonLocomotiveHardware.Name = "radioButtonLocomotiveHardware";
            this.radioButtonLocomotiveHardware.Size = new System.Drawing.Size(152, 20);
            this.radioButtonLocomotiveHardware.TabIndex = 6;
            this.radioButtonLocomotiveHardware.Text = "Locomotive own settings";
            this.radioButtonLocomotiveHardware.CheckedChanged += this.RadioButtonLocomotiveHardware_CheckedChanged;
            // 
            // MotionRampEditor
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.groupBox1});
            this.Name = "MotionRampEditor";
            this.Size = new System.Drawing.Size(218, 98);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion
        private GroupBox groupBox1;
        private RadioButton radioButtonSpeedChangeLength;
        private TextBox textBoxSpeedChangeLength;
        private Label label1;
        private RadioButton radioButtonSpeedChangeRate;
        private TextBox textBoxSpeedChangeRate;
        private Label label2;
        private RadioButton radioButtonLocomotiveHardware;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;

    }
}