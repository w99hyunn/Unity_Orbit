using UnityEngine;

public class HUDDetailPopup : MonoBehaviour
{
    public GameObject tooltip; // 팝업 UI 오브젝트

    // 마우스가 해당 UI 요소 위에 올라왔을 때 호출
    public void ShowTooltip()
    {
        tooltip.SetActive(true);
    }

    // 마우스가 해당 UI 요소를 벗어났을 때 호출
    public void HideTooltip()
    {
        tooltip.SetActive(false);
    }
}
