
using LayoutManager;
using LayoutManager.Model;

namespace LayoutLGB {
    [LayoutModule("LGB MTS Bus Definitions")]
    internal class LGBcontrolComponents : LayoutModuleBase {
        private ControlBusType dccBus = null;
        private ControlBusType lgbBus = null;

        [LayoutEvent("get-control-bus-type", IfEvent = "LayoutEvent[./Options/@BusTypeName='LGBDCC']")]
        private void getLGBDCCbusType(LayoutEvent e) {
            if (dccBus == null) {
                dccBus = new ControlBusType {
                    BusFamilyName = "DCC",
                    BusTypeName = "LGBDCC",
                    Name = "Tracks (DCC)",
                    Topology = ControlBusTopology.RandomAddressing,
                    FirstAddress = 1,
                    LastAddress = 128,
                    Usage = ControlConnectionPointUsage.Output
                };
            }

            e.Info = dccBus;
        }

        [LayoutEvent("get-control-bus-type", IfEvent = "LayoutEvent[./Options/@BusTypeName='LGBBUS']")]
        private void getLGBBUSbusType(LayoutEvent e) {
            if (lgbBus == null) {
                lgbBus = new ControlBusType {
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

            e.Info = lgbBus;
        }

        [LayoutEvent("add-bus-connectable-to-module", IfEvent = "LayoutEvent[Options/@GenericBusType='DCC']")]
        private void AddLGBDCCbusToDCCcompatibleModule(LayoutEvent e) {
            ControlModuleType moduleType = (ControlModuleType)e.Sender;

            moduleType.BusTypeNames.Add("LGBDCC");
        }

        [LayoutEvent("add-bus-connectable-to-module", IfEvent = "LayoutEvent[Options/@GenericBusType='LGBBUS']")]
        private void AddLGBBUStoLGBBUScompatibleModule(LayoutEvent e) {
            ControlModuleType moduleType = (ControlModuleType)e.Sender;

            moduleType.BusTypeNames.Add("LGBBUS");
        }
    }
}
