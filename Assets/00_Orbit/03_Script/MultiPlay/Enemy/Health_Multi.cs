using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace STARTING
{
	public enum MonsterType
	{
		MONSTER,
		HUMAN
	}

	public class Health_Multi : MonoBehaviour
	{
		private Inventory inventory;

		[Header("몬스터 타입")]
		public MonsterType monsterType = MonsterType.MONSTER;
        public PlayerStats_Multi playerStats;

        [Space]
        public Health_Multi Parent;

		[Space]
		public EfxManager.ImpactType MaterialType = EfxManager.ImpactType.STONE;
        public float PenetrationResistance = 0.5f;
        public float DamageMultiplier = 1f;

		[Header("몬스터 HP")]
		public float MaxPoints = 0f;

		[Header("몬스터가 주는 EXP")]
		public int expPoints = 100;

		[Header("몬스터 UI")]
		public GameObject enemyUI;
        public EnemyUI_Multi enemyUIcanvas;
        public Slider hpSlider;
        public TMP_Text enemyName;

		[Header("죽었을 때 이벤트")]
		public UnityEvent OnDeath;

		private float _lastDamage;
		private int _lastDamageIndex;

		[Header("죽었을 때 킬로그 종류")]
		public KillLogType killLogType = STARTING.KillLogType.NORMAL;
		public Sprite deathIcon;

		[Header("칩 획득 확률")]
        [Range(0, 100)] public float probability = 15f;

		public void SetName(string name)
		{
			enemyName.text = name;
        }

        public bool _alive
		{
			get
			{
				return playerStats.currentHealth > 0f;
			}
		}

        private void Start()
        {
            if (Parent == null)
            {
                playerStats = GetComponent<PlayerStats_Multi>();
                hpSlider = enemyUI.GetComponentInChildren<Slider>();
                enemyName = enemyUI.GetComponentInChildren<TMP_Text>();
                enemyUIcanvas = enemyUI.GetComponent<EnemyUI_Multi>();
                Setup();
            }
        }

        public void Setup()
		{
			//hpSlider.value = playerStats.currentHealth / playerStats.maxHealth;
			enemyName.text = this.gameObject.name;
		}

		public void TakeDamage(ushort senderID, float damage) => TakeDamage(senderID, damage, Time.frameCount);

		private void TakeDamage(ushort senderID, float damage, int damageIndex)
		{
			Debug.Log("테이크데미지");
			damage *= DamageMultiplier;

			// Check if damage is already applied.
			if (_lastDamageIndex == damageIndex)
			{
				// Undo last damage before applying new damage.
				if (damage > _lastDamage)
					TakeDamage(senderID, -_lastDamage, damageIndex);
				// If new damage is less than last damage, ignore.
				else return;
			}
			_lastDamage = damage;
			_lastDamageIndex = damageIndex;

			// apply damage
			if (Parent != null)
			{
				Debug.Log("ㅋㅋ");
				Parent.TakeDamage(senderID, damage);
			}
			else if (_alive)
			{
				playerStats.currentHealth -= (int)damage;
                GameManager_Multi.Instance.EnemyHit();

                enemyUIcanvas.ShowCanvasGroup();
				hpSlider.value = playerStats.currentHealth / MaxPoints;

				if (playerStats.currentHealth <= 0f)
				{
                    playerStats.currentHealth = 0;
                    playerStats.GainExperience(expPoints);

					//온데스 이벤트
					KillLog();
                    OnDeath.Invoke();
                    // 피 0되면 파괴 X → 오브젝트풀로 반환하는 이벤트 정의 ↑
                    //Destroy(this.gameObject); 
                }
			}
		}

		public void KillLog()
		{
            inventory = FindAnyObjectByType<Inventory>();

            switch (killLogType)
			{
				case KillLogType.NORMAL:
                    UIManager.Instance.ShowKillLog(enemyName.text);
                    float randomValue = Random.Range(0f, 100f);
					if (randomValue <= probability)
					{
						inventory.GainChip();
                    }
                    break;
                case KillLogType.ARETE:
                    UIManager.Instance.ShowKillLog(enemyName.text, 5f, "purple", deathIcon);
                    inventory.GainChip();
                    break;
            }
            
        }

	}
}