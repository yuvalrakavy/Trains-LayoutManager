using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;
using System.Threading.Tasks;
using MethodDispatcher;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Logic {
    /// <summary>
    /// Summary description for ComponentManager.
    /// </summary>
    [LayoutModule("Component Manager", UserControl = false)]
    public class ComponentManager : LayoutModuleBase {
        #region Handle control-connection-point-notification

        [LayoutEvent("control-connection-point-state-changed-notification")]
        private void ControlConnectionPointStateChangedNotification(LayoutEvent e) {
            var connectionPointRef = Ensure.NotNull<ControlConnectionPointReference>(e.Sender, "connectionPointRef");
            int state = (int)(e.Info ?? 0);

            if (connectionPointRef.IsConnected) {
                var connectionPoint = connectionPointRef.ConnectionPoint;
                var component = (ModelComponent?)connectionPoint?.Component;

                if (component is LayoutTrackContactComponent) {
                    if (state == 1)
                        EventManager.Event(new LayoutEvent("track-contact-triggered-notification", component));
                }
                else if (component is LayoutProximitySensorComponent)
                    EventManager.Event(new LayoutEvent("proximity-sensor-state-changed-notification", component, state != 0));
                else if (component is LayoutBlockDefinitionComponent)
                    EventManager.Event(new LayoutEvent("train-detection-state-changed-notification", component, state != 0));
                else if (component is IModelComponentHasSwitchingState)
                    EventManager.Event(new LayoutEvent("track-component-state-changed-notification", connectionPoint, state));
                else if (component is LayoutSignalComponent)
                    EventManager.Event(new LayoutEvent("signal-state-changed-notification", component, state == 0 ? LayoutSignalState.Red : LayoutSignalState.Green));
            }
        }

        [LayoutEvent("control-connection-point-state-unstable-notification")]
        private void ControlConnectionPointStateUnstable(LayoutEvent e) {
            var connectionPointRef = Ensure.NotNull<ControlConnectionPointReference>(e.Sender, "connectionPointRef");

            Message(connectionPointRef.ConnectionPoint?.Component, "Intermittent (very short) state change - you should check your hardware");
        }

        #endregion

        #region Multipath Component (e.g. turnout, double-slip)

        [LayoutAsyncEvent("set-track-components-state")]
        private Task SetTrackComponentsState(LayoutEvent e) {
            var switchingCommands = Ensure.NotNull<List<SwitchingCommand>>(e.Info, "switchingCommands");
            var tasks = new List<Task>();

            if (switchingCommands.Count > 0) {
                Dictionary<IModelComponentIsBusProvider, List<SwitchingCommand>> commandStationIdToSwitchingCommands = new();
                Guid defaultCommandStationId = switchingCommands[0].CommandStationId;
                List<SwitchingCommand> movedSwitchingCommands = new();

                var busProvider = switchingCommands[0].BusProvider;
                if (busProvider != null)
                    commandStationIdToSwitchingCommands.Add(busProvider, switchingCommands);

                // Split the batch of switching commands to groups of switching commands based on the command station that handles them.
                // Optimize for the (very) common case that all of the switching commands are handled by the same command station.
                foreach (SwitchingCommand switchingCommand in switchingCommands) {
                    if (switchingCommand.CommandStationId != defaultCommandStationId) {
                        if (switchingCommand.BusProvider != null) {
                            if (!commandStationIdToSwitchingCommands.TryGetValue(switchingCommand.BusProvider, out List<SwitchingCommand>? commandStationSwitchingCommands)) {
                                commandStationSwitchingCommands = new List<SwitchingCommand>();
                                commandStationIdToSwitchingCommands.Add(switchingCommand.BusProvider, commandStationSwitchingCommands);
                            }

                            commandStationSwitchingCommands.Add(switchingCommand);
                            movedSwitchingCommands.Add(switchingCommand);
                        }
                    }
                }

                foreach (SwitchingCommand movedSwitchingCommand in movedSwitchingCommands)
                    switchingCommands.Remove(movedSwitchingCommand);

                foreach (KeyValuePair<IModelComponentIsBusProvider, List<SwitchingCommand>> commandStationAndSwitchCommands in commandStationIdToSwitchingCommands) {
                    if (commandStationAndSwitchCommands.Key.BatchMultipathSwitchingSupported)
                        tasks.Add(EventManager.AsyncEvent(new LayoutEvent("change-batch-of-track-component-state-command", this, commandStationAndSwitchCommands.Value).SetOption("CommandStationID", commandStationAndSwitchCommands.Key.Id)));
                    else {
                        foreach (SwitchingCommand switchingCommand in commandStationAndSwitchCommands.Value)
                            tasks.Add(EventManager.AsyncEvent(new LayoutEventInfoValueType<ControlConnectionPointReference, int>("change-track-component-state-command", switchingCommand.ControlPointReference, switchingCommand.SwitchState).SetCommandStation(commandStationAndSwitchCommands.Key)));
                    }
                }
            }

            return Task.WhenAll(tasks);
        }

        [LayoutEvent("track-component-state-changed-notification")]
        private void TrackComponentStateChanged(LayoutEvent e) {
            var connectionPoint = Ensure.NotNull<ControlConnectionPoint>(e.Sender, "connectionPoint");

            if (connectionPoint.Component is IModelComponentHasSwitchingState componentWithSwitchingState) {
                int state = (int)(e.Info ?? 0);
                bool reverseLogic = false;

                if (connectionPoint.Component is IModelComponentHasReverseLogic component)
                    reverseLogic = component.ReverseLogic;

                componentWithSwitchingState.SetSwitchState(connectionPoint, reverseLogic ? 1 - state : state, connectionPoint.Name);
            }
        }

        #endregion

        private int _proximitySensorSensitivityDelay = -1;

        private int ProximitySensorSensitivityDelay {
            get {
                if (_proximitySensorSensitivityDelay < 0)
                    _proximitySensorSensitivityDelay = LayoutModel.StateManager.TrainTrackingOptions.ProximitySensorSensitivityDelay;
                return _proximitySensorSensitivityDelay;
            }
        }

        [LayoutEvent("proximity-sensitivity-delay-changed")]
        private void ProximitySensorSensitivityDelayChanged(LayoutEvent e) => _proximitySensorSensitivityDelay = -1;

        private readonly Dictionary<Guid, LayoutDelayedEvent> _changingProximitySensors = new();

        [LayoutEvent("proximity-sensor-state-changed-notification", SenderType = typeof(LayoutProximitySensorComponent))]
        private void ProximitySensorComponentStateChanged(LayoutEvent e) {
            var component = Ensure.NotNull<LayoutProximitySensorComponent>(e.Sender, "component");
            bool state = Ensure.ValueNotNull<bool>(e.Info, "state");

            if (_changingProximitySensors.TryGetValue(component.Id, out var delayedEvent)) {
                delayedEvent.Cancel();
                _changingProximitySensors.Remove(component.Id);
            }

            _changingProximitySensors.Add(component.Id,
                    EventManager.DelayedEvent(ProximitySensorSensitivityDelay, new LayoutEvent("proximity-sensor-change-stable", component, state)));
        }

        [LayoutEvent("proximity-sensor-change-stable")]
        private void ProximitySensorChangeStable(LayoutEvent e) {
            var component = Ensure.NotNull<LayoutProximitySensorComponent>(e.Sender, "component");
            bool state = Ensure.ValueNotNull<bool>(e.Info, "state");

            _changingProximitySensors.Remove(component.Id);
            component.IsTriggered = state;
        }

        #region Signal Component

        [LayoutEvent("signal-state-changed-notification", SenderType = typeof(LayoutSignalComponent))]
        private void SignalComponentTurnoutStateChanged(LayoutEvent e) {
            var signal = Ensure.NotNull<LayoutSignalComponent>(e.Sender, "signal");
            var state = Ensure.ValueNotNull<LayoutSignalState>(e.Info, "state");

            if (signal.Info.ReverseLogic)
                state = (state == LayoutSignalState.Red) ? LayoutSignalState.Green : LayoutSignalState.Red;

            signal.SetSignalState(state);
        }

        #endregion

        #region Train Detection Module

        [LayoutEvent("train-detection-state-changed-notification", SenderType = typeof(LayoutBlockDefinitionComponent))]
        private void TrainDetectionStateChanged(LayoutEvent e) {
            var blockDefinition = Ensure.NotNull<LayoutBlockDefinitionComponent>(e.Sender, "blockDefinition");
            bool detected = Ensure.ValueNotNull<bool>(e.Info, "detected");

            Trace.WriteLineIf(LocomotiveTracking.traceLocomotiveTracking.TraceInfo, "=-=-= Train " + (detected ? "detected" : "not detected") + " in block " + blockDefinition.FullDescription);
            blockDefinition.Info.TrainDetected = detected;
        }

        #endregion

        #region Linked signals

        [LayoutEvent("logical-signal-state-changed")]
        private void SignalStateChanged(LayoutEvent e) {
            if (LayoutController.IsOperationMode) {
                var blockEdge = Ensure.NotNull<LayoutBlockEdgeBase>(e.Sender, "blockEdge");
                var signalState = Ensure.ValueNotNull<LayoutSignalState>(e.Info, "signalState");

                foreach (LinkedSignalInfo linkedSignal in blockEdge.LinkedSignals) {
                    var signalComponent = LayoutModel.Component<LayoutSignalComponent>(linkedSignal.SignalId, LayoutModel.ActivePhases);

                    if (signalComponent != null)
                        signalComponent.SignalState = signalState;
                }
            }
        }

        [DispatchTarget(Order =2000)]
        private void EnterOperationMode(OperationModeParameters settings) {         
            foreach (LayoutBlockEdgeBase blockEdge in LayoutModel.Components<LayoutBlockEdgeBase>(LayoutModel.ActivePhases)) {
                LayoutSignalState signalState = blockEdge.SignalState;

                foreach (LinkedSignalInfo linkedSignal in blockEdge.LinkedSignals) {
                    var signalComponent = LayoutModel.Component<LayoutSignalComponent>(linkedSignal.SignalId, LayoutModel.ActivePhases);

                    if (signalComponent != null)
                        signalComponent.SignalState = signalState;
                }
            }
        }

        #region Linked signal hash table, given a signal component it will return whether it is linked or not

        private Dictionary<Guid, LayoutBlockEdgeBase>? linkedSignalMap = null;

        protected Dictionary<Guid, LayoutBlockEdgeBase> LinkedSignalMap {
            get {
                if (linkedSignalMap == null) {
                    linkedSignalMap = new Dictionary<Guid, LayoutBlockEdgeBase>();

                    foreach (LayoutBlockEdgeBase blockEdgeBase in LayoutModel.Components<LayoutBlockEdgeBase>(LayoutModel.ActivePhases)) {
                        foreach (LinkedSignalInfo linkedSignalInfo in blockEdgeBase.LinkedSignals)
                            linkedSignalMap.Add(linkedSignalInfo.SignalId, blockEdgeBase);
                    }
                }

                return linkedSignalMap;
            }
        }

        [LayoutEvent("get-linked-signal-map")]
        private void GetLinkedSignalMap(LayoutEvent e) {
            e.Info = LinkedSignalMap;
        }

        [LayoutEvent("signal-component-linked")]
        private void SignalComponentLinked(LayoutEvent e) {
            var blockEdge = Ensure.NotNull<LayoutBlockEdgeBase>(e.Sender, "blockEdge");
            var signalComponent = Ensure.NotNull<LayoutSignalComponent>(e.Info, "signalComponent");
            Dictionary<Guid, LayoutBlockEdgeBase> map = LinkedSignalMap;

            if (!map.ContainsKey(signalComponent.Id)) {
                signalComponent.EraseImage();
                map.Add(signalComponent.Id, blockEdge);
                signalComponent.Redraw();
            }
            else
                Debug.Fail($"Signal component {signalComponent} not found not found in linked signal map");
        }

        [LayoutEvent("signal-component-unlinked")]
        private void SignalComponentUnlinked(LayoutEvent e) {
            var signalComponent = Ensure.NotNull<LayoutSignalComponent>(e.Info, "signalComponent");
            Dictionary<Guid, LayoutBlockEdgeBase> map = LinkedSignalMap;

            if (map.ContainsKey(signalComponent.Id)) {
                signalComponent.EraseImage();
                map.Remove(signalComponent.Id);
                signalComponent.Redraw();
            }
            else
                Debug.Fail($"Signal component {signalComponent} not found not found in linked signal map");
        }

        [LayoutEvent("removed-from-model", SenderType = typeof(LayoutBlockEdgeBase))]
        private void RemovingBlockEdgeComponent(LayoutEvent e) {
            var removedBlockEdge = Ensure.NotNull<LayoutBlockEdgeBase>(e.Sender, "removedBlockEdge");

            if (removedBlockEdge.LinkedSignalsElement != null && removedBlockEdge.LinkedSignals.Count > 0) {
                Dictionary<Guid, LayoutBlockEdgeBase> map = LinkedSignalMap;
                List<Guid> removeList = new();

                foreach (KeyValuePair<Guid, LayoutBlockEdgeBase> signalIdBlockEdgePair in map)
                    if (signalIdBlockEdgePair.Value == removedBlockEdge)
                        removeList.Add(signalIdBlockEdgePair.Key);

                removeList.ForEach((Guid signalId) => {
                    var signal = LayoutModel.Component<LayoutSignalComponent>(signalId, LayoutPhase.All);

                    if (signal != null)
                        EventManager.Event(new LayoutEvent("signal-component-unlinked", removedBlockEdge, signal));
                });
            }
        }

        [LayoutEvent("component-configuration-changed", SenderType = typeof(LayoutBlockEdgeBase))]
        private void BlockEdgeModified(LayoutEvent e) {
            var modifiedBlockEdge = Ensure.NotNull<LayoutBlockEdgeBase>(e.Sender, "modifiedBlockEdge");
            Dictionary<Guid, LayoutBlockEdgeBase> map = LinkedSignalMap;
            List<Guid> previousLinkedSignals = new();

            foreach (KeyValuePair<Guid, LayoutBlockEdgeBase> signalIdBlockEdgePair in map)
                if (signalIdBlockEdgePair.Value == modifiedBlockEdge)
                    previousLinkedSignals.Add(signalIdBlockEdgePair.Key);

            foreach (LinkedSignalInfo linkedSignalInfo in modifiedBlockEdge.LinkedSignals) {
                if (previousLinkedSignals.Contains(linkedSignalInfo.SignalId))
                    previousLinkedSignals.Remove(linkedSignalInfo.SignalId);
                else
                    EventManager.Event(new LayoutEvent("signal-component-linked", modifiedBlockEdge, LayoutModel.Component<LayoutSignalComponent>(linkedSignalInfo.SignalId, LayoutPhase.All)));
            }

            // The signals that are left, are not linked now, so remove them from the map
            foreach (Guid signalId in previousLinkedSignals)
                EventManager.Event(new LayoutEvent("signal-component-unlinked", modifiedBlockEdge, LayoutModel.Component<LayoutSignalComponent>(signalId, LayoutPhase.All)));
        }

        #endregion

        #endregion
    }
}
