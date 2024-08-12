using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerList : MonoBehaviour
{
    // 게임 오브젝트 리스트
    public List<GameObject> gameObjects;

    void Start()
    {
        // 코루틴 실행
        StartCoroutine(ActivateObjectsAfterDelay(0.5f));
    }

    IEnumerator ActivateObjectsAfterDelay(float delay)
    {
        // 0.5초 대기
        yield return new WaitForSeconds(delay);

        // 리스트의 모든 게임 오브젝트 활성화
        foreach (GameObject obj in gameObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }
}
