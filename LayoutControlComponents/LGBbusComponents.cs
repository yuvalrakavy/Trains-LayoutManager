using LayoutManager.CommonUI;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.ControlComponents {
    [LayoutModule("LGBBUS Control Components")]
    internal class LGBBUScomponents : LayoutModuleBase {

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_LGB55070([DispatchFilter("RegEx", "(LGB55070|ALL)")] string moduleTypeName) {
            var moduleType = new ControlModuleType("LGB55070", "LGB Feedback Interface") {
                AddressAlignment = 4,
                ConnectionPointsPerAddress = 2,
                NumberOfAddresses = 4,
                DefaultControlConnectionPointType = ControlConnectionPointTypes.InputDryTrigger,
                ConnectionPointIndexBase = 0,
                ConnectionPointLabelFormat = ControlConnectionPointLabelFormatOptions.Alpha | ControlConnectionPointLabelFormatOptions.AlphaLowercase | ControlConnectionPointLabelFormatOptions.AttachAddress,
                LastAddress = 256,
            };
            moduleType.AddBusTypes("LGBBUS");

            return moduleType;
        }

        [DispatchTarget]
        private ControlModuleType GetControlModuleType_LGB55075([DispatchFilter("RegEx", "(LGB55075|ALL)")] string moduleTypeName) {
            var moduleType = new ControlModuleType("LGB55075", "LGB Train detection module") {
                AddressAlignment = 4,
                ConnectionPointsPerAddress = 1,
                NumberOfAddresses = 4,
                DefaultControlConnectionPointType = ControlConnectionPointTypes.InputCurrent,
                ConnectionPointIndexBase = 0,
                LastAddress = 256,
            };

            moduleType.AddBusTypes("LGBBUS");

            return moduleType;
        }

        [DispatchTarget]
        private void RecommendControlModuleTypes_LGBBUS(ControlConnectionPointDestination connectionDestination, List<string> moduleTypeNames, string busFamilyName, [DispatchFilter] string busTypeName = "LGBBUS") {
            if (connectionDestination.ConnectionDescription.IsCompatibleWith("CurrentSensor"))
                moduleTypeNames.Add("LGB55075");
            else if (connectionDestination.ConnectionDescription.IsCompatibleWith("Feedback", "DryContact"))
                moduleTypeNames.Add("LGB55070");
        }

        [DispatchTarget(Order = 100)]
        private void AddControlModuleEditingContextMenuEntries([DispatchFilter] DrawControlModule drawModule, MenuOrMenuItem menu) {
            ControlModuleType moduleType = drawModule.Module.ModuleType;

            if (moduleType.TypeName == "LGB55070" || moduleType.TypeName == "LGB55075") {
                if (menu.Items.Count > 0 && menu.Items[menu.Items.Count - 1] is not ToolStripSeparator)
                    menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(new LGBmoduleSettingMenuItem(drawModule.Module));
            }
        }

        private class LGBmoduleSettingMenuItem : LayoutMenuItem {
            private readonly ControlModule module;

            public LGBmoduleSettingMenuItem(ControlModule module) {
                this.module = module;

                Text = "Setup...";
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                var d = new Dialogs.LGBbusDIPswitchSetting(module.ModuleType.Name, module.Address, module.UserActionRequired);

                d.ShowDialog(LayoutController.ActiveFrameWindow);

                if (d.ClearUserActionRequiredFlag) {
                    var command = new SetControlUserActionRequiredCommand(module, false);

                    LayoutController.Do(command);
                }
            }
        }
    }
}
