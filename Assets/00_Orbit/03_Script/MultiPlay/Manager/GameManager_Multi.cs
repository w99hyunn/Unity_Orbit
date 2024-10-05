using Demo.Scripts.Runtime.Character;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace STARTING
{
    public class GameManager_Multi : NetworkBehaviour
    {
        public static GameManager_Multi Instance { get; private set; }
        public ClientNetworkHandler clientNetworkHandler;

        public event Action OnEnemyHit;

        public GameObject player;
        public FPSMovement_Multi controller;
        public PlayerStats_Multi playerStats;
        public Inventory inventory;

        // 인스턴스 던전관련
        public List<ZoneData> zones;
        public string currentZoneName;
        public Vector3 lastPlayerPosition { get; private set; }
        public AudioSource audioSource;

        public float gameTime = 13600f; // 21600

        private string _saveFilePath;
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
            //_saveFilePath = Path.Combine(Application.persistentDataPath, "gameData.json");
        }

        private void Start()
        {
            StartCoroutine(FindLocalPlayer());
        }

        private void Update()
        {
            UpdateGameTime();
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

                LoadGame();
                SaveGame();
                Debug.Log("얜가");
            }
            else
            {
                Debug.LogWarning("Player 못찾음. 할당 X");
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
                InitializePlayerAfterGameOver();
                LoadGame();
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

            InitializePlayerAfterGameOver();
            LoadGame();
            SceneManager.UnloadSceneAsync("DungeonScene");

            yield return new WaitForSeconds(delay);
        }

        private void InitializePlayerAfterGameOver()
        {
            GameData data = DBManager.Instance.clientGameData;

            data.currentHealth = 50;
            data.currentExperience = Mathf.Max(0, data.currentExperience - (int)(data.currentExperience * 0.3f));
            data.playerPosition = new Vector3(0, 0, 0);

            SaveGame();
        }

        public void SaveGame()
        {
            clientNetworkHandler.SendRequestUpdatedGameData(DBManager.Instance.clientGameData);
        }

        public void LoadGame()
        {
            GameData data = DBManager.Instance.clientGameData;

            if (data.level != -1)
            {
                this.gameTime = data.gameTime;
                controller.SetPos(data.playerPosition);
                playerStats.SetStats(data.maxHealth, data.maxMana, data.maxExperience, data.currentHealth, data.currentMana, data.currentExperience, data.level);
                //this.zones = data.zones;
                inventory.SetInventory(data.chip);
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

        //public void LiberateZone(string zoneName)
        //{
        //    foreach (ZoneData zone in zones)
        //    {
        //        if (zone.zoneName == zoneName)
        //        {
        //            zone.isLiberated = true;
        //            //SaveZoneData(zoneName, true);
        //            return;
        //        }
        //    }
        //}

        //public bool IsZoneLiberated(string zoneName)
        //{
        //    foreach (ZoneData zone in zones)
        //    {
        //        if (zone.zoneName == zoneName)
        //        {
        //            return zone.isLiberated;
        //        }
        //    }
        //    return false;
        //}

        //public bool SetCurrentZone(string zoneName)
        //{
        //    currentZoneName = zoneName;

        //    if (!zones.Any(zone => zone.zoneName == zoneName))
        //    {
        //        zones.Add(new ZoneData(zoneName, false));
        //    }

        //    return IsZoneLiberated(zoneName);
        //}

        public void PlaySound(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        //public void SaveZoneData(string zoneName, bool isLiberated)
        //{
        //    GameData data = LoadGameData();

        //    ZoneData zone = data.zones.FirstOrDefault(z => z.zoneName == zoneName);

        //    Debug.Log(zone);


        //    if (zone != null)
        //    {
        //        zone.isLiberated = isLiberated;
        //    }
        //    else
        //    {
        //        data.zones.Add(new ZoneData(zoneName, isLiberated));
        //        Debug.Log(zoneName + "/" + isLiberated);
        //    }

        //    string json = JsonUtility.ToJson(data);
        //    string encryptedJson = CryptoUtility.EncryptString(json);
        //    File.WriteAllText(_saveFilePath, encryptedJson);
        //}

        public void SaveGamePartial(string fieldName, object value)
        {
            if (SceneManager.GetActiveScene().name == "DungeonScene")
            {
                return;
            }

            GameData data = DBManager.Instance.clientGameData;

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
            }

            SaveGame();
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