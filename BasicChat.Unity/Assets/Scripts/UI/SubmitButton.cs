using UnityEngine;
using UnityEngine.UI;

namespace BasicChat.Client.UI
{
    public class SubmitButton : MonoBehaviour
    {
        [SerializeField] private InputField _textInputField;
        [SerializeField] private ChatBubbleGenerator _chatBubbleGenerator;

        private bool _isHotkey1Pressed = false;

        private void Awake()
        {
            Debug.Assert(_textInputField);
        }

        private void Update()
        {
            // Down과 Up 각각 한번씩만 호출되어야 함. GetKey를 쓰면 Update마다 설정되어서 안됨
            if (Input.GetKeyDown(KeyCode.LeftShift)) _isHotkey1Pressed = true;
            if (Input.GetKeyUp(KeyCode.LeftShift)) _isHotkey1Pressed = false;

            // enter(return)         : submit
            // shift + enter(return) : newline
            // shift를 누른 상태에서는 _isHotkey1Pressed가 true이기 때문에 아래 조건문 실행 안됨
            // 아래 조건문을 실행하지 않기 때문에 InputField에서는 그대로 enter키를 읽어서 newline이 들어감
            if (!_isHotkey1Pressed && Input.GetKeyDown(KeyCode.Return))
            {
                OnClick();
                _textInputField.ActivateInputField();
                return;
            }
        }

        public void OnClick()
        {
            var text = _textInputField.text;
            _textInputField.text = string.Empty;

            if (string.IsNullOrWhiteSpace(text)) return;
            _chatBubbleGenerator.AddChatBubble("Player", text);
            print(text);
        }
    }
}
