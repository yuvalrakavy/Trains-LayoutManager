using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

using LayoutManager;

namespace LayoutBaseServices {
    [LayoutModule("Interthreads Event Relay", Enabled=true, UserControl=false)]
	class InterThreadsEvents : LayoutModuleBase, ILayoutInterThreadEventInvoker, IDisposable {
		Control controlInUIthread;
		Queue<LayoutEvent> eventQueue = new Queue<LayoutEvent>();
		ManualResetEvent eventInQueue = new ManualResetEvent(false);
		AutoResetEvent terminatedEvent = new AutoResetEvent(false);
		Thread relayThread = null;
		object queueLock = new object();
		Form uiThreadForm;
        bool _disposed;

		[LayoutEvent("initialize-event-interthread-relay")]
		private void initializeEventInterthreadRelay(LayoutEvent e) {
			if(relayThread == null) {
				uiThreadForm = new Form();

				uiThreadForm.ShowInTaskbar = false;
				uiThreadForm.FormBorderStyle = FormBorderStyle.None;
				uiThreadForm.Opacity = 0;
				uiThreadForm.Show();

				Debug.Assert(uiThreadForm.IsHandleCreated);

				controlInUIthread = uiThreadForm;
				relayThread = new Thread(new ThreadStart(eventRelayThreadFunction));
				relayThread.Name = "Relay Events";
				relayThread.Start();
			}
		}

		[LayoutEvent("terminate-event-interthread-relay")]
		private void terminateEventInterthreadRelay(LayoutEvent e) {
			QueueEvent(new LayoutEvent(this, "terminate-event-relay-thread"));

			if(!terminatedEvent.WaitOne(1000, false))
				relayThread.Abort();

			uiThreadForm.Close();
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
			while(!controlInUIthread.IsHandleCreated)
				Thread.Sleep(100);

			do {
				LayoutEvent e = null;

				eventInQueue.WaitOne();

				lock(queueLock) {
					e = eventQueue.Dequeue();

					if(eventQueue.Count == 0)
						eventInQueue.Reset();
				}

				if(e.Sender != this || e.EventName != "terminate-event-relay-thread") {
					try {
						controlInUIthread.Invoke(new LayoutEventCaller(EventManager.Event), new object[] { e });
					}
					catch(Exception ex) {
						Trace.WriteLine("Exception when invoking event in UI thread: " + ex.Message + " at " + ex.StackTrace);
					}
				}
				else
					break;
			} while(true);

			terminatedEvent.Set();
		}

		#region ILayoutInvoker Members

		public void QueueEvent(LayoutEvent e) {
			lock(queueLock) {
				eventQueue.Enqueue(e);
				eventInQueue.Set();
			}
		}

		#endregion

        void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    uiThreadForm.Dispose();
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
