using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using BasicChat.Lib;
using Serilog;

namespace LocalChat.ConsoleTest
{
    internal class ConsoleClient
    {
        private static async Task Main()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            IPEndPoint clientAddress = new(IPAddress.Any, 0);
            TcpClient client = new(clientAddress);

            try
            {
                await client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 9090);
            }
            catch (SocketException)
            {
                Log.Warning("서버에 연결하지 못했습니다.");
                return;
            }

            Console.Write("이름 입력: ");
            var userName = Console.ReadLine();

            var recieveTask = Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        if (!client.Connected) return;

                        var chat = ChatPacket.Recieve(client);
                        Console.WriteLine(chat.ToStringWithTimeAndName());
                    }
                }
                catch (SocketException)
                {
                    Log.Warning("서버와의 연결이 끊겼습니다.");
                }
                catch (IOException)
                {
                    Log.Warning("서버와의 연결이 끊겼습니다.");
                }
            });

            var sendTask = Task.Run(() =>
            {
                var packet = new ChatPacket { Name = userName };

                try
                {
                    packet.Status = ChatStatus.Welcome;
                    ChatPacket.Send(client, packet);

                    while (true)
                    {
                        var str = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(str)) continue;

                        packet.Status = ChatStatus.Chat;
                        packet.Message = str;
                        ChatPacket.Send(client, packet);
                    }
                }
                catch (SocketException)
                {
                    Log.Warning("서버와의 연결이 끊겼습니다.");
                }
            });

            await Task.WhenAny(sendTask, recieveTask);

            client.Close();
            Log.Information("채팅을 종료했습니다.");
        }
    }
}
