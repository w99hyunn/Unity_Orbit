using UnityEngine;

public class DeadZone : MonoBehaviour
{
    PlayerStats playerStats;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerStats = other.GetComponent<PlayerStats>();
            playerStats.TakeDamage(-1);
        }
    }
}
