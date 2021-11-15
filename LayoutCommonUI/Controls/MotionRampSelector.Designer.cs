namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for MotionRampSelector.
    /// </summary>
    partial class MotionRampSelector : System.Windows.Forms.UserControl, IObjectHasXml {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.groupBoxRampSelector = new GroupBox();
            this.checkBoxOverrideDefault = new CheckBox();
            this.buttonOverrideDefault = new Button();
            this.labelRampDescription = new Label();
            this.groupBoxRampSelector.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxRampSelector
            // 
            this.groupBoxRampSelector.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                               this.checkBoxOverrideDefault,
                                                                                               this.buttonOverrideDefault,
                                                                                               this.labelRampDescription});
            this.groupBoxRampSelector.Location = new System.Drawing.Point(4, 0);
            this.groupBoxRampSelector.Name = "groupBoxRampSelector";
            this.groupBoxRampSelector.Size = new System.Drawing.Size(264, 41);
            this.groupBoxRampSelector.TabIndex = 0;
            this.groupBoxRampSelector.TabStop = false;
            this.groupBoxRampSelector.Text = "Acceleration Profile:";
            // 
            // checkBoxOverrideDefault
            // 
            this.checkBoxOverrideDefault.Location = new System.Drawing.Point(5, 18);
            this.checkBoxOverrideDefault.Name = "checkBoxOverrideDefault";
            this.checkBoxOverrideDefault.Size = new System.Drawing.Size(16, 16);
            this.checkBoxOverrideDefault.TabIndex = 4;
            this.checkBoxOverrideDefault.Click += this.CheckBoxOverrideDefault_Click;
            // 
            // buttonOverrideDefault
            // 
            this.buttonOverrideDefault.Location = new System.Drawing.Point(21, 16);
            this.buttonOverrideDefault.Name = "buttonOverrideDefault";
            this.buttonOverrideDefault.Size = new System.Drawing.Size(97, 20);
            this.buttonOverrideDefault.TabIndex = 3;
            this.buttonOverrideDefault.Text = "&Override Default";
            this.buttonOverrideDefault.Click += this.ButtonOverrideDefault_Click;
            // 
            // labelRampDescription
            // 
            this.labelRampDescription.Location = new System.Drawing.Point(124, 16);
            this.labelRampDescription.Name = "labelRampDescription";
            this.labelRampDescription.Size = new System.Drawing.Size(132, 16);
            this.labelRampDescription.TabIndex = 2;
            // 
            // MotionRampSelector
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.groupBoxRampSelector});
            this.Name = "MotionRampSelector";
            this.Size = new System.Drawing.Size(272, 41);
            this.groupBoxRampSelector.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion
        private GroupBox groupBoxRampSelector;
        private Label labelRampDescription;
        private Button buttonOverrideDefault;
        private CheckBox checkBoxOverrideDefault;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
    }
}

