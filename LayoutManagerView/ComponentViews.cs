using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Windows.Forms;
using LayoutManager.Components;
using LayoutManager.Model;

#pragma warning disable IDE0051
#nullable enable
namespace LayoutManager.View {

    /// <summary>
    /// Summary description for ComponentViews.
    /// </summary>
    [LayoutModule("Component Views", UserControl = false)]
    public class ComponentViews : System.ComponentModel.Component, ILayoutModuleSetup {
        private IContainer components;
        private ImageList imageListComponents;
        private ImageList imageListSignals;
        private ImageList imageListConnectionPointImages;

        #region Constructors
        #nullable disable
        public ComponentViews(IContainer container) {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            container.Add(this);
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        public ComponentViews() {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
        #nullable enable

        #endregion

        #region Event Handlers

        [LayoutEvent("get-components-image-list")]
        private void getComponentsImageList(LayoutEvent e) {
            e.Info = imageListComponents;
        }

        #endregion

        #region Track Components

        #region Stright Track

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(LayoutStraightTrackComponent))]
        void GetStraightTrackDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = Ensure.NotNull<LayoutGetDrawingRegionsEvent>(eBase, "e");

            if (LayoutDrawingRegionGrid.IsComponentGridVisible(e))
                e.AddRegion(new LayoutDrawingRegionStraightTrack(e.Component, e.View));
        }

        class LayoutDrawingRegionStraightTrack : LayoutDrawingRegionGrid {
            readonly LayoutStraightTrackComponent component;

            public LayoutDrawingRegionStraightTrack(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = (LayoutStraightTrackComponent)component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {
                TrackSegment trackSegment = new TrackSegment(component, component.ConnectionPoints);
                TrackColors colors = view.GetTrackSegmentColor(trackSegment);

                LayoutStraightTrackPainter painter = new LayoutStraightTrackPainter(view.GridSizeInModelCoordinates, trackSegment.ConnectionPoints) {
                    TrackColor = colors.Color(0),
                    TrackColor2 = colors.Color(1)
                };
                painter.Paint(g);

                if (colors.Annotations != null) {
                    GraphicsState gs = g.Save();

                    g.TranslateTransform(view.GridSizeInModelCoordinates.Width / 2, view.GridSizeInModelCoordinates.Height / 2);
                    SizeF minSize = new SizeF((float)view.GridSizeInModelCoordinates.Width / 3, view.GridSizeInModelCoordinates.Height / 4);

                    foreach (RoutePreviewAnnotation annotation in colors.Annotations) {
                        LocomotivePainter locoPainter = new LocomotivePainter {
                            BackgroundBrush = null,
                            DrawFront = true,
                            DrawLabel = false,
                            Front = annotation.Front,
                            MinSize = minSize,
                            Speed = annotation.Direction == LocomotiveOrientation.Forward ? 1 : -1
                        };
                        locoPainter.Draw(g);
                    }

                    g.Restore(gs);
                }

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Double Track Component

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(LayoutDoubleTrackComponent))]
        void GetDoubleTrackDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = Ensure.NotNull<LayoutGetDrawingRegionsEvent>(eBase, "e");

            if (LayoutDrawingRegionGrid.IsComponentGridVisible(e))
                e.AddRegion(new LayoutDrawingRegionDoubleTrack(e.Component, e.View));
        }

        class LayoutDrawingRegionDoubleTrack : LayoutDrawingRegionGrid {
            readonly LayoutDoubleTrackComponent component;

            public LayoutDrawingRegionDoubleTrack(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = (LayoutDoubleTrackComponent)component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {
                TrackSegment segment1 = new TrackSegment(component, component.GetTrackPath(0));
                TrackSegment segment2 = new TrackSegment(component, component.GetTrackPath(1));
                TrackColors trackColors1 = view.GetTrackSegmentColor(segment1);
                TrackColors trackColors2 = view.GetTrackSegmentColor(segment2);

                LayoutStraightTrackPainter painter = new LayoutStraightTrackPainter(view.GridSizeInModelCoordinates, segment1.ConnectionPoints) {
                    TrackColor = trackColors1.Color(0),
                    TrackColor2 = trackColors1.Color(1)
                };

                painter.Paint(g);

                painter = new LayoutStraightTrackPainter(view.GridSizeInModelCoordinates, segment2.ConnectionPoints) {
                    TrackColor = trackColors2.Color(0),
                    TrackColor2 = trackColors2.Color(1)
                };
                painter.Paint(g);

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Turnout Track Component

        [LayoutEvent("get-connection-point-component-image", SenderType = typeof(LayoutTurnoutTrackComponent))]
        private void getTurnoutConnectionPointImage(LayoutEvent e) {
            e.Info = imageListConnectionPointImages.Images[0];
        }

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(LayoutTurnoutTrackComponent))]
        void GetSwitchDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = Ensure.NotNull<LayoutGetDrawingRegionsEvent>(eBase, "e");

            if (LayoutDrawingRegionGrid.IsComponentGridVisible(e))
                e.AddRegion(new LayoutDrawingRegionTurnoutTrack(e.Component, e.View));

            e.AddRegion(new LayoutDrawingRegionNotConnected(e.Component, e.View));
        }

        class LayoutDrawingRegionTurnoutTrack : LayoutDrawingRegionGrid {
            readonly LayoutTurnoutTrackComponent component;

            public LayoutDrawingRegionTurnoutTrack(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = (LayoutTurnoutTrackComponent)component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {
                TrackSegment straightSegment = new TrackSegment(component, component.Tip, component.Straight);
                TrackSegment branchSegment = new TrackSegment(component, component.Tip, component.Branch);
                LayoutTurnoutTrackPainter painter = new LayoutTurnoutTrackPainter(view.GridSizeInModelCoordinates,
                    component.Tip, component.Straight, component.Branch,
                    (component.CurrentSwitchState == 0) ? component.Straight : component.Branch);

                TrackColors straightColors = view.GetTrackSegmentColor(straightSegment);
                TrackColors branchColors = view.GetTrackSegmentColor(branchSegment);

                painter.TrackColor = straightColors.Color(0);
                painter.BranchColor = branchColors.Color(0);
                painter.Paint(g);
                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Three way Turnout Track Component

        [LayoutEvent("get-connection-point-component-image", SenderType = typeof(LayoutThreeWayTurnoutComponent))]
        private void getThreeWayTurnoutConnectionPointImage(LayoutEvent e) {
            e.Info = imageListConnectionPointImages.Images[7];
        }

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(LayoutThreeWayTurnoutComponent))]
        void GetThreeWaySwitchDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = Ensure.NotNull<LayoutGetDrawingRegionsEvent>(eBase, "e");

            if (LayoutDrawingRegionGrid.IsComponentGridVisible(e))
                e.AddRegion(new LayoutDrawingRegionThreeWayTurnoutTrack(e.Component, e.View));

            e.AddRegion(new LayoutDrawingRegionNotConnected(e.Component, e.View));
        }

        class LayoutDrawingRegionThreeWayTurnoutTrack : LayoutDrawingRegionGrid {
            readonly LayoutThreeWayTurnoutComponent component;

            public LayoutDrawingRegionThreeWayTurnoutTrack(ModelComponent component, ILayoutView view)
                : base(component, view) {
                this.component = (LayoutThreeWayTurnoutComponent)component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {
                Color[] segmentColors = new Color[3];

                for (int segment = 0; segment < 3; segment++) {
                    TrackSegment trackSegment = new TrackSegment(component, component.Tip, component.ConnectTo(component.Tip, segment));
                    segmentColors[segment] = view.GetTrackSegmentColor(trackSegment).Color(0);
                }

                LayoutThreeWayTurnoutPainter painter = new LayoutThreeWayTurnoutPainter(view.GridSizeInModelCoordinates, component.Tip, component.CurrentSwitchState) {
                    SegmentColors = segmentColors
                };
                painter.Paint(g);
                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Double slip Track Component

        [LayoutEvent("get-connection-point-component-image", SenderType = typeof(LayoutDoubleSlipTrackComponent))]
        private void getDoubleSlipConnectionPointImage(LayoutEvent e) {
            e.Info = imageListConnectionPointImages.Images[6];
        }

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(LayoutDoubleSlipTrackComponent))]
        void GetDoubleSlipDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = Ensure.NotNull<LayoutGetDrawingRegionsEvent>(eBase, "e");

            if (LayoutDrawingRegionGrid.IsComponentGridVisible(e))
                e.AddRegion(new LayoutDrawingRegionDoubleSlipTrack(e.Component, e.View));

            e.AddRegion(new LayoutDrawingRegionNotConnected(e.Component, e.View));
        }

        class LayoutDrawingRegionDoubleSlipTrack : LayoutDrawingRegionGrid {
            readonly LayoutDoubleSlipTrackComponent component;

            public LayoutDrawingRegionDoubleSlipTrack(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = (LayoutDoubleSlipTrackComponent)component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {
                TrackSegment horizontalSegment = new TrackSegment(component, LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.R);
                TrackSegment verticalSegment = new TrackSegment(component, LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B);
                TrackSegment leftSegment;
                TrackSegment rightSegment;

                if (component.DiagonalIndex == 0) {
                    leftSegment = new TrackSegment(component, LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.T);
                    rightSegment = new TrackSegment(component, LayoutComponentConnectionPoint.R, LayoutComponentConnectionPoint.B);
                }
                else {
                    leftSegment = new TrackSegment(component, LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.B);
                    rightSegment = new TrackSegment(component, LayoutComponentConnectionPoint.R, LayoutComponentConnectionPoint.T);
                }

                LayoutDoubleSlipPainter painter = new LayoutDoubleSlipPainter(view.GridSizeInModelCoordinates, component.DiagonalIndex, component.CurrentSwitchState) {
                    HorizontalTrackColor = view.GetTrackSegmentColor(horizontalSegment).Color(0),
                    VerticalTrackColor = view.GetTrackSegmentColor(verticalSegment).Color(0),
                    LeftBranchColor = view.GetTrackSegmentColor(leftSegment).Color(0),
                    RightBranchColor = view.GetTrackSegmentColor(rightSegment).Color(0)
                };

                painter.Paint(g);
                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #endregion

        #region TriggerableBlockEdgeComponent (track contact/proximity sensor)

        [LayoutEvent("get-connection-point-component-image", SenderType = typeof(LayoutTrackContactComponent))]
        [LayoutEvent("get-connection-point-component-image", SenderType = typeof(LayoutProximitySensorComponent))]
        private void getTrackContactConnectionPointImage(LayoutEvent e) {
            e.Info = imageListConnectionPointImages.Images[1];
        }

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(LayoutTrackContactComponent))]
        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(LayoutProximitySensorComponent))]
        void GetTriggerableBlockEdgeDrawingRegions(LayoutEvent eBase) {
            var e = Ensure.NotNull<LayoutGetDrawingRegionsEvent>(eBase, "e");

            if (LayoutDrawingRegionGrid.IsComponentGridVisible(e))
                e.AddRegion(new LayoutDrawingRegionTriggerableBlockEdge(e.Component, e.View));

            LayoutTextInfo nameProvider = new LayoutTextInfo(e.Component);

            if (nameProvider.OptionalElement != null && nameProvider.Visible)
                e.AddRegion(new LayoutDrawingRegionText(e, nameProvider));

            e.AddRegion(new LayoutDrawingRegionNotConnected(e.Component, e.View));
        }

        class LayoutDrawingRegionTriggerableBlockEdge : LayoutDrawingRegionGrid {
            readonly LayoutTriggerableBlockEdgeBase component;

            public LayoutDrawingRegionTriggerableBlockEdge(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = (LayoutTriggerableBlockEdgeBase)component;
            }

            public LayoutTriggerableBlockEdgePainter.ComponentType PainterComponentType {
                get => this.component switch
                {
                    LayoutTrackContactComponent _ => LayoutTriggerableBlockEdgePainter.ComponentType.TrackContact,
                    LayoutProximitySensorComponent _ => LayoutTriggerableBlockEdgePainter.ComponentType.ProximitySensor,
                    _ => throw new NotImplementedException()
                };
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {
                bool disposeFill = true;

                if (component.OptionalTrack != null) {
                    var painter = new LayoutTriggerableBlockEdgePainter(componentType: PainterComponentType, componentSize: view.GridSizeInModelCoordinates, cp: component.Track.ConnectionPoints);

                    if (component.IsEmergencySensor) {
                        painter.Fill = new SolidBrush(Color.DarkRed);
                        painter.ContactSize = component.IsTriggered ? new Size(13, 13) : new Size(11, 11);
                    }
                    else {
                        if (LayoutController.IsOperationMode) {
                            switch (component.SignalState) {
                                case LayoutSignalState.Red:
                                    painter.Fill = new SolidBrush(Color.Red);
                                    painter.ContactSize = new Size(11, 11);
                                    break;

                                case LayoutSignalState.Yellow:
                                    painter.Fill = new SolidBrush(Color.Yellow);
                                    break;

                                case LayoutSignalState.Green:
                                    painter.Fill = new SolidBrush(Color.Green);
                                    painter.ContactSize = new Size(11, 11);
                                    break;
                            }
                        }
                        else
                            disposeFill = false;
                    }

                    if (component.IsTriggered)
                        painter.ContactSize = new Size(13, 13);

                    painter.Paint(g);
                    if (disposeFill)
                        painter.Fill.Dispose();
                }
                else {
                    // If there is no track, paint a large contact in the middle of the component. This case should
                    // not really happend
                    using (LayoutTriggerableBlockEdgePainter painter = new LayoutTriggerableBlockEdgePainter(componentType: PainterComponentType, componentSize: view.GridSizeInModelCoordinates,
                        cp: new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B })) {

                        painter.ContactSize = new Size(12, 12);
                        painter.Paint(g);
                    }
                }
                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Occupancy Block Edge Component

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(LayoutBlockEdgeComponent))]
        void GetOccupancyBlockEdgeDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = Ensure.NotNull<LayoutGetDrawingRegionsEvent>(eBase, "e");

            if (LayoutDrawingRegionGrid.IsComponentGridVisible(e))
                e.AddRegion(new LayoutDrawingRegionOccpancyBlockEdge(e.Component, e.View));
        }

        class LayoutDrawingRegionOccpancyBlockEdge : LayoutDrawingRegionGrid {
            readonly LayoutBlockEdgeComponent component;

            public LayoutDrawingRegionOccpancyBlockEdge(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = (LayoutBlockEdgeComponent)component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {
                if (component.OptionalTrack != null) {
                    using (LayoutBlockEdgePainter painter = new LayoutBlockEdgePainter(view.GridSizeInModelCoordinates, component.Track.ConnectionPoints)) {

                        if (LayoutController.IsOperationMode) {
                            switch (component.SignalState) {
                                case LayoutSignalState.Red:
                                    painter.Fill = new SolidBrush(Color.Red);
                                    break;

                                case LayoutSignalState.Yellow:
                                    painter.Fill = new SolidBrush(Color.Yellow);
                                    break;

                                case LayoutSignalState.Green:
                                    painter.Fill = new SolidBrush(Color.Green);
                                    break;
                            }
                            painter.Paint(g);
                            painter.Fill.Dispose();
                        }
                        else
                            painter.Paint(g);
                    }
                }
                else {
                    // If there is no track, paint a large contact in the middle of the component. This case should
                    // not really happend
                    using (LayoutBlockEdgePainter painter = new LayoutBlockEdgePainter(view.GridSizeInModelCoordinates,
                              new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B })) {

                        painter.ContactSize = new Size(12, 12);
                        painter.Paint(g);
                    }
                }

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Block Info Component

        [LayoutEvent("get-connection-point-component-image", SenderType = typeof(LayoutBlockDefinitionComponent))]
        private void getBlockInfoConnectionPointImage(LayoutEvent e) {
            e.Info = imageListConnectionPointImages.Images[2];
        }

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(LayoutBlockDefinitionComponent))]
        void GetBlockInfoRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = Ensure.NotNull<LayoutGetDrawingRegionsEvent>(eBase, "e");
            LayoutBlockDefinitionComponent blockDefinition = Ensure.NotNull<LayoutBlockDefinitionComponent>(e.Component, "blockDefinition");

            if (LayoutBlockBallon.IsDisplayed(blockDefinition))
                e.AddRegion(new LayoutDrawingRegionBallonInfo(e.Component, e.View, e.Graphics, LayoutBlockBallon.Get(blockDefinition)));

            if (LayoutController.IsOperationMode && blockDefinition.OptionalBlock != null && blockDefinition.OptionalBlock.HasTrains) {
                IList<TrainLocationInfo> trainLocations = blockDefinition.Block.Trains;
                TrainLocationPainter[] painters = new TrainLocationPainter[trainLocations.Count];
                int i;
                SizeF totalSize = new SizeF(0, 0);
                int gap = 4;
                bool trainDetected = blockDefinition.Info.TrainDetected;
                bool vertical = LayoutTrackComponent.IsVertical(trainLocations[0].DisplayFront);


                for (i = 0; i < trainLocations.Count; i++) {
                    painters[i] = new TrainLocationPainter(trainLocations[i], e.DetailLevel);


                    if (trainLocations[i].TrainPart != TrainPart.Locomotive)
                        painters[i].DrawExtensionMark = true;

                    TrainStateInfo train = trainLocations[i].Train;

                    if (train.Trip != null) {
                        switch (train.Trip.Status) {

                            case TripStatus.Go:
                                painters[i].BackColor = trainDetected ? Color.SpringGreen : Color.SeaGreen;
                                if (!trainDetected)
                                    painters[i].LabelBrush = Brushes.White;

                                break;

                            case TripStatus.PrepareStop:
                                painters[i].BackColor = trainDetected ? Color.Yellow : Color.PaleGoldenrod;
                                break;

                            case TripStatus.Suspended:
                                painters[i].BackColor = Color.MistyRose;
                                break;

                            case TripStatus.WaitLock:
                                painters[i].BackColor = trainDetected ? Color.Red : Color.DarkSalmon;
                                painters[i].LabelBrush = Brushes.Yellow;
                                break;

                            case TripStatus.WaitStartCondition:
                                painters[i].BackColor = Color.DeepPink;
                                break;

                            default:
                                painters[i].BackColor = trainDetected ? Color.Tomato : Color.Coral;
                                break;
                        }
                    }
                    else if (!trainDetected) {
                        painters[i].BackColor = Color.DarkViolet;
                        painters[i].LabelBrush = Brushes.Yellow;
                    }

                    SizeF locoSize = painters[i].Measure(e.Graphics);

                    if (locoSize.Height > totalSize.Height)
                        totalSize.Height = locoSize.Height;

                    if (totalSize.Width > 0)
                        totalSize.Width += gap;
                    totalSize.Width += locoSize.Width;
                }

                PointF gridLocation = e.View.ModelLocationInModelCoordinates(blockDefinition.Location);

                if (vertical) {
                    PointF origin = new PointF(gridLocation.X + e.View.GridSizeInModelCoordinates.Width / 2 - totalSize.Height / 2,
                        gridLocation.Y + e.View.GridSizeInModelCoordinates.Height / 2 - totalSize.Height / 2 - totalSize.Width / 2);

                    for (i = 0; i < painters.Length; i++) {
                        SizeF locoSize = painters[i].Measure(e.Graphics);

                        e.AddRegion(new LayoutDrawingRegionLocomotive(blockDefinition, origin, locoSize, vertical, painters[i]));
                        origin.Y += locoSize.Width;
                        if (i > 0)
                            origin.Y += gap;
                    }
                }
                else {  // Either horizontal or diagonal
                        // Start adding the regions
                    PointF origin = new PointF(gridLocation.X + e.View.GridSizeInModelCoordinates.Width / 2 - totalSize.Width / 2, gridLocation.Y + e.View.GridSizeInModelCoordinates.Height / 2 - totalSize.Height / 2);

                    for (i = 0; i < painters.Length; i++) {
                        SizeF locoSize = painters[i].Measure(e.Graphics);

                        e.AddRegion(new LayoutDrawingRegionLocomotive(blockDefinition, origin, locoSize, vertical, painters[i]));
                        origin.X += locoSize.Width;
                        if (i > 0)
                            origin.X += gap;
                    }
                }
            }
            else {
                if (LayoutController.IsOperationMode && blockDefinition.Info.UnexpectedTrainDetected) {
                    LocomotivePainter locoPainter = new LocomotivePainter {
                        Label = " ? ",
                        DrawFront = false,
                        BackgroundBrush = Brushes.DarkViolet,
                        LabelBrush = Brushes.Yellow,
                        MinSize = new SizeF(50, 12),
                        Front = blockDefinition.Track.ConnectionPoints[0]
                    };

                    Size totalSize = locoPainter.Measure(e.Graphics);
                    PointF gridLocation = e.View.ModelLocationInModelCoordinates(blockDefinition.Location);

                    if (LayoutTrackComponent.IsVertical(blockDefinition.Track.ConnectionPoints[0])) {
                        PointF origin = new PointF(gridLocation.X + e.View.GridSizeInModelCoordinates.Width / 2 - totalSize.Height / 2,
                            gridLocation.Y + e.View.GridSizeInModelCoordinates.Height / 2 - totalSize.Height / 2 - totalSize.Width / 2);

                        e.AddRegion(new LayoutDrawingRegionLocomotive(blockDefinition, origin, totalSize, true, locoPainter));
                    }
                    else {
                        PointF origin = new PointF(gridLocation.X + e.View.GridSizeInModelCoordinates.Width / 2 - totalSize.Width / 2, gridLocation.Y + e.View.GridSizeInModelCoordinates.Height / 2 - totalSize.Height / 2);

                        e.AddRegion(new LayoutDrawingRegionLocomotive(blockDefinition, origin, totalSize, false, locoPainter));
                    }
                }
                else if (LayoutDrawingRegionGrid.IsComponentGridVisible(e)) {
                    e.AddRegion(new LayoutDrawingRegionBlockInfo(e.Component, e.View));

                    if (blockDefinition.Info.IsOccupancyDetectionBlock)
                        e.AddRegion(new LayoutDrawingRegionNotConnected(e.Component, e.View));
                }
            }

            LayoutTextInfo nameProvider = new LayoutTextInfo(e.Component);

            if (nameProvider.Element != null && nameProvider.Visible)
                e.AddRegion(new LayoutDrawingRegionText(e, nameProvider));
        }

        class LayoutDrawingRegionLocomotive : LayoutDrawingRegion {
            readonly ILayoutComponentPainter painter;

            public LayoutDrawingRegionLocomotive(LayoutBlockDefinitionComponent blockInfo, PointF origin, SizeF locoSize, bool vertical, ILayoutComponentPainter painter) : base(blockInfo) {
                this.painter = painter;

                if (vertical) {
                    SizeF regionSize = new SizeF(locoSize.Height + 4, locoSize.Width + 4);

                    BoundingRegionInModelCoordinates = new Region(new RectangleF(new PointF(origin.X - 2, origin.Y - 2), regionSize));
                }
                else {
                    SizeF regionSize = new SizeF(locoSize.Width + 4, locoSize.Height + 4);

                    BoundingRegionInModelCoordinates = new Region(new RectangleF(new PointF(origin.X - 2, origin.Y - 2), regionSize));
                }
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {
                GraphicsState gs = g.Save();
                RectangleF bbox = BoundingRegionInModelCoordinates.GetBounds(g);

                g.TranslateTransform(bbox.Width / 2.0F, bbox.Height / 2.0F);
                painter.Draw(g);
                g.Restore(gs);

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        class LayoutDrawingRegionBlockInfo : LayoutDrawingRegionGrid {
            readonly LayoutBlockDefinitionComponent component;

            public LayoutDrawingRegionBlockInfo(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = (LayoutBlockDefinitionComponent)component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {
                if (component.Track != null) {
                    using (LayoutBlockInfoPainter painter = new LayoutBlockInfoPainter(view.GridSizeInModelCoordinates, component.Track.ConnectionPoints)) {
                        if ((component.Spot.Phase & LayoutModel.ActivePhases) != 0) {
                            if (LayoutController.IsOperationMode && !component.Info.CanTrainWait)
                                painter.Fill = new SolidBrush(Color.Black);
                            else if (component.Info.SuggestForPlacement || component.Info.SuggestForDestination)
                                painter.Fill = new SolidBrush(Color.LightBlue);
                            else
                                painter.Fill = new SolidBrush(Color.Blue);


                            if (!component.Info.TrainPassCondition.IsConditionEmpty || !component.Info.TrainStopCondition.IsConditionEmpty)
                                painter.Outline = Pens.Red;
                        }
                        else
                            painter.Fill = new SolidBrush(Color.Black);

                        painter.OccupancyDetectionBlock = component.Info.IsOccupancyDetectionBlock;

                        painter.Paint(g);
                        painter.Fill.Dispose();
                    }
                }
                else {
                    // If there is no track, paint a large contact in the middle of the component. This case should
                    // not really happend
                    using (LayoutBlockInfoPainter painter = new LayoutBlockInfoPainter(view.GridSizeInModelCoordinates,
                              new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B })) {

                        painter.InfoBoxSize = new Size(12, 12);
                        painter.Paint(g);
                    }
                }
                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Signal Component

        [LayoutEvent("get-connection-point-component-image", SenderType = typeof(LayoutSignalComponent))]
        private void getSignalConnectionPointImage(LayoutEvent e) {
            LayoutSignalComponent component = Ensure.NotNull<LayoutSignalComponent>(e.Sender, "component");

            switch (component.Info.SignalType) {

                case LayoutSignalType.Lights:
                    e.Info = imageListConnectionPointImages.Images[3];
                    break;

                case LayoutSignalType.Semaphore:
                    e.Info = imageListConnectionPointImages.Images[4];
                    break;

                case LayoutSignalType.Distance:
                    e.Info = imageListConnectionPointImages.Images[5];
                    break;
            }
        }

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(LayoutSignalComponent))]
        void GetSignalDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = Ensure.NotNull<LayoutGetDrawingRegionsEvent>(eBase, "e");

            if (LayoutDrawingRegionGrid.IsComponentGridVisible(e)) {

                e.AddRegion(new LayoutDrawingRegionSignal(e.Component, e.View, e.DetailLevel, imageListSignals));
            }

            e.AddRegion(new LayoutDrawingRegionNotConnected(e.Component, e.View, e.DetailLevel != ViewDetailLevel.High));
        }

        class LayoutDrawingRegionSignal : LayoutDrawingRegion {
            readonly bool vertical;
            readonly LayoutSignalType signalType;
            readonly LayoutSignalState state;
            readonly ImageList imageListSignals;
            readonly bool showNotLinked;

            static Dictionary<Guid, LayoutBlockEdgeBase>? linkedSignalMap = null;

            public LayoutDrawingRegionSignal(ModelComponent component, ILayoutView view, ViewDetailLevel detailLevel, ImageList imageListSignals) : base(component) {
                LayoutSignalComponent signalComponent = Ensure.NotNull<LayoutSignalComponent>(component, "signalComponent");

                this.signalType = signalComponent.Info.SignalType;
                this.state = signalComponent.SignalState;
                this.imageListSignals = imageListSignals;

                if (signalComponent.Track != null)
                    vertical = LayoutTrackComponent.IsVertical(signalComponent.Track);

                PointF center = view.ModelLocationInModelCoordinates(signalComponent.Location);

                center.X += 16;
                center.Y += 16;

                if (LayoutController.IsOperationMode)
                    showNotLinked = false;
                else {
                    if (linkedSignalMap == null)
                        linkedSignalMap = (Dictionary<Guid, LayoutBlockEdgeBase>)EventManager.Event(new LayoutEvent("get-linked-signal-map", this))!;

                    showNotLinked = !linkedSignalMap.ContainsKey(signalComponent.Id);
                }

                if (detailLevel == ViewDetailLevel.High) {
                    if (vertical)
                        BoundingRegionInModelCoordinates = new Region(new RectangleF(center.X + 5, center.Y - 11, 22, 22));
                    else
                        BoundingRegionInModelCoordinates = new Region(new RectangleF(center.X - 11, center.Y - 21, 22, 22));
                }
                else
                    BoundingRegionInModelCoordinates = new Region(new RectangleF(center.X - 15, center.Y - 15, 30, 30));
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {
                int imageIndex = 0;

                if (detailLevel == ViewDetailLevel.High) {
                    if (state == LayoutSignalState.Green)
                        imageIndex = 1;

                    if (signalType == LayoutSignalType.Semaphore)
                        imageIndex += 2;
                    else if (signalType == LayoutSignalType.Distance)
                        imageIndex += 4;

                    Rectangle imageRect = new Rectangle(new Point(1, 1), imageListSignals.ImageSize);

                    g.DrawImage(imageListSignals.Images[imageIndex], imageRect);

                    if (showNotLinked)
                        using (Pen p = new Pen(Color.Red, 2))
                            g.DrawEllipse(p, imageRect);
                }
                else {
                    int size = detailLevel == ViewDetailLevel.Medium ? 10 : 14;
                    Point[] points = new Point[] { new Point(16, 16 - size), new Point(16 - size, 16), new Point(16, 16 + size), new Point(16 + size, 16) };
                    Brush fillBrush;

                    if (LayoutController.IsOperationMode) {
                        switch (state) {
                            case LayoutSignalState.Green: fillBrush = Brushes.LightGreen; break;
                            case LayoutSignalState.Red: fillBrush = Brushes.Red; break;
                            default:
                            case LayoutSignalState.Yellow: fillBrush = Brushes.Yellow; break;
                        }
                    }
                    else
                        fillBrush = Brushes.Yellow;

                    g.FillPolygon(fillBrush, points);

                    if (detailLevel == ViewDetailLevel.Medium)
                        g.DrawPolygon(Pens.Black, points);

                    if (showNotLinked)
                        using (Pen p = new Pen(Color.Red, 3)) {
                            int d = size + 2;

                            g.DrawEllipse(p, new Rectangle(new Point(16 - d, 16 - d), new Size(2 * d, 2 * d)));
                        }
                }

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Track Isolation component

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(LayoutTrackIsolationComponent))]
        void GetTrackIsolationDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = Ensure.NotNull<LayoutGetDrawingRegionsEvent>(eBase, "e");

            if (LayoutDrawingRegionGrid.IsComponentGridVisible(e))
                e.AddRegion(new LayoutDrawingRegionTrackIsolation(e.Component, e.View));
        }

        class LayoutDrawingRegionTrackIsolation : LayoutDrawingRegionGrid {
            readonly ModelComponent component;

            public LayoutDrawingRegionTrackIsolation(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {

                if (component.Spot.Track is LayoutStraightTrackComponent track) {
                    LayoutTrackIsolationPainter painter = new LayoutTrackIsolationPainter(view.GridSizeInModelCoordinates, track.ConnectionPoints);

                    painter.Paint(g);
                }

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Track reverse loop moduole component

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(LayoutTrackReverseLoopModule))]
        void GetTrackReverseLoopModuleDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = Ensure.NotNull<LayoutGetDrawingRegionsEvent>(eBase, "e");

            if (LayoutDrawingRegionGrid.IsComponentGridVisible(e))
                e.AddRegion(new LayoutDrawingRegionTrackReverseLoopModule(e.Component, e.View));
        }

        class LayoutDrawingRegionTrackReverseLoopModule : LayoutDrawingRegionGrid {
            readonly ModelComponent component;

            public LayoutDrawingRegionTrackReverseLoopModule(ModelComponent component, ILayoutView view)
                : base(component, view) {
                this.component = component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {

                if (component.Spot.Track is LayoutStraightTrackComponent track) {
                    var painter = new LayoutTrackReverseLoopModulePainter(view.GridSizeInModelCoordinates, track.ConnectionPoints);

                    painter.Paint(g);
                }

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Track Power connector

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(LayoutTrackPowerConnectorComponent))]
        void GetTrackPowerConnectorDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = Ensure.NotNull<LayoutGetDrawingRegionsEvent>(eBase, "e");

            if (LayoutDrawingRegionGrid.IsComponentGridVisible(e))
                e.AddRegion(new LayoutDrawingRegionTrackPowerConnector(e.Component, e.View));

            LayoutTrackPowerConnectorInfo textProvider = new LayoutTrackPowerConnectorInfo(e.Component);

            if (textProvider.Element != null && textProvider.Visible)
                e.AddRegion(new LayoutDrawingRegionText(e, textProvider));
        }

        class LayoutDrawingRegionTrackPowerConnector : LayoutDrawingRegionGrid {
            readonly ModelComponent component;

            public LayoutDrawingRegionTrackPowerConnector(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {

                if (component.Spot.Track is LayoutStraightTrackComponent track) {
                    using (LayoutPowerConnectorPainter painter = new LayoutPowerConnectorPainter(view.GridSizeInModelCoordinates, track.ConnectionPoints)) {

                        painter.Paint(g);
                    }
                }

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Power Selector/Switch component

        [LayoutEvent("get-connection-point-component-image", SenderType = typeof(LayoutPowerSelectorComponent))]
        private void getPowerSelectorConnectionPointImage(LayoutEvent e) {

            if (e.Sender is LayoutPowerSelectorComponent component && component.IsSwitch)
                e.Info = imageListConnectionPointImages.Images[9];
            else
                e.Info = imageListConnectionPointImages.Images[8];
        }

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(LayoutPowerSelectorComponent))]
        void GetPowerSelectorDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = Ensure.NotNull<LayoutGetDrawingRegionsEvent>(eBase, "e");

            if (LayoutDrawingRegionGrid.IsComponentGridVisible(e))
                e.AddRegion(new LayoutDrawingRegionPowerSelectorConnector(e.Component, e.View));

            var textProvider = new LayoutPowerSelectorComponent.LayoutPowerSelectorNameInfo(e.Component);

            if (textProvider.Element != null && textProvider.Visible)
                e.AddRegion(new LayoutDrawingRegionText(e, textProvider));

            e.AddRegion(new LayoutDrawingRegionNotConnected(e.Component, e.View));
        }

        class LayoutDrawingRegionPowerSelectorConnector : LayoutDrawingRegionGrid {
            LayoutPowerSelectorComponent Component { get; }

            public LayoutDrawingRegionPowerSelectorConnector(ModelComponent component, ILayoutView view)
                : base(component, view) {
                this.Component = (LayoutPowerSelectorComponent)component;
            }

            private int GetPainterSwitchState() {
                if (Component.IsSwitch) {
                    if (Component.CurrentSelectedInlet != null)
                        return 1;       // Show ON
                    else
                        return 0;       // Show OFF
                }
                else {
                    var selectedInlet = Component.CurrentSelectedInlet;

                    if (selectedInlet == null)
                        return -1;      // Not connected
                    else
                        return selectedInlet == Component.Inlet1 ? (Component.ReverseLogic ? 1 : 0) : (Component.ReverseLogic ? 0 : 1);

                }
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {
                var painter = new PowerSelectorPainter() { IsSwitch = Component.IsSwitch, SwitchState = GetPainterSwitchState() };

                painter.Paint(g);
                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Track Link Component

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(LayoutTrackLinkComponent))]
        void GetTrackLinkDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = Ensure.NotNull<LayoutGetDrawingRegionsEvent>(eBase, "e");

            if (LayoutDrawingRegionGrid.IsComponentGridVisible(e))
                e.AddRegion(new LayoutDrawingRegionTrackLink(e.Component, e.View));

            LayoutTrackLinkTextInfo textProvider = new LayoutTrackLinkTextInfo(e.Component, "Name");

            if (textProvider.Element != null && textProvider.Visible)
                e.AddRegion(new LayoutDrawingRegionText(e, textProvider));
        }

        class LayoutDrawingRegionTrackLink : LayoutDrawingRegionGrid {
            readonly LayoutTrackLinkComponent component;

            public LayoutDrawingRegionTrackLink(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = (LayoutTrackLinkComponent)component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {
                LayoutTrackComponent track = component.Spot.Track;

                if (track != null) {
                    using (LayoutTrackLinkPainter painter = new LayoutTrackLinkPainter(view.GridSizeInModelCoordinates, track.ConnectionPoints)) {

                        if (component.Link == null)
                            painter.Fill = Brushes.Red;

                        painter.Paint(g);
                    }
                }
                else {
                    // If there is no track, This case should not really happend
                    using (LayoutTrackLinkPainter painter = new LayoutTrackLinkPainter(view.GridSizeInModelCoordinates,
                              Array.AsReadOnly<LayoutComponentConnectionPoint>(new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B }))) {
                        painter.Paint(g);
                    }
                }

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Text Component

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(LayoutTextComponent))]
        void GetTextDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = Ensure.NotNull<LayoutGetDrawingRegionsEvent>(eBase, "e");

            LayoutTextInfo textProvider = new LayoutTextInfo(e.Component, "Text");

            if (textProvider.Element != null)
                e.AddRegion(new LayoutDrawingRegionText(e, textProvider));
        }

        #endregion

        #region Image component

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(LayoutImageComponent))]
        private void getImageDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = Ensure.NotNull<LayoutGetDrawingRegionsEvent>(eBase, "e");
            LayoutImageInfo imageProvider = new LayoutImageInfo(e.Component.Element);
            Image? image = null;

            try {
                if (!imageProvider.ImageError)
                    image = (Image?)EventManager.Event(new LayoutEvent("get-image-from-cache", e.Component, imageProvider.ImageFile, imageProvider.ImageCacheEventXml));
            }
            catch (LayoutException lex) {
                lex.Report();
            }

            if (image == null) {
                image = imageListComponents.Images[0];
                imageProvider.ImageError = true;
            }

            e.AddRegion(new LayoutDrawingRegionImage(e.Component, image, imageProvider, e.View));
        }

        class LayoutDrawingRegionImage : LayoutDrawingRegion {
            readonly Image image;
            readonly LayoutImageInfo imageProvider;
            SizeF imageSize;
            PointF imagePosition;

            public LayoutDrawingRegionImage(ModelComponent component, Image image, LayoutImageInfo imageProvider, ILayoutView view) : base(component) {
                this.image = image;
                this.imageProvider = imageProvider;

                // Calculate the region size and position
                imageSize = new SizeF(image.Size);

                if (imageProvider.Size.Width > 0) {
                    if (imageProvider.WidthSizeUnit == LayoutImageInfo.ImageSizeUnit.Pixels)
                        imageSize.Width = imageProvider.Size.Width;
                    else
                        imageSize.Width = imageProvider.Size.Width * view.GridSizeInModelCoordinates.Width + (imageProvider.Size.Width - 1) * view.GridLineWidthInModelCoordinates;

                    if (imageProvider.Size.Height < 0)
                        imageSize.Height = imageSize.Width * (float)image.Height / (float)image.Width;
                }

                if (imageProvider.Size.Height > 0) {
                    if (imageProvider.HeightSizeUnit == LayoutImageInfo.ImageSizeUnit.Pixels)
                        imageSize.Height = imageProvider.Size.Height;
                    else
                        imageSize.Height = imageProvider.Size.Height * view.GridSizeInModelCoordinates.Height + (imageProvider.Size.Height - 1) * view.GridLineWidthInModelCoordinates;

                    if (imageProvider.Size.Width < 0)
                        imageSize.Width = imageSize.Height * (float)image.Width / (float)image.Height;
                }

                // Now calculate the position
                imagePosition = view.ModelLocationInModelCoordinates(component.Spot.Location);

                if (imageProvider.OriginMethod == LayoutImageInfo.ImageOriginMethod.Center) {
                    imagePosition.X += view.GridSizeInModelCoordinates.Width / 2;
                    imagePosition.Y += view.GridSizeInModelCoordinates.Height / 2;
                }

                if (imageProvider.HorizontalAlignment == LayoutImageInfo.ImageHorizontalAlignment.Center)
                    imagePosition.X -= imageSize.Width / 2;
                else if (imageProvider.HorizontalAlignment == LayoutImageInfo.ImageHorizontalAlignment.Right)
                    imagePosition.X -= imageSize.Width;

                if (imageProvider.VerticalAlignment == LayoutImageInfo.ImageVerticalAlignment.Middle)
                    imagePosition.Y -= imageSize.Height / 2;
                else if (imageProvider.VerticalAlignment == LayoutImageInfo.ImageVerticalAlignment.Bottom)
                    imagePosition.Y -= imageSize.Height;

                imagePosition.X += imageProvider.Offset.Width;
                imagePosition.Y += imageProvider.Offset.Height;

                BoundingRegionInModelCoordinates = new Region(new RectangleF(imagePosition, imageSize));
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {

                if (imageProvider.Size.Width < 0 && imageProvider.Size.Height < 0) {
                    // simple case, draw image in its original size
                    g.DrawImage(image, new Rectangle(new Point(0, 0), image.Size));
                }
                else {
                    if (imageProvider.FillEffect == LayoutImageInfo.ImageFillEffect.Stretch)
                        g.DrawImage(image, new RectangleF(new PointF(0, 0), imageSize));
                    else if (imageProvider.FillEffect == LayoutImageInfo.ImageFillEffect.Tile) {
                        if (imageProvider.HeightSizeUnit == LayoutImageInfo.ImageSizeUnit.GridUnits &&
                            imageProvider.WidthSizeUnit == LayoutImageInfo.ImageSizeUnit.GridUnits &&
                            imageProvider.OriginMethod == LayoutImageInfo.ImageOriginMethod.TopLeft &&
                            imageProvider.Offset.Width == 0 && imageProvider.Offset.Height == 0) {
                            // Special case for tile in grid
                            Bitmap scaledImage = new Bitmap(image, view.GridSizeInModelCoordinates);

                            for (int iy = 0; iy < imageProvider.Size.Height; iy++) {
                                for (int ix = 0; ix < imageProvider.Size.Width; ix++) {
                                    g.DrawImage(scaledImage, ix * view.GridSizeInModelCoordinates.Width + (ix - 1) * view.GridLineWidthInModelCoordinates,
                                        iy * view.GridSizeInModelCoordinates.Height + (iy - 1) * view.GridLineWidthInModelCoordinates);
                                }
                            }
                        }
                        else {
                            for (int y = 0; y < imageSize.Height; y += image.Height) {
                                for (int x = 0; x < imageSize.Width; x += image.Width) {
                                    g.DrawImage(image, new Rectangle(new Point(x, y), new Size(image.Width, image.Height)));
                                }
                            }
                        }
                    }
                    else
                        throw new ArgumentException("Invalid fill effect");
                }

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }



        #endregion

        #region Bridge component

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(LayoutBridgeComponent))]
        void GetBridgeDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = Ensure.NotNull<LayoutGetDrawingRegionsEvent>(eBase, "e");

            if (LayoutDrawingRegionGrid.IsComponentGridVisible(e))
                e.AddRegion(new LayoutDrawingRegionBridge(e.Component, e.View));
        }

        class LayoutDrawingRegionBridge : LayoutDrawingRegionGrid {
            readonly LayoutBridgeComponent component;

            public LayoutDrawingRegionBridge(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = (LayoutBridgeComponent)component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {
                if (component.Track != null) {
                    LayoutBridgePainter painter = new LayoutBridgePainter(view.GridSizeInModelCoordinates, component.Track.ConnectionPoints);

                    painter.Paint(g);
                }
                else {
                    // If there is no track, paint a large contact in the middle of the component. This case should
                    // not really happend
                    LayoutBridgePainter painter = new LayoutBridgePainter(view.GridSizeInModelCoordinates,
                        new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B });

                    painter.Paint(g);
                }

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Tunnel component

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(LayoutTunnelComponent))]
        void GetTunnelDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = Ensure.NotNull<LayoutGetDrawingRegionsEvent>(eBase, "e");

            if (LayoutDrawingRegionGrid.IsComponentGridVisible(e))
                e.AddRegion(new LayoutDrawingRegionTunnel(e.Component, e.View));
        }

        class LayoutDrawingRegionTunnel : LayoutDrawingRegionGrid {
            readonly LayoutTunnelComponent component;

            public LayoutDrawingRegionTunnel(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = (LayoutTunnelComponent)component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {
                if (component.Track != null) {
                    LayoutTunnelPainter painter = new LayoutTunnelPainter(view.GridSizeInModelCoordinates, component.Track.ConnectionPoints);

                    painter.Paint(g);
                }
                else {
                    // If there is no track, paint a large contact in the middle of the component. This case should
                    // not really happend
                    LayoutTunnelPainter painter = new LayoutTunnelPainter(view.GridSizeInModelCoordinates,
                        new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B });

                    painter.Paint(g);
                }

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Gate Component

        [LayoutEvent("get-connection-point-component-image", SenderType = typeof(LayoutGateComponent))]
        private void getGateConnectionPointImage(LayoutEvent e) {

            if (e.Sender is LayoutGateComponent)
                e.Info = imageListConnectionPointImages.Images[10];
        }

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(LayoutGateComponent))]
        void GetGatelDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = Ensure.NotNull<LayoutGetDrawingRegionsEvent>(eBase, "e");

            if (LayoutDrawingRegionGrid.IsComponentGridVisible(e))
                e.AddRegion(new LayoutDrawingRegionGate(e.Component, e.View));

            LayoutTextInfo nameProvider = new LayoutTextInfo(e.Component);

            if (nameProvider.Element != null && nameProvider.Visible)
                e.AddRegion(new LayoutDrawingRegionText(e, nameProvider));

            e.AddRegion(new LayoutDrawingRegionNotConnected(e.Component, e.View));
        }

        class LayoutDrawingRegionGate : LayoutDrawingRegionGrid {
            readonly LayoutGateComponent component;

            public LayoutDrawingRegionGate(ModelComponent component, ILayoutView view)
                : base(component, view) {
                this.component = (LayoutGateComponent)component;
            }

            int GetClearance() {
                if (LayoutController.IsOperationMode) {
                    switch (component.GateState) {
                        case LayoutGateState.Open: return 100;
                        case LayoutGateState.Close: return 0;
                        case LayoutGateState.Opening: return 70;
                        case LayoutGateState.Closing: return 30;
                        default: return 60;
                    }
                }
                else
                    return 60;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {
                if (component.Track != null) {
                    LayoutGatePainter painter = new LayoutGatePainter(view.GridSizeInModelCoordinates, LayoutStraightTrackComponent.IsVertical(component.Track),
                        component.Info.OpenUpOrLeft, GetClearance());

                    painter.Paint(g);
                }
                else {
                    // If there is no track, paint a large contact in the middle of the component. This case should
                    // not really happen
                    LayoutGatePainter painter = new LayoutGatePainter(view.GridSizeInModelCoordinates, false, false, 60);

                    painter.Paint(g);
                }

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Control Module Location component

        [LayoutEvent("get-image", SenderType = typeof(ControlModuleLocationPainter))]
        private void getControlModuleLocationImage(LayoutEvent e) {
            e.Info = imageListComponents.Images[1];
        }

        [LayoutEvent("get-model-component-drawing-regions", SenderType = typeof(LayoutControlModuleLocationComponent))]
        void GetControlModuleLocationDrawingRegions(LayoutEvent eBase) {
            LayoutGetDrawingRegionsEvent e = Ensure.NotNull<LayoutGetDrawingRegionsEvent>(eBase, "e");

            if (LayoutDrawingRegionGrid.IsComponentGridVisible(e))
                e.AddRegion(new LayoutDrawingRegionControlModuleLocation(e.Component, e.View));

            LayoutTextInfo nameProvider = new LayoutTextInfo(e.Component);

            if (nameProvider.Element != null && nameProvider.Visible)
                e.AddRegion(new LayoutDrawingRegionText(e, nameProvider));
        }

        class LayoutDrawingRegionControlModuleLocation : LayoutDrawingRegionGrid {
            public LayoutDrawingRegionControlModuleLocation(ModelComponent component, ILayoutView view) : base(component, view) {
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {
                ControlModuleLocationPainter painter = new ControlModuleLocationPainter();

                painter.Paint(g);

                base.Draw(view, detailLevel, selectionLook, g);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ComponentViews));
            this.imageListComponents = new ImageList(this.components);
            this.imageListSignals = new ImageList(this.components);
            this.imageListConnectionPointImages = new ImageList(this.components);
            // 
            // imageListComponents
            // 
            this.imageListComponents.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListComponents.ImageStream")));
            this.imageListComponents.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListComponents.Images.SetKeyName(0, "");
            this.imageListComponents.Images.SetKeyName(1, "");
            this.imageListComponents.Images.SetKeyName(2, "");
            // 
            // imageListSignals
            // 
            this.imageListSignals.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListSignals.ImageStream")));
            this.imageListSignals.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListSignals.Images.SetKeyName(0, "");
            this.imageListSignals.Images.SetKeyName(1, "");
            this.imageListSignals.Images.SetKeyName(2, "");
            this.imageListSignals.Images.SetKeyName(3, "");
            this.imageListSignals.Images.SetKeyName(4, "");
            this.imageListSignals.Images.SetKeyName(5, "");
            // 
            // imageListConnectionPointImages
            // 
            this.imageListConnectionPointImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListConnectionPointImages.ImageStream")));
            this.imageListConnectionPointImages.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListConnectionPointImages.Images.SetKeyName(0, "");
            this.imageListConnectionPointImages.Images.SetKeyName(1, "");
            this.imageListConnectionPointImages.Images.SetKeyName(2, "");
            this.imageListConnectionPointImages.Images.SetKeyName(3, "");
            this.imageListConnectionPointImages.Images.SetKeyName(4, "");
            this.imageListConnectionPointImages.Images.SetKeyName(5, "");
            this.imageListConnectionPointImages.Images.SetKeyName(6, "ConnectionToDoubleSlip.ico");
            this.imageListConnectionPointImages.Images.SetKeyName(7, "ConnectionToThreeWayTurnout.ico");
            this.imageListConnectionPointImages.Images.SetKeyName(8, "ImagePowerSelectorConnectionPoint.bmp");
            this.imageListConnectionPointImages.Images.SetKeyName(9, "ImagePowerSwitchConnectionPoint.bmp");
            this.imageListConnectionPointImages.Images.SetKeyName(10, "ImageGateConnectionPoint.bmp");

        }
        #endregion
    }
}
