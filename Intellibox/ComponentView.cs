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

            if (textProvider.Element != null)
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

        [LayoutEvent("get-image", SenderType = typeof(IntelliboxPainter))]
        private void GetCentralStationImage(LayoutEvent e) {
            e.Info = imageListComponents.Images[0];
        }

        private class IntelliboxPainter {
            internal IntelliboxPainter(Size _) {
            }

            internal void Paint(Graphics g) {
                var image = Ensure.NotNull<Image>(EventManager.Event(new LayoutEvent("get-image", this)));

                g.DrawImage(image, new Rectangle(new Point(1, 1), image.Size));
            }
        }

        #endregion

        #region Address Format Handler

        [LayoutEvent("get-command-station-address-format", IfEvent = "*[CommandStation/@Type='IntelliboxMarklin']")]
        [LayoutEvent("get-command-station-address-format", IfEvent = "*[CommandStation/@Type='Any']")]
        private void GetCommandStationFormat(LayoutEvent e) {
            if (e.Info == null) {
                var usage = Ensure.ValueNotNull<AddressUsage>(e.Sender);
                AddressFormatInfo addressFormat = new();

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

        #region Internal singleton (i.e. not for each component instance) event handlers

        /// <summary>
        /// This event handler is "invoked" for a command manager thread. When polling the intellibox, the polling handler
        /// create an array of events that need to be invoked. Those events are invoked in the context of the main thread.
        /// </summary>
        /// <param name="e.Info">The array of events to be invoked</param>
        [LayoutEvent("intellibox-invoke-events")]
        private void IntelliboxInvokeEvents(LayoutEvent e) {
            List<LayoutEvent> events = Ensure.NotNull<List<LayoutEvent>>(e.Info);

            events.ForEach((LayoutEvent ev) => EventManager.Event(ev));
        }

        #endregion

    }
}
