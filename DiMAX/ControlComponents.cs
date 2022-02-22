
using LayoutManager;
using LayoutManager.Model;
using MethodDispatcher;

#nullable enable
namespace DiMAX {
    [LayoutModule("DiMAX Bus Definitions")]
    internal class DiMAXControlComponents : LayoutModuleBase {
        private ControlBusType? dccBus = null;
        private ControlBusType? dimaxBus = null;
        private ControlBusType? locoBus = null;

        [DispatchTarget]
        private ControlBusType GetControlBusType_DiMAXDCC([DispatchFilter] string busTypeName = "DiMAXDCC") {
            return dccBus ??= new ControlBusType {
                BusFamilyName = "DCC",
                BusTypeName = "DiMAXDCC",
                Name = "Tracks (DCC)",
                Topology = ControlBusTopology.RandomAddressing,
                FirstAddress = 1,
                LastAddress = 2048,
                Usage = ControlConnectionPointUsage.Output
            };
        }

        [DispatchTarget]
        private ControlBusType GetControlBusType_DiMAXBUS([DispatchFilter] string busTypeName = "DiMAXBUS") {
            return dimaxBus ??= new ControlBusType {
                BusFamilyName = "DiMAXBus",
                BusTypeName = "DiMAXBUS",
                Name = "DiMAX Bus",
                Topology = ControlBusTopology.RandomAddressing,
                FirstAddress = 1,
                LastAddress = 2048,
                RecommendedStartAddress = 1,
                Usage = ControlConnectionPointUsage.Input
            };
        }

        [DispatchTarget]
        private ControlBusType GetControlBusType_DiMAXLocoBus([DispatchFilter] string busTypeName = "DiMAXLocoBus") {
            return locoBus ??= new ControlBusType {
                BusFamilyName = "LocoBus",
                BusTypeName = "DiMAXLocoBus",
                Name = "Function Decoders",
                Topology = ControlBusTopology.RandomAddressing,
                FirstAddress = 1,
                LastAddress = 10239,
                RecommendedStartAddress = 2000,
                Usage = ControlConnectionPointUsage.Output
            };
        }

        [LayoutEvent("add-bus-connectable-to-module", IfEvent = "LayoutEvent[Options/@GenericBusType='DCC']")]
        private void AddDiMAXDCCbusToDCCcompatibleModule(LayoutEvent e) {
            var moduleType = Ensure.NotNull<ControlModuleType>(e.Sender, "moduleType");

            moduleType.BusTypeNames.Add("DiMAXDCC");
        }

        [LayoutEvent("add-bus-connectable-to-module", IfEvent = "LayoutEvent[Options/@GenericBusType='DiMAXBus']")]
        private void AddDiMAXBUStoLGBBUScompatibleModule(LayoutEvent e) {
            var moduleType = Ensure.NotNull<ControlModuleType>(e.Sender, "moduleType");

            moduleType.BusTypeNames.Add("DiMAXBUS");
        }

        [LayoutEvent("add-bus-connectable-to-module", IfEvent = "LayoutEvent[Options/@GenericBusType='LocoBus']")]
        private void AddDiMAXLocoBustoLGBBUScompatibleModule(LayoutEvent e) {
            var moduleType = Ensure.NotNull<ControlModuleType>(e.Sender, "moduleType");

            moduleType.BusTypeNames.Add("DiMAXLocoBus");
        }
    }
}
