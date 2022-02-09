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
    }
}
