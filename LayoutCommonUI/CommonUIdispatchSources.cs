
using System.Xml;
using LayoutManager.CommonUI;
using LayoutManager.CommonUI.Controls;
using LayoutManager.Model;
using MethodDispatcher;

namespace LayoutManager {
    public static class CommonUIDispatchSources {
        public static void Initialize() { }

        static DispatchSource? ds_AddControlModuleEditingContextMenuEntries = null;
        [DispatchSource]
        public static void AddControlModuleEditingContextMenuEntries(this Dispatcher d, DrawControlBase module, MenuOrMenuItem menu) {
            ds_AddControlModuleEditingContextMenuEntries ??= d[nameof(AddControlModuleEditingContextMenuEntries)];
            ds_AddControlModuleEditingContextMenuEntries.CallVoid(module, menu);
        }

        static DispatchSource? ds_AddControlModuleOperationContextMenuEntries = null;
        [DispatchSource]
        public static void AddControlModuleOperationContextMenuEntries(this Dispatcher d, DrawControlBase module, MenuOrMenuItem menu) {
            ds_AddControlModuleOperationContextMenuEntries ??= d[nameof(AddControlModuleOperationContextMenuEntries)];
            ds_AddControlModuleOperationContextMenuEntries.CallVoid(module, menu);
        }

        static DispatchSource? ds_ControlModuleDefaultAction = null;
        [DispatchSource]
        public static void ControlModuleDefaultAction(this Dispatcher d, DrawControlBase module) {
            ds_ControlModuleDefaultAction ??= d[nameof(ControlModuleDefaultAction)];
            ds_ControlModuleDefaultAction.CallVoid(module);
        }

        static DispatchSource? ds_QueryTestLayoutObject = null;
        [DispatchSource]
        public static bool QueryTestLayoutObject(this Dispatcher d) {
            ds_QueryTestLayoutObject ??= d[nameof(QueryTestLayoutObject)];
            return ds_QueryTestLayoutObject.CallBoolFunctions(invokeUntil: true, invokeAll: false);
        }

        static DispatchSource? ds_TestLayoutObjectRequest = null;
        [DispatchSource]
        public static void TestLayoutObjectRequest(this Dispatcher d, Guid frameWindowId, object testedObject) {
            ds_TestLayoutObjectRequest ??= d[nameof(TestLayoutObjectRequest)];
            ds_TestLayoutObjectRequest.CallVoid(frameWindowId, testedObject);
        }

        static DispatchSource? ds_GetTestComponentDialog = null;
        [DispatchSource]
        public static Form? GetTestComponentDialog(this Dispatcher d, Guid frameWindowId, object testedObject) {
            ds_GetTestComponentDialog ??= d[nameof(GetTestComponentDialog)];
            return ds_GetTestComponentDialog.CallUntilNotNull<Form>(frameWindowId, testedObject);
        }

        static DispatchSource? ds_AddControlEditingContextMenuEntries = null;
        [DispatchSource]
        public static void AddControlEditingContextMenuEntries(this Dispatcher d, DrawControlBase module) {
            ds_AddControlEditingContextMenuEntries ??= d[nameof(AddControlEditingContextMenuEntries)];
            ds_AddControlEditingContextMenuEntries.CallVoid(module);
        }

        static DispatchSource? ds_AddContextMenuTopEntries = null;
        [DispatchSource]
        public static void AddContextMenuTopEntries(this Dispatcher d, Guid frameWindowId, ModelComponent component, MenuOrMenuItem menu) {
            ds_AddContextMenuTopEntries ??= d[nameof(AddContextMenuTopEntries)];
            ds_AddContextMenuTopEntries.CallVoid(frameWindowId, component, menu);
        }

        static DispatchSource? ds_AddComponentContextMenuEntries = null;
        [DispatchSource]
        public static void AddComponentContextMenuEntries(this Dispatcher d, Guid frameWindowId, ModelComponent component, MenuOrMenuItem menu) {
            ds_AddComponentContextMenuEntries ??= d[nameof(AddComponentContextMenuEntries)];
            ds_AddComponentContextMenuEntries.CallVoid(frameWindowId, component, menu);
        }

        static DispatchSource? ds_AddCommonContextMenuEntries = null;
        [DispatchSource]
        public static void AddCommonContextMenuEntries(this Dispatcher d, LayoutHitTestResult hitTestResult, MenuOrMenuItem menu) {
            ds_AddCommonContextMenuEntries ??= d[nameof(AddCommonContextMenuEntries)];
            ds_AddCommonContextMenuEntries.CallVoid(hitTestResult, menu);
        }

        static DispatchSource? ds_AddComponentInModuleLocationContextMenuEntries = null;
        [DispatchSource]
        public static void AddComponentInModuleLocationContextMenuEntries(this Dispatcher d, Guid frameWindowId, ModelComponent component, Guid moduleLocationId, MenuOrMenuItem menu) {
            ds_AddComponentInModuleLocationContextMenuEntries ??= d[nameof(AddComponentInModuleLocationContextMenuEntries)];
            ds_AddComponentInModuleLocationContextMenuEntries.CallVoid(frameWindowId, component, moduleLocationId, menu);
        }

        static DispatchSource? ds_ToolsMenuOpenRequest = null;
        [DispatchSource]
        public static void ToolsMenuOpenRequest(this Dispatcher d, MenuOrMenuItem menu) {
            ds_ToolsMenuOpenRequest ??= d[nameof(ToolsMenuOpenRequest)];
            ds_ToolsMenuOpenRequest.CallVoid(menu);
        }

        static DispatchSource? ds_AddSelectionMenuEntries = null;
        [DispatchSource]
        public static void AddSelectionMenuEntries(this Dispatcher d, LayoutHitTestResult hitTestResult, MenuOrMenuItem menu) {
            ds_AddSelectionMenuEntries ??= d[nameof(AddSelectionMenuEntries)];
            ds_AddSelectionMenuEntries.CallVoid(hitTestResult, menu);
        }

        static DispatchSource? ds_AddComponentContextEmptySpotEntries = null;
        [DispatchSource]
        public static void AddComponentContextEmptySpotEntries(this Dispatcher d, LayoutHitTestResult hitTestResult, MenuOrMenuItem menu) {
            ds_AddComponentContextEmptySpotEntries ??= d[nameof(AddComponentContextEmptySpotEntries)];
            ds_AddComponentContextEmptySpotEntries.CallVoid(hitTestResult, menu);
        }

        static DispatchSource? ds_IncludeInComponentContextMenu = null;
        [DispatchSource]
        public static bool IncludeInComponentContextMenu(this Dispatcher d, ModelComponent component) {
            ds_IncludeInComponentContextMenu ??= d[nameof(IncludeInComponentContextMenu)];
            return ds_IncludeInComponentContextMenu.CallBoolFunctions(invokeUntil: true, invokeAll: false, component);
        }

        static DispatchSource? ds_GetEventScriptEditorTreeNode = null;
        [DispatchSource]
        public static LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode(this Dispatcher d, XmlElement element) {
            ds_GetEventScriptEditorTreeNode ??= d[nameof(GetEventScriptEditorTreeNode)];
            return ds_GetEventScriptEditorTreeNode.Call<LayoutEventScriptEditorTreeNode>(element);
        }

        static DispatchSource? ds_EditEventScriptElement = null;
        [DispatchSource]
        public static void EditEventScriptElement(this Dispatcher d, XmlElement element, IEventScriptEditorSite site) {
            ds_EditEventScriptElement ??= d[nameof(EditEventScriptElement)];
            ds_EditEventScriptElement.CallVoid(element, site);
        }

    }
}

