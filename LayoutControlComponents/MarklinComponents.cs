using LayoutManager.CommonUI;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.ControlComponents {
    public interface IMarklinControlModuleSettingDialog {
        bool ClearUserActionRequiredFlag { get; }

        DialogResult ShowDialog(IWin32Window owner);
    }

    [LayoutModule("Marklin Control Components")]
    internal class MarklinControlComponents : LayoutModuleBase {
        private ControlBusType? motorola = null;
        private ControlBusType? s88bus = null;

        [DispatchTarget]
        private ControlBusType GetControlBusType_Motorola([DispatchFilter] string busTypeName = "Motorola") {
            return motorola ??= new ControlBusType {
                BusFamilyName = "Motorola",
                BusTypeName = "Motorola",
                Name = "Tracks (Motorola)",
                Topology = ControlBusTopology.RandomAddressing,
                FirstAddress = 1,
                LastAddress = 256,
                Usage = ControlConnectionPointUsage.Output
            };
        }

        [DispatchTarget]
        private ControlBusType GetControlBusType_S88BUS([DispatchFilter] string busTypeName = "S88BUS") {
            return s88bus ??= new ControlBusType {
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

        [DispatchTarget]
        private void RecommendControlModuleTypes_Motorola(ControlConnectionPointDestination connectionDestination, List<string> moduleTypeNames, string busFamilyName, [DispatchFilter] string busTypeName = "Motorola") {
            if (connectionDestination.ConnectionDescription.IsCompatibleWith("Control", "Solenoid")) {
                moduleTypeNames.Add("K83");
                moduleTypeNames.Add("K73");
                moduleTypeNames.Add("74460");
            }
        }

        [DispatchTarget]
        private void RecommendControlModuleTypes_S88BUS(ControlConnectionPointDestination connectionDestination, List<string> moduleTypeNames, string busFamilyName, [DispatchFilter] string busTypeName = "S88BUS") {
            if (connectionDestination.ConnectionDescription.IsCompatibleWith("Feedback", "DryContact"))
                moduleTypeNames.Add("S88");
        }

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_S88([DispatchFilter("RegEx", "(S88|ALL)")] string moduleTypeName) {
            var moduleType = new ControlModuleType("S88", "S88 Decoder") {
                ConnectionPointsPerAddress = 16,
                ConnectionPointIndexBase = 1,
                DefaultControlConnectionPointType = ControlConnectionPointTypes.InputDryTrigger,
                NumberOfAddresses = 1,
                ConnectionPointLabelFormat = ControlConnectionPointLabelFormatOptions.NoAttachedAddress,
                ConnectionPointArrangement = ControlModuleConnectionPointArrangementOptions.BothRows | ControlModuleConnectionPointArrangementOptions.TopRightToLeft,
            };

            moduleType.BusTypeNames.Add("S88BUS");
            return moduleType;
        }

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_K83([DispatchFilter("RegEx", "(K83|ALL)")] string moduleTypeName) {
            var moduleType = new ControlModuleType("K83", "K83 Turnouts Decoder") {
                ConnectionPointsPerAddress = 1,
                DefaultControlConnectionPointType = ControlConnectionPointTypes.OutputSolenoid,
                NumberOfAddresses = 4,
            };

            moduleType.BusTypeNames.Add("Motorola");
            return moduleType;
        }

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_K73([DispatchFilter("RegEx", "(K73|ALL)")] string moduleTypeName) {
            var moduleType = new ControlModuleType("K73", "K73 Turnout Decoder") {
                ConnectionPointsPerAddress = 1,
                DefaultControlConnectionPointType = ControlConnectionPointTypes.OutputSolenoid,
                NumberOfAddresses = 1,
            };

            moduleType.BusTypeNames.Add("Motorola");
            return moduleType;
        }

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_K84([DispatchFilter("RegEx", "(K84|ALL)")] string moduleTypeName) {
            var moduleType = new ControlModuleType("K84", "K84 Accessories Decoder") {
                ConnectionPointsPerAddress = 1,
                DefaultControlConnectionPointType = ControlConnectionPointTypes.OutputRelay,
                NumberOfAddresses = 4,
            };

            moduleType.BusTypeNames.Add("Motorola");
            return moduleType;
        }

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_74460([DispatchFilter("RegEx", "(74460|ALL)")] string moduleTypeName) {
            var moduleType = new ControlModuleType("74460", "C-Track Turnout Decoder") {
                ConnectionPointsPerAddress = 1,
                DefaultControlConnectionPointType = ControlConnectionPointTypes.OutputSolenoid,
                NumberOfAddresses = 1,
                BuiltIn = true,
            };

            moduleType.BusTypeNames.Add("Motorola");
            return moduleType;
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