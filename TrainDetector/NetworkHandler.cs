using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using LayoutManager;

#nullable enable
namespace TrainDetector {
    public interface IRequestReplyPacket {
        bool IsValid { get; }
        bool IsReply { get; }
        int RequestNumber { get; }

        void Encode(BinaryWriter w);
    }

    [LayoutModule("Train Detector Network handler")]
    internal class TrainDetectorNetworkHandler : LayoutModuleBase {
        [LayoutEvent("set-network-request-exception")]
        private void SetNetworkRequestException(LayoutEvent e) {
            (var taskSource, var ex) = Ensure.NotNull<TaskCompletionSource<UdpReceiveResult>, Exception>(e);

            taskSource.SetException(ex);
        }

        [LayoutEvent("set-network-request-reply")]
        private void SetNetworkRequestReply(LayoutEvent e) {
            var taskSource = Ensure.NotNull<TaskCompletionSource<UdpReceiveResult>>(e.Sender);
            var reply = Ensure.ValueNotNull<UdpReceiveResult>(e.Info);

            taskSource.SetResult(reply);
        }
    }

     public class NetworkHandler : IDisposable {
        public UdpClient UdpClient { get; private set; }
        readonly Dictionary<int, TaskCompletionSource<UdpReceiveResult>> pendingRequests = new Dictionary<int, TaskCompletionSource<UdpReceiveResult>>();
        readonly TaskCompletionSource<bool> networkHandlerDoneSource;
        readonly Task<bool> networkHandlerDoneTask;
        UInt16 nextRequestNumber = 1;
        Func<UdpReceiveResult, IRequestReplyPacket> ExtractPacket { get; set; }
        bool disposed = false;

        public bool IsStarted { get; private set; } = false;

        public ILayoutInterThreadEventInvoker InterThreadEventInvoker => EventManager.Instance.InterThreadEventInvoker;

        public NetworkHandler(Func<UdpReceiveResult, IRequestReplyPacket> extractPacket) {
            networkHandlerDoneSource = new TaskCompletionSource<bool>();
            networkHandlerDoneTask = networkHandlerDoneSource.Task;
            this.ExtractPacket = extractPacket;
            this.UdpClient = new UdpClient(0);
        }

        public event EventHandler<UdpReceiveResult>? OnReceivingPacket;

        public void Start() {

            IsStarted = true;
            Task.Run(async () => {
                while (true) {
                    var udpReceiveTask = UdpClient.ReceiveAsync();

                    if (await Task.WhenAny(udpReceiveTask, networkHandlerDoneTask) == networkHandlerDoneTask)
                        break;

                    var udpReceiveResult = await udpReceiveTask;
                    var packet = ExtractPacket(udpReceiveResult);

                    if (packet.IsValid) {
                        if (packet.IsReply) {
                            if (pendingRequests.TryGetValue(packet.RequestNumber, out TaskCompletionSource<UdpReceiveResult>? pending)) {
                                pending.SetResult(udpReceiveResult);
                            }
                            else
                                Trace.WriteLine($"TrainDetector: got unexpected reply for request# {packet.RequestNumber}");

                        }
                        else
                            OnReceivingPacket?.Invoke(this, udpReceiveResult);
                    }
                    else
                        Trace.WriteLine("TrainDetector: Received invalid UDP packet");
                }

                UdpClient.Close();
                OnReceivingPacket = null;
                IsStarted = false;
                Trace.WriteLine("Network handler terminated");
            });
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposed) {
                disposed = true;

                if (disposing)
                    networkHandlerDoneSource.SetResult(true);
            }
        }

        public Task<UdpReceiveResult> Request(Action<UInt16> sendRequestPacket, int retries = 3, int timeoutMs = 300) {
            Debug.Assert(IsStarted);

            var resultSource = new TaskCompletionSource<UdpReceiveResult>();
            var pendingRequestNumber = (UInt16)(this.nextRequestNumber++);

            Task.Run(async () => {
                try {
                    var gotReplySource = new TaskCompletionSource<UdpReceiveResult>();
                    var gotReplyTask = gotReplySource.Task;

                    lock (this.pendingRequests) {
                        pendingRequests.Add(pendingRequestNumber, gotReplySource);
                    }

                    for (var retry = 0; retry < retries && !gotReplyTask.IsCompleted; retry++) {
                        sendRequestPacket(pendingRequestNumber);
                        await Task.WhenAny(gotReplyTask, Task.Delay(timeoutMs)).ConfigureAwait(false);
                    }

                    if (!gotReplyTask.IsCompleted)
                        InterThreadEventInvoker.QueueEvent(new LayoutEvent("set-network-request-exception", resultSource, new TimeoutException($"No reply for request# {pendingRequestNumber}")));
                    else
                        InterThreadEventInvoker.QueueEvent(new LayoutEvent("set-network-request-reply", resultSource, gotReplyTask.Result));
                }
                finally {
                    lock (this.pendingRequests) {
                        pendingRequests.Remove(pendingRequestNumber);
                    }
                }
            });

            return resultSource.Task;
        }
    }
}
