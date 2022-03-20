using LayoutManager;
using LayoutManager.Components;
using LayoutManager.ControlComponents;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Xml;

#nullable enable
namespace TrainDetector {
    public class TrainDetectorsComponent : LayoutBusProviderSupport, IModelComponentIsBusProvider {
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static LayoutTraceSwitch TraceTrainDetector = new LayoutTraceSwitch("TrainDetectors", "VillaRakavy TrainDetectors");
#pragma warning restore CA2211 // Non-constant fields should not be visible

        private ControlBus? _trainDetectorsBus = null;
        public NetworkHandler? OptionalNetworkHandler { get; set; }
        public NetworkHandler NetworkHandler => Ensure.NotNull<NetworkHandler>(OptionalNetworkHandler);
        readonly Dictionary<IPEndPoint, ControllerState> ipToStateMap = new Dictionary<IPEndPoint, ControllerState>();

        public ControlBus TrainDetectorsBus => _trainDetectorsBus ??= Ensure.NotNull<ControlBus>(LayoutModel.ControlManager.Buses.GetBus(this, "TrainDetectorsBus"), "TrainDetectorBus");

        public TrainDetectorsComponent() {
            this.XmlDocument.LoadXml("<TrainDetectors />");
        }

        #region Model Component overrides

        public override void OnAddedToModel() {
            base.OnAddedToModel();

            LayoutModel.ControlManager.Buses.AddBusProvider(this);
        }

        public override void OnRemovingFromModel() {
            base.OnRemovingFromModel();

            LayoutModel.ControlManager.Buses.RemoveBusProvider(this);
        }

        public override ModelComponentKind Kind => ModelComponentKind.ControlComponent;

        public override bool DrawOutOfGrid => NameProvider.OptionalElement != null && NameProvider.Visible;

        #endregion

        public TrainDetectorsInfo Info => new TrainDetectorsInfo(this, Element);

        #region IModelComponentIsBusProvider Members
        public override IList<string> BusTypeNames => Array.AsReadOnly<string>(new string[] { "TrainDetectorsBus" });

        #endregion

        #region Event Handlers

        [DispatchTarget]
        private bool BeginDesignTimeLayoutActivation() => true;

        [DispatchTarget]
        private bool EndDesignTimeLayoutActivation() => true;

        [DispatchTarget]
        private async Task EnterOperationModeAsync() {
            Debug.Assert(OptionalNetworkHandler == null);
            OptionalNetworkHandler = new NetworkHandler(rawPacket => rawPacket.GetPacketHeader());

            NetworkHandler.Start();
            NetworkHandler.OnReceivingPacket += (sender, rawPacket) => {
                switch (rawPacket.GetPacket()) {
                    case StateChangedNotificationPacket stateChangedPacket:
                        if (ipToStateMap.TryGetValue(rawPacket.RemoteEndPoint, out var controllerState))
                            controllerState.UpdateState(stateChangedPacket.Version, stateChangedPacket.SensorNumber, stateChangedPacket.IsCovered, stateChangedPacket.States);
                        else
                            Trace.WriteLine($"TrainDetector: State changed packet from unexpected source {rawPacket.RemoteEndPoint}");
                        NetworkHandler.SendPacketAsync(new StateChangedAcknowledgePacket((UInt16)stateChangedPacket.RequestNumber), rawPacket.RemoteEndPoint);
                        break;

                    default:
                        Trace.WriteLine($"TrainDetector: Unexpected packet received from {rawPacket.RemoteEndPoint}");
                        break;
                }
            };

            await VerifyControllers();
        }

        [DispatchTarget]
        private async Task ExitOperationModeAsync_TrainDetector() {
            var requests = new List<Task<UdpReceiveResult>>(capacity: ipToStateMap.Count);

            // Unsubscribe from all the controllers
            foreach (var ipEndPoint in ipToStateMap.Keys) {
                requests.Add(NetworkHandler.Request(requestNumber => NetworkHandler.SendPacketAsync(new UnsubscribeRequestPacket((UInt16)requestNumber), ipEndPoint)));
            }
            await Task.WhenAll(requests);

            ipToStateMap.Clear();
            OptionalNetworkHandler?.Dispose();
            OptionalNetworkHandler = null;
        }
        #endregion


        private async Task VerifyControllers() {
            ipToStateMap.Clear();

            foreach (var module in TrainDetectorsBus.Modules) {
                var trainConrollerModule = new TrainDetectorControllerModule(module);
                var ipEndPoint = trainConrollerModule.ControllerIpAddress;

                if (trainConrollerModule.Phase == LayoutPhase.Operational && ipEndPoint == null)
                    throw new LayoutControlException(trainConrollerModule, "This module is not assigned IP address");
                else if (ipEndPoint != null) {
                    try {
                        var rawPacket = await NetworkHandler.Request((requestNumber) => NetworkHandler.SendPacketAsync(new GetStateRequestPacket((UInt16)requestNumber), ipEndPoint));

                        if (rawPacket.GetPacket() is GetStateReplyPacket getStateReply) {
                            if (getStateReply.States.Count != trainConrollerModule.NumberOfConnectionPoints)
                                throw new LayoutControlException(trainConrollerModule, $"Mismatch number of sensors, module is defined with {trainConrollerModule.NumberOfConnectionPoints} where as hardware report {getStateReply.States.Count} sensors");
                            else {
                                ipToStateMap.Add(rawPacket.RemoteEndPoint, new ControllerState(this, trainConrollerModule, getStateReply.Version, getStateReply.States));
                                await NetworkHandler.Request(requestNumber => NetworkHandler.SendPacketAsync(new SubscribeRequestPacket(requestNumber, SubscribeTo.StateChanges), ipEndPoint));
                            }
                        }
                        else
                            throw new LayoutControlException(trainConrollerModule, $"Received unexpected reply type");

                    }
                    catch (TimeoutException) {
                        throw new LayoutControlException(trainConrollerModule, $"No reply from train controller module ({ipEndPoint})");
                    }
                }

            }
        }
    }

    public class TrainDetectorsInfo : LayoutInfo {
        const string A_AutoDetect = "AutoDetect";

        public TrainDetectorsComponent TrainDetectorsComponent { get; private set; }

        public TrainDetectorsInfo(TrainDetectorsComponent component, XmlElement element) : base(element) {
            this.TrainDetectorsComponent = component;

        }
        public bool AutoDetect {
            get => (bool?)AttributeValue(A_AutoDetect) ?? false;
            set => SetAttributeValue(A_AutoDetect, value, removeIf: false);
        }
    }

    internal class ControllerState {
        public TrainDetectorsComponent TrainDetectorsComponent { get; private set; }
        public TrainDetectorControllerModule Module { get; private set; }
        public UInt32 StateVersion { get; private set; }
        public List<bool> States { get; private set; }

        public ControllerState(TrainDetectorsComponent trainDetectorsComponent, TrainDetectorControllerModule module, UInt32 initialVersion, List<bool> initialStates) {
            this.TrainDetectorsComponent = trainDetectorsComponent;
            this.Module = module;
            this.StateVersion = initialVersion;
            this.States = new List<bool>(initialStates);

            int sensorIndex = 0;


            foreach (var cp in module.ConnectionPoints) {
                if (States[sensorIndex]) {
                    if (cp.IsConnected)
                        trainDetectorsComponent.InterThreadEventInvoker.Queue(() => Dispatch.Notification.OnControlConnectionPointStateChanged(new ControlConnectionPointReference(cp), States[sensorIndex] ? 1 : 0));
                }
                sensorIndex++;
            }
        }

        public void UpdateState(UInt32 newVersion, int sensorIndex, bool isCovered, List<bool> allStates) {
            // The simple case
            if (newVersion == StateVersion+1) {
                if (Module.ConnectionPoints[sensorIndex].IsConnected)
                    TrainDetectorsComponent.InterThreadEventInvoker.Queue(() => Dispatch.Notification.OnControlConnectionPointStateChanged(new ControlConnectionPointReference(Module.ConnectionPoints[sensorIndex]), isCovered ? 1 : 0));
            }
            else {
                Trace.WriteLine($"TrainDetectors: Unexpected version {newVersion} (expected {StateVersion + 1} for module {Module.Label}@{Module.ControllerIpAddress?.ToString() ?? "No address"}");

                sensorIndex = 0;
                foreach (var cp in Module.ConnectionPoints) {
                    if (cp.IsConnected) {
                        TrainDetectorsComponent.InterThreadEventInvoker.Queue(() => Dispatch.Notification.OnControlConnectionPointStateChanged(new ControlConnectionPointReference(cp), allStates[sensorIndex] ? 1 : 0));
                    }

                    States[sensorIndex] = allStates[sensorIndex];
                    sensorIndex++;
                }
            }

            StateVersion = newVersion;
        }
    }
}
