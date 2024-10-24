using Demo.Scripts.Runtime.Character;
using KINEMATION.FPSAnimationFramework.Runtime.Recoil;
using System.Collections;
using TMPro;
using UnityEngine;

namespace STARTING
{
    public class GunControll_Multi : MonoBehaviour
    {
        private GameObject player;
        private RecoilAnimation recoil;
        private FPSController_Multi fpsController;
        private AudioSource audioSource;


        public CanvasGroup hitAimImage;
        public TMP_Text fireMode;
        public AudioClip hitSound;

        private Coroutine _currentCoroutine;

        private void Start()
        {
            StartCoroutine(FindLocalPlayer());
        }

        private IEnumerator FindLocalPlayer()
        {
            while (GameManager_Multi.Instance.player == null)
            {
                yield return null;
            }

            player = GameManager_Multi.Instance.player;
            //Debug.Log(this.gameObject.name + player.name);
            if (player != null)
            {
                audioSource = player.GetComponent<AudioSource>();
                recoil = player.GetComponent<RecoilAnimation>();
                fpsController = player.GetComponent<FPSController_Multi>();

                fpsController.OnActiveAiming += ChangeAimState;
            }
            else
            {
                Debug.LogWarning("Player 못찾음. 할당 X");
            }
        }

        void OnEnable()
        {
            //GameManager_Multi.Instance.OnEnemyHit += OnAnyEnemyHit;
        }

        void OnDisable()
        {
            //GameManager_Multi.Instance.OnEnemyHit -= OnAnyEnemyHit;
        }

        void Update()
        {
            ShowFireMode();
        }

        private void ShowFireMode()
        {
            //fireMode.text = recoil.fireMode.ToString().ToUpper();
        }

        private void ChangeAimState(FPSAimState_Multi aimState)
        {
            if (aimState == FPSAimState_Multi.Aiming)
            {
                UIManager.Instance.ShowAim(false);
            }
            else if (aimState == FPSAimState_Multi.None)
            {
                UIManager.Instance.ShowAim(true);
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