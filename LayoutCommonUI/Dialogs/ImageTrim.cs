using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace LayoutManager.CommonUI.Dialogs {
    public partial class ImageTrim : Form {
        /// <summary>
        /// The input image to be trimmed
        /// </summary>
        public Image InputImage { get; private set; }

        /// <summary>
        /// The required size of the timmed image
        /// </summary>
        public Size RequiredSize { get; private set; }

        /// <summary>
        /// The resulted trimmed image
        /// </summary>
        public Image Image { get => Ensure.NotNull<Image>(OptionalImage); private set => OptionalImage = value; }

        private Image? OptionalImage { get; set; }

        public Size Insets = new(10, 10);

        private Rectangle _clipFrame;

        private Rectangle _startDragClipFrame;
        private Point _startDragLocation;

        private enum Dragging {
            None = -1,
            CornerNW = 0,
            CornerNE = 1,
            CornerSW = 2,
            CornerSE = 3,
            ClipFrame = 4
        };

        // Can be NotDragging, 0..3 dragging a corner or DraggingClipRect
        private Dragging _dragging = Dragging.None;

        public ImageTrim(Image inputImage, Size requiredSize) {
            InitializeComponent();

            this.InputImage = inputImage;
            this.RequiredSize = requiredSize;

            _clipFrame = InitClipFrame();

            panelImage.Paint += PanelImage_Paint;
            panelImage.SizeChanged += (sender, e) => panelImage.Invalidate();
            panelImage.MouseMove += PanelImage_MouseMove;
            panelImage.MouseUp += (sender, e) => _dragging = Dragging.None;

            panelImage.MouseDown += (sender, e) => {
                _startDragClipFrame = _clipFrame;
                _startDragLocation = e.Location;
                _dragging = GetDragRegion(e.Location);
            };
        }

        private (int, int) AdjustForCornerDragging(int dx, int dy) => (dx, dx * RequiredSize.Height / RequiredSize.Width);

        private void PanelImage_MouseMove(object? sender, MouseEventArgs e) {
            if (_dragging != Dragging.None) {
                var transformMatrix = GetTransformMatrix();
                transformMatrix.Invert();

                var points = new Point[] { _startDragLocation, e.Location };
                transformMatrix.TransformPoints(points);

                var dx = points[1].X - points[0].X;
                var dy = points[1].Y - points[0].Y;

                if (_dragging != Dragging.ClipFrame)
                    (dx, dy) = AdjustForCornerDragging(dx, dy);

                Trace.WriteLine($"Dragging {_dragging} from {points[0]} to {points[1]} dx: {dx} dy: {dy}");

                switch (_dragging) {
                    case Dragging.ClipFrame:
                        _clipFrame = _startDragClipFrame;
                        _clipFrame.Offset(dx, dy);
                        break;

                    case Dragging.CornerNW:
                        if (_startDragClipFrame.Width - dx > 0 && _startDragClipFrame.Height - dy > 0)
                            _clipFrame = new Rectangle(_startDragClipFrame.X + dx, _startDragClipFrame.Y + dy, _startDragClipFrame.Width - dx, _startDragClipFrame.Height - dy);
                        break;

                    case Dragging.CornerNE:
                        if (_startDragClipFrame.Width + dx > 0 && _startDragClipFrame.Height + dy > 0)
                            _clipFrame = new Rectangle(_startDragClipFrame.X, _startDragClipFrame.Y - dy, _startDragClipFrame.Width + dx, _startDragClipFrame.Height + dy);
                        break;

                    case Dragging.CornerSW:
                        if (_startDragClipFrame.Width - dx > 0 && _startDragClipFrame.Height - dy > 0)
                            _clipFrame = new Rectangle(_startDragClipFrame.X + dx, _startDragClipFrame.Y, _startDragClipFrame.Width - dx, _startDragClipFrame.Height - dy);
                        break;

                    case Dragging.CornerSE:
                        if (_startDragClipFrame.Width - dx > 0 && _startDragClipFrame.Height + dy > 0)
                            _clipFrame = new Rectangle(_startDragClipFrame.X, _startDragClipFrame.Y, _startDragClipFrame.Width + dx, _startDragClipFrame.Height + dy);
                        break;
                }

                panelImage.Invalidate();
            }
            else {
                switch (GetDragRegion(e.Location)) {
                    case Dragging.None: panelImage.Cursor = Cursors.Arrow; break;
                    case Dragging.CornerNW: case Dragging.CornerSE: panelImage.Cursor = Cursors.SizeNWSE; break;
                    case Dragging.CornerNE: case Dragging.CornerSW: panelImage.Cursor = Cursors.SizeNESW; break;
                    case Dragging.ClipFrame: panelImage.Cursor = Cursors.Hand; break;
                }
            }
        }

        private Dragging GetDragRegion(Point pt) {
            Size cornerHotZone = new(20, 20);

            foreach (var corner in Enumerable.Range(0, 4))
                if (Transformed(GetClipFrameCornerRect(corner, cornerHotZone)).IsVisible(pt))
                    return (Dragging)corner;

            return Transformed(_clipFrame).IsVisible(pt) ? Dragging.ClipFrame : Dragging.None;
        }

        private Rectangle InitClipFrame() {
            Size clipFrameSize;
            Point clipFrameOrigin;
            if (RequiredSize.Width > RequiredSize.Height) {
                clipFrameSize = new Size(InputImage.Width, InputImage.Width * RequiredSize.Height / RequiredSize.Width);
                clipFrameOrigin = new Point(0, (InputImage.Height - clipFrameSize.Height) / 2);
            }
            else {
                clipFrameSize = new Size(InputImage.Width * RequiredSize.Width / RequiredSize.Height, InputImage.Height);
                clipFrameOrigin = new Point((InputImage.Width - clipFrameSize.Width) / 2, 0);
            }

            return new Rectangle(clipFrameOrigin, clipFrameSize);
        }

        private void PanelImage_Paint(object? sender, PaintEventArgs e) {
            e.Graphics.FillRectangle(SystemBrushes.Window, e.ClipRectangle);
            var state = e.Graphics.Save();

            e.Graphics.Transform = GetTransformMatrix();
            e.Graphics.DrawImage(this.InputImage, new Rectangle(new Point(0, 0), this.InputImage.Size));

            DrawClipFrame(e.Graphics);
            e.Graphics.Restore(state);
        }

        private Matrix GetTransformMatrix() {
            var m = new Matrix();
            var displayRect = new Rectangle(0, 0, panelImage.Width - (Insets.Width * 2), panelImage.Height - (Insets.Height * 2));
            float scale = Math.Min((float)displayRect.Height / InputImage.Height, (float)displayRect.Width / InputImage.Width);
            float xOffset = (displayRect.Width - (InputImage.Width * scale)) / 2.0f;
            float yOffset = (displayRect.Height - (InputImage.Height * scale)) / 2.0f;

            m.Translate(xOffset + Insets.Width, yOffset + Insets.Height);
            m.Scale(scale, scale);

            return m;
        }

        private Region Transformed(Rectangle rect) {
            var r = new Region(rect);

            r.Transform(GetTransformMatrix());
            return r;
        }

        private void DrawClipFrame(Graphics g) {
            var cornerSize = new Size(10, 10);

            g.DrawRectangle(Pens.Black, _clipFrame);

            foreach (var corner in Enumerable.Range(0, 4))
                g.FillRectangle(Brushes.Black, GetClipFrameCornerRect(corner, cornerSize));
        }

        private Rectangle GetClipFrameCornerRect(int corner, Size cornerSize) {
            return new Rectangle(new Point(
                (corner & 1) == 0 ? _clipFrame.X : _clipFrame.X + _clipFrame.Width - cornerSize.Width,
                (corner & 2) == 0 ? _clipFrame.Y : _clipFrame.Y + _clipFrame.Height - cornerSize.Height),
                cornerSize);
        }

        private void ButtonOK_Click(object? sender, EventArgs e) {
            var result = new Bitmap(RequiredSize.Width, RequiredSize.Height);

            using (Graphics g = Graphics.FromImage(result)) {
                g.DrawImage(InputImage, new Rectangle(new Point(0, 0), result.Size), _clipFrame.X, _clipFrame.Y, _clipFrame.Width, _clipFrame.Height, GraphicsUnit.Pixel);
            }

            this.Image = result;
        }
    }
}
