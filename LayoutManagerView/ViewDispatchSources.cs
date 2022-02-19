using System;
using System.Windows.Forms;

using MethodDispatcher;
using LayoutManager.Model;
using LayoutManager.View;
using LayoutManager;

namespace LayoutManager {
    public static class ViewDispatchSources {
        public static void Initialize() { }


        static DispatchSource? ds_AddDetailsWindowSections = null;
        [DispatchSource]
        public static void AddDetailsWindowSections(this Dispatcher d, ModelComponent component, PopupWindowContainerSection container) {
            ds_AddDetailsWindowSections ??= d[nameof(AddDetailsWindowSections)];
            ds_AddDetailsWindowSections.CallVoid(component, container);
        }

        static DispatchSource? ds_EditingDefaultActionCommand = null;
        [DispatchSource]
        public static void EditingDefaultActionCommand(this Dispatcher d, ModelComponent component, LayoutHitTestResult hitResults) {
            ds_EditingDefaultActionCommand ??= d[nameof(EditingDefaultActionCommand)];
            ds_EditingDefaultActionCommand.CallVoid(component, hitResults);
        }

        static DispatchSource? ds_DoOperationDrop = null;
        [DispatchSource]
        public static void DoOperationDrop(this Dispatcher d, ModelComponent component, DragEventArgs dragEventArgs) {
            ds_DoOperationDrop ??= d[nameof(DoOperationDrop)];
            ds_DoOperationDrop.CallVoid(component, dragEventArgs);
        }

        static DispatchSource? ds_DoEditingDrop = null;
        [DispatchSource]
        public static void DoEditingDrop(this Dispatcher d, ModelComponent component, DragEventArgs dragEventArgs) {
            ds_DoEditingDrop ??= d[nameof(DoEditingDrop)];
            ds_DoEditingDrop.CallVoid(component, dragEventArgs);
        }

        static DispatchSource? ds_QueryOperationDrop = null;
        [DispatchSource]
        public static void QueryOperationDrop(this Dispatcher d, ModelComponent component, DragEventArgs dragEventArgs) {
            ds_QueryOperationDrop ??= d[nameof(QueryOperationDrop)];
            ds_QueryOperationDrop.CallVoid(component, dragEventArgs);
        }

        static DispatchSource? ds_QueryEditingDrop = null;
        [DispatchSource]
        public static void QueryEditingDrop(this Dispatcher d, ModelComponent component, DragEventArgs dragEventArgs) {
            ds_QueryEditingDrop ??= d[nameof(QueryEditingDrop)];
            ds_QueryEditingDrop.CallVoid(component, dragEventArgs);
        }

        static DispatchSource? ds_QueryOperationDrag = null;
        [DispatchSource]
        public static object? QueryOperationDrag(this Dispatcher d, ModelComponent component) {
            ds_QueryOperationDrag ??= d[nameof(QueryOperationDrag)];
            return ds_QueryOperationDrag.CallUntilNotNull<Object>(component);
        }

        static DispatchSource? ds_QueryEditingDrag = null;
        [DispatchSource]
        public static object? QueryEditingDrag(this Dispatcher d, ModelComponent component) {
            ds_QueryEditingDrag ??= d[nameof(QueryEditingDrag)];
            return ds_QueryEditingDrag.CallUntilNotNull<Object>(component);
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

    }
}