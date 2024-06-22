using System.Linq;
using TMPro;
using UnityEngine;

namespace Orbit_Character
{
	public partial class PlayerController
	{
        /* Gun */

        [Header("Gun")]
        public Transform GunRoot;
        public Transform FirePoint;
        public IKControl IKController;
        public Animator GunAnimator;
        public LayerMask BulletCollisionLayers = ~0;
        public int GunMagazineSize = 5;
        public float GunFireTime = 0.2f;
        public float GunReloadTime = 2.35f;
        public float DistanceFromBody = 0.3f;
        public TMP_Text GunUI;
        public GameObject AimImage_zoom;
        public GameObject AimImage_def;
        [SerializeField]
        private TipKey tipKey;

        [Header("Aiming")]
        public float ZoomFov = 30f;
        public float ZoomInTime = 0.2f;
        public float ZoomOutTime = 0.18f;

        private float _gunFireCooldown;
        private float _gunReloadCooldown;
        private int _gunMagazine;

        private float _gunBaseHeight;

        private Transform _gunLooker;

        private void InitializeGun()
        {
            _gunBaseHeight = GunRoot.localPosition.y;
            _gunLooker = new GameObject("GunLooker").transform;
            _gunLooker.SetParent(GunRoot);
            _gunLooker.position = GunRoot.position;
        }

        private void ResetAmmo()
        {
            _gunMagazine = GunMagazineSize;
        }

        private void GunAction(bool lockInput = false)
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                CinemachineVirtualCameraInstance.Instance.SetFov(ZoomFov, ZoomInTime);
                AimImage_zoom.SetActive(true);
                AimImage_def.SetActive(false);
            }
            else
            {
                CinemachineVirtualCameraInstance.Instance.ResetFov(ZoomOutTime);
                AimImage_zoom.SetActive(false);
                AimImage_def.SetActive(true);
            }

            if (GunAnimator != null)
            {

                if (_firstPerson || Quaternion.Angle(_cameraTarget.rotation, GunRoot.parent.rotation) < 50)
                {

                    var camForward = _cameraTarget.forward;
                    var rad = GetAngleBetweenAngles(_cameraTarget.eulerAngles.y, GunRoot.parent.eulerAngles.y) * Mathf.Deg2Rad;
                    GunRoot.localPosition = new Vector3(Mathf.Sin(rad / 2f) * DistanceFromBody, _gunBaseHeight + camForward.y * DistanceFromBody * 0.95f, Mathf.Cos(rad) * DistanceFromBody);
                    Vector3 point;
                    if (Physics.Raycast(_cameraTarget.position + camForward, camForward, out var hit, 50f) && hit.collider.gameObject != gameObject)
                    {
                        point = hit.point;
                    }
                    else
                    {
                        point = camForward * 100 + _cameraTarget.position;
                    }
                    _gunLooker.LookAt(point);
                    GunRoot.rotation = Quaternion.Lerp(GunRoot.rotation, _gunLooker.rotation, Time.deltaTime * 10);

                }

                IKController.IkActive = true;
                if ((_gunFireCooldown -= Time.deltaTime) > 0 || (_gunReloadCooldown -= Time.deltaTime) > 0)
                {
                    return;
                }
                if (_isOwner)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0) && _gunMagazine > 0 && !(CursorManager.Instance.PauseMenu))
                    {
                        FireBullet(0, FirePoint.position, FirePoint.forward);
                        return;
                    }
                }

                if (_reload || _gunMagazine <= 0) //리로드
                {
                    if (!(_gunMagazine == GunMagazineSize)) //완충돼있을 땐 재장전 X
                    { 
                        GunAnimator.Play(_animIDGunReload);
                        _gunReloadCooldown = GunReloadTime;
                        _gunMagazine = GunMagazineSize;
                    }
                }
            }
            else
            {
                IKController.IkActive = false;
            }
        }
        public void Success_Reload()
        {
            tipKey.gameObject.SetActive(false); //키가이드 없애고
            GunUI.text = "0" + _gunMagazine.ToString(); //잔탄 수 업데이트
        }


        private void FireBullet(ushort senderID, Vector3 origin, Vector3 direction, float penetration = 10f, float damage = 10f)
        {
            GunAnimator.Play(_animIDGunFire);

            _gunFireCooldown = GunFireTime;
            _gunMagazine--;
            GunUI.text = "0" + _gunMagazine.ToString();
            if (_gunMagazine <= 15)
            {
                tipKey.Reload();
            }
            else
            {
                tipKey.gameObject.SetActive(false);
            }

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
                    if (hits[i].collider.gameObject.layer == 0)
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