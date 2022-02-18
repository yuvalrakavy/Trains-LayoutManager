using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MethodDispatcher;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

namespace MarklinDigital {
    public class MarklinCommandStationEmulator : ILayoutCommandStationEmulator {
        private readonly Guid commandStationId;
        private readonly string pipeName;

        private FileStream? commStream;
        private readonly ILayoutEmulatorServices layoutEmulationServices;
        private readonly CancellationTokenSource stopInterfaceThrad;
        private readonly Task interfaceTask;
        private readonly UInt16[] feedbackDecoders = new UInt16[31];
        private bool resetMode = true;
        private readonly Timer? feedbackTimer = null;

        public MarklinCommandStationEmulator(IModelComponentIsCommandStation commandStation, string pipeName, int emulationTickTime) {
            this.commandStationId = commandStation.Id;
            this.pipeName = pipeName;

            layoutEmulationServices = Dispatch.Call.GetLayoutEmulationServices();

            Dispatch.Call.InitializeLayoutEmulation(commandStation.EmulateTrainMotion, commandStation.EmulationTickTime);

            stopInterfaceThrad = new CancellationTokenSource();
            interfaceTask = InterfaceThreadFunction(stopInterfaceThrad.Token);
            feedbackTimer = new Timer(new TimerCallback(ReadFeedbacksCallback), null, 0, emulationTickTime);
        }

        public async void Dispose() {
            feedbackTimer?.Dispose();
            stopInterfaceThrad.Cancel();
            await interfaceTask;
            commStream?.Close();

            GC.SuppressFinalize(this);
        }

        private async Task InterfaceThreadFunction(CancellationToken stopMe) {
            // Create the pipe for communication

            commStream = Dispatch.Call.WaitNamedPipeRequest(pipeName, false);

            try {
                while (true) {
                    var byteBuffer = new byte[1];

                    await commStream.ReadAsync(byteBuffer, stopMe);
                    if (stopMe.IsCancellationRequested)
                        break;
                    byte command = byteBuffer[0];

                    if (command < 32) {
                        await commStream.ReadAsync(byteBuffer, stopMe);
                        if (stopMe.IsCancellationRequested)
                            break;
                        int unit = byteBuffer[0];

                        // Locomotive command
                        command &= 0xf;         // The state of the aux function is ignored

                        if (command != 0xf)
                            layoutEmulationServices.SetLocomotiveSpeed(commandStationId, unit, (int)command);
                        else
                            layoutEmulationServices.ToggleLocomotiveDirection(commandStationId, unit);
                    }
                    else if (command == 33 || command == 34) {
                        await commStream.ReadAsync(byteBuffer, stopMe);
                        if (stopMe.IsCancellationRequested)
                            break;
                        int unit = byteBuffer[0];

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
                            SendFeedback(i);
                    }
                    else if (command == 192)
                        resetMode = true;
                    else if (193 <= command && command <= 224)
                        SendFeedback(command - 193);
                }
            }
            catch (Exception ex) {
                Trace.WriteLine("InterfaceThread terminated as a result of an exception: " + ex.GetType().Name + " message: " + ex.Message);
            }
        }

        private void SendFeedback(int feedbackIndex) {
            lock (feedbackDecoders) {
                commStream?.WriteByte((byte)((feedbackDecoders[feedbackIndex] >> 8) & 0xff));
                commStream?.WriteByte((byte)(feedbackDecoders[feedbackIndex] & 0xff));
            }
        }

        private void ReadFeedbacksCallback(object? state) {
            IList<ILocomotiveLocation> locomotiveLocations = layoutEmulationServices.GetLocomotiveLocations(commandStationId);

            if (resetMode) {
                for (int i = 0; i < 31; i++)
                    feedbackDecoders[i] = 0;
            }

            lock (feedbackDecoders) {
                foreach (ILocomotiveLocation location in locomotiveLocations) {
                    LayoutBlockDefinitionComponent blockInfo = location.Track.GetBlock(location.Front).BlockDefinintion;

                    if (blockInfo?.Info.IsOccupancyDetectionBlock == true) {
                        var connectionPoint = Ensure.NotNull<ControlConnectionPoint>(LayoutModel.ControlManager.ConnectionPoints[blockInfo]?[0]);

                        int decoderIndex = connectionPoint.Module.Address - 1;
                        int contactIndex = 15 - connectionPoint.Index;

                        feedbackDecoders[decoderIndex] |= (UInt16)(1 << contactIndex);
                    }
                }
            }
        }
    }
}

