using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class StartManager : MonoBehaviour
{
    public GameObject continueButton;
    public GameObject continueDescription;
    public GameObject newDescription;
    public TMP_Text gameTimeText;
    public TMP_Text levelText;
    public TMP_Text hpText;
    public TMP_Text mpText;
    public TMP_Text lastModifiedText;

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
            continueDescription.SetActive(true);
            newDescription.SetActive(false);

            LoadGameData(saveFilePath);

            FileInfo fileInfo = new FileInfo(saveFilePath);
            lastModifiedText.text = fileInfo.LastWriteTime.ToString("G");
        }
        else
        {
            continueButton.SetActive(false);
            continueDescription.SetActive(false);
            newDescription.SetActive(true);
        }
    }

    private void LoadGameData(string path)
    {
        string encryptedJson = File.ReadAllText(path);
        string json = CryptoUtility.DecryptString(encryptedJson); // 복호화
        GameData data = JsonUtility.FromJson<GameData>(json);

        if (data != null)
        {
        
            int hours = (int)(data.gameTime / 3600) % 24;
            int minutes = (int)(data.gameTime % 3600 / 60);

            string period = hours >= 12 ? "오후" : "오전";
            hours = hours % 12;

            if (period == "오전" && hours == 0)
            {
                hours = 0;
            }
            else if (period == "오후" && hours == 0)
            {
                hours = 12;
            }
            else if (hours == 0)
            {
                hours = 12;
            }

            string timeFormatted = string.Format("{0} {1:D2}:{2:D2}", period, hours, minutes);

            gameTimeText.text = timeFormatted;
            levelText.text = data.level.ToString() + "레벨 (" + data.currentExperience.ToString() + "%)";
            hpText.text = data.currentHealth.ToString() + "%";
            mpText.text = data.currentMana.ToString() + "%";
        }
        else
        {
            Debug.LogError("Failed to load game data.");
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
