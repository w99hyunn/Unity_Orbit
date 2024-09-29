using Mirror;
using System;
using System.Collections;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace STARTING
{
    enum ServerType {
        HOST,
        CLIENT
    };
    
    public class StartMultiplay : MonoBehaviour
    {
        public MainUISupport uiSupport;

        public UnityEvent startLoading;
        public UnityEvent connectFail;

        private string publicIP;
        private string hamachiIP;

        private void Start()
        {
            StartCoroutine(GetPublicIP());
            hamachiIP = GetHamachiIP();
        }

        IEnumerator GetPublicIP()
        {
            UnityWebRequest request = UnityWebRequest.Get("https://checkip.amazonaws.com");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                publicIP = request.downloadHandler.text;
            }
        }

        string GetHamachiIP()
        {
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.Name.Contains("Hamachi"))
                {
                    foreach (UnicastIPAddressInformation ip in networkInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            return ip.Address.ToString();
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 멀티플레이 - 서버 오픈
        /// </summary>
        public void ServerOpen()
        {
            string ip = "localhost";
            ushort port = GetAvailablePort(1024, 65535);

            CustomNetworkManager.singleton.networkAddress = ip;
            if (Transport.active is PortTransport portTransport)
            {
                portTransport.Port = port;
            }

            CustomNetworkManager.singleton.StartHost();
            StartCoroutine(CheckOpen(port));
        }

        private IEnumerator CheckOpen(ushort port)
        {
            while (true == NetworkServer.active)
            {
                if (true == NetworkServer.active) // 서버에 연결된 상태
                {
                    startLoading?.Invoke();
                    GameStart(ServerType.HOST, port);
                    yield break;
                }
                yield return null;
            }
            connectFail?.Invoke();
            yield break;
        }

        private ushort GetAvailablePort(int minPort, int maxPort)
        {
            System.Random rand = new System.Random();
            ushort randomPort;
            bool isPortAvailable;

            do
            {
                randomPort = (ushort)rand.Next(minPort, maxPort + 1);
                isPortAvailable = CheckPortAvailable(randomPort);
            } while (!isPortAvailable);

            return randomPort;
        }

        private bool CheckPortAvailable(int port)
        {
            bool isAvailable = true;

            try
            {
                TcpListener listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                listener.Stop();
            }
            catch (SocketException)
            {
                isAvailable = false;
            }

            return isAvailable;
        }

        /// <summary>
        /// 멀티플레이 - 클라이언트 접속
        /// </summary>
        public void ClientConnect()
        {
            string ip = uiSupport.GetClientIP();
            string portText = uiSupport.GetClientPort();
            ushort uport = 0;

            CustomNetworkManager.singleton.networkAddress = ip;
            if (Transport.active is PortTransport portTransport)
            {
                if (ushort.TryParse(portText, out ushort port))
                {
                    portTransport.Port = port;
                    uport = port;
                }
            }

            CustomNetworkManager.singleton.StartClient();
            StartCoroutine(CheckConnection(uport, ip));
        }

        /// <summary>
        /// 클라이언트가 StartClient()를 실행시 NetworkClient.active 상태가 됨.
        /// 이 상태동안 서버가 연결되는지 확인하고 연결되지 않으면 Fail 알림을 띄움
        /// </summary>
        /// <param name="uport"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        private IEnumerator CheckConnection(ushort uport, string ip)
        {
            while (true == NetworkClient.active) // StartClient가 실행되어 서버에 연결이 됐는지 체크하는 상태
            {
                if (true == NetworkClient.isConnected) // 서버에 연결된 상태
                {
                    startLoading?.Invoke();
                    GameStart(ServerType.CLIENT, uport, ip);
                    yield break;
                }
                yield return null;
            }
            connectFail?.Invoke();
            yield break;
        }

        void GameStart(ServerType type, ushort port, string ip = null)
        {
            string message;

            if (type == ServerType.HOST)
            {
                if (!string.IsNullOrEmpty(hamachiIP))
                {
                    message = $"공인 IP : {publicIP}하마치 IP : {hamachiIP}\nPort : {port}";
                }
                else
                {
                    message = $"공인 IP : {publicIP}Port : {port}";
                }
            }
            else
            {
                message = $"연결 IP : {ip}\n연결 Port : {port}";
            }

            uiSupport.MultiConnectInfo(message);
            StartCoroutine(LoadWorldScene());
        }

        private IEnumerator LoadWorldScene()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            yield return new WaitForSeconds(5f);

            AsyncOperation op = SceneManager.LoadSceneAsync("WorldScene_Multi", LoadSceneMode.Single);
            op.allowSceneActivation = false;

            while (op.progress < 0.9f)
            {
                float progress = Mathf.Clamp01(op.progress / 0.9f);
                uiSupport.LoadingProgress((progress * 100).ToString("F0"));

                yield return null;
            }
            uiSupport.LoadingProgress("100");

            yield return new WaitForSeconds(1f);

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