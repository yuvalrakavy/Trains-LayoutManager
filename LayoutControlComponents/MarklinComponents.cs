using System;
using System.Collections.Generic;
using System.Xml;
using System.Windows.Forms;
using MethodDispatcher;
using LayoutManager.Model;
using LayoutManager.CommonUI;

namespace LayoutManager.ControlComponents {
    public interface IMarklinControlModuleSettingDialog {
        bool ClearUserActionRequiredFlag { get; }

        DialogResult ShowDialog(IWin32Window owner);
    }

    [LayoutModule("Marklin Control Components")]
    internal class MarklinControlComponents : LayoutModuleBase {
        private ControlBusType? motorola = null;
        private ControlBusType? s88bus = null;

        [LayoutEvent("get-control-bus-type", IfEvent = "LayoutEvent[./Options/@BusTypeName='Motorola']")]
        private void GetMotorolaBusType(LayoutEvent e) {
            if (motorola == null) {
                motorola = new ControlBusType {
                    BusFamilyName = "Motorola",
                    BusTypeName = "Motorola",
                    Name = "Tracks (Motorola)",
                    Topology = ControlBusTopology.RandomAddressing,
                    FirstAddress = 1,
                    LastAddress = 256,
                    Usage = ControlConnectionPointUsage.Output
                };
            }

            e.Info = motorola;
        }

        [LayoutEvent("get-control-bus-type", IfEvent = "LayoutEvent[./Options/@BusTypeName='S88BUS']")]
        private void Gets88busType(LayoutEvent e) {
            if (s88bus == null) {
                s88bus = new ControlBusType {
                    BusFamilyName = "S88BUS",
                    BusTypeName = "S88BUS",
                    Name = "S88 Bus",
                    Topology = ControlBusTopology.DaisyChain,
                    AddressingMethod = ControlAddressingMethod.ModuleConnectionPointAddressing,
                    FirstAddress = 1,
                    LastAddress = 31,
                    Usage = ControlConnectionPointUsage.Input
                };
            }

            e.Info = s88bus;
        }

        [LayoutEvent("recommend-control-module-types", IfEvent = "LayoutEvent[./Options/@BusFamily='Motorola']")]
        private void RecommendMotorolaDCCcontrolModuleTypes(LayoutEvent e) {
            var connectionDestination = Ensure.NotNull<ControlConnectionPointDestination>(e.Sender, "connectionDestination");
            var moduleTypeNames = Ensure.NotNull<IList<string>>(e.Info, "moduleTypeNames");

            if (connectionDestination.ConnectionDescription.IsCompatibleWith("Control", "Solenoid")) {
                moduleTypeNames.Add("K83");
                moduleTypeNames.Add("K73");
                moduleTypeNames.Add("74460");
            }
        }

        [LayoutEvent("recommend-control-module-types", IfEvent = "LayoutEvent[./Options/@BusFamily='S88BUS']")]
        private void RecommendS88BUScontrolModuleTypes(LayoutEvent e) {
            var connectionDestination = Ensure.NotNull<ControlConnectionPointDestination>(e.Sender, "connectionDestination");
            var moduleTypeNames = Ensure.NotNull<IList<string>>(e.Info, "moduleTypeNames");

            if (connectionDestination.ConnectionDescription.IsCompatibleWith("Feedback", "DryContact"))
                moduleTypeNames.Add("S88");
        }

        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='S88']")]
        [LayoutEvent("enum-control-module-types")]
        private void GetS88(LayoutEvent e) {
            var parentElement = Ensure.NotNull<XmlElement>(e.Sender, "parentElement");

            var moduleType = new ControlModuleType(parentElement, "S88", "S88 Decoder");

            moduleType.BusTypeNames.Add("S88BUS");
            moduleType.ConnectionPointsPerAddress = 16;
            moduleType.ConnectionPointIndexBase = 1;
            moduleType.DefaultControlConnectionPointType = ControlConnectionPointTypes.InputDryTrigger;
            moduleType.NumberOfAddresses = 1;
            moduleType.ConnectionPointLabelFormat = ControlConnectionPointLabelFormatOptions.NoAttachedAddress;
            moduleType.ConnectionPointArrangement = ControlModuleConnectionPointArrangementOptions.BothRows | ControlModuleConnectionPointArrangementOptions.TopRightToLeft;
        }

        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='K83']")]
        [LayoutEvent("enum-control-module-types")]
        private void GetK83(LayoutEvent e) {
            var parentElement = Ensure.NotNull<XmlElement>(e.Sender, "parentElement");

            var moduleType = new ControlModuleType(parentElement, "K83", "K83 Turnouts Decoder");

            moduleType.BusTypeNames.Add("Motorola");
            moduleType.ConnectionPointsPerAddress = 1;
            moduleType.DefaultControlConnectionPointType = ControlConnectionPointTypes.OutputSolenoid;
            moduleType.NumberOfAddresses = 4;
        }

        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='K73']")]
        [LayoutEvent("enum-control-module-types")]
        private void GetK73(LayoutEvent e) {
            var parentElement = Ensure.NotNull<XmlElement>(e.Sender, "parentElement");

            var moduleType = new ControlModuleType(parentElement, "K73", "K73 Turnout Decoder");

            moduleType.BusTypeNames.Add("Motorola");
            moduleType.ConnectionPointsPerAddress = 1;
            moduleType.DefaultControlConnectionPointType = ControlConnectionPointTypes.OutputSolenoid;
            moduleType.NumberOfAddresses = 1;
        }

        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='K84']")]
        [LayoutEvent("enum-control-module-types")]
        private void GetK84(LayoutEvent e) {
            var parentElement = Ensure.NotNull<XmlElement>(e.Sender, "parentElement");

            var moduleType = new ControlModuleType(parentElement, "K84", "K84 Accessories Decoder");

            moduleType.BusTypeNames.Add("Motorola");
            moduleType.ConnectionPointsPerAddress = 1;
            moduleType.DefaultControlConnectionPointType = ControlConnectionPointTypes.OutputRelay;
            moduleType.NumberOfAddresses = 4;
        }

        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='74460']")]
        [LayoutEvent("enum-control-module-types")]
        private void Get74460(LayoutEvent e) {
            var parentElement = Ensure.NotNull<XmlElement>(e.Sender, "parentElement");

            var moduleType = new ControlModuleType(parentElement, "74460", "C-Track Turnout Decoder");

            moduleType.BusTypeNames.Add("Motorola");
            moduleType.ConnectionPointsPerAddress = 1;
            moduleType.DefaultControlConnectionPointType = ControlConnectionPointTypes.OutputSolenoid;
            moduleType.NumberOfAddresses = 1;
            moduleType.BuiltIn = true;
        }

        [DispatchTarget(Order = 100)]
        private void AddControlModuleEditingContextMenuEntries([DispatchFilter] DrawControlModule drawModule, MenuOrMenuItem menu) {
            ControlModuleType moduleType = drawModule.Module.ModuleType;

            if (moduleType.TypeName == "K83" || moduleType.TypeName == "K84" || moduleType.TypeName == "74460") {
                if (menu.Items.Count > 0 && menu.Items[menu.Items.Count - 1] is not ToolStripSeparator)
                    menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(new MarklinKxxModuleSettingMenuItem(drawModule.Module));
            }
        }

        private class MarklinKxxModuleSettingMenuItem : LayoutMenuItem {
            private readonly ControlModule module;

            public MarklinKxxModuleSettingMenuItem(ControlModule module) {
                this.module = module;

                Text = "Setup...";
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                IMarklinControlModuleSettingDialog d;

                if (module.ModuleTypeName == "74460")
                    d = new Dialogs.TurnoutDecoderDIPswitchSetting(module.ModuleType.Name, module.Address, module.UserActionRequired);
                else
                    d = new Dialogs.KxxDIPswitchSetting(module.ModuleType.Name, module.Address, module.UserActionRequired);

                d.ShowDialog(LayoutController.ActiveFrameWindow);

                if (d.ClearUserActionRequiredFlag) {
                    var command = new SetControlUserActionRequiredCommand(module, false);

                    LayoutController.Do(command);
                }
            }
        }
    }
}