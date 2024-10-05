using Mirror;
using System;
using UnityEngine;

namespace STARTING
{
    public class CustomNetworkManager : NetworkManager
    {
        public static new CustomNetworkManager singleton;

        public override void Awake()
        {
            base.Awake();

            if (singleton == null)
            {
                singleton = this;
            }
            else if (singleton != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.RegisterHandler<LoginRequestMessage>(OnLoginRequest);
            NetworkServer.RegisterHandler<RegisterRequestMessage>(OnRegisterRequest);
            NetworkServer.RegisterHandler<GameDataRequestMessage>(OnGameDataRequest);
            NetworkServer.RegisterHandler<GameDataUpdateRequestMessage>(OnGameDataUpdateRequest);
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            // 클라이언트가 연결될 때 모든 네트워크 오브젝트의 상태를 동기화
            //foreach (var netObj in FindObjectsOfType<NetworkedObject>())
            //{
            //    netObj.RpcSyncColor(netObj.objectColor);
            //}
        }

        //public override void OnClientDisconnect()
        //{
        //    SceneManager.LoadScene("MainScene");
            
        //    ModalWindowManager disconnectPopup = GameObject.Find("ClientDisconnect").GetComponent<ModalWindowManager>();
        //    disconnectPopup.ModalWindowIn();
        //}

        /// <summary>
        /// 클라이언트로부터 받은 로그인 요청 메시지
        /// 처리 결과에 따라 결과값 반환
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msg"></param>
        private void OnLoginRequest(NetworkConnectionToClient conn, LoginRequestMessage msg)
        {
            bool success = DBManager.Instance.Login(msg.username, msg.password, out int userId);

            LoginResponseMessage response = new LoginResponseMessage
            {
                success = success,
                userName = msg.username,
                userId = success ? userId : -1
            };
            Debug.Log($"서버가 로그인 요청 받음 : ID {msg.username}, PW {msg.password}, 성공여부 {success}, userID {userId}");
            conn.Send(response);
        }

        /// <summary>
        /// 클라이언트로부터 받은 회원가입 요청 메시지
        /// 처리 결과에 따라 결과값 반환
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msg"></param>
        private void OnRegisterRequest(NetworkConnectionToClient conn, RegisterRequestMessage msg)
        {
            bool success = false;

            try
            {
                DBManager.Instance.Register(msg.username, msg.password);
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                Debug.Log("회원가입 오류 :" + ex);
            }

            RegisterResponseMessage response = new RegisterResponseMessage
            {
                success = success
            };
            conn.Send(response);
        }


        private void OnGameDataRequest(NetworkConnection conn, GameDataRequestMessage msg)
        {
            GameData gameData = DBManager.Instance.GetGameDataFromDB(msg.userId);

            GameDataResponseMessage response = new GameDataResponseMessage
            {
                gameData = gameData
            };

            conn.Send(response);
        }

        private void OnGameDataUpdateRequest(NetworkConnection conn, GameDataUpdateRequestMessage msg)
        {
            DBManager.Instance.UpdateGameDataInDB(msg.userId, msg.gameData);
        }

        public void BackToMain()
        {
            if (true == NetworkServer.active)
            {
                StopHost();
            }

            if (true == NetworkClient.active)
            {
                StopClient();
            }
        }
    }
}