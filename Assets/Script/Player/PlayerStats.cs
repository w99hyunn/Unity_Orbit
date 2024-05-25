using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private Slider healthBar;
    [SerializeField]
    private Slider manaBar;

    [SerializeField]
    private TMP_Text healthText;
    [SerializeField]
    private TMP_Text manaText;

    [SerializeField]
    private CanvasGroup screenFlashCanvasGroup;

    private int maxHealth = 100;
    private int maxMana = 100;
    private int currentHealth;
    private int currentMana;

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
        UpdateHealthText();
        UpdateManaText();

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
}
