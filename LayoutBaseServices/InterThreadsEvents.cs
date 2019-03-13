using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

using LayoutManager;

#nullable enable
namespace LayoutBaseServices {
#pragma warning disable IDE0051

    [LayoutModule("Interthreads Event Relay", Enabled = true, UserControl = false)]
    class InterThreadsEvents : LayoutModuleBase, ILayoutInterThreadEventInvoker, IDisposable {
        Control? controlInUIthread;
        readonly Queue<LayoutEvent> eventQueue = new Queue<LayoutEvent>();
        readonly ManualResetEvent eventInQueue = new ManualResetEvent(false);
        readonly AutoResetEvent terminatedEvent = new AutoResetEvent(false);
        Thread? relayThread = null;
        readonly object queueLock = new object();
        Form? uiThreadForm;
        bool _disposed;

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
                relayThread = new Thread(new ThreadStart(eventRelayThreadFunction)) {
                    Name = "Relay Events"
                };
                relayThread.Start();
            }
        }

        [LayoutEvent("terminate-event-interthread-relay")]
#pragma warning disable IDE0060 // Remove unused parameter
        private void terminateEventInterthreadRelay(LayoutEvent e) {
#pragma warning restore IDE0060 // Remove unused parameter
            Debug.Assert(relayThread != null);

            QueueEvent(new LayoutEvent("terminate-event-relay-thread", this));

            if (!terminatedEvent.WaitOne(1000, false))
                relayThread?.Abort();

            uiThreadForm?.Close();
            Dispose();
        }

        [LayoutEvent("get-inter-thread-event-invoker")]
        void GetInvoker(LayoutEvent e) {
            e.Info = (ILayoutInterThreadEventInvoker)this;
        }

        private delegate object LayoutEventCaller(LayoutEvent e);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void eventRelayThreadFunction() {
            // Wait until control is created, so events can be delivered to it
            while (controlInUIthread == null || !controlInUIthread.IsHandleCreated)
                Thread.Sleep(100);

            do {
                LayoutEvent e;

                eventInQueue.WaitOne();

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
            } while (true);

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

        void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
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
