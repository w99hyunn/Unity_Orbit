using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapFollowIcon : MonoBehaviour
{
    public RectTransform playerIcon;
    public Transform player;
    public Camera minimapCamera;

    void LateUpdate()
    {
        // 플레이어의 월드 좌표를 미니맵 카메라의 화면 좌표로 변환
        Vector3 playerPosition = minimapCamera.WorldToViewportPoint(player.position);

        // 미니맵 카메라의 화면 좌표를 UI 캔버스 좌표로 변환
        playerIcon.anchorMin = playerPosition;
        playerIcon.anchorMax = playerPosition;
        playerIcon.anchoredPosition = Vector2.zero;
    }
}
