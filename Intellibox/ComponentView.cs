using LayoutManager;
using LayoutManager.Model;
using LayoutManager.View;
using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Xml;

namespace Intellibox {
    /// <summary>
    /// Summary description for ComponentView.
    /// </summary>
    [LayoutModule("Intellibox Component View", UserControl = false)]
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
                categoryElement.InnerXml += "<Item Name='Intellibox' Tooltip='Intellibox Command Station' />";
        }

        [DispatchTarget]
        private void PaintImageMenuItem_Intellibox(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "Intellibox") {
            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            IntelliboxPainter painter = new(new Size(32, 32));

            painter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_Intellibox(XmlElement _, [DispatchFilter] string name = "Intellibox") => new IntelliboxComponent();

        #endregion

        #region Component view

        [DispatchTarget]
        void GetModelComponentDrawingRegions_Intellibox([DispatchFilter] IntelliboxComponent component, LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions))
                regions.AddRegion(new DrawingRegionIntellibox(regions.Component, regions.View));

            LayoutTextInfo textProvider = new(regions.Component);

            if (textProvider.OptionalElement != null)
                regions.AddRegion(new LayoutDrawingRegionText(regions, textProvider));
        }

        private class DrawingRegionIntellibox : LayoutDrawingRegionGrid {
            internal DrawingRegionIntellibox(ModelComponent component, ILayoutView view) : base(component, view) {
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                IntelliboxPainter painter = new(view.GridSizeInModelCoordinates);

                painter.Paint(g);
                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Component Painter

        [DispatchTarget]
        private Image GetImage([DispatchFilter] IntelliboxPainter requestor) {
            return imageListComponents.Images[0];
        }

        private class IntelliboxPainter {
            internal IntelliboxPainter(Size _) {
            }

            internal void Paint(Graphics g) {
                var image = Dispatch.Call.GetImage(this);

                g.DrawImage(image, new Rectangle(new Point(1, 1), image.Size));
            }
        }

        #endregion
    }
}
