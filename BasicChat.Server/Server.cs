using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using BasicChat.Server;

const int Port = 9090;

var listener = new TcpListener(IPAddress.Any, Port);
listener.Server.NoDelay = true;
listener.Start();

var acceptTask = Task.Run(async () =>
{
    while (true)
    {
        var client = await listener.AcceptTcpClientAsync();
        ChatListener.AddClient(client);
    }
});

var closeTask = Task.Run(() =>
{
    string answer;
    while (true)
    {
        Console.WriteLine("If you want to stop the bot, enter 'stop'");
        answer = Console.ReadLine();
        if (answer is "stop")
        {
            listener.Stop();
            break;
        }
    }
});

await Task.WhenAny(acceptTask);
