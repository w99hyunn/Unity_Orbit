using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace STARTING
{
    public class SplashSceneManager : MonoBehaviour
    {
        public GameObject canvasA;
        public GameObject canvasB;
        public GameObject canvasC;
        public GameObject canvas_Loading;
        public string nextSceneName;
        public float delayBetweenCanvases = 6.0f;

        private AsyncOperation _asyncLoad;

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
            _asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
            _asyncLoad.allowSceneActivation = false;
            yield return _asyncLoad;
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
            _asyncLoad.allowSceneActivation = true;
        }

        //스플래시 스킵
        private void OnSkip(InputValue value)
        {
            Cursor.visible = true;
            SceneManager.LoadScene("MainScene");
        }
    }
}