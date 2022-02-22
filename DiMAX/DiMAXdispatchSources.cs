using LayoutManager.Model;
using MethodDispatcher;
using System.Threading.Tasks;

namespace DiMAX {
    public static class DiMAXdispatchSources {

        static DispatchSource? ds_OnDiMAXstatusUpdate = null;
        [DispatchSource]
        public static void OnDiMAXstatusUpdate(this Dispatcher d, IModelComponentHasNameAndId commandStation, DiMAXstatus DiMAXstatus) {
            ds_OnDiMAXstatusUpdate ??= d[nameof(OnDiMAXstatusUpdate)];
            ds_OnDiMAXstatusUpdate.CallVoid(commandStation, DiMAXstatus);
        }

        static DispatchSource? ds_DiMAXtestLocoSpeed = null;
        [DispatchSource]
        public static Task DiMAXtestLocoSpeed(this Dispatcher d, IModelComponentHasNameAndId commandStation, int address, int speed) {
            ds_DiMAXtestLocoSpeed ??= d[nameof(DiMAXtestLocoSpeed)];
            return ds_DiMAXtestLocoSpeed.Call<Task>(commandStation, address, speed);
        }


        static DispatchSource? ds_DiMAXtestLocoSelect = null;
        [DispatchSource]
        public static Task DiMAXtestLocoSelect(this Dispatcher d, IModelComponentHasNameAndId commandStation, int address, bool select, bool active, bool unconditional) {
            ds_DiMAXtestLocoSelect ??= d[nameof(DiMAXtestLocoSelect)];
            return ds_DiMAXtestLocoSelect.Call<Task>(commandStation, address, select, active, unconditional);
        }
    }
}