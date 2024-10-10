using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STARTING
{
    public class Inventory : MonoBehaviour
    {
        public int chip { get; private set; }
        public Sprite chipIcon;

        [Header("무기 변경 시스템")]
        public List<int> availableWeaponIndices;
        public List<int> equippedWeaponIndices;

        void Start()
        {
            InitializeInventory();
        }

        public void InitializeInventory()
        {
            chip = 0;
            availableWeaponIndices = new List<int>();
            equippedWeaponIndices = new List<int>();

            availableWeaponIndices.Add(0);
            equippedWeaponIndices.Add(0);
        }

        // 무기 구매
        public bool PurchaseWeapon(int weaponIndex, int weaponCost)
        {
            if (chip >= weaponCost)
            {
                chip -= weaponCost;
                availableWeaponIndices.Add(weaponIndex);
                Debug.Log($"무기 {weaponIndex}를 구매. 남은 칩: {chip}");

                for (int i = 0; i < availableWeaponIndices.Count; i++)
                    Debug.Log(availableWeaponIndices[i]);

                UIManager.Instance.UpdateStats("chip", chip);
                GameManager.Instance.SaveGamePartial("chip", chip);
                GameManager.Instance.SaveGamePartial("availableWeaponIndices", availableWeaponIndices);
                return true;
            }
            else
            {
                Debug.Log("칩이 부족합니다.");
                return false;
            }
        }

        // 무기 장착
        public bool EquipWeapon(int weaponIndex)
        {
            if (equippedWeaponIndices.Count < 2 && !equippedWeaponIndices.Contains(weaponIndex))
            {
                equippedWeaponIndices.Add(weaponIndex);
                Debug.Log($"무기 {weaponIndex}를 장착했습니다.");

                GameManager.Instance.SaveGamePartial("equippedWeaponIndices", equippedWeaponIndices);
                return true;
            }
            else
            {
                return false;
            }
        }

        // 장착 해제
        public void UnequipWeapon(int weaponIndex)
        {
            equippedWeaponIndices.Remove(weaponIndex);
            Debug.Log($"무기 {weaponIndex}를 장착 해제했습니다.");

            // 게임 상태 저장
            GameManager.Instance.SaveGamePartial("equippedWeaponIndices", equippedWeaponIndices);
        }

        public void GainChip(int gain = 1)
        {
            chip = chip + gain;
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
