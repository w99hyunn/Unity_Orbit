using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace STARTING
{
    /// <summary>
    /// MainScene에서 싱글플레이 게임 시작 과정을 위한 코드
    /// 시작과 동시에 세이브파일 존재 유무를 파악하여 정보 로드
    /// </summary>
    public class StartSingleplay : MonoBehaviour
    {
        public MainUISupport uiSupport;
        public List<GameObject> objectsToDestroy = new List<GameObject>();

        private void Start()
        {
            CheckForSavedGame();
            objectsToDestroy.Add(DBManager.Instance.gameObject);
            objectsToDestroy.Add(CustomNetworkManager.singleton.gameObject);
        }

        public void DestroyObjectsInList()
        {
            foreach (GameObject obj in objectsToDestroy)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            objectsToDestroy.Clear();
        }

        private void CheckForSavedGame()
        {
            string saveFilePath = Path.Combine(Application.persistentDataPath, "gameData.json");
            if (File.Exists(saveFilePath))
            {
                uiSupport.ContinueGame(true);

                LoadGameData(saveFilePath);

                FileInfo fileInfo = new FileInfo(saveFilePath);
                uiSupport.LastSaveDate(fileInfo.LastWriteTime.ToString("G") + "에 저장된 세이브파일입니다.");
            }
            else
            {
                uiSupport.ContinueGame(false);
                uiSupport.ShowMultiplay(false);
            }
        }

        /// <summary>
        /// 데이터에 저장된 게임 시간 계산
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        private string TimeCalc(float gameTime)
        {
            int hours = (int)(gameTime / 3600) % 24;
            int minutes = (int)(gameTime % 3600 / 60);

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

            return timeFormatted;
        }

        private void LoadGameData(string path)
        {
            string encryptedJson = File.ReadAllText(path);
            string json = CryptoUtility.DecryptString(encryptedJson); // 복호화
            GameData data = JsonUtility.FromJson<GameData>(json);

            if (data != null)
            {
                uiSupport.SaveFileInfo(TimeCalc(data.gameTime),
                    data.level.ToString() + "레벨 (" + ((int)((float)data.currentExperience / data.maxExperience * 100)).ToString() + "%)",
                    ((int)((float)data.currentHealth / data.maxHealth * 100)).ToString() + "%",
                    ((int)((float)data.currentMana / data.maxMana * 100)).ToString() + "%",
                    ((int)((float)data.chip)).ToString() + "개",
                    data.zones.Count(zone => zone.isLiberated) + "개");

                //최초 1회 레벨 3 이상 달성시 PlayerPrefs에 멀티플레이 가능 여부를 저장하여 싱글플레이 데이터 삭제시에도 유지
                if ((false == PlayerPrefs.HasKey("MultiplayAvailable")) && (data.level >= 3))
                {
                    PlayerPrefs.SetInt("MultiplayAvailable", 1);
                    PlayerPrefs.Save();
                }

                if (true == PlayerPrefs.HasKey("MultiplayAvailable"))
                {
                    bool isMultiplayAvailable = PlayerPrefs.GetInt("MultiplayAvailable") == 1;

                    if (true == isMultiplayAvailable)
                    {
                        uiSupport.ShowMultiplay(true);
                    }
                    else
                    {
                        uiSupport.ShowMultiplay(false);
                    }
                }
                else
                {
                    uiSupport.ShowMultiplay(false);
                }
            }
        }

        private IEnumerator LoadWorldScene()
        {
            DestroyObjectsInList();
            yield return new WaitForSeconds(2f);

            AsyncOperation op = SceneManager.LoadSceneAsync("WorldScene", LoadSceneMode.Single);
            op.allowSceneActivation = false;

            while (op.progress < 0.9f)
            {
                float progress = Mathf.Clamp01(op.progress / 0.9f);
                uiSupport.LoadingProgress((progress * 100).ToString("F0"));

                yield return null;
            }
            uiSupport.LoadingProgress("100");

            yield return new WaitForSeconds(1f); // 1초 대기

            op.allowSceneActivation = true;
            while (!op.isDone)
            {
                yield return null;
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByName("WorldScene"));
        }

        public void StartNewGame()
        {
            string saveFilePath = Path.Combine(Application.persistentDataPath, "gameData.json");
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
            }
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
}