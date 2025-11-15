using Mirror;
using System.Linq;
using UnityEngine;

namespace NOLDA
{
    public class GunFire_Multi : GunFire
    {
        public override void UseBullet()
        {
            Fire();
            GameManager_Multi.Instance.PlaySound(shootSound);
            currentBullet--;
            UIManager.Instance.UpdateCurrentBullet(currentBullet);
        }

        public override void Fire()
        {
            FireBullet(GetComponentInParent<NetworkIdentity>(), firePoint.position, firePoint.forward, 10f, damage);
            base.ShowMuzzleFlash();
        }

        /// <summary>
        /// EFX 효과는 Health의 MaterialType보고 결정됨. 없으면 디폴트 스톤값이 들어감. 레이어 0, 7이 아니면 궤적만표시
        /// </summary>
        /// <param name="senderID"></param>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="penetration"></param>
        /// <param name="damage"></param>
        public void FireBullet(NetworkIdentity attacker, Vector3 origin, Vector3 direction, float penetration = 10f, float damage = 10f)
        {

            float currentPenetration = penetration;
            float hitDistance = 40f;

            RaycastHit[] hits = Physics.RaycastAll(origin, direction, hitDistance, BulletCollisionLayers);

            hits = hits.OrderBy(h => h.distance).ToArray();
            int l = hits.Length;

            for (int i = 0; i < l; i++)
            {
                if (hits[i].collider.TryGetComponent(out Health_Multi target))
                {
                    EfxManager.Instance.PlayImpact(hits[i].point, hits[i].normal, hits[i].transform, target.MaterialType);

                    float distanceDamageDropoff = 10f / (hits[i].distance + 10f);
                    if ((currentPenetration - target.PenetrationResistance) / penetration * distanceDamageDropoff <= 0)
                    {
                        DrawLine(i, Color.red);
                        target.TakeDamage(attacker, Mathf.Min(2 * currentPenetration / penetration, 1f) * damage * distanceDamageDropoff);
                        hitDistance = hits[i].distance;
                        break;
                    }
                    target.TakeDamage(attacker, currentPenetration / penetration * damage * distanceDamageDropoff);
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
}