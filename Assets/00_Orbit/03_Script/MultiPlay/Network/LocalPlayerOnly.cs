using Mirror;
using UnityEngine;

namespace STARTING
{
    public class LocalPlayerOnly : NetworkBehaviour
    {
        void Start()
        {
            Debug.Log("dd / " + isLocalPlayer + " / " + this.gameObject.name);
            this.gameObject.SetActive(isLocalPlayer);
        }
    }
}
