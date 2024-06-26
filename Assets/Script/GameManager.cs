using Orbit_Character;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private string saveFilePath;
    private bool isPlayerStatsInitialized = false;
    private bool isPlayerControllerInitialized = false;

    // 인스턴스 던전관련
    public List<ZoneData> zones;
    public string currentZoneName;
    public Vector3 lastPlayerPosition;

    public void SavePlayerPosition(Vector3 position)
    {
        lastPlayerPosition = position;
    }

    public Vector3 LoadPlayerPosition()
    {
        return lastPlayerPosition;
    }

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

    private void OnEnable()
    {
        PlayerStats.OnPlayerStatsInitialized += OnPlayerStatsInitialized;
        PlayerController.OnPlayerControllerInitialized += OnPlayerControllerInitialized;
    }

    private void OnDisable()
    {
        PlayerStats.OnPlayerStatsInitialized -= OnPlayerStatsInitialized;
        PlayerController.OnPlayerControllerInitialized -= OnPlayerControllerInitialized;
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
        }
    }

    public void GameOver()
    {
        SaveGame();
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
            playerPosition = PlayerController.Instance.transform.position,
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

            Debug.Log("Loaded JSON: " + json); // 데이터가 제대로 읽혔는지 로그 확인

            GameData data = JsonUtility.FromJson<GameData>(json);

            if (data != null)
            {
                UIManager.Instance.GameTime = data.gameTime;
                PlayerStats.Instance.SetStats(data.currentHealth, data.currentMana, data.currentExperience, data.level);
                PlayerController.Instance.SetPos(data.playerPosition);
                zones = data.zones;

                Debug.Log("Game loaded successfully.");
            }
            else
            {
                Debug.LogError("Failed to parse JSON data.");
            }
        }
        else
        {
            Debug.LogError("Save file does not exist!");
        }
    }

    public void NewGame()
    {
        UIManager.Instance.GameTime = 0f;
        PlayerStats.Instance.InitializeStats();
        PlayerController.Instance.transform.position = Vector3.zero; // 초기 위치 설정
        zones = new List<ZoneData>();
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

}
