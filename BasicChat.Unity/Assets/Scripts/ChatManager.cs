using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
using BasicChat.Client.Network;
using BasicChat.Client.UI;
using BasicChat.Lib;
using UnityEngine;

namespace BasicChat.Client
{
    public class ChatManager : MonoBehaviour
    {
        private class ChatBubbleData
        {
            public string Name { get; set; }
            public Color? NameColor { get; set; } = null;
            public string Message { get; set; }
            public Color? MessageColor { get; set; } = null;
        }

        [SerializeField]
        private ChatBubbleGenerator _generator;

        [SerializeField]
        private ChatClient _chatClient;

        [SerializeField]
        private SubmitButton _submitButton;

        private const string AssistantName = "(ㅇㅅㅇ)";

        private readonly ConcurrentQueue<ChatBubbleData> _messages = new ConcurrentQueue<ChatBubbleData>();

        private void Start()
        {
            _submitButton.OnSubmitted.AddListener(HandleNameRegister);

            _chatClient.OnExceptionThrownEvent.AddListener(HandleException);
            _chatClient.OnConnectionFaildEvent.AddListener(HandleConnectionFaild);
            _chatClient.OnConnectedEvent.AddListener(HandleConnected);
            _chatClient.OnDisconnectedEvent.AddListener(HandleDisconnected);

            ChatData.UserName = "unknown";
            _generator.AddChatBubble(AssistantName, "안녕하세요!");
            _generator.AddChatBubble(AssistantName, "채팅방에서 사용할 이름을 알려주세요.");
        }

        private void Update()
        {
            if (_messages.TryDequeue(out var result))
            {
                _generator.AddChatBubble(result.Name, result.Message, result.NameColor, result.MessageColor);
            }

            if (_chatClient.TryRecieve(out var packet))
            {
                switch (packet.Status)
                {
                    case ChatStatus.Welcome:
                        _generator.AddChatBubble(
                            name: AssistantName,
                            message: $"{packet.Name}님이 채팅방에 들어왔습니다.",
                            messageColor: BasicChatColors.Blue);
                        break;

                    case ChatStatus.Leave:
                        _generator.AddChatBubble(
                            name: AssistantName,
                            message: $"{packet.Name}님이 채팅방을 나갔습니다.",
                            messageColor: BasicChatColors.Blue);
                        break;

                    case ChatStatus.Chat:
                        _generator.AddChatBubble(
                            name: packet.Name,
                            message: packet.Message,
                            messageColor: BasicChatColors.White);
                        break;

                    default:
                        break;
                }
            }
        }

        private void AddChatBubble(string name, string message, Color? nameColor = null, Color? messageColor = null)
        {
            _messages.Enqueue(new ChatBubbleData
            {
                Name = name,
                Message = message,
                NameColor = nameColor,
                MessageColor = messageColor
            });
        }

        public void HandleNameRegister(string name)
        {
            if (!Regex.IsMatch(name, "^[0-9a-zA-Z가-힣]*$"))
            {
                AddChatBubble(name: AssistantName, message: "특수문자는 입력할 수 없어요... 다시 입력해주세요.");

                return;
            }
            int count = name.ToCharArray().Sum(ch => Regex.IsMatch(ch.ToString(), "[0-9a-zA-Z]") ? 1 : 2);

            if (count > 12)
            {
                AddChatBubble(
                    name: AssistantName,
                    message: "이름은 최대 한글 6자, 영어/숫자 12자까지만 입력가능해요! 다시 입력해주세요.");

                return;
            }

            AddChatBubble(
                name: AssistantName,
                message: $"{name}이라... 좋은 이름이네요! 채팅방에 연결을 시작합니다.");

            ChatData.UserName = name;
            _submitButton.OnSubmitted.RemoveListener(HandleNameRegister);

            _submitButton.OnSubmitted.AddListener(HandleSubmiitedChat);
            _chatClient.TryConnect();
        }

        public void HandleSubmiitedChat(string text)
        {
            if (text.StartsWith("/"))
            {
                HandleCommand(text.Length != 1 ? text.Substring(1) : string.Empty);
                return;
            }

            if (!_chatClient.IsConnected)
            {
                AddChatBubble(
                    name: ChatData.UserName,
                    message: text,
                    messageColor: BasicChatColors.Gray);
                return;
            }

            var packet = new ChatPacket { Name = ChatData.UserName, Status = ChatStatus.Chat, Message = text };
            _chatClient.AddPacketToWaitList(packet);
        }

        private void HandleCommand(string command)
        {
            Color messageColor = BasicChatColors.White;
            Color nameColor = BasicChatColors.Black;
            string message;

            switch (command)
            {
                case "connect":
                    if (_chatClient.IsConnected)
                    {
                        message = "이미 연결되어 있습니다.";
                        break;
                    }

                    message = "채팅방에 접속을 시도합니다.";
                    _chatClient.TryConnect();
                    break;

                case "disconnect":
                    if (!_chatClient.IsConnected)
                    {
                        message = "채팅방이 연결되어 있지 않습니다.";
                        break;
                    }

                    message = "채팅방을 나갑니다.";
                    _chatClient.Disconnect();
                    break;

                default:
                    message = "잘못된 명령어입니다.";
                    nameColor = BasicChatColors.Gray;
                    messageColor = BasicChatColors.Gray;
                    break;
            }

            AddChatBubble(
                name: AssistantName,
                nameColor: nameColor,
                message: message,
                messageColor: messageColor);
        }

        private void HandleException(Exception e)
        {
            Debug.LogError(e);
        }

        private void HandleConnectionFaild()
        {
            AddChatBubble(
                name: AssistantName,
                nameColor: BasicChatColors.Red,
                message: "채팅방에 연결하지 못했습니다.",
                messageColor: BasicChatColors.Red);
        }

        private void HandleConnected()
        {
            AddChatBubble(
                name: AssistantName,
                message: "채팅방에 연결되었습니다!");

            var packet = new ChatPacket { Name = ChatData.UserName, Status = ChatStatus.Welcome };
            _chatClient.AddPacketToWaitList(packet);
        }

        private void HandleDisconnected()
        {
            AddChatBubble(
                name: AssistantName,
                message: "서버와의 연결이 끊겼습니다.",
                messageColor: BasicChatColors.White);
        }
    }
}
