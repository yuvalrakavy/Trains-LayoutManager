using System.Windows.Forms;
using LayoutManager.Model;

//****
// This file is no longer used, its functionality was moved to the new publish/subscribe model
//****

namespace LayoutManager {
    /// <summary>
    /// This tool is used for editing the layout
    /// </summary>
    public class LayoutOperationTool : LayoutTool {
        private System.ComponentModel.IContainer components;

        public LayoutOperationTool() {
            InitializeComponent();
            EventManager.AddObjectSubscriptions(this);
        }

        protected override ContextMenu GetEmptySpotMenu(LayoutModelArea area, LayoutHitTestResult hitTestResult) => null;

        #region Implement properties for returning various context menu related event names

        protected override string ComponentContextMenuAddTopEntriesEventName => "add-component-operation-context-menu-top-entries";

        protected override string ComponentContextMenuQueryEventName => "query-component-operation-context-menu";

        protected override string ComponentContextMenuQueryCanRemoveEventName => null;

        protected override string ComponentContextMenuAddEntriesEventName => "add-component-operation-context-menu-entries";

        protected override string ComponentContextMenuAddBottomEntriesEventName => "add-component-operation-context-menu-bottom-entries";

        protected override string ComponentContextMenuAddCommonEntriesEventName => "add-component-operation-context-menu-common-entries";

        protected override string ComponentContextMenuQueryNameEventName => "query-component-operation-context-menu-name";

        protected override string ComponentContextMenuAddEmptySpotEntriesEventName => "add-operation-empty-spot-context-menu-entries";

        protected override string ComponentContextMenuAddSelectionEntriesEventName => "add-operation-selection-menu-entries";

        protected override string QueryDragEventName => "query-operation-drag";

        protected override string DragDoneEventName => "operation-drag-done";

        protected override string QueryDropEventName => "query-operation-drop";

        protected override string DropEventName => "do-operation-drop";

        #endregion

        protected override void DefaultAction(LayoutModelArea area, LayoutHitTestResult hitTestResult) {
            foreach (ModelComponent component in hitTestResult.Selection)
                EventManager.Event(new LayoutEvent(component, "default-action-command", null, hitTestResult));
        }

        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
        }
    }
}
