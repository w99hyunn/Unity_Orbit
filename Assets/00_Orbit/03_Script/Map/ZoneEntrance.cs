using UnityEngine;

namespace STARTING
{
    /// <summary>
    /// 맵의 존 트리거에 닿았을 때 출력 및 GameManager에 전달
    /// </summary>
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
}