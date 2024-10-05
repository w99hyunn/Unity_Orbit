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
        private GameObject player;
        private RecoilAnimation recoil;
        private FPSController_Multi fpsController;
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
            StartCoroutine(FindLocalPlayer());
        }

        private IEnumerator FindLocalPlayer()
        {
            while (NetworkClient.localPlayer == null)
            {
                yield return null;
            }

            player = GameManager_Multi.Instance.player;
            Debug.Log(player.name);
            if (player != null)
            {
                audioSource = player.GetComponent<AudioSource>();
                recoil = player.GetComponent<RecoilAnimation>();
                fpsController = player.GetComponent<FPSController_Multi>();

                // 무기 변경 상태와 에임 변경 상태 이벤트 구독
                fpsController.OnActiveWeaponIndexChanged += ChangeWeapon;
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
                aimImage.SetActive(true);
            }
            else if (aimState == FPSAimState_Multi.None)
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