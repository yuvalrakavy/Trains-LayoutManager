using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;

using LayoutManager;
using LayoutManager.Model;

#pragma warning disable IDE0051, IDE0060, IDE0052
namespace NumatoController {
    public class NumatorEmulator : ILayoutCommandStationEmulator {
        readonly string pipeName;

        FileStream commStream;
        readonly ILayoutEmulatorServices layoutEmulationServices;
        Thread interfaceThread = null;
        readonly ILayoutInterThreadEventInvoker interThreadEventInvoker;

        enum NumatoState {
            GetUser, GetPassword, GetCommand
        }

        NumatoState state;

        static readonly LayoutTraceSwitch traceNumatoEmulator = new LayoutTraceSwitch("NumatoEmulator", "Trace Numato Relay Board emulation");

        public NumatorEmulator(IModelComponentIsBusProvider numatoComponent, string pipeName) {
            this.pipeName = pipeName;
            this.interThreadEventInvoker = (ILayoutInterThreadEventInvoker)EventManager.Event(new LayoutEvent("get-inter-thread-event-invoker", this));

            layoutEmulationServices = (ILayoutEmulatorServices)EventManager.Event(new LayoutEvent("get-layout-emulation-services", this));
            EventManager.Event(new LayoutEvent("initialize-layout-emulation"));

            interfaceThread = new Thread(new ThreadStart(InterfaceThreadFunction)) {
                Name = "Command station emulation for " + numatoComponent.NameProvider.Name
            };
            interfaceThread.Start();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void InterfaceThreadFunction() {
            commStream = (FileStream)EventManager.Event(new LayoutEvent("wait-named-pipe-request", pipeName, true));

            state = NumatoState.GetUser;


            try {
                while (true) {
                    byte[] prompt = null;

                    switch (state) {
                        case NumatoState.GetUser: prompt = Encoding.UTF8.GetBytes("Numato Lab 32 Channel Ethernet Relay Module\r\nUser Name: "); break;
                        case NumatoState.GetPassword: prompt = Encoding.UTF8.GetBytes("Password: ").Concat(new byte[] { 0xff, 0xfd, 0x2d }).ToArray(); break;
                        case NumatoState.GetCommand: prompt = Encoding.UTF8.GetBytes(">"); break;
                    }

                    Trace.WriteLineIf(traceNumatoEmulator.TraceVerbose, $"Numato emulator prompt with {Encoding.UTF8.GetString(prompt)}");
                    commStream.Write(prompt, 0, prompt.Length);

                    var inputBytes = new List<byte>(80);
                    int inputByte;

                    for (var inputCount = 0; inputCount < 80 && (inputByte = commStream.ReadByte()) != -1 && inputByte != '\n'; inputCount++)
                        inputBytes.Add((byte)inputByte);

                    string input = Encoding.UTF8.GetString(inputBytes.ToArray()).TrimEnd('\r', '\n');

                    Trace.WriteLineIf(traceNumatoEmulator.TraceVerbose, $"Numato emulator: got {input}");

                    byte[] reply = null;

                    switch (state) {
                        case NumatoState.GetUser: state = NumatoState.GetPassword; break;
                        case NumatoState.GetPassword: state = NumatoState.GetCommand; break;
                    }

                    if (reply != null)
                        commStream.Write(reply, 0, reply.Length);
                }
            }
            catch (Exception ex) {
                Trace.WriteLine("Numato Emulator interfaceThread terminated as a result of an exception: " + ex.GetType().Name + " message: " + ex.Message);
            }
        }

        public void Dispose() {
            if (interfaceThread != null) {
                if (interfaceThread.IsAlive)
                    interfaceThread.Abort();
                interfaceThread = null;
            }

            commStream.Close();
            commStream = null;
            GC.SuppressFinalize(this);
        }
    }
}
