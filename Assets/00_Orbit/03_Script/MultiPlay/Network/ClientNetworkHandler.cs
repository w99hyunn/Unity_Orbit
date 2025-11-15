using Mirror;
using UnityEngine;

namespace NOLDA
{
    public class ClientNetworkHandler : MonoBehaviour
    {
        public MainUISupport MainUISupport_multi;

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
                DBManager.Instance.userName = msg.userName;
                DBManager.Instance.userId = msg.userId;
                MainUISupport_multi.LoginSuccessEvent();
                
                //게임 데이터 요청
                SendRequestGameData();
            }
            else
            {
                MainUISupport_multi.LoginFailEvent();
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
                MainUISupport_multi.RegisterSuccessEvent();
            }
            else
            {
                MainUISupport_multi.RegisterFailEvent();
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
            //Debug.Log("클라이언트로부터 게임 저장 요청함. 유저아이디: " + userId + "/ 레벨은: " + currentGameData.level);
            NetworkClient.Send(updateMessage);
        }
    }
}