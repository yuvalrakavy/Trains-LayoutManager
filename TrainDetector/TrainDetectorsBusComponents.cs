using LayoutManager.Model;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Xml;
using MethodDispatcher;

#nullable enable
namespace LayoutManager.ControlComponents {
    [LayoutModule("Train Detector Bus Components")]
    internal class TrainDetectorBusComponents : LayoutModuleBase {
        private ControlBusType? trainDetectorBus = null;

        [DispatchTarget]
        private ControlBusType GetControlBusType_TrainDetectorsBus([DispatchFilter] string busTypeName = "TrainDetectorsBus") {
            return trainDetectorBus ??= new ControlBusType {
                BusFamilyName = "TrainDetectorsBus",
                BusTypeName = "TrainDetectorsBus",
                Name = "TrainDetectors",
                Topology = ControlBusTopology.RandomAddressing,
                AddressingMethod = ControlAddressingMethod.ModuleConnectionPointAddressing,
                FirstAddress = 0,
                LastAddress = int.MaxValue,
                Usage = ControlConnectionPointUsage.Input,
                ClickToAddModuleDispatchSource = "ClickToAddTrainDetector",
                CanChangeAddress = false,
                AllowEmptyLabel = false,
            };
        }

        [DispatchTarget]
        private void RecommendControlModuleTypes_TrainDetectorsBus(ControlConnectionPointDestination connectionDestination, List<string> moduleTypeNames, string busFamilyName, [DispatchFilter] string busTypeName = "TrainDetectorsBus") {
            if (connectionDestination.ConnectionDescription.IsCompatibleWith("Feedback", "Level"))
                moduleTypeNames.Add("TrainDetectorController");
        }

        [DispatchTarget]
        private ControlModuleType GetControlModuleType([DispatchFilter("RegEx", "(TrainDetectorController|ALL)")] string moduleTypeName) {
            var moduleType = new ControlModuleType("TrainDetectorController", "TrainDetector Controller") {
                DefaultControlConnectionPointType = ControlConnectionPointTypes.Level,
                ConnectionPointIndexBase = 1,
                ConnectionPointLabelFormat = ControlConnectionPointLabelFormatOptions.ConnectionPointIndex,
            };

            moduleType.BusTypeNames.Add("TrainDetectorsBus");
            return moduleType;
        }

        [DispatchTarget]
        private string GetTrainDetectorControllerDescipriotn(ControlModule module, string addressText, [DispatchFilter] string moduleTypeName = "TrainDetectorController") {
            var trainDetectorController = new TrainDetectorControllerModule(module);
            var ipEndPoint = trainDetectorController.ControllerIpAddress;

            return ipEndPoint == null
                ? $"{trainDetectorController.Label} (unassigned)"
                : $"{trainDetectorController.Label}@{ipEndPoint.Address}";
        }
    }

    public class TrainDetectorControllerModule : ControlModule {
        const string A_SensorsCount = "SensorsCount";
        const string A_IPaddress = "IPaddress";

        public TrainDetectorControllerModule(ControlModule module) : base(module.ControlManager, module.Element) {
        }

        /// <summary>
        /// The controller IP address (null if no address is assigned to this controller)
        /// </summary>
        public IPEndPoint? ControllerIpAddress {
            get {
                var ipEndPointText = (string?)AttributeValue(A_IPaddress);

                if (ipEndPointText != null) {
                    var endPointParts = ipEndPointText.Split(':');

                    if (endPointParts.Length != 2) {
                        Debug.Assert(endPointParts.Length == 2);
                        return null;
                    }
                    else
                        return new IPEndPoint(IPAddress.Parse(endPointParts[0]), int.Parse(endPointParts[1]));
                }
                else
                    return null;
            }

            set {
                if (value != null)
                    SetAttributeValue(A_IPaddress, value.ToString());
                else
                    RemoveAttribute(A_IPaddress);
            }
        }
    }

    public class SetTrainDetectorIpEndPointCommand : LayoutCommand {
        readonly ControlModuleReference moduleRef;
        IPEndPoint? ipEndPoint;

        public SetTrainDetectorIpEndPointCommand(TrainDetectorControllerModule trainDetectorController, IPEndPoint? ipEndPoint) {
            this.moduleRef = new ControlModuleReference(trainDetectorController);
            this.ipEndPoint = ipEndPoint;
        }

        public override void Do() {
            var trainDetectorController = new TrainDetectorControllerModule(moduleRef.Module);

            (ipEndPoint, trainDetectorController.ControllerIpAddress) = (trainDetectorController.ControllerIpAddress, ipEndPoint);
            Dispatch.Notification.OnControlModuleModified(trainDetectorController);
        }

        public override void Undo() {
            Do();
        }

        public override string ToString() => "Change Train Detector controller IP address";
    }
}

