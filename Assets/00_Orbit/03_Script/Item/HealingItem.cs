using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealingItem : MonoBehaviour
{
    private bool isPlayerInRange = false;

    void Start()
    {
        // 15초 뒤에 오브젝트를 파괴
        Destroy(gameObject, 15f);
    }

    void Update()
    {
        // 오브젝트를 계속 회전시킴
        transform.Rotate(0, 0.5f, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Player 태그를 가진 오브젝트가 범위에 들어올 때
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            StartCoroutine(HealOverTime());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Player 태그를 가진 오브젝트가 범위를 벗어날 때
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    IEnumerator HealOverTime()
    {
        while (isPlayerInRange)
        {
            PlayerStats.Instance.Healing(10);
            yield return new WaitForSeconds(1f);
        }
    }
}


