using Mirror;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace STARTING
{
    public class ChatSupport : MonoBehaviour
    {
        public static ChatSupport Instance { get; private set; }

        public CanvasGroup chatCanvasGroup;
        public GameObject chatMessagePrefab; // 채팅 메시지 프리팹
        public Transform chatContentPanel;
        public TMP_InputField chatInputField;
        public ScrollRect scrollView;

        private bool _isInputFieldActive = false;
        private Coroutine fadeCoroutine;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            chatInputField.interactable = false;
            StartCoroutine(FadeOutChatCanvasGroup(5f));
            chatInputField.onEndEdit.AddListener(delegate { OnInputFieldSubmit(); });
        }

        private void Update()
        {
            // Enter 키로 InputField 활성화/비활성화 전환
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (_isInputFieldActive && !string.IsNullOrEmpty(chatInputField.text))
                {
                    // 클라이언트에서 ChatManager를 통해 메시지를 서버로 전송
                    ChatManager.Instance?.CmdSendChatMessage(
                        NetworkClient.localPlayer != null ? NetworkClient.localPlayer.gameObject.name : "Anonymous",
                        chatInputField.text
                    );
                    chatInputField.text = ""; // 채팅 후 입력란 초기화
                }
                else
                {
                    ToggleInputField();
                }
            }
        }

        private void ToggleInputField()
        {
            _isInputFieldActive = !_isInputFieldActive;
            chatInputField.interactable = _isInputFieldActive;

            if (_isInputFieldActive)
            {
                chatCanvasGroup.alpha = 1f;
                chatInputField.ActivateInputField();

                // 활성화되면 코루틴 중지
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                    fadeCoroutine = null; // 코루틴 참조를 null로 설정
                }
            }
            else
            {
                fadeCoroutine = StartCoroutine(FadeOutChatCanvasGroup(5f)); //5초 뒤 채팅창 투명도 줄이기
            }
        }

        // 채팅 메시지 추가, 스크롤 위치를 아래 고정
        public void AddChatMessage(string playerName, string message)
        {
            GameObject chatMessageObject = Instantiate(chatMessagePrefab, chatContentPanel);
            ChatMessage chatMessage = chatMessageObject.GetComponent<ChatMessage>();

            chatMessage.SetMessage(playerName, message);

            Canvas.ForceUpdateCanvases();
            scrollView.verticalNormalizedPosition = 0f;
        }

        // InputField 엔터
        private void OnInputFieldSubmit()
        {
            if (_isInputFieldActive && !string.IsNullOrEmpty(chatInputField.text))
            {
                // 클라이언트에서 ChatManager를 통해 메시지를 서버로 전송
                ChatManager.Instance?.CmdSendChatMessage(
                    NetworkClient.localPlayer != null ? NetworkClient.localPlayer.gameObject.name : "Anonymous",
                    chatInputField.text
                );
                chatInputField.text = ""; // 채팅 후 입력란 초기화
            }
            else
            {
                ToggleInputField();
            }
        }

        // 알파 값을 줄이는 코루틴
        private IEnumerator FadeOutChatCanvasGroup(float duration)
        {
            float startAlpha = chatCanvasGroup.alpha;
            float targetAlpha = 0.15f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                chatCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
                yield return null; // 다음 프레임까지 대기
            }

            // 최종적으로 알파를 목표 값으로 설정
            chatCanvasGroup.alpha = targetAlpha;
        }
    }
}