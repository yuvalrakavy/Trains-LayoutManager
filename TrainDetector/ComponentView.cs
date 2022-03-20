using System.Drawing;
using System.Xml;
using MethodDispatcher;
using LayoutManager;
using LayoutManager.Model;
using LayoutManager.View;

namespace TrainDetector {
    /// <summary>
    /// Summary description for ComponentView.
    /// </summary>
    [LayoutModule("Train Detector Component View", UserControl = false)]
    partial class ComponentView : System.ComponentModel.Component, ILayoutModuleSetup {

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

        [DispatchTarget]
        private void GetComponentMenuCategoryItems_Control(ModelComponent? existingTrack, XmlElement categoryElement, [DispatchFilter] string categoryName = "Control") {
            if (existingTrack == null)
                categoryElement.InnerXml += "<Item Name='TrainDetectorController' Tooltip='VillaRakavy TrainDetector' />";
        }

        [DispatchTarget]
        private void PaintImageMenuItem_TrainDetectorController(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "TrainDetectorController") {
            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            var painter = new TrainDetectorPainter(new Size(32, 32));

            painter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_TrainDetectorController(XmlElement itemElement, [DispatchFilter] string name = "TrainDetectorController") => new TrainDetectorsComponent();

        #endregion

        #region Component view

        [DispatchTarget]
        void GetModelComponentDrawingRegions_TrainDetectorts([DispatchFilter] TrainDetectorsComponent component, LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions))
                regions.AddRegion(new DrawingRegionTrainDetector(regions.Component, regions.View));

            LayoutTextInfo textProvider = new LayoutTextInfo(regions.Component);

            if (textProvider.OptionalElement != null)
                regions.AddRegion(new LayoutDrawingRegionText(regions, textProvider));
        }

        private class DrawingRegionTrainDetector : LayoutDrawingRegionGrid {

            internal DrawingRegionTrainDetector(ModelComponent component, ILayoutView view) : base(component, view) {
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                TrainDetectorPainter painter = new TrainDetectorPainter(view.GridSizeInModelCoordinates);

                painter.Paint(g);
                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Component Painter

        [DispatchTarget]
        private Image GetImage([DispatchFilter] TrainDetectorPainter requestor) {
            return imageListComponents.Images[0];
        }

        private class TrainDetectorPainter {

            internal TrainDetectorPainter(Size _) {
            }

            internal void Paint(Graphics g) {
                var image = Dispatch.Call.GetImage(this);

                g.DrawImage(image, new Rectangle(new Point(1, 1), image.Size));
            }
        }

        #endregion

    }
}
