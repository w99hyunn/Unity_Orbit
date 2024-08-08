using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashSceneManager : MonoBehaviour
{
    public GameObject warningCanvas;
    public string loadScene;
    public float endDelay = 6.0f;

    private AsyncOperation asyncLoad;

    private void Start()
    {
        warningCanvas.SetActive(false);

        Cursor.visible = false;
        StartCoroutine(LoadSceneAsync());
        StartCoroutine(SwitchCanvasAndLoadScene());
    }

    /* 비동기 씬 로딩 */
    private IEnumerator LoadSceneAsync()
    {
        asyncLoad = SceneManager.LoadSceneAsync(loadScene);
        asyncLoad.allowSceneActivation = false; 
        yield return asyncLoad;
    }

    private IEnumerator SwitchCanvasAndLoadScene()
    {
        yield return new WaitForSeconds(5);

        warningCanvas.SetActive(true);

        yield return new WaitForSeconds(endDelay);

        Cursor.visible = true;
        asyncLoad.allowSceneActivation = true;
    }
}
