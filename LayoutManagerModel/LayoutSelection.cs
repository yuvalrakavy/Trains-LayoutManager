using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Xml;
using System.Windows.Forms;
using System.Linq;

using LayoutManager;

namespace LayoutManager.Model {

	/// <summary>
	/// Represent a selection of layout components. A selection is a collection of components. If a
	/// selection is displayed, then the component in the selection should be highlighted by the view
	/// </summary>
	public class LayoutSelection : IEnumerable<ModelComponent> {
		Dictionary<ModelComponent, object> selection = new Dictionary<ModelComponent, object>();
		ILayoutSelectionLook			selectionLook;

		public const int ZOrderTopmost = 0;
		public const int ZOrderBottom = 1;

		public LayoutSelection() {
		}

		public LayoutSelection(IEnumerable<ModelComponent> components) {
			Add(components);
		}

        public int Count => selection.Count;

        /// <summary>
        /// Return a point with the top left component that is in this selection
        /// </summary>
        public Point TopLeftLocation {
			get {
				Point			minLocation = new Point(0, 0);
				bool			first = true;

				foreach(ModelComponent component in this) {
					if(first) {
						minLocation = component.Location;
						first = false;
					}
					else {
						if(component.Location.X < minLocation.X)
							minLocation.X = component.Location.X;
						
						if(component.Location.Y < minLocation.Y)
							minLocation.Y = component.Location.Y;
					}
				}

				return minLocation;
			}
		}

        /// <summary>
        /// An array with all the components in the selection
        /// </summary>
        public IEnumerable<ModelComponent> Components => selection.Keys;

        /// <summary>
        /// Check if the selection contains a given component
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool Contains(ModelComponent component) => selection.ContainsKey(component);

        /// <summary>
        /// Check if this selection contains any component that is part of another selection
        /// </summary>
        /// <param name="selection">The other selection</param>
        /// <returns>True if this selection contains any component which is also part of the other selection</returns>
        public bool ContainsAny(LayoutSelection selection) {
			foreach(ModelComponent component in selection)
				if(Contains(component))
					return true;
			return false;
		}

		/// <summary>
		/// Add a component to this selection
		/// </summary>
		/// <param name="component">Component to add to the selection</param>
		/// <param name="canUndo">If true the operation can be undone</param>
		/// <param name="containerCommand">if the select operation is part of a compund command then
		/// this parameter is the compound command object, otherwise is is null</param>
		public void Add(ModelComponent component) {
			if(!selection.ContainsKey(component)) {
				selection.Add(component, null);
				if(selectionLook != null)
					component.Redraw();
			}
		}

		public void Add(IModelComponent component) {
			Add((ModelComponent)component);
		}

		/// <summary>
		/// Add all components in another selection to this selection
		/// </summary>
		/// <param name="selection">The selection with the components to be added</param>
		public void Add(LayoutSelection selection) {
			foreach(ModelComponent component in selection)
				Add(component);
		}

		public void Add(LayoutBlock block) {
			if(block.BlockDefinintion != null)
				Add(block.BlockDefinintion);
			else {
				foreach(TrackEdge edge in block.TrackEdges)
					if(edge.Track.BlockEdgeBase == null)
						Add(edge.Track);
			}
		}

		public void Add(LayoutOccupancyBlock occupancyBlock) {
			foreach(LayoutBlock block in occupancyBlock.ContainedBlocks)
				Add(block);
		}

		public void Add(TrainStateInfo train) {
			foreach(TrainLocationInfo trainLocation in train.Locations)
				Add(trainLocation.Block);
		}

		public void Add<T>(IEnumerable<T> components) where T : ModelComponent {
			foreach(ModelComponent component in components)
				Add(component);
		}

		public void Add(Guid id) {
			var component = LayoutModel.Component<ModelComponent>(id, LayoutModel.ActivePhases);

			if(component != null)
				Add(component);
			else {
				LayoutBlock	block;

				if(LayoutModel.Blocks.TryGetValue(id, out block))
					Add(block);
				else {
					TrainStateInfo	train = LayoutModel.StateManager.Trains[id];

					if(train != null)
						Add(train);
					else {
						ManualDispatchRegionInfo	manualDispatchRegion = LayoutModel.StateManager.ManualDispatchRegions[id];

						if(manualDispatchRegion != null)
							Add(manualDispatchRegion.Selection);
						else
							Trace.WriteLine("The ID " + id.ToString() + " given as message subject can not be associated with an object");
					}
				}
			}
		}

		/// <summary>
		/// Remove a component from the selection
		/// </summary>
		/// <param name="component">The component to remove from the selection</param>
		public void Remove(ModelComponent component) {
			if(selection.ContainsKey(component)) {
				if(selectionLook != null)
					component.EraseImage();

				selection.Remove(component);

				if(selectionLook != null)
					component.Redraw();
			}
		}

		/// <summary>
		/// Remove all the components in another selection from this selection
		/// </summary>
		/// <param name="selection">The selection with the components to be removed</param>
		public void Remove(LayoutSelection selection) {
			foreach(ModelComponent component in selection)
				Remove(component);
		}

		/// <summary>
		/// Check if any component in a location is in the selection
		/// </summary>
		/// <param name="ml">The location</param>
		/// <returns>True if any component in that location is in the selection</returns>
		public bool IsLocationSelected(LayoutModelArea area, Point ml) {
			foreach(ModelComponent component in selection.Keys)
				if(component.Spot.Area == area && component.Location == ml)
					return true;

			return false;
		}

		/// <summary>
		/// Remove all components from the selection
		/// </summary>
		public void Clear() {
			if(selection.Count > 0) {
				foreach(ModelComponent component in this.Components.ToArray())
					Remove(component);
			}
		}

		/// <summary>
		/// Ask all the components in the selection to redraw.
		/// </summary>
		public void Redraw() {
			foreach(ModelComponent component in this)
				component.Redraw();
		}

		/// <summary>
		/// Ask all the components to erase themself from the screen
		/// </summary>
		public void EraseImage() {
			foreach(ModelComponent component in this)
				component.EraseImage();
		}

		/// <summary>
		/// Cause the selection to be visiable
		/// </summary>
		/// <param name="selectionLook">The selection's visual look</param>
		/// <param name="zOrder">Can be either ZOrderTopMost or ZOrderBottom</param>
		public void Display(ILayoutSelectionLook selectionLook, int zOrder) {
			this.selectionLook = selectionLook;
			LayoutController.SelectionManager.DisplaySelection(this, zOrder);
			this.Redraw();
		}

		public void Display(ILayoutSelectionLook selectionLook) {
			Display(selectionLook, ZOrderTopmost);
		}

        public ILayoutSelectionLook SelectionLook => selectionLook;

        /// <summary>
        /// Hide the selection
        /// </summary>
        public void Hide() {
			this.EraseImage();
			this.selectionLook = null;
			LayoutController.SelectionManager.HideSelection(this);
			this.Redraw();
		}
		
		/// <summary>
		/// If a component is selected, return its selection look
		/// </summary>
		/// <param name="component">The component to check</param>
		/// <returns>
		/// null if component not in selection, 
		/// otherwise an instance of a LayoutSelectionLook object.
		/// </returns>
		public ILayoutSelectionLook GetComponentSelectionLook(ModelComponent component) {
			if(selection.ContainsKey(component))
				return selectionLook;
			return null;
		}

        /// <summary>
        /// Return an enumerator on all the components in the selection
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ModelComponent> GetEnumerator() => selection.Keys.GetEnumerator();

        private void AddComponentsToDataObject(XmlWriter w, Point mlOrigin, bool doTracks) {
			foreach(ModelComponent component in this) {
				if((doTracks && component.Kind == ModelComponentKind.Track) || (!doTracks && component.Kind != ModelComponentKind.Track)) {
					w.WriteStartElement("Component");
					w.WriteAttributeString("Class", component.GetType().FullName);
					w.WriteAttributeString("X", XmlConvert.ToString(component.Location.X - mlOrigin.X));
					w.WriteAttributeString("Y", XmlConvert.ToString(component.Location.Y - mlOrigin.Y));
					component.WriteInnerXml(w);
					w.WriteEndElement();
				}
			}
		}

		/// <summary>
		/// Get data object encupsolating the components in the selection
		/// </summary>
		/// <param name="mlOrigin">Origin in which the component location relates to</param>
		/// <returns>The data object</returns>
		public IDataObject GetDataObject(Point mlOrigin) {
			System.IO.MemoryStream		ms = new System.IO.MemoryStream();
			XmlTextWriter				w = new XmlTextWriter(ms, System.Text.Encoding.UTF8);

			w.WriteStartDocument();
			w.WriteStartElement("Components");

			// First save track components, and then other components. This is done since some components (e.g. block edge, block Info)
			// need to know the track they are overlayed on (for example for correct drawing)
			AddComponentsToDataObject(w, mlOrigin, true);
			AddComponentsToDataObject(w, mlOrigin, false);

			w.WriteEndElement();
			w.WriteEndDocument();
			w.Close();

			DataObject	dataObj = new DataObject();

			dataObj.SetData("LayoutManagerComponents", ms.GetBuffer());
			String	xml = new System.Text.UTF8Encoding().GetString(ms.GetBuffer());

			dataObj.SetData(DataFormats.Text, xml);

			return dataObj;
		}

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }

	/// <summary>
	/// Encupsolate the information on the look of a selection, such as the selection
	/// color etc.
	/// </summary>
	public class LayoutSelectionLook : ILayoutSelectionLook {
		Color	color;

		/// <summary>
		/// Create a selection color
		/// </summary>
		/// <param name="color">The selection visual indicator color</param>
		public LayoutSelectionLook(Color color) {
			this.color = color;
		}

        /// <summary>
        /// The selection visual indicator color
        /// </summary>
        public Color Color => color;
    }


}



