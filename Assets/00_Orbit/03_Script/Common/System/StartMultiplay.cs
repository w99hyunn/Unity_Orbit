using Mirror;
using System.Collections;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace NOLDA
{
    enum StartType
    {
        NONE,
        HOST,
        CLIENT
    };

    public class StartMultiplay : MonoBehaviour
    {
        public MainUISupport uiSupport;
        private ClientNetworkHandler clientNetworkHandler;

        private string publicIP;
        private string hamachiIP;
        private ushort port;
        private string ip;
        private StartType startType;

        private void Start()
        {
            clientNetworkHandler = FindAnyObjectByType<ClientNetworkHandler>();
            StartCoroutine(GetPublicIP());
            hamachiIP = GetHamachiIP();
        }

        public void BackToMain()
        {
            CustomNetworkManager.singleton.BackToMain();
            DBManager.Instance.CloseDBServer();
        }

        IEnumerator GetPublicIP()
        {
            UnityWebRequest request = UnityWebRequest.Get("https://api.ip.pe.kr");
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
        /// 멀티플레이 - 서버 오픈
        /// </summary>
        public void ServerOpen()
        {
            bool success = DBManager.Instance.ConnectDB();

            if (true == success)
            {
                ip = "localhost";
                port = GetAvailablePort(1024, 65535);

                CustomNetworkManager.singleton.networkAddress = ip;
                if (Transport.active is PortTransport portTransport)
                {
                    portTransport.Port = port;
                }

                CustomNetworkManager.singleton.StartHost();
                StartCoroutine(CheckOpen());
            }
            else if (false == success)
            {
                uiSupport.DBConnectFailEvent();
            }
        }

        private IEnumerator CheckOpen()
        {
            while (true == NetworkServer.active)
            {
                if (true == NetworkServer.active) // 서버에 연결된 상태
                {
                    uiSupport.LoginEvent();
                    startType = StartType.HOST;
                    yield break;
                }
                yield return null;
            }
            uiSupport.ConnectFailEvent();
            yield break;
        }

        /// <summary>
        /// 멀티플레이 - 클라이언트 접속
        /// </summary>
        public void ClientConnect()
        {
            ip = uiSupport.GetClientIP();
            string portText = uiSupport.GetClientPort();

            CustomNetworkManager.singleton.networkAddress = ip;
            if (Transport.active is PortTransport portTransport)
            {
                if (ushort.TryParse(portText, out ushort port))
                {
                    portTransport.Port = port;
                    this.port = port;
                }
            }

            CustomNetworkManager.singleton.StartClient();
            StartCoroutine(CheckConnection(port, ip));
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
                    uiSupport.LoginClientEvent();
                    startType = StartType.CLIENT;
                    yield break;
                }
                yield return null;
            }
            uiSupport.ConnectFailEvent();
            yield break;
        }

        public void Login()
        {
            string username = uiSupport.GetLoginID();
            string password = uiSupport.GetLoginPW();

            clientNetworkHandler.SendLoginRequest(username, password);
        }

        public void Register()
        {
            string username = uiSupport.GetRegisterID();
            string password = uiSupport.GetRegisterPW();


            clientNetworkHandler.SendRegisterRequest(username, password);
        }

        public void GameStart()
        {
            uiSupport.StartLoadingEvent();

            if (startType == StartType.HOST)
            {
                if (hamachiIP != null)
                {
                    uiSupport.MultiConnectInfo($"{hamachiIP} : {port}에 접속중입니다.");
                }
                else
                {
                    uiSupport.MultiConnectInfo($"{ip} : {port}에 접속중입니다.\n외부에서 접속하려면 고정IP또는 하마치IP로 접속하세요.");
                }
            }
            else if (startType == StartType.CLIENT)
            {
                uiSupport.MultiConnectInfo($"{ip} : {port}에 접속중입니다.");
            }

            StartCoroutine(LoadWorldScene());
        }

        private IEnumerator LoadWorldScene()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            yield return new WaitForSeconds(2f);

            AsyncOperation op = SceneManager.LoadSceneAsync(SceneDataManager.GetSceneName("Multi"), LoadSceneMode.Single);
            op.allowSceneActivation = false;

            while (op.progress < 0.9f)
            {
                float progress = Mathf.Clamp01(op.progress / 0.9f);
                uiSupport.LoadingProgress((progress * 100).ToString("F0"));

                yield return null;
            }
            uiSupport.LoadingProgress("100");

            yield return new WaitForSeconds(1f);

            op.allowSceneActivation = true;

            while (!op.isDone)
            {
                yield return null;
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneDataManager.GetSceneName("Multi")));
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == SceneDataManager.GetSceneName("Multi"))
            {
                if (NetworkClient.isConnected)
                {
                    NetworkClient.AddPlayer();
                    ChatSupport.Instance?.AddChatMessage("멀티플레이", $"{ip} : {port}에 접속되었습니다.");
                }
                else
                {
                    Debug.LogError("NetworkClient가 연결 x");
                }

                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }
    }
}