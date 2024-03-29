﻿using LayoutManager;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable IDE0051, IDE0060, IDE0052
namespace NumatoController {
    public class NumatorEmulator : ILayoutCommandStationEmulator {
        private readonly string pipeName;

        private NamedPipeServerStream? commStream;
        private readonly ILayoutEmulatorServices layoutEmulationServices;
        private readonly CancellationTokenSource stopInterfaceThrad;
        private readonly Task interfaceTask;
        private readonly ILayoutInterThreadEventInvoker interThreadEventInvoker;

        private enum NumatoState {
            GetUser, GetPassword, GetCommand
        }

        private NumatoState state;

        private static readonly LayoutTraceSwitch traceNumatoEmulator = new("NumatoEmulator", "Trace Numato Relay Board emulation");

        public NumatorEmulator(IModelComponentIsBusProvider numatoComponent, string pipeName) {
            this.pipeName = pipeName;
            this.interThreadEventInvoker = Dispatch.Call.GetInterthreadInvoker();

            layoutEmulationServices = Dispatch.Call.GetLayoutEmulationServices();
            Dispatch.Call.InitializeLayoutEmulation(false, 0);

            stopInterfaceThrad = new CancellationTokenSource();
            interfaceTask = InterfaceThreadFunction(stopInterfaceThrad.Token);
        }

        private async Task InterfaceThreadFunction(CancellationToken stopMe) {
            commStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            state = NumatoState.GetUser;

            async Task<byte> ReadByte() {
                if (commStream == null)
                    throw new NullReferenceException(nameof(commStream));

                var buffer = new byte[1];
                await commStream.ReadAsync(buffer, stopMe);
                stopMe.ThrowIfCancellationRequested();
                return buffer[0];
            }

            try {
                await commStream.WaitForConnectionAsync(stopMe);

                while (!stopMe.IsCancellationRequested) {
                    byte[]? prompt = null;

                    prompt =state switch {
                        NumatoState.GetUser => Encoding.UTF8.GetBytes("Numato Lab 32 Channel Ethernet Relay Module\r\nUser Name: "),
                        NumatoState.GetPassword => Encoding.UTF8.GetBytes("Password: ").Concat(new byte[] { 0xff, 0xfd, 0x2d }).ToArray(),
                        NumatoState.GetCommand => Encoding.UTF8.GetBytes(">"),
                        _ => throw new InvalidOperationException("Invalid numatoState"),
                    };

                    Trace.WriteLineIf(traceNumatoEmulator.TraceVerbose, $"Numato emulator prompt with {Encoding.UTF8.GetString(prompt)}");
                    await commStream.WriteAsync(prompt, stopMe);
                    stopMe.ThrowIfCancellationRequested();

                    var inputBytes = new List<byte>(80);
                    int inputByte;

                    for (var inputCount = 0; inputCount < 80 && (inputByte = await ReadByte()) != -1 && inputByte != '\n'; inputCount++)
                        inputBytes.Add((byte)inputByte);

                    string input = Encoding.UTF8.GetString(inputBytes.ToArray()).TrimEnd('\r', '\n');

                    Trace.WriteLineIf(traceNumatoEmulator.TraceVerbose, $"Numato emulator: got {input}");

                    byte[]? reply = null;

                    switch (state) {
                        case NumatoState.GetUser: state = NumatoState.GetPassword; break;
                        case NumatoState.GetPassword: state = NumatoState.GetCommand; break;
                    }

                    if (reply != null)
                        await commStream.WriteAsync(reply, stopMe);
                }
            }
            catch (TaskCanceledException) {

            }
            catch (Exception ex) {
                Trace.WriteLine("Numato Emulator interfaceThread terminated as a result of an exception: " + ex.GetType().Name + " message: " + ex.Message);
            }
        }

        public async void Dispose() {
            stopInterfaceThrad.Cancel();
            await interfaceTask;

            commStream?.Close();
            commStream = null;
            GC.SuppressFinalize(this);
        }
    }
}
