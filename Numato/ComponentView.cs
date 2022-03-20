using System.Drawing;
using System.Xml;
using MethodDispatcher;
using LayoutManager;
using LayoutManager.Model;
using LayoutManager.View;

namespace NumatoController {
    /// <summary>
    /// Summary description for ComponentView.
    /// </summary>
    [LayoutModule("Numato Relay controller Component View", UserControl = false)]
    public partial class ComponentView : System.ComponentModel.Component, ILayoutModuleSetup {
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
                categoryElement.InnerXml += "<Item Name='NumatoRelayController' Tooltip='Numato Labs Relay Controller' />";
        }

        [DispatchTarget]
        private void PaintImageMenuItem_NumatoRelayController(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "NumatoRelayController") {
            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            var painter = new NumatoRelayControllerPainter(new Size(32, 32));

            painter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_NumatoRelayController(XmlElement itemElement, [DispatchFilter] string name = "NumatoRelayController") => new NumatoController();

        #endregion

        #region Component view

        [DispatchTarget]
        void GetModelComponentDrawingRegions_Numatol([DispatchFilter] NumatoController component, LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions))
                regions.AddRegion(new DrawingRegionNumatoRelayController(regions.Component, regions.View));

            var textProvider = new LayoutTextInfo(regions.Component);

            if (textProvider.OptionalElement != null)
                regions.AddRegion(new LayoutDrawingRegionText(regions, textProvider));
        }

        private class DrawingRegionNumatoRelayController : LayoutDrawingRegionGrid {

            internal DrawingRegionNumatoRelayController(ModelComponent component, ILayoutView view) : base(component, view) {
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                var painter = new NumatoRelayControllerPainter(view.GridSizeInModelCoordinates);

                painter.Paint(g);
                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Component Painter

        [DispatchTarget]
        private Image GetImage([DispatchFilter] NumatoRelayControllerPainter requestor) {
            return imageListComponents.Images[0];
        }

        private class NumatoRelayControllerPainter {
            internal NumatoRelayControllerPainter(Size _) {
            }

            internal void Paint(Graphics g) {
                var image = Dispatch.Call.GetImage(this);

                g.DrawImage(image, new Rectangle(new Point(1, 1), image.Size));
            }
        }

        #endregion
    }
}
