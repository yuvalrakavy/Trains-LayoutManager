using System;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Xml;
using System.Collections;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.UIGadgets;
using LayoutManager.Components;
using LayoutManager.Tools;

//****
// This file is no longer used, its functionality was moved to the new publish/subscribe model
//****

namespace LayoutManager
{
	/// <summary>
	/// This tool is used for editing the layout
	/// </summary>
	public class LayoutOperationTool : LayoutTool
	{
		private System.ComponentModel.IContainer components;

		public LayoutOperationTool() {
			InitializeComponent();
			EventManager.AddObjectSubscriptions(this);
		}

		protected override ContextMenu GetEmptySpotMenu(LayoutModelArea area, LayoutHitTestResult hitTestResult) {
			return null;
		}

		#region Implement properties for returning various context menu related event names

		protected override string ComponentContextMenuAddTopEntriesEventName {
			get {
				return "add-component-operation-context-menu-top-entries";
			}
		}

		protected override string ComponentContextMenuQueryEventName {
			get {
				return "query-component-operation-context-menu";
			}
		}

		protected override string ComponentContextMenuQueryCanRemoveEventName {
			get {
				return null;
			}
		}

		protected override string ComponentContextMenuAddEntriesEventName {
			get {
				return "add-component-operation-context-menu-entries";
			}
		}

		protected override string ComponentContextMenuAddBottomEntriesEventName {
			get {
				return "add-component-operation-context-menu-bottom-entries";
			}
		}

		protected override string ComponentContextMenuAddCommonEntriesEventName {
			get {
				return "add-component-operation-context-menu-common-entries";
			}
		}

		protected override string ComponentContextMenuQueryNameEventName {
			get {
				return "query-component-operation-context-menu-name";
			}
		}

		protected override string ComponentContextMenuAddEmptySpotEntriesEventName {
			get {
				return "add-operation-empty-spot-context-menu-entries";
			}
		}

		protected override string ComponentContextMenuAddSelectionEntriesEventName {
			get {
				return "add-operation-selection-menu-entries";
			}
		}

		protected override string QueryDragEventName {
			get {
				return "query-operation-drag";
			}
		}

		protected override string DragDoneEventName {
			get {
				return "operation-drag-done";
			}
		}

		protected override string QueryDropEventName {
			get {
				return "query-operation-drop";
			}
		}

		protected override string DropEventName {
			get {
				return "do-operation-drop";
			}
		}

		#endregion

		protected override void DefaultAction(LayoutModelArea area, LayoutHitTestResult hitTestResult) {
			foreach(ModelComponent component in hitTestResult.Selection)
				EventManager.Event(new LayoutEvent(component, "default-action-command", null, hitTestResult));
		}

		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
		}
	}
}
