using System.ComponentModel;
using System.Drawing;
using System.Xml;
using MethodDispatcher;
using LayoutManager;
using LayoutManager.Model;
using LayoutManager.View;

namespace MarklinDigital {
    /// <summary>
    /// Summary description for ComponentView.
    /// </summary>
    [LayoutModule("Marklin Digital Component View", UserControl = false)]
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
        private void GetComponentMenuCategoryItems_Control(ModelComponent? existingTrack, XmlElement categoryElement, [DispatchFilter] string categoryName = "Control") {
            if (existingTrack == null)
                categoryElement.InnerXml += "<Item Name='MarklinDigital' Tooltip='Marklin Digital (6051) Interface' />";
        }

        [DispatchTarget]
        private void PaintImageMenuItem_MarklinDigital(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "MarklinDigital") {
            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            var painter = new MarklinDigitalPainter();

            painter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_MarklinDigital(XmlElement itemElement, [DispatchFilter] string name = "MarklinDigital") => new MarklinDigitalCentralStation();

        #endregion

        #region Component view

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(MarklinDigitalCentralStation))]
        private void GetCentralStationDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = (LayoutGetDrawingRegionsEvent)eBase;

            if (LayoutDrawingRegionGrid.IsComponentGridVisible(e))
                e.AddRegion(new DrawingRegionCentralStation(e.Component, e.View));

            var textProvider = new LayoutTextInfo(e.Component);

            if (textProvider.Element != null)
                e.AddRegion(new LayoutDrawingRegionText(e, textProvider));
        }

        private class DrawingRegionCentralStation : LayoutDrawingRegionGrid {
            internal DrawingRegionCentralStation(ModelComponent component, ILayoutView view) : base(component, view) {
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                var painter = new MarklinDigitalPainter();

                painter.Paint(g);
                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Component Painter

        [LayoutEvent("get-image", SenderType = typeof(MarklinDigitalPainter))]
        private void GetCentralStationImage(LayoutEvent e) {
            e.Info = imageListComponents.Images[0];
        }

        private class MarklinDigitalPainter {
            internal void Paint(Graphics g) {
                var image = Ensure.NotNull<Image>(EventManager.Event(new LayoutEvent("get-image", this)));

                g.DrawImage(image, new Rectangle(new Point(1, 1), image.Size));
            }
        }

        #endregion

        #region Address Format Handler

        [LayoutEvent("get-command-station-address-format", IfEvent = "*[CommandStation/@Type='MarklinDigital']")]
        [LayoutEvent("get-command-station-address-format", IfEvent = "*[CommandStation/@Type='Any']")]
        private void GetCommandStationFormat(LayoutEvent e) {
            if (e.Info == null) {
                var usage = Ensure.ValueNotNull<AddressUsage>(e.Sender);
                var addressFormat = new AddressFormatInfo();

                switch (usage) {
                    case AddressUsage.Locomotive:
                        addressFormat.Namespace = "Locomotives";
                        addressFormat.UnitMin = 1;
                        addressFormat.UnitMax = 80;
                        break;

                    case AddressUsage.Signal:
                    case AddressUsage.Turnout:
                        addressFormat.Namespace = "Accessories";
                        addressFormat.UnitMin = 0;
                        addressFormat.UnitMax = 255;
                        break;

                    case AddressUsage.TrainDetectionBlock:
                    case AddressUsage.TrackContact:
                        addressFormat.Namespace = "Feedback";
                        addressFormat.UnitMin = 1;
                        addressFormat.UnitMax = 31;
                        addressFormat.ShowSubunit = true;
                        addressFormat.SubunitMin = 1;
                        addressFormat.SubunitMax = 16;
                        addressFormat.SubunitFormat = AddressFormatInfo.SubunitFormatValue.Number;
                        break;
                }

                e.Info = addressFormat;
            }
        }

        #endregion

    }
}
