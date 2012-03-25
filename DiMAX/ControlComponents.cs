using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Windows.Forms;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;
using LayoutManager.CommonUI;

namespace DiMAX {

	[LayoutModule("DiMAX Bus Definitions")]
	class LGBcontrolComponents : LayoutModuleBase {

		ControlBusType	dccBus = null;
		ControlBusType	dimaxBus = null;
		ControlBusType  locoBus = null;

		[LayoutEvent("get-control-bus-type", IfEvent = "LayoutEvent[./Options/@BusTypeName='DiMAXDCC']")]
		private void getDCCbusType(LayoutEvent e) {
			if(dccBus == null) {
				dccBus = new ControlBusType();

				dccBus.BusFamilyName = "DCC";
				dccBus.BusTypeName = "DiMAXDCC";
				dccBus.Name = "Tracks (DCC)";
				dccBus.Topology = ControlBusTopology.RandomAddressing;
				dccBus.FirstAddress = 1;
				dccBus.LastAddress = 2048;
				dccBus.Usage = ControlConnectionPointUsage.Output;
			}

			e.Info = dccBus;
		}

		[LayoutEvent("get-control-bus-type", IfEvent = "LayoutEvent[./Options/@BusTypeName='DiMAXBUS']")]
		private void getDIMAXBUSbusType(LayoutEvent e) {
			if(dimaxBus == null) {
				dimaxBus = new ControlBusType();

				dimaxBus.BusFamilyName = "DiMAXBus";
				dimaxBus.BusTypeName = "DiMAXBUS";
				dimaxBus.Name = "DiMAX Bus";
				dimaxBus.Topology = ControlBusTopology.RandomAddressing;
				dimaxBus.FirstAddress = 1;
				dimaxBus.LastAddress = 2048;
				dimaxBus.RecommendedStartAddress = 1;
				dimaxBus.Usage = ControlConnectionPointUsage.Input;
			}

			e.Info = dimaxBus;
		}

		[LayoutEvent("get-control-bus-type", IfEvent = "LayoutEvent[./Options/@BusTypeName='DiMAXLocoBus']")]
		private void getDIMAXLocoBusType(LayoutEvent e) {
			if(locoBus == null) {
				locoBus = new ControlBusType();

				locoBus.BusFamilyName = "LocoBus";
				locoBus.BusTypeName = "DiMAXLocoBus";
				locoBus.Name = "Function Decoders";
				locoBus.Topology = ControlBusTopology.RandomAddressing;
				locoBus.FirstAddress = 1;
				locoBus.LastAddress = 10239;
				locoBus.RecommendedStartAddress = 2000;
				locoBus.Usage = ControlConnectionPointUsage.Output;
			}

			e.Info = locoBus;
		}

		[LayoutEvent("add-bus-connectable-to-module", IfEvent = "LayoutEvent[Options/@GenericBusType='DCC']")]
		private void AddDiMAXDCCbusToDCCcompatibleModule(LayoutEvent e) {
			ControlModuleType moduleType = (ControlModuleType)e.Sender;

			moduleType.BusTypeNames.Add("DiMAXDCC");
		}

		[LayoutEvent("add-bus-connectable-to-module", IfEvent = "LayoutEvent[Options/@GenericBusType='DiMAXBus']")]
		private void AddDiMAXBUStoLGBBUScompatibleModule(LayoutEvent e) {
			ControlModuleType moduleType = (ControlModuleType)e.Sender;

			moduleType.BusTypeNames.Add("DiMAXBUS");
		}

		[LayoutEvent("add-bus-connectable-to-module", IfEvent = "LayoutEvent[Options/@GenericBusType='LocoBus']")]
		private void AddDiMAXLocoBustoLGBBUScompatibleModule(LayoutEvent e) {
			ControlModuleType moduleType = (ControlModuleType)e.Sender;

			moduleType.BusTypeNames.Add("DiMAXLocoBus");
		}
	}
}
