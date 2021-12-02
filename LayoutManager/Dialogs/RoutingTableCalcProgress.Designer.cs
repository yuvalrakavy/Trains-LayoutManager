using System.ComponentModel;
using System.Windows.Forms;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for RoutingTableCalcProgress.
    /// </summary>
    partial class RoutingTableCalcProgress : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new Label();
            this.progressBar = new ProgressBar();
            this.buttonAbort = new Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(280, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Generating Routing Tables (Containing information used for finding optimal routes" +
                ")";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(8, 56);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(288, 16);
            this.progressBar.TabIndex = 1;
            // 
            // buttonAbort
            // 
            this.buttonAbort.Location = new System.Drawing.Point(120, 80);
            this.buttonAbort.Name = "buttonAbort";
            this.buttonAbort.TabIndex = 2;
            this.buttonAbort.Text = "&Abort";
            this.buttonAbort.Click += this.ButtonAbort_Click;
            // 
            // RoutingTableCalcProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 110);
            this.ControlBox = false;
            this.Controls.Add(this.buttonAbort);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "RoutingTableCalcProgress";
            this.ShowInTaskbar = false;
            this.Text = "Generating Routing Tables...";
            this.Closed += this.RoutingTableCalcProgress_Closed;
            this.ResumeLayout(false);
        }
        #endregion

        private Label label1;
        private ProgressBar progressBar;
        private Button buttonAbort;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

    }
}

