using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class MinimapFollow : MonoBehaviour
{
    public RectTransform playerIcon;
    private Transform player;
    private Camera minimapCamera;

    private void Start()
    {
        minimapCamera = GetComponent<Camera>();
        player = GameObject.FindWithTag("Player").transform;
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
