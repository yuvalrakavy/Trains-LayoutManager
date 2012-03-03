using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;
using System.Threading.Tasks;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Logic
{
	/// <summary>
	/// Summary description for ComponentManager.
	/// </summary>
	[LayoutModule("Component Manager", UserControl=false)]
	public class ComponentManager : LayoutModuleBase {

		#region Handle control-connection-point-notification

		[LayoutEvent("control-connection-point-state-changed-notification")]
		private void controlConnectionPointStateChangedNotification(LayoutEvent e) {
			ControlConnectionPointReference	connectionPointRef = (ControlConnectionPointReference)e.Sender;
			int								state = (int)e.Info;

			if(connectionPointRef.IsConnected) {
				ControlConnectionPoint	connectionPoint = connectionPointRef.ConnectionPoint;
				ModelComponent			component = (ModelComponent)connectionPoint.Component;

				if(component is LayoutTrackContactComponent) {
					if(state == 1)
						EventManager.Event(new LayoutEvent(component, "track-contact-triggered-notification"));
				}
				else if(component is LayoutBlockDefinitionComponent)
					EventManager.Event(new LayoutEvent(component, "train-detection-state-changed-notification", null, state != 0 ? true : false));
				else if(component is IModelComponentHasSwitchingState)
					EventManager.Event(new LayoutEvent(connectionPoint, "track-component-state-changed-notification", null, state));
				else if(component is LayoutSignalComponent)
					EventManager.Event(new LayoutEvent(component, "signal-state-changed-notification", null, state == 0 ? LayoutSignalState.Red : LayoutSignalState.Green));
			}
		}

		[LayoutEvent("control-connection-point-state-unstable-notification")]
		private void controlConnectionPointStateUnstable(LayoutEvent e) {
			ControlConnectionPointReference connectionPointRef = (ControlConnectionPointReference)e.Sender;

			Message(connectionPointRef.ConnectionPoint.Component, "Intermittent (very short) state change - you should check your hardware");
		}

		#endregion

		#region Multipath Component (e.g. turnout, double-slip)

		[LayoutAsyncEvent("set-track-components-state")]
		private Task setTrackComponentsState(LayoutEvent e) {
			List<SwitchingCommand> switchingCommands = (List<SwitchingCommand>)e.Info;
			var tasks = new List<Task>();

			if(switchingCommands.Count > 0) {
				Dictionary<IModelComponentIsBusProvider, List<SwitchingCommand>> commandStationIdToSwitchingCommands = new Dictionary<IModelComponentIsBusProvider, List<SwitchingCommand>>();
				Guid defaultCommandStationId = switchingCommands[0].CommandStationId;
				List<SwitchingCommand> movedSwitchingCommands = new List<SwitchingCommand>();

				commandStationIdToSwitchingCommands.Add(switchingCommands[0].BusProvider, switchingCommands);

				// Split the batch of switching commands to groups of switching commands based on the command station that handles them.
				// Optimize for the (very) common case that all of the switching commands are handled by the same command station.
				foreach(SwitchingCommand switchingCommand in switchingCommands) {
					if(switchingCommand.CommandStationId != defaultCommandStationId) {
						List<SwitchingCommand> commandStationSwitchingCommands;

						if(!commandStationIdToSwitchingCommands.TryGetValue(switchingCommand.BusProvider, out commandStationSwitchingCommands)) {
							commandStationSwitchingCommands = new List<SwitchingCommand>();
							commandStationIdToSwitchingCommands.Add(switchingCommand.BusProvider, commandStationSwitchingCommands);
						}

						commandStationSwitchingCommands.Add(switchingCommand);
						movedSwitchingCommands.Add(switchingCommand);
					}
				}

				foreach(SwitchingCommand movedSwitchingCommand in movedSwitchingCommands)
					switchingCommands.Remove(movedSwitchingCommand);

				foreach(KeyValuePair<IModelComponentIsBusProvider, List<SwitchingCommand>> commandStationAndSwitchCommands in commandStationIdToSwitchingCommands) {
					if(commandStationAndSwitchCommands.Key.BatchMultipathSwitchingSupported) {
						tasks.Add(EventManager.AsyncEvent(new LayoutEvent(this, "change-batch-of-track-component-state-command", null, commandStationAndSwitchCommands.Value).SetOption("CommandStationID", XmlConvert.ToString(commandStationAndSwitchCommands.Key.Id))));
					}
					else {
						foreach(SwitchingCommand switchingCommand in commandStationAndSwitchCommands.Value)
							tasks.Add(EventManager.AsyncEvent(new LayoutEvent<ControlConnectionPointReference, int>("change-track-component-state-command", switchingCommand.ControlPointReference, switchingCommand.SwitchState).SetCommandStation(commandStationAndSwitchCommands.Key)));
					}
				}
			}

			return Task.WhenAll(tasks);
		}

		[LayoutEvent("track-component-state-changed-notification")]
		private void trackComponentStateChanged(LayoutEvent e) {
			ControlConnectionPoint connectionPoint = (ControlConnectionPoint)e.Sender;
			IModelComponentHasSwitchingState componentWithSwitchingState = connectionPoint.Component as IModelComponentHasSwitchingState;

			if(componentWithSwitchingState != null) {
				int state = (int)e.Info;
				bool reverseLogic = false;

				if(connectionPoint.Component is IModelComponentHasReverseLogic)
					reverseLogic = ((IModelComponentHasReverseLogic)connectionPoint.Component).ReverseLogic;

				componentWithSwitchingState.SetSwitchState(connectionPoint, reverseLogic ? 1 - state : state);
			}
		}

		#endregion

		#region Track Contact Component

		[LayoutEvent("track-contact-triggered-notification", SenderType=typeof(LayoutTrackContactComponent))]
		private void trackContactComponentTurnoutStateChanged(LayoutEvent e) {
			LayoutTrackContactComponent	trackContact = (LayoutTrackContactComponent)e.Sender;

			if(!trackContact.IsTriggered) {
				trackContact.IsTriggered = true;
				EventManager.DelayedEvent(200, new LayoutEvent(trackContact, "exit-track-contact-triggered-state"));
				EventManager.Event(new LayoutEvent(trackContact, "anonymous-track-contact-triggerd"));
			}
			else
				Trace.WriteLine("Track contact " + trackContact.FullDescription + " triggered twice in 200 milli");
				
		}

		[LayoutEvent("exit-track-contact-triggered-state")]
		private void exitTrackContactTriggeredState(LayoutEvent e) {
			LayoutTrackContactComponent	trackContact = (LayoutTrackContactComponent)e.Sender;

			trackContact.IsTriggered = false;
		}

		#endregion

		#region Signal Component

		[LayoutEvent("signal-state-changed-notification", SenderType=typeof(LayoutSignalComponent))]
		private void signalComponentTurnoutStateChanged(LayoutEvent e) {
			LayoutSignalComponent	signal = (LayoutSignalComponent)e.Sender;
			LayoutSignalState		state = (LayoutSignalState)e.Info;

			if(signal.Info.ReverseLogic)
				state = (state == LayoutSignalState.Red) ? LayoutSignalState.Green : LayoutSignalState.Red;

			signal.SetSignalState(state);
		}

		#endregion

		#region Train Detection Module

		[LayoutEvent("train-detection-state-changed-notification", SenderType=typeof(LayoutBlockDefinitionComponent))]
		private void trainDetectionStateChanged(LayoutEvent e) {
			LayoutBlockDefinitionComponent	blockDefinition = (LayoutBlockDefinitionComponent)e.Sender;
			bool						detected = (bool)e.Info;

			Trace.WriteLineIf(LocomotiveTracking.traceLocomotiveTracking.TraceInfo, "=-=-= Train " + (detected ? "detected" : "not detected") + " in block " + blockDefinition.FullDescription);
			blockDefinition.Info.TrainDetected = detected;
		}

		#endregion

		#region Linked signals

		[LayoutEvent("logical-signal-state-changed")]
		private void signalStateChanged(LayoutEvent e) {
			if(LayoutController.IsOperationMode) {
				LayoutBlockEdgeBase	blockEdge = (LayoutBlockEdgeBase)e.Sender;
				LayoutSignalState			signalState = (LayoutSignalState)e.Info;

				foreach(LinkedSignalInfo linkedSignal in blockEdge.LinkedSignals) {
					LayoutSignalComponent signalComponent = LayoutModel.Component<LayoutSignalComponent>(linkedSignal.SignalId, LayoutModel.ActivePhases);

					if(signalComponent != null)
						signalComponent.SignalState = signalState;
				}
			}
		}

		[LayoutEvent("enter-operation-mode", Order=2000)]
		private void setSignalsOnEnterOperationMode(LayoutEvent e) {
			foreach(LayoutBlockEdgeBase blockEdge in LayoutModel.Components<LayoutBlockEdgeBase>(LayoutModel.ActivePhases)) {
				LayoutSignalState	signalState = blockEdge.SignalState;

				foreach(LinkedSignalInfo linkedSignal in blockEdge.LinkedSignals) {
					LayoutSignalComponent signalComponent = LayoutModel.Component<LayoutSignalComponent>(linkedSignal.SignalId, LayoutModel.ActivePhases);

					if(signalComponent != null)
						signalComponent.SignalState = signalState;
				}
			}
		}

		#region Linked signal hash table, given a signal component it will return whether it is linked or not

		Dictionary<Guid, LayoutBlockEdgeBase> linkedSignalMap = null;

		protected Dictionary<Guid, LayoutBlockEdgeBase> LinkedSignalMap {
			get {
				if(linkedSignalMap == null) {
					linkedSignalMap = new Dictionary<Guid, LayoutBlockEdgeBase>();

					foreach(LayoutBlockEdgeBase blockEdgeBase in LayoutModel.Components<LayoutBlockEdgeBase>(LayoutModel.ActivePhases)) {
						foreach(LinkedSignalInfo linkedSignalInfo in blockEdgeBase.LinkedSignals)
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
			LayoutBlockEdgeBase blockEdge = (LayoutBlockEdgeBase)e.Sender;
			LayoutSignalComponent signalComponent = (LayoutSignalComponent)e.Info;
			Dictionary<Guid, LayoutBlockEdgeBase> map = LinkedSignalMap;

			if(!map.ContainsKey(signalComponent.Id)) {
				signalComponent.EraseImage();
				map.Add(signalComponent.Id, blockEdge);
				signalComponent.Redraw();
			}
			else
				Debug.Assert(false);
		}

		[LayoutEvent("signal-component-unlinked")]
		private void signalComponentUnlinked(LayoutEvent e) {
			LayoutBlockEdgeBase blockEdge = (LayoutBlockEdgeBase)e.Sender;
			LayoutSignalComponent signalComponent = (LayoutSignalComponent)e.Info;
			Dictionary<Guid, LayoutBlockEdgeBase> map = LinkedSignalMap;

			if(map.ContainsKey(signalComponent.Id)) {
				signalComponent.EraseImage();
				map.Remove(signalComponent.Id);
				signalComponent.Redraw();
			}
			else
				Debug.Assert(false);
		}

		[LayoutEvent("removed-from-model", SenderType = typeof(LayoutBlockEdgeBase))]
		private void removingBlockEdgeComponent(LayoutEvent e) {
			LayoutBlockEdgeBase removedBlockEdge = (LayoutBlockEdgeBase)e.Sender;

			if(removedBlockEdge.LinkedSignalsElement != null && removedBlockEdge.LinkedSignals.Count > 0) {
				Dictionary<Guid, LayoutBlockEdgeBase> map = LinkedSignalMap;
				List<Guid> removeList = new List<Guid>();

				foreach(KeyValuePair<Guid, LayoutBlockEdgeBase> signalIdBlockEdgePair in map)
					if(signalIdBlockEdgePair.Value == removedBlockEdge)
						removeList.Add(signalIdBlockEdgePair.Key);

				removeList.ForEach(delegate(Guid signalId) {
					LayoutSignalComponent signal = LayoutModel.Component<LayoutSignalComponent>(signalId, LayoutPhase.All);

					if(signal != null)
						EventManager.Event(new LayoutEvent(removedBlockEdge, "signal-component-unlinked", null, signal));
				});
			}
		}

		[LayoutEvent("component-configuration-changed", SenderType = typeof(LayoutBlockEdgeBase))]
		private void blockEdgeModified(LayoutEvent e) {
			LayoutBlockEdgeBase modifiedBlockEdge = (LayoutBlockEdgeBase)e.Sender;
			Dictionary<Guid, LayoutBlockEdgeBase> map = LinkedSignalMap;
			List<Guid> previousLinkedSignals = new List<Guid>();

			foreach(KeyValuePair<Guid, LayoutBlockEdgeBase> signalIdBlockEdgePair in map)
				if(signalIdBlockEdgePair.Value == modifiedBlockEdge)
					previousLinkedSignals.Add(signalIdBlockEdgePair.Key);

			foreach(LinkedSignalInfo linkedSignalInfo in modifiedBlockEdge.LinkedSignals) {
				if(previousLinkedSignals.Contains(linkedSignalInfo.SignalId))
					previousLinkedSignals.Remove(linkedSignalInfo.SignalId);
				else
					EventManager.Event(new LayoutEvent(modifiedBlockEdge, "signal-component-linked", null, LayoutModel.Component<LayoutSignalComponent>(linkedSignalInfo.SignalId, LayoutPhase.All)));
			}

			// The signals that are left, are not linked now, so remove them from the map
			foreach(Guid signalId in previousLinkedSignals)
				EventManager.Event(new LayoutEvent(modifiedBlockEdge, "signal-component-unlinked", null, LayoutModel.Component<LayoutSignalComponent>(signalId, LayoutPhase.All)));
		}

		#endregion

		#endregion
	}
}
