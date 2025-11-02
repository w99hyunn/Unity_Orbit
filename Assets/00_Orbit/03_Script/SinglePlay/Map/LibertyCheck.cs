using UnityEngine;

namespace STARTING
{
    public class LibertyCheck : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            ZoneEntrance zoneEntrance = other.GetComponent<ZoneEntrance>();

            if (zoneEntrance != null)
            {
                string zoneName = zoneEntrance.zoneName;
                bool _isZoneLiberated = GameManager.Instance.IsZoneLiberated(zoneName);

                if (_isZoneLiberated)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}