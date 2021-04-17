using UnityEngine;
using UnityEngine.UI;

namespace BasicChat.Client.UI
{
    public class ChatBubble : MonoBehaviour
    {
        [SerializeField]
        private Text _playerNameText;

        [SerializeField]
        private Text _contentsText;

        private ContentSizeFitter _chatBubbleFitter;
        private ContentSizeFitter _contentsFitter;
        private RectTransform _rectTransform;

        private void Awake()
        {
            Debug.Assert(_playerNameText && _contentsText);

            _chatBubbleFitter = GetComponent<ContentSizeFitter>();
            _contentsFitter = _contentsText.gameObject.GetComponent<ContentSizeFitter>();
            _rectTransform = GetComponent<RectTransform>();
        }

        public string UserName
        {
            get
            {
                return _playerNameText.text;
            }
            set
            {
                _playerNameText.text = value;
            }
        }

        public Color UserNameColor
        {
            get
            {
                return _playerNameText.color;
            }
            set
            {
                _playerNameText.color = value;
            }
        }

        public string Message
        {
            get
            {
                return _contentsText.text;
            }
            set
            {
                _contentsText.text = value;
                _contentsFitter.SetLayoutVertical();
                ResetChatBubbleLayout();
            }
        }

        public Color MessageColor
        {
            get
            {
                return _contentsText.color;
            }
            set
            {
                _contentsText.color = value;
            }
        }

        private void ResetChatBubbleLayout()
        {
            _chatBubbleFitter.SetLayoutVertical();
            _rectTransform.ForceUpdateRectTransforms();
        }
    }
}
