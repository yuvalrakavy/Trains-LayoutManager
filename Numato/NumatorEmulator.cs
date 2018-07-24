using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;

using LayoutManager;
using LayoutManager.Model;

namespace NumatoController {
    public class NumatorEmulator : ILayoutCommandStationEmulator {
        Guid numatoId;
        string pipeName;

        FileStream commStream;
        ILayoutEmulatorServices layoutEmulationServices;
        Thread interfaceThread = null;
        ILayoutInterThreadEventInvoker interThreadEventInvoker;

        enum NumatoState {
            GetUser, GetPassword, GetCommand
        }

        NumatoState state;

        static LayoutTraceSwitch traceNumatoEmulator = new LayoutTraceSwitch("NumatoEmulator", "Trace Numato Relay Board emulation");

        public NumatorEmulator(IModelComponentIsBusProvider numatoComponent, string pipeName) {
            this.numatoId = numatoComponent.Id;
            this.pipeName = pipeName;
            this.interThreadEventInvoker = (ILayoutInterThreadEventInvoker)EventManager.Event(new LayoutEvent(this, "get-inter-thread-event-invoker"));

            layoutEmulationServices = (ILayoutEmulatorServices)EventManager.Event(new LayoutEvent(this, "get-layout-emulation-services"));
            EventManager.Event(new LayoutEvent(null, "initialize-layout-emulation"));

            interfaceThread = new Thread(new ThreadStart(InterfaceThreadFunction));
            interfaceThread.Name = "Command station emulation for " + numatoComponent.NameProvider.Name;
            interfaceThread.Start();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void InterfaceThreadFunction() {
            commStream = (FileStream)EventManager.Event(new LayoutEvent(pipeName, "wait-named-pipe-request", null, true));

            state = NumatoState.GetUser;


            try {
                while(true) {
                    byte[] prompt = null;

                    switch(state) {
                        case NumatoState.GetUser: prompt = Encoding.UTF8.GetBytes("Numato Lab 32 Channel Ethernet Relay Module\r\nUser Name: "); break;
                        case NumatoState.GetPassword: prompt = Encoding.UTF8.GetBytes("Password: ").Concat(new byte[] { 0xff, 0xfd, 0x2d }).ToArray(); break;
                        case NumatoState.GetCommand: prompt =Encoding.UTF8.GetBytes(">"); break;
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

                    switch(state) {
                        case NumatoState.GetUser: state = NumatoState.GetPassword; break;
                        case NumatoState.GetPassword: state = NumatoState.GetCommand; break;
                    }

                    if(reply != null)
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
