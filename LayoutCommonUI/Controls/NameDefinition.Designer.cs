namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for NameDefinition.
    /// </summary>
    partial class NameDefinition : System.Windows.Forms.UserControl {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.checkBoxVisible = new CheckBox();
            this.textBoxName = new TextBox();
            this.buttonSettings = new Button();
            this.labelName = new Label();
            this.SuspendLayout();
            // 
            // checkBoxVisible
            // 
            this.checkBoxVisible.Location = new System.Drawing.Point(64, 28);
            this.checkBoxVisible.Name = "checkBoxVisible";
            this.checkBoxVisible.Size = new System.Drawing.Size(64, 24);
            this.checkBoxVisible.TabIndex = 2;
            this.checkBoxVisible.Text = "Visible";
            this.checkBoxVisible.CheckedChanged += this.CheckBoxVisible_CheckedChanged;
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.textBoxName.Location = new System.Drawing.Point(64, 4);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(208, 20);
            this.textBoxName.TabIndex = 1;
            this.textBoxName.Text = "";
            // 
            // buttonSettings
            // 
            this.buttonSettings.Location = new System.Drawing.Point(136, 28);
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.TabIndex = 3;
            this.buttonSettings.Text = "Settings...";
            this.buttonSettings.Click += this.ButtonSettings_Click;
            // 
            // labelName
            // 
            this.labelName.Location = new System.Drawing.Point(8, 6);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(48, 16);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "Name:";
            this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NameDefinition
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.checkBoxVisible,
                                                                          this.textBoxName,
                                                                          this.buttonSettings,
                                                                          this.labelName});
            this.Name = "NameDefinition";
            this.Size = new System.Drawing.Size(280, 53);
            this.ResumeLayout(false);
        }
        #endregion
        private CheckBox checkBoxVisible;
        private TextBox textBoxName;
        private Button buttonSettings;
        private Label labelName;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
    }
}

