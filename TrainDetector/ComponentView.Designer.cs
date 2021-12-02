using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Xml;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.View;

namespace TrainDetector {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ComponentView));
            this.imageListComponents = new System.Windows.Forms.ImageList(this.components);
            // 
            // imageListComponents
            // 
            this.imageListComponents.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListComponents.ImageStream")));
            this.imageListComponents.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListComponents.Images.SetKeyName(0, "TrainDetectorController.png");

        }
        #endregion

        private ImageList imageListComponents;
        private IContainer components = null;
    }
}

