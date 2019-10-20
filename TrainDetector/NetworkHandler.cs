using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace TrainDetector {
    public interface IRequestReplyPacket {
        bool IsValid { get; }
        bool IsReply { get; }
        int RequestNumber { get; }

        void Encode(BinaryWriter w);
    }

    public class NetworkHandler : IDisposable {
        public UdpClient UdpClient { get; private set; }
        readonly Dictionary<int, TaskCompletionSource<UdpReceiveResult>> pendingRequests = new Dictionary<int, TaskCompletionSource<UdpReceiveResult>>();
        readonly TaskCompletionSource<bool> networkHandlerDoneSource;
        readonly Task<bool> networkHandlerDoneTask;
        UInt16 nextRequestNumber = 1;
        Func<UdpReceiveResult, IRequestReplyPacket> ExtractPacket { get; set; }
        bool disposed = false;

        public NetworkHandler(Func<UdpReceiveResult, IRequestReplyPacket> extractPacket) {
            networkHandlerDoneSource = new TaskCompletionSource<bool>();
            networkHandlerDoneTask = networkHandlerDoneSource.Task;
            this.ExtractPacket = extractPacket;
            this.UdpClient = new UdpClient(0);
        }

        public event EventHandler<UdpReceiveResult>? OnReceivingPacket;

        public async void Start() {

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
            Trace.WriteLine("Network handler terminated");
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

        public async Task<UdpReceiveResult> Request(Action<UInt16> sendRequestPacket, int retries = 3, int timeoutMs = 300) {
            var doneSource = new TaskCompletionSource<UdpReceiveResult>();
            var doneTask = doneSource.Task;
            var pendingRequestNumber = (UInt16)(this.nextRequestNumber++);

            lock (this.pendingRequests) {
                pendingRequests.Add(pendingRequestNumber, doneSource);
            }

            for (var retry = 0; retry < retries && !doneTask.IsCompleted; retry++) {
                sendRequestPacket(pendingRequestNumber);
                await Task.WhenAny(doneTask, Task.Delay(timeoutMs));
            }

            lock (this.pendingRequests) {
                pendingRequests.Remove(pendingRequestNumber);
            }

            if (doneTask.IsCompleted)
                return doneTask.Result;
            else
                throw new TimeoutException($"Request# {pendingRequestNumber} timedout");
        }

    }
}
