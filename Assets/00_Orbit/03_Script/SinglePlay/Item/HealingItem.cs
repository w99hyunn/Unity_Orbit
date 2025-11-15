using System.Collections;
using UnityEngine;

namespace NOLDA
{
    public class HealingItem : MonoBehaviour
    {
        public int healPerSec = 10;

        private bool _isPlayerInRange = false;

        void Start()
        {
            Destroy(gameObject, 15f);
        }

        void Update()
        {
            transform.Rotate(0, 0.5f, 0);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerInRange = true;
                StartCoroutine(HealOverTime());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerInRange = false;
            }
        }

        IEnumerator HealOverTime()
        {
            while (_isPlayerInRange)
            {
                PlayerStats.Instance.Healing(healPerSec);
                yield return new WaitForSeconds(1f);
            }
        }
    }
}