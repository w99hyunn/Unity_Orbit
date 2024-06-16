using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Analytics;
using Orbit_Character;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private Slider healthBar;
    [SerializeField]
    private Slider manaBar;
    [SerializeField]
    private Slider experienceBar; // 경험치 슬라이더 추가

    [SerializeField]
    private TMP_Text healthText;
    [SerializeField]
    private TMP_Text manaText;
    [SerializeField]
    private TMP_Text levelText; // 레벨 텍스트 추가

    [SerializeField]
    private CanvasGroup screenFlashCanvasGroup;

    [Header("레벨업")]
    [SerializeField]
    private AudioClip levelUpSound;
    [SerializeField]
    private GameObject[] levelUpPrefabs; // Level Up 시 나타날 프리팹들
    [SerializeField]
    private Transform[] levelUpPositions; // 프리팹이 나타날 위치들

    private int maxHealth = 100;
    private int maxMana = 100;
    private int currentHealth;
    private int currentMana;

    private int maxExperience = 100; // 최대 경험치 설정
    private int currentExperience = 0; // 현재 경험치
    private int level = 1; // 초기 레벨 설정

    private float manaRegenRate = 10f;
    private float healthRegenRate = 5f;
    private float regenInterval = 10f;

    private bool isFlashing = false;

    void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        healthBar.maxValue = maxHealth;
        manaBar.maxValue = maxMana;
        healthBar.value = currentHealth;
        manaBar.value = currentMana;
        experienceBar.maxValue = maxExperience; // 경험치 슬라이더 초기화
        experienceBar.value = currentExperience;
        UpdateHealthText();
        UpdateManaText();
        UpdateLevelText(); // 레벨 텍스트 업데이트

        InvokeRepeating("RegenerateMana", regenInterval, regenInterval);
        InvokeRepeating("RegenerateHealth", regenInterval, regenInterval);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            TakeDamage(20);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            UseMana(20);
        }
        if (Input.GetKeyDown(KeyCode.N)) // N키를 눌러 경험치 얻기 테스트
        {
            GainExperience(50);
        }

        if (currentHealth <= 0)
        {
            GameOver_();
        }
    }

    public void GameOver_()
    {
        GameManager.Instance.GameOver();
    }

    void RegenerateMana()
    {
        currentMana = Mathf.Min(currentMana + (int)manaRegenRate, maxMana);
        StartCoroutine(SmoothSliderChange(manaBar, currentMana));
        UpdateManaText();
    }

    void RegenerateHealth()
    {
        currentHealth = Mathf.Min(currentHealth + (int)healthRegenRate, maxHealth);
        StartCoroutine(SmoothSliderChange(healthBar, currentHealth));
        UpdateHealthText();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;
        StartCoroutine(SmoothSliderChange(healthBar, currentHealth));
        UpdateHealthText();
        // 체력 <= 0 death 추가 해야함
    }

    public void UseMana(int amount)
    {
        currentMana -= amount;
        if (currentMana < 0) currentMana = 0;
        StartCoroutine(SmoothSliderChange(manaBar, currentMana));
        UpdateManaText();
        // 마나 <= 0 스킬사용 x 추가
    }

    public void GainExperience(int amount)
    {
        currentExperience += amount;
        if (currentExperience >= maxExperience)
        {
            LevelUp();
        }
        StartCoroutine(SmoothSliderChange(experienceBar, currentExperience));
        UpdateExperienceText();
    }

    void LevelUp()
    {
        PlayerController.Instance.PlaySound(levelUpSound);
        level++;
        currentExperience -= maxExperience; // 남은 경험치는 다음 레벨로 이월
        StartCoroutine(SmoothSliderChange(experienceBar, currentExperience));
        UpdateLevelText();

        for (int i = 0; i < levelUpPrefabs.Length; i++)
        {
            if (i < levelUpPositions.Length)
            {
                StartCoroutine(FadeInAndOut(levelUpPrefabs[i], levelUpPositions[i]));
            }
        }
    }

    void UpdateHealthText()
    {
        healthText.text = currentHealth.ToString();
        if (currentHealth <= 20 && !isFlashing)
        {
            StartCoroutine(FlashScreen()); // 체력이 30 미만이면 화면 깜빡임 시작
        }
    }

    void UpdateManaText()
    {
        manaText.text = currentMana.ToString();
    }

    void UpdateExperienceText()
    {
        experienceBar.value = currentExperience;
    }

    void UpdateLevelText()
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

        // Fade In
        while (Time.time < startTime + fadeInDuration)
        {
            float t = (Time.time - startTime) / fadeInDuration;
            screenFlashCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        // Wait
        yield return new WaitForSeconds(flashDuration - fadeInDuration - fadeOutDuration);

        startTime = Time.time;

        // Fade Out
        while (Time.time < startTime + fadeOutDuration)
        {
            float t = (Time.time - startTime) / fadeOutDuration;
            screenFlashCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        screenFlashCanvasGroup.alpha = 0f;
        isFlashing = false;
    }

    private IEnumerator FadeInAndOut(GameObject prefab, Transform position)
    {
        // 파티클 시스템 인스턴스화
        GameObject instance = Instantiate(prefab, position.position, Quaternion.identity);
        ParticleSystem particleSystem = instance.GetComponent<ParticleSystem>();
        if (particleSystem == null)
        {
            Debug.LogError("Prefab does not have a ParticleSystem component");
            yield break;
        }

        // 페이드 인/아웃 시간 설정
        float fadeInDuration = 1f;
        float fadeOutDuration = 1f;
        float displayDuration = 3f;

        // 페이드 인
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            var main = particleSystem.main;
            main.startColor = new Color(main.startColor.color.r, main.startColor.color.g, main.startColor.color.b, Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 디스플레이 기간 대기
        yield return new WaitForSeconds(displayDuration);

        // 페이드 아웃
        elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            var main = particleSystem.main;
            main.startColor = new Color(main.startColor.color.r, main.startColor.color.g, main.startColor.color.b, Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 파티클 시스템 제거
        Destroy(instance);
    }


}
