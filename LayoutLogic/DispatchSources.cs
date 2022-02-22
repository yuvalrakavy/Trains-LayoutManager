
using MethodDispatcher;

namespace LayoutManager {
    public static class LogicDispatchSources {
        [DispatchSource]
        public static IRoutePlanningServices GetRoutePlanningServices(this Dispatcher d) {
            return d[nameof(GetRoutePlanningServices)].Call<IRoutePlanningServices>();
        }
    }
}


