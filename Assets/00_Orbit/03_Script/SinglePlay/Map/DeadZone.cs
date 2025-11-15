using UnityEngine;

namespace NOLDA
{
    public class DeadZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerStats>().TakeDamage(-1);
            }
            else
            {
                Debug.Log("DeadZone¿¡ ÀÇÇØ ÆÄ±«µÊ: " + other.gameObject);
                Destroy(other.gameObject);
            }
        }
    }
}