using LayoutManager;
using LayoutManager.CommonUI;
using LayoutManager.ControlComponents;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Linq;
using System.Windows.Forms;

namespace TrainDetector {

    [LayoutModule("Train Detector Component Editing Tool", UserControl = false)]
    public class ComponentTool : System.ComponentModel.Component, ILayoutModuleSetup {

        [DispatchTarget]
        bool RequestModelComponentPlacement([DispatchFilter] TrainDetectorsComponent component, PlacementInfo placement) {
            if (LayoutModel.Components<TrainDetectorsComponent>(LayoutPhase.All).Any()) {
                MessageBox.Show("Layout cannot contain more than one 'Train Detector' component", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else {
                using var propertiesDialog = new Dialogs.TrainDetectorProperties(component, initialPlacment: true);

                if (propertiesDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    component.XmlInfo.XmlDocument = propertiesDialog.XmlInfo.XmlDocument;
                    if (propertiesDialog.AutoDetect) { }

                    return true;      // Place component
                }
                else
                    return false;     // Do not place component
            }
        }

        [DispatchTarget]
        async void OnModelComponentPlacedNotification([DispatchFilter] TrainDetectorsComponent component, ILayoutCompoundCommand command, PlacementInfo placement) {
            if (component.Info.AutoDetect) {
                var detector = new TrainDetectorControllersDetection(component.TrainDetectorsBus, null);

                var result = await detector.UpdateBus();
                component.Info.AutoDetect = false;

                if (!result.IsEmpty)
                    MessageBox.Show(result.ToString(), "Train Detector Controllers", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        [DispatchTarget]
        [DispatchFilter("InDesignMode")]
        private bool IncludeInComponentContextMenu([DispatchFilter] TrainDetectorsComponent _) => true;

        [DispatchTarget]
        [DispatchFilter("InDesignMode")]
        private void AddComponentInModuleLocationContextMenuEntries(Guid frameWindowId, [DispatchFilter] TrainDetectorsComponent component, Guid moduleLocationId, MenuOrMenuItem menu) {
            menu.Items.Add("Detect controllers", null, async (s, ea) => {
                var detector = new TrainDetectorControllersDetection(component.TrainDetectorsBus, moduleLocationId);
                var result = await detector.UpdateBus();

                if (!result.IsEmpty)
                    MessageBox.Show(result.ToString(), "Train Detector Controllers", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Train Detectors are up-to-date", "Train Detector Controllers", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });

            menu.Items.Add("&Properties", null, (s, ea) => {
                var d = new Dialogs.TrainDetectorProperties(component, initialPlacment: false);

                if (d.ShowDialog() == DialogResult.OK)
                    LayoutController.Do(new LayoutModifyComponentDocumentCommand(component, d.XmlInfo));
            });
        }

        [DispatchTarget]
        private void ClickToAddTrainDetector(DrawControlClickToAddModule drawObject) {
            using var d = new Dialogs.AddTrainDetectorController(name =>
                string.IsNullOrWhiteSpace(name) ? "You must specify a non-empty name" :
                drawObject.Bus.Modules.Any(module => module.Label == name) ? $"A module named '{name}' is already defined" : null
            );

            if (d.ShowDialog() == DialogResult.OK) {
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

        [DispatchTarget]
        private async void OnControlModuleLabelChanged([DispatchFilter("ModuleType", "TrainDetectorController")] ControlModule module, string? name) {
            var controller = new TrainDetectorControllerModule(module);

            name ??= string.Empty;

            // If assigned - program the controller with the new name
            if (controller.ControllerIpAddress != null) {
                using var networkHandler = new NetworkHandler(rawPacket => rawPacket.GetPacketHeader());

                try {
                    networkHandler.Start();
                    await networkHandler.Request((requestNumber) => networkHandler.SendPacketAsync(new ConfigSetRequestPacket(requestNumber, "Name", name), controller.ControllerIpAddress));
                }
                catch (TimeoutException) {
                    MessageBox.Show("Error: could not program train detector controller with the new name (it is on-line?)", "Timeout error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        [DispatchTarget]
        private string? ValidateControlModuleLabel([DispatchFilter("ModuleType", "TrainDetectorController")] ControlModule module, string label) {
            if (string.IsNullOrWhiteSpace(label))
                return "Module label cannot be empty";

            if (module.Bus.Modules.Any(otherModule => otherModule.Id != module.Id && otherModule.Label == label))
                return $"A module with the entered label ({label}), is already defined";

            return null;
        }

    }
}
