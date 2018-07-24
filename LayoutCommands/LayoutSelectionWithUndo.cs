using System.Drawing;
using LayoutManager.Model;

namespace LayoutManager {
    /// <summary>
    /// Represent a selection of layout components. A selection is a collection of components. If a
    /// selection is displayed, then the component in the selection should be highlighted by the view.
    /// 
    /// Commands on this type of selection can be undone.
    /// 
    /// </summary>
    public class LayoutSelectionWithUndo : LayoutSelection
	{
		/// <summary>
		/// Add a component to this selection. The operation can be undone
		/// </summary>
		/// <param name="component">Component to add to the selection</param>
		/// <param name="containerCommand">if the select operation is part of a compund command then
		/// this parameter is the compound command object, otherwise is is null</param>
		public void Add(ModelComponent component, ILayoutCompoundCommand containerCommand) {
			LayoutComponentSelectCommand	selectCommand = new LayoutComponentSelectCommand(this, component, "select");

			if(containerCommand != null)
				containerCommand.Add(selectCommand);
			else
				LayoutController.Do(selectCommand);
		}

		/// <summary>
		/// Add all the components in a given grid location to the selection
		/// </summary>
		/// <param name="ml">The model location to add to the selection</param>
		public void Add(LayoutModelArea area, Point ml, LayoutPhase phase) {
			LayoutModelSpotComponentCollection	spot = area[ml, phase];

			if(spot != null) {
				ILayoutCompoundCommand	selectCommand = null;

				selectCommand = new LayoutCompoundCommand("select");

				foreach(ModelComponent component in spot)
					Add(component, selectCommand);

				LayoutController.Do(selectCommand);
			}
		}

		/// <summary>
		/// Select all components in a given rectangle in the model
		/// </summary>
		/// <param name="area">The area</param>
		/// <param name="p1">One region corner</param>
		/// <param name="p2">Second region corner</param>
		public void Add(LayoutPhase phase, LayoutModelArea area, Point p1, Point p2) {
			ILayoutCompoundCommand selectCommand = new LayoutCompoundCommand("select region");
			// Make sure that p1 is top, left and p2 is bottom right of the rectangle

			if(p1.X > p2.X) {
				int t = p1.X;

				p1.X = p2.X;
				p2.X = t;
			}

			if(p1.Y > p2.Y) {
				int t = p1.Y;

				p1.Y = p2.Y;
				p2.Y = t;
			}

			for(int y = p1.Y; y <= p2.Y; y++) {
				for(int x = p1.X; x <= p2.X; x++) {
					LayoutModelSpotComponentCollection spot = area[new Point(x, y), phase];

					if(spot != null) {
						foreach(ModelComponent component in spot)
							Add(component, selectCommand);
					}
				}
			}

			if(selectCommand.Count > 0)
				LayoutController.Do(selectCommand);
		}

		/// <summary>
		/// Remove a component from the selection
		/// </summary>
		/// <param name="component">The component to remove from the selection</param>
		public void Remove(ModelComponent component, ILayoutCompoundCommand container) {
			LayoutComponentDeselectCommand deselectCommand = new LayoutComponentDeselectCommand(this, component, "unselect");

			if(container != null)
				container.Add(deselectCommand);
			else
				LayoutController.Do(deselectCommand);
		}

		/// <summary>
		/// Remove all the components in a given model location from the selection
		/// </summary>
		/// <param name="ml">Model location</param>
		public void Remove(LayoutPhase phase, LayoutModelArea area, Point ml) {
			LayoutModelSpotComponentCollection	spot = area[ml, phase];

			if(spot != null) {
				ILayoutCompoundCommand	unselectCommand = new LayoutCompoundCommand("unselect");

				foreach(ModelComponent component in spot)
					Remove(component, unselectCommand);

				if(unselectCommand.Count > 0)
					LayoutController.Do(unselectCommand);
			}
		}

		/// <summary>
		/// Remove all components from the selection
		/// </summary>
		public void Clear(ILayoutCompoundCommand container) {
			if(this.Count > 0) {
				ILayoutCompoundCommand	clearCommand = new LayoutCompoundCommand("unselect all");

				foreach(ModelComponent component in this.Components)
					Remove(component, clearCommand);

				if(container != null)
					container.Add(clearCommand);
				else
					LayoutController.Do(clearCommand);
			}
		}
	}
}
