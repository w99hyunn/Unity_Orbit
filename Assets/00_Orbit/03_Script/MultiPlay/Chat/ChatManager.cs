using Mirror;

namespace NOLDA
{
    public class ChatManager : NetworkBehaviour
    {
        public static ChatManager Instance;

        private void Awake()
        {
            // 싱글톤 패턴 적용
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        // 메시지를 클라이언트에서 서버로 전송
        [Command(requiresAuthority = false)]
        public void CmdSendChatMessage(string playerName, string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            // 서버에서 모든 클라이언트로 메시지 전송
            RpcReceiveChatMessage(playerName, message);
        }

        // 서버에서 모든 클라이언트에 메시지를 전달
        [ClientRpc]
        private void RpcReceiveChatMessage(string playerName, string message)
        {
            // UIManager의 인스턴스를 통해 UI 업데이트
            ChatSupport.Instance?.AddChatMessage(playerName, message);
        }
    }
}