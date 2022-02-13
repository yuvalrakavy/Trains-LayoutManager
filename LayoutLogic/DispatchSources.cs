﻿using System.Threading.Tasks;
using System.Xml;
using LayoutManager.Components;
using LayoutManager.Logic;
using LayoutManager.Model;

using MethodDispatcher;

namespace LayoutManager {
    public static class LogicDispatchSources {
        [DispatchSource]
        public static IRoutePlanningServices GetRoutePlanningServices(this Dispatcher d) {
            return d[nameof(GetRoutePlanningServices)].Call<IRoutePlanningServices>();
        }
    }
}


