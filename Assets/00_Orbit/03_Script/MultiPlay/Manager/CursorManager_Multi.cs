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

        private void Awake()
        {
            if (isLocalPlayer)
            {
                playerStats = FindAnyObjectByType<PlayerStats_Multi>();
            }
        }

        private void Start()
        {
            if (isLocalPlayer)  // 로컬 플레이어일 때만 동작
            {
                ContinueGame();
                objectsToDestroy.Add(GameManager.Instance.gameObject);
            }
        }

        private IEnumerator CheckPlayerState()
        {
            while (isLocalPlayer)
            {
                if (playerStats.playerState == PlayerState_Multi.IDLE)
                {
                    pauseMenuHotkey.SetActive(true);
                    yield break;
                }
                yield return new WaitForSeconds(0.5f);
            }
        }

        public void BackToMain()
        {
            if (isLocalPlayer)
            {
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
            if (isLocalPlayer)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                pauseMenuHotkey.SetActive(false);
            }
        }

        public void ContinueGame()
        {
            if (isLocalPlayer)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                pauseMenuHotkey.SetActive(true);
            }
        }

        public void CustomResume()
        {
            if (isLocalPlayer)
            {
                playerStats.playerState = PlayerState_Multi.IDLE;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        public void CustomPause()
        {
            if (isLocalPlayer)
            {
                playerStats.playerState = PlayerState_Multi.PAUSE;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
}