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
    public class TrainDetectorController {
        public IPEndPoint Address { get; private set; }
        public string Name { get; set; }

        public int SensorsCount { get; private set; }

        public int Version { get; private set; }

        public TrainDetectorController(IPEndPoint address, string name, int sensorsCount, int version) {
            this.Address = address;
            this.Name = name;
            this.SensorsCount = sensorsCount;
            this.Version = version;
        }
    }

    public static class TrainDetectorOps {
        public const int TrainDetectorPort = 3000;

        public static async Task<List<TrainDetectorController>> FindControllers(NetworkHandler netHandler, int detectionTimeMs = 500) {
            var controllers = new List<TrainDetectorController>();
            void gotPacket(object? sender, UdpReceiveResult rawPacket) {
                var packet = rawPacket.GetPacket();

                if (packet is IdentificationInfoPacket identifyInfo) {
                    controllers.Add(new TrainDetectorController(rawPacket.RemoteEndPoint, identifyInfo.Name, identifyInfo.SensorCount, identifyInfo.ProtocolVersion));
                    netHandler.UdpClient.SendPacketAsync(new IdentificationAcknowledgePacket((UInt16)identifyInfo.RequestNumber), rawPacket.RemoteEndPoint);
                }
                else
                    Trace.WriteLine($"Unexpected reply to PlaseIdentify: {packet.GetType().Name}");
            }

            netHandler.OnReceivingPacket += gotPacket;
            netHandler.UdpClient.EnableBroadcast = true;
            var broadcastAddress = new IPEndPoint(IPAddress.Broadcast, TrainDetectorPort);
            await netHandler.UdpClient.SendPacketAsync(new PleaseIdentifyPacket(0), broadcastAddress);
            await Task.Delay(detectionTimeMs);

            netHandler.OnReceivingPacket -= gotPacket;

            return controllers;
        }
    }
}