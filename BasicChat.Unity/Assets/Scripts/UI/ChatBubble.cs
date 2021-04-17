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
        private ContentSizeFitter _playerNameFitter;
        private ContentSizeFitter _contentsFitter;
        private RectTransform _rectTransform;

        private void Awake()
        {
            Debug.Assert(_playerNameText && _contentsText);

            _chatBubbleFitter = GetComponent<ContentSizeFitter>();
            _playerNameFitter = _playerNameText.gameObject.GetComponent<ContentSizeFitter>();
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

                _playerNameFitter.SetLayoutHorizontal();
                ResetChatBubbleLayout();
            }
        }

        public string Contents
        {
            get
            {
                return _contentsText.text;
            }
            set
            {
                var text = value.Trim('\n', '\t', '\r');
                _contentsText.text = text;

                _contentsFitter.SetLayoutVertical();
                ResetChatBubbleLayout();
            }
        }

        private void ResetChatBubbleLayout()
        {
            _chatBubbleFitter.SetLayoutHorizontal();
            _chatBubbleFitter.SetLayoutVertical();
            _rectTransform.ForceUpdateRectTransforms();
        }
    }
}
