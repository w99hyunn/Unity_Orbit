using UnityEngine;
using UnityEngine.UI;
using Demo.Scripts.Runtime.Character;
using Demo.Scripts.Runtime.Item;
using TMPro;

namespace STARTING
{
    public class ChangeWeaponUI : MonoBehaviour
    {
        public GameObject weaponButtonPrefab;  // 무기 버튼 프리팹
        public GameObject currentWeaponButtonPrefab;  // 무기 버튼 프리팹
        public Transform leftWeaponListParent; // 좌측 리스트 부모 (전체 무기)
        public Transform rightWeaponListParent; // 우측 리스트 부모 (장착 중인 무기)


        private GameObject player;
        private Inventory inventory;
        private FPSController fpsController;

        void OnEnable()
        {
            player = GameObject.FindWithTag("Player");
            // Inventory와 FPSController 컴포넌트 가져오기
            inventory = player.GetComponent<Inventory>();
            fpsController = player.GetComponent<FPSController>();

            // 무기 리스트 초기화
            UpdateWeaponLists();
        }

        // 좌측 리스트와 우측 리스트 업데이트
        public void UpdateWeaponLists()
        {
            // 좌측 리스트 (전체 무기) 업데이트
            foreach (Transform child in leftWeaponListParent)
            {
                Destroy(child.gameObject);  // 기존 항목 삭제
            }

            for (int i = 0; i < fpsController._instantiatedWeapons.Count; i++)
            {
                FPSItem weapon = fpsController._instantiatedWeapons[i];
                Weapon weaponC = weapon.gameObject.GetComponent<Weapon>();
                GunFire gunFire = weapon.gameObject.GetComponent<GunFire>();

                GameObject weaponButton = Instantiate(weaponButtonPrefab, leftWeaponListParent);

                // 무기 이미지 설정
                Image weaponImage = weaponButton.transform.Find("WeaponImage").GetComponent<Image>();
                weaponImage.sprite = weapon.weaponPreview;

                // 무기 이름 설정
                TMP_Text weaponName = weaponButton.transform.Find("WeaponName").GetComponent<TMP_Text>();
                weaponName.text = weapon.name.Replace("(Clone)", "").Trim();

                // 무기 가격
                TMP_Text cost = weaponButton.transform.Find("Lock/LockText/Chip/ChipCost").GetComponent<TMP_Text>();
                cost.text = weapon.cost.ToString();

                //무기 정보
                TMP_Text weaponInfo = weaponButton.transform.Find("WeaponInfo").GetComponent<TMP_Text>();
                weaponInfo.text = $"연사속도 {weaponC.fireRate} / 데미지 {gunFire.damage}";

                // 구매 및 장착 버튼 제어
                Button purchaseButton = weaponButton.transform.Find("Lock/LockText/PurchaseButton").GetComponent<Button>();
                Button equipButton = weaponButton.transform.Find("EquipButton").GetComponent<Button>();

                // 지역 변수로 무기 인덱스 복사
                int currentIndex = i;

                if (inventory.availableWeaponIndices.Contains(currentIndex))  // 이미 구매한 무기
                {
                    // '잠김' 텍스트 비활성화
                    weaponButton.transform.Find("Lock").gameObject.SetActive(false);
                    equipButton.gameObject.SetActive(true);  // 장착 버튼 활성화
                }
                else  // 아직 구매하지 않은 무기
                {
                    // '잠김' 텍스트와 구매 버튼 활성화
                    weaponButton.transform.Find("Lock").gameObject.SetActive(true);
                    equipButton.gameObject.SetActive(false);  // 장착 버튼 비활성화

                    // 구매 버튼 클릭 시 실행될 함수 연결 (currentIndex 사용)
                    purchaseButton.onClick.AddListener(() => OnPurchaseButtonClicked(currentIndex, weapon.cost));
                }

                // 장착 버튼 클릭 시 실행될 함수 연결 (currentIndex 사용)
                equipButton.onClick.AddListener(() => OnEquipButtonClicked(currentIndex));
            }


            // 우측 리스트 (장착 중인 무기) 업데이트
            foreach (Transform child in rightWeaponListParent)
            {
                Destroy(child.gameObject);  // 기존 항목 삭제
            }

            foreach (int weaponIndex in inventory.equippedWeaponIndices)
            {
                FPSItem weapon = fpsController._instantiatedWeapons[weaponIndex];
                GameObject weaponButton = Instantiate(currentWeaponButtonPrefab, rightWeaponListParent);

                // 무기 이미지와 이름 설정 (장착 중인 무기)
                Image weaponImage = weaponButton.transform.Find("WeaponImage").GetComponent<Image>();
                weaponImage.sprite = weapon.weaponPreview;

                TMP_Text weaponName = weaponButton.transform.Find("WeaponName").GetComponent<TMP_Text>();
                weaponName.text = weapon.name;

                // 장착 중인 무기는 잠김 표시와 구매 버튼 없음
                //weaponButton.transform.Find("Lock").gameObject.SetActive(false);
            }
        }

        // 무기 구매 버튼 클릭 시 실행되는 함수
        public void OnPurchaseButtonClicked(int weaponIndex, int cost)
        {
            // cost == 무기 가격
            // 임시는 0으로 할게
            inventory.PurchaseWeapon(weaponIndex, 0); // 0대신 cost 전달

            // 무기 리스트 업데이트
            UpdateWeaponLists();
        }

        // 무기 장착 버튼 클릭 시 실행되는 함수
        public void OnEquipButtonClicked(int weaponIndex)
        {
            if (inventory.equippedWeaponIndices.Contains(weaponIndex))  // 장착 해제
            {
                inventory.UnequipWeapon(weaponIndex);
            }
            else  // 무기 장착
            {
                inventory.EquipWeapon(weaponIndex);
            }

            // 무기 리스트 업데이트
            UpdateWeaponLists();
        }
    }
}