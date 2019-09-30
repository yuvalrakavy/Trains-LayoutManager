using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Net;

using System.IO;

#pragma warning disable RCS1090
namespace DigiFinder {
    public class DigiDevice {
        public byte[] MacAddress { get; internal set; }
        public IPAddress IpAddress { get; internal set; }
        public IPAddress GatewayAddress { get; internal set; }
        public IPAddress NetMask { get; internal set; }
        public string DeviceName { get; internal set; }
        public bool DhcpEnabled { get; internal set; }
        public string FirmwareDescription { get; internal set; }
        public int RealPortNumber { get; internal set; }
        public int EncryptedRealPortNumber { get; internal set; }
        public int PortCount { get; internal set; }

        public override string ToString() => IpAddress.ToString() + " [" + RealPortNumber + "]: " + DeviceName + " (" + FirmwareDescription + ") " + PortCount + " serial ports";
    }

    public class DigiFinder {
        private readonly IPAddress DigiMulticastGroup = IPAddress.Parse("224.0.5.128");
        private readonly int DigiMulticastPort = 2362;
        private readonly byte[] replyBuffer = new byte[512];

        public async Task DiscoverDigiDevices(ICollection<DigiDevice> devices, int timeout = 4000) {
            using var discoverySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var endPoint = new IPEndPoint(IPAddress.Any, 0);

            discoverySocket.Bind(endPoint);

            // Listen to responses
            EndPoint remoteEndPoint = new IPEndPoint(0, 0);
            void onReply(IAsyncResult ia) {
                try {
                    var count = discoverySocket.EndReceiveFrom(ia, ref remoteEndPoint);

                    // Parse reply and build a new DigiDevice entry
                    DigiReplyPacket replyPacket = new DigiReplyPacket(replyBuffer);
                    byte[] fieldData;
                    DigiDevice device = new DigiDevice();

                    while ((fieldData = replyPacket.GetField(out DigiReplyFieldType fieldType)) != null) {
                        System.Diagnostics.Trace.WriteLine("Got field: " + fieldType.ToString());

                        switch (fieldType) {
                            case DigiReplyFieldType.DeviceName: device.DeviceName = DigiReplyPacket.GetString(fieldData); break;
                            case DigiReplyFieldType.DhcpEnabled: device.DhcpEnabled = fieldData[0] != 0; break;
                            case DigiReplyFieldType.EncryptedRealPortNumber: device.EncryptedRealPortNumber = DigiReplyPacket.GetInt32(fieldData); break;
                            case DigiReplyFieldType.FirmwareDescription: device.FirmwareDescription = DigiReplyPacket.GetString(fieldData); break;
                            case DigiReplyFieldType.IpAddress: device.IpAddress = DigiReplyPacket.GetIpAddress(fieldData); break;
                            case DigiReplyFieldType.IpGateway: device.GatewayAddress = DigiReplyPacket.GetIpAddress(fieldData); break;
                            case DigiReplyFieldType.MacAddress: device.MacAddress = fieldData; break;
                            case DigiReplyFieldType.NetMask: device.NetMask = DigiReplyPacket.GetIpAddress(fieldData); break;
                            case DigiReplyFieldType.RealPortNumber: device.RealPortNumber = DigiReplyPacket.GetInt32(fieldData); break;
                            case DigiReplyFieldType.SerialPortCount: device.PortCount = fieldData[0]; break;
                        }
                    }

                    devices.Add(device);

                    // Start to get the next response
                    discoverySocket.BeginReceiveFrom(replyBuffer, 0, replyBuffer.Length, SocketFlags.None, ref remoteEndPoint, onReply, null);
                }
                catch (ObjectDisposedException) {
                }
            }

            discoverySocket.BeginReceiveFrom(replyBuffer, 0, replyBuffer.Length, SocketFlags.None, ref remoteEndPoint, onReply, null);

            // Send the request
            var digiEndpoint = new IPEndPoint(DigiMulticastGroup, DigiMulticastPort);
            byte[] buffer = new DiscoveryDigiPacket().GetBuffer();

            discoverySocket.SendTo(buffer, digiEndpoint);

            await Task.Delay(timeout);
        }
    }

    internal class DigiRequestPacket {
        private readonly MemoryStream packetStream;

        public DigiRequestPacket(int packetType) {
            this.packetStream = new MemoryStream();

            Add("DIGI");
            Add((Int16)packetType);
            Add((Int16)0);      // Will be filled by payload length
        }

        public byte[] GetBuffer() {
            packetStream.Flush();

            Int16 payloadLength = (Int16)(packetStream.Length - 8);            // payload length is the stream size - the header length

            packetStream.Seek(6, SeekOrigin.Begin);
            Add(payloadLength);
            packetStream.Close();

            return packetStream.ToArray();
        }

        protected void Add(string s) {
            foreach (char c in s)
                packetStream.WriteByte((byte)c);
        }

        protected void Add(byte b) {
            packetStream.WriteByte(b);
        }

        protected void Add(byte[] buffer) {
            foreach (byte b in buffer)
                Add(b);
        }

        protected void Add(Int16 i) {
            Add((byte)(i >> 8));
            Add((byte)(i & 0xff));
        }
    }

    internal class DiscoveryDigiPacket : DigiRequestPacket {
        public DiscoveryDigiPacket(byte[] specificTargetMac = null)
            : base(1) {
            byte[] mac = specificTargetMac ?? new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };

            if (mac.Length != 6)
                throw new ArgumentException("Invalid target MAC address");

            Add(mac);
        }
    }

    internal enum DigiReplyFieldType : byte {
        MacAddress = 0x01,
        IpAddress = 0x02,
        NetMask = 0x03,
        NetworkName = 0x04,
        Unknown7 = 0x07,
        FirmwareDescription = 0x08,
        ResultMessage = 0x09,
        ResultFlag = 0x0a,
        IpGateway = 0x0b,
        ConfigError = 0x0c,
        DeviceName = 0x0d,
        RealPortNumber = 0x0e,
        UnknownIpAddress = 0x0f,
        DhcpEnabled = 0x10,
        ErrorCode = 0x011,
        SerialPortCount = 0x12,
        EncryptedRealPortNumber = 0x13
    }

    internal class DigiReplyPacket {
        private readonly MemoryStream packetStream;

        public int PacketType { get; }
        public int PayloadLength { get; }

        public DigiReplyPacket(byte[] buffer) {
            packetStream = new MemoryStream(buffer);

            byte[] magic = new byte[4];

            packetStream.Read(magic, 0, 4);
            if (magic[0] != 'D' || magic[1] != 'I' || magic[2] != 'G' || magic[3] != 'I')
                throw new ArgumentException("Buffer is not valid DIGI reply packet");

            this.PacketType = GetShort();
            this.PayloadLength = GetShort();
        }

        private Int16 GetShort() {
            int h = packetStream.ReadByte();
            int l = packetStream.ReadByte();

            return (Int16)((h << 8) + l);
        }

        public byte[] GetField(out DigiReplyFieldType fieldType) {
            if (packetStream.Position >= PayloadLength + 6) {
                fieldType = 0;
                return null;            // No more fields
            }

            fieldType = (DigiReplyFieldType)packetStream.ReadByte();

            int fieldLength = packetStream.ReadByte();
            byte[] fieldData = new byte[fieldLength];

            packetStream.Read(fieldData, 0, fieldData.Length);
            return fieldData;
        }

        public static IPAddress GetIpAddress(byte[] fieldData) {
            if (fieldData.Length != 4)
                throw new ArgumentException("Field data must be 4 bytes");

            return new IPAddress(fieldData);
        }

        public static int GetInt32(byte[] fieldData) {
            if (fieldData.Length != 4)
                throw new ArgumentException("Field data must be 4 bytes");

            return ((int)fieldData[0] << 24) | ((int)fieldData[1] << 16) | ((int)fieldData[2] << 8) | (int)fieldData[3];
        }

        public static string GetString(byte[] fieldData) => System.Text.ASCIIEncoding.Default.GetString(fieldData);
    }
}
