namespace LayoutManager.View {
    partial class LayoutView {
        private System.ComponentModel.IContainer components;


        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            vScrollBar.Dispose();
            hScrollBar.Dispose();
            timerScrollForDrop.Dispose();

            if (disposing && components!= null) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.hScrollBar = new System.Windows.Forms.HScrollBar();
            this.timerScrollForDrop = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.Location = new System.Drawing.Point(134, 0);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(16, 134);
            this.vScrollBar.TabIndex = 1;
            this.vScrollBar.Scroll += this.VScrollBar_Scroll;
            // 
            // hScrollBar
            // 
            this.hScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hScrollBar.Location = new System.Drawing.Point(0, 134);
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.Size = new System.Drawing.Size(150, 16);
            this.hScrollBar.TabIndex = 0;
            this.hScrollBar.Scroll += this.HScrollBar_Scroll;
            // 
            // timerScrollForDrop
            // 
            this.timerScrollForDrop.Interval = 250;
            this.timerScrollForDrop.Tick += this.TimerScrollForDrop_Tick;
            // 
            // LayoutView
            // 
            this.AllowDrop = true;
            this.Controls.Add(this.vScrollBar);
            this.Controls.Add(this.hScrollBar);
            this.Name = E_LayoutView;
            this.Resize += this.LayoutView_Resize;
            this.MouseEnter += this.LayoutView_MouseEnter;
            this.KeyDown += this.LayoutView_KeyDown;
            this.ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.HScrollBar hScrollBar;
        private System.Windows.Forms.VScrollBar vScrollBar;
        private System.Windows.Forms.Timer timerScrollForDrop;
    }

}
