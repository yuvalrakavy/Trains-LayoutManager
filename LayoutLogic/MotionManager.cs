using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;
using LayoutManager.Model;

#pragma warning disable IDE0051,IDE0060
#nullable enable
namespace LayoutManager.Logic {
    /// <summary>
    /// Handle train motion issues, such as acceleration
    /// </summary>
    [LayoutModule("Train Motion Manager", UserControl = false)]
    public class TrainMotionManager : LayoutModuleBase {
        private readonly Dictionary<string, CommandStationCapabilitiesInfo> commandStationCapabiltiesMap = new Dictionary<string, CommandStationCapabilitiesInfo>();
        private string? _commandStationName;
        private CommandStationCapabilitiesInfo? _commandStationCapabilties;
        private readonly Dictionary<Guid, SpeedChangeState> trainIDtoSpeedChangeState = new Dictionary<Guid, SpeedChangeState>();

        [LayoutEvent("train-speed-change-request")]
        private void trainSpeedChangeRequest(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");
            var scp = Ensure.NotNull<TrainSpeedChangeParameters>(e.Info, "scp");
            MotionRampInfo ramp = scp.Ramp;
            int requestedSpeed = scp.RequestedSpeed;

            if (train.Speed != requestedSpeed) {
                train.FinalSpeed = requestedSpeed;

                int speedDiff = requestedSpeed - train.Speed;
                int startSpeed = train.Speed;
                int targetSpeed = requestedSpeed;
                double logicalToTrainSpeedStepsFactor = (double)train.SpeedSteps / (double)LayoutModel.Instance.LogicalSpeedSteps;

                // Ensure that speed diff is in decoder speed steps
                if (LayoutModel.Instance.LogicalSpeedSteps != train.SpeedSteps) {
                    startSpeed = (int)Math.Round(train.Speed * logicalToTrainSpeedStepsFactor);
                    speedDiff = (int)Math.Round(logicalToTrainSpeedStepsFactor * speedDiff);
                    targetSpeed = startSpeed + speedDiff;
                }

                if (ramp.RampType == MotionRampType.LocomotiveHardware)
                    train.SpeedInSteps = targetSpeed;           // Just send the new speed setting
                else {
                    int motionTime;

                    if (trainIDtoSpeedChangeState.TryGetValue(train.Id, out SpeedChangeState speedChangeState))
                        speedChangeState.Dispose();

                    speedChangeState = new SpeedChangeState(train) {
                        LogicalToTrainSpeedStepsFactor = logicalToTrainSpeedStepsFactor
                    };

                    int steps = Math.Abs(speedDiff);

                    speedChangeState.ExpectedSpeed = startSpeed;
                    speedChangeState.StepsPerTick = 1;
                    speedChangeState.TargetSpeedSteps = startSpeed + speedDiff;

                    speedChangeState.Ticks = steps;

                    if (ramp.RampType == MotionRampType.TimeFixed)
                        motionTime = ramp.MotionTime;
                    else if (ramp.RampType == MotionRampType.RateFixed) {
                        speedChangeState.TimePerTick = 1000 / ((int)Math.Round(ramp.SpeedChangeRate * speedChangeState.LogicalToTrainSpeedStepsFactor));
                        motionTime = speedChangeState.Ticks * speedChangeState.TimePerTick;
                    }
                    else {
                        Debug.Assert(false, "Unknown ramp type: " + ramp.RampType);
                        return;
                    }

                    CommandStationCapabilitiesInfo commandStationCapabilities = GetCommandStationCapabilities(train);
                    int minTickTime = commandStationCapabilities.MinTimeBetweenSpeedSteps;

                    // Make sure that the train speed it not changed in a rate that will violate the minimum time
                    // to wait between two consecutive speed change commands.
                    //** BUG here speadChangeState.Ticks
                    while (speedChangeState.Ticks > 0 && (speedChangeState.TimePerTick = motionTime / speedChangeState.Ticks) < minTickTime) {
                        speedChangeState.StepsPerTick++;        // try to incremement the speed by more steps in order to reduce the
                                                                // number of ticks and thus reducing their rate
                        speedChangeState.Ticks = steps / speedChangeState.StepsPerTick;
                    }

                    int actualSteps = speedChangeState.Ticks * speedChangeState.StepsPerTick;
                    int residualSteps = steps - actualSteps;

                    if (residualSteps > 0)
                        speedChangeState.AddStepTickCount = speedChangeState.Ticks / residualSteps;
                    else
                        speedChangeState.AddStepTickCount = 0;

                    if (speedDiff < 0)
                        speedChangeState.StepsPerTick = -speedChangeState.StepsPerTick;

                    trainIDtoSpeedChangeState.Add(train.Id, speedChangeState);
                    speedChangeState.Start();
                }
            }
        }

        [LayoutEvent("train-speed-change-abort")]
        private void trainSpeedChangeAbort(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");

            if (trainIDtoSpeedChangeState.TryGetValue(train.Id, out SpeedChangeState speedStateChange))
                speedStateChange.Dispose();
        }

        [LayoutEvent("train-speed-change-done")]
        [LayoutEvent("train-speed-change-aborted")]
        private void removeSpeedChangeState(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");

            trainIDtoSpeedChangeState.Remove(train.Id);
        }

        [LayoutEvent("train-speed-change-timer")]
        private void trainSpeedChnageTimer(LayoutEvent e) {
            SpeedChangeState speedChangeState = (SpeedChangeState)e.Sender;

            speedChangeState.OnTimer(e);
        }

        [LayoutEvent("exit-operation-mode")]
        private void exitOperationMode(LayoutEvent e) {
            List<SpeedChangeState> activeSpeedChanges = new List<SpeedChangeState>();

            foreach (SpeedChangeState speedChangeState in trainIDtoSpeedChangeState.Values)
                activeSpeedChanges.Add(speedChangeState);

            // TODO: Probably need to stop each train
            foreach (SpeedChangeState speedChangeState in activeSpeedChanges)
                speedChangeState.Dispose();

            _commandStationCapabilties = null;
            _commandStationName = null;
            commandStationCapabiltiesMap.Clear();
        }

        #region Utility methods

        /// <summary>
        /// Return the command station capabilties for a given train
        /// </summary>
        private CommandStationCapabilitiesInfo GetCommandStationCapabilities(TrainStateInfo train) {
            if (train.CommandStation != null) {
                string commandStationName = train.CommandStation.NameProvider.Name;

                // Fast cache
                if (commandStationName != _commandStationName) {
                    _commandStationName = commandStationName;

                    if (!commandStationCapabiltiesMap.TryGetValue(commandStationName, out _commandStationCapabilties)) {
                        var commandStationCapabilitiesElement = Ensure.NotNull<XmlElement>(EventManager.Event(new LayoutEvent("get-command-station-capabilities").SetCommandStation(train)),
                            $"Command station {commandStationName} does not return capabilities information");

                        _commandStationCapabilties = new CommandStationCapabilitiesInfo(commandStationCapabilitiesElement);
                        commandStationCapabiltiesMap.Add(commandStationName, _commandStationCapabilties);
                    }
                }
            }

            Debug.Assert(_commandStationCapabilties != null);
            return _commandStationCapabilties;
        }

        #endregion

        #region Data structures

        private struct SpeedChangeState : IDisposable {
            private readonly TrainStateInfo train;
            private double logicalToTrainSpeedStepsFactor;
            private double trainToLogicalSpeedStepsFactor;

            private int tickCount;
            private LayoutDelayedEvent? delayedEvent;
            private int nextSpeed;

            public SpeedChangeState(TrainStateInfo train) {
                this.train = train;
                tickCount = 0;
                delayedEvent = null;
                logicalToTrainSpeedStepsFactor = 1.0;
                trainToLogicalSpeedStepsFactor = 1.0;
                TimePerTick = 10;
                StepsPerTick = 1;
                Ticks = 0;
                AddStepTickCount = 0;
                ExpectedSpeed = 0;
                nextSpeed = 0;
                TargetSpeedSteps = 0;
            }

            #region Properties

            public int TargetSpeedSteps { get; set; }

            public int Ticks { get; set; }

            public int TimePerTick { get; set; }

            public int StepsPerTick { get; set; }

            public int AddStepTickCount { get; set; }

            public int ExpectedSpeed { get; set; }

            public double LogicalToTrainSpeedStepsFactor {
                get {
                    return logicalToTrainSpeedStepsFactor;
                }

                set {
                    logicalToTrainSpeedStepsFactor = value;
                    trainToLogicalSpeedStepsFactor = 1 / value;
                }
            }

            public double TrainToLogicalSpeedStepsFactor {
                get {
                    return trainToLogicalSpeedStepsFactor;
                }

                set {
                    trainToLogicalSpeedStepsFactor = value;
                    logicalToTrainSpeedStepsFactor = 1 / trainToLogicalSpeedStepsFactor;
                }
            }

            #endregion

            #region Operations

            public void Start() {
                nextSpeed = ExpectedSpeed + StepsPerTick;
                train.SpeedInSteps = nextSpeed;
                ExpectedSpeed = nextSpeed;
                setTimer();
            }

            private void setTimer() {
                if (ExpectedSpeed != TargetSpeedSteps) {
                    if (Math.Abs(TargetSpeedSteps - ExpectedSpeed) < Math.Abs(StepsPerTick))
                        nextSpeed = TargetSpeedSteps;
                    else {
                        nextSpeed = ExpectedSpeed + StepsPerTick;

                        if (AddStepTickCount > 0 && ((tickCount % AddStepTickCount) == 0))
                            nextSpeed += (StepsPerTick > 0) ? 1 : -1;
                    }

                    delayedEvent = EventManager.DelayedEvent(TimePerTick, new LayoutEvent("train-speed-change-timer", this));
                }
                else {
                    delayedEvent = null;
                    EventManager.Event(new LayoutEvent("train-speed-change-done", train));
                }
            }

            public void OnTimer(LayoutEvent e) {
                if (train.SpeedInSteps != ExpectedSpeed)
                    EventManager.Event(new LayoutEvent("train-speed-change-aborted", train));
                else {
                    train.SpeedInSteps = nextSpeed;
                    ExpectedSpeed = nextSpeed;

                    tickCount++;
                    setTimer();     // This calculates the next speed
                }
            }

            public void Dispose() {
                if (delayedEvent != null) {
                    delayedEvent.Cancel();
                    delayedEvent = null;
                }
                EventManager.Event(new LayoutEvent("train-speed-change-aborted", train));
            }

            #endregion
        }

        #endregion
    }
}
