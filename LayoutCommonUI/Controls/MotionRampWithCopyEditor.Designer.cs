
namespace LayoutManager.CommonUI.Controls {

    partial class MotionRampWithCopyEditor {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.buttonUsrExistingRamp = new Button();
            this.SuspendLayout();
            // 
            // buttonUsrExistingRamp
            // 
            this.buttonUsrExistingRamp.Location = new System.Drawing.Point(8, 99);
            this.buttonUsrExistingRamp.Name = "buttonUsrExistingRamp";
            this.buttonUsrExistingRamp.Size = new System.Drawing.Size(112, 22);
            this.buttonUsrExistingRamp.TabIndex = 1;
            this.buttonUsrExistingRamp.Text = "Use existing profile";
            this.buttonUsrExistingRamp.Click += this.ButtonUsrExistingRamp_Click;
            // 
            // MotionRampWithCopyEditor
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonUsrExistingRamp});
            this.Name = "MotionRampWithCopyEditor";
            this.Size = new System.Drawing.Size(232, 128);
            this.ResumeLayout(false);
        }
        #endregion

        private Button buttonUsrExistingRamp;
    }
}