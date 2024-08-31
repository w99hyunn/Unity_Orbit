using UnityEngine;

public class HUDMovement : MonoBehaviour
{
    public RectTransform hud;
    public float moveSpeed = 1f;
    public Vector2 minPosition;
    public Vector2 maxPosition;

    private Vector2 targetPosition;

    void Start()
    {
        // 초기 목표 위치는 현재 HUD 위치로 설정
        targetPosition = hud.anchoredPosition;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        targetPosition.x = Mathf.Clamp(targetPosition.x + (mouseX * moveSpeed), minPosition.x, maxPosition.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y + (mouseY * moveSpeed), minPosition.y, maxPosition.y);

        // HUD의 현재 위치를 목표 위치로 서서히 이동시키기
        hud.anchoredPosition = Vector2.Lerp(hud.anchoredPosition, targetPosition, Time.deltaTime * moveSpeed);
    }
}
