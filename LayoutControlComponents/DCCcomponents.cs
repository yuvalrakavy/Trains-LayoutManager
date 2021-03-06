using System.Collections.Generic;
using System.Xml;
using System.Windows.Forms;
using LayoutManager.Model;

#pragma warning disable IDE0051
#nullable enable
namespace LayoutManager.ControlComponents {
    public class ControlComponents {
        public static string ControlComponentsVersion = "1.0";
    }

    [LayoutModule("DCC Control Components")]
    class DCCconrolComponents : LayoutModuleBase {
        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='LGB55025']")]
        [LayoutEvent("enum-control-module-types")]
        private void get55025(LayoutEvent e) {
            var parentElement = Ensure.NotNull<XmlElement>(e.Sender, "parentElement");

            ControlModuleType moduleType = new ControlModuleType(parentElement, "LGB55025", "LGB Switch Decoder") {
                AddressAlignment = 4
            };
            EventManager.Event(new LayoutEvent("add-bus-connectable-to-module", moduleType).SetOption("GenericBusType", "DCC"));

            moduleType.ConnectionPointsPerAddress = 1;
            moduleType.NumberOfAddresses = 4;
            moduleType.LastAddress = 128;
        }

        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='Massoth8156001']")]
        [LayoutEvent("enum-control-module-types")]
        private void GetMassoth8156001(LayoutEvent e) {
            var parentElement = Ensure.NotNull<XmlElement>(e.Sender, "parentElement");

            ControlModuleType moduleType = new ControlModuleType(parentElement, "Massoth8156001", "Massoth Switch Decoder");

            EventManager.Event(new LayoutEvent("add-bus-connectable-to-module", moduleType).SetOption("GenericBusType", "DCC"));

            moduleType.AddressAlignment = 1;
            moduleType.ConnectionPointsPerAddress = 1;
            moduleType.NumberOfAddresses = 4;
            moduleType.LastAddress = 2048;
            moduleType.DecoderTypeName = "GenericDCC";
        }

        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='Massoth8156001_AsFunctionDecoder']")]
        [LayoutEvent("enum-control-module-types")]
        private void GetMassoth8156001_AsFunctionDecoder(LayoutEvent e) {
            var parentElement = Ensure.NotNull<XmlElement>(e.Sender, "parentElement");

            ControlModuleType moduleType = new ControlModuleType(parentElement, "Massoth8156001_AsFunctionDecoder", "Massoth Function Decoder");

            EventManager.Event(new LayoutEvent("add-bus-connectable-to-module", moduleType).SetOption("GenericBusType", "LocoBus"));

            moduleType.AddressAlignment = 1;
            moduleType.ConnectionPointsPerAddress = 8;
            moduleType.NumberOfAddresses = 1;
            moduleType.LastAddress = 10239;
            moduleType.ConnectionPointLabelFormat = ControlConnectionPointLabelFormatOptions.ConnectionPointIndex | ControlConnectionPointLabelFormatOptions.NoAttachedAddress;
            moduleType.ConnectionPointIndexBase = 1;
            moduleType.DefaultControlConnectionPointType = ControlConnectionPointTypes.OutputRelay;
            moduleType.DecoderTypeName = "GenericDCC";
        }

        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='Massoth8156501']")]
        [LayoutEvent("enum-control-module-types")]
        private void GetMassoth8156501(LayoutEvent e) {
            var parentElement = Ensure.NotNull<XmlElement>(e.Sender, "parentElement");

            ControlModuleType moduleType = new ControlModuleType(parentElement, "Massoth8156501", "Massoth Single Switch Decoder");

            EventManager.Event(new LayoutEvent("add-bus-connectable-to-module", moduleType).SetOption("GenericBusType", "DCC"));

            moduleType.AddressAlignment = 1;
            moduleType.ConnectionPointsPerAddress = 1;
            moduleType.NumberOfAddresses = 1;
            moduleType.LastAddress = 2048;
            moduleType.DecoderTypeName = "GenericDCC";
        }

        class ProgramMassothSwitchDecoder : LayoutDccProgrammingAction<ControlModule> {
            public ProgramMassothSwitchDecoder(XmlElement actionElement, ControlModule switchDecoder)
                : base(actionElement, switchDecoder) {
            }
        }

        interface IMassothIgnoreFeedback {
        }

        class ProgramMassothSwitchDecoderAddress : ProgramMassothSwitchDecoder, IMassothIgnoreFeedback {
            public ProgramMassothSwitchDecoderAddress(XmlElement actionElement, ControlModule switchDecoder)
                : base(actionElement, switchDecoder) {
            }

            public override void PrepareProgramming() {
                if (ProgrammingTarget.Bus.BusType.BusFamilyName == "DCC") {
                    // DCC bus - use switch decoders address space
                    SetCV(29, 0x80);

                    int address = ProgrammingTarget.Address;
                    for (int n = 0; n < ProgrammingTarget.ModuleType.NumberOfAddresses; n++) {
                        SetCV(31 + n * 2, (byte)((address >> 8) & 0xff));
                        SetCV(31 + n * 2 + 1, (byte)(address & 0xff));

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
                        if (ProgrammingTarget.ModuleType.NumberOfAddresses > 1)
                            return "Set address to switch address range " + ProgrammingTarget.Address + " to " + (ProgrammingTarget.Address + ProgrammingTarget.ModuleType.NumberOfAddresses - 1).ToString();
                        else
                            return "Set address to switch address " + ProgrammingTarget.Address;
                    }
                    else
                        return "Set address to locomotive bus address " + ProgrammingTarget.Address;
                }
            }
        }

        [LayoutEvent("query-action", IfEvent = "LayoutEvent[Options/@Action='set-address']", SenderType = typeof(ControlModule), IfSender = "*[starts-with(@ModuleTypeName, 'Massoth8156')]")]
        private void querySetMassothSwitchDecoderAddress(LayoutEvent e0) {
            var e = (LayoutEventInfoResultValueType<IHasDecoder, bool, bool>)e0;

            e.Result = true;
        }

        [LayoutEvent("get-action", IfSender = "Action[@Type='set-address']", InfoType = typeof(ControlModule), IfInfo = "*[starts-with(@ModuleTypeName, 'Massoth8156')]")]
        private void getProgramMassothSwitchDecoderAddressAction(LayoutEvent e) {
            var actionElement = Ensure.NotNull<XmlElement>(e.Sender, "actionElement");

            if (e.Info is ControlModule module)
                e.Info = new ProgramMassothSwitchDecoderAddress(actionElement, module);
        }

        [LayoutEvent("can-control-be-connected", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='LGB55025']")]
        [LayoutEvent("can-control-be-connected", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='Massoth8156001']")]
        [LayoutEvent("can-control-be-connected", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='Massoth8156501']")]
        private void canSwitchDecoderBeConnected(LayoutEvent e) {
            var connectionDestination = Ensure.NotNull<ControlConnectionPointDestination>(e.Sender, "connectionDestination");
            var module = LayoutModel.ControlManager.GetModule(XmlConvert.ToGuid(e.GetOption("ModuleID")));

            if (module != null && connectionDestination.ConnectionDescription.IsCompatibleWith(ControlConnectionPointTypes.OutputSolenoid)) {
                int index = XmlConvert.ToInt32(e.GetOption("Index"));

                e.Info = module.ConnectionPoints.GetConnectionPointType(index) == ControlConnectionPointTypes.OutputSolenoid;
            }
            else
                e.Info = false;
        }

        [LayoutEvent("can-control-be-connected", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='Massoth8156001_AsFunctionDecoder']")]
        private void canFunctionDecoderBeConnected(LayoutEvent e) {
            var connectionDestination = Ensure.NotNull<ControlConnectionPointDestination>(e.Sender, "connectionDestination");
            var module = LayoutModel.ControlManager.GetModule(XmlConvert.ToGuid(e.GetOption("ModuleID")));

            e.Info = false;

            if (module != null && connectionDestination.ConnectionDescription.IsCompatibleWith(ControlConnectionPointTypes.OutputSolenoid, ControlConnectionPointTypes.OutputOnOff)) {
                int index = XmlConvert.ToInt32(e.GetOption("Index"));

                switch (module.ConnectionPoints.GetConnectionPointType(index)) {
                    case ControlConnectionPointTypes.OutputSolenoid:
                    case ControlConnectionPointTypes.OutputOnOff:
                    case ControlConnectionPointTypes.OutputRelay:
                        e.Info = true;
                        break;

                }
            }
        }

        /// <summary>
        /// Recommend on a module type to be used for connecting a given component
        /// </summary>
        /// <param name="e"></param>
        [LayoutEvent("recommend-control-module-types", IfEvent = "LayoutEvent[./Options/@BusFamily='DCC']")]
        private void recommendDCCcontrolModuleType(LayoutEvent e) {
            var connectionDestination = Ensure.NotNull<ControlConnectionPointDestination>(e.Sender, "connectionDestination");
            var moduleTypeNames = Ensure.NotNull<IList<string>>(e.Info, "moduleTypeNames");

            if (connectionDestination.ConnectionDescription.IsCompatibleWith("Control", ControlConnectionPointTypes.OutputSolenoid)) {
                moduleTypeNames.Add("Massoth8156001");          // 4 switch decoders
                moduleTypeNames.Add("Massoth8156501");          // single switch decoder
                moduleTypeNames.Add("LGB55025");
            }
        }

        [LayoutEvent("recommend-control-module-types", IfEvent = "LayoutEvent[./Options/@BusFamily='LocoBus']")]
        private void recommendLocoBuscontrolModuleType(LayoutEvent e) {
            var connectionDestination = Ensure.NotNull<ControlConnectionPointDestination>(e.Sender, "connectionDestination");
            var moduleTypeNames = Ensure.NotNull<IList<string>>(e.Info, "moduleTypeNames");

            if (connectionDestination.ConnectionDescription.IsCompatibleWith("Control", ControlConnectionPointTypes.OutputSolenoid)) {
                moduleTypeNames.Add("Massoth8156001_AsFunctionDecoder");
            }
        }

        [LayoutEvent("edit-action-settings", InfoType = typeof(IMassothIgnoreFeedback))]
        private void editMassothIgnoreFeedback(LayoutEvent e0) {
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
