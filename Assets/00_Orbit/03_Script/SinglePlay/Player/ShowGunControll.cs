using Demo.Scripts.Runtime.Character;
using KINEMATION.FPSAnimationFramework.Runtime.Recoil;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace STARTING
{
    public class ShowGunControll : MonoBehaviour
    {
        private GameObject Player;
        private RecoilAnimation recoil;
        private FPSController fpsController;
        public GameObject aimImage;
        public CanvasGroup hitAimImage;
        public TMP_Text fireMode;

        public Image weaponBase;
        public Sprite weaponMK18;
        public Sprite weaponAK12;
        public Sprite weaponAK74;
        public Sprite weaponPistol;
        public Sprite weaponFAL;

        private Coroutine currentCoroutine;

        private void Start()
        {
            Player = GameObject.FindWithTag("Player");
            recoil = Player.GetComponent<RecoilAnimation>();
            fpsController = Player.GetComponent<FPSController>();

            // 무기 변경 상태와 에임 변경 상태 이벤트 구독
            fpsController.OnActiveWeaponIndexChanged += ChangeWeapon;
            fpsController.OnActiveAiming += ChangeAimState;
        }

        void OnEnable()
        {
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

            // 새로운 코루틴 시작
            currentCoroutine = StartCoroutine(FadeHitAim());
        }

        // 0에서 1로 페이드하고 다시 1에서 0으로 페이드하는 코루틴
        private IEnumerator FadeHitAim()
        {
            // 알파 값을 0에서 1로 0.5초 동안 변경
            float duration = 0.2f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                hitAimImage.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
                yield return null; // 다음 프레임까지 대기
            }

            // 알파 값이 정확히 1로 맞춰지도록 설정
            hitAimImage.alpha = 1f;

            // 0.5초 대기
            yield return new WaitForSeconds(0.3f);

            // 알파 값을 1에서 0으로 0.5초 동안 변경
            elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                hitAimImage.alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
                yield return null;
            }

            // 알파 값이 정확히 0으로 맞춰지도록 설정
            hitAimImage.alpha = 0f;

            // 코루틴 종료 후 참조 초기화
            currentCoroutine = null;
        }
    }
}