using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.View {
    /// <summary>
    /// Summary description for LocomotivePainter.
    /// </summary>
    public class LocomotivePainter : System.ComponentModel.Component, ILayoutComponentPainter, IDisposable {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components;
        private bool frontDefined;
        private LayoutComponentConnectionPoint front;
        private readonly int verticalMargin = 1;
        private readonly int HorizontalMargin = 5;
        private readonly int frontMargin = 4;
        private readonly int motionTriangleHeight = 3;
        private readonly int motionTriangleGap = 1;
        private readonly int extensionMarkSize = 8;
        private readonly ViewDetailLevel detailLevel = ViewDetailLevel.High;

        public LocomotivePainter(IContainer container) {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            container.Add(this);
            InitializeComponent();
        }

        public LocomotivePainter(ViewDetailLevel detailLevel) {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            InitializeComponent();
            this.detailLevel = detailLevel;
        }

        public LocomotivePainter()
            : this(ViewDetailLevel.High) {
        }

        /// <summary>
        /// Given a locomotive state, construct a painter to paint it
        /// </summary>
        /// <param name="trainState"></param>
        public LocomotivePainter(TrainStateInfo trainState, TrainLocationInfo trainLocation) {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            InitializeComponent();

            Speed = trainState.Speed;
            Label = trainState.DisplayName;

            if (trainLocation.IsDisplayFrontKnown) {
                DrawFront = true;
                Front = trainLocation.DisplayFront;
            }
            else {
                LayoutBlockDefinitionComponent blockInfo = LayoutModel.Blocks[trainLocation.BlockId].BlockDefinintion;

                DrawFront = false;
                Front = blockInfo.Track.ConnectionPoints[0];
            }
        }

        #region Properties

        public string Label { set; get; } = "";

        public LayoutComponentConnectionPoint Front {
            get {
                return front;
            }

            set {
                front = value;
                frontDefined = true;
            }
        }

        public LocomotiveOrientation Orientation { get; set; } = LocomotiveOrientation.Forward;

        public LocomotiveOrientation Direction => Speed >= 0 ? LocomotiveOrientation.Forward : LocomotiveOrientation.Backward;

        public bool DrawFront { get; set; }

        public bool DrawLabel { get; set; } = true;

        public int Speed { get; set; }

        public Font LabelFont { get; set; } = new Font("Arial", 7.0F, GraphicsUnit.World);

        public Brush LabelBrush { get; set; } = Brushes.Black;

        public Brush BackgroundBrush { get; set; } = Brushes.WhiteSmoke;

        public Pen FramePen { get; set; } = Pens.Black;

        public Brush MotionBrush { get; set; } = Brushes.Red;

        public SizeF MinSize { get; set; } = new SizeF(20, 12);

        public bool DrawExtensionMark { get; set; }

        #endregion

        #region Operations

        protected SizeF GetLabelSize(Graphics g) {
            SizeF labelSize;

            if (string.IsNullOrEmpty(Label))
                labelSize = MinSize;
            else
                labelSize = g.MeasureString(Label, LabelFont);

            if (labelSize.Width < MinSize.Width)
                labelSize.Width = MinSize.Width;
            if (labelSize.Height < MinSize.Height)
                labelSize.Height = MinSize.Height;

            return labelSize;
        }

        protected SizeF GetLocoRectSize(Graphics g) {
            SizeF labelSize = GetLabelSize(g);

            return new SizeF(labelSize.Width + (2 * HorizontalMargin), labelSize.Height + (2 * verticalMargin));
        }

        public SizeF GetDirectionalLocoSize(Graphics g) {
            SizeF locoRectSize = GetLocoRectSize(g);

            return new SizeF(locoRectSize.Width + (DrawFront ? frontMargin : 0) + (DrawExtensionMark ? extensionMarkSize : 0), locoRectSize.Height);
        }

        public Size Measure(Graphics g) => Size.Ceiling(GetDirectionalLocoSize(g));

        /// <summary>
        /// Draw the locomotive image. It is assumed that the passed graphics is set such
        /// that the center of the drawn locomotive is at (0, 0)
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g) {
            if (!frontDefined)
                throw new ArgumentException("Locomotive Painter 'front' is not set");

            LayoutComponentConnectionPoint drawnFront = Front;

            if (Orientation == LocomotiveOrientation.Backward)
                drawnFront = LayoutTrackComponent.GetPointConnectingTo(Front);

            GraphicsState gs = g.Save();

            switch (drawnFront) {
                case LayoutComponentConnectionPoint.T:
                    g.RotateTransform(-90);
                    break;

                case LayoutComponentConnectionPoint.B:
                    g.RotateTransform(90);
                    break;

                case LayoutComponentConnectionPoint.L:
                    g.RotateTransform(180);
                    break;

                case LayoutComponentConnectionPoint.R:
                    break;
            }

            SizeF[] locoShapeMold = new SizeF[] {
                                                      new SizeF(-0.5F,  0.5F),
                                                      new SizeF( 0.5F,  0.5F),
                                                      new SizeF( 0.5F,  0.0F),
                                                      new SizeF( 0.5F, -0.5F),
                                                      new SizeF(-0.5F, -0.5F)
                                                  };

            PointF[] locoShape = new PointF[locoShapeMold.Length];
            SizeF locoRectSize = GetLocoRectSize(g);
            int i = 0;

            foreach (SizeF s in locoShapeMold)
                locoShape[i++] = new PointF(s.Width * locoRectSize.Width, s.Height * locoRectSize.Height);

            if (DrawFront)
                locoShape[2].X += frontMargin;

            if (BackgroundBrush != null)
                g.FillPolygon(BackgroundBrush, locoShape);
            g.DrawPolygon(FramePen, locoShape);

            if (DrawExtensionMark) {
                RectangleF extensionRect = new RectangleF(new PointF((float)((locoRectSize.Width * -0.5) - extensionMarkSize), (float)(0 - (extensionMarkSize * 0.5))),
                    new SizeF(extensionMarkSize, extensionMarkSize));

                g.FillEllipse(BackgroundBrush, extensionRect);
                g.DrawEllipse(Pens.Black, extensionRect);
            }

            if (Speed != 0) {
                PointF[] triangle = new PointF[3];
                float d = (Direction == LocomotiveOrientation.Forward) ? 1.0F : -1.0F;
                float xTip = ((locoRectSize.Width * 0.5F) - motionTriangleGap);
                float xBase = (xTip - motionTriangleHeight);
                float y = (0.5F * locoRectSize.Height) - verticalMargin;

                if (Orientation == LocomotiveOrientation.Backward)
                    d = -d;

                xBase *= d;
                xTip *= d;

                triangle[0] = new PointF(xBase, y);
                triangle[1] = new PointF(xTip, 0);
                triangle[2] = new PointF(xBase, -y);

                g.FillPolygon(MotionBrush, triangle);
            }

            g.Restore(gs);

            // Draw the label
            if (DrawLabel)
                DrawLocomotiveLabel(g);
        }

        public void DrawLocomotiveLabel(Graphics g) {
            GraphicsState gs = g.Save();

            if (LayoutTrackComponent.IsVertical(Front))
                g.RotateTransform(90);

            SizeF labelSize = GetLabelSize(g);

            StringFormat format = new StringFormat {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            g.DrawString(Label, LabelFont, LabelBrush, new RectangleF(new PointF(-(labelSize.Width / 2.0F), -(labelSize.Height / 2.0F)), labelSize), format);
            g.Restore(gs);
        }

        #endregion

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            components = new Container();
        }
        #endregion

        #region IDisposable Members

        void IDisposable.Dispose() {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }

    public class TrainLocationPainter : ILayoutComponentPainter {
        private readonly TrainLocationInfo trainLocation;
        private readonly int verticalMargin = 2;
        private readonly int horizontalMargin = 2;
        private readonly int gap = 2;
        private readonly LayoutComponentConnectionPoint front;
        private readonly bool drawFront;
        private readonly bool vertical;
        private readonly ViewDetailLevel detailLevel;

        public TrainLocationPainter(TrainLocationInfo trainLocation, ViewDetailLevel detailLevel) {
            this.trainLocation = trainLocation;
            this.LocomotiveState = new TrainStateInfo(trainLocation.LocomotiveStateElement);

            LayoutTrackComponent blockInfoTrack = LayoutModel.Blocks[trainLocation.BlockId].BlockDefinintion.Track;

            this.detailLevel = detailLevel;

            if (trainLocation.IsDisplayFrontKnown) {
                drawFront = true;
                front = trainLocation.DisplayFront;
            }
            else
                front = blockInfoTrack.ConnectionPoints[0];

            vertical = LayoutTrackComponent.IsVertical(blockInfoTrack);
        }

        public TrainStateInfo LocomotiveState { get; }

        public bool ShowTrainDetails { get; set; }

        public Color BackColor { get; set; } = Color.Empty;

        public Brush LabelBrush { get; set; } = Brushes.Black;

        public bool DrawExtensionMark { get; set; }

        public SizeF MinSize { get; set; } = new SizeF(40, 12);

        private Pen getFramePen() {
            Color penColor = Color.Black;

            return LocomotiveState.Locomotives.Count > 1 ? new Pen(penColor, 2.0F) : new Pen(penColor);
        }

        private string GetLocoLabel(LocomotiveInfo loco) {
            if (detailLevel == ViewDetailLevel.High)
                return loco.DisplayName;
            else if (!string.IsNullOrEmpty(loco.CollectionId))
                return loco.CollectionId;
            else return loco.AddressProvider.Element != null ? loco.AddressProvider.Unit.ToString() : loco.DisplayName;
        }

        private string GetTrainLabel(TrainStateInfo train) {
            if (detailLevel == ViewDetailLevel.High)
                return train.DisplayName;
            else {
                bool first = true;
                string s = "";

                foreach (TrainLocomotiveInfo trainLoco in train.Locomotives) {
                    LocomotiveInfo loco = trainLoco.Locomotive;

                    if (!first)
                        s += "+";
                    else
                        first = false;

                    s += GetLocoLabel(loco);
                }

                return s;
            }
        }

        private Font GetLabelFont() {
            return detailLevel == ViewDetailLevel.High
                ? new Font("Arial", 9.0F, GraphicsUnit.World)
                : new Font("Arial", 16.0F, FontStyle.Bold, GraphicsUnit.World);
        }

        private SizeF GetTrainSize(Graphics g, IList<TrainLocomotiveInfo> trainLocos) {
            float x = 0;
            float y = 0;

            foreach (TrainLocomotiveInfo trainLoco in trainLocos) {
                LocomotiveInfo loco = trainLoco.Locomotive;
                LocomotivePainter locoPainter = new LocomotivePainter {
                    Label = GetLocoLabel(loco),
                    LabelFont = GetLabelFont(),
                    Front = front
                };

                if (loco.Kind == LocomotiveKind.SoundUnit)
                    locoPainter.DrawFront = false;
                else
                    locoPainter.DrawFront = drawFront;
                locoPainter.Orientation = trainLoco.Orientation;

                SizeF s = locoPainter.GetDirectionalLocoSize(g);

                x += s.Width + gap;
                if (s.Height > y)
                    y = s.Height;

                locoPainter.LabelFont.Dispose();
            }

            x -= gap;

            return new SizeF(x + (2 * horizontalMargin), y + (2 * verticalMargin));
        }

        public SizeF Measure(Graphics g) {
            SizeF result;

            if (ShowTrainDetails) {
                LocomotivePainter locoPainter = new LocomotivePainter {
                    Front = front,
                    DrawFront = drawFront,
                    MinSize = GetTrainSize(g, LocomotiveState.Locomotives),
                    FramePen = getFramePen(),
                    DrawExtensionMark = DrawExtensionMark
                };

                result = Size.Ceiling(locoPainter.Measure(g));
                locoPainter.FramePen.Dispose();
            }
            else {
                LocomotivePainter framePainter = new LocomotivePainter {
                    Label = GetTrainLabel(LocomotiveState),
                    LabelFont = GetLabelFont(),
                    Front = front,
                    DrawFront = drawFront,
                    Speed = LocomotiveState.Speed,
                    FramePen = getFramePen(),
                    MinSize = MinSize,
                    DrawExtensionMark = DrawExtensionMark
                };

                result = Size.Ceiling(framePainter.Measure(g));
                framePainter.LabelFont.Dispose();
                framePainter.FramePen.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Draw the locomotive set image. It is assumed that the passed graphics is set such
        /// that the center of the drawn locomotive is at (0, 0)
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g) {
            if (ShowTrainDetails) {
                LocomotivePainter framePainter = new LocomotivePainter {
                    Label = GetTrainLabel(LocomotiveState),
                    LabelFont = GetLabelFont(),
                    Front = front,
                    DrawFront = drawFront,
                    Speed = LocomotiveState.Speed,
                    MinSize = GetTrainSize(g, LocomotiveState.Locomotives),
                    DrawLabel = false,
                    FramePen = getFramePen(),
                    DrawExtensionMark = DrawExtensionMark,
                    LabelBrush = LabelBrush
                };

                if (BackColor != Color.Empty)
                    framePainter.BackgroundBrush = new SolidBrush(BackColor);

                framePainter.Draw(g);

                // Draw the set members

                SizeF locoSetSize = GetTrainSize(g, LocomotiveState.Locomotives);
                float x = (-locoSetSize.Width / 2) + horizontalMargin;

                foreach (TrainLocomotiveInfo trainLoco in LocomotiveState.Locomotives) {
                    LocomotiveInfo loco = trainLoco.Locomotive;
                    LocomotivePainter locoPainter = new LocomotivePainter {
                        Label = GetLocoLabel(loco),
                        LabelFont = GetLabelFont(),
                        LabelBrush = Brushes.LightGray,
                        FramePen = Pens.LightGray,
                        MotionBrush = Brushes.LightGray,
                        Front = front,
                        Speed = LocomotiveState.Speed
                    };

                    if (loco.Kind == LocomotiveKind.SoundUnit)
                        locoPainter.DrawFront = false;
                    else
                        locoPainter.DrawFront = drawFront;
                    locoPainter.Orientation = trainLoco.Orientation;

                    SizeF locoSize = locoPainter.GetDirectionalLocoSize(g);

                    GraphicsState gs = g.Save();

                    if (vertical)
                        g.TranslateTransform(0, x + (locoSize.Width / 2));
                    else
                        g.TranslateTransform(x + (locoSize.Width / 2), 0);

                    locoPainter.Draw(g);
                    g.Restore(gs);

                    x += locoSize.Width + gap;

                    locoPainter.LabelFont.Dispose();
                }

                framePainter.DrawLocomotiveLabel(g);

                framePainter.FramePen.Dispose();
                framePainter.LabelFont.Dispose();
                if (BackColor != Color.Empty)
                    framePainter.BackgroundBrush.Dispose();
            }
            else {
                LocomotivePainter framePainter = new LocomotivePainter {
                    Label = GetTrainLabel(LocomotiveState),
                    LabelFont = GetLabelFont(),
                    LabelBrush = LabelBrush,
                    Front = front,
                    DrawFront = drawFront,
                    Speed = LocomotiveState.Speed,
                    FramePen = getFramePen(),
                    DrawExtensionMark = DrawExtensionMark,
                    MinSize = MinSize
                };

                if (BackColor != Color.Empty)
                    framePainter.BackgroundBrush = new SolidBrush(BackColor);

                framePainter.Draw(g);

                framePainter.FramePen.Dispose();
                framePainter.LabelFont.Dispose();

                if (BackColor != Color.Empty)
                    framePainter.BackgroundBrush.Dispose();
            }
        }
    }
}
