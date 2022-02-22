using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace LayoutManager {
    public static class InterthreadDispatchSources {
        [DispatchSource]
        public static void InitializeEventInterthreadRelay(this Dispatcher d) {
            d[nameof(InitializeEventInterthreadRelay)].CallVoid();
        }

        [DispatchSource]
        public static void TerminateEventInterthreadRelay(this Dispatcher d) {
            d[nameof(TerminateEventInterthreadRelay)].CallVoid();
        }

        static DispatchSource? GetInterthreadInvokerDispatchSource;

        [DispatchSource]
        public static ILayoutInterThreadEventInvoker GetInterthreadInvoker(this Dispatcher d) =>
            (GetInterthreadInvokerDispatchSource ??= d[nameof(GetInterthreadInvoker)]).Call<ILayoutInterThreadEventInvoker>();
    }

#nullable enable
    namespace LayoutBaseServices {
#pragma warning disable IDE0051, RCS1163

        abstract class QueueEntry {

        }

        class EventQueueEntry : QueueEntry {
            public LayoutEvent LayoutEvent { get; private set; }

            public EventQueueEntry(LayoutEvent e) {
                this.LayoutEvent = e;
            }
        }

        class ActionQueueEntry : QueueEntry {
            public Action Action { get; private set; }

            public ActionQueueEntry(Action a) {
                this.Action = a;
            }
        }

        class TerminateThreadQueueEntry : QueueEntry {

        }

        [LayoutModule("Inter threads Event Relay", Enabled = true, UserControl = false)]
        internal class InterThreadsEvents : LayoutModuleBase, ILayoutInterThreadEventInvoker, IDisposable {
            private Control? controlInUIthread;
            private readonly Queue<QueueEntry> eventQueue = new();
            private readonly ManualResetEvent eventInQueue = new(false);
            private readonly AutoResetEvent terminatedEvent = new(false);
            private Thread? relayThread = null;
            private readonly object queueLock = new();
            private Form? uiThreadForm;
            private bool _disposed;

            [DispatchTarget]
            private void InitializeEventInterthreadRelay() {
                if (relayThread == null) {
                    uiThreadForm = new Form {
                        ShowInTaskbar = false,
                        FormBorderStyle = FormBorderStyle.None,
                        Opacity = 0
                    };
                    uiThreadForm.Show();

                    Debug.Assert(uiThreadForm.IsHandleCreated);

                    controlInUIthread = uiThreadForm;
                    relayThread = new Thread(new ThreadStart(EventRelayThreadFunction)) {
                        Name = "Relay Events"
                    };
                    relayThread.Start();
                }
            }

            [DispatchTarget]
            private void TerminateEventInterthreadRelay() {
                Debug.Assert(relayThread != null);

                QueueEntry(new TerminateThreadQueueEntry());

                if (!terminatedEvent.WaitOne(1000, false)) {
                    eventInQueue.Set();
                }

                uiThreadForm?.Close();
                Dispose();
            }

            [DispatchTarget]
            ILayoutInterThreadEventInvoker GetInterthreadInvoker() => this;

            private delegate object? LayoutEventCaller(LayoutEvent e);

            private void EventRelayThreadFunction() {
                // Wait until control is created, so events can be delivered to it
                while (controlInUIthread?.IsHandleCreated != true)
                    Thread.Sleep(100);

                bool terminate = false;

                while (!terminate) {
                    QueueEntry e;

                    eventInQueue.WaitOne();
                    if (terminate)
                        break;

                    lock (queueLock) {
                        e = eventQueue.Dequeue();

                        if (eventQueue.Count == 0)
                            eventInQueue.Reset();
                    }

                    switch (e) {
                        case EventQueueEntry { LayoutEvent: LayoutEvent theEvent }:
                            try {
                                controlInUIthread.Invoke(new LayoutEventCaller(EventManager.Event), new object[] { theEvent });
                            }
                            catch (Exception ex) {
                                Trace.WriteLine("Exception when invoking event in UI thread: " + ex.Message + " at " + ex.StackTrace);
                            }
                            break;

                        case ActionQueueEntry { Action: Action a }:
                            try {
                                controlInUIthread.Invoke(a);
                            }
                            catch (Exception ex) {
                                Trace.WriteLine("Exception when invoking action in UI thread: " + ex.Message + " at " + ex.StackTrace);
                            }
                            break;

                        case TerminateThreadQueueEntry:
                            terminate = true;
                            break;
                    }
                }

                terminatedEvent.Set();
            }

            private void QueueEntry(QueueEntry entry) {
                lock (queueLock) {
                    eventQueue.Enqueue(entry);
                    eventInQueue.Set();

                }
            }

            #region ILayoutInvoker Members

            public void QueueEvent(LayoutEvent e) {
                QueueEntry(new EventQueueEntry(e));
            }

            public void Queue(Action a) {
                QueueEntry(new ActionQueueEntry(a));
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
}
