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

	[LayoutModule("NCD Relay Components")]
	class NCDControlComponents : LayoutModuleBase {
		ControlBusType	relayBus = null;
		ControlBusType inputBus = null;

		[LayoutEvent("get-control-bus-type", IfEvent="LayoutEvent[./Options/@BusTypeName='NCDRelayBus']")]
		private void getRelayBusBusType(LayoutEvent e) {
			if(relayBus == null) {
				relayBus = new ControlBusType();

				relayBus.BusFamilyName = "RelayBus";
				relayBus.BusTypeName = "NCDRelayBus";
				relayBus.Name = "Relay Bus";
				relayBus.Topology = ControlBusTopology.DaisyChain;
				relayBus.AddressingMethod = ControlAddressingMethod.ModuleConnectionPointAddressing;
				relayBus.FirstAddress = 0;
				relayBus.LastAddress = 255;
				relayBus.Usage = ControlConnectionPointUsage.Output;
			}

			e.Info = relayBus;
		}

		[LayoutEvent("get-control-bus-type", IfEvent = "LayoutEvent[./Options/@BusTypeName='NCDInputBus']")]
		private void getInputBusBusType(LayoutEvent e) {
			if(inputBus == null) {
				inputBus = new ControlBusType();

				inputBus.BusFamilyName = "NCDInputBus";
				inputBus.BusTypeName = "NCDInputBus";
				inputBus.Name = "Input Bus";
				inputBus.Topology = ControlBusTopology.DaisyChain;
				inputBus.AddressingMethod = ControlAddressingMethod.ModuleConnectionPointAddressing;
				inputBus.FirstAddress = 0;
				inputBus.LastAddress = 255;
				inputBus.Usage = ControlConnectionPointUsage.Input;
			}

			e.Info = inputBus;
		}

		[LayoutEvent("recommend-control-module-types", IfEvent="LayoutEvent[./Options/@BusFamily='RelayBus']")]
		private void recommendRelayBusControlModuleTypes(LayoutEvent e) {
			ControlConnectionPointDestination connectionDestination = (ControlConnectionPointDestination)e.Sender;
			IList<string> moduleTypeNames = (IList<string>)e.Info;
			string connectionName = connectionDestination.ConnectionDescription.Name;

			if(connectionDestination.ConnectionDescription.IsCompatibleWith("Relay")) {
				moduleTypeNames.Add("8_Relays");
				moduleTypeNames.Add("16_Relays");
				moduleTypeNames.Add("24_Relays");
				moduleTypeNames.Add("32_Relays");
			}
		}

		[LayoutEvent("recommend-control-module-types", IfEvent = "LayoutEvent[./Options/@BusFamily='NCDInputBus']")]
		private void recommendInputBusControlModuleTypes(LayoutEvent e) {
			ControlConnectionPointDestination connectionDestination = (ControlConnectionPointDestination)e.Sender;
			IList<string> moduleTypeNames = (IList<string>)e.Info;
			string connectionName = connectionDestination.ConnectionDescription.Name;

			if(connectionDestination.ConnectionDescription.IsCompatibleWith("Feedback", "DryContact")) {
				moduleTypeNames.Add("16_ContactsClosure");
				moduleTypeNames.Add("32_ContactsClosure");
				moduleTypeNames.Add("48_ContactsClosure");
			}
		}


		private ControlModuleType GetRelayModule(XmlElement parentElement, int nRelays) {
			var moduleType = new ControlModuleType(parentElement, nRelays.ToString() + "_Relays", nRelays.ToString() + " Relays Module")
			{
				ConnectionPointsPerAddress = 1,
				ConnectionPointIndexBase = 0,
				DefaultControlConnectionPointType =	ControlConnectionPointTypes.OutputRelay,
				NumberOfAddresses = nRelays,
				ConnectionPointLabelFormat = ControlConnectionPointLabelFormatOptions.ConnectionPointIndex,
				ConnectionPointArrangement = nRelays == 8 ? (ControlModuleConnectionPointArrangementOptions.BottomRow|ControlModuleConnectionPointArrangementOptions.StartOnBottom) : ControlModuleConnectionPointArrangementOptions.BothRows
			};

			moduleType.BusTypeNames.Add("NCDRelayBus");

			return moduleType;
		}

		[LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='8_Relays']")]
		[LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='16_Relays']")]
		[LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='24_Relays']")]
		[LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='32_Relays']")]
		[LayoutEvent("enum-control-module-types")]
		private void getRelayModule(LayoutEvent e) {
			XmlElement	parentElement = (XmlElement)e.Sender;

			if(e.EventName == "enum-control-module-types") {
				foreach(var nRelays in new int[] { 8, 16, 24, 32 })
					GetRelayModule(parentElement, nRelays);
			}
			else {
				var moduleTypeName = e.GetOption("ModuleTypeName");

				int nRelay = Convert.ToInt32(moduleTypeName.Substring(0, moduleTypeName.IndexOf('_')));

				GetRelayModule(parentElement, nRelay);
			}
		}

		private ControlModuleType GetContactClosureModule(XmlElement parentElement, int nContacts) {
			var moduleType = new ControlModuleType(parentElement, nContacts.ToString() + "_ContactsClosure", nContacts.ToString() + " Contacts Closure Module")
			{
				ConnectionPointsPerAddress = 1,
				ConnectionPointIndexBase = 0,
				DefaultControlConnectionPointType = ControlConnectionPointTypes.InputDryContact,
				NumberOfAddresses = nContacts,
				ConnectionPointLabelFormat = ControlConnectionPointLabelFormatOptions.ConnectionPointIndex,
				ConnectionPointArrangement = ControlModuleConnectionPointArrangementOptions.BothRows
			};

			moduleType.BusTypeNames.Add("NCDInputBus");

			return moduleType;
		}

		[LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='16_ContactsClosure']")]
		[LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='32_ContactsClosure']")]
		[LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='48_ContactsClosure']")]
		[LayoutEvent("enum-control-module-types")]
		private void getContactClosureModule(LayoutEvent e) {
			XmlElement parentElement = (XmlElement)e.Sender;

			if(e.EventName == "enum-control-module-types") {
				foreach(var nContacts in new int[] { 16, 32, 48 })
					GetContactClosureModule(parentElement, nContacts);
			}
			else {
				var moduleTypeName = e.GetOption("ModuleTypeName");

				int nContacts = Convert.ToInt32(moduleTypeName.Substring(0, moduleTypeName.IndexOf('_')));

				GetContactClosureModule(parentElement, nContacts);
			}
		}
	}
}