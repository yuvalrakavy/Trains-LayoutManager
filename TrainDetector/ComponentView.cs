﻿using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Xml;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.View;

#pragma warning disable IDE0051
namespace TrainDetector {
    /// <summary>
    /// Summary description for ComponentView.
    /// </summary>
    [LayoutModule("Train Detector Component View", UserControl = false)]
    public class ComponentView : System.ComponentModel.Component, ILayoutModuleSetup {
        private ImageList imageListComponents;
        private IContainer components = null;

        #region Implementation of ILayoutModuleSetup

        #endregion

        #region Constructors

        public ComponentView() {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            InitializeComponent();
        }

        #endregion

        #region Component menu Item

        [LayoutEvent("get-component-menu-category-items", IfSender = "Category[@Name='Control']")]
        private void AddTrainDetectorItem(LayoutEvent e) {
            XmlElement categoryElement = (XmlElement)e.Sender;
            ModelComponent old = (ModelComponent)e.Info;

            if (old == null)
                categoryElement.InnerXml += "<Item Name='TrainDetectorController' Tooltip='VillaRakavy TrainDetector' />";
        }

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='TrainDetectorController']")]
        private void PaintCentralStationItem(LayoutEvent e) {
            Graphics g = (Graphics)e.Info;

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            var painter = new TrainDetectorPainter(new Size(32, 32));

            painter.Paint(g);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='TrainDetectorController']")]
        private void CreateTrainDetectorComponent(LayoutEvent e) {
            e.Info = new TrainDetectorsComponent();
        }

        #endregion

        #region Component view

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(TrainDetectorsComponent))]
        private void GetTrainDetectorDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = (LayoutGetDrawingRegionsEvent)eBase;

            if (LayoutDrawingRegionGrid.IsComponentGridVisible(e))
                e.AddRegion(new DrawingRegionTrainDetector(e.Component, e.View));

            LayoutTextInfo textProvider = new LayoutTextInfo(e.Component);

            if (textProvider.Element != null)
                e.AddRegion(new LayoutDrawingRegionText(e, textProvider));
        }

        private class DrawingRegionTrainDetector : LayoutDrawingRegionGrid {

            internal DrawingRegionTrainDetector(ModelComponent component, ILayoutView view) : base(component, view) {
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {
                TrainDetectorPainter painter = new TrainDetectorPainter(view.GridSizeInModelCoordinates);

                painter.Paint(g);
                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Component Painter

        [LayoutEvent("get-image", SenderType = typeof(TrainDetectorPainter))]
        private void GetTrainDetectorImage(LayoutEvent e) {
            e.Info = imageListComponents.Images[0];
        }

        private class TrainDetectorPainter {

            internal TrainDetectorPainter(Size _) {
            }

            internal void Paint(Graphics g) {
                Image image = (Image)EventManager.Event(new LayoutEvent("get-image", this));

                g.DrawImage(image, new Rectangle(new Point(1, 1), image.Size));
            }
        }

        #endregion

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        #pragma warning disable IDE0017
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

    }
}