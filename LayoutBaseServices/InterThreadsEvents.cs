using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

using LayoutManager;

#nullable enable
namespace LayoutBaseServices {
#pragma warning disable IDE0051, RCS1163

    [LayoutModule("Interthreads Event Relay", Enabled = true, UserControl = false)]
    internal class InterThreadsEvents : LayoutModuleBase, ILayoutInterThreadEventInvoker, IDisposable {
        private Control? controlInUIthread;
        private readonly Queue<LayoutEvent> eventQueue = new Queue<LayoutEvent>();
        private readonly ManualResetEvent eventInQueue = new ManualResetEvent(false);
        private readonly AutoResetEvent terminatedEvent = new AutoResetEvent(false);
        private Thread? relayThread = null;
        private readonly object queueLock = new object();
        private Form? uiThreadForm;
        private bool _disposed;
        private bool terminate;

        [LayoutEvent("initialize-event-interthread-relay")]
        private void InitializeEventInterthreadRelay(LayoutEvent e) {
            if (relayThread == null) {
                uiThreadForm = new Form {
                    ShowInTaskbar = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Opacity = 0
                };
                uiThreadForm.Show();

                Debug.Assert(uiThreadForm.IsHandleCreated);

                controlInUIthread = uiThreadForm;
                terminate = false;
                relayThread = new Thread(new ThreadStart(EventRelayThreadFunction)) {
                    Name = "Relay Events"
                };
                relayThread.Start();
            }
        }

        [LayoutEvent("terminate-event-interthread-relay")]
        private void TerminateEventInterthreadRelay(LayoutEvent e) {
            Debug.Assert(relayThread != null);

            QueueEvent(new LayoutEvent("terminate-event-relay-thread", this));

            if (!terminatedEvent.WaitOne(1000, false)) {
                terminate = true;
                eventInQueue.Set();
            }

            uiThreadForm?.Close();
            Dispose();
        }

        [LayoutEvent("get-inter-thread-event-invoker")]
        private void GetInvoker(LayoutEvent e) {
            e.Info = (ILayoutInterThreadEventInvoker)this;
        }

        private delegate object? LayoutEventCaller(LayoutEvent e);

        private void EventRelayThreadFunction() {
            // Wait until control is created, so events can be delivered to it
            while (controlInUIthread?.IsHandleCreated != true)
                Thread.Sleep(100);

            while (true) {
                LayoutEvent e;

                eventInQueue.WaitOne();
                if (terminate)
                    break;

                lock (queueLock) {
                    e = eventQueue.Dequeue();

                    if (eventQueue.Count == 0)
                        eventInQueue.Reset();
                }

                if (e.Sender != this || e.EventName != "terminate-event-relay-thread") {
                    try {
                        controlInUIthread.Invoke(new LayoutEventCaller(EventManager.Event), new object[] { e });
                    }
                    catch (Exception ex) {
                        Trace.WriteLine("Exception when invoking event in UI thread: " + ex.Message + " at " + ex.StackTrace);
                    }
                }
                else
                    break;
            }

            terminatedEvent.Set();
        }

        #region ILayoutInvoker Members

        public void QueueEvent(LayoutEvent e) {
            lock (queueLock) {
                eventQueue.Enqueue(e);
                eventInQueue.Set();
            }
        }

        #endregion

        private void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    controlInUIthread?.Dispose();
                    uiThreadForm?.Dispose();
                    eventInQueue.Dispose();
                    terminatedEvent.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose() {
            Dispose(true);
        }
    }
}
