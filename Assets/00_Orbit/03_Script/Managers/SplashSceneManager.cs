using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashSceneManager : MonoBehaviour
{
    public GameObject canvasA;
    public GameObject canvasB;
    public GameObject canvasC;
    public GameObject canvas_Loading;
    public string nextSceneName;
    public float delayBetweenCanvases = 6.0f;

    private AsyncOperation asyncLoad;

    private void Start()
    {
        //canvasA.SetActive(true);
        canvasB.SetActive(false);
        canvasC.SetActive(false);
        canvas_Loading.SetActive(false);

        Cursor.visible = false;
        StartCoroutine(LoadSceneAsync());

        StartCoroutine(SwitchCanvasAndLoadScene());
    }

    private IEnumerator LoadSceneAsync()
    {
        asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
        asyncLoad.allowSceneActivation = false;
        yield return asyncLoad;
    }

    private IEnumerator SwitchCanvasAndLoadScene()
    {
        yield return new WaitForSeconds(5);

        //canvasA.SetActive(false);
        canvasB.SetActive(true);
        canvas_Loading.SetActive(true);

        yield return new WaitForSeconds(delayBetweenCanvases);
        canvasB.SetActive(false);
        canvasC.SetActive(true);

        yield return new WaitForSeconds(delayBetweenCanvases);

        Cursor.visible = true;
        asyncLoad.allowSceneActivation = true;
    }
}