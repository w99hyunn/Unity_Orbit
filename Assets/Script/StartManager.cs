using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public void AsyncLoadScene(string name)
    {
        StartCoroutine(LoadSceneProcess(name));
    }

    IEnumerator LoadSceneProcess(string name)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(name);
        op.allowSceneActivation = true; //로딩 중 씬 로드가 끝났을 때 바로 넘어갈 것인가

        if (op.isDone)
        {
            yield break;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void DevSite(string url)
    {
        Application.OpenURL(url);
    }

}
