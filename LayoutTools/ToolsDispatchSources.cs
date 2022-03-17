using System.Collections.Generic;
using System.Xml;
using LayoutManager.CommonUI;
using LayoutManager.Components;
using LayoutManager.Model;
using LayoutManager.Tools;
using MethodDispatcher;
using static LayoutManager.Tools.LayoutControlTools;

namespace LayoutManager {
    public static class ToolsDispatchSources {
        static DispatchSource? ds_RequestComponentToControlConnect = null;
        [DispatchSource]
        public static void RequestComponentToControlConnect(this Dispatcher d, ControlConnectionPointDestination destination) {
            ds_RequestComponentToControlConnect ??= d[nameof(RequestComponentToControlConnect)];
            ds_RequestComponentToControlConnect.CallVoid(destination);

        }

        static DispatchSource? ds_CheckTripPlanDestination = null;
        [DispatchSource]
        public static bool CheckTripPlanDestination(this Dispatcher d, ApplicableTripPlanData applicableTripPlan) {
            ds_CheckTripPlanDestination ??= d[nameof(CheckTripPlanDestination)];
            return ds_CheckTripPlanDestination.Call<bool>(applicableTripPlan);
        }

        static DispatchSource? ds_CancelComponentToControlConnect = null;
        [DispatchSource]
        public static void CancelComponentToControlConnect(this Dispatcher d) {
            ds_CancelComponentToControlConnect ??= d[nameof(CancelComponentToControlConnect)];
            ds_CancelComponentToControlConnect.CallVoid();
        }

        static DispatchSource? ds_RequestControlToComponentConnect = null;
        [DispatchSource]
        public static void RequestControlToComponentConnect(this Dispatcher d, ControlConnectionPointReference connectionPointReference) {
            ds_RequestControlToComponentConnect ??= d[nameof(RequestControlToComponentConnect)];
            ds_RequestControlToComponentConnect.CallVoid(connectionPointReference);
        }

        static DispatchSource? ds_GetControlToComponentConnect = null;
        [DispatchSource]
        public static ControlConnectionPointReference? GetControlToComponentConnect(this Dispatcher d) {
            ds_GetControlToComponentConnect ??= d[nameof(GetControlToComponentConnect)];
            return ds_GetControlToComponentConnect.Call<ControlConnectionPointReference?>();
        }

        static DispatchSource? ds_CancelControlToComponentConnect = null;
        [DispatchSource]
        public static void CancelControlToComponentConnect(this Dispatcher d) {
            ds_CancelControlToComponentConnect ??= d[nameof(CancelControlToComponentConnect)];
            ds_CancelControlToComponentConnect.CallVoid();
        }

        static DispatchSource? ds_RequestControlAutoConnect = null;
        [DispatchSource]
        internal static ControlConnectionPoint? RequestControlAutoConnect(this Dispatcher d, ControlAutoConnectRequest request, LayoutCompoundCommand? command = null) {
            ds_RequestControlAutoConnect ??= d[nameof(RequestControlAutoConnect)];
            return ds_RequestControlAutoConnect.Call<ControlConnectionPoint?>(request, command);
        }

        static DispatchSource? ds_GetNearestControlModuleLocation = null;
        [DispatchSource]
        public static LayoutControlModuleLocationComponent? GetNearestControlModuleLocation(this Dispatcher d, IModelComponentConnectToControl component) {
            ds_GetNearestControlModuleLocation ??= d[nameof(GetNearestControlModuleLocation)];
            return ds_GetNearestControlModuleLocation.Call<LayoutControlModuleLocationComponent?>(component);
        }

        static DispatchSource? ds_ConnectLayoutToControlRequest = null;
        [DispatchSource]
        public static void ConnectLayoutToControlRequest(this Dispatcher d) {
            ds_ConnectLayoutToControlRequest ??= d[nameof(ConnectLayoutToControlRequest)];
            ds_ConnectLayoutToControlRequest.CallVoid();
        }

        static DispatchSource? ds_GetControlModuleProgrammingActions = null;
        [DispatchSource]
        internal static void GetControlModuleProgrammingActions(this Dispatcher d, ControlModule module, IList<ControlProgrammingAction> actions) {
            ds_GetControlModuleProgrammingActions ??= d[nameof(GetControlModuleProgrammingActions)];
            ds_GetControlModuleProgrammingActions.CallVoid(module, actions);
        }

        static DispatchSource? ds_AddPlaceableBlocksMenuEntries = null;
        [DispatchSource]
        public static void AddPlaceableBlocksMenuEntries(this Dispatcher d, XmlElement placedElement, MenuOrMenuItem menu) {
            ds_AddPlaceableBlocksMenuEntries ??= d[nameof(AddPlaceableBlocksMenuEntries)];
            ds_AddPlaceableBlocksMenuEntries.CallVoid(placedElement, menu);
        }

        static DispatchSource? ds_AddLocomotiveProgrammingMenuEntries = null;
        [DispatchSource]
        public static void AddLocomotiveProgrammingMenuEntries(this Dispatcher d, LocomotiveInfo locomotive, MenuOrMenuItem m) {
            ds_AddLocomotiveProgrammingMenuEntries ??= d[nameof(AddLocomotiveProgrammingMenuEntries)];
            ds_AddLocomotiveProgrammingMenuEntries.CallVoid(locomotive, m);
        }

    }
}
