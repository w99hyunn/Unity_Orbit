using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace STARTING
{
    public class CursorManager : MonoBehaviour
    {
        public List<GameObject> objectsToDestroy = new List<GameObject>();
        public GameObject pauseMenuHotkey;

        private void Start()
        {
            ContinueGame();
            objectsToDestroy.Add(GameManager.Instance.gameObject);
            objectsToDestroy.Add(PlayerStats.Instance.gameObject);
        }

        public void BackToMain()
        {
            SceneManager.LoadScene("MainScene");
            DestroyObjectsInList();
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
            PlayerStats.Instance.playerState = PlayerState.PAUSE;
        }

        public void ClosePauseMenu()
        {
            PlayerStats.Instance.playerState = PlayerState.IDLE;
        }
    }
}