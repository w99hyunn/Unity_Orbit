using UnityEngine;

public class ZoneEntrance : MonoBehaviour
{
    public string zoneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // UI 업데이트
            GameManager.Instance.currentZoneName = zoneName;
            bool isLiberated = GameManager.Instance.IsZoneLiberated(zoneName);
            UIManager.Instance.UpdateZoneInfo(zoneName, isLiberated);
        }
    }
}
