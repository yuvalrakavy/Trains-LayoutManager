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

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
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
                var image = Ensure.NotNull<Image>(EventManager.Event(new LayoutEvent("get-image", this)));

                g.DrawImage(image, new Rectangle(new Point(1, 1), image.Size));
            }
        }

        #endregion

    }
}
