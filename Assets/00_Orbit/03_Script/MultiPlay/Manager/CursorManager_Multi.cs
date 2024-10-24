using System.Collections;
using UnityEngine;

namespace STARTING
{
    public class CursorManager_Multi : CursorManager
    {
        public PlayerStats_Multi playerStats;

        private void Start()
        {
            StartCoroutine(FindLocalPlayer());
            //objectsToDestroy.Add(GameManager_Multi.Instance.gameObject);
        }

        private IEnumerator FindLocalPlayer()
        {
            while (GameManager_Multi.Instance.playerStats == null)
            {
                yield return null; 
            }
            playerStats = GameManager_Multi.Instance.playerStats;
            //Debug.Log(this.gameObject.name + playerStats);
            if (playerStats != null)
            {
                ContinueGame();
            }
            else
            {
                Debug.LogWarning("PlayerStats_Multi วาด็ X");
            }
        }

        public override void BackToMain()
        {
            CustomNetworkManager.singleton.BackToMain();
            DBManager.Instance.CloseDBServer();

            base.BackToMain();
        }

        public override void OpenPauseMenu()
        {
            playerStats.playerState = PlayerState_Multi.PAUSE;
        }

        public override void ClosePauseMenu()
        {
            playerStats.playerState = PlayerState_Multi.IDLE;
        }
    }
}