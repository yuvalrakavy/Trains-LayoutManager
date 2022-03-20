namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for LayoutEmulationSetup.
    /// </summary>
    partial class LayoutEmulationSetup : System.Windows.Forms.UserControl {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.groupBoxEmulationServices = new System.Windows.Forms.GroupBox();
            this.checkBoxEmulateTrainMotion = new System.Windows.Forms.CheckBox();
            this.checkBoxAnimateTrainMotion = new System.Windows.Forms.CheckBox();
            this.textBoxEmulationTickTime = new System.Windows.Forms.TextBox();
            this.labelEmulationTickTime = new System.Windows.Forms.Label();
            this.groupBoxEmulationServices.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxEmulationServices
            // 
            this.groupBoxEmulationServices.Controls.Add(this.checkBoxEmulateTrainMotion);
            this.groupBoxEmulationServices.Controls.Add(this.checkBoxAnimateTrainMotion);
            this.groupBoxEmulationServices.Controls.Add(this.textBoxEmulationTickTime);
            this.groupBoxEmulationServices.Controls.Add(this.labelEmulationTickTime);
            this.groupBoxEmulationServices.Location = new System.Drawing.Point(0, 0);
            this.groupBoxEmulationServices.Name = "groupBoxEmulationServices";
            this.groupBoxEmulationServices.Size = new System.Drawing.Size(336, 87);
            this.groupBoxEmulationServices.TabIndex = 11;
            this.groupBoxEmulationServices.TabStop = false;
            this.groupBoxEmulationServices.Text = "Simulation of layout operation";
            // 
            // checkBoxEmulateTrainMotion
            // 
            this.checkBoxEmulateTrainMotion.Location = new System.Drawing.Point(27, 20);
            this.checkBoxEmulateTrainMotion.Name = "checkBoxEmulateTrainMotion";
            this.checkBoxEmulateTrainMotion.Size = new System.Drawing.Size(144, 16);
            this.checkBoxEmulateTrainMotion.TabIndex = 4;
            this.checkBoxEmulateTrainMotion.Text = "Emulate train motion";
            this.checkBoxEmulateTrainMotion.CheckedChanged += new EventHandler(this.CheckBoxEmulateTrainMotion_CheckedChanged);
            // 
            // checkBoxAnimateTrainMotion
            // 
            this.checkBoxAnimateTrainMotion.Location = new System.Drawing.Point(27, 59);
            this.checkBoxAnimateTrainMotion.Name = "checkBoxAnimateTrainMotion";
            this.checkBoxAnimateTrainMotion.Size = new System.Drawing.Size(144, 16);
            this.checkBoxAnimateTrainMotion.TabIndex = 3;
            this.checkBoxAnimateTrainMotion.Text = "Animate train motion";
            // 
            // textBoxEmulationTickTime
            // 
            this.textBoxEmulationTickTime.Location = new System.Drawing.Point(211, 37);
            this.textBoxEmulationTickTime.Name = "textBoxEmulationTickTime";
            this.textBoxEmulationTickTime.Size = new System.Drawing.Size(56, 20);
            this.textBoxEmulationTickTime.TabIndex = 2;
            // 
            // labelEmulationTickTime
            // 
            this.labelEmulationTickTime.AutoSize = true;
            this.labelEmulationTickTime.Location = new System.Drawing.Point(24, 38);
            this.labelEmulationTickTime.Name = "labelEmulationTickTime";
            this.labelEmulationTickTime.Size = new System.Drawing.Size(168, 13);
            this.labelEmulationTickTime.TabIndex = 1;
            this.labelEmulationTickTime.Text = "Simulation tick time (milliseconds): ";
            this.labelEmulationTickTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LayoutEmulationSetup
            // 
            this.Controls.Add(this.groupBoxEmulationServices);
            this.Name = "LayoutEmulationSetup";
            this.Size = new System.Drawing.Size(338, 97);
            this.groupBoxEmulationServices.ResumeLayout(false);
            this.groupBoxEmulationServices.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion
        private GroupBox groupBoxEmulationServices;
        private CheckBox checkBoxAnimateTrainMotion;
        private TextBox textBoxEmulationTickTime;
        private Label labelEmulationTickTime;
        private CheckBox checkBoxEmulateTrainMotion;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
    }
}