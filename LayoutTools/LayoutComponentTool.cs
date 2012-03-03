using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools {

	/// <summary>
	/// Base class for implementing component specific UI behavior.
	/// </summary>
	public abstract class LayoutComponentTool : IObjectFactoryProduct, IDisposable {
		ObjectFactory	factory;		// the factory that created this component view class
		Object			productKey;		// The key that was used to produce this object
		ModelComponent	component;		// The component for which this tool is used

		#region Implementation of IObjectFactoryProduct and IDisposable

		/// <summary>
		/// The component that this class should draw
		/// </summary>
		public Object ProductKey {
			set {
				productKey = value;
			}

			get {
				return productKey;
			}
		}

		public ObjectFactory Factory {
			get {
				return factory;
			}

			set {
				factory = value;
			}
		}

		/// <summary>
		/// Dispose the component tool class. The component tool object is returned to the factory
		/// pool so it can be reused.
		/// </summary>
		public void Dispose() {
			component = null;
			ObjectFactory.ReleaseObject(this);
		}

		#endregion

		/// <summary>
		/// The component for which this tool is used
		/// </summary>
		public ModelComponent Component {
			get {
				return component;
			}

			set {
				component = value;
			}
		}

		/// <summary>
		/// Return true if this component would like to have entries in a context menu
		/// </summary>
		public abstract bool HasContextMenu {
			get;
		}

		/// <summary>
		/// Return the component name as it should appear in context menus
		/// </summary>
		public abstract String ComponentName {
			get;
		}

		/// <summary>
		/// Return true if there should be an option to remove only this component (and not
		/// all component in this location)
		/// </summary>
		public virtual bool CanRemoveComponent {
			get {
				return true;
			}
		}

		/// <summary>
		/// Add context menu items.
		/// </summary>
		/// <remarks>The context entries must be added to the end of the passed context menu</remarks>
		/// <param name="menu">The menu to add items to</param>
		/// <returns>True if items were added</returns>
		public virtual bool AddContextMenuItems(ILayoutController controller, Menu menu) {
			return false;
		}

		/// <summary>
		/// Add items to the main context menu item.
		/// </summary>
		/// <remarks>
		/// The top level context menu items may contain cascading menu for each component (if there are
		/// more than one component in the spot).
		/// </remarks>
		/// <param name="menu"></param>
		/// <returns>True if items were added</returns>
		public virtual bool AddTopLevelContextMenuItems(ILayoutController controller, Menu menu) {
			return false;
		}

		/// <summary>
		/// Called when a component is created. You may show UI for initializing the new
		/// component.
		/// </summary>
		/// <param name="controller">The layout controller</param>
		/// <param name="area">The model area in which the component will be placed</param>
		/// <param name="ml">The location in which the component is to be created</param>
		/// <returns>True if the component should be placed in the area</returns>
		public virtual bool OnCreateComponent(ILayoutController controller, LayoutModelArea area, Point ml) {
			return true;
		}
	}

	/// <summary>
	/// Object factory for component tools. You must call Dispose on a component tool when it
	/// is no longer used
	/// </summary>
	public class LayoutComponentToolFactory : ObjectFactory {

		public LayoutComponentToolFactory() {
			// Adding component type to component tool factory
			Add(typeof(LayoutTrackLinkComponent), typeof(LayoutTrackLinkComponentTool), 2);
			Add(typeof(LayoutTextComponent), typeof(LayoutTextComponentTool), 2);
			Add(typeof(LayoutTrackContactComponent), typeof(LayoutTrackContactComponentTool), 2);
		}

		public LayoutComponentTool GetComponentTool(ModelComponent component) {
			LayoutComponentTool	componentTool = (LayoutComponentTool)ProduceObject(component.GetType());

			if(componentTool != null)
				componentTool.Component = component;

			return componentTool;
		}
	}

}
