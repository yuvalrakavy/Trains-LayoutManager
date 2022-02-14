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

        [DispatchTarget]
        private void OnControlConnectionPointStateChanged(ControlConnectionPointReference connectionPointRef, int state) {
            if (connectionPointRef.IsConnected) {
                var connectionPoint = connectionPointRef.ConnectionPoint;

                if (connectionPoint != null) {
                    var component = (ModelComponent?)connectionPoint.Component;

                    switch (component) {
                        case LayoutTrackContactComponent trackContact:
                            if (state == 1)
                                Dispatch.Notification.OnTrackContactTriggered(trackContact);
                            break;
                        case LayoutProximitySensorComponent proximitySensor:
                            OnProximitySensorStateChanged(proximitySensor, state != 0);
                            break;
                        case LayoutBlockDefinitionComponent blockDefinition:
                            OnTrainDetectionStateChanged(blockDefinition, state != 0);
                            break;
                        case IModelComponentHasSwitchingState:
                            OnTrackComponentStateChanged(connectionPoint, state);
                            break;
                        case LayoutSignalComponent signalComponent:
                            OnSignalStateChanged(signalComponent, state == 0 ? LayoutSignalState.Red : LayoutSignalState.Green);
                            break;
                    }
                }
            }
        }

        [DispatchTarget]
        private void OnControlConnectionPointStateUnstable(ControlConnectionPointReference connectionPointRef) {
            Message(connectionPointRef.ConnectionPoint?.Component, "Intermittent (very short) state change - you should check your hardware");
        }

        #endregion

        #region Multi-path Component (e.g. turnout, double-slip)

        [DispatchTarget]
        private Task SetTrackComponentsState(List<SwitchingCommand> switchingCommands) {
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
                        tasks.Add(Dispatch.Call.ChangeBatchOfTrackComponentStateCommand(commandStationAndSwitchCommands.Key.Id, commandStationAndSwitchCommands.Value));
                    else {
                        foreach (SwitchingCommand switchingCommand in commandStationAndSwitchCommands.Value)
                            tasks.Add(Dispatch.Call.ChangeTrackComponentStateCommand(commandStationAndSwitchCommands.Key, switchingCommand.ControlPointReference, switchingCommand.SwitchState));
                    }
                }
            }

            return Task.WhenAll(tasks);
        }

        [DispatchTarget]
        private void OnTrackComponentStateChanged(ControlConnectionPoint connectionPoint, int state) {
            if (connectionPoint.Component is IModelComponentHasSwitchingState componentWithSwitchingState) {
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

        [DispatchTarget]
        private void OnProximitySensitivityDelayChanged() => _proximitySensorSensitivityDelay = -1;

        private readonly Dictionary<Guid, LayoutDelayedEvent> _changingProximitySensors = new();

        [DispatchTarget]
        private void OnProximitySensorStateChanged(LayoutProximitySensorComponent component, bool state) {
            if (_changingProximitySensors.TryGetValue(component.Id, out var delayedEvent)) {
                delayedEvent.Cancel();
                _changingProximitySensors.Remove(component.Id);
            }

            _changingProximitySensors.Add(component.Id,
                    EventManager.DelayedEvent(ProximitySensorSensitivityDelay, () => ProximitySensorChangeStable(component, state)));
        }

        private void ProximitySensorChangeStable(LayoutProximitySensorComponent component, bool state) {
            _changingProximitySensors.Remove(component.Id);
            component.IsTriggered = state;
        }

        #region Signal Component

        [DispatchTarget]
        private void OnSignalStateChanged(LayoutSignalComponent signal, LayoutSignalState state) {
            if (signal.Info.ReverseLogic)
                state = (state == LayoutSignalState.Red) ? LayoutSignalState.Green : LayoutSignalState.Red;

            signal.SetSignalState(state);
        }

        #endregion

        #region Train Detection Module

        [DispatchTarget]
        private void OnTrainDetectionStateChanged(LayoutBlockDefinitionComponent blockDefinition, bool detected) {
            Trace.WriteLineIf(LocomotiveTracking.traceLocomotiveTracking.TraceInfo, "=-=-= Train " + (detected ? "detected" : "not detected") + " in block " + blockDefinition.FullDescription);
            blockDefinition.Info.TrainDetected = detected;
        }

        #endregion

        #region Linked signals

        [DispatchTarget]
        private void OnLogicalSignalStateChanged(LayoutBlockEdgeBase blockEdge, LayoutSignalState signalState) {
            if (LayoutController.IsOperationMode) {
                foreach (LinkedSignalInfo linkedSignal in blockEdge.LinkedSignals) {
                    var signalComponent = LayoutModel.Component<LayoutSignalComponent>(linkedSignal.SignalId, LayoutModel.ActivePhases);

                    if (signalComponent != null)
                        signalComponent.SignalState = signalState;
                }
            }
        }

        [DispatchTarget(Order =2000)]
        private void OnEnteredOperationMode(OperationModeParameters settings) {         
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

        [DispatchTarget]
        private Dictionary<Guid, LayoutBlockEdgeBase> GetLinkedSignalMap() => LinkedSignalMap;

        [DispatchTarget]
        private void OnSignalComponentLinked(LayoutBlockEdgeBase blockEdge, LayoutSignalComponent signalComponent) {
            Dictionary<Guid, LayoutBlockEdgeBase> map = LinkedSignalMap;

            if (!map.ContainsKey(signalComponent.Id)) {
                signalComponent.EraseImage();
                map.Add(signalComponent.Id, blockEdge);
                signalComponent.Redraw();
            }
            else
                Debug.Fail($"Signal component {signalComponent} not found not found in linked signal map");
        }

        [DispatchTarget]
        private void OnSignalComponentUnlinked(LayoutSignalComponent signalComponent) {
            Dictionary<Guid, LayoutBlockEdgeBase> map = LinkedSignalMap;

            if (map.ContainsKey(signalComponent.Id)) {
                signalComponent.EraseImage();
                map.Remove(signalComponent.Id);
                signalComponent.Redraw();
            }
            else
                Debug.Fail($"Signal component {signalComponent} not found not found in linked signal map");
        }

        [DispatchTarget]
        private void OnRemovedFromModel_BlockEdge([DispatchFilter] LayoutBlockEdgeBase removedBlockEdge) {
            if (removedBlockEdge.LinkedSignalsElement != null && removedBlockEdge.LinkedSignals.Count > 0) {
                Dictionary<Guid, LayoutBlockEdgeBase> map = LinkedSignalMap;
                List<Guid> removeList = new();

                foreach (KeyValuePair<Guid, LayoutBlockEdgeBase> signalIdBlockEdgePair in map)
                    if (signalIdBlockEdgePair.Value == removedBlockEdge)
                        removeList.Add(signalIdBlockEdgePair.Key);

                removeList.ForEach((Guid signalId) => {
                    var signal = LayoutModel.Component<LayoutSignalComponent>(signalId, LayoutPhase.All);

                    if (signal != null)
                        Dispatch.Notification.OnSignalComponentUnlinked(signal);
                });
            }
        }

        [DispatchTarget]
        private void OnComponentConfigurationChanged_BlockEdge([DispatchFilter] LayoutBlockEdgeBase modifiedBlockEdge) {
            Dictionary<Guid, LayoutBlockEdgeBase> map = LinkedSignalMap;
            List<Guid> previousLinkedSignals = new();

            foreach (KeyValuePair<Guid, LayoutBlockEdgeBase> signalIdBlockEdgePair in map)
                if (signalIdBlockEdgePair.Value == modifiedBlockEdge)
                    previousLinkedSignals.Add(signalIdBlockEdgePair.Key);

            foreach (LinkedSignalInfo linkedSignalInfo in modifiedBlockEdge.LinkedSignals) {
                if (previousLinkedSignals.Contains(linkedSignalInfo.SignalId))
                    previousLinkedSignals.Remove(linkedSignalInfo.SignalId);
                else {
                    var signalComponent = LayoutModel.Component<LayoutSignalComponent>(linkedSignalInfo.SignalId, LayoutPhase.All);

                    if(signalComponent != null)
                        Dispatch.Notification.OnSignalComponentLinked(modifiedBlockEdge, signalComponent);
                }
            }

            // The signals that are left, are not linked now, so remove them from the map
            foreach (Guid signalId in previousLinkedSignals) {
                var signalComponent = LayoutModel.Component<LayoutSignalComponent>(signalId, LayoutPhase.All);

                if (signalComponent != null)
                    Dispatch.Notification.OnSignalComponentUnlinked(signalComponent);
            }
        }

        #endregion

        #endregion
    }
}
