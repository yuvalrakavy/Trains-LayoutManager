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
    public class LocomotivePainter : System.ComponentModel.Component, ILayoutComponentPainter, IDisposable
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components;

		String							label = "";
		bool							frontDefined;
		bool							drawFront;
		bool							drawLabel = true;
		LayoutComponentConnectionPoint	front;
		LocomotiveOrientation			orientation = LocomotiveOrientation.Forward;
		int								speed;
		Font							labelFont = new Font("Arial", 7.0F, GraphicsUnit.World);
		Brush							labelBrush = Brushes.Black;
		Brush							motionBrush = Brushes.Red;
		Brush							backgroundBrush = Brushes.WhiteSmoke;
		Pen								framePen = Pens.Black;
		int								verticalMargin = 1;
		int								HorizontalMargin = 5;
		int								frontMargin = 4;
		int								motionTriangleHeight = 3;
		int								motionTriangleGap = 1;
		int								extensionMarkSize = 8;
		SizeF							minSize = new SizeF(20, 12);
		bool							drawExtensionMark;
		ViewDetailLevel					detailLevel = ViewDetailLevel.High;

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

			if(trainLocation.IsDisplayFrontKnown) {
				DrawFront = true;
				Front = trainLocation.DisplayFront;
			}
			else {
				LayoutBlockDefinitionComponent	blockInfo = LayoutModel.Blocks[trainLocation.BlockId].BlockDefinintion;

				DrawFront = false;
				Front = blockInfo.Track.ConnectionPoints[0];
			}
		}

		#region Properties

		public String Label {
			set {
				label = value;
			}

			get {
				return label;
			}
		}

		public LayoutComponentConnectionPoint Front {
			get {
				return front;
			}

			set {
				front = value;
				frontDefined = true;
			}
		}

		public LocomotiveOrientation Orientation {
			get {
				return orientation;
			}

			set {
				orientation = value;
			}
		}

        public LocomotiveOrientation Direction => Speed >= 0 ? LocomotiveOrientation.Forward : LocomotiveOrientation.Backward;

        public bool DrawFront {
			get {
				return drawFront;
			}

			set {
				drawFront = value;
			}
		}

		public bool DrawLabel {
			get {
				return drawLabel;
			}

			set {
				drawLabel = value;
			}
		}

		public int Speed {
			get {
				return speed;
			}

			set {
				speed = value;
			}
		}

		public Font LabelFont {
			get {
				return labelFont;
			}

			set {
				labelFont = value;
			}
		}

		public Brush LabelBrush {
			get {
				return labelBrush;
			}

			set {
				labelBrush = value;
			}
		}

		public Brush BackgroundBrush {
			get {
				return backgroundBrush;
			}

			set {
				backgroundBrush = value;
			}
		}

		public Pen FramePen {
			get {
				return framePen;
			}

			set {
				framePen = value;
			}
		}

		public Brush MotionBrush {
			get {
				return motionBrush;
			}

			set {
				motionBrush = value;
			}
		}

		public SizeF MinSize {
			get {
				return minSize;
			}

			set {
				minSize = value;
			}
		}

		public bool DrawExtensionMark {
			get {
				return drawExtensionMark;
			}

			set {
				drawExtensionMark = value;
			}
		}

		#endregion

		#region Operations

		protected SizeF GetLabelSize(Graphics g) {
			SizeF	labelSize;

			if(string.IsNullOrEmpty(label))
				labelSize = minSize;
			else
				labelSize = g.MeasureString(label, labelFont);

			if(labelSize.Width < minSize.Width)
				labelSize.Width = minSize.Width;
			if(labelSize.Height < minSize.Height)
				labelSize.Height = minSize.Height;

			return labelSize;
		}

		protected SizeF GetLocoRectSize(Graphics g) {
			SizeF	labelSize = GetLabelSize(g);

			return new SizeF(labelSize.Width + 2*HorizontalMargin, labelSize.Height + 2*verticalMargin);
		}

		public SizeF GetDirectionalLocoSize(Graphics g) {
			SizeF	locoRectSize = GetLocoRectSize(g);

			return new SizeF(locoRectSize.Width + (DrawFront ? frontMargin : 0) + (DrawExtensionMark ? extensionMarkSize : 0), locoRectSize.Height);
		}

        public Size Measure(Graphics g) => Size.Ceiling(GetDirectionalLocoSize(g));

        /// <summary>
        /// Draw the locomotive image. It is assumed that the passed graphics is set such
        /// that the center of the drawn locomotive is at (0, 0)
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g) {
			if(!frontDefined)
				throw new ArgumentException("Locomotive Painter 'front' is not set");

			LayoutComponentConnectionPoint		drawnFront = Front;

			if(Orientation == LocomotiveOrientation.Backward)
				drawnFront = LayoutTrackComponent.GetPointConnectingTo(Front);

			GraphicsState	gs = g.Save();

			switch(drawnFront) {
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

			PointF[]	locoShape = new PointF[locoShapeMold.Length];
			SizeF		locoRectSize = GetLocoRectSize(g);
			int			i = 0;

			foreach(SizeF s in locoShapeMold)
				locoShape[i++] = new PointF(s.Width * locoRectSize.Width, s.Height * locoRectSize.Height);

			if(DrawFront)
				locoShape[2].X += frontMargin;

			if(backgroundBrush != null)
				g.FillPolygon(backgroundBrush, locoShape);
			g.DrawPolygon(framePen, locoShape);

			if(drawExtensionMark) {
				RectangleF extensionRect = new RectangleF(new PointF((float)(locoRectSize.Width * -0.5 - extensionMarkSize), (float)(0-extensionMarkSize*0.5)),
					new SizeF(extensionMarkSize, extensionMarkSize));

				g.FillEllipse(backgroundBrush, extensionRect);
				g.DrawEllipse(Pens.Black, extensionRect);
			}

			if(Speed != 0) {
				PointF[]	triangle = new PointF[3];
				float		d = (Direction == LocomotiveOrientation.Forward) ? 1.0F : -1.0F;
				float		xTip = (locoRectSize.Width*0.5F - motionTriangleGap);
				float		xBase = (xTip - motionTriangleHeight);
				float		y = 0.5F * locoRectSize.Height - verticalMargin;

				if(Orientation == LocomotiveOrientation.Backward)
					d = -d;

				xBase *= d;
				xTip *= d;

				triangle[0] = new PointF(xBase , y);
				triangle[1] = new PointF(xTip, 0);
				triangle[2] = new PointF(xBase, -y);

				g.FillPolygon(motionBrush, triangle);
			}

			g.Restore(gs);

			// Draw the label
			if(drawLabel)
				DrawLocomotiveLabel(g);
		}

		public void DrawLocomotiveLabel(Graphics g) {
			GraphicsState	gs = g.Save();

			if(LayoutTrackComponent.IsVertical(Front))
				g.RotateTransform(90);

			SizeF	labelSize = GetLabelSize(g);

			StringFormat	format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;

			g.DrawString(Label, LabelFont, LabelBrush, new RectangleF(new PointF(-(labelSize.Width / 2.0F), -(labelSize.Height / 2.0F)), labelSize), format);
			g.Restore(gs);
		}

		#endregion

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new Container();
		}
		#endregion
	
#region IDisposable Members

void  IDisposable.Dispose()
{
 	throw new Exception("The method or operation is not implemented.");
}

#endregion
}

	public class TrainLocationPainter : ILayoutComponentPainter {
		TrainLocationInfo				trainLocation;
		TrainStateInfo					trainState;
		int								verticalMargin = 2;
		int								horizontalMargin = 2;
		int								gap = 2;
		LayoutComponentConnectionPoint	front;
		bool							drawFront;
		bool							vertical;
		bool							showTrainDetails;
		Color							backColor = Color.Empty;
		Brush							labelBrush = Brushes.Black;
		bool							drawExtensionMark;
		ViewDetailLevel					detailLevel;
		SizeF							minSize = new SizeF(40, 12);

		public TrainLocationPainter(TrainLocationInfo trainLocation, ViewDetailLevel detailLevel) {
			this.trainLocation = trainLocation;
			this.trainState = new TrainStateInfo(trainLocation.LocomotiveStateElement);

			LayoutTrackComponent	 blockInfoTrack = LayoutModel.Blocks[trainLocation.BlockId].BlockDefinintion.Track;

			this.detailLevel = detailLevel;

			if(trainLocation.IsDisplayFrontKnown) {
				drawFront = true;
				front = trainLocation.DisplayFront;
			}
			else
				front = blockInfoTrack.ConnectionPoints[0];

			vertical = LayoutTrackComponent.IsVertical(blockInfoTrack);
		}

        public TrainStateInfo LocomotiveState => trainState;

        public bool ShowTrainDetails {
			get { return showTrainDetails; }
			set { showTrainDetails = value; }
		}

		public Color BackColor {
			get { return backColor; }
			set { backColor = value; }
		}

		public Brush LabelBrush {
			get { return labelBrush; }
			set { labelBrush = value; }
		}

		public bool DrawExtensionMark {
			get { return drawExtensionMark; }
			set { drawExtensionMark = value; }
		}

		public SizeF MinSize {
			get { return minSize; }
			set { minSize = value; }
		}
	
		private Pen getFramePen() {
			Color	penColor = Color.Black;

			if(trainState.Locomotives.Count > 1)
				return new Pen(penColor, 2.0F);
			else
				return new Pen(penColor);
		}

		string GetLocoLabel(LocomotiveInfo loco) {
			if(detailLevel == ViewDetailLevel.High)
				return loco.DisplayName;
			else if(!string.IsNullOrEmpty(loco.CollectionId))
				return loco.CollectionId;
			else if(loco.AddressProvider.Element != null)
				return loco.AddressProvider.Unit.ToString();
			else
				return loco.DisplayName;
		}

		string GetTrainLabel(TrainStateInfo train) {
			if(detailLevel == ViewDetailLevel.High)
				return train.DisplayName;
			else {
				bool first = true;
				string s = "";

				foreach(TrainLocomotiveInfo trainLoco in train.Locomotives) {
					LocomotiveInfo loco = trainLoco.Locomotive;

					if(!first)
						s += "+";
					else
						first = false;

					s += GetLocoLabel(loco);
				}

				return s;
			}
		}

		Font GetLabelFont() {
			if(detailLevel == ViewDetailLevel.High)
				return new Font("Arial", 9.0F, GraphicsUnit.World);
			else
				return new Font("Arial", 16.0F, FontStyle.Bold, GraphicsUnit.World);
		}
		
		SizeF GetTrainSize(Graphics g, IList<TrainLocomotiveInfo> trainLocos) {
			float							x = 0;
			float							y = 0;

			foreach(TrainLocomotiveInfo trainLoco in trainLocos) {
				LocomotiveInfo			loco = trainLoco.Locomotive;
				LocomotivePainter		locoPainter = new LocomotivePainter();

				locoPainter.Label = GetLocoLabel(loco);
				locoPainter.LabelFont = GetLabelFont();
				locoPainter.Front = front;

				if(loco.Kind == LocomotiveKind.SoundUnit)
					locoPainter.DrawFront = false;
				else
					locoPainter.DrawFront = drawFront;
				locoPainter.Orientation = trainLoco.Orientation;

				SizeF						s = locoPainter.GetDirectionalLocoSize(g);

				x += s.Width + gap;
				if(s.Height > y)
					y = s.Height;

				locoPainter.LabelFont.Dispose();
			}

			x -= gap;

			return new SizeF(x + 2*horizontalMargin, y+2*verticalMargin);
		}

		public SizeF Measure(Graphics g) {
			SizeF		result;

			if(showTrainDetails) {				
				LocomotivePainter	locoPainter = new LocomotivePainter();

				locoPainter.Front = front;
				locoPainter.DrawFront = drawFront;
				locoPainter.MinSize = GetTrainSize(g, trainState.Locomotives);
				locoPainter.FramePen = getFramePen();
				locoPainter.DrawExtensionMark = drawExtensionMark;

				result = Size.Ceiling(locoPainter.Measure(g));
				locoPainter.FramePen.Dispose();
			}
			else {
				LocomotivePainter	framePainter = new LocomotivePainter();

				framePainter.Label = GetTrainLabel(trainState);
				framePainter.LabelFont = GetLabelFont();
				framePainter.Front = front;
				framePainter.DrawFront = drawFront;
				framePainter.Speed = trainState.Speed;
				framePainter.FramePen = getFramePen();
				framePainter.MinSize = minSize;
				framePainter.DrawExtensionMark = drawExtensionMark;

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
			if(showTrainDetails) {
				LocomotivePainter		framePainter = new LocomotivePainter();

				framePainter.Label = GetTrainLabel(trainState);
				framePainter.LabelFont = GetLabelFont();
				framePainter.Front = front;
				framePainter.DrawFront = drawFront;
				framePainter.Speed = trainState.Speed;
				framePainter.MinSize = GetTrainSize(g, trainState.Locomotives);
				framePainter.DrawLabel = false;
				framePainter.FramePen = getFramePen();
				framePainter.DrawExtensionMark = drawExtensionMark;
				framePainter.LabelBrush = labelBrush;

				if(backColor != Color.Empty)
					framePainter.BackgroundBrush = new SolidBrush(backColor);

				framePainter.Draw(g);

				// Draw the set members

				SizeF		locoSetSize = GetTrainSize(g, trainState.Locomotives);
				float		x = -locoSetSize.Width/2 + horizontalMargin;

				foreach(TrainLocomotiveInfo trainLoco in trainState.Locomotives) {
					LocomotiveInfo			loco = trainLoco.Locomotive;
					LocomotivePainter		locoPainter = new LocomotivePainter();

					locoPainter.Label = GetLocoLabel(loco);
					locoPainter.LabelFont = GetLabelFont();
					locoPainter.LabelBrush = Brushes.LightGray;
					locoPainter.FramePen = Pens.LightGray;
					locoPainter.MotionBrush = Brushes.LightGray;
					locoPainter.Front = front;
					locoPainter.Speed = trainState.Speed;

					if(loco.Kind == LocomotiveKind.SoundUnit)
						locoPainter.DrawFront = false;
					else
						locoPainter.DrawFront = drawFront;
					locoPainter.Orientation = trainLoco.Orientation;

					SizeF	locoSize = locoPainter.GetDirectionalLocoSize(g);

					GraphicsState	gs = g.Save();

					if(vertical)
						g.TranslateTransform(0, x + locoSize.Width / 2);
					else
						g.TranslateTransform(x + locoSize.Width / 2, 0);

					locoPainter.Draw(g);
					g.Restore(gs);

					x += locoSize.Width + gap;

					locoPainter.LabelFont.Dispose();
				}

				framePainter.DrawLocomotiveLabel(g);

				framePainter.FramePen.Dispose();
				framePainter.LabelFont.Dispose();
				if(backColor != Color.Empty)
					framePainter.BackgroundBrush.Dispose();
			}
			else {
				LocomotivePainter	framePainter = new LocomotivePainter();

				framePainter.Label = GetTrainLabel(trainState);
				framePainter.LabelFont = GetLabelFont();
				framePainter.LabelBrush = labelBrush;
				framePainter.Front = front;
				framePainter.DrawFront = drawFront;
				framePainter.Speed = trainState.Speed;
				framePainter.FramePen = getFramePen();
				framePainter.DrawExtensionMark = drawExtensionMark;
				framePainter.MinSize = minSize;

				if(backColor != Color.Empty)
					framePainter.BackgroundBrush = new SolidBrush(backColor);

				framePainter.Draw(g);

				framePainter.FramePen.Dispose();
				framePainter.LabelFont.Dispose();

				if(backColor != Color.Empty)
					framePainter.BackgroundBrush.Dispose();
			}
		}
	}
}
