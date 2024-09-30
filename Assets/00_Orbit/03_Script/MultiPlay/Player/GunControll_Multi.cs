using Demo.Scripts.Runtime.Character;
using KINEMATION.FPSAnimationFramework.Runtime.Recoil;
using Mirror;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace STARTING
{
    public class GunControll_Multi : NetworkBehaviour
    {
        private GameObject Player;
        private RecoilAnimation recoil;
        private FPSController fpsController;
        private AudioSource audioSource;

        public GameObject aimImage;
        public CanvasGroup hitAimImage;
        public TMP_Text fireMode;

        public AudioClip hitSound;

        public Image weaponBase;
        public Sprite weaponMK18;
        public Sprite weaponAK12;
        public Sprite weaponAK74;
        public Sprite weaponPistol;
        public Sprite weaponFAL;

        private Coroutine currentCoroutine;

        private void Start()
        {
            if (isLocalPlayer)
            {
                Player = GameObject.FindWithTag("Player");
                audioSource = Player.GetComponent<AudioSource>();
                recoil = Player.GetComponent<RecoilAnimation>();
                fpsController = Player.GetComponent<FPSController>();

                // 무기 변경 상태와 에임 변경 상태 이벤트 구독
                fpsController.OnActiveWeaponIndexChanged += ChangeWeapon;
                fpsController.OnActiveAiming += ChangeAimState;
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

        private void ChangeAimState(FPSAimState aimState)
        {
            if (aimState == FPSAimState.Aiming)
            {
                aimImage.SetActive(true);
            }
            else if (aimState == FPSAimState.None)
            {
                aimImage.SetActive(false);
            }
        }

        private void ChangeWeapon(int index)
        {
            switch (index)
            {
                case 0:
                    weaponBase.sprite = weaponMK18;
                    break;
                case 1:
                    weaponBase.sprite = weaponAK12;
                    break;
                case 2:
                    weaponBase.sprite = weaponAK74;
                    break;
                case 3:
                    weaponBase.sprite = weaponPistol;
                    break;
                case 4:
                    weaponBase.sprite = weaponFAL;
                    break;
            }
        }
        void OnAnyEnemyHit()
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            PlaySound(hitSound);
            currentCoroutine = StartCoroutine(FadeHitAim());
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

            currentCoroutine = null;
        }
    }
}