using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.InteropServices;
using System.Diagnostics;

using LayoutManager;
using LayoutManager.Model;

namespace LayoutManager.View {
	/// <summary>
	/// Summary description for LayoutView.
	/// </summary>
	public class LayoutView : System.Windows.Forms.UserControl, ILayoutView, ILayoutViewModelGridToModelCoordinatesSettings {
		/// <summary>
		/// Each component should draw it self in this size
		/// </summary>
		readonly Size			componentSize = new Size(32, 32);		// Component size in world coordinates (model points)
		Size					dcGridLineSize = new Size(1, 1);
		SizeF					dcComponentSize = new SizeF(32, 32);	// Component size in client coordinates
		static BooleanSwitch	showCoordinatesSwitch = new BooleanSwitch("ShowCoordinates", "Show coordinates on grid");


		float					zoom = 1.0F;				// zoom factor
		float					defaultZoom = 1.0F;			// Default view zoom factor

		Point					mlOrigin = new Point(0, 0);	// origin
		Point					mlDefaultOrigin = new Point(0, 0);	// View default origin
		Point					mlScrolledOrigin = new Point(0, 0);		// Origin that scroll bar is set to
		ShowGridLinesOption	showGrid = ShowGridLinesOption.AutoHide;
		bool					showingGrid = true;
		ModelComponent			erasedComponent;			// Do not draw this component, it is erased!

		LayoutModelArea			area;						// The model area that this view shows
		ILayoutFrameWindow		frameWindow;

		// Define various colors
		Color					gridLineColor = Color.LightGray;
		bool					showCoordinates;

		// grid line size in world coordinate (taking into accunt the zoom factor)
		SizeF					mpGridLineSize = new SizeF(1.0F, 1.0F);
		Stack<ILayoutViewModelGridToModelCoordinatesSettings> modelGridToModelCoordinatesStack = new Stack<ILayoutViewModelGridToModelCoordinatesSettings>();

		// Drop support
		enum ScrollDirection {
			None, Up, Down, Left, Right
		};

		ScrollDirection			dropScrollDirection = ScrollDirection.None;

		// Drag support
		Rectangle				dragSourceRectangle = Rectangle.Empty;
		LayoutHitTestResult		draggedHitTestResult = null;

		// Child controls
		private System.Windows.Forms.HScrollBar hScrollBar;
		private System.Windows.Forms.VScrollBar vScrollBar;
		private Timer timerScrollForDrop;
		private IContainer components;

		public LayoutView() {
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			modelGridToModelCoordinatesStack.Push(this);
			Phases = LayoutPhase.All;
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Public Exposed Events

		public event EventHandler<LayoutViewEventArgs>	ModelComponentClick;

		public event EventHandler<LayoutViewEventArgs>	ModelComponentDragged;

		public event EventHandler<LayoutViewEventArgs>	ModelComponentQueryDrop;

		public event EventHandler<LayoutViewEventArgs>	ModelComponentDrop;

		#endregion

		#region Exposed view properties and methods

		/// <summary>
		/// Create a graphics object and apply all the transformation needed so world
		/// coordinates will be correctly rendered.
		/// </summary>
		/// <returns>Graphics object (which should be disposed by calling Dispose)</returns>
		Graphics CreateTransformedGraphics() {
			Graphics	g = CreateGraphics();

			g.ScaleTransform(zoom, zoom);
			return g;
		}

		/// <summary>
		/// Zoom factor. For example zoom of 2 will show double sized image of each component 
		/// </summary>
		public float Zoom {
			set {
				// Round the zoom so that each grid cell will be an integral number of pixels
				double	zoomValue = Math.Round(value * componentSize.Height) / componentSize.Height;

				zoom = (float)zoomValue;

				// calculate the size of a grid line, convert it from its given
				// size in device coordinate to its size in world coordinate
				// (taking the zoom into account)
				using(Graphics g = CreateTransformedGraphics()) {
					PointF[]	pt = new PointF[1];

					pt[0].X = dcGridLineSize.Width;
					pt[0].Y = dcGridLineSize.Height;

					g.TransformPoints(CoordinateSpace.World, CoordinateSpace.Device, pt);

					mpGridLineSize.Width = pt[0].X;
					mpGridLineSize.Height = pt[0].Y;

					// Calculate the component size in device units (pixels). This will be used
					// to convert between client (or screen) coordinates to world coordinates.
					// For example, when calculating the area to invalidate when a component changes
					pt[0].X = GridSizeInModelCoordinates.Width;
					pt[0].Y = GridSizeInModelCoordinates.Height;
					g.TransformPoints(CoordinateSpace.Device, CoordinateSpace.World, pt);
					dcComponentSize.Width = pt[0].X;
					dcComponentSize.Height = pt[0].Y;

					// If the grid line is more than 1/6 of the component size, and the grid
					// mode is auto-hide, prevent drawing of the grid. (it will look funny)
					if(showGrid == ShowGridLinesOption.AutoHide) {
						if(mpGridLineSize.Width > GridSizeInModelCoordinates.Width / 6)
							showingGrid = false;		// grid is too thick relative to the component
						else
							showingGrid = true;
					}
				}

				LayoutController.LayoutModified();

				if(Area != null)
					setScrollBars();
				Invalidate();
			}

			get {
				return zoom;
			}
		}

		public float DefaultZoom {
			get {
				return defaultZoom;
			}

			set {
				defaultZoom = value;
			}
		}

		public ViewDetailLevel DetailLevel {
			get {
				if(zoom < 0.5)
					return ViewDetailLevel.Low;
				else if(zoom < 0.8)
					return ViewDetailLevel.Medium;
				else
					return ViewDetailLevel.High;
			}
		}

		/// <summary>
		/// Layout phases (operational, in-construction, planned) that are visible in this view
		/// </summary>
		public LayoutPhase Phases {
			get;
			set;
		}

		/// <summary>
		/// Frame window containing this view
		/// </summary>
		public ILayoutFrameWindow FrameWindow {
			get {
				if(frameWindow == null)
					frameWindow = FindForm() as ILayoutFrameWindow;
				return frameWindow;
			}
		}

		/// <summary>
		/// The model location to be shown on the top left corner of the view
		/// </summary>
		public Point OriginInModelGridUnits {
			set {
				mlOrigin = value;

				updateScrollBarPosition();

				LayoutController.LayoutModified();
				Invalidate();
			}

			get {
				return mlOrigin;
			}
		}

		public Point DefaultOriginInModelGridUnits {
			set {
				mlDefaultOrigin = value;
			}

			get {
				return mlDefaultOrigin;
			}
		}

		public void SetAsDefaultOriginAndZoom() {
			DefaultZoom = Zoom;
			DefaultOriginInModelGridUnits = OriginInModelGridUnits;
		}

		public void SetOriginAndZoomToDefault() {
			OriginInModelGridUnits = DefaultOriginInModelGridUnits;
			Zoom = DefaultZoom;
		}

		/// <summary>
		/// The grid line color
		/// </summary>
		public Color GridLineColor {
			set {
				gridLineColor = value;
				Invalidate();
			}

			get {
				return gridLineColor;
			}
		}

		public bool ShowCoordinates {
			get {
				return showCoordinates;
			}

			set {
				showCoordinates = value;
				Invalidate();
			}
		}

		/// <summary>
		/// Control whether grid is shown. The possible values are:
		/// show - grid is shown
		/// hide - grid is not shown
		/// autoHide - grid is not shown if the component are too small
		/// </summary>
		public ShowGridLinesOption ShowGrid {
			set {
				showGrid = value;

				switch(showGrid) {
					case ShowGridLinesOption.Show:
						showingGrid = true;
						break;

					case ShowGridLinesOption.Hide:
						showingGrid = false;
						break;

					case ShowGridLinesOption.AutoHide:
						this.Zoom = zoom;		// force recalculation
						break;
				}

				LayoutController.LayoutModified();
				Invalidate();
			}

			get {
				return showGrid;
			}
		}

		/// <summary>
		/// The model area that this view shows. 
		/// </summary>
		/// <remarks>
		/// This property must be set as early as possible (before the first call to OnPain)
		/// </remarks>
		public LayoutModelArea Area {
			set {
				area  = value;

				if(area != null) {
					area.AreaChanged += new EventHandler(this.OnAreaChanged);
					area.EraseComponentImage += new EventHandler(this.OnEraseComponentImage);
					area.AreaBoundsChanged += new EventHandler(this.OnAreaBoundsChanged);

					setScrollBars();
				}

				EventManager.Instance.OptimizeForFilteringBySenderType("get-model-component-drawing-regions");
			}

			get {
				return area;
			}
		}

		/// <summary>
		/// return the size of layout components
		/// </summary>
		public Size GridSizeInModelCoordinates {
			get {
				return componentSize;
			}
		}

		/// <summary>
		/// Width in world coordinates of a "line" (for example selection rectangle line)
		/// </summary>
		public float LineWidthInModelCoordinates {
			get {
				return mpGridLineSize.Width;
			}
		}


		/// <summary>
		/// Given a point in view window client coordinates, check what is "hit" by this point.
		/// </summary>
		/// <param name="dcLocation">Point in view client coordinates</param>
		/// <returns>A LayoutHitTestObject describing the hit test results <see cref="LayoutHitTestResult"/></returns>
		public LayoutHitTestResult HitTest(LayoutPhase phases, Point dcLocation) {
			var mpLocation = new PointF(dcLocation.X / Zoom, dcLocation.Y / Zoom);
			var regions = new List<ILayoutDrawingRegion>();
			var clickedRegions = new List<ILayoutDrawingRegion>();

			LayoutSelection	selection = new LayoutSelection();

			using(Graphics g = CreateTransformedGraphics()) {
				foreach(LayoutModelSpotComponentCollection spot in Area.Grid.Values) {
					if((spot.Phase & phases) != 0) {
						foreach(ModelComponent component in spot) {
							regions.Clear();

							EventManager.Event(new LayoutGetDrawingRegionsEvent(component, this, DetailLevel, g, regions));

							foreach(ILayoutDrawingRegion region in regions) {
								if(region.BoundingRegionInModelCoordinates.GetBounds(g).Contains(mpLocation) && region.CanBeClicked) {
									clickedRegions.Add(region);
									selection.Add(component);
								}

								IDisposable disposableRegion = region as IDisposable;
								if(disposableRegion != null)
									disposableRegion.Dispose();
							}
						}
					}
				}
			}

			return new LayoutHitTestResult(Area, this, dcLocation, MapFromDeviceCoordinatesToModelLocationCoordinates(dcLocation), selection, clickedRegions);
		}

		public LayoutHitTestResult HitTest(Point dcLocation) {
			return HitTest(LayoutModel.ActivePhases, dcLocation);
		}

		public void ShowAllArea() {
			SizeF		neededSize = new SizeF(
				(area.Bounds.Width+2) * (GridSizeInModelCoordinates.Width + GridLineWidthInModelCoordinates),
				(area.Bounds.Height+2) * (GridSizeInModelCoordinates.Height + GridLineWidthInModelCoordinates));

			this.OriginInModelGridUnits = new Point(area.Bounds.Left - 1, area.Bounds.Top - 1);
			double zoomValue = Math.Min((ClientSize.Width - vScrollBar.Width) / neededSize.Width, (ClientSize.Height - hScrollBar.Height) / neededSize.Height);

			zoomValue = Math.Floor(zoomValue* componentSize.Height) / componentSize.Height;
			this.Zoom = (float)zoomValue;
		}

		// Ensure that a given point on the model is visible
		public void EnsureVisible(Point mlAbs) {
			Point mlOrigin = modelGridToModelCoordinatesStack.Peek().OriginInModelGridUnits;
			Point	ml = new Point(mlAbs.X - mlOrigin.X, mlAbs.Y - mlOrigin.Y);
			SizeF	mlClientSize = ClientSizeInModelGridUnits;

			if(ml.X >= 0 && ml.X < mlClientSize.Width && ml.Y >= 0 && ml.Y < mlClientSize.Height)
				return;				// Already visible, nothing to do

			// Set the origin so the required point will be at the middle of the screen
			Point	origin = new Point((int)(mlAbs.X - mlClientSize.Width / 2), (int)(mlAbs.Y - mlClientSize.Height / 2));

			if(origin.X < area.Bounds.Left)
				origin.X = area.Bounds.Left;

			if(origin.Y < area.Bounds.Top)
				origin.Y = area.Bounds.Top;

			this.OriginInModelGridUnits = origin;
		}

		#endregion

		#region Track Color services

		public Color GetTrackEdgeColor(TrackEdge edge) {
			Color			resultColor = Color.Black;

			if(LayoutController.IsOperationMode) {
				LayoutBlock		block = edge.Track.GetBlock(edge.ConnectionPoint);

				if(block == null) {
					resultColor = Color.LightGray;
				}
				else {
					if(block.IsLocked) {
						switch(block.LockRequest.Type) {
							case LayoutLockType.ManualDispatch: {
									ILayoutPower power = edge.Track.GetPower(edge.ConnectionPoint);

                                    if (power != null) {
                                        switch (power.Type) {
                                            default:
                                            case LayoutPowerType.Digital: resultColor = Color.DarkCyan; break;
                                            case LayoutPowerType.Disconnected: resultColor = Color.FromArgb(0x20, 0x4b, 0x4b); break;
                                            case LayoutPowerType.Programmer: resultColor = Color.Red; break;
                                        }
                                    }
                                    else
                                        resultColor = Color.FromArgb(0x20, 0x4b, 0x4b);
								}
								break;

							case LayoutLockType.Train:
								resultColor = Color.MediumSeaGreen;
								break;

							case LayoutLockType.Programming:
								resultColor = Color.Red;
								break;
						}
					}
					else {
						ILayoutPower power = edge.Track.GetPower(edge.ConnectionPoint);

						if(power != null) {
							if(power.Type == LayoutPowerType.Disconnected)
								resultColor = Color.DarkGray;
							else if(power.Type == LayoutPowerType.Programmer)
								resultColor = Color.LightPink;
						}
						else
							resultColor = Color.LightGray;
					}
				}
			}

			return resultColor;
		}

		public TrackColors GetTrackSegmentColor(TrackSegment trackSegment) {
			TrackColors					result;
			TrackSegmentPreviewResult	previewResult = LayoutController.PreviewRouteManager[trackSegment];

			if(previewResult != null)
				result = new TrackColors(trackSegment, previewResult.Request.Color, previewResult.Annotations);
			else
				result = new TrackColors(trackSegment, 
					GetTrackEdgeColor(new TrackEdge(trackSegment.Track, trackSegment.Cp1)), GetTrackEdgeColor(new TrackEdge(trackSegment.Track, trackSegment.Cp2)));

			return result;
		}

		#endregion

		#region Coordinate transformation and mapping

#if DEBUG
		/// <summary>
		/// Debug a rectangle on the debug output
		/// </summary>
		/// <param name="desc">A description of the dumped rectangle</param>
		/// <param name="r">the rectangle to dump</param>
		void dumpRect(String desc, Rectangle r) {
			Debug.WriteLine(String.Format("{0}: (X={1:g},Y={2:g}-Width={3:g},Height={4:g})", desc,
				r.Left, r.Top, r.Width, r.Height));
		}

		void dumpRect(String desc, RectangleF r) {
			Debug.WriteLine(String.Format("{0}: (X={1:g},Y={2:g}-Width={3:g},Height={4:g})", desc,
				r.Left, r.Top, r.Width, r.Height));
		}

#endif

		public float GridLineWidthInModelCoordinates {
			get {
				switch(showGrid) {
					case ShowGridLinesOption.Hide:
						return 0;

					case ShowGridLinesOption.Show:
						return mpGridLineSize.Width;

					case ShowGridLinesOption.AutoHide:
						return showingGrid ? mpGridLineSize.Width : 0;

					default:
						throw new NotImplementedException("Invalid ShowGrid value");
				}
			}
		}

		int DcGridLineWidth {
			get {
				switch(showGrid) {
					case ShowGridLinesOption.Hide:
						return 0;

					case ShowGridLinesOption.Show:
						return dcGridLineSize.Width;

					case ShowGridLinesOption.AutoHide:
						return showingGrid ? dcGridLineSize.Width : 0;

					default:
						throw new NotImplementedException("Invalid ShowGrid value");
				}
			}
		}

		/// <summary>
		/// Return the client rectangle in model points
		/// </summary>
		public RectangleF ClientRectangleInModelCoordinates {
			get {
				return new RectangleF(0, 0, ClientRectangle.Right / zoom, ClientRectangle.Bottom / zoom);
			}
		}

		public RectangleF DrawingRectangleInModelCoordinates {
			get {
				return modelGridToModelCoordinatesStack.Peek().ClientRectangleInModelCoordinates;
			}
		}

		/// <summary>
		/// Return the client window size in model grid units
		/// </summary>
		public SizeF ClientSizeInModelGridUnits {
			get {
				return new SizeF((ClientSize.Width - vScrollBar.Width) / (dcComponentSize.Width + DcGridLineWidth),
					(ClientSize.Height - hScrollBar.Height) / (dcComponentSize.Height + DcGridLineWidth));
			}
		}

		/// <summary>
		/// Get the top left coordinate in model points of a given location in the model
		/// </summary>
		/// <param name="mlAbs">Model location</param>
		/// <returns>Top left coordinate in world coordinates</returns>
		public PointF ModelLocationInModelCoordinates(Point mlAbs) {
			Point mlOrigin = modelGridToModelCoordinatesStack.Peek().OriginInModelGridUnits;
			Point ml = new Point(mlAbs.X - mlOrigin.X, mlAbs.Y - mlOrigin.Y);

			return new PointF(GridLineWidthInModelCoordinates * (1 + ml.X) + GridSizeInModelCoordinates.Width * ml.X,
				GridLineWidthInModelCoordinates * (1 + ml.Y) + GridSizeInModelCoordinates.Height * ml.Y);
		}

		/// <summary>
		/// Get the bounding rectangle of a given model grid location
		/// </summary>
		/// <param name="ml">The model location</param>
		/// <returns>The grid location in model points</returns>
		public RectangleF ModelLocationRectangleInModelCoordinates(Point ml) {
			return new RectangleF(ModelLocationInModelCoordinates(ml), GridSizeInModelCoordinates);
		}

		/// <summary>
		/// Return a given the bounding rectangle of a given model component (specified
		/// by location) in client coordinates (pixels relative to the upper left corner of the view)
		/// </summary>
		/// <param name="modelLocation">Component location in the model</param>
		/// <returns>Component bouding rectangle in client coordinates</returns>
		protected Rectangle GetModelLocationInDeviceCoordinates(Point ml) {
			Point	mlRelativeToOrigin = ml - (Size)mlOrigin;
			
			return new Rectangle(
				new Point((int)(mlRelativeToOrigin.X * dcComponentSize.Width + (mlRelativeToOrigin.X + 1) * DcGridLineWidth),
				(int)(mlRelativeToOrigin.Y * dcComponentSize.Height + (mlRelativeToOrigin.Y  + 1) * DcGridLineWidth)), 
				Size.Ceiling(dcComponentSize));
		}

		/// <summary>
		/// Map a rectangle from world cooridnates to page coordinates
		/// </summary>
		Rectangle dcMapRectangle(Graphics g, RectangleF wcRect) {
			PointF[]	wc = new PointF[2];

			wc[0] = wcRect.Location;
			wc[1] = wcRect.Size.ToPointF();

			g.TransformPoints(CoordinateSpace.Page, CoordinateSpace.World, wc);

			return new Rectangle(new Point((int)wc[0].X, (int)wc[0].Y), Size.Ceiling(new SizeF(wc[1])));
		}

		/// <summary>
		/// Map an area in the device to the rectangle in model coordinates that is displayed in this area
		/// </summary>
		/// <param name="dcRect">The area (rectangle) in device coordinates</param>
		/// <returns>The area in model coordinates</returns>
		Rectangle MapRectangleFromDeviceCoordinatesToModelGridUnits(Rectangle dcRect) {
			Point	mlTopLeft = MapFromDeviceCoordinatesToModelLocationCoordinates(dcRect.Location);
			Point	mlBottomRight = MapFromDeviceCoordinatesToModelLocationCoordinates(new Point(dcRect.Right, dcRect.Bottom));

			return Rectangle.FromLTRB(mlTopLeft.X, mlTopLeft.Y, mlBottomRight.X, mlBottomRight.Y);
		}

		/// <summary>
		/// Given a point in client coordinate (device units relative to client upper-left point)
		/// return the model location that contains this point.
		/// </summary>
		/// <param name="dcClient">Point in client coordinates</param>
		/// <returns>The model location that contains this point</returns>
		public Point MapFromDeviceCoordinatesToModelLocationCoordinates(Point dcClient) {
			return new Point(
				(int)(dcClient.X / (dcComponentSize.Width + DcGridLineWidth)),
				(int)(dcClient.Y / (dcComponentSize.Height + DcGridLineWidth))) +
				(Size)mlOrigin;
		}

		#endregion

		#region Handle Write/Read data in XML

		public void WriteXml(XmlWriter w) {
			w.WriteStartElement("LayoutView");

			if(DefaultZoom == 0.0)
				SetAsDefaultOriginAndZoom();

			w.WriteStartElement("Origin");
			w.WriteAttributeString("x", XmlConvert.ToString(DefaultOriginInModelGridUnits.X));
			w.WriteAttributeString("y", XmlConvert.ToString(DefaultOriginInModelGridUnits.Y));
			w.WriteEndElement();

			w.WriteStartElement("Zoom");
			w.WriteAttributeString("factor", XmlConvert.ToString(DefaultZoom));
			w.WriteEndElement();

			w.WriteStartElement("Grid");
			w.WriteAttributeString("visible", ShowGrid.ToString());
			w.WriteAttributeString("color", GridLineColor.Name);
			// Uncomment the following line if decide to make show coordinates a presistant property (read is already in...)
			//w.WriteAttributeString("ShowCoordinates", XmlConvert.ToString(ShowCoordinates));
			w.WriteEndElement();

			w.WriteEndElement();
		}

		public void ReadXml(XmlReader r) {
			if(r.IsEmptyElement)
				r.Read();
			else {
				r.Read();		// <LayoutView>

				while(r.NodeType == XmlNodeType.Element) {
					if(r.Name == "Origin") {
						this.DefaultOriginInModelGridUnits = new Point(XmlConvert.ToInt32(r.GetAttribute("x")), XmlConvert.ToInt32(r.GetAttribute("y")));
						r.Skip();
					}
					else if(r.Name == "Zoom") {
						this.DefaultZoom = XmlConvert.ToSingle(r.GetAttribute("factor"));
						r.Skip();
					}
					else if(r.Name == "Grid") {
						this.GridLineColor = Color.FromName(r.GetAttribute("color"));
						this.ShowGrid = (ShowGridLinesOption)Enum.Parse(typeof(ShowGridLinesOption), r.GetAttribute("visible"));
						
						if(r.GetAttribute("ShowCoordinates") != null)
							this.ShowCoordinates = XmlConvert.ToBoolean(r.GetAttribute("ShowCoordinates"));

						r.Skip();
					}
					else
						r.Skip();
				}

				SetOriginAndZoomToDefault();
				
				if(showCoordinatesSwitch.Enabled)
					showCoordinates = true;

				r.Read();		// </LayoutView>
			}
		}

		#endregion

		#region Handle drawing

		private float getValue(float offset, int i, float slope) {
			return offset + i*slope;
		}


		private void drawGrid(Graphics g, RectangleF boundingRect, float gridLineWidthInModelCoordinates) {
			using(Pen penGrid = new Pen(gridLineColor, gridLineWidthInModelCoordinates)) {
				float f;

				f = boundingRect.Left;

				for(int i = 0; f < boundingRect.Right; i++) {
					f = getValue(boundingRect.Left, i, gridLineWidthInModelCoordinates + GridSizeInModelCoordinates.Width);
					g.DrawLine(penGrid, f, boundingRect.Top, f, boundingRect.Bottom);
				}

				f = boundingRect.Top;

				for(int i = 0; f < boundingRect.Bottom; i++) {
					f = getValue(boundingRect.Top, i, gridLineWidthInModelCoordinates + GridSizeInModelCoordinates.Height);
					g.DrawLine(penGrid, boundingRect.Left, f, boundingRect.Right, f);
				}
			}
		}

		private void drawCoordinates(Graphics g, RectangleF boundingRectInModelCoordinates, Point originInModelGridUnits, float gridLineWidthInModelCoordinates) {
			float		fy;

			fy = boundingRectInModelCoordinates.Top;

			using(Font font = new Font("Arial", 9, GraphicsUnit.World)) {
				for(int iy = 0; fy < boundingRectInModelCoordinates.Bottom; iy++) {
					fy = getValue(boundingRectInModelCoordinates.Top, iy, gridLineWidthInModelCoordinates + GridSizeInModelCoordinates.Height);

					float fx = boundingRectInModelCoordinates.Left;

					for(int ix = 0; fx < boundingRectInModelCoordinates.Right; ix++) {
						fx = getValue(boundingRectInModelCoordinates.Left, ix, gridLineWidthInModelCoordinates + GridSizeInModelCoordinates.Width);
						
						int		x = originInModelGridUnits.X + ix;
						int		y = originInModelGridUnits.Y + iy;

						g.DrawString(x.ToString() + ", " + y.ToString(), font, Brushes.Gray, new PointF(fx+3, fy+3));
					}
				}
			}
		}

		void drawBackground(Graphics g, RectangleF clipBounds) {
			g.FillRectangle(Brushes.White, clipBounds);
		}

		/// <summary>
		/// Check if a component is in a dispayed selection
		/// </summary>
		/// <param name="component">The component</param>
		/// <returns>A non null SelectionLook is the component is selected in a visible selection</returns>
		private ILayoutSelectionLook getSelectionLook(ModelComponent component) {
			ILayoutSelectionLook	selectionLook = null;

			foreach(LayoutSelection selection in LayoutController.SelectionManager.DisplayedSelections) {
				selectionLook = selection.GetComponentSelectionLook(component);
				if(selectionLook != null)
					break;
			}

			return selectionLook;
		}

		public enum VisibleResult {
			No,
			Partial,
			Yes,
		};

		/// <summary>
		/// Check if a component is visible in the current view
		/// </summary>
		/// <param name="component">The component</param>
		/// <returns></returns>
		public VisibleResult IsComponentVisible(ModelComponent component) {
			VisibleResult	result = VisibleResult.No;

			if(component.Spot.Area != Area)
				return VisibleResult.No;

			using(Graphics g = CreateTransformedGraphics()) {
				List<ILayoutDrawingRegion> regions = new List<ILayoutDrawingRegion>();

				EventManager.Event(new LayoutGetDrawingRegionsEvent(component, this, DetailLevel, g, regions));

				int				nVisibleRegions = 0;

				foreach(ILayoutDrawingRegion region in regions) {
					if(g.IsVisible(region.BoundingRegionInModelCoordinates.GetBounds(g))) {
						nVisibleRegions++;
					}

					IDisposable disposableRegion = region as IDisposable;
					if(disposableRegion != null)
						disposableRegion.Dispose();
				}

				if(nVisibleRegions == 0)
					result = VisibleResult.No;
				else if(nVisibleRegions == regions.Count)
					result = VisibleResult.Yes;
				else
					result = VisibleResult.Partial;
			}

			return result;
		}

		/// <summary>
		/// Internal structure that represent an entry in the drawing list. The entry
		/// contain the region to be drawn and a selection look for this region. If
		/// the selection look is null, then the drawn component was not selected (or
		/// that this selection is not displayed)
		/// </summary>
		internal struct DrawListEntry {
			internal ILayoutDrawingRegion	region;
			internal ILayoutSelectionLook	selectionLook;

			internal DrawListEntry(ILayoutDrawingRegion region, ILayoutSelectionLook selectionLook) {
				this.region = region;
				this.selectionLook = selectionLook;
			}
		}

		class SpotBackgroundRegion : ILayoutDrawingRegion {
			LayoutModelSpotComponentCollection spot;
			ILayoutView view;
			Region _boundingRegion = null;

			public SpotBackgroundRegion(ILayoutView view, LayoutModelSpotComponentCollection spot) {
				this.view = view;
				this.spot = spot;
			}

			#region ILayoutDrawingRegion Members

			public Region BoundingRegionInModelCoordinates {
				get {
					if(_boundingRegion == null)
						_boundingRegion = new Region(view.ModelLocationRectangleInModelCoordinates(spot.Location));
					return _boundingRegion;
				}
			}

			public int ZOrder {
				get { return 0; }
			}

			public void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {
				switch(spot.Phase) {
					case LayoutPhase.Planned: {
							using(var b = new SolidBrush(Color.FromArgb(170, 0xa0, 0xa0, 0xa0))) {
								g.FillRectangle(b, new Rectangle(new Point(0, 0), view.GridSizeInModelCoordinates));
							}
						}
						break;

					case LayoutPhase.Construction: {
							using(var b = new SolidBrush(Color.FromArgb(170, 0xf0, 0xf0, 0xd0))) {
								g.FillRectangle(b, new Rectangle(new Point(0, 0), view.GridSizeInModelCoordinates));
							}
						}
						break;
				}
			}

			public Func<bool> ClickHandler { get; set; }

			public Func<bool> RightClickHandler { get; set; }

			public bool CanBeClicked {
				get { return false; }
			}

			#endregion
		}

		private void addSpotDrawingRegions(Graphics g, ViewDetailLevel detailLevel, List<DrawListEntry> drawList, LayoutModelSpotComponentCollection spot) {
			List<ILayoutDrawingRegion>	regions = new List<ILayoutDrawingRegion>(10);

			if(spot.Phase != LayoutPhase.Operational)
				drawList.Add(new DrawListEntry(new SpotBackgroundRegion(this, spot), null));

			foreach(ModelComponent component in spot) {
				if(component != erasedComponent) {
					ILayoutSelectionLook	selectionLook = null;
					bool					selectionChecked = false;

					regions.Clear();
					EventManager.Event(new LayoutGetDrawingRegionsEvent(component, this, detailLevel, g, regions));

					foreach(ILayoutDrawingRegion region in regions) {
						if(g.IsVisible(region.BoundingRegionInModelCoordinates.GetBounds(g))) {
							if(!selectionChecked) {
								selectionChecked = true;
								selectionLook = getSelectionLook(component);
							}

							drawList.Add(new DrawListEntry(region, selectionLook));
						}
						else {
							IDisposable disposableRegion = region as IDisposable;

							if(disposableRegion != null)
								disposableRegion.Dispose();
						}
					}
				}
			}
		}

		private void drawComponents(Graphics g, ViewDetailLevel detailLevel, Rectangle modelAreaInModelGridUnits) {
			List<DrawListEntry>	drawList = new List<DrawListEntry>(Area.Grid.Count);

			int		drawLevel = ++LayoutModel.Instance.DrawLevel;

			foreach(SortedVector<LayoutModelSpotComponentCollection> sortedRow in Area.SortedRows.RangeValues(modelAreaInModelGridUnits.Top, modelAreaInModelGridUnits.Bottom + 1)) {
				foreach(LayoutModelSpotComponentCollection spot in sortedRow.RangeValues(modelAreaInModelGridUnits.Left, modelAreaInModelGridUnits.Right + 1)) {
					if((spot.Phase & Phases) != 0) {
						addSpotDrawingRegions(g, detailLevel, drawList, spot);
						spot.DrawLevel = drawLevel;
					}
				}
			}

			foreach(LayoutModelSpotComponentCollection spot in Area.OutOfGridSpots) {
				if(spot.DrawLevel != drawLevel) {
					if((spot.Phase & Phases) != 0) {
						addSpotDrawingRegions(g, detailLevel, drawList, spot);
						spot.DrawLevel = drawLevel;
					}
				}
			}

			drawList.Sort(delegate(DrawListEntry d1, DrawListEntry d2) { return d1.region.ZOrder - d2.region.ZOrder; });

			foreach(DrawListEntry listEntry in drawList) {
				GraphicsState	gs = g.Save();
				PointF			regionOrigin = listEntry.region.BoundingRegionInModelCoordinates.GetBounds(g).Location;

				g.SetClip(listEntry.region.BoundingRegionInModelCoordinates, CombineMode.Intersect);
				g.TranslateTransform(regionOrigin.X, regionOrigin.Y);
				listEntry.region.Draw(this, detailLevel, listEntry.selectionLook, g);

				IDisposable disposableRegion = listEntry.region as IDisposable;
				if(disposableRegion != null)
					disposableRegion.Dispose();

				g.Restore(gs);
			}
		}

		Bitmap allocateOffScreenBuffer(Graphics g, RectangleF clipBounds) {
			return (Bitmap)EventManager.Event(new LayoutEvent(g, "allocate-offscreen-buffer", null, clipBounds));
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);

			Graphics	g;
			RectangleF	clipBounds = e.ClipRectangle;

			if(clipBounds.IsEmpty)
				return;

			Bitmap		buffer = allocateOffScreenBuffer(e.Graphics, clipBounds);

			using(g = Graphics.FromImage(buffer)) {

				// Translate so the top-left of the clip region is on the top/left (0,0)
				// of the bitmap
				g.TranslateTransform(-clipBounds.Left, -clipBounds.Top);
				drawBackground(g, clipBounds);

				g.ScaleTransform(zoom, zoom);

				if(showingGrid) {
					drawGrid(g, ClientRectangleInModelCoordinates, mpGridLineSize.Width);
					if(ShowCoordinates)
						drawCoordinates(g, ClientRectangleInModelCoordinates, mlOrigin, mpGridLineSize.Width);
				}

				if(area != null)
					drawComponents(g, DetailLevel, MapRectangleFromDeviceCoordinatesToModelGridUnits(Rectangle.Truncate(clipBounds)));
			}

			// After the background image is created, draw it on the screen
			e.Graphics.DrawImage(buffer, clipBounds.Location);
		}

		private class MapFromGridUnitsToModelLocation : ILayoutViewModelGridToModelCoordinatesSettings {
			Point originInGridUnits;
			float lineWidthInModelCoordinates;
			RectangleF drawingAreaInModelCoordinates;

			public MapFromGridUnitsToModelLocation(Point originInGridUnits, RectangleF drawingAreaInModelCoordinates, float lineWidthInModelCoordinates) {
				this.originInGridUnits = originInGridUnits;
				this.drawingAreaInModelCoordinates = drawingAreaInModelCoordinates;
				this.lineWidthInModelCoordinates = lineWidthInModelCoordinates;
			}

			#region ILayoutViewModelGridToModelCoordinatesSettings Members
			public Point OriginInModelGridUnits {
				get { return originInGridUnits; }
			}

			public RectangleF ClientRectangleInModelCoordinates {
				get { return drawingAreaInModelCoordinates; }
			}

			public float LineWidthInModelCoordinates {
				get { return lineWidthInModelCoordinates; }
			}

			#endregion
		}

		public void Draw(Graphics g, Rectangle drawingRectangleInDeviceCoordinates, Rectangle modelAreaInGridUnits, int gridLineWidthInDeviceCoordinates, bool showCoordinates) {
			drawBackground(g, drawingRectangleInDeviceCoordinates);

			SizeF neededSize = new SizeF(
				modelAreaInGridUnits.Width * (GridSizeInModelCoordinates.Width + gridLineWidthInDeviceCoordinates),
				modelAreaInGridUnits.Height * (GridSizeInModelCoordinates.Height + gridLineWidthInDeviceCoordinates));


			double zoomX = drawingRectangleInDeviceCoordinates.Width / neededSize.Width;
			double zoomY = drawingRectangleInDeviceCoordinates.Height / neededSize.Height;
			float zoomValue = (float)(Math.Round(Math.Min(zoomX, zoomY) * componentSize.Height) / componentSize.Height);
			float gridLineWidthInModelCoordinates = 0;

			if(gridLineWidthInDeviceCoordinates > 0) {
				GraphicsState save = g.Save();

				PointF[] pt = new PointF[1];

				pt[0].X = gridLineWidthInDeviceCoordinates;
				pt[0].Y = gridLineWidthInDeviceCoordinates;

				g.ScaleTransform(zoomValue, zoomValue);
				g.TransformPoints(CoordinateSpace.World, CoordinateSpace.Device, pt);
				gridLineWidthInModelCoordinates = pt[0].X;

				g.Restore(save);
			}

			GraphicsState gs = g.Save();

			g.SetClip(drawingRectangleInDeviceCoordinates);
			g.TranslateTransform(drawingRectangleInDeviceCoordinates.Left, drawingRectangleInDeviceCoordinates.Top);
			g.ScaleTransform(zoomValue, zoomValue);

			if(gridLineWidthInDeviceCoordinates > 0) {
				RectangleF boundingRectInModelCoordinates = new RectangleF(0, 0, drawingRectangleInDeviceCoordinates.Width / zoomValue, drawingRectangleInDeviceCoordinates.Height / zoomValue);

				drawGrid(g, boundingRectInModelCoordinates, gridLineWidthInModelCoordinates);

				if(showCoordinates)
					drawCoordinates(g, boundingRectInModelCoordinates, modelAreaInGridUnits.Location, gridLineWidthInModelCoordinates);
			}

			modelGridToModelCoordinatesStack.Push(new MapFromGridUnitsToModelLocation(modelAreaInGridUnits.Location, new RectangleF(0, 0, modelAreaInGridUnits.Width * GridSizeInModelCoordinates.Width, modelAreaInGridUnits.Height * GridSizeInModelCoordinates.Height), gridLineWidthInModelCoordinates));
			drawComponents(g, ViewDetailLevel.High, modelAreaInGridUnits);
			modelGridToModelCoordinatesStack.Pop();

			g.Restore(gs);
		}

		protected override void OnPaintBackground(PaintEventArgs e) {
			// Prevent background erase
		}

		#endregion

		#region Handle scroll bars

		void setScrollBars() {
			Size		mlClientSize = Size.Round(ClientSizeInModelGridUnits);
			Rectangle	mlBounds = Area.Bounds;
			Size		scrollBarSize = new Size(Math.Max(mlClientSize.Width, mlBounds.Width), 
				Math.Max(mlClientSize.Height, mlBounds.Height));

			hScrollBar.Minimum = mlBounds.Left;
			hScrollBar.Maximum = mlBounds.Left + scrollBarSize.Width;
			vScrollBar.Minimum = mlBounds.Top;
			vScrollBar.Maximum = mlBounds.Top + scrollBarSize.Height;

			if(mlClientSize.Width > 0)
				hScrollBar.LargeChange = mlClientSize.Width - 1;
			if(mlClientSize.Height > 0)
				vScrollBar.LargeChange = mlClientSize.Height - 1;
		}

		void updateScrollBarPosition() {
			if(OriginInModelGridUnits.X != mlScrolledOrigin.X) {
				if(OriginInModelGridUnits.X < hScrollBar.Minimum)
					hScrollBar.Value = hScrollBar.Minimum;
				else if(OriginInModelGridUnits.X > hScrollBar.Maximum)
					hScrollBar.Value = hScrollBar.Maximum;
				else
					hScrollBar.Value = OriginInModelGridUnits.X;
				mlScrolledOrigin.X = OriginInModelGridUnits.X;
			}

			if(OriginInModelGridUnits.Y != mlScrolledOrigin.Y) {
				if(OriginInModelGridUnits.Y < vScrollBar.Minimum)
					vScrollBar.Value = vScrollBar.Minimum;
				else if(OriginInModelGridUnits.Y > vScrollBar.Maximum)
					vScrollBar.Value = vScrollBar.Maximum;
				else
					vScrollBar.Value = OriginInModelGridUnits.Y;
				mlScrolledOrigin.Y = OriginInModelGridUnits.Y;
			}
		}

		public void DoVerticalScroll(ScrollEventType type) {
			vScrollBar_Scroll(null, new ScrollEventArgs(type, 0));
		}

		public void DoHorizontalScroll(ScrollEventType type) {
			hScrollBar_Scroll(null, new ScrollEventArgs(type, 0));
		}

		#endregion

		#region Handle drag-drop

		ScrollDirection NeedToScroll(Point p) {
			Size scrollRegionSize = new Size(16, 16);
			ScrollDirection result;

			if(new Rectangle(0, 0, scrollRegionSize.Width, ClientRectangle.Height).Contains(p))
				result = ScrollDirection.Left;
			else if(new Rectangle(0, 0, ClientRectangle.Width, scrollRegionSize.Height).Contains(p))
				result = ScrollDirection.Up;
			else if(new Rectangle(ClientRectangle.Width - scrollRegionSize.Width, 0, scrollRegionSize.Width, ClientRectangle.Height).Contains(p))
				result = ScrollDirection.Right;
			else if(new Rectangle(0, ClientRectangle.Height - scrollRegionSize.Height, ClientRectangle.Width, scrollRegionSize.Height).Contains(p))
				result = ScrollDirection.Down;
			else
				result = ScrollDirection.None;

			return result;
		}

		protected override void OnDragOver(DragEventArgs drgevent) {
			base.OnDragOver(drgevent);

			Point dragPoint = this.PointToClient(new Point(drgevent.X, drgevent.Y));

			dropScrollDirection = NeedToScroll(dragPoint);

			if(dropScrollDirection != ScrollDirection.None) {
				drgevent.Effect |= DragDropEffects.Scroll;

				timerScrollForDrop.Enabled = true;
			}
			else {
				timerScrollForDrop.Enabled = false;

				if(ModelComponentQueryDrop != null)
					ModelComponentQueryDrop(this, new LayoutViewEventArgs(HitTest(dragPoint), drgevent));
			}
		}

		protected override void OnDragDrop(DragEventArgs drgevent) {
			base.OnDragDrop(drgevent);

			Point dragPoint = this.PointToClient(new Point(drgevent.X, drgevent.Y));

			if(ModelComponentDrop != null)
				ModelComponentDrop(this, new LayoutViewEventArgs(HitTest(dragPoint), drgevent));
		}

		#endregion

		#region Event handlers

		void invalidateComponentDrawingRegion(ILayoutDrawingRegion region) {
			Matrix	m = new Matrix();

			using(System.Drawing.Region	r = region.BoundingRegionInModelCoordinates.Clone()) {
				m.Scale(zoom, zoom);
				r.Transform(m);
				Invalidate(r);
			}
		}

		void invalidateComponent(ModelComponent component) {
			using(Graphics g = CreateTransformedGraphics()) {
				List<ILayoutDrawingRegion> regions = new List<ILayoutDrawingRegion>();

				EventManager.Event(new LayoutGetDrawingRegionsEvent(component, this, DetailLevel, g, regions));

				foreach(ILayoutDrawingRegion region in regions) {
					invalidateComponentDrawingRegion(region);

					IDisposable disposableRegion = region as IDisposable;
					if(disposableRegion != null)
						disposableRegion.Dispose();
				}
			}
		}

		private void OnAreaChanged(Object sender, EventArgs e) {
			if(sender is LayoutModelArea)
				Invalidate();
			else {
				LayoutModelSpotComponentCollection spot = sender as LayoutModelSpotComponentCollection;

				if(spot != null) {
					System.Drawing.Region spotRegion = new Region(ModelLocationRectangleInModelCoordinates(spot.Location));
					Matrix m = new Matrix();

					m.Scale(Zoom, Zoom);
					spotRegion.Transform(m);
					Invalidate(spotRegion);
				}
				else {
					ModelComponent component = sender as ModelComponent;

					if(component != null)
						invalidateComponent(component);
					else
						Debug.Assert(false, "Unknown sender in OnModelChanged");
				}
			}
		}

		/// <summary>
		/// This event is triggerd when a model component would like to erase its image from the
		/// view. This is needed if a component draw regions outside its grid location. This ensure
		/// that no residuals are left from a deleted or modified component.
		/// </summary>
		/// <param name="sender">The model component to be erased</param>
		/// <param name="e"></param>
		private void OnEraseComponentImage(Object sender, EventArgs e) {
			ModelComponent	c = (ModelComponent)sender;

			erasedComponent = c;		// ensure that this component is not painted
			OnAreaChanged(c, e);		// Invalidate this component
			//			Update();					// Repaint all invalidated regions (without the component)
			erasedComponent = null;
		}

		/// <summary>
		/// This event happends when the bounding region in which there are components in the model
		/// area changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAreaBoundsChanged(Object sender, EventArgs e) {
			setScrollBars();		// update the scroll bars to reflect the change
		}

		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.vScrollBar = new System.Windows.Forms.VScrollBar();
			this.hScrollBar = new System.Windows.Forms.HScrollBar();
			this.timerScrollForDrop = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// vScrollBar
			// 
			this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
			this.vScrollBar.Location = new System.Drawing.Point(134, 0);
			this.vScrollBar.Name = "vScrollBar";
			this.vScrollBar.Size = new System.Drawing.Size(16, 134);
			this.vScrollBar.TabIndex = 1;
			this.vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar_Scroll);
			// 
			// hScrollBar
			// 
			this.hScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.hScrollBar.Location = new System.Drawing.Point(0, 134);
			this.hScrollBar.Name = "hScrollBar";
			this.hScrollBar.Size = new System.Drawing.Size(150, 16);
			this.hScrollBar.TabIndex = 0;
			this.hScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar_Scroll);
			// 
			// timerScrollForDrop
			// 
			this.timerScrollForDrop.Interval = 250;
			this.timerScrollForDrop.Tick += new System.EventHandler(this.timerScrollForDrop_Tick);
			// 
			// LayoutView
			// 
			this.AllowDrop = true;
			this.Controls.Add(this.vScrollBar);
			this.Controls.Add(this.hScrollBar);
			this.Name = "LayoutView";
			this.Resize += new System.EventHandler(this.LayoutView_Resize);
			this.MouseEnter += new System.EventHandler(this.LayoutView_MouseEnter);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LayoutView_KeyDown);
			this.ResumeLayout(false);

		}
		#endregion

		#region Scroll handling

		#region Low level interop

		[StructLayout(LayoutKind.Sequential)]
		private class RECT {
			public Int32	left;
			public Int32	top;
			public Int32	right;
			public Int32	bottom;

			public RECT(Rectangle r) {
				left = r.Left;
				top = r.Top;
				right = r.Right;
				bottom = r.Bottom;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		struct POINT {
			public Int32	x;
			public Int32	y;
		}

		[DllImport("user32.dll")]
		private static extern int ScrollWindowEx(
			System.IntPtr hWnd,
			int dx,
			int dy,
			[MarshalAs(UnmanagedType.LPStruct)]
			RECT prcScroll,
			[MarshalAs(UnmanagedType.LPStruct)]
			RECT prcClip,
			System.IntPtr hrgnUpdate,
			[MarshalAs(UnmanagedType.LPStruct)]
			RECT prcUpdate,
			ScrollWindowExFlags flags
			);

		[Flags]
		private enum ScrollWindowExFlags : uint {
			SW_SCROLLCHILDREN = 0x0001,
			SW_INVALIDATE = 0x0002,
			SW_ERASE = 0x0004,
			SW_SMOOTHSCROLL = 0x0010,
		};

		[Flags]
		private enum QsFlags {
			QS_KEY              = 0x0001,
			QS_MOUSEMOVE        = 0x0002,
			QS_MOUSEBUTTON      = 0x0004,
			QS_POSTMESSAGE      = 0x0008,
			QS_TIMER            = 0x0010,
			QS_PAINT            = 0x0020,
			QS_SENDMESSAGE      = 0x0040,
			QS_HOTKEY           = 0x0080,
			QS_ALLPOSTMESSAGE   = 0x0100,
			QS_RAWINPUT         = 0x0400,

			QS_MOUSE			= QsFlags.QS_MOUSEMOVE|QsFlags.QS_MOUSEBUTTON,
			QS_INPUT			= QsFlags.QS_MOUSE|QsFlags.QS_KEY|QsFlags.QS_RAWINPUT,
		}


		protected void DoScrollWindow(int deltaX, int deltaY) {
			ScrollWindowEx(Handle, deltaX, deltaY, null, null, IntPtr.Zero, null, ScrollWindowExFlags.SW_INVALIDATE);
		}

		#endregion

		protected void ScrollWindow(Size mlSize) {
			this.mlOrigin = new Point(mlOrigin.X + mlSize.Width, mlOrigin.Y + mlSize.Height);

			DoScrollWindow(-(int)(mlSize.Width * (dcComponentSize.Width + DcGridLineWidth)),
				-(int)(mlSize.Height * (dcComponentSize.Height + DcGridLineWidth)));
			updateScrollBarPosition();

			LayoutController.LayoutModified();
			Update();
		}

		private void vScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e) {
			switch(e.Type) {

				case ScrollEventType.SmallIncrement:
					ScrollWindow(new Size(0, 1));
					break;

				case ScrollEventType.SmallDecrement:
					ScrollWindow(new Size(0, -1));
					break;

				case ScrollEventType.LargeIncrement:
					ScrollWindow(new Size(0, vScrollBar.LargeChange));
					break;

				case ScrollEventType.LargeDecrement:
					ScrollWindow(new Size(0, -vScrollBar.LargeChange));
					break;

				case ScrollEventType.First:
					OriginInModelGridUnits = new Point(OriginInModelGridUnits.X, Area.Bounds.Top);
					break;

				case ScrollEventType.Last:
					OriginInModelGridUnits = new Point(OriginInModelGridUnits.X, Area.Bounds.Bottom - Size.Round(ClientSizeInModelGridUnits).Height);
					break;

				case ScrollEventType.ThumbTrack:
					OriginInModelGridUnits = new Point(OriginInModelGridUnits.X, e.NewValue);
					break;
			}
		}

		private void hScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e) {
			switch(e.Type) {

				case ScrollEventType.SmallIncrement:
					ScrollWindow(new Size(1, 0));
					break;

				case ScrollEventType.SmallDecrement:
					ScrollWindow(new Size(-1, 0));
					break;

				case ScrollEventType.LargeIncrement:
					ScrollWindow(new Size(hScrollBar.LargeChange, 0));
					break;

				case ScrollEventType.LargeDecrement:
					ScrollWindow(new Size(-hScrollBar.LargeChange, 0));
					break;

				case ScrollEventType.First:
					OriginInModelGridUnits = new Point(Area.Bounds.Left, OriginInModelGridUnits.Y);
					break;

				case ScrollEventType.Last:
					OriginInModelGridUnits = new Point(Area.Bounds.Right- Size.Round(ClientSizeInModelGridUnits).Width, OriginInModelGridUnits.Y);
					break;

				case ScrollEventType.ThumbTrack:
					OriginInModelGridUnits = new Point(e.NewValue, OriginInModelGridUnits.Y);
					break;
					
			}

		}

		private void LayoutView_Resize(object sender, System.EventArgs e) {
			setScrollBars();
		}

		private void LayoutView_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			switch(e.KeyCode ) {

				case Keys.Left:
					DoHorizontalScroll((e.Modifiers & Keys.Control) != 0 ? ScrollEventType.LargeDecrement : ScrollEventType.SmallDecrement);
					break;

				case Keys.Right:
					DoHorizontalScroll((e.Modifiers & Keys.Control) != 0 ? ScrollEventType.LargeIncrement : ScrollEventType.SmallIncrement);
					break;

				case Keys.Up:
					DoVerticalScroll(ScrollEventType.SmallDecrement);
					break;

				case Keys.Down:
					DoVerticalScroll(ScrollEventType.SmallIncrement);
					break;

				case Keys.PageUp:
					DoVerticalScroll(ScrollEventType.LargeDecrement);
					break;

				case Keys.PageDown:
					DoVerticalScroll(ScrollEventType.LargeIncrement);
					break;

				case Keys.Home:
					if((e.Modifiers & Keys.Control) != 0)
						OriginInModelGridUnits = Area.Bounds.Location;
					else
						OriginInModelGridUnits = new Point(Area.Bounds.Left, OriginInModelGridUnits.Y);
					break;

				case Keys.End:
					if((e.Modifiers & Keys.Control) != 0)
						OriginInModelGridUnits = new Point(Area.Bounds.Right - Size.Round(ClientSizeInModelGridUnits).Width, 
							Area.Bounds.Bottom - Size.Round(ClientSizeInModelGridUnits).Height);
					else
						OriginInModelGridUnits = new Point(Area.Bounds.Right - Size.Round(ClientSizeInModelGridUnits).Width, OriginInModelGridUnits.Y);
					break;

				case Keys.C:
					if((e.Modifiers & (Keys.Control | Keys.Shift)) == (Keys.Control | Keys.Shift)) {
						if(ShowGrid != ShowGridLinesOption.Hide)
							ShowCoordinates = !ShowCoordinates;
					}
					break;

			}

		}

		protected override void OnMouseClick(MouseEventArgs e) {
			base.OnMouseClick(e);

			if(ModelComponentClick != null)
				ModelComponentClick(this, new LayoutViewEventArgs(HitTest(new Point(e.X, e.Y)), e));
		}

		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);

			draggedHitTestResult = HitTest(new Point(e.X, e.Y));
			Size dragAreaSize = SystemInformation.DragSize;

			dragSourceRectangle = new Rectangle(new Point(e.X - dragAreaSize.Width / 2, e.Y - dragAreaSize.Height / 2), dragAreaSize);

		}

		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			dragSourceRectangle = Rectangle.Empty;
			draggedHitTestResult = null;
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);

			if(dragSourceRectangle != Rectangle.Empty && !dragSourceRectangle.Contains(e.X, e.Y) && ModelComponentDragged != null) {
				ModelComponentDragged(this, new LayoutViewEventArgs(draggedHitTestResult, e));
				draggedHitTestResult = null;
			}
		}

		protected override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);

			if(!Focused)
				return;

			if(e.Delta > 0) {
				int steps = e.Delta / 120;

				if((Control.ModifierKeys & Keys.Shift) != 0)
					ScrollWindow(new Size(-steps, 0));
				else if((Control.ModifierKeys & Keys.Control) != 0) {
					float	zoom = Zoom + steps*0.1F;

					if(zoom > 4)
						zoom = 4F;
					Zoom = zoom;
				}
				else
					ScrollWindow(new Size(0, -steps));
			}
			else {
				int	steps = -e.Delta / 120;

				if((Control.ModifierKeys & Keys.Shift) != 0)
					ScrollWindow(new Size(steps, 0));
				else if((Control.ModifierKeys & Keys.Control) != 0) {
					float	zoom = Zoom - steps*0.1F;

					if(zoom < 0.1)
						zoom = 0.1F;
					Zoom = zoom;
				}
				else
					ScrollWindow(new Size(0, steps));
			}
		}

		protected override bool IsInputKey(Keys keyData) {
			return true;
		}

		private void LayoutView_MouseEnter(object sender, System.EventArgs e) {
			Focus();
		}

		private void timerScrollForDrop_Tick(object sender, EventArgs e) {
			switch(dropScrollDirection) {

				case ScrollDirection.Down:
					DoVerticalScroll(ScrollEventType.SmallIncrement);
					break;

				case ScrollDirection.Up:
					DoVerticalScroll(ScrollEventType.SmallDecrement);
					break;

				case ScrollDirection.Right:
					DoHorizontalScroll(ScrollEventType.SmallIncrement);
					break;

				case ScrollDirection.Left:
					DoHorizontalScroll(ScrollEventType.SmallDecrement);
					break;
			}
		}

		#endregion

	}

	public enum ShowGridLinesOption {
		Show, Hide, AutoHide
	};

	public enum ViewDetailLevel {
		High, Medium, Low
	};

	/// <summary>
	/// Define a view as passed to other objects (for example to components
	/// paint method). This interface allows those objects to obtain details
	/// about the view (for example background color)
	/// </summary>
	public interface ILayoutView {
		/// <summary>
		/// The model area that this view shows
		/// </summary>
		LayoutModelArea Area {
			get;
		}

		/// <summary>
		/// The size of each grid location in model point
		/// </summary>
		Size GridSizeInModelCoordinates {
			get;
		}

		/// <summary>
		/// The detail level of this view. The detail level reduces based on the view's zoom factor
		/// </summary>
		ViewDetailLevel DetailLevel {
			get;
		}

		/// <summary>
		/// Client rectangle in model points
		/// </summary>
		RectangleF DrawingRectangleInModelCoordinates {
			get;
		}

		/// <summary>
		/// Get the top left coordinate in model points of a given location in the area
		/// </summary>
		/// <param name="mlAbs">model location</param>
		/// <returns>Top left coordinate in world coordinates</returns>
		PointF ModelLocationInModelCoordinates(Point mlAbs);

		/// <summary>
		/// Get the bounding rectangle of a given model grid location
		/// </summary>
		/// <param name="ml">The model location</param>
		/// <returns>The grid location in area points</returns>
		RectangleF ModelLocationRectangleInModelCoordinates(Point ml);

		float LineWidthInModelCoordinates {
			get;
		}

		float GridLineWidthInModelCoordinates {
			get;
		}

		/// <summary>
		/// Displayed grid origin 
		/// </summary>
		/// <value></value>
		Point OriginInModelGridUnits {
			get;
		}

		/// <summary>
		/// Displayed grid size
		/// </summary>
		/// <value></value>
		SizeF ClientSizeInModelGridUnits {
			get;
		}

		/// <summary>
		/// Get the color of a given track edge. The color may be a function of various
		/// factors, such as if the track has power or not, if the track's Block is occuipied
		/// etc.
		/// </summary>
		TrackColors GetTrackSegmentColor(TrackSegment edge);

		/// <summary>
		/// Draw view on a given graphic surface. One of the main use of this is for printing
		/// </summary>
		/// <param name="g">Graphic context</param>
		/// <param name="drawingRectangleInDeviceCoordinates">Where to draw</param>
		/// <param name="modelAreaInGridUnits">What to draw</param>
		/// <param name="gridLineWidthInDeviceCoordinates">Width of grid line (0 means, no grid)</param>
		/// <param name="showCoordinates">Should coordinates be shown</param>
		void Draw(Graphics g, Rectangle drawingRectangleInDeviceCoordinates, Rectangle modelAreaInGridUnits, int gridLineWidthInDeviceCoordinates, bool showCoordinates);
	}

	/// <summary>
	/// Define properties for returning values that are used to map between model grid units to model coordinates. Model coordinates
	/// are eventually mapped to device coordinates.
	/// </summary>
	interface ILayoutViewModelGridToModelCoordinatesSettings {
		/// <summary>
		/// The view origin (left, top) point
		/// </summary>
		/// <value></value>
		Point OriginInModelGridUnits { get; }

		/// <summary>
		/// Client rectangle in model points
		/// </summary>
		RectangleF ClientRectangleInModelCoordinates {
			get;
		}

		/// <summary>
		/// Width of a grid line
		/// </summary>
		/// <value></value>
		float LineWidthInModelCoordinates { get; }
	}

	public class TrackColors {
		TrackSegment				trackSegment;
		Color[]						colors;
		IList<RoutePreviewAnnotation>	annotations;

		public TrackColors(TrackSegment trackSegment, Color color1, Color color2) {
			this.trackSegment = trackSegment;
			this.colors = new Color[2];
			colors[0] = color1;
			colors[1] = color2;
		}

		public TrackColors(TrackSegment trackSegment, Color color, IList<RoutePreviewAnnotation> annotations) {
			this.trackSegment = trackSegment;
			this.colors = new Color[2];
			colors[0] = colors[1] = color;
			this.annotations = annotations;
		}

		public Color Color(int i) {
			return colors[i];
		}

		public LayoutComponentConnectionPoint ConnectionPoint(int i) {
			if(i == 0)
				return trackSegment.Cp1;
			else
				return trackSegment.Cp2;
		}

		public IList<RoutePreviewAnnotation> Annotations {
			get {
				return annotations;
			}
		}
	}

	public class LayoutGetDrawingRegionsEvent : LayoutEvent {
		ILayoutView		view;
		ViewDetailLevel detailLevel;
		Graphics		g;
		IList<ILayoutDrawingRegion> regions;

		public LayoutGetDrawingRegionsEvent(ModelComponent component, ILayoutView view, ViewDetailLevel detailLevel, Graphics g, IList<ILayoutDrawingRegion> drawingRegions) : 
			base(component, "get-model-component-drawing-regions", null, drawingRegions) {
			this.view = view;
			this.detailLevel = detailLevel;
			this.g = g;
			this.regions = drawingRegions;
		}

		public ILayoutView View {
			get {
				return view;
			}
		}

		public ViewDetailLevel DetailLevel {
			get {
				return detailLevel;
			}
		}

		public Graphics Graphics {
			get {
				return g;
			}
		}

		public ModelComponent Component {
			get {
				return (ModelComponent)this.Sender;
			}
		}

		public IList<ILayoutDrawingRegion> Regions {
			get {
				return regions;
			}
		}

		public void AddRegion(ILayoutDrawingRegion region) {
			regions.Add(region);
		}
	}

	/// <summary>
	/// Passed as EventArgs for various view triggered events
	/// </summary>
	public class LayoutViewEventArgs : EventArgs {
		public LayoutHitTestResult HitTestResult;
		public MouseEventArgs MouseEventArgs;
		public DragEventArgs DragEventArgs;

		public LayoutViewEventArgs(LayoutHitTestResult hitTestResult, MouseEventArgs mouseEventArgs) {
			this.HitTestResult = hitTestResult;
			this.MouseEventArgs = mouseEventArgs;
		}

		public LayoutViewEventArgs(LayoutHitTestResult hitTestResult, DragEventArgs dragEventArgs) {
			this.HitTestResult = hitTestResult;
			this.DragEventArgs = dragEventArgs;
		}

	}

	/// <summary>
	/// Manage the allocation of off-screen buffer. Drawing is done to this buffer which is
	/// then drawn on the screen
	/// </summary>
	[LayoutModule("Off-screen Buffer Manager", UserControl=false)]
	class OffscreenBufferManager {
		Bitmap	offScreenBuffer;

		[LayoutEvent("allocate-offscreen-buffer")]
		void AllocateOffScreenBuffer(LayoutEvent e) {
			Graphics	g = (Graphics)e.Sender;
			RectangleF	clipBounds = (RectangleF)e.Info;

			if(offScreenBuffer == null || clipBounds.Width > offScreenBuffer.Width || clipBounds.Height > offScreenBuffer.Height) {
				if(offScreenBuffer != null)
					offScreenBuffer.Dispose();

				offScreenBuffer = new Bitmap((int)Math.Ceiling(clipBounds.Width), (int)Math.Ceiling(clipBounds.Height), g);
			}
				
			e.Info = offScreenBuffer;
		}

		[LayoutEvent("free-resources")]
		void FreeOffScreenBuffer(LayoutEvent e) {
			if(offScreenBuffer != null) {
				offScreenBuffer.Dispose();
				offScreenBuffer = null;
			}
		}
	}
}
