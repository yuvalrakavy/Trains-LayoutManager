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

        [LayoutEvent("get-image", SenderType = typeof(NCDRelayControllerPainter))]
        private void GetCentralStationImage(LayoutEvent e) {
            e.Info = imageListComponents.Images[0];
        }

        private class NCDRelayControllerPainter {
            internal void Paint(Graphics g) {
                var image = Ensure.NotNull<Image>(EventManager.Event(new LayoutEvent("get-image", this)));

                g.DrawImage(image, new Rectangle(new Point(1, 1), image.Size));
            }
        }

        #endregion

        #region Address Format Handler

        [LayoutEvent("get-command-station-address-format", IfEvent = "*[CommandStation/@Type='DiMAX']")]
        [LayoutEvent("get-command-station-address-format", IfEvent = "*[CommandStation/@Type='Any']")]
        private void GetCommandStationFormat(LayoutEvent e) {
            if (e.Info == null) {
                var usage = Ensure.ValueNotNull<AddressUsage>(e.Sender);
                var addressFormat = new AddressFormatInfo();

                switch (usage) {
                    case AddressUsage.Locomotive:
                        addressFormat.Namespace = "Locomotives";
                        addressFormat.UnitMin = 1;
                        addressFormat.UnitMax = 10239;
                        break;

                    case AddressUsage.Signal:
                    case AddressUsage.Turnout:
                        addressFormat.Namespace = "Accessories";
                        addressFormat.UnitMin = 0;
                        addressFormat.UnitMax = 2047;
                        break;

                    case AddressUsage.TrainDetectionBlock:
                        addressFormat.Namespace = "Accessories";
                        addressFormat.UnitMin = 0;
                        addressFormat.UnitMax = 2047;
                        break;

                    case AddressUsage.TrackContact:
                        addressFormat.Namespace = "Accessories";
                        addressFormat.UnitMin = 0;
                        addressFormat.UnitMax = 2047;
                        addressFormat.ShowSubunit = true;
                        addressFormat.SubunitMin = 0;
                        addressFormat.SubunitMax = 1;
                        addressFormat.SubunitFormat = AddressFormatInfo.SubunitFormatValue.Alphabet;
                        break;
                }

                e.Info = addressFormat;
            }
        }

        #endregion
    }
}
