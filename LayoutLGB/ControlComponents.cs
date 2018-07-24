
using LayoutManager;
using LayoutManager.Model;

namespace LayoutLGB {

    [LayoutModule("LGB MTS Bus Definitions")]
	class LGBcontrolComponents : LayoutModuleBase {

		ControlBusType	dccBus = null;
		ControlBusType	lgbBus = null;

		[LayoutEvent("get-control-bus-type", IfEvent="LayoutEvent[./Options/@BusTypeName='LGBDCC']")]
		private void getLGBDCCbusType(LayoutEvent e) {
			if(dccBus == null) {
				dccBus = new ControlBusType();

				dccBus.BusFamilyName = "DCC";
				dccBus.BusTypeName = "LGBDCC";
				dccBus.Name = "Tracks (DCC)";
				dccBus.Topology = ControlBusTopology.RandomAddressing;
				dccBus.FirstAddress = 1;
				dccBus.LastAddress = 128;
				dccBus.Usage = ControlConnectionPointUsage.Output;
			}

			e.Info = dccBus;
		}

		[LayoutEvent("get-control-bus-type", IfEvent="LayoutEvent[./Options/@BusTypeName='LGBBUS']")]
		private void getLGBBUSbusType(LayoutEvent e) {
			if(lgbBus == null) {
				lgbBus = new ControlBusType();

				lgbBus.BusFamilyName = "LGBBUS";
				lgbBus.BusTypeName = "LGBBUS";
				lgbBus.Name = "LGB Bus";
				lgbBus.Topology = ControlBusTopology.RandomAddressing;
				lgbBus.FirstAddress = 1;
				lgbBus.LastAddress = 256;
				lgbBus.RecommendedStartAddress = 129;
				lgbBus.Usage = ControlConnectionPointUsage.Input;
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
