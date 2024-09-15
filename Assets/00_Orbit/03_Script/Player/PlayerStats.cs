using System;
using System.Collections;
using UnityEngine;

[Serializable]
public enum PlayerState
{
    IDLE,
    INIT,
    LOADING,
    DIE,
    PAUSE
};

public class PlayerStats : MonoBehaviour
{
    public AudioClip levelUpSound;
    public int maxHealth {get; private set;}
    public int maxMana { get; private set; }
    public int maxExperience { get; private set; }
    public int currentHealth { get; private set; }
    public int currentMana { get; private set; }
    public int currentExperience { get; private set; }
    public int level { get; private set; }

    private float manaRegenRate = 10f;
    private float healthRegenRate = 5f;
    private float regenInterval = 10f;

    [Header("플레이어 상태")]
    public PlayerState playerState;

    public static event System.Action OnPlayerStatsInitialized;
    public static PlayerStats Instance;

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

    void Start()
    {
        playerState = PlayerState.INIT;
        Debug.Log(PlayerStats.Instance.playerState);
        InvokeRepeating("RegenerateMana", regenInterval, regenInterval);
        InvokeRepeating("RegenerateHealth", regenInterval, regenInterval);
        InitializeStats();
        OnPlayerStatsInitialized?.Invoke(); // 초기화 완료 이벤트 호출
    }
    public void SetStats(int maxHealth, int maxMana, int maxExperience, int health, int mana, int experience, int level)
    {
        this.maxHealth = maxHealth;
        this.maxMana = maxMana;
        this.maxExperience = maxExperience;

        this.currentHealth = health;
        this.currentMana = mana;
        this.currentExperience = experience;

        this.level = level;

        //시작시 5초간 무적
        StartCoroutine(ChangePlayerState(5f, PlayerState.IDLE));

        UpdateUI();
    }

    public IEnumerator ChangePlayerState(float time, PlayerState playerState)
    {
        yield return new WaitForSeconds(time);
        if (this.playerState == PlayerState.INIT)
        {
            this.playerState = playerState;
        }
    }

    public void ChangeState(float seconds, PlayerState playerState)
    {
        StartCoroutine(ChangePlayerStateAbsolute(seconds, playerState));
    }

    public IEnumerator ChangePlayerStateAbsolute(float seconds, PlayerState playerState)
    {
        yield return new WaitForSeconds(seconds);
        this.playerState = playerState;
    }

    public void InitializeStats()
    {
        maxHealth = 100;
        maxMana = 100;
        maxExperience = 100;

        currentHealth = 100;
        currentMana = 100;
        currentExperience = 0;

        level = 1;

        StartCoroutine(ChangePlayerState(5f, PlayerState.IDLE));
        UpdateUI();
    }

    public void Healing(int index)
    {
        currentHealth = Mathf.Min(currentHealth + index, maxHealth);
        UpdateUI();
    }

    void RegenerateHealth()
    {
        currentHealth = Mathf.Min(currentHealth + (int)healthRegenRate, maxHealth);
        UpdateUI();
    }

    void RegenerateMana()
    {
        currentMana = Mathf.Min(currentMana + (int)manaRegenRate, maxMana);
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        if (amount == -1)
        {
            currentHealth -= maxHealth;
        }
        else
        {
            currentHealth -= amount;
        }
        //UIManager.Instance.hudMovement.Shake();
        StartCoroutine(UIManager.Instance.FlashScreen());
        UpdateUI();

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
        if (currentMana < 0) currentMana = 0;
        UpdateUI();

        // 마나 <= 0 스킬사용 x 추가
    }

    public void GainExperience(int amount)
    {
        currentExperience += amount;
        if (currentExperience >= maxExperience)
        {
            LevelUp();
        }
        UpdateUI();
    }
    void LevelUp()
    {
        GameManager.Instance.PlaySound(levelUpSound);
        level++;
        currentExperience -= maxExperience; // 남은 경험치는 다음 레벨로 이월

        // 레벨업 시 스탯 증가
        maxHealth += 50;
        maxMana += 50;
        maxExperience += 500;

        currentHealth = maxHealth;
        currentMana = maxMana;

        UIManager.Instance.LevelUpStatPlusAlert();
        UpdateUI();
    }

    public void UpdateUI()
    {
        UIManager.Instance.UpdateStats("level", level);
        UIManager.Instance.UpdateStats("exp", currentExperience, maxExperience);
        UIManager.Instance.UpdateStats("health", currentHealth, maxHealth);
        UIManager.Instance.UpdateStats("mana", currentMana, maxMana);
    }
}
