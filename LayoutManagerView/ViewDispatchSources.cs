using LayoutManager.Model;
using LayoutManager.View;
using MethodDispatcher;
using System;
using System.Windows.Forms;

namespace LayoutManager {
    public static class ViewDispatchSources {
        public static void Initialize() { }

        static DispatchSource? ds_AddDetailsWindowSections = null;
        [DispatchSource]
        public static void AddDetailsWindowSections(this Dispatcher d, ModelComponent component, PopupWindowContainerSection container) {
            ds_AddDetailsWindowSections ??= d[nameof(AddDetailsWindowSections)];
            ds_AddDetailsWindowSections.CallVoid(component, container);
        }


        static DispatchSource? ds_QueryEditingDefaultAction = null;
        [DispatchSource]
        public static bool QueryEditingDefaultAction(this Dispatcher d, ModelComponent component) {
            ds_QueryEditingDefaultAction ??= d[nameof(QueryEditingDefaultAction)];
            return ds_QueryEditingDefaultAction.CallBoolFunctions(invokeUntil: true, invokeAll: false, component);
        }

        static DispatchSource? ds_EditingDefaultActionCommand = null;
        [DispatchSource]
        public static void EditingDefaultActionCommand(this Dispatcher d, ModelComponent component, LayoutHitTestResult hitResults) {
            ds_EditingDefaultActionCommand ??= d[nameof(EditingDefaultActionCommand)];
            ds_EditingDefaultActionCommand.CallVoid(component, hitResults);
        }

        static DispatchSource? ds_DoDrop = null;
        [DispatchSource]
        public static void DoDrop(this Dispatcher d, ModelComponent component, DragEventArgs dragEventArgs) {
            ds_DoDrop ??= d[nameof(DoDrop)];
            ds_DoDrop.CallVoid(component, dragEventArgs);
        }

        static DispatchSource? ds_QueryDrop = null;
        [DispatchSource]
        public static void QueryDrop(this Dispatcher d, ModelComponent component, DragEventArgs dragEventArgs) {
            ds_QueryDrop ??= d[nameof(QueryDrop)];
            ds_QueryDrop.CallVoid(component, dragEventArgs);
        }

        static DispatchSource? ds_QueryDrag = null;
        [DispatchSource]
        public static object? QueryDrag(this Dispatcher d, ModelComponent component) {
            ds_QueryDrag ??= d[nameof(QueryDrag)];
            return ds_QueryDrag.CallUntilNotNull<Object>(component);
        }

        static DispatchSource? ds_OnDragDone = null;
        [DispatchSource]
        public static void OnDragDone(this Dispatcher d, object draggedObject, DragDropEffects dragDropEffects) {
            ds_OnDragDone ??= d[nameof(OnDragDone)];
            ds_OnDragDone.CallVoid(draggedObject, dragDropEffects);
        }

        static DispatchSource? ds_QueryCanRemoveModelComponent = null;
        [DispatchSource]
        public static bool QueryCanRemoveModelComponent(this Dispatcher d, ModelComponent component) {
            ds_QueryCanRemoveModelComponent ??= d[nameof(QueryCanRemoveModelComponent)];
            return ds_QueryCanRemoveModelComponent.CallBoolFunctions(invokeUntil: false, invokeAll: false, component);
        }

        static DispatchSource? ds_GetComponentsImageList = null;
        [DispatchSource]
        public static ImageList GetComponentsImageList(this Dispatcher d) {
            ds_GetComponentsImageList ??= d[nameof(GetComponentsImageList)];
            return ds_GetComponentsImageList.Call<ImageList>();
        }

        static DispatchSource? ds_GetModelComponentDrawingRegions = null;
        [DispatchSource]
        public static void GetModelComponentDrawingRegions(this Dispatcher d, ModelComponent component, LayoutGetDrawingRegions regions) {
            ds_GetModelComponentDrawingRegions ??= d[nameof(GetModelComponentDrawingRegions)];
            ds_GetModelComponentDrawingRegions.CallVoid(component, regions);
        }
    }
}