using Mirror;
using UnityEngine.InputSystem;

namespace STARTING
{
    public class NetworkPlayerManager : NetworkBehaviour
    {
        public Health_Multi health;

        [SyncVar(hook = nameof(OnNameChanged))]
        public string playerName;

        void Start()
        {
            //멀티플레이 세팅
            if (!isLocalPlayer)
            {
                GetComponent<PlayerInput>().enabled = false;

                //GetComponent<UserInputController>().enabled = false;
                //GetComponent<FPSMovement_Multi>().enabled = false;
                //GetComponent<FPSPlayablesController>().enabled = false;
                //GetComponent<FPSController_Multi>().enabled = false;
                //GetComponent<CharacterController>().enabled = false;
                //GetComponent<FPSBoneController>().enabled = false;
                //GetComponent<FPSAnimator>().enabled = false;
            }

            if (isLocalPlayer)
            {
                CmdSetPlayerName(DBManager.Instance.userName);
            }
        }

        [Command]
        void CmdSetPlayerName(string name)
        {
            playerName = name;
        }

        void OnNameChanged(string oldName, string newName)
        {
            gameObject.name = newName;
            health.SetName(newName);
        }
    }
}