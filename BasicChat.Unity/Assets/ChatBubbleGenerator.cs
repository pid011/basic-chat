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

        public void AddChatBubble(string name, string contents)
        {
            var chatbubble = Instantiate(_chatBubblePrefab, transform);
            var chatbox = chatbubble.GetComponent<ChatBubble>();

            chatbox.UserName = name;
            chatbox.Contents = contents;

            _fitter.SetLayoutVertical();
            _fitter.SetLayoutHorizontal();
            _rectTransform.ForceUpdateRectTransforms();
        }
    }
}
