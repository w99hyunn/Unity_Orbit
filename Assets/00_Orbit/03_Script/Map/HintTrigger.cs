using UnityEngine;

public class HintTrigger : MonoBehaviour
{
    public string infoText;
    private string currentZoneName;
    private bool isLiberated;

    private void OnTriggerEnter(Collider other)
    {
        currentZoneName = GameManager.Instance.currentZoneName;
        isLiberated = GameManager.Instance.IsZoneLiberated(currentZoneName);

        if (isLiberated == false && other.CompareTag("Player"))
        {
            UIManager.Instance.ScriptText_Enable(infoText);
        }
    }
}
