using Mirror;
using System.Collections;
using UnityEngine;

namespace STARTING
{
    public class EnemyUI_Multi : MonoBehaviour
    {
        private Camera mainCamera;
        private CanvasGroup canvasGroup;
        private Coroutine _fadeCoroutine;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

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
        }

        public void OnDisable()
        {
            canvasGroup.alpha = 0;
        }

        void Update()
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
        }

        public void ShowCanvasGroup()
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
            canvasGroup.alpha = 1;

            _fadeCoroutine = StartCoroutine(FadeOutAfterDelay(3f));
        }

        private IEnumerator FadeOutAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            float startAlpha = canvasGroup.alpha;
            float elapsedTime = 0f;

            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0.1f, elapsedTime / 1f);
                yield return null;
            }

            canvasGroup.alpha = 0.1f;
        }
    }
}