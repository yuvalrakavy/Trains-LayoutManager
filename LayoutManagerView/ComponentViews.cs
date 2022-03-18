using LayoutManager.Components;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

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

        [DispatchTarget]
        private ImageList GetComponentsImageList() => imageListComponents;

        #endregion

        #region Track Components

        #region Straight Track

        [DispatchTarget]
        void GetModelComponentDrawingRegions_StraightTrack([DispatchFilter] LayoutStraightTrackComponent component, LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions))
                regions.AddRegion(new LayoutDrawingRegionStraightTrack(regions.Component, regions.View));
        }

        private class LayoutDrawingRegionStraightTrack : LayoutDrawingRegionGrid {
            private readonly LayoutStraightTrackComponent component;

            public LayoutDrawingRegionStraightTrack(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = (LayoutStraightTrackComponent)component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                var trackSegment = new TrackSegment(component, component.ConnectionPoints);
                TrackColors colors = view.GetTrackSegmentColor(trackSegment);

                LayoutStraightTrackPainter painter = new(view.GridSizeInModelCoordinates, trackSegment.ConnectionPoints) {
                    TrackColor = colors.Color(0),
                    TrackColor2 = colors.Color(1)
                };
                painter.Paint(g);

                if (colors.Annotations != null) {
                    GraphicsState gs = g.Save();

                    g.TranslateTransform(view.GridSizeInModelCoordinates.Width / 2, view.GridSizeInModelCoordinates.Height / 2);
                    SizeF minSize = new((float)view.GridSizeInModelCoordinates.Width / 3, view.GridSizeInModelCoordinates.Height / 4);

                    foreach (RoutePreviewAnnotation annotation in colors.Annotations) {
                        LocomotivePainter locoPainter = new() {
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

        [DispatchTarget]
        void GetModelComponentDrawingRegions_DoubleTrack([DispatchFilter] LayoutDoubleTrackComponent component, LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions))
                regions.AddRegion(new LayoutDrawingRegionDoubleTrack(regions.Component, regions.View));
        }

        private class LayoutDrawingRegionDoubleTrack : LayoutDrawingRegionGrid {
            private readonly LayoutDoubleTrackComponent component;

            public LayoutDrawingRegionDoubleTrack(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = (LayoutDoubleTrackComponent)component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                TrackSegment segment1 = new(component, component.GetTrackPath(0));
                TrackSegment segment2 = new(component, component.GetTrackPath(1));
                TrackColors trackColors1 = view.GetTrackSegmentColor(segment1);
                TrackColors trackColors2 = view.GetTrackSegmentColor(segment2);

                LayoutStraightTrackPainter painter = new(view.GridSizeInModelCoordinates, segment1.ConnectionPoints) {
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

        [DispatchTarget]
        private Image? GetConnectionPointComponentImage_Track([DispatchFilter] LayoutTurnoutTrackComponent component, ControlModule module, bool topRow, int index) => imageListConnectionPointImages.Images[0];

        [DispatchTarget]
        void GetModelComponentDrawingRegions_TurnoutTrack([DispatchFilter] LayoutTurnoutTrackComponent component, LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions))
                regions.AddRegion(new LayoutDrawingRegionTurnoutTrack(regions.Component, regions.View));

            regions.AddRegion(new LayoutDrawingRegionNotConnected(regions.Component, regions.View));
        }

        private class LayoutDrawingRegionTurnoutTrack : LayoutDrawingRegionGrid {
            private readonly LayoutTurnoutTrackComponent component;

            public LayoutDrawingRegionTurnoutTrack(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = (LayoutTurnoutTrackComponent)component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                TrackSegment straightSegment = new(component, component.Tip, component.Straight);
                TrackSegment branchSegment = new(component, component.Tip, component.Branch);
                LayoutTurnoutTrackPainter painter = new(view.GridSizeInModelCoordinates,
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

        [DispatchTarget]
        private Image? GetConnectionPointComponentImage_ThreeWayTurnout([DispatchFilter] LayoutThreeWayTurnoutComponent component, ControlModule module, bool topRow, int index) => imageListConnectionPointImages.Images[7];

        [DispatchTarget]
        void GetModelComponentDrawingRegions_ThreewayTurnoutTrack([DispatchFilter] LayoutThreeWayTurnoutComponent component, LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions))
                regions.AddRegion(new LayoutDrawingRegionThreeWayTurnoutTrack(regions.Component, regions.View));

            regions.AddRegion(new LayoutDrawingRegionNotConnected(regions.Component, regions.View));
        }

        private class LayoutDrawingRegionThreeWayTurnoutTrack : LayoutDrawingRegionGrid {
            private readonly LayoutThreeWayTurnoutComponent component;

            public LayoutDrawingRegionThreeWayTurnoutTrack(ModelComponent component, ILayoutView view)
                : base(component, view) {
                this.component = (LayoutThreeWayTurnoutComponent)component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                Color[] segmentColors = new Color[3];

                for (int segment = 0; segment < 3; segment++) {
                    TrackSegment trackSegment = new(component, component.Tip, component.ConnectTo(component.Tip, segment));
                    segmentColors[segment] = view.GetTrackSegmentColor(trackSegment).Color(0);
                }

                LayoutThreeWayTurnoutPainter painter = new(view.GridSizeInModelCoordinates, component.Tip, component.CurrentSwitchState) {
                    SegmentColors = segmentColors
                };
                painter.Paint(g);
                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Double slip Track Component

        [DispatchTarget]
        private Image? GetConnectionPointComponentImage_DoubleSlip([DispatchFilter] LayoutDoubleSlipTrackComponent component, ControlModule module, bool topRow, int index) => imageListConnectionPointImages.Images[6];

        [DispatchTarget]
        void GetModelComponentDrawingRegions_DoubleSlipTrack([DispatchFilter] LayoutDoubleSlipTrackComponent component, LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions))
                regions.AddRegion(new LayoutDrawingRegionDoubleSlipTrack(regions.Component, regions.View));

            regions.AddRegion(new LayoutDrawingRegionNotConnected(regions.Component, regions.View));
        }

        private class LayoutDrawingRegionDoubleSlipTrack : LayoutDrawingRegionGrid {
            private readonly LayoutDoubleSlipTrackComponent component;

            public LayoutDrawingRegionDoubleSlipTrack(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = (LayoutDoubleSlipTrackComponent)component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                TrackSegment horizontalSegment = new(component, LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.R);
                TrackSegment verticalSegment = new(component, LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B);
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

                LayoutDoubleSlipPainter painter = new(view.GridSizeInModelCoordinates, component.DiagonalIndex, component.CurrentSwitchState) {
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

        [DispatchTarget]
        private Image? GetConnectionPointComponentImage_TrackContact([DispatchFilter] LayoutTrackContactComponent component, ControlModule module, bool topRow, int index) => imageListConnectionPointImages.Images[1];

        [DispatchTarget]
        private Image? GetConnectionPointComponentImage_ProximitySeonsor([DispatchFilter] LayoutProximitySensorComponent component, ControlModule module, bool topRow, int index) => imageListConnectionPointImages.Images[1];

        private void DoGetRegions(LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions))
                regions.AddRegion(new LayoutDrawingRegionTriggerableBlockEdge(regions.Component, regions.View));

            var nameProvider = new LayoutTextInfo(regions.Component);

            if (nameProvider.OptionalElement != null && nameProvider.Visible)
                regions.AddRegion(new LayoutDrawingRegionText(regions, nameProvider));

            regions.AddRegion(new LayoutDrawingRegionNotConnected(regions.Component, regions.View));
        }

        [DispatchTarget]
        void GetModelComponentDrawingRegions_TrackContact([DispatchFilter] LayoutTrackContactComponent component, LayoutGetDrawingRegions regions) => DoGetRegions(regions);

        [DispatchTarget]
        void GetModelComponentDrawingRegions_ProximitySensor([DispatchFilter] LayoutProximitySensorComponent component, LayoutGetDrawingRegions regions) => DoGetRegions(regions);

        private class LayoutDrawingRegionTriggerableBlockEdge : LayoutDrawingRegionGrid {
            private readonly LayoutTriggerableBlockEdgeBase component;

            public LayoutDrawingRegionTriggerableBlockEdge(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = (LayoutTriggerableBlockEdgeBase)component;
            }

            public LayoutTriggerableBlockEdgePainter.ComponentType PainterComponentType => this.component switch {
                LayoutTrackContactComponent _ => LayoutTriggerableBlockEdgePainter.ComponentType.TrackContact,
                LayoutProximitySensorComponent sensor when !sensor.IsTriggered => LayoutTriggerableBlockEdgePainter.ComponentType.ProximitySensor,
                LayoutProximitySensorComponent sensor when sensor.IsTriggered => LayoutTriggerableBlockEdgePainter.ComponentType.ActiveProximitySensor,
                _ => throw new NotImplementedException()
            };

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
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
                    using LayoutTriggerableBlockEdgePainter painter = new(componentType: PainterComponentType, componentSize: view.GridSizeInModelCoordinates,
                        cp: new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B }) {
                        ContactSize = new Size(12, 12)
                    };
                    painter.Paint(g);
                }
                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Occupancy Block Edge Component

        [DispatchTarget]
        void GetModelComponentDrawingRegions_BlockEdge([DispatchFilter] LayoutBlockEdgeComponent component, LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions))
                regions.AddRegion(new LayoutDrawingRegionOccpancyBlockEdge(regions.Component, regions.View));
        }

        private class LayoutDrawingRegionOccpancyBlockEdge : LayoutDrawingRegionGrid {
            private readonly LayoutBlockEdgeComponent component;

            public LayoutDrawingRegionOccpancyBlockEdge(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = (LayoutBlockEdgeComponent)component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                if (component.OptionalTrack != null) {
                    using LayoutBlockEdgePainter painter = new(view.GridSizeInModelCoordinates, component.Track.ConnectionPoints);
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
                else {
                    // If there is no track, paint a large contact in the middle of the component. This case should
                    // not really happend
                    using LayoutBlockEdgePainter painter = new(view.GridSizeInModelCoordinates,
                              new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B }) {
                        ContactSize = new Size(12, 12)
                    };
                    painter.Paint(g);
                }

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Block Info Component

        [DispatchTarget]
        private Image? GetConnectionPointComponentImage_BlockDefinition([DispatchFilter] LayoutBlockDefinitionComponent component, ControlModule module, bool topRow, int index) => imageListConnectionPointImages.Images[2];

        [DispatchTarget]
        void GetModelComponentDrawingRegions_BlockDefinition([DispatchFilter] LayoutBlockDefinitionComponent component, LayoutGetDrawingRegions regions) {
            var blockDefinition = (LayoutBlockDefinitionComponent)regions.Component;

            if (LayoutBlockBalloon.IsDisplayed(blockDefinition))
                regions.AddRegion(new LayoutDrawingRegionBalloonInfo(regions.Component, regions.View, regions.Graphics, LayoutBlockBalloon.Get(blockDefinition)));

            if (LayoutController.IsOperationMode && blockDefinition.OptionalBlock != null && blockDefinition.OptionalBlock.HasTrains) {
                IList<TrainLocationInfo> trainLocations = blockDefinition.Block.Trains;
                TrainLocationPainter[] painters = new TrainLocationPainter[trainLocations.Count];
                int i;
                SizeF totalSize = new(0, 0);
                int gap = 4;
                bool trainDetected = blockDefinition.Info.TrainDetected;
                bool vertical = LayoutTrackComponent.IsVertical(trainLocations[0].DisplayFront);

                for (i = 0; i < trainLocations.Count; i++) {
                    painters[i] = new TrainLocationPainter(trainLocations[i], regions.DetailLevel);

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

                    SizeF locoSize = painters[i].Measure(regions.Graphics);

                    if (locoSize.Height > totalSize.Height)
                        totalSize.Height = locoSize.Height;

                    if (totalSize.Width > 0)
                        totalSize.Width += gap;
                    totalSize.Width += locoSize.Width;
                }

                PointF gridLocation = regions.View.ModelLocationInModelCoordinates(blockDefinition.Location);

                if (vertical) {
                    PointF origin = new(gridLocation.X + (regions.View.GridSizeInModelCoordinates.Width / 2) - (totalSize.Height / 2),
                        gridLocation.Y + (regions.View.GridSizeInModelCoordinates.Height / 2) - (totalSize.Height / 2) - (totalSize.Width / 2));

                    for (i = 0; i < painters.Length; i++) {
                        SizeF locoSize = painters[i].Measure(regions.Graphics);

                        regions.AddRegion(new LayoutDrawingRegionLocomotive(blockDefinition, origin, locoSize, vertical, painters[i]));
                        origin.Y += locoSize.Width;
                        if (i > 0)
                            origin.Y += gap;
                    }
                }
                else {  // Either horizontal or diagonal
                        // Start adding the regions
                    PointF origin = new(gridLocation.X + (regions.View.GridSizeInModelCoordinates.Width / 2) - (totalSize.Width / 2), gridLocation.Y + (regions.View.GridSizeInModelCoordinates.Height / 2) - (totalSize.Height / 2));

                    for (i = 0; i < painters.Length; i++) {
                        SizeF locoSize = painters[i].Measure(regions.Graphics);

                        regions.AddRegion(new LayoutDrawingRegionLocomotive(blockDefinition, origin, locoSize, vertical, painters[i]));
                        origin.X += locoSize.Width;
                        if (i > 0)
                            origin.X += gap;
                    }
                }
            }
            else {
                if (LayoutController.IsOperationMode && blockDefinition.Info.UnexpectedTrainDetected) {
                    LocomotivePainter locoPainter = new() {
                        Label = " ? ",
                        DrawFront = false,
                        BackgroundBrush = Brushes.DarkViolet,
                        LabelBrush = Brushes.Yellow,
                        MinSize = new SizeF(50, 12),
                        Front = blockDefinition.Track.ConnectionPoints[0]
                    };

                    Size totalSize = locoPainter.Measure(regions.Graphics);
                    PointF gridLocation = regions.View.ModelLocationInModelCoordinates(blockDefinition.Location);

                    if (LayoutTrackComponent.IsVertical(blockDefinition.Track.ConnectionPoints[0])) {
                        PointF origin = new(gridLocation.X + (regions.View.GridSizeInModelCoordinates.Width / 2) - (totalSize.Height / 2),
                            gridLocation.Y + (regions.View.GridSizeInModelCoordinates.Height / 2) - (totalSize.Height / 2) - (totalSize.Width / 2));

                        regions.AddRegion(new LayoutDrawingRegionLocomotive(blockDefinition, origin, totalSize, true, locoPainter));
                    }
                    else {
                        PointF origin = new(gridLocation.X + (regions.View.GridSizeInModelCoordinates.Width / 2) - (totalSize.Width / 2), gridLocation.Y + (regions.View.GridSizeInModelCoordinates.Height / 2) - (totalSize.Height / 2));

                        regions.AddRegion(new LayoutDrawingRegionLocomotive(blockDefinition, origin, totalSize, false, locoPainter));
                    }
                }
                else if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions)) {
                    regions.AddRegion(new LayoutDrawingRegionBlockInfo(regions.Component, regions.View));

                    if (blockDefinition.Info.IsOccupancyDetectionBlock)
                        regions.AddRegion(new LayoutDrawingRegionNotConnected(regions.Component, regions.View));
                }
            }

            LayoutTextInfo nameProvider = new(regions.Component);

            if (nameProvider.Element != null && nameProvider.Visible)
                regions.AddRegion(new LayoutDrawingRegionText(regions, nameProvider));
        }

        private class LayoutDrawingRegionLocomotive : LayoutDrawingRegion {
            private readonly ILayoutComponentPainter painter;

            public LayoutDrawingRegionLocomotive(LayoutBlockDefinitionComponent blockInfo, PointF origin, SizeF locoSize, bool vertical, ILayoutComponentPainter painter) : base(blockInfo) {
                this.painter = painter;

                if (vertical) {
                    SizeF regionSize = new(locoSize.Height + 4, locoSize.Width + 4);

                    BoundingRegionInModelCoordinates = new Region(new RectangleF(new PointF(origin.X - 2, origin.Y - 2), regionSize));
                }
                else {
                    SizeF regionSize = new(locoSize.Width + 4, locoSize.Height + 4);

                    BoundingRegionInModelCoordinates = new Region(new RectangleF(new PointF(origin.X - 2, origin.Y - 2), regionSize));
                }
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                GraphicsState gs = g.Save();
                RectangleF bbox = BoundingRegionInModelCoordinates.GetBounds(g);

                g.TranslateTransform(bbox.Width / 2.0F, bbox.Height / 2.0F);
                painter.Draw(g);
                g.Restore(gs);

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        private class LayoutDrawingRegionBlockInfo : LayoutDrawingRegionGrid {
            private readonly LayoutBlockDefinitionComponent component;

            public LayoutDrawingRegionBlockInfo(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = (LayoutBlockDefinitionComponent)component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                if (component.Track != null) {
                    using LayoutBlockInfoPainter painter = new(view.GridSizeInModelCoordinates, component.Track.ConnectionPoints);
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
                else {
                    // If there is no track, paint a large contact in the middle of the component. This case should
                    // not really happend
                    using LayoutBlockInfoPainter painter = new(view.GridSizeInModelCoordinates,
                              new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B }) {
                        InfoBoxSize = new Size(12, 12)
                    };
                    painter.Paint(g);
                }
                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Signal Component

        [DispatchTarget]
        private Image? GetConnectionPointComponentImage_Signal([DispatchFilter] LayoutSignalComponent component, ControlModule module, bool topRow, int index) {
            return component.Info.SignalType switch {
                LayoutSignalType.Lights => imageListConnectionPointImages.Images[3],
                LayoutSignalType.Semaphore => imageListConnectionPointImages.Images[4],
                LayoutSignalType.Distance => imageListConnectionPointImages.Images[5],
                _ => null,
            };
        }

        [DispatchTarget]
        void GetModelComponentDrawingRegions_Signal([DispatchFilter] LayoutSignalComponent component, LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions)) {
                regions.AddRegion(new LayoutDrawingRegionSignal(regions.Component, regions.View, regions.DetailLevel, imageListSignals));
            }

            regions.AddRegion(new LayoutDrawingRegionNotConnected(regions.Component, regions.View, regions.DetailLevel != ViewDetailLevel.High));
        }

        private class LayoutDrawingRegionSignal : LayoutDrawingRegion {
            private readonly bool vertical;
            private readonly LayoutSignalType signalType;
            private readonly LayoutSignalState state;
            private readonly ImageList imageListSignals;
            private readonly bool showNotLinked;

            private static Dictionary<Guid, LayoutBlockEdgeBase>? linkedSignalMap = null;

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
                        linkedSignalMap = Dispatch.Call.GetLinkedSignalMap();

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

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                int imageIndex = 0;

                if (detailLevel == ViewDetailLevel.High) {
                    if (state == LayoutSignalState.Green)
                        imageIndex = 1;

                    if (signalType == LayoutSignalType.Semaphore)
                        imageIndex += 2;
                    else if (signalType == LayoutSignalType.Distance)
                        imageIndex += 4;

                    Rectangle imageRect = new(new Point(1, 1), imageListSignals.ImageSize);

                    g.DrawImage(imageListSignals.Images[imageIndex], imageRect);

                    if (showNotLinked)
                        using (Pen p = new(Color.Red, 2))
                            g.DrawEllipse(p, imageRect);
                }
                else {
                    int size = detailLevel == ViewDetailLevel.Medium ? 10 : 14;
                    Point[] points = new Point[] { new Point(16, 16 - size), new Point(16 - size, 16), new Point(16, 16 + size), new Point(16 + size, 16) };
                    Brush fillBrush;

                    if (LayoutController.IsOperationMode) {
                        fillBrush = state switch {
                            LayoutSignalState.Green => Brushes.LightGreen,
                            LayoutSignalState.Red => Brushes.Red,
                            _ => Brushes.Yellow,
                        };
                    }
                    else
                        fillBrush = Brushes.Yellow;

                    g.FillPolygon(fillBrush, points);

                    if (detailLevel == ViewDetailLevel.Medium)
                        g.DrawPolygon(Pens.Black, points);

                    if (showNotLinked)
                        using (Pen p = new(Color.Red, 3)) {
                            int d = size + 2;

                            g.DrawEllipse(p, new Rectangle(new Point(16 - d, 16 - d), new Size(2 * d, 2 * d)));
                        }
                }

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Track Isolation component

        [DispatchTarget]
        void GetModelComponentDrawingRegions_TrackIsolation([DispatchFilter] LayoutTrackIsolationComponent component, LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions))
                regions.AddRegion(new LayoutDrawingRegionTrackIsolation(regions.Component, regions.View));
        }

        private class LayoutDrawingRegionTrackIsolation : LayoutDrawingRegionGrid {
            private readonly ModelComponent component;

            public LayoutDrawingRegionTrackIsolation(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                if (component.Spot.Track is LayoutStraightTrackComponent track) {
                    LayoutTrackIsolationPainter painter = new(view.GridSizeInModelCoordinates, track.ConnectionPoints);

                    painter.Paint(g);
                }

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Track reverse loop moduole component

        [DispatchTarget]
        void GetModelComponentDrawingRegions_TrackReverseLoopModule([DispatchFilter] LayoutTrackReverseLoopModule component, LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions))
                regions.AddRegion(new LayoutDrawingRegionTrackReverseLoopModule(regions.Component, regions.View));
        }

        private class LayoutDrawingRegionTrackReverseLoopModule : LayoutDrawingRegionGrid {
            private readonly ModelComponent component;

            public LayoutDrawingRegionTrackReverseLoopModule(ModelComponent component, ILayoutView view)
                : base(component, view) {
                this.component = component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                if (component.Spot.Track is LayoutStraightTrackComponent track) {
                    var painter = new LayoutTrackReverseLoopModulePainter(view.GridSizeInModelCoordinates, track.ConnectionPoints);

                    painter.Paint(g);
                }

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Track Power connector

        [DispatchTarget]
        void GetModelComponentDrawingRegions_TrackPowerConnector([DispatchFilter] LayoutTrackPowerConnectorComponent component, LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions))
                regions.AddRegion(new LayoutDrawingRegionTrackPowerConnector(regions.Component, regions.View));

            LayoutTrackPowerConnectorInfo textProvider = new(regions.Component);

            if (textProvider.Element != null && textProvider.Visible)
                regions.AddRegion(new LayoutDrawingRegionText(regions, textProvider));
        }

        private class LayoutDrawingRegionTrackPowerConnector : LayoutDrawingRegionGrid {
            private readonly ModelComponent component;

            public LayoutDrawingRegionTrackPowerConnector(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                if (component.Spot.Track is LayoutStraightTrackComponent track) {
                    using LayoutPowerConnectorPainter painter = new(view.GridSizeInModelCoordinates, track.ConnectionPoints);
                    painter.Paint(g);
                }

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Power Selector/Switch component

        [DispatchTarget]
        private Image? GetConnectionPointComponentImage_PowerSelector([DispatchFilter] LayoutPowerSelectorComponent component, ControlModule module, bool topRow, int index) {
            if (component is LayoutPowerSelectorComponent selector && selector.IsSwitch)
                return imageListConnectionPointImages.Images[9];
            else
                return imageListConnectionPointImages.Images[8];
        }

        [DispatchTarget]
        void GetModelComponentDrawingRegions_TrackPowerSelector([DispatchFilter] LayoutPowerSelectorComponent component, LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions))
                regions.AddRegion(new LayoutDrawingRegionPowerSelectorConnector(regions.Component, regions.View));

            var textProvider = new LayoutPowerSelectorComponent.LayoutPowerSelectorNameInfo(regions.Component);

            if (textProvider.Element != null && textProvider.Visible)
                regions.AddRegion(new LayoutDrawingRegionText(regions, textProvider));

            regions.AddRegion(new LayoutDrawingRegionNotConnected(regions.Component, regions.View));
        }

        private class LayoutDrawingRegionPowerSelectorConnector : LayoutDrawingRegionGrid {
            private LayoutPowerSelectorComponent Component { get; }

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

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                var painter = new PowerSelectorPainter() { IsSwitch = Component.IsSwitch, SwitchState = GetPainterSwitchState() };

                painter.Paint(g);
                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Track Link Component

        [DispatchTarget]
        void GetModelComponentDrawingRegions_TrackLink([DispatchFilter] LayoutTrackLinkComponent component, LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions))
                regions.AddRegion(new LayoutDrawingRegionTrackLink(regions.Component, regions.View));

            LayoutTrackLinkTextInfo textProvider = new(regions.Component, "Name");

            if (textProvider.Element != null && textProvider.Visible)
                regions.AddRegion(new LayoutDrawingRegionText(regions, textProvider));
        }

        private class LayoutDrawingRegionTrackLink : LayoutDrawingRegionGrid {
            private readonly LayoutTrackLinkComponent component;

            public LayoutDrawingRegionTrackLink(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = (LayoutTrackLinkComponent)component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                var track = component.Spot.Track;

                if (track != null) {
                    using LayoutTrackLinkPainter painter = new(view.GridSizeInModelCoordinates, track.ConnectionPoints);
                    if (component.Link == null)
                        painter.Fill = Brushes.Red;

                    painter.Paint(g);
                }
                else {
                    // If there is no track, This case should not really happend
                    using LayoutTrackLinkPainter painter = new(view.GridSizeInModelCoordinates,
                              Array.AsReadOnly<LayoutComponentConnectionPoint>(new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B }));
                    painter.Paint(g);
                }

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Text Component

        [DispatchTarget]
        void GetModelComponentDrawingRegions_Text([DispatchFilter] LayoutTextComponent component, LayoutGetDrawingRegions regions) {
            LayoutTextInfo textProvider = new(regions.Component, "Text");

            if (textProvider.Element != null)
                regions.AddRegion(new LayoutDrawingRegionText(regions, textProvider));
        }

        #endregion

        #region Image component

        [DispatchTarget]
        void GetModelComponentDrawingRegions_Image([DispatchFilter] LayoutImageComponent component, LayoutGetDrawingRegions regions) {
            LayoutImageInfo imageProvider = new(regions.Component.Element);
            Image? image = null;

            try {
                if (!imageProvider.ImageError)
                    image = Dispatch.Call.GetImageFromCache(regions.Component, imageProvider.ImageFile, imageProvider.RotateFlipEffect);
            }
            catch (LayoutException lex) {
                lex.Report();
            }

            if (image == null) {
                image = imageListComponents.Images[0];
                imageProvider.ImageError = true;
            }

            regions.AddRegion(new LayoutDrawingRegionImage(regions.Component, image, imageProvider, regions.View));
        }

        private class LayoutDrawingRegionImage : LayoutDrawingRegion {
            private readonly Image image;
            private readonly LayoutImageInfo imageProvider;
            private SizeF imageSize;
            private PointF imagePosition;

            public LayoutDrawingRegionImage(ModelComponent component, Image image, LayoutImageInfo imageProvider, ILayoutView view) : base(component) {
                this.image = image;
                this.imageProvider = imageProvider;

                // Calculate the region size and position
                imageSize = new SizeF(image.Size);

                if (imageProvider.Size.Width > 0) {
                    if (imageProvider.WidthSizeUnit == LayoutImageInfo.ImageSizeUnit.Pixels)
                        imageSize.Width = imageProvider.Size.Width;
                    else
                        imageSize.Width = (imageProvider.Size.Width * view.GridSizeInModelCoordinates.Width) + ((imageProvider.Size.Width - 1) * view.GridLineWidthInModelCoordinates);

                    if (imageProvider.Size.Height < 0)
                        imageSize.Height = imageSize.Width * (float)image.Height / (float)image.Width;
                }

                if (imageProvider.Size.Height > 0) {
                    if (imageProvider.HeightSizeUnit == LayoutImageInfo.ImageSizeUnit.Pixels)
                        imageSize.Height = imageProvider.Size.Height;
                    else
                        imageSize.Height = (imageProvider.Size.Height * view.GridSizeInModelCoordinates.Height) + ((imageProvider.Size.Height - 1) * view.GridLineWidthInModelCoordinates);

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

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
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
                            var scaledImage = new Bitmap(image, view.GridSizeInModelCoordinates);

                            for (int iy = 0; iy < imageProvider.Size.Height; iy++) {
                                for (int ix = 0; ix < imageProvider.Size.Width; ix++) {
                                    g.DrawImage(scaledImage, (ix * view.GridSizeInModelCoordinates.Width) + ((ix - 1) * view.GridLineWidthInModelCoordinates),
                                        (iy * view.GridSizeInModelCoordinates.Height) + ((iy - 1) * view.GridLineWidthInModelCoordinates));
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

        [DispatchTarget]
        void GetModelComponentDrawingRegions_Bridge([DispatchFilter] LayoutBridgeComponent component, LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions))
                regions.AddRegion(new LayoutDrawingRegionBridge(regions.Component, regions.View));
        }

        private class LayoutDrawingRegionBridge : LayoutDrawingRegionGrid {
            private readonly LayoutBridgeComponent component;

            public LayoutDrawingRegionBridge(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = (LayoutBridgeComponent)component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                if (component.Track != null) {
                    LayoutBridgePainter painter = new(view.GridSizeInModelCoordinates, component.Track.ConnectionPoints);

                    painter.Paint(g);
                }
                else {
                    // If there is no track, paint a large contact in the middle of the component. This case should
                    // not really happend
                    LayoutBridgePainter painter = new(view.GridSizeInModelCoordinates,
                        new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B });

                    painter.Paint(g);
                }

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Tunnel component

        [DispatchTarget]
        void GetModelComponentDrawingRegions_Tunnle([DispatchFilter] LayoutTunnelComponent component, LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions))
                regions.AddRegion(new LayoutDrawingRegionTunnel(regions.Component, regions.View));
        }

        private class LayoutDrawingRegionTunnel : LayoutDrawingRegionGrid {
            private readonly LayoutTunnelComponent component;

            public LayoutDrawingRegionTunnel(ModelComponent component, ILayoutView view) : base(component, view) {
                this.component = (LayoutTunnelComponent)component;
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                if (component.Track != null) {
                    LayoutTunnelPainter painter = new(view.GridSizeInModelCoordinates, component.Track.ConnectionPoints);

                    painter.Paint(g);
                }
                else {
                    // If there is no track, paint a large contact in the middle of the component. This case should
                    // not really happend
                    LayoutTunnelPainter painter = new(view.GridSizeInModelCoordinates,
                        new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B });

                    painter.Paint(g);
                }

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Gate Component

        [DispatchTarget]
        private Image? GetConnectionPointComponentImage_Gate([DispatchFilter] LayoutGateComponent component, ControlModule module, bool topRow, int index) {
            return component is LayoutGateComponent ? imageListConnectionPointImages.Images[10] : null;
        }

        [DispatchTarget]
        void GetModelComponentDrawingRegions_Gate([DispatchFilter] LayoutGateComponent component, LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions))
                regions.AddRegion(new LayoutDrawingRegionGate(regions.Component, regions.View));

            LayoutTextInfo nameProvider = new(regions.Component);

            if (nameProvider.Element != null && nameProvider.Visible)
                regions.AddRegion(new LayoutDrawingRegionText(regions, nameProvider));

            regions.AddRegion(new LayoutDrawingRegionNotConnected(regions.Component, regions.View));
        }

        private class LayoutDrawingRegionGate : LayoutDrawingRegionGrid {
            private readonly LayoutGateComponent component;

            public LayoutDrawingRegionGate(ModelComponent component, ILayoutView view)
                : base(component, view) {
                this.component = (LayoutGateComponent)component;
            }

            private int GetClearance() => LayoutController.IsOperationMode ? component.GateState switch {
                LayoutGateState.Open => 100,
                LayoutGateState.Close => 0,
                LayoutGateState.Opening => 70,
                LayoutGateState.Closing => 30,
                _ => 60,
            } : 60;

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                if (component.Track != null) {
                    LayoutGatePainter painter = new(view.GridSizeInModelCoordinates, LayoutStraightTrackComponent.IsVertical(component.Track),
                        component.Info.OpenUpOrLeft, GetClearance());

                    painter.Paint(g);
                }
                else {
                    // If there is no track, paint a large contact in the middle of the component. This case should
                    // not really happen
                    LayoutGatePainter painter = new(view.GridSizeInModelCoordinates, false, false, 60);

                    painter.Paint(g);
                }

                base.Draw(view, detailLevel, selectionLook, g);
            }
        }

        #endregion

        #region Control Module Location component

        [DispatchTarget]
        private Image GetImage() {
            return imageListComponents.Images[1];
        }

        [DispatchTarget]
        void GetModelComponentDrawingRegions_ControlModuleLocation([DispatchFilter] LayoutControlModuleLocationComponent component, LayoutGetDrawingRegions regions) {
            if (LayoutDrawingRegionGrid.IsComponentGridVisible(regions))
                regions.AddRegion(new LayoutDrawingRegionControlModuleLocation(regions.Component, regions.View));

            LayoutTextInfo nameProvider = new(regions.Component);

            if (nameProvider.Element != null && nameProvider.Visible)
                regions.AddRegion(new LayoutDrawingRegionText(regions, nameProvider));
        }

        private class LayoutDrawingRegionControlModuleLocation : LayoutDrawingRegionGrid {
            public LayoutDrawingRegionControlModuleLocation(ModelComponent component, ILayoutView view) : base(component, view) {
            }

            public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
                ControlModuleLocationPainter painter = new();

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
            System.ComponentModel.ComponentResourceManager resources = new(typeof(ComponentViews));
            this.imageListComponents = new ImageList(this.components);
            this.imageListSignals = new ImageList(this.components);
            this.imageListConnectionPointImages = new ImageList(this.components);
            // 
            // imageListComponents
            // 
            this.imageListComponents.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListComponents.ImageStream")!;
            this.imageListComponents.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListComponents.Images.SetKeyName(0, "");
            this.imageListComponents.Images.SetKeyName(1, "");
            this.imageListComponents.Images.SetKeyName(2, "");
            // 
            // imageListSignals
            // 
            this.imageListSignals.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListSignals.ImageStream")!;
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
            this.imageListConnectionPointImages.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListConnectionPointImages.ImageStream")!;
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
