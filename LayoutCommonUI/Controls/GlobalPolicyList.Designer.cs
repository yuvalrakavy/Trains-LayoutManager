namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for GlobalPolicyList.
    /// </summary>
    partial class GlobalPolicyList : PolicyList {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.buttonStartStop = new Button();
            this.SuspendLayout();
            // 
            // buttonStartStop
            // 
            this.buttonStartStop.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonStartStop.Location = new System.Drawing.Point(248, 128);
            this.buttonStartStop.Name = "buttonStartStop";
            this.buttonStartStop.Size = new System.Drawing.Size(72, 23);
            this.buttonStartStop.TabIndex = 4;
            this.buttonStartStop.Text = "&Deactivate";
            this.buttonStartStop.Click += this.ButtonStartStop_Click;
            // 
            // GlobalPolicyList
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonStartStop});
            this.Name = "GlobalPolicyList";
            this.ResumeLayout(false);
        }
        #endregion
        private Button buttonStartStop;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
    }
}

