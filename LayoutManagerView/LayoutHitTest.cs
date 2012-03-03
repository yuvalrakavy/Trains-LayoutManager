using System;
using System.Collections;
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
		LayoutModelArea					area;
		LayoutView						view;
		Point							clientLocation;
		Point							modelLocation;
		LayoutSelection					selection;
		IList<ILayoutDrawingRegion>		regions;

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
		public LayoutModelArea Area {
			get {
				return area;
			}
		}

		/// <summary>
		/// The view that the hit test was performed on
		/// </summary>
		public LayoutView View {
			get {
				return view;
			}
		}

		/// <summary>
		/// The frame window that the hit test was perform on
		/// </summary>
		public ILayoutFrameWindow FrameWindow {
			get {
				return view.FrameWindow;
			}
		}

		/// <summary>
		/// Client coordinates for the view
		/// </summary>
		public Point ClientLocation {
			get {
				return clientLocation;
			}
		}

		/// <summary>
		/// The model grid location that was hit
		/// </summary>
		public Point ModelLocation {
			get {
				return modelLocation;
			}
		}

		/// <summary>
		/// Selection containing all the components that were "hit"
		/// </summary>
		public LayoutSelection Selection {
			get {
				return selection;
			}
		}

		public IList<ILayoutDrawingRegion> Regions {
			get {
				return regions;
			}
		}
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

