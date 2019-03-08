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

    /// <summary>
    /// 
    /// </summary>
    public class LayoutTrackPainter : LayoutComponentPainter {
        Color trackColor = Color.Black;
        int trackWidth = 6;

        public Color TrackColor {
            get {
                return trackColor;
            }

            set {
                trackColor = value;
            }
        }

        public int TrackWidth {
            get {
                return trackWidth;
            }

            set {
                trackWidth = value;
            }
        }

    }

    public class LayoutTrackOverlayPainter : LayoutComponentPainter {
        readonly IList<LayoutComponentConnectionPoint> cp;
        Size componentSize;

        public LayoutTrackOverlayPainter(Size componentSize, IList<LayoutComponentConnectionPoint> cp) {
            Debug.Assert(cp.Count >= 2);

            this.cp = cp;
            this.componentSize = componentSize;
        }

        protected Size ComponentSize => componentSize;

        protected IList<LayoutComponentConnectionPoint> ConnectionPoints => cp;

        /// <summary>
        /// Center the origin to the middle of the component grid. If the component is vertical, a rotate
        /// transform is applied
        /// </summary>
        /// <param name="g"></param>
        protected void CenterOrigin(Graphics g) {
            g.TranslateTransform(componentSize.Width / 2.0F, componentSize.Height / 2.0F);

            if (LayoutTrackComponent.IsVertical(cp[0]))
                g.RotateTransform(90.0F);
        }

        /// <summary>
        /// Return the center point between the two connection points
        /// </summary>
        protected Point CenterPoint {
            get {
                LayoutComponentConnectionPoint oppositeCp = LayoutTrackComponent.OppositeConnectPoint(cp[0]);
                LayoutComponentConnectionPoint otherCp = cp[1];

                if (cp.Count > 2) {
                    foreach (LayoutComponentConnectionPoint cpLookup in cp) {
                        if (cpLookup == oppositeCp) {
                            otherCp = oppositeCp;
                            break;
                        }
                    }
                }

                return Midpoint(LayoutComponentPainter.GetConnectionPointPosition(cp[0], componentSize),
                    LayoutComponentPainter.GetConnectionPointPosition(otherCp, componentSize));
            }
        }
    }

    /// <summary>
    /// Handle drawing of straight tracks
    /// </summary>
    public class LayoutStraightTrackPainter : LayoutTrackPainter {
        readonly LayoutComponentConnectionPoint cp1, cp2;
        Size componentSize;
        Color trackColor2 = Color.Black;

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

        public Color TrackColor2 {
            set {
                trackColor2 = value;
            }

            get {
                return trackColor2;
            }
        }

        public void Paint(Graphics g) {
            Point cp1Position = GetConnectionPointPosition(cp1, componentSize);
            Point cp2Position = GetConnectionPointPosition(cp2, componentSize);

            using (Pen p = new Pen(TrackColor, TrackWidth)) {
                if (TrackColor == TrackColor2)
                    g.DrawLine(p, cp1Position, cp2Position);
                else {
                    Point cpMiddlePosition = Midpoint(cp1Position, cp2Position);

                    using (Pen p2 = new Pen(TrackColor2, TrackWidth)) {
                        g.DrawLine(p, cp1Position, cpMiddlePosition);
                        g.DrawLine(p2, cpMiddlePosition, cp2Position);
                    }
                }
            }
        }
    }

    public class LayoutTurnoutTrackPainter : LayoutTrackPainter {
        Size componentSize;
        readonly LayoutComponentConnectionPoint tip;
        readonly LayoutComponentConnectionPoint straight;
        readonly LayoutComponentConnectionPoint branch;
        readonly LayoutComponentConnectionPoint switchPosition;
        Color branchColor = Color.Black;

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

        public Color BranchColor {
            get {
                return branchColor;
            }

            set {
                branchColor = value;
            }
        }

        public void Paint(Graphics g) {
            Point tipPosition = GetConnectionPointPosition(tip, componentSize);
            Point straightPosition = GetConnectionPointPosition(straight, componentSize);
            Point branchPosition = GetConnectionPointPosition(branch, componentSize);


            if (switchPosition == straight) {
                using (Pen penSwitched = new Pen(TrackColor, TrackWidth), penNotSwitch = new Pen(BranchColor, TrackWidth)) {
                    penNotSwitch.DashStyle = DashStyle.Dot;
                    g.DrawLine(penNotSwitch, tipPosition, branchPosition);
                    g.DrawLine(penSwitched, tipPosition, straightPosition);
                }
            }
            else if (switchPosition == branch) {
                using (Pen penSwitched = new Pen(BranchColor, TrackWidth), penNotSwitch1 = new Pen(TrackColor, TrackWidth), penNotSwitch2 = new Pen(TrackColor, TrackWidth)) {
                    Point middle = new Point((tipPosition.X + straightPosition.X) / 2, (tipPosition.Y + straightPosition.Y) / 2);

                    penNotSwitch1.DashStyle = DashStyle.Dot;
                    g.DrawLine(penNotSwitch1, tipPosition, middle);
                    g.DrawLine(penNotSwitch2, middle, straightPosition);
                    g.DrawLine(penSwitched, tipPosition, branchPosition);
                }
            }
            else {
                using (Pen p = new Pen(TrackColor, TrackWidth)) {
                    g.DrawLine(p, tipPosition, branchPosition);
                    g.DrawLine(p, tipPosition, straightPosition);
                }
            }
        }
    }

    public class LayoutThreeWayTurnoutPainter : LayoutTrackPainter {
        Size componentSize;
        readonly LayoutComponentConnectionPoint tip;
        readonly int switchState;
        Color[] segmentColors = new Color[] { Color.Black, Color.Black, Color.Black };

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
            Point tipPosition = GetConnectionPointPosition(tip, componentSize);
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

                    using (Pen penNotSwitch1 = new Pen(segmentColors[0], TrackWidth), penNotSwitch2 = new Pen(segmentColors[0], TrackWidth)) {
                        penNotSwitch1.DashStyle = DashStyle.Dot;
                        g.DrawLine(penNotSwitch1, segmentPositions[0], middle);
                        g.DrawLine(penNotSwitch2, middle, segmentPositions[1]);
                    }
                }
                else {
                    using (Pen p = new Pen(segmentColors[segment], TrackWidth)) {
                        if (switchState >= 0 && switchState != segment)
                            p.DashStyle = DashStyle.Dot;

                        g.DrawLine(p, segmentPositions[0], segmentPositions[1 + segment]);
                    }
                }
            }
        }
    }

    public class LayoutDoubleSlipPainter : LayoutTrackPainter {
        Size componentSize;
        readonly int diagonalIndex;
        readonly int switchState;
        Color horizontalTrackColor = Color.Black;
        Color verticalTrackColor = Color.Black;
        Color leftBranchColor = Color.Black;
        Color rightBranchColor = Color.Black;

        public LayoutDoubleSlipPainter(Size componentSize, int diagonalIndex, int switchState) {
            this.componentSize = componentSize;
            this.diagonalIndex = diagonalIndex;
            this.switchState = switchState;
        }

        public Color HorizontalTrackColor {
            get { return horizontalTrackColor; }
            set { horizontalTrackColor = value; }
        }

        public Color VerticalTrackColor {
            get { return verticalTrackColor; }
            set { verticalTrackColor = value; }
        }

        public Color LeftBranchColor {
            get { return leftBranchColor; }
            set { leftBranchColor = value; }
        }

        public Color RightBranchColor {
            get { return rightBranchColor; }
            set { rightBranchColor = value; }
        }

        public void Paint(Graphics g) {
            using (Pen p = new Pen(VerticalTrackColor, TrackWidth)) {
                Point p1 = GetConnectionPointPosition(LayoutComponentConnectionPoint.T, componentSize);
                Point p2 = GetConnectionPointPosition(LayoutComponentConnectionPoint.B, componentSize);

                if (switchState == 1)
                    p.DashStyle = DashStyle.Dot;
                g.DrawLine(p, p1, p2);
            }

            using (Pen p = new Pen(HorizontalTrackColor, TrackWidth)) {
                Point p1 = GetConnectionPointPosition(LayoutComponentConnectionPoint.L, componentSize);
                Point p2 = GetConnectionPointPosition(LayoutComponentConnectionPoint.R, componentSize);

                if (switchState == 1)
                    p.DashStyle = DashStyle.Dot;
                g.DrawLine(p, p1, p2);
            }

            using (Pen p = new Pen(LeftBranchColor, TrackWidth)) {
                if (switchState == 0)
                    p.DashStyle = DashStyle.Dot;

                if (diagonalIndex == 0)
                    g.DrawArc(p, new Rectangle(new Point(-componentSize.Width / 2, -componentSize.Height / 2), componentSize), 90, -90);
                else
                    g.DrawArc(p, new Rectangle(new Point(-componentSize.Width / 2, componentSize.Height / 2), componentSize), -90, 90);
            }

            using (Pen p = new Pen(RightBranchColor, TrackWidth)) {
                if (switchState == 0)
                    p.DashStyle = DashStyle.Dot;

                if (diagonalIndex == 0)
                    g.DrawArc(p, new Rectangle(new Point(componentSize.Width / 2, componentSize.Height / 2), componentSize), 180, 90);
                else
                    g.DrawArc(p, new Rectangle(new Point(componentSize.Width / 2, -componentSize.Height / 2), componentSize), 180, -90);
            }
        }
    }

    /// <summary>
    /// Paint a track contact. The track contact is painted as an outlined filled circle on the middle of the track
    /// </summary>
    public class LayoutTrackContactPainter : LayoutTrackOverlayPainter, IDisposable {
        Brush fill;
        Pen outline;
        Size contactSize = new Size(9, 9);

        public LayoutTrackContactPainter(Size componentSize,
            IList<LayoutComponentConnectionPoint> cp) : base(componentSize, cp) {
            fill = Brushes.Orange;
            outline = Pens.Black;
        }

        public Brush Fill {
            set {
                fill = value;
            }

            get {
                return fill;
            }
        }

        public Pen Outline {
            set {
                outline = value;
            }

            get {
                return outline;
            }
        }

        public Size ContactSize {
            set {
                contactSize = value;
            }

            get {
                return contactSize;
            }
        }

        public virtual void Paint(Graphics g) {
            Point centerPoint = CenterPoint;
            Rectangle contactBbox;

            contactBbox = new Rectangle(new Point(centerPoint.X - contactSize.Width / 2, centerPoint.Y - contactSize.Height / 2), contactSize);
            g.FillEllipse(fill, contactBbox);
            g.DrawEllipse(outline, contactBbox);
        }

        #region IDisposable Members

        public void Dispose() {
        }

        #endregion
    }

    public class LayoutBlockEdgePainter : LayoutTrackContactPainter {
        const int crossingLineExtra = 4;

        public LayoutBlockEdgePainter(Size componentSize,
            IList<LayoutComponentConnectionPoint> cp) : base(componentSize, cp) {
        }

        public override void Paint(Graphics g) {
            base.Paint(g);

            Point centerPoint = CenterPoint;

            if (LayoutTrackComponent.IsHorizontal(ConnectionPoints))
                g.DrawLine(Pens.Black, centerPoint.X, centerPoint.Y - (ContactSize.Height / 2 + crossingLineExtra), centerPoint.X, centerPoint.Y + (ContactSize.Height / 2 + crossingLineExtra));
            else
                g.DrawLine(Pens.Black, centerPoint.X - (ContactSize.Width / 2 + crossingLineExtra), centerPoint.Y, centerPoint.X + (ContactSize.Width / 2 + crossingLineExtra), centerPoint.Y);
        }
    }

    public class LayoutTrackLinkPainter : LayoutTrackOverlayPainter, IDisposable {
        Brush fill = Brushes.Black;
        Pen outline = Pens.Black;

        public LayoutTrackLinkPainter(Size componentSize,
            IList<LayoutComponentConnectionPoint> cp) : base(componentSize, cp) {
        }

        public Brush Fill {
            get {
                return fill;
            }

            set {
                fill = value;
            }
        }

        public Pen OutlinePen {
            get {
                return outline;
            }

            set {
                outline = value;
            }
        }

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
            g.FillPolygon(fill, points);
            g.DrawPolygon(outline, points);
            g.Restore(gs);
        }

        #region IDisposable Members

        public void Dispose() {
        }

        #endregion
    }

    public class LayoutPowerConnectorPainter : LayoutTrackOverlayPainter, IDisposable {
        readonly Pen linePen = new Pen(Color.Black, 2.0F);
        readonly Pen circlePen = new Pen(Color.Black, 1.0F);
        Brush circleFill = new SolidBrush(Color.Black);

        public LayoutPowerConnectorPainter(Size componentSize,
            IList<LayoutComponentConnectionPoint> cp) : base(componentSize, cp) {
        }

        public Brush CircleFill {
            get {
                return circleFill;
            }

            set {
                circleFill = value;
            }
        }

        public void Paint(Graphics g) {
            GraphicsState gs = g.Save();
            float circleSize = ComponentSize.Height / 6.0F;
            float lineLength = ComponentSize.Height / 3.0F;

            CenterOrigin(g);
            g.DrawLine(linePen, 0, 0, 0, lineLength);

            float x = -circleSize / 2.0F;
            float y = lineLength - circleSize / 2.0F;

            g.FillEllipse(circleFill, x, y, circleSize, circleSize);
            g.DrawEllipse(circlePen, x, y, circleSize, circleSize);

            g.Restore(gs);
        }

        public void Dispose() {
            linePen.Dispose();
            circlePen.Dispose();
            circleFill.Dispose();
        }
    }

    public class LayoutTrackIsolationPainter : LayoutTrackOverlayPainter {

        public LayoutTrackIsolationPainter(Size componentSize,
            IList<LayoutComponentConnectionPoint> cp) : base(componentSize, cp) {
        }

        public void Paint(Graphics g) {
            GraphicsState gs = g.Save();
            float u = ComponentSize.Height / 16.0F;

            using (Pen trackPen = new Pen(Color.Black, u)) {
                using (Brush backgroundBrush = new SolidBrush(Color.White)) {

                    CenterOrigin(g);
                    g.FillRectangle(backgroundBrush, -u, -u, 2 * u, 2 * u);
                    g.DrawLine(trackPen, -u, -2 * u, -u, 2 * u);
                    g.DrawLine(trackPen, u, -2 * u, u, 2 * u);
                    g.Restore(gs);
                }
            }
        }
    }

    public class LayoutTrackReverseLoopModulePainter : LayoutTrackOverlayPainter {

        public LayoutTrackReverseLoopModulePainter(Size componentSize,
            IList<LayoutComponentConnectionPoint> cp)
            : base(componentSize, cp) {
        }

        void DrawArrow(Graphics g, Pen p, PointF tail, PointF head, float headSize) {

            g.DrawLine(p, tail, head);

            if (tail.X < head.X)
                headSize = -headSize;

            g.DrawLine(p, head, new PointF(head.X + headSize, head.Y - headSize));
            g.DrawLine(p, head, new PointF(head.X + headSize, head.Y + headSize));
        }

        public void Paint(Graphics g) {
            GraphicsState gs = g.Save();
            float u = ComponentSize.Height / 16.0F;

            using (Pen trackPen = new Pen(Color.Black, u)) {
                using (Brush backgroundBrush = new SolidBrush(Color.White)) {

                    CenterOrigin(g);
                    g.FillRectangle(backgroundBrush, -u, -u, 2 * u, 2 * u);
                    g.DrawLine(trackPen, -u, -2 * u, -u, 2 * u);
                    g.DrawLine(trackPen, u, -2 * u, u, 2 * u);

                    DrawArrow(g, trackPen, new PointF(-3 * u, 5 * u), new PointF(3 * u, 5 * u), 2 * u);
                    DrawArrow(g, trackPen, new PointF(3 * u, -5 * u), new PointF(-3 * u, -5 * u), 2 * u);

                    g.Restore(gs);
                }
            }
        }
    }

    /// <summary>
    /// Paint a Block information
    /// </summary>
    public class LayoutBlockInfoPainter : LayoutTrackOverlayPainter, IDisposable {
        Brush fill = Brushes.LightSkyBlue;
        Pen outline = Pens.Black;
        Size infoBoxSize = new Size(6, 6);
        bool occupancyDetectionBlock;

        public LayoutBlockInfoPainter(Size componentSize,
            IList<LayoutComponentConnectionPoint> cp) : base(componentSize, cp) {
        }

        public Brush Fill {
            set {
                fill = value;
            }

            get {
                return fill;
            }
        }

        public Pen Outline {
            set {
                outline = value;
            }

            get {
                return outline;
            }
        }

        public Size InfoBoxSize {
            set {
                infoBoxSize = value;
            }

            get {
                return infoBoxSize;
            }
        }

        public bool OccupancyDetectionBlock {
            get {
                return occupancyDetectionBlock;
            }

            set {
                occupancyDetectionBlock = value;
            }
        }

        public void Paint(Graphics g) {
            Rectangle info;
            Point centerPoint = CenterPoint;

            info = new Rectangle(
                new Point(centerPoint.X - infoBoxSize.Width / 2, centerPoint.Y - infoBoxSize.Height / 2), infoBoxSize);

            g.FillRectangle(fill, info);
            g.DrawRectangle(outline, info);

            if (occupancyDetectionBlock) {
                if (LayoutTrackComponent.IsHorizontal(ConnectionPoints))
                    g.DrawLine(Pens.Black, centerPoint.X, centerPoint.Y - (infoBoxSize.Height / 2 + 3), centerPoint.X, centerPoint.Y + (infoBoxSize.Height / 2 + 3));
                else
                    g.DrawLine(Pens.Black, centerPoint.X - (infoBoxSize.Width / 2 + 3), centerPoint.Y, centerPoint.X + (infoBoxSize.Width / 2 + 3), centerPoint.X);
            }
        }

        public void Dispose() {
        }
    }

    public class LayoutTrackAnnotationPainter : LayoutTrackOverlayPainter {
        int offset = 6;

        public LayoutTrackAnnotationPainter(Size componentSize, IList<LayoutComponentConnectionPoint> cps) : base(componentSize, cps) {
        }

        public int Offset {
            get {
                return offset;
            }

            set {
                offset = value;
            }
        }

        protected Point GetEdgePoint(LayoutComponentConnectionPoint cp, bool rightSide) {
            Point pt = LayoutComponentPainter.GetConnectionPointPosition(cp, ComponentSize);

            if (LayoutTrackComponent.IsVertical(cp)) {
                if (rightSide)
                    return new Point(pt.X + offset, pt.Y);
                else
                    return new Point(pt.X - offset, pt.Y);
            }
            else {
                if (rightSide)
                    return new Point(pt.X, pt.Y + offset);
                else
                    return new Point(pt.X, pt.Y - offset);
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
        readonly Color tunnelColor;

        public LayoutTunnelPainter(Size componentSize, IList<LayoutComponentConnectionPoint> cps) : base(componentSize, cps) {
            tunnelColor = Color.FromArgb(192, Color.Gray);
        }

        public void Paint(Graphics g) {
            Point[] pts = GetEdgePoints();

            // TODO: Support tunnel portals...

            using (Brush br = new SolidBrush(tunnelColor)) {
                g.FillPolygon(br, pts);
            }
        }
    }

    public class BallonPainter : IDisposable {
        RectangleF bbox = new Rectangle(new Point(0, 0), new Size(180, 100));
        PointF hotspot = new Point(20, 150);
        SizeF cornerSize = new SizeF(10, 10);
        SizeF arrowSize = new SizeF(8, 6);
        Pen outline = new Pen(Brushes.Black, 2);
        Brush fill = Brushes.Yellow;
        int hotspotOrigin = 5;

        #region Properties

        public RectangleF Bounds {
            get {
                return bbox;
            }

            set {
                bbox = value;
            }
        }

        public PointF Hotspot {
            get {
                return hotspot;
            }

            set {
                hotspot = value;
            }
        }

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

        public SizeF CornerSize {
            get {
                return cornerSize;
            }

            set {
                cornerSize = value;
            }
        }

        public SizeF ArrowSize {
            get {
                return arrowSize;
            }

            set {
                arrowSize = value;
            }
        }

        public Pen Outline {
            get {
                return outline;
            }

            set {
                outline = value;
            }
        }

        public Brush Fill {
            get {
                return fill;
            }

            set {
                fill = value;
            }
        }

        #endregion

        #region LastPoint class 

        class LastPoint {
            readonly GraphicsPath p;
            PointF cp;
            int lastPointCount;

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
                LastPoint lp = new LastPoint(p, new PointF(bbox.Left + cornerSize.Width, bbox.Top));
                PointF cornerOrigin;

                cornerOrigin = new PointF(bbox.Right - cornerSize.Width, lp.Y);

                if (hotspotOrigin == 0) {
                    PointF np = new PointF(lp.X + arrowSize.Width, lp.Y);

                    p.AddLine(lp, hotspot);
                    p.AddLine(lp, np);
                }
                else if (hotspotOrigin == 1) {
                    p.AddLine(lp, new PointF(bbox.Right - cornerSize.Width - arrowSize.Width, lp.Y));
                    p.AddLine(lp, hotspot);
                }

                p.AddLine(lp, cornerOrigin);
                p.AddArc(new RectangleF(cornerOrigin, cornerSize), 270, 90);

                cornerOrigin = new PointF(bbox.Right, bbox.Bottom - cornerSize.Height);

                if (hotspotOrigin == 2) {
                    PointF np = new PointF(bbox.Right, bbox.Top + cornerSize.Height + arrowSize.Height);

                    p.AddLine(lp, hotspot);
                    p.AddLine(lp, np);
                }
                else if (hotspotOrigin == 3) {
                    p.AddLine(lp, new PointF(bbox.Right, bbox.Bottom - cornerSize.Height - arrowSize.Height));
                    p.AddLine(lp, hotspot);
                }

                p.AddLine(lp, cornerOrigin);
                p.AddArc(new RectangleF(new PointF(cornerOrigin.X - cornerSize.Width, cornerOrigin.Y), cornerSize), 0, 90);

                cornerOrigin = new PointF(bbox.Left + cornerSize.Width, bbox.Bottom);

                if (hotspotOrigin == 4) {
                    PointF np = new PointF(bbox.Right - cornerSize.Width - arrowSize.Width, bbox.Bottom);

                    p.AddLine(lp, hotspot);
                    p.AddLine(lp, np);
                }
                else if (hotspotOrigin == 5) {
                    p.AddLine(lp, new PointF(bbox.Left + cornerSize.Width + arrowSize.Width, bbox.Bottom));
                    p.AddLine(lp, hotspot);
                }

                p.AddLine(lp, cornerOrigin);
                p.AddArc(new RectangleF(new PointF(bbox.Left, cornerOrigin.Y - cornerSize.Height), cornerSize), 90, 90);

                cornerOrigin = new PointF(bbox.Left, bbox.Top + cornerSize.Height);

                if (hotspotOrigin == 6) {
                    PointF np = new PointF(bbox.Left, bbox.Bottom - cornerSize.Height - arrowSize.Height);

                    p.AddLine(lp, hotspot);
                    p.AddLine(lp, np);
                }
                else if (hotspotOrigin == 7) {
                    p.AddLine(lp, new PointF(bbox.Left, bbox.Top + cornerSize.Height + arrowSize.Height));
                    p.AddLine(lp, hotspot);
                }

                p.AddLine(lp, cornerOrigin);
                p.AddArc(new RectangleF(new PointF(bbox.Left, bbox.Top), cornerSize), 180, 90);

                p.CloseFigure();

                return p;
            }
        }

        public void Paint(Graphics g) {
            using (GraphicsPath p = BallonGraphicPath) {
                g.FillPath(fill, p);
                g.DrawPath(outline, p);
            }
        }

        #region IDisposable Members

        public void Dispose() {
            if (outline != null)
                outline.Dispose();
        }

        #endregion
    }

    public class LayoutGatePainter : IDisposable {
        Size componentSize;
        readonly bool isVertical;
        readonly bool openUpOrLeft;
        Color gateColor = Color.DarkGoldenrod;
        Color columnsColor = Color.DarkGray;
        int openingClearance;

        public LayoutGatePainter(Size componentSize, bool isVertical, bool openUpOrLeft, int openingClearance) {
            this.componentSize = componentSize;
            this.isVertical = isVertical;
            this.openUpOrLeft = openUpOrLeft;
            this.openingClearance = openingClearance;
        }

        public Color GateColor {
            get { return gateColor; }
            set { gateColor = value; }
        }

        public Color ColumnsColor {
            get { return columnsColor; }
            set { columnsColor = value; }
        }

        public int OpeningClearance {
            get { return openingClearance; }
            set { openingClearance = value; }
        }

        public void Paint(Graphics g) {
            GraphicsState gs = g.Save();

            g.TranslateTransform(componentSize.Width / 2.0f, componentSize.Height / 2.0f);

            if (isVertical)
                g.RotateTransform(90);

            if (openUpOrLeft)
                g.RotateTransform(180);

            float gateSize = componentSize.Height / 2.0f - 2;

            float gateEdgeX = (float)Math.Sin(2 * Math.PI / 400 * openingClearance) * gateSize;
            float gateEdgeY = gateSize - (float)Math.Cos(2 * Math.PI / 400 * openingClearance) * gateSize;

            using (Pen gatePen = new Pen(GateColor, 2.0f)) {
                g.DrawLine(gatePen, 0, gateSize, gateEdgeX, gateEdgeY);
                g.DrawLine(gatePen, 0, -gateSize + 1, gateEdgeX, -gateEdgeY + 1);

                using (Brush columnBrush = new SolidBrush(ColumnsColor)) {
                    g.FillEllipse(columnBrush, -3, componentSize.Height / 2 - 6, 6, 6);
                    g.FillEllipse(columnBrush, -3, -(componentSize.Height / 2), 6, 6);
                }
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

            g.FillEllipse(Brushes.Black, new RectangleF(new PointF(ContactOffset, ComponentSize.Height / 2.0f - ContactSize.Height / 2.0f), ContactSize));
            g.FillEllipse(Brushes.Black, new RectangleF(new PointF(ComponentSize.Width - ContactOffset - ContactSize.Width, ComponentSize.Height / 2.0f - ContactSize.Height / 2.0f), ContactSize));

            if (IsSwitch)
                g.DrawLine(SwitchPen, side0, SwitchState != 0 ? side1 : new PointF(side1.X, side1.Y - (side1.X - side0.X)));
            else {
                PointF switchTipBottom = new PointF(ComponentSize.Width / 2.0f, ComponentSize.Height - SelectorTipOffset);
                PointF switchTip = new PointF(ComponentSize.Width / 2.0f, ComponentSize.Height - SelectorTipOffset - SelectorTipLength);

                g.DrawLine(SwitchPen, switchTipBottom, switchTip);
                g.DrawLine(SwitchPen, switchTip, stateToPoint[SwitchState + 1]);
                g.FillEllipse(Brushes.Black, new RectangleF(new PointF(switchTipBottom.X - ContactSize.Width / 2.0f, switchTipBottom.Y - ContactSize.Height / 2.0f), ContactSize));
            }
        }
    }
}
