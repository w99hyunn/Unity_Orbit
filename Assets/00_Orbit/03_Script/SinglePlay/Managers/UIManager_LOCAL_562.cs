using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace STARTING
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        public TMP_Text timeText;

        public GameObject ZoneName;
        public GameObject lockBack;
        public GameObject unlockBack;
        public TMP_Text zoneNameText;
        public TMP_Text minimapZoneNameText;
        public TMP_Text liberatedText;

        public GameObject minimapLockBack;
        public GameObject minimapUnlockBack;

        public TMP_Text currentBulletText;
        public TMP_Text maxBulletText;

        [Header("우하단 키가이드")]
        public GameObject tipKey;
        private TMP_Text tipText;
        private TMP_Text tipKeyText;
        private Animator tipKeyAnimator;

        [Header("상호작용 키가이드")]
        public GameObject interactionKey;
        private TMP_Text interactionText;
        private TMP_Text interactionKeyText;
        private Animator interactionKeyAnimator;

        [Header("몬스터 처치 알림")]
        public GameObject killLog;
        public AudioClip killLogSound;
        private Animator killLogAnimator;
        private Image back1;
        private Image back2;
        private TMP_Text killLogTitle;
        private TMP_Text killLogText;
        private Image killLogIcon;
        private Sprite defaultIcon;

        [Header("스크립트 텍스트")]
        public GameObject scriptText;

        [Header("플레이어 스탯")]
        public Slider healthBar;
        private Image fillImage; // 슬라이더 내부 Bar의 이미지
        private Color _originalColor; // 원래의 색상을 저장하기 위한 변수
        private int _loopCount = Mathf.CeilToInt(1f / (0.1f * 2)); // 왕복하는 데 걸리는 시간을 고려하여 반복 횟수 계산

        public Slider manaBar;
        public TMP_Text healthText;
        public TMP_Text manaText;
        public TMP_Text levelText;
        public TMP_Text xpText;
        public CanvasGroup screenFlashCanvasGroup;
        public CanvasGroup levelUpHpPlusAlert;
        public CanvasGroup levelUpMpPlusAlert;

        [Header("게임오버시")]
        public UnityEvent onGameover;

        [Header("던전 관련")]
        public Text loadingText;
        public TMP_Text text1;
        public TMP_Text text2;
        public TMP_Text text3;
        public UnityEvent onDungeonEnter;
        public UnityEvent onDungeonLoadingComplete;

        [Header("캐릭터 상세 Info")]
        public GameObject infoUI;
        private Animator infoUiAnimator;
        public TMP_Text currentChipText;
        public TMP_Text xpDetailText;
        public TMP_Text healthDetailText;
        public TMP_Text manaDetailText;

        [Header("무기 변경 UI")]
        public UnityEvent changeWeaponUI;

        private CancellationTokenSource _deactivateZoneNameCancellationTokenSource;
        private CancellationTokenSource _scriptTextCancellationTokenSource;
        private CancellationTokenSource _killLogCancellationTokenSource;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            infoUiAnimator = infoUI.GetComponent<Animator>();
            killLogAnimator = killLog.GetComponent<Animator>();
            interactionKeyAnimator = interactionKey.GetComponent<Animator>();
            tipKeyAnimator = tipKey.GetComponent<Animator>();

            fillImage = healthBar.fillRect.GetComponent<Image>();

            tipText = tipKey.transform.Find("tipText").gameObject.GetComponent<TMP_Text>();
            tipKeyText = tipKey.transform.Find("tipkeyText").gameObject.GetComponent<TMP_Text>();
            interactionText = interactionKey.transform.Find("interactionText").gameObject.GetComponent<TMP_Text>();
            interactionKeyText = interactionKey.transform.Find("interactionKeyText").gameObject.GetComponent<TMP_Text>();
            killLogTitle = killLog.transform.Find("killLogTitle").gameObject.GetComponent<TMP_Text>();
            killLogText = killLog.transform.Find("killLogText").gameObject.GetComponent<TMP_Text>();
            back1 = killLog.transform.Find("Back1").gameObject.GetComponent<Image>();
            back2 = killLog.transform.Find("Back2").gameObject.GetComponent<Image>();
            killLogIcon = killLog.transform.Find("killLogIcon").gameObject.GetComponent<Image>();
        }
        
        private void Start()
        {
            defaultIcon = killLogIcon.sprite;
            _originalColor = fillImage.color; // 원래 색상을 저장

            minimapUnlockBack.transform.DORotate(new Vector3(0, 0, -360), 20f, RotateMode.FastBeyond360)
                 .SetLoops(-1, LoopType.Incremental) // 무한 반복
                 .SetEase(Ease.Linear); // 일정한 속도로 회전
            minimapLockBack.transform.DORotate(new Vector3(0, 0, 360), 60f, RotateMode.FastBeyond360)
                 .SetLoops(-1, LoopType.Incremental) // 무한 반복
                 .SetEase(Ease.Linear); // 일정한 속도로 회전
        }

        public void DungeonLoading(string t1, string t2, string t3)
        {
            onDungeonEnter.Invoke();

            loadingText.DOText("NOW LOADING", 2, true, ScrambleMode.All).SetDelay(0.5f);
            text1.text = t1;
            text2.text = t2;
            text3.text = t3;
        }

        public void DungeonLoadingComplete()
        {
            onDungeonLoadingComplete.Invoke();
        }

        public async UniTaskVoid ShowKillLog(string text, string title = "처치", float time = 3f, string backgroundColor = "red", Sprite icon = null)
        {
            Color red = new Color(251f / 255f, 92f / 255f, 87f / 255f, 100f / 255f);
            Color blue = new Color(21f / 255f, 184f / 255f, 198f / 255f, 100f / 255f);
            Color purple = new Color(249f / 255f, 87f / 255f, 251f / 255f, 100f / 255f);

            // 배경색 설정
            switch (backgroundColor)
            {
                case "red":
                    back1.color = red;
                    back2.color = red;
                    break;
                case "blue":
                    back1.color = blue;
                    back2.color = blue;
                    break;
                case "purple":
                    back1.color = purple;
                    back2.color = purple;
                    break;
            }

            killLogTitle.text = title;
            killLogText.text = text;

            killLogIcon.sprite = icon != null ? icon : defaultIcon;

            if (_killLogCancellationTokenSource != null)
            {
                _killLogCancellationTokenSource.Cancel();
                _killLogCancellationTokenSource.Dispose();
            }

            GameManager.Instance.PlaySound(killLogSound);

            _killLogCancellationTokenSource = new CancellationTokenSource();
            await ShowAndHideKillLog(time, _killLogCancellationTokenSource.Token);
        }

        public async UniTask ShowAndHideKillLog(float time, CancellationToken cancellationToken)
        {
            killLogAnimator.Play("Window In");

            await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: cancellationToken);

            killLogAnimator.Play("Window Out");

            _killLogCancellationTokenSource = null;
        }

        public async UniTask ShowTipKey()
        {
            tipKeyEnable("상세정보", "TAB");
            await UniTask.Delay(TimeSpan.FromSeconds(3f));
            tipKeyDisable();
        }

        public void interactionKeyEnable(string title, string key)
        {
            interactionKeyAnimator.Play("Window In");
            interactionText.text = title;
            interactionKeyText.text = key;
        }

        public void interactionKeyDisable()
        {
            interactionKeyAnimator.Play("Window Out");
        }

        public void tipKeyEnable(string title, string key)
        {
            tipKeyAnimator.Play("Window In");
            tipText.text = title;
            tipKeyText.text = key;
        }

        public void tipKeyDisable()
        {
            tipKeyAnimator.Play("Window Out");
        }

        public void OpenChangeWeapon()
        {
            changeWeaponUI?.Invoke();
        }

        public async UniTaskVoid UpdateZoneInfo(string zoneName, bool isLiberated)
        {
            ZoneName.SetActive(false);

            minimapZoneNameText.text = zoneName;
            zoneNameText.text = zoneName;

            // 해방됨
            if (isLiberated)
            {
                unlockBack.SetActive(true);
                lockBack.SetActive(false);
                timeText.color = new Color(21f / 255f, 184f / 255f, 198f / 255f);

                minimapUnlockBack.GetComponent<Image>().DOFade(1, 1f);
                minimapLockBack.GetComponent<Image>().DOFade(0, 1f);
            }
            // 해방 안됨
            else
            {
                unlockBack.SetActive(false);
                lockBack.SetActive(true);
                timeText.color = new Color(251f / 255f, 92f / 255f, 87f / 255f);

                minimapUnlockBack.GetComponent<Image>().DOFade(0, 1f);
                minimapLockBack.GetComponent<Image>().DOFade(1, 1f);
            }

            liberatedText.text = isLiberated ? "해방됨" : "해방되지 않음";
            ZoneName.SetActive(true);

            if (_deactivateZoneNameCancellationTokenSource != null)
            {
                _deactivateZoneNameCancellationTokenSource.Cancel();
                _deactivateZoneNameCancellationTokenSource.Dispose();
            }

            _deactivateZoneNameCancellationTokenSource = new CancellationTokenSource();
            await DeactivateZoneNameAfterDelay(6f, _deactivateZoneNameCancellationTokenSource.Token);
        }

        private async UniTask DeactivateZoneNameAfterDelay(float delay, CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: cancellationToken);
            ZoneName.SetActive(false);
        }

        public async UniTaskVoid ScriptText_Enable(string text)
        {
            scriptText.SetActive(false);

            scriptText.GetComponent<TMP_Text>().text = text;
            scriptText.SetActive(true);


            if (_scriptTextCancellationTokenSource != null)
            {
                _scriptTextCancellationTokenSource.Cancel();
                _scriptTextCancellationTokenSource.Dispose();
            }

            _scriptTextCancellationTokenSource = new CancellationTokenSource();
            await DeactivateScriptAfterDelay(6f, _scriptTextCancellationTokenSource.Token);
        }

        private async UniTask DeactivateScriptAfterDelay(float delay, CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: cancellationToken);
            scriptText.SetActive(false);
        }

        public void ScriptText_Disable()
        {
            scriptText.SetActive(false);
        }

        public void GameOverUI()
        {
            onGameover.Invoke();
        }

        public void UpdateTime(string time)
        {
            timeText.text = time;
        }

        public void UpdateCurrentBullet(int current)
        {
            currentBulletText.text = current.ToString();
        }

        public void UpdateMaxBullet(int max)
        {
            maxBulletText.text = max.ToString();
        }

        public void UpdateStats(string order, int currentindex, int maxindex = 0)
        {
            switch (order)
            {
                case "health":
                    UpdateHealthUI(currentindex, maxindex);
                    break;
                case "mana":
                    UpdateManaUI(currentindex, maxindex);
                    break;
                case "exp":
                    UpdateExperienceUI(currentindex, maxindex);
                    break;
                case "level":
                    UpdateLevelUI(currentindex);
                    break;
                case "chip":
                    UpdateChipUI(currentindex);
                    break;
            }
        }

        private void UpdateChipUI(int currentindex)
        {
            currentChipText.text = $"{currentindex}";
        }

        private void UpdateHealthUI(int currentindex, int maxindex)
        {
            int currentHealthPercentage = (int)((float)currentindex / maxindex * 100);

            healthText.text = currentHealthPercentage.ToString();
            healthDetailText.text = $"{currentindex} / {maxindex}";
            StartCoroutine(SmoothSliderChange(healthBar, currentHealthPercentage));

            //체력이 20 미만이면 화면 깜빡임 시작
            //추후 Vignette 효과로 대체하면 될듯함
            //if (currentHealthPercentage <= 20 && !isFlashing)
            //{
            //    StartCoroutine(FlashScreen());
            //}
            //피격시마다 뜨게 변경
        }

        public void StartBlinking()
        {
            fillImage.DOColor(new Color(251f / 255f, 92f / 255f, 87f / 255f), 0.1f)
                     .SetLoops(_loopCount, LoopType.Yoyo) // 2초 동안 반복
                     .SetEase(Ease.Linear)
                     .OnComplete(() => fillImage.color = _originalColor);
        }

        private void UpdateManaUI(int currentindex, int maxindex)
        {
            int currentManaPercentage = (int)((float)currentindex / maxindex * 100);

            manaText.text = currentManaPercentage.ToString();
            manaDetailText.text = $"{currentindex} / {maxindex}";
            StartCoroutine(SmoothSliderChange(manaBar, currentManaPercentage));
        }

        private void UpdateExperienceUI(int currentindex, int maxindex)
        {
            int currentExperiencePercentage = (int)((float)currentindex / maxindex * 100);
            xpDetailText.text = $"{currentindex} / {maxindex}";
            xpText.text = currentExperiencePercentage.ToString();
        }

        private void UpdateLevelUI(int level)
        {
            levelText.text = level.ToString();
        }

        public void LevelUpStatPlusAlert()
        {
            StartCoroutine(FadeCanvasGroup(levelUpHpPlusAlert));
            StartCoroutine(FadeCanvasGroup(levelUpMpPlusAlert));
        }

        public void InfoUI(bool index)
        {
            if (true == index)
            {
                infoUiAnimator.Play("Window In");
            }
            else
            {
                infoUiAnimator.Play("Window Out");
            }
        }

        private IEnumerator SmoothSliderChange(Slider slider, float targetValue)
        {
            float elapsedTime = 0f;
            float duration = 0.5f;
            float startValue = slider.value;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                slider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
                yield return null;
            }

            slider.value = targetValue;
        }

        public IEnumerator FlashScreen()
        {
            StopCoroutine(FlashScreen());
            StartBlinking();

            float flashDuration = 2f;
            float fadeInDuration = 0.5f;
            float fadeOutDuration = 0.5f;

            float startTime = Time.time;

            while (Time.time < startTime + fadeInDuration)
            {
                float t = (Time.time - startTime) / fadeInDuration;
                screenFlashCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }

            yield return new WaitForSeconds(flashDuration - fadeInDuration - fadeOutDuration);

            startTime = Time.time;

            while (Time.time < startTime + fadeOutDuration)
            {
                float t = (Time.time - startTime) / fadeOutDuration;
                screenFlashCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }

            screenFlashCanvasGroup.alpha = 0f;
        }

        IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup)
        {
            StopCoroutine(FadeIn(canvasGroup));


            yield return StartCoroutine(FadeIn(canvasGroup));

            yield return new WaitForSeconds(5f);

            yield return StartCoroutine(FadeOut(canvasGroup));
        }

        IEnumerator FadeIn(CanvasGroup canvasGroup)
        {
            float startAlpha = canvasGroup.alpha;
            float elapsedTime = 0f;

            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, elapsedTime / 1f);
                yield return null;
            }

            canvasGroup.alpha = 1f;
        }

        IEnumerator FadeOut(CanvasGroup canvasGroup)
        {
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