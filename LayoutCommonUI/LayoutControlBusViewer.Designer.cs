namespace LayoutManager.CommonUI {
    /// <summary>
    /// Summary description for LayoutControlBusViewer.
    /// </summary>
    partial class LayoutControlBusViewer : System.Windows.Forms.UserControl {
        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayoutControlBusViewer));
            this.hScrollBar = new System.Windows.Forms.HScrollBar();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.imageListConnectionPointTypes = new ImageList(this.components);
            this.SuspendLayout();
            // 
            // hScrollBar
            // 
            this.hScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hScrollBar.Location = new System.Drawing.Point(117, 17);
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.Size = new System.Drawing.Size(80, 17);
            this.hScrollBar.TabIndex = 0;
            this.hScrollBar.Scroll += new ScrollEventHandler(this.HScrollBar_Scroll);
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.Location = new System.Drawing.Point(17, 17);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(17, 80);
            this.vScrollBar.TabIndex = 0;
            this.vScrollBar.Scroll += new ScrollEventHandler(this.VScrollBar_Scroll);
            // 
            // imageListConnectionPointTypes
            // 
            this.imageListConnectionPointTypes.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListConnectionPointTypes.ImageStream");
            this.imageListConnectionPointTypes.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListConnectionPointTypes.Images.SetKeyName(0, "");
            this.imageListConnectionPointTypes.Images.SetKeyName(1, "");
            this.imageListConnectionPointTypes.Images.SetKeyName(2, "");
            this.imageListConnectionPointTypes.Images.SetKeyName(3, "");
            // 
            // LayoutControlBusViewer
            // 
            this.Controls.Add(hScrollBar);
            this.Controls.Add(vScrollBar);

            this.SizeChanged += new EventHandler(this.LayoutControlBusViewer_SizeChanged);
            this.Click += new EventHandler(this.LayoutControlBusViewer_Click);
            this.DoubleClick += new EventHandler(this.LayoutControlBusViewer_DoubleClick);
            this.KeyDown += new KeyEventHandler(this.LayoutControlBusViewer_KeyDown);
            this.MouseDown += new MouseEventHandler(this.LayoutControlBusViewer_MouseDown);
            this.MouseEnter += new EventHandler(this.LayoutControlBusViewer_MouseEnter);
            this.MouseLeave += new EventHandler(this.LayoutControlBusViewer_MouseLeave);
            this.MouseMove += new MouseEventHandler(this.LayoutControlBusViewer_MouseMove);
            this.ResumeLayout(false);
        }
        #endregion
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.HScrollBar hScrollBar;
        private ImageList imageListConnectionPointTypes;
        private System.Windows.Forms.VScrollBar vScrollBar;
    }
}