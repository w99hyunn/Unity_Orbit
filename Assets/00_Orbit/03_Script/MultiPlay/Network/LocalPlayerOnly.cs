using Mirror;
using UnityEngine;

namespace STARTING
{
    public class LocalPlayerOnly : NetworkBehaviour
    {
        void Start()
        {
            Debug.Log("dd / " + isLocalPlayer + " / " + this.gameObject.name);
            if (!isLocalPlayer)
                Destroy(gameObject);
        }
    }
}
