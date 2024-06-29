using UnityEngine;

public class Arete : MonoBehaviour
{
    public DungeonManager dungeonManager;

    public void Destroy_Arete()
    {
        dungeonManager.destroyArete = true;
        string currentZoneName = GameManager.Instance.currentZoneName;
        GameManager.Instance.LiberateZone(currentZoneName);

        // UI 업데이트
        bool isLiberated = GameManager.Instance.IsZoneLiberated(currentZoneName);
        UIManager.Instance.UpdateZoneInfo(currentZoneName, isLiberated);
    }
}
