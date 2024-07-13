using UnityEngine;

/* 
 * 맵의 존 트리거에 닿았을 때 현재 존 출력 및 GameManager에 전달
 */
public class ZoneEntrance : MonoBehaviour
{
    public string zoneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.currentZoneName = zoneName;
            bool isLiberated = GameManager.Instance.IsZoneLiberated(zoneName);
            UIManager.Instance.UpdateZoneInfo(zoneName, isLiberated);
        }
    }
}
