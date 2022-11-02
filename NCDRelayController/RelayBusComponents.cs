using LayoutManager.Model;
using System;
using System.Collections.Generic;
using System.Xml;
using MethodDispatcher;

namespace LayoutManager.ControlComponents {
    [LayoutModule("NCD Relay Components")]
    internal class NCDControlComponents : LayoutModuleBase {
        private ControlBusType? relayBus = null;
        private ControlBusType? inputBus = null;

        [DispatchTarget]
        private ControlBusType GetControlBusType_NCDRelayBus([DispatchFilter] string busTypeName = "NCDRelayBus") {
            return relayBus ??= new ControlBusType {
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

        [DispatchTarget]
        private ControlBusType GetControlBusType_NCDInputBus([DispatchFilter] string busTypeName = "NCDInputBus") {
            return inputBus ??= new ControlBusType {
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

        [DispatchTarget]
        private void RecommendControlModuleTypes_NCDRelayBus(ControlConnectionPointDestination connectionDestination, List<string> moduleTypeNames, string busTypeName, [DispatchFilter] string busFamilyName = "NCDRelayBus") {
            if (connectionDestination.ConnectionDescription.IsCompatibleWith("Relay")) {
                moduleTypeNames.Add("8_NCDRelays");
                moduleTypeNames.Add("16_NCDRelays");
                moduleTypeNames.Add("24_NCDRelays");
                moduleTypeNames.Add("32_NCDRelays");
            }
        }

        [DispatchTarget]
        private void RecommendControlModuleTypes_NCDInputBus(ControlConnectionPointDestination connectionDestination, List<string> moduleTypeNames, string busTypeName, [DispatchFilter] string busFamilyName = "NCDInputBus") {
            if (connectionDestination.ConnectionDescription.IsCompatibleWith("Feedback", ControlConnectionPointTypes.InputDryTrigger)) {
                moduleTypeNames.Add("16_NCDContactsClosure");
                moduleTypeNames.Add("32_NCDContactsClosure");
                moduleTypeNames.Add("48_NCDContactsClosure");
            }
        }

        private ControlModuleType GetRelayModuleType(int nRelays) {
            var moduleType = new ControlModuleType($"{nRelays}_NCDRelays", $"{nRelays} Relays Module") {
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

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_8relays([DispatchFilter("RegEx", "^(8_NCDRelays|ALL)$")] string moduleTypeName) => GetRelayModuleType
            (8);

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_16relays([DispatchFilter("RegEx", "^(16_NCDRelays|ALL)$")] string moduleTypeName) => GetRelayModuleType(16);

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_24relays([DispatchFilter("RegEx", "^(24_NCDRelays|ALL)$")] string moduleTypeName) => GetRelayModuleType(24);

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_32relays([DispatchFilter("RegEx", "^(32_NCDRelays|ALL)$")] string moduleTypeName) => GetRelayModuleType(32);


        private ControlModuleType GetContactClosureModuleType(int nContacts) {
            var moduleType = new ControlModuleType($"{nContacts}_NCDContactsClosure", $"{nContacts} Contacts Closure Module") {
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

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_16contacts([DispatchFilter("RegEx", "^(16_NCDContactsClosure|ALL)$")] string moduleType) => GetContactClosureModuleType(16);

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_32contacts([DispatchFilter("RegEx", "^(32_NCDContactsClosure|ALL)$")] string moduleType) => GetContactClosureModuleType(32);

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_48contacts([DispatchFilter("RegEx", "^(48_NCDContactsClosure|ALL)$")] string moduleType) => GetContactClosureModuleType(48);

    }
}