using TMPro;
using UnityEngine;

namespace NOLDA
{
    public class MultiUI : MonoBehaviour
    {
        public TMP_Text connectIP;
        public TMP_Text connectPort;
        public TMP_Text loginID;

        void Start()
        {
            connectIP.text = CustomNetworkManager.singleton.networkAddress;
            connectPort.text = CustomNetworkManager.singleton.transport.ToString();
            loginID.text = DBManager.Instance.userName;
        }
    }
}