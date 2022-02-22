
using LayoutManager;
using LayoutManager.Model;
using MethodDispatcher;

namespace LayoutLGB {
    [LayoutModule("LGB MTS Bus Definitions")]
    internal class LGBcontrolComponents : LayoutModuleBase {
        private ControlBusType? dccBus = null;
        private ControlBusType? lgbBus = null;

        [DispatchTarget]
        private ControlBusType GetControlBusType_LGBDCC([DispatchFilter] string busTypeName = "LGBDCC") {
            return dccBus ??= new ControlBusType {
                BusFamilyName = "DCC",
                BusTypeName = "LGBDCC",
                Name = "Tracks (DCC)",
                Topology = ControlBusTopology.RandomAddressing,
                FirstAddress = 1,
                LastAddress = 128,
                Usage = ControlConnectionPointUsage.Output
            };
        }

        [DispatchTarget]
        private ControlBusType GetControlBusType_LGBBUS([DispatchFilter] string busTypeName = "LGBBUS") {
            return lgbBus ??= new ControlBusType {
                BusFamilyName = "LGBBUS",
                BusTypeName = "LGBBUS",
                Name = "LGB Bus",
                Topology = ControlBusTopology.RandomAddressing,
                FirstAddress = 1,
                LastAddress = 256,
                RecommendedStartAddress = 129,
                Usage = ControlConnectionPointUsage.Input
            };
        }

        [LayoutEvent("add-bus-connectable-to-module", IfEvent = "LayoutEvent[Options/@GenericBusType='DCC']")]
        private void AddLGBDCCbusToDCCcompatibleModule(LayoutEvent e) {
            var moduleType = Ensure.NotNull<ControlModuleType>(e.Sender);

            moduleType.BusTypeNames.Add("LGBDCC");
        }

        [LayoutEvent("add-bus-connectable-to-module", IfEvent = "LayoutEvent[Options/@GenericBusType='LGBBUS']")]
        private void AddLGBBUStoLGBBUScompatibleModule(LayoutEvent e) {
            var moduleType = Ensure.NotNull<ControlModuleType>(e.Sender);

            moduleType.BusTypeNames.Add("LGBBUS");
        }
    }
}
