using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Xml;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.View;

namespace NumatoController {
    /// <summary>
    /// Summary description for ComponentView.
    /// </summary>
    [LayoutModule("Numato Relay controller Component View", UserControl = false)]
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
        void AddRelayControllerItem(LayoutEvent e) {
            XmlElement categoryElement = (XmlElement)e.Sender;
            ModelComponent old = (ModelComponent)e.Info;

            if (old == null)
                categoryElement.InnerXml += "<Item Name='NumatoRelayController' Tooltip='Numato Labs Relay Controller' />";
        }

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='NumatoRelayController']")]
        void PaintCentralStationItem(LayoutEvent e) {
            Graphics g = (Graphics)e.Info;

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            NumatoRelayControllerPainter painter = new NumatoRelayControllerPainter(new Size(32, 32));

            painter.Paint(g);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='NumatoRelayController']")]
        void CreateNumatoControllerComponent(LayoutEvent e) {
            e.Info = new NumatoController();
        }

        #endregion

        #region Component view

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(NumatoController))]
        void GetDiMAXcommandStationDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = (LayoutGetDrawingRegionsEvent)eBase;

            if (LayoutDrawingRegionGrid.IsComponentGridVisible(e))
                e.AddRegion(new DrawingRegionNumatoRelayController(e.Component, e.View));

            LayoutTextInfo textProvider = new LayoutTextInfo(e.Component);

            if (textProvider.Element != null)
                e.AddRegion(new LayoutDrawingRegionText(e, textProvider));

        }

        class DrawingRegionNumatoRelayController : LayoutDrawingRegionGrid {
            readonly NumatoController component;

            internal DrawingRegionNumatoRelayController(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = (NumatoController)component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {
                NumatoRelayControllerPainter painter = new NumatoRelayControllerPainter(view.GridSizeInModelCoordinates);

                painter.Paint(g);
                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Component Painter

        [LayoutEvent("get-image", SenderType = typeof(NumatoRelayControllerPainter))]
        void GetCentralStationImage(LayoutEvent e) {
            e.Info = imageListComponents.Images[0];
        }

        class NumatoRelayControllerPainter {
            Size componentSize;

            internal NumatoRelayControllerPainter(Size componentSize) {
                this.componentSize = componentSize;
            }

            internal void Paint(Graphics g) {
                Image image = (Image)EventManager.Event(new LayoutEvent("get-image", this));

                g.DrawImage(image, new Rectangle(new Point(1, 1), image.Size));
            }
        }

        #endregion

        #region Address Format Handler

        [LayoutEvent("get-command-station-address-format", IfEvent = "*[CommandStation/@Type='DiMAX']")]
        [LayoutEvent("get-command-station-address-format", IfEvent = "*[CommandStation/@Type='Any']")]
        private void getCommandStationFormat(LayoutEvent e) {
            if (e.Info == null) {
                AddressUsage usage = (AddressUsage)e.Sender;
                AddressFormatInfo addressFormat = new AddressFormatInfo();

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

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ComponentView));
            this.imageListComponents = new System.Windows.Forms.ImageList(this.components) {
                // 
                // imageListComponents
                // 
                ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListComponents.ImageStream"))),
                TransparentColor = System.Drawing.Color.Lime
            };
            this.imageListComponents.Images.SetKeyName(0, "Numto.png");

        }
        #endregion

    }
}
