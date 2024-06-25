using UnityEngine;

public class Arete : MonoBehaviour
{
    private void OnDestroy()
    {
        Debug.Log("파괴 호출");
        string currentZoneName = GameManager.Instance.currentZoneName;
        GameManager.Instance.LiberateZone(currentZoneName);

        // UI 업데이트
        bool isLiberated = GameManager.Instance.IsZoneLiberated(currentZoneName);
        UIManager.Instance.UpdateZoneInfo(currentZoneName, isLiberated);
        Debug.Log($"Zone {currentZoneName} has been liberated!");
    }
}
