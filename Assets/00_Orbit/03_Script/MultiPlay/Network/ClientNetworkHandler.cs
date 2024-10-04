using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace STARTING
{
    public class ClientNetworkHandler : MonoBehaviour
    {
        public UnityEvent loginSuccess;
        public UnityEvent loginFail;
        public UnityEvent registerSuccess;
        public UnityEvent registerFail;

        /// <summary>
        /// 로그인 요청 메시지
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void SendLoginRequest(string username, string password)
        {
            NetworkClient.RegisterHandler<LoginResponseMessage>(OnLoginResponse);

            LoginRequestMessage loginRequest = new LoginRequestMessage
            {
                username = username,
                password = password
            };

            NetworkClient.Send(loginRequest);
        }

        /// <summary>
        /// 로그인 요청에 대해 서버로부터 응답받은 메시지
        /// </summary>
        /// <param name="msg"></param>
        private void OnLoginResponse(LoginResponseMessage msg)
        {
            if (msg.success)
            {
                Debug.Log("Login successful! User ID: " + msg.userId);
                DBManager.Instance.userName = msg.userName;
                DBManager.Instance.userId = msg.userId;
                loginSuccess?.Invoke();
                
                //게임 데이터 요청
                SendRequestGameData();
            }
            else
            {
                loginFail?.Invoke();
                Debug.LogError("Login failed.");
            }
        }

        /// <summary>
        /// 회원가입 요청 메시지
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void SendRegisterRequest(string username, string password)
        {
            NetworkClient.RegisterHandler<RegisterResponseMessage>(OnRegisterResponse);

            RegisterRequestMessage registerRequest = new RegisterRequestMessage
            {
                username = username,
                password = password
            };

            NetworkClient.Send(registerRequest);
        }

        /// <summary>
        /// 회원가입 요청에 대해 서버로부터 응답받은 메시지
        /// </summary>
        /// <param name="msg"></param>
        private void OnRegisterResponse(RegisterResponseMessage msg)
        {
            if (msg.success)
            {
                registerSuccess?.Invoke();
            }
            else
            {
                registerFail?.Invoke();
            }
        }

        /// <summary>
        /// 게임 데이터 요청 메시지
        /// </summary>
        public void SendRequestGameData()
        {
            NetworkClient.RegisterHandler<GameDataResponseMessage>(OnGameDataResponse);
            int userId = DBManager.Instance.userId;

            GameDataRequestMessage request = new GameDataRequestMessage
            {
                userId = userId
            };

            NetworkClient.Send(request);
        }

        /// <summary>
        /// 게임 데이터 요청에 대해 응답받은 메시지
        /// </summary>
        /// <param name="msg"></param>
        private void OnGameDataResponse(GameDataResponseMessage msg)
        {
            DBManager.Instance.clientGameData = msg.gameData;

            // 받은 게임 데이터를 활용하여 게임을 초기화
            Debug.Log("Game data loaded:");
            Debug.Log("Health: " + DBManager.Instance.clientGameData.currentHealth);
            Debug.Log("Mana: " + DBManager.Instance.clientGameData.currentMana);
            Debug.Log("Experience: " + DBManager.Instance.clientGameData.currentExperience);
            Debug.Log("Player Position: " + DBManager.Instance.clientGameData.playerPosition);
            Debug.Log("level: " + DBManager.Instance.clientGameData.level);
        }

        /// <summary>
        /// 클라이언트의 게임 데이터 저장 요청 메시지
        /// </summary>
        /// <param name="currentGameData"></param>
        public void SendRequestUpdatedGameData(GameData currentGameData)
        {
            int userId = DBManager.Instance.userId;
            GameDataUpdateRequestMessage updateMessage = new GameDataUpdateRequestMessage
            {
                userId = userId,
                gameData = currentGameData
            };
            Debug.Log("클라이언트로부터 게임 저장 요청함. 유저아이디: " + userId + "/ 레벨은: " + currentGameData.level);
            NetworkClient.Send(updateMessage);
        }
    }
}