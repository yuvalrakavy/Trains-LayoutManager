using System;
using System.Collections.Generic;
using System.Xml;
using System.Windows.Forms;
using LayoutManager.Model;
using LayoutManager.CommonUI;

namespace LayoutManager.ControlComponents {
    [LayoutModule("LGBBUS Control Components")]
    internal class LGBBUScomponents : LayoutModuleBase {
        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='LGB55070']")]
        [LayoutEvent("enum-control-module-types")]
        private void Get55070(LayoutEvent e) {
            var parentElement = Ensure.NotNull<XmlElement>(e.Sender, "parentElement");

            var moduleType = new ControlModuleType(parentElement, "LGB55070", "LGB Feedback Interface") {
                AddressAlignment = 4
            };
            EventManager.Event(new LayoutEvent("add-bus-connectable-to-module", moduleType).SetOption("GenericBusType", "LGBBUS"));
            moduleType.ConnectionPointsPerAddress = 2;
            moduleType.NumberOfAddresses = 4;
            moduleType.DefaultControlConnectionPointType = ControlConnectionPointTypes.InputDryTrigger;
            moduleType.ConnectionPointIndexBase = 0;
            moduleType.ConnectionPointLabelFormat = ControlConnectionPointLabelFormatOptions.Alpha | ControlConnectionPointLabelFormatOptions.AlphaLowercase | ControlConnectionPointLabelFormatOptions.AttachAddress;
            moduleType.LastAddress = 256;
        }

        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='LGB55075']")]
        [LayoutEvent("enum-control-module-types")]
        private void Get55075(LayoutEvent e) {
            var parentElement = Ensure.NotNull<XmlElement>(e.Sender, "parentElement");

            var moduleType = new ControlModuleType(parentElement, "LGB55075", "LGB Train detection module") {
                AddressAlignment = 4
            };
            EventManager.Event(new LayoutEvent("add-bus-connectable-to-module", moduleType).SetOption("GenericBusType", "LGBBUS"));
            moduleType.ConnectionPointsPerAddress = 1;
            moduleType.NumberOfAddresses = 4;
            moduleType.DefaultControlConnectionPointType = ControlConnectionPointTypes.InputCurrent;
            moduleType.ConnectionPointIndexBase = 0;
            moduleType.LastAddress = 256;
        }

        [LayoutEvent("recommend-control-module-types", IfEvent = "LayoutEvent[./Options/@BusFamily='LGBBUS']")]
        private void RecommendLGBcompatibleBusControlModuleType(LayoutEvent e) {
            var connectionDestination = Ensure.NotNull<ControlConnectionPointDestination>(e.Sender, "connectionDestination");
            var moduleTypeNames = Ensure.NotNull<IList<string>>(e.Info, "moduleTypeNames");

            if (connectionDestination.ConnectionDescription.IsCompatibleWith("CurrentSensor"))
                moduleTypeNames.Add("LGB55075");
            else if (connectionDestination.ConnectionDescription.IsCompatibleWith("Feedback", "DryContact"))
                moduleTypeNames.Add("LGB55070");
        }

        [LayoutEvent("add-control-editing-context-menu-entries", Order = 100, SenderType = typeof(DrawControlModule))]
        private void AddControlModuleEditingContextMenu(LayoutEvent e) {
            var drawModule = Ensure.NotNull<DrawControlModule>(e.Sender, "drawModule");
            var menu = Ensure.ValueNotNull<MenuOrMenuItem>(e.Info, "menu");
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
