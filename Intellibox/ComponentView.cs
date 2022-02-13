using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using MethodDispatcher;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.View;

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

        [LayoutEvent("get-component-menu-category-items", IfSender = "Category[@Name='Control']")]
        private void AddCentralStationItem(LayoutEvent e) {
            var categoryElement = Ensure.NotNull<XmlElement>(e.Sender);
            var old = (ModelComponent?)e.Info;

            if (old == null)
                categoryElement.InnerXml += "<Item Name='Intellibox' Tooltip='Intellibox Command Station' />";
        }

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='Intellibox']")]
        private void PaintCentralStationItem(LayoutEvent e) {
            var g = Ensure.NotNull<Graphics>(e.Info);

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            IntelliboxPainter painter = new(new Size(32, 32));

            painter.Paint(g);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='Intellibox']")]
        private void CreateCentralStationComponent(LayoutEvent e) {
            e.Info = new IntelliboxComponent();
        }

        #endregion

        #region Component view

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(IntelliboxComponent))]
        private void GetCentralStationDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = (LayoutGetDrawingRegionsEvent)eBase;

            if (LayoutDrawingRegionGrid.IsComponentGridVisible(e))
                e.AddRegion(new DrawingRegionIntellibox(e.Component, e.View));

            LayoutTextInfo textProvider = new(e.Component);

            if (textProvider.Element != null)
                e.AddRegion(new LayoutDrawingRegionText(e, textProvider));
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

        [LayoutEvent("intellibox-notify-locomotive-state")]
        private void IntelliboxNotifiyLocomotiveState(LayoutEvent e) {
            var component = Ensure.NotNull<IntelliboxComponent>(e.Sender);
            var info = Ensure.ValueNotNull<ExternalLocomotiveEventInfo>(e.Info);
            var addressMap = Dispatch.Call.GetOnTrackLocomotiveAddressMap(component);
            var addressMapEntry = addressMap?[info.Unit];

            if (addressMapEntry == null)
                LayoutModuleBase.Warning("Locomotive status reported for a unrecognized locomotive (unit " + info.Unit + ")");
            else {
                TrainStateInfo train = addressMapEntry.Train;

                int speedInSteps = 0;

                if (info.LogicalSpeed > 1) {
                    double factor = (double)train.SpeedSteps / 126;

                    speedInSteps = (int)Math.Round((info.LogicalSpeed - 2) * factor);

                    if (speedInSteps == 0)
                        speedInSteps = 1;
                    else if (speedInSteps > train.SpeedSteps)
                        speedInSteps = train.SpeedSteps;

                    if (info.Direction == LocomotiveOrientation.Backward)
                        speedInSteps = -speedInSteps;
                }

                Dispatch.Notification.OnLocomotiveMotion(component, speedInSteps, info.Unit);

                if (info.Lights != train.Lights)
                    Dispatch.Notification.OnLocomotiveLightsChanged(component, info.Unit, info.Lights);
            }
        }

        #endregion

    }
}
