using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace NOLDA
{
	public enum KillLogType
	{
		NORMAL,
		ARETE,
	}

	public class Health : MonoBehaviour
	{
		private Inventory inventory;

		public Health Parent;

		[Space]
		public EfxManager.ImpactType MaterialType = EfxManager.ImpactType.STONE;

		public float PenetrationResistance = 0.5f;
		public float DamageMultiplier = 1f;
		private float HealthPoints = 0f;

		[Header("몬스터 HP")]
		public float MaxPoints = 0f;

		[Header("몬스터가 주는 EXP")]
		public int expPoints = 100;

		[Header("몬스터 UI")]
		public GameObject enemyUI;
		private EnemyUI enemyUIcanvas;
		private Slider hpSlider;
		private TMP_Text enemyName;

		[Header("죽었을 때 이벤트")]
		public UnityEvent OnDeath;

		private float _lastDamage;
		private int _lastDamageIndex;

		[Header("죽었을 때 킬로그 종류")]
		public KillLogType killLogType = NOLDA.KillLogType.NORMAL;
		public Sprite deathIcon;

		[Header("칩 획득 확률")]
        [Range(0, 100)] public float probability = 15f;

        public bool _alive
		{
			get
			{
				return HealthPoints > 0f;
			}
		}

		private void Awake()
		{
			hpSlider = enemyUI.GetComponentInChildren<Slider>();
			enemyName = enemyUI.GetComponentInChildren<TMP_Text>();
			enemyUIcanvas = enemyUI.GetComponent<EnemyUI>();

			Setup();
		}

		public void Setup()
		{
			HealthPoints = MaxPoints;
			hpSlider.value = HealthPoints / MaxPoints;
			enemyName.text = this.gameObject.name;
		}

		public void TakeDamage(ushort senderID, float damage) => TakeDamage(senderID, damage, Time.frameCount);

		private void TakeDamage(ushort senderID, float damage, int damageIndex)
		{
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
				Parent.TakeDamage(senderID, damage);
			}
			else if (_alive)
			{
				HealthPoints -= damage;
                GameManager.Instance.EnemyHit();

                enemyUIcanvas.ShowCanvasGroup();
				hpSlider.value = HealthPoints / MaxPoints;

				if (HealthPoints <= 0f)
				{
					HealthPoints = 0f;
					PlayerStats.Instance.GainExperience(expPoints);

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
                    AchievementManager.Instance.UpdateAchievement("MonsterKill", 1);
                    if (randomValue <= probability)
					{
						inventory.GainChip();
                        AchievementManager.Instance.UpdateAchievement("IntactChipCollection", 1);
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