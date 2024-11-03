using Demo.Scripts.Runtime.Character;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace STARTING
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public event Action OnEnemyHit;

        public GameObject player;
        private FPSMovement controller;
        private PlayerStats playerStats;
        private Inventory inventory;

        // 인스턴스 던전관련
        public List<ZoneData> zones;
        public string currentZoneName;
        public Vector3 lastPlayerPosition { get; private set; }
        public AudioSource audioSource;

        public float gameTime = 13600f; // 21600

        private string _saveFilePath;
        private bool _isPlayerStatsInitialized = false;
        private bool _isPlayerControllerInitialized = false;
        private const float _realSecondsPerGameDay = 3 * 60 * 60;
        private const float _gameSecondsPerRealSecond = 24 * 60 * 60 / _realSecondsPerGameDay;
        private int _lastLoggedMinute = -1;

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

            _saveFilePath = Path.Combine(Application.persistentDataPath, "gameData.json");
            controller = player.GetComponent<FPSMovement>();
            playerStats = player.GetComponent<PlayerStats>();
            inventory = player.GetComponent<Inventory>();
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

        private void Update()
        {
            UpdateGameTime();
        }

        private void OnPlayerStatsInitialized()
        {
            _isPlayerStatsInitialized = true;
            TryLoadGame();
        }

        private void OnPlayerControllerInitialized()
        {
            _isPlayerControllerInitialized = true;
            TryLoadGame();
        }

        private void TryLoadGame()
        {
            if (_isPlayerStatsInitialized && _isPlayerControllerInitialized)
            {
                LoadGame();
                SaveGame();
            }
        }

        public void GameOver()
        {
            UIManager.Instance.GameOverUI();
        }

        // 게임 오버시 체크포인트 불러오기
        public void ContinueGame()
        {
            if (SceneManager.GetActiveScene().name == SceneDataManager.GetSceneName("SingleDungeon"))
            {
                StartCoroutine(LoadWorldSceneAfterDelay(3f));
            }
            else
            {
                InitializePlayerAfterGameOver();
                LoadGame();
            }
        }

        private IEnumerator LoadWorldSceneAfterDelay(float delay)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneDataManager.GetSceneName("Single"), LoadSceneMode.Additive);
            asyncLoad.allowSceneActivation = false;

            yield return new WaitUntil(() => asyncLoad.progress >= 0.9f);

            asyncLoad.allowSceneActivation = true;

            yield return new WaitUntil(() => asyncLoad.isDone);

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneDataManager.GetSceneName("Single")));

            InitializePlayerAfterGameOver();
            LoadGame();
            SceneManager.UnloadSceneAsync(SceneDataManager.GetSceneName("SingleDungeon"));

            yield return new WaitForSeconds(delay);
        }

        private void InitializePlayerAfterGameOver()
        {
            GameData data = LoadGameData();

            data.currentHealth = 50;
            data.currentExperience = Mathf.Max(0, data.currentExperience - (int)(data.currentExperience * 0.3f));
            data.playerPosition = new Vector3(26.19f, 2.1f, -47.5f);

            string json = JsonUtility.ToJson(data);
            string encryptedJson = CryptoUtility.EncryptString(json);
            File.WriteAllText(_saveFilePath, encryptedJson);
        }

        public void SaveGame()
        {
            GameData data = new GameData
            {
                gameTime = this.gameTime,

                maxHealth = playerStats.maxHealth,
                maxMana = playerStats.maxMana,
                maxExperience = playerStats.maxExperience,

                currentHealth = playerStats.currentHealth,
                currentMana = playerStats.currentMana,
                currentExperience = playerStats.currentExperience,
                level = playerStats.level,
                playerPosition = player.transform.position,
                zones = this.zones,
                chip = inventory.chip,

                availableWeaponIndices = inventory.availableWeaponIndices,
                equippedWeaponIndices = inventory.equippedWeaponIndices
            };
            //Debug.Log("saved JSON: " + json);

            string json = JsonUtility.ToJson(data);
            string encryptedJson = CryptoUtility.EncryptString(json); // 암호화
            File.WriteAllText(_saveFilePath, encryptedJson);
        }

        public void LoadGame()
        {
            if (File.Exists(_saveFilePath))
            {
                string encryptedJson = File.ReadAllText(_saveFilePath);
                string json = CryptoUtility.DecryptString(encryptedJson); // 복호화

                //Debug.Log("Loaded JSON: " + json);

                GameData data = JsonUtility.FromJson<GameData>(json);
                if (data != null)
                {
                    this.gameTime = data.gameTime;
                    controller.SetPos(data.playerPosition);
                    playerStats.SetStats(data.maxHealth, data.maxMana, data.maxExperience, data.currentHealth, data.currentMana, data.currentExperience, data.level);
                    this.zones = data.zones;
                    inventory.SetInventory(data.chip);

                    // 무기 인덱스 로드
                    inventory.availableWeaponIndices = data.availableWeaponIndices;
                    inventory.equippedWeaponIndices = data.equippedWeaponIndices;
                }
            }
        }

        public void SetPos(Vector3 pos)
        {
            controller.SetPos(pos);
        }

        public void ResetPos()
        {
            controller.ResetPos();
        }

        public void SavePlayerPosition(Vector3 position)
        {
            lastPlayerPosition = position;
        }

        public Vector3 LoadPlayerPosition()
        {
            return lastPlayerPosition;
        }

        public void UpdateGameTime()
        {
            gameTime += Time.deltaTime * _gameSecondsPerRealSecond;

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

            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateTime(timeFormatted);
            }

            if (minutes != _lastLoggedMinute)
            {
                SaveGamePartial("time", gameTime);
                _lastLoggedMinute = minutes;
            }
        }

        public void LiberateZone(string zoneName)
        {
            foreach (ZoneData zone in zones)
            {
                if (zone.zoneName == zoneName)
                {
                    zone.isLiberated = true;
                    SaveZoneData(zoneName, true);
                    AchievementManager.Instance.UpdateAchievement("ZoneLiberations", 1);
                    return;
                }
            }
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

        public bool SetCurrentZone(string zoneName)
        {
            currentZoneName = zoneName;

            if (!zones.Any(zone => zone.zoneName == zoneName))
            {
                zones.Add(new ZoneData(zoneName, false));
            }

            return IsZoneLiberated(zoneName);
        }

        public void PlaySound(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }

        public void SaveZoneData(string zoneName, bool isLiberated)
        {
            GameData data = LoadGameData();
            ZoneData zone = data.zones.FirstOrDefault(z => z.zoneName == zoneName);

            if (zone != null)
            {
                zone.isLiberated = isLiberated;
            }
            else
            {
                data.zones.Add(new ZoneData(zoneName, isLiberated));
            }

            string json = JsonUtility.ToJson(data);
            string encryptedJson = CryptoUtility.EncryptString(json);
            File.WriteAllText(_saveFilePath, encryptedJson);
        }

        public void SaveGamePartial(string fieldName, object value)
        {
            if (SceneManager.GetActiveScene().name == SceneDataManager.GetSceneName("SingleDungeon"))
            {
                return;
            }

            GameData data = LoadGameData();

            switch (fieldName)
            {
                case "maxHealth":
                    data.maxHealth = (int)value;
                    break;
                case "maxMana":
                    data.maxMana = (int)value;
                    break;
                case "maxExperience":
                    data.maxExperience = (int)value;
                    break;
                case "currentHealth":
                    data.currentHealth = (int)value;
                    break;
                case "currentMana":
                    data.currentMana = (int)value;
                    break;
                case "currentExperience":
                    data.currentExperience = (int)value;
                    break;
                case "level":
                    data.level = (int)value;
                    break;
                case "playerPosition":
                    data.playerPosition = (Vector3)value;
                    break;
                case "chip":
                    data.chip = (int)value;
                    break;
                case "time":
                    data.gameTime = (float)value;
                    break;
                case "availableWeaponIndices":
                    data.availableWeaponIndices = (List<int>)value;  // 사용 가능한 무기 인덱스 업데이트
                    break;
                case "equippedWeaponIndices":
                    data.equippedWeaponIndices = (List<int>)value;  // 장착 중인 무기 인덱스 업데이트
                    break;
            }

            string json = JsonUtility.ToJson(data);
            string encryptedJson = CryptoUtility.EncryptString(json);
            File.WriteAllText(_saveFilePath, encryptedJson);
        }

        private GameData LoadGameData()
        {
            if (File.Exists(_saveFilePath))
            {
                string encryptedJson = File.ReadAllText(_saveFilePath);
                string json = CryptoUtility.DecryptString(encryptedJson);
                return JsonUtility.FromJson<GameData>(json);
            }
            else
            {
                // 파일이 없을 때, 새로운 GameData 객체 반환
                return new GameData();
            }
        }

        public void EnemyHit()
        {
            if (OnEnemyHit != null)
            {
                OnEnemyHit.Invoke();
            }
        }
    }
}