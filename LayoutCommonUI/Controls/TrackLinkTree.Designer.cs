namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for TrackLinkTree.
    /// </summary>
    partial class TrackLinkTree : TreeView {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrackLinkTree));
            this.imageListTrackLinks = new ImageList(this.components) {
                // 
                // imageListTrackLinks
                // 
                ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit,
                ImageSize = new System.Drawing.Size(16, 16),
                ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListTrackLinks.ImageStream"),
                TransparentColor = System.Drawing.Color.Transparent
            };
            // 
            // TrackLinkTree
            // 
            this.ImageIndex = 0;
            this.ImageList = this.imageListTrackLinks;
            this.SelectedImageIndex = 0;
        }
        #endregion
        private System.ComponentModel.IContainer components;
        private ImageList imageListTrackLinks;
    }
}
