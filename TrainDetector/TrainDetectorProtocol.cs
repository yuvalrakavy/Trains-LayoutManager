using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TrainDetector {
    [Flags]
    public enum PacketType : byte {
        PacketClassRequest = 0x00,
        PacketClassReply = 0x80,
        PacketClassNotification = PacketClassRequest,
        PacketClassAcknowledge = PacketClassReply,

        PleaseIdentify = 0,

        Identification = 1,
        IdentificationInfo = Identification | PacketClassRequest,
        IdentificationAcknowledge = Identification | PacketClassAcknowledge,

        ConfigSet = 2,
        ConfigSetRequest = ConfigSet | PacketClassRequest,
        ConfigSetReply = ConfigSet | PacketClassReply,

        ConfigGet = 3,
        ConfigGetRequest = ConfigGet | PacketClassRequest,
        ConfigGetReply = ConfigGet | PacketClassReply,

        GetState = 4,
        GetStateRequest = GetState | PacketClassRequest,
        GetStateReply = GetState | PacketClassReply,

        Subscribe = 5,
        SubscribeRequest = Subscribe | PacketClassRequest,
        SubscribeReply = Subscribe | PacketClassReply,

        Unsubscribe = 6,
        UnsubscribeRequest = Unsubscribe | PacketClassRequest,
        UnsubscribeReply = Unsubscribe | PacketClassReply,

        StateChanged = 7,
        StateChangedNotification = StateChanged | PacketClassNotification,
        StateChangedAcknowledge = StateChanged | PacketClassAcknowledge,

        ConfigChanged = 8,
        ConfigChangedNotification = ConfigChanged | PacketClassNotification,
        ConfigChangedAcknowledge = ConfigChanged | PacketClassAcknowledge,
    }

    [Flags]
    public enum SubscribeTo : UInt16 {
        StateChanges = 0x0001,
        ConfigChanges = 0x0002,
    }

    internal static class BinaryIOExtension {
        public static string ReadPacketString(this BinaryReader r) {
            var count = r.ReadUInt16();
            var bytes = r.ReadBytes(count);

            return Encoding.UTF8.GetString(bytes);
        }

        public static void WritePacketString(this BinaryWriter w, string s) {
            var bytes = Encoding.UTF8.GetBytes(s);
            w.Write((UInt16)bytes.Length);
            w.Write(bytes);
        }

        public static PacketHeader GetPacketHeader(this UdpReceiveResult udpResult) {
            using var s = new MemoryStream(udpResult.Buffer);
            var r = new BinaryReader(s);

            return new PacketHeader(r);
        }

        public static TrainDetectorPacket GetPacket(this UdpReceiveResult udpResult) {
            using var s = new MemoryStream(udpResult.Buffer);
            var r = new BinaryReader(s);
            var h = new PacketHeader(r);

            return h.Type switch
            {
                PacketType.PleaseIdentify => new PleaseIdentifyPacket(h, r),
                PacketType.IdentificationInfo => new IdentificationInfoPacket(h, r),
                PacketType.IdentificationAcknowledge => new IdentificationAcknowledgePacket(h),
                PacketType.ConfigSetRequest => new ConfigSetRequestPacket(h, r),
                PacketType.ConfigSetReply => new ConfigSetReplyPacket(h, r),
                PacketType.ConfigGetRequest => new ConfigGetRequestPacket(h, r),
                PacketType.ConfigGetReply => new ConfigGetReplyPacket(h, r),
                PacketType.GetStateRequest => new GetStateRequestPacket(h),
                PacketType.GetStateReply => new GetStateReplyPacket(h, r),
                PacketType.SubscribeRequest => new SubscribeRequestPacket(h, r),
                PacketType.SubscribeReply => new SubscribeReplyPacket(h),
                PacketType.UnsubscribeRequest => new UnsubscribeRequestPacket(h),
                PacketType.UnsubscribeReply => new UnsubscribeReplyPacket(h),
                PacketType.StateChangedNotification => new StateChangedNotificationPacket(h, r),
                PacketType.StateChangedAcknowledge => new StateChangedAcknowledgePacket(h),
                PacketType.ConfigChangedNotification => new ConfigChangedNotificationPacket(h, r),
                PacketType.ConfigChangedAcknowledge => new ConfigChangedAcknowledgePacket(h),
                _ => throw new ArgumentException($"Invalid train detector packet type: {h.Type}")
            };
        }

        public static Task<int> SendPacketAsync(this NetworkHandler netHandler, TrainDetectorPacket packet, IPEndPoint destination) {
            var m = new MemoryStream();
            using var w = new BinaryWriter(m);

            packet.Encode(w);
            return netHandler.UdpClient.SendAsync(m.GetBuffer(), (int)m.Length, destination);
        }
    }

    public abstract class TrainDetectorPacket {
        public abstract void Encode(BinaryWriter w);
    }

    public class PacketHeader : TrainDetectorPacket, IRequestReplyPacket {
        public PacketType Type { get; private set; }
        private byte TypeCompliment { get; set; }
        public int RequestNumber { get; private set; }

        public PacketHeader(PacketType type, UInt16 requestNumber) {
            this.Type = type;
            this.TypeCompliment = (byte)(~type);
            this.RequestNumber = requestNumber;
        }

        public PacketHeader(BinaryReader r) {
            this.Type = (PacketType)r.ReadByte();
            this.TypeCompliment = r.ReadByte();
            if(!IsValid)
                throw new ArgumentException("Train Detector: Invalid packet");
            this.RequestNumber = r.ReadUInt16();
        }

        public PacketHeader(PacketHeader h) {
            this.Type = h.Type;
            this.TypeCompliment = h.TypeCompliment;
            this.RequestNumber = h.RequestNumber;
        }

        public bool IsValid => (byte)~Type == TypeCompliment;

        public bool IsReply => (Type & PacketType.PacketClassReply) != 0;

        public override void Encode(BinaryWriter w) {
            w.Write((byte)Type);
            w.Write(TypeCompliment);
            w.Write((UInt16)RequestNumber);
        }
    }


    public class PleaseIdentifyPacket : PacketHeader {
        public byte MinProtocolVersion { get; private set; }
        public byte MaxProtocolVersion { get; private set; }

        public const int TrainDetectorPort = 3000;

        public PleaseIdentifyPacket(UInt16 requestNumber) : base(PacketType.PleaseIdentify, requestNumber) {
            MinProtocolVersion = 1;
            MaxProtocolVersion = 1;
        }

        public PleaseIdentifyPacket(PacketHeader h, BinaryReader r) : base(h) {
            MinProtocolVersion = r.ReadByte();
            MaxProtocolVersion = r.ReadByte();
        }

        public override void Encode(BinaryWriter w) {
            base.Encode(w);

            w.Write(MinProtocolVersion);
            w.Write(MaxProtocolVersion);
        }
    }

    public class IdentificationInfoPacket : PacketHeader {
        public byte ProtocolVersion { get; private set; }
        public UInt16 SensorCount { get; private set; }
        public string Name { get; private set; }

        public IdentificationInfoPacket(UInt16 requestNumber, UInt16 sensorCount, string name) : base(PacketType.IdentificationInfo, requestNumber) {
            this.ProtocolVersion = 1;
            this.SensorCount = sensorCount;
            this.Name = name;
        }

        public IdentificationInfoPacket(PacketHeader h, BinaryReader r) : base(h) {
            this.ProtocolVersion = r.ReadByte();
            this.SensorCount = r.ReadUInt16();
            this.Name = r.ReadPacketString();
        }

        public override void Encode(BinaryWriter w) {
            base.Encode(w);
            w.Write(ProtocolVersion);
            w.Write(SensorCount);
            w.WritePacketString(Name);
        }
    }

    public class IdentificationAcknowledgePacket : PacketHeader {
        public IdentificationAcknowledgePacket(UInt16 requestNumber) : base(PacketType.IdentificationAcknowledge, requestNumber) { }

        public IdentificationAcknowledgePacket(PacketHeader h) : base(h) { }

        public override void Encode(BinaryWriter w) {
            base.Encode(w);
        }
    }

    public class ConfigSetRequestPacket : PacketHeader {
        public string Name { get; private set; }
        public string Value { get; private set; }

        public ConfigSetRequestPacket(UInt16 requestNumber, string name, string value) : base(PacketType.ConfigSetRequest, requestNumber) {
            this.Name = name;
            this.Value = value;
        }

        public ConfigSetRequestPacket(PacketHeader h, BinaryReader r) : base(h) {
            Name = r.ReadPacketString();
            Value = r.ReadPacketString();
        }

        public override void Encode(BinaryWriter w) {
            base.Encode(w);
            w.WritePacketString(Name);
            w.WritePacketString(Value);
        }
    }

    public class ConfigSetReplyPacket : PacketHeader {
        public string Error { get; private set; }

        public ConfigSetReplyPacket(UInt16 requestNumber, string error = "") : base(PacketType.ConfigSetReply, requestNumber) {
            Error = error;
        }

        public ConfigSetReplyPacket(PacketHeader h, BinaryReader r) : base(h) {
            Error = r.ReadPacketString();
        }

        public override void Encode(BinaryWriter w) {
            base.Encode(w);
            w.WritePacketString(Error);
        }
    }

    public class ConfigGetRequestPacket : PacketHeader {
        public string Name { get; private set; }

        public ConfigGetRequestPacket(UInt16 requestNumber, string name) : base(PacketType.ConfigGetRequest, requestNumber) {
            this.Name = name;
        }

        public ConfigGetRequestPacket(PacketHeader h, BinaryReader r) : base(h) {
            this.Name = r.ReadPacketString();
        }

        public override void Encode(BinaryWriter w) {
            base.Encode(w);
            w.WritePacketString(Name);
        }
    }

    public class ConfigGetReplyPacket : PacketHeader {
        public bool IsError { get; private set; }
        public string Value { get; private set; }

        public ConfigGetReplyPacket(UInt16 requestNumber, string value, string error = null) : base(PacketType.ConfigGetReply, requestNumber) {
            if(error != null) {
                IsError = true;
                Value = error;
            }
            else {
                IsError = false;
                Value = value;
            }
        }

        public ConfigGetReplyPacket(PacketHeader h, BinaryReader r) : base(h) {
            var b = r.ReadByte();
            IsError = (b != 0);
            Value = r.ReadPacketString();
        }

        public override void Encode(BinaryWriter w) {
            base.Encode(w);
            w.Write(IsError ? (byte)1 : (byte)0);
            w.WritePacketString(Value);
        }
    }

    public class GetStateRequestPacket : PacketHeader {
        public GetStateRequestPacket(UInt16 requestNumber) : base(PacketType.GetStateRequest, requestNumber) { }
        public GetStateRequestPacket(PacketHeader h) : base(h) { }

        public override void Encode(BinaryWriter w) {
            base.Encode(w);
        }
    }

    public class GetStateReplyPacket : PacketHeader {
        public UInt32 Version { get; private set; }
        public List<bool> States { get; private set; }

        public GetStateReplyPacket(UInt16 requestNumber, UInt32 version, List<bool> states) : base(PacketType.GetStateReply, requestNumber) {
            Version = version;
            States = states;
        }

        public GetStateReplyPacket(PacketHeader h, BinaryReader r) : base(h) {
            Version = r.ReadUInt32();
            var count = r.ReadByte();

            States = new List<bool>(capacity: count);
            for (var i = 0; i < count; i++)
                States[i] = r.ReadByte() != 0;
        }

        public override void Encode(BinaryWriter w) {
            base.Encode(w);
            w.Write(Version);
            w.Write((byte)States.Count);

            foreach (var s in States)
                w.Write((byte)(s ? 1 : 0));
        }
    }

    public class SubscribeRequestPacket : PacketHeader {
        public SubscribeTo SubscribeTo { get; private set; }

        public SubscribeRequestPacket(UInt16 requestNumber, SubscribeTo subscribeTo = SubscribeTo.StateChanges) : base(PacketType.SubscribeRequest, requestNumber) {
            SubscribeTo = subscribeTo;
        }

        public SubscribeRequestPacket(PacketHeader h, BinaryReader r) : base(h) {
            SubscribeTo = (SubscribeTo)r.ReadUInt16();
        }

        public override void Encode(BinaryWriter w) {
            base.Encode(w);
            w.Write((UInt16)SubscribeTo);
        }
    }


    public class SubscribeReplyPacket : PacketHeader {
        public SubscribeReplyPacket(UInt16 requestNumber) : base(PacketType.SubscribeReply, requestNumber) { }
        public SubscribeReplyPacket(PacketHeader h) : base(h) { }
        public override void Encode(BinaryWriter w) {
            base.Encode(w);
        }
    }

    public class UnsubscribeRequestPacket : PacketHeader {
        public UnsubscribeRequestPacket(UInt16 requestNumber) : base(PacketType.UnsubscribeRequest, requestNumber) { }
        public UnsubscribeRequestPacket(PacketHeader h) : base(h) { }

        public override void Encode(BinaryWriter w) {
            base.Encode(w);
        }
    }

    public class UnsubscribeReplyPacket : PacketHeader {
        public UnsubscribeReplyPacket(UInt16 requestNumber) : base(PacketType.UnsubscribeReply, requestNumber) { }
        public UnsubscribeReplyPacket(PacketHeader h) : base(h) { }

        public override void Encode(BinaryWriter w) {
            base.Encode(w);
        }
    }

    public class StateChangedNotificationPacket : PacketHeader {
        public byte SensorNumber { get; private set; }
        public bool IsCovered { get; private set; }
        public UInt32 Version { get; private set; }
        public List<bool>States { get; private set; }

        public StateChangedNotificationPacket(UInt16 requestNumber, byte sensorNumber, bool isCovered, UInt32 version, List<bool> states) : base(PacketType.StateChangedNotification, requestNumber) {
            SensorNumber = sensorNumber;
            IsCovered = isCovered;
            Version = version;
            States = states;
        }

        public StateChangedNotificationPacket(PacketHeader h, BinaryReader r) : base(h) {
            SensorNumber = r.ReadByte();
            IsCovered = (r.ReadByte() != 0);
            Version = r.ReadUInt32();

            var count = r.ReadByte();

            States = new List<bool>(capacity: count);
            for (var i = 0; i < count; i++)
                States[i] = r.ReadByte() != 0;
        }

        public override void Encode(BinaryWriter w) {
            base.Encode(w);
            w.Write(SensorNumber);
            w.Write((byte)(IsCovered ? 1 : 0));
            w.Write(Version);
            w.Write((byte)States.Count);

            foreach (var s in States)
                w.Write((byte)(s ? 1 : 0));
        }
    }

    public class StateChangedAcknowledgePacket : PacketHeader {
        public StateChangedAcknowledgePacket(UInt16 requestNumber) : base(PacketType.StateChangedAcknowledge, requestNumber) { }
        public StateChangedAcknowledgePacket(PacketHeader h) : base(h) { }

        public override void Encode(BinaryWriter w) {
            base.Encode(w);
        }
    }

    public class ConfigChangedNotificationPacket : PacketHeader {
        public string Name { get; private set; }
        public string Value { get; private set; }

        public ConfigChangedNotificationPacket(UInt16 requestNumber, string name, string value) : base(PacketType.ConfigChangedNotification, requestNumber) {
            Name = name;
            Value = value;
        }

        public ConfigChangedNotificationPacket(PacketHeader h, BinaryReader r) : base(h) {
            Name = r.ReadPacketString();
            Value = r.ReadPacketString();
        }

        public override void Encode(BinaryWriter w) {
            base.Encode(w);
            w.WritePacketString(Name);
            w.WritePacketString(Value);
        }
    }

    public class ConfigChangedAcknowledgePacket : PacketHeader {
        public ConfigChangedAcknowledgePacket(UInt16 requestNumber) : base(PacketType.ConfigChangedNotification, requestNumber) { }
        public ConfigChangedAcknowledgePacket(PacketHeader h) : base(h) { }

        public override void Encode(BinaryWriter w) {
            base.Encode(w);
        }
    }
}
