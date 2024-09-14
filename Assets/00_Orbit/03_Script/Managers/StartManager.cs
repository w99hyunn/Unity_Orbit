using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/*
 * MainScene에서 게임 시작 과정을 위한 코드
 * 시작과 동시에 세이브파일 존재 유무를 파악하여 정보 로드
 */

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
            lastModifiedText.text = fileInfo.LastWriteTime.ToString("G") + "에 저장된 세이브파일입니다.";
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
            levelText.text = data.level.ToString() + "레벨 (" + ((int)((float)data.currentExperience / data.maxExperience * 100)).ToString() + "%)";
            hpText.text = ((int)((float)data.currentHealth / data.maxHealth * 100)).ToString() + "%";
            mpText.text = ((int)((float)data.currentMana / data.maxMana * 100)).ToString() + "%";
        }
    }


    private IEnumerator LoadWorldScene()
    {
        // 씬 로딩을 위한 AsyncOperation 객체 생성
        AsyncOperation op = SceneManager.LoadSceneAsync("WorldScene", LoadSceneMode.Single);
        op.allowSceneActivation = false;



        // 모든 씬이 0.9f 이상 로드될 때까지 대기
        yield return new WaitUntil(() => op.progress >= 0.9f);
        Debug.Log("모든 씬 로딩 완료");

        SceneManager.LoadScene("Element_UI", LoadSceneMode.Additive);
        op.allowSceneActivation = true;

        GameManager.Instance.LoadGame();

    }

    public void StartNewGame()
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, "gameData.json");
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }

        PlayerPrefs.DeleteKey("ContinueGame");

        StartCoroutine(LoadWorldScene());
    }

    public void ContinueGame()
    {
        StartCoroutine(LoadWorldScene());
    }

    public void DevSite(string url)
    {
        Application.OpenURL(url);
    }
}
