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
        private Point clientLocation;
        private Point modelLocation;

        internal LayoutHitTestResult(LayoutModelArea area, LayoutView view, Point clientLocation, Point modelLocation, LayoutSelection selection,
          IList<ILayoutDrawingRegion> regions) {
            this.Area = area;
            this.View = view;
            this.clientLocation = clientLocation;
            this.modelLocation = modelLocation;
            this.Selection = selection;
            this.Regions = regions;
        }

        /// <summary>
        /// The area that was "hit"
        /// </summary>
        public LayoutModelArea Area { get; }

        /// <summary>
        /// The view that the hit test was performed on
        /// </summary>
        public LayoutView View { get; }

        /// <summary>
        /// The frame window that the hit test was perform on
        /// </summary>
        public ILayoutFrameWindow FrameWindow => View.FrameWindow;

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
        public LayoutSelection Selection { get; }

        public IList<ILayoutDrawingRegion> Regions { get; }
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

