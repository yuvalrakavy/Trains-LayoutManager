
using LayoutManager;
using LayoutManager.Model;

#pragma warning disable IDE0051, IDE0060
#nullable enable
namespace DiMAX {

    [LayoutModule("DiMAX Bus Definitions")]
    class DiMAXControlComponents : LayoutModuleBase {

        ControlBusType? dccBus = null;
        ControlBusType? dimaxBus = null;
        ControlBusType? locoBus = null;

        [LayoutEvent("get-control-bus-type", IfEvent = "LayoutEvent[./Options/@BusTypeName='DiMAXDCC']")]
        private void getDCCbusType(LayoutEvent e) {
            if (dccBus == null) {
                dccBus = new ControlBusType {
                    BusFamilyName = "DCC",
                    BusTypeName = "DiMAXDCC",
                    Name = "Tracks (DCC)",
                    Topology = ControlBusTopology.RandomAddressing,
                    FirstAddress = 1,
                    LastAddress = 2048,
                    Usage = ControlConnectionPointUsage.Output
                };
            }

            e.Info = dccBus;
        }

        [LayoutEvent("get-control-bus-type", IfEvent = "LayoutEvent[./Options/@BusTypeName='DiMAXBUS']")]
        private void getDIMAXBUSbusType(LayoutEvent e) {
            if (dimaxBus == null) {
                dimaxBus = new ControlBusType {
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

            e.Info = dimaxBus;
        }

        [LayoutEvent("get-control-bus-type", IfEvent = "LayoutEvent[./Options/@BusTypeName='DiMAXLocoBus']")]
        private void getDIMAXLocoBusType(LayoutEvent e) {
            if (locoBus == null) {
                locoBus = new ControlBusType {
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

            e.Info = locoBus;
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
