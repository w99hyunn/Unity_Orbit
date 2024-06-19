using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public GameObject continueButton;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        CheckForSavedGame();
    }

    private void CheckForSavedGame()
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, "gameData.json");
        if (File.Exists(saveFilePath))
        {
            continueButton.SetActive(true);
        }
        else
        {
            continueButton.SetActive(false);
        }
    }

    public void AsyncLoadScene(string name)
    {
        StartCoroutine(LoadSceneProcess(name));
    }

    private IEnumerator LoadSceneProcess(string name)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(name);
        op.allowSceneActivation = true;

        yield return new WaitUntil(() => op.isDone);

        if (name == "OutdoorsScene")
        {
            GameManager.Instance.LoadGame();
        }
    }

    public void StartNewGame()
    {
        // Delete the existing save file if it exists
        string saveFilePath = Path.Combine(Application.persistentDataPath, "gameData.json");
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }

        PlayerPrefs.DeleteKey("ContinueGame");

        // Load the OutdoorsScene
        AsyncLoadScene("OutdoorsScene");
    }

    public void ContinueGame()
    {
        // Load the OutdoorsScene and load the game data
        AsyncLoadScene("OutdoorsScene");
    }

    public void DevSite(string url)
    {
        Application.OpenURL(url);
    }
}
