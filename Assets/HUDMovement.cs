using UnityEngine;

public class HUDMovement : MonoBehaviour
{
    public RectTransform hud;  // 이동할 HUD UI 요소
    public float moveSpeed = 5f;  // HUD 이동 속도 (Lerp 계수로 사용됨)
    public Vector2 minPosition;  // HUD의 최소 위치 제한
    public Vector2 maxPosition;  // HUD의 최대 위치 제한

    private Vector2 targetPosition; // HUD의 목표 위치

    void Start()
    {
        // 초기 목표 위치는 현재 HUD 위치로 설정
        targetPosition = hud.anchoredPosition;
    }

    void Update()
    {
        // 마우스 움직임 가져오기
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 목표 위치 계산
        targetPosition.x = Mathf.Clamp(targetPosition.x + (mouseX * moveSpeed), minPosition.x, maxPosition.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y + (mouseY * moveSpeed), minPosition.y, maxPosition.y);

        // HUD의 현재 위치를 목표 위치로 서서히 이동시키기
        hud.anchoredPosition = Vector2.Lerp(hud.anchoredPosition, targetPosition, Time.deltaTime * moveSpeed);
    }
}
