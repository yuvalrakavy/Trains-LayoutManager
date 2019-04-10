using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

namespace MarklinDigital {
    public class MarkliinCommandStationEmulator : ILayoutCommandStationEmulator {
        private Guid commandStationId;
        private readonly string pipeName;

        private FileStream commStream;
        private readonly ILayoutEmulatorServices layoutEmulationServices;
        private readonly Thread interfaceThread = null;
        private readonly UInt16[] feedbackDecoders = new UInt16[31];
        private bool resetMode = true;
        private Timer feedbackTimer = null;

        public MarkliinCommandStationEmulator(IModelComponentIsCommandStation commandStation, string pipeName, int emulationTickTime) {
            this.commandStationId = commandStation.Id;
            this.pipeName = pipeName;

            layoutEmulationServices = (ILayoutEmulatorServices)EventManager.Event(new LayoutEvent("get-layout-emulation-services", this));

            EventManager.Event(new LayoutEvent("initialize-layout-emulation", this)
                .SetOption(LayoutCommandStationComponent.Option_EmulateTrainMotion, commandStation.EmulateTrainMotion)
                .SetOption(LayoutCommandStationComponent.Option_EmulationTickTime, commandStation.EmulationTickTime)
            );

            interfaceThread = new Thread(new ThreadStart(InterfaceThreadFunction)) {
                Name = "Command station emulation for " + commandStation.Name
            };
            interfaceThread.Start();

            feedbackTimer = new Timer(new TimerCallback(readFeedbacksCallback), null, 0, 200);
        }

        public void Dispose() {
            if (feedbackTimer != null) {
                feedbackTimer.Dispose();
                feedbackTimer = null;
            }

            if (interfaceThread != null) {
                if (interfaceThread.IsAlive)
                    interfaceThread.Abort();
            }

            commStream.Close();
        }

        private void InterfaceThreadFunction() {
            // Create the pipe for communication

            commStream = (FileStream)EventManager.Event(new LayoutEvent("wait-named-pipe-request", pipeName, false));

            try {
                while (true) {
                    byte command = (byte)commStream.ReadByte();

                    if (command < 32) {
                        int unit = commStream.ReadByte();

                        // Locomotive command
                        command &= 0xf;         // The state of the aux function is ignored

                        if (command != 0xf)
                            layoutEmulationServices.SetLocomotiveSpeed(commandStationId, unit, (int)command);
                        else
                            layoutEmulationServices.ToggleLocomotiveDirection(commandStationId, unit);
                    }
                    else if (command == 33 || command == 34) {
                        int unit = commStream.ReadByte();

                        layoutEmulationServices.SetTurnoutState(commandStationId, unit, (command == 33) ? 0 : 1);
                    }
                    else if (command == 96)
                        layoutEmulationServices.StartEmulation();
                    else if (command == 97)
                        layoutEmulationServices.StopEmulation();
                    else if (command == 128)
                        resetMode = false;
                    else if (129 <= command && command <= 160) {
                        for (int i = 0; i < command - 128; i++)
                            sendFeedback(i);
                    }
                    else if (command == 192)
                        resetMode = true;
                    else if (193 <= command && command <= 224)
                        sendFeedback(command - 193);
                }
            }
            catch (Exception ex) {
                Trace.WriteLine("InterfaceThread terminated as a result of an exception: " + ex.GetType().Name + " message: " + ex.Message);
            }
        }

        private void sendFeedback(int feedbackIndex) {
            lock (feedbackDecoders) {
                commStream.WriteByte((byte)((feedbackDecoders[feedbackIndex] >> 8) & 0xff));
                commStream.WriteByte((byte)(feedbackDecoders[feedbackIndex] & 0xff));
            }
        }

        private void readFeedbacksCallback(object state) {
            IList<ILocomotiveLocation> locomotiveLocations = layoutEmulationServices.GetLocomotiveLocations(commandStationId);

            if (resetMode) {
                for (int i = 0; i < 31; i++)
                    feedbackDecoders[i] = 0;
            }

            lock (feedbackDecoders) {
                foreach (ILocomotiveLocation location in locomotiveLocations) {
                    LayoutBlockDefinitionComponent blockInfo = location.Track.GetBlock(location.Front).BlockDefinintion;

                    if (blockInfo != null && blockInfo.Info.IsOccupancyDetectionBlock) {
                        ControlConnectionPoint connectionPoint = LayoutModel.ControlManager.ConnectionPoints[blockInfo][0];

                        int decoderIndex = connectionPoint.Module.Address - 1;
                        int contactIndex = 15 - connectionPoint.Index;

                        feedbackDecoders[decoderIndex] |= (UInt16)(1 << contactIndex);
                    }
                }
            }
        }
    }
}

