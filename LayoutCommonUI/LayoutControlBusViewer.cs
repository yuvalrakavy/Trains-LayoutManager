using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;

using LayoutManager;
using LayoutManager.Model;

namespace LayoutManager.CommonUI
{
	/// <summary>
	/// Summary description for LayoutControlBusViewer.
	/// </summary>
	public class LayoutControlBusViewer : System.Windows.Forms.Control {
		private IContainer components;
		private System.Windows.Forms.HScrollBar hScrollBar;
		private ImageList imageListConnectionPointTypes;
		private System.Windows.Forms.VScrollBar vScrollBar;

		private void endOfDesignerVariables() { }

		ILayoutFrameWindow		frameWindow = null;

		float					zoom = 1.0F;
		Point					origin = new Point(0, 0);

		DrawControlBase			drawRoot = null;

		Guid					busProviderId = Guid.Empty;
		string					busType = null;
		Guid					controlModuleLocationId = Guid.Empty;
		bool					showOnlyNotInLocation = false;

		PointF					startingPoint = new PointF(10, 10);
		Pen						busPen = new Pen(Color.Black, 2);

		MouseButtons			mouseDownButton = MouseButtons.None;
		DrawControlBase			mouseDrawObject = null;
		IDictionary				imageMap = new HybridDictionary();

		ControlModule			selectedModule = null;
		ControlConnectionPoint	selectedConnectionPoint = null;
		LayoutSelection			selectedComponents = null;

		public LayoutControlBusViewer() {
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			Controls.Add(hScrollBar);
			Controls.Add(vScrollBar);
		}

		#region Public Properties

		public ILayoutFrameWindow FrameWindow {
			get {
				if(frameWindow == null)
					frameWindow = (ILayoutFrameWindow)FindForm();
				return frameWindow;
			}
		}

		public float Zoom {
			get {
				return zoom;
			}

			set {
				zoom = value;
				Recalc();
			}
		}

		public Guid BusProviderId {
			get {
				return busProviderId;
			}

			set {
				busProviderId = value;
				Recalc();
			}
		}

		public string BusTypeName {
			get {
				return busType;
			}

			set {
				busType = value;
				Recalc();
			}
		}

		public Guid ModuleLocationID {
			get {
				return controlModuleLocationId;
			}

			set {
				controlModuleLocationId = value;
				Recalc();
			}
		}

		public PointF StartingPoint {
			get {
				return startingPoint;
			}

			set {
				startingPoint = value;
			}
		}

		public Pen BusPen {
			get {
				return busPen;
			}

			set {
				busPen = value;
			}
		}

		public bool ShowOnlyNotInLocation {
			get {
				return showOnlyNotInLocation;
			}

			set {
				showOnlyNotInLocation = value;
				// Not Recalc not called because it is assumed that ModuleLocationID will be set next
			}
		}

		public bool IsOperationMode {
			get {
				return LayoutController.IsOperationMode;
			}
		}

		public void Initialize() {
			EventManager.AddObjectSubscriptions(this);

			selectedComponents = new LayoutSelection();
		}


		public void EnsureVisible(ControlConnectionPointReference connectionPointRef, bool select) {
			Recalc();
			this.Update();

			DrawControlConnectionPoint	drawConnectionPointObject = drawRoot.FindConnectionPoint(connectionPointRef);

			if(drawConnectionPointObject != null) {
				EnsureVisible(drawConnectionPointObject.DrawModule);

				if(select && connectionPointRef.IsConnected)
					drawConnectionPointObject.Selected = true;
			}
		}

		public void EnsureVisible(ControlModuleReference moduleRef, bool select) {
			Recalc();
			this.Update();

			DrawControlModule drawModuleObject = drawRoot.FindModule(moduleRef);

			if(drawModuleObject != null) {
				EnsureVisible(drawModuleObject);

				if(select)
					drawModuleObject.Selected = true;
			}
		}

		public void DeselectAll() {
			SelectedModule = null;
		}

		private RectangleF DrawnRectangle {
			get {
				return new RectangleF(new PointF(origin.X, origin.Y), new SizeF(
					(ClientSize.Width - (vScrollBar.Visible ? vScrollBar.Width : 0)) / zoom,
					(ClientSize.Height - (hScrollBar.Visible ? hScrollBar.Height : 0)) / zoom));
			}
		}

		internal void EnsureVisible(DrawControlModule drawModuleObject) {
			zoom = 1.0F;

			while(!DrawnRectangle.Contains(drawModuleObject.Bounds)) {
				// Move the origin and check again...

				
				Point	newOrigin = origin;
				
				newOrigin.X = drawModuleObject.Bounds.Left > 20 ? (int)drawModuleObject.Bounds.Left-20 : 0;

				if(drawModuleObject.Bounds.Bottom < newOrigin.Y)
					newOrigin.Y = (int)(drawModuleObject.Bounds.Top - 20F / zoom);
				else if(drawModuleObject.Bounds.Top > DrawnRectangle.Bottom)
					newOrigin.Y = (int)(drawModuleObject.Bounds.Bottom + 20F / zoom - DrawnRectangle.Height);

				origin = newOrigin;

				if(DrawnRectangle.Contains(drawModuleObject.Bounds))
					break;

				// Still cannot fit, adjust the zoom
				zoom = zoom * 0.9F;

				if(zoom < 0.2F)
					break;
			}
		}

		/// <summary>
		/// Get the image of a given connection point type. This image is shown for non-connected connection points
		/// </summary>
		/// <param name="connectionPointType">The connection point type</param>
		/// <param name="topRow">True - the image will be shown on the top row of an image</param>
		/// <returns>An image or null if no image</returns>
		internal Image GetConnectionPointImage(string connectionPointType, bool topRow) {
			string	key = connectionPointType + "-" + topRow.ToString();
			Image	typeImage = null;

			if(!imageMap.Contains(key)) {

				if(connectionPointType == ControlConnectionPointTypes.OutputSolenoid)
					typeImage = imageListConnectionPointTypes.Images[0];
				else if(connectionPointType == ControlConnectionPointTypes.InputDryContact) {
					if(topRow)
						typeImage = imageListConnectionPointTypes.Images[1];
					else
						typeImage = imageListConnectionPointTypes.Images[2];
				}
				else if(connectionPointType == ControlConnectionPointTypes.InputCurrent)
					typeImage = imageListConnectionPointTypes.Images[3];
				else
					typeImage = (Image)EventManager.Event(new LayoutEvent(this, "get-connection-point-type-image").SetOption("ConnectionPointType", connectionPointType).SetOption("TopRow", topRow));
				

				imageMap.Add(key, typeImage);
			}
			else
				typeImage = (Image)imageMap[key];

			return typeImage;
		}

		/// <summary>
		/// Get the image of a given connection point. This image is shown for connected connection points
		/// </summary>
		/// <param name="connectionPoint">The connection point</param>
		/// <param name="topRow">True - the image will be shown on the top row of an image</param>
		/// <returns>An image or null if no image</returns>
		internal Image GetConnectionPointImage(ControlConnectionPoint connectionPoint, bool topRow) {
			IModelComponentConnectToControl	component = (IModelComponentConnectToControl)connectionPoint.Component;

			if(component != null) {
				string						key = component.GetType().Name + "-" + component.ToString() + topRow.ToString() + "_";
				Image			image = null;

				if(!imageMap.Contains(key)) {
					image = (Image)EventManager.Event(
						new LayoutEvent(component, "get-connection-point-component-image").SetOption("TopRow", topRow).SetOption("ModuleID", XmlConvert.ToString(connectionPoint.Module.Id)).SetOption("Index", XmlConvert.ToString(connectionPoint.Index)));

					imageMap.Add(key, image);
				}
				else
					image = (Image)imageMap[key];

				return image;
			}
			else
				return null;
		}

		internal ControlModule SelectedModule {
			get {
				return selectedModule;
			}

			set {
				selectedModule = value;
				selectedConnectionPoint = null;

				updateComponentSelection();
				Invalidate();
			}
		}

		internal ControlConnectionPoint SelectedConnectionPoint {
			get {
				return selectedConnectionPoint;
			}

			set {
				selectedConnectionPoint = value;
				selectedModule = null;

				updateComponentSelection();
				Invalidate();
			}
		}

		#endregion

		#region Internal methods (initialization etc.)

		public void Recalc() {
			drawRoot = new DrawControlBusProviders(this);
			Invalidate();
		}

		internal void VerticalScroll(ScrollEventType type) {
			vScrollBar_Scroll(null, new ScrollEventArgs(type, 0));
		}

		internal void HorizontalScroll(ScrollEventType type) {
			hScrollBar_Scroll(null, new ScrollEventArgs(type, 0));
		}

		private void updateComponentSelection() {
			selectedComponents.Clear();

			if(SelectedConnectionPoint != null) {
				IModelComponentConnectToControl component = SelectedConnectionPoint.Component;

				if(component != null) {
					selectedComponents.Add((ModelComponent)component);
					EventManager.Event(new LayoutEvent(component, "ensure-component-visible", null, false).SetFrameWindow(FrameWindow));
				}
			}

			if(SelectedModule != null) {
				foreach(ControlConnectionPoint connectionPoint in SelectedModule.ConnectionPoints)
					if(connectionPoint.ComponentId != Guid.Empty)
						selectedComponents.Add(connectionPoint.ComponentId);
			}
		}

		#endregion


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing ) {

			if( disposing ) {
				EventManager.Subscriptions.RemoveObjectSubscriptions(this);

				if(components != null)
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayoutControlBusViewer));
			this.hScrollBar = new System.Windows.Forms.HScrollBar();
			this.vScrollBar = new System.Windows.Forms.VScrollBar();
			this.imageListConnectionPointTypes = new ImageList(this.components);
			this.SuspendLayout();
			// 
			// hScrollBar
			// 
			this.hScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.hScrollBar.Location = new System.Drawing.Point(117, 17);
			this.hScrollBar.Name = "hScrollBar";
			this.hScrollBar.Size = new System.Drawing.Size(80, 17);
			this.hScrollBar.TabIndex = 0;
			this.hScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar_Scroll);
			// 
			// vScrollBar
			// 
			this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
			this.vScrollBar.Location = new System.Drawing.Point(17, 17);
			this.vScrollBar.Name = "vScrollBar";
			this.vScrollBar.Size = new System.Drawing.Size(17, 80);
			this.vScrollBar.TabIndex = 0;
			this.vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar_Scroll);
			// 
			// imageListConnectionPointTypes
			// 
			this.imageListConnectionPointTypes.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListConnectionPointTypes.ImageStream")));
			this.imageListConnectionPointTypes.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListConnectionPointTypes.Images.SetKeyName(0, "");
			this.imageListConnectionPointTypes.Images.SetKeyName(1, "");
			this.imageListConnectionPointTypes.Images.SetKeyName(2, "");
			this.imageListConnectionPointTypes.Images.SetKeyName(3, "");
			// 
			// LayoutControlBusViewer
			// 
			this.SizeChanged += new System.EventHandler(this.LayoutControlBusViewer_SizeChanged);
			this.Click += new System.EventHandler(this.LayoutControlBusViewer_Click);
			this.DoubleClick += new System.EventHandler(this.LayoutControlBusViewer_DoubleClick);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LayoutControlBusViewer_KeyDown);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LayoutControlBusViewer_MouseDown);
			this.MouseEnter += new System.EventHandler(this.LayoutControlBusViewer_MouseEnter);
			this.MouseLeave += new System.EventHandler(this.LayoutControlBusViewer_MouseLeave);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LayoutControlBusViewer_MouseMove);
			this.ResumeLayout(false);

		}
		#endregion

		#region Drawing

		private void drawBackground(Graphics g, RectangleF clipBounds) {
			using(Brush b = new SolidBrush(Parent.BackColor))
				g.FillRectangle(b, clipBounds);
		}

		private RectangleF DrawingBounds {
			get {
				return RectangleF.FromLTRB(0, 0, drawRoot.Bounds.Right, drawRoot.Bounds.Bottom);
			}
		}

		private bool updateScrollBars() {
			bool	needRedraw = false;
			float	vMargin = 20 / zoom;
			float	hMargin = 20 / zoom;

			if(drawRoot != null && drawRoot.Bounds != RectangleF.Empty) {
				RectangleF	bounds = DrawingBounds;
				RectangleF	clientBounds = RectangleF.FromLTRB(ClientRectangle.Left / zoom, ClientRectangle.Top / zoom,
					ClientRectangle.Right / zoom, ClientRectangle.Bottom / zoom);

				if(bounds.Width > clientBounds.Width) {
					hScrollBar.Maximum = (int)(bounds.Width - clientBounds.Width + vScrollBar.Width / zoom + hMargin);
					hScrollBar.LargeChange = (int)(hScrollBar.Maximum * clientBounds.Width / bounds.Width);

					hScrollBar.SmallChange = 4;
					hScrollBar.Visible = true;
				}
				else
					hScrollBar.Visible = false;

				if(bounds.Height > clientBounds.Height) {
					vScrollBar.Maximum = (int)(bounds.Height - clientBounds.Height + hScrollBar.Height / zoom + vMargin);
					vScrollBar.LargeChange = (int)(vScrollBar.Maximum * clientBounds.Height / bounds.Height);

					if(vScrollBar.LargeChange > clientBounds.Height)
						vScrollBar.LargeChange = (int)clientBounds.Height;
					vScrollBar.SmallChange = 4;
					vScrollBar.Visible = true;
				}
				else
					vScrollBar.Visible = false;

				if(hScrollBar.Visible == false) {
					origin.X = 0;
					hScrollBar.Value = 0;
					needRedraw = true;
				}

				if(vScrollBar.Visible == false) {
					origin.Y = 0;
					vScrollBar.Value = 0;
					needRedraw = true;
				}
			}

			return needRedraw;
		}

		protected override void OnPaint(PaintEventArgs pe) {
			base.OnPaint(pe);

			if(DesignMode || EventManager.Instance == null)
				return;

			RectangleF	clipBounds = pe.Graphics.VisibleClipBounds;
			Bitmap		buffer = (Bitmap)EventManager.Event(new LayoutEvent(pe.Graphics, "allocate-offscreen-buffer", null, clipBounds));

			using(Graphics g = Graphics.FromImage(buffer)) {
				// Translate so the top-left of the clip region is on the top/left (0,0)
				// of the bitmap
				g.TranslateTransform(-clipBounds.Left, -clipBounds.Top);
				drawBackground(g, clipBounds);

				GraphicsState	gs = g.Save();

				g.ScaleTransform(zoom, zoom);
				g.TranslateTransform(-origin.X, -origin.Y);

				if(drawRoot != null) {
					drawRoot.Draw(g, StartingPoint);

					g.Restore(gs);

					if(updateScrollBars()) {
						drawBackground(g, clipBounds);

						gs = g.Save();

						g.ScaleTransform(zoom, zoom);
						g.TranslateTransform(-origin.X, -origin.Y);

						drawRoot.Draw(g, StartingPoint);

						g.Restore(gs);
					}
				}
			}

			// After the background image is created, draw it on the screen
			pe.Graphics.DrawImage(buffer, clipBounds.Location);
		}

		#endregion

		#region Mouse Hit methods

		DrawControlBase GetHitDrawObject(Point hitPoint) {
			DrawControlBase	result = null;

			// Convert from client coordinate to world coordinates
			PointF		p = new PointF(hitPoint.X / zoom + origin.X, (hitPoint.Y) / zoom + origin.Y);

			if(drawRoot != null)
				result = drawRoot.GetDrawObjectContainingPoint(p);

			return result;
		}

		#endregion

		#region Layout event handlers

		[LayoutEvent("model-loaded")]
		[LayoutEvent("new-layout-document")]
		private void ModelLoaded(LayoutEvent e) {
			Recalc();
		}

		[LayoutEvent("enter-operation-mode")]
		[LayoutEvent("enter-design-mode")]
		[LayoutEvent("control-module-removed")]
		[LayoutEvent("control-module-added")]
		[LayoutEvent("control-module-address-changed")]
		[LayoutEvent("control-module-location-changed")]
		[LayoutEvent("control-bus-reconnected")]
		[LayoutEvent("component-disconnected-from-control-module")]
		[LayoutEvent("component-connected-to-control-module")]
		[LayoutEvent("component-configuration-changed")]
		[LayoutEvent("control-module-label-changed")]
		[LayoutEvent("control-user-action-required-changed")]
		[LayoutEvent("control-address-programming-required-changed")]
		[LayoutEvent("control-buses-added")]
		[LayoutEvent("control-buses-removed")]
		private void changeMode(LayoutEvent e) {
			Recalc();
		}

		[LayoutEvent("component-disconnected-from-control-module")]
		private void componentDisconnectedFromControlModule(LayoutEvent e) {
			ControlConnectionPoint	connectionPoint = (ControlConnectionPoint)e.Info;

			if(selectedModule != null && connectionPoint.Module.Id == selectedModule.Id) {
				SelectedModule = null;
				Invalidate();
			}
			
			if(selectedConnectionPoint != null && selectedConnectionPoint.Module.Id == connectionPoint.Module.Id && selectedConnectionPoint.Index == connectionPoint.Index) {
				SelectedConnectionPoint = null;
				Invalidate();
			}
		}

		[LayoutEvent("layout-control-shown")]
		private void layoutControlShown(LayoutEvent e) {
			selectedComponents.Display(new LayoutSelectionLook(Color.Red));
		}

		[LayoutEvent("layout-control-hidden")]
		private void layoutControlHidden(LayoutEvent e) {
			selectedComponents.Hide();
		}

		#endregion

		#region Event handlers

		protected override void OnPaintBackground(PaintEventArgs pevent) {
			
		}

		private void LayoutControlBusViewer_SizeChanged(object sender, System.EventArgs e) {
			// Check if need to update scroll bars
			if(drawRoot != null) {
				if(updateScrollBars())
					Invalidate();
			}
		}

		private void hScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e) {
			RectangleF	bounds = DrawingBounds;
			RectangleF	clientBounds = RectangleF.FromLTRB(ClientRectangle.Left / zoom, ClientRectangle.Top / zoom,
				ClientRectangle.Right / zoom, ClientRectangle.Bottom / zoom);

			switch(e.Type) {

				case ScrollEventType.First:
					origin.X = 0;
					break;

				case ScrollEventType.Last:
					origin.X = hScrollBar.Maximum;
					break;

				case ScrollEventType.LargeDecrement:
					origin.X -= hScrollBar.LargeChange;
					break;

				case ScrollEventType.LargeIncrement:
					origin.X += hScrollBar.LargeChange;
					break;

				case ScrollEventType.SmallDecrement:
					origin.X -= hScrollBar.SmallChange;
					break;

				case ScrollEventType.SmallIncrement:
					origin.X += hScrollBar.SmallChange;
					break;

				case ScrollEventType.ThumbTrack:
					origin.X = (int)(e.NewValue * (float)hScrollBar.Maximum / (hScrollBar.Maximum - hScrollBar.LargeChange));
					break;
			}

			if(origin.X < hScrollBar.Minimum)
				origin.X = hScrollBar.Minimum;
			else if(origin.X > hScrollBar.Maximum)
				origin.X = hScrollBar.Maximum;
			hScrollBar.Value = origin.X;

			Invalidate();
		}

		private void vScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e) {
			RectangleF	bounds = DrawingBounds;
			RectangleF	clientBounds = RectangleF.FromLTRB(ClientRectangle.Left / zoom, ClientRectangle.Top / zoom,
				ClientRectangle.Right / zoom, ClientRectangle.Bottom / zoom);

			switch(e.Type) {
				case ScrollEventType.First:
					origin.Y = 0;
					break;

				case ScrollEventType.Last:
					origin.Y = vScrollBar.Maximum;
					break;

				case ScrollEventType.LargeDecrement:
					origin.Y -= vScrollBar.LargeChange;
					break;

				case ScrollEventType.LargeIncrement:
					origin.Y += vScrollBar.LargeChange;
					break;

				case ScrollEventType.SmallDecrement:
					origin.Y -= vScrollBar.SmallChange;
					break;

				case ScrollEventType.SmallIncrement:
					origin.Y += vScrollBar.SmallChange;
					break;

				case ScrollEventType.ThumbTrack:
					origin.Y = (int)(e.NewValue * (float)vScrollBar.Maximum / (vScrollBar.Maximum - vScrollBar.LargeChange));
					break;
			}

			if(origin.Y < 0)
				origin.Y = 0;
			else if(origin.Y > vScrollBar.Maximum)
				origin.Y = vScrollBar.Maximum;

			vScrollBar.Value = (int)origin.Y;
			Invalidate();
		}

		private void LayoutControlBusViewer_DoubleClick(object sender, System.EventArgs e) {
			DrawControlBase	hitDrawObject = GetHitDrawObject(PointToClient(Control.MousePosition));

			if(hitDrawObject != null)
				hitDrawObject.OnDoubleClick();
		}

		private void LayoutControlBusViewer_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			DrawControlBase	hitDrawObject = GetHitDrawObject(PointToClient(Control.MousePosition));

			mouseDownButton = e.Button;

			if(hitDrawObject != null)
				hitDrawObject.OnMouseDown(e);
		}

		private void LayoutControlBusViewer_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			DrawControlBase	hitDrawObject = GetHitDrawObject(PointToClient(Control.MousePosition));

			if(hitDrawObject != mouseDrawObject) {
				if(mouseDrawObject != null)
					mouseDrawObject.OnMouseExit();
				
				if(hitDrawObject != null)
					hitDrawObject.OnMouseEnter();

				mouseDrawObject = hitDrawObject;
			}
		}

		private void LayoutControlBusViewer_Click(object sender, System.EventArgs e) {
			DrawControlBase	hitDrawObject = GetHitDrawObject(PointToClient(Control.MousePosition));
		
			if(hitDrawObject != null)
				hitDrawObject.OnClick(mouseDownButton);
		}


		private void LayoutControlBusViewer_MouseLeave(object sender, System.EventArgs e) {
			if(mouseDrawObject != null)
				mouseDrawObject.OnMouseExit();
		}

		protected override bool IsInputKey(Keys keyData) {
			if((keyData & ~Keys.Shift) == Keys.Tab)
				return false;
			return true;
		}

		private void LayoutControlBusViewer_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			switch(e.KeyCode ) {

				case Keys.Left:
					HorizontalScroll((e.Modifiers & Keys.Control) != 0 ? ScrollEventType.LargeDecrement : ScrollEventType.SmallDecrement);
					break;

				case Keys.Right:
					HorizontalScroll((e.Modifiers & Keys.Control) != 0 ? ScrollEventType.LargeIncrement : ScrollEventType.SmallIncrement);
					break;

				case Keys.Up:
					VerticalScroll(ScrollEventType.SmallDecrement);
					break;

				case Keys.Down:
					VerticalScroll(ScrollEventType.SmallIncrement);
					break;

				case Keys.PageUp:
					VerticalScroll(ScrollEventType.LargeDecrement);
					break;

				case Keys.PageDown:
					VerticalScroll(ScrollEventType.LargeIncrement);
					break;

				case Keys.Home:
					if((e.Modifiers & Keys.Control) != 0)
						HorizontalScroll(ScrollEventType.First);
					else
						VerticalScroll(ScrollEventType.First);
					break;

				case Keys.End:
					if((e.Modifiers & Keys.Control) != 0)
						HorizontalScroll(ScrollEventType.Last);
					else
						VerticalScroll(ScrollEventType.Last);
					break;
			}
		}

		protected override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);

			if(e.Delta > 0) {
				int	delta = e.Delta;

				while(delta > 0) {
					if((Control.ModifierKeys & Keys.Shift) != 0) {
						for(int i = 0; i < 3; i++)
							HorizontalScroll(ScrollEventType.SmallDecrement);
					}
					else if((Control.ModifierKeys & Keys.Control) != 0) {
						if(Zoom > 0.3)
							Zoom -= 0.1F;
					}
					else {
						for(int i = 0; i < 6; i++)
							VerticalScroll(ScrollEventType.SmallDecrement);
					}

					delta -= 120;
				}
			}
			else if(e.Delta < 0) {
				int		delta = e.Delta;

				if((Control.ModifierKeys & Keys.Shift) != 0) {
					for(int i = 0; i < 3; i++)
						HorizontalScroll(ScrollEventType.SmallIncrement);
				}
				else if((Control.ModifierKeys & Keys.Control) != 0) {
					if(Zoom < 2.0)
						Zoom += 0.1F;
				}
				else {
					for(int i = 0; i < 6; i++)
						VerticalScroll(ScrollEventType.SmallIncrement);
				}

				delta += 120;
			}
		}

		private void LayoutControlBusViewer_MouseEnter(object sender, System.EventArgs e) {
			Focus();
		}

#endregion
	}

	#region Drawing classes

	public abstract class DrawControlBase {
		LayoutControlBusViewer	viewer;
		bool					enabled = true;

		RectangleF				bounds = RectangleF.Empty;
		ArrayList				drawObjects = new ArrayList();

		Cursor					savedCursor = null;

		public DrawControlBase(LayoutControlBusViewer viewer) {
			this.viewer = viewer;
		}

		public LayoutControlBusViewer Viewer {
			get {
				return viewer;
			}
		}

		public bool Enabled {
			get {
				return enabled;
			}

			set {
				enabled = value;
			}
		}

		/// <summary>
		/// Return the bounds of the drawn object. The result is valid only after calling the Draw method
		/// </summary>
		public virtual RectangleF Bounds {
			get {
				RectangleF	totalBounds = bounds;

				foreach(DrawControlBase drawObject in drawObjects) {
					if(drawObject.Bounds != RectangleF.Empty) {
						if(totalBounds == RectangleF.Empty)
							totalBounds = drawObject.Bounds;
						else
							totalBounds = RectangleF.Union(totalBounds, drawObject.Bounds);
					}
				}

				return totalBounds;
			}

			set {
				bounds = value;
			}
		}

		public void UnionBounds(RectangleF rect) {
			if(bounds == RectangleF.Empty)
				bounds = rect;
			else
				bounds = RectangleF.Union(bounds, rect);
		}

		public void AddDrawObject(DrawControlBase drawObject) {
			drawObjects.Add(drawObject);
		}

		public DrawControlBase[] DrawObjects {
			get {
				return (DrawControlBase[] )drawObjects.ToArray(typeof(DrawControlBase));
			}
		}

		public virtual Cursor Cursor {
			get {
				return null;
			}
		}

		public abstract void Draw(Graphics g, PointF startingPoint);

		public DrawControlBase GetDrawObjectContainingPoint(PointF p) {
			DrawControlBase	hitDrawObject = null;

			// Check if a sub-object was hit
			foreach(DrawControlBase drawObject in DrawObjects)
				if((hitDrawObject = drawObject.GetDrawObjectContainingPoint(p)) != null)
					return hitDrawObject;

			// Check if this object was hit
			if(Bounds.Contains(p))
				return this;

			return null;
		}

		public virtual void OnMouseDown(MouseEventArgs e) {
			if(Enabled) {
				if(e.Button == MouseButtons.Right) {		// Right click
					string	eventName = "add-control-" + (Viewer.IsOperationMode ? "operation" : "editing") + "-context-menu-entries";
					ContextMenu	menu = new ContextMenu();

					EventManager.Event(new LayoutEvent(this, eventName, null, menu).SetFrameWindow(Viewer.FrameWindow));
					if(menu.MenuItems.Count > 0)
						menu.Show(Viewer, Viewer.PointToClient(Control.MousePosition));
				}
			}
		}

		public virtual void OnClick(MouseButtons mouseDownButton) {
			if(mouseDownButton == MouseButtons.Left)														// Normal click
				EventManager.Event(new LayoutEvent(this, "control-default-action").SetFrameWindow(Viewer.FrameWindow));
		}

		public virtual void OnDoubleClick() {
		}

		public virtual void OnMouseEnter() {
			savedCursor = Cursor.Current;

			Cursor	newCursor = this.Cursor;

			if(newCursor != null)
				Viewer.Cursor = newCursor;
		}

		public virtual void OnMouseExit() {
			if(savedCursor != null)
				Viewer.Cursor = savedCursor;
		}

		public virtual DrawControlConnectionPoint FindConnectionPoint(ControlConnectionPointReference connectionPointRef) {
			DrawControlConnectionPoint	result = null;

			foreach(DrawControlBase drawObject in DrawObjects) {
				result = drawObject.FindConnectionPoint(connectionPointRef);
				if(result != null)
					break;
			}

			return result;
		}

		public virtual DrawControlModule FindModule(ControlModuleReference moduleRef) {
			DrawControlModule result = null;

			foreach(DrawControlBase drawObject in DrawObjects) {
				result = drawObject.FindModule(moduleRef);
				if(result != null)
					break;
			}

			return result;
		}
	}

	public class DrawControlBusProviders : DrawControlBase {
		const int	csGap = 20;

		public DrawControlBusProviders(LayoutControlBusViewer viewer) : base(viewer) {
			if(Viewer.BusProviderId == Guid.Empty) {
				if(LayoutModel.Instance != null) {
					foreach(IModelComponentIsBusProvider busProvider in LayoutModel.Components<IModelComponentIsBusProvider>(LayoutModel.ActivePhases)) {
						DrawControlBusProvider drawBusProvider = new DrawControlBusProvider(Viewer, busProvider);

						AddDrawObject(drawBusProvider);
					}
				}
			}
			else {
				var busProvider = LayoutModel.Component<IModelComponentIsBusProvider>(Viewer.BusProviderId, LayoutModel.ActivePhases);

				if(busProvider != null) {
					DrawControlBusProvider	drawBusProvider = new DrawControlBusProvider(Viewer, busProvider);

					AddDrawObject(drawBusProvider);
				}
			}
		}
				

		public override void Draw(Graphics g, PointF startingPoint) {
			PointF		csOrigin = startingPoint;

			foreach(DrawControlBusProvider drawBusProvider in DrawObjects) {
				drawBusProvider.Draw(g, csOrigin);
				csOrigin.X += drawBusProvider.Bounds.Width + csGap;
			}
		}
	}

	public class DrawControlBusProvider : DrawControlBase {
		IModelComponentIsBusProvider	busProvider;

		SizeF		minSize = new SizeF(60, 28);
		const int	busLineHorizontalGap = 6;
		const int	busLineVerticalGap = 16;
		const int	busGap = 16;

		public DrawControlBusProvider(LayoutControlBusViewer viewer, IModelComponentIsBusProvider busProvider) : base(viewer) {
			IEnumerable<ControlBus>	buses = LayoutModel.ControlManager.Buses.Buses(busProvider);

			this.busProvider = busProvider;

			if(Viewer.BusTypeName == null) {
				foreach(ControlBus bus in buses)
					AddDrawObject(new DrawControlBus(Viewer, bus));
			}
			else {
				ControlBus	bus = null;

				foreach(ControlBus aBus in buses)
					if(aBus.BusTypeName == Viewer.BusTypeName) {
						bus = aBus;
						break;
					}

				if(bus != null) {
					DrawControlBus	drawBus = new DrawControlBus(Viewer, bus);
					AddDrawObject(drawBus);
				}
			}
		}

		public override void Draw(Graphics g, PointF startingPoint) {
			RectangleF		busProviderRect;

			using(Font nameFont = new Font("Arial", 9)) {
				SizeF	busProviderRectSize = new SizeF(0, 0);
				SizeF	nameSize = g.MeasureString(busProvider.NameProvider.Name, nameFont);
					
				minSize.Width = Math.Max(minSize.Width, busLineHorizontalGap * (1 + DrawObjects.Length));
				busProviderRectSize.Width = Math.Max(nameSize.Width + 4, minSize.Width);
				busProviderRectSize.Height = Math.Max(nameSize.Height + 4, minSize.Height);

				busProviderRect = new RectangleF(startingPoint, busProviderRectSize);

				g.FillRectangle(Brushes.Khaki, busProviderRect);
				g.DrawRectangle(Pens.Black, busProviderRect.Left, busProviderRect.Top, busProviderRect.Width, busProviderRect.Height);

				StringFormat	format = new StringFormat();

				format.Alignment = StringAlignment.Center;
				format.LineAlignment = StringAlignment.Center;

				g.DrawString(busProvider.NameProvider.Name, nameFont, Brushes.Black, busProviderRect, format);

				UnionBounds(busProviderRect);
			}

			// Draw buses for each bus provider
			int		verticalLength = DrawObjects.Length * busLineVerticalGap;
			PointF	busLineStartingPoint = new PointF(busProviderRect.Left + busLineHorizontalGap, busProviderRect.Bottom);
			PointF	busStartingPoint = new PointF(busLineStartingPoint.X, busLineStartingPoint.Y + verticalLength);

			int		iBus = 0;
			foreach(DrawControlBus drawBus in DrawObjects) {
				PointF	corner1 = new PointF(busLineStartingPoint.X, busLineStartingPoint.Y + (DrawObjects.Length - iBus) * busLineVerticalGap);
				PointF	corner2 = new PointF(busStartingPoint.X, busLineStartingPoint.Y + (DrawObjects.Length - iBus) * busLineVerticalGap);

				g.DrawLines(Viewer.BusPen, new PointF[] {busLineStartingPoint, corner1, corner2, busStartingPoint });
				drawBus.Draw(g, busStartingPoint);

				busLineStartingPoint.X += busLineHorizontalGap;
				busStartingPoint.X += drawBus.Bounds.Width + busGap;
				iBus++;
			}

			UnionBounds(RectangleF.FromLTRB(busProviderRect.Left, busProviderRect.Bottom, busStartingPoint.X, busStartingPoint.Y));
		}
	}

	public class DrawControlBus : DrawControlBase {
		ControlBus	bus;

		const int	belowNameMargin = 6;
		const int	aboveNameMargin = 4;
		const int	leftNameMargin = 4;

		const int	moduleLeftMargin = 6;
		const int	moduleVerticalGap = 10;

		public DrawControlBus(LayoutControlBusViewer viewer, ControlBus bus) : base(viewer) {
			this.bus = bus;

			IList<ControlModule>	modules = bus.Modules;
			List<ControlModule>		modulesByAddress = new List<ControlModule>();

			if(Viewer.ModuleLocationID == Guid.Empty && !Viewer.ShowOnlyNotInLocation)
				modulesByAddress.AddRange(modules);
			else {
				foreach(ControlModule module in modules)
					if((Viewer.ShowOnlyNotInLocation && module.LocationId == Guid.Empty) || module.LocationId == Viewer.ModuleLocationID)
						modulesByAddress.Add(module);
			}

			modulesByAddress.Sort();

			foreach(ControlModule module in modulesByAddress)
				AddDrawObject(new DrawControlModule(Viewer, module));

			if(!Viewer.IsOperationMode) {
				DrawControlClickToAddModule	d = new DrawControlClickToAddModule(Viewer, bus);

				AddDrawObject(d);
			}
		}

		[Flags]
		enum DrawControlBusLineFlags {
			Default			= 0,
			LastComponent	= 0x00000001,
			BrokenBus		= 0x00000002,		// Show the bus as broken (daisy chain bus only)
		}

		public ControlBus Bus {
			get {
				return bus;
			}
		}

		public override void Draw(Graphics g, PointF startingPoint) {
			PointF		busStartPoint;

			using(Font busNameFont = new Font("Arial", 8)) {
				SizeF	busNameSize = g.MeasureString(bus.BusType.Name, busNameFont);

				busStartPoint = new PointF(startingPoint.X, startingPoint.Y + busNameSize.Height + aboveNameMargin + belowNameMargin);

				g.DrawLine(Viewer.BusPen, startingPoint, busStartPoint);
					
				StringFormat	format = new StringFormat();

				format.LineAlignment = StringAlignment.Center;

				PointF	busNamePoint = new PointF(startingPoint.X + leftNameMargin, startingPoint.Y + aboveNameMargin);

				g.DrawString(bus.BusType.Name, busNameFont, Brushes.Black, busNamePoint);
				UnionBounds(new RectangleF(busNamePoint, busNameSize));
			}

			bool	daisyChainBus = bus.BusType.Topology == ControlBusTopology.DaisyChain;
			int		expectedAddress = bus.BusType.FirstAddress;

			// Draw the modules on the bus
			if(DrawObjects.Length > 0) {
				DrawControlBase	lastModule = DrawObjects[DrawObjects.Length-1];

				foreach(DrawControlBase drawObject in DrawObjects) {
					DrawControlBusLineFlags	flags = DrawControlBusLineFlags.Default;
					DrawControlModule		drawModule = drawObject as DrawControlModule;

					if(drawModule != null && daisyChainBus) {
						if(drawModule.Module.Address != expectedAddress)
							flags |= DrawControlBusLineFlags.BrokenBus;

						expectedAddress = drawModule.Module.Address + drawModule.Module.ModuleType.NumberOfAddresses;
					}
					
					if(drawObject == lastModule)
						flags |= DrawControlBusLineFlags.LastComponent;

					drawObject.Draw(g, new PointF(busStartPoint.X + moduleLeftMargin, busStartPoint.Y + moduleVerticalGap));
					busStartPoint = drawBusLine(g, busStartPoint, drawObject, flags);
				}
			}
		}

		PointF drawBusLine(Graphics g, PointF busStartPoint, DrawControlBase moduleDrawObject, DrawControlBusLineFlags flags) {
			PointF	busEndPoint;

			if(bus.BusType.Topology == ControlBusTopology.DaisyChain) {
				float		daisyChainVerticalMargin = moduleDrawObject.Bounds.Height / 4;
				float		y = moduleDrawObject.Bounds.Top + daisyChainVerticalMargin;

				busEndPoint = new PointF(busStartPoint.X, y);

				if((flags & DrawControlBusLineFlags.BrokenBus) != 0) {
					float	y1 = (busStartPoint.Y + busEndPoint.Y) / 2;
					PointF	p1 = new PointF(busStartPoint.X, y1-2);
					PointF	p2 = new PointF(busStartPoint.X, y1+2);

					g.DrawLine(Viewer.BusPen, busStartPoint, p1);
					g.DrawLine(Viewer.BusPen, p2, busEndPoint);
					g.DrawLine(Pens.Black, new PointF(p1.X-4, p1.Y-2), new PointF(p1.X+4, p1.Y+2));
					g.DrawLine(Pens.Black, new PointF(p2.X-4, p2.Y-2), new PointF(p2.X+4, p2.Y+2));
				}
				else
					g.DrawLine(Viewer.BusPen, busStartPoint, busEndPoint);

				g.DrawLine(Viewer.BusPen, busEndPoint, new PointF(moduleDrawObject.Bounds.Left, busEndPoint.Y));

				if((flags & DrawControlBusLineFlags.LastComponent) == 0) {			// It is not last component
					busStartPoint = new PointF(busStartPoint.X, moduleDrawObject.Bounds.Bottom - daisyChainVerticalMargin);
					busEndPoint = new PointF(busStartPoint.X, moduleDrawObject.Bounds.Bottom);

					g.DrawLines(Viewer.BusPen, 
						new PointF[] { new PointF(moduleDrawObject.Bounds.Left, busStartPoint.Y), busStartPoint, busEndPoint });
				}
			}
			else {
				float		y = moduleDrawObject.Bounds.Top + moduleDrawObject.Bounds.Height / 2;

				if((flags & DrawControlBusLineFlags.LastComponent) != 0)
					busEndPoint = new PointF(busStartPoint.X, y);
				else
					busEndPoint = new PointF(busStartPoint.X, moduleDrawObject.Bounds.Bottom);

				g.DrawLine(Viewer.BusPen, busStartPoint, busEndPoint);
				g.DrawLine(Viewer.BusPen, new PointF(busStartPoint.X, y), new PointF(moduleDrawObject.Bounds.Left, y));
			}

			return busEndPoint;
		}
	}

	public class DrawControlModule : DrawControlBase {
		ControlModule	module;
		RectangleF		moduleRect = RectangleF.Empty;
		RectangleF		phaseTextRect = RectangleF.Empty;
		RectangleF		moduleTextRect = RectangleF.Empty;
		string			moduleText;

		SizeF							minModuleSize = new SizeF(80, 50);		// Minimal module size
		const int						verticalModuleNameMargin = 24;
		const int						horizontalModuleNameMargin = 8;
		const int						connectionPointMargin = 10;
		const int						connectionPointGap = 10;
		DrawControlConnectionPoint[]	topRow = null;
		DrawControlConnectionPoint[]	bottomRow = null;

		public DrawControlModule(LayoutControlBusViewer viewer, ControlModule module) : base(viewer) {
			this.module = module;

			for(int index = 0; index < module.ModuleType.NumberOfConnectionPoints; index++)
				AddDrawObject(new DrawControlConnectionPoint(Viewer, this, index));

			if((module.ModuleType.ConnectionPointArrangement & ControlModuleConnectionPointArrangementOptions.TopRow) != 0) {
				int		b = ((module.ModuleType.ConnectionPointArrangement & ControlModuleConnectionPointArrangementOptions.StartOnBottom) == ControlModuleConnectionPointArrangementOptions.StartOnBottom) ? ConnectionPointsPerRow : 0;
				int		t = 0;

				topRow = new DrawControlConnectionPoint[ConnectionPointsPerRow];

				if((module.ModuleType.ConnectionPointArrangement & ControlModuleConnectionPointArrangementOptions.TopRightToLeft) == ControlModuleConnectionPointArrangementOptions.TopRightToLeft) {

					for(int i = b + ConnectionPointsPerRow - 1; i >= b; i--) {
						topRow[t] = (DrawControlConnectionPoint)DrawObjects[i];
						topRow[t++].TopRow = true;
					}
				}
				else {
					for(int i = b; i < b+ConnectionPointsPerRow; i++) {
						topRow[t] = (DrawControlConnectionPoint)DrawObjects[i];
						topRow[t++].TopRow = true;
					}
				}
			}

			if((module.ModuleType.ConnectionPointArrangement & ControlModuleConnectionPointArrangementOptions.BottomRow) != 0) {
				int		b = ((module.ModuleType.ConnectionPointArrangement & ControlModuleConnectionPointArrangementOptions.StartOnBottom) == ControlModuleConnectionPointArrangementOptions.StartOnBottom) ? 0 : ConnectionPointsPerRow;
				int		t = 0;

				bottomRow = new DrawControlConnectionPoint[ConnectionPointsPerRow];

				if((module.ModuleType.ConnectionPointArrangement & ControlModuleConnectionPointArrangementOptions.BottomRightToLeft) == ControlModuleConnectionPointArrangementOptions.BottomRightToLeft) {
					for(int i = b + ConnectionPointsPerRow - 1; i >= b; i--) {
						bottomRow[t] = (DrawControlConnectionPoint)DrawObjects[i];
						bottomRow[t++].TopRow = false;
					}
				}
				else {
					for(int i = b; i < b+ConnectionPointsPerRow; i++) {
						bottomRow[t] = (DrawControlConnectionPoint)DrawObjects[i];
						bottomRow[t++].TopRow = false;
					}
				}
			}
		}

		public ControlModule Module {
			get {
				return module;
			}
		}

		public bool Selected {
			get {
				if(Viewer.SelectedModule == null)
					return false;
				return Viewer.SelectedModule.Id == Module.Id;
			}

			set {
				if(value)
					Viewer.SelectedModule = Module;
				else
					Viewer.SelectedModule = null;
			}
		}

		private void drawConnectionPoints(Graphics g, DrawControlConnectionPoint[] drawObjects, PointF cpStartingPoint, bool topRow) {
			StringFormat	format = new StringFormat();

			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;

			using(Font labelFont = new Font("Arial", 7.5F)) {
				foreach(DrawControlConnectionPoint drawObject in drawObjects) {
					drawObject.Draw(g, cpStartingPoint);

					string		label = module.ConnectionPoints.GetLabel(drawObject.Index);
					SizeF		labelSize = g.MeasureString(label, labelFont);
					
					RectangleF	labelRect = new RectangleF(new PointF(drawObject.Bounds.Left + (drawObject.Bounds.Width - labelSize.Width) / 2, 
						topRow ? drawObject.Bounds.Bottom : drawObject.Bounds.Top - labelSize.Height), labelSize);

					g.DrawString(label, labelFont, Brushes.Gray, labelRect, format);

					cpStartingPoint.X += drawObject.Bounds.Width + connectionPointGap;
					UnionBounds(drawObject.Bounds);
				}
			}
		}

		public override void Draw(Graphics g, PointF startingPoint) {
			if(moduleRect == RectangleF.Empty)
				calcModuleRect(g, startingPoint);

			StringFormat	format = new StringFormat();

			format.Alignment = StringAlignment.Near;

			if(Selected)
				g.FillRectangle(Brushes.White, moduleRect.Left, moduleRect.Top, moduleRect.Width, moduleRect.Height);

			g.DrawRectangle(Pens.Black, moduleRect.Left, moduleRect.Top, moduleRect.Width, moduleRect.Height);

			if(!Viewer.IsOperationMode && module.UserActionRequired) {
				RectangleF	actionRect = new RectangleF(moduleRect.Location, new SizeF(4, moduleRect.Height));

				g.FillRectangle(Brushes.Red, actionRect.Left, actionRect.Top, actionRect.Width, actionRect.Height);
			}

			if(!Viewer.IsOperationMode && module.AddressProgrammingRequired) {
				RectangleF rect = new RectangleF(new PointF(moduleRect.Location.X+4, moduleRect.Location.Y), new SizeF(4, moduleRect.Height));

				g.FillRectangle(Brushes.DarkBlue, rect.Left, rect.Top, rect.Width, rect.Height);
			}

			string modulePhaseText = getModulePhaseText();

			if(modulePhaseText != null) {
				using(var font = getModulePhaseFont())
					g.DrawString(modulePhaseText, font, Brushes.Gray, phaseTextRect, format);
			}

			using(var font = getModuleNameFont())
				g.DrawString(moduleText, font, Brushes.Black, moduleTextRect, format);

			UnionBounds(moduleRect);

			// Draw top row connection points
			if(topRow != null) 
				drawConnectionPoints(g, topRow, new PointF(moduleRect.Left + connectionPointMargin, moduleRect.Top), true);

			if(bottomRow != null)
				drawConnectionPoints(g, bottomRow, new PointF(moduleRect.Left + connectionPointMargin, moduleRect.Bottom), false);
		}

		private Font getModuleNameFont() {
			return new Font("Arial", 8, FontStyle.Bold);
		}

		private Font getModulePhaseFont() {
			return new Font("Arial", 8, FontStyle.Regular);
		}

		int ConnectionPointsPerRow {
			get {
				int		connectionPointsPerRow = module.ModuleType.NumberOfConnectionPoints;

				if((module.ModuleType.ConnectionPointArrangement & ControlModuleConnectionPointArrangementOptions.BothRows) == ControlModuleConnectionPointArrangementOptions.BothRows)
					connectionPointsPerRow /= 2;

				return connectionPointsPerRow;
			}
		}

		private string getModulePhaseText() {
			switch(Module.Phase) {
				default:
				case LayoutPhase.Operational: return null;
				case LayoutPhase.Construction: return "(In construction)";
				case LayoutPhase.Planned: return "(Planned)";
			}
		}

		private void calcModuleRect(Graphics g, PointF startingPoint) {
			SizeF	moduleNameTextSize;

			startingPoint.Y += DrawControlConnectionPoint.Measure().Height;

			string phaseText = getModulePhaseText();
			SizeF modulePhaseTextSize = new Size(0, 0);

			if(phaseText != null) {
				using(Font modulePhaseFont = getModulePhaseFont())
					modulePhaseTextSize = g.MeasureString(phaseText, modulePhaseFont);
			}

			moduleText = getModuleText();

			using(Font moduleNameFont = getModuleNameFont())
				moduleNameTextSize = g.MeasureString(moduleText, moduleNameFont);

			SizeF	moduleSize = new SizeF(0, 0);
			SizeF	textSize = new SizeF(Math.Max(modulePhaseTextSize.Width, moduleNameTextSize.Width), modulePhaseTextSize.Height + moduleNameTextSize.Height);


			moduleSize.Height = 2 * verticalModuleNameMargin + textSize.Height;
			moduleSize.Width  = 2 * horizontalModuleNameMargin + textSize.Width;

			int		connectionPointsPerRow = ConnectionPointsPerRow;
			float	connectionPointRowWidth = 2*connectionPointMargin + (connectionPointsPerRow-1) * connectionPointGap + connectionPointsPerRow * DrawControlConnectionPoint.Measure().Width;

			moduleSize.Width = Math.Max(moduleSize.Width, minModuleSize.Width);
			moduleSize.Width = Math.Max(moduleSize.Width, connectionPointRowWidth);
			moduleSize.Height = Math.Max(moduleSize.Height, minModuleSize.Height);

			moduleRect = new RectangleF(startingPoint, moduleSize);
			phaseTextRect = new RectangleF(new PointF(startingPoint.X + horizontalModuleNameMargin, startingPoint.Y + (moduleSize.Height - textSize.Height) / 2), modulePhaseTextSize);
			moduleTextRect = new RectangleF(new PointF(startingPoint.X + horizontalModuleNameMargin, phaseTextRect.Bottom), textSize);
		}

		private string getModuleText() {
			string addressText;

			if(module.ModuleType.NumberOfAddresses == 1)
				addressText = module.Address.ToString();
			else {
				int lastAddress = module.Address + module.ModuleType.NumberOfAddresses - 1;

				addressText = module.Address.ToString() + " - " + lastAddress.ToString();
			}

			return module.ModuleType.Name + " (" + addressText + ")" + (module.Label != null ? "\n" + module.Label : "");
		}

		public override DrawControlModule FindModule(ControlModuleReference moduleRef) {
			if(module.Id == moduleRef.ModuleId)
				return this;
			return null;
		}

	}

	public class DrawControlClickToAddModule : DrawControlBase {
		ControlBus	bus;
		string		text = "";

		const int	horizontalMargin = 4;
		const int	verticalMargin = 3;

		public DrawControlClickToAddModule(LayoutControlBusViewer viewer, ControlBus bus) : base(viewer) {
			bool	busFull = true;
			int		address = bus.BusType.FirstAddress;

			this.bus = bus;

			while(address <= bus.BusType.LastAddress) {
				ControlModule	module = bus.GetModuleUsingAddress(address);

				if(module == null) {
					busFull = false;
					break;
				}
				else
					address += module.ModuleType.NumberOfAddresses;
			}

			Enabled = true;

			if(busFull) {
				text = "Bus is full";
				Enabled = false;
			}
			else {
				IList<ControlModuleType>	moduleTypes = bus.BusType.ModuleTypes;

				if(moduleTypes.Count == 1)
					text = "Click here to Add " + moduleTypes[0].Name;
				else if(moduleTypes.Count == 0) {
					text = "No modules can be added";
					Enabled = false;
				}
				else
					text = "Click here to add module";
			}
		}

		public ControlBus Bus {
			get {
				return bus;
			}
		}

		public override Cursor Cursor {
			get {
				return Cursors.Hand;
			}
		}


		public override void Draw(Graphics g, PointF startingPoint) {
			Font	textFont;
			Pen		framePen;
			Brush	textBrush;

			if(Enabled) {
				textFont = new Font("Arial", 7.5F, FontStyle.Underline);
				framePen = new Pen(Color.Black);
				framePen.DashStyle = DashStyle.Dash;
				textBrush = Brushes.Blue;
			}
			else {
				textFont = new Font("Arial", 7.5F);
				framePen = new Pen(Color.Gray);
				textBrush = Brushes.Gray;
			}

			using(textFont) {
				using(framePen) {
					SizeF			textSize = g.MeasureString(text, textFont);
					RectangleF		r = new RectangleF(startingPoint, new SizeF(textSize.Width + 2*horizontalMargin, textSize.Height + 2*verticalMargin));
					StringFormat	format = new StringFormat();

					format.Alignment = StringAlignment.Center;
					format.LineAlignment = StringAlignment.Center;

					g.DrawRectangle(framePen, r.Left, r.Top, r.Width, r.Height);
					g.DrawString(text, textFont, textBrush, r, format);

					UnionBounds(r);
				}
			}
		}
	}

	public class DrawControlConnectionPoint : DrawControlBase {
		DrawControlModule	drawModule;
		int					index;
		bool				topRow = false;
		Image				image = null;

		const int		connectionPointWidth = 20;
		const int		connectionPointShapeHeight = 20;
		const int		connectionPointStatusHeight = 5;
		const int		connectionPointHeight = connectionPointShapeHeight + connectionPointStatusHeight - 1;
		const int		connectionPointOffset = 10;			// Offset into module

		public DrawControlConnectionPoint(LayoutControlBusViewer viewer, DrawControlModule drawModule, int index) : base(viewer) {
			this.drawModule = drawModule;
			this.index = index;
		}

		public ControlModule Module {
			get {
				return drawModule.Module;
			}
		}

		public DrawControlModule DrawModule {
			get {
				return drawModule;
			}
		}

		public int Index {
			get {
				return index;
			}
		}

		public bool Selected {
			get {
				if(Viewer.SelectedConnectionPoint == null)
					return false;
				return Viewer.SelectedConnectionPoint.Module.Id == Module.Id && Viewer.SelectedConnectionPoint.Index == Index;
			}

			set {
				if(value)
					Viewer.SelectedConnectionPoint = Module.ConnectionPoints[Index];
				else
					Viewer.SelectedConnectionPoint = null;
			}
		}

		private bool drawSelection {
			get {
				return Selected || (drawModule.Selected && Module.ConnectionPoints.IsConnected(index));
			}
		}

		public bool TopRow {
			get {
				return topRow;
			}

			set {
				topRow = value;

				if(Module.ConnectionPoints.IsConnected(index))
					image = Viewer.GetConnectionPointImage(Module.ConnectionPoints[index], topRow);
				else
					image = Viewer.GetConnectionPointImage(Module.ConnectionPoints.GetConnectionPointType(index), topRow);
			}
		}

		public ControlConnectionPoint ConnectionPoint {
			get {
				return Module.ConnectionPoints[index];
			}
		}

		public static SizeF Measure() {
			return new SizeF(connectionPointWidth, connectionPointHeight);
		}

		public string FullDescription {
			get {
				return Module.ModuleType.Name + " address " + Module.ConnectionPoints.GetLabel(index);
			}
		}
						
		public override void Draw(Graphics g, PointF startingPoint) {
			RectangleF	shapeRect;
			RectangleF	statusRect;

			if(topRow) {
				shapeRect = new RectangleF(
					new PointF(startingPoint.X, startingPoint.Y - connectionPointHeight + connectionPointOffset), new SizeF(connectionPointWidth, connectionPointShapeHeight));
				statusRect = new RectangleF(new PointF(startingPoint.X, shapeRect.Top - connectionPointStatusHeight + 1), new SizeF(shapeRect.Width, connectionPointStatusHeight));
			}
			else {
				shapeRect = new RectangleF(new PointF(startingPoint.X, startingPoint.Y - connectionPointOffset + 2), new SizeF(connectionPointWidth, connectionPointShapeHeight));
				statusRect = new RectangleF(new PointF(startingPoint.X, shapeRect.Bottom), new SizeF(shapeRect.Width, connectionPointStatusHeight));
			}

			Color	backColor = drawSelection ? Color.White : Viewer.BackColor;

			using(Brush backBrush = new SolidBrush(backColor)) {
				g.FillRectangle(backBrush, shapeRect);
				g.DrawRectangle(Pens.Black, shapeRect.Left, shapeRect.Top, shapeRect.Width, shapeRect.Height);

				if(!Viewer.IsOperationMode && DrawModule.Module.ConnectionPoints.IsUserActionRequired(index))
					g.FillRectangle(Brushes.Red, statusRect);
				else
					g.FillRectangle(backBrush, statusRect);

				g.DrawRectangle(Pens.Black, statusRect.Left, statusRect.Top, statusRect.Width, statusRect.Height);

				if(drawSelection)
					g.DrawRectangle(Pens.Red, shapeRect.Left+1, shapeRect.Top+1, shapeRect.Width-2, shapeRect.Height-2);

				if(image != null)
					g.DrawImage(image, shapeRect.Left + 2, shapeRect.Top + 2);
			}

			UnionBounds(shapeRect);
			UnionBounds(statusRect);
		}

		public override Cursor Cursor {
			get {
				ControlConnectionPointDestination connectionDestination = (ControlConnectionPointDestination)EventManager.Event(new LayoutEvent(this, "get-component-to-control-connect"));

				if(connectionDestination != null && Module.ConnectionPoints.CanBeConnected(connectionDestination, index))
					return Cursors.Cross;
				else
					return base.Cursor;
			}
		}

		public override DrawControlConnectionPoint FindConnectionPoint(ControlConnectionPointReference connectionPointRef) {
			if(connectionPointRef.Module.Id == drawModule.Module.Id && connectionPointRef.Index == Index)
				return this;
			return null;
		}
	}

	#endregion
}
