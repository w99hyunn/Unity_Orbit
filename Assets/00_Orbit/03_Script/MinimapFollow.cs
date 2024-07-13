using UnityEngine;

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

        Vector3 playerPosition = minimapCamera.WorldToViewportPoint(player.position);

        playerIcon.anchorMin = playerPosition;
        playerIcon.anchorMax = playerPosition;
        playerIcon.anchoredPosition = Vector2.zero;
    }
}
