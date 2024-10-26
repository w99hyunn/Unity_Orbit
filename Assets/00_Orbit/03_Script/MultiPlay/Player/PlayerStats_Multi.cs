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
        public AudioClip levelUpSound;
        [SyncVar] public int maxHealth;
        [SyncVar] public int maxMana;
        [SyncVar] public int maxExperience;
        [SyncVar(hook = nameof(OnHealthChanged))] public int currentHealth;
        [SyncVar] public uint lastAttackerId;
        [SyncVar] public int currentMana;
        [SyncVar] public int currentExperience;
        [SyncVar] public int level;

        [Header("플레이어 상태")]
        public PlayerState_Multi playerState;

        private float _manaRegenRate = 10f;
        private float _healthRegenRate = 15f;
        private float _regenInterval = 5f;
        private bool _isShowInfoUI;

        private void Awake()
        {
            if (isLocalPlayer)
            {
                InitializeStats();
            }
        }

        private void Start()
        {
            if (isLocalPlayer)
            {
                InvokeRepeating("RegenerateMana", _regenInterval, _regenInterval);
                InvokeRepeating("RegenerateHealth", _regenInterval, _regenInterval);
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdTakeDamage(int amount, uint attackerId)
        {
            if (!isServer) return;

            currentHealth -= amount;
            lastAttackerId = attackerId;

            if (currentHealth <= 0)
            {
                currentHealth = 0;

                // 공격자의 NetworkIdentity 가져오기
                NetworkIdentity attackerIdentity = NetworkServer.spawned[attackerId];
                if (attackerIdentity != null)
                {
                    PlayerStats_Multi attackerStats = attackerIdentity.GetComponent<PlayerStats_Multi>();
                    if (attackerStats != null)
                    {
                        attackerStats.TargetShowKillLog(attackerStats.connectionToClient, gameObject.name);
                    }
                }
            }
        }

        [TargetRpc]
        public void TargetShowKillLog(NetworkConnection target, string enemyName)
        {
            GetComponent<Health_Multi>().KillLog(enemyName);
            GainExperience(GetComponent<Health_Multi>().expPoints);
        }

        void OnHealthChanged(int oldHealth, int newHealth)
        {
            if (isLocalPlayer && oldHealth > newHealth)
            {
                StartCoroutine(UIManager.Instance.FlashScreen());
                GameManager_Multi.Instance.SaveGamePartial("currentHealth", currentHealth);
            }

            if (newHealth <= 0)
            {
                currentHealth = 0;
                if (isLocalPlayer)
                {
                    GameManager_Multi.Instance.GameOver();
                }
            }

            UpdateUI();
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
            StartCoroutine(ChangePlayerState(5f, PlayerState_Multi.IDLE));

            UpdateUI();
            if (!isServer)
            {
                CmdUpdateStats(maxHealth, maxMana, maxExperience, health, mana, experience, level);
            }
        }

        //호스트의 클라이언트 플레이어 정보 업데이트
        [Command(requiresAuthority = false)]
        public void CmdUpdateStats(int newMaxHealth, int newMaxMana, int newMaxExperience, int newHealth, int newMana, int newExperience, int newLevel)
        {
            SetStats(newMaxHealth, newMaxMana, newMaxExperience, newHealth, newMana, newExperience, newLevel);
        }

        public IEnumerator ChangePlayerState(float time, PlayerState_Multi playerState)
        {
            this.playerState = PlayerState_Multi.INIT;
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
            GameManager_Multi.Instance.SaveGamePartial("currentHealth", currentHealth);
        }

        void RegenerateHealth()
        {
            currentHealth = Mathf.Min(currentHealth + (int)_healthRegenRate, maxHealth);
            UpdateUI();
            GameManager_Multi.Instance.SaveGamePartial("currentHealth", currentHealth);
        }

        void RegenerateMana()
        {
            currentMana = Mathf.Min(currentMana + (int)_manaRegenRate, maxMana);
            UpdateUI();
            GameManager_Multi.Instance.SaveGamePartial("currentMana", currentMana);
        }

        public void UseMana(int amount)
        {
            currentMana -= amount;
            if (currentMana < 0) currentMana = 0;
            UpdateUI();

            GameManager_Multi.Instance.SaveGamePartial("currentMana", currentMana);
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
            GameManager_Multi.Instance.SaveGamePartial("currentExperience", currentExperience);
        }

        void LevelUp()
        {
            GameManager_Multi.Instance.PlaySound(levelUpSound);
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

            GameManager_Multi.Instance.SaveGamePartial("level", level);
            GameManager_Multi.Instance.SaveGamePartial("maxHealth", maxHealth);
            GameManager_Multi.Instance.SaveGamePartial("maxMana", maxMana);
            GameManager_Multi.Instance.SaveGamePartial("maxExperience", maxExperience);
            GameManager_Multi.Instance.SaveGamePartial("currentHealth", currentHealth);
            GameManager_Multi.Instance.SaveGamePartial("currentMana", currentMana);
            GameManager_Multi.Instance.SaveGamePartial("currentExperience", currentExperience);
        }

        public void UpdateUI()
        {
            if (isLocalPlayer)
            {
                UIManager.Instance.UpdateStats("level", level);
                UIManager.Instance.UpdateStats("exp", currentExperience, maxExperience);
                UIManager.Instance.UpdateStats("health", currentHealth, maxHealth);
                UIManager.Instance.UpdateStats("mana", currentMana, maxMana);
            }
        }

        
        public void OnCurrentInfo(InputValue value)
        {
            _isShowInfoUI = _isShowInfoUI ? false : true;
            UIManager.Instance.InfoUI(_isShowInfoUI);
        }
    }
}