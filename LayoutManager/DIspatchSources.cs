using MethodDispatcher;

namespace LayoutManager {
    public static class LayoutControllerDispatchSources {
        [DispatchSource]
        public static void CloseAllFrameWindows(this Dispatcher d) {
            d[nameof(CloseAllFrameWindows)].CallVoid();
        }
    }


}