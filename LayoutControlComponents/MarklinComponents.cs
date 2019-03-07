using System;
using System.Collections.Generic;
using System.Xml;
using System.Windows.Forms;
using LayoutManager.Model;
using LayoutManager.CommonUI;

namespace LayoutManager.ControlComponents {

    public interface IMarklinControlModuleSettingDialog {
        bool ClearUserActionRequiredFlag { get; }

        DialogResult ShowDialog(IWin32Window owner);
    }

    [LayoutModule("Marklin Control Components")]
    class MarklinControlComponents : LayoutModuleBase {
        ControlBusType motorola = null;
        ControlBusType s88bus = null;

        [LayoutEvent("get-control-bus-type", IfEvent = "LayoutEvent[./Options/@BusTypeName='Motorola']")]
        private void getMotorolaBusType(LayoutEvent e) {
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
        private void gets88busType(LayoutEvent e) {
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
        private void recommendMotorolaDCCcontrolModuleTypes(LayoutEvent e) {
            ControlConnectionPointDestination connectionDestination = (ControlConnectionPointDestination)e.Sender;
            IList<string> moduleTypeNames = (IList<string>)e.Info;

            if (connectionDestination.ConnectionDescription.IsCompatibleWith("Control", "Solenoid")) {
                moduleTypeNames.Add("K83");
                moduleTypeNames.Add("K73");
                moduleTypeNames.Add("74460");
            }
        }

        [LayoutEvent("recommend-control-module-types", IfEvent = "LayoutEvent[./Options/@BusFamily='S88BUS']")]
        private void recommendS88BUScontrolModuleTypes(LayoutEvent e) {
            ControlConnectionPointDestination connectionDestination = (ControlConnectionPointDestination)e.Sender;
            IList<string> moduleTypeNames = (IList<string>)e.Info;
            string connectionName = connectionDestination.ConnectionDescription.Name;

            if (connectionDestination.ConnectionDescription.IsCompatibleWith("Feedback", "DryContact"))
                moduleTypeNames.Add("S88");
        }

        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='S88']")]
        [LayoutEvent("enum-control-module-types")]
        private void getS88(LayoutEvent e) {
            XmlElement parentElement = (XmlElement)e.Sender;

            ControlModuleType moduleType = new ControlModuleType(parentElement, "S88", "S88 Decoder");

            moduleType.BusTypeNames.Add("S88BUS");
            moduleType.ConnectionPointsPerAddress = 16;
            moduleType.ConnectionPointIndexBase = 1;
            moduleType.DefaultControlConnectionPointType = ControlConnectionPointTypes.InputDryContact;
            moduleType.NumberOfAddresses = 1;
            moduleType.ConnectionPointLabelFormat = ControlConnectionPointLabelFormatOptions.NoAttachedAddress;
            moduleType.ConnectionPointArrangement = ControlModuleConnectionPointArrangementOptions.BothRows | ControlModuleConnectionPointArrangementOptions.TopRightToLeft;
        }

        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='K83']")]
        [LayoutEvent("enum-control-module-types")]
        private void getK83(LayoutEvent e) {
            XmlElement parentElement = (XmlElement)e.Sender;

            ControlModuleType moduleType = new ControlModuleType(parentElement, "K83", "K83 Turnouts Decoder");

            moduleType.BusTypeNames.Add("Motorola");
            moduleType.ConnectionPointsPerAddress = 1;
            moduleType.DefaultControlConnectionPointType = ControlConnectionPointTypes.OutputSolenoid;
            moduleType.NumberOfAddresses = 4;
        }

        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='K73']")]
        [LayoutEvent("enum-control-module-types")]
        private void getK73(LayoutEvent e) {
            XmlElement parentElement = (XmlElement)e.Sender;

            ControlModuleType moduleType = new ControlModuleType(parentElement, "K73", "K73 Turnout Decoder");

            moduleType.BusTypeNames.Add("Motorola");
            moduleType.ConnectionPointsPerAddress = 1;
            moduleType.DefaultControlConnectionPointType = ControlConnectionPointTypes.OutputSolenoid;
            moduleType.NumberOfAddresses = 1;
        }

        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='K84']")]
        [LayoutEvent("enum-control-module-types")]
        private void getK84(LayoutEvent e) {
            XmlElement parentElement = (XmlElement)e.Sender;

            ControlModuleType moduleType = new ControlModuleType(parentElement, "K84", "K84 Accessories Decoder");

            moduleType.BusTypeNames.Add("Motorola");
            moduleType.ConnectionPointsPerAddress = 1;
            moduleType.DefaultControlConnectionPointType = ControlConnectionPointTypes.OutputRelay;
            moduleType.NumberOfAddresses = 4;
        }

        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='74460']")]
        [LayoutEvent("enum-control-module-types")]
        private void get74460(LayoutEvent e) {
            XmlElement parentElement = (XmlElement)e.Sender;

            ControlModuleType moduleType = new ControlModuleType(parentElement, "74460", "C-Track Turnout Decoder");

            moduleType.BusTypeNames.Add("Motorola");
            moduleType.ConnectionPointsPerAddress = 1;
            moduleType.DefaultControlConnectionPointType = ControlConnectionPointTypes.OutputSolenoid;
            moduleType.NumberOfAddresses = 1;
            moduleType.BuiltIn = true;
        }

        [LayoutEvent("add-control-editing-context-menu-entries", Order = 100, SenderType = typeof(DrawControlModule))]
        private void addControlModuleEditingContextMenu(LayoutEvent e) {
            DrawControlModule drawModule = (DrawControlModule)e.Sender;
            Menu menu = (Menu)e.Info;
            ControlModuleType moduleType = drawModule.Module.ModuleType;

            if (moduleType.TypeName == "K83" || moduleType.TypeName == "K84" || moduleType.TypeName == "74460") {
                if (menu.MenuItems.Count > 0 && menu.MenuItems[menu.MenuItems.Count - 1].Text != "-")
                    menu.MenuItems.Add("-");
                menu.MenuItems.Add(new MarklinKxxModuleSettingMenuItem(drawModule.Module));
            }
        }

        class MarklinKxxModuleSettingMenuItem : MenuItem {
            readonly ControlModule module;

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
                    SetControlUserActionRequiredCommand command = new SetControlUserActionRequiredCommand(module, false);

                    LayoutController.Do(command);
                }
            }
        }

    }
}