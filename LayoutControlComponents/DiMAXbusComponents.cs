﻿using LayoutManager.Model;
using MethodDispatcher;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.ControlComponents {
    [LayoutModule("DiMAX Bus Control Components")]
    internal class DiMAXBusComponents : LayoutModuleBase {

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_Massoth8170001([DispatchFilter("RegEx", "(Massoth8170001|ALL)")] string moduleTypeName) {
            var moduleType = new ControlModuleType("Massoth8170001", "Massoth Trigger Feedback Interface") {
                AddressAlignment = 4,
                ConnectionPointsPerAddress = 2,
                NumberOfAddresses = 4,
                DefaultControlConnectionPointType = ControlConnectionPointTypes.InputDryTrigger,
                ConnectionPointIndexBase = 0,
                ConnectionPointLabelFormat = ControlConnectionPointLabelFormatOptions.Alpha | ControlConnectionPointLabelFormatOptions.AlphaLowercase | ControlConnectionPointLabelFormatOptions.AttachAddress,
                LastAddress = 2048,
            };

            moduleType.AddBusTypes("DiMAXBus");
            return moduleType;
        }

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_Massoth8170001level([DispatchFilter("RegEx", "(Massoth8170001level|ALL)")] string moduleTypeName) {
            var moduleType = new ControlModuleType("Massoth8170001level", "Massoth Level Feedback Interface") {
                AddressAlignment = 4,
                ConnectionPointsPerAddress = 1,
                NumberOfAddresses = 8,
                DefaultControlConnectionPointType = ControlConnectionPointTypes.InputDryLevel,
                ConnectionPointIndexBase = 0,
                ConnectionPointLabelFormat = 0,
                LastAddress = 2048,
                DecoderTypeName = "GenericDCC",
            };

            moduleType.AddBusTypes("DiMAXBus");
            return moduleType;
        }

        [DispatchTarget]
        private void RecommendControlModuleTypes_DiMAXBus(ControlConnectionPointDestination connectionDestination, List<string> moduleTypeNames, string busFamilyName, [DispatchFilter] string busTypeName = "DiMAXBus") {
            if (connectionDestination.ConnectionDescription.IsCompatibleWith("Feedback", "DryContact"))
                moduleTypeNames.Add("Massoth8170001");
            if (connectionDestination.ConnectionDescription.IsCompatibleWith("Feedback", "Level"))
                moduleTypeNames.Add("Massoth8170001level");
        }

        [DispatchTarget]
        private bool QueryAction_SetMassothFeedbackDecoderAddress([DispatchFilter("ModuleType", "Massoth817001.*")] ControlModule target, [DispatchFilter] string actionName = "set-address") {
            return true;
        }

        [DispatchTarget]
        private LayoutAction? GetControlModuleAction_ProgramMassothTriggerFeedbackDecoderAddress(XmlElement actionElement, [DispatchFilter("ModuleType", "Massoth8170001")] ControlModule module, [DispatchFilter] string actionType = "set-address") {
            return new ProgramMassothTriggerFeedbackDecoderAddress(actionElement, module);
        }

        [DispatchTarget]
        private LayoutAction? GetControlModuleAction_ProgramMassothLevelFeedbackDecoderAddress(XmlElement actionElement, [DispatchFilter("ModuleType", "Massoth8170001level")] ControlModule module, [DispatchFilter] string actionType = "set-address") {
            return new ProgramMassothLevelFeedbackDecoderAddress(actionElement, module);
        }

        [DispatchTarget]
        private bool EditActionSettings_MassothFeedbackSetAddressSettings(ControlModule module, [DispatchFilter] IMassothFeedbackDecoderSetAddress action) {
            using var d = new Dialogs.MassothFeedbackDecoderAddressSettings(action, module);

            return d.ShowDialog() == DialogResult.OK;
        }
    }

    internal class ProgramMassothFeedbackDecoder : LayoutDccProgrammingAction<ControlModule> {
        public ProgramMassothFeedbackDecoder(XmlElement actionElement, ControlModule feedbackDecoder)
            : base(actionElement, feedbackDecoder) {
        }
    }

    /// <summary>
    /// How the feedback module is connected: Master - it is connected to a DiMAX bus port, Slave - it is chained to a "master" feedback module
    /// </summary>
    public enum MassothFeedbackDecoderBusConnectionMethod {
        Master, Slave
    }

    public interface IMassothFeedbackDecoderSetAddress : ILayoutAction {
        MassothFeedbackDecoderBusConnectionMethod BusConnectionMethod { get; set; }

        int BusId { get; set; }
    }

    public class MassothFeedbackModule : ControlModule {
        private const string A_DiMAXBusConnectionMethod = "DiMAXBusConnectionMethod";
        private const string A_DiMAXBusID = "DiMAXBusID";

        public MassothFeedbackModule(ControlModule module)
            : base(module.ControlManager, module.Element) {
        }

        public bool HasDiMAX_BusConnectionMethod => HasAttribute(A_DiMAXBusConnectionMethod);

        public MassothFeedbackDecoderBusConnectionMethod DiMAX_BusConnectionMethod {
            get => AttributeValue(A_DiMAXBusConnectionMethod).Enum<MassothFeedbackDecoderBusConnectionMethod>() ?? MassothFeedbackDecoderBusConnectionMethod.Slave;
            set => SetAttributeValue(A_DiMAXBusConnectionMethod, value);
        }

        public bool HasDiMAX_BusId => HasAttribute(A_DiMAXBusID);

        public int DiMAX_BusId {
            get => (int?)AttributeValue(A_DiMAXBusID) ?? 12;
            set => SetAttributeValue(A_DiMAXBusID, value);
        }

        /// <summary>
        /// Allocate DiMAX bus ID for using on a master feedback module
        /// </summary>
        /// <param name="commandStation"></param>
        /// <returns></returns>
        public static int AllocateDiMAX_BusID(IModelComponentIsBusProvider commandStation) {
            var bus = LayoutModel.ControlManager.Buses.GetBus(commandStation, "DiMAXBUS");

            if (bus != null) {
                var masterFeedbackModules = from module in bus.Modules where module.ModuleTypeName == "Massoth8170001" && new MassothFeedbackModule(module).DiMAX_BusConnectionMethod == MassothFeedbackDecoderBusConnectionMethod.Master select new MassothFeedbackModule(module);

                for (int busId = 11; busId <= 20; busId++)
                    if (!masterFeedbackModules.Any(m => m.DiMAX_BusId == busId))
                        return busId;
            }

            return -1;
        }

        public static MassothFeedbackModule? GetMasterUsingBusId(IModelComponentIsBusProvider commandStation, int busId) {
            var bus = Ensure.NotNull<ControlBus>(LayoutModel.ControlManager.Buses.GetBus(commandStation, "DiMAXBUS"), "DiMAXBUS");

            return (from module in bus.Modules
                    where module.ModuleTypeName == "Massoth8170001"
                    && new MassothFeedbackModule(module).DiMAX_BusConnectionMethod == MassothFeedbackDecoderBusConnectionMethod.Master
                    && new MassothFeedbackModule(module).DiMAX_BusId == busId
                    select new MassothFeedbackModule(module)).FirstOrDefault();
        }
    }

    internal class ProgramMassothTriggerFeedbackDecoderAddress : ProgramMassothFeedbackDecoder, IMassothFeedbackDecoderSetAddress {
        public ProgramMassothTriggerFeedbackDecoderAddress(XmlElement actionElement, ControlModule feedbackModule)
            : base(actionElement, feedbackModule) {
        }

        public override void PrepareProgramming() {
            SetCV(1, 0);        // DiMAX mode

            if (BusConnectionMethod == MassothFeedbackDecoderBusConnectionMethod.Master)
                SetCV(2, (byte)BusId);

            SetCV(3, (byte)(BusConnectionMethod == MassothFeedbackDecoderBusConnectionMethod.Master ? 1 : 2));

            int address = ProgrammingTarget.Address;
            for (int i = 0; i < 4; i++) {
                SetCV(51 + (i * 10), (byte)(address >> 8));
                SetCV(51 + (i * 10) + 1, (byte)(address & 0xff));
                SetCV(55 + (i * 10), 0);      // Feedback command - trigger
                address++;
            }
        }

        public override void Commit() {
            var module = new MassothFeedbackModule(ProgrammingTarget) {
                DiMAX_BusConnectionMethod = BusConnectionMethod
            };

            string label = module.Label ?? "";
            label = Regex.Replace(label, "\\[.*\\]", "");

            if (label.Length > 0)
                label += " ";

            if (BusConnectionMethod == MassothFeedbackDecoderBusConnectionMethod.Slave)
                label += "[Slave]";
            else
                label += $"[Master ID: {module.DiMAX_BusId}]";

            module.Label = label;
            module.AddressProgrammingRequired = false;
        }

        public override string Description => $"Set feedback module address to {ProgrammingTarget.Address} to {ProgrammingTarget.Address + ProgrammingTarget.ModuleType.NumberOfAddresses - 1}";

        #region IMassothFeedbackDecoderSetAddress Members
        private const string A_BusConnection = "BusConnectionMethod";
        private const string A_BusId = "BusID";

        public MassothFeedbackDecoderBusConnectionMethod BusConnectionMethod {
            get => AttributeValue(A_BusConnection).Enum<MassothFeedbackDecoderBusConnectionMethod>() ?? MassothFeedbackDecoderBusConnectionMethod.Slave;
            set => SetAttributeValue(A_BusConnection, value);
        }

        public int BusId {
            get => (int)AttributeValue(A_BusId);
            set => SetAttributeValue(A_BusId, value);
        }

        #endregion
    }

    internal class ProgramMassothLevelFeedbackDecoderAddress : ProgramMassothFeedbackDecoder, IMassothFeedbackDecoderSetAddress {
        public ProgramMassothLevelFeedbackDecoderAddress(XmlElement actionElement, ControlModule feedbackModule)
            : base(actionElement, feedbackModule) {
        }

        public override void PrepareProgramming() {
            SetCV(1, 0);        // DiMAX mode

            if (BusConnectionMethod == MassothFeedbackDecoderBusConnectionMethod.Master)
                SetCV(2, (byte)BusId);

            SetCV(3, (byte)(BusConnectionMethod == MassothFeedbackDecoderBusConnectionMethod.Master ? 1 : 2));

            int address = ProgrammingTarget.Address;
            for (int i = 0; i < 4; i++) {
                SetCV(51 + (i * 10), (byte)(address >> 8));
                SetCV(51 + (i * 10) + 1, (byte)(address & 0xff));
                SetCV(55 + (i * 10), 2);      // Feedback command - level
                address += 2;
            }
        }

        public override void Commit() {
            var module = new MassothFeedbackModule(ProgrammingTarget) {
                DiMAX_BusConnectionMethod = BusConnectionMethod
            };

            string label = module.Label ?? "";
            label = Regex.Replace(label, "\\[.*\\]", "");

            if (label.Length > 0)
                label += " ";

            if (BusConnectionMethod == MassothFeedbackDecoderBusConnectionMethod.Slave)
                label += "[Slave]";
            else
                label += $"[Master ID: {module.DiMAX_BusId}]";

            module.Label = label;
            module.AddressProgrammingRequired = false;
        }

        public override string Description => "Set feedback module address to " + ProgrammingTarget.Address + " to " + (ProgrammingTarget.Address + ProgrammingTarget.ModuleType.NumberOfAddresses - 1);

        #region IMassothFeedbackDecoderSetAddress Members
        private const string A_BusConnection = "BusConnectionMethod";
        private const string A_BusId = "BusID";

        public MassothFeedbackDecoderBusConnectionMethod BusConnectionMethod {
            get => AttributeValue(A_BusConnection).Enum<MassothFeedbackDecoderBusConnectionMethod>() ?? MassothFeedbackDecoderBusConnectionMethod.Slave;
            set => Element.SetAttributeValue(A_BusConnection, value);
        }

        public int BusId {
            get => (int)AttributeValue(A_BusId);
            set => SetAttributeValue(A_BusId, value);
        }

        #endregion
    }
}
