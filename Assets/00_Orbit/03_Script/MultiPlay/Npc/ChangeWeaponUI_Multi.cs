using UnityEngine;
using UnityEngine.UI;
using Demo.Scripts.Runtime.Character;
using Demo.Scripts.Runtime.Item;
using TMPro;
using UnityEngine.Events;
using Mirror;
using System.Collections;

namespace STARTING
{
    public class ChangeWeaponUI_Multi : MonoBehaviour
    {
        public GameObject weaponButtonPrefab;  // 전체 무기
        public GameObject currentWeaponButtonPrefab;  // 장착 중인 무기
        public GameObject weaponEmptyPrefab; // 현재 무기에서 빈 슬롯
        public Transform leftWeaponListParent; // 전체 무기 리스트
        public Transform rightWeaponListParent; // 장착 중인 무기 리스트

        public UnityEvent unEquip;
        public UnityEvent chipLess;
        public UnityEvent fullWeapon;

        private GameObject player;
        private Inventory inventory;
        private FPSController_Multi fpsController;

        private void Start()
        {
            StartCoroutine(FindLocalPlayer());
        }

        void OnEnable()
        {
            UpdateWeaponLists();
        }

        private IEnumerator FindLocalPlayer()
        {
            while (NetworkClient.localPlayer == null)
            {
                yield return null;
            }

            player = NetworkClient.localPlayer.gameObject;
            inventory = player.GetComponent<Inventory>();
            fpsController = player.GetComponent<FPSController_Multi>();
        }

        public void UpdateWeaponLists()
        {
            // 좌측 리스트
            foreach (Transform child in leftWeaponListParent)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < fpsController._instantiatedWeapons.Count; i++)
            {
                FPSItem weapon = fpsController._instantiatedWeapons[i];
                Weapon_Multi weaponC = weapon.gameObject.GetComponent<Weapon_Multi>();
                GunFire_Multi gunFire = weapon.gameObject.GetComponent<GunFire_Multi>();

                GameObject weaponButton = Instantiate(weaponButtonPrefab, leftWeaponListParent);

                // 무기 이미지
                Image weaponImage = weaponButton.transform.Find("WeaponImage").GetComponent<Image>();
                weaponImage.sprite = weapon.weaponPreview;

                // 무기 이름
                TMP_Text weaponName = weaponButton.transform.Find("WeaponName").GetComponent<TMP_Text>();
                weaponName.text = weapon.name.Replace("(Clone)", "").Trim();

                // 무기 가격
                TMP_Text cost = weaponButton.transform.Find("Lock/LockText/Chip/ChipCost").GetComponent<TMP_Text>();
                cost.text = weapon.cost.ToString();

                //무기 정보
                TMP_Text weaponInfo = weaponButton.transform.Find("WeaponInfo").GetComponent<TMP_Text>();
                weaponInfo.text = $"연사속도 {weaponC.fireRate} / 데미지 {gunFire.damage}";

                Button purchaseButton = weaponButton.transform.Find("Lock/LockText/PurchaseButton").GetComponent<Button>();
                Button equipButton = weaponButton.transform.Find("EquipButton").GetComponent<Button>();
                GameObject equipWeapon = weaponButton.transform.Find("EquipWeapon").gameObject;

                int currentIndex = i;
                if (inventory.availableWeaponIndices.Contains(currentIndex))  // 구매한 무기
                {
                    weaponButton.transform.Find("Lock").gameObject.SetActive(false);
                    if (inventory.equippedWeaponIndices.Contains(currentIndex))
                    {
                        equipButton.gameObject.SetActive(false);
                        equipWeapon.SetActive(true);
                    }
                    else
                    {
                        equipButton.gameObject.SetActive(true);
                        equipWeapon.SetActive(false);
                    }
                }
                else  // 구매하지 않은 무기
                {
                    weaponButton.transform.Find("Lock").gameObject.SetActive(true);
                    equipButton.gameObject.SetActive(false);
                    equipWeapon.gameObject.SetActive(false);

                    purchaseButton.onClick.AddListener(() => OnPurchaseButtonClicked(currentIndex, weapon.cost));
                }
                equipButton.onClick.AddListener(() => OnEquipButtonClicked(currentIndex));
            }


            //
            // 우측 리스트 (장착 중인 무기) 업데이트
            //
            foreach (Transform child in rightWeaponListParent)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < inventory.equippedWeaponIndices.Count; i++)
            {
                int weaponIndex = inventory.equippedWeaponIndices[i];

                FPSItem weapon = fpsController._instantiatedWeapons[weaponIndex];
                GameObject weaponButton = Instantiate(currentWeaponButtonPrefab, rightWeaponListParent);
                Button unEquipButton = weaponButton.transform.Find("UnEquipButton").GetComponent<Button>();

                Image weaponImage = weaponButton.transform.Find("WeaponImage").GetComponent<Image>();
                weaponImage.sprite = weapon.weaponPreview;

                TMP_Text weaponName = weaponButton.transform.Find("WeaponName").GetComponent<TMP_Text>();
                weaponName.text = weapon.name.Replace("(Clone)", "").Trim();

                WeaponSlotDragHandler weaponPutIndex = weaponButton.GetComponent<WeaponSlotDragHandler>();
                weaponPutIndex.slotIndex = i;

                unEquipButton.onClick.AddListener(() => OnEquipButtonClicked(weaponIndex));
            }

            int maxSlots = 2;
            int emptySlotsNeeded = maxSlots - inventory.equippedWeaponIndices.Count;

            for (int i = 0; i < emptySlotsNeeded; i++)
            {
                GameObject emptySlot = Instantiate(weaponEmptyPrefab, rightWeaponListParent);
            }
        }

        // 무기 구매
        public void OnPurchaseButtonClicked(int weaponIndex, int cost)
        {
            if (true == inventory.PurchaseWeapon(weaponIndex, cost))
            {
                UpdateWeaponLists();
            }
            else
            {
                chipLess?.Invoke();
            }
        }

        // 무기 장착
        public void OnEquipButtonClicked(int weaponIndex)
        {
            if (inventory.equippedWeaponIndices.Contains(weaponIndex))
            {
                if (inventory.equippedWeaponIndices.Count <= 1)
                {
                    unEquip?.Invoke();
                    return;
                }
                inventory.UnequipWeapon(weaponIndex);
            }
            else
            {
                if (false == inventory.EquipWeapon(weaponIndex))
                {
                    fullWeapon?.Invoke();
                }
            }

            UpdateWeaponLists();
        }

        // 무기 슬롯 교체
        public void SwapWeaponSlots(int index1, int index2)
        {
            if (inventory.equippedWeaponIndices.Count <= Mathf.Max(index1, index2)) return;

            inventory.SwapWeapon(index1, index2);
            UpdateWeaponLists();
        }
    }
}