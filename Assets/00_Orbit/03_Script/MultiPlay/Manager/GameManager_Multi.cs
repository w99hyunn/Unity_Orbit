using Demo.Scripts.Runtime.Character;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace STARTING
{
    public class GameManager_Multi : MonoBehaviour
    {
        public static GameManager_Multi Instance { get; private set; }

        private GameObject player;
        private FPSMovement_Multi controller;
        private PlayerStats_Multi playerStats;
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
        }

        private void Start()
        {
            StartCoroutine(FindLocalPlayer());
        }

        private IEnumerator FindLocalPlayer()
        {
            while (NetworkClient.localPlayer == null)
            {
                yield return null;
            }

            player = NetworkClient.localPlayer.gameObject;
            Debug.Log(player.name);
            if (player != null)
            {
                controller = player.GetComponent<FPSMovement_Multi>();
                playerStats = player.GetComponent<PlayerStats_Multi>();
                inventory = player.GetComponent<Inventory>();
            }
            else
            {
                Debug.LogWarning("Player 못찾음. 할당 X");
            }
        }

        private void OnEnable()
        {
            PlayerStats_Multi.OnPlayerStatsInitialized += OnPlayerStatsInitialized;
            FPSMovement_Multi.OnPlayerControllerInitialized += OnPlayerControllerInitialized;
        }

        private void OnDisable()
        {
            PlayerStats_Multi.OnPlayerStatsInitialized -= OnPlayerStatsInitialized;
            FPSMovement_Multi.OnPlayerControllerInitialized -= OnPlayerControllerInitialized;
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
               // SaveGame();
            }
        }

        public void GameOver()
        {
            UIManager.Instance.GameOverUI();
        }

        // 게임 오버시 체크포인트 불러오기
        public void ContinueGame()
        {
            if (SceneManager.GetActiveScene().name == "DungeonScene")
            {
                StartCoroutine(LoadWorldSceneAfterDelay(3f));
            }
            else
            {
                //LoadGame();
            }

        }

        private IEnumerator LoadWorldSceneAfterDelay(float delay)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("WorldScene", LoadSceneMode.Additive);
            asyncLoad.allowSceneActivation = false;

            yield return new WaitUntil(() => asyncLoad.progress >= 0.9f);

            asyncLoad.allowSceneActivation = true;

            yield return new WaitUntil(() => asyncLoad.isDone);

            SceneManager.SetActiveScene(SceneManager.GetSceneByName("WorldScene"));

            //LoadGame();
            SceneManager.UnloadSceneAsync("DungeonScene");

            yield return new WaitForSeconds(delay);
            //PlayerStats.Instance.ChangeState(1f, PlayerState.IDLE);
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
            };

            // DB에 게임 데이터 저장
            int userId = PlayerPrefs.GetInt("UserID"); // 현재 로그인한 사용자 ID
            DBManager.Instance.SaveGame(data, userId); // DBManager의 SaveGame 메서드 호출

            PlayerPrefs.SetInt("ContinueGame", 1); // 게임 상태 저장
            PlayerPrefs.Save();
        }

        public void LoadGame()
        {
            int userId = PlayerPrefs.GetInt("UserID"); // 현재 로그인한 사용자 ID
            RequestGameData(userId); // 로그인 성공 시 게임 데이터 요청
        }

        public void RequestGameData(int userId)
        {
            NetworkClient.RegisterHandler<GameDataResponseMessage>(OnGameDataResponse);

            GameDataRequestMessage gameDataRequest = new GameDataRequestMessage
            {
                userId = userId
            };

            NetworkClient.Send(gameDataRequest);
        }

        private void OnGameDataResponse(GameDataResponseMessage msg)
        {
            ApplyGameData(msg.gameData);
        }

        private void ApplyGameData(GameData data)
        {
            if (data != null)
            {
                this.gameTime = data.gameTime;
                playerStats.SetStats(data.maxHealth, data.maxMana, data.maxExperience, data.currentHealth, data.currentMana, data.currentExperience, data.level);
                controller.SetPos(data.playerPosition);
                this.zones = data.zones;
                inventory.SetInventory(data.chip);

                Debug.Log("Game loaded successfully from DB.");
                Debug.Log($"{data.gameTime}, {data.currentExperience}, {data.level} ");
            }
            else
            {
                Debug.LogError("No game data found for user.");
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
            //zones.Add(new ZoneData { zoneName = zoneName, isLiberated = true });
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
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }
}