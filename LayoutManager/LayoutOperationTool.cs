using System;
using System.Windows.Forms;
using MethodDispatcher;
using LayoutManager.Model;
using LayoutManager.CommonUI;

//****
// This file is no longer used, its functionality was moved to the new publish/subscribe model
//****

namespace LayoutManager {
    /// <summary>
    /// This tool is used for editing the layout
    /// </summary>
    public class LayoutOperationTool : LayoutTool {
        public LayoutOperationTool() {
            EventManager.AddObjectSubscriptions(this);
        }

        protected override ContextMenuStrip? GetEmptySpotMenu(LayoutModelArea area, LayoutHitTestResult hitTestResult) => null;

        #region Implement properties for returning various context menu related event names

        protected override Func<ModelComponent, object?> QueryDragFunction => (component) => Dispatch.Call.QueryOperationDrag(component);

        protected override Action<ModelComponent, DragEventArgs> QueryDropAction => (component, dragEventArgs) => Dispatch.Call.QueryOperationDrop(component, dragEventArgs);

        protected override Action<ModelComponent, DragEventArgs> DropAction => (component, dragEventArgs) => Dispatch.Call.DoOperationDrop(component, dragEventArgs);

        #endregion

        protected override void DefaultAction(LayoutModelArea area, LayoutHitTestResult hitTestResult) {
            foreach (ModelComponent component in hitTestResult.Selection)
                EventManager.Event(new LayoutEvent("default-action-command", component, hitTestResult));
        }
    }
}
