using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _Debug : MonoBehaviour
{
    private CanvasGroup CanvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        CanvasGroup = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Tab))
        {
            CanvasGroup.alpha = 1;
        }
        else
        {
            CanvasGroup.alpha = 0;
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            PlayerStats.Instance.TakeDamage(20);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            PlayerStats.Instance.UseMana(20);
        }
        if (Input.GetKeyDown(KeyCode.N)) // N키를 눌러 경험치 얻기 테스트
        {
            PlayerStats.Instance.GainExperience(50);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            GameManager.Instance.SaveGame();
        }

        // 시간 관련 디버그
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SetDebugTime(6, 59, "오전");
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            SetDebugTime(6, 59, "오후");
        }

    }


    private void SetDebugTime(int hours, int minutes, string period)
    {
        // 오전/오후에 따른 시간 변환
        if (period == "오후" && hours != 12)
        {
            hours += 12;
        }
        else if (period == "오전" && hours == 12)
        {
            hours = 0;
        }

        UIManager.Instance.GameTime = hours * 3600 + minutes * 60;
        UIManager.Instance.UpdateGameTime();
    }
}
