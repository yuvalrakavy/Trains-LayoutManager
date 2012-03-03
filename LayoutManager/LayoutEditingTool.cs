using System;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.UIGadgets;
using LayoutManager.Components;
using LayoutManager.Tools;

namespace LayoutManager
{
	/// <summary>
	/// This tool is used for editing the layout
	/// </summary>
	public class LayoutEditingTool : LayoutTool
	{
		LayoutStraightTrackComponent	lastTrack = null;
		String							lastCategoryName = null;
		private System.Windows.Forms.ImageList imageListComponentMenu;
		private System.ComponentModel.IContainer components;

		public LayoutEditingTool() {
			InitializeComponent();
			EventManager.AddObjectSubscriptions(this);
		}

		[LayoutEvent("add-editing-empty-spot-context-menu-entries", Order=0)]
		private void addEditingEmptySpotMenuEntries(LayoutEvent e) {
			LayoutHitTestResult	hitTestResult = (LayoutHitTestResult)e.Sender;
			Menu				menu = (Menu)e.Info;

			menu.MenuItems.Add(new MenuItemPasteComponents(hitTestResult));			
		}

		#region Implement properties for returning various context menu related event names

		protected override string ComponentContextMenuAddTopEntriesEventName {
			get {
				return "add-component-editing-context-menu-top-entries";
			}
		}

		protected override string ComponentContextMenuQueryEventName {
			get {
				return "query-component-editing-context-menu";
			}
		}

		protected override string ComponentContextMenuQueryCanRemoveEventName {
			get {
				return "query-can-remove-model-component";
			}
		}

		protected override string ComponentContextMenuAddEntriesEventName {
			get {
				return "add-component-editing-context-menu-entries";
			}
		}

		protected override string ComponentContextMenuAddBottomEntriesEventName {
			get {
				return "add-component-editing-context-menu-bottom-entries";
			}
		}

		protected override string ComponentContextMenuAddCommonEntriesEventName {
			get {
				return "add-component-editing-context-menu-common-entries";
			}
		}

		protected override string ComponentContextMenuQueryNameEventName {
			get {
				return "query-component-editing-context-menu-name";
			}
		}

		protected override string ComponentContextMenuAddEmptySpotEntriesEventName {
			get {
				return "add-editing-empty-spot-context-menu-entries";
			}
		}

		protected override string ComponentContextMenuAddSelectionEntriesEventName {
			get {
				return "add-editing-selection-menu-entries";
			}
		}

		protected override string QueryDragEventName {
			get {
				return "query-editing-drag";
			}
		}

		protected override string DragDoneEventName {
			get {
				return "editing-drag-done";
			}
		}

		protected override string QueryDropEventName {
			get {
				return "query-editing-drop";
			}
		}

		protected override string DropEventName {
			get {
				return "do-editing-drop";
			}
		}


		#endregion

		[LayoutEvent("add-component-editing-context-menu-common-entries", Order=0)]
		void addContextMenuCommonEntries(LayoutEvent e) {
			Menu				menu = (Menu)e.Info;
			LayoutHitTestResult	hitTestResult = (LayoutHitTestResult)e.Sender;

			if(menu.MenuItems.Count > 0)
				menu.MenuItems.Add("-");

			menu.MenuItems.Add(new MenuItemDeleteAllComponents(hitTestResult));
		}

		[LayoutEvent("add-component-editing-context-menu-common-entries", Order=200)]
		private void addClipboardCommonEntries(LayoutEvent e) {
			Menu				menu = (Menu)e.Info;
			LayoutHitTestResult	hitTestResult = (LayoutHitTestResult)e.Sender;

			if(menu.MenuItems.Count > 0)
				menu.MenuItems.Add("-");
			menu.MenuItems.Add(new MenuItemCopyComponent(hitTestResult));
			menu.MenuItems.Add(new MenuItemCutComponent(hitTestResult));
			menu.MenuItems.Add(new MenuItemPasteComponents(hitTestResult));

			var spot = (from component in hitTestResult.Selection.Components select component.Spot).FirstOrDefault();

			if(spot != null) {
				menu.MenuItems.Add("-");
				menu.MenuItems.Add(GetSetPhaseMenuItem((phaneName, phase) => LayoutController.Do(new ChangePhaseCommand(spot, phase))));
			}
		}

		[LayoutEvent("add-editing-selection-menu-entries", Order=0)]
		private void addSelectionCommandsMenuEntries(LayoutEvent e) {
			LayoutHitTestResult	hitTestResult = (LayoutHitTestResult)e.Sender;
			Menu				menu = (Menu)e.Info;

			menu.MenuItems.Add(new MenuItemDeleteSelection());
		}

		[LayoutEvent("add-editing-selection-menu-entries", Order=1000)]
		private void addSelectionClipboardMenuEntries(LayoutEvent e) {
			LayoutHitTestResult	hitTestResult = (LayoutHitTestResult)e.Sender;
			Menu				menu = (Menu)e.Info;

			if(menu.MenuItems.Count > 0)
				menu.MenuItems.Add("-");
			menu.MenuItems.Add(new MenuItemCopySelection(hitTestResult.ModelLocation));
			menu.MenuItems.Add(new MenuItemCutSelection(hitTestResult.ModelLocation));
			menu.MenuItems.Add(new MenuItemPasteComponents(hitTestResult));

			menu.MenuItems.Add("-");
			menu.MenuItems.Add(GetSetPhaseMenuItem((phaseName, phase) =>
			{
				var command = new LayoutCompoundCommand("Set selection phase to " + phaseName);

				foreach(var spot in (from component in LayoutController.UserSelection.Components select component.Spot).Distinct())
					command.Add(new ChangePhaseCommand(spot, phase));

				LayoutController.Do(command);
			}));
		}

		protected override void DefaultAction(LayoutModelArea area, LayoutHitTestResult hitTestResult) {
			// Create the component
			Point ml = hitTestResult.ModelLocation;
			LayoutModelSpotComponentCollection spot = area[ml, LayoutPhase.All];
			LayoutTrackComponent oldTrack = null;
			ModelComponent component = null;
			bool showComponentsMenu = true;

			if(spot != null) {
				IList<ModelComponent>	components = spot.Components;
				ModelComponent			componentWithDefaultEditingAction = null;

				oldTrack = spot.Track;

				foreach(ModelComponent c in components)
					if((bool)EventManager.Event(new LayoutEvent(c, "query-editing-default-action", null, (bool)false).SetFrameWindow(hitTestResult.FrameWindow))) {
						componentWithDefaultEditingAction = c;
						break;
					}

				if(componentWithDefaultEditingAction != null) {
					showComponentsMenu = false;
					EventManager.Event(new LayoutEvent(componentWithDefaultEditingAction, "editing-default-action-command", null, hitTestResult).SetFrameWindow(hitTestResult.FrameWindow));
				}
			}

			if(showComponentsMenu) {
				if(oldTrack == null && (Control.ModifierKeys & Keys.Shift) == 0)
					// There is not track in that spot. Check if using the last track
					// will connect to one of its neighbors
					component = canUseLast(area, ml);

				if(component == null) {
					ImageMenu		m = new ImageMenu();
					XmlDocument		categories = new XmlDocument();

					categories.LoadXml("<ComponentMenuCategories />");

					EventManager.Event(new LayoutEvent(categories.DocumentElement, "get-component-menu-categories",
						null, oldTrack));

					foreach(XmlElement categoryElement in categories.DocumentElement.ChildNodes) {
						EventManager.Event(new LayoutEvent(categoryElement, "get-component-menu-category-items",
							null, oldTrack));

						// If this category contains at least one item, create the category and add the items
						if(categoryElement.ChildNodes.Count > 0)
							m.Categories.Add(new LayoutImageMenuCategory(categoryElement));
					}

					if(lastCategoryName != null) {
						if(lastCategoryName == "Tracks" && oldTrack is LayoutStraightTrackComponent && (Control.ModifierKeys & Keys.Shift) == 0)
							m.InitialCategoryName = "ComposedTracks";
						else
							m.InitialCategoryName = lastCategoryName;
					}

					m.ShiftKeyCategory = "Tracks";

					LayoutImageMenuItem	s = (LayoutImageMenuItem)m.Show(hitTestResult.View, hitTestResult.ClientLocation);

					if(m.SelectedCategory != null)
						lastCategoryName = m.SelectedCategory.Name;

					if(s != null) {
						component = s.CreateComponent(oldTrack);

						if(component is LayoutStraightTrackComponent)
							lastTrack = (LayoutStraightTrackComponent)component;
					}
				}

				if(component != null) {
					bool					placeComponent = true;

					placeComponent = (bool)EventManager.Event(new LayoutEvent(component, "model-component-placement-request",
						"<PlacementInfo AreaID='" +
						XmlConvert.ToString(area.AreaGuid) +
						"' X='" + XmlConvert.ToString(ml.X) + 
						"' Y='" + XmlConvert.ToString(ml.Y) + "' />", true));

					if(placeComponent) {
						LayoutComponentPlacmentCommand	command = new LayoutComponentPlacmentCommand(
							area, ml, component, "add " + component, area.Phase(ml));

						LayoutController.Do(command);
					}
				}
			}
		}

		private MenuItem GetSetPhaseMenuItem(Action<string, LayoutPhase> doThis) {
			var m = new MenuItem("Change phase to");

			m.MenuItems.Add("Operational", (s, ea) => doThis("Operational", LayoutPhase.Operational));
			m.MenuItems.Add("In construction", (s, ea) => doThis("Construction", LayoutPhase.Construction));
			m.MenuItems.Add("Planned", (s, ea) => doThis("Planned", LayoutPhase.Planned));

			return m;
		}

		/// <summary>
		/// Check if it make sense to use the last inserted component
		/// </summary>
		/// <param name="area">The model location</param>
		/// <param name="ml">Grid location</param>
		/// <returns>The component (if the last one could be used) or null</returns>
		private ModelComponent canUseLast(LayoutModelArea area, Point ml) {
			ModelComponent				result = null;

			if(lastTrack != null) {
				foreach(LayoutComponentConnectionPoint c in lastTrack.ConnectionPoints) {
					ILayoutConnectableComponent neighbor = null;
					Point neighborLocation = ml + LayoutTrackComponent.GetConnectionOffset(c);
					LayoutModelSpotComponentCollection neighborSpot = area[neighborLocation, LayoutPhase.All];

					if(neighborSpot != null)
						neighbor = neighborSpot.Track as ILayoutConnectableComponent;

					if(neighbor != null) {
						LayoutComponentConnectionPoint		neighborPoint = LayoutTrackComponent.GetPointConnectingTo(c);
						LayoutComponentConnectionPoint[]	neighborConnectsTo = neighbor.ConnectTo(neighborPoint, LayoutComponentConnectionType.Passage);

						if(neighborConnectsTo != null) {
							result = new LayoutStraightTrackComponent(lastTrack.ConnectionPoints[0], lastTrack.ConnectionPoints[1]);
							break;
						}
					}
				}
			}

			return result;
		}

		[LayoutEvent("change-phase")]
		private void changePhase(LayoutEvent e0) {
			var e = (LayoutEvent<LayoutSelection, LayoutPhase>)e0;
			LayoutSelection selection = e.Sender != null ? e.Sender : LayoutController.UserSelection;
			LayoutPhase phase = e.Info;
			string phaseName = "**UNKNOWN**";

			switch(phase) {
				case LayoutPhase.Planned: phaseName = "Plan"; break;
				case LayoutPhase.Construction: phaseName = "Construction"; break;
				case LayoutPhase.Operational: phaseName = "Operational"; break;
			}

			LayoutCompoundCommand command = null;

			if(selection.Count == 0) {
				if(MessageBox.Show("Set all components to '" + phaseName + "'", "Are you sure?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
					command = new LayoutCompoundCommand("Set all components to '" + phaseName + "'");

					foreach(var area in LayoutModel.Areas) {
						foreach(var spot in area.Grid.Values)
							command.Add(new ChangePhaseCommand(spot, phase));
					}
				}
			}
			else {
				command = new LayoutCompoundCommand("Set selected components to '" + phaseName + "'");

				foreach(var spot in (from component in selection.Components select component.Spot).Distinct())
					command.Add(new ChangePhaseCommand(spot, phase));
			}

			if(command != null)
				LayoutController.Do(command);
		}

		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.imageListComponentMenu = new System.Windows.Forms.ImageList(this.components);
			// 
			// imageListComponentMenu
			// 
			this.imageListComponentMenu.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageListComponentMenu.ImageSize = new System.Drawing.Size(18, 18);
			this.imageListComponentMenu.TransparentColor = System.Drawing.Color.Transparent;

		}
	}

	class LayoutImageMenuCategory : ImageMenuCategory {
		XmlElement			categoryElement;
		
		internal LayoutImageMenuCategory(XmlElement categoryElement) {
			this.categoryElement = categoryElement;

			this.Tooltip = categoryElement.GetAttribute("Tooltip");
			this.Name = categoryElement.GetAttribute("Name");

			foreach(XmlElement itemElement in categoryElement.ChildNodes)
				this.Items.Add(new LayoutImageMenuItem(itemElement));
		}

		protected override void Paint(Graphics g) {
			EventManager.Event(new LayoutEvent(categoryElement, "paint-image-menu-category", null, g));
		}
	}

	class LayoutImageMenuItem : ImageMenuItem {
		XmlElement			itemElement;

		internal LayoutImageMenuItem(XmlElement itemElement) {
			this.itemElement = itemElement;

			this.Tooltip = itemElement.GetAttribute("Tooltip");
		}

		protected override void Paint(Graphics g) {
			EventManager.Event(new LayoutEvent(itemElement, "paint-image-menu-item", null, g));
		}

		public ModelComponent CreateComponent(ModelComponent old) {
			ModelComponent	newComponent = (ModelComponent)EventManager.Event(new LayoutEvent(itemElement, 
				"create-model-component", null, old));

			if(newComponent != old)
				return newComponent;
			return null;
		}
	}
}
