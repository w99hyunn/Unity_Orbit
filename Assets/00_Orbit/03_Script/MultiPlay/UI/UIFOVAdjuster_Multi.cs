using UnityEngine;
using System.Collections;
using Demo.Scripts.Runtime.Character;
using Mirror;

namespace STARTING
{
    public class UIFOVAdjuster_Multi : MonoBehaviour
    {
        public Camera mainCamera;
        public Transform uiTransform;

        private FPSController_Multi _fpsController;
        private Coroutine _adjustCoroutine;
        private Coroutine _defaultCoroutine;

        private float _initialFOV;
        private float _initialZDistance;
        private float _lerpSpeed = 3f;

        private void Start()
        {
            StartCoroutine(FindLocalPlayer());
        }

        private IEnumerator FindLocalPlayer()
        {
            while (NetworkClient.localPlayer == null)
            {
                yield return null;
            }

            mainCamera = NetworkClient.localPlayer.gameObject.GetComponentInChildren<Camera>();

            _initialFOV = mainCamera.fieldOfView;
            uiTransform = this.transform;
            _initialZDistance = Vector3.Distance(mainCamera.transform.localPosition, uiTransform.localPosition);

            this.transform.SetParent(mainCamera.transform, false);

            _fpsController = NetworkClient.localPlayer.gameObject.GetComponent<FPSController_Multi>();
            _fpsController.OnActiveAiming += OnAimStateChanged;
        }

        // Aiming 상태가 변경될 때
        public void OnAimStateChanged(FPSAimState_Multi aimState)
        {
            if (aimState == FPSAimState_Multi.Aiming)
            {
                if (_adjustCoroutine == null)
                {
                    _adjustCoroutine = StartCoroutine(AdjustUIPositionCoroutine());
                }

                if (_defaultCoroutine != null)
                {
                    StopCoroutine(_defaultCoroutine);
                    _defaultCoroutine = null;
                }
            }
            else if (aimState == FPSAimState_Multi.None)
            {
                if (_adjustCoroutine != null)
                {
                    StopCoroutine(_adjustCoroutine);
                    _adjustCoroutine = null;
                }
                if (_defaultCoroutine != null)
                {
                    _defaultCoroutine = null;
                }

                _defaultCoroutine = StartCoroutine(AdjustUIPositionDefaultCoroutine());
            }
        }

        private IEnumerator AdjustUIPositionCoroutine()
        {
            while (true)
            {
                AdjustUIPosition();
                yield return null;
            }
        }

        private IEnumerator AdjustUIPositionDefaultCoroutine()
        {
            float elapsedTime = 0f;
            float duration = 2f;

            while (elapsedTime < duration)
            {
                AdjustUIPositionDefault();
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _defaultCoroutine = null;
        }

        private void AdjustUIPosition()
        {
            float fovRatio = _initialFOV / mainCamera.fieldOfView;
            Vector3 direction = (uiTransform.position - mainCamera.transform.position).normalized;
            float adjustedZDistance = _initialZDistance * fovRatio * 1.1f;

            // 카메라의 로컬 공간을 기준으로 위치를 조정
            Vector3 targetPosition = mainCamera.transform.position + direction * adjustedZDistance;

            uiTransform.position = Vector3.Lerp(uiTransform.position, targetPosition, Time.deltaTime * _lerpSpeed);
        }

        private void AdjustUIPositionDefault()
        {
            float fovRatio = _initialFOV / mainCamera.fieldOfView;
            Vector3 direction = (uiTransform.position - mainCamera.transform.position).normalized;
            float adjustedZDistance = _initialZDistance * fovRatio * 1.0f;

            // 카메라의 로컬 공간을 기준으로 위치를 조정
            Vector3 targetPosition = mainCamera.transform.position + direction * adjustedZDistance;

            uiTransform.position = Vector3.Lerp(uiTransform.position, targetPosition, Time.deltaTime * _lerpSpeed);
        }

    }
}