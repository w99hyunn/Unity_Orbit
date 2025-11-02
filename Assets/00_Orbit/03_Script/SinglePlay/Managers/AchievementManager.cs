using System.IO;
using UnityEngine;

namespace STARTING
{
    [System.Serializable]
    public class AchievementData
    {
        public int monsterKills; //몬스터를 처치한 횟수
        public int lootBoxOpens; //루트 박스 오픈 횟수
        public int deaths; //사망 횟수
        public int zoneLiberations; //구역 해방 횟수
        public int chipCollections; //칩 획득 횟수
        public int intactChipCollections; //칩 획득 횟수인데, 몬스터에게서만
        public int levelUps; //레벨업 횟수
        public int elevatorUses; //엘리베이터 이용횟수
        public int autoDoorUses; //자동문 이용횟수
        public int weaponPurchases; // 무기구매 횟수
    }

    public class AchievementManager : MonoBehaviour
    {
        private string saveFilePath;
        private AchievementData achievementData;

        private static AchievementManager _instance;

        public static AchievementManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject obj = new GameObject("AchievementManager");
                    _instance = obj.AddComponent<AchievementManager>();
                    DontDestroyOnLoad(obj);

                    _instance.Initialize();
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Initialize()
        {
            saveFilePath = Path.Combine(Application.persistentDataPath, "achievementData.json");
            LoadAchievements();
        }

        // JSON 파일에서 데이터 로드
        public void LoadAchievements()
        {
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                achievementData = JsonUtility.FromJson<AchievementData>(json);
            }
            else
            {
                achievementData = new AchievementData();
            }
        }

        // JSON 파일에 데이터 저장
        public void SaveAchievements()
        {
            string json = JsonUtility.ToJson(achievementData, true);
            File.WriteAllText(saveFilePath, json);
        }

        // 항목 업데이트 메서드
        public void UpdateAchievement(string type, int amount)
        {
            switch (type)
            {
                case "MonsterKill": achievementData.monsterKills += amount; break;
                case "LootBoxOpen": achievementData.lootBoxOpens += amount; break;
                case "Deaths": achievementData.deaths += amount; break;
                case "ZoneLiberations": achievementData.zoneLiberations += amount; break;
                case "ChipCollection": achievementData.chipCollections += amount; break;
                case "IntactChipCollection": achievementData.intactChipCollections += amount; break;
                case "LevelUp": achievementData.levelUps += amount; break;
                case "ElevatorUse": achievementData.elevatorUses += amount; break;
                case "AutoDoorUse": achievementData.autoDoorUses += amount; break;
                case "WeaponPurchase": achievementData.weaponPurchases += amount; break;
            }
            SaveAchievements();
        }

        // 각 항목의 현재 상태를 가져오는 메서드
        public int GetAchievementCount(string type)
        {
            return type switch
            {
                "MonsterKill" => achievementData.monsterKills,
                "LootBoxOpen" => achievementData.lootBoxOpens,
                "Deaths" => achievementData.deaths,
                "ZoneLiberations" => achievementData.zoneLiberations,
                "ChipCollection" => achievementData.chipCollections,
                "IntactChipCollection" => achievementData.intactChipCollections,
                "LevelUp" => achievementData.levelUps,
                "ElevatorUse" => achievementData.elevatorUses,
                "AutoDoorUse" => achievementData.autoDoorUses,
                "WeaponPurchase" => achievementData.weaponPurchases,
                _ => 0
            };
        }
    }
}