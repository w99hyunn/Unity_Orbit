using Orbit_Character;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameData
{
    public float gameTime;
    public int currentHealth;
    public int currentMana;
    public int currentExperience;
    public int level;
    public Vector3 playerPosition;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private string saveFilePath;

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
        SceneManager.sceneLoaded += OnSceneLoaded;
        PlayerStats.OnPlayerStatsInitialized += LoadGame; // 이벤트 구독
        PlayerController.OnPlayerStatsInitialized += LoadGame;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        PlayerStats.OnPlayerStatsInitialized -= LoadGame; // 이벤트 구독 해제
        PlayerController.OnPlayerStatsInitialized -= LoadGame;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "OutdoorsScene")
        {
            Debug.Log("OutdoorsScene 로드됨");
            // LoadGame 호출을 여기서 제거
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
            playerPosition = PlayerController.Instance.transform.position
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(saveFilePath, json);
        PlayerPrefs.SetInt("ContinueGame", 1);
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            Debug.Log("Loaded JSON: " + json); // 데이터가 제대로 읽혔는지 로그 확인

            GameData data = JsonUtility.FromJson<GameData>(json);

            if (data != null)
            {
                UIManager.Instance.GameTime = data.gameTime;
                PlayerStats.Instance.SetStats(data.currentHealth, data.currentMana, data.currentExperience, data.level);
                PlayerController.Instance.SetPos(data.playerPosition);

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
    }
}
