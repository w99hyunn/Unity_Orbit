using Mirror;
using UnityEngine;

namespace NOLDA
{
    public class LocalPlayerOnly : NetworkBehaviour
    {
        void Start()
        {
            this.gameObject.SetActive(isLocalPlayer);
        }
    }
}
