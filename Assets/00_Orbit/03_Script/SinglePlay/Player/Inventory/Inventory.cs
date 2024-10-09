using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STARTING
{
    public class Inventory : MonoBehaviour
    {
        public int chip { get; private set; }
        public Sprite chipIcon;

        public List<int> availableWeaponIndices;  // 사용할 수 있는 무기 인덱스들
        public List<int> equippedWeaponIndices;   // 장착 중인 무기 인덱스들

        void Start()
        {
            InitializeInventory();
        }

        public void InitializeInventory()
        {
            chip = 0;
            availableWeaponIndices = new List<int>();
            equippedWeaponIndices = new List<int>();
        }

        // 무기 구매 함수
        public void PurchaseWeapon(int weaponIndex, int weaponCost)
        {
            // 칩이 충분한지 확인
            if (chip >= weaponCost)
            {
                chip -= weaponCost;  // 칩 차감
                availableWeaponIndices.Add(weaponIndex);  // 구매한 무기의 인덱스를 리스트에 추가
                Debug.Log($"무기 {weaponIndex}를 구매했습니다. 남은 칩: {chip}");


                for (int i = 0; i < availableWeaponIndices.Count; i++)
                    Debug.Log(availableWeaponIndices[i]);


                // 칩 상태 UI 업데이트 (UIManager에서 처리)
                UIManager.Instance.UpdateStats("chip", chip);
            }
            else
            {
                Debug.Log("칩이 부족합니다.");
            }
        }

        // 무기 구매 시 사용할 수 있는 무기 인덱스에 추가
        public void AddWeaponToAvailable(int weaponIndex)
        {
            if (!availableWeaponIndices.Contains(weaponIndex))
            {
                availableWeaponIndices.Add(weaponIndex);
            }
        }

        // 무기 장착 시 호출
        public void EquipWeapon(int weaponIndex)
        {
            if (equippedWeaponIndices.Count < 2 && !equippedWeaponIndices.Contains(weaponIndex))
            {
                equippedWeaponIndices.Add(weaponIndex);
            }
        }


        // 장착 해제 시 호출
        public void UnequipWeapon(int weaponIndex)
        {
            equippedWeaponIndices.Remove(weaponIndex);
        }

        // 현재 장착 중인 무기 인덱스 반환
        public int GetNextAvailableWeaponIndex(int currentWeaponIndex)
        {
            int currentIndex = availableWeaponIndices.IndexOf(currentWeaponIndex);
            return availableWeaponIndices[(currentIndex + 1) % availableWeaponIndices.Count];  // 순환 방식
        }






        public void GainChip()
        {
            chip++;
            GameManager.Instance.SaveGamePartial("chip", chip);
            UIManager.Instance.UpdateStats("chip", chip);
            StartCoroutine(ChipLog(2f));
        }

        public IEnumerator ChipLog(float time)
        {
            yield return new WaitForSeconds(time);
            UIManager.Instance.ShowKillLog("온전한 칩", 2f, "blue", chipIcon);
            StartCoroutine(UIManager.Instance.ShowTipKey());
        }


        public void SetInventory(int chip)
        {
            this.chip = chip;
            UIManager.Instance.UpdateStats("chip", chip);
        }

        public int GetChip() { return chip; }
    }
}
