using UnityEngine;
using UnityEngine.UI;

namespace BasicChat.Client.UI
{
    public class ChatBubbleGenerator : MonoBehaviour
    {
        [SerializeField] private VerticalLayoutGroup _layoutGroup;
        [SerializeField] private GameObject _chatBubblePrefab;

        private ContentSizeFitter _fitter;
        private RectTransform _rectTransform;

        private void Awake()
        {
            Debug.Assert(_layoutGroup && _chatBubblePrefab);
            _fitter = GetComponent<ContentSizeFitter>();
            _rectTransform = GetComponent<RectTransform>();
        }

        public void AddChatBubble(string name, string message, Color? nameColor = null, Color? messageColor = null)
        {
            if (nameColor == null) nameColor = BasicChatColors.Black;
            if (messageColor == null) messageColor = BasicChatColors.White;

            var chatbubble = Instantiate(_chatBubblePrefab, transform);
            var chatbox = chatbubble.GetComponent<ChatBubble>();

            chatbox.UserName = name;
            chatbox.UserNameColor = nameColor.Value;

            chatbox.Message = message;
            chatbox.MessageColor = messageColor.Value;

            _fitter.SetLayoutVertical();
            _fitter.SetLayoutHorizontal();
            _rectTransform.ForceUpdateRectTransforms();
        }
    }
}
