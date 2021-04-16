using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using BasicChat.Lib;

namespace BasicChat.Server
{
    public static class ChatListener
    {
        /// <summary>
        /// 연결된 TcpClient 목록
        /// </summary>
        private static readonly ConcurrentDictionary<EndPoint, TcpClient> s_listeners = new();

        public static void AddClient(TcpClient client)
        {
            if (client == null || !client.Connected) throw new ArgumentException($"클라이언트가 연결되어있지 않습니다.");

            Task.Run(() => HadleRecieve(client));
        }

        private static void HadleRecieve(TcpClient client)
        {
            var endpoint = client.Client.RemoteEndPoint;
            s_listeners.TryAdd(endpoint, client);

            string userName = null;
            try
            {
                while (true)
                {
                    var packet = ChatPacket.Recieve(client);
                    userName = packet.Name;
                    SendChatToAll(packet);
                }
            }
            catch (IOException)
            {
                Console.WriteLine($"{endpoint}와의 연결이 끊겼습니다.");
            }
            finally
            {
                s_listeners.TryRemove(endpoint, out var removed);
                if (userName != null)
                {
                    var packet = new ChatPacket { Status = ChatStatus.Leave, Name = userName };
                    SendChatToAll(packet);
                }

                removed.Close();
            }
        }

        private static void SendChatToAll(ChatPacket packet)
        {
            foreach (var item in s_listeners)
            {
                ChatPacket.Send(item.Value, packet);
            }
            Console.WriteLine(packet.ToStringWithTime());
        }
    }
}
