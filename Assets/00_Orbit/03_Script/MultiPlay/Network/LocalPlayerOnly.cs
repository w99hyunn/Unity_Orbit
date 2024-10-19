using Mirror;
using UnityEngine;

namespace STARTING
{
    public class LocalPlayerOnly : NetworkBehaviour
    {
        void Start()
        {
            this.gameObject.SetActive(isLocalPlayer);
        }
    }
}
