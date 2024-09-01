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
	private float HealthPoints = 0f;
	[Header("몬스터 HP")]
	public float MaxPoints = 0f;
    [Header("몬스터가 주는 EXP")]
    public int expPoints = 100;
	public GameObject enemyUI;
	private EnemyUI enemyUIcanvas;
    private Slider hpSlider;
    private TMP_Text enemyName;

    private AudioSource audioSource;
    public AudioClip hitSound;

    [Header("죽었을 때 이벤트")]
    public UnityEvent OnDeath;
		
	private float _lastDamage;
	private int _lastDamageIndex;



    public bool Alive
	{
		get
		{
			return HealthPoints > 0f;
		}
	}

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        hpSlider = enemyUI.GetComponentInChildren<Slider>();
        enemyName = enemyUI.GetComponentInChildren<TMP_Text>();
        enemyUIcanvas = enemyUI.GetComponent<EnemyUI>();

        Setup();
    }

	public void Setup()
	{
		StopSound();
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
		else if (Alive)
		{
			PlaySound(hitSound);
			HealthPoints -= damage;
            enemyUIcanvas.ShowCanvasGroup();
            hpSlider.value = HealthPoints / MaxPoints;

            if (HealthPoints <= 0f)
			{
				HealthPoints = 0f;
				PlayerStats.Instance.GainExperience(expPoints);

				//온데스 이벤트
                OnDeath.Invoke();
                // 피 0되면 파괴 X → 오브젝트풀로 반환하는 이벤트 정의 ↑
                //Destroy(this.gameObject); 
            }
        }
	}

    public void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.Stop(); // 기존에 재생 중이던 사운드를 멈춤
            audioSource.clip = clip; // 새로운 클립 할당
            audioSource.Play(); // 새로운 클립 재생
        }
    }

	public void StopSound()
	{
        audioSource.Stop();
    }
}
