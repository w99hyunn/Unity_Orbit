using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Analytics;

public class PlayerStats : MonoBehaviour
{
    private int maxHealth = 100;
    private int maxMana = 100;
    private int maxExperience = 100;
    public int currentHealth { get; private set; }
    public int currentMana { get; private set; }
    public int currentExperience { get; private set; } = 0;
    public int level { get; private set; } = 1;

    private float manaRegenRate = 10f;
    private float healthRegenRate = 5f;
    private float regenInterval = 10f;

    public static event System.Action OnPlayerStatsInitialized;

    void Start()
    {
        InvokeRepeating("RegenerateMana", regenInterval, regenInterval);
        InvokeRepeating("RegenerateHealth", regenInterval, regenInterval);

        InitializeStats();
        OnPlayerStatsInitialized?.Invoke(); // 초기화 완료 이벤트 호출
    }
    public void SetStats(int health, int mana, int experience, int level)
    {
        currentHealth = health;
        currentMana = mana;
        currentExperience = experience;
        this.level = level;

        UIManager.Instance.UpdateStats("health", currentHealth);
        UIManager.Instance.UpdateStats("mana", currentMana);
        UIManager.Instance.UpdateStats("exp", currentExperience);
        UIManager.Instance.UpdateStats("level", level);
    }

    public void InitializeStats()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentExperience = 0;
        level = 1;

        UIManager.Instance.UpdateStats("health", currentHealth);
        UIManager.Instance.UpdateStats("mana", currentMana);
        UIManager.Instance.UpdateStats("exp", currentHealth);
        UIManager.Instance.UpdateStats("level", level);
    }

    void RegenerateHealth()
    {
        currentHealth = Mathf.Min(currentHealth + (int)healthRegenRate, maxHealth);
        UIManager.Instance.UpdateStats("health", currentHealth);
    }

    void RegenerateMana()
    {
        currentMana = Mathf.Min(currentMana + (int)manaRegenRate, maxMana);
        UIManager.Instance.UpdateStats("mana", currentMana);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        UIManager.Instance.UpdateStats("health", currentHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GameManager.Instance.GameOver();
        }
        // 체력 <= 0 death 추가 해야함
    }

    public void UseMana(int amount)
    {
        currentMana -= amount;
        UIManager.Instance.UpdateStats("mana", currentMana);
        if (currentMana < 0) currentMana = 0;
        // 마나 <= 0 스킬사용 x 추가
    }

    public void GainExperience(int amount)
    {
        currentExperience += amount;
        if (currentExperience >= maxExperience)
        {
            LevelUp();
        }
        UIManager.Instance.UpdateStats("exp", currentExperience);
    }
    void LevelUp()
    {
        //0803 PlayerController.Instance.PlaySound(levelUpSound);
        level++;
        currentExperience -= maxExperience; // 남은 경험치는 다음 레벨로 이월
        UIManager.Instance.UpdateStats("level", level);
        UIManager.Instance.UpdateStats("exp", currentExperience);
    }
}
