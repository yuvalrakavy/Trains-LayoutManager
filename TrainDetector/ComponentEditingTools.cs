using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;

using LayoutManager;
using LayoutManager.Model;
using System.Linq;
using LayoutManager.CommonUI;
using LayoutManager.ControlComponents;

#pragma warning disable IDE0051, CA1031
namespace TrainDetector {

    [LayoutModule("Train Detector Component Editing Tool", UserControl = false)]
    public class ComponentTool : System.ComponentModel.Component, ILayoutModuleSetup {
        #region Implementation of ILayoutModuleSetup
        public LayoutModule Module { set; get; }

        #endregion

        [LayoutEvent("model-component-placement-request", SenderType = typeof(TrainDetectorsComponent))]
        private void PlaceNumatoTrainDetectpr(LayoutEvent e) {
            var component = Ensure.NotNull<TrainDetectorsComponent>(e.Sender);

            if (LayoutModel.Components<TrainDetectorsComponent>(LayoutPhase.All).Count() > 0) {
                MessageBox.Show("Layout cannot contain more than one 'Train Detector' component", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Info = false;
            }
            else {
                using var propertiesDialog = new Dialogs.TrainDetectorProperties(component, initialPlacment: true);

                if (propertiesDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    component.XmlInfo.XmlDocument = propertiesDialog.XmlInfo.XmlDocument;
                    if(propertiesDialog.AutoDetect) { }

                    e.Info = true;      // Place component
                }
                else
                    e.Info = false;     // Do not place component
            }
        }

        [LayoutEvent("model-component-post-placement-request", SenderType =typeof(TrainDetectorsComponent))]
        private async void checkIfAutoDetect(LayoutEvent e) {
            var component = Ensure.NotNull<TrainDetectorsComponent>(e.Sender);

            if(component.Info.AutoDetect) {
                var detector = new TrainDetectorControllersDetection(component.TrainDetectorsBus, null);

                var result = await detector.UpdateBus();
                component.Info.AutoDetect = false;

                if (!result.IsEmpty)
                    MessageBox.Show(result.ToString(), "Train Detector Controllers", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        [LayoutEvent("query-component-editing-context-menu", SenderType = typeof(TrainDetectorsComponent))]
        private void QueryEditingMenu(LayoutEvent e) {
            e.Info = e.Sender;
        }


        [LayoutEvent("add-component-editing-context-menu-entries", SenderType = typeof(TrainDetectorsComponent))]
        private void AddEditingContextMenuEntries(LayoutEvent e) {
            var menu = Ensure.NotNull<Menu>(e.Info);
            var component = Ensure.NotNull<TrainDetectorsComponent>(e.Sender);
            var moduleLocationId = (Guid?)e.GetOption("ModuleLocationId");

            menu.MenuItems.Add("Detect controllers", async (s, ea) => {
                var detector = new TrainDetectorControllersDetection(component.TrainDetectorsBus, moduleLocationId);
                var result = await detector.UpdateBus();

                if (!result.IsEmpty)
                    MessageBox.Show(result.ToString(), "Train Detector Controllers", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Train Detectors are up-to-date", "Train Detector Controllers", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });

            menu.MenuItems.Add("&Properties", (s, ea) => {
                var d = new Dialogs.TrainDetectorProperties(component, initialPlacment: false);

                if (d.ShowDialog() == DialogResult.OK)
                    LayoutController.Do(new LayoutModifyComponentDocumentCommand(component, d.XmlInfo));
            });
        }

        [LayoutEvent("click-to-add-train-detector")]
        private void clickToAddTrainDetector(LayoutEvent e) {
            var drawObject = Ensure.NotNull<DrawControlClickToAddModule>(e.Sender);

            using var d = new Dialogs.AddTrainDetectorController(name => 
                string.IsNullOrWhiteSpace(name) ? "You must specify a non-empty name" :
                drawObject.Bus.Modules.Any(module => module.Label == name) ? $"A module named '{name}' is already defined" : null
            );

            if(d.ShowDialog() == DialogResult.OK) {
                var bus = drawObject.Bus;
                var moduleType = bus.BusType.ModuleTypes.First();
                var address = drawObject.Bus.AllocateFreeAddress(moduleType, drawObject.Viewer.ModuleLocationID);

                var command = new LayoutCompoundCommand("Add Train detector controller", executeAtOnce: true);
                var addCommand = new AddControlModuleCommand(drawObject.Bus, moduleType.TypeName, drawObject.Viewer.ModuleLocationID, address);

                command.Add(addCommand);
                command.Add(new SetControlModuleConnectionPointsCount(addCommand.AddedModule, d.SensorsCount));
                command.Add(new SetControlModuleLabelCommand(addCommand.AddedModule, d.ControllerName));

                LayoutController.Do(command);
            }
        }

        [LayoutEvent("control-module-label-changed", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='TrainDetectorController']")]
        private async void moduleNameChanged(LayoutEvent e) {
            (var module, var name) = Ensure.NotNull<ControlModule, string>(e);
            var controller = new TrainDetectorControllerModule(module);

            // If assigned - program the controller with the new name
            if(controller.ControllerIpAddress != null) {
                using var networkHandler = new NetworkHandler(rawPacket => rawPacket.GetPacketHeader());

                try {
                    networkHandler.Start();
                    await networkHandler.Request((requestNumber) => networkHandler.SendPacketAsync(new ConfigSetRequestPacket(requestNumber, "Name", name), controller.ControllerIpAddress));
                } catch(TimeoutException) {
                    MessageBox.Show("Error: could not program train detector controller with the new name (it is online?)", "Timeout error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        [LayoutEvent("validate-control-module-label", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='TrainDetectorController']")]
        private void validateControlModuleLabel(LayoutEvent e) {
            var module = Ensure.NotNull<ControlModule>(e.Sender);
            var label = (string)e.GetOption("Label");

            if (string.IsNullOrWhiteSpace(label))
                e.Info = "Module label cannot be empty";

            if (module.Bus.Modules.Any(otherModule => otherModule.Id != module.Id && otherModule.Label == label))
                e.Info = $"A module with the entered label ({label}), is already defined";
        }

    }
}
