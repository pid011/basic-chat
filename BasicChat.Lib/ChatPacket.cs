using System;
using System.IO;
using System.Net.Sockets;

namespace BasicChat.Lib
{
    public enum ChatStatus
    {
        Welcome,
        Leave,
        Chat,
        TypingStart,
        TypingEnd,
    }

    [Serializable]
    public class ChatPacket
    {
        public DateTime Time { get; private set; }
        public ChatStatus Status { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }

        public override string ToString()
        {
            switch (Status)
            {
                case ChatStatus.Welcome:
                    return $"{Name}님이 채팅방에 들어왔습니다.";
                case ChatStatus.Leave:
                    return $"{Name}님이 채팅방을 나갔습니다.";
                case ChatStatus.Chat:
                    return $"[{Name}] {Contents}";
                case ChatStatus.TypingStart:
                    return $"{Name}님이 채팅을 입력 중 입니다.";
                case ChatStatus.TypingEnd:
                    return $"{Name}님이 채팅을 입력 완료했습니다.";
                default:
                    return base.ToString();
            }
        }

        public string ToStringWithTime() => $"[{Time.ToLongTimeString()}] {ToString()}";

        public static bool Send(TcpClient sender, ChatPacket packet)
        {
            var stream = sender.GetStream();
            var writer = new BinaryWriter(stream);

            if (!(stream?.CanWrite) ?? false) return false;
            if (packet == null) throw new ArgumentNullException(nameof(packet));

            packet.Time = DateTime.Now;


            // Time - local -> utc 시간으로 변환 후 보내기
            writer.Write(packet.Time.ToUniversalTime().ToBinary());
            // Status
            writer.Write((int)packet.Status);
            // Name
            writer.Write(packet.Name ?? "NULL");
            // Chat contents
            if (packet.Status == ChatStatus.Chat) writer.Write(packet.Contents ?? "NULL");

            return true;
        }

        public static ChatPacket Recieve(TcpClient reciever)
        {
            var stream = reciever.GetStream();
            var reader = new BinaryReader(stream);

            if (!(stream?.CanRead) ?? false) return null;

            var packet = new ChatPacket
            {
                // Time - utc 시간을 받아서 local 시간으로 변환
                Time = DateTime.FromBinary(reader.ReadInt64()).ToLocalTime(),
                // Status
                Status = (ChatStatus)reader.ReadInt32(),
                // Name
                Name = reader.ReadString()
            };
            // Chat contents
            if (packet.Status == ChatStatus.Chat) packet.Contents = reader.ReadString();

            return packet;
        }
    }
}
