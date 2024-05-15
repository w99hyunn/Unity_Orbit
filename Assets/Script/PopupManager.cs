using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    [SerializeField]
    private GameObject popupUI; // 팝업 UI GameObject
    [SerializeField]
    private float AutoPopupCloseTime = 3.0f; //0을 할당할 경우 자동으로 없어지지 않음.

    void Start()
    {
        // 팝업 UI를 시작할 때 비활성화 상태로 설정
        if (popupUI != null)
        {
            popupUI.SetActive(false);
        }
    }

    // 버튼 클릭 시 호출될 함수
    public void OpenPopup()
    {
        // 팝업 UI를 활성화하여 보여줌
        if (popupUI != null)
        {
            popupUI.SetActive(true);

            if (AutoPopupCloseTime > 0)
            {
                Invoke("ClosePopup", AutoPopupCloseTime);
            }
        }

      
    }

    // 팝업 UI 내부의 닫기 버튼이나 다른 동작에 사용될 함수
    public void ClosePopup()
    {
        // 팝업 UI를 비활성화하여 감춤
        if (popupUI != null)
        {
            popupUI.SetActive(false);
        }
    }
}