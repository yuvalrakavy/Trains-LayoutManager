using System;
using System.ComponentModel;
using System.Xml;
using System.Drawing;
using LayoutManager.Components;
using LayoutManager.Model;

namespace LayoutManager.Tools {
    /// <summary>
    /// Summary description for ComponentMenuItems.
    /// </summary>
    partial class ComponentMenuItems : System.ComponentModel.Component, ILayoutModuleSetup {
        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ComponentMenuItems));
            this.imageListCategories = new System.Windows.Forms.ImageList(this.components);
            this.imageListComponents = new System.Windows.Forms.ImageList(this.components);
            this.imageListComponentsLarge = new System.Windows.Forms.ImageList(this.components);
            // 
            // imageListCategories
            // 
            this.imageListCategories.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListCategories.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListCategories.ImageStream");
            this.imageListCategories.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListCategories.Images.SetKeyName(0, "BitMapTrackCategory.bmp");
            this.imageListCategories.Images.SetKeyName(1, "");
            this.imageListCategories.Images.SetKeyName(2, "");
            this.imageListCategories.Images.SetKeyName(3, "");
            this.imageListCategories.Images.SetKeyName(4, "BitmapComposedTracksCategory.bmp");
            // 
            // imageListComponents
            // 
            this.imageListComponents.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListComponents.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListComponents.ImageStream");
            this.imageListComponents.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListComponents.Images.SetKeyName(0, "");
            this.imageListComponents.Images.SetKeyName(1, "");
            // 
            // imageListComponentsLarge
            // 
            this.imageListComponentsLarge.ImageSize = new System.Drawing.Size(32, 32);
            this.imageListComponentsLarge.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListComponentsLarge.ImageStream");
            this.imageListComponentsLarge.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListComponentsLarge.Images.SetKeyName(0, "");
        }
        #endregion

        private System.Windows.Forms.ImageList imageListCategories;
        private System.Windows.Forms.ImageList imageListComponents;
        private System.Windows.Forms.ImageList imageListComponentsLarge;
        private IContainer components;
    }
}
