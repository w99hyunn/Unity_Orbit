using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

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

        [Header("상호작용 키가이드")]
        public GameObject interactionKey;
        private TMP_Text interactionText;
        private TMP_Text interactionKeyText;

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
        private Color originalColor; // 원래의 색상을 저장하기 위한 변수
        private int loopCount = Mathf.CeilToInt(1f / (0.1f * 2)); // 왕복하는 데 걸리는 시간을 고려하여 반복 횟수 계산

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
        private UIInfoUpdate uIInfoUpdate;
        private Animator infoUiAnimator;

        /* 존 이름 & 해방여부 업데이트 */
        private Coroutine deactivateCoroutine;
        private Coroutine killLogCoroutine;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        
        private void Start()
        {
            tipText = tipKey.transform.Find("tipText").gameObject.GetComponent<TMP_Text>();
            tipKeyText = tipKey.transform.Find("tipkeyText").gameObject.GetComponent<TMP_Text>();
            interactionText = interactionKey.transform.Find("interactionText").gameObject.GetComponent<TMP_Text>();
            interactionKeyText = interactionKey.transform.Find("interactionKeyText").gameObject.GetComponent<TMP_Text>();
            killLogTitle = killLog.transform.Find("killLogTitle").gameObject.GetComponent<TMP_Text>();
            killLogText = killLog.transform.Find("killLogText").gameObject.GetComponent<TMP_Text>();
            back1 = killLog.transform.Find("Back1").gameObject.GetComponent<Image>();
            back2 = killLog.transform.Find("Back2").gameObject.GetComponent<Image>();
            killLogIcon = killLog.transform.Find("killLogIcon").gameObject.GetComponent<Image>();
            killLogAnimator = killLog.GetComponent<Animator>();
            defaultIcon = killLogIcon.sprite;

            uIInfoUpdate = infoUI.GetComponent<UIInfoUpdate>();
            infoUiAnimator = infoUI.GetComponent<Animator>();

            minimapUnlockBack.transform.DORotate(new Vector3(0, 0, -360), 20f, RotateMode.FastBeyond360)
                 .SetLoops(-1, LoopType.Incremental) // 무한 반복
                 .SetEase(Ease.Linear); // 일정한 속도로 회전
            minimapLockBack.transform.DORotate(new Vector3(0, 0, 360), 60f, RotateMode.FastBeyond360)
                 .SetLoops(-1, LoopType.Incremental) // 무한 반복
                 .SetEase(Ease.Linear); // 일정한 속도로 회전

            fillImage = healthBar.fillRect.GetComponent<Image>();
            originalColor = fillImage.color; // 원래 색상을 저장
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

        public void ShowKillLog(string text, string title = "처치", float time = 3f, string backgroundColor = "red", Sprite icon = null)
        {
            Color red = new Color(251f / 255f, 92f / 255f, 87f / 255f, 100f / 255f);
            Color blue = new Color(21f / 255f, 184f / 255f, 198f / 255f, 100f / 255f);
            Color purple = new Color(249f / 255f, 87f / 255f, 251f / 255f, 100f / 255f);

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

            if (icon != null)
            {
                killLogIcon.sprite = icon;
            }
            else
            {
                killLogIcon.sprite = defaultIcon;
            }

            if (killLogCoroutine != null)
            {
                StopCoroutine(killLogCoroutine);
            }

            GameManager.Instance.PlaySound(killLogSound);
            killLogCoroutine = StartCoroutine(ShowAndHideKillLog(time));
        }

        public IEnumerator ShowAndHideKillLog(float time)
        {
            killLogAnimator.Play("Window In");

            yield return new WaitForSeconds(time);

            killLogAnimator.Play("Window Out");

            killLogCoroutine = null;
        }

        public IEnumerator ChipLog(float time)
        {
            yield return new WaitForSeconds(time);
            Inventory.Instance.GetChip();
        }

        public void interactionKeyEnable(string title, string key)
        {
            interactionKey.SetActive(true);
            interactionText.text = title;
            interactionKeyText.text = key;
        }

        public void interactionKeyDisable()
        {
            interactionKey.SetActive(false);
        }

        public void tipKeyEnable(string title, string key)
        {
            tipKey.SetActive(true);
            tipText.text = title;
            tipKeyText.text = key;
        }

        public void tipKeyDisable()
        {
            tipKey.SetActive(false);
        }

        public void UpdateZoneInfo(string zoneName, bool isLiberated)
        {
            ZoneName.SetActive(false);

            minimapZoneNameText.text = zoneName;
            zoneNameText.text = zoneName;

            //해방됨
            if (isLiberated)
            {
                unlockBack.SetActive(true);
                lockBack.SetActive(false);
                timeText.color = new Color(21f / 255f, 184f / 255f, 198f / 255f);

                minimapUnlockBack.GetComponent<Image>().DOFade(1, 1f);
                minimapLockBack.GetComponent<Image>().DOFade(0, 1f);
                //minimapUnlockBack.SetActive(true);
                //minimapLockBack.SetActive(false);
            }
            //해방안됨
            else
            {
                unlockBack.SetActive(false);
                lockBack.SetActive(true);
                timeText.color = new Color(251f / 255f, 92f / 255f, 87f / 255f);

                minimapUnlockBack.GetComponent<Image>().DOFade(0, 1f);
                minimapLockBack.GetComponent<Image>().DOFade(1, 1f);
                //minimapUnlockBack.SetActive(false);
                //minimapLockBack.SetActive(true);
            }

            liberatedText.text = isLiberated ? "해방됨" : "해방되지 않음";
            ZoneName.SetActive(true);

            // 기존 코루틴이 있으면 중지
            if (deactivateCoroutine != null)
            {
                StopCoroutine(deactivateCoroutine);
            }

            // 새로운 코루틴 시작
            deactivateCoroutine = StartCoroutine(DeactivateZoneNameAfterDelay(6f));
        }

        private IEnumerator DeactivateZoneNameAfterDelay(float delay)
        {
            // delay 동안 대기
            yield return new WaitForSeconds(delay);
            // ZoneName 비활성화
            ZoneName.SetActive(false);
        }

        /* 가운데 하단 스크립트 텍스트 */
        private Coroutine deactivateScriptCoroutine;

        public void ScriptText_Enable(string text)
        {
            scriptText.SetActive(false);

            scriptText.GetComponent<TMP_Text>().text = text;
            scriptText.SetActive(true);

            if (deactivateScriptCoroutine != null)
            {
                StopCoroutine(deactivateScriptCoroutine);
            }
            deactivateScriptCoroutine = StartCoroutine(DeactivateScriptAfterDelay(6f));
        }

        private IEnumerator DeactivateScriptAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
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
            uIInfoUpdate.SetDetailText("chip", $"{currentindex}");
        }

        private void UpdateHealthUI(int currentindex, int maxindex)
        {
            int currentHealthPercentage = (int)((float)currentindex / maxindex * 100);

            healthText.text = currentHealthPercentage.ToString();
            uIInfoUpdate.SetDetailText("health", $"{currentindex} / {maxindex}");
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
                     .SetLoops(loopCount, LoopType.Yoyo) // 2초 동안 반복
                     .SetEase(Ease.Linear)
                     .OnComplete(() => fillImage.color = originalColor);
        }

        private void UpdateManaUI(int currentindex, int maxindex)
        {
            int currentManaPercentage = (int)((float)currentindex / maxindex * 100);

            manaText.text = currentManaPercentage.ToString();
            uIInfoUpdate.SetDetailText("mana", $"{currentindex} / {maxindex}");
            StartCoroutine(SmoothSliderChange(manaBar, currentManaPercentage));
        }

        private void UpdateExperienceUI(int currentindex, int maxindex)
        {
            int currentExperiencePercentage = (int)((float)currentindex / maxindex * 100);

            uIInfoUpdate.SetDetailText("exp", $"{currentindex} / {maxindex}");
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