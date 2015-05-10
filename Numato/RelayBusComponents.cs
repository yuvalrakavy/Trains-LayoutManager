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

namespace LayoutManager.ControlComponents {

	[LayoutModule("Numato Relay Components")]
	class NumatoControlComponents : LayoutModuleBase {
		ControlBusType	relayBus = null;
		ControlBusType inputBus = null;

		[LayoutEvent("get-control-bus-type", IfEvent="LayoutEvent[./Options/@BusTypeName='NumatoRelayBus']")]
		private void getRelayBusBusType(LayoutEvent e) {
			if(relayBus == null) {
				relayBus = new ControlBusType();

				relayBus.BusFamilyName = "RelayBus";
				relayBus.BusTypeName = "NumatoRelayBus";
				relayBus.Name = "Relays";
                relayBus.Topology = ControlBusTopology.Fixed;       // Modules cannot be added/removed by the user
				relayBus.AddressingMethod = ControlAddressingMethod.ModuleConnectionPointAddressing;
				relayBus.FirstAddress = 0;
				relayBus.LastAddress = 255;
				relayBus.Usage = ControlConnectionPointUsage.Output;
			}

			e.Info = relayBus;
		}

		[LayoutEvent("recommend-control-module-types", IfEvent="LayoutEvent[./Options/@BusFamily='RelayBus']")]
		private void recommendRelayBusControlModuleTypes(LayoutEvent e) {
			ControlConnectionPointDestination connectionDestination = (ControlConnectionPointDestination)e.Sender;
			IList<string> moduleTypeNames = (IList<string>)e.Info;
			string connectionName = connectionDestination.ConnectionDescription.Name;

			if(connectionDestination.ConnectionDescription.IsCompatibleWith("Relay")) {
				moduleTypeNames.Add("2_NumatoRelays");
                moduleTypeNames.Add("4_NumatoRelays");
                moduleTypeNames.Add("8_NumatoRelays");
                moduleTypeNames.Add("16_NumatoRelays");
				moduleTypeNames.Add("32_NumatoRelays");
			}
		}


		private ControlModuleType GetRelayModuleType(XmlElement parentElement, int nRelays) {
			var moduleType = new ControlModuleType(parentElement, nRelays.ToString() + "_NumatoRelays", nRelays.ToString() + " Relays Module")
			{
				ConnectionPointsPerAddress = 1,
				ConnectionPointIndexBase = 0,
				DefaultControlConnectionPointType =	ControlConnectionPointTypes.OutputRelay,
				NumberOfAddresses = nRelays,
				ConnectionPointLabelFormat = ControlConnectionPointLabelFormatOptions.ConnectionPointIndex,
				ConnectionPointArrangement = nRelays == 8 ? (ControlModuleConnectionPointArrangementOptions.BottomRow|ControlModuleConnectionPointArrangementOptions.StartOnBottom) : ControlModuleConnectionPointArrangementOptions.BothRows
			};

			moduleType.BusTypeNames.Add("NumatoRelayBus");

			return moduleType;
		}

        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='2_NumatoRelays']")]
        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='4_NumatoRelays']")]
        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='8_NumatoRelays']")]
		[LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='16_NumatoRelays']")]
		[LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='32_NumatoRelays']")]
		[LayoutEvent("enum-control-module-types")]
		private void getRelayModule(LayoutEvent e) {
			XmlElement	parentElement = (XmlElement)e.Sender;

			if(e.EventName == "enum-control-module-types") {
				foreach(var nRelays in new int[] { 2, 4, 8, 16, 32 })
					GetRelayModuleType(parentElement, nRelays);
			}
			else {
				var moduleTypeName = e.GetOption("ModuleTypeName");

				int nRelay = Convert.ToInt32(moduleTypeName.Substring(0, moduleTypeName.IndexOf('_')));

				GetRelayModuleType(parentElement, nRelay);
			}
		}
	}
}