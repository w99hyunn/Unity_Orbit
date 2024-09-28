using Mirror;
using UnityEngine;

namespace STARTING
{
    public class PlayerLocalCamera : NetworkBehaviour
    {
        private Camera playerCamera;

        void Start()
        {
            playerCamera = GetComponent<Camera>();
            gameObject.SetActive(isLocalPlayer);
        }
    }
}
