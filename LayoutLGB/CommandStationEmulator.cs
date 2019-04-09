using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutLGB {
    public class MTScommandStationEmulator : ILayoutCommandStationEmulator {
        private static readonly LayoutTraceSwitch traceMTSemulator = new LayoutTraceSwitch("TraceMTSemulator", "Trace MTS command station emulation");
        private Guid commandStationId;
        private readonly string pipeName;
        private readonly Dictionary<int, PositionEntry> positions = new Dictionary<int, PositionEntry>();

        private volatile FileStream commStream;
        private readonly ILayoutEmulatorServices layoutEmulationServices;
        private Thread interfaceThread = null;

        public MTScommandStationEmulator(IModelComponentIsCommandStation commandStation, string pipeName, int emulationTickTime) {
            this.commandStationId = commandStation.Id;
            this.pipeName = pipeName;

            layoutEmulationServices = (ILayoutEmulatorServices)EventManager.Event(new LayoutEvent("get-layout-emulation-services", this));

            EventManager.Event(new LayoutEvent("initialize-layout-emulation", null, emulationTickTime));

            interfaceThread = new Thread(new ThreadStart(InterfaceThreadFunction)) {
                Name = "Command station emulation for " + commandStation.Name
            };
            interfaceThread.Start();
        }

        public void Dispose() {
            layoutEmulationServices.LocomotiveMoved -= new EventHandler<LocomotiveMovedEventArgs>(layoutEmulationServices_LocomotiveMoved);

            if (interfaceThread != null) {
                if (interfaceThread.IsAlive)
                    interfaceThread.Abort();
                interfaceThread = null;
            }

            commStream.Close();
            commStream = null;
        }

        private void InterfaceThreadFunction() {
            // Create the pipe for communication

            commStream = (FileStream)EventManager.Event(new LayoutEvent("wait-named-pipe-request", pipeName, true));
            layoutEmulationServices.LocomotiveMoved += new EventHandler<LocomotiveMovedEventArgs>(layoutEmulationServices_LocomotiveMoved);

            try {
                while (true) {
                    byte[] buffer = new byte[4];

                    commStream.Read(buffer, 0, 4);

                    MTSmessage command = new MTSmessage(buffer);

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

        private void layoutEmulationServices_LocomotiveMoved(object sender, LocomotiveMovedEventArgs e) {
            if (e.CommandStationId == this.commandStationId) {
                if (e.Location.Track.TrackContactComponent != null) {
                    positions.TryGetValue(e.Unit, out PositionEntry position);

                    if (position == null || e.Location.Edge != position.Edge || e.Direction != position.Direction) {
                        LayoutTrackContactComponent trackContact = e.Location.Track.TrackContactComponent;
                        ControlConnectionPoint connectionPoint = LayoutModel.ControlManager.ConnectionPoints[trackContact][0];
                        int address = connectionPoint.Module.Address + connectionPoint.Index / 2;

                        MTSmessage triggerMessage = new MTSmessage(MTScommand.TurnoutControl, (byte)address, (byte)(connectionPoint.Index & 1));

                        Trace.WriteLineIf(traceMTSemulator.TraceInfo, "Sending MTS message " + triggerMessage.ToString());
                        commStream.Write(triggerMessage.Buffer, 0, 4);
                        commStream.Flush();

                        positions[e.Unit] = new PositionEntry(e.Location.Edge, e.Direction, e.Speed);
                    }
                }
            }
        }
    }
}

