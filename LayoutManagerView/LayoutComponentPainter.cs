using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

using LayoutManager.Model;

namespace LayoutManager.Components {
    /// <summary>
    /// This is the base class for all "painters". A painter is a class that does the actual painting
    /// of a given component. Painters exist to seperate the painting from the model/controller/view model
    /// classes. This make it possible for example, to draw the graphics of a component as an icon on a menu.
    /// </summary>
    public class LayoutComponentPainter {
        protected LayoutComponentPainter() {
        }

        protected static Point GetConnectionPointPosition(LayoutComponentConnectionPoint p, Size boundingBox) {
            switch (p) {
                case LayoutComponentConnectionPoint.T: return new Point(boundingBox.Width / 2, 0);
                case LayoutComponentConnectionPoint.B: return new Point(boundingBox.Width / 2, boundingBox.Height);
                case LayoutComponentConnectionPoint.R: return new Point(boundingBox.Width, boundingBox.Height / 2);
                case LayoutComponentConnectionPoint.L: return new Point(0, boundingBox.Height / 2);
                default:
                    throw new ArgumentException("Invalid track connection point");
            }
        }

        protected static Point Midpoint(Point p1, Point p2) => new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
    }

    public class LayoutTrackPainter : LayoutComponentPainter {
        public Color TrackColor { get; set; } = Color.Black;

        public int TrackWidth { get; set; } = 6;
    }

    public class LayoutTrackOverlayPainter : LayoutComponentPainter {
        private Size componentSize;

        public LayoutTrackOverlayPainter(Size componentSize, IList<LayoutComponentConnectionPoint> cp) {
            Debug.Assert(cp.Count >= 2);

            this.ConnectionPoints = cp;
            this.componentSize = componentSize;
        }

        protected Size ComponentSize => componentSize;

        protected IList<LayoutComponentConnectionPoint> ConnectionPoints { get; }

        /// <summary>
        /// Center the origin to the middle of the component grid. If the component is vertical, a rotate
        /// transform is applied
        /// </summary>
        /// <param name="g"></param>
        protected void CenterOrigin(Graphics g) {
            g.TranslateTransform(componentSize.Width / 2.0F, componentSize.Height / 2.0F);

            if (LayoutTrackComponent.IsVertical(ConnectionPoints[0]))
                g.RotateTransform(90.0F);
        }

        public enum TrackOrientation {
            Horizontal,
            Vertical,
            Diagonal,
            Other
        }

        public TrackOrientation Orientation {
            get {
                if (ConnectionPoints.Count > 2)
                    return TrackOrientation.Other;
                else if (ConnectionPoints[0].IsHorizontal && ConnectionPoints[1].IsHorizontal)
                    return TrackOrientation.Horizontal;
                else return ConnectionPoints[0].IsVertical && ConnectionPoints[1].IsVertical ? TrackOrientation.Vertical : TrackOrientation.Diagonal;
            }
        }

        /// <summary>
        /// Return the center point between the two connection points
        /// </summary>
        protected Point CenterPoint {
            get {
                LayoutComponentConnectionPoint oppositeCp = LayoutTrackComponent.OppositeConnectPoint(ConnectionPoints[0]);
                LayoutComponentConnectionPoint otherCp = ConnectionPoints[1];

                if (ConnectionPoints.Count > 2) {
                    foreach (LayoutComponentConnectionPoint cpLookup in ConnectionPoints) {
                        if (cpLookup == oppositeCp) {
                            otherCp = oppositeCp;
                            break;
                        }
                    }
                }

                return Midpoint(LayoutComponentPainter.GetConnectionPointPosition(ConnectionPoints[0], componentSize),
                    LayoutComponentPainter.GetConnectionPointPosition(otherCp, componentSize));
            }
        }
    }

    /// <summary>
    /// Handle drawing of straight tracks
    /// </summary>
    public class LayoutStraightTrackPainter : LayoutTrackPainter {
        private readonly LayoutComponentConnectionPoint cp1, cp2;
        private Size componentSize;

        public LayoutStraightTrackPainter(
            Size componentSize,
            LayoutComponentConnectionPoint cp1,
            LayoutComponentConnectionPoint cp2) {
            this.cp1 = cp1;
            this.cp2 = cp2;
            this.componentSize = componentSize;
        }

        public LayoutStraightTrackPainter(
            Size componentSize, IList<LayoutComponentConnectionPoint> cps) {
            this.cp1 = cps[0];
            this.cp2 = cps[1];
            this.componentSize = componentSize;
        }

        public Color TrackColor2 { set; get; } = Color.Black;

        public void Paint(Graphics g) {
            Point cp1Position = GetConnectionPointPosition(cp1, componentSize);
            Point cp2Position = GetConnectionPointPosition(cp2, componentSize);

            using Pen p = new Pen(TrackColor, TrackWidth);
            if (TrackColor == TrackColor2)
                g.DrawLine(p, cp1Position, cp2Position);
            else {
                Point cpMiddlePosition = Midpoint(cp1Position, cp2Position);

                using Pen p2 = new Pen(TrackColor2, TrackWidth);
                g.DrawLine(p, cp1Position, cpMiddlePosition);
                g.DrawLine(p2, cpMiddlePosition, cp2Position);
            }
        }
    }

    public class LayoutTurnoutTrackPainter : LayoutTrackPainter {
        private Size componentSize;
        private readonly LayoutComponentConnectionPoint tip;
        private readonly LayoutComponentConnectionPoint straight;
        private readonly LayoutComponentConnectionPoint branch;
        private readonly LayoutComponentConnectionPoint switchPosition;

        public LayoutTurnoutTrackPainter(
            Size componentSize,
            LayoutComponentConnectionPoint tip,
            LayoutComponentConnectionPoint straight,
            LayoutComponentConnectionPoint branch,
            LayoutComponentConnectionPoint switchPosition) {
            this.componentSize = componentSize;
            this.tip = tip;
            this.straight = straight;
            this.branch = branch;
            this.switchPosition = switchPosition;

            if (switchPosition != LayoutComponentConnectionPoint.Empty && switchPosition != straight && switchPosition != branch)
                throw new ArgumentException("Invalid switch position");
        }

        public Color BranchColor { get; set; } = Color.Black;

        public void Paint(Graphics g) {
            Point tipPosition = GetConnectionPointPosition(tip, componentSize);
            Point straightPosition = GetConnectionPointPosition(straight, componentSize);
            Point branchPosition = GetConnectionPointPosition(branch, componentSize);

            if (switchPosition == straight) {
                using Pen penSwitched = new Pen(TrackColor, TrackWidth), penNotSwitch = new Pen(BranchColor, TrackWidth) {
                    DashStyle = DashStyle.Dot
                };
                g.DrawLine(penNotSwitch, tipPosition, branchPosition);
                g.DrawLine(penSwitched, tipPosition, straightPosition);
            }
            else if (switchPosition == branch) {
                using Pen penSwitched = new Pen(BranchColor, TrackWidth), penNotSwitch1 = new Pen(TrackColor, TrackWidth), penNotSwitch2 = new Pen(TrackColor, TrackWidth);
                Point middle = new Point((tipPosition.X + straightPosition.X) / 2, (tipPosition.Y + straightPosition.Y) / 2);

                penNotSwitch1.DashStyle = DashStyle.Dot;
                g.DrawLine(penNotSwitch1, tipPosition, middle);
                g.DrawLine(penNotSwitch2, middle, straightPosition);
                g.DrawLine(penSwitched, tipPosition, branchPosition);
            }
            else {
                using Pen p = new Pen(TrackColor, TrackWidth);
                g.DrawLine(p, tipPosition, branchPosition);
                g.DrawLine(p, tipPosition, straightPosition);
            }
        }
    }

    public class LayoutThreeWayTurnoutPainter : LayoutTrackPainter {
        private Size componentSize;
        private readonly LayoutComponentConnectionPoint tip;
        private readonly int switchState;
        private Color[] segmentColors = new Color[] { Color.Black, Color.Black, Color.Black };

        public LayoutThreeWayTurnoutPainter(
            Size componentSize,
            LayoutComponentConnectionPoint tip,
            int switchState) {
            this.componentSize = componentSize;
            this.tip = tip;
            this.switchState = switchState;
        }

        public Color[] SegmentColors {
            get {
                return segmentColors;
            }

            set {
                if (value.Length != 3)
                    throw new ArgumentException("SegmentColors must have 3 elements");
                segmentColors = value;
            }
        }

        public void Paint(Graphics g) {
            Point[] segmentPositions = new Point[4];

            segmentPositions[0] = GetConnectionPointPosition(tip, componentSize);
            segmentPositions[1] = GetConnectionPointPosition(LayoutTrackComponent.OppositeConnectPoint(tip), componentSize);

            switch (tip) {
                case LayoutComponentConnectionPoint.T:
                    segmentPositions[2] = GetConnectionPointPosition(LayoutComponentConnectionPoint.L, componentSize);
                    segmentPositions[3] = GetConnectionPointPosition(LayoutComponentConnectionPoint.R, componentSize);
                    break;

                case LayoutComponentConnectionPoint.B:
                    segmentPositions[2] = GetConnectionPointPosition(LayoutComponentConnectionPoint.R, componentSize);
                    segmentPositions[3] = GetConnectionPointPosition(LayoutComponentConnectionPoint.L, componentSize);
                    break;

                case LayoutComponentConnectionPoint.R:
                    segmentPositions[2] = GetConnectionPointPosition(LayoutComponentConnectionPoint.T, componentSize);
                    segmentPositions[3] = GetConnectionPointPosition(LayoutComponentConnectionPoint.B, componentSize);
                    break;

                case LayoutComponentConnectionPoint.L:
                    segmentPositions[2] = GetConnectionPointPosition(LayoutComponentConnectionPoint.B, componentSize);
                    segmentPositions[3] = GetConnectionPointPosition(LayoutComponentConnectionPoint.T, componentSize);
                    break;
            }

            for (int segment = 0; segment < 3; segment++) {
                // Special case for straight line (segment==0) if not switched, dotted line is only to the mid point
                if (segment == 0 && (switchState != 0 && switchState >= 0)) {
                    Point middle = new Point((segmentPositions[0].X + segmentPositions[1].X) / 2, (segmentPositions[0].Y + segmentPositions[1].Y) / 2);

                    using Pen penNotSwitch1 = new Pen(segmentColors[0], TrackWidth) {
                        DashStyle = DashStyle.Dot
                    }, penNotSwitch2 = new Pen(segmentColors[0], TrackWidth);
                    g.DrawLine(penNotSwitch1, segmentPositions[0], middle);
                    g.DrawLine(penNotSwitch2, middle, segmentPositions[1]);
                }
                else {
                    using Pen p = new Pen(segmentColors[segment], TrackWidth);
                    if (switchState >= 0 && switchState != segment)
                        p.DashStyle = DashStyle.Dot;

                    g.DrawLine(p, segmentPositions[0], segmentPositions[1 + segment]);
                }
            }
        }
    }

    public class LayoutDoubleSlipPainter : LayoutTrackPainter {
        private Size componentSize;
        private readonly int diagonalIndex;
        private readonly int switchState;

        public LayoutDoubleSlipPainter(Size componentSize, int diagonalIndex, int switchState) {
            this.componentSize = componentSize;
            this.diagonalIndex = diagonalIndex;
            this.switchState = switchState;
        }

        public Color HorizontalTrackColor { get; set; } = Color.Black;

        public Color VerticalTrackColor { get; set; } = Color.Black;

        public Color LeftBranchColor { get; set; } = Color.Black;

        public Color RightBranchColor { get; set; } = Color.Black;

        public void Paint(Graphics g) {
            using (Pen penVerticalTrack = new Pen(VerticalTrackColor, TrackWidth)) {
                Point p1 = GetConnectionPointPosition(LayoutComponentConnectionPoint.T, componentSize);
                Point p2 = GetConnectionPointPosition(LayoutComponentConnectionPoint.B, componentSize);

                if (switchState == 1)
                    penVerticalTrack.DashStyle = DashStyle.Dot;
                g.DrawLine(penVerticalTrack, p1, p2);
            }

            using (Pen penHorizontalTrack = new Pen(HorizontalTrackColor, TrackWidth)) {
                Point p1 = GetConnectionPointPosition(LayoutComponentConnectionPoint.L, componentSize);
                Point p2 = GetConnectionPointPosition(LayoutComponentConnectionPoint.R, componentSize);

                if (switchState == 1)
                    penHorizontalTrack.DashStyle = DashStyle.Dot;
                g.DrawLine(penHorizontalTrack, p1, p2);
            }

            using (Pen penLeftBranch = new Pen(LeftBranchColor, TrackWidth)) {
                if (switchState == 0)
                    penLeftBranch.DashStyle = DashStyle.Dot;

                if (diagonalIndex == 0)
                    g.DrawArc(penLeftBranch, new Rectangle(new Point(-componentSize.Width / 2, -componentSize.Height / 2), componentSize), 90, -90);
                else
                    g.DrawArc(penLeftBranch, new Rectangle(new Point(-componentSize.Width / 2, componentSize.Height / 2), componentSize), -90, 90);
            }

            using Pen penRightBranch = new Pen(RightBranchColor, TrackWidth);
            if (switchState == 0)
                penRightBranch.DashStyle = DashStyle.Dot;

            if (diagonalIndex == 0)
                g.DrawArc(penRightBranch, new Rectangle(new Point(componentSize.Width / 2, componentSize.Height / 2), componentSize), 180, 90);
            else
                g.DrawArc(penRightBranch, new Rectangle(new Point(componentSize.Width / 2, -componentSize.Height / 2), componentSize), 180, -90);
        }
    }

    /// <summary>
    /// Paint a track contact. The track contact is painted as an outlined filled circle on the middle of the track
    /// </summary>
    public class LayoutTriggerableBlockEdgePainter : LayoutTrackOverlayPainter, IDisposable {
        public enum ComponentType {
            TrackContact,
            ProximitySensor,
            ActiveProximitySensor,
        };

        private readonly ComponentType componentType;

        public LayoutTriggerableBlockEdgePainter(ComponentType componentType,
            Size componentSize, IList<LayoutComponentConnectionPoint> cp) : base(componentSize, cp) {
            this.componentType = componentType;
            Fill = Brushes.Orange;
            Outline = Pens.Black;
            ContactSize = new Size(9, 9);
        }

        public Brush Fill { set; get; }

        public Pen Outline { set; get; }

        public Size ContactSize { set; get; }

        public virtual void Paint(Graphics g) {
            Point centerPoint = CenterPoint;
            Rectangle contactBbox;
            var adjustedSize = new Size((ContactSize.Width + 1) / 2 * 2, (ContactSize.Height + 1) / 2 * 2); // Make sure width/height are even so it will look nice and centered

            contactBbox = new Rectangle(new Point(centerPoint.X - (adjustedSize.Width / 2), centerPoint.Y - (adjustedSize.Height / 2)), adjustedSize);
            g.FillEllipse(Fill, contactBbox);
            g.DrawEllipse(Outline, contactBbox);

            if (componentType == ComponentType.ProximitySensor || componentType == ComponentType.ActiveProximitySensor) {
                int w = (componentType == ComponentType.ActiveProximitySensor && Orientation != TrackOrientation.Vertical) ? adjustedSize.Width : ((adjustedSize.Width * 6 / 4) + 1) / 2 * 2;
                int h = (componentType == ComponentType.ActiveProximitySensor && Orientation == TrackOrientation.Vertical) ? adjustedSize.Height : ((adjustedSize.Height * 6 / 4) + 1) / 2 * 2;
                var proximitySize = new Size(w, h);
                var proximityBox = new Rectangle(new Point(centerPoint.X - (proximitySize.Width / 2), centerPoint.Y - (proximitySize.Height / 2)), proximitySize);

                g.DrawEllipse(Outline, proximityBox);
            }
        }

        #region IDisposable Members

        public void Dispose() {
        }

        #endregion
    }

    public class LayoutBlockEdgePainter : LayoutTriggerableBlockEdgePainter {
        private const int crossingLineExtra = 4;

        public LayoutBlockEdgePainter(Size componentSize,
            IList<LayoutComponentConnectionPoint> cp) : base(componentType: ComponentType.TrackContact, componentSize: componentSize, cp: cp) {
        }

        public override void Paint(Graphics g) {
            base.Paint(g);

            Point centerPoint = CenterPoint;

            if (LayoutTrackComponent.IsHorizontal(ConnectionPoints))
                g.DrawLine(Pens.Black, centerPoint.X, centerPoint.Y - ((ContactSize.Height / 2) + crossingLineExtra), centerPoint.X, centerPoint.Y + ((ContactSize.Height / 2) + crossingLineExtra));
            else
                g.DrawLine(Pens.Black, centerPoint.X - ((ContactSize.Width / 2) + crossingLineExtra), centerPoint.Y, centerPoint.X + ((ContactSize.Width / 2) + crossingLineExtra), centerPoint.Y);
        }
    }

    public class LayoutTrackLinkPainter : LayoutTrackOverlayPainter, IDisposable {
        public LayoutTrackLinkPainter(Size componentSize,
            IList<LayoutComponentConnectionPoint> cp) : base(componentSize, cp) {
        }

        public Brush Fill { get; set; } = Brushes.Black;

        public Pen OutlinePen { get; set; } = Pens.Black;

        public void Paint(Graphics g) {
            int u = ComponentSize.Width / 6;
            Point[] points = new Point[] {
                                             new Point(u, u),
                                             new Point(2*u, 0),
                                             new Point(u, -u),
                                             new Point(-u, -u),
                                             new Point(-2*u, 0),
                                             new Point(-u, u)
                                         };

            GraphicsState gs = g.Save();

            CenterOrigin(g);
            g.FillPolygon(Fill, points);
            g.DrawPolygon(OutlinePen, points);
            g.Restore(gs);
        }

        #region IDisposable Members

        public void Dispose() {
        }

        #endregion
    }

    public class LayoutPowerConnectorPainter : LayoutTrackOverlayPainter, IDisposable {
        private readonly Pen linePen = new Pen(Color.Black, 2.0F);
        private readonly Pen circlePen = new Pen(Color.Black, 1.0F);

        public LayoutPowerConnectorPainter(Size componentSize,
            IList<LayoutComponentConnectionPoint> cp) : base(componentSize, cp) {
        }

        public Brush CircleFill { get; set; } = new SolidBrush(Color.Black);

        public void Paint(Graphics g) {
            GraphicsState gs = g.Save();
            float circleSize = ComponentSize.Height / 6.0F;
            float lineLength = ComponentSize.Height / 3.0F;

            CenterOrigin(g);
            g.DrawLine(linePen, 0, 0, 0, lineLength);

            float x = -circleSize / 2.0F;
            float y = lineLength - (circleSize / 2.0F);

            g.FillEllipse(CircleFill, x, y, circleSize, circleSize);
            g.DrawEllipse(circlePen, x, y, circleSize, circleSize);

            g.Restore(gs);
        }

        public void Dispose() {
            linePen.Dispose();
            circlePen.Dispose();
            CircleFill.Dispose();
        }
    }

    public class LayoutTrackIsolationPainter : LayoutTrackOverlayPainter {
        public LayoutTrackIsolationPainter(Size componentSize,
            IList<LayoutComponentConnectionPoint> cp) : base(componentSize, cp) {
        }

        public void Paint(Graphics g) {
            GraphicsState gs = g.Save();
            float u = ComponentSize.Height / 16.0F;

            using Pen trackPen = new Pen(Color.Black, u);
            using Brush backgroundBrush = new SolidBrush(Color.White);
            CenterOrigin(g);
            g.FillRectangle(backgroundBrush, -u, -u, 2 * u, 2 * u);
            g.DrawLine(trackPen, -u, -2 * u, -u, 2 * u);
            g.DrawLine(trackPen, u, -2 * u, u, 2 * u);
            g.Restore(gs);
        }
    }

    public class LayoutTrackReverseLoopModulePainter : LayoutTrackOverlayPainter {
        public LayoutTrackReverseLoopModulePainter(Size componentSize,
            IList<LayoutComponentConnectionPoint> cp)
            : base(componentSize, cp) {
        }

        private void DrawArrow(Graphics g, Pen p, PointF tail, PointF head, float headSize) {
            g.DrawLine(p, tail, head);

            if (tail.X < head.X)
                headSize = -headSize;

            g.DrawLine(p, head, new PointF(head.X + headSize, head.Y - headSize));
            g.DrawLine(p, head, new PointF(head.X + headSize, head.Y + headSize));
        }

        public void Paint(Graphics g) {
            GraphicsState gs = g.Save();
            float u = ComponentSize.Height / 16.0F;

            using Pen trackPen = new Pen(Color.Black, u);
            using Brush backgroundBrush = new SolidBrush(Color.White);
            CenterOrigin(g);
            g.FillRectangle(backgroundBrush, -u, -u, 2 * u, 2 * u);
            g.DrawLine(trackPen, -u, -2 * u, -u, 2 * u);
            g.DrawLine(trackPen, u, -2 * u, u, 2 * u);

            DrawArrow(g, trackPen, new PointF(-3 * u, 5 * u), new PointF(3 * u, 5 * u), 2 * u);
            DrawArrow(g, trackPen, new PointF(3 * u, -5 * u), new PointF(-3 * u, -5 * u), 2 * u);

            g.Restore(gs);
        }
    }

    /// <summary>
    /// Paint a Block information
    /// </summary>
    public class LayoutBlockInfoPainter : LayoutTrackOverlayPainter, IDisposable {
        public LayoutBlockInfoPainter(Size componentSize,
            IList<LayoutComponentConnectionPoint> cp) : base(componentSize, cp) {
        }

        public Brush Fill { set; get; } = Brushes.LightSkyBlue;

        public Pen Outline { set; get; } = Pens.Black;

        public Size InfoBoxSize { set; get; } = new Size(6, 6);

        public bool OccupancyDetectionBlock { get; set; }

        public void Paint(Graphics g) {
            Rectangle info;
            Point centerPoint = CenterPoint;

            info = new Rectangle(
                new Point(centerPoint.X - (InfoBoxSize.Width / 2), centerPoint.Y - (InfoBoxSize.Height / 2)), InfoBoxSize);

            g.FillRectangle(Fill, info);
            g.DrawRectangle(Outline, info);

            if (OccupancyDetectionBlock) {
                if (LayoutTrackComponent.IsHorizontal(ConnectionPoints))
                    g.DrawLine(Pens.Black, centerPoint.X, centerPoint.Y - ((InfoBoxSize.Height / 2) + 3), centerPoint.X, centerPoint.Y + ((InfoBoxSize.Height / 2) + 3));
                else
                    g.DrawLine(Pens.Black, centerPoint.X - ((InfoBoxSize.Width / 2) + 3), centerPoint.Y, centerPoint.X + ((InfoBoxSize.Width / 2) + 3), centerPoint.X);
            }
        }

        public void Dispose() {
        }
    }

    public class LayoutTrackAnnotationPainter : LayoutTrackOverlayPainter {
        public LayoutTrackAnnotationPainter(Size componentSize, IList<LayoutComponentConnectionPoint> cps) : base(componentSize, cps) {
        }

        public int Offset { get; set; } = 6;

        protected Point GetEdgePoint(LayoutComponentConnectionPoint cp, bool rightSide) {
            Point pt = LayoutComponentPainter.GetConnectionPointPosition(cp, ComponentSize);

            if (LayoutTrackComponent.IsVertical(cp)) {
                return rightSide ? new Point(pt.X + Offset, pt.Y) : new Point(pt.X - Offset, pt.Y);
            }
            else {
                return rightSide ? new Point(pt.X, pt.Y + Offset) : new Point(pt.X, pt.Y - Offset);
            }
        }

        protected Point[] GetEdgePoints() {
            Point[] pts = new Point[4];

            if (!LayoutStraightTrackComponent.IsDiagonal(ConnectionPoints)) {
                pts[0] = GetEdgePoint(ConnectionPoints[0], true);
                pts[1] = GetEdgePoint(ConnectionPoints[1], true);
                pts[2] = GetEdgePoint(ConnectionPoints[0], false);
                pts[3] = GetEdgePoint(ConnectionPoints[1], false);
            }
            else {  // Diagonal
                LayoutComponentConnectionPoint cpH, cpV;

                cpH = LayoutStraightTrackComponent.IsHorizontal(ConnectionPoints[0]) ? ConnectionPoints[0] : ConnectionPoints[1];
                cpV = LayoutStraightTrackComponent.IsVertical(ConnectionPoints[0]) ? ConnectionPoints[0] : ConnectionPoints[1];

                Point ptH = GetConnectionPointPosition(cpH, ComponentSize), ptV = GetConnectionPointPosition(cpV, ComponentSize);

                if (ptH.X < ptV.X ^ ptH.Y > ptV.Y) {
                    pts[0] = GetEdgePoint(ConnectionPoints[0], false);
                    pts[1] = GetEdgePoint(ConnectionPoints[1], true);
                    pts[2] = GetEdgePoint(ConnectionPoints[0], true);
                    pts[3] = GetEdgePoint(ConnectionPoints[1], false);
                }
                else {
                    pts[0] = GetEdgePoint(ConnectionPoints[0], true);
                    pts[1] = GetEdgePoint(ConnectionPoints[1], true);
                    pts[2] = GetEdgePoint(ConnectionPoints[0], false);
                    pts[3] = GetEdgePoint(ConnectionPoints[1], false);
                }
            }

            if (pts[1].Y != pts[2].Y) {
                Point t = pts[2];

                pts[2] = pts[3];
                pts[3] = t;
            }

            return pts;
        }
    }

    public class LayoutBridgePainter : LayoutTrackAnnotationPainter {
        public LayoutBridgePainter(Size componentSize, IList<LayoutComponentConnectionPoint> cps) : base(componentSize, cps) {
        }

        public void Paint(Graphics g) {
            Point[] pts = GetEdgePoints();

            g.DrawLine(Pens.Black, pts[0], pts[1]);
            g.DrawLine(Pens.Black, pts[2], pts[3]);

            // Draw columns
            Point[] columns = new Point[2];

            columns[0] = Midpoint(pts[0], pts[1]);
            columns[1] = Midpoint(pts[2], pts[3]);

            g.FillRectangles(Brushes.Black, new Rectangle[] {
                                                                new Rectangle(new Point(columns[0].X-2, columns[0].Y-2), new Size(4, 4)),
                                                                new Rectangle(new Point(columns[1].X-2, columns[1].Y-2), new Size(4, 4))
                                                            });
        }
    }

    public class LayoutTunnelPainter : LayoutTrackAnnotationPainter {
        private readonly Color tunnelColor;

        public LayoutTunnelPainter(Size componentSize, IList<LayoutComponentConnectionPoint> cps) : base(componentSize, cps) {
            tunnelColor = Color.FromArgb(192, Color.Gray);
        }

        public void Paint(Graphics g) {
            Point[] pts = GetEdgePoints();

            // TODO: Support tunnel portals...

            using Brush br = new SolidBrush(tunnelColor);
            g.FillPolygon(br, pts);
        }
    }

    public class BallonPainter : IDisposable {
        private int hotspotOrigin = 5;

        #region Properties

        public RectangleF Bounds { get; set; } = new Rectangle(new Point(0, 0), new Size(180, 100));

        public PointF Hotspot { get; set; } = new Point(20, 150);

        public int HotspotOrigin {
            get {
                return hotspotOrigin;
            }

            set {
                if (value < 0 || value > 7)
                    throw new ArgumentException("Invalid value for HotSpotOrigin (must be between 0 and 7)");
                hotspotOrigin = value;
            }
        }

        public SizeF CornerSize { get; set; } = new SizeF(10, 10);

        public SizeF ArrowSize { get; set; } = new SizeF(8, 6);

        public Pen Outline { get; set; } = new Pen(Brushes.Black, 2);

        public Brush Fill { get; set; } = Brushes.Yellow;

        #endregion

        #region LastPoint class 

        private class LastPoint {
            private readonly GraphicsPath p;
            private PointF cp;
            private int lastPointCount;

            public LastPoint(GraphicsPath p, PointF cp) {
                this.p = p;
                this.cp = cp;
            }

            public PointF Value {
                get {
                    if (p.PointCount != lastPointCount) {
                        cp = p.GetLastPoint();
                        lastPointCount = p.PointCount;
                    }

                    return cp;
                }
            }

            public float X => Value.X;

            public float Y => Value.Y;

            public static implicit operator PointF(LastPoint lp) => lp.Value;
        }

        #endregion

        public GraphicsPath BallonGraphicPath {
            get {
                GraphicsPath p = new GraphicsPath();
                LastPoint lp = new LastPoint(p, new PointF(Bounds.Left + CornerSize.Width, Bounds.Top));
                PointF cornerOrigin = new PointF(Bounds.Right - CornerSize.Width, lp.Y);

                if (hotspotOrigin == 0) {
                    PointF np = new PointF(lp.X + ArrowSize.Width, lp.Y);

                    p.AddLine(lp, Hotspot);
                    p.AddLine(lp, np);
                }
                else if (hotspotOrigin == 1) {
                    p.AddLine(lp, new PointF(Bounds.Right - CornerSize.Width - ArrowSize.Width, lp.Y));
                    p.AddLine(lp, Hotspot);
                }

                p.AddLine(lp, cornerOrigin);
                p.AddArc(new RectangleF(cornerOrigin, CornerSize), 270, 90);

                cornerOrigin = new PointF(Bounds.Right, Bounds.Bottom - CornerSize.Height);

                if (hotspotOrigin == 2) {
                    PointF np = new PointF(Bounds.Right, Bounds.Top + CornerSize.Height + ArrowSize.Height);

                    p.AddLine(lp, Hotspot);
                    p.AddLine(lp, np);
                }
                else if (hotspotOrigin == 3) {
                    p.AddLine(lp, new PointF(Bounds.Right, Bounds.Bottom - CornerSize.Height - ArrowSize.Height));
                    p.AddLine(lp, Hotspot);
                }

                p.AddLine(lp, cornerOrigin);
                p.AddArc(new RectangleF(new PointF(cornerOrigin.X - CornerSize.Width, cornerOrigin.Y), CornerSize), 0, 90);

                cornerOrigin = new PointF(Bounds.Left + CornerSize.Width, Bounds.Bottom);

                if (hotspotOrigin == 4) {
                    PointF np = new PointF(Bounds.Right - CornerSize.Width - ArrowSize.Width, Bounds.Bottom);

                    p.AddLine(lp, Hotspot);
                    p.AddLine(lp, np);
                }
                else if (hotspotOrigin == 5) {
                    p.AddLine(lp, new PointF(Bounds.Left + CornerSize.Width + ArrowSize.Width, Bounds.Bottom));
                    p.AddLine(lp, Hotspot);
                }

                p.AddLine(lp, cornerOrigin);
                p.AddArc(new RectangleF(new PointF(Bounds.Left, cornerOrigin.Y - CornerSize.Height), CornerSize), 90, 90);

                cornerOrigin = new PointF(Bounds.Left, Bounds.Top + CornerSize.Height);

                if (hotspotOrigin == 6) {
                    PointF np = new PointF(Bounds.Left, Bounds.Bottom - CornerSize.Height - ArrowSize.Height);

                    p.AddLine(lp, Hotspot);
                    p.AddLine(lp, np);
                }
                else if (hotspotOrigin == 7) {
                    p.AddLine(lp, new PointF(Bounds.Left, Bounds.Top + CornerSize.Height + ArrowSize.Height));
                    p.AddLine(lp, Hotspot);
                }

                p.AddLine(lp, cornerOrigin);
                p.AddArc(new RectangleF(new PointF(Bounds.Left, Bounds.Top), CornerSize), 180, 90);

                p.CloseFigure();

                return p;
            }
        }

        public void Paint(Graphics g) {
            using GraphicsPath p = BallonGraphicPath;
            g.FillPath(Fill, p);
            g.DrawPath(Outline, p);
        }

        #region IDisposable Members

        public void Dispose() {
            if (Outline != null)
                Outline.Dispose();
        }

        #endregion
    }

    public class LayoutGatePainter : IDisposable {
        private Size componentSize;
        private readonly bool isVertical;
        private readonly bool openUpOrLeft;

        public LayoutGatePainter(Size componentSize, bool isVertical, bool openUpOrLeft, int openingClearance) {
            this.componentSize = componentSize;
            this.isVertical = isVertical;
            this.openUpOrLeft = openUpOrLeft;
            this.OpeningClearance = openingClearance;
        }

        public Color GateColor { get; set; } = Color.DarkGoldenrod;

        public Color ColumnsColor { get; set; } = Color.DarkGray;

        public int OpeningClearance { get; set; }

        public void Paint(Graphics g) {
            GraphicsState gs = g.Save();

            g.TranslateTransform(componentSize.Width / 2.0f, componentSize.Height / 2.0f);

            if (isVertical)
                g.RotateTransform(90);

            if (openUpOrLeft)
                g.RotateTransform(180);

            float gateSize = (componentSize.Height / 2.0f) - 2;

            float gateEdgeX = (float)Math.Sin(2 * Math.PI / 400 * OpeningClearance) * gateSize;
            float gateEdgeY = gateSize - ((float)Math.Cos(2 * Math.PI / 400 * OpeningClearance) * gateSize);

            using (Pen gatePen = new Pen(GateColor, 2.0f)) {
                g.DrawLine(gatePen, 0, gateSize, gateEdgeX, gateEdgeY);
                g.DrawLine(gatePen, 0, -gateSize + 1, gateEdgeX, -gateEdgeY + 1);

                using Brush columnBrush = new SolidBrush(ColumnsColor);
                g.FillEllipse(columnBrush, -3, (componentSize.Height / 2) - 6, 6, 6);
                g.FillEllipse(columnBrush, -3, -(componentSize.Height / 2), 6, 6);
            }

            g.Restore(gs);
        }

        #region IDisposable Members

        public void Dispose() {
        }

        #endregion
    }

    public class ControlModuleLocationPainter {
        public ControlModuleLocationPainter() {
        }

        public void Paint(Graphics g) {
            System.Drawing.Imaging.ImageAttributes attributes = new System.Drawing.Imaging.ImageAttributes();
            Image image = (Image)EventManager.Event(new LayoutEvent("get-image", this));

            attributes.SetColorKey(Color.White, Color.White);

            g.DrawImage(image, new Rectangle(new Point(1, 1), image.Size), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
        }
    }

    public class PowerSelectorPainter {
        public bool IsSwitch { get; set; }
        public Size ComponentSize { get; set; }
        public int SwitchState { get; set; }
        public int SwitchGap { get; set; }
        public SizeF ContactSize { get; set; }
        public int ContactOffset { get; set; }
        public int SelectorTipLength { get; set; }
        public int SelectorTipOffset { get; set; }
        public Pen SwitchPen { get; set; }

        public PowerSelectorPainter() {
            ComponentSize = new Size(32, 32);
            IsSwitch = false;
            SwitchState = 0;
            SwitchGap = 10;
            ContactSize = new SizeF(5, 5);
            ContactOffset = 2;
            SelectorTipLength = 6;
            SelectorTipOffset = 3;
            SwitchPen = new Pen(Brushes.Blue, 2.0f);
        }

        public void Paint(Graphics g) {
            PointF side0 = new PointF((ComponentSize.Width - SwitchGap) / 2.0f, ComponentSize.Height / 2.0f);
            PointF side1 = new PointF((ComponentSize.Width + SwitchGap) / 2.0f, ComponentSize.Height / 2.0f);
            PointF middle = new PointF(ComponentSize.Width / 2.0f, ComponentSize.Height / 2.0f);
            var stateToPoint = new PointF[] { middle, side0, side1 };

            g.DrawLine(Pens.Blue, new PointF(ContactOffset, ComponentSize.Height / 2.0f), side0);
            g.DrawLine(Pens.Blue, side1, new PointF(ComponentSize.Width - ContactOffset, ComponentSize.Height / 2.0f));

            g.FillEllipse(Brushes.Black, new RectangleF(new PointF(ContactOffset, (ComponentSize.Height / 2.0f) - (ContactSize.Height / 2.0f)), ContactSize));
            g.FillEllipse(Brushes.Black, new RectangleF(new PointF(ComponentSize.Width - ContactOffset - ContactSize.Width, (ComponentSize.Height / 2.0f) - (ContactSize.Height / 2.0f)), ContactSize));

            if (IsSwitch)
                g.DrawLine(SwitchPen, side0, SwitchState != 0 ? side1 : new PointF(side1.X, side1.Y - (side1.X - side0.X)));
            else {
                PointF switchTipBottom = new PointF(ComponentSize.Width / 2.0f, ComponentSize.Height - SelectorTipOffset);
                PointF switchTip = new PointF(ComponentSize.Width / 2.0f, ComponentSize.Height - SelectorTipOffset - SelectorTipLength);

                g.DrawLine(SwitchPen, switchTipBottom, switchTip);
                g.DrawLine(SwitchPen, switchTip, stateToPoint[SwitchState + 1]);
                g.FillEllipse(Brushes.Black, new RectangleF(new PointF(switchTipBottom.X - (ContactSize.Width / 2.0f), switchTipBottom.Y - (ContactSize.Height / 2.0f)), ContactSize));
            }
        }
    }
}
