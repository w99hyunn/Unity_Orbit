using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace STARTING
{
    public class CursorManager_Multi : MonoBehaviour
    {
        public List<GameObject> objectsToDestroy = new List<GameObject>();
        public GameObject pauseMenuHotkey;

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
                Debug.Log("찾는중");
                yield return null; 
            }
            playerStats = GameManager_Multi.Instance.playerStats;
            Debug.Log(playerStats);
            if (playerStats != null)
            {
                ContinueGame();
            }
            else
            {
                Debug.LogWarning("PlayerStats_Multi 할당 X");
            }
        }

        public void BackToMain()
        {
            CustomNetworkManager.singleton.BackToMain();
            DBManager.Instance.CloseDBServer();
            DestroyObjectsInList();
            SceneManager.LoadScene("MainScene");
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

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                pauseMenuHotkey.SetActive(false);

        }

        public void ContinueGame()
        {

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                pauseMenuHotkey.SetActive(true);

        }

        public void CustomResume()
        {

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

        }

        public void CustomPause()
        {

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

        }

        public void OpenPauseMenu()
        {

                playerStats.playerState = PlayerState_Multi.PAUSE;

        }

        public void ClosePauseMenu()
        {

                playerStats.playerState = PlayerState_Multi.IDLE;

        }
    }
}