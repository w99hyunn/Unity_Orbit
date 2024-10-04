using Mirror;
using STARTING;
using TMPro;
using UnityEngine;

public class MultiUI : NetworkBehaviour
{
    public TMP_Text connectIP;
    public TMP_Text connectPort;
    public TMP_Text loginID;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        connectIP.text = CustomNetworkManager.singleton.networkAddress;
        connectPort.text = CustomNetworkManager.singleton.transport.ToString();
        loginID.text = DBManager.Instance.userName;
    }

}
