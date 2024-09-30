using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace STARTING
{
    public class CursorManager_Multi : NetworkBehaviour
    {
        public List<GameObject> objectsToDestroy = new List<GameObject>();
        public GameObject pauseMenuHotkey;

        private PlayerStats_Multi playerStats;

        private void Start()
        {
            StartCoroutine(FindLocalPlayer());
        }

        private IEnumerator FindLocalPlayer()
        {
            while (NetworkClient.localPlayer == null)
            {
                yield return null; 
            }
            playerStats = NetworkClient.localPlayer.GetComponent<PlayerStats_Multi>();
            if (playerStats != null)
            {
                ContinueGame();
            }
            else
            {
                Debug.LogWarning("PlayerStats_Multi วาด็ X");
            }
        }

        public void BackToMain()
        {
            if (NetworkClient.localPlayer)
            {
                CustomNetworkManager.singleton.BackToMain();
                DBManager.Instance.CloseDBServer();
                SceneManager.LoadScene("MainScene");
                DestroyObjectsInList();
            }
        }

        public void DestroyObjectsInList()
        {
            foreach (GameObject obj in objectsToDestroy)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            objectsToDestroy.Clear();
        }
        public void DieGame()
        {
            if (NetworkClient.localPlayer)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                pauseMenuHotkey.SetActive(false);
            }
        }

        public void ContinueGame()
        {
            if (NetworkClient.localPlayer)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                pauseMenuHotkey.SetActive(true);
            }
        }

        public void CustomResume()
        {
            if (NetworkClient.localPlayer)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        public void CustomPause()
        {
            if (NetworkClient.localPlayer)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        public void OpenPauseMenu()
        {
            if (NetworkClient.localPlayer)
            {
                playerStats.playerState = PlayerState_Multi.PAUSE;
            }
        }

        public void ClosePauseMenu()
        {
            if (NetworkClient.localPlayer)
            {
                playerStats.playerState = PlayerState_Multi.IDLE;
            }
        }
    }
}