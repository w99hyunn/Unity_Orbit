using UnityEngine;
using UnityEngine.SceneManagement;

/* 
 * 던전 씬에서 비활성화 하고 싶은 PauseMenu 버튼들
 */

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
        if (scene.name == "DungeonScene")
        {
            SetGameObjectsActive(false);
        }
        else if (scene.name == "OutdoorsScene")
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
