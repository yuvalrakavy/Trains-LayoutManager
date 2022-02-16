using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Net;
using LayoutManager.Model;
using System.Diagnostics;
using LayoutManager.ControlComponents;
using System.Threading.Tasks;
using LayoutManager;
using System.Windows.Forms;

#nullable enable
namespace TrainDetector {
    public class TrainDetectorControllersDetection {
        public ControlBus Bus { get; private set; }
        public Guid? ModuleLocationId { get; private set; }

        public TrainDetectorControllersDetection(ControlBus bus, Guid? moduleLocationId) {
            this.Bus = bus;
            this.ModuleLocationId = moduleLocationId;
        }

        IEnumerable<TrainDetectorControllerModule> Modules => new List<TrainDetectorControllerModule>(from module in Bus.Modules select new TrainDetectorControllerModule(module));

        IEnumerable<TrainDetectorControllerModule> UnassignedModules => from module in Modules where module.ControllerIpAddress == null select module;

        IEnumerable<TrainDetectorControllerModule> AssignedModules => from module in Modules where module.ControllerIpAddress != null select module;
           

        async Task<List<DetectedTrainDetectorController>> DetectControllers(int detectionTimeMs = 500) {
            var detectedContollers = new Dictionary<EndPoint, DetectedTrainDetectorController>();
            using var networkHandler = new NetworkHandler(rawPacket => rawPacket.GetPacketHeader());
            var broadcastAddress = new IPEndPoint(IPAddress.Broadcast, PleaseIdentifyPacket.TrainDetectorPort);

            networkHandler.OnReceivingPacket += (sender, rawPacket) => {
                var packet = rawPacket.GetPacket();

                if (packet is IdentificationInfoPacket identificationPacket) {
                    if(!detectedContollers.ContainsKey(rawPacket.RemoteEndPoint))
                        detectedContollers.Add(rawPacket.RemoteEndPoint, new DetectedTrainDetectorController(rawPacket.RemoteEndPoint, identificationPacket.SensorCount, identificationPacket.Name));
                    networkHandler.SendPacketAsync(new IdentificationAcknowledgePacket((UInt16)identificationPacket.RequestNumber), rawPacket.RemoteEndPoint);
                }
            };

            networkHandler.Start();
            await networkHandler.SendPacketAsync(new PleaseIdentifyPacket(0), broadcastAddress);
            await Task.Delay(detectionTimeMs);

            return new List<DetectedTrainDetectorController>(detectedContollers.Values);
        }
        
        public async Task<TrainDetectorControllersDetectionResult> UpdateBus() {
            var commands = new LayoutCompoundCommand("Detect controllers", true);
            var detectedControllers = await DetectControllers();
            var result = new TrainDetectorControllersDetectionResult();

            // Check if the detected controller can be assigned to a module that is not yet assigned
            // First check if there is unassigned controller with the same name and same number of sensors,
            // if such controller is found, bingo!
            //
            // Next check if there is an assigned controller(s) with the same number of sensors and is named _TrainDetector_,
            // if there is only one, use this and program the controller name to be the same as the unassigned controller name.
            //
            // In any other case, show the user the list of unassigned controllers with the same number of sensors as the detected
            // one and ask the user which one (if any) should be used.
            //
            // If the user selected none, or no unassigned controller could be found, add the detected controller to the bus
            //
            foreach (var detectedController in detectedControllers) {
                if (AssignedModules.Any(
                    module => module.ControllerIpAddress!.ToString() == detectedController.IpEndPoint.ToString() && module.NumberOfConnectionPoints == detectedController.SensorsCount && module.Label == detectedController.Name
                )) {
                    result.FoundControllers++;
                    continue;       // Found the detected controller on the bus
                }

                // See if the detected controller has the same name and address but with more sensors...
                var fixSensorCountModule = AssignedModules.FirstOrDefault(
                    module => module.ControllerIpAddress!.ToString() == detectedController.IpEndPoint.ToString() && detectedController.SensorsCount > module.NumberOfConnectionPoints && module.Label == detectedController.Name
                );

                if (fixSensorCountModule != null) {
                    commands.Add(new SetControlModuleConnectionPointsCount(fixSensorCountModule, detectedController.SensorsCount));
                    result.SensorsCountUpdated++;
                    continue;
                }

                var sameNameAndSensorCountModule = (from module in Modules where module.NumberOfConnectionPoints == detectedController.SensorsCount && module.Label == detectedController.Name select module).FirstOrDefault();

                if(sameNameAndSensorCountModule != null) {
                    // Assign the new IPaddress
                    commands.Add(new SetTrainDetectorIpEndPointCommand(sameNameAndSensorCountModule, detectedController.IpEndPoint));
                    result.IpAddressChanged++;
                    continue;
                }

                var unassignedSameSensorCountList = from module in UnassignedModules where module.NumberOfConnectionPoints == detectedController.SensorsCount select module;

                if(unassignedSameSensorCountList.Count() > 0) {
                    var unassignedModule = unassignedSameSensorCountList.Count() == 1 ? unassignedSameSensorCountList.First() : selectUnassignedModule(unassignedSameSensorCountList, detectedController);

                    if (unassignedModule != null) {
                        commands.Add(new SetTrainDetectorIpEndPointCommand(new TrainDetectorControllerModule(unassignedModule), detectedController.IpEndPoint));
                        if (unassignedModule.Label != detectedController.Name && unassignedModule.Label != null)
                            await renameController(detectedController, unassignedModule.Label);
                        result.AssignedControllers++;
                        continue;
                    }
                }

                // Add the new detected controller. First check if it has valid name (one that does not already exist).If so it will be renamed
                var name = Modules.Any(module => module.Label == detectedController.Name) ? formUniqueName(detectedController.Name) : detectedController.Name;
                var moduleType = Bus.BusType.ModuleTypes.First();
                var address = Bus.AllocateFreeAddress(moduleType, ModuleLocationId ?? Guid.Empty);
                var addCommand = new AddControlModuleCommand(Bus, "TrainDetectorController", ModuleLocationId, address);

                commands.Add(addCommand);
                commands.Add(new SetControlModuleLabelCommand(addCommand.AddedModule.Module, name));
                commands.Add(new SetTrainDetectorIpEndPointCommand(new TrainDetectorControllerModule(addCommand.AddedModule.Module), detectedController.IpEndPoint));
                commands.Add(new SetControlModuleConnectionPointsCount(addCommand.AddedModule.Module, detectedController.SensorsCount));

                // Rename the controller with the new unique name
                if(name != detectedController.Name)
                    await renameController(detectedController, name);
                result.AddedControllers++;
            }

            LayoutController.Do(commands);
            return result;
        }

        private async static Task renameController(DetectedTrainDetectorController detectedController, string name) {
            using var networkHandler = new NetworkHandler(rawPacket => rawPacket.GetPacketHeader());

            networkHandler.Start();
            await networkHandler.Request(requestNumber => networkHandler.SendPacketAsync(new ConfigSetRequestPacket(requestNumber, "Name", name), detectedController.IpEndPoint));
        }

        private TrainDetectorControllerModule? selectUnassignedModule(IEnumerable<TrainDetectorControllerModule> unassignedSameSensorCountList, DetectedTrainDetectorController detectedController) {
            using var d = new Dialogs.SelectTrainDetectorModule(detectedController, new List<TrainDetectorControllerModule>(unassignedSameSensorCountList));

            if (d.ShowDialog() == DialogResult.OK)
                return d.Module;
            return null;
        }

        private string formUniqueName(string existingName) {
            int suffix = 1;

            while(true) {
                var newName = $"{existingName}{suffix}";

                if (!Modules.Any(module => module.Label == newName))
                    return newName;
                suffix++;
            }
        }

    }

    public class DetectedTrainDetectorController {
        public IPEndPoint IpEndPoint { get; private set; }
        public int SensorsCount { get; private set; }
        public string Name { get; private set; }

        public DetectedTrainDetectorController(IPEndPoint ipEndPoint, int sensorsCount, string name) {
            this.IpEndPoint = ipEndPoint;
            this.SensorsCount = sensorsCount;
            this.Name = name;
        }
    }

    public class TrainDetectorControllersDetectionResult {
        public int FoundControllers { get; set; }

        public int IpAddressChanged { get; set; }

        public int AddedControllers { get; set; }

        public int AssignedControllers { get; set; }

        public int SensorsCountUpdated { get; set; }

        private string? value(int value, string suffix) => value == 0 ? null : $"{value} train detector {(value == 1 ? "controller" : "controllers")} {suffix}";

        public bool IsEmpty => FoundControllers == 0 && IpAddressChanged == 0 && AddedControllers == 0 && AssignedControllers == 0 && SensorsCountUpdated == 0;

        public override string ToString() {
            var parts = new string?[] {
                value(FoundControllers, "found"),
                value(IpAddressChanged, "with changed IP adress"),
                value(AddedControllers, "added"),
                value(SensorsCountUpdated, "with more sensors"),
            };

            return string.Join("\n", parts.Where(p => p != null));
        }
    };
}
