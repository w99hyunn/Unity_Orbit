using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace STARTING
{
    public enum PlayerState_Multi
    {
        IDLE,
        INIT,
        LOADING,
        DIE,
        PAUSE
    };

    public class PlayerStats_Multi : NetworkBehaviour
    {
        public static PlayerStats_Multi Instance;
        public static event System.Action OnPlayerStatsInitialized;

        public AudioClip levelUpSound;
        [SyncVar] public int maxHealth;
        [SyncVar] public int maxMana;
        [SyncVar] public int maxExperience;
        [SyncVar] public int currentHealth;
        [SyncVar] public int currentMana;
        [SyncVar] public int currentExperience;
        [SyncVar] public int level;

        [Header("플레이어 상태")]
        public PlayerState_Multi playerState;

        private float _manaRegenRate = 10f;
        private float _healthRegenRate = 5f;
        private float _regenInterval = 10f;
        private bool _isShowInfoUI;

        void Awake()
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
            playerState = PlayerState_Multi.INIT;
            InvokeRepeating("RegenerateMana", _regenInterval, _regenInterval);
            InvokeRepeating("RegenerateHealth", _regenInterval, _regenInterval);
            InitializeStats();
            OnPlayerStatsInitialized?.Invoke(); // 초기화 완료 이벤트 호출
        }

        [Command]
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
            StartCoroutine(ChangePlayerState(5f, PlayerState_Multi.IDLE));

            UpdateUI();
        }

        public IEnumerator ChangePlayerState(float time, PlayerState_Multi playerState)
        {
            yield return new WaitForSeconds(time);
            if (this.playerState == PlayerState_Multi.INIT)
            {
                this.playerState = playerState;
            }
        }

        public void ChangeState(float seconds, PlayerState_Multi playerState)
        {
            StartCoroutine(ChangePlayerStateAbsolute(seconds, playerState));
        }

        public IEnumerator ChangePlayerStateAbsolute(float seconds, PlayerState_Multi playerState)
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

            StartCoroutine(ChangePlayerState(5f, PlayerState_Multi.IDLE));
            UpdateUI();
        }

        public void Healing(int index)
        {
            currentHealth = Mathf.Min(currentHealth + index, maxHealth);
            UpdateUI();
        }

        void RegenerateHealth()
        {
            currentHealth = Mathf.Min(currentHealth + (int)_healthRegenRate, maxHealth);
            UpdateUI();
        }

        void RegenerateMana()
        {
            currentMana = Mathf.Min(currentMana + (int)_manaRegenRate, maxMana);
            UpdateUI();
        }

        [Command]
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

        [Command]
        public void GainExperience(int amount)
        {
            currentExperience += amount;
            if (currentExperience >= maxExperience)
            {
                LevelUp();
            }
            UpdateUI();
        }

        [Server]
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

        [ClientRpc]
        public void UpdateUI()
        {
            UIManager.Instance.UpdateStats("level", level);
            UIManager.Instance.UpdateStats("exp", currentExperience, maxExperience);
            UIManager.Instance.UpdateStats("health", currentHealth, maxHealth);
            UIManager.Instance.UpdateStats("mana", currentMana, maxMana);
        }

        
        public void OnCurrentInfo(InputValue value)
        {
            _isShowInfoUI = _isShowInfoUI ? false : true;
            UIManager.Instance.InfoUI(_isShowInfoUI);
        }
    }
}