using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.View {
    #region Drawing Region interface and helper classes

    public interface ILayoutDrawingRegion {
        /// <summary>
        /// Return a region object describing the bounds of the drawn region. The region is in model points
        /// </summary>
        Region BoundingRegionInModelCoordinates {
            get;
        }

        /// <summary>
        /// Return the Z order of that drawing region. Regions with smaller Z order are drawn behind
        /// regions with higher Z order.
        /// </summary>
        int ZOrder {
            get;
        }

        /// <summary>
        /// Draw the region
        /// </summary>
        /// <param name="view">The view in which the region needs to be drawn</param>
        /// <param name="selectionLook">If region is selected, encupsolate selection visual cues</param>
        /// <param name="g">The graphics on which the region is to be drawn</param>
        void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g);

        /// <summary>
        /// If not null this will be called if region is clicked
        /// </summary>
        Func<bool>? ClickHandler { get; set; }

        /// <summary>
        /// If not null this will be called if region is right clicked
        /// </summary>
        Func<bool>? RightClickHandler { get; set; }

        /// <summary>
        /// Return true if the region can be clicked (clicking on it will be considered as clicking on the component)
        /// </summary>
        bool CanBeClicked {
            get;
        }
    }

    public abstract class LayoutDrawingRegion : ILayoutDrawingRegion {
        static readonly Region emptyRegion = new();

        protected LayoutDrawingRegion(ModelComponent component) {
            ZOrder = component.ZOrder;
            BoundingRegionInModelCoordinates = emptyRegion;
        }

        public Region BoundingRegionInModelCoordinates { get; protected init; }

        public virtual int ZOrder { get; }

        public Func<bool>? ClickHandler { get; set; }

        public Func<bool>? RightClickHandler { get; set; }

        public virtual bool CanBeClicked => true;

        public virtual void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
            if (selectionLook != null) {
                RectangleF rc = BoundingRegionInModelCoordinates.GetBounds(g);
                Pen p = new(selectionLook.Color, view.LineWidthInModelCoordinates) {
                    Alignment = System.Drawing.Drawing2D.PenAlignment.Center
                };
                g.DrawRectangle(p, view.LineWidthInModelCoordinates / 2.0F, view.LineWidthInModelCoordinates / 2.0F, rc.Width - (1.5F * view.LineWidthInModelCoordinates), rc.Height - (1.5F * view.LineWidthInModelCoordinates));
                p.Dispose();
            }
        }
    }

    /// <summary>
    /// Base class for drawing region class that draw in the grid. All the derived class has to do is to draw
    /// the grid.
    /// </summary>
    public abstract class LayoutDrawingRegionGrid : LayoutDrawingRegion {
        protected LayoutDrawingRegionGrid(ModelComponent component, ILayoutView view) : base(component) {
            BoundingRegionInModelCoordinates = new Region(view.ModelLocationRectangleInModelCoordinates(component.Location));
        }

        /// <summary>
        /// Return true if the component grid is visible. The component and the view are extracted
        /// from the event structure
        /// </summary>
        /// <param name="e">LayoutGetDrawingRegion event structure</param>
        /// <returns></returns>
        public static bool IsComponentGridVisible(LayoutGetDrawingRegionsEvent e) => e.View.DrawingRectangleInModelCoordinates.IntersectsWith(e.View.ModelLocationRectangleInModelCoordinates(e.Component.Location));
    }

    /// <summary>
    /// Base class for drawing region that draws a rectangle whose relative to a grid location is provided
    /// </summary>
    public abstract class LayoutDrawingRegionRectangle : LayoutDrawingRegion {
        private float GetAlignedValue(float v, float d, LayoutDrawingAnchorPoint a) => a switch
        {
            LayoutDrawingAnchorPoint.Center => v - (d / 2.0f),
            LayoutDrawingAnchorPoint.Left => v,
            LayoutDrawingAnchorPoint.Right => v - d,
            _ => throw new ArgumentException("Invalid Anchor point value"),
        };

        /// <summary>
        /// Construct a drawing region for drawing a rectangle with relative position to a model grid
        /// location
        /// </summary>
        /// <param name="component">The component to which relate the position</param>
        /// <param name="view">The view</param>
        /// <param name="positionProvider">Provide details about the relative position</param>
        /// <param name="rectSize">The rectangle size</param>
        protected LayoutDrawingRegionRectangle(ModelComponent component, ILayoutView view,
            LayoutPositionInfo positionProvider, SizeF rectSize) : base(component) {
            PointF ptTopLeft = view.ModelLocationInModelCoordinates(component.Location);
            PointF origin = new(ptTopLeft.X + (view.GridSizeInModelCoordinates.Width / 2), ptTopLeft.Y + (view.GridSizeInModelCoordinates.Height / 2));

            var rcRegion = positionProvider.Side switch
            {
                LayoutDrawingSide.Top => new RectangleF(new PointF(GetAlignedValue(origin.X, rectSize.Width, positionProvider.AnchorPoint),
                                            origin.Y - rectSize.Height - positionProvider.Distance), rectSize),

                LayoutDrawingSide.Bottom => new RectangleF(
                        new PointF(GetAlignedValue(origin.X, rectSize.Width, positionProvider.AnchorPoint),
                        origin.Y + positionProvider.Distance), rectSize),

                LayoutDrawingSide.Left => new RectangleF(
                        new PointF(origin.X - rectSize.Width - positionProvider.Distance,
                        GetAlignedValue(origin.Y, rectSize.Height, positionProvider.AnchorPoint)), rectSize),

                LayoutDrawingSide.Right => new RectangleF(new PointF(origin.X + positionProvider.Distance,
                        GetAlignedValue(origin.Y, rectSize.Height, positionProvider.AnchorPoint)), rectSize),

                LayoutDrawingSide.Center => new RectangleF(new PointF(origin.X - (rectSize.Width / 2), origin.Y - (rectSize.Height / 2)),
                        rectSize),

                _ => throw new ArgumentException("Invalid LayoutDrawingSide value"),
            };
            BoundingRegionInModelCoordinates = new Region(rcRegion);
        }
    }

    public abstract class LayoutDrawingRegionBallon : LayoutDrawingRegion {
        private SizeF ballonContentSize;
        private readonly float hangSize = 0.25f;

        protected LayoutDrawingRegionBallon(ModelComponent component, ILayoutView view, SizeF ballonContentSize) : base(component) {
            this.ballonContentSize = ballonContentSize;

            bool horizontal = true;
            var track = component.Spot.Track;
            PointF ptTopLeft = view.ModelLocationInModelCoordinates(component.Location);
            PointF ptCenter = new(ptTopLeft.X + (view.GridSizeInModelCoordinates.Width / 2), ptTopLeft.Y + (view.GridSizeInModelCoordinates.Height / 2));
            SizeF totalContentSize = new(ballonContentSize.Width + (Margins.Width * 2), ballonContentSize.Height + (Margins.Height * 2));
            bool onTop = true;
            bool onRight = true;
            float xBallon;
            float yBallon;

            if (ptCenter.Y < view.DrawingRectangleInModelCoordinates.Height / 2)
                onTop = false;

            if (ptCenter.X > view.DrawingRectangleInModelCoordinates.Width / 2)
                onRight = false;

            if (track != null && LayoutTrackComponent.IsVertical(track))
                horizontal = false;

            if (horizontal) {
                Painter.Hotspot = new PointF(ptCenter.X, ptCenter.Y + (onTop ? -4 : 4));

                if (onRight) {
                    xBallon = ptCenter.X - (hangSize * totalContentSize.Width);
                    Painter.HotspotOrigin = onTop ? 5 : 0;
                }
                else {
                    xBallon = ptCenter.X - ((1 - hangSize) * totalContentSize.Width);
                    Painter.HotspotOrigin = onTop ? 4 : 1;
                }

                if (onTop)
                    yBallon = ptCenter.Y - totalContentSize.Height - 32;
                else
                    yBallon = ptCenter.Y + 32;
            }
            else {
                Painter.Hotspot = new PointF(ptCenter.X + (onRight ? 4 : -4), ptCenter.Y);

                if (onTop) {
                    yBallon = ptCenter.Y - (hangSize * totalContentSize.Height);
                    Painter.HotspotOrigin = onRight ? 6 : 3;
                }
                else {
                    yBallon = ptCenter.Y - ((1 - hangSize) * totalContentSize.Height);
                    Painter.HotspotOrigin = onRight ? 7 : 2;
                }

                if (onRight)
                    xBallon = ptCenter.X + 32;
                else
                    xBallon = ptCenter.X - totalContentSize.Width - 32;
            }

            Painter.Bounds = new RectangleF(new PointF(xBallon, yBallon), totalContentSize);

            PointF offsetAmount;

            using (GraphicsPath p = Painter.BallonGraphicPath) {
                RectangleF bounds = p.GetBounds();

                bounds.Inflate(6, 6);

                BoundingRegionInModelCoordinates = new Region(bounds);
                offsetAmount = new PointF(bounds.Left + 3, bounds.Top + 3);
                offsetAmount.X *= -1;
                offsetAmount.Y *= -1;
            }

            RectangleF newBounds = Painter.Bounds;

            newBounds.Offset(offsetAmount);
            Painter.Bounds = newBounds;
            Painter.Hotspot = new PointF(Painter.Hotspot.X + offsetAmount.X, Painter.Hotspot.Y + offsetAmount.Y);
        }

        public override int ZOrder => 100;

        protected BallonPainter Painter { get; } = new BallonPainter();

        public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
            GraphicsState gs = g.Save();

            g.TranslateTransform(3, 3);
            Painter.Paint(g);

            g.TranslateTransform(Painter.Bounds.Location.X + Margins.Width, Painter.Bounds.Location.Y + Margins.Height);
            g.SetClip(new RectangleF(new PointF(0, 0), ballonContentSize));
            DrawBallonContent(g);

            g.Restore(gs);

            base.Draw(view, detailLevel, null, g);
        }

        /// <summary>
        /// Draw the ballon content. The graphics canvas is set so 0, 0 is the top left of the ballon content area
        /// </summary>
        /// <param name="g"></param>
        protected abstract void DrawBallonContent(Graphics g);

        /// <summary>
        /// The margins around the content in the ballon
        /// </summary>
        protected SizeF Margins => new(4, 4);

        #region IDisposable Members

        virtual public void Dispose() {
            if (Painter != null)
                Painter.Dispose();
        }

        #endregion
    }

    public class LayoutDrawingRegionBallonText : LayoutDrawingRegionBallon {
        private readonly string text;

        public LayoutDrawingRegionBallonText(ModelComponent component, ILayoutView view, Graphics g, string text, Font font) : base(component, view, GetBallonContentSize(g, text, font)) {
            this.text = text;
            this.Font = font;
        }

        static protected SizeF GetBallonContentSize(Graphics g, string text, Font font) {
            SizeF contentSize = g.MeasureString(text, font);

            contentSize.Width += 2;
            contentSize.Height += 2;

            return contentSize;
        }

        protected override void DrawBallonContent(Graphics g) {
            g.DrawString(text, Font, TextBrush, Margins.Width, Margins.Height);
        }

        public Brush TextBrush { get; set; } = Brushes.Black;

        protected Font Font { get; }
    }

    public class LayoutDrawingRegionBallonInfo : LayoutDrawingRegionBallonText {
        public LayoutDrawingRegionBallonInfo(ModelComponent component, ILayoutView view, Graphics g, LayoutBlockBallon ballonInfo) :
            base(component, view, g, ballonInfo.Text, new Font("Arial", ballonInfo.FontSize, GraphicsUnit.World)) {
            Painter.Fill = new SolidBrush(ballonInfo.FillColor);
            TextBrush = new SolidBrush(ballonInfo.TextColor);

            ClickHandler = () => {
                if (ballonInfo.RemoveOnClick) {
                    var blockDefinition = (LayoutBlockDefinitionComponent)component;

                    LayoutBlockBallon.Remove(blockDefinition, LayoutBlockBallon.TerminationReason.Clicked);
                    return true;
                }
                return false;
            };
        }

        #region IDisposable Members

        public override void Dispose() {
            Font.Dispose();
            Painter.Fill.Dispose();
            TextBrush.Dispose();
            base.Dispose();
        }

        #endregion
    }

    public class LayoutDrawingRegionPopupBallon : LayoutDrawingRegionBallon {
        private readonly Ballon ballon;

        public LayoutDrawingRegionPopupBallon(ModelComponent component, ILayoutView view, Graphics g, Ballon ballon) : base(component, view, ballon.Content.GetSize(g)) {
            this.ballon = ballon;
        }

        protected override void DrawBallonContent(Graphics g) {
            ballon.Content.Paint(g);
        }
    }

    /// <summary>
    /// Drawing region for drawing text
    /// </summary>
    public class LayoutDrawingRegionText : LayoutDrawingRegionRectangle {
        private readonly LayoutTextInfo textProvider;

        public bool ForceDraw { get; set; }

        public LayoutDrawingRegionText(ModelComponent component, ILayoutView view, ViewDetailLevel detailLevel, Graphics g, LayoutTextInfo textProvider) :
            base(component, view, textProvider.PositionProvider, MeasureProviderString(g, detailLevel, textProvider)) {
            this.textProvider = textProvider;
        }

        public LayoutDrawingRegionText(LayoutGetDrawingRegionsEvent e, LayoutTextInfo textProvider)
            : this(e.Component, e.View, e.DetailLevel, e.Graphics, textProvider) {
        }

        public LayoutDrawingRegionText(LayoutGetDrawingRegionsEvent e)
            : this(e, new LayoutTextInfo(e.Component)) {
        }

        public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
            base.Draw(view, detailLevel, selectionLook, g);

            if (textProvider.Element != null && (textProvider.Visible || ForceDraw)) {
                int width = textProvider.PositionProvider.Width;
                Color fontColor = textProvider.FontProvider.Color;

                if (detailLevel == ViewDetailLevel.High || textProvider.FontProvider.Font.Height > 14) {
                    using SolidBrush fontBrush = new(fontColor);
                    if (width == 0)
                        g.DrawString(textProvider.Text, textProvider.FontProvider.Font, fontBrush, new Point(0, 0));
                    else
                        g.DrawString(textProvider.Text, textProvider.FontProvider.Font, fontBrush,
                            new RectangleF(new PointF(0, 0), new SizeF(width, 10000.0F)), new StringFormat());
                }
                else {
                    SizeF size = MeasureProviderString(g, detailLevel, textProvider);
                    RectangleF textArea = new(new PointF(0, 0), size);

                    using Pen p = new(fontColor, 3.0f) {
                        DashStyle = DashStyle.DashDotDot
                    };
                    float y = (float)((textArea.Bottom - textArea.Top) / 2.0);

                    g.DrawLine(p, new PointF(textArea.Left, y), new PointF(textArea.Right, y));
                }
            }
        }

        static private SizeF MeasureProviderString(Graphics g, ViewDetailLevel detailLevel, LayoutTextInfo textProvider) {
            if (textProvider.Element != null) {
                int width = textProvider.PositionProvider.Width;

                if (width == 0) {
                    if (detailLevel == ViewDetailLevel.High || textProvider.FontProvider.Font.Height > 14)
                        return g.MeasureString(textProvider.Text, textProvider.FontProvider.Font);
                    else {
                        float h = textProvider.FontProvider.Font.Height;

                        return new SizeF((float)(textProvider.Text.Length * (h * 0.4)), h);
                    }
                }
                else {
                    if (detailLevel == ViewDetailLevel.High || textProvider.FontProvider.Font.Height > 14)
                        return g.MeasureString(textProvider.Text, textProvider.FontProvider.Font, width);
                    else {
                        float h = textProvider.FontProvider.Font.Height;
                        float w = (float)(textProvider.Text.Length * (h * 0.4));

                        return new SizeF(w > width ? width : w, h);
                    }
                }
            }
            else
                return new SizeF(0, 0);
        }
    }

    public class LayoutDrawingRegionNotConnected : LayoutDrawingRegionGrid {
        private readonly IModelComponentHasId component;
        private readonly bool onTop;
        private readonly ImageList imageListComponents;

        public LayoutDrawingRegionNotConnected(ModelComponent component, ILayoutView view, bool onTop) : base(component, view) {
            this.component = (IModelComponentHasId)component;
            imageListComponents = Ensure.NotNull<ImageList>(EventManager.Event(new LayoutEvent("get-components-image-list", this)));
            this.onTop = onTop;
        }

        public LayoutDrawingRegionNotConnected(ModelComponent component, ILayoutView view)
            : this(component, view, false) {
        }

        public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook? selectionLook, Graphics g) {
            // If the component is not connected, mark it
            if (!LayoutModel.ControlManager.ConnectionPoints.IsFullyConnected(component.Id)) {
                var notConnectedImage = imageListComponents.Images[2];

                if(notConnectedImage != null)
                    g.DrawImage(notConnectedImage, new Rectangle(new Point(0, 0), notConnectedImage.Size));
            }
        }

        public override int ZOrder => onTop ? base.ZOrder + 1 : base.ZOrder - 1;        // So it will painted on top or below of the component
    }

    #endregion
}
