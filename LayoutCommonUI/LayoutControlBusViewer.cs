using System.ComponentModel;
using System.Drawing.Drawing2D;
using MethodDispatcher;
using LayoutManager.Model;

namespace LayoutManager.CommonUI {
    /// <summary>
    /// Summary description for LayoutControlBusViewer.
    /// </summary>
    public partial class LayoutControlBusViewer : UserControl {

        private ILayoutFrameWindow? frameWindow = null;

        private float zoom = 1.0F;
        private Point origin = new(0, 0);

        private DrawControlBase? drawRoot = null;

        private Guid busProviderId = Guid.Empty;
        private string? busType = null;
        private Guid controlModuleLocationId = Guid.Empty;

        private MouseButtons mouseDownButton = MouseButtons.None;
        private DrawControlBase? mouseDrawObject = null;
        private readonly Dictionary<string, Image?> imageMap = new();

        private ControlModule? selectedModule = null;
        private ControlConnectionPoint? selectedConnectionPoint = null;
        private readonly LayoutSelection selectedComponents;

        public LayoutControlBusViewer() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            selectedComponents = new LayoutSelection();
        }


    #region Public Properties

    public ILayoutFrameWindow FrameWindow => frameWindow ??= (ILayoutFrameWindow)FindForm();

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

        public string? BusTypeName {
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

        public PointF StartingPoint { get; set; } = new PointF(10, 10);

        public Pen BusPen { get; set; } = new Pen(Color.Black, 2);

        public bool ShowOnlyNotInLocation { get; set; } = false;

        public bool IsOperationMode => LayoutController.IsOperationMode;

        public void Initialize() {
            EventManager.AddObjectSubscriptions(this);
            Dispatch.AddObjectInstanceDispatcherTargets(this);
        }

        public void EnsureVisible(ControlConnectionPointReference connectionPointRef, bool select) {
            Recalc();
            this.Update();

            var drawConnectionPointObject = DrawRoot.FindConnectionPoint(connectionPointRef);

            if (drawConnectionPointObject != null) {
                EnsureVisible(drawConnectionPointObject.DrawModule);

                if (select && connectionPointRef.IsConnected)
                    drawConnectionPointObject.Selected = true;
            }
        }

        public void EnsureVisible(ControlModuleReference moduleRef, bool select) {
            Recalc();
            this.Update();

            var drawModuleObject = DrawRoot.FindModule(moduleRef);

            if (drawModuleObject != null) {
                EnsureVisible(drawModuleObject);

                if (select)
                    drawModuleObject.Selected = true;
            }
        }

        public void DeselectAll() {
            SelectedModule = null;
        }

        private RectangleF DrawnRectangle => new(new PointF(origin.X, origin.Y), new SizeF(
                    (ClientSize.Width - (vScrollBar.Visible ? vScrollBar.Width : 0)) / zoom,
                    (ClientSize.Height - (hScrollBar.Visible ? hScrollBar.Height : 0)) / zoom));

        internal void EnsureVisible(DrawControlModule drawModuleObject) {
            zoom = 1.0F;

            while (!DrawnRectangle.Contains(drawModuleObject.Bounds)) {
                // Move the origin and check again...

                Point newOrigin = origin;

                newOrigin.X = drawModuleObject.Bounds.Left > 20 ? (int)drawModuleObject.Bounds.Left - 20 : 0;

                if (drawModuleObject.Bounds.Bottom < newOrigin.Y)
                    newOrigin.Y = (int)(drawModuleObject.Bounds.Top - (20F / zoom));
                else if (drawModuleObject.Bounds.Top > DrawnRectangle.Bottom)
                    newOrigin.Y = (int)(drawModuleObject.Bounds.Bottom + (20F / zoom) - DrawnRectangle.Height);

                origin = newOrigin;

                if (DrawnRectangle.Contains(drawModuleObject.Bounds))
                    break;

                // Still cannot fit, adjust the zoom
                zoom *= 0.9F;

                if (zoom < 0.2F)
                    break;
            }
        }

        /// <summary>
        /// Get the image of a given connection point type. This image is shown for non-connected connection points
        /// </summary>
        /// <param name="connectionPointType">The connection point type</param>
        /// <param name="topRow">True - the image will be shown on the top row of an image</param>
        /// <returns>An image or null if no image</returns>
        internal Image? GetConnectionPointImage(string connectionPointType, bool topRow) {
            string key = connectionPointType + "-" + topRow.ToString();
            Image? typeImage;

            if (!imageMap.ContainsKey(key)) {
                if (connectionPointType == ControlConnectionPointTypes.OutputSolenoid)
                    typeImage = imageListConnectionPointTypes.Images[0];
                else if (connectionPointType == ControlConnectionPointTypes.InputDryTrigger) {
                    if (topRow)
                        typeImage = imageListConnectionPointTypes.Images[1];
                    else
                        typeImage = imageListConnectionPointTypes.Images[2];
                }
                else if (connectionPointType == ControlConnectionPointTypes.InputCurrent)
                    typeImage = imageListConnectionPointTypes.Images[3];
                else
                    typeImage = (Image?)EventManager.Event(new LayoutEvent("get-connection-point-type-image", this).SetOption("ConnectionPointType", connectionPointType).SetOption("TopRow", topRow));

                imageMap.Add(key, typeImage);
            }
            else
                typeImage = (Image?)imageMap[key];

            return typeImage;
        }

        /// <summary>
        /// Get the image of a given connection point. This image is shown for connected connection points
        /// </summary>
        /// <param name="connectionPoint">The connection point</param>
        /// <param name="topRow">True - the image will be shown on the top row of an image</param>
        /// <returns>An image or null if no image</returns>
        internal Image? GetConnectionPointImage(ControlConnectionPoint connectionPoint, bool topRow) {
            var component = connectionPoint.Component;

            if (component != null) {
                string key = component.GetType().Name + "-" + component.ToString() + topRow.ToString() + "_";
                Image? image;

                if (!imageMap.ContainsKey(key)) {
                    image = (Image?)EventManager.Event(
                        new LayoutEvent("get-connection-point-component-image", component).SetOption("TopRow", topRow).SetOption("ModuleID", connectionPoint.Module.Id).SetOption("Index", connectionPoint.Index));

                    imageMap.Add(key, image);
                }
                else
                    image = (Image?)imageMap[key];

                return image;
            }
            else
                return null;
        }

        internal ControlModule? SelectedModule {
            get {
                return selectedModule;
            }

            set {
                selectedModule = value;
                selectedConnectionPoint = null;

                UpdateComponentSelection();
                Invalidate();
            }
        }

        internal ControlConnectionPoint? SelectedConnectionPoint {
            get {
                return selectedConnectionPoint;
            }

            set {
                selectedConnectionPoint = value;
                selectedModule = null;

                UpdateComponentSelection();
                Invalidate();
            }
        }

        #endregion

        #region Internal methods (initialization etc.)

        public void Recalc() {
            DrawRoot = new DrawControlBusProviders(this);
            Invalidate();
        }

        internal void DoVerticalScroll(ScrollEventType type) {
            VScrollBar_Scroll(null, new ScrollEventArgs(type, 0));
        }

        internal void DoHorizontalScroll(ScrollEventType type) {
            HScrollBar_Scroll(null, new ScrollEventArgs(type, 0));
        }

        private void UpdateComponentSelection() {
            selectedComponents.Clear();

            if (SelectedConnectionPoint != null) {
                var component = SelectedConnectionPoint.Component;

                if (component != null) {
                    selectedComponents.Add((ModelComponent)component);
                    EventManager.Event(new LayoutEvent("ensure-component-visible", component, false).SetFrameWindow(FrameWindow));
                }
            }

            if (SelectedModule != null) {
                foreach (var connectionPoint in SelectedModule.ConnectionPoints)
                    if (connectionPoint.ComponentId != Guid.Empty)
                        selectedComponents.Add(connectionPoint.ComponentId);
            }
        }

        #endregion

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                EventManager.Subscriptions.RemoveObjectSubscriptions(this);

                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

 
        #region Drawing

        private void DrawBackground(Graphics g, RectangleF clipBounds) {
            using Brush b = new SolidBrush(Parent.BackColor);
            g.FillRectangle(b, clipBounds);
        }

        private RectangleF DrawingBounds => RectangleF.FromLTRB(0, 0, DrawRoot.Bounds.Right, DrawRoot.Bounds.Bottom);

        public DrawControlBase? OptionalDrawRoot {
            get => drawRoot;
            set => drawRoot = value;
        }

        public DrawControlBase DrawRoot {
            get => Ensure.NotNull<DrawControlBase>(OptionalDrawRoot);
            set => OptionalDrawRoot = value;
        }

        private bool UpdateScrollBars() {
            bool needRedraw = false;
            float vMargin = 20 / zoom;
            float hMargin = 20 / zoom;

            if (DrawRoot != null && DrawRoot.Bounds != RectangleF.Empty) {
                RectangleF bounds = DrawingBounds;
                RectangleF clientBounds = RectangleF.FromLTRB(ClientRectangle.Left / zoom, ClientRectangle.Top / zoom,
                    ClientRectangle.Right / zoom, ClientRectangle.Bottom / zoom);

                if (bounds.Width > clientBounds.Width) {
                    hScrollBar.Maximum = (int)(bounds.Width - clientBounds.Width + (vScrollBar.Width / zoom) + hMargin);
                    hScrollBar.LargeChange = (int)(hScrollBar.Maximum * clientBounds.Width / bounds.Width);

                    hScrollBar.SmallChange = 4;
                    hScrollBar.Visible = true;
                }
                else
                    hScrollBar.Visible = false;

                if (bounds.Height > clientBounds.Height) {
                    vScrollBar.Maximum = (int)(bounds.Height - clientBounds.Height + (hScrollBar.Height / zoom) + vMargin);
                    vScrollBar.LargeChange = (int)(vScrollBar.Maximum * clientBounds.Height / bounds.Height);

                    if (vScrollBar.LargeChange > clientBounds.Height)
                        vScrollBar.LargeChange = (int)clientBounds.Height;
                    vScrollBar.SmallChange = 4;
                    vScrollBar.Visible = true;
                }
                else
                    vScrollBar.Visible = false;

                if (!hScrollBar.Visible) {
                    origin.X = 0;
                    hScrollBar.Value = 0;
                    needRedraw = true;
                }

                if (!vScrollBar.Visible) {
                    origin.Y = 0;
                    vScrollBar.Value = 0;
                    needRedraw = true;
                }
            }

            return needRedraw;
        }

        protected override void OnPaint(PaintEventArgs pe) {
            base.OnPaint(pe);

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime || EventManager.OptionalInstance == null)
                return;

            var clipBounds = pe.Graphics.VisibleClipBounds;
            var buffer = Ensure.NotNull<Bitmap>(EventManager.Event(new LayoutEvent("allocate-offscreen-buffer", pe.Graphics, clipBounds)));

            using (Graphics g = Graphics.FromImage(buffer)) {
                // Translate so the top-left of the clip region is on the top/left (0,0)
                // of the bitmap
                g.TranslateTransform(-clipBounds.Left, -clipBounds.Top);
                DrawBackground(g, clipBounds);

                GraphicsState gs = g.Save();

                g.ScaleTransform(zoom, zoom);
                g.TranslateTransform(-origin.X, -origin.Y);

                if (DrawRoot != null) {
                    DrawRoot.Draw(g, StartingPoint);

                    g.Restore(gs);

                    if (UpdateScrollBars()) {
                        DrawBackground(g, clipBounds);

                        gs = g.Save();

                        g.ScaleTransform(zoom, zoom);
                        g.TranslateTransform(-origin.X, -origin.Y);

                        DrawRoot.Draw(g, StartingPoint);

                        g.Restore(gs);
                    }
                }
            }

            // After the background image is created, draw it on the screen
            pe.Graphics.DrawImage(buffer, clipBounds.Location);
        }

        #endregion

        #region Mouse Hit methods

        private DrawControlBase? GetHitDrawObject(Point hitPoint) {
            DrawControlBase? result = null;

            // Convert from client coordinate to world coordinates
            PointF p = new((hitPoint.X / zoom) + origin.X, (hitPoint.Y / zoom) + origin.Y);

            if (DrawRoot != null)
                result = DrawRoot.GetDrawObjectContainingPoint(p);

            return result;
        }

        #endregion

        #region Layout event handlers

        [LayoutEvent("model-loaded")]
        [LayoutEvent("new-layout-document")]
        private void ModelLoaded(LayoutEvent e) {
            Recalc();
        }

        [DispatchTarget]
        private void OnEnteredOperationMode(OperationModeParameters settings) {
            Recalc();
        }

        [DispatchTarget]
        private void OnComponentConfigurationChanged(ModelComponent component) {
            Recalc();
        }

        [LayoutEvent("enter-design-mode")]
        [LayoutEvent("control-module-removed")]
        [LayoutEvent("control-module-added")]
        [LayoutEvent("control-module-address-changed")]
        [LayoutEvent("control-module-location-changed")]
        [LayoutEvent("control-bus-reconnected")]
        [LayoutEvent("component-disconnected-from-control-module")]
        [LayoutEvent("component-connected-to-control-module")]
        [LayoutEvent("control-module-label-changed")]
        [LayoutEvent("control-user-action-required-changed")]
        [LayoutEvent("control-address-programming-required-changed")]
        [LayoutEvent("control-buses-added")]
        [LayoutEvent("control-buses-removed")]
        [LayoutEvent("control-module-modified")]
        private void ChangeMode(LayoutEvent e) {
            Recalc();
        }

        [LayoutEvent("component-disconnected-from-control-module")]
        private void ComponentDisconnectedFromControlModule(LayoutEvent e) {
            var connectionPoint = Ensure.NotNull<ControlConnectionPoint>(e.Info);

            if (selectedModule != null && connectionPoint.Module.Id == selectedModule.Id) {
                SelectedModule = null;
                Invalidate();
            }

            if (selectedConnectionPoint != null && selectedConnectionPoint.Module.Id == connectionPoint.Module.Id && selectedConnectionPoint.Index == connectionPoint.Index) {
                SelectedConnectionPoint = null;
                Invalidate();
            }
        }

        [LayoutEvent("layout-control-shown")]
        private void LayoutControlShown(LayoutEvent e) {
            selectedComponents.Display(new LayoutSelectionLook(Color.Red));
        }

        [LayoutEvent("layout-control-hidden")]
        private void LayoutControlHidden(LayoutEvent e) {
            selectedComponents.Hide();
        }

        #endregion

        #region Event handlers

        protected override void OnPaintBackground(PaintEventArgs pevent) {
        }

        private void LayoutControlBusViewer_SizeChanged(object? sender, EventArgs e) {
            // Check if need to update scroll bars
            if (OptionalDrawRoot != null) {
                if (UpdateScrollBars())
                    Invalidate();
            }
        }

        private void HScrollBar_Scroll(object? sender, ScrollEventArgs e) {
            switch (e.Type) {
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

            if (origin.X < hScrollBar.Minimum)
                origin.X = hScrollBar.Minimum;
            else if (origin.X > hScrollBar.Maximum)
                origin.X = hScrollBar.Maximum;
            hScrollBar.Value = origin.X;

            Invalidate();
        }

        private void VScrollBar_Scroll(object? sender, ScrollEventArgs e) {
            switch (e.Type) {
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

            if (origin.Y < 0)
                origin.Y = 0;
            else if (origin.Y > vScrollBar.Maximum)
                origin.Y = vScrollBar.Maximum;

            vScrollBar.Value = (int)origin.Y;
            Invalidate();
        }

        private void LayoutControlBusViewer_DoubleClick(object? sender, EventArgs e) {
            var hitDrawObject = GetHitDrawObject(PointToClient(Control.MousePosition));

            if (hitDrawObject != null)
                hitDrawObject.OnDoubleClick();
        }

        private void LayoutControlBusViewer_MouseDown(object? sender, MouseEventArgs e) {
            var hitDrawObject = GetHitDrawObject(PointToClient(Control.MousePosition));

            mouseDownButton = e.Button;

            if (hitDrawObject != null)
                hitDrawObject.OnMouseDown(e);
        }

        private void LayoutControlBusViewer_MouseMove(object? sender, MouseEventArgs e) {
            var hitDrawObject = GetHitDrawObject(PointToClient(Control.MousePosition));

            if (hitDrawObject != mouseDrawObject) {
                if (mouseDrawObject != null)
                    mouseDrawObject.OnMouseExit();

                if (hitDrawObject != null)
                    hitDrawObject.OnMouseEnter();

                mouseDrawObject = hitDrawObject;
            }
        }

        private void LayoutControlBusViewer_Click(object? sender, EventArgs e) {
            var hitDrawObject = GetHitDrawObject(PointToClient(Control.MousePosition));

            if (hitDrawObject != null)
                hitDrawObject.OnClick(mouseDownButton);
        }

        private void LayoutControlBusViewer_MouseLeave(object? sender, EventArgs e) {
            if (mouseDrawObject != null)
                mouseDrawObject.OnMouseExit();
        }

        protected override bool IsInputKey(Keys keyData) {
            return (keyData & ~Keys.Shift) != Keys.Tab;
        }

        private void LayoutControlBusViewer_KeyDown(object? sender, KeyEventArgs e) {
            switch (e.KeyCode) {
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
                    if ((e.Modifiers & Keys.Control) != 0)
                        DoHorizontalScroll(ScrollEventType.First);
                    else
                        DoVerticalScroll(ScrollEventType.First);
                    break;

                case Keys.End:
                    if ((e.Modifiers & Keys.Control) != 0)
                        DoHorizontalScroll(ScrollEventType.Last);
                    else
                        DoVerticalScroll(ScrollEventType.Last);
                    break;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);

            if (e.Delta > 0) {
                int delta = e.Delta;

                while (delta > 0) {
                    if ((Control.ModifierKeys & (Keys.Shift|Keys.Alt)) != 0) {
                        for (int i = 0; i < 3; i++)
                            DoHorizontalScroll(ScrollEventType.SmallDecrement);
                    }
                    else if ((Control.ModifierKeys & Keys.Control) != 0) {
                        if (Zoom > 0.3)
                            Zoom -= 0.1F;
                    }
                    else {
                        for (int i = 0; i < 6; i++)
                            DoVerticalScroll(ScrollEventType.SmallDecrement);
                    }

                    delta -= 120;
                }
            }
            else if (e.Delta < 0) {
                if ((Control.ModifierKeys & (Keys.Shift|Keys.Alt)) != 0) {
                    for (int i = 0; i < 3; i++)
                        DoHorizontalScroll(ScrollEventType.SmallIncrement);
                }
                else if ((Control.ModifierKeys & Keys.Control) != 0) {
                    if (Zoom < 2.0)
                        Zoom += 0.1F;
                }
                else {
                    for (int i = 0; i < 6; i++)
                        DoVerticalScroll(ScrollEventType.SmallIncrement);
                }
            }
        }

        private void LayoutControlBusViewer_MouseEnter(object? sender, EventArgs e) {
            Focus();
        }

        #endregion
    }

    #region Drawing classes

    public abstract class DrawControlBase {
        private RectangleF bounds = RectangleF.Empty;
        private readonly List<DrawControlBase> drawObjects = new();

        private Cursor? savedCursor = null;

        protected DrawControlBase(LayoutControlBusViewer viewer) {
            this.Viewer = viewer;
        }

        public LayoutControlBusViewer Viewer { get; }

        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Return the bounds of the drawn object. The result is valid only after calling the Draw method
        /// </summary>
        public virtual RectangleF Bounds {
            get {
                RectangleF totalBounds = bounds;

                foreach (DrawControlBase drawObject in drawObjects) {
                    if (drawObject.Bounds != RectangleF.Empty) {
                        if (totalBounds == RectangleF.Empty)
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
            if (bounds == RectangleF.Empty)
                bounds = rect;
            else
                bounds = RectangleF.Union(bounds, rect);
        }

        public void AddDrawObject(DrawControlBase drawObject) {
            drawObjects.Add(drawObject);
        }

        public List<DrawControlBase> DrawObjects => drawObjects;

        public virtual Cursor? Cursor => null;

        public abstract void Draw(Graphics g, PointF startingPoint);

        public DrawControlBase? GetDrawObjectContainingPoint(PointF p) {
            DrawControlBase? hitDrawObject;

            // Check if a sub-object was hit
            foreach (DrawControlBase drawObject in DrawObjects)
                if ((hitDrawObject = drawObject.GetDrawObjectContainingPoint(p)) != null)
                    return hitDrawObject;

            // Check if this object was hit
            return Bounds.Contains(p) ? (this) : null;
        }

        public virtual void OnMouseDown(MouseEventArgs e) {
            if (Enabled) {
                if (e.Button == MouseButtons.Right) {       // Right click
                    string eventName = "add-control-" + (Viewer.IsOperationMode ? "operation" : "editing") + "-context-menu-entries";
                    var menu = new ContextMenuStrip();

                    EventManager.Event(new LayoutEvent(eventName, this, menu).SetFrameWindow(Viewer.FrameWindow));
                    if (menu.Items.Count > 0)
                        menu.Show(Viewer, Viewer.PointToClient(Control.MousePosition));
                }
            }
        }

        public virtual void OnClick(MouseButtons mouseDownButton) {
            if (mouseDownButton == MouseButtons.Left)                                                       // Normal click
                EventManager.Event(new LayoutEvent("control-default-action", this).SetFrameWindow(Viewer.FrameWindow));
        }

        public virtual void OnDoubleClick() {
        }

        public virtual void OnMouseEnter() {
            savedCursor = Cursor.Current;

            var newCursor = this.Cursor;

            if (newCursor is not null)
                Viewer.Cursor = newCursor;
        }

        public virtual void OnMouseExit() {
            if (savedCursor is not null)
                Viewer.Cursor = savedCursor;
        }

        public virtual DrawControlConnectionPoint? FindConnectionPoint(ControlConnectionPointReference connectionPointRef) {
            DrawControlConnectionPoint? result = null;

            foreach (DrawControlBase drawObject in DrawObjects) {
                result = drawObject.FindConnectionPoint(connectionPointRef);
                if (result != null)
                    break;
            }

            return result;
        }

        public virtual DrawControlModule? FindModule(ControlModuleReference moduleRef) {
            DrawControlModule? result = null;

            foreach (DrawControlBase drawObject in DrawObjects) {
                result = drawObject.FindModule(moduleRef);
                if (result != null)
                    break;
            }

            return result;
        }
    }

    public class DrawControlBusProviders : DrawControlBase {
        private const int csGap = 20;

        public DrawControlBusProviders(LayoutControlBusViewer viewer) : base(viewer) {
            if (Viewer.BusProviderId == Guid.Empty) {
                if (LayoutModel.OptionalInstance != null) {
                    foreach (IModelComponentIsBusProvider busProvider in LayoutModel.Components<IModelComponentIsBusProvider>(LayoutModel.ActivePhases)) {
                        DrawControlBusProvider drawBusProvider = new(Viewer, busProvider);

                        AddDrawObject(drawBusProvider);
                    }
                }
            }
            else {
                var busProvider = LayoutModel.Component<IModelComponentIsBusProvider>(Viewer.BusProviderId, LayoutModel.ActivePhases);

                if (busProvider != null) {
                    DrawControlBusProvider drawBusProvider = new(Viewer, busProvider);

                    AddDrawObject(drawBusProvider);
                }
            }
        }

        public override void Draw(Graphics g, PointF startingPoint) {
            PointF csOrigin = startingPoint;

            foreach (DrawControlBusProvider drawBusProvider in DrawObjects) {
                drawBusProvider.Draw(g, csOrigin);
                csOrigin.X += drawBusProvider.Bounds.Width + csGap;
            }
        }
    }

    public class DrawControlBusProvider : DrawControlBase {
        private SizeF minSize = new(60, 28);
        private const int busLineHorizontalGap = 6;
        private const int busLineVerticalGap = 16;
        private const int busGap = 16;

        public DrawControlBusProvider(LayoutControlBusViewer viewer, IModelComponentIsBusProvider busProvider) : base(viewer) {
            IEnumerable<ControlBus> buses = LayoutModel.ControlManager.Buses.Buses(busProvider);

            this.BusProvider = busProvider;

            if (Viewer.BusTypeName == null) {
                foreach (ControlBus bus in buses)
                    AddDrawObject(new DrawControlBus(Viewer, bus));
            }
            else {
                ControlBus? bus = null;

                foreach (ControlBus aBus in buses)
                    if (aBus.BusTypeName == Viewer.BusTypeName) {
                        bus = aBus;
                        break;
                    }

                if (bus != null) {
                    DrawControlBus drawBus = new(Viewer, bus);
                    AddDrawObject(drawBus);
                }
            }
        }

        public IModelComponentIsBusProvider BusProvider { get; }

        public override void Draw(Graphics g, PointF startingPoint) {
            RectangleF busProviderRect;

            using (Font nameFont = new("Arial", 9)) {
                SizeF busProviderRectSize = new(0, 0);
                SizeF nameSize = g.MeasureString(BusProvider.NameProvider.Name, nameFont);

                minSize.Width = Math.Max(minSize.Width, busLineHorizontalGap * (1 + DrawObjects.Count));
                busProviderRectSize.Width = Math.Max(nameSize.Width + 4, minSize.Width);
                busProviderRectSize.Height = Math.Max(nameSize.Height + 4, minSize.Height);

                busProviderRect = new RectangleF(startingPoint, busProviderRectSize);

                g.FillRectangle(Brushes.Khaki, busProviderRect);
                g.DrawRectangle(Pens.Black, busProviderRect.Left, busProviderRect.Top, busProviderRect.Width, busProviderRect.Height);

                StringFormat format = new() {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                g.DrawString(BusProvider.NameProvider.Name, nameFont, Brushes.Black, busProviderRect, format);

                UnionBounds(busProviderRect);
            }

            // Draw buses for each bus provider
            int verticalLength = DrawObjects.Count * busLineVerticalGap;
            PointF busLineStartingPoint = new(busProviderRect.Left + busLineHorizontalGap, busProviderRect.Bottom);
            PointF busStartingPoint = new(busLineStartingPoint.X, busLineStartingPoint.Y + verticalLength);

            int iBus = 0;
            foreach (DrawControlBus drawBus in DrawObjects) {
                PointF corner1 = new(busLineStartingPoint.X, busLineStartingPoint.Y + ((DrawObjects.Count - iBus) * busLineVerticalGap));
                PointF corner2 = new(busStartingPoint.X, busLineStartingPoint.Y + ((DrawObjects.Count - iBus) * busLineVerticalGap));

                g.DrawLines(Viewer.BusPen, new PointF[] { busLineStartingPoint, corner1, corner2, busStartingPoint });
                drawBus.Draw(g, busStartingPoint);

                busLineStartingPoint.X += busLineHorizontalGap;
                busStartingPoint.X += drawBus.Bounds.Width + busGap;
                iBus++;
            }

            UnionBounds(RectangleF.FromLTRB(busProviderRect.Left, busProviderRect.Bottom, busStartingPoint.X, busStartingPoint.Y));
        }
    }

    public class DrawControlBus : DrawControlBase {
        private const int belowNameMargin = 6;
        private const int aboveNameMargin = 4;
        private const int leftNameMargin = 4;

        private const int moduleLeftMargin = 6;
        private const int moduleVerticalGap = 10;

        public DrawControlBus(LayoutControlBusViewer viewer, ControlBus bus) : base(viewer) {
            this.Bus = bus;

            IList<ControlModule> modules = bus.Modules;
            List<ControlModule> modulesByAddress = new();

            if (Viewer.ModuleLocationID == Guid.Empty && !Viewer.ShowOnlyNotInLocation)
                modulesByAddress.AddRange(modules);
            else {
                foreach (ControlModule module in modules)
                    if ((Viewer.ShowOnlyNotInLocation && module.LocationId == Guid.Empty) || module.LocationId == Viewer.ModuleLocationID)
                        modulesByAddress.Add(module);
            }

            modulesByAddress.Sort();

            foreach (ControlModule module in modulesByAddress)
                AddDrawObject(new DrawControlModule(Viewer, module));

            if (!Viewer.IsOperationMode && bus.BusType.Topology != ControlBusTopology.Fixed) {
                DrawControlClickToAddModule d = new(Viewer, bus);

                AddDrawObject(d);
            }
        }

        [Flags]
        private enum DrawControlBusLineFlags {
            Default = 0,
            LastComponent = 0x00000001,
            BrokenBus = 0x00000002,     // Show the bus as broken (daisy chain bus only)
        }

        public ControlBus Bus { get; }

        public override void Draw(Graphics g, PointF startingPoint) {
            PointF busStartPoint;

            using (Font busNameFont = new("Arial", 8)) {
                SizeF busNameSize = g.MeasureString(Bus.BusType.Name, busNameFont);

                busStartPoint = new PointF(startingPoint.X, startingPoint.Y + busNameSize.Height + aboveNameMargin + belowNameMargin);

                g.DrawLine(Viewer.BusPen, startingPoint, busStartPoint);

                StringFormat format = new() {
                    LineAlignment = StringAlignment.Center
                };

                PointF busNamePoint = new(startingPoint.X + leftNameMargin, startingPoint.Y + aboveNameMargin);

                g.DrawString(Bus.BusType.Name, busNameFont, Brushes.Black, busNamePoint);
                UnionBounds(new RectangleF(busNamePoint, busNameSize));
            }

            bool daisyChainBus = Bus.BusType.Topology == ControlBusTopology.DaisyChain;
            int expectedAddress = Bus.BusType.FirstAddress;

            // Draw the modules on the bus
            if (DrawObjects.Count > 0) {
                DrawControlBase lastModule = DrawObjects[^1];

                foreach (DrawControlBase drawObject in DrawObjects) {
                    DrawControlBusLineFlags flags = DrawControlBusLineFlags.Default;

                    if (drawObject is DrawControlModule drawModule && daisyChainBus) {
                        if (drawModule.Module.Address != expectedAddress)
                            flags |= DrawControlBusLineFlags.BrokenBus;

                        expectedAddress = drawModule.Module.Address + drawModule.Module.ModuleType.NumberOfAddresses;
                    }

                    if (drawObject == lastModule)
                        flags |= DrawControlBusLineFlags.LastComponent;

                    drawObject.Draw(g, new PointF(busStartPoint.X + moduleLeftMargin, busStartPoint.Y + moduleVerticalGap));
                    busStartPoint = DrawBusLine(g, busStartPoint, drawObject, flags);
                }
            }
        }

        private PointF DrawBusLine(Graphics g, PointF busStartPoint, DrawControlBase moduleDrawObject, DrawControlBusLineFlags flags) {
            PointF busEndPoint;

            if (Bus.BusType.Topology == ControlBusTopology.DaisyChain) {
                float daisyChainVerticalMargin = moduleDrawObject.Bounds.Height / 4;
                float y = moduleDrawObject.Bounds.Top + daisyChainVerticalMargin;

                busEndPoint = new PointF(busStartPoint.X, y);

                if ((flags & DrawControlBusLineFlags.BrokenBus) != 0) {
                    float y1 = (busStartPoint.Y + busEndPoint.Y) / 2;
                    PointF p1 = new(busStartPoint.X, y1 - 2);
                    PointF p2 = new(busStartPoint.X, y1 + 2);

                    g.DrawLine(Viewer.BusPen, busStartPoint, p1);
                    g.DrawLine(Viewer.BusPen, p2, busEndPoint);
                    g.DrawLine(Pens.Black, new PointF(p1.X - 4, p1.Y - 2), new PointF(p1.X + 4, p1.Y + 2));
                    g.DrawLine(Pens.Black, new PointF(p2.X - 4, p2.Y - 2), new PointF(p2.X + 4, p2.Y + 2));
                }
                else
                    g.DrawLine(Viewer.BusPen, busStartPoint, busEndPoint);

                g.DrawLine(Viewer.BusPen, busEndPoint, new PointF(moduleDrawObject.Bounds.Left, busEndPoint.Y));

                if ((flags & DrawControlBusLineFlags.LastComponent) == 0) {         // It is not last component
                    busStartPoint = new PointF(busStartPoint.X, moduleDrawObject.Bounds.Bottom - daisyChainVerticalMargin);
                    busEndPoint = new PointF(busStartPoint.X, moduleDrawObject.Bounds.Bottom);

                    g.DrawLines(Viewer.BusPen,
                        new PointF[] { new PointF(moduleDrawObject.Bounds.Left, busStartPoint.Y), busStartPoint, busEndPoint });
                }
            }
            else {
                float y = moduleDrawObject.Bounds.Top + (moduleDrawObject.Bounds.Height / 2);

                if ((flags & DrawControlBusLineFlags.LastComponent) != 0)
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
        private RectangleF moduleRect = RectangleF.Empty;
        private RectangleF phaseTextRect = RectangleF.Empty;
        private RectangleF moduleTextRect = RectangleF.Empty;
        private string moduleText;

        private SizeF minModuleSize = new(80, 50);        // Minimal module size
        private const int verticalModuleNameMargin = 24;
        private const int horizontalModuleNameMargin = 8;
        private const int connectionPointMargin = 10;
        private const int connectionPointGap = 10;
        private readonly DrawControlConnectionPoint[] topRow;
        private readonly DrawControlConnectionPoint[] bottomRow;

        public DrawControlModule(LayoutControlBusViewer viewer, ControlModule module) : base(viewer) {
            this.Module = module;

            moduleText = "";

            for (int index = 0; index < module.NumberOfConnectionPoints; index++)
                AddDrawObject(new DrawControlConnectionPoint(Viewer, this, index));

            if ((module.ModuleType.ConnectionPointArrangement & ControlModuleConnectionPointArrangementOptions.TopRow) != 0) {
                int b = ((module.ModuleType.ConnectionPointArrangement & ControlModuleConnectionPointArrangementOptions.StartOnBottom) == ControlModuleConnectionPointArrangementOptions.StartOnBottom) ? ConnectionPointsPerRow : 0;
                int t = 0;

                topRow = new DrawControlConnectionPoint[ConnectionPointsPerRow];

                if ((module.ModuleType.ConnectionPointArrangement & ControlModuleConnectionPointArrangementOptions.TopRightToLeft) == ControlModuleConnectionPointArrangementOptions.TopRightToLeft) {
                    for (int i = b + ConnectionPointsPerRow - 1; i >= b; i--) {
                        topRow[t] = (DrawControlConnectionPoint)DrawObjects[i];
                        topRow[t++].TopRow = true;
                    }
                }
                else {
                    for (int i = b; i < b + ConnectionPointsPerRow; i++) {
                        topRow[t] = (DrawControlConnectionPoint)DrawObjects[i];
                        topRow[t++].TopRow = true;
                    }
                }
            }
            else
                topRow = Array.Empty<DrawControlConnectionPoint>();

            if ((module.ModuleType.ConnectionPointArrangement & ControlModuleConnectionPointArrangementOptions.BottomRow) != 0) {
                int b = ((module.ModuleType.ConnectionPointArrangement & ControlModuleConnectionPointArrangementOptions.StartOnBottom) == ControlModuleConnectionPointArrangementOptions.StartOnBottom) ? 0 : ConnectionPointsPerRow;
                int t = 0;

                bottomRow = new DrawControlConnectionPoint[ConnectionPointsPerRow];

                if ((module.ModuleType.ConnectionPointArrangement & ControlModuleConnectionPointArrangementOptions.BottomRightToLeft) == ControlModuleConnectionPointArrangementOptions.BottomRightToLeft) {
                    for (int i = b + ConnectionPointsPerRow - 1; i >= b; i--) {
                        bottomRow[t] = (DrawControlConnectionPoint)DrawObjects[i];
                        bottomRow[t++].TopRow = false;
                    }
                }
                else {
                    for (int i = b; i < b + ConnectionPointsPerRow; i++) {
                        bottomRow[t] = (DrawControlConnectionPoint)DrawObjects[i];
                        bottomRow[t++].TopRow = false;
                    }
                }
            }
            else
                bottomRow = Array.Empty<DrawControlConnectionPoint>();
        }

        public ControlModule Module { get; }

        public bool Selected {
            get => Viewer.SelectedModule != null &&Viewer.SelectedModule.Id == Module.Id;
            set => Viewer.SelectedModule =value ? Module : null;
        }

        private void DrawConnectionPoints(Graphics g, DrawControlConnectionPoint[] drawObjects, PointF cpStartingPoint, bool topRow) {
            StringFormat format = new() {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            using Font labelFont = new("Arial", 7.5F);
            foreach (DrawControlConnectionPoint drawObject in drawObjects) {
                drawObject.Draw(g, cpStartingPoint);

                string label = Module.ConnectionPoints.GetLabel(drawObject.Index);
                SizeF labelSize = g.MeasureString(label, labelFont);

                RectangleF labelRect = new(new PointF(drawObject.Bounds.Left + ((drawObject.Bounds.Width - labelSize.Width) / 2),
                    topRow ? drawObject.Bounds.Bottom : drawObject.Bounds.Top - labelSize.Height), labelSize);

                g.DrawString(label, labelFont, Brushes.Gray, labelRect, format);

                cpStartingPoint.X += drawObject.Bounds.Width + connectionPointGap;
                UnionBounds(drawObject.Bounds);
            }
        }

        public override void Draw(Graphics g, PointF startingPoint) {
            if (moduleRect == RectangleF.Empty)
                CalcModuleRect(g, startingPoint);

            StringFormat format = new() {
                Alignment = StringAlignment.Near
            };

            if (Selected)
                g.FillRectangle(Brushes.White, moduleRect.Left, moduleRect.Top, moduleRect.Width, moduleRect.Height);

            g.DrawRectangle(Pens.Black, moduleRect.Left, moduleRect.Top, moduleRect.Width, moduleRect.Height);

            if (!Viewer.IsOperationMode && Module.UserActionRequired) {
                RectangleF actionRect = new(moduleRect.Location, new SizeF(4, moduleRect.Height));

                g.FillRectangle(Brushes.Red, actionRect.Left, actionRect.Top, actionRect.Width, actionRect.Height);
            }

            if (!Viewer.IsOperationMode && Module.AddressProgrammingRequired) {
                RectangleF rect = new(new PointF(moduleRect.Location.X + 4, moduleRect.Location.Y), new SizeF(4, moduleRect.Height));

                g.FillRectangle(Brushes.DarkBlue, rect.Left, rect.Top, rect.Width, rect.Height);
            }

            string modulePhaseText = GetModulePhaseText();

            if (modulePhaseText != null) {
                using var font = GetModulePhaseFont();
                g.DrawString(modulePhaseText, font, Brushes.Gray, phaseTextRect, format);
            }

            using (var font = GetModuleNameFont())
                g.DrawString(moduleText, font, Brushes.Black, moduleTextRect, format);

            UnionBounds(moduleRect);

            // Draw top row connection points
            if (topRow != null)
                DrawConnectionPoints(g, topRow, new PointF(moduleRect.Left + connectionPointMargin, moduleRect.Top), true);

            if (bottomRow != null)
                DrawConnectionPoints(g, bottomRow, new PointF(moduleRect.Left + connectionPointMargin, moduleRect.Bottom), false);
        }

        private Font GetModuleNameFont() => new("Arial", 8, FontStyle.Bold);

        private Font GetModulePhaseFont() => new("Arial", 8, FontStyle.Regular);

        private int ConnectionPointsPerRow {
            get {
                int connectionPointsPerRow = Module.NumberOfConnectionPoints;

                if ((Module.ModuleType.ConnectionPointArrangement & ControlModuleConnectionPointArrangementOptions.BothRows) == ControlModuleConnectionPointArrangementOptions.BothRows)
                    connectionPointsPerRow /= 2;

                return connectionPointsPerRow;
            }
        }

        private string GetModulePhaseText() => Module.Phase switch
        {
            LayoutPhase.Construction => "(In construction)",
            LayoutPhase.Planned => "(Planned)",
            LayoutPhase.Operational => "",
            _ => throw new ArgumentException("Invalid module phase"),
        };

        private void CalcModuleRect(Graphics g, PointF startingPoint) {
            SizeF moduleNameTextSize;

            startingPoint.Y += DrawControlConnectionPoint.Measure().Height;

            string phaseText = GetModulePhaseText();
            SizeF modulePhaseTextSize = new Size(0, 0);

            if (phaseText != null) {
                using Font modulePhaseFont = GetModulePhaseFont();
                modulePhaseTextSize = g.MeasureString(phaseText, modulePhaseFont);
            }

            moduleText = GetModuleText();

            using (Font moduleNameFont = GetModuleNameFont())
                moduleNameTextSize = g.MeasureString(moduleText, moduleNameFont);

            SizeF moduleSize = new(0, 0);
            SizeF textSize = new(Math.Max(modulePhaseTextSize.Width, moduleNameTextSize.Width), modulePhaseTextSize.Height + moduleNameTextSize.Height);

            moduleSize.Height = (2 * verticalModuleNameMargin) + textSize.Height;
            moduleSize.Width = (2 * horizontalModuleNameMargin) + textSize.Width;

            int connectionPointsPerRow = ConnectionPointsPerRow;
            float connectionPointRowWidth = (2 * connectionPointMargin) + ((connectionPointsPerRow - 1) * connectionPointGap) + (connectionPointsPerRow * DrawControlConnectionPoint.Measure().Width);

            moduleSize.Width = Math.Max(moduleSize.Width, minModuleSize.Width);
            moduleSize.Width = Math.Max(moduleSize.Width, connectionPointRowWidth);
            moduleSize.Height = Math.Max(moduleSize.Height, minModuleSize.Height);

            moduleRect = new RectangleF(startingPoint, moduleSize);
            phaseTextRect = new RectangleF(new PointF(startingPoint.X + horizontalModuleNameMargin, startingPoint.Y + ((moduleSize.Height - textSize.Height) / 2)), modulePhaseTextSize);
            moduleTextRect = new RectangleF(new PointF(startingPoint.X + horizontalModuleNameMargin, phaseTextRect.Bottom), textSize);
        }

        private string GetModuleText() {
            string addressText;

            if (Module.ModuleType.NumberOfAddresses == 1)
                addressText = Module.Address.ToString();
            else {
                int lastAddress = Module.Address + Module.ModuleType.NumberOfAddresses - 1;

                addressText = Module.Address.ToString() + " - " + lastAddress.ToString();
            }

            var text = (string?)EventManager.Event(new LayoutEvent("get-control-module-description", Module).SetOption("AddressText", addressText).SetOption("ModuleTypeName", Module.ModuleTypeName));
            return text ?? Module.ModuleType.Name + " (" + addressText + ")" + (Module.Label != null ? "\n" + Module.Label : "");
        }

        public override DrawControlModule? FindModule(ControlModuleReference moduleRef) {
            return Module.Id == moduleRef.ModuleId ? (this) : null;
        }
    }

    public class DrawControlClickToAddModule : DrawControlBase {
        private readonly string text = "";

        private const int horizontalMargin = 4;
        private const int verticalMargin = 3;

        public DrawControlClickToAddModule(LayoutControlBusViewer viewer, ControlBus bus) : base(viewer) {
            bool busFull = true;
            int address = bus.BusType.FirstAddress;

            this.Bus = bus;

            while (address <= bus.BusType.LastAddress) {
                var module = bus.GetModuleUsingAddress(address);

                if (module == null) {
                    busFull = false;
                    break;
                }
                else
                    address += module.ModuleType.NumberOfAddresses;
            }

            Enabled = true;

            if (busFull) {
                text = "Bus is full";
                Enabled = false;
            }
            else {
                IList<ControlModuleType> moduleTypes = bus.BusType.ModuleTypes;

                if (moduleTypes.Count == 1)
                    text = "Click here to Add " + moduleTypes[0].Name;
                else if (moduleTypes.Count == 0) {
                    text = "No modules can be added";
                    Enabled = false;
                }
                else
                    text = "Click here to add module";
            }
        }

        public ControlBus Bus { get; }

        public override Cursor Cursor => Cursors.Hand;

        public override void Draw(Graphics g, PointF startingPoint) {
            Font textFont;
            Pen framePen;
            Brush textBrush;

            if (Enabled) {
                textFont = new Font("Arial", 7.5F, FontStyle.Underline);
                framePen = new Pen(Color.Black) {
                    DashStyle = DashStyle.Dash
                };
                textBrush = Brushes.Blue;
            }
            else {
                textFont = new Font("Arial", 7.5F);
                framePen = new Pen(Color.Gray);
                textBrush = Brushes.Gray;
            }

            using (textFont) {
                using (framePen) {
                    SizeF textSize = g.MeasureString(text, textFont);
                    RectangleF r = new(startingPoint, new SizeF(textSize.Width + (2 * horizontalMargin), textSize.Height + (2 * verticalMargin)));
                    StringFormat format = new() {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    g.DrawRectangle(framePen, r.Left, r.Top, r.Width, r.Height);
                    g.DrawString(text, textFont, textBrush, r, format);

                    UnionBounds(r);
                }
            }
        }
    }

    public class DrawControlConnectionPoint : DrawControlBase {
        private bool topRow = false;
        private Image? image = null;

        private const int connectionPointWidth = 20;
        private const int connectionPointShapeHeight = 20;
        private const int connectionPointStatusHeight = 5;
        private const int connectionPointHeight = connectionPointShapeHeight + connectionPointStatusHeight - 1;
        private const int connectionPointOffset = 10;           // Offset into module

        public DrawControlConnectionPoint(LayoutControlBusViewer viewer, DrawControlModule drawModule, int index) : base(viewer) {
            this.DrawModule = drawModule;
            this.Index = index;
        }

        public ControlModule Module => DrawModule.Module;

        public DrawControlModule DrawModule { get; }

        public int Index { get; }

        public bool Selected {
            get => Viewer.SelectedConnectionPoint != null &&
                    Viewer.SelectedConnectionPoint.Module.Id == Module.Id &&
                    Viewer.SelectedConnectionPoint.Index == Index;

            set => Viewer.SelectedConnectionPoint = value ? Module.ConnectionPoints[Index] : null;
        }

        private bool DrawSelection => Selected || (DrawModule.Selected && Module.ConnectionPoints.IsConnected(Index));

        public bool TopRow {
            get {
                return topRow;
            }

            set {
                topRow = value;

                if (Module.ConnectionPoints.IsConnected(Index))
                    image = Viewer.GetConnectionPointImage(Module.ConnectionPoints[Index], topRow);
                else
                    image = Viewer.GetConnectionPointImage(Module.ConnectionPoints.GetConnectionPointType(Index), topRow);
            }
        }

        public ControlConnectionPoint ConnectionPoint => Module.ConnectionPoints[Index];

        public static SizeF Measure() => new(connectionPointWidth, connectionPointHeight);

        public string FullDescription => Module.ModuleType.Name + " address " + Module.ConnectionPoints.GetLabel(Index);

        public override void Draw(Graphics g, PointF startingPoint) {
            RectangleF shapeRect;
            RectangleF statusRect;

            if (topRow) {
                shapeRect = new RectangleF(
                    new PointF(startingPoint.X, startingPoint.Y - connectionPointHeight + connectionPointOffset), new SizeF(connectionPointWidth, connectionPointShapeHeight));
                statusRect = new RectangleF(new PointF(startingPoint.X, shapeRect.Top - connectionPointStatusHeight + 1), new SizeF(shapeRect.Width, connectionPointStatusHeight));
            }
            else {
                shapeRect = new RectangleF(new PointF(startingPoint.X, startingPoint.Y - connectionPointOffset + 2), new SizeF(connectionPointWidth, connectionPointShapeHeight));
                statusRect = new RectangleF(new PointF(startingPoint.X, shapeRect.Bottom), new SizeF(shapeRect.Width, connectionPointStatusHeight));
            }

            Color backColor = DrawSelection ? Color.White : Viewer.BackColor;

            using (Brush backBrush = new SolidBrush(backColor)) {
                g.FillRectangle(backBrush, shapeRect);
                g.DrawRectangle(Pens.Black, shapeRect.Left, shapeRect.Top, shapeRect.Width, shapeRect.Height);

                if (!Viewer.IsOperationMode && DrawModule.Module.ConnectionPoints.IsUserActionRequired(Index))
                    g.FillRectangle(Brushes.Red, statusRect);
                else
                    g.FillRectangle(backBrush, statusRect);

                g.DrawRectangle(Pens.Black, statusRect.Left, statusRect.Top, statusRect.Width, statusRect.Height);

                if (DrawSelection)
                    g.DrawRectangle(Pens.Red, shapeRect.Left + 1, shapeRect.Top + 1, shapeRect.Width - 2, shapeRect.Height - 2);

                if (image != null)
                    g.DrawImage(image, shapeRect.Left + 2, shapeRect.Top + 2);
            }

            UnionBounds(shapeRect);
            UnionBounds(statusRect);
        }

        public override Cursor? Cursor {
            get {
                var connectionDestination = (ControlConnectionPointDestination?)EventManager.Event(new LayoutEvent("get-component-to-control-connect", this));

                return connectionDestination != null && Module.ConnectionPoints.CanBeConnected(connectionDestination, Index)
                    ? Cursors.Cross
                    : base.Cursor;
            }
        }

        public override DrawControlConnectionPoint? FindConnectionPoint(ControlConnectionPointReference connectionPointRef) {
            return connectionPointRef.Module.Id == DrawModule.Module.Id && connectionPointRef.Index == Index ? (this) : null;
        }
    }

    #endregion
}
