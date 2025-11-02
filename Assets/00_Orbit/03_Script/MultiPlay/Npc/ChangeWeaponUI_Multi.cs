using UnityEngine;
using Demo.Scripts.Runtime.Character;
using Mirror;
using System.Collections;

namespace STARTING
{
    public class ChangeWeaponUI_Multi : ChangeWeaponUI
    {
        private GameObject player;
        private Inventory inventory;
        private FPSController_Multi fpsController;

        public override void Awake()
        {
            StartCoroutine(FindLocalPlayer());
        }

        private void Start()
        {
            if (NetworkClient.localPlayer != null)
            {
                UpdateWeaponLists();
            }
        }

        private IEnumerator FindLocalPlayer()
        {
            while (NetworkClient.localPlayer == null)
            {
                yield return null;
            }

            player = NetworkClient.localPlayer.gameObject;

            Debug.Log("체인지웨폰 UI 초기화");

            inventory = player.GetComponent<Inventory>();
            fpsController = player.GetComponent<FPSController_Multi>();

            UpdateWeaponLists();
        }
    }
}