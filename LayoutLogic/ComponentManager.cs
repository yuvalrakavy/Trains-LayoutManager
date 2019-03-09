using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;
using System.Threading.Tasks;
using LayoutManager.Model;
using LayoutManager.Components;

#nullable enable
#pragma warning disable IDE0051, IDE0060
namespace LayoutManager.Logic {
    /// <summary>
    /// Summary description for ComponentManager.
    /// </summary>
    [LayoutModule("Component Manager", UserControl = false)]
    public class ComponentManager : LayoutModuleBase {

        #region Handle control-connection-point-notification

        [LayoutEvent("control-connection-point-state-changed-notification")]
        private void controlConnectionPointStateChangedNotification(LayoutEvent e) {
            var connectionPointRef = Ensure.NotNull<ControlConnectionPointReference>(e.Sender, "connectionPointRef");
            int state = (int)e.Info;

            if (connectionPointRef.IsConnected) {
                ControlConnectionPoint connectionPoint = connectionPointRef.ConnectionPoint;
                ModelComponent component = (ModelComponent)connectionPoint.Component;

                if (component is LayoutTrackContactComponent) {
                    if (state == 1)
                        EventManager.Event(new LayoutEvent("track-contact-triggered-notification", component));
                }
                else if (component is LayoutBlockDefinitionComponent)
                    EventManager.Event(new LayoutEvent("train-detection-state-changed-notification", component, state != 0 ? true : false, null));
                else if (component is IModelComponentHasSwitchingState)
                    EventManager.Event(new LayoutEvent("track-component-state-changed-notification", connectionPoint, state, null));
                else if (component is LayoutSignalComponent)
                    EventManager.Event(new LayoutEvent("signal-state-changed-notification", component, state == 0 ? LayoutSignalState.Red : LayoutSignalState.Green, null));
            }
        }

        [LayoutEvent("control-connection-point-state-unstable-notification")]
        private void controlConnectionPointStateUnstable(LayoutEvent e) {
            var connectionPointRef = Ensure.NotNull<ControlConnectionPointReference>(e.Sender, "connectionPointRef");

            Message(connectionPointRef.ConnectionPoint.Component, "Intermittent (very short) state change - you should check your hardware");
        }

        #endregion

        #region Multipath Component (e.g. turnout, double-slip)

        [LayoutAsyncEvent("set-track-components-state")]
        private Task setTrackComponentsState(LayoutEvent e) {
            var switchingCommands = Ensure.NotNull<List<SwitchingCommand>>(e.Info, "switchingCommands");
            var tasks = new List<Task>();

            if (switchingCommands.Count > 0) {
                Dictionary<IModelComponentIsBusProvider, List<SwitchingCommand>> commandStationIdToSwitchingCommands = new Dictionary<IModelComponentIsBusProvider, List<SwitchingCommand>>();
                Guid defaultCommandStationId = switchingCommands[0].CommandStationId;
                List<SwitchingCommand> movedSwitchingCommands = new List<SwitchingCommand>();

                commandStationIdToSwitchingCommands.Add(switchingCommands[0].BusProvider, switchingCommands);

                // Split the batch of switching commands to groups of switching commands based on the command station that handles them.
                // Optimize for the (very) common case that all of the switching commands are handled by the same command station.
                foreach (SwitchingCommand switchingCommand in switchingCommands) {
                    if (switchingCommand.CommandStationId != defaultCommandStationId) {

                        if (!commandStationIdToSwitchingCommands.TryGetValue(switchingCommand.BusProvider, out List<SwitchingCommand> commandStationSwitchingCommands)) {
                            commandStationSwitchingCommands = new List<SwitchingCommand>();
                            commandStationIdToSwitchingCommands.Add(switchingCommand.BusProvider, commandStationSwitchingCommands);
                        }

                        commandStationSwitchingCommands.Add(switchingCommand);
                        movedSwitchingCommands.Add(switchingCommand);
                    }
                }

                foreach (SwitchingCommand movedSwitchingCommand in movedSwitchingCommands)
                    switchingCommands.Remove(movedSwitchingCommand);

                foreach (KeyValuePair<IModelComponentIsBusProvider, List<SwitchingCommand>> commandStationAndSwitchCommands in commandStationIdToSwitchingCommands) {
                    if (commandStationAndSwitchCommands.Key.BatchMultipathSwitchingSupported) {
                        tasks.Add(EventManager.AsyncEvent(new LayoutEvent("change-batch-of-track-component-state-command", this, commandStationAndSwitchCommands.Value, null).SetOption("CommandStationID", XmlConvert.ToString(commandStationAndSwitchCommands.Key.Id))));
                    }
                    else {
                        foreach (SwitchingCommand switchingCommand in commandStationAndSwitchCommands.Value)
                            tasks.Add(EventManager.AsyncEvent(new LayoutEventInfoValueType<ControlConnectionPointReference, int>("change-track-component-state-command", switchingCommand.ControlPointReference, switchingCommand.SwitchState).SetCommandStation(commandStationAndSwitchCommands.Key)));
                    }
                }
            }

            return Task.WhenAll(tasks);
        }

        [LayoutEvent("track-component-state-changed-notification")]
        private void trackComponentStateChanged(LayoutEvent e) {
            var connectionPoint = Ensure.NotNull<ControlConnectionPoint>(e.Sender, "connectionPoint");

            if (connectionPoint.Component is IModelComponentHasSwitchingState componentWithSwitchingState) {
                int state = (int)e.Info;
                bool reverseLogic = false;

                if (connectionPoint.Component is IModelComponentHasReverseLogic)
                    reverseLogic = ((IModelComponentHasReverseLogic)connectionPoint.Component).ReverseLogic;

                componentWithSwitchingState.SetSwitchState(connectionPoint, reverseLogic ? 1 - state : state, connectionPoint.Name);
            }
        }

        #endregion

        #region Track Contact Component

        [LayoutEvent("track-contact-triggered-notification", SenderType = typeof(LayoutTrackContactComponent))]
        private void trackContactComponentTurnoutStateChanged(LayoutEvent e) {
            var trackContact = Ensure.NotNull<LayoutTrackContactComponent>(e.Sender, "connectionPoint");

            if (!trackContact.IsTriggered) {
                trackContact.IsTriggered = true;
                EventManager.DelayedEvent(200, new LayoutEvent("exit-track-contact-triggered-state", trackContact));
                EventManager.Event(new LayoutEvent("anonymous-track-contact-triggerd", trackContact));
            }
            else
                Trace.WriteLine("Track contact " + trackContact.FullDescription + " triggered twice in 200 milli");

        }

        [LayoutEvent("exit-track-contact-triggered-state")]
        private void exitTrackContactTriggeredState(LayoutEvent e) {
            var trackContact = Ensure.NotNull<LayoutTrackContactComponent>(e.Sender, "connectionPoint");

            trackContact.IsTriggered = false;
        }

        #endregion

        #region Signal Component

        [LayoutEvent("signal-state-changed-notification", SenderType = typeof(LayoutSignalComponent))]
        private void signalComponentTurnoutStateChanged(LayoutEvent e) {
            var signal = Ensure.NotNull<LayoutSignalComponent>(e.Sender, "signal");
            LayoutSignalState state = (LayoutSignalState)e.Info;

            if (signal.Info.ReverseLogic)
                state = (state == LayoutSignalState.Red) ? LayoutSignalState.Green : LayoutSignalState.Red;

            signal.SetSignalState(state);
        }

        #endregion

        #region Train Detection Module

        [LayoutEvent("train-detection-state-changed-notification", SenderType = typeof(LayoutBlockDefinitionComponent))]
        private void trainDetectionStateChanged(LayoutEvent e) {
            var blockDefinition = Ensure.NotNull<LayoutBlockDefinitionComponent>(e.Sender, "blockDefinition");
            bool detected = (bool)e.Info;

            Trace.WriteLineIf(LocomotiveTracking.traceLocomotiveTracking.TraceInfo, "=-=-= Train " + (detected ? "detected" : "not detected") + " in block " + blockDefinition.FullDescription);
            blockDefinition.Info.TrainDetected = detected;
        }

        #endregion

        #region Linked signals

        [LayoutEvent("logical-signal-state-changed")]
        private void signalStateChanged(LayoutEvent e) {
            if (LayoutController.IsOperationMode) {
                var blockEdge = Ensure.NotNull<LayoutBlockEdgeBase>(e.Sender, "blockEdge");
                LayoutSignalState signalState = (LayoutSignalState)e.Info;

                foreach (LinkedSignalInfo linkedSignal in blockEdge.LinkedSignals) {
                    LayoutSignalComponent signalComponent = LayoutModel.Component<LayoutSignalComponent>(linkedSignal.SignalId, LayoutModel.ActivePhases);

                    if (signalComponent != null)
                        signalComponent.SignalState = signalState;
                }
            }
        }

        [LayoutEvent("enter-operation-mode", Order = 2000)]
        private void setSignalsOnEnterOperationMode(LayoutEvent e) {
            foreach (LayoutBlockEdgeBase blockEdge in LayoutModel.Components<LayoutBlockEdgeBase>(LayoutModel.ActivePhases)) {
                LayoutSignalState signalState = blockEdge.SignalState;

                foreach (LinkedSignalInfo linkedSignal in blockEdge.LinkedSignals) {
                    LayoutSignalComponent signalComponent = LayoutModel.Component<LayoutSignalComponent>(linkedSignal.SignalId, LayoutModel.ActivePhases);

                    if (signalComponent != null)
                        signalComponent.SignalState = signalState;
                }
            }
        }

        #region Linked signal hash table, given a signal component it will return whether it is linked or not

        Dictionary<Guid, LayoutBlockEdgeBase>? linkedSignalMap = null;

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
        private void getLinkedSignalMap(LayoutEvent e) {
            e.Info = LinkedSignalMap;
        }

        [LayoutEvent("signal-component-linked")]
        private void signalComponentLinked(LayoutEvent e) {
            var blockEdge = Ensure.NotNull<LayoutBlockEdgeBase>(e.Sender, "blockEdge");
            var signalComponent = Ensure.NotNull<LayoutSignalComponent>(e.Info, "signalComponent");
            Dictionary <Guid, LayoutBlockEdgeBase> map = LinkedSignalMap;

            if (!map.ContainsKey(signalComponent.Id)) {
                signalComponent.EraseImage();
                map.Add(signalComponent.Id, blockEdge);
                signalComponent.Redraw();
            }
            else
                Debug.Assert(false);
        }

        [LayoutEvent("signal-component-unlinked")]
        private void signalComponentUnlinked(LayoutEvent e) {
            var signalComponent = Ensure.NotNull<LayoutSignalComponent>(e.Info, "signalComponent");
            Dictionary<Guid, LayoutBlockEdgeBase> map = LinkedSignalMap;

            if (map.ContainsKey(signalComponent.Id)) {
                signalComponent.EraseImage();
                map.Remove(signalComponent.Id);
                signalComponent.Redraw();
            }
            else
                Debug.Assert(false);
        }

        [LayoutEvent("removed-from-model", SenderType = typeof(LayoutBlockEdgeBase))]
        private void removingBlockEdgeComponent(LayoutEvent e) {
            var removedBlockEdge = Ensure.NotNull<LayoutBlockEdgeBase>(e.Sender, "removedBlockEdge");

            if (removedBlockEdge.LinkedSignalsElement != null && removedBlockEdge.LinkedSignals.Count > 0) {
                Dictionary<Guid, LayoutBlockEdgeBase> map = LinkedSignalMap;
                List<Guid> removeList = new List<Guid>();

                foreach (KeyValuePair<Guid, LayoutBlockEdgeBase> signalIdBlockEdgePair in map)
                    if (signalIdBlockEdgePair.Value == removedBlockEdge)
                        removeList.Add(signalIdBlockEdgePair.Key);

                removeList.ForEach(delegate (Guid signalId) {
                    LayoutSignalComponent signal = LayoutModel.Component<LayoutSignalComponent>(signalId, LayoutPhase.All);

                    if (signal != null)
                        EventManager.Event(new LayoutEvent("signal-component-unlinked", removedBlockEdge, signal, null));
                });
            }
        }

        [LayoutEvent("component-configuration-changed", SenderType = typeof(LayoutBlockEdgeBase))]
        private void blockEdgeModified(LayoutEvent e) {
            var modifiedBlockEdge = Ensure.NotNull<LayoutBlockEdgeBase>(e.Sender, "modifiedBlockEdge");
            Dictionary <Guid, LayoutBlockEdgeBase> map = LinkedSignalMap;
            List<Guid> previousLinkedSignals = new List<Guid>();

            foreach (KeyValuePair<Guid, LayoutBlockEdgeBase> signalIdBlockEdgePair in map)
                if (signalIdBlockEdgePair.Value == modifiedBlockEdge)
                    previousLinkedSignals.Add(signalIdBlockEdgePair.Key);

            foreach (LinkedSignalInfo linkedSignalInfo in modifiedBlockEdge.LinkedSignals) {
                if (previousLinkedSignals.Contains(linkedSignalInfo.SignalId))
                    previousLinkedSignals.Remove(linkedSignalInfo.SignalId);
                else
                    EventManager.Event(new LayoutEvent("signal-component-linked", modifiedBlockEdge, LayoutModel.Component<LayoutSignalComponent>(linkedSignalInfo.SignalId, LayoutPhase.All), null));
            }

            // The signals that are left, are not linked now, so remove them from the map
            foreach (Guid signalId in previousLinkedSignals)
                EventManager.Event(new LayoutEvent("signal-component-unlinked", modifiedBlockEdge, LayoutModel.Component<LayoutSignalComponent>(signalId, LayoutPhase.All), null));
        }

        #endregion

        #endregion
    }
}
