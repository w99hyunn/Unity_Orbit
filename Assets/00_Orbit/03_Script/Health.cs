using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class Health : MonoBehaviour
{
	public Health Parent;
		
	[Space]
	public EfxManager.ImpactType MaterialType = EfxManager.ImpactType.Stone;
		
	public float PenetrationResistance = 0.5f;
	public float DamageMultiplier = 1f;
	public float HealthPoints = 0f;
	private float MaxPoints = 0f;

	public int expPoints = 100;
	public GameObject enemyUI;
	private EnemyUI enemyUIcanvas;
    private Slider hpSlider;
    private TMP_Text enemyName;

    public UnityEvent<ushort> OnDeath;
		
	// Only apply once per health family.
	private float _lastDamage;
	private int _lastDamageIndex;

	public bool Alive
	{
		get
		{
			return HealthPoints > 0f;
		}
	}

    private void Start()
    {
		MaxPoints = HealthPoints;

        hpSlider = enemyUI.GetComponentInChildren<Slider>();
		enemyName = enemyUI.GetComponentInChildren<TMP_Text>();
        enemyName.text = this.gameObject.name;
        enemyUIcanvas = enemyUI.GetComponent<EnemyUI>();
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
		else if (Alive)
		{
			HealthPoints -= damage;
            enemyUIcanvas.ShowCanvasGroup();
            hpSlider.value = HealthPoints / MaxPoints;

            if (HealthPoints <= 0f)
			{
				HealthPoints = 0f;
				PlayerStats.Instance.GainExperience(expPoints);

                Destroy(this.gameObject); // 피 0되면 파괴! 다른 로직도 추가하면될듯
            }
		}
	}
}
