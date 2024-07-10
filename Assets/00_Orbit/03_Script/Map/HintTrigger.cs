using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintTrigger : MonoBehaviour
{
    private string currentZoneName;
    private bool isLiberated;

    private void OnTriggerEnter(Collider other)
    {
        currentZoneName = GameManager.Instance.currentZoneName;
        isLiberated = GameManager.Instance.IsZoneLiberated(currentZoneName);

        if (isLiberated == false && other.CompareTag("Player"))
        {
            UIManager.Instance.ScriptText_Enable("여신상의 뒷편이 수상해 보인다.");
        }
    }
}
