using System.ComponentModel;
using System.Drawing;
using System.Xml;
using MethodDispatcher;
using LayoutManager;
using LayoutManager.Model;
using LayoutManager.View;

namespace LayoutLGB {
    /// <summary>
    /// Summary description for ComponentView.
    /// </summary>
    [LayoutModule("LGB MTS Component View", UserControl = false)]
    public partial class ComponentView : System.ComponentModel.Component, ILayoutModuleSetup {
        #region Constructors

        public ComponentView(IContainer container) {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            container.Add(this);
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        public ComponentView() {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        #endregion

        #region Component menu Item

        [DispatchTarget]
        private void GetComponentMenuCategoryItems(ModelComponent? track, XmlElement categoryElement, [DispatchFilter] string categoryName = "Control") {
            if (track == null)
                categoryElement.InnerXml += "<Item Name='CentralStation' Tooltip='LGB MTS Central Station' />";
        }

        [DispatchTarget]
        private void PaintImageMenuItem_CentralStation(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "CentralStation") {
            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            var painter = new CentralStationPainter();

            painter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_CentralStation(XmlElement _, [DispatchFilter] string name = "CentralStation") => new MTScentralStation();

        #endregion

        #region Component view

        [DispatchTarget]
        void GetModelComponentDrawingRegions_MTScentralStation([DispatchFilter] MTScentralStation component, LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions))
                regions.AddRegion(new DrawingRegionCentralStation(regions.Component, regions.View));

            var textProvider = new LayoutTextInfo(regions.Component);

            if (textProvider.Element != null)
                regions.AddRegion(new LayoutDrawingRegionText(regions, textProvider));
        }

        private class DrawingRegionCentralStation : LayoutDrawingRegionGrid {
            internal DrawingRegionCentralStation(ModelComponent component, ILayoutView view) : base(component, view) {
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                var painter = new CentralStationPainter();

                painter.Paint(g);
                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Component Painter

        [DispatchTarget]
        private Image GetImage([DispatchFilter] CentralStationPainter requestor) {
            return imageListComponents.Images[0];
        }

        private class CentralStationPainter {

            internal CentralStationPainter() {
            }

            internal void Paint(Graphics g) {
                var image = Dispatch.Call.GetImage(this);

                g.DrawImage(image, new Rectangle(new Point(1, 1), image.Size));
            }
        }

        #endregion
    }
}
