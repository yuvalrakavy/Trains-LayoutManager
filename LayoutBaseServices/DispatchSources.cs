using System;
using System.Xml;
using MethodDispatcher;

namespace LayoutManager {
    public static class BaseServicesDispatchSources {
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


    }
}
