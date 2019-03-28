using System;
using System.Collections.Generic;
using System.Xml;
using LayoutManager.Model;

#pragma warning disable IDE0051
namespace LayoutManager.ControlComponents {

    [LayoutModule("NCD Relay Components")]
    class NCDControlComponents : LayoutModuleBase {
        ControlBusType relayBus = null;
        ControlBusType inputBus = null;

        [LayoutEvent("get-control-bus-type", IfEvent = "LayoutEvent[./Options/@BusTypeName='NCDRelayBus']")]
        private void getRelayBusBusType(LayoutEvent e) {
            if (relayBus == null) {
                relayBus = new ControlBusType {
                    BusFamilyName = "RelayBus",
                    BusTypeName = "NCDRelayBus",
                    Name = "Relay Bus",
                    Topology = ControlBusTopology.DaisyChain,
                    AddressingMethod = ControlAddressingMethod.ModuleConnectionPointAddressing,
                    FirstAddress = 0,
                    LastAddress = 255,
                    Usage = ControlConnectionPointUsage.Output
                };
            }

            e.Info = relayBus;
        }

        [LayoutEvent("get-control-bus-type", IfEvent = "LayoutEvent[./Options/@BusTypeName='NCDInputBus']")]
        private void getInputBusBusType(LayoutEvent e) {
            if (inputBus == null) {
                inputBus = new ControlBusType {
                    BusFamilyName = "NCDInputBus",
                    BusTypeName = "NCDInputBus",
                    Name = "Input Bus",
                    Topology = ControlBusTopology.DaisyChain,
                    AddressingMethod = ControlAddressingMethod.ModuleConnectionPointAddressing,
                    FirstAddress = 0,
                    LastAddress = 255,
                    Usage = ControlConnectionPointUsage.Input
                };
            }

            e.Info = inputBus;
        }

        [LayoutEvent("recommend-control-module-types", IfEvent = "LayoutEvent[./Options/@BusTypeName='NCDRelayBus']")]
        private void recommendRelayBusControlModuleTypes(LayoutEvent e) {
            ControlConnectionPointDestination connectionDestination = (ControlConnectionPointDestination)e.Sender;
            IList<string> moduleTypeNames = (IList<string>)e.Info;

            if (connectionDestination.ConnectionDescription.IsCompatibleWith("Relay")) {
                moduleTypeNames.Add("8_NCDRelays");
                moduleTypeNames.Add("16_NCDRelays");
                moduleTypeNames.Add("24_NCDRelays");
                moduleTypeNames.Add("32_NCDRelays");
            }
        }

        [LayoutEvent("recommend-control-module-types", IfEvent = "LayoutEvent[./Options/@BusTypeName='NCDInputBus']")]
        private void recommendInputBusControlModuleTypes(LayoutEvent e) {
            ControlConnectionPointDestination connectionDestination = (ControlConnectionPointDestination)e.Sender;
            IList<string> moduleTypeNames = (IList<string>)e.Info;

            if (connectionDestination.ConnectionDescription.IsCompatibleWith("Feedback", ControlConnectionPointTypes.InputDryTrigger)) {
                moduleTypeNames.Add("16_NCDContactsClosure");
                moduleTypeNames.Add("32_NCDContactsClosure");
                moduleTypeNames.Add("48_NCDContactsClosure");
            }
        }


        private ControlModuleType GetRelayModule(XmlElement parentElement, int nRelays) {
            var moduleType = new ControlModuleType(parentElement, nRelays.ToString() + "_NCDRelays", nRelays.ToString() + " Relays Module") {
                ConnectionPointsPerAddress = 1,
                ConnectionPointIndexBase = 0,
                DefaultControlConnectionPointType = ControlConnectionPointTypes.OutputRelay,
                NumberOfAddresses = nRelays,
                ConnectionPointLabelFormat = ControlConnectionPointLabelFormatOptions.ConnectionPointIndex,
                ConnectionPointArrangement = nRelays == 8 ? (ControlModuleConnectionPointArrangementOptions.BottomRow | ControlModuleConnectionPointArrangementOptions.StartOnBottom) : ControlModuleConnectionPointArrangementOptions.BothRows
            };

            moduleType.BusTypeNames.Add("NCDRelayBus");

            return moduleType;
        }

        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='8_NCDRelays']")]
        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='16_NCDRelays']")]
        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='24_NCDRelays']")]
        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='32_NCDRelays']")]
        [LayoutEvent("enum-control-module-types")]
        private void getRelayModule(LayoutEvent e) {
            XmlElement parentElement = (XmlElement)e.Sender;

            if (e.EventName == "enum-control-module-types") {
                foreach (var nRelays in new int[] { 8, 16, 24, 32 })
                    GetRelayModule(parentElement, nRelays);
            }
            else {
                var moduleTypeName = e.GetOption("ModuleTypeName");

                int nRelay = Convert.ToInt32(moduleTypeName.Substring(0, moduleTypeName.IndexOf('_')));

                GetRelayModule(parentElement, nRelay);
            }
        }

        private ControlModuleType GetContactClosureModule(XmlElement parentElement, int nContacts) {
            var moduleType = new ControlModuleType(parentElement, nContacts.ToString() + "_NCDContactsClosure", nContacts.ToString() + " Contacts Closure Module") {
                ConnectionPointsPerAddress = 1,
                ConnectionPointIndexBase = 0,
                DefaultControlConnectionPointType = ControlConnectionPointTypes.InputDryTrigger,
                NumberOfAddresses = nContacts,
                ConnectionPointLabelFormat = ControlConnectionPointLabelFormatOptions.ConnectionPointIndex,
                ConnectionPointArrangement = ControlModuleConnectionPointArrangementOptions.BothRows
            };

            moduleType.BusTypeNames.Add("NCDInputBus");

            return moduleType;
        }

        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='16_NCDContactsClosure']")]
        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='32_NCDContactsClosure']")]
        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='48_NCDContactsClosure']")]
        [LayoutEvent("enum-control-module-types")]
        private void getContactClosureModule(LayoutEvent e) {
            XmlElement parentElement = (XmlElement)e.Sender;

            if (e.EventName == "enum-control-module-types") {
                foreach (var nContacts in new int[] { 16, 32, 48 })
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