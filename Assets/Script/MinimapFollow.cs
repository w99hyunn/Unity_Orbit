using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MinimapFollow : MonoBehaviour
{
    public RectTransform playerIcon;
    public Transform player;
    private Camera minimapCamera;
    public Slider sizeSlider;    // 슬라이더 UI

    private void Start()
    {
        minimapCamera = GetComponent<Camera>();

        // 슬라이더 초기값 설정
        if (minimapCamera != null && sizeSlider != null)
        {
            sizeSlider.value = minimapCamera.orthographicSize;
            sizeSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    void OnSliderValueChanged(float value)
    {
        // 슬라이더 값 변경 시 카메라 Orthographic Size 변경
        if (minimapCamera != null)
        {
            minimapCamera.orthographicSize = value;
        }
    }

    void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);

        // 플레이어의 월드 좌표를 미니맵 카메라의 화면 좌표로 변환
        Vector3 playerPosition = minimapCamera.WorldToViewportPoint(player.position);

        // 미니맵 카메라의 화면 좌표를 UI 캔버스 좌표로 변환
        playerIcon.anchorMin = playerPosition;
        playerIcon.anchorMax = playerPosition;
        playerIcon.anchoredPosition = Vector2.zero;
    }
}
