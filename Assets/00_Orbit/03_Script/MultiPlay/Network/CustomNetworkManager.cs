using Mirror;
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

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            Transform startPos = GetStartPosition();
            GameObject player = startPos != null
                ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                : Instantiate(playerPrefab);

            //// instantiating a "Player" prefab gives it the name "Player(clone)"
            //// => appending the connectionId is WAY more useful for debugging!
            player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
            NetworkServer.AddPlayerForConnection(conn, player);

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