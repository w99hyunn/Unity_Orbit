using TMPro;
using UnityEngine;

namespace STARTING
{
    public class UIAchievement : MonoBehaviour
    {
        public GameObject achievementItemPrefab;
        public Transform contentTransform;

        private void Start()
        {
            DisplayAchievements();
        }

        public void DisplayAchievements()
        {
            foreach (Transform child in contentTransform)
            {
                Destroy(child.gameObject);
            }

            // 업적 항목 리스트
            CreateAchievementItem("몬스터 처치한 횟수", AchievementManager.Instance.GetAchievementCount("MonsterKill"));
            CreateAchievementItem("필드에 존재하는 루트 박스를 오픈한 횟수", AchievementManager.Instance.GetAchievementCount("LootBoxOpen"));
            CreateAchievementItem("사망 횟수(맵 밖으로 이탈하거나, 몬스터에게 당한 횟수 모두 포함)", AchievementManager.Instance.GetAchievementCount("Deaths"));
            CreateAchievementItem("구역을 해방한 횟수", AchievementManager.Instance.GetAchievementCount("ZoneLiberations"));
            CreateAchievementItem("온전한 칩을 획득한 횟수", AchievementManager.Instance.GetAchievementCount("ChipCollection"));
            CreateAchievementItem("몬스터를 처치해서 온전한 칩을 획득한 횟수", AchievementManager.Instance.GetAchievementCount("IntactChipCollection"));
            CreateAchievementItem("레벨업을 한 횟수", AchievementManager.Instance.GetAchievementCount("LevelUp"));
            CreateAchievementItem("엘리베이터를 이용한 횟수", AchievementManager.Instance.GetAchievementCount("ElevatorUse"));
            CreateAchievementItem("자동문을 이용한 횟수", AchievementManager.Instance.GetAchievementCount("AutoDoorUse"));
            CreateAchievementItem("보급소에서 무기를 구매한 횟수", AchievementManager.Instance.GetAchievementCount("WeaponPurchase"));
        }

        private void CreateAchievementItem(string title, int count)
        {
            GameObject achievementItem = Instantiate(achievementItemPrefab, contentTransform);
            TMP_Text[] texts = achievementItem.GetComponentsInChildren<TMP_Text>();

            if (texts.Length >= 2)
            {
                texts[0].text = title;
                texts[1].text = count.ToString() + "회";
            }
        }
    }
}