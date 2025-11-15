using Michsky.UI.Shift;
using UnityEngine;

namespace NOLDA
{
    public class HUDMovement : MonoBehaviour
    {
        public RectTransform hud;
        public float moveSpeed = 1f;
        public Vector2 minPosition;
        public Vector2 maxPosition;
        private QualityManager qualityManager;

        private Vector2 _targetPosition;

        void Start()
        {
            qualityManager = FindAnyObjectByType<QualityManager>();
            _targetPosition = hud.anchoredPosition;
        }

        void Update()
        {
            if (false == qualityManager.isMovementUI)
            {
                return;
            }

            float mouseX = -(Input.GetAxis("Mouse X"));
            float mouseY = -(Input.GetAxis("Mouse Y"));

            _targetPosition.x = Mathf.Clamp(_targetPosition.x + (mouseX * moveSpeed), minPosition.x, maxPosition.x);
            _targetPosition.y = Mathf.Clamp(_targetPosition.y + (mouseY * moveSpeed), minPosition.y, maxPosition.y);

            // HUD의 현재 위치를 목표 위치로 서서히 이동시키기
            hud.anchoredPosition = Vector2.Lerp(hud.anchoredPosition, _targetPosition, Time.deltaTime * moveSpeed);
        }
    }
}