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

        public override void OnStartServer()
        {
            base.OnStartServer();

            NetworkServer.RegisterHandler<LoginRequestMessage>(OnLoginRequest);
            NetworkServer.RegisterHandler<RegisterRequestMessage>(OnRegisterRequest);
            NetworkServer.RegisterHandler<GameDataRequestMessage>(OnGameDataRequest);
            NetworkServer.RegisterHandler<SaveGameMessage>(OnSaveGameRequest);
        }

        private void OnLoginRequest(NetworkConnectionToClient conn, LoginRequestMessage msg)
        {
            bool success = DBManager.Instance.Login(msg.username, msg.password, out int userId);

            LoginResponseMessage response = new LoginResponseMessage
            {
                success = success,
                userId = success ? userId : -1
            };
            Debug.Log("서버가 로그인 요청 받음" + msg.username + msg.password +  success + userId);
            conn.Send(response);
        }

        private void OnGameDataRequest(NetworkConnectionToClient conn, GameDataRequestMessage msg)
        {
            GameData gameData = DBManager.Instance.LoadGame(msg.userId);

            GameDataResponseMessage response = new GameDataResponseMessage
            {
                gameData = gameData
            };

            conn.Send(response);
        }

        private void OnSaveGameRequest(NetworkConnectionToClient conn, SaveGameMessage msg)
        {
            DBManager.Instance.SaveGame(msg.gameData, PlayerPrefs.GetInt("UserID")); // 유저 ID에 맞춰 게임 데이터 저장
        }

        private void OnRegisterRequest(NetworkConnectionToClient conn, RegisterRequestMessage msg)
        {
            string resultMessage;
            bool success = false;

            try
            {
                // DB에 회원 정보 저장
                DBManager.Instance.Register(msg.username, msg.password);
                success = true;
                resultMessage = "Registration successful!";
            }
            catch (Exception ex)
            {
                success = false;
                resultMessage = "Registration failed: " + ex.Message;
            }

            // 결과를 클라이언트에 전송
            RegisterResponseMessage response = new RegisterResponseMessage
            {
                success = success,
                message = resultMessage
            };
            conn.Send(response);
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            // targetSceneName 씬이 활성화된 씬들 중 하나인지 확인
            /*Debug.Log(targetSceneName);
            Scene targetScene = SceneManager.GetSceneByName(targetSceneName);
            if (targetScene.IsValid() && targetScene.isLoaded)
            {
                // targetScene에서 사용 가능한 시작 위치 찾기
                Transform startPos = GetStartPositionInScene(targetScene);
                GameObject player = startPos != null
                    ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                    : Instantiate(playerPrefab);
                SceneManager.MoveGameObjectToScene(player, targetScene);
                // 플레이어 객체의 이름 설정
                player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
                NetworkServer.AddPlayerForConnection(conn, player);
            }
            else
            {
                Debug.LogWarning($"씬 '{targetSceneName}'이 활성화되지 않았거나 로드되지 않았습니다.");
            }*/

            //folderDownloader.TriggerFolderDownload();



            // 클라이언트가 연결될 때 모든 네트워크 오브젝트의 상태를 동기화
            //foreach (var netObj in FindObjectsOfType<NetworkedObject>())
            //{
            //    netObj.RpcSyncColor(netObj.objectColor);
            //}
        }

    }
}