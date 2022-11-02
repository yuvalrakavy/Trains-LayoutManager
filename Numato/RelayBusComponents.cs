using LayoutManager.Model;
using System;
using System.Collections.Generic;
using System.Xml;
using MethodDispatcher;

namespace LayoutManager.ControlComponents {
    [LayoutModule("Numato Relay Components")]
    internal class NumatoControlComponents : LayoutModuleBase {
        private ControlBusType? relayBus = null;

        [DispatchTarget]
        private ControlBusType GetControlBusType_NumatoRelayBus([DispatchFilter] string busTypeName = "NumatoRelayBus") {
            return relayBus ??= new ControlBusType {
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

        [DispatchTarget]
        private void RecommendControlModuleTypes_RelayBus(ControlConnectionPointDestination connectionDestination, List<string> moduleTypeNames, string busTypeName, [DispatchFilter] string busFamilyName = "RelayBus") {
            if (connectionDestination.ConnectionDescription.IsCompatibleWith("Relay")) {
                moduleTypeNames.Add("2_NumatoRelays");
                moduleTypeNames.Add("4_NumatoRelays");
                moduleTypeNames.Add("8_NumatoRelays");
                moduleTypeNames.Add("16_NumatoRelays");
                moduleTypeNames.Add("32_NumatoRelays");
            }
        }

        private ControlModuleType GetRelayModuleType(int nRelays) {
            var moduleType = new ControlModuleType($"{nRelays}_NumatoRelays", $"{nRelays} Relays Module") {
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

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_2_NumatoRelays([DispatchFilter("RegEx", "^(2_NumatoRelays|ALL)$")] string moduleTypeName) => GetRelayModuleType(2);

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_4_NumatoRelays([DispatchFilter("RegEx", "^(4_NumatoRelays|ALL)$")] string moduleTypeName) => GetRelayModuleType(4);

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_8_NumatoRelays([DispatchFilter("RegEx", "^(8_NumatoRelays|ALL)$")] string moduleTypeName) => GetRelayModuleType(8);

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_16_NumatoRelays([DispatchFilter("RegEx", "^(16_NumatoRelays|ALL)$")] string moduleTypeName) => GetRelayModuleType(16);

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_32_NumatoRelays([DispatchFilter("RegEx", "^(32_NumatoRelays|ALL)$")] string moduleTypeName) => GetRelayModuleType(32);
    }
}