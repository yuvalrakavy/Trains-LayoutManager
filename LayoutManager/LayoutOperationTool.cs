using LayoutManager.Model;
using MethodDispatcher;
using System.Windows.Forms;

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

        protected override void DefaultAction(LayoutModelArea area, LayoutHitTestResult hitTestResult) {
            foreach (ModelComponent component in hitTestResult.Selection)
                Dispatch.Call.DefaultActionCommand(component, hitTestResult);
        }
    }
}
