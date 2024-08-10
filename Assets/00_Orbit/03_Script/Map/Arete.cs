using UnityEngine;

public class Arete : MonoBehaviour
{
    public DungeonTimer dungeonTimer;

    public void Destroy_Arete()
    {
        dungeonTimer.destroyArete = true;
        string currentZoneName = GameManager.Instance.currentZoneName;
        GameManager.Instance.LiberateZone(currentZoneName);

        // UI 업데이트
        bool isLiberated = GameManager.Instance.IsZoneLiberated(currentZoneName);
        UIManager.Instance.UpdateZoneInfo(currentZoneName, isLiberated);
    }
}
