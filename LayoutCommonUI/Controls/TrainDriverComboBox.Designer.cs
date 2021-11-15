namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for TrainDriverComboBox.
    /// </summary>
    partial class TrainDriverComboBox : System.Windows.Forms.UserControl {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.comboBoxDrivers = new ComboBox();
            this.buttonDriverSettings = new Button();
            this.SuspendLayout();
            // 
            // comboBoxDrivers
            // 
            this.comboBoxDrivers.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right);
            this.comboBoxDrivers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDrivers.Location = new System.Drawing.Point(0, 0);
            this.comboBoxDrivers.Name = "comboBoxDrivers";
            this.comboBoxDrivers.Size = new System.Drawing.Size(121, 21);
            this.comboBoxDrivers.TabIndex = 0;
            this.comboBoxDrivers.SelectedIndexChanged += this.ComboBoxDrivers_SelectedIndexChanged;
            // 
            // buttonDriverSettings
            // 
            this.buttonDriverSettings.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.buttonDriverSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (System.Byte)177);
            this.buttonDriverSettings.Location = new System.Drawing.Point(120, 0);
            this.buttonDriverSettings.Name = "buttonDriverSettings";
            this.buttonDriverSettings.Size = new System.Drawing.Size(24, 21);
            this.buttonDriverSettings.TabIndex = 1;
            this.buttonDriverSettings.Text = "...";
            this.buttonDriverSettings.Click += this.ButtonDriverSettings_Click;
            // 
            // TrainDriverComboBox
            // 
            this.Controls.Add(this.buttonDriverSettings);
            this.Controls.Add(this.comboBoxDrivers);
            this.Name = "TrainDriverComboBox";
            this.Size = new System.Drawing.Size(144, 24);
            this.ResumeLayout(false);
        }
        #endregion
        private ComboBox comboBoxDrivers;
        private Button buttonDriverSettings;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
    }
}
