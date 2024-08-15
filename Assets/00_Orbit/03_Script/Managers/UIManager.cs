using Michsky.UI.Shift;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public TMP_Text timeText;

    public GameObject ZoneName;
    public GameObject lockBack;
    public GameObject unlockBack;
    public TMP_Text zoneNameText; // 변수명 변경
    public TMP_Text minimapZoneNameText; // 변수명 변경
    public TMP_Text liberatedText; // 변수명 변경

    public TMP_Text currentBulletText;
    public TMP_Text maxBulletText;

    [Header("팁가이드")]
    public GameObject TipKey;
    private TMP_Text TipText;
    private TMP_Text TipKeys;

    [Header("스크립트 텍스트")]
    public GameObject scriptText;

    public Light sun; // SUN 오브젝트

    [Header("플레이어 스탯")]
    public Slider healthBar;
    public Slider manaBar;

    public TMP_Text healthText;
    public TMP_Text manaText;
    public TMP_Text levelText;
    public TMP_Text xpText;

    public CanvasGroup screenFlashCanvasGroup;
    private bool isFlashing = false;

    public UnityEvent onGameover;

    private void Start()
    {
        Transform TipTextTransform = TipKey.transform.Find("TipText");
        TipText = TipTextTransform.gameObject.GetComponent<TMP_Text>();

        Transform TipKeysTransform = TipKey.transform.Find("Tipkey");
        TipKeys = TipKeysTransform.gameObject.GetComponent<TMP_Text>();

        GameObject lightObject = GameObject.Find("Directional Light");
    }

    public void TipKey_Enable(string Title, string Key)
    {
        TipKey.SetActive(true);
        TipText.text = Title;
        TipKeys.text = Key;
    }

    public void TipKey_Disable()
    {
        TipKey.SetActive(false);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /* 존 이름 & 해방여부 업데이트 */
    private Coroutine deactivateCoroutine;

    public void UpdateZoneInfo(string zoneName, bool isLiberated)
    {
        ZoneName.SetActive(false);

        minimapZoneNameText.text = zoneName;
        zoneNameText.text = zoneName;

        if (isLiberated)
        { //해방됨
            unlockBack.SetActive(true);
            lockBack.SetActive(false);
        }
        else
        {
            unlockBack.SetActive(false);
            lockBack.SetActive(true);
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

    public void UpdateStats(string order, int index)
    {
        switch (order)
        {
            case "health":
                UpdateHealthUI(index);
                break;
            case "mana":
                UpdateManaUI(index);
                break;
            case "exp":
                UpdateExperienceUI(index);
                break;
            case "level":
                UpdateLevelUI(index);
                break;
        }
    }

    private void UpdateHealthUI(int currentHealthPercentage)
    {
        healthText.text = currentHealthPercentage.ToString();
        StartCoroutine(SmoothSliderChange(healthBar, currentHealthPercentage));

        //체력이 20 미만이면 화면 깜빡임 시작
        //추후 Vignette 효과로 대체하면 될듯함
        if (currentHealthPercentage <= 20 && !isFlashing)
        {
            StartCoroutine(FlashScreen());
        }
    }
    private void UpdateManaUI(int currentManaPercentage)
    {
        manaText.text = currentManaPercentage.ToString();
        StartCoroutine(SmoothSliderChange(manaBar, currentManaPercentage));
    }

    private void UpdateExperienceUI(int currentExperience)
    {
        xpText.text = currentExperience.ToString();
    }

    private void UpdateLevelUI(int level)
    {
        levelText.text = level.ToString();
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

    private IEnumerator FlashScreen()
    {
        isFlashing = true;

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
        isFlashing = false;
    }
}
