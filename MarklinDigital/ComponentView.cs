using System.ComponentModel;
using System.Drawing;
using System.Xml;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.View;

namespace MarklinDigital {
    /// <summary>
    /// Summary description for ComponentView.
    /// </summary>
    [LayoutModule("Marklin Digital Component View", UserControl = false)]
    public class ComponentView : System.ComponentModel.Component, ILayoutModuleSetup {
        private System.Windows.Forms.ImageList imageListComponents;
        private IContainer components;

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

        [LayoutEvent("get-component-menu-category-items", IfSender = "Category[@Name='Control']")]
        void AddCentralStationItem(LayoutEvent e) {
            XmlElement categoryElement = (XmlElement)e.Sender;
            ModelComponent old = (ModelComponent)e.Info;

            if (old == null)
                categoryElement.InnerXml += "<Item Name='MarklinDigital' Tooltip='Marklin Digital (6051) Interface' />";
        }

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='MarklinDigital']")]
        void PaintCentralStationItem(LayoutEvent e) {
            Graphics g = (Graphics)e.Info;

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            MarklinDigitalPainter painter = new MarklinDigitalPainter(new Size(32, 32));

            painter.Paint(g);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='MarklinDigital']")]
        void CreateCentralStationComponent(LayoutEvent e) {
            e.Info = new MarklinDigitalCentralStation();
        }

        #endregion

        #region Component view

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(MarklinDigitalCentralStation))]
        void GetCentralStationDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = (LayoutGetDrawingRegionsEvent)eBase;

            if (LayoutDrawingRegionGrid.IsComponentGridVisible(e))
                e.AddRegion(new DrawingRegionCentralStation(e.Component, e.View));

            LayoutTextInfo textProvider = new LayoutTextInfo(e.Component);

            if (textProvider.Element != null)
                e.AddRegion(new LayoutDrawingRegionText(e, textProvider));

        }

        class DrawingRegionCentralStation : LayoutDrawingRegionGrid {
            readonly MarklinDigitalCentralStation component;

            internal DrawingRegionCentralStation(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = (MarklinDigitalCentralStation)component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {
                MarklinDigitalPainter painter = new MarklinDigitalPainter(view.GridSizeInModelCoordinates);

                painter.Paint(g);
                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Component Painter

        [LayoutEvent("get-image", SenderType = typeof(MarklinDigitalPainter))]
        void GetCentralStationImage(LayoutEvent e) {
            e.Info = imageListComponents.Images[0];
        }

        class MarklinDigitalPainter {
            Size componentSize;

            internal MarklinDigitalPainter(Size componentSize) {
                this.componentSize = componentSize;
            }

            internal void Paint(Graphics g) {
                Image image = (Image)EventManager.Event(new LayoutEvent("get-image", this));

                g.DrawImage(image, new Rectangle(new Point(1, 1), image.Size));
            }
        }

        #endregion

        #region Address Format Handler

        [LayoutEvent("get-command-station-address-format", IfEvent = "*[CommandStation/@Type='MarklinDigital']")]
        [LayoutEvent("get-command-station-address-format", IfEvent = "*[CommandStation/@Type='Any']")]
        private void getCommandStationFormat(LayoutEvent e) {
            if (e.Info == null) {
                AddressUsage usage = (AddressUsage)e.Sender;
                AddressFormatInfo addressFormat = new AddressFormatInfo();

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

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new Container();
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ComponentView));
            this.imageListComponents = new System.Windows.Forms.ImageList(this.components) {
                // 
                // imageListComponents
                // 
                ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit,
                ImageSize = new System.Drawing.Size(30, 30),
                ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListComponents.ImageStream"))),
                TransparentColor = System.Drawing.Color.Transparent
            };

        }
        #endregion

    }
}
