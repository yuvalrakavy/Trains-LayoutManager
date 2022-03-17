using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.ControlComponents {
    public static class ControlComponents {
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static string ControlComponentsVersion = "1.0";
#pragma warning restore CA2211 // Non-constant fields should not be visible
    }

    [LayoutModule("DCC Control Components")]
    internal class DCCconrolComponents : LayoutModuleBase {

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_LGB55025([DispatchFilter("RegEx", "(LGB55025|ALL)")] string moduleTypeName) {
            ControlModuleType moduleType = new("LGB55025", "LGB Switch Decoder") {
                AddressAlignment = 4,
                ConnectionPointsPerAddress = 1,
                NumberOfAddresses = 4,
                LastAddress = 128
            };

            moduleType.BusTypeNames.Add("DCC");

            return moduleType;
        }

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_Massoth8156001([DispatchFilter("RegEx", "(Massoth8156001|ALL)")] string moduleTypeName) {
            ControlModuleType moduleType = new("Massoth8156001", "Massoth Switch Decoder") {
                AddressAlignment = 1,
                ConnectionPointsPerAddress = 1,
                NumberOfAddresses = 4,
                LastAddress = 2048,
            };

            moduleType.BusTypeNames.Add("DCC");
            return moduleType;
        }

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_Massoth8156001asFunctionDecoder([DispatchFilter("RegEx", "(Massoth8156001_AsFunctionDecoder|ALL)")] string moduleTypeName) {
            ControlModuleType moduleType = new("Massoth8156001_AsFunctionDecoder", "Massoth Function Decoder") {
                AddressAlignment = 1,
                ConnectionPointsPerAddress = 8,
                NumberOfAddresses = 1,
                LastAddress = 10239,
                ConnectionPointLabelFormat = ControlConnectionPointLabelFormatOptions.ConnectionPointIndex | ControlConnectionPointLabelFormatOptions.NoAttachedAddress,
                ConnectionPointIndexBase = 1,
                DefaultControlConnectionPointType = ControlConnectionPointTypes.OutputRelay,
            };

            moduleType.BusTypeNames.Add("DCC");
            return moduleType;
        }

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_Massoth8156501([DispatchFilter("RegEx", "(Massoth8156501|ALL)")] string moduleTypeName) {
            ControlModuleType moduleType = new("Massoth8156501", "Massoth Single Switch Decoder") {
                AddressAlignment = 1,
                ConnectionPointsPerAddress = 1,
                NumberOfAddresses = 1,
                LastAddress = 2048,
                DecoderTypeName = "GenericDCC",
            };

            moduleType.BusTypeNames.Add("DCC");
            return moduleType;
        }

        private class ProgramMassothSwitchDecoder : LayoutDccProgrammingAction<ControlModule> {
            public ProgramMassothSwitchDecoder(XmlElement actionElement, ControlModule switchDecoder)
                : base(actionElement, switchDecoder) {
            }
        }

        private interface IMassothIgnoreFeedback {
        }

        private class ProgramMassothSwitchDecoderAddress : ProgramMassothSwitchDecoder, IMassothIgnoreFeedback {
            public ProgramMassothSwitchDecoderAddress(XmlElement actionElement, ControlModule switchDecoder)
                : base(actionElement, switchDecoder) {
            }

            public override void PrepareProgramming() {
                if (ProgrammingTarget.Bus.BusType.BusFamilyName == "DCC") {
                    // DCC bus - use switch decoders address space
                    SetCV(29, 0x80);

                    int address = ProgrammingTarget.Address;
                    for (int n = 0; n < ProgrammingTarget.ModuleType.NumberOfAddresses; n++) {
                        SetCV(31 + (n * 2), (byte)((address >> 8) & 0xff));
                        SetCV(31 + (n * 2) + 1, (byte)(address & 0xff));

                        address++;
                    }
                }
                else {
                    // Loco bus (locomotive address space)
                    if (ProgrammingTarget.Address > 127) {
                        // Long address
                        SetCV(29, 0x20);
                        SetCV(17, (byte)((ProgrammingTarget.Address >> 8) & 0xff));
                        SetCV(18, (byte)(ProgrammingTarget.Address & 0xff));
                    }
                    else {
                        // Short address
                        SetCV(29, 0);
                        SetCV(1, (byte)ProgrammingTarget.Address);
                    }
                }
            }

            public override void Commit() {
                ProgrammingTarget.AddressProgrammingRequired = false;
            }

            public override string Description {
                get {
                    if (ProgrammingTarget.Bus.BusType.BusFamilyName == "DCC") {
                        return ProgrammingTarget.ModuleType.NumberOfAddresses > 1
                            ? "Set address to switch address range " + ProgrammingTarget.Address + " to " + (ProgrammingTarget.Address + ProgrammingTarget.ModuleType.NumberOfAddresses - 1).ToString()
                            : "Set address to switch address " + ProgrammingTarget.Address;
                    }
                    else
                        return "Set address to locomotive bus address " + ProgrammingTarget.Address;
                }
            }
        }

        //[LayoutEvent("query-action", IfEvent = "LayoutEvent[Options/@Action='set-address']", SenderType = typeof(ControlModule), IfSender = "*[starts-with(@ModuleTypeName, 'Massoth8156')]")]
        [DispatchTarget]
        private bool QueryControlModuleAction_SetMassothSwitchDecoderAddress([DispatchFilter(Type = "RegEx", Value = "Massoth8156.*")] string moduleTypeName, [DispatchFilter] string action = "set-address") {
            return true;
        }

        //[LayoutEvent("get-action", IfSender = "Action[@Type='set-address']", InfoType = typeof(ControlModule), IfInfo = "*[starts-with(@ModuleTypeName, 'Massoth8156')]")]
        [DispatchTarget]
        private LayoutAction? GetControlModuleAction_ProgramMassothSwitchDecoderAddress(XmlElement actionElement, [DispatchFilter("ModuleType", "Massoth8156.*")] ControlModule module, [DispatchFilter] string actionName = "set-address") {
            return new ProgramMassothSwitchDecoderAddress(actionElement, module);

        }

        //[LayoutEvent("can-control-be-connected", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='LGB55025']")]
        //[LayoutEvent("can-control-be-connected", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='Massoth8156001']")]
        //[LayoutEvent("can-control-be-connected", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='Massoth8156501']")]
        [DispatchTarget]
        private bool CanControlBeConnected_SwitchDecoder(ControlConnectionPointDestination connectionDestination, Guid moduleId, int index, [DispatchFilter("RegEx", "(LGB55025|Massoth8156001|Massoth8156501)")] string moduleTypeName) {
            var module = LayoutModel.ControlManager.GetModule(moduleId);

            return module != null && connectionDestination.ConnectionDescription.IsCompatibleWith(ControlConnectionPointTypes.OutputSolenoid) &&
                module.ConnectionPoints.GetConnectionPointType(index) == ControlConnectionPointTypes.OutputSolenoid;
        }

        //[LayoutEvent("can-control-be-connected", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='Massoth8156001_AsFunctionDecoder']")]
        [DispatchTarget]
        private bool CanControlBeConnected_FunctionDecoder(ControlConnectionPointDestination connectionDestination, Guid moduleId, int index, [DispatchFilter] string moduleTypeName = "Massoth8156001_AsFunctionDecoder") {
            var module = LayoutModel.ControlManager.GetModule(moduleId);

            if (module != null && connectionDestination.ConnectionDescription.IsCompatibleWith(ControlConnectionPointTypes.OutputSolenoid, ControlConnectionPointTypes.OutputOnOff)) {
                switch (module.ConnectionPoints.GetConnectionPointType(index)) {
                    case ControlConnectionPointTypes.OutputSolenoid:
                    case ControlConnectionPointTypes.OutputOnOff:
                    case ControlConnectionPointTypes.OutputRelay:
                        return true;
                }
            }

            return false;
        }

        [DispatchTarget]
        private void RecommendControlModuleTypes_DCC(ControlConnectionPointDestination connectionDestination, List<string> moduleTypeNames, string busFamilyName, [DispatchFilter] string busTypeName = "DCC") {
            if (connectionDestination.ConnectionDescription.IsCompatibleWith("Control", ControlConnectionPointTypes.OutputSolenoid)) {
                moduleTypeNames.Add("Massoth8156001");          // 4 switch decoders
                moduleTypeNames.Add("Massoth8156501");          // single switch decoder
                moduleTypeNames.Add("LGB55025");
            }
        }

        [DispatchTarget]
        private void RecommendControlModuleTypes_LocoBus(ControlConnectionPointDestination connectionDestination, List<string> moduleTypeNames, string busFamilyName, [DispatchFilter] string busTypeName = "LocoBus") {
            if (connectionDestination.ConnectionDescription.IsCompatibleWith("Control", ControlConnectionPointTypes.OutputSolenoid)) {
                moduleTypeNames.Add("Massoth8156001_AsFunctionDecoder");
            }
        }

        [LayoutEvent("edit-action-settings", InfoType = typeof(IMassothIgnoreFeedback))]
        private void EditMassothIgnoreFeedback(LayoutEvent e0) {
            var e = (LayoutEventResultValueType<object, ILayoutAction, bool>)e0;
            var programmingAction = Ensure.NotNull<ILayoutProgrammingAction>(e.Info, "programmingAction");

            switch (MessageBox.Show(null, "Did you connect load (e.g. turnout) to SW1 output?", "Is load connected", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)) {
                case DialogResult.Cancel:
                    e.Result = false;
                    break;

                case DialogResult.Yes:
                    programmingAction.IgnoreNoResponseResult = false;
                    e.Result = true;
                    break;

                case DialogResult.No:
                    programmingAction.IgnoreNoResponseResult = true;
                    e.Result = true;
                    break;
            }
        }
    }
}
