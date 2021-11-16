using System;
using System.Collections.Generic;
using System.Xml;
using LayoutManager.Model;

namespace LayoutManager.ControlComponents {
    [LayoutModule("Numato Relay Components")]
    internal class NumatoControlComponents : LayoutModuleBase {
        private ControlBusType? relayBus = null;

        [LayoutEvent("get-control-bus-type", IfEvent = "LayoutEvent[./Options/@BusTypeName='NumatoRelayBus']")]
        private void GetRelayBusBusType(LayoutEvent e) {
            if (relayBus == null) {
                relayBus = new ControlBusType {
                    BusFamilyName = "RelayBus",
                    BusTypeName = "NumatoRelayBus",
                    Name = "Relays",
                    Topology = ControlBusTopology.Fixed,       // Modules cannot be added/removed by the user
                    AddressingMethod = ControlAddressingMethod.ModuleConnectionPointAddressing,
                    FirstAddress = 0,
                    LastAddress = 255,
                    Usage = ControlConnectionPointUsage.Output
                };
            }

            e.Info = relayBus;
        }

        [LayoutEvent("recommend-control-module-types", IfEvent = "LayoutEvent[./Options/@BusFamily='RelayBus']")]
        private void RecommendRelayBusControlModuleTypes(LayoutEvent e) {
            var connectionDestination = Ensure.NotNull<ControlConnectionPointDestination>(e.Sender);
            var moduleTypeNames = Ensure.NotNull<IList<string>>(e.Info);

            if (connectionDestination.ConnectionDescription.IsCompatibleWith("Relay")) {
                moduleTypeNames.Add("2_NumatoRelays");
                moduleTypeNames.Add("4_NumatoRelays");
                moduleTypeNames.Add("8_NumatoRelays");
                moduleTypeNames.Add("16_NumatoRelays");
                moduleTypeNames.Add("32_NumatoRelays");
            }
        }

        private ControlModuleType GetRelayModuleType(XmlElement parentElement, int nRelays) {
            var moduleType = new ControlModuleType(parentElement, nRelays.ToString() + "_NumatoRelays", nRelays.ToString() + " Relays Module") {
                ConnectionPointsPerAddress = 1,
                ConnectionPointIndexBase = 0,
                DefaultControlConnectionPointType = ControlConnectionPointTypes.OutputRelay,
                NumberOfAddresses = nRelays,
                ConnectionPointLabelFormat = ControlConnectionPointLabelFormatOptions.ConnectionPointIndex,
                ConnectionPointArrangement = nRelays == 8 ? (ControlModuleConnectionPointArrangementOptions.BottomRow | ControlModuleConnectionPointArrangementOptions.StartOnBottom) : ControlModuleConnectionPointArrangementOptions.BothRows
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
        private void GetRelayModule(LayoutEvent e) {
            var parentElement = Ensure.NotNull<XmlElement>(e.Sender);

            if (e.EventName == "enum-control-module-types") {
                foreach (var nRelays in new int[] { 2, 4, 8, 16, 32 })
                    GetRelayModuleType(parentElement, nRelays);
            }
            else {
                var moduleTypeName = e.GetOption("ModuleTypeName").ValidString();

                int nRelay = Convert.ToInt32(moduleTypeName[..moduleTypeName.IndexOf('_')]);

                GetRelayModuleType(parentElement, nRelay);
            }
        }
    }
}