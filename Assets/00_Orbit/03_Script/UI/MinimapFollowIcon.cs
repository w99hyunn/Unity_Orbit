using UnityEngine;

public class MinimapFollowIcon : MonoBehaviour
{
    public RectTransform playerIcon;
    public Transform player;
    public Camera minimapCamera;

    void LateUpdate()
    {
        Vector3 playerPosition = minimapCamera.WorldToViewportPoint(player.position);

        playerIcon.anchorMin = playerPosition;
        playerIcon.anchorMax = playerPosition;
        playerIcon.anchoredPosition = Vector2.zero;
    }
}
