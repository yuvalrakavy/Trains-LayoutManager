using System.Threading.Tasks;
using System.Xml;
using LayoutManager.Components;
using LayoutManager.Logic;
using LayoutManager.Model;
using MethodDispatcher;

namespace LayoutManager {
    public static class LayoutControllerDispatchSources {
        [DispatchSource]
        public static void CloseAllFrameWindows(this Dispatcher d) {
            d[nameof(CloseAllFrameWindows)].CallVoid();
        }
    }


}