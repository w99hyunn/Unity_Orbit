using UnityEngine;

namespace Orbit_Character
{
	public partial class PlayerController
	{
        /*Health*/
        [Header("Health")]
        public float MaxHealth = 20f;

        private Health _health;
        private int _lastSpawnIndex;

        private void InitializeHealth()
        {
            _health = GetComponent<Health>();
            if (_isOwner)
            {
                _health.OnDeath.AddListener(OnDeath);
                _health.HealthPoints = MaxHealth;
            }
        }

        private void OnDeath(ushort senderID)
        {
            if (_possesed)
            {
                CinemachineVirtualCameraInstance.Instance.gameObject.SetActive(false);
                CinemachineVirtualCameraInstance.Instance.Follow(null);
                if (_isHost)
                {
                }
            }

            _health.HealthPoints = MaxHealth;

            transform.position = Vector3.zero;
        }
    }
}