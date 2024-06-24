using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoSaveManager : MonoBehaviour
{
    public Button SaveButton; // PauseMenu > Save 버튼 클릭 이벤트 발생
    [Header("설정된 시간마다 자동저장(초 단위)")]
    public float interval = 180f;

    private void Start()
    {
        StartCoroutine(AutoClick());
    }

    private IEnumerator AutoClick()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            SaveButton.onClick.Invoke();
            Debug.Log("자동저장");
        }
    }
}
