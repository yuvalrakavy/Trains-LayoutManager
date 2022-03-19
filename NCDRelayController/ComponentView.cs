using System.Drawing;
using System.Xml;
using MethodDispatcher;
using LayoutManager;
using LayoutManager.Model;
using LayoutManager.View;

namespace NCDRelayController {
    /// <summary>
    /// Summary description for ComponentView.
    /// </summary>
    [LayoutModule("NCD Relay controller Component View", UserControl = false)]
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
                categoryElement.InnerXml += "<Item Name='NCDRelayController' Tooltip='NCD Relay Controller' />";
        }

        [DispatchTarget]
        private void PaintImageMenuItem_NCDRelayController(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "NCDRelayController") {
            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            var painter = new NCDRelayControllerPainter();

            painter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_NCDRelayControllerl(XmlElement itemElement, [DispatchFilter] string name = "NCDRelayController") => new NCDRelayController();

        #endregion

        #region Component view

        [DispatchTarget]
        void GetModelComponentDrawingRegions_NCDrelayl([DispatchFilter] NCDRelayController component, LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions))
                regions.AddRegion(new DrawingRegionNCDRelayController(regions.Component, regions.View));

            var textProvider = new LayoutTextInfo(regions.Component);

            if (textProvider.Element != null)
                regions.AddRegion(new LayoutDrawingRegionText(regions, textProvider));
        }

        private class DrawingRegionNCDRelayController : LayoutDrawingRegionGrid {

            internal DrawingRegionNCDRelayController(ModelComponent component, ILayoutView view) : base(component, view) {
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                var painter = new NCDRelayControllerPainter();

                painter.Paint(g);
                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Component Painter

        [DispatchTarget]
        private Image GetImage([DispatchFilter] NCDRelayControllerPainter requestor) {
            return imageListComponents.Images[0];
        }

        private class NCDRelayControllerPainter {
            internal void Paint(Graphics g) {
                var image = Dispatch.Call.GetImage(this);

                g.DrawImage(image, new Rectangle(new Point(1, 1), image.Size));
            }
        }

        #endregion
    }
}
