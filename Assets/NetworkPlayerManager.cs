using Demo.Scripts.Runtime.Character;
using KINEMATION.FPSAnimationFramework.Runtime.Playables;
using KINEMATION.KAnimationCore.Runtime.Input;
using Mirror;
using UnityEngine.InputSystem;

namespace STARTING
{
    public class NetworkPlayerManager : NetworkBehaviour
    {
        private PlayerInput playerInput;
        private FPSPlayablesController fpsPlayablesController;
        private UserInputController userInputController;
        private FPSController_Multi fPSController;
        private FPSMovement_Multi fPSMovement;
        public Health_Multi health;

        [SyncVar(hook = nameof(OnNameChanged))]
        public string playerName; // 닉네임을 동기화할 변수

        void Start()
        {
            if (isLocalPlayer)
            {
                string nickname = DBManager.Instance.userName;
                CmdSetPlayerName(nickname);
            }

            //멀티플레이 세팅
            if (!isLocalPlayer)
            {
                playerInput = GetComponent<PlayerInput>();
                playerInput.enabled = false;
                fpsPlayablesController = GetComponent<FPSPlayablesController>();
                fpsPlayablesController.enabled = false;

                userInputController = GetComponent<UserInputController>();
                userInputController.enabled = false;

                fPSController = GetComponent<FPSController_Multi>();
                fPSController.enabled = false;

                fPSMovement = GetComponent<FPSMovement_Multi>();
                fPSMovement.enabled = false;
                return;
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