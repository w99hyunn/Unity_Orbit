using System.Collections;
using UnityEngine;

namespace STARTING
{
    public class EnemyUI : MonoBehaviour
    {
        private Camera mainCamera;
        private CanvasGroup canvasGroup;

        private Coroutine _fadeCoroutine;

        void Awake()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            canvasGroup = GetComponent<CanvasGroup>();
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
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / 1f);
                yield return null;
            }

            canvasGroup.alpha = 0f;
        }
    }
}