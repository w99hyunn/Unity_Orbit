using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace STARTING
{
    public class WeaponSlotDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public int slotIndex; // 무기 슬롯의 인덱스 (equippedWeaponIndices에서 사용)

        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Vector2 originalPosition;
        private ChangeWeaponUI changeWeaponUI;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            changeWeaponUI = GetComponentInParent<ChangeWeaponUI>();
        }

        // 드래그 시작 시 호출
        public void OnBeginDrag(PointerEventData eventData)
        {
            originalPosition = rectTransform.anchoredPosition;
            canvasGroup.blocksRaycasts = false;  // 드래그 중엔 Raycast를 차단하여 Slot이 드랍 가능한 상태가 됨
        }

        // 드래그 중에 호출
        public void OnDrag(PointerEventData eventData)
        {
            rectTransform.anchoredPosition += eventData.delta / canvasGroup.transform.lossyScale;
        }

        // 드래그 끝나면 호출
        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true;

            // 드래그 후 위치로 이동 or 원래 자리로 복귀
            if (!TrySwapWithAnotherSlot(eventData))
            {
                rectTransform.anchoredPosition = originalPosition; // 원래 위치로 되돌림
            }
        }

        // 다른 슬롯과 스왑 시도
        private bool TrySwapWithAnotherSlot(PointerEventData eventData)
        {
            // 드래그 중인 슬롯과 교체할 수 있는 슬롯을 찾음
            GameObject target = eventData.pointerEnter;

            if (target != null && target.GetComponent<WeaponSlotDragHandler>() != null)
            {
                // 대상 슬롯 인덱스
                int targetIndex = target.GetComponent<WeaponSlotDragHandler>().slotIndex;

                // 슬롯 교체 호출
                changeWeaponUI.SwapWeaponSlots(slotIndex, targetIndex);
                return true;
            }
            return false;
        }
    }
}