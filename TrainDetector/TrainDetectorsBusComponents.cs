using System;
using System.Collections.Generic;
using System.Xml;
using System.Net;
using LayoutManager.Model;
using System.Diagnostics;

#pragma warning disable IDE0051
#nullable enable
namespace LayoutManager.ControlComponents {
    [LayoutModule("Train Detector Bus Components")]
    internal class TrainDetectorBusComponents : LayoutModuleBase {
        private ControlBusType? trainDetectorBus = null;

        [LayoutEvent("get-control-bus-type", IfEvent = "LayoutEvent[./Options/@BusTypeName='TrainDetectorsBus']")]
        private void getTrainDetectorsBusBusType(LayoutEvent e) {
            if (trainDetectorBus == null) {
                trainDetectorBus = new ControlBusType {
                    BusFamilyName = "TrainDetectorsBus",
                    BusTypeName = "TrainDetectorsBus",
                    Name = "TrainDetectors",
                    Topology = ControlBusTopology.RandomAddressing,
                    AddressingMethod = ControlAddressingMethod.ModuleConnectionPointAddressing,
                    FirstAddress = 0,
                    LastAddress = int.MaxValue,
                    Usage = ControlConnectionPointUsage.Input,
                    ClickToAddModuleEventName = "click-to-add-train-detector",
                    CanChangeAddress = false,
                    AllowEmptyLabel = false,
                };
            }

            e.Info = trainDetectorBus;
        }

        [LayoutEvent("recommend-control-module-types", IfEvent = "LayoutEvent[./Options/@BusFamily='TrainDetectorsBus']")]
        private void recommendTrainDetectorsBusControlModuleType(LayoutEvent e) {
            var connectionDestination = Ensure.NotNull<ControlConnectionPointDestination>(e.Sender, "connectionDestination");
            var moduleTypeNames = Ensure.NotNull<IList<string>>(e.Info, "moduleTypeNames");

            if (connectionDestination.ConnectionDescription.IsCompatibleWith("Feedback", "Level"))
                moduleTypeNames.Add("TrainDetectorController");
        }

        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='TrainDetectorController']")]
        [LayoutEvent("enum-control-module-types")]
        private void getTrainDetectorControllerModule(LayoutEvent e) {
            var parentElement = Ensure.NotNull<XmlElement>(e.Sender, "parentElement");

            var moduleType = new ControlModuleType(parentElement, "TrainDetectorController", "TrainDetector Controller") {
                DefaultControlConnectionPointType = ControlConnectionPointTypes.Level,
                ConnectionPointIndexBase = 1,
                ConnectionPointLabelFormat = ControlConnectionPointLabelFormatOptions.ConnectionPointIndex,
            };

            moduleType.BusTypeNames.Add("TrainDetectorsBus");
        }

        [LayoutEvent("get-control-module-description", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='TrainDetectorController']")]
        private void getTrainDetectorControllerDescipriotn(LayoutEvent e) {
            var module = Ensure.NotNull<ControlModule>(e.Sender);
            var trainDetectorController = new TrainDetectorControllerModule(module);
            var ipEndPoint = trainDetectorController.ControllerIpAddress;

            e.Info = ipEndPoint == null
                ? $"{trainDetectorController.Label} (unassigned)"
                : $"{trainDetectorController.Label}@{ipEndPoint.Address.ToString()}";
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

            var prevIpEndPoint = trainDetectorController.ControllerIpAddress;
            trainDetectorController.ControllerIpAddress = ipEndPoint;
            ipEndPoint = prevIpEndPoint;
            EventManager.Event(new LayoutEvent("control-module-modified", trainDetectorController).SetOption("ModuleTypeName", trainDetectorController.ModuleTypeName));
        }

        public override void Undo() {
            Do();
        }

        public override string ToString() => "Change Train Detector controller IP address";
    }
}

