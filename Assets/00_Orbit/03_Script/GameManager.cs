using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class ZoneData
{
    public string zoneName;
    public bool isLiberated;
}

[System.Serializable]
public class GameData
{
    public float gameTime;
    public int currentHealth;
    public int currentMana;
    public int currentExperience;
    public int level;
    public Vector3 playerPosition;
    public List<ZoneData> zones;
}

public class GameManager : MonoBehaviour
{
    // 인스턴스 던전관련
    public List<ZoneData> zones; //디버그용 public
    public string currentZoneName;
    public Vector3 lastPlayerPosition;

    [Header("설정된 시간마다 자동저장(초 단위)")]
    public float interval = 180f;
    public bool isGameOver = false;

    private AudioSource audioSource;
    private string saveFilePath;
    private bool isPlayerStatsInitialized = false;
    private bool isPlayerControllerInitialized = false;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        saveFilePath = Path.Combine(Application.persistentDataPath, "gameData.json");
    }

    private void Start()
    {
        StartCoroutine(AutoClick());
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        PlayerStats.OnPlayerStatsInitialized += OnPlayerStatsInitialized;
        //0803 PlayerController.OnPlayerControllerInitialized += OnPlayerControllerInitialized;
    }

    private void OnDisable()
    {
        PlayerStats.OnPlayerStatsInitialized -= OnPlayerStatsInitialized;
        //0803 PlayerController.OnPlayerControllerInitialized -= OnPlayerControllerInitialized;
    }

    public void SavePlayerPosition(Vector3 position)
    {
        lastPlayerPosition = position;
    }

    public Vector3 LoadPlayerPosition()
    {
        return lastPlayerPosition;
    }
    private void OnPlayerStatsInitialized()
    {
        isPlayerStatsInitialized = true;
        TryLoadGame();
    }

    private void OnPlayerControllerInitialized()
    {
        isPlayerControllerInitialized = true;
        TryLoadGame();
    }

    private void TryLoadGame()
    {
        if (isPlayerStatsInitialized && isPlayerControllerInitialized)
        {
            LoadGame();
            SaveGame();
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        UIManager.Instance.GameOverUI();
        CursorManager.Instance.CustomPause();
    }

    // 게임 오버시 체크포인트 불러오기
    public void ContinueGame()
    {
        isGameOver = false;
        LoadGame();
    }

    public void SaveGame()
    {
        GameData data = new GameData
        {
            gameTime = UIManager.Instance.GameTime,
            currentHealth = PlayerStats.Instance.currentHealth,
            currentMana = PlayerStats.Instance.currentMana,
            currentExperience = PlayerStats.Instance.currentExperience,
            level = PlayerStats.Instance.level,
            //0803 playerPosition = PlayerController.Instance.transform.position,
            zones = zones
        };

        string json = JsonUtility.ToJson(data);
        string encryptedJson = CryptoUtility.EncryptString(json); // 암호화

        File.WriteAllText(saveFilePath, encryptedJson);
        PlayerPrefs.SetInt("ContinueGame", 1);
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string encryptedJson = File.ReadAllText(saveFilePath);
            string json = CryptoUtility.DecryptString(encryptedJson); // 복호화

            //Debug.Log("Loaded JSON: " + json);

            GameData data = JsonUtility.FromJson<GameData>(json);
            if (data != null)
            {
                UIManager.Instance.GameTime = data.gameTime;
                PlayerStats.Instance.SetStats(data.currentHealth, data.currentMana, data.currentExperience, data.level);
                //0803  PlayerController.Instance.SetPos(data.playerPosition);
                zones = data.zones;
            }
        }
    }

    public void LiberateZone(string zoneName)
    {
        foreach (ZoneData zone in zones)
        {
            if (zone.zoneName == zoneName)
            {
                zone.isLiberated = true;
                return;
            }
        }
        zones.Add(new ZoneData { zoneName = zoneName, isLiberated = true });
    }

    public bool IsZoneLiberated(string zoneName)
    {
        foreach (ZoneData zone in zones)
        {
            if (zone.zoneName == zoneName)
            {
                return zone.isLiberated;
            }
        }
        return false;
    }

    public void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    /* Auto Save Manager */
    public Button SaveButton; // PauseMenu > Save 버튼 클릭 이벤트 발생

    private IEnumerator AutoClick()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            if (SceneManager.GetActiveScene().name == "OutdoorsScene" && isGameOver == false)
            {
                SaveButton.onClick.Invoke();
            }
            else
            {
                Debug.Log("자동저장 요건 충족 X");
            }
        }
    }
}
