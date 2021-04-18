using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using BasicChat.Lib;
using UnityEngine;
using UnityEngine.Events;

namespace BasicChat.Client.Network
{
    public class ChatClient : MonoBehaviour
    {
        public class ChatEvent : UnityEvent<ChatPacket> { }
        public class ExceptionThrownEvent : UnityEvent<Exception> { }

        private readonly ConcurrentQueue<ChatPacket> _recievedPackets = new ConcurrentQueue<ChatPacket>();
        private readonly ConcurrentQueue<ChatPacket> _packetWaitList = new ConcurrentQueue<ChatPacket>();

        private readonly IPAddress _serverAddress = IPAddress.Parse("127.0.0.1");
        private const int ServerPort = 9090;
        private readonly IPEndPoint _clientAddress = new IPEndPoint(IPAddress.Any, 0);

        public TcpClient Client => _client;
        private TcpClient _client;

        public ExceptionThrownEvent OnExceptionThrownEvent { get; set; } = new ExceptionThrownEvent();

        public UnityEvent OnConnectionFaildEvent { get; set; } = new UnityEvent();
        public UnityEvent OnConnectedEvent { get; set; } = new UnityEvent();
        public UnityEvent OnDisconnectedEvent { get; set; } = new UnityEvent();

        public bool IsConnected
        {
            get
            {
                if (_client == null) return false;

                try
                {
                    var stream = _client.GetStream();
                    return stream.CanRead;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 채팅서버에 연결을 시도합니다. 성공여부는 <see cref="OnConnectedEvent"/>로 전달됩니다.
        /// </summary>
        /// <exception cref="SocketException"/>
        public void TryConnect()
        {
            Task.Run(HandleConnect);
        }

        public void Disconnect()
        {
            // _client.Client.Shutdown(SocketShutdown.Both);
            _client.Close();
        }

        /// <summary>
        /// 패킷을 전송 대기열에 추가합니다.
        /// </summary>
        /// <param name="packet">전송할 패킷</param>
        public void AddPacketToWaitList(ChatPacket packet)
        {
            _packetWaitList.Enqueue(packet);
        }

        /// <summary>
        /// 받은 패킷이 있는지 확인합니다.
        /// </summary>
        /// <param name="packet">패킷이 있을 경우 받은 패킷 반환. 없으면 기본 인스턴스 반환</param>
        /// <returns>받은 패킷이 있는지 여부</returns>
        public bool TryRecieve(out ChatPacket packet)
        {
            return _recievedPackets.TryDequeue(out packet);
        }

        private void HandleConnect()
        {
            // if (_client.Connected) _client.Close();

            _client = new TcpClient(_clientAddress);
            try
            {
                _client.Connect(_serverAddress, ServerPort);
            }
            catch (SocketException e)
            {
                OnExceptionThrownEvent?.Invoke(e);
                OnConnectionFaildEvent?.Invoke();
                return;
            }

            OnConnectedEvent?.Invoke();

            Task.Run(() => HandleSend());
            Task.Run(() => HandleRecieve());
        }

        private void HandleSend()
        {
            while (true)
            {
                if (_packetWaitList.TryDequeue(out var packet))
                {
                    try
                    {
                        ChatPacket.Send(_client, packet);
                    }
                    catch (IOException)
                    {
                        // OnExceptionThrownEvent?.Invoke(e);
                        OnDisconnectedEvent?.Invoke();
                        return;
                    }
                }
            }
        }

        private void HandleRecieve()
        {
            while (true)
            {
                try
                {
                    var packet = ChatPacket.Recieve(_client);
                    _recievedPackets.Enqueue(packet);
                }
                catch (EndOfStreamException)
                {
                    OnDisconnectedEvent?.Invoke();
                    return;
                }
                catch (IOException)
                {
                    // OnExceptionThrownEvent?.Invoke(e);
                    OnDisconnectedEvent?.Invoke();
                    return;
                }
            }
        }
    }
}
