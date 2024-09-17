using System.Collections;
using UnityEngine;

namespace STARTING
{
    public class HealingItem : MonoBehaviour
    {
        private bool isPlayerInRange = false;
        public int healPerSec = 10;

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
                isPlayerInRange = true;
                StartCoroutine(HealOverTime());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInRange = false;
            }
        }

        IEnumerator HealOverTime()
        {
            while (isPlayerInRange)
            {
                PlayerStats.Instance.Healing(healPerSec);
                yield return new WaitForSeconds(1f);
            }
        }
    }
}