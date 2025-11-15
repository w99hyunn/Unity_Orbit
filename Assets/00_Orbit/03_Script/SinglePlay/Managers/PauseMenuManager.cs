using UnityEngine;
using UnityEngine.SceneManagement;

namespace NOLDA
{
    /// <summary>
    /// 던전 씬에서 비활성화할 메뉴 버튼들
    /// </summary>
    public class PauseMenuManager : MonoBehaviour
    {
        public GameObject[] hideBtns;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == SceneDataManager.GetSceneName("SingleDungeon"))
            {
                SetGameObjectsActive(false);
            }
            else if (scene.name == SceneDataManager.GetSceneName("Single"))
            {
                SetGameObjectsActive(true);
            }
        }

        private void SetGameObjectsActive(bool isActive)
        {
            for (int i = 0; i < hideBtns.Length; i++)
            {
                GameObject btn = hideBtns[i];
                btn.SetActive(isActive);
            }
        }
    }
}