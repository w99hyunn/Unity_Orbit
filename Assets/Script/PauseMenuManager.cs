using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject gameObject1;
    public GameObject gameObject2;

    private void OnEnable()
    {
        // 씬 로드 이벤트에 콜백 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 씬 로드 이벤트에서 콜백 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 이름에 따라 GameObject 활성화/비활성화
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
        gameObject1.SetActive(isActive);
        gameObject2.SetActive(isActive);
    }
}
