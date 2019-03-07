using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for LocomotiveFront.
    /// </summary>
    public class LocomotiveFront : System.Windows.Forms.Control {
        IList<LayoutComponentConnectionPoint> connectionPoints = null;
        LayoutComponentConnectionPoint front = LayoutComponentConnectionPoint.Empty;
        String locomotiveName = null;
        Cursor saveCursor = null;
        const int inMargin = 8;
        const int edgeSize = 6;

        public LocomotiveFront() {
        }

        [Browsable(false)]
        public IList<LayoutComponentConnectionPoint> ConnectionPoints {
            get {
                if (connectionPoints == null)
                    return Array.AsReadOnly<LayoutComponentConnectionPoint>(new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.R });
                return connectionPoints;
            }

            set {
                connectionPoints = value;
                if (connectionPoints != null)
                    front = connectionPoints[0];
            }
        }

        [Browsable(false)]
        [DefaultValue(LayoutComponentConnectionPoint.Empty)]
        public LayoutComponentConnectionPoint Front {
            get {
                return front;
            }

            set {
                if (!DesignMode && value != LayoutComponentConnectionPoint.Empty && connectionPoints != null && value != connectionPoints[0] && value != connectionPoints[1])
                    throw new ArgumentException("Invalid front value - not one of the connection points");
                front = value;
            }
        }

        public String LocomotiveName {
            get {
                return locomotiveName;
            }

            set {
                locomotiveName = value;
            }
        }

        private Point getEdgeLocation(LayoutComponentConnectionPoint cp, int margin) {
            Size offset = LayoutTrackComponent.GetConnectionOffset(cp);
            Point result = new Point(0, 0) {
                X = offset.Width * (ClientSize.Width / 2 - margin),
                Y = offset.Height * (ClientSize.Height / 2 - margin)
            };

            return result;
        }

        protected override void OnPaint(PaintEventArgs pe) {
            GraphicsState gs = pe.Graphics.Save();

            // Move origin to center of control
            pe.Graphics.TranslateTransform(ClientSize.Width / 2, ClientSize.Height / 2);

            Point[] edgeLocations = new Point[2];
            Point[] trackPoints = new Point[2];

            for (int i = 0; i < 2; i++) {
                edgeLocations[i] = getEdgeLocation(ConnectionPoints[i], inMargin);
                edgeLocations[i].X -= edgeSize / 2;
                edgeLocations[i].Y -= edgeSize / 2;
                trackPoints[i] = getEdgeLocation(ConnectionPoints[i], inMargin + edgeSize + 2);
            }

            Color trackColor = Enabled ? Color.Black : Color.LightGray;

            // paint the track
            using (Pen trackPen = new Pen(trackColor, 4.0F)) {
                pe.Graphics.DrawLine(trackPen, trackPoints[0], trackPoints[1]);
            }

            // Draw the edge front squars
            for (int i = 0; i < 2; i++) {
                Brush fillColor;

                if (Enabled)
                    fillColor = front == ConnectionPoints[i] ? Brushes.Red : Brushes.White;
                else
                    fillColor = Brushes.LightGray;

                pe.Graphics.FillRectangle(fillColor,
                    new Rectangle(edgeLocations[i], new Size(edgeSize, edgeSize)));
                pe.Graphics.DrawRectangle(Pens.Black,
                    new Rectangle(edgeLocations[i], new Size(edgeSize, edgeSize)));
            }

            LayoutManager.View.LocomotivePainter locoPainter = new LayoutManager.View.LocomotivePainter();

            if (!Enabled)
                locoPainter.BackgroundBrush = Brushes.LightGray;

            locoPainter.Front = front;
            if (locomotiveName != null)
                locoPainter.Label = locomotiveName;
            locoPainter.DrawFront = true;

            locoPainter.Draw(pe.Graphics);

            pe.Graphics.Restore(gs);

            base.OnPaint(pe);
        }

        // Mouse hit test
        private Rectangle getEdgeHitRectangle(int iPoint) {
            Point edgeCenter = getEdgeLocation(ConnectionPoints[iPoint], inMargin);

            edgeCenter.X += ClientSize.Width / 2 - edgeSize / 2 - 6;
            edgeCenter.Y += ClientSize.Height / 2 - edgeSize / 2 - 6;

            return new Rectangle(edgeCenter, new Size(edgeSize + 12, edgeSize + 12));
        }

        private int getHitEdgeIndex(Point mouse) {
            for (int i = 0; i < 2; i++) {
                Rectangle edgeRect = getEdgeHitRectangle(i);

                if (edgeRect.Contains(mouse))
                    return i;
            }

            return -1;
        }

        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);

            saveCursor = Cursor.Current;
        }

        protected override void OnMouseLeave(EventArgs e) {
            Cursor.Current = saveCursor;

            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if (getHitEdgeIndex(new Point(e.X, e.Y)) >= 0)
                Cursor.Current = Cursors.Default;
            else
                Cursor.Current = Cursors.No;
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            int hitEdge = getHitEdgeIndex(new Point(e.X, e.Y));

            if (hitEdge >= 0) {
                front = ConnectionPoints[hitEdge];
                Invalidate();
            }
        }
    }
}
