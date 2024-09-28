using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace STARTING
{
    public class StartMultiplay : MonoBehaviour
    {
        public MainUISupport uiSupport;

        /// <summary>
        /// 멀티플레이 - 서버 오픈
        /// </summary>
        public void ServerOpen()
        {
            string ip = uiSupport.GetHostIP();
            string portText = uiSupport.GetHostPort();

            CustomNetworkManager.singleton.networkAddress = ip;
            if (Transport.active is PortTransport portTransport)
            {
                if (ushort.TryParse(portText, out ushort port))
                    portTransport.Port = port;
            }

            CustomNetworkManager.singleton.StartHost();
            StartCoroutine(LoadWorldScene());
        }

        /// <summary>
        /// 멀티플레이 - 클라이언트 접속
        /// </summary>
        public void ClientConnect()
        {
            string ip = uiSupport.GetClientIP();
            string portText = uiSupport.GetClientPort();

            CustomNetworkManager.singleton.networkAddress = ip;
            if (Transport.active is PortTransport portTransport)
            {
                if (ushort.TryParse(portText, out ushort port))
                    portTransport.Port = port;
            }

            CustomNetworkManager.singleton.StartClient();

            StartCoroutine(LoadWorldScene());
        }

        private IEnumerator LoadWorldScene()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            AsyncOperation op = SceneManager.LoadSceneAsync("WorldScene_Multi", LoadSceneMode.Single);
            op.allowSceneActivation = false;

            while (op.progress < 0.9f)
            {
                float progress = Mathf.Clamp01(op.progress / 0.9f);
                uiSupport.LoadingProgress((progress * 100).ToString("F0"));

                yield return null;
            }
            uiSupport.LoadingProgress("100");

            yield return new WaitForSeconds(1f); // 1초 대기

            SceneManager.LoadScene("Element_UI_Multi", LoadSceneMode.Additive);

            op.allowSceneActivation = true;
            while (!op.isDone)
            {
                yield return null;
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByName("WorldScene_Multi"));
        }


        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("OnSceneLoaded 호출됨: " + scene.name);

            if (scene.name == "WorldScene_Multi")
            {
                if (NetworkClient.isConnected)
                {
                    NetworkClient.AddPlayer();
                    Debug.Log("플레이어 추가");
                }
                else
                {
                    Debug.LogError("NetworkClient가 연결 x");
                }
            }
        }

    }
}