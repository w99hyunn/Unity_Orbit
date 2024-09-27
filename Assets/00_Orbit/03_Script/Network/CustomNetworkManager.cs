using Mirror;
using UnityEngine;

namespace STARTING
{
    public class CustomNetworkManager : NetworkManager
    {
        public static new CustomNetworkManager singleton;

        public override void Awake()
        {
            base.Awake();

            if (singleton == null)
            {
                singleton = this;
            }
            else if (singleton != this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }
    }
}