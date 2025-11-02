using Demo.Scripts.Runtime.Character;
using KINEMATION.FPSAnimationFramework.Runtime.Recoil;
using TMPro;
using UnityEngine;
using System.Collections;

namespace STARTING
{
    public class GunControll : MonoBehaviour
    {
        private GameObject Player;
        private RecoilAnimation recoil;
        private FPSController fpsController;
        private AudioSource audioSource;

        public CanvasGroup hitAimImage;
        public TMP_Text fireMode;
        public AudioClip hitSound;

        private Coroutine _currentCoroutine;

        private void Start()
        {
            Player = GameObject.FindWithTag("Player");
            audioSource = Player.GetComponent<AudioSource>();
            recoil = Player.GetComponent<RecoilAnimation>();
            fpsController = Player.GetComponent<FPSController>();

            fpsController.OnActiveAiming += ChangeAimState;
            GameManager.Instance.OnEnemyHit += OnAnyEnemyHit;
        }

        void OnDisable()
        {
            GameManager.Instance.OnEnemyHit -= OnAnyEnemyHit;
        }

        void Update()
        {
            ShowFireMode();
        }

        private void ShowFireMode()
        {
            fireMode.text = recoil.fireMode.ToString().ToUpper();
        }

        private void ChangeAimState(FPSAimState aimState)
        {
            if (aimState == FPSAimState.None)
            {
                UIManager.Instance.ShowAim(true);
            }
            else if (aimState == FPSAimState.Aiming)
            {
                UIManager.Instance.ShowAim(false);
            }
        }

        void OnAnyEnemyHit()
        {
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
            }
            PlaySound(hitSound);
            _currentCoroutine = StartCoroutine(FadeHitAim());
        }

        public void PlaySound(AudioClip clip)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }

        public void StopSound()
        {
            audioSource.Stop();
        }

        private IEnumerator FadeHitAim()
        {
            float duration = 0.2f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                hitAimImage.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
                yield return null;
            }
            hitAimImage.alpha = 1f;

            yield return new WaitForSeconds(0.1f);

            elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                hitAimImage.alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
                yield return null;
            }

            hitAimImage.alpha = 0f;

            _currentCoroutine = null;
        }
    }
}