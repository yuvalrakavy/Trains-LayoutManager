using System.Drawing;
using System.Xml;
using MethodDispatcher;
using LayoutManager;
using LayoutManager.Model;
using LayoutManager.View;

namespace DiMAX {
    /// <summary>
    /// Summary description for ComponentView.
    /// </summary>
    [LayoutModule("DiMAX Component View", UserControl = false)]
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
        private void GetComponentMenuCategoryItems(ModelComponent? track, XmlElement categoryElement, [DispatchFilter] string categoryName = "Control") {
            if (track == null)
                categoryElement.InnerXml += "<Item Name='DiMAX' Tooltip='Massoth DiMAX Command Station' />";
        }

        [DispatchTarget]
        private void PaintImageMenuItem_DiMAX(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "DiMAX") {
            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            var painter = new DiMAXcommandStationPainter(new Size(32, 32));

            painter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_DiMax(XmlElement _, [DispatchFilter] string name = "DiMAX") => new DiMAXcommandStation();

        #endregion

        #region Component view

        [DispatchTarget]
        void GetModelComponentDrawingRegions_DiMAX([DispatchFilter] DiMAXcommandStation component, LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions))
                regions.AddRegion(new DrawingRegionDiMAXcommandStation(regions.Component, regions.View));

            var textProvider = new LayoutTextInfo(regions.Component);

            if (textProvider.OptionalElement != null)
                regions.AddRegion(new LayoutDrawingRegionText(regions, textProvider));
        }

        private class DrawingRegionDiMAXcommandStation : LayoutDrawingRegionGrid {

            internal DrawingRegionDiMAXcommandStation(ModelComponent component, ILayoutView view) : base(component, view) {
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                var painter = new DiMAXcommandStationPainter(view.GridSizeInModelCoordinates);

                painter.Paint(g);
                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Component Painter

        [DispatchTarget]
        private Image GetImage([DispatchFilter] DiMAXcommandStationPainter requestor) => imageListComponents.Images[0];

        private class DiMAXcommandStationPainter {
            internal DiMAXcommandStationPainter(Size _) {
            }

            internal void Paint(Graphics g) {
                Image image = Dispatch.Call.GetImage(this);

                g.DrawImage(image, new Rectangle(new Point(1, 1), image.Size));
            }
        }

        #endregion
    }
}
