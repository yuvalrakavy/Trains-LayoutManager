using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Collections.Generic;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

namespace DiMAX {
    public class DiMAXcommandStationEmulator : ILayoutCommandStationEmulator {
        private Guid commandStationId;
        private readonly string pipeName;

        private FileStream commStream;
        private readonly ILayoutEmulatorServices layoutEmulationServices;
        private Thread interfaceThread = null;
        private readonly ILayoutInterThreadEventInvoker interThreadEventInvoker;
        private readonly Dictionary<int, PositionEntry> positions = new Dictionary<int, PositionEntry>();
        private static readonly LayoutTraceSwitch traceDiMAXemulator = new LayoutTraceSwitch("TraceDiMAXemulator", "Trace DiMAX command station emulation");

        public DiMAXcommandStationEmulator(IModelComponentIsCommandStation commandStation, string pipeName, int emulationTickTime) {
            this.commandStationId = commandStation.Id;
            this.pipeName = pipeName;
            this.interThreadEventInvoker = (ILayoutInterThreadEventInvoker)EventManager.Event(new LayoutEvent("get-inter-thread-event-invoker", this));

            traceDiMAXemulator.Level = TraceLevel.Off;      // Until it seems to work

            layoutEmulationServices = (ILayoutEmulatorServices)EventManager.Event(new LayoutEvent("get-layout-emulation-services", this));
            EventManager.Event(new LayoutEvent("initialize-layout-emulation", null, emulationTickTime));

            interfaceThread = new Thread(new ThreadStart(InterfaceThreadFunction)) {
                Name = "Command station emulation for " + commandStation.Name
            };
            interfaceThread.Start();
        }

        public void InterfaceThreadError(object subject, string message) {
            interThreadEventInvoker.QueueEvent(new LayoutEvent("add-error", subject, message));
        }

        public void InterfaceThreadWarning(object subject, string message) {
            interThreadEventInvoker.QueueEvent(new LayoutEvent("add-warning", subject, message));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void InterfaceThreadFunction() {
            commStream = (FileStream)EventManager.Event(new LayoutEvent("wait-named-pipe-request", pipeName, true));
            layoutEmulationServices.LocomotiveMoved += new EventHandler<LocomotiveMovedEventArgs>(layoutEmulationServices_LocomotiveMoved);
            layoutEmulationServices.LocomotiveFallFromTrack += (s, ea) => InterfaceThreadError(ea.Location.Track, "Locomotive (address " + ea.Unit + ") fall from track");

            try {
                while (true) {
                    byte commandAndLength = (byte)commStream.ReadByte();
                    int length = ((commandAndLength & 0xe0) >> 5);
                    byte[] buffer = new byte[length + 1];       // 1 more byte for the xor byte

                    for (int i = 0; i < buffer.Length;)
                        i += commStream.Read(buffer, i, buffer.Length - i);

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

        private void layoutEmulationServices_LocomotiveMoved(object sender, LocomotiveMovedEventArgs e) {
            if (e.CommandStationId == this.commandStationId) {
                if (e.Location.Track.TrackContactComponent != null) {
                    positions.TryGetValue(e.Unit, out PositionEntry position);

                    if (position == null || e.Location.Edge != position.Edge || e.Direction != position.Direction) {
                        LayoutTrackContactComponent trackContact = e.Location.Track.TrackContactComponent;
                        ControlConnectionPoint connectionPoint = LayoutModel.ControlManager.ConnectionPoints[trackContact][0];

                        var connectionPointPerAddress = connectionPoint.Module.ModuleType.ConnectionPointsPerAddress;
                        int address = connectionPoint.Module.Address + connectionPoint.Index / connectionPointPerAddress;

                        var triggerMessage = new DiMAXpacket(DiMAXcommandCode.FeedbackControl, new byte[] {
                                              (byte)((address >> 6) & 0x3f), (byte)(((address & 0x3f) << 2) | ((byte)(connectionPoint.Index % connectionPointPerAddress)))
                        });

                        if (traceDiMAXemulator.TraceInfo)
                            triggerMessage.Dump("Sending DiMAX message (track contact: " + address + "ab"[connectionPoint.Index % connectionPointPerAddress] + ")");

                        byte[] buffer = triggerMessage.GetBuffer();
                        commStream.Write(buffer, 0, buffer.Length);
                        commStream.Flush();

                        positions[e.Unit] = new PositionEntry(e.Location.Edge, e.Direction, e.Speed);
                    }
                }
            }
        }

        #region IDisposable Members

        public void Dispose() {
            if (interfaceThread != null) {
                if (interfaceThread.IsAlive)
                    interfaceThread.Abort();
                interfaceThread = null;
            }

            layoutEmulationServices.LocomotiveMoved -= new EventHandler<LocomotiveMovedEventArgs>(layoutEmulationServices_LocomotiveMoved);

            commStream.Close();
            commStream = null;
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}