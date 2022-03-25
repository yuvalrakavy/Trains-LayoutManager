using LayoutManager;
using LayoutManager.Components;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace LayoutLGB {
    public class MTScommandStationEmulator : ILayoutCommandStationEmulator {
        private static readonly LayoutTraceSwitch traceMTSemulator = new("TraceMTSemulator", "Trace MTS command station emulation");
        private readonly Guid commandStationId;
        private readonly string pipeName;
        private readonly Dictionary<int, PositionEntry> positions = new();
        private readonly CancellationTokenSource stopInterfaceThrad;
        private readonly Task interfaceTask;

        private volatile NamedPipeServerStream? commStream;
        private readonly ILayoutEmulatorServices layoutEmulationServices;

        public MTScommandStationEmulator(IModelComponentIsCommandStation commandStation, string pipeName) {
            this.commandStationId = commandStation.Id;
            this.pipeName = pipeName;

            layoutEmulationServices = Dispatch.Call.GetLayoutEmulationServices();

            Dispatch.Call.InitializeLayoutEmulation(commandStation.EmulateTrainMotion, commandStation.EmulationTickTime);

            stopInterfaceThrad = new CancellationTokenSource();
            interfaceTask = InterfaceThreadFunction(stopInterfaceThrad.Token);
        }

        public async void Dispose() {
            stopInterfaceThrad.Cancel();
            await interfaceTask;

            layoutEmulationServices.LocomotiveMoved -= LayoutEmulationServices_LocomotiveMoved;

            commStream?.Close();
            commStream = null;
            GC.SuppressFinalize(this);
        }

        private async Task InterfaceThreadFunction(CancellationToken stopMe) {
            // Create the pipe for communication
            commStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            layoutEmulationServices.LocomotiveMoved += LayoutEmulationServices_LocomotiveMoved;

            try {
                await commStream.WaitForConnectionAsync(stopMe);

                while (!stopMe.IsCancellationRequested) {
                    byte[] buffer = new byte[4];

                    await commStream.ReadAsync(buffer.AsMemory(0, 4), stopMe);

                    if (stopMe.IsCancellationRequested)
                        break;

                    var command = new MTSmessage(buffer);

                    Trace.WriteLineIf(traceMTSemulator.TraceInfo, "Command station emulator, got command: " + command.ToString());

                    switch (command.Command) {
                        case MTScommand.PowerStation:
                            layoutEmulationServices.StartEmulation();
                            break;

                        case MTScommand.EmergencyStop:
                            layoutEmulationServices.StopEmulation();
                            break;

                        case MTScommand.LocomotiveMotion: {
                                byte speedValue = command.Value;
                                int speed;
                                LocomotiveOrientation direction = LocomotiveOrientation.Forward;

                                if (speedValue >= 0x21)
                                    speed = speedValue - 0x21;
                                else {
                                    direction = LocomotiveOrientation.Backward;
                                    speed = speedValue - 1;
                                }

                                layoutEmulationServices.SetLocomotiveDirection(commandStationId, command.Address, direction);
                                layoutEmulationServices.SetLocomotiveSpeed(commandStationId, command.Address, speed);
                                break;
                            }

                        case MTScommand.TurnoutControl:
                            layoutEmulationServices.SetTurnoutState(commandStationId, command.Address, command.Value);
                            break;
                    }
                }
            }
            catch (Exception ex) {
                Trace.WriteLine("InterfaceThread terminated as a result of an exception: " + ex.GetType().Name + " message: " + ex.Message);
            }
        }

        private class PositionEntry {
            public TrackEdge Edge { get; set; }

            public int Speed { get; set; }

            public LocomotiveOrientation Direction { get; set; }

            public PositionEntry(TrackEdge edge, LocomotiveOrientation direction, int speed) {
                this.Edge = edge;
                this.Speed = speed;
                this.Direction = direction;
            }
        }

        private void LayoutEmulationServices_LocomotiveMoved(object? sender, LocomotiveMovedEventArgs e) {
            if (e.CommandStationId == this.commandStationId) {
                if (e.Location.Track.TrackContactComponent != null) {
                    positions.TryGetValue(e.Unit, out PositionEntry? position);

                    if (position == null || e.Location.Edge != position.Edge || e.Direction != position.Direction) {
                        LayoutTrackContactComponent trackContact = e.Location.Track.TrackContactComponent;
                        var connectionPoint = Ensure.NotNull<ControlConnectionPoint>(LayoutModel.ControlManager.ConnectionPoints[trackContact]?[0]);
                        int address = connectionPoint.Module.Address + (connectionPoint.Index / 2);

                        var triggerMessage = new MTSmessage(MTScommand.TurnoutControl, (byte)address, (byte)(connectionPoint.Index & 1));

                        Trace.WriteLineIf(traceMTSemulator.TraceInfo, "Sending MTS message " + triggerMessage.ToString());
                        commStream?.Write(triggerMessage.Buffer, 0, 4);
                        commStream?.Flush();

                        positions[e.Unit] = new PositionEntry(e.Location.Edge, e.Direction, e.Speed);
                    }
                }
            }
        }
    }
}

