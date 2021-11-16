using System.ComponentModel;
using System.Drawing;
using System.Xml;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.View;

namespace MarklinDigital {
    /// <summary>
    /// Summary description for ComponentView.
    /// </summary>
    partial class ComponentView : System.ComponentModel.Component, ILayoutModuleSetup {
        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ComponentView));
            this.imageListComponents = new System.Windows.Forms.ImageList(this.components) {
                // 
                // imageListComponents
                // 
                ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit,
                ImageSize = new System.Drawing.Size(30, 30),
                ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListComponents.ImageStream"),
                TransparentColor = System.Drawing.Color.Transparent
            };
        }
        #endregion

        private System.Windows.Forms.ImageList imageListComponents;
        private IContainer components;
    }
}

