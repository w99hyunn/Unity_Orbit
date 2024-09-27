using Mirror;
using TMPro;
using UnityEngine;

namespace STARTING
{
    public class StartMultiplay : MonoBehaviour
    {
        [Header("멀티플레이")]
        [Header("호스트 오픈")]
        public TMP_InputField hostIpInput;
        public TMP_InputField hostPortInput;

        [Header("클라이언트 접속")]
        public TMP_InputField clientIpInput;
        public TMP_InputField clientPortInput;


        /// <summary>
        /// 멀티플레이 - 서버 오픈
        /// </summary>
        public void ServerOpen()
        {
            string ip = hostIpInput.text;
            string portText = hostPortInput.text;

            CustomNetworkManager.singleton.networkAddress = ip;
            if (Transport.active is PortTransport portTransport)
            {
                if (ushort.TryParse(portText, out ushort port))
                    portTransport.Port = port;
            }

            CustomNetworkManager.singleton.StartHost();
        }

        /// <summary>
        /// 멀티플레이 - 클라이언트 접속
        /// </summary>
        public void ClientConnect()
        {
            string ip = clientIpInput.text;
            string portText = clientPortInput.text;

            CustomNetworkManager.singleton.networkAddress = ip;
            if (Transport.active is PortTransport portTransport)
            {
                if (ushort.TryParse(portText, out ushort port))
                    portTransport.Port = port;
            }

            CustomNetworkManager.singleton.StartClient();
        }
    }
}