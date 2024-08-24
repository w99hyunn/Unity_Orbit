using UnityEngine;

public class LibertyCheck : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        ZoneEntrance zoneEntrance = other.GetComponent<ZoneEntrance>();
        if (zoneEntrance != null)
        {
            string zoneName = zoneEntrance.zoneName;
            bool isZoneLiberated = GameManager.Instance.IsZoneLiberated(zoneName);

            if (isZoneLiberated)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
