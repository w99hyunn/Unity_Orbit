using Demo.Scripts.Runtime.Character;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
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
    public GameObject player;
    private FPSMovement _controller;
    private PlayerStats _plyerStats;


    public bool isGameOver = false;

    // 인스턴스 던전관련
    public List<ZoneData> zones { get; private set; } //디버그용 public
    public string currentZoneName;
    public Vector3 lastPlayerPosition { get; private set; }

    private AudioSource audioSource;
    private string saveFilePath;
    private bool isPlayerStatsInitialized = false;
    private bool isPlayerControllerInitialized = false;

    public float gameTime  = 13600f; // 21600
    private const float realSecondsPerGameDay = 3 * 60 * 60;
    private const float gameSecondsPerRealSecond = 24 * 60 * 60 / realSecondsPerGameDay;

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
        _controller = player.GetComponent<FPSMovement>();
        _plyerStats = player.GetComponent<PlayerStats>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        UpdateGameTime();

    }

    private void OnEnable()
    {
        PlayerStats.OnPlayerStatsInitialized += OnPlayerStatsInitialized;
        FPSMovement.OnPlayerControllerInitialized += OnPlayerControllerInitialized;
    }

    private void OnDisable()
    {
        PlayerStats.OnPlayerStatsInitialized -= OnPlayerStatsInitialized;
        FPSMovement.OnPlayerControllerInitialized -= OnPlayerControllerInitialized;
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
            gameTime = this.gameTime,
            currentHealth = _plyerStats.currentHealth,
            currentMana = _plyerStats.currentMana,
            currentExperience = _plyerStats.currentExperience,
            level = _plyerStats.level,
            playerPosition = player.transform.position,
            zones = this.zones
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

            Debug.Log("Loaded JSON: " + json);

            GameData data = JsonUtility.FromJson<GameData>(json);
            if (data != null)
            {
                this.gameTime = data.gameTime;
                _plyerStats.SetStats(data.currentHealth, data.currentMana, data.currentExperience, data.level);
                _controller.SetPos(data.playerPosition);
                this.zones = data.zones;
            }
        }
    }

    public void ResetPos()
    {
        _controller.ResetPos();
    }

    public void UpdateGameTime()
    {
        gameTime += Time.deltaTime * gameSecondsPerRealSecond;

        if (gameTime >= 24 * 60 * 60)
        {
            gameTime -= 24 * 60 * 60;
        }

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

        UIManager.Instance.UpdateTime(timeFormatted);
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
}
