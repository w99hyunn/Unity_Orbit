using Mirror;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
        private Coroutine _fadeCoroutine;

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
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnInputFieldSubmit();
            }
        }

        private void ToggleInputField()
        {
            _isInputFieldActive = !_isInputFieldActive;
            chatInputField.interactable = _isInputFieldActive;
            NetworkClient.localPlayer.gameObject.GetComponent<PlayerInput>().enabled = !_isInputFieldActive;


            if (_isInputFieldActive)
            {
                chatCanvasGroup.alpha = 1f;
                chatInputField.ActivateInputField();

                if (_fadeCoroutine != null)
                {
                    StopCoroutine(_fadeCoroutine);
                    _fadeCoroutine = null;
                }
            }
            else
            {
                _fadeCoroutine = StartCoroutine(FadeOutChatCanvasGroup(5f));
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
                // 클라이언트에서 서버로 메시지 전송
                ChatManager.Instance?.CmdSendChatMessage(
                    NetworkClient.localPlayer != null ? NetworkClient.localPlayer.gameObject.name : "Anonymous",
                    chatInputField.text.Trim()
                );
                chatInputField.text = "";
                ToggleInputField();
            }
            else
            {
                ToggleInputField();
            }
        }

        private IEnumerator FadeOutChatCanvasGroup(float duration)
        {
            float startAlpha = chatCanvasGroup.alpha;
            float targetAlpha = 0.15f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                chatCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
                yield return null;
            }
            chatCanvasGroup.alpha = targetAlpha;
        }
    }
}