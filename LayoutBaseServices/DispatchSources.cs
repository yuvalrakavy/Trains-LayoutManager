using MethodDispatcher;
using System;
using System.Xml;
using System.Collections.Generic;

namespace LayoutManager {
    public static class BaseServicesDispatchSources {
        public static void Initialize() { }

        [DispatchSource]
        public static void AddDispatcherFilters(this Dispatcher d) {
            d[nameof(AddDispatcherFilters)].CallVoid();
        }

        [DispatchSource]
        public static string? GetEventScriptDescription(this Dispatcher d, XmlElement element) {
            return d[nameof(GetEventScriptDescription)].CallNullable<string>(element);
        }

        [DispatchSource]
        public static void OnEventScriptReset(this Dispatcher d, LayoutEventScript script, LayoutEventScriptTask task) {
            d[nameof(OnEventScriptReset)].CallVoid(script, task);
        }

        [DispatchSource]
        public static void OnEventScriptDispose(this Dispatcher d, LayoutEventScript script) {
            d[nameof(OnEventScriptDispose)].CallVoid(script);
        }

        [DispatchSource]
        public static void OnEventScriptTerminated(this Dispatcher d, LayoutEventScript script) {
            d[nameof(OnEventScriptTerminated)].CallVoid(script);
        }

        [DispatchSource]
        public static LayoutEventScript? GetActiveScript(this Dispatcher d, Guid scriptId) {
            return d[nameof(GetActiveScript)].Call<LayoutEventScript?>(scriptId);
        }

        static DispatchSource? ds_SetScriptContext = null;
        public static void SetScriptContext(this Dispatcher d, object v, LayoutScriptContext context) {
            ds_SetScriptContext ??= d[nameof(SetScriptContext)];
            ds_SetScriptContext.CallVoid(v, context);
        }

        static DispatchSource? ds_ParseIfTimeElement = null;
        [DispatchSource]
        public static IIfTimeNode[] ParseIfTimeElement(this Dispatcher d, XmlElement element, string constraintName) {
            ds_ParseIfTimeElement ??= d[nameof(ParseIfTimeElement)];
            return ds_ParseIfTimeElement.Call<IIfTimeNode[]>(element, constraintName);
        }

        static DispatchSource? ds_ParseEventScriptDefinition = null;
        [DispatchSource]
        public static LayoutEventScriptNode ParseEventScriptDefinition(this Dispatcher d, LayoutParseEventScript parseEventInfo) {
            ds_ParseEventScriptDefinition ??= d[nameof(ParseEventScriptDefinition)];
            return ds_ParseEventScriptDefinition.Call<LayoutEventScriptNode>(parseEventInfo);
        }

        static DispatchSource? ds_EventScriptError = null;
        [DispatchSource]
        public static void EventScriptError(this Dispatcher d, LayoutEventScript script, LayoutEventScriptErrorInfo errorInfo) {
            ds_EventScriptError ??= d[nameof(EventScriptError)];
            ds_EventScriptError.CallVoid(script, errorInfo);
        }

        static DispatchSource? ds_GetGlobalEventScriptContext = null;
        [DispatchSource]
        public static LayoutScriptContext? GetGlobalEventScriptContext(this Dispatcher d) {
            ds_GetGlobalEventScriptContext ??= d[nameof(GetGlobalEventScriptContext)];
            return ds_GetGlobalEventScriptContext.Call<LayoutScriptContext?>();
        }

        static DispatchSource? ds_GetCurrentDateTimeRequest = null;
        [DispatchSource]
        public static DateTime GetCurrentDateTimeRequest(this Dispatcher d) {
            ds_GetCurrentDateTimeRequest ??= d[nameof(GetCurrentDateTimeRequest)];
            return ds_GetCurrentDateTimeRequest.Call<DateTime>();
        }

        static DispatchSource? ds_AllocateIfTimeNode = null;
        [DispatchSource]
        public static IIfTimeNode AllocateIfTimeNode(this Dispatcher d, XmlElement nodeElement) {
            ds_AllocateIfTimeNode ??= d[nameof(AllocateIfTimeNode)];
            return ds_AllocateIfTimeNode.Call<IIfTimeNode>(nodeElement);
        }
    }
}
