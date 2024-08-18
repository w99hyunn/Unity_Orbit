using System.Linq;
using UnityEngine;

public class GunFire : MonoBehaviour
{
    public int maxBullet = 30;
    public int currentBullet;
    public float damage = 10f;

    public Transform firePoint;
    public AudioClip shootSound;
    public AudioClip clipOutSound;
    public AudioClip clipInSound;
    public AudioClip clipArmingSound;
    public AudioClip dryFireSound;

    public LayerMask BulletCollisionLayers = ~0 & ~(1 << 31);

    //무기 스왑시 UI업데이트
    private void OnEnable()
    {
        UIManager.Instance.UpdateCurrentBullet(currentBullet);
        UIManager.Instance.UpdateMaxBullet(maxBullet);

        //총 변경시 체크
        BulletCheck();
    }

    private void Awake()
    {
        currentBullet = maxBullet;
    }

    public void BulletCheck()
    {
        if (currentBullet <= (maxBullet / 3))
        {
            UIManager.Instance.tipKeyEnable("재장전", "R");
        }
        else
        {
            UIManager.Instance.tipKeyDisable();
        }
    }

    public void UseBullet()
    {
        Fire();
        GameManager.Instance.PlaySound(shootSound);
        currentBullet--;
        UIManager.Instance.UpdateCurrentBullet(currentBullet);
    }
    public void ReloadBullet()
    {
        currentBullet = maxBullet;
        BulletCheck();
        UIManager.Instance.UpdateCurrentBullet(currentBullet);
    }

    public void Fire()
    {
        FireBullet(0, firePoint.position, firePoint.forward, 10f, damage);
    }

    //EFX 효과는 Health의 MaterialType보고 결정됨. 없으면 디폴트 스톤값이 들어감. 레이어 0, 7이 아니면 궤적만표시
    private void FireBullet(ushort senderID, Vector3 origin, Vector3 direction, float penetration = 10f, float damage = 10f)
    {

        float currentPenetration = penetration;
        float hitDistance = 40f;

        RaycastHit[] hits = Physics.RaycastAll(origin, direction, hitDistance, BulletCollisionLayers);

        hits = hits.OrderBy(h => h.distance).ToArray();
        int l = hits.Length;

        for (int i = 0; i < l; i++)
        {
            if (hits[i].collider.TryGetComponent(out Health target))
            {
                EfxManager.Instance.PlayImpact(hits[i].point, hits[i].normal, hits[i].transform, target.MaterialType);

                float distanceDamageDropoff = 10f / (hits[i].distance + 10f);
                if ((currentPenetration - target.PenetrationResistance) / penetration * distanceDamageDropoff <= 0)
                {
                    DrawLine(i, Color.red);
                    target.TakeDamage(senderID, Mathf.Min(2 * currentPenetration / penetration, 1f) * damage * distanceDamageDropoff);
                    hitDistance = hits[i].distance;
                    break;
                }
                target.TakeDamage(senderID, currentPenetration / penetration * damage * distanceDamageDropoff);
                DrawLine(i, Color.yellow);
                currentPenetration -= target.PenetrationResistance;
            }
            else
            {
                if (hits[i].collider.gameObject.layer == 0 || hits[i].collider.gameObject.layer == 7)
                {
                    EfxManager.Instance.PlayImpact(hits[i].point, hits[i].normal, hits[i].transform);
                    currentPenetration -= 5f;

                    DrawLine(i, Color.grey);
                }
                else
                    DrawLine(i, Color.black);
            }
        }

        EfxManager.Instance.PlayBullet(origin, direction, hitDistance / 100f);

        void DrawLine(int i, Color color, float duration = 1f)
        {
            if (i == 0)
                Debug.DrawLine(origin, hits[i].point, color, duration);
            else
                Debug.DrawLine(hits[i - 1].point, hits[i].point, color, duration);
        }
    }
}
