using System.Collections.Generic;
using System.Drawing;

using LayoutManager.Model;
using LayoutManager.View;
using System.Windows.Forms;

namespace LayoutManager {
    /// <summary>
    /// Hold the results of a hit test.
    /// </summary>
    public class LayoutHitTestResult {
        readonly LayoutModelArea area;
        readonly LayoutView view;
        Point clientLocation;
        Point modelLocation;
        readonly LayoutSelection selection;
        readonly IList<ILayoutDrawingRegion> regions;

        internal LayoutHitTestResult(LayoutModelArea area, LayoutView view, Point clientLocation, Point modelLocation, LayoutSelection selection,
          IList<ILayoutDrawingRegion> regions) {
            this.area = area;
            this.view = view;
            this.clientLocation = clientLocation;
            this.modelLocation = modelLocation;
            this.selection = selection;
            this.regions = regions;
        }

        /// <summary>
        /// The area that was "hit"
        /// </summary>
        public LayoutModelArea Area => area;

        /// <summary>
        /// The view that the hit test was performed on
        /// </summary>
        public LayoutView View => view;

        /// <summary>
        /// The frame window that the hit test was perform on
        /// </summary>
        public ILayoutFrameWindow FrameWindow => view.FrameWindow;

        /// <summary>
        /// Client coordinates for the view
        /// </summary>
        public Point ClientLocation => clientLocation;

        /// <summary>
        /// The model grid location that was hit
        /// </summary>
        public Point ModelLocation => modelLocation;

        /// <summary>
        /// Selection containing all the components that were "hit"
        /// </summary>
        public LayoutSelection Selection => selection;

        public IList<ILayoutDrawingRegion> Regions => regions;
    }

    public class LayoutDraggedObject {
        public object DraggedObject;
        public DragDropEffects AllowedEffects;

        public LayoutDraggedObject(object draggedObject, DragDropEffects allowedEffects) {
            this.DraggedObject = draggedObject;
            this.AllowedEffects = allowedEffects;
        }
    }
}

