using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using MethodDispatcher;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

namespace DiMAX {
    public class DiMAXcommandStationEmulator : ILayoutCommandStationEmulator {
        private readonly Guid commandStationId;
        private readonly string pipeName;
        private readonly CancellationTokenSource stopInterfaceThrad;
        private readonly Task interfaceTask;

        private FileStream? commStream;
        private readonly ILayoutEmulatorServices layoutEmulationServices;
        private readonly ILayoutInterThreadEventInvoker interThreadEventInvoker;
        private readonly Dictionary<int, PositionEntry> positions = new();
        private static readonly LayoutTraceSwitch traceDiMAXemulator = new("TraceDiMAXemulator", "Trace DiMAX command station emulation");

        public DiMAXcommandStationEmulator(IModelComponentIsCommandStation commandStation, string pipeName) {
            this.commandStationId = commandStation.Id;
            this.pipeName = pipeName;
            this.interThreadEventInvoker = Dispatch.Call.GetInterthreadInvoker();

            traceDiMAXemulator.Level = TraceLevel.Off;      // Until it seems to work

            layoutEmulationServices = Ensure.NotNull<ILayoutEmulatorServices>(EventManager.Event(new LayoutEvent("get-layout-emulation-services", this)));
            EventManager.Event(new LayoutEvent("initialize-layout-emulation", this)
                .SetOption(LayoutCommandStationComponent.Option_EmulateTrainMotion, commandStation.EmulateTrainMotion)
                .SetOption(LayoutCommandStationComponent.Option_EmulationTickTime, commandStation.EmulationTickTime)
            );

            stopInterfaceThrad = new CancellationTokenSource();

            interfaceTask = InterfaceThreadFunction(stopInterfaceThrad.Token);
        }

        public void InterfaceThreadError(object? subject, string message) {
            interThreadEventInvoker.Queue(() => Dispatch.Call.AddError(message, subject));
        }

        public void InterfaceThreadWarning(object? subject, string message) {
            interThreadEventInvoker.Queue(() => Dispatch.Call.AddWarning(message, subject));
        }

        private async Task InterfaceThreadFunction(CancellationToken stopMe) {
            commStream = Ensure.NotNull<FileStream>(EventManager.Event(new LayoutEvent("wait-named-pipe-request", pipeName, true)));
            layoutEmulationServices.LocomotiveMoved += LayoutEmulationServices_LocomotiveMoved;
            layoutEmulationServices.LocomotiveFallFromTrack += (s, ea) => InterfaceThreadError(ea.Location.Track, "Locomotive (address " + ea.Unit + ") fall from track");

            try {
                while (true) {
                    var commandAndLengthBuffer = new byte[1];

                    await commStream.ReadAsync(commandAndLengthBuffer.AsMemory(0, 1), stopMe);
                    if (stopMe.IsCancellationRequested)
                        break;

                    byte commandAndLength = commandAndLengthBuffer[0];

                    int length = (commandAndLength & 0xe0) >> 5;
                    byte[] buffer = new byte[length + 1];       // 1 more byte for the xor byte

                    for (int i = 0; i < buffer.Length;) {
                        i += await commStream.ReadAsync(buffer.AsMemory(i, buffer.Length - i), stopMe);
                        if (stopMe.IsCancellationRequested)
                            break;
                    }

                    if (stopMe.IsCancellationRequested)
                        break;

                    var packet = new DiMAXpacket(commandAndLength, buffer);

                    if (traceDiMAXemulator.TraceVerbose)
                        packet.Dump("DiMAX emulator received packet: ");

                    switch (packet.CommandCode) {
                        case DiMAXcommandCode.LocoSpeedControl: {
                                int unit = ((int)(packet.Parameters[0] & 0x3f) << 8) | packet.Parameters[1];
                                int speed = packet.Parameters[2] & 0x7f;

                                layoutEmulationServices.SetLocomotiveSpeed(commandStationId, unit, speed);
                                layoutEmulationServices.SetLocomotiveDirection(commandStationId, unit, (packet.Parameters[2] & 0x80) != 0 ? LocomotiveOrientation.Forward : LocomotiveOrientation.Backward);
                            }
                            break;

                        case DiMAXcommandCode.TurnoutControl: {
                                int address = ((packet.Parameters[0] & 0x3f) << 6) | ((packet.Parameters[1] & 0xfc) >> 2);
                                int switchState = packet.Parameters[1] & 1;

                                layoutEmulationServices.SetTurnoutState(commandStationId, address, switchState);
                            }
                            break;

                        case DiMAXcommandCode.EmergencyStopBegin:
                            layoutEmulationServices.StopEmulation();
                            break;

                        case DiMAXcommandCode.EmergencyStopFinish:
                            layoutEmulationServices.StartEmulation();
                            break;

                        case DiMAXcommandCode.LocoSelection: {
                                int unit = ((int)(packet.Parameters[0] & 0x3f) << 8) | packet.Parameters[1];
                                byte[] directReply = new byte[7] {
                                    0x40, 0, 4, 0x81, (byte)((unit >> 8) & 0x3f), (byte)(unit & 0xff), 100
                                };

                                byte xor = 0;
                                foreach (byte b in directReply)
                                    xor ^= b;
                                directReply[1] = xor;

                                // Send direct reply for loco selection
                                commStream.Write(directReply, 0, directReply.Length);
                            }
                            break;

                        default:
                            Trace.WriteLineIf(traceDiMAXemulator.TraceVerbose, "Command " + packet.CommandCode.ToString() + " is ignored by DiMAX emulator");
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

                        var connectionPointPerAddress = connectionPoint.Module.ModuleType.ConnectionPointsPerAddress;
                        int address = connectionPoint.Module.Address + (connectionPoint.Index / connectionPointPerAddress);

                        var triggerMessage = new DiMAXpacket(DiMAXcommandCode.FeedbackControl, new byte[] {
                                              (byte)((address >> 6) & 0x3f), (byte)(((address & 0x3f) << 2) | ((byte)(connectionPoint.Index % connectionPointPerAddress)))
                        });

                        if (traceDiMAXemulator.TraceInfo)
                            triggerMessage.Dump("Sending DiMAX message (track contact: " + address + "ab"[connectionPoint.Index % connectionPointPerAddress] + ")");

                        byte[] buffer = triggerMessage.GetBuffer();
                        commStream?.Write(buffer, 0, buffer.Length);
                        commStream?.Flush();

                        positions[e.Unit] = new PositionEntry(e.Location.Edge, e.Direction, e.Speed);
                    }
                }
            }
        }

        #region IDisposable Members

        public async void Dispose() {
            stopInterfaceThrad.Cancel();
            await interfaceTask;

            layoutEmulationServices.LocomotiveMoved -= LayoutEmulationServices_LocomotiveMoved;

            commStream?.Close();
            commStream = null;
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}